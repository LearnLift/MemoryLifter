using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MLifter.Generics;

namespace MLifterSettingsManager
{
    public class ByteToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Let Parse throw an exception if the input is bad
            long bytes = long.Parse(value.ToString());
            string output = string.Empty;

            if (bytes < 2)
                return "Empty.";

            output = Methods.GetFileSize(bytes);

            return string.Format("Data size: {0}", output);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
