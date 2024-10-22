using DESCADA.Service;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
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
    public partial class User : System.Windows.Controls.UserControl
    {

        DataTable MyDataTable;

        public User()
        {
            InitializeComponent();
            this.DataContext = new UiDesign.ViewModel.UserViewModel();
            Show();
            acl();

        }

        private void acl()
        {
            try
            {
                if (Global.Role == 3) this.btnAddUser.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }
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
                sqlHead += ", CASE Role WHEN 1 THEN '管理员'  WHEN 2   THEN '站长'    WHEN 3 THEN '站员' END AS RoleName ";
                sqlHead += ", CASE IsEnabled WHEN 1 THEN '正常'  WHEN 0   THEN '锁定'  END AS EnableStatus ";

                sql += " from user  where  1=1 "; //isEnabled=0

                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";
                    switch (InqType.Text)
                    {
                        case "姓名":
                            inqField = "Name";
                            break;
                        case "账号":
                            inqField = "Account";
                            break;
                    }
                    sql += " and " + inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }
                sql += "  order by userid desc  ";
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
                Global.WriteError("[Error]User-Show" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("查询账号记录失败" + ex.Message);

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
        private void btnUserEidt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var button = sender as TextBlock;
                DataGridRow row = Global.FindVisualParent<DataGridRow>(button);
                int selectIndex = MyDataGrid.Items.IndexOf(row.DataContext);
                if (selectIndex == -1) return;
                string UserID = MyDataTable.Rows[selectIndex]["UserID"].ToString().Replace("'", "''");

                UserEdit popupWindow = new UserEdit(this);
                popupWindow.UserID = UserID;
                popupWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Global.WriteError(" btnUserEidt_MouseDown" + ",error:" + ex.Message + "," + ex.StackTrace);
            }

        }


        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as TextBlock;
                UserEdit popupWindow = new UserEdit(this);
                popupWindow.UserID = "";
                popupWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Global.WriteError(" btnUserEidt_MouseDown" + ",error:" + ex.Message + "," + ex.StackTrace);
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
