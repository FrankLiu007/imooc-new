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
    /// Interaction logic for MessageWin.xaml
    /// </summary>
    public partial class MessageWin : Window
    {
        public bool IsYes { get; set; }
        public bool IsCancel { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentText">要显示的内容</param>
        public MessageWin(MessageWinType type,string contentText)
        {
            InitializeComponent();
            IsYes = false;
            IsCancel = false;
            SetMessageWin(type, contentText);
        }

        private void SetMessageWin(MessageWinType type, string content)
        {
            if (type == MessageWinType.Prompt)
            {
                PromptWin.Visibility = Visibility.Visible;
                PromptLab.Content = "提示";
                ContentTextLab.Content = content;
                OKBtn.Visibility = Visibility.Visible;
                OKBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c87ce"));
                YesOrNo.Visibility = Visibility.Hidden;
                ContentText.Visibility = Visibility.Hidden;
                SignPicLab.Visibility = Visibility.Hidden;
                YesNoCancel.Visibility = Visibility.Hidden;
            }
            else if (type == MessageWinType.Warning)
            {
                PromptWin.Visibility = Visibility.Hidden;
                OKBtn.Visibility = Visibility.Hidden;
                YesOrNo.Visibility = Visibility.Visible;
                ContentTextLab.Visibility = Visibility.Hidden;
                YesNoCancel.Visibility = Visibility.Hidden;
                ContentText.Visibility = Visibility.Visible;
                ContentText.Content = content;
                YesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff971e"));
                PromptLab.Content = "警告";
                SignPicLab.Content = FindResource("warning");
            }
            else if (type == MessageWinType.Error)
            {
                PromptWin.Visibility = Visibility.Hidden;
                OKBtn.Visibility = Visibility.Visible;
                ContentTextLab.Visibility = Visibility.Hidden;
                YesNoCancel.Visibility = Visibility.Hidden;
                ContentText.Visibility = Visibility.Visible;
                ContentText.Content = content;
                OKBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f04134"));
                SignPicLab.Content = FindResource("error");
                PromptLab.Content = "错误";
                YesOrNo.Visibility = Visibility.Hidden;
            }
            else if (type==MessageWinType.YesNoCancel)
            {
                PromptWin.Visibility = Visibility.Hidden;
                OKBtn.Visibility = Visibility.Hidden;
                YesOrNo.Visibility = Visibility.Hidden;
                ContentTextLab.Visibility = Visibility.Hidden;
                YesNoCancel.Visibility = Visibility.Visible;
                ContentText.Visibility = Visibility.Visible;
                ContentText.Content = content;
                YesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff971e"));
                PromptLab.Content = "警告";
                SignPicLab.Content = FindResource("warning");
            }
           
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void YesBtn_Click(object sender, RoutedEventArgs e)
        {
            IsYes = true;
            this.Close();
        }

        private void NoBtn_Click(object sender, RoutedEventArgs e)
        {
            IsYes = false;
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            IsYes = true;
            this.Close();
        }

        private void UnSave_Click(object sender, RoutedEventArgs e)
        {
            IsYes = false;
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            IsYes = false;
            IsCancel = true;
            this.Close();
        }
    }

    public enum MessageWinType
    {
        //Ok = 1,
        //YesOrNo = 2

        Prompt = 1,
        Warning = 2,
        Error = 3,
        YesNoCancel=4
    }
}