using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for WinSaveFilesOptions.xaml
    /// </summary>
    public partial class WinSaveFilesOptions : Window
    {
        private bool isOpenNewCourse;
        public bool IsOpenNewCourse
        {
           get{ return isOpenNewCourse;}
        }

        private string courseName;

        public string CourseName
        {
            get { return courseName; }            
        }

        private string coursePath;

        public string CoursePath
        {
            get { return coursePath; }
        }

        List<string> AllCourseName;

        public WinSaveFilesOptions(string NewCoursePath, string NewCourseName, List<string> allCourseName)
        {
            InitializeComponent();
            AllCourseName = allCourseName;
            isOpenNewCourse = false;            
            courseName = NewCourseName;
            coursePath = NewCoursePath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SavePathTextBox.Text = coursePath;
            filenameTB.Text = courseName;
        }

        private void SaveBrower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SavePathTextBox.Text = folderBrowserDialog1.SelectedPath;  //获取用户选中路径
            }
        }

        private void StartNewCourse_Click(object sender, RoutedEventArgs e)
        {
            coursePath = SavePathTextBox.Text+"\\";
            courseName = filenameTB.Text;
            if (Directory.Exists(coursePath+courseName)||!InspectCourseName(courseName))
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "已存在同名课程，请填写新的课程名");
                message.ShowDialog();
                Public.Log.WriterLog(coursePath  + courseName + "   已存在同名课程，请填写新的课程名！");

                return;
            }
            isOpenNewCourse = true;
            Close();
        }
 
        public bool InspectCourseName(string courseName)
        {
            if (AllCourseName.Contains(courseName))
            {
                return false;
            }
            return true;
        }

        

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //private void OpenPPTBrower_Click(object sender, RoutedEventArgs e)
        //{
        //    Names.Clear();
        //    var ofd = new OpenFileDialog();
        //    ofd.Multiselect = true;
        //    ofd.Filter = ".ppt;.pptx;.pps;.ppsx|*.ppt;*.pptx;*.pps";
        //    ofd.Multiselect = true;

        //    if (ofd.ShowDialog() == true)
        //    {
        //        var sb = new StringBuilder();
        //        foreach (var item in ofd.SafeFileNames)
        //        {
        //            sb.Append(item);
        //            sb.Append(" ");
        //        }
        //        ReadyPPTPathTBox.Text = sb.ToString();
        //        foreach (var item in ofd.FileNames)
        //        {
        //            Names.Add(item);
        //        }
        //    }
        //}

        //private void ConvertPPT_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Names == null|| ReadyPPTPathTBox.Text=="")
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        (sender as Button).IsEnabled = false;
        //        (sender as Button).Content = "进行中";
        //        var win = new ProgressWindow(Names,this);
        //        win.Show();
        //    }
            
        //}   

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            isOpenNewCourse = false;
            Close();           
        }      
    }
}
