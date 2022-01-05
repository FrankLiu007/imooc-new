using Public;
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
    /// Interaction logic for ReName.xaml
    /// </summary>
    public partial class ReName : Window
    {
        /// <summary>
        /// 判断是否重命名并保存
        /// </summary>
        public bool isSave { get; set; }
        public bool isCancel { get; set; } 
        private Course CurrCourse;
        //private ReNameWinType _type;
        public ReName(Course currCourse)
        {
            InitializeComponent();
            isSave = false;
            isCancel = false;
            CurrCourse = currCourse;
            UnnamedSavePathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\" + MyHttpWebRequest.CurrUser.UserName + "\\";
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            isCancel = true;   
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(UnnamedSavePathTextBox.Text.ToString() + UnnamedfilenameTBox.Text.ToString()))
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "已存在同名课程，请填写新的课程名");
                message.ShowDialog();
                return;
            }            
            try
            {
                Public.HelperMethods.CopyFolder(UnnamedSavePathTextBox.Text.ToString() + "未命名", UnnamedSavePathTextBox.Text.ToString() + UnnamedfilenameTBox.Text.ToString());
                Public.Log.WriterLog("拷贝重命名时未命名课程文件  " + "源文件 = " + UnnamedSavePathTextBox.Text.ToString() + "未命名" + ";目标文件 = " + UnnamedSavePathTextBox.Text.ToString() + UnnamedfilenameTBox.Text.ToString());
            }
            catch (InvalidOperationException ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "课程重命名时发生错误");
                message.ShowDialog();
                MessageBox.Show("课程重命名时发生错误\n"+ex);
            }

            isSave = true;
            CurrCourse.IsNew = true;
            CurrCourse.CoursePath= UnnamedSavePathTextBox.Text.ToString();
            CurrCourse.Name= UnnamedfilenameTBox.Text.ToString();
            CourseControl.CourseSavePath = UnnamedSavePathTextBox.Text.ToString()+ UnnamedfilenameTBox.Text.ToString() + "\\";
            Close();
        }

        private void UnSave_Click(object sender, RoutedEventArgs e)
        {
            isSave = false;            
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void UnnamedBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UnnamedSavePathTextBox.Text = folderBrowserDialog1.SelectedPath;  //获取用户选中路径
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            UnnamedfilenameTBox.Text = "";
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (_type==ReNameWinType.CourseNameAndPath)
        //    {
        //        SaveGrid.Visibility = Visibility.Visible;
        //        ReNameGrid.Visibility = Visibility.Hidden;
        //    }
        //    else if (_type==ReNameWinType.WarringPrompt)
        //    {
        //        SaveGrid.Visibility = Visibility.Hidden;
        //        ReNameGrid.Visibility=Visibility.Visible;
        //    }
        //}
    }

    public enum ReNameWinType
    {
        CourseNameAndPath=1,
        WarringPrompt=2
    }
}
