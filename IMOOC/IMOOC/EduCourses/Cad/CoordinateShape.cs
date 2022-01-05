using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public sealed class CoordinateShape : BaseShape
    {
        private Point p1;
        private double length = 200;
        private Path coordinate;
        public override Shape Shape
        {
            get { return coordinate; }
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
            if (p1.X==0)
            {
                p1 = p;
            }
            else
            {
                length = (p - p1).Length;
                var ps = new Point[9];
                var delta = length / 3;
                for (int i = 0; i < 5; i++)
                {
                    ps[i] = new Point(p1.X + (i - 2) * delta, p1.Y);
                }
                ps[5] = new Point(p1.X, p1.Y - 2 * delta);
                ps[6] = new Point(p1.X, p1.Y - delta);
                ps[7] = new Point(p1.X, p1.Y + delta);
                ps[8] = new Point(p1.X, p1.Y + 2 * delta);

                var horizontal = new PathFigure();
                horizontal.StartPoint = new Point(p1.X - length, p1.Y);
                horizontal.Segments.Add(new LineSegment(new Point(p1.X + length, p1.Y), true));
                

                var vertical = new PathFigure();
                vertical.StartPoint = new Point(p1.X, p1.Y - length);
                vertical.Segments.Add(new LineSegment(new Point(p1.X, p1.Y + length), true));

                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(horizontal);
                pathGeometry.Figures.Add(vertical);
                for (int i = 0; i < 5; i++)
                {
                    if (i!=2)
                    {
                        pathGeometry.Figures.Add(GetLineFigure(ps[i], "vertical"));
                    }
                }
                for (int i = 5; i < 9; i++)
                {
                    pathGeometry.Figures.Add(GetLineFigure(ps[i], "horizontal"));
                }

                coordinate = new Path();
                coordinate.Data = pathGeometry;
                coordinate.Stroke = Brushes.AliceBlue;

                Rect = new Rect(p1.X - length, p1.Y - length, length, length);
                SetHitPoints(ps);
            }
            
        }

        private PathFigure GetLineFigure(Point p,string orientation)
        {
            var figure = new PathFigure();
            figure.StartPoint = p;
            if (orientation=="horizontal")
            {
                figure.Segments.Add(new LineSegment(new Point(p.X + 7, p.Y), true));
            }
            else if (orientation=="vertical")
            {
                figure.Segments.Add(new LineSegment(new Point(p.X, p.Y - 7), true));
            }
            return figure;
        }

    }
}
