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
    /// MsSqlCeDatabaseConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-12</remarks>
    class MsSqlCeDatabaseConnector : IDbDatabaseConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeDatabaseConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeDatabaseConnector>();
        public static MsSqlCeDatabaseConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeDatabaseConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass parent;
        private MsSqlCeDatabaseConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbDatabaseConnector Members

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetDatabaseVersion()
        {
            string version = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
            if (version != null && version.Length > 0)
                return version;

            GetDatabaseValues(parent);

            return parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
        }

        /// <summary>
        /// Gets the supported data layer versions.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public List<string> GetSupportedDLVersions()
        {
            List<string> versions = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SupportedDataLayerVersions, 0)] as List<string>;
            if (versions != null && versions.Count > 0)
                return versions;

            GetDatabaseValues(parent);

            return parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SupportedDataLayerVersions, 0)] as List<string>;
        }

        #endregion

        /// <summary>
        /// Gets the database values.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public static void GetDatabaseValues(ParentClass parent)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT * FROM \"DatabaseInformation\"";
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

                while (reader.Read())
                {
                    switch ((DataBaseInformation)Enum.Parse(typeof(DataBaseInformation), reader["property"].ToString()))
                    {
                        case DataBaseInformation.Version:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DataBaseVersion, 0, new TimeSpan(1, 0, 0))] = reader["value"].ToString();
                            break;
                        case DataBaseInformation.SupportedDataLayerVersions:
                            string vrs = reader["value"].ToString();
                            if (vrs.Length > 0)
                            {
                                List<string> versions = new List<string>();
                                versions.AddRange(vrs.Split(new char[] { ',' }));
                                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SupportedDataLayerVersions, 0, new TimeSpan(1, 0, 0))] = versions;
                            }
                            break;
                        case DataBaseInformation.ListAuthentication:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ListAuthentication, 0, new TimeSpan(1, 0, 0))] = (bool?)Convert.ToBoolean(reader["value"]);
                            break;
                        case DataBaseInformation.FormsAuthentication:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.FormsAuthentication, 0, new TimeSpan(1, 0, 0))] = (bool?)Convert.ToBoolean(reader["value"]);
                            break;
                        case DataBaseInformation.LocalDirectoryAuthentication:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LocalDirectoryAuthentication, 0, new TimeSpan(1, 0, 0))] = (bool?)Convert.ToBoolean(reader["value"]);
                            break;
                        case DataBaseInformation.LocalDirectoryType:
                            try
                            {
                                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LocalDirectoryType, 0, new TimeSpan(1, 0, 0))] =
                                    (LocalDirectoryType)Enum.Parse(typeof(LocalDirectoryType), reader["value"].ToString());
                            }
                            catch (Exception)
                            {
                                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LocalDirectoryType, 0, new TimeSpan(1, 0, 0))] =
                                    LocalDirectoryType.ActiveDirectory;
                            }
                            break;
                        case DataBaseInformation.LdapServer:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapServer, 0, new TimeSpan(1, 0, 0))] = reader["value"].ToString();
                            break;
                        case DataBaseInformation.LdapPort:
                            try
                            {
                                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapPort, 0, new TimeSpan(1, 0, 0))] = Convert.ToInt32(reader["value"]);
                            }
                            catch
                            {
                                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapPort, 0, new TimeSpan(1, 0, 0))] = 0;
                            }
                            break;
                        case DataBaseInformation.LdapUser:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapUser, 0, new TimeSpan(1, 0, 0))] = reader["value"].ToString();
                            break;
                        case DataBaseInformation.LdapPassword:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapPassword, 0, new TimeSpan(1, 0, 0))] = reader["value"].ToString();
                            break;
                        case DataBaseInformation.LdapContext:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapContext, 0, new TimeSpan(1, 0, 0))] = reader["value"].ToString();
                            break;
                        case DataBaseInformation.LdapUseSsl:
                            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapUseSsl, 0, new TimeSpan(1, 0, 0))] = (bool?)Convert.ToBoolean(reader["value"]);
                            break;
                        default:
                            break;
                    }
                }
                reader.Close();
            }
        }
    }
}
