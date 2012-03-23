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
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.Generics;

namespace MLifterTest.Generics
{
    /// <summary>
    /// Summary Description for PasswordGeneratorTests
    /// </summary>
    [TestClass]
    public class PasswordGeneratorTests
    {
        public PasswordGeneratorTests()
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
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Generate01()
        {
            string s;
            s = RandomPassword.Generate(0, 1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Generate02()
        {
            string s;
            s = RandomPassword.Generate(1, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Generate03()
        {
            string s;
            s = RandomPassword.Generate(2, 1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Generate04()
        {
            string s;
            s = RandomPassword.Generate(2, 1001);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Generate05()
        {
            string s;
            s = RandomPassword.Generate(0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Generate06()
        {
            string s;
            s = RandomPassword.Generate(1001);
        }
        [TestMethod]
        public void GenerateRandom01()
        {
            for (int i = 0; i < 100; i++)
            {
                int length = TestInfrastructure.RandomGen.Next(1, 1000);
                string s = RandomPassword.Generate(length);
                Assert.AreEqual<int>(length, s.Length);
            }
        }
        [TestMethod]
        public void GenerateRandom02()
        {
            for (int i = 0; i < 100; i++)
            {
                int lower = TestInfrastructure.RandomGen.Next(1, 1000);
                int upper = TestInfrastructure.RandomGen.Next(lower, 1000);
                string s = RandomPassword.Generate(lower, upper);
                Assert.IsTrue((s.Length >= lower) && (s.Length <= upper));
            }
        }
        [TestMethod]
        public void GenerateCharsetCheck()
        {
            // '-' must be escaped because it has a special meaning in a character class
            // the rest dont need to be escaped because they are in a character class
            string CHARS_SPECIAL = @"*$\-+?_&=!%.,#~/";
            Regex validChar = new Regex("^[A-Za-z0-9" + CHARS_SPECIAL + "]$");

            // test with 100 random strings of random length
            for (int i = 0; i < 100; i++)
            {
                int len = TestInfrastructure.RandomGen.Next(1, 1000);
                string s = RandomPassword.Generate(len);
                foreach (char c in s)
                {
                    Assert.IsTrue(validChar.IsMatch(c.ToString()));
                }
            }
        }
    }
}
