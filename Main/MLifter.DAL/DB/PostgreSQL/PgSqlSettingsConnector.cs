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
using System.Globalization;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;
using Npgsql;
using System.ComponentModel;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlSettingsConnector : IDbSettingsConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlSettingsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlSettingsConnector>();
        public static PgSqlSettingsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlSettingsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlSettingsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        /******************************************************************************************/
        //Global caching functions
        /******************************************************************************************/
        private void GetSettingsValue(int settingsId, CacheObject cacheObjectType, out object cacheValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"GetAllSettings\"(:id)";
                    cmd.Parameters.Add("id", settingsId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();
                    //read the values
                    bool? autoplayAudio = DbValueConverter.Convert<bool>(reader["autoplay_audio"]);
                    bool? caseSensitive = DbValueConverter.Convert<bool>(reader["case_sensitive"]);
                    bool? confirmDemote = DbValueConverter.Convert<bool>(reader["confirm_demote"]);
                    bool? enableCommentary = DbValueConverter.Convert<bool>(reader["enable_commentary"]);
                    bool? correctOnTheFly = DbValueConverter.Convert<bool>(reader["correct_on_the_fly"]);
                    bool? enableTimer = DbValueConverter.Convert<bool>(reader["enable_timer"]);
                    int? synonymGradingsId = DbValueConverter.Convert<int>(reader["synonymGradingsId"]);    //FK
                    int? typeGradingsId = DbValueConverter.Convert<int>(reader["typeGradingsId"]);    //FK
                    int? multipleChoiceOptionsId = DbValueConverter.Convert<int>(reader["multipleChoiceOptionsId"]);    //FK
                    int? queryDirectionsId = DbValueConverter.Convert<int>(reader["queryDirectionsId"]);    //FK
                    int? queryTypesId = DbValueConverter.Convert<int>(reader["queryTypesId"]);    //FK
                    bool? randomPool = DbValueConverter.Convert<bool>(reader["random_pool"]);
                    bool? selfAssessment = DbValueConverter.Convert<bool>(reader["self_assessment"]);
                    bool? showImages = DbValueConverter.Convert<bool>(reader["show_images"]);
                    string stripChars = DbValueConverter.Convert(reader["stripchars"]);
                    string questionCulture = DbValueConverter.Convert(reader["question_culture"]);
                    string answerCulture = DbValueConverter.Convert(reader["answer_culture"]);
                    string questionCaption = DbValueConverter.Convert(reader["question_caption"]);
                    string answerCaption = DbValueConverter.Convert(reader["answer_caption"]);
                    int? logoId = DbValueConverter.Convert<int>(reader["logo"]);    //FK
                    bool? autoBoxsize = DbValueConverter.Convert<bool>(reader["auto_boxsize"]);
                    bool? poolEmptyMessageShown = DbValueConverter.Convert<bool>(reader["pool_empty_message_shown"]);
                    bool? showStatistics = DbValueConverter.Convert<bool>(reader["show_statistics"]);
                    bool? skipCorrectAnswers = DbValueConverter.Convert<bool>(reader["skip_correct_answers"]);
                    int? snoozeOptionsId = DbValueConverter.Convert<int>(reader["snoozeOptionsId"]);    //FK
                    bool? useLmStylesheets = DbValueConverter.Convert<bool>(reader["use_lm_stylesheets"]);
                    int? cardStyleId = DbValueConverter.Convert<int>(reader["cardStyleId"]);    //FK
                    int? boxesId = DbValueConverter.Convert<int>(reader["boxesId"]);    //FK
                    bool? isCached = DbValueConverter.Convert<bool>(reader["isCached"]);
                    //StyleSheets
                    string questionStylesheet = DbValueConverter.Convert(reader["question_stylesheet"]);
                    string answerStylesheet = DbValueConverter.Convert(reader["answer_stylesheet"]);
                    string cardStyle = DbValueConverter.Convert(reader["cardstyle"]);

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
                    //System.Diagnostics.Trace.Assert(cacheValue == null);
                }
            }
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

        public void CheckSettingsId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int? count = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (!count.HasValue || count.Value < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public IQueryDirections GetQueryDirections(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryDirectionsFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryDirectionsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? queryDirectionsId = cacheValue as int?;
            return queryDirectionsId.HasValue && queryDirectionsId.Value > 0 ? new DbQueryDirections(queryDirectionsId.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT query_directions FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int? qdId = PostgreSQLConn.ExecuteScalar<int>(cmd);

            //        return qdId.HasValue && qdId.Value > 0 ? new DbQueryDirections(qdId.Value, false, Parent) : null;
            //    }
            //}
        }
        public void SetQueryDirections(int id, IQueryDirections QueryDirections)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT query_directions FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int? qdid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (qdid.HasValue && qdid.Value > 0)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"QueryDirections\" SET question2answer=:q2a, answer2question=:a2q, mixed=:mixed WHERE id=:id";
                            cmd2.Parameters.Add("id", qdid.Value);
                            cmd2.Parameters.Add("q2a", QueryDirections.Question2Answer);
                            cmd2.Parameters.Add("a2q", QueryDirections.Answer2Question);
                            cmd2.Parameters.Add("mixed", QueryDirections.Mixed);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsFk, id, Cache.DefaultSettingsValidationTime)] = qdid;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, qdid.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Question2Answer;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, qdid.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Answer2Question;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, qdid.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Mixed;
                        }
                    }
                    else
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"QueryDirections\" (question2answer, answer2question, mixed) VALUES " +
                                "(:q2a, :a2q, :mixed); UPDATE \"Settings\" SET query_directions=currval('\"QueryDirections_id_seq\"') WHERE id=:id;" +
                                "SELECT query_directions FROM \"Settings\" WHERE id=:id";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("q2a", QueryDirections.Question2Answer);
                            cmd3.Parameters.Add("a2q", QueryDirections.Answer2Question);
                            cmd3.Parameters.Add("mixed", QueryDirections.Mixed);

                            int? queryDirectionsId = PostgreSQLConn.ExecuteScalar<int>(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsFk, id, Cache.DefaultSettingsValidationTime)] = queryDirectionsId;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsQuestion2Answer, queryDirectionsId.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Question2Answer;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsAnswer2Question, queryDirectionsId.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Answer2Question;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryDirectionsMixed, queryDirectionsId.Value, Cache.DefaultSettingsValidationTime)] = QueryDirections.Mixed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the type of the query.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-13</remarks>
        public IQueryType GetQueryType(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsQueryTypesFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsQueryTypesFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? qtid = cacheValue as int?;
            return qtid.HasValue && qtid.Value > 0 ? new DbQueryTypes(qtid.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT query_types FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int qtid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

            //        return qtid > 0 ? new DbQueryTypes(qtid, false, Parent) : null;
            //    }
            //}
        }
        /// <summary>
        /// Sets the type of the query.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="QueryType">Type of the query.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetQueryType(int id, IQueryType QueryType)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT query_types FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int qtid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                    if (qtid > 0)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"QueryTypes\" SET " +
                                "image_recognition=:i_r, listening_comprehension=:l_c, multiple_choice=:m_c, sentence=:s, word=:w WHERE id=:id";
                            cmd2.Parameters.Add("id", qtid);
                            cmd2.Parameters.Add("i_r", QueryType.ImageRecognition);
                            cmd2.Parameters.Add("l_c", QueryType.ListeningComprehension);
                            cmd2.Parameters.Add("m_c", QueryType.MultipleChoice);
                            cmd2.Parameters.Add("s", QueryType.Sentence);
                            cmd2.Parameters.Add("w", QueryType.Word);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesFk, id, Cache.DefaultSettingsValidationTime)] = qtid;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.ImageRecognition;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.ListeningComprehension;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.MultipleChoice;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.Sentence;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, qtid, Cache.DefaultSettingsValidationTime)] = QueryType.Word;
                        }
                    }
                    else
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"QueryTypes\" (image_recognition, listening_comprehension, multiple_choice, sentence, word) VALUES " +
                                "(:i_r, :l_c, :m_c, :s, :w); UPDATE \"Settings\" SET query_types=currval('\"QueryTypes_id_seq\"') WHERE id=:id; " +
                                "SELECT query_types FROM \"Settings\" WHERE id=:id";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("i_r", QueryType.ImageRecognition);
                            cmd3.Parameters.Add("l_c", QueryType.ListeningComprehension);
                            cmd3.Parameters.Add("m_c", QueryType.MultipleChoice);
                            cmd3.Parameters.Add("s", QueryType.Sentence);
                            cmd3.Parameters.Add("w", QueryType.Word);

                            int? queryTypeId = PostgreSQLConn.ExecuteScalar<int>(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesFk, id, Cache.DefaultSettingsValidationTime)] = queryTypeId;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesImageRecognition, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.ImageRecognition;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesListeningComprehension, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.ListeningComprehension;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesMultipleChoice, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.MultipleChoice;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesSentence, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.Sentence;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsQueryTypesWord, queryTypeId.Value, Cache.DefaultSettingsValidationTime)] = QueryType.Word;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the snooze options.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public ISnoozeOptions GetSnoozeOptions(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSnoozeOptionsFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSnoozeOptionsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? snoozeOptionsId = cacheValue as int?;
            return snoozeOptionsId.HasValue && snoozeOptionsId.Value > 0 ? new DbSnoozeOptions(snoozeOptionsId.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT snooze_options FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int soid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd));
            //        return soid > 0 ? new DbSnoozeOptions(soid, false, Parent) : null;
            //    }
            //}
        }
        /// <summary>
        /// Sets the snooze options.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="SnoozeOptions">The snooze options.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetSnoozeOptions(int id, ISnoozeOptions SnoozeOptions)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT snooze_options FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int soid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                    if (soid > 0)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"SnoozeOptions\" SET " +
                                "cards_enabled=:c_e, rights_enabled=:r_e, time_enabled=:t_e, snooze_cards=:s_c, snooze_hight=:s_h, snooze_low=:s_l, " +
                                "snooze_mode=:s_m, snooze_rights=:s_r, snooze_time=:s_t WHERE id=:id";
                            cmd2.Parameters.Add("id", soid);
                            cmd2.Parameters.Add("c_e", SnoozeOptions.IsCardsEnabled);
                            cmd2.Parameters.Add("r_e", SnoozeOptions.IsRightsEnabled);
                            cmd2.Parameters.Add("t_e", SnoozeOptions.IsTimeEnabled);
                            cmd2.Parameters.Add("s_c", SnoozeOptions.SnoozeCards);
                            cmd2.Parameters.Add("s_h", SnoozeOptions.SnoozeHigh);
                            cmd2.Parameters.Add("s_l", SnoozeOptions.SnoozeLow);
                            cmd2.Parameters.Add("s_m", SnoozeOptions.SnoozeMode);
                            cmd2.Parameters.Add("s_r", SnoozeOptions.SnoozeRights);
                            cmd2.Parameters.Add("s_t", SnoozeOptions.SnoozeTime);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

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
                    }
                    else
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"SnoozeOptions\" " +
                                "(cards_enabled, rights_enabled, time_enabled, snooze_cards, snooze_hight, snooze_low, snooze_mode, snooze_rights, snooze_time) VALUES " +
                                "(:c_e, :r_e, :t_e, :s_c, :s_h, :s_l, :s_m, :s_r, :s_t); UPDATE \"Settings\" SET snooze_options=currval('\"SnoozeOptions_id_seq\"') WHERE id=:id; " +
                                "SELECT snooze_options FROM \"Settings\" WHERE id=:id";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("c_e", SnoozeOptions.IsCardsEnabled);
                            cmd3.Parameters.Add("r_e", SnoozeOptions.IsRightsEnabled);
                            cmd3.Parameters.Add("t_e", SnoozeOptions.IsTimeEnabled);
                            cmd3.Parameters.Add("s_c", SnoozeOptions.SnoozeCards);
                            cmd3.Parameters.Add("s_h", SnoozeOptions.SnoozeHigh);
                            cmd3.Parameters.Add("s_l", SnoozeOptions.SnoozeLow);
                            cmd3.Parameters.Add("s_m", SnoozeOptions.SnoozeMode);
                            cmd3.Parameters.Add("s_r", SnoozeOptions.SnoozeRights);
                            cmd3.Parameters.Add("s_t", SnoozeOptions.SnoozeTime);

                            //PostgreSQLConn.ExecuteNonQuery(cmd3);
                            int? snoozeOptionsId = PostgreSQLConn.ExecuteScalar<int>(cmd3, Parent.CurrentUser);

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
                }
            }
        }

        /// <summary>
        /// Gets the multiple choice options.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public IQueryMultipleChoiceOptions GetMultipleChoiceOptions(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsMultipleChoiceOptionsFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsMultipleChoiceOptionsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? mcoid = cacheValue as int?;
            return mcoid.HasValue && mcoid.Value > 0 ? new DbQueryMultipleChoiceOptions(mcoid.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT multiple_choice_options FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int mcoid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd));
            //        return mcoid > 0 ? new DbQueryMultipleChoiceOptions(mcoid, false, Parent) : null;

            //    }
            //}
        }
        /// <summary>
        /// Sets the multiple choice options.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="MultipleChoiceOptions">The multiple choice options.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetMultipleChoiceOptions(int id, IQueryMultipleChoiceOptions MultipleChoiceOptions)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT multiple_choice_options FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int mcoid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                    if (mcoid > 0)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"MultipleChoiceOptions\" SET " +
                                "allow_multiple_correct_answers=:a_m_c_a, allow_random_distractors=:r_d, max_correct_answers=:m_c_a, number_of_choices=:n_o_c WHERE id=:id";
                            cmd2.Parameters.Add("id", mcoid);
                            cmd2.Parameters.Add("a_m_c_a", MultipleChoiceOptions.AllowMultipleCorrectAnswers);
                            cmd2.Parameters.Add("r_d", MultipleChoiceOptions.AllowRandomDistractors);
                            cmd2.Parameters.Add("m_c_a", MultipleChoiceOptions.MaxNumberOfCorrectAnswers);
                            cmd2.Parameters.Add("n_o_c", MultipleChoiceOptions.NumberOfChoices);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsFk, id, Cache.DefaultSettingsValidationTime)] = mcoid;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowMultipleCorrectAnswers;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowRandomDistractors;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, mcoid, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.NumberOfChoices;
                        }
                    }
                    else
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"MultipleChoiceOptions\" " +
                                "(allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices) VALUES " +
                                "(:a_m_c_a, :r_d, :m_c_a, :n_o_c); UPDATE \"Settings\" SET multiple_choice_options=currval('\"MultipleChoiceOptions_id_seq\"') WHERE id=:id; " +
                                "SELECT multiple_choice_options FROM \"Settings\" WHERE id=:id";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("a_m_c_a", MultipleChoiceOptions.AllowMultipleCorrectAnswers);
                            cmd3.Parameters.Add("r_d", MultipleChoiceOptions.AllowRandomDistractors);
                            cmd3.Parameters.Add("m_c_a", MultipleChoiceOptions.MaxNumberOfCorrectAnswers);
                            cmd3.Parameters.Add("n_o_c", MultipleChoiceOptions.NumberOfChoices);

                            int? multipleChoiceOptionsId = PostgreSQLConn.ExecuteScalar<int>(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsFk, id, Cache.DefaultSettingsValidationTime)] = multipleChoiceOptionsId;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowMultipleCorrectAnswers, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowMultipleCorrectAnswers;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsAllowRandomDistractors, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.AllowRandomDistractors;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsMaxCorrectAnswers, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsMultipleChoiceOptionsNumberOfChoices, multipleChoiceOptionsId.Value, Cache.DefaultSettingsValidationTime)] = MultipleChoiceOptions.NumberOfChoices;

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the grade typing.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public IGradeTyping GetGradeTyping(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsTypeGradingsFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsTypeGradingsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? gtid = cacheValue as int?;
            return gtid.HasValue && gtid.Value > 0 ? new DbGradeTyping(gtid.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT type_gradings FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int gtid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd));
            //        return gtid > 0 ? new DbGradeTyping(gtid, false, Parent) : null;
            //    }
            //}
        }
        /// <summary>
        /// Sets the grade typing.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="GradeTyping">The grade typing.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetGradeTyping(int id, IGradeTyping GradeTyping)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT type_gradings FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int gtid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                    if (gtid > 0)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"TypeGradings\" SET all_correct=:a_c, half_correct=:h_c, none_correct=:n_c, prompt=:p WHERE id=:id";
                            cmd2.Parameters.Add("id", gtid);
                            cmd2.Parameters.Add("a_c", GradeTyping.AllCorrect);
                            cmd2.Parameters.Add("h_c", GradeTyping.HalfCorrect);
                            cmd2.Parameters.Add("n_c", GradeTyping.NoneCorrect);
                            cmd2.Parameters.Add("p", GradeTyping.Prompt);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsFk, id, Cache.DefaultSettingsValidationTime)] = gtid;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.AllCorrect;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.HalfCorrect;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.NoneCorrect;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, gtid, Cache.DefaultSettingsValidationTime)] = GradeTyping.Prompt;
                        }
                    }
                    else
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"TypeGradings\" (all_correct, half_correct, none_correct, prompt) VALUES " +
                                "(:a_c, :h_c, :n_c, :p); UPDATE \"Settings\" SET type_gradings=currval('\"TypeGradings_id_seq\"') WHERE id=:id";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("a_c", GradeTyping.AllCorrect);
                            cmd3.Parameters.Add("h_c", GradeTyping.HalfCorrect);
                            cmd3.Parameters.Add("n_c", GradeTyping.NoneCorrect);
                            cmd3.Parameters.Add("p", GradeTyping.Prompt);

                            int? gradeTypeingId = PostgreSQLConn.ExecuteScalar<int>(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsFk, id, Cache.DefaultSettingsValidationTime)] = gradeTypeingId;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsAllCorrect, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.AllCorrect;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsHalfCorrect, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.HalfCorrect;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsNoneCorrect, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.NoneCorrect;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsTypeGradingsPrompt, gradeTypeingId.Value, Cache.DefaultSettingsValidationTime)] = GradeTyping.Prompt;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the grade synonyms.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public IGradeSynonyms GetGradeSynonyms(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSynonymGradingsFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSynonymGradingsFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? synonymGradingsId = cacheValue as int?;
            return synonymGradingsId.HasValue && synonymGradingsId.Value > 0 ? new DbGradeSynonyms(synonymGradingsId.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT synonym_gradings FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int gsid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd));
            //        return gsid > 0 ? new DbGradeSynonyms(gsid, false, Parent) : null;
            //    }
            //}
        }
        /// <summary>
        /// Sets the grade synonyms.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="GradeSynonyms">The grade synonyms.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetGradeSynonyms(int id, IGradeSynonyms GradeSynonyms)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT synonym_gradings FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int gsid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                    if (gsid > 0)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"SynonymGradings\" SET all_known=:a_k, half_known=:h_k, one_known=:o_k, first_known=:f_k, prompt=:p WHERE id=:id";
                            cmd2.Parameters.Add("id", gsid);
                            cmd2.Parameters.Add("a_k", GradeSynonyms.AllKnown);
                            cmd2.Parameters.Add("h_k", GradeSynonyms.HalfKnown);
                            cmd2.Parameters.Add("o_k", GradeSynonyms.OneKnown);
                            cmd2.Parameters.Add("f_k", GradeSynonyms.FirstKnown);
                            cmd2.Parameters.Add("p", GradeSynonyms.Prompt);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFk, id, Cache.DefaultSettingsValidationTime)] = gsid;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.AllKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.HalfKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.OneKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.FirstKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, gsid, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.Prompt;
                        }
                    }
                    else
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"SynonymGradings\" (all_known, half_known, one_known, first_known, prompt) VALUES " +
                                "(:a_k, :h_k, :o_k, :f_k, :p); UPDATE \"Settings\" SET synonym_gradings=currval('\"SynonymGradings\"') WHERE id=:id;" +
                                "SELECT synonym_grading FROM \"Settings\" WHERE id=:id";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("a_k", GradeSynonyms.AllKnown);
                            cmd3.Parameters.Add("h_k", GradeSynonyms.HalfKnown);
                            cmd3.Parameters.Add("o_k", GradeSynonyms.OneKnown);
                            cmd3.Parameters.Add("f_k", GradeSynonyms.FirstKnown);
                            cmd3.Parameters.Add("p", GradeSynonyms.Prompt);

                            int? synonymsGrading = PostgreSQLConn.ExecuteScalar<int>(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFk, id, Cache.DefaultSettingsValidationTime)] = synonymsGrading;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsAllKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.AllKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsHalfKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.HalfKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsOneKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.OneKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsFirstKnown, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.FirstKnown;
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSynonymGradingsPrompt, synonymsGrading.Value, Cache.DefaultSettingsValidationTime)] = GradeSynonyms.Prompt;
                        }
                    }
                }
            }
        }

        public Dictionary<CommentarySoundIdentifier, IMedia> GetCommentarySounds(int id)
        {
            if ((Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsCommentarySounds, id)] as Dictionary<CommentarySoundIdentifier, IMedia>) == null)
            {
                Dictionary<CommentarySoundIdentifier, IMedia> commentarySounds = new Dictionary<CommentarySoundIdentifier, IMedia>();
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT media_id, side, type FROM \"CommentarySounds\" WHERE settings_id=:id";
                        cmd.Parameters.Add("id", id);
                        NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                        while (reader.Read())
                        {
                            int mediaid = Convert.ToInt32(reader["media_id"]);
                            Side side = (Side)Enum.Parse(typeof(Side), Convert.ToString(reader["side"]));
                            ECommentarySoundType type = (ECommentarySoundType)Enum.Parse(typeof(ECommentarySoundType), Convert.ToString(reader["type"]));

                            IMedia media = new DbAudio(mediaid, -1, false, side, WordType.Word, true, false, Parent);
                            commentarySounds[CommentarySoundIdentifier.Create(side, type)] = media;
                        }
                    }
                }
                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCommentarySounds, id, Cache.DefaultSettingsValidationTime)] = commentarySounds;
            }

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsCommentarySounds, id)] as Dictionary<CommentarySoundIdentifier, IMedia>;
        }
        public void SetCommentarySounds(int id, Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction trans = con.BeginTransaction();

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"CommentarySounds\" WHERE settings_id=:id";
                    cmd.Parameters.Add("id", id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                foreach (KeyValuePair<CommentarySoundIdentifier, IMedia> commentarySound in CommentarySounds)
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO \"CommentarySounds\" (media_id, settings_id, side, type) VALUES (:mediaid, :id, :side, :type)";
                        cmd.Parameters.Add("mediaid", commentarySound.Value.Id);
                        cmd.Parameters.Add("id", id);
                        cmd.Parameters.Add("side", commentarySound.Key.Side.ToString());
                        cmd.Parameters.Add("type", commentarySound.Key.Type.ToString());
                        PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    }
                }

                trans.Commit();

                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCommentarySounds, id, Cache.DefaultSettingsValidationTime)] = CommentarySounds;
            }
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public CultureInfo GetCulture(int id, Side side)
        {
            CacheObject cacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCulture : CacheObject.SettingsAnswerCulture);
            object cacheValue;
            if (!SettingsCached(id, cacheObjectSide, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, cacheObjectSide, out cacheValue);    //Saves the current Settings from the DB to the Cache
            string culture = (cacheValue != null) ? cacheValue as string : string.Empty;
            return (culture.Length > 0 ? new CultureInfo(culture, false) : null);

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        switch (side)
            //        {
            //            case Side.Question:
            //                cmd.CommandText = "SELECT question_culture FROM \"Settings\" WHERE id=:id";
            //                cmd.Parameters.Add("id", id);
            //                break;
            //            case Side.Answer:
            //                cmd.CommandText = "SELECT answer_culture FROM \"Settings\" WHERE id=:id";
            //                cmd.Parameters.Add("id", id);
            //                break;
            //            default:
            //                break;
            //        }
            //        String gcid = Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd));
            //        return gcid.Length > 0 ? new CultureInfo(gcid, false) : null;
            //    }
            //}
        }
        /// <summary>
        /// Sets the culture.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <param name="Culture">The culture.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetCulture(int id, Side side, CultureInfo Culture)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    switch (side)
                    {
                        case Side.Question:
                            cmd.CommandText = "UPDATE \"Settings\" SET question_culture=:value WHERE id=:id";
                            break;
                        case Side.Answer:
                            cmd.CommandText = "UPDATE \"Settings\" SET answer_culture=:value WHERE id=:id";
                            break;
                        default:
                            break;
                    }

                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", Culture == null ? null : Culture.Name.Length > 0 ? Culture.Name : "en");

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    CacheObject CacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCulture : CacheObject.SettingsAnswerCulture);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObjectSide, id, Cache.DefaultSettingsValidationTime)] = Culture == null ? null : Culture.Name;
                }
            }
        }

        public bool? GetAutoplayAudio(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsAutoPlayAudio, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsAutoPlayAudio, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT autoplay_audio FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAutoplayAudio(int id, bool? AutoplayAudio)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET autoplay_audio=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", AutoplayAudio);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAutoPlayAudio, id, Cache.DefaultSettingsValidationTime)] = AutoplayAudio;
                }
            }
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public string GetCaption(int id, Side side)
        {
            CacheObject cacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCaption : CacheObject.SettingsAnswerCaption);
            object cacheValue;
            if (!SettingsCached(id, cacheObjectSide, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, cacheObjectSide, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as string;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {

            //        switch (side)
            //        {
            //            case Side.Question:
            //                cmd.CommandText = "SELECT question_caption FROM \"Settings\" WHERE id=:id";
            //                cmd.Parameters.Add("id", id);
            //                break;
            //            case Side.Answer:
            //                cmd.CommandText = "SELECT answer_caption FROM \"Settings\" WHERE id=:id";
            //                cmd.Parameters.Add("id", id);
            //                break;
            //            default:
            //                break;
            //        }

            //        String gcid = Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd));
            //        return gcid;
            //    }
            //}
        }
        /// <summary>
        /// Sets the caption.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <param name="Caption">The caption.</param>
        /// <remarks>Documented by Dev11, 2008-08-14</remarks>
        public void SetCaption(int id, Side side, string Caption)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    switch (side)
                    {
                        case Side.Question:
                            cmd.CommandText = "UPDATE \"Settings\" SET question_caption=:value WHERE id=:id";
                            break;
                        case Side.Answer:
                            cmd.CommandText = "UPDATE \"Settings\" SET answer_caption=:value WHERE id=:id";
                            break;
                        default:
                            break;
                    }

                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", Caption);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    CacheObject CacheObjectSide = (side == Side.Question ? CacheObject.SettingsQuestionCaption : CacheObject.SettingsAnswerCaption);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObjectSide, id, Cache.DefaultSettingsValidationTime)] = Caption ?? string.Empty;
                }
            }
        }

        public bool? GetCaseSensitive(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsCaseSensetive, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsCaseSensetive, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT case_sensetive FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetCaseSensitive(int id, bool? CaseSensetive)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET case_sensitive=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", CaseSensetive);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCaseSensetive, id, Cache.DefaultSettingsValidationTime)] = CaseSensetive;
                }
            }
        }

        public bool? GetConfirmDemote(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsConfirmDemote, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsConfirmDemote, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT confirm_demote FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetConfirmDemote(int id, bool? ConfirmDemote)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET confirm_demote=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", ConfirmDemote);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsConfirmDemote, id, Cache.DefaultSettingsValidationTime)] = ConfirmDemote;

                }
            }
        }

        public bool? GetCorrectOnTheFly(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsCorrectOnTheFly, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsCorrectOnTheFly, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT correct_on_the_fly FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetCorrectOnTheFly(int id, bool? CorrectOnTheFly)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET correct_on_the_fly=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", CorrectOnTheFly);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCorrectOnTheFly, id, Cache.DefaultSettingsValidationTime)] = CorrectOnTheFly;
                }
            }
        }

        public bool? GetEnableCommentary(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsEnableCommentary, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsEnableCommentary, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT enable_commentary FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetEnableCommentary(int id, bool? EnableCommentary)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET enable_commentary=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", EnableCommentary);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsEnableCommentary, id, Cache.DefaultSettingsValidationTime)] = EnableCommentary;
                }
            }
        }

        public bool? GetEnableTimer(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsEnableTimer, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsEnableTimer, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT enable_timer FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetEnableTimer(int id, bool? EnableTimer)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET enable_timer=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", EnableTimer);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsEnableTimer, id, Cache.DefaultSettingsValidationTime)] = EnableTimer;
                }
            }
        }

        public IMedia GetLogo(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsLogoFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsLogoFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? logoId = cacheValue as int?;
            return logoId.HasValue ? DbMedia.CreateDisconnectedCardMedia(logoId.Value, EMedia.Image, false, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT logo FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int? lid = PostgreSQLConn.ExecuteScalar<int>(cmd);

            //        return lid.HasValue ? DbMedia.CreateDisconnectedCardMedia(lid.Value, EMedia.Image, false, false, Parent) : null;
            //    }
            //}
        }
        public void SetLogo(int id, IMedia Logo)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET logo=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Logo == null)
                        cmd.Parameters.Add("value", null);
                    else
                        cmd.Parameters.Add("value", Logo.Id);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    if (Logo == null)
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsLogoFk, id, Cache.DefaultSettingsValidationTime)] = null;
                    else
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsLogoFk, id, Cache.DefaultSettingsValidationTime)] = Logo.Id;
                }
            }
        }

        public bool? GetRandomPool(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsRandomPool, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsRandomPool, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT random_pool FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetRandomPool(int id, bool? RandomPool)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET random_pool=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", RandomPool);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsRandomPool, id, Cache.DefaultSettingsValidationTime)] = RandomPool;
                }
            }
        }

        public bool? GetSelfAssessment(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSelfAssessment, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSelfAssessment, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //return from Cache

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT self_assessment FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetSelfAssessment(int id, bool? SelfAssessment)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET self_assessment=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", SelfAssessment);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelfAssessment, id, Cache.DefaultSettingsValidationTime)] = SelfAssessment;
                }
            }
        }

        public bool? GetShowImages(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsShowImages, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsShowImages, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT show_images FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetShowImages(int id, bool? ShowImages)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET show_images=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", ShowImages);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsShowImages, id, Cache.DefaultSettingsValidationTime)] = ShowImages;
                }
            }
        }

        public bool? GetShowStatistics(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsShowStatistics, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsShowStatistics, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT show_statistics FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetShowStatistics(int id, bool? ShowStatistics)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET show_statistics=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", ShowStatistics);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsShowStatistics, id, Cache.DefaultSettingsValidationTime)] = ShowStatistics;
                }
            }
        }

        public bool? GetSkipCorrectAnswers(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsSkipCorrectAnswers, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsSkipCorrectAnswers, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT skip_correct_answers FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetSkipCorrectAnswers(int id, bool? SkipCorrectAnswers)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET skip_correct_answers=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", SkipCorrectAnswers);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSkipCorrectAnswers, id, Cache.DefaultSettingsValidationTime)] = SkipCorrectAnswers;
                }
            }
        }

        public string GetStripChars(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsStripchars, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsStripchars, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as string;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT stripchars FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);
            //        string stripChars = Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd));
            //        if (stripChars != null)
            //            return stripChars;
            //        return null;
            //    }
            //}
        }
        public void SetStripChars(int id, string StripChars)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET stripchars=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", StripChars);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStripchars, id, Cache.DefaultSettingsValidationTime)] = StripChars ?? string.Empty;
                }
            }
        }

        public bool? GetPoolEmptyMessage(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsPoolEmptyMessageShown, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsPoolEmptyMessageShown, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT pool_empty_message_shown FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetPoolEmptyMessage(int id, bool? PoolEmtyMessage)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET pool_empty_message_shown=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", PoolEmtyMessage);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsPoolEmptyMessageShown, id, Cache.DefaultSettingsValidationTime)] = PoolEmtyMessage;
                }
            }
        }

        public bool? GetUseLmStylesheets(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsUseLearningModuleStylesheet, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsUseLearningModuleStylesheet, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT use_lm_stylesheets FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetUseLmStylesheets(int id, bool? UseLmStylesheets)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET use_lm_stylesheets=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", UseLmStylesheets);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsUseLearningModuleStylesheet, id, Cache.DefaultSettingsValidationTime)] = UseLmStylesheets;
                }
            }
        }

        public IList<int> GetSelectedLearningChapters(int id)
        {
            IList<int> cachedList;
            if ((cachedList = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SettingsSelectedLearnChapterList, id)] as IList<int>) == null)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT chapters_id as id FROM \"SelectedLearnChapters\" WHERE settings_id=:id";
                        cmd.Parameters.Add("id", id);

                        NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                        cachedList = new List<int>();
                        while (reader.Read())
                            cachedList.Add(Convert.ToInt32(reader["id"]));

                        //Save to Cache
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelectedLearnChapterList, id, Cache.DefaultSettingsValidationTime)] = cachedList;
                    }
                }
            }

            return cachedList;
        }
        public void SetSelectedLearningChapters(int id, IList<int> SelectedLearningChapters)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"SelectedLearnChapters\" WHERE settings_id=:id; ";
                    cmd.Parameters.Add("id", id);

                    if (SelectedLearningChapters != null && SelectedLearningChapters.Count > 0)
                    {
                        cmd.CommandText += "INSERT INTO \"SelectedLearnChapters\" (settings_id, chapters_id) VALUES ";

                        int i = 0;
                        foreach (int cid in SelectedLearningChapters)
                        {
                            cmd.CommandText += "(:id, :cid" + ((int)(++i)).ToString() + "), ";
                            cmd.Parameters.Add("cid" + i, cid);
                        }

                        cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.LastIndexOf(',')) + ";";
                    }
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsSelectedLearnChapterList, id, Cache.DefaultSettingsValidationTime)] = SelectedLearningChapters;
                }
            }
        }

        public bool? GetAutoBoxSize(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsAutoBoxsize, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsAutoBoxsize, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as bool?;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT auto_boxsize FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        return PostgreSQLConn.ExecuteScalar<bool>(cmd);
            //    }
            //}
        }
        public void SetAutoBoxSize(int id, bool? AutoBoxSize)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Settings\" SET auto_boxsize=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", AutoBoxSize);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    //Save to Cache
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsAutoBoxsize, id, Cache.DefaultSettingsValidationTime)] = AutoBoxSize;
                }
            }
        }

        public ICardStyle GetCardStyle(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsCardStyleFk, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsCardStyleFk, out cacheValue);    //Saves the current Settings from the DB to the Cache
            int? cardStyleId = cacheValue as int?;
            return cardStyleId.HasValue ? new DbCardStyle(cardStyleId.Value, false, Parent) : null;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT cardstyle FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd);

            //        return sid.HasValue ? new DbCardStyle(sid.Value, false, Parent) : null;
            //    }
            //}
        }
        public void SetCardStyle(int id, ICardStyle Style)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                if (Style is DbCardStyle)
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE \"Settings\" SET cardstyle=:csid WHERE id=:id;";
                        cmd.Parameters.Add("id", id);
                        cmd.Parameters.Add("csid", (Style as DbCardStyle).Id);

                        PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    }
                    using (NpgsqlCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "UPDATE \"CardStyles\" SET value=:style WHERE id=:csid;";
                        cmd2.Parameters.Add("csid", (Style as DbCardStyle).Id);
                        cmd2.Parameters.Add("style", Style.Xml);

                        PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);

                        //Save to Cache
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleFk, id, Cache.DefaultSettingsValidationTime)] = (Style as DbCardStyle).Id;
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleValue, id, Cache.DefaultSettingsValidationTime)] = Style.Xml;
                    }
                }
                else
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(:style) RETURNING id; UPDATE \"Settings\" SET cardstyle=currval('\"CardStyles_id_seq\"') WHERE id=:id;";
                        cmd.Parameters.Add("id", id);
                        cmd.Parameters.Add("style", (Style != null) ? Style.Xml : String.Empty);

                        //PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                        int newId = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                        //Save to Cache
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleFk, id, Cache.DefaultSettingsValidationTime)] = newId;
                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsCardStyleValue, id, Cache.DefaultSettingsValidationTime)] = (Style != null) ? Style.Xml : String.Empty;
                    }
                }
            }
        }

        public string GetQuestionStylesheet(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsStyleSheetsQuestionValue, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsStyleSheetsQuestionValue, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as string;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT question_stylesheet FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd);

            //        if (!sid.HasValue)
            //            return null;

            //        using (NpgsqlCommand cmd2 = con.CreateCommand())
            //        {
            //            cmd2.CommandText = "SELECT value FROM \"StyleSheets\" WHERE id=:id";
            //            cmd2.Parameters.Add("id", sid);

            //            return PostgreSQLConn.ExecuteScalar(cmd2).ToString();
            //        }
            //    }
            //}
        }
        public void SetQuestionStylesheet(int id, string QuestionStylesheet)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT question_stylesheet FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (sid.HasValue)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "DELETE FROM \"StyleSheets\" WHERE id=:id";
                            cmd2.Parameters.Add("id", sid);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);
                        }
                    }

                    if (QuestionStylesheet != null && QuestionStylesheet != string.Empty)
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"StyleSheets\" (value) VALUES (:value); " +
                                "UPDATE \"Settings\" SET question_stylesheet=currval('\"StyleSheets_id_seq\"') WHERE id=:id;";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("value", QuestionStylesheet);

                            PostgreSQLConn.ExecuteNonQuery(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStyleSheetsQuestionValue, id, Cache.DefaultSettingsValidationTime)] = QuestionStylesheet ?? string.Empty;
                        }
                    }
                }
            }
        }

        public string GetAnswerStylesheet(int id)
        {
            object cacheValue;
            if (!SettingsCached(id, CacheObject.SettingsStyleSheetsAnswerValue, out cacheValue))      //if settings are not in Cache --> load them
                GetSettingsValue(id, CacheObject.SettingsStyleSheetsAnswerValue, out cacheValue);    //Saves the current Settings from the DB to the Cache
            return cacheValue as string;

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT answer_stylesheet FROM \"Settings\" WHERE id=:id";
            //        cmd.Parameters.Add("id", id);

            //        int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd);

            //        if (!sid.HasValue)
            //            return null;

            //        using (NpgsqlCommand cmd2 = con.CreateCommand())
            //        {
            //            cmd2.CommandText = "SELECT value FROM \"StyleSheets\" WHERE id=:id";
            //            cmd2.Parameters.Add("id", sid);

            //            return PostgreSQLConn.ExecuteScalar(cmd2).ToString();
            //        }
            //    }
            //}
        }
        public void SetAnswerStylesheet(int id, string AnswerStylesheet)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT answer_stylesheet FROM \"Settings\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (sid.HasValue)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "DELETE FROM \"StyleSheets\" WHERE id=:id";
                            cmd2.Parameters.Add("id", sid);

                            PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);
                        }
                    }

                    if (AnswerStylesheet != null && AnswerStylesheet != string.Empty)
                    {
                        using (NpgsqlCommand cmd3 = con.CreateCommand())
                        {
                            cmd3.CommandText = "INSERT INTO \"StyleSheets\" (value) VALUES (:value); " +
                                "UPDATE \"Settings\" SET answer_stylesheet=currval('\"StyleSheets_id_seq\"') WHERE id=:id;";
                            cmd3.Parameters.Add("id", id);
                            cmd3.Parameters.Add("value", AnswerStylesheet);

                            PostgreSQLConn.ExecuteNonQuery(cmd3, Parent.CurrentUser);

                            //Save to Cache
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.SettingsStyleSheetsAnswerValue, id, Cache.DefaultSettingsValidationTime)] = AnswerStylesheet ?? string.Empty;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
