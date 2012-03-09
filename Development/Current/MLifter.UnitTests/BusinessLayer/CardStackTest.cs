﻿// The following code was generated by Microsoft Visual Studio 2005.
// The generateQuestionThread owner should check each generateQuestionThread for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using System.Threading;
namespace MLifterTest.BusinessLayer
{
    /// <summary>
    ///This is a generateQuestionThread class for MLifter.BusinessLayer.CardStack and is intended
    ///to contain all MLifter.BusinessLayer.CardStack Unit Tests
    ///</summary>
    [TestClass()]
    public class CardStackTest
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
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first generateQuestionThread in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestInfrastructure.MyClassCleanup();
        }
        //
        //Use TestInitialize to run code before running each generateQuestionThread
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each generateQuestionThread has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private static Random random
        {
            get { return DictionaryTest.Random; }
        }

        private static bool GetRandBool()
        {
            return DictionaryTest.GetRandBool();
        }

        /// <summary>
        /// A test for the CardStack
        /// </summary>
        [TestMethod()]
        [TestProperty("BL", "DanAch"), DataSource("TestSources")]
        public void CardStackUnitTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (Dictionary dictionary = TestInfrastructure.GetConnection(TestContext))
                {
                    DictionaryTest.FillDictionary(dictionary);

                    CardStack target = new CardStack(new LearnLogic(MLifterTest.DAL.OpenUserProfileTests.GetUserAdmin, (MLifter.DAL.DataAccessErrorDelegate)delegate { return; }));
                    target.StackChanged += new EventHandler(CardStack_StackChanged);
                    cardstackChanged = false;
                    target.Clear();
                    Assert.IsTrue(cardstackChanged, "CardStackChanged event did not fire for Clear.");

                    int cardcount = 0;
                    int rightcount = 0;
                    TimeSpan duration = new TimeSpan(0);

                    foreach (ICard card in dictionary.Cards.Cards)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            cardcount++;
                            bool promoted = GetRandBool();
                            if (promoted)
                                rightcount++;
                            DateTime asked = DateTime.Now - new TimeSpan(0, 0, random.Next(1000));
                            DateTime answered = DateTime.Now;
                            duration += (answered - asked);
                            AnswerResult result = promoted ? AnswerResult.Correct : AnswerResult.Wrong;
                            StackCard stackCard = new StackCard(card, result, promoted, EQueryDirection.Question2Answer, LearnModes.Word, asked, answered, Thread.CurrentThread.CurrentCulture, Thread.CurrentThread.CurrentCulture, card.Answer.ToString(), 0, 0, false, dictionary, 0);

                            cardstackChanged = false;
                            target.Push(stackCard);
                            Assert.IsTrue(cardstackChanged, "CardStackChanged event did not fire for Push.");
                        }
                    }

                    Assert.AreEqual(cardcount, target.Count, "Not all cards were added to the stack.");
                    Assert.AreEqual(duration, target.SessionDuration, "Session duration sum does not match.");
                    Assert.AreEqual(rightcount, target.RightCount, "RightCount does not match.");
                    Assert.AreEqual(cardcount - rightcount, target.WrongCount, "WrongCount does not match.");
                    Assert.IsTrue(rightcount > 0 && target.VisibleStack.Count > 0, "No StackCards in VisibleStack property.");
                }
            }
        }

        private bool cardstackChanged = false;

        void CardStack_StackChanged(object sender, EventArgs e)
        {
            cardstackChanged = true;
        }

    }


}
