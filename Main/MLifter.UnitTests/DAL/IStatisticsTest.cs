using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;
using MLifter.DAL;
using MLifter.DAL.Interfaces.DB;
using System.Threading;

namespace MLifterTest.DAL
{
    [TestClass()]
    public class IStatisticsTest
    {
        private TestContext testContextInstance;

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

        [TestMethod()]
        [TestProperty("DAL", "ChrFin"), DataSource("TestSources")]
        public void IStatisticsTesting()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "File")       //TODO??? Only in case of XML
            {
                int lmid;
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, "IStatisticsTest"))
                {
                    if (target.Parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
                        return;

                    lmid = target.Id;
                    IStatistics statistics = target.Statistics;
                    statistics.Clear();

                    Log.OpenUserSession(target.Id, target.Parent);

                    IStatistic stat = statistics[0];
                    stat.StartTimestamp = new DateTime(2008, 10, 10, 12, 0, 0);
                    stat.Right = 10;
                    stat.Wrong = 12;
                    stat.Boxes[0] = 1;
                    stat.Boxes[1] = 2;
                    stat.Boxes[2] = 3;
                    stat.Boxes[3] = 4;
                    stat.Boxes[4] = 5;
                    stat.Boxes[5] = 6;
                    stat.Boxes[6] = 7;
                    stat.Boxes[7] = 8;
                    stat.Boxes[8] = 9;
                    stat.Boxes[9] = 10;
                    stat.EndTimestamp = new DateTime(2008, 10, 10, 12, 30, 0);

                    target.Save();
                }

                using (IDictionary target = TestInfrastructure.GetPersistentLMConnection(TestContext, "IStatisticsTest", lmid))
                {
                    IStatistics statistics = target.Statistics;
                    Assert.AreEqual<int>(1, statistics.Count, "Statistics Count is wrong!");

                    IStatistic stat = statistics[0];

                    Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10, 12, 0, 0), stat.StartTimestamp, "Start timestamp not saved correctly!");
                    Assert.AreEqual<int>(10, stat.Right, "Right Count not saved correctly!");
                    Assert.AreEqual<int>(12, stat.Wrong, "Wrong Count not saved correctly!");
                    Assert.AreEqual<int>(1, stat.Boxes[0], "Box size not saved correctly!");
                    Assert.AreEqual<int>(2, stat.Boxes[1], "Box size not saved correctly!");
                    Assert.AreEqual<int>(3, stat.Boxes[2], "Box size not saved correctly!");
                    Assert.AreEqual<int>(4, stat.Boxes[3], "Box size not saved correctly!");
                    Assert.AreEqual<int>(5, stat.Boxes[4], "Box size not saved correctly!");
                    Assert.AreEqual<int>(6, stat.Boxes[5], "Box size not saved correctly!");
                    Assert.AreEqual<int>(7, stat.Boxes[6], "Box size not saved correctly!");
                    Assert.AreEqual<int>(8, stat.Boxes[7], "Box size not saved correctly!");
                    Assert.AreEqual<int>(9, stat.Boxes[8], "Box size not saved correctly!");
                    Assert.AreEqual<int>(10, stat.Boxes[9], "Box size not saved correctly!");
                    Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10, 12, 30, 0), stat.EndTimestamp, "End timestamp not saved correctly!");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the statistics log and session test.
        /// </summary>
        /// <remarks>Documented by Dev08, 2008-11-21</remarks>
        [TestMethod()]
        [TestProperty("DAL", "FabThe"), DataSource("TestSources")]
        public void IStatisticsLogAndSessionTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))      //Learning
                {
                    if (target.Parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
                        return;

                    ICardsTests.FillDummyDic(target);
                    Log.OpenUserSession(target.Id, target.Parent);

                    for (int i = 0; i < 10; i++)
                    {
                        LearnLogStruct lls = new LearnLogStruct();
                        lls.SessionID = Log.LastSessionID;
                        lls.CardsID = target.Cards.Cards[i].Id;
                        lls.Answer = "testAnswer " + i;
                        lls.CaseSensitive = i % 2 == 0 ? true : false;
                        lls.CorrectOnTheFly = i % 2 == 0 ? true : false;
                        lls.Direction = EQueryDirection.Mixed;
                        lls.Duration = new TimeSpan(0, 0, 50).Ticks;
                        lls.LearnMode = EQueryType.Word;
                        lls.MoveType = i % 4 == 0 ? MoveType.AutoPromote : MoveType.AutoDemote;

                        if (lls.MoveType == MoveType.AutoPromote)       //promote
                        {
                            if (target.Cards.Cards[i].Box == 10)
                                lls.NewBox = 10;
                            else if (target.Cards.Cards[i].Box == 0)
                                lls.NewBox = 2;
                            else
                                lls.NewBox = target.Cards.Cards[i].Box + 1;
                        }
                        else                                            //demote
                            lls.NewBox = 1;

                        lls.OldBox = target.Cards.Cards[i].Box;
                        lls.PercentageKnown = 100;
                        lls.PercentageRequired = 50;
                        lls.TimeStamp = DateTime.Now;

                        Thread.Sleep(100);

                        Log.CreateLearnLogEntry(lls, target.Parent);
                    }

                    Log.CloseUserSession(target.Parent);

                    Thread.Sleep(250);

                    Assert.AreEqual(1, target.Statistics.Count, "Dictionary.Statistics should contain ONE entry");
                    Assert.AreEqual(3, target.Statistics[0].Right, "Dictionary.Statistics[0].Right should contain the number 3.");
                    Assert.AreEqual(7, target.Statistics[0].Wrong, "Dictionary.Statistics[0].Wrong should contain the number 7.");
                }
            }
        }
    }


}
