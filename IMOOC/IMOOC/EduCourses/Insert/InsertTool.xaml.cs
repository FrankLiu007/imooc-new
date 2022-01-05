using IMOOC.EduCourses.Utils;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IMOOC.EduCourses.Insert
{
    /// <summary>
    /// Interaction logic for InsertTool.xaml
    /// </summary>
    public partial class InsertTool : UserControl
    {
        private InsertCanvas insert;

        public InsertTool()
        {
            InitializeComponent();
            insert = new Insert.InsertCanvas();
        }

        private void OpenImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "图片格式|*.jpg;*.png;*.bmp;*.jpeg";

            if (ofd.ShowDialog() == true)
            {
                string PicName =Path.GetDirectoryName(ofd.FileName)+"\\"+ Path.GetFileNameWithoutExtension(ofd.FileName);
                PicName=PicName.Replace("\\", "");
                PicName = PicName.Replace(":", "")+".jpg"; 

                string PicPath = CourseControl.CourseSavePath + "\\Resource\\picture\\";
                if (!Directory.Exists(PicPath))
                {
                    Directory.CreateDirectory(PicPath);
                }
                var destFileName = PicPath + PicName;
                try
                {                    
                    if (!File.Exists(destFileName))
                    {
                        if (Path.GetExtension(ofd.FileName).ToLower()==".bmp")
                        {
                            if (!Public.HelperMethods.CompressPic(ofd.FileName, destFileName, 75))
                            {
                                MessageWin message = new MessageWin(MessageWinType.Prompt, "插入图片时发生错误！");
                                message.ShowDialog();
                                Public.Log.WriterLog("转换Bmp图片时发生错误！");
                                return;
                            }                           
                        }
                        else
                        {
                            File.Copy(ofd.FileName, destFileName);
                        }                        
                        Public.Log.FileOrFolder(Public.LogType.CopeFile, ofd.FileName + "  to  " + destFileName);
                    }                                            
                }
                catch(Exception ex)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "插入图片时发生错误！");
                    message.ShowDialog();
                    Public.Log.WriterLog(ex.Message + "插入图片时发生错误！");
                    return;
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                Uri imageSource = new Uri(destFileName);
                image.UriSource = imageSource;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

               // ImageSource image = new BitmapImage(new Uri(destFileName));
                Image img = new Image();
                img.Source = image;
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

                (DataContext as CoursesViewModel).CourseCtrl.GotoBanshu();
                (DataContext as CoursesViewModel).BanshuMenu();

                var tt = new InkCanvasChild() { X = 100, Y = 100, UiEle = img };
                (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.AllChild.Add(tt);
                (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.Select(new UIElement[] { img });

            }
        }

        private void insertCanvasBtn_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ToolGrid.Children.Add(insert);
        }

    }
}
