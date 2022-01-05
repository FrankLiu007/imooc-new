using IMOOC.EduCourses.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Media;

namespace IMOOC.EduCourses.PPTModule
{
    /// <summary>
    /// Interaction logic for PptTool.xaml
    /// </summary>
    public partial class PptTool : UserControl
    {

        public bool isHighlighted;
        private ColorToBrushConverte promptVisibleConverte;
        public InkCanvasEditingMode MyinkEditMd;

        public DrawingAttributes inkDA;
        public DrawingAttributes highlightDA;
        public CoursesViewModel viewModel;

        public PptTool()
        {
            InitializeComponent();

            MyinkEditMd = InkCanvasEditingMode.Ink;
            inkDA = InitInkDA();
            highlightDA = InitHighlightDA();
            isHighlighted = false;
            PenProm();

            promptVisibleConverte = new ColorToBrushConverte();
            thickness.SetBinding(CustomButton.PromptColorProperty, new Binding("CustomInkCan.DefaultDrawingAttributes.Color") { Converter = promptVisibleConverte, Mode = BindingMode.TwoWay });
            InitCustomBtnBinding();


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = (DataContext as CoursesViewModel);
        }

        private void PPTChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //(DataContext as CoursesViewModel).ChangePPT((sender as ComboBox).SelectedValue.ToString());

        }

        private void JieTuPPT(object sender, RoutedEventArgs e)
        {
            ScreenCaptureLib.MaskWindow mask = new ScreenCaptureLib.MaskWindow();
            mask.indicater.CourseCtrl = viewModel.CourseCtrl;
            mask.ShowDialog();
        }

        //public void CommonView()
        //{
        //    ButtonRemarkView.IsEnabled = false;
        //    PiZhuSp.Visibility = Visibility.Collapsed;
        //    //pptOperation.PPTHideWin();
        //    PPTViewGrid.Children.Clear();
        //    PPTViewGrid.Children.Add(pptCommonView);
        //    //isPlayPPT = false;
        //}

        //public void BrowseView()
        //{
        //    ButtonRemarkView.IsEnabled = false;
        //    PiZhuSp.Visibility = Visibility.Collapsed;
        //    //pptOperation.PPTHideWin();
        //    PPTViewGrid.Children.Clear();
        //    PPTViewGrid.Children.Add(pptBrowseView);
        //    //isPlayPPT = false;
        //}


        private DrawingAttributes InitInkDA()
        {
            DrawingAttributes inkDA = new DrawingAttributes();
            inkDA.IgnorePressure = false;
            inkDA.Color = Colors.Red;
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
            highlightDA.Width = 30;
            highlightDA.Height = 30;
            highlightDA.IgnorePressure = true;
            highlightDA.StylusTip = StylusTip.Rectangle;
            return highlightDA;
        }


        private void InitCusButBinding()
        {
            foreach (var s in thicknessSp.Children)
            {
                (s as CustomButton).SetBinding(CustomButton.PromptColorProperty, new Binding("CustomInkCan.DefaultDrawingAttributes.Color") { Converter = promptVisibleConverte, Mode = BindingMode.TwoWay });
            }
        }

        private void InitCustomBtnBinding()
        {
            foreach (var s in thicknessSp.Children)
            {
                (s as CustomButton).SetBinding(CustomButton.PromptColorProperty, new Binding("CustomInkCan.DefaultDrawingAttributes.Color") { Converter = promptVisibleConverte, Mode = BindingMode.TwoWay });
            }
        }

        private void ChangedCheckedColor()
        {
            for (int i = 0; i < colorSp.Children.Count - 1; i++)
            {
                var btn = colorSp.Children[i] as RadioButton;
                if (((SolidColorBrush)btn.Background).Color == viewModel.CourseCtrl.myInkCanvas.DefaultDrawingAttributes.Color)
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
                if (((SolidColorBrush)btn.Background).Color == viewModel.CourseCtrl.myInkCanvas.DefaultDrawingAttributes.Color)
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
            thickness.PromptThick = 15;
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
            if (btn.Name.ToString() == "selectColorBtn")
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
                    Public.Log.WriterLog("笔的宽度已经最小！",viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                    return;
                }

                thickness.PromptThick -= 10;
                highlightDA.Height = thickness.PromptThick / 8;
                highlightDA.Width = thickness.PromptThick / 8;

            }
            else
            {
                if (thickness.PromptThick < 20)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "笔的宽度已经最小！");
                    message.ShowDialog();
                    Public.Log.WriterLog("笔的宽度已经最小！", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                    return;
                }
                thickness.PromptThick -= 10;
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
                    Public.Log.WriterLog("笔的宽度已经最大！", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                    return;
                }

                thickness.PromptThick += 10;
                highlightDA.Height = thickness.PromptThick * 1.5;
                highlightDA.Width = thickness.PromptThick * 1.5;


            }
            else
            {
                if (thickness.PromptThick > 200)
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "笔的宽度已经最大！");
                    message.ShowDialog();
                    Public.Log.WriterLog("笔的宽度已经最大！", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                    return;
                }
                thickness.PromptThick += 10;
                inkDA.Height = thickness.PromptThick / 8;
                inkDA.Width = thickness.PromptThick / 8;

            }
        }

        private void ButtonHilight_Click(object sender, RoutedEventArgs e)
        {
            ButtonColor1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#68F3FF"));
            ButtonColor2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86FF56"));
            ButtonColor3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF952"));
            ButtonColor4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DC5"));
            (DataContext as CoursesViewModel).CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            isHighlighted = true;
            TransPenPrompt();
            HilightProm();
            //highlightDA.Width = thickness.PromptThick;
            //highlightDA.Height = thickness.PromptThick;
            viewModel.CourseCtrl.myInkCanvas.DefaultDrawingAttributes = highlightDA;
            ChangedCheckedColor();

            viewModel.CourseCtrl.UpDateCursor();
        }

        private void ButtonLineEraser_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            MyinkEditMd = InkCanvasEditingMode.EraseByStroke;
        }

        private void ButtonSelectArea_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Select;
            MyinkEditMd = InkCanvasEditingMode.Select;
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

        private void ButtonPencil_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CourseCtrl.myInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            MyinkEditMd = InkCanvasEditingMode.Ink;
            ButtonColor1.Background = Brushes.Black;
            ButtonColor2.Background = Brushes.White;
            ButtonColor3.Background = Brushes.Blue;
            ButtonColor4.Background = Brushes.Red;

            isHighlighted = false;
            TransPenPrompt();
            PenProm();

            viewModel.CourseCtrl.myInkCanvas.DefaultDrawingAttributes = inkDA;
            ChangedCheckedColor();

            viewModel.CourseCtrl.UpDateCursor();

        }

        private void OpenPPT_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();
            // ofd.Filter = ".ppt;.pptx;.pps;.ppsx;.html|*.ppt;*.pptx;*.pps;*.html";
            ofd.Filter = ".ppt;.pptx;.pps;.ppsx;.zh|*.ppt;*.pptx;*.pps;*.zh";
            if (ofd.ShowDialog() == true)
            {
                if (viewModel.PptNameList.Contains(Path.GetFileNameWithoutExtension(ofd.FileName.Replace(" ",""))))
                {
                    MessageWin message = new MessageWin(MessageWinType.Prompt, "当前PPT或ZH文件已打开！");
                    message.ShowDialog();
                    Public.Log.WriterLog(ofd.FileName + "当前PPT或ZH文件已打开！", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                    return;
                }
                var name = Path.GetFileName(ofd.FileName);
                string name1 = Path.GetFileNameWithoutExtension(ofd.FileName);
                string folderPath = ofd.FileName.Replace("\\" + ofd.SafeFileName, "");
                string destPath = viewModel.CurrCourse.CoursePath + "\\" + viewModel.CurrCourse.Name;
                if (!Directory.Exists(destPath + "\\Resource\\ppt"))
                {
                    Directory.CreateDirectory(destPath + "\\Resource\\ppt");
                    Public.Log.FileOrFolder(Public.LogType.CreatFolder, destPath + "\\Resource\\ppt", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                }                                                                                
                if (Path.GetExtension(name) == ".zh")
                {
                    if (!Directory.Exists(destPath + "\\Resource\\ppt\\" + name1))
                    {
                        Public.HelperMethods.UnZipFolder(ofd.FileName, destPath + "\\Resource\\ppt");                        
                    }
                    try
                    {
                        if (!File.Exists(destPath + "\\" + name))
                        {
                            File.Copy(ofd.FileName, destPath + "\\" + name);
                            Public.Log.FileOrFolder(Public.LogType.CopeFile, ofd.FileName + "  to  " + destPath + "\\" + name,viewModel.CurrCourse.Name,viewModel.CurrCourse.CoursePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n移动.ZH文件至课程文件夹下时发生错误");
                    }                                                           
                    viewModel.showhtml(destPath + "\\Resource\\ppt\\" + name1+"\\"+ name1 + ".html");
                }
                else
                {
                    if (Type.GetTypeFromProgID("Powerpoint.Application") == null)
                    {
                        MessageWin message = new MessageWin(MessageWinType.Prompt, "未检测到安装Powerpoint，未安装前该模块不可用！");
                        message.ShowDialog();
                        Public.Log.WriterLog("未检测到安装Powerpoint，未安装前该模块不可用!", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                        viewModel.BanshuMenu();
                        viewModel.CourseCtrl.GotoBanshu();
                        return;
                    }

                    if (!Public.HelperMethods.IsOffice2003Above())
                    {
                        MessageWin message = new MessageWin(MessageWinType.Prompt, "当前Office版本过低，请安装Office2007及以上版本");
                        message.ShowDialog();
                        Public.Log.WriterLog("当前Office版本过低，请安装Office2007及以上版本", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                        viewModel.BanshuMenu();
                        viewModel.CourseCtrl.GotoBanshu();
                        return;
                    }
                    if (viewModel.CourseCtrl.IsWin7OrAbove)
                    {
                        if (viewModel.CourseCtrl.convert==null)
                        {
                            try
                            {
                                viewModel.CourseCtrl.convert = new Utils.PPTConvert(new PPTConvertInfo
                                {
                                    Number = 1,
                                    PPTName = ofd.FileName,
                                    State = "等待"
                                });
                                viewModel.CourseCtrl.convert.viewModel = viewModel;
                            }
                            catch (Exception ex)
                            {
                                MessageWin message = new MessageWin(MessageWinType.Prompt, "未能加载到PPT转换模块！请联系我们。");
                                message.ShowDialog();
                                Public.Log.WriterLog("未能加载到PPT转换模块！请联系我们" + ex.Message, viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                                viewModel.CourseCtrl.convert = null;
                                return;
                            }

                        }
                        else
                        {
                            if (viewModel.CourseCtrl.convert.ISAllConverted)
                            {
                                if (viewModel.CourseCtrl.convert.isPPTSContain(name))
                                {
                                    MessageWin message = new MessageWin(MessageWinType.Warning, "当前PPT文件已在转换列表中,是否重新转换？");
                                    message.ShowDialog();
                                    Public.Log.WriterLog("当前PPT文件已在转换列表中,是否重新转换？", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                                    if (message.IsYes == true)
                                    {
                                        viewModel.CourseCtrl.convert.AddPPT(ofd.FileName);
                                    }
                                }
                                else
                                {
                                    viewModel.CourseCtrl.convert.AddPPT(ofd.FileName);
                                }
                            }
                            else
                            {
                                MessageWin message = new MessageWin(MessageWinType.Prompt, "请等待当前转换完成！");
                                message.ShowDialog();
                                Public.Log.WriterLog("请等待当前转换完成!", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                            }

                        }
                        viewModel.CourseCtrl.convert.Hide();
                        viewModel.CourseCtrl.convert.Activate();
                        viewModel.CourseCtrl.convert.Show();
                    }
                    else
                    {
                        MessageWin message = new MessageWin(MessageWinType.Warning, "当前系统版本低于Win7，不支持PPT转换！");
                        message.ShowDialog();
                        Public.Log.WriterLog("当前系统版本低于Win7，不支持PPT转换！", viewModel.CurrCourse.Name, viewModel.CurrCourse.CoursePath);
                    }

                }
            }
        }
       
    }
}
