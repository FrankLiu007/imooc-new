using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public sealed class LineShape : BaseShape
    {
        private Point p1, p2;

        private Line line;

        public override Shape Shape
        {
            get { return line; }
        }

        public override BaseShape CatchShape(Point p)
        {
            var lx = line.X1 - line.X2;
            var ly = line.Y1 - line.Y2;
            var l1 = Math.Sqrt(lx * lx + ly * ly);
            lx = p.X - line.X1;
            ly = p.Y - line.Y1;
            var l2 = Math.Sqrt(lx * lx + ly * ly);
            lx = p.X - line.X2;
            ly = p.Y - line.Y2;
            var l3 = Math.Sqrt(lx * lx + ly * ly);

            if (l2 + l3 - l1 < .1)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public override void PointerDown(Point p)
        {
            throw new NotImplementedException();
        }

        public override void PointerMove(Point p)
        {
            throw new NotImplementedException();
        }

        public override void PointerUp(Point p)
        {
            if (p1.X == 0)
            {
                p1 = p;
            }
            else if (p2.X == 0)
            {
                p2 = p;
                line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.AliceBlue };
                updata();
            }
        }

        private void updata()
        {
            Rect = new Rect(p1, p2);
            var ps = new Point[4];
            ps[0] = new Point(line.X1, line.Y1);
            ps[1] = new Point((line.X1 + line.X2) / 2, (line.Y1 + line.Y2) / 2);
            ps[2] = new Point(line.X2, line.Y2);
            ps[3] = new Point(-1, -1);
            SetHitPoints(ps);
        }

        public void SetPer(Point p)
        {
            if (p.X==-1)
            {
                SetHitPoints(p, 3);
            }
            else
            {
                var k = ((p.X - p1.X) * (p2.X - p1.X) + (p.Y - p1.Y) * (p2.Y - p1.Y)) / ((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
                SetHitPoints(new Point(p1.X + k * (p2.X - p1.X), p1.Y + k * (p2.Y - p1.Y)), 3);
            }
            
        }
    }
}
