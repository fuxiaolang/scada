using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DESCADA.Service
{
    //蓝色（收）和绿色（发）
    public static class Charger
    {
        public struct Msghead
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Head;//head	2
            public Int16 Len;
            public Byte Info;
            public Byte MsgNo;
            public Int16 CMD;//head	2
        }


        //************************************服务器向充电桩设置/查询工作参数和命令*********************
        // send 1  上位机下发设置/查询充电机整型工作参数 // vardata 设置数据 N 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD1
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte Type;//类型 0-查询/1-设置
            public UInt32 StartAdd;//设置/查询参数起始地址	4
            public Byte RequestNum;//设置/查询个数	1
            public UInt16 BytesNum;//设置参数字节数	2
            //public Byte SetData;
        }

        //receive 2 充电机对CMD1的应答
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte Type;//类型 0-查询/1-设置
            public UInt32 StartAdd;//设置/查询参数起始地址	4
            public Byte RequestNum;//设置/查询个数	1
            public Byte RequestResult;//设置/查询结果	1  0表示成功，其它失败
            public Byte RequestPara;// 查询参数信息 N !!!!ending 当类型为设置时才有此字段

            public Byte CheckFlag;//校验标志	1

        }

        // send 3  后台服务器下发充电桩字符型参数 // vardata 设置数据 N !!!!ending 当类型为设置时才有此字段
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD3
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte Type;//类型 0-查询/1-设置
            public UInt32 StartAdd;//设置/查询参数起始地址	4
            public UInt16 BytesNum;//设置参数字节数	2
            //public Byte SetData;
        }

        //receive 4 充电机对CMD3的应答 // 查询参数信息 N !!!!ending 当类型为设置时才有此字段
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD4
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte Type;//类型 0-查询/1-设置
            public UInt32 StartAdd;//设置/查询参数起始地址	4
            //public Byte RequestNum;//设置/查询个数	1
            public Byte InqResult;//设置/查询结果	1  0表示成功，其它失败
            //public Byte InqPara;// 查询参数信息 N !!!!ending 当类型为设置时才有此字段

            public Byte CheckFlag;//校验标志	1

        }

        // send 5  后台服务器下发充电桩控制命令 SetData; 命令参数 n
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD5
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte GunNo;//充电枪口	1	只有一机一桩此参数可为0，多枪编号从1开始 
            public UInt32 StartAdd;// 启始命令地址	4
            public Byte RequestNum;//命令个数	1	
            public UInt16 RequestLen;//命令参数长度	2	命令个数*4（字节）
            //public Byte SetData; 命令参数 n
        }

        //receive 6 充电桩对后台控制命令应答
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD6
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte GunNo;//充电枪口	
            public UInt32 StartAdd;//命令启始标志	4
            public Byte RequestNum;//命令个数	1
            public Byte InqResult;//命令执行结果	1  0表示成功，其它失败

            public Byte CheckFlag;//校验标志	1

        }

        // send 7  后台服务器下发充电桩开启充电控制命令
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD7
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte GunNo;//充电枪口	1	只有一机一桩此参数可为0，多枪编号从1开始 

            public UInt32 EffectType;//充电生效类型	4
            public UInt32 StopPass;//界面充电停止密码	4
            public UInt32 ChargeStrategy;//充电策略	4
            public UInt32 ChargeStrategyPara_3;//充电策略参数	4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] SystemTime;//预约/定时启动时间	8
            public Byte ReserveTimeout;//预约超时时间	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ReserveCardNO;//用户卡号/用户识别号	32
            public Byte DisconnChargeFlag;//断网充电标志	1
            public UInt32 MaxKWH;//离线时本次最大可充电总电量	4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargeSN_1; //充电流水号	32; 
            public Byte BmsU;//Bms辅源电压	1
            public UInt32 CardNum;//用户账号余额	4
            public Byte ChargeFlag;//充放电标志	1
        }

        //receive 8 充电桩对后台下发的充电桩开启充电控制应答
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD8
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte GunNo;//充电枪口	
            //public Byte RequestResult;//命令执行结果	1  0表示成功，其它失败
            public Int32 RequestResult;//20240328 长度按4字节
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargeSN_1; //充电流水号	32; 

            public Byte CheckFlag;//校验标志	1

        }

        //AA-F5-52-00-02-00-08-00
        //-00-00-
        //00-00
        //-31-00-00-00-00-00-00-00-00-00
        //-00-00-00-00-00-00-00-00-00-00
        //-00-00-00-00-00-00-00-00-00-00
        //-00-00
        //
        //-01
        //-0D-00-00-00
        //-31-31-32-30-32-34-30-31-33-30
        //-31-37-35-30-33-38-00-00-00-00
        //-00-00-00-00-00-00-00-00-00-00
        //-00-00
        //-6D


        // send 11  服务器查询充电桩软件版本(非必实现功能)
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD11
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public UInt16 InqDeviceType;//设备类型	2
        }

        //receive 12 充电桩应答软件版本(非必实现功能)
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD12
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public UInt16 RtnDeviceType;//返回设备类型	2 与命令11字段4定义设备含义一致
            public UInt16 SoftVer1_030;//1号软件版本	2 
            public UInt16 SoftVer2_030;//1号软件版本	2 
            public UInt16 SoftVer3_030;//1号软件版本	2 

            public Byte CheckFlag;//校验标志	1

        }


        //************************************充电机主动上报数据*********************


        // send 101  服务器应答心跳包信息
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD101
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public UInt16 heart;//心跳应答	2 到最大值时为1，重新累加
        }
        //receive 102 充电桩上传心跳包信息
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD102
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public UInt16 heart;//心跳应答	2 到最大值时为1，重新累加 缺省=0
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] GunStatus_2;//各枪状态	16 每位代表一个枪 0-未插入枪 1-已插枪

            public Byte CheckFlag;//校验标志	1

        }
        // send 103 服务器应答充电桩状态信息包
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD103
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte GunNo;//1 充电枪口	 这个字段与104报文字段5
            public Byte Send104Flag;//1 是否立即上报一次104报文 0：否 1：是
        }

        //receive 104 充电桩状态
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD104
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte GunNum;//充电枪个数	1
            public Byte GunNo;//充电口（枪）号	1
            public Byte GunType;//充电枪类型	1
            public Byte WorkStatus;//工作状态 1

            public Byte CurrentSOC; //当前SOC %	1;
            public UInt32 CurrentWarnMaxNo; //当前最高告警编码	4;
            public Byte VehConnectStatus; //车连接状态	1;
            public UInt32 ChargeFee_030; //本次充电累计充电费用	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] PrivateVar2; //内部变量2	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] PrivateVar3; //内部变量3	4;
            public UInt16 ChargeU_010; //直流充电电压	2;
            public UInt16 ChargeI_010; //直流充电电流	2;
            public UInt16 BmsU_010; //Bms需求电压	2;
            public UInt16 BmsI_010; //Bms需求电流	2;
            public Byte BmsChargeMode; //Bms充电模式	1;
            public UInt16 AU_010; //交流A相充电电压	2;
            public UInt16 BU_010; //交流B相充电电压	2;
            public UInt16 CU_010; //交流C相充电电压	2;
            public UInt16 AI_010; //交流A相充电电流	2;
            public UInt16 BI_010; //交流B相充电电流	2;
            public UInt16 CI_010; //交流C相充电电流	2;
            public UInt16 RemainChargeTime; //剩余充电时间(min)	2;
            public UInt32 ChargeTime; //充电时长(秒)	4;
            public UInt32 KWH_030; //本次充电累计充电电量（0.01kwh）	4;
            public UInt32 PreMeterNum_030; //充电前电表读数	4;
            public UInt32 CurrentMeterNum_030; //当前电表读数	4;
            public Byte StartMode; //充电启动方式	1;
            public Byte ChargeStrategy; //充电策略	1;
            public UInt32 ChargeStrategyPara_3; //充电策略参数	4;
            public Byte ReserveFlag; //预约标志	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ReserveCardNO_1; //充电/预约卡号	32;
            public Byte ReserveTimeout; //预约超时时间	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] ReserveStartChargeTime_4; //预约/开始充电开始时间	8;
            public UInt32 PreCardNum_030; //充电前卡余额	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] Reserve3; //预留	4;
            public UInt32 KW_010; //充电功率	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] Var3; //系统变量3	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] Var4; //系统变量4	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] Var5; //系统变量5	4;
            public Byte ExitC_002; //出风口温度	1;
            public Byte EnvirC_002; //环境温度	1;
            public Byte GunC_002; //充电枪温度	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public Byte[] VIN_1; //车辆VIN码	18;
            public Byte GateStatus; //舱门状态	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargeSN_1; //充电流水号	32; 
            public UInt32 ElecFee_030; //本次充电累计充电电费	4;
            public UInt32 ServeFee_030; //本次充电累计充电服务费	4;
            public Byte GunGoHomeStatus; //枪归位状态	1;
            public Byte GroundLockStatus; //地锁状态	1;
            public Byte ThousandKWH_040; //千分位电量	1;
            public UInt32 MeterKWH_040; //直流桩交流电表电量	4;
            public Byte CurrentChargeMode; //当前充电模式	1;
            public Byte ChargeFlag; //充放电标志	1;
            public Byte PowerStatus; //power状态	1;
            public Byte GunGas1; //充电仓气体高1	1;
            public Byte GunGas2; //充电仓气体高2	1;
            public Byte GunSmoke1; //充电仓烟雾高1	1;
            public Byte GunSmoke2; //充电仓烟雾高2	1;
            public Byte FireLevel; //消防预警级别	1;

            public Byte CheckFlag;//校验标志	1

        }

        // send 105  签到106的回执
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD105
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] RandNum;//桩生成随机数	4
            public Byte logonCheck;//登入验证
            public Byte EncryptionFlag; //加密标志
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public Byte[] RSAPublic;//RSA公共模数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] RSAPublicKey;//RSA公密
            public Byte EnableFlag;//启停用标志
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] SystemTime;//平台标准BCD时间	8
            public Byte Enable222Flag;//启停用标志 0
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            //public Byte[] AESKey;//RSA公密


        }

        //receive 106 签到  回105
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD106
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte Flag;//标志	1
            public UInt32 SoftVer_030;//充电桩软件版本	4
            public UInt16 ProjType;//充电桩项目类型	2
            public UInt32 StartNum;//启动次数	4
            public Byte UploadMode;//数据上传模式	1
            public UInt16 TimeSpan;//签到间隔时间	2
            public Byte InternalVar;//运行内部变量	1
            public Byte GunNum;//充电枪个数	1
            public Byte HeartCycle;//心跳上报周期	1
            public Byte TimeoutNum;//心跳包检测超时次数	1
            public UInt32 RecordNum;//充电记录数量	4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] SystemTime_4;//当前充电桩系统时间	8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public Byte[] Reserve3;//预留	24
            //public Int32 RandNum;//桩生成随机数	4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] RandNum;//桩生成随机数	4
            public UInt16 CommSoftVer_010;//桩后台通信协议版本	2
            public UInt32 WhiteListVer_000;//白名单版本号	4
            public Byte DeviceType;//设备类型	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] PropertyNo_1;//充电桩资产编码	32
            public Byte CheckFlag;//校验标志	1
        }

        //receive 108 充电桩告警信息上报（预留）
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD108
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] Warn_2;//告警位信息	32 每一位代码一个告警

            public Byte CheckFlag;//校验标志	1

        }

        //************************************充电信息数据*********************
        // send 221  服务器应答充电桩上报充电信息报文
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD221
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte GunNo;//充电口（枪）号	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] CardNO;//用户卡号/用户识别号	32
            public Int32 PrivateIndex; //内部索引号	4;
            public Byte Field789ValidFlag;//7、8、9字段有效标志	1
            public UInt32 OriginalFee;//充电优惠前金额	4
            public UInt32 Discount;// 充电折扣金额	4
            public UInt32 ActualFee;//充电实扣金额	4
            public UInt32 UserBalance;//用户剩余金额	4
            public UInt32 ActualElecFee;//充电实扣电费	4
            public UInt32 ActualServeFee;//充电实扣服务费	4
        }

        //receive 222 充电桩上报充电记录信息
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD222
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte GunAType; //充电枪位置类型	1; 1-直流 2-交流
            public Byte GunNo; //充电枪口	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] CardNO_1;//充电卡号	32;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] ChargeStartTime_4;//充电开始时间		8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] ChargeEndTime_4;//充电结束时间	8
            public UInt32 ChaegeTimeLen; //充电时间长度	4;
            public Byte StartSOC; //开始SOC	1;
            public Byte EndSOC; //结束SOC	1;
            public Int32 EndReason; //充电结束原因	4;
            public Int32 ChargeCapacity_040; //本次充电电量	4;
            public UInt32 PreMeterNum_040; //充电前电表读数	4;
            public UInt32 AfterMeterNum_040; //充电后电表读数	4;
            public Int32 ChargePrice_030; //本次充电金额	4;
            public Int32 PrivateIndex; //内部索引号	4;
            public Int32 PreCardNum_030; //充电前卡余额	4;
            public Int32 CurrentRecordIndex; //当前充电记录索引	4;
            public Int32 TTLRecordNum; //总充电记录条目	4;
            public Byte Reserve3; //预留	1;
            public Byte ChargeStrategy; //充电策略	1;
            public UInt32 ChargeStrategyPara_3; //充电策略参数	4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public Byte[] VIN_1; //车辆VIN码
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] PlateNo_1; //车牌号	8;
            public UInt32 Period1KWH_040; //时段1充电电量	2/4;
            public UInt32 Period2KWH_040; //时段2充电电量	2/4;
            public UInt32 Period3KWH_040; //时段3充电电量	2/4;
            public UInt32 Period4KWH_040; //时段4充电电量	2/4;
            public UInt32 Period5KWH_040; //时段5充电电量	2/4;
            public UInt32 Period6KWH_040; //时段6充电电量	2/4;
            public UInt32 Period7KWH_040; //时段7充电电量	2/4;
            public UInt32 Period8KWH_040; //时段8充电电量	2/4;
            public UInt32 Period9KWH_040; //时段9充电电量	2/4;
            public UInt32 Period10KWH_040; //时段10充电电量	2/4;
            public UInt32 Period11KWH_040; //时段11充电电量	2/4;
            public UInt32 Period12KWH_040; //时段12充电电量	2/4;
            public UInt32 Period13KWH_040; //时段13充电电量	2/4;
            public UInt32 Period14KWH_040; //时段14充电电量	2/4;
            public UInt32 Period15KWH_040; //时段15充电电量	2/4;
            public UInt32 Period16KWH_040; //时段16充电电量	2/4;
            public UInt32 Period17KWH_040; //时段1充电电量	2/4;
            public UInt32 Period18KWH_040; //时段2充电电量	2/4;
            public UInt32 Period19KWH_040; //时段3充电电量	2/4;
            public UInt32 Period20KWH_040; //时段4充电电量	2/4;
            public UInt32 Period21KWH_040; //时段5充电电量	2/4;
            public UInt32 Period22KWH_040; //时段6充电电量	2/4;
            public UInt32 Period23KWH_040; //时段7充电电量	2/4;
            public UInt32 Period24KWH_040; //时段8充电电量	2/4;
            public UInt32 Period25KWH_040; //时段9充电电量	2/4;
            public UInt32 Period26KWH_040; //时段10充电电量	2/4;
            public UInt32 Period27KWH_040; //时段11充电电量	2/4;
            public UInt32 Period28KWH_040; //时段12充电电量	2/4;
            public UInt32 Period29KWH_040; //时段13充电电量	2/4;
            public UInt32 Period30KWH_040; //时段14充电电量	2/4;
            public UInt32 Period31KWH_040; //时段15充电电量	2/4;
            public UInt32 Period32KWH_040; //时段16充电电量	2/4;
            public UInt32 Period33KWH_040; //时段1充电电量	2/4;
            public UInt32 Period34KWH_040; //时段2充电电量	2/4;
            public UInt32 Period35KWH_040; //时段3充电电量	2/4;
            public UInt32 Period36KWH_040; //时段4充电电量	2/4;
            public UInt32 Period37KWH_040; //时段5充电电量	2/4;
            public UInt32 Period38KWH_040; //时段6充电电量	2/4;
            public UInt32 Period39KWH_040; //时段7充电电量	2/4;
            public UInt32 Period40KWH_040; //时段8充电电量	2/4;
            public UInt32 Period41KWH_040; //时段9充电电量	2/4;
            public UInt32 Period42KWH_040; //时段10充电电量	2/4;
            public UInt32 Period43KWH_040; //时段11充电电量	2/4;
            public UInt32 Period44KWH_040; //时段12充电电量	2/4;
            public UInt32 Period45KWH_040; //时段13充电电量	2/4;
            public UInt32 Period46KWH_040; //时段14充电电量	2/4;
            public UInt32 Period47KWH_040; //时段15充电电量	2/4;
            public UInt32 Period48KWH_040; //时段16充电电量	2/4;
            public Byte StartMode; //充电启动方式	1; //启动方式	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargeSN_1; //充电流水号	32;
            public UInt32 ServeFee_030; //充电服务费	4;
            public Byte ThousandKWH_040; //千分位电量	1;
            public Byte ParallelChargeFlag; //并充标志	1;
            public Byte ChargeFlag; //充放电标志	;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 34)]
            public Byte[] ElecMeterEncryData; //电表加密数据	34;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Byte[] ElecMeterNo_5; //电表表号	6;
            public Int16 ElecMeterSoftVer; //电表协议版本	2;
            public Byte EncryptionMethod; //加密方式	1;

            public Byte CheckFlag;//校验标志	1

        }

        //====************************************Bms信息数据*********************
        // send CMD301	上位机应答充电机上报Bms信息
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD301
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
        }

        //receive CMD302	充电机上报Bms信息（充电中默认30s/次）
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD302
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public UInt16 MsgSN; //报文次序计数	2;
            public Byte GunNo; //充电枪口	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte WorkStatus;//工作状态 1
            public Byte VehConnectStatus; //车连接状态	1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] BrmBmsCommVer; //Brm-Bms通讯协议版本号	3	0x00 0x01 0x01表示v1.1
            public Byte BrmBatteryType; //Brm-电池类型	1	电池类型：0x01-铅酸电池，0x02-镍氢电池,0x03-磷酸铁锂电池,0x04-锰酸锂电池,0x05-钴酸锂电池,0x06-三元次料电池,0x07-聚合物锂离子电池,0x08-钛酸锂电池,0xff-其他电池
            public UInt32 BrmAh_010; //Brm-整车动力蓄电池系统额定容量/Ah	4	精度：0.1
            public UInt32 BrmV_010; //Brm-整车动力蓄电池系统额定总电压/V	4	精度：0.1
            public UInt32 BrmBatteryProducer; //Brm-电池生产厂商	4	
            public UInt32 BrmPackNo; //Brm-电池组序号	4	预留，由厂商自行定义
            public UInt16 BrmBatteryProdY; //Brm-电池组生厂日期：年	2	如0x07 0xdf表示2015
            public Byte BrmBatteryProdM; //Brm-电池组生厂日期：月	1	如0x01表示1月
            public Byte BrmBatteryProdD; //Brm-电池组生厂日期：日	1	如0x01表示1号
            public UInt32 BrmPackChargeNum; //Brm-电池组充电次数	4	以Bms统计为准
            public Byte BrmPackPropertyID; //Brm-电池组产权标识	1	0X01-表示租赁，0x01表示车自有
            public Byte Reserve3; //预留	1	
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public Byte[] BrmVin_2; //Brm-车辆识别码vin	17	
            public UInt32 BrmBmsSoftVer; //Brm-Bms软件版本号	8	Byte1表示版本流水号：0x01~0xfe；byte2表示日：0x01表示1日；byte3：0x01表示1月；byte4和byte5表示年:0x07 0xdf表示2015年；Byte6和byte7、byte8预留，填0xffffff
            public UInt32 BcpBatteryMaxU_010; //Bcp-单体动力蓄电池最高允许充电电压	4	根据分辨率：0.1A位，数据范围：0-24V
            public UInt32 BcpMaxI_010; //Bcp-最高允许充电电流	4	分辨率：0.1A/位
            public UInt32 BcpTtlKWH_010; //Bcp-动力蓄电池标称总能量	4	分辨率：0.1kw.h，范围0-1000kw.h
            public UInt32 BcpMaxU_010; //Bcp-最高允许充电总电压	4	分辨率：0.1V
            public Byte BcpMaxC_002; //Bcp-最高允许温度	1	对-50的偏移量，0表示50，250表示200
            public UInt16 BcpBatteryStatus_010; //Bcp-整车动力蓄电池荷电状态	2	分辨率：0.1，0-100 %
            public UInt32 BcpBatteryCurrentU_010; //Bcp-整车动力蓄电池当前电池电压	4	分辨率：0.1V
            public Byte BROBmsReadyFlag; //BRO-Bms是否充电准备好	1	0x00-表示未做好充电准备，0xaa表示Bms完成充电准备，0xff表示无效
            public UInt32 BclU_010; //Bcl-电压需求	4	分辨率：0.1V
            public UInt32 BclI_010; //Bcl-电流需求	4	分辨率：0.1A
            public Byte BclChargeMode; //Bcl-充电模式	1	0x01表示恒压充电，0x02表示恒流充电
            public UInt32 BcsU_010; //Bcs-充电电压测量值	4	分辨率：0.1V
            public UInt32 BcsI_010; //Bcs-充电电流测量值	4	分辨率：0.1A
            public Byte BcsBatteryMaxU_030; //Bcs-最高单体动力蓄电池电压	4	分辨率：0.01V
            public Byte BcsBatteryNo; //Bcs-最高单体动力蓄电池组号	1	0-15
            public Byte BcsSoc_010; //Bcs-当前荷电状态soc%	2	分辨率：0.1，0-100%
            public UInt32 RemainChargeTime; //估算剩余充电时间	4	0-600min
            public Byte BsmBatteryMaxUNo; //Bsm-最高单体动力蓄电池电压所在编号	1	1-256
            public Byte BsmBatteryMaxC_002; //Bsm-最高动力蓄电池温度	1	对-50的偏移量，0表示50，250表示200
            public Byte BsmBatteryMaxCheckNo; //Bsm-最高温度检测点编号	1	1-128
            public Byte BsmBatteryMinC_002; //Bsm-最低动力蓄电池温度	1	对-50的偏移量，0表示50，250表示200
            public Byte BsmBatteryMinCheckNo; //Bsm-最低动力蓄电池温度检测点编号	1	1-128
            public Byte BsmBatteryUHighFlag; //Bsm-单体动力蓄电池电压过高或过低	1	0x00-正常，0x01-过高，0x10-过低
            public Byte BsmBatterySocHighFlag; //Bsm-整车动力蓄电池荷电状态soc过高或过低	1	0x00-正常，0x01-过高，0x10-过低
            public Byte BsmBatteryIHighFlag; //Bsm-动力蓄电池充电过电流	1	0x00-正常，0x01-过流，0x10-不可信状态
            public Byte BsmBatteryCHighFlag; //Bsm-动力蓄电池温度过高	1	0x00-正常，0x01-过高，0x10-不可信状态
            public Byte BsmBatteryInsulationFlag; //Bsm-动力蓄电池绝缘状态	1	0x00-正常，0x01-不正常，0x10-不可信状态
            public Byte BsmBatteryConnFlag; //Bsm-动力蓄电池组输出连接器连接状态	1	0x00-正常，0x01-不正常，0x10-不可信状态
            public Byte BsmEnableChargeFlag; //Bsm-允许充电	1	0x00-禁止，0x01-允许
            public Byte BstBmsSoc; //Bst-Bms达到所需求的SOC目标值	1	0x00-未达到所需soc目标值，0x01-达到所需soc目标值，0x10-不可信状态
            public Byte BstBmsU; //Bst-Bms达到总电压的设定值	1	0x00-未达到总电压设定值，0x01-达到总电压设定值，0x10-不可信状态
            public Byte BstU; //Bst-达到单体电压的设定值	1	0x00-未达到单体电压设定值，0x01-达到单体电压设定值，0x10-不可信状态
            public Byte BstChargeStop; //Bst-充电机主动终止	1	0x00-正常，0x01-充电机终止，0x10-不可信状态
            public Byte BstInsulationFault; //Bst-绝缘故障	1	0x00-正常，0x01-故障，0x10-不可信状态
            public Byte BstCHighFault; //Bst-输出连接器过温故障	1	0x00-正常，0x01-故障，0x10-不可信状态
            public Byte BstBmsCHighFault; //Bst-Bms元件，输出连接器过温	1	0x00-正常，0x01-故障，0x10-不可信状态
            public Byte BstConnFault; //Bst-充电连接器故障	1	0x00-充电连接器正常，0x01-充电连接器故障，0x10-不可信状态
            public Byte BstBatteryCHighFault; //Bst-电池组温度过高故障	1	0x00-电池组温度正常，0x01-电池组温度过高，0x10-不可信状态
            public Byte BstUFault; //Bst-高压继电器故障	1	0x00-正常，0x01-故障，0x10-不可信状态
            public Byte BstCheck2UFault; //Bst-检测点2电压检测故障	1	0x00-正常，0x01-故障，0x10-不可信状态
            public Byte BstOtherFault; //Bst-其他故障	1	0x00-正常，0x01-故障，0x10-不可信状态
            public Byte BstUHigh; //Bst-电流过大	1	0x00-电流正常，0x01-电流超过需求值，0x10-不可信状态
            public Byte BstUException; //Bst-电压异常	1	0x00-正常，0x01-电压异常，0x10-不可信状态
            public UInt16 BsdSoc_010; //BSD-终止荷电状态soc	2	分辨率：0.1，0-100%
            public UInt32 BsdMinU_030; //BSD-动力蓄电池单体最低电压	4	分辨率：0.01,0-24
            public UInt32 BsdMaxU_030; //BSD-动力蓄电池单体最高电压	4     	分辨率：0.01,0-24
            public Byte BsdMinC_002; //BSD-动力蓄电池最低温度	1	对-50的偏移量，0表示50，250表示200
            public Byte BsdMaxC_002; //BSD-动力蓄电池最高温度	1	对-50的偏移量，0表示50，250表示200
            public Byte Bem0Timeout; //BEM-接收SPN2560=0x00的充电机辨识报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemATimeout; //BEM-接收SPN2560=0xaa的充电机辨识报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemMaxAbMsgTimeout; //BEM-接收充电机的时间同步和最大输出能力报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemReadyMsgTimeout; //BEM-接收充电机完成充电准备报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemStatusMsgTimeout; //BEM-接收充电机充电状态报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemStopMsgTimeout; //BEM-接收充电机终止充电报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemStatMsgTimeout; //BEM-接收充电机充电统计报文超时	1	0x00-正常，0x01-超时，0x10-不可信状态
            public Byte BemOther; //BEM-其他	1	

            public Byte CheckFlag;//校验标志	1
        }


        //************************************计费策略相关指令*********************
        // send CMD1103	上位机设置24小时时段电费计价策略
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD1103
        {
            public Byte StartH1; //开始小时	1	0~24
            public Byte StartM1; //开始分钟	1	0或30
            public Byte EndH1; //结束小时	1	0~24
            public Byte EndM1; //结束分钟	1	0或30
            public Byte StartH2; //开始小时	1	0~24
            public Byte StartM2; //开始分钟	1	0或30
            public Byte EndH2; //结束小时	1	0~24
            public Byte EndM2; //结束分钟	1	0或30
            public UInt32 Rate2; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH3; //开始小时	1	0~24
            public Byte StartM3; //开始分钟	1	0或30
            public Byte EndH3; //结束小时	1	0~24
            public Byte EndM3; //结束分钟	1	0或30
            public UInt32 Rate3; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH4; //开始小时	1	0~24
            public Byte StartM4; //开始分钟	1	0或30
            public Byte EndH4; //结束小时	1	0~24
            public Byte EndM4; //结束分钟	1	0或30
            public UInt32 Rate4; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH5; //开始小时	1	0~24
            public Byte StartM5; //开始分钟	1	0或30
            public Byte EndH5; //结束小时	1	0~24
            public Byte EndM5; //结束分钟	1	0或30
            public UInt32 Rate5; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH6; //开始小时	1	0~24
            public Byte StartM6; //开始分钟	1	0或30
            public Byte EndH6; //结束小时	1	0~24
            public Byte EndM6; //结束分钟	1	0或30
            public UInt32 Rate6; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH7; //开始小时	1	0~24
            public Byte StartM7; //开始分钟	1	0或30
            public Byte EndH7; //结束小时	1	0~24
            public Byte EndM7; //结束分钟	1	0或30
            public UInt32 Rate7; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH8; //开始小时	1	0~24
            public Byte StartM8; //开始分钟	1	0或30
            public Byte EndH8; //结束小时	1	0~24
            public Byte EndM8; //结束分钟	1	0或30
            public UInt32 Rate8; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH9; //开始小时	1	0~24
            public Byte StartM9; //开始分钟	1	0或30
            public Byte EndH9; //结束小时	1	0~24
            public Byte EndM9; //结束分钟	1	0或30
            public UInt32 Rate9; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH10; //开始小时	1	0~24
            public Byte StartM10; //开始分钟	1	0或30
            public Byte EndH10; //结束小时	1	0~24
            public Byte EndM10; //结束分钟	1	0或30
            public UInt32 Rate10; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH11; //开始小时	1	0~24
            public Byte StartM11; //开始分钟	1	0或30
            public Byte EndH11; //结束小时	1	0~24
            public Byte EndM11; //结束分钟	1	0或30
            public UInt32 Rate11; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
            public Byte StartH12; //开始小时	1	0~24
            public Byte StartM12; //开始分钟	1	0或30
            public Byte EndH12; //结束小时	1	0~24
            public Byte EndM12; //结束分钟	1	0或30
            public UInt32 Rate12; //费率1	4	该时段内每度电的电费，用整型值表示，要乘0.01才能得到真实的值
        }

        //receive CMD1104	充电机应答上位机24时电费计价策略
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD1104
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            public Byte Result; //确认结果	1	0--成功  1--失败

            public Byte CheckFlag;//校验标志	1

        }


        //************************************5.3版新增电池监控报文meng标注*********************
        // receive CMD116	充电机转发整车CAN电池监控报文
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD116
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            public Byte BattertySNLen; //电电池SN编码长度	1
            public Byte BattertyFactory; //电池厂家	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
            public Byte[] BattertySN_1; //电池SN编码	27
            public UInt16 PackAh; //电池包额定容量	2
            public UInt16 PackU; //电池包额定电压	2
            public UInt16 PackPower; //电池包额定总能量	2
            public Byte BatteryCoolMethod; //电池冷却方式	1
            public Byte BatteryType; //电池类型	1
            public Byte CSC; //CSC总数	1
            public UInt16 UnitCellTtl; //单体电芯总数	2
            public UInt16 CellCCheckTtl; //电芯温度测点总数 	2
            public Byte UnitU_010; //单体平台电压	1
            public Byte UnitMaxU_010; //单体可用最高电压	1
            public Byte UnitMinU_010; //单体可用最低电压	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] BMSWarn1_2; //BMS报警信息1（1880d0f3）	8
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] BMSWarn2_2; //BMS报警信息2（1881d0f3）	8
            public Byte PackSoc_020; //电池包SOC	1
            public Byte PackSoh_020; //电池包SOH	1
            public UInt16 PackTtlI_003; //电池包总电流	2
            public UInt16 PackMaxChargeI_010; //电池包允许最大回充电流	2
            public UInt16 PackMaxDischargeI_010; //电池包允许最大放电电流	2
            public UInt16 PackPositive; //电池包正极绝缘值	2
            public UInt16 PackNegative; //电池包负极绝缘值	2
            public UInt16 BatteryHighU_010; //电池端高压	2
            public UInt16 GeneratrixHighU_010; //母线端高压	2
            public Byte UnitCellCMax_002; //电芯温度最大值	1
            public Byte UnitCellCMin_002; //电芯温度最小值	1
            public Byte UnitCellCAvg_002; //电芯温度平均值	1
            public Byte UnitCellCMaxNo; //电芯温度最大值所在编号	1
            public Byte UnitCellCMinNo; //电芯温度最小值所在编号	1
            public UInt16 UnitCellUMax_040; //电芯电压最大值	2
            public UInt16 UnitCellUMin_040; //电芯电压最小值	2
            public UInt16 UnitCellUAvg_040; //电芯电压平均值	2
            public Byte UnitCellUMaxNo_040; //电芯电压最大值所在编号	1
            public Byte UnitCellUMinNo_040; //电芯电压最小值所在编号	1
            public Byte AGunC1_002; //A枪温度1/充电连接器1温度1	1
            public Byte AGunC2_002; //A枪温度2/充电连接器1温度2	1
            public Byte BGunC1_002; //B枪温度1/充电连接器2温度1	1
            public Byte BGunC2_002; //B枪温度2/充电连接器2温度2	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] HardVer_1; //硬件版本	3
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Byte[] SoftVer; //软件版本	6
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] TtlChargeNum_610; //累计充电量	3 !!!
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] TtlDisChargeNum_610; //累计放电量	3
            public UInt16 SingleChargeNum_010; //单次充电量	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] TtlFeedbackI_610; //累计动能回馈电流	3
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] TtlSwitchI_610; //累计换电电量	3
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Byte[] TtlChargeI_610; //累计插枪充电电量	3
            public Byte TmsWorkStatus; //TMS工作状态	1
            public Byte TmsRelayStatus; //TMS继电器状态	1
            public Byte TmsEffluentC_001; //TMS出水温度	1
            public Byte TmsInletC_001; //TMS进水温度	1
            public UInt16 ReqPower_010; //水冷机组需求功率	2
            public Byte TMSFaultCode; //TMS故障代码	1
            public Byte TMSFaultLevel; //TMS故障等级	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte CheckFlag;//校验标志	1
        }

        // receive ending CMD118	电池单体电压报文 m=CSC总数  N=单体电芯总数/m （116）!!!!!!!!!!!
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD118
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte CheckFlag;//校验标志	1
        }

        //   receive ending CMD120	电池单体温度报文  m=CSC总数  N=单体电芯总数/m （116）!!!!!!!!!!!!!!!
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD120
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte CheckFlag;//校验标志	1
        }


        // send 207  平台回复”即插即充“请求充电结果
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD207
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte GunNo;//充电口（枪）号	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public Byte[] VIN_1; //车辆VIN码	17;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] VinCardNO;// 车辆VIN 绑定账号	32
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargeSN_1; //充电流水号	32; 
            public Byte CheckResult;//验证结果	1
            public Byte CheckReason;//验证原因	1
            public UInt32 UserBalance;//可充电余额	4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Byte[] ScreenPass_1; //屏幕停机密码	6; 
        }

        //receive 208 充电桩上报“即插即充“启动充电请求
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD208
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Header;//head	8

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] ChargerNo_1;//充电桩编码	32
            public Byte GunNo;//充电口（枪）号	1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public Byte[] VIN_1; //车辆VIN码	17;
            public Byte ChargeStrategy; //充电策略	1;
            public UInt32 ChargeStrategyPara_3; //充电策略参数	4;

            public Byte CheckFlag;//校验标志	1

        }

        // send 1401 服务器下发FTP远程升级命令
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD1401
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public UInt16 SoftVer;//升级软件文件版本号	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] FTPIP;//FTP服务器IP地址	32
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] FTPuser; //FTP 用户名	16
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] FTPPass; //FTP 密码	16
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public Byte[] FTPPath; //FTP 下载路径	64
            public Byte RunMode; //F执行方式	1
            public Byte DevType; //本次升级设备类型	1

        }

     

        // send 1406 log
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CMD1406
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve1;//预留	2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Reserve2;//预留	2
            public Byte LogType;//获取日志类型	1.bms日志2.系统日志3.记录日志4.全部日志
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] FTPIP;//FTP服务器IP地址	32
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] FTPuser; //FTP 用户名	16
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] FTPPass; //FTP 密码	16
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public Byte[] FTPPath; //FTP 存放路径	64
            public Byte RunMode; //F执行方式	1.空闲执行 2.立即执行

        }
    }
}
