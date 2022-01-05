using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace ScreenCaptureLib
{
    /// <summary>
    /// Interaction logic for Indicater.xaml
    /// </summary>
    using Public;
    using IMOOC;
    using IMOOC.EduCourses;
    using Microsoft.Win32;
    using System.Windows.Media.Imaging;
    using System.IO;
    using System;

    public partial class Indicater : UserControl
    {
        public object parentMaskWindow;
        public object CourseCtrl;
        public Indicater()
        {
            InitializeComponent();
        }

        private void CloseMaskWindow(object sender, RoutedEventArgs e)
        {
            (parentMaskWindow as MaskWindow).Close();
        }

        private void InsertToWhiteBoard(object sender, RoutedEventArgs e)
        {
            MainWindow win = App.Current.MainWindow as MainWindow;
            var src = (parentMaskWindow as MaskWindow).GetSelectionImage();
            string PicName = CourseControl.CourseSavePath.Replace("\\", "");
            PicName = PicName.Replace(":", "");
            var path = CourseControl.CourseSavePath + "Resource\\picture\\" + PicName+ "snip" + (CourseCtrl as CourseControl).PicCount.ToString() + ".jpg";
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            try
            {
                string picPath = CourseControl.CourseSavePath + "Resource\\picture\\";
                if (!Directory.Exists(picPath))
                {
                    Directory.CreateDirectory(picPath);
                }
                var fs = File.OpenWrite(path);
                encoder.Save(fs);
                (CourseCtrl as CourseControl).PicCount++;
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog(ex.Message);
            }
            

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            Uri imageSource = new Uri(path);
            bitmap.UriSource = imageSource;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            Image img = new Image();
            img.Source = bitmap;
            img.Stretch = Stretch.Fill;
            double dWidth = SystemParameters.PrimaryScreenWidth;
            double dHeight = SystemParameters.PrimaryScreenHeight;
            double ratio = 0;
            if (img.Source.Height >= (img.Source.Width * 2))
            {
                if (img.Source.Width >= (dWidth / 4) || img.Source.Width <= (dWidth / 20))
                {
                    ratio = (dWidth / 4) / img.Source.Width;
                    img.Width = (dWidth / 4);
                    img.Height = img.Source.Height * ratio;
                }
                else
                {
                    img.Width = img.Source.Width;
                    img.Height = img.Source.Height;
                }
            }
            else
            {
                if (img.Source.Height >= (dHeight / 4) || img.Source.Height <= (dHeight / 20))
                {
                    ratio = (dHeight / 4) / img.Source.Height;
                    img.Height = (dHeight / 4);
                    img.Width = img.Source.Width * ratio;
                }
                else
                {
                    img.Width = img.Source.Width;
                    img.Height = img.Source.Height;
                }
            }
            var tt = new InkCanvasChild() { X = 100, Y = 100, UiEle = img };


            (CourseCtrl as CourseControl).GotoBanshu();
            (CourseCtrl as CourseControl).viewModel.BanshuMenu();
            (CourseCtrl as CourseControl).myInkCanvas.AllChild.Add(tt);
            (CourseCtrl as CourseControl).myInkCanvas.Select(new UIElement[] { img });
            (parentMaskWindow as MaskWindow).Close();
        }

        //private void CopyMaskWindow(object sender, RoutedEventArgs e)
        //{
        //    ImageSource src = (parentMaskWindow as MaskWindow).GetSelectionImage();
        //    Image img = new Image();
        //    img.Source = src;
        //    img.Stretch = Stretch.Fill;
        //    var tt = new InkCanvasChild() { X = 100, Y = 100, UiEle = img };                        
        //    (CourseCtrl as CourseControl).viewModel.CurrPage.AllChild.Add(tt);            
        //    (parentMaskWindow as MaskWindow).Close();
        //    (CourseCtrl as CourseControl).GotoEdit();
        //    (CourseCtrl as CourseControl).myInkCanvas.Select(new UIElement[] { img });
        //    (CourseCtrl as CourseControl).ButtonCopySelection();
        //}
    }
}
