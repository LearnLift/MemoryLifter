using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MLifterSettingsManager
{
    public class CheckedToOnOffConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? val = value as bool?;

            TextBlock textBlock = new TextBlock();
            textBlock.Foreground = val.HasValue && val.Value ? Brushes.DarkGreen : Brushes.DarkRed;
            textBlock.Text = val.HasValue && val.Value ? "On" : "Off";

            return textBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
