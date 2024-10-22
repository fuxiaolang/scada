using ControlzEx.Standard;
using DESCADA.Models;
using DESCADA.Service;
using MahApps.Metro.Controls;
using Microsoft.VisualStudio.RpcContracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static DESCADA.Service.Charger;
using static DESCADA.Service.PLC;
using static DESCADA.Switch;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace DESCADA
{
    /// <summary>
    /// Charge.xaml 的交互逻辑
    /// </summary>
    public partial class Charge : System.Windows.Controls.UserControl
    {
        public StationConfig myconfig;
        public DispatcherTimer showtimer1;
        public DispatcherTimer showtimer2;

        //public HttpClient httpClient;
        public Local local;
        public byte GunChargerID=1;

        public Charge(StationConfig config)
        {
            InitializeComponent();
            myconfig = config;
            //Show();
            //SetCharger();
            //SetbtnCharger();
            showtimer1 = new DispatcherTimer();
            //showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Interval = new TimeSpan(0, 0, 0, 0, 3000);
            showtimer1.Start();
            Showtimer1_Tick(null, null);

            showtimer2 = new DispatcherTimer();
            showtimer2.Tick += Showtimer2_Tick;
            showtimer2.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            showtimer2.Start();
            Showtimer2_Tick(null, null);
        }
        private void Showtimer1_Tick(object sender, EventArgs e)
        {
            SetCharger();
            SetbtnCharger();
            SetOverview();
            SetGun();
            Show();

        }
        private void Showtimer2_Tick(object sender, EventArgs e)
        {
            SetbtnGun();
        }



        public static ChargerDetail chargerDetail = null; //singlton
        public static bool ChargerDetailClicked = false;

        private void Charger_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string btnName = (sender as FrameworkElement).Name;
            string chargerID = btnName.Replace("Border", "").Replace("Grid", "");
            if (chargerID == "") return;
            if (Global.SelChargerViewModel.ChargerID.ToString() == chargerID && chargerID!="0") return;

            ImageBrush imageCurrentBrush = (ImageBrush)this.FindName("imgBackGround" + chargerID);
            imageCurrentBrush.Opacity = 0.6;
            string imgPath = imageCurrentBrush.ImageSource.ToString();//.Replace(@"pack://application:,,,/DESCADA;component/", "");
            imageCurrentBrush.ImageSource = new BitmapImage(new Uri(imgPath.Replace("_默认", "_选中和鼠标悬浮"), UriKind.RelativeOrAbsolute));
     
        }

        private void Charger_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string btnName = (sender as FrameworkElement).Name;
            string chargerID = btnName.Replace("Border", "").Replace("Grid", "");
            if (chargerID == "") return;
            if (Global.SelChargerViewModel.ChargerID.ToString() == chargerID && chargerID != "0") return;
            
             ImageBrush imageCurrentBrush = (ImageBrush)this.FindName("imgBackGround" + chargerID);
            imageCurrentBrush.Opacity = 1;

            string imgPath = imageCurrentBrush.ImageSource.ToString(); //.Replace(@"pack://application:,,,/DESCADA;component/", "");
            if (imgPath.StartsWith("pack:") ){
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(imgPath.Replace("_选中和鼠标悬浮", "_默认"), UriKind.Absolute));
            }
            else {
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(imgPath.Replace("_选中和鼠标悬浮", "_默认"), UriKind.Relative));
            }

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(7);
        }

        private void Charger7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(7);
        }
        private void Charger6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(6);
        }
        private void Charger5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(5);
        }
        private void Charger4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(4);
        }
        private void Charger3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(3);
        }
        private void Charger2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(2);
        }
        private void Charger1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(1);
        }
        private void Charger0_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChargerViewModel(0);
        }

        public void SetChargerViewModel(int charger)
        {
            Global.SelChargerViewModel.ChargerID= charger;

            ImageBrush imageBrush = (ImageBrush)this.FindName("imgBackGround" + charger);
            //pack://application:,,,/DESCADA;component/Resources/charge/充电仓/电池仓_电量不足_默认.png
            string imgPath = imageBrush.ImageSource.ToString();//.Replace(@"pack://application:,,,/DESCADA;component/", "");
           // var bitmapFrame = (BitmapFrame)imageBrush.GetValue(ImageBrush.ImageSourceProperty);
           // string relativePath = ((Uri)(bitmapFrame as BitmapMetadata).GetQuery("/"))?.ToString() ?? string.Empty;
           // string relativePath = ((Uri)(imageBrush.ImageSource as BitmapFrame)?.Decoder?.ToString())?.OriginalString;
           imageBrush.ImageSource = new BitmapImage(new Uri(imgPath.Replace("_默认", "_选中和鼠标悬浮"), UriKind.RelativeOrAbsolute));
            for (int i = 1; i < 8; i++)  //带两把枪的桩号 默认是1号桩
            {
                if (i == charger) continue;
                ImageBrush imageCurrentBrush = (ImageBrush)this.FindName("imgBackGround" + i);
                string imgCurrentPath = imageCurrentBrush.ImageSource.ToString();//.Replace(@"pack://application:,,,/DESCADA;component/", "");
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(imgCurrentPath.Replace("_选中和鼠标悬浮", "_默认"), UriKind.RelativeOrAbsolute));
            }
     

            //switch (charger)
            //{
            //    case 7:
            //        Global.SelChargerViewModel.ChargerID = 7;
            //        Global.SelChargerViewModel.ChargerNo = "07027145A";
            //        Global.SelChargerViewModel.BatteryNo = "07XPE14QC5123ZCAP1101430";
            //        break;
            //    case 3:
            //        Global.SelChargerViewModel.ChargerID = 3;
            //        Global.SelChargerViewModel.ChargerNo = "03027145A";
            //        Global.SelChargerViewModel.BatteryNo = "03XPE14QC5123ZCAP1101430";
            //        break;

            //}
            //var source = e.OriginalSource as FrameworkElement;
            //if (source == null)
            //    return;
            //System.Windows.MessageBox.Show(source.Name);

            if (chargerDetail == null)
            {
                chargerDetail = new ChargerDetail();
            }
            if (Global.ChargerViewISShow == false)
            {
                chargerDetail.Show();
                Global.ChargerViewISShow = true;
            }
            ChargerDetailClicked = true;


        }


        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.MessageBox.Show("ok");
            if (chargerDetail != null && ChargerDetailClicked == false)
            {
                chargerDetail.Hide();
                Global.ChargerViewISShow = false;
            }
            ChargerDetailClicked = false;


        }

        //3S
        System.Windows.Media.BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
        void SetCharger()
        {
            int ChargerStopCount = 0; //封仓总数
            int BatteryCount = 0;      //电池总数
            int freeChargerCount = 0;  //空闲
            int ChargingCount=0; //充电中
            int ChargeFinishCount= 0; //充满

            if(Global.PlcConnStatus==0) BattertySN0.Text = "--";
            else if (Global.PLCMsg4.locationCheck0 == 1) BattertySN0.Text = "有电池"; 
            else BattertySN0.Text = "无电池";


            if (Global.ChargerEnableFlag[0] == 0) txtChargerStopStatus0.Text = "停用"; else txtChargerStopStatus0.Text = "";

            for (int i = 1; i < 8; i++)
            {
                //不在文字上显示封仓状态，只在背景图上提现，否则会覆盖其它并行的状态
                string tmpchargerStatus = "";
                TextBlock txtChargerStopStatus = (TextBlock)this.FindName("txtChargerStopStatus"+i);
                Border ChagerHeadBorder = (Border)this.FindName("ChagerHeadBorder" + i);

                if (Global.ChargerEnableFlag[i] == 0)
                {
                    tmpchargerStatus = "停用"; txtChargerStopStatus.Text = "停用"; ChargerStopCount++;
                }
                else {
                    txtChargerStopStatus.Text = "";
                }
                ImageBrush imgBackGround = (ImageBrush)this.FindName("imgBackGround" + i);
                if (tmpchargerStatus == "停用")
                {
                    imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
                    ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#A9CDEF");

                }

                TextBlock txtBattertySN = (TextBlock)this.FindName("BattertySN" + i);
                TextBlock txtBatteryHighU = (TextBlock)this.FindName("BatteryHighU" + i);
                TextBlock txtPackTtlI = (TextBlock)this.FindName("PackTtlI" + i);
                TextBlock txtPackSoc = (TextBlock)this.FindName("PackSoc" + i);
                System.Windows.Controls.Image imgChargerStatus = (System.Windows.Controls.Image)this.FindName("imgChargerStatus" + i);
                TextBlock txtChargerStatus = (TextBlock)this.FindName("txtChargerStatus" + i);
                System.Windows.Controls.Image imgSmoke = (System.Windows.Controls.Image)this.FindName("imgSmoke" + i);
                TextBlock txSmoke = (TextBlock)this.FindName("txtSmoke" + i);
                System.Windows.Controls.Image imgGas = (System.Windows.Controls.Image)this.FindName("imgGas" + i);
                TextBlock txtGas = (TextBlock)this.FindName("txtGas" + i);
                System.Windows.Controls.Image imgFire = (System.Windows.Controls.Image)this.FindName("imgFire" + i);
                TextBlock txtFire = (TextBlock)this.FindName("txtFire" + i);
                TextBlock txtFault = (TextBlock)this.FindName("txtFault" + i);
                System.Windows.Controls.Image imgFault = (System.Windows.Controls.Image)this.FindName("imgFault" + i);

                if (Global.SqlCmd104_1[i] == null || Global.SqlCmd116[i] == null
                     || Global.SqlCmd104_1[i].Parameters.Count == 0 || Global.SqlCmd116[i].Parameters.Count == 0)
                {
                    txtBattertySN.Text = "--"; txtBattertySN.Foreground = new SolidColorBrush(Colors.White);
                    txtBatteryHighU.Text = "--"; txtBatteryHighU.Foreground = new SolidColorBrush(Colors.White);
                    txtPackTtlI.Text = "--"; txtPackTtlI.Foreground = new SolidColorBrush(Colors.White);
                    txtPackSoc.Text = "--"; txtPackSoc.Foreground = new SolidColorBrush(Colors.White);
                    txtChargerStatus.Text = "--"; txtChargerStatus.Foreground = new SolidColorBrush(Colors.White);
                    txSmoke.Text = "--"; txSmoke.Foreground = new SolidColorBrush(Colors.White);
                    txtGas.Text = "--"; txtGas.Foreground = new SolidColorBrush(Colors.White);
                    txtFire.Text = "--"; txtFire.Foreground = new SolidColorBrush(Colors.White);
                    txtFault.Text = "--"; txtFault.Foreground = new SolidColorBrush(Colors.White);
                    imgChargerStatus.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgSmoke.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgGas.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgFire.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgFault.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
                    ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#A9CDEF");
                    continue;
                }

                string BattertySN = Global.SqlCmd116[i].Parameters["@BattertySN"].Value.ToString();
                txtBattertySN.Text = BattertySN;

                //充电仓总览区电压值 CMD116:24:电池端高压 BatteryHighU_010
                string BatteryHighU = Global.SqlCmd116[i].Parameters["@BatteryHighU"].Value.ToString();
                txtBatteryHighU.Text = BatteryHighU;

                //充电仓总览区电流值   CMD116: 19:电池包总电流 PackTtlI_003
                string PackTtlI = Global.SqlCmd116[i].Parameters["@PackTtlI"].Value.ToString();
                txtPackTtlI.Text = PackTtlI;

                //充电仓总览区+充电仓详情区域：SOC	CMD116:17:电池包SOC PackSoc_020
                string PackSoc = Global.SqlCmd116[i].Parameters["@PackSoc"].Value.ToString();
                txtPackSoc.Text = PackSoc;

                //充电仓状态
                string VehConnectStatus = Global.SqlCmd104_1[i].Parameters["@VehConnectStatus"].Value.ToString();

                string WorkStatus1 = "", WorkStatus2 = "", WorkStatus3 = "", WorkStatus4 = "";
                 if (Global.SqlCmd104_1[i]!=null && Global.SqlCmd104_1[i].Parameters["@WorkStatus"]!=null)
                    WorkStatus1= Global.SqlCmd104_1[i].Parameters["@WorkStatus"].Value.ToString();
                if (Global.SqlCmd104_2[i] != null && Global.SqlCmd104_2[i].Parameters["@WorkStatus"] != null)
                    WorkStatus2 = Global.SqlCmd104_2[i].Parameters["@WorkStatus"].Value.ToString();
                if (Global.SqlCmd104_3[i] != null && Global.SqlCmd104_3[i].Parameters["@WorkStatus"] != null)
                    WorkStatus3 = Global.SqlCmd104_3[i].Parameters["@WorkStatus"].Value.ToString();
                if (Global.SqlCmd104_4[i] != null && Global.SqlCmd104_4[i].Parameters["@WorkStatus"] != null)
                    WorkStatus4 = Global.SqlCmd104_4[i].Parameters["@WorkStatus"].Value.ToString();

                bool chargerIsConnected=  Global.chargerServer.ChargerIsConnected(i);
                TypeInfo type = typeof(PLC.PLCW4).GetTypeInfo();
                FieldInfo stringFieldInfo = type.GetField("locationCheck" + i);
                string locationCheck = stringFieldInfo.GetValue(Global.PLCMsg4).ToString();

                string chargerStatus = "";
                string faultStatus = "正常";

                if (chargerIsConnected)
                {
                    Global.chargerConnStatus[i] = 1;
                }

                if (chargerIsConnected == false)
                {
                    chargerStatus = "离线"; Global.ChargerStatus[i] = -1;
                }
                //else if(WorkStatus3=="2" || WorkStatus4=="2")
                //{
                //    chargerStatus = "占用"; Global.ChargerStatus[i] = 1;
                //}
                else if (WorkStatus1 == "2" || WorkStatus2 == "2")
                {
                    chargerStatus = "充电"; Global.ChargerStatus[i] = 4; ChargingCount++;
                }
                else if (locationCheck == "0")
                {
                    chargerStatus = "空闲"; Global.ChargerStatus[i] = 2; freeChargerCount++;
                }
                else if (locationCheck == "1")
                {
                    chargerStatus = "监控"; Global.ChargerStatus[i] = 3;
                }

                //byte warnbytes = Global.cmd116.BMSWarn2_2[2];
                //string strWarn = Convert.ToString(warnbytes, 2);
                //byte[] byteArray = new byte[] { warnbytes };
                //string strWarn = string.Join("", Array.ConvertAll(byteArray, b => Convert.ToString(b, 2).PadLeft(8, '0')));

                int BMSWarnLevel = Global.chargerServer.getfltRnk(Global.cmd116.BMSWarn2_2); //strWarn[1]; //最高报警等级
                int chargestatus = Global.chargerServer.getChargeStatus(Global.cmd116.BMSWarn2_2); //strWarn[3];充电状态

                if (WorkStatus1 == "6" || WorkStatus2== "6" || WorkStatus3 == "6" || WorkStatus4 == "6" || chargestatus == 3 || BMSWarnLevel != 0)
                {
                    //CMD104:7=6或CMD116:16:Byte3:bit4~3=3或CMD116:16:Byte3:bit6~5≠0
                    faultStatus = "故障";
                    imgFault.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                    txtFault.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FF4A3B");

                    Global.ChargerFaultStatus[i] = 1; 
                }
                else {
                    faultStatus = "正常";
                    imgFault.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    txtFault.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#00FDFA");

                    Global.ChargerFaultStatus[i] = 0;
                }
                txtFault.Text = faultStatus;

                txtChargerStatus.Text = chargerStatus;
                if (chargerStatus == "空闲" || chargerStatus == "监控" || chargerStatus == "插枪")
                {
                    imgChargerStatus.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#00FDFA");
                }
                else
                {
                    imgChargerStatus.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/等待.png", UriKind.Relative));
                    txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FFBE2E");

                }


                //System.Windows.Controls.Image imgBackGround = (System.Windows.Controls.Image)this.FindName("imgBackGround" + i);
                if (tmpchargerStatus == "停用")
                {
                    //imgBackGround.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
                }
                else if (faultStatus == "故障")
                {
                    imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_异常_默认.png", UriKind.Relative));
                    ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FF4A3B");
                    txtFault.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FF4A3B");
                }
                else {
                    if (chargerStatus == "空闲" || chargerStatus == "监控")
                    {
                        imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_电量充足_默认.png", UriKind.Relative));
                        ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#00FDFA");
                        txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#00FDFA");

                    }
                    else if (chargerStatus == "占用" || chargerStatus == "充电")
                    {
                        imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_电量不足_默认.png", UriKind.Relative));
                        ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FFBE2E");
                        txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FFBE2E");
                    }
                    else if (chargerStatus == "离线" || chargerStatus == "停用")
                    {
                        imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
                        ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#A9CDEF");
                        txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FFFFFF");
                    }
                }

                //烟感: 充电仓总览区：烟雾（告警）	CMD104:60|61|CMD116:16:Byte8:bit1，或逻辑 有一个异常就算异常
                int smokeWran = 0;
                string GunSmoke1 = Global.SqlCmd104_1[i].Parameters["@GunSmoke1"].Value.ToString();
                string GunSmoke2 = Global.SqlCmd104_1[i].Parameters["@GunSmoke2"].Value.ToString();
                byte bytes = Global.cmd116.BMSWarn2_2[7];
                string str = Convert.ToString(bytes, 2);
                int BMSWarn = str[6];
                if (GunSmoke1 == "1" || GunSmoke2 == "1" || BMSWarn == 1) smokeWran = 1;
                if (smokeWran == 1)
                {
                    imgSmoke.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                    txSmoke.Text = "异常";
                }
                else
                {
                    imgSmoke.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    txSmoke.Text = "异常";
                }

                //气体 充电仓总览区：气体（告警）	CMD104:58|59，或逻辑
                int gasWarn = 0;
                string GunGas1 = Global.SqlCmd104_1[i].Parameters["@GunGas1"].Value.ToString();
                string GunGas2 = Global.SqlCmd104_1[i].Parameters["@GunGas2"].Value.ToString();
                if (GunGas1 == "1" || GunGas1 == "1") gasWarn = 1;
                if (gasWarn == 1)
                {
                    imgGas.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                    txtGas.Text = "异常";
                }
                else
                {
                    imgGas.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    txtGas.Text = "正常";
                }

                //火警 充电仓总览区：火警（告警）+充电仓详情区：消防预警级别	
                //CMD104:62:消防预警级别|CMD116:16:Byte8:bit2火灾报警:0为正常，非0为告警
                int fireWarn = str[5];//0：正常 1：故障
                if (fireWarn == 1)
                {
                    imgFire.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                    txtFire.Text = "异常";
                    Global.ChargerFireStatus[i] = 1;
                }
                else
                {
                    imgFire.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    txtFire.Text = "正常";
                    Global.ChargerFireStatus[i] = 0;
                }

                //overview
                //电池总数 PLC W4 locationCheck 汇总
                //空闲  充电仓状态：空闲
                //充满  配置SOC  116 电池SOC
                if (locationCheck == "1") BatteryCount++;
                int intPackSoc = 0; bool PackSocIsInt = false;
                //string PackSoc = Global.SqlCmd116[i].Parameters["@PackSoc"].Value.ToString();
                if (PackSoc != null) PackSocIsInt = int.TryParse(PackSoc, out intPackSoc);
                if (PackSocIsInt == true && intPackSoc >= Global.config.NomalConfig.ChargeExTactics.StopSoc)
                {
                    ChargeFinishCount++;
                }

            }

            txtChargersCount.Text = Global.config.NomalConfig.ChargerNetNum.ToString(); //电池仓总数
            txtChargerStopCount.Text = ChargerStopCount.ToString(); //封仓数
            txfreeChargerCount.Text = freeChargerCount.ToString();  //空闲
            txtChargingCount.Text = ChargingCount.ToString(); //充电中
            txChargeFinishCount.Text = ChargeFinishCount.ToString(); //充满
            txtBatteryCount.Text = BatteryCount.ToString();      //电池总数

        }

        //暂未用
        string Get116Para(int ChargerID,string ParaName)
        {
            string rtn = "--";

            if (Global.SqlCmd116[ChargerID] != null && Global.SqlCmd116[ChargerID].Parameters.Count != 0)
            {
                rtn = Global.SqlCmd116[ChargerID].Parameters["@" + ParaName].Value.ToString();
            }
            return rtn;
        }

        //10S
        void SetOverview()
        {
            //仓位总数来自站基础配置信息
            //充电/空闲/充满状态来自充电机上报的电池状态(充满的判断条件来自运营配置参数满电soC值
            //电池总数根据充电机状态及仓位电池传感器信息综合判断
            //封仓数量根据对仓位的封仓(停用)指令计算
            //电池仓及充电仓热管理信息(含电池仓温度和各散热风扇运行状态)来源为PLC报文Msg #4;

            //电池仓温度1

            if (Global.PlcConnStatus == 0)
            {
                txttempLocation1.Text = "--";
                txttempLocation2.Text = "--";
                txtFanBatteryCharger1.Text = "--";
                txtFanBatteryCharger2.Text = "--";

                txtFan1.Text = "--";
                txtFan2.Text = "--";
                txtFan3.Text = "--";
                txtFan4.Text = "--";

            }
            else { 
            txttempLocation1.Text = Global.PLCMsg4.tempLocation1.ToString();
            txttempLocation2.Text = Global.PLCMsg4.tempLocation2.ToString();
            txtFanBatteryCharger1.Text = getFanStatus(Global.PLCMsg4.FanBatteryCharger1.ToString());
            txtFanBatteryCharger2.Text = getFanStatus(Global.PLCMsg4.FanBatteryCharger2.ToString());

            txtFan1.Text = getFanRunStatus(Global.PLCMsg4.Fan1.ToString());
            txtFan2.Text = getFanRunStatus(Global.PLCMsg4.Fan2.ToString());
            txtFan3.Text = getFanRunStatus(Global.PLCMsg4.Fan3.ToString());
            txtFan4.Text = getFanRunStatus(Global.PLCMsg4.Fan4.ToString());
           }

        }

        //哪个桩有两个枪
        /*            充电枪区域：VIN显示 CMD104:46:车辆VIN码 VIN_1
        充电枪区域：是否显示车辆图标 CMD104:10:车辆连接状态（2状态显示有车辆）	VehConnectStatus
        充电枪区域：电压 / 电流 / SOC / 剩余时长    CMD104: 14 / 15 / 8 / 25   ChargeU_010 ChargeI_010 CurrentSOC RemainChargeTime
        充电枪区域：充电枪温度 CMD104:45   GunC_002
        */
        void SetGun()
        {
                for (int i = 1; i < 8; i++)  //带两把枪的桩号 默认是1号桩
                {
                if (Global.SqlCmd104_3[i] != null && Global.SqlCmd104_3[i].Parameters.Count > 0)
                {
                    GunChargerID = (Byte)i;
                    string WorkStatus = Global.SqlCmd104_3[i].Parameters["@WorkStatus"].Value.ToString();
                    GunInfo1.Text = Global.transGunWorkStatus(WorkStatus);
                    string VehConnectStatus1 = Global.SqlCmd104_3[i].Parameters["@VehConnectStatus"].Value.ToString(); //0-	断开 1-半连接 2-连接
                    if (VehConnectStatus1 == "2")
                        imgGunConn1.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电枪/充电连接-已连接.png", UriKind.Relative));
                    else
                        imgGunConn1.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电枪/充电连接-未连接.png", UriKind.Relative));
                    VIN1.Text = Global.SqlCmd104_3[i].Parameters["@VIN"].Value.ToString();
                    ChargeU1.Text = Global.SqlCmd104_3[i].Parameters["@ChargeU"].Value.ToString();
                    ChargeI1.Text = Global.SqlCmd104_3[i].Parameters["@ChargeI"].Value.ToString();
                    CurrentSOC1.Text = Global.SqlCmd104_3[i].Parameters["@CurrentSOC"].Value.ToString();
                    RemainChargeTime1.Text = Global.SqlCmd104_3[i].Parameters["@RemainChargeTime"].Value.ToString();
                    GunC1.Text = Global.SqlCmd104_3[i].Parameters["@GunC"].Value.ToString();
                }
                else {
                    GunInfo1.Text = "--";
                   imgGunConn1.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电枪/充电连接-未连接.png", UriKind.Relative));
                    VIN1.Text = "--";
                    ChargeU1.Text = "--";
                    ChargeI1.Text =  "--";
                    CurrentSOC1.Text =  "--";
                    RemainChargeTime2.Text = "--";
                    GunC1.Text = "--";
                }

                if (Global.SqlCmd104_4[i] != null && Global.SqlCmd104_4[i].Parameters.Count > 0)
                    {
                        GunChargerID = (Byte)i;
                        string WorkStatus = Global.SqlCmd104_4[i].Parameters["@WorkStatus"].Value.ToString();
                        GunInfo2.Text = Global.transGunWorkStatus(WorkStatus);
                        string VehConnectStatus2 = Global.SqlCmd104_4[i].Parameters["@VehConnectStatus"].Value.ToString(); //0-	断开 1-半连接 2-连接
                        if (VehConnectStatus2 == "2")
                            imgGunConn2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电枪/充电连接-已连接.png", UriKind.Relative));
                        else
                            imgGunConn2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电枪/充电连接-未连接.png", UriKind.Relative));

                        VIN2.Text = Global.SqlCmd104_4[i].Parameters["@VIN"].Value.ToString();
                        ChargeU2.Text = Global.SqlCmd104_4[i].Parameters["@ChargeU"].Value.ToString();
                        ChargeI2.Text = Global.SqlCmd104_4[i].Parameters["@ChargeI"].Value.ToString();
                        CurrentSOC2.Text = Global.SqlCmd104_4[i].Parameters["@CurrentSOC"].Value.ToString();
                        RemainChargeTime2.Text = Global.SqlCmd104_4[i].Parameters["@RemainChargeTime"].Value.ToString();
                        GunC2.Text = Global.SqlCmd104_4[i].Parameters["@GunC"].Value.ToString();
                }
                else
                {
                    GunInfo2.Text = "--";
                    imgGunConn2.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电枪/充电连接-未连接.png", UriKind.Relative));
                    VIN2.Text = "--";
                    ChargeU2.Text = "--";
                    ChargeI2.Text = "--";
                    CurrentSOC2.Text = "--";
                    RemainChargeTime2.Text = "--";
                    GunC2.Text = "--";
                }

            }
        }

        //0：停止状态；
        //1：低速旋转状态；
        //2：中速旋转状态；
        //3：高速旋转状态；
        string getFanStatus(string FanStatus)
        {
            string rtn = "";
            switch (FanStatus)
            {
                case "0":
                    rtn = "停止";
                    break;
                case "1":
                    rtn = "低速";
                    break;
                case "2":
                    rtn = "中速";
                    break;
                case "3":
                    rtn = "高速";
                    break;

            }
            return rtn;

        }

        //0：停止状态；
        //1：旋转状态；
        string getFanRunStatus(string FanStatus)
        {
            string rtn = "";
            switch (FanStatus)
            {
                case "0":
                    rtn = "停止";
                    break;
                case "1":
                    rtn = "旋转";
                    break;            
            }
            return rtn;

        }
        //2-充电进行中u
        private void btnGun1Start_Click(object sender, RoutedEventArgs e)
        {
            if (Global.SqlCmd104_3[GunChargerID] == null || Global.SqlCmd104_3[GunChargerID].Parameters.Count == 0) return;

            string WorkStatus = Global.SqlCmd104_3[GunChargerID].Parameters["@WorkStatus"].Value.ToString();
            if (WorkStatus == "2")
                Global.chargerServer.Stop(GunChargerID, 3);
            else
            {
                //如仓内已在充电则不允许启动插枪充电
                if (Global.chargerServer.IsUsing12(GunChargerID))
                {
                    Global.PromptFail("充电占用");
                    return;
                }
                else { 
                    Global.chargerServer.send7(GunChargerID, 3);
                }
            }
        }

        private void btnGun2Start_Click(object sender, RoutedEventArgs e)
        {
            if (Global.SqlCmd104_4[GunChargerID] == null || Global.SqlCmd104_4[GunChargerID].Parameters.Count == 0) return;

            string WorkStatus = Global.SqlCmd104_4[GunChargerID].Parameters["@WorkStatus"].Value.ToString();
            if (WorkStatus == "2")
                Global.chargerServer.Stop(GunChargerID, 4);
            else
            { 
                //如仓内已在充电则不允许启动插枪充电
                if (Global.chargerServer.IsUsing12(GunChargerID))
                {
                    Global.PromptFail("充电占用");
                    return;
                }
                else
                {
                    Global.chargerServer.send7(GunChargerID, 4);
                }
            }
        }

        private void SetbtnGun()
        {
            if (Global.SqlCmd104_3[GunChargerID] == null || Global.SqlCmd104_3[GunChargerID].Parameters.Count == 0)
            {
                ImageBrush imageCurrentBrush = new ImageBrush();
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/开始充电-禁用.png", UriKind.Relative));
                btnGun1Start.Background = imageCurrentBrush;
            }
            else { 
                string WorkStatus1 = Global.SqlCmd104_3[GunChargerID].Parameters["@WorkStatus"].Value.ToString();
                string VehConnectStatus1 = Global.SqlCmd104_3[GunChargerID].Parameters["@VehConnectStatus"].Value.ToString(); //0-	断开 1-半连接 2-连接

                if (VehConnectStatus1 != "2")
                {
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/开始充电-禁用.png", UriKind.Relative));
                    btnGun1Start.Background = imageCurrentBrush;
                }
                else 
                { 
                    if (WorkStatus1 == "2")
                    {
                        ImageBrush imageCurrentBrush = new ImageBrush();
                        imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/结束充电.png", UriKind.Relative));
                        btnGun1Start.Background = imageCurrentBrush;
                    }
                    else
                    {
                        ImageBrush imageCurrentBrush = new ImageBrush();
                        imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/开始充电-默认.png", UriKind.Relative));
                        btnGun1Start.Background = imageCurrentBrush;
                    }
                }
            }
            if (Global.SqlCmd104_4[GunChargerID] == null || Global.SqlCmd104_4[GunChargerID].Parameters.Count == 0)
            {
                ImageBrush imageCurrentBrush = new ImageBrush();
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/开始充电-禁用.png", UriKind.Relative));
                btnGun2Start.Background = imageCurrentBrush;
            }
            else { 

                string WorkStatus2 = Global.SqlCmd104_4[GunChargerID].Parameters["@WorkStatus"].Value.ToString();
                string VehConnectStatus2 = Global.SqlCmd104_4[GunChargerID].Parameters["@VehConnectStatus"].Value.ToString(); //0-	断开 1-半连接 2-连接

                if (VehConnectStatus2 != "2")
                {
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/开始充电-禁用.png", UriKind.Relative));
                    btnGun2Start.Background = imageCurrentBrush;
                }
                else
                {
                    if (WorkStatus2 == "2")
                    {
                        ImageBrush imageCurrentBrush = new ImageBrush();
                        imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/结束充电.png", UriKind.Relative));
                        btnGun2Start.Background = imageCurrentBrush;
                    }
                    else
                    {
                        ImageBrush imageCurrentBrush = new ImageBrush();
                        imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电枪/开始充电-默认.png", UriKind.Relative));
                        btnGun2Start.Background = imageCurrentBrush;
                    }
                }
            }
        }


        //充电机开始充电按钮启用条件：仓位有电池，且powerOn(CMD104:57=1),且未充电(CMD104:7=0)
        private void SetbtnCharger()
        {

            for (int i = 1; i < 8; i++)  //带两把枪的桩号 默认是1号桩
            {
                System.Windows.Controls.Button btnStart = (System.Windows.Controls.Button)this.FindName("btnStart_" + i);

                TypeInfo type = typeof(PLC.PLCW4).GetTypeInfo();
                //字段用Field ，属性用Poroperty
                FieldInfo stringFieldInfo= type.GetField("locationCheck" + i);
                string locationCheck = stringFieldInfo.GetValue(Global.PLCMsg4).ToString();
                //PropertyInfo propertyInfo = type..GetProperty("locationCheck" + i); 
                //string locationCheck = propertyInfo.GetValue(Global.PLCMsg4).ToString(); //0：无电池； 1：有电池；
                if (Global.SqlCmd104_1[GunChargerID] == null || Global.SqlCmd104_1[GunChargerID].Parameters.Count == 0)
                {
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电仓/按钮-开始充电-禁用.png", UriKind.Relative));
                    btnStart.Background = imageCurrentBrush;
                    continue;
                } 
                string WorkStatus1 = Global.SqlCmd104_1[GunChargerID].Parameters["@WorkStatus"].Value.ToString(); //1-正准备开始充电
                string PowerStatus1 = Global.SqlCmd104_1[GunChargerID].Parameters["@PowerStatus"].Value.ToString();


                //ImageSource image = new BitmapImage(new Uri(@"Resources/charge/充电仓/按钮-开始充电-默认.png", UriKind.Relative));
                //if (btnStart.Background is ImageBrush brush && brush.ImageSource == image)
                //    System.Windows.MessageBox.Show(i+"");

                if (locationCheck == "1" && PowerStatus1=="0" && WorkStatus1=="1")
                {
                    //充电机开始充电按钮启用
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电仓/按钮-开始充电-默认.png", UriKind.Relative));
                    btnStart.Background = imageCurrentBrush;
                }
                else
                {
                    if (WorkStatus1 == "2")
                    {
                        ImageBrush imageCurrentBrush = new ImageBrush();
                        imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电仓/按钮-停止充电-默认.png", UriKind.Relative));
                        btnStart.Background = imageCurrentBrush;
                    }
                    else
                    {
                        ImageBrush imageCurrentBrush = new ImageBrush();
                        imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/charge/充电仓/按钮-开始充电-禁用.png", UriKind.Relative));
                        btnStart.Background = imageCurrentBrush;
                    }
                }
            }
         
        }


        private void ChargerRestart(int chargerID)
        {
            Global.chargerServer.Restart(chargerID, 1);

        }
        private void BatteryRestart(int chargerID)
        {
            Global.chargerServer.Power(chargerID, 1);

        }

        //故障列表显示
        public void Show()
        {
            DBTrans dBTrans = new DBTrans();
            try
            {
                string sql = "select CONCAT_WS(' ', createtime, faultcontent)    as FaultContent1  from alarm  where 1=1 ";
                sql += " LIMIT 0,20";
                AlarmList.ItemsSource = dBTrans.GetDataTable(sql).DefaultView;
                DataTable dt = new DataTable();
            }
            catch (Exception ex)
            {
                Global.PromptFail("[Error]Charge-Show"+ ex.Message);
                Global.WriteLog("[Error]Charge-Show"+ ex.Message + "\r\n" +ex.StackTrace);
            }
            finally
            {
                dBTrans.Close();
            }

        }

        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            int chargerID = getChargerID(sender);

            if (Global.SqlCmd104_1[chargerID] == null || Global.SqlCmd104_1[chargerID].Parameters.Count == 0) return;

            string WorkStatus = Global.SqlCmd104_1[chargerID].Parameters["@WorkStatus"].Value.ToString();
            if (WorkStatus == "2") //充电进行中
                Global.chargerServer.Stop(chargerID, 1);
            else
            {
                //车辆插枪时仓位未充电则不允许仓内自动启动充电；
                if (Global.chargerServer.IsUsing34(GunChargerID))
                {
                    Global.PromptFail("充电占用");
                    return;
                }
                else
                {
                    Global.chargerServer.send7(chargerID, 1);
                }
            }


        }

        private void btnChargerRestart_Click(object sender, RoutedEventArgs e)
        {
            int chargerID = getChargerID(sender);
            ChargerRestart(chargerID);
        }

        private void btnBatteryRestart_Click(object sender, RoutedEventArgs e)
        {
            int chargerID = getChargerID(sender);
            BatteryRestart(chargerID);
        }

        private void btnBatteryCheck_Click(object sender, RoutedEventArgs e)
        {
            int chargerID = getChargerID(sender);
            TextBlock txtBattertySN = (TextBlock)this.FindName("BattertySN" + chargerID);
            string BattertySN = txtBattertySN.Text;
            Global.chargerServer.Publish3002Check(BattertySN, chargerID);
        }

        //停用
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            int chargerID = getChargerID(sender);
            //System.Windows.MessageBox.Show(chargerID.ToString());
            Global.ChargerEnableFlag[chargerID] = 0;
            string sql = "update ChargerStatus set  EnableFlag=0 where ChargerNo='" + chargerID + "' ";
            Global.dbTrans.ExcuteScript(sql);
            Global.chargerServer.Publish3005(chargerID,1);

            ImageBrush imgBackGround = (ImageBrush)this.FindName("imgBackGround" + chargerID);
            imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
            Border ChagerHeadBorder = (Border)this.FindName("ChagerHeadBorder" + chargerID);
            ChagerHeadBorder.Background = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#A9CDEF");


        }

        //启用
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            int chargerID = getChargerID(sender);
            Global.ChargerEnableFlag[chargerID] = 1;
            string sql = "update ChargerStatus set  EnableFlag=1 where ChargerNo='"+ chargerID + "' ";
            Global.dbTrans.ExcuteScript(sql);
            //System.Windows.MessageBox.Show((sender as FrameworkElement).Name);
            Global.chargerServer.Publish3005( chargerID,1);
            SetCharger();
        }

        private int getChargerID(object sender)
        {
            string btnName = (sender as FrameworkElement).Name;
            string[] btnNames = btnName.Split("_");
            
            int chargerID = Int32.Parse(btnNames[1]);

            return chargerID;

        }



        private void txtInqBatterySN_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tmp = txtInqBatterySN.Text;
            if (tmp == "")
            {
                for (int i = 1; i < 8; i++)
                {
                    TextBlock BattertySN = (TextBlock)this.FindName("BattertySN" + i);
                    if (BattertySN == null) return;
                    if (BattertySN.Text.IndexOf(tmp) >= 0)
                    {
                        BattertySN.FontSize = 12;
                        BattertySN.FontWeight = FontWeights.Bold;
                        BattertySN.FontStyle = FontStyles.Normal;
                    }
                }
            }
            if (tmp.Length < 3) return;
            for (int i = 1; i < 8; i++)
            {
                TextBlock BattertySN = (TextBlock)this.FindName("BattertySN" + i);
                if (BattertySN == null) return;
                if (BattertySN.Text.IndexOf(tmp) >= 0)
                {
                    //BattertySN.FontSize = 14;
                    BattertySN.FontWeight = FontWeights.ExtraBold;
                    BattertySN.FontStyle = FontStyles.Oblique;
                }
                else {
                    BattertySN.FontSize = 12;
                    BattertySN.FontWeight = FontWeights.Bold;
                    BattertySN.FontStyle = FontStyles.Normal;
                }
            }
        }

        private void txtInqBatterySN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtInqBatterySN.Text == "请输入电池编码") txtInqBatterySN.Text = "";
        }

        private void txtInqBatterySN_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (txtInqBatterySN.Text == "请输入电池编码") txtInqBatterySN.Text = "";
        }
    }

    public class YourAddressConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values[0].ToString() + "" + values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
