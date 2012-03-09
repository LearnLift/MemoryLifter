using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    internal interface IDbDatabaseConnector
    {
        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        string GetDatabaseVersion();
        /// <summary>
        /// Gets the supported data layer versions.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-09</remarks>
        List<string> GetSupportedDLVersions();
    }

    /// <summary>
    /// Database information property types.
    /// </summary>
    public enum DataBaseInformation
    {
        /// <summary>
        /// Database version
        /// </summary>
        Version,
        /// <summary>
        /// Supported data layer versions
        /// </summary>
        SupportedDataLayerVersions,
        /// <summary>
        /// Database supports list authentication
        /// </summary>
        ListAuthentication,
        /// <summary>
        /// Database supports forms authentication
        /// </summary>
        FormsAuthentication,
        /// <summary>
        /// Database supports local directory authentication
        /// </summary>
        LocalDirectoryAuthentication,
        /// <summary>
        /// Local directory type authentication
        /// </summary>
        LocalDirectoryType,
        /// <summary>
        /// Local directory server
        /// </summary>
        LdapServer,
        /// <summary>
        /// Local directory server port
        /// </summary>
        LdapPort,
        /// <summary>
        /// Local directory username
        /// </summary>
        LdapUser,
        /// <summary>
        /// Local directory user password
        /// </summary>
        LdapPassword,
        /// <summary>
        /// Local directory context
        /// </summary>
        LdapContext,
        /// <summary>
        /// Use SSL for local directory connection
        /// </summary>
        LdapUseSsl
    }
}
