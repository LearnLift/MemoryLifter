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
using System.Diagnostics;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Database implementation of IChapter.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    class DbChapter : IChapter
    {
        private Interfaces.DB.IDbChapterConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return MLifter.DAL.DB.PostgreSQL.PgSqlChapterConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeChapterConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbChapter"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbChapter(int id, ParentClass parent)
            : this(id, true, parent) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DbChapter"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="CheckId">if set to <c>true</c> [check id].</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbChapter(int id, bool CheckId, ParentClass parent)
        {
            this.parent = parent;
            if (CheckId)
                connector.CheckChapterId(id);
            this.id = id;
        }

        /// <summary>
        /// Gets the lm id.
        /// </summary>
        /// <value>The lm id.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int LmId
        {
            get
            {
                return connector.GetLmId(id);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public override bool Equals(object obj)
        {
            DbChapter chapter = obj as DbChapter;
            if (chapter == null)
                return false;

            return this.GetHashCode() == chapter.GetHashCode();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public override int GetHashCode()
        {
            return (this.Id + this.ToString()).GetHashCode();
        }

        #region IChapter Members

        private int id;
        /// <summary>
        /// Gets or sets the id for a chapter.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Gets an Xml representation of the chapter.
        /// </summary>
        /// <value>The chapter.</value>
        /// <remarks>Documented by Dev03, 2008-08-14</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public System.Xml.XmlElement Chapter
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                settings.Encoding = Encoding.Unicode;
                settings.Indent = true;
                System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sb, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("chapter");
                writer.WriteAttributeString("id", Id.ToString());

                writer.WriteStartElement("title");
                writer.WriteString(Title);
                writer.WriteEndElement();

                writer.WriteStartElement("descriptipn");
                writer.WriteString(Title);
                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Flush();

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                string xml = sb.ToString();
                try
                {
                    doc.LoadXml(xml);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                return doc.DocumentElement;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string Title
        {
            get
            {
                return connector.GetTitle(id);
            }
            set
            {
                connector.SetTitle(id, value);
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string Description
        {
            get
            {
                return connector.GetDescription(id);
            }
            set
            {
                connector.SetDescription(id, value);
            }
        }

        /// <summary>
        /// Gets the total number of cards.
        /// </summary>
        /// <value>The total number of cards.</value>
        /// <remarks>Documented by Dev03, 2007-11-22</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Size
        {
            get { return connector.GetSize(id); }
        }

        /// <summary>
        /// Gets the number of active cards.
        /// </summary>
        /// <value>The number of active cards.</value>
        /// <remarks>Documented by Dev03, 2007-11-22</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int ActiveSize
        {
            get { return connector.GetActiveSize(id); }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public MLifter.DAL.Interfaces.ICardStyle Style
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Creates and returns a card style.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public MLifter.DAL.Interfaces.ICardStyle CreateCardStyle()
        {
            return connector.CreateId();
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public MLifter.DAL.Interfaces.ISettings Settings
        {
            get
            {
                return connector.GetSettings(id);
            }
            set
            {
                connector.SetSettings(id, value);
            }
        }

        #endregion

        #region ICopy Members

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void CopyTo(ICopy target, CopyToProgress progressDelegate)
        {
            IChapter targetChapter = target as IChapter;
            if (targetChapter != null && targetChapter.Settings == null && this.Settings != null)
                targetChapter.Settings = targetChapter.Parent.GetParentDictionary().CreateSettings();

            CopyBase.Copy(this, target, typeof(IChapter), progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
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
