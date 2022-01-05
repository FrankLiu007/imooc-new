using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public abstract class RecordItem
    {
        public abstract void InitialReplay();
        public abstract void Replay(StrokeCollection pageStrokes);
        public TimeSpan Time { set; get; }
        public List<TimeSpan> TimePoints { set; get; }
        public List<int> CountList { set; get; }
    }

    public class StrokesAddedOrRemovedRI : RecordItem
    {
        private StrokeCollection _added, _removed;
        //private DispatcherTimer timer;
        //private DateTime lastTime;

        public StrokesAddedOrRemovedRI(StrokeCollection added,
            StrokeCollection removed, TimeSpan time)
        {
            _added = added;
            _removed = removed;
            Time = time;
        }

        public override void InitialReplay()
        {
            //if (TimePoints != null)
            //{

            //}
        }

        public override void Replay(StrokeCollection pageStrokes)
        {
            if (TimePoints != null)
            {
                var tempStroke = _added[0].Clone();
                for (int i = _added[0].StylusPoints.Count - 1; i > 0; i--)
                {
                    _added[0].StylusPoints.RemoveAt(i);
                }

                //int j = 0;
                //int start = 0;
                //lastTime = DateTime.Now;
                //timer = new DispatcherTimer();
                //timer.Interval = new TimeSpan(0, 0, 0, 0, 2);
                //timer.Tick += (sender, e) =>
                //{
                //    if (j < TimePoints.Count)
                //    {
                //        if (DateTime.Now - lastTime >= TimePoints[j])
                //        {
                //            if (CountList == null)
                //            {
                //                _added[0].StylusPoints.Add(tempStroke.StylusPoints[j]);
                //            }
                //            else
                //            {
                //                for (int i = start; i < CountList[j]; i++)
                //                {
                //                    _added[0].StylusPoints.Add(tempStroke.StylusPoints[i]);
                //                }
                //                start = CountList[j];
                //            }

                //            j++;
                //        }
                //    }
                //    else
                //    {
                //        timer.Stop();
                //    }
                //};

                pageStrokes.Add(_added);
                new Thread(() =>
                {
                    if (CountList == null)
                    {
                        for (int i = 0; i < TimePoints.Count; i++)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _added[0].StylusPoints.Add(tempStroke.StylusPoints[i]);

                            }));
                            Thread.Sleep(TimePoints[i]);
                        }
                    }
                    else if (CountList!=null)
                    {
                        int index = 1;
                        for (int i = 1; i < TimePoints.Count; i++)
                        {
                            for (int j = 0; j < CountList[i]; j++)
                            {
                                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    _added[0].StylusPoints.Add(tempStroke.StylusPoints[index]);
                                    index += 1;

                                }));
                            }
                            Thread.Sleep(TimePoints[i]);
                        }
                    }
                    
                }).Start();

                //timer.Start();
                //addStylusPoints(tempStroke.StylusPoints);
            }
            else
            {
                pageStrokes.Add(_added);
                pageStrokes.Remove(_removed);
            }

        }

        //private async void addStylusPoints(StylusPointCollection source)
        //{
        //    await Task.Run(new Action(() =>
        //    {
        //        for (int i = 0; i < TimePoints.Count; i++)
        //        {
        //            _added[0].StylusPoints.Add(source[i]);
        //            Thread.Sleep(TimePoints[i]);
        //        }
        //    }));
        //}
    }

    public class SelectionMovedOrResizedRI : RecordItem
    {
        StrokeCollection _selection;
        Rect _newrect, _oldrect;

        public SelectionMovedOrResizedRI(StrokeCollection selection,
            Rect newrect, Rect oldrect, TimeSpan time)
        {
            _selection = selection;
            _newrect = newrect;
            _oldrect = oldrect;
            Time = time;
        }

        public override void InitialReplay()
        {
            Matrix m = GetTransformFromRectToRect(_newrect, _oldrect);
            _selection.Transform(m, false);
        }

        public override void Replay(StrokeCollection pageStrokes)
        {
            Matrix m = GetTransformFromRectToRect(_oldrect, _newrect);
            _selection.Transform(m, false);
        }

        public Matrix GetTransformFromRectToRect(Rect src, Rect dst)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-src.X, -src.Y);
            m.Scale(dst.Width / src.Width, dst.Height / src.Height);
            m.Translate(+dst.X, +dst.Y);
            return m;
        }
    }
}
