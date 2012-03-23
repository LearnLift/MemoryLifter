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
using System.IO;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    /// <summary>
    /// XML implementation of IMedia.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    internal abstract class XmlMedia : IMedia
    {
        private EMedia m_mediaIdentifier;
        protected string m_filename;
        protected bool m_default = false;
        protected bool m_active = true;
        protected bool m_example = false;
        private XmlDictionary m_oDictionary;

        protected XmlMedia(EMedia mediaType, string filename, ParentClass parent)
        {
            this.parent = parent;
            m_mediaIdentifier = mediaType;
            m_filename = filename;
        }

        protected XmlMedia(XmlDictionary dictionary, EMedia mediaType, string filename, ParentClass parent)
        {
            this.parent = parent;
            m_mediaIdentifier = mediaType;
            m_oDictionary = dictionary;
            m_filename = filename;
        }

        internal XmlDictionary Dictionary
        {
            get { return m_oDictionary; }
            set { m_oDictionary = value; }
        }

        #region IMedia Members

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public int Id
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public string Filename
        {
            get
            {
                if (Dictionary == null)
                    return m_filename;
                if (File.Exists(m_filename))
                    return m_filename;
                else if (File.Exists(Path.Combine(Path.GetDirectoryName(Dictionary.Path), m_filename)))
                    return Path.Combine(Path.GetDirectoryName(Dictionary.Path), m_filename);
                else
                    return String.Empty;
            }
            set
            {
                m_filename = value;
            }
        }

        /// <summary>
        /// Gets the stream containing the media file.
        /// </summary>
        /// <value>The stream.</value>
        /// <remarks>Documented by Dev05, 2008-08-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public Stream Stream
        {
            get { return File.OpenRead(Filename); }
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <value>The type of the media.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public EMedia MediaType
        {
            get
            {
                return m_mediaIdentifier;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IMedia"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public bool? Active
        {
            get
            {
                return m_active;
            }
            set
            {
                m_active = value.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IAudio"/> is an example audio file.
        /// </summary>
        /// <value><c>true</c> if example; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public bool? Example
        {
            get
            {
                return m_example;
            }
            set
            {
                m_example = value.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IAudio"/> is the default audio file.
        /// </summary>
        /// <value><c>true</c> if default; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public bool? Default
        {
            get
            {
                return m_default;
            }
            set
            {
                m_default = value.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Gets the mime type for the file.
        /// </summary>
        /// <value>The mime type.</value>
        /// <remarks>Documented by Dev03, 2008-08-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public string MimeType
        {
            get { return Helper.GetMimeType(m_filename); }
        }

        /// <summary>
        /// Gets the extension for the file.
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks>Documented by Dev03, 2008-08-13</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public string Extension
        {
            get { return Path.GetExtension(m_filename); }
        }

        /// <summary>
        /// Gets the size of the media.
        /// </summary>
        /// <value>The size of the media.</value>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public int MediaSize
        {
            get
            {
                try
                {
                    return (int)Stream.Length;
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public override bool Equals(object obj)
        {
            XmlMedia media = obj as XmlMedia;
            if (media != null)
                return media.GetHashCode() == this.GetHashCode();
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public override int GetHashCode()
        {
            //describe unique media object
            return (Filename.ToString()).GetHashCode();
        }

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
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
            return true;
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            return new List<SecurityFramework.PermissionInfo>();
        }

        #endregion
    }
}
