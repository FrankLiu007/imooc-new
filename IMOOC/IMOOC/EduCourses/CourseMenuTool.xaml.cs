using IMOOC.EduCourses.PPTModule;
using IMOOC.EduCourses.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace IMOOC.EduCourses
{
    /// <summary>
    /// Interaction logic for CourseMenuTool.xaml
    /// </summary>
    public partial class CourseMenuTool : UserControl
    {
        /// <summary>
        /// 所有添加进来过得课程名，用于结束录制时复制资源
        /// </summary>
        public List<string> allCoursePath;
        public bool isOpenNewCourse;
        public CoursesViewModel viewMode;
        public CourseMenuTool()
        {
            InitializeComponent();
            //isCurrcourse = false;
            isOpenNewCourse = false;
            allCoursePath = new List<string>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewMode = (DataContext as CoursesViewModel);
        }

        private void OpenOldCourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = ".course;|*course";

                if (ofd.ShowDialog() == true)
                {
                    try
                    {
                        viewMode.CourseCtrl.StartOldCourse(Path.GetFileNameWithoutExtension(ofd.FileName), ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageWin message = new MessageWin(MessageWinType.Error, "打开旧课时发生错误!");
                        message.ShowDialog();
                        Public.Log.WriterLog(ex.Message + "打开旧课时发生错误!", viewMode.CurrCourse.Name, viewMode.CurrCourse.CoursePath);
                        return;
                    }

                    allCoursePath.Add(Path.GetDirectoryName(ofd.FileName));
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "打开旧课时发生错误!");
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "打开旧课时发生错误!", viewMode.CurrCourse.Name, viewMode.CurrCourse.CoursePath);
            }
        }        

        private void AllCourseCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllCourseCombox.SelectedIndex==-1)
            {
                return;
            }
            (DataContext as CoursesViewModel).CourseCtrl.indicater.updataIndicater(new Thickness(0, 0, 0, 0), 0, 0);

        }

        private void OpenNewCourse_Click(object sender, RoutedEventArgs e)
        {
            viewMode.CourseCtrl.createWinSaveFilesOptions();            
        }

        private void CloseCourse_Click(object sender, RoutedEventArgs e)
        {
            if (AllCourseCombox.Items.Count == 0)
            {
                return;
            }
            else if (AllCourseCombox.Items.Count == 1)
            {
                MessageWin message = new MessageWin(MessageWinType.Warning, "当前已为最后一门课程，是否退出程序？");
                message.ShowDialog();
                Public.Log.WriterLog("当前已为最后一门课程，是否退出程序？", viewMode.CurrCourse.Name, viewMode.CurrCourse.CoursePath);
                if (message.IsYes == true)
                {
                    viewMode.CourseCtrl.mainWnd.exitIMOOC();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (viewMode.CurrCourse.IsNew == false)
                {
                    viewMode.AllCourse.RemoveAt(AllCourseCombox.SelectedIndex);
                    AllCourseCombox.SelectedIndex = 0;
                }
                else
                {
                    if (viewMode.CurrCourse.Name == "未命名")
                    {
                        MessageWin message = new MessageWin(MessageWinType.Warning, "当前课程未保存，是否保存当前课程？");
                        message.ShowDialog();
                        Public.Log.WriterLog("当前课程未保存，是否保存当前课程？", viewMode.CurrCourse.Name, viewMode.CurrCourse.CoursePath);
                        if (message.IsYes == true)
                        {
                            ReName reName = new ReName(viewMode.CurrCourse);
                            reName.Activate();
                            reName.Top = 80;
                            reName.Left = (this.ActualWidth - 400) / 2;
                            reName.ShowDialog();
                            if (reName.isSave)
                            {
                                (AllCourseCombox.SelectedItem as ComboBoxItem).Content = viewMode.CurrCourse.Name;
                            }
                            else if (reName.isCancel)
                            {
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    viewMode.AllCourse.RemoveAt(AllCourseCombox.SelectedIndex);
                    AllCourseCombox.SelectedIndex = 0;
                }

            }
        }

        private void SaveCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewMode.CurrCourse.IsModify == false || viewMode.IsRecord == true)
            {
                PromptLable.Content = "已保存";
                promptTimer();
                return;
            }

            if (viewMode.CurrCourse.Name == "未命名")
            {
                ReName reName = new ReName(viewMode.CurrCourse);
                reName.Activate();
                reName.Top = 80;
                reName.Left = (this.ActualWidth - 400) / 2;
                reName.ShowDialog();
                if (reName.isSave)
                {
                    for (int i = 0; i < AllCourseCombox.Items.Count; i++)
                    {
                        if ((AllCourseCombox.Items[i] as FastSwitchListItem).Name.ToString() == "未命名")
                        {
                            //待修复，此处AllCourseCombox显示更新不及时，应尝试其他的更新方案
                            (AllCourseCombox.Items[i] as FastSwitchListItem).Name = viewMode.CurrCourse.Name;
                            AllCourseCombox.SelectedIndex = i;
                        }
                    }
                }
                else if (reName.isCancel)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
            viewMode.saveCourse();
            PromptLable.Content = "课程" + viewMode.CurrCourse.Name + "保存完毕！";
            promptTimer();
        }


        private void promptTimer()
        {
            PromptLable.Visibility = Visibility.Visible;
            Timer timer = new Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            (sender as Timer).Stop();
            Dispatcher.Invoke(new Action(() =>
            {
                PromptLable.Visibility = Visibility.Hidden;
            }));

        }

    }
}
