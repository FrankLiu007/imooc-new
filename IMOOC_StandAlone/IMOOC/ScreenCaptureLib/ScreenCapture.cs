using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace ScreenCaptureLib
{


    class ScreenCapture
    {
 //       [System.Runtime.InteropServices.DllImport("gdi32.dll")]
 //       public static extern bool DeleteObject(IntPtr hObject);


        public  ScreenCapture()
        {
            //screenBmp = CopyScreen();
        }

        public static BitmapImage CopyScreen()
        {

            Bitmap screenBmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            var bmpGraphics = Graphics.FromImage(screenBmp);
            IntPtr hBitmap = screenBmp.GetHbitmap(); 

            bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);


            BitmapImage bmp = Bitmap2BitmapImage(screenBmp);


            return bmp;

        }
        private static BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }
        private static BitmapSource Bitmap2BitmapSource(System.Drawing.Bitmap bitmap)
        {/*
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource  
            DeleteObject(ptr);*/

            return null;
        }  


    }
}
