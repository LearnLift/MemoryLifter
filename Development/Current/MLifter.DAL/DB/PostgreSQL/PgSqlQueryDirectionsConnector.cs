using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlQueryDirectionsConnector : IDbQueryDirectionsConnector
    {        
        private static Dictionary<ConnectionStringStruct, PgSqlQueryDirectionsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlQueryDirectionsConnector>();
        public static PgSqlQueryDirectionsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlQueryDirectionsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlQueryDirectionsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        private void GetSettingsValue(int queryDirectionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"QueryDirections\" WHERE id=:id";
                    cmd.Parameters.Add("id", queryDirectionsId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();

                    int? did = DbValueConverter.Convert<int>(reader["id"]);
                    bool? questionToAnswer = DbValueConverter.Convert<bool>(reader["question2answer"]);
                    bool? answerToQuestion = DbValueConverter.Convert<bool>(reader["answer2question"]);
                    bool? mixed = DbValueConverter.Convert<bool>(reader["mixed"]);

                    //cache values
                    DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, queryDirectionsId, expires)] = questionToAnswer;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, queryDirectionsId, expires)] = answerToQuestion;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, queryDirectionsId, expires)] = mixed;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsId, queryDirectionsId, expires)] = did;

                    //set output value
                    switch (cacheObjectType)
                    {
                        case CacheObject.SettingsQueryDirectionsQuestion2Answer: cacheValue = questionToAnswer; break;
                        case CacheObject.SettingsQueryDirectionsAnswer2Question: cacheValue = answerToQuestion; break;
                        case CacheObject.SettingsQueryDirectionsMixed: cacheValue = mixed; break;
                        case CacheObject.SettingsQueryDirectionsId: cacheValue = did; break;
                        default: cacheValue = null; break;
                    }
                }
            }
        }

        private bool SettingsCached(int queryDirectionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? queryDirectionsIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsQueryDirectionsId, queryDirectionsId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, queryDirectionsId)];
            return queryDirectionsIdCached.HasValue && (cacheValue != null);
        }

        
        #region IDbQueryDirectionsConnector Members

        public void CheckId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"QueryDirections\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public bool? GetQuestion2Answer(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsQuestion2Answer, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsQuestion2Answer, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT question2answer FROM \"QueryDirections\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetQuestion2Answer(int id, bool? Question2Answer)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryDirections\" SET question2answer=" + (Question2Answer.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Question2Answer.HasValue)
                        cmd.Parameters.Add("value", Question2Answer);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, id, Cache.DefaultSettingsValidationTime)] = Question2Answer;
                }
            }
        }

        public bool? GetAnswer2Question(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsAnswer2Question, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsAnswer2Question, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT answer2question FROM \"QueryDirections\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAnswer2Question(int id, bool? Answer2Question)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryDirections\" SET answer2question=" + (Answer2Question.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Answer2Question.HasValue)
                        cmd.Parameters.Add("value", Answer2Question);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, id, Cache.DefaultSettingsValidationTime)] = Answer2Question;
                }
            }
        }

        public bool? GetMixed(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsMixed, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsMixed, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT mixed FROM \"QueryDirections\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetMixed(int id, bool? Mixed)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"QueryDirections\" SET mixed=" + (Mixed.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Mixed.HasValue)
                        cmd.Parameters.Add("value", Mixed);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, id, Cache.DefaultSettingsValidationTime)] = Mixed;
                }
            }
        }

        #endregion
    }
}
