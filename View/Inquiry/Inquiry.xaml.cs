using System;
using System.Collections.Generic;
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
    public partial class Inquiry : System.Windows.Controls.UserControl
    {

        public string currentMenu = "btnSwitchRecord";

        public Inquiry()
        {
            InitializeComponent();

           SwitchRecord pew = new SwitchRecord();
           Page_Config.Content = pew;
        }



        private void btnSwitchRecord_Click(object sender, RoutedEventArgs e)
        {


            SwitchRecord pew = new SwitchRecord();
            Page_Config.Content = pew;

            SetMenu("btnSwitchRecord");


        }


        private void bntChargeRecord_Click(object sender, RoutedEventArgs e)
        {
            ChargeRecord pew = new ChargeRecord();
            Page_Config.Content = pew;

            SetMenu("bntChargeRecord");


        }

        private void bntVeh_Click(object sender, RoutedEventArgs e)
        {
            Veh pew = new Veh();
            Page_Config.Content = pew;

            SetMenu("bntVeh");


        }

        private void bntBattery_Click(object sender, RoutedEventArgs e)
        {
            Battery pew = new Battery();
            Page_Config.Content = pew;

            SetMenu("bntBattery");


        }

        private void bntChargeState_Click(object sender, RoutedEventArgs e)
        {
            ChargeState pew = new ChargeState();
            Page_Config.Content = pew;

            SetMenu("bntChargeState");


        }
        private void bntBatteryRecord_Click(object sender, RoutedEventArgs e)
        {
            BatteryRecord pew = new BatteryRecord();
            Page_Config.Content = pew;

            SetMenu("bntBatteryRecord");


        }
        private void bntUnitRecord_Click(object sender, RoutedEventArgs e)
        {
            UnitRecord pew = new UnitRecord();
            Page_Config.Content = pew;

            SetMenu("bntUnitRecord");


        }
        private void bntEnergy_Click(object sender, RoutedEventArgs e)
        {
            Energy pew = new Energy();
            Page_Config.Content = pew;

            SetMenu("bntEnergy");


        }

        private void bntUser_Click(object sender, RoutedEventArgs e)
        {
            User pew = new User();
            Page_Config.Content = pew;

            SetMenu("bntUser");


        }
        

        private void bntConfigBasic_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.txtAlert.Text = "保存成功";
            }

            //ConfigBasic pew = new ConfigBasic();

            if (Global.pConfigBasic == null)
            {
                Global.pConfigBasic = new ConfigBasic();
            }

            Page_Config.Content = Global.pConfigBasic;
            SetMenu("bntConfigBasic");

        }

        private void SetMenu(string selMenu)
        {
            try
            {

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources/圆角矩形2拷贝5-1.png", UriKind.Relative));
                imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式


                ImageBrush imageCurrentBrush = new ImageBrush();
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources/手动充电按钮选中.png", UriKind.Relative));
                imageCurrentBrush.Stretch = Stretch.Fill;//设置图像的显示格式


                ((System.Windows.Controls.Button)this.FindName(currentMenu)).Background = imageCurrentBrush;
                ((System.Windows.Controls.Button)this.FindName(selMenu)).Background = imageBrush;

                currentMenu = selMenu;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }
    }
}
