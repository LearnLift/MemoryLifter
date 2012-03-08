using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents the implementation of IQueryMultipleChoiceOptions used for presets.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPresetMultipleChoiceOptions : IQueryMultipleChoiceOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlPresetMultipleChoiceOptions"/> class.
		/// </summary>
		public XmlPresetMultipleChoiceOptions() { }

		#region IQueryMultipleChoiceOptions Members

		private bool m_AllowRandomDistractors;
		/// <summary>
		/// Gets or sets if allow random distractors.
		/// </summary>
		/// <value>
		/// Allow random distractors.
		/// </value>
		public bool? AllowRandomDistractors
		{
			get { return m_AllowRandomDistractors; }
			set { m_AllowRandomDistractors = value.GetValueOrDefault(); }
		}

		private bool m_AllowMultipleCorrectAnswers;
		/// <summary>
		/// Gets or sets if allow multiple correct answers.
		/// </summary>
		/// <value>
		/// Allow multiple correct answers.
		/// </value>
		public bool? AllowMultipleCorrectAnswers
		{
			get { return m_AllowMultipleCorrectAnswers; }
			set { m_AllowMultipleCorrectAnswers = value.GetValueOrDefault(); }
		}

		private int m_NumberOfChoices;
		/// <summary>
		/// Gets or sets the number of choices.
		/// </summary>
		/// <value>
		/// The number of choices.
		/// </value>
		public int? NumberOfChoices
		{
			get { return m_NumberOfChoices; }
			set { m_NumberOfChoices = value.GetValueOrDefault(); }
		}

		private int m_MaxNumberOfCorrectAnswers;
		/// <summary>
		/// Gets or sets the max number of correct answers.
		/// </summary>
		/// <value>
		/// The max number of correct answers.
		/// </value>
		public int? MaxNumberOfCorrectAnswers
		{
			get { return m_MaxNumberOfCorrectAnswers; }
			set { m_MaxNumberOfCorrectAnswers = value.GetValueOrDefault(); }
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
