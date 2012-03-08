using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Diagnostics;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary Description for IWords
    /// </summary>
    [TestClass]
    public class IWordsTests
    {
        public IWordsTests()
        {
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void Cleanup()
        {
            TestInfrastructure.MyClassCleanup();
        }

        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region TestContext
        private TestContext testContextInstance;
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
        #endregion

        /// <summary>
        /// Test the culture property.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void CultureTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, string.Empty))
                {
                    CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                    CultureInfo ai = (CultureInfo)cultures.GetValue(TestInfrastructure.Random.Next(1, cultures.Length));
                    CultureInfo qi = (CultureInfo)cultures.GetValue(TestInfrastructure.Random.Next(1, cultures.Length));
                    target.UserSettings.AnswerCulture = ai;
                    Assert.AreEqual<CultureInfo>(target.UserSettings.AnswerCulture, ai, "The culture does not match for AnswerCulture.");
                    target.UserSettings.QuestionCulture = qi;
                    Assert.AreEqual<CultureInfo>(target.UserSettings.QuestionCulture, qi, "The culture does not match for QuestionCulture.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Test the words property.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        public void WordsTest()
        {
            //
            // TODO: Add test logic	here
            //
        }

        /// <summary>
        /// Basic tests for CreateWord().
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void CreateWordTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = target.Cards.AddNew();
                    IWord word = card.Question.CreateWord("test", WordType.Word, false);
                    Assert.AreEqual<string>(word.Word, "test", "The values for word do not match.");
                    Assert.AreEqual<WordType>(word.Type, WordType.Word, "The values for word do not match.");
                    Assert.AreEqual<Boolean>(word.Default, false, "The values for word do not match.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests an empty value for CreateWord().
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void CreateEmptyWordTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = target.Cards.AddNew();
                    IWord word = card.Question.CreateWord(String.Empty, WordType.Word, false);
                    Assert.AreEqual<string>(word.Word, String.Empty, "The value for word should be empty.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests an empty value for CreateWord().
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void CreateNullWordTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = target.Cards.AddNew();
                    IWord word = card.Question.CreateWord(null, WordType.Word, false);
                    Assert.AreEqual<string>(word.Word, String.Empty, "The value for word should be empty.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Basic tests for AddWord().
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = target.Cards.AddNew();
                    IWord word = card.Question.CreateWord("test", WordType.Word, false);
                    card.Question.AddWord(word);
                    Assert.IsTrue(card.Question.Words.Count == 1, "Word count should be 1 but is " + card.Question.Words.Count.ToString());
                    Assert.AreEqual<string>(card.Question.Words[0].Word, "test", "The values for word do not match.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests a null value for AddWord().
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException), "Adding a null value should throw an exception.")]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordNullTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = target.Cards.AddNew();
                    card.Question.AddWord(null);
                }
            }
            else
                throw new System.NullReferenceException();
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Basic tests for AddWords(WordList).
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordsWordListTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "A chapter";

                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;

                    List<string> wordList = new List<string>();
                    for (int i = 0; i < TestInfrastructure.Random.Next(10, 50); i++)
                        wordList.Add("Word " + i.ToString());

                    card.Question.AddWords(wordList.ToArray());

                    for (int i = 0; i < writeLM.Cards.Cards[0].Question.Words.Count; i++)
                        Assert.AreEqual<string>("Word " + i.ToString(), writeLM.Cards.Cards[0].Question.Words[i].Word, "IWords.AddWords(WordList) does not add a list of words.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests AddWords(WordList) with a null WordList.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(System.NullReferenceException), "Adding a null value should throw an exception.")]
        public void AddWordsNullWordListTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    card.Question.AddWords((string[])null);
                }
            }
            else
                throw new System.NullReferenceException();
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests AddWords(WordList) with an empty WordList.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordsEmptyWordListTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "A chapter";

                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;
                    List<string> wordList = new List<string>();

                    card.Question.AddWords(wordList.ToArray());
                    Assert.AreEqual<int>(0, writeLM.Cards.Cards[0].Question.Words.Count, "IWords.AddWords(empty WordList) does something strange.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Basic tests for AddWords(List&lt;IWord&gt;).
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordsListTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "A chapter";

                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;
                    List<IWord> wordList = new List<IWord>();

                    for (int i = 0; i < TestInfrastructure.Random.Next(10, 50); i++)
                    {
                        IWord word = card.Question.CreateWord("Word " + i.ToString(), WordType.Word, true);
                        wordList.Add(word);
                    }

                    card.Question.AddWords(wordList);

                    for (int i = 0; i < wordList.Count; i++)
                        Assert.AreEqual<string>("Word " + i.ToString(), writeLM.Cards.Get(card.Id).Question.Words[i].Word, "IWords.AddWords(List) does not add a list of words.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests AddWords(List&lt;IWord&gt;) with a null List.
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException), "Adding a null value should throw an exception.")]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordsNullListTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    card.Question.AddWords((List<IWord>)null);
                    Assert.AreEqual<int>(0, writeLM.Cards.Cards[0].Question.Words.Count, "IWords.AddWords((List<IWord>)null) does something strange.");
                }
            }
            else
                throw new System.NullReferenceException();
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests AddWords(List&lt;IWord&gt;) with an empty List.
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void AddWordsEmptyListTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "A chapter";

                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;
                    List<IWord> wordList = new List<IWord>();

                    wordList.Clear();
                    card.Question.AddWords(wordList);

                    Assert.AreEqual<int>(0, writeLM.Cards.Cards[0].Question.Words.Count, "IWords.AddWords(empty list) does something strange.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests ClearWords() which should delete all words.
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ClearWordsTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    IWords words = card.Question;

                    for (int i = 0; i < TestInfrastructure.Random.Next(10, 50); i++)
                    {
                        IWord word = words.CreateWord("Word " + i.ToString(), WordType.Word, true);
                        words.AddWord(word);
                    }

                    words.ClearWords();
                    Assert.IsTrue(words.Words.Count == 0, "IWords.ClearWords does not delete all words.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests ToString().
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ToStringTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    IWords words = card.Question;

                    for (int i = 0; i < TestInfrastructure.Random.Next(10, 50); i++)
                    {
                        IWord word = words.CreateWord("Word " + i.ToString(), WordType.Word, true);
                        words.AddWord(word);
                    }

                    string quotedStringClone = string.Empty;
                    foreach (IWord var in words.Words)
                        quotedStringClone += var.Word + ", ";
                    quotedStringClone = quotedStringClone.Substring(0, quotedStringClone.Length - 2);

                    Assert.AreEqual<string>(quotedStringClone, words.ToString(), "IWords.ToString does not match with expected output.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests ToQuotedString().
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ToQuotedStringTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    IWords words = card.Question;

                    for (int i = 0; i < TestInfrastructure.Random.Next(10, 50); i++)
                    {
                        IWord word = words.CreateWord("Word " + i.ToString(), WordType.Word, true);
                        words.AddWord(word);
                    }

                    string quotedStringClone = string.Empty;
                    foreach (IWord var in words.Words)
                        quotedStringClone += "\"" + var.Word + "\", ";
                    quotedStringClone = quotedStringClone.Substring(0, quotedStringClone.Length - 2);

                    Assert.AreEqual<string>(quotedStringClone, words.ToQuotedString(), "IWords.ToQuotedString does not match with expected output.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Tests ToNewlineString().
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-07-28</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ToNewlineStringTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    IWords words = card.Question;

                    for (int i = 0; i < TestInfrastructure.Random.Next(10, 50); i++)
                    {
                        IWord word = words.CreateWord("Word " + i.ToString(), WordType.Word, true);
                        words.AddWord(word);
                    }

                    string newLineStringClone = string.Empty;
                    foreach (IWord var in words.Words)
                        newLineStringClone += var.Word + "\r\n";
                    newLineStringClone = newLineStringClone.Substring(0, newLineStringClone.Length - 2);

                    Assert.AreEqual<string>(newLineStringClone, words.ToNewlineString(), "IWords.ToNewlineStringTest does not match with expected output.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
