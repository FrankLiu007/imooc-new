using IMOOC.EduCourses.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace IMOOC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Taskbar taskBar;
        Taskbar.AppBarStates taskBarStates;       
        private bool isSaveNewCourse;
        //private bool isSaved;

        private string SaveAsDest;
        private string SaveAsDestName;
        private string SaveAsSource;
        private string SaveAsSourceName;
        private string SaveAsSourcePath;

        public EduCourses.CourseControl courseControl;

        [DllImport("user32.dll", EntryPoint = "#2507")]
        extern static bool SetAutoRotation(bool bEnable);

        private UserAppConfig appConfig;

        public MainWindow()
        {
            if (Public.HelperMethods.CheckIMOOCIsOpen())
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "当前程序已打开！");
                message.ShowDialog();
                this.Close();
                return;
            }

            InitializeComponent();         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskBar = new Taskbar();
            appConfig = new UserAppConfig();
            appConfig.UserAppConfigToUserAppConfigStatic();
            courseControl = new EduCourses.CourseControl();
            taskBarStates = taskBar.GetTaskbarState();
            this.Topmost = false;
            courseControl.mainWnd = this;
            isSaveNewCourse = false;
            Container.Children.Add(courseControl);

            InitConfig();

            try { SetAutoRotation(false); } catch { }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            exitIMOOC();
        }

        public void exitIMOOC()
        {
            if (courseControl.viewModel.IsRecord==true)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "请先停止当前新课的录制");
                message.ShowDialog();
                Public.Log.WriterLog("请先停止当前新课的录制！",courseControl.viewModel.CurrCourse.Name,courseControl.viewModel.CurrCourse.CoursePath);
                return;
            }

            if (courseControl.existIsNewCourse())
            {
                string courseName = courseControl.viewModel.CurrCourse.Name;
                string coursePath = courseControl.viewModel.CurrCourse.CoursePath;

                string Path1 = coursePath + courseName;
                if (courseControl.viewModel.CurrCourse.IsModify == true)
                {
                    if (courseControl.viewModel.CurrCourse.Name == "未命名")
                    {
                        MessageWin message = new MessageWin(MessageWinType.YesNoCancel, "当前新课未保存，是否保存？");
                        message.ShowDialog();
                        if (message.IsYes)
                        {
                            SaveAsWin saveAsWin = new SaveAsWin(courseName, coursePath);
                            saveAsWin.ShowDialog();
                            if (saveAsWin.isSaveAs)
                            {
                                SaveAsDestName = saveAsWin.SaveCourseName;
                                SaveAsSource = Path1;
                                SaveAsSourceName = courseName;
                                SaveAsSourcePath = coursePath;
                                SaveAsDest = saveAsWin.SavePath + saveAsWin.SaveCourseName;
                                isSaveNewCourse = true;
                            }
                            
                        }
                        else if (message.IsCancel)
                        {
                            return;
                        }
                        else
                        {
                            //isSaved = false;
                        }
                    }
                    courseControl.viewModel.saveCourse();
                }
            }

            var nameCollection = GetSaveCourseNameCollection();
            if (nameCollection.Count>0)
            {
                SaveOldCourseWin saveOldCourseWin = new SaveOldCourseWin(nameCollection);
                saveOldCourseWin.ShowDialog();
                if (saveOldCourseWin.IsSave)
                {
                    SaveAllCourse(saveOldCourseWin.CourseNameList);
                }
                else if (saveOldCourseWin.IsCancel)
                {
                    return;
                }
            }
            
            Close();

            //之前的关闭逻辑，已注销
            #region
            //if (!courseControl.existIsNewCourse())
            //{
            //    isSaved = true;
            //    Close();
            //    return;
            //}
            //else
            //{
            //    string courseName = courseControl.viewModel.CurrCourse.Name;
            //    string coursePath = courseControl.viewModel.CurrCourse.CoursePath;

            //    string Path1 = coursePath + courseName;               
            //    if (Directory.Exists(Path1))
            //    {
            //        if (!File.Exists(Path1 + "\\" + courseName + ".course")
            //             && !File.Exists(Path1 + "\\play\\" + courseName + ".json"))
            //        {
            //            if (CheckCourseContent(courseControl.viewModel.CurrCourse))
            //            {
            //                MessageWin message = new MessageWin(MessageWinType.YesNoCancel, "当前新课未保存，是否保存？");
            //                message.ShowDialog();
            //                if (message.IsYes)
            //                {
            //                    SaveAsWin saveAsWin = new SaveAsWin(courseName, coursePath);
            //                    saveAsWin.ShowDialog();
            //                    if (saveAsWin.isSaveAs)
            //                    {
            //                        SaveAsDest = saveAsWin.SavePath + saveAsWin.SaveCourseName;
            //                        isSaveNewCourse = true;
            //                    }
                                
            //                    Close();
            //                }
            //                else if (message.IsCancel)
            //                {
            //                    return;
            //                }
            //                else
            //                {
            //                    isSaved = false;
            //                    Close();
            //                }
            //            }
            //            else
            //            {
            //                isSaved = false;
            //                Close();
            //            }
            //        }
            //        else
            //        {
            //            isSaved = true;
            //            Close();
            //        }
            //    }
            //    else
            //    {
            //        MessageWin message = new MessageWin(MessageWinType.Error, "当前课程文件被意外删除，程序即将关闭!");
            //        message.ShowDialog();
            //        Public.Log.WriterLog("当前课程文件被意外删除，程序即将关闭！", courseControl.viewModel.CurrCourse.Name, courseControl.viewModel.CurrCourse.CoursePath);
            //        isSaved = true;
            //        Close();
            //    }
            //}
            #endregion

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveConfig();
            if (isSaveNewCourse == true)
            {
                //string CourseName = Path.GetFileNameWithoutExtension(SaveAsSource);
                //string CoursePath = courseControl.viewModel.CurrCourse.CoursePath;
                //courseControl.viewModel.saveCourse();
                //string sourceFolder = CoursePath + CourseName;

                try
                {
                    Public.HelperMethods.CopyFolder(SaveAsSource, SaveAsDest);
                    if (File.Exists(SaveAsDest+"\\"+SaveAsSourceName+".course"))
                    {
                        File.Move(SaveAsDest + "\\" + SaveAsSourceName + ".course",SaveAsDest + "\\" + SaveAsDestName + ".course");
                    }

                    DeleteCourseFolder(SaveAsSourcePath, SaveAsSourceName);
                }
                catch (Exception ex)
                {
                    MessageWin message = new MessageWin(MessageWinType.Error, "另存该课程时发生错误!");
                    message.ShowDialog();
                    Public.Log.WriterLog(ex.Message + "复制课程另存为时发生错误！" + SaveAsDest, SaveAsSourceName, SaveAsSourcePath);
                }
            }
            else
            {
                DeleteCourseFolder(SaveAsSourcePath, SaveAsSourceName);
            }

            //之前的保存逻辑，已注销
            #region
            //if (!isSaved)
            //{
            //    string CourseName = courseControl.viewModel.CurrCourse.Name;
            //    string CoursePath = courseControl.viewModel.CurrCourse.CoursePath;

            //    if (isSaveNewCourse == true)
            //    {
            //        courseControl.viewModel.saveCourse();
            //        string sourceFolder = CoursePath + CourseName;

            //        try
            //        {
            //            Public.HelperMethods.CopyFolder(sourceFolder, SaveAsDest);
            //            DeleteCourseFolder(CoursePath, CourseName);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageWin message = new MessageWin(MessageWinType.Error, "另存该课程时发生错误!");
            //            message.ShowDialog();
            //            Public.Log.WriterLog(ex.Message + "复制课程另存为时发生错误！" + SaveAsDest, CourseName, CoursePath);
            //        }                                                   
            //    }
            //    else
            //    {
            //        DeleteCourseFolder(CoursePath, CourseName);
            //    }
            //}
            #endregion

            try {  SetAutoRotation(true); }  catch {  }
            taskBar.SetTaskbarState(taskBarStates);
            Public.HelperMethods.KillProcess("node_shanyun");
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void DeleteCourseFolder(string CoursePath,string CourseName)
        {
            try
            {
                if ((CoursePath != null) && (CourseName != null))
                {
                    string path1 = CoursePath + CourseName;
                    Public.HelperMethods.DeleteFolder(path1);
                    Public.Log.FileOrFolder(Public.LogType.DeleteFolder, path1);
                }
            }
            catch (Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "删除课程文件时发生错误!");
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "删除课程文件时发生错误！", CourseName, CoursePath);
            }
        }

        private void InitConfig()
        {
            string Path1 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\善而教\\AppConfig";
            string JsonPath = Path1 + "\\AppConfig.json";

            if (Directory.Exists(Path1) == false)
            {
                Directory.CreateDirectory(Path1);
                Public.Log.FileOrFolder(Public.LogType.CreatFolder, Path1);
            }

            //读取appConfig文件
            #region
            if (File.Exists(JsonPath))
            {
                try
                {
                    var js = new JavaScriptSerializer();
                    js.MaxJsonLength = 52428800;

                    appConfig = js.Deserialize<UserAppConfig>(File.ReadAllText(JsonPath));
                    appConfig.UserAppConfigToUserAppConfigStatic();

                }
                catch (Exception ex)
                {
                    Public.Log.WriterLog("反序列化AppConfig文件，生产配置文件时发生错误   " + JsonPath +ex.Message, courseControl.viewModel.CurrCourse.Name, courseControl.viewModel.CurrCourse.CoursePath);
                }
                
            }
            else
            {
                try
                {
                    var js = new JavaScriptSerializer();
                    js.MaxJsonLength = 52428800;
                    var jsonString = js.Serialize(appConfig);
                    File.WriteAllText(JsonPath, jsonString);
                }
                catch (Exception ex)
                {
                    Public.Log.WriterLog("序列化AppConfig文件时发生错误   " + JsonPath + ex.Message, courseControl.viewModel.CurrCourse.Name, courseControl.viewModel.CurrCourse.CoursePath);
                }
            }
            #endregion
            
        }

        private void SaveConfig()
        {       
            string JsonPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\善而教\\AppConfig\\AppConfig.json";
            try
            {
                UserAppConfig appConfig = new UserAppConfig();
                appConfig.UserAppConfigStaticToUserAppConfig();

                //开始序列化
                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 52428800;
                var jsonString = js.Serialize(appConfig);
                File.WriteAllText(JsonPath, jsonString);
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog("序列化AppConfig文件时发生错误   " + JsonPath + ex.Message, courseControl.viewModel.CurrCourse.Name, courseControl.viewModel.CurrCourse.CoursePath);
            }

        }


        private bool CheckCourseContent(EduCourses.Course course)
        {
            foreach (var item in course.AllPage)
            {
                if (item.Strokes.Count>0||item.AllChild.Count>0)
                {
                    return true;
                }
            }
            if (course.AllPPT.Count>0)
            {
                return true;
            }

            return false;
        }

        private ObservableCollection<CourseNameListItem> GetSaveCourseNameCollection()
        {
            ObservableCollection<CourseNameListItem> collection = new ObservableCollection<CourseNameListItem>();
            foreach (var item in courseControl.viewModel.AllCourse)
            {
                if (item.IsNew==false&&item.IsModify==true&&item.Name!="未命名")
                {
                    collection.Add(new CourseNameListItem() { Name =item.Name});
                }
            }
            return collection;
        }

        private bool SaveAllCourse(ObservableCollection<CourseNameListItem> nameCollection)
        {
            try
            {
                foreach (var name in nameCollection)
                {
                    for (int i = 0; i < courseControl.viewModel.AllCourse.Count; i++)
                    {
                        if (courseControl.viewModel.AllCourse[i].Name==name.Name)
                        {
                            courseControl.viewModel.CurrCourseIndex = i;
                            courseControl.viewModel.saveCourse();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog("保存所有旧课课程文件是时发生错误" +ex.Message);
                return false;
            }

            return true;
            
        }

    }
}
