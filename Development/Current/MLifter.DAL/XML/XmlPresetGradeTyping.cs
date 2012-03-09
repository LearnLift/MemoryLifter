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
