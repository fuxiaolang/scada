using DESCADA.Service;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using DESCADA.Service;
using DESCADA.Test;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Windows.Interop;

namespace DESCADA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool isOpen = false;
        //public StationConfig config;
        public string currentMenu = "btnMenuIndex";
        public MainWindow()
        {
            InitializeComponent();

            DESCADA.login frm = new login();
            frm.ShowDialog();

            init();

            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;


        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Global.showDBConnStatus(Global.DBConnStatus);
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确认要退出应用吗?", "确认", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true; // 取消关闭
            }
            else
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }

        private void init()
        {
            //App.Config= StationConfig.FromFile();
            //config = StationConfig.FromFile();

            Switch pew = new Switch(Global.config);
            Page_Change.Content = pew;

            DispatcherTimer showtimer = new DispatcherTimer();
            showtimer.Tick += Showtimer_Tick;
            showtimer.Interval = new TimeSpan(0, 0, 0, 1);
            showtimer.Start();

            //this.BorderAlert.Height = 0;
            StartAutoAlert();
            AlertTimer_Tick(null, null);
            //CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);

            PreviewKeyDown += Window1_PreviewKeyDown;
            StateChanged += MainWindow_StateChanged;

            //DESCADA.Test.frmChargerTest frm = new frmChargerTest();
            //frm.Show();

            //DESCADA.Test.frmVehTest frm1 = new frmVehTest();
            //frm1.Show();

            //DESCADA.Form1 frm = new Form1();
            //frm.Show();

            //DESCADA.KHT.KHT kHT = new KHT.KHT();
            //kHT.CallbackFuntion();



        }



        int k = 0;
        private void Showtimer_Tick(object sender, EventArgs e)
        {
            this.txtDate.Text = DateTime.Now.ToString(("yyyy MM dd"));
            this.txtTime.Text = DateTime.Now.ToString(("HH:mm:ss"));
            //if (k < 3) 
            //{ k++; }
            //else
            //{
            //    SetStatus();
            //    k = 0;
            //}

        }



        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            var state = ((MainWindow)sender).WindowState;

            if (state == WindowState.Normal)
            {
                // When escaping
                //ResizeMode = ResizeMode.CanResize;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
            else if (state == WindowState.Maximized)
            {
                // When maximizing
                // ResizeMode = ResizeMode.NoResize;
                WindowStyle = WindowStyle.None;
                //Topmost = true;//不能被其它窗体覆盖
            }
        }


        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="obj"></param>
        private void Window1_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {


                    case Key.Escape:

                        CloseWindow();
                        break;

                }
            }
        }


        private void CloseWindow()
        {
            this.Window1.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void btnMenuConfig_Click(object sender, RoutedEventArgs e)
        {
            //DESCADA.Service.LED led = new Service.LED();
            //led.ShowLEDPage();

            Config pew = new Config();
            Page_Change.Content = pew;

            SetMenu("btnMenuConfig");

        }

        private static Switch pSwitch = null; //singlton
        public void btnMenuIndex_Click(object sender, RoutedEventArgs e)
        {
            //Switch pew = new Switch(config);
            if (pSwitch == null)
            {
                pSwitch = new Switch(Global.config);
            }
            Page_Change.Content = pSwitch;
            SetMenu("btnMenuIndex");
        }

    

        private static Charge pCharge = null; //singlton
        private void btnMenuCharge_Click(object sender, RoutedEventArgs e)
        {
            if (pCharge == null)
            {
                pCharge = new Charge(Global.config);
            }
            Page_Change.Content = pCharge;

            SetMenu("btnMenuCharge");
        }

        private void btnMenuFire_Click(object sender, RoutedEventArgs e)
        {
            Fire fire = new Fire();
            Page_Change.Content = fire;

            SetMenu("btnMenuFire");
        }

        private void btnMenuLog_Click(object sender, RoutedEventArgs e)
        {

            //Track alarm = new Track();
            if (Global.pTrack == null)
            {
                Global.pTrack = new Track();
            }
            Page_Change.Content = Global.pTrack;

            SetMenu("btnMenuLog");
        }


        private void btnMenuInquiry_Click(object sender, RoutedEventArgs e)
        {
            Inquiry pew = new Inquiry();
            Page_Change.Content = pew;

            SetMenu("btnMenuInquiry");
        }

        private void SetMenu(string selMenu)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\导航栏选中.png", UriKind.Relative));
            imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式

            ((Button)this.FindName(currentMenu)).Background = null;
            ((Button)this.FindName(selMenu)).Background = imageBrush;

            currentMenu = selMenu;
            Window1_MouseDown(null, null);

        }

        public void StartAutoAlert()
        {
            DispatcherTimer AlertTimer = new DispatcherTimer();
            AlertTimer.Tick += AlertTimer_Tick;
            AlertTimer.Interval = new TimeSpan(0, 0, 0, 1);//4
            AlertTimer.Start();
        }

        //操作提示Alertlist 3s去除; 和3-4级告警warnlist 切换
        public void AlertTimer_Tick(object sender, EventArgs e)
        {
           // return;
            //清除超3S的历史提示  操作提示不需要轮播了，来一条显示一条
            if (Global.OperInfoStartShowTime != default(DateTime) && Global.DiffSeconds(Global.OperInfoStartShowTime, DateTime.Now) >= 3)
            {
                this.BorderInfo.Height = 0;  //操作
                Global.OperInfoStartShowTime = default(DateTime);
                Global.OperInfoMsg = null;
            }

            //3-4级故障轮询
            if (Global.Alertlist.Count == 0)
            {
                this.BorderAlert.Height = 0;  //34级故障
            }
            else
            {
                Global.AlertInfo alertInfo;
                if (Global.Alertlist.Any(x => x.showNum < 2))
                {
                    alertInfo = (Global.AlertInfo)Global.Alertlist.First(x => x.showNum < 2);
                    Global.Alertlist.Remove(alertInfo);
                    Global.Alertlist.Add(alertInfo);//放在列表末尾

                    this.txtAlert.Text = alertInfo.msg;
                    this.BorderAlert.Height = 89;
                    alertInfo.StartShowTime = DateTime.Now;
                    alertInfo.showNum++;

                }
                else {
                    this.BorderAlert.Height = 0;
                    return;
                }
            }

        }

        internal void Alert(string msg, int msgType, int showTime)
        {
            this.txtAlert.Text = msg;
        }

        private void Window1_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (Charge.chargerDetail != null)
            {
                Charge.chargerDetail.Hide();
                Global.ChargerViewISShow = false;
            }
        }

        private void btnDeviceStatus_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            string devices = "";
            string chargers = "";
            if (Global.PlcConnStatus != 1) devices += ",PLC";
            for (int i = 1; i < 8; i++)
            {
                if (Global.chargerConnStatus[i] != 1) devices += "，充电机" + i;
            }
            if (Global.LakiConnStauts != 1) devices += ",雷达";
            if (Global.LedConnStatus != 1) devices += ",引导屏";//显示
            if (Global.Kht1ConnStatus != 1) devices += ",车牌识别";
            if (devices != "")
            {
                devices = "以下设备未连接：\r\n" + devices.Substring(1);
            }
            else
            {
                msg = "设备连接正常";
            }

            msg += devices;
            Global.Dialog("设备连接状态", msg, "yes");

        }

        private void btnDBStaus_Click(object sender, RoutedEventArgs e)
        {
            string path = "DatabaseErrorLog.txt";
            if (System.IO.File.Exists(path))
            {
                System.Diagnostics.Process pro = new System.Diagnostics.Process();
                pro.StartInfo.FileName = "notepad.exe";
                pro.StartInfo.Arguments = path;
                pro.Start();
            }
        }

        private void btnSysLog_Click(object sender, RoutedEventArgs e)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\APP" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";


            if (System.IO.File.Exists(path))
            {
                System.Diagnostics.Process pro = new System.Diagnostics.Process();
                pro.StartInfo.FileName = "notepad.exe";
                pro.StartInfo.Arguments = path;
                pro.Start();
            }
        }
        private async void cmbUser_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                string selectedValue = selectedItem.Content.ToString();
                if (selectedValue == "退出登录")
                {
                    Global.AddEvent("登出", 4, Global.Account);

                    Global.Account = "";
                    Global.Role = 0;
                    Global.LogInType = 2;

                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            login popupWindow = new login();
                            popupWindow.ShowDialog();
                        });
                    });
                }
                else if (selectedValue == "切换账号")
                {
                    Global.LogInType = 3;
                    await Task.Run(() =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            login popupWindow = new login();
                            popupWindow.ShowDialog();
                        });
                    });
                }

                cmbUser.Items[0] = new ComboBoxItem { Content = Global.Account };
                cmbUser.SelectedIndex = 0;
            }

        }

        private bool isDragging = false;
        private void moveablePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            this.moveablePanel.CaptureMouse();
            this.moveablePanel.Cursor = Cursors.Hand;
        }

        private void moveablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point mousePos = e.GetPosition(null);
                this.moveablePanel.SetValue(Canvas.TopProperty, mousePos.Y - 50);
                this.moveablePanel.SetValue(Canvas.LeftProperty, mousePos.X - 100);
            }
        }

        private void moveablePanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            this.moveablePanel.ReleaseMouseCapture();
            this.moveablePanel.Cursor = null;
        }

        private void imgNewVersion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Global.pConfig == null)
            {
                Global.pConfig = new Config();
            }

            Page_Change.Content = Global.pConfig;

            Global.pConfig.bntConfigBasic_Click(null, null);

            SetMenu("btnMenuConfig");
        }


    }
}
