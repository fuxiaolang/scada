using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
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

using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Xceed.Wpf.AvalonDock.Converters;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DESCADA.Controls
{
    /// <summary>
    /// Pagination.xaml 的交互逻辑
    /// </summary>
    public partial class Pagination : System.Windows.Controls.UserControl
    {
        public event EventHandler<DataEventArgs> SendDataEvent;                                                     
        private int page = 1;// 当前页码
        public int Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
                this.txtPage.Text = value.ToString();
            }
        }

        public int PageSize = 20; // 每页显示的记录数

        private int recordSize;
        public int RecordSize
        {
            get
            {
                return recordSize;
            }
            set
            {
                recordSize = value;
                this.txtRecordSize.Text = recordSize.ToString();
                this.txtPageNum.Text = CalculateTotalPages(recordSize, PageSize).ToString();
                SetBtns();
            }
        }

        public Pagination()
        {
            InitializeComponent();
            //this.txtRecordSize.Text = RecordSize.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int page = 1;// 当前页码
            int pageSize = 20; // 每页显示的记录数
            int recordSize = 200;

            OnSendDataEvent(new DataEventArgs(page, pageSize));
        }

        protected virtual void OnSendDataEvent(DataEventArgs e)
        {
            SendDataEvent?.Invoke(this, e);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Page++;
            OnSendDataEvent(new DataEventArgs(Page, PageSize));
        }
        private void btnNum_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Controls.Button But = sender as System.Windows.Controls.Button;
            if (But != null)
            {
                string strNum = But.Content.ToString();
                if (strNum == "...")
                {
                    Page = Page - 3;
                }
                else
                {
                    int num = Int32.Parse(But.Content.ToString());
                    Page = num;
                }
                OnSendDataEvent(new DataEventArgs(Page, PageSize));
            }

        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            Page--;
            OnSendDataEvent(new DataEventArgs(Page, PageSize));
        }

        private void cmbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPageSize.SelectedItem != null)
            {
                string selectedContent = (cmbPageSize.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();
                int selectedValue = Int32.Parse(selectedContent.Replace(" 条/页", ""));
                Page = 1;
                PageSize = selectedValue;
                OnSendDataEvent(new DataEventArgs(Page, PageSize));

            }

        }

        private void SetBtns()
        {
            int pageNum = CalculateTotalPages(recordSize, PageSize);
            if (this.page <= 1) this.btnLast.Visibility = Visibility.Collapsed; else this.btnLast.Visibility = Visibility.Visible;
            if (this.page == pageNum) this.btnNext.Visibility = Visibility.Collapsed; else this.btnNext.Visibility = Visibility.Visible;

            btn1.Visibility = Visibility.Visible;
            btn2.Visibility = Visibility.Visible;
            btn3.Visibility = Visibility.Visible;
            btn4.Visibility = Visibility.Visible;
            btn5.Visibility = Visibility.Visible;
            btn6.Visibility = Visibility.Visible;
            btn7.Visibility = Visibility.Visible;
            btn1.Content = "1";
            btn2.Content = "2";
            btn3.Content = "3";
            btn4.Content = "4";
            btn5.Content = "5";
            btn6.Content = "6";
            btn7.Content = "7";

            System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(0xFF, 0x01, 0xFE, 0xFB);
            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(color);

            System.Windows.Media.Color colorNormal = System.Windows.Media.Color.FromArgb(0xFF, 0x04, 0x1D, 0x35);
            System.Windows.Media.SolidColorBrush brushNormal = new System.Windows.Media.SolidColorBrush(colorNormal);

            btn1.Background = brushNormal;
            btn2.Background = brushNormal;
            btn3.Background = brushNormal;
            btn4.Background = brushNormal;
            btn5.Background = brushNormal;
            btn6.Background = brushNormal;
            btn7.Background = brushNormal;

            if (pageNum <= 7)
            {
                switch (pageNum)
                {
                    case 1:
                        btn2.Visibility = Visibility.Collapsed;
                        btn3.Visibility = Visibility.Collapsed;
                        btn4.Visibility = Visibility.Collapsed;
                        btn5.Visibility = Visibility.Collapsed;
                        btn6.Visibility = Visibility.Collapsed;
                        btn7.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        btn3.Visibility = Visibility.Collapsed;
                        btn4.Visibility = Visibility.Collapsed;
                        btn5.Visibility = Visibility.Collapsed;
                        btn6.Visibility = Visibility.Collapsed;
                        btn7.Visibility = Visibility.Collapsed;
                        break;
                    case 3:
                        btn4.Visibility = Visibility.Collapsed;
                        btn5.Visibility = Visibility.Collapsed;
                        btn6.Visibility = Visibility.Collapsed;
                        btn7.Visibility = Visibility.Collapsed;
                        break;
                    case 4:
                        btn5.Visibility = Visibility.Collapsed;
                        btn6.Visibility = Visibility.Collapsed;
                        btn7.Visibility = Visibility.Collapsed;
                        break;
                    case 5:
                        btn6.Visibility = Visibility.Collapsed;
                        btn7.Visibility = Visibility.Collapsed;
                        break;
                    case 6:
                        btn7.Visibility = Visibility.Collapsed;
                        break;
                }

                switch (page)
                {
                    case 1:
                        btn1.Background = brush;
                        break;
                    case 2:
                        btn2.Background = brush;
                        break;
                    case 3:
                        btn3.Background = brush;
                        break;
                    case 4:
                        btn4.Background = brush;
                        break;
                    case 5:
                        btn5.Background = brush;
                        break;
                    case 6:
                        btn6.Background = brush;
                        break;
                    case 7:
                        btn7.Background = brush;
                        break;
                }
            }
            else
            {
                if (page < 6)
                {

                    switch (page)
                    {
                        case 1:
                            btn1.Background = brush;
                            break;
                        case 2:
                            btn2.Background = brush;
                            break;
                        case 3:
                            btn3.Background = brush;
                            break;
                        case 4:
                            btn4.Background = brush;
                            break;
                        case 5:
                            btn5.Background = brush;
                            break;
                    }
                }
                else if (page >= 6)
                {
                    btn2.Content = "...";
                    btn3.Content = page - 2;
                    btn4.Content = page - 1;
                    btn5.Content = page;
                    btn5.Background = brush;
                    btn5.Background = brush;
                    btn6.Content = page + 1;
                    btn7.Content = page + 2;

                    //大于总页数的不显示；
                    if (page + 1 > pageNum)
                    {
                        btn6.Visibility = Visibility.Collapsed;
                        btn7.Visibility = Visibility.Collapsed;
                    }
                    if (page + 2 > pageNum)
                    {
                        btn7.Visibility = Visibility.Collapsed;
                    }
                }

            }

        }

        public int CalculateTotalPages(int totalRecords, int recordsPerPage)
        {
            if (recordsPerPage == 0)
                throw new DivideByZeroException("每页记录数不能为0");

            int totalPages;

            if (totalRecords % recordsPerPage == 0)
            {
                totalPages = totalRecords / recordsPerPage;
            }
            else
            {
                totalPages = totalRecords / recordsPerPage + 1;
            }

            return totalPages;
        }

        private void btn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(0xFF, 0x01, 0xFE, 0xFB);
            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(color);

            System.Windows.Controls.Button But = sender as System.Windows.Controls.Button;
            if (But != null)
            {
                But.Background = brush;
            }

        }

        private void btn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(0xFF, 0x04, 0x1D, 0x35);
            System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(color);

            System.Windows.Controls.Button But = sender as System.Windows.Controls.Button;


            if (But != null && But.Content.ToString() != page.ToString())
            {
                But.Background = brush;
            }

        }
    }
    public class DataEventArgs : EventArgs
    {
        public int Page;// 当前页码
        public int PageSize; // 每页显示的记录数

        public DataEventArgs(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
