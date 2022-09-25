using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GrepExcel.View.Converters
{
    public class BooleanToVisibiltyInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLoading = (bool)value;
            return !isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
