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
    public partial class Veh : System.Windows.Controls.UserControl
    {

        DataTable MyDataTable;

        public Veh()
        {
            InitializeComponent();
            this.DataContext = new UiDesign.ViewModel.MainViewModel();
            Show();

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
            isAdding = false;

            DBTrans dBTrans = new DBTrans();
            string sql = "";
            try
            {
                string sqlHead = " SELECT *";
                sql += " from veh  where isdeleted=0 ";

                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";
                    switch (InqType.Text)
                    {
                        case "车牌号":
                            inqField = "PlateNO";
                            break;
                        case "VIN":
                            inqField = "VIN";
                            break;
                    }
                    sql += " and " + inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }
                sql += "  order by vehid desc  ";
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
                Global.WriteError("[Error]veh-Show" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("查询车辆记录失败" + ex.Message);

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
        private void btnVehEidt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as TextBlock;
            DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
            //int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);
            // 获取当前选中行的索引
            int selectIndex = MyDataGrid.Items.IndexOf(MyDataGrid.SelectedItem);

            if (selectIndex == -1) return;
            MyDataGrid.CommitEdit();
            MyDataGrid.CommitEdit();
            string vehID = "";
            string sql = "";
            bool isFail = false;

            try
            {
                if (MyDataTable.Rows[selectIndex]["PlateNO"].ToString() == "")
                {
                    Global.PromptFail("请输入车牌号");
                    isFail = true;
                    return;
                }
                if (MyDataTable.Rows[selectIndex]["vin"].ToString() == "")
                {
                    Global.PromptFail("请输入VIN码");
                    isFail = true;
                    return;
                }
                vehID = MyDataTable.Rows[selectIndex]["VehID"].ToString().Replace("'", "''");

                string exists = Global.dbTrans.ExecuteScalar0("select count(*) from veh where vehid=" + vehID).ToString(); //*
                if (exists == "0" || vehID == "")
                {
                    sql = "insert into  veh (VIN,PlateNO,VehModel,OperateType,TransComp) values (";
                    sql += " '" + MyDataTable.Rows[selectIndex]["vin"].ToString().Replace("'", "''") + "'";
                    sql += ", '" + MyDataTable.Rows[selectIndex]["PlateNO"].ToString().Replace("'", "''") + "'";
                    sql += ", '" + MyDataTable.Rows[selectIndex]["VehModel"].ToString().Replace("'", "''") + "'";
                    sql += ", '" + MyDataTable.Rows[selectIndex]["OperateType"].ToString().Replace("'", "''") + "'";
                    sql += ", '" + MyDataTable.Rows[selectIndex]["TransComp"].ToString().Replace("'", "''") + "'";
                    sql += ")";
                }
                else
                {
                    sql = "update  veh set ";
                    sql += " VIN='" + MyDataTable.Rows[selectIndex]["vin"].ToString().Replace("'", "''") + "'";
                    sql += ", PlateNO='" + MyDataTable.Rows[selectIndex]["PlateNO"].ToString().Replace("'", "''") + "'";
                    sql += ", VehModel='" + MyDataTable.Rows[selectIndex]["VehModel"].ToString().Replace("'", "''") + "'";
                    sql += ", OperateType='" + MyDataTable.Rows[selectIndex]["OperateType"].ToString().Replace("'", "''") + "'";
                    sql += ", TransComp='" + MyDataTable.Rows[selectIndex]["TransComp"].ToString().Replace("'", "''") + "'";
                    sql += " where vehid=" + vehID;
                }
                Global.WriteDebugAsync("Veh edit" + sql, "Veh");
                Global.dbTrans.ExcuteScript(sql);
                Global.PromptSucc("车辆信息保存成功");
                if (exists == "0" || vehID == "")
                {
                    Show();
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]veh-btnVehEidt_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("更新车辆记录失败" + ex.Message);
            }
            finally
            {


                if (vehID == "" && !isFail)
                {
                    Show();
                    isAdding = false;
                }
            }

        }

        private void btnVehDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as TextBlock;
            DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
            int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);

            string sql = "";
            if (selectIndex == -1) return;
            try
            {
                string vehid = MyDataTable.Rows[selectIndex]["VehID"].ToString();
                // sql = " delete from   veh  ";
                sql = " update   veh  set isdeleted=1 ";
                sql += " where vehid=" + vehid;
                if (vehid != "")
                {
                    Global.dbTrans.ExcuteScript(sql);
                    Global.PromptSucc("删除车信息成功");
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]veh-btnVehDel_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("删除车记录失败" + ex.Message);
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
        private void btnAddVeh_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!isAdding)
                {
                    MyDataTable.Rows.InsertAt(MyDataTable.NewRow(), 0);
                    MyDataTable.Rows[0]["VehID"] = 0;
                }
                isAdding = true;
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


}
