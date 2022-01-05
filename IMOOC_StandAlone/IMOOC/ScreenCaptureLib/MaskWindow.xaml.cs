using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenCaptureLib
{
    /// <summary>
    /// Interaction logic for MaskWindow.xaml
    /// </summary>
    public partial class MaskWindow : Window
    {
        BitmapImage bmp;
        Point drawRectPoint;
        bool start;
        Rect selectRect;
        public Indicater indicater;
        

        public MaskWindow()
        {
            
           InitializeComponent();

           bmp= ScreenCapture.CopyScreen();
           this.Background = new ImageBrush(bmp);
           start = false;

           indicater = new Indicater();
           indicater.parentMaskWindow = this;
           MainMask.Children.Add(indicater);           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LeftMask.Width = ActualWidth;
            LeftMask.Height = ActualHeight;

            indicater.Width = 0;
            indicater.Height = 0;
        }

        public BitmapSource GetSelectionImage()
        {
            double scaleX = bmp.PixelHeight / ActualHeight, scaleY = bmp.PixelWidth / ActualWidth;

            int x = Convert.ToInt32(selectRect.X * scaleX);
            int y = Convert.ToInt32(selectRect.Y * scaleY);
            int w = Convert.ToInt32(selectRect.Width * scaleX);
            int h = Convert.ToInt32(selectRect.Height * scaleY);

            return new CroppedBitmap(bmp, new Int32Rect(x, y, w, h));
        }

        private void MainMask_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawRectPoint = e.GetPosition(this);
            start = true;
        }

        private void MainMask_MouseUp(object sender, MouseButtonEventArgs e)
        {
            start = false;

        }

        private void MainMask_MouseMove(object sender, MouseEventArgs e)
        {
            if (start && e.LeftButton==MouseButtonState.Pressed)
            {
                Point p1 = e.GetPosition(this);
                selectRect = new Rect(drawRectPoint, p1);

                UpdateMask(selectRect);
                UpdateIndicater(selectRect);
            }
        }

        public void UpdateSelectRegion(Rect rect)
        {
            selectRect = rect;
            UpdateIndicater(rect);
            UpdateMask(rect);
        }

        public void UpdateIndicater(Rect rect)
        {
            indicater.Width = rect.Width;
            indicater.Height = rect.Height;
            Canvas.SetLeft(indicater, rect.Left);
            Canvas.SetTop(indicater, rect.Top);           
        }

        public void UpdateMask(Rect rect)
        {
            LeftMask.Height = ActualHeight;
            LeftMask.Width = rect.Left;
            Canvas.SetLeft(LeftMask,0);
            Canvas.SetTop(LeftMask, 0);

            RightMask.Height = ActualHeight;
            RightMask.Width = ActualWidth - rect.Right;
            Canvas.SetLeft(RightMask, rect.Right);
            Canvas.SetTop(RightMask, 0);

            BottomMask.Width =rect.Width;
            BottomMask.Height = ActualHeight -rect.Bottom;
            Canvas.SetTop(BottomMask, rect.Bottom);
            Canvas.SetLeft(BottomMask, rect.Left);

            TopMask.Width = rect.Width;
            TopMask.Height = rect.Top;
            Canvas.SetLeft(TopMask, rect.Left);
            Canvas.SetTop(TopMask, 0);
        }
    }
}
