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
    /// MsSqlCeGradeTypingConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-13</remarks>
    class MsSqlCeGradeTypingConnector : IDbGradeTypingConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeGradeTypingConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeGradeTypingConnector>();
        public static MsSqlCeGradeTypingConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeGradeTypingConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass parent;
        private MsSqlCeGradeTypingConnector(ParentClass parentClass)
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
        /// <param name="typeGradingsId">The type gradings id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private void GetSettingsValue(int typeGradingsId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT * FROM \"TypeGradings\" WHERE id=@id";
                cmd.Parameters.Add("@id", typeGradingsId);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                reader.Read();

                int? gid = DbValueConverter.Convert<int>(reader["id"]);
                bool? allCorrect = DbValueConverter.Convert<bool>(reader["all_correct"]);
                bool? halfCorrect = DbValueConverter.Convert<bool>(reader["half_correct"]);
                bool? noneCorrect = DbValueConverter.Convert<bool>(reader["none_correct"]);
                bool? prompt = DbValueConverter.Convert<bool>(reader["prompt"]);
                reader.Close();

                //cache values
                DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, typeGradingsId, expires)] = allCorrect;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, typeGradingsId, expires)] = halfCorrect;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, typeGradingsId, expires)] = noneCorrect;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, typeGradingsId, expires)] = prompt;
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsId, typeGradingsId, expires)] = gid;

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

        /// <summary>
        /// Settingses the cached.
        /// </summary>
        /// <param name="typeGradingsId">The type gradings id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private bool SettingsCached(int typeGradingsId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? typeGradingsIdCached = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsTypeGradingsId, typeGradingsId)] as int?;
            cacheValue = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, typeGradingsId)];
            return typeGradingsIdCached.HasValue && (cacheValue != null);
        }

        #region IDbGradeTypingConnector Members

        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void CheckId(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"TypeGradings\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);

                if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                    throw new IdAccessException(id);
            }
        }

        /// <summary>
        /// Gets all correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetAllCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsAllCorrect, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsAllCorrect, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

		/// <summary>
		/// Sets all correct.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="allCorrect">All correct.</param>
		/// <remarks>
		/// Documented by FabThe, 13.1.2009
		/// </remarks>
        public void SetAllCorrect(int id, bool? allCorrect)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"TypeGradings\" SET all_correct=" + (allCorrect.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (allCorrect.HasValue)
                    cmd.Parameters.Add("@value", allCorrect);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, id, Cache.DefaultSettingsValidationTime)] = allCorrect;
            }
        }

        /// <summary>
        /// Gets the half correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetHalfCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsHalfCorrect, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsHalfCorrect, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        /// <summary>
        /// Sets the half correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="halfCorrect">The half correct.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetHalfCorrect(int id, bool? halfCorrect)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"TypeGradings\" SET half_correct=" + (halfCorrect.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("id", id);
                if (halfCorrect.HasValue)
                    cmd.Parameters.Add("value", halfCorrect);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, id, Cache.DefaultSettingsValidationTime)] = halfCorrect;
            }
        }

        /// <summary>
        /// Gets the none correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetNoneCorrect(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsNoneCorrect, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsNoneCorrect, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        /// <summary>
        /// Sets the none correct.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="noneCorrect">The none correct.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetNoneCorrect(int id, bool? noneCorrect)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"TypeGradings\" SET none_correct=" + (noneCorrect.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (noneCorrect.HasValue)
                    cmd.Parameters.Add("@value", noneCorrect);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, id, Cache.DefaultSettingsValidationTime)] = noneCorrect;
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
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsPrompt, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsPrompt, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }

        public void SetPrompt(int id, bool? prompt)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"TypeGradings\" SET prompt=" + (prompt.HasValue ? "@value" : "null") + " WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (prompt.HasValue)
                    cmd.Parameters.Add("@value", prompt);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                //Save to Cache
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, id, Cache.DefaultSettingsValidationTime)] = prompt;
            }
        }

        #endregion
    }
}
