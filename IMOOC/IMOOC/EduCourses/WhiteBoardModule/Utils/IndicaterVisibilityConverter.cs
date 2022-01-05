using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
namespace IMOOC.EduCourses.WhiteBoardModule
{
        [ValueConversion(typeof( InkCanvasEditingMode), typeof(System.Windows.Visibility))]
        public class IndicaterVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                InkCanvasEditingMode InkM = (InkCanvasEditingMode)value;
                if (InkM != InkCanvasEditingMode.Select)
                { 
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Visible;
                }
            }

            public object ConvertBack(object value, Type targetType,
                object parameter, CultureInfo culture)
            {
                return null;
            }
        }

}
