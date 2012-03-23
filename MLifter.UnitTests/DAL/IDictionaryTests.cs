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
using System.Xml;


namespace MLifterTest.DAL
{
	/// <summary>
	/// Summary Description for IDictionary
	/// </summary>
	[TestClass]
	public class IDictionaryTests
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

		public IDictionaryTests()
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
		public void IDictionaryFactoryCreateInstanceTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty);
				Assert.IsNotNull(testLM, "Factory returned a null reference.");
				//Better would be if instead of XmlDictionary something is typed that extends IDictionary
				Assert.IsTrue(testLM is IDictionary);
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// IOs the exception test.
		/// Provokes a IO Exception by first creating a odx file somewhere and then try to create instances
		/// of the same connection strings two times.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-28-07</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(System.IO.IOException), "System.IO.IOException did not occur, after using same (file) connection string although it should.")]
		public void IDictionaryIOExceptionTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			//This test will only execute, when using a File-Connection-String
			if (TestInfrastructure.IsActive(TestContext) && TestContext.DataRow["Type"].ToString() == "File")
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					testLM.Save();
					MLifter.DAL.Interfaces.IUser user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
						testLM.Parent.CurrentUser.ConnectionString, (DataAccessErrorDelegate)delegate { return; }, TestContext);
					IDictionary testLM2 = user.Open();
				}
			}
			else
				throw new System.IO.IOException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary author property test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryAuthorPropertyTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				String myAuthor = "The author was a creazy guy";
				testLM.Author = myAuthor;
				Assert.AreEqual<String>(myAuthor, testLM.Author, "Author String give back does not match with input");
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary Description property test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryDescriptionPropertyTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				String myDescription = "Ich hatte 18 Flaschen Whisky im Keller. Meine Frau befahl mir, den Inhalt jeder einzelnen Flasche ins Sp�lbecken zu gie�en, sonst k�nnte ich was erlebe. Ich fing also mit der unangenehmen Arbeit an. Ich zog den Korken von der ersten Flasche und goss den Inhalt ins Becken, mit Ausnahme von einem Glas, das ich trank. Dann extrahierte ich den Korken von der zweiten Flasche und tat desselbe mit Ausnahme eines Glases, das ich trank. Dann zog ich den Korken von der dritten Flasche und goss den Whisky ins Becken, das ich trank. Ich zog den Korken von der vierten ins Becken und goss die Flasche ins Glas, das ich trank. Ich zog die Flasche vom n�chsten Korken, trank ein Becken daraus und warf den Rest ins Glas. Ich zog das Becken aus dem n�chsten Glas und goss den Korken in die Flasche. Dann korkte ich das Becken mit dem Glas, flachte den Trank und trinkte den Genuss. Als ich alles entleert hatte, hielt ich das Haus mit der einen Hand fest, z�hlte die Gl�ser, Korken und Flaschen und Becken mit der anderen und stellte fest, dass es 39 waren. Und als das Haus wieder vorbeikam, z�hlte ich sie nochmals und hatte dann alle H�user in der Flasche, die ich trank. Ich stehe gar nicht unter Alfluss von Einkohol wie manche Denker leuten, ich bin nicht halb so bedenkt wie trinken k�nntet, aber ich, ich habe so ein f�hlsames Geselt.. aaaaaaaaahhhhhhhhhhhh.............. ";
				testLM.Description = myDescription;
				Assert.AreEqual<String>(myDescription, testLM.Description, "Description String give back does not match with input");
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary category test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryCategoryTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					List<Category> categories = Category.GetCategories();
					Category category = categories[TestInfrastructure.Random.Next(0, categories.Count)];
					target.Category = category;
					Assert.AreEqual<int>(target.Category.Id, category.Id, "The saved category is not correct.");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary question caption test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryQuestionCaptionTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);

				String myQuestionCaption = "Romulan language";
				testLM.DefaultSettings.QuestionCaption = myQuestionCaption;
				Assert.AreEqual<String>(myQuestionCaption, testLM.DefaultSettings.QuestionCaption, "QuestionCaptionTest does not match");
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary answer caption test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryAnswerCaptionTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);

				String myAnswerCaption = "Klingon language";
				testLM.DefaultSettings.AnswerCaption = myAnswerCaption;
				Assert.AreEqual<String>(myAnswerCaption, testLM.DefaultSettings.AnswerCaption, "AnswerCaptionTest does not match");
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Test the culture property.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryCultureTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
					CultureInfo ai = (CultureInfo)cultures.GetValue(TestInfrastructure.Random.Next(1, cultures.Length));
					CultureInfo qi = (CultureInfo)cultures.GetValue(TestInfrastructure.Random.Next(1, cultures.Length));
					target.DefaultSettings.AnswerCulture = ai;
					Assert.AreEqual<CultureInfo>(ai, target.DefaultSettings.AnswerCulture, "The culture does not match for AnswerCulture.");
					target.DefaultSettings.QuestionCulture = qi;
					Assert.AreEqual<CultureInfo>(qi, target.DefaultSettings.QuestionCulture, "The culture does not match for QuestionCulture.");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary Media directory test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void IDictionaryMediaDirectoryTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					try
					{
						String myBadPath = "C:/��j/fluppipath\\backagainhttp://somewhere";
						testLM.MediaDirectory = myBadPath;
						Assert.AreNotEqual<String>(myBadPath, testLM.MediaDirectory, "Bad directory path did get accepted.");
					}
					catch (NotSupportedException)
					{
						//throw expected exception for the test to run successful
						throw new DirectoryNotFoundException();
					}
				}
			}
			else
				throw new DirectoryNotFoundException();     //if tests are disabled, nevertheless the test should run successfully.

			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary comment audio test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryCommentAudioTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					Assert.AreEqual(typeof(Dictionary<CommentarySoundIdentifier, IMedia>), testLM.DefaultSettings.CommentarySounds.GetType(),
						"CommentarySounds were not a Dictionary<CommentarySoundIdentifier,IMedia> type.");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary allowed query types.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryAllowedQueryTypes()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					testLM.AllowedSettings.QueryTypes.ImageRecognition = true;
					testLM.AllowedSettings.QueryTypes.ListeningComprehension = false;
					testLM.AllowedSettings.QueryTypes.MultipleChoice = true;
					testLM.AllowedSettings.QueryTypes.Sentence = false;
					testLM.AllowedSettings.QueryTypes.Word = true;

					Assert.IsTrue(testLM.AllowedSettings.QueryTypes.ImageRecognition.GetValueOrDefault());
					Assert.IsFalse(testLM.AllowedSettings.QueryTypes.ListeningComprehension.GetValueOrDefault());
					Assert.IsTrue(testLM.AllowedSettings.QueryTypes.MultipleChoice.GetValueOrDefault());
					Assert.IsFalse(testLM.AllowedSettings.QueryTypes.Sentence.GetValueOrDefault());
					Assert.IsTrue(testLM.AllowedSettings.QueryTypes.Word.GetValueOrDefault());
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary allowed query directions.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryAllowedQueryDirections()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					String blockErrorMessage;

					blockErrorMessage = "1st Block failed";
					testLM.AllowedSettings.QueryDirections.Answer2Question = true;
					testLM.AllowedSettings.QueryDirections.Question2Answer = true;
					testLM.AllowedSettings.QueryDirections.Mixed = true;
					Assert.IsTrue(testLM.AllowedSettings.QueryDirections.Answer2Question.GetValueOrDefault(), blockErrorMessage);
					Assert.IsTrue(testLM.AllowedSettings.QueryDirections.Question2Answer.GetValueOrDefault(), blockErrorMessage);
					Assert.IsTrue(testLM.AllowedSettings.QueryDirections.Mixed.GetValueOrDefault(), blockErrorMessage);

					blockErrorMessage = "2nd Block failed";
					testLM.AllowedSettings.QueryDirections.Answer2Question = false;
					testLM.AllowedSettings.QueryDirections.Question2Answer = false;
					testLM.AllowedSettings.QueryDirections.Mixed = false;
					Assert.IsFalse(testLM.AllowedSettings.QueryDirections.Answer2Question.GetValueOrDefault(), blockErrorMessage);
					Assert.IsFalse(testLM.AllowedSettings.QueryDirections.Question2Answer.GetValueOrDefault(), blockErrorMessage);
					Assert.IsFalse(testLM.AllowedSettings.QueryDirections.Mixed.GetValueOrDefault(), blockErrorMessage);

					blockErrorMessage = "3rd Block failed";
					testLM.AllowedSettings.QueryDirections.Answer2Question = false;
					testLM.AllowedSettings.QueryDirections.Question2Answer = true;
					testLM.AllowedSettings.QueryDirections.Mixed = false;
					Assert.IsFalse(testLM.AllowedSettings.QueryDirections.Answer2Question.GetValueOrDefault(), blockErrorMessage);
					Assert.IsTrue(testLM.AllowedSettings.QueryDirections.Question2Answer.GetValueOrDefault(), blockErrorMessage);
					Assert.IsFalse(testLM.AllowedSettings.QueryDirections.Mixed.GetValueOrDefault(), blockErrorMessage);

					blockErrorMessage = "4th Block failed";
					testLM.AllowedSettings.QueryDirections.Answer2Question = true;
					testLM.AllowedSettings.QueryDirections.Question2Answer = false;
					testLM.AllowedSettings.QueryDirections.Mixed = true;
					Assert.IsTrue(testLM.AllowedSettings.QueryDirections.Answer2Question.GetValueOrDefault(), blockErrorMessage);
					Assert.IsFalse(testLM.AllowedSettings.QueryDirections.Question2Answer.GetValueOrDefault(), blockErrorMessage);
					Assert.IsTrue(testLM.AllowedSettings.QueryDirections.Mixed.GetValueOrDefault(), blockErrorMessage);

				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary score.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryScoreTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					foreach (ICard card in testLM.Cards.Cards)
						card.Box = 0;

					Assert.AreEqual<double>(0, testLM.Score, "Score is not right calculated!");

					foreach (ICard card in testLM.Cards.Cards)
						card.Box = 5;

					Assert.AreEqual<double>(testLM.Cards.Count * 5, testLM.Score, "Score is not right calculated!");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary high score.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryHighScoreTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					//Specific HighScore Number
					int myHighScore = 5645238;
					testLM.HighScore = myHighScore;
					Assert.AreEqual<double>(myHighScore, testLM.HighScore);

					//Max Int HighScore Number
					testLM.HighScore = 100;
					Assert.AreEqual<double>(100, testLM.HighScore);

					//Min Int HighScore Number
					testLM.HighScore = 0;
					Assert.AreEqual<double>(0, testLM.HighScore);
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary query options.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryQueryOptionsTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					testLM.DefaultSettings.AutoplayAudio = true;
					testLM.DefaultSettings.CaseSensitive = false;
					testLM.DefaultSettings.ConfirmDemote = true;
					testLM.DefaultSettings.CorrectOnTheFly = false;
					testLM.DefaultSettings.EnableCommentary = true;
					testLM.DefaultSettings.EnableTimer = false;
					testLM.DefaultSettings.GradeSynonyms.AllKnown = true;

					testLM.DefaultSettings.GradeTyping.AllCorrect = true;

					testLM.DefaultSettings.MultipleChoiceOptions.AllowMultipleCorrectAnswers = true;
					testLM.DefaultSettings.MultipleChoiceOptions.AllowRandomDistractors = false;
					testLM.DefaultSettings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers = 3;
					testLM.DefaultSettings.MultipleChoiceOptions.NumberOfChoices = 7;

					//reset all query directions (only one is allowed)
					testLM.DefaultSettings.QueryDirections.Question2Answer =
						testLM.DefaultSettings.QueryDirections.Answer2Question =
						testLM.DefaultSettings.QueryDirections.Mixed =
						false;

					testLM.DefaultSettings.QueryDirections.Answer2Question = true;

					testLM.DefaultSettings.QueryTypes.ListeningComprehension = true;

					testLM.DefaultSettings.RandomPool = false;
					testLM.DefaultSettings.SelfAssessment = true;
					testLM.DefaultSettings.ShowImages = false;
					testLM.DefaultSettings.ShowStatistics = true;
					testLM.DefaultSettings.SkipCorrectAnswers = false;

					testLM.DefaultSettings.SnoozeOptions.IsCardsEnabled = true;
					testLM.DefaultSettings.StripChars = "StripStringTest";

					Assert.IsTrue(testLM.DefaultSettings.AutoplayAudio.Value);
					Assert.IsFalse(testLM.DefaultSettings.CaseSensitive.Value);
					Assert.IsTrue(testLM.DefaultSettings.ConfirmDemote.Value);
					Assert.IsFalse(testLM.DefaultSettings.CorrectOnTheFly.Value);
					Assert.IsTrue(testLM.DefaultSettings.EnableCommentary.Value);
					Assert.IsFalse(testLM.DefaultSettings.EnableTimer.Value);

					Assert.IsTrue(testLM.DefaultSettings.GradeSynonyms.AllKnown.Value);
					Assert.IsTrue(testLM.DefaultSettings.GradeTyping.AllCorrect.Value);

					Assert.IsTrue(testLM.DefaultSettings.MultipleChoiceOptions.AllowMultipleCorrectAnswers.Value);
					Assert.IsFalse(testLM.DefaultSettings.MultipleChoiceOptions.AllowRandomDistractors.Value);
					Assert.AreEqual(testLM.DefaultSettings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers, 3);
					Assert.AreEqual(testLM.DefaultSettings.MultipleChoiceOptions.NumberOfChoices, 7);


					Assert.IsTrue(testLM.DefaultSettings.QueryDirections.Answer2Question.Value);
					Assert.IsTrue(testLM.DefaultSettings.QueryTypes.ListeningComprehension.Value);

					Assert.IsFalse(testLM.DefaultSettings.RandomPool.Value);
					Assert.IsTrue(testLM.DefaultSettings.SelfAssessment.Value);
					Assert.IsFalse(testLM.DefaultSettings.ShowImages.Value);
					Assert.IsTrue(testLM.DefaultSettings.ShowStatistics.Value);
					Assert.IsFalse(testLM.DefaultSettings.SkipCorrectAnswers.Value);

					Assert.IsTrue(testLM.DefaultSettings.SnoozeOptions.IsCardsEnabled.Value);
					Assert.AreEqual<String>(testLM.DefaultSettings.StripChars, "StripStringTest");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary pool empty test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryPoolEmptyTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					testLM.DefaultSettings.PoolEmptyMessageShown = true;
					Assert.IsTrue(testLM.DefaultSettings.PoolEmptyMessageShown.Value);

					testLM.DefaultSettings.PoolEmptyMessageShown = false;
					Assert.IsFalse(testLM.DefaultSettings.PoolEmptyMessageShown.Value);

				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary use dictionary style test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-28</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryUseDictionaryStyleTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					testLM.DefaultSettings.UseLMStylesheets = true;
					Assert.IsTrue(testLM.DefaultSettings.UseLMStylesheets.Value);

					testLM.DefaultSettings.UseLMStylesheets = false;
					Assert.IsFalse(testLM.DefaultSettings.UseLMStylesheets.Value);

				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary cards test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-29</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryCards()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					Assert.IsNotNull(testLM.Cards, "Cards property is null");
					Assert.IsTrue(testLM.Cards is ICards, "Cards not of type ICards");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Is the dictionary chapters test.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-07-29</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryChapters()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					Assert.IsNotNull(testLM.Chapters, "Chapters property is null");
					Assert.IsTrue(testLM.Chapters is IChapters, "Chapters not of type IChapters");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Is the dictionary Description test with persistent LM to verify if persistency realy works.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-01</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void IDictionaryDescriptionTestWithPersistentLM()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				string myDescription = "Special cognetive Interaction and mental distortion technics";
				int lmid = 0;
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, "IDictionaryTestsLMs"))
				{
					lmid = testLM.Id;
					testLM.Description = myDescription;
					Assert.AreEqual<String>(myDescription, testLM.Description);
					testLM.Save();
				}

				using (IDictionary testLM = TestInfrastructure.GetPersistentLMConnection(TestContext, "IDictionaryTestsLMs", lmid))
				{
					Assert.AreEqual<String>(myDescription, testLM.Description);
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}
		//IStatistics

		[TestMethod]
		[TestProperty("DL", "DanAch"), DataSource("TestSources")]
		public void ResetLearningProgressTest()
		{
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary dictionary = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
				{
					ICardsTests.FillDummyDic(dictionary);

					dictionary.HighScore = 50;

					dictionary.ResetLearningProgress();

					Assert.AreEqual(dictionary is XmlDictionary ? 0 : 10, dictionary.UserSettings.SelectedLearnChapters.Count, "SelectedLearnChapters were not resetted.");

					foreach (ICard card in dictionary.Cards.Cards)
						Assert.IsTrue(card.Box < 1, "Card box not resetted.");

					foreach (IBox box in dictionary.Boxes.Box)
						if (box.Id == 0)
							Assert.AreEqual(dictionary.Cards.Cards.Count, box.CurrentSize, "Not all cards went to the Pool.");
						else
							Assert.AreEqual(0, box.CurrentSize, "Box is not empty.");

					Assert.AreEqual(0, dictionary.Score, "Score was not resetted.");
					Assert.AreEqual(0, dictionary.HighScore, "Highscore was not resetted.");
				}
			}
		}
	}
}
