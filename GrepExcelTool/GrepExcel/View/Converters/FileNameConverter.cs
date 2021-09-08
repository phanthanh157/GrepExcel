using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace GrepExcel.View.Converters
{
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string pathFile = value.ToString();

            if (File.Exists(pathFile))
            {
                string fileName = Path.GetFileName(pathFile);

                return fileName;

            }

            return pathFile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
