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
using MLifter.DAL.Tools;
using System.Drawing;

namespace MLifter.DAL.Preview
{
    /// <summary>
    /// Used for preview and in the CardEdit for new media objects.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-25</remarks>
    internal class PreviewMedia : Interfaces.IMedia
    {
        public PreviewMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample, ParentClass parent)
        {
            this.parent = parent;
            this.mediatype = type;
            this.filename = path;
            this.active = isActive;
            this._default = isDefault;
            this.example = isExample;
        }

        #region IMedia Members

        public int Id
        {
            get { return -1; }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public System.IO.Stream Stream
        {
            get
            {
                if (System.IO.File.Exists(Filename))
                    return System.IO.File.OpenRead(Filename);
                else
                    return null;
            }
        }

        private EMedia mediatype;
        public MLifter.DAL.Interfaces.EMedia MediaType
        {
            get { return mediatype; }
        }

        private bool? active;
        public bool? Active
        {
            get { return active; }
        }

        private bool? example;
        public bool? Example
        {
            get { return example; }
        }

        private bool? _default;
        public bool? Default
        {
            get { return _default; }
        }

        public string MimeType
        {
            get { return Helper.GetMimeType(Filename); }
        }

        public string Extension
        {
            get { return System.IO.Path.GetExtension(Filename); }
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

        #region IParent Members

        private ParentClass parent;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Used for preview and in the CardEdit for new media objects.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-25</remarks>
    internal class PreviewAudio : PreviewMedia, IAudio
    {
        public PreviewAudio(string path, bool isActive, bool isDefault, bool isExample, ParentClass parent)
            : base(EMedia.Audio, path, isActive, isDefault, isExample, parent) { }
    }

    /// <summary>
    /// Used for preview and in the CardEdit for new media objects.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-25</remarks>
    internal class PreviewImage : PreviewMedia, IImage
    {
        public PreviewImage(string path, bool isActive, bool isDefault, bool isExample, ParentClass parent)
            : base(EMedia.Image, path, isActive, isDefault, isExample, parent)
        {
            try
            {
                if (Stream != null)
                {
                    using (Image image = Image.FromStream(Stream))
                    {
                        Height = image.Size.Height;
                        Width = image.Size.Width;
                    }
                }
            }
            catch { }
        }

        #region IImage Members

        int width = 0;
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        int height = 0;
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Used for preview and in the CardEdit for new media objects.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-25</remarks>
    internal class PreviewVideo : PreviewMedia, IVideo
    {
        public PreviewVideo(string path, bool isActive, bool isDefault, bool isExample, ParentClass parent)
            : base(EMedia.Video, path, isActive, isDefault, isExample, parent) { }
    }

}
