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
using System.Globalization;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
	/// <summary>
	/// MsSqlCeSettingsConnector
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-01-12</remarks>
	class MsSqlCeSettingsConnector : IDbSettingsConnector
	{
		private static Dictionary<ConnectionStringStruct, MsSqlCeSettingsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeSettingsConnector>();
		public static MsSqlCeSettingsConnector GetInstance(ParentClass parentClass)
		{
			lock (instances)
			{
				ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

				if (!instances.ContainsKey(connection))
					instances.Add(connection, new MsSqlCeSettingsConnector(parentClass));

				return instances[connection];
			}
		}

		private ParentClass Parent;
		private MsSqlCeSettingsConnector(ParentClass parentClass)
		{
			Parent = parentClass;
			Parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
		}

		void parent_DictionaryClosed(object sender, EventArgs e)
		{
			IParent parent = sender as IParent;
			instances.Remove(parent.Parent.CurrentUser.ConnectionString);
		}

		/******************************************************************************************/
		//Global caching functions
		/******************************************************************************************/
		/// <summary>
		/// Gets the settings value.
		/// </summary>
		/// <param name="settingsId">The settings id.</param>
		/// <param name="cacheObjectType">Type of the cache object.</param>
		/// <param name="cacheValue">The cache value.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		private void GetSettingsValue(int settingsId, CacheObject cacheObjectType, out object cacheValue)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT * FROM \"Settings\" WHERE id=@id;";
			cmd.Parameters.Add("@id", settingsId);

			SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
			reader.Read();
			//read the values
			bool? autoplayAudio = DbValueConverter.Convert<bool>(reader["autoplay_audio"]);
			bool? caseSensitive = DbValueConverter.Convert<bool>(reader["case_sensitive"]);
			bool? confirmDemote = DbValueConverter.Convert<bool>(reader["confirm_demote"]);
			bool? enableCommentary = DbValueConverter.Convert<bool>(reader["enable_commentary"]);
			bool? correctOnTheFly = DbValueConverter.Convert<bool>(reader["correct_on_the_fly"]);
			bool? enableTimer = DbValueConverter.Convert<bool>(reader["enable_timer"]);
			int? synonymGradingsId = DbValueConverter.Convert<int>(reader["synonym_gradings"]);    //FK
			int? typeGradingsId = DbValueConverter.Convert<int>(reader["type_gradings"]);    //FK
			int? multipleChoiceOptionsId = DbValueConverter.Convert<int>(reader["multiple_choice_options"]);    //FK
			int? queryDirectionsId = DbValueConverter.Convert<int>(reader["query_directions"]);    //FK
			int? queryTypesId = DbValueConverter.Convert<int>(reader["query_types"]);    //FK
			bool? randomPool = DbValueConverter.Convert<bool>(reader["random_pool"]);
			bool? selfAssessment = DbValueConverter.Convert<bool>(reader["self_assessment"]);
			bool? showImages = DbValueConverter.Convert<bool>(reader["show_images"]);
			string stripChars = DbValueConverter.Convert(reader["stripchars"]);
			string questionCulture = DbValueConverter.Convert(reader["question_culture"]);
			string answerCulture = DbValueConverter.Convert(reader["answer_culture"]);
			string questionCaption = DbValueConverter.Convert(reader["question_caption"]);
			string answerCaption = DbValueConverter.Convert(reader["answer_caption"]);
			int? logoId = DbValueConverter.Convert<int>(reader["logo"]);    //FK
			int? questionStylesheetId = DbValueConverter.Convert<int>(reader["question_stylesheet"]);    //FK
			int? answerStylesheetId = DbValueConverter.Convert<int>(reader["answer_stylesheet"]);    //FK
			bool? autoBoxsize = DbValueConverter.Convert<bool>(reader["auto_boxsize"]);
			bool? poolEmptyMessageShown = DbValueConverter.Convert<bool>(reader["pool_empty_message_shown"]);
			bool? showStatistics = DbValueConverter.Convert<bool>(reader["show_statistics"]);
			bool? skipCorrectAnswers = DbValueConverter.Convert<bool>(reader["skip_correct_answers"]);
			int? snoozeOptionsId = DbValueConverter.Convert<int>(reader["snooze_options"]);    //FK
			bool? useLmStylesheets = DbValueConverter.Convert<bool>(reader["use_lm_stylesheets"]);
			int? cardStyleId = DbValueConverter.Convert<int>(reader["cardstyle"]);    //FK
			int? boxesId = DbValueConverter.Convert<int>(reader["boxes"]);    //FK
			bool? isCached = DbValueConverter.Convert<bool>(reader["isCached"]);
			reader.Close();

			//StyleSheets
			string questionStylesheet = questionStylesheetId.HasValue ? GetStyleSheet(questionStylesheetId.Value) : string.Empty;
			string answerStylesheet = answerStylesheetId.HasValue ? GetStyleSheet(answerStylesheetId.Value) : string.Empty;
			string cardStyle = cardStyleId.HasValue ? GetStyle(cardStyleId.Value) : string.Empty;

			//cache values
			DateTime expires = DateTime.Now.Add(Cache.DefaultSettingsValidationTime);
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAutoPlayAudio, settingsId, expires)] = autoplayAudio;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCaseSensetive, settingsId, expires)] = caseSensitive;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsConfirmDemote, settingsId, expires)] = confirmDemote;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsEnableCommentary, settingsId, expires)] = enableCommentary;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCorrectOnTheFly, settingsId, expires)] = correctOnTheFly;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsEnableTimer, settingsId, expires)] = enableTimer;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFk, settingsId, expires)] = synonymGradingsId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsFk, settingsId, expires)] = typeGradingsId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsFk, settingsId, expires)] = multipleChoiceOptionsId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsFk, settingsId, expires)] = queryDirectionsId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesFk, settingsId, expires)] = queryTypesId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsRandomPool, settingsId, expires)] = randomPool;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelfAssessment, settingsId, expires)] = selfAssessment;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsShowImages, settingsId, expires)] = showImages;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStripchars, settingsId, expires)] = stripChars;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQuestionCulture, settingsId, expires)] = questionCulture;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAnswerCulture, settingsId, expires)] = answerCulture;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQuestionCaption, settingsId, expires)] = questionCaption;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAnswerCaption, settingsId, expires)] = answerCaption;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsLogoFk, settingsId, expires)] = logoId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAutoBoxsize, settingsId, expires)] = autoBoxsize;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsPoolEmptyMessageShown, settingsId, expires)] = poolEmptyMessageShown;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsShowStatistics, settingsId, expires)] = showStatistics;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSkipCorrectAnswers, settingsId, expires)] = skipCorrectAnswers;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeOptionsFk, settingsId, expires)] = snoozeOptionsId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsUseLearningModuleStylesheet, settingsId, expires)] = useLmStylesheets;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleFk, settingsId, expires)] = cardStyleId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsBoxesFk, settingsId, expires)] = boxesId;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStyleSheetsQuestionValue, settingsId, expires)] = questionStylesheet;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStyleSheetsAnswerValue, settingsId, expires)] = answerStylesheet;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleValue, settingsId, expires)] = cardStyle;
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsIsCached, settingsId, expires)] = isCached;

			//set output value
			switch (cacheObjectType)
			{
				case CacheObject.SettingsAutoPlayAudio: cacheValue = autoplayAudio; break;
				case CacheObject.SettingsCaseSensetive: cacheValue = caseSensitive; break;
				case CacheObject.SettingsConfirmDemote: cacheValue = confirmDemote; break;
				case CacheObject.SettingsEnableCommentary: cacheValue = enableCommentary; break;
				case CacheObject.SettingsCorrectOnTheFly: cacheValue = correctOnTheFly; break;
				case CacheObject.SettingsEnableTimer: cacheValue = enableTimer; break;
				case CacheObject.SettingsSynonymGradingsFk: cacheValue = synonymGradingsId; break;
				case CacheObject.SettingsTypeGradingsFk: cacheValue = typeGradingsId; break;
				case CacheObject.SettingsMultipleChoiceOptionsFk: cacheValue = multipleChoiceOptionsId; break;
				case CacheObject.SettingsQueryDirectionsFk: cacheValue = queryDirectionsId; break;
				case CacheObject.SettingsQueryTypesFk: cacheValue = queryTypesId; break;
				case CacheObject.SettingsRandomPool: cacheValue = randomPool; break;
				case CacheObject.SettingsSelfAssessment: cacheValue = selfAssessment; break;
				case CacheObject.SettingsShowImages: cacheValue = showImages; break;
				case CacheObject.SettingsStripchars: cacheValue = stripChars; break;
				case CacheObject.SettingsQuestionCulture: cacheValue = questionCulture; break;
				case CacheObject.SettingsAnswerCulture: cacheValue = answerCulture; break;
				case CacheObject.SettingsQuestionCaption: cacheValue = questionCaption; break;
				case CacheObject.SettingsAnswerCaption: cacheValue = answerCaption; break;
				case CacheObject.SettingsLogoFk: cacheValue = logoId; break;
				case CacheObject.SettingsAutoBoxsize: cacheValue = autoBoxsize; break;
				case CacheObject.SettingsPoolEmptyMessageShown: cacheValue = poolEmptyMessageShown; break;
				case CacheObject.SettingsShowStatistics: cacheValue = showStatistics; break;
				case CacheObject.SettingsSkipCorrectAnswers: cacheValue = skipCorrectAnswers; break;
				case CacheObject.SettingsSnoozeOptionsFk: cacheValue = snoozeOptionsId; break;
				case CacheObject.SettingsUseLearningModuleStylesheet: cacheValue = useLmStylesheets; break;
				case CacheObject.SettingsCardStyleFk: cacheValue = cardStyleId; break;
				case CacheObject.SettingsBoxesFk: cacheValue = boxesId; break;
				case CacheObject.SettingsIsCached: cacheValue = isCached; break;
				case CacheObject.SettingsStyleSheetsQuestionValue: cacheValue = questionStylesheet; break;
				case CacheObject.SettingsStyleSheetsAnswerValue: cacheValue = answerStylesheet; break;
				case CacheObject.SettingsCardStyleValue: cacheValue = cardStyle; break;
				default: cacheValue = null; break;
			}
		}

		/// <summary>
		/// Gets the style sheet.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		private string GetStyleSheet(int id)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT value FROM StylSheets WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			return MSSQLCEConn.ExecuteScalar(cmd).ToString();
		}

		/// <summary>
		/// Gets the style.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		private string GetStyle(int id)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT value FROM CardStyles WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			return MSSQLCEConn.ExecuteScalar(cmd).ToString();
		}

		/// <summary>
		/// Checks if a given setting value is cached and outputs the value.
		/// </summary>
		/// <param name="settingsId">The settings Id.</param>
		/// <param name="cacheObjectType">Type of the cache object.</param>
		/// <param name="cacheValue">The cache value.</param>
		/// <returns>[true] if cached.</returns>
		/// <remarks>Documented by Dev03, 2008-11-26</remarks>
		private bool SettingsCached(int settingsId, CacheObject cacheObjectType, out object cacheValue)
		{
			bool? isCached = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsIsCached, settingsId)] as bool?;
			cacheValue = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(cacheObjectType, settingsId)];
			return isCached.HasValue && (cacheValue != null);
		}

		#region IDbSettingsConnector Members

		/// <summary>
		/// Checks the settings id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void CheckSettingsId(int id)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT count(*) FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int? count = MSSQLCEConn.ExecuteScalar<int>(cmd);

			if (!count.HasValue || count.Value < 1)
				throw new IdAccessException(id);
		}

		/// <summary>
		/// Gets the query directions.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IQueryDirections GetQueryDirections(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryDirectionsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? queryDirectionsId = cacheValue as int?;
			return queryDirectionsId.HasValue && queryDirectionsId.Value > 0 ? new DbQueryDirections(queryDirectionsId.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the query directions.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="QueryDirections">The query directions.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetQueryDirections(int id, IQueryDirections QueryDirections)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT query_directions FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int? qdid = MSSQLCEConn.ExecuteScalar<int>(cmd);

			if (qdid.HasValue && qdid.Value > 0)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"QueryDirections\" SET question2answer=@q2a, answer2question=@a2q, mixed=@mixed WHERE id=@id";
				cmd2.Parameters.Add("@id", qdid.Value);
				cmd2.Parameters.Add("@q2a", QueryDirections.Question2Answer);
				cmd2.Parameters.Add("@a2q", QueryDirections.Answer2Question);
				cmd2.Parameters.Add("@mixed", QueryDirections.Mixed);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsFk, id, Cache.DefaultSettingsValidationTime)] = qdid;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, qdid.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Question2Answer;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, qdid.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Answer2Question;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, qdid.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Mixed;
			}
			else
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"QueryDirections\" (question2answer, answer2question, mixed) VALUES " +
					"(@q2a, @a2q, @mixed); UPDATE \"Settings\" SET query_directions=@@IDENTITY WHERE id=@id;" +
					"SELECT query_directions FROM \"Settings\" WHERE id=@id";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@q2a", QueryDirections.Question2Answer);
				cmd3.Parameters.Add("@a2q", QueryDirections.Answer2Question);
				cmd3.Parameters.Add("@mixed", QueryDirections.Mixed);

				int? queryDirectionsId = MSSQLCEConn.ExecuteScalar<int>(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsFk, id, Cache.DefaultSettingsValidationTime)] = queryDirectionsId;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, queryDirectionsId.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Question2Answer;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, queryDirectionsId.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Answer2Question;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, queryDirectionsId.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Mixed;
			}
		}

		/// <summary>
		/// Gets the type of the query.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IQueryType GetQueryType(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsQueryTypesFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsQueryTypesFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? qtid = cacheValue as int?;
			return qtid.HasValue && qtid.Value > 0 ? new DbQueryTypes(qtid.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the type of the query.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="QueryType">Type of the query.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetQueryType(int id, IQueryType QueryType)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT query_types FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int qtid = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			if (qtid > 0)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"QueryTypes\" SET " +
					"image_recognition=@i_r, listening_comprehension=@l_c, multiple_choice=@m_c, sentence=@s, word=@w WHERE id=@id";
				cmd2.Parameters.Add("@id", qtid);
				cmd2.Parameters.Add("@i_r", QueryType.ImageRecognition);
				cmd2.Parameters.Add("@l_c", QueryType.ListeningComprehension);
				cmd2.Parameters.Add("@m_c", QueryType.MultipleChoice);
				cmd2.Parameters.Add("@s", QueryType.Sentence);
				cmd2.Parameters.Add("@w", QueryType.Word);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesFk, id, Cache.DefaultSettingsValidationTime)] = qtid;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.ImageRecognition;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.ListeningComprehension;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.MultipleChoice;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.Sentence;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.Word;
			}
			else
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"QueryTypes\" (image_recognition, listening_comprehension, multiple_choice, sentence, word) VALUES " +
					"(@i_r, @l_c, @m_c, @s, @w); UPDATE \"Settings\" SET query_types=@@IDENTITY WHERE id=@id; " +
					"SELECT query_types FROM \"Settings\" WHERE id=@id";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@i_r", QueryType.ImageRecognition);
				cmd3.Parameters.Add("@l_c", QueryType.ListeningComprehension);
				cmd3.Parameters.Add("@m_c", QueryType.MultipleChoice);
				cmd3.Parameters.Add("@s", QueryType.Sentence);
				cmd3.Parameters.Add("@w", QueryType.Word);

				int? queryTypeId = MSSQLCEConn.ExecuteScalar<int>(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesFk, id, Cache.DefaultSettingsValidationTime)] = queryTypeId;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.ImageRecognition;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.ListeningComprehension;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.MultipleChoice;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.Sentence;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.Word;
			}
		}

		/// <summary>
		/// Gets the snooze options.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public ISnoozeOptions GetSnoozeOptions(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsSnoozeOptionsFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsSnoozeOptionsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? snoozeOptionsId = cacheValue as int?;
			return snoozeOptionsId.HasValue && snoozeOptionsId.Value > 0 ? new DbSnoozeOptions(snoozeOptionsId.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the snooze options.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="SnoozeOptions">The snooze options.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetSnoozeOptions(int id, ISnoozeOptions SnoozeOptions)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT snooze_options FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int soid = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			if (soid > 0)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"SnoozeOptions\" SET " +
					"cards_enabled=@c_e, rights_enabled=@r_e, time_enabled=@t_e, snooze_cards=@s_c, snooze_hight=@s_h, snooze_low=@s_l, " +
					"snooze_mode=@s_m, snooze_rights=@s_r, snooze_time=@s_t WHERE id=@id";
				cmd2.Parameters.Add("@id", soid);
				cmd2.Parameters.Add("@c_e", SnoozeOptions.IsCardsEnabled);
				cmd2.Parameters.Add("@r_e", SnoozeOptions.IsRightsEnabled);
				cmd2.Parameters.Add("@t_e", SnoozeOptions.IsTimeEnabled);
				cmd2.Parameters.Add("@s_c", SnoozeOptions.SnoozeCards);
				cmd2.Parameters.Add("@s_h", SnoozeOptions.SnoozeHigh);
				cmd2.Parameters.Add("@s_l", SnoozeOptions.SnoozeLow);
				cmd2.Parameters.Add("@s_m", SnoozeOptions.SnoozeMode);
				cmd2.Parameters.Add("@s_r", SnoozeOptions.SnoozeRights);
				cmd2.Parameters.Add("@s_t", SnoozeOptions.SnoozeTime);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeOptionsFk, id, Cache.DefaultSettingsValidationTime)] = soid;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCardsEnabled, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.IsCardsEnabled;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRightsEnabled, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.IsRightsEnabled;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTimeEnabled, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.IsTimeEnabled;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCards, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeCards;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeHigh, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeHigh;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeLow, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeLow;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeMode, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeMode;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRights, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeRights;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTime, soid, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeTime;
			}
			else
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"SnoozeOptions\" " +
					"(cards_enabled, rights_enabled, time_enabled, snooze_cards, snooze_hight, snooze_low, snooze_mode, snooze_rights, snooze_time) VALUES " +
					"(@c_e, @r_e, @t_e, @s_c, @s_h, @s_l, @s_m, @s_r, @s_t); UPDATE \"Settings\" SET snooze_options=@@IDENTITY WHERE id=@id; " +
					"SELECT snooze_options FROM \"Settings\" WHERE id=@id";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@c_e", SnoozeOptions.IsCardsEnabled);
				cmd3.Parameters.Add("@r_e", SnoozeOptions.IsRightsEnabled);
				cmd3.Parameters.Add("@t_e", SnoozeOptions.IsTimeEnabled);
				cmd3.Parameters.Add("@s_c", SnoozeOptions.SnoozeCards);
				cmd3.Parameters.Add("@s_h", SnoozeOptions.SnoozeHigh);
				cmd3.Parameters.Add("@s_l", SnoozeOptions.SnoozeLow);
				cmd3.Parameters.Add("@s_m", SnoozeOptions.SnoozeMode);
				cmd3.Parameters.Add("@s_r", SnoozeOptions.SnoozeRights);
				cmd3.Parameters.Add("@s_t", SnoozeOptions.SnoozeTime);

				//MSSQLCEConn.ExecuteNonQuery(cmd3);
				int? snoozeOptionsId = MSSQLCEConn.ExecuteScalar<int>(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeOptionsFk, id, Cache.DefaultSettingsValidationTime)] = snoozeOptionsId.Value;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCardsEnabled, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.IsCardsEnabled;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRightsEnabled, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.IsRightsEnabled;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTimeEnabled, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.IsTimeEnabled;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeCards, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeCards;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeHigh, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeHigh;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeLow, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeLow;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeMode, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeMode;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeRights, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeRights;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSnoozeTime, snoozeOptionsId.Value, Cache.DefaultSettingsValidationTime)] = SnoozeOptions.SnoozeTime;
			}
		}

		/// <summary>
		/// Gets the multiple choice options.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IQueryMultipleChoiceOptions GetMultipleChoiceOptions(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? mcoid = cacheValue as int?;
			return mcoid.HasValue && mcoid.Value > 0 ? new DbQueryMultipleChoiceOptions(mcoid.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the multiple choice options.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="MultipleChoiceOptions">The multiple choice options.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetMultipleChoiceOptions(int id, IQueryMultipleChoiceOptions MultipleChoiceOptions)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT multiple_choice_options FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int mcoid = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			if (mcoid > 0)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"MultipleChoiceOptions\" SET " +
					"allow_multiple_correct_answers=@a_m_c_a, allow_random_distractors=@r_d, max_correct_answers=@m_c_a, number_of_choices=@n_o_c WHERE id=@id";
				cmd2.Parameters.Add("@id", mcoid);
				cmd2.Parameters.Add("@a_m_c_a", MultipleChoiceOptions.AllowMultipleCorrectAnswers);
				cmd2.Parameters.Add("@r_d", MultipleChoiceOptions.AllowRandomDistractors);
				cmd2.Parameters.Add("@m_c_a", MultipleChoiceOptions.MaxNumberOfCorrectAnswers);
				cmd2.Parameters.Add("@n_o_c", MultipleChoiceOptions.NumberOfChoices);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsFk, id, Cache.DefaultSettingsValidationTime)] = mcoid;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowMultipleCorrectAnswers;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowRandomDistractors;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.NumberOfChoices;
			}
			else
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"MultipleChoiceOptions\" " +
					"(allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices) VALUES " +
					"(@a_m_c_a, @r_d, @m_c_a, @n_o_c); UPDATE \"Settings\" SET multiple_choice_options=@@IDENTITY WHERE id=@id; " +
					"SELECT multiple_choice_options FROM \"Settings\" WHERE id=@id";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@a_m_c_a", MultipleChoiceOptions.AllowMultipleCorrectAnswers);
				cmd3.Parameters.Add("@r_d", MultipleChoiceOptions.AllowRandomDistractors);
				cmd3.Parameters.Add("@m_c_a", MultipleChoiceOptions.MaxNumberOfCorrectAnswers);
				cmd3.Parameters.Add("@n_o_c", MultipleChoiceOptions.NumberOfChoices);

				int? multipleChoiceOptionsId = MSSQLCEConn.ExecuteScalar<int>(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsFk, id, Cache.DefaultSettingsValidationTime)] = multipleChoiceOptionsId;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowMultipleCorrectAnswers;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowRandomDistractors;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.NumberOfChoices;
			}
		}

		/// <summary>
		/// Gets the grade typing.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IGradeTyping GetGradeTyping(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsTypeGradingsFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsTypeGradingsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? gtid = cacheValue as int?;
			return gtid.HasValue && gtid.Value > 0 ? new DbGradeTyping(gtid.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the grade typing.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="GradeTyping">The grade typing.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetGradeTyping(int id, IGradeTyping GradeTyping)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT type_gradings FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int gtid = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			if (gtid > 0)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"TypeGradings\" SET all_correct=@a_c, half_correct=@h_c, none_correct=@n_c, prompt=@p WHERE id=@id";
				cmd2.Parameters.Add("@id", gtid);
				cmd2.Parameters.Add("@a_c", GradeTyping.AllCorrect);
				cmd2.Parameters.Add("@h_c", GradeTyping.HalfCorrect);
				cmd2.Parameters.Add("@n_c", GradeTyping.NoneCorrect);
				cmd2.Parameters.Add("@p", GradeTyping.Prompt);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsFk, id, Cache.DefaultSettingsValidationTime)] = gtid;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.AllCorrect;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.HalfCorrect;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.NoneCorrect;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.Prompt;
			}
			else
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"TypeGradings\" (all_correct, half_correct, none_correct, prompt) VALUES " +
					"(@a_c, @h_c, @n_c, @p); UPDATE \"Settings\" SET type_gradings=@@IDENTITY WHERE id=@id; SELECT @@IDENTITY;";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@a_c", GradeTyping.AllCorrect);
				cmd3.Parameters.Add("@h_c", GradeTyping.HalfCorrect);
				cmd3.Parameters.Add("@n_c", GradeTyping.NoneCorrect);
				cmd3.Parameters.Add("@p", GradeTyping.Prompt);

				int? gradeTypeingId = MSSQLCEConn.ExecuteScalar<int>(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsFk, id, Cache.DefaultSettingsValidationTime)] = gradeTypeingId;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.AllCorrect;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.HalfCorrect;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.NoneCorrect;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.Prompt;
			}
		}

		/// <summary>
		/// Gets the grade synonyms.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IGradeSynonyms GetGradeSynonyms(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsSynonymGradingsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? synonymGradingsId = cacheValue as int?;
			return synonymGradingsId.HasValue && synonymGradingsId.Value > 0 ? new DbGradeSynonyms(synonymGradingsId.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the grade synonyms.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="GradeSynonyms">The grade synonyms.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetGradeSynonyms(int id, IGradeSynonyms GradeSynonyms)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT synonym_gradings FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int gsid = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			if (gsid > 0)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"SynonymGradings\" SET all_known=@a_k, half_known=@h_k, one_known=@o_k, first_known=@f_k, prompt=@p WHERE id=@id";
				cmd2.Parameters.Add("@id", gsid);
				cmd2.Parameters.Add("@a_k", GradeSynonyms.AllKnown);
				cmd2.Parameters.Add("@h_k", GradeSynonyms.HalfKnown);
				cmd2.Parameters.Add("@o_k", GradeSynonyms.OneKnown);
				cmd2.Parameters.Add("@f_k", GradeSynonyms.FirstKnown);
				cmd2.Parameters.Add("@p", GradeSynonyms.Prompt);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFk, id, Cache.DefaultSettingsValidationTime)] = gsid;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.AllKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.HalfKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.OneKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.FirstKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.Prompt;
			}
			else
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"SynonymGradings\" (all_known, half_known, one_known, first_known, prompt) VALUES " +
					"(@a_k, @h_k, @o_k, @f_k, @p); UPDATE \"Settings\" SET synonym_gradings=@@IDENTITY WHERE id=@id;" +
					"SELECT synonym_grading FROM \"Settings\" WHERE id=@id";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@a_k", GradeSynonyms.AllKnown);
				cmd3.Parameters.Add("@h_k", GradeSynonyms.HalfKnown);
				cmd3.Parameters.Add("@o_k", GradeSynonyms.OneKnown);
				cmd3.Parameters.Add("@f_k", GradeSynonyms.FirstKnown);
				cmd3.Parameters.Add("@p", GradeSynonyms.Prompt);

				int? synonymsGrading = MSSQLCEConn.ExecuteScalar<int>(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFk, id, Cache.DefaultSettingsValidationTime)] = synonymsGrading;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.AllKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.HalfKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.OneKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.FirstKnown;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.Prompt;
			}
		}

		/// <summary>
		/// Gets the commentary sounds.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public Dictionary<CommentarySoundIdentifier, IMedia> GetCommentarySounds(int id)
		{
			if ((Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsCommentarySounds, id)] as Dictionary<CommentarySoundIdentifier, IMedia>) == null)
			{
				Dictionary<CommentarySoundIdentifier, IMedia> commentarySounds = new Dictionary<CommentarySoundIdentifier, IMedia>();
				SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd.CommandText = "SELECT media_id, side, type FROM \"CommentarySounds\" WHERE settings_id=@id;";
				cmd.Parameters.Add("@id", id);
				SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
				while (reader.Read())
				{
					int mediaid = Convert.ToInt32(reader["media_id"]);
					Side side = (Side)Enum.Parse(typeof(Side), Convert.ToString(reader["side"]));
					ECommentarySoundType type = (ECommentarySoundType)Enum.Parse(typeof(ECommentarySoundType), Convert.ToString(reader["type"]));

					IMedia media = new DbAudio(mediaid, -1, false, side, WordType.Word, true, false, Parent);
					commentarySounds[CommentarySoundIdentifier.Create(side, type)] = media;
				}
				reader.Close();
				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCommentarySounds, id, Cache.DefaultSettingsValidationTime)] = commentarySounds;
			}

			return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsCommentarySounds, id)] as Dictionary<CommentarySoundIdentifier, IMedia>;
		}
		/// <summary>
		/// Sets the commentary sounds.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="CommentarySounds">The commentary sounds.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetCommentarySounds(int id, Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			SqlCeTransaction trans = cmd.Connection.BeginTransaction();
			cmd.CommandText = "DELETE FROM \"CommentarySounds\" WHERE settings_id=@id";
			cmd.Parameters.Add("@id", id);
			MSSQLCEConn.ExecuteNonQuery(cmd);

			foreach (KeyValuePair<CommentarySoundIdentifier, IMedia> commentarySound in CommentarySounds)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "INSERT INTO \"CommentarySounds\" (media_id, settings_id, side, type) VALUES (@mediaid, @id, @side, @type)";
				cmd2.Parameters.Add("@mediaid", commentarySound.Value.Id);
				cmd2.Parameters.Add("@id", id);
				cmd2.Parameters.Add("@side", commentarySound.Key.Side.ToString());
				cmd2.Parameters.Add("@type", commentarySound.Key.Type.ToString());
				MSSQLCEConn.ExecuteNonQuery(cmd2);
			}

			trans.Commit();

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCommentarySounds, id, Cache.DefaultSettingsValidationTime)] = CommentarySounds;
		}

		/// <summary>
		/// Gets the culture.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="side">The side.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public CultureInfo GetCulture(int id, Side side)
		{
			CacheObject cacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCulture : CacheObject.SettingsAnswerCulture);
			object cacheValue;
			if (!SettingsCached(id, cacheObjectSide, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, cacheObjectSide, out cacheValue);    //Saves the current Settings from the DB to the Cache
			string culture = (cacheValue != null) ? cacheValue as string : string.Empty;
			return (culture.Length > 0 ? new CultureInfo(culture, false) : null);
		}
		/// <summary>
		/// Sets the culture.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="side">The side.</param>
		/// <param name="Culture">The culture.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetCulture(int id, Side side, CultureInfo Culture)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			switch (side)
			{
				case Side.Question:
					cmd.CommandText = "UPDATE \"Settings\" SET question_culture=@value WHERE id=@id";
					break;
				case Side.Answer:
					cmd.CommandText = "UPDATE \"Settings\" SET answer_culture=@value WHERE id=@id";
					break;
				default:
					break;
			}

			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", Culture == null ? DBNull.Value as object : Culture.Name.Length > 0 ? Culture.Name : "en");

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			CacheObject CacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCulture : CacheObject.SettingsAnswerCulture);
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObjectSide, id, Cache.DefaultSettingsValidationTime)] = Culture == null ? null : Culture.Name;
		}

		/// <summary>
		/// Gets the autoplay audio.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetAutoplayAudio(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsAutoPlayAudio, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsAutoPlayAudio, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the autoplay audio.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="AutoplayAudio">The autoplay audio.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetAutoplayAudio(int id, bool? AutoplayAudio)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET autoplay_audio=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", AutoplayAudio.HasValue == true ? AutoplayAudio : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAutoPlayAudio, id, Cache.DefaultSettingsValidationTime)] = AutoplayAudio;
		}

		/// <summary>
		/// Gets the caption.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="side">The side.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public string GetCaption(int id, Side side)
		{
			CacheObject cacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCaption : CacheObject.SettingsAnswerCaption);
			object cacheValue;
			if (!SettingsCached(id, cacheObjectSide, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, cacheObjectSide, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as string;
		}
		/// <summary>
		/// Sets the caption.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="side">The side.</param>
		/// <param name="Caption">The caption.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetCaption(int id, Side side, string Caption)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			switch (side)
			{
				case Side.Question:
					cmd.CommandText = "UPDATE \"Settings\" SET question_caption=@value WHERE id=@id";
					break;
				case Side.Answer:
					cmd.CommandText = "UPDATE \"Settings\" SET answer_caption=@value WHERE id=@id";
					break;
				default:
					break;
			}

			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", Caption);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			CacheObject CacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCaption : CacheObject.SettingsAnswerCaption);
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObjectSide, id, Cache.DefaultSettingsValidationTime)] = Caption ?? string.Empty;
		}

		/// <summary>
		/// Gets the case sensetive.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetCaseSensitive(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsCaseSensetive, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsCaseSensetive, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the case sensetive.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="CaseSensetive">The case sensetive.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetCaseSensitive(int id, bool? CaseSensetive)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET case_sensitive=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", CaseSensetive.HasValue ? CaseSensetive : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCaseSensetive, id, Cache.DefaultSettingsValidationTime)] = CaseSensetive;
		}

		/// <summary>
		/// Gets the confirm demote.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetConfirmDemote(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsConfirmDemote, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsConfirmDemote, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the confirm demote.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="ConfirmDemote">The confirm demote.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetConfirmDemote(int id, bool? ConfirmDemote)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET confirm_demote=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", ConfirmDemote.HasValue ? ConfirmDemote : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsConfirmDemote, id, Cache.DefaultSettingsValidationTime)] = ConfirmDemote;

		}

		/// <summary>
		/// Gets the correct on the fly.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetCorrectOnTheFly(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsCorrectOnTheFly, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsCorrectOnTheFly, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the correct on the fly.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="CorrectOnTheFly">The correct on the fly.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetCorrectOnTheFly(int id, bool? CorrectOnTheFly)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET correct_on_the_fly=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", CorrectOnTheFly.HasValue ? CorrectOnTheFly : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCorrectOnTheFly, id, Cache.DefaultSettingsValidationTime)] = CorrectOnTheFly;
		}

		/// <summary>
		/// Gets the enable commentary.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetEnableCommentary(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsEnableCommentary, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsEnableCommentary, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the enable commentary.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="EnableCommentary">The enable commentary.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetEnableCommentary(int id, bool? EnableCommentary)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET enable_commentary=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", EnableCommentary.HasValue ? EnableCommentary : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsEnableCommentary, id, Cache.DefaultSettingsValidationTime)] = EnableCommentary;
		}

		/// <summary>
		/// Gets the enable timer.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetEnableTimer(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsEnableTimer, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsEnableTimer, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the enable timer.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="EnableTimer">The enable timer.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetEnableTimer(int id, bool? EnableTimer)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET enable_timer=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", EnableTimer.HasValue ? EnableTimer : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsEnableTimer, id, Cache.DefaultSettingsValidationTime)] = EnableTimer;
		}

		/// <summary>
		/// Gets the logo.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IMedia GetLogo(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsLogoFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsLogoFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? logoId = cacheValue as int?;
			return logoId.HasValue ? DbMedia.CreateDisconnectedCardMedia(logoId.Value, EMedia.Image, false, false, Parent) : null;
		}
		/// <summary>
		/// Sets the logo.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="Logo">The logo.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetLogo(int id, IMedia Logo)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET logo=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			if (Logo == null)
				cmd.Parameters.Add("@value", DBNull.Value);
			else
				cmd.Parameters.Add("@value", Logo.Id);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			if (Logo == null)
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsLogoFk, id, Cache.DefaultSettingsValidationTime)] = null;
			else
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsLogoFk, id, Cache.DefaultSettingsValidationTime)] = Logo.Id;
		}

		/// <summary>
		/// Gets the random pool.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetRandomPool(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsRandomPool, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsRandomPool, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the random pool.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="RandomPool">The random pool.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetRandomPool(int id, bool? RandomPool)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET random_pool=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", RandomPool.HasValue ? RandomPool : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsRandomPool, id, Cache.DefaultSettingsValidationTime)] = RandomPool;
		}

		/// <summary>
		/// Gets the self assessment.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetSelfAssessment(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsSelfAssessment, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsSelfAssessment, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the self assessment.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="SelfAssessment">The self assessment.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetSelfAssessment(int id, bool? SelfAssessment)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET self_assessment=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", SelfAssessment.HasValue ? SelfAssessment : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelfAssessment, id, Cache.DefaultSettingsValidationTime)] = SelfAssessment;
		}

		/// <summary>
		/// Gets the show images.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetShowImages(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsShowImages, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsShowImages, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the show images.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="ShowImages">The show images.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetShowImages(int id, bool? ShowImages)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET show_images=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", ShowImages.HasValue ? ShowImages : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsShowImages, id, Cache.DefaultSettingsValidationTime)] = ShowImages;
		}

		/// <summary>
		/// Gets the show statistics.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetShowStatistics(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsShowStatistics, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsShowStatistics, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the show statistics.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="ShowStatistics">The show statistics.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetShowStatistics(int id, bool? ShowStatistics)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET show_statistics=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", ShowStatistics.HasValue ? ShowStatistics : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsShowStatistics, id, Cache.DefaultSettingsValidationTime)] = ShowStatistics;
		}

		/// <summary>
		/// Gets the skip correct answers.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetSkipCorrectAnswers(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsSkipCorrectAnswers, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsSkipCorrectAnswers, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the skip correct answers.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="SkipCorrectAnswers">The skip correct answers.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetSkipCorrectAnswers(int id, bool? SkipCorrectAnswers)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET skip_correct_answers=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", SkipCorrectAnswers.HasValue ? SkipCorrectAnswers : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSkipCorrectAnswers, id, Cache.DefaultSettingsValidationTime)] = SkipCorrectAnswers;
		}

		/// <summary>
		/// Gets the strip chars.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public string GetStripChars(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsStripchars, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsStripchars, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as string;
		}
		/// <summary>
		/// Sets the strip chars.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="StripChars">The strip chars.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetStripChars(int id, string StripChars)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET stripchars=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", StripChars);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStripchars, id, Cache.DefaultSettingsValidationTime)] = StripChars ?? string.Empty;
		}

		/// <summary>
		/// Gets the pool empty message.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetPoolEmptyMessage(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsPoolEmptyMessageShown, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsPoolEmptyMessageShown, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the pool empty message.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="PoolEmtyMessage">The pool emty message.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetPoolEmptyMessage(int id, bool? PoolEmtyMessage)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET pool_empty_message_shown=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", PoolEmtyMessage.HasValue ? PoolEmtyMessage : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsPoolEmptyMessageShown, id, Cache.DefaultSettingsValidationTime)] = PoolEmtyMessage;
		}

		/// <summary>
		/// Gets the use lm stylesheets.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetUseLmStylesheets(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsUseLearningModuleStylesheet, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsUseLearningModuleStylesheet, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the use lm stylesheets.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="UseLmStylesheets">The use lm stylesheets.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetUseLmStylesheets(int id, bool? UseLmStylesheets)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET use_lm_stylesheets=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", UseLmStylesheets.HasValue ? UseLmStylesheets : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsUseLearningModuleStylesheet, id, Cache.DefaultSettingsValidationTime)] = UseLmStylesheets;
		}

		/// <summary>
		/// Gets the selected learning chapters.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public IList<int> GetSelectedLearningChapters(int id)
		{
			IList<int> cachedList;
			if ((cachedList = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsSelectedLearnChapterList, id)] as IList<int>) == null)
			{
				SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd.CommandText = "SELECT chapters_id as id FROM \"SelectedLearnChapters\" WHERE settings_id=@id";
				cmd.Parameters.Add("@id", id);

				SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

				cachedList = new List<int>();
				while (reader.Read())
					cachedList.Add(Convert.ToInt32(reader["id"]));
				reader.Close();

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelectedLearnChapterList, id, Cache.DefaultSettingsValidationTime)] = cachedList;
			}

			return cachedList;
		}
		/// <summary>
		/// Sets the selected learning chapters.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="SelectedLearningChapters">The selected learning chapters.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetSelectedLearningChapters(int id, IList<int> SelectedLearningChapters)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "DELETE FROM \"SelectedLearnChapters\" WHERE settings_id=@id; ";
			cmd.Parameters.Add("@id", id);

			if (SelectedLearningChapters != null && SelectedLearningChapters.Count > 0)
			{
				int i = 0;
				foreach (int cid in SelectedLearningChapters)
				{
					cmd.CommandText += "INSERT INTO \"SelectedLearnChapters\" (settings_id, chapters_id) VALUES ";
					cmd.CommandText += "(@id, @cid" + ((int)(++i)).ToString() + "); ";
					cmd.Parameters.Add("@cid" + i, cid);
				}
			}
			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelectedLearnChapterList, id, Cache.DefaultSettingsValidationTime)] = SelectedLearningChapters;
		}

		/// <summary>
		/// Gets the size of the auto box.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public bool? GetAutoBoxSize(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsAutoBoxsize, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsAutoBoxsize, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as bool?;
		}
		/// <summary>
		/// Sets the size of the auto box.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="AutoBoxSize">Size of the auto box.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetAutoBoxSize(int id, bool? AutoBoxSize)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "UPDATE \"Settings\" SET auto_boxsize=@value WHERE id=@id";
			cmd.Parameters.Add("@id", id);
			cmd.Parameters.Add("@value", AutoBoxSize.HasValue ? AutoBoxSize : DBNull.Value as object);

			MSSQLCEConn.ExecuteNonQuery(cmd);

			//Save to Cache
			Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAutoBoxsize, id, Cache.DefaultSettingsValidationTime)] = AutoBoxSize;
		}

		/// <summary>
		/// Gets the card style.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public ICardStyle GetCardStyle(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsCardStyleFk, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsCardStyleFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
			int? cardStyleId = cacheValue as int?;
			return cardStyleId.HasValue ? new DbCardStyle(cardStyleId.Value, false, Parent) : null;
		}
		/// <summary>
		/// Sets the card style.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="Style">The style.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetCardStyle(int id, ICardStyle Style)
		{
			if (Style is DbCardStyle)
			{
				SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd.CommandText = "UPDATE \"Settings\" SET cardstyle=@csid WHERE id=@id;";
				cmd.Parameters.Add("@id", id);
				cmd.Parameters.Add("@csid", (Style as DbCardStyle).Id);

				MSSQLCEConn.ExecuteNonQuery(cmd);
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "UPDATE \"CardStyles\" SET value=@style WHERE id=@csid;";
				cmd2.Parameters.Add("@csid", (Style as DbCardStyle).Id);
				cmd2.Parameters.Add("@style", Style.Xml);

				MSSQLCEConn.ExecuteNonQuery(cmd2);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleFk, id, Cache.DefaultSettingsValidationTime)] = (Style as DbCardStyle).Id;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleValue, id, Cache.DefaultSettingsValidationTime)] = Style.Xml;
			}
			else
			{
				SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(@style); UPDATE \"Settings\" SET cardstyle=@@IDENTITY WHERE id=@id; SELECT @@IDENTITY;";
				cmd.Parameters.Add("@id", id);
				cmd.Parameters.Add("@style", (Style != null) ? Style.Xml : String.Empty);

				//MSSQLCEConn.ExecuteNonQuery(cmd);
				int newId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleFk, id, Cache.DefaultSettingsValidationTime)] = newId;
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleValue, id, Cache.DefaultSettingsValidationTime)] = (Style != null) ? Style.Xml : String.Empty;
			}
		}

		/// <summary>
		/// Gets the question stylesheet.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public string GetQuestionStylesheet(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsStyleSheetsQuestionValue, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsStyleSheetsQuestionValue, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as string;
		}
		/// <summary>
		/// Sets the question stylesheet.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="QuestionStylesheet">The question stylesheet.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetQuestionStylesheet(int id, string QuestionStylesheet)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT question_stylesheet FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int? sid = MSSQLCEConn.ExecuteScalar<int>(cmd);

			if (sid.HasValue)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "DELETE FROM \"StyleSheets\" WHERE id=@id";
				cmd2.Parameters.Add("@id", sid);

				MSSQLCEConn.ExecuteNonQuery(cmd2);
			}

			if (QuestionStylesheet != null && QuestionStylesheet != string.Empty)
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"StyleSheets\" (value) VALUES (@value); " +
					"UPDATE \"Settings\" SET question_stylesheet=@@IDENTITY WHERE id=@id;";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@value", QuestionStylesheet);

				MSSQLCEConn.ExecuteNonQuery(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStyleSheetsQuestionValue, id, Cache.DefaultSettingsValidationTime)] = QuestionStylesheet ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets the answer stylesheet.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public string GetAnswerStylesheet(int id)
		{
			object cacheValue;
			if (!SettingsCached(id, CacheObject.SettingsStyleSheetsAnswerValue, out cacheValue))      //if settings are not in Cache --> load them
				GetSettingsValue(id, CacheObject.SettingsStyleSheetsAnswerValue, out cacheValue);    //Saves the current Settings from the DB to the Cache
			return cacheValue as string;
		}
		/// <summary>
		/// Sets the answer stylesheet.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="AnswerStylesheet">The answer stylesheet.</param>
		/// <remarks>Documented by Dev05, 2009-01-15</remarks>
		public void SetAnswerStylesheet(int id, string AnswerStylesheet)
		{
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "SELECT answer_stylesheet FROM \"Settings\" WHERE id=@id";
			cmd.Parameters.Add("@id", id);

			int? sid = MSSQLCEConn.ExecuteScalar<int>(cmd);

			if (sid.HasValue)
			{
				SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd2.CommandText = "DELETE FROM \"StyleSheets\" WHERE id=@id";
				cmd2.Parameters.Add("@id", sid);

				MSSQLCEConn.ExecuteNonQuery(cmd2);
			}

			if (AnswerStylesheet != null && AnswerStylesheet != string.Empty)
			{
				SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
				cmd3.CommandText = "INSERT INTO \"StyleSheets\" (value) VALUES (@value); " +
					"UPDATE \"Settings\" SET answer_stylesheet=@@IDENTITY WHERE id=@id;";
				cmd3.Parameters.Add("@id", id);
				cmd3.Parameters.Add("@value", AnswerStylesheet);

				MSSQLCEConn.ExecuteNonQuery(cmd3);

				//Save to Cache
				Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStyleSheetsAnswerValue, id, Cache.DefaultSettingsValidationTime)] = AnswerStylesheet ?? string.Empty;
			}
		}

		#endregion

		/// <summary>
		/// Creates the new settings. (stored procedure from pgsql
		/// </summary>
		/// <param name="Parent">The parent.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by FabThe, 12.1.2009
		/// </remarks>
		public static int CreateNewSettings(ParentClass Parent)
		{
			return CreateNewSettings(Parent, false);
		}

		/// <summary>
		/// Creates the new settings. (stored procedure from pgsql
		/// </summary>
		/// <param name="Parent">The parent.</param>
		/// <param name="defaultBoxSizes">if set to <c>true</c> [default box sizes].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-12</remarks>
		public static int CreateNewSettings(ParentClass Parent, bool defaultBoxSizes)
		{
			//1. SnoozeOptions Table
			int snoozeOptionsId;
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"SnoozeOptions\"(cards_enabled,rights_enabled,time_enabled)	VALUES(null,null,null); SELECT @@IDENTITY";
			snoozeOptionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//2. MultipleChoiceOptions Table
			int multipleChoiceOptionsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO\"MultipleChoiceOptions\" (allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices)" +
							  "VALUES(null, null, null, null); SELECT @@IDENTITY";
			multipleChoiceOptionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//3. QueryTypes Table
			int queryTypesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"QueryTypes\"(image_recognition, listening_comprehension, multiple_choice, sentence, word)" +
							  "VALUES(null, null, null, null, null); SELECT @@IDENTITY";
			queryTypesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//4. TypeGradings Table
			int typeGradingsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"TypeGradings\"(all_correct, half_correct, none_correct, prompt) VALUES(null, null, null, null); SELECT @@IDENTITY";
			typeGradingsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//5. SynonymGradings Table
			int synonymGradingsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"SynonymGradings\"(all_known, half_known, one_known, first_known, prompt) VALUES(null, null, null, null, null); SELECT @@IDENTITY";
			synonymGradingsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//6. QueryDirections Table
			int queryDirectionsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"QueryDirections\"(question2answer, answer2question, mixed)	VALUES(null, null, null); SELECT @@IDENTITY";
			queryDirectionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//7. CardStyles Table
			int cardStylesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(NULL); SELECT @@IDENTITY";
			cardStylesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//8. Boxes Table
			int boxesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"Boxes\"(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size) " +
								(defaultBoxSizes ? "VALUES(10, 20, 50, 100, 250, 500, 1000, 2000, 4000);" :
							  "VALUES(null, null, null, null, null, null, null, null, null);") +
							  " SELECT @@IDENTITY";
			boxesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//9. Settings Table --> insert all foreign keys into the settings table:
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"Settings\"" +
							  "(snooze_options, multiple_choice_options, query_types, type_gradings, synonym_gradings, query_directions, cardstyle, boxes, " +
							  "autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, " +
							  "show_images, stripchars, auto_boxsize, pool_empty_message_shown, show_statistics, skip_correct_answers, use_lm_stylesheets, question_culture, answer_culture)" +
							  "VALUES(@snooze_options_id, @multiple_choice_options_id, @query_types_id, @type_gradings_id, @synonym_gradings_id, @query_directions_id, " +
							  "@card_styles_id, @boxes_id, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 'en', 'en'); SELECT @@IDENTITY";
			cmd.Parameters.Add("@snooze_options_id", snoozeOptionsId);
			cmd.Parameters.Add("@multiple_choice_options_id", multipleChoiceOptionsId);
			cmd.Parameters.Add("@query_types_id", queryTypesId);
			cmd.Parameters.Add("@type_gradings_id", typeGradingsId);
			cmd.Parameters.Add("@synonym_gradings_id", synonymGradingsId);
			cmd.Parameters.Add("@query_directions_id", queryDirectionsId);
			cmd.Parameters.Add("@card_styles_id", cardStylesId);
			cmd.Parameters.Add("@boxes_id", boxesId);
			return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
		}

		public static int CreateNewDefaultSettings(ParentClass Parent)
		{
			//1. SnoozeOptions Table
			int snoozeOptionsId;
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"SnoozeOptions\"(cards_enabled,rights_enabled,time_enabled)	" +
				"VALUES(0,0,0); SELECT @@IDENTITY";
			snoozeOptionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//2. MultipleChoiceOptions Table
			int multipleChoiceOptionsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO\"MultipleChoiceOptions\" (allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices)" +
							  "VALUES(0, 1, 1, 4); SELECT @@IDENTITY";
			multipleChoiceOptionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//3. QueryTypes Table
			int queryTypesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"QueryTypes\"(image_recognition, listening_comprehension, multiple_choice, sentence, word)" +
							  "VALUES(0, 0, 1, 0, 1); SELECT @@IDENTITY";
			queryTypesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//4. TypeGradings Table
			int typeGradingsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"TypeGradings\"(all_correct, half_correct, none_correct, prompt) " +
				"VALUES(0, 1, 0, 0); SELECT @@IDENTITY";
			typeGradingsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//5. SynonymGradings Table
			int synonymGradingsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"SynonymGradings\"(all_known, half_known, one_known, first_known, prompt) " +
				"VALUES(0, 0, 1, 0, 0); SELECT @@IDENTITY";
			synonymGradingsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//6. QueryDirections Table
			int queryDirectionsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"QueryDirections\"(question2answer, answer2question, mixed)	" +
				"VALUES(1, 0, 0); SELECT @@IDENTITY";
			queryDirectionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//7. CardStyles Table
			int cardStylesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(NULL); SELECT @@IDENTITY";
			cardStylesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//8. Boxes Table
			int boxesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"Boxes\"(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size) " +
								"VALUES(10, 20, 50, 100, 250, 500, 1000, 2000, 4000);" +
							  " SELECT @@IDENTITY";
			boxesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//9. Settings Table --> insert all foreign keys into the settings table:
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"Settings\"" +
							  "(snooze_options, multiple_choice_options, query_types, type_gradings, synonym_gradings, query_directions, cardstyle, boxes, " +
							  "autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, " +
							  "show_images, stripchars, auto_boxsize, pool_empty_message_shown, show_statistics, skip_correct_answers, use_lm_stylesheets, question_culture, answer_culture)" +
							  "VALUES(@snooze_options_id, @multiple_choice_options_id, @query_types_id, @type_gradings_id, @synonym_gradings_id, @query_directions_id, " +
							  "@card_styles_id, @boxes_id, 1, 0, 0, 0, 0, 0, 1, 0, 1, @stripchars, 0, 0, 1, 0, 1, 'en', 'en'); SELECT @@IDENTITY";
			cmd.Parameters.Add("@snooze_options_id", snoozeOptionsId);
			cmd.Parameters.Add("@multiple_choice_options_id", multipleChoiceOptionsId);
			cmd.Parameters.Add("@query_types_id", queryTypesId);
			cmd.Parameters.Add("@type_gradings_id", typeGradingsId);
			cmd.Parameters.Add("@synonym_gradings_id", synonymGradingsId);
			cmd.Parameters.Add("@query_directions_id", queryDirectionsId);
			cmd.Parameters.Add("@card_styles_id", cardStylesId);
			cmd.Parameters.Add("@boxes_id", boxesId);
			cmd.Parameters.Add("@stripchars", "!,.?;");
			return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
		}

		/// <summary>
		/// Creates the new settings with values for allowed settings.
		/// </summary>
		/// <param name="Parent">The parent.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by FabThe, 12.1.2009
		/// </remarks>
		public static int CreateNewAllowedSettings(ParentClass Parent)
		{
			//1. SnoozeOptions Table
			int snoozeOptionsId;
			SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"SnoozeOptions\"(cards_enabled,rights_enabled,time_enabled)	VALUES(1,1,1); SELECT @@IDENTITY";
			snoozeOptionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//2. MultipleChoiceOptions Table
			int multipleChoiceOptionsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO\"MultipleChoiceOptions\" (allow_multiple_correct_answers, allow_random_distractors)" +
							  "VALUES(1, 1); SELECT @@IDENTITY";
			multipleChoiceOptionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//3. QueryTypes Table
			int queryTypesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"QueryTypes\"(image_recognition, listening_comprehension, multiple_choice, sentence, word)" +
							  "VALUES(1, 1, 1, 1, 1); SELECT @@IDENTITY";
			queryTypesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//4. TypeGradings Table
			int typeGradingsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"TypeGradings\"(all_correct, half_correct, none_correct, prompt) VALUES(1, 1, 1, 1); SELECT @@IDENTITY";
			typeGradingsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//5. SynonymGradings Table
			int synonymGradingsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"SynonymGradings\"(all_known, half_known, one_known, first_known, prompt) VALUES(1, 1, 1, 1, 1); SELECT @@IDENTITY";
			synonymGradingsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//6. QueryDirections Table
			int queryDirectionsId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"QueryDirections\"(question2answer, answer2question, mixed)	VALUES(1, 1, 1); SELECT @@IDENTITY";
			queryDirectionsId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//7. CardStyles Table
			int cardStylesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(NULL); SELECT @@IDENTITY";
			cardStylesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//8. Boxes Table
			int boxesId;
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"Boxes\"(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size) " +
							  "VALUES(null, null, null, null, null, null, null, null, null); SELECT @@IDENTITY";
			boxesId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

			//9. Settings Table --> insert all foreign keys into the settings table:
			cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
			cmd.CommandText = "INSERT INTO \"Settings\"" +
							  "(snooze_options, multiple_choice_options, query_types, type_gradings, synonym_gradings, query_directions, cardstyle, boxes, " +
							  "autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, " +
							  "show_images, stripchars, auto_boxsize, pool_empty_message_shown, show_statistics, skip_correct_answers, use_lm_stylesheets, question_culture, answer_culture)" +
							  "VALUES(@snooze_options_id, @multiple_choice_options_id, @query_types_id, @type_gradings_id, @synonym_gradings_id, @query_directions_id, " +
							  "@card_styles_id, @boxes_id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 'en', 'en'); SELECT @@IDENTITY";
			cmd.Parameters.Add("@snooze_options_id", snoozeOptionsId);
			cmd.Parameters.Add("@multiple_choice_options_id", multipleChoiceOptionsId);
			cmd.Parameters.Add("@query_types_id", queryTypesId);
			cmd.Parameters.Add("@type_gradings_id", typeGradingsId);
			cmd.Parameters.Add("@synonym_gradings_id", synonymGradingsId);
			cmd.Parameters.Add("@query_directions_id", queryDirectionsId);
			cmd.Parameters.Add("@card_styles_id", cardStylesId);
			cmd.Parameters.Add("@boxes_id", boxesId);
			return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
		}
	}
}
