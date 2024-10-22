using DESCADA.Service;
using DESCADA.Test;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
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
    /// Plateno.xaml 的交互逻辑
    /// </summary>
    public partial class Password : Window
    {
        public string UserID = "";
        public string UserAccount = "";

        public Password()
        {
            InitializeComponent();
            Global.SetShowScreen(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserAccount != Global.Account)
            {
                btnOK.IsEnabled=false;
            }
        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string sql = "";
            bool isFail = false;

            try
            {
                if (txtOldPass.Password == "")
                {
                    Global.PromptFail("请输入旧密码");
                    isFail = true;
                    return;
                }
                if (txtNewPass1.Password == "")
                {
                    Global.PromptFail("请输入新密码");
                    isFail = true;
                    return;
                }
                if (txtNewPass2.Password == "")
                {
                    Global.PromptFail("请输入确认密码");
                    isFail = true;
                    return;
                }
                if (txtNewPass1.Password.Replace("'", "''")!= txtNewPass1.Password)
                {
                    Global.PromptFail("新密码不能包含单引号");
                    isFail = true;
                    return;
                }
                if (txtNewPass2.Password != txtNewPass1.Password)
                {
                    Global.PromptFail("确认密码和新密码不一致");
                    isFail = true;
                    return;
                }

                string exiSql = "select Account from User where  UserID=" + UserID;
                exiSql += " and Password='" + Common.DESEncrypt(this.txtOldPass.Password) + "'";
                var obj = Global.dbTrans.ExecuteScalar0(exiSql);
                if (obj == null)
                {
                    System.Windows.MessageBox.Show("旧密码不正确");
                    return;
                }


                sql = "update  User set ";
                sql += " Password='" + Common.DESEncrypt(txtNewPass1.Password) + "'";
                sql += " where UserID=" + UserID;

                Global.WriteDebugAsync("User password" + sql, "user");
                Global.dbTrans.ExcuteScript(sql);
                Global.PromptSucc("密码修改成功");

                Global.AddEvent("修改"+ UserAccount + "账号密码", 4);


                this.Close();
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]密码修改btnOK_Click" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("密码修改失败" + ex.Message);
            }

        }

        private void btnResetPass_Click(object sender, RoutedEventArgs e)
        {
            string sql = "";
            try
            {
                sql = " update   User  set ";
                sql += " Password='" + Common.DESEncrypt("123456") + "'";
                sql += " where Userid=" + UserID;

                Global.dbTrans.ExcuteScript(sql);
                Global.PromptSucc("密码重置成功");
                Global.AddEvent("重置" + UserAccount + "账号密码", 4);

                this.Close();

            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]User-btnResetPass_Click" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("密码重置成功" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
