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
using System.IO;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces.DB;

namespace MLifter.DAL.DB.PostgreSQL
{
    /// <summary>
    /// The PostGreSQL connector for the IDbMediaConnector interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    internal class PgSqlMediaConnector : Interfaces.DB.IDbMediaConnector
    {

        private static Dictionary<ConnectionStringStruct, PgSqlMediaConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlMediaConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public static PgSqlMediaConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlMediaConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlMediaConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        /// <summary>
        /// The size of chunks to read from the db at once.
        /// </summary>
        private readonly int chunkSize = 204800; //200 KB

        /// <summary>
        /// Writes the content of a buffer into a LargeObject.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="largeObject">The large object.</param>
        /// <remarks>Documented by Dev02, 2008-08-08</remarks>
        private void BufferToLargeObject(byte[] buffer, LargeObject largeObject)
        {
            largeObject.Seek(0);

            int offset = 0;
            int size = buffer.Length;
            while (offset < size)
            {
                largeObject.Write(buffer, offset, Math.Min(chunkSize, size - offset));
                offset += chunkSize;
            }
        }

        /// <summary>
        /// Writes the content of a buffer into a LargeObject.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="largeObject">The large object.</param>
        /// <param name="rpu">The rpu.</param>
        /// <param name="caller">The calling object.</param>
        /// <remarks>Documented by Dev02, 2008-08-08</remarks>
        private void BufferToLargeObject(byte[] buffer, LargeObject largeObject, StatusMessageReportProgress rpu, object caller)
        {
            largeObject.Seek(0);

            int offset = 0;
            int size = buffer.Length;
            StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.CreateMediaProgress, buffer.Length);
            while (offset < size)
            {
                largeObject.Write(buffer, offset, Math.Min(chunkSize, size - offset));
                offset += chunkSize;
                args.Progress = offset;
                if (rpu != null)
                    rpu(args, caller);
            }
        }

        /// <summary>
        /// Gets the contents of a LargeObject into a buffer.
        /// </summary>
        /// <param name="largeObject">The large object.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-08</remarks>
        private byte[] LargeObjectToBuffer(LargeObject largeObject)
        {
            largeObject.Seek(0);
            int size = largeObject.Size();
            byte[] buffer = new byte[size];

            int offset = 0;
            while (offset < size)
            {
                largeObject.Read(buffer, offset, Math.Min(chunkSize, size - offset));
                offset += chunkSize;
            }

            return buffer;
        }

        #region IDbMediaConnector Members

        /// <summary>
        /// Gets the media resources.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        public List<int> GetMediaResources(int id)
        {
            List<int> ids = new List<int>();

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"Syncview_MediaContent\" AS S WHERE S.session_lm_id=:id";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["id"]));
                }
            }

            return ids;
        }

        /// <summary>
        /// Gets the media resources.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        public List<int> GetEmptyMediaResources(int id)
        {
            List<int> ids = new List<int>();

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"Syncview_MediaContent\" AS S WHERE S.session_lm_id=:id WHERE (SELECT data FROM \"MediaContent\" as MC WHERE MC.id=id LIMIT 1)=0;";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["id"]));
                }
            }

            return ids;
        }

        /// <summary>
        /// Creates a new media object.
        /// </summary>
        /// <param name="media">The memory stream containing the media.</param>
        /// <param name="type">The media type.</param>
        /// <param name="rpu">A delegate of type <see cref="StatusMessageReportProgress"/> used to send messages back to the calling object.</param>
        /// <param name="caller">The calling object.</param>
        /// <returns>The id for the new media object.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int CreateMedia(Stream media, EMedia type, StatusMessageReportProgress rpu, object caller)
        {
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction tran = conn.BeginTransaction();

                LargeObjectManager lbm = new LargeObjectManager(conn);
                int noid = lbm.Create(LargeObjectManager.READWRITE);
                LargeObject largeObject = lbm.Open(noid, LargeObjectManager.READWRITE);
                byte[] buffer = new byte[media.Length];
                media.Read(buffer, 0, (int)media.Length);
                BufferToLargeObject(buffer, largeObject, rpu, caller);
                largeObject.Close();

                int newId = 0;
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"MediaContent\" (data, media_type) VALUES (:data, :type) RETURNING id;";
                    cmd.Parameters.Add("data", noid);
                    cmd.Parameters.Add("type", type.ToString());
                    newId = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
                tran.Commit();

                return newId;
            }
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-06-25</remarks>
        /// <remarks>Documented by Dev02, 2009-06-25</remarks>
        public EMedia GetMediaType(int id)
        {
            try
            {
                using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT media_type FROM \"MediaContent\" WHERE id=:id;";
                        cmd.Parameters.Add("id", id);
                        return (EMedia)Enum.Parse(typeof(EMedia), Convert.ToString(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)));
                    }
                }
            }
            catch (ArgumentException)
            {
                return EMedia.Unknown;
            }
        }

        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="media">The media.</param>
        /// <remarks>Documented by Dev02, 2008-08-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void UpdateMedia(int id, Stream media)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction tran = con.BeginTransaction();

                int noid;
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM \"MediaContent\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    noid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }

                LargeObjectManager lbm = new LargeObjectManager(con);
                lbm.Delete(noid);

                noid = lbm.Create(LargeObjectManager.READWRITE);
                LargeObject largeObject = lbm.Open(noid, LargeObjectManager.READWRITE);
                byte[] buffer = new byte[media.Length];
                media.Read(buffer, 0, (int)media.Length);
                BufferToLargeObject(buffer, largeObject);
                largeObject.Close();

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"MediaContent\" SET data=:data WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("data", noid);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                tran.Commit();
            }
        }

        /// <summary>
        /// Deletes the media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void DeleteMedia(int id)
        {
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                int noid = 0;
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM \"MediaContent\" WHERE id=:id;";
                    cmd.Parameters.Add("id", id);
                    noid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }

                NpgsqlTransaction tran = conn.BeginTransaction();
                LargeObjectManager lbm = new LargeObjectManager(conn);
                lbm.Delete(noid);

                using (NpgsqlCommand deletecmd = conn.CreateCommand())
                {
                    deletecmd.CommandText = "DELETE FROM \"MediaContent\" WHERE id=:id;";
                    deletecmd.Parameters.Add("id", id);
                    PostgreSQLConn.ExecuteNonQuery(deletecmd, Parent.CurrentUser);
                }
                tran.Commit();
            }
        }

        /// <summary>
        /// Determines whether this media is available.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// 	<c>true</c> if media is available; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        public bool IsMediaAvailable(int id)
        {
            return true;
        }

        /// <summary>
        /// Gets the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        public Stream GetMediaStream(int id)
        {
            return GetMediaStream(id, null);
        }
        /// <summary>
        /// Gets the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cacheConnector">The cache connector.</param>
        /// <returns>A memory stream for the media object.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public Stream GetMediaStream(int id, IDbMediaConnector cacheConnector)
        {
            CachingStream stream = null;
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                int noid = 0;
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM \"MediaContent\" WHERE id=:id;";
                    cmd.Parameters.Add("id", id);
                    noid = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }

                NpgsqlTransaction tran = conn.BeginTransaction();
                LargeObjectManager lbm = new LargeObjectManager(conn);
                LargeObject largeObject = lbm.Open(noid, LargeObjectManager.READWRITE);
                byte[] buffer = LargeObjectToBuffer(largeObject);
                stream = new CachingStream(buffer, id, cacheConnector);
                largeObject.Close();
                tran.Commit();
            }
            return stream;
        }

        /// <summary>
        /// Gets the properties for a media object.
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <returns>A list of properties.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public Dictionary<MediaProperty, string> GetProperties(int id)
        {
            Dictionary<MediaProperty, string> props = new Dictionary<MediaProperty, string>();
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT media_id, property, \"value\" FROM \"MediaProperties\" WHERE media_id=:id;";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    while (reader.Read())
                        try
                        {
                            props.Add((MediaProperty)Enum.Parse(typeof(MediaProperty), Convert.ToString(reader["property"]), true), Convert.ToString(reader["value"]));
                        }
                        catch (ArgumentException) //ignore argument exception (thrown in case the enum cannot be parsed).
                        { }
                }
            }
            return props;
        }

        /// <summary>
        /// Sets the properties for a media object.
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <param name="properties">The properties for the media object.</param>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetProperties(int id, Dictionary<MediaProperty, string> properties)
        {
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction tran = conn.BeginTransaction();

                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"MediaProperties\" WHERE media_id=:id;";
                    cmd.Parameters.Add("id", id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                foreach (KeyValuePair<MediaProperty, string> item in properties)
                {
                    using (NpgsqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO \"MediaProperties\" (media_id, property, \"value\") VALUES (:media_id, :property, :value);";
                        cmd.Parameters.Add("media_id", id);
                        cmd.Parameters.Add("property", item.Key.ToString());
                        cmd.Parameters.Add("value", item.Value);
                        PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    }
                }

                tran.Commit();
            }
        }

        /// <summary>
        /// Gets a single property value for a media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string GetPropertyValue(int id, MediaProperty property)
        {
            Dictionary<MediaProperty, string> properties = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MediaProperties, id)] as Dictionary<MediaProperty, string>;
            if (properties != null && properties.ContainsKey(property))
                return properties[property];

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT property, value FROM \"MediaProperties\" WHERE media_id=:id";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    properties = new Dictionary<MediaProperty, string>();
                    while (reader.Read())
                        properties[(MediaProperty)Enum.Parse(typeof(MediaProperty), reader["property"].ToString())] = reader["value"].ToString();

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.MediaProperties, id, new TimeSpan(1, 0, 0))] = properties;

                    if (properties.ContainsKey(property))
                        return properties[property];
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// Sets a single property value for a media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Documented by Dev02, 2008-08-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetPropertyValue(int id, MediaProperty property, string value)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction tran = con.BeginTransaction();

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    if (GetPropertyValue(id, property) == null)
                        cmd.CommandText = "INSERT INTO \"MediaProperties\" (media_id, property, \"value\") VALUES (:media_id, :property, :value);";
                    else
                        cmd.CommandText = "UPDATE \"MediaProperties\" SET \"value\"=:value WHERE media_id=:media_id AND property=:property;";

                    cmd.Parameters.Add("media_id", id);
                    cmd.Parameters.Add("property", property.ToString());
                    cmd.Parameters.Add("value", value);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                tran.Commit();
            }
        }

        /// <summary>
        /// Checks the media id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void CheckMediaId(int id)
        {
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"MediaContent\" WHERE id=:id;";
                    cmd.Parameters.Add("id", id);
                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        #endregion
    }
}
