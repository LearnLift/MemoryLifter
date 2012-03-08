using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.Generics;
using System.Text;
namespace MLifterTest.Generics
{


    /// <summary>
    ///This is a test class for RSATest and is intended
    ///to contain all RSATest Unit Tests
    ///</summary>
    [TestClass()]
    public class RSATest
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

        #region EncryptData
        /// <summary>
        /// Tests EncryptData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EncryptData01()
        {
            RSA rSA;
            string s;
            rSA = new RSA();
            s = rSA.EncryptData("data", "");
        }
        /// <summary>
        /// Tests EncryptData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EncryptData02()
        {
            RSA rSA;
            string s;
            rSA = new RSA();
            s = rSA.EncryptData("data", "<invalidxml>");
        }
        /// <summary>
        /// Tests EncryptData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncryptData03()
        {
            RSA rSA;
            string s;
            rSA = new RSA();
            s = rSA.EncryptData("data", (string)null);
        }
        /// <summary>
        /// Tests EncryptData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncryptData04()
        {
            RSA rSA;
            string s;
            rSA = new RSA();
            s = rSA.EncryptData(null, rSA.GetPrivateKey());
        }
        #endregion

        #region SignData
        /// <summary>
        /// Tests SignData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SignData01()
        {
            RSA rSA;
            byte[] bs;
            rSA = new RSA();
            byte[] bs1 = new byte[0];
            bs = rSA.SignData(bs1, "<invalidxml>");
        }
        /// <summary>
        /// Tests SignData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SignData02()
        {
            RSA rSA;
            byte[] bs;
            rSA = new RSA();
            byte[] bs1 = new byte[0];
            bs = rSA.SignData(bs1, "");
        }
        /// <summary>
        /// Tests SignData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SignData03()
        {
            RSA rSA;
            byte[] bs;
            rSA = new RSA();
            byte[] bs1 = new byte[0];
            bs = rSA.SignData(bs1, (string)null);
        }
        /// <summary>
        /// Tests SignData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SignData04()
        {
            RSA rSA;
            byte[] bs;
            rSA = new RSA();
            bs = rSA.SignData(null, rSA.GetPrivateKey());
        }
        #endregion

        #region VerifyData
        /// <summary>
        /// Tests VerifyData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(SignatureInvalidException))]
        public void VerifyData02()
        {
            RSA rSA;
            rSA = new RSA();
            byte[] bs = new byte[0];
            byte[] bs1 = new byte[0];
            byte[] bs2 = new byte[0];
            rSA.VerifyData((byte[])null, bs, bs1, bs2);
        }
        /// <summary>
        /// Tests VerifyData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(SignatureInvalidException))]
        public void VerifyData03()
        {
            RSA rSA;
            rSA = new RSA();
            byte[] bs = new byte[0];
            byte[] bs1 = new byte[0];
            byte[] bs2 = new byte[0];
            rSA.VerifyData(bs, bs1, bs2, (byte[])null);
        }
        /// <summary>
        /// Tests VerifyData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(SignatureInvalidException))]
        public void VerifyData04()
        {
            RSA rSA;
            rSA = new RSA();
            byte[] bs = new byte[0];
            byte[] bs1 = new byte[0];
            byte[] bs2 = new byte[0];
            rSA.VerifyData(bs, (byte[])null, bs1, bs2);
        }
        /// <summary>
        /// Tests VerifyData.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        [ExpectedException(typeof(SignatureInvalidException))]
        public void VerifyData05()
        {
            RSA rSA;
            rSA = new RSA();
            byte[] bs = new byte[0];
            byte[] bs1 = new byte[0];
            byte[] bs2 = new byte[0];
            rSA.VerifyData(bs, bs1, (byte[])null, bs2);
        }
        #endregion

        /// <summary>
        /// Tests the sign and verify.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-02-17</remarks>
        [TestMethod]
        public void TestSignAndVerify()
        {
            for (int i = 0; i < 100; i++)
            {
                RSA rSA;
                rSA = new RSA();
                string data = TestInfrastructure.GetRandomString(TestInfrastructure.RandomGen.Next(1, 1000));
                Encoding enc = Encoding.Unicode;
                byte[] signature = rSA.SignData(enc.GetBytes(data), rSA.GetPrivateKey());
                byte[] mod = Methods.HexStringToByteArray(Methods.ExtractPublicKey(rSA.GetPublicKey()));
                byte[] exp = Methods.HexStringToByteArray(Methods.ExtractExponent(rSA.GetPublicKey()));
                rSA.VerifyData(enc.GetBytes(data), signature, mod, exp);
            }
        }
    }
}
