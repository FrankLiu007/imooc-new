using DigitalOfficePro.Html5PointSdk;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IMOOC.EduCourses.Utils
{
    /// <summary>
    /// Interaction logic for PPTConvert.xaml
    /// </summary>
    public partial class PPTConvert : Window
    {

        List<string> PPTNames;
        private ObservableCollection<PPTConvertInfo> pptConvertList;
        PresentationConverter pptConverter;
        Task task;
        int slideCount;
        string printPath;
        int ConvertIndex;
        public ProgressWindow progressWnd;
        public CoursesViewModel viewModel;

        public bool ISAllConverted { get; set; }

        public PPTConvert()
        {
            InitializeComponent();
            InitPPTConvertWnd();
        }

        public PPTConvert(PPTConvertInfo pptInfo)
        {
            InitializeComponent();
            InitPPTConvertWnd();
            PPTNames.Add(pptInfo.PPTName);
            PrintPathTextBox.Text = Path.GetDirectoryName(pptInfo.PPTName);
            pptInfo.PPTName = Path.GetFileName(pptInfo.PPTName);
            pptConvertList.Add(pptInfo);            
            PPTInfoListBox.ItemsSource = pptConvertList;
        }

        private void InitPPTConvertWnd()
        {
            PPTNames = new List<string>();
            pptConvertList = new ObservableCollection<PPTConvertInfo>();
            pptConverter = InitConverter();

            ISAllConverted = true;
            ConvertIndex = 0;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            progressWnd = new ProgressWindow(this);
            progressWnd.Topmost = true;
            progressWnd.Show();
        }

        private void OpenPPTBrower_Click(object sender, RoutedEventArgs e)
        {
            PPTNames.Clear();
            var ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = ".ppt;.pptx;.pps;.ppsx|*.ppt;*.pptx;*.pps";

            if (ofd.ShowDialog() == true)
            {
                pptConvertList.Clear();
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    if (!PPTNames.Contains(ofd.FileNames[i]))
                    {
                        pptConvertList.Add(new PPTConvertInfo
                        {
                            Number = i + 1,
                            PPTName = System.IO.Path.GetFileName(ofd.FileNames[i]),
                            State = "去除"
                        });
                        PPTNames.Add(ofd.FileNames[i]);
                    }                    
                }

                if (PrintPathTextBox.Text == "")
                {
                    PrintPathTextBox.Text = System.IO.Path.GetDirectoryName(ofd.FileName);
                }
                PPTInfoListBox.ItemsSource = pptConvertList;
            }
        }

        private void SetPrintPath_Click(object sender, RoutedEventArgs e)
        {
            //var item = pptConvertList[0];
            //item.State = "56%";
            //pptConvertList.RemoveAt(0);
            //pptConvertList.Insert(0, item);                         
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择输出文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                PrintPathTextBox.Text = dialog.SelectedPath;
            }

        }

        private void ConvertPPT_Click(object sender, RoutedEventArgs e)
        {
            if (PPTNames == null)
            {
                return;
            }
            if (PrintPathTextBox.Text == "")
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "请选择文件输出位置!");
                message.ShowDialog();
                Public.Log.WriterLog("请选择文件输出位置！");
                return;
            }
            else
            {
                try
                {
                    if (!Directory.Exists(PrintPathTextBox.Text))
                    {
                        Directory.CreateDirectory(PrintPathTextBox.Text);
                        Public.Log.FileOrFolder(Public.LogType.CreatFolder, PrintPathTextBox.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "创建输出文件夹时发生错误!");
                    message.ShowDialog();
                    Public.Log.WriterLog("创建输出文件夹时发生错误！" + ex.Message);
                }
            }

            for (int i = 0; i < pptConvertList.Count; i++)
            {
                if (pptConvertList[i].State=="去除")
                {
                    pptConvertList[i].State = "等待";
                }
            }

            PPTInfoListBox.ItemsSource = null;//因为ItemsSource中item改变的内容不更新
            PPTInfoListBox.ItemsSource = pptConvertList;

            for (int i = 0; i < PPTNames.Count(); i++)
            {
                string a = Path.GetExtension(PPTNames[i]);
                if (!(Path.GetExtension(PPTNames[i]).ToLower() == ".ppt" || Path.GetExtension(PPTNames[i]).ToLower() == ".pptx"))
                {
                    MessageBoxResult re = MessageBox.Show(PPTNames[i] + "不是PPT文件，是否跳过", "注意！",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                    if (re == MessageBoxResult.Yes)
                    {
                        PPTNames.RemoveAt(i);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            printPath = PrintPathTextBox.Text;
            task = new Task(new Action(Convert));
            task.Start();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ReleasePresentationConverter();
            progressWnd.Close();
            this.Close();
            viewModel.CourseCtrl.convert = null;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            PrintPathTextBox.Text = "";
            pptConvertList.Clear();
        }

        private PresentationConverter InitConverter()
        {
            var presentationConverter = new PresentationConverter();
            presentationConverter.Settings.Player.Path = @"C:\Program Files (x86)\DigitalOfficePro\HTML5PointSDK\Players\WithoutControls";
            presentationConverter.OnPresentationReadProgress += PresentationConverter_OnPresentationReadProgress;
            presentationConverter.OnHtml5WriteProgress += PresentationConverter_OnHtml5WriteProgress;
            //presentationConverter.OnConversionEnd += PresentationConverter_OnConversionEnd;

            return presentationConverter;
        }
        private void ReleasePresentationConverter()
        {
            //-----------------------------------------------------------------------
            //Release _presentationConverter object's event
            pptConverter.OnPresentationReadProgress -= PresentationConverter_OnPresentationReadProgress;
            pptConverter.OnHtml5WriteProgress -= PresentationConverter_OnHtml5WriteProgress;
            //pptConverter.OnConversionEnd -= PresentationConverter_OnConversionEnd;
            //-----------------------------------------------------------------------
        }

        private void Convert()
        {
            ISAllConverted = false;
            
            Dispatcher.Invoke(new Action(() =>
            {
                ConvertBtn.IsEnabled = false; 
                ConvertBtn.Content = "进行中";
                progressWnd.StatusInfo.Content = "进行中";
                ExitButton.IsEnabled = false;
                ClearButton.IsEnabled = false;
            }));
            foreach (var item in PPTNames)
            {
                try
                {                    
                    ConvertIndex = PPTNames.IndexOf(item);
                    if (pptConvertList[ConvertIndex].State=="打开"|| pptConvertList[ConvertIndex].State == "错误")
                    {
                        continue;
                    }
                    var name = Path.GetFileNameWithoutExtension(item).Replace(" ", "");
                    string path = printPath + "\\" + name;
                    pptConverter.OpenPresentation(item);
                    pptConverter.Convert(path + "\\" + name + ".html");
                    pptConverter.ClosePresentation();

                    try
                    {
                        var sw = new StreamWriter(path + "\\" + name + ".txt");
                        sw.WriteLine(slideCount);
                        sw.Flush();
                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        Public.Log.WriterLog("写出PPT页数文件" + path + "\\" + name + ".txt" + ex.Message);
                    }


                    Public.HelperMethods.ZipFolder(path, printPath + "\\" + name + ".zh");
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            Public.HelperMethods.DeleteFolder(path);
                            Public.Log.FileOrFolder(Public.LogType.DeleteFolder, path);
                        }
                        catch (Exception ex)
                        {
                            Public.Log.WriterLog("删除临时转换文件时发生错误\n" + ex.Message);
                            MessageWin message = new MessageWin(MessageWinType.Prompt, "删除临时转换文件时发生错误");
                            message.ShowDialog();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var PPTInfoItem = pptConvertList[ConvertIndex];
                        PPTInfoItem.State = "错误";
                        pptConvertList.RemoveAt(ConvertIndex);
                        pptConvertList.Insert(ConvertIndex, PPTInfoItem);
                    }));
                    MessageBoxResult re = MessageBox.Show(ex.Message + item + "\n转换时发出错误，是否继续下一个PPT", "注意！",
                             MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                    if (re == MessageBoxResult.Yes)
                    {

                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            ISAllConverted = true;
            Dispatcher.Invoke(new Action(() =>
            {
                ConvertBtn.IsEnabled = true;
                ConvertBtn.Content = "转换";
                progressWnd.StatusInfo.Content = "完成";
                ExitButton.IsEnabled = true;
                ClearButton.IsEnabled = true;
            }));
        }

        private void PresentationConverter_OnConversionEnd(ConversionStatus status)
        {
            //To show conversion completed message
            Dispatcher.Invoke(new Action(() =>
            {
                switch (status)
                {
                    case ConversionStatus.Success:
                        var item = pptConvertList[ConvertIndex];
                        item.State = "打开";
                        pptConvertList.RemoveAt(ConvertIndex);
                        pptConvertList.Insert(ConvertIndex, item);
                        break;
                    case ConversionStatus.Failed:
                        var item1 = pptConvertList[ConvertIndex];
                        item1.State = "出错";
                        pptConvertList.RemoveAt(ConvertIndex);
                        pptConvertList.Insert(ConvertIndex, item1);
                        break;
                    case ConversionStatus.Cancelled:
                        break;
                }

            }));
            //-----------------------------------------------------------------------
        }

        private void PresentationConverter_OnHtml5WriteProgress(int slideIndex, int itemIndex, float writeProgress)
        {
            var str0 = writeProgress.ToString();

            Dispatcher.Invoke(new Action(() =>
            {
                slideCount = slideIndex;
                if (str0=="100")
                {
                    var item = pptConvertList[ConvertIndex];
                    item.State = "打开";
                    pptConvertList.RemoveAt(ConvertIndex);
                    pptConvertList.Insert(ConvertIndex, item);
                }
                else
                {
                    var item = pptConvertList[ConvertIndex];
                    item.State = "输出" + str0 + "%";
                    pptConvertList.RemoveAt(ConvertIndex);
                    pptConvertList.Insert(ConvertIndex, item);
                }                
            }));    
        }

        private void PresentationConverter_OnPresentationReadProgress(int slideIndex, int itemIndex, float readProgress)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var item = pptConvertList[ConvertIndex];
                item.State = "读取" + readProgress.ToString() + "%";
                pptConvertList.RemoveAt(ConvertIndex);
                pptConvertList.Insert(ConvertIndex, item);             
            }));
        }

        private void KillBackgroundPPT()
        {

            Process[] myProgress;
            myProgress = Process.GetProcesses();　　　　　　　　　　//获取当前启动的所有进程
            foreach (Process p in myProgress)　　　　　　　　　　　　//关闭当前启动的Excel进程
            {
                if (p.ProcessName == "POWERPNT")　　　　　　　　　　//通过进程名来寻找
                {
                    p.Kill();
                    return;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            KillBackgroundPPT();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lable = sender as Label;
            string selectedPPTName = Path.GetFileNameWithoutExtension(pptConvertList[(Int32)lable.Tag - 1].PPTName).Replace(" ", "");

            if (lable.Content.ToString() == "去除")
            {
                pptConvertList.RemoveAt((Int32)lable.Tag - 1);
                for (int i = 0; i < pptConvertList.Count; i++)
                {
                    pptConvertList[i].Number = i + 1;
                }
                PPTInfoListBox.ItemsSource=null;//因为ItemsSource中item改变的内容不更新
                PPTInfoListBox.ItemsSource = pptConvertList;
                PPTNames.RemoveAt((Int32)lable.Tag - 1);
            }
            else if (lable.Content.ToString()=="打开")
            {                
                string str1 = PrintPathTextBox.Text + "\\" + selectedPPTName + ".zh";
                string str2 = viewModel.CurrCourse.CoursePath + viewModel.CurrCourse.Name + "\\";
                try
                {
                    File.Copy(str1, str2 + selectedPPTName + ".zh", true);
                    Public.Log.FileOrFolder(Public.LogType.CopeFile, str1 + "  to  " + str2);
                }
                catch (Exception ex)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "复制转换文件至课程文件夹时发生错误！");
                    message.ShowDialog();
                    Public.Log.WriterLog("复制转换文件至课程文件夹时发生错误！" + ex.Message);
                    Public.Log.FileOrFolder(Public.LogType.CopeFile, str2 + selectedPPTName + ".zh");
                }
                               
                Public.HelperMethods.UnZipFolder(str1, str2 + "\\Resource\\ppt");

                viewModel.PPTMenu();
                viewModel.showhtml(str2 + "\\Resource\\ppt\\" + selectedPPTName + "\\" + selectedPPTName + ".html");
            }
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// 判断转换列表中是否已经包含该PPT
        /// </summary>
        /// <param name="pptName"></param>
        /// <returns></returns>
        public bool isPPTSContain(string pptName)
        {
            foreach (var item in pptConvertList)
            {
                if (item.PPTName == pptName)
                {
                    return true;
                }                
            }
            return false;
        }

        public void AddPPT(string pptName)
        {
            PPTNames.Add(pptName);
            pptConvertList.Add(new PPTConvertInfo
            {
                Number = PPTNames.Count,
                PPTName = Path.GetFileName(pptName),
                State = "等待"
            });
        }
    }

    public class PPTConvertInfo
    {
        public PPTConvertInfo()
        {
            Number = -1;
            PPTName = "NULL";
            State = "初始";
        }
        public int Number { get; set; }
        public string PPTName { get; set; }
        public string State { get; set; }
    }
}