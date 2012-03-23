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

namespace MLifterTest.DAL
{
	[TestClass]
	public class ISettingsTest
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

		public ISettingsTest()
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
		[TestInitialize()]
		public void MyTestInitialize()
		{

		}

		// Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup() {
		//}

		#endregion




		/// <summary>
		/// Set and Get Settings Test for AllowedQueryDirections.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsQueryDirectionsTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				ISettings theSettings = testLM.DefaultSettings;

				//Get Property Object
				IQueryDirections theQueryDirections = theSettings.QueryDirections;

				//Test against the Property Object
				//Set Properties to true
				theQueryDirections.Answer2Question = true;
				theQueryDirections.Mixed = true;
				theQueryDirections.Question2Answer = true;
				//Check if hasValue is valid
				Assert.IsTrue(theQueryDirections.Answer2Question.HasValue, "Answer2Question Property has no value although it should");
				Assert.IsTrue(theQueryDirections.Mixed.HasValue, "Mixed Property has no value although it should");
				Assert.IsTrue(theQueryDirections.Question2Answer.HasValue, "Question2Answer Property has no value although it should");
				//Check if value is true
				Assert.IsTrue(theQueryDirections.Answer2Question.Value, "Answer2Question Property was not set correctly");
				Assert.IsTrue(theQueryDirections.Mixed.Value, "Mixed Property was not set correctly");
				Assert.IsTrue(theQueryDirections.Question2Answer.Value, "Question2Answer Property was not set correctly");
				//Set Properties to false
				theQueryDirections.Answer2Question = false;
				theQueryDirections.Mixed = false;
				theQueryDirections.Question2Answer = false;
				//Check if hasValue is valid
				Assert.IsTrue(theQueryDirections.Answer2Question.HasValue, "Answer2Question Property has no value although it should");
				Assert.IsTrue(theQueryDirections.Mixed.HasValue, "Mixed Property has no value although it should");
				Assert.IsTrue(theQueryDirections.Question2Answer.HasValue, "Question2Answer Property has no value although it should");
				//Check if value is true
				Assert.IsFalse(theQueryDirections.Answer2Question.Value, "Answer2Question Property was not set correctly");
				Assert.IsFalse(theQueryDirections.Mixed.Value, "Mixed Property was not set correctly");
				Assert.IsFalse(theQueryDirections.Question2Answer.Value, "Question2Answer Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					//Set Properties to null
					theQueryDirections.Answer2Question = null;
					theQueryDirections.Mixed = null;
					theQueryDirections.Question2Answer = null;
					//Check if hasValue is valid
					Assert.IsFalse(theQueryDirections.Answer2Question.HasValue, "Answer2Question Property has value although it should not");
					Assert.IsFalse(theQueryDirections.Mixed.HasValue, "Mixed Property has value although it should not");
					Assert.IsFalse(theQueryDirections.Question2Answer.HasValue, "Question2Answer Property has value although it should not");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Get and Set Settings for QueryTypes.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsQueryTypesTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				ISettings theSettings = testLM.DefaultSettings;

				//Get Property Object
				IQueryType theQueryTypes = theSettings.QueryTypes;

				//Test against the Property Object
				theQueryTypes.ImageRecognition = true;
				theQueryTypes.ListeningComprehension = true;
				theQueryTypes.MultipleChoice = true;
				theQueryTypes.Sentence = true;
				theQueryTypes.Word = true;
				//Check if hasValue is valid
				Assert.IsTrue(theQueryTypes.ImageRecognition.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.ListeningComprehension.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.MultipleChoice.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.Sentence.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.Word.HasValue, "QueryTypes Property has no value although it should");
				//Check if value is true
				Assert.IsTrue(theQueryTypes.ImageRecognition.Value, "ImageRecognition Property was not set correctly");
				Assert.IsTrue(theQueryTypes.ListeningComprehension.Value, "ListeningComprehension Property was not set correctly");
				Assert.IsTrue(theQueryTypes.MultipleChoice.Value, "MultipleChoice Property was not set correctly");
				Assert.IsTrue(theQueryTypes.Sentence.Value, "Sentence Property was not set correctly");
				Assert.IsTrue(theQueryTypes.Word.Value, "Word Property was not set correctly");
				//Test against the Property Object
				theQueryTypes.ImageRecognition = false;
				theQueryTypes.ListeningComprehension = false;
				theQueryTypes.MultipleChoice = false;
				theQueryTypes.Sentence = false;
				theQueryTypes.Word = false;
				//Check if hasValue is valid
				Assert.IsTrue(theQueryTypes.ImageRecognition.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.ListeningComprehension.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.MultipleChoice.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.Sentence.HasValue, "QueryTypes Property has no value although it should");
				Assert.IsTrue(theQueryTypes.Word.HasValue, "QueryTypes Property has no value although it should");
				//Check if value is False
				Assert.IsFalse(theQueryTypes.ImageRecognition.Value, "ImageRecognition Property was not set correctly");
				Assert.IsFalse(theQueryTypes.ListeningComprehension.Value, "ListeningComprehension Property was not set correctly");
				Assert.IsFalse(theQueryTypes.MultipleChoice.Value, "MultipleChoice Property was not set correctly");
				Assert.IsFalse(theQueryTypes.Sentence.Value, "Sentence Property was not set correctly");
				Assert.IsFalse(theQueryTypes.Word.Value, "Word Property was not set correctly");
				//Test against the Property Object
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theQueryTypes.ImageRecognition = null;
					theQueryTypes.ListeningComprehension = null;
					theQueryTypes.MultipleChoice = null;
					theQueryTypes.Sentence = null;
					theQueryTypes.Word = null;
					//Check if hasValue is valid
					Assert.IsFalse(theQueryTypes.ImageRecognition.HasValue, "QueryTypes Property has value although it should not");
					Assert.IsFalse(theQueryTypes.ListeningComprehension.HasValue, "QueryTypes Property has value although it should not");
					Assert.IsFalse(theQueryTypes.MultipleChoice.HasValue, "QueryTypes Property has value although it should not");
					Assert.IsFalse(theQueryTypes.Sentence.HasValue, "QueryTypes Property has value although it should not");
					Assert.IsFalse(theQueryTypes.Word.HasValue, "QueryTypes Property has value although it should not");
				}

			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		/// <summary>
		/// Get and Set Settings for QueryTypes.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsMultipleChoiceOptionsTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				ISettings theSettings = testLM.DefaultSettings;

				//Get Property Object
				IQueryMultipleChoiceOptions theMultipleChoiceOptions = theSettings.MultipleChoiceOptions;
				Assert.IsTrue(theMultipleChoiceOptions is IQueryMultipleChoiceOptions, "MultipleChoiceOptions Property gave back wrong type");

				//Test against the Property Object
				int myMaxNumberOfCorrectAnswers = 4;
				int myNumberOfChoices = 2;
				theMultipleChoiceOptions.AllowMultipleCorrectAnswers = true;
				theMultipleChoiceOptions.AllowRandomDistractors = true;
				theMultipleChoiceOptions.MaxNumberOfCorrectAnswers = myMaxNumberOfCorrectAnswers;
				theMultipleChoiceOptions.NumberOfChoices = myNumberOfChoices;
				//Check if hasValue is valid
				Assert.IsTrue(theMultipleChoiceOptions.AllowMultipleCorrectAnswers.HasValue, "MultipleChoiceOptions Property has no value although it should");
				Assert.IsTrue(theMultipleChoiceOptions.AllowRandomDistractors.HasValue, "MultipleChoiceOptions Property has no value although it should");
				Assert.IsTrue(theMultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue, "MultipleChoiceOptions Property has no value although it should");
				Assert.IsTrue(theMultipleChoiceOptions.NumberOfChoices.HasValue, "MultipleChoiceOptions Property has no value although it should");

				//Check if value is true
				Assert.IsTrue(theMultipleChoiceOptions.AllowMultipleCorrectAnswers.Value, "AllowMultipleCorrectAnswers Property was not set correctly");
				Assert.IsTrue(theMultipleChoiceOptions.AllowRandomDistractors.Value, "AllowRandomDistractors Property was not set correctly");
				Assert.AreEqual<int?>(theMultipleChoiceOptions.MaxNumberOfCorrectAnswers, myMaxNumberOfCorrectAnswers, "MaxNumberOfCorrectAnswers Property was not set correctly");
				Assert.AreEqual<int?>(theMultipleChoiceOptions.NumberOfChoices, myNumberOfChoices, "NumberOfChoices Property was not set correctly");
				//Test against the Property Object
				myMaxNumberOfCorrectAnswers = 3;
				myNumberOfChoices = 6;
				theMultipleChoiceOptions.AllowMultipleCorrectAnswers = false;
				theMultipleChoiceOptions.AllowRandomDistractors = false;
				theMultipleChoiceOptions.MaxNumberOfCorrectAnswers = myMaxNumberOfCorrectAnswers;
				theMultipleChoiceOptions.NumberOfChoices = myNumberOfChoices;
				//Check if hasValue is valid
				Assert.IsTrue(theMultipleChoiceOptions.AllowMultipleCorrectAnswers.HasValue, "MultipleChoiceOptions Property has no value although it should");
				Assert.IsTrue(theMultipleChoiceOptions.AllowRandomDistractors.HasValue, "MultipleChoiceOptions Property has no value although it should");
				Assert.IsTrue(theMultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue, "MultipleChoiceOptions Property has no value although it should");
				Assert.IsTrue(theMultipleChoiceOptions.NumberOfChoices.HasValue, "MultipleChoiceOptions Property has no value although it should");
				//Check if value is false
				Assert.IsFalse(theMultipleChoiceOptions.AllowMultipleCorrectAnswers.Value, "AllowMultipleCorrectAnswers Property was not set correctly");
				Assert.IsFalse(theMultipleChoiceOptions.AllowRandomDistractors.Value, "AllowRandomDistractors Property was not set correctly");
				Assert.AreEqual<int?>(theMultipleChoiceOptions.MaxNumberOfCorrectAnswers, myMaxNumberOfCorrectAnswers, "MaxNumberOfCorrectAnswers Property was not set correctly");
				Assert.AreEqual<int?>(theMultipleChoiceOptions.NumberOfChoices, myNumberOfChoices, "NumberOfChoices Property was not set correctly");
				//Test against the Property Object

				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theMultipleChoiceOptions.AllowMultipleCorrectAnswers = null;
					theMultipleChoiceOptions.AllowRandomDistractors = null;
					theMultipleChoiceOptions.MaxNumberOfCorrectAnswers = null;
					theMultipleChoiceOptions.NumberOfChoices = null;

					//Check if hasValue is valid
					Assert.IsFalse(theMultipleChoiceOptions.AllowMultipleCorrectAnswers.HasValue, "MultipleChoiceOptions Property has value although it should not");
					Assert.IsFalse(theMultipleChoiceOptions.AllowRandomDistractors.HasValue, "MultipleChoiceOptions Property has value although it should not");
					Assert.IsFalse(theMultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue, "MultipleChoiceOptions Property has value although it should not");
					Assert.IsFalse(theMultipleChoiceOptions.NumberOfChoices.HasValue, "MultipleChoiceOptions Property has value although it should not");
				}

			}

		}

		/// <summary>
		/// Get and Set Settings for GradeTyping.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsGradeTypingTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				ISettings theSettings = testLM.DefaultSettings;

				//Get Property Object
				IGradeTyping theGradeTyping = theSettings.GradeTyping;
				Assert.IsTrue(theGradeTyping is IGradeTyping, "GradeTyping Property gave back wrong type");
				//Test against the Property Object
				theGradeTyping.AllCorrect = true;
				theGradeTyping.HalfCorrect = true;
				theGradeTyping.NoneCorrect = true;
				theGradeTyping.Prompt = true;
				//Check if hasValue is valid
				Assert.IsTrue(theGradeTyping.AllCorrect.HasValue, "GradeTyping Property has no value although it should");
				Assert.IsTrue(theGradeTyping.HalfCorrect.HasValue, "GradeTyping Property has no value although it should");
				Assert.IsTrue(theGradeTyping.NoneCorrect.HasValue, "GradeTyping Property has no value although it should");
				Assert.IsTrue(theGradeTyping.Prompt.HasValue, "GradeTyping Property has no value although it should");
				//Check if value is true
				Assert.IsTrue(theGradeTyping.AllCorrect.Value, "AllCorrect Property was not set correctly");
				Assert.IsTrue(theGradeTyping.HalfCorrect.Value, "HalfCorrect Property was not set correctly");
				Assert.IsTrue(theGradeTyping.NoneCorrect.Value, "NoneCorrect Property was not set correctly");
				Assert.IsTrue(theGradeTyping.Prompt.Value, "Prompt Property was not set correctly");
				//Test against the Property Object
				theGradeTyping.AllCorrect = false;
				theGradeTyping.HalfCorrect = false;
				theGradeTyping.NoneCorrect = false;
				theGradeTyping.Prompt = false;
				//Check if hasValue is valid
				Assert.IsTrue(theGradeTyping.AllCorrect.HasValue, "GradeTyping Property has no value although it should");
				Assert.IsTrue(theGradeTyping.HalfCorrect.HasValue, "GradeTyping Property has no value although it should");
				Assert.IsTrue(theGradeTyping.NoneCorrect.HasValue, "GradeTyping Property has no value although it should");
				Assert.IsTrue(theGradeTyping.Prompt.HasValue, "GradeTyping Property has no value although it should");
				//Check if value is true
				Assert.IsFalse(theGradeTyping.AllCorrect.Value, "AllCorrect Property was not set correctly");
				Assert.IsFalse(theGradeTyping.HalfCorrect.Value, "HalfCorrect Property was not set correctly");
				Assert.IsFalse(theGradeTyping.NoneCorrect.Value, "NoneCorrect Property was not set correctly");
				Assert.IsFalse(theGradeTyping.Prompt.Value, "Prompt Property was not set correctly");
				//Test against the Property Object
				theGradeTyping.AllCorrect = null;
				theGradeTyping.HalfCorrect = null;
				theGradeTyping.NoneCorrect = null;
				theGradeTyping.Prompt = null;
				//Check if hasValue is valid
				Assert.IsFalse(theGradeTyping.AllCorrect.HasValue, "GradeTyping Property has value although it should not");
				Assert.IsFalse(theGradeTyping.HalfCorrect.HasValue, "GradeTyping Property has value although it should not");
				Assert.IsFalse(theGradeTyping.NoneCorrect.HasValue, "GradeTyping Property has value although it should not");
				Assert.IsFalse(theGradeTyping.Prompt.HasValue, "GradeTyping Property has value although it should not");

			}

		}


		/// <summary>
		/// Set and Get Settings for GradeSynonyms.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsGradeSynonymsTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				ISettings theSettings = testLM.DefaultSettings;

				//Get Property Object
				IGradeSynonyms theGradeSynonyms = theSettings.GradeSynonyms;
				Assert.IsTrue(theGradeSynonyms is IGradeSynonyms, "GradeSynonyms Property gave back wrong type");
				//Test against the Property Object
				theGradeSynonyms.AllKnown = true;
				theGradeSynonyms.FirstKnown = true;
				theGradeSynonyms.HalfKnown = true;
				theGradeSynonyms.OneKnown = true;
				theGradeSynonyms.Prompt = true;
				//Check if hasValue is valid
				Assert.IsTrue(theGradeSynonyms.AllKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.FirstKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.HalfKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.OneKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.Prompt.HasValue, "GradeSynonyms Property has no value although it should");
				//Check if value is true
				Assert.IsTrue(theGradeSynonyms.AllKnown.Value, "AllKnown Property was not set correctly");
				Assert.IsTrue(theGradeSynonyms.FirstKnown.Value, "FirstKnown Property was not set correctly");
				Assert.IsTrue(theGradeSynonyms.HalfKnown.Value, "HalfKnown Property was not set correctly");
				Assert.IsTrue(theGradeSynonyms.OneKnown.Value, "OneKnown Property was not set correctly");
				Assert.IsTrue(theGradeSynonyms.Prompt.Value, "Prompt Property was not set correctly");
				//Test against the Property Object
				theGradeSynonyms.AllKnown = false;
				theGradeSynonyms.FirstKnown = false;
				theGradeSynonyms.HalfKnown = false;
				theGradeSynonyms.OneKnown = false;
				theGradeSynonyms.Prompt = false;
				//Check if hasValue is valid
				Assert.IsTrue(theGradeSynonyms.AllKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.FirstKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.HalfKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.OneKnown.HasValue, "GradeSynonyms Property has no value although it should");
				Assert.IsTrue(theGradeSynonyms.Prompt.HasValue, "GradeSynonyms Property has no value although it should");
				//Check if value is true
				Assert.IsFalse(theGradeSynonyms.AllKnown.Value, "AllKnown Property was not set correctly");
				Assert.IsFalse(theGradeSynonyms.FirstKnown.Value, "FirstKnown Property was not set correctly");
				Assert.IsFalse(theGradeSynonyms.HalfKnown.Value, "HalfKnown Property was not set correctly");
				Assert.IsFalse(theGradeSynonyms.OneKnown.Value, "OneKnown Property was not set correctly");
				Assert.IsFalse(theGradeSynonyms.Prompt.Value, "Prompt Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					//Test against the Property Object
					theGradeSynonyms.AllKnown = null;
					theGradeSynonyms.FirstKnown = null;
					theGradeSynonyms.HalfKnown = null;
					theGradeSynonyms.OneKnown = null;
					theGradeSynonyms.Prompt = null;
					//Check if hasValue is valid
					Assert.IsFalse(theGradeSynonyms.AllKnown.HasValue, "GradeSynonyms Property has value although it should not");
					Assert.IsFalse(theGradeSynonyms.FirstKnown.HasValue, "GradeSynonyms Property has value although it should not");
					Assert.IsFalse(theGradeSynonyms.HalfKnown.HasValue, "GradeSynonyms Property has value although it should not");
					Assert.IsFalse(theGradeSynonyms.OneKnown.HasValue, "GradeSynonyms Property has value although it should not");
					Assert.IsFalse(theGradeSynonyms.Prompt.HasValue, "GradeSynonyms Property has value although it should not");
				}
			}

		}

		/// <summary>
		/// Set and Get Settings for SnoozeOptions.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsSnoozeOptionsTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false);
				ISettings theSettings = testLM.DefaultSettings;

				//Get Property Object
				ISnoozeOptions theSnoozeOptions = theSettings.SnoozeOptions;
				Assert.IsTrue(theSnoozeOptions is ISnoozeOptions, "GradeSynonyms Property gave back wrong type");
				//Test against the Property Object
				int? mySnoozeCards = 23;
				int? mySnoozeHigh = 12;
				int? mySnoozeLow = 9;
				int? mySnoozeRights = 2;
				int? mySnoozeTime = 23;

				theSnoozeOptions.IsCardsEnabled = true;
				theSnoozeOptions.IsRightsEnabled = true;
				theSnoozeOptions.IsTimeEnabled = true;
				theSnoozeOptions.SnoozeCards = mySnoozeCards;
				theSnoozeOptions.SnoozeHigh = mySnoozeHigh;
				theSnoozeOptions.SnoozeLow = mySnoozeLow;
				theSnoozeOptions.SnoozeRights = mySnoozeRights;
				theSnoozeOptions.SnoozeTime = mySnoozeTime;
				theSnoozeOptions.SnoozeMode = ESnoozeMode.QuitProgram;
				//Check if hasValue is valid
				Assert.IsTrue(theSnoozeOptions.IsCardsEnabled.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.IsRightsEnabled.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.IsTimeEnabled.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeCards.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeHigh.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeLow.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeRights.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeTime.HasValue, "SnoozeOptions Property has no value although it should");
				//Check if values are set correctly
				Assert.IsTrue(theSnoozeOptions.IsCardsEnabled.Value, "SnoozeOptions Property was not set correctly");
				Assert.IsTrue(theSnoozeOptions.IsRightsEnabled.Value, "SnoozeOptions Property was not set correctly");
				Assert.IsTrue(theSnoozeOptions.IsTimeEnabled.Value, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeCards, theSnoozeOptions.SnoozeCards, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeHigh, theSnoozeOptions.SnoozeHigh, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeLow, theSnoozeOptions.SnoozeLow, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeRights, theSnoozeOptions.SnoozeRights, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeTime, theSnoozeOptions.SnoozeTime, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<ESnoozeMode?>(ESnoozeMode.QuitProgram, theSnoozeOptions.SnoozeMode, "SnoozeOptions Property was not set correctly");



				mySnoozeCards = 9;
				mySnoozeHigh = 15;
				mySnoozeLow = 6;
				mySnoozeRights = 4;
				mySnoozeTime = 13;

				theSnoozeOptions.IsCardsEnabled = false;
				theSnoozeOptions.IsRightsEnabled = false;
				theSnoozeOptions.IsTimeEnabled = false;
				theSnoozeOptions.SnoozeCards = mySnoozeCards;
				theSnoozeOptions.SnoozeHigh = mySnoozeHigh;
				theSnoozeOptions.SnoozeLow = mySnoozeLow;
				theSnoozeOptions.SnoozeRights = mySnoozeRights;
				theSnoozeOptions.SnoozeTime = mySnoozeTime;
				theSnoozeOptions.SnoozeMode = ESnoozeMode.SendToTray;
				//Check if hasValue is valid
				Assert.IsTrue(theSnoozeOptions.IsCardsEnabled.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.IsRightsEnabled.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.IsTimeEnabled.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeCards.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeHigh.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeLow.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeRights.HasValue, "SnoozeOptions Property has no value although it should");
				Assert.IsTrue(theSnoozeOptions.SnoozeTime.HasValue, "SnoozeOptions Property has no value although it should");
				//Check if values are set correctly
				Assert.IsFalse(theSnoozeOptions.IsCardsEnabled.Value, "SnoozeOptions Property was not set correctly");
				Assert.IsFalse(theSnoozeOptions.IsRightsEnabled.Value, "SnoozeOptions Property was not set correctly");
				Assert.IsFalse(theSnoozeOptions.IsTimeEnabled.Value, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeCards, theSnoozeOptions.SnoozeCards, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeHigh, theSnoozeOptions.SnoozeHigh, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeLow, theSnoozeOptions.SnoozeLow, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeRights, theSnoozeOptions.SnoozeRights, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<int?>(mySnoozeTime, theSnoozeOptions.SnoozeTime, "SnoozeOptions Property was not set correctly");
				Assert.AreEqual<ESnoozeMode?>(ESnoozeMode.SendToTray, theSnoozeOptions.SnoozeMode, "SnoozeOptions Property was not set correctly");

				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					mySnoozeCards = null;
					mySnoozeHigh = null;
					mySnoozeLow = null;
					mySnoozeRights = null;
					mySnoozeTime = null;

					theSnoozeOptions.IsCardsEnabled = null;
					theSnoozeOptions.IsRightsEnabled = null;
					theSnoozeOptions.IsTimeEnabled = null;
					theSnoozeOptions.SnoozeCards = mySnoozeCards;
					theSnoozeOptions.SnoozeHigh = mySnoozeHigh;
					theSnoozeOptions.SnoozeLow = mySnoozeLow;
					theSnoozeOptions.SnoozeRights = mySnoozeRights;
					theSnoozeOptions.SnoozeTime = mySnoozeTime;
					theSnoozeOptions.SnoozeMode = ESnoozeMode.SendToTray;
					//Check if hasValue is valid
					Assert.IsFalse(theSnoozeOptions.IsCardsEnabled.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.IsRightsEnabled.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.IsTimeEnabled.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.SnoozeCards.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.SnoozeHigh.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.SnoozeLow.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.SnoozeRights.HasValue, "SnoozeOptions Property has value although it should not");
					Assert.IsFalse(theSnoozeOptions.SnoozeTime.HasValue, "SnoozeOptions Property has value although it should not");
				}

			}
		}



		/// <summary>
		/// Set and Get Settings for OtherProperties.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsOtherPropertiesTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser);
				ISettings theSettings = testLM.DefaultSettings;

				//AutoplayAudio Property
				theSettings.AutoplayAudio = true;
				Assert.IsTrue(theSettings.AutoplayAudio.HasValue, "AutoplayAudio Property has no value although it should");
				Assert.IsTrue(theSettings.AutoplayAudio.Value, "AutoplayAudio Property was not set correctly");
				theSettings.AutoplayAudio = false;
				Assert.IsTrue(theSettings.AutoplayAudio.HasValue, "AutoplayAudio Property has no value although it should");
				Assert.IsFalse(theSettings.AutoplayAudio.Value, "AutoplayAudio Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.AutoplayAudio = null;
					Assert.IsFalse(theSettings.AutoplayAudio.HasValue, "AutoplayAudio Property value although it should not");
				}

				//CaseSensitive Property
				theSettings.CaseSensitive = true;
				Assert.IsTrue(theSettings.CaseSensitive.HasValue, "CaseSensitive Property has no value although it should");
				Assert.IsTrue(theSettings.CaseSensitive.Value, "CaseSensitive Property was not set correctly");
				theSettings.CaseSensitive = false;
				Assert.IsTrue(theSettings.CaseSensitive.HasValue, "CaseSensitive Property has no value although it should");
				Assert.IsFalse(theSettings.CaseSensitive.Value, "CaseSensitive Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.CaseSensitive = null;
					Assert.IsFalse(theSettings.CaseSensitive.HasValue, "CaseSensitive Property value although it should not");
				}


				//ConfirmDemote Property
				theSettings.ConfirmDemote = true;
				Assert.IsTrue(theSettings.ConfirmDemote.HasValue, "ConfirmDemote Property has no value although it should");
				Assert.IsTrue(theSettings.ConfirmDemote.Value, "ConfirmDemote Property was not set correctly");
				theSettings.ConfirmDemote = false;
				Assert.IsTrue(theSettings.ConfirmDemote.HasValue, "ConfirmDemote Property has no value although it should");
				Assert.IsFalse(theSettings.ConfirmDemote.Value, "ConfirmDemote Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.ConfirmDemote = null;
					Assert.IsFalse(theSettings.ConfirmDemote.HasValue, "ConfirmDemote Property value although it should not");
				}


				//EnableCommentary Property
				theSettings.EnableCommentary = true;
				Assert.IsTrue(theSettings.EnableCommentary.HasValue, "EnableCommentary Property has no value although it should");
				Assert.IsTrue(theSettings.EnableCommentary.Value, "EnableCommentary Property was not set correctly");
				theSettings.EnableCommentary = false;
				Assert.IsTrue(theSettings.EnableCommentary.HasValue, "EnableCommentary Property has no value although it should");
				Assert.IsFalse(theSettings.EnableCommentary.Value, "EnableCommentary Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.EnableCommentary = null;
					Assert.IsFalse(theSettings.EnableCommentary.HasValue, "EnableCommentary Property value although it should not");
				}

				//CorrectOnTheFly Property
				theSettings.CorrectOnTheFly = true;
				Assert.IsTrue(theSettings.CorrectOnTheFly.HasValue, "CorrectOnTheFly Property has no value although it should");
				Assert.IsTrue(theSettings.CorrectOnTheFly.Value, "CorrectOnTheFly Property was not set correctly");
				theSettings.CorrectOnTheFly = false;
				Assert.IsTrue(theSettings.CorrectOnTheFly.HasValue, "CorrectOnTheFly Property has no value although it should");
				Assert.IsFalse(theSettings.CorrectOnTheFly.Value, "CorrectOnTheFly Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.CorrectOnTheFly = null;
					Assert.IsFalse(theSettings.CorrectOnTheFly.HasValue, "CorrectOnTheFly Property value although it should not");
				}

				//EnableTimer Property
				theSettings.EnableTimer = true;
				Assert.IsTrue(theSettings.EnableTimer.HasValue, "EnableTimer Property has no value although it should");
				Assert.IsTrue(theSettings.EnableTimer.Value, "EnableTimer Property was not set correctly");
				theSettings.EnableTimer = false;
				Assert.IsTrue(theSettings.EnableTimer.HasValue, "EnableTimer Property has no value although it should");
				Assert.IsFalse(theSettings.EnableTimer.Value, "EnableTimer Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.EnableTimer = null;
					Assert.IsFalse(theSettings.EnableTimer.HasValue, "EnableTimer Property value although it should not");
				}

				//RandomPool Property
				theSettings.RandomPool = true;
				Assert.IsTrue(theSettings.RandomPool.HasValue, "RandomPool Property has no value although it should");
				Assert.IsTrue(theSettings.RandomPool.Value, "RandomPool Property was not set correctly");
				theSettings.RandomPool = false;
				Assert.IsTrue(theSettings.RandomPool.HasValue, "RandomPool Property has no value although it should");
				Assert.IsFalse(theSettings.RandomPool.Value, "RandomPool Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.RandomPool = null;
					Assert.IsFalse(theSettings.RandomPool.HasValue, "RandomPool Property value although it should not");
				}

				//SelfAssessment Property
				theSettings.SelfAssessment = true;
				Assert.IsTrue(theSettings.SelfAssessment.HasValue, "SelfAssessment Property has no value although it should");
				Assert.IsTrue(theSettings.SelfAssessment.Value, "SelfAssessment Property was not set correctly");
				theSettings.SelfAssessment = false;
				Assert.IsTrue(theSettings.SelfAssessment.HasValue, "SelfAssessment Property has no value although it should");
				Assert.IsFalse(theSettings.SelfAssessment.Value, "SelfAssessment Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.SelfAssessment = null;
					Assert.IsFalse(theSettings.SelfAssessment.HasValue, "SelfAssessment Property value although it should not");
				}

				//ShowImages Property
				theSettings.ShowImages = true;
				Assert.IsTrue(theSettings.ShowImages.HasValue, "ShowImages Property has no value although it should");
				Assert.IsTrue(theSettings.ShowImages.Value, "ShowImages Property was not set correctly");
				theSettings.ShowImages = false;
				Assert.IsTrue(theSettings.ShowImages.HasValue, "ShowImages Property has no value although it should");
				Assert.IsFalse(theSettings.ShowImages.Value, "ShowImages Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.ShowImages = null;
					Assert.IsFalse(theSettings.ShowImages.HasValue, "ShowImages Property value although it should not");
				}

				//ShowStatistics Property
				theSettings.ShowStatistics = true;
				Assert.IsTrue(theSettings.ShowStatistics.HasValue, "ShowStatistics Property has no value although it should");
				Assert.IsTrue(theSettings.ShowStatistics.Value, "ShowStatistics Property was not set correctly");
				theSettings.ShowStatistics = false;
				Assert.IsTrue(theSettings.ShowStatistics.HasValue, "ShowStatistics Property has no value although it should");
				Assert.IsFalse(theSettings.ShowStatistics.Value, "ShowStatistics Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.ShowStatistics = null;
					Assert.IsFalse(theSettings.ShowStatistics.HasValue, "ShowStatistics Property value although it should not");
				}

				//SkipCorrectAnswers Property
				theSettings.SkipCorrectAnswers = true;
				Assert.IsTrue(theSettings.SkipCorrectAnswers.HasValue, "SkipCorrectAnswers Property has no value although it should");
				Assert.IsTrue(theSettings.SkipCorrectAnswers.Value, "SkipCorrectAnswers Property was not set correctly");
				theSettings.SkipCorrectAnswers = false;
				Assert.IsTrue(theSettings.SkipCorrectAnswers.HasValue, "SkipCorrectAnswers Property has no value although it should");
				Assert.IsFalse(theSettings.SkipCorrectAnswers.Value, "SkipCorrectAnswers Property was not set correctly");
				if (TestInfrastructure.SupportsNullableValues(TestContext))
				{
					theSettings.SkipCorrectAnswers = null;
					Assert.IsFalse(theSettings.SkipCorrectAnswers.HasValue, "SkipCorrectAnswers Property value although it should not");
				}

				//StripChars Property
				String myStripChars = "\\#�l";
				theSettings.StripChars = myStripChars;
				Assert.AreEqual<string>(myStripChars, theSettings.StripChars, "StripChars Property was not set correctly");
				myStripChars = ":,.sS";
				theSettings.StripChars = myStripChars;
				Assert.AreEqual<string>(myStripChars, theSettings.StripChars, "StripChars Property was not set correctly");

				//QuestionCaption Property
				String myQuestionCaption = "\\#�l";
				theSettings.QuestionCaption = myQuestionCaption;
				Assert.AreEqual<string>(myQuestionCaption, theSettings.QuestionCaption, "QuestionCaption Property was not set correctly");
				myQuestionCaption = ":,.sS";
				theSettings.QuestionCaption = myQuestionCaption;
				Assert.AreEqual<string>(myQuestionCaption, theSettings.QuestionCaption, "QuestionCaption Property was not set correctly");

				//AnswerCaption Property
				String myAnswerCaption = "\\#�l";
				theSettings.AnswerCaption = myAnswerCaption;
				Assert.AreEqual<string>(myAnswerCaption, theSettings.AnswerCaption, "AnswerCaption Property was not set correctly");
				myAnswerCaption = ":,.sS";
				theSettings.AnswerCaption = myAnswerCaption;
				Assert.AreEqual<string>(myAnswerCaption, theSettings.AnswerCaption, "AnswerCaption Property was not set correctly");

				//Culture Property
				CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				CultureInfo ai = (CultureInfo)cultures.GetValue(TestInfrastructure.Random.Next(1, cultures.Length));
				CultureInfo qi = (CultureInfo)cultures.GetValue(TestInfrastructure.Random.Next(1, cultures.Length));
				theSettings.AnswerCulture = ai;
				Assert.AreEqual<CultureInfo>(ai, theSettings.AnswerCulture, "The culture does not match for AnswerCulture.");
				theSettings.QuestionCulture = qi;
				Assert.AreEqual<CultureInfo>(qi, theSettings.QuestionCulture, "The culture does not match for QuestionCulture.");

				//CommentarySounds
				CommentarySoundIdentifier nr1 = CommentarySoundIdentifier.Create(Side.Answer, ECommentarySoundType.Wrong);
				CommentarySoundIdentifier nr2 = CommentarySoundIdentifier.Create(Side.Question, ECommentarySoundType.RightStandAlone);
				string testAudio = Path.Combine(Path.GetTempPath(), "homer.wav");
				string testAudio2 = Path.Combine(Path.GetTempPath(), "homer2.wav");

				//prepare test files
				byte[] buffer = new byte[Properties.Resources.homer_wav.Length];
				Properties.Resources.homer_wav.Read(buffer, 0, Convert.ToInt32(Properties.Resources.homer_wav.Length));
				File.WriteAllBytes(testAudio, buffer);
				File.WriteAllBytes(testAudio2, buffer);

				IMedia media = testLM.CreateMedia(EMedia.Audio, testAudio, true, true, true);
				IMedia media2 = testLM.CreateMedia(EMedia.Audio, testAudio2, true, true, true);

				Dictionary<CommentarySoundIdentifier, IMedia> commentarySounds = theSettings.CommentarySounds;
				commentarySounds[nr1] = media;
				commentarySounds[nr2] = media2;
				theSettings.CommentarySounds = commentarySounds;

				Assert.IsTrue(theSettings.CommentarySounds.ContainsKey(nr1), "nr1 not available in Dictionary, although it should");
				Assert.IsTrue(theSettings.CommentarySounds.ContainsKey(nr2), "nr2 not available in Dictionary, although it should");

				if (testLM.IsDB)
				{
					Assert.AreEqual(media.Filename, theSettings.CommentarySounds[nr1].Filename, "CommentarySounds Property was not set correctly (name compared)");
					Assert.AreEqual(media2.Filename, theSettings.CommentarySounds[nr2].Filename, "CommentarySounds Property was not set correctly (name compared)");
				}
				else
				{
					Assert.AreEqual((new FileInfo(media.Filename)).Length, (new FileInfo(theSettings.CommentarySounds[nr1].Filename)).Length,
						"CommentarySounds Property was not set correctly (size compared)");
					Assert.AreEqual((new FileInfo(media2.Filename)).Length, (new FileInfo(theSettings.CommentarySounds[nr2].Filename)).Length,
						 "CommentarySounds Property was not set correctly (size compared)");

					Assert.IsTrue(theSettings.CommentarySounds[nr1].Filename.StartsWith(testLM.MediaDirectory) ||
						theSettings.CommentarySounds[nr1].Filename.StartsWith(Path.Combine(Path.GetDirectoryName(testLM.Connection), testLM.MediaDirectory)),
						"CommentarySound wasn't copyed to Media dir!");
					Assert.IsTrue(theSettings.CommentarySounds[nr2].Filename.StartsWith(testLM.MediaDirectory) ||
						theSettings.CommentarySounds[nr2].Filename.StartsWith(Path.Combine(Path.GetDirectoryName(testLM.Connection), testLM.MediaDirectory)),
						"CommentarySound wasn't copyed to Media dir!");
				}

				File.Delete(testAudio);
				File.Delete(testAudio2);

			}
		}

		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsSelectedChaptersNullTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
				{
					// add some chapters
					for (int i = 0; i < 10; i++)
					{
						IChapter chapter = testLM.Chapters.AddNew();
						chapter.Title = "Chapter - " + Guid.NewGuid().ToString();
						chapter.Description = "Description - " + Guid.NewGuid().ToString();
					}

					testLM.DefaultSettings.SelectedLearnChapters = null;
					Assert.AreEqual<int>(0, testLM.DefaultSettings.SelectedLearnChapters.Count, "The number of learning chapters should be 0.");
				}
			}
		}

		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsSelectedChaptersEmptyTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
				{
					// add some chapters
					for (int i = 0; i < 10; i++)
					{
						IChapter chapter = testLM.Chapters.AddNew();
						chapter.Title = "Chapter - " + Guid.NewGuid().ToString();
						chapter.Description = "Description - " + Guid.NewGuid().ToString();
					}

					testLM.DefaultSettings.SelectedLearnChapters = new List<int>();
					Assert.AreEqual<int>(0, testLM.DefaultSettings.SelectedLearnChapters.Count, "The number of learning chapters should be 0.");
				}
			}
		}

		/// <summary>
		/// Set and Get Settings for OtherProperties.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-11</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsSelectedChaptersTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
				{
					List<int> chapterIds = new List<int>();
					// add some chapters
					for (int i = 0; i < 10; i++)
					{
						IChapter chapter = testLM.Chapters.AddNew();
						chapter.Title = "Chapter - " + Guid.NewGuid().ToString();
						chapter.Description = "Description - " + Guid.NewGuid().ToString();
						chapterIds.Add(chapter.Id);
					}

					ISettings settings = testLM.DefaultSettings;
					List<int> selChapterIds = new List<int>();
					for (int i = 0; i < TestInfrastructure.Random.Next(1, 10); i++)
						selChapterIds.Add(chapterIds[i]);
					settings.SelectedLearnChapters = selChapterIds;
					Assert.AreEqual<int>(selChapterIds.Count, testLM.DefaultSettings.SelectedLearnChapters.Count, "The number of selected learning chapters does not match.");

					foreach (int id in selChapterIds)
						Assert.IsTrue(settings.SelectedLearnChapters.Contains(id), String.Format("Expected chapter {0} not found.", id));
				}

			}
		}


		/// <summary>
		/// Tests the logo property.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-08-21</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void ISettingsLogoTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				IMedia media;
				int lmid;

				LMConnectionParameter param = new LMConnectionParameter();
				param.TestContext = TestContext;
				param.RepositoryName = TestContext.TestName;
				param.Callback = TestInfrastructure.GetAdminUser;
				param.ConnectionType = string.Empty;
				param.IsProtected = false;
				param.LearningModuleId = -1;
				param.Password = string.Empty;
				param.standAlone = false;

				using (IDictionary testLM = TestInfrastructure.GetLMConnection(param))
				{
					lmid = testLM.Id;
					string testImage = TestInfrastructure.GetTestImage();
					media = testLM.CreateMedia(EMedia.Image, testImage, true, true, true);
					testLM.DefaultSettings.Logo = media;
					testLM.Save();
				}
				param.LearningModuleId = lmid;
				using (IDictionary testLM = TestInfrastructure.GetLMConnection(param))
				{
					IMedia logo = testLM.DefaultSettings.Logo;
					Assert.IsNotNull(logo, "LM does not contain a logo Media file.");
					Assert.AreEqual<IMedia>(media, logo, "Logo Media was not persistent.");
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}
	}
}
