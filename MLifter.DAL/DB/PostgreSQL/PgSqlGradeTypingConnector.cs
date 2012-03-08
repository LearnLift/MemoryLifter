using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlGradeTypingConnector : IDbGradeTypingConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlGradeTypingConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlGradeTypingConnector>();
        public static PgSqlGradeTypingConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlGradeTypingConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlGradeTypingConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        private void GetSettingsValue(int typeGradingsId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"TypeGradings\" WHERE id=:id";
                    cmd.Parameters.Add("id", typeGradingsId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();

                    int? gid = DbValueConverter.Convert<int>(reader["id"]);
                    bool? allCorrect = DbValueConverter.Convert<bool>(reader["all_correct"]);
                    bool? halfCorrect = DbValueConverter.Convert<bool>(reader["half_correct"]);
                    bool? noneCorrect = DbValueConverter.Convert<bool>(reader["none_correct"]);
                    bool? prompt = DbValueConverter.Convert<bool>(reader["prompt"]);

                    //cache values
                    DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, typeGradingsId, expires)] = allCorrect;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, typeGradingsId, expires)] = halfCorrect;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, typeGradingsId, expires)] = noneCorrect;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, typeGradingsId, expires)] = prompt;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsId, typeGradingsId, expires)] = gid;

                    //set output value
                    switch (cacheObjectType)
                    {
                        case CacheObject.SettingsTypeGradingsAllCorrect: cacheValue = allCorrect; break;
                        case CacheObject.SettingsTypeGradingsHalfCorrect: cacheValue = halfCorrect; break;
                        case CacheObject.SettingsTypeGradingsNoneCorrect: cacheValue = noneCorrect; break;
                        case CacheObject.SettingsTypeGradingsPrompt: cacheValue = prompt; break;
                        case CacheObject.SettingsTypeGradingsId: cacheValue = gid; break;
                        default: cacheValue = null; break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a given grade typing value is cached and outputs the value.
        /// </summary>
        /// <param name="typeGradingsId">The type gradings id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns>[true] if cached.</returns>
        /// <remarks>Documented by Dev03, 2008-11-26</remarks>
        private bool SettingsCached(int typeGradingsId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? typeGradingsIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsTypeGradingsId, typeGradingsId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, typeGradingsId)];
            return typeGradingsIdCached.HasValue && (cacheValue != null);
        }

        #region IDbGradeTypingConnector Members

        public void CheckId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"TypeGradings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public bool? GetAllCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsAllCorrect, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsAllCorrect, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT all_correct FROM \"TypeGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAllCorrect(int id, bool? AllCorrect)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TypeGradings\" SET all_correct=" + (AllCorrect.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (AllCorrect.HasValue)
                        cmd.Parameters.Add("value", AllCorrect);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, id, Cache.DefaultSettingsValidationTime)] = AllCorrect;
                }
            }
        }

        public bool? GetHalfCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsHalfCorrect, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsHalfCorrect, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT half_correct FROM \"TypeGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetHalfCorrect(int id, bool? HalfCorrect)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TypeGradings\" SET half_correct=" + (HalfCorrect.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (HalfCorrect.HasValue)
                        cmd.Parameters.Add("value", HalfCorrect);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, id, Cache.DefaultSettingsValidationTime)] = HalfCorrect;
                }
            }
        }

        public bool? GetNoneCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsNoneCorrect, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsNoneCorrect, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT none_correct FROM \"TypeGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetNoneCorrect(int id, bool? NoneCorrect)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TypeGradings\" SET none_correct=" + (NoneCorrect.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (NoneCorrect.HasValue)
                        cmd.Parameters.Add("value", NoneCorrect);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, id, Cache.DefaultSettingsValidationTime)] = NoneCorrect;
                }
            }
        }

        public bool? GetPrompt(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsPrompt, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsPrompt, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT prompt FROM \"TypeGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetPrompt(int id, bool? Prompt)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"TypeGradings\" SET prompt=" + (Prompt.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Prompt.HasValue)
                        cmd.Parameters.Add("value", Prompt);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, id, Cache.DefaultSettingsValidationTime)] = Prompt;
                }
            }
        }

        #endregion
    }
}
