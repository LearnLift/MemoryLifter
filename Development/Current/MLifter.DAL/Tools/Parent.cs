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

namespace MLifter.DAL.Tools
{
	/// <summary>
	/// Parent interface.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-12-04</remarks>
	public interface IParent
	{
		/// <summary>
		/// Gets the parent.
		/// </summary>
		ParentClass Parent { get; }
	}

	/// <summary>
	/// Parent class used to link objects with their parents.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-12-04</remarks>
	public class ParentClass
	{
		private IUser currentUser;
		/// <summary>
		/// Gets the current user.
		/// </summary>
		public IUser CurrentUser { get { return currentUser; } }
		private IParent parentObject;
		/// <summary>
		/// Gets the parent object.
		/// </summary>
		public IParent ParentObject { get { return parentObject; } }
		/// <summary>
		/// Gets the current session id.
		/// </summary>
		public Guid CurrentSessionId { get { return (currentUser as User).SessionId; } }

		internal void GenerateNewSession() { (currentUser as User).GenerateNewSession(); }

		private Dictionary<ParentProperty, object> properties = new Dictionary<ParentProperty, object>();
		/// <summary>
		/// Gets or sets the properties.
		/// </summary>
		/// <value>
		/// The properties.
		/// </value>
		public Dictionary<ParentProperty, object> Properties
		{
			get { return properties; }
			set { properties = value; }
		}


		/// <summary>
		/// Occurs when the opened dictionary is closed.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		public event EventHandler DictionaryClosed;

		/// <summary>
		/// Raises the <see cref="E:DictionaryClosed"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>
		/// Documented by DAC, 2008-09-23.
		/// </remarks>
		internal void OnDictionaryClosed(IParent sender, EventArgs e)
		{
			if (currentUser != null)
				if (currentUser.Cache != null)
					currentUser.Cache.Clear();

			if (DictionaryClosed != null)
				DictionaryClosed(sender, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParentClass"/> class.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		public ParentClass(IUser user, IParent parent)
		{
			currentUser = user;
			parentObject = parent;
		}

		/// <summary>
		/// Gets the parent dictionary.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		public IDictionary GetParentDictionary()
		{
			if (ParentObject is IDictionary)
				return ParentObject as IDictionary;

			if (ParentObject.Parent != null)
				return ParentObject.Parent.GetParentDictionary();

			return null;
		}

		/// <summary>
		/// Gets the child parent class.
		/// </summary>
		/// <param name="newParent">The new parent.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		public ParentClass GetChildParentClass(IParent newParent)
		{
			return new ParentClass(CurrentUser, newParent);
		}
	}

	/// <summary>
	/// The parent property type.
	/// </summary>
	public enum ParentProperty
	{
		/// <summary>
		/// 
		/// </summary>
		ChapterMappings,
		/// <summary>
		/// 
		/// </summary>
		Statistic
	}
}
