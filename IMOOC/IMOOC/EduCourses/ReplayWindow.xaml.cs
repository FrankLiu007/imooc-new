using System.Windows;
using System.Windows.Data;

namespace IMOOC.EduCourses
{
    /// <summary>
    /// Interaction logic for ReplayWindow.xaml
    /// </summary>
    public partial class ReplayWindow : Window
    {


        public double TargetPoint
        {
            get { return (double)GetValue(TargetPointProperty); }
            set { SetValue(TargetPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetPointProperty =
            DependencyProperty.Register("TargetPoint", typeof(double), typeof(ReplayWindow), new PropertyMetadata(0.0));


        public ReplayWindow(ReplayViewModel viewModel, string mediaSource)
        {
            DataContext = viewModel;
            InitializeComponent();
            progressBar.Maximum = viewModel.TotalTime;
            BindingOperations.SetBinding(this, TargetPointProperty, new Binding("TargetPoint") { Source = DataContext, Mode = BindingMode.TwoWay });
            mediaPlayer.Source = new System.Uri(mediaSource);
        }

        private void progressBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var p = e.GetPosition(progressBar);
            TargetPoint = p.X * progressBar.Maximum / progressBar.ActualWidth;
            rateTb.Text = int.MaxValue.ToString();
            shiftBtn.IsChecked = true;
        }

        private void innnBtn_click(object sender, RoutedEventArgs e)
        {
            shiftBtn.IsChecked = true;
            rateTb.Text = "4";
        }

        private void Player_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }
    }
}
