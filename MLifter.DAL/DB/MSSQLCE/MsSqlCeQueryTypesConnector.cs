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
	/// MsSqlCeQueryTypesConnector
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-01-13</remarks>
	class MsSqlCeQueryTypesConnector : IDbQueryTypesConnector
	{
		private static Dictionary<ConnectionStringStruct, MsSqlCeQueryTypesConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeQueryTypesConnector>();
		public static MsSqlCeQueryTypesConnector GetInstance(ParentClass ParentClass)
		{
			lock (instances)
			{
				ConnectionStringStruct connection = ParentClass.CurrentUser.ConnectionString;

				if (!instances.ContainsKey(connection))
					instances.Add(connection, new MsSqlCeQueryTypesConnector(ParentClass));

				return instances[connection];
			}
		}

		private ParentClass Parent;
		private MsSqlCeQueryTypesConnector(ParentClass ParentClass)
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
		/// <param name="queryTypeId">The query type id.</param>
		/// <param name="cacheObjectType">Type of the cache object.</param>
		/// <param name="cacheValue">The cache value.</param>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		private void GetSettingsValue(int queryTypeId, CacheObject cacheObjectType, out object cacheValue)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT * FROM \"QueryTypes\" WHERE id=@id";
			cmd.Parameters.Add("@id", queryTypeId);

			SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
			reader.Read();

			int? qid = DbValueConverter.Convert<int>(reader["id"]);
			bool? imageRecognition = DbValueConverter.Convert<bool>(reader["image_recognition"]);
			bool? listeningComprehension = DbValueConverter.Convert<bool>(reader["listening_comprehension"]);
			bool? multipleChoice = DbValueConverter.Convert<bool>(reader["multiple_choice"]);
			bool? sentenceMode = DbValueConverter.Convert<bool>(reader["sentence"]);
			bool? wordMode = DbValueConverter.Convert<bool>(reader["word"]);
			reader.Close();

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

		/// <summary>
		/// Settingses the cached.
		/// </summary>
		/// <param name="queryTypeId">The query type id.</param>
		/// <param name="cacheObjectType">Type of the cache object.</param>
		/// <param name="cacheValue">The cache value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		private bool SettingsCached(int queryTypeId, CacheObject cacheObjectType, out object cacheValue)
		{
			int? queryTypeIdCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsQueryTypesId, queryTypeId)] as int?;
			cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, queryTypeId)];
			return queryTypeIdCached.HasValue;// && (cacheValue != null);
		}

		#region IDbQueryTypesConnector Members

		/// <summary>
		/// Checks the id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public void CheckId(int id)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT count(*) FROM \"QueryTypes\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
				throw new IdAccessException(id);
		}

		/// <summary>
		/// Gets the image recognition.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public bool? GetImageRecognition(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryTypesImageRecognition, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryTypesImageRecognition, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the image recognition.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="imageRecognition">The image recognition.</param>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public void SetImageRecognition(int id, bool? imageRecognition)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"QueryTypes\" SET image_recognition=" + (imageRecognition.HasValue ? "@value" : "null") + " WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			if (imageRecognition.HasValue)
				cmd.Parameters.Add("@value", imageRecognition);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, id, Cache.DefaultSettingsValidationTime)] = imageRecognition;
		}

		/// <summary>
		/// Gets the listening comprehension.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public bool? GetListeningComprehension(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryTypesListeningComprehension, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryTypesListeningComprehension, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the listening comprehension.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="listeningComprehension">The listening comprehension.</param>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public void SetListeningComprehension(int id, bool? listeningComprehension)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"QueryTypes\" SET listening_comprehension=" + (listeningComprehension.HasValue ? "@value" : "null") + " WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			if (listeningComprehension.HasValue)
				cmd.Parameters.Add("@value", listeningComprehension);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, id, Cache.DefaultSettingsValidationTime)] = listeningComprehension;
		}

		/// <summary>
		/// Gets the multiple choice.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public bool? GetMultipleChoice(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryTypesMultipleChoice, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryTypesMultipleChoice, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the multiple choice.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="multipleChoice">The multiple choice.</param>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public void SetMultipleChoice(int id, bool? multipleChoice)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"QueryTypes\" SET multiple_choice=" + (multipleChoice.HasValue ? "@value" : "null") + " WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			if (multipleChoice.HasValue)
				cmd.Parameters.Add("@value", multipleChoice);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, id, Cache.DefaultSettingsValidationTime)] = multipleChoice;
		}

		/// <summary>
		/// Gets the sentence.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public bool? GetSentence(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryTypesSentence, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryTypesSentence, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the sentence.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="sentence">The sentence.</param>
		/// <remarks>
		/// Documented by FabThe, 13.1.2009
		/// </remarks>
		public void SetSentence(int id, bool? sentence)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"QueryTypes\" SET sentence=" + (sentence.HasValue ? "@value" : "null") + " WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			if (sentence.HasValue)
				cmd.Parameters.Add("@value", sentence);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, id, Cache.DefaultSettingsValidationTime)] = sentence;
		}

		/// <summary>
		/// Gets the word.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public bool? GetWord(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryTypesWord, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryTypesWord, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the word.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="word">The word.</param>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public void SetWord(int id, bool? word)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"QueryTypes\" SET word=" + (word.HasValue ? "@value" : "null") + " WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			if (word.HasValue)
				cmd.Parameters.Add("@value", word);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, id, Cache.DefaultSettingsValidationTime)] = word;
		}

		#endregion
	}
}
