using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Security.Principal;

namespace MLifterTest.DAL
{
	/// <summary>
	/// Summary Description for ICardStyleTest
	/// </summary>
	[TestClass]
	public class OpenUserProfileTests
	{
		public OpenUserProfileTests()
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
		/// Logins the user profile test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void LoginUserProfileTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty))
				{
					IList<UserStruct> users;
					if (writeLM.IsDB)
						users = UserFactory.GetUserList(writeLM.Parent.CurrentUser.ConnectionString);

					if (writeLM.Parent.CurrentUser.ConnectionString.Typ != DatabaseType.MsSqlCe)
					{
						IUser user = (IUser)UserFactory.Create((GetLoginInformation)GetUserAdmin, writeLM.Parent.CurrentUser.ConnectionString,
							(DataAccessErrorDelegate)delegate { return; }, TestContext);

						//before: 7ff135854376850e9711bd75ce942e07
						Assert.AreEqual<string>(TestInfrastructure.SupportsNullableValues(TestContext) ? "21232f297a57a5a743894a0e4a801fc3" : string.Empty,
						user.Password, "Password isnt't encrypted properly");
						Assert.AreEqual<string>(TestInfrastructure.SupportsNullableValues(TestContext) ?
							GetUserAdmin(new UserStruct(), new ConnectionStringStruct()).Value.UserName : string.Empty,
							user.UserName, "UserName isn't saved properly");
					}
					else
					{
						Assert.AreEqual<string>(WindowsIdentity.GetCurrent().Name.ToLower(), writeLM.Parent.CurrentUser.UserName.ToLower());
					}

				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Logins the user profile case sensitive test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		public void LoginUserProfileCaseSensitiveTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext))
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, string.Empty, false))
				{
					IUser user;
					if (writeLM.Parent.CurrentUser.ConnectionString.Typ != DatabaseType.MsSqlCe)
					{
						user = UserFactory.Create((GetLoginInformation)GetUserTeacher, writeLM.Parent.CurrentUser.ConnectionString,
						(DataAccessErrorDelegate)delegate { return; }, TestContext);

						Assert.AreNotEqual<string>(GetUserTeacher(new UserStruct(), new ConnectionStringStruct()).Value.UserName,
							user.UserName, "The Login mechanism is CaseSensitive, although it shouldn't be!");
					}
				}
			}
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Logins the list user invalid test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(NoValidUserException), "InvalidCredentialsException Exception was not thrown.")]
		public void LoginListUserInvalidTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "pgsql")
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, (GetLoginInformation)GetInvalidListUser))
				{
				}
			}
			else
				throw new NoValidUserException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Logins the list user invalid2 test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(NoValidUserException), "InvalidCredentialsException Exception was not thrown.")]
		public void LoginListUserInvalid2Test()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "pgsql")
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, (GetLoginInformation)GetInvalidListUser2))
				{
				}
			}
			else
				throw new NoValidUserException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Logins the forms user invalid test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(NoValidUserException), "InvalidCredentialsException Exception was not thrown.")]
		public void LoginFormsUserInvalidTest()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "pgsql")
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, (GetLoginInformation)GetInvalidFormsUser))
				{
				}
			}
			else
				throw new NoValidUserException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Logins the forms user invalid2 test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(NoValidUserException), "InvalidCredentialsException Exception was not thrown.")]
		public void LoginFormsUserInvalid2Test()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "pgsql")
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, (GetLoginInformation)GetInvalidFormsUser2))
				{
				}
			}
			else
				throw new NoValidUserException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		/// <summary>
		/// Logins the forms user invalid3 test.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-01-23</remarks>
		[TestMethod]
		[TestProperty("TestCategory", "Developer"), DataSource("TestSources")]
		[ExpectedException(typeof(NoValidUserException), "InvalidCredentialsException Exception was not thrown.")]
		public void LoginFormsUserInvalid3Test()
		{
			TestInfrastructure.DebugLineStart(TestContext);
			if (TestInfrastructure.IsActive(TestContext) && TestInfrastructure.ConnectionType(TestContext) == "pgsql")
			{
				using (IDictionary writeLM = TestInfrastructure.GetLMConnection(TestContext, (GetLoginInformation)GetInvalidFormsUser3))
				{
				}
			}
			else
				throw new NoValidUserException();
			TestInfrastructure.DebugLineEnd(TestContext);
		}

		private bool invalidListUserReceived = false;
		private UserStruct? GetInvalidListUser(UserStruct userStr, ConnectionStringStruct con)
		{
			if (!invalidListUserReceived)
				invalidListUserReceived = true;
			else
			{
				LoginError err = userStr.LastLoginError;
				if (err == LoginError.InvalidUsername || err == LoginError.InvalidPassword)
					throw new NoValidUserException();
				else
					throw new Exception("Invalide user not recognised");
			}

			return new UserStruct("invalid", UserAuthenticationTyp.ListAuthentication);
		}

		private bool invalidListUser2Received = false;
		private UserStruct? GetInvalidListUser2(UserStruct userStr, ConnectionStringStruct con)
		{
			if (!invalidListUser2Received)
				invalidListUser2Received = true;
			else
			{
				LoginError err = userStr.LastLoginError;
				if (err == LoginError.WrongAuthentication)
					throw new NoValidUserException();
				else
					throw new Exception("Invalide user not recognised");
			}

			return new UserStruct("alex", UserAuthenticationTyp.ListAuthentication);
		}

		private bool invalidFormsUserReceived = false;
		private UserStruct? GetInvalidFormsUser(UserStruct userStr, ConnectionStringStruct con)
		{
			if (!invalidFormsUserReceived)
				invalidFormsUserReceived = true;
			else
			{
				LoginError err = userStr.LastLoginError;
				if (err == LoginError.InvalidUsername || err == LoginError.InvalidPassword)
					throw new NoValidUserException();
				else
					throw new Exception("Invalide user not recognised");
			}

			return new UserStruct("invalid", string.Empty, UserAuthenticationTyp.FormsAuthentication, true);
		}

		private bool invalidFormsUser2Received = false;
		private UserStruct? GetInvalidFormsUser2(UserStruct userStr, ConnectionStringStruct con)
		{
			if (!invalidFormsUser2Received)
				invalidFormsUser2Received = true;
			else
			{
				LoginError err = userStr.LastLoginError;
				if (err == LoginError.WrongAuthentication)
					throw new NoValidUserException();
				else
					throw new Exception("Invalide authentication not recognised");
			}

			return new UserStruct("testuser", string.Empty, UserAuthenticationTyp.FormsAuthentication, true);
		}

		private bool invalidFormsUser3Received = false;
		private UserStruct? GetInvalidFormsUser3(UserStruct userStr, ConnectionStringStruct con)
		{
			if (!invalidFormsUser3Received)
				invalidFormsUser3Received = true;
			else
			{
				LoginError err = userStr.LastLoginError;
				if (err == LoginError.InvalidUsername || err == LoginError.InvalidPassword)
					throw new NoValidUserException();
				else
					throw new Exception("Invalide user not recognised");
			}

			System.Security.SecureString SecurePassword = new System.Security.SecureString();
			return new UserStruct("alex", string.Empty, UserAuthenticationTyp.FormsAuthentication, true);
		}

		private bool invalidLdUserReceived = false;
		private UserStruct? GetInvalidLdUser(UserStruct userStr, ConnectionStringStruct con)
		{
			if (!invalidLdUserReceived)
				invalidLdUserReceived = true;
			else
			{
				LoginError err = userStr.LastLoginError;
				if (err == LoginError.ForbiddenAuthentication)
					throw new NoValidUserException();
				else
					throw new Exception("Invalide authentication not recognised");
			}

			return new UserStruct("invalid", UserAuthenticationTyp.LocalDirectoryAuthentication);
		}

		public static UserStruct? GetUserAdmin(UserStruct userStr, ConnectionStringStruct con)
		{
			return new UserStruct("admin", "admin", UserAuthenticationTyp.FormsAuthentication, false, true);
		}

		private UserStruct? GetUserTeacher(UserStruct userStr, ConnectionStringStruct con)
		{
			return new UserStruct("Teacher", "teacher", UserAuthenticationTyp.FormsAuthentication, false, true);
		}
	}
}
