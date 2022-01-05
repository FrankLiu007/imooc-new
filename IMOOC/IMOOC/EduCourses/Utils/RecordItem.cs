using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace IMOOC.EduCourses
{
    public sealed class AddTime
    {
        public TimeSpan Time { private set; get; }
        public List<TimeSpan> TimePoints { private set; get; }
        public List<int> Counts { private set; get; }

        public AddTime(List<TimeSpan> timePoints, List<int> counts)
        {
            TimePoints = timePoints;
            Counts = counts;
            TimeSpan time = new TimeSpan();
            for (int i = 0; i < timePoints.Count; i++)
            {
                time = time + timePoints[i];
            }
            Time = time;
        }

        public AddTime(BinaryReader br)
        {
            var timePointsCount = br.ReadInt32();
            TimePoints = new List<TimeSpan>();
            for (int i = 0; i < timePointsCount; i++)
            {
                TimePoints.Add(TimeSpan.FromMilliseconds(br.ReadDouble()));
            }
            var isCountsExist = br.ReadInt32();
            if (isCountsExist==1)
            {
                var countsCount = br.ReadInt32();
                Counts = new List<int>();
                for (int i = 0; i < countsCount; i++)
                {
                    Counts.Add(br.ReadInt32());
                }
            }
            
            Time = TimeSpan.FromMilliseconds(br.ReadDouble());
        }

        public void Save(BinaryWriter bw)
        {
            bw.Write(TimePoints.Count);
            foreach (var item in TimePoints)
            {
                bw.Write(item.TotalMilliseconds);
            }
            if (Counts==null)
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(1);
                bw.Write(Counts.Count);
                foreach (var item in Counts)
                {
                    bw.Write(item);
                }
            }
            
            bw.Write(Time.TotalMilliseconds);
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine(TimePoints.Count);
            foreach (var item in TimePoints)
            {
                sw.WriteLine(item.TotalMilliseconds);
            }
            if (Counts == null)
            {
                sw.WriteLine(0);
            }
            else
            {
                sw.WriteLine(1);
                sw.WriteLine(Counts.Count);
                foreach (var item in Counts)
                {
                    sw.WriteLine(item);
                }
            }

            sw.WriteLine(Time.TotalMilliseconds);
        }
    }

    public abstract class RecordItem
    {
        protected int _pageIndex;

        public abstract void RollBack();
        public abstract void Replay(InkCanvasPage page);
        public abstract void Save(BinaryWriter bw);
        public abstract void Save(StreamWriter sw);
    }

    public sealed class StrokesAddedOrRemovedRI : RecordItem
    {
        private int[] _addedIndex, _removedIndex;
        private StrokeCollection _added, _removed;
        private StrokeCollection _pageStrokes;
        public AddTime times;

        public StrokesAddedOrRemovedRI(int[] addedIndex, int[] removedIndex, int pageIndex)
        {
            _addedIndex = addedIndex;
            _removedIndex = removedIndex;
            _pageIndex = pageIndex;
            //Time = time;
        }

        public StrokesAddedOrRemovedRI(List<InkCanvasPage> addedItems, BinaryReader br)
        {
            _pageIndex = br.ReadInt32();
            var addedCount = br.ReadInt32();
            _added = new StrokeCollection();
            for (int j = 0; j < addedCount; j++)
            {
                _added.Add(addedItems[_pageIndex].Strokes[br.ReadInt32()]);
            }
            var removedCount = br.ReadInt32();
            _removed = new StrokeCollection();
            for (int j = 0; j < removedCount; j++)
            {
                _removed.Add(addedItems[_pageIndex].Strokes[br.ReadInt32()]);
            }
            var isTimeExist = br.ReadInt32();
            if (isTimeExist == 1)
            {
                times = new AddTime(br);
            }
        }

        public StrokesAddedOrRemovedRI(List<StrokeCollection> addedItems, BinaryReader br)
        {
            _pageIndex = br.ReadInt32();
            var addedCount = br.ReadInt32();
            _added = new StrokeCollection();
            for (int j = 0; j < addedCount; j++)
            {
                _added.Add(addedItems[_pageIndex][br.ReadInt32()]);
            }
            var removedCount = br.ReadInt32();
            _removed = new StrokeCollection();
            for (int j = 0; j < removedCount; j++)
            {
                _removed.Add(addedItems[_pageIndex][br.ReadInt32()]);
            }
            var isTimeExist = br.ReadInt32();
            if (isTimeExist == 1)
            {
                times = new AddTime(br);
            }
        }

        public override void RollBack()
        {
            _pageStrokes.Add(_removed);
            _pageStrokes.Remove(_added);
        }

        public override void Replay(InkCanvasPage page)
        {
            if (times != null)//add by writing
            {
                var tempStroke = _added[0].Clone();
                for (int i = _added[0].StylusPoints.Count - 1; i > 0; i--)
                {
                    _added[0].StylusPoints.RemoveAt(i);
                }

                page.Strokes.Add(_added);
                new Thread(() =>
                {
                    if (times.Counts.Count > 0)//add by writing with stylus
                    {
                        int index = 0;
                        for (int i = 1; i < times.TimePoints.Count; i++)
                        {
                            for (int j = 0; j < times.Counts[i]; j++)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    _added[0].StylusPoints.Add(tempStroke.StylusPoints[index]);
                                    index += 1;

                                }));
                            }
                            Thread.Sleep(times.TimePoints[i]);
                        }
                    }
                    else//add by writing with mouse
                    {

                        for (int i = 1; i < times.TimePoints.Count - 2; i++)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                _added[0].StylusPoints.Add(tempStroke.StylusPoints[i]);

                            }));
                            Thread.Sleep(times.TimePoints[i]);
                        }
                    }

                }).Start();

            }
            else//add by paste
            {
                page.Strokes.Add(_added);
                page.Strokes.Remove(_removed);
            }
            _pageStrokes = page.Strokes;
        }

        public void pptReplay(StrokeCollection strokes)
        {
            strokes.Add(_added);
            strokes.Remove(_removed);
            _pageStrokes = strokes;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write("_pageIndex");
            bw.Write(_pageIndex);
            bw.Write("_addedIndex.Length");
            bw.Write(_addedIndex.Length);
            foreach (var item in _addedIndex)
            {
                bw.Write(item);
            }
            bw.Write(_removedIndex.Length);
            foreach (var item in _removedIndex)
            {
                bw.Write(item);
            }
            if (times == null)
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(1);
                times.Save(bw);
            }

        }

        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(_pageIndex);
            sw.WriteLine(_addedIndex.Length);
            foreach (var item in _addedIndex)
            {
                sw.WriteLine(item);
            }
            sw.WriteLine(_removedIndex.Length);
            foreach (var item in _removedIndex)
            {
                sw.WriteLine(item);
            }
            if (times == null)
            {
                sw.WriteLine(0);
            }
            else
            {
                sw.WriteLine(1);
                times.Save(sw);
            }
        }
    }

    public sealed class AllChildAddedOrRemovedRI : RecordItem
    {
        private int[] _addedIndex, _removedIndex;
        private InkCanvasChildCollection _added, _removed;
        private InkCanvasChildCollection _pageAllChild;

        public AllChildAddedOrRemovedRI(int[] addedIndex, int[] removedIndex, int pageIndex)
        {
            _addedIndex = addedIndex;
            _removedIndex = removedIndex;
            _pageIndex = pageIndex;
            //Time = time;
        }

        public AllChildAddedOrRemovedRI(List<InkCanvasPage> addedItems, BinaryReader br)
        {
            _pageIndex = br.ReadInt32();
            var addedCount = br.ReadInt32();
            _added = new InkCanvasChildCollection();
            for (int i = 0; i < addedCount; i++)
            {
                _added.Add(addedItems[_pageIndex].AllChild[br.ReadInt32()]);
            }
            var removedCount = br.ReadInt32();
            _removed = new InkCanvasChildCollection();
            for (int i = 0; i < removedCount; i++)
            {
                _removed.Add(addedItems[_pageIndex].AllChild[br.ReadInt32()]);
            }
        }

        public override void RollBack()
        {
            _pageAllChild.Add(_removed);
            _pageAllChild.Remove(_added);
        }

        public override void Replay(InkCanvasPage page)
        {
            page.AllChild.Add(_added);
            page.AllChild.Remove(_removed);
            _pageAllChild = page.AllChild;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(_pageIndex);
            bw.Write(_addedIndex.Length);
            foreach (var item in _addedIndex)
            {
                bw.Write(item);
            }
            bw.Write(_removedIndex.Length);
            foreach (var item in _removedIndex)
            {
                bw.Write(item);
            }
        }

        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(_pageIndex);
            sw.WriteLine(_addedIndex.Length);
            foreach (var item in _addedIndex)
            {
                sw.WriteLine(item);
            }
            sw.WriteLine(_removedIndex.Length);
            foreach (var item in _removedIndex)
            {
                sw.WriteLine(item);
            }
        }
    }

    public sealed class SelectionMovedOrResizedRI : RecordItem
    {
        private int[] _selectedStrokesIndex, _selectedChildIndex;
        private StrokeCollection _selectedStrokes;
        private InkCanvasChildCollection _selectedChild;
        private Rect _newrect, _oldrect;

        public SelectionMovedOrResizedRI(int[] selectedStrokesIndex, int[] selectedChildIndex,
            Rect newrect, Rect oldrect, int pageIndex)
        {
            _selectedStrokesIndex = selectedStrokesIndex;
            _selectedChildIndex = selectedChildIndex;
            _newrect = newrect;
            _oldrect = oldrect;
            _pageIndex = pageIndex;
            //Time = time;
        }

        public SelectionMovedOrResizedRI(List<InkCanvasPage> addedItems, BinaryReader br)
        {
            _pageIndex = br.ReadInt32();
            var selectedStrokesCount = br.ReadInt32();
            _selectedStrokes = new StrokeCollection();
            for (int i = 0; i < selectedStrokesCount; i++)
            {
                _selectedStrokes.Add(addedItems[_pageIndex].Strokes[br.ReadInt32()]);
            }
            var selectedChildCount = br.ReadInt32();
            _selectedChild = new InkCanvasChildCollection();
            for (int i = 0; i < selectedChildCount; i++)
            {
                _selectedChild.Add(addedItems[_pageIndex].AllChild[br.ReadInt32()]);
            }
            _newrect = new Rect(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
            _oldrect = new Rect(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
        }

        public void initial()
        {
            Matrix m = GetTransformFromRectToRect(_newrect, _oldrect);
            _selectedStrokes.Transform(m, false);
        }

        public override void RollBack()
        {
            Matrix m = GetTransformFromRectToRect(_newrect, _oldrect);
            _selectedStrokes.Transform(m, false);
            _selectedChild.Transform(_newrect, _oldrect);
        }

        public override void Replay(InkCanvasPage page)
        {
            Matrix m = GetTransformFromRectToRect(_oldrect, _newrect);
            _selectedStrokes.Transform(m, false);
            _selectedChild.Transform(_oldrect, _newrect);
        }

        public static Matrix GetTransformFromRectToRect(Rect src, Rect dst)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-src.X, -src.Y);
            m.Scale(dst.Width / src.Width, dst.Height / src.Height);
            m.Translate(+dst.X, +dst.Y);
            return m;
        }

        public override void Save(BinaryWriter bw)
        {
            bw.Write(_pageIndex);
            bw.Write(_selectedStrokesIndex.Length);
            foreach (var item in _selectedStrokesIndex)
            {
                bw.Write(item);
            }
            bw.Write(_selectedChildIndex.Length);
            foreach (var item in _selectedChildIndex)
            {
                bw.Write(item);
            }
            bw.Write(_newrect.X);
            bw.Write(_newrect.Y);
            bw.Write(_newrect.Width);
            bw.Write(_newrect.Height);
            bw.Write(_oldrect.X);
            bw.Write(_oldrect.Y);
            bw.Write(_oldrect.Width);
            bw.Write(_oldrect.Height);
        }

        public override void Save(StreamWriter sw)
        {
            sw.WriteLine(_pageIndex);
            sw.WriteLine(_selectedStrokesIndex.Length);
            foreach (var item in _selectedStrokesIndex)
            {
                sw.WriteLine(item);
            }
            sw.WriteLine(_selectedChildIndex.Length);
            foreach (var item in _selectedChildIndex)
            {
                sw.WriteLine(item);
            }
            sw.WriteLine(_newrect.X);
            sw.WriteLine(_newrect.Y);
            sw.WriteLine(_newrect.Width);
            sw.WriteLine(_newrect.Height);
            sw.WriteLine(_oldrect.X);
            sw.WriteLine(_oldrect.Y);
            sw.WriteLine(_oldrect.Width);
            sw.WriteLine(_oldrect.Height);
        }
    }
}
