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
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE connector for the IDbMediaConnector interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-12</remarks>
    class MsSqlCeMediaConnector : IDbMediaConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeMediaConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeMediaConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public static MsSqlCeMediaConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeMediaConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeMediaConnector(ParentClass parentClass)
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
        /// Gets the byte array from stream.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="rpu">The rpu.</param>
        /// <param name="caller">The caller.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        private static byte[] GetByteArrayFromStream(System.IO.Stream media, MLifter.DAL.Tools.StatusMessageReportProgress rpu, object caller)
        {
            int buffer_length = 10240;
            byte[] data = new byte[media.Length];
            StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.CreateMediaProgress, (int)media.Length);

            media.Seek(0, SeekOrigin.Begin);

            int read = 0;
            int pos = 0;
            do
            {
                read = media.Read(data, pos, Math.Min(buffer_length, data.Length - pos));
                pos += read;
                args.Progress = pos;
                if (rpu != null)
                    rpu.Invoke(args, caller);
            }
            while (read == buffer_length);
            return data;
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

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT id FROM MediaContent";
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

            while (reader.Read())
                ids.Add(Convert.ToInt32(reader["id"]));

            return ids;
        }

        /// <summary>
        /// Gets the empty media resources.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-31</remarks>
        public List<int> GetEmptyMediaResources(int id)
        {
            List<int> ids = new List<int>();

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT id FROM MediaContent WHERE data IS NULL";
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

            while (reader.Read())
                ids.Add(Convert.ToInt32(reader["id"]));

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
        public int CreateMedia(System.IO.Stream media, MLifter.DAL.Interfaces.EMedia type, MLifter.DAL.Tools.StatusMessageReportProgress rpu, object caller)
        {
            int newId;
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "INSERT INTO MediaContent (data, media_type) VALUES (@data, @type); SELECT @@IDENTITY;";
            cmd.Parameters.Add("@data", SqlDbType.Image);
            cmd.Parameters.Add("@type", SqlDbType.NVarChar, 100);
            cmd.Parameters["@data"].Value = GetByteArrayFromStream(media, rpu, caller);
            cmd.Parameters["@type"].Value = type.ToString();
            newId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

            return newId;
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
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT media_type FROM MediaContent WHERE id=@id";
                cmd.Parameters.Add("@id", SqlDbType.Int, 4);
                cmd.Parameters["@id"].Value = id;
                return (EMedia)Enum.Parse(typeof(EMedia), Convert.ToString(MSSQLCEConn.ExecuteScalar(cmd)));
            }
            catch(ArgumentException)
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
        public void UpdateMedia(int id, System.IO.Stream media)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE MediaContent SET data=@data WHERE id=@id";
            cmd.Parameters.Add("@data", SqlDbType.Image);
            cmd.Parameters.Add("@id", SqlDbType.Int, 4);
            cmd.Parameters["@data"].Value = GetByteArrayFromStream(media, null, null);
            cmd.Parameters["@id"].Value = id;
            MSSQLCEConn.ExecuteNonQuery(cmd);
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
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT count(*) FROM MediaContent WHERE id=@id AND data IS NOT NULL";
            cmd.Parameters.Add("@id", id);

            return MSSQLCEConn.ExecuteScalar<int>(cmd).Value > 0;
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
        public System.IO.Stream GetMediaStream(int id, IDbMediaConnector cacheConnector)
        {
            MemoryStream stream = null;
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT data FROM \"MediaContent\" WHERE id=@id;";
            cmd.Parameters.Add("@id", id);
            byte[] media = (byte[])MSSQLCEConn.ExecuteScalar(cmd);
            if (media != null)
            {
                stream = new MemoryStream(media);
                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }
        /// <summary>
        /// Deletes the media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void DeleteMedia(int id)
        {
            SqlCeCommand deletecmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            deletecmd.CommandText = "DELETE FROM MediaProperties WHERE media_id=@id; DELETE FROM MediaContent WHERE id=@id;";
            deletecmd.Parameters.Add("@id", id);
            MSSQLCEConn.ExecuteNonQuery(deletecmd);
        }

        /// <summary>
        /// Gets the properties for a media object.
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <returns>A list of properties.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public Dictionary<MLifter.DAL.Interfaces.MediaProperty, string> GetProperties(int id)
        {
            Dictionary<MediaProperty, string> props = new Dictionary<MediaProperty, string>();
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT media_id, property, value FROM MediaProperties WHERE media_id=@id;";
            cmd.Parameters.Add("@id", id);
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
            while (reader.Read())
                try
                {
                    props.Add((MediaProperty)Enum.Parse(typeof(MediaProperty), Convert.ToString(reader["property"]), true), Convert.ToString(reader["value"]));
                }
                catch (ArgumentException) //ignore argument exception (thrown in case the enum cannot be parsed).
                { }
            reader.Close();
            return props;
        }
        /// <summary>
        /// Sets the properties for a media object.
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <param name="properties">The properties for the media object.</param>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetProperties(int id, Dictionary<MLifter.DAL.Interfaces.MediaProperty, string> properties)
        {
            SqlCeCommand cmd1 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            SqlCeTransaction tran = cmd1.Connection.BeginTransaction();

            cmd1.CommandText = "DELETE FROM MediaProperties WHERE media_id=@id;";
            cmd1.Parameters.Add("@id", id);
            MSSQLCEConn.ExecuteNonQuery(cmd1);

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "INSERT INTO MediaProperties (media_id, property, value) VALUES (@media_id, @property, @value);";
            cmd.Parameters.Add("@media_id", SqlDbType.Int, 4);
            cmd.Parameters.Add("@property", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@value", SqlDbType.NVarChar, 100);
            foreach (KeyValuePair<MediaProperty, string> item in properties)
            {
                cmd.Parameters["@media_id"].Value = id;
                cmd.Parameters["@property"].Value = item.Key.ToString();
                cmd.Parameters["@value"].Value = item.Value;
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }

            tran.Commit();
        }

        /// <summary>
        /// Gets a single property value for a media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string GetPropertyValue(int id, MLifter.DAL.Interfaces.MediaProperty property)
        {
            Dictionary<MediaProperty, string> properties = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MediaProperties, id)] as Dictionary<MediaProperty, string>;
            if (properties != null && properties.ContainsKey(property))
                return properties[property];

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT property, value FROM MediaProperties WHERE media_id=@id";
            cmd.Parameters.Add("@id", id);
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

            properties = new Dictionary<MediaProperty, string>();
            while (reader.Read())
                properties[(MediaProperty)Enum.Parse(typeof(MediaProperty), reader["property"].ToString())] = reader["value"].ToString();

            reader.Close();
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.MediaProperties, id, new TimeSpan(1, 0, 0))] = properties;

            if (properties.ContainsKey(property))
                return properties[property];
            else
                return null;
        }
        /// <summary>
        /// Sets a single property value for a media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Documented by Dev02, 2008-08-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetPropertyValue(int id, MLifter.DAL.Interfaces.MediaProperty property, string value)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            SqlCeTransaction tran = cmd.Connection.BeginTransaction();

            if (GetPropertyValue(id, property) == null)
                cmd.CommandText = "INSERT INTO MediaProperties (media_id, property, value) VALUES (@media_id, @property, @value);";
            else
                cmd.CommandText = "UPDATE MediaProperties SET value=@value WHERE media_id=@media_id AND property=@property;";

            Dictionary<MediaProperty, string> properties = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MediaProperties, id)] as Dictionary<MediaProperty, string>;
            if (properties == null)
                properties = new Dictionary<MediaProperty, string>();
            properties[property] = value;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.MediaProperties, id, new TimeSpan(1, 0, 0))] = properties;

            cmd.Parameters.Add("@media_id", id);
            cmd.Parameters.Add("@property", property.ToString());
            cmd.Parameters.Add("@value", value);
            MSSQLCEConn.ExecuteNonQuery(cmd);

            tran.Commit();
        }

        /// <summary>
        /// Checks if the media object exists.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void CheckMediaId(int id)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT count(*) FROM MediaContent WHERE id=@id;";
            cmd.Parameters.Add("@id", id);
            if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                throw new IdAccessException(id);
        }

        #endregion
    }
}
