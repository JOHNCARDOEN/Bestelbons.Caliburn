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
    public class BoolToButtonBackgroundConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom((string)parameter));//System.Windows.Media.Brushes.Orange; "#CC4E06"
                //SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("Orange"));//System.Windows.Media.Brushes.Orange; "#CC4E06"
                return brush;
            }
            else
            {
                SolidColorBrush brush = System.Windows.Media.Brushes.Transparent;
                return brush;
            }


            throw new ArgumentException("Value is not valid");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
