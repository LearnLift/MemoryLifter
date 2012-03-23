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
    /// Summary Description for ICard
    /// </summary>
    [TestClass]
    public class ICardTests
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

        public ICardTests()
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


        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetActive()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;

                    card.Active = true;
                    Assert.IsTrue(writeLM.Cards.Cards[0].Active, "ICard.Active does not save the property");

                    card.Active = false;
                    Assert.IsFalse(writeLM.Cards.Cards[0].Active, "ICard.Active does not save the property");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetBox()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();

                    int box = new Random().Next(1, 10);
                    card.Box = box;
                    Assert.AreEqual<int>(box, card.Box, "ICard.Box does not save the property");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(InvalidBoxException), "IdAccessException Exception (or something else) was not thrown.")]
        public void ICardInvalidBox()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card1 = writeLM.Cards.AddNew();
                    ICard card2 = writeLM.Cards.AddNew();

                    card1.Box = -200;
                    card2.Box = 200;
                }
            }
            else
                throw new InvalidBoxException(0);
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetChapter()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + (i + 1);
                    }
                    ICard card = writeLM.Cards.AddNew();
                    IList<IChapter> chapters = writeLM.Chapters.Chapters;
                    int id = chapters[TestInfrastructure.Random.Next(0, chapters.Count)].Id;
                    card.Chapter = id;
                    Assert.AreEqual<int>(id, card.Chapter, "ICard.Chapter does not save the property");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(IdAccessException), "IdAccessException Exception (or something else) was not thrown.")]
        public void ICardUnknownChapter()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                card.Chapter = int.MaxValue;
            }
            else
                throw new IdAccessException(0);
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(IdAccessException), "IdAccessException Exception (or something else) was not thrown.")]
        public void ICardInvalidChapterID()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                card.Chapter = -123;
            }
            else
                throw new IdAccessException(0);
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetTimeStamp()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                DateTime timeStamp = DateTime.Now;
                card.Timestamp = timeStamp;
                Assert.IsTrue(TestInfrastructure.CompareDateTimes(timeStamp, card.Timestamp), "ICard.TimeStamp does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetQuestion()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                string word = Guid.NewGuid().ToString();
                card.Question.AddWord(card.Question.CreateWord(word, WordType.Word, true));
                card.Question.AddWord(card.Question.CreateWord(word + "blub", WordType.Word, true));

                Assert.AreEqual<string>(card.Question.Words[0].Word, word, "ICard.Question (Word) does not save the property");
                Assert.AreEqual<string>(card.Question.Words[1].Word, word + "blub", "ICard.Question (Word) does not save the property");
                Assert.AreEqual<WordType>(card.Question.Words[1].Type, WordType.Word, "ICard.Question (Word)(WordType) does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetQuestionExample()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                string sentence = Guid.NewGuid().ToString();
                card.QuestionExample.AddWord(card.QuestionExample.CreateWord(sentence, WordType.Sentence, true));

                Assert.AreEqual<string>(card.QuestionExample.Words[0].Word, sentence, "ICard.Question (Example) does not save the property");
                Assert.AreEqual<WordType>(card.QuestionExample.Words[0].Type, WordType.Sentence, "ICard.Question (Example)(Wordtype) does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        public void ICardSetGetQuestionStylesheet()
        {
            //
            // TODO: Add test logic	here
            //
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetQuestionDistractors()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                string word = Guid.NewGuid().ToString();
                card.QuestionDistractors.AddWord(card.QuestionDistractors.CreateWord(word, WordType.Distractor, true));
                card.QuestionDistractors.AddWord(card.QuestionDistractors.CreateWord(word + "blub", WordType.Distractor, true));

                Assert.AreEqual<string>(card.QuestionDistractors.Words[0].Word, word, "ICard.Question (Distractor) does not save the property");
                Assert.AreEqual<string>(card.QuestionDistractors.Words[1].Word, word + "blub", "ICard.Question (Distractor) does not save the property");
                Assert.AreEqual<WordType>(card.QuestionDistractors.Words[1].Type, WordType.Distractor, "ICard.Question (Distractor)(WordType) does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetAnswer()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                string word = Guid.NewGuid().ToString();
                card.Answer.AddWord(card.Answer.CreateWord(word, WordType.Word, true));
                card.Answer.AddWord(card.Answer.CreateWord(word + "blub", WordType.Word, true));

                Assert.AreEqual<string>(card.Answer.Words[0].Word, word, "ICard.Answer (Word) does not save the property");
                Assert.AreEqual<string>(card.Answer.Words[1].Word, word + "blub", "ICard.Answer (Word) does not save the property");
                Assert.AreEqual<WordType>(card.Answer.Words[1].Type, WordType.Word, "ICard.Question (Word)(WordType) does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetAnswerExample()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                string sentence = Guid.NewGuid().ToString();
                card.AnswerExample.AddWord(card.AnswerExample.CreateWord(sentence, WordType.Sentence, true));

                Assert.AreEqual<string>(card.AnswerExample.Words[0].Word, sentence, "ICard.Answer (Example) does not save the property");
                Assert.AreEqual<WordType>(card.AnswerExample.Words[0].Type, WordType.Sentence, "ICard.Answer (Example)(Wordtype) does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        public void ICardSetGetAnswerStylesheet()
        {
            //
            // TODO: Add test logic	here
            //
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetAnswerDistractors()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
                ICard card = writeLM.Cards.AddNew();

                string word = Guid.NewGuid().ToString();
                card.AnswerDistractors.AddWord(card.AnswerDistractors.CreateWord(word, WordType.Distractor, true));
                card.AnswerDistractors.AddWord(card.AnswerDistractors.CreateWord(word + "blub", WordType.Distractor, true));

                Assert.AreEqual<string>(card.AnswerDistractors.Words[0].Word, word, "ICard.Answer (Distractorr) does not save the property");
                Assert.AreEqual<string>(card.AnswerDistractors.Words[1].Word, word + "blub", "ICard.Answer (Distractor) does not save the property");
                Assert.AreEqual<WordType>(card.AnswerDistractors.Words[1].Type, WordType.Distractor, "ICard.Answer (Distractor)(WordType) does not save the property");

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        public void ICardSetGetCardStyle()
        {
            //
            // TODO: Add test logic	here
            //
        }

        [TestMethod]
        public void ICardCreateCardStyleTest()
        {
            //
            // TODO: Add test logic	here
            //
        }
    }
}
