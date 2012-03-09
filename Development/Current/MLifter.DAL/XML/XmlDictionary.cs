using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Properties;
using MLifter.DAL.Tools;
using System.Net;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents a dictionary including settings, chapters, cards and statistics.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-28</remarks>
	public class XmlDictionary : IDictionary
	{
		private bool isDisposed = false;

		private bool m_UnicodeFilenames = false;    //unicode filenames allowed

		private BackgroundWorker m_BackgroundWorker = null;
		private FileStream m_FileStreamRead = null;
		private FileStream m_FileStreamWrite = null;
		private string m_TempFile = string.Empty;
		private AccessMode accessMode = AccessMode.ForLearning;

		private XmlDocument m_dictionary = new XmlDocument();
		private XmlElement m_generalSettings, m_userSettings;
		private List<string> m_audioComments;
		private List<int> m_queryChapters;
		private IBoxes m_Boxes;
		private MLifter.DAL.Interfaces.IQueryOptions m_queryOptions;
		private MLifter.DAL.Interfaces.ICards m_cards;
		private MLifter.DAL.Interfaces.IChapters m_chapters;
		private MLifter.DAL.Interfaces.IStatistics m_statistics;
		private MLifter.DAL.Interfaces.IQueryType m_AllowedQueryTypes;
		private MLifter.DAL.Interfaces.IQueryDirections m_AllowedQueryDirections;
		private string m_path;
		private ICardStyle m_Style = null;

		/// <summary>
		/// The number of boxes.
		/// </summary>
		public const int m_numberOfBoxes = 10;
		private int m_CurrentVersion = 0x10B;   // current dictionary version

		private Random m_random = new Random();

		private const string m_xpathUsrSet = "/dictionary/user";
		private const string m_xpathGenSet = "/dictionary/general";
		private const string m_xpathId = "id";
		private const string m_xpathAttributeId = "@id";
		private const string m_xpathIdFilter = "[@id={0}]";
		private const string m_xpathVersion = "version";
		private const string m_xpathAuthor = "author";
		private const string m_xpathDescription = "description";
		private const string m_xpathLogo = "logo";
		private const string m_xpathEncryption = "encryption";
		private const string m_xpathEncryptedVersion = "encryptedVersion";
		private const string m_xpathCategory = "category";
		private const string m_xpathQuestionCaption = "questioncaption";
		private const string m_xpathAnswerCaption = "answercaption";
		private const string m_xpathQuestionCulture = "questionculture";
		private const string m_xpathAnswerCulture = "answerculture";
		private const string m_xpathMediaDirectory = "sounddir";
		private const string m_xpathScore = "score";
		private const string m_xpathHiScore = "hiscore";
		private const string m_xpathEmptyMessage = "emptymessage";
		private const string m_xpathUseDicionaryStyle = "usedicstyle";
		private const string m_xpathCommentSound = "commentsound";
		private const string m_xpathBoxsize = "boxsize";
		private const string m_xpathQueryChapter = "querychapter";
		private const string m_XPath_Style = "cardStyle";

		private const string m_xpathResourceFilter = "//logo[text() != '']/text()|//commentsound[text() != '']/text()|//questionaudio[text() != '']/text()|//questionexampleaudio[text() != '']/text()|//answeraudio[text() != '']/text()|//answerexampleaudio[text() != '']/text()|//questionvideo[text() != '']/text()|//answervideo[text() != '']/text()|//questionimage[text() != '']/text()|//answerimage[text() != '']/text()|//unusedmedia[text() != '']/text()|//questionstylesheet[text() != '']/text()|//answerstylesheet[text() != '']/text()";
		private const string m_xpathTextNodeFilter = "//text()[.='{0}']";
		private Regex m_ResourceFinder = new Regex(@"url\((?<url>[^\)]+)\)");   //used to filter for css resources

		private readonly string m_dictionaryTemplate = Resources.BlankDictionaryV2_0;

		private XmlSerializer m_StyleSerializer = null;

		/// <summary>
		/// This defines the modes for the stream.
		/// </summary>
		private enum Mode
		{
			/// <summary>
			/// Stream is for reading.
			/// </summary>
			ForReading,
			/// <summary>
			/// Stream is for writing.
			/// </summary>
			ForWriting
		}

		/// <summary>
		/// Defines in which mode the Xml dictionary is opened.
		/// </summary>
		public enum AccessMode
		{
			/// <summary>
			/// Preview mode (read-only, no file lock!).
			/// </summary>
			ForPreview,
			/// <summary>
			/// Learning mode.
			/// </summary>
			ForLearning
		}

		/// <summary>
		/// Defines the possible encryption modes.
		/// </summary>
		public enum EncryptionMode
		{
			/// <summary>
			/// No encryption.
			/// </summary>
			None,
			/// <summary>
			/// Stick encryption.
			/// </summary>
			StickEncryption
		}

		/// <summary>
		/// Gets the file stream.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private FileStream GetFileStream(Mode mode)
		{
			OpenStream(mode);
			if (mode == Mode.ForWriting)
				return m_FileStreamWrite;
			else
				return m_FileStreamRead;
		}

		/// <summary>
		/// Opens the stream.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private void OpenStream(Mode mode)
		{
			if (accessMode == AccessMode.ForPreview)
			{
				m_FileStreamRead = new FileStream(m_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				return;
			}
			if (mode == Mode.ForWriting)
			{
				SetTempFile();
				if (System.IO.File.Exists(m_TempFile))
				{
					CloseStream(mode);
					m_FileStreamWrite = new FileStream(m_TempFile, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Read);
				}
				else
				{
					CloseStream(mode);
					m_FileStreamWrite = new FileStream(m_TempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
				}
			}
			else
			{
				CloseStream(mode);
				m_FileStreamRead = new FileStream(m_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			}
		}

		/// <summary>
		/// Sets the path for the temp file.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private void SetTempFile()
		{
			m_TempFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_path), "~$" + System.IO.Path.GetFileName(m_path));
		}

		/// <summary>
		/// Deletes the temp file.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private void DeleteTempFile()
		{
			try
			{
				if ((m_TempFile.Length > 0) && (System.IO.File.Exists(m_TempFile)))
					System.IO.File.Delete(m_TempFile);
			}
			catch { }
		}

		internal FileCleanupQueue FileCleanupQueue = new FileCleanupQueue();

		/// <summary>
		/// Closes the stream.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private void CloseStream()
		{
			CloseStream(Mode.ForReading);
			CloseStream(Mode.ForWriting);
		}

		/// <summary>
		/// Closes the stream for saving or reading.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private void CloseStream(Mode mode)
		{
			if (mode == Mode.ForWriting)
			{
				if (m_FileStreamWrite != null)
				{
					try
					{
						m_FileStreamWrite.Close();
						m_FileStreamWrite.Dispose();
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.TraceWarning("XmlDictionary - failed to close stream ({0})", ex.Message);
					}
					m_FileStreamWrite = null;
				}
			}
			else
			{
				if (m_FileStreamRead != null)
				{
					try
					{
						m_FileStreamRead.Close();
						m_FileStreamRead.Dispose();
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.TraceWarning("XmlDictionary - failed to close stream ({0})", ex.Message);
					}
					m_FileStreamRead = null;
				}
			}
		}

		/// <summary>
		/// Saves the dictionary.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		private void SaveDictionary()
		{
			m_dictionary.PreserveWhitespace = false;
			//save to a temporary file
			m_dictionary.Save(GetFileStream(Mode.ForWriting));
			CloseStream(Mode.ForReading);
			//if it was successfull we delete the original and replace it with the saved
			System.IO.File.Delete(m_path);
			CloseStream(Mode.ForWriting);
			System.IO.File.Move(m_TempFile, m_path);
			//re-open the stream to create a file lock
			OpenStream(Mode.ForReading);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDictionary"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="ignoreOldVersion">if set to <c>true</c> [ignore old version].</param>
		/// <param name="user">The user.</param>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		internal XmlDictionary(string path, bool ignoreOldVersion, IUser user)
			: this(path, ignoreOldVersion, AccessMode.ForLearning, user)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlDictionary"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="ignoreOldVersion">if set to <c>true</c> [ignore old version].</param>
		/// <param name="accessMode">The access mode.</param>
		/// <param name="user">The user.</param>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		internal XmlDictionary(string path, bool ignoreOldVersion, AccessMode accessMode, IUser user)
		{
			m_path = path;
			this.accessMode = accessMode;

			parent = new ParentClass(user, this);

			//ensure that the path does not get saved as relative (Environment.CurrentDirectory may change)
			if (!System.IO.Path.IsPathRooted(m_path))
				m_path = System.IO.Path.Combine(Environment.CurrentDirectory, m_path);

			if (File.Exists(m_path))
			{
				m_dictionary.Load(GetFileStream(Mode.ForReading));
			}
			else
			{

				m_dictionary.LoadXml(m_dictionaryTemplate);
				m_dictionary.DocumentElement.Attributes[m_xpathId].Value = System.Guid.NewGuid().ToString();   //create a new GUID for this dictionary
			}

			Initialize(ignoreOldVersion);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="XmlDictionary"/> is reclaimed by garbage collection.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		~XmlDictionary()
		{
			Dispose(false);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (Parent != null)
					Parent.OnDictionaryClosed(this, EventArgs.Empty);

				//close the stream if still open
				CloseStream();
				if (disposing)
				{
					//and delete any temporary file
					DeleteTempFile();
					FileCleanupQueue.DoCleanup();
				}
			}
			isDisposed = true;
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>The dictionary.</value>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		internal XmlDocument Dictionary
		{
			get
			{
				return this.m_dictionary;
			}
		}

		/// <summary>
		/// Gets the style serializer.
		/// </summary>
		/// <value>The style serializer.</value>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		internal XmlSerializer StyleSerializer
		{
			get
			{
				if (m_StyleSerializer == null)
					m_StyleSerializer = new XmlSerializer(typeof(XmlCardStyle));
				return m_StyleSerializer;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [unicode filenames].
		/// </summary>
		/// <value><c>true</c> if [unicode filenames]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool UnicodeFilenames
		{
			get { return m_UnicodeFilenames; }
			set { m_UnicodeFilenames = value; }
		}

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Path
		{
			get
			{
				return m_path;
			}
			set
			{
				Move(value);
			}
		}

		#region IDictionary Members

		/// <summary>
		/// Gets a value indicating whether this instance is DB.
		/// </summary>
		/// <value><c>true</c> if this instance is DB; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-08-22</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool IsDB
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// If !string.emty --> Can open LM --> else --> Can't open LM.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public string Password
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the background worker.
		/// </summary>
		/// <value>The background worker.</value>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		/// <remarks>Documented by Dev03, 2008-02-05</remarks>
		public BackgroundWorker BackgroundWorker
		{
			get
			{
				return m_BackgroundWorker;
			}
			set
			{
				m_BackgroundWorker = value;
			}
		}

		/// <summary>
		/// Occurs when [XML progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event StatusMessageEventHandler XmlProgressChanged;

		/// <summary>
		/// Occurs when [move progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event StatusMessageEventHandler MoveProgressChanged;

		/// <summary>
		/// Occurs when [save as progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event StatusMessageEventHandler SaveAsProgressChanged;

		/// <summary>
		/// Occurs when [create media progress changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event StatusMessageEventHandler CreateMediaProgressChanged;

		/// <summary>
		/// Gets the number of boxes.
		/// </summary>
		/// <value>The number of boxes.</value>
		/// <remarks>Documented by Dev03, 2007-10-17</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int NumberOfBoxes
		{
			get
			{
				return m_numberOfBoxes;
			}
		}

		/// <summary>
		/// Gets the dictionary as Xml.
		/// </summary>
		/// <value>The Xml.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Xml
		{
			get
			{
				return this.m_dictionary.OuterXml;
			}
		}

		/// <summary>
		/// Gets the connection string (could conatain a path or a db connection string).
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev03, 2007-10-17</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Connection
		{
			get
			{
				return m_path;
			}
		}

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int Version
		{
			get
			{
				try
				{
					if (Encryption == EncryptionMode.StickEncryption)
						throw new DictionaryNotDecryptedException();
					else
						return XmlConvert.ToInt32(m_generalSettings[m_xpathVersion].InnerText);
				}
				catch (Exception exp)
				{
					if (exp is DictionaryNotDecryptedException)
						throw;
					else
						throw new InvalidDictionaryException();
				}
			}
			set
			{
				if (m_generalSettings[m_xpathVersion] != null)
					m_generalSettings[m_xpathVersion].InnerText = XmlConvert.ToString(Encryption == EncryptionMode.StickEncryption ? value + 1 : value);
				else
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathVersion, XmlConvert.ToString(Encryption == EncryptionMode.StickEncryption ? value + 1 : value));

				if (Encryption == EncryptionMode.StickEncryption)
					throw new DictionaryNotDecryptedException();
				else if (m_generalSettings[m_xpathVersion] != null)
					m_generalSettings[m_xpathVersion].RemoveAttribute(m_xpathEncryptedVersion);
			}
		}

		/// <summary>
		/// Gets or sets the encryption mode.
		/// </summary>
		/// <value>The encryption.</value>
		/// <remarks>Documented by Dev02, 2008-09-16</remarks>
		public EncryptionMode Encryption
		{
			get
			{
				try
				{
					if (m_dictionary.DocumentElement.Attributes[m_xpathEncryption] != null)
						return (EncryptionMode)Enum.Parse(typeof(EncryptionMode), m_dictionary.DocumentElement.Attributes[m_xpathEncryption].Value, true);
				}
				catch (ArgumentException)
				{ }

				return EncryptionMode.None;
			}
			set
			{
				if (value != Encryption)
				{
					if (value == EncryptionMode.None)
					{
						if (m_dictionary.DocumentElement.Attributes[m_xpathEncryption] != null)
							m_dictionary.DocumentElement.RemoveAttribute(m_xpathEncryption);
					}
					else
					{
						m_dictionary.DocumentElement.SetAttribute(m_xpathEncryption, value.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Defines whether the content of the LM is protected from being copied/extracted.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool ContentProtected
		{
			get { return Encryption != EncryptionMode.None; }
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Title
		{
			get
			{
				return System.IO.Path.GetFileNameWithoutExtension(Path);
			}
			set
			{
				Debug.WriteLine("The method or operation is not implemented.");
			}
		}


		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Author
		{
			get
			{
				if (m_generalSettings[m_xpathAuthor] != null)
				{
					return m_generalSettings[m_xpathAuthor].InnerText;
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				if (m_generalSettings[m_xpathAuthor] != null)
				{
					m_generalSettings[m_xpathAuthor].InnerText = value;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathAuthor, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Description
		{
			get
			{
				if (m_generalSettings[m_xpathDescription] != null)
				{
					return m_generalSettings[m_xpathDescription].InnerText;
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				if (m_generalSettings[m_xpathDescription] != null)
				{
					m_generalSettings[m_xpathDescription].InnerText = value;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathDescription, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the logo.
		/// </summary>
		/// <value>The logo.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Logo
		{
			get
			{
				if (m_generalSettings[m_xpathLogo] != null)
				{
					return m_generalSettings[m_xpathLogo].InnerText;
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				if (m_generalSettings[m_xpathLogo] != null)
				{
					m_generalSettings[m_xpathLogo].InnerText = value;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathLogo, value);
				}
			}
		}

		private int id = 0;
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>The ID.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int Id
		{
			get
			{
				return id;
			}
			internal set
			{
				id = value;
			}
		}

		/// <summary>
		/// Gets or sets the GUID.
		/// </summary>
		/// <value>The GUID.</value>
		/// <remarks>Documented by Dev02, 2008-07-28</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string Guid
		{
			get
			{

				if (m_dictionary.DocumentElement.Attributes[m_xpathId] != null)
					return m_dictionary.DocumentElement.Attributes[m_xpathId].Value;
				else
					return String.Empty;
			}
			set
			{
				if (m_dictionary.DocumentElement.Attributes[m_xpathId] != null)
					m_dictionary.DocumentElement.Attributes[m_xpathId].Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the category.
		/// </summary>
		/// <value>The category.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public Category Category
		{
			get
			{
				try
				{
					XmlElement categoryNode = (XmlElement)m_generalSettings[m_xpathCategory];
					if (categoryNode.Attributes[m_xpathId] != null)
					{
						if (XmlConvert.ToInt32(categoryNode.Attributes[m_xpathId].Value) < 0)
							return new MLifter.DAL.Category(XmlConvert.ToInt32(categoryNode.InnerText), false);
						else
							return new MLifter.DAL.Category(XmlConvert.ToInt32(categoryNode.Attributes[m_xpathId].Value), true);
					}
					else
					{
						categoryNode.SetAttribute(m_xpathId, XmlConvert.ToString(-1));
						return new MLifter.DAL.Category(XmlConvert.ToInt32(categoryNode.InnerText), false);
					}
				}
				catch (Exception)
				{
					throw new InvalidDictionaryException();
				}
			}
			set
			{
				int newId = (value.Converted) ? value.Id : -1;
				XmlElement categoryNode = (XmlElement)m_generalSettings[m_xpathCategory];
				if (categoryNode != null)
				{
					categoryNode.SetAttribute(m_xpathId, XmlConvert.ToString(newId));
					categoryNode.InnerText = XmlConvert.ToString(value.OldId);
				}
				else
				{
					XmlHelper.CreateElementWithAttribute(m_generalSettings, m_xpathCategory, XmlConvert.ToString(value.OldId), m_xpathId, XmlConvert.ToString(newId));
				}
			}
		}

		/// <summary>
		/// Gets or sets the question caption.
		/// </summary>
		/// <value>The question caption.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string QuestionCaption
		{
			get
			{
				if (m_generalSettings[m_xpathQuestionCaption] != null)
				{
					return m_generalSettings[m_xpathQuestionCaption].InnerText;
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				if (m_generalSettings[m_xpathQuestionCaption] != null)
				{
					m_generalSettings[m_xpathQuestionCaption].InnerText = value;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathQuestionCaption, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the answer caption.
		/// </summary>
		/// <value>The answer caption.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string AnswerCaption
		{
			get
			{
				if (m_generalSettings[m_xpathAnswerCaption] != null)
				{
					return m_generalSettings[m_xpathAnswerCaption].InnerText;
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				if (m_generalSettings[m_xpathAnswerCaption] != null)
				{
					m_generalSettings[m_xpathAnswerCaption].InnerText = value;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathAnswerCaption, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the question culture.
		/// </summary>
		/// <value>The question culture.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public CultureInfo QuestionCulture
		{
			get
			{
				CultureInfo ci;
				if (m_generalSettings[m_xpathQuestionCulture] != null)
				{
					try
					{
						ci = new CultureInfo(m_generalSettings[m_xpathQuestionCulture].InnerText);
					}
					catch (Exception ex)
					{
						Trace.TraceError("Failed to read question culture: {0}", ex.Message);
						ci = System.Threading.Thread.CurrentThread.CurrentCulture;
					}
				}
				else
				{
					ci = System.Threading.Thread.CurrentThread.CurrentCulture;
				}
				return ci;
			}
			set
			{
				if (m_generalSettings[m_xpathQuestionCulture] != null)
				{
					m_generalSettings[m_xpathQuestionCulture].InnerText = value.Name;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathQuestionCulture, value.Name);
				}
			}
		}

		/// <summary>
		/// Gets or sets the answer culture.
		/// </summary>
		/// <value>The answer culture.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public CultureInfo AnswerCulture
		{
			get
			{
				CultureInfo ci;
				if (m_generalSettings[m_xpathAnswerCulture] != null)
				{
					try
					{
						ci = new CultureInfo(m_generalSettings[m_xpathAnswerCulture].InnerText);
					}
					catch (Exception ex)
					{
						Trace.TraceError("Failed to read question culture: {0}", ex.Message);
						ci = System.Threading.Thread.CurrentThread.CurrentCulture;
					}
				}
				else
				{
					ci = System.Threading.Thread.CurrentThread.CurrentCulture;
				}
				return ci;
			}
			set
			{
				if (m_generalSettings[m_xpathAnswerCulture] != null)
				{
					m_generalSettings[m_xpathAnswerCulture].InnerText = value.Name;
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_generalSettings, m_xpathAnswerCulture, value.Name);
				}
			}
		}

		/// <summary>
		/// Gets or sets the media directory.
		/// </summary>
		/// <value>The media directory.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public string MediaDirectory
		{
			get
			{
				string mediaPath = m_generalSettings[m_xpathMediaDirectory].InnerText;
				return mediaPath;
			}
			set
			{
				if (!Directory.Exists(value) && !Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Path), value)))
					throw new DirectoryNotFoundException();

				string mediaPath = value;
				if (System.IO.Path.IsPathRooted(value))
				{
					string dicFolderPath = System.IO.Path.GetDirectoryName(this.Path);
					if (value.StartsWith(dicFolderPath))
					{
						mediaPath = value.Replace(dicFolderPath, String.Empty);
					}
				}
				m_generalSettings[m_xpathMediaDirectory].InnerText = mediaPath.Trim(new char[] { System.IO.Path.DirectorySeparatorChar, ' ' });
			}
		}

		/// <summary>
		/// Gets the size of the dictionary.
		/// </summary>
		/// <value>The size of the dictionary.</value>
		/// <remarks>Documented by Dev08, 2008-10-02</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public long DictionarySize
		{
			get
			{
				long space = 0;
				if (File.Exists(Path))
				{
					space += new FileInfo(Path).Length;
				}
				foreach (string resource in this.GetResources())
				{
					if (File.Exists(resource))
					{
						space += new FileInfo(resource).Length;
					}
				}
				return space;
			}
		}

		/// <summary>
		/// Gets the number of all dictionary media objects/files.
		/// </summary>
		/// <value>The dictionary media objects count.</value>
		/// <remarks>Documented by Dev08, 2008-10-02</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public int DictionaryMediaObjectsCount
		{
			get
			{
				int count = 1;
				foreach (string resource in this.GetResources())
				{
					if (File.Exists(resource))
						count++;
				}
				return count;
			}
		}

		/// <summary>
		/// Gets or sets the comment audio.
		/// </summary>
		/// <value>The comment audio.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public List<string> CommentAudio
		{
			get
			{
				return GetCommentarySounds();
			}
			set
			{
				if (m_audioComments.Count > value.Count)
				{
					if (value.Count > 0)
						m_audioComments.RemoveRange(value.Count - 1, m_audioComments.Count - value.Count);
					else
						m_audioComments.Clear();
				}
				for (int i = 0; i < value.Count; i++)
				{
					string commentarySound = AddCommentaryAudioToMediaFolder(value[i], Helper.CommentarySoundNames[i]);
					if (i < m_audioComments.Count)
						m_audioComments[i] = commentarySound;
					else
						m_audioComments.Add(commentarySound);
				}
			}
		}

		internal int score
		{
			get
			{
				try
				{
					int sc = XmlConvert.ToInt32(m_userSettings[m_xpathScore].InnerText);

					int aScore = 0;
					for (int i = 1; i < Boxes.Box.Count; i++)
						aScore += Boxes.Box[i].Size * (i - 1);

					if (aScore != sc)
						score = aScore;

					return aScore;
				}
				catch
				{
					return 0;
				}
			}
			set
			{
				if (m_userSettings[m_xpathScore] != null)
				{
					m_userSettings[m_xpathScore].InnerText = XmlConvert.ToString(value);
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_userSettings, m_xpathScore, XmlConvert.ToString(value));
				}
			}
		}
		/// <summary>
		/// Gets actual the score.
		/// </summary>
		/// <value>The score.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public double Score { get { return score; } }

		/// <summary>
		/// Gets or sets the high score.
		/// </summary>
		/// <value>The high score.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public double HighScore
		{
			get
			{
				try
				{
					return XmlConvert.ToInt32(m_userSettings[m_xpathHiScore].InnerText);
				}
				catch
				{
					return 0;
				}
			}
			set
			{
				if (m_userSettings[m_xpathHiScore] != null)
				{
					m_userSettings[m_xpathHiScore].InnerText = XmlConvert.ToString(value);
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_userSettings, m_xpathHiScore, XmlConvert.ToString(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the allowed query types.
		/// </summary>
		/// <value>The allowed query types.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IQueryType AllowedQueryTypes
		{
			get
			{
				return m_AllowedQueryTypes;
			}
			set
			{
				m_AllowedQueryTypes.ImageRecognition = value.ImageRecognition;
				m_AllowedQueryTypes.ListeningComprehension = value.ListeningComprehension;
				m_AllowedQueryTypes.MultipleChoice = value.MultipleChoice;
				m_AllowedQueryTypes.Sentence = value.Sentence;
				m_AllowedQueryTypes.Word = value.Word;
			}
		}

		/// <summary>
		/// Gets or sets the allowed query directions.
		/// </summary>
		/// <value>The allowed query directions.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IQueryDirections AllowedQueryDirections
		{
			get
			{
				return m_AllowedQueryDirections;
			}
			set
			{
				m_AllowedQueryDirections.Answer2Question = value.Answer2Question;
				m_AllowedQueryDirections.Question2Answer = value.Question2Answer;
				m_AllowedQueryDirections.Mixed = value.Mixed;
			}
		}

		/// <summary>
		/// Gets or sets the query options.
		/// </summary>
		/// <value>The query options.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IQueryOptions QueryOptions
		{
			get
			{
				return m_queryOptions;
			}
			set
			{
				m_queryOptions.AutoplayAudio = value.AutoplayAudio;
				m_queryOptions.CaseSensitive = value.CaseSensitive;
				m_queryOptions.ConfirmDemote = value.ConfirmDemote;
				m_queryOptions.CorrectOnTheFly = value.CorrectOnTheFly;
				m_queryOptions.EnableCommentary = value.EnableCommentary;
				m_queryOptions.EnableTimer = value.EnableTimer;
				m_queryOptions.GradeSynonyms = value.GradeSynonyms;
				m_queryOptions.GradeTyping = value.GradeTyping;
				m_queryOptions.QueryDirection = value.QueryDirection;
				m_queryOptions.QueryTypes = value.QueryTypes;
				m_queryOptions.RandomPool = value.RandomPool;
				m_queryOptions.SelfAssessment = value.SelfAssessment;
				m_queryOptions.ShowImages = value.ShowImages;
				m_queryOptions.ShowStatistics = value.ShowStatistics;
				m_queryOptions.SkipCorrectAnswers = value.SkipCorrectAnswers;
				m_queryOptions.SnoozeOptions = value.SnoozeOptions;
				m_queryOptions.StripChars = value.StripChars;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [empty message].
		/// </summary>
		/// <value><c>true</c> if [empty message]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool EmptyMessage
		{
			get
			{
				if (m_userSettings[m_xpathEmptyMessage] != null)
				{
					return (m_userSettings[m_xpathEmptyMessage].InnerText.ToLower() == bool.TrueString.ToLower());
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (m_userSettings[m_xpathEmptyMessage] != null)
				{
					m_userSettings[m_xpathEmptyMessage].InnerText = value.ToString().ToLower();
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_userSettings, m_xpathEmptyMessage, value.ToString().ToLower());
				}
			}
		}

		/// <summary>
		/// Gets the boxes.
		/// </summary>
		/// <value>The boxes.</value>
		/// <remarks>Documented by Dev03, 2007-11-22</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IBoxes Boxes
		{
			get
			{
				return m_Boxes;
			}
		}

		/// <summary>
		/// Gets the query chapter.
		/// </summary>
		/// <value>The query chapter.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IList<int> QueryChapter
		{
			get
			{
				BindingList<int> bindingQueryChapters = new BindingList<int>(m_queryChapters);
				bindingQueryChapters.ListChanged += new ListChangedEventHandler(bindingQueryChapters_ListChanged);
				return bindingQueryChapters;
			}
		}

		void bindingQueryChapters_ListChanged(object sender, ListChangedEventArgs e)
		{
			((XmlBoxes)Boxes).Update();
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use dictionary style].
		/// </summary>
		/// <value><c>true</c> if [use dictionary style]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool UseDictionaryStyle
		{
			get
			{
				if (m_userSettings[m_xpathUseDicionaryStyle] != null)
				{
					return (m_userSettings[m_xpathUseDicionaryStyle].InnerText.ToLower() == bool.TrueString.ToLower());
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (m_userSettings[m_xpathUseDicionaryStyle] != null)
				{
					m_userSettings[m_xpathUseDicionaryStyle].InnerText = value.ToString().ToLower();
				}
				else
				{
					XmlHelper.CreateAndAppendElement(m_userSettings, m_xpathUseDicionaryStyle, value.ToString().ToLower());
				}
			}
		}

		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>The style.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
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
		/// Gets or sets the cards.
		/// </summary>
		/// <value>The cards.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ICards Cards
		{
			get
			{
				return m_cards;
			}
		}

		/// <summary>
		/// Gets or sets the chapters.
		/// </summary>
		/// <value>The chapters.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IChapters Chapters
		{
			get
			{
				return m_chapters;
			}
		}

		/// <summary>
		/// Gets or sets the statistics.
		/// </summary>
		/// <value>The statistics.</value>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IStatistics Statistics
		{
			get
			{
				return m_statistics;
			}
		}

		/// <summary>
		/// Loads this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void Load()
		{
		}

		/// <summary>
		/// Saves this dictionary.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-03</remarks>
		public void Save()
		{
			this.Version = m_CurrentVersion;

			string dstPath = System.IO.Path.GetDirectoryName(m_path);
			if (!System.IO.Directory.Exists(dstPath))
				System.IO.Directory.CreateDirectory(dstPath);

			FlushListsToXml();
			//this.Statistics.Save();
			SaveDictionary();
		}

		/// <summary>
		/// Gets the resources.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-13</remarks>
		public List<string> GetResources()
		{
			List<string> resources = new List<string>();
			foreach (XmlNode resource in m_dictionary.SelectNodes(m_xpathResourceFilter))
			{
				if (!resources.Exists(delegate(string res) { return (res == resource.Value); }))
				{
					resources.Add(resource.Value);
				}
			}
			foreach (ICard card in Cards.Cards)
			{
				if (card.Settings.Style == null)
					continue;
				string css = card.Settings.Style.ToString();
				Match m = m_ResourceFinder.Match(css);
				while (m.Success)
				{
					string resource = m.Groups["url"].Value.Trim(new char[] { '"', '\'' });
					if (!resources.Exists(delegate(string res) { return (res == resource); }))
					{
						resources.Add(resource);
					}
					m = m.NextMatch();
				}
			}
			foreach (IChapter chapter in Chapters.Chapters)
			{
				if (chapter.Settings == null)
					continue;
				if (chapter.Settings.Style == null)
					continue;
				string css = chapter.Settings.Style.ToString();
				Match m = m_ResourceFinder.Match(css);
				while (m.Success)
				{
					string resource = m.Groups["url"].Value.Trim(new char[] { '"', '\'' });
					if (!resources.Exists(delegate(string res) { return (res == resource); }))
					{
						resources.Add(resource);
					}
					m = m.NextMatch();
				}
			}
			if (Style != null)
			{
				string css = Style.ToString();
				Match m = m_ResourceFinder.Match(css);
				while (m.Success)
				{
					string resource = m.Groups["url"].Value.Trim(new char[] { '"', '\'' });
					if (!resources.Exists(delegate(string res) { return (res == resource); }))
					{
						resources.Add(resource);
					}
					m = m.NextMatch();
				}
			}
			return resources;
		}

		/// <summary>
		/// Gets the empty resources.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-31</remarks>
		public List<int> GetEmptyResources()
		{
			return null;
		}

		/// <summary>
		/// Changes the media path.
		/// </summary>
		/// <param name="newPath">The path (either relative or absolute).</param>
		/// <param name="move">if set to <c>true</c> [move].</param>
		/// <remarks>Documented by Dev03, 2007-08-09</remarks>
		public void ChangeMediaPath(string newPath, bool move)
		{
			if (MediaDirectory != newPath)
			{
				if (move)
				{
					string srcPath, dstPath;

					if (System.IO.Path.IsPathRooted(MediaDirectory))
						srcPath = MediaDirectory;
					else
						srcPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_path), MediaDirectory);
					if (System.IO.Directory.Exists(srcPath))
					{
						if (System.IO.Path.IsPathRooted(newPath))
							dstPath = newPath;
						else
							dstPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_path), newPath);

						if (!System.IO.Directory.Exists(dstPath))
							System.IO.Directory.CreateDirectory(dstPath);

						//now get all the files and move them one by one
						foreach (string file in GetResources())
						{
							string absFilePath = file;
							if (!System.IO.Path.IsPathRooted(file))
								absFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_path), file);

							if (absFilePath.StartsWith(srcPath))
							{
								if (!System.IO.File.Exists(absFilePath))
									continue;

								//we need to check if the path goes any deeper from mediadir and handle this appropriatly
								string dstDirPath = System.IO.Path.Combine(dstPath, System.IO.Path.GetDirectoryName(absFilePath).Replace(srcPath, String.Empty));
								if (!System.IO.Directory.Exists(dstDirPath))
									System.IO.Directory.CreateDirectory(dstDirPath);

								string fileName = System.IO.Path.GetFileName(file);
								System.IO.File.Copy(absFilePath, System.IO.Path.Combine(dstPath, fileName), true);
								try
								{
									System.IO.File.Delete(absFilePath);
								}
								catch { }
								//find all matching media path nodes and replace the values
								foreach (XmlNode node in m_dictionary.SelectNodes(String.Format(m_xpathTextNodeFilter, file)))
								{
									node.Value = System.IO.Path.Combine(newPath, fileName);
								}
							}
						}
						System.IO.Directory.Delete(srcPath, true);
					}
				}
				MediaDirectory = newPath;
			}
		}

		/// <summary>
		/// Saves the dictionary to the new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void SaveAs(string newPath)
		{
			SaveAs(newPath, false);
		}

		/// <summary>
		/// Saves the dictionary to the new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
		/// <remarks>Documented by Dev03, 2007-08-13</remarks>
		public void SaveAs(string newPath, bool overwrite)
		{
			this.Version = m_CurrentVersion;

			StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.SaveAsProgress);
			ReportProgressUpdate(args);

			FlushListsToXml();

			string srcPath = System.IO.Path.GetDirectoryName(m_path);
			string dstPath = System.IO.Path.GetDirectoryName(newPath);
			string dstFileName = System.IO.Path.GetFileName(newPath);
			if (dstFileName == String.Empty)
			{
				dstFileName = System.IO.Path.GetFileName(m_path);
			}

			if (srcPath != dstPath)
			{
				if (!System.IO.Directory.Exists(dstPath))
					System.IO.Directory.CreateDirectory(dstPath);

				List<string> resources = GetResources();
				for (int i = 0; i < resources.Count; i++)
				{
					string file = resources[i];
					string absFilePath = file;
					if (!System.IO.Path.IsPathRooted(file))
						absFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_path), file);

					if (absFilePath.StartsWith(srcPath))
					{
						if (!System.IO.File.Exists(absFilePath))
							continue;

						//we need to check if the path goes any deeper from mediadir and handle this appropriatly
						string dstDirPath = System.IO.Path.Combine(dstPath, System.IO.Path.GetDirectoryName(absFilePath).Replace(srcPath, String.Empty).Trim(new char[] { '\\', ' ' }));
						if (!System.IO.Directory.Exists(dstDirPath))
							System.IO.Directory.CreateDirectory(dstDirPath);

						string fileName = System.IO.Path.GetFileName(file);
						try
						{
							System.IO.File.Copy(absFilePath, System.IO.Path.Combine(dstDirPath, fileName), overwrite);
						}
						catch { }
					}

					if ((i % 10) == 0)
					{
						args.Progress = (int)Math.Floor(100.0 * (i + 1) / resources.Count);
						ReportProgressUpdate(args);
					}
				}
			}
			string orgDicPath = m_path;
			m_path = System.IO.Path.Combine(dstPath, dstFileName);

			SaveDictionary();

			args.Progress = 100;
			ReportProgressUpdate(args);
		}

		/// <summary>
		/// Moves the specified new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void Move(string newPath)
		{
			Move(newPath, false);
		}

		/// <summary>
		/// Moves the dictionary to the specified new path.
		/// </summary>
		/// <param name="newPath">The new path.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
		/// <remarks>Documented by Dev03, 2007-08-13</remarks>
		public void Move(string newPath, bool overwrite)
		{
			StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.MoveProgress);
			ReportProgressUpdate(args);

			FlushListsToXml();

			string srcPath = System.IO.Path.GetDirectoryName(m_path);
			string dstPath = System.IO.Path.GetDirectoryName(newPath);
			string dstFileName = System.IO.Path.GetFileName(newPath);
			if (dstFileName == String.Empty)
			{
				dstFileName = System.IO.Path.GetFileName(m_path);
			}

			if (srcPath != dstPath)
			{
				if (!System.IO.Directory.Exists(dstPath))
					System.IO.Directory.CreateDirectory(dstPath);

				List<string> resources = GetResources();
				for (int i = 0; i < resources.Count; i++)
				{
					string file = resources[i];
					string absFilePath = file;
					if (!System.IO.Path.IsPathRooted(file))
						absFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(m_path), file);

					if (absFilePath.StartsWith(srcPath))
					{
						if (!System.IO.File.Exists(absFilePath))
							continue;

						//we need to check if the path goes any deeper from mediadir and handle this appropriatly
						string dstDirPath = System.IO.Path.Combine(dstPath, System.IO.Path.GetDirectoryName(absFilePath).Replace(srcPath, String.Empty).Trim(new char[] { '\\', ' ' }));
						if (!System.IO.Directory.Exists(dstDirPath))
							System.IO.Directory.CreateDirectory(dstDirPath);

						string fileName = System.IO.Path.GetFileName(file);
						try
						{
							System.IO.File.Copy(absFilePath, System.IO.Path.Combine(dstDirPath, fileName), overwrite);
						}
						catch { }
						try
						{
							System.IO.File.Delete(absFilePath);
						}
						catch { }   //we do not care if this fails
					}

					if ((i % 10) == 0)
					{
						args.Progress = (int)Math.Floor(100.0 * (i + 1) / resources.Count);
						ReportProgressUpdate(args);
					}
				}
				//try
				//{
				//    System.IO.Directory.Delete(srcPath, true);
				//}
				//catch { }   //we do not care if this fails
			}
			string orgDicPath = m_path;
			m_path = System.IO.Path.Combine(dstPath, dstFileName);
			SaveDictionary();
			try
			{
				System.IO.File.Delete(orgDicPath);
			}
			catch { }   //we do not care if this fails

			args.Progress = 100;
			ReportProgressUpdate(args);
		}

		/// <summary>
		/// Creates a new instance of a card style object.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-10-30</remarks>
		public ICardStyle CreateCardStyle()
		{
			return new XmlCardStyle(parent);
		}

		/// <summary>
		/// Creates a new media object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-11</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			return CreateNewMediaObject(type, path, isActive, isDefault, isExample);
		}

		/// <summary>
		/// Creates a new media object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="isExample">if set to <c>true</c> [is example].</param>
		/// <returns></returns>
		internal IMedia CreateNewMediaObject(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
		{
			IMedia media;
			Uri uri;

			if (path == null)
				throw new ArgumentNullException("Null value not allowed for media file path!");

			try
			{
				uri = new Uri(path);
			}
			catch (UriFormatException exception)
			{
				throw new FileNotFoundException("Uri format is invalid.", exception);
			}

			if (uri.Scheme == "http" && uri.IsLoopback)
			{
				//download the file
				WebClient client = new WebClient();
				byte[] buffer = client.DownloadData(uri);

				string extension = System.IO.Path.GetExtension(uri.ToString()); //the uri must contain the file extension
				path = FileCleanupQueue.GetTempFilePath(extension);

				File.WriteAllBytes(path, buffer);
			}

			if ((path.Trim().Length == 0) || !File.Exists(path))
				throw new FileNotFoundException("The media file does not exist at the given location!");

			switch (type)
			{
				case EMedia.Video:
					media = new XmlVideo(path, isActive, parent);
					break;
				case EMedia.Image:
					media = new XmlImage(path, isActive, parent);
					break;
				case EMedia.Audio:
				default:
					media = new XmlAudio(path, isActive, isDefault, isExample, parent);
					break;
			}
			return media;
		}

		/// <summary>
		/// Preloads the card cache.
		/// </summary>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		public void PreloadCardCache()
		{
			//not necessary to implement
		}

		/// <summary>
		/// Checks the user session.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public bool CheckUserSession() { return true; }

		/// <summary>
		/// Gets the LearningModule extensions.
		/// </summary>
		/// <value>The extensions.</value>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public IList<IExtension> Extensions
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		/// <summary>
		/// Adds the commentary audio to media folder.
		/// </summary>
		/// <param name="path">The path to the original file.</param>
		/// <param name="name">The name of the commentary file.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-03-05</remarks>
		internal string AddCommentaryAudioToMediaFolder(string path, string name)
		{
			if (!String.IsNullOrEmpty(path) && (path.Trim().Length > 0))
			{
				string srcPath, dstPath, relPath, mediaDir, dictionaryDir, filename, fileNameStrict;
				filename = System.IO.Path.GetFileName(path);
				fileNameStrict = name + System.IO.Path.GetExtension(filename);
				if (!String.IsNullOrEmpty(filename) && (filename.Length > 0))
				{
					dictionaryDir = System.IO.Path.GetDirectoryName(this.Path);
					System.IO.Directory.SetCurrentDirectory(dictionaryDir);
					srcPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), filename);
					mediaDir = System.IO.Path.Combine(dictionaryDir, this.MediaDirectory);
					relPath = System.IO.Path.Combine(this.MediaDirectory, fileNameStrict);
					dstPath = System.IO.Path.Combine(mediaDir, fileNameStrict);

					if (!System.IO.Path.IsPathRooted(srcPath))
					{
						if (srcPath.ToLower().Equals(relPath.ToLower()))
							return relPath;    // file is already in the media dir
					}
					if (File.Exists(srcPath))
					{
						if (!Directory.Exists(mediaDir))
							Directory.CreateDirectory(mediaDir);
						if (srcPath != dstPath)
						{
							try
							{
								File.Copy(srcPath, dstPath, true);
							}
							catch
							{
								return String.Empty;
							}
						}
						return relPath;
					}
				}
			}
			return String.Empty;
		}

		/// <summary>
		/// Get the commentary sounds.
		/// </summary>
		/// <returns>The commentary sounds.</returns>
		/// <remarks>Documented by Dev03, 2008-03-05</remarks>
		private List<string> GetCommentarySounds()
		{
			string dictionaryDir = System.IO.Path.GetDirectoryName(this.Path);
			List<string> sounds = new List<string>();
			foreach (string sound in m_audioComments)
			{
				if (!System.IO.Path.IsPathRooted(sound))
				{
					string absoluteSound = System.IO.Path.Combine(dictionaryDir, sound);
					if (System.IO.File.Exists(absoluteSound))
					{
						sounds.Add(absoluteSound);
						continue;
					}
				}
				sounds.Add(sound);
			}
			return sounds;
		}

		/// <summary>
		/// Flushes the lists to XML.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-13</remarks>
		internal void FlushListsToXml()
		{
			//XmlNodeList audioComments = m_generalSettings.SelectNodes(m_xpathCommentSound);
			//foreach (XmlNode audioComment in audioComments)
			//{
			//    audioComment.ParentNode.RemoveChild(audioComment);
			//}
			//for (int i = 0; i < m_audioComments.Count; i++)
			//{
			//    XmlNode audioComment = m_generalSettings.SelectSingleNode(m_xpathCommentSound + String.Format(m_xpathIdFilter, i));
			//    if (audioComment == null)
			//        XmlHelper.CreateElementWithAttribute(m_generalSettings, m_xpathCommentSound, m_audioComments[i].ToString(), m_xpathId, i.ToString());
			//    else
			//        audioComment.InnerText = m_audioComments[i].ToString();
			//}
			if (DefaultSettings != null && DefaultSettings.Style != null)
				SaveStyleToDOM(DefaultSettings.Style);
			XmlNodeList boxSizes = m_userSettings.SelectNodes(m_xpathBoxsize);
			foreach (XmlNode boxsize in boxSizes)
			{
				boxsize.ParentNode.RemoveChild(boxsize);
			}
			for (int i = 1; i < Boxes.Box.Count - 1; i++)     // box 1 to 9 (pool and box 10 are not written to the DOM)
			{
				XmlBox box = (XmlBox)Boxes.Box[i];
				XmlNode boxsize = m_userSettings.SelectSingleNode(m_xpathBoxsize + String.Format(m_xpathIdFilter, box.Id));
				if (boxsize == null)
					XmlHelper.CreateElementWithAttribute(m_userSettings, m_xpathBoxsize, box.MaximalSize.ToString(), m_xpathId, box.Id.ToString());
				else
					boxsize.InnerText = box.MaximalSize.ToString();
			}
			XmlNodeList queryChapters = m_userSettings.SelectNodes(m_xpathQueryChapter);
			foreach (XmlNode queryChapter in queryChapters)
			{
				queryChapter.ParentNode.RemoveChild(queryChapter);
			}
			for (int i = 0; i < m_queryChapters.Count; i++)
			{
				XmlNode querychapter = m_userSettings.SelectSingleNode(m_xpathQueryChapter + String.Format(m_xpathIdFilter, i));
				if (querychapter == null)
					XmlHelper.CreateElementWithAttribute(m_userSettings, m_xpathQueryChapter, m_queryChapters[i].ToString(), m_xpathId, i.ToString());
				else
					querychapter.InnerText = m_queryChapters[i].ToString();
			}
		}

		/// <summary>
		/// Gets the random number - helper function.
		/// </summary>
		/// <returns>A random number.</returns>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		internal int GetRandomNumber()
		{
			return m_random.Next();
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private void Initialize(bool ignoreOldVersion)
		{
			m_generalSettings = (XmlElement)m_dictionary.SelectSingleNode(m_xpathGenSet);
			m_userSettings = (XmlElement)m_dictionary.SelectSingleNode(m_xpathUsrSet);
			m_queryOptions = new MLifter.DAL.XML.XmlQueryOptions(this, parent);
			settings = new XmlSettings(this, Parent.GetChildParentClass(this));
			allowed_settings = new XmlAllowedSettings(this, Parent.GetChildParentClass(this));

			//check dictionary version
			if (this.Version > m_CurrentVersion)
			{
				CloseStream();
				throw new DictionaryFormatNotSupported(this.Version);
			}
			else if (this.Version < m_CurrentVersion && !ignoreOldVersion)
			{
				CloseStream();
				throw new DictionaryFormatOldVersion(this.Version);
			}

			XPathNavigator navigator;
			XPathExpression expression;
			XPathNodeIterator nodeIterator;
			//read commentsound
			m_audioComments = new List<string>();
			navigator = m_generalSettings.CreateNavigator();
			expression = navigator.Compile(m_xpathCommentSound);
			expression.AddSort(m_xpathAttributeId, XmlSortOrder.Ascending, XmlCaseOrder.None, String.Empty, XmlDataType.Number);
			nodeIterator = navigator.Select(expression);
			while (nodeIterator.MoveNext())
				m_audioComments.Add(nodeIterator.Current.Value);
			//read boxsize
			m_Boxes = new XmlBoxes(this, Parent.GetChildParentClass(this));
			navigator = m_userSettings.CreateNavigator();
			expression = navigator.Compile(m_xpathBoxsize);
			expression.AddSort(m_xpathAttributeId, XmlSortOrder.Ascending, XmlCaseOrder.None, String.Empty, XmlDataType.Number);
			nodeIterator = navigator.Select(expression);
			while (nodeIterator.MoveNext())
			{
				string boxSizeValue = nodeIterator.Current.Value;
				int currentId = 0;
				if (nodeIterator.Current.MoveToAttribute(m_xpathId, String.Empty))
				{
					if (Int32.TryParse(nodeIterator.Current.Value, out currentId))
					{
						if ((currentId > 0) && (currentId < m_numberOfBoxes))   // box 1 to 9 (pool and box 10 are not written)
						{
							m_Boxes.Box[currentId].MaximalSize = XmlConvert.ToInt32(boxSizeValue);
						}
					}
				}
			}
			//read querychapter
			m_queryChapters = new List<int>();
			navigator = m_userSettings.CreateNavigator();
			expression = navigator.Compile(m_xpathQueryChapter);
			expression.AddSort(m_xpathAttributeId, XmlSortOrder.Ascending, XmlCaseOrder.None, String.Empty, XmlDataType.Number);
			nodeIterator = navigator.Select(expression);
			while (nodeIterator.MoveNext())
				m_queryChapters.Add(XmlConvert.ToInt32(nodeIterator.Current.Value));

			m_cards = new MLifter.DAL.XML.XmlCards(this, parent.GetChildParentClass(this));
			m_chapters = new MLifter.DAL.XML.XmlChapters(this, Parent.GetChildParentClass(this));
			m_statistics = new MLifter.DAL.XML.XmlStatistics(this);

			m_AllowedQueryTypes = new XML.XmlAllowedQueryTypes(this, Parent.GetChildParentClass(this));
			m_AllowedQueryDirections = new XML.XmlAllowedQueryDirections(this, Parent.GetChildParentClass(this));

			this.Category.IdChanged += new EventHandler(Category_IdChanged);
		}

		/// <summary>
		/// Reads the style from DOM.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-11-07</remarks>
		private void ReadStyleFromDOM()
		{
			XmlElement xeStyle = m_generalSettings[m_XPath_Style];
			if (xeStyle != null)
			{
				if (xeStyle.HasChildNodes)
				{
					XmlReader xmlReader = new XmlNodeReader(xeStyle);
					if (StyleSerializer.CanDeserialize(xmlReader))
					{
						m_Style = (XmlCardStyle)StyleSerializer.Deserialize(xmlReader);
						(m_Style as XmlCardStyle).Parent = Parent;
					}
				}
			}
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
				XmlElement xeStyle = m_generalSettings[m_XPath_Style];
				if (xeStyle != null)
				{
					m_generalSettings.RemoveChild(xeStyle);
				}
				XmlDocument xdStyle = new XmlDocument();
				xdStyle.LoadXml(stringBuilder.ToString());
				m_generalSettings.AppendChild(m_dictionary.ImportNode(xdStyle.DocumentElement, true));
				success = true;
			}
			catch { }
			return success;
		}

		/// <summary>
		/// Reports the progress update.
		/// </summary>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <returns>[true] if the process has been canceled.</returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		private bool ReportProgressUpdate(StatusMessageEventArgs args)
		{
			switch (args.MessageType)
			{
				case StatusMessageType.XmlProgress:
					if (XmlProgressChanged != null) XmlProgressChanged(null, args);
					break;
				case StatusMessageType.MoveProgress:
					if (MoveProgressChanged != null) MoveProgressChanged(null, args);
					break;
				case StatusMessageType.SaveAsProgress:
					if (SaveAsProgressChanged != null) SaveAsProgressChanged(null, args);
					break;
				case StatusMessageType.CreateMediaProgress:
					if (CreateMediaProgressChanged != null) CreateMediaProgressChanged(null, args);
					break;
			}

			bool cancelProcess = false;
			if (m_BackgroundWorker != null)
			{
				if (m_BackgroundWorker.CancellationPending)
				{
					cancelProcess = true;
				}
				else
				{
					m_BackgroundWorker.ReportProgress(args.ProgressPercentage);
				}
			}
			return !cancelProcess;
		}

		private void Category_IdChanged(object sender, EventArgs e)
		{
			this.Category = (Category)sender;
		}

		#region IDictionary Members

		private ISettings settings;
		/// <summary>
		/// Gets or sets the default settings.
		/// </summary>
		/// <value>The settings.</value>
		/// <remarks>Documented by Dev05, 2008-08-11</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings DefaultSettings
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

		/// <summary>
		/// Gets or sets the user settings.
		/// </summary>
		/// <value>The user settings.</value>
		/// <remarks>Documented by Dev05, 2008-10-01</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings UserSettings { get { return DefaultSettings; } set { DefaultSettings = value; } }

		ISettings allowed_settings;
		/// <summary>
		/// Gets or sets the allowed settings.
		/// </summary>
		/// <value>The allowed settings.</value>
		/// <remarks>Documented by Dev05, 2008-09-22</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings AllowedSettings
		{
			get
			{
				return allowed_settings;
			}
			set
			{
				allowed_settings = value;
			}
		}

		/// <summary>
		/// Creates the settings.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ISettings CreateSettings()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resets the learning progress.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public IDictionary ResetLearningProgress()
		{
			#region Save a backup copy
			Save();

			string filename = m_path;
			string backupFilename = string.Empty;
			int counter = 0;
			do
			{
				backupFilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), string.Format(Properties.Resources.XML_BACKUP_FILENAME, counter++, System.IO.Path.GetFileName(filename)));
			}
			while (File.Exists(backupFilename) && counter < 100);

			File.Copy(filename, backupFilename, true);
			OnBackupCompleted(backupFilename);
			#endregion

			Cards.ClearAllBoxes();
			HighScore = score = 0;
			DefaultSettings.SelectedLearnChapters.Clear();
			Statistics.Clear();

			return this;
		}

		/// <summary>
		/// Clears the unused media.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-27</remarks>
		public void ClearUnusedMedia()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Occurs when [backup completed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public event BackupCompletedEventHandler BackupCompleted;

		/// <summary>
		/// Called when [backup completed].
		/// </summary>
		/// <param name="backupFilename">The backup filename.</param>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		private void OnBackupCompleted(string backupFilename)
		{
			if (BackupCompleted != null)
			{
				BackupCompletedEventArgs e = new BackupCompletedEventArgs();
				e.BackupFilename = backupFilename;
				BackupCompleted(this, e);
			}
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(IDictionary), progressDelegate);
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
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

		#region IDictionary Members


		/// <summary>
		/// Creates a new extension.
		/// </summary>
		/// <returns></returns>
		public IExtension ExtensionFactory()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates new extensions.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public IExtension ExtensionFactory(Guid guid)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
