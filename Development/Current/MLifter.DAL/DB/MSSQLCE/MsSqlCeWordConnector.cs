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
using System.Data.SqlServerCe;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE connector for the IWord interface.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-31</remarks>
    class MsSqlCeWordConnector : IDbWordConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeWordConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeWordConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public static MsSqlCeWordConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeWordConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeWordConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbWordConnector Members

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public bool GetDefault(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT is_default FROM TextContent WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            return Convert.ToBoolean(MSSQLCEConn.ExecuteScalar(cmd));
        }

        /// <summary>
        /// Sets the default.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Default">if set to <c>true</c> [default].</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public void SetDefault(int id, bool Default)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE TextContent SET is_default=@isdefault WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            cmd.Parameters.Add("@isdefault", Default);
            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public WordType GetType(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT type FROM TextContent WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            return (WordType)Enum.Parse(typeof(WordType), MSSQLCEConn.ExecuteScalar(cmd).ToString(), true);
        }
        /// <summary>
        /// Sets the type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Type">The type.</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public void SetType(int id, WordType Type)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE TextContent SET type=@typ WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            cmd.Parameters.Add("@typ", Type);
            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public string GetWord(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT text FROM TextContent WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            return MSSQLCEConn.ExecuteScalar(cmd).ToString();
        }
        /// <summary>
        /// Sets the word.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Word">The word.</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public void SetWord(int id, string Word)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE TextContent SET text=@text WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            cmd.Parameters.Add("@text", Word);
            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        #endregion
    }
}
