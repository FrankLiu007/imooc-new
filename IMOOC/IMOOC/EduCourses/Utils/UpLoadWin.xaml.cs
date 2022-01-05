using System;
using System.Collections.Generic;
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
using Public;
using System.Threading.Tasks;

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for UpLoadWin.xaml
    /// </summary>
    public partial class UpLoadWin : Window
    {
        public UpLoadWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyHttpWebRequest.IsOnline==false)
            {
                return;
            }

            Task task1 = new Task(() =>
            {
                MyHttpWebRequest.GetCourses(new Dictionary<string, string>()
                {
                    {"school_id", MyHttpWebRequest.CurrUser.SchoolName.Id.ToString()},
                });
            });
            Task task2 = new Task(() =>
            {
                for (int i = 0; i < MyHttpWebRequest.WebCourses.Count; i++)
                {
                    MyHttpWebRequest.GetChapter(new Dictionary<string, string>()
                    {
                         {"course_id",MyHttpWebRequest.WebCourses[i].course.Id.ToString() },
                         { "school_id",MyHttpWebRequest.CurrUser.SchoolName.Id.ToString() }
                    }, MyHttpWebRequest.WebCourses[i].course.Id.ToString());
                }

            });
            task1.Start();
            task1.Wait();
            task2.Start();
            task2.ContinueWith((t) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                   
                }));
            });
            //MyHttpWebRequest.GetChapter();




        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void SelectCourseBrower_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void UpLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChapterLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public class CoursetInfo
    {
        public CoursetInfo()
        {
            CourseName = "NULL";
            ChapterName = "未选择";
            FileSize = "NULL";
            State = "NULL";
        }
        public string CourseName { get; set; }
        public string ChapterName { get; set; }
        public string FileSize { get; set; }
        public string State { get; set; }
    }
}
