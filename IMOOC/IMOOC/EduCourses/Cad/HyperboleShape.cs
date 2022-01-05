using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public class HyperboleShape : BaseShape
    {
        private Point p1, p2, p3;
        private Path hyperbola;
        public override Shape Shape
        {
            get { return hyperbola; }
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

                var c = (p2 - p1).Length / 2;
                var a = Math.Abs((p3 - p1).Length - (p3 - p2).Length) / 2;
                var b = Math.Sqrt(c * c - a * a);

                var y = Math.Sqrt(c * c / a / a - 1) * b;
                
                var leftFigure = new PathFigure();
                leftFigure.StartPoint = new Point(p1.X, p1.Y - y);
                leftFigure.Segments.Add(new LineSegment(new Point(p1.X + c - a, p1.Y), true));
                leftFigure.Segments.Add(new LineSegment(new Point(p1.X, p1.Y + y), true));
                
                var rightFigure = new PathFigure();
                rightFigure.StartPoint = new Point(p2.X, p2.Y - y);
                rightFigure.Segments.Add(new LineSegment(new Point(p2.X - c + a, p2.Y), true));
                rightFigure.Segments.Add(new LineSegment(new Point(p2.X, p2.Y + y), true));

                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(leftFigure);
                pathGeometry.Figures.Add(rightFigure);

                hyperbola = new Path();
                hyperbola.Data = pathGeometry;
                hyperbola.Stroke = Brushes.AliceBlue;


                Rect = new Rect(new Point(p1.X, p1.Y - y), new Point(p2.X, p2.Y + y));
                var ps = new Point[2];
                ps[0] = p1;
                ps[1] = p2;
                SetHitPoints(ps);
            }
        }
        
    }
}
