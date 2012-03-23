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
