﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace GrepExcel.View.Converters
{
    public class PathNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string pathFile = value.ToString();

            if (File.Exists(pathFile))
            {
                return Path.GetDirectoryName(pathFile);
            }

            return pathFile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
