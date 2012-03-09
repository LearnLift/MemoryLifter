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
    /// MsSqlCeSnoozeOptionsConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-13</remarks>
    class MsSqlCeSnoozeOptionsConnector : IDbSnoozeOptionsConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeSnoozeOptionsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeSnoozeOptionsConnector>();
        public static MsSqlCeSnoozeOptionsConnector GetInstance(ParentClass ParentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = ParentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeSnoozeOptionsConnector(ParentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeSnoozeOptionsConnector(ParentClass ParentClass)
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
        /// <param name="snoozeOptionsId">The snooze options id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private void GetSettingsValue(int snoozeOptionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT * FROM \"SnoozeOptions\" WHERE id=@id";
            cmd.Parameters.Add("@id", snoozeOptionsId);

            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
            reader.Read();

            int? sid = DbValueConverter.Convert<int>(reader["id"]);
            bool? cardsEnabled = DbValueConverter.Convert<bool>(reader["cards_enabled"]);
            bool? rightsEnabled = DbValueConverter.Convert<bool>(reader["rights_enabled"]);
            bool? timeEnabled = DbValueConverter.Convert<bool>(reader["time_enabled"]);
            int? snoozeCards = DbValueConverter.Convert<int>(reader["snooze_cards"]);
            int? snoozeHigh = DbValueConverter.Convert<int>(reader["snooze_high"]);
            int? snoozeLow = DbValueConverter.Convert<int>(reader["snooze_low"]);
            ESnoozeMode? snoozeMode = DbValueConverter.Convert<ESnoozeMode>(reader["snooze_mode"]);
            int? snoozeRights = DbValueConverter.Convert<int>(reader["snooze_rights"]);
            int? snoozeTime = DbValueConverter.Convert<int>(reader["snooze_time"]);
            reader.Close();

            //cache values
            DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCardsEnabled, snoozeOptionsId, expires)] = cardsEnabled;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRightsEnabled, snoozeOptionsId, expires)] = rightsEnabled;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTimeEnabled, snoozeOptionsId, expires)] = timeEnabled;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCards, snoozeOptionsId, expires)] = snoozeCards;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeHigh, snoozeOptionsId, expires)] = snoozeHigh;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeLow, snoozeOptionsId, expires)] = snoozeLow;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeMode, snoozeOptionsId, expires)] = snoozeMode;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRights, snoozeOptionsId, expires)] = snoozeRights;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTime, snoozeOptionsId, expires)] = snoozeTime;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeOptionsId, snoozeOptionsId, expires)] = sid;

            //set output value
            switch (cacheObjectType)
            {
                case CacheObject.SettingsSnoozeCardsEnabled: cacheValue = cardsEnabled; break;
                case CacheObject.SettingsSnoozeRightsEnabled: cacheValue = rightsEnabled; break;
                case CacheObject.SettingsSnoozeTimeEnabled: cacheValue = timeEnabled; break;
                case CacheObject.SettingsSnoozeCards: cacheValue = snoozeCards; break;
                case CacheObject.SettingsSnoozeHigh: cacheValue = snoozeHigh; break;
                case CacheObject.SettingsSnoozeLow: cacheValue = snoozeLow; break;
                case CacheObject.SettingsSnoozeMode: cacheValue = snoozeMode; break;
                case CacheObject.SettingsSnoozeRights: cacheValue = snoozeRights; break;
                case CacheObject.SettingsSnoozeTime: cacheValue = snoozeTime; break;
                case CacheObject.SettingsSnoozeOptionsId: cacheValue = sid; break;
                default: cacheValue = null; break;
            }
        }

        /// <summary>
        /// Settingses the cached.
        /// </summary>
        /// <param name="snoozeOptionsId">The snooze options id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private bool SettingsCached(int snoozeOptionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? snoozeOptionsIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsSnoozeOptionsId, snoozeOptionsId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, snoozeOptionsId)];
            //return snoozeOptionsIdCached.HasValue && (cacheValue != null);
            return snoozeOptionsIdCached.HasValue;
        }

        #region IDbSnoozeOptionsConnector Members

        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void CheckId(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT count(*) FROM \"SnoozeOptions\" WHERE id=@id";
            cmd.Parameters.Add("@id", id);

            if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                throw new IdAccessException(id);
        }

        /// <summary>
        /// Gets the cards enabled.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetCardsEnabled(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeCardsEnabled, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeCardsEnabled, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the cards enabled.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cardsEnabled">The cards enabled.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetCardsEnabled(int id, bool? cardsEnabled)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET cards_enabled=" + (cardsEnabled.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (cardsEnabled.HasValue)
                cmd.Parameters.Add("@value", cardsEnabled);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCardsEnabled, id, Cache.DefaultSettingsValidationTime)] = cardsEnabled;
        }

        /// <summary>
        /// Gets the rights enabled.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetRightsEnabled(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeRightsEnabled, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeRightsEnabled, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the rights enabled.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="rightsEnabled">The rights enabled.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetRightsEnabled(int id, bool? rightsEnabled)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET rights_enabled=" + (rightsEnabled.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (rightsEnabled.HasValue)
                cmd.Parameters.Add("@value", rightsEnabled);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRightsEnabled, id, Cache.DefaultSettingsValidationTime)] = rightsEnabled;
        }

        /// <summary>
        /// Gets the time enabled.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetTimeEnabled(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeTimeEnabled, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeTimeEnabled, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the time enabled.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="timeEnabled">The time enabled.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetTimeEnabled(int id, bool? timeEnabled)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET time_enabled=" + (timeEnabled.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (timeEnabled.HasValue)
                cmd.Parameters.Add("@value", timeEnabled);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTimeEnabled, id, Cache.DefaultSettingsValidationTime)] = timeEnabled;
        }

        /// <summary>
        /// Gets the snooze cards.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetSnoozeCards(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeCards, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeCards, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the snooze cards.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="snoozeCards">The snooze cards.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetSnoozeCards(int id, int? snoozeCards)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_cards=" + (snoozeCards.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (snoozeCards.HasValue)
                cmd.Parameters.Add("@value", snoozeCards);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCards, id, Cache.DefaultSettingsValidationTime)] = snoozeCards;
        }

        /// <summary>
        /// Gets the snooze high.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetSnoozeHigh(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeHigh, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeHigh, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the snooze high.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="snoozeHigh">The snooze high.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetSnoozeHigh(int id, int? snoozeHigh)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_high=" + (snoozeHigh.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (snoozeHigh.HasValue)
                cmd.Parameters.Add("@value", snoozeHigh);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeHigh, id, Cache.DefaultSettingsValidationTime)] = snoozeHigh;
        }

        /// <summary>
        /// Gets the snooze low.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetSnoozeLow(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeLow, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeLow, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the snooze low.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="snoozeLow">The snooze low.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetSnoozeLow(int id, int? snoozeLow)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_low=" + (snoozeLow.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (snoozeLow.HasValue)
                cmd.Parameters.Add("@value", snoozeLow);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeLow, id, Cache.DefaultSettingsValidationTime)] = snoozeLow;
        }

        /// <summary>
        /// Gets the snooze mode.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public ESnoozeMode? GetSnoozeMode(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeMode, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeMode, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as ESnoozeMode?;
        }
        /// <summary>
        /// Sets the snooze mode.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="snoozeMode">The snooze mode.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetSnoozeMode(int id, ESnoozeMode? snoozeMode)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_mode=" + (snoozeMode.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (snoozeMode.HasValue)
                cmd.Parameters.Add("@value", snoozeMode.Value.ToString());

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeMode, id, Cache.DefaultSettingsValidationTime)] = snoozeMode;
        }

        /// <summary>
        /// Gets the snooze rights.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetSnoozeRights(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeRights, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeRights, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the snooze rights.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="snoozeRights">The snooze rights.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetSnoozeRights(int id, int? snoozeRights)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_rights=" + (snoozeRights.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (snoozeRights.HasValue)
                cmd.Parameters.Add("@value", snoozeRights);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRights, id, Cache.DefaultSettingsValidationTime)] = snoozeRights;
        }

        /// <summary>
        /// Gets the snooze time.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int? GetSnoozeTime(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeTime, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeTime, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;
        }
        /// <summary>
        /// Sets the snooze time.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="snoozeTime">The snooze time.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetSnoozeTime(int id, int? snoozeTime)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_time=" + (snoozeTime.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (snoozeTime.HasValue)
                cmd.Parameters.Add("@value", snoozeTime);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTime, id, Cache.DefaultSettingsValidationTime)] = snoozeTime;
        }

        #endregion
    }
}
