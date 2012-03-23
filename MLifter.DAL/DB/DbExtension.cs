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
using MLifter.Generics;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.ComponentModel;
using System.Collections;

namespace MLifter.DAL.DB
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-07-02</remarks>
	public class DbExtension : IExtension
	{
		private Interfaces.DB.IDbExtensionConnector connector
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

		/// <summary>
		/// Initializes a new instance of the <see cref="DbChapters"/> class.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>
		/// Documented by AAB, 13.01.2009.
		/// </remarks>
		public DbExtension(Guid guid, ParentClass parent)
		{
			this.parent = parent;
			this.guid = guid;
		}

		private Guid guid;

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

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(IExtension), progressDelegate);

			//copy data stream
			using (Stream data = this.Data)
			{
				if (data != null)
					((IExtension)target).Data = data;
			}

			//copy actions
			IList<ExtensionAction> actions = ((IExtension)target).Actions;
			actions.Clear();
			foreach (ExtensionAction action in this.Actions)
				actions.Add(action);
		}

		#endregion

		#region IExtension Members

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public Guid Id
		{
			get
			{
				return guid;
			}
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name
		{
			get
			{
				return connector.GetExtensionName(guid);
			}
			set
			{
				connector.SetExtensionName(guid, value);
			}
		}

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public Version Version
		{
			get
			{
				return connector.GetExtensionVersion(guid);
			}
			set
			{
				connector.SetExtensionVersion(guid, value);
			}
		}

		/// <summary>
		/// Gets or sets the type of the extension.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public ExtensionType Type
		{
			get
			{
				return connector.GetExtensionType(guid);
			}
			set
			{
				connector.SetExtensionType(guid, value);
			}
		}

		/// <summary>
		/// Gets or sets the start file.
		/// </summary>
		/// <value>
		/// The start file.
		/// </value>
		public string StartFile
		{
			get
			{
				return connector.GetExtensionStartFile(guid);
			}
			set
			{
				connector.SetExtensionStartFile(guid, value);
			}
		}

		/// <summary>
		/// Gets or sets the data stream.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public Stream Data
		{
			get
			{
				return connector.GetExtensionStream(guid);
			}
			set
			{
				connector.SetExtensionStream(guid, value);
			}
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>
		/// The actions.
		/// </value>
		public IList<ExtensionAction> Actions
		{
			get
			{
				ObservableList<ExtensionAction> extensionActions = new ObservableList<ExtensionAction>();
				extensionActions.AddRange(connector.GetExtensionActions(guid));

				extensionActions.ListChanged += new EventHandler<ObservableListChangedEventArgs<ExtensionAction>>(extensionActions_ListChanged);
				return extensionActions;
			}
		}

		/// <summary>
		/// Handles the ListChanged event of the extensionActions control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The MLifter.Generics.ObservableListChangedEventArgs&lt;MLifter.DAL.Interfaces.ExtensionAction&gt; instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-07-06</remarks>
		private void extensionActions_ListChanged(object sender, ObservableListChangedEventArgs<ExtensionAction> e)
		{
			IList<ExtensionAction> eaList = connector.GetExtensionActions(guid);

			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					eaList.Add(e.Item);
					connector.SetExtensionActions(guid, eaList);
					break;
				case ListChangedType.ItemDeleted:
					eaList.Remove(e.Item);
					connector.SetExtensionActions(guid, eaList);
					break;
				case ListChangedType.Reset:
					IList<ExtensionAction> oldList = (IList<ExtensionAction>)sender;
					connector.SetExtensionActions(guid, oldList);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Extracts the data.
		/// </summary>
		/// <param name="targetPath">The target path.</param>
		public void ExtractData(string targetPath)
		{
			using (Stream data = this.Data)
			{
				if (data != null)
					Tools.ZipHelper.ExtractZipStream(data, targetPath);
			}
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0} ({1})", Name, Version);
		}
	}
}
