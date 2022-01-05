using System;
using System.Windows;
using System.Runtime.InteropServices;
using PPT = Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;

using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace IMOOC.EduCourses.PPTModule
{
    public class PPTOperation
    {
        #region "API usage declarations"


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern ulong GetWindowThreadProcessId(IntPtr hwnd, ulong ID);

        [DllImport("user32.dll")]
        public static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        [DllImport("user32.dll")]
        public static extern int CloseWindow(int hwnd);


        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int parentHandle, int childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags             // window-positioning options
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(
            Int32 hWnd,
            Int32 X,
            Int32 Y,
            Int32 nWidth,
            Int32 nHeight,
            bool Repaint
            );

        [DllImport("user32.dll", EntryPoint = "DrawMenuBar")]
        static extern Int32 DrawMenuBar(
            Int32 hWnd
            );
        [DllImport("user32.dll")]
        static extern int GetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        static extern Int32 GetMenuItemCount(
            Int32 hMenu
            );

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern Int32 GetSystemMenu(
            Int32 hWnd,
            bool hRevert
            );

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern Int32 RemoveMenu(
            Int32 hMenu,
            Int32 nPosition,
            Int32 wFlags
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;


        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOZORDER = 0x4;
        const int SWP_HIDEWINDOW = 0x0080;
        const int SWP_FRAMECHANGED = 0x0020;
        #endregion

        public List<PPT.Presentation> LPPTdocument;
        public PPT.Presentation document;
        public List<PPTPlayWindow> LPlayWin;
        public List<PPTWindow> LPPTWin;
        public List<string> PPTAddFileNameList; //用于回放，所有打开过的PPT，PPT不重复添加
        public List<string> PPTFlieName;   //PPT模块中正处于打开时的PPT
        public List<string> PPTNameList;  //用于回放，每当PPT打开或者切换，就将切换后的PPT名字存入其中
        public List<int> PPTPlayCurrSlideIndexList; //记录每次放映从第几页开始
       
        private int CVC;

        public int PPTCVC { get { return CVC; } }

        public PPT.Application PPT_App = null;


        public int PPTWnd = 0;
        public string filename = null;

        int Left, Top, Width, Height;
        //        private string PPTversion;
        private int currPPTPageIndex = 0;
        private int noOfPages = 0;

        bool powerpointOpened;
        bool playingPPT;
        bool IntoPPT;
        //  public object parentObject;
        private bool pptNumbExcFour;
        public bool PPTNumbExcFour
        {
            get { return pptNumbExcFour; }
        }
       

        public PPTOperation()
        {
            powerpointOpened = new bool();
            LPPTdocument = new List<PPT.Presentation>();
            LPlayWin = new List<PPTPlayWindow>();
            LPPTWin = new List<PPTWindow>();
            PPTFlieName = new List<string>();
            PPTNameList = new List<string>();
            PPTPlayCurrSlideIndexList = new List<int>();
            CVC = 0;
            IntoPPT = false;
            pptNumbExcFour = false;
            playingPPT = false;
            PPTAddFileNameList = new List<string>();


        }
        public void ClearOperation()
        {
            LPPTdocument.Clear();
            LPlayWin.Clear();
            LPPTWin.Clear();
            PPTAddFileNameList.Clear();
            PPTFlieName.Clear();
            PPTNameList.Clear();
            PPTPlayCurrSlideIndexList.Clear();
            CVC = 0;
            

        }
        public void CreatePPTApplication()
        {
            PPT_App = new PPT.Application();
            PPTWnd = PPT_App.HWND;
            PPT_App.SlideShowNextSlide += new PPT.EApplication_SlideShowNextSlideEventHandler(Application_SlideShowNextSlide);
            PPT_App.SlideShowNextClick += new PPT.EApplication_SlideShowNextClickEventHandler(Application_SlideShowNextClick);
        }

        public bool CheckPowerpointOpened()
        {
            try
            {
                PPT_App = (Microsoft.Office.Interop.PowerPoint.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application");
                powerpointOpened = true;
                return true;
            }
            catch
            {
                powerpointOpened = false;
                return false;
            }

        }
        public static bool CheckPowerpointInstalled()
        {
            Type officeType = Type.GetTypeFromProgID("Powerpoint.Application");
            if (officeType == null)
            { return false; }
            else { return true; }
        }

        public void loadPPT(string File)
        {
            if ((int)PPTWnd != 0)
            {
                if (!PPTAddFileNameList.Contains(File))
                {
                    PPTAddFileNameList.Add(File);
                }                        
                IntoPPT = false;
                if (CVC >= 3)
                {
                    pptNumbExcFour = true;
                    MessageBoxResult re = MessageBox.Show("本程序最多允许同时打开四个PPT，如要打开新的PPT，请关闭先前PPT程序", "注意！",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    if (re == MessageBoxResult.OK)
                    {
                        return;
                    }
                    else { return; }
                }

               
                var pptWin = new PPTWindow();
                var playWin = new PPTPlayWindow();
                PPT_App.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
                document = PPT_App.Presentations.Open(File, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue);

                int x = GetNoOfPages();
                AddBlankSlide();

                pptWin.Init((System.IntPtr)document.Application.HWND);
                int i = document.Application.HWND;
                LPPTWin.Add(pptWin);
                LPPTdocument.Add(document);
                CVC = LPPTdocument.Count-1;
                //-----------------------------------------------------------------万恶的分割线-----------------------------------------------------------------------------------

                document.SlideShowSettings.Run();

                playWin.Init((System.IntPtr)document.SlideShowWindow.HWND, 0);
                playingPPT = true;

                LPlayWin.Add(playWin);

                LPPTWin[CVC].Hide();
                LPlayWin[CVC].Hide();

               
                GotoSlide(1);

                UpdateCurrPageIndex();



                //document.Application.SlideShowNextClick += new PPT.EApplication_SlideShowNextClickEventHandler(Application_SlideShowNextClick);

                  //document.Application.SlideShowOnNext += new PPT.EApplication_SlideShowOnNextEventHandler(Application_SlideShowOnNext);

                  //document.Application.SlideShowBegin += new PPT.EApplication_SlideShowBeginEventHandler
                  //(Application_SlideShowBegin);
                  //                document.Application.SlideShowEnd += new PPT.EApplication_SlideShowEndEventHandler
                  //(Application_SlideShowEnd);*/

                //let the pptWin Window to handle the ppt application

                // document.SlideShowWindow.View.Last();
            }

        }

        private void AddBlankSlide()
        {
            PPT.CustomLayout customLayout =
            document.SlideMaster.CustomLayouts[PPT.PpSlideLayout.ppLayoutText];
            document.Slides.AddSlide(noOfPages + 1, customLayout);
        }

        public void SetPosition(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public void Application_SlideShowNextSlide(PPT.SlideShowWindow Pres)
        {
            UpdateCurrPageIndex();
            InitNoOfPages();
            if (currPPTPageIndex > noOfPages)
            {
                MessageBoxResult re = MessageBox.Show("已经是最后一张PPT，是否跳转到第一张？", "注意！",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                if (re == MessageBoxResult.OK)
                {
                    LPPTdocument[CVC].SlideShowWindow.View.GotoSlide(1);
                }
                else { LPPTdocument[CVC].SlideShowWindow.View.Previous(); }

            }

        }
        private void Application_SlideShowNextClick(PPT.SlideShowWindow Wn, PPT.Effect nEffect)
        {
            UpdateCurrPageIndex();

        }
        /*
                private void Application_SlideShowBegin(PPT.SlideShowWindow Wn)
        {
            //throw new NotImplementedException();
            PlayPPT();
            // MessageBox.Show("放映开始", "注意！", MessageBoxButtons.OK); 
        }
          private void Application_SlideShowNextClick(PPT.SlideShowWindow Wn, PPT.Effect nEffect)
               {
                     UpdateCurrPageIndex();

                        }
                        public void Application_SlideShowOnNext(PPT.SlideShowWindow wn)
                        {
           
                        }
                        public void Application_SlideShowEnd(PPT.Presentation Sel)
                        {

                        }

                        public void SetPenColor(System.Drawing.Color color)
                        {
                            // wd.SlideShowWindows[1].View.
                            //document.Application.ActivePresentation.SlideShowSettings.Run().View.PointerColor.RGB = color.ToArgb();
                            //Microsoft.Office.Interop.PowerPoint.PpSlideShowPointerType.
                        }*/

        private bool AppExist(System.IntPtr wnd)
        {
            if (!IsWindow((IntPtr)PPTWnd))
            {
                MessageBox.Show("Microsoft PowerPoint文档已经意外关闭！\n此PPT模块将关闭，即将为您切换到 白板 模块！", "错误！", MessageBoxButton.OKCancel);
                return false;
            }
            return true;
        }
        public void GotoSlide(int i)
        {
            
            LPPTdocument[CVC].SlideShowWindow.View.GotoSlide(i);
            LPlayWin[CVC].Hide();
            LPlayWin[CVC].Show();
            UpdateCurrPageIndex();
        }
        public void Next()
        {
            LPPTdocument[CVC].SlideShowWindow.Activate();
            LPPTdocument[CVC].SlideShowWindow.View.Next();


        }


        public void Previous()
        {
            if (playingPPT)
            {
                LPPTdocument[CVC].SlideShowWindow.Activate();

                if (LPPTdocument[CVC].SlideShowWindow.View.Slide.SlideIndex == 1)
                {
                    //MessageBox.Show(this, "已经是第一张PPT！", "注意！", MessageBoxButton.OK);
                    //MessageBox.Show(this, "已经是第一张PPT！");
                }
                else
                {
                    LPPTdocument[CVC].SlideShowWindow.View.Previous();
                    UpdateCurrPageIndex();
                }
                LPPTdocument[CVC].SlideShowWindow.Activate();
            }

        }
        private void UpdateCurrPageIndex()
        {
            currPPTPageIndex = LPPTdocument[CVC].SlideShowWindow.View.Slide.SlideIndex;
        }


        private int GetNoOfPages()
        {
            noOfPages = document.Slides.Count;
            return noOfPages;
        }
        private void InitNoOfPages()
        {
            if (LPPTdocument.Count==0)
            {
                return;
            }
            noOfPages = LPPTdocument[CVC].Slides.Count-1;
        }
        //----------------------------------------------------------
        public List<string> GetPPTImages()
        {
            string FileName = Path.GetFileNameWithoutExtension(PPTFlieName[CVC]);

            List<string> imageList = new List<string>();
            string tmp = System.IO.Path.GetTempPath();
            string str0;

            int i;
            float height = document.PageSetup.SlideHeight;
            float width = document.PageSetup.SlideWidth;

            if (Directory.Exists(tmp + "IMOOC" + "\\" + FileName))
            {
                //Directory.Delete(tmp + "IMOOC" + "\\" + FileName, true);
                for (i = 1; i < noOfPages + 1; i++)
                {
                    str0 = tmp + "IMOOC" + "\\" + FileName + "\\" + i.ToString() + ".jpg";
                    imageList.Add(str0);
                }
                return imageList;
            }
            else
            {
                Directory.CreateDirectory(tmp + "IMOOC" + "\\" + FileName);
                for (i = 1; i < noOfPages + 1; i++)
                {
                    str0 = tmp + "IMOOC" + "\\" + FileName + "\\" + i.ToString() + ".jpg";
                    LPPTdocument[CVC].Slides[i].Export(str0, "jpg", (int)width, (int)height);
                    imageList.Add(str0);
                }
                return imageList;
            }

        }



        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }
        //-------------------------------------------
        public int GetCurrPageIndex()
        {
            return currPPTPageIndex;
        }

        public void ClosePPT()
        {
            LPPTdocument[CVC].SlideShowWindow.View.Exit();

            int j = LPPTdocument[CVC].Windows.Count;
            for (int i = 1; i <= j; i++)
            {
                LPPTdocument[CVC].Windows[i].Close();

            }
            LPPTdocument.Remove(LPPTdocument[CVC]);
            LPlayWin.Remove(LPlayWin[CVC]);
            LPPTWin.Remove(LPPTWin[CVC]);           
        }

        public void ClosePPT(int cvc)
        {
            LPPTdocument[cvc].SlideShowWindow.View.Exit();

            int j = LPPTdocument[cvc].Windows.Count;
            for (int i = 1; i <= j; i++)
            {
                LPPTdocument[cvc].Windows[i].Close();

            }
            LPPTdocument.Remove(LPPTdocument[cvc]);
            LPlayWin.Remove(LPlayWin[cvc]);
            LPPTWin.Remove(LPPTWin[cvc]);
        }

        public void ActivateSlideShowWindow()
        {
            LPPTdocument[CVC].SlideShowWindow.Activate();
            LPlayWin[CVC].Show();
        }

        /*       public void EnablePen()
 //       {
 //            document.SlideShowWindow.Activate();
 //            System.Windows.Forms.SendKeys.SendWait("^p");
 //        }

        public void EnableHighlighter()
        {
            document.SlideShowWindow.Activate();
            System.Windows.Forms.SendKeys.SendWait("^i");
        }*/



        public void KillBackgroundPPT()
        {

            Process[] myProgress;
            myProgress = Process.GetProcesses();　　　　　　　　　　//获取当前启动的所有进程
            foreach (Process p in myProgress)　　　　　　　　　　　　//关闭当前启动的Excel进程
            {
                if (p.ProcessName == "POWERPNT")　　　　　　　　　　//通过进程名来寻找
                {
                    p.Kill();
                    return;
                }
            }
        }

        public void PPTHideWin()
        {
            for (int i = 0; i < LPlayWin.Count; i++)
            {
                LPPTWin[i].Hide();
                LPlayWin[i].Hide();
            }
        }
        public void ChangePPT()
        {
            for (int i = 0; i < LPlayWin.Count; i++)
            {
                LPPTWin[i].Hide();
                LPlayWin[i].Hide();
            }
            LPlayWin[CVC].Show();
        }

        public void ChangePPT(int cvc)
        {

            if (IntoPPT)
            {
                CVC = cvc;
                for (int i = 0; i < LPlayWin.Count; i++)
                {
                    LPPTWin[i].Hide();
                    LPlayWin[i].Hide();
                }

            }
            else
            {
                LPlayWin[cvc].Hide();
            }

            document = LPPTdocument[cvc];
            IntoPPT = true;
        }


        public void PPTUpdataLocation(System.Drawing.Point PlayWtop)
        {
            LPlayWin[CVC].updatalocation(PlayWtop);
        }

        public void Save(BinaryWriter bw, string directory)
        {

            bw.Write(PPTAddFileNameList.Count);
            for (int i = 0; i < PPTAddFileNameList.Count; i++)
            {
                File.Copy(PPTAddFileNameList[i], directory + "\\" + Path.GetFileName(PPTAddFileNameList[i]), true);
                bw.Write(Path.GetFileName(PPTAddFileNameList[i]));
            }

        }

        public void Open(BinaryReader br, string directory)
        {
            int count = br.ReadInt32();
            PPTAddFileNameList = new List<string>();
            for (int i = 0; i < count; i++)
            {
                PPTAddFileNameList.Add(directory + br.ReadString());
            }

        }

        private void CopyPPTFile(string SaveFileName)
        {

        }

        public void FillAllPPT()
        {
            CreatePPTApplication();
            CheckPowerpointOpened();
            for (int i = 0; i < PPTAddFileNameList.Count; i++)
            {
                loadPPT(PPTAddFileNameList[i]);
            }
            CVC=0;
            InitNoOfPages();
        }
        public void AddPPTNameList()
        {
            PPTNameList.Add(LPPTdocument[CVC].Name.ToString());
        }

        public void ReplayShowPPT(string PPTName)
        {
            for (int i = 0; i < LPPTdocument.Count; i++)
            {
                PPTHideWin();
                if (LPPTdocument[i].Name.ToString()==PPTName)
                {
                    LPlayWin[i].Activate();
                    LPlayWin[i].Show();
                    return;
                }
            }
        }

        public int ReplayReturnPPTIndex(string PPTName)
        {
            int Index=new int();
            for (int i = 0; i < LPPTdocument.Count; i++)
            {              
                if (LPPTdocument[i].Name.ToString() == PPTName)
                {
                    Index = i;
                }
            }
            return Index;
        }

        public void ChangCVC(string PPTName)
        {
           CVC= ReplayReturnPPTIndex(PPTName);

        }

    }


}
