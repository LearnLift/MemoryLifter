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
using MLifter.DAL.Interfaces;
using System.IO;

namespace MLifterTest.DAL
{
    [TestClass]
    public class IExtensionTest
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

        public IExtensionTest()
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
        /// Adds a new extension to a database (without binding to a LM)
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-05</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IExtensionTestAddExtension()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) != "File")
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    //add a new extension
                    byte[] bufCheck = new byte[5];
                    byte[] bufOriginal = new byte[5] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };

                    foreach (IExtension ext in writeLM.Parent.CurrentUser.List().Extensions)
                        writeLM.Parent.CurrentUser.List().DeleteExtension(ext);

                    IExtension extension = writeLM.Parent.CurrentUser.List().ExtensionFactory();
                    extension.Type = ExtensionType.Skin;
                    extension.Name = "TheurySkin";
                    extension.Version = new Version("1.0.0.0");
                    extension.StartFile = "TheurySkin.style";
                    extension.Data = new System.IO.MemoryStream(bufOriginal);

                    ////////////////////////////////////////////////////////////////

                    Assert.AreEqual<int>(1, writeLM.Parent.CurrentUser.List().Extensions.Count, "The current database contains more than one exception");
                    Assert.AreEqual<int>(0, writeLM.Extensions.Count, "The current LearningModule must not contain an Extension");

                    IExtension checkExtension = writeLM.Parent.CurrentUser.List().Extensions[0];
                    Assert.AreEqual<ExtensionType>(ExtensionType.Skin, checkExtension.Type, "The ExtensionType is wrong");
                    Assert.AreEqual<string>("TheurySkin", checkExtension.Name, "The ExtensionName is wrong");
                    Assert.AreEqual<Version>(new Version("1.0.0.0"), checkExtension.Version, "The ExtensionVersion is wrong");
                    Assert.AreEqual<string>("TheurySkin.style", checkExtension.StartFile, "The ExtensionStartFile is wrong");

                    checkExtension.Data.Read(bufCheck, 0, bufOriginal.Length);
                    for (int i = 0; i < bufCheck.Length; i++)
                        Assert.AreEqual<byte>(bufCheck[i], bufOriginal[i], "The ExtensionData is wrong");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Adds a new extension to a database (with binding to a LM)
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-06</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IExtensionTestAddExtensionToLM()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) != "File")
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    byte[] bufCheck = new byte[5];
                    byte[] bufOriginal = new byte[5] { (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };

                    ExtensionAction extensionAction = new ExtensionAction() { Kind = ExtensionActionKind.Install, Execution = ExtensionActionExecution.Once };

                    IExtension extension = writeLM.Parent.CurrentUser.List().ExtensionFactory();
                    extension.Type = ExtensionType.Skin;
                    extension.Name = "TheurySkin";
                    extension.Version = new Version("1.0.0.0");
                    extension.StartFile = "TheurySkin.style";
                    extension.Data = new System.IO.MemoryStream(bufOriginal);
                    extension.Actions.Add(extensionAction);

                    writeLM.Extensions.Add(extension);

                    ////////////////////////////////////////////////////////////////

                    Assert.AreEqual<int>(1, writeLM.Extensions.Count, "The Extension is not correctly saved to the database/LM");
                    Assert.AreEqual<ExtensionType>(ExtensionType.Skin, writeLM.Extensions[0].Type, "The ExtensionType is wrong");
                    Assert.AreEqual<string>("TheurySkin", writeLM.Extensions[0].Name, "The ExtensionName is wrong");
                    Assert.AreEqual<Version>(new Version("1.0.0.0"), writeLM.Extensions[0].Version, "The ExtensionVersion is wrong");
                    Assert.AreEqual<string>("TheurySkin.style", writeLM.Extensions[0].StartFile, "The ExtensionStartFile is wrong");
                    Assert.AreEqual<ExtensionAction>(extensionAction, writeLM.Extensions[0].Actions[0], "The ExtensionAction is wrong");

                    writeLM.Extensions[0].Data.Read(bufCheck, 0, bufOriginal.Length);
                    for (int i = 0; i < bufCheck.Length; i++)
                        Assert.AreEqual<byte>(bufCheck[i], bufOriginal[i], "The ExtensionData is wrong");

                    ////////////////////////////////////////////////////////////////

                    writeLM.Extensions[0].Actions.RemoveAt(0);
                    Assert.AreEqual<int>(0, writeLM.Extensions[0].Actions.Count, "Number of ExtensionActions is wrong");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }

        /// <summary>
        /// Deletes an Extension 
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-06</remarks>
        [TestMethod]
        [TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
        public void IExtensionTestDeleteExtension()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) != "File")
            {
                using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, TestInfrastructure.GetAdminUser))
                {
                    writeLM.Extensions.Add(writeLM.Parent.CurrentUser.List().ExtensionFactory());       //Creates a new extension
                    writeLM.Extensions.Add(writeLM.Parent.CurrentUser.List().ExtensionFactory());       //Creates a new extension
                    writeLM.Extensions.Add(writeLM.Parent.CurrentUser.List().ExtensionFactory());       //Creates a new extension

                    Assert.AreEqual<int>(3, writeLM.Extensions.Count, "The number of created Extensions is wrong");
                    writeLM.Extensions.RemoveAt(0);
                    Assert.AreEqual<int>(2, writeLM.Extensions.Count, "The number of created Extensions is wrong");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }
    }
}
