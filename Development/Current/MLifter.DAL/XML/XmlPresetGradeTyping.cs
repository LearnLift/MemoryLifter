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
	/// Represents the implementation of IGradeTyping used for presets.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPresetGradeTyping : IGradeTyping
	{
		#region IGradeTyping Members

		private bool m_AllCorrect;
		/// <summary>
		/// Gets or sets all correct mode.
		/// </summary>
		/// <value>
		/// All correct.
		/// </value>
		public bool? AllCorrect
		{
			get { return m_AllCorrect; }
			set { m_AllCorrect = value.GetValueOrDefault(); }
		}

		private bool m_HalfCorrect;
		/// <summary>
		/// Gets or sets the half correct mode.
		/// </summary>
		/// <value>
		/// The half correct.
		/// </value>
		public bool? HalfCorrect
		{
			get { return m_HalfCorrect; }
			set { m_HalfCorrect = value.GetValueOrDefault(); }
		}

		private bool m_NoneCorrect;
		/// <summary>
		/// Gets or sets the none correct mode.
		/// </summary>
		/// <value>
		/// The none correct.
		/// </value>
		public bool? NoneCorrect
		{
			get { return m_NoneCorrect; }
			set { m_NoneCorrect = value.GetValueOrDefault(); }
		}

		private bool m_Prompt;
		/// <summary>
		/// Gets or sets the prompt mode.
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
