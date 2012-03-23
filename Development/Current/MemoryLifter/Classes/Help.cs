/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
