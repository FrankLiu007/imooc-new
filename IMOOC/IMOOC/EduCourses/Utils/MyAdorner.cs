using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Utils
{
    public sealed class PointAdorner : Adorner
    {
        private Pen pen;
        private Point p;
        private string str;

        public PointAdorner(UIElement AdornedElement, Point point, string txt) : base(AdornedElement)
        {
            this.IsHitTestVisible = true;
            p = point;
            str = txt;
            pen = new Pen(Brushes.Red, 1.0);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(null, pen, new Rect(p.X - 6, p.Y - 6, 12, 12));
            if (str != null)
            {
                var typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                dc.DrawText(new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, 12, Brushes.Black), new Point(p.X + 10, p.Y - 5));
            }
        }

    }

    public sealed class CoordinateAdorner : Adorner
    {
        private Pen pen;
        private Point p1, p2;

        public CoordinateAdorner(UIElement adornedElement, Point point) : base(adornedElement)
        {
            IsHitTestVisible = false;
            p1 = point;
            pen = new Pen(Brushes.LightGray, 1.0);
            pen.DashStyle = DashStyles.DashDotDot;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            p2 = e.GetPosition(AdornedElement);
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseMouseCapture();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (p2.X==0)
            {
                dc.DrawLine(pen, new Point(p1.X - 120, p1.Y), new Point(p1.X + 120, p1.Y));
                dc.DrawLine(pen, new Point(p1.X, p1.Y - 120), new Point(p1.X, p1.Y + 120));
            }
            else
            {
                var length = (p2 - p1).Length;
                dc.DrawLine(pen, new Point(p1.X - length, p1.Y), new Point(p1.X + length, p1.Y));
                dc.DrawLine(pen, new Point(p1.X, p1.Y - length), new Point(p1.X, p1.Y + length));
            }
        }
    }

    public sealed class PreLineAdorner : Adorner
    {
        private Pen pen;
        private Point p1, p2;
        private double angle;
        private Typeface typeFace;

        public PreLineAdorner(UIElement adornedElement, Point point) : base(adornedElement)
        {
            IsHitTestVisible = false;
            p1 = p2 = point;
            pen = new Pen(Brushes.LightGray, 1.0);
            pen.DashStyle = DashStyles.Dash;
            typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            p2 = e.GetPosition(AdornedElement);
            angle = Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X)) / Math.PI * 180;
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseMouseCapture();
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawLine(pen, p1, p2);
            dc.DrawText(new FormattedText(angle.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, 12, Brushes.Black), p1);
        }
    }

    public sealed class PreCircleAdorner : Adorner
    {
        private Pen pen;
        private Point p1, p2, p3, po;
        private bool isp2Set;
        private double rx, ry, radius;

        public PreCircleAdorner(UIElement adornedElement, Point point) : base(adornedElement)
        {
            IsHitTestVisible = false;
            p1 = p2 = p3 = point;
            pen = new Pen(Brushes.LightGray, 1.0);
            pen.DashStyle = DashStyles.Dash;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            isp2Set = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!isp2Set)
            {
                p2 = e.GetPosition(AdornedElement);
            }
            else
            {
                p3 = e.GetPosition(AdornedElement);
            }
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (isp2Set)
                ReleaseMouseCapture();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (!isp2Set)
            {
                po.X = (p2.X + p1.X) / 2;
                po.Y = (p2.Y + p1.Y) / 2;
                rx = po.X - p1.X;
                ry = po.Y - p1.Y;
                radius = Math.Sqrt(rx * rx + ry * ry);
                dc.DrawEllipse(null, pen, po, radius, radius);
            }
            else
            {
                po.X = ((p3.Y - p1.Y) * (p2.Y * p2.Y - p1.Y * p1.Y + p2.X * p2.X - p1.X * p1.X) + (p2.Y - p1.Y) * (p1.Y * p1.Y - p3.Y * p3.Y + p1.X * p1.X - p3.X * p3.X)) / (2 * (p2.X - p1.X) * (p3.Y - p1.Y) - 2 * (p3.X - p1.X) * (p2.Y - p1.Y));
                po.Y = ((p3.X - p1.X) * (p2.X * p2.X - p1.X * p1.X + p2.Y * p2.Y - p1.Y * p1.Y) + (p2.X - p1.X) * (p1.X * p1.X - p3.X * p3.X + p1.Y * p1.Y - p3.Y * p3.Y)) / (2 * (p2.Y - p1.Y) * (p3.X - p1.X) - 2 * (p3.Y - p1.Y) * (p2.X - p1.X));
                rx = po.X - p1.X;
                ry = po.Y - p1.Y;
                radius = Math.Sqrt(rx * rx + ry * ry);
                dc.DrawEllipse(null, pen, po, radius, radius);
            }
        }
    }

    public sealed class PreEllipseAdorner : Adorner
    {
        private Pen pen;
        private bool isp2Set;
        private Point p1, p2, p3, po;
        private double a, b, c;

        public PreEllipseAdorner(UIElement adornedElement, Point point) : base(adornedElement)
        {
            IsHitTestVisible = false;
            p1 = p2 = p3 = point;
            pen = new Pen(Brushes.LightGray, 1.0);
            pen.DashStyle = DashStyles.Dash;

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            isp2Set = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!isp2Set)
            {
                var p = e.GetPosition(AdornedElement);
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
            else
            {
                p3 = e.GetPosition(AdornedElement);
            }
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (isp2Set)
                ReleaseMouseCapture();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (!isp2Set)
            {
                dc.DrawLine(pen, p1, p2);
            }
            else
            {
                if (p2.Y == p1.Y)
                {
                    c = (p2 - p1).Length / 2;
                    a = ((p3 - p1).Length + (p3 - p2).Length) / 2;
                    b = Math.Sqrt(a * a - c * c);
                    po.X = p1.X + c;
                    po.Y = p1.Y;
                }
                else
                {
                    c = (p2 - p1).Length / 2;
                    b = ((p3 - p1).Length + (p3 - p2).Length) / 2;
                    a = Math.Sqrt(b * b - c * c);
                    po.X = p1.X;
                    po.Y = p1.Y + c;
                }
                dc.DrawEllipse(null, pen, po, a, b);
            }
        }

    }

    public sealed class PreHyperboleAdorner : Adorner
    {
        private Pen pen;
        private bool isp2Set;
        private Point p1, p2, p3;
        private double a, b, c;

        public PreHyperboleAdorner(UIElement adornedElement, Point point) : base(adornedElement)
        {
            IsHitTestVisible = false;
            p1 = p2 = p3 = point;
            pen = new Pen(Brushes.LightGray, 1.0);
            pen.DashStyle = DashStyles.Dash;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            isp2Set = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!isp2Set)
            {
                var p = e.GetPosition(AdornedElement);
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
            else
            {
                p3 = e.GetPosition(AdornedElement);
            }
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (isp2Set)
                ReleaseMouseCapture();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (!isp2Set)
            {
                dc.DrawLine(pen, p1, p2);
            }
            else
            {
                c = (p2 - p1).Length / 2;
                a = Math.Abs((p3 - p1).Length - (p3 - p2).Length) / 2;
                b = Math.Sqrt(c * c - a * a);

                var y = Math.Sqrt(c * c / a / a - 1) * b;

                var line = new LineSegment(new Point(p1.X + c - a, p1.Y), true);
                var line1 = new LineSegment(new Point(p1.X, p1.Y + y), true);
                var figure = new PathFigure();
                figure.StartPoint = new Point(p1.X, p1.Y - y);
                figure.Segments.Add(line);
                figure.Segments.Add(line1);

                var line2 = new LineSegment(new Point(p2.X - c + a, p2.Y), true);
                var line3 = new LineSegment(new Point(p2.X, p2.Y + y), true);
                var figure1 = new PathFigure();
                figure1.StartPoint = new Point(p2.X, p2.Y - y);
                figure1.Segments.Add(line2);
                figure1.Segments.Add(line3);

                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure);
                pathGeometry.Figures.Add(figure1);
                dc.DrawGeometry(null, pen, pathGeometry);

            }
        }
    }

}
