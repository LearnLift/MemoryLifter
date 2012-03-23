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
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// DB implementation of IMedia.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    abstract class DbMedia : IMedia
    {
        protected Interfaces.DB.IDbCardMediaConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlCardMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        switch (parent.CurrentUser.ConnectionString.SyncType)
                        {
                            case SyncType.NotSynchronized:
                                return MsSqlCeCardMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithDbAccess:
                                return PgSqlCardMediaConnector.GetInstance(new ParentClass(parent.CurrentUser.ConnectionString.ServerUser, this));
                            case SyncType.FullSynchronized:
                                return MsSqlCeCardMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithoutDbAccess:
                            default:
                                throw new NotAllowedInSyncedModeException();
                        }
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        protected Interfaces.DB.IDbMediaConnector mediaconnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PostgreSQL.PgSqlMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        switch (parent.CurrentUser.ConnectionString.SyncType)
                        {
                            case SyncType.NotSynchronized:
                                return MsSqlCeMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithDbAccess:
                                return PgSqlMediaConnector.GetInstance(new ParentClass(parent.CurrentUser.ConnectionString.ServerUser, this));
                            case SyncType.FullSynchronized:
                                return MsSqlCeMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithoutDbAccess:
                            default:
                                throw new NotAllowedInSyncedModeException();
                        }
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private int id;
        private int cardid;

        internal int CardId
        {
            get { return cardid; }
        }

        private EMedia mediaIdentifier = EMedia.Unknown;

        internal DbMedia(int id, int cardid, bool checkId, EMedia mediaIdentifier, Side side, WordType type, bool isDefault, bool isExample, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckCardMediaId(id, cardid);

            this.id = id;
            this.cardid = cardid;
            this.mediaIdentifier = mediaIdentifier;
            this.side = side;
            this.type = type;
            this.isDefault = isDefault;
            this.isExample = isExample;
        }

        private Side side;

        public Side Side
        {
            get { return side; }
        }

        private WordType type;

        public WordType Type
        {
            get { return type; }
        }

        private bool isDefault;

        public bool IsDefault
        {
            get { return isDefault; }
        }

        private bool isExample;

        public bool IsExample
        {
            get { return isExample; }
        }

        public bool IsDisconnected
        {
            get { return (cardid < 0); }
        }

        #region IMedia Members

        public int Id
        {
            get { return this.id; }
        }

        public string Filename
        {
            get
            {
                return DbMediaServer.DbMediaServer.Instance(parent).GetMediaURI(id).ToString();
            }
            set
            {
                Uri uri = new Uri(value);

                if (uri.Scheme == "file" && uri.IsFile && File.Exists(value))
                {
                    using (FileStream stream = new FileStream(value, FileMode.Open))
                        mediaconnector.UpdateMedia(id, stream);

                    Helper.UpdateMediaProperties(value, id, mediaconnector);
                }

            }
        }

        public Stream Stream
        {
            get { return mediaconnector.GetMediaStream(id); }
        }

        public MLifter.DAL.Interfaces.EMedia MediaType
        {
            get { return mediaIdentifier; }
        }

        public bool? Active
        {
            get { return true; }
            set { }
        }

        public bool? Example
        {
            get
            {
                return IsExample;
            }
        }

        public bool? Default
        {
            get
            {
                return IsDefault;
            }
        }

        public string MimeType
        {
            get
            {
                string mimetype = mediaconnector.GetPropertyValue(id, MediaProperty.MimeType);

                if (mimetype == null)
                    return Helper.UnknownMimeType;

                return mimetype;
            }
        }

        public string Extension
        {
            get
            {
                string extension = mediaconnector.GetPropertyValue(id, MediaProperty.Extension);

                if (extension == null)
                    return String.Empty;

                return extension;
            }
        }


        /// <summary>
        /// Gets or sets the size of the media.
        /// </summary>
        /// <value>The size of the media.</value>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        public int MediaSize
        {
            get
            {
                string mediaSize = mediaconnector.GetPropertyValue(id, MediaProperty.MediaSize);

                try
                {
                    return Convert.ToInt32(mediaSize);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                mediaconnector.SetPropertyValue(id, MediaProperty.MediaSize, value.ToString());
            }
        }

        #endregion

        /// <summary>
        /// Creates a disconnected card media object.
        /// </summary>
        /// <param name="id">The id of the media.</param>
        /// <param name="mediaIdentifier">The media type identifier.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="isExample">if set to <c>true</c> [is example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        internal IMedia CreateDisconnectedCardMedia(int id, EMedia mediaIdentifier, bool isDefault, bool isExample)
        {
            switch (mediaIdentifier)
            {
                case EMedia.Audio:
                    return new DbAudio(id, -1, false, Side.Answer, WordType.Word, isDefault, isExample, parent);
                case EMedia.Image:
                    return new DbImage(id, -1, false, Side.Answer, WordType.Word, isDefault, isExample, parent);
                case EMedia.Video:
                    return new DbVideo(id, -1, false, Side.Answer, WordType.Word, isDefault, isExample, parent);
                default:
                    throw new ArgumentException("Invalid EMedia type.");
            }
        }

		/// <summary>
		/// Creates a disconnected card media object.
		/// </summary>
		/// <param name="id">The id of the media.</param>
		/// <param name="mediaIdentifier">The media type identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <param name="parent">The parent.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by AAB, 6.8.2008.
		/// </remarks>
        internal static IMedia CreateDisconnectedCardMedia(int id, EMedia mediaIdentifier, bool isDefault, bool isExample, ParentClass parent)
        {
            switch (mediaIdentifier)
            {
                case EMedia.Audio:
                    return new DbAudio(id, -1, false, Side.Answer, WordType.Word, isDefault, isExample, parent);
                case EMedia.Image:
                    return new DbImage(id, -1, false, Side.Answer, WordType.Word, isDefault, isExample, parent);
                case EMedia.Video:
                    return new DbVideo(id, -1, false, Side.Answer, WordType.Word, isDefault, isExample, parent);
                default:
                    throw new ArgumentException("Invalid EMedia type.");
            }
        }

        public override bool Equals(object obj)
        {
            DbMedia media = obj as DbMedia;

            if (media != null)
                return media.GetHashCode() == this.GetHashCode();

            return false;
        }

        public override int GetHashCode()
        {
            //describe a unique media object
            if (IsDisconnected)
                return (id.ToString()).GetHashCode();
            else
                return (id.ToString() + cardid.ToString()).GetHashCode();
        }

        #region IParent Members

        protected ParentClass parent;
        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion

        #region ISecurity Members

        /// <summary>
        /// Determines whether the object has the specified permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns>
        /// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public bool HasPermission(string permissionName)
        {
            return Parent.CurrentUser.HasPermission(this, permissionName);
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            return Parent.CurrentUser.GetPermissions(this);
        }

        #endregion
    }
}
