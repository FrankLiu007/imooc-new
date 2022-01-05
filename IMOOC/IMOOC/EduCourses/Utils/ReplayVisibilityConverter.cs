using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMOOC.EduCourses
{
    class ReplayVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            string para = (string)parameter;
            if (str==para)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
