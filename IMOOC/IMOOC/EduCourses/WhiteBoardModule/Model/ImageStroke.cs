using System.Windows.Media;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public class ImageStroke : Stroke
    {
        ImageSource image;
        public ImageStroke(StylusPointCollection stylusPoints, ImageSource imageSource) : base(stylusPoints)
        {
            image = imageSource;
        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            //drawingContext.DrawImage(image, new Rect(0,0,900,image.Height*900/image.Width));
            
            Point topLeft = new Point(StylusPoints[0].X, StylusPoints[0].Y);//100, 100
            Point topRight = new Point(StylusPoints[1].X, StylusPoints[1].Y);//200, 100
            Point centerLeft = new Point(StylusPoints[2].X, StylusPoints[2].Y);//100, 200
            Point centerRight = new Point(StylusPoints[3].X, StylusPoints[3].Y);//200, 200
            PathGeometry pathGeomery = new PathGeometry();
            PathSegmentCollection pathCollection = new PathSegmentCollection();
            pathCollection.Add(new LineSegment(topLeft, true));
            pathCollection.Add(new LineSegment(topRight, true));
            pathCollection.Add(new LineSegment(centerRight, true));
            pathCollection.Add(new LineSegment(centerLeft, true));
            pathCollection.Add(new LineSegment(topLeft, true));
            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = true;
            pathFigure.IsFilled = true;
            pathFigure.StartPoint = topLeft;
            pathFigure.Segments = pathCollection;
            //图片刷
            ImageBrush myimageBrush = new ImageBrush();
            myimageBrush.ImageSource = image;
            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathFigure);
            pathGeomery.Figures = pathFigureCollection;
            drawingContext.DrawGeometry(myimageBrush, new Pen(Brushes.Blue, 2), pathGeomery);
            
        }

    }
}
