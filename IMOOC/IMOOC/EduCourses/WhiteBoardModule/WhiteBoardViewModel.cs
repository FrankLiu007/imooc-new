
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Threading;

using TCPSocketModule;
namespace IMOOC.EduCourses.WhiteBoardModule
{
    using IMOOC.EduCourses;

    public class WhiteBoardViewModel : NotificationObject
    {
        private ObservableCollection<InkPage> allPage;
        public ObservableCollection<InkPage> AllPage
        {
            get { return allPage; }
            set { allPage = value; }
        }

        private StrokeCollection currPageStroke;
        public StrokeCollection CurrPageStroke
        {
            get { return currPageStroke; }
            set
            {
                currPageStroke = value;
                RaisePropertyChanged("CurrPageStroke");
            }
        }

        private int currPageIndex;
        public int CurrPageIndex
        {
            get { return currPageIndex; }
            set
            {
                currPageIndex = value;
                RaisePropertyChanged("CurrPageIndex");
                CurrPageStroke = allPage[currPageIndex].Strokes;
            }
        }

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
        
        public SocketClient ConnectSocket { get; set; }
        

        public List<CommandStack> cmdStackList;
        public List<int> editingOperationCountList;
        private List<DelegateCommand> commandList;
        private List<TimeSpan> commandTimeList;

        //private DispatcherTimer timer;
        private DateTime lastTime;
        //private int strokeIndex;
        //private int commandIndex;

        //private SocketServer server;

        public DelegateCommand NewPageCommand { set; get; }
        public DelegateCommand DeletePageCommand { set; get; }
        public DelegateCommand NextPageCommand { set; get; }
        public DelegateCommand PreviousPageCommand { set; get; }
        public DelegateCommand StartRecordCommand { set; get; }
        public DelegateCommand ReplayCommand { set; get; }

        public WhiteBoardViewModel()
        {
            IsRecord = false;

            AllPage = new ObservableCollection<InkPage>();
            cmdStackList = new List<CommandStack>();
            editingOperationCountList = new List<int>();
            initialPgae();

            commandList = new List<DelegateCommand>();
            commandTimeList = new List<TimeSpan>();
            RecordItemList = new List<RecordItem>();
            //timer = new DispatcherTimer();
            //timer.Tick += Timer_Tick;
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 100);


            NewPageCommand = new DelegateCommand(new Action(newPage));
            DeletePageCommand = new DelegateCommand(new Action(deletePage));
            NextPageCommand = new DelegateCommand(new Action(nextPage));
            PreviousPageCommand = new DelegateCommand(new Action(previousPage));
            StartRecordCommand = new DelegateCommand(new Action(StartRecord));
            ReplayCommand = new DelegateCommand(new Action(Replay));
        }

        private void initialPgae()
        {
            AllPage.Clear();
            AllPage.Add(new InkPage() { PageNum = 1 });
            CurrPageIndex = 0;
            CurrPageStroke.StrokesChanged += Strokes_StrokesChanged;
            cmdStackList.Clear();
            editingOperationCountList.Clear();
            cmdStackList.Add(new CommandStack(allPage[currPageIndex].Strokes));
            editingOperationCountList.Add(0);
        }

        private void newPage()
        {
            AllPage.Insert(currPageIndex + 1, new InkPage() { PageNum = currPageIndex + 2 });
            cmdStackList.Insert(currPageIndex + 1, new CommandStack(allPage[currPageIndex + 1].Strokes));
            editingOperationCountList.Insert(currPageIndex + 1, 0);
            CurrPageIndex += 1;
            CurrPageStroke.StrokesChanged += Strokes_StrokesChanged;
            for (int i = currPageIndex + 1; i < allPage.Count; i++)
            {
                allPage[i].PageNum = i + 1;
            }
            record(NewPageCommand);
        }

        private void deletePage()
        {
            if (allPage.Count > 1)
            {
                AllPage.RemoveAt(currPageIndex);
                cmdStackList.RemoveAt(currPageIndex);
                editingOperationCountList.RemoveAt(currPageIndex);
                CurrPageIndex = allPage.Count > currPageIndex ? currPageIndex : currPageIndex - 1;
                for (int i = currPageIndex; i < allPage.Count; i++)
                {
                    allPage[i].PageNum = i + 1;
                }
                record(DeletePageCommand);
            }

        }

        private void nextPage()
        {
            if (currPageIndex < allPage.Count - 1)
            {
                CurrPageIndex += 1;
                record(NextPageCommand);
            }

        }

        private void previousPage()
        {
            if (currPageIndex > 0)
            {
                CurrPageIndex -= 1;
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

            CommandItem cItem = new StrokesAddedOrRemovedCI(cmdStackList[currPageIndex],
                InkCanvasEditingMode.Ink, added, removed, editingOperationCountList[currPageIndex]);
            cmdStackList[currPageIndex].Enqueue(cItem);

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
            CommandItem item = new SelectionMovedOrResizedCI(cmdStackList[CurrPageIndex],
                selectedStrokes, newRect, oldRect, editingOperationCountList[CurrPageIndex]);
            cmdStackList[CurrPageIndex].Enqueue(item);

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
                initialPgae();
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
            initialPgae();
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
                        commandList[i - 1].Execute(CurrPageStroke);
                    }));

                }
            }).Start();
            //lastTime = DateTime.Now;
            //strokeIndex = 0;
            //commandIndex = 0;
            //timer.Start();
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
