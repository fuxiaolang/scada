﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DESCADA
{
    /// <summary>
    /// SelCharger.xaml 的交互逻辑
    /// </summary>
    public partial class SelCharger : Window
    {
        public SelCharger(string info)
        {
            InitializeComponent();
            //ShowScreen();
            Global.SetShowScreen(this);
            txtSelCharger.Text = info;
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

        private void initCharger()
        {
            for (int i = 0; i < selCharger.Items.Count; i++)
            {
                string strWorkStatus = "";
                if (Global.SqlCmd104[i] != null && Global.SqlCmd104[i].Parameters.Count > 0)
                    Global.SqlCmd104[i].Parameters["@WorkStatus"].Value.ToString(); //2-充电进行中


                if (Global.ChargerEnableFlag[i] == 0 || strWorkStatus == "2")
                {
                    ComboBoxItem item = selCharger.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                    if (item != null)
                        item.IsEnabled = false;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            initCharger();
        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //do something here
            this.Close();            
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)(selCharger as System.Windows.Controls.ComboBox).SelectedItem;
            string selectedText = cbi.Content.ToString();

            //校验仓位状态
            ushort ChargerID = ushort.Parse(selectedText);
            string strWorkStatus = "";
            if (Global.SqlCmd104[ChargerID] != null && Global.SqlCmd104[ChargerID].Parameters.Count > 0)
                Global.SqlCmd104[ChargerID].Parameters["@WorkStatus"].Value.ToString(); //2-充电进行中
            if (Global.ChargerEnableFlag[ChargerID] == 0)
            {
                Global.Dialog("请确认", "目标仓位停用，请选择其它仓位", "yes");
                return;
            }
            if (strWorkStatus == "2")
            {
                Global.Dialog("请确认", "目标仓位充电中，请选择其它仓位", "yes");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        public string Answer
        {
            get {
                ComboBoxItem cbi = (ComboBoxItem)(selCharger as System.Windows.Controls.ComboBox).SelectedItem;
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
