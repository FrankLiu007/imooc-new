using IMOOC.EduCourses.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Set
{
    /// <summary>
    /// Interaction logic for SetTool.xaml
    /// </summary>
    public partial class SetTool : UserControl
    {
        CoursesViewModel viewModel;
        public SetTool()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = (DataContext as CoursesViewModel);
            PPTConvert.Visibility = viewModel.CourseCtrl.IsWin7OrAbove ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            AboutUsWin aboutUs = new AboutUsWin();
            aboutUs.ShowDialog();              
        }

        private void SystemSettings_Click(object sender, RoutedEventArgs e)
        {
            SetWin setWin = new SetWin();
            setWin.ShowDialog();
            if (UserAppConfigStatic.BoardMode)
            {
                viewModel.CourseCtrl.SetCursor(true);
            }
            else if (UserAppConfigStatic.PenMode)
            {
                viewModel.CourseCtrl.SetCursor(false);
            }

            viewModel.CourseCtrl.InkCansBackgrand = setWin.backGroundLabel.Background;
            viewModel.CourseCtrl.myInkCanvas.Background = setWin.backGroundLabel.Background;

        }

        private void PPTConvert_Click(object sender, RoutedEventArgs e)
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

            if (viewModel.CourseCtrl.convert==null)
            {
                try
                {
                    viewModel.CourseCtrl.convert = new Utils.PPTConvert();
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
            viewModel.CourseCtrl.convert.Hide();
            viewModel.CourseCtrl.convert.Activate();
            viewModel.CourseCtrl.convert.Show();

        }

        private void CourseUpload_Click(object sender, RoutedEventArgs e)
        {
            UpLoadWin uploadWin = new UpLoadWin();
            uploadWin.ShowDialog();
        }
    }
}
