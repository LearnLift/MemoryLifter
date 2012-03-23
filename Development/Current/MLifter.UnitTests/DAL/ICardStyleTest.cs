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
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.DAL.Interfaces;
using System.Net;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary Description for ICardStyleTest
    /// </summary>
    [TestClass]
    public class ICardStyleTest
    {
        private static Regex cssUrlEntry = new Regex(@"url\((?<url>[^\)]+)\)");

        public ICardStyleTest()
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

        /// <summary>
        /// Is the card style clone test.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-01-21</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardStyleCloneTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICardStyle style1 = writeLM.CreateCardStyle();
                    style1.Answer.BackgroundColor = Color.Red;
                    style1.Answer.FontFamily = new FontFamily("Arial");

                    writeLM.UserSettings.Style = style1;

                    ICardStyle style2 = writeLM.UserSettings.Style.Clone();

                    //Check if style1 and style2 are equal
                    Assert.AreEqual<Color>(writeLM.UserSettings.Style.Answer.BackgroundColor, style2.Answer.BackgroundColor, "ICardStyle.Clone does not clone the original instance");
                    Assert.AreEqual<FontFamily>(writeLM.UserSettings.Style.Answer.FontFamily, style2.Answer.FontFamily, "ICardStyle.Clone does not clone the original instance");

                    style2.Answer.BackgroundColor = Color.Blue;
                    style2.Answer.FontFamily = new FontFamily("Courier New");

                    //Check if style 1 and style2 are independent
                    Assert.AreNotEqual<Color>(writeLM.UserSettings.Style.Answer.BackgroundColor, style2.Answer.BackgroundColor, "ICardStyle.Clone does not make an independent copy of the original instance");
                    Assert.AreNotEqual<FontFamily>(writeLM.UserSettings.Style.Answer.FontFamily, style2.Answer.FontFamily, "ICardStyle.Clone does not make an independent copy of the original instance");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }


        /// <summary>
        /// Is the card style save styles test.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-01-21</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardStyleSaveStylesTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICardStyle style = writeLM.CreateCardStyle();
                    ICard card = writeLM.Cards.AddNew();

                    style.Answer.BackgroundColor = Color.Red;
                    style.Answer.FontFamily = new FontFamily("Arial");
                    card.Settings.Style = style;

                    //Check if style save the settings
                    Assert.AreEqual<Color>(card.Settings.Style.Answer.BackgroundColor, Color.Red, "ICardStyle.Clone does not clone the original instance");
                    Assert.AreEqual<FontFamily>(card.Settings.Style.Answer.FontFamily, new FontFamily("Arial"), "ICardStyle.Clone does not clone the original instance");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }


        /// <summary>
        /// Is the card style empty values test.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-01-21</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardStyleEmptyValuesTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICardStyle style = writeLM.CreateCardStyle();
                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;

                    style.Answer.BackgroundColor = new Color();
                    card.Settings.Style = style;

                    ICard card2 = writeLM.Cards.Get(card.Id);

                    Assert.AreEqual<string>(card.Settings.Style.Answer.BackgroundColor.ToString(), card2.Settings.Style.Answer.BackgroundColor.ToString(), "The Styles (Color) is not empty");
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the card style test for CSS watermark images.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-03-12</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ICardStyleWatermarkTest01()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext) && !(TestContext.DataRow["type"].ToString().ToLower() == "file"))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICardStyle style = writeLM.CreateCardStyle();
                    ICard card = writeLM.Cards.AddNew();
                    card.Chapter = chapter.Id;

                    string image = TestInfrastructure.GetTestImage();
                    string image2 = TestInfrastructure.GetTestImage();
                    string image3 = TestInfrastructure.GetTestImage();
                    Uri imageUri = new Uri(image, UriKind.Absolute);
                    Uri imageUri2 = new Uri(image2, UriKind.Absolute);
                    style.Question.OtherElements.Add("background", String.Format("url({0}) repeat-x", imageUri.AbsoluteUri));
                    style.Answer.OtherElements.Add("background-image", String.Format("url({0})", imageUri2.AbsolutePath));
                    style.AnswerExample.OtherElements.Add("background-image", String.Format("url({0})", TestInfrastructure.GetTestImage()));
                    card.Settings.Style = style;

                    writeLM.Save();

                    ICard card2 = writeLM.Cards.Get(card.Id);

                    string css = card2.Settings.Style.ToString();

                    MatchCollection mc = cssUrlEntry.Matches(css);
                    if (mc.Count != 3)
                        Assert.Fail("Invalid number of media objects found! (Should be 3)");
                    foreach (Match m in mc)
                    {
                        string url = m.Groups["url"].Value.Trim(new char[] { '"', '\'' });
                        Uri uri;
                        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            uri = new Uri(url);
                            WebClient client = new WebClient();
                            byte[] data = client.DownloadData(uri);
                            if (data.Length <= 0)
                                Assert.Fail("Invalid media file size! (0)");
                        }
                        else
                        {
                            Assert.Fail("Invalid media URI found! ({0})", url);
                        }
                    }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
