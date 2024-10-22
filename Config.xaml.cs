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
    public partial class Config : System.Windows.Controls.UserControl
    {

        public string currentMenu = "btnConfigOper";

        public Config()
        {
            InitializeComponent();

            // ConfigOper pew = new ConfigOper();

            if (Global.pConfigOper == null)
            {
                Global.pConfigOper = new ConfigOper();
            }


            Page_Config.Content = Global.pConfigOper;
        }

        private void bntPlaneno1_Click(object sender, RoutedEventArgs e)
        {
            var ownerWindow = Window.GetWindow(this);
            double[] d = new double[2];
            d[0] = ownerWindow.Top + ownerWindow.Height / 2;
            d[1] = ownerWindow.Left + ownerWindow.Width / 2;


            Plateno popupWindow = new Plateno();
            //popupWindow.Parent = ownerWindow;
            popupWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            popupWindow.Top = d[0] - popupWindow.Height / 2;
            popupWindow.Left = d[1] - popupWindow.Width / 2;

            //WindowLocateHelper.Locate(popupWindow, ownerWindow, HorizontalLocate.CENTER, VerticalLocate.CENTER, 0, 0);
            //MainWindow mainWindow = new MainWindow();
            // Screen targetScreen = Screen.AllScreens[1] ;
            //System.Drawing.Rectangle viewport = targetScreen.WorkingArea;
            // mainWindow.Top = viewport.Top;
            //mainWindow.Left = viewport.Left;
            // mainWindow.Show();
            // popupWindow.Top = viewport.Top;
            //popupWindow.Left = viewport.Left;

            popupWindow.Top = 13;
            popupWindow.Left = -1906;
            //popupWindow.WindowState = WindowState.Maximized;



            if (Screen.AllScreens.Length > 1)
            {
                Screen s2 = Screen.AllScreens[1];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                popupWindow.Left = r2.Left;
                popupWindow.Top = r2.Top;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }
            else
            {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                popupWindow.Top = r1.Top;
                popupWindow.Left = r1.Left;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }



            popupWindow.ShowDialog();
        }


        private void bntPlaneno_Click(object sender, RoutedEventArgs e)
        {
            Plateno popupWindow = new Plateno();
            popupWindow.ShowDialog();
        }


        private void bntQRCode_Click(object sender, RoutedEventArgs e)
        {
            QRCode popupWindow = new QRCode();
            popupWindow.ShowDialog();
        }

        private void btnLeida_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnConfigOper_Click(object sender, RoutedEventArgs e)
        {
            ConfigOper pew = new ConfigOper();
            Page_Config.Content = pew;

            SetMenu("btnConfigOper");


        }

        public void bntConfigBasic_Click(object sender, RoutedEventArgs e)
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
                imageBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\圆角矩形2拷贝5-1.png", UriKind.Relative));
                imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式


                ImageBrush imageCurrentBrush = new ImageBrush();
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\手动充电按钮选中.png", UriKind.Relative));
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
