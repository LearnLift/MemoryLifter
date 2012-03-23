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
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    /// <summary>
    /// The PostgreSQL connector for the IWords interface.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-31</remarks>
    class PgSqlWordConnector : IDbWordConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlWordConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlWordConnector>();
        public static PgSqlWordConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlWordConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlWordConnector(ParentClass parentClass)
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

        public bool GetDefault(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT is_default FROM \"TextContent\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    return Convert.ToBoolean(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }
        public void SetDefault(int id, bool Default)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TextContent\" SET is_default=:isdefault WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("isdefault", Default);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public WordType GetType(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT type FROM \"TextContent\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    return (WordType)Enum.Parse(typeof(WordType), PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString(), true);
                }
            }
        }
        public void SetType(int id, WordType Type)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TextContent\" SET type=:typ WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("typ", Type);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public string GetWord(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT text FROM \"TextContent\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    return PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString();
                }
            }
        }
        public void SetWord(int id, string Word)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TextContent\" SET text=:text WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("text", Word);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        #endregion
    }
}
