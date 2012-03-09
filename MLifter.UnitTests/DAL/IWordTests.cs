using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Diagnostics;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary Description for IWord
    /// </summary>
    [TestClass]
    public class IWordTests
    {
        public IWordTests()
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
        //
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

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IWordSetGetType()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();

                    IWord question_word = card.Question.CreateWord("content", WordType.Word, true);
                    IWord question_dist = card.QuestionDistractors.CreateWord("content", WordType.Distractor, true);
                    IWord question_example = card.QuestionExample.CreateWord("content", WordType.Sentence, true);

                    card.Question.AddWord(question_word);
                    card.QuestionDistractors.AddWord(question_dist);
                    card.QuestionExample.AddWord(question_example);

                    Assert.AreEqual<WordType>(card.Question.Words[0].Type, WordType.Word, "IWord.Type does not save the property (1)");
                    Assert.AreEqual<WordType>(card.QuestionDistractors.Words[0].Type, WordType.Distractor, "IWord.Type does not save the property (2)");
                    Assert.AreEqual<WordType>(card.QuestionExample.Words[0].Type, WordType.Sentence, "IWord.Type does not save the property (3)");

                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IWordSetGetDefault()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();

                    IWord question1 = card.Question.CreateWord("content", WordType.Word, true);
                    IWord question2 = card.Question.CreateWord("content", WordType.Word, true);
                    question2.Default = false;

                    card.Question.AddWord(question1);
                    card.Question.AddWord(question2);

                    Assert.IsTrue(card.Question.Words[0].Default, "IWord.Default does not save the property (1)");
                    Assert.IsFalse(card.Question.Words[1].Default, "IWord.Default does not save the property (2)");

                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IWordSetGetWord()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();

                    IWord question = card.Question.CreateWord("content", WordType.Word, true);          //"content" becomes overwritten
                    string word = Guid.NewGuid().ToString();
                    question.Word = word;

                    card.Question.AddWord(question);
                    Assert.AreEqual<string>(card.Question.Words[0].Word, word, "IWord.Word does not save the property");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
