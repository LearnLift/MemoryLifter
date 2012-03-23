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
using System.IO;
using System.Text;
using System.Xml;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Preview;
using System.Diagnostics;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// XML implementation of IDictionaries
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-12-02</remarks>
	public class XmlDictionaries : IDictionaries
	{
		private string m_learningModulesFolder;
		List<IDictionary> m_learningModules = new List<IDictionary>();

		/// <summary>
		/// Gets or sets the learning modules folder.
		/// </summary>
		/// <value>The learning modules folder.</value>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		public string LearningModulesFolder
		{
			get { return m_learningModulesFolder; }
			set { m_learningModulesFolder = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDictionaries"/> class.
		/// </summary>
		/// <param name="learningModulesFolder">The learning modules folder.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		internal XmlDictionaries(string learningModulesFolder, ParentClass parent)
		{
			m_learningModulesFolder = learningModulesFolder;
			this.parent = parent;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDictionaries"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public XmlDictionaries(string path){
			m_learningModulesFolder = Path.GetDirectoryName(path);
		}
		private void LoadLearningModules()
		{
			int counter = 1;
			m_learningModules.Clear();
			if (!Directory.Exists(m_learningModulesFolder))
				return;

			//string[] dicPaths = Directory.GetFiles(m_learningModulesFolder, "*" + Helper.OdxExtension, SearchOption.AllDirectories);
			string[] dicPaths = Helper.GetFilesRecursive(m_learningModulesFolder, "*" + Helper.OdxExtension);

			foreach (string path in dicPaths)
			{
				PreviewDictionary dic = User.GetExtendedPreviewDictionary(path, parent.CurrentUser);
				dic.Id = counter++;
				m_learningModules.Add(dic);
			}
		}

		#region IDictionaries Members

		/// <summary>
		/// Gets all learning modules.
		/// </summary>
		/// <value>The dictionaries.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		public IList<IDictionary> Dictionaries
		{
			get
			{
				LoadLearningModules();
				return m_learningModules;
			}
		}

		/// <summary>
		/// Gets the specified learning module.
		/// </summary>
		/// <param name="id">The id of the learning module.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		public IDictionary Get(int id)
		{
			return m_learningModules.Find(delegate(IDictionary dic) { return dic.Id == id; });
		}

		/// <summary>
		/// Deletes the specified id.
		/// </summary>
		/// <param name="css">The connection string struct</param>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		/// <remarks>Documented by Dev08, 2008-12-09</remarks>
		public void Delete(ConnectionStringStruct css)
		{
			foreach (IDictionary lmDelete in Dictionaries)
			{
				if (lmDelete.Connection != css.ConnectionString)
					continue;

				//Delete the LM
				if (File.Exists(lmDelete.Connection))
				{
					try
					{
						Directory.Delete(Path.Combine(Path.GetDirectoryName(lmDelete.Connection), lmDelete.MediaDirectory), true);
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Can't delete LM media folder - " + lmDelete.MediaDirectory + "! " + ex.Message);
					}
					string filename = lmDelete.Connection;
					try
					{
						File.Delete(filename);
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Can't delete LM - " + filename + "! " + ex.Message);
					}
				}
				break;
			}
		}

		/// <summary>
		/// Adds a new learning module.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="title">The title.</param>
		/// <returns>The new learning module.</returns>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		public IDictionary AddNew(int categoryId, string title)
		{
			string path = Path.Combine(LearningModulesFolder, Path.Combine(title, title + Helper.OdxExtension));
			IDictionary newDic=null;
			if (Parent == null)
			{
				ParentClass dummyParent = new ParentClass(null, this);
				newDic = new XmlDictionary(path, false, dummyParent.CurrentUser);
			}
			else
			{
				newDic = new XmlDictionary(path, false, Parent.CurrentUser);
			}
			newDic.Category = new Category(categoryId);
			return newDic;
		}

		/// <summary>
		/// Adds a new learning module.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="title">The title.</param>
		/// <param name="licenceKey">The licence key.</param>
		/// <param name="contentProtected">if set to <c>true</c> the content is protected.</param>
		/// <param name="calCount">The cal count.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2009-02-12
		/// </remarks>
		public IDictionary AddNew(int categoryId, string title, string licenceKey, bool contentProtected, int calCount)
		{
			throw new NotImplementedException("Only in DB mode possible!");
		}

		/// <summary>
		/// Gets the count of learning modules.
		/// </summary>
		/// <value>The count.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		public int Count
		{
			get { return Dictionaries.Count; }
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
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Creates new extensions.
		/// </summary>
		/// <returns></returns>
		public IExtension ExtensionFactory()
		{
			ExtensionFactory(-1);
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the Extensions factory.
		/// </summary>
		/// <param name="lmId">The lm id.</param>
		/// <returns></returns>
		public IExtension ExtensionFactory(int lmId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes the extension.
		/// </summary>
		/// <param name="extension">The extension.</param>
		public void DeleteExtension(IExtension extension)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2008-12-02</remarks>
		public ParentClass Parent
		{
			get { return parent; }
		}

		#endregion

		#region ISecurity members

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
			return true;
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			return new List<SecurityFramework.PermissionInfo>();
		}

		#endregion

	}
}
