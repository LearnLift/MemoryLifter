using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;

using MLifterTest.BusinessLayer;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;


namespace MLifterTest.DAL
{
	/// <summary>
	/// Summary Description for IDictionary
	/// </summary>
	[TestClass]
	public class UniCodeTest
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

		public UniCodeTest()
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
		/// Simples the test.
		/// Create an Instance of a IDictionary, check if reference is != null and verify the typ of the object.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-28-07</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void UniCodeTestVerifyUniCodeExamples()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				//Later to remove disturbing stuff from myCardsList toString method
				string pattern = @"^\d*\s\-\s(.*?)\s\-\s$";
				string replacement = @"$1";

				//Get the LM
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
				IChapter chapter = testLM.Chapters.AddNew();

				//Get the reference test data
				string[] UniCodeExamples = File.ReadAllLines(Path.Combine(TestContext.TestDeploymentDir, Properties.Resources.UNICODETSTDATA), Encoding.UTF8);

				Dictionary<int, int> cardIds = new Dictionary<int, int>(); //Key: indexCounter, Value: CardId => the cards can be in any order
				int indexCounter = 0;

				//Insert the data as Word into a list of cards of the LM
				foreach (string UniCodeExample in UniCodeExamples)
				{
					ICard myCard = testLM.Cards.AddNew();
					myCard.Chapter = chapter.Id;
					IWord myWord = myCard.Question.CreateWord(UniCodeExample, WordType.Word, false);
					myCard.Question.AddWord(myWord);
					cardIds[indexCounter] = myCard.Id;
					++indexCounter;
				}

				//Compare the reference data with the cardlist
				indexCounter = 0;
				foreach (string UniCodeExample in UniCodeExamples)
				{
					Assert.AreEqual<string>(UniCodeExample, Regex.Replace(testLM.Cards.Get(cardIds[indexCounter]).ToString(), pattern, replacement), "Problems at line :" + indexCounter);
					++indexCounter;
				}


			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}
	}
}
