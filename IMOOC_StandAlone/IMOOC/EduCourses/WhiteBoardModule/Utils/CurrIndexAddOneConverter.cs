using System;
using System.Globalization;
using System.Windows.Data;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public class CurrIndexAddOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            return index + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
