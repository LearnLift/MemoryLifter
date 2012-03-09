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
    class PgSqlQueryTypesConnector : IDbQueryTypesConnector
    {        
        private static Dictionary<ConnectionStringStruct, PgSqlQueryTypesConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlQueryTypesConnector>();
        public static PgSqlQueryTypesConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlQueryTypesConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlQueryTypesConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);            
        }

        private void GetSettingsValue(int queryTypeId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"QueryTypes\" WHERE id=:id";
                    cmd.Parameters.Add("id", queryTypeId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();

                    int? qid = DbValueConverter.Convert<int>(reader["id"]);
                    bool? imageRecognition = DbValueConverter.Convert<bool>(reader["image_recognition"]);
                    bool? listeningComprehension = DbValueConverter.Convert<bool>(reader["listening_comprehension"]);
                    bool? multipleChoice = DbValueConverter.Convert<bool>(reader["multiple_choice"]);
                    bool? sentenceMode = DbValueConverter.Convert<bool>(reader["sentence"]);
                    bool? wordMode = DbValueConverter.Convert<bool>(reader["word"]);

                    //cache values
                    DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, queryTypeId, expires)] = imageRecognition;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, queryTypeId, expires)] = listeningComprehension;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, queryTypeId, expires)] = multipleChoice;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, queryTypeId, expires)] = sentenceMode;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, queryTypeId, expires)] = wordMode;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesId, queryTypeId, expires)] = qid;

                    //set output value
                    switch (cacheObjectType)
                    {
                        case CacheObject.SettingsQueryTypesImageRecognition: cacheValue = imageRecognition; break;
                        case CacheObject.SettingsQueryTypesListeningComprehension: cacheValue = listeningComprehension; break;
                        case CacheObject.SettingsQueryTypesMultipleChoice: cacheValue = multipleChoice; break;
                        case CacheObject.SettingsQueryTypesSentence: cacheValue = sentenceMode; break;
                        case CacheObject.SettingsQueryTypesWord: cacheValue = wordMode; break;
                        case CacheObject.SettingsQueryTypesId: cacheValue = qid; break;
                        default: cacheValue = null; break;
                    }
                }
            }
        }

        private bool SettingsCached(int queryTypeId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? queryTypeIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsQueryTypesId, queryTypeId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, queryTypeId)];
            return queryTypeIdCached.HasValue && (cacheValue != null);
        }


        #region IDbQueryTypesConnector Members

        public void CheckId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"QueryTypes\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public bool? GetImageRecognition(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryTypesImageRecognition, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryTypesImageRecognition, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT image_recognition FROM \"QueryTypes\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetImageRecognition(int id, bool? ImageRecognition)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryTypes\" SET image_recognition=" + (ImageRecognition.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (ImageRecognition.HasValue)
                        cmd.Parameters.Add("value", ImageRecognition);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, id, Cache.DefaultSettingsValidationTime)] = ImageRecognition;
                }
            }
        }

        public bool? GetListeningComprehension(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryTypesListeningComprehension, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryTypesListeningComprehension, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT listening_comprehension FROM \"QueryTypes\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetListeningComprehension(int id, bool? ListeningComprehension)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryTypes\" SET listening_comprehension=" + (ListeningComprehension.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (ListeningComprehension.HasValue)
                        cmd.Parameters.Add("value", ListeningComprehension);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, id, Cache.DefaultSettingsValidationTime)] = ListeningComprehension;
                }
            }
        }

        public bool? GetMultipleChoice(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryTypesMultipleChoice, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryTypesMultipleChoice, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT multiple_choice FROM \"QueryTypes\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetMultipleChoice(int id, bool? MultipleChoice)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryTypes\" SET multiple_choice=" + (MultipleChoice.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (MultipleChoice.HasValue)
                        cmd.Parameters.Add("value", MultipleChoice);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, id, Cache.DefaultSettingsValidationTime)] = MultipleChoice;
                }
            }
        }

        public bool? GetSentence(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryTypesSentence, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryTypesSentence, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT sentence FROM \"QueryTypes\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetSentence(int id, bool? Sentence)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryTypes\" SET sentence=" + (Sentence.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Sentence.HasValue)
                        cmd.Parameters.Add("value", Sentence);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, id, Cache.DefaultSettingsValidationTime)] = Sentence;
                }
            }
        }

        public bool? GetWord(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryTypesWord, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryTypesWord, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT word FROM \"QueryTypes\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetWord(int id, bool? Word)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryTypes\" SET word=" + (Word.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Word.HasValue)
                        cmd.Parameters.Add("value", Word);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, id, Cache.DefaultSettingsValidationTime)] = Word;
                }
            }
        }

        #endregion
    }
}
