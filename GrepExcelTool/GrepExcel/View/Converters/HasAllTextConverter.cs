using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GrepExcel.View.Converters
{
     public class HasAllTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
             //  bool res = true;
            object type = Visibility.Visible;

            foreach (object val in values)
            {
                if (string.IsNullOrEmpty(val as string))
                {
                    type = Visibility.Collapsed;
                }
            }

            return type;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
