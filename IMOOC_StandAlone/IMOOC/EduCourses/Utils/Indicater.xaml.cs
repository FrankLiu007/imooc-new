using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Windows.Media;
//Test For HouShuai
namespace IMOOC.EduCourses
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class Indicater : UserControl
    {
        private InkCanvasChildCollection selectedAllChild;
        private StrokeCollection selectedStrokes;
        public CustomInkCanvas inkcanvas;
        public event EventHandler<IndicaterDragCompletedEventArgs> DragCompleted;
        private DateTime lastTime;

        public Indicater()
        {
            InitializeComponent();
            Width = 0;
            Height = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
                
        }

        public void updataIndicater(Thickness margin, double w, double h,
            StrokeCollection selectStrokes = null, ReadOnlyCollection<UIElement> selectUIElement = null)
        {
            Margin = margin;
            Width = w;
            Height = h;
            if (Width >= 30 && Height >= 30)
            {
                middleThumb.Width = Width - 30;
                middleThumb.Height = Height - 30;
            }
            selectedStrokes = selectStrokes;
            if (selectUIElement != null)
            {
                selectedAllChild = new InkCanvasChildCollection();
                foreach (var item in selectUIElement)
                {
                    selectedAllChild.Add((item as FrameworkElement).DataContext as InkCanvasChild);
                }

            }

        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            lastTime = DateTime.Now;          
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            Point p1 = new Point(e.HorizontalChange, e.VerticalChange);

            if (Math.Abs(p1.X) < SystemParameters.MinimumHorizontalDragDistance &&
                   Math.Abs(p1.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }           
            if (thumb.Name == "leftTopThumb")
            {
                p1.Y = (p1.X * Height) / Width;//让四个角只能按比例缩放
                if (Width - p1.X <= 66 || Height - p1.Y <= 66)
                    return;
                Width -= p1.X;
                Height -= p1.Y;
                Margin = new Thickness(Margin.Left + p1.X, Margin.Top + p1.Y, Margin.Right, Margin.Bottom);
            }
            else if (thumb.Name == "middleTopThumb")
            {
                if (Height - p1.Y <= 66)
                { return; }
                Height -= p1.Y;
                Margin = new Thickness(Margin.Left, Margin.Top + p1.Y, Margin.Right, Margin.Bottom);
            }
            else if (thumb.Name == "rightTopThumb")
            {
                p1.Y = -(p1.X * Height) / Width;//让四个角只能按比例缩放
                if (Width + p1.X <= 66 || Height - p1.Y <= 66)
                    return;
                Width += p1.X;
                Height -= p1.Y;
                Margin = new Thickness(Margin.Left, Margin.Top + p1.Y, Margin.Right - p1.X, Margin.Bottom);
            }
            else if (thumb.Name == "leftMiddleThumb")
            {
                if (Width - p1.X <= 66)
                { return; }

                Width -= p1.X;
                Margin = new Thickness(Margin.Left + p1.X, Margin.Top, Margin.Right, Margin.Bottom);
            }
            else if (thumb.Name == "rightMiddleThumb")
            {
                if (Width + p1.X <= 66)
                { return; }

                Width += p1.X;
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right - p1.X, Margin.Bottom);
            }
            else if (thumb.Name == "leftBottomThumb")
            {
                p1.Y = -(p1.X * Height) / Width;//让四个角只能按比例缩放
                if (Width - p1.X <= 66 || Height + p1.Y <= 66)
                    return;
                Width -= p1.X;
                Height += p1.Y;
                Margin = new Thickness(Margin.Left + p1.X, Margin.Top, Margin.Right, Margin.Bottom - p1.Y);
            }
            else if (thumb.Name == "middleBottomThumb")
            {
                if (Height + p1.Y <= 66)
                { return; }
                Height += p1.Y;
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom - p1.Y);
            }
            else if (thumb.Name == "rightBottomThumb")
            {
                p1.Y = (p1.X * Height) / Width;//让四个角只能按比例缩放
                if (Width + p1.X <= 66 || Height + p1.Y <= 66)
                    return;
                Width += p1.X;
                Height += p1.Y;
                Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right - p1.X, Margin.Bottom - p1.Y);
            }
            else if (thumb.Name == "middleThumb")
            {
                Margin = new Thickness(Margin.Left + p1.X, Margin.Top + p1.Y, Margin.Right - p1.X, Margin.Bottom - p1.Y);
            }

            if (Width >= 30 && Height >= 30)
            {
                middleThumb.Width = Width - 30;
                middleThumb.Height = Height - 30;
            }

        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var thumb = sender as Thumb;
            Point p = new Point(e.HorizontalChange, e.VerticalChange);

            if (Math.Abs(p.X) < SystemParameters.MinimumHorizontalDragDistance &&
                   Math.Abs(p.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            var source = PresentationSource.FromVisual(this);
            double scaleX = 1, scaleY = 1;
            if (source != null)
            {
                scaleX = source.CompositionTarget.TransformToDevice.M11;
                scaleY = source.CompositionTarget.TransformToDevice.M22;
            }
            p.X /= scaleX;
            p.Y /= scaleY;

            Rect oldRect, newRect;
            oldRect = inkcanvas.GetSelectionBounds();
            if (double.IsInfinity(oldRect.Width))
                return;

            newRect = new Rect(Margin.Left + 28, Margin.Top + 28, Width - 56, Height - 56);

            TransformStrokes(oldRect, newRect);
            TransformImage(oldRect, newRect);

            (DataContext as CoursesViewModel).indicaterMoveDuration = (DateTime.Now - lastTime).TotalMilliseconds;

            OnDragCompleted(new IndicaterDragCompletedEventArgs(oldRect, newRect, selectedStrokes, selectedAllChild));

        }

        public void TransformStrokes(Rect src, Rect dst)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-src.X, -src.Y);
            m.Scale(dst.Width / src.Width, dst.Height / src.Height);
            m.Translate(+dst.X, +dst.Y);
            selectedStrokes.Transform(m, false);
        }

        public void TransformImage(Rect src, Rect dst)
        {
            if (selectedAllChild == null) return;
            selectedAllChild.Transform(src, dst);
        }

        protected virtual void OnDragCompleted(IndicaterDragCompletedEventArgs e)
        {
            DragCompleted?.Invoke(this, e);
        }

        private void IndicaterCut_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ButtonCutSelection();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ButtonCopySelection();
        }
    }

    public class IndicaterDragCompletedEventArgs : EventArgs
    {
        public Rect OldRectangle { get; private set; }
        public Rect NewRectangle { get; private set; }
        public StrokeCollection SelectedStrokes { get; private set; }
        public InkCanvasChildCollection SelectedAllChild { get; private set; }

        public IndicaterDragCompletedEventArgs(Rect oldRect, Rect newRect,
            StrokeCollection selectedStrokes, InkCanvasChildCollection selectedAllChild)
        {
            OldRectangle = oldRect;
            NewRectangle = newRect;
            SelectedStrokes = selectedStrokes;
            SelectedAllChild = selectedAllChild;
        }
    }
}
