using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IMOOC.EduCourses
{
    class CustomButton : Button
    {

        public Brush PromptColor
        {
            get { return (Brush)GetValue(PromptColorProperty); }
            set { SetValue(PromptColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PromptColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PromptColorProperty =
            DependencyProperty.Register("PromptColor", typeof(Brush), typeof(CustomButton), new PropertyMetadata(Brushes.Black,
                            new PropertyChangedCallback(PromptColorPropertyChangedCallback)));

        private static void PromptColorPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null && sender is CustomButton)
            {
                CustomButton but = sender as CustomButton;
                but.PromptPen.Stroke = but.PromptColor;
                but.PromptHilight.Fill = but.PromptColor;
            }
        }



        public int PromptThick
        {
            get { return (int)GetValue(PromptThickProperty); }
            set { SetValue(PromptThickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PromptThick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PromptThickProperty =
            DependencyProperty.Register("PromptThick", typeof(int), typeof(CustomButton), new PropertyMetadata(10,
                            new PropertyChangedCallback(PromptThickPropertyChangedCallback)));

        private static void PromptThickPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null && sender is CustomButton)
            {
                CustomButton but = sender as CustomButton;
                but.PromptPen.StrokeThickness = but.PromptThick;
                but.PromptHilight.Height = but.PromptThick;
            }
        }



        public bool PenTrans
        {
            get { return (bool)GetValue(PenTransProperty); }
            set { SetValue(PenTransProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PenTrans.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PenTransProperty =
            DependencyProperty.Register("PenTrans", typeof(bool), typeof(CustomButton),
                            new PropertyMetadata(false, new PropertyChangedCallback(PenTransPropertyChangedCallback)));

        private static void PenTransPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null && sender is CustomButton)
            {
                if ((sender as CustomButton).PenTrans == true)
                {
                    CustomButton but = sender as CustomButton;
                    but.Vb.Child = but.PromptHilight;
                }
                else
                {
                    CustomButton but = sender as CustomButton;
                    but.Vb.Child = but.PromptPen;
                }

            }
        }

        Path PromptPen;
        Rectangle PromptHilight;
        Viewbox Vb;
        public CustomButton() : base()
        {
            this.Width = 80;
            this.Height = 40;
            this.Background = Brushes.Turquoise;
            this.BorderThickness = new Thickness(0, 0, 0, 0);


            PromptPen = new Path();
            PromptHilight = new Rectangle();
            PromptPen = GetPenPath();
            PromptHilight = GetHilightPath();

            Vb = new Viewbox();
            Vb.HorizontalAlignment = HorizontalAlignment.Stretch;
            Vb.Child = PromptPen;
            Vb.Margin = new Thickness(10, 0, 10, 0);
            this.Content = Vb;

        }

        private Rectangle GetHilightPath()
        {
            PromptHilight.Height = PromptThick;
            PromptHilight.Width = 80;
            PromptHilight.Fill = PromptColor;
            return PromptHilight;
        }


        private Path GetPenPath()
        {
            var pathFigure = new PathFigure { StartPoint = new Point(0.5, 62.0814) };
            var c1 = new PolyBezierSegment();
            c1.Points.Add(new Point(63.2869, 32.0771));
            c1.Points.Add(new Point(126.074, 2.07277));
            c1.Points.Add(new Point(172.627, 0.499985));
            var c2 = new PolyBezierSegment();
            c1.Points.Add(new Point(219.18, -1.0728));
            c1.Points.Add(new Point(249.499, 25.7859));
            c1.Points.Add(new Point(281.999, 53.1192));
            var c3 = new PolyBezierSegment();
            c1.Points.Add(new Point(314.499, 80.4526));
            c1.Points.Add(new Point(349.18, 108.26));
            c1.Points.Add(new Point(398.872, 107.333));
            var c4 = new PolyBezierSegment();
            c1.Points.Add(new Point(448.564, 106.406));
            c1.Points.Add(new Point(513.268, 76.7443));
            c1.Points.Add(new Point(577.971, 47.0822));
            pathFigure.Segments.Add(c1);
            pathFigure.Segments.Add(c2);
            pathFigure.Segments.Add(c3);
            pathFigure.Segments.Add(c4);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            PromptPen.Data = pathGeometry;
            PromptPen.StrokeStartLineCap = PenLineCap.Round;
            PromptPen.StrokeEndLineCap = PenLineCap.Round;

            PromptPen.StrokeThickness = PromptThick;
            PromptPen.Stroke = Brushes.Red;
            return PromptPen;
        }

    }
}
