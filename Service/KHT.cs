
using DESCADA.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DESCADA.KHT
{
    public class KHT
    {
        public string IP1 = "192.168.3.121"; //"192.168.8.98"; 有配置取配置通道1
        public string IP2 = "192.168.3.121";
        bool IP2EnableFlag= false; //是否启用第2个相机；
        public  DESCADA.KHT.CLIENT_LPRC_ConnectCallback ConnectCallback = null;  // 连接回调
        public DESCADA.KHT.CLIENT_LPRC_DataEx2Callback DataEx2Callback = null;   // 识别回调
        bool running1 = false;
        bool running2 = false;
        static DESCADA.KHT.CLIENT_LPRC_PLATE_RESULTEX recRes1;   // 识别结果
        static DESCADA.KHT.CLIENT_LPRC_PLATE_RESULTEX recRes2;   // 识别结果
        IntPtr pIP1 = IntPtr.Zero;   // 相机1 ip
        IntPtr pIP2 = IntPtr.Zero;   // 相机2 ip


        //public Switch myswithch;
        public  string msg = "";


        public KHT()
        {
            if (Global.config.PasssGateWayConfigs.Count > 0)
            {
                PassGateWayConfig Temp1 = Global.config.PasssGateWayConfigs[0];
                IP1 = Temp1.busIdentConfig.IP;
            }

            //线程可以控制控件
            //CheckForIllegalCrossThreadCalls = false;
            //InitializeComponent();
            GC.KeepAlive(ConnectCallback);
            GC.KeepAlive(DataEx2Callback);
        }
        //public KHT(Switch MYswitch)
        //{
        //    myswithch = MYswitch;
        //    //线程可以控制控件
        //    //CheckForIllegalCrossThreadCalls = false;
        //    //InitializeComponent();
        //}

        //H264获取视频流
        public void CallbackFuntion()
        {
            this.ConnectCallback = new CLIENT_LPRC_ConnectCallback(this.OnConnectCallback);
            this.DataEx2Callback = new CLIENT_LPRC_DataEx2Callback(OnDataEx2Callback);
            // 注释掉jpeg回调获取视频流的方式显示视频。（可以使用同时也存在使用动态库的方式播放jpeg视频流的方式）
            //this.JpegCallback = new CLIENT_LPRC_JpegCallback(OnJpegCallback); 
            pIP1 = Marshal.StringToHGlobalAnsi(IP1); //Marshal.StringToHGlobalAnsi(textBox1.Text.Trim());
            pIP2 = Marshal.StringToHGlobalAnsi(IP2); //Marshal.StringToHGlobalAnsi(textBox3.Text.Trim());

            //注册回调函数
            NativeMethods.CLIENT_LPRC_RegCLIENTConnEvent(this.ConnectCallback);
            NativeMethods.CLIENT_LPRC_RegDataEx2Event(this.DataEx2Callback);
            // 注释掉jpeg回调获取视频流的方式显示视频。（可以使用同时也存在使用动态库的方式播放jpeg视频流的方式）
            //NativeMethods.CLIENT_LPRC_RegJpegEvent(this.JpegCallback); 
            //NativeMethods.CLIENT_LPRC_RegSerialDataEvent(this.SerialDataCallback);

            IntPtr init = Marshal.StringToHGlobalAnsi("C://Image");
            NativeMethods.CLIENT_LPRC_SetSavePath(init);

            //设备连接
            if (NativeMethods.CLIENT_LPRC_InitSDK(8080, IntPtr.Zero, 0, pIP1, 1) != 0)  // 用最后一个参数简单来区别相机(一般传入用户数据)
            {
                msg+= "设备1初始化失败！";
                Global.Kht1ConnStatus = -1;
            }
            else
            {
                msg += "设备1初始化成功！";
                Global.Kht1ConnStatus = 1;
                running1 = true;
            }
            if(IP2EnableFlag==true)
            { 
                if (NativeMethods.CLIENT_LPRC_InitSDK(8080, IntPtr.Zero, 0, pIP2, 2) != 0)
                {
                    msg += "设备2初始化失败！";
                }
                else
                {
                    msg += "设备2初始化成功！";
                    running2 = true;
                }
            }
           // NativeMethods.WT_H264Init();

        }

        //连接状态回调函数
        public void OnConnectCallback(System.IntPtr chCLIENTIP, uint nStatus, uint dwUser)
        {
            if (nStatus == 0)
            {
                if (dwUser == 1)
                {
                    msg +=  "设备1连接异常";
                }
                if (dwUser == 2)
                {
                    msg += "设备2连接异常";
                }
            }
        }

        //识别结果回调函数
        private void OnDataEx2Callback(ref CLIENT_LPRC_PLATE_RESULTEX recResultEx, uint dwUser)
        {
            try
            {




                if (dwUser == 1)
                {
                    recRes1 = recResultEx;
                    //MessageBox.Show( recRes1.chLicense);
                    //this.data(recRes1, 1); //显示识别图片
                }
                if (dwUser == 2)
                {
                    recRes2 = recResultEx;
                    // MessageBox.Show(recRes2.chLicense);
                    //this.data(recRes2, 2); //显示识别图片
                }

                //this.myswithch.showPlateno(recRes1.chLicense);
                if (recRes1.chLicense != "")
                {  
                    Global.PlateNO = recRes1.chLicense;
                    Global.PlateNOTimerInNum = 0;
                }



            }
            catch (Exception e)
            {
                Console.WriteLine("Catch clause caught : {0} \n", e.Message);
            }
        }

    }
}
