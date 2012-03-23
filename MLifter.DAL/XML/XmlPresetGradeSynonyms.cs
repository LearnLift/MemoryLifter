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
	/// Represents the implementation of IGradeSynonyms used for presets.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPresetGradeSynonyms : IGradeSynonyms
	{
		#region IGradeSynonyms Members

		private bool m_AllKnown;
		/// <summary>
		/// Gets or sets all known.
		/// </summary>
		/// <value>
		/// All known.
		/// </value>
		public bool? AllKnown
		{
			get { return m_AllKnown; }
			set { m_AllKnown = value.GetValueOrDefault(); }
		}

		private bool m_HalfKnown;
		/// <summary>
		/// Gets or sets the half known.
		/// </summary>
		/// <value>
		/// The half known.
		/// </value>
		public bool? HalfKnown
		{
			get { return m_HalfKnown; }
			set { m_HalfKnown = value.GetValueOrDefault(); }
		}

		private bool m_OneKnown;
		/// <summary>
		/// Gets or sets the one known.
		/// </summary>
		/// <value>
		/// The one known.
		/// </value>
		public bool? OneKnown
		{
			get { return m_OneKnown; }
			set { m_OneKnown = value.GetValueOrDefault(); }
		}

		private bool m_FirstKnown;
		/// <summary>
		/// Gets or sets the first known.
		/// </summary>
		/// <value>
		/// The first known.
		/// </value>
		public bool? FirstKnown
		{
			get { return m_FirstKnown; }
			set { m_FirstKnown = value.GetValueOrDefault(); }
		}

		private bool m_Prompt;
		/// <summary>
		/// Gets or sets the prompt.
		/// </summary>
		/// <value>
		/// The prompt.
		/// </value>
		public bool? Prompt
		{
			get { return m_Prompt; }
			set { m_Prompt = value.GetValueOrDefault(); }
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
