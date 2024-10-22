using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
    public partial class ConfigOper : UserControl
    {
        private StationConfig Config;
        private StationConfig OldConfig;


        public ConfigOper()
        {
            InitializeComponent();
            this.OldConfig = Global.config;
            this.Config = Global.config;
            init();
            acl();
        }

        /// <summary>
        /// chkPer2   换电策略 GridSwitch
        //        chkPer3 用户验证模式 GridCehck
        //        chkPer4   充电策略 GridCharge
        //chkPer5 电池筛选 GridBattery
        //chkPer6 程通信配置 GridRemote
        /// </summary>
        private void acl()
        {
            if (Global.Role == 3)
            {
                StaPer.IsEnabled = false;
                if (Global.config.NomalConfig.Per2 == 0)
                {
                    GridSwitch.IsEnabled = false;
                }
                if (Global.config.NomalConfig.Per3 == 0)
                {
                    GridCehck.IsEnabled = false;
                }
                if (Global.config.NomalConfig.Per4 == 0)
                {
                    GridCharge.IsEnabled = false;
                }
                if (Global.config.NomalConfig.Per5 == 0)
                {
                    GridBattery.IsEnabled = false;
                }
                if (Global.config.NomalConfig.Per6 == 0)
                {
                    GridRemote.IsEnabled = false;
                }
            }
        }

        private void init()
        {
            InValidFromConfig();

        }

        public void InValidFromConfig()
        {
            try
            {
                //充电策略
                this.ChargeStartSocInput.Text = this.Config.NomalConfig.ChargeExTactics.StartSoc.ToString(); //自动充电启动SOC
                this.ChargeStopSocInput.Text = this.Config.NomalConfig.ChargeExTactics.StopSoc.ToString(); //自动充电截止SOC：
                                                                                                           //充电电流单选类型 ending电流 : A 功率：Kw    倍率 ：C
                switch (this.Config.NomalConfig.ChargeExTactics.DefaultApType.ToUpper())
                {
                    case "A":
                        this.ApTypeA.IsChecked = true;
                        break;
                    case "KW":
                        this.ApTypeKW.IsChecked = true;
                        break;
                    case "C":
                        this.ApTypeC.IsChecked = true;
                        break;
                    default:
                        this.ApTypeA.IsChecked = true;
                        break;
                }

                this.ApInput.ToolTip = this.Config.NomalConfig.ChargeExTactics.DefaultApType.ToUpper();
                this.ApInput.Text = this.Config.NomalConfig.ChargeExTactics.DefaultAp.ToString();//  / 100; 充电电流限值

                //换电策略
                this.ChangeEnableSocInput.Text = this.Config.NomalConfig.ChargeExTactics.SocForEx.ToString(); //(int) 换电电池SOC筛选阈值
                this.identenable.Text = this.Config.NomalConfig.ChargeExTactics.IdentEnabelSoc.ToString(); // (int) 可用电池SOC计算阈值 

                //电池选择策略         /// 电池选择策略 1 高SOC 2 充电完成时间 3 用户预约
                switch (this.Config.NomalConfig.ChargeExTactics.SocPrior)
                {
                    case 1:
                        this.SOCpriCheck.IsChecked = true;
                        break;
                    case 2:
                        this.StoptimePriCheck.IsChecked = true;
                        break;
                    case 3:
                        this.UserPriCheck.IsChecked = true;
                        break;
                    default:
                        this.SOCpriCheck.IsChecked = true;
                        break;
                }

                //用户验证模式  
                switch (this.Config.NomalConfig.ValidType)
                {
                    case 1:
                        this.ValidType1.IsChecked = true;
                        break;
                    case 2:
                        this.ValidType2.IsChecked = true;
                        break;
                    case 3:
                        this.ValidType3.IsChecked = true;
                        break;
                    default:
                        this.ValidType1.IsChecked = true;
                        break;
                }

                //远程传输
                this.timeoutinput.Text = this.Config.NomalConfig.RemoteConfig.RemoteIdentTimeOut.ToString(); //远程验证超时
                this.RemoteEnableCheck.IsOn = this.Config.NomalConfig.RemoteConfig.RemoteEnable;

                //权限
                if (this.Config.NomalConfig.Per1 == 1) this.chkPer1.IsChecked = true;
                if (this.Config.NomalConfig.Per2 == 1) this.chkPer2.IsChecked = true;
                if (this.Config.NomalConfig.Per3 == 1) this.chkPer3.IsChecked = true;
                if (this.Config.NomalConfig.Per4 == 1) this.chkPer4.IsChecked = true;
                if (this.Config.NomalConfig.Per5 == 1) this.chkPer5.IsChecked = true;
                if (this.Config.NomalConfig.Per6 == 1) this.chkPer6.IsChecked = true;


                //this.ChargeRecTimeSpanInput.Text = this.Config.NomalConfig.RecordTactics.ChargerDetailInterval / 1000;
                //this.statetimerinput.Text = this.Config.NomalConfig.RecordTactics.StationRawDataRecInterval / 1000;
                //this.envirtimerinput.Text = this.Config.NomalConfig.RecordTactics.EnvirRecInterval / 1000;
                //this.EnergyRecInput.Text = this.Config.NomalConfig.RecordTactics.WattRecInterval / 1000;
                //this.RemoteStateInput.Text = this.Config.NomalConfig.RecordTactics.RemoteStateInterval / 1000;
                //this.RemoteEnvirInput.Text = this.Config.NomalConfig.RecordTactics.RemoteEnvirInterval / 1000;
                //this.UnitVoltTemperInput.Text = this.Config.NomalConfig.RecordTactics.RemoteVTRecInterval / 1000;
                //this.AutoDelCheck.Checked = this.Config.NomalConfig.RemoteConfig.DelSucData;

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

                //充电策略
                this.Config.NomalConfig.ChargeExTactics.StartSoc = Convert.ToSingle(this.ChargeStartSocInput.Text);
                this.Config.NomalConfig.ChargeExTactics.StopSoc = Convert.ToSingle(this.ChargeStopSocInput.Text);
                this.Config.NomalConfig.ChargeExTactics.DefaultAp = Convert.ToInt32(this.ApInput.Text);// * 100;
                                                                                                       //充电电流单选类型 ending电流 : A 功率：Kw    倍率 ：C
                if (this.ApTypeA.IsChecked == true)
                    this.Config.NomalConfig.ChargeExTactics.DefaultApType = "A";
                else if (this.ApTypeKW.IsChecked == true)
                    this.Config.NomalConfig.ChargeExTactics.DefaultApType = "KW";
                else if (this.ApTypeC.IsChecked == true)
                    this.Config.NomalConfig.ChargeExTactics.DefaultApType = "C";

                //换电策略
                this.Config.NomalConfig.ChargeExTactics.SocForEx = Convert.ToSingle(this.ChangeEnableSocInput.Text);
                this.Config.NomalConfig.ChargeExTactics.IdentEnabelSoc = Convert.ToSingle(this.identenable.Text);


                //电池选择策略
                if (this.SOCpriCheck.IsChecked == true)
                    this.Config.NomalConfig.ChargeExTactics.SocPrior = 1;
                else if (this.StoptimePriCheck.IsChecked == true)
                    this.Config.NomalConfig.ChargeExTactics.SocPrior = 2;
                else if (this.UserPriCheck.IsChecked == true)
                    this.Config.NomalConfig.ChargeExTactics.SocPrior = 3;


                //用户验证模式  ending
                if (this.ValidType1.IsChecked == true)
                    this.Config.NomalConfig.ValidType = 1;
                else if (this.ValidType2.IsChecked == true)
                    this.Config.NomalConfig.ValidType = 2;
                else if (this.ValidType3.IsChecked == true)
                    this.Config.NomalConfig.ValidType = 3;

                //远程传输
                this.Config.NomalConfig.RemoteConfig.RemoteIdentTimeOut = Convert.ToInt32(this.timeoutinput.Text);
                this.Config.NomalConfig.RemoteConfig.RemoteEnable = this.RemoteEnableCheck.IsOn;

                //权限
                if (this.chkPer1.IsChecked == true) this.Config.NomalConfig.Per1 = 1; else this.Config.NomalConfig.Per1 = 0;
                if (this.chkPer2.IsChecked == true) this.Config.NomalConfig.Per2 = 1; else this.Config.NomalConfig.Per2 = 0;
                if (this.chkPer3.IsChecked == true) this.Config.NomalConfig.Per3 = 1; else this.Config.NomalConfig.Per3 = 0;
                if (this.chkPer4.IsChecked == true) this.Config.NomalConfig.Per4 = 1; else this.Config.NomalConfig.Per4 = 0;
                if (this.chkPer5.IsChecked == true) this.Config.NomalConfig.Per5 = 1; else this.Config.NomalConfig.Per5 = 0;
                if (this.chkPer6.IsChecked == true) this.Config.NomalConfig.Per6 = 1; else this.Config.NomalConfig.Per6 = 0;


                //this.Config.NomalConfig.RecordTactics.ChargerDetailInterval = this.ChargeRecTimeSpanInput.Text * 1000;
                //this.Config.NomalConfig.RecordTactics.StationRawDataRecInterval = this.statetimerinput.Text * 1000;
                //this.Config.NomalConfig.RecordTactics.EnvirRecInterval = this.envirtimerinput.Text * 1000;
                //this.Config.NomalConfig.RecordTactics.WattRecInterval = this.EnergyRecInput.Text * 1000;
                //this.Config.NomalConfig.RecordTactics.RemoteStateInterval = this.RemoteStateInput.Text * 1000;
                //this.Config.NomalConfig.RecordTactics.RemoteEnvirInterval = this.RemoteEnvirInput.Text * 1000;
                //this.Config.NomalConfig.RecordTactics.RemoteVTRecInterval = this.UnitVoltTemperInput.Text * 1000;
                //this.Config.NomalConfig.RemoteConfig.DelSucData = this.AutoDelCheck.Checked;

                //txtAlert
                //this.Parent.
                //MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                //if (mainWindow != null)
                //{
                //    mainWindow.txtAlert.Text="保存成功";
                //}
                DESCADA.Global.PromptSucc("保存成功");
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
                //充电策略
                if (this.Config.NomalConfig.ChargeExTactics.StartSoc != this.OldConfig.NomalConfig.ChargeExTactics.StartSoc)
                {
                    Global.AddEvent("修改配置自动充电启动SOC" + "（" + this.Config.NomalConfig.ChargeExTactics.StartSoc + "）", 4);
                }
                if (this.Config.NomalConfig.ChargeExTactics.StopSoc != this.OldConfig.NomalConfig.ChargeExTactics.StopSoc)
                {
                    Global.AddEvent("修改配置自动充电截止SOC" + "（" + this.Config.NomalConfig.ChargeExTactics.StopSoc + "）", 4);
                }
                if (this.Config.NomalConfig.ChargeExTactics.DefaultAp != this.OldConfig.NomalConfig.ChargeExTactics.DefaultAp || this.Config.NomalConfig.ChargeExTactics.DefaultApType != this.OldConfig.NomalConfig.ChargeExTactics.DefaultApType)
                {
                    if (this.ApTypeA.IsChecked == true)
                        Global.AddEvent("修改配置枪口充电电流限值" + "（" + this.Config.NomalConfig.ChargeExTactics.DefaultAp + "）", 4);
                    else if (this.ApTypeKW.IsChecked == true)
                        Global.AddEvent("修改配置枪口充电功率限值" + "（" + this.Config.NomalConfig.ChargeExTactics.DefaultAp + "）", 4);
                }
                //换电策略
                if (this.Config.NomalConfig.ChargeExTactics.SocForEx != this.OldConfig.NomalConfig.ChargeExTactics.SocForEx)
                {
                    Global.AddEvent("修改配置换电电池SOC筛选阀值" + "（" + this.Config.NomalConfig.ChargeExTactics.SocForEx + "）", 4);
                }
                if (this.Config.NomalConfig.ChargeExTactics.IdentEnabelSoc != this.OldConfig.NomalConfig.ChargeExTactics.IdentEnabelSoc)
                {
                    Global.AddEvent("修改配置可用电池SOC计算阀值" + "（" + this.Config.NomalConfig.ChargeExTactics.IdentEnabelSoc + "）", 4);
                }

                //电池选择策略
                if (this.Config.NomalConfig.ChargeExTactics.SocPrior != this.OldConfig.NomalConfig.ChargeExTactics.SocPrior)
                {
                    if (this.SOCpriCheck.IsChecked == true)
                        Global.AddEvent("修改配置电池选择策略" + "（高SOC电池优先）", 4);
                    else if (this.StoptimePriCheck.IsChecked == true)
                        Global.AddEvent("修改配置电池选择策略" + "（充电完成时间优先）", 4);
                    else if (this.UserPriCheck.IsChecked == true)
                        Global.AddEvent("修改配置电池选择策略" + "（用户预约）", 4);
                }

                //用户验证模式  ending
                if (this.Config.NomalConfig.ValidType != this.OldConfig.NomalConfig.ValidType)
                {
                    if (this.ValidType1.IsChecked == true)
                        Global.AddEvent("修改配置用户验证模式" + "（远程验证）", 4);
                    else if (this.ValidType2.IsChecked == true)
                        Global.AddEvent("修改配置用户验证模式" + "（本地验证）", 4);
                }

                //远程传输
                if (this.Config.NomalConfig.RemoteConfig.RemoteEnable != this.OldConfig.NomalConfig.RemoteConfig.RemoteEnable)
                {
                    if (this.RemoteEnableCheck.IsOn == true)
                        Global.AddEvent("修改配置远程传输（启用远程通信）", 4);
                    else if (this.ValidType2.IsChecked == true)
                        Global.AddEvent("修改配置远程传输（关闭远程通信）", 4);
                }

                //权限
                if (this.Config.NomalConfig.Per1 != this.OldConfig.NomalConfig.Per1)
                {
                    if (this.chkPer1.IsChecked == true)
                        Global.AddEvent("修改配置站员权限（允许基础配置）", 4);
                    else
                        Global.AddEvent("修改配置站员权限（禁止基础配置）", 4);
                }
                if (this.Config.NomalConfig.Per2 != this.OldConfig.NomalConfig.Per2)
                {
                    if (this.chkPer2.IsChecked == true)
                        Global.AddEvent("修改配置站员权限（允许换电策略）", 4);
                    else
                        Global.AddEvent("修改配置站员权限（禁止换电策略）", 4);
                }
                if (this.Config.NomalConfig.Per3 != this.OldConfig.NomalConfig.Per3)
                {
                    if (this.chkPer3.IsChecked == true)
                        Global.AddEvent("修改配置站员权限（允许用户验证模式）", 4);
                    else
                        Global.AddEvent("修改配置站员权限（禁止户验证模式）", 4);
                }
                if (this.Config.NomalConfig.Per4 != this.OldConfig.NomalConfig.Per4)
                {
                    if (this.chkPer4.IsChecked == true)
                        Global.AddEvent("修改配置站员权限（允许充电策略）", 4);
                    else
                        Global.AddEvent("修改配置站员权限（禁止充电策略）", 4);
                }
                if (this.Config.NomalConfig.Per5 != this.OldConfig.NomalConfig.Per5)
                {
                    if (this.chkPer5.IsChecked == true)
                        Global.AddEvent("修改配置站员权限（允许电池筛选）", 4);
                    else
                        Global.AddEvent("修改配置站员权限（禁止电池筛选）", 4);
                }
                if (this.Config.NomalConfig.Per6 != this.OldConfig.NomalConfig.Per6)
                {
                    if (this.chkPer6.IsChecked == true)
                        Global.AddEvent("修改配置站员权限（允许远程通信配置）", 4);
                    else
                        Global.AddEvent("修改配置站员权限（禁止远程通信配置）", 4);
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
                AddEvents();
                Global.chargerServer.ConfigAll();
                
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }


        }
        private void ApTypeA_Click(object sender, RoutedEventArgs e)
        {
            ApInput.ToolTip = "A";
        }
        private void ApTypeKW_Click(object sender, RoutedEventArgs e)
        {
            ApInput.ToolTip = "KW";
        }
        private void ApTypeC_Click(object sender, RoutedEventArgs e)
        {
            ApInput.ToolTip = "C";
        }

    }
}
