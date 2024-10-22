using ControlzEx.Standard;
using DESCADA.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using static DESCADA.Global;

namespace DESCADA
{
    /// <summary>
    /// ChargerDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ChargerDetail : Window
    {
        public DispatcherTimer showtimer1;

        public ChargerDetail()
        {
            InitializeComponent();
            Global.SetShowScreen(this);

            // Set the view model as the data context
            DataContext =Global.SelChargerViewModel;

            showtimer1 = new DispatcherTimer();
            //showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Interval = new TimeSpan(0, 0, 0, 0, 3000);
            showtimer1.Start();

        }
        private void Showtimer1_Tick(object sender, EventArgs e)
        {
            SetDetail();
        }
        
        public static void SetShowScreen(Window window)
        {
            //主窗体在扩展屏?
            bool isExtending = false;
            double windowLeft = 0;
            if (Screen.AllScreens.Length > 1)
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                windowLeft = mainWindow.Left;
                if (mainWindow != null)  //-7 扩展1400
                {
                    if (windowLeft > 200 || windowLeft < -1000)   //-2100
                        isExtending = true;
                }
            }

            if (isExtending)
            {
                Screen s2 = Screen.AllScreens[1];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                //this.Left = -1920;  //r2.Left; //-2880   -1920; 
                if (r2.Left < 0) //往左扩
                {
                    window.Left = -1920;
                }
                else
                {                     //往右扩
                    window.Left = windowLeft;// 1494;
                }
                window.Top = r2.Top-130;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }
            else
            {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                window.Top = r1.Top;
                window.Left = r1.Left;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;

            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // this.WindowState = WindowState.Maximized;
            SetDetail();
        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //do something here
            //this.Close();            
            this.Hide();
            Global.ChargerViewISShow = false;

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();            
            this.Hide();
            Global.ChargerViewISShow = false;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();            
            this.Hide();
            Global.ChargerViewISShow = false;

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
           // System.Windows.MessageBox.Show("Window_Deactivated");
           //this.Hide();
           
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            // this.Hide();
           // double x = this.Top;

        }

        void SetDetail()
        {
            int i = Global.SelChargerViewModel.ChargerID;
            if (Global.SqlCmd104_1[i] == null || Global.SqlCmd106[i] == null|| Global.SqlCmd116[i] == null) return;
            if (Global.SqlCmd104_1[i].Parameters.Count == 0 || Global.SqlCmd106[i].Parameters.Count == 0 || Global.SqlCmd116[i].Parameters.Count == 0) return;

            ChargerSoftVer.Text= Global.SqlCmd106[i].Parameters["@SoftVer"].Value.ToString();// SoftVer_030 CMD106: 5:充电桩软件版本
            CommSoftVer.Text = Global.SqlCmd106[i].Parameters["@CommSoftVer"].Value.ToString();// CMD106: 20:桩后台通信协议版本
            GunNum.Text = Global.SqlCmd106[i].Parameters["@GunNum"].Value.ToString();//  CMD106: 11:充电枪个数
            string strBmsChargeMode= Global.SqlCmd104_1[i].Parameters["@BmsChargeMode"].Value.ToString();//   CMD104: 18:BMS充电模式 1-	恒压2-	恒流
            if (strBmsChargeMode == "1") BmsChargeMode.Text = "恒压";
            if (strBmsChargeMode == "2") BmsChargeMode.Text = "恒流";
            ChargeU.Text = Global.SqlCmd104_1[i].Parameters["@ChargeU"].Value.ToString();// CMD104: 14:直流充电电压
            ChargeI.Text = Global.SqlCmd104_1[i].Parameters["@ChargeI"].Value.ToString();// CMD104: 15:直流充电电流
            //充电机实时功率取1.2.3.4号枪之和
            double kw1=0, kw2=0, kw3=0, kw4=0,kw=0;
            string strKW1 = "", strKW2 = "", strKW3 = "", strKW4 = "";
            if (Global.SqlCmd104_1[i] != null && Global.SqlCmd104_1[i].Parameters.Count> 0)  strKW1 =Global.SqlCmd104_1[i].Parameters["@KW"].Value.ToString();//  CMD104: 39:充电功率
            if (Global.SqlCmd104_2[i] != null && Global.SqlCmd104_2[i].Parameters.Count > 0) strKW2 = Global.SqlCmd104_2[i].Parameters["@KW"].Value.ToString();
            if (Global.SqlCmd104_3[i] != null && Global.SqlCmd104_3[i].Parameters.Count > 0) strKW3 = Global.SqlCmd104_3[i].Parameters["@KW"].Value.ToString();
            if (Global.SqlCmd104_4[i] != null && Global.SqlCmd104_4[i].Parameters.Count > 0) strKW4 = Global.SqlCmd104_4[i].Parameters["@KW"].Value.ToString();
            if (strKW1 != null && strKW1 != "") kw1 = Double.Parse(strKW1);
            if (strKW2 != null && strKW2 != "") kw2 = Double.Parse(strKW2);
            if (strKW3 != null && strKW3 != "") kw3 = Double.Parse(strKW3);
            if (strKW4 != null && strKW4 != "") kw4 = Double.Parse(strKW4);
            kw = kw1 + kw2 + kw3 + kw4;
            KW.Text = kw.ToString();        
            KWH.Text = Global.SqlCmd104_1[i].Parameters["@KWH"].Value.ToString();// CMD104: 27:本次充电累计充电电量
            AU.Text = Global.SqlCmd104_1[i].Parameters["@AU"].Value.ToString();//  CMD104: 19   交流A相电压
            AI.Text = Global.SqlCmd104_1[i].Parameters["@AI"].Value.ToString();// CMD104:22 交流A相电流
            ExitC.Text = Global.SqlCmd104_1[i].Parameters["@ExitC"].Value.ToString();// CMD104:43 出风口温度 
            string strCurrentWarnMaxNo = Global.SqlCmd104_1[i].Parameters["@CurrentWarnMaxNo"].Value.ToString();// CMD104:9[0 - 无故障，非0 - 有故障] 充电机故障状态
            if (strCurrentWarnMaxNo == "0") { CurrentWarnMaxNo.Text = "无故障"; } else { CurrentWarnMaxNo.Text = "有故障"; }
            string strGunGas1 = Global.SqlCmd104_1[i].Parameters["@GunGas1"].Value.ToString();//1 || GunGas2  CMD104: 58 | 59，或逻辑 充电机气体报警
            string strGunGas2 = Global.SqlCmd104_1[i].Parameters["@GunGas2"].Value.ToString();
            if (strGunGas1 == "0" || strGunGas2=="0") { GunGas.Text = "无报警"; } else { GunGas.Text = "有报警"; }
            string strGunSmoke1 = Global.SqlCmd104_1[i].Parameters["@GunSmoke1"].Value.ToString();//1 || GunSmoke2  CMD104: 60 | 61，或逻辑 充电机烟雾报警
            string strGunSmoke2 = Global.SqlCmd104_1[i].Parameters["@GunSmoke2"].Value.ToString();
            if (strGunSmoke1 == "0" || strGunSmoke2 == "0") { GunSmoke.Text = "无报警"; } else { GunSmoke.Text = "有报警"; }
            FireLevel.Text = Global.SqlCmd104_1[i].Parameters["@FireLevel"].Value.ToString();//   CMD104: 62 充电机消防预警级别:

            BattertySN.Text = Global.SqlCmd116[i].Parameters["@BattertySN"].Value.ToString();//  电池编码    CMD116: 3 - 电池SN编码
            /*有效值 0~15，范围 (0~15) ，偏移量 0，比例因子 1/bit1“铅酸电池”2“镍氢电池”3“磷酸铁鲤电池”4“酸理电池"5“钻酸鲤电池”6"三元材料电池”7“聚合物鲤离子电池”8“钦酸鲤电池”9“超级电容”10"Reserved”11Reserved” 12Reserved”13"Reserved” 14“燃料电池5"Reserved */
            BatteryType.Text = Global.transBatteryType( Global.SqlCmd116[i].Parameters["@BatteryType"].Value.ToString());//  电池类型    CMD116: 8 - 电池类型
            BatteryCoolMethod.Text = Global.transBatteryCoolMethod(Global.SqlCmd116[i].Parameters["@BatteryCoolMethod"].Value.ToString());//  电池冷却方式  CMD116: 7 - 电池冷却方式
            PackPower.Text = Global.SqlCmd116[i].Parameters["@PackPower"].Value.ToString();//  电池额定总能量 CMD116: 6:电池包额定总能量
            HardVer.Text = Global.SqlCmd116[i].Parameters["@HardVer"].Value.ToString();//    电池硬件版本号 CMD116:41 - 硬件版本
            BatSoftVer.Text = Global.SqlCmd116[i].Parameters["@SoftVer"].Value.ToString();//  SoftVer 电池软件版本号 CMD116:42 - 软件版本
            PackSoh.Text = Global.SqlCmd116[i].Parameters["@PackSoh"].Value.ToString();//  SOH CMD116: 18:电池包SOH
            PackSoc.Text = Global.SqlCmd116[i].Parameters["@PackSoc"].Value.ToString();//  soc CMD116:17:电池包SOC
            BatteryHighU.Text = Global.SqlCmd116[i].Parameters["@BatteryHighU"].Value.ToString();//     电压 CMD116:24:电池端高压
            PackTtlI.Text = Global.SqlCmd116[i].Parameters["@PackTtlI"].Value.ToString();//     电流 CMD116:19:电池包总电流
            RemainChargeTime.Text = Global.SqlCmd104_1[i].Parameters["@RemainChargeTime"].Value.ToString();//     剩余充电时间 CMD104:25:剩余充电时间
            UnitCellUMax.Text = Global.SqlCmd116[i].Parameters["@UnitCellUMax"].Value.ToString();//     最高单体电压  116 - 31
            UnitCellUMin.Text = Global.SqlCmd116[i].Parameters["@UnitCellUMin"].Value.ToString();//  最低单体电压  116 - 32
            UnitCellCMax.Text = Global.SqlCmd116[i].Parameters["@UnitCellCMax"].Value.ToString();//  最高测点温度  116 - 26
            UnitCellCMin.Text = Global.SqlCmd116[i].Parameters["@UnitCellCMin"].Value.ToString();//  最低测点温度  116 - 27

            string strBMSWarn2 = Global.SqlCmd116[i].Parameters["@BMSWarn2"].Value.ToString(); 
            Byte[]  BytesBMSWarn2=Encoding.UTF8.GetBytes(strBMSWarn2);  //test
            
            if (Global.GetBytes(BytesBMSWarn2, 1, 0) == 0) BatBalance.Text = "未均衡"; else BatBalance.Text = "均衡";//  BMSWarn2_2  电池均衡状态 CMD116:16:Byte2: bit7 - BMS当前均衡状态：0 - 未均衡；1 - 均衡

            AGunC.Text = Global.SqlCmd116[i].Parameters["@AGunC1"].Value.ToString() + "|" + Global.SqlCmd116[i].Parameters["@AGunC2"].Value.ToString();//  充电连接器1温度    CMD116: 36~40
            BGunC.Text = Global.SqlCmd116[i].Parameters["@BGunC1"].Value.ToString() + "|" + Global.SqlCmd116[i].Parameters["@BGunC2"].Value.ToString();//  充电连接器2温度    CMD116: 36~40
            TmsWorkStatus.Text = Global.transTMSWorkStatus( Global.SqlCmd116[i].Parameters["@TmsWorkStatus"].Value.ToString());//  TMS工作状态 CMD116: 49:TMS工作状态
            TmsEffluentC.Text = Global.SqlCmd116[i].Parameters["@TmsEffluentC"].Value.ToString();//     TMS出水 / 进水温度  CMD116: 51:TMS出水温度
            TMSFaultLevel.Text = Global.SqlCmd116[i].Parameters["@TMSFaultLevel"].Value.ToString();//        TMS故障等级 CMD116:55:TMS故障等级

            if (Global.GetBytes(BytesBMSWarn2, 7, 6) == 0) BatSmoke.Text = "无报警"; else BatSmoke.Text = "有报警"; //         电池烟雾报警  116 BMSWarn2 Byte8 bit1
            if (Global.GetBytes(BytesBMSWarn2, 7, 5) == 0) BatFire.Text = "无报警"; else BatFire.Text = "有报警"; //      电池火警报警  116 BMSWarn2 Byte8 bit2
            if (Global.GetBytes(BytesBMSWarn2, 5, 1) == 0) BatInsulation.Text = "无报警"; else BatInsulation.Text = "有报警"; //         绝缘报警 CMD116:16:Byte5: bit7~6:绝缘报警
            if (Global.GetBytes(BytesBMSWarn2, 2, 2) == 0) BatFaultLeve.Text = "无报警"; else BatFaultLeve.Text = "有报警"; //     116 BMSWarn2 电池故障等级  CMD116: 16:Byte3: bit6~5最高报警等级：0 - 无故障；1 - 一级轻微故障；2 - 二级普通故障；3 - 三级严重故障；

            /*
             Global.SelChargerViewModel.ChargerNo= i.ToString(); //编码
             Global.SelChargerViewModel.SoftVersion = Global.SqlCmd116[i].Parameters["@BattertySN"].Value.ToString(); //软件版本
             Global.SelChargerViewModel.Model=" ";//充电机型号
             Global.SelChargerViewModel.V= Global.SqlCmd104_1[i].Parameters["@ChargeU"].Value.ToString()+"V";//直流电压
             Global.SelChargerViewModel.A = Global.SqlCmd104_1[i].Parameters["@ChargeI"].Value.ToString() + "A";//直流电流
             Global.SelChargerViewModel.KW= Global.SqlCmd104_1[i].Parameters["@KW"].Value.ToString() + "A";//实时功率
             Global.SelChargerViewModel.KWH= Global.SqlCmd104_1[i].Parameters["@KWH"].Value.ToString() + "A";//累计输出电量
             Global.SelChargerViewModel.Vac=" ";//
             Global.SelChargerViewModel.Vab=" ";//不用了
             Global.SelChargerViewModel.Vbc=" ";//不用了
             Global.SelChargerViewModel.Lac=" ";//
             Global.SelChargerViewModel.Lab=" ";//不用了
             Global.SelChargerViewModel.Lbc=" ";//不用了
             Global.SelChargerViewModel.PowerFactor=" ";//功率因素
            
             if( Global.SqlCmd104_1[i].Parameters["@CurrentWarnMaxNo"].Value.ToString()=="0")
                Global.SelChargerViewModel.ChargerErrStatus="无故障";//故障状态
             else
                Global.SelChargerViewModel.ChargerErrStatus = "有故障";//故障状态
                                                                    //Col 1
             Global.SelChargerViewModel.BatteryNo = Global.SqlCmd116[i].Parameters["@SoftVer"].Value.ToString();//电池编码
             Global.SelChargerViewModel.BatteryModel=" ";//电池型号
             Global.SelChargerViewModel.soh=" ";//
             Global.SelChargerViewModel.CurrentSoc=" ";//
             Global.SelChargerViewModel.CurrentKWH=" ";//当前电量
             Global.SelChargerViewModel.CurrentAh=" ";//当前容量
             Global.SelChargerViewModel.RemainMin=" ";//剩余充电电量
             Global.SelChargerViewModel.MaxV=" ";//最高电压单体
             Global.SelChargerViewModel.MinV=" ";//最低电压单体
             Global.SelChargerViewModel.MaxC = " ";//最高测点温度
             Global.SelChargerViewModel.MinC = " ";//最低测点温度
             Global.SelChargerViewModel.BatteryErrStatus =" ";//电池故障状态
            */

        }
    }
}
