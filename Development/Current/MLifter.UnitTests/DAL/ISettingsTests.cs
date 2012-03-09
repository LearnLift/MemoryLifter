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
    /// 
    /// </summary>
    /// <remarks>Documented by Dev11, 2008-08-07</remarks>
    [TestClass]
    public class ISettingsTests
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

        public ISettingsTests()
        {
            //
            // TODO: Add constructor logic here
            //
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


        /*[TestMethod]
        [TestProperty("TestCategory", "Developer"),
            DataSource("System.Data.SqlClient",
            "Data Source=ALEABE10\\SQLEXPRESS;Initial Catalog=UnitTests;Persist Security Info=True;User ID=unittests;Password=memorylifter",
            "ConnectionStrings",
            DataAccessMethod.Sequential)]
        public void ISettingsTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);


                writeLM.Dispose();
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }*/
    }
}
