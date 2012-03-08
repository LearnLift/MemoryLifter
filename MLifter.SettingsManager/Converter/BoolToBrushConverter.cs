using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MLifter.Generics;
using System.Windows.Media;

namespace MLifterSettingsManager
{
    public class BoolToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? val = value as bool?;
            return val.HasValue && val.Value ? Brushes.Orange : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
