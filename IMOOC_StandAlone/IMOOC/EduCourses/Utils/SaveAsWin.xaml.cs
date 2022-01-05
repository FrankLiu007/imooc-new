using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for SaveAsWin.xaml
    /// </summary>
    public partial class SaveAsWin : Window
    {
        /// <summary>
        /// 是否另存为
        /// </summary>
        public bool isSaveAs { get; set; }

        /// <summary>
        /// 保存的课程文件名
        /// </summary>
        public string SaveCourseName { get; set; }

        /// <summary>
        /// 保存的文件夹路径，以"\\"结尾。不包含课程文件部分。
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseName"></param>
        /// <param name="courseSavePath"> 路径以"\\"结尾</param>
        public SaveAsWin(string courseName="",string courseSavePath="")
        {
            InitializeComponent();
            isSaveAs = false;
            saveAsCourseNameTBox.Text = courseName;
            saveAsPathTextBox.Text = courseSavePath;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            saveAsCourseNameTBox.Text = "";
        }

        private void UnnamedBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveAsPathTextBox.Text = folderBrowserDialog1.SelectedPath;  //获取用户选中路径
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {        
            SaveCourseName = saveAsCourseNameTBox.Text;
            SavePath = saveAsPathTextBox.Text;
            if (!CheckPathAndName())
            {
                return;
            }

            isSaveAs = true;
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            isSaveAs = false;
            SaveCourseName = saveAsCourseNameTBox.Text;
            SavePath = saveAsPathTextBox.Text;

            Close();
        }

        private bool CheckPathAndName()
        {
            string path = SavePath + SaveCourseName;
            if (SaveCourseName=="未命名")
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "请输入课程名");
                message.ShowDialog();
                return false;
            }
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(SavePath);
                    Public.Log.FileOrFolder(Public.LogType.CreatFolder, SavePath);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "创建另存课程文件夹时发生错误！");
                    message.ShowDialog();
                    Public.Log.WriterLog("创建另存课程文件夹时发生错误,    " + ex.Message + path);
                    return false;
                }
            }
            else
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "此课程文件夹已存在！");
                message.ShowDialog();
                Public.Log.WriterLog("此课程文件夹已存在,    " + path);
                return false;
            }
        }



    }
}
