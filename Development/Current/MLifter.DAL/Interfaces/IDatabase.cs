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
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// Offers methods and properties related to the database environment.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-09</remarks>
	public interface IDatabase : IParent
	{
		/// <summary>
		/// Gets the database version.
		/// </summary>
		/// <value>The database version.</value>
		/// <remarks>Documented by Dev03, 2008-09-09</remarks>
		Version DatabaseVersion { get; }
		/// <summary>
		/// Gets the supported data layer versions.
		/// </summary>
		/// <value>The supported data layer versions.</value>
		/// <remarks>Documented by Dev03, 2008-09-09</remarks>
		List<DataLayerVersionInfo> SupportedDataLayerVersions { get; }
		/// <summary>
		/// Checks if a connection to the database is supported by this data layer. Throws an exception of DatabaseVersionNotSupported if not.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-09-09</remarks>
		void CheckIfDatabaseVersionSupported();
		/// <summary>
		/// Upgrades the database to the current version.
		/// </summary>
		/// <param name="currentVersion">The current version of the database.</param>
		/// <returns>[true] if success.</returns>
		/// <remarks>Documented by Dev03, 2009-04-30</remarks>
		bool UpgradeDatabase(Version currentVersion);
	}

	/// <summary>
	/// The verison info struct.
	/// </summary>
	public struct DataLayerVersionInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataLayerVersionInfo"/> struct.
		/// </summary>
		/// <param name="version">The version.</param>
		public DataLayerVersionInfo(Version version)
		{
			Type = VersionType.Equal;
			Version = version;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataLayerVersionInfo"/> struct.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="version">The version.</param>
		public DataLayerVersionInfo(VersionType type, Version version)
		{
			Type = type;
			Version = version;
		}

		/// <summary>
		/// The type
		/// </summary>
		public VersionType Type;
		/// <summary>
		/// The version
		/// </summary>
		public Version Version;
	}

	/// <summary>
	/// The type of the version
	/// </summary>
	public enum VersionType
	{
		/// <summary>
		/// 
		/// </summary>
		LowerBound,
		/// <summary>
		/// 
		/// </summary>
		UpperBound,
		/// <summary>
		/// 
		/// </summary>
		Equal
	}
}
