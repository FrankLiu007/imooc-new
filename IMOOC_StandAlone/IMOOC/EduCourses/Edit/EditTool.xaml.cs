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
using System.Windows.Navigation;

namespace IMOOC.EduCourses.Edit
{
    /// <summary>
    /// Interaction logic for EditTool.xaml
    /// </summary>
    public partial class EditTool : UserControl
    {
        public EditTool()
        {
            InitializeComponent();
        }

        private void ButtonCutSelection_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ButtonCutSelection();
        }

        private void ButtonCopySelection_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ButtonCopySelection();
        }

        private void ButtonPasteSelection_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ButtonPasteSelection();
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.Redo();
        }

        private void ButtonSelectArea_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }
    }
}
