using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlDictionariesConnector : IDbDictionariesConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlDictionariesConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlDictionariesConnector>();
        public static PgSqlDictionariesConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new PgSqlDictionariesConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private PgSqlDictionariesConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbDictionariesConnector Members

        public IList<int> GetLMIds()
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    IList<int> ids = new List<int>();

                    cmd.CommandText = "SELECT * FROM \"LearningModules\"";
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    int id;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader["id"]);

                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DefaultLearningModuleSettings, id, new TimeSpan(0, 10, 0))] =
                            new DbSettings(Convert.ToInt32(reader["default_settings_id"]), false, Parent);

                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LearningModuleAuthor, id)] = Convert.ToString(reader["author"]);
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LearningModuleTitle, id)] = Convert.ToString(reader["title"]);

                        ids.Add(id);
                    }

                    return ids;
                }
            }
        }

        public string GetDbVersion()
        {
            string version = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
            if (version != null && version.Length > 0)
                return version;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
        }

        public void DeleteLM(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"DeleteLearningModule\"(:id); ";
                    cmd.Parameters.Add("id", id);
                    cmd.CommandTimeout = 240; //ToDo: Optimize speed
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public int AddNewLM(string guid, int categoryId, string title, string licenceKey, bool contentProtected, int calCount)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                int? lmId;

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"CreateNewLearningModule\"(:guid, (SELECT id FROM  \"Categories\" WHERE global_id=:categoryid), :title)";
                    cmd.Parameters.Add("guid", guid);
                    cmd.Parameters.Add("categoryid", categoryId);
                    cmd.Parameters.Add("title", title);
                    lmId = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                }

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET licence_key=:lk, content_protected=:cp, cal_count=:cals WHERE id=:id";
                    cmd.Parameters.Add("id", lmId);
                    cmd.Parameters.Add("lk", licenceKey);
                    cmd.Parameters.Add("cp", contentProtected);
                    cmd.Parameters.Add("cals", calCount);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                return lmId.Value;
            }
        }

        public int GetLMCount()
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) as count FROM \"LearningModules\"";
                    return Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }

        public IList<Guid> GetExtensions()
        {
            IList<Guid> guids = new List<Guid>();
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT guid FROM \"Extensions\"";
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    while (reader.Read())
                        guids.Add(new Guid(reader["guid"] as string));
                }
            }

            return guids;
        }

        #endregion
    }
}
