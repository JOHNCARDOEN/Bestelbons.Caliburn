using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF_Bestelbons.Converters
{
    public class StringInTextToBackgroundColor : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (!string.IsNullOrEmpty((string)values[1]))
            {
                if ((((string)values[0]).ToLower()).Contains((string)values[1]))
                {
                    SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("deeppink"));//System.Windows.Media.Brushes.Orange; "#CC4E06"
                    return brush;
                }
                else
                {
                    SolidColorBrush brush = System.Windows.Media.Brushes.Transparent;
                    return brush;
                }
            }

            else
            {
                SolidColorBrush brush = System.Windows.Media.Brushes.Transparent;
                return brush;
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
