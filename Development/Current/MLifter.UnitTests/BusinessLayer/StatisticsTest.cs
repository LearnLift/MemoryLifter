using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using System.Threading;
using MLifter.DAL;
using MLifterTest.DAL;

namespace MLifterTest.BusinessLayer
{
    /// <summary>
    /// Summary Description for StatisticsText
    /// </summary>
    [TestClass]
    public class StatisticsTest
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

        public StatisticsTest()
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
        // [ClassCleanup()]
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
        /// Statistics test. (currently only for XML)
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-10-27</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void DeleteAllStatisticsTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                ConnectionStringStruct connectionString = TestInfrastructure.GetConnectionStringWithDummyData(TestContext);
                LearnLogic learnLogic = new LearnLogic(OpenUserProfileTests.GetUserAdmin, (DataAccessErrorDelegate)delegate { return; });
                try
                {
                    learnLogic.OpenLearningModule(new LearningModulesIndexEntry(connectionString));
                }
                catch (IsOdxFormatException)
                {
                    if (TestInfrastructure.ConnectionType(TestContext) == "File")
                        return;
                    else
                        throw;
                }

                learnLogic.OnLearningModuleOptionsChanged();
                learnLogic.ResetLearningProgress();

                LearnStats learnStats = learnLogic.Dictionary.Statistics.GetCurrentStats();
                Assert.AreEqual<int>(0, learnStats.NumberOfRights, "DeleteAllStatistics() did not delete all Stats!");
                Assert.AreEqual<int>(0, learnStats.NumberOfWrongs, "DeleteAllStatistics() did not delete all Stats!");
                Assert.IsTrue(learnLogic.Dictionary.Statistics.GetNewestStatistic().StartTimestamp == learnLogic.Dictionary.Statistics.GetOldestStatistic().StartTimestamp, "GetNewestStatistic() and GetOldestStatistic seems to be not equal. There should be only ONE stat in the restarted LM");
            }
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void GetBoxContentTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                ConnectionStringStruct connectionString = TestInfrastructure.GetConnectionStringWithDummyData(TestContext);
                LearnLogic learnLogic = new LearnLogic(OpenUserProfileTests.GetUserAdmin, (DataAccessErrorDelegate)delegate { return; });

                //learnLogic.User.Authenticate((GetLoginInformation)MLifterTest.DAL.TestInfrastructure.GetTestUser,
                //    connectionString, (DataAccessErrorDelegate)delegate { return; });
                try
                {
                    learnLogic.OpenLearningModule(new LearningModulesIndexEntry(connectionString));
                }
                catch (IsOdxFormatException)
                {
                    if (TestInfrastructure.ConnectionType(TestContext) == "File")
                        return;
                    else
                        throw;
                }

                learnLogic.OnLearningModuleOptionsChanged();
                learnLogic.ResetLearningProgress();

                Card currentCard = learnLogic.Dictionary.Cards.GetCardByID(learnLogic.CurrentCardID);

                //Answer Card correct
                UserInputSubmitEventArgs e = new UserInputSubmitTextEventArgs(0, currentCard.CurrentAnswer.Words.Count,
                    currentCard.CurrentAnswer.Words.Count, true, currentCard.CurrentAnswer.Words.ToString());
                learnLogic.OnUserInputSubmit(this, e);
                learnLogic.OnUserInputSubmit(this, new UserInputSubmitEventArgs());
                Assert.AreEqual<int>(1, learnLogic.Dictionary.Statistics.GetBoxContent(2, DateTime.Now.AddDays(1)), "GetBoxContent() did not return the correct number of cards");
                for (int i = 1; i < 10; i++)
                {
                    if (i == 2)
                        continue;
                    else
                        Assert.AreEqual<int>(0, learnLogic.Dictionary.Statistics.GetBoxContent(i, DateTime.Now.AddDays(1)), "GetBoxContent() says the Box " + i + " is NOT 0 (although it should)");
                }
            }
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void GetKnownTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                ConnectionStringStruct connectionString = TestInfrastructure.GetConnectionStringWithDummyData(TestContext);
                LearnLogic learnLogic = new LearnLogic(OpenUserProfileTests.GetUserAdmin, (DataAccessErrorDelegate)delegate { return; });
                //learnLogic.User.Authenticate((GetLoginInformation)MLifterTest.DAL.TestInfrastructure.GetTestUser,
                //    connectionString, (DataAccessErrorDelegate)delegate { return; });
                try
                {
                    learnLogic.OpenLearningModule(new LearningModulesIndexEntry(connectionString));
                }
                catch (IsOdxFormatException)
                {
                    if (TestInfrastructure.ConnectionType(TestContext) == "File")
                        return;
                    else
                        throw;
                }

                learnLogic.OnLearningModuleOptionsChanged();
                learnLogic.ResetLearningProgress();

                //Answer 5 cards correct
                for (int i = 0; i < 5; i++)
                {
                    Card currentCard = learnLogic.Dictionary.Cards.GetCardByID(learnLogic.CurrentCardID);

                    //Answer Card correct
                    UserInputSubmitEventArgs e = new UserInputSubmitTextEventArgs(0, currentCard.CurrentAnswer.Words.Count,
                        currentCard.CurrentAnswer.Words.Count, true, currentCard.CurrentAnswer.Words.ToString());

                    learnLogic.OnUserInputSubmit(this, e);
                    learnLogic.OnUserInputSubmit(this, new UserInputSubmitEventArgs());
                }

                int known = learnLogic.Dictionary.Statistics.GetKnown(DateTime.Now.AddDays(1));
                Assert.AreEqual<int>(5, known, "GetKnown() did not return the correct number of known cards");
            }
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void GetCurrentStatsTest()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                ConnectionStringStruct connectionString = TestInfrastructure.GetConnectionStringWithDummyData(TestContext);
                LearnLogic learnLogic = new LearnLogic(OpenUserProfileTests.GetUserAdmin, (DataAccessErrorDelegate)delegate { return; });
                try
                {
                    learnLogic.OpenLearningModule(new LearningModulesIndexEntry(connectionString));
                }
                catch (IsOdxFormatException)
                {
                    if (TestInfrastructure.ConnectionType(TestContext) == "File")
                        return;
                    else
                        throw;
                }

                learnLogic.OnLearningModuleOptionsChanged();
                learnLogic.Dictionary.ResetLearningProgress();

                //Answer Card correct
                Card currentCard = learnLogic.Dictionary.Cards.GetCardByID(learnLogic.CurrentCardID);
                UserInputSubmitEventArgs e = new UserInputSubmitTextEventArgs(0, currentCard.CurrentAnswer.Words.Count,
                    currentCard.CurrentAnswer.Words.Count, true, currentCard.CurrentAnswer.Words.ToString());
                learnLogic.OnUserInputSubmit(this, e);
                learnLogic.OnUserInputSubmit(this, new UserInputSubmitEventArgs());

                //Answer Card wrong
                currentCard = learnLogic.Dictionary.Cards.GetCardByID(learnLogic.CurrentCardID);
                e = new UserInputSubmitTextEventArgs(0, 0, currentCard.CurrentAnswer.Words.Count, false, string.Empty);
                learnLogic.OnUserInputSubmit(this, e);
                learnLogic.OnUserInputSubmit(this, new UserInputSubmitEventArgs());

                LearnStats stats = learnLogic.Dictionary.Statistics.GetCurrentStats();
                Assert.AreEqual<int>(1, stats.NumberOfRights, "GetCurrentStats() did not return the correct LearnStats.NumberOfRights");
                Assert.AreEqual<int>(1, stats.NumberOfWrongs, "GetCurrentStats() did not return the correct LearnStats.NumberOfWrongs");
            }
        }
    }
}
