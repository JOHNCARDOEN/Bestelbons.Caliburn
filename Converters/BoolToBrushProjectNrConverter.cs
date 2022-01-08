using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF_Bestelbons.Converters
{
    class BoolToBrushProjectNrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                SolidColorBrush brush = new SolidColorBrush(color: Colors.Green);
                return brush;
            }
            else
            {
                SolidColorBrush brush = new SolidColorBrush(color: Colors.Red);
                return brush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
