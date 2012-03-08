using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Properties;

namespace MLifter.DAL
{
	/// <summary>
	/// This file declares the specific exceptions that may be thrown by the MLifter.DAL.
	/// </summary>
	/// <remarks>Documented by Dev01, 2007-08-03</remarks>
	public class IdOverflowException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IdOverflowException"/> class.
		/// </summary>
		public IdOverflowException()
			: base(String.Format(Resources.EXCEPTION_ID_OVERFLOW, int.MaxValue))
		{
		}
	}

	/// <summary>
	/// Exception is thrown if you try to add a card with an already used ID.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class IdExistsException : Exception
	{
		private int _failed_id;

		/// <summary>
		/// Initializes a new instance of the <see cref="IdExistsException"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public IdExistsException(int id)
			: base(String.Format(Resources.EXCEPTION_ID_EXISTS, id))
		{
			_failed_id = id;
		}

		/// <summary>
		/// Gets or sets the failed id.
		/// </summary>
		/// <value>
		/// The failed id.
		/// </value>
		public int FailedId
		{
			get { return _failed_id; }
			set { _failed_id = value; }
		}
	}
	/// <summary>
	/// Exception is thrown if the card/chapter for the given ID could not be found.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class IdAccessException : Exception
	{
		private int _failed_id;

		/// <summary>
		/// Initializes a new instance of the <see cref="IdAccessException"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public IdAccessException(int id)
			: base(String.Format(Resources.EXCEPTION_ID_ACCESS, id))
		{
			_failed_id = id;
		}

		/// <summary>
		/// Gets or sets the failed id.
		/// </summary>
		/// <value>
		/// The failed id.
		/// </value>
		public int FailedId
		{
			get { return _failed_id; }
			set { _failed_id = value; }
		}
	}

	/// <summary>
	/// Exception is thrown if an invalid box id is used.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class InvalidBoxException : Exception
	{
		private int _failed_id;

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBoxException"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public InvalidBoxException(int id)
			: base(String.Format(Resources.EXCEPTION_INVALID_BOX, id))
		{
			_failed_id = id;
		}

		/// <summary>
		/// Gets or sets the failed id.
		/// </summary>
		/// <value>
		/// The failed id.
		/// </value>
		public int FailedId
		{
			get { return _failed_id; }
			set { _failed_id = value; }
		}
	}

	/// <summary>
	/// Exception is thrown if an unknown media file extension was given.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class UnknowMediaException : Exception
	{
		private string _ext;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnknowMediaException"/> class.
		/// </summary>
		/// <param name="ext">The ext.</param>
		public UnknowMediaException(string ext)
			: base(String.Format(Resources.EXCEPTION_UNKNOWN_MEDIA, ext))
		{
			_ext = ext;
		}

		/// <summary>
		/// Gets the unknown extension.
		/// </summary>
		public string UnknownExtension
		{
			get { return _ext; }
		}
	}

	/// <summary>
	/// Exception is thrown if a dictionary already exists at the given path.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class DictionaryPathExistsException : Exception
	{
		private string filename;

		internal DictionaryPathExistsException(string path)
			: base(String.Format(Resources.EXCEPTION_DICTIONARY_PATH_EXISTS, path))
		{
			filename = path;
		}

		/// <summary>
		/// Gets the filename.
		/// </summary>
		public string Filename
		{
			get { return filename; }
		}
	}

	/// <summary>
	/// Exception is thrown if the dictionary is invalid or damaged.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class InvalidImportFormatException : Exception
	{
		private string filename;

		internal InvalidImportFormatException(string path)
			: base(String.Format(Resources.EXCEPTION_INVALID_IMPORT_FORMAT, path))
		{
			filename = path;
		}

		/// <summary>
		/// Gets the filename.
		/// </summary>
		public string Filename
		{
			get { return filename; }
		}
	}

	/// <summary>
	/// Exception is thrown if no valid filename for a media file could be found.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class NoValidMediaFileName : Exception
	{
		private string filename;

		internal NoValidMediaFileName()
			: base(String.Format(Resources.EXCEPTION_NO_VALID_MEDIA_FILENAME, String.Empty))
		{
		}

		internal NoValidMediaFileName(string filename)
			: base(String.Format(Resources.EXCEPTION_NO_VALID_MEDIA_FILENAME, filename))
		{
			this.filename = filename;
		}

		/// <summary>
		/// Gets the filename.
		/// </summary>
		public string Filename
		{
			get { return filename; }
		}
	}

	/// <summary>
	/// Exception is thrown if the dictionary version is not supported.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-09-07</remarks>
	public class DictionaryFormatNotSupported : Exception
	{
		private int version;

		internal DictionaryFormatNotSupported(int version)
			: base(String.Format(Resources.EXCEPTION_DICTIONARY_FORMAT_NOT_SUPPORTED, version))
		{
			this.version = version;
		}

		/// <summary>
		/// Gets the version.
		/// </summary>
		public int Version
		{
			get { return version; }
		}
	}

	/// <summary>
	/// Exception is thrown in case the dictionary could not be decrypted.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-09-22</remarks>
	public class DictionaryNotDecryptedException : Exception
	{
		internal DictionaryNotDecryptedException()
			: base(Resources.EXCEPTION_NOT_DECRYPTED)
		{ }
	}

	/// <summary>
	/// Exception is thrown if the dictionary has been generated by an older version.
	/// </summary>
	/// <remarks>Documented by Dev02, 2007-12-12</remarks>
	public class DictionaryFormatOldVersion : Exception
	{
		private int version;

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryFormatOldVersion"/> class.
		/// </summary>
		/// <param name="version">The version.</param>
		internal DictionaryFormatOldVersion(int version)
			: base(String.Format(Resources.EXCEPTION_DICTIONARY_FORMAT_OLD_VERSION, version))
		{
			this.version = version;
		}

		/// <summary>
		/// Gets the version.
		/// </summary>
		public int Version
		{
			get { return version; }
		}
	}

	/// <summary>
	/// Thrown whne the archiv has no valid header.
	/// </summary>
	public class NoValidArchiveHeaderException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValidArchiveHeaderException"/> class.
		/// </summary>
		public NoValidArchiveHeaderException()
			: base(String.Format(Resources.EXCEPTION_NO_VALID_ARCHIVE_HEADER, int.MaxValue))
		{
		}
	}

	/// <summary>
	/// Thrown when the archive is invalid.
	/// </summary>
	public class NoValidArchiveException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValidArchiveException"/> class.
		/// </summary>
		public NoValidArchiveException()
			: base(String.Format(Resources.EXCEPTION_NO_VALID_ARCHIVE, int.MaxValue))
		{
		}
	}

	/// <summary>
	/// Thrown when the dictionary is invalid.
	/// </summary>
	public class InvalidDictionaryException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDictionaryException"/> class.
		/// </summary>
		public InvalidDictionaryException()
			: base(Resources.EXCEPTION_INVALID_DICTIONARY)
		{
		}
	}

	/// <summary>
	/// Exception that is thrown when a connection prefix is unknown.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-07-25</remarks>
	public class UnknownConnectionPrefixException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnknownConnectionPrefixException"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public UnknownConnectionPrefixException(string connectionString)
			: base(string.Format(Resources.EXCEPTION_UNKNOWN_CONNECTIONPREFIX, connectionString))
		{
		}
	}

	/// <summary>
	/// Exception that is thrown when the database type is not supported.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-07-25</remarks>
	public class UnsupportedDatabaseTypeException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnsupportedDatabaseTypeException"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public UnsupportedDatabaseTypeException(DatabaseType type)
			: base(string.Format(Resources.EXCEPTION_UNSUPPORTED_DATABASETYPE, type.ToString()))
		{
		}
	}

	/// <summary>
	/// Exception that is thrown when an error happens during database access.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-07-28</remarks>
	public class DatabaseAccessException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseAccessException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DatabaseAccessException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Exception that is thrown when the selected file is a Dip file which is not
	/// supported.
	/// </summary>
	/// <remarks>Documented by Dev10, 2009-26-02</remarks>
	public class DipNotSupportedException : Exception { }

	/// <summary>
	/// Excpetion is thrown if a connection to the database is supported by this data layer.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-09</remarks>
	public class DatabaseVersionNotSupported : Exception
	{
		/// <summary>
		/// List with informanion on supported data layer versions.
		/// </summary>
		public List<MLifter.DAL.Interfaces.DataLayerVersionInfo> VersionInfo { get; set; }
		/// <summary>
		/// The database version.
		/// </summary>
		public Version DatabaseVersion { get; set; }
		/// <summary>
		/// Gets or sets the dal version.
		/// </summary>
		/// <value>The dal version.</value>
		/// <remarks>Documented by Dev05, 2009-07-10</remarks>
		public Version DalVersion { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [silent upgrade].
		/// </summary>
		/// <value><c>true</c> if [silent upgrade]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2009-07-03</remarks>
		public bool SilentUpgrade { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this database can be updated.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this database can be updated; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev05, 2010-02-01</remarks>
		public bool CanUpdate { get; set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseVersionNotSupported"/> class.
		/// </summary>
		/// <param name="versionInfo">The version info.</param>
		/// <param name="databaseVersion">The database version.</param>
		/// <param name="dalVersion">The dal version.</param>
		/// <remarks>Documented by Dev02, 2009-07-03</remarks>
		public DatabaseVersionNotSupported(List<MLifter.DAL.Interfaces.DataLayerVersionInfo> versionInfo, Version databaseVersion, Version dalVersion)
			: this(versionInfo, databaseVersion, dalVersion, false) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseVersionNotSupported"/> class.
		/// </summary>
		/// <param name="versionInfo">The version info.</param>
		/// <param name="databaseVersion">The database version.</param>
		/// <param name="dalVersion">The dal version.</param>
		/// <param name="silentUpgrade">if set to <c>true</c> [silent upgrade].</param>
		/// <remarks>Documented by Dev03, 2009-04-30</remarks>
		public DatabaseVersionNotSupported(List<MLifter.DAL.Interfaces.DataLayerVersionInfo> versionInfo, Version databaseVersion, Version dalVersion, bool silentUpgrade)
		{
			CanUpdate = false;
			VersionInfo = versionInfo;
			DatabaseVersion = databaseVersion;
			SilentUpgrade = silentUpgrade;
			DalVersion = dalVersion;
		}
	}

	/// <summary>
	/// Exception is thrown if a new learning session could not be created (usually only occurs for PGSQL).
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-02-12</remarks>
	public class StartLearningSessionException : Exception { }
}
