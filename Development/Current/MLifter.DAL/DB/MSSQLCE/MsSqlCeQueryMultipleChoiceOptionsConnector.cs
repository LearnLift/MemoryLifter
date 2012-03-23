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
    class MsSqlCeQueryMultipleChoiceOptionsConnector : IDbQueryMultipleChoiceOptionsConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeQueryMultipleChoiceOptionsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeQueryMultipleChoiceOptionsConnector>();
        public static MsSqlCeQueryMultipleChoiceOptionsConnector GetInstance(ParentClass ParentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = ParentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeQueryMultipleChoiceOptionsConnector(ParentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeQueryMultipleChoiceOptionsConnector(ParentClass ParentClass)
        {
            Parent = ParentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent Parent = sender as IParent;
            instances.Remove(Parent.Parent.CurrentUser.ConnectionString);
        }

        /// <summary>
        /// Gets the settings value.
        /// </summary>
        /// <param name="multipleChoiceId">The multiple choice id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private void GetSettingsValue(int multipleChoiceId, CacheObject cacheObjectType, out object cacheValue)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT * FROM \"MultipleChoiceOptions\" WHERE id=@id";
            cmd.Parameters.Add("@id", multipleChoiceId);

            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
            reader.Read();

            int? mid = DbValueConverter.Convert<int>(reader["id"]);
            bool? allowMultipleCorrectAnswers = DbValueConverter.Convert<bool>(reader["allow_multiple_correct_answers"]);
            bool? allowRandomDistractors = DbValueConverter.Convert<bool>(reader["allow_random_distractors"]);
            int? maxCorrectAnswers = DbValueConverter.Convert<int>(reader["max_correct_answers"]);
            int? numberOfChoices = DbValueConverter.Convert<int>(reader["number_of_choices"]);
            reader.Close();

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

        /// <summary>
        /// Settingses the cached.
        /// </summary>
        /// <param name="multipleChoiceId">The multiple choice id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private bool SettingsCached(int multipleChoiceId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? multipleChoiceIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsMultipleChoiceOptionsId, multipleChoiceId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, multipleChoiceId)];
            return multipleChoiceIdCached.HasValue;// && (cacheValue != null);
        }

        #region IDbQueryMultipleChoiceOptionsConnector Members

        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void CheckId(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT count(*) FROM \"MultipleChoiceOptions\" WHERE id=@id";
            cmd.Parameters.Add("@id", id);

            if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                throw new IdAccessException(id);
        }

        public bool? GetAllowMultiple(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the allow multiple.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="allowMultiple">The allow multiple.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetAllowMultiple(int id, bool? allowMultiple)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET allow_multiple_correct_answers=" + (allowMultiple.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (allowMultiple.HasValue)
                cmd.Parameters.Add("@value", allowMultiple);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, id, Cache.DefaultSettingsValidationTime)] = allowMultiple;
        }

        /// <summary>
        /// Gets the allow random.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetAllowRandom(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the allow random.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="allowRandom">The allow random.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetAllowRandom(int id, bool? allowRandom)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET allow_random_distractors=" + (allowRandom.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (allowRandom.HasValue)
                cmd.Parameters.Add("@value", allowRandom);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, id, Cache.DefaultSettingsValidationTime)] = allowRandom;
        }

        /// <summary>
        /// Gets the max correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetMaxCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the max correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="maxCorrect">The max correct.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetMaxCorrect(int id, int? maxCorrect)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET max_correct_answers=" + (maxCorrect.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (maxCorrect.HasValue)
                cmd.Parameters.Add("@value", maxCorrect);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, id, Cache.DefaultSettingsValidationTime)] = maxCorrect;
        }

        /// <summary>
        /// Gets the choices.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetChoices(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the choices.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="choices">The choices.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetChoices(int id, int? choices)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET number_of_choices=" + (choices.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (choices.HasValue)
                cmd.Parameters.Add("@value", choices);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, id, Cache.DefaultSettingsValidationTime)] = choices;
        }

        #endregion
    }
}
