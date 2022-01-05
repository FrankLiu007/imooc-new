using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System;
using System.Windows.Ink;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    using IMOOC;

    public partial class WhiteBoard : UserControl
    {

        //enum ToolTypes { Pencil, Line, DashedLine, Rectangle, Circle, Ellipse, HighlightPen, Pen, Eraser, Hand, Arrow, SelectRectangle };
        public object parentObject;


        private bool highLighted;
        private DrawingAttributes inkDA, highlightDA;
        //private DrawingAttributes defaultDA;
        private StrokeCollection copiedStrokes;

        WhiteBoardViewModel viewModel;

        public WhiteBoard( )
        {
            viewModel = new WhiteBoardViewModel();
          //  viewModel.ConnectSocket = socket;
            DataContext = viewModel;
            InitializeComponent();
            inkDA = initPen();
            highlightDA = initHighlighter();
            myInkCanvas.DefaultDrawingAttributes = inkDA;
            myInkCanvas.DataContext = viewModel;
            myInkCanvas.StrokeCollected += (sender, e) => copiedStrokes = null;
         //   myInkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private DrawingAttributes initHighlighter()
        {
            DrawingAttributes highlightDA = new DrawingAttributes();
            highlightDA.IsHighlighter = false;
            highlightDA.Color = Color.FromArgb(128, Colors.Yellow.R, Colors.Yellow.G, Colors.Yellow.B);
            highlightDA.Width = 30;
            highlightDA.Height = 30;
            highlightDA.IgnorePressure = true;
            highlightDA.StylusTip = StylusTip.Rectangle;
            return highlightDA;
        }

        private DrawingAttributes initPen()
        {
            DrawingAttributes inkDA = new DrawingAttributes();
            inkDA.Color = Colors.Black;
            inkDA.Width = 2;
            inkDA.Height = 2;
            inkDA.IsHighlighter = false;
            return inkDA;
        }

        private void arrowLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PageViewerContaner.IsOpen = !PageViewerContaner.IsOpen;
            if (arrowLeft.RenderTransform == Transform.Identity)
            {
                arrowLeft.RenderTransform = new TranslateTransform(300, 0);
            }
            else
            {
                arrowLeft.RenderTransform = Transform.Identity;
            }
        }

        private void arrowRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //showToolViewer = !showToolViewer;

        }
        //------------------set tools-------------------------------

        private void ButtonHilight_Click(object sender, RoutedEventArgs e)
        {

            ButtonColor1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#68F3FF"));
            ButtonColor2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86FF56"));
            ButtonColor3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF952"));
            ButtonColor4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DC5"));
            //myInkCanvas.DefaultDrawingAttributes.IsHighlighter = true;
            highLighted = true;
            myInkCanvas.DefaultDrawingAttributes = highlightDA;
            myInkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            ChangedCheckedColor();
        }

        private void ChangedCheckedColor()
        {
            for (int i = 0; i < colorSp.Children.Count; i++)
            {
                var btn = colorSp.Children[i] as RadioButton;
                if (((SolidColorBrush)btn.Background).Color == myInkCanvas.DefaultDrawingAttributes.Color)
                {
                    btn.IsChecked = true;
                    for (int j = 0; j < colorWp.Children.Count; j++)
                    {
                        var btn1 = colorWp.Children[j] as RadioButton;
                        if (btn1.IsChecked == true)
                        {
                            btn1.IsChecked = false;
                            return;
                        }
                    }
                }
            }
            for (int i = 0; i < colorWp.Children.Count; i++)
            {
                var btn = colorWp.Children[i] as RadioButton;
                if (((SolidColorBrush)btn.Background).Color == myInkCanvas.DefaultDrawingAttributes.Color)
                {
                    btn.IsChecked = true;
                    for (int j = 0; j < colorSp.Children.Count; j++)
                    {
                        var btn1 = colorSp.Children[j] as RadioButton;
                        if (btn1.IsChecked == true)
                        {
                            btn1.IsChecked = false;
                            return;
                        }
                    }
                }
            }
        }

        private void ButtonPencil_Click(object sender, RoutedEventArgs e)
        {
            //----set the content of the four color button-------
            ButtonColor1.Background = Brushes.Black;
            ButtonColor2.Background = Brushes.Blue;
            ButtonColor3.Background = Brushes.Yellow;
            ButtonColor4.Background = Brushes.Coral;
            //--------------------
            highLighted = false;
            myInkCanvas.DefaultDrawingAttributes = inkDA;
            myInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            //myInkCanvas.IsHitTestVisible = true;
            //myInkCanvas.Children.

            ChangedCheckedColor();
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (highLighted)
            {
                if (myInkCanvas.DefaultDrawingAttributes.Width > 100 || myInkCanvas.DefaultDrawingAttributes.Height > 100)
                {
                    MessageBox.Show("笔的宽度已经最大！");
                    return;
                }
                myInkCanvas.DefaultDrawingAttributes.Height += 5;
                myInkCanvas.DefaultDrawingAttributes.Width += 2.5;
                lineWidthPoly.StrokeThickness += 2.5;
            }
            else
            {
                if (myInkCanvas.DefaultDrawingAttributes.Width > 100)
                {
                    MessageBox.Show("笔的宽度已经最大！");
                    return;
                }
                myInkCanvas.DefaultDrawingAttributes.Height += 2;
                myInkCanvas.DefaultDrawingAttributes.Width += 2;
                lineWidthPoly.StrokeThickness += 2;
            }


        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (highLighted)
            {
                if (myInkCanvas.DefaultDrawingAttributes.Width < 5 || myInkCanvas.DefaultDrawingAttributes.Height < 5)
                {
                    MessageBox.Show("笔的宽度已经最小！");
                    return;
                }
                myInkCanvas.DefaultDrawingAttributes.Height -= 5;
                myInkCanvas.DefaultDrawingAttributes.Width -= 2.5;
                lineWidthPoly.StrokeThickness -= 2.5;
            }
            else
            {
                if (myInkCanvas.DefaultDrawingAttributes.Width < 3)
                {
                    MessageBox.Show("笔的宽度已经最小！");
                    return;
                }
                myInkCanvas.DefaultDrawingAttributes.Width -= 2;
                myInkCanvas.DefaultDrawingAttributes.Height -= 2;
                lineWidthPoly.StrokeThickness -= 2;
            }

        }

        private void ButtonSelectArea_Click(object sender, RoutedEventArgs e)
        {
            myInkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void ButtonLineEraser_Click(object sender, RoutedEventArgs e)
        {
            myInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        //----------------------------------
        private void myInkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (myInkCanvas.EditingMode == InkCanvasEditingMode.Select) return;
            //if (!mouseInputEnabled)                e.Handled = true;
            
        }

        private void myInkCanvas_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            //textBox.Text = e.StylusDevice.TabletDevice.Type.ToString();
            if (e.StylusDevice.TabletDevice.Type != TabletDeviceType.Stylus)
            {
                e.Handled = true;
            }

        }

        private void myInkCanvas_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }

        //-------------------------------------------------------
        private void ButtonCutSelection_Click(object sender, RoutedEventArgs e)
        {
            //myInkCanvas.CutSelection();
            copiedStrokes = myInkCanvas.GetSelectedStrokes();
            myInkCanvas.Strokes.Remove(copiedStrokes);
        }

        private void ButtonPasteSelection_Click(object sender, RoutedEventArgs e)
        {

            //myInkCanvas.Paste(p1);
            if (copiedStrokes!=null)
            {
                Point p1 = new Point();
                p1.X = myInkCanvas.ActualWidth / 3;
                p1.Y = myInkCanvas.ActualHeight / 3;
                Rect bound = copiedStrokes.GetBounds();
                Rect newBound = new Rect(p1, bound.Size);
                Matrix m = GetTransformFromRectToRect(bound, newBound);
                var strokes = copiedStrokes.Clone();
                strokes.Transform(m, false);
                myInkCanvas.Strokes.Add(strokes);
            }
            
        }

        private Matrix GetTransformFromRectToRect(Rect src, Rect dst)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-src.X, -src.Y);
            m.Scale(dst.Width / src.Width, dst.Height / src.Height);
            m.Translate(+dst.X, +dst.Y);
            return m;
        }

        private void ButtonCopySelection_Click(object sender, RoutedEventArgs e)
        {
            //myInkCanvas.CopySelection();
            copiedStrokes = myInkCanvas.GetSelectedStrokes();
        }

        private void ButtonClearAll_Click(object sender, RoutedEventArgs e)
        {
            myInkCanvas.Strokes.Clear();
        }

        private void Grid_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }

        //-----------------undo,redo---------------------------------
        
        void MyInkCanvas_SelectionMovingOrResizing(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            viewModel.SelectionMovingOrResizing(myInkCanvas.GetSelectedStrokes(), e);
        }

        void MyInkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.editingOperationCountList[viewModel.CurrPageIndex]++;
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (viewModel.cmdStackList[viewModel.CurrPageIndex].CanUndo)
                viewModel.cmdStackList[viewModel.CurrPageIndex].Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            if (viewModel.cmdStackList[viewModel.CurrPageIndex].CanRedo)
                viewModel.cmdStackList[viewModel.CurrPageIndex].Redo();
        }

        //--------------------------------------------------------------
    

        private void OpenImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = ".jpg|*.jpg|.png|*.png";
            if (ofd.ShowDialog() == true)
            {
                StylusPointCollection pts = new StylusPointCollection();
                pts.Add(new StylusPoint(400, 100)); // LeftTopPoint
                pts.Add(new StylusPoint(600, 100));
                pts.Add(new StylusPoint(400, 300));
                pts.Add(new StylusPoint(600, 300));

                ImageSource image = new BitmapImage(new Uri(ofd.FileName));
                Image img=new Image();
                img.Source=image;
                myInkCanvas.Children.Add(img);
                //ImageStroke imageStroke = new ImageStroke(pts, image);
               // myInkCanvas.Strokes.Add(imageStroke);
            }
        }
        public void AddImageStroke(ImageSource image)
        {

            Image img = new Image();
            img.Source = image;
            myInkCanvas.Children.Add(img);
            if (img.Width > myInkCanvas.ActualWidth / 3 || img.Width > myInkCanvas.ActualWidth / 3)
            {
                img.Width = myInkCanvas.ActualWidth / 3;
                CustomInkCanvas.SetLeft(img, myInkCanvas.ActualWidth / 3);
                CustomInkCanvas.SetTop(img, myInkCanvas.ActualHeight / 3);
            }
            else
            {
                CustomInkCanvas.SetLeft(img, myInkCanvas.ActualWidth - img.Width / 2);
                CustomInkCanvas.SetTop(img, myInkCanvas.ActualHeight - img.Height / 2);
            }



        }

        private void SelectColorBtn_Click(object sender, RoutedEventArgs e)
        {
            colorPp.IsOpen = true;
        }

        private void colorWp_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as RadioButton;
            //btn.IsChecked = true;
            var color = ((SolidColorBrush)btn.Background).Color;
            if (highLighted)
            {
                myInkCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(128, color.R, color.G, color.B);
            }
            else
            {
                myInkCanvas.DefaultDrawingAttributes.Color = color;
            }
            for (int i = 0; i < colorSp.Children.Count; i++)
            {
                var btn1 = colorSp.Children[i] as RadioButton;
                if (btn1.IsChecked == true)
                {
                    btn1.IsChecked = false;
                    break;
                }
            }
            colorPp.IsOpen = false;
        }

        private void colorSp_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as RadioButton;
            //btn.IsChecked = true;
            var color = ((SolidColorBrush)btn.Background).Color;
            if (highLighted)
            {
                myInkCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(128, color.R, color.G, color.B);
            }
            else
            {
                myInkCanvas.DefaultDrawingAttributes.Color = color;
            }
            for (int i = 0; i < colorWp.Children.Count; i++)
            {
                var btn1 = colorWp.Children[i] as RadioButton;
                if (btn1.IsChecked == true)
                {
                    btn1.IsChecked = false;
                    break;
                }
            }
        }

        private void PageSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            if ((index = (sender as ListBox).SelectedIndex) > -1)
                viewModel.CurrPageIndex = index;
        }

        private void ShowPageOperation(object sender, RoutedEventArgs e)
        {

        }

        private void PageListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = MouseWheelEvent;
            eventArg.Source = sender;
            (sender as ListBox).RaiseEvent(eventArg);
        }

        private void ButtonBackGround_Click(object sender, RoutedEventArgs e)
        {
            PopChangeBackGround.IsOpen = !PopChangeBackGround.IsOpen;
        }

        private void ChangeBackGround(object sender, RoutedEventArgs e)
        {
            string s0 = (sender as Button).Content.ToString();
            SolidColorBrush sb = new SolidColorBrush();
            
            if(s0=="黑色")
            {
                myInkCanvas.Background = Brushes.Black;
            }
            else if(s0=="白色")
            {
                myInkCanvas.Background = Brushes.White;
            }
            else if (s0=="绿色")
            {
                myInkCanvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#305543"));

            }
        }

        private void ButtonShowSource_Click(object sender, RoutedEventArgs e)
        {
           // (parentObject as MainWindow).ShowCourseMenu();
        }


    }
}
