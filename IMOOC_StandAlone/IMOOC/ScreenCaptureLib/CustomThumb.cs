using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

//screen
namespace ScreenCaptureLib
{
    using Public;
    enum CustomThumbPlacement
    {
        None,
        LeftTop,
        MiddleTop,
        RightTop,
        LeftMiddle,
        Middle,
        RightMiddle,
        LeftBottom,
        MiddleBottom,
        RightBottom
    }

    class CustomThumb : Thumb
    {
           
        public MaskWindow mask;
        public Indicater indicater;

        public CustomThumbPlacement Placement
        {
            get
            {
                return (CustomThumbPlacement)GetValue(PlacementProperty);
            }
            set
            {
                SetValue(PlacementProperty, value);
            }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(CustomThumbPlacement),
            typeof(CustomThumb), new UIPropertyMetadata(CustomThumbPlacement.None));

        public CustomThumb()
            : base()
        {
            FocusVisualStyle = null;
            DragStarted += Thumb_DragStart;
            DragDelta += Thumb_DragDelta;
            DragCompleted += Thumb_DragCompleted;

        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
       
        }

        private void Thumb_DragStart(object sender, DragStartedEventArgs e)
        {
            //throw new NotImplementedException();

        }


        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (mask == null) return;
            Point p1 = new Point(e.HorizontalChange, e.VerticalChange);

            if (Math.Abs(p1.X) < SystemParameters.MinimumHorizontalDragDistance &&
                   Math.Abs(p1.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            double x = Canvas.GetLeft(indicater);
            double y = Canvas.GetTop(indicater);
            double w = indicater.ActualWidth;
            double h = indicater.ActualHeight;

            if (Placement == CustomThumbPlacement.LeftTop)
            {
                x = x + p1.X;
                y = y + p1.Y;
                w = w - p1.X ;
                h = h - p1.Y;
            }
            else if(Placement==CustomThumbPlacement.MiddleTop)
            {
              //  x = x + (p1.X - p0.X);
                y = y + p1.Y ;
             //   w = w - (p1.X - p0.X);
                h = h - p1.Y ;
            }
            else if(Placement==CustomThumbPlacement.RightTop)
            {
               // x = x -(p1.X - p0.X);
                y = y + p1.Y ;
                w = w + p1.X ;
                h = h - p1.Y ;
            }
            else if (Placement == CustomThumbPlacement.LeftMiddle)
            {
                 x = x +p1.X ;
               // y = y + (p1.Y - p0.Y);
                w = w - p1.X ;
               // h = h - (p1.Y - p0.Y);
            }
            else if (Placement == CustomThumbPlacement.RightMiddle)
            {
               // x = x + (p1.X - p0.X);
                // y = y + (p1.Y - p0.Y);
                w = w + p1.X ;
                // h = h - (p1.Y - p0.Y);
            }
            else if (Placement == CustomThumbPlacement.LeftBottom)
            {
                 x = x + p1.X ;
                // y = y + (p1.Y - p0.Y);
                w = w - p1.X ;
                 h = h + p1.Y ;
            }
            else if (Placement == CustomThumbPlacement.MiddleBottom)
            {
                //x = x + (p1.X - p0.X);
                // y = y + (p1.Y - p0.Y);
               // w = w - (p1.X - p0.X);
                h = h + p1.Y ;
            }
            else if (Placement == CustomThumbPlacement.RightBottom)
            {
                //x = x + (p1.X - p0.X);
                // y = y + (p1.Y - p0.Y);
                 w = w +p1.X ;
                h = h + p1.Y ;
            }
            else if (Placement == CustomThumbPlacement.Middle)
            {
                x = x + p1.X ;
                 y = y + p1.Y ;
               // w = w + (p1.X - p0.X);
               // h = h + (p1.Y - p0.Y);
            }
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (w < 0) w = 0;
            if (h < 0) h = 0;

         //   Rect rect = new Rect(x, y, w, h);
            mask.UpdateSelectRegion(new Rect(x, y, w, h));

        }


        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
 
            indicater = this.GetAncestor<Indicater>();
            mask = this.GetAncestor<MaskWindow>();
  
        }





    }





}
