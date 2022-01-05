using IMOOC.EduCourses.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IMOOC.EduCourses
{
    /// <summary>
    /// Interaction logic for RecourdTool.xaml
    /// </summary>
    public partial class RecourdTool : UserControl
    {
        private CoursesViewModel viewModel;
        public RecourdTool()
        {
            InitializeComponent();
        }        

        private void StartRecord_Click(object sender, RoutedEventArgs e)
        {

            if (!viewModel.CourseCtrl.IsInitRecorder)
            {
                try
                {
                    viewModel.CourseCtrl.mediaRecorder = new Recorder.MediaRecorder();
                    viewModel.CourseCtrl.mediaRecorder.InitMediaRecorder();
                    viewModel.CourseCtrl.IsInitRecorder = true;
                }
                catch (Exception ex)
                {
                    MessageWin message = new MessageWin(MessageWinType.Error, "初始化录制时发生错误！");
                    message.ShowDialog();
                    Public.Log.WriterLog("初始化录制时发生错误!" +ex.Message);
                    return;
                }
                
            }
            Recorder.RecorderWin recorderWnd;
            recorderWnd = new Recorder.RecorderWin(viewModel.CourseCtrl.mediaRecorder, viewModel.CurrCourse);
            recorderWnd.Activate();
            recorderWnd.ShowDialog();

            viewModel.CourseCtrl.mediaRecorder = recorderWnd.Recorder;
            if (recorderWnd.IsStart)
            {
                if (recorderWnd.IsClearBanshu)
                {
                    viewModel.CleraBanshu();
                }
                if (recorderWnd.IsCloseAllOldCourse)
                {
                    viewModel.CloseAllOldCourse();
                }

                RedirectPPTSource();
                viewModel.InitStartRecord();
                viewModel.CourseCtrl.myInkCanvas.StartRecord();
                viewModel.SetAllCourseName();
                foreach (var item in viewModel.AllCourse)
                {
                    if (isBeforeContent(item))
                    {
                        viewModel.SaveAddedOldCourse(item);
                    }
                    foreach (var PPT in item.AllPPT)
                    {
                        PPT.Value.isNewOpen = false;
                    }
                }
                StartRecord.IsEnabled = false;
                PauseRecord.IsEnabled = true;
                EndRecord.IsEnabled = true;
            }

        }

        private void RedirectPPTSource()
        {
            if (viewModel.CurrCourse.IsNew==true)
            {
                if (viewModel.CurrCourse.AllPPT != null && viewModel.CurrCourse.AllPPT.Count != 0)
                {
                    foreach (var item in viewModel.CurrCourse.AllPPT)
                    {
                        string path = viewModel.CurrCourse.CoursePath + viewModel.CurrCourse.Name + "\\Resource\\ppt\\" + item.Key + "\\" + item.Key + ".html";
                        item.Value.browser.Source = new Uri(@path);
                    }
                }
            }                        
        }

        /// <summary>
        /// 判断在录制之前,是否存在板书PPT或者打开的旧课等内容
        /// </summary>
        /// <returns></returns>
        private bool isBeforeContent(Course course)
        {
            foreach (var page in course.AllPage)
            {
                if (page.AllChild.Count > 0 || page.Strokes.Count > 0)
                {
                    return true;
                }
            }
            if (course.AllPPT != null && course.PPTNameList.Count > 0)
            {
                return true;
            }

            return false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = DataContext as CoursesViewModel;
        }

        private void EndRecord_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CourseCtrl.mediaRecorder != null)
            {
                viewModel.CourseCtrl.mediaRecorder.Close();
                viewModel.CourseCtrl.mediaRecorder = null;
            }
            string coursePath = viewModel.CurrCourse.CoursePath + viewModel.CurrCourse.Name;
            CopyOldCourseResources(coursePath);
            viewModel.save();            

            StartRecord.IsEnabled = false;
            PauseRecord.IsEnabled = false;
            EndRecord.IsEnabled = false;           
            viewModel.CourseCtrl.courseMenuTool.OpenNewCourseBtn.IsEnabled = true;
        }

        private void PauseRecord_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CourseCtrl.pauseRecourd();            
        }

        /// <summary>
        /// 拷贝旧课的资源到新课的目录下，用于回放是使用
        /// </summary>
        private void CopyOldCourseResources(string NewCoursePath)
        {
            foreach (var item in viewModel.CourseCtrl.courseMenuTool.allCoursePath)
            {
                try
                {
                    Public.HelperMethods.CopyFolder(item + "\\Resource\\ppt", NewCoursePath + "\\Resource\\ppt");
                }
                catch (Exception)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "拷贝play课程文件时发生错误！");
                    message.ShowDialog();
                    Public.Log.WriterLog("拷贝play课程文件时发生错误");
                }

                try
                {
                    Public.HelperMethods.CopyFolder(item + "\\Resource\\picture", NewCoursePath + "\\Resource\\picture");
                }
                catch (Exception)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "拷贝play课程文件时发生错误！");
                    message.ShowDialog();
                    Public.Log.WriterLog("拷贝play课程文件时发生错误");
                }
            }

        }

        private void LocalPlay_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = ".course;|*course";

            if (ofd.ShowDialog() == true)
            {
                string courseName = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                string currCoursePath = System.IO.Path.GetDirectoryName(ofd.FileName);
                string httpServerRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\AppConfig\\Courses\\" + courseName;

                Public.HelperMethods.CopyFolder(currCoursePath, httpServerRoot, true);

                int port = UserAppConfigStatic.Port;
                if (!Public.HelperMethods.CheckProcess("node_shanyun"))
                {
                    port = CheckProtIsUse(port);
                    string strCmdText = " /C  " + @"http-server -p " + port + " " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IMOOC\AppConfig -c-1";
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
                    p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                    p.StartInfo.Arguments = strCmdText;
                    p.Start();//启动程序

                    //System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                    UserAppConfigStatic.Port = port;
                }

                Public.Log.WriterLog("启动node_shanyun.exe,端口为： " + port.ToString());
                string link = "http://localhost:" + port.ToString() + "/Courses/" + courseName + "/index.html";
                System.Diagnostics.Process.Start(link);
                RQCode rqCode = new RQCode(courseName, link);
                rqCode.ShowDialog();
            }
        }

        private int CheckProtIsUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    port++;
                    CheckProtIsUse(port);
                }
            }
            return port;
        }


    }
}
