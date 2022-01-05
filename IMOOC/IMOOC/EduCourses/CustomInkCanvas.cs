using IMOOC.EduCourses.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IMOOC.EduCourses
{
    //CustomDynamicRenderer for Inkcanvas to avoiding 
    sealed class CustomDynamicRenderer : DynamicRenderer
    {
        private bool allowDraw;
        public bool AllowDraw
        {
            get { return allowDraw; }
            set { allowDraw = value; }
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints,
                                       Geometry geometry, Brush fillBrush)
        {
            if (!allowDraw) return;
            base.OnDraw(drawingContext, stylusPoints, geometry, fillBrush);

        }
    }

    public sealed class CustomInkCanvas : InkCanvas
    {
        #region dependencyProperty
        //public bool IsRecord
        //{
        //    get { return (bool)GetValue(isRecordProperty); }
        //    set { SetValue(isRecordProperty, value);
        //        if (value)
        //        {
        //            startTime = DateTime.Now;
        //        }
        //    }
        //}
        //// Using a DependencyProperty as the backing store for isRecord.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty isRecordProperty =
        //    DependencyProperty.Register("IsRecord", typeof(bool), typeof(CustomInkCanvas), new PropertyMetadata(false));

        //disable finger and mouse input 
        //CustomDynamicRenderer renderer;

        public bool IsStylusOnly
        {
            get { return (bool)GetValue(isStylusOnlyProperty); }
            set { SetValue(isStylusOnlyProperty, value); }
        }
        public static readonly DependencyProperty isStylusOnlyProperty =
            DependencyProperty.Register("IsStylusOnly", typeof(bool), typeof(CustomInkCanvas), new PropertyMetadata(true));



        //public Collection<AddTime> AddTimes
        //{
        //    get { return (Collection<AddTime>)GetValue(AddTimesProperty); }
        //    set { SetValue(AddTimesProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for AddTimesList.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty AddTimesProperty =
        //    DependencyProperty.Register("AddTimes", typeof(Collection<AddTime>), typeof(CustomInkCanvas), new PropertyMetadata(new Collection<AddTime>()));



        public Collection<StrokeAction> StrokeXes
        {
            get { return (Collection<StrokeAction>)GetValue(StrokeXesProperty); }
            set { SetValue(StrokeXesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeXes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeXesProperty =
            DependencyProperty.Register("StrokeXes", typeof(Collection<StrokeAction>), typeof(CustomInkCanvas), new PropertyMetadata(new Collection<StrokeAction>()));



        #endregion

        #region Collectionollection
        //Just a simple INotifyCollectionChanged collection
        public InkCanvasChildCollection AllChild
        {
            get { return (InkCanvasChildCollection)GetValue(AllChildProperty); }
            set { SetValue(AllChildProperty, value); }
        }


        public static readonly DependencyProperty AllChildProperty =
            DependencyProperty.Register("AllChild",
            typeof(InkCanvasChildCollection),
            typeof(CustomInkCanvas),
            new FrameworkPropertyMetadata(new InkCanvasChildCollection(),
            new PropertyChangedCallback(AllChildChanged)));



        private static void AllChildChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            //gets the instance that changed the "local" value
            var instance = sender as CustomInkCanvas;
            //the new collection that will be set
            InkCanvasChildCollection newCollection = args.NewValue as InkCanvasChildCollection;
            //the previous collection that was set
            InkCanvasChildCollection oldCollection = args.OldValue as InkCanvasChildCollection;

            if (oldCollection != null)
            {
                //removes the CollectionChangedEventHandler from the old collection
                oldCollection.CollectionChanged -= instance.AllChildChanged;
            }

            //clears all the previous children in the collection
            instance.Children.Clear();

            if (newCollection != null)
            {
                //adds all the children of the new collection
                foreach (InkCanvasChild item in newCollection)
                {
                    AddControl(item, instance);
                }

                //adds a new CollectionChangedEventHandler to the new collection
                newCollection.CollectionChanged += instance.AllChildChanged;
            }

        }

        //append when an Item in the collection is changed
        private void AllChildChanged(object sender, InkCanvasChildCollectionChangedEventArgs e)
        {
            //adds the new items in the children collection
            foreach (var item in e.Added)
            {
                AddControl(item);
            }
            int count = e.Removed.Count;
            while (count > 0)
            {
                Children.Remove(e.Removed[count - 1].UiEle);
                count--;
            }
        }


        private static void AddControl(InkCanvasChild child, InkCanvas parentControl)
        {
            //Image img = child.UiEle as Image;
            child.UiEle.DataContext = child;
            parentControl.Children.Add(child.UiEle);

            //binds to the control to the properties X and Y of the viewModel
            Binding XBinding = new Binding("X") { Source = child, Mode = BindingMode.TwoWay };

            Binding YBinding = new Binding("Y") { Source = child, Mode = BindingMode.TwoWay };

            child.UiEle.SetBinding(InkCanvas.LeftProperty, XBinding);
            child.UiEle.SetBinding(InkCanvas.TopProperty, YBinding);
        }

        private void AddControl(InkCanvasChild child)
        {
            AddControl(child, this);
        }
        #endregion

        #region Members
        //private List<TimeSpan> TimeSpans;
        //private List<int> indexes;
        private DateTime startTime;
        private DateTime lastTime;
        private List<Dot> dotList;
        private StrokeAction action;
        InkCanvasEditingMode lastEditingMode;
        bool isStylus;
        //bool temporarySelect = false;
        #endregion

        public CustomInkCanvas() : base()
        {
            AllChild.CollectionChanged += AllChildChanged;
            //renderer = new CustomDynamicRenderer();

            //this.DynamicRenderer = renderer;
            lastEditingMode = EditingMode;
            isStylus = false;

        }

        public void StartRecord()
        {
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 用于暂停录制之后，对录制时间进行补偿
        /// </summary>
        /// <param name="timeSpan">暂停的时间间隔</param>
        public void SetStartTime(TimeSpan timeSpan)
        {
            DateTime time = startTime.Add(timeSpan);
            startTime = time;
        }

        private bool IsInSelctionBound(Point p)
        {
            var Edge_Number = 16;
            var Point_X = p.X;
            var Point_Y = p.Y;
            
            var Rect_Xbottomright = GetSelectionBounds().BottomRight.X + Edge_Number;
            var Rect_Ybottomright = GetSelectionBounds().BottomRight.Y + Edge_Number;

            var Rect_Xtopleft = GetSelectionBounds().TopLeft.X - Edge_Number;
            var Rect_Ytopleft = GetSelectionBounds().TopLeft.Y - Edge_Number;

            if ((Point_X > Rect_Xtopleft) && (Point_X < Rect_Xbottomright) && (Point_Y < Rect_Ybottomright) && (Point_Y > Rect_Ytopleft))
            {
                return (true);
            }

            return (false);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {

            if ((DataContext as CoursesViewModel).CourseCtrl.IsBanshu == false
               && (DataContext as CoursesViewModel).CurrCourse.AllPPT.Count == 0)
            {
                e.Handled = true;
                return;
            }

            if (IsStylusOnly && !isStylus)
            {
                if (EditingMode != InkCanvasEditingMode.None)
                {
                    lastEditingMode = EditingMode;
                    EditingMode = InkCanvasEditingMode.None;
                }

            }

            base.OnPreviewMouseDown(e);

            //dotList = new List<Dot>();
            //action = GetAction("stroke");
            //var item = e.GetPosition(this);
            //dotList.Add(new Dot() { x = item.X.ToSF(), y = item.Y.ToSF(), radius = "0.5" });

        }

        //get the Inkcanvas EditingMode from the toolbar
        private InkCanvasEditingMode GetEditingModeFromBanshuToolbar(  )
        {
            InkCanvasEditingMode editingMode=InkCanvasEditingMode.None;
            BanshuTool banshuTool = (DataContext as CoursesViewModel).CourseCtrl.banshuTool;
            if (banshuTool.PencilStyle.IsChecked == true)
            {
                editingMode = InkCanvasEditingMode.Ink;
            }
            else if (banshuTool.HighlightStyle.IsChecked == true)
            {
                editingMode = InkCanvasEditingMode.Ink;
            }
            else if (banshuTool.EraserStyle.IsChecked == true)
            {
                editingMode = InkCanvasEditingMode.EraseByStroke;
            }
            else if (banshuTool.SelectStyle.IsChecked == true)
            {
                editingMode = InkCanvasEditingMode.Select;
            }
            return editingMode;
        }
        protected override void OnPreviewStylusButtonDown(StylusButtonEventArgs e)
        {
            //如果是手指触摸，则函数返回
            if( e.StylusDevice.TabletDevice.Type!=TabletDeviceType.Stylus)
            {
                return;
            }

            //if (temporarySelect && !(GetSelectionBounds().IsEmpty))
            //{
            //    if (!IsInSelctionBound(e.GetPosition(this)))
            //    {
            //        //EditingMode = lastEditingMode;
            //        EditingMode = GetEditingModeFromBanshuToolbar();
            //        temporarySelect = false;
            //    }
            //}
            //else
            //{
            //    //the barrel button on the stylus has been pressed
            //   var myStylusButton = e.StylusDevice.StylusButtons;
            //    if (myStylusButton[1].StylusButtonState == StylusButtonState.Down)
            //    {
            //        EditingMode = InkCanvasEditingMode.Select;
            //        temporarySelect = true;
            //    }
            //}
            base.OnPreviewStylusButtonDown(e);
        }

        protected override void OnPreviewStylusButtonUp(StylusButtonEventArgs e)
        {
            base.OnPreviewStylusButtonUp(e);

            //if (temporarySelect)
            //{
            //    EditingMode = GetEditingModeFromBanshuToolbar();
            //    temporarySelect = false;
            //}
            ////if (temporarySelect && !(GetSelectionBounds().IsEmpty))
            ////{
            ////    if (!IsInSelctionBound(e.GetPosition(this)))
            ////    {
            ////        //EditingMode = lastEditingMode;
            ////        EditingMode = GetEditingModeFromBanshuToolbar();
            ////        temporarySelect = false;
            ////    }
            ////}
        }

        protected override void OnPreviewStylusDown(StylusDownEventArgs e)
        {
            if ((DataContext as CoursesViewModel).CourseCtrl.IsBanshu == false
               && (DataContext as CoursesViewModel).CurrCourse.AllPPT.Count == 0)
            {
                e.Handled = true;
                return;
            }
            var deviceType = Stylus.CurrentStylusDevice.TabletDevice.Type;

            if (IsStylusOnly && deviceType == TabletDeviceType.Touch)
            {
                //renderer.AllowDraw = false;
                if (EditingMode != InkCanvasEditingMode.None)
                {
                    lastEditingMode = EditingMode;
                    EditingMode = InkCanvasEditingMode.None;
                }
                return;
            }
            //renderer.AllowDraw = true;
            if (EditingMode == InkCanvasEditingMode.None)
            {
                EditingMode = lastEditingMode;
            }

            isStylus = true;
         
            base.OnPreviewStylusDown(e);
        }


        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            base.OnStylusDown(e);
  
            if (e.Inverted)
            {
                return;
            }
            if (EditingMode==InkCanvasEditingMode.Ink)
            {
                dotList = new List<Dot>();
                action = GetAction("stroke");
                if (DefaultDrawingAttributes.IsHighlighter)
                {
                    foreach (var item in e.GetStylusPoints(this))
                    {
                        dotList.Add(new Dot() { x = item.X, y = item.Y, radius = this.DefaultDrawingAttributes.Height });
                    }
                }
                else
                {
                    foreach (var item in e.GetStylusPoints(this))
                    {
                        dotList.Add(new Dot() { x = item.X, y = item.Y, radius = (this.DefaultDrawingAttributes.Height * item.PressureFactor) });
                    }
                }
                
            }
            

            //dotcount += e.GetStylusPoints(this).Count;
        }

        private StrokeAction GetAction(string type)
        {
            lastTime = DateTime.Now;
            var strokeAction = new Utils.StrokeAction();
            strokeAction.time = (lastTime - startTime).TotalMilliseconds;
            strokeAction.type = type;
            var opt = new Option();
            var color = DefaultDrawingAttributes.Color;
            opt.color = string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, color.ScA);
            if (DefaultDrawingAttributes.IsHighlighter)
            {
                opt.dotType = "square";
            }
            else
            {
                opt.dotType = "round";
            }
            strokeAction.options = opt;
            return strokeAction;
        }

        protected override void OnStylusMove(StylusEventArgs e)
        {
            base.OnStylusMove(e);

            if (e.Inverted)
            {
                return;
            }

            if (dotList == null)
            {
                return;
            }
            if (EditingMode!=InkCanvasEditingMode.Ink)
            {
                return;
            }

            var duration = (DateTime.Now - lastTime).TotalMilliseconds;
            if (duration > 400)
            {
                action.duration = duration;
                if (action.type == "stroke")
                {
                    action.type = "startStroke";
                }
                action.dots = dotList;
                StrokeXes.Add(action);

                dotList = new List<Dot>();
                if (DefaultDrawingAttributes.IsHighlighter)
                {
                    foreach (var item in e.GetStylusPoints(this))
                    {
                        dotList.Add(new Dot() { x = item.X, y = item.Y, radius = this.DefaultDrawingAttributes.Height });
                    }
                }
                else
                {
                    foreach (var item in e.GetStylusPoints(this))
                    {
                        dotList.Add(new Dot() { x = item.X, y = item.Y, radius = (this.DefaultDrawingAttributes.Height * item.PressureFactor) });
                    }
                }                
                action = GetAction("stroking");
            }

            else
            {
                if (DefaultDrawingAttributes.IsHighlighter)
                {
                    foreach (var item in e.GetStylusPoints(this))
                    {
                        dotList.Add(new Dot() { x = item.X, y = item.Y, radius = this.DefaultDrawingAttributes.Height });
                    }
                }
                else
                {
                    foreach (var item in e.GetStylusPoints(this))
                    {
                        dotList.Add(new Dot() { x = item.X, y = item.Y, radius = (this.DefaultDrawingAttributes.Height * item.PressureFactor) });
                    }
                }              
            }
            
        }

        protected override void OnStylusUp(StylusEventArgs e)
        {
            base.OnStylusUp(e);
            isStylus = false;
        }

        //protected override void OnPreviewTouchDown(TouchEventArgs e)
        //{
        //    if (IsStylusOnly)
        //    {
        //        EditingMode = InkCanvasEditingMode.None;
        //    }
        //    base.OnPreviewTouchDown(e);
        //}

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Don't collect points unless the left mouse button
            // is down.
            // If a stylus generated this event, return.
            if (EditingMode==InkCanvasEditingMode.EraseByStroke)
            {
                return;
            }

            if (e.StylusDevice != null)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Released ||
                EditingMode != InkCanvasEditingMode.Ink)
            {
                return;
            }

            if ((DataContext as CoursesViewModel).isOpenHtml)
            {
                (DataContext as CoursesViewModel).isOpenHtml = false;
                return;
            }

            if (EditingMode == InkCanvasEditingMode.Ink)
            {
                var point1 = e.GetPosition(this);
                if (dotList == null)
                {
                    dotList = new List<Dot>();
                    action = GetAction("stroke");                    
                    if (DefaultDrawingAttributes.IsHighlighter)
                    {
                        dotList.Add(new Dot() { x = point1.X, y = point1.Y, radius = this.DefaultDrawingAttributes.Height });
                    }
                    else
                    {
                        dotList.Add(new Dot() { x = point1.X, y = point1.Y, radius = (this.DefaultDrawingAttributes.Height * 0.5) });
                    }
                }
                else
                {
                    var duration = (DateTime.Now - lastTime).TotalMilliseconds;
                    if (duration > 400)
                    {
                        action.duration = duration;
                        if (action.type == "stroke")
                        {
                            action.type = "startStroke";
                        }
                        action.dots = dotList;
                        StrokeXes.Add(action);
                        dotList = new List<Dot>();
                        if (DefaultDrawingAttributes.IsHighlighter)
                        {
                            dotList.Add(new Dot() { x = point1.X, y = point1.Y, radius = this.DefaultDrawingAttributes.Height });
                        }
                        else
                        {
                            dotList.Add(new Dot() { x = point1.X, y = point1.Y, radius = (this.DefaultDrawingAttributes.Height * 0.5) });
                        }
                        action = GetAction("stroking");
                    }

                    else
                    {
                        if (DefaultDrawingAttributes.IsHighlighter)
                        {
                            dotList.Add(new Dot() { x = point1.X, y = point1.Y, radius = this.DefaultDrawingAttributes.Height });
                        }
                        else
                        {
                            dotList.Add(new Dot() { x = point1.X, y = point1.Y, radius = (this.DefaultDrawingAttributes.Height * 0.5) });
                        }
                    }
                }
            }
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {           
            base.OnStrokeCollected(e);
            if (action == null)
            {
                return;
            }
            action.duration = (DateTime.Now - lastTime).TotalMilliseconds;
            if (action.type == "stroking")
            {
                action.type = "endStroke";
            }
            action.dots = dotList;
            StrokeXes.Add(action);
            dotList = null;
            var aa = GetSelectedStrokes();
        }

    }

    public sealed class InkCanvasChildCollection : Collection<InkCanvasChild>
    {
        public event EventHandler<InkCanvasChildCollectionChangedEventArgs> CollectionChanged;

        public InkCanvasChildCollection(BinaryReader br, string directory)
        {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var inkCanvaschild = new InkCanvasChild();
                var image = new Image() { Source = new BitmapImage(new Uri(directory + br.ReadString())) };
                image.Width = br.ReadDouble();
                image.Height = br.ReadDouble();
                image.Stretch = Stretch.Fill;
                inkCanvaschild.X = br.ReadDouble();
                inkCanvaschild.Y = br.ReadDouble();
                inkCanvaschild.UiEle = image;
                Add(inkCanvaschild);
            }

        }

        public InkCanvasChildCollection()
        {

        }

        protected override void InsertItem(int index, InkCanvasChild item)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new InkCanvasChildCollectionChangedEventArgs(item, null));
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new InkCanvasChildCollectionChangedEventArgs(null, this[index]));
            }
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new InkCanvasChildCollectionChangedEventArgs(null, this));
            }
            base.ClearItems();
        }

        public void Add(InkCanvasChildCollection childs)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                Add(childs[i]);
            }
        }

        public void Remove(InkCanvasChildCollection childs)
        {
            int childsCount = childs.Count;
            while (childsCount > 0)
            {
                Remove(childs[childsCount - 1]);
                childsCount--;
            }
        }

        public void Save(BinaryWriter bw)
        {
            bw.Write(this.Count);
            foreach (var item in this)
            {
                var image = item.UiEle as Image;
                bw.Write(Path.GetFileName((image.Source as BitmapImage).UriSource.LocalPath));
                bw.Write(image.Width);
                bw.Write(image.Height);
                bw.Write(item.X);
                bw.Write(item.Y);
            }
        }

        public void Save(StreamWriter sw)
        {
            sw.WriteLine("AllChlid.Count");
            sw.WriteLine(Count);
            foreach (var item in this)
            {
                sw.WriteLine("Chlid.Path");
                sw.WriteLine(Path.GetFileName(((item.UiEle as Image).Source as BitmapImage).UriSource.LocalPath));
            }
        }
    }

    public sealed class InkCanvasChildCollectionChangedEventArgs : EventArgs
    {
        public InkCanvasChildCollection Added { get; private set; }
        public InkCanvasChildCollection Removed { get; private set; }

        public InkCanvasChildCollectionChangedEventArgs(InkCanvasChild added, InkCanvasChild removed)
        {
            Added = new InkCanvasChildCollection();
            if (added != null)
            {
                Added.Add(added);
            }
            Removed = new InkCanvasChildCollection();
            if (removed != null)
            {
                Removed.Add(removed);
            }

        }

        public InkCanvasChildCollectionChangedEventArgs(InkCanvasChildCollection added, InkCanvasChildCollection removed)
        {
            Added = new InkCanvasChildCollection();
            if (added != null)
            {
                Added.Add(added);
            }
            Removed = new InkCanvasChildCollection();
            if (removed != null)
            {
                Removed.Add(removed);
            }
        }
    }

    public sealed class InkCanvasChild : NotificationObject
    {
        public InkCanvasChild()
        {
            x = 0;
            y = 0;
            uiEle = null;
        }

        private double x;
        public double X
        {
            get { return x; }
            set { RaisePropertyChanged(ref x, value, "X"); }
        }

        private double y;
        public double Y
        {
            get { return y; }
            set { RaisePropertyChanged(ref y, value, "Y"); }
        }

        private FrameworkElement uiEle;
        public FrameworkElement UiEle
        {
            get { return uiEle; }
            set { RaisePropertyChanged(ref uiEle, value, "UiEle"); }
        }

        public int index { get; set; }
        public int id { get; set; }
    }

    public static class Extentions
    {
        public static InkCanvasChildCollection Clone(this InkCanvasChildCollection childs)
        {
            var result = new InkCanvasChildCollection();
            foreach (var item in childs)
            {
                var oldImage = item.UiEle as Image;
                if (oldImage != null)
                {
                    ImageSource imageSource = new BitmapImage(new Uri((oldImage.Source as BitmapImage).UriSource.LocalPath));
                    Image newImage = new Image();
                    newImage.Source = imageSource;
                    newImage.Stretch = Stretch.Fill;
                    newImage.Width = oldImage.Width;
                    newImage.Height = oldImage.Height;
                    var newitem = new InkCanvasChild() { X = item.X + 30, Y = item.Y, UiEle = newImage };
                    result.Add(newitem);
                    continue;
                }
                var oldCanvas = item.UiEle as Canvas;
                if (oldCanvas != null)
                {
                    Canvas newCanvas = new Canvas();
                    newCanvas.Width = oldCanvas.ActualWidth;
                    newCanvas.Height = oldCanvas.ActualHeight;
                    foreach (var child in oldCanvas.Children)
                    {
                        var line = child as System.Windows.Shapes.Line;
                        if (line != null)
                        {
                            var newline = new System.Windows.Shapes.Line() { X1 = line.X1, Y1 = line.Y1, X2 = line.X2, Y2 = line.Y2, Stroke = line.Stroke };
                            newCanvas.Children.Add(newline);
                        }

                    }
                    var newitem = new InkCanvasChild() { X = item.X + 30, Y = item.Y, UiEle = newCanvas };
                    result.Add(newitem);
                    continue;
                }

            }
            return result;
        }

        public static void Transform(this InkCanvasChildCollection selectedChild, Rect src, Rect dst)
        {
            for (int i = 0; i < selectedChild.Count; i++)
            {                
                var element = selectedChild[i].UiEle;

                var scaleX = dst.Width / src.Width;
                var scaleY = dst.Height / src.Height;
                selectedChild[i].X = dst.Left + (selectedChild[i].X - src.Left) * scaleX;
                selectedChild[i].Y = dst.Top + (selectedChild[i].Y - src.Top) * scaleY;
                element.Width = element.ActualWidth * scaleX;
                element.Height = element.ActualHeight * scaleY;

                if (scaleX != 1 || scaleY != 1)
                {
                    var canvas = element as Canvas;
                    if (canvas != null)
                    {
                        foreach (var item in canvas.Children)
                        {
                            var line = item as System.Windows.Shapes.Line;
                            if (line != null)
                            {
                                line.X1 *= scaleX;
                                line.X2 *= scaleX;
                                line.Y1 *= scaleY;
                                line.Y2 *= scaleY;
                                continue;
                            }
                            var ellipse = item as System.Windows.Shapes.Ellipse;
                            if (ellipse != null)
                            {
                                var left = (double)ellipse.GetValue(Canvas.LeftProperty);
                                var top = (double)ellipse.GetValue(Canvas.TopProperty);
                                ellipse.SetValue(Canvas.LeftProperty, left * scaleX);
                                ellipse.SetValue(Canvas.TopProperty, top * scaleY);
                                ellipse.Width *= scaleX;
                                ellipse.Height *= scaleY;
                                continue;
                            }
                        }
                    }
                }

            }
        }

        public static string ToSF(this double d)
        {
            return string.Format("{0:0.0}", d);
        }

        public static string ToSF(this float f)
        {
            return string.Format("{0:0.0}", f);
        }

        public static void Save(this StrokeCollection strokes, StreamWriter sw)
        {
            sw.WriteLine("Strokes.Count");
            sw.WriteLine(strokes.Count);
            int i = 1;
            foreach (var item in strokes)
            {
                sw.WriteLine("Strokes" + i.ToString());
                item.Save(sw);
                i++;
            }
        }

        public static void Save(this Stroke stroke, StreamWriter sw)
        {
            sw.WriteLine(stroke.DrawingAttributes.Color);
            sw.WriteLine(stroke.StylusPoints.Count);
            foreach (var item in stroke.StylusPoints)
            {
                sw.WriteLine(item.X);
                sw.WriteLine(item.Y);
                sw.WriteLine(item.PressureFactor);
            }
        }
    }

}
