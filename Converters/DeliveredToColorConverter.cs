using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF_Bestelbons.Converters
{
    public class DeliveredToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                SolidColorBrush color = new SolidColorBrush(System.Windows.Media.Colors.Aqua);
                return color;
            }
            else
            {
                SolidColorBrush color = new SolidColorBrush(System.Windows.Media.Colors.Aqua);
                return color;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
