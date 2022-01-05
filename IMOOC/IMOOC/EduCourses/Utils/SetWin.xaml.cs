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

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for SetWin.xaml
    /// </summary>
    public partial class SetWin : Window
    {
        public SetWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PenMode.IsChecked = UserAppConfigStatic.PenMode;
            BoardMode.IsChecked = UserAppConfigStatic.BoardMode;
            backGroundLabel.Background = greenBoard.Background;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void DebugInfo_Click(object sender, RoutedEventArgs e)
        {
            if (DebugInfo.IsChecked == true)
            {
                Public.Log.isDebugInfoOn = true;
            }
            else
            {
                Public.Log.isDebugInfoOn = false;
            }
        }

        private void InitWin()
        {
            //太过冗余，再次添加新的设置选项是应该考虑更加简洁的设置方式
            if (UserAppConfigStatic.IsDebugInfoOn == true)
            {
                DebugInfo.IsChecked = true;
                Public.Log.isDebugInfoOn = true;
            }
            else
            {
                DebugInfo.IsChecked = true;
                Public.Log.isDebugInfoOn = false;
            }

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PrintSetGrid.Visibility == Visibility.Collapsed)
            {
                PrintSetGrid.Visibility = Visibility.Visible;
                PrintArrow.Content = FindResource("X3");
            }
            else
            {
                PrintSetGrid.Visibility = Visibility.Collapsed;
                PrintArrow.Content = FindResource("X2");
            }
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (ModeSetGrid.Visibility == Visibility.Collapsed)
            {
                ModeSetGrid.Visibility = Visibility.Visible;
                Mode.Content = FindResource("X3");
            }
            else
            {
                ModeSetGrid.Visibility = Visibility.Collapsed;
                Mode.Content = FindResource("X2");
            }
        }

        private void Grid_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            if (background.Content == FindResource("X3"))
            {
                background.Content = FindResource("X2");
            }
            else
            {
                background.Content = FindResource("X3");
            }

            title.Content = "背景设置";
            PrintSp.Visibility = Visibility.Hidden;
            ModeSp.Visibility = Visibility.Hidden;
            SetBackgroundWp.Visibility = Visibility.Visible;
            backGroundLabel.Visibility = Visibility.Visible;


        }

        private void PrintLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            title.Content = "输出设置";
            PrintSp.Visibility = Visibility.Visible;
            ModeSp.Visibility = Visibility.Hidden;
            SetBackgroundWp.Visibility = Visibility.Hidden;
            backGroundLabel.Visibility = Visibility.Hidden;
        }

        private void ModeLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            title.Content = "模式设置";
            PrintSp.Visibility = Visibility.Hidden;
            ModeSp.Visibility = Visibility.Visible;
            SetBackgroundWp.Visibility = Visibility.Hidden;
            backGroundLabel.Visibility = Visibility.Hidden;
        }

        private void BoardMode_Click(object sender, RoutedEventArgs e)
        {
            UserAppConfigStatic.BoardMode = BoardMode.IsChecked ?? true;
            UserAppConfigStatic.PenMode = !BoardMode.IsChecked ?? false;
        }

        private void PenMode_Click(object sender, RoutedEventArgs e)
        {
            UserAppConfigStatic.PenMode = PenMode.IsChecked ?? false;
            UserAppConfigStatic.BoardMode = !PenMode.IsChecked ?? true;
        }

        private void SetBackgroundSp_MouseDown(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType().ToString() == "System.Windows.Controls.Border")
            {
                backGroundLabel.Background = (e.OriginalSource as Border).Background;
            }

        }



        //private void InitAllLabel()
        //{
        //    SetLabelBackground(label_1);
        //    SetLabelBackground(label_2);
        //}

        //private void SetLabelBackground(Label label)
        //{
        //    label.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("##f8f8f8"));
        //    label.Foreground = Brushes.Black;
        //}
    }
}
