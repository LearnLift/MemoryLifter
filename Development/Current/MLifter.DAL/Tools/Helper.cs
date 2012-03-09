//#define debug_output

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.ComponentModel;
using MLifter.DAL.Interfaces;
using System.Xml;
using System.Diagnostics;
using MLifter.DAL.Properties;
using System.Runtime.InteropServices;
using System.Reflection;
using MLifter.DAL.Tools;
using System.Data.SqlServerCe;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB;

namespace MLifter.DAL
{
	/// <summary>
	/// Deifferent database types
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// The database type is an normal xml file (.odx).
		/// </summary>
		Xml,
		/// <summary>
		/// The database type is a PostgreSql-Server.
		/// </summary>
		PostgreSQL,
		/// <summary>
		/// The Connection is a (local) unc-path.
		/// </summary>
		Unc,
		/// <summary>
		/// The Database type is a Microsoft SQL Compact Edition.
		/// </summary>
		MsSqlCe,
		/// <summary>
		/// The Connection is to a WebService.
		/// </summary>
		Web
	}

	/// <summary>
	/// Database helpers.
	/// </summary>
	public class Helper
	{
		/// <summary>
		/// The different fields of a card.
		/// </summary>
		public enum CardFields
		{
			/// <summary>
			/// The Question field.
			/// </summary>
			Question,
			/// <summary>
			/// The Question Audio field.
			/// </summary>
			QuestionAudio,
			/// <summary>
			/// The Question Image field.
			/// </summary>
			QuestionImage,
			/// <summary>
			/// The Question Video field.
			/// </summary>
			QuestionVideo,
			/// <summary>
			/// The Question Example field.
			/// </summary>
			QuestionExample,
			/// <summary>
			/// The Question Example Audio field.
			/// </summary>
			QuestionExampleAudio,
			/// <summary>
			/// The Answer field.
			/// </summary>
			Answer,
			/// <summary>
			/// The Answer Audio field.
			/// </summary>
			AnswerAudio,
			/// <summary>
			/// The Answer Image field.
			/// </summary>
			AnswerImage,
			/// <summary>
			/// The Answer Video field.
			/// </summary>
			AnswerVideo,
			/// <summary>
			/// The Answer Example field.
			/// </summary>
			AnswerExample,
			/// <summary>
			/// The Answer Example Audio field.
			/// </summary>
			AnswerExampleAudio,
		}

		/// <summary>
		/// Commentary sound names
		/// </summary>
		public static readonly string[] CommentarySoundNames = new string[12]
		{
			"commentary_q_sa_correct",
			"commentary_q_sa_wrong",
			"commentary_q_sa_almost",
			"commentary_q_correct",
			"commentary_q_wrong",
			"commentary_q_almost",
			"commentary_a_sa_correct",
			"commentary_a_sa_wrong",
			"commentary_a_sa_almost",
			"commentary_a_correct",
			"commentary_a_wrong",
			"commentary_a_almost"
		};

		private static Regex regEx = new Regex(@"A=(?<AV>\d{1,3}),\s*R=(?<RV>\d{1,3}),\s*G=(?<GV>\d{1,3}),\s*B=(?<BV>\d{1,3})");
		private static Regex regExColorNameMatch = new Regex(@"\[(?<NAME>\w+)\]");
		/// <summary>
		/// Parses the specified string read form the XML and returns the value
		/// it represents as the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		public static object GetValue(Type type, string value)
		{
			if (type == typeof(string))
			{
				return value;
			}
			else if (type == typeof(bool))
			{
				return Convert.ToBoolean(value);
			}
			else if (type.IsEnum)
			{
				return Enum.Parse(type, value, true);
			}
			else if (type == typeof(Color))
			{
				Match match = regEx.Match(value);

				if (match.Success)
					return Color.FromArgb(Convert.ToInt32(match.Groups["AV"].Value), Convert.ToInt32(match.Groups["RV"].Value),
						Convert.ToInt32(match.Groups["GV"].Value), Convert.ToInt32(match.Groups["BV"].Value));
				else
				{
					match = regExColorNameMatch.Match(value);

					if (match.Success)
						return Color.FromName(match.Groups["NAME"].Value);
					else
						return Color.Empty;
				}
			}
			else if (type == typeof(DateTime))
			{
				if (value.Length == 0)
				{
					return DateTime.MinValue;
				}
				else
				{
					return DateTime.Parse(value);
				}
			}
			else if (type == typeof(TimeSpan))
			{
				return TimeSpan.Parse(value);
			}
			else if (type == typeof(Int16))
			{
				return Convert.ToInt16(value);
			}
			else if (type == typeof(Int32))
			{
				return Convert.ToInt32(value);
			}
			else if (type == typeof(Int64))
			{
				return Convert.ToInt64(value);
			}
			else if (type == typeof(float))
			{
				return Convert.ToSingle(value);
			}
			else if (type == typeof(double))
			{
				return Convert.ToDouble(value);
			}
			else if (type == typeof(decimal))
			{
				return Convert.ToDecimal(value);
			}
			else if (type == typeof(char))
			{
				return Convert.ToChar(value);
			}
			else if (type == typeof(byte))
			{
				return Convert.ToByte(value);
			}
			else if (type == typeof(UInt16))
			{
				return Convert.ToUInt16(value);
			}
			else if (type == typeof(UInt32))
			{
				return Convert.ToUInt32(value);
			}
			else if (type == typeof(UInt64))
			{
				return Convert.ToUInt64(value);
			}
			else if (type == typeof(Guid))
			{
				return new Guid(value);
			}
			else
			{
				return GetFromInvariantString(type, value);
			}
		}
		/// <summary>
		/// Gets the object from the invariant string.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-13</remarks>
		public static object GetFromInvariantString(Type type, string value)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			return converter.ConvertFromInvariantString(value);
		}

		/// <summary>
		/// Gets the string representing the given object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-13</remarks>
		public static string GetString(Type type, object value)
		{
			if (type == typeof(string))
			{
				return (string)value;
			}
			else if (type == typeof(bool) || type.IsEnum || type == typeof(Color) || type == typeof(DateTime) || type == typeof(TimeSpan) ||
				type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(float) || type == typeof(double) ||
				type == typeof(decimal) || type == typeof(char) || type == typeof(byte) || type == typeof(UInt16) || type == typeof(UInt32) ||
				type == typeof(UInt64) || type == typeof(Guid))
			{
				return value.ToString();
			}
			else
			{
				return GetInvariantString(type, value);
			}
		}
		/// <summary>
		/// Gets the invariant string of the object.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2007-08-13.
		/// </remarks>
		public static string GetInvariantString(Type type, object value)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			return converter.ConvertToInvariantString(value);
		}

		/// <summary>
		/// Works out which types to treat as attibutes and which the treat as child objects.
		/// </summary>
		/// <param name="type">The Type to check.</param>
		/// <returns>true if the Type is atomic (e.g. string, date, enum or number), false if it is arrayList compound sub-object.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <i>type</i> is null (Nothing in Visual Basic).</exception>
		public static bool TypeIsAtomic(Type type)
		{
			if (type == typeof(string) || TypeIsNumeric(type) || type == typeof(bool) || type == typeof(DateTime) ||
				type == typeof(TimeSpan) || type == typeof(char) || type == typeof(byte) || type.IsSubclassOf(typeof(Enum)) ||
				type == typeof(Guid) || type == typeof(Color) || type == typeof(Font))
			{
				return true;
			}

			return false;
		}
		/// <summary>
		/// Returns true if the specified type is one of the numeric types
		/// (Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal)
		/// </summary>
		/// <param name="type">The Type to check.</param>
		/// <returns>
		/// true if the specified type is one of the numeric types
		/// (Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal)
		/// </returns>
		public static bool TypeIsNumeric(Type type)
		{
			if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(float) ||
				type == typeof(double) || type == typeof(decimal) || type == typeof(UInt16) || type == typeof(UInt32) ||
				type == typeof(UInt64))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks the name of the file if the file extension is a supported one.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		public static bool CheckFileName(string path)
		{
			string extension = Path.GetExtension(path).ToLower();
			if ((extension == Helper.OdxExtension)
				|| (extension == Helper.OdfExtension)
				|| (extension == Helper.DzpExtension)
				|| (extension == Helper.ZipExtension)
				|| (extension == Helper.EmbeddedDbExtension)
				|| (extension == Helper.ConfigFileExtension))
				return true;
			return false;
		}

		/// <summary>
		/// Determines whether the specified path is a learning module file name.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		///   <c>true</c> if the specified path is a learning module file name; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsLearningModuleFileName(string path)
		{
			string extension = Path.GetExtension(path);
			if ((extension == Helper.OdxExtension)
				|| (extension == Helper.OdfExtension)
				|| (extension == Helper.DzpExtension)
				|| (extension == Helper.ZipExtension)
				|| (extension == Helper.EmbeddedDbExtension))
				return true;
			return false;
		}

		/// <summary>
		/// Gets the type of the media.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-20</remarks>
		public static EMedia GetMediaType(string path)
		{
			EMedia type;
			switch (Path.GetExtension(path).ToLower())
			{
				case ".jpg":
				case ".jpeg":
				case ".bmp":
				case ".ico":
				case ".emf":
				case ".wmf":
				case ".gif":
				case ".png":
					type = EMedia.Image;
					break;
				case ".wav":
				case ".wma":
				case ".mp3":
				case ".mid":
					type = EMedia.Audio;
					break;
				case ".wmv":
				case ".avi":
				case ".mpg":
				case ".mpeg":
					type = EMedia.Video;
					break;
				default:
					type = EMedia.Unknown;
					break;
			}
			return type;
		}

		/// <summary>
		/// String for a unknown mime type.
		/// </summary>
		public const string UnknownMimeType = "application/unknown";

		/// <summary>
		/// Gets the MIME type for a filename, using the Windows Registry.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		public static string GetMimeType(string fileName)
		{
			//TODO: Provide a less environment specific way to get the MIME type (e.g. with a table).
			string mimeType = UnknownMimeType;
			string ext = System.IO.Path.GetExtension(fileName).ToLower();
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (regKey != null && regKey.GetValue("Content Type") != null)
				mimeType = regKey.GetValue("Content Type").ToString();
			return mimeType;
		}

		/// <summary>
		/// Reads the media properties.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="id">The id.</param>
		/// <param name="mediaconnector">The mediaconnector.</param>
		/// <remarks>
		/// Documented by DAC, 2008-08-06.
		/// </remarks>
		internal static void UpdateMediaProperties(string path, int id, Interfaces.DB.IDbMediaConnector mediaconnector)
		{
			Uri uri = new Uri(path);
			Stream mediaStream = uri.IsFile ? null : mediaconnector.GetMediaStream(id);
			mediaconnector.SetPropertyValue(id, MediaProperty.MimeType, GetMimeType(path));
			mediaconnector.SetPropertyValue(id, MediaProperty.Extension, Path.GetExtension(path));
			mediaconnector.SetPropertyValue(id, MediaProperty.MediaSize, uri.IsFile ? new FileInfo(path).Length.ToString() : mediaStream.Length.ToString());

			switch (GetMediaType(path))
			{
				case EMedia.Audio:
					break;
				case EMedia.Video:
					break;
				case EMedia.Image:
					using (Image image = uri.IsFile ? Image.FromFile(path) : Image.FromStream(mediaStream))
					{
						mediaconnector.SetPropertyValue(id, MediaProperty.Width, image.Width.ToString());
						mediaconnector.SetPropertyValue(id, MediaProperty.Height, image.Height.ToString());
					}
					break;
				case EMedia.Unknown:
				default:
					break;
			}
		}

		/// <summary>
		/// Gets the odx connection prefix.
		/// </summary>
		/// <value>The odx connection prefix.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string OdxConnectionPrefix { get { return Resources.DICTIONARY_ODX_CONNECTION_PREFIX; } }
		/// <summary>
		/// Gets the db postgres connection prefix.
		/// </summary>
		/// <value>The db postgres connection prefix.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string DbPostgresConnectionPrefix { get { return Resources.DICTIONARY_DB_POSTGRES_CONNECTION_PREFIX; } }

		/// <summary>
		/// Gets the dip extension.
		/// </summary>
		/// <value>The dip extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string DipExtension { get { return Resources.DICTIONARY_DIP_EXTENSION; } }
		/// <summary>
		/// Gets the odx extension.
		/// </summary>
		/// <value>The odx extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string OdxExtension { get { return Resources.DICTIONARY_ODX_EXTENSION; } }
		/// <summary>
		/// Gets the odf extension.
		/// </summary>
		/// <value>The odf extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string OdfExtension { get { return Resources.DICTIONARY_ODF_EXTENSION; } }
		/// <summary>
		/// Gets the DZP extension.
		/// </summary>
		/// <value>The DZP extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string DzpExtension { get { return Resources.DICTIONARY_ARCHIVE_EXTENSION; } }
		/// <summary>
		/// Gets the embedded db extension.
		/// </summary>
		/// <value>The embedded db extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string EmbeddedDbExtension { get { return Resources.DICTIONARY_EDB_EXTENSION; } }
		/// <summary>
		/// Gets the synced embedded db extension.
		/// </summary>
		/// <value>The synced embedded db extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string SyncedEmbeddedDbExtension { get { return Resources.DICTIONARY_SYNCED_EDB_EXTENSION1; } }
		/// <summary>
		/// Gets the zip extension.
		/// </summary>
		/// <value>The zip extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string ZipExtension { get { return Resources.DICTIONARY_ZIP_EXTENSION; } }
		/// <summary>
		/// Gets the config file extension.
		/// </summary>
		/// <value>The config file extension.</value>
		/// <remarks>Documented by Dev05, 2009-02-20</remarks>
		public static string ConfigFileExtension { get { return Resources.CONFIGURATION_FILE_EXTENSION; } }

		/// <summary>
		/// Determines whether [is odf format] [the specified database path].
		/// </summary>
		/// <param name="databasePath">The database path.</param>
		/// <returns>
		/// 	<c>true</c> if [is odf format] [the specified database path]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		public static bool IsOdfFormat(string databasePath)
		{
			return Path.GetExtension(databasePath).Equals(OdfExtension, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether [is odx format] [the specified database path].
		/// </summary>
		/// <param name="databasePath">The database path.</param>
		/// <returns>
		/// 	<c>true</c> if [is odx format] [the specified database path]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev10, 2009-27-02</remarks>
		public static bool IsOdxFormat(string databasePath)
		{
			return Path.GetExtension(databasePath).Equals(OdxExtension, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether [is embedded db format] [the specified database path].
		/// </summary>
		/// <param name="databasePath">The database path.</param>
		/// <returns>
		/// 	<c>true</c> if [is embedded db format] [the specified database path]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-03-16</remarks>
		public static bool IsEmbeddedDbFormat(string databasePath)
		{
			return Path.GetExtension(databasePath).Equals(EmbeddedDbExtension, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Builds the ODX connection string.
		/// </summary>
		/// <param name="databasePath">The database path.</param>
		/// <returns>The connection string.</returns>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		public static string BuildOdxConnectionString(string databasePath)
		{
			return OdxConnectionPrefix + databasePath;
		}

		/// <summary>
		/// Builds the DB Postgres connection string.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-07-23</remarks>
		public static string BuildDbPostgresConnectionString(string connectionString)
		{
			return DbPostgresConnectionPrefix + connectionString;
		}

		/// <summary>
		/// Generates the XML card.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <returns></returns>
		public static XmlElement GenerateXmlCard(ICard card)
		{
			StringBuilder sb = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.Unicode;
			settings.Indent = true;
			XmlWriter writer = XmlWriter.Create(sb, settings);

			writer.WriteStartDocument();
			writer.WriteStartElement("card");
			writer.WriteAttributeString("id", card.Id.ToString());

			writer.WriteStartElement("answer");
			if (card.Answer.Words.Count > 0)
				writer.WriteString(card.Answer.ToQuotedString());
			writer.WriteEndElement();

			writer.WriteStartElement("answerexample");
			if (card.AnswerExample.Words.Count > 0)
				writer.WriteString(card.AnswerExample.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("answerdistractors");
			foreach (IWord distractor in card.AnswerDistractors.Words)
			{
				writer.WriteStartElement("distractor");
				writer.WriteAttributeString("id", distractor.Id.ToString());
				writer.WriteString(distractor.Word);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			foreach (IMedia media in card.AnswerMedia)
			{
				if (!media.Active.GetValueOrDefault()) continue;
				if (media is IAudio)
				{
					if (media.Example.GetValueOrDefault())
					{
						writer.WriteStartElement("answerexampleaudio");
						writer.WriteString(media.Filename);
						writer.WriteEndElement();
					}
					else
					{
						writer.WriteStartElement("answeraudio");
						if (media.Default.GetValueOrDefault())
							writer.WriteAttributeString("id", "std");
						writer.WriteString(media.Filename);
						writer.WriteEndElement();
					}
				}
				if (media is IImage)
				{
					writer.WriteStartElement("answerimage");
					writer.WriteAttributeString("width", (media as IImage).Width.ToString());
					writer.WriteAttributeString("height", (media as IImage).Height.ToString());
					writer.WriteString(media.Filename);
					writer.WriteEndElement();
				}
				if (media is IVideo)
				{
					writer.WriteStartElement("answervideo");
					writer.WriteString(media.Filename);
					writer.WriteEndElement();
				}
			}

			writer.WriteStartElement("question");
			if (card.Question.Words.Count > 0)
				writer.WriteString(card.Question.ToQuotedString());
			writer.WriteEndElement();

			writer.WriteStartElement("questionexample");
			if (card.QuestionExample.Words.Count > 0)
				writer.WriteString(card.QuestionExample.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("questiondistractors");
			foreach (IWord distractor in card.QuestionDistractors.Words)
			{
				writer.WriteStartElement("distractor");
				writer.WriteAttributeString("id", distractor.Id.ToString());
				writer.WriteString(distractor.Word);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			foreach (IMedia media in card.QuestionMedia)
			{
				if (!media.Active.GetValueOrDefault()) continue;
				if (media is IAudio)
				{
					if (media.Example.GetValueOrDefault())
					{
						writer.WriteStartElement("questionexampleaudio");
						writer.WriteString(media.Filename);
						writer.WriteEndElement();
					}
					else
					{
						writer.WriteStartElement("questionaudio");
						if (media.Default.GetValueOrDefault())
							writer.WriteAttributeString("id", "std");
						writer.WriteString(media.Filename);
						writer.WriteEndElement();
					}
				}
				if (media is IImage)
				{
					writer.WriteStartElement("questionimage");
					writer.WriteAttributeString("width", (media as IImage).Width.ToString());
					writer.WriteAttributeString("height", (media as IImage).Height.ToString());
					writer.WriteString(media.Filename);
					writer.WriteEndElement();
				}
				if (media is IVideo)
				{
					writer.WriteStartElement("questionvideo");
					writer.WriteString(media.Filename);
					writer.WriteEndElement();
				}
			}

			writer.WriteStartElement("chapter");
			writer.WriteString(card.Chapter.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("box");
			writer.WriteString(card.Box.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("timestamp");
			writer.WriteString(XmlConvert.ToString(card.Timestamp, XmlDateTimeSerializationMode.RoundtripKind));
			writer.WriteEndElement();

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();

			XmlDocument doc = new XmlDocument();
			string xml = sb.ToString();
			try
			{
				doc.LoadXml(xml);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return doc.DocumentElement;
		}

		/// <summary>
		/// Splits the word list.
		/// </summary>
		/// <param name="wordList">The word list.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-28</remarks>
		public static string[] SplitWordList(string wordList)
		{
			string[] words;
			if (wordList != null)
			{
				//wordList = wordList.Trim(new char[] { ',', ' ' });
				if (wordList.Length > 0)
				{
					words = wordList.Split(new string[] { "\",\"", "\", \"" }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < words.Length; i++)
					{
						words[i] = words[i].Trim();
					}
					//remove trailing and leading '"' which is used to enclose synonym text
					if (words.Length > 0)
					{
						if (words[0].StartsWith("\"")) words[0] = words[0].Substring(1);
						if (words[words.Length - 1].EndsWith("\"")) words[words.Length - 1] = words[words.Length - 1].Substring(0, words[words.Length - 1].Length - 1);
					}
					//decode some protected chars ('"' and ',') 
					for (int i = 0; i < words.Length; i++)
					{
						words[i] = words[i].Replace("\"\"", "\"").Replace("\\,", ",");
					}
				}
				else
				{
					words = new string[] { };
				}
			}
			else
			{
				words = new string[] { };
			}
			return words;
		}

		/// <summary>
		/// Converts the array To a quoted comma string.
		/// </summary>
		/// <param name="wordsToQuote">The words to quote.</param>
		/// <returns></returns>
		public static string ToQuotedCommaString(string[] wordsToQuote)
		{
			string[] words = (string[])wordsToQuote.Clone();
			//encode some protected chars ('"' and ',') 
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = words[i].Replace("\"", "\"\"").Replace(",", "\\,");
			}
			return "\"" + String.Join("\", \"", words) + "\"";
		}

		/// <summary>
		/// Finds all files in all subfolders in a recursive way.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <returns>The found file list.</returns>
		/// <remarks>Documented by Dev02, 2008-12-18</remarks>
		public static string[] GetFilesRecursive(string path, string searchPattern)
		{
			List<string> files = new List<string>();

			try
			{
				if (!Directory.Exists(path))
					return new string[0];

#if DEBUG && debug_output
				Debug.WriteLine("Scanning directory (for " + searchPattern + ") " + path);
#endif

				foreach (string file in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
					files.Add(file);

				foreach (string directory in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
					files.AddRange(GetFilesRecursive(directory, searchPattern));
			}
			catch (Exception exp)
			{
				System.Diagnostics.Trace.WriteLine("Could not access directory " + path + " because: " + exp.Message);
			}

			return files.ToArray();
		}

		/// <summary>
		/// Gets the ms SQL ce script.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-13</remarks>
		public static string GetMsSqlCeScript()
		{
			return Resources.MsSqlCeDbCreateScript;
		}


		/// <summary>
		/// Occurs when the save copy to progress changed.
		/// </summary>
		public static event StatusMessageEventHandler SaveCopyToProgressChanged;

		/// <summary>
		/// Saves the copy to.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev09, 2009-03-06</remarks>
		public static bool SaveCopyTo(string source, string destination, bool overwrite)
		{
			//StreamReader reader = new StreamReader(source);
			//StreamWriter writer = new StreamWriter(destination, false);

			int nBytes = 2048;
			byte[] data = new byte[nBytes];
			using (FileStream writer = new FileStream(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
			using (FileStream reader = new FileStream(source, FileMode.Open, FileAccess.Read))
			{
				StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.SaveAsProgress, (int)reader.Length);
				int counter = 0;
				while ((nBytes = reader.Read(data, 0, data.Length)) > 0)
				{
					writer.Write(data, 0, nBytes);
					args.Progress = counter++ * data.Length;
					ReportSaveCopyToProgress(args);
				}
				reader.Close();
				writer.Close();
			}

			return false;
		}

		private static void ReportSaveCopyToProgress(StatusMessageEventArgs args)
		{
			if (SaveCopyToProgressChanged != null)
				SaveCopyToProgressChanged(null, args);
		}

		/// <summary>
		/// Filters the invalid filename characters.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-06-25</remarks>
		public static string FilterInvalidFilenameCharacters(string filename)
		{
			Match match = Regex.Match(filename, @"(?'drive'[a-z]{1}:\\)(?'filepath'.*)", RegexOptions.IgnoreCase);
			if (!match.Success)
				return filename;

			string drive = match.Groups["drive"].Value;
			string filepath = match.Groups["filepath"].Value;

			List<Char> filterChars = new List<char>(Path.GetInvalidPathChars());
			filterChars.Add(':');
			filterChars.Add('?');
			filterChars.Add(';'); //to avoid connectionstring parse issues
			filterChars.Add('/'); //only backslash allowed
			filepath = Regex.Replace(filepath, "[" + Regex.Escape(new string(filterChars.ToArray())) + "]", "_");

			return drive + filepath;
		}
	}

	/// <summary>
	/// Novell DLL-references.
	/// </summary>
	public class IcNovell
	{
		/// <summary>
		/// NWs the calls init.
		/// </summary>
		/// <param name="reserved1">The reserved1.</param>
		/// <param name="reserved2">The reserved2.</param>
		/// <returns></returns>
		[DllImport("calwin32.dll")]
		public static extern int NWCallsInit(byte reserved1, byte reserved2);
		/// <summary>
		/// Create the NWDS context handle.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		[DllImport("netwin32.dll", EntryPoint = "NWDSCreateContextHandle")]
		public static extern int NWDSCreateContextHandle(ref int context);
		/// <summary>
		/// Who am I in NWDS.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="NovellUserId">The novell user id.</param>
		/// <returns></returns>
		[DllImport("netwin32.dll", EntryPoint = "NWDSWhoAmI")]
		public static extern int NWDSWhoAmI(int context, StringBuilder NovellUserId);
		/// <summary>
		/// Free the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		[DllImport("netwin32.dll", EntryPoint = "NWDSFreeContext")]
		public static extern int NWDSFreeContext(int context);
	}

	/// <summary>
	/// EventHandler for the BackupCompleted Event.
	/// </summary>
	public delegate void BackupCompletedEventHandler(object sender, BackupCompletedEventArgs args);

	/// <summary>
	/// EventArgs for the BackupCompleted Event.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-09-08</remarks>
	public class BackupCompletedEventArgs : EventArgs
	{
		string backupFilename = string.Empty;

		/// <summary>
		/// Gets or sets the backup filename.
		/// </summary>
		/// <value>The backup filename.</value>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		public string BackupFilename
		{
			get { return backupFilename; }
			set { backupFilename = value; }
		}
	}
}
