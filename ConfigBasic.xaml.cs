using MQTTnet.Internal;
using MySqlX.XDevAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;

namespace DESCADA
{
    /// <summary>
    /// ConfigBasic.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigBasic : UserControl
    {
        private StationConfig Config;
        private StationConfig OldConfig;
        public ConfigBasic()
        {
            InitializeComponent();
            this.OldConfig = Global.config;
            this.Config = Global.config;

            initSensor();
            init();
            acl();
        }

        private void acl()
        {
            if (Global.Role == 3 && Global.config.NomalConfig.Per1 == 0)
            {
                GridBasic.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
        }

        //初始化探测器下拉列表
        private void initSensor()
        {
            try
            {

                if (DeviceSensor0input == null) return;
                int sensorNum;
                bool sensorNumIsInt = true;
                if (this.EnvirSensorNum != null)
                    sensorNumIsInt = int.TryParse(this.EnvirSensorNum.Text, out sensorNum);
                else
                {
                    sensorNumIsInt = int.TryParse(this.Config.EnvirTemperConfig.EnvirSensorNum, out sensorNum);
                }

                if (sensorNumIsInt == false) { Global.PromptFail("探测器数量不正确！"); return; }

                List<string> lsitSensor = new List<string>();
                lsitSensor.Add("--");
                for (int j = 1; j <= sensorNum; j++)
                {
                    lsitSensor.Add("探测器" + j);
                }

                DeviceSensor8input.ItemsSource = lsitSensor;
                DeviceSensor9input.ItemsSource = lsitSensor;
                for (int i = 0; i < 8; i++)
                {
                    ComboBox device = this.FindName("DeviceSensor" + i + "input") as ComboBox;
                    device.ItemsSource = lsitSensor;
                }

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }
        private void EnvirSensorNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            initSensor();
        }


        private void init()
        {
            InValidFromConfig();

        }

        public void InValidFromConfig()
        {
            try
            {


                //****************站概况******************************
                this.StationnameText.Text = this.Config.NomalConfig.StationName; //站名称
                this.stationModel.Text = this.Config.NomalConfig.stationModel;//站型号  ending stationModel
                this.stationidinput.Text = this.Config.NomalConfig.RemoteConfig.RemoteCode;  //站编号 stationidinput 设备编号（云端读取）
                this.OperatorNo.Text = this.Config.NomalConfig.OperatorNo;   //运营商编号   OperatorNo
                this.gatewaynuminput.Text = this.Config.NomalConfig.PasssGateWayNum.ToString(); //换电通道数 read gatewaynuminput
                this.ChargerNetNumR.Text = this.Config.NomalConfig.ChargerNetNum.ToString();  //充电仓数 read ChargerNetNumR
                this.domainname.Text = this.Config.NomalConfig.RemoteConfig.RemoteDomiName;  //平台地址
                this.RemotePassInput.Password = this.Config.NomalConfig.RemoteConfig.RemoteLogoKey; //平台登录密码
                this.TransitNum.Text = this.Config.NomalConfig.TransitNum.ToString();   //中转仓数 read TransitNum
                this.scadaVer.Text = this.Config.NomalConfig.scadaVer;  //scada版本号 scadaVer
                this.BluetoothIP.Text = this.Config.NomalConfig.BluetoothIP;   //蓝牙设备地址 BluetoothIP
                this.RemotekeyInput.Text = this.Config.NomalConfig.RemoteConfig.RemoteEnryKey;
                this.licenseTime.Text = this.Config.NomalConfig.licenseTime; //有效授权时间  //licenseTime
                this.databasenameinput.Text = this.Config.NomalConfig.DataServerConfig.DataBaseName;
                this.userinput.Text = this.Config.NomalConfig.DataServerConfig.UserName;
                this.passwordinput.Password = this.Config.NomalConfig.DataServerConfig.PassWord;

                //****************充电机******************************
                this.ChargerNetNum.Text = this.Config.NomalConfig.ChargerNetNum.ToString();  //充电机数量
                this.ChargerNetStartIP.Text = this.Config.NomalConfig.ChargerNetStartIP;  //充电机开始地址  去明细 
                                                                                          //this.Config.ChargerCabinetConfig
                for (int i = 1; i < 8; i++)
                {
                    if (this.Config.ChargerCabinetConfig.Count >= i)
                    {
                        TextBox txtUniqId = (TextBox)this.FindName("ChargerUniqId" + i + "ipinput");
                        txtUniqId.Text = this.Config.ChargerCabinetConfig[i - 1].UniqId;

                        ComboBox txtOutGunNum = (ComboBox)this.FindName("GunNum" + i + "input");
                        txtOutGunNum.Text = this.Config.ChargerCabinetConfig[i - 1].OutGunNum.ToString();

                    }
                }

                //****************总电表******************************
                this.WattNetNumInput.Text = this.Config.NomalConfig.WattNetNum.ToString(); //总电表数量
                this.WattNetRatio.Text = this.Config.NomalConfig.WattNetRatio;   //电流互感器变比 列表上提 WattNetRatio
                this.WattNetStartIP.Text = this.Config.NomalConfig.WattNetStartIP;    //总电表开始地址 列表上提 WattNetStartIP

                //****************换电设备******************************
                this.machineipinput.Text = this.Config.MachineConfig.IP;

                //****************消防设备******************************
                this.envirip.Text = this.Config.EnvirTemperConfig.IP;
                this.EnvirSensorNum.Text = this.Config.EnvirTemperConfig.EnvirSensorNum; //探测器数量
                                                                                         //环境探测器 EnvirTemperDataConfig     
                for (int i = 0; i < this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs.Count; i++)
                {
                    ChanelConfig chanelConfig = this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs[i];
                    int DeviceNumber = chanelConfig.DeviceNumber;
                    int SensorNumber = chanelConfig.SensorNumber;
                    ComboBox device = this.FindName("DeviceSensor" + DeviceNumber + "input") as ComboBox;
                    if (device != null && SensorNumber != 0)
                    {
                        device.SelectedIndex = 2;
                        device.SelectedItem = "探测器" + SensorNumber;
                    }
                }
                //****************通道设备******************************
                GridLength l110 = new GridLength(110);
                GridLength l0 = new GridLength(0);
                if (this.Config.PasssGateWayConfigs.Count > 0)
                {
                    PassGateWayConfig Temp1 = this.Config.PasssGateWayConfigs[0];
                    this.gateway1ipinput.Text = Temp1.busIdentConfig.IP;
                    this.Laki1IP.Text = Temp1.LakiIP;
                    this.screen1ip.Text = Temp1.ScreenConfig.IP;
                }
                if (this.Config.PasssGateWayConfigs.Count > 1)
                {
                    PassGateWayConfig Temp2 = this.Config.PasssGateWayConfigs[1];
                    this.gateway2ipinput.Text = Temp2.busIdentConfig.IP;
                    this.Laki2IP.Text = Temp2.LakiIP;
                    this.screen2ip.Text = Temp2.ScreenConfig.IP;
                    if (Temp2.Enable == true)
                    { TblPassGateWayR1.Height = l110; }
                    else { TblPassGateWayR1.Height = l0; }
                }
                if (this.Config.PasssGateWayConfigs.Count > 2)
                {
                    PassGateWayConfig Temp3 = this.Config.PasssGateWayConfigs[2];
                    this.gateway3ipinput.Text = Temp3.busIdentConfig.IP;
                    this.Laki3IP.Text = Temp3.LakiIP;
                    this.screen3ip.Text = Temp3.ScreenConfig.IP;
                    if (Temp3.Enable == true)
                    { TblPassGateWayR2.Height = l110; }
                    else { TblPassGateWayR2.Height = l0; }
                }
                if (this.Config.PasssGateWayConfigs.Count > 3)
                {
                    PassGateWayConfig Temp4 = this.Config.PasssGateWayConfigs[3];
                    this.gateway4ipinput.Text = Temp4.busIdentConfig.IP;
                    this.Laki4IP.Text = Temp4.LakiIP;
                    this.screen4ip.Text = Temp4.ScreenConfig.IP;
                    if (Temp4.Enable == true)
                    { TblPassGateWayR3.Height = l110; }
                    else { TblPassGateWayR3.Height = l0; }
                }

                //****************本地记录******************************
                this.ChargeRecTimeSpanInput.Text = this.Config.NomalConfig.RecordTactics.ChargerDetailInterval.ToString();  /// 1000; //电池监控记录间隔
                this.EnergyRecInput.Text = this.Config.NomalConfig.RecordTactics.WattRecInterval.ToString(); /// 1000; //电表
                this.statetimerinput.Text = this.Config.NomalConfig.RecordTactics.StationRawDataRecInterval.ToString(); /// 1000; //仓位状态

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }


        public void InValidToConfig()
        {
            try
            {
                //****************站概况******************************
                this.Config.NomalConfig.StationName = this.StationnameText.Text; //站名称
                this.Config.NomalConfig.stationModel = this.stationModel.Text;//站型号  ending stationModel
                this.Config.NomalConfig.RemoteConfig.RemoteCode = this.stationidinput.Text;  //站编号 stationidinput
                this.Config.NomalConfig.OperatorNo = this.OperatorNo.Text;   //运营商编号   OperatorNo
                this.Config.NomalConfig.PasssGateWayNum = Convert.ToInt32(this.gatewaynuminput.Text); //换电通道数 read gatewaynuminput
                this.Config.NomalConfig.ChargerNetNum = Convert.ToInt32(this.ChargerNetNumR.Text);  //充电仓数 read ChargerNetNumR
                this.Config.NomalConfig.RemoteConfig.RemoteDomiName = this.domainname.Text;  //平台地址
                this.Config.NomalConfig.RemoteConfig.RemoteLogoKey = this.RemotePassInput.Password; //平台登录密码
                this.Config.NomalConfig.RemoteConfig.RemoteLogoAccount = this.RemoteLogoAccount.Text;
                this.Config.NomalConfig.TransitNum = Convert.ToInt32(this.TransitNum.Text);   //中转仓数 read TransitNum
                this.Config.NomalConfig.scadaVer = this.scadaVer.Text;  //scada版本号 scadaVer
                this.Config.NomalConfig.BluetoothIP = this.BluetoothIP.Text;   //蓝牙设备地址 BluetoothIP
                this.Config.NomalConfig.RemoteConfig.RemoteEnryKey = this.RemotekeyInput.Text;
                this.Config.NomalConfig.licenseTime = this.licenseTime.Text; //有效授权时间  //licenseTime
                this.Config.NomalConfig.DataServerConfig.DataBaseName = this.databasenameinput.Text;
                this.Config.NomalConfig.DataServerConfig.UserName = this.userinput.Text;
                this.Config.NomalConfig.DataServerConfig.PassWord = this.passwordinput.Password;

                //****************充电机******************************
                this.Config.NomalConfig.ChargerNetNum = Convert.ToInt32(this.ChargerNetNum.Text);  //充电机数量
                this.Config.NomalConfig.ChargerNetStartIP = this.ChargerNetStartIP.Text;  //充电机开始地址  去明细 
                for (int i = 1; i < 8; i++)
                {
                    if (this.Config.ChargerCabinetConfig.Count >= i)
                    {
                        TextBox txtUniqId = (TextBox)this.FindName("ChargerUniqId" + i + "ipinput");
                        this.Config.ChargerCabinetConfig[i - 1].UniqId = txtUniqId.Text;

                        ComboBox txtOutGunNum = (ComboBox)this.FindName("GunNum" + i + "input");
                        if (txtOutGunNum.Text == "")
                            this.Config.ChargerCabinetConfig[i - 1].OutGunNum = 0;
                        else
                            this.Config.ChargerCabinetConfig[i - 1].OutGunNum = Int16.Parse(txtOutGunNum.Text.ToString());

                    }
                }


                //****************总电表******************************
                this.Config.NomalConfig.WattNetNum = Convert.ToInt32(this.WattNetNumInput.Text); //总电表数量
                this.Config.NomalConfig.WattNetRatio = this.WattNetRatio.Text;   //电流互感器变比 列表上提 WattNetRatio
                this.Config.NomalConfig.WattNetStartIP = this.WattNetStartIP.Text;    //总电表开始地址 列表上提 WattNetStartIP

                //****************换电设备******************************
                this.Config.MachineConfig.IP = this.machineipinput.Text;

                //****************消防设备******************************
                this.Config.EnvirTemperConfig.IP = this.envirip.Text;
                this.Config.EnvirTemperConfig.EnvirSensorNum = this.EnvirSensorNum.Text; //探测器数量
                                                                                         //环境探测器 EnvirTemperDataConfig     
                for (int i = 0; i < this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs.Count; i++)
                {
                    ChanelConfig chanelConfig = this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs[i];
                    int DeviceNumber = chanelConfig.DeviceNumber;
                    ComboBox device = this.FindName("DeviceSensor" + DeviceNumber + "input") as ComboBox;
                    string strdevice = device.SelectedValue.ToString().Replace("探测器", "");
                    if (strdevice == "--")
                        chanelConfig.SensorNumber = 0;
                    else
                        chanelConfig.SensorNumber = Int16.Parse(strdevice);
                }

                //****************通道设备******************************
                if (this.Config.PasssGateWayConfigs.Count > 0)
                {
                    PassGateWayConfig Temp1 = this.Config.PasssGateWayConfigs[0];
                    Temp1.busIdentConfig.IP = this.gateway1ipinput.Text;
                    Temp1.ScreenConfig.IP = this.screen1ip.Text;
                    Temp1.LakiIP = this.Laki1IP.Text;
                }
                if (this.Config.PasssGateWayConfigs.Count > 1)
                {
                    PassGateWayConfig Temp2 = this.Config.PasssGateWayConfigs[1];
                    Temp2.busIdentConfig.IP = this.gateway2ipinput.Text;
                    Temp2.ScreenConfig.IP = this.screen2ip.Text;
                    Temp2.LakiIP = this.Laki2IP.Text;
                }
                if (this.Config.PasssGateWayConfigs.Count > 2)
                {
                    PassGateWayConfig Temp3 = this.Config.PasssGateWayConfigs[2];
                    Temp3.busIdentConfig.IP = this.gateway3ipinput.Text;
                    Temp3.ScreenConfig.IP = this.screen3ip.Text;
                    Temp3.LakiIP = this.Laki3IP.Text;
                }
                if (this.Config.PasssGateWayConfigs.Count > 3)
                {
                    PassGateWayConfig Temp4 = this.Config.PasssGateWayConfigs[3];
                    Temp4.busIdentConfig.IP = this.gateway4ipinput.Text;
                    Temp4.ScreenConfig.IP = this.screen4ip.Text;
                    Temp4.LakiIP = this.Laki4IP.Text;
                }

                //****************本地记录******************************
                this.Config.NomalConfig.RecordTactics.ChargerDetailInterval = Convert.ToInt32(this.ChargeRecTimeSpanInput.Text);  /// 1000=  ; //电池监控记录间隔
                this.Config.NomalConfig.RecordTactics.WattRecInterval = Convert.ToInt32(this.EnergyRecInput.Text);/// 1000=  ; //电表
                this.Config.NomalConfig.RecordTactics.StationRawDataRecInterval = Convert.ToInt32(this.statetimerinput.Text); /// 1000=  ; //仓位状态

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }


        private void btnDelTongdao1_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                GridLength l = new GridLength(0);
                TblPassGateWayR1.Height = l;
                PassGateWayConfig Temp2 = this.Config.PasssGateWayConfigs[1];
                Temp2.Enable = false;
                this.gatewaynuminput.Text = (Convert.ToInt32(this.gatewaynuminput.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }
        private void btnDelTongdao2_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                GridLength l = new GridLength(0);
                TblPassGateWayR2.Height = l;
                PassGateWayConfig Temp3 = this.Config.PasssGateWayConfigs[2];
                Temp3.Enable = false;
                this.gatewaynuminput.Text = (Convert.ToInt32(this.gatewaynuminput.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }
        private void btnDelTongdao3_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                GridLength l = new GridLength(0);
                TblPassGateWayR3.Height = l;
                PassGateWayConfig Temp4 = this.Config.PasssGateWayConfigs[3];
                Temp4.Enable = false;
                this.gatewaynuminput.Text = (Convert.ToInt32(this.gatewaynuminput.Text) - 1).ToString();
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        public void AddEvents()
        {
            try
            {
                string head = "修改基础配置";
                //****************站概况******************************
                if (this.Config.NomalConfig.StationName != this.OldConfig.NomalConfig.StationName)
                {
                    Global.AddEvent(head + "站名称" + "（" + this.Config.NomalConfig.StationName + "）", 4);
                }
                if (this.Config.NomalConfig.stationModel != this.OldConfig.NomalConfig.stationModel)
                {
                    Global.AddEvent(head + "站型号" + "（" + this.Config.NomalConfig.stationModel + "）", 4);
                }
                if (this.Config.NomalConfig.OperatorNo != this.OldConfig.NomalConfig.OperatorNo)
                {
                    Global.AddEvent(head + "运营商编号" + "（" + this.Config.NomalConfig.OperatorNo + "）", 4);
                }
                if (this.Config.NomalConfig.RemoteConfig.RemoteDomiName != this.OldConfig.NomalConfig.RemoteConfig.RemoteDomiName)
                {
                    Global.AddEvent(head + "平台地址" + "（" + this.Config.NomalConfig.RemoteConfig.RemoteDomiName + "）", 4);
                }
                if (this.Config.NomalConfig.RemoteConfig.RemoteLogoKey != this.OldConfig.NomalConfig.RemoteConfig.RemoteLogoKey)
                {
                    Global.AddEvent(head + "平台登录密码", 4);
                }
                if (this.Config.NomalConfig.RemoteConfig.RemoteLogoAccount != this.OldConfig.NomalConfig.RemoteConfig.RemoteLogoAccount)
                {
                    Global.AddEvent(head + "平台登录账号" + "（" + this.Config.NomalConfig.RemoteConfig.RemoteLogoAccount + "）", 4);
                }
                if (this.Config.NomalConfig.TransitNum != this.OldConfig.NomalConfig.TransitNum)
                {
                    Global.AddEvent(head + "中转仓数" + "（" + this.Config.NomalConfig.TransitNum + "）", 4);
                }
                if (this.Config.NomalConfig.scadaVer != this.OldConfig.NomalConfig.scadaVer)
                {
                    Global.AddEvent(head + "scada版本号" + "（" + this.Config.NomalConfig.scadaVer + "）", 4);
                }
                if (this.Config.NomalConfig.BluetoothIP != this.OldConfig.NomalConfig.BluetoothIP)
                {
                    Global.AddEvent(head + "蓝牙设备地址" + "（" + this.Config.NomalConfig.BluetoothIP + "）", 4);
                }
                if (this.Config.NomalConfig.RemoteConfig.RemoteEnryKey != this.OldConfig.NomalConfig.RemoteConfig.RemoteEnryKey)
                {
                    Global.AddEvent(head + "消息秘钥" + "（" + this.Config.NomalConfig.RemoteConfig.RemoteEnryKey + "）", 4);
                }

                //****************充电机******************************
                if (this.Config.NomalConfig.ChargerNetNum != this.OldConfig.NomalConfig.ChargerNetNum)
                {
                    Global.AddEvent(head + "充电机数量" + "（" + this.Config.NomalConfig.ChargerNetNum + "）", 4);
                }
                if (this.Config.NomalConfig.ChargerNetStartIP != this.OldConfig.NomalConfig.ChargerNetStartIP)
                {
                    Global.AddEvent(head + "充电机开始地址" + "（" + this.Config.NomalConfig.ChargerNetStartIP + "）", 4);
                }
                for (int i = 1; i < 8; i++)
                {
                    if (this.Config.ChargerCabinetConfig.Count >= i)
                    {
                        if (this.Config.ChargerCabinetConfig[i - 1].UniqId != this.OldConfig.ChargerCabinetConfig[i - 1].UniqId)
                        {
                            Global.AddEvent(head + "铭牌码" + i + "（" + this.Config.ChargerCabinetConfig[i - 1].UniqId + "）", 4);
                        }
                        if (this.Config.ChargerCabinetConfig[i - 1].OutGunNum != this.OldConfig.ChargerCabinetConfig[i - 1].OutGunNum)
                        {
                            Global.AddEvent(head + "外接充电枪数" + i + "（" + this.Config.ChargerCabinetConfig[i - 1].OutGunNum + "）", 4);
                        }
                    }
                }

                //****************总电表******************************
                if (this.Config.NomalConfig.WattNetNum != this.OldConfig.NomalConfig.WattNetNum)
                {
                    Global.AddEvent(head + "总电表数量" + "（" + this.Config.NomalConfig.WattNetNum + "）", 4);
                }
                if (this.Config.NomalConfig.WattNetRatio != this.OldConfig.NomalConfig.WattNetRatio)
                {
                    Global.AddEvent(head + "电流互感器变比" + "（" + this.Config.NomalConfig.WattNetRatio + "）", 4);
                }
                if (this.Config.NomalConfig.WattNetStartIP != this.OldConfig.NomalConfig.WattNetStartIP)
                {
                    Global.AddEvent(head + "总电表开始地址" + "（" + this.Config.NomalConfig.WattNetStartIP + "）", 4);
                }

                //****************换电设备******************************
                if (this.Config.MachineConfig.IP != this.OldConfig.MachineConfig.IP)
                {
                    Global.AddEvent(head + "换电设备地址" + "（" + this.Config.MachineConfig.IP + "）", 4);
                }

                //****************消防设备******************************
                if (this.Config.EnvirTemperConfig.IP != this.OldConfig.EnvirTemperConfig.IP)
                {
                    Global.AddEvent(head + "消防设备主机地址" + "（" + this.Config.EnvirTemperConfig.IP + "）", 4);
                }
                if (this.Config.EnvirTemperConfig.EnvirSensorNum != this.OldConfig.EnvirTemperConfig.EnvirSensorNum)
                {
                    Global.AddEvent(head + "探测器数量" + "（" + this.Config.EnvirTemperConfig.EnvirSensorNum + "）", 4);
                }

                //环境探测器 EnvirTemperDataConfig     
                for (int i = 0; i < this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs.Count; i++)
                {
                    if (this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs[i].SensorNumber != this.OldConfig.EnvirTemperConfig.DataConfig.ChanelConfigs[i].SensorNumber)
                    {
                        Global.AddEvent(head + this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs[i].Text + "探测器" + "（" + this.Config.EnvirTemperConfig.DataConfig.ChanelConfigs[i].SensorNumber + "）", 4);
                    }
                }

                //****************通道设备******************************
                if (this.Config.PasssGateWayConfigs.Count > this.OldConfig.PasssGateWayConfigs.Count)
                {
                    int c = this.Config.PasssGateWayConfigs.Count - this.OldConfig.PasssGateWayConfigs.Count;
                    for (int i = 1; i <= c; i++)
                    {
                        int no = this.OldConfig.PasssGateWayConfigs.Count + i;
                        Global.AddEvent(head + "添加通道" + "（" + no + "）", 4);
                    }
                }
                else if (this.Config.PasssGateWayConfigs.Count < this.OldConfig.PasssGateWayConfigs.Count)
                {
                    Global.AddEvent(head + "删除通道" + "（1）", 4);
                    int c = this.OldConfig.PasssGateWayConfigs.Count - this.Config.PasssGateWayConfigs.Count;
                    for (int i = 1; i <= c; i++)
                    {
                        int no = this.Config.PasssGateWayConfigs.Count + i;
                        Global.AddEvent(head + "删除通道" + "（" + no + "）", 4);
                    }
                }

                if (this.Config.PasssGateWayConfigs.Count > 0)
                {
                    PassGateWayConfig Temp1 = this.Config.PasssGateWayConfigs[0];
                    if (Temp1.busIdentConfig.IP != this.OldConfig.PasssGateWayConfigs[0].busIdentConfig.IP)
                    {
                        Global.AddEvent(head + "通道1车牌识别地址" + "（" + Temp1.busIdentConfig.IP + "）", 4);
                    }
                    if (Temp1.ScreenConfig.IP != this.OldConfig.PasssGateWayConfigs[0].ScreenConfig.IP)
                    {
                        Global.AddEvent(head + "通道1引导显示屏地址" + "（" + Temp1.ScreenConfig.IP + "）", 4);
                    }
                    if (Temp1.LakiIP != this.OldConfig.PasssGateWayConfigs[0].LakiIP)
                    {
                        Global.AddEvent(head + "通道1激光雷达地址" + "（" + Temp1.ScreenConfig.IP + "）", 4);
                    }
                }
                if (this.Config.PasssGateWayConfigs.Count > 1)
                {
                    PassGateWayConfig Temp2 = this.Config.PasssGateWayConfigs[1];
                    if (Temp2.busIdentConfig.IP != this.OldConfig.PasssGateWayConfigs[1].busIdentConfig.IP)
                    {
                        Global.AddEvent(head + "通道2车牌识别地址" + "（" + Temp2.busIdentConfig.IP + "）", 4);
                    }
                    if (Temp2.ScreenConfig.IP != this.OldConfig.PasssGateWayConfigs[1].ScreenConfig.IP)
                    {
                        Global.AddEvent(head + "通道2引导显示屏地址" + "（" + Temp2.ScreenConfig.IP + "）", 4);
                    }
                    if (Temp2.LakiIP != this.OldConfig.PasssGateWayConfigs[1].LakiIP)
                    {
                        Global.AddEvent(head + "通道2激光雷达地址" + "（" + Temp2.ScreenConfig.IP + "）", 4);
                    }
                }
                if (this.Config.PasssGateWayConfigs.Count > 2)
                {
                    PassGateWayConfig Temp3 = this.Config.PasssGateWayConfigs[2];
                    if (Temp3.busIdentConfig.IP != this.OldConfig.PasssGateWayConfigs[2].busIdentConfig.IP)
                    {
                        Global.AddEvent(head + "通道3车牌识别地址" + "（" + Temp3.busIdentConfig.IP + "）", 4);
                    }
                    if (Temp3.ScreenConfig.IP != this.OldConfig.PasssGateWayConfigs[2].ScreenConfig.IP)
                    {
                        Global.AddEvent(head + "通道3引导显示屏地址" + "（" + Temp3.ScreenConfig.IP + "）", 4);
                    }
                    if (Temp3.LakiIP != this.OldConfig.PasssGateWayConfigs[2].LakiIP)
                    {
                        Global.AddEvent(head + "通道3激光雷达地址" + "（" + Temp3.ScreenConfig.IP + "）", 4);
                    }
                }
                if (this.Config.PasssGateWayConfigs.Count > 3)
                {
                    PassGateWayConfig Temp4 = this.Config.PasssGateWayConfigs[3];
                    if (Temp4.busIdentConfig.IP != this.OldConfig.PasssGateWayConfigs[3].busIdentConfig.IP)
                    {
                        Global.AddEvent(head + "通道4车牌识别地址" + "（" + Temp4.busIdentConfig.IP + "）", 4);
                    }
                    if (Temp4.ScreenConfig.IP != this.OldConfig.PasssGateWayConfigs[3].ScreenConfig.IP)
                    {
                        Global.AddEvent(head + "通道4引导显示屏地址" + "（" + Temp4.ScreenConfig.IP + "）", 4);
                    }
                    if (Temp4.LakiIP != this.OldConfig.PasssGateWayConfigs[3].LakiIP)
                    {
                        Global.AddEvent(head + "通道4激光雷达地址" + "（" + Temp4.ScreenConfig.IP + "）", 4);
                    }
                }

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                InValidToConfig();
                this.Config.ToFile();
                Global.config = StationConfig.FromFile();

                DESCADA.Global.PromptSucc("保存成功");
                AddEvents();

                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    //mainWindow.txtStationName.Text = Global.config.NomalConfig.StationName;
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }




        private void btnAddTongdao_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                PassGateWayConfig Temp2 = this.Config.PasssGateWayConfigs[1];
                PassGateWayConfig Temp3 = this.Config.PasssGateWayConfigs[2];
                PassGateWayConfig Temp4 = this.Config.PasssGateWayConfigs[3];
                GridLength l = new GridLength(110);

                if (Temp2.Enable == false)
                {
                    Temp2.Enable = true;
                    TblPassGateWayR1.Height = l;
                }
                else if (Temp3.Enable == false)
                {
                    Temp3.Enable = true;
                    TblPassGateWayR2.Height = l;
                }
                else if (Temp4.Enable == false)
                {
                    Temp4.Enable = true;
                    TblPassGateWayR3.Height = l;
                }

                this.gatewaynuminput.Text = (Convert.ToInt32(this.gatewaynuminput.Text) + 1).ToString();

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        private void ChargerNetNum_TextChanged(object sender, RoutedEventArgs e)
        {
            ChargerNetNumR.Text = this.ChargerNetNum.Text;
        }
        private async void  btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ReleaseBakPath = "C:\\descada\\ReleaseBak\\";
                if (scadaVerInfo.Text == "检查更新")
                {
                    Global.mgttClientService.Publish300F();
                }
                else if (scadaVerInfo.Text == "有更新")
                {
                    Dialog inputDialog = new Dialog("下载", "当前有可用更新" + Global.NewVer + "，是否下载?", "yesno");
                    if (inputDialog.ShowDialog() == true)
                    {
                        string zipFile = ReleaseBakPath + Global.NewVerFileName;
                        scadaVerInfo.Text = "下载中";
                        //bool rtn = Global.httpClient.HttpDownload(Global.NewVerUrl, zipFile);
                        bool rtn=false;
                        await Task.Run(() =>
                        {
                             rtn =  Global.httpClient.HttpDownload(Global.NewVerUrl, zipFile); // 异步等待方法执行完成并获取结果
                        });

                        if (rtn)
                        {
                            string extractPath = zipFile.ToLower().Replace(".zip", "");
                            if (Directory.Exists(extractPath))
                            {
                                Directory.Delete(extractPath, true);
                            }
                            Directory.CreateDirectory(extractPath);
                            System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);

                            scadaVerInfo.Text = "可升级";
                        }
                    }
                }
                else if (scadaVerInfo.Text == "可升级")
                {
                    Dialog inputDialog = new Dialog("升级", "升级包" + Global.NewVer + "已下载，是否立即升级?" + "\r\n" + "升级前请确认没有正在执行的换电和充电", "yesno");
                    if (inputDialog.ShowDialog() == true)
                    {
                        Global.mgttClientService.Publish3010(7);
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.UseShellExecute = false;// 必须设置为false才能重定向标准输入/输出
                        startInfo.CreateNoWindow = false; // 设置为true则不显示cmd窗口
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;// .Hidden; 
                        startInfo.FileName = "cmd.exe"; // 设置要启动的应用程序或命令
                        //startInfo.Arguments = "/c publish  \"C:\\data\\Scada_1.0.2_20240827.zip\"  \"C:\\data\\New1Scada_1.0.2_20240827.zip\""; // 设置cmd.exe的启动参数，‌/c表示执行完命令后关闭cmd窗口
                        string zipPath = ReleaseBakPath + Global.NewVerFileName;
                        string extractPath = zipPath.ToLower().Replace(".zip", "");
                        string NewFile = extractPath + "\\DESCADA.dll";
                        string OldFile = "C:\\descada\\Release\\net6.0-windows\\DESCADA.dll";
                        startInfo.Arguments = "/c publish  \"" + NewFile + "\"  \"" + OldFile + "\""; // 设置cmd.exe的启动参数，‌/c表示执行完命令后关闭cmd窗口

                        //startInfo.Arguments = $"/C copy /Y \"{NewFile}\" \"{OldFile}\"";
                        //startInfo.Arguments = "/c publish  \"C:\\descada\\bak\\0703-1\\DESCADA.dll\"  \"C:\\0HM\\12站端\\Code\\DESCADA\\DESCADA\\bin\\Debug\\net6.0-windows\\DESCADA.dll\""; // 设置cmd.exe的启动参数，‌/c表示执行完命令后关闭cmd窗口

                        process.StartInfo = startInfo;
                        process.Start(); // 开始执行
                        //process.WaitForExit(); // 等待进程执行完毕  C:\descada\Release\net6.0-windows
                        //ExecuteCmdAsync();

                        Application.Current.Shutdown();
                        Global.AddEvent("系统升级退出", 5);
                        Environment.Exit(0);


                    }
                }
            }
            catch (Exception ex)
            {
                if (scadaVerInfo.Text == "可升级")
                {
                    Global.mgttClientService.Publish3010(9);
                }
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        static async Task ExecuteCmdAsync()
        {

            /*
             *                FileName = "cmd.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
             * */
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c publish  \"C:\\descada\\bak\\0703-1\\DESCADA.dll\"  \"C:\\0HM\\12站端\\Code\\DESCADA\\DESCADA\\bin\\Debug\\net6.0-windows\\\\DESCADA.dll\""

            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                // 异步读取输出
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // 等待进程执行完毕
                await process.WaitForExitAsync();

                // 获取输出和错误信息
                string output = await outputTask;
                string error = await errorTask;

                Console.WriteLine($"Output: {output}");
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Error: {error}");
                }
            }
        }
    }



    public class ComboBoxItemData
    {
        public string DisplayText { get; set; }
        public int Value { get; set; }
    }
}
