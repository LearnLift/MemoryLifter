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
using System.Drawing;

namespace MLifterTest.DAL
{
    /// <summary>
    /// Summary description for ICardStyleTest
    /// </summary>
    [TestClass]
    public class OpenUserProfilTests
    {
        public OpenUserProfilTests()
        {
            //
            // TODO: Add constructor logic here
            //
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
        [TestProperty("TestCategory", "Developer"),
            DataSource("System.Data.SqlClient",
            "Data Source=ALEABE10\\SQLEXPRESS;Initial Catalog=UnitTests;Persist Security Info=True;User ID=unittests;Password=memorylifter",
            "ConnectionStrings",
            DataAccessMethod.Sequential)]
        public void LoginUserProfileTest()
        {
            TestInfrastructure.DebugLineStart(TestContext);
            if (TestInfrastructure.IsActive(TestContext))
            {
                using (IDictionary writeLM = TestInfrastructure.GetConnection(TestContext))
                {
                    ICard card = writeLM.Cards.AddNew();
                    ICardStyle style1 = card.CreateCardStyle();

                    style1.Answer.BackgroundColor = Color.Red;
                    style1.Answer.FontFamily = new FontFamily("Arial");

                    ICardStyle style2 = style1.Clone();

                    //Check if style1 and style2 are equal
                    Assert.AreEqual<Color>(style1.Answer.BackgroundColor, style2.Answer.BackgroundColor, "ICardStyle.Clone does not clone the original instance");
                    Assert.AreEqual<FontFamily>(style1.Answer.FontFamily, style2.Answer.FontFamily, "ICardStyle.Clone does not clone the original instance");

                    style2.Answer.BackgroundColor = Color.Blue;
                    style2.Answer.FontFamily = new FontFamily("Courier New");

                    //Check if style 1 and style2 are independent
                    Assert.AreNotEqual<Color>(style1.Answer.BackgroundColor, style2.Answer.BackgroundColor, "ICardStyle.Clone does not make an independent copy of the original instance");
                    Assert.AreNotEqual<FontFamily>(style1.Answer.FontFamily, style2.Answer.FontFamily, "ICardStyle.Clone does not make an independent copy of the original instance");
                }
            }
            TestInfrastructure.DebugLineEnd(TestContext);
        }


    }
}
