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
    /// Summary Description for IChapters
    /// </summary>
    [TestClass()]
    public class IChaptersTests
    {
        public struct TitleDesc
        {
            public string title;
            public string desc;
        }

        public IChaptersTests()
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
        public void IChaptersAddNewCheckNull()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        Assert.IsInstanceOfType(chapter, typeof(IChapter), "IDictionaryName.Chapters.AddNew() does not create an IChapter!");
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersAddNewCheckChapterContent()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    List<TitleDesc> test = new List<TitleDesc>();
                    int numberOfTestData = 10;

                    for (int i = 0; i < numberOfTestData; i++)
                    {
                        TitleDesc td = new TitleDesc();
                        td.title = "Chapter - " + Guid.NewGuid().ToString();
                        td.desc = "Description - " + Guid.NewGuid().ToString();
                        test.Add(td);
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = test[i].title;
                        chapter.Description = test[i].desc;

                        Assert.AreEqual<string>(chapter.Title, test[i].title, "IChapter does not save the Title to IDictionary");
                        Assert.AreEqual<string>(chapter.Description, test[i].desc, "IChapter does not save the Description to IDictionary");
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersDeleteChapter()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    int currentChapters = writeLM.Chapters.Chapters.Count;

                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "1";
                    int chp_id = chapter.Id;

                    writeLM.Chapters.Delete(chp_id);
                    Assert.AreEqual<int>(writeLM.Chapters.Chapters.Count, currentChapters, "IChapters.Delete() does not delete the chapter");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [ExpectedException(typeof(IdAccessException), "IdAccessException Exception was not thrown.")]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersDeleteUnkownChapter()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "1";

                    writeLM.Chapters.Delete(int.MaxValue);       //unknown id
                }
            }
            else
                throw new IdAccessException(0);
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersFindChapterByTitle()
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

                    IChapter lostChild = writeLM.Chapters.Find("Chapter 5");
                    Assert.AreEqual<string>(lostChild.Title, "Chapter 5", "IChapters.Find() does not find the correct chapter");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersFindChapterByUnknownTitle()
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
                    IChapter lostChild = writeLM.Chapters.Find("Chapter xyz");
                    Assert.IsNull(lostChild, "Chapters.Find(unknown) does not return null.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersGetChapterByID()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    int[] ids = new int[10];
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + i.ToString();
                        ids[i] = chapter.Id;
                    }

                    IChapter lostChild = writeLM.Chapters.Get(ids[5]);
                    Assert.AreEqual<string>(lostChild.Title, "Chapter 5", "IChapters.Get() does not get the correct chapter");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [ExpectedException(typeof(IdAccessException), "IdAccessException Exception was not thrown.")]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersGetChapterByUnknownID()
        {
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + (i + 1);
                    }
                    IChapter lostChild = writeLM.Chapters.Get(int.MaxValue);
                }
                throw new IdAccessException(0);
            }
            else
                throw new IdAccessException(0);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersMoveChapter()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    Tools.TestStopWatch.Start(TestContext);
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + (i + 1);
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        IList<IChapter> chapters = writeLM.Chapters.Chapters;
                        int firstIdx = TestInfrastructure.Random.Next(0, chapters.Count);
                        int secondIdx = TestInfrastructure.Random.Next(0, chapters.Count);
                        int first = chapters[firstIdx].Id;
                        int second = chapters[secondIdx].Id;
                        if (firstIdx == secondIdx)
                            continue;

                        writeLM.Chapters.Move(first, second);
                        IList<IChapter> chapters2 = writeLM.Chapters.Chapters;
                        int newIdx = -1;
                        for (int k = 0; k < chapters2.Count; k++)
                        {
                            if (chapters2[k].Id == first)
                                newIdx = k;
                        }
                        Assert.IsFalse(newIdx < 0, "Could not find first ID!");
                        if (firstIdx < secondIdx)
                        {
                            Assert.AreEqual<int>(chapters2[newIdx - 1].Id, second, string.Format("IChapters.Move does not work correct. [{0} <-> {1}]", first, second));
                        }
                        else
                        {
                            Assert.AreEqual<int>(chapters2[newIdx + 1].Id, second, string.Format("IChapters.Move does not work correct. [{0} <-> {1}]", first, second));
                        }
                    }
                    Tools.TestStopWatch.Stop(TestContext);
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(IdAccessException), "IdAccessException Exception was not thrown.")]
        public void IChaptersMoveChapterUnknownFirstID()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            //This test moves 10 chapters from 1,2,3,4,5,6,7,8,9,10 to 10,9,8,7,6,5,4,3,2,1 :-)
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    List<int> chapters = new List<int>();
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + (i + 1);
                        chapters.Add(chapter.Id);
                    }
                    int unknown = chapters[chapters.Count - 1] + 1000;  //probably unknown
                    int second = chapters[TestInfrastructure.Random.Next(0, chapters.Count)];
                    writeLM.Chapters.Move(unknown, second);
                }
            }
            else
                throw new IdAccessException(0);
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        [ExpectedException(typeof(IdAccessException), "IdAccessException Exception was not thrown.")]
        public void IChaptersMoveChapterUnknownSecondID()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            //This test moves 10 chapters from 1,2,3,4,5,6,7,8,9,10 to 10,9,8,7,6,5,4,3,2,1 :-)
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    List<int> chapters = new List<int>();
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + (i + 1);
                        chapters.Add(chapter.Id);
                    }
                    int unknown = chapters[chapters.Count - 1] + 1000;  //probably unknown
                    int first = chapters[TestInfrastructure.Random.Next(0, chapters.Count)];
                    writeLM.Chapters.Move(first, unknown);
                }
            }
            else
                throw new IdAccessException(0);
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersMoveChapterFirstIDequalSecondID()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + i.ToString();
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        IList<IChapter> chapters = writeLM.Chapters.Chapters;
                        int idx = TestInfrastructure.Random.Next(0, chapters.Count);
                        int randomId = chapters[idx].Id;
                        writeLM.Chapters.Move(randomId, randomId);
                        IList<IChapter> chapters2 = writeLM.Chapters.Chapters;
                        int newIdx = -1;
                        for (int k = 0; k < chapters2.Count; k++)
                        {
                            if (chapters2[k].Id == randomId)
                                newIdx = k;
                        }
                        Assert.IsTrue(idx == newIdx, "Chapter was moved!");
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChaptersCount()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    int nrOfChapters = 123;
                    int currentChapters = writeLM.Chapters.Chapters.Count;
                    for (int i = 0; i < nrOfChapters; i++)
                    {
                        IChapter chapter = writeLM.Chapters.AddNew();
                        chapter.Title = "Chapter " + (i + 1);
                    }
                    Assert.AreEqual<int>(nrOfChapters + currentChapters, writeLM.Chapters.Chapters.Count, "IChapters.Count does not return the correct number.");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
