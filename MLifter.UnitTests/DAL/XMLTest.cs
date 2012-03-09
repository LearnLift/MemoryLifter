using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using MLifterTest.BusinessLayer;
using System.IO;
using System.Reflection;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;

namespace MLifterTest.DAL
{
    /// <summary>
    ///This is a test class for MLifter.DAL.XML.XmlQueryOptions and is intended
    ///to contain all MLifter.DAL.XML.XmlQueryOptions Unit Tests
    ///</summary>
    [TestClass()]
    public class XmlTest
    {
        private TestContext testContextInstance;
        private static IUser user = null;

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

        public static IUser User
        {
            get
            {
                return user;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DictionaryTest.MyClassCleanup(); //in case the LM is still open
            DictionaryTest.ExtractTestDictionary();
            user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                new ConnectionStringStruct(DatabaseType.Xml, DictionaryTest.testDic, false), (DataAccessErrorDelegate)delegate { return; }, testContext);
            Assert.IsTrue(File.Exists(DictionaryTest.testDic), "Test Learning Module file cannot be found.");
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
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

        private readonly int loopcount = 100;

        /// <summary>
        /// Tests the query options.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        [TestMethod()]
        public void QueryOptionsTest()
        {
            Dictionary<string, object> values;
            using (IDictionary target = user.Open())
            {
                values = GetInitialValues(target.DefaultSettings);
            }

            for (int i = 0; i < loopcount; i++)
            {
                using (IDictionary target = user.Open())
                {
                    CheckAndAssign(target.DefaultSettings, values);
                    target.Save();
                }
            }
        }

        /// <summary>
        /// Checks the property values of the target object for changes, and assigns new random values.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="values">The values.</param>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        private static void CheckAndAssign(object target, Dictionary<string, object> values)
        {
            //check if values are equal and assign new random values
            foreach (PropertyInfo info in target.GetType().GetProperties())
            {
                if (values.ContainsKey(info.Name))
                {
                    Assert.AreEqual(values[info.Name], info.GetValue(target, null), info.Name + " returned an other value than the set one.");
                    object newvalue = null;
                    if (info.PropertyType == typeof(bool))
                        newvalue = GetRandBool();
                    else if (info.PropertyType.IsEnum)
                    {
                        Type enumType = info.PropertyType;
                        newvalue = Enum.GetValues(enumType).GetValue(random.Next(Enum.GetValues(enumType).Length));
                    }
                    else if (info.PropertyType == typeof(int))
                        newvalue = random.Next(99) + 1; //exception for MultipleChoice - AnswerCount

                    info.SetValue(target, newvalue, null);
                    values[info.Name] = newvalue;
                }
            }
        }

        /// <summary>
        /// Gets the initial property values of the target object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        private static Dictionary<string, object> GetInitialValues(object target)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (PropertyInfo info in target.GetType().GetProperties())
            {
                if (info.CanRead && info.CanWrite && (info.PropertyType == typeof(bool) || info.PropertyType.IsEnum || info.PropertyType == typeof(int))
                    && info.PropertyType != typeof(ESnoozeMode)) //special exception for ESnoozeMode: not all values are allowed
                {
                    Assert.IsFalse(values.ContainsKey(info.Name), "Duplicate property name.");
                    values.Add(info.Name, info.GetValue(target, null));
                }
            }
            return values;
        }

        /// <summary>
        /// Tests the multiple choice options.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        [TestMethod()]
        public void MultipleChoiceOptionsTest()
        {
            using (IDictionary target = user.Open())
            {
                Dictionary<string, object> values = GetInitialValues(target.DefaultSettings.MultipleChoiceOptions);
                target.Dispose();

                for (int i = 0; i < loopcount; i++)
                {
                    using (IDictionary target2 = user.Open())
                    {
                        CheckAndAssign(target2.DefaultSettings.MultipleChoiceOptions, values);
                        target2.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Tests the query types.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        [TestMethod()]
        public void QueryTypesTest()
        {
            using (IDictionary target = user.Open())
            {
                Dictionary<string, object> values = GetInitialValues(target.DefaultSettings.QueryTypes);
                target.Dispose();

                for (int i = 0; i < loopcount; i++)
                {
                    using (IDictionary target2 = user.Open())
                    {
                        CheckAndAssign(target2.DefaultSettings.QueryTypes, values);
                        target2.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Tests the snooze options.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        [TestMethod()]
        public void SnoozeOptionsTest()
        {
            using (IDictionary target = user.Open())
            {
                Dictionary<string, object> values = GetInitialValues(target.DefaultSettings.SnoozeOptions);
                target.Dispose();

                for (int i = 0; i < loopcount; i++)
                {
                    using (IDictionary target2 = user.Open())
                    {
                        CheckAndAssign(target2.DefaultSettings.SnoozeOptions, values);
                        target2.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Tests the card generation.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        [TestMethod()]
        public void CardAddRemoveTest()
        {
            for (int i = 0; i < loopcount / 2; i++)
            {
                int cardcount = 0;
                int newcardid = 0;
                using (IDictionary target = user.Open())
                {
                    cardcount = target.Cards.Count;
                    if (GetRandBool())
                        newcardid = target.Cards.AddNew().Id;
                    else
                    {
                        ICard card = target.Cards.Create();
                        target.Cards.Cards.Add(card);
                        newcardid = card.Id;
                    }
                    Assert.IsTrue(newcardid > 0, "New card did not have a valid ID.");
                    target.Save();
                }
                //reopen the LM file
                using (IDictionary target = user.Open())
                {
                    Assert.AreEqual(cardcount + 1, target.Cards.Count, "New card was not added properly.");
                    target.Cards.Delete(newcardid);
                    target.Save();
                }
                //reopen the LM file
                using (IDictionary target = user.Open())
                {
                    Assert.AreEqual(cardcount, target.Cards.Count, "New card was not deleted properly.");
                }
            }
        }

        /// <summary>
        /// Tests card data assignment.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-05</remarks>
        [TestMethod()]
        public void CardDataTest()
        {
            //create new card
            using (IDictionary target = user.Open())
            {
                ICard card = target.Cards.AddNew();
                int cardid = card.Id;
                int boxno = card.Box;
                DateTime timestamp = card.Timestamp;
                string answer = "1";
                card.Answer.AddWord(card.Answer.CreateWord(answer, WordType.Word, true));
                card.AnswerDistractors.AddWord(card.AnswerDistractors.CreateWord(answer, WordType.Distractor, true));
                target.Save();
                target.Dispose();

                for (int i = 0; i < loopcount; i++)
                {
                    using (IDictionary target2 = user.Open())
                    {
                        card = target2.Cards.Get(cardid);
                        Assert.AreEqual(boxno, card.Box, "Box was not saved properly.");
                        Assert.AreEqual(timestamp, card.Timestamp, "Timestamp was not saved properly.");
                        Assert.AreEqual(1, card.Answer.Words.Count, "Not all answers were saved properly.");
                        Assert.AreEqual(1, card.AnswerDistractors.Words.Count, "Not all answer distractors were saved properly.");
                        Assert.AreEqual(answer, card.Answer.Words[0].Word, "Answer was not saved properly.");
                        Assert.AreEqual(answer, card.AnswerDistractors.Words[0].Word, "Answer distractor was not saved properly.");

                        boxno = card.Box = random.Next(target2.Boxes.Box.Count + 1) - 1;
                        timestamp = card.Timestamp = DateTime.Now;

                        if (GetRandBool()) //test different methods to change the word
                        {
                            answer = card.Answer.Words[0].Word = random.Next(100).ToString();
                            card.AnswerDistractors.Words[0].Word = answer;
                        }
                        else
                        {
                            answer = random.Next(100).ToString();
                            card.Answer.Words.Clear();
                            card.Answer.AddWord(card.Answer.CreateWord(answer, WordType.Word, true));
                            card.AnswerDistractors.Words.Clear();
                            card.AnswerDistractors.AddWord(card.AnswerDistractors.CreateWord(answer, WordType.Distractor, true));
                        }
                        target2.Save();
                    }
                }
            }
        }

        /// <summary>
        /// A test for the AddMedia function.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-03</remarks>
        [TestMethod(), TestCategory("Deprecated")]
        public void AddMediaObjectsTest()
        {
            string workingDirectory = Environment.CurrentDirectory;
            int newcardid = 0;
            int newmediacount = 0;
            using (IDictionary target = user.Open())
            {
                //get temp folder
                DirectoryInfo tempfolder;
                int tempfolderindex = 0;
                do
                {
                    tempfolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "MLifterTest" + tempfolderindex.ToString()));
                    tempfolderindex++;
                } while (tempfolder.Exists);
                tempfolder.Create();

                try
                {
                    List<KeyValuePair<FileInfo, EMedia>> mediafiles = new List<KeyValuePair<FileInfo, EMedia>>();

                    //get some Media files
                    foreach (ICard card in target.Cards.Cards)
                    {
                        foreach (IMedia media in card.QuestionMedia)
                        {
                            FileInfo mediafile = new FileInfo(media.Filename);
                            if (mediafile.Exists)
                            {
                                mediafile = mediafile.CopyTo(Path.Combine(tempfolder.FullName, mediafile.Name), true);
                                mediafiles.Add(new KeyValuePair<FileInfo, EMedia>(mediafile, media.MediaType));
                            }
                        }
                    }

                    Assert.IsTrue(mediafiles.Count > 0, "No Media file found in sample LM.");

                    //add the Media files to a new card
                    ICard newcard = target.Cards.AddNew();
                    newcardid = newcard.Id;

                    foreach (KeyValuePair<FileInfo, EMedia> mediafile in mediafiles)
                    {
                        IMedia media = newcard.CreateMedia(mediafile.Value, mediafile.Key.FullName, true, true, false);
                        if (GetRandBool()) //both ways must work
                            newcard.AddMedia(media, Side.Question);
                        else
                            newcard.QuestionMedia.Add(media);
                        newmediacount++;
                    }

                    //AddMedia has changed the current directory
                    Environment.CurrentDirectory = workingDirectory;

                    target.Save();
                }
                finally
                {
                    if (tempfolder != null && tempfolder.Exists)
                        tempfolder.Delete(true);
                }
            }
            //reopen file
            using (IDictionary target = user.Open())
            {
                ICard card = target.Cards.Get(newcardid);

                //Assert.IsTrue(card.QuestionMedia.Count == newmediacount && card.AnswerMedia.Count == 0, "Not all Media files were added properly to the card.");
                //Can currently not be checked: Audio fils get cleaned up (only the last one survives) [ML-1320]

                int foundmediacount = 0;
                foreach (IMedia media in card.QuestionMedia)
                {
                    //check if file exists in Media directory
                    Assert.IsTrue(File.Exists(media.Filename), "Media file could not be found: " + media.Filename);
                    foundmediacount++;
                }
                Assert.IsTrue(newmediacount == 0 || foundmediacount > 0, "There were Media objects added, but could not be found afterwards.");
                target.Cards.Delete(newcardid);
                target.Save();
            }
        }

        /// <summary>
        /// Tests the box size change functions.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-04</remarks>
        [TestMethod()]
        public void BoxSizesTest()
        {
            using (IDictionary target = user.Open())
            {
                //add new chapter, add new card in chaptert
                IChapter chapter = target.Chapters.AddNew();
                chapter.Title = "TestChapter";

                ICard card = target.Cards.AddNew();
                int poolsize = target.Boxes.Box[0].Size;
                int boxsize = target.Boxes.Box[5].Size;

                card.Chapter = chapter.Id;
                Assert.AreEqual(0, card.Box, "Newly created card is not in pool.");
                Assert.AreEqual(true, card.Active, "Newly created card is not active.");

                card.Box = 5;
                Assert.AreEqual(poolsize - 1, target.Boxes.Box[0].Size, "Card was not removed from pool.");
                Assert.AreEqual(boxsize, target.Boxes.Box[5].Size, "Box size did change, although moved card is not in querychapters."); //[ML-1321]

                target.DefaultSettings.SelectedLearnChapters.Add(chapter.Id);
                Assert.AreEqual(boxsize + 1, target.Boxes.Box[5].Size, "Box size did not change, although moved card is in querychapters.");

                card.Active = false;
                Assert.AreEqual(boxsize, target.Boxes.Box[5].Size, "Box size did not change during deactivation, although moved card is in querychapters.");

                target.DefaultSettings.SelectedLearnChapters.Remove(chapter.Id);
                card.Active = true;
                Assert.AreEqual(boxsize, target.Boxes.Box[5].Size, "Box size did change during deactivation, although moved card is not in querychapters.");
                Assert.AreEqual(poolsize - 1, target.Boxes.Box[0].Size, "Poolsize did increase during activation, although activated card is not in querychapters.");

                target.Cards.Delete(card.Id);
            }
        }

        /// <summary>
        /// Tests chapter adding and removing.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-04</remarks>
        [TestMethod()]
        public void ChaptersAddRemoveTest()
        {
            int existingchaptercount;
            int addedchaptercount = random.Next(100);
            using (IDictionary target = user.Open())
            {
                Assert.AreEqual(target.Chapters.Count, target.Chapters.Chapters.Count, "Chapter count must be equal.");
                existingchaptercount = target.Chapters.Count;
                for (int i = 0; i < addedchaptercount; i++)
                {
                    IChapter chapter = null;
                    chapter = target.Chapters.AddNew();

                    chapter.Title = (existingchaptercount + i).ToString();
                    Assert.IsTrue(chapter.Id >= 0, "New chapter did not have a valid ID.");
                    Assert.AreEqual(0, chapter.ActiveSize, "New chapter must have an active size of 0.");
                    Assert.AreEqual(0, chapter.Size, "New chapter must have a size of 0.");
                }
                Assert.AreEqual(target.Chapters.Count, target.Chapters.Chapters.Count, "Chapter count must be equal.");
                target.Save();
            }
            //reopen LM
            using (IDictionary target = user.Open())
            {
                Assert.AreEqual(target.Chapters.Count, target.Chapters.Chapters.Count, "Chapter count must be equal.");
                Assert.AreEqual(existingchaptercount + addedchaptercount, target.Chapters.Count, "Chapter count missmatch - not all chapters were created properly.");
                List<int> deleteIDs = new List<int>();
                for (int i = 0; i < target.Chapters.Count; i++)
                {
                    if (i >= existingchaptercount)
                    {
                        Assert.AreEqual(i.ToString(), target.Chapters.Chapters[i].Title, "Chapter title was not saved successfully.");
                        deleteIDs.Add(target.Chapters.Chapters[i].Id);
                    }
                }
                foreach (int deleteID in deleteIDs)
                    target.Chapters.Delete(deleteID);
                target.Save();
            }
            //reopen LM
            using (IDictionary target = user.Open())
            {
                Assert.AreEqual(existingchaptercount, target.Chapters.Count, "Chapter count missmatch - not all chapters were deleted properly.");
            }
        }
    }
}
