using System.Collections.Generic;
using System.Windows.Ink;

namespace IMOOC.EduCourses
{
    public class InkCanvasPage:NotificationObject
    {
        private StrokeCollection strokes;

        public StrokeCollection Strokes
        {
            get { return strokes; }
            set { RaisePropertyChanged(ref strokes, value, "Strokes"); }
        }
        
        public InkCanvasChildCollection AllChild { get; set; }

        public List<int> HistoryMaxCount { get; set; }

        public InkCanvasPage()
        {
            Strokes = new StrokeCollection();
            AllChild = new InkCanvasChildCollection();
            HistoryMaxCount = new List<int>();
            HistoryMaxCount.Add(0);
            HistoryMaxCount[0] = 0;
        }
    }
}
