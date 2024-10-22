using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace DESCADA
{
    /// <summary>
    /// QRCode.xaml 的交互逻辑
    /// </summary>
    public partial class QRCode : Window
    {

        public QRCode()
        {
            InitializeComponent();

            //#region 分屏显示
            //this.Loaded += (s, e) =>
            //{
            //    foreach (Screen scr in Screen.AllScreens)
            //    {
            //        //if (!scr.Primary)
            //        // {
            //        //设置窗体位置
            //        this.WindowStartupLocation = WindowStartupLocation.Manual;
            //        this.Left = -1920;// scr.WorkingArea.Left;
            //        this.Top = 0;// scr.WorkingArea.Top;
            //        //this.WindowState = WindowState.Maximized;
            //        // break;
            //        //}
            //    }
            //};
            //#endregion

            Global.SetShowScreen(this);
        }

        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="contents">要生成二维码包含的信息</param>
        /// <param name="width">生成的二维码宽度（默认300像素）</param>
        /// <param name="height">生成的二维码高度（默认300像素）</param>
        /// <returns>二维码图片</returns>
        //public static Bitmap GeneratorQrCodeImage(string contents, int width = 300, int height = 300)
        //{
        //    if (string.IsNullOrEmpty(contents))
        //    {
        //        return null;
        //    }

        //    EncodingOptions options = null;
        //    ZXing.BarcodeWriter writer = null;
        //    options = new QrCodeEncodingOptions
        //    {
        //        DisableECI = true,
        //        CharacterSet = "UTF-8",
        //        Width = width,
        //        Height = height,
        //        ErrorCorrection = ErrorCorrectionLevel.H,
        //        //控制二维码图片的边框
        //        Margin = 0
        //    };
        //    writer = new BarcodeWriter
        //    {
        //        Format = BarcodeFormat.QR_CODE,
        //        Options = options
        //    };

        //    Bitmap bitmap = writer.Write(contents);

        //    return bitmap;
        //}


        private void QRCreate()
        {
            var qrcode = new QRCodeWriter();
            EncodingOptions options = null;
            options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 272,
                Height = 272,
                ErrorCorrection = ErrorCorrectionLevel.H,
                //控制二维码图片的边框
                Margin = 0
            };
            //48CQCDC3N,SH1030,1
            // var qrValue = "097DADS_123,097DADS_123,1,沪B1N987D,LVHRE488885044118,20221012201212,001PBAV3000001C950700028:001PBAV3000001C950700029:001PBAV3000001C950700030";
            //var qrValue = "48CQCDC3N,SH1030,1,沪B1N987D,LVHRE488885044118,20221012201212,001PBAV3000001C950700028:001PBAV3000001C950700029:001PBAV3000001C950700030";
            //48CQCDC3N,SH1030,1,渝LD52419,LRWXCCEA7PC527016,20231204123000,CUNIL9LRX43IKLWSDEZR8PS8
            string OperatorID = Global.config.NomalConfig.OperatorNo; //"48CQCDC3N"
            var qrValue = OperatorID+",SH1030,1,渝LD52419,LRWXCCEA7PC527016,20221012201212,CUNIL9LRX43IKLWSDEZR8PS8";
            var barcodeWriter = new ZXing.Windows.Compatibility.BarcodeWriter       
              {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

        var writer = new ZXing.Windows.Compatibility.BarcodeWriter();

            using (var bitmap = barcodeWriter.Write(qrValue))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = stream;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                imgQR.Source = bi; //A WPF Image control
            }

        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            QRCreate();
        }


        private void Close_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //do something here
            this.Close();

            
        }
    }
}
