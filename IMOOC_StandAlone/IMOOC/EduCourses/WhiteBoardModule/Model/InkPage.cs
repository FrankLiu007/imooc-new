using System.Windows.Ink;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public class InkPage : NotificationObject
    {
        private StrokeCollection strokes;

        public StrokeCollection Strokes
        {
            get { return strokes; }
            set
            {
                strokes = value;
                RaisePropertyChanged("Strokes");
            }
        }

        private int pageNum;

        public int PageNum
        {
            get { return pageNum; }
            set
            {
                pageNum = value;
                RaisePropertyChanged("PageNum");
            }
        }
        
        public InkPage()
        {
            strokes = new StrokeCollection();
        }
    }
}
