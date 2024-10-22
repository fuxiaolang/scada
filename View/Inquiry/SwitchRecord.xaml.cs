using DESCADA.Service;
using Microsoft.VisualStudio.RpcContracts.Commands;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.DataFormats;

namespace DESCADA
{
    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class SwitchRecord : System.Windows.Controls.UserControl
    {
        DataTable MyDataTable;
        public SwitchRecord()
        {
            InitializeComponent();
            //this.DataContext = new UiDesign.ViewModel.MainViewModel();
            if (Global.saveRecordFromIndex == true)
            {
                this.chkManual.IsChecked = true;
                Global.saveRecordFromIndex = false;
            }
            else {
                this.chkManual.IsChecked = false;
            }
            //Show();


             Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Show();
                });
            });

       

        }


        private void export_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                Global.CopyAsCsvHandler(this.MyDataGrid);
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }

        public void Show(bool isFromPage = false)
        {
            DBTrans dBTrans = new DBTrans();
            string sql = "";
            try
            {
                string sqlHead = " SELECT VehInNO, thoroughfare, PlateNO, VehInTime, VehOutTime, StartTime, EndTime, RecordTime,";
                sqlHead += " PCPutNum, BatteryInSN,BatteryInSOC,BatteryInKWH,BatteryOutSN,BatteryOutSOC,BatteryOutKWH ";
                //sql += " concat(BatteryInSN, '/', BatteryInSOC, '/', BatteryInKWH) as BatteryIn,";
                // sql += " concat(BatteryOutSN, '/', BatteryOutSOC, '/', BatteryOutKWH) as BatteryOut";
                sql += " from switchrecord  where isdeleted=0 ";
                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";
                    switch (InqType.Text)
                    {
                        case "进站唯一编号":
                            inqField = "VehInNO";
                            break;
                        case "车牌号":
                            inqField = "PlateNO";
                            break;
                        case "用户号":
                            inqField = "CardNO";
                            break;
                        case "归还电池编码":
                            inqField = "BatteryInSN";
                            break;
                        case "借出电池编码":
                            inqField = "BatteryOutSN";
                            break;
                    }
                    sql += " and " + inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }

                if (this.chkManual.IsChecked==true)
                {
                    sql += " and createtime > NOW() - INTERVAL 48 HOUR";
                    sql += " and ( ";
                    sql += "         (createtime < NOW() - INTERVAL 20 MINUTE  and ISNULL(EndTime)=1)  ";
                    sql += "            or ";
                    sql += "         (NOW() > EndTime + INTERVAL 1 MINUTE  and ISNULL(EndTime)=0)  ";
                    sql += "     )";
                    sql += " and (ISNULL(BatteryOutSOC)=1 or ISNULL(BatteryInSOC)=1 or ifnull(BatteryOutSN, '')='' or ifnull(BatteryInSN, '')='' or ifnull(VehOutTime, '')='' or ifnull(StartTime, '')='' or ifnull(EndTime, '')='' ) ";
                }

                if (StartTime.SelectedDateTime != null)
                    sql += " and VehInTime>='" + StartTime.SelectedDateTime + "'";
                if (EndTime.SelectedDateTime != null)
                    sql += " and VehInTime<='" + EndTime.SelectedDateTime + "'";
                sql += "  order by switchrecordid desc  ";
                //sql += " LIMIT 0,100";
                //LIMIT @pageSize OFFSET(@page - 1) * @pageSize;
                int offset = (page - 1) * pageSize;
                string sqlLimit= " LIMIT "+pageSize+" OFFSET "+ offset;

                MyDataTable = dBTrans.GetDataTable(sqlHead+sql+ sqlLimit);
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
                Global.WriteError("[Error]switchrecord-Show" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("查询换电记录失败" + ex.Message);
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
        private void btnEidt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as TextBlock;
            DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
            int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);
            if (selectIndex == -1) return;
            MyDataGrid.CommitEdit();
            MyDataGrid.CommitEdit();
            string VehInNO = "";
            string sql = "";
            bool isFail = false;
            try
            {
                if (MyDataTable.Rows[selectIndex]["VehInNO"].ToString() == "")
                {
                    Global.PromptFail("进站唯一码不能为空");
                    isFail = true;
                    return;
                }
                VehInNO = MyDataTable.Rows[selectIndex]["VehInNO"].ToString().Replace("'", "''");
                string BatteryOutSN = MyDataTable.Rows[selectIndex]["BatteryOutSN"].ToString().Replace("'", "''");
                BatteryOutSN = (BatteryOutSN.Trim() == "") ? "null" : "'" + BatteryOutSN + "'";
                string BatteryOutSOC = MyDataTable.Rows[selectIndex]["BatteryOutSOC"].ToString().Replace("'", "''");
                BatteryOutSOC = (BatteryOutSOC.Trim() == "") ? "null" : BatteryOutSOC;
                string BatteryInSN = MyDataTable.Rows[selectIndex]["BatteryInSN"].ToString().Replace("'", "''");
                BatteryInSN = (BatteryInSN.Trim() == "") ? "null" : "'" + BatteryInSN + "'";
                string BatteryInSOC = MyDataTable.Rows[selectIndex]["BatteryInSOC"].ToString().Replace("'", "''");
                BatteryInSOC = (BatteryInSOC.Trim() == "") ? "null" : BatteryInSOC;
                string VehOutTime = MyDataTable.Rows[selectIndex]["VehOutTime"].ToString().Replace("'", "''");
                VehOutTime = (VehOutTime.Trim() == "") ? "null" : "'" + VehOutTime + "'";
                string StartTime = MyDataTable.Rows[selectIndex]["StartTime"].ToString().Replace("'", "''");
                StartTime = (StartTime.Trim() == "") ? "null" : "'" + StartTime + "'";
                string EndTime = MyDataTable.Rows[selectIndex]["EndTime"].ToString().Replace("'", "''");
                EndTime = (EndTime.Trim() == "") ? "null" : "'"+ EndTime + "'";

                sql = "update  switchrecord set ";
                sql += " BatteryOutSN=" + BatteryOutSN ;
                sql += ", BatteryOutSOC=" + BatteryOutSOC ;
                sql += ", BatteryInSN=" + BatteryInSN;
                sql += ", BatteryInSOC=" + BatteryInSOC ;
                sql += ", VehOutTime=" + VehOutTime ;
                sql += ", StartTime=" + StartTime ;
                sql += ", EndTime=" + EndTime ;
                sql += " where VehInNO='" + VehInNO + "'";

               // --update  switchrecord set  BatteryOutSN = ''13'', BatteryOutSOC = 99, BatteryInSN = ''12'', BatteryInSOC = 23, VehOutTime = null, StartTime = ''2024 / 7 / 2 10:14:33'', EndTime = ''2024 / 7 / 2 10:15:33'' where VehInNO = '120240702101433bzjg01'

                if (Global.dbTrans.ExcuteScript(sql))
                {
                    Global.PromptSucc("换电记录保存成功");
                    Global.AddEvent("补填唯一码"+ VehInNO + "的换电记录", 1);

                }
                else {
                    Global.PromptSucc("换电记录保存失败");
                }

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]Switchrecord-btnEidt_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("更新换电记录失败" + ex.Message);
            }
            finally
            {
                if (VehInNO == "" && !isFail)
                {
                    Show();
                    isAdding = false;
                }
            }

        }

        private void btnDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as TextBlock;
            DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
            int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);

            string sql = "";
            if (selectIndex == -1) return;
            try
            {
                string VehInNO = MyDataTable.Rows[selectIndex]["VehInNO"].ToString();
                sql = " update   switchrecord  set isdeleted=1 ";
                sql += " where VehInNO='" + VehInNO + "'";

                if (VehInNO != "")
                {
                    if (Global.dbTrans.ExcuteScript(sql))
                    {
                        Global.PromptSucc("删除换电记录成功");
                    }
                    else
                    {
                        Global.PromptFail("删除换电记录失败");
                    }
                }

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]Battery-btnBatteryDel_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("删除换电记录失败" + ex.Message);
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
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
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 当滚动变化时，同步列头滚动
            if (this.ColumnHeaderOverlay != null)
            {
                this.ColumnHeaderOverlay.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        // 这是一个辅助的ScrollViewer，用于同步列头的滚动
        private ScrollViewer ColumnHeaderOverlay { get; set; }

      
    }
}

