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
	/// Represents the implementation of IQueryType used for presets.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPresetQueryType : IQueryType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlPresetQueryType"/> class.
		/// </summary>
		public XmlPresetQueryType()
		{

		}

		#region IQueryType Members

		private bool m_ImageRecognition;
		/// <summary>
		/// Gets or sets if the image recognition query mode is possible.
		/// </summary>
		/// <value>
		/// The image recognition query mode is possible.
		/// </value>
		public bool? ImageRecognition
		{
			get { return m_ImageRecognition; }
			set { m_ImageRecognition = value.GetValueOrDefault(); }
		}

		private bool m_ListeningComprehension;
		/// <summary>
		/// Gets or sets if the listening comprehension query mode is possible.
		/// </summary>
		/// <value>
		/// The listening comprehension query mode is possible.
		/// </value>
		public bool? ListeningComprehension
		{
			get { return m_ListeningComprehension; }
			set { m_ListeningComprehension = value.GetValueOrDefault(); }
		}

		private bool m_MultipleChoice;
		/// <summary>
		/// Gets or sets if the multiple choice query mode is possible.
		/// </summary>
		/// <value>
		/// The multiple choice query mode is possible.
		/// </value>
		public bool? MultipleChoice
		{
			get { return m_MultipleChoice; }
			set { m_MultipleChoice = value.GetValueOrDefault(); }
		}

		private bool m_Sentence;
		/// <summary>
		/// Gets or sets if the sentence query mode is possible.
		/// </summary>
		/// <value>
		/// The sentence query mode is possible.
		/// </value>
		public bool? Sentence
		{
			get { return m_Sentence; }
			set { m_Sentence = value.GetValueOrDefault(); }
		}

		private bool m_Word;
		/// <summary>
		/// Gets or sets if the word query mode is possible.
		/// </summary>
		/// <value>
		/// The word query mode is possible.
		/// </value>
		public bool? Word
		{
			get { return m_Word; }
			set { m_Word = value.GetValueOrDefault(); }
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
