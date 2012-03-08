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
    /// Summary Description for ICardMediaTests
    /// </summary>
    [TestClass]
    public class ICardMediaTests
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

        public ICardMediaTests()
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
        /// Is the card set get question Media.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetQuestionMedia()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);

                IChapter chapter = writeLM.Chapters.AddNew();
                ICard card = writeLM.Cards.AddNew();
                card.Chapter = chapter.Id;

                string testImage = TestInfrastructure.GetTestImage();
                IMedia media = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                card.AddMedia(media, Side.Question);
                Assert.IsTrue(writeLM.Cards.Get(card.Id).QuestionMedia[0] != null, "Couldn't get QuestionMediaFile from IDictionary");
                try { File.Delete(testImage); }
                catch { }

                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the card set get answer Media.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardSetGetAnswerMedia()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;

                    string testImage = TestInfrastructure.GetTestImage();
                    IMedia media = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                    card.AddMedia(media, Side.Answer);
                    Assert.IsTrue(writeLM.Cards.Get(card.Id).AnswerMedia[0] != null, "Couldn't get AnswerMediaFile from IDictionary");
                    try { File.Delete(testImage); }
                    catch { }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardAddMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            IMedia media;
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    string testImage = TestInfrastructure.GetTestImage();
                    media = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (image) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (image) to an AnswerMedia");
                    card.ClearAllMedia();
                    card.AddMedia(media, Side.Answer);
                    Assert.AreEqual(1, card.AnswerMedia.Count, "ICard doesn't save an AnswerMedia (image) with the method 'card.AddMedia(Media, Side.Answer)'");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "ICard saves a AnswerMedia (image) to an QuestionMedia");
                    card.ClearAllMedia();

                    string testAudio = TestInfrastructure.GetTestAudio();
                    //With the following parameters, the audiofile is saved 2 times
                    media = card.CreateMedia(EMedia.Audio, testAudio, true, true, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (audio) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (audio) to an AnswerMedia");
                    card.ClearAllMedia();
                    card.AddMedia(media, Side.Answer);
                    Assert.AreEqual(1, card.AnswerMedia.Count, "ICard doesn't save an AnswerMedia (audio) with the method 'card.AddMedia(Media, Side.Answer)'");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "ICard saves a AnswerMedia (audio) to an QuestionMedia");
                    card.ClearAllMedia();

                    media = card.CreateMedia(EMedia.Audio, testAudio, true, false, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (audio) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (audio) to an AnswerMedia");
                    card.ClearAllMedia();
                    card.AddMedia(media, Side.Answer);
                    Assert.AreEqual(1, card.AnswerMedia.Count, "ICard doesn't save an AnswerMedia (audio) with the method 'card.AddMedia(Media, Side.Answer)'");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "ICard saves a AnswerMedia (audio) to an QuestionMedia");
                    card.ClearAllMedia();

                    media = card.CreateMedia(EMedia.Audio, testAudio, true, true, false);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (audio) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (audio) to an AnswerMedia");
                    card.ClearAllMedia();
                    card.AddMedia(media, Side.Answer);
                    Assert.AreEqual(1, card.AnswerMedia.Count, "ICard doesn't save an AnswerMedia (audio) with the method 'card.AddMedia(Media, Side.Answer)'");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "ICard saves a AnswerMedia (audio) to an QuestionMedia");
                    card.ClearAllMedia();

                    media = card.CreateMedia(EMedia.Audio, testAudio, true, false, false);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (audio) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (audio) to an AnswerMedia");
                    card.ClearAllMedia();
                    card.AddMedia(media, Side.Answer);
                    Assert.AreEqual(1, card.AnswerMedia.Count, "ICard doesn't save an AnswerMedia (audio) with the method 'card.AddMedia(Media, Side.Answer)'");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "ICard saves a AnswerMedia (audio) to an QuestionMedia");
                    card.ClearAllMedia();

                    string testVideo = TestInfrastructure.GetTestVideo();
                    media = card.CreateMedia(EMedia.Video, testVideo, true, true, true);
                    card.AddMedia(media, Side.Question);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "ICard doesn't save a QuestionMedia (video) with the method 'card.AddMedia(Media, Side.Question)'");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "ICard saves a QuestionMedia (video) to an AnswerMedia");
                    card.ClearAllMedia();
                    card.AddMedia(media, Side.Answer);
                    Assert.AreEqual(1, card.AnswerMedia.Count, "ICard doesn't save an AnswerMedia (video) with the method 'card.AddMedia(Media, Side.Answer)'");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "ICard saves a AnswerMedia (video) to an QuestionMedia");
                    card.ClearAllMedia();

                    try { File.Delete(testImage); }
                    catch { }
                    try { File.Delete(testAudio); }
                    catch { }
                    try { File.Delete(testVideo); }
                    catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the card add empty Media test.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ICardAddEmptyMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            IMedia media;
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    media = card.CreateMedia(EMedia.Video, String.Empty, true, false, false);
                }
            }
            else
                throw new FileNotFoundException();
        }

        /// <summary>
        /// Is the card add null Media test.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ICardAddNullMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            IMedia media;
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    media = card.CreateMedia(EMedia.Video, null, true, false, false);
                }
            }
            else
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Is the card remove Media test.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardRemoveMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    string testImage = TestInfrastructure.GetTestImage();
                    IMedia mediaImage = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                    card.AddMedia(mediaImage, Side.Question);
                    string testAudio = TestInfrastructure.GetTestAudio();
                    IMedia mediaAudio = card.CreateMedia(EMedia.Audio, testAudio, true, true, true);
                    card.AddMedia(mediaAudio, Side.Answer);
                    card.RemoveMedia(mediaAudio);
                    Assert.AreEqual(1, card.QuestionMedia.Count, "Media was removed at the wrong Place (QuestionSide instead of AnswerSide)");
                    Assert.AreEqual(0, card.AnswerMedia.Count, "Media couldn't be removed from ICard");
                    try { File.Delete(testImage); }
                    catch { }
                    try { File.Delete(testAudio); }
                    catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the card create Media test.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardCreateMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    string testImage = TestInfrastructure.GetTestImage();
                    IMedia mediaImage = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                    Assert.IsInstanceOfType(mediaImage, typeof(IMedia), "MediaInstance isn't from Type IMedia");
                    try { File.Delete(testImage); }
                    catch { }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the card null object Media test.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(System.NullReferenceException), "Object reference not set to an instance of an object..")]
        public void ICardNullObjectMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    card.AddMedia(null, Side.Question);
                }
            }
            else
                throw new NullReferenceException();

            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the card clear all Media test.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardClearAllMediaTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    ICard card = writeLM.Cards.AddNew();
                    string testImage = TestInfrastructure.GetTestImage();
                    IMedia media = card.CreateMedia(EMedia.Image, testImage, true, true, true);

                    for (int i = 0; i < 5; i++)
                    {
                        card.AddMedia(media, Side.Question);
                        card.AddMedia(media, Side.Answer);
                    }
                    card.ClearAllMedia();
                    Assert.AreEqual(0, card.AnswerMedia.Count, "Media couldn't be cleared from ICard");
                    Assert.AreEqual(0, card.QuestionMedia.Count, "Media couldn't be cleared from ICard");
                    try { File.Delete(testImage); }
                    catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
