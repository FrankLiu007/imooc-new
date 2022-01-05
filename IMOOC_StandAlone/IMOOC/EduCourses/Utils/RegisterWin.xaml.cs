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
    /// Interaction logic for RegisterWin.xaml
    /// </summary>
    public partial class RegisterWin : Window
    {
        public RegisterWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            machineNumberTextBox.Text= Public.Register.GetAllID();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            string serialNumber = serialNumberTextBox.Text;
            if (serialNumber.Length!=44)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "注册码长度错误");
                message.ShowDialog();
                Public.Log.WriterLog("注册码长度错误" + serialNumber);
                return;
            }

            if (Public.Register.RegisterIMOOC(serialNumber))
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "注册成功");
                message.ShowDialog();
                Public.Log.WriterLog("注册成功"+ serialNumber);
                Close();
            }
            else
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "注册失败");
                message.ShowDialog();
                Public.Log.WriterLog("注册失败" + serialNumber);
                return;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }
    }
}
