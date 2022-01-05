using DigitalOfficePro.Html5PointSdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private PPTConvert pptConvertWnd;
        double wndX,wndY;
        public ProgressWindow(PPTConvert pptConvert)
        {
            InitializeComponent();
            pptConvertWnd = pptConvert;
        }

        private void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var p1 = new Point(Width / 2, Height / 2);
            this.Clip = new EllipseGeometry(p1, Width / 2, Height / 2);

            wndX = this.Left;
            wndY = this.Top;
        }

        private void showPPTConvertWnd()
        {
            if (pptConvertWnd != null)
            {
                pptConvertWnd.Hide();
                pptConvertWnd.Activate();
                pptConvertWnd.Show();
            }
        }

        private void StatusInfo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
            }
            if (Math.Abs(this.Top - wndY) <= 5 && Math.Abs(this.Left - wndX) <= 5)
            {
                showPPTConvertWnd();
            }
            wndX = this.Left;
            wndY = this.Top;
        }
    }
}
