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
    class PgSqlGradeSynonymsConnector : IDbGradeSynonymsConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlGradeSynonymsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlGradeSynonymsConnector>();
        public static PgSqlGradeSynonymsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlGradeSynonymsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlGradeSynonymsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);            
        }


        private void GetSettingsValue(int synonymsGradingId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"SynonymGradings\" WHERE id=:id";
                    cmd.Parameters.Add("id", synonymsGradingId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();

                    int? sid = DbValueConverter.Convert<int>(reader["id"]);
                    bool? allKnown = DbValueConverter.Convert<bool>(reader["all_known"]);
                    bool? halfKnown = DbValueConverter.Convert<bool>(reader["half_known"]);
                    bool? oneKnown = DbValueConverter.Convert<bool>(reader["one_known"]);
                    bool? firstKnown = DbValueConverter.Convert<bool>(reader["first_known"]);
                    bool? prompt = DbValueConverter.Convert<bool>(reader["prompt"]);

                    //cache values
                    DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, synonymsGradingId, expires)] = allKnown;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, synonymsGradingId, expires)] = halfKnown;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, synonymsGradingId, expires)] = oneKnown;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, synonymsGradingId, expires)] = firstKnown;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, synonymsGradingId, expires)] = prompt;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsId, synonymsGradingId, expires)] = sid;

                    //set output value
                    switch (cacheObjectType)
                    {
                        case CacheObject.SettingsSynonymGradingsAllKnown: cacheValue = allKnown; break;
                        case CacheObject.SettingsSynonymGradingsHalfKnown: cacheValue = halfKnown; break;
                        case CacheObject.SettingsSynonymGradingsOneKnown: cacheValue = oneKnown; break;
                        case CacheObject.SettingsSynonymGradingsFirstKnown: cacheValue = firstKnown; break;
                        case CacheObject.SettingsSynonymGradingsPrompt: cacheValue = prompt; break;
                        case CacheObject.SettingsSynonymGradingsId: cacheValue = sid; break;
                        default: cacheValue = null; break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a given synonym grading value is cached and outputs the value.
        /// </summary>
        /// <param name="synonymsGradingId">The synonyms grading id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns>[true] if cached.</returns>
        /// <remarks>Documented by Dev03, 2008-11-26</remarks>
        private bool SettingsCached(int synonymsGradingId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? synonymsGradingIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsSynonymGradingsId, synonymsGradingId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, synonymsGradingId)];
            return synonymsGradingIdCached.HasValue && (cacheValue != null);
        }


        #region IDbGradeSynonymsConnector Members

        public void CheckId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"SynonymGradings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public bool? GetAllKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsAllKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsAllKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT all_known FROM \"SynonymGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAllKnown(int id, bool? AllKnown)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SynonymGradings\" SET all_known=" + (AllKnown.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (AllKnown.HasValue)
                        cmd.Parameters.Add("value", AllKnown);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, id, Cache.DefaultSettingsValidationTime)] = AllKnown;
                }
            }
        }

        public bool? GetHalfKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsHalfKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsHalfKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT half_known FROM \"SynonymGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetHalfKnown(int id, bool? HalfKnown)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SynonymGradings\" SET half_known=" + (HalfKnown.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (HalfKnown.HasValue)
                        cmd.Parameters.Add("value", HalfKnown);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, id, Cache.DefaultSettingsValidationTime)] = HalfKnown;
                }
            }
        }

        public bool? GetOneKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsOneKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsOneKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT one_known FROM \"SynonymGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetOneKnown(int id, bool? OneKnown)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SynonymGradings\" SET one_known=" + (OneKnown.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (OneKnown.HasValue)
                        cmd.Parameters.Add("value", OneKnown);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, id, Cache.DefaultSettingsValidationTime)] = OneKnown;   
                }
            }
        }

        public bool? GetFirstKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsFirstKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsFirstKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT first_known FROM \"SynonymGradings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetFirstKnown(int id, bool? FirstKnown)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SynonymGradings\" SET first_known=" + (FirstKnown.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (FirstKnown.HasValue)
                        cmd.Parameters.Add("value", FirstKnown);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, id, Cache.DefaultSettingsValidationTime)] = FirstKnown;
                }
            }
        }

        public bool? GetPrompt(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsPrompt, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsPrompt, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT prompt FROM \"SynonymGradings\" WHERE id=:id";
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
                    cmd.CommandText = "UPDATE \"SynonymGradings\" SET prompt=" + (Prompt.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Prompt.HasValue)
                        cmd.Parameters.Add("value", Prompt);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, id, Cache.DefaultSettingsValidationTime)] = Prompt;
                }
            }
        }

        #endregion
    }
}
