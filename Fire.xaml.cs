using DESCADA.Models;
using DESCADA.Service;
using DESCADA.Test;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.RpcContracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static DESCADA.Service.PLC;
using static DESCADA.Switch;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static WPF_DateTimePicker.UserControls.DateTimePicker.View.TMinSexView;

namespace DESCADA
{
    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class Fire : System.Windows.Controls.UserControl
    {
        private System.Threading.Timer timer1;
        public Local local;

        public Fire()
        {
            InitializeComponent();
            timer1 = new System.Threading.Timer(timer1_Tick, null, 0, 1000);

        }

        //每秒一次
        private void timer1_Tick(object state)
        {
            setMainStatus();
            setCharger();
        }

        //集中探测器状态显示
        private void setMainStatus()
        {
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if (Global.FireConnected && Global.FireServer != null && Global.FireServer.CmdC[6]!=null )
                {
                    byte Controller = Global.FireServer.CmdC[6];//控制器状态 0代表正常，值1代表故障
                    byte Monitor = Global.FireServer.CmdC[7];//显示器状态 0代表正常，值1代表故障
                    byte WarnEnable = Global.FireServer.CmdB[11];//报警禁止状态状态 不禁止时，值为0  禁止时，值为1
                    byte FireEnable = Global.FireServer.CmdB[12];//灭火禁止器状态  不禁止时，值为0  禁止时，值为1

                    if (Global.DiffSeconds(Global.FireServer.CmdCTime, DateTime.Now) < 180)
                    {
                        txtControllerMain.Text = (Controller == 0) ? "正常" : "故障";
                        txtMonitorMain.Text = (Monitor == 0) ? "正常" : "故障";
                    }
                    else
                    {
                        txtControllerMain.Text = "--";
                        txtMonitorMain.Text = "--";
                    }

                    if (Global.DiffSeconds(Global.FireServer.CmdBTime, DateTime.Now) < 180)
                    {
                        txtWarnEnable.Text = (WarnEnable == 0) ? "否" : "是";
                        txtFireEnable.Text = (FireEnable == 0) ? "否" : "是";
                    }
                    else {
                        txtWarnEnable.Text = "--";
                        txtFireEnable.Text = "--";
                    }

                }
                else {
                    //未连接显示--
                    txtControllerMain.Text = "--";
                    txtMonitorMain.Text = "--";
                    txtWarnEnable.Text = "--";
                    txtFireEnable.Text = "--";
                }
            });
        }

        //电池仓状态显示
        private void setCharger()
        {
            //暂时假定仓位号和探测器号对应 pending  加个配置；
            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                for (int i = 0; i < 10; i++)
                {
                    ChanelConfig chanelConfig = Global.config.EnvirTemperConfig.DataConfig.ChanelConfigs.FirstOrDefault(t => t.DeviceNumber == i);
                    int k = chanelConfig.SensorNumber;//探测器编号，加个配置匹配；
                    TextBlock txtAlarmLevel = (TextBlock)this.FindName("txtAlarmLevel" + i);
                    TextBlock txtSomkeState = (TextBlock)this.FindName("txtSomkeState" + i);
                    TextBlock txtCo = (TextBlock)this.FindName("txtCo" + i);
                    TextBlock txttemp = (TextBlock)this.FindName("txttemp" + i);
                    TextBlock txtDetector = (TextBlock)this.FindName("txtDetector" + i);
                    TextBlock txtSensor = (TextBlock)this.FindName("txtSensor" + i);
                    Border CharerBorder = (Border)this.FindName("CharerBorder" + i);
                    Border CharerFireLevelBorder = (Border)this.FindName("CharerFireLevelBorder" + i);

                    if ( Global.FireConnected && Global.FireServer != null
                         && Global.DiffSeconds(Global.FireServer.CmdATime[k], DateTime.Now) < 180
                        &&  Global.FireServer.CmdA[k] != null)
                        {
                        //if (Global.FireServer.CmdA[k][7] == null) continue;

                        byte alarmLevel = Global.FireServer.CmdA[k][7]; //报警等级
                        txtAlarmLevel.Text = alarmLevel.ToString();

                        byte somkeState = Global.FireServer.CmdA[k][10]; //烟雾状态
                        txtSomkeState.Text = (somkeState == 0) ? "正常" : "报警";

                        double co = Global.FireServer.CmdA[k][8]; //一氧化碳浓度
                        co = co * 0.1;
                        txtCo.Text = co.ToString()+ "ppm";

                        double temp = Global.FireServer.CmdA[k][9]; //温度数据
                        temp = temp - 40;
                        txttemp.Text = temp.ToString()+ "℃";

                        byte workState = Global.FireServer.CmdA[k][11]; //探测器 
                        txtDetector.Text = (workState == 1) ? "故障" : "正常";
                        txtSensor.Text = (workState == 2) ? "故障" : "正常";

                        //reg warn
                        if (alarmLevel != 0)
                        {
                            CharerBorder.Style = (Style)this.FindResource("ChargerFireWarn");
                            CharerFireLevelBorder.Style = (Style)this.FindResource("ChargerFireLevelWarn");
                        }
                        else {
                            if (CharerBorder.Style != (Style)this.FindResource("ChargerFireNormal"))
                            {
                                CharerBorder.Style = (Style)this.FindResource("ChargerFireNormal");
                                CharerFireLevelBorder.Style = (Style)this.FindResource("ChargerFireLevelNormal");
                            }
                        }
                    }
                    else
                    {
                        //未连接显示--
                        txtAlarmLevel.Text = "--";
                        txtSomkeState.Text = "--";
                        txtCo.Text = "--";
                        txttemp.Text = "--";
                        txtDetector.Text = "--";
                        txtSensor.Text = "--";
                        if (CharerBorder.Style != (Style)this.FindResource("ChargerFireNormal"))
                        {
                            CharerBorder.Style = (Style)this.FindResource("ChargerFireNormal");
                            CharerFireLevelBorder.Style = (Style)this.FindResource("ChargerFireLevelNormal");
                        }
                    }
                }


            });

        }

        private void btnFirereset_Click(object sender, RoutedEventArgs e)
        {
            Global.FireServer.reset();
        }
    }
}
