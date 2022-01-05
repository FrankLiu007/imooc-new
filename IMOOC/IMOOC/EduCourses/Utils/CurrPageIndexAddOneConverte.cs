using System;
using System.Globalization;
using System.Windows.Data;

namespace IMOOC.EduCourses
{
    [ValueConversion(typeof(int), typeof(int))]
    class CurrPageIndexAddOneConverte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {           
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
