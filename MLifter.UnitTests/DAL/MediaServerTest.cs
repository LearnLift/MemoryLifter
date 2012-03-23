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
using MLifter.DAL.DB.DbMediaServer;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.DAL.Interfaces;
using Npgsql;
using System.IO;
using NpgsqlTypes;
using System.Collections;
using System.Diagnostics;

using System.Threading;
using System.Net;
using MLifter.DAL.Tools;

namespace MLifterTest.DAL
{
	[TestClass]
	public class MediaServerTest
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

		public MediaServerTest()
		{
		}

		/// <summary>
		/// Tests if the Instance Method gives back an Instance and if it is of correct type.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void MediaServerTestGetServerInstancTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				if (!(TestContext.DataRow["type"].ToString().ToLower() == "file"))
				{
					using (MLifter.DAL.Interfaces.IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
					{
						DbMediaServer myMediaServer = DbMediaServer.Instance(testLM.Parent);
						Assert.IsNotNull(myMediaServer, "DbMediaServer class returned a null reference");
						Assert.IsInstanceOfType(myMediaServer, typeof(DbMediaServer), "DbMediaServer Instance is not of the correct type");
					}
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Create some test data, puts it into the db and retrieves it back via MediaServer
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void MediaServerTestGetMediaTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				if (!(TestContext.DataRow["type"].ToString().ToLower() == "file"))
				{
					using (MLifter.DAL.Interfaces.IDictionary testLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
					{

						Dictionary<int, int> mediaIds = new Dictionary<int, int>(); //key: idCounter, value: mediaId
						Dictionary<string, byte[]> testData = new Dictionary<string, byte[]>();
						//Filling DB with Test Data
						for (int i = 0; i < TestInfrastructure.Loopcount; i++)
						{
							string testMedia = TestInfrastructure.GetTestImage();
							testData.Add(testMedia, File.ReadAllBytes(testMedia));
						}
						int idCounter = 0;
						//Putting Test Data into db
						foreach (string testMedia in testData.Keys)
						{
							ICard card = testLM.Cards.Create();
							IMedia media = card.AddMedia(card.CreateMedia(EMedia.Image, testMedia, true, true, false), Side.Question);
							mediaIds[idCounter++] = media.Id;
						}

						//Create a new MediaServerInstance
						DbMediaServer myMediaServer = DbMediaServer.Instance(8080, testLM.Parent);
						myMediaServer.Start();

						//Wait until server is ready
						while (!myMediaServer.IsReady) ;

						//Verify
						idCounter = 0;
						foreach (string testMedia in testData.Keys)
						{
							Uri theServerUri_itme = DbMediaServer.Instance(testLM.Parent).GetMediaURI(mediaIds[idCounter++]);
							WebClient client = new WebClient();
							byte[] downloadData_item = client.DownloadData(theServerUri_itme.ToString());
							byte[] b = testData[testMedia];

							for (int i = 0; i < b.Length; i++)
							{
								Assert.IsTrue(b[i].Equals(downloadData_item[i]), "Test Data does not match");
							}
						}

						//Stop the Media Server again
						myMediaServer.Stop();
					}
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}


		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public sttheServerUri_itme0atic void MyClassInitialize(TestContext testContext) { }
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
	}
}
