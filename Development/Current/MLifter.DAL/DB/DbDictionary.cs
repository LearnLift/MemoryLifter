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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Security;
using MLifter.Generics;

namespace MLifter.DAL.DB
{
	/// <summary>
	/// Database implementation of IDictionary.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-13</remarks>
	public class DbDictionary : IDictionary
	{
		private BackgroundWorker m_BackgroundWorker = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbDictionary"/> class.
		/// </summary>
		/// <param name="lmid">The lmid.</param>
		/// <param name="user">The user.</param>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public DbDictionary(int lmid, IUser user)
		{
			id = lmid;
			parent = new ParentClass(user, this);
			DbMediaServer.DbMediaServer.Instance(parent).Start();
		}

		private IDbDictionaryConnector connector
		{
			get
			{
				switch (parent.CurrentUser.ConnectionString.Typ)
				{
					case DatabaseType.PostgreSQL:
						return MLifter.DAL.DB.PostgreSQL.PgSqlDictionaryConnector.GetInstance(parent);
					case DatabaseType.MsSqlCe:
						return MLifter.DAL.DB.MsSqlCe.MsSqlCeDictionaryConnector.GetInstance(parent);
					default:
						throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
				}
			}
		}

		private IDbCardStyleConnector cardstyleconnector
		{
			get
			{
				switch (parent.CurrentUser.ConnectionString.Typ)
				{
					case DatabaseType.PostgreSQL:
						return MLifter.DAL.DB.PostgreSQL.PgSqlCardStyleConnector.GetInstance(parent);
					case DatabaseType.MsSqlCe:
						return MsSqlCeCardStyleConnector.GetInstance(parent);
					default:
						throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
				}
			}
		}

		private IDbMediaConnector mediaconnector
		{
			get
			{
				switch (parent.CurrentUser.ConnectionString.Typ)
				{
					case DatabaseType.PostgreSQL:
						return MLifter.DAL.DB.PostgreSQL.PgSqlMediaConnector.GetInstance(parent);
					case DatabaseType.MsSqlCe:
						return MLifter.DAL.DB.MsSqlCe.MsSqlCeMediaConnector.GetInstance(parent);
					default:
						throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
				}
			}
		}

		private IDbMediaConnector GetMediaConnector(ParentClass customParent)
		{
			switch (customParent.CurrentUser.ConnectionString.Typ)
			{
				case DatabaseType.PostgreSQL:
					return MLifter.DAL.DB.PostgreSQL.PgSqlMediaConnector.GetInstance(customParent);
				case DatabaseType.MsSqlCe:
					return MLifter.DAL.DB.MsSqlCe.MsSqlCeMediaConnector.GetInstance(customParent);
				default:
					throw new UnsupportedDatabaseTypeException(customParent.CurrentUser.ConnectionString.Typ);
			}
		}

		private IDbExtensionConnector extensionconnector
		{
			get
			{
				switch (parent.CurrentUser.ConnectionString.Typ)
				{
					case DatabaseType.PostgreSQL:
						return MLifter.DAL.DB.PostgreSQL.PgSqlExtensionConnector.GetInstance(parent);
					case DatabaseType.MsSqlCe:
						return MLifter.DAL.DB.MsSqlCe.MsSqlCeExtensionConnector.GetInstance(parent);
					default:
						throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
				}
			}
		}

		internal FileCleanupQueue FileCleanupQueue = new FileCleanupQueue();
		
		#region IDictionary Members

		/// <summary>
		/// Gets a value indicating whether this instance is DB.
		/// </summary>
		/// <value><c>true</c> if this instance is DB; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-08-22</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public bool IsDB
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets or sets the background worker.
		/// </summary>
		/// <value>The background worker.</value>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public System.ComponentModel.BackgroundWorker BackgroundWorker
		{
			get
			{
				return m_BackgroundWorker;
			}
			set
			{
				m_BackgroundWorker = value;
			}
		}

		/// <summary>
		/// Occurs when [XML progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public event StatusMessageEventHandler XmlProgressChanged;

		/// <summary>
		/// Occurs when [move progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public event StatusMessageEventHandler MoveProgressChanged;

		/// <summary>
		/// Occurs when [save as progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public event StatusMessageEventHandler SaveAsProgressChanged;

		/// <summary>
		/// Occurs when [create media progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public event StatusMessageEventHandler CreateMediaProgressChanged;

		/// <summary>
		/// Gets the connection string (could conatain a path or a db connection string).
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev03, 2007-10-17</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Connection
		{
			get
			{
				//ToDo (done): Convert.ToString(Global.Properties[Property.ConnectionString])
				return Parent.CurrentUser.ConnectionString.ConnectionString; //return Environment.CurrentDirectory;
			}
		}

		/// <summary>
		/// Gets the dictionary as Xml.
		/// </summary>
		/// <value>The Xml.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Xml
		{
			get
			{
				//XmlDocument based approach

				XmlDocument document = new XmlDocument();
				XmlNode dictionary = document.CreateElement(string.Empty, "dictionary", string.Empty);

				foreach (IChapter chapter in Chapters.Chapters)
				{
					dictionary.AppendChild(document.ImportNode((XmlNode)(chapter as DbChapter).Chapter, true));
				}
				StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.XmlProgress, Cards.Count);
				foreach (ICard card in Cards.Cards)
				{
					ReportProgressUpdate(args);
					args.Progress++;
					dictionary.AppendChild(document.ImportNode((XmlNode)card.Card, true));
				}

				document.AppendChild(dictionary);

				return document.OuterXml;
			}
		}

		/// <summary>
		/// Sends the status message update.
		/// </summary>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <returns>
		/// [true] if the process should be canceled.
		/// </returns>
		/// <remarks>Documented by Dev03, 2008-08-20</remarks>
		private bool ReportProgressUpdate(StatusMessageEventArgs args)
		{
			switch (args.MessageType)
			{
				case StatusMessageType.XmlProgress:
					if (XmlProgressChanged != null) XmlProgressChanged(null, args);
					break;
				case StatusMessageType.MoveProgress:
					if (MoveProgressChanged != null) MoveProgressChanged(null, args);
					break;
				case StatusMessageType.SaveAsProgress:
					if (SaveAsProgressChanged != null) SaveAsProgressChanged(null, args);
					break;
				case StatusMessageType.CreateMediaProgress:
					if (CreateMediaProgressChanged != null) CreateMediaProgressChanged(null, args);
					break;
			}

			bool cancelProcess = false;
			if (m_BackgroundWorker != null)
			{
				if (m_BackgroundWorker.CancellationPending)
				{
					cancelProcess = true;
				}
				else
				{
					m_BackgroundWorker.ReportProgress(args.ProgressPercentage);
				}
			}
			return !cancelProcess;
		}

		/// <summary>
		/// Sends the status message update.
		/// </summary>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <param name="caller">The calling object.</param>
		/// <returns>
		/// [true] if the process should be canceled.
		/// </returns>
		/// <remarks>Documented by Dev03, 2008-08-20</remarks>
		private bool ReportProgressUpdate(StatusMessageEventArgs args, object caller)
		{
			switch (args.MessageType)
			{
				case StatusMessageType.CreateMediaProgress:
					if ((caller != null) && (caller is DbDictionary) && ((caller as DbDictionary).CreateMediaProgressChanged != null))
						(caller as DbDictionary).CreateMediaProgressChanged(null, args);
					break;
			}
			return true;
		}

		/// <summary>
		/// Defines whether the content of the LM is protected from being copied/extracted.
		/// </summary>
		public bool ContentProtected { get { return connector.GetContentProtected(id); } }

		/// <summary>
		/// Gets the number of boxes.
		/// </summary>
		/// <value>The number of boxes.</value>
		/// <remarks>Documented by Dev03, 2007-10-17</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public int NumberOfBoxes
		{
			get
			{
				return Boxes.Box.Count - 1;
			}
		}

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public int Version
		{
			get
			{
				//TODO: Find a better solution than simply converting (region settings/comma issues)
				return Convert.ToInt32(connector.GetDbVersion());
			}
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Title
		{
			get
			{
				return connector.GetTitle(Id);
			}
			set
			{
				connector.SetTitle(Id, value);
			}
		}

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Author
		{
			get
			{
				return connector.GetAuthor(Id);
			}
			set
			{
				connector.SetAuthor(Id, value);
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Description
		{
			get
			{
				return connector.GetDescription(Id);
			}
			set
			{
				connector.SetDescription(Id, value);
			}
		}

		private int id;
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>The ID.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public int Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Gets or sets the GUID.
		/// </summary>
		/// <value>The GUID.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Guid
		{
			get
			{
				return connector.GetGuid(Id);
			}
			set
			{
				connector.SetGuid(Id, value);
			}
		}

		/// <summary>
		/// Gets or sets the category.
		/// </summary>
		/// <value>The category.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public Category Category
		{
			get
			{
				return connector.GetCategoryId(id);
			}
			set
			{
				connector.SetCategory(id, value.Converted ? value.Id : MLifter.DAL.Category.ConvertCategoryId(value.Id));
			}
		}

		/// <summary>
		/// Gets or sets the media directory.
		/// </summary>
		/// <value>The media directory.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string MediaDirectory
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Gets the size of the dictionary.
		/// </summary>
		/// <value>The size of the dictionary.</value>
		/// <remarks>Documented by Dev08, 2008-10-02</remarks>
		public long DictionarySize
		{
			get
			{
				return connector.GetDictionarySize(id, 1024);
			}
		}

		/// <summary>
		/// Gets the number of all dictionary media objects/files.
		/// </summary>
		/// <value>The dictionary media objects count.</value>
		/// <remarks>Documented by Dev08, 2008-10-02</remarks>
		public int DictionaryMediaObjectsCount
		{
			get
			{
				return connector.GetDictionaryMediaObjectsCount(id);
			}
		}

		/// <summary>
		/// Gets actual the score.
		/// </summary>
		/// <value>The score.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public double Score
		{
			get
			{
				double value = connector.GetScore(id);
				return value < 0 || value == double.NaN ? 0 : value > 100 ? 100 : value;
			}
		}

		/// <summary>
		/// Gets or sets the high score.
		/// </summary>
		/// <value>The high score.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public double HighScore
		{
			get
			{
				return connector.GetHighscore(id);
			}
			set
			{
				connector.SetHighscore(id, value);
			}
		}

		/// <summary>
		/// Gets the boxes.
		/// </summary>
		/// <value>The boxes.</value>
		/// <remarks>Documented by Dev03, 2007-11-22</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public IBoxes Boxes
		{
			get
			{
				return new DbBoxes(Parent.GetChildParentClass(this));
			}
		}

		/// <summary>
		/// Gets or sets the cards.
		/// </summary>
		/// <value>The cards.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ICards Cards
		{
			get
			{
				return new DbCards(Id, parent.GetChildParentClass(this));
			}
		}

		/// <summary>
		/// Gets or sets the chapters.
		/// </summary>
		/// <value>The chapters.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public IChapters Chapters
		{
			get
			{
				return new DbChapters(Id, Parent.GetChildParentClass(this));
			}
		}

		/// <summary>
		/// Gets or sets the statistics.
		/// </summary>
		/// <value>The statistics.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public IStatistics Statistics
		{
			get
			{
				return new DbStatistics(id, Parent.GetChildParentClass(this));
			}
		}

		/// <summary>
		/// Loads this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void Load()
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void Save()
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Saves the dictionary to the new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void SaveAs(string newPath)
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Saves the dictionary to the new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void SaveAs(string newPath, bool overwrite)
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Moves the specified new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void Move(string newPath)
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Moves the specified new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void Move(string newPath, bool overwrite)
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Changes the media path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="move">if set to <c>true</c> [move].</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void ChangeMediaPath(string path, bool move)
		{
			Debug.WriteLine("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the resources.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public List<string> GetResources()
		{
			IList<int> mediaIds = mediaconnector.GetMediaResources(Id);
			List<string> mediaUris = new List<string>();
			foreach (int id in mediaIds)
				mediaUris.Add(MLifter.DAL.DB.DbMediaServer.DbMediaServer.Instance(parent).GetMediaURI(id).ToString());
			return mediaUris;
		}

		/// <summary>
		/// Gets the empty resources (media content with missing data).
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-31</remarks>
		public List<int> GetEmptyResources()
		{
			return mediaconnector.GetEmptyMediaResources(id);
		}

		/// <summary>
		/// Creates a new instance of a card style object.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ICardStyle CreateCardStyle()
		{
			if (!this.HasPermission(PermissionTypes.CanModifyStyles))
				throw new PermissionException();

			return new DbCardStyle(cardstyleconnector.CreateNewCardStyle(), Parent.GetChildParentClass(this));
		}

		/// <summary>
		/// Creates a new media object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-11</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			StatusMessageReportProgress rpu = new StatusMessageReportProgress(ReportProgressUpdate);
			return CreateNewMediaObject(this, rpu, type, path, isActive, isDefault, isExample);
		}

		/// <summary>
		/// Creates a new media object.
		/// </summary>
		/// <param name="caller">The calling object.</param>
		/// <param name="rpu">A delegate of the type StatusMessageReportProgress.</param>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-11</remarks>
		internal IMedia CreateNewMediaObject(object caller, StatusMessageReportProgress rpu, EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			IMedia media = null;
			Uri uri;

			if (!this.HasPermission(PermissionTypes.CanModifyMedia))
				throw new PermissionException();

			if (path == null)
				throw new ArgumentNullException("Null value not allowed for media file path!");

			try
			{
				if (File.Exists(Path.Combine(Environment.CurrentDirectory, path))) //to allow relative paths
					path = Path.Combine(Environment.CurrentDirectory, path);

				uri = new Uri(path);
			}
			catch (UriFormatException exception)
			{
				throw new FileNotFoundException("Uri format is invalid.", exception);
			}

			if (uri.Scheme == Uri.UriSchemeFile && uri.IsFile) //we got a new file
			{
				if (File.Exists(path))
				{
					int newid;
					using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
						newid = mediaconnector.CreateMedia(stream, type, rpu, caller);

					media = DbMedia.CreateDisconnectedCardMedia(newid, type, isDefault, isExample, parent);

					Helper.UpdateMediaProperties(path, newid, mediaconnector);
				}
				else
					throw new FileNotFoundException("Media file could not be found.", path);
			}
			else if (uri.Scheme == "http" && uri.IsLoopback) //we got a http reference => file is already in db
			{
				if (DbMediaServer.DbMediaServer.Instance(parent).IsYours(uri))
				{
					int mediaId = DbMediaServer.DbMediaServer.GetMediaID(uri.AbsolutePath);
					media = DbMedia.CreateDisconnectedCardMedia(mediaId, type, isDefault, isExample, parent);
					rpu(new StatusMessageEventArgs(StatusMessageType.CreateMediaProgress, 100, 100), caller);
				}
				else
				{
					DbMediaServer.DbMediaServer server = DbMediaServer.DbMediaServer.Instance(uri);

					int newid = mediaconnector.CreateMedia(GetMediaConnector(server.Parent).GetMediaStream(DbMediaServer.DbMediaServer.GetMediaID(uri.AbsolutePath)), type, rpu, caller);

					media = DbMedia.CreateDisconnectedCardMedia(newid, type, isDefault, isExample, parent);

					Helper.UpdateMediaProperties(path, newid, mediaconnector);
				}
			}

			return media;
		}

		/// <summary>
		/// Gets or sets the default settings.
		/// </summary>
		/// <value>The settings.</value>
		/// <remarks>Documented by Dev05, 2008-08-11</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ISettings DefaultSettings
		{
			get
			{
				return connector.GetDefaultSettings(Id);
			}
			set
			{
				if (!(value is DbSettings))
					return;

				connector.SetDefaultSettings(Id, ((DbSettings)value).Id);
			}
		}

		/// <summary>
		/// Gets or sets the user settings.
		/// </summary>
		/// <value>The user settings.</value>
		/// <remarks>Documented by Dev05, 2008-10-01</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ISettings UserSettings
		{
			get
			{
				return connector.GetUserSettings(id);
			}
			set
			{
				connector.SetUserSettings(id, ((DbSettings)value).Id);
			}
		}

		/// <summary>
		/// Gets or sets the allowed settings.
		/// </summary>
		/// <value>The allowed settings.</value>
		/// <remarks>Documented by Dev05, 2008-09-22</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ISettings AllowedSettings
		{
			get
			{
				return connector.GetAllowedSettings(id);
			}
			set
			{
				connector.SetAllowedSettings(id, ((DbSettings)value).Id);
			}
		}

		/// <summary>
		/// Creates the settings.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ISettings CreateSettings()
		{
			if (!this.HasPermission(PermissionTypes.CanModifySettings))
				throw new PermissionException();

			return connector.CreateSettings();
		}

		/// <summary>
		/// Creates the settings object.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ISettings CreateSettingsObject()
		{
			if (!this.HasPermission(PermissionTypes.CanModifySettings))
				throw new PermissionException();

			return connector.CreateSettings();
		}

		/// <summary>
		/// Resets the learning progress.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public IDictionary ResetLearningProgress()
		{
			if (parent.CurrentUser.ConnectionString.Typ != DatabaseType.MsSqlCe || parent.CurrentUser.ConnectionString.SyncType != SyncType.NotSynchronized)
			{
				Cards.ClearAllBoxes();
				HighScore = 0;
			}
			parent.CurrentUser.Cache.Clear();
			return Log.RestartLearningSuccess(parent);
		}

		/// <summary>
		/// Checks the user session.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public bool CheckUserSession()
		{
			return connector.CheckUserSession();
		}

		/// <summary>
		/// Preloads the card cache.
		/// </summary>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		public void PreloadCardCache()
		{
			connector.PreloadCardCache(id);
		}

		/// <summary>
		/// Clears the unused media.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-27</remarks>
		public void ClearUnusedMedia()
		{
			connector.ClearUnusedMedia(id);
		}

		/// <summary>
		/// Occurs when [backup completed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public event BackupCompletedEventHandler BackupCompleted
		{
			add { /* throw new NotSupportedException(); */ }
			remove { }
		}

		/// <summary>
		/// Gets the LearningModule extensions.
		/// </summary>
		/// <value>The extensions.</value>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public IList<IExtension> Extensions
		{
			get
			{
				ObservableList<IExtension> extensions = new ObservableList<IExtension>();
				IList<Guid> ExtensionGuids = connector.GetExtensions(id);

				foreach (Guid guid in ExtensionGuids)
					extensions.Add(new DbExtension(guid, parent));

				extensions.ListChanged += new EventHandler<ObservableListChangedEventArgs<IExtension>>(extensions_ListChanged);

				return extensions;
			}
		}

		/// <summary>
		/// Handles the ListChanged event of the extensions control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The MLifter.Generics.ObservableListChangedEventArgs&lt;MLifter.DAL.Interfaces.IExtension&gt; instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-03</remarks>
		void extensions_ListChanged(object sender, ObservableListChangedEventArgs<IExtension> e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					extensionconnector.SetExtensionLM(e.Item.Id, this.Id);
					break;
				case ListChangedType.ItemDeleted:
					extensionconnector.DeleteExtension(e.Item.Id);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Creates a new extension.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		public IExtension ExtensionFactory()
		{
			Guid extensionGuid = extensionconnector.AddNewExtension();
			extensionconnector.SetExtensionLM(extensionGuid, this.Id);
			return new DbExtension(extensionGuid, parent);
		}

		/// <summary>
		/// Creates new extensions.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		public IExtension ExtensionFactory(Guid guid)
		{
			Guid extensionGuid = extensionconnector.AddNewExtension(guid);
			extensionconnector.SetExtensionLM(extensionGuid, this.Id);
			return new DbExtension(extensionGuid, parent);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void Dispose()
		{
			if (DbMediaServer.DbMediaServer.Instance(parent).IsAlive)
				DbMediaServer.DbMediaServer.Instance(parent).Stop();

			if (Parent != null)
				Parent.OnDictionaryClosed(this, EventArgs.Empty);

			if (Parent.CurrentUser.ConnectionString.Typ == DatabaseType.MsSqlCe)
				MSSQLCEConn.CloseMyConnection(this.Connection);
			//MSSQLCEConn.CloseAllConnections();

			FileCleanupQueue.DoCleanup();
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
			CopyBase.Copy(this, target, typeof(IDictionary), progressDelegate);

			//copy extensions
			foreach (IExtension extension in Extensions)
			{
				IExtension newExtension = ((IDictionary)target).ExtensionFactory(extension.Id);
				extension.CopyTo(newExtension, progressDelegate);
			}
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ParentClass Parent { get { return parent; } }

		#endregion

		#region ISecurity Members

		/// <summary>
		/// Determines whether the object has the specified permission.
		/// </summary>
		/// <param name="permissionName">Name of the permission.</param>
		/// <returns>
		///   <c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
		/// </returns>
		public bool HasPermission(string permissionName)
		{
			return Parent.CurrentUser.HasPermission(this, permissionName);
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>
		/// A list of permissions for the object.
		/// </returns>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			return Parent.CurrentUser.GetPermissions(this);
		}

		#endregion
	}

	/// <summary>
	/// The requested operation is not possible in a database learning module.
	/// </summary>
	public class NotAllowedInDbModeException : Exception { }
	/// <summary>
	/// The requested operation is not possible on a synced learning module.
	/// </summary>
	public class NotAllowedInSyncedModeException : Exception { }
}
