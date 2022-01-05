using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SaveOldCourseWin.xaml
    /// </summary>
    public partial class SaveOldCourseWin : Window
    {
        public bool IsSave { get; set; }
        public bool IsCancel { get; set; }

        private ObservableCollection<CourseNameListItem> courseNameList;

        public ObservableCollection<CourseNameListItem> CourseNameList
        {
            get { return courseNameList; }
            set { courseNameList = value; }
        }
        public SaveOldCourseWin(ObservableCollection<CourseNameListItem> courseNameList)
        {
            InitializeComponent();
            this.courseNameList = courseNameList;
            IsSave = false;
            IsCancel = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            courseNameListBox.ItemsSource = courseNameList;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            for (int i = 0; i < CourseNameList.Count; i++)
            {
                if (CourseNameList[i].Name==button.ToolTip.ToString())
                {
                    CourseNameList.RemoveAt(i);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            IsSave = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = true;
            Close();
        }
    }

    public class CourseNameListItem
    {
        public string Name { get; set; }
    }
}
