using IMOOC.EduCourses.Cad;
using IMOOC.EduCourses.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Insert
{
    /// <summary>
    /// Interaction logic for InsertCanvas.xaml
    /// </summary>
    public partial class InsertCanvas : UserControl
    {
        private Canvas canvas;
        private ObservableCollection<BaseShape> shapeList;
        private ItemStack itemStack;
        private BaseShape shape;
        private Point hitPoint;
        private AdornerLayer myAdornerLayer;
        private Adorner preAdorner;

        public InsertCanvas()
        {
            InitializeComponent();
            hitPoint = new Point(-1, -1);
            Loaded += InsertCanvas_Loaded;
        }

        private void InsertCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = new Canvas()
            {
                Width = (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.ActualWidth,
                Height = (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.ActualHeight,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01ffffff"))
            };
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            (DataContext as CoursesViewModel).CourseCtrl.inkcanvasGrid.Children.Add(canvas);
            myAdornerLayer = AdornerLayer.GetAdornerLayer(canvas);

            shapeList = new ObservableCollection<BaseShape>();
            shapeList.CollectionChanged += ShapeList_CollectionChanged;
            itemStack = new ItemStack(shapeList);
        }

        private void ShapeList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                var temp = (BaseShape)e.NewItems[0];
                canvas.Children.Add(temp.Shape);
                ActionItem ai = new ShapeAddedAI(itemStack, temp);
                itemStack.Enqueue(ai);
            }
            if (e.OldItems != null)
            {
                var temp = (BaseShape)e.OldItems[0];
                canvas.Children.Remove(temp.Shape);
                ActionItem ai = new ShapeRemovedAI(itemStack, temp);
                itemStack.Enqueue(ai);
            }

        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p2 = e.GetPosition(canvas);
            if (DeleteBtn.IsChecked == true)
            {
                foreach (var item in shapeList)
                {
                    var catchedShape = item.CatchShape(p2);
                    if (catchedShape != null)
                    {
                        shapeList.Remove(catchedShape);
                        return;
                    }
                }
            }
            if (shape == null)
            {
                if (lineBtn.IsChecked == true)
                {
                    shape = new LineShape();
                }
                else if (circleBtn.IsChecked == true)
                {
                    shape = new CircleShape();
                }
                else if (ellipseBtn.IsChecked == true)
                {
                    shape = new EllipseShape();
                }
                else if (hyperbolaBtn.IsChecked == true)
                {
                    shape = new HyperboleShape();
                }
                else if (coordinateBtn.IsChecked==true)
                {
                    shape = new CoordinateShape();
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var p2 = e.GetPosition(canvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //catchedShape.PointerMove(p2-lastPoint);
                //lastPoint = p2;
                return;
            }

            foreach (var item in shapeList)
            {
                hitPoint = item.CatchHitPoint(p2);
                if (hitPoint.X != -1)
                {
                    myAdornerLayer.Add(new PointAdorner(canvas, hitPoint, "垂足"));
                    return;
                }
            }
            if (shape == null)
            {
                Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(canvas);
                if (toRemoveArray != null)
                {
                    myAdornerLayer.Remove(toRemoveArray[0]);
                }
            }
            //   catchedShape = null;

        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p1;
            if (hitPoint.X == -1)
                p1 = e.GetPosition(canvas);
            else
                p1 = hitPoint;

            if (shape != null)
            {
                shape.PointerUp(p1);
                if (shape.Shape != null)
                {
                    shapeList.Add(shape);
                    shape = null;
                    preAdorner = null;
                    Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(canvas);
                    if (toRemoveArray != null)
                    {
                        for (int x = 0; x < toRemoveArray.Length; x++)
                        {
                            myAdornerLayer.Remove(toRemoveArray[x]);
                        }
                    }

                    foreach (var item in shapeList)
                    {
                        var line = item as LineShape;
                        if (line != null)
                        {
                            line.SetPer(new Point(-1, -1));
                        }
                    }
                }
                else
                {
                    if (shape is LineShape)
                    {
                        myAdornerLayer.Add(new CoordinateAdorner(canvas, p1));
                        preAdorner = new PreLineAdorner(canvas, p1);
                    }
                    else if (shape is CircleShape)
                    {
                        preAdorner = new PreCircleAdorner(canvas, p1);

                    }
                    else if (shape is EllipseShape)
                    {
                        preAdorner = new PreEllipseAdorner(canvas, p1);
                    }
                    else if (shape is HyperboleShape)
                    {
                        preAdorner = new PreHyperboleAdorner(canvas, p1);
                    }
                    else if (shape is CoordinateShape)
                    {
                        preAdorner = new CoordinateAdorner(canvas, p1);
                    }
                    else
                    {
                        preAdorner = new PreLineAdorner(canvas, p1);
                    }
                    preAdorner.MouseDown += PreAdorner_MouseDown;
                    preAdorner.MouseMove += PreAdorner_MouseMove;
                    preAdorner.MouseUp += PreAdorner_MouseUp;
                    myAdornerLayer.Add(preAdorner);
                    preAdorner.CaptureMouse();
                }

                foreach (var item in shapeList)
                {
                    var line = item as LineShape;
                    if (line != null)
                    {
                        line.SetPer(p1);
                    }
                }
            }

        }

        private void PreAdorner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                shape = null;
                preAdorner = null;
                Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(canvas);
                if (toRemoveArray != null)
                {
                    for (int x = 0; x < toRemoveArray.Length; x++)
                    {
                        myAdornerLayer.Remove(toRemoveArray[x]);
                    }
                }
            }

        }

        private void PreAdorner_MouseUp(object sender, MouseButtonEventArgs e)
        {
            shape.PointerUp(e.GetPosition(canvas));
        }

        private void PreAdorner_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas_MouseMove(sender, e);
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (itemStack.CanUndo)
            {
                itemStack.Undo();
            }
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (itemStack.CanRedo)
            {
                itemStack.Redo();
            }
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.inkcanvasGrid.Children.Remove(canvas);

            if (shapeList.Count > 0)
            {
                double x1 = double.MaxValue, y1 = double.MaxValue, x2 = double.MinValue, y2 = double.MinValue;

                foreach (var item in shapeList)
                {
                    if (item.Rect.Left < x1)
                        x1 = item.Rect.Left;
                    if (item.Rect.Right > x2)
                        x2 = item.Rect.Right;
                    if (item.Rect.Top < y1)
                        y1 = item.Rect.Top;
                    if (item.Rect.Bottom > y2)
                        y2 = item.Rect.Bottom;
                }
                foreach (var item in canvas.Children)
                {
                    var line = item as Line;
                    if (line != null)
                    {
                        line.X1 -= x1;
                        line.X2 -= x1;
                        line.Y1 -= y1;
                        line.Y2 -= y1;
                        continue;
                    }
                    var ellipse = item as Ellipse;
                    if (ellipse != null)
                    {
                        var left = (double)ellipse.GetValue(Canvas.LeftProperty);
                        var top = (double)ellipse.GetValue(Canvas.TopProperty);
                        ellipse.SetValue(Canvas.LeftProperty, left - x1);
                        ellipse.SetValue(Canvas.TopProperty, top - y1);
                        continue;
                    }
                }
                canvas.Width = x2 - x1;
                canvas.Height = y2 - y1;
                (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.AllChild.Add(new InkCanvasChild() { X = x1, Y = y1, UiEle = canvas });
            }

            (DataContext as CoursesViewModel).CourseCtrl.ToolGrid.Children.Remove(this);
        }
    }
}
