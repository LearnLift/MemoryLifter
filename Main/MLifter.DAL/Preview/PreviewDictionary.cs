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
using System.Diagnostics;
using System.IO;
using MLifter.DAL.Security;

namespace MLifter.DAL.Preview
{
	/// <summary>
	/// This is a container object which is used for a preview dictionary.
	/// It does not implement any data persistence!
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-12-03</remarks>
	public class PreviewDictionary : IDictionary
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PreviewDictionary"/> class.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public PreviewDictionary(IUser user)
		{
			parent = new ParentClass(user, this);
			cards = new PreviewCards(parent.GetChildParentClass(this));
			userSettings = new PreviewSettings(parent.GetChildParentClass(this));
			allowedSettings = new PreviewSettings(parent.GetChildParentClass(this));
			defaultSettings = new PreviewSettings(parent.GetChildParentClass(this));
			statistics = new PreviewStatistics();
		}

		private ICards cards;

		#region IDictionary Members

		/// <summary>
		/// Gets a value indicating whether this instance is DB.
		/// </summary>
		/// <value><c>true</c> if this instance is DB; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-08-22</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool IsDB
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// If !string.emty --&gt; Can open LM --&gt; else --&gt; Can't open LM.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public string Password
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the background worker.
		/// </summary>
		/// <value>The background worker.</value>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public System.ComponentModel.BackgroundWorker BackgroundWorker
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
		/// Occurs when [XML progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event MLifter.DAL.Tools.StatusMessageEventHandler XmlProgressChanged
		{
			add
			{
				throw new Exception("The method or operation is not implemented.");
			}
			remove
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Occurs when [move progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event MLifter.DAL.Tools.StatusMessageEventHandler MoveProgressChanged
		{
			add
			{
				throw new Exception("The method or operation is not implemented.");
			}
			remove
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Occurs when [save as progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event MLifter.DAL.Tools.StatusMessageEventHandler SaveAsProgressChanged
		{
			add
			{
				throw new Exception("The method or operation is not implemented.");
			}
			remove
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Occurs when [create media progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event MLifter.DAL.Tools.StatusMessageEventHandler CreateMediaProgressChanged
		{
			add
			{
				throw new Exception("The method or operation is not implemented.");
			}
			remove
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		private ISettings defaultSettings = null;
		/// <summary>
		/// Gets or sets the default settings.
		/// </summary>
		/// <value>The settings.</value>
		/// <remarks>Documented by Dev05, 2008-08-11</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings DefaultSettings
		{
			get
			{
				return defaultSettings;
			}
			set
			{
				defaultSettings = value;
			}
		}

		private ISettings allowedSettings = null;
		/// <summary>
		/// Gets or sets the allowed settings.
		/// </summary>
		/// <value>The allowed settings.</value>
		/// <remarks>Documented by Dev05, 2008-09-22</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings AllowedSettings
		{
			get
			{
				return allowedSettings;
			}
			set
			{
				allowedSettings = value;
			}
		}

		private ISettings userSettings = null;
		/// <summary>
		/// Gets or sets the user settings.
		/// </summary>
		/// <value>The user settings.</value>
		/// <remarks>Documented by Dev05, 2008-10-01</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings UserSettings
		{
			get
			{
				return userSettings;
			}
			set
			{
				userSettings = value;
			}
		}

		/// <summary>
		/// Creates the settings.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings CreateSettings()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Defines whether the content of the LM is protected from being copied/extracted.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool ContentProtected
		{
			get { return false; }
		}


		private string connection;
		/// <summary>
		/// Gets the connection string (could conatain a path or a db connection string).
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev03, 2007-10-17</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Connection
		{
			get { return connection; }
			internal set { connection = value; }
		}

		/// <summary>
		/// Gets the dictionary as Xml.
		/// </summary>
		/// <value>The Xml.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Xml
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the number of boxes.
		/// </summary>
		/// <value>The number of boxes.</value>
		/// <remarks>Documented by Dev03, 2007-10-17</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int NumberOfBoxes
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int Version
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		private string title = string.Empty;
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Title
		{
			get
			{
				if (title != string.Empty)
					return title;

				if (File.Exists(Connection))
					return System.IO.Path.GetFileNameWithoutExtension(Connection);
				else
					return string.Empty;
			}
			set
			{
				title = value;
			}
		}

		private string author = String.Empty;
		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Author
		{
			get
			{
				return author;
			}
			set
			{
				author = value;
			}
		}

		private string description = String.Empty;
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		private int id = 0;
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>The ID.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int Id
		{
			get
			{
				return id;
			}
			internal set
			{
				id = value;
			}
		}

		/// <summary>
		/// Gets or sets the GUID.
		/// </summary>
		/// <value>The GUID.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Guid
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

		private Category category = null;
		/// <summary>
		/// Gets or sets the category.
		/// </summary>
		/// <value>The category.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public Category Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		private string mediaDirectory = string.Empty;
		/// <summary>
		/// Gets or sets the media directory.
		/// </summary>
		/// <value>The media directory.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string MediaDirectory
		{
			get
			{
				return mediaDirectory;
			}
			set
			{
				mediaDirectory = value;
			}
		}

		/// <summary>
		/// Gets the size of the dictionary.
		/// </summary>
		/// <value>The size of the dictionary.</value>
		/// <remarks>Documented by Dev08, 2008-10-02</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public long DictionarySize
		{
			get { return (new FileInfo(parent.CurrentUser.ConnectionString.ConnectionString)).Length; }
		}

		/// <summary>
		/// Gets the number of all dictionary media objects/files.
		/// </summary>
		/// <value>The dictionary media objects count.</value>
		/// <remarks>Documented by Dev08, 2008-10-02</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int DictionaryMediaObjectsCount
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets actual the score.
		/// </summary>
		/// <value>
		/// The score.
		/// </value>
		public double Score
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets or sets the high score.
		/// </summary>
		/// <value>The high score.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public double HighScore
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
		/// Gets the boxes.
		/// </summary>
		/// <value>The boxes.</value>
		/// <remarks>Documented by Dev03, 2007-11-22</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IBoxes Boxes
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets or sets the cards.
		/// </summary>
		/// <value>The cards.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ICards Cards
		{
			get { return cards; }
		}

		/// <summary>
		/// Gets or sets the chapters.
		/// </summary>
		/// <value>The chapters.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IChapters Chapters
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		private IStatistics statistics = null;
		/// <summary>
		/// Gets or sets the statistics.
		/// </summary>
		/// <value>The statistics.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IStatistics Statistics
		{
			get { return statistics; }
		}

		/// <summary>
		/// Loads this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void Load()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void Save()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Saves the dictionary to the new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void SaveAs(string newPath)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Saves the dictionary to the new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void SaveAs(string newPath, bool overwrite)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Moves the specified new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void Move(string newPath)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Moves the specified new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void Move(string newPath, bool overwrite)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Changes the media path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="move">if set to <c>true</c> [move].</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void ChangeMediaPath(string path, bool move)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the resources.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public List<string> GetResources()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the empty resources.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-31</remarks>
		public List<int> GetEmptyResources()
		{
			return null;
		}

		/// <summary>
		/// Creates a new instance of a card style object.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ICardStyle CreateCardStyle()
		{
			throw new Exception("The method or operation is not implemented.");
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
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Resets the learning progress.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IDictionary ResetLearningProgress()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Checks the user session.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool CheckUserSession()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Preloads the card cache.
		/// </summary>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		public void PreloadCardCache()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Clears the unused media.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-27</remarks>
		public void ClearUnusedMedia()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Occurs when [backup completed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event BackupCompletedEventHandler BackupCompleted
		{
			add
			{
				throw new Exception("The method or operation is not implemented.");
			}
			remove
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets the LearningModule extensions.
		/// </summary>
		/// <value>The extensions.</value>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public IList<IExtension> Extensions
		{
			get { throw new NotSupportedException(); }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void Dispose()
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get { return parent; } }

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
			if (permissionName == PermissionTypes.Visible)
				return true;

			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return true;

			return false;
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

		#region IDictionary Members


		/// <summary>
		/// Creates a new extension.
		/// </summary>
		/// <returns></returns>
		public IExtension ExtensionFactory()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates new extensions.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public IExtension ExtensionFactory(Guid guid)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
