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
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using MLifter.DAL;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.Transformer.V17
{
	/// <summary>
	/// This class can convert a MemoryLifter V1.7 dictionary to a newer format.
	/// </summary>
	public sealed class Converter
	{
		private string m_applicationPath = String.Empty;
		private int m_nrBox = 10;
		private int m_defaultCategory = Category.DefaultCategory;

		private const int m_releaseYear = 2006;
		private const int m_oldFileVersion = 0x0109;
		private const string m_odfFileHeaderString = "OMICRON dictionary file";
		private GetLoginInformation m_loginCallback = null;

		private Encoding m_SourceEncoding = Encoding.Default;

		private BackgroundWorker m_BackgroundWorker;

		/// <summary>
		/// Initializes a new instance of the <see cref="Converter"/> class.
		/// </summary>
		public Converter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Converter"/> class.
		/// </summary>
		/// <param name="backgroundWorker">The background worker.</param>
		public Converter(BackgroundWorker backgroundWorker)
		{
			m_BackgroundWorker = backgroundWorker;
		}

		/// <summary>
		/// Gets or sets the source encoding.
		/// </summary>
		/// <value>
		/// The source encoding.
		/// </value>
		public Encoding SourceEncoding
		{
			get { return m_SourceEncoding; }
			set { m_SourceEncoding = value; }
		}

		/// <summary>
		/// Gets or sets the application path.
		/// </summary>
		/// <value>
		/// The application path.
		/// </value>
		public string ApplicationPath
		{
			get { return m_applicationPath; }
			set { m_applicationPath = value; }
		}

		/// <summary>
		/// Gets or sets the number of boxes.
		/// </summary>
		/// <value>
		/// The number of boxes.
		/// </value>
		public int NumberOfBoxes
		{
			get { return m_nrBox; }
			set { m_nrBox = value; }
		}

		/// <summary>
		/// Gets or sets the default category.
		/// </summary>
		/// <value>
		/// The default category.
		/// </value>
		public int DefaultCategory
		{
			get { return m_defaultCategory; }
			set { m_defaultCategory = value; }
		}

		/// <summary>
		/// Gets or sets the login callback.
		/// </summary>
		/// <value>The login callback.</value>
		/// <remarks>Documented by Dev03, 2008-09-08</remarks>
		public GetLoginInformation LoginCallback
		{
			get { return m_loginCallback; }
			set { m_loginCallback = value; }
		}

		/// <summary>
		/// This enumerator defines the available multimedia resources for old version dictionaries.
		/// </summary>
		private enum TCardPart { None = 0, SentSrc = 1, SentDst = 2, Image = 4, AudioSrc = 8, AudioDst = 16 };
		/// <summary>
		/// This enumerator defines the available blob types for the import of old version dictionary files.
		/// </summary>
		private enum TBlobType
		{
			questionaudio = 0, answeraudio, questionexampleaudio, answerexampleaudio,
			questionvideo, answervideo, image
		};
		/// <summary>
		/// Enumeration which defines the values for the 'Synonyms' options:
		/// Full - all synonyms need to be known,
		/// Half - half need to be known,
		/// One - one need to be known,
		/// First - the card is promoted when the first synonym is known,
		/// Promp - prompt when not all were correct
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-26</remarks>
		private enum TSynonymGrading { Full = 0, Half, One, First, Prompt };

		#region nested classes required to load old import files
		/// <summary>
		/// rBox (-> tabBox) stores data for each box - required for the import of v1 dictionaries
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-20</remarks>
		private class _rBox_
		{
			public int First, Last, MaxLen, Len;				//First & last card ID, max size

			public _rBox_()
			{
				First = 0;
				Last = 0;
				MaxLen = 0;
				Len = 0;
			}
		}

		/// <summary>
		/// rChapter (-> tabChapter) stores chapter data - required for the import of v1 dictionaries
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-20</remarks>
		private class _rChapter_
		{
			public string Title;								//Title
			public string Description;							//Description

			public _rChapter_()
			{
				Title = string.Empty;
				Description = string.Empty;
			}
		}

		/// <summary>
		/// rCards (-> tabCards) contains the actual cards - required for the import of v1 dictionaries
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-20</remarks>
		private class _rCards_
		{
			public StringList Src, Dst;								//Q & A
			public string SentSrc, SentDst;						//examples
			public int ChapterID;								//ID pointing to tabChapter
			public byte CardParts;								//used parts of the card
			//public byte Pr;										//probability

			public _rCards_()
			{
				Src = new StringList();
				Dst = new StringList();
				SentSrc = string.Empty;
				SentDst = string.Empty;
				ChapterID = 0;
				CardParts = (byte)TCardPart.None;
				//Pr = 0;
			}
		}

		/// <summary>
		/// rBoxes (-> tabBoxes) orders the cards in arrayList box - required for the import of v1 dictionaries
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-20</remarks>
		private class _rBoxes_
		{
			public int BoxID;									//ID pointing to tabBox
			public int CardID;									//ID pointing to tabCards
			public int Prior, Next;								//prior & next in Box (dl list)

			public _rBoxes_()
			{
				BoxID = 1;
				CardID = 0;
				Prior = 0;
				Next = 0;
			}
		}

		/// <summary>
		/// rBlob (-> tabBlobs) stores blob data - only used for importing old files!
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-20</remarks>
		private class _rBlob_
		{
			public TBlobType SrcDst;							//the blob type
			public string Link;									//link to the blob (relative)
			public int CardID;									//CardID it belongs to

			public _rBlob_()
			{
				SrcDst = TBlobType.image;
				Link = string.Empty;
				CardID = 0;
			}
		}

		/// <summary>
		///  rStats (-> tabStats) stores statistics - required for the import of v1 dictionaries
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-07-20</remarks>
		private class _rStats_
		{
			public System.DateTime SStart, SEnd;				//Start & End of session
			public int Right, Wrong;						    //Number of rights & wrongs
			public int[] Boxes;									//Number of card in boxes

			public _rStats_(int nrBox)
			{
				SStart = System.DateTime.Now.ToUniversalTime();
				SEnd = System.DateTime.Now.ToUniversalTime();
				Right = 0;
				Wrong = 0;
				Boxes = new int[nrBox];
			}
		}
		#endregion

		#region helper methods
		private System.Array SetLength(System.Array oldArray, int newSize)
		{
			int oldSize = oldArray.Length;
			System.Type elementType = oldArray.GetType().GetElementType();
			System.Array newArray = System.Array.CreateInstance(elementType, newSize);
			int preserveLength = System.Math.Min(oldSize, newSize);
			if (preserveLength > 0)
				System.Array.Copy(oldArray, newArray, preserveLength);
			return newArray;
		}

		/// <summary>
		/// Converts arrayList byte array to arrayList string.
		/// </summary>
		/// <param name="data">byte array</param>
		/// <returns>string</returns>
		/// <remarks>Documented by Dev03, 2007-07-26</remarks>
		private string byte2string(byte[] data)
		{
			return m_SourceEncoding.GetString(data);
		}

		/// <summary>
		/// This method translates the skin path given in the old dictionary format to the new format.
		/// Only required during import of old version dictionaries.
		/// </summary>
		/// <param name="sdir">Old version skin path</param>
		/// <param name="dicPath">The dicionary path.</param>
		/// <param name="appPath">The application path.</param>
		/// <returns>Translated skin path</returns>
		/// <remarks>Documented by Dev03, 2007-07-26</remarks>
		public string SkinDir2RealDir(string sdir, string dicPath, string appPath)
		{
			if (sdir.Equals("") || File.Exists(sdir))
				return sdir;

			if (sdir.IndexOf("($SKINS)") == 0)
				return sdir.Replace("($SKINS)", Path.Combine(appPath, "skins"));
			else if (sdir.IndexOf("($PRGM)") == 0)
				return sdir.Replace("($PRGM)", appPath);
			else
				return Path.Combine(dicPath, sdir);
		}
		#endregion

		/// <summary>
		/// Reports the progress update.
		/// </summary>
		/// <param name="percent">The process advancement percent.</param>
		/// <returns>True if the process has been canceled.</returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		private bool ReportProgressUpdate(int percent)
		{
			bool cancelProcess = false;
			if (m_BackgroundWorker != null)
			{
				if (m_BackgroundWorker.CancellationPending)
				{
					cancelProcess = true;
				}
				else
				{
					m_BackgroundWorker.ReportProgress(percent);
				}
			}
			return !cancelProcess;
		}

		/// <summary>
		/// Loads dictionary from the old database format (ODF) and stores it to the current format.
		/// </summary>
		/// <param name="srcFile">The source database file.</param>
		/// <param name="dstFile">The destination database file.</param>
		/// <remarks>Documented by Dev03, 2007-07-26</remarks>
		public MLifter.DAL.Interfaces.IDictionary Load(string srcFile, string dstFile)
		{
			MLifter.DAL.Interfaces.IDictionary dictionary = null;
			int wbVersion;

#if !DEBUG
			try
			{
#endif
			string srcPath = Path.GetDirectoryName(srcFile);
			_rBox_[] box_data = new _rBox_[0];
			_rChapter_[] chapter_data = new _rChapter_[0];
			_rCards_[] cards_data = new _rCards_[0];
			_rBoxes_[] boxes_data = new _rBoxes_[0];
			_rBlob_[] blob_data = new _rBlob_[0];
			_rStats_[] stats_data = new _rStats_[0];

			string headerstr = string.Empty;
			UInt16 ssize;
			int len, rsize;

			using (FileStream DicFile = new FileStream(srcFile, FileMode.Open))
			{
				BinaryReader Dic = new BinaryReader(DicFile, System.Text.UnicodeEncoding.UTF8);
				// Check header first 
				len = Dic.ReadInt32();
				if (len < 30)
					headerstr = byte2string(Dic.ReadBytes(len));
				else
					headerstr = string.Empty;
				// Check version byte 
				wbVersion = Dic.ReadInt16();

				// Some code to correct "bugs" in previous dictionary versions
				if (wbVersion <= 0x0104)
				{
					throw new DictionaryFormatNotSupported(wbVersion);
				}

				// Header has to be okay, and Version smaller than prog's version 
				if (headerstr.Equals(m_odfFileHeaderString) && (wbVersion <= m_oldFileVersion))
				{
					if (File.Exists(dstFile))
					{
						throw new DictionaryPathExistsException(dstFile);
					}

					IUser user = UserFactory.Create(m_loginCallback, new ConnectionStringStruct(DatabaseType.Xml, dstFile, false), 
						(DataAccessErrorDelegate)delegate { return; }, this);
					dictionary =user.Open();
					len = Dic.ReadByte();
					dictionary.DefaultSettings.QuestionCaption = byte2string(Dic.ReadBytes(20)).Substring(0, len);
					len = Dic.ReadByte();
					dictionary.DefaultSettings.AnswerCaption = byte2string(Dic.ReadBytes(20)).Substring(0, len);

					Dic.ReadBytes(82); //SrcFont & DstFont
					Dic.ReadBytes(2); //SrcCharSet & DstCharSet

					Dic.ReadBoolean(); //ReadOnly

					ssize = Dic.ReadUInt16();
					dictionary.Author = byte2string(Dic.ReadBytes(ssize));

					ssize = Dic.ReadUInt16();
					dictionary.Description = byte2string(Dic.ReadBytes(ssize));

					if (wbVersion >= 0x0104)
					{
						ssize = Dic.ReadUInt16();
						string mediafolder = byte2string(Dic.ReadBytes(ssize)).TrimEnd(new char[] { Path.DirectorySeparatorChar });
						if (Directory.Exists(mediafolder))
							dictionary.MediaDirectory = mediafolder;
					}

					if (wbVersion >= 0x0107)
					{
						for (int i = 0; i < 12; i++)
						{
							ssize = Dic.ReadUInt16();
							dictionary.DefaultSettings.CommentarySounds[CommentarySoundIdentifier.Create(i > 6 ? Side.Answer : Side.Question,
								(ECommentarySoundType)(i > 6 ? i - 6 : i))] =
							   new MLifter.DAL.XML.XmlAudio(SkinDir2RealDir(byte2string(Dic.ReadBytes(ssize)), Path.GetDirectoryName(dstFile), m_applicationPath),
							   new MLifter.DAL.Tools.ParentClass(user, dictionary));
						}
					}

					if (wbVersion >= 0x0108)
					{
						Dic.ReadBytes(10); //Pwd
					}
					if (wbVersion >= 0x0109)
					{
						dictionary.Category = new Category((int)Dic.ReadByte(), false);
					}
					else
					{
						dictionary.Category = new Category(m_defaultCategory, true);
					}

					(dictionary as MLifter.DAL.XML.XmlDictionary).score = Dic.ReadInt32();
					dictionary.HighScore = Dic.ReadInt32();

					int queryChapter = 0;
					rsize = Dic.ReadInt32();
					dictionary.DefaultSettings.SelectedLearnChapters.Clear();
					for (int i = 0; i < rsize; i++)
					{
						queryChapter = Math.Abs(Dic.ReadInt32()); // BUG?? what is the minus for??
						if (queryChapter < 0) //chapterID starts with 1, should be zero-based
							queryChapter++;
						else
							queryChapter--;

						dictionary.DefaultSettings.SelectedLearnChapters.Add(queryChapter);
					}

					Dic.ReadUInt16();   //the LastBox user property is not used anymore 
					EQueryDirection dir = (EQueryDirection)Dic.ReadByte();
					switch (dir)
					{
						case EQueryDirection.Question2Answer:
							dictionary.DefaultSettings.QueryDirections.Question2Answer = true;
							break;
						case EQueryDirection.Answer2Question:
							dictionary.DefaultSettings.QueryDirections.Answer2Question = true;
							break;
						case EQueryDirection.Mixed:
							dictionary.DefaultSettings.QueryDirections.Mixed = true;
							break;
					}

					int queryType = Convert.ToInt32(Dic.ReadByte());
					dictionary.DefaultSettings.QueryTypes.ImageRecognition = ((((int)EQueryType.ImageRecognition) & queryType) > 0);
					dictionary.DefaultSettings.QueryTypes.ListeningComprehension = ((((int)EQueryType.ListeningComprehension) & queryType) > 0);
					dictionary.DefaultSettings.QueryTypes.MultipleChoice = ((((int)EQueryType.MultipleChoice) & queryType) > 0);
					dictionary.DefaultSettings.QueryTypes.Sentence = ((((int)EQueryType.Sentences) & queryType) > 0);
					dictionary.DefaultSettings.QueryTypes.Word = ((((int)EQueryType.Word) & queryType) > 0);

					int queryOptions;
					if (wbVersion >= 0x0103)
					{
						queryOptions = Convert.ToInt32(Dic.ReadUInt16());
					}
					else
					{
						queryOptions = Convert.ToInt32(Dic.ReadByte());
					}

					dictionary.DefaultSettings.CaseSensitive = ((((int)EQueryOption.CaseSensitive) & queryOptions) > 0);
					dictionary.DefaultSettings.EnableTimer = ((((int)EQueryOption.CountDown) & queryOptions) > 0);
					dictionary.DefaultSettings.ShowStatistics = ((((int)EQueryOption.Stats) & queryOptions) > 0);
					dictionary.DefaultSettings.ShowImages = ((((int)EQueryOption.Images) & queryOptions) > 0);
					dictionary.DefaultSettings.AutoplayAudio = ((((int)EQueryOption.Sounds) & queryOptions) > 0);
					dictionary.DefaultSettings.EnableCommentary = ((((int)EQueryOption.Commentary) & queryOptions) > 0);
					dictionary.DefaultSettings.CorrectOnTheFly = ((((int)EQueryOption.Correct) & queryOptions) > 0);
					dictionary.DefaultSettings.SkipCorrectAnswers = ((((int)EQueryOption.Skip) & queryOptions) > 0);
					dictionary.DefaultSettings.SelfAssessment = ((((int)EQueryOption.Self) & queryOptions) > 0);
					dictionary.DefaultSettings.RandomPool = ((((int)EQueryOption.RandomPool) & queryOptions) > 0);
					dictionary.DefaultSettings.ConfirmDemote = ((((int)EQueryOption.ConfirmDemote) & queryOptions) > 0);

					if (wbVersion >= 0x0101)
					{
						EGradeSynonyms gradeS = (EGradeSynonyms)Dic.ReadByte();
						switch (gradeS)
						{
							case EGradeSynonyms.AllKnown:
								dictionary.DefaultSettings.GradeSynonyms.AllKnown = true;
								break;
							case EGradeSynonyms.HalfKnown:
								dictionary.DefaultSettings.GradeSynonyms.HalfKnown = true;
								break;
							case EGradeSynonyms.OneKnown:
								dictionary.DefaultSettings.GradeSynonyms.OneKnown = true;
								break;
							case EGradeSynonyms.FirstKnown:
								dictionary.DefaultSettings.GradeSynonyms.FirstKnown = true;
								break;
							case EGradeSynonyms.Prompt:
								dictionary.DefaultSettings.GradeSynonyms.Prompt = true;
								break;
						}
					}
					else
					{
						dictionary.DefaultSettings.GradeSynonyms.OneKnown = true;
					}

					dictionary.DefaultSettings.PoolEmptyMessageShown = Dic.ReadBoolean();
					dictionary.DefaultSettings.UseLMStylesheets = false;

					ssize = Dic.ReadUInt16();
					Dic.ReadBytes(ssize); //Skin
					Dic.ReadBoolean(); //UseSkin

					string stripchars = String.Empty;
					if (wbVersion >= 0x0102)
					{
						byte[] temp = Dic.ReadBytes(32);
						for (int i = 0; i < 8; i++)
						{
							for (int j = 0; j < 8; j++)
							{
								if ((temp[i] & (0x0001 << j)) > 0)
								{
									stripchars += (char)(i * 8 + j);
								}
							}
						}
						dictionary.DefaultSettings.StripChars = stripchars;
					}

					EGradeTyping gradeT = (EGradeTyping)Dic.ReadByte();
					switch (gradeT)
					{
						case EGradeTyping.AllCorrect:
							dictionary.DefaultSettings.GradeTyping.AllCorrect = true;
							break;
						case EGradeTyping.HalfCorrect:
							dictionary.DefaultSettings.GradeTyping.HalfCorrect = true;
							break;
						case EGradeTyping.NoneCorrect:
							dictionary.DefaultSettings.GradeTyping.NoneCorrect = true;
							break;
						case EGradeTyping.Prompt:
							dictionary.DefaultSettings.GradeTyping.Prompt = true;
							break;
						default:
							break;
					}

					if ((((int)ESnoozeMode.QuitProgram) & queryOptions) > 0)
					{
						dictionary.DefaultSettings.SnoozeOptions.SnoozeMode = ESnoozeMode.QuitProgram;
					}
					if ((((int)ESnoozeMode.SendToTray) & queryOptions) > 0)
					{
						dictionary.DefaultSettings.SnoozeOptions.SnoozeMode = ESnoozeMode.SendToTray;
					}

					if (!ReportProgressUpdate(5)) return null;

					if (wbVersion >= 0x0103)
					{
						int snoozeTime = Convert.ToInt32(Dic.ReadByte());
						int snoozeRights = Convert.ToInt32(Dic.ReadByte());
						int snoozeCards = Convert.ToInt32(Dic.ReadByte());
						int snoozeTimeLow = Convert.ToInt32(Dic.ReadByte());
						int snoozeTimeHigh = Convert.ToInt32(Dic.ReadByte());
						if ((((int)ESnoozeMode.Time) & queryType) > 0)
						{
							dictionary.DefaultSettings.SnoozeOptions.EnableTime(snoozeTime);
						}
						if ((((int)ESnoozeMode.Rights) & queryType) > 0)
						{
							dictionary.DefaultSettings.SnoozeOptions.EnableRights(snoozeRights);
						}
						if ((((int)ESnoozeMode.Cards) & queryType) > 0)
						{
							dictionary.DefaultSettings.SnoozeOptions.EnableCards(snoozeCards);
						}
						dictionary.DefaultSettings.SnoozeOptions.SetSnoozeTimes(snoozeTimeLow, snoozeTimeHigh);
					}
					//box_data -> user
					rsize = Dic.ReadInt32();
					box_data = (_rBox_[])SetLength(box_data, rsize);
					for (int i = 0; i < box_data.Length; i++)
					{
						box_data[i] = new _rBox_();
						box_data[i].First = Dic.ReadInt32();
						Dic.ReadInt32(); // box_data[i].Last

						if (i < box_data.Length - 2)
						{
							dictionary.Boxes.Box[i].MaximalSize = Dic.ReadInt32();
						}
						else
						{
							Dic.ReadInt32(); //MaxLen for the last two boxes - not needed
						}

						if (wbVersion > 0x0106)
						{
							Dic.ReadInt32(); // box_data[i].Len
						}
					}
					// end of user -----------------------------------------------------------------

					if (!ReportProgressUpdate(10)) return null;

					//chapter_data
					rsize = Dic.ReadInt32();
					chapter_data = (_rChapter_[])SetLength(chapter_data, rsize);
					for (int i = 0; i < chapter_data.Length; i++)
					{
						chapter_data[i] = new _rChapter_();
						Dic.ReadUInt16(); //ID
						Dic.ReadUInt16(); // SubID
						len = Dic.ReadByte();
						chapter_data[i].Title = byte2string(Dic.ReadBytes(30)).Substring(0, len);
						len = Dic.ReadByte();
						chapter_data[i].Description = byte2string(Dic.ReadBytes(256)).Substring(0, len);
					}

					if (!ReportProgressUpdate(15)) return null;

					//cards_data
					rsize = Dic.ReadInt32();
					cards_data = (_rCards_[])SetLength(cards_data, rsize);
					for (int i = 0; i < cards_data.Length; i++)
					{
						cards_data[i] = new _rCards_();
						ssize = Dic.ReadUInt16();
						cards_data[i].Src.SpecialImportFormat = byte2string(Dic.ReadBytes(ssize));
						ssize = Dic.ReadUInt16();
						cards_data[i].Dst.SpecialImportFormat = byte2string(Dic.ReadBytes(ssize));
						ssize = Dic.ReadUInt16();
						cards_data[i].SentSrc = byte2string(Dic.ReadBytes(ssize));
						ssize = Dic.ReadUInt16();
						cards_data[i].SentDst = byte2string(Dic.ReadBytes(ssize));
						cards_data[i].ChapterID = Dic.ReadUInt16() - 1; //chapterID starts with 1, should be zero-based
						cards_data[i].CardParts = Dic.ReadByte();
						if (wbVersion >= 0x0107)
							Dic.ReadByte(); // Probability
					}

					if (!ReportProgressUpdate(20)) return null;

					//boxes_data
					rsize = Dic.ReadInt32();
					boxes_data = (_rBoxes_[])SetLength(boxes_data, rsize);
					for (int i = 0; i < boxes_data.Length; i++)
					{
						boxes_data[i] = new _rBoxes_();
						boxes_data[i].BoxID = Dic.ReadByte();
						Dic.ReadBytes(3); //TODO BUG ?
						boxes_data[i].CardID = Dic.ReadInt32();
						boxes_data[i].Prior = Dic.ReadInt32();
						boxes_data[i].Next = Dic.ReadInt32();
					}

					if (!ReportProgressUpdate(25)) return null;

					//blob_data
					rsize = Dic.ReadInt32();
					blob_data = (_rBlob_[])SetLength(blob_data, rsize);
					for (int i = 0; i < blob_data.Length; i++)
					{
						blob_data[i] = new _rBlob_();
						blob_data[i].SrcDst = (TBlobType)Dic.ReadByte();
						len = Dic.ReadByte();
						string tstr = byte2string(Dic.ReadBytes(101));
						blob_data[i].Link = tstr.Substring(0, len);
						blob_data[i].Link = blob_data[i].Link.Replace('/', '\\'); // repair work
						blob_data[i].Link = blob_data[i].Link.Replace("\\\\", "\\"); // repair work
						Dic.ReadByte();
						blob_data[i].CardID = Dic.ReadInt32();
					}

					if (!ReportProgressUpdate(30)) return null;

					//stats_data
					rsize = Dic.ReadInt32();
					stats_data = (_rStats_[])SetLength(stats_data, rsize);
					for (int i = 0; i < stats_data.Length; i++)
					{
						stats_data[i] = new _rStats_(m_nrBox);
						stats_data[i].SStart = new System.DateTime(1899, 12, 30, 0, 0, 0); // startdate in delphi
						stats_data[i].SStart = stats_data[i].SStart.AddDays(Dic.ReadDouble());
						stats_data[i].SEnd = new System.DateTime(1899, 12, 30, 0, 0, 0); // startdate in delphi
						stats_data[i].SEnd = stats_data[i].SEnd.AddDays(Dic.ReadDouble());
						stats_data[i].Right = Dic.ReadInt32();
						stats_data[i].Wrong = Dic.ReadInt32();
						for (int j = 0; j < m_nrBox; j++)
							stats_data[i].Boxes[j] = Dic.ReadInt32();
					}

					if (!ReportProgressUpdate(35)) return null;

					//chapters
					for (int i = 0; i < chapter_data.Length; i++)
					{
						IChapter chapter = dictionary.Chapters.AddNew();
						//chapter.Id = i; //Interface change
						chapter.Title = chapter_data[i].Title;
						chapter.Description = chapter_data[i].Description;
					}

					if (!ReportProgressUpdate(40)) return null;

					//cards
					for (int i = 0; i < cards_data.Length; i++)
					{
						if ((i % 10) == 0)
						{
							int progress = (int)Math.Floor(35.0 + ((90.0 - 35.0) * (i + 1) / cards_data.Length));
							if (!ReportProgressUpdate(progress)) return null;
						}
						ICard card = dictionary.Cards.AddNew();
						//card.Id = i;  //Interface change
						StringList words = new StringList();
						words.CommaText = cards_data[i].Src.CommaText;
						for (int k = 0; k < words.Count; k++)
						{
							IWord word = card.Question.CreateWord(words[k], WordType.Word, (k == 0));
							card.Question.AddWord(word);
						}
						card.QuestionExample.AddWord(card.QuestionExample.CreateWord(cards_data[i].SentSrc, WordType.Sentence, false));
						words.CommaText = cards_data[i].Dst.CommaText;
						for (int k = 0; k < words.Count; k++)
						{
							IWord word = card.Answer.CreateWord(words[k], WordType.Word, (k == 0));
							card.Answer.AddWord(word);
						}
						card.AnswerExample.AddWord(card.AnswerExample.CreateWord(cards_data[i].SentDst, WordType.Sentence, false));
						card.Chapter = cards_data[i].ChapterID;
						card.Box = 0;
						card.Timestamp = DateTime.Now;

						for (int j = 0; j < blob_data.Length; j++)
						{
							if (blob_data[j].CardID == i)
							{
								if (File.Exists(Path.Combine(srcPath, blob_data[j].Link)))
								{
									blob_data[j].Link = Path.Combine(srcPath, blob_data[j].Link);
									if (blob_data[j].SrcDst.ToString().Equals("image"))
									{
										IMedia media = card.CreateMedia(EMedia.Image, blob_data[j].Link, true, false, false);
										card.AddMedia(media, Side.Question);
										card.AddMedia(media, Side.Answer);
									}
									else
									{
										IMedia media = null;
										Side side = Side.Question;
										switch (blob_data[j].SrcDst.ToString())
										{
											case "questionaudio":
												media = card.CreateMedia(EMedia.Audio, blob_data[j].Link, true, true, false);
												side = Side.Question;
												break;
											case "questionexampleaudio":
												media = card.CreateMedia(EMedia.Audio, blob_data[j].Link, true, false, true);
												side = Side.Question;
												break;
											case "questionvideo":
												media = card.CreateMedia(EMedia.Video, blob_data[j].Link, true, false, false);
												side = Side.Question;
												break;
											case "questionimage":
												media = card.CreateMedia(EMedia.Image, blob_data[j].Link, true, false, false);
												side = Side.Question;
												break;
											case "answeraudio":
												media = card.CreateMedia(EMedia.Audio, blob_data[j].Link, true, true, false);
												side = Side.Answer;
												break;
											case "answerexampleaudio":
												media = card.CreateMedia(EMedia.Audio, blob_data[j].Link, true, false, true);
												side = Side.Answer;
												break;
											case "answervideo":
												media = card.CreateMedia(EMedia.Video, blob_data[j].Link, true, false, false);
												side = Side.Answer;
												break;
											case "answerimage":
												media = card.CreateMedia(EMedia.Image, blob_data[j].Link, true, false, false);
												side = Side.Answer;
												break;
											case "unusedmedia":
											default:
												try
												{
													EMedia mType = Helper.GetMediaType(blob_data[j].Link);
													if (mType == EMedia.Unknown)
													{
														media = card.CreateMedia(mType, blob_data[j].Link, false, false, false);
														side = Side.Question;    //doesn't matter
													}
												}
												catch { }
												break;
										}
										if (media != null)
										{
											card.AddMedia(media, side);
										}
									}
								}
							}
						}
					}

					if (!ReportProgressUpdate(95)) return null;

					//stats
					for (int i = 0; i < stats_data.Length; i++)
					{
						IStatistic stat = new MLifter.DAL.XML.XmlStatistic(dictionary as MLifter.DAL.XML.XmlDictionary, i, true);
						dictionary.Statistics.Add(stat);
						stat.StartTimestamp = stats_data[i].SStart;
						stat.EndTimestamp = stats_data[i].SEnd;
						stat.Right = stats_data[i].Right;
						stat.Wrong = stats_data[i].Wrong;
						stat.Boxes.Clear();
						for (int j = 0; j < stats_data[i].Boxes.Length; j++)
						{
							stat.Boxes.Add(stats_data[i].Boxes[j]);
						}
					}

					DateTime start_of_year = new DateTime(m_releaseYear, 1, 1);
					int additional_seconds = 1;

					for (int i = box_data.Length - 1; i >= 0; i--)
					{
						int exitCounter = 0;
						for (int j = box_data[i].First; j != (-1); j = boxes_data[j].Next)
						{
							try
							{
								ICard card = dictionary.Cards.Get(boxes_data[j].CardID);
								card.Box = boxes_data[j].BoxID;
								card.Timestamp = start_of_year.AddSeconds(additional_seconds++).ToUniversalTime();
							}
							catch { }
							//boxes_data seems to be a linked list which sometimes may be broken - this is the emergency exit
							if (exitCounter++ > dictionary.Cards.Count) break;
						}
					}

					dictionary.Save();

				}
				else
				{
					throw new InvalidImportFormatException(dstFile);
				}

				Dic.Close();
			}
#if !DEBUG
			}
			catch (DictionaryPathExistsException ex)
			{
				throw ex;
			}
			catch (DictionaryFormatNotSupported ex)
			{
				throw ex;
			}
			catch (InvalidImportFormatException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				throw ex;
			}
#endif
			return dictionary;
		}
	}
}
