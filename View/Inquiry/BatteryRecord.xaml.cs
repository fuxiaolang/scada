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
    public partial class BatteryRecord : System.Windows.Controls.UserControl
    {
        DataTable MyDataTable;

        public BatteryRecord()
        {
            InitializeComponent();

            //this.DataContext = new UiDesign.ViewModel.MainViewModel();
            Task.Run(() =>
           {
               this.Dispatcher.Invoke(async () =>
               {
                   await Show();
               });
           });


        }

        private void export_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Global.CopyAsCsvHandler(this.MyDataGrid);

        }

        public async Task<string> Show(bool isFromPage = false)
        {
            DBTrans dBTrans = new DBTrans();
            string sql = "";
            try
            {
                string sqlHead = "SELECT BatteryHighU,PackTtlI,BMSWarn2,BMSWarn2,BMSWarn2,PackPositive,UnitCellUMax,UnitCellUMin,UnitCellCMaxNo,UnitCellCMinNo,";
                sqlHead += "concat(AGunC1,'/',AGunC2) as  AGunC,concat(BGunC1,'/',BGunC2) as  BGunC ,TmsWorkStatus,TmsEffluentC,TMSFaultLevel,ChargerNo,CreateTime";
                sql += " from cmd116  where 1=1 ";

                if (txtInq.Text.Trim() != "")
                {
                    string inqField = "";


                    switch (InqType.Text)
                    {

                        case "充电仓编号":
                            inqField = "ChargerNo";
                            break;
                        case "充电机编号":
                            inqField = "ChargerNo";
                            break;
                        case "电池编码":
                            inqField = "BattertySN";
                            break;
                    }
                    sql += " and " + inqField + "='" + txtInq.Text.Replace("'", "''") + "'";
                }

                if (StartTime.SelectedDateTime != null)
                    sql += " and CreateTime>='" + StartTime.SelectedDateTime + "'";
                if (EndTime.SelectedDateTime != null)
                    sql += " and CreateTime<='" + EndTime.SelectedDateTime + "'";
                sql += "  order by id desc  ";
                //LIMIT @pageSize OFFSET(@page - 1) * @pageSize;
                int offset = (page - 1) * pageSize;
                string sqlLimit = " LIMIT " + pageSize + " OFFSET " + offset;

                MyDataTable = dBTrans.GetDataTable(sqlHead + sql + sqlLimit);
                MyDataGrid.ItemsSource = MyDataTable.DefaultView;


                Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        string strCount = ctlPage.RecordSize.ToString();
                        if (isFromPage == false)
                        {
                            strCount = dBTrans.ExecuteScalar0("select count(*) " + sql).ToString(); //id
                        }
                        ctlPage.RecordSize = Int32.Parse(strCount);
                    });
                });

                return "";

            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.Message);
                Global.WriteError("[Error]Batteryrecord-Show" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("查询电池记录失败" + ex.Message);
                return "";
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
    public class BattModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strTmp = value.ToString();
            string strValue = "";

            Byte[] bytes = Global.strToToHexByte(strTmp);
            int intTmp = Global.chargerServer.getBattMode(bytes);

            //CMD116:16:Byte3:bit4~3充电状态:0|2-监控；1-充电中；3-故障
            switch (intTmp)
            {
                case 0:
                    return "监控";
                case 2:
                    return "监控";
                case 1:
                    return "充电中";
                case 3:
                    return "故障";
                default:
                    return "NA";
            }
            return strValue;
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
    public class BalStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                string strTmp = value.ToString();
                string strValue = "";

                Byte[] bytes = Global.strToToHexByte(strTmp);
                int intTmp = Global.chargerServer.getBalStatus(bytes);

                //CMD116:16:Byte2:bit7-BMS当前均衡状态：0-未均衡；1-均衡
                switch (intTmp)
                {
                    case 0:
                        return "未均衡";
                    case 1:
                        return "均衡";
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
    public class fltRnkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

                string strTmp = value.ToString();
                string strValue = "";

                Byte[] bytes = Global.strToToHexByte(strTmp);
                int intTmp = Global.chargerServer.getfltRnk(bytes);

                //CMD116:16:Byte3:bit6~5最高报警等级：0-无故障；1-一级轻微故障；2-二级普通故障；3-三级严重故障；
                switch (intTmp)
                {
                    case 0:
                        return "无故障";
                    case 1:
                        return "一级轻微故障";
                    case 2:
                        return "二级普通故障";
                    case 3:
                        return "三级严重故障";
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
