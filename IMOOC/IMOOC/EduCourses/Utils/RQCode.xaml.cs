using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThoughtWorks.QRCode.Codec;

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for RQCode.xaml
    /// </summary>
    public partial class RQCode : Window
    {
        string _link;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseName">回放的课程名</param>
        /// <param name="link">要生成的连接</param>
        public RQCode(string courseName,string link)
        {
            InitializeComponent();
            titleLable.Content = courseName;
            _link = link;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string ipAddress = GetIpAddress();           
            if (ipAddress!=null)
            {
                string link = _link.Replace("localhost", ipAddress);
                BitmapImage bitmapImage= GetDimensionalCode(link);
                if (bitmapImage!=null)
                {
                    image.Source = bitmapImage;
                }
                else
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "生成二维码失败");
                    message.ShowDialog();
                    Public.Log.WriterLog("生成二维码失败");
                }               
            }
            else
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "未能获取到本地IP");
                message.ShowDialog();
                Public.Log.WriterLog("未能获取到本地IP");
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string GetIpAddress()
        {
            int i = 0;
            try
            {
                IPAddress[] localIPs;
                localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress ip in localIPs)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) //判断是否ipv4 InterNetwork是ipv4 InterNetWorkV6是ipv6
                        return ip.ToString();
                    i++;
                }
            }
            catch (Exception exceptionERR)
            {
                MessageBox.Show("Error: " + exceptionERR.Message);
            }

            return null;

            //string hostName = Dns.GetHostName();   //获取本机名
            ////IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            //IPAddress localaddr = localhost.AddressList[0];
            //return localaddr.ToString();
        }

        /// <summary>
        /// 根据链接获取二维码
        /// </summary>
        /// <param name="link">链接</param>
        /// <returns>返回二维码图片</returns>
        private BitmapImage GetDimensionalCode(string link)
        {
            System.Drawing.Bitmap bmp = null;
            BitmapImage bitmapImage = null;
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 4;
                //int version = Convert.ToInt16(cboVersion.Text);
                qrCodeEncoder.QRCodeVersion = 7;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                //qrCodeEncoder.Encode(link);
                bmp = qrCodeEncoder.Encode(link);

                using (MemoryStream memory = new MemoryStream())
                {
                    bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

            }
            catch (Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "生成二维码失败");
                message.ShowDialog();
                Public.Log.WriterLog("生成二维码失败" + ex.Message);
            }
            return bitmapImage;
        }
    }
}
