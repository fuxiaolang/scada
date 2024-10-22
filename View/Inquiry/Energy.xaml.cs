using DESCADA.Service;
using MySqlX.XDevAPI.Relational;
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
    public partial class Energy : System.Windows.Controls.UserControl
    {

        public Energy()
        {
            InitializeComponent();
            this.DataContext = new UiDesign.ViewModel.MainViewModel();
            //Show();

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

        public void Show()
        {
            DBTrans dBTrans = new DBTrans();
            string sql = "";
            string sql1 = "";
            Double ChargeTtl = 0, NoChargeTtl = 0;
            try
            {
                if (StartTime.SelectedDateTime == null || EndTime.SelectedDateTime == null)
                {
                    Global.PromptFail("请选择统计区间！");
                    return;
                }
                sql += " SELECT   chargerno, gunno,IFNULL(max(CurrentMeterNum)- min(CurrentMeterNum), 0)  CurrentMeterNum from cmd104 ";
                sql += "   where  createtime BETWEEN  '" + StartTime.SelectedDateTime + "' and  '" + EndTime.SelectedDateTime + "'" ;
                sql += "    group by chargerno, gunno order by chargerno, gunno ";
                DataTable dt = dBTrans.GetDataTable(sql);
                MyDataGrid.ItemsSource = dt.DefaultView;
                Double TtlCurrentMeterNum = 0;
                foreach (DataRow row in dt.Rows)
                {
                    TtlCurrentMeterNum += Double.Parse(row["CurrentMeterNum"].ToString());
                }
                txtTotal2.Text = TtlCurrentMeterNum.ToString("N3") + "kwh";

                sql1 += " SELECT IFNULL( max(absorbWattHour)- min(absorbWattHour), 0) absorbWattHour ";
                sql1 += "  , IFNULL( max(releaseWattHour)- min(releaseWattHour), 0) releaseWattHour ";
                sql1 += "  ,IFNULL(  max(induReactiveEnergy)- min(induReactiveEnergy), 0) induReactiveEnergy  ";
                sql1 += "   , IFNULL( max(capaReactiveEnergy)- min(capaReactiveEnergy), 0) capaReactiveEnergy  ";
                sql1 += "  from plcw4 ";
                sql1 += "   where  createtime BETWEEN  '" + StartTime.SelectedDateTime + "' and  '" + EndTime.SelectedDateTime + "'";

                DataTable dt1 = dBTrans.GetDataTable(sql1);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    this.txtDetail1.Text = dt1.Rows[0]["absorbWattHour"].ToString();
                    this.txtDetail2.Text = dt1.Rows[0]["releaseWattHour"].ToString();
                    this.txtDetail3.Text = dt1.Rows[0]["induReactiveEnergy"].ToString();
                    this.txtDetail4.Text = dt1.Rows[0]["capaReactiveEnergy"].ToString();
                    NoChargeTtl = Double.Parse(this.txtDetail1.Text) + Double.Parse(this.txtDetail2.Text) + Double.Parse(this.txtDetail3.Text) + Double.Parse(this.txtDetail4.Text);
                    txtTotal3.Text = NoChargeTtl.ToString("N3") + "kwh";
                }

                Double ttl = TtlCurrentMeterNum + NoChargeTtl;
                txtTotal1.Text = ttl.ToString("N3") + "kwh";

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

  
    }




}
