using System;
using System.Collections.Generic;
using System.Text;

using Npgsql;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.DB.PostgreSQL
{
    internal class PgSqlDatabaseConnector : IDbDatabaseConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlDatabaseConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlDatabaseConnector>();
        public static PgSqlDatabaseConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new PgSqlDatabaseConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private PgSqlDatabaseConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbDatabaseConnector Members

        public string GetDatabaseVersion()
        {
            string version = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
            if (version != null && version.Length > 0)
                return version;

            GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
        }

        public List<string> GetSupportedDLVersions()
        {
            List<string> versions = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SupportedDataLayerVersions, 0)] as List<string>;
            if (versions != null && versions.Count > 0)
                return versions;

            GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SupportedDataLayerVersions, 0)] as List<string>;
        }

        #endregion

        /// <summary>
        /// Gets the database values from the DatabaseInformation table.
        /// </summary>
        /// <remarks>Documented by Dev05, 2008-09-12</remarks>
        public static void GetDatabaseValues(ParentClass parent)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"DatabaseInformation\"";
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, parent.CurrentUser, false);

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
                                    parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LdapPort, 0, new TimeSpan(1, 0, 0))] = 
                                        Convert.ToInt32(reader["value"]);
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
                }
            }
        }
    }
}
