using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;
using MLifter.BusinessLayer;

using ICSharpCode.SharpZipLib.Zip;

using MLifterTest.BusinessLayer;


namespace MLifterTest.BusinessLayer
{
    /// <summary>
    /// Summary Description for CopyTest
    /// </summary>
    [TestClass]
    public class CopyToTest
    {
        private TestContext testContextInstance;

        public static string TestDic { get; set; }
        public static string TestFolder { get; set; }
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(TestFolder);
            MyClassCleanup(); //in case the LM is still open from another test
            ExtractTestDictionary();
            InitializeDictionary();

        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            try
            {
                Directory.Delete(TestFolder, true);
            }
            catch { }
            TestInfrastructure.MyClassCleanup();

        }
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        //

        /// <summary>
        /// Extracts the test dictionary.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-16</remarks>
        public static void ExtractTestDictionary()
        {
            //extract test LM
            MemoryStream zipStream = new MemoryStream(Properties.Resources.StartupFolders);
            ZipInputStream zipFile = new ZipInputStream(zipStream);
            ZipEntry entry;

            while ((entry = zipFile.GetNextEntry()) != null)
            {
                string directoryName = Path.Combine(TestFolder, Path.GetDirectoryName(entry.Name));
                string fileName = Path.GetFileName(entry.Name);

                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                if (fileName.Length > 0)
                {
                    if (Path.GetExtension(fileName).ToLowerInvariant() == Helper.OdxExtension)
                        TestDic = Path.Combine(directoryName, fileName);

                    FileStream stream = File.Open(Path.Combine(directoryName, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
                    int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];

                    while (bufferSize > 0)
                    {
                        bufferSize = zipFile.Read(buffer, 0, buffer.Length);
                        if (bufferSize > 0)
                            stream.Write(buffer, 0, bufferSize);
                    }

                    stream.Close();
                }
            }

            zipFile.Close();
        }



        /// <summary>
        /// Initializes the dictionary.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-05-16</remarks>
        public static void InitializeDictionary()
        {
        }

        #endregion

        /// <summary>
        /// Simples the test.
        /// Create an Instance of a IDictionary, check if reference is != null and verify the typ of the object.
        /// </summary>
        /// <remarks>Documented by Dev10, 2008-28-07</remarks>
        [TestMethod]
        [TestProperty("BusinessLayer", "Matbre00"), DataSource("TestSources")]
        public void CopyToBasicTest()
        {
            //Perform Test only if we use a db connection otherwise test case makes no sense
            if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) != "File")
            {
                IUser sourceUser = null;
                IUser targetUser = null;
                try
                {
                    string repositoryNameFinal = "finalTargetForCopyTest" + System.Environment.MachineName.ToLower();

                    //Do we have a target to copy from
                    Assert.IsTrue(File.Exists(TestDic), "Test Learning Module file cannot be found.");
                    ConnectionStringStruct sourceConnection = new ConnectionStringStruct(DatabaseType.Xml, CopyToTest.TestDic, false);
                    sourceUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                        sourceConnection, (DataAccessErrorDelegate)delegate { return; }, this);
                    if (!authenticationUsers.ContainsKey(sourceConnection.ConnectionString))
                        authenticationUsers.Add(sourceConnection.ConnectionString, sourceUser.AuthenticationStruct);

                    //Copy the reference LM to the new persistent LM
                    ConnectionStringStruct dbConnection;
                    using (Dictionary dbTarget = TestInfrastructure.GetConnection(TestContext))
                    {
                        IUser dbUser = dbTarget.DictionaryDAL.Parent.CurrentUser;
                        dbConnection = dbUser.ConnectionString;
                        if (!authenticationUsers.ContainsKey(dbConnection.ConnectionString))
                            authenticationUsers.Add(dbConnection.ConnectionString, dbUser.AuthenticationStruct);
                        dbTarget.Dispose();

                        finished = false;
                        LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
                        LearnLogic.CopyLearningModule(sourceConnection, dbConnection,
                            GetUser, (MLifter.DAL.Tools.CopyToProgress)delegate(string m, double p) { return; }, (DataAccessErrorDelegate)delegate { return; }, null);
                        while (!finished) { System.Threading.Thread.Sleep(100); };
                        LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
                    }


                    //copy to another persistent LM where we use Save to store as odx again
                    ConnectionStringStruct targetConnection;
                    using (Dictionary target = TestInfrastructure.GetPersistentLMConnection(TestContext, "sqlce"))
                    {
                        targetUser = target.DictionaryDAL.Parent.CurrentUser;
                        targetConnection = targetUser.ConnectionString;
                        if (!authenticationUsers.ContainsKey(targetConnection.ConnectionString))
                            authenticationUsers.Add(targetConnection.ConnectionString, targetUser.AuthenticationStruct);
                        target.Dispose();

                        finished = false;
                        LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
                        LearnLogic.CopyLearningModule(dbConnection, targetConnection,
                            GetUser, (MLifter.DAL.Tools.CopyToProgress)delegate(string m, double p) { return; }, (DataAccessErrorDelegate)delegate { return; }, null);
                        while (!finished) { System.Threading.Thread.Sleep(100); };
                        LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
                    }


                    using (Dictionary source = new Dictionary(sourceUser.Open(), null))
                    using (Dictionary target = new Dictionary(targetUser.Open(), null))
                    {
                        CompareChapters(source.Chapters.Chapters, target.Chapters.Chapters);

                        //Verification Code compare finalTarget with the reference target
                        CompareCards(source.Cards.Cards, target.Cards.Cards, source, target);

                        //Compare Settings
                        CompareSettings(source.Settings, target.Settings);
                        if ((source.Settings != null) && (target.Settings != null))
                        {
                            //Compare Styles
                            CompareStyles(source.Settings.Style, target.Settings.Style);
                        }
                    }
                }
                finally
                {
                    if (sourceUser != null)
                        sourceUser.Logout();
                    if (targetUser != null)
                        targetUser.Logout();
                }
            }
        }

        private bool finished = false;
        void LearnLogic_CopyToFinished(object sender, EventArgs e)
        {
            finished = true;
        }

        private System.Collections.Generic.Dictionary<string, UserStruct> authenticationUsers = new System.Collections.Generic.Dictionary<string, UserStruct>();

        private UserStruct? GetUser(UserStruct user, ConnectionStringStruct connection)
        {
            return authenticationUsers[connection.ConnectionString];
        }

        /// <summary>
        /// Compares the cards.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="copy">The copy.</param>
        /// <remarks>Documented by Dev03, 2008-09-26</remarks>
        private void CompareCards(IList<ICard> reference, IList<ICard> copy, Dictionary referenceDic, Dictionary copyDic)
        {
            Assert.AreEqual<int>(reference.Count, copy.Count, "Numbers of cards are not equal");

            //create Lists to work on
            List<ICard> referenceList = new List<ICard>();
            List<ICard> copyList = new List<ICard>();
            referenceList.AddRange(reference);
            copyList.AddRange(copy);

            //compare the cards
            foreach (ICard card in reference)
            {
                ICard match = copyList.Find(
                    delegate(ICard cardCopy)
                    {
                        bool isMatch = true;
                        isMatch = isMatch && (card.Active == cardCopy.Active);
                        isMatch = isMatch && (card.Answer.ToString() == cardCopy.Answer.ToString());
                        isMatch = isMatch && (card.Question.ToString() == cardCopy.Question.ToString());
                        isMatch = isMatch && (card.AnswerExample.ToString() == cardCopy.AnswerExample.ToString());
                        isMatch = isMatch && (card.QuestionExample.ToString() == cardCopy.QuestionExample.ToString());
                        isMatch = isMatch && (card.AnswerDistractors.ToString() == cardCopy.AnswerDistractors.ToString());
                        isMatch = isMatch && (card.QuestionDistractors.ToString() == cardCopy.QuestionDistractors.ToString());
                        isMatch = isMatch && (card.Box == cardCopy.Box);
                        IChapter chapter2search = FindChapter(referenceDic.Chapters.Chapters, card.Chapter);
                        isMatch = isMatch && (FindChapter(copyDic.Chapters.Chapters, chapter2search) != null);


                        Debug.WriteLine("#####" + card.Id);
                        Debug.WriteLine("########" + card.Answer);
                        Debug.WriteLine("########" + cardCopy.Answer);
                        Debug.WriteLine("########" + isMatch);

                        return isMatch;
                    }
                );

                Assert.IsTrue(match != null, String.Format("Card not found:\n{0}", card.ToString()));
                //CompareMedia(card, match);

                //AAB_MBR not implemented for XML# CompareSettings(card.Settings, match.Settings);
                if ((card.Settings != null) && (match.Settings != null))
                {
                    CompareStyles(card.Settings.Style, match.Settings.Style);
                }
            }
        }

        /// <summary>
        /// Compares the chapters.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="copy">The copy.</param>
        /// <remarks>Documented by Dev03, 2008-09-26</remarks>
        private void CompareChapters(IList<IChapter> reference, IList<IChapter> copy)
        {
            Assert.AreEqual<int>(reference.Count, copy.Count, "Numbers of chapters are not equal");

            //create Lists to work on
            List<IChapter> referenceList = new List<IChapter>();
            List<IChapter> copyList = new List<IChapter>();
            referenceList.AddRange(reference);
            copyList.AddRange(copy);

            //compare the cards
            foreach (IChapter chapter in reference)
            {
                IChapter match = copyList.Find(
                    delegate(IChapter chapterCopy)
                    {
                        bool isMatch = true;
                        isMatch = isMatch && (chapter.Title == chapterCopy.Title);
                        isMatch = isMatch && (chapter.Description == chapterCopy.Description);

                        Debug.WriteLine("#####" + chapter.Id);
                        Debug.WriteLine("########" + chapter.Title);
                        Debug.WriteLine("########" + chapter.Description);
                        Debug.WriteLine("########" + chapterCopy.Title);
                        Debug.WriteLine("########" + chapterCopy.Description);
                        Debug.WriteLine("########" + isMatch);

                        return isMatch;
                    }
                );

                Assert.IsTrue(match != null, "Chapter not found");

                //AAB_MBR not implemented for XML# CompareSettings(chapter.Settings, match.Settings);
                if ((chapter.Settings != null) && (match.Settings != null))
                {
                    CompareStyles(chapter.Settings.Style, match.Settings.Style);
                }
            }
        }



        /// <summary>
        /// Finds the chapter.
        /// </summary>
        /// <param name="list">The list chapters to search.</param>
        /// <param name="chapter2find">The chapter to find.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-26</remarks>
        private IChapter FindChapter(IList<IChapter> chapters2search, IChapter chapter2find)
        {
            List<IChapter> list = new List<IChapter>();
            list.AddRange(chapters2search);
            IChapter match = list.Find(
                delegate(IChapter chapter)
                {
                    bool isMatch = true;
                    isMatch = isMatch && (chapter2find.Title == chapter.Title);
                    isMatch = isMatch && (chapter2find.Description == chapter.Description);
                    return isMatch;
                }
            );
            return match;
        }

        /// <summary>
        /// Finds the chapter.
        /// </summary>
        /// <param name="chapters2search">The list chapters to search.</param>
        /// <param name="chapter2find">The chapter id to find.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-26</remarks>
        private IChapter FindChapter(IList<IChapter> chapters2search, int chapter2find)
        {
            List<IChapter> list = new List<IChapter>();
            list.AddRange(chapters2search);
            IChapter match = list.Find(
                delegate(IChapter chapter)
                {
                    bool isMatch = true;
                    isMatch = isMatch && (chapter2find == chapter.Id);
                    return isMatch;
                }
            );
            return match;
        }

        /// <summary>
        /// Compares two objects that implement ISettings.
        /// </summary>
        /// <param name="one">The first ISettings object.</param>
        /// <param name="two">The second ISettings object.</param>
        /// <remarks>Documented by Dev03, 2008-09-26</remarks>
        private void CompareSettings(ISettings one, ISettings two)
        {
            if ((one == null) || (two == null))
            {
                Assert.AreEqual(one, two, "Both settings should be null!");
                return;
            }
            if ((one != null) && (two != null))
            {
                //Basic type properties
                Assert.AreEqual<string>(one.AnswerCaption, two.AnswerCaption, "AnswerCaption do not match");
                Assert.AreEqual<bool?>(one.AutoBoxSize, two.AutoBoxSize, "AutoBoxSize do not match");
                Assert.AreEqual<bool?>(one.AutoplayAudio, two.AutoplayAudio, "AutoplayAudio do not match");
                Assert.AreEqual<bool?>(one.CaseSensitive, two.CaseSensitive, "CaseSensitive do not match");
                Assert.AreEqual<bool?>(one.ConfirmDemote, two.ConfirmDemote, "ConfirmDemote do not match");
                Assert.AreEqual<bool?>(one.CorrectOnTheFly, two.CorrectOnTheFly, "CorrectOnTheFly do not match");
                Assert.AreEqual<bool?>(one.EnableCommentary, two.EnableCommentary, "EnableCommentary do not match");
                Assert.AreEqual<bool?>(one.EnableTimer, two.EnableTimer, "EnableTimer do not match");
                Assert.AreEqual<bool?>(one.PoolEmptyMessageShown, two.PoolEmptyMessageShown, "PoolEmptyMessageShown do not match");
                Assert.AreEqual<string>(one.QuestionCaption, two.QuestionCaption, "QuestionCaption do not match");
                Assert.AreEqual<bool?>(one.RandomPool, two.RandomPool, "RandomPool do not match");
                Assert.AreEqual<bool?>(one.SelfAssessment, two.SelfAssessment, "SelfAssessment do not match");
                Assert.AreEqual<bool?>(one.ShowImages, two.ShowImages, "ShowImages do not match");
                Assert.AreEqual<bool?>(one.ShowStatistics, two.ShowStatistics, "ShowStatistics do not match");
                Assert.AreEqual<bool?>(one.SkipCorrectAnswers, two.SkipCorrectAnswers, "SkipCorrectAnswers do not match");
                Assert.AreEqual<string>(one.StripChars, two.StripChars, "StripChars do not match");
                Assert.AreEqual<string>(one.ToString(), two.ToString(), ".ToString() do not match");
                Assert.AreEqual<bool?>(one.UseLMStylesheets, two.UseLMStylesheets, "UseLMStylesheets do not match");

                //Other Types
                Assert.IsTrue(one.AnswerCulture.Equals(two.AnswerCulture), "AnswerCulture do not match");
                Assert.IsTrue(one.QuestionCulture.Equals(two.QuestionCulture), "QuestionCulture do not match");

                Assert.IsTrue(one.GradeSynonyms.Equals(two.GradeSynonyms), "GradeSynonyms do not match");

                Assert.IsTrue(one.GradeTyping.Equals(two.GradeTyping), "GradeTyping do not match");

                Assert.IsTrue(checkMedia(one.Logo, two.Logo), "Logo do not match");

                Assert.IsTrue(one.MultipleChoiceOptions.Equals(two.MultipleChoiceOptions), "MultipleChoiceOptions do not match");

                Assert.IsTrue(one.QueryDirections.Equals(two.QueryDirections), "QueryDirections do not match");

                Assert.IsTrue(one.QueryTypes.Equals(two.QueryTypes), "QueryTypes do not match");

                Assert.IsTrue(checkStyleSheet(one.AnswerStylesheet, two.AnswerStylesheet), "AnswerStylesheet do not match");
                Assert.IsTrue(checkStyleSheet(one.QuestionStylesheet, two.QuestionStylesheet), "QuestionStylesheet do not match");

                checkSelectedLearnChapters(one.SelectedLearnChapters, two.SelectedLearnChapters);

                checkCommentarySounds(one.CommentarySounds, two.CommentarySounds);

                //Snooze Options
                Assert.AreEqual<bool?>(one.SnoozeOptions.IsCardsEnabled, two.SnoozeOptions.IsCardsEnabled, "SnoozeOptions.IsCardsEnabled do not match");
                Assert.AreEqual<bool?>(one.SnoozeOptions.IsRightsEnabled, two.SnoozeOptions.IsRightsEnabled, "SnoozeOptions.IsRightsEnabled do not match");
                Assert.AreEqual<bool?>(one.SnoozeOptions.IsTimeEnabled, two.SnoozeOptions.IsTimeEnabled, "SnoozeOptions.IsTimeEnabled do not match");
                Assert.AreEqual<int?>(one.SnoozeOptions.SnoozeCards, two.SnoozeOptions.SnoozeCards, "SnoozeOptions.SnoozeCards do not match");
                Assert.AreEqual<int?>(one.SnoozeOptions.SnoozeHigh, two.SnoozeOptions.SnoozeHigh, "SnoozeOptions.SnoozeHigh do not match");
                Assert.AreEqual<int?>(one.SnoozeOptions.SnoozeLow, two.SnoozeOptions.SnoozeLow, "SnoozeOptions.SnoozeLow do not match");
                Assert.AreEqual<int?>(one.SnoozeOptions.SnoozeRights, two.SnoozeOptions.SnoozeRights, "SnoozeOptions.SnoozeRights do not match");
                Assert.AreEqual<int?>(one.SnoozeOptions.SnoozeTime, two.SnoozeOptions.SnoozeTime, "SnoozeOptions.SnoozeTime do not match");
                Assert.AreEqual<string>(one.SnoozeOptions.SnoozeMode.ToString(), two.SnoozeOptions.SnoozeMode.ToString(), "SnoozeOptions.SnoozeMode.ToString() do not match");
                Assert.AreEqual<ESnoozeMode>(one.SnoozeOptions.SnoozeMode.Value, one.SnoozeOptions.SnoozeMode.Value, "SnoozeOptions.SnoozeMode.Value do not match");

                //Styles
                CompareStyles(one.Style, two.Style);

            }
        }

        /// <summary>
        /// Checks the commentary sounds.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <remarks>Documented by Dev10, 2008-29-09</remarks>
        private void checkCommentarySounds(Dictionary<CommentarySoundIdentifier, IMedia> one, Dictionary<CommentarySoundIdentifier, IMedia> two)
        {
            Assert.AreEqual<int>(one.Count, two.Count, "Number of commentary Sounds do not match");
            foreach (KeyValuePair<CommentarySoundIdentifier, IMedia> kvp in one)
            {
                if (two.ContainsKey(kvp.Key))
                {
                    Assert.IsTrue(checkMedia(two[kvp.Key], kvp.Value), "Media do not match");
                }
                else
                    Assert.Fail("Key is missing in one directory, in commentary Sounds");

            }


        }

        /// <summary>
        /// Checks Media.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-29-09</remarks>
        private bool checkMedia(IMedia one, IMedia two)
        {
            if (one != null && two != null)
            {
                bool isMatch = true;
                isMatch = isMatch && (one.Active == two.Active);
                isMatch = isMatch && (one.Default == two.Default);
                isMatch = isMatch && (one.Example == two.Example);
                isMatch = isMatch && (one.MimeType == two.MimeType);
                isMatch = isMatch && (one.MediaType == two.MediaType);
                isMatch = isMatch && (one.Stream.Length == two.Stream.Length);
                return isMatch;
            }
            else if (one == null && two == null)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Checks the selected learn chapters, whether they are available in both LMs.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <remarks>Documented by Dev10, 2008-29-09</remarks>
        private void checkSelectedLearnChapters(IList<int> one, IList<int> two)
        {
            //List<int> twoSelectedLearnChapters = new List<int>();
            //twoSelectedLearnChapters.AddRange(two);
            //foreach (int chapter in one)
            //{
            //    Assert.IsTrue(twoSelectedLearnChapters.Exists(c => c == chapter), string.Format("Chapter {0} not found in both.", chapter));
            //}

            Assert.AreEqual<int>(one.Count, two.Count, "Selected learning chapters are not equal");

            return;
        }

        /// <summary>
        /// Checks the style sheet for e.g. Answer StyleSheet or Question StyleSheet.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-29-09</remarks>
        private bool checkStyleSheet(CompiledTransform? one, CompiledTransform? two)
        {
            if (one.HasValue && two.HasValue)
                return one.Value.Equals(two.Value);
            else if (!one.HasValue && !two.HasValue)
                return true;
            else if (!one.HasValue && two.Value.XslContent == string.Empty)
                return true;
            else if (!two.HasValue && one.Value.XslContent == string.Empty)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Compares the Media of card one with those of card two.
        /// </summary>
        /// <param name="one">The card one.</param>
        /// <param name="two">The card two.</param>
        /// <remarks>Documented by Dev03, 2008-09-29</remarks>
        private void CompareMedia(ICard one, ICard two)
        {
            List<IMedia> answerMedia = new List<IMedia>();
            answerMedia.AddRange(two.AnswerMedia);
            foreach (IMedia media in one.AnswerMedia)
            {
                Assert.IsTrue(
                    answerMedia.Exists(
                        delegate(IMedia m)
                        {
                            bool isMatch = true;
                            isMatch = isMatch && (m.Active == media.Active);
                            isMatch = isMatch && (m.Default == media.Default);
                            isMatch = isMatch && (m.Example == media.Example);
                            isMatch = isMatch && (m.MimeType == media.MimeType);
                            isMatch = isMatch && (m.MediaType == media.MediaType);
                            isMatch = isMatch && (m.Stream.Length == media.Stream.Length);
                            return isMatch;
                        }
                    ), String.Format("No match for answer Media item found in card {0}.", one.ToString()));
            }
            List<IMedia> questionMedia = new List<IMedia>();
            questionMedia.AddRange(two.QuestionMedia);
            foreach (IMedia media in one.QuestionMedia)
            {
                Assert.IsTrue(
                    questionMedia.Exists(
                        delegate(IMedia m)
                        {
                            bool isMatch = true;
                            isMatch = isMatch && (m.Active == media.Active);
                            isMatch = isMatch && (m.Default == media.Default);
                            isMatch = isMatch && (m.Example == media.Example);
                            isMatch = isMatch && (m.MimeType == media.MimeType);
                            isMatch = isMatch && (m.MediaType == media.MediaType);
                            isMatch = isMatch && (m.Stream.Length == media.Stream.Length);
                            return isMatch;
                        }
                    ), String.Format("No match for question Media item found in card {0}.", one.ToString()));
            }
        }

        /// <summary>
        /// Compares two objects that implement ICardStyle.
        /// </summary>
        /// <param name="one">The first ICardStyle object one.</param>
        /// <param name="two">The second ICardStyle object.</param>
        /// <remarks>Documented by Dev03, 2008-09-26</remarks>
        private void CompareStyles(ICardStyle one, ICardStyle two)
        {
            if ((one == null) || (two == null))
            {
                Assert.IsTrue(IsEmptyStyle(one), "Both styles should be null! (one)");
                Assert.IsTrue(IsEmptyStyle(two), "Both styles should be null! (two)");
                return;
            }

            Type typeCardStyle = one.GetType();
            foreach (PropertyInfo property in typeCardStyle.GetProperties())
            {
                if (property.GetType().Equals(typeof(ITextStyle)))
                {
                    ITextStyle textStyleOne = property.GetValue(one, null) as ITextStyle;
                    ITextStyle textStyleTwo = property.GetValue(two, null) as ITextStyle;
                    Type typeTextStyle = textStyleOne.GetType();
                    foreach (PropertyInfo prop in typeTextStyle.GetProperties())
                    {
                        if (prop.GetType().Equals(typeof(int)))
                        {
                            int valueOne = (int)prop.GetValue(textStyleOne, null);
                            int valueTwo = (int)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(string)))
                        {
                            string valueOne = (string)prop.GetValue(textStyleOne, null);
                            string valueTwo = (string)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(System.Drawing.Color)))
                        {
                            System.Drawing.Color valueOne = (System.Drawing.Color)prop.GetValue(textStyleOne, null);
                            System.Drawing.Color valueTwo = (System.Drawing.Color)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(System.Drawing.FontFamily)))
                        {
                            System.Drawing.FontFamily valueOne = (System.Drawing.FontFamily)prop.GetValue(textStyleOne, null);
                            System.Drawing.FontFamily valueTwo = (System.Drawing.FontFamily)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(FontSizeUnit)))
                        {
                            FontSizeUnit valueOne = (FontSizeUnit)prop.GetValue(textStyleOne, null);
                            FontSizeUnit valueTwo = (FontSizeUnit)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(CSSFontStyle)))
                        {
                            CSSFontStyle valueOne = (CSSFontStyle)prop.GetValue(textStyleOne, null);
                            CSSFontStyle valueTwo = (CSSFontStyle)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(HorizontalAlignment)))
                        {
                            HorizontalAlignment valueOne = (HorizontalAlignment)prop.GetValue(textStyleOne, null);
                            HorizontalAlignment valueTwo = (HorizontalAlignment)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.GetType().Equals(typeof(VerticalAlignment)))
                        {
                            VerticalAlignment valueOne = (VerticalAlignment)prop.GetValue(textStyleOne, null);
                            VerticalAlignment valueTwo = (VerticalAlignment)prop.GetValue(textStyleTwo, null);
                            Assert.AreEqual(valueOne, valueOne, String.Format("Both values for {0} should be equal!", prop.Name));
                        }
                        else if (prop.Name == "OtherElements")
                        {
                            SerializableDictionary<string, string> otherElementsOne = prop.GetValue(textStyleOne, null) as SerializableDictionary<string, string>;
                            SerializableDictionary<string, string> otherElementsTwo = prop.GetValue(textStyleTwo, null) as SerializableDictionary<string, string>;
                            foreach (string key in otherElementsOne.Keys)
                            {
                                Assert.IsTrue((otherElementsTwo.ContainsKey(key) && otherElementsOne[key].Equals(otherElementsTwo[key])), String.Format("Both values for {0}[{1}] should be equal!", prop.Name, key));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is empty style] [the specified style].
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>
        /// 	<c>true</c> if [is empty style] [the specified style]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2008-09-29</remarks>
        private bool IsEmptyStyle(ICardStyle style)
        {
            if (style == null) return true;
            bool result = true;

            Type typeCardStyle = style.GetType();
            foreach (PropertyInfo property in typeCardStyle.GetProperties())
            {
                if (property.GetType().Equals(typeof(ITextStyle)))
                {
                    ITextStyle textStyle = property.GetValue(style, null) as ITextStyle;
                    Type typeTextStyle = textStyle.GetType();
                    foreach (PropertyInfo prop in typeTextStyle.GetProperties())
                    {
                        if (prop.GetType().Equals(typeof(int)))
                        {
                            int value = (int)prop.GetValue(textStyle, null);
                            result = result && (value == 0);
                        }
                        else if (prop.GetType().Equals(typeof(string)))
                        {
                            string value = (string)prop.GetValue(textStyle, null);
                            result = result && (value.Length == 0);
                        }
                        else if (prop.GetType().Equals(typeof(System.Drawing.Color)))
                        {
                            System.Drawing.Color value = (System.Drawing.Color)prop.GetValue(textStyle, null);
                            result = result && (value == System.Drawing.Color.Empty);
                        }
                        else if (prop.GetType().Equals(typeof(System.Drawing.FontFamily)))
                        {
                            System.Drawing.FontFamily value = (System.Drawing.FontFamily)prop.GetValue(textStyle, null);
                            result = result && (value == null);
                        }
                        else if (prop.GetType().Equals(typeof(FontSizeUnit)))
                        {
                            FontSizeUnit value = (FontSizeUnit)prop.GetValue(textStyle, null);
                            result = result && (value == FontSizeUnit.Pixel);
                        }
                        else if (prop.GetType().Equals(typeof(CSSFontStyle)))
                        {
                            CSSFontStyle value = (CSSFontStyle)prop.GetValue(textStyle, null);
                            result = result && (value == CSSFontStyle.None);
                        }
                        else if (prop.GetType().Equals(typeof(HorizontalAlignment)))
                        {
                            HorizontalAlignment value = (HorizontalAlignment)prop.GetValue(textStyle, null);
                            result = result && (value == HorizontalAlignment.None);
                        }
                        else if (prop.GetType().Equals(typeof(VerticalAlignment)))
                        {
                            VerticalAlignment value = (VerticalAlignment)prop.GetValue(textStyle, null);
                            result = result && (value == VerticalAlignment.None);
                        }
                        else if (prop.Name == "OtherElements")
                        {
                            SerializableDictionary<string, string> otherElements = prop.GetValue(textStyle, null) as SerializableDictionary<string, string>;
                            result = result && (otherElements.Count == 0);
                        }
                    }
                }
            }
            return result;
        }
    }
}
