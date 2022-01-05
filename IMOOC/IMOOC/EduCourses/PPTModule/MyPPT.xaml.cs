using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;


namespace IMOOC.EduCourses.PPTModule
{
    /// <summary>
    /// Interaction logic for MyPPT.xaml
    /// </summary>
    public partial class MyPPT : UserControl
    {
        //public PPTOperation pptOperation;
        PPTBrowseView pptBrowseView;
        PPTCommonView pptCommonView;
        public Grid PPTViewGrid;

        //bool isIntoPPT;
        //bool isPlayPPT;
        //bool ppt;



        public MyPPT()
        {
            InitializeComponent();

            //pptOperation = new PPTOperation();
            pptBrowseView = new PPTBrowseView();
            pptCommonView=new PPTCommonView();
            PPTViewGrid = new Grid();
            //pptTool.PPTChange.SelectionChanged += PPTChange_SelectionChanged;
            //isIntoPPT = false;
            //isPlayPPT = false;
            //ppt = false;

            PPTViewGrid.Children.Add(pptCommonView);
        }

        //public void OpenPPT(string fileName)
        //{

            //if (!isIntoPPT)
            //{

            //    if (pptOperation.CheckPowerpointOpened())
            //    {
            //        MessageBoxResult re = MessageBox.Show("检测到未关闭的PPT程序，请确认您已保存当前工作 \n点击确定，将强制关闭PPT", "注意！",
            //           MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            //        if (re == MessageBoxResult.OK)
            //        {
            //            pptOperation.KillBackgroundPPT();
            //        }
            //        else
            //        {
            //            return;
            //        }

            //    }
            //    pptOperation.CreatePPTApplication();
            //}



            //var ofd = new OpenFileDialog();
            //ofd.Filter = ".ppt;.pptx;.pps;.ppsx|*.ppt;*.pptx;*.pps";

            //if (ofd.ShowDialog() == true)
            //{
            //for (int i = 0; i < pptOperation.PPTFlieName.Count; i++)
            //{
                //if (pptOperation.PPTFlieName.Contains(fileName))
                //{
                //    MessageBoxResult re = MessageBox.Show("当前PPT可能已经打开 \n如要打开当前PPT,请确定已经关闭", "注意！",
                //    MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                //    return;
                //}
            //}

            //    ppt = false;
            //    pptOperation.loadPPT(ofd.FileName);
            //    pptOperation.AddPPTNameList();          
            //    pptOperation.PPTHideWin();                
            //    if (pptOperation.PPTNumbExcFour == true)
            //    {
            //        return;
            //    }

            //pptTool.PPTAddComboBoxItem(fileName);
            //pptOperation.PPTFlieName.Add(fileName);

            //    // (DataContext as CoursesViewModel).CurrSlideList.Clear();
            //    (DataContext as CoursesViewModel).InitAllPage(pptOperation.GetPPTImages());
            //}
            //else
            //{
            //    return;
            //}

            //pptOperation.LPPTdocument[pptOperation.PPTCVC].Application.SlideShowNextSlide += showNextSlide;


            //pptTool.Close.IsEnabled = true;
            //pptTool.ButtonPlayView.IsEnabled = true;
            //isIntoPPT = true;
            //UpdataLocation();

        //}

        //private void PPTChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if((sender as ComboBox).SelectedIndex<0)
        //    {
        //        return;
        //    }

        //    pptOperation.ChangePPT((sender as ComboBox).SelectedIndex);
            //if (ppt)
            //{
            //    (DataContext as CoursesViewModel).UpdataComBroView((sender as ComboBox).SelectedIndex);
            //    pptOperation.AddPPTNameList();
            //    (DataContext as CoursesViewModel).record(CommandType.ChangePPT);
            //}
            //ppt = true;

            //if (PPTViewGrid.Children.Count == 0)
            //{
            //    PPTViewGrid.Children.Add(pptCommonView);
            //}
        //}


        //public void JieTuPPT()
        //{
            //ScreenCaptureLib.MaskWindow mask = new ScreenCaptureLib.MaskWindow();
            //mask.indicater.CourseCtrl = (DataContext as CoursesViewModel).CourseCtrl;
            //mask.ShowDialog();
        //}

        //public bool PiZhu()
        //{
        //    if(pptTool.PiZhuSp.Visibility==Visibility.Visible)
        //    {
        //        pptTool.PiZhuSp.Visibility = Visibility.Collapsed;
        //        return false;
        //    }
        //    else
        //    {
        //        pptTool.PiZhuSp.Visibility = Visibility.Visible;
        //        return true;
        //    }
            
        //}

        

        //public void CloseAllPPT()
        //{
        //    for (int i = pptTool.PPTChange.Items.Count; i >0; i--)
        //    {
        //        pptOperation.ClosePPT(i-1);
        //        pptTool.PPTChange.Items.Remove(pptTool.PPTChange.Items[i-1]);
        //        pptOperation.PPTFlieName.Remove(pptOperation.PPTFlieName[i-1]);

        //        isPlayPPT = false;
        //    }
        //}

        //public void ClosePPT()
        //{
        //    pptOperation.ClosePPT();
        //    pptTool.PPTChange.Items.Remove(pptTool.PPTChange.Items[pptOperation.PPTCVC]);
        //    pptOperation.PPTFlieName.Remove(pptOperation.PPTFlieName[pptOperation.PPTCVC]);
           
        //    isPlayPPT = false;
        //}

        //public void NextPPT()
        //{
        //    pptOperation.Next();
        //}

        //public void PreviousPPT()
        //{
        //    pptOperation.Previous();
        //}

        //public void PlayCurrSlide()
        //{
        //    pptTool.ButtonRemarkView.IsEnabled = true;
        //    pptTool.PiZhuSp.Visibility = Visibility.Collapsed;
        //    PPTViewGrid.Children.Clear();
        //    pptOperation.PPTPlayCurrSlideIndexList.Add((DataContext as CoursesViewModel).CurrSlideIndex + 1);
        //    pptOperation.GotoSlide((DataContext as CoursesViewModel).CurrSlideIndex + 1);
        //    isPlayPPT = true;
        //}

        //public void UpdataLocation()
        //{
        //    Point p1 = pptTool.PointToScreen(new Point(0, pptTool.ActualHeight));

        //    System.Drawing.Point dPoint = new System.Drawing.Point();
        //    dPoint.X = (int)p1.X;
        //    dPoint.Y = (int)p1.Y;
        //    pptOperation.PPTUpdataLocation(dPoint);
        //}




    }
}
