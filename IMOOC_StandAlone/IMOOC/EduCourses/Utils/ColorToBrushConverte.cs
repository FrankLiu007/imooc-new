using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMOOC.EduCourses
{
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToBrushConverte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush;
            SolidColorBrush pen = new SolidColorBrush();
            pen.Color = (Color)value;
            brush = pen;

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
