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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Diagnostics;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary Description for ExporterTests
    /// </summary>
    [TestClass]
    public class ExporterTests
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

        public ExporterTests()
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

        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void ExportHelperTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary target = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    IChapter chapter = target.Chapters.AddNew();

                    List<int> cardIds = new List<int>();
                    List<string> testImages = new List<string>();
                    List<string> testAudios = new List<string>();
                    List<string> largeTestAudios = new List<string>();
                    List<string> testVideos = new List<string>();
                    List<string> exportFiles = new List<string>();
                    for (int i = 0; i < 10; i++)
                    {
                        ICard card = target.Cards.AddNew();
                        card.Chapter = chapter.Id;
                        cardIds.Add(card.Id);
                        string testImage = TestInfrastructure.GetTestImage();
                        IMedia image = card.CreateMedia(EMedia.Image, testImage, true, true, true);
                        card.AddMedia(image, Side.Question);
                        card.AddMedia(image, Side.Answer);
                        string testAudio = TestInfrastructure.GetTestAudio();
                        IMedia audio = card.CreateMedia(EMedia.Audio, testAudio, true, true, false);
                        card.AddMedia(audio, Side.Question);
                        card.AddMedia(audio, Side.Answer);
                        string largeTestAudio = TestInfrastructure.GetLargeTestAudio();
                        IMedia exaudio = card.CreateMedia(EMedia.Audio, largeTestAudio, true, false, true);
                        card.AddMedia(exaudio, Side.Question);
                        card.AddMedia(exaudio, Side.Answer);
                        string testVideo = TestInfrastructure.GetTestVideo();
                        IMedia video = card.CreateMedia(EMedia.Video, testVideo, true, false, false);
                        card.AddMedia(video, Side.Question);
                        card.AddMedia(video, Side.Answer);

                        testImages.Add(testImage);
                        testAudios.Add(testAudio);
                        largeTestAudios.Add(largeTestAudio);
                        testVideos.Add(testVideo);
                    }

                    MLifter.DAL.ImportExport.Exporter.ExportHelper exportHelper = new MLifter.DAL.ImportExport.Exporter.ExportHelper(target, Path.Combine(Path.GetTempPath(), "export.csv"));

                    for (int j = 0; j < target.Cards.Count; j++)
                    {
                        ICard card = target.Cards.Get(cardIds[j]);
                        foreach (IMedia media in card.QuestionMedia)
                        {
                            string fileName = Path.Combine(Path.GetTempPath(), exportHelper.GetLocalFile(media.Filename, media.MediaType.ToString()));
                            string fileName2 = String.Empty;
                            exportFiles.Add(fileName);
                            switch (media.MediaType)
                            {
                                case EMedia.Audio:
                                    if (media.Default.Value)
                                        fileName2 = testAudios[j];
                                    else if (media.Example.Value)
                                        fileName2 = largeTestAudios[j];
                                    else
                                        continue;
                                    break;
                                case EMedia.Image:
                                    fileName2 = testImages[j];
                                    break;
                                case EMedia.Video:
                                    fileName2 = testVideos[j];
                                    break;
                                default:
                                    continue;
                            }
                            byte[] eb = File.ReadAllBytes(fileName);
                            byte[] ib = File.ReadAllBytes(fileName2);

                            Assert.AreEqual<int>(ib.Length, eb.Length, String.Format("{0} and {1} are different in size! (loop={2})", fileName, fileName2, j));
                        }
                    }

                    foreach (string file in testImages)
                        try { File.Delete(file); }
                        catch { }
                    foreach (string file in testAudios)
                        try { File.Delete(file); }
                        catch { }
                    foreach (string file in largeTestAudios)
                        try { File.Delete(file); }
                        catch { }
                    foreach (string file in testVideos)
                        try { File.Delete(file); }
                        catch { }
                    foreach (string file in exportFiles)
                        try { File.Delete(file); }
                        catch { }
                }
            }

            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
