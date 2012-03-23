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
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// MsSqlCeCardStyleConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-12</remarks>
    class MsSqlCeCardStyleConnector : IDbCardStyleConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeCardStyleConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeCardStyleConnector>();
        public static MsSqlCeCardStyleConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeCardStyleConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass parent;
        private MsSqlCeCardStyleConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbCardStyleConnector Members

		/// <summary>
		/// Checks the id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <remarks>
		/// Documented by FabThe, 9.1.2009
		/// </remarks>
        public void CheckId(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"CardStyles\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);

                int? count = MSSQLCEConn.ExecuteScalar<int>(cmd);

                if (!count.HasValue || count.Value < 1)
                    throw new IdAccessException(id);
            }
        }

        /// <summary>
        /// Gets the card style.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public string GetCardStyle(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT value FROM \"CardStyles\" WHERE id=@id";
                cmd.Parameters.Add("@id", id);

                return MSSQLCEConn.ExecuteScalar(cmd).ToString();
            }
        }

        /// <summary>
        /// Sets the card style.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cardStyle">The card style.</param>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void SetCardStyle(int id, string cardStyle)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                using (SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd2.CommandText = "UPDATE \"CardStyles\" SET value=@value WHERE id=@id";
                    cmd2.Parameters.Add("@id", id);
                    cmd2.Parameters.Add("@value", cardStyle);
                    MSSQLCEConn.ExecuteNonQuery(cmd2);
                }
            }
        }

        /// <summary>
        /// Creates the new card style.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-09</remarks>
        public int CreateNewCardStyle()
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO \"CardStyles\" (value) VALUES (''); SELECT @@IDENTITY";
                int? newId = MSSQLCEConn.ExecuteScalar<int>(cmd);
                if (newId.HasValue)
                    return newId.Value;
                else
                    return -1;
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
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO \"MediaContent_CardStyles\" (media_id, cardstyles_id) VALUES (@media_id, @cardstyle_id)";
                cmd.Parameters.Add("@media_id", mediaId);
                cmd.Parameters.Add("@cardstyle_id", Id);
                int? newId = MSSQLCEConn.ExecuteScalar<int>(cmd);
                if (newId.HasValue)
                    return newId.Value;
                else
                    return -1;
            }
        }

        public List<int> GetMediaForCardStyle(int Id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT media_id FROM \"MediaContent_CardStyles\" WHERE cardstyles_id = @cardstyles_id";
                cmd.Parameters.Add("@cardstyles_id", Id);
                SqlCeDataReader reader;
                reader = cmd.ExecuteReader();

                List<int> cs = new List<int>();
                while (reader.Read())
                    cs.Add(Convert.ToInt32(reader["media_id"]));

                return cs;
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
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM \"MediaContent_CardStyles\" WHERE media_id = @media_id AND cardstyle_id = @cardstyle_id";
                cmd.Parameters.Add("@media_id", mediaId);
                cmd.Parameters.Add("@cardstyle_id", Id);
                MSSQLCEConn.ExecuteNonQuery(cmd);
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
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM \"MediaContent_CardStyles\" WHERE cardstyles_id = @cardstyle_id";
                cmd.Parameters.Add("@cardstyle_id", Id);
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }
        }
        #endregion
    }
}
