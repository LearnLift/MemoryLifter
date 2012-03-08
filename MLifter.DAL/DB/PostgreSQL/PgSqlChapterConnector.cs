using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlChapterConnector : Interfaces.DB.IDbChapterConnector
    {        
        private static Dictionary<ConnectionStringStruct, PgSqlChapterConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlChapterConnector>();
        public static PgSqlChapterConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlChapterConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlChapterConnector(ParentClass parentClass)
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

        public string GetTitle(int id)
        {
            string titleCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterTitle, id)] as string;
            if (titleCache != null)
                return titleCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, title FROM \"Chapters\" WHERE lm_id=(SELECT lm_id FROM \"Chapters\" WHERE id=:id)";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    string title = string.Empty;
                    while (reader.Read())
                    {
                        int chapterId = Convert.ToInt32(reader["id"]);
                        string chapterTitle = Convert.ToString(reader["title"]);
                        if (id == chapterId)
                            title = chapterTitle;

                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterTitle, chapterId, new TimeSpan(0, 10, 0))] = chapterTitle;
                    }

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
                    cmd.CommandText = "UPDATE \"Chapters\" SET title=:title WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("title", title);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterTitle, id));
                }
            }
        }

        public string GetDescription(int id)
        {
            string descriptionCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterDescription, id)] as string;
            if (descriptionCache != null)
                return descriptionCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, description FROM \"Chapters\" WHERE lm_id=(SELECT lm_id FROM \"Chapters\" WHERE id=:id)";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    string chapterDescription = string.Empty;
                    while (reader.Read())
                    {
                        int chip = Convert.ToInt32(reader["id"]);
                        string chapterDesc = reader["description"].ToString();
                        if (id == chip)
                            chapterDescription = chapterDesc;

                        Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChapterDescription, chip)] = chapterDesc;
                    }

                    return chapterDescription;
                }
            }
        }
        public void SetDescription(int id, string description)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Chapters\" SET description=:description WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("description", description);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChapterDescription, id));
                }
            }
        }

        public int GetSize(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"Chapters_Cards\" WHERE chapters_id=:id";
                    cmd.Parameters.Add("id", id);
                    return Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }

        public int GetActiveSize(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT (SELECT count(*) FROM \"Chapters_Cards\" WHERE chapters_id=:chapterid) - (SELECT count(*) FROM \"UserCardState\" WHERE cards_id IN (SELECT cards_id FROM \"Chapters_Cards\" WHERE chapters_id=:chapterid) AND active=false AND user_id=:userid);";
                    cmd.Parameters.Add("chapterid", id);
                    cmd.Parameters.Add("userid", Parent.CurrentUser.Id);
                    return Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }

        public int GetLmId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT lm_id FROM \"Chapters\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    return Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }

        public void CheckChapterId(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"Chapters\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) != 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public MLifter.DAL.Interfaces.ISettings GetSettings(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT settings_id FROM \"Chapters\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (sid.HasValue)
                        return new DbSettings(sid.Value, false, Parent);
                    else
                        return null;
                }
            }
        }

        public void SetSettings(int id, MLifter.DAL.Interfaces.ISettings Settings)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Chapters\" SET settings_id=:value WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("value", (Settings as DbSettings).Id);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public ICardStyle CreateId()
        {

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"CardStyles\" (id, value) VALUES (DEFAULT, :value) RETURNING id";
                    cmd.Parameters.Add("value", "<cardStyle></cardStyle>");
                    int? id = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    return new DbCardStyle(id.GetValueOrDefault(), false, Parent);

                }
            }
        }

        #endregion
    }
}
