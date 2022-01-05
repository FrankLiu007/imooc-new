using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    /// <summary>
    /// Indicater.xaml 的交互逻辑
    /// </summary>
    public partial class Indicater : UserControl
    {
        public object parentInk;
        public int state_image;
        public object parentMain;
        public Indicater()
        {
            InitializeComponent();
            Width = 0;
            Height = 0;
            state_image = 0;


        }

        public void updataIndicater(Thickness margin, Rect bounds)
        {
            Width = bounds.Width + 32;
            Height = bounds.Height + 40;
            Margin = margin;



        }
        public void updataIndicater(Thickness margin, double w, double h)
        {


                Width = w;
                Height = h;
            
            Margin = margin;

        }

        public void moveStroke(double x, double y)
        {
            (parentInk as CustomInkCanvas).MoveSelectedStrokes(x, y);
        }

        public void ResizeStroke(Point anchor, double ratio_x, double ratio_y)
        {
            (parentInk as CustomInkCanvas).ResizeStroke( anchor,  ratio_x,  ratio_y);
        }

        public void updataImage()
        {
          //  (parentMain as MainWindow).updataImage();
        }

    }
}
