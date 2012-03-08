using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

namespace MLifter.DAL.V17
{
	public sealed class Importer
	{
        public Importer(string applicationPath)
        {
            m_applicationPath = applicationPath;
        }

        // Number of boxes is currently constant, default box sizes
        private const int m_nrBox = 10;
        private const int m_defaultCategory = 7;
        private const int m_dictionaryVersion = 0x010A;

        private string m_applicationPath;

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

            public _rStats_()
            {
                SStart = System.DateTime.Now.ToUniversalTime();
                SEnd = System.DateTime.Now.ToUniversalTime();
                Right = 0;
                Wrong = 0;
                Boxes = new int[m_nrBox];
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
            return System.Text.Encoding.ASCII.GetString(data);
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
        /// Loads arrayList dictionary from the old database format and stores it to the new format.
        /// </summary>
        /// <param name="filename">Filename of the old version dictionary file</param>
        /// <remarks>Documented by Dev03, 2007-07-26</remarks>
        public void Load(string filename)
        {
            int wbVersion;
            /*
            try
            {
                _rBox_[] box_data = new _rBox_[0];
                _rChapter_[] chapter_data = new _rChapter_[0];
                _rCards_[] cards_data = new _rCards_[0];
                _rBoxes_[] boxes_data = new _rBoxes_[0];
                _rBlob_[] blob_data = new _rBlob_[0];
                _rStats_[] stats_data = new _rStats_[0];

                string headerstr = string.Empty;
                UInt16 ssize;
                int len, rsize;
                FileStream DicFile = null;
                BinaryReader Dic = null;
                StreamReader XmlReadFileStream = null;
                XmlTextReader XmlReadFile = null;
                XmlTextWriter XmlFile = null;

                DicFile = new FileStream(filename, FileMode.Open);
                Dic = new BinaryReader(DicFile, System.Text.UnicodeEncoding.UTF8);
                // Check header first 
                len = Dic.ReadInt32();
                if (len < 30)
                    headerstr = byte2string(Dic.ReadBytes(len));
                else
                    headerstr = string.Empty;
                // Check version byte 
                wbVersion = Dic.ReadInt16();
                // Header has to be okay, and Version smaller than prog's version 
                if (headerstr.Equals(Resources.DIC_HEADER_ODFFILE_TEXT) && (wbVersion <= FILE.OLD_FILE_VERSION))
                {
                    //change filename for new XML file
                    string oldfilename = Path.GetFileName(filename);
                    filename = Path.ChangeExtension(filename, "." + MLIFTER.FILE_EXTENSION);
                    if (File.Exists(filename))
                    {
                        throw new DictionaryPathExistsException(filename);
                    }

                    XmlFile = new XmlTextWriter(filename, System.Text.UnicodeEncoding.Unicode);
                    XmlFile.Formatting = System.Xml.Formatting.Indented;
                    XmlFile.Flush();
                    XmlFile.WriteStartDocument();
                    XmlFile.WriteComment(FILE.FILE_VERSION);
                    XmlFile.WriteComment(FILE.FILE_COPYRIGHT);
                    XmlFile.WriteStartElement("dictionary");


                    // save general settings --------------------------------------------------------------------
                    XmlFile.WriteStartElement("general");
                    XmlFile.WriteElementString("version", m_dictionaryVersion.ToString());

                    len = Dic.ReadByte();
                    XmlFile.WriteElementString("questioncaption",
                        byte2string(Dic.ReadBytes(20)).Substring(0, len));

                    len = Dic.ReadByte();
                    XmlFile.WriteElementString("answercaption",
                        byte2string(Dic.ReadBytes(20)).Substring(0, len));

                    Dic.ReadBytes(82); //SrcFont & DstFont
                    Dic.ReadBytes(2); //SrcCharSet & DstCharSet

                    Dic.ReadBoolean(); //ReadOnly

                    ssize = Dic.ReadUInt16();
                    XmlFile.WriteElementString("author",
                        byte2string(Dic.ReadBytes(ssize)));

                    ssize = Dic.ReadUInt16();
                    XmlFile.WriteElementString("description",
                        byte2string(Dic.ReadBytes(ssize)));

                    if (wbVersion >= 0x0104)
                    {
                        ssize = Dic.ReadUInt16();
                        XmlFile.WriteElementString("sounddir",
                            byte2string(Dic.ReadBytes(ssize)));
                    }
                    else
                        XmlFile.WriteElementString("sounddir", "Media");

                    if (wbVersion >= 0x0107)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            XmlFile.WriteStartElement("commentsound");
                            XmlFile.WriteAttributeString("id", i.ToString());
                            ssize = Dic.ReadUInt16();
                            XmlFile.WriteString(SkinDir2RealDir(byte2string(Dic.ReadBytes(ssize)), Path.GetDirectoryName(filename), m_applicationPath));
                            XmlFile.WriteEndElement();
                        }
                    }
                    else
                        for (int i = 0; i < 12; i++)
                        {
                            XmlFile.WriteStartElement("commentsound");
                            XmlFile.WriteAttributeString("id", i.ToString());
                            XmlFile.WriteEndElement();
                        }

                    if (wbVersion >= 0x0108)
                        Dic.ReadBytes(10); //Pwd
                    if (wbVersion >= 0x0109)
                        XmlFile.WriteElementString("category",
                            XmlConvert.ToString((int)Dic.ReadByte()));
                    else
                        XmlFile.WriteElementString("category",
                            XmlConvert.ToString(m_defaultCategory));

                    XmlFile.WriteEndElement();

                    // save user settings ---------------------------------------------------------------------
                    XmlFile.WriteStartElement("user");

                    XmlFile.WriteElementString("score", XmlConvert.ToString(Dic.ReadInt32()));
                    XmlFile.WriteElementString("hiscore", XmlConvert.ToString(Dic.ReadInt32()));

                    int querychapter_temp = 0;
                    rsize = Dic.ReadInt32();
                    for (int i = 0; i < rsize; i++)
                    {
                        querychapter_temp = Math.Abs(Dic.ReadInt32()); // BUG?? what is the minus for??
                        if (querychapter_temp < 0) //chapterID starts with 1, should be zero-based
                            querychapter_temp++;
                        else
                            querychapter_temp--;
                        XmlFile.WriteStartElement("querychapter");
                        XmlFile.WriteAttributeString("id", i.ToString());
                        XmlFile.WriteString(XmlConvert.ToString(querychapter_temp));
                        XmlFile.WriteEndElement();
                    }

                    XmlFile.WriteElementString("querydir", XmlConvert.ToString(Dic.ReadByte()));
                    XmlFile.WriteElementString("querytype", XmlConvert.ToString(Dic.ReadByte()));
                    if (wbVersion >= 0x0103)
                        XmlFile.WriteElementString("queryoptions", XmlConvert.ToString(Dic.ReadUInt16()));
                    else
                        XmlFile.WriteElementString("queryoptions", XmlConvert.ToString(Dic.ReadByte()));
                    if (wbVersion >= 0x0101)
                        XmlFile.WriteElementString("gradesynonyms", XmlConvert.ToString(Dic.ReadByte()));
                    else
                        XmlFile.WriteElementString("gradesynonyms", XmlConvert.ToString((int)TSynonymGrading.One));
                    XmlFile.WriteElementString("emptymessage", XmlConvert.ToString(Dic.ReadBoolean()));

                    XmlFile.WriteElementString("usedicstyle", XmlConvert.ToString(false));

                    ssize = Dic.ReadUInt16();
                    Dic.ReadBytes(ssize); //Skin
                    Dic.ReadBoolean(); //UseSkin

                    string stripchars = "";
                    if (wbVersion >= 0x0102)
                    {
                        byte[] temp = Dic.ReadBytes(32);
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                                if ((temp[i] & (0x0001 << j)) > 0) stripchars += (char)(i * 8 + j);
                    }
                    else
                        stripchars = ".!?;,";
                    XmlFile.WriteElementString("stripchars", stripchars);

                    XmlFile.WriteElementString("gradetyping", XmlConvert.ToString(Dic.ReadByte()));
                    if (wbVersion >= 0x0103)
                    {
                        XmlFile.WriteElementString("end-time", XmlConvert.ToString(Dic.ReadByte()));
                        XmlFile.WriteElementString("end-rights", XmlConvert.ToString(Dic.ReadByte()));
                        XmlFile.WriteElementString("end-cards", XmlConvert.ToString(Dic.ReadByte()));
                        XmlFile.WriteElementString("snooze-low", XmlConvert.ToString(Dic.ReadByte()));
                        XmlFile.WriteElementString("snooze-high", XmlConvert.ToString(Dic.ReadByte()));
                    }
                    else
                    {
                        XmlFile.WriteElementString("end-time", XmlConvert.ToString(10));
                        XmlFile.WriteElementString("end-rights", XmlConvert.ToString(10));
                        XmlFile.WriteElementString("end-cards", XmlConvert.ToString(5));
                        XmlFile.WriteElementString("snooze-low", XmlConvert.ToString(55));
                        XmlFile.WriteElementString("snooze-high", XmlConvert.ToString(110));
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
                            XmlFile.WriteStartElement("boxsize");
                            XmlFile.WriteAttributeString("id", (i + 1).ToString());
                            XmlFile.WriteString(XmlConvert.ToString(Dic.ReadInt32())); //MaxLen
                            XmlFile.WriteEndElement();
                        }
                        else
                            Dic.ReadInt32(); //MaxLen for the last two boxes - not needed

                        if (wbVersion > 0x0106)
                            Dic.ReadInt32(); // box_data[i].Len
                    }
                    XmlFile.WriteEndElement();
                    // end of user -----------------------------------------------------------------

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

                    //blob_data
                    rsize = Dic.ReadInt32();
                    blob_data = (_rBlob_[])SetLength(blob_data, rsize);
                    for (int i = 0; i < blob_data.Length; i++)
                    {
                        blob_data[i] = new _rBlob_();
                        blob_data[i].SrcDst = (TBlobType)Dic.ReadByte();
                        len = Dic.ReadByte();
                        blob_data[i].Link = byte2string(Dic.ReadBytes(101)).Substring(0, len);
                        blob_data[i].Link = blob_data[i].Link.Replace('/', '\\'); // repair work
                        blob_data[i].Link = blob_data[i].Link.Replace("\\\\", "\\"); // repair work
                        Dic.ReadByte();
                        blob_data[i].CardID = Dic.ReadInt32();
                    }

                    //stats_data
                    rsize = Dic.ReadInt32();
                    stats_data = (_rStats_[])SetLength(stats_data, rsize);
                    for (int i = 0; i < stats_data.Length; i++)
                    {
                        stats_data[i] = new _rStats_();
                        stats_data[i].SStart = new System.DateTime(1899, 12, 30, 0, 0, 0); // startdate in delphi
                        stats_data[i].SStart = stats_data[i].SStart.AddDays(Dic.ReadDouble());
                        stats_data[i].SEnd = new System.DateTime(1899, 12, 30, 0, 0, 0); // startdate in delphi
                        stats_data[i].SEnd = stats_data[i].SEnd.AddDays(Dic.ReadDouble());
                        stats_data[i].Right = Dic.ReadInt32();
                        stats_data[i].Wrong = Dic.ReadInt32();
                        for (int j = 0; j < m_nrBox; j++)
                            stats_data[i].Boxes[j] = Dic.ReadInt32();
                    }

                    // Some code to correct "bugs" in previous dictionary versions }
                    if (wbVersion <= 0x0104)
                    {
                        throw new DictionaryFormatNotSupported(wbVersion);
                    }

                    //cards
                    for (int i = 0; i < cards_data.Length; i++)
                    {
                        XmlFile.WriteStartElement("card");
                        XmlFile.WriteAttributeString("id", i.ToString());

                        // AWE: Removed standard stylesheet for loading the current stylesheet of the corresponding style

                        XmlFile.WriteElementString("questionstylesheet", FILE.INITIAL_QUESTION_STYLESHEET);
                        XmlFile.WriteElementString("question", cards_data[i].Src.CommaText);
                        XmlFile.WriteElementString("questionexample", cards_data[i].SentSrc);
                        XmlFile.WriteElementString("answerstylesheet", FILE.INITIAL_ANSWER_STYLESHEET);
                        XmlFile.WriteElementString("answer", cards_data[i].Dst.CommaText);
                        XmlFile.WriteElementString("answerexample", cards_data[i].SentDst);
                        XmlFile.WriteElementString("chapter", cards_data[i].ChapterID.ToString());
                        XmlFile.WriteElementString("box", "0");
                        XmlFile.WriteElementString("timestamp", XmlConvert.ToString(DateTime.Now.ToUniversalTime(), XmlDateTimeSerializationMode.RoundtripKind));

                        for (int j = 0; j < blob_data.Length; j++)
                        {
                            if (blob_data[j].CardID == i)
                            {
                                if (blob_data[j].SrcDst.ToString().Equals("image"))
                                {
                                    XmlFile.WriteElementString("questionimage", blob_data[j].Link);
                                    XmlFile.WriteElementString("answerimage", blob_data[j].Link);
                                }
                                else
                                {
                                    XmlFile.WriteStartElement(blob_data[j].SrcDst.ToString());

                                    // write std attribute
                                    if (blob_data[j].SrcDst.ToString().Equals("questionaudio") || blob_data[j].SrcDst.ToString().Equals("answeraudio"))
                                        XmlFile.WriteAttributeString("id", "std");

                                    XmlFile.WriteString(blob_data[j].Link);
                                    XmlFile.WriteEndElement();
                                }
                            }
                        }

                        XmlFile.WriteEndElement();
                    }

                    //chapters
                    for (int i = 0; i < chapter_data.Length; i++)
                    {
                        XmlFile.WriteStartElement("chapter");
                        XmlFile.WriteAttributeString("id", i.ToString());
                        XmlFile.WriteElementString("title", chapter_data[i].Title);
                        XmlFile.WriteElementString("description", chapter_data[i].Description);
                        XmlFile.WriteEndElement();
                    }

                    //stats
                    for (int i = 0; i < stats_data.Length; i++)
                    {
                        XmlFile.WriteStartElement("stats");
                        XmlFile.WriteAttributeString("id", i.ToString());
                        XmlFile.WriteElementString("start", System.Xml.XmlConvert.ToString(stats_data[i].SStart, XmlDateTimeSerializationMode.RoundtripKind));
                        XmlFile.WriteElementString("end", System.Xml.XmlConvert.ToString(stats_data[i].SEnd, XmlDateTimeSerializationMode.RoundtripKind));
                        XmlFile.WriteElementString("right", stats_data[i].Right.ToString());
                        XmlFile.WriteElementString("wrong", stats_data[i].Wrong.ToString());
                        for (int j = 0; j < stats_data[i].Boxes.Length; j++)
                        {
                            XmlFile.WriteStartElement("boxes");
                            XmlFile.WriteAttributeString("id", j.ToString());
                            XmlFile.WriteString(stats_data[i].Boxes[j].ToString());
                            XmlFile.WriteEndElement();
                        }
                        XmlFile.WriteEndElement();
                    }

                    XmlFile.WriteEndDocument();
                    XmlFile.Close();

                    XmlReadFileStream = new System.IO.StreamReader(filename, System.Text.UnicodeEncoding.Unicode);
                    XmlReadFile = new XmlTextReader(XmlReadFileStream);
                    XML_DICTIONARY = new System.Xml.XmlDocument();

                    // XmlException thrown if it is not arrayList valid xml file
                    XML_DICTIONARY.Load(XmlReadFile);

                    XmlReadFile.Close();
                    XmlReadFileStream.Close();

                    //XmlNode parent_card;
                    XmlNode card_box, card_timestamp;
                    XmlNodeList all_cards = XML_DICTIONARY.SelectNodes(XPATH.CARD);
                    bool[] boxdata_attached = new bool[all_cards.Count];

                    for (int i = 0; i < all_cards.Count; i++)
                        boxdata_attached[i] = false;

                    DateTime start_of_year = new DateTime(Convert.ToInt32(MLIFTER.RELEASE_YEAR), 1, 1);
                    int additional_seconds = 1;

                    for (int i = box_data.Length - 1; i >= 0; i--)
                    {
                        for (int j = box_data[i].First; j != (-1); j = boxes_data[j].Next)
                        {
                            card_box = XML_DICTIONARY.SelectSingleNode(XPATH.CARD_ID + boxes_data[j].CardID + XPATH.END_BOX);
                            card_box.InnerText = XmlConvert.ToString(boxes_data[j].BoxID);
                            card_timestamp = XML_DICTIONARY.SelectSingleNode(XPATH.CARD_ID + boxes_data[j].CardID + "']/timestamp");
                            card_timestamp.InnerText = XmlConvert.ToString(start_of_year.AddSeconds(additional_seconds++).ToUniversalTime(), XmlDateTimeSerializationMode.RoundtripKind); // Creating increasing timestamps
                        }
                    }

                    XML_DICTIONARY.Save(filename);

                    box_data = (_rBox_[])SetLength(box_data, 0);
                    chapter_data = (_rChapter_[])SetLength(chapter_data, 0);
                    cards_data = (_rCards_[])SetLength(cards_data, 0);
                    boxes_data = (_rBoxes_[])SetLength(boxes_data, 0);
                    blob_data = (_rBlob_[])SetLength(blob_data, 0);
                    stats_data = (_rStats_[])SetLength(stats_data, 0);

                    Dic.Close();
                    DicFile.Close();
                    return filename;
                }
                else
                {
                    throw new InvalidImportFormatException(filename);
                }
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
             */
        }
    }
}
