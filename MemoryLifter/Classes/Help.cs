using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Classes
{
    /// <summary>
    /// Provides helper functions for providing an online help.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-03-06</remarks>
    public static class Help
    {
        /// <summary>
        /// Gets the help path (help namespace).
        /// </summary>
        /// <value>The help path.</value>
        /// <remarks>Documented by Dev02, 2008-03-06</remarks>
        public static string HelpPath
        {
            get
            {
                return System.IO.Path.Combine(Application.StartupPath, Properties.Resources.HELP_FILEPATH);
            }
        }

        /// <summary>
        /// Sets the help name space.
        /// </summary>
        /// <param name="helpProvider">The help provider.</param>
        /// <remarks>Documented by Dev02, 2008-03-06</remarks>
        public static void SetHelpNameSpace(HelpProvider helpProvider)
        {
            helpProvider.HelpNamespace = HelpPath;
        }
    }
}
