using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using SecurityFramework;

namespace SecurityAdminSuite
{
    public static class Converters
    {
        public static Bool2VisibilityConverter Bool2VisibilityConverter = new Bool2VisibilityConverter();
        public static Inherited2OpacityConverter Inherited2OpacityConverter = new Inherited2OpacityConverter();
        public static Group2MembersConverter Group2MembersConverter = new Group2MembersConverter();
        public static Null2VisibilityConverter Null2VisibilityConverter = new Null2VisibilityConverter();
        public static InverseBool2VisibilityConverter InverseBool2VisibilityConverter = new InverseBool2VisibilityConverter();
    }
    public class Group2MembersConverter :IMultiValueConverter
    {
      
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Group g = values[0] as Group;
            if (g == null) return null;
            Facade facade = values[1] as Facade;

            var x = from ui in facade.Users
                    from gi in ui.Groups
                    where gi.Group.Name == g.Name && gi.IsMember==true
                    select ui;

                   

            int count = x.Count<UserInfo>();
            List<UserInfo> uis = x.ToList<UserInfo>();
            return uis;


            //Group g = values[0] as Group;
            //if (g == null) return null;
            //Framework fw = values[1] as Framework;

            //var x = from u in fw.Users
            //        from gr in u.Groups
            //        where gr.Name == g.Name
                   
            //        select u;

            //int count = x.Count<User>();
            //return x.ToList<User>();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class Bool2VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)value;
            if (b) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class InverseBool2VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)value;
            if (!b) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class Null2VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           if(value!=null) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class Inherited2OpacityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isInherited = (bool)value;
            if (isInherited) return 0.3;
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
