using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;

namespace MLifterTest.BusinessLayer
{
    internal static class TestInfrastructure
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Gets the loopcount.
        /// </summary>
        /// <value>The loopcount.</value>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static int LoopCount
        {
            get { return 100; }
        }

        /// <summary>
        /// Gets the random.
        /// </summary>
        /// <value>The random.</value>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static Random Random
        {
            get { return random; }
        }

        /// <summary>
        /// Gets a value indicating whether [random bool].
        /// </summary>
        /// <value><c>true</c> if [random bool]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2008-09-12</remarks>
        public static bool RandomBool
        {
            get { return (random.Next(2) == 0); }
        }

        /// <summary>
        /// Mies the class cleanup.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        public static void MyClassCleanup()
        {
            MLifterTest.DAL.TestInfrastructure.cleanupQueue.DoCleanup();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-10</remarks>
        public static Dictionary GetConnection(TestContext testContext)
        {
            return GetConnection(testContext, false);
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="standAlone">if set to <c>true</c> a stand alone user will be created.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-10</remarks>
        public static Dictionary GetConnection(TestContext testContext, bool standAlone)
        {
            MLifterTest.DAL.LMConnectionParameter param = new MLifterTest.DAL.LMConnectionParameter(testContext);
            param.Callback = MLifterTest.DAL.TestInfrastructure.GetAdminUser;
            param.ConnectionType = string.Empty;
            param.IsProtected = false;
            param.LearningModuleId = -1;
            param.Password = string.Empty;
            param.RepositoryName = string.Empty;
            param.standAlone = standAlone;

            IDictionary targetDAL = MLifterTest.DAL.TestInfrastructure.GetLMConnection(param);
            Dictionary target = new Dictionary(targetDAL, null);
            return target;
        }

        /// <summary>
        /// Gets a persistent LM connection according choosen connectionType e.g. file or pgsql.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-26-09</remarks>
        public static Dictionary GetPersistentLMConnection(TestContext testContext, string connectionType)
        {
            return GetPersistentLMConnection(testContext, connectionType, false);
        }

        /// <summary>
        /// Gets a persistent LM connection according chosen connectionType e.g. file or pgsql.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="standAlone">if set to <c>true</c> a stand alone user will be created.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-26-09</remarks>
        public static Dictionary GetPersistentLMConnection(TestContext testContext, string connectionType, bool standAlone)
        {
            MLifterTest.DAL.LMConnectionParameter param = new MLifterTest.DAL.LMConnectionParameter(testContext);
            param.Callback = MLifterTest.DAL.TestInfrastructure.GetAdminUser;
            param.ConnectionType = connectionType;
            param.IsProtected = false;
            param.LearningModuleId = -1;
            param.Password = string.Empty;
            param.RepositoryName = string.Empty;
            param.standAlone = standAlone;

            IDictionary targetDAL = MLifterTest.DAL.TestInfrastructure.GetLMConnection(param);
            Dictionary target = new Dictionary(targetDAL, null);
            return target;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="testContex">The test contex.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-30</remarks>
        public static ConnectionStringStruct GetConnectionString(TestContext testContex)
        {
            using (Dictionary dictionary = TestInfrastructure.GetConnection(testContex))
            {
                dictionary.Save();
                return dictionary.DictionaryDAL.Parent.CurrentUser.ConnectionString;
            }
        }

        /// <summary>
        /// Gets the connection string for LM with dummy data.
        /// </summary>
        /// <param name="testContex">The test contex.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-30</remarks>
        public static ConnectionStringStruct GetConnectionStringWithDummyData(TestContext testContext)
        {
            using (Dictionary dictionary = TestInfrastructure.GetConnection(testContext))
            {
                DAL.ICardsTests.FillDummyDic(dictionary.DictionaryDAL);
                dictionary.Save();
                MLifter.DAL.Log.CloseUserSession(dictionary.DictionaryDAL.Parent);

                return dictionary.DictionaryDAL.Parent.CurrentUser.ConnectionString;
            }
        }

        /// <summary>
        /// Determines whether the specified test context is active.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns>
        /// 	<c>true</c> if the specified test context is active; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-09-10</remarks>
        public static bool IsActive(TestContext testContext)
        {
            return MLifterTest.DAL.TestInfrastructure.IsActive(testContext);
        }

        /// <summary>
        /// Get the Connection Type
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-26-09</remarks>
        public static string ConnectionType(TestContext testContext)
        {
            return MLifterTest.DAL.TestInfrastructure.ConnectionType(testContext);
        }

        /// <summary>
        /// Gets the test audio.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetTestAudio()
        {
            return MLifterTest.DAL.TestInfrastructure.GetTestAudio();
        }

        /// <summary>
        /// Gets the test image.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetTestImage()
        {
            return MLifterTest.DAL.TestInfrastructure.GetTestImage();
        }

        /// <summary>
        /// Gets the test video.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetTestVideo()
        {
            return MLifterTest.DAL.TestInfrastructure.GetTestVideo();
        }

        public static void FillDummyDic(Dictionary dictionary)
        {
            List<int> chapters = new List<int>();
            for (int k = 0; k < 10; k++)
            {
                int chapterId = dictionary.Chapters.AddChapter("Chapter " + Convert.ToString(k + 1), "Chapter Description" + Convert.ToString(k + 1));
                chapters.Add(chapterId);
                dictionary.QueryChapters.Add(chapterId);
            }
            for (int i = 0; i < TestInfrastructure.LoopCount; i++)
            {
                bool hasQImage = TestInfrastructure.RandomBool, hasAImage = TestInfrastructure.RandomBool;
                bool hasQAudio = TestInfrastructure.RandomBool, hasAAudio = TestInfrastructure.RandomBool;
                bool hasQEAudio = TestInfrastructure.RandomBool, hasAEAudio = TestInfrastructure.RandomBool;
                bool hasQVideo = TestInfrastructure.RandomBool, hasAVideo = TestInfrastructure.RandomBool;
                bool hasQExample = TestInfrastructure.RandomBool, hasAExample = TestInfrastructure.RandomBool;
                int cardId = dictionary.Cards.AddCard("question " + i, "answer " + i, (hasQExample) ? "question example " + i : String.Empty, (hasAExample) ? "answer example " + i : String.Empty, String.Empty, String.Empty, chapters[TestInfrastructure.Random.Next(0, chapters.Count)]);
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
            }
        }
    }
}
