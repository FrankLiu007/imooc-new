using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IMOOC.EduCourses.PPTModule
{
    public partial class PPTWindow : Form
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


        System.IntPtr pptWindowHandle;

        public PPTWindow()
        {
            InitializeComponent();
        }
        public void Init(System.IntPtr ppt)
        {
            pptWindowHandle = ppt;
            SetParent(pptWindowHandle.ToInt32(), this.Handle.ToInt32());

          //  ResizeWindow();
        }
        private void ResizeWindow()
        {
            SendMessage(pptWindowHandle, WM_COMMAND, WM_PAINT, 0);
            PostMessage(pptWindowHandle, WM_QT_PAINT, 0, 0);

            SetWindowPos(
           pptWindowHandle,
               HWND_TOP,
               0 ,//设置偏移量,把原来窗口的菜单遮住
             0 - 100,
              this.Width ,
               this.Height ,
               SWP_FRAMECHANGED);

            SendMessage(pptWindowHandle, WM_COMMAND, WM_SIZE, 0);
        }

        private void PPTWindow_Resize(object sender, EventArgs e)
        {
         //   ResizeWindow();
        }
    }
}
