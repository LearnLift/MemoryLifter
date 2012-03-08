using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifterTest.BusinessLayer;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Diagnostics;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary Description for ICards
    /// </summary>
    [TestClass]
    public class ICardsTests
    {
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

        public ICardsTests()
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
        public static void MyClassCleanup()
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

        /// <summary>
        /// Test method which tests ICards.Create.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsCreateTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = target.Cards.Create();
                    Assert.IsNotNull(card, "Created card was null.");
                    Assert.IsInstanceOfType(card, typeof(ICard), "Card was not of type ICard.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.Add if .
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsAddTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = target.Chapters.AddNew();

                    ICard card = target.Cards.Create();
                    card.Chapter = chapter.Id;
                    Assert.IsNotNull(card, "Created card was null.");
                    Assert.IsInstanceOfType(card, typeof(ICard), "Card was not of type ICard.");
                    int before = target.Cards.Count;
                    target.Cards.Add(card);
                    Assert.IsTrue(target.Cards.Count == before + 1, "Card count should be " + (before + 1) + " but is " + target.Cards.Count.ToString());
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.AddNew.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsAddNewTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    int before = target.Cards.Count;
                    IChapter chapter = target.Chapters.AddNew();

                    ICard card = target.Cards.AddNew();
                    card.Chapter = chapter.Id;
                    Assert.IsTrue(target.Cards.Count == before + 1, "Card count should be " + (before + 1) + " but is " + target.Cards.Count.ToString());
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.CopyCard which copies a card from one learning module to another.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsCopyCardTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser, true))
                {
                    IChapter chapter = target.Chapters.AddNew();
                    ICard card = target.Cards.AddNew();
                    card.Chapter = chapter.Id;

                    LMConnectionParameter param = new LMConnectionParameter(TestContext);
                    param.Callback = TestInfrastructure.GetAdminUser;
                    param.ConnectionType = string.Empty;
                    param.IsProtected = false;
                    param.LearningModuleId = -1;
                    param.Password = string.Empty;
                    param.RepositoryName = string.Empty;
                    param.standAlone = true;

                    using (IDictionary target2 = TestInfrastructure.GetLMConnection(param))
                    {
                        //IChapter chapter2 = target2.Chapters.AddNew();
                        //card.Chapter = chapter2.Id;

                        int before = target2.Cards.Count;
                        int id = target2.Cards.CopyCard(card);

                        Assert.IsTrue(target2.Cards.Count == before + 1, "Card count should be " + (before + 1) + " but is " + target2.Cards.Count.ToString());
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.LoadCardFromXml.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsLoadCardFromXmlTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            string xmlCard = @"<card id=""0"">
                                <answer>""die F�higkeit"", ""das Geschick""</answer>
                                <answerexample>Manche Schildkr�ten haben die F�higkeit, an Land und unter Wasser zu leben.</answerexample>
                                <answerstylesheet />
                                <box>0</box>
                                <chapter>1</chapter>
                                <question>the ability</question>
                                <questionexample>Some turtles have the ability to live on land and under water.</questionexample>
                                <questionstylesheet />
                                <timestamp>2008-02-20T18:19:53.9975693+01:00</timestamp>
                                <answerdistractors />
                                <questiondistractors />
                                <questionimage>Media\EN_theability_IMG_00.jpg</questionimage>
                                <answerimage>Media\EN_theability_IMG_00.jpg</answerimage>
                                <answeraudio id=""std"">Media\DE_dief�higkeitdas_AUD.mp3</answeraudio>
                                <answerexampleaudio>Media\DE_dief�higkeitdas_example_AUD.mp3</answerexampleaudio>
                                <questionaudio id=""std"">Media\EN_theability_AUD.mp3</questionaudio>
                                <questionexampleaudio>Media\EN_theability_example_AUD.mp3</questionexampleaudio>
                               </card>";

            //This test is only implemented for File
            if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "File")
            {
				using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
                {
                    ICard card = target.Cards.LoadCardFromXml(xmlCard);
                    Assert.IsNotNull(card, "Created card was null.");
                    Assert.IsInstanceOfType(card, typeof(ICard), "Card was not of type ICard.");
                    int before = target.Cards.Count;
                    target.Cards.Add(card);
                    Assert.IsTrue(target.Cards.Count == before + 1, "Card count should be " + (before + 1) + " but is " + target.Cards.Count.ToString());
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.Delete.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsDeleteTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = target.Chapters.AddNew();

                    ICard card = target.Cards.AddNew();
                    card.Chapter = chapter.Id;
                    int before = target.Cards.Count;
                    target.Cards.Delete(card.Id);
                    Assert.IsTrue(target.Cards.Count == before - 1, "Card count should be " + (before - 1) + " but is " + target.Cards.Count.ToString());
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.Get.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsGetTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = target.Chapters.AddNew();
                    List<int> cardIds = new List<int>();
                    for (int i = 0; i < TestInfrastructure.Loopcount; i++)
                    {
                        ICard card = target.Cards.AddNew();
                        card.Chapter = chapter.Id;
                        cardIds.Add(card.Id);
                    }
                    for (int i = 0; i < TestInfrastructure.Loopcount; i++)
                    {
                        int nextId = cardIds[TestInfrastructure.Random.Next(0, cardIds.Count)];
                        ICard card = target.Cards.Get(nextId);
                        Assert.IsNotNull(card, "Card Id could not be found.");
                        Assert.IsInstanceOfType(card, typeof(ICard), "Card was not of type ICard.");
                        Assert.IsTrue(card.Id == nextId, "Card Id should be " + nextId + " but is " + card.Id);
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.GetCards filtering for active cards.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsGetCardsActiveTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    FillDummyDic(target);
                    List<ICard> cards = target.Cards.GetCards(new QueryStruct[] { new QueryStruct(QueryCardState.Active) }, QueryOrder.None, QueryOrderDir.Ascending, -1);
                    foreach (ICard card in cards)
                    {
                        Assert.IsTrue(card.Active, "Card should be active.");
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.GetCards filtering for inactive cards.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsGetCardsInActiveTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    FillDummyDic(target);
                    List<ICard> cards = target.Cards.GetCards(new QueryStruct[] { new QueryStruct(QueryCardState.Inactive) }, QueryOrder.None, QueryOrderDir.Ascending, -1);
                    foreach (ICard card in cards)
                    {
                        Assert.IsTrue(card.Active == false, "Card should be inactive.");
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.GetCards filtering for boxes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsGetCardsBoxTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    FillDummyDic(target);
                    for (int i = 0; i <= 10; i++)
                    {
                        List<ICard> cards = target.Cards.GetCards(new QueryStruct[] { new QueryStruct(-1, i) }, QueryOrder.None, QueryOrderDir.Ascending, -1);
                        foreach (ICard card in cards)
                        {
                            Assert.IsTrue(card.Box == i, "Box should be " + i.ToString() + " but is " + card.Box + ".");
                        }
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
        /// <summary>
        /// Test method which tests ICards.GetCards filtering for chapters.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-07-25</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardsTestsGetCardsChapterTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    List<int> chapters = FillDummyDic(target);
                    for (int i = 0; i < chapters.Count; i++)
                    {
                        List<ICard> cards = target.Cards.GetCards(new QueryStruct[] { new QueryStruct(chapters[i], -1) }, QueryOrder.None, QueryOrderDir.Ascending, -1);
                        foreach (ICard card in cards)
                        {
                            Assert.IsTrue(card.Chapter == chapters[i], "Chapter should be " + chapters[i].ToString() + " but is " + card.Chapter + ".");
                        }
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        public static List<int> FillDummyDic(IDictionary target)
        {
            Debug.WriteLine(TestInfrastructure.beginLine + " Now filling Demodictionary " + TestInfrastructure.beginLine);
            List<int> chapters = new List<int>();
            for (int k = 0; k < 10; k++)
            {
                IChapter chapter = target.Chapters.AddNew();
                chapter.Title = "Chapter " + Convert.ToString(k + 1);
                chapters.Add(chapter.Id);

                target.DefaultSettings.SelectedLearnChapters.Add(chapter.Id);
                target.UserSettings.SelectedLearnChapters.Add(chapter.Id);
            }
            for (int k = 0; k < TestInfrastructure.Loopcount; k++)
            {
                ICard card = target.Cards.AddNew();
                card.Chapter = chapters[TestInfrastructure.Random.Next(0, chapters.Count)];
                int box = TestInfrastructure.Random.Next(0, 10);
                card.Box = (box == 11) ? -1 : box;
                List<IWord> questionWords = new List<IWord>();
                List<IWord> questionDistractors = new List<IWord>();
                List<IWord> questionExamples = new List<IWord>();
                List<IWord> answerWords = new List<IWord>();
                List<IWord> answerDistractors = new List<IWord>();
                List<IWord> answerExamples = new List<IWord>();
                IWords words = card.Question;
                for (int i = 0; i < TestInfrastructure.Random.Next(1, 5); i++)
                {
                    IWord word = words.CreateWord("Question_" + Convert.ToString(k + 1) + "_" + Convert.ToString(i + 1), WordType.Word, (i == 0));
                    questionWords.Add(word);
                }
                for (int i = 0; i < TestInfrastructure.Random.Next(1, 5); i++)
                {
                    IWord word = words.CreateWord("QuestionDistractor_" + Convert.ToString(k + 1) + "_" + Convert.ToString(i + 1), WordType.Distractor, false);
                    questionDistractors.Add(word);
                }
                for (int i = 0; i < TestInfrastructure.Random.Next(1, 5); i++)
                {
                    IWord word = words.CreateWord("QuestionExample_" + Convert.ToString(k + 1) + "_" + Convert.ToString(i + 1), WordType.Sentence, false);
                    questionExamples.Add(word);
                }
                words = card.Answer;
                for (int i = 0; i < TestInfrastructure.Random.Next(1, 5); i++)
                {
                    IWord word = words.CreateWord("Answer_" + Convert.ToString(k + 1) + "_" + Convert.ToString(i + 1), WordType.Word, (i == 0));
                    answerWords.Add(word);
                }
                for (int i = 0; i < TestInfrastructure.Random.Next(1, 5); i++)
                {
                    IWord word = words.CreateWord("AnswerDistractor_" + Convert.ToString(k + 1) + "_" + Convert.ToString(i + 1), WordType.Distractor, false);
                    answerDistractors.Add(word);
                }
                for (int i = 0; i < TestInfrastructure.Random.Next(1, 5); i++)
                {
                    IWord word = words.CreateWord("AnswerExample_" + Convert.ToString(k + 1) + "_" + Convert.ToString(i + 1), WordType.Sentence, false);
                    answerExamples.Add(word);
                }
                card.Question.AddWords(questionWords);
                card.QuestionDistractors.AddWords(questionDistractors);
                card.QuestionExample.AddWords(questionExamples);
                card.Answer.AddWords(answerWords);
                card.AnswerDistractors.AddWords(answerDistractors);
                card.AnswerExample.AddWords(answerExamples);
            }

            Debug.WriteLine(TestInfrastructure.endLine + " Demodictionary filled " + TestInfrastructure.endLine);
            return chapters;
        }


        /// <summary>
        ///A test for ClearAllBoxes ()
        ///</summary>
        [TestMethod()]
        [TestProperty("DL", "DanAch"), DataSource("TestSources")]
        public void ICardsTestsClearAllBoxesTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
				using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
                {
                    target.Cards.ClearAllBoxes();

                    foreach (ICard card in target.Cards.Cards)
                    {
                        Assert.AreEqual(0, card.Box, "Not all cards are in box 0.");
                    }

                    foreach (IBox box in target.Boxes.Box)
                    {
                        if (box.Id == 0)
                            Assert.AreEqual(target.Cards.Cards.Count, box.CurrentSize, "The size of the pool is not equal to the amount of cards.");
                        else
                            Assert.AreEqual(0, box.Size, "Not all boxes are of size 0.");
                    }
                }
            }
        }

    }
}
