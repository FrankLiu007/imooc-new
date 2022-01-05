using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public class CircleShape : BaseShape
    {
        private Point p1, p2, p3, po;
        private double left, top, radius;

        private Ellipse ellipse;
        public override Shape Shape
        {
            get { return ellipse; }
        }

        public override BaseShape CatchShape(Point p)
        {
            var len = (p - po).Length;
            if (len < radius + 5 && len > radius - 5)
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
            }
            else if (p3.X == 0)
            {
                p3 = p;
                po = new Point(((p3.Y - p1.Y) * (p2.Y * p2.Y - p1.Y * p1.Y + p2.X * p2.X - p1.X * p1.X) + (p2.Y - p1.Y) * (p1.Y * p1.Y - p3.Y * p3.Y + p1.X * p1.X - p3.X * p3.X)) / (2 * (p2.X - p1.X) * (p3.Y - p1.Y) - 2 * (p3.X - p1.X) * (p2.Y - p1.Y)),
                    ((p3.X - p1.X) * (p2.X * p2.X - p1.X * p1.X + p2.Y * p2.Y - p1.Y * p1.Y) + (p2.X - p1.X) * (p1.X * p1.X - p3.X * p3.X + p1.Y * p1.Y - p3.Y * p3.Y)) / (2 * (p2.Y - p1.Y) * (p3.X - p1.X) - 2 * (p3.Y - p1.Y) * (p2.X - p1.X)));

                radius = (po - p1).Length;
                ellipse = new Ellipse() { Width = radius * 2, Height = radius * 2, Stroke = Brushes.AliceBlue };
                updata();
            }
        }

        private void updata()
        {
            left = po.X - radius;
            top = po.Y - radius;
            ellipse.SetValue(Canvas.LeftProperty, left);
            ellipse.SetValue(Canvas.TopProperty, top);
            Rect = new Rect(left, top, ellipse.Width, ellipse.Height);
            var ps = new Point[1];
            ps[0] = po;
            SetHitPoints(ps);
        }
    }
}
