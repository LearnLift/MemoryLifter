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
using System.Xml;

namespace MLifter.DAL.XML
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-08-02</remarks>
    internal class XmlConfigHelper
    {
        private XmlConfigHelper()
        {
        }

        /// <summary>
        /// Sets the specified config setting.
        /// </summary>
        /// <param name="document">The Xml document.</param>
        /// <param name="setting_xpath">The setting_xpath.</param>
        /// <param name="setting_value">The setting_value.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        public static void Set(XmlDocument document, string setting_xpath, int setting_value)
        {
            int setting = XmlConvert.ToInt32(document.SelectSingleNode(setting_xpath).InnerText);
            XmlHelper.SetXmlInt32(document, setting_xpath, (setting | setting_value));
        }

        /// <summary>
        /// Unsets the specified config setting.
        /// </summary>
        /// <param name="document">The Xml document.</param>
        /// <param name="setting_xpath">The setting_xpath.</param>
        /// <param name="setting_value">The setting_value.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        public static void Unset(XmlDocument document, string setting_xpath, int setting_value)
        {
            int setting = XmlConvert.ToInt32(document.SelectSingleNode(setting_xpath).InnerText);
            XmlHelper.SetXmlInt32(document, setting_xpath, (setting & ~setting_value));
        }

        /// <summary>
        /// Toggles the specified config setting.
        /// </summary>
        /// <param name="document">The Xml document.</param>
        /// <param name="setting_xpath">The setting_xpath.</param>
        /// <param name="setting_value">The setting_value.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        public static void Toggle(XmlDocument document, string setting_xpath, int setting_value)
        {
            int setting = XmlConvert.ToInt32(document.SelectSingleNode(setting_xpath).InnerText);
            XmlHelper.SetXmlInt32(document, setting_xpath, (setting ^ setting_value));
        }

        /// <summary>
        /// Checks the option fieldIDs in user settings for the requested option.
        /// </summary>
        /// <param name="document">XML document</param>
        /// <param name="setting_xpath">An XPath query string</param>
        /// <param name="setting_value">A bitmask to get the setting for the option</param>
        /// <returns>True of the found value is equal setting_value</returns>
        /// <example>The bitmask 0100 (4) would be used to check if the option EQueryOption.Stats (Display statistics) was set.</example>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        public static bool Check(XmlDocument document, string setting_xpath, int setting_value)
        {
            int setting = XmlConvert.ToInt32(document.SelectSingleNode(setting_xpath).InnerText);
            return (setting & setting_value) > 0;
        }

    }
}
