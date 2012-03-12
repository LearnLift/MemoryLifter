using MLifter.Generics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.Isam.Esent.Collections.Generic;
using System.Threading;

namespace MLifterTest
{	
	/// <summary>
	///This is a test class for PersistentMemoryCacheTest and is intended
	///to contain all PersistentMemoryCacheTest Unit Tests
	///</summary>
	[TestClass()]
	public class PersistentMemoryCacheTest
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
		
		/// <summary>
		///A test for PersistentMemoryCache`1 Constructor
		///</summary>
		public void PersistentMemoryCacheTestHelper<T>(T testValue)
		{
			Random rand = new Random(DateTime.Now.Millisecond);
			string name = Path.Combine(Path.GetTempPath(), "MLifterCacheTest_" + rand.Next(100, 10000));
			string testKey = "test";

			if (PersistentDictionaryFile.Exists(Path.Combine(name, "values")) || PersistentDictionaryFile.Exists(Path.Combine(name, "lifetime")))
				Assert.Fail("Cache already exisits");
			
			Directory.CreateDirectory(name);
			using (PersistentMemoryCache<T> target = new PersistentMemoryCache<T>(name))
			{
				target.Add(testKey, testValue, DateTime.Now.AddDays(1));
				Assert.AreEqual(target[testKey], testValue, "Value not saved!");
				target.Remove(testKey);
				Assert.IsNull(target[testKey], "Value not removed!");

				target.Add(testKey, testValue, DateTime.Now.AddDays(1));
				target.Dispose();
			}

			using (PersistentMemoryCache<T> reload = new PersistentMemoryCache<T>(name))
			{
				Assert.AreEqual(reload[testKey], testValue, "Reloaded value not restored!");
				reload.Remove(testKey);
				Assert.IsNull(reload[testKey], "Reloaded value not removed!");
				reload.Dispose();
			}

			using (PersistentMemoryCache<T> reload = new PersistentMemoryCache<T>(name))
			{
				Assert.IsNull(reload[testKey], "Reloaded value not null!");

				reload.Add(testKey, testValue, DateTime.Now.AddMilliseconds(50));
				Assert.AreEqual(reload[testKey], testValue, "Timeout to low!");
				Thread.Sleep(75);
				Assert.IsNull(reload[testKey], "Expired value not null!");

				reload.Dispose();
			}

			PersistentDictionaryFile.DeleteFiles(Path.Combine(name, "values"));
			PersistentDictionaryFile.DeleteFiles(Path.Combine(name, "lifetime"));
			if (PersistentDictionaryFile.Exists(Path.Combine(name, "values")) || PersistentDictionaryFile.Exists(Path.Combine(name, "lifetime")))
				Assert.Fail("Cache not deleted!");
		}

		[TestMethod()]
		public void PersistentMemoryCacheMainTest()
		{
			PersistentMemoryCacheTestHelper<string>("test");
		}
	}
}
