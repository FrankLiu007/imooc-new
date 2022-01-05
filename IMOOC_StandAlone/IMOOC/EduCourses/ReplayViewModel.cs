using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using IMOOC.EduCourses.PPTModule;
using PPT = Microsoft.Office.Interop.PowerPoint;

namespace IMOOC.EduCourses
{
    public sealed class ReplayViewModel : NotificationObject
    {
        private InkCanvasPage currPage;

        public InkCanvasPage CurrPage
        {
            get { return currPage; }
            set { RaisePropertyChanged(ref currPage, value, "CurrPage"); }
        }

        private int currPageIndex;

        public int CurrPageIndex
        {
            get { return currPageIndex; }
            set
            {
                RaisePropertyChanged(ref currPageIndex, value, "CurrPageIndex");
                CurrPage = allPage[currPageIndex];
            }
        }

        private double currTime;

        public double CurrTime
        {
            get { return currTime; }
            set { RaisePropertyChanged(ref currTime, value, "CurrTime"); }
        }

        private bool isShifting;

        public bool IsShifting
        {
            get { return isShifting; }
            set { RaisePropertyChanged(ref isShifting, value, "IsShifting"); }
        }

        private int rate;

        public int Rate
        {
            get { return rate; }
            set { RaisePropertyChanged(ref rate, value, "Rate"); }
        }

        private double targetPoint;

        public double TargetPoint
        {
            get { return targetPoint; }
            set { RaisePropertyChanged(ref targetPoint, value, "TargetPoint"); }
        }

        private string currView;

        public string CurrView
        {
            get { return currView; }
            set { RaisePropertyChanged(ref currView, value, "CurrView"); }
        }

        private StrokeCollection currPPTStrokes;

        public StrokeCollection CurrPPTStrokes
        {
            get { return currPPTStrokes; }
            set { RaisePropertyChanged(ref currPPTStrokes, value, "CurrPPTStrokes"); }
        }

        private List<PPTPage> slideList;

        public List<PPTPage> SlideList
        {
            get { return slideList; }
            set { RaisePropertyChanged(ref slideList, value, "SlideList"); }
        }

        private System.Drawing.Image currSlideImage;
        public System.Drawing.Image CurrSlideImage
        {
            get { return currSlideImage; }
            set { RaisePropertyChanged(ref currSlideImage, value, "CurrSlideImage"); }
        }

        private int currOpenPPTIndex;

        public int CurrOpenPPTIndex
        {
            get { return currOpenPPTIndex; }
            set
            {
                currOpenPPTIndex = value;
                //currPPtName = pptOperation.PPTNameList[value];
            }
        }


        private List<InkCanvasPage> allPage;
        private List<RecordItem> recordItemList;
        private Dictionary<string, List<StrokeCollection>> pptStrokesList;
        private Dictionary<string, List<StrokesAddedOrRemovedRI>> pptSARList;
        private Dictionary<string, int> currPPTSARIndex;
        private List<CommandType> commandList;
        private List<TimeSpan> commandTimeList;
        private string last;
        private Task replayTask;
        private int currItemIndex, currCommandIndex, currPPTSlide;
        private string currPPtName;
        private CancellationTokenSource cancellSource;
        public double TotalTime;

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        public DelegateCommand NextPageCommand { get; set; }

        public ReplayViewModel(List<InkCanvasPage> Pages, Dictionary<string, List<StrokeCollection>> PPTStrokesList)
        {
            allPage = Pages;
            pptStrokesList = PPTStrokesList;
            CurrPageIndex = 0;
            CurrView = "BanShu";
            NextPageCommand = new DelegateCommand(new Action(nextPage));
        }

        private void nextPage()
        {
            if (currPageIndex < allPage.Count - 1)
            {
                CurrPageIndex += 1;
            }
        }

        public ReplayViewModel(List<RecordItem> itemList, Dictionary<string, List<StrokesAddedOrRemovedRI>> pptItemList, List<CommandType> typeList, List<TimeSpan> timeList)
        {
            allPage = new List<InkCanvasPage>();
            allPage.Add(new InkCanvasPage());
            CurrPageIndex = 0;
            currPPTSARIndex = new Dictionary<string, int>();

            pptStrokesList = new Dictionary<string, List<StrokeCollection>>();

            currOpenPPTIndex = -1;

            recordItemList = itemList;
            commandList = typeList;
            commandTimeList = timeList;
            pptSARList = pptItemList;

            foreach (var item in recordItemList)
            {
                var smr = item as SelectionMovedOrResizedRI;
                if (smr != null)
                {
                    smr.initial();
                }
            }
            timeList.ForEach((item) =>
            {
                TotalTime += item.TotalSeconds;
            });

            CurrView = "BanShu";
            last = "PPTCommon";
            cancellSource = new CancellationTokenSource();
            replayTask = new Task(new Action(setReplay), cancellSource.Token);

            StartCommand = new DelegateCommand(new Action(replayTask.Start));
            StopCommand = new DelegateCommand(new Action(cancellSource.Cancel));


            //pptOperation.FillAllPPT();
            //for (int i = 0; i < pptOperation.LPPTdocument.Count; i++)
            //{
            //    pptOperation.LPPTdocument[i].Application.SlideShowNextSlide += showNextSlide;

            //}

        }

        private void setReplay()
        {
            var forwardAction = new Action(forward);
            while (currCommandIndex < commandList.Count)
            {
                if (!isShifting)
                {
                    Thread.Sleep(commandTimeList[currCommandIndex]);
                    Application.Current.Dispatcher.Invoke(forwardAction);
                    CurrTime += commandTimeList[currCommandIndex].TotalSeconds;
                    currCommandIndex++;
                }
                else//isShifting
                {
                    if (rate != int.MaxValue)//普通快进
                    {
                        Thread.Sleep((int)commandTimeList[currCommandIndex].TotalMilliseconds / rate);
                        Application.Current.Dispatcher.Invoke(forwardAction);
                        CurrTime += commandTimeList[currCommandIndex].TotalSeconds;
                        currCommandIndex++;
                    }
                    else//最大速度快进或快退
                    {
                        if (targetPoint >= currTime)//进
                        {
                            Application.Current.Dispatcher.Invoke(forwardAction);
                            CurrTime += commandTimeList[currCommandIndex].TotalSeconds;
                            currCommandIndex++;
                            if (targetPoint <= currTime)
                            {
                                IsShifting = false;
                            }
                        }
                        else//退
                        {
                            //Application.Current.Dispatcher.Invoke(new Action(back));
                            //CurrTime -= commandTimeList[currCommandIndex].TotalSeconds;
                            //currCommandIndex--;
                            //if (targetPoint >= currTime)
                            //{
                            //    IsShifting = false;
                            //}
                        }

                    }
                }
                if (cancellSource.Token.IsCancellationRequested)
                    break;
            }
        }

        private void forward()
        {
            switch (commandList[currCommandIndex])
            {
                case CommandType.RePlay:
                    recordItemList[currItemIndex].Replay(currPage);
                    currItemIndex += 1;
                    break;
                case CommandType.NewPage:
                    allPage.Insert(currPageIndex + 1, new InkCanvasPage());
                    CurrPageIndex += 1;
                    break;
                case CommandType.DeletePage:
                    allPage.RemoveAt(currPageIndex);
                    CurrPageIndex = allPage.Count > currPageIndex ? currPageIndex : currPageIndex - 1;
                    break;
                case CommandType.PageIndexChanged:
                    CurrPageIndex += 1;
                    break;
                case CommandType.PPTRemark:
                    pptSARList[currPPtName][currPPTSARIndex[currPPtName]].pptReplay(currPPTStrokes);
                    currPPTSARIndex[currPPtName]++;
                    break;
                case CommandType.NextPPT:
                    //pptOperation.Next();
                    break;
                case CommandType.BanshuMenu:
                    last = currView;
                    CurrView = "BanShu";
                    break;
                case CommandType.PPTMenu:
                    CurrView = "PPTCommon";
                    break;
                case CommandType.CommonView:
                    CurrView = "PPTCommon";
                    break;
                case CommandType.BrowseView:
                    CurrView = "PPTBrowse";
                    break;
                case CommandType.PlayView:
                    //pptOperation.GotoSlide(pptOperation.PPTPlayCurrSlideIndexList[currPPTSlide]);
                    //pptOperation.ActivateSlideShowWindow();
                    currPPTSlide++;
                    CurrView = "PPTPlaying";
                    break;
                case CommandType.OpenPPT:
                    CurrOpenPPTIndex++;
                    //pptOperation.ReplayShowPPT(currPPtName);
                    openPPT(currPPtName);
                    CurrView = "PPTCommon";
                    break;
                case CommandType.ChangePPT:
                    CurrOpenPPTIndex++;
                    //pptOperation.ReplayShowPPT(currPPtName);
                    CurrPPTStrokes = pptStrokesList[currPPtName][0];
                    //pptOperation.ChangCVC(currPPtName);
                    CurrView = "PPTCommon";
                    break;
                default:
                    break;
            }
        }

        private void back()
        {
            //switch (commandList[currCommandIndex])
            //{
            //    case CommandType.NewPage:
            //        //AllPage.Insert(currPageIndex + 1, new InkCanvasPage());
            //        //CurrPageIndex += 1;
            //        break;
            //    case CommandType.DeletePage:
            //        //AllPage.RemoveAt(currPageIndex);
            //        //CurrPageIndex = allPage.Count > currPageIndex ? currPageIndex : currPageIndex - 1;
            //        break;
            //    case CommandType.NextPage:
            //        //CurrPageIndex += 1;
            //        break;
            //    case CommandType.PreviousPage:
            //        //CurrPageIndex -= 1;
            //        break;
            //    case CommandType.RePlay:
            //        currItemIndex--;
            //        recordItemList[currItemIndex].RollBack();
            //        break;
            //    default:
            //        break;
            //}
        }

        private void openPPT(string filename)
        {
            //pptOperation.courseControl = courseControl;

            //pptOperation.KillBackgroundPPT();

            //pptOperation.CreatePPTApplication();

            //pptOperation.loadPPT(filename);

            //pptOperation.document.Application.SlideShowNextSlide += showNextSlide;


            //var temp = new List<PPTPage>();

            //var lst = pptOperation.GetPPTImages();           
            var allStrokes = new List<StrokeCollection>();
            //for (int i = 0; i < pptOperation.LPPTdocument[pptOperation.ReplayReturnPPTIndex(currPPtName)].Slides.Count; i++)
            //{
            //    //PPTPage pg = new PPTPage(lst[i]);
            //    //pg.PageNum = i + 1;
            //    //temp.Add(pg);
            //    var strokes = new StrokeCollection();
            //    allStrokes.Add(strokes);
            //}
            //SlideList = temp;
            //CurrSlideImage = slideList[0].PageImage;
            pptStrokesList.Add(filename, allStrokes);
            currPPTSARIndex.Add(filename, 0);
            CurrPPTStrokes = pptStrokesList[filename][0];

            //pptOperation.ChangCVC(currPPtName);

        }

        private void showNextSlide(PPT.SlideShowWindow Wn)
        {

            if (currPPtName==null)
            {
                return;
            }
            //CurrPPTStrokes = pptStrokesList[currPPtName][pptOperation.LPPTdocument[pptOperation.ReplayReturnPPTIndex(currPPtName)].SlideShowWindow.View.Slide.SlideIndex - 1];

        }


    }

}
