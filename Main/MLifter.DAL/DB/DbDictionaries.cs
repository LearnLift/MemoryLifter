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
using System.Globalization;
using System.IO;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.DB.MsSqlCe;
using System.Diagnostics;
using MLifter.DAL.Security;

namespace MLifter.DAL.DB
{
    class DbDictionaries : IDictionaries
    {
        /// <summary>
        /// Gets the connector.
        /// </summary>
        /// <value>The connector.</value>
        private Interfaces.DB.IDbDictionariesConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlDictionariesConnector.GetInstance(Parent.GetChildParentClass(this));
                    case DatabaseType.Unc:
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeDictionariesConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private Interfaces.DB.IDbExtensionConnector extensionConnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlExtensionConnector.GetInstance(Parent.GetChildParentClass(this));
                    case DatabaseType.Unc:
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeExtensionConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        /// <summary>
        /// Gets the connector.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-20</remarks>
        internal static IDbDictionariesConnector GetConnector(ParentClass parentClass)
        {
            switch (parentClass.CurrentUser.ConnectionString.Typ)
            {
                case DatabaseType.PostgreSQL:
                    return PgSqlDictionariesConnector.GetInstance(parentClass);
                case DatabaseType.Unc:
                case DatabaseType.MsSqlCe:
                    return MsSqlCeDictionariesConnector.GetInstance(parentClass);
                default:
                    throw new UnsupportedDatabaseTypeException(parentClass.CurrentUser.ConnectionString.Typ);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbDictionaries"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        internal DbDictionaries(ParentClass parent)
        {
            this.parent = parent;
        }

        #region IDictionaries Members

        /// <summary>
        /// Gets all learning modules.
        /// </summary>
        /// <value>The dictionaries.</value>
        public IList<IDictionary> Dictionaries
        {
            get
            {
                if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Unc)
                {
                    List<IDictionary> dics = new List<IDictionary>();
                    string folder = parent.CurrentUser.ConnectionString.ConnectionString;
                    if (!Directory.Exists(folder))
                        return dics;

                    string[] dicPaths = Helper.GetFilesRecursive(folder, "*" + Helper.EmbeddedDbExtension);

                    foreach (string path in dicPaths)
                    {
                        int lmId;
                        try
                        {
                            IList<int> ids = GetConnector(new ParentClass(new DbUser(parent.CurrentUser.AuthenticationStruct, parent,
                                new ConnectionStringStruct(DatabaseType.MsSqlCe, path, -1), parent.CurrentUser.ErrorMessageDelegate, true), null)).GetLMIds();
                            if (ids.Count == 0)
                                continue;
                            lmId = ids[0];
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("DbDictionaries.Dictionaries - " + ex.Message);
                            continue;
                        }
                        dics.Add(new DbDictionary(lmId, UserFactory.Create(parent.CurrentUser.GetLoginDelegate,
                            new ConnectionStringStruct(DatabaseType.MsSqlCe, path, lmId), parent.CurrentUser.ErrorMessageDelegate, parent.CurrentUser)));
                    }

                    return dics;
                }
                else
                {
                    parent.CurrentUser.CheckConnection(parent.CurrentUser.ConnectionString);

                    IList<IDictionary> list = new List<IDictionary>();

                    foreach (int id in connector.GetLMIds())
                    {
                        DbDictionary dictionary = new DbDictionary(id, parent.CurrentUser);
                        if (dictionary.HasPermission(PermissionTypes.Visible))
                            list.Add(dictionary);
                    }

                    return list;
                }
            }
        }

        /// <summary>
        /// Gets the specified learning module.
        /// </summary>
        /// <param name="id">The id of the learning module.</param>
        /// <returns></returns>
        public IDictionary Get(int id)
        {
            DbDictionary dictionary = new DbDictionary(id, parent.CurrentUser);
            if (dictionary.HasPermission(PermissionTypes.Visible))
                return dictionary;
            else
                throw new PermissionException();
        }

        /// <summary>
        /// Deletes a specific LM.
        /// </summary>
        /// <param name="css">The connection string struct.</param>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        public void Delete(ConnectionStringStruct css)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            switch (css.Typ)
            {
                case DatabaseType.PostgreSQL:
                    parent.CurrentUser.CheckConnection(parent.CurrentUser.ConnectionString);
                    connector.DeleteLM(css.LmId);
                    break;
                case DatabaseType.MsSqlCe:
                    //MSSQLCEConn.CloseAllConnections();
                    MSSQLCEConn.CloseMyConnection(css.ConnectionString);        //[ML-1939] "Delete" of EDB needs very long time
                    File.Delete(css.ConnectionString);
                    break;
                default:
                    throw new UnsupportedDatabaseTypeException(css.Typ);
            }
        }

        /// <summary>
        /// Adds a new learning module.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="title">The title.</param>
        /// <returns>The new learning module.</returns>
        public IDictionary AddNew(int categoryId, string title)
        {
            return AddNew(categoryId, title, string.Empty, false, 1);
        }
		/// <summary>
		/// Adds a new learning module.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="title">The title.</param>
		/// <param name="licenceKey"></param>
		/// <param name="contentProtected"></param>
		/// <param name="calCount">The cal count.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2009-02-12
		/// </remarks>
        public IDictionary AddNew(int categoryId, string title, string licenceKey, bool contentProtected, int calCount)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            string guid = Guid.NewGuid().ToString();
            string invariantCulture = CultureInfo.InvariantCulture.Name;

            if (parent.CurrentUser.ConnectionString.Typ != DatabaseType.Unc && parent.CurrentUser.ConnectionString.Typ != DatabaseType.MsSqlCe && parent.CurrentUser.ConnectionString.Typ != DatabaseType.Xml)
                parent.CurrentUser.CheckConnection(parent.CurrentUser.ConnectionString);

            return new DbDictionary(connector.AddNewLM(guid, categoryId, title, licenceKey, contentProtected, calCount), parent.CurrentUser);
        }

        /// <summary>
        /// Gets the count of learning modules.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                parent.CurrentUser.CheckConnection(parent.CurrentUser.ConnectionString);
                return connector.GetLMCount();
            }
        }

        /// <summary>
        /// Gets all extensions (independent of the LearningModule id).
        /// </summary>
        /// <value>The extensions.</value>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        public IList<IExtension> Extensions
        {
            get
            {
                IList<IExtension> extensions = new List<IExtension>();
                IList<Guid> ExtensionGuids = connector.GetExtensions();

                foreach (Guid guid in ExtensionGuids)
                    extensions.Add(new DbExtension(guid, parent));

                return extensions;
            }
        }

        /// <summary>
        /// Creates new extensions.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public IExtension ExtensionFactory()
        {
            Guid extensionGuid = extensionConnector.AddNewExtension();
            return new DbExtension(extensionGuid, parent);
        }

        /// <summary>
        /// Deletes the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <remarks>Documented by Dev02, 2009-07-10</remarks>
        /// <remarks>Documented by Dev02, 2009-07-10</remarks>
        public void DeleteExtension(IExtension extension)
        {
            extensionConnector.DeleteExtension(extension.Id);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
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
