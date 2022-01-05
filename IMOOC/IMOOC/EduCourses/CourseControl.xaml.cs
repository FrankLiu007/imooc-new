using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using IMOOC.EduCourses.Utils;
using PPT = Microsoft.Office.Interop.PowerPoint;
using System.Windows.Data;
using Ionic.Zip;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Diagnostics;
using System.Windows.Resources;
using Public;

namespace IMOOC.EduCourses
{
    /// <summary>
    /// Interaction logic for CourseControl.xaml
    /// </summary>
    public partial class CourseControl : UserControl
    {
        private StrokeCollection copiedStrokes;
        private InkCanvasChildCollection copiedChilds;
        public MainWindow mainWnd;
        public CoursesViewModel viewModel;

        public CourseMenuTool courseMenuTool;
        public Edit.EditTool editTool;
        public Insert.InsertTool insertTool;
        public PPTModule.PptTool pptTool;
        public BanshuTool banshuTool;
        public RecourdTool recourdTool;
        public Set.SetTool setTool;

        private InkCanvasPage temporaryPage;
        public Dictionary<string, List<StrokeCollection>> temporaryOldCoursePPTStrokes;

        /// <summary>
        /// 判断录制前是否处于板书
        /// </summary>
        public bool IsBanshu { get; set; }
        public static string CourseSavePath; 

        public string OldCourseName;
        Cursor custom32Cursor;
        Cursor custom64Cursor;
        public int PicCount;//用于截图区分
        public Recorder.MediaRecorder mediaRecorder;
        public bool IsInitRecorder;
        public PPTConvert convert;

        public bool IsWin7OrAbove { get; set; }

        TransformGroup transformGroup;
        ScaleTransform scaleTransform;
        TranslateTransform translateTransform;

        public Brush InkCansBackgrand;
        public string copyPageIndex;
        private Rect rect;
        private DateTime interval;

        public CourseControl()
        {
            InitializeComponent();

            viewModel = new CoursesViewModel();
            DataContext = viewModel;
            rect = new Rect();

            courseMenuTool = new CourseMenuTool();
            editTool = new Edit.EditTool();
            insertTool = new Insert.InsertTool();
            pptTool = new PPTModule.PptTool();
            banshuTool = new BanshuTool();
            recourdTool = new RecourdTool();
            setTool = new EduCourses.Set.SetTool();

            temporaryPage = new InkCanvasPage();
            temporaryOldCoursePPTStrokes = new Dictionary<string, List<StrokeCollection>>();
            PicCount = 0;
            IsBanshu = true;
            IsInitRecorder = false;

            copiedChilds = new InkCanvasChildCollection();
            transformGroup = new TransformGroup();
            scaleTransform = new ScaleTransform();
            translateTransform = new TranslateTransform();            

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ToolGrid.Children.Add(editTool);
            ToolGrid.Children.Add(insertTool);
            ToolGrid.Children.Add(pptTool);
            ToolGrid.Children.Add(banshuTool);
            ToolGrid.Children.Add(courseMenuTool);
            ToolGrid.Children.Add(recourdTool);
            ToolGrid.Children.Add(setTool);

            courseMenuTool.Visibility = Visibility.Visible;
               
            viewModel.CourseCtrl = this;
            viewModel.CustomInkCan = myInkCanvas;
            indicater.inkcanvas = myInkCanvas;

            IsWin7OrAbove = isWin7OrAbove();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            //createWinSaveFilesOptions(true);
            creatUnNameCourse();

            StreamResourceInfo sri32 = Application.GetResourceStream(new Uri(@"\Images\pen32.cur", UriKind.Relative));
            custom32Cursor = new Cursor(sri32.Stream);
            StreamResourceInfo sri64 = Application.GetResourceStream(new Uri(@"\Images\pen64.cur", UriKind.Relative));
            custom64Cursor = new Cursor(sri64.Stream);

            SetCursor(true);            
        }

        //-------------------------------------------万恶的分割线---------------------------------------------------------
        public void GotoBanshu()
        {
            myInkCanvas.EditingMode = banshuTool.MyinkEditMd;
            if (banshuTool.isHighlighted)
            {
                myInkCanvas.DefaultDrawingAttributes = banshuTool.highlightDA;
            }
            else
            {
                myInkCanvas.DefaultDrawingAttributes = banshuTool.inkDA;
            }
            hiddenAllTool();
            banshuTool.Visibility = Visibility.Visible;
            myInkCanvas.Background = InkCansBackgrand;
            BanshuMenu.IsChecked = true;
            IsBanshu = true;
        }

        public void GotoPPT()
        {
            myInkCanvas.EditingMode = pptTool.MyinkEditMd;
            if (pptTool.isHighlighted)
            {
                myInkCanvas.DefaultDrawingAttributes = pptTool.highlightDA;
            }
            else
            {
                myInkCanvas.DefaultDrawingAttributes = pptTool.inkDA;
            }
            hiddenAllTool();
            pptTool.Visibility = Visibility.Visible;
            if (viewModel.CurrPPT == null)
            {
                VisualBrush PPTBack = new VisualBrush();
                PPTBack.Stretch = Stretch.None;
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Desert.png"));
                PPTBack.Visual = image;
                myInkCanvas.Background = PPTBack;
            }
            else
            {
                myInkCanvas.Background = Brushes.Transparent;
            }
            IsBanshu = false;
        }

        public void GotoEdit()
        {
            hiddenAllTool();
            editTool.Visibility = Visibility.Visible;
        }

        public void GotoInsert()
        {
            hiddenAllTool();
            insertTool.Visibility = Visibility.Visible;
        }

        public void GotoRecourdMenu()
        {
            hiddenAllTool();
            recourdTool.Visibility = Visibility.Visible;            
        }

        public void GotoCoursMenu()
        {
            hiddenAllTool();
            courseMenuTool.Visibility = Visibility.Visible;
            CourseMenu.IsChecked = true;
        }

        public void GoToSetMenu()
        {
            hiddenAllTool();
            setTool.Visibility = Visibility.Visible;
        }

        private void hiddenAllTool()
        {
            courseMenuTool.Visibility = Visibility.Hidden;
            insertTool.Visibility = Visibility.Hidden;
            banshuTool.Visibility = Visibility.Hidden;
            pptTool.Visibility = Visibility.Hidden;
            editTool.Visibility = Visibility.Hidden;
            recourdTool.Visibility = Visibility.Hidden;
            setTool.Visibility = Visibility.Hidden;
        }

        public void InitIsEnabled()
        {
            if (viewModel.CurrCourse.IsNew)
            {
                if (viewModel.IsRecord==false)
                {
                    recourdTool.StartRecord.IsEnabled = true;
                }
                else
                {
                    recourdTool.StartRecord.IsEnabled = false;
                }
                
                banshuTool.CreateNewPage.IsEnabled = true;
                pptTool.OpenPPT.IsEnabled = true;
                pptTool.jietu.IsEnabled = true;
                if (!viewModel.CurrCourse.IsSaved)
                {
                    courseMenuTool.OpenNewCourseBtn.IsEnabled = false;
                }              
                insertTool.insertPic.IsEnabled = true;
            }
            else
            {
                recourdTool.StartRecord.IsEnabled = false;
                banshuTool.CreateNewPage.IsEnabled = false;
                pptTool.OpenPPT.IsEnabled = false;
                pptTool.jietu.IsEnabled = false;
                insertTool.insertPic.IsEnabled = false;
                //courseMenuTool.OpenNewCourseBtn.IsEnabled = true;
            }
        }

        //-------------------------------------------万恶的分割线---------------------------------------------------------

        public void ButtonPasteSelection()
        {
            if (copiedStrokes == null)
            {
                return;
            }
            StrokeCollection Strokesclone = copiedStrokes.Clone();
            var childs = copiedChilds.Clone();
            viewModel.paste();
            if (copiedStrokes != null)
            {
                Matrix m = Matrix.Identity;
                m.Translate(20, 0);
                Strokesclone.Transform(m, false);
                myInkCanvas.Strokes.Add(Strokesclone);
                myInkCanvas.Select(Strokesclone);

            }
            myInkCanvas.AllChild.Add(childs);
            var UiElmentClone = new ObservableCollection<UIElement>();
            for (int i = 0; i < childs.Count; i++)
            {
                UiElmentClone.Add(childs[i].UiEle);
            }

            myInkCanvas.Select(Strokesclone, UiElmentClone);
        }

        public void ButtonCutSelection()
        {
            copiedChilds.Clear();
            copiedStrokes = myInkCanvas.GetSelectedStrokes();
            myInkCanvas.Strokes.Remove(copiedStrokes);

            var se = myInkCanvas.GetSelectedElements();
            int seCount = se.Count;
            while (seCount > 0)
            {
                if (se[seCount - 1] is FrameworkElement)
                {
                    var inkChild = (se[seCount - 1] as FrameworkElement).DataContext as InkCanvasChild;
                    copiedChilds.Add(inkChild);
                    myInkCanvas.AllChild.Remove(inkChild);
                    seCount--;
                }

            }
            updataIndicater();
        }


        public void ButtonCopySelection()
        {
            copyPageIndex = viewModel.CurrCourse.Name + "_" + viewModel.CurrPageIndex.ToString();
            copiedChilds.Clear();
            copiedStrokes = myInkCanvas.GetSelectedStrokes();
            var se = myInkCanvas.GetSelectedElements();
            for (int i = 0; i < se.Count; i++)
            {
                copiedChilds.Add((se[i] as FrameworkElement).DataContext as InkCanvasChild);
            }
        }

        private void FastSwitch(object sender, RoutedEventArgs e)
        {

        }

        public void OpenImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = ".jpg;.png|*.jpg;*.png";

                if (ofd.ShowDialog() == true)
                {
                    ImageSource image = new BitmapImage(new Uri(ofd.FileName));
                    Image img = new Image();
                    img.Source = image;
                    img.Stretch = Stretch.Fill;
                    var tt = new InkCanvasChild() { X = 100, Y = 100, UiEle = img };
                    myInkCanvas.AllChild.Add(tt);
                }
            }
            catch(Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "未能获取到图片！");
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "未能获取到图片！",viewModel.CurrCourse.Name,viewModel.CurrCourse.CoursePath);
            }

        }

        //-----------------undo,redo---------------------------------

        private void indicater_DragCompleted(object sender, IndicaterDragCompletedEventArgs e)
        {
            viewModel.SelectionMovingOrResizing(e);
        }

        public void Undo()
        {
            if (viewModel.CurrCourse.IsNew==false)
            {
                return;
            }
            viewModel.undo();
            updataIndicater();
        }

        public void Redo()
        {
            if (viewModel.CurrCourse.IsNew == false)
            {
                return;
            }
            viewModel.redo();
            updataIndicater();
        }

        //--------------------------------------------------------------

        public void myInkCanvas_SelectionChanged(object sender, EventArgs e)
        {
            updataIndicater();
        }

        private void updataIndicater()
        {
            if (rect == myInkCanvas.GetSelectionBounds())
            {
                return;
            }
            rect = myInkCanvas.GetSelectionBounds();
            if (double.IsInfinity(rect.Width))
            {
                indicater.updataIndicater(new Thickness(0, 0, 0, 0), 0, 0);
                viewModel.Updataindicater(0, 0, 0, 0);
            }
            else
            {
                indicater.updataIndicater(new Thickness(rect.Left - 28, rect.Top - 28,
                    myInkCanvas.ActualWidth - rect.Left - rect.Width - 28,
                    myInkCanvas.ActualHeight - rect.Top - rect.Height - 28),
                    rect.Width + 56, rect.Height + 56,
                    myInkCanvas.GetSelectedStrokes(), myInkCanvas.GetSelectedElements());
                indicater.Visibility = Visibility.Visible;
                viewModel.Updataindicater(rect.Left - 28, rect.Top - 28,
                    rect.Width + 56, rect.Height + 56);
            }
        }

        private void scroll_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (scaleTransform.ScaleX < 1.0)
                {
                    scaleTransform.ScaleX = 1.0;
                    scaleTransform.ScaleY = 1.0;
                    translateTransform.X = 0.0;
                    translateTransform.Y = 0.0;
                    return;
                }
                if (e.Delta > 0)
                {
                    scaleTransform.ScaleX *= 1.05;
                    scaleTransform.ScaleY *= 1.05;
                }
                else if (e.Delta < 0)
                {
                    scaleTransform.ScaleX *= 0.95;
                    scaleTransform.ScaleY *= 0.95;
                }

                Point cursorPos = e.GetPosition(inkcanvasGrid);
                Point newCenter = scaleTransform.Inverse.Transform(translateTransform.Inverse.Transform(cursorPos));
                Point oldCenter = new Point(scaleTransform.CenterX, scaleTransform.CenterY);
                Vector oldToNewCenter = newCenter - oldCenter;
                scaleTransform.CenterX = newCenter.X;
                scaleTransform.CenterY = newCenter.Y;
                translateTransform.X += oldToNewCenter.X * (scaleTransform.ScaleX - 1.0);
                translateTransform.Y += oldToNewCenter.Y * (scaleTransform.ScaleY - 1.0);


            }
        }

        //-----------------------------------------------------------------------------------------------


        private void EditMenu_Click(object sender, RoutedEventArgs e)
        {
            GotoEdit();
        }

        private void InsertMenu_Click(object sender, RoutedEventArgs e)
        {
            GotoInsert();
        }

        private void CourseMenu_Click(object sender, RoutedEventArgs e)
        {
            GotoCoursMenu();
        }

        private void RecourdMenu_Click(object sender, RoutedEventArgs e)
        {
            GotoRecourdMenu();
            if (viewModel.CurrCourse.IsNew==false)
            {
                recourdTool.StartRecord.IsEnabled = false;
                recourdTool.EndRecord.IsEnabled = false;
                recourdTool.PauseRecord.IsEnabled = false;
                recourdTool.LocadPlay.IsEnabled = true;
            }
            else
            {
                if (viewModel.IsRecord)
                {
                    recourdTool.StartRecord.IsEnabled = false;
                    recourdTool.EndRecord.IsEnabled = true;
                    recourdTool.PauseRecord.IsEnabled = true;
                    recourdTool.LocadPlay.IsEnabled = true;
                }
                else
                {
                    recourdTool.StartRecord.IsEnabled = true;
                    recourdTool.EndRecord.IsEnabled = false;
                    recourdTool.PauseRecord.IsEnabled = false;
                    recourdTool.LocadPlay.IsEnabled = true;
                }                
            }
        }       

        //--------------------StartCourse------------------------------------------------------------------------
        public void StartOldCourse(string name, string OpenOldPath)
        {
            Course course = OpenOldCourse(OpenOldPath);
            course.Name = name;

            var path = Path.GetDirectoryName(OpenOldPath);
            path = path.Substring(0, path.Length - name.Length);

            course.CoursePath = path;
            course.IsNew = false;
            if (course == null)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "未能成功获取旧课！");
                message.ShowDialog();
                Public.Log.WriterLog("未能成功获取旧课！", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                return;
            }
           
            CreateCourse(course);            
           
        }        

        public Course OpenOldCourse(string OpenOldPath)
        {
            Course oldcourse = new Course();
            temporaryOldCoursePPTStrokes.Clear();

            using (var fs = File.OpenRead(OpenOldPath))
            {
                using (var br = new BinaryReader(fs, Encoding.UTF8))
                {                      
                        //  List<InkCanvasPage> allPage;
                        //List<InkCanvasPage> addedItems;
                        var path = Path.GetDirectoryName(OpenOldPath) + "\\";
                        var pageCount = br.ReadInt32();
                        //addedItems = new List<InkCanvasPage>(pageCount);
                        //for (int i = 0; i < pageCount; i++)
                        //{
                        //    var page = new InkCanvasPage();
                        //    page.Strokes = new StrokeCollection(fs);
                        //    page.AllChild = new InkCanvasChildCollection(br, path);
                        //    addedItems.Add(page);
                        //}
                        ObservableCollection<InkCanvasPage> oldcoursePage = new ObservableCollection<InkCanvasPage>();
                        //   allPage = new List<InkCanvasPage>(pageCount);
                        for (int i = 0; i < pageCount; i++)
                        {
                            var page = new InkCanvasPage();
                            page.Strokes = new StrokeCollection(fs);
                            page.AllChild = new InkCanvasChildCollection(br, path+ "Resource\\picture\\");
                            //var indexLength = br.ReadInt32();
                            //    page.AllChild = new InkCanvasChildCollection();
                            //    for (int j = 0; j < indexLength; j++)
                            //    {
                            //        page.AllChild.Add(addedItems[i].AllChild[br.ReadInt32()]);
                            //    }
                            //  allPage.Add(page);
                            oldcoursePage.Add(page);
                        }

                        //-----------------------------------------------
                        var PPTCount = br.ReadInt32();
                        var pptNameList = new ObservableCollection<string>();
                        for (int i = 0; i < PPTCount; i++)
                        {
                            string pptName = br.ReadString();
                            pptNameList.Add(pptName);
                        }

                        Dictionary<string, PPTModule.PPTModel> oldAllPPT = new Dictionary<string, PPTModule.PPTModel>();                        
                        for (int i = 0; i < PPTCount; i++)
                        {
                            List<StrokeCollection> strokesList = new List<StrokeCollection>();
                            var strokeListCount = br.ReadInt32();
                            var pptKey = br.ReadString();
                            for (int j = 0; j < strokeListCount; j++)
                            {
                                strokesList.Add(new StrokeCollection(fs));
                            }                                                       
                            temporaryOldCoursePPTStrokes.Add(pptKey, strokesList);
                        }

                        oldcourse.PPTNameList = pptNameList;
                        oldcourse.AllPPT = oldAllPPT;
                        //---------------------------------------------


                        RefreshInkCanvas(oldcoursePage);
                        oldcourse.AllPage = oldcoursePage;
                        oldcourse.CurrPageIndex = 0;
                }
            }
            return oldcourse;

        }


        private void RefreshInkCanvas(ObservableCollection<InkCanvasPage> pages)
        {
            int index = 0;
            foreach (var page in pages)
            {
                foreach (var item in page.Strokes)
                {
                    item.AddPropertyData(viewModel.guid, index++);
                }

                foreach (var item in page.AllChild)
                {                    
                    item.index= index++;                    
                }

                page.HistoryMaxCount[0] = page.Strokes.Count + page.AllChild.Count;
            }
        }
        //private void UnZipAndMoveOldCoursePPT(string oldCoursePath)
        //{
        //    var pptFiles = Directory.GetFiles(Path.GetDirectoryName(oldCoursePath), "*.zh");
        //    foreach (var item in pptFiles)
        //    {
        //        System.IO.File.Copy(item, CourseControl.CourseSavePath + Path.GetFileName(item), true);
        //        Public.Log.FileOrFolder(Public.LogType.CopeFile, item + "  to  " + CourseControl.CourseSavePath + Path.GetFileName(item),viewModel.CurrCourse.Name,viewModel.CurrCourse.CoursePath);

        //        string targetPath = CourseControl.CourseSavePath + "temporary";
        //        if (!Directory.Exists(targetPath))
        //        {
        //            Directory.CreateDirectory(targetPath);
        //            Public.Log.FileOrFolder(Public.LogType.CreatFolder, targetPath, viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
        //        }
        //        Public.HelperMethods.UnZipFolder(item, targetPath);
        //    }
        //}

        public bool InspectCourseCombox(string OldCoursePath)
        {
            OldCoursePath = Path.GetFileNameWithoutExtension(OldCoursePath);
            foreach (ComboBoxItem item in courseMenuTool.AllCourseCombox.Items)
            {
                if (item.Content.ToString() == OldCoursePath)
                {
                    return true;
                }
            }
            return false;
        }

        private void PageSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            if ((index = (sender as ListBox).SelectedIndex) > -1)
                viewModel.selectPage(index);
        }

        private void PageListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = MouseWheelEvent;
            eventArg.Source = sender;
            (sender as ListBox).RaiseEvent(eventArg);
        }

        private void CreateCourse(Course course)
        {            
            viewModel.AllCourse.Add(course);
            viewModel.CurrCourseIndex = viewModel.AllCourse.Count - 1;
            CourseSavePath = course.CoursePath + course.Name + "\\";
            
            if (course.IsNew==false)
            {                
                foreach (var item in course.PPTNameList)
                {
                    viewModel.showhtml(course.CoursePath + course.Name  + "\\Resource\\ppt\\" + item + "\\" + item + ".html");
                }

                if (viewModel.IsRecord)
                {
                    viewModel.SaveAddedOldCourse(viewModel.AllCourse[viewModel.AllCourse.Count - 1]);
                }

                viewModel.allPageAddCollectionChanged(viewModel.CurrCourse);             
            }
            else
            {
                viewModel.AddCollectionChanged();
            }
             
            GotoBanshu();
            viewModel.BanshuMenu();

        }

        public bool existIsNewCourse()
        {
            for (int i = 0; i < viewModel.AllCourse.Count; i++)
            {
                if (viewModel.AllCourse[i].IsNew == true)
                {
                    courseMenuTool.AllCourseCombox.SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        private bool IsNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            foreach (char c in str)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        private void startNewCourse(string newCoursePath,string newCourseName)
        {
            if (UserAppConfigStatic.CourseName.Length > 8)
            {
                string str = UserAppConfigStatic.CourseName.Substring(0, 8);
                string number = UserAppConfigStatic.CourseName.Remove(0, 8);
                if (str == "MyCourse")
                {
                    if (IsNumber(number))
                    {
                        int Number = int.Parse(number) + 1;
                        while (true)
                        {
                            if (Directory.Exists(newCoursePath + "\\" + "MyCourse" + Number.ToString()))
                            {
                                Number = Number + 1;
                            }
                            else
                            {
                                UserAppConfigStatic.CourseName = "MyCourse" + Number.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                UserAppConfigStatic.CourseName = UserAppConfigStatic.CourseName + "1";
            }

            UserAppConfigStatic.SavePath = newCoursePath;

            //CourseSavePath = savePath ;
            //courseName = savePath + "\\" + filename + ".bv";
            try
            {
                string path2 = newCoursePath +"\\"+ newCourseName;
                if (Directory.Exists(path2))
                {
                    Public.HelperMethods.DeleteFolder(path2);
                    Public.Log.FileOrFolder(Public.LogType.DeleteFolder, newCoursePath, viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                }
                Directory.CreateDirectory(path2);
                Public.Log.FileOrFolder(Public.LogType.CreatFolder, newCoursePath, viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
            }
            catch(Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "创建该新课文件夹时发生错误！\n  请尝试更换课程名重新创建!");
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "创建该新课文件夹时发生错误！\n  请尝试更换课程名重新创建!", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                createWinSaveFilesOptions();
            }

            CreateCourse(new Course(newCourseName, newCoursePath, true));
            GotoBanshu();
            return;
        }

        public void createWinSaveFilesOptions()
        {
            List<string> allCourseName = new List<string>();
            foreach (var item in viewModel.AllCourse)
            {
                allCourseName.Add(item.Name);
            }
            WinSaveFilesOptions WinSaveFile = new WinSaveFilesOptions(UserAppConfigStatic.SavePath, UserAppConfigStatic.CourseName, allCourseName);
            WinSaveFile.Activate();
            WinSaveFile.Top = 0;
            WinSaveFile.Left = (this.ActualWidth - WinSaveFile.Width) / 2;
            WinSaveFile.ShowDialog();

            if (WinSaveFile.IsOpenNewCourse)
            {
                if (existIsNewCourse())
                {
                    viewModel.CurrCourse.IsNew = false;
                }
                UserAppConfigStatic.CourseName = WinSaveFile.CourseName;
                UserAppConfigStatic.SavePath = WinSaveFile.CoursePath;
                startNewCourse(WinSaveFile.CoursePath, WinSaveFile.CourseName);
                InitIsEnabled();
                recourdTool.StartRecord.IsEnabled = true;
            }
        }

        public void creatUnNameCourse()
        {
           var savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\"+ MyHttpWebRequest.CurrUser.UserName+"\\";
           var filename = "未命名";
            try
            {
                if (Directory.Exists(savePath+filename))
                {
                    Public.HelperMethods.DeleteFolder(savePath + filename);
                    Public.Log.FileOrFolder(Public.LogType.DeleteFolder, savePath + filename, viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                }
                Directory.CreateDirectory(savePath + filename);
                Public.Log.FileOrFolder(Public.LogType.CreatFolder, savePath + filename, viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
            }
            catch(Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "创建未命名课程文件夹时发生错误！\n            请尝试更换课程名");
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "创建未命名课程文件夹时发生错误!", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                createWinSaveFilesOptions();
            }

            CreateCourse(new Course(filename, savePath, true));
        }

        private void PPTMenu_Click(object sender, RoutedEventArgs e)
        {
            GotoPPT();            
        }

        private void BanshuMenu_Click(object sender, RoutedEventArgs e)
        {
            GotoBanshu();
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            mainWnd.WindowState = WindowState.Minimized;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {                    
            mainWnd.exitIMOOC();
        }

        private void LocalWatch_Click(object sender, RoutedEventArgs e)
        {
            //Process[] myProgress;
            //myProgress = Process.GetProcesses();　　　　　　　　　　//获取当前启动的所有进程
            //foreach (Process p in myProgress)　　　　　　　　　　　　//关闭当前启动的Excel进程
            //{
            //    if (p.ProcessName == "POWERPNT")　　　　　　　　　　//通过进程名来寻找
            //    {
            //        p.Kill();
            //        return;
            //    }
            //}

            //System.Diagnostics.Process.Start("http://localhost:8081");
        }

        private int CheckProtIsUse(int port) 
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    port++;
                    CheckProtIsUse(port);
                }
            }
            return port;
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            GoToSetMenu();            
        }

        public void FastSwitchToBanShu(string courseName)
        {
            int courseIndex = -1;
            for (int i = 0; i < viewModel.AllCourse.Count; i++)
            {
                if (viewModel.AllCourse[i].Name==courseName)
                {
                    courseIndex = i;
                    break;
                }
            }
            courseMenuTool.AllCourseCombox.SelectedIndex = courseIndex;

            GotoBanshu();
            viewModel.BanshuMenu();
        }

        private void FastSwitchToPPT(string courseName,string pptName)
        {
            FastSwitchToBanShu(courseName);
            int currPPTIndex = -1;
            currPPTIndex = viewModel.CurrCourse.PPTNameList.IndexOf(pptName);

            pptTool.PPTChange.SelectedIndex = currPPTIndex;
            //viewModel.CurrPptIndex = currPPTIndex;
            GotoPPT();
            viewModel.PPTMenu();
            PPTMenu.IsChecked = true;

        }

        private void FastSwitchMenu_Click(object sender, RoutedEventArgs e)
        {
            FastSwitchPp.IsOpen = true;
        }

        private void FastSwitchToBanShu_Click(object sender, RoutedEventArgs e)
        {
            FastSwitchToBanShu((sender as Button).Tag.ToString());
            FastSwitchPp.IsOpen = false;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedIndex==-1)
            {
                return;
            }

            string courseName = (sender as ListBox).Tag.ToString();
            string PPTName = (sender as ListBox).Items[(sender as ListBox).SelectedIndex].ToString();

            FastSwitchToPPT(courseName, PPTName);
            FastSwitchPp.IsOpen = false;

           (sender as ListBox).SelectedIndex = -1;
        }

        private void pauseGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pauseGrid.Visibility = Visibility.Hidden;           
            pauseRecoder(false);
        }

        private void pauseRecoder(bool isPause)
        {
            if (isPause)
            {
                interval = DateTime.Now;
                mediaRecorder.Pause();
            }
            else
            {
                viewModel.SetLastTime(DateTime.Now-interval);
                myInkCanvas.SetStartTime(DateTime.Now - interval);
                mediaRecorder.Start();
            }
            
        }    

        public void pauseRecourd()
        {
            pauseGrid.Visibility = Visibility.Visible;
            pauseRecoder(true);
        }

        private void ShowDesktop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWnd.WindowState = WindowState.Minimized;
        }

        public void SetCursor(bool display)
        {
            if (display && UserAppConfigStatic.BoardMode == true)
            {
                PresentationSource source = PresentationSource.FromVisual(this);
                double dpiX, dpiY;
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
                if (dpiX>1&&dpiY>1)
                {
                    grid.Cursor = custom64Cursor;
                    CoursesMenu.Cursor = custom64Cursor;
                    ToolGrid.Cursor = custom64Cursor;
                    myInkCanvas.UseCustomCursor = true;
                    myInkCanvas.Cursor = custom64Cursor;                    
                }
                else
                {
                    grid.Cursor = custom32Cursor;
                    CoursesMenu.Cursor = custom32Cursor;
                    ToolGrid.Cursor = custom32Cursor;
                    myInkCanvas.UseCustomCursor = true;
                    myInkCanvas.Cursor = custom32Cursor;
                }
                
            }
            else
            {
                grid.Cursor = Cursors.Arrow;
                CoursesMenu.Cursor = Cursors.Arrow;
                ToolGrid.Cursor = Cursors.Arrow;
                myInkCanvas.UseCustomCursor = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetCursor(false);
        }

        private void myInkCanvas_EditingModeChanged(object sender, RoutedEventArgs e)
        {
            UpDateCursor();
        }

        public void UpDateCursor()
        {
            if ((myInkCanvas.EditingMode == InkCanvasEditingMode.Ink && myInkCanvas.DefaultDrawingAttributes.IsHighlighter == false)
                || myInkCanvas.EditingMode == InkCanvasEditingMode.None)
            {
                SetCursor(true);
            }
            else
            {
                SetCursor(false);
            }
        }

        private bool isWin7OrAbove()
        {
            Version currVersion = Environment.OSVersion.Version;
            if (currVersion.Major >= 6 && currVersion.Minor >= 1)
            {
                return true;
            }
            return false;
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            UserInfoPp.IsOpen = true;
        }

        private void moreLabe_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BackToNewCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.AllCourse.Count == 1)
            {
                return;
            }
            existIsNewCourse();
        }
    }
}



