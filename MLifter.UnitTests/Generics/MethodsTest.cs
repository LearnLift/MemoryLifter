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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.Generics;
namespace MLifterTest.Generics
{


    /// <summary>
    ///This is a test class for MethodsTest and is intended
    ///to contain all MethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MethodsTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region ByteArrayToCompressedArrayString
        /// <summary>
        /// Test for ByteArrayToCompressedArrayString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ByteArrayToCompressedArrayString01()
        {
            string s;
            s = Methods.ByteArrayToCompressedArrayString((byte[])null);
        }

        /// <summary>
        /// Test for ByteArrayToCompressedArrayString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void ByteArrayToCompressedArrayString02()
        {
            string s;
            byte[] bs = new byte[0];
            s = Methods.ByteArrayToCompressedArrayString(bs);
            Assert.AreEqual<string>("", s);
        }

        /// <summary>
        /// Test for ByteArrayToCompressedArrayString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void ByteArrayToCompressedArrayStringRandom()
        {
            for (int k = 0; k < 100; k++)
            {
                string input = TestInfrastructure.GetRandomString(TestInfrastructure.RandomGen.Next(2, 100));
                byte[] bs = new byte[input.Length];
                for (int i = 0; i < input.Length; i++)
                    bs[i] = (byte)input[i];
                string step1 = Methods.ByteArrayToCompressedArrayString(bs);
                byte[] bs2 = Methods.CompressedArrayStringToByteArray(step1, input.Length);
                string step2 = String.Empty;
                for (int i = 0; i < bs2.Length; i++)
                    step2 += (char)bs2[i];

                Assert.AreEqual<string>(input, step2);
            }
        }

        /// <summary>
        /// Compares the length of random compressed array string and hex strings
        /// to ensure the compressed array strings are shorter.
        /// </summary>
        /// <remarks>Documented by Dev09, 2009-02-18</remarks>
        [TestMethod]
        public void CompressedArrayStringHexStringLengthComparison()
        {
            for (int k = 0; k < 100; k++)
            {
                // generate the random string and corresponding byte array
                string input = TestInfrastructure.GetRandomString(TestInfrastructure.RandomGen.Next(2, 100));
                byte[] bs = new byte[input.Length];
                for (int i = 0; i < input.Length; i++)
                    bs[i] = (byte)input[i];

                // encode the byte array both as compressed string and hex string
                string compressedStr = Methods.ByteArrayToCompressedArrayString(bs);
                string hexStr = Methods.ByteArrayToHexString(bs);

                Assert.IsTrue(compressedStr.Length <= hexStr.Length);
            }
        }

        #endregion

        #region CompressedArrayStringToByteArray
        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompressedArrayStringToByteArray01()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray("", int.MinValue);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompressedArrayStringToByteArray02()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray("", 0);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressedArrayStringToByteArray03()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray((string)null, 2);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CompressedArrayStringToByteArray04()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray("", 1);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(1, bs.Length);
            Assert.AreEqual<byte>((byte)0, bs[0]);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CompressedArrayStringToByteArray05()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray("\0", 1);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(1, bs.Length);
            Assert.AreEqual<byte>((byte)0, bs[0]);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CompressedArrayStringToByteArray06()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray(new string('J', 62), 1);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(1, bs.Length);
            Assert.AreEqual<byte>((byte)132, bs[0]);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CompressedArrayStringToByteArray07()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray("JJ", 2);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(2, bs.Length);
            Assert.AreEqual<byte>((byte)132, bs[0]);
            Assert.AreEqual<byte>((byte)0, bs[1]);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CompressedArrayStringToByteArray08()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray
                ("2222222222\022222222222\022222222222222\022222222", 2);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(2, bs.Length);
            Assert.AreEqual<byte>((byte)0, bs[0]);
            Assert.AreEqual<byte>((byte)0, bs[1]);
        }

        /// <summary>
        /// Test for CompressedArrayStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CompressedArrayStringToByteArray09()
        {
            byte[] bs;
            bs = Methods.CompressedArrayStringToByteArray(new string('2', 46), 3);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(3, bs.Length);
            Assert.AreEqual<byte>((byte)0, bs[0]);
            Assert.AreEqual<byte>((byte)0, bs[1]);
            Assert.AreEqual<byte>((byte)0, bs[2]);
        }
        #endregion

        #region CreateLicenseKey
        /// <summary>
        /// Test for CreateLicenseKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateLicenseKeyNull()
        {
            string s;
            s = Methods.CreateLicenseKey((string)null);
        }
        /// <summary>
        /// Test for CreateLicenseKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateLicenseKeyInvalid01()
        {
            string s;
            s = Methods.CreateLicenseKey(String.Empty);
        }
        /// <summary>
        /// Test for CreateLicenseKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateLicenseKeyInvalid02()
        {
            string s;
            s = Methods.CreateLicenseKey("other");
        }
        /// <summary>
        /// Test for CreateLicenseKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CreateLicenseKeyClient()
        {
            string s;
            for (int i = 0; i < 100; i++)
            {
                s = Methods.CreateLicenseKey("client");
                string[] blocks = s.Split(new char[] { '-' });
                foreach (string block in blocks)
                {
                    int sum = 0;
                    foreach (char c in block)
                        sum += (int)c;
                    Assert.IsFalse(sum % 2 == 0);
                }
            }
        }
        /// <summary>
        /// Test for CreateLicenseKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void CreateLicenseKeyServer()
        {
            string s;
            for (int i = 0; i < 100; i++)
            {
                s = Methods.CreateLicenseKey("server");
                string[] blocks = s.Split(new char[] { '-' });
                foreach (string block in blocks)
                {
                    int sum = 0;
                    foreach (char c in block)
                        sum += (int)c;
                    Assert.IsTrue(sum % 2 == 0);
                }
            }
        }
        #endregion

        #region IsEven
        /// <summary>
        /// Test for IsEven().
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IsEven01()
        {
            bool b;
            b = Methods.IsEven(-1);
        }
        /// <summary>
        /// Test for IsEven().
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IsEven02()
        {
            bool b;
            b = Methods.IsEven(10000);
        }
        /// <summary>
        /// Test for IsEven().
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-13</remarks>
        [TestMethod]
        public void IsEvenRandom()
        {
            for (int i = 0; i < 100; i++)
            {
                int val = TestInfrastructure.RandomGen.Next(0, 9999);

                string block = val.ToString("0000");
                int sum = 0;
                foreach (char c in block)
                    sum += (int)c;

                Assert.AreEqual<bool>(Methods.IsEven(val), (sum % 2 == 0));
            }
        }
        #endregion

        #region GenerateSymKey
        /// <summary>
        /// Tests for GenerateSymKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateSymKey01()
        {
            string s;
            s = Methods.GenerateSymKey((string)null);
        }
        /// <summary>
        /// Tests for GenerateSymKey.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        public void GenerateSymKey02()
        {
            string s;
            s = Methods.GenerateSymKey("test");
            Assert.AreEqual<string>("098f6bcd4621d373cade4e83", s);
        }
        #endregion

        #region TDesDecryptBytes
        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TDesDecrypt01()
        {
            string s;
            s = Methods.TDesDecrypt((string)null, "", false);
        }
        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TDesDecryptBytes01()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, "", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TDesDecryptBytes02()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, (string)null, false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TDesDecryptBytes03()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, "\0", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TDesDecryptBytes04()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, "\0", true);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TDesDecryptBytes05()
        {
            byte[] bs;
            bs = Methods.TDesDecryptBytes((byte[])null, "\0", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TDesDecryptBytes06()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, "\u3000", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TDesDecryptBytes07()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, "\0\u1680", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TDesDecryptBytes08()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, " \0", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TDesDecryptBytes09()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesDecryptBytes(bs1, "\u3000\u3000", false);
        }

        /// <summary>
        /// Test for TDesDecryptBytes.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TDesDecryptBytes10()
        {
            byte[] bs;
            byte[] bs1 = new byte[2];
            bs = Methods.TDesDecryptBytes(bs1, "\0\u00a0\u2028", false);
        }
        #endregion

        #region TDesEncryptBytes
        /// <summary>
        /// Test for TDesEncryptByte.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TDesEncryptBytes0101()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesEncryptBytes(bs1, "", false);
        }
        /// <summary>
        /// Test for TDesEncryptByte.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        public void TDesEncryptBytes0102()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesEncryptBytes(bs1, "", true);
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(8, bs.Length);
            Assert.AreEqual<byte>((byte)76, bs[0]);
            Assert.AreEqual<byte>((byte)135, bs[1]);
            Assert.AreEqual<byte>((byte)82, bs[2]);
            Assert.AreEqual<byte>((byte)247, bs[3]);
            Assert.AreEqual<byte>((byte)237, bs[4]);
            Assert.AreEqual<byte>((byte)57, bs[5]);
            Assert.AreEqual<byte>((byte)1, bs[6]);
            Assert.AreEqual<byte>((byte)171, bs[7]);
        }
        /// <summary>
        /// Test for TDesEncryptByte.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TDesEncryptBytes0103()
        {
            byte[] bs;
            bs = Methods.TDesEncryptBytes((byte[])null, "", false);
        }
        /// <summary>
        /// Test for TDesEncryptByte.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-16</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TDesEncryptBytes0104()
        {
            byte[] bs;
            byte[] bs1 = new byte[0];
            bs = Methods.TDesEncryptBytes(bs1, (string)null, false);
        }
        #endregion

        #region EncryptDecryptDes
        /// <summary>
        /// Encrypts the decrypt des01.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void EncryptDecryptDes01()
        {
            string k = Methods.GenerateSymKey("testkey");
            string input = "testdata";
            string de = Methods.TDesEncrypt(input, k, false);
            string output = Methods.TDesDecrypt(de, k, false);
            Assert.AreEqual<string>(input, output);
        }
        /// <summary>
        /// Encrypts the decrypt des02.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void EncryptDecryptDes02()
        {
            string k = Methods.GenerateSymKey("testkey");
            string input = "testdata";
            string de = Methods.TDesEncrypt(input, k, true);
            string output = Methods.TDesDecrypt(de, k, true);
            Assert.AreEqual<string>(input, output);
        }
        #endregion

        #region ExtractPublicKey
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtractPublicKey01()
        {
            string s;
            s = Methods.ExtractPublicKey((string)null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractPublicKey02()
        {
            string s;
            string key = "";
            s = Methods.ExtractPublicKey(key);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractPublicKey03()
        {
            string s;
            string key = "<invalidxml>";
            s = Methods.ExtractPublicKey(key);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractPublicKey04()
        {
            string s;
            string key = "<validxml></validxml>";
            s = Methods.ExtractPublicKey(key);
        }
        [TestMethod]
        public void ExtractPublicKey05()
        {
            string s;
            MLifter.Generics.RSA rsa = new MLifter.Generics.RSA();
            string key = rsa.GetPublicKey();
            s = Methods.ExtractPublicKey(key);
            Regex regex = new Regex(@"<Modulus>(?<modulus>.+)</Modulus>", RegexOptions.Multiline);
            Match m = regex.Match(key);
            string mod = String.Empty;
            if (!m.Success)
            {
                Assert.Fail("Could not match modulus!");
            }
            else
            {
                mod = Methods.Base64ToHex(m.Groups["modulus"].Value);
            }
            Assert.AreEqual<string>(mod, s);
        }
        #endregion

        #region ExtractExponent
        /// <summary>
        /// Tests ExtractExponent.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtractExponent01()
        {
            string s;
            s = Methods.ExtractExponent((string)null);
        }
        /// <summary>
        /// Tests ExtractExponent.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractExponent02()
        {
            string s;
            string key = "";
            s = Methods.ExtractExponent(key);
        }
        /// <summary>
        /// Tests ExtractExponent.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractExponent03()
        {
            string s;
            string key = "<invalidxml>";
            s = Methods.ExtractExponent(key);
        }
        /// <summary>
        /// Tests ExtractExponent.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractExponent04()
        {
            string s;
            string key = "<validxml></validxml>";
            s = Methods.ExtractExponent(key);
        }
        /// <summary>
        /// Tests ExtractExponent.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void ExtractExponent05()
        {
            string s;
            MLifter.Generics.RSA rsa = new MLifter.Generics.RSA();
            string key = rsa.GetPublicKey();
            s = Methods.ExtractExponent(key);
            Regex regex = new Regex(@"<Exponent>(?<exponent>.+)</Exponent>", RegexOptions.Multiline);
            Match m = regex.Match(key);
            string mod = String.Empty;
            if (!m.Success)
            {
                Assert.Fail("Could not match exponent!");
            }
            else
            {
                mod = Methods.Base64ToHex(m.Groups["exponent"].Value);
            }
            Assert.AreEqual<string>(mod, s);
        }
        #endregion

        #region Base64ToHex
        /// <summary>
        /// Tests Base64ToHex.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Base64ToHex01()
        {
            string s;
            s = Methods.Base64ToHex((string)null);
        }
        /// <summary>
        /// Tests Base64ToHex.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void Base64ToHex02()
        {
            string s;
            s = Methods.Base64ToHex("");
            Assert.AreEqual<string>("", s);
        }
        /// <summary>
        /// Tests Base64ToHex.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void Base64ToHex03()
        {
            string input = "A test is a test is a test is a test!";
            string s = Methods.Base64ToHex(Convert.ToBase64String(Encoding.ASCII.GetBytes(input)));
            string reverse = Encoding.ASCII.GetString(Methods.HexStringToByteArray(s));
            Assert.AreEqual<string>(input, reverse);
        }
        /// <summary>
        /// Tests Base64ToHex.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void Base64ToHex04()
        {
            for (int i = 0; i <= 100; i++)
            {
                string input = TestInfrastructure.GetRandomString(TestInfrastructure.RandomGen.Next(15, 1000));
                string s = Methods.Base64ToHex(Convert.ToBase64String(Encoding.ASCII.GetBytes(input)));
                string reverse = Encoding.ASCII.GetString(Methods.HexStringToByteArray(s));
                Assert.AreEqual<string>(input, reverse);
            }
        }
        #endregion

        #region ByteArrayToHexString
        /// <summary>
        /// Tests ByteArrayToHexString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ByteArrayToHexString01()
        {
            string s;
            s = Methods.ByteArrayToHexString((byte[])null);
        }
        /// <summary>
        /// Tests ByteArrayToHexString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void ByteArrayToHexString02()
        {
            string s;
            byte[] bs = new byte[0];
            s = Methods.ByteArrayToHexString(bs);
            Assert.AreEqual<string>("", s);
        }
        /// <summary>
        /// Tests ByteArrayToHexString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void ByteArrayToHexString03()
        {
            string s;
            byte[] bs = new byte[1];
            s = Methods.ByteArrayToHexString(bs);
            Assert.AreEqual<string>("00", s);
        }
        /// <summary>
        /// Tests ByteArrayToHexString.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void ByteArrayToHexString04()
        {
            string s;
            byte[] bs = new byte[2];
            s = Methods.ByteArrayToHexString(bs);
            Assert.AreEqual<string>("0000", s);
        }

        /// <summary>
        /// Tests ByteArrayToHexString with sample known case.
        /// </summary>
        /// <remarks>Documented by Dev09, 2009-02-18</remarks>
        [TestMethod]
        public void ByteArrayToHexString05()
        {
            string s;
            byte[] bs = new byte[4] { 170, 187, 204, 221 };
            s = Methods.ByteArrayToHexString(bs);
            Assert.AreEqual<string>("AABBCCDD", s);
        }
        #endregion

        #region HexStringToByteArray
        /// <summary>
        /// Tests HexStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HexStringToByteArray01()
        {
            byte[] bs;
            bs = Methods.HexStringToByteArray((string)null);
        }
        /// <summary>
        /// Tests HexStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HexStringToByteArray02()
        {
            byte[] bs;
            bs = Methods.HexStringToByteArray("\0"); //not a hex string
        }
        /// <summary>
        /// Tests HexStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HexStringToByteArray03()
        {
            byte[] bs;
            bs = Methods.HexStringToByteArray("0a011"); //not a hex string
        }
        /// <summary>
        /// Tests HexStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HexStringToByteArray04()
        {
            byte[] bs;
            bs = Methods.HexStringToByteArray("0a0H10"); //not a hex string
        }
        /// <summary>
        /// Tests HexStringToByteArray.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void HexStringToByteArray05()
        {
            byte[] bs;
            bs = Methods.HexStringToByteArray("");
            Assert.IsNotNull((object)bs);
            Assert.AreEqual<int>(0, bs.Length);
        }

        /// <summary>
        /// Tests HexStringToByteArray with known value.
        /// </summary>
        /// <remarks>Documented by Dev09, 2009-02-18</remarks>
        [TestMethod]
        public void HexStringToByteArray06()
        {
            byte[] bs;
            byte[] correctBs = new byte[4] { 170, 187, 204, 221 };
            string s = "AABBCCDD";
            bs = Methods.HexStringToByteArray(s);

            Assert.AreEqual<int>(correctBs.Length, bs.Length);
            for (int i = 0; i < correctBs.Length; i++)
            {
                Assert.AreEqual<byte>(correctBs[i], bs[i]);
            }
        }
        #endregion

        #region Right
        /// <summary>
        /// Tests Right.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Right01()
        {
            string s;
            s = Methods.Right("", 2);
        }
        /// <summary>
        /// Tests Right.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void Right02()
        {
            string s;
            s = Methods.Right("", 0);
            Assert.AreEqual<string>("", s);
        }
        /// <summary>
        /// Tests Right.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Right03()
        {
            string s;
            s = Methods.Right((string)null, 2);
        }
        /// <summary>
        /// Tests Right.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Right04()
        {
            string s;
            s = Methods.Right("", -262145);
        }
        [TestMethod]
        public void Right05()
        {
            string s;
            s = Methods.Right("This is a test string", 5);
            Assert.AreEqual<string>("tring", s);
        }
        #endregion

        /// <summary>
        /// Tests Right.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void GetMID01()
        {
            string before = String.Empty;
            for (int i = 0; i < 10; i++)
            {
                string s;
                s = Methods.GetMID();
                if (i > 0)
                    Assert.AreEqual<string>(before, s);
                before = s;
            }
        }

    }
}
