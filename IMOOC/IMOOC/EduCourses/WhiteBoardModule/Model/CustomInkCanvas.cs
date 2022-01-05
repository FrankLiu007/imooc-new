using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public class CustomInkCanvas : InkCanvas
    {
        public bool IsRecord
        {
            get { return (bool)GetValue(isRecordProperty); }
            set { SetValue(isRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isRecordProperty =
            DependencyProperty.Register("IsRecord", typeof(bool), typeof(CustomInkCanvas), new PropertyMetadata(false));


        public List<RecordItem> RecordList
        {
            get { return (List<RecordItem>)GetValue(RecordListProperty); }
            set { SetValue(RecordListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecordList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecordListProperty =
            DependencyProperty.Register("RecordList", typeof(List<RecordItem>),
                typeof(CustomInkCanvas), new PropertyMetadata(new List<RecordItem>()));


        private List<TimeSpan> TimeSpans;
        private List<int> index;

        private DateTime lastTime;
        private StrokeCollection SelectedStrokes;
        private Rect SelectedRect;
        private  System.Collections.ObjectModel.ReadOnlyCollection<UIElement> SelectedEliments;

        public CustomInkCanvas() : base()
        {
            // Use the custom dynamic renderer on the
            // custom InkCanvas.
            SelectedEliments = null;
            SelectedRect = new Rect(0,0,0,0);
            SelectedStrokes = null;
        }

        #region stylus
        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            base.OnStylusDown(e);
            // Allocate memory for the StylusPointsCollection and
            // add the StylusPoints that have come in so far.
            //stylusPoints = new StylusPointCollection();
            //StylusPointCollection eventPoints =
            //    e.GetStylusPoints(this, stylusPoints.Description);

            //stylusPoints.Add(eventPoints);

            lastTime = DateTime.Now;
            TimeSpans = new List<TimeSpan>();
            index = new List<int>();
            TimeSpans.Add(DateTime.Now - lastTime);
            index.Add(Strokes.Count);
        }

        protected override void OnStylusMove(StylusEventArgs e)
        {
            base.OnStylusMove(e);
            if (TimeSpans == null)
            {
                return;
            }
            DateTime now = DateTime.Now;
            TimeSpans.Add(now - lastTime);
            lastTime = now;
            index.Add(e.GetStylusPoints(this).Count);
        }

        //protected override void OnStylusUp(StylusEventArgs e)
        //{
        //    base.OnStylusUp(e);
        //}
        #endregion

        #region mouse
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);

        //}


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Don't collect points unless the left mouse button
            // is down.
            // If a stylus generated this event, return.
            if (e.StylusDevice != null)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Released ||
                EditingMode != InkCanvasEditingMode.Ink)
            {
                return;
            }
            if (TimeSpans == null)
            {
                lastTime = DateTime.Now;
                TimeSpans = new List<TimeSpan>();
            }
            DateTime now = DateTime.Now;
            TimeSpans.Add(now - lastTime);
            lastTime = now;

        }

        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonUp(e);
        //    // If a stylus generated this event, return.
        //    if (e.StylusDevice != null|| TimeSpans == null)
        //    {
        //        return;
        //    }

        //    TimeSpans.Add(DateTime.Now - lastTime);

        //}
        #endregion

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            base.OnStrokeCollected(e);
            if (IsRecord)
            {
                int count = RecordList.Count - 1;
                RecordList[count].TimePoints = TimeSpans;
                RecordList[count].CountList = index;
                RecordList[count].Time -= TimeSpans[TimeSpans.Count - 1];
            }
            
            //Debug.WriteLine(e.Stroke.StylusPoints.Count.ToString() +" "+ TimeSpans.Count.ToString());
            TimeSpans = null;
            index = null;
        }

//--------------------------------------------------------------------



        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            UpdateAdorner();          

        }

        private void UpdateAdorner()
        {

            SelectedRect = GetSelectionBounds();
            if (SelectedRect.IsEmpty)
            {
                return;
            }

            if (GetSelectedStrokes() != null)
            {
                SelectedStrokes = GetSelectedStrokes();
                Strokes.Remove(SelectedStrokes);
                Strokes.Add(SelectedStrokes);
            }
            if (GetSelectedElements() == null)
            {
                SelectedEliments = GetSelectedElements();
                foreach (var s in SelectedEliments)
                {
                    Children.Remove(s);
                    Children.Add(s);
                }
            }

            (DataContext as WhiteBoardViewModel).IndicaterMargin=new Thickness(SelectedRect.X - 16, SelectedRect.Y - 30,
             ActualWidth - SelectedRect.Right - 16, ActualHeight - SelectedRect.Bottom - 10);

           // (objectIndicater as Indicater).updataIndicater(new Thickness(bounds.X - 16, bounds.Y - 30,
           //  this.RenderSize.Width - bounds.X - bounds.Width - 16, this.RenderSize.Height - bounds.Y - bounds.Height - 10), bounds);
           

        }

        public void MoveSelectedStrokes(double x, double y)
        {

            foreach (Stroke stroke in SelectedStrokes)
            {
                Strokes.Remove(stroke);
                //  CustomInkCanvas.SetLeft(stroke, 10);

                StylusPointCollection sps = new StylusPointCollection();
                for (int i = 0; i < stroke.StylusPoints.Count; i++)
                {

                    sps.Add(new StylusPoint(stroke.StylusPoints[i].X + x, stroke.StylusPoints[i].Y + y,
                        stroke.StylusPoints[i].PressureFactor));

                }
                Stroke newStroke = new Stroke(sps, stroke.DrawingAttributes);
                Strokes.Add(newStroke);
              //  newselect.Add(newStroke);


            }
         //   Select(newselect);
         //   newselect.Clear();


        }

        public void ResizeStroke(Point anchor, double ratio_x, double ratio_y)
        {
            foreach (Stroke stroke in SelectedStrokes)
            {
                Strokes.Remove(stroke);
                StylusPointCollection sps = new StylusPointCollection();
                for (int i = 0; i < stroke.StylusPoints.Count; i++)
                {
                    sps.Add(new StylusPoint(stroke.StylusPoints[i].X * ratio_x + (SelectedRect.X - 32) * (1 - ratio_x) + anchor.X,
                        stroke.StylusPoints[i].Y * ratio_y + (SelectedRect.Y - 40) * (1 - ratio_y) + anchor.Y,
                        stroke.StylusPoints[i].PressureFactor));
                }
                Stroke   newStroke = new Stroke(sps, stroke.DrawingAttributes);
                Strokes.Add(newStroke);
             //   newselect.Add(newStroke);


            }
         //   Select(newselect);
         //   newselect.Clear();
        }





    }

}
