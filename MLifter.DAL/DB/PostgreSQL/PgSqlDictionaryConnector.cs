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
using MLifter.DAL.Tools;
using MLifter.Generics;
using Npgsql;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlDictionaryConnector : MLifter.DAL.Interfaces.DB.IDbDictionaryConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlDictionaryConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlDictionaryConnector>();
        public static PgSqlDictionaryConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlDictionaryConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlDictionaryConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbDictionaryConnector Members

        public string GetDbVersion()
        {
            string version = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
            if (version != null && version.Length > 0)
                return version;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT value FROM \"DatabaseInformation\" WHERE property=:prop";
                    cmd.Parameters.Add("prop", DataBaseInformation.Version.ToString());
                    version = Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DataBaseVersion, 0, new TimeSpan(1, 0, 0))] = version;
                    return version;
                }
            }
        }

        /// <summary>
        /// Gets the content protected.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-13</remarks>
        public bool GetContentProtected(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT content_protected FROM \"LearningModules\" WHERE id=:id;";
                    cmd.Parameters.Add("id", id);
                    bool? output = PostgreSQLConn.ExecuteScalar<bool>(cmd, Parent.CurrentUser);

                    return output.HasValue ? output.Value : false;
                }
            }
        }

        public string GetTitle(int id)
        {
            string titleCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleTitle, id)] as string;
            if (titleCache != null)
                return titleCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT title FROM \"LearningModules\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    string title = Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LearningModuleTitle, id)] = title;
                    return title;
                }
            }
        }
        public void SetTitle(int id, string title)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET title=:title WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("title", title);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleTitle, id));
                }
            }
        }


        public string GetAuthor(int id)
        {
            string authorCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleAuthor, id)] as string;
            if (authorCache != null)
                return authorCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT author FROM \"LearningModules\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    string author = Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LearningModuleAuthor, id)] = author;
                    return author;
                }
            }
        }
        public void SetAuthor(int id, string author)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET author=:author WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("author", author);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleAuthor, id));
                }
            }
        }

        public string GetDescription(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT description FROM \"LearningModules\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    return Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }
        public void SetDescription(int id, string description)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET description=:description WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("description", description);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public string GetGuid(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT guid FROM \"LearningModules\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    return Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }
        public void SetGuid(int id, string guid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET guid=:guid WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("guid", guid);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public ISettings GetDefaultSettings(int id)
        {
            DbSettings settingsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DefaultLearningModuleSettings, id)] as DbSettings;
            if (settingsCache != null)
                return settingsCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT default_settings_id FROM \"LearningModules\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    int? settingsid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    if (!settingsid.HasValue)
                        return null;
                    DbSettings settings = new DbSettings(settingsid.Value, false, Parent);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DefaultLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                    return settings;

                }
            }
        }
        public void SetDefaultSettings(int id, int settingsId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET default_settings_id=:settingsid WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("settingsid", settingsId);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DefaultLearningModuleSettings, id));
                }
            }
        }

        public ISettings GetAllowedSettings(int id)
        {
            DbSettings settingsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.AllowedLearningModuleSettings, id)] as DbSettings;
            if (settingsCache != null)
                return settingsCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    int? settingsid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    if (!settingsid.HasValue)
                        return null;
                    DbSettings settings = new DbSettings(settingsid.Value, false, Parent);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AllowedLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                    return settings;

                }
            }
        }
        public void SetAllowedSettings(int id, int settingsId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET allowed_settings_id=:settingsid WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("settingsid", settingsId);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.AllowedLearningModuleSettings, id));
                }
            }
        }

        public ISettings GetUserSettings(int id)
        {
            DbSettings settingsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserLearningModuleSettings, id)] as DbSettings;
            if (settingsCache != null)
                return settingsCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"GetUserSettings\"(:uid, :lm_id);";
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", id);

                    int? settingsid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    if (!settingsid.HasValue)
                        return null;
                    DbSettings settings = new DbSettings(settingsid.Value, false, Parent);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.UserLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                    return settings;

                }
            }
        }
        public void SetUserSettings(int id, int settingsId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"UserProfilesLearningModulesSettings\" SET settings_id=:settingsid WHERE user_id=:uid and lm_id=:lm_id;";
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", id);
                    cmd.Parameters.Add("settingsid", settingsId);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.AllowedLearningModuleSettings, id));
                }
            }
        }

        public ISettings CreateSettings()
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"CreateNewSetting\"()";

                    //PostgreSQLConn.ExecuteNonQuery(cmd);
                    int sid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString());
                    return new DbSettings(sid, false, Parent);
                }
            }
        }

        public double GetScore(int id)
        {
            double? score = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Score, id)] as double?;
            if (score != null && score.HasValue)
                return (double)score.Value;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"GetScore\"(:uid, :lm_id);";
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", id);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();

                    double sum, total;
                    double? result;
                    try
                    {
                        sum = System.Convert.ToDouble(reader["sum"]);
                        total = System.Convert.ToDouble(reader["total"]);

                        result = (double)(sum / total * 100);
                    }
                    catch
                    {
                        result = null;
                    }

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.Score, id, new TimeSpan(0, 0, 1))] = result;
                    return result.HasValue ? result.Value : 0;
                }
            }
        }

        public double GetHighscore(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT highscore FROM \"UserProfilesLearningModulesSettings\" WHERE user_id=:uid and lm_id=:lm_id;";
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", id);

                    return PostgreSQLConn.ExecuteScalar<double>(cmd, Parent.CurrentUser).Value;
                }
            }
        }
        public void SetHighscore(int id, double Highscore)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"UserProfilesLearningModulesSettings\" SET highscore=:value WHERE user_id=:uid and lm_id=:lm_id;";
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", id);
                    cmd.Parameters.Add("value", Highscore);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        /// <summary>
        /// Gets the size of the dictionary.
        /// </summary>
        /// <param name="id">The LearningModule id.</param>
        /// <param name="defaultCardSizeValue">The default size of a card without media (e.g. 1024 bytes).</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        public long GetDictionarySize(int id, int defaultCardSizeValue)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    long? mediaSize;
                    long? cardsCount;

                    //1. Get the FileSize sum of all MediaFiles in this LearningModule
                    cmd.CommandText = "SELECT SUM(CAST(\"MediaProperties\".value AS INT)) AS LearningModuleSize" +
                        " FROM \"MediaContent\", \"Cards_MediaContent\", \"MediaProperties\"" +
                        " WHERE \"Cards_MediaContent\".media_id = \"MediaContent\".id AND \"Cards_MediaContent\".cards_id IN" +
                        " (SELECT id FROM \"Cards\", \"LearningModules_Cards\" WHERE \"LearningModules_Cards\".cards_id = \"Cards\".id AND" +
                        " \"LearningModules_Cards\".lm_id = :id) AND \"MediaProperties\".media_id = \"MediaContent\".id AND" +
                        "\"MediaProperties\".property = 'MediaSize'";
                    cmd.Parameters.Add("id", id);
                    mediaSize = PostgreSQLConn.ExecuteScalar<long>(cmd, Parent.CurrentUser);

                    //2. Get the number of cards in this LearningModule (to calculate the approximately size of all Cards without Media)
                    using (NpgsqlCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT COUNT(*) FROM \"Cards\", \"LearningModules_Cards\" WHERE \"LearningModules_Cards\".cards_id = \"Cards\".id AND \"LearningModules_Cards\".lm_id = :id";
                        cmd2.Parameters.Add("id", id);
                        cardsCount = PostgreSQLConn.ExecuteScalar<long>(cmd2, Parent.CurrentUser);
                    }

                    long size = 0;
                    if (mediaSize.HasValue)
                        size += mediaSize.Value;
                    if (cardsCount.HasValue)
                        size += cardsCount.Value * defaultCardSizeValue;

                    return size;
                }
            }
        }

        /// <summary>
        /// Gets the number of all Media Objects/Files in this LearningModules.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        public int GetDictionaryMediaObjectsCount(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    int? mediaSizeObjectsCount;

                    cmd.CommandText = "SELECT COUNT(*) AS LearningModuleMediaObjectsCount" +
                        " FROM \"MediaContent\", \"Cards_MediaContent\", \"MediaProperties\"" +
                        " WHERE \"Cards_MediaContent\".media_id = \"MediaContent\".id AND \"Cards_MediaContent\".cards_id IN" +
                        " (SELECT id FROM \"Cards\", \"LearningModules_Cards\" WHERE \"LearningModules_Cards\".cards_id = \"Cards\".id AND" +
                        " \"LearningModules_Cards\".lm_id = :id) AND \"MediaProperties\".media_id = \"MediaContent\".id AND" +
                        "\"MediaProperties\".property = 'MediaSize'";
                    cmd.Parameters.Add("id", id);
                    mediaSizeObjectsCount = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    if (!mediaSizeObjectsCount.HasValue)
                        return 0;

                    return mediaSizeObjectsCount.Value;
                }
            }
        }

        /// <summary>
        /// Gets the category Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>the global category ID (0-5)</returns>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        public Category GetCategoryId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT global_id FROM \"LearningModules\", \"Categories\" WHERE \"LearningModules\".categories_id = \"Categories\".id AND \"LearningModules\".id = :id";
                    cmd.Parameters.Add("id", id);
                    int? globalId = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (!globalId.HasValue)
                        return new Category(0);

                    return new Category(globalId.Value);
                }
            }
        }

        /// <summary>
        /// Sets the category.
        /// </summary>
        /// <param name="id">The LM id.</param>
        /// <param name="catId">The category id (global id).</param>
        /// <remarks>Documented by Dev08, 2008-10-03</remarks>
        public void SetCategory(int id, int catId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningModules\" SET categories_id = (SELECT id FROM \"Categories\" WHERE global_id = :globalCatId) WHERE \"LearningModules\".id = :lmId";
                    cmd.Parameters.Add("lmId", id);
                    cmd.Parameters.Add("globalCatId", catId);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the category.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cat">The category.</param>
        /// <remarks>Documented by Dev08, 2008-10-03</remarks>
        public void SetCategory(int id, Category cat)
        {
            SetCategory(id, cat.Id);
        }

        public bool CheckUserSession()
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    return PostgreSQLConn.CheckSession(cmd, Parent.CurrentUser);
                }
            }
        }

        public void PreloadCardCache(int id)
        {
            if (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardCacheInitialized, id)] != null)
                return;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    // Pull all the data from the database
                    cmd.CommandText = "SELECT cards_id, chapters_id, chapters_title, box, active, \"timestamp\", \"text\", side FROM \"vwGetCardsForCache\" WHERE lm_id = :lm_id AND user_id = :user_id";
                    cmd.Parameters.Add("lm_id", id);
                    cmd.Parameters.Add("user_id", Parent.CurrentUser.Id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    // Status and temporary data holding variables
                    int cardId = -1;
                    int chapterId = -1;
                    IList<IWord> question = new List<IWord>();
                    IList<IWord> answer = new List<IWord>();
                    while (reader.Read())
                    {
                        // Order of the values is the order in the SQL command text
                        object[] values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        int thisId = Convert.ToInt32(values[0]);

                        if (thisId != cardId)
                        {
                            if (cardId != -1)
                            {
                                // Now store the question and answer words for this card before going to the next
                                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.QuestionWords, cardId)] = question;
                                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AnswerWords, cardId)] = answer;
                                question = new List<IWord>();
                                answer = new List<IWord>();
                            }

                            cardId = thisId;
                            chapterId = Convert.ToInt32(values[1]);

                            // Update Id and store initial data found in every row for this card
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardState, id)] = new CardState(Convert.ToInt32(values[3]),
                                Convert.ToBoolean(values[4]), values[5] is DBNull || values[5] == null ? new DateTime() : Convert.ToDateTime(values[5])); // box, active, timestamp
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterTitle, chapterId)] = values[2].ToString();
                        }

                        // Pull the question word or answer word from this row
                        String side = values[7].ToString();
                        if (side == Side.Question.ToString())
                        {
                            question.Add(new DbWord(cardId, values[6].ToString(), WordType.Word, true, Parent));
                        }
                        else if (side == Side.Answer.ToString())
                        {
                            answer.Add(new DbWord(cardId, values[6].ToString(), WordType.Word, true, Parent));
                        }
                    }
                }
            }
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardCacheInitialized, id)] = true;
        }

        public void ClearUnusedMedia(int id)
        {
            //ToDo: Implement
        }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        public IList<Guid> GetExtensions(int id)
        {
            IList<Guid> guids = new List<Guid>();
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT guid FROM \"Extensions\" WHERE lm_id=@lm_id";
                    cmd.Parameters.Add("@lm_id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    while (reader.Read())
                        guids.Add(new Guid(reader["guid"] as string));
                }
            }

            return guids;
        }

        #endregion
    }
}
