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
using System.Xml.Serialization;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents the implementation of IQueryDirections used for presets.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPresetQueryDirections : IQueryDirections
	{
		#region IQueryDirections Members

		private bool m_Question2Answer;
		/// <summary>
		/// Gets or sets a value indicating whether [question to answer] is allowed.
		/// </summary>
		/// <value>
		///   <c>true</c> if [question to answer]; otherwise, <c>false</c>.
		/// </value>
		public bool? Question2Answer
		{
			get { return m_Question2Answer; }
			set { m_Question2Answer = value.GetValueOrDefault(); }
		}

		private bool m_Answer2Question;
		/// <summary>
		/// Gets or sets a value indicating whether [answer to question] is allowed.
		/// </summary>
		/// <value>
		///   <c>true</c> if [answer to question]; otherwise, <c>false</c>.
		/// </value>
		public bool? Answer2Question
		{
			get { return m_Answer2Question; }
			set { m_Answer2Question = value.GetValueOrDefault(); }
		}

		private bool m_Mixed;
		/// <summary>
		/// Gets or sets a value indicating whether this is in mixed mode.
		/// </summary>
		/// <value>
		///   <c>true</c> if mixed; otherwise, <c>false</c>.
		/// </value>
		public bool? Mixed
		{
			get { return m_Mixed; }
			set { m_Mixed = value.GetValueOrDefault(); }
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		[XmlIgnore]
		public MLifter.DAL.Tools.ParentClass Parent
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion
	}
}
