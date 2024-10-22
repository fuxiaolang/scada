using DESCADA.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace DESCADA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex = new Mutex(true, "{DESCADA}");
        static async Task Main1()
        {
            DESCADA.KHT.KHT kHT = new KHT.KHT();
            kHT.CallbackFuntion();

            while (true)  //!Console.KeyAvailable  ??? 抓回调?  test 0318
            {
                GC.Collect();
                await Task.Delay(1);
            }
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        protected override void OnStartup(StartupEventArgs e)
        {

            if (!mutex.WaitOne(0, false))
            {
                //MessageBox.Show("应用程序已运行。");
                // 应用程序已运行，找到现有的应用程序并激活它
                IntPtr windowHandle = FindWindow(null, "MainWindow");
                if (windowHandle != IntPtr.Zero)
                {
                    // 显示并激活窗口
                    ShowWindowAsync(windowHandle, 4); // 4代表 SW_SHOWNORMAL
                    SetForegroundWindow(windowHandle);
                }


                Application.Current.Shutdown();
                Environment.Exit(0);
                return;
            }

            //System.Threading.Thread thread = new System.Threading.Thread(() => {

            //    DESCADA.KHT.KHT kHT = new KHT.KHT();
            //    kHT.CallbackFuntion();



            //});

            //thread.Start();

           

            Global.config = StationConfig.FromFile();
            //Global.initPLCWarn(); //            plcWarn[2]
            Global.httpClient = new DESCADA.HttpClient(Global.config);

            Main1();

            Global.initCharger();
            Global.chargerServer = new ChargerServer();
            Global.chargerServer.Start();


            //Global.pLCServer = new PLCServer();
            initPLCMsg();
            //Global.pLCServer.Start();

            if (Global.VehServer == null)
            {
                Global.VehServer = new VehServer();
                Global.VehServer.Start();
            }

            Global.FireServer=new FireServer();
            Global.FireServer.Start();

            // public Byte[][] CmdA =new byte[33][];  //一台消防主机16个探测头 //14
            //33 取配置
            for (int i = 0; i < 33; i++)
            {
                Global.FireServer.CmdA[i] = new Byte[14];
            }
        }

        //private static StationConfig config;
        //public static StationConfig Config
        //{
        //    get
        //    {
        //        if (config == null)
        //        {
        //            config = new StationConfig();
        //        }
        //        return config;
        //    }
        //}


        void initPLCMsg()
        {
            //bool IsDefalut = false;
            bool IsDefalut = true;

            if (IsDefalut)
            { 
                Global.PLCMsg1.pkghead = 65518;
                Global.PLCMsg1.head = 1;
                Global.PLCMsg1.BatteryChargerStatus1 = 9999;    //	充电仓位1充电机状态
                Global.PLCMsg1.BatteryChargerStatus2 = 9999;    //	充电仓位2充电机状态
                Global.PLCMsg1.BatteryChargerStatus3 = 9999;    //	充电仓位3充电机状态
                Global.PLCMsg1.BatteryChargerStatus4 = 9999;    //	充电仓位4充电机状态
                Global.PLCMsg1.BatteryChargerStatus5 = 9999;    //	充电仓位5充电机状态
                Global.PLCMsg1.BatteryChargerStatus6 = 9999;    //	充电仓位6充电机状态
                Global.PLCMsg1.BatteryChargerStatus7 = 9999;    //	充电仓位7充电机状态
                Global.PLCMsg1.tempCharger1 = 9999;  //	充电机1排风口温度
                Global.PLCMsg1.tempCharger2 = 9999;  //	充电机2排风口温度
                Global.PLCMsg1.tempCharger3 = 9999;  //	充电机3排风口温度
                Global.PLCMsg1.tempCharger4 = 9999;  //	充电机4排风口温度
                Global.PLCMsg1.tempCharger5 = 9999;  //	充电机5排风口温度
                Global.PLCMsg1.tempCharger6 = 9999;  //	充电机6排风口温度
                Global.PLCMsg1.tempCharger7 = 9999;  //	充电机7排风口温度
                Global.PLCMsg1.CarWaitInfo = 0;  //	车辆排队信息

                Global.PLCMsg2.pkghead = 65518;
                Global.PLCMsg2.head = 2;
                Global.PLCMsg2.heart = 0;
                Global.PLCMsg2.PCToPLCCmd = 4;   //	PC发送PLC运行指令 cada启动默认值4（自动）
                Global.PLCMsg2.PCAutoPickNum = 9999;    //	自动取电池仓位
                Global.PLCMsg2.PCAutoPutNum = 9999; //	自动放电池仓位
                Global.PLCMsg2.PCAutoExchangeCmd = 9999;    //	PC自动换电指令
                Global.PLCMsg2.FireNum = 9999;  //	消防报警仓位
                Global.PLCMsg2.PCOutFireCmd = 9999; //	PC一键消防指令
                Global.PLCMsg2.PCRecExchangeDone = 0;  //	换电完成接受信号
                Global.PLCMsg2.PCManuExchangeCmd = 9999;    //	PC手动换仓指令
                Global.PLCMsg2.PCManuPickNum = 9999;   //	PC手动换电仓位信息
                Global.PLCMsg2.PCManuExchangeStartCmd = 9999;   //	PC手动换仓启动指令
                Global.PLCMsg2.PCGoHome = 0;   //	PC发送复归指令
                Global.PLCMsg2.PCReset = 0;    //	PC发送复位指令
                Global.PLCMsg2.PumpOpen = 0;   //	PC发送注油指令
                Global.PLCMsg2.RadarLocationAnswer = 0;    //	激光雷达定位响应
                Global.PLCMsg2.RadarX = 9999;// 1.25F;    //	激光雷达X偏移值
                Global.PLCMsg2.RadarY = 9999;// 2.04F;    //	激光雷达Y偏移值
                Global.PLCMsg2.RadarR = 9999;// 3.10F;    //	激光雷达R偏移值
                Global.PLCMsg2.RadarSendSignal = 0;    //	激光雷达值发送信号

                Global.PLCMsg3.pkghead = 65518;
                Global.PLCMsg3.head = 3;
                Global.PLCMsg3.CarExConnector = 0; //	换电控制器连接状态
                Global.PLCMsg3.CarHandBrake = 9999; //	手刹状态
                Global.PLCMsg3.CarKey = 9999;   //	钥匙状态
                Global.PLCMsg3.CarGears = 9999; //	档位状态
                Global.PLCMsg3.CarLockBattery = 9999;   //	车端底座锁止状态
                Global.PLCMsg3.CarChargeConnector = 9999;   //	充电连接器连接状态
                Global.PLCMsg3.CarDischargeConnector = 9999;    //	放电连接器连接状态
                Global.PLCMsg3.BatteryModel = 9999; //	电池类型
            }
            else { 
            //test data
            
                  Global.PLCMsg1.pkghead = 65518;
                  Global.PLCMsg1.head = 1;
                  Global.PLCMsg1.BatteryChargerStatus1 = 1;    //	充电仓位1充电机状态
                  Global.PLCMsg1.BatteryChargerStatus2 = 2;    //	充电仓位2充电机状态
                  Global.PLCMsg1.BatteryChargerStatus3 = 3;    //	充电仓位3充电机状态
                  Global.PLCMsg1.BatteryChargerStatus4 = 4;    //	充电仓位4充电机状态
                  Global.PLCMsg1.BatteryChargerStatus5 = 5;    //	充电仓位5充电机状态
                  Global.PLCMsg1.BatteryChargerStatus6 = 6;    //	充电仓位6充电机状态
                  Global.PLCMsg1.BatteryChargerStatus7 = 7;    //	充电仓位7充电机状态
                  Global.PLCMsg1.tempCharger1 = 8;  //	充电机1排风口温度
                  Global.PLCMsg1.tempCharger2 = 9;  //	充电机2排风口温度
                  Global.PLCMsg1.tempCharger3 = 10;  //	充电机3排风口温度
                  Global.PLCMsg1.tempCharger4 = 11;  //	充电机4排风口温度
                  Global.PLCMsg1.tempCharger5 = 12;  //	充电机5排风口温度
                  Global.PLCMsg1.tempCharger6 = 13;  //	充电机6排风口温度
                  Global.PLCMsg1.tempCharger7 = 14;  //	充电机7排风口温度
                  Global.PLCMsg1.CarWaitInfo = 28;  //	车辆排队信息

                  Global.PLCMsg2.pkghead = 65518;
                  Global.PLCMsg2.head = 2;
                  Global.PLCMsg2.heart = 0;
                  Global.PLCMsg2.PCToPLCCmd = 15;   //	PC发送PLC运行指令
                  Global.PLCMsg2.PCAutoPickNum = 16;    //	自动取电池仓位
                  Global.PLCMsg2.PCAutoPutNum = 17; //	自动放电池仓位
                  Global.PLCMsg2.PCAutoExchangeCmd = 18;    //	PC自动换电指令
                  Global.PLCMsg2.FireNum = 19;  //	消防报警仓位
                  Global.PLCMsg2.PCOutFireCmd = 20; //	PC一键消防指令
                  Global.PLCMsg2.PCRecExchangeDone = 1;  //	换电完成接受信号
                  Global.PLCMsg2.PCManuExchangeCmd = 22;    //	PC手动换仓指令
                  Global.PLCMsg2.PCManuPickNum = 23;   //	PC手动换电仓位信息
                  Global.PLCMsg2.PCManuExchangeStartCmd = 24;   //	PC手动换仓启动指令
                  Global.PLCMsg2.PCGoHome = 1;   //	PC发送复归指令
                  Global.PLCMsg2.PCReset = 1;    //	PC发送复位指令
                  Global.PLCMsg2.PumpOpen = 1;   //	PC发送注油指令
                  Global.PLCMsg2.RadarLocationAnswer = 0;    //	激光雷达定位响应
                  Global.PLCMsg2.RadarX = 9999;// 1.25F;    //	激光雷达X偏移值
                  Global.PLCMsg2.RadarY = 9999;// 2.04F;    //	激光雷达Y偏移值
                  Global.PLCMsg2.RadarR = 9999;// 3.10F;    //	激光雷达R偏移值
                  Global.PLCMsg2.RadarSendSignal = 0;    //	激光雷达值发送信号

                  Global.PLCMsg3.pkghead = 65518;
                  Global.PLCMsg3.head = 3;
                  Global.PLCMsg3.CarExConnector = 0; //	换电控制器连接状态
                  Global.PLCMsg3.CarHandBrake = 1; //	手刹状态
                  Global.PLCMsg3.CarKey = 1;   //	钥匙状态
                  Global.PLCMsg3.CarGears = 1; //	档位状态
                  Global.PLCMsg3.CarLockBattery = 1;   //	车端底座锁止状态
                  Global.PLCMsg3.CarChargeConnector = 1;   //	充电连接器连接状态
                  Global.PLCMsg3.CarDischargeConnector = 1;    //	放电连接器连接状态
                  Global.PLCMsg3.BatteryModel = 1; //	电池类型
            }

        }

    }
}
