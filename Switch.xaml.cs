using DESCADA.Models;
using DESCADA.Service;
using DESCADA.Test;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.RpcContracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.IO.Pipelines;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static DESCADA.Service.PLC;
using static DESCADA.Switch;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static WPF_DateTimePicker.UserControls.DateTimePicker.View.TMinSexView;

namespace DESCADA
{
    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class Switch : System.Windows.Controls.UserControl
    {
        LED lED;
        public StationConfig myconfig;
        public DispatcherTimer showtimer1, showtimer2;
        public lakiudp udp;
        public Speech speech;
        //public HttpClient httpClient;
        public Local local;
        public int MaxRetryNum=3;
        public int LakiMaxRetryNum = 10;
        //public PLCW5 PLCW5;

        public Switch(StationConfig config)
        {
            InitializeComponent();
            this.txtPlateno.Text = "";//0307拍视频

            myconfig = config;
            //this.Resources["MahApps.Brushes.ToggleSwitch.StrokeOn"] = "Red";

            //DESCADA.KHT.KHT kHT = new KHT.KHT(this);
            // kHT.CallbackFuntion();

            showtimer1 = new DispatcherTimer();
            //showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Interval = new TimeSpan(0, 0, 0, 0,500);
            showtimer1.Start();  //0307拍视频


            showtimer2 = new DispatcherTimer();
            showtimer2.Tick += Showtimer2_Tick;
            showtimer2.Interval = new TimeSpan(0, 0, 0, 0, 3000);
            showtimer2.Start();//0307拍视频
            Showtimer2_Tick(null, null);

            //udp = new lakiudp();


            lED = new LED();
            initChecker();//singlton //0307拍视频

            speech =new Speech();
            //httpClient = new HttpClient(myconfig);
            local = new Local(myconfig);
            //PLCW5 = Global.PLCMsg5;

            frmJianKong frmJK = new frmJianKong();
            frmJK.TopLevel = false;
            wfh.Child = frmJK;

            Notice(12); //欢迎换电，目的是通过发这个，判断显示屏连接状态  0403
        }
        private void Showtimer2_Tick(object sender, EventArgs e)
        {
            SetCharger();
        }
            //checker.checkStatus = 0;//0 未开始校验，1 校验中，2 校验通过，3 校验未通过，4 校验暂停，待人工选择确认,5 取消
        private void Showtimer1_Tick(object sender, EventArgs e)
        {

            showPlateno(Global.plateNO);
            //非自动模式：单动、消防、检修模式下，不让换电，扫牌识别那些都不要走
            //if (Global.PLCMsg5.RunMode!=4) return; 

            //this.txtPlateno.Dispatcher.Invoke(new Action(() => { this.txtPlateno.Text = Plateno; }));
            // if (this.txtPlateno.Text != DESCADA.Global.PlateNO && checker.checkStatus != 1)
            if (Global.PlateNOTimerInNum==0 && checker.checkStatus != 1) //车牌第1次处理
            {
                //非校验中，新扫的车牌
                //this.txtPlateno.Text = DESCADA.Global.PlateNO;
                this.txtPlateno.Dispatcher.Invoke(new Action(() => { this.txtPlateno.Text = DESCADA.Global.PlateNO; }));

                showtimer1.Stop();//校验时暂停执行Timer
                Notice(1);
                initChecker();  //新进换电车辆校验初始化
                if (checker.checkStatus == 0) checker.checkStatus = 1;
                StartSwitchCheck();
                showtimer1.Start();
            }
            else if (checker.checkStatus == 1) //retry
            {
                //校验中，丢弃新扫的车牌
                showtimer1.Stop();
                StartSwitchCheck();
                showtimer1.Start();
            }

            if(Global.PlateNOTimerInNum>=0) Global.PlateNOTimerInNum += 1;

            ShowRobotInfo();

            //响应PLC雷达请求
            if(checker.checkStatus!=1 && Global.PLCMsg5.RadarLocationRequest ==1)
            {
                showtimer1.Stop();
                PLCLakiRequest();
                showtimer1.Start();
            }

            bool Connected = false;
            if (Global.SqlVehCmd3.Parameters.Count > 0)
            {
                string strCreateTime = Global.SqlVehCmd3.Parameters["@CreateTime"].Value.ToString();
                if (DateTime.TryParse(strCreateTime, out DateTime CreateTime))
                {
                    if (Global.DiffSeconds(CreateTime, DateTime.Now) <= 10)
                    {
                        Connected = true;
                    }
                }
            }
            if (Connected)
            {
                Global.VEHConnected = true;
                TogLock.IsEnabled = true;
                if (Global.SqlVehCmd3.Parameters["@LockStatus"].Value.ToString() == "2")
                { 
                    this.txtLockStatus.Text = "上锁";
                    //TogLock.IsOn = true;
                }
                else
                {
                    this.txtLockStatus.Text = "解锁";
                    //TogLock.IsOn = false;
                }
            }
            else {
                Global.VEHConnected = false;
                this.txtLockStatus.Text = "未连";
                TogLock.IsEnabled = false;
               // TogLock.IsOn = false;
            }


        }

        string plcVehLocationChecked = "0";
        int plcLakiFailRetryTimes=0;
        int plcLakicheckStatus = 0;//0 未开始校验，1 校验中，2 校验通过，3 校验未通过
        bool plcFailRetryTimeout=false;
        string plcvehLocationChecked = "0";

        //处理PLC雷达定位请求
        public bool PLCLakiRequest()
        {
            Global.ShowSwitchSteps("收到PLC雷达定位请求" + DateTime.Now.ToString("HH:mm:ss"));
            Global.WriteLog("收到PLC雷达定位请求");

            if (Global.scanStatus == 1) return false;//true 扫描中
            if (plcVehLocationChecked == "0" )
            {
                if (Global.scanStatus == 0)
                {
                    udp = new lakiudp();
                    if (Global.LakiConnStauts == -1)  //&& udp != null
                    {
                        plcLakiFailRetryTimes += 1;
                        //雷达连接失败，10S提示，10分退出
                        if (plcLakiFailRetryTimes >= LakiMaxRetryNum) //5000  100 / checker.RetryInterval
                        {
                            txtSwitchSteps.Text = "激光雷达响应超时（PLC）";
                            if (checker.FailRetryTimes >= LakiMaxRetryNum) //10分钟超时退出 300000 / checker.RetryInterval
                            {
                                plcLakicheckStatus = 3;
                                plcFailRetryTimeout = true;
                                txtSwitchSteps.Text = "激光雷达连接失败(PLC)，超时退出";
                                return false;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        Global.scanStatus = 1;
                        udp.handle();
                        showscanrtn(); //0313 add 待测试
                    }
                    if (Global.scanStatus == 1) return false;//true 扫描中
                }
                if (Global.scanStatus == 2)
                {
                    plcvehLocationChecked = showscanrtn();
                }
                //GC.SuppressFinalize(udp);  //释放对象
                if (plcvehLocationChecked == "-1")//-1 异常 
                {
                    plcLakiFailRetryTimes += 1;
                    if (plcLakiFailRetryTimes >= LakiMaxRetryNum) // MaxRetryNum
                    {
                        txtSwitchSteps.Text = "激光雷达响应异常(PLC)；";
                        plcLakicheckStatus = 3;
                        checker.FailRetryTimeout = true;
                        return false;

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    plcLakiFailRetryTimes = 0;//校验成功清异常
                    if (plcvehLocationChecked != "1")  //校验未通过
                    {
                        Notice(3);
                        txtSwitchSteps.Text = "停车位置超限";
                        RecCehLocation.Stroke = new SolidColorBrush(Colors.Red);
                        plcLakicheckStatus = 3;
                        return false;
                    }
                }
            }
            return true;

        }
        //新车入网，手工录入车牌； 再次触发核验流程 ending

        //当前车辆换电前检查状态 
        public Checker checker;
        public   struct Checker
        {
            public string Plateno; //当前车牌号
            public string rtnVeh; // 1 车辆入网检验 1：已入网
            public string vehBatteryChecked; //2 是否有可换电池
            public string vehConnected; //3 车是否连站成功
            public string vehLocationChecked; //4 车辆位置是否OK
            public string vehConrolerChecked; //5 获取换电控制器 车辆 / 电池状态
            public string vehOldBatteryChecked;//6 车载电池鉴权
            public string vehStatusChecked;  //7 车辆状态是否OK
            public string vehnewBatteryChecked;  //8 借出电池鉴权
            public string vehnewBatteryPWOffChecked;  //8.1 借出电池下电
            public string RobotStatusChecked; //9 机器人状态正常?
            public int checkStatus;  //0 未开始校验，1 校验中，2 校验通过，3 校验未通过，4 校验暂停，待人工选择确认,5 取消
            public string FailSteps ; //发生异常的当前步骤
            public int FailRetryTimes;//发生异常的当前步骤重试次数
            public bool FailRetryTimeout;//发生异常的当前步骤重试超时
            public int RetryInterval;
        }

  


        //0 初始值 未通过  1 通过  -1 异常 或其他返回字符串信息  2:已提交下电申请，等待回复中；
        public void initChecker()
        {
            checker.Plateno = "";
            checker.rtnVeh = "0";  //0 初始值 未通过  1 通过  -1 异常 或其他返回字符串信息
            if (Global.config.NomalConfig.ValidType == 3) checker.rtnVeh = "1"; //不验证，则直接通过
            else if (Global.config.NomalConfig.ValidType == 2)
            {
                checker.FailSteps = "rtnVehB"; //本地验证方式直接跳到本地校验
            }


            checker.vehBatteryChecked = "0"; //是否有可换电池
            checker.vehConnected = "0"; //车是否连站成功
            checker.vehLocationChecked = "0"; //车辆位置是否OK
            checker.vehConrolerChecked = "0"; //获取换电控制器 车辆 / 电池状态
            checker.vehOldBatteryChecked = "1";//车载电池鉴权 20240403 电池信息拿不到，不鉴权了
            //if (Global.config.NomalConfig.ValidType == 3) checker.vehOldBatteryChecked = "1"; //不验证，则直接通过
            checker.vehStatusChecked = "1";  //车辆状态是否OK  20240403 档位刹车拿不到，不鉴权了
            checker.vehnewBatteryChecked = "0";  //借出电池鉴权
            if (Global.config.NomalConfig.ValidType == 3) checker.vehnewBatteryChecked = "1";
            checker.vehnewBatteryPWOffChecked = "0";  //借出电池下电鉴权
            checker.RobotStatusChecked = "0"; //机器人状态正常?
            checker.checkStatus = 0;//0 未开始校验，1 校验中，2 校验通过，3 校验未通过
            checker.FailSteps=""; //发生异常的当前步骤
            checker.FailRetryTimes=0;//发生异常的当前步骤重试次数
            checker.FailRetryTimeout = false;
            checker.RetryInterval = 100;//ms

            lakia.Text = "---";
            lakiX.Text = "---";
            lakiH.Text = "---";
            txtSOC.Text = "---";
            //txtBatteryNo.Text = "---";
            txtCommunicationStatus.Text = "---";
            txtCheckStatus.Text = "---";
            txtCheckStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FDFA"));
            txtSwitchReady.Text = "---";
            txtLockStatus.Text = "---";
            //TogLock.IsOn = false;
            txtPassStatus.Text = "---";
            txtVehNum.Text = "---";

            if(Global.plateNOTimerInNum==0)
                 txtSwitchSteps.Text = "开始验证";
            else
                txtSwitchSteps.Text = "";
            RecCehLocation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A9CDEF"));
            Global.PCPutNum = 9999;//空闲（无动作指令时）所有仓位号都用默认值9999填充
                                   //Global.PlateNO = "";
            Global.PowerOffApply = 0;
        }

        public void ShowRobotInfo()
        {
            txtPositionX.Text = "X轴："+ Global.PLCMsg5.PositionX.ToString("F1") +"mm"; 
            txtPositionY.Text = "Y轴："+ Global.PLCMsg5.PositionY.ToString("F1") + "mm"; 
            txtPositionZ.Text = "Z轴："+ Global.PLCMsg5.PositionZ.ToString("F1") + "mm"; 
            txtPositionR.Text = "R轴："+ Global.PLCMsg5.PositionR.ToString("F1") + "mm"; 

            //机器人换电状态 ExchangeStatus 翻译成目标仓位 ending
            switch (Global.PLCMsg5.ExchangeStatus)
            {
                case 10:
                case 15:
                case 30:
                    txtPCPutNum.Text = "车辆";
                    break;
                case 20:
                case 35:
                    txtPCPutNum.Text = "中转仓";
                    break;
                case 25:
                    txtPCPutNum.Text = "借出电池来源仓位号"+ Global.PCPutNum;
                    break;;
                case 40:
                    txtPCPutNum.Text = "归还电池目标仓位号" + Global.PCPutNum;//取还同仓
                    break;
                case 45:
                    txtPCPutNum.Text = "原点";
                    break;
            }

            //3：单动模式； 4：自动模式； 5：消防模式； 6 检修模式
            int runmode = Global.PLCMsg5.RunMode;
            //runmode =4;

            ImageBrush imageCurrentBrush = new ImageBrush();
            ImageBrush imageBlackBrush = new ImageBrush();
            ImageBrush imageBlackBrush1 = new ImageBrush();
            imageCurrentBrush.Stretch = Stretch.Fill;//设置图像的显示格式
            imageBlackBrush.Stretch = Stretch.Fill;//设置图像的显示格式
            imageBlackBrush1.Stretch = Stretch.Fill;//设置图像的显示格式
            imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/自动充电按钮选中.png", UriKind.Relative));
            imageBlackBrush.ImageSource = new BitmapImage(new Uri(@"Resources/手动充电按钮选中.png", UriKind.Relative));
            imageBlackBrush1.ImageSource = new BitmapImage(new Uri(@"Resources/手动充电按钮选中.png", UriKind.Relative));

            //取消高亮"暂停"
            ImageBrush imageFireBrush = new ImageBrush();
            imageFireBrush.ImageSource = new BitmapImage(new Uri(@"Resources/手动操作按钮小_悬停和点击.png", UriKind.Relative));
            imageFireBrush.Stretch = Stretch.Fill;//设置图像的显示格式
            ImageBrush imageFireBlackBrush = new ImageBrush();
            imageFireBlackBrush.ImageSource = new BitmapImage(new Uri(@"Resources/手动操作按钮小默认.png", UriKind.Relative));
            imageFireBlackBrush.Stretch = Stretch.Fill;//设置图像的显示格式


            switch (runmode)
            {
                case 4:
                    //高亮"自动换电"
                    bntAuto.Background = imageCurrentBrush;
                    btnManual.Background = imageBlackBrush;
                    btnRepair.Background = imageBlackBrush1;
                    btnFire.Background = imageFireBlackBrush;
                    break;
                case 3:
                    //高亮"手动换电"
                    btnManual.Background = imageCurrentBrush;
                    bntAuto.Background = imageBlackBrush;
                    btnRepair.Background = imageBlackBrush1;
                    btnFire.Background = imageFireBlackBrush;
                    break;
                case 6:
                    btnRepair.Background = imageCurrentBrush;
                    bntAuto.Background = imageBlackBrush;
                    btnManual.Background = imageBlackBrush1;
                    btnFire.Background = imageFireBlackBrush;
                    break;
                case 5:
                    btnFire.Background = imageFireBrush;
                    btnRepair.Background = imageBlackBrush;
                    bntAuto.Background = imageBlackBrush;
                    btnManual.Background = imageBlackBrush1;
                    break;

            }


            //机器人状态
            /*
            5：激光雷达测量中
10：机器人车端电池定位；
15：取车端亏电电池；
20：放亏电电池至中转仓；
25：取充电仓满电电池；
30：放满电电池至车端；
35：取中转仓亏电池；
40：放亏电电池至充电仓位；
45：运行至待机位置；*/
            int exchangeStatus = Global.PLCMsg5.ExchangeStatus;
            //txtSwitchSteps
            switch (exchangeStatus)
            {
                case 5:
                    txtExchangeSteps.Text = "激光雷达测量中";
                        break;
                case 10:
                    txtExchangeSteps.Text = "机器人车端电池定位";
                    break;
                case 15:
                    txtExchangeSteps.Text = "取车端亏电电池";
                    break;
                case 20:
                    txtExchangeSteps.Text = "放亏电电池至中转仓";
                    break;
                case 25:
                    txtExchangeSteps.Text = "取充电仓满电电池";
                    break;
                case 30:
                    txtExchangeSteps.Text = "放满电电池至车端";
                    break;
                case 35:
                    txtExchangeSteps.Text = "取中转仓亏电池";
                    break;
                case 40:
                    txtExchangeSteps.Text = "放亏电电池至充电仓位";
                    break;
                case 45:
                    txtExchangeSteps.Text = "运行至待机位置";
                    break;
            }

            //提示换电完成
            if (Global.ExchangeDoneHandled ==false && Global.oldExchangeDone==0 && Global.PLCMsg5.ExchangeDone == 1)
            {
                Global.oldExchangeDone = 1;
                Global.PLCMsg2.PCRecExchangeDone = 1;
                Notice(11);
                Global.ExchangeDoneHandled = true;
                initChecker(); //0314
                Global.httpClient.swapRecord(); //上传换电记录；也可以放在PLCServer.RestorePLCDefault

            }

            switch (Global.PLCMsg5.ControlMode)
            {
                case 0:
                    txtControlMode.Text = "离线模式";
                    break;
                case 1:
                    txtControlMode.Text = "本地模式";
                    break;
                case 2:
                    txtControlMode.Text = "联机模式";
                    break;
            }
              

        }

        //numer:显示屏页码，声音文件名
        public void Notice(int numer)
        {
            try
            {
                lED.ShowLEDPage(numer); //+1
                speech.PlaySound(numer.ToString());
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error]Notice"+ ex.Message + "\r\n" +ex.StackTrace);
            }
        }

        //换电前准备工作检查
        private bool StartSwitchCheck()
        {
            //车辆入网云端检验rtnVehA
            //ValidType 1 远程验证 2 本地 3 不验证
            if ((checker.rtnVeh=="0" || checker.FailSteps == "rtnVehA") && Global.config.NomalConfig.ValidType == 1)
            { 
                checker.rtnVeh = Global.httpClient.vehEnterReq();
                if (checker.rtnVeh == "-1")//异常
                {
                    checker.FailSteps = "rtnVehA"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >=MaxRetryNum)
                    { 
                        checker.FailRetryTimeout = true;//重试超时

                        //20240430Begin 联网模式不再走本地校验了，增加如下
                        txtSwitchSteps.Text = "车辆验证超时";
                        txtCheckStatus.Text = "验证超时";
                        checker.checkStatus = 3;
                        checker.FailRetryTimeout = true;
                        return false;
                        //20240430ENd 联网模式不再走本地校验了
                    }
                    else { 
                       return false;
                    }
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.rtnVeh != "1")  //校验未通过
                    {
                        Notice(2);
                        txtSwitchSteps.Text = "车辆校验未通过:" + checker.rtnVeh; //"车辆未入网" 20240429
                        txtCheckStatus.Text = "未入网";
                        txtCheckStatus.Foreground = new SolidColorBrush(Colors.Red);
                        checker.checkStatus = 3;
                        return false;
                    }
                    else {
                        //校验通过,创建换电记录
                        Global.dbTrans.CreateSwitchRecord();
                    }
                }
            }

            //车辆入网本地检验rtnVehB

            //ValidType 1 远程验证 2 本地 3 不验证
            //if ((checker.FailRetryTimeout == true && checker.FailSteps == "rtnVehA") 
            //|| checker.FailSteps == "rtnVehB")
            if ((checker.rtnVeh == "0" || checker.FailSteps == "rtnVehB") && Global.config.NomalConfig.ValidType == 2)
            {
                if (checker.FailSteps == "rtnVehA") { checker.FailSteps = ""; checker.FailRetryTimes = 0; checker.FailRetryTimeout = false; }
                checker.rtnVeh = local.vehEnterReq();
                if (checker.rtnVeh == "-1")
                {
                    checker.FailSteps = "rtnVehB";checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum)
                    {
                        txtSwitchSteps.Text = "车辆验证超时";
                        txtCheckStatus.Text = "验证超时";
                        checker.checkStatus = 3;
                        checker.FailRetryTimeout = true;
                        return false;
                    }
                    else { 
                      return false;
                    }
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.rtnVeh != "1")  //校验未通过
                    {
                        Notice(2);
                        txtSwitchSteps.Text = "车辆未入网";
                        txtCheckStatus.Text = "未入网";
                        checker.checkStatus = 3;
                        return false;
                    }
                    else
                    {
                        //校验通过,创建换电记录
                        Global.dbTrans.CreateSwitchRecord();
                    }
                }
            }

            //车辆已经入网,判断是否有可换电池
            if (checker.vehBatteryChecked == "0" || checker.FailSteps == "vehBatteryChecked")//经检验允许进一步换电的电池和车
            {
                checker.vehBatteryChecked = Global.batterySOCMatch();
                if (checker.vehBatteryChecked == "-1")//异常
                {
                    checker.FailSteps = "vehBatteryChecked"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum)
                    {
                        txtSwitchSteps.Text = "可换电池验证超时";
                        checker.checkStatus = 3;
                        checker.FailRetryTimeout = true;
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.vehBatteryChecked != "1")   //没有可换电池
                    {
                        if (Global.batteryModelMatch() == "0")//电池型号不匹配
                        {
                            Notice(6);
                            txtSwitchSteps.Text = "无匹配电池";
                            //System.Windows.MessageBox.Show("无匹配电池，请确认");
                            Global.Dialog("请确认", "无匹配电池，请确认", "yes");
                            checker.checkStatus = 3;
                            return false;
                        }
                        else {  //有匹配电池，人工选择SOC电池
                            Notice(7);
                            SelCharger inputDialog = new SelCharger("无满电电池，请指定借出电池"); //忽略则结束
                            if (inputDialog.ShowDialog() == false)
                            {
                                checker.checkStatus = 3;
                                return false;
                            }
                            else
                            {
                                string chargerNo = inputDialog.Answer;
                                Global.PCPutNum=Int32.Parse(chargerNo);
                                checker.vehBatteryChecked = "1";checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                                //write plc
                                //PLC.PLCR2 pLCR2 = new PLC.PLCR2();
                                //pLCR2.PCAutoPickNum = ushort.Parse(chargerNo);

                            }
                        }
                    }
                }
            }

            //判断是否连站成功,可等待下次人工介入重连再校验
            //checker.vehConnected = "1";//temp
            if (checker.vehConnected == "0" || checker.FailSteps == "vehConnected")
            {
                if (checker.FailSteps != "vehConnected") { checker.FailSteps = ""; checker.FailRetryTimes = 0; checker.FailRetryTimeout = false; }
                checker.vehConnected = Global.vehConnected();
                if (checker.vehConnected != "1")//异常 -1 ，没连上 0 连上 1
                {
                    checker.FailSteps = "vehConnected"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum)
                    {
                        txtSwitchSteps.Text = "车站连接中";
                        if (checker.FailRetryTimes >=MaxRetryNum ) //10分钟超时退出  400000 / checker.RetryInterval
                        {
                            checker.checkStatus = 3;
                            checker.FailRetryTimeout = true;
                            txtSwitchSteps.Text = "车站连接失败，超时退出";
                            txtCommunicationStatus.Text = "未连接";
                            //System.Windows.MessageBox.Show("换电前车站连接失败，超时退出");
                        }
                        return false;

                    }
                    else
                    {
                        return false;
                    }
                 }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    txtSwitchSteps.Text = "车站连接成功";
                    txtCommunicationStatus.Text = "正常";
                    
                }
            }

            //启动激光雷达定位
            //车辆位置是否OK，可调整停车位后重试
            //checker.vehLocationChecked = "1";//temp
            if (Global.scanStatus == 1 ) return false;//true 扫描中
            if  (checker.vehLocationChecked == "0" || checker.FailSteps == "vehLocationChecked")  //10 debug jump
            {
                if (checker.FailSteps != "vehLocationChecked")
                { 
                   checker.FailSteps = ""; checker.FailRetryTimes = 0; checker.FailRetryTimeout = false;
                }

                //if (Global.StartScan == true) checker.vehLocationChecked = "-1"; //等执行完扫描
                if (Global.scanStatus == 0)
                {
                    udp = new lakiudp();
                    if (Global.LakiConnStauts == -1 )  //&& udp != null
                    {
                        checker.FailSteps = "vehLocationChecked"; checker.FailRetryTimes += 1;
                        //雷达连接失败，10S提示，10分退出
                        if (checker.FailRetryTimes >= LakiMaxRetryNum) //5000
                        {
                            txtSwitchSteps.Text = "激光雷达响应超时";
                            //if(checker.FailRetryTimes==20) Notice(6); //debug
                            if (checker.FailRetryTimes >=LakiMaxRetryNum ) //10分钟超时退出  300000 / checker.RetryInterval
                            {
                                checker.checkStatus = 3;
                                checker.FailRetryTimeout = true;
                                //System.Windows.MessageBox.Show("换电前检验失败，超时退出");
                                txtSwitchSteps.Text = "激光雷达连接失败，超时退出";
                                return false;
                            }
                        }
                        return  false;
                    }
                    else
                    {
                        Global.scanStatus = 1;
                        udp.handle();
                        showscanrtn();//0319
                    }
                    if (Global.scanStatus == 1) return false;//true 扫描中
                }
                if (Global.scanStatus == 2)
                {
                    checker.vehLocationChecked = showscanrtn();
                }
                //GC.SuppressFinalize(udp);  //释放对象
                if (checker.vehLocationChecked == "-1")//-1 异常 
                {
                    Global.scanStatus = 1;//0319
                    udp.handle(); //0319
                    showscanrtn();//0319
                    checker.FailSteps = "vehLocationChecked"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= LakiMaxRetryNum)
                    {
                        txtSwitchSteps.Text = "激光雷达响应异常；";
                        checker.checkStatus = 3;
                        checker.FailRetryTimeout = true;
                        return false;

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.vehLocationChecked != "1")  //校验未通过
                    {
                        Notice(3);
                        txtSwitchSteps.Text = "停车位置超限";
                        RecCehLocation.Stroke = new SolidColorBrush(Colors.Red);
                        checker.checkStatus = 3;
                        return false;
                    }
                }
            }

            //pending:获取换电控制器  车辆 / 电池状态 转lichang

            if (checker.vehLocationChecked == "0")
            {
                checker.vehConrolerChecked = "1";
            }

            //暂不启用（initchecker中屏蔽）：车载电池鉴权 云端检验vehOldBatteryCheckedA
            if (checker.vehOldBatteryChecked == "0" || checker.FailSteps == "vehOldBatteryCheckedA")
            {
                checker.vehOldBatteryChecked = Global.httpClient.oldbatt_auth_req();
                if (checker.vehOldBatteryChecked == "-1")//异常
                {
                    checker.FailSteps = "vehOldBatteryCheckedA"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum)
                        checker.FailRetryTimeout = true;//重试超时
                    else
                      return false;
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.vehOldBatteryChecked != "1")  //校验未通过
                    {
                        Notice(4);
                        txtSwitchSteps.Text = "车载电池鉴权失败";
                       // DialogResult result = System.Windows.Forms.MessageBox.Show("车载电池鉴权失败，是否继续？", "确认继续", MessageBoxButtons.YesNo);
                        Dialog inputDialog = new Dialog("请确认", "\"车载电池鉴权失败，是否继续？", "yesno");
                        //if (result != DialogResult.Yes)
                        if (inputDialog.ShowDialog() == false)
                        {
                            checker.checkStatus = 3;
                            return false;
                        }
                    }
                }
            }

            //暂不启用（initchecker中屏蔽）：车载电池鉴权 本地检验vehOldBatteryCheckedB
            if ((checker.FailRetryTimeout == true && checker.FailSteps == "vehOldBatteryCheckedA")
                || checker.FailSteps == "vehOldBatteryCheckedB")
            {
                if (checker.FailSteps == "vehOldBatteryCheckedA") checker.FailSteps = ""; checker.FailRetryTimes = 0; checker.FailRetryTimeout = false;
                checker.vehOldBatteryChecked = local.oldbatt_auth_req();
                if (checker.vehOldBatteryChecked == "-1")
                {
                    checker.FailSteps = "vehOldBatteryCheckedB"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum)
                    {
                        txtSwitchSteps.Text = "车载电池鉴权超时";
                        //DialogResult result = System.Windows.Forms.MessageBox.Show("车载电池鉴权超时，是否继续？", "确认继续", MessageBoxButtons.YesNo);
                        Dialog inputDialog = new Dialog("请确认", "车载电池鉴权超时，是否继续？", "yesno");
                        if (inputDialog.ShowDialog() == false)
                        {
                            checker.checkStatus = 3;
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.vehOldBatteryChecked != "1")  //校验未通过
                    {
                        Notice(4);
                        txtSwitchSteps.Text = "车载电池鉴权失败";
                        //DialogResult result = System.Windows.Forms.MessageBox.Show("车载电池鉴权失败，是否继续？", "确认继续", MessageBoxButtons.YesNo);
                        Dialog inputDialog = new Dialog("请确认", "车载电池鉴权失败，是否继续？", "yesno");
                        if (inputDialog.ShowDialog() == false)
                        {
                            checker.checkStatus = 3;
                            return false;
                        }                      
                    }
                }
            }

            #region  暂不启用：车辆状态是否OK
            //车辆状态是否OK  pending; 提示 5 0313 pending test
            if (checker.vehStatusChecked == "0")  //车辆状态是否OK 手刹拿不到
            {
                checker.vehStatusChecked = "1";
                txtSwitchReady.Text = "车辆Ready";
                Thickness margin = new Thickness();
                margin.Left = 790;
                margin.Top = 0;
                margin.Right = 0;
                margin.Bottom = 0;
                panelVehLoc.Margin = margin;
            }
            else {
                txtSwitchReady.Text = "未准备";
            }
            #endregion

            //借出电池鉴权 test pending 手动选择借出电池
            if (checker.vehnewBatteryChecked == "0" || checker.FailSteps == "vehnewBatteryChecked")//经检验允许进一步换电的电池和车
            {
                //选电池 test pending ;下电
                ushort putNum = GetPCPutNum();//自动选取的借出电池的所在仓位；
                if (putNum == 0) { checker.vehnewBatteryChecked = "-1"; } else {
                    checker.vehnewBatteryChecked = Global.httpClient.newbatt_auth_req(); //可能不需要了 20240228，如需要，传入putNum对应的电池参数；
                }
                if (checker.vehnewBatteryChecked == "-1")//异常
                {
                    checker.FailSteps = "vehnewBatteryChecked"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum)
                    {
                        txtSwitchSteps.Text = "借出电池鉴权超时";
                        //DialogResult result = System.Windows.Forms.MessageBox.Show("借出电池鉴权超时，是否继续", "确认继续", MessageBoxButtons.YesNo);//ending;
                        Dialog inputDialog = new Dialog("请确认", "借出电池鉴权超时，是否继续？", "yesno");
                        if (inputDialog.ShowDialog() == false)
                        {
                            checker.checkStatus = 3;
                            checker.FailRetryTimeout = true;
                            return false;
                        }
                        else {
                            checker.vehnewBatteryChecked = "1";
                            checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                        }
                    }
                    else {

                        return false;
                    }
                }
                else
                {
                    checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                    if (checker.vehnewBatteryChecked != "1")   //没有可换电池
                    {
                        checker.FailSteps = ""; checker.FailRetryTimes = 0;//校验成功清异常
                        if (checker.vehnewBatteryChecked != "1")  //校验未通过
                        {
                            //采用自动动选择的电池（鉴权失败的）
                            SelCharger inputDialog = new SelCharger("借出电池鉴权失败，请重新选择电池"); //ending:只能选择不在充电中的，否则先手动去停止；
                            if (inputDialog.ShowDialog() == false) //忽略，仍采用校验失败的电池
                            {
                                checker.vehnewBatteryChecked = "1";
                                Global.PCPutNum = putNum;
                                Global.PLCMsg2.PCAutoPickNum = putNum;
                            }
                            else
                            {
                                //采用手动选择的电池
                                string chargerNo = inputDialog.Answer;
                                Global.PCPutNum = Int32.Parse(chargerNo);
                                //write plc
                                //PLC.PLCR2 pLCR2 = new PLC.PLCR2();
                                //pLCR2.PCAutoPickNum = ushort.Parse(chargerNo
                                Global.PLCMsg2.PCAutoPickNum = ushort.Parse(chargerNo); //ending
                                                                                        //借出电池下电请求 下一段 
                                                                                        // Global.chargerServer.Power(ushort.Parse(chargerNo), 0);  //重试 超时的 单独放节点
                                checker.vehnewBatteryChecked = "1";

                            }
                            inputDialog.Close();

                        }
                    }
                    else
                    {
                        //自动选择的电池校验通过
                        Global.PCPutNum = putNum;
                        Global.PLCMsg2.PCAutoPickNum = putNum; 

                    }
                }
            }


            checker.vehnewBatteryPWOffChecked = "1"; //0313 test pending
            //借出电池下电 以后判断是否取CMD104poweroff状态 vehnewBatteryPWOffChecked  test 20240227
            if (checker.vehnewBatteryPWOffChecked == "0" || checker.vehnewBatteryPWOffChecked == "2" || checker.FailSteps == "vehnewBatteryPWOffChecked")
            {
                int i = Global.PLCMsg2.PCAutoPickNum;
                Global.PowerOffChargerID = i;

                if (checker.vehnewBatteryPWOffChecked == "0" || checker.vehnewBatteryPWOffChecked == "-1")
                {
                    Global.chargerServer.Power(i, 0);
                    checker.vehnewBatteryPWOffChecked = "2";
                    Global.PowerOffApplyTime = DateTime.Now;

                }
                if (checker.vehnewBatteryPWOffChecked == "2")
                {
                    if (Global.SqlCmd2[i] != null || Global.SqlCmd2[i] != null && Global.SqlCmd2[i].Parameters.Count > 0 || Global.SqlCmd2[i].Parameters.Count > 0)
                    { 
                        //报文时间比较  20240226
                        string StartAdd = Global.SqlCmd2[i].Parameters["@StartAdd"].Value.ToString();
                        string RequestResult = Global.SqlCmd2[i].Parameters["@RequestResult"].Value.ToString();
                        string strCreateTime = Global.SqlCmd2[i].Parameters["@CreateTime"].Value.ToString();
                        DateTime CreateTime = DateTime.Parse(strCreateTime);

                        if (StartAdd == "92" && DateTime.Compare(CreateTime, Global.PowerOffApplyTime)>0){ //要处理发出申请后的报文，避免历史
                            if( RequestResult == "0" )
                            { 
                                 checker.vehnewBatteryPWOffChecked = "1";
                                 Global.PowerOffApply = 1;
                                 Global.PowerOffTime=DateTime.Now;
                            }
                            else
                                checker.vehnewBatteryPWOffChecked = "-1";
                        }
                    }
                }
                if (checker.vehnewBatteryPWOffChecked == "-1")//异常
                {
                    checker.FailSteps = "vehnewBatteryPWOffChecked"; checker.FailRetryTimes += 1;
                    if (checker.FailRetryTimes >= MaxRetryNum) //重试3次 3 / checker.RetryInterval
                    {                        
                        checker.checkStatus = 3;
                        checker.FailRetryTimeout = true;
                        return false;
                    }
                }
              
            }


            //机器人状态正常? 持续判断
            if (checker.RobotStatusChecked == "0")
            {
                if (checker.FailRetryTimes >= MaxRetryNum) //10分钟超时退出 防止车走机器未好。 30000 / checker.RetryInterval
                {
                    checker.checkStatus = 3;
                    checker.FailRetryTimeout = true;
                    txtSwitchSteps.Text = "超时退出，机器人未就绪";
                    return false;
                }

                // PLCW5 = Global.pLCServer.s5.RobotStatus;
                //if (PLCW5.RobotStatus == 1)
                if (Global.PLCMsg5.RobotStatus == 1 && Global.PLCMsg5.ControlMode == 2 && Global.AllowSwitch) // && Global.PLCMsg5.RunMode == 4
                {
                    checker.RobotStatusChecked = "1";
                    checker.checkStatus = 2; //校验通过，一切就绪
                    Global.PlateNOTimerInNum = -1;
                    Notice(9);
                    //DialogResult result = System.Windows.Forms.MessageBox.Show("准备就绪，是否换电？", "确认继续", MessageBoxButtons.YesNo);//ending;
                    txtSwitchSteps.Text = "校验通过，一切就绪";
                    Dialog inputDialog = new Dialog("请确认", "准备就绪，是否换电？", "yesno");
                    if (inputDialog.ShowDialog() == true) //"是"启动换电
                    {
                        //ending  启动换电 PCAutoPickNum 需要提前指定好 Global.PLCMsg2.PCToPLCCmd !=6
                        // if (Global.PLCMsg5.RobotStatus == 1 && Global.PLCMsg5.RunMode==4 && Global.PLCMsg5.ControlMode!=2) 
                        if (Global.PLCMsg5.RobotStatus == 1  && Global.PLCMsg5.ControlMode == 2) //&& Global.PLCMsg5.RunMode == 4
                        {
                              Global.PLCMsg2.PCToPLCCmd = 4;
                              Global.PLCMsg2.PCAutoPickNum=ushort.Parse(Global.PCPutNum.ToString());  //test
                              Global.PLCMsg2.PCAutoPutNum = ushort.Parse(Global.PCPutNum.ToString());  //test
                              Global.PLCMsg2.PCAutoExchangeCmd = 1; //PC自动换电指令
                        }
                    }
                }
                else {
                    if(checker.FailRetryTimes==2) Notice(8); //只播一次
                    txtSwitchSteps.Text = "当前机器人状态："+ Global.transRobotStatus(Global.PLCMsg5.RobotStatus);
                    return false;
                }

            }
            return true;

        }

        //获取自动换电取电池的仓位 0 无可以用的电池仓位
        private ushort GetPCPutNum()
        {
            return 1; //0313 test pending 
            ushort ret = 0;
            double PackSoc = 0;
            float SocForEx=Global.config.NomalConfig.ChargeExTactics.SocForEx; // 换电电池SOC筛选阈值

            for (int i = 0; i < 8; i++)
            {
                if (Global.SqlCmd104[i] == null || Global.SqlCmd104[i].Parameters.Count < 1) continue;
                string strCreateTime = Global.SqlCmd104[i].Parameters["@CreateTime"].Value.ToString();
                if (DateTime.TryParse(strCreateTime, out DateTime CreateTime))
                {
                    if (Global.DiffSeconds(CreateTime, DateTime.Now) <= 60)
                    {
                        string strWorkStatus = "";
                        if (Global.SqlCmd104[i] != null && Global.SqlCmd104[i].Parameters.Count > 0)
                            Global.SqlCmd104[i].Parameters["@WorkStatus"].Value.ToString(); //2-充电进行中

                        if (Global.ChargerEnableFlag[i] != 0 && strWorkStatus != "2")  //未封仓
                        {
                            if (Double.TryParse(Global.SqlCmd116[i].Parameters["@PackSoc"].Value.ToString(), out double tmpPackSoc))
                            {
                                if (tmpPackSoc >= SocForEx && tmpPackSoc >= PackSoc)
                                {
                                    PackSoc = tmpPackSoc;
                                    ret = (ushort)i;
                                }
                            }
                        }
                    }
                }       
            }
            return ret;
        }
        //判断车站是否连接成功
        public static string vehLocationChecked()
        {
            return "1";
        }

        /*
         * fixAB: 拟合后AB
         * cutH:截取后H（平均）
         * cutfixa:截取拟合后a
           cutX3:截取后X3-横向偏移

         车辆位置OK：true （待给出判断规则）
         * */
        private string showscanrtn()
        {
            string rtn = "0";
            String filter = " pointnum>100 and AB>800  and AB<900 and  x1<641 and x1>210 and y1>-1050"; //" pointnum>50 and fixAB>400 ";            //filter = " pointnum>50 and ab>400 ";

            //String filter = " pointnum>50 and AB>690 "; //" pointnum>50 and fixAB>400 ";            //filter = " pointnum>50 and ab>400 ";
            if (udp!=null && udp.dtscanlinertn != null) // && dataGridView1.DataSource==null
            {
                udp.dtscanlinertn.DefaultView.RowFilter = filter;
                DataRow[] drs = udp.dtscanlinertn.Select(filter);
                if (drs.Length != 1)
                {
                    rtn = "-1";
                    Global.WriteLog("[Error]激光雷达异常showscanrtn" + "识别出的电池数量不唯一。");
                    //System.Windows.Forms.MessageBox.Show("识别出的电池数量不唯一，请重新手动进行激光雷达扫描。电池数量：" + drs.Length);
                }
                else
                {
                    double lakiX, lakiH,lakia = 0;
                    lakiX = (double)drs[0]["cutX3"];
                    lakiH = (double)drs[0]["cutH"];
                    lakia = (double)drs[0]["cutfixa"];
                    //lakia = Global.Adj_a(lakia);

                    Global.PLCMsg2.RadarLocationAnswer = 0;
                    Global.PLCMsg2.RadarSendSignal = 1;
                    Global.PLCMsg2.RadarX = (float)lakiX;
                    Global.PLCMsg2.RadarY = (float)lakiH;
                    Global.PLCMsg2.RadarR = (float)lakia;

                    this.lakiX.Text = lakiX.ToString();
                    this.lakiH.Text = lakiH.ToString();
                    this.lakia.Text = lakia.ToString();
                    //if (Math.Abs(lakiX) < 300 && Math.Abs(lakiH) < 200 && Math.Abs(lakia) < 4)
                    if (Math.Abs(lakiH) < 760)
                    {
                        rtn = "-2";
                        txtSwitchSteps.Text = "激光雷达异常showscanrtn" + "车站距离" + lakiH + "大于760，请重新停车";
                        Global.WriteLog("激光雷达异常showscanrtn" + "车站距离" + lakiH + "大于760，请重新停车");
                        //System.Windows.MessageBox.Show("车站距离" + lakiH + "大于760，请重新停车");
                    }
                    else
                    {
                        if (Math.Abs(lakiX) <= 300 && Math.Abs(lakia) <= 3 && Math.Abs(lakiH) >= 815 - 200 && Math.Abs(lakiH) <= 815 + 200)  // 距离的基准值先安按照815，在这个基础上偏差+-200

                        {
                            rtn = "1";
                        }
                        else
                        {
                            rtn = "1";// "0"; Debug 1 only for Lab
                        }
                    }
                }
            }
            else {
                rtn = "-1";
                Global.WriteLog("[Error]激光雷达异常showscanrtn"+"未识别出的电池，请重新手动进行激光雷达扫描。");
                //System.Windows.Forms.MessageBox.Show("未识别出的电池，请重新手动进行激光雷达扫描。" );
            }

                return rtn;
        }


        private void bntPlaneno1_Click(object sender, RoutedEventArgs e)
        {
            var ownerWindow = System.Windows.Window.GetWindow(this);
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
            
            popupWindow.Top =13;
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


        public void showPlateno(string Plateno )
        { 
            //this.txtPlateno.Text= Plateno;
            this.txtPlateno.Dispatcher.Invoke(new Action(() => { this.txtPlateno.Text = Plateno; }));
        }

        //1：启动；2：暂停；3：停止；
        //PCToPLCCmd
        // 换电/调仓：单动模式下，点击后弹窗，选择PC手动换仓指令 PENDIGN 
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (Global.PLCMsg5.ControlMode != 2)
            {
                Global.Dialog("请确认", "非联机模式，无法执行该指令", "yes");
               // return;
            }
            if (Global.PLCMsg5.RobotStatus != 1)
            {
                Global.Dialog("请确认", "换电机器人未就绪，无法执行该指令", "yes");
                //return;
            }
            


            //Global.PLCMsg2.PCToPLCCmd = 3; //单动模式固定发送
            ////Global.PLCMsg2.PCAutoExchangeCmd = 1;
            //Global.PLCMsg2.PCManuExchangeStartCmd = 1;

            PutBattery inputDialog = new PutBattery("请指定仓位"); //忽略则结束
            if (inputDialog.ShowDialog() == false)
            {
                //return;
            }
            else
            {
                string chargerNo = inputDialog.Answer;
                if (chargerNo != "") //20240429 归还为空，不需要下电
                {
                    //借出电池下电请求  
                    Global.chargerServer.Power(ushort.Parse(chargerNo), 0);
                }
            }
        }


        bool btnPauseClicked = false;
        //暂停：PC自动换电指令PCAutoExchangeCmd/PC一键消防指令PCOutFireCmd/PC手动换仓启动指令PCManuExchangeStartCmd启动（指令码1）后需要暂停时点击，站控发出对应消息的暂停指令（指令码2），机器人暂停，后续可继续执行待完成动作；暂停状态下，暂停键应高亮提示；
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (Global.PLCMsg5.ControlMode != 2)
            {
                Global.Dialog("请确认", "非联机模式，无法执行该指令", "yes");
                return;
            }
            Global.PLCSendLock = true;
            try
            {
                int runmode= Global.PLCMsg5.RunMode;
                if (runmode == 3 && Global.PLCMsg5.RobotStatus == 2) //单动
                {
                    Global.PLCMsg2.PCManuExchangeStartCmd = 2; // PCAutoExchangeCmd
                }
                if (runmode == 4 && Global.PLCMsg5.RobotStatus==2) //自动 
                {
                    Global.PLCMsg2.PCAutoExchangeCmd = 2; // 
                }
                if (runmode == 5 && Global.PLCMsg5.RobotStatus == 2)//消防
                {
                    Global.PLCMsg2.PCOutFireCmd = 2; // 
                }

                if (runmode == 3 || runmode == 4 || runmode == 5)
                {
                    //高亮"暂停"
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/手动操作按钮小_悬停和点击.png", UriKind.Relative));
                    imageCurrentBrush.Stretch = Stretch.Fill;//设置图像的显示格式
                    btnPause.Background =  imageCurrentBrush;

                    btnPauseClicked = true;
                }

                Global.PLCSendLock = false;
            }
            catch (Exception ex) { Global.WriteLog("[Error]switch btnPause_Click" + ex.Message.ToString()); }
            finally { Global.PLCSendLock = false; }
        }

        //本地用
        //取消换电：取消本次换电流程，车辆图标清除；注：车辆电池处于非解锁状态下方可执行，当电池锁止状态未知时需要弹窗提示人工确认电池已上锁；取消换电在机器人忙（RobotStatus=2：Buzy）时不可用；
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Global.PLCMsg2.PCToPLCCmd = 3; //单动模式固定发送
            // Global.PLCMsg2.PCAutoExchangeCmd = 1;
            if (Global.PLCMsg3.CarLockBattery != 2)
            {
                //提示人工确认电池已上锁
                Global.Dialog("请确认", "请确认电池已上锁", "yes");

            }
            initChecker();

            if (Global.PLCMsg5.RobotStatus == 2)
            {
                return;
            }
        }

        //换电完成：手动换电完成，电池上锁之后执行，触发车辆引导提示信息，提示司机可以离开；
        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            //提示语11
            Notice(11);
            Global.dbTrans.UpdateSwitchRecordeEndTime();
            Global.httpClient.swapRecord();//上传换电记录
        }
        //故障复位：点击后PC发送复位指令PCReset，PLC执行清故障报警。
        private void btnFaultReset_Click(object sender, RoutedEventArgs e)
        {
            if (Global.PLCMsg5.ControlMode != 2)
            {
                Global.Dialog("请确认", "非联机模式，无法执行该指令", "yes");
                return;
            }
            Global.PLCMsg2.PCReset = 1;
        }

        //停止：PC自动换电指令PCAutoExchangeCmd/PC一键消防指令PCOutFireCmd/PC手动换仓启动指令PCManuExchangeStartCmd启动后（指令码1）或暂停后（指令码2），需要终止本次动作时点击停止（指令码3），点击后不可动作不可恢复执行。
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Global.PLCSendLock = true;
            try
            {

                int runmode = Global.PLCMsg5.RunMode;
                if (runmode == 3 && (Global.PLCMsg5.RobotStatus == 2 || Global.PLCMsg5.RobotStatus == 3)) //单动 RobotStatus=2或3(BUSY或Pause)
                { 
                    Global.PLCMsg2.PCManuExchangeStartCmd = 3; // PCAutoExchangeCmd
                }
                if (runmode == 4 && (Global.PLCMsg5.RobotStatus == 2 || Global.PLCMsg5.RobotStatus == 3)) //自动 
                {
                    Global.PLCMsg2.PCAutoExchangeCmd = 3; // 
                }
                if (runmode == 5 && (Global.PLCMsg5.RobotStatus == 2 || Global.PLCMsg5.RobotStatus == 3))//消防
                {
                    Global.PLCMsg2.PCOutFireCmd =3; // 
                }

                if (runmode == 3 || runmode == 4 || runmode == 5)
                {
                    //Global.PLCMsg2.PCToPLCCmd = 3; //单动模式固定发送

                    //取消高亮"暂停"
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/手动操作按钮小默认.png", UriKind.Relative));
                    imageCurrentBrush.Stretch = Stretch.Fill;//设置图像的显示格式
                    btnPause.Background = imageCurrentBrush;

                    btnPauseClicked = false;
                }
                Global.PLCSendLock = false;
            }
            catch (Exception ex) { Global.WriteLog("[Error]switch btnStopClick" + ex.Message.ToString()); }
            finally { Global.PLCSendLock = false; }



        }

        //点了暂停，才能继续
        //继续：PC自动换电指令PCAutoExchangeCmd/PC一键消防指令PCOutFireCmd/PC手动换仓启动指令PCManuExchangeStartCmd暂停后（指令码2）后需要继续执行时点击，站控发出对应消息的启动指令（指令码1），机器人继续执行后续动作；
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            
            if (btnPauseClicked==false)
            {
                return;
            }

            Global.PLCSendLock = true;
            try
            {
                int runmode = Global.PLCMsg5.RunMode;
                if (runmode == 3 && Global.PLCMsg5.RobotStatus == 3) //单动 RobotStatus=3(Pause)
                {
                    Global.PLCMsg2.PCManuExchangeStartCmd = 1; // PCAutoExchangeCmd
                }
                if (runmode == 4 && Global.PLCMsg5.RobotStatus == 3) //自动 
                {
                    Global.PLCMsg2.PCAutoExchangeCmd = 1; // 
                }
                if (runmode == 5 && Global.PLCMsg5.RobotStatus == 3)//消防
                {
                    Global.PLCMsg2.PCOutFireCmd = 1; // 
                }

                if (runmode == 3 || runmode == 4 || runmode == 5)
                {
                    //Global.PLCMsg2.PCToPLCCmd = 3; //单动模式固定发送

                    //取消高亮"暂停"
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/手动操作按钮小默认.png", UriKind.Relative));
                    imageCurrentBrush.Stretch = Stretch.Fill;//设置图像的显示格式
                    btnPause.Background = imageCurrentBrush;

                    btnPauseClicked = false;
                }
                Global.PLCSendLock = false;
            }
            catch (Exception ex) { Global.WriteLog("[Error]switch btnContinue_Click" + ex.Message.ToString()); }
            finally { Global.PLCSendLock = false; }


        }


        //换电复归：点击后PC发送复归指令PCGoHome，机器人回原点。单动模式下均可用；
        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            Global.PLCSendLock = true;
            try
            {
                Global.PLCMsg2.PCToPLCCmd = 3; //单动模式固定发送            
                Global.PLCMsg2.PCGoHome = 1;
                Global.PLCSendLock = false;
            }
            catch (Exception ex) { }
            finally { Global.PLCSendLock = false; }



        }
        //保存记录：换电完成后，当换电记录不完整时高亮提示，可点击该按钮手动补全换电信息；
        private void btnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            //换电完成后，当换电记录不完整时高亮提示，可点击该按钮手动补全换电信息；
        }

        //一键消防
        //一键消防：自动或手动模式下均可触发，点击后弹窗请求录入消防报警仓位，确认后同时发出消防报警仓位号FireNum和PC一键消防指令PCOutFireCmd:1以及PC发送PLC运行指令PCToPLCCmd：5（消防模式）给PLC；
        private void btnFire_Click(object sender, RoutedEventArgs e)
        {
            //Global.PLCMsg2.PCToPLCCmd = 5; //单动模式固定发送
            SelCharger inputDialog = new SelCharger("请确定消防报警仓位"); //忽略则结束
            if (inputDialog.ShowDialog() == false)
            {
                return;
            }
            else
            {
                string chargerNo = inputDialog.Answer;
                Global.PLCSendLock = true;
                try
                {
                    if (Global.PLCMsg5.RobotStatus == 1)
                    { 
                        Global.PLCMsg2.FireNum = ushort.Parse(chargerNo); //消防报警仓位号FireNum
                        Global.PLCMsg2.PCOutFireCmd = 1;
                        Global.PLCMsg2.PCToPLCCmd = 5;//消防模式
                        Global.PLCSendLock = false;
                    }
                }
                catch (Exception ex) { Global.WriteLog("[Error]switch btnFire_Click" + ex.Message.ToString()); }
                finally { Global.PLCSendLock = false; }
            }

        }

        //自动换电: PLC read/PC发送PLC运行指令PCToPLCCmd:4
        private void bntAuto_Click(object sender, RoutedEventArgs e)
        {
            Global.PLCMsg2.PCToPLCCmd = 4;

            btnStart.IsEnabled = true;
            btnPause.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnFinish.IsEnabled = true;
            btnStop.IsEnabled = true;
            btnContinue.IsEnabled = true;
            btnGoHome.IsEnabled = true;
            btnFire.IsEnabled = true;

        }

        //手动换电（单动模式）: PLC read/PC发送PLC运行指令PCToPLCCmd:3
        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            Global.PLCMsg2.PCToPLCCmd = 3;

            btnStart.IsEnabled = true;
            btnPause.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnFinish.IsEnabled = true;
            btnStop.IsEnabled = true;
            btnContinue.IsEnabled = true;
            btnGoHome.IsEnabled = true;
            btnFire.IsEnabled = true;
        }

        //检修模式: PLC read/PC发送PLC运行指令PCToPLCCmd:6(该模式下一切由站控端发起的机器人动作指令禁用)
        private void btnRepair_Click(object sender, RoutedEventArgs e)
        {
            Global.PLCMsg2.PCToPLCCmd = 6;

            btnStart.IsEnabled = false;
            btnPause.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnFinish.IsEnabled = false;
            btnStop.IsEnabled = false;
            btnContinue.IsEnabled = false;
            btnGoHome.IsEnabled = false;
            btnFire.IsEnabled = false;

            Dialog inputDialog = new Dialog("检修模式中", "当前检修中，确认要退出检修吗？", "yesWithoutClose");
            if (inputDialog.ShowDialog() == true)
            {
                //退出检修模式
                btnStart.IsEnabled = true;
                btnPause.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnFinish.IsEnabled = true;
                btnStop.IsEnabled = true;
                btnContinue.IsEnabled = true;
                btnGoHome.IsEnabled = true;
                btnFire.IsEnabled = true;
                //进入自动换电模式

                bntAuto_Click(null, null);
            }




        }


 


        private void txtSwitchStep_MouseDown(object sender, MouseButtonEventArgs e)
        {
            checker.vehStatusChecked = "1";
            txtSwitchReady.Text = "车辆Ready";
            Thickness margin = new Thickness();
            margin.Left = 790;
            margin.Top = 0;
            margin.Right = 0;
            margin.Bottom = 0;
            panelVehLoc.Margin = margin;


            // Dialog inputDialog = new Dialog("请确认", "准备就绪，是否换电？", "yesno");
            Dialog3 inputDialog = new Dialog3("请注意", "用户已确认，即将启动自动换电！", "yesno");
            if (inputDialog.ShowDialog() == true) //"是"启动换电
            {
                txtSwitchSteps.Text = "取车端电池至中转仓";
                txtPCPutNum.Text = "目标仓位：0号仓";
            }
        }


        //原来调LED用的，现在不用了
        /*
        private static Window1 window1 = null; //singlton
        private void imgcar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (window1 == null)
            {
                window1 = new Window1();
            }
            else { 
                if (window1.IsVisible ==false)
                {
                    window1 = null;
                    window1 = new Window1();
                }
            }
            window1.Show();
            window1.Activate();
            //window1.BringIntoView();
            //window1.Topmost = true;
        }
        */

        //TBOX调试
        private void imgcar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //window1.BringIntoView();
            //window1.Topmost = true;
        }


        //上锁，解锁；
        private void TogLock_Click(object sender, RoutedEventArgs e)
        {
            if (TogLock.Tag == "解锁")
            {
                //System.Windows.MessageBox.Show("On");
                Byte[] VIN = Global.Fill0(Encoding.ASCII.GetBytes(Global.VIN), 17);
                Global.VehServer.send7(VIN); //解锁
                Global.VehServer.send7(VIN); //解锁
                Global.VehServer.send7(VIN); //解锁
            }
            else
            {
                //System.Windows.MessageBox.Show("Off");
                Byte[] VIN = Global.Fill0(Encoding.ASCII.GetBytes(Global.VIN), 17);
                Global.VehServer.send5(VIN);                //上锁
                Global.VehServer.send5(VIN);                //上锁
                Global.VehServer.send5(VIN);                //上锁

            }

        }

        //重新扫牌
        private void bntScanPlant_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //"192.168.3.121";
                IntPtr pIP1 = Marshal.StringToHGlobalAnsi("192.168.3.121");
                DESCADA.KHT.NativeMethods.CLIENT_LPRC_SetTrigger(pIP1, 8080);
            }catch(Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.Message);
                Global.PromptFail("重新扫牌失败"+ ex.Message);
                Global.WriteLog("[Error]重新扫牌失败switch-bntScanPlant_Click" + ex.Message+ "\r\n" + ex.StackTrace);
            }
            //if (running1 == true)
            //{
            //if (NativeMethods.CLIENT_LPRC_SetTrigger(pIP1, 8080) == 0)
            //{
            //    listBox1.Items.Insert(0, textBox1.Text.ToString() + "设备1模拟触发命令成功！");
            //    listBox1.SelectedIndex = 0;
            //}
            //else
            //{
            //    listBox1.Items.Insert(0, textBox1.Text.ToString() + "设备1模拟触发命令失败！");
            //    listBox1.SelectedIndex = 0;
            //}
            //}
        }

        private void cmbViewChanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Global.JianKongViewChanel=cmbViewChanel.SelectedIndex+1;
        }

        System.Windows.Media.BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
        void SetCharger()
        {
            if (Global.PlcConnStatus == 0) BattertySN0.Text = "--";
            else if (Global.PLCMsg4.locationCheck0 == 1) BattertySN0.Text = "有电池";
            else BattertySN0.Text = "无电池";

            for (int i = 1; i < 8; i++)
            {
                //不在文字上显示封仓状态，只在背景图上提现，否则会覆盖其它并行的状态
                string tmpchargerStatus = "";
                if (Global.ChargerEnableFlag[i] == 0)
                {
                    tmpchargerStatus = "停用";
                }
             
                ImageBrush imgBackGround = (ImageBrush)this.FindName("imgBackGround" + i);
                if (tmpchargerStatus == "停用")
                {
                    imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
                }

                TextBlock txtBattertySN = (TextBlock)this.FindName("BattertySN" + i);
                System.Windows.Controls.Image imgChargerStatus = (System.Windows.Controls.Image)this.FindName("imgChargerStatus" + i);
                TextBlock txtChargerStatus = (TextBlock)this.FindName("txtChargerStatus" + i);
                System.Windows.Controls.Image imgSmoke = (System.Windows.Controls.Image)this.FindName("imgSmoke" + i);
                TextBlock txSmoke = (TextBlock)this.FindName("txtSmoke" + i);
                System.Windows.Controls.Image imgGas = (System.Windows.Controls.Image)this.FindName("imgGas" + i);
                //TextBlock txtGas = (TextBlock)this.FindName("txtGas" + i);
                System.Windows.Controls.Image imgFire = (System.Windows.Controls.Image)this.FindName("imgFire" + i);
                TextBlock txtFire = (TextBlock)this.FindName("txtFire" + i);
                //TextBlock txtFault = (TextBlock)this.FindName("txtFault" + i);
                //System.Windows.Controls.Image imgFault = (System.Windows.Controls.Image)this.FindName("imgFault" + i);

                if (Global.SqlCmd104_1[i] == null || Global.SqlCmd116[i] == null
                          || Global.SqlCmd104_1[i].Parameters.Count == 0 || Global.SqlCmd116[i].Parameters.Count == 0)
                {
                    txtBattertySN.Text = "--"; txtBattertySN.Foreground = new SolidColorBrush(Colors.White);
                    txtChargerStatus.Text = "--"; txtChargerStatus.Foreground = new SolidColorBrush(Colors.White);
                    txSmoke.Text = "--"; txSmoke.Foreground = new SolidColorBrush(Colors.White);
                    txtFire.Text = "--"; txtFire.Foreground = new SolidColorBrush(Colors.White);
                    imgChargerStatus.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgSmoke.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    //imgGas.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgFire.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/置灰.png", UriKind.Relative));
                    imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/组1585(2).png", UriKind.Relative));
                    continue;
                }

                string BattertySN = Global.SqlCmd116[i].Parameters["@BattertySN"].Value.ToString();
                txtBattertySN.Text = BattertySN;

                //充电仓状态 ending
                string VehConnectStatus = Global.SqlCmd104_1[i].Parameters["@VehConnectStatus"].Value.ToString();

                string WorkStatus1 = "", WorkStatus2 = "", WorkStatus3 = "", WorkStatus4 = "";
                if (Global.SqlCmd104_1[i] != null && Global.SqlCmd104_1[i].Parameters["@WorkStatus"] != null)
                    WorkStatus1 = Global.SqlCmd104_1[i].Parameters["@WorkStatus"].Value.ToString();
                if (Global.SqlCmd104_2[i] != null && Global.SqlCmd104_2[i].Parameters["@WorkStatus"] != null)
                    WorkStatus2 = Global.SqlCmd104_2[i].Parameters["@WorkStatus"].Value.ToString();
                if (Global.SqlCmd104_3[i] != null && Global.SqlCmd104_3[i].Parameters["@WorkStatus"] != null)
                    WorkStatus3 = Global.SqlCmd104_3[i].Parameters["@WorkStatus"].Value.ToString();
                if (Global.SqlCmd104_4[i] != null && Global.SqlCmd104_4[i].Parameters["@WorkStatus"] != null)
                    WorkStatus4 = Global.SqlCmd104_4[i].Parameters["@WorkStatus"].Value.ToString();

                bool chargerIsConnected = Global.chargerServer.ChargerIsConnected(i);
                TypeInfo type = typeof(PLC.PLCW4).GetTypeInfo();
                FieldInfo stringFieldInfo = type.GetField("locationCheck" + i);
                string locationCheck = stringFieldInfo.GetValue(Global.PLCMsg4).ToString();


                string chargerStatus = "";
                string faultStatus = "正常";

                if (chargerIsConnected )
                {
                    Global.chargerConnStatus[i] = 1;
                }

                if (chargerIsConnected == false)
                {
                    Global.chargerConnStatus[i] = -1;
                    chargerStatus = "离线"; Global.ChargerStatus[i] = 0;
                }
                //else if (WorkStatus3 == "2" || WorkStatus4 == "2")
                //{
                //    chargerStatus = "占用"; Global.ChargerStatus[i] = 1;
                //}
                else if (WorkStatus1 == "2" || WorkStatus2 == "2")
                {
                    chargerStatus = "充电"; Global.ChargerStatus[i] = 4;
                }
                else if (locationCheck == "0")
                {
                    chargerStatus = "空闲"; Global.ChargerStatus[i] = 2;
                }
                else if (locationCheck == "1")
                {
                    chargerStatus = "监控"; Global.ChargerStatus[i] = 3;
                }

                if (Global.ChargerEnableFlag[i] == 0)
                {
                    chargerStatus = "停用";
                }

                //byte warnbytes = Global.cmd116.BMSWarn2_2[2];
                //string strWarn = Convert.ToString(warnbytes, 2);
                //byte[] byteArray = new byte[] { warnbytes };
                //string strWarn = string.Join("", Array.ConvertAll(byteArray, b => Convert.ToString(b, 2).PadLeft(8, '0')));
                int BMSWarnLevel = Global.chargerServer.getfltRnk(Global.cmd116.BMSWarn2_2); //strWarn[1]; //最高报警等级
                int chargestatus = Global.chargerServer.getChargeStatus(Global.cmd116.BMSWarn2_2); //strWarn[3];充电状态
                //TextBlock txFault = (TextBlock)this.FindName("txFault" + i);
                //Image imgFault = (Image)this.FindName("imgFault" + i);

                if (WorkStatus1 == "6" || WorkStatus2 == "6" || WorkStatus3 == "6" || WorkStatus4 == "6" || chargestatus == 3 || BMSWarnLevel != 0)
                {
                    //CMD104:7=6或CMD116:16:Byte3:bit4~3=3或CMD116:16:Byte3:bit6~5≠0
                    faultStatus = "故障";
                    //imgFault.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                    Global.ChargerFaultStatus[i] = 1;
                }
                else
                {
                    faultStatus = "正常";
                    //imgFault.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    Global.ChargerFaultStatus[i] = 0;
                }
                //txFault.Text = faultStatus;

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


                if (tmpchargerStatus == "停用")
                {
                    //imgBackGround.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/charge/充电仓/电池仓_无电池_默认.png", UriKind.Relative));
                }
                else if (faultStatus == "故障")
                {
                    imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/充电仓故障.png", UriKind.Relative)); //待换图标
                }
                else
                {
                    if (chargerStatus == "空闲" || chargerStatus == "监控")
                    {
                        imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/充电仓电量足.png", UriKind.Relative));
                        txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#00FDFA");

                    }
                    else if (chargerStatus == "占用" || chargerStatus == "充电")
                    {
                        imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/组1585.png", UriKind.Relative));
                        txtChargerStatus.Foreground = (System.Windows.Media.Brush)brushConverter.ConvertFromString("#FFBE2E");
                    }
                    else if (chargerStatus == "离线" || chargerStatus == "停用")
                    {
                        imgBackGround.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/组1585(2).png", UriKind.Relative));
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
                //int gasWarn = 0;
                //string GunGas1 = Global.SqlCmd104_1[i].Parameters["@GunGas1"].Value.ToString();
                //string GunGas2 = Global.SqlCmd104_1[i].Parameters["@GunGas2"].Value.ToString();
                //if (GunGas1 == "1" || GunGas1 == "1") gasWarn = 1;
                //Image imgGas = (Image)this.FindName("imgGas" + i);
                //TextBlock txtGas = (TextBlock)this.FindName("txtGas" + i);
                //if (gasWarn == 1)
                //{
                //    imgGas.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                //    txtGas.Text = "异常";
                //}
                //else
                //{
                //    imgGas.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                //    txtGas.Text = "正常";
                //}

                //火警 充电仓总览区：火警（告警）+充电仓详情区：消防预警级别	
                //CMD104:62:消防预警级别|CMD116:16:Byte8:bit2火灾报警:0为正常，非0为告警
                int fireWarn = str[5];//0：正常 1：故障
                if (fireWarn == 1)
                {
                    imgFire.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                    txtFire.Text = "异常";
                }
                else
                {
                    imgFire.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                    txtFire.Text = "正常";
                }

                //overview
         

            }
        }
    }
}
