using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Ink;
using TCPSocketModule;


namespace IMOOC.EduCourses
{
    using System.Windows.Controls;
    using System.Windows.Media;
    using WhiteBoardModule;
    class CourseViewModel:NotificationObject
    {
        private ObservableCollection<InkPage> allPage;
        
        private CustomInkCanvas currBanshuPage;
        public CustomInkCanvas CurrBanshuPage
        {
            get { return currBanshuPage; }
            set
            {
                currBanshuPage = value;
                RaisePropertyChanged("CurrBanshuPage");  
            }
        }

        private ObservableCollection<CustomInkCanvas> currBanshu;
        public ObservableCollection<CustomInkCanvas> CurrBanshu
        {
            get { return currBanshu; }
            set 
            {
                currBanshu = value;
                RaisePropertyChanged("CurrBanshu");
            }
        }


        private int currBanshuPageIndex;
        public int CurrBanshuPageIndex
        {
            get { return currBanshuPageIndex; }
            set
            {
                currBanshuPageIndex = value;
                RaisePropertyChanged("CurrBanshuPageIndex");
                AllCourse[currCourseIndex].currBanshuPageIndex = currBanshuPageIndex;
                CurrBanshuPage = AllCourse[currCourseIndex].currBanShuPage;
            }
        }

        private List<Course> AllCourse;

        private List<string> courseNameList;
        public List<string> CourseNameList
        {
            get
            {
                courseNameList.Clear();
                foreach (Course cs in AllCourse)
                { courseNameList.Add(cs.GetCourseName()); }
                return courseNameList;
            }
        }

        private Course currCourse;
        private Course CurrCourse
        {
            get { return currCourse; }
            set 
            {
                currCourse = value;
                RaisePropertyChanged("CurrCourse");    
            }
        }
        private int currCourseIndex;
        private int CurrCourseIndex
        {
            get { return currCourseIndex; }
            set
            {
                currCourseIndex = value;
                RaisePropertyChanged("CurrCourseIndex");
                CurrCourse = AllCourse[currCourseIndex];
            }
        }
        /*
 private StrokeCollection currPageStroke;
 public StrokeCollection CurrPageStroke
 {
     get { return currPageStroke; }
     set
     {
         currPageStroke = value;
         RaisePropertyChanged("CurrPageStroke");
     }
 }*/
        private Thickness indicaterMargin;
        public Thickness IndicaterMargin
        {
            get { return indicaterMargin; }

            set
            {
                indicaterMargin = value;                
                RaisePropertyChanged("IndicaterMargin");
                IndicaterHeight = indicaterMargin.Bottom - indicaterMargin.Top+16;
                IndicaterWith = indicaterMargin.Right - indicaterMargin.Left+16;
            }
        }

        private double indicaterWith;
        public double IndicaterWith
        {
            get { return indicaterWith; }
               
            set
            {
                indicaterWith = value;
                RaisePropertyChanged("IndicaterWith");

            }
        }

        private double indicaterHeight;
        public double IndicaterHeight
        {
            get { return indicaterHeight; }
            set
            {
                indicaterHeight = value;
                RaisePropertyChanged("IndicaterHeight");
            }
        }

        private Visibility indicaterVisibility;
        public Visibility IndicaterVisibility
        {
            get { return indicaterVisibility; }
            set 
            {
                indicaterVisibility = value;
                RaisePropertyChanged("IndicaterVisibility");
            }

        }

        private List<RecordItem> recordItemList;

        public List<RecordItem> RecordItemList
        {
            get { return recordItemList; }
            set
            {
                recordItemList = value;
                RaisePropertyChanged("RecordList");
            }
        }

        private bool isRecord;

        public bool IsRecord
        {
            get { return isRecord; }
            set
            {
                isRecord = value;
                RaisePropertyChanged("IsRecord");
            }
        }

        private Color penColor;
        public Color PenColor
        {
            get { return penColor; }
            set
            { 
                penColor = value;
                RaisePropertyChanged("PenColor");
            }
        }

        private double penWidth;
        public double PenWidth
        {
            get { return penWidth; }
            set
            {
                penWidth = value;
                RaisePropertyChanged("PenWidth");
            }
        }
        
        public SocketClient ConnectSocket { get; set; }
        

        public List<CommandStack> cmdStackList;
        public List<int> editingOperationCountList;

        private List<DelegateCommand> commandList;
        private List<TimeSpan> commandTimeList;
 
        private DateTime lastTime;
        

        //private SocketServer server;

        public DelegateCommand NewPageCommand { set; get; }
        public DelegateCommand DeletePageCommand { set; get; }
        public DelegateCommand NextPageCommand { set; get; }
        public DelegateCommand PreviousPageCommand { set; get; }
        public DelegateCommand StartRecordCommand { set; get; }
        public DelegateCommand ReplayCommand { set; get; }

        

        public CourseViewModel()
        {
            IsRecord = false;
            AllCourse = new List<Course>();

            allPage = new ObservableCollection<InkPage>();

            cmdStackList = new List<CommandStack>();
            editingOperationCountList = new List<int>();

            initCourse();

            commandList = new List<DelegateCommand>();
            commandTimeList = new List<TimeSpan>();
            RecordItemList = new List<RecordItem>();

            NewPageCommand = new DelegateCommand(new Action(newPage));
            DeletePageCommand = new DelegateCommand(new Action(deletePage));
            NextPageCommand = new DelegateCommand(new Action(nextPage));
            PreviousPageCommand = new DelegateCommand(new Action(previousPage));
            StartRecordCommand = new DelegateCommand(new Action(StartRecord));
            ReplayCommand = new DelegateCommand(new Action(Replay));
        }

        private void initCourse()
        {
          //  allPage.Clear();
            CurrCourse = new Course();
            AllCourse.Add(currCourse);
//            CurrBanshuPage = currCourse.GetBanShu()[0];
            CurrBanshuPageIndex = 0;
            CurrBanshuPage.EditingModeChanged += currBanshuPage_EditingModeChanged;
            CurrBanshuPage.DefaultDrawingAttributesReplaced += CurrBanshuPage_DefaultDrawingAttributesReplaced;
//            allPage.Add(new InkPage() { PageNum = 1 });
            
         //   CurrPageStroke.StrokesChanged += Strokes_StrokesChanged;
            cmdStackList.Clear();
            editingOperationCountList.Clear();
         //   cmdStackList.Add(new CommandStack(allPage[currPageIndex].Strokes));
            editingOperationCountList.Add(0);
        }

        void CurrBanshuPage_DefaultDrawingAttributesReplaced(object sender, DrawingAttributesReplacedEventArgs e)
        {
            //throw new NotImplementedException();
            PenColor = e.NewDrawingAttributes.Color;
            PenWidth = e.NewDrawingAttributes.Height;
        }

        void currBanshuPage_EditingModeChanged(object sender, RoutedEventArgs e)
        {
            if(CurrBanshuPage.EditingMode!=InkCanvasEditingMode.Select)
            {
                IndicaterVisibility = Visibility.Collapsed;
            }
            else
            {
                IndicaterVisibility = Visibility.Visible;
            }
        }

        private void newPage()
        {
           AllCourse[currCourseIndex].BanShu.Insert(currBanshuPageIndex + 1, new CustomInkCanvas() );
            CurrBanshuPageIndex += 1;
            CurrBanshuPage.EditingModeChanged += currBanshuPage_EditingModeChanged;
            CurrBanshuPage.DefaultDrawingAttributesReplaced += CurrBanshuPage_DefaultDrawingAttributesReplaced;

          //  CurrBanshuPage = currBanshu[CurrBanshuPageIndex];
          //  cmdStackList.Insert(currBanshuPageIndex + 1, new CommandStack(allPage[currBanshuPageIndex + 1].Strokes));
          //  editingOperationCountList.Insert(currBanshuPageIndex + 1, 0);
/*
         //   CurrPageStroke.StrokesChanged += Strokes_StrokesChanged;
            for (int i = currBanshuPageIndex + 1; i < allPage.Count; i++)
            {
                allPage[i].PageNum = i + 1;
            }*/
        //    record(NewPageCommand);
        }

        private void deletePage()
        {
            if (allPage.Count > 1)
            {
                allPage.RemoveAt(currBanshuPageIndex);
                cmdStackList.RemoveAt(currBanshuPageIndex);
                editingOperationCountList.RemoveAt(currBanshuPageIndex);
                CurrBanshuPageIndex = allPage.Count > currBanshuPageIndex ? currBanshuPageIndex : currBanshuPageIndex - 1;
                for (int i = currBanshuPageIndex; i < allPage.Count; i++)
                {
                    allPage[i].PageNum = i + 1;
                }
                record(DeletePageCommand);
            }

        }

        private void nextPage()
        {
            if (currBanshuPageIndex < AllCourse[currCourseIndex].BanShu.Count - 1)
            {
                CurrBanshuPageIndex += 1;
                record(NextPageCommand);
            }

        }


        private void previousPage()
        {
            if (currBanshuPageIndex > 0)
            {
                CurrBanshuPageIndex -= 1;
                record(PreviousPageCommand);
            }

        }

        private void record(DelegateCommand command)
        {
            if (isRecord)
            {
                commandList.Add(command);
                DateTime now = DateTime.Now;
                commandTimeList.Add(now - lastTime);
                lastTime = now;
            }
            //server.Send(Encoding.UTF8.GetBytes("command added\r\n"));
        }

        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            StrokeCollection added = new StrokeCollection(e.Added);
            StrokeCollection removed = new StrokeCollection(e.Removed);

            CommandItem cItem = new StrokesAddedOrRemovedCI(cmdStackList[currBanshuPageIndex],
                InkCanvasEditingMode.Ink, added, removed, editingOperationCountList[currBanshuPageIndex]);
            cmdStackList[currBanshuPageIndex].Enqueue(cItem);

            if (isRecord)
            {
                DateTime now = DateTime.Now;
                RecordItem rItem = new StrokesAddedOrRemovedRI(added, removed, now - lastTime);
                RecordItemList.Add(rItem);
                commandList.Add(new DelegateCommand(new Action<StrokeCollection>(rItem.Replay)));
                commandTimeList.Add(now - lastTime);
                lastTime = now;
            }
            SendStrokes(added);
        }

        public void SelectionMovingOrResizing(StrokeCollection selectedStrokes,
            InkCanvasSelectionEditingEventArgs e)
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

                e.NewRectangle = newRect2;
            }
            CommandItem item = new SelectionMovedOrResizedCI(cmdStackList[CurrBanshuPageIndex],
                selectedStrokes, newRect, oldRect, editingOperationCountList[CurrBanshuPageIndex]);
            cmdStackList[CurrBanshuPageIndex].Enqueue(item);

            if (isRecord)
            {
                DateTime now = DateTime.Now;
                RecordItem rItem = new SelectionMovedOrResizedRI(selectedStrokes, newRect, oldRect, now - lastTime);
                RecordItemList.Add(rItem);
                commandList.Add(new DelegateCommand(new Action<StrokeCollection>(rItem.Replay)));
                commandTimeList.Add(now - lastTime);
                lastTime = now;
            }
        }

        private void StartRecord()
        {
            if (MessageBox.Show("删除所有页面，重新开始录制吗？", "caption",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                initCourse();
                IsRecord = true;
                commandList.Clear();
                commandTimeList.Clear();
                RecordItemList.Clear();
                lastTime = DateTime.Now;
            }

        }

        private void Replay()
        {
            IsRecord = false;
            initCourse();
            for (int i = 0; i < recordItemList.Count; i++)
            {
                RecordItemList[i].InitialReplay();
            }
            new Thread(() =>
            {
                for (int i = 0; i < commandTimeList.Count; i++)
                {
                    Thread.Sleep(commandTimeList[i]);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
              //          commandList[i - 1].Execute(CurrPageStroke);
                    }));

                }
            }).Start();

        }

        void SendStrokes(StrokeCollection strokes)
        {
            if (ConnectSocket != null)
            {
                var customStrokes = new CustomStrokes(strokes);

                ConnectSocket.Send(customStrokes.GetBuffer());
                //server.Send(Encoding.UTF8.GetBytes("0123456789abcdefghijklmn"));
            }
        }
    }
}
