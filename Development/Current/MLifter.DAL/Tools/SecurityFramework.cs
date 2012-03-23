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

using MLifter.DAL.DB;
using MLifter.DAL.Interfaces;

using SecurityFramework;

namespace MLifter.DAL.Tools
{
    /// <summary>
    /// Implements GetLocator() 
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-14</remarks>
    public class SecurityFramework : Framework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityFramework"/> class.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        internal SecurityFramework(ISecurityDataAdapter adapter):base(adapter)
        {

        }

        /// <summary>
        /// Gets the security ID locator.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public override string GetLocator(object o)
        {
            if (o is DbDictionaries)
                return "learning-modules";
            if (o is DbDictionary)
                return "learning-module-" + (o as DbDictionary).Id.ToString();
            if (o is DbChapters)
                return "chapters-" + (o as DbChapters).Parent.GetParentDictionary().Id.ToString();
            if (o is DbChapter)
                return "chapter-" + (o as DbChapter).Id.ToString();
            if (o is DbCards)
                return "cards-" + (o as DbCards).Parent.GetParentDictionary().Id.ToString();
            if (o is DbCard)
                return "card-" + (o as DbCard).Id.ToString();
            if (o is DbSettings)
                return "settings-" + (o as DbSettings).Id.ToString();
            if (o is DbCardStyle)
                return "cardstyle-" + (o as DbCardStyle).Id.ToString();

            return string.Empty;
        }

        /// <summary>
        /// Gets the security data adapter.
        /// </summary>
        /// <param name="connectionString">The connection string struct.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        internal static Framework GetDataAdapter(ConnectionStringStruct connectionString)
        {
            XmlSecurityDataAdapter adapter;
            //TODO implement data adapter code here
            //adapter = new DbSecurityDataAdapter(); 
            //test code using XmlSecurityDataAdapter
            adapter = new XmlSecurityDataAdapter("Resources\\XmlSecurityFramework.xml");
            return new SecurityFramework(adapter);
        }
    }
}
