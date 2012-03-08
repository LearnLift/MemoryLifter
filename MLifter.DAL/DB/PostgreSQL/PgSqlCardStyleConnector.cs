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
    class PgSqlCardStyleConnector : IDbCardStyleConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlCardStyleConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlCardStyleConnector>();
        public static PgSqlCardStyleConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlCardStyleConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlCardStyleConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbCardStyleConnector Members

        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public void CheckId(int Id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"CardStyles\" WHERE id=:id";
                    cmd.Parameters.Add("id", Id);

                    int? count = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (!count.HasValue || count.Value < 1)
                        throw new IdAccessException(Id);
                }
            }
        }

        /// <summary>
        /// Gets the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <returns>The CSS for the card style.</returns>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public string GetCardStyle(int Id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT value FROM \"CardStyles\" WHERE id=:id";
                    cmd.Parameters.Add("id", Id);

                    return PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString();
                }
            }
        }

        /// <summary>
        /// Sets the card style.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="CardStyle">The card style.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public void SetCardStyle(int id, string CardStyle)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd2 = con.CreateCommand())
                {
                    cmd2.CommandText = "UPDATE \"CardStyles\" SET value=:value WHERE id=:id";
                    cmd2.Parameters.Add("id", id);
                    cmd2.Parameters.Add("value", CardStyle);
                    PostgreSQLConn.ExecuteNonQuery(cmd2, Parent.CurrentUser);
                }
            }
        }


        /// <summary>
        /// Creates the new card style.
        /// </summary>
        /// <returns>The style id.</returns>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public int CreateNewCardStyle()
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"CardStyles\" (id) VALUES (DEFAULT) RETURNING id";
                    int? newId = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    if (newId.HasValue)
                        return newId.Value;
                    else
                        return -1;
                }
            }
        }

        /// <summary>
        /// Adds the media for the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <param name="mediaId">The media id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public int AddMediaForCardStyle(int Id, int mediaId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"MediaContent_CardStyles\" (media_id, cardstyles_id) VALUES (:media_id, :cardstyles_id)";
                    cmd.Parameters.Add("media_id", mediaId);
                    cmd.Parameters.Add("cardstyles_id", Id);
                    int? newId = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);
                    if (newId.HasValue)
                        return newId.Value;
                    else
                        return -1;
                }
            }
        }

        public List<int> GetMediaForCardStyle(int Id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT media_id FROM \"MediaContent_CardStyles\" WHERE cardstyles_id = :cardstyles_id";
                    cmd.Parameters.Add("cardstyles_id", Id);
                    NpgsqlDataReader reader;
                    reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    List<int> cs = new List<int>();
                    while (reader.Read())
                        cs.Add(Convert.ToInt32(reader["media_id"]));

                    return cs;
                }
            }
        }

        /// <summary>
        /// Deletes the media id for the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <param name="mediaId">The media id.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public void DeleteMediaForCardStyle(int Id, int mediaId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"MediaContent_CardStyles\" WHERE media_id = :media_id AND cardstyles_id = :cardstyles_id";
                    cmd.Parameters.Add("media_id", mediaId);
                    cmd.Parameters.Add("cardstyles_id", Id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        /// <summary>
        /// Clears the media for card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        public void ClearMediaForCardStyle(int Id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"MediaContent_CardStyles\" WHERE cardstyles_id = :cardstyles_id";
                    cmd.Parameters.Add("cardstyles_id", Id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        #endregion
    }
}
