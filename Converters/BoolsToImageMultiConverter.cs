using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WPF_Bestelbons.Converters
{
    public class BoolsToImageMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool ask_to_approve = false;
            bool approved = false;

            if (values[0] != DependencyProperty.UnsetValue)  ask_to_approve = (bool)values[0];
            if (values[1] != DependencyProperty.UnsetValue) approved = (bool)values[1];

            if (!ask_to_approve && !approved )
            {
                BitmapImage image1 = new BitmapImage(new Uri("pack://application:,,,/Resources/BLANCO.png"));
                return image1;
            }

            if (ask_to_approve)
            {
                BitmapImage image2 = new BitmapImage(new Uri("pack://application:,,,/Resources/ASK_TO_APPROVE.png"));
                return image2;
            }

            if (approved)
            {
                BitmapImage image3 = new BitmapImage(new Uri("pack://application:,,,/Resources/APPROVED.png"));
                return image3;
            }

           return new BitmapImage(new Uri("pack://application:,,,/Resources/BLANCO.png"));

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
