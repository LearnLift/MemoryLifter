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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using System.Threading;
using MLifter.DAL.Preview;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Defines the available learning modes.
    /// </summary>
    public enum LearnModes
    {
        /// <summary>
        /// Image recognition mode.
        /// </summary>
        ImageRecognition,
        /// <summary>
        /// Listening comprehension mode.
        /// </summary>
        ListeningComprehension,
        /// <summary>
        /// Multiple choice mode.
        /// </summary>
        MultipleChoice,
        /// <summary>
        /// Sentence mode.
        /// </summary>
        Sentence,
        /// <summary>
        /// Wort (standard) mode.
        /// </summary>
        Word
    }

    /// <summary>
    /// Business layer dictionary / learning module.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-19</remarks>
    public partial class Dictionary : IDisposable
    {
        private bool isDisposed = false;
        private bool isActive = false;
        private Random m_Random;

        # region lists
        /// <summary>
        /// The cards of the dictionary.
        /// </summary>
        public CardDictionary Cards;
        /// <summary>
        /// The chapters of the dictionary.
        /// </summary>
        public ChapterDictionary Chapters;
        /// <summary>
        /// The statistics of the dictionary.
        /// </summary>
        public StatisticsDictionary Statistics;
        # endregion
        # region variables
        //used to filter for resources
        private const string XPathResourceFilter = "//questionaudio[Text() != '']/Text()|//questionexampleaudio[Text() != '']/Text()|//answeraudio[Text() != '']/Text()|//answerexampleaudio[Text() != '']/Text()|//questionvideo[Text() != '']/Text()|//answervideo[Text() != '']/Text()|//questionimage[Text() != '']/Text()|//answerimage[Text() != '']/Text()|//unusedmedia[Text() != '']/Text()|//questionstylesheet[Text() != '']/Text()|//answerstylesheet[Text() != '']/Text()";

        private LearnModes learnMode = LearnModes.Word;
        private EQueryDirection currentQueryDirection;
        private IDictionary dictionary;
        private Extensions extensions;
        private int currentCard = 0;
        private int currentBox = 0;
        private int learningBox = 0;
        private Dictionary<Side, XslCompiledTransform> xslTransformer = new Dictionary<Side, XslCompiledTransform>();
        public Dictionary<Side, string> CurrentlyLoadedStyleSheet = new Dictionary<Side, string>();

        private string questionStylesheet;
        private string answerStylesheet;

        private List<ICard> slideShowCards;


        # endregion

        #region events
        /// <summary>
        /// Occurs when [XML progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        public event StatusMessageEventHandler XmlProgressChanged;

        /// <summary>
        /// Occurs when [move progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        public event StatusMessageEventHandler MoveProgressChanged;

        /// <summary>
        /// Occurs when [save as progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        public event StatusMessageEventHandler SaveAsProgressChanged;

        /// <summary>
        /// Occurs when [create Media progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        public event StatusMessageEventHandler CreateMediaProgressChanged;
        #endregion

        # region properties

        /// <summary>
        /// Gets a value indicating whether this instance is a SQL DB.
        /// </summary>
        /// <value><c>true</c> if this instance is a SQL DB; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        public bool IsDB
        {
            get
            {
                return dictionary.IsDB;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a file DB.
        /// </summary>
        /// <value><c>true</c> if this instance is a file DB; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2009-04-26</remarks>
        public bool IsFileDB
        {
            get
            {
                switch (dictionary.Parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.MsSqlCe:
                    case DatabaseType.Xml:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is writeable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is writeable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-05-27</remarks>
        public bool IsWriteable
        {
            get
            {
                return User.ConnectionString.SyncType == SyncType.NotSynchronized;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }
        public List<IBox> Boxes
        {
            get { return dictionary.Boxes.Box; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [auto box size] mode is active.
        /// </summary>
        /// <value><c>true</c> if [auto box size]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-01-04</remarks>
        public bool AutoBoxSize
        {
            get { return Settings.AutoBoxSize.GetValueOrDefault(); }
            set { Settings.AutoBoxSize = value; }
        }
        /// <summary>
        /// Gets a value indicating whether this dictionary is emty.
        /// </summary>
        /// <value><c>true</c> if this dictionary is emty; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public bool IsEmpty { get { return dictionary.Cards.Count == 0; } }
        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <value>The XML.</value>
        /// <remarks>Documented by Dev02, 2007-11-12</remarks>
        public string Xml { get { return dictionary.Xml; } }

        public BackgroundWorker BackgroundWorker
        {
            get { return dictionary.BackgroundWorker; }
            set { dictionary.BackgroundWorker = value; }
        }

        private Settings settings;
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        public ISettings Settings { get { return settings; } }

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        /// <value>The default settings.</value>
        /// <remarks>Documented by Dev08, 2009-06-08</remarks>
        public ISettings DefaultSettings
        {
            get
            {
                //[ML-2145] Question/Answer caption are saved to the userProfiles instead to the DefaultSettings
                return dictionary.DefaultSettings;
            }
        }

        /// <summary>
        /// Gets the dictionary Media folder.
        /// </summary>
        /// <value>The dictionary Media folder.</value>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string MediaFolder
        {
            get
            {
                string dictionaryPath = DictionaryPath;
                string mediaDirectory = dictionary.MediaDirectory;

                return Path.Combine(Path.GetDirectoryName(dictionaryPath), mediaDirectory) + @"\";
            }
            set { dictionary.MediaDirectory = value; }
        }
        /// <summary>
        /// Gets or sets the learn mode.
        /// </summary>
        /// <value>The learn mode.</value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public LearnModes LearnMode
        {
            get { return learnMode; }
            set { learnMode = value; }
        }
        /// <summary>
        /// Gets or sets the current query direction.
        /// </summary>
        /// <value>The current query direction.</value>
        /// <remarks>Documented by Dev03, 2007-10-03</remarks>
        public EQueryDirection CurrentQueryDirection
        {
            get { return currentQueryDirection; }
            set { currentQueryDirection = value; }
        }

        /// <summary>
        /// Gets or sets the current multiple choice options.
        /// </summary>
        /// <value>The current multiple choice options.</value>
        /// <remarks>Documented by Dev08, 2009-04-10</remarks>
        public IQueryMultipleChoiceOptions CurrentMultipleChoiceOptions { get; set; }

        /// <summary>
        /// Gets the allowed query directions.
        /// </summary>
        /// <value>The allowed query directions.</value>
        /// <remarks>Documented by Dev03, 2008-01-09</remarks>
        public IQueryDirections AllowedQueryDirections
        {
            get { return dictionary.AllowedSettings.QueryDirections; }
            set { dictionary.AllowedSettings.QueryDirections = value; }
        }

        /// <summary>
        /// Gets the allowed query types.
        /// </summary>
        /// <value>The allowed query types.</value>
        /// <remarks>Documented by Dev03, 2008-01-09</remarks>
        public IQueryType AllowedQueryTypes
        {
            get { return dictionary.AllowedSettings.QueryTypes; }
            set { dictionary.AllowedSettings.QueryTypes = value; }
        }

        /// <summary>
        /// Gets the multiple choice options.
        /// </summary>
        /// <value>The multiple choice options.</value>
        /// <remarks>Documented by Dev03, 2008-01-09</remarks>
        public IQueryMultipleChoiceOptions MultipleChoiceOptions
        {
            get { return Settings.MultipleChoiceOptions; }
            set { Settings.MultipleChoiceOptions = value; }
        }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public double Score
        {
            get { return dictionary.Score; }
        }
        /// <summary>
        /// Gets or sets the question caption.
        /// </summary>
        /// <value>The question caption.</value>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string QuestionCaption
        {
            get { return Settings.QuestionCaption; }
            set { Settings.QuestionCaption = value; }
        }
        /// <summary>
        /// Gets or sets the answer caption.
        /// </summary>
        /// <value>The answer caption.</value>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string AnswerCaption
        {
            get { return Settings.AnswerCaption; }
            set { Settings.AnswerCaption = value; }
        }
        /// <summary>
        /// Gets or sets the question culture.
        /// </summary>
        /// <value>The question culture.</value>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public CultureInfo QuestionCulture
        {
            get { return Settings.QuestionCulture; }
            set { Settings.QuestionCulture = value; }
        }
        /// <summary>
        /// Gets or sets the answer culture.
        /// </summary>
        /// <value>The answer culture.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public CultureInfo AnswerCulture
        {
            get { return Settings.AnswerCulture; }
            set { Settings.AnswerCulture = value; }
        }
        /// <summary>
        /// Gets or sets the query chapters.
        /// </summary>
        /// <value>The query chapters.</value>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public IList<int> QueryChapters
        {
            get { return Settings.SelectedLearnChapters; }
        }
        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        /// <remarks>Documented by Dev05, 2007-09-06</remarks>
        /// <remarks>Documented by Dev02, 2007-11-12</remarks>
        public Category Category
        {
            get { return dictionary.Category; }
            set { dictionary.Category = value; }
        }
        /// <summary>
        /// Gets the available categories.
        /// </summary>
        /// <value>The categories.</value>
        /// <remarks>Documented by Dev02, 2008-02-15</remarks>
        public static List<Category> Categories
        {
            get
            {
                return Category.GetCategories();
            }
        }
        /// <summary>
        /// Gets the default category.
        /// </summary>
        /// <value>The default category.</value>
        /// <remarks>Documented by Dev02, 2008-02-15</remarks>
        public static Category DefaultCategory
        {
            get { return Category.GetDefaultCategory(); }
        }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        /// <remarks>Documented by Dev05, 2007-09-06</remarks>
        public string Author
        {
            get { return dictionary.Author; }
            set { dictionary.Author = value; }
        }
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The Description.</value>
        /// <remarks>Documented by Dev05, 2007-09-06</remarks>
        public string Description
        {
            get { return dictionary.Description; }
            set { dictionary.Description = value; }
        }
        /// <summary>
        /// Gets the dictionarys directory path.
        /// </summary>
        /// <value>The dictionary directory path.</value>
        /// <remarks>Documented by Dev03, 2008-09-01</remarks>
        public string DirectoryName
        {
            get
            {
                if (IsDB)
                {
                    return Path.GetTempPath();
                }
                else if (Path.GetDirectoryName(dictionary.Connection) != null)
                {
                    return Path.GetDirectoryName(dictionary.Connection);
                }
                else
                {
                    return Path.GetPathRoot(dictionary.Connection);
                }
            }
        }
        /// <summary>
        /// Gets the style path.
        /// </summary>
        /// <value>The style path.</value>
        /// <remarks>Documented by Dev03, 2008-09-01</remarks>
        private string StylePath
        {
            get
            {
                if (Path.GetDirectoryName(AnswerStyleSheet) != null)
                {
                    return Path.GetDirectoryName(AnswerStyleSheet);
                }
                else
                {
                    return Path.GetPathRoot(AnswerStyleSheet);
                }
            }
        }
        /// <summary>
        /// Gets the dictionary path.
        /// </summary>
        /// <value>The dictionary path.</value>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string DictionaryPath { get { return dictionary.Connection; } }
        /// <summary>
        /// Gets the dictionary title.
        /// </summary>
        /// <value>The dictionary title.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        public string DictionaryTitle { get { return dictionary.Title; } }
        /// <summary>
        /// Gets the dictionary display title.
        /// </summary>
        /// <value>The dictionary display title.</value>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        public string DictionaryDisplayTitle
        {
            get
            {
                if (dictionary.IsDB)
                    return dictionary.Title;
                else
                    return Path.GetFileName(dictionary.Connection);
            }
        }
        /// <summary>
        /// Gets the DAL dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev02, 2008-01-07</remarks>
        public IDictionary DictionaryDAL { get { return dictionary; } }
        /// <summary>
        /// Gets a value indicating whether to use the dictionary style sheets.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if use the dictionary style sheets; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public bool UseDictionaryStyleSheets
        {
            get { return Settings.UseLMStylesheets.GetValueOrDefault(); }
            set { Settings.UseLMStylesheets = value; }
        }
        /// <summary>
        /// Gets the commentary sound.
        /// </summary>
        /// <value>The commentary sound.</value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySound
        {
            get { return Settings.CommentarySounds; }
            set { Settings.CommentarySounds = value; }
        }
        /// <summary>
        /// Gets or sets the current box. (The box from which the current/last card is from.)
        /// </summary>
        /// <value>The current box.</value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public int CurrentBox
        {
            get { return currentBox; }
            set { currentBox = value; }
        }
        /// <summary>
        /// Gets the ID of the last delivered card.
        /// </summary>
        /// <value>ID of the last delivered card.</value>
        /// <remarks>Documented by Dev02, 2007-11-12</remarks>
        public int CurrentCard
        {
            get { return currentCard; }
        }
        /// <summary>
        /// Gets or sets the learning box. (The box from which cards are taken.)
        /// </summary>
        /// <value>The learning box.</value>
        /// <remarks>Documented by Dev05, 2007-10-09</remarks>
        public int LearningBox
        {
            get { return learningBox; }
            set { learningBox = value; }
        }
        /// <summary>
        /// Gets the highscore.
        /// </summary>
        /// <value>The highscore.</value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public double Highscore
        {
            get { return dictionary.HighScore; }
            set { dictionary.HighScore = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the pool emty message is already shown.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the pool emty message was shown; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public bool PoolEmptyMessageShown
        {
            get { return Settings.PoolEmptyMessageShown.Value; }
            set { Settings.PoolEmptyMessageShown = value; }
        }
        /// <summary>
        /// Gets the deaktivated cards count.
        /// </summary>
        /// <value>The deaktivated cards count.</value>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public int DeaktivatedCardsCount
        {
            get
            {
                return dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(QueryCardState.Inactive) }, QueryOrder.None, QueryOrderDir.Ascending, 0).Count;
            }
        }
        /// <summary>
        /// Gets or sets the question style sheet.
        /// </summary>
        /// <value>The question style sheet.</value>
        /// <remarks>Documented by Dev05, 2007-09-06</remarks>
        public string QuestionStyleSheet
        {
            get { return questionStylesheet; }
            set { questionStylesheet = value; }
        }
        /// <summary>
        /// Gets or sets the answer style sheet.
        /// </summary>
        /// <value>The answer style sheet.</value>
        /// <remarks>Documented by Dev05, 2007-09-06</remarks>
        public string AnswerStyleSheet
        {
            get { return answerStylesheet; }
            set { answerStylesheet = value; }
        }
        /// <summary>
        /// Gets or sets the card style.
        /// </summary>
        /// <value>The card style.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        public DAL.Interfaces.ICardStyle CardStyle
        {
            get { return Settings.Style; }
            set { Settings.Style = value; }
        }
        /// <summary>
        /// Gets the logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        public Stream Logo
        {
            get
            {
                IMedia logo = Settings.Logo as IImage;
                if (logo != null)
                    return logo.Stream;

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [confirmation required for demote].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [confirmation required for demote]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2008-02-04</remarks>
        public bool ConfirmationRequiredForDemote
        {
            get
            {
                return (Settings.ConfirmDemote.Value && (LearningBox == 0));
            }
        }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>The extensions.</value>
        /// <remarks>Documented by Dev02, 2009-07-03</remarks>
        public Extensions Extensions
        {
            get
            {
                return extensions;
            }
        }
        # endregion

        # region constructor and file access methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="answerStyleSheet">The answer style sheet.</param>
        /// <param name="questionStyleSheet">The question style sheet.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public Dictionary(IDictionary dictionary, LearnLogic learnLogic)
        {
            CurrentLearnLogic = learnLogic;
            LoadDictionary(dictionary);
            m_Random = new Random((int)DateTime.Now.Ticks);
        }

        private void LoadDictionary(IDictionary dictionary)
        {
            DetachEvents();
            this.dictionary = dictionary;
            this.extensions = new Extensions(dictionary);
            Open();

            if (!(dictionary is MLifter.DAL.Preview.PreviewDictionary))
                PrepareDictionary();

            if (slideShowCards == null)
                LoadSlideShowContent();
        }

        /// <summary>
        /// Starts the user session.
        /// Against: [ML-1378] - Import of ODX causes two LearningSessions
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        public void StartUserSession()
        {
            Log.OpenUserSession(dictionary.Id, dictionary.Parent);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="XmlDictionary"/> is reclaimed by garbage collection.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-02-05</remarks>
        ~Dictionary()
        {
            Dispose(false);
        }

        /// <summary>
        /// Loads the content of the slide show.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-10</remarks>
        public void LoadSlideShowContent()
        {
            List<QueryStruct> queryStructs = new List<QueryStruct>();
            if (QueryChapters.Count == Chapters.Chapters.Count)
                queryStructs.Add(new QueryStruct(-1, -1, QueryCardState.Active));
            else
                foreach (int chapter in QueryChapters)
                    queryStructs.Add(new QueryStruct(chapter, -1, QueryCardState.Active));

            slideShowCards =
             dictionary.Cards.GetCards(queryStructs.ToArray(), QueryOrder.Timestamp, QueryOrderDir.Ascending, 0);
        }
        private ICard getNextCard()
        {
            List<QueryStruct> queryStructs = new List<QueryStruct>();
            if (QueryChapters.Count == Chapters.Chapters.Count)
                queryStructs.Add(new QueryStruct(-1, -1, QueryCardState.Active));
            else
                foreach (int chapter in QueryChapters)
                    queryStructs.Add(new QueryStruct(chapter, -1, QueryCardState.Active));

            List<ICard> result = dictionary.Cards.GetCards(queryStructs.ToArray(), QueryOrder.Timestamp, QueryOrderDir.Ascending, 1);
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Opens the dictionary.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        private void Open()
        {
            AttachEvents();
            isActive = true;
        }

        /// <summary>
        /// Copies the contents of a dictionary to another one.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <remarks>Documented by Dev02, 2008-09-24</remarks>
        public void CopyTo(Dictionary dictionary, CopyToProgress progressDelegate)
        {
            this.DictionaryDAL.CopyTo(dictionary.DictionaryDAL, progressDelegate);
        }

        /// <summary>
        /// Begins the copy to.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <param name="sourceUser">The source user.</param>
        /// <param name="targetUser">The target user.</param>
        /// <param name="resetAfterCopy">if set to <c>true</c> to reset after copy.</param>
        /// <remarks>Documented by Dev08, 2008-09-26</remarks>
        public void BeginCopyTo(Dictionary dictionary, CopyToProgress progressDelegate, User sourceUser, User targetUser, bool resetAfterCopy)
        {
            Thread copyToThread = new Thread(delegate()
            {
                bool success = false;
                Exception exp = null;
                try
                {
                    CopyTo(dictionary, progressDelegate);

                    if (resetAfterCopy)
                        dictionary.DictionaryDAL.ResetLearningProgress();

                    success = true;
                }
                catch (Exception e)
                {
                    exp = e;
                    Trace.WriteLine("CopyTo(dictionary) failed: " + e.Message);
                }
                finally
                {
                    OnCopyToFinished(new CopyToEventArgs(this, dictionary, sourceUser, targetUser, success, exp));
                }
            });
            copyToThread.Name = "Dictionary CopyTo Thread";
            copyToThread.IsBackground = true;
            copyToThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            copyToThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            copyToThread.Start();
        }

        /// <summary>
        /// Occurs when [copy to finished].
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-09-26</remarks>
        public event CopyToFinishedEventHandler CopyToFinished;

        public delegate void CopyToFinishedEventHandler(object sender, CopyToEventArgs e);

        /// <summary>
        /// Raises the <see cref="E:CopyToFinished"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2008-09-26</remarks>
        private void OnCopyToFinished(CopyToEventArgs e)
        {
            if (CopyToFinished != null)
                CopyToFinished(this, e);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-28</remarks>
        public void Close()
        {
            DetachEvents();
            if (dictionary != null) dictionary.Dispose();
            dictionary = null;
            isActive = false;
        }

        /// <summary>
        /// Saves this dictionary.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public bool Save()
        {
            return Save(DictionaryPath);
        }

        /// <summary>
        /// Saves the dictionary to the specified dictionary path.
        /// </summary>
        /// <param name="dictionaryPath">The dictionary path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public bool Save(string dictionaryPath)
        {
            try
            {
                if (DictionaryPath != dictionaryPath)
                {
                    dictionary.SaveAs(DictionaryPath);
                }

                dictionary.Save();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Prepares the dictionary.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        private void PrepareDictionary()
        {
            settings = new Settings(dictionary);
            Cards = new CardDictionary(this, dictionary.Cards);
            Chapters = new ChapterDictionary(this, dictionary.Chapters);
            Statistics = new StatisticsDictionary(this, dictionary.Statistics);
        }

        private void AttachEvents()
        {
            if (dictionary != null)
            {
                dictionary.CreateMediaProgressChanged += new StatusMessageEventHandler(Dictionary_CreateMediaProgressChanged);
                dictionary.MoveProgressChanged += new StatusMessageEventHandler(Dictionary_MoveProgressChanged);
                dictionary.SaveAsProgressChanged += new StatusMessageEventHandler(Dictionary_SaveAsProgressChanged);
                dictionary.XmlProgressChanged += new StatusMessageEventHandler(Dictionary_XmlProgressChanged);
            }
        }

        void Dictionary_XmlProgressChanged(object sender, StatusMessageEventArgs args)
        {
            if (XmlProgressChanged != null) XmlProgressChanged(this, args);
        }

        void Dictionary_SaveAsProgressChanged(object sender, StatusMessageEventArgs args)
        {
            if (SaveAsProgressChanged != null) SaveAsProgressChanged(this, args);
        }

        void Dictionary_MoveProgressChanged(object sender, StatusMessageEventArgs args)
        {
            if (MoveProgressChanged != null) MoveProgressChanged(this, args);
        }

        void Dictionary_CreateMediaProgressChanged(object sender, StatusMessageEventArgs args)
        {
            if (CreateMediaProgressChanged != null) CreateMediaProgressChanged(this, args);
        }

        private void DetachEvents()
        {
            if (dictionary != null)
            {
                dictionary.CreateMediaProgressChanged -= new StatusMessageEventHandler(Dictionary_CreateMediaProgressChanged);
                dictionary.MoveProgressChanged -= new StatusMessageEventHandler(Dictionary_MoveProgressChanged);
                dictionary.SaveAsProgressChanged -= new StatusMessageEventHandler(Dictionary_SaveAsProgressChanged);
                dictionary.XmlProgressChanged -= new StatusMessageEventHandler(Dictionary_XmlProgressChanged);
            }
        }

        /// <summary>
        /// Calculates the auto box sizes.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-04</remarks>
        private void CalculateAutoBoxSizes()
        {
            if (this.AutoBoxSize)
            {
                int boxcount = this.Boxes.Count;
                //int minboxsize = Convert.ToInt32(Math.Pow(2, Convert.ToString(cardcount).Length));
                //int cardcount = this.Cards.Count;
                int cardcount = 0;
                foreach (IBox box in this.Boxes) cardcount += box.Size;
                int minboxsize = this.Boxes[1].MaximalSize = this.Boxes[1].DefaultSize;

                //set maximal size of boxes according to autoboxsize algorithm
                //without pool and the highest box
                for (int i = 2; i < boxcount - 1; i++)
                {
                    IBox box = this.Boxes[i];
                    box.MaximalSize = Convert.ToInt32(1.0 / Math.Pow(boxcount - i, 1.5) * (cardcount - minboxsize) + minboxsize);
                }
            }
        }


        # endregion
        # region pro- and demote
        /// <summary>
        /// Promotes the cardNode.
        /// </summary>
        /// <param name="cardID">The cardNode ID.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void PromoteCard(int cardID)
        {
            if (cardID < 0)
                return;
            ICard card = dictionary.Cards.Get(cardID);
            if (card == null)
                return;
            if (!card.Active)
                return;
            if (card.Box < Boxes.Count - 1)
            {
                if (card.Box == 0)
                    card.Box = 2;
                else
                    card.Box++;
            }

            CardUsed(cardID);
        }
        /// <summary>
        /// Sets the current time to the cards last use timestamp.
        /// </summary>
        /// <param name="cardID">The cardNode ID.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void CardUsed(int cardID)
        {
            ICard card = dictionary.Cards.Get(cardID);
            if (card == null)
                return;
            card.Timestamp = DateTime.Now;
        }
        /// <summary>
        /// Demotes the cardNode.
        /// </summary>
        /// <param name="cardID">The cardNode ID.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void DemoteCard(int cardID)
        {
            ICard card = dictionary.Cards.Get(cardID);
            if (card == null)
                return;
            if (!card.Active)
                return;
            card.Box = 1;
            CardUsed(cardID);
        }
        # endregion
        # region get card(s)
        /// <summary>
        /// Gets the next cardNode.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public ICard GetNextCard()
        {
            ICard nextCard = null;

            if (LearningBox == 0)
            {
                for (int i = Boxes.Count - 1; i > 0; i--)
                {
                    if (Boxes[i].Size >= Boxes[i].MaximalSize)
                    {
                        nextCard = GetNextCard(i);
                        if (nextCard != null)
                        {
                            currentCard = nextCard.Id;
                            return nextCard;
                        }
                    }
                }

                nextCard = GetNextCard(0);
                if (nextCard != null)
                {
                    currentCard = nextCard.Id;
                    return nextCard;
                }

                OnPoolEmpty(EventArgs.Empty);

                nextCard = GetNextCard(-1);
                if (nextCard != null)
                {
                    currentCard = nextCard.Id;
                    return nextCard;
                }
            }
            else
            {
                nextCard = GetNextCard(LearningBox);
                if (nextCard != null)
                {
                    currentCard = nextCard.Id;
                    return nextCard;
                }
            }

            return null;
        }
        /// <summary>
        /// Raises the <see cref="E:PoolEmpty"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        private void OnPoolEmpty(EventArgs e)
        {
            if (PoolEmpty != null)
                PoolEmpty(this, e);
        }
        /// <summary>
        /// Occurs when [pool empty].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public event EventHandler PoolEmpty;
        /// <summary>
        /// Gets the next cardNode.
        /// </summary>
        /// <param name="BoxNr">The box nr.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        private ICard GetNextCard(int BoxNr)
        {
            learnMode = GetNextLearnMode();
            currentQueryDirection = GetNextQueryDirection();
            List<QueryStruct> queryStructs = new List<QueryStruct>();

            //add user.Dictionary.MultipleChoice to user.Dictionary.CurrentMultipleChoice
            CurrentMultipleChoiceOptions = new QueryMultipleChoiceOptions();
            CurrentMultipleChoiceOptions.AllowMultipleCorrectAnswers = MultipleChoiceOptions.AllowMultipleCorrectAnswers;
            CurrentMultipleChoiceOptions.AllowRandomDistractors = MultipleChoiceOptions.AllowRandomDistractors;
            CurrentMultipleChoiceOptions.MaxNumberOfCorrectAnswers = MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
            CurrentMultipleChoiceOptions.NumberOfChoices = MultipleChoiceOptions.NumberOfChoices;

            if (QueryChapters.Count == Chapters.Chapters.Count)
                queryStructs.Add(new QueryStruct(-1, BoxNr, QueryCardState.Active));
            else
            {
                foreach (int chapter in QueryChapters)
                    queryStructs.Add(new QueryStruct(chapter, BoxNr, QueryCardState.Active));
            }

            List<ICard> cards;
            if ((BoxNr == 0) && Settings.RandomPool.GetValueOrDefault())
            {
                cards = dictionary.Cards.GetCards(queryStructs.ToArray(), QueryOrder.Random, QueryOrderDir.Ascending, 1);
            }
            else
                cards = dictionary.Cards.GetCards(queryStructs.ToArray(), QueryOrder.Timestamp, QueryOrderDir.Ascending, 1);

            if (cards.Count > 0)
            {
                CurrentBox = cards[0].Box;
                return cards[0];
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the previous slide card id.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev07, 2009-04-10</remarks>
        public ICard GetPreviousSlide()
        {
            int indexOfCurrentCard;
            ICard current = (Cards.GetCardByID(CurrentLearnLogic.CurrentCardID).BaseCard as ICard);
            indexOfCurrentCard = slideShowCards.IndexOf(current);
            if (indexOfCurrentCard < 0)
                return null;
            indexOfCurrentCard--;
            if (indexOfCurrentCard < 0)
            {
                CurrentBox = slideShowCards[slideShowCards.Count - 1].Box;
                return slideShowCards[slideShowCards.Count - 1];
            }
            CurrentBox = slideShowCards[indexOfCurrentCard].Box;

            return slideShowCards[indexOfCurrentCard];
        }
        /// <summary>
        /// Gets the next slide show card id.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        public ICard GetNextSlide()
        {
            int indexOfCurrentCard;
            ICard current = (Cards.GetCardByID(CurrentLearnLogic.CurrentCardID).BaseCard as ICard);
            indexOfCurrentCard = slideShowCards.IndexOf(current);
            if (indexOfCurrentCard == -1)
            {
                //check if the next card would be in there
                current = getNextCard();
                if (current == null)
                    return null;

                indexOfCurrentCard = slideShowCards.IndexOf(current);

                if (indexOfCurrentCard == -1)
                    return null;

                CurrentBox = slideShowCards[indexOfCurrentCard].Box;
                return slideShowCards[indexOfCurrentCard];
            }
            indexOfCurrentCard++;
            if (indexOfCurrentCard > (slideShowCards.Count - 1))
            {
                CurrentBox = slideShowCards[0].Box;
                return slideShowCards[0];
            }
            CurrentBox = slideShowCards[indexOfCurrentCard].Box;
            return slideShowCards[indexOfCurrentCard];
        }

        /// <summary>
        /// Get random answers from the same chapter as the given cardNode - for use in multiple choise mode.
        /// </summary>
        /// <param name="cardID">The cardNode ID.</param>
        /// <param name="answersCount">The answers count.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        [Obsolete("Use GetChoices instead.")]
        public string[] GetRandomAnswers(int cardID, int answersCount)
        {
            List<string> answers = new List<string>();

            List<ICard> cards = dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(dictionary.Cards.Get(cardID).Chapter, -1) }, QueryOrder.Random, QueryOrderDir.Ascending, 0);

            for (int i = 0; i < cards.Count; i++)
            {
                if (CurrentQueryDirection == EQueryDirection.Question2Answer)
                {
                    if ((cards[i].Id != cardID) && (cards[i].Answer.Words.Count > 0) && (cards[i].Answer.Words[0].Word.Length > 0))
                        answers.Add(cards[i].Answer.Words[0].Word);
                }
                else
                {
                    if ((cards[i].Id != cardID) && (cards[i].Question.Words.Count > 0) && (cards[i].Question.Words[0].Word.Length > 0))
                        answers.Add(cards[i].Question.Words[0].Word);
                }
            }

            if (answers.Count < answersCount)
                return null;

            return answers.ToArray();
        }

        /// <summary>
        /// Gets the choices for the multiple choice mode.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-11</remarks>
        public MultipleChoice GetChoices(ICard mcCard)
        {
            MultipleChoice choices = new MultipleChoice();
            MultipleChoice distractors = new MultipleChoice();
            MultipleChoice emptyChoices = new MultipleChoice();

            // load the correct choices and the distractors
            if (CurrentQueryDirection == EQueryDirection.Question2Answer)
            {
                for (int i = 0; i < mcCard.Answer.Words.Count; i++)
                {
                    if (CurrentMultipleChoiceOptions.MaxNumberOfCorrectAnswers <= i) break;
                    if (mcCard.Answer.Words[i].Word.Length > 0)
                        choices.Add(new Choice(mcCard.Answer.Words[i].Word, true));
                    if (!CurrentMultipleChoiceOptions.AllowMultipleCorrectAnswers.Value) break;
                }
                for (int i = 0; i < mcCard.AnswerDistractors.Words.Count; i++)
                {
                    if (mcCard.AnswerDistractors.Words[i].Word.Length > 0)
                        distractors.Add(new Choice(mcCard.AnswerDistractors.Words[i].Word, false));
                }
            }
            else
            {
                for (int i = 0; i < mcCard.Question.Words.Count; i++)
                {
                    if (CurrentMultipleChoiceOptions.MaxNumberOfCorrectAnswers <= i) break;
                    if (mcCard.Question.Words[i].Word.Length > 0)
                        choices.Add(new Choice(mcCard.Question.Words[i].Word, true));
                    if (!CurrentMultipleChoiceOptions.AllowMultipleCorrectAnswers.Value) break;
                }
                for (int i = 0; i < mcCard.QuestionDistractors.Words.Count; i++)
                {
                    if (mcCard.QuestionDistractors.Words[i].Word.Length > 0)
                        distractors.Add(new Choice(mcCard.QuestionDistractors.Words[i].Word, false));
                }
            }

            //load random distractors (if allowed)
            if (CurrentMultipleChoiceOptions.AllowRandomDistractors.Value)
            {
                int numberOfRandomDistractors = CurrentMultipleChoiceOptions.NumberOfChoices.GetValueOrDefault() - choices.Count;

                //enough distractors available... do not need to search for random distractors
                if (numberOfRandomDistractors <= distractors.Count)
                {
                    if (distractors.Count > numberOfRandomDistractors)
                        distractors.Randomize();
                    choices.AddRange(distractors.GetRange(0, numberOfRandomDistractors));
                }
                else        //to less distractors available... search for random distractors
                {
                    List<ICard> cards = dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(mcCard.Chapter, -1) }, QueryOrder.Random, QueryOrderDir.Ascending, numberOfRandomDistractors * 5 + choices.Count);
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (numberOfRandomDistractors <= distractors.Count) break;
                        if (cards[i].Id == mcCard.Id) continue;

                        IList<IWord> words;
                        if (CurrentQueryDirection == EQueryDirection.Question2Answer)
                        {
                            words = cards[i].Answer.Words;
                        }
                        else
                        {
                            words = cards[i].Question.Words;
                        }
                        if ((words.Count > 0) && (words[0].Word.Length > 0)
                            && !choices.Exists(c => c.Word.Equals(words[0].Word))
                            && !distractors.Exists(d => d.Word.Equals(words[0].Word)))
                            distractors.Add(new Choice(words[0].Word, false));
                    }
                    choices.AddRange(distractors);
                }

                if (choices.Count < CurrentMultipleChoiceOptions.NumberOfChoices)
                    return emptyChoices;
            }
            else
            {
                if (distractors.Count == 0)
                    return emptyChoices;
                int numberOfRandomDistractors = CurrentMultipleChoiceOptions.NumberOfChoices.GetValueOrDefault() - choices.Count;
                //shuffle
                if (distractors.Count > numberOfRandomDistractors)
                    distractors.Randomize();
                if (numberOfRandomDistractors <= distractors.Count)
                    choices.AddRange(distractors.GetRange(0, numberOfRandomDistractors));
                else
                    choices.AddRange(distractors);
            }

            choices.Randomize();
            return choices;
        }

        /// <summary>
        /// Checks if there are enough choices available.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        public bool ChoicesAvailable(ICard card, int count)
        {
            return (GetChoices(card).Count >= count); //ToDo: chache this for later use or don't build the whole thing only for counting choices
        }

        /// <summary>
        /// Gets the next learn mode.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        private LearnModes GetNextLearnMode()
        {
            List<LearnModes> learnModes = GetAllLearnModes();

            if (learnModes.Count == 0)
            {
                Settings.QueryTypes.Word = true;
                return LearnModes.Word;
            }

            Random rand = new Random((int)DateTime.Now.Ticks);
            int index = (int)Math.Round(rand.Next(0, (learnModes.Count - 1) * 1000) / 1000.0);
            return learnModes[index];
        }

        /// <summary>
        /// Get all activated LearnModes (NOT the allowed LearnModes!)
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-20</remarks>
        internal List<LearnModes> GetAllLearnModes()
        {
            List<LearnModes> learnModes = new List<LearnModes>();

            if (Settings.QueryTypes.ImageRecognition.Value && dictionary.AllowedSettings.QueryTypes.ImageRecognition.Value)
                learnModes.Add(LearnModes.ImageRecognition);
            if (Settings.QueryTypes.ListeningComprehension.Value && dictionary.AllowedSettings.QueryTypes.ListeningComprehension.Value)
                learnModes.Add(LearnModes.ListeningComprehension);
            if (Settings.QueryTypes.MultipleChoice.Value && dictionary.AllowedSettings.QueryTypes.MultipleChoice.Value)
                learnModes.Add(LearnModes.MultipleChoice);
            if (Settings.QueryTypes.Sentence.Value && dictionary.AllowedSettings.QueryTypes.Sentence.Value)
                learnModes.Add(LearnModes.Sentence);
            if (Settings.QueryTypes.Word.Value && dictionary.AllowedSettings.QueryTypes.Word.Value)
                learnModes.Add(LearnModes.Word);

            return learnModes;
        }
        /// <summary>
        /// Gets the next query direction.
        /// </summary>
        /// <returns>Query direction.</returns>
        /// <remarks>Documented by Dev03, 2007-10-03</remarks>
        private EQueryDirection GetNextQueryDirection()
        {
            if (Settings.QueryDirections.Mixed.Value)
            {
                if (dictionary.AllowedSettings.QueryDirections.Mixed.Value)
                {
                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int rnd = (int)Math.Round(rand.Next(0, 1000) / 1000.0);
                    if (rnd == 0)
                        return EQueryDirection.Answer2Question;
                    else
                        return EQueryDirection.Question2Answer;
                }
                else if (dictionary.AllowedSettings.QueryDirections.Question2Answer.Value)
                    return EQueryDirection.Question2Answer;
                else if (dictionary.AllowedSettings.QueryDirections.Answer2Question.Value)
                    return EQueryDirection.Answer2Question;
                else
                    return EQueryDirection.Question2Answer; //this is always the default
            }
            else
            {
                if ((dictionary.AllowedSettings.QueryDirections.Question2Answer.Value) && (dictionary.AllowedSettings.QueryDirections.Answer2Question.Value))
                    return Settings.QueryDirections.Question2Answer.Value ? EQueryDirection.Question2Answer : EQueryDirection.Answer2Question;
                else if (dictionary.AllowedSettings.QueryDirections.Question2Answer.Value)
                    return EQueryDirection.Question2Answer;
                else if (dictionary.AllowedSettings.QueryDirections.Answer2Question.Value)
                    return EQueryDirection.Answer2Question;
                else
                    return EQueryDirection.Question2Answer; //this is always the default
            }
        }

        /// <summary>
        /// Preloads the card cache.
        /// </summary>
        /// <remarks>Documented by Dev09, 2009-04-29</remarks>
        public void PreloadCardCache()
        {
            dictionary.PreloadCardCache();
        }
        # endregion
        # region generate html
        /// <summary>
        /// Generates the Html file for the question and saves it to the dictionary dir.
        /// Depends on GenerateCard to render the Cards.
        /// </summary>
        /// <param name="cardID">Card ID</param>
        /// <returns>Path to the question file</returns>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        public string GenerateQuestion(int cardID)
        {
            try
            {
                return GenerateCard(cardID, Side.Question, string.Empty, true);
            }
            catch (XsltCompileException exp)
            {
                Trace.WriteLine("Generate Question Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Question) && CurrentlyLoadedStyleSheet[Side.Question] != null
                        ? CurrentlyLoadedStyleSheet[Side.Question] : String.Empty));
            }
            catch (XsltException exp)
            {
                Trace.WriteLine("Generate Question Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Question) && CurrentlyLoadedStyleSheet[Side.Question] != null
                        ? CurrentlyLoadedStyleSheet[Side.Question] : String.Empty));
            }
            catch (ThreadAbortException exp)
            {
                Trace.WriteLine("Generate Question Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Question) && CurrentlyLoadedStyleSheet[Side.Question] != null
                        ? CurrentlyLoadedStyleSheet[Side.Question] : String.Empty));
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Generate Question Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Question) && CurrentlyLoadedStyleSheet[Side.Question] != null
                        ? CurrentlyLoadedStyleSheet[Side.Question] : String.Empty));
            }
        }
        /// <summary>
        /// Generates the Html file for the answer and saves it to the dictionary dir.
        /// Depends on GenerateCard to render the Cards.
        /// </summary>
        /// <param name="cardID">Card ID</param>
        /// <param name="answer">The answer.</param>
        /// <param name="correct">if set to <c>true</c> [correct].</param>
        /// <returns>Path to the question file</returns>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        public string GenerateAnswer(int cardID, string answer, bool correct)
        {
            try
            {
                return GenerateCard(cardID, Side.Answer, answer, correct);
            }
            catch (XsltCompileException exp)
            {
                Trace.WriteLine("Generate Answer Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Answer) && CurrentlyLoadedStyleSheet[Side.Answer] != null
                        ? CurrentlyLoadedStyleSheet[Side.Answer] : String.Empty));
            }
            catch (XsltException exp)
            {
                Trace.WriteLine("Generate Answer Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Answer) && CurrentlyLoadedStyleSheet[Side.Answer] != null
                        ? CurrentlyLoadedStyleSheet[Side.Answer] : String.Empty));
            }
            catch (ThreadAbortException exp)
            {
                Trace.WriteLine("Generate Question Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Question) && CurrentlyLoadedStyleSheet[Side.Question] != null
                        ? CurrentlyLoadedStyleSheet[Side.Question] : String.Empty));
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Generate Answer Exception: " + exp.ToString());
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT,
                    CurrentlyLoadedStyleSheet.ContainsKey(Side.Answer) && CurrentlyLoadedStyleSheet[Side.Answer] != null
                        ? CurrentlyLoadedStyleSheet[Side.Answer] : String.Empty));
            }
        }

        /// <summary>
        /// Gets or sets the current learn logic.
        /// </summary>
        /// <value>The current learn logic.</value>
        /// <remarks>Documented by Dev09, 2009-04-10</remarks>
        public LearnLogic CurrentLearnLogic { get; private set; }

        /// <summary>
        /// Reads the cards XML fragment from the dictionary, extends it with the settings and transforms it to Html using XSLT.
        /// </summary>
        /// <param name="cardID">Card ID</param>
        /// <param name="side">Which side should be rendered? (answer or question)</param>
        /// <param name="answer">CurrentAnswer of the user</param>
        /// <param name="correct">True if the answer was correct</param>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public string GenerateCard(int cardID, Side side, string answer, bool correct)
        {
            return GenerateCard(Cards.GetCardByID(cardID), side, answer, correct);
        }
        /// <summary>
        /// Generates the card.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="answer">The answer.</param>
        /// <param name="correct">if set to <c>true</c> [correct].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-01-04</remarks>
        public string GenerateCard(Card card, Side side, string answer, bool correct)
        {
            XmlNode cardNode = (XmlNode)card.BaseCard.Card;
            XmlDocument cardDocument = new XmlDocument();
            cardDocument.CreateXmlDeclaration("1.0", "utf-16", "");
            cardDocument.LoadXml(cardNode.OuterXml);

            //PrepareMedia(cardDocument);

            XsltArgumentList xsltArguments = new XsltArgumentList();
            xsltArguments.AddParam("correctAnswerText", string.Empty, Properties.Resources.XSL_CORRECT);
            xsltArguments.AddParam("correctFeedbackText", string.Empty, Properties.Resources.XSL_FEEDBACK_CORRECT);
            xsltArguments.AddParam("wrongFeedbackText", string.Empty, Properties.Resources.XSL_FEEDBACK_WRONG);
            xsltArguments.AddParam("youEnteredText", string.Empty, Properties.Resources.XSL_YOUENTERED);
            xsltArguments.AddParam("clickForQuestion", string.Empty, Properties.Resources.XSL_CLICKFORQ);
            xsltArguments.AddParam("clickForExample", string.Empty, Properties.Resources.XSL_CLICKFORE);
            xsltArguments.AddParam("resizePicture", string.Empty, Properties.Resources.XSL_RESIZE_PICTURE);
            xsltArguments.AddParam("restorePicture", string.Empty, Properties.Resources.XSL_RESTORE_PICTURE);
            xsltArguments.AddParam("listeningModeText", string.Empty, Properties.Resources.XSL_LISTENING_MODE_IMAGE_BUTTON_TEXT);
            xsltArguments.AddParam("userAnswer", string.Empty, answer);
            xsltArguments.AddParam("correct", string.Empty, correct ? "true" : "false");

            // pass through what mode(s) we are in
            if (card.BaseCard is PreviewCard)
                xsltArguments.AddParam("slideshowMode", string.Empty, true.ToString().ToLower());
            else
                xsltArguments.AddParam("slideshowMode", string.Empty, this.CurrentLearnLogic.SlideShow.ToString().ToLower());
            xsltArguments.AddParam("selfAssessmentMode", string.Empty, this.Settings.SelfAssessment.ToString().ToLower());
            xsltArguments.AddParam("learningBox", string.Empty, this.LearningBox);
            xsltArguments.AddParam("cardBox", string.Empty, card.BaseCard.Box);

            // the promoted property of CardStateChangedShowResultEventArgs is passed through as bool correct
            xsltArguments.AddParam("promoted", string.Empty, correct ? "true" : "false");

            string promoteMessage;
            if (card.BaseCard.Box == 10)
            {
                promoteMessage = Properties.Resources.XSL_PROMOTE_HIGHEST_MESSAGE;
            }
            else
            {
                promoteMessage = Properties.Resources.XSL_PROMOTE_MESSAGE.Replace("{box_number}", card.BaseCard.Box.ToString());
            }

            xsltArguments.AddParam("promotedMessage", string.Empty, promoteMessage);
            xsltArguments.AddParam("demotedMessage", string.Empty, Properties.Resources.XSL_DEMOTE_MESSAGE);
            //in case we are in self assement mode
            xsltArguments.AddParam("selfassesmentDemotedMessage", string.Empty, Properties.Resources.XSL_SELFASSESMENT_DEMOTE_MESSAGE);
            xsltArguments.AddParam("selfassesmentPromotedMessage", string.Empty, Properties.Resources.XSL_SELFASSESMENT_PROMOTE_MESSAGE);



            switch (LearnMode)
            {
                case LearnModes.Sentence:
                    xsltArguments.AddParam("mode", string.Empty, "sentencemode");
                    break;
                case LearnModes.ListeningComprehension:
                    xsltArguments.AddParam("mode", string.Empty, "listeningmode");
                    break;
                case LearnModes.ImageRecognition:
                    xsltArguments.AddParam("mode", string.Empty, "imagemode");
                    break;
                case LearnModes.MultipleChoice:
                case LearnModes.Word:
                default:
                    xsltArguments.AddParam("mode", string.Empty, "wordmode");
                    break;
            }

            xsltArguments.AddParam("displayImages", string.Empty, Settings.ShowImages.Value ? "true" : "false");
            xsltArguments.AddParam("question2answer", string.Empty, Settings.QueryDirections.Question2Answer.Value ? "true" : "false");
            xsltArguments.AddParam("autoPlaySound", string.Empty, Settings.AutoplayAudio.Value ? "true" : "false");
            xsltArguments.AddParam("baseURL", string.Empty,
                Uri.UnescapeDataString(new Uri(DirectoryName.Replace(@"\", @"/")).AbsoluteUri) + "/");
            xsltArguments.AddParam("stylePath", string.Empty,
                Uri.UnescapeDataString(new Uri(StylePath).AbsoluteUri) + "/");
            xsltArguments.AddExtensionObject("urn:cardobject", card);

            XsltSettings settings = new XsltSettings(false, false);     //disable scripts and document()

            string stylesheet = string.Empty;
            CompiledTransform? ct = card.BaseCard.Settings != null ? (side == Side.Question ? card.BaseCard.Settings.QuestionStylesheet : card.BaseCard.Settings.AnswerStylesheet) : null;

            if (!xslTransformer.ContainsKey(side) || xslTransformer[side] == null)
#if DEBUG
                xslTransformer[side] = new XslCompiledTransform(true);
#else
                xslTransformer[side] = new XslCompiledTransform();
#endif

            if (!CurrentlyLoadedStyleSheet.ContainsKey(side) || CurrentlyLoadedStyleSheet[side] == null)
                CurrentlyLoadedStyleSheet[side] = string.Empty;

            if (!UseDictionaryStyleSheets || !ct.HasValue || !(ct.HasValue && File.Exists(ct.Value.Filename)) || dictionary.Version <= 266)
            {
                stylesheet = (side == Side.Question ? QuestionStyleSheet : AnswerStyleSheet);

                if (CurrentlyLoadedStyleSheet[side] != stylesheet)
                {
                    CurrentlyLoadedStyleSheet[side] = stylesheet;
                    xslTransformer[side].Load(stylesheet, settings, new XmlUrlResolver());
                }
            }
            else
            {
                if (CurrentlyLoadedStyleSheet[side] != ct.Value.XslContent)
                {
                    CurrentlyLoadedStyleSheet[side] = ct.Value.XslContent;
                    xslTransformer[side].Load(XmlTextReader.Create(new StringReader(ct.Value.XslContent)), settings, new XmlUrlResolver());
                }
            }

            StringWriter htmlContent = new StringWriter();

            try
            {
                xslTransformer[side].Transform(cardDocument, xsltArguments, htmlContent);
            }
            catch (InvalidOperationException exp)   //Needed to prevent error during loading while the load-Method did not deliver any error...
            {
                Trace.WriteLine("Generate HTML Exception: " + exp.ToString());
                if (File.Exists(CurrentlyLoadedStyleSheet[side]))
                    xslTransformer[side].Load(CurrentlyLoadedStyleSheet[side], new XsltSettings(false, false), new XmlUrlResolver());
                else
                    xslTransformer[side].Load(XmlTextReader.Create(new StringReader(CurrentlyLoadedStyleSheet[side])), new XsltSettings(false, false), new XmlUrlResolver());
                xslTransformer[side].Transform(cardDocument, xsltArguments, htmlContent);
            }

            string content = htmlContent.ToString();
#if DEBUG
            try
            {
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), String.Format("{0}.html", side.ToString())), content);
            }
            catch { }
#endif
            return content;
        }

        /// <summary>
        /// Generates the Html file for printing, using given transformation stylesheet.
        /// </summary>
        /// <param name="chapters">Integer Array containing the IDs of the chapters to print</param>
        /// <param name="stylesheet">Card ID</param>
        /// <returns>Path to the print file</returns>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        /// <remarks>Documented by Dev02, 2007-11-22</remarks>
        public string GeneratePrintOut(List<ICard> cards, string stylesheet, BackgroundWorker backgroundworker, BackgroundWorker mediaprepareworker)
        {
            try
            {
                //generate new xml dictionary document for printing
                XmlDocument printDocument = new System.Xml.XmlDocument();
                XmlNode printDocumentDictionary = printDocument.AppendChild(printDocument.CreateElement("dictionary"));

                //attach each card as xml to the xml document
                foreach (ICard card in cards)
                {
                    printDocumentDictionary.AppendChild(printDocument.ImportNode(card.Card, true));
                }

                XsltArgumentList arguments = new XsltArgumentList();
                PrintStatusReport printstatusreport;
                arguments.AddParam("questioncaption", String.Empty, Settings.QuestionCaption);
                arguments.AddParam("answercaption", String.Empty, Settings.AnswerCaption);
                arguments.AddParam("Description", String.Empty, dictionary.Description);

                arguments.AddExtensionObject("urn:status", printstatusreport = new PrintStatusReport(backgroundworker, cards.Count, mediaprepareworker));
                arguments.AddExtensionObject("urn:cardobject", new Card(cards[0], this));

                return Transform(printDocument, stylesheet, arguments, printstatusreport);
            }
            catch (Exception)
            {
                return string.Format(Properties.Resources.ERROR_PAGE, Properties.Resources.XSL_ERROR_CAPTION,
                    String.Format(Properties.Resources.XSL_ERROR_TEXT, stylesheet));
            }
        }

        /// <summary>
        /// Gets the selected cards to print out.
        /// </summary>
        /// <param name="querystructs">The querystructs.</param>
        /// <param name="cardorder">The cardorder.</param>
        /// <param name="cardorderdir">The cardorderdir.</param>
        /// <returns>The selected cards.</returns>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        public List<ICard> GetPrintOutCards(List<QueryStruct> querystructs, DAL.Interfaces.QueryOrder cardorder, DAL.Interfaces.QueryOrderDir cardorderdir)
        {
            if (querystructs == null || querystructs.Count < 1)
                return null;

            List<ICard> cards = dictionary.Cards.GetCards(querystructs.ToArray(), cardorder, cardorderdir, 0);
            return cards;
        }

        /// <summary>
        /// Gets the selected cards to print out.
        /// </summary>
        /// <param name="cardIDs">The card IDs.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        public List<ICard> GetPrintOutCards(List<int> cardIDs, DAL.Interfaces.QueryOrder cardorder, DAL.Interfaces.QueryOrderDir cardorderdir)
        {
            if (cardIDs == null || cardIDs.Count < 1)
                return null;

            List<ICard> cards = new List<ICard>();

            foreach (int id in cardIDs)
            {
                cards.Add(dictionary.Cards.Get(id));
            }

            if (cardorder != QueryOrder.None && cardorder != QueryOrder.Timestamp && cardorder != QueryOrder.Random && cardorder != QueryOrder.Id)
            {
                cards.Sort(
                    delegate(ICard left, ICard right)
                    {
                        int result;
                        switch (cardorder)
                        {
                            case QueryOrder.Chapter:
                                result = Comparer<int>.Default.Compare(left.Chapter, right.Chapter);
                                break;
                            case QueryOrder.Box:
                                result = Comparer<int>.Default.Compare(left.Box, right.Box);
                                break;
                            case QueryOrder.Answer:
                                result = Comparer<string>.Default.Compare(left.Answer.ToString(), right.Answer.ToString());
                                break;
                            case QueryOrder.Question:
                                result = Comparer<string>.Default.Compare(left.Question.ToString(), right.Question.ToString());
                                break;
                            default:
                                result = 0;
                                break;
                        }
                        return result;
                    });
            }

            if (cardorderdir == QueryOrderDir.Descending)
                cards.Reverse();

            return cards;
        }

        /// <summary>
        /// Class for reporting and displaying the current printout status report.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-11-26</remarks>
        public class PrintStatusReport
        {
            private int CardCount;
            private int Position = 0;
            private BackgroundWorker worker;
            private BackgroundWorker prepareworker;
            private int preparenumber = 0;
            private int preparecount = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="PrintStatusReport"/> class.
            /// </summary>
            /// <param name="backgroundworker">The backgroundworker.</param>
            /// <param name="count">The card count.</param>
            /// <remarks>Documented by Dev02, 2007-11-26</remarks>
            public PrintStatusReport(BackgroundWorker backgroundworker, int cardCount, BackgroundWorker preparebackgroundworker)
            {
                worker = backgroundworker;
                prepareworker = preparebackgroundworker;
                CardCount = cardCount;
            }

            /// <summary>
            /// Sends the status.
            /// </summary>
            /// <returns></returns>
            /// <remarks>Documented by Dev02, 2007-11-26</remarks>
            public bool SendStatus()
            {
                Position++;
                if (worker != null && Position % 5 == 0)
                    worker.ReportProgress((int)((Position * 1.0) / (CardCount * 1.0) * 100));
                return true;
            }

            /// <summary>
            /// Sets the prepare Media status.
            /// </summary>
            /// <param name="number">The number.</param>
            /// <param name="count">The count.</param>
            /// <remarks>Documented by Dev02, 2008-01-24</remarks>
            public void SetPrepareStatus()
            {
                preparenumber++;
                if (preparenumber > preparecount)
                    preparenumber = preparecount;
                if (prepareworker != null && preparecount > 0 && preparenumber % 5 == 0)
                    prepareworker.ReportProgress((int)(preparenumber * 1.0 / preparecount * 100));
            }

            /// <summary>
            /// Gets or sets the prepare count.
            /// </summary>
            /// <value>The prepare count.</value>
            /// <remarks>Documented by Dev02, 2008-01-24</remarks>
            public int PrepareCount
            {
                get { return preparecount; }
                set { preparecount = value; }
            }
        }

        /// <summary>
        /// Transforms the specified card document.
        /// </summary>
        /// <param name="document">The card document.</param>
        /// <param name="stylesheet">The stylesheet.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The transformed document as HTML.</returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        private string Transform(XmlDocument document, string stylesheet, XsltArgumentList xsltArguments, PrintStatusReport printstatusreport)
        {
            if (dictionary.Parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
                PrepareMedia(document, printstatusreport);

            XslCompiledTransform xslTransformer = new XslCompiledTransform();
            //XsltArgumentList xsltArguments = new XsltArgumentList();
            //if (arguments != null)
            //{
            //    foreach (object argument in arguments)
            //    {
            //        xsltArguments.AddParam("chapters", String.Empty, argument);
            //    }
            //}
            xsltArguments.AddParam("baseURL", string.Empty,
                Uri.UnescapeDataString(new Uri(DirectoryName.Replace(@"\", @"/")).AbsoluteUri) + "/");
            xsltArguments.AddParam("titleText", string.Empty, Path.GetFileNameWithoutExtension(DictionaryPath));
            XsltSettings settings = new XsltSettings(false, false);     //disable scripts and document()
            xslTransformer.Load(stylesheet, settings, new XmlUrlResolver());

            StringWriter htmlContent = new StringWriter();
            xslTransformer.Transform(document, xsltArguments, htmlContent);
            //StringBuilder htmlContent = new StringBuilder();
            //StringWriter htmlWriter = new StringWriter(htmlContent);
            //XmlTextWriter htmlTextWriter = new XmlTextWriter(htmlWriter);
            //xslTransformer.Transform(document, xsltArguments, htmlTextWriter);

            return htmlContent.ToString();
        }

        /// <summary>
        /// Prepares the Media.
        /// </summary>
        /// <param name="cardNode">The cardNode.</param>
        /// <remarks>Documented by Dev03, 2007-08-17</remarks>
        private void PrepareMedia(XmlNode card)
        {
            PrepareMedia(card, null);
        }

        /// <summary>
        /// Prepares the Media.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="printstatusreport">The printstatusreport.</param>
        /// <remarks>Documented by Dev03, 2007-08-17</remarks>
        private void PrepareMedia(XmlNode card, PrintStatusReport printstatusreport)
        {
            if (printstatusreport != null)
                printstatusreport.PrepareCount = card.SelectNodes(XPathResourceFilter).Count;

            foreach (XmlNode resource in card.SelectNodes(XPathResourceFilter))
            {
                string path = resource.Value;
                try
                {
                    if (DAL.Helper.GetMediaType(path) == EMedia.Image)
                    {
                        //try to read the image size from the file if it is not given ([ML-128])
                        XmlElement xeMedia = (XmlElement)resource.ParentNode;
                        if (xeMedia.GetAttribute("width") == String.Empty)
                        {
                            using (System.Drawing.Bitmap image = new System.Drawing.Bitmap(path))
                            {
                                System.Drawing.Size size = image.Size;
                                xeMedia.SetAttribute("width", size.Width.ToString());
                                xeMedia.SetAttribute("height", size.Height.ToString());
                            }
                        }
                    }
                }
                catch { }
                if (printstatusreport != null)
                    printstatusreport.SetPrepareStatus();
            }
        }
        # endregion
        # region other

        /// <summary>
        /// Resets the learning progress.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-08</remarks>
        public void ResetLearningProgress()
        {
            LoadDictionary(dictionary.ResetLearningProgress());

            //Open new UserSession
            Log.OpenUserSession(dictionary.Id, dictionary.Parent);

            PoolEmptyMessageShown = false;
            Save();
        }

        /// <summary>
        /// Enables the card.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="enabled">if set to <c>true</c>, the card is enabled, otherwise disabled.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void EnableCard(int cardID, bool enabled)
        {
            dictionary.Cards.Get(cardID).Active = enabled;
        }

        /// <summary>
        /// Creates a new Media object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="path">The path.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="isExample">if set to <c>true</c> [is example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-11</remarks>
        public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
        {
            return dictionary.CreateMedia(type, path, IsActive, isDefault, isExample);
        }

        /// <summary>
        /// Gets the cards from a chapter.
        /// </summary>
        /// <param name="chapter">The chapter.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public List<ICard> GetCardsFromChapter(IChapter chapter)
        {
            return dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(chapter.Id, -1) }, QueryOrder.None, QueryOrderDir.Ascending, -1);
        }

        public ISettings CreateSettings()
        {
            return dictionary.CreateSettings();
        }

        /// <summary>
        /// Creates the card style.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-12-23</remarks>
        public ICardStyle CreateCardStyle()
        {
            if (dictionary.UserSettings == null)
                dictionary.UserSettings = dictionary.CreateSettings();
            if (dictionary.UserSettings.Style == null)
                dictionary.UserSettings.Style = dictionary.CreateCardStyle();
            return dictionary.UserSettings.Style;
        }

        /// <summary>
        /// Calculates the space used by the current dictionary and all its resources.
        /// </summary>
        /// <param name="space">The space used.</param>
        /// <param name="count">The count of files.</param>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public void UsedDiskSpace(out long space, out int count)
        {
            space = dictionary.DictionarySize;
            count = dictionary.DictionaryMediaObjectsCount;
        }

        /// <summary>
        /// Calculates the space used by the current dictionary and all its resources, asynchronously.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public void UsedDiskSpaceAsync()
        {
            System.Threading.Thread thread = new System.Threading.Thread(delegate()
            {
                try
                {
                    UsedDiskSpaceCompleteEventArgs e = new UsedDiskSpaceCompleteEventArgs();
                    UsedDiskSpace(out e.space, out e.count);

                    if (UsedDiskSpaceComplete != null)
                        UsedDiskSpaceComplete(this, e);
                }
                catch (Exception exp)
                {
                    Trace.WriteLine("UsedDiskSpace Calculation exception: " + exp.ToString());
                }
            });
            thread.Start();
        }

        /// <summary>
        /// Gets fired when the async disk space calculation is complete.
        /// </summary>
        public event UsedDiskSpaceCompleteHandler UsedDiskSpaceComplete;

        public delegate void UsedDiskSpaceCompleteHandler(object sender, UsedDiskSpaceCompleteEventArgs e);

        /// <summary>
        /// Holds the custom event args for the async disk space calculation.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public class UsedDiskSpaceCompleteEventArgs : EventArgs
        {
            /// <summary>
            /// The space used.
            /// </summary>
            public long space = 0;
            /// <summary>
            /// The count of files.
            /// </summary>
            public int count = 0;
        };

        # endregion
        #region IDisposable Members

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
                if (disposing)
                {
                    Close();
                }
            }
            isDisposed = true;
        }

        #endregion
    }

    /// <summary>
    /// Used to send the results of a copy process.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-04-03</remarks>
    public class CopyToEventArgs : EventArgs
    {
        public Dictionary source;
        public Dictionary target;
        public User sourceUser;
        public User targetUser;
        /// <summary>
        /// [true] if the copy process was successfull.
        /// </summary>
        public bool success;
        /// <summary>
        /// If not sucessfully, otherwise null.
        /// </summary>
        public Exception Exception;

        internal CopyToEventArgs(Dictionary source, Dictionary target, User sourceUser, User targetUser, bool success, Exception excepion)
        {
            this.source = source;
            this.target = target;
            this.sourceUser = sourceUser;
            this.targetUser = targetUser;
            this.success = success;
            this.Exception = excepion;
        }
    }
}
