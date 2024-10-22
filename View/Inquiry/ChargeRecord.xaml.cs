using DESCADA.Service;
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
    public partial class ChargeRecord : System.Windows.Controls.UserControl
    {
        DataTable MyDataTable;

        public ChargeRecord()
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
            DBTrans dBTrans = new DBTrans();
            string sql = "";
            try
            {
                string sqlHead  = " SELECT *";
                sql += " from cmd222  where 1=1 ";
  
                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";
          

                    switch (InqType.Text)
                    {
                        case "充电流水号":
                            inqField = "ChargeSN";
                            break;
                        case "进站唯一编号":
                            inqField = "VehInNO";
                            break;
                        case "充电仓编号":
                            inqField = "ChargerNo";
                            break;
                        case "充电机编号":
                            inqField = "ChargerNo";
                            break;
                        case "电池编码":
                            inqField = "BattertySN";
                            break;
                        case "车架号":
                            inqField = "VIN";
                            break;
                    }
                    sql += " and "+ inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }

                if (StartTime.SelectedDateTime != null)
                    sql += " and ChargeStartTime>='" + StartTime.SelectedDateTime + "'";
                if (EndTime.SelectedDateTime != null)
                    sql += " and ChargeStartTime<='" + EndTime.SelectedDateTime + "'";
                sql += "  order by id desc  ";
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
                //System.Windows.MessageBox.Show(ex.Message);
                Global.WriteError("[Error]chargerecord-Show" + ex.Message + "\r\n" + ex.StackTrace+  "\r\n"+ sql);
                Global.PromptFail("查询充电记录失败"+ ex.Message );

            }
            finally {
                dBTrans.Close();
            }
   
        }

        private void BtnInquiry_Click(object sender, RoutedEventArgs e)
        {
            Show();
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
