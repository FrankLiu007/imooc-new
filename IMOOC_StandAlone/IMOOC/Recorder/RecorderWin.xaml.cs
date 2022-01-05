using IMOOC.EduCourses;
using IMOOC.EduCourses.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace IMOOC.Recorder
{
    /// <summary>
    /// Interaction logic for RecorderWin.xaml
    /// </summary>
    public partial class RecorderWin : Window
    {
        public Video vid;
        private DeviceInfo[] m_videoDeviceList;
        private DeviceInfo[] m_audioDeviceList;        
        int sound;
        public int camera;
        private bool isCameraChanged;        
        private string OutputFile;
        private string MediaName;
        private EduCourses.Course CurrCourse;

        private MediaRecorder m_recorder;
        public MediaRecorder Recorder
        {
            get { return m_recorder;}
        }

        private bool isStart;
        public bool IsStart
        {
            get { return isStart;}
        }

        private bool isCloseAllOldCourse;
        public bool IsCloseAllOldCourse
        {
            get { return isCloseAllOldCourse; }
        }

        private bool isClearBanshu;
        public bool IsClearBanshu
        {
            get { return isClearBanshu; }
        }

        public RecorderWin(MediaRecorder mediaRecorder,EduCourses.Course currCourse)
        {
            InitializeComponent();
            //vid = new Video();
            isCameraChanged = false;
            isStart = false;
            isClearBanshu = false;
            isCloseAllOldCourse = false;
            m_recorder = mediaRecorder;
            //OutputFile = outputFile;
            //MediaName = currCourse.Name;

            m_videoDeviceList = MediaRecorder.GetCameraDeviceList();
            m_audioDeviceList = MediaRecorder.GetSoundDeviceList();
            InitControl();

            SavePathTextBox.Text= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\善而教";
            CurrCourse = currCourse;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrCourse.Name!="未命名")
            {
                CourseNameTextBox.Text = CurrCourse.Name;
                CourseNameTextBox.IsReadOnly = true;
                SavePathTextBox.Text = CurrCourse.CoursePath;
                SavePathTextBox.IsReadOnly = true;
                BrowseBtn.IsEnabled = false;
            }
            
        }

        private void InitControl()
        {
            comboBoxChannels.Items.Add("1");
            comboBoxChannels.Items.Add("2");
            comboBoxChannels.SelectedIndex = 1;

            comboBoxSampleRate.Items.Add("44100");
            comboBoxSampleRate.Items.Add("16000");
            comboBoxSampleRate.Items.Add("8000");
            comboBoxSampleRate.SelectedIndex = 2;

            comboBoxResolution.Items.Add("720*576");
            comboBoxResolution.Items.Add("640*480");
            comboBoxResolution.Items.Add("352*288");
            comboBoxResolution.Items.Add("320*240");
            comboBoxResolution.SelectedIndex = 1;

            comboBoxFrameRate.Items.Add(30);
            comboBoxFrameRate.Items.Add(25);
            comboBoxFrameRate.Items.Add(20);
            comboBoxFrameRate.Items.Add(15);
            comboBoxFrameRate.Items.Add(10);
            comboBoxFrameRate.SelectedIndex = 0;

            foreach (DeviceInfo dev in m_videoDeviceList)
            {
                comboBoxCamera.Items.Add(dev.name);
            }
            if (m_videoDeviceList.Length > 0)
            {
                comboBoxCamera.SelectedIndex = 0;
            }

            foreach (DeviceInfo dev in m_audioDeviceList)
            {
                comboBoxSound.Items.Add(dev.name);
            }
            if (m_audioDeviceList.Length > 0)
            {
                comboBoxSound.SelectedIndex = 0;
            }
        }

        //private void Init()
        //{
        //    MediaRecorder.Init();
        //    System.Console.WriteLine(MediaRecorder.GetVersion());
        //    MediaRecorder.StartMediaServer(MediaRecorder.RTSP_PORT);
        //}

        //private void Uninit()
        //{
        //    CloseRecorder();
        //    MediaRecorder.StopMediaServer();
        //    MediaRecorder.Quit();        
        //    Console.WriteLine("quit");
        //}

        private void CloseRecorder()
        {
            if (m_recorder != null)
            {
                m_recorder.Close();
                m_recorder = null;
            }
        }

        //public void CameraPreview()
        //{
        //    CloseRecorder();
        //    camera = getCamera();
        //}

        public void InitStart()
        {
            CloseRecorder();
            camera = getCamera();
            sound = getSound();
           
            m_recorder = MediaRecorder.Open(camera, sound);

            Size resolution = getResolution();
            m_recorder.SetResolution((int)resolution.Width, (int)resolution.Height);
            int fps = getFrameRate();
            m_recorder.SetFrameRate(fps);

            int channels = getChannels();
            int sampleRate = getSampleRate();
            m_recorder.SetAudioProfile(channels, sampleRate, 16);

            m_recorder.StartPublish(MediaName);
            m_recorder.Start();

            if (CheckBoxVideo.IsChecked == true)
            {
                CameraDiff();
            }
        }

        public void CameraDiff()
        {
            if (vid!=null)
            {
                vid.Close();
            }
            vid = new Video();        

            m_recorder.SetVideoWindow((int)vid.Handle);
            m_recorder.ShowVideoWindow(1);
            vid.Activate();
            Point p1 = VideoWnd.PointToScreen(new Point(0, 0));
            System.Drawing.Point dPoint = new System.Drawing.Point();
            dPoint.X = (int)p1.X;
            dPoint.Y = (int)p1.Y;
            vid.Location = dPoint;

            PresentationSource source = PresentationSource.FromVisual(this);
            double dpiX, dpiY;
            dpiX = source.CompositionTarget.TransformToDevice.M11;
            dpiY = source.CompositionTarget.TransformToDevice.M22;
            vid.Width = (int)(VideoWnd.ActualWidth * dpiX);
            vid.Height = (int)(VideoWnd.ActualHeight * dpiY);

            vid.Show();
            vid.TopMost = true;
        }

        public void Start()
        {
            CloseRecorder();
            if (CheckBoxVideo.IsChecked == true)
            {
                camera = getCamera(); 
            }
            else
            {
                if (vid!=null)
                {
                    vid.Close();
                }
                
                camera = -1;
            }
            sound = getSound();

            m_recorder = MediaRecorder.Open(camera, sound);

            if (m_recorder == null)
            {
                MessageBoxResult re = System.Windows.MessageBox.Show("录像机未工作 \n点击确定,只录制笔画，点击取消退出录制", "注意！",
                       MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                if (re == MessageBoxResult.OK)
                {
                    Stop();
                    this.Close();                    
                    return;
                }
                else
                {
                    Stop();
                    this.Close();
                    isStart = false;
                    return;
                }
            }
            else
            {
                Size resolution = getResolution();
                m_recorder.SetResolution((int)resolution.Width, (int)resolution.Height);
                int fps = getFrameRate();
                m_recorder.SetFrameRate(fps);

                int channels = getChannels();
                int sampleRate = getSampleRate();
                m_recorder.SetAudioProfile(channels, sampleRate, 16);

                if (CheckBoxVideo.IsChecked == true)
                {
                    m_recorder.SetOutputFile(OutputFile+".mp4");
                }
                else
                {
                    m_recorder.SetOutputFile(OutputFile + ".mp3");
                }
               
                m_recorder.StartPublish(MediaName);
                m_recorder.Start();
                if (vid!=null)
                {
                    vid.Close();
                }
                isStart = true;              
            }

        }

        public void UpdataLocation(System.Drawing.Point Location)
        {
            //vid.Location = Location;
        }

        public void Stop()
        {
            CloseRecorder();
            if (vid!=null)
            {
                vid.Close();
            }           
        }


        private int getCamera()
        {
            return comboBoxCamera.SelectedIndex;
        }

        private int getSound()
        {
            return comboBoxSound.SelectedIndex;
        }

        private Size getResolution()
        {
            string[] strs = comboBoxResolution.Text.Split('*');
            int width = Int32.Parse(strs[0]);
            int height = Int32.Parse(strs[1]);
            return new Size(width, height);
        }

        private int getChannels()
        {
            return Int32.Parse(comboBoxChannels.Text);
        }

        private int getSampleRate()
        {
            return Int32.Parse(comboBoxSampleRate.Text);
        }

        private int getFrameRate()
        {
            return Int32.Parse(comboBoxFrameRate.Text);
        }


        private void DemoRecorder_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Uninit();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (ReName())
            {
                Start();
                this.Close();
            }                     
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {            
            Stop();
            isStart = false;
            this.Close();
        }

        private void comboBoxCamera_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (m_recorder==null)
            {
                return;
            }
            if (isCameraChanged==true)
            {
                InitStart();
            }
            isCameraChanged = true;
            
        }

        private void CheckBoxVideo_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxVideo.IsChecked == true)
            {
                isCameraChanged = true;
                InitStart();
                VideoSettingsGrid.IsEnabled = true;
            }
            else
            {
                isCameraChanged = false;
                vid.Close();
                CloseRecorder();
                VideoSettingsGrid.IsEnabled = false;
            }
        }

        private void SaveBrower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SavePathTextBox.Text = folderBrowserDialog1.SelectedPath;  //获取用户选中路径
            }
        }

        private bool ReName()
        {
            if (Directory.Exists(SavePathTextBox.Text.ToString()+"\\" + CourseNameTextBox.Text.ToString()) && CurrCourse.Name == "未命名")
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "    已存在同名课程，请填写新的课程名");
                message.ShowDialog();
                Public.Log.WriterLog(SavePathTextBox.Text.ToString() + "\\" + CourseNameTextBox.Text.ToString() + "已存在同名课程，请填写新的课程名！");
                return false;
            }
            try
            {
                if (CurrCourse.Name=="未命名")
                {
                    //这里会出现有问题，待修复---------
                    string sourceFolder = SavePathTextBox.Text.ToString() + "\\" + "未命名";
                    string targetFolder = SavePathTextBox.Text.ToString() + "\\" + CourseNameTextBox.Text.ToString();
                    Public.HelperMethods.CopyFolder(sourceFolder, targetFolder);
                    //-----------------------       
                }

            }
            catch (InvalidOperationException ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "课程重命名时发生错误!" );
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "课程重命名时发生错误！");
                return false;
            }            

            CurrCourse.CoursePath = SavePathTextBox.Text.ToString() + "\\";
            CurrCourse.Name = CourseNameTextBox.Text.ToString();
            CourseControl.CourseSavePath = SavePathTextBox.Text.ToString() + "\\" + CourseNameTextBox.Text.ToString() + "\\";

            MediaName = CurrCourse.Name;
            OutputFile = CourseControl.CourseSavePath + "play\\" + MediaName;
            string play = CourseControl.CourseSavePath + "play\\";
            if (!Directory.Exists(play))
            {
                Directory.CreateDirectory(play);
                Public.Log.FileOrFolder(Public.LogType.CreatFolder, play);
            }

            return true;
        }



        private void GeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettingsGrid.Visibility = Visibility.Visible;
            SoundSettingsGird.Visibility = Visibility.Hidden;
            VideoSettingsGrid.Visibility = Visibility.Hidden;
            GeneralSettings.Background = Brushes.DarkGray;
            SoundSettings.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F6F7"));
            VideoSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F6F7"));
        }

        private void SoundSettings_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettingsGrid.Visibility = Visibility.Hidden;
            SoundSettingsGird.Visibility = Visibility.Visible;
            VideoSettingsGrid.Visibility = Visibility.Hidden;
            GeneralSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F6F7"));
            SoundSettings.Background = Brushes.DarkGray;
            VideoSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F6F7"));
        }

        private void VideoSettings_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettingsGrid.Visibility = Visibility.Hidden;
            SoundSettingsGird.Visibility = Visibility.Hidden;
            VideoSettingsGrid.Visibility = Visibility.Visible;
            GeneralSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F6F7"));
            SoundSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F6F7"));
            VideoSettings.Background = Brushes.DarkGray;
        }

        private void CloseAllOldCourseCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (CloseAllOldCourseCheckBox.IsChecked==true)
            {
                isCloseAllOldCourse = true;
            }
            else
            {
                isCloseAllOldCourse = false;
            }
        }

        private void ClearBanshuCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ClearBanshuCheckBox.IsChecked==true)
            {
                isClearBanshu = true;
            }
            else
            {
                isClearBanshu = false;
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
