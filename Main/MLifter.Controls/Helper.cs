using System;
using System.Collections.Generic;
using System.Text;
using MLifter.Controls.Properties;
using MLifter.DAL.Interfaces;

namespace MLifter.Controls
{
    public static class HelperClass
    {
        public static bool ShowNews
        {
            get { return Settings.Default.ShowNews; }
            set { Settings.Default.ShowNews = value; }
        }

        public static bool IsResizing { get; set; }

        public static DateTime NewsDate
        {
            get { return Settings.Default.NewsDate; }
            set { Settings.Default.NewsDate = value; }
        }
    }

    /// <summary>
    /// Represents the print settings as choosen by the user.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-01-03</remarks>
    public class PrintSettings
    {
        /// <summary>
        /// Which cards are to print.
        /// </summary>
        public PrintType Type = PrintType.All;
        /// <summary>
        /// Corresponding to PrintType, either CardIDs or ChapterIDs or BoxIDs.
        /// </summary>
        public List<int> IDs = new List<int>();
    }

    /// <summary>
    /// Select which cards to print.
    /// </summary>
    public enum PrintType
    {
        /// <summary>
        /// All cards.
        /// </summary>
        All,
        /// <summary>
        /// Wrong cards (cards in box 1).
        /// </summary>
        Wrong,
        /// <summary>
        /// The selected chapters.
        /// </summary>
        Chapter,
        /// <summary>
        /// The selected boxes.
        /// </summary>
        Boxes,
        /// <summary>
        /// The selected cards.
        /// </summary>
        Individual
    }

    /// <summary>
    /// Provides a way to localize enums.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-02-15</remarks>
    public class EnumLocalizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumLocalizer"/> class.
        /// </summary>
        /// <param name="rm">The rm.</param>
        /// <param name="e">The e.</param>
        /// <remarks>Documented by Dev02, 2008-02-15</remarks>
        public EnumLocalizer(System.Resources.ResourceManager rm, object e)
        {
            this.value = e;
            this.rm = rm;
        }

        public object value;
        private System.Resources.ResourceManager rm;

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-02-15</remarks>
        public override string ToString()
        {
            string resourcestring = value.GetType().Name.ToUpper() + "_" + value.ToString().ToUpper();
            string localizedstring = rm.GetString(resourcestring);
            if ((localizedstring == null) || (localizedstring.Length <= 0))
            {
                System.Diagnostics.Debug.WriteLine("Missing: " + resourcestring);
                return "<" + value.ToString() + ">";
            }
            return localizedstring;
        }

        /// <summary>
        /// Selects the first equaling item in a combobox.
        /// </summary>
        /// <param name="combobox">The combobox.</param>
        /// <returns>True, if found, else false.</returns>
        /// <remarks>Documented by Dev02, 2008-02-15</remarks>
        public static bool SelectItem(System.Windows.Forms.ComboBox combobox, object value)
        {
            foreach (Object item in combobox.Items)
            {
                if (item is EnumLocalizer)
                    if (((EnumLocalizer)item).value.Equals(value))
                    {
                        combobox.SelectedItem = item;
                        return true;
                    }
            }
            return false;
        }
    }
}
