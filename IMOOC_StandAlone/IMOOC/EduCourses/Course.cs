using IMOOC.EduCourses.PPTModule;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IMOOC.EduCourses
{

    public class Course : NotificationObject
    {
        //-------------------------板书部分--------------------------------
        private ObservableCollection<InkCanvasPage> allPage;

        public ObservableCollection<InkCanvasPage> AllPage
        {
            get { return allPage; }
            set { allPage = value; }
        }

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
                CurrPage = AllPage[CurrPageIndex];                
            }
        }

        //--------------------------PPT部分----------------------------------------

        private int currPPTIndex;

        public int CurrPPTIndex
        {
            get { return currPPTIndex; }
            set
            {
                RaisePropertyChanged(ref currPPTIndex, value, "CurrPage");
                CurrPPT = allPPT[pptNameList[value]];                
                CurrPage = new InkCanvasPage() { Strokes = currPPT.GetCurrStrokes(), HistoryMaxCount = currPPT.GetCurrPageHistoryMaxCount() };
            }            
        }

        private PPTModel currPPT;

        public PPTModel CurrPPT
        {
            get { return currPPT; }
            set { RaisePropertyChanged(ref currPPT, value, "CurrPage"); }
        }

        private ObservableCollection<string> pptNameList;

        public ObservableCollection<string> PPTNameList
        {
            get { return pptNameList; }
            set { pptNameList = value; }
        }

        private Dictionary<string, PPTModel> allPPT;

        public Dictionary<string, PPTModel> AllPPT
        {
            get { return allPPT; }
            set { allPPT = value; }
        }

        //--------------------------其他属性---------------------------------------
        
        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string coursePath;
        public string CoursePath
        {
            get { return coursePath; }
            set { coursePath = value; }
        }

        private bool isModify;
        /// <summary>
        /// 该课程资源是否有新的修改，用于保存课程，不用于录制
        /// </summary>
        public bool IsModify
        {
            get { return isModify; }
            set { isModify = value; }
        }

        private bool isSaved;
        /// <summary>
        /// 该课程是否保存过
        /// </summary>
        public bool IsSaved
        {
            get { return isSaved; }
            set { isSaved = value; }
        }

        private bool isBanshu;

        public bool IsBanshu
        {
            get { return isBanshu; }
            set { isBanshu = value; }
        }


        public Course()
        {
            allPage = new ObservableCollection<InkCanvasPage>();
            currPage = new InkCanvasPage();
            allPage.Add(currPage);
            currPageIndex = 0;
            isModify = false;
            isSaved = false;
            isBanshu = true;

            allPPT = new Dictionary<string, PPTModel>();
            currPPT = null;
            pptNameList = new ObservableCollection<string>();

        }

        public Course(string name,string path,bool isNew)
        {
            Name = name;
            CoursePath = path;
            IsNew = isNew;
            isModify = false;
            isSaved = false;
            isBanshu = true;

            allPage = new ObservableCollection<InkCanvasPage>();
            currPage = new InkCanvasPage();
            allPage.Add(currPage);
            currPageIndex = 0;

            allPPT = new Dictionary<string, PPTModel>();
            currPPT = null;
            pptNameList = new ObservableCollection<string>();

        }
  
    }
}
