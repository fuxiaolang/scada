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
    public partial class UserEdit : Window
    {
        public string UserID = "";
        public User user = null;
        public int IsEnabled = 0;
        public string EnableInfo = "";
        public string Name = "";
        public int Role = 0;

        public UserEdit(User user)
        {
            InitializeComponent();
            Global.SetShowScreen(this);
            this.user = user;
        }

        private void SetDetail()
        {
            try
            {
                if (UserID != "")
                {
                    string sql = "select *  from user where userid=" + UserID;
                    System.Data.DataTable dt = Global.dbTrans.GetDataTable(sql);
                    if (dt != null || dt.Rows.Count > 0)
                    {
                        txtAccount.Text = dt.Rows[0]["Account"].ToString();
                        txtName.Text = dt.Rows[0]["Name"].ToString();
                        cmbRole.SelectedIndex = int.Parse(dt.Rows[0]["Role"].ToString());
                        Name = txtName.Text;
                        Role = cmbRole.SelectedIndex;
                        IsEnabled = int.Parse(dt.Rows[0]["IsEnabled"].ToString());
                        if (IsEnabled == 1)
                        { EnableInfo = "锁定"; }
                        else { EnableInfo = "激活"; }
                        btnEnable.Content = EnableInfo;
                    }
                }
                else
                {
                    txtTitle.Text = "添加账号";
                    btnPass.Visibility = System.Windows.Visibility.Hidden;
                    btnEnable.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void acl()
        {
            try
            {
                DisCmbRole(1);
                if (UserID != "")
                {
                    txtAccount.IsEnabled = false;
                    if (txtAccount.Text != Global.Account && Global.Role != 1)
                    {
                        this.btnPass.IsEnabled = false;
                    }

                    if (Global.Role == 1)
                    {

                        if (txtAccount.Text == Global.Account)
                        {
                            btnEnable.IsEnabled = false;
                            cmbRole.IsEnabled = false;

                        }
                    }
                    else if (Global.Role == 2)
                    {
                        cmbRole.IsEnabled = false;
                        if (cmbRole.SelectedIndex == 1)
                        {
                            this.btnOK.IsEnabled = false;
                            btnEnable.IsEnabled = false;
                        }
                        else if (cmbRole.SelectedIndex == 2)
                        {
                            btnEnable.IsEnabled = false;

                            if (txtAccount.Text != Global.Account)
                            {
                                this.btnOK.IsEnabled = false;
                            }
                        }

                    }
                    else if (Global.Role == 3)
                    {
                        cmbRole.IsEnabled = false;
                        btnEnable.IsEnabled = false;
                        if (txtAccount.Text != Global.Account)
                        {
                            txtName.IsEnabled = false;
                            this.btnOK.IsEnabled = false;
                        }
                    }




                    if (cmbRole.SelectedIndex <= Global.Role) btnEnable.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void DisCmbRole(int index)
        {

            ComboBoxItem item = cmbRole.ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItem;
            if (item != null)
                item.IsEnabled = false;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetDetail();
            acl();
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
                if (txtAccount.Text == "")
                {
                    Global.PromptFail("请输入账号");
                    isFail = true;
                    return;
                }
                if (txtName.Text == "")
                {
                    Global.PromptFail("请输入姓名");
                    isFail = true;
                    return;
                }
                if (cmbRole.Text == "")
                {
                    Global.PromptFail("请选择账号类别");
                    isFail = true;
                    return;
                }
                if (cmbRole.Text == "站长")
                {
                    string exiSql = "select count(*) from User where  Role = 2 and isEnabled=1"; //*
                    string rtn = Global.dbTrans.ExecuteScalar0(exiSql).ToString();
                    if (rtn != "0")
                    {
                        Global.PromptFail("最多只能激活一个站长");
                        isFail = true;
                        return;
                    }
                }
                if (cmbRole.Text == "管理员")
                {
                    Global.PromptFail("最多只能一个管理员");
                    isFail = true;
                    return;
                }
                if (UserID == "")
                {
                    string exiSql = "select count(*) from User where  account = '" + txtAccount.Text + "' "; //*
                    string rtn = Global.dbTrans.ExecuteScalar0(exiSql).ToString();
                    if (rtn != "0")
                    {
                        Global.PromptFail("已经存在该账号");
                        isFail = true;
                        return;
                    }
                }

                if (UserID == "")
                {
                    sql = "insert into  User (Account,Password,Name,Role,CreatedBy) values (";
                    sql += " '" + txtAccount.Text.Replace("'", "''") + "'";
                    sql += ", '" + Common.DESEncrypt("123456") + "'";
                    sql += ", '" + txtName.Text.Replace("'", "''") + "'";
                    sql += ", " + cmbRole.SelectedIndex;
                    sql += ", '" + Global.Account + "'";
                    sql += ")";
                }
                else
                {
                    sql = "update  User set ";
                    sql += " Name='" + txtName.Text.Replace("'", "''") + "'";
                    sql += ", Role=" + cmbRole.SelectedIndex;
                    sql += " where UserID=" + UserID;
                }
                Global.WriteDebugAsync("User edit" + sql, "user");
                Global.dbTrans.ExcuteScript(sql);

                if (UserID == "")
                {
                    Global.AddEvent("创建" + txtAccount.Text + "账号", 4);
                }
                else
                {
                    if (Name != txtName.Text)
                    {
                        Global.AddEvent("修改" + txtAccount.Text + "账号姓名为" + txtName.Text, 4);
                    }
                    if (Role != cmbRole.SelectedIndex)
                    {
                        Global.AddEvent("变更" + txtAccount.Text + "账号类型为" + cmbRole.Text, 4);
                    }

                }

                user.Show();
                Global.PromptSucc("账号信息保存成功");
                this.Close();
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]user-btnUserEidt_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail("更新账号信息失败" + ex.Message);
            }

        }

        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            string sql = "";
            try
            {
                if (cmbRole.Text == "站长" && IsEnabled == 0)
                {
                    string exiSql = "select count(*) from User where  Role = 2 and isEnabled=1"; //*
                    string rtn = Global.dbTrans.ExecuteScalar0(exiSql).ToString();
                    if (rtn != "0")
                    {
                        Global.PromptFail("最多只能激活一个站长");
                        return;
                    }
                }


                sql = " update   User  set isenabled=1- isenabled";
                sql += " where Userid=" + UserID;
                if (UserID != "")
                {
                    Global.dbTrans.ExcuteScript(sql);
                    Global.PromptSucc(EnableInfo + "账户信息成功");
                    Global.AddEvent(EnableInfo + txtAccount.Text + "账号", 4);

                    user.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]User-btnUserDel_MouseDown" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + sql);
                Global.PromptFail(EnableInfo + "账户信息成功" + ex.Message);
            }
        }

        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
                string exiSql = "select Account from User where  UserID=" + UserID;
                var obj = Global.dbTrans.ExecuteScalar0(exiSql);

                Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Password popupWindow = new Password();
                        popupWindow.UserID = UserID;
                        popupWindow.UserAccount = obj.ToString();
                        popupWindow.ShowDialog();
                    });
                });

                this.Close();

            }
            catch (Exception ex)
            {
                Global.WriteError(" btnPass_Click" + ",error:" + ex.Message + "," + ex.StackTrace);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
