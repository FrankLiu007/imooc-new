using System;
using System.Net;
using System.Windows;

namespace IMOOCServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SocketServer server;
        private int teacherNum;
        private int studentNum;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 50000);
            server = new SocketServer(100, 10);
            server.Init();
            server.Start(iep);
            server.TeacherNumChanged += (s, a) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    teacherNum += a.TeacherNumChanged;
                    teacherNumTb.Text = teacherNum.ToString();
                }));
            };
            server.StudentNumChanged += (s, a) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    studentNum += a.StudentNumChanged;
                    studentNumTb.Text = studentNum.ToString();
                }));
            };
        }
    }
}

