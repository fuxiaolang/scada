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
    public partial class UnitRecord : System.Windows.Controls.UserControl
    {
        DataTable MyDataTable;

        public UnitRecord()
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
                string sqlHead = " SELECT ChargerNo,EnableFlag,VIN,BattertySN,ChargerStatus,GunWorkStatus,CreateTime";
                sql += " from UnitRecord  where 1=1 ";

                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";


                    switch (InqType.Text)
                    {

                        case "充电仓编号":
                            inqField = "ChargerNo";
                            break;
                        case "电池编码":
                            inqField = "BattertySN";
                            break;
                        case "车架号":
                            inqField = "VIN";
                            break;
                    }
                    sql += " and " + inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }

                if (StartTime.SelectedDateTime != null)
                    sql += " and CreateTime>='" + StartTime.SelectedDateTime + "'";
                if (EndTime.SelectedDateTime != null)
                    sql += " and CreateTime<='" + EndTime.SelectedDateTime + "'";
                sql += "  order by unitrecordid desc,ChargerNo desc  ";

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
                Global.WriteError("[Error]unitrecord-Show" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("查询仓位记录失败" + ex.Message);
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


    [ValueConversion(typeof(Int16), typeof(string))]
    public class GunWorkStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string strTmp = value.ToString();
                string strValue = "";

                switch (strTmp)
                {
                    case "0":
                        return "空闲中";
                    case "1":
                        return "正准备开始充电";
                    case "2":
                        return "充电进行中";
                    case "3":
                        return "充电结束";
                    case "4":
                        return "启动失败";
                    case "5":
                        return "预约状态";
                    case "6":
                        return "系统故障(不能给汽车充电)";
                    case "7":
                        return "暂停服务";
                    default:
                        return "NA";
                }
                return strValue;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return "";

            }


        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string price = (string)value;
                if (decimal.TryParse(price, System.Globalization.NumberStyles.Any, culture, out decimal result))
                {
                    return result;
                }
                return value;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return "";

            }

        }

 
    }


    [ValueConversion(typeof(Int16), typeof(string))]
    public class ChargerStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string strTmp = value.ToString();
                string strValue = "";

                switch (strTmp)
                {
                    case "0":
                        return "离线";
                    case "1":
                        return "占用";
                    case "2":
                        return "空闲";
                    case "3":
                        return "监控";
                    case "4":
                        return "充电";
                    case "5":
                        return "故障";
                    case "6":
                        return "火警告警";
                    default:
                        return "NA";
                }
                return strValue;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return "";

            }


        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string price = (string)value;
                if (decimal.TryParse(price, System.Globalization.NumberStyles.Any, culture, out decimal result))
                {
                    return result;
                }
                return value;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return "";

            }
        }
    }


}
