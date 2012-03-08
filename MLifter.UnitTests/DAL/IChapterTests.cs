using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary Description for IChapter
    /// </summary>
    [TestClass]
    public class IChapterTests
    {
        public IChapterTests()
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
        public void IChapterCheckProperties()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "Title";
                    chapter.Description = "Description";

                    for (int j = 0; j < 10; j++)
                    {
                        ICard card = writeLM.Cards.AddNew();
                        card.QuestionExample.AddWord(card.QuestionExample.CreateWord("Question " + j.ToString(), WordType.Sentence, true));
                        card.AnswerExample.AddWord(card.AnswerExample.CreateWord("Answer " + j.ToString(), WordType.Sentence, true));

                        card.Chapter = chapter.Id;
                        if (j == 0)
                            card.Active = false;
                    }

                    Assert.AreEqual<string>(chapter.Title, "Title", "IChapter does not save the Title");
                    Assert.AreEqual<string>(chapter.Description, "Description", "IChapter does not save the Description");

                    Assert.AreEqual<int>(chapter.Size, 10, "IChapter does not contain the correct number (total) of cards. ");
                    if (TestContext.DataRow["Type"].ToString() == "File")
                        Assert.AreEqual<int>(chapter.ActiveSize, 9, "IChapter does not contain the correct number (active) of cards. ");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChapterStyle()
        {
            //Todo: OtherElements, ITextStyle.ToString()
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    chapter.Title = "Chapter xyz";
                    ICardStyle style = chapter.CreateCardStyle();

                    ITextStyle defaultTextStyle = style.Answer;     //just for init

                    defaultTextStyle.BackgroundColor = Color.Red;
                    defaultTextStyle.FontFamily = new FontFamily("Arial");
                    defaultTextStyle.FontSize = 8;
                    defaultTextStyle.FontSizeUnit = FontSizeUnit.Pixel;
                    defaultTextStyle.FontStyle = CSSFontStyle.Bold;
                    defaultTextStyle.ForeColor = Color.Red;
                    defaultTextStyle.HorizontalAlign = HorizontalAlignment.Center;
                    defaultTextStyle.VerticalAlign = VerticalAlignment.Bottom;
                    defaultTextStyle.OtherElements.Add("key1", "value1");

                    style.Answer = defaultTextStyle;
                    style.AnswerCorrect = defaultTextStyle;
                    style.AnswerExample = defaultTextStyle;
                    style.AnswerWrong = defaultTextStyle;
                    style.Question = defaultTextStyle;
                    style.QuestionExample = defaultTextStyle;

                    if (chapter.Settings == null)
                        chapter.Settings = writeLM.CreateSettings();        //only occurs in DB mode
                    chapter.Settings.Style = style;      //assign Style to chapter

                    //Check
                    IChapter chap = writeLM.Chapters.Find("Chapter xyz");

                    ITextStyle[] checkStyle = new ITextStyle[6];
                    checkStyle[0] = chapter.Settings.Style.Answer;
                    checkStyle[1] = chapter.Settings.Style.AnswerCorrect;
                    checkStyle[2] = chapter.Settings.Style.AnswerExample;
                    checkStyle[3] = chapter.Settings.Style.AnswerWrong;
                    checkStyle[4] = chapter.Settings.Style.Question;
                    checkStyle[5] = chapter.Settings.Style.QuestionExample;

                    foreach (ITextStyle sty in checkStyle)
                    {
                        Assert.AreEqual(Color.Red, sty.BackgroundColor, "Style BackgroundColor was not saved to ICardStyle()");
                        Assert.AreEqual(new FontFamily("Arial").Name, sty.FontFamily.Name, "Style FontFamily was not saved to ICardStyle()");
                        Assert.AreEqual(8, sty.FontSize, "Style FontSize was not saved to ICardStyle()");
                        Assert.AreEqual(FontSizeUnit.Pixel, sty.FontSizeUnit, "Style FontSizeUnit was not saved to ICardStyle()");
                        Assert.AreEqual(CSSFontStyle.Bold, sty.FontStyle, "Style FontStyle was not saved to ICardStyle()");
                        Assert.AreEqual(Color.Red, sty.ForeColor, "Style ForeColor was not saved to ICardStyle()");
                        Assert.AreEqual(HorizontalAlignment.Center, sty.HorizontalAlign, "Style HorizontalAlign was not saved to ICardStyle()");
                        Assert.AreEqual(VerticalAlignment.Bottom, sty.VerticalAlign, "Style VerticalAlign was not saved to ICardStyle()");
                        Assert.AreEqual("value1", sty.OtherElements["key1"], "Style OtherElements(key, value) was not saved to ICardStyle()");
                    }
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }


        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChapterCreateCardStyle()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    Assert.IsInstanceOfType(chapter.CreateCardStyle(), typeof(ICardStyle), "CreateCardStyle() does not return the correct format!");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Is the chapter style to string.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-03-13</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IChapterStyleToString()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = writeLM.Chapters.AddNew();
                    ICardStyle style = chapter.CreateCardStyle();

                    ITextStyle defaultTextStyle = style.Answer;     //just for init

                    defaultTextStyle.BackgroundColor = Color.Red;
                    defaultTextStyle.FontFamily = new FontFamily("Arial");
                    defaultTextStyle.FontSize = 8;
                    defaultTextStyle.FontSizeUnit = FontSizeUnit.Pixel;
                    defaultTextStyle.FontStyle = CSSFontStyle.Bold;
                    defaultTextStyle.ForeColor = Color.Red;
                    defaultTextStyle.HorizontalAlign = HorizontalAlignment.Center;
                    defaultTextStyle.VerticalAlign = VerticalAlignment.Bottom;
                    defaultTextStyle.OtherElements.Add("key1", "value1");

                    style.Answer = defaultTextStyle;
                    style.AnswerCorrect = defaultTextStyle;
                    style.AnswerExample = defaultTextStyle;
                    style.AnswerWrong = defaultTextStyle;
                    style.Question = defaultTextStyle;
                    style.QuestionExample = defaultTextStyle;

                    if (chapter.Settings == null)
                        chapter.Settings = writeLM.CreateSettings();
                    chapter.Settings.Style = style;

                    //This is just a simple fulltext comparison
                    string output = "\n.answer\n{\n\tcolor:\t\t\t\t#ff0000;\n\tbackground-color:\t#ff0000;\n\tfont-family:\t\tArial;\n\tfont-weight:\t\tbold;\n\tText-decoration:none;\n\tfont-size:\t\t\t8px;\n\tvertical-align:\t\tbottom;\n\tText-align:\t\t\tcenter;\n\tkey1:\t\tvalue1;\n}\n\n.answer\n{\n\tcolor:\t\t\t\t#ff0000;\n\tbackground-color:\t#ff0000;\n\tfont-family:\t\tArial;\n\tfont-weight:\t\tbold;\n\tText-decoration:none;\n\tfont-size:\t\t\t8px;\n\tvertical-align:\t\tbottom;\n\tText-align:\t\t\tcenter;\n\tkey1:\t\tvalue1;\n}\n\n.answer\n{\n\tcolor:\t\t\t\t#ff0000;\n\tbackground-color:\t#ff0000;\n\tfont-family:\t\tArial;\n\tfont-weight:\t\tbold;\n\tText-decoration:none;\n\tfont-size:\t\t\t8px;\n\tvertical-align:\t\tbottom;\n\tText-align:\t\t\tcenter;\n\tkey1:\t\tvalue1;\n}\n\n.answer\n{\n\tcolor:\t\t\t\t#ff0000;\n\tbackground-color:\t#ff0000;\n\tfont-family:\t\tArial;\n\tfont-weight:\t\tbold;\n\tText-decoration:none;\n\tfont-size:\t\t\t8px;\n\tvertical-align:\t\tbottom;\n\tText-align:\t\t\tcenter;\n\tkey1:\t\tvalue1;\n}\n\n.answer\n{\n\tcolor:\t\t\t\t#ff0000;\n\tbackground-color:\t#ff0000;\n\tfont-family:\t\tArial;\n\tfont-weight:\t\tbold;\n\tText-decoration:none;\n\tfont-size:\t\t\t8px;\n\tvertical-align:\t\tbottom;\n\tText-align:\t\t\tcenter;\n\tkey1:\t\tvalue1;\n}\n\n.answer\n{\n\tcolor:\t\t\t\t#ff0000;\n\tbackground-color:\t#ff0000;\n\tfont-family:\t\tArial;\n\tfont-weight:\t\tbold;\n\tText-decoration:none;\n\tfont-size:\t\t\t8px;\n\tvertical-align:\t\tbottom;\n\tText-align:\t\t\tcenter;\n\tkey1:\t\tvalue1;\n}\n";
                    Assert.AreEqual<string>(output.ToLower().Trim(), chapter.Settings.Style.ToString().ToLower().Trim());
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
