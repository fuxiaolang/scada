using DESCADA.Models;
using DESCADA.Service;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WatsonTcp;
using static DESCADA.Global;
using MySql.Data.MySqlClient;
using static DESCADA.Service.Charger;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections;
using System.Windows.Threading;

namespace DESCADA
{

    public static class Global
    {
        public static string Account;
        public static int Role = 0;
        public static int LogInType = 0; // l Logon 2 exit 3 switch


        public static StationConfig config;
        public static string scadaIP = "172.16.11.34";// "192.168.3.177"; //scada电脑地址
        public static DESCADA.HttpClient httpClient; //= new DESCADA.HttpClient(StationConfig.FromFile());

        public static Switch pSwitch = null; //singlton
        public static Charge pCharge = null; //singlton
        public static Config pConfig = null; //singlton
        public static Track pTrack = null; //singlton
        public static Fire pFire = null; //singlton
        public static ConfigOper pConfigOper = null; //singlton
        public static ConfigBasic pConfigBasic = null; //singlton


        private static string vehInNO = ""; //车辆进站唯一编号
        public static string VehInNO
        {
            get
            {
                return vehInNO;
            }
            set
            {
                vehInNO = value;
            }
        }
        public static bool AllowSwitch = false; //司机app端同意换电 penging 待多通道支持
        public static bool SwitchRecordBatteryOutUpdated = false;//当前换电是否已更新借出电池信息到换电记录；
        public static bool saveRecordFromIndex = false;

        //云端车验证启用标志: true 启用，VIN从验证vehEnterReq获取， false： VIN vehserver心跳3报文取；
        //暂不用，都开启，无网用本地验证
        //public static bool vehCheckEnable = false;

        public static int JianKongViewChanel = 0;
        public static Dictionary<int, string> plcWarn = new Dictionary<int, string>();
        public static Dictionary<int, string> chargerStartWarn = new Dictionary<int, string>();
        public static Dictionary<int, string> chargerHighWarn = new Dictionary<int, string>();
        //充电启动失败编码定义
        public static Dictionary<int, string> chargerStartError = new Dictionary<int, string>();

        public class CMD108Warn
        {
            public int ByteNo { get; set; }
            public int start { get; set; }
            public string FaultInfo { get; set; }
            public int Faultlevel { get; set; }
            public int FaultCode { get; set; }
        }
        public static List<CMD108Warn> CMD108WarnList = new List<CMD108Warn>()
        {
            new CMD108Warn(){ByteNo=1,start=1,FaultInfo="紧急停机",Faultlevel=3,FaultCode=1},
            new CMD108Warn(){ByteNo=1,start=2,FaultInfo="绝缘故障",Faultlevel=3,FaultCode=2},
            new CMD108Warn(){ByteNo=1,start=3,FaultInfo="直流过压",Faultlevel=3,FaultCode=3},
            new CMD108Warn(){ByteNo=1,start=4,FaultInfo="直流欠压",Faultlevel=3,FaultCode=4},
            new CMD108Warn(){ByteNo=1,start=5,FaultInfo="软启失败",Faultlevel=2,FaultCode=5},
            new CMD108Warn(){ByteNo=1,start=6,FaultInfo="直流输出反接",Faultlevel=3,FaultCode=6},
            new CMD108Warn(){ByteNo=1,start=7,FaultInfo="直流接触器异常",Faultlevel=3,FaultCode=7},
            new CMD108Warn(){ByteNo=2,start=0,FaultInfo="模块故障",Faultlevel=3,FaultCode=8},
            new CMD108Warn(){ByteNo=2,start=1,FaultInfo="交流输入过压",Faultlevel=2,FaultCode=9},
            new CMD108Warn(){ByteNo=2,start=2,FaultInfo="交流输入欠压",Faultlevel=2,FaultCode=10},
            new CMD108Warn(){ByteNo=2,start=3,FaultInfo="交流输入过频",Faultlevel=2,FaultCode=11},
            new CMD108Warn(){ByteNo=2,start=4,FaultInfo="交流输入欠频",Faultlevel=2,FaultCode=12},
            new CMD108Warn(){ByteNo=2,start=5,FaultInfo="模块通信异常",Faultlevel=3,FaultCode=13},
            new CMD108Warn(){ByteNo=2,start=6,FaultInfo="模块类型不一致",Faultlevel=3,FaultCode=14},
            new CMD108Warn(){ByteNo=2,start=7,FaultInfo="系统辅源掉电",Faultlevel=2,FaultCode=15},
            new CMD108Warn(){ByteNo=3,start=0,FaultInfo="直流输出断路",Faultlevel=3,FaultCode=16},
            new CMD108Warn(){ByteNo=3,start=1,FaultInfo="进风口过温保护",Faultlevel=1,FaultCode=17},
            new CMD108Warn(){ByteNo=3,start=2,FaultInfo="进风口低温保护",Faultlevel=1,FaultCode=18},
            new CMD108Warn(){ByteNo=3,start=3,FaultInfo="出风口过温保护",Faultlevel=1,FaultCode=19},
            new CMD108Warn(){ByteNo=3,start=4,FaultInfo="群充模块过温",Faultlevel=3,FaultCode=20},
            new CMD108Warn(){ByteNo=3,start=5,FaultInfo="防雷故障",Faultlevel=2,FaultCode=21},
            new CMD108Warn(){ByteNo=3,start=6,FaultInfo="交流接触器异常",Faultlevel=2,FaultCode=22},
            new CMD108Warn(){ByteNo=3,start=7,FaultInfo="充电枪头过温",Faultlevel=2,FaultCode=23},
            new CMD108Warn(){ByteNo=4,start=0,FaultInfo="直流输出过流",Faultlevel=1,FaultCode=24},
            new CMD108Warn(){ByteNo=4,start=1,FaultInfo="充电枪锁异常",Faultlevel=2,FaultCode=25},
            new CMD108Warn(){ByteNo=4,start=2,FaultInfo="快充段，此枪无效",Faultlevel=1,FaultCode=26},
            new CMD108Warn(){ByteNo=4,start=3,FaultInfo="门禁保护",Faultlevel=1,FaultCode=27},
            new CMD108Warn(){ByteNo=4,start=4,FaultInfo="CAN3通信错误（扩展板）",Faultlevel=1,FaultCode=28},
            new CMD108Warn(){ByteNo=4,start=5,FaultInfo="群充模块通信故障（new）",Faultlevel=3,FaultCode=29},
            new CMD108Warn(){ByteNo=4,start=6,FaultInfo="输入缺相",Faultlevel=2,FaultCode=30},
            new CMD108Warn(){ByteNo=4,start=7,FaultInfo="控制导引故障",Faultlevel=1,FaultCode=31},
            new CMD108Warn(){ByteNo=5,start=0,FaultInfo="模块未准备就绪",Faultlevel=1,FaultCode=32},
            new CMD108Warn(){ByteNo=5,start=1,FaultInfo="预留",FaultCode=33},
            new CMD108Warn(){ByteNo=5,start=2,FaultInfo="枪锁故障开锁失败",Faultlevel=2,FaultCode=34},
            new CMD108Warn(){ByteNo=5,start=3,FaultInfo="枪锁故障关锁失败",Faultlevel=2,FaultCode=35},
            new CMD108Warn(){ByteNo=5,start=4,FaultInfo="软起失败，模块没有开启",Faultlevel=1,FaultCode=36},
            new CMD108Warn(){ByteNo=5,start=5,FaultInfo="软件失败，电池电压没有检测到",Faultlevel=1,FaultCode=37},
            new CMD108Warn(){ByteNo=5,start=6,FaultInfo="电网电压高，告警不停机",Faultlevel=1,FaultCode=38},
            new CMD108Warn(){ByteNo=5,start=7,FaultInfo="电网电压低，告警不停机",Faultlevel=1,FaultCode=39},
            new CMD108Warn(){ByteNo=6,start=0,FaultInfo="绝缘异常",Faultlevel=3,FaultCode=40},
            new CMD108Warn(){ByteNo=6,start=1,FaultInfo="输出短路",Faultlevel=2,FaultCode=41},
            new CMD108Warn(){ByteNo=6,start=2,FaultInfo="模块过温",Faultlevel=1,FaultCode=42},
            new CMD108Warn(){ByteNo=6,start=3,FaultInfo="模块异常",Faultlevel=1,FaultCode=43},

         };
        public class BMSWarn
        {
            public int ByteNo { get; set; }
            public int start { get; set; }
            public int FaultValue { get; set; }
            public int MulFlag { get; set; }
            public string FaultInfo { get; set; }
            public int Faultlevel { get; set; }
            public int FaultCode { get; set; }
        }
        public static List<BMSWarn> BMSWarn2List = new List<BMSWarn>()
        {
            new BMSWarn(){ByteNo=2,start=4,FaultValue=3,MulFlag=1,FaultInfo="高压上电故障",Faultlevel=2,FaultCode=129},
           new BMSWarn(){ByteNo=3,start=3,FaultValue=3,MulFlag=1,FaultInfo="充电故障",Faultlevel=2,FaultCode=130},
           new BMSWarn(){ByteNo=4,start=0,FaultValue=3,MulFlag=1,FaultInfo="电芯温差异常3级严重故障",Faultlevel=2,FaultCode=131},
           new BMSWarn(){ByteNo=4,start=0,FaultValue=2,MulFlag=1,FaultInfo="电芯温差异常2级普通故障",Faultlevel=2,FaultCode=132},
           new BMSWarn(){ByteNo=4,start=0,FaultValue=1,MulFlag=1,FaultInfo="电芯温差异常1级报警故障",Faultlevel=1,FaultCode=133},
           new BMSWarn(){ByteNo=4,start=2,FaultValue=3,MulFlag=1,FaultInfo="电芯温度过高3级严重故障",Faultlevel=3,FaultCode=134},
           new BMSWarn(){ByteNo=4,start=2,FaultValue=2,MulFlag=1,FaultInfo="电芯温度过高2级普通故障",Faultlevel=2,FaultCode=135},
           new BMSWarn(){ByteNo=4,start=2,FaultValue=1,MulFlag=1,FaultInfo="电芯温度过高1级报警故障",Faultlevel=1,FaultCode=136},
           new BMSWarn(){ByteNo=4,start=4,FaultValue=3,MulFlag=1,FaultInfo="Pack过压3级严重故障",Faultlevel=2,FaultCode=137},
           new BMSWarn(){ByteNo=4,start=4,FaultValue=2,MulFlag=1,FaultInfo="Pack过压2级普通故障",Faultlevel=2,FaultCode=138},
           new BMSWarn(){ByteNo=4,start=4,FaultValue=1,MulFlag=1,FaultInfo="Pack过压1级报警故障",Faultlevel=1,FaultCode=139},
           new BMSWarn(){ByteNo=4,start=6,FaultValue=3,MulFlag=1,FaultInfo="Pack欠压3级严重故障",Faultlevel=2,FaultCode=140},
           new BMSWarn(){ByteNo=4,start=6,FaultValue=2,MulFlag=1,FaultInfo="Pack欠压2级普通故障",Faultlevel=2,FaultCode=141},
           new BMSWarn(){ByteNo=4,start=6,FaultValue=1,MulFlag=1,FaultInfo="Pack欠压1级报警故障",Faultlevel=1,FaultCode=142},
           new BMSWarn(){ByteNo=5,start=0,FaultValue=3,MulFlag=1,FaultInfo="SOC过低3级严重故障",Faultlevel=2,FaultCode=143},
           new BMSWarn(){ByteNo=5,start=0,FaultValue=2,MulFlag=1,FaultInfo="SOC过低2级普通故障",Faultlevel=2,FaultCode=144},
           new BMSWarn(){ByteNo=5,start=0,FaultValue=1,MulFlag=1,FaultInfo="SOC过低1级报警故障",Faultlevel=1,FaultCode=145},
           new BMSWarn(){ByteNo=5,start=2,FaultValue=3,MulFlag=1,FaultInfo="单体过压3级严重故障",Faultlevel=2,FaultCode=146},
           new BMSWarn(){ByteNo=5,start=2,FaultValue=2,MulFlag=1,FaultInfo="单体过压2级普通故障",Faultlevel=2,FaultCode=147},
           new BMSWarn(){ByteNo=5,start=2,FaultValue=1,MulFlag=1,FaultInfo="单体过压1级报警故障",Faultlevel=1,FaultCode=148},
           new BMSWarn(){ByteNo=5,start=4,FaultValue=3,MulFlag=1,FaultInfo="单体欠压3级严重故障",Faultlevel=2,FaultCode=149},
           new BMSWarn(){ByteNo=5,start=4,FaultValue=2,MulFlag=1,FaultInfo="单体欠压2级普通故障",Faultlevel=2,FaultCode=150},
           new BMSWarn(){ByteNo=5,start=4,FaultValue=1,MulFlag=1,FaultInfo="单体欠压1级报警故障",Faultlevel=1,FaultCode=151},
           new BMSWarn(){ByteNo=5,start=6,FaultValue=3,MulFlag=1,FaultInfo="绝缘3级严重故障",Faultlevel=3,FaultCode=152},
           new BMSWarn(){ByteNo=5,start=6,FaultValue=2,MulFlag=1,FaultInfo="绝缘2级普通故障",Faultlevel=2,FaultCode=153},
           new BMSWarn(){ByteNo=5,start=6,FaultValue=1,MulFlag=1,FaultInfo="绝缘1级报警故障",Faultlevel=1,FaultCode=154},
           new BMSWarn(){ByteNo=6,start=0,FaultValue=3,MulFlag=1,FaultInfo="单体压差过大3级严重故障",Faultlevel=2,FaultCode=155},
           new BMSWarn(){ByteNo=6,start=0,FaultValue=2,MulFlag=1,FaultInfo="单体压差过大2级普通故障",Faultlevel=2,FaultCode=156},
           new BMSWarn(){ByteNo=6,start=0,FaultValue=1,MulFlag=1,FaultInfo="单体压差过大1级报警故障",Faultlevel=1,FaultCode=157},
           new BMSWarn(){ByteNo=6,start=2,FaultValue=3,MulFlag=1,FaultInfo="充电电流过大3级严重故障",Faultlevel=2,FaultCode=158},
           new BMSWarn(){ByteNo=6,start=2,FaultValue=2,MulFlag=1,FaultInfo="充电电流过大2级普通故障",Faultlevel=2,FaultCode=159},
           new BMSWarn(){ByteNo=6,start=2,FaultValue=1,MulFlag=1,FaultInfo="充电电流过大1级报警故障",Faultlevel=1,FaultCode=160},
           new BMSWarn(){ByteNo=6,start=4,FaultValue=3,MulFlag=1,FaultInfo="放电电流过大3级严重故障",Faultlevel=2,FaultCode=161},
           new BMSWarn(){ByteNo=6,start=4,FaultValue=2,MulFlag=1,FaultInfo="放电电流过大2级普通故障",Faultlevel=2,FaultCode=162},
           new BMSWarn(){ByteNo=6,start=4,FaultValue=1,MulFlag=1,FaultInfo="放电电流过大1级报警故障",Faultlevel=1,FaultCode=163},
           new BMSWarn(){ByteNo=6,start=6,FaultValue=3,MulFlag=1,FaultInfo="电芯温度过低3级严重故障",Faultlevel=2,FaultCode=164},
           new BMSWarn(){ByteNo=6,start=6,FaultValue=2,MulFlag=1,FaultInfo="电芯温度过低2级普通故障",Faultlevel=2,FaultCode=165},
           new BMSWarn(){ByteNo=6,start=6,FaultValue=1,MulFlag=1,FaultInfo="电芯温度过低1级报警故障",Faultlevel=1,FaultCode=166},
           new BMSWarn(){ByteNo=7,start=0,FaultValue=3,MulFlag=1,FaultInfo="支路压差过大3级严重故障",Faultlevel=2,FaultCode=167},
           new BMSWarn(){ByteNo=7,start=0,FaultValue=2,MulFlag=1,FaultInfo="支路压差过大2级普通故障",Faultlevel=2,FaultCode=168},
           new BMSWarn(){ByteNo=7,start=0,FaultValue=1,MulFlag=1,FaultInfo="支路压差过大1级报警故障",Faultlevel=1,FaultCode=169},
           new BMSWarn(){ByteNo=7,start=2,FaultValue=3,MulFlag=1,FaultInfo="BMS硬件3级严重故障",Faultlevel=2,FaultCode=170},
           new BMSWarn(){ByteNo=7,start=2,FaultValue=2,MulFlag=1,FaultInfo="BMS硬件2级普通故障",Faultlevel=2,FaultCode=171},
           new BMSWarn(){ByteNo=7,start=2,FaultValue=1,MulFlag=1,FaultInfo="BMS硬件1级报警故障",Faultlevel=1,FaultCode=172},
           new BMSWarn(){ByteNo=7,start=4,FaultValue=1,MulFlag=0,FaultInfo="SOC过高报警",Faultlevel=2,FaultCode=173},
           new BMSWarn(){ByteNo=7,start=5,FaultValue=1,MulFlag=0,FaultInfo="SOC跳变报警",Faultlevel=1,FaultCode=174},
           new BMSWarn(){ByteNo=7,start=6,FaultValue=1,MulFlag=0,FaultInfo="BMS内部通讯故障",Faultlevel=2,FaultCode=175},
           new BMSWarn(){ByteNo=7,start=7,FaultValue=1,MulFlag=0,FaultInfo="BMS系统不匹配",Faultlevel=1,FaultCode=176},
           new BMSWarn(){ByteNo=8,start=0,FaultValue=1,MulFlag=1,FaultInfo="高压互锁报警",Faultlevel=2,FaultCode=177},
           new BMSWarn(){ByteNo=8,start=1,FaultValue=1,MulFlag=1,FaultInfo="烟雾报警",Faultlevel=3,FaultCode=178},
           new BMSWarn(){ByteNo=8,start=2,FaultValue=1,MulFlag=1,FaultInfo="火灾报警",Faultlevel=3,FaultCode=179},

        };
        public static List<BMSWarn> BMSWarn1List = new List<BMSWarn>()
        {
            new BMSWarn(){ByteNo=1,start=4,FaultValue=1,MulFlag=1,FaultInfo="主正继电器粘连故障",Faultlevel=1,FaultCode=192},
            new BMSWarn(){ByteNo=1,start=5,FaultValue=1,MulFlag=1,FaultInfo="主负继电器粘连故障",Faultlevel=1,FaultCode=193},
            new BMSWarn(){ByteNo=1,start=6,FaultValue=1,MulFlag=1,FaultInfo="充正1继电器粘连故障",Faultlevel=1,FaultCode=194},
            new BMSWarn(){ByteNo=1,start=7,FaultValue=1,MulFlag=1,FaultInfo="充负1继电器粘连故障",Faultlevel=1,FaultCode=195},
            new BMSWarn(){ByteNo=2,start=0,FaultValue=1,MulFlag=1,FaultInfo="充正2继电器粘连故障",Faultlevel=1,FaultCode=196},
            new BMSWarn(){ByteNo=2,start=1,FaultValue=1,MulFlag=1,FaultInfo="充负2继电器粘连故障",Faultlevel=1,FaultCode=197},
            new BMSWarn(){ByteNo=2,start=2,FaultValue=1,MulFlag=1,FaultInfo="加热1继电器粘连故障",Faultlevel=1,FaultCode=198},
            new BMSWarn(){ByteNo=2,start=3,FaultValue=1,MulFlag=1,FaultInfo="加热2继电器粘连故障",Faultlevel=1,FaultCode=199},
            new BMSWarn(){ByteNo=4,start=2,FaultValue=1,MulFlag=1,FaultInfo="预留",Faultlevel=0,FaultCode=200},
            new BMSWarn(){ByteNo=4,start=3,FaultValue=1,MulFlag=1,FaultInfo="支路断路故障",Faultlevel=1,FaultCode=201},
            new BMSWarn(){ByteNo=4,start=5,FaultValue=1,MulFlag=1,FaultInfo="回充电流超限故障",Faultlevel=1,FaultCode=202},
            new BMSWarn(){ByteNo=4,start=6,FaultValue=1,MulFlag=1,FaultInfo="主正继电器无法闭合报警",Faultlevel=1,FaultCode=203},
            new BMSWarn(){ByteNo=4,start=7,FaultValue=1,MulFlag=1,FaultInfo="主负继电器无法闭合报警",Faultlevel=1,FaultCode=204},
            new BMSWarn(){ByteNo=5,start=0,FaultValue=1,MulFlag=1,FaultInfo="直流充电1正继电器无法闭合报警",Faultlevel=2,FaultCode=205},
            new BMSWarn(){ByteNo=5,start=1,FaultValue=1,MulFlag=1,FaultInfo="直流充电2正继电器无法闭合报警",Faultlevel=2,FaultCode=206},
            new BMSWarn(){ByteNo=5,start=2,FaultValue=1,MulFlag=1,FaultInfo="直流充电1负继电器无法闭合报警",Faultlevel=2,FaultCode=207},
            new BMSWarn(){ByteNo=5,start=3,FaultValue=1,MulFlag=1,FaultInfo="直流充电2负继电器无法闭合报警",Faultlevel=2,FaultCode=208},
            new BMSWarn(){ByteNo=5,start=4,FaultValue=1,MulFlag=1,FaultInfo="加热膜或TMS接触器无法断开报警",Faultlevel=1,FaultCode=209},
            new BMSWarn(){ByteNo=5,start=5,FaultValue=1,MulFlag=1,FaultInfo="加热膜或TMS接触器无法闭合故障",Faultlevel=1,FaultCode=210},
            new BMSWarn(){ByteNo=5,start=6,FaultValue=1,MulFlag=1,FaultInfo="热管理系统故障",Faultlevel=1,FaultCode=211},
            new BMSWarn(){ByteNo=5,start=7,FaultValue=1,MulFlag=1,FaultInfo="BMS24V供电异常报警",Faultlevel=1,FaultCode=212},
            new BMSWarn(){ByteNo=6,start=0,FaultValue=1,MulFlag=1,FaultInfo="电池包自保护报警",Faultlevel=1,FaultCode=213},
            new BMSWarn(){ByteNo=6,start=1,FaultValue=2,MulFlag=1,FaultInfo="充电插座过温2级严重故障",Faultlevel=2,FaultCode=214},
            new BMSWarn(){ByteNo=6,start=1,FaultValue=1,MulFlag=1,FaultInfo="充电插座过温1级故障",Faultlevel=1,FaultCode=215},
        };
        public static string entryUniqueCode = "";
        public static DBTrans dbTrans = new DBTrans();



        public static ChargerServer chargerServer = null; //singlton
        public static string ChargerPolicyNo = ""; //云端MQT4003中下发的policyno
        public static int[] ChargerEnableFlag = new int[8]; //1 启用 0 停用
        public static int[] ChargerStatus = new int[8];//充电机状态 3s 有延迟，所以实时的要直接取PLC信号
        public static int[] ChargerFaultStatus = new int[8];//暂未用：直接取PLC了，充电机故障状态  //1 有故障 0 无故障
        public static int[,] ChargerGunFaultStatus = new int[8, 5];//暂未用：直接取PLC了，充电枪故障状态  //1 有故障 0 无故障
        public static int[] ChargerFireStatus = new int[8];//暂未用：直接取PLC了，充电机火警状态 //1 有火警 0 无火警


        public static int PowerOffApply;//下电请求 0 未完成  1 完成
        public static DateTime PowerOffApplyTime;//下电申请时间
        public static int PowerOffChargerID;//下电仓位 换电记录：取放同仓（调仓的情况不适用，后续pending转lichang）
        public static DateTime PowerOffTime;//下电完成时间

        public static int chargerMsgNo = 0; //报文流水号 
        public static int ChargerMsgNo
        {
            get
            {
                return chargerMsgNo;
            }
            set
            {
                if (value == 255)
                    chargerMsgNo = 0;
                else
                    chargerMsgNo = value;
            }
        }
        public static Charger.CMD2 cmd2;
        public static Charger.CMD4 cmd4;
        public static Charger.CMD6 cmd6;
        public static Charger.CMD8 cmd8;
        public static Charger.CMD12 cmd12;
        public static Charger.CMD102 cmd102;
        public static Charger.CMD104 cmd104;
        public static Charger.CMD106 cmd106;
        public static Charger.CMD108 cmd108;
        public static Charger.CMD222 cmd222;
        public static Charger.CMD302 cmd302;
        public static Charger.CMD1104 cmd1104;
        public static Charger.CMD116 cmd116;
        public static Charger.CMD118 cmd118;
        public static Charger.CMD120 cmd120;
        public static Charger.CMD208 cmd208;

        public static MySqlCommand[] SqlCmd2 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd4 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd6 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd8 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd12 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd102 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd104 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd104_1 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd104_2 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd104_3 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd104_4 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd106 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd108 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd222 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd302 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd1104 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd116 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd118 = new MySqlCommand[8];
        public static MySqlCommand[] SqlCmd120 = new MySqlCommand[8];
        public static string[] CMD118JsonData = new string[8];
        public static string[] CMD120JsonData = new string[8];
        public static Byte[] oldCmd8RequestResult = new Byte[8];

        public static int[,] ChargerStartApplyFlag = new int[8, 5];  //8 桩 5 枪 转发云端启动充电机申请  0 未申请 1 已申请 2 已回执A 
        public static int[,] ChargerStartApplyFlagB = new int[8, 5];  //8 桩 5 枪 转发云端启动充电机申请  0 未回执B  1 已回执B 
        public static int[,] ChargerStartApplyType = new int[8, 5];  //转发云端启动充电机申请类型   1 启动 2 停止
        public static DateTime[,] ChargerStartApplyTime = new DateTime[8, 5];  //转发云端启动充电机申请时间
        public static string[,] ChargerReqId = new string[8, 5]; //请求充电订单号 string 启停使用同一个订单号
        public static string[,] ChargerReqpolicyNo = new string[8, 5]; //时段策略编号

        public static Byte[] cmd106RandNum;
        //由0变为1时判为电池入仓
        public static bool[] PLCW4_locationCheckHandled = { true, true, true, true, true, true, true, true };
        //-1默认，0 ：1变0  1：0变1
        public static int[] PLCW4_locationCheck = { -1, -1, -1, -1, -1, -1, -1, -1 };

        //***********************************************

        //public static PLCServer pLCServer = null; //singlton
        public static bool PLCSendLock = false;
        public static PLC.PLCW4 PLCMsg4;
        public static PLC.PLCW5 PLCMsg5;
        public static PLC.PLCW6 PLCMsg6;
        public static PLC.PLCR1 PLCMsg1;
        public static PLC.PLCR2 PLCMsg2;
        public static PLC.PLCR3 PLCMsg3;
        public static int oldExchangeDone = 0;
        public static bool ExchangeDoneHandled = false; //换电完成信号默认未处理（提示）

        public static string PLCDeviceCode = "00001";
        public static string[] infoErrFlag1 = new string[] { "急停按钮按下", "通道区急停按下", "HMI急停按下", "消防区急停按下", "维修门开关打开", "回原点超时报警", "", "", "", "", "", "", "", "", "", "", "", "", }; //不定长      
        public static string[] infoErrFlag2 = new string[] { "X轴伺服故障", "X轴正限位报警", "X轴负限位报警", "X轴正限位软极限报警", "X轴负限位软极限报警", "X轴扭矩过载报警", "X轴未设原点", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag3 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag4 = new string[] { "Y轴伺服故障", "Y轴正限位动作", "Y轴负限位动作", "Y轴正限位软极限报警", "Y轴负限位软极限报警", "Y轴扭矩过载报警", "Y轴未设原点", "Y轴原点传感器OFF报警", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag5 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag6 = new string[] { "Z轴伺服故障", "Z轴正限位动作", "Z轴负限位动作", "Z轴正限位软极限报警", "Z轴负限位软极限报警", "Z轴扭矩过载报警", "Z轴未设原点", "Z轴原点传感器OFF报警", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag7 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag8 = new string[] { "R轴伺服故障", "", "", "R轴正限位软极限报警", "R轴负限位软极限报警", "R轴扭矩过载报警", "R轴未设原点", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag9 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag10 = new string[] { "锁止电缸1步进报警", "锁止电缸2步进报警", "锁止电缸3步进报警", "锁止电缸4步进报警", "锁止电缸1伸出超时报警", "锁止电缸1缩回超时报警", "锁止电缸2伸出超时报警", "锁止电缸2缩回超时报警", "锁止电缸3伸出超时报警", "锁止电缸3缩回超时报警", "锁止电缸4伸出超时报警", "锁止电缸4缩回超时报警", "", "", "", "" };
        public static string[] infoErrFlag11 = new string[] { "插销电缸1步进报警", "插销电缸2步进报警", "插销电缸1伸出超时报警", "插销电缸1缩回超时报警", "插销电缸2伸出超时报警", "插销电缸2缩回超时报警", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag12 = new string[] { "吊具到位传感器1未触发", "吊具到位传感器2未触发", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag13 = new string[] { "激光雷达旋转角度校验NG", "激光雷达X偏差值校验NG", "激光雷达Y偏差值校验NG", "激光雷达值读取超时报警", "激光反馈值为默认值", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag14 = new string[] { "车辆解锁超时报警", "车辆上锁超时报警", "车辆解锁故障报警", "车辆无电池检测报警", "车辆有电池检测报警", "车辆未熄火报警", "车辆未空挡报警", "车辆未拉手刹报警", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag15 = new string[] { "中转仓位有无电池检测报警", "1号仓位有无电池检测报警", "2号仓位有无电池检测报警", "3号仓位有无电池检测报警", "4号仓位有无电池检测报警", "5号仓位有无电池检测报警", "6号仓位有无电池检测报警", "7号仓位有无电池检测报警", "取放电池，充电机1未断电报警", "取放电池，充电机2未断电报警", "取放电池，充电机3未断电报警", "取放电池，充电机4未断电报警", "取放电池，充电机5未断电报警", "取放电池，充电机6未断电报警", "取放电池，充电机7未断电报警", "" };
        public static string[] infoErrFlag16 = new string[] { "换电门打开超时报警", "换电门关闭超时报警", "消防门打开超时报警", "消防门关闭超时报警", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag17 = new string[] { "2.2kw变频风机1故障", " 2.2kw变频风机2故障", " 0.5kw风机1故障", " 0.5kw风机2故障", "0.5kw风机3故障", "0.5kw风机4故障", "油泵高压报警1", "油泵高压报警2", "油泵液位传感器报警", "", "", "", "", "", "", "" };
        public static string[] infoErrFlag18 = new string[] { "TCP通讯中断", "EtherCAT总线报警", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        public static int[] levelErrFlag1 = new int[] { 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //不定长      
        public static int[] levelErrFlag2 = new int[] { 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag3 = new int[] { 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag4 = new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag5 = new int[] { 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag6 = new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag7 = new int[] { 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag8 = new int[] { 2, 0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag9 = new int[] { 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag10 = new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 };
        public static int[] levelErrFlag11 = new int[] { 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag12 = new int[] { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag13 = new int[] { 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag14 = new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag15 = new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 };
        public static int[] levelErrFlag16 = new int[] { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag17 = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] levelErrFlag18 = new int[] { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static int[] NoErrFlag1 = new int[] { 256, 257, 258, 259, 260, 261, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag2 = new int[] { 264, 265, 266, 267, 268, 269, 270, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag3 = new int[] { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag4 = new int[] { 272, 274, 274, 275, 276, 277, 278, 279, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag5 = new int[] { 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag6 = new int[] { 280, 281, 282, 283, 284, 285, 286, 287, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag7 = new int[] { 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag8 = new int[] { 288, 289, 290, 291, 292, 293, 294, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag9 = new int[] { 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag10 = new int[] { 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 0, 0, 0, 0 };
        public static int[] NoErrFlag11 = new int[] { 312, 313, 314, 315, 316, 317, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag12 = new int[] { 320, 321, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag13 = new int[] { 328, 329, 330, 331, 332, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag14 = new int[] { 336, 337, 338, 339, 340, 341, 342, 343, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag15 = new int[] { 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 0 };
        public static int[] NoErrFlag16 = new int[] { 360, 361, 362, 363, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag17 = new int[] { 368, 369, 370, 371, 372, 373, 374, 375, 376, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] NoErrFlag18 = new int[] { 382, 382, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static MqttClientService mgttClientService = null;


        public static int PCPutNum = 0;
        public static ChargerViewModel SelChargerViewModel = new ChargerViewModel();
        public static bool ChargerViewISShow = false;

        public static void initCharger()
        {
            initChargerStartWarn();
            initChargerHighWarn();
            initChargerEnableFlag();
            initChargerStartError();
        }

        //附录3  充电启动失败编码定义
        public static void initChargerStartError()
        {
            chargerStartError.Add(1, "CC1未连接");
            chargerStartError.Add(2, "绝缘检测超时");
            chargerStartError.Add(3, "绝缘检测异常");
            chargerStartError.Add(4, "充电机暂停服务");
            chargerStartError.Add(5, "充电机系统故障, 不能充电");
            chargerStartError.Add(6, "辅电不匹配");
            chargerStartError.Add(7, "辅电开启失败");
            chargerStartError.Add(9, "充电启动超时");
            chargerStartError.Add(10, "BMS通信握手失败");
            chargerStartError.Add(11, "BMS通信配置失败");
            chargerStartError.Add(12, "BMS参数异常");
            chargerStartError.Add(13, "桩正在充电中，不能再启动");
            chargerStartError.Add(14, "本地模式，不能启动充电");
            chargerStartError.Add(15, "启动未知错误");
            chargerStartError.Add(16, "桩已预约，启动失败");
            chargerStartError.Add(17, "预约枪口号不存在的");
            chargerStartError.Add(18, "预约账户不一致");
            chargerStartError.Add(19, "保留");
            chargerStartError.Add(20, "参数错误1");
            chargerStartError.Add(21, "参数错误2");
            chargerStartError.Add(30, "其他未定义失败");
            chargerStartError.Add(80, "设备不可用");
            chargerStartError.Add(96, "不允许充电操作");
            chargerStartError.Add(100001, "充电机系统故障");
            chargerStartError.Add(100002, "车辆准备就绪超时");
            chargerStartError.Add(100003, "桩正在充电中，不能再启动");
            chargerStartError.Add(100004, "本地模式，不能启动充电");
            chargerStartError.Add(100005, "枪口号不对");
        }
        public static void initChargerHighWarn()
        {
            chargerStartWarn.Add(1, "CC1未连接");
            chargerStartWarn.Add(2, "绝缘检测超时");
            chargerStartWarn.Add(3, "绝缘检测异常");
            chargerStartWarn.Add(4, "充电机暂停服务");
            chargerStartWarn.Add(5, "充电机系统故障, 不能充电");
            chargerStartWarn.Add(6, "辅电不匹配");
            chargerStartWarn.Add(7, "辅电开启失败");
            chargerStartWarn.Add(9, "充电启动超时");
            chargerStartWarn.Add(10, "BMS通信握手失败");
            chargerStartWarn.Add(11, "BMS通信配置失败");
            chargerStartWarn.Add(12, "BMS参数异常");
            chargerStartWarn.Add(13, "桩正在充电中，不能再启动");
            chargerStartWarn.Add(14, "本地模式，不能启动充电");
            chargerStartWarn.Add(15, "启动未知错误");
            chargerStartWarn.Add(16, "桩已预约，启动失败");
            chargerStartWarn.Add(17, "预约枪口号不存在的");
            chargerStartWarn.Add(18, "预约账户不一致");
            chargerStartWarn.Add(19, "保留");
            chargerStartWarn.Add(20, "参数错误1");
            chargerStartWarn.Add(21, "参数错误2");
            chargerStartWarn.Add(30, "其他未定义失败");
            chargerStartWarn.Add(80, "设备不可用");
            chargerStartWarn.Add(96, "不允许充电操作");
            chargerStartWarn.Add(100001, "充电机系统故障");
            chargerStartWarn.Add(100002, "车辆准备就绪超时");
            chargerStartWarn.Add(100003, "桩正在充电中，不能再启动");
            chargerStartWarn.Add(100004, "本地模式，不能启动充电");
            chargerStartWarn.Add(100005, "枪口号不对");

        }
        public static void initChargerStartWarn()
        {
            chargerHighWarn.Add(0, "无告警");
            chargerHighWarn.Add(1, "绝缘检测异常");
            chargerHighWarn.Add(2, "预留");
            chargerHighWarn.Add(3, "紧急停机");
            chargerHighWarn.Add(4, "预留");
            chargerHighWarn.Add(5, "直流输出过压");
            chargerHighWarn.Add(6, "直流输出欠压");
            chargerHighWarn.Add(7, "预留");
            chargerHighWarn.Add(8, "直流输出断路");
            chargerHighWarn.Add(9, "环境温度过高");
            chargerHighWarn.Add(10, "预留");
            chargerHighWarn.Add(11, "预留");
            chargerHighWarn.Add(12, "预留");
            chargerHighWarn.Add(13, "直流输出反接");
            chargerHighWarn.Add(14, "预留");
            chargerHighWarn.Add(15, "预留");
            chargerHighWarn.Add(16, "模块类型不一致");
            chargerHighWarn.Add(17, "熔断器故障");
            chargerHighWarn.Add(18, "直流接触器异常");
            chargerHighWarn.Add(19, "模块故障");
            chargerHighWarn.Add(20, "模块CAN通信异常");
            chargerHighWarn.Add(21, "保留");
            chargerHighWarn.Add(22, "交流输入电压过压");
            chargerHighWarn.Add(23, "交流输入电压欠压");
            chargerHighWarn.Add(24, "交流输入频率过频");
            chargerHighWarn.Add(25, "交流输入频率欠频");
            chargerHighWarn.Add(26, "保留");
            chargerHighWarn.Add(27, "防雷器故障");
            chargerHighWarn.Add(28, "保留");
            chargerHighWarn.Add(29, "交流AC输入A相缺相");
            chargerHighWarn.Add(30, "交流AC输入B相缺相");
            chargerHighWarn.Add(31, "交流AC输入C相缺相");
            chargerHighWarn.Add(32, "直流输出短路");
            chargerHighWarn.Add(33, "充电枪锁故障");
            chargerHighWarn.Add(34, "低压辅源异常");
            chargerHighWarn.Add(35, "充电枪温度过高");
            chargerHighWarn.Add(36, "采集器代码错误");
            chargerHighWarn.Add(40, "保留");
            chargerHighWarn.Add(1000, "控制板通讯故障");
            chargerHighWarn.Add(1001, "采集板通讯故障");
            chargerHighWarn.Add(1002, "电表通讯异常");
            chargerHighWarn.Add(1003, "与集中器通信中断");
            chargerHighWarn.Add(1004, "后台通信中断");
            chargerHighWarn.Add(1005, "保留");
            chargerHighWarn.Add(1006, "读卡器故障");
            chargerHighWarn.Add(1007, "电表电量为0");
            chargerHighWarn.Add(2001, "紧急停机故障");
            chargerHighWarn.Add(2002, "绝缘故障");
            chargerHighWarn.Add(2003, "直流过压");
            chargerHighWarn.Add(2004, "直流欠压");
            chargerHighWarn.Add(2005, "软启失败");
            chargerHighWarn.Add(2006, "输出反接故障");
            chargerHighWarn.Add(2007, "接触器异常");
            chargerHighWarn.Add(2008, "模块故障");
            chargerHighWarn.Add(2009, "电网电压高");
            chargerHighWarn.Add(2010, "电网电压低");
            chargerHighWarn.Add(2011, "电网频率高");
            chargerHighWarn.Add(2012, "电网频率低");
            chargerHighWarn.Add(2013, "模块通信异常");
            chargerHighWarn.Add(2014, "模块类型不一致");
            chargerHighWarn.Add(2015, "充电机系统掉电");
            chargerHighWarn.Add(2016, "直流输出断路");
            chargerHighWarn.Add(2017, "进风口过温保护");
            chargerHighWarn.Add(2018, "进风口低温保护");
            chargerHighWarn.Add(2019, "出风口过温保护");
            chargerHighWarn.Add(2020, "群充模块过温");
            chargerHighWarn.Add(2021, "防雷故障");
            chargerHighWarn.Add(2022, "交流接触器异常");
            chargerHighWarn.Add(2023, "充电枪头过温");
            chargerHighWarn.Add(2024, "直流输出过流");
            chargerHighWarn.Add(2025, "充电枪锁异常");
            chargerHighWarn.Add(2026, "快充段, 此枪无效");
            chargerHighWarn.Add(2027, "快充段，此枪无效");
            chargerHighWarn.Add(2028, "门禁保护");
            chargerHighWarn.Add(2029, "CAN3通信错误（扩展板）");
            chargerHighWarn.Add(2030, "运行剩余天数无效");
            chargerHighWarn.Add(2031, "控制导引故障");
            chargerHighWarn.Add(2038, "电网电压高，告警不停机");
            chargerHighWarn.Add(2039, "电网电压低，告警不停机");
            chargerHighWarn.Add(2040, "绝缘异常");
            chargerHighWarn.Add(2041, "输出短路");
            chargerHighWarn.Add(2042, "模块过温");
            chargerHighWarn.Add(2043, "模块异常");
            chargerHighWarn.Add(100001, "迪文通信告警");
            chargerHighWarn.Add(100002, "读卡器通信告警");
            chargerHighWarn.Add(100003, "防雷器故障");
            chargerHighWarn.Add(100004, "主开关及熔断器故障");
            chargerHighWarn.Add(100005, "紧急停机故障");
            chargerHighWarn.Add(100032, "电表1通信告警");
            chargerHighWarn.Add(100033, "电表2通信告警");
            chargerHighWarn.Add(100034, "电表3通信告警");
            chargerHighWarn.Add(100035, "电表4通信告警");
            chargerHighWarn.Add(100036, "电表5通信告警");
            chargerHighWarn.Add(100037, "电表6通信告警");
            chargerHighWarn.Add(100038, "电表7通信告警");
            chargerHighWarn.Add(100039, "电表8通信告警");
            chargerHighWarn.Add(100040, "电表9通信告警");
            chargerHighWarn.Add(100041, "电表10通信告警");
            chargerHighWarn.Add(100042, "电表11通信告警");
            chargerHighWarn.Add(100043, "电表12通信告警");
            chargerHighWarn.Add(100044, "电表13通信告警");
            chargerHighWarn.Add(100045, "电表14通信告警");
            chargerHighWarn.Add(100046, "电表15通信告警");
            chargerHighWarn.Add(100047, "电表16通信告警");
            chargerHighWarn.Add(100065, "过压告警");
            chargerHighWarn.Add(100066, "充电欠压告警");
            chargerHighWarn.Add(100067, "过流告警");
            chargerHighWarn.Add(100068, "继电器故障");
            chargerHighWarn.Add(100076, "过温告警");
            chargerHighWarn.Add(100077, "输入欠压告警");
        }
        public static void initChargerEnableFlag()
        {
            DBTrans dBTrans = new DBTrans();
            try
            {
                string sql = "select* from ChargerStatus order by ChargerNo ";
                DataTable dt = dBTrans.GetDataTable(sql);//new DataTable();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Global.ChargerEnableFlag[i + 1] = Int16.Parse(dt.Rows[i]["EnableFlag"].ToString());
                }
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.Message);
                Global.PromptFail("初始化充电机状态失败");
                Global.WriteLog("[Error]Gloabl-initChargerEnableFlag" + ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                dBTrans.Close();
            }

        }
        public static void initPLCWarn()
        {
            plcWarn.Add(300, "换电模式未收到，无法启动运行");
            plcWarn.Add(301, "单动换电指令未收到，无法启动运行");
            plcWarn.Add(302, "取仓位信息未收到，无法启动运行");
            plcWarn.Add(303, "放仓位信息未收到，无法启动运行");
            plcWarn.Add(304, "激光雷达值未收到，无法启动运行");
            plcWarn.Add(305, "中转仓位有电池，无法启动运行");
            plcWarn.Add(306, "机器人处于报警状态，无法启动运行");
            plcWarn.Add(307, "机器人处于运行状态，无法启动运行");
            plcWarn.Add(308, "机器人Y轴未在待机位置，无法启动运行");
            plcWarn.Add(309, "机器人Z轴未在待机位置，无法启动运行");
            plcWarn.Add(310, "机器人有电池，无法执行取电操作");
            plcWarn.Add(311, "吊具定位销未伸出，无法执行取电操作");
            plcWarn.Add(312, "吊具锁舌未缩回，无法执行取电操作");
            plcWarn.Add(313, "取仓位无电池，无法执行取电操作");
            plcWarn.Add(314, "单动模式取车端仓位未解锁，无法执行取电操作");
            plcWarn.Add(315, "取仓位电池未断电，无法执行取电操作");
            plcWarn.Add(316, "机器人无电池，无法执行放电操作");
            plcWarn.Add(317, "吊具定位销未伸出，无法执行放电操作");
            plcWarn.Add(318, "吊具锁舌未伸出，无法执行放电操作");
            plcWarn.Add(319, "放仓位有电池，无法执行放电操作");
            plcWarn.Add(320, "单动模式放车端仓位未解锁，无法执行放电操作");
            plcWarn.Add(321, "放仓位电池未断电，无法执行放电操作");
            plcWarn.Add(322, "车辆未熄火，无法执行车端取放动作");
            plcWarn.Add(323, "车辆未空挡，无法执行车端取放动作");
            plcWarn.Add(324, "车辆未拉手刹，无法执行车端取放动作");
            plcWarn.Add(325, "回原点操作Z轴坐标不在安全范围内");
            plcWarn.Add(400, "换电门未打卡，Y轴不能伸出");
            plcWarn.Add(401, "消防门未打卡，Y轴不能伸出");
            plcWarn.Add(402, "Y轴伸出换电门，换电门不动作");
            plcWarn.Add(403, "Y轴伸出消防门，消防门不动作");
            plcWarn.Add(404, "吊具有电池时，锁舌不动作");
            plcWarn.Add(405, "Y轴伸出车端侧，X轴移动超限");
            plcWarn.Add(406, "Y轴伸出车端侧，R轴移动超限");
            plcWarn.Add(407, "Y轴伸出车端侧，Z轴移动超限");
            plcWarn.Add(408, "Y轴伸出消防侧，R轴移动超限");
            plcWarn.Add(409, "Y轴伸出消防侧，Z轴移动超限");
            plcWarn.Add(410, "Z轴未在安全高度，Y轴不能伸出充电仓侧");
            plcWarn.Add(411, "Y轴伸出充电仓且Z轴未在安全高度，X轴不能移动");
            plcWarn.Add(412, "Y轴伸出充电仓且吊具有电池，X轴不能移动");
            plcWarn.Add(413, "吊具有电池且Y轴未在原位，X轴不能移动");
            plcWarn.Add(414, "X轴未在安全范围内，R轴不能旋转");
            plcWarn.Add(415, "Y轴未在原位，R轴不能旋转");
        }
        public static bool startScan = false; //定义变量
        public static bool StartScan
        {
            get
            {
                return startScan;
            }
            set
            {
                startScan = value;
            }
        }

        private static int lakiConnStaus = 0; //0 未连接  1：已连接 -1：连接故障
        public static int LakiConnStauts
        {
            get
            {
                return lakiConnStaus;
            }
            set
            {
                lakiConnStaus = value;
            }
        }

        public static int scanStatus = 0; //0 没开始扫  1 扫描中 2 扫描完成 
        public static int ScanStatus
        {
            get
            {
                return scanStatus;
            }
            set
            {
                scanStatus = value;
            }
        }

        //PLC
        private static Guid clientLastGuid; //定义变量
        public static Guid ClientLastGuid
        {
            get
            {
                return clientLastGuid;
            }
            set
            {
                clientLastGuid = value;
            }
        }
        //Charger
        private static Guid ChargerclientLastGuid; //定义变量
        public static Guid ChargerClientLastGuid
        {
            get
            {
                return ChargerclientLastGuid;
            }
            set
            {
                ChargerclientLastGuid = value;
            }
        }

        //Fire
        public static FireServer FireServer = null; //singlton
        private static Guid FireclientLastGuid; //定义变量
        public static bool FireConnected = false;
        public static Guid FireClientLastGuid
        {
            get
            {
                return FireclientLastGuid;
            }
            set
            {
                FireclientLastGuid = value;
            }
        }
        //VEH
        public static VehServer VehServer = null; //singlton
        public static MySqlCommand SqlVehCmd3 = new MySqlCommand();
        public static MySqlCommand SqlVehCmd6 = new MySqlCommand();
        public static MySqlCommand SqlVehCmd8 = new MySqlCommand();
        private static Guid VEHclientLastGuid; //定义变量
        public static Guid VEHClientLastGuid 
        {
            get
            {
                return VEHclientLastGuid;
            }
            set
            {
                VEHclientLastGuid = value;
            }
        }
        public static Int32 VEHHeartNum = 0;
        public static bool VEHConnected = false;
        public static Int16 VEHLockAppFlag = 0; //车端加锁申请 ：0 未请求 1 已请求 2收到回执
        public static DateTime VEHLockAppTime; //车端加锁申请时间
        public static Int16 VEHLockReAppNum = 0; //未收到回执时的补发次数
        public static Int16 VEHUnLockAppFlag = 0; //车端解锁申请 ：0 未请求 1 已请求 2收到回执
        public static DateTime VEHUnLockAppTime; //车端解锁申请时间
        public static Int16 VEHUnLockReAppNum = 0; //未收到回执时的补发次数
        public static bool VEHLockError = false; //f 无异常回执  T 异常

        public static string plateNO = ""; //车牌 京A123456
        public static string PlateNO
        {
            get
            {
                return plateNO;
            }
            set
            {
                if (plateNO != value) //车牌发生变化
                {
                    Global.VehInNO = "1001" + DateTime.Now.ToString("yyyyMMddhhmmss"); //pending 多通道
                    AllowSwitch = false; //重新初始化为新的车司机默认未点允许换电
                }
                plateNO = value;
            }
        }

        public static int plateNOTimerInNum = -1; //车牌进入校验Timer次数
        public static int PlateNOTimerInNum
        {
            get
            {
                return plateNOTimerInNum;
            }
            set
            {
                plateNOTimerInNum = value;
            }
        }

        private static string vin = ""; //VIN
        public static string VIN
        {
            get
            {
                return vin;
            }
            set
            {
                vin = value;
            }
        }

        //底部状态栏  0 未连接  1：已连接 -1：连接故障
        private static int cloudConnStatus = 0;
        public static int CloudConnStatus  //远程网络连接
        {
            get
            {
                return cloudConnStatus;
            }
            set
            {
                if (cloudConnStatus != value) //状态发生变化
                {
                    cloudConnStatus = value;
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            if (value == -1)
                            {
                                mainWindow.imgCloudStausLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                                mainWindow.imgCloudStaus.ImageSource = new BitmapImage(new Uri("Resources/色相饱和度1345.png", UriKind.Relative));
                            }
                            else
                            {
                                mainWindow.imgCloudStausLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                                mainWindow.imgCloudStaus.ImageSource = new BitmapImage(new Uri("Resources/按钮背板.png", UriKind.Relative));
                            }
                        }
                    });
                }
            }
        }

        private static int dBConnStatus = 0;
        public static int DBConnStatus  //数据库
        {
            get
            {
                return dBConnStatus;
            }
            set
            {
                if (dBConnStatus != value) //状态发生变化
                {
                    dBConnStatus = value;
                    showDBConnStatus(dBConnStatus);
                }
            }
        }
        //首页底部状态按钮显示数据库状态
        public static void showDBConnStatus(int dBConnStatus)
        {
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

                if (mainWindow != null)
                {
                    if (dBConnStatus == -1)
                    {
                        mainWindow.imgDBStausLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                        mainWindow.imgDBStaus.ImageSource = new BitmapImage(new Uri("Resources/色相饱和度1345.png", UriKind.Relative));
                    }
                    else
                    {
                        mainWindow.imgDBStausLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                        mainWindow.imgDBStaus.ImageSource = new BitmapImage(new Uri("Resources/按钮背板.png", UriKind.Relative));
                    }
                }
            });
        }

        private static int deviceConnStatus = 0;
        public static int DeviceConnStatus  //设备连接
        {
            get
            {
                return deviceConnStatus;
            }
            set
            {
                if (deviceConnStatus != value) //状态发生变化
                {
                    deviceConnStatus = value;
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            if (value == -1)
                            {
                                mainWindow.imgDeviceStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                                mainWindow.imgDeviceStatus.ImageSource = new BitmapImage(new Uri("Resources/色相饱和度1345.png", UriKind.Relative));
                            }
                            else
                            {
                                mainWindow.imgDeviceStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                                mainWindow.imgDeviceStatus.ImageSource = new BitmapImage(new Uri("Resources/按钮背板.png", UriKind.Relative));
                            }
                        }
                    });
                }
            }
        }

        private static int lakiConnStatus = 0;
        public static int LakiConnStatus  //雷达连接连接
        {
            get { return lakiConnStatus; }
            set
            {
                if (lakiConnStatus != value) //状态发生变化
                {
                    lakiConnStatus = value;
                    if (lakiConnStatus == -1)
                    { DeviceConnStatus = -1; }
                    else
                    { SetDeviceConnStatus1(); }
                }
            }
        }
        private static int ledConnStatus = 1;
        public static int LedConnStatus  //显示屏连接,默认连接，发送内容失败再0
        {
            get { return ledConnStatus; }
            set
            {
                if (ledConnStatus != value) //状态发生变化
                {
                    ledConnStatus = value;
                    if (ledConnStatus == -1)
                    { DeviceConnStatus = -1; }
                    else
                    { SetDeviceConnStatus1(); }
                }
            }
        }
        private static int kht1ConnStatus = 0;
        public static int Kht1ConnStatus  //车牌1连接
        {
            get { return kht1ConnStatus; }
            set
            {
                if (kht1ConnStatus != value) //状态发生变化
                {
                    kht1ConnStatus = value;
                    if (kht1ConnStatus == -1)
                    { DeviceConnStatus = -1; }
                    else
                    { SetDeviceConnStatus1(); }
                }
            }
        }

        private static int plcConnStatus = 0;
        public static int PlcConnStatus  //连接
        {
            get { return plcConnStatus; }
            set
            {
                if (plcConnStatus != value) //状态发生变化
                {
                    plcConnStatus = value;
                    if (plcConnStatus == -1)
                    { DeviceConnStatus = -1; }
                    else
                    { SetDeviceConnStatus1(); }
                }
            }
        }

        public static ChargersConnArray chargerConnStatus = new ChargersConnArray(8);


        //恢复设备连接状态为正常 连接1 
        public static void SetDeviceConnStatus1()
        {
            for (int i = 1; i < 8; i++)
            {
                if (chargerConnStatus[i] == -1) return;
            }

            if (LakiConnStatus == 1 && PlcConnStatus == 1 && LedConnStatus == 1 && Kht1ConnStatus == 1)
            {
                DeviceConnStatus = 1;
            }

        }

        //1：正常，0：异常 一般直接事件触发调用状态赋值就行，本方法适合app刚启动时的自检
        public static void ShowDeviceConnStatus(int stauts)
        {
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    ImageBrush backBrush = new ImageBrush();
                    if (stauts == -1)
                    {
                        mainWindow.imgDeviceStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                        mainWindow.imgDeviceStatus.ImageSource = new BitmapImage(new Uri("Resources/色相饱和度1345.png", UriKind.Relative));
                    }
                    else
                    {
                        mainWindow.imgDeviceStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                        mainWindow.imgDeviceStatus.ImageSource = new BitmapImage(new Uri("Resources/按钮背板.png", UriKind.Relative));
                    }


                }
            });
        }

        //当前故障状态灯显示 -1 故障  0 无故障
        public static void ShowFaultStatus(int stauts)
        {
            //从非UI线程调用UI元素
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    ImageBrush backBrush = new ImageBrush();
                    if (stauts == -1)
                    {
                        mainWindow.imgFaultStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                        mainWindow.imgFaultStatus.ImageSource = new BitmapImage(new Uri("Resources/色相饱和度1345.png", UriKind.Relative));
                    }
                    else
                    {
                        mainWindow.imgFaultStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                        mainWindow.imgFaultStatus.ImageSource = new BitmapImage(new Uri("Resources/按钮背板.png", UriKind.Relative));
                    }


                }
            });
        }

        private static int faultStaus = 0;
        public static int FaultStaus  //当前故障
        {
            get
            {
                return faultStaus;
            }
            set
            {
                if (faultStaus != value) //状态发生变化
                {
                    faultStaus = value;
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            if (value == -1)
                            {
                                mainWindow.imgFaultStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/异常.png", UriKind.Relative));
                                mainWindow.imgFaultStatus.ImageSource = new BitmapImage(new Uri("Resources/色相饱和度1345.png", UriKind.Relative));
                            }
                            else
                            {
                                mainWindow.imgFaultStatusLight.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("Resources/正常.png", UriKind.Relative));
                                mainWindow.imgFaultStatus.ImageSource = new BitmapImage(new Uri("Resources/按钮背板.png", UriKind.Relative));
                            }
                        }
                    });
                }
            }
        }



        //设置底部状态栏 
        /*
         底部状态栏  0 未连接  1：已连接 -1：连接故障
        public static int CloudStaus = 0;  //远程网络连接
        public static int DBStaus = 0;  //数据库
        public static int DeviceStaus = 0;  //设备连接
        public static int FaultStaus = 0;  //当前故障
        */
        private static void SetStatus()
        {

        }


        //plc request

        //public static int radarLocationRequest = 0;
        // public static int RadarLocationRequest
        // {
        //     get
        //     {
        //         return radarLocationRequest;
        //     }
        //     set
        //     {
        //         radarLocationRequest = value;
        //     }
        // }

        //send to PLC
        //public static int radarLocationAnswer = 0;
        //public static int RadarLocationAnswer
        //{
        //    get
        //    {
        //        return radarLocationAnswer;
        //    }
        //    set
        //    {
        //        radarLocationAnswer = value;
        //    }
        //}

        //public static int radarSendSignal = 0;
        //public static int RadarSendSignal
        //{
        //    get
        //    {
        //        return radarSendSignal;
        //    }
        //    set
        //    {
        //        radarSendSignal = value;
        //    }
        //}
        //public static float radarX = 999;  
        //public static float RadarX
        //{
        //    get
        //    {
        //        return radarX;
        //    }
        //    set
        //    {
        //        radarX = value;
        //    }
        //}
        //public static float radarY = 999;
        //public static float RadarY
        //{
        //    get
        //    {
        //        return radarY;
        //    }
        //    set
        //    {
        //        radarY = value;
        //    }
        //}
        //public static float radarR = 999;
        //public static float RadarR
        //{
        //    get
        //    {
        //        return radarR;
        //    }
        //    set
        //    {
        //        radarR = value;
        //    }
        //}

        //操作提示区+34级故障提示
        public struct AlertInfo
        {
            public string msg;
            //public int msgType;// 1 submit操作提示(不存历史了) 2 error34级故障提示
            public int showTime; //3S 
            public int showNum; //已展示次数，本级别同一故障持续时仅作2轮提示
            public DateTime CreateTime; //加入list的时间
            public DateTime StartShowTime; //开始提示时间 过showTime秒清除
        }

        //操作提示信息，分成功失败
        public static string OperInfoMsg;           //操作提示信息
        public static int OperInfoType;          // 1 成功  2 失败
        public static DateTime OperInfoStartShowTime; //开始提示时间 过showTime秒清除
                                                      //操作提示区1：操作提示 成功或失败
                                                      //成功提示
        public static void PromptSucc(string msg)
        {
            //操作提示
            OperInfoMsg = msg;
            OperInfoStartShowTime = DateTime.Now;
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    //显示最新的提示信息
                    TextBlock txtInfo = mainWindow.FindName("txtInfo") as TextBlock;
                    Border BorderInfo = mainWindow.FindName("BorderInfo") as Border;
                    txtInfo.Text = msg;
                    BorderInfo.Height = 89;

                    //操作成功，蓝色背景
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/Success.png", UriKind.Relative));
                    BorderInfo.Background = imageCurrentBrush;
                }
            });

        }
        //失败提示 
        public static void PromptFail(string msg)
        {
            //操作提示
            OperInfoMsg = msg;
            OperInfoStartShowTime = DateTime.Now;
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    //显示最新的提示信息
                   
                    TextBlock txtInfo = mainWindow.FindName("txtInfo") as TextBlock;
                    Border BorderInfo = mainWindow.FindName("BorderInfo") as Border;
                    txtInfo.Text = msg;
                    //BorderInfo.Visibility = Visibility.Visible ;
                    BorderInfo.Height = 89;

                    //操作失败，红色背景
                    ImageBrush imageCurrentBrush = new ImageBrush();
                    imageCurrentBrush.ImageSource = new BitmapImage(new Uri(@"Resources/告警弹窗.png", UriKind.Relative));
                    BorderInfo.Background = imageCurrentBrush;

                }
            });

        }


        public static List<AlertInfo> Alertlist = new List<AlertInfo>();
        ////操作提示区+34级故障提示List，等待轮询或立刻提示
        public static void Alert(string msg)
        {
            //Alertlist.Find()
            // Entry item = Alertlist.Find(x => x.msg ==msg);

            AlertInfo alertInfo = new AlertInfo();
            alertInfo.CreateTime = DateTime.Now;
            alertInfo.showTime = 3;
            alertInfo.msg = msg;

            //从无到有
            //if (Warnlist.Count == 0 && Alertlist.Count == 0)
            //{
            ShowFaultStatus(-1);//有故障
            //}

            if (!Alertlist.Contains(alertInfo))
            {
                //当前无34故障直接显示
                if (Alertlist.Count == 0)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            //显示最新的3-4告警
                            TextBlock txtAlert = mainWindow.FindName("txtAlert") as TextBlock;
                            Border BorderAlert = mainWindow.FindName("BorderAlert") as Border;
                            txtAlert.Text = msg;
                            BorderAlert.Height = 89;
                            alertInfo.StartShowTime = DateTime.Now;
                        }
                    });
                }
                Alertlist.Add(alertInfo);

            }

        }

     

        //轮播提示区2：34级告警故障
        public static void RemoveAlert(string msg)
        {
            //Alertlist.Find()
            // Entry item = Alertlist.Find(x => x.msg ==msg);

            AlertInfo alertInfo = new AlertInfo();
            alertInfo.showTime = 2;
            alertInfo.msg = msg;

            //if (!Alertlist.Contains(alertInfo))
            //{
            Alertlist.Remove(alertInfo);
            //}

            if (Warnlist.Count == 0 && Alertlist.Count == 0)
            {
                ShowFaultStatus(0);
            }

        }


        //弹窗12级告警
        public struct WarnInfo
        {
            public string msg;
            public int msgType;// 1 PLC 2 充电机 3电池 4消防
            public DateTime OKTime; //点确认时间 间隔10S再次弹出（10S清除）
            public int showNum; //已展示次数，暂未使用，同一故障一共弹窗不超过x次；
        }
        //1-2级别弹窗告警List
        public static List<WarnInfo> Warnlist = new List<WarnInfo>();

        //1-2级别弹窗告警
        //msgType;// 1 PLC 2 充电机 3电池 4消防
        public static void Warn(string msg, int msgType = 1)
        {
            WarnInfo warnInfo = new WarnInfo();
            warnInfo.msgType = msgType;
            warnInfo.msg = msg;

            if (Warnlist.Count == 0) //从无警告到有警告0-1转换故障状态为有故障
            {
                ShowFaultStatus(-1);
            }


            if (!Warnlist.Contains(warnInfo))
            {
                //不在Warnlist中的，加入并弹窗
                Warnlist.Add(warnInfo);
                App.Current.Dispatcher.Invoke((Action)delegate ()
                {
                    DialogWarn dialog = new DialogWarn(msg);
                    dialog.ShowDialog();
                });


                //点ok的时候不清除，让10S内进来的同一warn不再弹出；清除所有超10S的历史Warn
                Warnlist.RemoveAll(item => DiffSeconds(item.OKTime, DateTime.Now) >= 10);

            }
            else
            {
                //在Warnlist中的，10S内关闭的不处理，超过10S的还没清除的直接更新okTime重计10S
                WarnInfo warnInfoCache = Warnlist.FirstOrDefault(p => p.msg == msg);
                DateTime okTime = warnInfoCache.OKTime;
                if (okTime != default(DateTime) && DiffSeconds(okTime, DateTime.Now) >= 10)
                {
                    //Warnlist.Add(warnInfo);
                    warnInfoCache.OKTime = default(DateTime);
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        DialogWarn dialog = new DialogWarn(msg);
                        dialog.ShowDialog();
                    });

                }
                //10S内的什么都不做。
            }

        }

        public static void RemoveWarn(string msg, int msgType)
        {
            //Warnlist.Find()
            // Entry item = Warnlist.Find(x => x.msg ==msg);

            WarnInfo warnInfo = new WarnInfo();
            warnInfo.msgType = msgType;
            warnInfo.msg = msg;

            //if (!Warnlist.Contains(warnInfo))
            //{
            Warnlist.Remove(warnInfo);
            //}

            if (Warnlist.Count == 0 && Alertlist.Count == 0)
            {
                ShowFaultStatus(0);
            }

        }

        public static void AddEvent(string Content, int eventType, string account)
        {
            if (Content == "") return;
            string Sql = "INSERT INTO event( eventType, account, content) ";
            Sql += " VALUES(" + eventType + ",'" + account + "','" + Content.Replace("'", "''") + "')";
            Global.dbTrans.ProcessNoqureyAsync(Sql);

        }
        public static void AddEvent(string Content, int eventType)
        {
            AddEvent(Content, eventType, Global.Account);
        }

        public static async Task WriteDebugAsync(string msg, string logType = "")
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + logType + "debug" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                try
                {
                    using (StreamWriter sw = File.AppendText(logPath))
                    {
                        sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "消息：" + msg);
                        sw.WriteLineAsync("**************************************************");
                        sw.WriteLineAsync();
                        sw.FlushAsync();
                        sw.Close();
                        sw.DisposeAsync();
                    }
                }
                catch (IOException e)
                {
                    using (StreamWriter sw = File.AppendText(logPath))
                    {
                        sw.WriteLineAsync("异常：" + e.Message);
                        sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                        sw.WriteLineAsync("**************************************************");
                        sw.WriteLineAsync();
                        sw.FlushAsync();
                        sw.Close();
                        sw.DisposeAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public static async Task WriteLogAsync(string msg, string logType = "")
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + logType + "APP" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "消息：" + msg);
                    sw.WriteLineAsync("**************************************************");
                    sw.WriteLineAsync();
                    sw.FlushAsync();
                    sw.Close();
                    sw.DisposeAsync();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLineAsync("异常：" + e.Message);
                    sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    sw.WriteLineAsync("**************************************************");
                    sw.WriteLineAsync();
                    sw.FlushAsync();
                    sw.Close();
                    sw.DisposeAsync();
                }
            }
        }

        //显示换电当前进行的动作或步骤
        public static void ShowSwitchSteps(string msg)
        {

            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow == null) return;
                try
                {
                    TextBlock txtSwitchSteps = mainWindow.FindName("txtSwitchSteps") as TextBlock;
                    if (txtSwitchSteps != null) txtSwitchSteps.Text = msg;
                }
                catch (Exception ex)
                {
                    WriteLog("[Error-ShowSwitchSteps]" + msg + ex.Message + "\r\n" + ex.StackTrace);
                }

            });
        }


        //判断车站是否连接成功 1 连 0 没连  -1 err
        public static string vehConnected()
        {
            if (Global.VEHConnected)
                return "1";
            else
                return "0";
        }
        //判断是否有可换电池 SOC 1 连 0 没连  -1 err  pending 0313
        public static string batterySOCMatch()
        {
            return "1";
        }
        //电池型号匹配 1 连 0 没连  -1 err pending
        public static string batteryModelMatch()
        {
            return "1";
        }
        /*
        0：Free；
1：Ready；
2：BUSY；
3：Pause；
4：Alarm；
        */
        public static string transRobotStatus(int robotStatus)
        {
            switch (robotStatus)
            {
                case 0:
                    return "Free";
                case 1:
                    return "Ready";
                case 2:
                    return "BUSY";
                case 3:
                    return "Pause";
                case 4:
                    return "Alarm";
                default:
                    return "NA";
            }
        }
        // uniqId = detail.uniqId;      //铭牌编码  站端需维护映射表 -1: 没找到
        public static int transChargeID(string uniqId)
        {
            for (int i = 1; i < Global.config.ChargerCabinetConfig.Count; i++)
            {
                if (Global.config.ChargerCabinetConfig[i - 1].UniqId.Trim().ToLower() == uniqId.Trim().ToLower())
                {
                    return Global.config.ChargerCabinetConfig[i - 1].ID;
                }
            }

            return -1;
        }

        public static string transuniqId(int ChargeID)
        {
            for (int i = 1; i < Global.config.ChargerCabinetConfig.Count; i++)
            {
                if (Global.config.ChargerCabinetConfig[i - 1].ID == ChargeID)
                {
                    return Global.config.ChargerCabinetConfig[i - 1].UniqId;
                }
            }

            return "";
        }

        /*有效值 0~15，范围 (0~15) ，偏移量 0，比例因子 1/bit1“铅酸电池"2“镍氢电池"3“磷酸铁鲤电池"4“酸理电池"5“钻酸鲤电池"6"三元材料电池"7“聚合物鲤离子电池"
         * 8“钦酸鲤电池"9“超级电容"10"Reserved"11Reserved" 12Reserved"13"Reserved" 14“燃料电池5"Reserved */
        public static string transBatteryType(string BatteryType)
        {
            switch (BatteryType)
            {
                case "1":
                    return "铅酸电池";
                case "2":
                    return "镍氢电池";
                case "3":
                    return "磷酸铁锂电池";
                case "4":
                    return "锰酸锂电池";
                case "5":
                    return "钻酸锂电池";
                case "6":
                    return "三元材料电池";
                case "7":
                    return "聚合物锂离子电池";
                case "8":
                    return "钛酸鲤电池";
                case "9":
                    return "超级电容";
                case "14":
                    return "燃料电池";
                default:
                    return "NA";
            }
        }

        public static string transBatteryCoolMethod(string BatteryCoolMethod)
        {
            switch (BatteryCoolMethod)
            {
                case "1":
                    return "自然冷却";
                case "2":
                    return "风冷";
                case "3":
                    return "水冷";
                default:
                    return "NA";
            }
        }

        public static string transGunWorkStatus(string WorkStatus)
        {
            switch (WorkStatus)
            {
                case "0":
                    return "空闲中";
                case "1":
                    return "正准备开始充电";
                case "2":
                    return "充电进行中";
                case "3":
                    return "充电结束";
                case "4":
                    return "启动失败";
                case "5":
                    return "预约状态";
                case "6":
                    return "系统故障(不能给汽车充电)";
                case "7":
                    return "暂停服务";
                default:
                    return "NA";
            }
        }

        //取第startBytes字节的第startbyte位（二进制位）
        public static byte GetBytes(byte[] bytesArr, int startBytes, int startbyte)
        {
            byte bytes = bytesArr[startBytes];
            byte[] byteArray = new byte[] { bytes };
            string str = string.Join("", Array.ConvertAll(byteArray, b => Convert.ToString(b, 2).PadLeft(8, '0')));

            //byte data =(byte) str[startbyte]; 
            byte data = (byte)Convert.ToByte(str.Substring(startbyte, 1), 2);
            return data;
        }

        //0 关机模式，1 制冷模式，2 制热模式，3 自循环模
        public static string transTMSWorkStatus(string TMSWorkStatus)
        {
            switch (TMSWorkStatus)
            {
                case "1":
                    return "制冷模式";
                case "2":
                    return "制热模式";
                case "3":
                    return "自循环模";
                default:
                    return "NA";
            }
        }

        //获取CMD116报文中的BatterySN，10S内的
        public static string get116BatterySN(int chargerID)
        {
            string batterySN = "";
            string strCreateTime = Global.SqlCmd116[chargerID].Parameters["@CreateTime"].Value.ToString();
            if (DateTime.TryParse(strCreateTime, out DateTime CreateTime))
            {
                if (Global.DiffSeconds(CreateTime, DateTime.Now) <= 10)
                {
                    batterySN = Global.SqlCmd116[chargerID].Parameters["@BattertySN"].Value.ToString(); //10s限定
                }
            }
            return batterySN;
        }

        //获取时间范围内的值，不在范围返回空字符串
        //Invalue 如果时间在范围内的值，strCreateTime 数据产生时间， seconds 有效时间
        public static string getValueInTime(string Invalue, string strCreateTime, int seconds)
        {
            string batterySN = "";
            //string strCreateTime = Global.SqlCmd116[chargerID].Parameters["@CreateTime"].Value.ToString();
            if (DateTime.TryParse(strCreateTime, out DateTime CreateTime))
            {
                if (Global.DiffSeconds(CreateTime, DateTime.Now) <= 10)
                {
                    batterySN = Invalue; //10s限定
                }
            }
            return batterySN;
        }

        public static void Dialog(string title, string info, string buttons)
        {
            Dialog dialog = new Dialog(title, info, buttons);
            dialog.ShowDialog();
        }

        public static void SetShowScreen(Window window)
        {
            try
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
                        window.Top = r2.Top - 130;
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
            }
            catch (Exception ex)
            {

            }
        }

        public static byte[] DecimalToByteArray(decimal src)
        {
            //创建内存流MemoryStream，stream作为存放 二进制数据 的缓存
            using (MemoryStream stream = new MemoryStream())
            {
                //创建一个BinaryWriter来写二进制数据到stream
                using (BinaryWriter write = new BinaryWriter(stream))
                {
                    write.Write(src);//将十进制数字src写到stream中，
                    return stream.ToArray();//将写到stream中的二进制数据转为字节数组

                }
            }
        }

        public static byte[] ToBCD16(DateTime d)
        {
            DateTime now = d;
            byte[] bcdBytes = new byte[8]; // 创建存放 BCD 字节数组的变量

            int y1 = Int16.Parse(now.Year.ToString().Substring(0, 2));
            int y2 = Int16.Parse(now.Year.ToString().Substring(2, 2));
            bcdBytes[0] = (byte)(y1);
            bcdBytes[1] = (byte)(y2);
            int month = now.Month;
            bcdBytes[2] = (byte)(month);
            int day = (int)now.Day;
            bcdBytes[3] = (byte)(day);
            int hour = now.Hour;
            bcdBytes[4] = (byte)(hour);
            int minute = now.Minute;
            bcdBytes[5] = (byte)(minute);
            int second = now.Second;
            bcdBytes[6] = (byte)(second);
            bcdBytes[7] = (byte)(0x00);

            return bcdBytes;
        }

        public static byte[] ToBCD(DateTime d)
        {
            List<byte> bytes = new List<byte>();
            string s = d.ToString("yyyyMMddHHmmss");
            for (int i = 0; i < s.Length; i += 2)
            {
                bytes.Add((byte)((s[i] - '0') << 4 | (s[i + 1] - '0')));
            }
            bytes.Add(0xFF);
            return bytes.ToArray();
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            //hexString = hexString.Replace(" ", "");
            //if ((hexString.Length % 2) != 0)
            //    hexString += " ";
            //byte[] returnBytes = new byte[hexString.Length / 2];
            //for (int i = 0; i < returnBytes.Length; i++)
            //    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            //return returnBytes;

            //string hexString = "48656C6C6F20576F726C64"; // 要转换的十六进制字符串
            byte[] byteArray = Enumerable.Range(0, hexString.Length / 2)
                                         .Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16))
                                         .ToArray();
            return byteArray;

        }


        //相差秒
        public static double DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return secondSpan.TotalSeconds;
        }

        //小时:分钟格式"08:30" 转 TimeSpan
        public static TimeSpan GetTimeSpanFromString(string time)
        {
            string[] parts = time.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            return new TimeSpan(0, hours, minutes, 0);
        }


        public static void CopyAsCsvHandler(DataGrid dataGrid)
        {
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "CSV Files (*.csv)|*.csv|Excel XML (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() == DialogResult.OK)
            {
                string strFormat = objSFD.FileName;
                dataGrid.SelectAllCells();
                dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, dataGrid);
                dataGrid.UnselectAllCells();
                string result = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);
                File.AppendAllText(strFormat, result, UnicodeEncoding.UTF8);
                System.Windows.Clipboard.Clear();//清空粘贴板               
                //System.Windows.MessageBox.Show("导出成功");
                Global.PromptSucc("导出成功");
            }

        }


        // 辅助方法，用于在VisualTree中查找指定类型的父级
        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // 获取父级
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // 如果父级是要查找的类型，则直接返回
            if (parentObject != null && parentObject is T)
                return (T)parentObject;

            // 否则递归查找父级的父级
            else
                return FindVisualParent<T>(parentObject);
        }

        //按坐标系，车头左偏大于0，机器人相反；
        public static double Adj_a(double a)
        {
            double rtn = 0;
            if (a > 0)
            {
                rtn = a - 180;
            }
            else
            {
                rtn = 180 + a;
            }
            return rtn;
        }

        public static async Task WriteLog(string msg)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\APP" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "消息：" + msg);
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        //暂未使用区分，全放在log
        public static void WriteError(string msg)
        {


            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\Error" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "消息：" + msg);
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
#if DEBUG
                throw (e);
#endif
            }
        }
        public static void WriteSingleLog(string msg)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\Temp" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(msg);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }


        public static byte[] Fill0(byte[] inputbytes, int ttlLength)
        {
            //byte[] inputbytes = Encoding.GetEncoding("us-ascii").GetBytes(chargerID.ToString() + cMD7.GunNo.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss")); // 输入需要补全的字符串
            int paddingLength = ttlLength - inputbytes.Length; // 计算需要添加的字节数量
            byte[] paddedBytes = new byte[paddingLength]; // 创建空白字节数组来存放补全后的内容
            Array.Fill<byte>(paddedBytes, (byte)0x00); // 使用0x00（ASCII值为0）对字节数组进行填充
            //byte[] resultBytes = Encoding.UTF8.GetBytes(input + new string('\0', paddingLength)); // 获取原始字符串并与补全后的字节数组合并成最终结果
            //string output = Encoding.UTF8.GetString(resultBytes); // 转换为字符串形式返回
            //Console.WriteLine("补全后的字符串：" + output);
            //cMD7.ChargeSN_1 = Encoding.GetEncoding("us-ascii").GetBytes(chargerID.ToString() + cMD7.GunNo.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss")); //充电流水号	32;  充电机号+枪号+时间戳（YYYYMMDDHHMMSS）
            return inputbytes.Concat(paddedBytes).ToArray();
        }

        //定时补发Http、MQ报文
        public static void SendMsgWait()
        {
            if (Global.CloudConnStatus != 1) return;
            try
            {
                string sql = "select * from msgwait where sendflag=0  order by CREATEtime desc,protocol ";
                DataTable dt = Global.dbTrans.GetDataTable(sql);
                string protocol = "";
                string MsgType = "", msg = "", Error = "", msgID;
                int k = dt.Rows.Count;
                //if (dt.Rows.Count > 500) k = 500; //每500条补发一次,暂不启用
                for (int i = 0; i < k; i++)
                {
                    msgID = dt.Rows[i]["ID"].ToString();
                    protocol = dt.Rows[i]["protocol"].ToString();
                    MsgType = dt.Rows[i]["MsgType"].ToString();
                    msg = dt.Rows[i]["msg"].ToString();
                    Error = dt.Rows[i]["Error"].ToString();
                    if (protocol == "1")
                    {
                        //1 http
                        Global.httpClient.postHttpMsgWait(msg, MsgType, msgID);
                    }
                    else
                    {
                        // 2 MQ
                        Global.mgttClientService.PublishMQMsgWait(msg, MsgType, msgID);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLog("[Error SendMsgWait]" + ex.Message + ex.StackTrace);
            }
        }
        public static string NewVer = "";  //平台当前最新版本
        public static string NewVerUrl = "";    //升级包下载地址
        public static string NewVerFileName = "";

        //暂未使用区分，全放在log
        public static void WriteError(string msg, string ErrorType = "")
        {


            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + ErrorType + "Error" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "消息：" + msg);
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
#if DEBUG
                //throw (e);
#endif
            }
        }
    }


    public class ChargersConnArray
    {
        private int[] _array;

        public ChargersConnArray(int size)
        {
            _array = new int[size];
        }

        // Getter方法，允许外部读取数组
        public int[] Array
        {
            get { return _array; }
        }

        // Setter方法，允许外部修改数组元素
        public int this[int index]
        {
            get { return _array[index]; }
            set
            {

                if (_array[index] != value) //状态发生变化
                {
                    _array[index] = value;
                    if (_array[index] == -1)
                    { DeviceConnStatus = -1; }
                    else
                    { SetDeviceConnStatus1(); }
                }
                _array[index] = value;

            }
        }
    }




}



