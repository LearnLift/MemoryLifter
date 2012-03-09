using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;
using Npgsql;

namespace MLifterTest.DAL
{
	/// <summary>
	/// Summary Description for IDictionaries
	/// </summary>
	[TestClass]
	public class IDictionariesTests
	{
		public IDictionariesTests()
		{
			//
			// TODO: Add constructor logic here
			//
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
		/// Tries to create and modifiy a password protected eDB LearningModule
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-02-16</remarks>
		[TestMethod]
		[ExpectedException(typeof(ProtectedLearningModuleException))]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionariesAccessToProtectedLM()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext).ToLower() == "sqlce")
			{
				LMConnectionParameter param = new LMConnectionParameter(TestContext);
				param.Password = "123";
				param.RepositoryName = "AccessToProtectedLM";
				IDictionary target = TestInfrastructure.GetLMConnection(param);
			}
			else
				throw new ProtectedLearningModuleException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}
	}
}
