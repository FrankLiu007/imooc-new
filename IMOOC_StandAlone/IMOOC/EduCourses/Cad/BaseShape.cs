using System.Windows;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public abstract class BaseShape
    {
        private Point[] hitPoints;
        private Rect[] hitPointRects;
        private int hitPointIndex;
        public Rect Rect { get; set; }
        public abstract Shape Shape { get; }


        protected void SetHitPoints(Point p,int index)
        {
            hitPoints[index] = p;
            hitPointRects[index] = new Rect(p.X - 5, p.Y - 5, 10, 10);
        }

        protected void SetHitPoints(Point[] points)
        {
            if (hitPoints == null)
            {
                hitPoints = new Point[points.Length];
                hitPointRects = new Rect[points.Length];
            }
            for (int i = 0; i < points.Length; i++)
            {
                SetHitPoints(points[i], i);
            }
        }

        /// <summary>
        /// 捕获跟踪点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point CatchHitPoint(Point p)
        {
            for (int i = 0; i < hitPointRects.Length; i++)
            {
                if (hitPointRects[i].Contains(p))
                {
                    hitPointIndex = i;
                    return hitPoints[i];
                }
            }
            return new Point(-1, -1);
        }
        
        public abstract BaseShape CatchShape(Point p);
        public abstract void PointerDown(Point p);
        public abstract void PointerMove(Point p);
        public abstract void PointerUp(Point p);

    }
}
