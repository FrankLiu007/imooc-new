using System.Windows.Controls;
using System.Windows.Input;

namespace IMOOC.EduCourses.PPTModule
{
    /// <summary>
    /// Interaction logic for PPTBrowseView.xaml
    /// </summary>
    public partial class PPTBrowseView : UserControl
    {
        public PPTBrowseView()
        {
            InitializeComponent();
        }
        
        private void SlideListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = MouseWheelEvent;
            eventArg.Source = sender;
            (sender as ListBox).RaiseEvent(eventArg);
        }

        private void stack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            contextPopup.IsOpen = true;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            contextPopup.IsOpen = false;
        }
        

    }
}
