using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Threading;

namespace MLifterTest.BusinessLayer
{


    /// <summary>
    /// Summary Description for RecentLearningModulesTest
    /// </summary>
    [TestClass]
    public class RecentLearningModulesTest
    {
        LearningModulesIndexEntry lm1;
        LearningModulesIndexEntry lm2;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentLearningModulesTest"/> class.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        public RecentLearningModulesTest()
        {
            lm1 = new LearningModulesIndexEntry();
            lm2 = new LearningModulesIndexEntry();
            ConnectionStringStruct css1 = new ConnectionStringStruct();
            css1.ConnectionString = "con1";
            css1.LmId = 1;

            ConnectionStringStruct css2 = new ConnectionStringStruct();
            css2.ConnectionString = "con2";
            css2.LmId = 2;

            lm1.DisplayName = "learnmodule1";
            lm1.ConnectionName = "connectionname1";
            lm1.ConnectionString = css1;

            lm2.DisplayName = "learnmodule2";
            lm2.ConnectionName = "connectionname2";
            lm2.ConnectionString = css2;
        }

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
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
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
        /// Tests if the getRecentModules function returns the expected list
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        [TestMethod]
        public void GetRecentModulesTest()
        {
            generateTestContext();

            List<LearningModulesIndexEntry> actual = RecentLearningModules.GetRecentModules();
            List<LearningModulesIndexEntry> expected = new List<LearningModulesIndexEntry>();

            expected.Add(lm2);
            Thread.Sleep(10);
            expected.Add(lm1);

            CompareLMIndexEntry(expected[0], actual[0]);
            CompareLMIndexEntry(expected[1], actual[1]);
        }

        /// <summary>
        /// Tests if the mostRecentLearningModule is the last added item
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        [TestMethod]
        public void MostRecentLearningModuleTest()
        {
            RecentLearningModules.Add(lm1);
            CompareLMIndexEntry(lm1, RecentLearningModules.GetRecentModules()[0]);
        }

        /// <summary>
        /// Tests if dump and restore results are the same.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        [TestMethod]
        public void DumpRestoreTest()
        {
            try
            {
                generateTestContext();
                RecentLearningModules.Dump("C:\\temp.tmp");
                List<LearningModulesIndexEntry> expected = RecentLearningModules.GetRecentModules();

                RecentLearningModules.Restore("C:\\temp.tmp");
                List<LearningModulesIndexEntry> actual = RecentLearningModules.GetRecentModules();

                CompareLMIndexEntry(expected[0], actual[0]);
                CompareLMIndexEntry(expected[1], actual[1]);
            }
            finally
            {
                try { File.Delete("C:\\temp.tmp"); }
                catch { }
            }
        }

        /// <summary>
        /// Test if a damaged file / not existing file can be restored
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-04</remarks>
        [TestMethod]
        public void RestoreTest00()
        {
            string filename = "C:\\temp.tmp";
            try
            {
                generateTestContext();
                RecentLearningModules.Dump(filename);

                Stream stream = null;
                stream = new FileStream(filename, FileMode.Open);
                stream.Seek(0, SeekOrigin.Begin);
                Random r = new Random();
                for (int i = 0; i < 10; i++)
                {
                    byte damage = (byte)r.Next(65, 90);
                    stream.WriteByte(damage);
                }
                stream.Close();

                RecentLearningModules.Restore(filename);
            }
            finally
            {
                try
                {
                    File.Delete(filename);
                }
                catch { }
            }
        }

        /// <summary>
        /// Tests for null argument.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTest()
        {
            RecentLearningModules.Add((LearningModulesIndexEntry)null);
        }

        /// <summary>
        /// Tests recent items replacement.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        [TestMethod]
        public void AddTest01()
        {
            generateTestContext();
            Thread.Sleep(10);
            RecentLearningModules.Add(lm1);

            // test that original recent list item was erased and replaced
            Assert.AreEqual(2, RecentLearningModules.GetRecentModules().Count);

            // test that the overwritten recent item is now the most recent
            CompareLMIndexEntry(lm1, RecentLearningModules.MostRecentLearningModule);
        }

        /// <summary>
        /// Tests if list is cleared correctly
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        [TestMethod]
        public void ClearTest()
        {
            generateTestContext();
            RecentLearningModules.Clear();
            Assert.AreEqual(0, RecentLearningModules.GetRecentModules().Count);

        }

        /// <summary>
        /// Regenerates the recent learning modules with two unique LearningModulesIndexEntry's
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        private void generateTestContext()
        {
            RecentLearningModules.Clear();

            RecentLearningModules.Add(lm1);
            Thread.Sleep(50);
            RecentLearningModules.Add(lm2);
            Thread.Sleep(50);
        }

        private void CompareLMIndexEntry(LearningModulesIndexEntry expected, LearningModulesIndexEntry actual)
        {
            Assert.IsTrue(actual.DisplayName == expected.DisplayName, "Display name is not equal");
            Assert.IsTrue(actual.ConnectionName == expected.ConnectionName, "Connection name is not equal");
            Assert.IsTrue(actual.ConnectionString.ConnectionString == expected.ConnectionString.ConnectionString, "Connection string is not equal");
            Assert.IsTrue(actual.ConnectionString.LmId == expected.ConnectionString.LmId, "LmId is not equal");
        }
    }
}
