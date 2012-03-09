using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;
using MLifter.DAL.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL
{
	/// <summary>
	/// UNC implementation of IDictionaries.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-15</remarks>
	public class UncDictionaries : IDictionaries
	{
		IDictionaries xmlDictionaries;
		IDictionaries dbDictionaries;

		internal UncDictionaries(IDictionaries xmlDics, IDictionaries dbDics)
		{
			xmlDictionaries = xmlDics;
			dbDictionaries = dbDics;
		}

		#region IDictionaries Members

		/// <summary>
		/// Gets all learning modules.
		/// </summary>
		/// <value>The dictionaries.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public IList<IDictionary> Dictionaries
		{
			get
			{
				List<IDictionary> dics = new List<IDictionary>();
				(xmlDictionaries.Dictionaries as List<IDictionary>).ForEach(delegate(IDictionary dic) { dics.Add(dic); });
				(dbDictionaries.Dictionaries as List<IDictionary>).ForEach(delegate(IDictionary dic) { dics.Add(dic); });
				return dics;
			}
		}

		/// <summary>
		/// Gets the specified learning module.
		/// </summary>
		/// <param name="id">The id of the learning module.</param>
		/// <returns></returns>
		public IDictionary Get(int id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes a specific LM.
		/// </summary>
		/// <param name="css">The connection string struct.</param>
		public void Delete(ConnectionStringStruct css)
		{
			switch (css.Typ)
			{
				case DatabaseType.Xml:
					xmlDictionaries.Delete(css);
					break;
				case DatabaseType.PostgreSQL:
					throw new NotImplementedException();
				case DatabaseType.Unc:
					throw new NotImplementedException();
				case DatabaseType.MsSqlCe:
					dbDictionaries.Delete(css);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Adds a new learning module.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="title">The title.</param>
		/// <returns>The new learning module.</returns>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev05, 2009-02-12</remarks>
		public IDictionary AddNew(int categoryId, string title)
		{
			return dbDictionaries.AddNew(categoryId, title);
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
		/// <value>
		/// The count.
		/// </value>
		public int Count
		{
			get { return xmlDictionaries.Count + dbDictionaries.Count; }
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
		/// Get the Extensions factory.
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

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent
		{
			get { throw new NotImplementedException(); }
		}

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
			return true;
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>
		/// A list of permissions for the object.
		/// </returns>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
