using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IMOOC.EduCourses.PPTModule
{
    public partial class PPTPlayWindow : Form
    {
        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter,
                    int X, int Y, int cx, int cy, uint uFlags);

        private const int HWND_TOP = 0x0;
        private const int WM_COMMAND = 0x0112;
        private const int WM_QT_PAINT = 0xC2DC;
        private const int WM_PAINT = 0x000F;
        private const int WM_SIZE = 0x0005;
        private const int SWP_FRAMECHANGED = 0x0020;


        private IntPtr playWin;
       

        public Point location;
        public PPTPlayWindow()
        {
            InitializeComponent();
            

        }
        public void Init(System.IntPtr win, int top)
        {


            playWin=win ;

            SetParent(playWin.ToInt32(), this.Handle.ToInt32());

            SetThisPos(top);

            ResizeWindow();

           
           
        }
        private void SetThisPos(int top)
        {
            int  h = Screen.AllScreens[0].Bounds.Height;
            int  w = Screen.AllScreens[0].Bounds.Width;
            this.Left = 0;
            this.Top = top;
            this.Width = w;
            this.Height = h - top;

        }

        private void ResizeWindow()
        {
            SendMessage(playWin, WM_COMMAND, WM_PAINT, 0);
            PostMessage(playWin, WM_QT_PAINT, 0, 0);

            SetWindowPos(
           playWin,
               HWND_TOP,
               0,//设置偏移量,把原来窗口的菜单遮住
             0 ,
              this.Width,
               this.Height,
               SWP_FRAMECHANGED);

            SendMessage(playWin, WM_COMMAND, WM_SIZE, 0);
                
        }

        public void updatalocation(Point location)
        {
            Location = location;
            this.Height = Height - location.Y;
            
        }

        private void PPTPlayWindow_Resize(object sender, EventArgs e)
        {
            ResizeWindow();
            
        }

        private void PPTPlayWindow_Activated(object sender, EventArgs e)
        {
         //   ()playWin
        }
    }
}
