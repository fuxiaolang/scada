using DESCADA.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DESCADA
{
    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class Alarm : System.Windows.Controls.UserControl
    {
        DataTable MyDataTable;

        public string currentMenu = "btnConfigOper";

        public Alarm()
        {
            try
            {

                InitializeComponent();
                //this.DataContext = new UiDesign.ViewModel.MainViewModel();

                _ = Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Show();
                    });
                });

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

                //do something here
                //this.Close();            
                //this.Hide();
                //Global.ChargerViewISShow = false;
                Global.CopyAsCsvHandler(this.MyDataGrid);
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }



        public void Show(bool isFromPage = false)
        {
            try
            {
                DBTrans dBTrans = new DBTrans();
                try
                {
                    string sqlHead = " SELECT *";
                    string sql = " from alarm  where 1=1 ";
                    if (txtDeviceNo.Text.Trim() != "")
                        sql += " and DeviceNo='" + txtDeviceNo.Text.Replace("'", "''") + "'";
                    if (FaultLevel.SelectedIndex > 0)
                        sql += " and FaultLevel=" + FaultLevel.SelectedIndex + "";
                    if (StartTime.SelectedDateTime != null)
                        sql += " and starttime>='" + StartTime.SelectedDateTime + "'";
                    if (EndTime.SelectedDateTime != null)
                        sql += " and EndTime<='" + EndTime.SelectedDateTime + "'";

                    sql += " order by AlarmID desc ";
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
                    Global.PromptFail("[Error]Alarm-Show" + ex.Message);
                    Global.WriteError("[Error]Alarm-Show" + ex.Message + ex.StackTrace);
                }
                finally
                {
                    dBTrans.Close();
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
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
            Show();
        }
    }

    [ValueConversion(typeof(Int16), typeof(string))]
    public class AlarmTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Int16 alarmType = (Int16)value;
            string strValue = "";
            switch (alarmType)
            {
                case 1:
                    strValue = "PLC/机器人";
                    break;
                case 2:
                    strValue = "充电机/电池";
                    break;
                case 3:
                    strValue = "TMS";
                    break;
                case 4:
                    strValue = "充电机";
                    break;
                case 5:
                    strValue = "消防系统";
                    break;
                case 6:
                    strValue = "其它故障";
                    break;
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
    public class DeviceLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Int16 alarmType = (Int16)value;
            string strValue = "";
            switch (alarmType)
            {
                case 1:
                    strValue = "一级";
                    break;
                case 2:
                    strValue = "二级";
                    break;
                case 3:
                    strValue = "三级";
                    break;

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
}
