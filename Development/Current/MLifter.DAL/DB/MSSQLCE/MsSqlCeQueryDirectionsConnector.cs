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
    /// MsSqlCeQueryDirectionsConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-13</remarks>
    class MsSqlCeQueryDirectionsConnector : IDbQueryDirectionsConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeQueryDirectionsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeQueryDirectionsConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="ParentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public static MsSqlCeQueryDirectionsConnector GetInstance(ParentClass ParentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = ParentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeQueryDirectionsConnector(ParentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeQueryDirectionsConnector(ParentClass ParentClass)
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
        /// <param name="queryDirectionsId">The query directions id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private void GetSettingsValue(int queryDirectionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT * FROM \"QueryDirections\" WHERE id=@id";
            cmd.Parameters.Add("@id", queryDirectionsId);

            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
            reader.Read();

            int? did = DbValueConverter.Convert<int>(reader["id"]);
            bool? questionToAnswer = DbValueConverter.Convert<bool>(reader["question2answer"]);
            bool? answerToQuestion = DbValueConverter.Convert<bool>(reader["answer2question"]);
            bool? mixed = DbValueConverter.Convert<bool>(reader["mixed"]);
            reader.Close();

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

        /// <summary>
        /// Settingses the cached.
        /// </summary>
        /// <param name="queryDirectionsId">The query directions id.</param>
        /// <param name="cacheObjectType">Type of the cache object.</param>
        /// <param name="cacheValue">The cache value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private bool SettingsCached(int queryDirectionsId, CacheObject cacheObjectType, out object cacheValue)
        {
            int? queryDirectionsIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsQueryDirectionsId, queryDirectionsId)] as int?;
            cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, queryDirectionsId)];
            return queryDirectionsIdCached.HasValue;// && (cacheValue != null);
        }

        #region IDbQueryDirectionsConnector Members

        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void CheckId(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT count(*) FROM \"QueryDirections\" WHERE id=@id";
            cmd.Parameters.Add("@id", id);

            if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                throw new IdAccessException(id);
        }

        /// <summary>
        /// Gets the question2 answer.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetQuestion2Answer(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsQuestion2Answer, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsQuestion2Answer, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
		/// <summary>
		/// Sets the question2 answer.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="question2Answer">The question2 answer.</param>
		/// <remarks>
		/// Documented by FabThe, 13.1.2009
		/// </remarks>
        public void SetQuestion2Answer(int id, bool? question2Answer)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"QueryDirections\" SET question2answer=" + (question2Answer.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (question2Answer.HasValue)
                cmd.Parameters.Add("@value", question2Answer);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, id, Cache.DefaultSettingsValidationTime)] = question2Answer;
        }

        /// <summary>
        /// Gets the answer2 question.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetAnswer2Question(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsAnswer2Question, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsAnswer2Question, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the answer2 question.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="answer2Question">The answer2 question.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetAnswer2Question(int id, bool? answer2Question)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"QueryDirections\" SET answer2question=" + (answer2Question.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (answer2Question.HasValue)
                cmd.Parameters.Add("@value", answer2Question);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, id, Cache.DefaultSettingsValidationTime)] = answer2Question;
        }

        /// <summary>
        /// Gets the mixed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool? GetMixed(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsMixed, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsMixed, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;
        }
        /// <summary>
        /// Sets the mixed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mixed">The mixed.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetMixed(int id, bool? mixed)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"QueryDirections\" SET mixed=" + (mixed.HasValue ? "@value" : "null") + " WHERE id=@id";
            cmd.Parameters.Add("@id", id);
            if (mixed.HasValue)
                cmd.Parameters.Add("@value", mixed);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Save to Cache
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, id, Cache.DefaultSettingsValidationTime)] = mixed;
        }

        #endregion
    }
}
