using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// XML implementation of ISettings.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-15</remarks>
	public class XmlPresetSettings : ISettings
	{
		private XmlPresetQueryDirections m_QueryDirections;
		private XmlPresetQueryType m_QueryTypes;
		private XmlPresetMultipleChoiceOptions m_MultipleChoiceOptions;
		private XmlPresetGradeTyping m_GradeTyping;
		private XmlPresetGradeSynonyms m_GradeSynonyms;
		private XmlPresetSnoozeOptions m_SnoozeOptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlPresetSettings"/> class.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public XmlPresetSettings()
		{

		}

		#region ISettings Members

		/// <summary>
		/// Interface that defines the available query directions for a dictionary.
		/// </summary>
		[XmlIgnore]
		public IQueryDirections QueryDirections
		{
			get
			{
				if (m_QueryDirections == null)
					m_QueryDirections = new XmlPresetQueryDirections();
				return m_QueryDirections;
			}
			set { m_QueryDirections = (XmlPresetQueryDirections)value; }
		}

		/// <summary>
		/// Gets or sets the serializable query directions.
		/// </summary>
		/// <value>
		/// The serializable query directions.
		/// </value>
		[XmlElement("QueryDirections")]
		public XmlPresetQueryDirections SerializableQueryDirections
		{
			get
			{
				if (m_QueryDirections == null)
					m_QueryDirections = new XmlPresetQueryDirections();
				return m_QueryDirections;
			}
			set { m_QueryDirections = value; }
		}

		/// <summary>
		/// Gets or sets the Learning modes (How MemoryLifter ask the questions)
		/// </summary>
		/// <value>
		/// The query types.
		/// </value>
		[XmlIgnore]
		public IQueryType QueryTypes
		{
			get
			{
				if (m_QueryTypes == null)
					m_QueryTypes = new XmlPresetQueryType();
				return m_QueryTypes;
			}
			set { m_QueryTypes = (XmlPresetQueryType)value; }
		}

		/// <summary>
		/// Gets or sets the serializable query types.
		/// </summary>
		/// <value>
		/// The serializable query types.
		/// </value>
		[XmlElement("QueryTypes")]
		public XmlPresetQueryType SerializableQueryTypes
		{
			get
			{
				if (m_QueryTypes == null)
					m_QueryTypes = new XmlPresetQueryType();
				return m_QueryTypes;
			}
			set { m_QueryTypes = value; }
		}

		/// <summary>
		/// Gets or sets the multiple choice options.
		/// </summary>
		/// <value>
		/// The multiple choice options.
		/// </value>
		[XmlIgnore]
		public IQueryMultipleChoiceOptions MultipleChoiceOptions
		{
			get
			{
				if (m_MultipleChoiceOptions == null)
					m_MultipleChoiceOptions = new XmlPresetMultipleChoiceOptions();
				return m_MultipleChoiceOptions;
			}
			set { m_MultipleChoiceOptions = (XmlPresetMultipleChoiceOptions)value; }
		}

		/// <summary>
		/// Gets or sets the serializable multiple choice options.
		/// </summary>
		/// <value>
		/// The serializable multiple choice options.
		/// </value>
		[XmlElement("MultipleChoiceOptions")]
		public XmlPresetMultipleChoiceOptions SerializableMultipleChoiceOptions
		{
			get
			{
				if (m_MultipleChoiceOptions == null)
					m_MultipleChoiceOptions = new XmlPresetMultipleChoiceOptions();
				return m_MultipleChoiceOptions;
			}
			set { m_MultipleChoiceOptions = value; }
		}

		/// <summary>
		/// Enumeration which defines the values for the 'Typing mistakes' options:
		/// AllCorrect - only promote when all correct,
		/// HalfCorrect - promote when more than 50% were typed correctly,
		/// NoneCorrect - always promote,
		/// Prompt - prompt
		/// </summary>
		/// <value>
		/// The grade typing.
		/// </value>
		[XmlIgnore]
		public IGradeTyping GradeTyping
		{
			get
			{
				if (m_GradeTyping == null)
					m_GradeTyping = new XmlPresetGradeTyping();
				return m_GradeTyping;
			}
			set { m_GradeTyping = (XmlPresetGradeTyping)value; }
		}

		/// <summary>
		/// Gets or sets the serializable grade typing.
		/// </summary>
		/// <value>
		/// The serializable grade typing.
		/// </value>
		[XmlElement("GradeTyping")]
		public XmlPresetGradeTyping SerializableGradeTyping
		{
			get
			{
				if (m_GradeTyping == null)
					m_GradeTyping = new XmlPresetGradeTyping();
				return m_GradeTyping;
			}
			set { m_GradeTyping = value; }
		}

		/// <summary>
		/// Enumeration which defines the values for the 'Synonyms' options:
		/// AllKnown - all synonyms need to be known,
		/// HalfKnown - half need to be known,
		/// OneKnown - one synonym needs to be known,
		/// FirstKnown - the card is promoted when the first synonym is known,
		/// Promp - prompt when not all were correct
		/// </summary>
		/// <value>
		/// The grade synonyms.
		/// </value>
		[XmlIgnore]
		public IGradeSynonyms GradeSynonyms
		{
			get
			{
				if (m_GradeSynonyms == null)
					m_GradeSynonyms = new XmlPresetGradeSynonyms();
				return m_GradeSynonyms;
			}
			set { m_GradeSynonyms = (XmlPresetGradeSynonyms)value; }
		}

		/// <summary>
		/// Gets or sets the serializable grade synonyms.
		/// </summary>
		/// <value>
		/// The serializable grade synonyms.
		/// </value>
		[XmlElement("GradeSynonyms")]
		public XmlPresetGradeSynonyms SerializableGradeSynonyms
		{
			get
			{
				if (m_GradeSynonyms == null)
					m_GradeSynonyms = new XmlPresetGradeSynonyms();
				return m_GradeSynonyms;
			}
			set { m_GradeSynonyms = value; }
		}

		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>
		/// The style.
		/// </value>
		[XmlIgnore]
		public ICardStyle Style
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the question stylesheet.
		/// </summary>
		/// <value>
		/// The question stylesheet.
		/// </value>
		[XmlIgnore]
		public CompiledTransform? QuestionStylesheet
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the answer stylesheet.
		/// </summary>
		/// <value>
		/// The answer stylesheet.
		/// </value>
		[XmlIgnore]
		public CompiledTransform? AnswerStylesheet
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		private bool m_AutoplayAudio;
		/// <summary>
		/// Gets or sets a value indicating whether standard audio files should be played automatically.
		/// </summary>
		/// <value>
		///   <c>true</c> if [autoplay audio]; otherwise, <c>false</c>.
		/// </value>
		public bool? AutoplayAudio
		{
			get { return m_AutoplayAudio; }
			set { m_AutoplayAudio = value.GetValueOrDefault(); }
		}

		private bool m_CaseSensitive;
		/// <summary>
		/// Gets or sets a value indicating whether the answer is case sensitive.
		/// </summary>
		/// <value>
		///   <c>true</c> if case sensitive; otherwise, <c>false</c>.
		/// </value>
		public bool? CaseSensitive
		{
			get { return m_CaseSensitive; }
			set { m_CaseSensitive = value.GetValueOrDefault(); }
		}

		private bool m_ConfirmDemote;
		/// <summary>
		/// Gets or sets a value indicating whether user confirmation is required to confirm demote.
		/// </summary>
		/// <value>
		///   <c>true</c> if [confirm demote]; otherwise, <c>false</c>.
		/// </value>
		public bool? ConfirmDemote
		{
			get { return m_ConfirmDemote; }
			set { m_ConfirmDemote = value.GetValueOrDefault(); }
		}

		private bool m_EnableCommentary;
		/// <summary>
		/// Gets or sets a value indicating whether commentary sounds are enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if [enable commentary]; otherwise, <c>false</c>.
		/// </value>
		public bool? EnableCommentary
		{
			get { return m_EnableCommentary; }
			set { m_EnableCommentary = value.GetValueOrDefault(); }
		}

		private bool m_CorrectOnTheFly;
		/// <summary>
		/// Gets or sets a value indicating whether the correct on the fly mode is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if the correct on the fly mode is enbaled; otherwise, <c>false</c>.
		/// </value>
		public bool? CorrectOnTheFly
		{
			get { return m_CorrectOnTheFly; }
			set { m_CorrectOnTheFly = value.GetValueOrDefault(); }
		}

		private bool m_EnableTimer;
		/// <summary>
		/// Gets or sets a value indicating whether the time should be enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if [enable timer]; otherwise, <c>false</c>.
		/// </value>
		public bool? EnableTimer
		{
			get { return m_EnableTimer; }
			set { m_EnableTimer = value.GetValueOrDefault(); }
		}

		private bool m_RandomPool;
		/// <summary>
		/// Gets or sets a value indicating whether cards should be drawn randomly from pool.
		/// </summary>
		/// <value>
		///   <c>true</c> if [random pool]; otherwise, <c>false</c>.
		/// </value>
		public bool? RandomPool
		{
			get { return m_RandomPool; }
			set { m_RandomPool = value.GetValueOrDefault(); }
		}

		private bool m_SelfAssessment;
		/// <summary>
		/// Gets or sets a value indicating whether self assessment mode is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if [self assessment]; otherwise, <c>false</c>.
		/// </value>
		public bool? SelfAssessment
		{
			get { return m_SelfAssessment; }
			set { m_SelfAssessment = value.GetValueOrDefault(); }
		}

		private bool m_ShowImages;
		/// <summary>
		/// Gets or sets a value indicating whether to show images or not.
		/// </summary>
		/// <value>
		///   <c>true</c> if to show images; otherwise, <c>false</c>.
		/// </value>
		public bool? ShowImages
		{
			get { return m_ShowImages; }
			set { m_ShowImages = value.GetValueOrDefault(); }
		}

		private bool m_PoolEmptyMessageShown;
		/// <summary>
		/// Gets or sets wether the pool emty message was shown.
		/// </summary>
		/// <value>
		/// The pool emty message shown.
		/// </value>
		public bool? PoolEmptyMessageShown
		{
			get { return m_PoolEmptyMessageShown; }
			set { m_PoolEmptyMessageShown = value.GetValueOrDefault(); }
		}

		private bool m_UseLMStylesheets;
		/// <summary>
		/// Gets or sets the use LM stylesheets.
		/// </summary>
		/// <value>
		/// The use LM stylesheets.
		/// </value>
		public bool? UseLMStylesheets
		{
			get { return m_UseLMStylesheets; }
			set { m_UseLMStylesheets = value.GetValueOrDefault(); }
		}

		/// <summary>
		/// Gets or sets the the auto box size.
		/// </summary>
		/// <value>
		/// The size of the auto box.
		/// </value>
		[XmlIgnore]
		public bool? AutoBoxSize
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		private string m_StripChars;
		/// <summary>
		/// Gets or sets the strip chars.
		/// </summary>
		/// <value>
		/// The strip chars.
		/// </value>
		public string StripChars
		{
			get { return m_StripChars; }
			set { m_StripChars = value; }
		}

		/// <summary>
		/// Gets or sets the question culture.
		/// </summary>
		/// <value>
		/// The question culture.
		/// </value>
		[XmlIgnore]
		public System.Globalization.CultureInfo QuestionCulture
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the answer culture.
		/// </summary>
		/// <value>
		/// The answer culture.
		/// </value>
		[XmlIgnore]
		public System.Globalization.CultureInfo AnswerCulture
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the question caption.
		/// </summary>
		/// <value>
		/// The question caption.
		/// </value>
		[XmlIgnore]
		public string QuestionCaption
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the answer caption.
		/// </summary>
		/// <value>
		/// The answer caption.
		/// </value>
		[XmlIgnore]
		public string AnswerCaption
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the commentary sounds.
		/// </summary>
		/// <value>
		/// The commentary sounds.
		/// </value>
		[XmlIgnore]
		public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Gets or sets the logo.
		/// </summary>
		/// <value>
		/// The logo.
		/// </value>
		[XmlIgnore]
		public IMedia Logo
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		private bool m_ShowStatistics;
		/// <summary>
		/// Gets or sets a value indicating whether [show statistics].
		/// </summary>
		/// <value>
		///   <c>true</c> if [show statistics]; otherwise, <c>false</c>.
		/// </value>
		public bool? ShowStatistics
		{
			get { return m_ShowStatistics; }
			set { m_ShowStatistics = value.GetValueOrDefault(); }
		}

		private bool m_SkipCorrectAnswers;
		/// <summary>
		/// Gets or sets a value indicating whether [skip correct answers].
		/// </summary>
		/// <value>
		///   <c>true</c> if [skip correct answers]; otherwise, <c>false</c>.
		/// </value>
		public bool? SkipCorrectAnswers
		{
			get { return m_SkipCorrectAnswers; }
			set { m_SkipCorrectAnswers = value.GetValueOrDefault(); }
		}

		/// <summary>
		/// Gets or sets the snooze options (MemoryLifter minimize after a few cards)
		/// </summary>
		/// <value>
		/// The snooze options.
		/// </value>
		[XmlIgnore]
		public ISnoozeOptions SnoozeOptions
		{
			get
			{
				if (m_SnoozeOptions == null)
					m_SnoozeOptions = new XmlPresetSnoozeOptions();
				return m_SnoozeOptions;
			}
			set
			{
				m_SnoozeOptions = (XmlPresetSnoozeOptions)value;
			}
		}

		/// <summary>
		/// Gets or sets the serializable snooze options.
		/// </summary>
		/// <value>
		/// The serializable snooze options.
		/// </value>
		[XmlElement("SnoozeOptions")]
		public XmlPresetSnoozeOptions SerializableSnoozeOptions
		{
			get
			{
				if (m_SnoozeOptions == null)
					m_SnoozeOptions = new XmlPresetSnoozeOptions();
				return m_SnoozeOptions;
			}
			set { m_SnoozeOptions = value; }
		}

		/// <summary>
		/// Gets or sets the selected learn chapters.
		/// </summary>
		/// <value>
		/// The selected learn chapters.
		/// </value>
		[XmlIgnore]
		public IList<int> SelectedLearnChapters
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
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

		#region ISecurity Members

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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
