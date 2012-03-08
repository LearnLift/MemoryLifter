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
    public class PerformanceMediaTests
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

        public PerformanceMediaTests()
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
        /// Is the card add Media performance test.
        /// </summary>
        /// <remarks>Documented by Dev11, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardAddImagePerformanceTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            IMedia media;
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    Tools.TestStopWatch.Start(TestContext);
                    ICard card = writeLM.Cards.AddNew();
                    string testImage = TestInfrastructure.GetLargeTestImage();
                    media = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (image) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (image) to an AnswerMedia");
                    card.ClearAllMedia();
                    Tools.TestStopWatch.Stop(TestContext);
                    try { File.Delete(testImage); }
                    catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardAddAudioPerformanceTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            IMedia media;
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    Tools.TestStopWatch.Start(TestContext);
                    ICard card = writeLM.Cards.AddNew();
                    string testAudio = TestInfrastructure.GetLargeTestAudio();
                    media = card.CreateMedia(EMedia.Audio, testAudio, true, true, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (audio) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (audio) to an AnswerMedia");
                    card.ClearAllMedia();
                    Tools.TestStopWatch.Stop(TestContext);
                    try { File.Delete(testAudio); }
                    catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardAddVideoPerformanceTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            IMedia media;
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    Tools.TestStopWatch.Start(TestContext);
                    ICard card = writeLM.Cards.AddNew();
                    string testVideo = TestInfrastructure.GetLargeTestVideo();
                    media = card.CreateMedia(EMedia.Video, testVideo, true, true, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (video) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (video) to an AnswerMedia");
                    card.ClearAllMedia();
                    Tools.TestStopWatch.Stop(TestContext);
                    try { File.Delete(testVideo); }
                    catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }



    }
}
