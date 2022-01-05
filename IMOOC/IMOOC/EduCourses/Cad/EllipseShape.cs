using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public class EllipseShape : BaseShape
    {
        private Point p1, p2, p3;
        private double left, top, width, height;

        private Ellipse ellipse;
        public override Shape Shape
        {
            get { return ellipse; }
        }

        public override BaseShape CatchShape(Point p)
        {
            return null;
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
                if (p.X - p1.X > p.Y - p1.Y)
                {
                    p2.X = p.X;
                    p2.Y = p1.Y;
                }
                else
                {
                    p2.X = p1.X;
                    p2.Y = p.Y;
                }
            }
            else if (p3.X == 0)
            {
                p3 = p;
                if (p2.Y == p1.Y)
                {
                    var c = (p2 - p1).Length / 2;
                    var a = ((p3 - p1).Length + (p3 - p2).Length) / 2;
                    var b = Math.Sqrt(a * a - c * c);
                    width = a * 2;
                    height = b * 2;
                    left = p1.X - a + c;
                    top = p1.Y - b;
                }
                else
                {
                    var c = (p2 - p1).Length / 2;
                    var a = ((p3 - p1).Length + (p3 - p2).Length) / 2;
                    var b = Math.Sqrt(a * a - c * c);
                    height = a * 2;
                    width = b * 2;
                    left = p1.X - b;
                    top = p1.Y - a + c;
                }

                ellipse = new Ellipse() { Width = width, Height = height, Stroke = Brushes.AliceBlue };
                updata();
            }
        }

        private void updata()
        {
            ellipse.SetValue(Canvas.LeftProperty, left);
            ellipse.SetValue(Canvas.TopProperty, top);
            Rect = new Rect(left, top, width, height);
            var ps = new Point[2];
            ps[0] = p1;
            ps[1] = p2;
            SetHitPoints(ps);
        }
    }
}
