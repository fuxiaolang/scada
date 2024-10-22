using DESCADA.Service;
using DESCADA.Test;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DESCADA
{
    /// <summary>
    /// Plateno.xaml 的交互逻辑
    /// </summary>
    public partial class login : System.Windows.Window
    {
        public login()
        {
            InitializeComponent();
            Global.SetShowScreen(this);
            if (Global.LogInType == 2)
            {
                imgClose.Visibility = Visibility.Hidden;
                btnCancel.Content = "关闭程序";
            }
            else if (Global.LogInType == 3)
            {
                txtTitle.Text = "切换账号";
            }

            this.DataContext = new UiDesign.ViewModel.MainViewModel();

            string sql = "select userid,account from user   WHERE isEnabled=1 ";
            DataTable dt1 = Global.dbTrans.GetDataTable(sql);
            foreach (DataRow row in dt1.Rows)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = row["account"];
                txtUserID.Items.Add(comboBoxItem);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string machineName = Environment.MachineName;
            if (machineName.ToUpper() == "MENG")


            {
                this.txtUserID.Text = "admin";
                this.txtUserPass.Password = "Jundian2024";
                //btnOK_Click(null, null);
            }
            this.WindowState = WindowState.Maximized;


        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Global.LogInType == 3)
            {
                this.Close();
            }
            else
            {
                Global.AddEvent("系统退出", 5);

                Environment.Exit(0);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string exiSql = "select Role from User where  account='" + this.txtUserID.Text + "' and isEnabled=1";
            exiSql += " and Password='" + Common.DESEncrypt(this.txtUserPass.Password) + "'";
            var obj = Global.dbTrans.ExecuteScalar0(exiSql);
            //string rtn = Global.dbTrans.ExecuteScalar0(exiSql).ToString();
            if (obj == null)
            {
                System.Windows.MessageBox.Show("账号密码不正确");
                return;
            }
            else
            {
                Global.Role = int.Parse(Global.dbTrans.ExecuteScalar0(exiSql).ToString());
                Global.Account = this.txtUserID.Text;
                Global.AddEvent("登入", 4);

                if (Global.LogInType == 3 || Global.LogInType == 2)
                {
                    MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.btnMenuIndex_Click(null,null);
                    }
                }
                this.Close();
            }

            return;
            if (this.txtUserID.Text == "admin" && this.txtUserPass.Password == "Jundian2024")
            {
                this.Close();
                Global.Account = "admin";
                Global.Role = 1;
            }
            else
            {
                System.Windows.MessageBox.Show("账号或密码不正确");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Global.LogInType == 3)
            {
                this.Close();
            }
            else
            {
                Global.AddEvent("系统退出", 5);

                Environment.Exit(0);
            }
        }
    }
}

