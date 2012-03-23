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
