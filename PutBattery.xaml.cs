using DESCADA.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DESCADA
{
    /// <summary>
    /// PutBattery.xaml 的交互逻辑
    /// </summary>
    public partial class PutBattery : System.Windows.Window
    {
        public PutBattery(string info)
        {
            InitializeComponent();
            //ShowScreen();
            Global.SetShowScreen(this);
            //txtSelCharger.Text = info;
        }

        void ShowScreen()
        {
            if (Screen.AllScreens.Length > 1)
            {
                Screen s2 = Screen.AllScreens[1];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                this.Left = -1920;  //r2.Left; //-2880   -1920; 
                //this.Top = r2.Top;
                this.Top = r2.Top-300;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }
            else
            {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                //this.Top = r1.Top;
                this.Top = r1.Top-300;
                this.Left = r1.Left;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            initCharger();
        }


        private void initCharger()
        {

            for (int i = 0; i < selChargerPick.Items.Count; i++)
            {
                int k = i;//仓位号
                ComboBoxItem itemPick = (ComboBoxItem)selChargerPick.Items[i];
                ComboBoxItem itemPut = (ComboBoxItem)selChargerPut.Items[i];
                string itemContent = itemPick.Content.ToString();
                if (itemContent == "车端") k = 20;
                else if (itemContent == "消防仓") k = 30;

                if (k != 20 && k != 30)
                {
                    string strWorkStatus = "";
                    if (Global.SqlCmd104[i] != null && Global.SqlCmd104[i].Parameters.Count > 0)
                        strWorkStatus = Global.SqlCmd104[i].Parameters["@WorkStatus"].Value.ToString(); //2-充电进行中

                    if (Global.ChargerEnableFlag[i] == 0 || strWorkStatus == "2")
                    {
                        if (itemPick != null)
                        {
                            itemPick.IsEnabled = false;
                            itemPick.Background= new SolidColorBrush(Colors.Gray);
                        }
                        if (itemPut != null)
                        {
                            itemPut.IsEnabled = false;
                            itemPut.Background = new SolidColorBrush(Colors.Gray);
                        }
                    }
                }
                
            }
        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //do something here
            this.Close();            
        }


        private void ValidType_Checked(object sender, RoutedEventArgs e)
        {
            if (ValidType1.IsChecked == true )
            {
                this.panelSelChargerPick.Visibility=Visibility.Visible;
                this.panelSelChargerPut.Visibility = Visibility.Hidden;
            }
            else if (ValidType2.IsChecked == true )
            {
                this.panelSelChargerPick.Visibility = Visibility.Hidden;
                this.panelSelChargerPut.Visibility = Visibility.Visible;
            }
            else if ( ValidType3.IsChecked == true && this.panelSelChargerPick!=null)
            {
                this.panelSelChargerPick.Visibility = Visibility.Visible;
                this.panelSelChargerPut.Visibility = Visibility.Visible;
            }
        }


        /*
         * 换电/调仓：单动模式下，点击后弹窗，选择PC手动换仓指令PCManuExchangeCmd：
1-取电池（此时机器人工位须无电池RobotLocationCheck=1，否则不可用）；
2-放电池（此时机器人工位须有电池RobotLocationCheck=2，否则不可用），

        */
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            Global.PLCSendLock = true;
            try { 
                ComboBoxItem cbiPick = (ComboBoxItem)(selChargerPick as System.Windows.Controls.ComboBox).SelectedItem;
                string selectedTextPick = (cbiPick==null)?"":cbiPick.Content.ToString();
                ComboBoxItem cbiPut = (ComboBoxItem)(selChargerPut as System.Windows.Controls.ComboBox).SelectedItem;
                string selectedTextPut = (cbiPut == null) ? "" : cbiPut.Content.ToString();
                int ChargerIDPick = (selectedTextPick == "") ? -1 : Int16.Parse(selectedTextPick);
                int ChargerIDPut = (selectedTextPut == "") ? -1 : Int16.Parse(selectedTextPut);
                //校验仓位状态
                string strWorkStatusPick = "";
                string strWorkStatusPut = "";
                if (ChargerIDPick!=-1 && Global.SqlCmd104[ChargerIDPick] != null && Global.SqlCmd104[ChargerIDPick].Parameters.Count > 0)
                        strWorkStatusPick=Global.SqlCmd104[ChargerIDPick].Parameters["@WorkStatus"].Value.ToString(); //2-充电进行中
                if (ChargerIDPut != -1 && Global.SqlCmd104[ChargerIDPut] != null && Global.SqlCmd104[ChargerIDPut].Parameters.Count > 0)
                        strWorkStatusPut = Global.SqlCmd104[ChargerIDPut].Parameters["@WorkStatus"].Value.ToString(); //2-充电进行中

                if (ValidType1.IsChecked==true || ValidType3.IsChecked == true) 
                {
                        // 取电池
                        if (selectedTextPick == "")
                        {
                            Global.Dialog("请确认", "请先选择取电池仓位", "yes");
                            return;
                        }                   
                        else if (Global.ChargerEnableFlag[ChargerIDPick] == 0)
                        {
                            Global.Dialog("请确认", "取电池仓位停用，请选择其它仓位", "yes");
                            return;
                        }
                        else if (strWorkStatusPick == "2")
                        {
                            Global.Dialog("请确认", "取电池仓位充电中，请选择其它仓位", "yes");
                            return;
                        }
                        else if (Global.PLCMsg4.RobotLocationCheck == 2)  //1：无电池；2：有电池；
                        {
                            Global.Dialog("请确认", "机器人上有电池，无法执行该指令", "yes");
                            return;
                        }
                    }
                
                if (ValidType2.IsChecked == true || ValidType3.IsChecked == true)
                {
                        // 放电池
                        if (selectedTextPut == "")
                        {
                            Global.Dialog("请确认", "请先选择放电池仓位", "yes");
                            return;
                        }
                        else if (Global.ChargerEnableFlag[ChargerIDPut] == 0)
                        {
                            Global.Dialog("请确认", "放电池仓位停用，请选择其它仓位", "yes");
                            return;
                        }
                        else if (strWorkStatusPut == "2")
                        {
                            Global.Dialog("请确认", "放电池仓位充电中，请选择其它仓位", "yes");
                            return;
                        }
                        else if (Global.PLCMsg4.RobotLocationCheck == 1) //1：无电池；2：有电池；
                        {
                            Global.Dialog("请确认", "机器人上无电池，无法执行该指令", "yes"); 
                            return;
                        }
                }

                if (ValidType1.IsChecked == true)
                {
                    Global.PLCMsg2.PCManuExchangeCmd = 1;//取电池
                    Global.PLCMsg2.PCManuPickNum = (ushort)ChargerIDPick;
                    Global.PLCMsg2.PCManuPutNum = 9999;
                }
                else if (ValidType2.IsChecked == true)
                {
                    Global.PLCMsg2.PCManuExchangeCmd = 2; //放电池
                    Global.PLCMsg2.PCManuPickNum =  9999;
                    Global.PLCMsg2.PCManuPutNum = (ushort)ChargerIDPut;
                }
                else if (ValidType3.IsChecked == true)
                {
                    Global.PLCMsg2.PCManuExchangeCmd = 4; //取放电池
                    Global.PLCMsg2.PCManuPickNum = (ushort)ChargerIDPick;
                    Global.PLCMsg2.PCManuPutNum = (ushort)ChargerIDPut;
                }

                Global.PLCMsg2.PCManuExchangeStartCmd = 1;
                Global.PLCMsg2.PCToPLCCmd = 3; //单动模式固定发送
                Global.PLCSendLock = false;
            }catch (Exception ex) { Global.WriteLog("[Error]PutBattery btnDialogOk_Click"+ex.Message.ToString()); }
            finally { Global.PLCSendLock = false;}

            this.DialogResult = true;
            this.Close();
        }

        //返回取电池仓位，后续下电请求
        public string Answer
        {
            get {
                ComboBoxItem cbi = (ComboBoxItem)(selChargerPick as System.Windows.Controls.ComboBox).SelectedItem;
                string selectedText = cbi.Content.ToString();

                return selectedText;// selCharger.SelectedItem.ToString(); 
            }
        }

        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();


        }

   
    }
}
