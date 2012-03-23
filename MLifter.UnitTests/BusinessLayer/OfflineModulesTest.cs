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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using System.IO;

namespace MLifterTest.BusinessLayer
{
    [TestClass]
    public class OfflineModulesTest
    {
        static string tempFolder;
        LearningModulesIndexEntry lm1;
        LearningModulesIndexEntry lm2;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentLearningModulesTest"/> class.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        public OfflineModulesTest()
        {
            tempFolder = Path.Combine(Path.GetTempPath(), "ML_Temp");
            Directory.CreateDirectory(tempFolder);

            File.Create(Path.Combine(tempFolder, "lm1.mlm"));
            File.Create(Path.Combine(tempFolder, "lm2.mlm"));

            lm1 = new LearningModulesIndexEntry();
            lm2 = new LearningModulesIndexEntry();
            ConnectionStringStruct css1 = new ConnectionStringStruct();
            css1.ConnectionString = Path.Combine(tempFolder, "lm1.mlm");
            css1.Typ = MLifter.DAL.DatabaseType.MsSqlCe;
            css1.LmId = 1;

            ConnectionStringStruct css2 = new ConnectionStringStruct();
            css2.ConnectionString = Path.Combine(tempFolder, "lm2.mlm");
            css2.Typ = MLifter.DAL.DatabaseType.MsSqlCe;
            css2.LmId = 2;

            IConnectionString con = new UncConnectionStringBuilder(tempFolder);

            lm1.DisplayName = "learnmodule1";
            lm1.ConnectionName = "connectionname1";
            lm1.Connection = con;
            lm1.ConnectionString = css1;
            lm1.SyncedPath = css1.ConnectionString;

            lm2.DisplayName = "learnmodule2";
            lm2.ConnectionName = "connectionname2";
            lm2.Connection = con;
            lm2.ConnectionString = css2;
            lm2.SyncedPath = css2.ConnectionString;
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
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Directory.Delete(tempFolder, true);
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
        /// Tests the offline storage.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-02</remarks>
        [TestMethod]
        public void TestOfflineStorage()
        {
            string storageFile = Path.Combine(tempFolder, "storage.dat");
            IConnectionString con = new UncConnectionStringBuilder(tempFolder);
            SyncedModulesIndex.Clear();

            SyncedModulesIndex.Add(lm1.Connection, lm1);
            SyncedModulesIndex.Add(lm2.Connection, lm2);
            SyncedModulesIndex.Dump(storageFile);
            Assert.AreEqual<int>(2, SyncedModulesIndex.Get(con).Count);

            SyncedModulesIndex.Clear();
            Assert.AreEqual<int>(0, SyncedModulesIndex.Get(con).Count);

            SyncedModulesIndex.Restore(storageFile);
            Assert.AreEqual<int>(2, SyncedModulesIndex.Get(con).Count);
            Assert.AreEqual<string>(lm1.ConnectionString.ConnectionString, SyncedModulesIndex.Get(con)[0].ConnectionString.ConnectionString);
            Assert.AreEqual<string>(lm2.ConnectionString.ConnectionString, SyncedModulesIndex.Get(con)[1].ConnectionString.ConnectionString);
        }
    }
}
