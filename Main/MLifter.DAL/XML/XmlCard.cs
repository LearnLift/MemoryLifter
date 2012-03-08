using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

using MLifter.DAL;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents an Xml card object derived from ICard
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-07</remarks>
	public class XmlCard : ICard
	{
		private bool m_IsDisposed = false;

		private XmlDictionary m_oDictionary;
		private XmlDocument m_dictionary;

		private XmlElement m_card;

		private IWords m_question;
		private IWords m_questionExample;
		private IWords m_answer;
		private IWords m_answerExample;
		private IWords m_QuestionDistractors;
		private IWords m_AnswerDistractors;

		private List<IMedia> m_questionMedia = new List<IMedia>();
		private List<IMedia> m_answerMedia = new List<IMedia>();

		private ICardStyle m_Style = null;

		private int m_random;
		private DateTime m_timestamp;

		private const string m_XPath_Style = "cardStyle";

		private Regex m_RegExFileNameCleaner = new Regex(@"\W");
		private Regex m_RegExStrictFileNameCleaner = new Regex(@"\W", RegexOptions.ECMAScript);
		private const string m_MediaFileFormatString = "{0}_{1}_{2}";
		private const string m_ExampleMediaFileFormatString = "{0}_{1}_example_{2}";

		private XmlCard(ParentClass parentClass)
		{
			parent = parentClass;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlCard"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="parentClass">The parent class.</param>
		/// <remarks>
		/// Documented by AAB, 7.8.2007.
		/// </remarks>
		internal XmlCard(XmlDictionary dictionary, ParentClass parentClass)
			: this(parentClass)
		{
			settings = new XmlCardSettings(this, Parent.GetChildParentClass(this));

			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;

			XmlElement xeCard = m_dictionary.CreateElement("card");
			XmlHelper.CreateAndAppendAttribute(xeCard, "id", Convert.ToString(-1));
			XmlHelper.CreateAndAppendElement(xeCard, "answer");
			XmlHelper.CreateAndAppendElement(xeCard, "answerexample");
			XmlHelper.CreateAndAppendElement(xeCard, "answerstylesheet");
			XmlHelper.CreateAndAppendElement(xeCard, "box", Convert.ToString(0));
			XmlHelper.CreateAndAppendElement(xeCard, "chapter", Convert.ToString(0));
			XmlHelper.CreateAndAppendElement(xeCard, "question");
			XmlHelper.CreateAndAppendElement(xeCard, "questionexample");
			XmlHelper.CreateAndAppendElement(xeCard, "questionstylesheet");
			XmlHelper.CreateAndAppendElement(xeCard, "timestamp", XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.RoundtripKind));
			m_card = xeCard;

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlCard"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="card">The card.</param>
		/// <param name="parentClass">The parent class.</param>
		/// <remarks>
		/// Documented by AAB, 7.8.2007.
		/// </remarks>
		internal XmlCard(XmlDictionary dictionary, XmlElement card, ParentClass parentClass)
			: this(parentClass)
		{
			settings = new XmlCardSettings(this, Parent.GetChildParentClass(this));

			m_oDictionary = dictionary;
			if (dictionary != null)
			{
				m_dictionary = card.OwnerDocument;
			}
			m_card = card;

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlCard"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="card_id">The card_id.</param>
		/// <param name="parentClass">The parent class.</param>
		/// <remarks>
		/// Documented by AAB, 7.8.2007.
		/// </remarks>
		internal XmlCard(XmlDictionary dictionary, int card_id, ParentClass parentClass)
			: this(parentClass)
		{
			settings = new XmlCardSettings(this, Parent.GetChildParentClass(this));

			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;
			m_card = (XmlElement)m_dictionary.SelectSingleNode("/dictionary/card[@id='" + card_id.ToString() + "']");
			if (m_card == null)
				throw new IdAccessException(card_id);

			Initialize();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="XmlCard"/> is reclaimed by garbage collection.
		/// </summary>
		~XmlCard()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!m_IsDisposed)
			{
				//if (this.Id < 0)
				//{
				//    //[ML-696] Preview throws an exception for a card that is not attached
				//    //this.ClearAllMedia();
				//}
			}
			m_IsDisposed = true;
		}

		/// <summary>
		/// Gets a random integer value.
		/// </summary>
		/// <value>The random value.</value>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		internal int Random
		{
			get { return m_random; }
			set { m_random = value; }
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>The dictionary.</value>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		internal XmlDictionary Dictionary
		{
			get
			{
				return m_oDictionary;
			}
			set
			{
				m_oDictionary = value;
				if (m_oDictionary != null)
				{
					m_dictionary = m_oDictionary.Dictionary;
				}
			}
		}

		private XmlSerializer StyleSerializer
		{
			get
			{
				return m_oDictionary.StyleSerializer;
			}
		}

		internal XmlElement Xml { get { return m_card; } }

		#region ICard Members

		/// <summary>
		/// Occurs when [create media progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		public event StatusMessageEventHandler CreateMediaProgressChanged;

		/// <summary>
		/// Gets the card.
		/// </summary>
		/// <value>The card.</value>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public XmlElement Card
		{
			get
			{
				return m_oDictionary.Encryption == XmlDictionary.EncryptionMode.StickEncryption ?
					Helper.GenerateXmlCard(this) : m_card;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ICard"/> is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if active; otherwise, <c>false</c>.
		/// </value>
		public bool Active
		{
			get
			{
				return (Box >= 0);
			}
			set
			{
				if (value == false)
				{
					if (m_oDictionary.QueryChapter.Contains(this.Chapter))
						if (Box >= 0) ((XmlBox)m_oDictionary.Boxes.Box[Box]).Size--;
					Box = -1;
				}
				else if ((value == true) && (Box == -1))
				{
					if (m_oDictionary.QueryChapter.Contains(this.Chapter))
						((XmlBox)m_oDictionary.Boxes.Box[0]).Size++;
					Box = 0;
				}
			}
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public int Id
		{
			get
			{
				return XmlConvert.ToInt32(m_card.GetAttribute("id"));
			}
			set
			{
				if (value < 0)
					throw new IdAccessException(value);
				if (Id != value)
				{
					if (Dictionary.Cards.Get(value) != null)
					{
						throw new IdExistsException(value);
					}
					else
					{
						m_card.SetAttribute("id", XmlConvert.ToString(value));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the box.
		/// </summary>
		/// <value>
		/// The box.
		/// </value>
		public int Box
		{
			get
			{
				if (m_card["box"] != null)
				{
					return XmlConvert.ToInt32(m_card["box"].InnerText);
				}
				else
				{
					return 0;
				}
			}
			set
			{
				if ((value > m_oDictionary.NumberOfBoxes) || (value < -1))
					throw new InvalidBoxException(value); ;
				if (m_card["box"] != null)
				{
					int tempBox = XmlConvert.ToInt32(m_card["box"].InnerText);
					if ((tempBox != value) && m_oDictionary.QueryChapter.Contains(this.Chapter))
					{
						if (tempBox >= 0)
							((XmlBox)m_oDictionary.Boxes.Box[tempBox]).Size--;
						if (value >= 0)
							((XmlBox)m_oDictionary.Boxes.Box[value]).Size++;
					}
					m_card["box"].InnerText = XmlConvert.ToString(value);
				}
				else
				{
					if ((value >= 0) && m_oDictionary.QueryChapter.Contains(this.Chapter))
						((XmlBox)m_oDictionary.Boxes.Box[value]).Size++;
					XmlHelper.CreateAndAppendElement(m_card, "box", XmlConvert.ToString(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the chapter.
		/// </summary>
		/// <value>
		/// The chapter.
		/// </value>
		public int Chapter
		{
			get
			{
				return XmlConvert.ToInt32(m_card["chapter"].InnerText);
			}
			set
			{
				if (Dictionary.Chapters.Get(value) == null)
					throw new IdAccessException(value);
				if ((Box >= 0) && (m_oDictionary.QueryChapter.Contains(this.Chapter) ^ m_oDictionary.QueryChapter.Contains(value)))
				{
					if (m_oDictionary.QueryChapter.Contains(value)) //card is now in query chapters and was not before
						((XmlBox)m_oDictionary.Boxes.Box[Box]).Size++;
					else //card was in query chapters before and is no longer
						((XmlBox)m_oDictionary.Boxes.Box[Box]).Size--;
				}
				m_card["chapter"].InnerText = XmlConvert.ToString(value);
			}
		}

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>
		/// The timestamp.
		/// </value>
		public DateTime Timestamp
		{
			get
			{
				return m_timestamp;
			}
			set
			{
				if (m_card["timestamp"] != null)
				{
					m_card["timestamp"].InnerText = XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_card, "timestamp", XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));
				}
				m_timestamp = value;
			}
		}

		/// <summary>
		/// Gets or sets the question.
		/// </summary>
		/// <value>
		/// The question.
		/// </value>
		public IWords Question
		{
			get
			{
				return m_question;
			}
			set
			{
				m_question = value;
			}
		}

		/// <summary>
		/// Gets or sets the question example.
		/// </summary>
		/// <value>
		/// The question example.
		/// </value>
		public IWords QuestionExample
		{
			get
			{
				return m_questionExample;
			}
			set
			{
				m_questionExample = value;
			}
		}

		/// <summary>
		/// Gets or sets the question stylesheet.
		/// </summary>
		/// <value>
		/// The question stylesheet.
		/// </value>
		public string QuestionStylesheet
		{
			get
			{
				return m_card["questionstylesheet"].InnerText;
			}
			set
			{
				m_card["questionstylesheet"].InnerText = value;
			}
		}

		/// <summary>
		/// Gets or sets the question media.
		/// </summary>
		/// <value>
		/// The question media.
		/// </value>
		public IList<IMedia> QuestionMedia
		{
			get
			{
				BindingList<IMedia> bindingListQuestionMedia = new BindingList<IMedia>(m_questionMedia);
				bindingListQuestionMedia.ListChanged += new ListChangedEventHandler(bindingListQuestionMedia_ListChanged);
				return bindingListQuestionMedia;
			}
		}

		void bindingListQuestionMedia_ListChanged(object sender, ListChangedEventArgs e)
		{
			SaveMedia(Side.Question);
		}

		/// <summary>
		/// Gets or sets the question distractors.
		/// </summary>
		/// <value>
		/// The question distractors.
		/// </value>
		public IWords QuestionDistractors
		{
			get
			{
				return m_QuestionDistractors;
			}
			set
			{
				m_QuestionDistractors = value;
			}
		}

		/// <summary>
		/// Gets or sets the answer.
		/// </summary>
		/// <value>
		/// The answer.
		/// </value>
		public IWords Answer
		{
			get
			{
				return m_answer;
			}
			set
			{
				m_answer = value;
			}
		}

		/// <summary>
		/// Gets or sets the answer example.
		/// </summary>
		/// <value>
		/// The answer example.
		/// </value>
		public IWords AnswerExample
		{
			get
			{
				return m_answerExample;
			}
			set
			{
				m_answerExample = value;
			}
		}

		/// <summary>
		/// Gets or sets the answer stylesheet.
		/// </summary>
		/// <value>
		/// The answer stylesheet.
		/// </value>
		public string AnswerStylesheet
		{
			get
			{
				return m_card["answerstylesheet"].InnerText;
			}
			set
			{
				m_card["answerstylesheet"].InnerText = value;
			}
		}

		/// <summary>
		/// Gets or sets the answer media.
		/// </summary>
		/// <value>
		/// The answer media.
		/// </value>
		public IList<IMedia> AnswerMedia
		{
			get
			{
				BindingList<IMedia> bindingListAnswerMedia = new BindingList<IMedia>(m_answerMedia);
				bindingListAnswerMedia.ListChanged += new ListChangedEventHandler(bindingListAnswerMedia_ListChanged);
				return bindingListAnswerMedia;
			}
		}

		void bindingListAnswerMedia_ListChanged(object sender, ListChangedEventArgs e)
		{
			SaveMedia(Side.Answer);
		}

		/// <summary>
		/// Gets or sets the answer distractors.
		/// </summary>
		/// <value>
		/// The answer distractors.
		/// </value>
		public IWords AnswerDistractors
		{
			get
			{
				return m_AnswerDistractors;
			}
			set
			{
				m_AnswerDistractors = value;
			}
		}

		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>
		/// The style.
		/// </value>
		public ICardStyle Style
		{
			get
			{
				if (m_Style == null)
					ReadStyleFromDOM();
				return m_Style;
			}
			set
			{
				if (SaveStyleToDOM(value))
					m_Style = value;
				else
					m_Style = null;
			}
		}

		/// <summary>
		/// Adds the media to the card.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="side">The side.</param>
		/// <returns>The media object.</returns>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public IMedia AddMedia(IMedia media, Side side)
		{
			if (media == null) throw new NullReferenceException("Media object must not be null!");
			if (!(media is XmlMedia))
				media = CreateMedia(media.MediaType, media.Filename, media.Active.Value, media.Default.Value, media.Example.Value);
			RemoveMediaNodes(media, side); //cleanup old media nodes with the same path
			StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.CreateMediaProgress);
			ReportProgressUpdate(args);
			if (CopyFileToMediaFolder(media, side))
			{
				ReportProgressUpdate(args);
				CreateMediaNode(media, side);
				return media;
			}
			else
			{
				ReportProgressUpdate(args);
				return null;
			}
		}

		/// <summary>
		/// Sends the status message update.
		/// </summary>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <returns>
		/// [true] if the process should be canceled.
		/// </returns>
		/// <remarks>Documented by Dev03, 2008-08-20</remarks>
		private bool ReportProgressUpdate(StatusMessageEventArgs args)
		{
			switch (args.MessageType)
			{
				case StatusMessageType.CreateMediaProgress:
					if (CreateMediaProgressChanged != null)
						CreateMediaProgressChanged(this, args);
					break;
			}
			return true;
		}

		/// <summary>
		/// Creates the media object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default audio].</param>
		/// <param name="isExample">if set to <c>true</c> [is example audio].</param>
		/// <returns>A media object that implements IMedia.</returns>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			return (parent.GetParentDictionary() as XmlDictionary).CreateNewMediaObject(type, path, isActive, isDefault, isExample);
		}

		/// <summary>
		/// Removes the media.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public void RemoveMedia(IMedia media)
		{
			DeleteFileFromMediaFolder(media);
			RemoveMediaNodes(media, Side.Question);
			RemoveMediaNodes(media, Side.Answer);
		}

		/// <summary>
		/// Saves this card.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public void Save()
		{
			SaveMedia(Side.Question);
			SaveMedia(Side.Answer);
		}

		/// <summary>
		/// Saves the media.
		/// </summary>
		/// <param name="side">The side.</param>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		private void SaveMedia(Side side)
		{
			if (side == Side.Question)
			{
				List<IMedia> questionMediaClone = new List<IMedia>(m_questionMedia); //clone the list because AddMedia modifies the original one
				foreach (IMedia media in questionMediaClone)
					AddMedia(media, side);
			}
			else
			{
				List<IMedia> anserMediaClone = new List<IMedia>(m_answerMedia);
				foreach (IMedia media in anserMediaClone)
					AddMedia(media, side);
			}
		}

		/// <summary>
		/// Creates the media node.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="side">The side.</param>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		private void CreateMediaNode(IMedia media, Side side)
		{
			RemoveMediaNodes(media, side);  //remove old media nodes for this media resource
			string tagPrefix = side.ToString().ToLower();
			string typeString = media.MediaType.ToString().ToLower();
			switch (media.MediaType)
			{
				case EMedia.Audio:
					XmlElement xeMedia;
					if (!media.Active.GetValueOrDefault())
						xeMedia = XmlHelper.CreateElementWithAttribute(m_card, "unusedmedia", media.Filename, "type", typeString);
					else
					{
						if (((XmlAudio)media).Default.GetValueOrDefault())
						{
							xeMedia = (XmlElement)m_card.SelectSingleNode(tagPrefix + typeString + "[@id='std']");
							if (xeMedia == null)
							{
								xeMedia = XmlHelper.CreateElementWithAttribute(m_card, tagPrefix + typeString, media.Filename, "id", "std");
							}
							else
							{
								DeleteFileFromMediaFolder(new XmlAudio(xeMedia.InnerText, Parent.GetChildParentClass(this)));
								xeMedia.InnerText = media.Filename;
							}
						}
						else if (((XmlAudio)media).Example.GetValueOrDefault())
						{
							xeMedia = (XmlElement)m_card.SelectSingleNode(tagPrefix + "example" + typeString);
							if (xeMedia == null)
							{
								xeMedia = XmlHelper.CreateAndAppendElement(m_card, tagPrefix + "example" + typeString, media.Filename);
							}
							else
							{
								DeleteFileFromMediaFolder(new XmlAudio(xeMedia.InnerText, Parent.GetChildParentClass(this)));
								xeMedia.InnerText = media.Filename;
							}
						}
						if (!((XmlAudio)media).Default.GetValueOrDefault() && !((XmlAudio)media).Example.GetValueOrDefault())
							xeMedia = XmlHelper.CreateAndAppendElement(m_card, tagPrefix + typeString, media.Filename);
					}
					break;
				case EMedia.Image:
					if (!media.Active.GetValueOrDefault())
						xeMedia = XmlHelper.CreateElementWithAttribute(m_card, "unusedmedia", media.Filename, "type", typeString);
					else
						xeMedia = XmlHelper.CreateAndAppendElement(m_card, tagPrefix + typeString, media.Filename);
					IImage image = (IImage)media;
					if ((image.Height > 0) && (image.Width > 0))
					{
						XmlHelper.CreateAndAppendAttribute(xeMedia, "width", image.Width.ToString());
						XmlHelper.CreateAndAppendAttribute(xeMedia, "height", image.Height.ToString());
					}
					break;
				case EMedia.Video:
				default:
					if (!media.Active.GetValueOrDefault())
						XmlHelper.CreateElementWithAttribute(m_card, "unusedmedia", media.Filename, "type", typeString);
					else
						XmlHelper.CreateAndAppendElement(m_card, tagPrefix + typeString, media.Filename);
					break;
			}
			Initialize();
		}

		/// <summary>
		/// Removes the media nodes.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="side">The side.</param>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		private void RemoveMediaNodes(IMedia media, Side side)
		{
			string tagPrefix = side.ToString().ToLower();
			//remove existing media nodes with the same file path
			string xpath = String.Format("{0}audio[text() = '{1}']|{0}exampleaudio[text() = '{1}']|{0}image[text() = '{1}']|{0}video[text() = '{1}']|unusedmedia[text() = '{1}']",
				tagPrefix, media.Filename.Replace("'", "&apos;"));
			foreach (XmlNode existingNode in m_card.SelectNodes(xpath))
				existingNode.ParentNode.RemoveChild(existingNode);
			Initialize();   //reload the question/answer media lists
		}

		/// <summary>
		/// Clears all media.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		public void ClearAllMedia()
		{
			ClearAllMedia(true);
		}

		/// <summary>
		/// Clears all media.
		/// </summary>
		/// <param name="removeFiles">if set to <c>true</c> [remove files].</param>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		public void ClearAllMedia(bool removeFiles)
		{
			if (removeFiles)
			{
				foreach (IMedia media in m_answerMedia)
					DeleteFileFromMediaFolder(media);
				foreach (IMedia media in m_questionMedia)
					DeleteFileFromMediaFolder(media);
			}
			ClearQuestionMedia();
			ClearAnswerMedia();
			ClearUnusedMedia();
			m_questionMedia.Clear();
			m_answerMedia.Clear();
		}

		/// <summary>
		/// Creates and returns a card style.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		public ICardStyle CreateCardStyle()
		{
			return new XmlCardStyle(parent);
		}

		private ISettings settings;
		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public ISettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
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
			return String.Format("{0} - {1} - {2}", Id, Question, Answer);
		}

		/// <summary>
		/// Clears the question media.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private void ClearQuestionMedia()
		{
			XmlNodeList questionaudio = m_card.SelectNodes("questionaudio");
			foreach (XmlNode node in questionaudio)
				m_card.RemoveChild(node);
			XmlNodeList questionimage = m_card.SelectNodes("questionimage");
			foreach (XmlNode node in questionimage)
				m_card.RemoveChild(node);
			XmlNodeList questionvideo = m_card.SelectNodes("questionvideo");
			foreach (XmlNode node in questionvideo)
				m_card.RemoveChild(node);
			XmlNodeList questionexampleaudio = m_card.SelectNodes("questionexampleaudio");
			foreach (XmlNode node in questionexampleaudio)
				m_card.RemoveChild(node);
		}

		/// <summary>
		/// Clears the answer media.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private void ClearAnswerMedia()
		{
			XmlNodeList answeraudio = m_card.SelectNodes("answeraudio");
			foreach (XmlNode node in answeraudio)
				m_card.RemoveChild(node);
			XmlNodeList answerimage = m_card.SelectNodes("answerimage");
			foreach (XmlNode node in answerimage)
				m_card.RemoveChild(node);
			XmlNodeList answervideo = m_card.SelectNodes("answervideo");
			foreach (XmlNode node in answervideo)
				m_card.RemoveChild(node);
			XmlNodeList answerexampleaudio = m_card.SelectNodes("answerexampleaudio");
			foreach (XmlNode node in answerexampleaudio)
				m_card.RemoveChild(node);
		}

		/// <summary>
		/// Clears the unused media.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private void ClearUnusedMedia()
		{
			XmlNodeList unusedmedia = m_card.SelectNodes("unusedmedia");
			foreach (XmlNode node in unusedmedia)
				m_card.RemoveChild(node);
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private void Initialize()
		{
			m_questionMedia.Clear();
			m_answerMedia.Clear();
			foreach (XmlNode xnMedia in m_card.SelectNodes("questionaudio"))
			{
				XmlNode xaId;
				bool isDefault = false;
				if ((xaId = xnMedia.Attributes.GetNamedItem("id")) != null)
					if (xaId.Value.Equals("std"))
						isDefault = true;
				m_questionMedia.Add(new XmlAudio(Dictionary, xnMedia.InnerText, isDefault, false, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("questionexampleaudio"))
			{
				m_questionMedia.Add(new XmlAudio(Dictionary, xnMedia.InnerText, false, true, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("answeraudio"))
			{
				XmlNode xaId;
				bool isDefault = false;
				if ((xaId = xnMedia.Attributes.GetNamedItem("id")) != null)
					if (xaId.Value.Equals("std"))
						isDefault = true;
				m_answerMedia.Add(new XmlAudio(Dictionary, xnMedia.InnerText, isDefault, false, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("answerexampleaudio"))
			{
				m_answerMedia.Add(new XmlAudio(Dictionary, xnMedia.InnerText, false, true, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("questionvideo"))
			{
				m_questionMedia.Add(new XmlVideo(Dictionary, xnMedia.InnerText, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("answervideo"))
			{
				m_answerMedia.Add(new XmlVideo(Dictionary, xnMedia.InnerText, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("questionimage"))
			{
				XmlNode xaWidht, xaHeight;
				int width = 0, height = 0;
				if ((xaWidht = xnMedia.Attributes.GetNamedItem("width")) != null)
				{
					Int32.TryParse(xaWidht.Value, out width);
				}
				if ((xaHeight = xnMedia.Attributes.GetNamedItem("height")) != null)
				{
					Int32.TryParse(xaHeight.Value, out height);
				}
				m_questionMedia.Add(new XmlImage(Dictionary, xnMedia.InnerText, width, height, true, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("answerimage"))
			{
				XmlNode xaWidht, xaHeight;
				int width = 0, height = 0;
				if ((xaWidht = xnMedia.Attributes.GetNamedItem("width")) != null)
				{
					Int32.TryParse(xaWidht.Value, out width);
				}
				if ((xaHeight = xnMedia.Attributes.GetNamedItem("height")) != null)
				{
					Int32.TryParse(xaHeight.Value, out height);
				}
				m_answerMedia.Add(new XmlImage(Dictionary, xnMedia.InnerText, width, height, true, Parent.GetChildParentClass(this)));
			}
			foreach (XmlNode xnMedia in m_card.SelectNodes("unusedmedia"))
			{
				//unused (not active) media is added to answer media
				string mediaPath = xnMedia.InnerText;
				EMedia mediaType;
				XmlAttribute xaType = xnMedia.Attributes["type"];
				if (xaType != null)
				{
					mediaType = (EMedia)Enum.Parse(typeof(EMedia), xaType.Value, true);
				}
				else
				{
					mediaType = DAL.Helper.GetMediaType(mediaPath);
				}
				switch (mediaType)
				{
					case EMedia.Audio:
						m_answerMedia.Add(new XmlAudio(Dictionary, mediaPath, false, false, false, Parent.GetChildParentClass(this)));
						break;
					case EMedia.Video:
						m_answerMedia.Add(new XmlVideo(Dictionary, mediaPath, false, Parent.GetChildParentClass(this)));
						break;
					case EMedia.Image:
						m_answerMedia.Add(new XmlImage(Dictionary, mediaPath, false, Parent.GetChildParentClass(this)));
						break;
					case EMedia.Unknown:
					default:
						break;
				}
			}

			m_question = new XmlQuestion(this, Parent.GetChildParentClass(this));
			m_questionExample = new XmlQuestionExample(this, Parent.GetChildParentClass(this));
			m_answer = new XmlAnswer(this, Parent.GetChildParentClass(this));
			m_answerExample = new XmlAnswerExample(this, Parent.GetChildParentClass(this));

			m_QuestionDistractors = new XmlQuestionDistractors(m_oDictionary, this, Parent.GetChildParentClass(this));
			m_AnswerDistractors = new XmlAnswerDistractors(m_oDictionary, this, Parent.GetChildParentClass(this));

			if (m_card["timestamp"] != null)
			{
				m_timestamp = XmlConvert.ToDateTime(m_card["timestamp"].InnerText, XmlDateTimeSerializationMode.RoundtripKind);
			}
			else
			{
				Timestamp = DateTime.Now;
			}

			m_random = m_oDictionary.GetRandomNumber();
		}

		/// <summary>
		/// Reads the style from DOM.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-11-07</remarks>
		private void ReadStyleFromDOM()
		{
			long lTicks = DateTime.Now.Ticks;
			//System.Diagnostics.Trace.TraceInformation(@"Read card style (id={0}) - {1}", this.Id, DateTime.Now.Ticks - lTicks);
			//System.Diagnostics.Trace.Indent();
			XmlElement xeStyle = m_card[m_XPath_Style];
			if (xeStyle != null)
			{
				if (xeStyle.HasChildNodes)
				{
					XmlReader xmlReader = new XmlNodeReader(xeStyle);
					if (StyleSerializer.CanDeserialize(xmlReader))
					{
						m_Style = (XmlCardStyle)StyleSerializer.Deserialize(xmlReader);
						(m_Style as XmlCardStyle).Parent = Parent;
						//System.Diagnostics.Trace.TraceInformation(@"did it - {0}", DateTime.Now.Ticks - lTicks);
					}
				}
			}
			//System.Diagnostics.Trace.TraceInformation(@"finished - {0}", DateTime.Now.Ticks - lTicks);
			//System.Diagnostics.Trace.Unindent();
		}

		/// <summary>
		/// Saves the style to the DOM.
		/// </summary>
		/// <param name="style">The style.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-10-30</remarks>
		private bool SaveStyleToDOM(ICardStyle style)
		{
			bool success = false;
			StringBuilder stringBuilder = new StringBuilder();
			TextWriter stringWriter = new StringWriter(stringBuilder);
			try
			{
				StyleSerializer.Serialize(stringWriter, style);
				XmlElement xeStyle = m_card[m_XPath_Style];
				if (xeStyle != null)
				{
					m_card.RemoveChild(xeStyle);
				}
				XmlDocument xdStyle = new XmlDocument();
				xdStyle.LoadXml(stringBuilder.ToString());
				m_card.AppendChild(m_dictionary.ImportNode(xdStyle.DocumentElement, true));
				success = true;
			}
			catch { }
			return success;
		}

		/// <summary>
		/// Copy the file to media folder.
		/// </summary>
		/// <param name="media">The media object.</param>
		/// <param name="side">The side.</param>
		/// <returns>True if success.</returns>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		private bool CopyFileToMediaFolder(IMedia media, Side side)
		{
			if (media != null)
			{
				if (media.Filename.Trim().Length > 0)
				{
					string srcPath, dstPath, relPath, mediaDir, dictionaryDir, filename, fileNameStrict;
					filename = Path.GetFileName(media.Filename);
					if ((filename != null) && (filename.Length > 0))
					{
						dictionaryDir = Path.GetDirectoryName(m_oDictionary.Path);
						System.IO.Directory.SetCurrentDirectory(dictionaryDir);
						srcPath = Path.Combine(Path.GetDirectoryName(media.Filename), filename);
						mediaDir = Path.Combine(dictionaryDir, m_oDictionary.MediaDirectory);
						relPath = Path.Combine(m_oDictionary.MediaDirectory, filename);
						dstPath = Path.Combine(mediaDir, filename);

						if (!Path.IsPathRooted(srcPath))
						{
							if (srcPath.ToLower().Equals(relPath.ToLower()))
								return true;    // file is already in the media dir
						}
						if (File.Exists(srcPath))
						{
							if (!Directory.Exists(mediaDir))
								Directory.CreateDirectory(mediaDir);
							if (srcPath != dstPath)
							{
								fileNameStrict = GetMediaFileName(media, side, mediaDir, this.Dictionary.UnicodeFilenames, false);
								dstPath = Path.Combine(Path.GetDirectoryName(dstPath), fileNameStrict + Path.GetExtension(dstPath));
								relPath = Path.Combine(m_oDictionary.MediaDirectory, fileNameStrict + Path.GetExtension(dstPath));
								try
								{
									CheckAndCreateMediaFileName(ref dstPath, ref relPath);
								}
								catch (NoValidMediaFileName)
								{
									fileNameStrict = GetMediaFileName(media, side, mediaDir, this.Dictionary.UnicodeFilenames, true);
									dstPath = Path.Combine(Path.GetDirectoryName(dstPath), fileNameStrict + Path.GetExtension(dstPath));
									relPath = Path.Combine(m_oDictionary.MediaDirectory, fileNameStrict + Path.GetExtension(dstPath));
									CheckAndCreateMediaFileName(ref dstPath, ref relPath);
								}
								File.Copy(srcPath, dstPath, true);
							}
							media.Filename = relPath;
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Checks and creates a valid media file name.
		/// </summary>
		/// <param name="dstPath">The DST path.</param>
		/// <param name="relPath">The rel path.</param>
		/// <remarks>Documented by Dev03, 2007-09-07</remarks>
		private void CheckAndCreateMediaFileName(ref string dstPath, ref string relPath)
		{
			if (File.Exists(dstPath))
			{
				string newDstFile;
				string dstPathWithoutExt = Path.Combine(Path.GetDirectoryName(dstPath), Path.GetFileNameWithoutExtension(dstPath));
				string dstFileExt = Path.GetExtension(dstPath);
				int fileCounter = 0;
				do
				{
					newDstFile = dstPathWithoutExt + "_" + fileCounter.ToString("00") + dstFileExt;
					if (fileCounter++ >= 100)
					{
						throw new NoValidMediaFileName(dstPathWithoutExt + dstFileExt);
					}
				} while (File.Exists(newDstFile));
				dstPath = newDstFile;
				relPath = Path.Combine(m_oDictionary.MediaDirectory, Path.GetFileName(newDstFile));
			}
		}

		/// <summary>
		/// Gets the name of the media file.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="side">The side.</param>
		/// <param name="mediaDir">The media dir.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-11-30</remarks>
		private string GetMediaFileName(IMedia media, Side side, string mediaDir)
		{
			return GetMediaFileName(media, side, mediaDir, true, false);
		}

		/// <summary>
		/// Gets the name of the media file.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="side">The side.</param>
		/// <param name="mediaDir">The media dir.</param>
		/// <param name="unicodeAllowed">if set to <c>true</c> unicode characters are allowed as part of the filename.</param>
		/// <param name="forceSimpleFilename">if set to <c>true</c> only simple filenames are allowed (e.g. question_101).</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-11-30</remarks>
		private string GetMediaFileName(IMedia media, Side side, string mediaDir, bool unicodeAllowed, bool forceSimpleFilename)
		{
			string fileNameFormat = m_MediaFileFormatString;
			string fileName = String.Empty;
			string langPrefix = "NA";
			string typePrefix = String.Empty;
			string cardText = String.Empty;

			if (side == Side.Question)
			{
				if (Question.Culture != null)
					langPrefix = Question.Culture.TwoLetterISOLanguageName.ToUpper();
			}
			else
			{
				if (Answer.Culture != null)
					langPrefix = Answer.Culture.TwoLetterISOLanguageName.ToUpper();
			}

			if (side == Side.Question)
				cardText = Question.ToString();
			else
				cardText = Answer.ToString();
			if (unicodeAllowed)
				cardText = m_RegExFileNameCleaner.Replace(cardText, String.Empty).ToLower().Trim();
			else
				cardText = m_RegExStrictFileNameCleaner.Replace(cardText, String.Empty).ToLower().Trim();
			if (forceSimpleFilename || (cardText.Length == 0) || (cardText.IndexOfAny(Path.GetInvalidFileNameChars()) > -1))
			{
				cardText = side.ToString().ToLower() + Id.ToString();
			}
			else if (cardText.Length > 15)
			{
				cardText = cardText.Substring(0, 15);
			}

			if ((media.MediaType == EMedia.Audio) && ((IAudio)media).Example.GetValueOrDefault()) fileNameFormat = m_ExampleMediaFileFormatString;

			switch (media.MediaType)
			{
				case EMedia.Image:
					typePrefix = "IMG";
					break;
				default:
					typePrefix = media.MediaType.ToString().ToUpper().Substring(0, 3);
					break;
			}

			fileName = String.Format(fileNameFormat, langPrefix, cardText, typePrefix);
			return fileName;
		}

		private void DeleteFile(IMedia media, Side side)
		{
			//is there more than one reference for this file?
			if (m_card.SelectNodes(String.Format("//text()='{0}'", media.Filename)).Count == 1)
			{
				DeleteFileFromMediaFolder(media);
			}
		}

		/// <summary>
		/// Deletes the file from media folder.
		/// </summary>
		/// <param name="media">The media object.</param>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		private void DeleteFileFromMediaFolder(IMedia media)
		{
			if (Path.IsPathRooted(media.Filename))
			{
				//only delete if it belongs to the dictionary folder
				if (media.Filename.StartsWith(Path.GetDirectoryName(m_oDictionary.Path)))
				{
					if (File.Exists(media.Filename))
						File.Delete(media.Filename);
				}
			}
			else
			{
				string fullPath = Path.Combine(Path.GetDirectoryName(m_oDictionary.Path), media.Filename);
				if (File.Exists(fullPath))
					File.Delete(fullPath);
			}
		}

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2009-04-19</remarks>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(ICard), progressDelegate);

			//copy media objects
			ICard targetCard = target as ICard;
			if (targetCard != null)
			{
				foreach (IMedia media in QuestionMedia)
					try
					{
						targetCard.AddMedia(media, Side.Question);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex, "DbCard.AddMedia() throws an exception.");
					}
				foreach (IMedia media in AnswerMedia)
					try
					{
						targetCard.AddMedia(media, Side.Answer);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex, "DbCard.AddMedia() throws an exception.");
					}
			}
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2009-04-19</remarks>
		public ParentClass Parent { get { return parent; } }

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
			return true;
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			return new List<SecurityFramework.PermissionInfo>();
		}

		#endregion
	}
}
