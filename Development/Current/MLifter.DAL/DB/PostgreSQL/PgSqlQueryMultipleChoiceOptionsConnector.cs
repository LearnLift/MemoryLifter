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
using MLifter.DAL.DB.PostgreSQL;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlQueryMultipleChoiceOptionsConnector : IDbQueryMultipleChoiceOptionsConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlQueryMultipleChoiceOptionsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlQueryMultipleChoiceOptionsConnector>();
        public static PgSqlQueryMultipleChoiceOptionsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlQueryMultipleChoiceOptionsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlQueryMultipleChoiceOptionsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }


        private void GetSettingsValue(int multipleChoiceId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"MultipleChoiceOptions\" WHERE id=:id";
                    cmd.Parameters.Add("id", multipleChoiceId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();

                    int? mid = DbValueConverter.Convert<int>(reader["id"]);
                    bool? allowMultipleCorrectAnswers = DbValueConverter.Convert<bool>(reader["allow_multiple_correct_answers"]);
                    bool? allowRandomDistractors = DbValueConverter.Convert<bool>(reader["allow_random_distractors"]);
                    int? maxCorrectAnswers = DbValueConverter.Convert<int>(reader["max_correct_answers"]);
                    int? numberOfChoices = DbValueConverter.Convert<int>(reader["number_of_choices"]);

                    //cache values
                    DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, multipleChoiceId, expires)] = allowMultipleCorrectAnswers;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, multipleChoiceId, expires)] = allowRandomDistractors;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, multipleChoiceId, expires)] = maxCorrectAnswers;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, multipleChoiceId, expires)] = numberOfChoices;
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsId, multipleChoiceId, expires)] = mid;

                    //set output value
                    switch (cacheObjectType)
                    {
                        case CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers: cacheValue = allowMultipleCorrectAnswers; break;
                        case CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors: cacheValue = allowRandomDistractors; break;
                        case CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers: cacheValue = maxCorrectAnswers; break;
                        case CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices: cacheValue = numberOfChoices; break;
                        case CacheObject.SettingsMultipleChoiceOptionsId: cacheValue = mid; break;
                        default: cacheValue = null; break;
                    }
                }
            }
        }

        private bool SettingsCached(int multipleChoiceId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? multipleChoiceIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsMultipleChoiceOptionsId, multipleChoiceId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, multipleChoiceId)];
            return multipleChoiceIdCached.HasValue && (cacheValue != null);
        }


        #region IDbQueryMultipleChoiceOptionsConnector Members

        public void CheckId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"MultipleChoiceOptions\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public bool? GetAllowMultiple(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT allow_multiple_correct_answers FROM \"MultipleChoiceOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAllowMultiple(int id, bool? AllowMultiple)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET allow_multiple_correct_answers=" + (AllowMultiple.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (AllowMultiple.HasValue)
                        cmd.Parameters.Add("value", AllowMultiple);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, id, Cache.DefaultSettingsValidationTime)] = AllowMultiple;
                }
            }
        }

        public bool? GetAllowRandom(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT allow_random_distractors FROM \"MultipleChoiceOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAllowRandom(int id, bool? AllowRandom)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET allow_random_distractors=" + (AllowRandom.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (AllowRandom.HasValue)
                        cmd.Parameters.Add("value", AllowRandom);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, id, Cache.DefaultSettingsValidationTime)] = AllowRandom;
                }
            }
        }

        public int? GetMaxCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT max_correct_answers FROM \"MultipleChoiceOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetMaxCorrect(int id, int? MaxCorrect)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET max_correct_answers=" + (MaxCorrect.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (MaxCorrect.HasValue)
                        cmd.Parameters.Add("value", MaxCorrect);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, id, Cache.DefaultSettingsValidationTime)] = MaxCorrect;
                }
            }
        }

        public int? GetChoices(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT number_of_choices FROM \"MultipleChoiceOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetChoices(int id, int? Choices)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET number_of_choices=" + (Choices.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Choices.HasValue)
                        cmd.Parameters.Add("value", Choices);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, id, Cache.DefaultSettingsValidationTime)] = Choices;
                }
            }
        }

        #endregion
    }
}
