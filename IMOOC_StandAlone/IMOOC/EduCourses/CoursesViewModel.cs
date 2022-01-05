using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Ink;
using IMOOC.EduCourses.PPTModule;
using System.Collections.ObjectModel;
using IMOOC.EduCourses.Utils;
using DigitalOfficePro.Html5PointSdk;
using Microsoft.Win32;
using Awesomium.Windows.Controls;
using System.Text;
using System.Windows.Media.Imaging;
using System.Web.Script.Serialization;
using System.Windows.Media;
using System.Windows.Controls;
using Shell32;
using System.Text.RegularExpressions;

namespace IMOOC.EduCourses
{

    public partial class CoursesViewModel : NotificationObject, IDisposable
    {
        #region property and field
        private ObservableCollection<InkCanvasPage> allPage;
        /// <summary>
        /// 所有板书页面
        /// </summary>
        public ObservableCollection<InkCanvasPage> AllPage
        {
            get { return allPage; }
            set { allPage = value; }
        }


        private InkCanvasPage currPage;
        /// <summary>
        /// 当前板书页面
        /// </summary>
        public InkCanvasPage CurrPage
        {
            get { return currPage; }
            set { RaisePropertyChanged(ref currPage, value, "CurrPage"); }
        }

        private int currPageIndex;
        /// <summary>
        /// 当前板书页面在AllPage里的index
        /// </summary>
        public int CurrPageIndex
        {
            get { return currPageIndex; }
            set
            {
                RaisePropertyChanged(ref currPageIndex, value, "CurrPageIndex");
                CurrPage = AllPage[CurrPageIndex];
                CurrCourse.CurrPageIndex = CurrPageIndex;
                //注意，这里没直接注销了
                if (isRecord)
                {
                    saveData.actions.Add(new ChoosePageAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "choosePage",
                        index = currCourse.Name + "_" + value.ToString()
                    });
                    currChoosePage = currCourse.Name + "_" + value.ToString();

                }
            }
        }

        //private ObservableCollection<Course> allCourse;
        ///// <summary>
        ///// 当前所有课程
        ///// </summary>
        //public ObservableCollection<Course> AllCourse
        //{
        //    get { return allCourse; }
        //    set { RaisePropertyChanged(ref allCourse, value, "AllCourse"); }
        //}

        private bool isRecord;
        /// <summary>
        /// 指示是否在录制
        /// </summary>
        public bool IsRecord
        {
            get { return isRecord; }
            set
            {
                RaisePropertyChanged(ref isRecord, value, "IsRecord");
            }
        }


        //private ObservableCollection<AddTime> stylusTimes;
        ///// <summary>
        ///// 和custominkcanvas里的AddTimes绑定
        ///// </summary>
        //public ObservableCollection<AddTime> StylusTimes
        //{
        //    get { return stylusTimes; }
        //    set { RaisePropertyChanged(ref stylusTimes, value, "StylusTimes"); }
        //}


        private ObservableCollection<StrokeAction> strokeXList;

        public ObservableCollection<StrokeAction> StrokeXList
        {
            get { return strokeXList; }
            set { RaisePropertyChanged(ref strokeXList, value, "StrokeXList"); }
        }


        /// <summary>
        /// 记录每个板书页面所有被添加到这个页面的stroke和child，包括添加了又删除了的
        /// </summary>
        //private List<InkCanvasPage> addedStrokesAndChilds;
        /// <summary>
        /// 每次当前页面的index变化也就是CurrPageIndex变化就把变化后的值保存，好像没有用到它。。
        /// </summary>
        //private List<int> pageIndexList;
        /// <summary>
        /// 撤销与重做
        /// </summary>
        public List<CommandStack> cmdStackList;

        private DateTime lastTime;
        private SaveData saveData;
        private List<int> addedIndexList;
        private int imageIndex;
        private bool isNotWrite;
        public Guid guid = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1);
        /// <summary>
        /// 录制模块
        /// </summary>


        public DelegateCommand NewPageCommand { set; get; }
        public DelegateCommand DeletePageCommand { set; get; }
        public DelegateCommand NextPageCommand { set; get; }
        public DelegateCommand PreviousPageCommand { set; get; }
        public DelegateCommand SaveCommand { set; get; }
        public DelegateCommand ShowVideoCommand { set; get; }
        public DelegateCommand SaveBanshuCommand { set; get; }
        public DelegateCommand UndoCommand { set; get; }
        public DelegateCommand RedoCommand { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public CoursesViewModel()
        {
            IsRecord = false;
            InitPPT();         
            isNotWrite = false;
            allPage = new ObservableCollection<InkCanvasPage>();

            allCourse = new ObservableCollection<Course>();
            allCourse.CollectionChanged += AllCourse_CollectionChanged;
            currCourseIndex = 0;
            currCourse = new Course();

            cmdStackList = new List<CommandStack>();


            NewPageCommand = new DelegateCommand(new Action(newPage));
            DeletePageCommand = new DelegateCommand(new Action(deletePage));
            NextPageCommand = new DelegateCommand(new Action(nextPage));
            PreviousPageCommand = new DelegateCommand(new Action(previousPage));
            SaveCommand = new DelegateCommand(new Action(save));
            ShowVideoCommand = new DelegateCommand(new Action(ShowVideo));
            //SaveBanshuCommand = new DelegateCommand(new Action(SaveBanshu));

            saveData = new SaveData();
            StrokeXList = new ObservableCollection<StrokeAction>();
            StrokeXList.CollectionChanged += StrokeXList_CollectionChanged;
            addedIndexList = new List<int>();
            imageIndex = 0;

            initallPage();
        }

        private void AllCourse_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (AllCourse!=null)
            {
                SetAllCourseName();
            }            
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
               
            }
        }

        private void StrokeXList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (isRecord)
            {
                StrokeAction strokeAction = StrokeXList[strokeXList.Count - 1];
                if (strokeAction.type== "stroke")
                {
                    strokeAction.index = CurrPage.HistoryMaxCount[0]-1;
                }
                else if (strokeAction.type == "endStroke")
                {                    
                    for (int i = strokeXList.Count-1; i >=0; i--)
                    {
                        strokeXList[i].index = CurrPage.HistoryMaxCount[0] - 1;
                        if (strokeXList[i].type== "startStroke")
                        {
                            break;
                        }
                    }
                }

                saveData.actions.Add(StrokeXList[strokeXList.Count - 1]);
            }
        }

        private void ShowVideo()
        {
            ////if (!isColseRecorderWin)
            ////{
            //recorder.Activate();
            //recorder.Show();
            ////}
            ////else
            ////{
            ////    return;
            ////}
            //if (recorder.camera != -1)
            //{
            //    recorder.vid.Activate();
            //    recorder.vid.Show();
            //}
        }

        private void initallPage()
        {
            AllPage.Clear();
            AllPage.Add(new InkCanvasPage());
            //cmdStackList.Clear();
            //cmdStackList.Add(new CommandStack(allPage[0]));
            CurrPageIndex = 0;
            addedIndexList.Add(0);
            //CurrPage.Strokes.StrokesChanged += Strokes_StrokesChanged;
            //CurrPage.AllChild.CollectionChanged += AllChild_CollectionChanged;
            if (isRecord)
            {
                if (!isContainPageInSaveData(currCourse.Name + "_" + currPageIndex.ToString()))
                {
                    saveData.pages.Add(new Utils.Page()
                    {
                        name = currCourse.Name + "_" + currPageIndex.ToString(),
                        type = "whiteboard"
                    });
                }
                
            }
        }

        private void newPage()
        {
            if (!Public.Register.IsRegister&&CurrCourse.AllPage.Count>=2)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "请先激活该程序！");
                message.ShowDialog();
                return;
            }
            AllPage.Insert(currPageIndex + 1, new InkCanvasPage());
            cmdStackList.Insert(currPageIndex + 1, new CommandStack(allPage[currPageIndex + 1]));
            //  saveData.pages.Insert(currPageIndex + 1, new Utils.Page());
            addedIndexList.Insert(currPageIndex + 1, 0);
            CurrPageIndex += 1;
            CurrPage.Strokes.StrokesChanged += Strokes_StrokesChanged;
            CurrPage.AllChild.CollectionChanged += AllChild_CollectionChanged;
            if (isRecord)
            {
                saveData.pages.Add(new Utils.Page()
                {
                    name = currCourse.Name + "_" + currPageIndex.ToString(),
                    type = "whiteboard"
                });
            }
        }

        private void deletePage()
        {
            if (allPage.Count > 1)
            {
                AllPage.RemoveAt(currPageIndex);
                cmdStackList.RemoveAt(currPageIndex);
                CurrPageIndex = allPage.Count > currPageIndex ? currPageIndex : currPageIndex - 1;
                //record(CommandType.DeletePage);
            }

        }

        private void nextPage()
        {
            if (currPageIndex < allPage.Count - 1)
            {
                CurrPageIndex += 1;
                //record(CommandType.PageIndexChanged);
            }

        }

        private void previousPage()
        {
            if (currPageIndex > 0)
            {
                CurrPageIndex -= 1;
                //record(CommandType.PageIndexChanged);
            }
        }

        public void selectPage(int index)
        {
            CurrPageIndex = index;
            //record(CommandType.PageIndexChanged);
        }

        public void undo()
        {
            if (cmdStackList[CurrPageIndex].CanUndo)
            {
                isNotWrite = true;
                RecordMoveingOrResizing(cmdStackList[CurrPageIndex].Undo());                
            }
        }

        public void redo()
        {
            if (cmdStackList[CurrPageIndex].CanRedo)
            {
                isNotWrite = true;
                RecordMoveingOrResizing(cmdStackList[CurrPageIndex].Redo());                
            }
        }

        public void paste()
        {
            isNotWrite = true;
        }

        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (CurrCourse.IsNew == true && CurrCourse.IsBanshu)
            {
                CommandItem cItem = new StrokesAddedOrRemovedCI(cmdStackList[currPageIndex], e.Added, e.Removed);
                cmdStackList[currPageIndex].Enqueue(cItem);
            }            

            if (isRecord)
            {
                foreach (var item in e.Added)
                {
                    item.AddPropertyData(guid, CurrPage.HistoryMaxCount[0]++);                    
                }
                foreach (var item in e.Added)
                {
                    if (isNotWrite)
                    {
                        var opt = new Option();
                        var color = item.DrawingAttributes.Color;
                        opt.color = string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, color.ScA);                        
                        var dots = new List<Dot>(item.StylusPoints.Count);
                        if (item.DrawingAttributes.IsHighlighter)
                        {
                            opt.dotType = "square";
                            foreach (var point in item.StylusPoints)
                            {
                                dots.Add(new Dot() { x = point.X, y = point.Y, radius = item.DrawingAttributes.Height });
                            }
                        }
                        else
                        {
                            opt.dotType = "round";
                            foreach (var point in item.StylusPoints)
                            {
                                dots.Add(new Dot() { x = point.X, y = point.Y, radius = (item.DrawingAttributes.Height * point.PressureFactor) });
                            }
                        }
                        saveData.actions.Add(new StrokeAction()
                        {
                            time = (DateTime.Now - lastTime).TotalMilliseconds,
                            index = (Int32)item.GetPropertyData(guid),
                            type = "stroke",
                            duration = 0,
                            options = opt,
                            dots = dots
                        });
                    }
                    
                }
                if (e.Removed.Count > 0)
                {
                    var contents = new DeleteContent[e.Removed.Count];
                    int i = 0;
                    foreach (var item in e.Removed)
                    {
                        contents[i++] = new DeleteContent() { type = "stroke", index = item.GetPropertyData(guid).ToString() };
                    }
                    saveData.actions.Add(new DeleteAction() { time = (DateTime.Now - lastTime).TotalMilliseconds, type = "delete", contents = contents });
                }
            }

            isNotWrite = false;
            CurrCourse.IsModify = true;
        }

        private void AllChild_CollectionChanged(object sender, InkCanvasChildCollectionChangedEventArgs e)

        {
            if (CurrCourse.IsNew==true)
            {
                CommandItem cItem = new AllChildAddedOrRemovedCI(cmdStackList[currPageIndex], e.Added, e.Removed);
                cmdStackList[currPageIndex].Enqueue(cItem);
            }
            
            if (isRecord)
            {
                string courseName = NewCourseName();
                var picturesDictionary = "Resource/picture/";
                foreach (var item in e.Added)
                {
                    var image = item.UiEle as System.Windows.Controls.Image;
                    var imagePath = (image.Source as BitmapImage).UriSource.LocalPath;

                    item.index = CurrPage.HistoryMaxCount[0]++;
                    item.id= imageIndex;



                    if (saveData.pictures.Count == 0)
                    {
                        var picture = new Picture();
                        picture.id.Add(item.id.ToString());
                        picture.src = picturesDictionary + Path.GetFileName(imagePath);
                        saveData.pictures.Add(picture);
                    }
                    else
                    {
                        bool isContain = false;
                        for (int i = 0; i < saveData.pictures.Count; i++)
                        {
                            if (saveData.pictures[i].src == picturesDictionary + Path.GetFileName(imagePath))
                            {
                                isContain = true;
                                saveData.pictures[i].id.Add(item.id.ToString());
                                break;
                            }                            
                        }
                        if (!isContain)
                        {
                            var picture = new Picture();
                            picture.id.Add(item.id.ToString());
                            picture.src = picturesDictionary + Path.GetFileName(imagePath);
                            saveData.pictures.Add(picture);
                        }
                    }

                    saveData.actions.Add(new InsertPictureAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        index=item.index,
                        type = "insetPic",
                        id = item.id.ToString(),
                        pos = new Pos(item.X, item.Y),
                        width = image.Width,
                        height = image.Height
                    });


                    imageIndex++;
                }
                if (e.Removed.Count > 0)
                {
                    var contents = new DeleteContent[e.Removed.Count];
                    int i = 0;
                    foreach (var item in e.Removed)
                    {
                        contents[i++] = new DeleteContent() { type = "pic", index = item.index.ToString() };
                    }
                    saveData.actions.Add(new DeleteAction() { time = (DateTime.Now - lastTime).TotalMilliseconds, type = "delete", contents = contents });
                }
            }
            else
            {
                for (int i = 0; i < e.Added.Count; i++)
                {
                    e.Added[i].id = imageIndex;
                    imageIndex++;
                }
            }

            CurrCourse.IsModify = true;
        }

        public void AddCollectionChanged()
        {
            if (CurrCourse.IsNew==true)
            {
                cmdStackList.Clear();
                cmdStackList.Add(new CommandStack(allPage[0]));
            }            
            CurrPage.Strokes.StrokesChanged += Strokes_StrokesChanged;
            CurrPage.AllChild.CollectionChanged += AllChild_CollectionChanged;
        }

        /// <summary>
        /// 为所有旧课Page添加CollectionChanged事件，用于记录旧课中笔画的删除操作。
        /// </summary>
        /// <param name="oldCourse"></param>
        public void allPageAddCollectionChanged(Course oldCourse)
        {
            for (int i = 0; i < oldCourse.AllPage.Count; i++)
            {
                oldCourse.AllPage[i].Strokes.StrokesChanged += Strokes_StrokesChanged;
                oldCourse.AllPage[i].AllChild.CollectionChanged += AllChild_CollectionChanged;
            }
        }

        public void SelectionMovingOrResizing(IndicaterDragCompletedEventArgs e)
        {
            // Enforce stroke bounds to positive territory.
            Rect newRect = e.NewRectangle;
            Rect oldRect = e.OldRectangle;

            if (newRect.Top < 0d || newRect.Left < 0d)
            {
                Rect newRect2 =
                    new Rect(newRect.Left < 0d ? 0d : newRect.Left,
                                newRect.Top < 0d ? 0d : newRect.Top,
                                newRect.Width,
                                newRect.Height);

                newRect = newRect2;
            }

            if (CurrCourse.IsNew==true)
            {
                CommandItem cItem = new SelectionMovedOrResizedCI(cmdStackList[CurrPageIndex], e.SelectedStrokes, e.SelectedAllChild, newRect, oldRect);
                cmdStackList[CurrPageIndex].Enqueue(cItem);
            }            

            RecordMoveingOrResizing(e);
        }

        private void RecordMoveingOrResizing(IndicaterDragCompletedEventArgs e)
        {
            if (e == null)
            {
                return;
            }
            if (isRecord)
            {                
                Updataindicater(CourseCtrl.indicater.Margin.Left, CourseCtrl.indicater.Margin.Top,
                                    CourseCtrl.indicater.ActualWidth, CourseCtrl.indicater.ActualHeight);
                var scaleX = e.NewRectangle.Width / e.OldRectangle.Width;
                var scaleY = e.NewRectangle.Height / e.OldRectangle.Height;
                isNotWrite = false;

                var count = e.SelectedStrokes.Count + e.SelectedAllChild.Count;

                if (Math.Abs(scaleX - 1) > 0.02 || Math.Abs(scaleY - 1) > 0.02)
                {
                    int contentIndex = 0;
                    var contents = new ScaleTransfromContent[count];
                    foreach (var item in e.SelectedStrokes)
                    {
                        var scaleContent = new ScaleTransfromContent();
                        scaleContent.type = "stroke";
                        scaleContent.index = (Int32)item.GetPropertyData(guid);
                        scaleContent.scale = scaleX;
                        scaleContent.pos = new Pos(item.StylusPoints[0].X, item.StylusPoints[0].Y);
                        contents[contentIndex] = scaleContent;
                        contentIndex++;

                    }
                    foreach (var item in e.SelectedAllChild)
                    {
                        var scaleContent = new ScaleTransfromContent();
                        scaleContent.type = "pic";                        
                        scaleContent.index = item.index;
                        scaleContent.scale = scaleX;
                        scaleContent.pos = new Pos(item.X, item.Y);
                        contents[contentIndex] = scaleContent;
                        contentIndex++;
                    }
                    saveData.actions.Add(new ScaleAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "scale",
                        contents = contents
                    });
                }
                else
                {
                    int contentIndex = 0;
                    var contents = new MoveTransformContent[count];
                    foreach (var item in e.SelectedStrokes)
                    {
                        var moveContent = new MoveTransformContent();
                        moveContent.type = "stroke";
                        moveContent.index = (Int32)item.GetPropertyData(guid);
                        moveContent.pos = new Pos(item.StylusPoints[0].X, item.StylusPoints[0].Y);
                        contents[contentIndex] = moveContent;
                        contentIndex++;

                    }
                    foreach (var item in e.SelectedAllChild)
                    {
                        var moveContent = new MoveTransformContent();
                        moveContent.type = "pic";                        
                        moveContent.index = item.index;
                        moveContent.pos = new Pos(item.X, item.Y);
                        contents[contentIndex] = moveContent;
                        contentIndex++;
                    }
                    saveData.actions.Add(new MoveAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "move",
                        contents = contents
                    });
                }
            }
        }

        /// <summary>
        /// 用于暂停录制之后，对录制时间进行补偿
        /// </summary>
        /// <param name="timeSpan">暂停的时间间隔</param>
        public void SetLastTime(TimeSpan timeSpan)
        {
            DateTime time = lastTime.Add(timeSpan);
            lastTime = time;
        }

        public void InitStartRecord()
        {
            lastTime = DateTime.Now;
            IsRecord = true;
            //initallPage();
            CourseCtrl.indicater.updataIndicater(new Thickness(0, 0, 0, 0), 0, 0);
            beginAction();
        }

        private void beginAction()
        {
            //if (CourseCtrl.PPTMenu.IsChecked == true)
            //{
            //    saveData.actions.Add(new ChoosePPTAction
            //    {
            //        time = (DateTime.Now - lastTime).TotalMilliseconds,
            //        type = "choosePPT",
            //        name = currPPTName,
            //        page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
            //    });                
            //}
            //else
            //{
            //    saveData.actions.Add(new ChoosePageAction
            //    {
            //        time = (DateTime.Now - lastTime).TotalMilliseconds,
            //        type = "choosePage",
            //        index = currCourse.Name + "_" + currPageIndex.ToString()
            //    });
            //}

            if (!isContainPageInSaveData(currCourse.Name + "_0"))
            {
                saveData.pages.Add(new Utils.Page() { name = currCourse.Name + "_0", type = "whiteboard" });
            }
        }

        public void save()
        {
            CourseCtrl.GotoBanshu();
            BanshuMenu();            
            saveAscii();
            saveCourse();

            CourseCtrl.courseMenuTool.OpenNewCourseBtn.IsEnabled = true;
            IsRecord = false;
            cmdStackList = null;
            cmdStackList = new List<CommandStack>();
            saveData = null;        
            saveData = new SaveData();

            CurrCourse.IsNew = false;
            MessageBox.Show("保存完毕 \n路径为" + CourseControl.CourseSavePath);

            //string currCoursePath = currCourse.CoursePath + currCourse.Name;
            //string httpServerRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\善而教\\PlayBack\\src\\course\\" + CurrCourse.Name;
            //CopyCourseDirectory(currCoursePath, httpServerRoot);
        }

        public bool creatReName()
        {
            if (CurrCourse.Name == "未命名")
            {
                ReName reName = new ReName(CurrCourse);
                reName.Activate();
                reName.Top = 80;
                reName.Left = (CourseCtrl.ActualWidth - 400) / 2;
                reName.ShowDialog();
                if (reName.isSave)
                {
                    //CourseCtrl.savePath = CurrCourse.CoursePath;
                    //CourseCtrl.filename = CurrCourse.Name;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            
        }

        public void saveCourse()
        {
            string SaveBanshuPath = currCourse.CoursePath + CurrCourse.Name + "\\" + CurrCourse.Name + ".course";
            if (!Directory.Exists(currCourse.CoursePath + CurrCourse.Name))
            {
                Directory.CreateDirectory(currCourse.CoursePath + CurrCourse.Name);
                Public.Log.FileOrFolder(Public.LogType.CreatFolder, currCourse.CoursePath + CurrCourse.Name, CurrCourse.Name, CurrCourse.CoursePath);
            }
            FileStream fs = File.OpenWrite(SaveBanshuPath);
            var bw = new BinaryWriter(fs, Encoding.UTF8);
            try
            {
                bw.Write(allPage.Count);
                foreach (var item in allPage)
                {
                    item.Strokes.Save(fs);
                    item.AllChild.Save(bw);
                }


                bw.Write((Int32)allPPT.Count);
                foreach (var item in pptNameList)
                {
                    bw.Write(item);
                }
                foreach (var item in allPPT)
                {
                    bw.Write(item.Value.strokesList.Count);
                    bw.Write(item.Key);
                    foreach (var stroke in item.Value.strokesList)
                    {
                        stroke.Save(fs);
                    }
                }                

            }

            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message);
                Public.Log.WriterLog(e.Message, CurrCourse.Name, CurrCourse.CoursePath);
            }
            bw.Flush();
            bw.Close();
            fs.Close();

            if (CurrCourse.IsNew)
            {
                CurrCourse.IsSaved = true;
                CourseCtrl.courseMenuTool.OpenNewCourseBtn.IsEnabled = true;
            }
            CurrCourse.IsModify = false;
        }
       

        private void saveAscii()
        {
            saveData.size.h = courseCtrl.myInkCanvas.ActualHeight;
            saveData.size.w = courseCtrl.myInkCanvas.ActualWidth;
            saveData.background.type = "radial";
            var background = courseCtrl.myInkCanvas.Background as RadialGradientBrush;
            var centerX = background.Center.X * saveData.size.w;
            var centerY = background.Center.Y * saveData.size.h;
            saveData.background.start = new Utils.GradientCircle() { x = centerX, y = centerY, radiusX = 0, radiusY = 0 };
            saveData.background.end = new Utils.GradientCircle()
            {
                x = centerX,
                y = centerY,
                radiusX = background.GradientStops[1].Offset * saveData.size.w / 2,
                radiusY = background.GradientStops[1].Offset * saveData.size.h / 2
            };
            var colors = new GradientColor[2];
            var color0 = background.GradientStops[0].Color;
            var color1 = background.GradientStops[1].Color;
            colors[0] = new GradientColor() { pos = 0, value = string.Format("rgba({0},{1},{2},{3})", color0.R, color0.G, color0.B, color0.ScA) };
            colors[1] = new Utils.GradientColor() { pos = 1, value = string.Format("rgba({0},{1},{2},{3})", color1.R, color1.G, color1.B, color1.ScA) };
            saveData.background.colors = colors;

            saveData.audioTimeLeng = new AudioTimeLength { timeLength = (DateTime.Now - lastTime).TotalMilliseconds };
            DeleteNotStrokedPage();

            try
            {
                string SavePath = currCourse.CoursePath + CurrCourse.Name + "\\play\\" + CurrCourse.Name + ".json";
                if (!Directory.Exists(currCourse.CoursePath + CurrCourse.Name +"\\play"))
                {
                    Directory.CreateDirectory(currCourse.CoursePath + CurrCourse.Name + "\\play");                    
                }
                var fs = File.OpenWrite(SavePath);
                var sw = new StreamWriter(fs);

                //var serializer = new DataContractJsonSerializer(typeof(SaveData));
                //serializer.WriteObject(fs, saveData);

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 52428800;
                var jsonString = js.Serialize(saveData);
                sw.Write(jsonString);

                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog(ex.Message + "保存笔画录制文件.course时发生错误!", CurrCourse.Name, CurrCourse.CoursePath);
                MessageWin message = new MessageWin(MessageWinType.Prompt, "保存笔画录制文件.course时发生错误!");
                message.ShowDialog();                
            }


           // ------------------------用于本地演示，最终发布时记得去掉-------------------------------- -
            try
            {

                if (CurrCourse.Name!=null&&CurrCourse.Name!="未命名")
                {
                    string indexPath1 = currCourse.CoursePath + CurrCourse.Name + "\\index.html";
            
                    //index.html-> string1
                    #region 
                    string string1 = "<!DOCTYPE html>\n" +
                                     "<html lang=\"en\">\n" +
                                     "<head>\n" +
                                     "    <meta charset=\"UTF-8\">\n" +
                                     "    <title>白板动画</title>\n" +
                                     "    <!--build:css style/common.css  -->\n" +
                                     "    <link rel=\"stylesheet\" href=\"/player/style/common.min.css\">\n" +
                                     "    <!--endbuild  -->\n" +
                                     "</head>\n" +
                                     "<body>\n" +
                                     "    <div id=\"whiteboard\"></div>\n" +
                                     "    <!--build:js script/common.js  -->\n" +
                                     "    <script src=\"/player/script/common.min.js\"></script>\n" +
                                     "    <!--endbuild  -->\n" +
                                     "\n" +
                                     "    <script>\n" +
                                     "        var player = new Player('" + "play/" + CurrCourse.Name + ".json', '" + "play/" + CurrCourse.Name + ".mp3', document.querySelector('#whiteboard'))\n" +
                                     "    </script>\n" +
                                     "</body>\n" +
                                     "</html>\n";
                    #endregion

                    File.WriteAllText(indexPath1, string1);
                }                                              
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog(ex.Message, CurrCourse.Name, CurrCourse.CoursePath);
            }            

        }


        //public void CopyCourseDirectory(string currCoursePath, string httpServerRoot)
        //{
        //    if (!Directory.Exists(httpServerRoot + "\\play"))
        //    {
        //        Directory.CreateDirectory(httpServerRoot + "\\play");
        //        try
        //        {
        //            Public.HelperMethods.CopyFolder(currCoursePath + "\\play", httpServerRoot + "\\play");
        //            Public.Log.WriterLog("拷贝play课程文件   " +"源文件 = "+ currCoursePath + "\\play"+";目标文件 = "+ httpServerRoot + "\\play",
        //                CurrCourse.Name, CurrCourse.CoursePath);
        //        }
        //        catch (Exception)
        //        {
        //            MessageWin message = new MessageWin(MessageWinType.Prompt, "拷贝play课程文件时发生错误！");
        //            message.ShowDialog();
        //            Public.Log.WriterLog("拷贝play课程文件时发生错误", CurrCourse.Name, CurrCourse.CoursePath);
        //        }
        //    }

        //    if (!Directory.Exists(httpServerRoot + "\\Resource"))
        //    {
        //        Directory.CreateDirectory(httpServerRoot + "\\Resource");
        //        try
        //        {
        //            Public.HelperMethods.CopyFolder(currCoursePath + "\\Resource", httpServerRoot + "\\Resource");
        //            Public.Log.WriterLog("拷贝Resource课程文件   " + "源文件 = " + currCoursePath + "\\Resource" + ";目标文件 = " + httpServerRoot + "\\Resource",
        //                CurrCourse.Name, CurrCourse.CoursePath);
        //        }
        //        catch (Exception)
        //        {
        //            MessageWin message = new MessageWin(MessageWinType.Prompt, "拷贝Resource课程文件时发生错误！");
        //            message.ShowDialog();
        //            Public.Log.WriterLog("拷贝Resource课程文件时发生错误", CurrCourse.Name, CurrCourse.CoursePath);
        //        }
        //    }
        //}


        /// <summary>
        /// 清除当前新课板书
        /// </summary>
        public void CleraBanshu()
        {
            //注意这里是因为只有在新课时才能录制，所以未做course的判断。
            AllPage.Clear();
            AllPage.Add(new InkCanvasPage());            
            CurrPageIndex = 0;
            CurrPage.Strokes.StrokesChanged += Strokes_StrokesChanged;
            CurrPage.AllChild.CollectionChanged += AllChild_CollectionChanged;

        }

        public void CloseAllOldCourse()
        {
            for (int i = AllCourse.Count-1; i >=0; i--)
            {
                if (AllCourse[i].IsNew==false)
                {
                    AllCourse.RemoveAt(i);
                }
            }
        }

        public void Dispose()
        {
            foreach (var item in allPPT)
            {
                item.Value.Dispose();
            }
        }
    }


    public partial class CoursesViewModel
    {
        #region xxx     

        private ObservableCollection<FastSwitchListItem> fastSwitchList;

        public ObservableCollection<FastSwitchListItem> FastSwitchList
        {
            get { return fastSwitchList; }
            set { fastSwitchList = value; }
        }


        //private ObservableCollection<string> allCourseName;
        //public ObservableCollection<string> AllCourseName
        //{
        //    get { return allCourseName; }
        //    set
        //    {
        //        RaisePropertyChanged(ref allCourseName, value, "AllCourseName");                
        //    }
        //}

        public void SetAllCourseName()
        {
            FastSwitchList.Clear();
            foreach (var item in AllCourse)
            {
                FastSwitchList.Add(new FastSwitchListItem
                {
                    Name = item.Name,
                    PPTNameList = item.PPTNameList
                });
            }

            CourseCtrl.GotoBanshu();
            BanshuMenu();

            //foreach (var item in AllCourse)
            //{
            //    if (item.IsNew == true)
            //    {
            //        if (CourseCtrl.isBanshu)
            //        {
            //            CourseCtrl.GotoBanshu();
            //            BanshuMenu();                        
            //        }
            //        else
            //        {
            //            CourseCtrl.GotoPPT();
            //            PPTMenu();
            //        }                    
            //    }
            //}

            //AllCourseName.Clear();
            //foreach (var item in AllCourse)
            //{
            //    AllCourseName.Add(item.Name);                
            //}            
        }
           
        private ObservableCollection<Course> allCourse;
        public ObservableCollection<Course> AllCourse
        {
            get { return allCourse; }
            set
            {
                RaisePropertyChanged(ref allCourse, value, "AllCourse");                
            }
        }

        private Course currCourse;
        public Course CurrCourse
        {
            get { return currCourse; }
            set
            {
                RaisePropertyChanged(ref currCourse, value, "CurrCourse");

                allPPT = CurrCourse.AllPPT;
                PptNameList = CurrCourse.PPTNameList;
                CurrPptIndex = CurrCourse.CurrPPTIndex;

                AllPage = CurrCourse.AllPage;
                CurrPageIndex = CurrCourse.CurrPageIndex;

                CourseControl.CourseSavePath = value.CoursePath + value.Name + "\\";                          
            }
        }

        private int currCourseIndex;
        public int CurrCourseIndex
        {
            get { return currCourseIndex; }
            set
            {
                RaisePropertyChanged(ref currCourseIndex, value, "CurrCourseIndex");
                if (CurrCourseIndex == -1)
                {
                    return;
                }
                CurrCourse = AllCourse[CurrCourseIndex];
                CourseCtrl.InitIsEnabled();

                if (CurrCourse.AllPPT.Count == 0)
                {
                    CourseCtrl.GotoBanshu();
                    BanshuMenu();
                }
                else
                {
                    if (CurrCourse.IsBanshu)
                    {
                        CourseCtrl.GotoBanshu();
                        BanshuMenu();
                    }
                    else
                    {
                        CourseCtrl.GotoPPT();
                        PPTMenu();
                    }
                }
            }
        }

        private CustomInkCanvas customInkCan;
        public CustomInkCanvas CustomInkCan
        {
            get { return customInkCan; }
            set { RaisePropertyChanged(ref customInkCan, value, "CustomInk"); }
        }

        private CourseControl courseCtrl;
        public CourseControl CourseCtrl
        {
            get { return courseCtrl; }
            set { RaisePropertyChanged(ref courseCtrl, value, "CourseCtrl"); }
        }



        #endregion

        private ObservableCollection<string> pptNameList;//PPT模块中正处于打开时的PPT

        public ObservableCollection<string> PptNameList
        {
            get { return pptNameList; }
            set { RaisePropertyChanged(ref pptNameList, value, "PptNameList"); }
        }

        private PPTModel currPPT;

        public PPTModel CurrPPT
        {
            get { return currPPT; }
            set { RaisePropertyChanged(ref currPPT, value, "CurrPPT"); }
        }

        private int currPptIndex;

        public int CurrPptIndex
        {
            get { return currPptIndex; }
            set
            {
                if (value < 0|| pptNameList.Count == 0||allPPT.Count==0)
                {
                    return;
                }

                //if ((currPPTName != pptNameList[value])||currPPTName==null)
                //{
                currPPTName = pptNameList[value];
                CurrPPT = allPPT[currPPTName];
                CurrCourse.CurrPPTIndex = value;
                CurrPage = new InkCanvasPage() { Strokes = currPPT.GetCurrStrokes(), HistoryMaxCount = currPPT.GetCurrPageHistoryMaxCount() };
                if (IsRecord)
                {
                    saveData.actions.Add(new ChoosePPTAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "choosePPT",
                        name = currPPTName,
                        page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
                    });

                    int Slide = CurrPPT.CurrSlide;
                    int Anim = CurrPPT.CurrAdim;
                    if ( !(Slide == 0 && Anim == 0) && CurrPPT.isNewOpen == false)
                    {
                        saveData.actions.Add(new PPTActionAction()
                        {
                            time = (DateTime.Now - lastTime).TotalMilliseconds,
                            type = "PPTJump",
                            slide = Slide,
                            anim = Anim,
                            page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
                        });
                        CurrPPT.isNewOpen = true;
                        CurrPPT.JumpSlideAndAnim();
                    }
                }


                RaisePropertyChanged(ref currPptIndex, value, "CurrPptIndex");
            }
        }


        //private List<string> addedFileNameList; //用于回放，所有打开过的PPT，PPT不重复添加
        //private List<string> nameList;  //用于回放，每当PPT打开或者切换，就将切换后的PPT名字存入其中
        //private List<int> playCurrSlideIndexList; //记录每次放映从第几页开始
        private Dictionary<string, PPTModel> allPPT;
        //private bool PPTSkip; //为了跳过放映按钮所激发的SlideShowNextClick事件
        //private List<List<PPTPage>> slideList;

        private int slideCount;
        private string currPPTName;
        public bool isIntoPPT;
        public bool isOpenHtml;//用于跳过在双击.zh文件时出发onMouseMove事件的问题。
        private string currChoosePage;
        private int CurrPPTSlideAnim;
        /// <summary>
        /// 判断是否存在PPT
        /// </summary>
        public bool isOpenPPT;
        public DelegateCommand BanShuMenuCommand { set; get; }
        public DelegateCommand CommonViewCommand { set; get; }
        public DelegateCommand BrowseViewCommand { set; get; }
        public DelegateCommand PlayViewCommand { set; get; }
        public DelegateCommand PPTMenuCommand { set; get; }
        public DelegateCommand ClosePPTCommand { set; get; }
        public DelegateCommand NextPPTCommand { set; get; }
        public DelegateCommand PlayCurrSlideCommand { set; get; }
        public DelegateCommand PreviousPPTCommand { set; get; }

        private void InitPPT()
        {
            PptNameList = new ObservableCollection<string>();
            fastSwitchList = new ObservableCollection<FastSwitchListItem>();
            allPPT = new Dictionary<string, PPTModel>();
            isIntoPPT = false;
            isOpenPPT = false;
            isOpenHtml = false;
            CurrPPTSlideAnim = -1;

            BanShuMenuCommand = new DelegateCommand(new Action(BanshuMenu));
            CommonViewCommand = new DelegateCommand(new Action(CommonView));
            BrowseViewCommand = new DelegateCommand(new Action(BrowseView));
            PPTMenuCommand = new DelegateCommand(new Action(PPTMenu));
            ClosePPTCommand = new DelegateCommand(new Action(ClosePPT));
            NextPPTCommand = new DelegateCommand(new Action(NextPPT));
            PlayCurrSlideCommand = new DelegateCommand(new Action(PlayCurrSlide));
            PreviousPPTCommand = new DelegateCommand(new Action(PreviousPPT));
        }

        private void PlayCurrSlide()
        {
            //PPTSkip = true;
            //CurrCourse.CoursePPT.PlayCurrSlide();
            //var page = new InkCanvasPage();
            //page.AllChild = new InkCanvasChildCollection();
            //page.Strokes = pptStrokesList[currPPTName][CurrSlideIndex];
            //CurrPage = page;

            //record(CommandType.PlayView);
            //CurrView = "PPTPlaying";
        }

        private void NextPPT()
        {
            if (CurrPPT == null)
            {
                return;
            }

            if (!Public.Register.IsRegister&&CurrPPT.CurrSlide>=3)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "请先激活该程序！");
                message.ShowDialog();
                return;
            }

            int Slide=-1;
            int Anim = 0;
            int ClickCount = -1;
            bool IsNextSlide = false;

            CurrPPT.Next(ref Slide,ref Anim,ref ClickCount, ref IsNextSlide);

            if (IsNextSlide)
            {
                CurrPage = new InkCanvasPage() { Strokes = currPPT.GetCurrStrokes(), HistoryMaxCount = currPPT.GetCurrPageHistoryMaxCount() };
            }
            if (isRecord)
            {
                if (IsNextSlide)
                {
                    var ac = saveData.actions[saveData.actions.Count - 1];
                    if (ac.type == "pptNext"&&ClickCount!=-1)
                    {                        
                        saveData.actions.Add(new PPTActionAction()
                        {
                            time = ac.time,
                            type = "PPTJump",
                            slide = Slide-1,
                            anim = ClickCount,
                            page = CurrCourse.Name + "_" + currPPTName + "_" + (CurrPPT.CurrSlideIndex-1).ToString()
                        });
                        saveData.actions.Remove(ac);
                        ClickCount = -1;
                    }                    
                    saveData.actions.Add(new PPTActionAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "PPTJump",
                        slide = Slide,
                        anim = 0,
                        page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
                    });
                    CurrPPTSlideAnim = -1;
                }
                else
                {
                    if (CurrPPTSlideAnim!= Anim && Anim != -1)
                    {
                        CurrPPTSlideAnim = Anim;
                        saveData.actions.Add(new Actions
                        {
                            time = (DateTime.Now - lastTime).TotalMilliseconds,
                            type = "pptNext"
                        });
                    }                    
                }
                                    
            }

        }

        private void PreviousPPT()
        {
            if (CurrPPT == null)
            {
                return;
            }

            int Slide = -1;
            int Anim = 0;
            bool IsPrevSlide = false;
            CurrPPT.Previous(ref Slide,ref Anim, ref IsPrevSlide);

            if (IsPrevSlide)
            {
                CurrPage = new InkCanvasPage() { Strokes = currPPT.GetCurrStrokes(), HistoryMaxCount = currPPT.GetCurrPageHistoryMaxCount() };
            };

            if (isRecord)
            {
                if (IsPrevSlide)
                {
                    saveData.actions.Add(new PPTActionAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "PPTGoTo",
                        slide = Slide,
                        page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
                    });
                    CurrPPTSlideAnim = -1;
                }
                else
                {
                    if (CurrPPTSlideAnim!=Anim)
                    {
                        CurrPPTSlideAnim = Anim;
                        saveData.actions.Add(new Actions
                        {
                            time = (DateTime.Now - lastTime).TotalMilliseconds,
                            type = "pptPrev"
                        });
                    }                    
                }
            }            
        }

        //private void RecordPPTSlideChanged(StrokeCollection lastSlideStrokes)
        //{
        //    if (isRecord)
        //    {
        //        saveData.actions.Add(new ChoosePageAction()
        //        {
        //            time = (DateTime.Now - lastTime).TotalMilliseconds,
        //            type = "choosePage",
        //            index = currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
        //        });
        //        currChoosePage = currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString();
        //    }
        //}

        //private PPTActionAction GetLastPPTAction()
        //{
        //    for (int i = saveData.actions.Count-1; i>=0; i--)
        //    {
        //        if ((saveData.actions[i] as Actions).type=="PPTAction")
        //        {
        //            return (saveData.actions[i] as PPTActionAction);
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// 用于保存数据时对数据做一些处理，主要是去除或者修改一些没有stroke的Page。
        /// </summary>
        /// <param name="name"></param>
        private void DeleteNotStrokedPage()
        {
            for (int s = 0; s < AllCourse.Count; s++)
            {
                for (int y = 0; y < AllCourse[s].PPTNameList.Count; y++)
                {
                    var name = AllCourse[s].PPTNameList[y];
                    var pptModel = AllCourse[s].AllPPT[name];

                    for (int i = 0; i < pptModel.SlideCount; i++)
                    {
                        if (pptModel.strokeIndexList[i][0] <= 0)
                        {
                            var index = AllCourse[s].Name + "_" + name + "_" + i.ToString();
                            for (int j = saveData.pages.Count - 1; j >= 0; j--)
                            {
                                if (saveData.pages[j].name == index)
                                {
                                    saveData.pages.RemoveAt(j);
                                }
                            }

                            for (int k = 0; k < saveData.actions.Count; k++)
                            {
                                var action = saveData.actions[k] as Actions;
                                if (action.type == "PPTJump" || action.type == "PPTGoTo")
                                {
                                    PPTActionAction pptAc = (saveData.actions[k] as PPTActionAction);
                                    if (!isContainStroke(pptAc.page))
                                    {
                                        (saveData.actions[k] as PPTActionAction).page = null;
                                    }
                                }
                            }
                        }

                        //if (pptModel.strokesList[i].Count == 0)
                        //{
                           

                        //    //for (int j = saveData.actions.Count - 1; j >= 0; j--)
                        //    //{
                        //    //    var action = saveData.actions[j] as ChoosePageAction;
                        //    //    if (action != null && action.index == index)
                        //    //    {
                        //    //        action.index = null;
                        //    //    }
                        //    //}
                        //}
                    }
                }                
            }                        
        }

        private void ClosePPT()
        {
            if (CurrPptIndex > 0)
            {
                var name = pptNameList[currPptIndex];
                CurrPptIndex--;                
                allPPT.Remove(name);
                pptNameList.Remove(name);
                DeleteNotStrokedPage();
            }
            else
            {
                if (allPPT.Count>1&&CurrPptIndex==0)
                {
                    var name = pptNameList[0];
                    allPPT.Remove(name);
                    pptNameList.Remove(name);
                    CurrPptIndex = 0;
                }
                else if (allPPT.Count==1&&CurrCourseIndex==0)
                {
                    allPPT.Clear();
                 //   allPPT = null;
                    pptNameList.Clear();
                  //  PptNameList = null;
                    CurrPPT = null;
                    CurrPptIndex = -1;
                    //CourseCtrl.GotoBanshu();
                    //BanshuMenu();
                }
                //if (allPPT.Count >= 1)
                //{
                //    var name = pptNameList[currPptIndex];
                //    CurrPptIndex=-1;                    
                //    allPPT.Remove(name);
                //    pptNameList.Remove(name);
                //}
            }
        }

        public void BanshuMenu()
        {
            //CourseCtrl.GotoBanshu();
            CurrPPT = null;
            CurrPage = allPage[CurrPageIndex];
            isIntoPPT = false;
            if (isRecord)
            {
                if (currCourse.Name + "_" + currPageIndex.ToString() == currChoosePage)
                {
                    return;
                }
                saveData.actions.Add(new ChoosePageAction()
                {
                    time = (DateTime.Now - lastTime).TotalMilliseconds,
                    type = "choosePage",
                    index = currCourse.Name + "_" + currPageIndex.ToString()
                });
                currChoosePage = currCourse.Name + "_" + currPageIndex.ToString();
            }
        }

        public void PPTMenu()
        {
            if (allPPT.Count > 0)
            {
                CurrPPT = allPPT[pptNameList[currPptIndex]];

                if (isRecord)
                {
                    if (currChoosePage == currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString())
                    {
                        return;
                    }
                    saveData.actions.Add(new ChoosePPTAction()
                    {
                        time = (DateTime.Now - lastTime).TotalMilliseconds,
                        type = "choosePPT",
                        name = currPPTName,
                        page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
                    });


                    int Slide = CurrPPT.CurrSlide;
                    int Anim = CurrPPT.CurrAdim;
                    if ( !(Slide == 0 && Anim == 0) && CurrPPT.isNewOpen == false)
                    {
                        saveData.actions.Add(new PPTActionAction()
                        {
                            time = (DateTime.Now - lastTime).TotalMilliseconds,
                            type = "PPTJump",
                            slide = Slide,
                            anim = Anim,
                            page = CurrCourse.Name + "_" + currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString()
                        });
                        CurrPPT.isNewOpen = true;
                        CurrPPT.JumpSlideAndAnim();
                    }
                   
                    currChoosePage = currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString();

                }

            }

            CourseCtrl.GotoPPT();

            if (CurrPPT != null)
            {
                CurrPage = new InkCanvasPage() { Strokes = currPPT.GetCurrStrokes(), HistoryMaxCount = currPPT.GetCurrPageHistoryMaxCount() };
            }               
            else
            {
                CurrPage = new InkCanvasPage();                
            }                            
            isIntoPPT = true;
        }

        private void CommonView()
        {
            //CurrCourse.CoursePPT.CommonView();

            //record(CommandType.CommonView);
        }

        private void BrowseView()
        {
            //CurrCourse.CoursePPT.BrowseView();

            //record(CommandType.BrowseView);
        }

        public void showhtml(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (allPPT.ContainsKey(name)&&CurrCourse.IsNew==true)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "当前PPT已打开！");
                message.ShowDialog();
                Public.Log.WriterLog("当前PPT已打开！", CurrCourse.Name, CurrCourse.CoursePath);
                return;
            }
            var webControl = new WebControl();
            string Path2 = Path.GetDirectoryName(path) + "\\" + name.Replace(" ", "");
            using (var sr = new StreamReader(Path.ChangeExtension(Path2, "txt")))
            {
                slideCount = int.Parse(sr.ReadLine());
            }
            webControl.Source = new Uri(path);

            var ppt = new PPTModel(webControl, slideCount);
            if (currCourse.IsNew==false)
            {
                ppt.strokesList = CourseCtrl.temporaryOldCoursePPTStrokes[name];
            }
            for (int i = 0; i < ppt.strokesList.Count; i++)
            {
                ppt.strokesList[i].StrokesChanged += Strokes_StrokesChanged;
            }

            allPPT.Add(name, ppt);
            //addedFileNameList.Add(name);
            if (CurrCourse.IsNew)
            {
                PptNameList.Add(name);
            }            
            //nameList.Add(Path.GetFileNameWithoutExtension(path));
            CurrPptIndex = pptNameList.IndexOf(name);//这里会记录一个changeppt

            if (isRecord)
            {
                if (!isContainPPTS(name))
                {
                    string courseName = NewCourseName();
                    saveData.PPTS.Add(new Utils.PPT() { name = name, src = "Resource/ppt/" + name + "/" + name + ".html" });
                }                
                for (int i = 0; i < slideCount; i++)
                {
                    saveData.pages.Add(new Utils.Page() { name =CurrCourse.Name+ "_" + name + "_" + i.ToString(), type = "ppt" });
                }
                //saveData.actions.Add(new ChoosePageAction()
                //{
                //    time = (DateTime.Now - lastTime).TotalMilliseconds,
                //    type = "choosePage",
                //    index = currName + "_" + CurrPPT.CurrSlideIndex.ToString()
                //});
                currChoosePage = currPPTName + "_" + CurrPPT.CurrSlideIndex.ToString();
                isOpenHtml = true;

                //saveData.actions.Add(new ChoosePPTAction() { time = (DateTime.Now - lastTime).TotalMilliseconds, type = "choosePPT", name = name });
            }

            CourseCtrl.myInkCanvas.Background = Brushes.Transparent;

            CurrCourse.IsModify = true;
        }

        public double indicaterMoveDuration = 0;

        public void Updataindicater(double X, double Y, double Width, double Height)
        {
            if (IsRecord)
            {
                saveData.actions.Add(new UpdataIndicater()
                {
                    time = (DateTime.Now - lastTime).TotalMilliseconds - indicaterMoveDuration,
                    type = "updateIndicator",
                    duration = indicaterMoveDuration,
                    x = X,
                    y = Y,
                    width = Width,
                    height = Height
                });
                indicaterMoveDuration = 0;
            }
        }

        /// <summary>
        /// 开始录制后，保存旧课中的数据，用于回放
        /// </summary>
        public void SaveAddedOldCourse(Course saveCourse)
        {           
            var name = saveCourse.Name;
            var allPage1 = saveCourse.AllPage;
            var allPPT = saveCourse.AllPPT;
            for (int i = 0; i < allPage1.Count; i++)
            {
                var oldPage = new OldPage();
                var courseName = name + "_" + i.ToString();
                if (!isContainPageInSaveData(courseName))
                {
                    saveData.pages.Add(new Utils.Page() { name = courseName, type = "whiteboard" });
                }               
                oldPage.index = courseName;
                var contents = new Content[allPage1[i].Strokes.Count + allPage1[i].AllChild.Count];
                var contentIndex = 0;               
                int MaxIndex = 0;//用于给笔画进行初始编号
                foreach (var item in allPage1[i].Strokes)
                {
                    item.AddPropertyData(guid, MaxIndex++);   
                    contents[contentIndex] = SaveOldCourseStroke(item); 
                    contentIndex++;
                }
                foreach (var item in allPage1[i].AllChild)
                {
                    item.index = MaxIndex++;
                    item.id = imageIndex;

                    string pictrueName = Path.GetFileName((item.UiEle as Image).Source.ToString());
                    var content = new PictrueContent()
                    {
                        type = "pic",
                        index=item.index,
                        pos = new Pos(item.X, item.Y),
                        id = item.id.ToString(),
                        width = item.UiEle.Width,
                        height = item.UiEle.Height
                    };

                    //---------------现在是依靠录制必定有新课来保证一定存在，未来可能会出现某些问题。
                    string path1 = "";
                    foreach (var course in allCourse)
                    {
                        if (course.IsNew == true)
                        {
                            path1 = course.CoursePath + course.Name;
                            //这里出现了没有新课导致path1=""的情况，待修复
                        }
                    }
                    //---------------------
                    //var sourceFile = ((item.UiEle as Image).Source as BitmapImage).UriSource.LocalPath;
                    //var destinationFile = path1 + "\\Resource\\picture\\" + pictrueName;
                    //if (!Directory.Exists(path1 + "\\Resource\\picture\\"))
                    //{
                    //    Directory.CreateDirectory(path1 + "\\Resource\\picture\\");
                    //    Public.Log.FileOrFolder(Public.LogType.CreatFolder, path1 + "\\Resource\\picture\\");
                    //}
                    //System.IO.File.Copy(sourceFile, destinationFile, true);
                    //Public.Log.FileOrFolder(Public.LogType.CopeFile, sourceFile + "  to  " + destinationFile, CurrCourse.Name, CurrCourse.CoursePath);

                    string courseName1 = NewCourseName();
                    var picturesDictionary = "Resource/picture/";
                    if (saveData.pictures.Count == 0)
                    {
                        var picture = new Picture();
                        picture.id.Add(item.id.ToString());
                        picture.src = picturesDictionary + Path.GetFileName(pictrueName);
                        saveData.pictures.Add(picture);
                    }
                    else
                    {
                        bool isContain = false;
                        for (int j = 0; j < saveData.pictures.Count; j++)
                        {
                            if (saveData.pictures[j].src == picturesDictionary + Path.GetFileName(pictrueName))
                            {
                                isContain = true;
                                saveData.pictures[j].id.Add(item.id.ToString());
                                break;
                            }
                        }
                        if (!isContain)
                        {
                            var picture = new Picture();
                            picture.id.Add(item.id.ToString());
                            picture.src = picturesDictionary + Path.GetFileName(pictrueName);
                            saveData.pictures.Add(picture);
                        }
                    }
                    
                    imageIndex++;
                    contents[contentIndex] = content;
                    contentIndex++;
                }

                allPage1[i].HistoryMaxCount[0] = MaxIndex;
                oldPage.contents = contents;
                if (!saveData.oldPages.Contains(oldPage))
                {
                    saveData.oldPages.Add(oldPage);
                }                
            }

            //ppt中的笔画------------------------------------------
            if (saveCourse.AllPPT != null&& saveCourse.AllPPT.Count>0)
            {
                foreach (var PPT in saveCourse.AllPPT)
                {
                    var pptName = PPT.Key;
                    if (!isContainPPTS(pptName))
                    {
                        string courseName = NewCourseName();
                        saveData.PPTS.Add(new Utils.PPT() { name = pptName, src = "Resource/ppt/" + pptName + "/" + pptName + ".html" });
                    }

                    for (int i = 0; i < PPT.Value.strokesList.Count; i++)
                    {
                        var pptPageName = saveCourse.Name + "_" + pptName + "_" + i.ToString();
                        if (!isContainPageInSaveData(pptPageName))
                        {
                            saveData.pages.Add(new Utils.Page() { name = pptPageName, type = "ppt" });
                        }

                        if (PPT.Value.strokesList[i].Count == 0)
                        {
                            continue;
                        }


                        int MaxIndex = 0;//用于旧课中笔画的计数
                        foreach (var item in PPT.Value.strokesList[i])
                        {
                            item.AddPropertyData(guid, MaxIndex++);                            
                        }
                        PPT.Value.strokeIndexList[i][0] = MaxIndex;


                        var oldPage = new OldPage();                        
                        oldPage.index = pptPageName;
                        var contents = new List<Content>();

                        foreach (var item in PPT.Value.strokesList[i])
                        {
                            contents.Add(SaveOldCourseStroke(item));
                        }

                        Content[] PPTContent=new Content[contents.Count];
                        for (int h = 0; h < contents.Count; h++)
                        {
                            PPTContent[h] = contents[h];
                        }
                        oldPage.contents = PPTContent;
                        if (!saveData.oldPages.Contains(oldPage))
                        {
                            saveData.oldPages.Add(oldPage);
                        }                        
                    }
                }
            }            
        }

        private string NewCourseName()
        {
            foreach (var item in AllCourse)
            {
                if (item.IsNew==true)
                {
                    return item.Name;
                }
            }
            return CurrCourse.Name;
        }

        private bool isContainPageInSaveData(string PageName)
        {
            foreach (var item in saveData.pages)
            {
                if (item.name==PageName)
                {                    
                    return true;
                }
            }
            return false;
        }

        private bool isContainStroke(string pageName)
        {
            foreach (var item in saveData.pages)
            {
                if (item.name==pageName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 在录制状态下，用于保存旧课中各个Page上的Strokes
        /// </summary>
        /// <param name="strokes"></param>
        private LineContent SaveOldCourseStroke(Stroke item)
        {
            var content = new LineContent();
            content.type = "Stroke";
            var dots = new Dot[item.StylusPoints.Count];
            var dotIndex = 0;
            if (item.DrawingAttributes.IsHighlighter)
            {
                foreach (var point in item.StylusPoints)
                {
                    var dot = new Dot();
                    dot.x = point.X;
                    dot.y = point.Y;
                    dot.radius = item.DrawingAttributes.Width;
                    dots[dotIndex] = dot;
                    dotIndex++;
                }
            }
            else
            {
                foreach (var point in item.StylusPoints)
                {
                    var dot = new Dot();
                    dot.x = point.X;
                    dot.y = point.Y;
                    dot.radius = item.DrawingAttributes.Width * point.PressureFactor;
                    dots[dotIndex] = dot;
                    dotIndex++;
                }
            }
            content.dots = dots;
            var options = new Option();
            var color = item.DrawingAttributes.Color;
            options.color = string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, color.ScA);
            options.dotType = item.DrawingAttributes.IsHighlighter ? "square" : "round";
            content.options = options;
            content.index= (Int32)item.GetPropertyData(guid);

            return content;
        }

        /// <summary>
        /// 判断当前的PPT是否已经添加到PPTS中
        /// </summary>
        /// <param name="pptName">新添加的PPT的名字</param>
        /// <returns></returns>
        private bool isContainPPTS(string pptName)
        {
            foreach (var item in saveData.PPTS)
            {
                if (item.name == pptName)
                {
                    return true;
                }                
            }
            return false;
        }
    
    }
}
