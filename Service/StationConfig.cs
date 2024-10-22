using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

namespace DESCADA
{
    public class ChargerNetConfig
    {
        /// <summary>
        /// 充电机通讯网络IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 充电机网络通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 网络ID
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 铭牌ID
        /// </summary>
        [XmlAttribute]
        public string UniqId
        {
            get;
            set;
        }

        /// <summary>
        /// 外接枪数量
        /// </summary>
        [XmlAttribute]
        public int OutGunNum
        {
            get;
            set;
        }



        /// <summary>
        /// 网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        /// <summary>
        /// 充电机数量
        /// </summary>
        [XmlAttribute]
        public int ChargerCount
        {
            get;
            set;
        }

        /// <summary>
        /// 充电机起始地址
        /// </summary>
        [XmlAttribute]
        public byte StartChargerAddr
        {
            get;
            set;
        }
        /// <summary>
        /// 初始充电机ID
        /// </summary>
        [XmlAttribute]
        public int StartChargerID
        {
            get;
            set;
        }

        public ChargerNetConfig()
        {
            this.Text = string.Empty; this.IP = string.Empty;
        }

        public ChargerNetConfig(string ip, int port, int chargercount, int netid, string nettext, byte startaddress, int startchargerid)
        {
            this.IP = ip; this.Port = port; this.ChargerCount = chargercount; this.ID = netid; this.Text = nettext; this.StartChargerAddr = startaddress; this.StartChargerID = startchargerid;
        }
        /// <summary>
        /// 是合法的配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(this.IP, out ip)) return false;
            if (this.Port < 1 || this.ChargerCount < 1 || this.StartChargerAddr <= 0 || this.StartChargerID < 0 || ((this.StartChargerAddr + this.ChargerCount) > 254) || this.Text.Length < 1) return false;
            return true;
        }
    }
    /// <summary>
    /// 电表当前值偏置
    /// </summary>
    public class WattMeterOffSet
    {
        /// <summary>
        /// 电表编号
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        public double ComboEnergyOffset
        {
            get;
            set;
        }
        [XmlAttribute]
        public double ComboSharpEnergyOffset
        {
            get;
            set;
        }
        [XmlAttribute]
        public double ComboPeakEnergyOffset
        {
            get;
            set;
        }
        [XmlAttribute]
        public double ComboFlatEnergyOffset
        {
            get;
            set;
        }
        [XmlAttribute]
        public double ComboValleyEnergyOffset
        {
            get;
            set;
        }
        public double ComboVarEnergyOffset
        {
            get;
            set;
        }
        public WattMeterOffSet()
        { }
        public WattMeterOffSet(int id, double comboener, double sharpcombo, double peakcombo, double flatcombo, double valleycombo, double varcombo)
        {
            this.ID = id;
            this.ComboEnergyOffset = comboener;
            this.ComboSharpEnergyOffset = sharpcombo;
            this.ComboPeakEnergyOffset = peakcombo;
            this.ComboFlatEnergyOffset = flatcombo;
            this.ComboValleyEnergyOffset = valleycombo;
            this.ComboVarEnergyOffset = varcombo;
        }

    }
    public class WattMeterNetConfig
    {
        /// <summary>
        /// 电表通讯网络IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 电表网络通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 网络ID
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        /// <summary>
        /// 电度表数量
        /// </summary>
        [XmlAttribute]
        public int WattMeterCount
        {
            get;
            set;
        }


        /// <summary>
        /// 电度表起始地址
        /// </summary>
        [XmlAttribute]
        public byte StartAddr
        {
            get;
            set;
        }
        /// <summary>
        /// 电表起始ID
        /// </summary>
        [XmlAttribute]
        public int StartWattID
        {
            get;
            set;
        }
        /// <summary>
        /// 电表电流互感器分压比
        /// </summary>
        [XmlAttribute]
        public int Scale
        {
            get;
            set;
        }
        /// <summary>
        /// 协议代码雅达MODBUS=0,645=1
        /// </summary>
        [XmlAttribute]
        public int Protocol
        {
            get;
            set;
        }
        public WattMeterNetConfig()
        {
            this.IP = string.Empty; this.Text = string.Empty;
        }
        public WattMeterNetConfig(string ip, int port, int wattcount, int netid, string nettext, byte startaddress, int startwattid, int scale, int factorycode)
        {
            this.IP = ip; this.Port = port; this.ID = netid; this.Text = nettext; this.WattMeterCount = wattcount; this.StartAddr = startaddress;
            this.StartWattID = startwattid; this.Scale = scale; this.Protocol = factorycode;
        }
        /// <summary>
        /// 是合法的配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(this.IP, out ip)) return false;
            ///电表MODBUS地址不能用254，因为和645协议有冲突
            if (this.Port < 1 || this.WattMeterCount < 1 || this.StartAddr < 1 || this.StartWattID < 0 || ((this.StartAddr + this.WattMeterCount) > 253) || this.Text.Length < 1) return false;
            return true;
        }
    }
    public class InterFace
    {
        [XmlAttribute]
        public int Left
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Top
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Width
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Height
        {
            get;
            set;
        }
        public InterFace()
        { }
        public InterFace(int left, int top, int width, int height)
        {
            this.Left = left; this.Top = top; this.Width = width; this.Height = height;
        }
    }
    public class UnitConfig
    {
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        [XmlAttribute]
        public bool Habbit
        {
            get;
            set;
        }
        [XmlAttribute]
        public string HabbitReason
        {
            get;
            set;
        }
        public InterFace InterFace
        {
            get;
            set;
        }
        public UnitConfig()
        { this.Text = string.Empty; this.InterFace = new InterFace(); }
        public UnitConfig(int id, string text, InterFace face)
        {
            this.ID = id; this.Text = text; this.InterFace = face; this.Habbit = false; this.HabbitReason = string.Empty;
        }
        public bool IsValidConfig()
        {
            return (this.ID >= 0 && this.Text.Length > 0);
        }
    }
    public class ChargeUnitConfigs
    {
        [XmlArray("UnitConfigs")]
        public List<UnitConfig> Configs
        {
            get;
            set;
        }

        [XmlIgnore]
        public int UnitCount
        {
            get { return this.Configs.Count; }
        }
        public ChargeUnitConfigs()
        {
            this.Configs = new List<UnitConfig>();
        }
        public ChargeUnitConfigs(int count) : this()
        {
            for (int x = 0; x < count; x++)
            {
                UnitConfig temp = new UnitConfig(x, string.Format("仓位{0}", x), new InterFace());
                this.Configs.Add(temp);
            }
        }
        public void HabbitUnit(int id, string des)
        {
            if (id < 0 || id > this.Configs.Count - 1) return;//非法索引
            this.Configs[id].Habbit = true;
            this.Configs[id].HabbitReason = des;
        }
        public void EnableUnit(int id)
        {
            if (id < 0 || id > this.Configs.Count - 1) return;//非法索引
            this.Configs[id].Habbit = false;
        }
        public bool IsValidConfig()
        {
            if (this.UnitCount < 1) return false;
            for (int x = 0; x < this.UnitCount; x++)
            {
                if (this.Configs[x].ID != x || this.Configs[x].Text.Length < 1)
                    return false;
            }
            return true;
        }
    }
    public class MachineConfig
    {
        /// <summary>
        /// 设备通讯网络IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 设备网络通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 设备网络ID
        /// </summary>
        [XmlAttribute]
        public int NetID
        {
            get;
            set;
        }
        /// <summary>
        /// 设备ID
        /// </summary>
        [XmlAttribute]
        public int DeviceID
        {
            get;
            set;
        }
        /// <summary>
        /// 网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        public Size MaDuoSize
        {
            get;
            set;
        }
        public InterFace InterFace
        {
            get;
            set;
        }
        /// <summary>
        /// 正向移动吗
        /// </summary>
        public bool IsForWardMove
        {
            get;
            set;
        }
        public MachineConfig()
        {
            this.InterFace = new InterFace();
            this.Text = string.Empty;
            this.IP = string.Empty;
        }
        public MachineConfig(string ip, int port, int netid, int deviceid, string text, InterFace face, bool forward)
        {
            this.NetID = netid; this.DeviceID = deviceid; this.IP = ip; this.Port = port; this.Text = text; this.InterFace = face; this.IsForWardMove = forward;
        }
        /// <summary>
        /// 是合法的配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(this.IP, out ip)) return false;
            if (this.Port < 1 || this.Text.Length < 1) return false;
            return true;
        }
    }
    public class TempUnitConfig
    {
        /// <summary>
        /// 通讯网络IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 网络通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 通讯网络ID
        /// </summary>
        [XmlAttribute]
        public int NetID
        {
            get;
            set;
        }

        /// <summary>
        /// 网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        public UnitConfig Unit
        {
            get;
            set;
        }

        public TempUnitConfig()
        {
            this.Text = string.Empty; this.Unit = new UnitConfig(); this.Unit = new UnitConfig();
        }
        public TempUnitConfig(string ip, int port, string nettext, int netid, int unitid, string unittext, InterFace unitface)
        {
            this.IP = ip; this.Port = port; this.Text = nettext; this.NetID = netid;
            this.Unit = new UnitConfig(unitid, unittext, unitface);
        }
        /// <summary>
        /// 是合法的配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(this.IP, out ip)) return false;
            ///电表MODBUS地址不能用254，因为和645协议有冲突
            if (this.Port < 1 || this.Text.Length < 1) return false;
            if (this.Unit == null || this.Unit.IsValidConfig() == false) return false;
            return this.Unit.IsValidConfig();

        }
    }
    public class BusIdentityConfig
    {
        /// <summary>
        /// 车牌识别网络IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 车牌识别网络通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 车牌识别通讯网络ID
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 车牌识别网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }

        public InterFace InterFace
        {
            get;
            set;
        }
        [XmlAttribute]
        public bool IsForwardMove
        {
            get;
            set;
        }
        public BusIdentityConfig()
        {
            this.Text = string.Empty; this.InterFace = new InterFace();
        }
        public BusIdentityConfig(int netid, string ip, int port, string text, int deviceid, InterFace face)
        {
            this.ID = netid; this.IP = ip; this.Port = port; this.Text = text; this.InterFace = face;
        }
        /// <summary>
        /// 配置合法吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(this.IP, out ip)) return false;
            if (this.Port < 1 || this.Text.Length < 1) return false;
            return true;
        }
    }
    public class RfidConfig
    {
        /// <summary>
        /// 用户身份识别系统IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }
        /// <summary>
        /// 用户身份识别系统通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 用户身份识别系统网络ID
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 用户身份识别系统网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        public RfidConfig()
        {
            this.IP = string.Empty;
        }
        public RfidConfig(string ip, int port, string text, int id)
        {
            this.IP = ip; this.Port = port; this.Text = text; this.ID = id;
        }
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (IPAddress.TryParse(this.IP, out ip) == false) return false;
            if (this.Text == null || this.Text.Length == 0) return false;
            return true;
        }
    }
    /// <summary>
    /// 通道显示屏配置
    /// </summary>
    public class PassGateScreenConfig
    {
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        public PassGateScreenConfig(string ip, int port, string text, int id)
        {
            this.IP = ip; this.Port = port; this.Text = text; this.ID = id;
        }
        public PassGateScreenConfig()
        {
            this.IP = string.Empty;
            this.Text = string.Empty;
        }
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (IPAddress.TryParse(this.IP, out ip) == false) return false;
            if (this.Text == null || this.Text.Length == 0) return false;
            return true;
        }
    }
    public class PassGateWayConfig
    {
        /// <summary>
        /// 通道编号
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        /// <summary>
        /// 是否禁用此项检测
        /// </summary>
        [XmlAttribute]
        public bool Enable
        {
            get;
            set;
        }
        /// <summary>
        /// 激光雷达地址
        /// </summary>
        [XmlAttribute]
        public string LakiIP
        {
            get;
            set;
        }
        [XmlAttribute]
        public bool IsForwardMove
        {
            get;
            set;
        }

        /// <summary>
        /// 通道车牌配置
        /// </summary>
        public BusIdentityConfig busIdentConfig
        {
            get;
            set;
        }
        public RfidConfig CustomerIDGeter
        {
            get;
            set;
        }
        public PassGateScreenConfig ScreenConfig
        {
            get;
            set;
        }

        public PassGateWayConfig()
        {
            this.busIdentConfig = new BusIdentityConfig();
            this.CustomerIDGeter = new RfidConfig();
            this.ScreenConfig = new PassGateScreenConfig();
            //this.InterFace = new InterFace();
            this.Text = string.Empty;
        }
        public PassGateWayConfig(string lakiIP, TempUnitConfig newconfig, TempUnitConfig old, BusIdentityConfig busconfig, RfidConfig rfidconfig, PassGateScreenConfig screenconfig, int gatewaynum)
        {
            // this.InterFace = new InterFace();
            this.LakiIP = lakiIP;
            this.busIdentConfig = busconfig;
            this.CustomerIDGeter = rfidconfig;
            this.ScreenConfig = screenconfig;
            this.ID = gatewaynum;
            this.Text = string.Format("通道{0}", this.ID);

        }
        /// <summary>
        /// 是合法配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            if (busIdentConfig.IsValidConfig() == false || this.CustomerIDGeter.IsValidConfig() == false || this.ScreenConfig.IsValidConfig() == false) return false;
            return true;
        }
    }
    public class Relation
    {
        [XmlAttribute]
        public int UnitID
        {
            get;
            set;
        }
        [XmlAttribute]
        public int ChargerID
        {
            get;
            set;
        }
        [XmlAttribute]
        public int WattID
        {
            get;
            set;
        }
        public Relation()
        { }
        public Relation(int unit, int charger, int watter)
        {
            this.UnitID = unit; this.ChargerID = charger; this.WattID = watter;
        }
        public bool IsValidConfig()
        {
            return (this.UnitID >= 0 && this.ChargerID >= 0 && this.WattID >= 0);
        }
    }

    /// <summary>
    /// 充换电策略
    /// </summary>
    public class ChargeExTactics
    {
        /// <summary>
        /// SOC低于此值自动充电
        /// </summary>
        [XmlAttribute]
        public float StartSoc
        {
            get;
            set;
        }
        [XmlAttribute]
        public float StopSoc
        {
            get;
            set;
        }
        /// <summary>
        /// 充电电流限制类型 A KW C
        /// </summary>
        [XmlAttribute]
        public string DefaultApType
        {
            get;
            set;
        }
        /// <summary>
        /// 充电电流限制
        /// </summary>
        [XmlAttribute]
        public int DefaultAp
        {
            get;
            set;
        }

        [XmlAttribute]
        public float SocForEx
        {
            get;
            set;
        }
        [XmlAttribute]
        /// <summary>
        /// 对外声明可用电池最低SOC
        /// </summary>

        public float IdentEnabelSoc
        {
            get;
            set;
        }
        /// <summary>
        /// 电池选择策略 1 高SOC 2 充电完成时间 3 用户预约
        /// </summary>
        [XmlAttribute]
        public int SocPrior
        {
            get;
            set;
        }

        public ChargeExTactics()
        {
            this.StartSoc = 90;
            this.StopSoc = 98;
            this.DefaultAp = 400;//0.1
            this.SocForEx = 95;
            this.SocPrior = 1;
            this.IdentEnabelSoc = 95;
        }
        /// <summary>
        /// 是合法配置吗,0.01A单位
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            if (StartSoc < 50) this.StartSoc = 50;
            if (StopSoc < 90) this.StopSoc = 90;
            if (this.DefaultAp < 100) this.DefaultAp = 100;
            if (this.SocForEx < 30) this.SocForEx = 30;
            return true;
        }

    }
    public class RecordTactics
    {
        [XmlAttribute]
        public int ChargerDetailInterval
        {
            get;
            set;
        }
        /// <summary>
        /// 站内数据实录间隔
        /// </summary>
        [XmlAttribute]
        public int StationRawDataRecInterval
        {
            get;
            set;
        }
        /// <summary>
        /// 环境温度记录间隔
        /// </summary>
        [XmlAttribute]
        public int EnvirRecInterval
        {
            get;
            set;
        }
        /// <summary>
        /// 站内电表记录间隔
        /// </summary>
        [XmlAttribute]
        public int WattRecInterval
        {
            get;
            set;
        }
        /// <summary>
        /// 远程仓位状态记录间隔
        /// </summary>
        [XmlAttribute]
        public int RemoteStateInterval
        {
            get;
            set;
        }
        /// <summary>
        /// 远程环境温度记录间隔
        /// </summary>
        [XmlAttribute]
        public int RemoteEnvirInterval
        {
            get;
            set;
        }
        /// <summary>
        /// 远程单体温度电压明细记录间隔
        /// </summary>
        [XmlAttribute]
        public int RemoteVTRecInterval
        {
            get;
            set;
        }
        public RecordTactics()
        {
            this.ChargerDetailInterval = 8000; this.StationRawDataRecInterval = 8000;
            this.EnvirRecInterval = 60000; this.WattRecInterval = 60000; this.RemoteStateInterval = 60000; this.RemoteVTRecInterval = 300000;
            this.RemoteEnvirInterval = 300000;
        }
        /// <summary>
        /// 是合法配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            if (this.ChargerDetailInterval < 2000) this.ChargerDetailInterval = 2000;
            if (this.StationRawDataRecInterval < 5000) this.StationRawDataRecInterval = 5000;
            if (this.EnvirRecInterval < 5000) this.EnvirRecInterval = 5000;
            if (this.WattRecInterval < 10000) this.WattRecInterval = 10000;
            if (this.RemoteStateInterval < 2000) this.RemoteStateInterval = 2000;
            if (this.RemoteEnvirInterval < 5000) this.RemoteEnvirInterval = 5000;
            if (this.RemoteVTRecInterval < 5000) this.RemoteVTRecInterval = 5000;
            return true;
        }
    }
    public class DataServerConfig
    {
        [XmlAttribute]
        public string DataServerName
        {
            get;
            set;
        }
        [XmlAttribute]
        public string DataBaseName
        {
            get;
            set;
        }
        [XmlAttribute]
        public string UserName
        {
            get;
            set;
        }
        [XmlAttribute]
        public string PassWord
        {
            get;
            set;
        }
        public DataServerConfig()
        {
            this.DataServerName = "localhost";
            this.DataBaseName = "test";
            this.UserName = "user";
            this.PassWord = "password";
        }
        /// <summary>
        /// 是合法配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (this.DataServerName.ToLower() != "localhost".ToLower() && IPAddress.TryParse(this.DataServerName, out ip) == false) return false;
            if (this.DataBaseName.Length < 1 || this.UserName.Length < 1 || this.PassWord.Length < 1) return false;
            return true;
        }
    }
    public class RemoteServerConfig
    {
        /// <summary>
        /// 站编号0X421
        /// </summary>
        [XmlAttribute]
        public string RemoteCode
        {
            get;
            set;
        }

        /// <summary>
        /// 站登陆账号
        /// </summary>
        [XmlAttribute]
        public string RemoteLogoAccount
        {
            get;
            set;
        }

        /// <summary>
        /// 站登陆密码
        /// </summary>
        [XmlAttribute]
        public string RemoteLogoKey
        {
            get;
            set;
        }

        /// <summary>
        /// 远程加密密钥
        /// </summary>
        [XmlAttribute]
        public string RemoteEnryKey
        {
            get;
            set;
        }
        /// <summary>
        /// 远程服务域名
        /// </summary>
        [XmlAttribute]
        public string RemoteDomiName
        {
            get;
            set;
        }
        /// <summary>
        /// 远程默认IP
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 远程网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        [XmlAttribute]
        public bool RemoteEnable
        {
            get;
            set;
        }
        /// <summary>
        /// 删除发送成功的数据
        /// </summary>
        [XmlAttribute]
        public bool DelSucData
        {
            get;
            set;
        }
        /// <summary>
        /// 远程验证超时
        /// </summary>
        [XmlAttribute]
        public int RemoteIdentTimeOut
        {
            get;
            set;
        }
        public RemoteServerConfig()
        {
            this.RemoteCode = "0x000";
            this.RemoteDomiName = "sync.aulton.com";
            this.IP = "47.93.2.150";
            this.Port = 8088;
            this.RemoteLogoKey = "PassWord";
            this.RemoteEnryKey = "Key";
            this.RemoteIdentTimeOut = 180000;
        }
        /// <summary>
        /// 是合法配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            if (this.RemoteCode.Length == 0 || this.RemoteLogoKey.Length == 0 || this.RemoteEnryKey.Length == 0 || this.RemoteDomiName.Length == 0 || this.IP.Length == 0) return false;
            if (this.RemoteIdentTimeOut < 5) this.RemoteIdentTimeOut = 5;
            IPAddress ip = null;
            if (IPAddress.TryParse(this.IP, out ip) == false) return false;
            return true;

        }
    }
    /// <summary>
    /// 站概况
    /// </summary>
    public class NormalConfig
    {
        /// <summary>
        /// 换电站名称
        /// </summary>
        [XmlAttribute]
        public string StationName
        {
            get;
            set;
        }
        /// <summary>
        /// 换点站编号，SN，流水
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 站型号
        /// </summary>
        [XmlAttribute]
        public string stationModel
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商编号   
        /// </summary>
        [XmlAttribute]
        public string OperatorNo
        {
            get;
            set;
        }
        /// <summary>
        /// 中转仓数 read    
        /// </summary>
        [XmlAttribute]
        public int TransitNum
        {
            get;
            set;
        }
        /// <summary>
        /// scada版本号   
        /// </summary>
        [XmlAttribute]
        public string scadaVer
        {
            get;
            set;
        }
        /// <summary>
        ///蓝牙设备地址    
        /// </summary>
        [XmlAttribute]
        public string BluetoothIP
        {
            get;
            set;
        }
        /// <summary>
        ///有效授权时间          
        /// </summary>
        [XmlAttribute]
        public string licenseTime
        {
            get;
            set;
        }
        /// <summary>
        ///用户验证模式 1远程 2 本地 3 不验证       
        /// </summary>
        [XmlAttribute]
        public int ValidType
        {
            get;
            set;
        }
        /// <summary>
        ///充电机开始地址        
        /// </summary>
        [XmlAttribute]
        public string ChargerNetStartIP
        {
            get;
            set;
        }
        /// <summary>
        ///电流互感器变比        
        /// </summary>
        [XmlAttribute]
        public string WattNetRatio
        {
            get;
            set;
        }
        /// <summary>
        ///总电表开始地址        
        /// </summary>
        [XmlAttribute]
        public string WattNetStartIP
        {
            get;
            set;
        }
        /// <summary>
        ///通道数       
        /// </summary>
        [XmlAttribute]
        public int PasssGateWayNum
        {
            get;
            set;
        }
        /// <summary>
        ///充电仓数       
        /// </summary>
        [XmlAttribute]
        public int ChargerNetNum
        {
            get;
            set;
        }
        /// <summary>
        ///充电模式 1 自动 2 手动充电（默认）    
        /// </summary>
        [XmlAttribute]
        public int ChargeMode
        {
            get;
            set;
        }
        /// <summary>
        ///总电表数量       
        /// </summary>
        [XmlAttribute]
        public int WattNetNum
        {
            get;
            set;
        }

        [XmlAttribute]
        public int Per1
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Per2
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Per3
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Per4
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Per5
        {
            get;
            set;
        }
        [XmlAttribute]
        public int Per6
        {
            get;
            set;
        }


        /// <summary>
        /// 进行充电机传感器异常检测吗
        /// </summary>
        [XmlAttribute]
        public bool ChargerSensorErrCK
        {
            get;
            set;
        }
        /// <summary>
        /// 本地数据库配置
        /// </summary>
        public DataServerConfig DataServerConfig
        {
            get;
            set;
        }
        /// <summary>
        /// 远程通讯配置
        /// </summary>
        public RemoteServerConfig RemoteConfig
        {
            get;
            set;
        }
        /// <summary>
        /// 换电站外观大小
        /// </summary>
        public InterFace InterFace
        {
            get;
            set;
        }
        /// <summary>
        /// 充换电策略
        /// </summary>
        public ChargeExTactics ChargeExTactics
        {
            get;
            set;
        }
        /// <summary>
        /// 记录策略
        /// </summary>
        public RecordTactics RecordTactics
        {
            get;
            set;
        }



        public NormalConfig()
        {
            this.DataServerConfig = new DataServerConfig();  //Basic.站概况.数据库
            this.RemoteConfig = new RemoteServerConfig();    //Basic.站概况.平台
            this.ChargeExTactics = new ChargeExTactics();    //运营.充电策略
            this.RecordTactics = new RecordTactics();        //Basic.本地记录  缺用户验证模式
            this.InterFace = new InterFace(0, 0, 1028, 880);
            this.StationName = "未名换电站";
        }
        public bool IsValidConfig()
        {
            if (this.DataServerConfig.IsValidConfig() == false || this.RemoteConfig.IsValidConfig() == false || this.ChargeExTactics.IsValidConfig() == false || this.RecordTactics.IsValidConfig() == false) return false;
            if (this.StationName == null || this.StationName.Length == 0) return false;
            return true;
        }
    }
    public class ChanelConfig
    {
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }
        [XmlAttribute]
        public bool Enable
        {
            get;
            set;
        }
        [XmlAttribute]
        public short WarningValue
        {
            get;
            set;
        }
        //探测器编号
        [XmlAttribute]
        public short SensorNumber
        {
            get;
            set;
        }
        //设备类型 1 换电通道 2 监控室 3 充电机仓位
        [XmlAttribute]
        public short DeviceType
        {
            get;
            set;
        }
        //设备编号  充电机：0-100 换电通道1000- 监控室2000-
        [XmlAttribute]
        public short DeviceNumber
        {
            get;
            set;
        }

        public ChanelConfig()
        {
        }
        public ChanelConfig(int id, string text, bool enable, short warningvalue, short sensornumber)
        {
            this.ID = id; this.Text = text; this.Enable = enable; this.WarningValue = warningvalue; this.SensorNumber = sensornumber;
        }
        /// <summary>
        /// 是合法配置吗
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            if (this.Text.Length < 1) return false;
            return true;//待续
        }

    }
    public class EnvirTemperDataConfig
    {
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }

        [XmlArray("ChanelConfigs")]
        public List<ChanelConfig> ChanelConfigs
        {
            get;
            set;
        }
        public EnvirTemperDataConfig()
        {
            this.ChanelConfigs = new List<ChanelConfig>();
        }
        public EnvirTemperDataConfig(int id, string text, int chanelnum)
        {
            this.ID = id;
            this.Text = text;
            this.ChanelConfigs = new List<ChanelConfig>();
            for (int x = 0; x < chanelnum; x++)
            {
                ChanelConfig Temp = new ChanelConfig(x, string.Format("通道{0}", x), false, 600, 14);
                this.ChanelConfigs.Add(Temp);
            }
        }
        public bool IsValidConfig()
        {
            for (int x = 0; x < this.ChanelConfigs.Count; x++)
            {
                if (this.ChanelConfigs[x].IsValidConfig() == false) return false;
                if (this.ChanelConfigs[x].ID != x) return false;
            }
            return true;
        }
    }
    public class EnvirTemperConfig
    {

        /// <summary>
        /// 环境温度通讯网络IP地址
        /// </summary>
        [XmlAttribute]
        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 报警阀值
        /// </summary>
        [XmlAttribute]
        public string EnvirSensorNum
        {
            get;
            set;
        }

        /// <summary>
        /// 环境温度网络通讯端口
        /// </summary>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }
        /// <summary>
        /// 环境温度网络ID
        /// </summary>
        [XmlAttribute]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 网络名称
        /// </summary>
        [XmlAttribute]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 是否禁用此项检测
        /// </summary>
        [XmlAttribute]
        public bool Enable
        {
            get;
            set;
        }
        public EnvirTemperDataConfig DataConfig
        {
            get;
            set;
        }


        public EnvirTemperConfig(int id, string text, string ip, int port, int chanelnum) : this()
        {
            this.IP = ip; this.Port = port; this.Text = text; this.ID = id;
            this.DataConfig = new EnvirTemperDataConfig(1, "环境温度检测数据集", chanelnum);
        }
        public EnvirTemperConfig()
        {
            this.DataConfig = new EnvirTemperDataConfig(1, "环境温度检测数据集", 40);
        }
        public bool IsValidConfig()
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(this.IP, out ip)) return false;
            if (this.Port < 1 || this.ID < 0 || this.Text.Length < 1) return false;
            return this.DataConfig.IsValidConfig();
        }

    }



    public class StationConfig
    {
        /// <summary>
        /// 换电站通用配置
        /// </summary>
        public NormalConfig NomalConfig
        {
            get;
            set;
        }
        /// <summary>
        /// 充电柜配置，每个充电柜配置若干台充电机
        /// </summary>
        [XmlArray("ChargerCabinetConfig")]
        public List<ChargerNetConfig> ChargerCabinetConfig
        {
            get;
            set;
        }
        /// <summary>
        /// 充电机电度表网络配置
        /// </summary>
        [XmlArray("WattMeterNetConfig")]
        public List<WattMeterNetConfig> ChargerWattCabinetConfig
        {
            get;
            set;
        }
        public ChargeUnitConfigs ChargeUnigConfigs
        {
            get;
            set;
        }
        [XmlArray("ObjectRelations")]
        public List<Relation> ObjectRelations
        {
            get;
            set;
        }
        [XmlArray("WattMeterOffsets")]
        public List<WattMeterOffSet> MeterOffSets
        {
            get;
            set;
        }
        public MachineConfig MachineConfig
        {
            get;
            set;
        }
        /// <summary>
        /// 通道配置列表
        /// </summary>
        [XmlArray("PassGateWayConfigs")]
        public List<PassGateWayConfig> PasssGateWayConfigs
        {
            get;
            set;
        }
        public WattMeterNetConfig StationWatterConfig
        {
            get;
            set;
        }
        [XmlArray("StationWattMeterOffsets")]
        public List<WattMeterOffSet> StationWattOffSets
        {
            get;
            set;
        }

        public EnvirTemperConfig EnvirTemperConfig
        {
            get;
            set;
        }

        public StationConfig()
        {

            this.NomalConfig = new NormalConfig();                           //站概况

            this.ChargerCabinetConfig = new List<ChargerNetConfig>();        //充电机
            this.ChargerWattCabinetConfig = new List<WattMeterNetConfig>();   //总电表
            this.StationWatterConfig = new WattMeterNetConfig();              //总电表
            this.MeterOffSets = new List<WattMeterOffSet>();                     //电表当前值偏置??
            this.StationWattOffSets = new List<WattMeterOffSet>();               //站电表当前值偏置??

            this.MachineConfig = new MachineConfig();                         //换电设备
            this.EnvirTemperConfig = new EnvirTemperConfig();                  //消防报警阀值  环境温度 （消防主机通讯）
            this.PasssGateWayConfigs = new List<PassGateWayConfig>();         //通道设备


            this.ChargeUnigConfigs = new ChargeUnitConfigs();                  //---充电单元
            this.ObjectRelations = new List<Relation>();                      //----充电机 电表 充电单元 关系


        }



        //***********************************************************Common*****************************************************************

        //默认出厂配置
        public void MakeStandTempHoriStation()
        {
            #region//充电机柜配置
            ChargerNetConfig Temp = new ChargerNetConfig("192.168.1.111", 4001, 7, 1, "1号充电柜", 1, 0);
            this.ChargerCabinetConfig.Add(Temp);
            Temp = new ChargerNetConfig("192.168.1.111", 4003, 7, 2, "2号充电柜", 1, 7);
            this.ChargerCabinetConfig.Add(Temp);
            Temp = new ChargerNetConfig("192.168.1.112", 4003, 7, 3, "3号充电柜", 1, 14);
            this.ChargerCabinetConfig.Add(Temp);
            Temp = new ChargerNetConfig("192.168.1.112", 4001, 7, 4, "4号充电柜", 1, 21);
            this.ChargerCabinetConfig.Add(Temp);
            #endregion
            #region//电表柜
            WattMeterNetConfig TempWatt = new WattMeterNetConfig("192.168.1.115", 4000, 14, 10, "1号计量屏", 1, 0, 10, 0);
            this.ChargerWattCabinetConfig.Add(TempWatt);

            TempWatt = new WattMeterNetConfig("192.168.1.115", 4001, 14, 11, "2号计量屏", 15, 14, 10, 0);//2号计量屏，14块电表，起始地址1，互感器10/1
            this.ChargerWattCabinetConfig.Add(TempWatt);
            this.StationWatterConfig = new WattMeterNetConfig("192.168.1.116", 4001, 2, 21, "总电表计量屏", 200, 200, 160, 0);//总表网络，互感器160/1
            for (int x = 0; x < this.StationWatterConfig.WattMeterCount; x++)
            {
                WattMeterOffSet tempoffset = new WattMeterOffSet(this.StationWatterConfig.StartWattID + x, 0, 0, 0, 0, 0, 0);
                this.StationWattOffSets.Add(tempoffset);
            }
            #endregion
            #region//仓位界面配置并添加关系

            for (int x = 0; x < 28; x++)
            {
                this.ObjectRelations.Add(new Relation(x, x, x));
                this.ChargeUnigConfigs.Configs.Add(new UnitConfig(x, string.Format("仓位{0}", x), new InterFace(0, 0, 255, 83)));
                this.MeterOffSets.Add(new WattMeterOffSet(x, 0, 0, 0, 0, 0, 0));
            }



            int left = 771;
            int top = 540;
            int UnitId = 0;
            for (int x = 0; x < 4; x++)
            {
                top = 540 + 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            left = 514; top = 540;
            for (int x = 0; x < 4; x++)
            {
                top = 540 + 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            left = 257; top = 540;
            for (int x = 0; x < 4; x++)
            {
                top = 540 + 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            left = 0; top = 540;
            for (int x = 0; x < 4; x++)
            {
                top = 540 + 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            left = 771; top = 355;
            for (int x = 0; x < 4; x++)
            {
                top = 355 - 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            left = 514; top = 355;
            for (int x = 0; x < 4; x++)
            {
                top = 355 - 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            left = 0; top = 355;
            for (int x = 0; x < 4; x++)
            {
                top = 355 - 85 * x;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Left = left;
                this.ChargeUnigConfigs.Configs[UnitId].InterFace.Top = top;
                UnitId++;
            }
            #endregion
            #region//通道配置
            this.PasssGateWayConfigs.Clear();
            TempUnitConfig newTemp = new TempUnitConfig("192.168.1.111", 4002, "新电池暂存仓网络", 200, 200, "新电池暂存仓", new InterFace(257, 270, 255, 83));
            TempUnitConfig oldTemp = new TempUnitConfig("192.168.1.111", 4004, "旧电池暂存仓网络", 201, 201, "旧电池暂存仓", new InterFace(257, 185, 255, 83));
            BusIdentityConfig TBus = new BusIdentityConfig(30, "192.168.1.1", 9009, "车牌识别通讯网络", 1, new InterFace(0, 0, 1028, 100));
            RfidConfig trfid = new RfidConfig("192.168.1.130", 5000, "用户身份识别网络", 40);
            PassGateScreenConfig screenconfig = new PassGateScreenConfig("192.168.1.131", 5001, "通道显示器网络", 50);
            PassGateWayConfig twayconfig = new PassGateWayConfig("192.168.1.111", newTemp, oldTemp, TBus, trfid, screenconfig, 0);
            // twayconfig.InterFace = new InterFace(0, 0, 1028, 100);
            this.PasssGateWayConfigs.Add(twayconfig);
            #endregion
            #region//设备配置
            this.MachineConfig = new MachineConfig("192.168.1.10", 4096, 31, 1, "快换设备", new InterFace(0, 441, 1028, 96), true);
            this.MachineConfig.MaDuoSize = new Size(253, 96);
            #endregion
            this.EnvirTemperConfig = new EnvirTemperConfig(35, "环境温度监测网络", "192.168.1.200", 5000, 40);
            this.EnvirTemperConfig.Enable = false;
        }

        public bool IsValidConfig()
        {
            string temp = null;
            bool ret = this.IsValidConfig(out temp);
            return ret;
        }

        /// <summary>
        /// 是合法的配置吗，如果不是返回原因
        /// </summary>
        /// <param name="errdes"></param>
        /// <returns></returns>
        public bool IsValidConfig(out string errdes)
        {
            try
            {

                errdes = "配置合法 ";
                if (this.ChargerCabinetConfig == null || this.ChargerCabinetConfig.Count == 0)
                {
                    errdes = "充电机网络配置错误";
                    return false;

                }
                for (int x = 0; x < this.ChargerCabinetConfig.Count; x++)
                {
                    if (this.ChargerCabinetConfig[x].IsValidConfig() == false)
                    {
                        errdes = string.Format("充电机网络{0}配置错误！", x);
                        return false;
                    }
                }
                if (this.ChargerWattCabinetConfig == null || this.ChargerWattCabinetConfig.Count == 0)
                {
                    errdes = "电表网络配置错误";
                    return false;
                }
                for (int x = 0; x < this.ChargerWattCabinetConfig.Count; x++)
                {
                    if (this.ChargerWattCabinetConfig[x].IsValidConfig() == false)
                    {
                        errdes = string.Format("电表网络{0}配置错误！", x);
                        return false;
                    }
                }
                if (this.ChargeUnigConfigs == null || this.ChargeUnigConfigs.IsValidConfig() == false)
                {
                    errdes = "充电仓位配置错误！";
                    return false;
                }
                if (this.MachineConfig == null || this.MachineConfig.IsValidConfig() == false)
                {
                    errdes = "设备配置错误！";
                    return false;
                }
                if (this.PasssGateWayConfigs == null || this.PasssGateWayConfigs.Count == 0)
                {
                    errdes = "通道配置错误！";
                    return false;
                }
                for (int x = 0; x < this.PasssGateWayConfigs.Count; x++)
                {
                    this.PasssGateWayConfigs[x].ID = x;
                    // this.PasssGateWayConfigs[x].newUnitConfig.Unit.ID = 200 + x * 2;
                    // this.PasssGateWayConfigs[x].oldUnitConfig.Unit.ID = 201 + x * 2;
                    if (this.PasssGateWayConfigs[x].IsValidConfig() == false)
                    {
                        errdes = string.Format("通道{0}配置错误！", x);
                        return false;
                    }

                }
                if (this.StationWatterConfig.IsValidConfig() == false)
                {
                    errdes = "总电表网络配置错误！";
                    return false;
                }
                if (this.EnvirTemperConfig.IsValidConfig() == false)
                {
                    errdes = "环境温度检测通讯网络配置错误！";
                    return false;
                }
                if (this.NomalConfig.IsValidConfig() == false)
                {
                    errdes = "本地数据库配置或者远程通讯配置错误！";
                    return false;
                }
                if (this.CommParaIsInvalid(ref errdes) == false) return false;
                if (this.ChargerCabinetConfig[0].StartChargerID != 0)
                {
                    errdes = "起始充电机编号必须为0！";
                    return false;
                }
                for (int x = 0; x < this.ChargerCabinetConfig.Count; x++)
                {
                    if (x + 1 < this.ChargerCabinetConfig.Count)
                    {
                        if (this.ChargerCabinetConfig[x + 1].StartChargerID != this.ChargerCabinetConfig[x].StartChargerID + this.ChargerCabinetConfig[x].ChargerCount)
                        {
                            errdes = "所有充电机网络中的充电机编号必须连续";
                            return false;
                        }
                    }
                }
                if (this.ChargerWattCabinetConfig[0].StartWattID != 0)
                {
                    errdes = "起始电度表编号必须为0！";
                    return false;
                }
                for (int x = 0; x < this.ChargerWattCabinetConfig.Count; x++)
                {
                    if (x + 1 < this.ChargerWattCabinetConfig.Count)
                    {
                        if (this.ChargerWattCabinetConfig[x + 1].StartWattID != this.ChargerWattCabinetConfig[x].StartWattID + this.ChargerWattCabinetConfig[x].WattMeterCount)
                        {
                            errdes = "所有网络中的电度表编号必须连续";
                            return false;
                        }
                    }
                }
                int MaxChargerid = this.ChargerCabinetConfig[this.ChargerCabinetConfig.Count - 1].StartChargerID + this.ChargerCabinetConfig[this.ChargerCabinetConfig.Count - 1].ChargerCount;
                int MaxWatterid = this.ChargerWattCabinetConfig[this.ChargerWattCabinetConfig.Count - 1].StartWattID + this.ChargerWattCabinetConfig[this.ChargerWattCabinetConfig.Count - 1].WattMeterCount;
                if (MaxChargerid != MaxWatterid || MaxWatterid != this.ChargeUnigConfigs.UnitCount)
                {
                    errdes = "充电机数量，电度表数量，充电仓位数量必须相等！";
                    return false;
                }
                if (this.ObjectRelations.Count != MaxChargerid)
                {
                    errdes = "对象配置表数量必须等于仓位数量";
                    return false;
                }
                for (int x = 0; x < this.ObjectRelations.Count; x++)
                {
                    if (this.ObjectRelations[x].ChargerID < 0 || this.ObjectRelations[x].ChargerID > MaxChargerid || this.ObjectRelations[x].WattID < 0 || this.ObjectRelations[x].WattID > MaxWatterid
                        || this.ObjectRelations[x].UnitID < 0 || this.ObjectRelations[x].UnitID > MaxWatterid)
                    {
                        errdes = "对象配置表中的对象ID必须存在且有效！";
                        return false;
                    }
                    for (int y = x + 1; y < this.ObjectRelations.Count; y++)
                    {
                        if ((this.ObjectRelations[x].UnitID == this.ObjectRelations[y].UnitID) || (this.ObjectRelations[x].ChargerID == this.ObjectRelations[y].ChargerID) || (this.ObjectRelations[x].WattID == this.ObjectRelations[y].WattID))
                        {
                            errdes = "对象配置表中的对象ID不能重复！";
                            return false;
                        }

                    }
                }
                if (this.MeterOffSets.Count != MaxWatterid)
                {
                    errdes = "电度表偏置配置表的数量必须等于电度表的数量";
                    return false;
                }
                int minmeterid = 500;
                int maxmeterid = -1;
                for (int x = 0; x < this.MeterOffSets.Count; x++)
                {
                    WattMeterOffSet tempoffset = this.MeterOffSets[x];
                    if (tempoffset.ID < minmeterid)
                        minmeterid = tempoffset.ID;
                    if (tempoffset.ID > maxmeterid)
                        maxmeterid = tempoffset.ID;
                    for (int y = x + 1; y < this.MeterOffSets.Count; y++)
                    {
                        if (tempoffset.ID == this.MeterOffSets[y].ID)
                        {
                            errdes = "电度表偏置配置表中电度表编号不能重复！";
                            return false;
                        }
                    }
                }
                if (minmeterid != 0 || maxmeterid != MaxWatterid - 1)
                {
                    errdes = "电度表偏置配置表中充电机编号必须在充电机编号列表中！";
                    return false;
                }
                if (this.StationWatterConfig.WattMeterCount != this.StationWattOffSets.Count)
                {
                    errdes = "站总电表数量必须与站总电表偏置列表数量相等！";
                    return false;
                }
                int minstationwattid = this.StationWatterConfig.StartWattID;
                int maxstationwattid = minstationwattid + this.StationWatterConfig.WattMeterCount;
                Dictionary<int, int> temp = new Dictionary<int, int>();
                for (int x = 0; x < this.StationWattOffSets.Count; x++)
                {
                    WattMeterOffSet toffset = this.StationWattOffSets[x];
                    if (toffset.ID < minstationwattid || toffset.ID > maxstationwattid)
                    {
                        errdes = "站总电表偏置表中的ID必须在占总表配置表中！";
                        return false;
                    }
                    for (int y = x + 1; y < this.StationWattOffSets.Count; y++)
                    {
                        if (toffset.ID == this.StationWattOffSets[y].ID)
                        {
                            errdes = "站总电表偏置表中的ID不能重复！";
                            return false;
                        }
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                errdes = "配置异常 ";
                return false;
            }


        }
        private bool CommParaIsInvalid(ref string errdes)
        {
            try
            {

                NetParas Paras = new NetParas();
                NetPara t;
                for (int x = 0; x < this.ChargerCabinetConfig.Count; x++)
                {
                    t = new NetPara(this.ChargerCabinetConfig[x].IP, this.ChargerCabinetConfig[x].Port, this.ChargerCabinetConfig[x].ID);
                    if (Paras.AddNetPara(t, ref errdes) == false) return false;
                }
                for (int x = 0; x < this.ChargerWattCabinetConfig.Count; x++)
                {
                    t = new NetPara(this.ChargerWattCabinetConfig[x].IP, this.ChargerWattCabinetConfig[x].Port, this.ChargerWattCabinetConfig[x].ID);
                    if (Paras.AddNetPara(t, ref errdes) == false) return false;
                }
                t = new NetPara(this.MachineConfig.IP, this.MachineConfig.Port, this.MachineConfig.NetID);
                if (Paras.AddNetPara(t, ref errdes) == false) return false;
                t = new NetPara(this.StationWatterConfig.IP, this.StationWatterConfig.Port, this.StationWatterConfig.ID);
                if (Paras.AddNetPara(t, ref errdes) == false) return false;
                t = new NetPara(this.EnvirTemperConfig.IP, this.EnvirTemperConfig.Port, this.EnvirTemperConfig.ID);
                if (Paras.AddNetPara(t, ref errdes) == false) return false;

                for (int x = 0; x < this.PasssGateWayConfigs.Count; x++)
                {
                    //t = new NetPara(this.PasssGateWayConfigs[x].newUnitConfig.IP, this.PasssGateWayConfigs[x].newUnitConfig.Port, this.PasssGateWayConfigs[x].newUnitConfig.NetID);
                    //if (Paras.AddNetPara(t, ref errdes) == false) return false;

                    //t = new NetPara(this.PasssGateWayConfigs[x].oldUnitConfig.IP, this.PasssGateWayConfigs[x].oldUnitConfig.Port, this.PasssGateWayConfigs[x].oldUnitConfig.NetID);
                    //if (Paras.AddNetPara(t, ref errdes) == false) return false;

                    t = new NetPara(this.PasssGateWayConfigs[x].busIdentConfig.IP, this.PasssGateWayConfigs[x].busIdentConfig.Port, this.PasssGateWayConfigs[x].busIdentConfig.ID);
                    if (Paras.AddNetPara(t, ref errdes) == false) return false;

                    t = new NetPara(this.PasssGateWayConfigs[x].CustomerIDGeter.IP, this.PasssGateWayConfigs[x].CustomerIDGeter.Port, this.PasssGateWayConfigs[x].CustomerIDGeter.ID);
                    if (Paras.AddNetPara(t, ref errdes) == false) return false;

                    t = new NetPara(this.PasssGateWayConfigs[x].ScreenConfig.IP, this.PasssGateWayConfigs[x].ScreenConfig.Port, this.PasssGateWayConfigs[x].ScreenConfig.ID);
                    if (Paras.AddNetPara(t, ref errdes) == false) return false;


                }
                return true;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return false;
            }

        }

        public int GetChargerCount()
        {
            try
            {


                int ret = 0;
                for (int x = 0; x < this.ChargerCabinetConfig.Count; x++)
                    ret += this.ChargerCabinetConfig[x].ChargerCount;
                return ret;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return 0;

            }
        }
        public int GetChargerWattCount()
        {
            try
            {


                int ret = 0;
                for (int x = 0; x < this.ChargerWattCabinetConfig.Count; x++)
                    ret += this.ChargerWattCabinetConfig[x].WattMeterCount;
                return ret;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return 0;

            }
        }
        public bool IsChargerNetValid(bool retdes, out string des)
        {
            try
            {


                des = null;
                if (this.ChargerCabinetConfig == null || this.ChargerCabinetConfig.Count == 0)
                {
                    if (retdes)
                        des = "充电机网络不能为空！";
                    return false;
                }
                if (this.ChargerCabinetConfig[0].StartChargerID != 0)
                {
                    if (retdes)
                        des = "起始充电机编号必须为零";
                    return false;
                }
                for (int x = 1; x < this.ChargerCabinetConfig.Count; x++)
                {
                    if (this.ChargerCabinetConfig[x].StartChargerID != this.ChargerCabinetConfig[x - 1].StartChargerID + this.ChargerCabinetConfig[x - 1].ChargerCount)
                    {
                        if (retdes)
                            des = "充电机编号必须连续";
                        return false;
                    }
                    if (this.ChargerCabinetConfig[x].ID == this.ChargerCabinetConfig[x - 1].ID)
                    {
                        if (retdes)
                            des = "充电机网络编号必须连续";
                        return false;
                    }
                }
                for (int x = 0; x < this.ChargerCabinetConfig.Count; x++)
                {
                    for (int y = x + 1; y < this.ChargerCabinetConfig.Count; y++)
                    {
                        if (this.ChargerCabinetConfig[x].IP == this.ChargerCabinetConfig[y].IP && this.ChargerCabinetConfig[x].Port == this.ChargerCabinetConfig[y].Port)
                        {
                            if (retdes)
                                des = "网络中的IP地址和端口号的组合不能重复！";
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                des = "充电机网络配置异常";
                return false;

            }
        }

        public bool IsChargerWattNetConfigIsValid(bool retdes, out string des)
        {
            try
            {


                des = null;
                if (this.ChargerWattCabinetConfig == null || this.ChargerWattCabinetConfig.Count == 0)
                {
                    if (retdes)
                        des = "电度表网络不能为空！";
                    return false;
                }
                if (this.ChargerWattCabinetConfig[0].StartWattID != 0)
                {
                    if (retdes)
                        des = "起始电度表必须为零";
                    return false;
                }
                for (int x = 1; x < this.ChargerWattCabinetConfig.Count; x++)
                {
                    if (this.ChargerWattCabinetConfig[x].StartWattID != this.ChargerWattCabinetConfig[x - 1].StartWattID + this.ChargerWattCabinetConfig[x - 1].WattMeterCount)
                    {
                        if (retdes)
                            des = "电度表编号必须连续";
                        return false;
                    }
                }
                for (int x = 0; x < this.ChargerWattCabinetConfig.Count; x++)
                {
                    for (int y = x + 1; y < this.ChargerWattCabinetConfig.Count; y++)
                    {
                        if (this.ChargerWattCabinetConfig[x].IP == this.ChargerWattCabinetConfig[y].IP && this.ChargerWattCabinetConfig[x].Port == this.ChargerWattCabinetConfig[y].Port)
                        {
                            if (retdes)
                                des = "网络中的IP地址和端口号的组合不能重复！";
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                des = "电度表网络异常";
                return false;
            }
        }
        public bool IsRelationAndUnitConfigValid(bool retdes, out string des)
        {
            try
            {


                des = null;
                if (this.ObjectRelations.Count != this.ChargeUnigConfigs.UnitCount)
                {
                    if (retdes)
                        des = "仓位数量与充电机数量电表数量必须一致";
                    return false;
                }
                for (int x = 0; x < this.ObjectRelations.Count; x++)
                {
                    for (int y = x + 1; y < this.ObjectRelations.Count; y++)
                    {
                        if (this.ObjectRelations[x].ChargerID == this.ObjectRelations[y].ChargerID)
                        {
                            if (retdes)
                                des = "在关系表中，充电机编号重复";
                            return false;

                        }
                        if (this.ObjectRelations[x].WattID == this.ObjectRelations[y].WattID)
                        {
                            if (retdes)
                                des = "在关系表中，电度表编号重复";
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                des = "仓位与充电机关系异常";
                return false;

            }
        }

        #region//序列化函数
        public bool ToFile()
        {

            FileStream fs = null;
            XmlSerializer xmlser = null;
            try
            {
                fs = new FileStream("StationConfig.xml", FileMode.Create, FileAccess.Write);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xmlser = new XmlSerializer(typeof(StationConfig));
                xmlser.Serialize(fs, this, ns);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }


        }
        public static StationConfig FromFile()
        {
            StationConfig config = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream("StationConfig.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer xmlSearializer = new XmlSerializer(typeof(StationConfig));
                config = (StationConfig)xmlSearializer.Deserialize(fs);
                return config;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

        }
        #endregion

        //***********************************************************Common*****************************************************************
    }

    //***********************************************************Common*****************************************************************
    public class NetPara
    {
        public string IP;
        public int Port;
        public int ID;
        public NetPara(string ip, int port, int id)
        {
            this.IP = ip; this.Port = port; this.ID = id;
        }
    }
    public class NetParas
    {
        private List<NetPara> paras = new List<NetPara>();
        public bool AddNetPara(NetPara para, ref string errdes)
        {
            try
            {


                for (int x = 0; x < paras.Count; x++)
                {
                    if ((para.ID == this.paras[x].ID) || ((para.IP == this.paras[x].IP && para.Port == this.paras[x].Port)))
                    {
                        errdes = string.Format("存在通讯参数相同的网络！");
                        return false;
                    }
                }
                this.paras.Add(para);
                return true;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
                return false;
            }
        }
    }
}
