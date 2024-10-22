using DESCADA.Service;
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
using System.Windows.Shapes;

namespace DESCADA
{
    /// <summary>
    /// Plateno.xaml 的交互逻辑
    /// </summary>
    public partial class Plateno : Window
    {
        public Plateno()
        {
            InitializeComponent();
            Global.SetShowScreen(this);
        }

    
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.WindowState = WindowState.Maximized;
            //this.txtPlateNo.Text = Global.PlateNO;
            this.txtPlateNo.Dispatcher.Invoke(new Action(() => { this.txtPlateNo.Text = Global.PlateNO; }));

        }

        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //do something here
            this.Close();            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Global.PlateNO = this.txtPlateNo.Text.Replace(" ","");
            Global.PlateNOTimerInNum = 0;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
