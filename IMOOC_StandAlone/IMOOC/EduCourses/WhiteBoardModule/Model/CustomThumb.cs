using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace IMOOC.EduCourses.WhiteBoardModule
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
        public Indicater indicater;
        public Thickness margin;


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


        }

        
          
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
           
            Point p1 = new Point(e.HorizontalChange, e.VerticalChange);

            if (Math.Abs(p1.X) < SystemParameters.MinimumHorizontalDragDistance &&
                   Math.Abs(p1.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (indicater.state_image == 0)
            {

                if (Placement == CustomThumbPlacement.LeftTop)
                {


                    double ratio_x = (indicater.Width - p1.X) / indicater.Width;
                    double ratio_y = (indicater.Height - p1.Y) / indicater.Height;

                    indicater.ResizeStroke(p1, ratio_x, ratio_y);



                }
                else if (Placement == CustomThumbPlacement.MiddleTop)
                {

                    double ratio_y = (indicater.Height - p1.Y) / indicater.Height;
                    p1.X = 0;
                    indicater.ResizeStroke(p1, 1, ratio_y);

                }
                else if (Placement == CustomThumbPlacement.RightTop)
                {


                    double ratio_x = (indicater.Width + p1.X) / indicater.Width;
                    double ratio_y = (indicater.Height - p1.Y) / indicater.Height;
                    p1.X = 32 * (1 - ratio_x);

                    indicater.ResizeStroke(p1, ratio_x, ratio_y);


                }
                else if (Placement == CustomThumbPlacement.LeftMiddle)
                {
                    double ratio_x = (indicater.Width - p1.X) / indicater.Width;

                    p1.Y = 0;

                    indicater.ResizeStroke(p1, ratio_x, 1);

                }
                else if (Placement == CustomThumbPlacement.RightMiddle)
                {
                    double ratio_x = (indicater.Width + p1.X) / indicater.Width;
                    p1.Y = 0;
                    p1.X = 32 * (1 - ratio_x);

                    indicater.ResizeStroke(p1, ratio_x, 1);


                }
                else if (Placement == CustomThumbPlacement.LeftBottom)
                {
                    double ratio_x = (indicater.Width - p1.X) / indicater.Width;
                    double ratio_y = (indicater.Height + p1.Y) / indicater.Height;
                    p1.Y = 40 * (1 - ratio_y);

                    indicater.ResizeStroke(p1, ratio_x, ratio_y);
                }
                else if (Placement == CustomThumbPlacement.MiddleBottom)
                {
                    double ratio_y = (indicater.Height + p1.Y) / indicater.Height;
                    p1.X = 0;
                    p1.Y = 40 * (1 - ratio_y);

                    indicater.ResizeStroke(p1, 1, ratio_y);


                }
                else if (Placement == CustomThumbPlacement.RightBottom)
                {
                    double ratio_x = (indicater.Width + p1.X) / indicater.Width;
                    double ratio_y = (indicater.Height + p1.Y) / indicater.Height;
                    p1.X = 32 * (1 - ratio_x);
                    p1.Y = 40 * (1 - ratio_y);
                    indicater.ResizeStroke(p1, ratio_x, ratio_y);

                }
                else if (Placement == CustomThumbPlacement.Middle)
                {
                    indicater.moveStroke(p1.X, p1.Y);

                }

            }
            else
            {
                if (Placement == CustomThumbPlacement.LeftTop)
                {

                    indicater.updataIndicater(new Thickness(indicater.Margin.Left + p1.X, indicater.Margin.Top + p1.Y,
                                                indicater.Margin.Right, indicater.Margin.Bottom)
                                                , indicater.ActualWidth-p1.X, indicater.ActualHeight-p1.Y);
                    indicater.updataImage();

                
                }

                else if (Placement == CustomThumbPlacement.MiddleTop)
                {

                    indicater.updataIndicater(new Thickness(indicater.Margin.Left , indicater.Margin.Top + p1.Y,
                                                indicater.Margin.Right, indicater.Margin.Bottom)
                                                , indicater.ActualWidth, indicater.ActualHeight - p1.Y);
                    indicater.updataImage();

                }
                else if (Placement == CustomThumbPlacement.RightTop)
                {


                    indicater.updataIndicater(new Thickness(indicater.Margin.Left, indicater.Margin.Top+p1.Y,
                                                indicater.Margin.Right-p1.X, indicater.Margin.Bottom)
                                                , indicater.ActualWidth + p1.X, indicater.ActualHeight - p1.Y);
                    indicater.updataImage();


                }
                else if (Placement == CustomThumbPlacement.LeftMiddle)
                {
                    indicater.updataIndicater(new Thickness(indicater.Margin.Left + p1.X, indicater.Margin.Top,
                                                indicater.Margin.Right, indicater.Margin.Bottom)
                                                , indicater.ActualWidth - p1.X, indicater.ActualHeight);
                    indicater.updataImage();

                }
                else if (Placement == CustomThumbPlacement.RightMiddle)
                {
                    indicater.updataIndicater(new Thickness(indicater.Margin.Left, indicater.Margin.Top,
                                                indicater.Margin.Right-p1.X, indicater.Margin.Bottom)
                                                , indicater.ActualWidth + p1.X, indicater.ActualHeight);
                    indicater.updataImage();


                }
                else if (Placement == CustomThumbPlacement.LeftBottom)
                {
                    indicater.updataIndicater(new Thickness(indicater.Margin.Left + p1.X, indicater.Margin.Top,
                                                indicater.Margin.Right, indicater.Margin.Bottom-p1.Y)
                                                , indicater.ActualWidth - p1.X, indicater.ActualHeight + p1.Y);
                    indicater.updataImage();
                }
                else if (Placement == CustomThumbPlacement.MiddleBottom)
                {
                    indicater.updataIndicater(new Thickness(indicater.Margin.Left, indicater.Margin.Top,
                                                indicater.Margin.Right, indicater.Margin.Bottom-p1.Y)
                                                , indicater.ActualWidth, indicater.ActualHeight +p1.Y);
                    indicater.updataImage();


                }
                else if (Placement == CustomThumbPlacement.RightBottom)
                {
                    indicater.updataIndicater(new Thickness(indicater.Margin.Left, indicater.Margin.Top,
                                                indicater.Margin.Right-p1.X, indicater.Margin.Bottom-p1.Y)
                                                , indicater.ActualWidth + p1.X, indicater.ActualHeight + p1.Y);
                    indicater.updataImage();

                }
                else if (Placement == CustomThumbPlacement.Middle)
                {

                    indicater.updataIndicater(new Thickness(indicater.Margin.Left+p1.X,indicater.Margin.Top+p1.Y,
                                                indicater.Margin.Right-p1.X,indicater.Margin.Bottom-p1.Y)
                                                ,indicater.Width,indicater.Height);

                    indicater.updataImage();
                }

            }

          
        }


        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            indicater = this.GetAncestor<Indicater>();
        }

        


    }
}
