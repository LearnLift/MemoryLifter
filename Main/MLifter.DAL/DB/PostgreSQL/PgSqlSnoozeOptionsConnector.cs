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
    class PgSqlSnoozeOptionsConnector : IDbSnoozeOptionsConnector
    {        
        private static Dictionary<ConnectionStringStruct, PgSqlSnoozeOptionsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlSnoozeOptionsConnector>();
        public static PgSqlSnoozeOptionsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlSnoozeOptionsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlSnoozeOptionsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        private void GetSettingsValue(int snoozeOptionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"SnoozeOptions\" WHERE id=:id";
                    cmd.Parameters.Add("id", snoozeOptionsId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
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
            }
        }

        private bool SettingsCached(int snoozeOptionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? snoozeOptionsIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsSnoozeOptionsId, snoozeOptionsId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, snoozeOptionsId)];
            return snoozeOptionsIdCached.HasValue && (cacheValue != null);
        }
        

        #region IDbSnoozeOptionsConnector Members

        public void CheckId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"SnoozeOptions\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public bool? GetCardsEnabled(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeCardsEnabled, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeCardsEnabled, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT cards_enabled FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetCardsEnabled(int id, bool? CardsEnabled)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET cards_enabled=" + (CardsEnabled.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (CardsEnabled.HasValue)
                        cmd.Parameters.Add("value", CardsEnabled);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCardsEnabled, id, Cache.DefaultSettingsValidationTime)] = CardsEnabled;
                }
            }
        }

        public bool? GetRightsEnabled(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeRightsEnabled, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeRightsEnabled, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT rights_enabled FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetRightsEnabled(int id, bool? RightsEnabled)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET rights_enabled=" + (RightsEnabled.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (RightsEnabled.HasValue)
                        cmd.Parameters.Add("value", RightsEnabled);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRightsEnabled, id, Cache.DefaultSettingsValidationTime)] = RightsEnabled;
                }
            }
        }

        public bool? GetTimeEnabled(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeTimeEnabled, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeTimeEnabled, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT time_enabled FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetTimeEnabled(int id, bool? TimeEnabled)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET time_enabled=" + (TimeEnabled.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (TimeEnabled.HasValue)
                        cmd.Parameters.Add("value", TimeEnabled);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTimeEnabled, id, Cache.DefaultSettingsValidationTime)] = TimeEnabled;
                }
            }
        }

        public int? GetSnoozeCards(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeCards, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeCards, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_cards FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetSnoozeCards(int id, int? SnoozeCards)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_cards=" + (SnoozeCards.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (SnoozeCards.HasValue)
                        cmd.Parameters.Add("value", SnoozeCards);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCards, id, Cache.DefaultSettingsValidationTime)] = SnoozeCards;
                }
            }
        }

        public int? GetSnoozeHigh(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeHigh, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeHigh, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_high FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetSnoozeHigh(int id, int? SnoozeHigh)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_high=" + (SnoozeHigh.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (SnoozeHigh.HasValue)
                        cmd.Parameters.Add("value", SnoozeHigh);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeHigh, id, Cache.DefaultSettingsValidationTime)] = SnoozeHigh;
                }
            }
        }

        public int? GetSnoozeLow(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeLow, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeLow, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_low FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetSnoozeLow(int id, int? SnoozeLow)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_low=" + (SnoozeLow.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (SnoozeLow.HasValue)
                        cmd.Parameters.Add("value", SnoozeLow);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeLow, id, Cache.DefaultSettingsValidationTime)] = SnoozeLow;
                }
            }
        }

        public ESnoozeMode? GetSnoozeMode(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeMode, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeMode, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as ESnoozeMode?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_mode FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        object res =PostgreSQLConn.ExecuteScalar(cmd);
            //        return res is DBNull ? (ESnoozeMode?)null : (ESnoozeMode?)Enum.Parse(typeof(ESnoozeMode), res.ToString(), true);
            //    }
            //}
        }
        public void SetSnoozeMode(int id, ESnoozeMode? SnoozeMode)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_mode=" + (SnoozeMode.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (SnoozeMode.HasValue)
                        cmd.Parameters.Add("value", SnoozeMode.Value.ToString());

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeMode, id, Cache.DefaultSettingsValidationTime)] = SnoozeMode;
                }
            }
        }

        public int? GetSnoozeRights(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeRights, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeRights, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_rights FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetSnoozeRights(int id, int? SnoozeRights)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_rights=" + (SnoozeRights.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (SnoozeRights.HasValue)
                        cmd.Parameters.Add("value", SnoozeRights);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRights, id, Cache.DefaultSettingsValidationTime)] = SnoozeRights;
                }
            }
        }

        public int? GetSnoozeTime(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeTime, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeTime, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as int?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_time FROM \"SnoozeOptions\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<int>(cmd);
            //    }
            //}
        }
        public void SetSnoozeTime(int id, int? SnoozeTime)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"SnoozeOptions\" SET snooze_time=" + (SnoozeTime.HasValue ? ":value" : "null") + " WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (SnoozeTime.HasValue)
                        cmd.Parameters.Add("value", SnoozeTime);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTime, id, Cache.DefaultSettingsValidationTime)] = SnoozeTime;
                }
            }
        }

        #endregion
    }
}
