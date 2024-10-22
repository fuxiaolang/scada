﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;

namespace DESCADA
{
    /// <summary>
    /// SelCharger.xaml 的交互逻辑
    /// </summary>
    public partial class Dialog3 : Window
    {
        public DispatcherTimer showtimer1;

        public Dialog3(string title,string info,string buttons)
        {
            InitializeComponent();
            //ShowScreen();
            Global.SetShowScreen(this);
            txtTitle.Text = title;
            txtInfo.Text = info;
            if (buttons == "yes" || buttons== "yesWithoutClose")
            { 
               this.btnDialogCancel.Visibility = System.Windows.Visibility.Hidden;
            }

            if ( buttons == "yesWithoutClose")
            {
                this.btnDialogCancel.Visibility = System.Windows.Visibility.Hidden;
                imgClose.Visibility = System.Windows.Visibility.Hidden;
            }


            showtimer1 = new DispatcherTimer();
            showtimer1.Tick += Showtimer1_Tick;
            showtimer1.Interval = new TimeSpan(0, 0, 0, 0,1000);
            showtimer1.Start();  //0311拍视频


        }
        int djs =3;
        private void Showtimer1_Tick(object sender, EventArgs e)
        {
            djs--;
            this.txtDJS.Dispatcher.Invoke(new Action(() => { this.txtDJS.Text = djs + ""; ; }));

            if (djs==0) this.Close();
        }

            void ShowScreen()
        {
            if (Screen.AllScreens.Length > 1)
            {
                Screen s2 = Screen.AllScreens[1];
                System.Drawing.Rectangle r2 = s2.WorkingArea;
                this.Left = -1920;  //r2.Left; //-2880   -1920; 
                //this.Top = r2.Top;
                this.Top = r2.Top;  //-300
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }
            else
            {
                Screen s1 = Screen.AllScreens[0];
                System.Drawing.Rectangle r1 = s1.WorkingArea;
                //this.Top = r1.Top;
                this.Top = r1.Top; //-300
                this.Left = r1.Left;
                //不能在这里设置窗体状态
                //this.WindowState = WindowState.Maximized;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //do something here
            this.Close();            
        }

   

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();

        }

        public string Answer
        {
            get {
                /// ComboBoxItem cbi = (ComboBoxItem)(selCharger as System.Windows.Controls.ComboBox).SelectedItem;
                //string selectedText = cbi.Content.ToString();

                return "";// selCharger.SelectedItem.ToString(); 
            }
        }

        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();


        }
    }
}
