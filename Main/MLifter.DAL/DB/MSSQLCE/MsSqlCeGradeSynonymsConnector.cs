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
    /// MsSqlCeGradeSynonymsConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-13</remarks>
    class MsSqlCeGradeSynonymsConnector : IDbGradeSynonymsConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeGradeSynonymsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeGradeSynonymsConnector>();
        public static MsSqlCeGradeSynonymsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeGradeSynonymsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass parent;
        private MsSqlCeGradeSynonymsConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        /// <summary>
        /// Gets the settings value.
        /// </summary>
        /// <param name="synonymsGradingId">The synonyms grading id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private void GetSettingsValue(int synonymsGradingId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT * FROM \"SynonymGradings\" WHERE id=@id";
                cmd.Parameters.Add("@id", synonymsGradingId);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                reader.Read();

                int? sid = DbValueConverter.Convert<int>(reader["id"]);
                bool? allKnown = DbValueConverter.Convert<bool>(reader["all_known"]);
                bool? halfKnown = DbValueConverter.Convert<bool>(reader["half_known"]);
                bool? oneKnown = DbValueConverter.Convert<bool>(reader["one_known"]);
                bool? firstKnown = DbValueConverter.Convert<bool>(reader["first_known"]);
                bool? prompt = DbValueConverter.Convert<bool>(reader["prompt"]);
                reader.Close();

                //save all values to cache
                DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, synonymsGradingId, expires)] = allKnown;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, synonymsGradingId, expires)] = halfKnown;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, synonymsGradingId, expires)] = oneKnown;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, synonymsGradingId, expires)] = firstKnown;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, synonymsGradingId, expires)] = prompt;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsId, synonymsGradingId, expires)] = sid;

                //set output value (the call by reference parameter)
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

        /// <summary>
        /// Settingses the cached.
        /// </summary>
        /// <param name="synonymsGradingId">The synonyms grading id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private bool SettingsCached(int synonymsGradingId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? synonymsGradingIdCached = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsSynonymGradingsId, synonymsGradingId)] as int?;
            cacheValue = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, synonymsGradingId)];
            return synonymsGradingIdCached.HasValue && (cacheValue != null);
        }

        #region IDbGradeSynonymsConnector Members

        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void CheckId(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"SynonymGradings\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);

                if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                    throw new IdAccessException(id);
            }
        }

        /// <summary>
        /// Gets all known.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetAllKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsAllKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsAllKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        /// <summary>
        /// Sets all known.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="AllKnown">All known.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetAllKnown(int id, bool? AllKnown)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"SynonymGradings\" SET all_known=" + (AllKnown.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (AllKnown.HasValue)
                    cmd.Parameters.Add("@value", AllKnown);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, id, Cache.DefaultSettingsValidationTime)] = AllKnown;
            }
        }

        /// <summary>
        /// Gets the half known.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetHalfKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsHalfKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsHalfKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        /// <summary>
        /// Sets the halfknown.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="halfKnown">The half known.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetHalfKnown(int id, bool? halfKnown)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"SynonymGradings\" SET half_known=" + (halfKnown.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (halfKnown.HasValue)
                    cmd.Parameters.Add("@value", halfKnown);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, id, Cache.DefaultSettingsValidationTime)] = halfKnown;
            }
        }

        /// <summary>
        /// Gets the one known.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetOneKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsOneKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsOneKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        /// <summary>
        /// Sets the one known.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="oneKnown">The one known.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetOneKnown(int id, bool? oneKnown)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"SynonymGradings\" SET one_known=" + (oneKnown.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (oneKnown.HasValue)
                    cmd.Parameters.Add("@value", oneKnown);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, id, Cache.DefaultSettingsValidationTime)] = oneKnown;
            }
        }

        /// <summary>
        /// Gets the first known.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetFirstKnown(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsFirstKnown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsFirstKnown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        public void SetFirstKnown(int id, bool? firstKnown)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"SynonymGradings\" SET first_known=" + (firstKnown.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (firstKnown.HasValue)
                    cmd.Parameters.Add("@value", firstKnown);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, id, Cache.DefaultSettingsValidationTime)] = firstKnown;
            }
        }

        /// <summary>
        /// Gets the prompt.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetPrompt(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsPrompt, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsPrompt, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        /// <summary>
        /// Sets the prompt.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="prompt">The prompt.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetPrompt(int id, bool? prompt)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"SynonymGradings\" SET prompt=" + (prompt.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (prompt.HasValue)
                    cmd.Parameters.Add("@value", prompt);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, id, Cache.DefaultSettingsValidationTime)] = prompt;
            }
        }

        #endregion
    }
}
