using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace E_Warehouse.Converts
{
    internal class VisibilityToggleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("Converter can only convert to value of type Visibility.");

            if(value == null) throw new Exception("Input value of the converter cannot be null");

            Visibility vis = (Visibility)value;
            var output = vis == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Invalid call - one way only");
        }
    }
}
