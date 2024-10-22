using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using static DESCADA.lakiudp;

namespace DESCADA.Service
{
    public static class PLC
    {
        /*
            PLC4 Write 全时发送 1000ms
         [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
         */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public  struct PLCW4
        {
            public ushort pkghead; // 包头   2
            public Byte head; //	数据流标识位
            public Byte locationCheck0; //	中转仓位电池检测
            public Byte locationCheck1; //	充电仓位1电池检测
            public Byte locationCheck2; //	充电仓位2电池检测
            public Byte locationCheck3; //	充电仓位3电池检测
            public Byte locationCheck4; //	充电仓位4电池检测
            public Byte locationCheck5; //	充电仓位5电池检测
            public Byte locationCheck6; //	充电仓位6电池检测
            public Byte locationCheck7; //	充电仓位7电池检测
            public Int16  tempLocation1; //	电池仓温度1
            public Int16 tempLocation2; //	电池仓温度2
            public ushort  FanBatteryCharger1;   //	充电仓进气风扇控制1
            public ushort FanBatteryCharger2;   //	充电仓排气风扇控制2
            public Byte Fan1; //	电池仓排气扇1控制
            public Byte Fan2; //	电池仓排气扇2控制
            public Byte Fan3; //	电池仓排气扇3控制 20231204
            public Byte Fan4; //	电池仓排气扇4控制 20231204
            public ushort RobotLocationCheck;   //	机器人工位电池检测
            public ushort HoistLockStatus;  //	吊具锁舌状态
            public ushort HoistLocationStatus;  //	吊具定位销状态
            public ushort Door1;    //	换电门状态
            public ushort Door2;    //	消防门状态
            public ushort ServiceDoorSignal;    //	维修门信号
            public ushort EBoxDoorSignal;   //	机器人电柜门信号
            public ushort AMC_PT;   //	PT变比
            public ushort AMC_CT;   //	CT变比
            public ushort AMC_UA;   //	相电压UA
            public ushort AMC_UB;   //	相电压UB
            public ushort AMC_UC;   //	相电压UC
            public ushort AMC_IA;   //	电流IA
            public ushort AMC_IB;   //	电流IB
            public ushort AMC_IC;   //	电流IC
            public Int16 ActivePower;   //	总有功功率
            public Int16 ReactivePower; //	总无功功率
            public ushort PowerFactor;  //	总功率因数
            public ushort ApparentPower;    //	总视在功率
            public ushort Frequency;    //	频率
            public float absorbWattHour;    //	吸收有功电能一次侧
            public float releaseWattHour;   //	释放有功电能一次侧
            public float induReactiveEnergy;    //	感性无功电能一次侧
            public float capaReactiveEnergy;	//	容性无功电能一次侧

        }
        //全时发送 100ms
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PLCW5
        {
            public ushort pkghead; // 包头
            public Byte head; //	数据流标识位
            public ushort heart; //	心跳信号 0-65535，溢出之后清零，循环计数
            public ushort RobotStatus;  //	机器人运行状态
            public ushort ControlMode;  //	PLC控制模式
            public ushort RunMode;  //	PLC运行模式
            public ushort ExchangeStatus;   //	机器人换电状态
            public float PositionX; //	机器人X轴位移
            public float PositionY; //	机器人Y轴位移
            public float PositionZ; //	机器人Z轴位移
            public float PositionR; //	机器人R轴位移
            public float SpeedX;    //	机器人X轴速度
            public float SpeedY;    //	机器人Y轴速度
            public float SpeedZ;    //	机器人Z轴速度
            public float SpeedR;    //	机器人R轴速度
            public Byte PumpOpenRec;    //	PLC回复注油响应
            public Byte RadarLocationRequest;   //	激光雷达定位请求
            public Byte RadarRecSignal; //	激光雷达值PLC接受完成
            public Byte ExchangeDone;    //	换电完成信号 20231219
            //public ushort WarningID1;    //	提示代码1 20231204
            //public ushort WarningID2;    //	提示代码1
            public short  WarningID1;    //	提示代码1 20231204
                                         //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = UnmanagedType.Struct)]
                                         // public little_pack_t[] depths;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            //public Byte[] ErrFlag1;

            // public Byte ErrFlag1[16];     //系统报警标志位
            public ushort ErrFlag1;     //系统报警标志位
            public ushort ErrFlag2;     //机器人X轴报警标志位
            public ushort ErrFlag3;     //机器人X轴伺服故障代码
            public ushort ErrFlag4;     //机器人Y轴报警标志位
            public ushort ErrFlag5;     //机器人Y轴伺服故障代码
            public ushort ErrFlag6;     //机器人Z轴报警标志位
            public ushort ErrFlag7;     //机器人Z轴伺服故障代码
            public ushort ErrFlag8;     //机器人R轴报警标志位
            public ushort ErrFlag9;     //机器人R轴伺服故障代码
            public ushort ErrFlag10;        //吊具锁舌报警标志位
            public ushort ErrFlag11;        //吊具定位销报警标志位
            public ushort ErrFlag12;        //吊具到位报警标志位
            public ushort ErrFlag13;        //激光定位报警标志位
            public ushort ErrFlag14;        //引导单元报警标志位
            public ushort ErrFlag15;        //充电仓报警标志位
            public ushort ErrFlag16;        //电动门报警标志位
            public ushort ErrFlag17;        //风机报警标志位
            public ushort ErrFlag18;		//通讯故障标志位
        }
        //cycle(100ms)，收到PC连接换电控制器报文后开始发送，PLC换电完成信号之后停发
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PLCW6
        {
            public ushort pkghead; // 包头
            public Byte head; //	数据流标识位
            public Byte UnlockCmd;  //	解锁请求信号
            public Byte LockCmd;    //	上锁请求信号
        }

        /*
          PLC1 Read cycle(1000ms) 全时发送
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PLCR1
        {
            public ushort pkghead; // 包头
            public Byte head; //	数据流标识位
            public ushort BatteryChargerStatus1;    //	充电仓位1充电机状态
            public ushort BatteryChargerStatus2;    //	充电仓位2充电机状态
            public ushort BatteryChargerStatus3;    //	充电仓位3充电机状态
            public ushort BatteryChargerStatus4;    //	充电仓位4充电机状态
            public ushort BatteryChargerStatus5;    //	充电仓位5充电机状态
            public ushort BatteryChargerStatus6;    //	充电仓位6充电机状态
            public ushort BatteryChargerStatus7;    //	充电仓位7充电机状态
            public Int16 tempCharger1;  //	充电机1排风口温度
            public Int16 tempCharger2;  //	充电机2排风口温度
            public Int16 tempCharger3;  //	充电机3排风口温度
            public Int16 tempCharger4;  //	充电机4排风口温度
            public Int16 tempCharger5;  //	充电机5排风口温度
            public Int16 tempCharger6;  //	充电机6排风口温度
            public Int16 tempCharger7;  //	充电机7排风口温度
            public ushort CarWaitInfo;  //	车辆排队信息
        }
        //cycle(100ms) 全时发送
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PLCR2
        {
            public ushort pkghead; // 包头
            public Byte head; //	数据流标识位
            public ushort heart; //	心跳信号 0-65535，溢出之后清零，循环计数
            public ushort PCToPLCCmd;   //	PC发送PLC运行指令
            public ushort PCAutoPickNum;    //	自动取电池仓位
            public ushort PCAutoPutNum; //	自动放电池仓位
            public ushort PCAutoExchangeCmd;    //	PC自动换电指令
            public ushort FireNum;  //	消防报警仓位
            public ushort PCOutFireCmd; //	PC一键消防指令
            public Byte PCRecExchangeDone;  //	换电完成接受信号
            public ushort PCManuExchangeCmd;    //	PC手动换仓指令 1:取电池;2:放电池;3:复归(仅单动模式下可用)，4:取放电池(20240429加4 )
            //public ushort  PCManuPickNum;   //	PC手动换电仓位信息  20240429改成PCManuPickNum
            public ushort PCManuPickNum;  //PC手动换电取仓位  20240429加
            public ushort PCManuExchangeStartCmd;   //	PC手动换仓指令
            public Byte PCGoHome;   //	PC发送复归指令     20240429暂不使用
            public Byte PCReset;    //	PC发送复位指令
            public Byte PumpOpen;   //	PC发送注油指令
            public Byte RadarLocationAnswer;    //	激光雷达定位响应
            public float RadarX;    //	激光雷达X偏移值
            public float RadarY;    //	激光雷达Y偏移值
            public float RadarR;    //	激光雷达R偏移值
            public Byte RadarSendSignal;    //	激光雷达值发送信号
            public ushort PCManuPutNum;   //PC手动换电放仓位  20240429加
        }
        //cycle(100ms)  PC连接换电控制器(车载)后开始发送，收到PLC换电完成信号之后停发。
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PLCR3
        {
            public ushort pkghead; // 包头
            public Byte head; //	数据流标识位
            public Byte CarExConnector; //	换电控制器连接状态
            public ushort CarHandBrake; //	手刹状态
            public ushort CarKey;   //	钥匙状态
            public ushort CarGears; //	档位状态
            public ushort CarLockBattery;   //	车端底座锁止状态
            public ushort CarChargeConnector;   //	充电连接器连接状态
            public ushort CarDischargeConnector;    //	放电连接器连接状态
            public ushort BatteryModel;	//	电池类型
        }
    }
}
