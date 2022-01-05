using IMOOC.EduCourses.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Media;

namespace IMOOC.EduCourses
{
    /// <summary>
    /// Interaction logic for BanshuTool.xaml
    /// </summary>
    public partial class BanshuTool : UserControl
    {     
        public  bool isHighlighted;
        private ColorToBrushConverte colorToBrushConverte;
        public InkCanvasEditingMode MyinkEditMd;

        public DrawingAttributes inkDA;
        public DrawingAttributes highlightDA;


        public BanshuTool()
        {
            InitializeComponent();
            MyinkEditMd =InkCanvasEditingMode.Ink;
            inkDA = InitInkDA();
            highlightDA = InitHighlightDA();
            isHighlighted = false;
            PenProm();   

            colorToBrushConverte = new ColorToBrushConverte();
            thickness.SetBinding(CustomButton.PromptColorProperty, new Binding("CustomInkCan.DefaultDrawingAttributes.Color") { Converter = colorToBrushConverte, Mode = BindingMode.TwoWay });
            InitCustomBtnBinding();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.InkCansBackgrand = greenBoard.Background;
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.Background = greenBoard.Background;
            if (UserAppConfigStatic.IsStylusOnly)
            {
                checkStylus.IsChecked = true;
                checkStylus.Content = FindResource("TrueCheck");
                (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.IsStylusOnly = true;
            }
            else
            {
                checkStylus.IsChecked = false;
                checkStylus.Content = FindResource("FalseCheck");
                (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.IsStylusOnly = false;
            }
            ButtonColor2.IsChecked = true;
        }

        private DrawingAttributes InitInkDA()
        {
            DrawingAttributes inkDA = new DrawingAttributes();
            inkDA.IgnorePressure = false;
            inkDA.Color = Colors.White;
            inkDA.Width = 2;
            inkDA.Height = 2;
            inkDA.IsHighlighter = false;          
            return inkDA;
        }

        private DrawingAttributes InitHighlightDA()
        {
            DrawingAttributes highlightDA = new DrawingAttributes();
            highlightDA.Color = (Color)ColorConverter.ConvertFromString("#86FF56");
            highlightDA.IsHighlighter = true;
            highlightDA.Width = 15;
            highlightDA.Height = 15;
            highlightDA.IgnorePressure = true;
            highlightDA.StylusTip = StylusTip.Rectangle;
            return highlightDA;
        }


        private void InitCustomBtnBinding()
        {
            foreach (var s in thicknessSp.Children)
            {
                (s as CustomButton).SetBinding(CustomButton.PromptColorProperty, new Binding("CustomInkCan.DefaultDrawingAttributes.Color") { Converter = colorToBrushConverte, Mode = BindingMode.TwoWay });
            }
        }      

        private void ChangedCheckedColor()
        {
            for (int i = 0; i < colorSp.Children.Count-1; i++)
            {
                var btn = colorSp.Children[i] as RadioButton;
                if (((SolidColorBrush)btn.Background).Color == (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.DefaultDrawingAttributes.Color)
                {         
                    for (int j = 0; j < colorWp.Children.Count; j++)
                    {
                        var btn1 = colorWp.Children[j] as RadioButton;
                        if (btn1.IsChecked == true)
                        {
                            btn1.IsChecked = false;
                        }
                    }
                    btn.IsChecked = true;
                    return;
                }
            }
            for (int i = 0; i < colorWp.Children.Count; i++)
            {
                var btn = colorWp.Children[i] as RadioButton;
                if (((SolidColorBrush)btn.Background).Color == (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.DefaultDrawingAttributes.Color)
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

        private void TransPenPrompt()
        {
            if (isHighlighted)
            {
                thickness.PenTrans = true;
                foreach (var s in thicknessSp.Children)
                {
                    (s as CustomButton).PenTrans = true;
                }
            }
            else
            {
                thickness.PenTrans = false;
                foreach (var s in thicknessSp.Children)
                {
                    (s as CustomButton).PenTrans = false;
                }
            }

        }

        private void PenProm()
        {
            thickness.PromptThick = 20;
            int i = 10;
            foreach (var s in thicknessSp.Children)
            {
                (s as CustomButton).PromptThick = i;
                i = i + 10;
            }

        }

        private void HilightProm()
        {
            thickness.PromptThick = 10;
            int i = 5;
            foreach (var s in thicknessSp.Children)
            {
                (s as CustomButton).PromptThick = i;
                i = i + 5;
            }
        }               

        private void colorSp_Click(object sender, RoutedEventArgs e)
        {

            var btn = e.OriginalSource as RadioButton;
            if (btn.Name.ToString()== "selectColorBtn")
            {
                colorPp.IsOpen = true;
                return;
            }
            var color = ((SolidColorBrush)btn.Background).Color;

            if (isHighlighted)
            {
                highlightDA.Color = color;
            }
            else
            {
                inkDA.Color = color;
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

        private void thickness_Click(object sender, RoutedEventArgs e)
        {
            thicknessPp.IsOpen = true;
            foreach (var s in thicknessSp.Children)
            {
                (s as CustomButton).Background = Brushes.Turquoise;
            }
            foreach (var s in thicknessSp.Children)
            {
                if ((s as CustomButton).PromptThick == thickness.PromptThick)
                {
                    (s as CustomButton).Background = Brushes.PaleTurquoise;
                }
                
            }
        }

        private void thicknessPp_Click(object sender, RoutedEventArgs e)
        {
            var but = e.Source as CustomButton;
            thickness.PromptThick = but.PromptThick;
            if (isHighlighted)
            {
                highlightDA.Height = but.PromptThick * 1.5;
                highlightDA.Width = but.PromptThick * 1.5;
            }
            else
            {
                inkDA.Height = but.PromptThick / 8;
                inkDA.Width = but.PromptThick / 8;
            }

            thicknessPp.IsOpen = false;
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (isHighlighted)
            {
                if (thickness.PromptThick < 20)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "笔的宽度已经最小！");
                    message.ShowDialog();
                    Public.Log.WriterLog("笔的宽度已经最小！");              
                    return;
                }

                thickness.PromptThick -= 5;
                highlightDA.Height = thickness.PromptThick / 8;
                highlightDA.Width = thickness.PromptThick / 8;

            }
            else
            {
                if (thickness.PromptThick < 20)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "笔的宽度已经最小！");
                    message.ShowDialog();
                    Public.Log.WriterLog("笔的宽度已经最小！");
                    return;
                }
                thickness.PromptThick -= 5;
                inkDA.Height = thickness.PromptThick / 8;
                inkDA.Width = thickness.PromptThick / 8;
            }


        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (isHighlighted)
            {
                if (thickness.PromptThick > 40)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "笔的宽度已经最大！");
                    message.ShowDialog();
                    Public.Log.WriterLog("笔的宽度已经最大！");
                    return;
                }

                thickness.PromptThick += 5;
                highlightDA.Height = thickness.PromptThick * 1.5;
                highlightDA.Width = thickness.PromptThick * 1.5;


            }
            else
            {
                if (thickness.PromptThick > 200)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "笔的宽度已经最大！");
                    message.ShowDialog();
                    Public.Log.WriterLog("笔的宽度已经最大！");
                    return;
                }
                thickness.PromptThick += 5;
                inkDA.Height = thickness.PromptThick / 8;
                inkDA.Width = thickness.PromptThick / 8;

            }
        }               

        private void colorWp_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as RadioButton;
            var color = ((SolidColorBrush)btn.Background).Color;
           
            if (isHighlighted)
            {
                highlightDA.Color = color;
            }
            else
            {
                //PenColor = color;
                inkDA.Color = color;
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

        private void ButtonNewNextPreviousPage_Click(object sender, RoutedEventArgs e)
        {
           (DataContext as CoursesViewModel).CourseCtrl.indicater.updataIndicater(new Thickness(0, 0, 0, 0), 0, 0);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkStylus.IsChecked == true)
            {
                checkStylus.Content = FindResource("TrueCheck");
            }
            else
            {
                checkStylus.Content = FindResource("FalseCheck");
            }
            UserAppConfigStatic.IsStylusOnly = (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.IsStylusOnly = (bool)checkStylus.IsChecked;
            SetEditingMode();
        }

        private void SetEditingMode() 
        {
            if (PencilStyle.IsChecked==true)
            {
                SetPencil();
            }
            else if(HighlightStyle.IsChecked==true)
            {
                SetHilightPencil();
            }
            else if (EraserStyle.IsChecked==true)
            {
                SetLineEraser();
            }
            else if (SelectStyle.IsChecked==true)
            {
                SetSelectArea();
            }
        }

        private void SetPencil()
        {
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            MyinkEditMd = InkCanvasEditingMode.Ink;
            ButtonColor1.Background = Brushes.Black;
            ButtonColor2.Background = Brushes.White;
            ButtonColor3.Background = Brushes.Violet;
            ButtonColor4.Background = Brushes.Cyan;

            isHighlighted = false;
            TransPenPrompt();
            PenProm();

            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.DefaultDrawingAttributes = inkDA;
            ChangedCheckedColor();

            (DataContext as CoursesViewModel).CourseCtrl.UpDateCursor();
        }

        private void SetHilightPencil()
        {
            ButtonColor1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#68F3FF"));
            ButtonColor2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86FF56"));
            ButtonColor3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF952"));
            ButtonColor4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DC5"));
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            isHighlighted = true;
            TransPenPrompt();
            HilightProm();
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.DefaultDrawingAttributes = highlightDA;
            ChangedCheckedColor();

            (DataContext as CoursesViewModel).CourseCtrl.UpDateCursor();
        }

        private void SetLineEraser()
        {
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            MyinkEditMd = InkCanvasEditingMode.EraseByStroke;
        }

        private void SetSelectArea()
        {
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Select;
            MyinkEditMd = InkCanvasEditingMode.Select;
        }

        private void ButtonPencil_Click(object sender, RoutedEventArgs e)
        {
            SetPencil();
        }

        private void ButtonHilight_Click(object sender, RoutedEventArgs e)
        {
            SetHilightPencil();
        }

        private void ButtonLineEraser_Click(object sender, RoutedEventArgs e)
        {
            SetLineEraser();
        }

        private void ButtonSelectArea_Click(object sender, RoutedEventArgs e)
        {
            SetSelectArea();
        }

        //private void PageUp_Click(object sender, RoutedEventArgs e)
        //{

        //    int i = (DataContext as CoursesViewModel).CourseCtrl.PageUp()+1;
        //    OldCourseText.Content = i.ToString();
        //}

        //private void PageDown_Click(object sender, RoutedEventArgs e)
        //{
        //    int i = (DataContext as CoursesViewModel).CourseCtrl.PageDown()+1;
        //    OldCourseText.Content = i.ToString();
        //}

        private void ButtonPasteSelection_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.ButtonPasteSelection();
        }

        private void SetBackgroundSp_MouseDown(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Border;
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.Background = btn.Background;
            (DataContext as CoursesViewModel).CourseCtrl.InkCansBackgrand= btn.Background;
        }

        //private void SetBackGroundBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    SetBackgroundPp.IsOpen = true;
        //}

        private void Undo(object sender, RoutedEventArgs e)
        {
            (DataContext as CoursesViewModel).CourseCtrl.Undo();
        }


    }
}
