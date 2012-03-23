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

using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifterTest.DAL;

namespace MLifterTest.BusinessLayer
{
    /// <summary>
    /// Summary Description for CardTypeRecognition
    /// </summary>
    [TestClass]
    public class CardTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the generateQuestionThread context which provides
        ///information about and functionality for the current generateQuestionThread run.
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
        public CardTests()
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

        /// <summary>
        /// Test for the Card.ContainsMedia methods.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-09-12</remarks>
        [TestMethod]
        [TestProperty("BL", "AleAbe"), DataSource("TestSources")]
        public void CardContainsMediaTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (Dictionary dictionary = TestInfrastructure.GetConnection(TestContext))
                {
                    int chapterId = dictionary.Chapters.AddChapter("test chapter", "test chapter Description");
                    for (int i = 0; i < TestInfrastructure.LoopCount; i++)
                    {
                        bool hasQImage = TestInfrastructure.RandomBool, hasAImage = TestInfrastructure.RandomBool;
                        bool hasQAudio = TestInfrastructure.RandomBool, hasAAudio = TestInfrastructure.RandomBool;
                        bool hasQEAudio = TestInfrastructure.RandomBool, hasAEAudio = TestInfrastructure.RandomBool;
                        bool hasQVideo = TestInfrastructure.RandomBool, hasAVideo = TestInfrastructure.RandomBool;
                        int cardId = dictionary.Cards.AddCard("question", "answer", "question example", "answer example", String.Empty, String.Empty, chapterId);
                        if (hasQImage)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestImage(), EMedia.Image, Side.Question, true, true, false);
                        if (hasAImage)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestImage(), EMedia.Image, Side.Answer, true, true, false);
                        if (hasQAudio)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestAudio(), EMedia.Audio, Side.Question, true, true, false);
                        if (hasAAudio)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestAudio(), EMedia.Audio, Side.Answer, true, true, false);
                        if (hasQEAudio)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestAudio(), EMedia.Audio, Side.Question, true, false, true);
                        if (hasAEAudio)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestAudio(), EMedia.Audio, Side.Answer, true, false, true);
                        if (hasQVideo)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestVideo(), EMedia.Video, Side.Question, true, true, false);
                        if (hasAVideo)
                            dictionary.Cards.AddMedia(cardId, TestInfrastructure.GetTestVideo(), EMedia.Video, Side.Answer, true, true, false);
                        Card card = dictionary.Cards.GetCardByID(cardId);
                        if (hasQImage)
                            Assert.IsTrue(card.ContainsImage(Side.Question.ToString()), "Card does not contain question image!");
                        else
                            Assert.IsFalse(card.ContainsImage(Side.Question.ToString()), "Card contains question image!");
                        if (hasAImage)
                            Assert.IsTrue(card.ContainsImage(Side.Answer.ToString()), "Card does not contain answer image!");
                        else
                            Assert.IsFalse(card.ContainsImage(Side.Answer.ToString()), "Card contains answer image!");
                        if (hasQAudio)
                            Assert.IsTrue(card.ContainsAudio(Side.Question.ToString()), "Card does not contain question audio!");
                        else
                            Assert.IsFalse(card.ContainsAudio(Side.Question.ToString()), "Card contains question audio!");
                        if (hasAAudio)
                            Assert.IsTrue(card.ContainsAudio(Side.Answer.ToString()), "Card does not contain answer audio!");
                        else
                            Assert.IsFalse(card.ContainsAudio(Side.Answer.ToString()), "Card contains answer audio!");
                        if (hasQEAudio)
                            Assert.IsTrue(card.ContainsExampleAudio(Side.Question.ToString()), "Card does not contain question example audio!");
                        else
                            Assert.IsFalse(card.ContainsExampleAudio(Side.Question.ToString()), "Card contains question example audio!");
                        if (hasAEAudio)
                            Assert.IsTrue(card.ContainsExampleAudio(Side.Answer.ToString()), "Card does not contain answer example audio!");
                        else
                            Assert.IsFalse(card.ContainsExampleAudio(Side.Answer.ToString()), "Card contains answer example audio!");
                        if (hasQVideo)
                            Assert.IsTrue(card.ContainsVideo(Side.Question.ToString()), "Card does not contain question video!");
                        else
                            Assert.IsFalse(card.ContainsVideo(Side.Question.ToString()), "Card contains question video!");
                        if (hasAVideo)
                            Assert.IsTrue(card.ContainsVideo(Side.Answer.ToString()), "Card does not contain answer video!");
                        else
                            Assert.IsFalse(card.ContainsVideo(Side.Answer.ToString()), "Card contains answer video!");
                    }
                }
            }
        }
    }
}
