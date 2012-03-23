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
using System.IO;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// MsSqlCeDictionaryConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-12</remarks>
    class MsSqlCeDictionaryConnector : IDbDictionaryConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeDictionaryConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeDictionaryConnector>();
        public static MsSqlCeDictionaryConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            lock (instances)
                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeDictionaryConnector(parentClass));

            return instances[connection];
        }

        private ParentClass parent;
        private MsSqlCeDictionaryConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            lock (instances)
                instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbDictionaryConnector Members

        /// <summary>
        /// Gets the db version.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetDbVersion()
        {
            string version = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
            if (version != null && version.Length > 0)
                return version;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT value FROM \"DatabaseInformation\" WHERE property=@prop";
                cmd.Parameters.Add("@prop", DataBaseInformation.Version.ToString());
                version = Convert.ToString(MSSQLCEConn.ExecuteScalar(cmd));

                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DataBaseVersion, 0, new TimeSpan(1, 0, 0))] = version;
                return version;
            }
        }

        /// <summary>
        /// Gets the current learning module password.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-02-17</remarks>
        public string GetLearningModulePassword(int id)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the content protected.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-13</remarks>
        public bool GetContentProtected(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT content_protected FROM \"LearningModules\" WHERE id=@id;";
                cmd.Parameters.Add("@id", id);
                bool? val = MSSQLCEConn.ExecuteScalar<bool>(cmd);

                return val.HasValue ? val.Value : false;        //[ML-2477] Exception on start page with DRM module and key file on USB stick
            }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetTitle(int id)
        {
            string titleCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleTitle, id)] as string;
            if (titleCache != null)
                return titleCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT title FROM \"LearningModules\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                string title = Convert.ToString(MSSQLCEConn.ExecuteScalar(cmd));
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LearningModuleTitle, id)] = title;
                return title;
            }
        }
        /// <summary>
        /// Sets the title.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetTitle(int id, string title)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET title=@title WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@title", title);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleTitle, id));
            }
        }

        /// <summary>
        /// Gets the author.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetAuthor(int id)
        {
            string authorCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleAuthor, id)] as string;
            if (authorCache != null)
                return authorCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT author FROM \"LearningModules\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                string author = Convert.ToString(MSSQLCEConn.ExecuteScalar(cmd));
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.LearningModuleAuthor, id)] = author;
                return author;
            }
        }
        /// <summary>
        /// Sets the author.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="author">The author.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetAuthor(int id, string author)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET author=@author WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@author", author);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LearningModuleAuthor, id));
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetDescription(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT description FROM \"LearningModules\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                return Convert.ToString(MSSQLCEConn.ExecuteScalar(cmd));
            }
        }
        /// <summary>
        /// Sets the description.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="description">The description.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetDescription(int id, string description)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET description=@description WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@description", description);
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetGuid(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT guid FROM \"LearningModules\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                return Convert.ToString(MSSQLCEConn.ExecuteScalar(cmd));
            }
        }
        /// <summary>
        /// Sets the GUID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="guid">The GUID.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetGuid(int id, string guid)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET guid=@guid WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@guid", guid);
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public ISettings GetDefaultSettings(int id)
        {
            DbSettings settingsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DefaultLearningModuleSettings, id)] as DbSettings;
            if (settingsCache != null)
                return settingsCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT default_settings_id FROM \"LearningModules\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                int? settingsid = MSSQLCEConn.ExecuteScalar<int>(cmd);
                if (!settingsid.HasValue)
                    return null;

                DbSettings settings = new DbSettings(settingsid.Value, false, parent);
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DefaultLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                return settings;
            }
        }
        /// <summary>
        /// Sets the default settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="settingsId">The settings id.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetDefaultSettings(int id, int settingsId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET default_settings_id=@settingsid WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@settingsid", settingsId);
                MSSQLCEConn.ExecuteNonQuery(cmd);

                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DefaultLearningModuleSettings, id));
            }
        }

        /// <summary>
        /// Gets the allowed settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public ISettings GetAllowedSettings(int id)
        {
            DbSettings settingsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.AllowedLearningModuleSettings, id)] as DbSettings;
            if (settingsCache != null)
                return settingsCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                int? settingsid = MSSQLCEConn.ExecuteScalar<int>(cmd);
                if (!settingsid.HasValue)
                    return null;

                DbSettings settings = new DbSettings(settingsid.Value, false, parent);
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AllowedLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                return settings;
            }
        }
        /// <summary>
        /// Sets the allowed settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="settingsId">The settings id.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetAllowedSettings(int id, int settingsId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET allowed_settings_id=@settingsid WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@settingsid", settingsId);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.AllowedLearningModuleSettings, id));
            }
        }

        /// <summary>
        /// Gets the user settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public ISettings GetUserSettings(int id)
        {
            DbSettings settingsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserLearningModuleSettings, id)] as DbSettings;
            if (settingsCache != null)
                return settingsCache;

            int count;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"UserProfilesLearningModulesSettings\" WHERE user_id=@param_user_id and lm_id=@param_lm_id;";
                cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
                cmd.Parameters.Add("@param_lm_id", id);

                count = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }

            int result;
            if (count < 1)      //Create new settings
            {
                using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd.CommandText = "INSERT INTO \"UserProfilesLearningModulesSettings\" (user_id, lm_id, settings_id) VALUES (@param_user_id, @param_lm_id, @param_new_settings);";
                    cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
                    cmd.Parameters.Add("@param_lm_id", id);
                    cmd.Parameters.Add("@param_new_settings", MsSqlCeSettingsConnector.CreateNewSettings(parent));

                    MSSQLCEConn.ExecuteNonQuery(cmd);
                }

                using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd.CommandText = "SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE user_id=@param_user_id and lm_id=@param_lm_id;";
                    cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
                    cmd.Parameters.Add("@param_lm_id", id);

                    result = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
                }

                //Selected Learn Chapters
                using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd.CommandText = "SELECT id FROM \"Chapters\" WHERE lm_id=@param_lm_id";
                    cmd.Parameters.Add("@param_lm_id", id);

                    SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        using (SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                        {
                            cmd2.CommandText = "INSERT INTO \"SelectedLearnChapters\" VALUES (@cid, @result);";
                            cmd2.Parameters.Add("@cid", reader["id"]);
                            cmd2.Parameters.Add("@result", result);

                            MSSQLCEConn.ExecuteNonQuery(cmd2);
                        }
                    }
                    reader.Close();
                }

                DbSettings settings = new DbSettings(result, false, parent);
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.UserLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                return settings;
            }
            else
            {
                using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd.CommandText = "SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE user_id=@param_user_id and lm_id=@param_lm_id;";
                    cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
                    cmd.Parameters.Add("@param_lm_id", id);

                    result = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
                }

                DbSettings settings = new DbSettings(result, false, parent);
                parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.UserLearningModuleSettings, id, Cache.DefaultSettingsValidationTime)] = settings;
                return settings;
            }
        }
        /// <summary>
        /// Sets the user settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="settingsId">The settings id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetUserSettings(int id, int settingsId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"UserProfilesLearningModulesSettings\" SET settings_id=@settingsid WHERE user_id=@uid and lm_id=@lm_id;";
                cmd.Parameters.Add("@uid", parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", id);
                cmd.Parameters.Add("@settingsid", settingsId);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.AllowedLearningModuleSettings, id));
            }
        }

        /// <summary>
        /// Creates the settings.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public ISettings CreateSettings()
        {
            int sid = MsSqlCeSettingsConnector.CreateNewSettings(parent);
            return new DbSettings(sid, false, parent);
        }

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        /// <remarks>Documented by Dev04, 2009-04-10</remarks>
        /// <remarks>Documented by Dev08, 2009-04-20</remarks>
        public double GetScore(int id)
        {
            //double? score = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Score, id)] as double?;
            //if (score != null && score.HasValue)
            //    return score.Value;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                //cmd.CommandText = "SELECT SUM(box) FROM \"UserCardState\" WHERE user_id=@param_user_id and cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=@param_lm_id)";

                double[] factor = new double[11];
                factor[0] = 0;
                for (int i = 1; i < factor.Length; i++)
                    factor[i] = factor[i - 1] + 1.0 / i;

                cmd.CommandText = "SELECT TOP(1) (";
                for (int i = 1; i < factor.Length; i++)
                {
                    cmd.CommandText = cmd.CommandText + "box" + i + "_content * " + factor[i - 1].ToString().Replace(",", ".");
                    if (i != factor.Length - 1)
                        cmd.CommandText += " + ";
                }
                cmd.CommandText += ") AS sum, ((pool_content + ";

                for (int i = 1; i < factor.Length; i++)
                {
                    cmd.CommandText += "box" + i + "_content ";
                    if (i != factor.Length - 1)
                        cmd.CommandText += "+";
                    else
                        cmd.CommandText += ")*" + factor[factor.Length - 2].ToString().Replace(",", ".");
                }

                cmd.CommandText += ") AS total FROM \"LearningSessions\" WHERE (user_id=@param_user_id AND lm_id=@param_lm_id) ORDER BY id DESC";

                cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
                cmd.Parameters.Add("@param_lm_id", id);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                reader.Read();

                double sum, total;
                double? result;
                try
                {
                    sum = System.Convert.ToDouble(reader["sum"]);
                    total = System.Convert.ToDouble(reader["total"]);

                    result = (double)(sum / total * 100.00);
                }
                catch
                {
                    result = null;
                }

                //parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.Score, id, new TimeSpan(0, 0, 1))] = result;
                return result.HasValue ? result.Value : 0;
            }
        }
        /// <summary>
        /// Gets the highscore.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public double GetHighscore(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT highscore FROM \"UserProfilesLearningModulesSettings\" WHERE user_id=@uid AND lm_id=@lm_id;";
                cmd.Parameters.Add("@uid", parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", id);

                double? value = MSSQLCEConn.ExecuteScalar<double>(cmd);
                return value.HasValue ? value.Value : 0;
            }
        }
        /// <summary>
        /// Sets the highscore.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="highscore">The highscore.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetHighscore(int id, double highscore)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"UserProfilesLearningModulesSettings\" SET highscore=@value WHERE user_id=@uid and lm_id=@lm_id;";
                cmd.Parameters.Add("@uid", parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", id);
                cmd.Parameters.Add("@value", highscore);

                MSSQLCEConn.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Gets the size of the dictionary.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="defaultCardSizeValue">The default card size value.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public long GetDictionarySize(int id, int defaultCardSizeValue)
        {
            FileInfo fi = new FileInfo(parent.CurrentUser.ConnectionString.ConnectionString);
            return fi.Length;
        }

        /// <summary>
        /// Gets the dictionary media objects count.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public int GetDictionaryMediaObjectsCount(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                int? mediaSizeObjectsCount;

                cmd.CommandText = "SELECT COUNT(*) AS LearningModuleMediaObjectsCount FROM \"MediaContent\"";
                mediaSizeObjectsCount = MSSQLCEConn.ExecuteScalar<int>(cmd);
                if (!mediaSizeObjectsCount.HasValue)
                    return 0;

                return mediaSizeObjectsCount.Value;
            }
        }

        /// <summary>
        /// Gets the category id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public Category GetCategoryId(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT global_id FROM \"LearningModules\", \"Categories\" WHERE \"LearningModules\".categories_id = \"Categories\".id AND \"LearningModules\".id = @id";
                cmd.Parameters.Add("@id", id);
                int? globalId = MSSQLCEConn.ExecuteScalar<int>(cmd);

                if (!globalId.HasValue)
                    return new Category(0);

                return new Category(globalId.Value);
            }
        }
        /// <summary>
        /// Sets the category.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="catId">The cat id.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetCategory(int id, int catId)
        {
            int categoryId;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id FROM \"Categories\" WHERE global_id = @globalCatId";
                cmd.Parameters.Add("@globalCatId", catId);

                categoryId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"LearningModules\" SET categories_id = @categoryId WHERE \"LearningModules\".id = @lmId";
                cmd.Parameters.Add("@lmId", id);
                cmd.Parameters.Add("@categoryId", categoryId);

                MSSQLCEConn.ExecuteNonQuery(cmd);
                return;
            }
        }
        /// <summary>
        /// Sets the category.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cat">The cat.</param>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public void SetCategory(int id, Category cat)
        {
            SetCategory(id, cat.Id);
        }

        /// <summary>
        /// Checks the user session.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public bool CheckUserSession()
        {
            return true;
        }

        public void PreloadCardCache(int id)
        {
            if (parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardCacheInitialized, id)] != null)
                return;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            using (SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            using (SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT C.id, C.chapters_id, CS.box, CS.active, CS.[timestamp] FROM Cards C INNER JOIN UserCardState CS ON C.id = CS.cards_id WHERE C.lm_id = @lm_id AND  CS.user_id = @user_id ORDER BY C.id";
                cmd.Parameters.Add("@user_id", parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", id);
                cmd2.CommandText = "SELECT id, title FROM Chapters WHERE id IN (SELECT chapters_id FROM Cards WHERE lm_id = @lm_id)";
                cmd2.Parameters.Add("@lm_id", id);
                cmd3.CommandText = "SELECT C.id, T.text, T.side FROM Cards C INNER JOIN TextContent T ON C.id = T.cards_id WHERE C.lm_id = @lm_id AND T.type = 'Word' ORDER BY C.id, T.side, T.position;";
                cmd3.Parameters.Add("@lm_id", id);

                List<object[]> cards = new List<object[]>();
                using (SqlCeDataReader cardReader = MSSQLCEConn.ExecuteReader(cmd))
                {
                    while (cardReader.Read())
                    {
                        object[] row = new object[5];
                        cardReader.GetValues(row);
                        cards.Add(row);
                    }
                }
                List<object[]> chapters = new List<object[]>();
                using (SqlCeDataReader chapterReader = MSSQLCEConn.ExecuteReader(cmd2))
                {
                    while (chapterReader.Read())
                    {
                        object[] row = new object[2];
                        chapterReader.GetValues(row);
                        chapters.Add(row);
                    }
                }
                List<object[]> allWords = new List<object[]>();
                using (SqlCeDataReader textReader = MSSQLCEConn.ExecuteReader(cmd3))
                {
                    while (textReader.Read())
                    {
                        object[] row = new object[3];
                        textReader.GetValues(row);
                        allWords.Add(row);
                    }
                }

                int pos = 0;
                foreach (object[] card in cards)
                {
                    int cardId = Convert.ToInt32(card[0]);
                    int chapterId = Convert.ToInt32(card[1]);
                    object[] chapter = chapters.Find(c => Convert.ToInt32(c[0]) == chapterId);
                    if (chapter != null)
                        chapters.Remove(chapter);
                    List<object[]> words = new List<object[]>();
                    if (allWords.Count > pos && Convert.ToInt32(allWords[pos][0]) == cardId)
                    {
                        while (allWords.Count > pos && Convert.ToInt32(allWords[pos][0]) == cardId)
                        {
                            words.Add(allWords[pos]);
                            pos++;
                        }
                    }
                    IList<IWord> question = new List<IWord>();
                    IList<IWord> answer = new List<IWord>();
                    foreach (object[] word in words)
                    {
                        // Pull the question word or answer word from this row
                        String side = word[2].ToString();
                        if (side == Side.Question.ToString())
                        {
                            question.Add(new DbWord(cardId, word[1].ToString(), WordType.Word, true, parent));
                        }
                        else
                        {
                            answer.Add(new DbWord(cardId, word[1].ToString(), WordType.Word, true, parent));
                        }
                    }
                    parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.QuestionWords, cardId)] = question;
                    parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AnswerWords, cardId)] = answer;
                    parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardState, cardId)] = new CardState(Convert.ToInt32(card[2]),
                        Convert.ToBoolean(card[3]), (card[4] is DBNull) ? new DateTime(1901, 1, 1) : Convert.ToDateTime(card[4])); // box, active, timestamp
                    if (chapter != null)
                        parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterTitle, Convert.ToInt32(chapter[0]))] = chapter[1].ToString();
                }
            }
            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardCacheInitialized, id)] = true;
        }

        public void ClearUnusedMedia(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                List<int> allMedia = new List<int>();
                cmd.CommandText = "SELECT id FROM MediaContent";
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                while (reader.Read())
                    allMedia.Add(Convert.ToInt32(reader["id"]));

                cmd.CommandText = "SELECT media_id FROM Cards_MediaContent UNION SELECT media_id FROM MediaContent_CardStyles UNION SELECT media_id FROM CommentarySounds UNION SELECT logo FROM Settings";
                reader = MSSQLCEConn.ExecuteReader(cmd);
                while (reader.Read())
                {
                    object val = reader[0];
                    if (val == null || val is DBNull)
                        continue;

                    int value = Convert.ToInt32(val);
                    if (allMedia.Contains(value))
                        allMedia.Remove(value);
                }

                foreach (int mid in allMedia)
                {
                    cmd.CommandText = "DELETE FROM MediaProperties WHERE media_id=@id; DELETE FROM MediaContent_Tags WHERE media_id=@id; DELETE FROM MediaContent WHERE id=@id";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("id", mid);
                    MSSQLCEConn.ExecuteNonQuery(cmd);
                }
            }
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
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT guid FROM \"Extensions\" WHERE lm_id=@lm_id";
                cmd.Parameters.Add("@lm_id", id);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

                while (reader.Read())
                    guids.Add(new Guid(reader["guid"] as string));
            }

            return guids;
        }

        #endregion
    }
}
