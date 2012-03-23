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
using System.Reflection;
using MLifter.DAL.DB;
using MLifter.DAL.Interfaces;
using SecurityFramework;
using SecurityPgSqlAdapter;
using MLifter.Security.MsSqlCe;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Security
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
                return "learning-module-" + (o as IParent).Parent.GetParentDictionary().Id.ToString();
            if (o is DbChapter)
                return "chapter-" + (o as DbChapter).Id.ToString();
            if (o is DbCards)
                return "learning-module-" + (o as IParent).Parent.GetParentDictionary().Id.ToString();
            if (o is DbCard)
                return "card-" + (o as DbCard).Id.ToString();
            if (o is DbSettings)
                return "settings-" + (o as DbSettings).Id.ToString();
            if (o is DbCardStyle)
                return "cardstyle-" + (o as DbCardStyle).Id.ToString();

            return string.Empty;
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>The parent object.</returns>
        /// <remarks>Documented by Dev03, 2009-02-23</remarks>
        public override object GetParent(object o)
        {
            object parent = null;
            if (o is DbDictionary)
            {

                parent = (o as IParent).Parent.CurrentUser.List() as DbDictionaries;
                return parent;
            }

            if (o is DbChapter)
            {

                parent = (o as IParent).Parent.GetParentDictionary();
                return parent;
            }

            if (o is DbCard)
            {
                int cid = (o as DbCard).Chapter;
                IDictionary dic = (o as IParent).Parent.GetParentDictionary();
                parent = dic.Chapters.Get(cid);
                return parent;
            }
            return null;
        }

        /// <summary>
        /// Gets the security data adapter.
        /// </summary>
        /// <param name="connectionString">The connection string struct.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public static Framework GetDataAdapter(string connectionString)
        {
            PgSqlSecurityDataAdapter adapter = new PgSqlSecurityDataAdapter(connectionString);
            return new SecurityFramework(adapter);
        }

        /// <summary>
        /// Gets the security data adapter.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public static Framework GetDataAdapter(IUser user)
        {
            SecurityFramework framework = null;
            switch (user.ConnectionString.Typ)
            {
                case DatabaseType.PostgreSQL:
                    framework = new SecurityFramework(
                        new PgSqlSecurityDataAdapter(user.ConnectionString.ConnectionString)
                        );
                    break;
                case DatabaseType.MsSqlCe:
                    framework = new SecurityFramework(
                        new SqlCeSecurityDataAdapter(user)
                    );
                    break;
            }

            return framework;
        }
       
    }
}
