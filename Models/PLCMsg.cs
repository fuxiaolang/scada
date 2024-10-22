using DESCADA.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DESCADA.Models
{


    public static class PLCM4
    {
        public static ushort pkghead { get; set; } = 9999; //	 包头   2
        public static Byte head { get; set; } = 255; //		数据流标识位
        public static Byte locationCheck0 { get; set; } = 255; //		中转仓位电池检测
        public static Byte locationCheck1 { get; set; } = 255; //		充电仓位1电池检测
        public static Byte locationCheck2 { get; set; } = 255; //		充电仓位2电池检测
        public static Byte locationCheck3 { get; set; } = 255; //		充电仓位3电池检测
        public static Byte locationCheck4 { get; set; } = 255; //		充电仓位4电池检测
        public static Byte locationCheck5 { get; set; } = 255; //		充电仓位5电池检测
        public static Byte locationCheck6 { get; set; } = 255; //		充电仓位6电池检测
        public static Byte locationCheck7 { get; set; } = 255; //		充电仓位7电池检测
        public static Int16 tempLocation1 { get; set; } = 9999; //		电池仓温度1
        public static Int16 tempLocation2 { get; set; } = 9999; //		电池仓温度2
        public static ushort FanBatteryCharger1 { get; set; } = 9999; //			充电仓进气风扇控制1
        public static ushort FanBatteryCharger2 { get; set; } = 9999; //			充电仓排气风扇控制2
        public static ushort Fan1 { get; set; } = 9999; //		电池仓排气扇1控制
        public static ushort Fan2 { get; set; } = 9999; //		电池仓排气扇2控制
        public static ushort Fan3 { get; set; } = 9999; //		电池仓排气扇3控制 20231204
        public static ushort Fan4 { get; set; } = 9999; //		电池仓排气扇4控制 20231204
        public static ushort RobotLocationCheck { get; set; } = 9999; //			机器人工位电池检测
        public static ushort HoistLockStatus { get; set; } = 9999; //			吊具锁舌状态
        public static ushort HoistLocationStatus { get; set; } = 9999; //			吊具定位销状态
        public static ushort Door1 { get; set; } = 9999; //			换电门状态
        public static ushort Door2 { get; set; } = 9999; //			消防门状态
        public static ushort ServiceDoorSignal { get; set; } = 9999; //			维修门信号
        public static ushort EBoxDoorSignal { get; set; } = 9999; //			机器人电柜门信号
        public static ushort AMC_PT { get; set; } = 9999; //			PT变比
        public static ushort AMC_CT { get; set; } = 9999; //			CT变比
        public static ushort AMC_UA { get; set; } = 9999; //			相电压UA
        public static ushort AMC_UB { get; set; } = 9999; //			相电压UB
        public static ushort AMC_UC { get; set; } = 9999; //			相电压UC
        public static ushort AMC_IA { get; set; } = 9999; //			电流IA
        public static ushort AMC_IB { get; set; } = 9999; //			电流IB
        public static ushort AMC_IC { get; set; } = 9999; //			电流IC
        public static Int16 ActivePower { get; set; } = 9999; //			总有功功率
        public static Int16 ReactivePower { get; set; } = 9999; //		总无功功率
        public static ushort PowerFactor { get; set; } = 9999; //			总功率因数
        public static ushort ApparentPower { get; set; } = 9999; //			总视在功率
        public static ushort Frequency { get; set; } = 9999; //			频率
        public static float absorbWattHour { get; set; } = 9999; //			吸收有功电能一次侧
        public static float releaseWattHour { get; set; } = 9999; //			释放有功电能一次侧
        public static float induReactiveEnergy { get; set; } = 9999; //			感性无功电能一次侧
        public static float capaReactiveEnergy { get; set; } = 9999; //			容性无功电能一次侧

    }

    public static class PLCM5
    {
        public static ushort pkghead { get; set; } = 9999; //	 包头
        public static Byte head { get; set; } = 255; //		数据流标识位
        public static ushort heart { get; set; } = 9999; //		心跳信号 0-65535，溢出之后清零，循环计数
        public static ushort RobotStatus { get; set; } = 9999; //			机器人运行状态
        public static ushort ControlMode { get; set; } = 9999; //			PLC控制模式
        public static ushort RunMode { get; set; } = 9999; //			PLC运行模式
        public static ushort ExchangeStatus { get; set; } = 9999; //			机器人换电状态
        public static float PositionX { get; set; } = 9999; //		机器人X轴位移
        public static float PositionY { get; set; } = 9999; //		机器人Y轴位移
        public static float PositionZ { get; set; } = 9999; //		机器人Z轴位移
        public static float PositionR { get; set; } = 9999; //		机器人R轴位移
        public static float SpeedX { get; set; } = 9999; //			机器人X轴速度
        public static float SpeedY { get; set; } = 9999; //			机器人Y轴速度
        public static float SpeedZ { get; set; } = 9999; //			机器人Z轴速度
        public static float SpeedR { get; set; } = 9999; //			机器人R轴速度
        public static Byte PumpOpenRec { get; set; } = 255; //			PLC回复注油响应
        public static Byte RadarLocationRequest { get; set; } = 255; //			激光雷达定位请求
        public static Byte RadarRecSignal { get; set; } = 255; //		激光雷达值PLC接受完成
        public static ushort WarningID1 { get; set; } = 9999; //			提示代码1 20231204
        public static ushort WarningID2 { get; set; } = 9999; //			提示代码1
        public static ushort ErrFlag1 { get; set; } = 9999; //	系统报警标志位
        public static ushort ErrFlag2 { get; set; } = 9999; //	机器人X轴报警标志位
        public static ushort ErrFlag3 { get; set; } = 9999; //	机器人X轴伺服故障代码
        public static ushort ErrFlag4 { get; set; } = 9999; //	机器人Y轴报警标志位
        public static ushort ErrFlag5 { get; set; } = 9999; //	机器人Y轴伺服故障代码
        public static ushort ErrFlag6 { get; set; } = 9999; //	机器人Z轴报警标志位
        public static ushort ErrFlag7 { get; set; } = 9999; //	机器人Z轴伺服故障代码
        public static ushort ErrFlag8 { get; set; } = 9999; //	机器人R轴报警标志位
        public static ushort ErrFlag9 { get; set; } = 9999; //	机器人R轴伺服故障代码
        public static ushort ErrFlag10 { get; set; } = 9999; //	吊具锁舌报警标志位
        public static ushort ErrFlag11 { get; set; } = 9999; //	吊具定位销报警标志位
        public static ushort ErrFlag12 { get; set; } = 9999; //	吊具到位报警标志位
        public static ushort ErrFlag13 { get; set; } = 9999; //	激光定位报警标志位
        public static ushort ErrFlag14 { get; set; } = 9999; //	引导单元报警标志位
        public static ushort ErrFlag15 { get; set; } = 9999; //	充电仓报警标志位
        public static ushort ErrFlag16 { get; set; } = 9999; //	电动门报警标志位
        public static ushort ErrFlag17 { get; set; } = 9999; //	风机报警标志位
        public static ushort ErrFlag18 { get; set; } = 9999; //	通讯故障标志位
    }

    public static class PLCM6
    {
        public static ushort pkghead { get; set; } = 9999; //	 包头
        public static Byte head { get; set; } = 255; //		数据流标识位
        public static Byte UnlockCmd = 255; //			解锁请求信号
        public static Byte LockCmd = 255; //			上锁请求信号
    }

}
