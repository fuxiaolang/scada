using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.DataFormats;

namespace DESCADA
{
    /// <summary>
    /// Switch.xaml 的交互逻辑
    /// </summary>
    public partial class Track : System.Windows.Controls.UserControl
    {
        public string currentMenu = "btnAlarmRecord";
        public Track()
        {
            InitializeComponent();

            Alarm pew = new Alarm();
            Page_Config.Content = pew;
        }

        private void btnAlarmRecord_Click(object sender, RoutedEventArgs e)
        {
            Alarm pew = new Alarm();
            Page_Config.Content = pew;

            SetMenu("btnAlarmRecord");
        }

        private void bntEventRecord_Click(object sender, RoutedEventArgs e)
        {
            Event pew = new Event();
            Page_Config.Content = pew;

            SetMenu("bntEventRecord");
        }

        private void SetMenu(string selMenu)
        {
            try
            {

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources/圆角矩形2拷贝5-1.png", UriKind.Relative));
                imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式


                ImageBrush imageCurrentBrush = new ImageBrush();
                imageCurrentBrush.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources/手动充电按钮选中.png", UriKind.Relative));
                imageCurrentBrush.Stretch = Stretch.Fill;//设置图像的显示格式


                ((System.Windows.Controls.Button)this.FindName(currentMenu)).Background = imageCurrentBrush;
                ((System.Windows.Controls.Button)this.FindName(selMenu)).Background = imageBrush;

                currentMenu = selMenu;
            }
            catch (Exception ex)
            {
                Global.WriteError("[Error]" + ex.Message + "\r\n" + ex.StackTrace);
            }
              

        }
    }
}
