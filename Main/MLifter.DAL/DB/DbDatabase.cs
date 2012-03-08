using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using System.Diagnostics;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Database implementation of IDatabase.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    internal class DbDatabase : IDatabase
    {
        private static Dictionary<ConnectionStringStruct, DbDatabase> instances = new Dictionary<ConnectionStringStruct, DbDatabase>();
        /// <summary>
        /// Gets a static instance of this class.
        /// </summary>
        /// <value>The instance.</value>
        /// <remarks>Documented by Dev03, 2008-09-10</remarks>
        public static DbDatabase GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new DbDatabase(parentClass));

            return instances[connection];
        }

        private DbDatabase(ParentClass parentClass)
        {
            this.parent = parentClass;
        }

        private Interfaces.DB.IDbDatabaseConnector connector
        {
            get
            {
                switch (Parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return MLifter.DAL.DB.PostgreSQL.PgSqlDatabaseConnector.GetInstance(Parent.GetChildParentClass(this));
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeDatabaseConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        #region IDatabase Members

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        /// <remarks>Documented by Dev03, 2008-09-10</remarks>
        public Version DatabaseVersion
        {
            get
            {
                try
                {
                    return new Version(connector.GetDatabaseVersion());
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to get DB version! (" + ex.ToString() + ")", "DbDatabase");
                    return new Version(1, 0, 0);
                }
            }
        }

        /// <summary>
        /// Gets the supported data layer versions.
        /// </summary>
        /// <value>The supported data layer versions.</value>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        /// <remarks>Documented by Dev03, 2008-09-10</remarks>
        public List<DataLayerVersionInfo> SupportedDataLayerVersions
        {
            get
            {
                List<DataLayerVersionInfo> versions = new List<DataLayerVersionInfo>();
                foreach (string version in connector.GetSupportedDLVersions())
                {
                    if (version.Trim().StartsWith(">"))
                        versions.Add(new DataLayerVersionInfo(VersionType.LowerBound, new Version(version.Trim().Substring(1))));
                    else if (version.Trim().StartsWith("<"))
                        versions.Add(new DataLayerVersionInfo(VersionType.UpperBound, new Version(version.Trim().Substring(1))));
                    else
                        versions.Add(new DataLayerVersionInfo(VersionType.Equal, new Version(version)));
                }
                return versions;
            }
        }

        /// <summary>
        /// Checks if a connection to the database is supported by this data layer. Throws an exception of DatabaseVersionNotSupported if not.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        /// <remarks>Documented by Dev03, 2008-09-10</remarks>
        public void CheckIfDatabaseVersionSupported()
        {
            if (DatabaseVersion < MsSqlCe.MSSQLCEConn.RequiredDatabaseVersion)
            {
                bool silent = false;
                if (Parent.CurrentUser.ConnectionString.Typ == DatabaseType.MsSqlCe)
                    silent = DatabaseVersion.Major == MsSqlCe.MSSQLCEConn.RequiredDatabaseVersion.Major &&
                       DatabaseVersion.Minor == MsSqlCe.MSSQLCEConn.RequiredDatabaseVersion.Minor;

                throw new DatabaseVersionNotSupported(SupportedDataLayerVersions, DatabaseVersion, AssemblyData.Version, silent) { CanUpdate = true };
            }

            bool rangeMatch = false;
            bool equalMatch = this.SupportedDataLayerVersions.Exists(
                delegate(DataLayerVersionInfo version)
                {
                    bool match = false;
                    if (version.Type == VersionType.Equal)
                    {
                        match = (version.Version.Major == AssemblyData.Version.Major)
                            && (version.Version.Minor == AssemblyData.Version.Minor)
                            && ((version.Version.Build == AssemblyData.Version.Build) || (version.Version.Build == -1))
                            && ((version.Version.Revision == AssemblyData.Version.Revision) || (version.Version.Revision == -1));
                    }
                    return match;
                });
            if (!equalMatch)
            {
                bool higherMatch = this.SupportedDataLayerVersions.Exists(
                    delegate(DataLayerVersionInfo version)
                    {
                        bool match = false;
                        if (version.Type == VersionType.LowerBound)
                        {
                            match = (version.Version < AssemblyData.Version);
                        }
                        return match;
                    });
                if (higherMatch)
                {
                    rangeMatch = !this.SupportedDataLayerVersions.Exists(
                        delegate(DataLayerVersionInfo version)
                        {
                            bool match = false;
                            if (version.Type == VersionType.UpperBound)
                            {
                                match = (version.Version < AssemblyData.Version);
                            }
                            return match;
                        });
                }
                else
                {
                    rangeMatch = this.SupportedDataLayerVersions.Exists(
                        delegate(DataLayerVersionInfo version)
                        {
                            bool match = false;
                            if (version.Type == VersionType.UpperBound)
                            {
                                match = (version.Version > AssemblyData.Version);
                            }
                            return match;
                        });
                }
            }
            if (!(equalMatch || rangeMatch))
                throw new DatabaseVersionNotSupported(this.SupportedDataLayerVersions, this.DatabaseVersion, AssemblyData.Version);
        }

        /// <summary>
        /// Upgrades the database to the current version.
        /// </summary>
        /// <param name="currentVersion">The current version of the database.</param>
        /// <returns>[true] if success.</returns>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        public bool UpgradeDatabase(Version currentVersion)
        {
            ConnectionStringStruct css = Parent.CurrentUser.ConnectionString;
            if (css.Typ != DatabaseType.MsSqlCe)
                return false;

            return MsSqlCe.MSSQLCEConn.UpgradeDatabase(Parent.CurrentUser, currentVersion);
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
    }
}
