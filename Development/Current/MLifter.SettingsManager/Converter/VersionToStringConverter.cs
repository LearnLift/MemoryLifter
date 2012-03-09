using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MLifterSettingsManager
{
    public class VersionToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Version version;
            if (value is Version)
                version = value as Version;
            else
                throw new InvalidOperationException();

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is String) || targetType != typeof(Version))
                throw new InvalidOperationException();

            try
            {
                return new Version(value as String);
            }
            catch (Exception)
            {
                return new Version(1, 0, 0);
            }
        }

        #endregion
    }
}
