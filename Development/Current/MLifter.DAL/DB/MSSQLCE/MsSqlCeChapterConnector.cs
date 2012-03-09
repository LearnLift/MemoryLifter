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
    /// SQL CE implementation of IDbChapterConnector.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-01-15</remarks>
    class MsSqlCeChapterConnector : IDbChapterConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeChapterConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeChapterConnector>();
        public static MsSqlCeChapterConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new MsSqlCeChapterConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private MsSqlCeChapterConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbChapterConnector Members

        /// <summary>
        /// Gets the title of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public string GetTitle(int id)
        {
            string titleCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterTitle, id)] as string;
            if (titleCache != null)
                return titleCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id, title FROM \"Chapters\" WHERE lm_id IN (SELECT lm_id FROM \"Chapters\" WHERE id=@id);";
                cmd.Parameters.Add("@id", id);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

                string title = string.Empty;
                while (reader.Read())
                {
                    int chapterId = Convert.ToInt32(reader["id"]);
                    string chapterTitle = Convert.ToString(reader["title"]);
                    if (id == chapterId)
                        title = chapterTitle;

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterTitle, chapterId, new TimeSpan(0, 10, 0))] = chapterTitle;
                }
                reader.Close();

                return title;
            }
        }
        /// <summary>
        /// Sets the title of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public void SetTitle(int id, string title)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"Chapters\" SET title=@title WHERE id=@id;";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@title", title);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterTitle, id));
            }
        }

        /// <summary>
        /// Gets the description of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public string GetDescription(int id)
        {
            string descriptionCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterDescription, id)] as string;
            if (descriptionCache != null)
                return descriptionCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id, description FROM \"Chapters\" WHERE lm_id IN (SELECT lm_id FROM \"Chapters\" WHERE id=@id)";
                cmd.Parameters.Add("@id", id);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

                string chapterDescription = string.Empty;
                while (reader.Read())
                {
                    int chip = Convert.ToInt32(reader["id"]);
                    string chapterDesc = reader["description"].ToString();
                    if (id == chip)
                        chapterDescription = chapterDesc;

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterDescription, chip)] = chapterDesc;
                }
                reader.Close();

                return chapterDescription;
            }
        }
        /// <summary>
        /// Sets the description of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="description">The description.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public void SetDescription(int id, string description)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"Chapters\" SET description=@description WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@description", description);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterDescription, id));
            }
        }

        /// <summary>
        /// Gets the size of the chapter (amount of cards).
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public int GetSize(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"Chapters_Cards\" WHERE chapters_id=@id";
                cmd.Parameters.Add("@id", id);
                return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }
        }
        /// <summary>
        /// Gets the active size of the chapter (amount of activated cards).
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-11</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public int GetActiveSize(int id)
        {

            int all, notActive;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"Chapters_Cards\" WHERE chapters_id=@chapterid;";
                cmd.Parameters.Add("@chapterid", id);
                all = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"UserCardState\" WHERE cards_id IN " +
                    "(SELECT cards_id FROM \"Chapters_Cards\" WHERE chapters_id=@chapterid) AND active=0 AND user_id=@userid;";
                cmd.Parameters.Add("@chapterid", id);
                cmd.Parameters.Add("@userid", Parent.CurrentUser.Id);
                notActive = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }

            return all - notActive;

        }

        /// <summary>
        /// Gets the associated lm id of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public int GetLmId(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT lm_id FROM \"Chapters\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// Checks the chapter id of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public void CheckChapterId(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"Chapters\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) != 1)
                    throw new IdAccessException(id);
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public ISettings GetSettings(int id)
        {
            int? chapterSettingsId = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterSetting, id)] as int?;
            if (chapterSettingsId.HasValue)
                return new DbSettings(chapterSettingsId.Value, false, Parent);

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT settings_id FROM \"Chapters\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);

                int? sid = MSSQLCEConn.ExecuteScalar<int>(cmd);

                if (sid.HasValue)
                {
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterSetting, id)] = sid;
                    return new DbSettings(sid.Value, false, Parent);
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Sets the settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Settings">The settings.</param>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public void SetSettings(int id, ISettings Settings)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"Chapters\" SET settings_id=@value WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@value", (Settings as DbSettings).Id);

                MSSQLCEConn.ExecuteNonQuery(cmd);
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterSetting, id)] = (Settings as DbSettings).Id;
            }
        }

        /// <summary>
        /// Creates the id.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-15</remarks>
        public ICardStyle CreateId()
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO \"CardStyles\" (value) VALUES (@value); SELECT @@IDENTITY;";
                cmd.Parameters.Add("@value", "<cardStyle></cardStyle>");
                int? id = MSSQLCEConn.ExecuteScalar<int>(cmd);
                return new DbCardStyle(id.GetValueOrDefault(), false, Parent);
            }
        }

        #endregion
    }
}
