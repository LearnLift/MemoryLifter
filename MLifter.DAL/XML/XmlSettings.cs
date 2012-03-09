using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Preview;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    /// <summary>
    /// XML implementation of ISettings.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    class XmlSettings : ISettings
    {
        XmlDictionary dictionary;
        XmlDocument m_dictionary;
        XmlElement m_Settings;
        private XmlElement m_generalSettings, m_userSettings;
        private IQueryMultipleChoiceOptions m_MultipleChoiceOptions;

        private const string m_basePath = "/dictionary/user";
        private const string m_xpathGenSet = "/dictionary/general";
        private const string m_xpathQueryOptions = "/queryoptions";
        private const string m_xpathStripChars = "stripchars";
        private const string m_xpathGradeSynonyms = "gradesynonyms";
        private const string m_xpathGradeTyping = "gradetyping";
        private const string m_xpathQueryDir = "querydir";

        private const string m_xpathId = "id";
        private const string m_xpathIdFilter = "[@id={0}]";

        private const string m_xpathUsrSet = "/dictionary/user";
        private const string m_xpathAttributeId = "@id";
        private const string m_xpathLogo = "logo";
        private const string m_xpathQuestionCaption = "questioncaption";
        private const string m_xpathAnswerCaption = "answercaption";
        private const string m_xpathQuestionCulture = "questionculture";
        private const string m_xpathAnswerCulture = "answerculture";
        private const string m_xpathCommentSound = "commentsound";

        private List<string> m_audioComments;

        internal XmlSettings(XmlDictionary dictionary, ParentClass parent)
        {
            this.parent = parent;
            this.dictionary = dictionary;
            m_dictionary = dictionary.Dictionary;
            m_Settings = (XmlElement)m_dictionary.SelectSingleNode(m_basePath);
            m_MultipleChoiceOptions = new XmlQueryMultipleChoiceOptions(dictionary, Parent.GetChildParentClass(this));

            m_generalSettings = (XmlElement)m_dictionary.SelectSingleNode(m_xpathGenSet);
            m_userSettings = (XmlElement)m_dictionary.SelectSingleNode(m_xpathUsrSet);

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
        }

        #region ISettings Members

        /// <summary>
        /// Interface that defines the available query directions for a dictionary.
        /// </summary>
        /// <value></value>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public IQueryDirections QueryDirections
        {
            get
            {
                XmlQueryDirections queryDir = new XmlQueryDirections(Parent.GetChildParentClass(this));
                int dir = XmlConvert.ToInt32(m_userSettings[m_xpathQueryDir].InnerText);
                queryDir.Question2Answer = false;
                queryDir.Answer2Question = false;
                queryDir.Mixed = false;
                switch (dir)
                {
                    case 0:
                        queryDir.Question2Answer = true;
                        break;
                    case 1:
                        queryDir.Answer2Question = true;
                        break;
                    case 2:
                        queryDir.Mixed = true;
                        break;
                }
                queryDir.ValueChanged += new EventHandler(queryDir_ValueChanged);
                return queryDir;
            }
            set
            {
                if (value.Question2Answer.GetValueOrDefault() == true) m_userSettings[m_xpathQueryDir].InnerText = XmlConvert.ToString(0);
                else if (value.Answer2Question.GetValueOrDefault() == true) m_userSettings[m_xpathQueryDir].InnerText = XmlConvert.ToString(1);
                else m_userSettings[m_xpathQueryDir].InnerText = XmlConvert.ToString(2);
            }
        }

        void queryDir_ValueChanged(object sender, EventArgs e)
        {
            QueryDirections = sender as XmlQueryDirections;
        }

        public IQueryType QueryTypes
        {
            get
            {
                return new XmlQueryType(m_dictionary, Parent.GetChildParentClass(this));
            }
            set
            {
                //not implemented because modifications to the XmlQueryType object directly affect the underlying XmlDocument dictionary - AAB 20070802
            }
        }

        public IQueryMultipleChoiceOptions MultipleChoiceOptions
        {
            get
            {
                return m_MultipleChoiceOptions;
            }
            set
            {
                m_MultipleChoiceOptions.AllowMultipleCorrectAnswers = value.AllowMultipleCorrectAnswers;
                m_MultipleChoiceOptions.AllowRandomDistractors = value.AllowRandomDistractors;
                m_MultipleChoiceOptions.MaxNumberOfCorrectAnswers = value.MaxNumberOfCorrectAnswers;
                m_MultipleChoiceOptions.NumberOfChoices = value.NumberOfChoices;
            }
        }

        public IGradeTyping GradeTyping
        {
            get
            {
                XmlGradeTyping gradeTyping = new XmlGradeTyping(Parent.GetChildParentClass(this));
                int gradeTypingInt = XmlConvert.ToInt32(m_userSettings[m_xpathGradeTyping].InnerText);
                gradeTyping.AllCorrect = false;
                gradeTyping.HalfCorrect = false;
                gradeTyping.NoneCorrect = false;
                gradeTyping.Prompt = false;
                switch (gradeTypingInt)
                {
                    case 0:
                        gradeTyping.AllCorrect = true;
                        break;
                    case 1:
                        gradeTyping.HalfCorrect = true;
                        break;
                    case 2:
                        gradeTyping.NoneCorrect = true;
                        break;
                    case 3:
                        gradeTyping.Prompt = true;
                        break;
                    default:
                        break;
                }
                gradeTyping.ValueChanged += new EventHandler(gradeTyping_ValueChanged);
                return gradeTyping;
            }
            set
            {
                if (value.AllCorrect.GetValueOrDefault() == true) m_userSettings[m_xpathGradeTyping].InnerText = XmlConvert.ToString(0);
                else if (value.HalfCorrect.GetValueOrDefault() == true) m_userSettings[m_xpathGradeTyping].InnerText = XmlConvert.ToString(1);
                else if (value.NoneCorrect.GetValueOrDefault() == true) m_userSettings[m_xpathGradeTyping].InnerText = XmlConvert.ToString(2);
                else m_userSettings[m_xpathGradeTyping].InnerText = XmlConvert.ToString(3);
            }
        }

        void gradeTyping_ValueChanged(object sender, EventArgs e)
        {
            GradeTyping = sender as XmlGradeTyping;
        }

        public IGradeSynonyms GradeSynonyms
        {
            get
            {
                XmlGradeSynonyms gradeSynonyms = new XmlGradeSynonyms(Parent.GetChildParentClass(this));
                int gradeSynonymsInt = XmlConvert.ToInt32(m_userSettings[m_xpathGradeSynonyms].InnerText);
                gradeSynonyms.AllKnown = false;
                gradeSynonyms.FirstKnown = false;
                gradeSynonyms.HalfKnown = false;
                gradeSynonyms.OneKnown = false;
                gradeSynonyms.Prompt = false;
                switch (gradeSynonymsInt)
                {
                    case 0:
                        gradeSynonyms.AllKnown = true;
                        break;
                    case 1:
                        gradeSynonyms.HalfKnown = true;
                        break;
                    case 2:
                        gradeSynonyms.OneKnown = true;
                        break;
                    case 3:
                        gradeSynonyms.FirstKnown = true;
                        break;
                    case 4:
                        gradeSynonyms.Prompt = true;
                        break;
                    default:
                        break;
                }

                gradeSynonyms.ValueChanged += new EventHandler(gradeSynonyms_ValueChanged);

                return gradeSynonyms;
            }
            set
            {
                if (value.AllKnown.GetValueOrDefault() == true) m_userSettings[m_xpathGradeSynonyms].InnerText = XmlConvert.ToString(0);
                else if (value.HalfKnown.GetValueOrDefault() == true) m_userSettings[m_xpathGradeSynonyms].InnerText = XmlConvert.ToString(1);
                else if (value.OneKnown.GetValueOrDefault() == true) m_userSettings[m_xpathGradeSynonyms].InnerText = XmlConvert.ToString(2);
                else if (value.FirstKnown.GetValueOrDefault() == true) m_userSettings[m_xpathGradeSynonyms].InnerText = XmlConvert.ToString(3);
                else m_userSettings[m_xpathGradeSynonyms].InnerText = XmlConvert.ToString(4);
            }
        }

        void gradeSynonyms_ValueChanged(object sender, EventArgs e)
        {
            GradeSynonyms = sender as XmlGradeSynonyms;
        }

        public bool? AutoplayAudio
        {
            get
            {
                return Check(EQueryOption.Sounds);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Sounds);
                else
                    Unset(EQueryOption.Sounds);
            }
        }

        public bool? CaseSensitive
        {
            get
            {
                return Check(EQueryOption.CaseSensitive);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.CaseSensitive);
                else
                    Unset(EQueryOption.CaseSensitive);
            }
        }

        public bool? ConfirmDemote
        {
            get
            {
                return Check(EQueryOption.ConfirmDemote);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.ConfirmDemote);
                else
                    Unset(EQueryOption.ConfirmDemote);
            }
        }

        public bool? EnableCommentary
        {
            get
            {
                return Check(EQueryOption.Commentary);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Commentary);
                else
                    Unset(EQueryOption.Commentary);
            }
        }

        public bool? CorrectOnTheFly
        {
            get
            {
                return Check(EQueryOption.Correct);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Correct);
                else
                    Unset(EQueryOption.Correct);
            }
        }

        public bool? EnableTimer
        {
            get
            {
                return Check(EQueryOption.CountDown);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.CountDown);
                else
                    Unset(EQueryOption.CountDown);
            }
        }

        public bool? RandomPool
        {
            get
            {
                return Check(EQueryOption.RandomPool);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.RandomPool);
                else
                    Unset(EQueryOption.RandomPool);
            }
        }

        public bool? SelfAssessment
        {
            get
            {
                return Check(EQueryOption.Self);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Self);
                else
                    Unset(EQueryOption.Self);
            }
        }

        public bool? ShowImages
        {
            get
            {
                return Check(EQueryOption.Images);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Images);
                else
                    Unset(EQueryOption.Images);
            }
        }

        public string StripChars
        {
            get
            {
                return m_Settings[m_xpathStripChars].InnerText;
            }
            set
            {
                m_Settings[m_xpathStripChars].InnerText = value;
            }
        }

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

        public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds
        {
            get
            {
                Dictionary<CommentarySoundIdentifier, IMedia> commentarySounds = new Dictionary<CommentarySoundIdentifier, IMedia>();
                int i = 0;
                string dictionaryDir = System.IO.Path.GetDirectoryName(dictionary.Path);

                foreach (string sound in m_audioComments)
                {
                    //Actually, the commentarySounds are saved in a fix order (see Helper.CommentarySoundNames)
                    if (i < 6)
                    {
                        string absoluteSound = System.IO.Path.IsPathRooted(sound) ? sound : System.IO.Path.Combine(dictionaryDir, sound);

                        if (System.IO.File.Exists(absoluteSound))
                            commentarySounds[CommentarySoundIdentifier.Create(Side.Question, (ECommentarySoundType)i)] = new XmlAudio(absoluteSound, Parent.GetChildParentClass(this));
                    }
                    else
                    {
                        int index = i - 6;
                        string absoluteSound = System.IO.Path.IsPathRooted(sound) ? sound : System.IO.Path.Combine(dictionaryDir, sound);

                        if (System.IO.File.Exists(absoluteSound))
                            commentarySounds[CommentarySoundIdentifier.Create(Side.Answer, (ECommentarySoundType)index)] = new XmlAudio(absoluteSound, Parent.GetChildParentClass(this));
                    }
                    i++;
                }
                return commentarySounds;
            }
            set
            {
                if (m_audioComments.Count != Helper.CommentarySoundNames.Length)
                {
                    m_audioComments.Clear();
                    while (m_audioComments.Count < Helper.CommentarySoundNames.Length)
                        m_audioComments.Add(string.Empty);
                }
                foreach (KeyValuePair<CommentarySoundIdentifier, IMedia> commentarySound in value)
                {
                    int index = 0;
                    if (commentarySound.Key.Side == Side.Answer)
                        index += 6;
                    index += (int)commentarySound.Key.Type;
                    m_audioComments[index] =
                        ((XmlDictionary)Parent.GetParentDictionary()).AddCommentaryAudioToMediaFolder(commentarySound.Value.Filename, Helper.CommentarySoundNames[index]);
                }
                FlushListstoXml();
            }
        }

        public IMedia Logo
        {
            get
            {
                if (m_Settings[m_xpathLogo] != null)
                {
                    return new XmlImage(m_Settings[m_xpathLogo].InnerText, parent);
                }
                else
                {
                    return null;
                }

            }
            set
            {
                if (value == null)
                    return;

                if (m_Settings[m_xpathLogo] != null)
                    m_Settings[m_xpathLogo].InnerText = value.Filename;
                else
                    XmlHelper.CreateAndAppendElement(m_Settings, m_xpathLogo, value.Filename);
            }
        }

        public bool? ShowStatistics
        {
            get
            {
                return Check(EQueryOption.Stats);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Stats);
                else
                    Unset(EQueryOption.Stats);
            }
        }

        public bool? SkipCorrectAnswers
        {
            get
            {
                return Check(EQueryOption.Skip);
            }
            set
            {
                if (value.GetValueOrDefault())
                    Set(EQueryOption.Skip);
                else
                    Unset(EQueryOption.Skip);
            }
        }

        public ISnoozeOptions SnoozeOptions
        {
            get
            {
                return new XmlSnoozeOptions(m_dictionary, Parent.GetChildParentClass(this));
            }
            set
            {
                //not implemented because modifications to the XmlSnoozeOptions object directly affect the underlying XmlDocument dictionary - AAB 20070802
            }
        }

        public bool? PoolEmptyMessageShown
        {
            get
            {
                return dictionary.EmptyMessage;
            }
            set
            {
                dictionary.EmptyMessage = value.Value;
            }
        }

        public bool? UseLMStylesheets
        {
            get
            {
                return dictionary.UseDictionaryStyle;
            }
            set
            {
                dictionary.UseDictionaryStyle = value.Value;
            }
        }

        public IList<int> SelectedLearnChapters
        {
            get
            {
                return dictionary.QueryChapter;
            }
            set
            {
                dictionary.QueryChapter.Clear();
                if (value == null) return;
                foreach (int id in value)
                {
                    dictionary.QueryChapter.Add(id);
                }
            }
        }

        public ICardStyle Style
        {
            get
            {
                return dictionary.Style;
            }
            set
            {
                dictionary.Style = value;
            }
        }

        public CompiledTransform? QuestionStylesheet
        {
            get
            {
                return null;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        public CompiledTransform? AnswerStylesheet
        {
            get
            {
                return null;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        public bool? AutoBoxSize
        {
            get
            {
                return false;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        #endregion

        /// <summary>
        /// Sets the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        private void Set(EQueryOption option)
        {
            XmlConfigHelper.Set(m_dictionary, m_basePath + m_xpathQueryOptions, (int)option);
        }

        /// <summary>
        /// Unsets the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        private void Unset(EQueryOption option)
        {
            XmlConfigHelper.Unset(m_dictionary, m_basePath + m_xpathQueryOptions, (int)option);
        }

        /// <summary>
        /// Checks the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        private bool Check(EQueryOption option)
        {
            return XmlConfigHelper.Check(m_dictionary, m_basePath + m_xpathQueryOptions, (int)option);
        }

        /// <summary>
        /// Flushes the liststo XML.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-08-14</remarks>
        internal void FlushListstoXml()
        {
            XmlNodeList audioComments = m_generalSettings.SelectNodes(m_xpathCommentSound);
            foreach (XmlNode audioComment in audioComments)
            {
                audioComment.ParentNode.RemoveChild(audioComment);
            }
            for (int i = 0; i < m_audioComments.Count; i++)
            {
                XmlNode audioComment = m_generalSettings.SelectSingleNode(m_xpathCommentSound + String.Format(m_xpathIdFilter, i));
                if (audioComment == null)
                    XmlHelper.CreateElementWithAttribute(m_generalSettings, m_xpathCommentSound, m_audioComments[i].ToString(), m_xpathId, i.ToString());
                else
                    audioComment.InnerText = m_audioComments[i].ToString();
            }
        }

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            SettingsHelper.Copy(this, target as ISettings, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
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

    /// <summary>
    /// XML implementation of ISettings for allowed settings.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    class XmlAllowedSettings : ISettings
    {
        XmlDictionary dic;

        internal XmlAllowedSettings(XmlDictionary dictionary, ParentClass parent)
        {
            dic = dictionary;
            this.parent = parent;
        }

        #region ISettings Members

        public IQueryDirections QueryDirections
        {
            get
            {
                return dic.AllowedQueryDirections;
            }
            set
            {
                dic.AllowedQueryDirections = value;
            }
        }

        public IQueryType QueryTypes
        {
            get
            {
                return dic.AllowedQueryTypes;
            }
            set
            {
                dic.AllowedQueryTypes = value;
            }
        }

        [IgnoreCopy]
        public IQueryMultipleChoiceOptions MultipleChoiceOptions
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

        [IgnoreCopy]
        public IGradeTyping GradeTyping
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

        [IgnoreCopy]
        public IGradeSynonyms GradeSynonyms
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

        [IgnoreCopy]
        public bool? AutoplayAudio
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

        [IgnoreCopy]
        public bool? CaseSensitive
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

        [IgnoreCopy]
        public bool? ConfirmDemote
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

        [IgnoreCopy]
        public bool? EnableCommentary
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

        [IgnoreCopy]
        public bool? CorrectOnTheFly
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

        [IgnoreCopy]
        public bool? EnableTimer
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

        [IgnoreCopy]
        public bool? RandomPool
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

        [IgnoreCopy]
        public bool? SelfAssessment
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

        [IgnoreCopy]
        public bool? ShowImages
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

        [IgnoreCopy]
        public string StripChars
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

        [IgnoreCopy]
        public CultureInfo QuestionCulture
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

        [IgnoreCopy]
        public CultureInfo AnswerCulture
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public bool? ShowStatistics
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

        [IgnoreCopy]
        public bool? SkipCorrectAnswers
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

        [IgnoreCopy]
        public ISnoozeOptions SnoozeOptions
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

        [IgnoreCopy]
        public bool? PoolEmptyMessageShown
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

        [IgnoreCopy]
        public bool? UseLMStylesheets
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            SettingsHelper.Copy(this, target as ISettings, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
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

    class XmlCardSettings : ISettings
    {
        XmlCard card;

        internal XmlCardSettings(XmlCard card, ParentClass parent)
        {
            this.card = card;
            this.parent = parent;
        }

        #region ISettings Members

        [IgnoreCopy]
        public IQueryDirections QueryDirections
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

        [IgnoreCopy]
        public IQueryType QueryTypes
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

        [IgnoreCopy]
        public IQueryMultipleChoiceOptions MultipleChoiceOptions
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

        [IgnoreCopy]
        public IGradeTyping GradeTyping
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

        [IgnoreCopy]
        public IGradeSynonyms GradeSynonyms
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

        public ICardStyle Style
        {
            get
            {
                return card.Style;
            }
            set
            {
                card.Style = value;
            }
        }

        public CompiledTransform? QuestionStylesheet
        {
            get
            {
                return new CompiledTransform(card.QuestionStylesheet, null);
            }
            set
            {
                card.QuestionStylesheet = value.Value.Filename;
            }
        }

        public CompiledTransform? AnswerStylesheet
        {
            get
            {
                return new CompiledTransform(card.AnswerStylesheet, null);
            }
            set
            {
                card.AnswerStylesheet = value.Value.Filename;
            }
        }

        [IgnoreCopy]
        public bool? AutoplayAudio
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

        [IgnoreCopy]
        public bool? CaseSensitive
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

        [IgnoreCopy]
        public bool? ConfirmDemote
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

        [IgnoreCopy]
        public bool? EnableCommentary
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

        [IgnoreCopy]
        public bool? CorrectOnTheFly
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

        [IgnoreCopy]
        public bool? EnableTimer
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

        [IgnoreCopy]
        public bool? RandomPool
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

        [IgnoreCopy]
        public bool? SelfAssessment
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

        [IgnoreCopy]
        public bool? ShowImages
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

        [IgnoreCopy]
        public bool? PoolEmptyMessageShown
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

        [IgnoreCopy]
        public bool? UseLMStylesheets
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public string StripChars
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

        [IgnoreCopy]
        public CultureInfo QuestionCulture
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

        [IgnoreCopy]
        public CultureInfo AnswerCulture
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public bool? ShowStatistics
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

        [IgnoreCopy]
        public bool? SkipCorrectAnswers
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

        [IgnoreCopy]
        public ISnoozeOptions SnoozeOptions
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

        [IgnoreCopy]
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

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            SettingsHelper.Copy(this, target as ISettings, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
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

    class XmlChapterSettings : ISettings
    {
        XmlChapter chapter;

        internal XmlChapterSettings(XmlChapter chapter, ParentClass parent)
        {
            this.chapter = chapter;
            this.parent = parent;
        }

        #region ISettings Members

        [IgnoreCopy]
        public IQueryDirections QueryDirections
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

        [IgnoreCopy]
        public IQueryType QueryTypes
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

        [IgnoreCopy]
        public IQueryMultipleChoiceOptions MultipleChoiceOptions
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

        [IgnoreCopy]
        public IGradeTyping GradeTyping
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

        [IgnoreCopy]
        public IGradeSynonyms GradeSynonyms
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

        public ICardStyle Style
        {
            get
            {
                return chapter.Style;
            }
            set
            {
                chapter.Style = value;
            }
        }

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public bool? AutoplayAudio
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

        [IgnoreCopy]
        public bool? CaseSensitive
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

        [IgnoreCopy]
        public bool? ConfirmDemote
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

        [IgnoreCopy]
        public bool? EnableCommentary
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

        [IgnoreCopy]
        public bool? CorrectOnTheFly
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

        [IgnoreCopy]
        public bool? EnableTimer
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

        [IgnoreCopy]
        public bool? RandomPool
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

        [IgnoreCopy]
        public bool? SelfAssessment
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

        [IgnoreCopy]
        public bool? ShowImages
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

        [IgnoreCopy]
        public bool? PoolEmptyMessageShown
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

        [IgnoreCopy]
        public bool? UseLMStylesheets
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public string StripChars
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

        [IgnoreCopy]
        public CultureInfo QuestionCulture
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

        [IgnoreCopy]
        public CultureInfo AnswerCulture
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public bool? ShowStatistics
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

        [IgnoreCopy]
        public bool? SkipCorrectAnswers
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

        [IgnoreCopy]
        public ISnoozeOptions SnoozeOptions
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

        [IgnoreCopy]
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

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            SettingsHelper.Copy(this, target as ISettings, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
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

    class XmlPreviewCardSettings : ISettings
    {
        PreviewCard card;

        internal XmlPreviewCardSettings(PreviewCard card, ParentClass parent)
        {
            this.card = card;
            this.parent = parent;
        }

        #region ISettings Members

        [IgnoreCopy]
        public IQueryDirections QueryDirections
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

        [IgnoreCopy]
        public IQueryType QueryTypes
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

        [IgnoreCopy]
        public IQueryMultipleChoiceOptions MultipleChoiceOptions
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

        [IgnoreCopy]
        public IGradeTyping GradeTyping
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

        [IgnoreCopy]
        public IGradeSynonyms GradeSynonyms
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

        public ICardStyle Style
        {
            get
            {
                return card.Style;
            }
            set
            {
                card.Style = value;
            }
        }

        public CompiledTransform? QuestionStylesheet
        {
            get
            {
                return new CompiledTransform(card.QuestionStylesheet, null);
            }
            set
            {
                card.QuestionStylesheet = value.Value.Filename;
            }
        }

        public CompiledTransform? AnswerStylesheet
        {
            get
            {
                return new CompiledTransform(card.AnswerStylesheet, null);
            }
            set
            {
                card.AnswerStylesheet = value.Value.Filename;
            }
        }

        [IgnoreCopy]
        public bool? AutoplayAudio
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

        [IgnoreCopy]
        public bool? CaseSensitive
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

        [IgnoreCopy]
        public bool? ConfirmDemote
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

        [IgnoreCopy]
        public bool? EnableCommentary
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

        [IgnoreCopy]
        public bool? CorrectOnTheFly
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

        [IgnoreCopy]
        public bool? EnableTimer
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

        [IgnoreCopy]
        public bool? RandomPool
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

        [IgnoreCopy]
        public bool? SelfAssessment
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

        [IgnoreCopy]
        public bool? ShowImages
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

        [IgnoreCopy]
        public bool? PoolEmptyMessageShown
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

        [IgnoreCopy]
        public bool? UseLMStylesheets
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public string StripChars
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

        [IgnoreCopy]
        public CultureInfo QuestionCulture
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

        [IgnoreCopy]
        public CultureInfo AnswerCulture
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
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

        [IgnoreCopy]
        public bool? ShowStatistics
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

        [IgnoreCopy]
        public bool? SkipCorrectAnswers
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

        [IgnoreCopy]
        public ISnoozeOptions SnoozeOptions
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

        [IgnoreCopy]
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

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            SettingsHelper.Copy(this, target as ISettings, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
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
