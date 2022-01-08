﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WPF_Bestelbons.Converters
{
    public class DeliveredToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri("pack://application:,,,/Resources/LISTITEMDELIVERED.png");
                img.EndInit();
                return img;

            }
            else
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri("pack://application:,,,/Resources/LISTITEM.png");
                img.EndInit();
                return img;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
