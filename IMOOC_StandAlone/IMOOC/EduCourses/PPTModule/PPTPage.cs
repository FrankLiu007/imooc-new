using System.Drawing;

namespace IMOOC.EduCourses.PPTModule
{
    public class PPTPage : NotificationObject
    {
        private Image pageImage;

        public PPTPage()
        {

        }
        public Image PageImage
        {
            get { return pageImage; }
            set { RaisePropertyChanged(ref pageImage, value, "PageImage"); }
        }

        private int pageNum;

        public int PageNum
        {
            get { return pageNum; }
            set { RaisePropertyChanged(ref pageNum, value, "PageNum"); }
        }

        public PPTPage(string str0)
        {
            pageImage = Image.FromFile(str0);
        }



    }
}
