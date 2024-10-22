using DESCADA.Service;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.DataFormats;

namespace DESCADA
{
    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class Battery : System.Windows.Controls.UserControl
    {

        DataTable MyDataTable;

        public Battery()
        {
            InitializeComponent();
            this.DataContext = new UiDesign.ViewModel.MainViewModel();
            Show();

        }


        private void export_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Global.CopyAsCsvHandler(this.MyDataGrid);

        }

        public void Show(bool isFromPage = false)
        {
            isAdding = false;
            DBTrans dBTrans = new DBTrans();
            string sql = "";
            try
            {
                string sqlHead = " SELECT *";
                sql += " from Battery  where  isdeleted=0 ";

                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";
                    switch (InqType.Text)
                    {
                        case "电池编码":
                            inqField = "BatteryNo";
                            break;
                        case "电池类型":
                            inqField = "BatteryType";
                            break;
                    }
                    sql += " and " + inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }
                sql += "  order by BatteryID desc  ";
                //LIMIT @pageSize OFFSET(@page - 1) * @pageSize;
                int offset = (page - 1) * pageSize;
                string sqlLimit = " LIMIT " + pageSize + " OFFSET " + offset;

                MyDataTable = dBTrans.GetDataTable(sqlHead + sql + sqlLimit);
                MyDataGrid.ItemsSource = MyDataTable.DefaultView;

                string strCount = ctlPage.RecordSize.ToString();
                if (isFromPage == false)
                {
                    strCount = dBTrans.ExecuteScalar0("select count(*) " + sql).ToString(); //id
                }
                ctlPage.RecordSize = Int32.Parse(strCount);
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]Battery-Show" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("查询电池记录失败" + ex.Message);

            }
            finally
            {
                dBTrans.Close();
            }

        }

        private void BtnInquiry_Click(object sender, RoutedEventArgs e)
        {
            Show();
        }

        public bool isAdding = false; //添加中
        private void btnBatteryEidt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as TextBlock;
            DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
            int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);
            if (selectIndex == -1) return;
            MyDataGrid.CommitEdit();
            MyDataGrid.CommitEdit();
            string BatteryID = "";
            string sql = "";
            bool isFail = false;
            try
            {
                if (MyDataTable.Rows[selectIndex]["BatterySn"].ToString() == "")
                {
                    Global.PromptFail("请输入电池编码");
                    isFail = true;
                    return;
                }
                BatteryID = MyDataTable.Rows[selectIndex]["BatteryID"].ToString().Replace("'", "''");
                string KWH = MyDataTable.Rows[selectIndex]["KWH"].ToString().Replace("'", "''");
                KWH = (KWH == "") ? "null" : KWH;

                string exists = Global.dbTrans.ExecuteScalar0("select count(*) from Battery where Batteryid=" + BatteryID).ToString(); //*
                if (exists == "0" || BatteryID == "")
                {
                    sql = "insert into  Battery (BatterySn,BatteryType,KWH,CheckTime) values (";
                    sql += " '" + MyDataTable.Rows[selectIndex]["BatterySn"].ToString().Replace("'", "''") + "'";
                    sql += ", '" + MyDataTable.Rows[selectIndex]["BatteryType"].ToString().Replace("'", "''") + "'";
                    sql += ", " + KWH + "";

                    if (MyDataTable.Rows[selectIndex]["CheckTime"].ToString().Trim() == "")
                        sql += ",null";
                    else
                        sql += ", '" + MyDataTable.Rows[selectIndex]["CheckTime"].ToString().Replace("'", "''") + "'";
                    sql += ")";
                }
                else
                {
                    sql = "update  Battery set ";
                    sql += " BatterySn='" + MyDataTable.Rows[selectIndex]["BatterySn"].ToString().Replace("'", "''") + "'";
                    sql += ", BatteryType='" + MyDataTable.Rows[selectIndex]["BatteryType"].ToString().Replace("'", "''") + "'";
                    sql += ", KWH=" + KWH + "";
                    if (MyDataTable.Rows[selectIndex]["CheckTime"].ToString().Trim() == "")
                        sql += ", CheckTime=null";
                    else
                        sql += ", CheckTime='" + MyDataTable.Rows[selectIndex]["CheckTime"].ToString().Replace("'", "''") + "'";
                    sql += " where Batteryid=" + BatteryID;
                }
                Global.dbTrans.ExcuteScript(sql);

                Global.PromptSucc("电池信息保存成功");
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]Battery-btnBatteryEidt_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("更新入网电池失败" + ex.Message);
            }
            finally
            {
                if (BatteryID == "" && !isFail)
                {
                    Show();
                    isAdding = false;
                }
            }

        }

        private void btnBatteryDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as TextBlock;
            DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
            int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);

            string sql = "";
            if (selectIndex == -1) return;
            try
            {
                string BatteryID = MyDataTable.Rows[selectIndex]["BatteryID"].ToString();
                //sql = " delete from   Battery  ";
                sql = " update   Battery  set isdeleted=1 ";
                sql += " where Batteryid=" + BatteryID;

                if (BatteryID != "")
                {
                    Global.dbTrans.ExcuteScript(sql);
                    Global.PromptSucc("删除电池信息成功");
                }

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]Battery-btnBatteryDel_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("删除入网电池失败" + ex.Message);
            }
            finally
            {
                Show();
            }

        }

        private void MyDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {

                e.Row.Header = e.Row.GetIndex() + 1;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }

        private void btnAddBatery_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!isAdding) MyDataTable.Rows.InsertAt(MyDataTable.NewRow(), 0);
                isAdding = true;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }

        int selectedIndex;
        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            try
            {

                if (e.Column.Header.ToString() == "操作") // 假设删除按钮在列头名为"删除"的列
                {
                    selectedIndex = e.Row.GetIndex();
                    // 在这里处理删除逻辑，使用selectedIndex作为参考
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        int page = 1; // 当前页码
        int pageSize = 20; // 每页显示的记录数
        int recordSize = 0;
        private void Pagination_SendDataEvent(object sender, Controls.DataEventArgs e)
        {
            this.page = e.Page;
            this.pageSize = e.PageSize;
            Show(true);
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool visibility = (bool)value;
                string parameterString = parameter as string;
                if (parameterString == "HiddenWhenTrue" && visibility || parameterString == "VisibleWhenTrue" && !visibility)
                {
                    return Visibility.Hidden;
                }
                return Visibility.Visible;
            }
            catch (Exception ex)
            {
                return Visibility.Hidden;
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Hidden;
        }
    }
}
