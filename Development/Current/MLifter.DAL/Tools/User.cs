using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.DirectoryServices.Protocols;
using System.Security.Principal;
using System.Text;
using MLifter.DAL.DB;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Properties;
using MLifter.DAL.Tools;
using MLifter.DAL.XML;
using Npgsql;
using SecurityFramework;
using MLifter.Generics;
using System.Diagnostics;
using MLifter.DAL.Preview;
using System.Xml;
using MLifter.DAL.LearningModulesService;
using System.DirectoryServices.AccountManagement;

namespace MLifter.DAL
{
	/// <summary>
	/// This is the general user class, which handles the user context for the requested connection.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	public class User : IUser
	{
		/// <summary>
		/// True is this is a WebService instance.
		/// </summary>
		public static bool IsWebService = false;

		private IDbUserConnector connector
		{
			get
			{
				switch (Parent.CurrentUser.ConnectionString.Typ)
				{
					case DatabaseType.PostgreSQL:
						return PgSqlUserConnector.GetInstance(Parent.GetChildParentClass(this));
					case DatabaseType.MsSqlCe:
						return MsSqlCeUserConnector.GetInstance(Parent.GetChildParentClass(this));
					default:
						throw new UnsupportedDatabaseTypeException(Parent.CurrentUser.ConnectionString.Typ);
				}
			}
		}

		private static Guid sessionId = Guid.NewGuid();
		private Guid standAloneId;
		/// <summary>
		/// Gets the session id.
		/// </summary>
		/// <value>The session id.</value>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public Guid SessionId { get { return ConnectionString.SessionId; } }

		private GetLoginInformation getLogin;
		private IUser user;
		/// <summary>
		/// Gets the base user.
		/// </summary>
		/// <value>The base user.</value>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public IUser BaseUser { get { return user; } }
		private bool isStandAlone = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		/// <param name="getLoginInformation">A delegate used to collect login information (username/password).</param>
		/// <param name="connectionString">A struct containing the connection info.</param>
		/// <param name="errorMessageDelegate">A delegate used to handle login errors.</param>
		/// <param name="standAlone">if set to <c>true</c> [stand alone].</param>
		/// <remarks>Documented by Dev03, 2008-11-25</remarks>
		public User(GetLoginInformation getLoginInformation, ConnectionStringStruct connectionString, DataAccessErrorDelegate errorMessageDelegate, bool standAlone)
		{
			if (getLoginInformation == null)
				throw new ArgumentNullException();
			if (errorMessageDelegate == null)
				throw new ArgumentNullException();

			isStandAlone = standAlone;
			if (standAlone) standAloneId = Guid.NewGuid();
			connectionString.SessionId = standAlone ? standAloneId : sessionId;

			getLogin = getLoginInformation;
			ErrorMessageDelegate = errorMessageDelegate;
			user = new DummyUser(connectionString);

			switch (connectionString.Typ)
			{
				case DatabaseType.MsSqlCe:
					user = GetCeUser(connectionString);
					break;
				case DatabaseType.Web:
					user = GetWebUser(connectionString);
					break;
				case DatabaseType.Unc:
					user = new UncUser(new XmlUser(connectionString, errorMessageDelegate), new DbUser(new UserStruct(), Parent, connectionString, errorMessageDelegate, standAlone), connectionString);
					break;
				case DatabaseType.Xml:
					user = new XmlUser(connectionString, errorMessageDelegate);
					break;
				case DatabaseType.PostgreSQL:
					user = GetDbUser(connectionString, errorMessageDelegate, standAlone);
					break;
				default:
					throw new UnsupportedDatabaseTypeException(connectionString.Typ);
			}
		}

		/// <summary>
		/// Gets the web user.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		private IUser GetWebUser(ConnectionStringStruct connection)
		{
			MLifterLearningModulesService webService = new MLifterLearningModulesService();
			webService.Url = connection.ConnectionString;
			webService.CookieContainer = new System.Net.CookieContainer();

			int uid = -1;
			UserStruct? user = null;

			do
			{
				user = getLogin.Invoke(user.HasValue ? user.Value : new UserStruct(), new ConnectionStringStruct(DatabaseType.Web, connection.ConnectionString, true));

				if (!user.HasValue)
					break;
				else if (user.Value.AuthenticationType != UserAuthenticationTyp.ListAuthentication)
					uid = webService.Login(user.Value.UserName, user.Value.Password);
				else
					uid = webService.Login(user.Value.UserName, string.Empty);

				UserStruct lastUser = user.Value;
				try { Methods.CheckUserId(uid); lastUser.LastLoginError = LoginError.NoError; }
				catch (InvalidUsernameException) { lastUser.LastLoginError = LoginError.InvalidUsername; }
				catch (InvalidPasswordException) { lastUser.LastLoginError = LoginError.InvalidPassword; }
				catch (WrongAuthenticationException) { lastUser.LastLoginError = LoginError.WrongAuthentication; }
				catch (ForbiddenAuthenticationException) { lastUser.LastLoginError = LoginError.ForbiddenAuthentication; }
				catch (UserSessionCreationException) { lastUser.LastLoginError = LoginError.AlreadyLoggedIn; }
				user = lastUser;
			}
			while (user.HasValue && uid < 0);

			if (!user.HasValue)
				throw new NoValidUserException();

			return new WebUser(uid, user.Value, connection, webService, Parent);
		}

		/// <summary>
		/// Gets the Compact Edition user.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-13</remarks>
		private IUser GetCeUser(ConnectionStringStruct connection)
		{
			int userId;
			UserStruct? userStruct = null;
			List<UserStruct> users = connector.GetUserList() as List<UserStruct>;
			if (connection.SyncType != SyncType.NotSynchronized)
			{
				if (connection.SyncType == SyncType.FullSynchronized && connection.ServerUser == null)
				{
					if (users.Count == 2)
					{
						userStruct = users[1];
						userId = connector.LoginListUser(userStruct.Value.UserName, Guid.NewGuid(), true, true);
					}
					else
					{
						UserStruct? us = getLogin.Invoke(new UserStruct(Resources.CREATE_NEW_USER, UserAuthenticationTyp.ListAuthentication), connection);
						if (!us.HasValue || us.Value.UserName == Resources.CREATE_NEW_USER)
							userId = connector.LoginFormsUser(CurrentWindowsUserName, CurrentWindowsUserId, new Guid(), true, true);
						else
							userId = connector.LoginLocalDirectoryUser(us.Value.UserName, CurrentWindowsUserId, new Guid(), true, true);
					}
				}
				else
				{
					userStruct = users.Find(u => u.UserName == connection.ServerUser.UserName);
					userId = connector.LoginListUser(userStruct.Value.UserName, connection.ServerUser.ConnectionString.SessionId, true, true);
				}
			}
			else if (Methods.IsOnMLifterStick(connection.ConnectionString))
			{
				if (users.Exists(u => u.UserName == Resources.StickUserName))
				{
					userStruct = users.Find(u => u.UserName == Resources.StickUserName);
					userId = connector.LoginListUser(Resources.StickUserName, new Guid(), true, true);
				}
				else
				{
					userStruct = new UserStruct(Resources.StickUserName, UserAuthenticationTyp.ListAuthentication);
					userId = connector.LoginFormsUser(Resources.StickUserName, string.Empty, new Guid(), true, true);
				}
			}
			else if (users.Exists(u => u.Identifier == CurrentWindowsUserId))
			{
				userStruct = users.Find(u => u.Identifier == CurrentWindowsUserId);
				userId = connector.LoginListUser(userStruct.Value.UserName, new Guid(), true, true);
			}
			else if (users.Count == 1)
				userId = connector.LoginFormsUser(CurrentWindowsUserName, CurrentWindowsUserId, new Guid(), true, true);
			else
			{
				UserStruct? us = getLogin.Invoke(new UserStruct(Resources.CREATE_NEW_USER, UserAuthenticationTyp.ListAuthentication), connection);
				if (!us.HasValue || us.Value.UserName == Resources.CREATE_NEW_USER)
					userId = connector.LoginFormsUser(CurrentWindowsUserName, CurrentWindowsUserId, new Guid(), true, true);
				else
					userId = connector.LoginLocalDirectoryUser(us.Value.UserName, CurrentWindowsUserId, new Guid(), true, true);
			}

			if (!userStruct.HasValue)
				userStruct = new UserStruct(CurrentWindowsUserName, UserAuthenticationTyp.ListAuthentication);

			DbUser newUser = new DbUser(userStruct, Parent, connection, errMsgDel, true);
			newUser.SetId(userId);
			return newUser;
		}
		/// <summary>
		/// Gets the name of the current windows user.
		/// </summary>
		/// <value>
		/// The name of the current windows user.
		/// </value>
		public static string CurrentWindowsUserName { get { return WindowsIdentity.GetCurrent().Name; } }
		/// <summary>
		/// Gets the current windows user id.
		/// </summary>
		public static string CurrentWindowsUserId { get { return WindowsIdentity.GetCurrent().User.Value; } }

		/// <summary>
		/// Gets the db user.
		/// </summary>
		/// <param name="connection">A struct containing the connection info.</param>
		/// <param name="errorMessageDelegate">A delegate used to handle login errors.</param>
		/// <param name="standAlone">if set to <c>true</c> [stand alone].</param>
		/// <returns>A DB user which implements IUser</returns>
		/// <remarks>Documented by Dev03, 2008-11-25</remarks>
		private IUser GetDbUser(ConnectionStringStruct connection, DataAccessErrorDelegate errorMessageDelegate, bool standAlone)
		{
			CheckConnection(connection);

			UserAuthenticationTyp authTyp = connector.GetAllowedAuthenticationModes().Value;

			bool ldUserChecked = false;
			IUser dbUser = null;
			UserStruct? userStruct = null;
			while (dbUser == null)
			{
				if (!IsWebService && !ldUserChecked && (authTyp & UserAuthenticationTyp.LocalDirectoryAuthentication) == UserAuthenticationTyp.LocalDirectoryAuthentication)
				{
					ldUserChecked = true;
					try { userStruct = GetLocalDirectoryUser(); }
					catch (NoValidUserException) { userStruct = null; }
				}
				else
					userStruct = getLogin.Invoke(userStruct.HasValue ? userStruct.Value : new UserStruct(string.Empty, authTyp), ConnectionString);

				if (userStruct.HasValue)
				{
					UserStruct lastUser = userStruct.Value;
					try { dbUser = new DbUser(userStruct, Parent, connection, errorMessageDelegate, standAlone); lastUser.LastLoginError = LoginError.NoError; }
					catch (InvalidUsernameException) { lastUser.LastLoginError = LoginError.InvalidUsername; }
					catch (InvalidPasswordException) { lastUser.LastLoginError = LoginError.InvalidPassword; }
					catch (WrongAuthenticationException) { lastUser.LastLoginError = LoginError.WrongAuthentication; }
					catch (ForbiddenAuthenticationException) { lastUser.LastLoginError = LoginError.ForbiddenAuthentication; }
					catch (UserSessionCreationException) { lastUser.LastLoginError = LoginError.AlreadyLoggedIn; }
					userStruct = lastUser;
				}
				else
					throw new NoValidUserException();
			}

			return dbUser;
		}
		/// <summary>
		/// Tries to authenticate a user with the local directory.
		/// </summary>
		/// <returns>A struct containing the user information.</returns>
		/// <remarks>Documented by Dev03, 2008-11-25</remarks>
		private UserStruct GetLocalDirectoryUser()
		{
			switch (connector.GetLocalDirectoryType())
			{
				case LocalDirectoryType.ActiveDirectory:
					string ad_username = WindowsIdentity.GetCurrent().Name;
					if (ad_username.StartsWith(System.Environment.MachineName) || !CheckLocalDirectoryUser(ad_username))
						throw new NoValidUserException();
					return new UserStruct(ad_username, UserAuthenticationTyp.LocalDirectoryAuthentication);
				case LocalDirectoryType.eDirectory:
					try
					{
						IcNovell.NWCallsInit(0, 0);
						int context = 0;
						IcNovell.NWDSCreateContextHandle(ref context);

						StringBuilder nds_username = new StringBuilder(256);
						IcNovell.NWDSWhoAmI(context, nds_username);

						IcNovell.NWDSFreeContext(context);

						string usern = nds_username.ToString().Substring(nds_username.ToString().IndexOf('=') + 1);

						if (nds_username.ToString() == "[Public]" || !CheckLocalDirectoryUser(usern))
							throw new NoValidUserException();

						return new UserStruct(nds_username.ToString(), UserAuthenticationTyp.LocalDirectoryAuthentication);
					}
					catch (Exception exp)
					{
						Trace.WriteLine(exp.ToString());
						throw new NoValidUserException(exp);
					}
				default:
					throw new NoValidUserException();
			}
		}
		/// <summary>
		/// Checks if the user is a local directory user.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>[true] if the user is a local directory user.</returns>
		/// <remarks>Documented by Dev03, 2008-11-25</remarks>
		private bool CheckLocalDirectoryUser(string username)
		{
			try
			{

				switch (connector.GetLocalDirectoryType())
				{
					case LocalDirectoryType.ActiveDirectory:
						string domain = username.Substring(0, username.IndexOf(@"\"));
						PrincipalContext context;
						if (!String.IsNullOrWhiteSpace(connector.GetLdapUser()) && !String.IsNullOrWhiteSpace(domain))
							context = new PrincipalContext(ContextType.Domain, domain, connector.GetLdapUser(), connector.GetLdapPassword());
						if (!String.IsNullOrWhiteSpace(domain))
							context = new PrincipalContext(ContextType.Domain, domain);
						else
							context = new PrincipalContext(ContextType.Domain);

						UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
						return user != null;
					case LocalDirectoryType.eDirectory:
						LdapConnection connection = new LdapConnection(new LdapDirectoryIdentifier(connector.GetLdapServer()));
						connection.AuthType = AuthType.Basic;

						connection.SessionOptions.SecureSocketLayer = connector.GetLdapUseSSL();
						if (connector.GetLdapUser() != null && connector.GetLdapUser().Length > 0)
							connection.Bind(new System.Net.NetworkCredential(connector.GetLdapUser(), connector.GetLdapPassword()));
						else
							connection.Bind();

						string searchString = String.Format("(&(|(cn={0})(uid={0}))(|(objectClass=user)(objectClass=person)))",	
							username.Substring(username.LastIndexOf("\\") + 1));
						SearchResponse response = connection.SendRequest(new SearchRequest(connector.GetLdapContext(),
							searchString, SearchScope.Subtree, null)) as SearchResponse;

						if (response.Entries.Count > 0)
							return true;
						break;
				}
			}
			catch { return false; }

			return true;
		}

		internal void GenerateNewSession()
		{
			sessionId = Guid.NewGuid();
			ConnectionStringStruct css = user.ConnectionString;
			css.SessionId = sessionId;
			user.ConnectionString = css;
		}

		/// <summary>
		/// Gets the id of a learning module.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2009-03-06
		/// </remarks>
		public static int GetIdOfLearningModule(string path, IUser user)
		{
			return GetIdOfLearningModule(path, user, string.Empty);
		}
		/// <summary>
		/// Gets the id of a learning module.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="user">The user.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2009-03-06
		/// </remarks>
		public static int GetIdOfLearningModule(string path, IUser user, string password)
		{
			if (!path.EndsWith(Helper.EmbeddedDbExtension))
				return -1;

			int lmId = -1;
			try
			{
				ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.MsSqlCe, path, -1);
				css.Password = password;
				IList<int> ids = DbDictionaries.GetConnector(new ParentClass(new DbUser(user.AuthenticationStruct, user.Parent, css, user.ErrorMessageDelegate, true), null)).GetLMIds();
				if (ids.Count == 0)
					return -1;
				lmId = ids[0];
			}
			catch (ProtectedLearningModuleException) { throw; }
			catch (Exception ex) { Debug.WriteLine("DbDictionaries.Dictionaries - " + ex.Message); }
			return lmId;
		}

		/// <summary>
		/// Gets the preview dictionary.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="user">The user.</param>
		/// <param name="extended">if set to <c>true</c> [extended].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public static PreviewDictionary GetPreviewDictionary(string path, IUser user, bool extended)
		{
			if (extended)
				return User.GetExtendedPreviewDictionary(path, user);
			else
				return User.GetPreviewDictionary(path, user);
		}

		/// <summary>
		/// Gets the preview dictionary.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public static PreviewDictionary GetPreviewDictionary(string path, IUser user)
		{
			PreviewDictionary dic = new PreviewDictionary(user);
			using (XmlReader reader = XmlReader.Create(path))
			{
				try
				{
					if (reader.ReadToFollowing("category"))
					{
						int oldId, newId;
						if (Int32.TryParse(reader.GetAttribute("id"), out newId) && (newId >= 0))
						{
							dic.Category = new Category(newId);
						}
						else if (Int32.TryParse(reader.ReadElementContentAsString(), out oldId) && (oldId >= 0))
						{
							dic.Category = new Category(oldId, false);
						}
					}
				}
				catch
				{
					dic.Category = new Category(0);
				}
				dic.Id = 1;
				dic.Connection = path;
				return dic;
			}
		}

		/// <summary>
		/// Gets the extended preview dictionary.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public static PreviewDictionary GetExtendedPreviewDictionary(string path, IUser user)
		{
			PreviewDictionary dic = new PreviewDictionary(user);
			dic.Connection = path;
			return UpdatePreviewDictionary(dic);
		}

		/// <summary>
		/// Updates the preview dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-03-30</remarks>
		public static PreviewDictionary UpdatePreviewDictionary(PreviewDictionary dictionary)
		{
			using (XmlReader reader = XmlReader.Create(dictionary.Connection))
			{
				try
				{
					ICard card = null;
					IStatistic statistic = null;
					bool imageFound = false;
					while (reader.Read())
					{
						if (reader.IsEmptyElement || reader.NodeType == XmlNodeType.EndElement)
							continue;
						switch (reader.Name)
						{
							case "category":
								int oldId, newId;
								if (Int32.TryParse(reader.GetAttribute("id"), out newId) && (newId >= 0))
								{
									dictionary.Category = new Category(newId);
								}
								else if (Int32.TryParse(reader.ReadElementContentAsString(), out oldId) && (oldId >= 0))
								{
									dictionary.Category = new Category(oldId, false);
								}
								break;
							case "author":
								dictionary.Author = reader.ReadElementContentAsString().Trim();
								break;
							case "description":
								dictionary.Description = reader.ReadElementContentAsString().Trim();
								break;
							case "sounddir":
								dictionary.MediaDirectory = reader.ReadElementContentAsString().Trim();
								break;
							case "card":
								card = dictionary.Cards.AddNew();
								break;
							case "answer":
								if (imageFound) continue;
								if (card != null)
								{
									card.Answer.AddWords(Helper.SplitWordList(reader.ReadElementContentAsString()));
								}
								break;
							case "question":
								if (imageFound) continue;
								if (card != null)
								{
									card.Question.AddWords(Helper.SplitWordList(reader.ReadElementContentAsString()));
								}
								break;
							case "answerexample":
								if (imageFound) continue;
								if (card != null)
								{
									card.AnswerExample.AddWords(Helper.SplitWordList(reader.ReadElementContentAsString()));
								}
								break;
							case "questionexample":
								if (imageFound) continue;
								if (card != null)
								{
									card.QuestionExample.AddWords(Helper.SplitWordList(reader.ReadElementContentAsString()));
								}
								break;
							case "questionimage":
								if (imageFound) continue;
								if (card != null)
								{
									IMedia qmedia = card.CreateMedia(EMedia.Image, reader.ReadElementContentAsString(), true, false, false);
									card.AddMedia(qmedia, Side.Question);
									imageFound = true;
								}
								break;
							case "answerimage":
								if (imageFound) continue;
								if (card != null)
								{
									IMedia amedia = card.CreateMedia(EMedia.Image, reader.ReadElementContentAsString(), true, false, false);
									card.AddMedia(amedia, Side.Question);
								}
								break;
							case "stats":
								statistic = new PreviewStatistic();
								int sId;
								if (Int32.TryParse(reader.GetAttribute("id"), out sId) && (sId >= 0))
								{
									(statistic as PreviewStatistic).Id = sId;
								}
								dictionary.Statistics.Add(statistic);
								break;
							case "start":
								if (statistic != null)
								{
									statistic.StartTimestamp = XmlConvert.ToDateTime(reader.ReadElementContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
								}
								break;
							case "end":
								if (statistic != null)
								{
									statistic.EndTimestamp = XmlConvert.ToDateTime(reader.ReadElementContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
								}
								break;
							case "right":
								if (statistic != null)
								{
									statistic.Right = XmlConvert.ToInt32(reader.ReadElementContentAsString());
								}
								break;
							case "wrong":
								if (statistic != null)
								{
									statistic.Wrong = XmlConvert.ToInt32(reader.ReadElementContentAsString());
								}
								break;
							default:
								break;
						}
						//System.Threading.Thread.Sleep(5);
					}
				}
				catch
				{
					dictionary.Category = new Category(0);
				}
				dictionary.Id = 1;
				return dictionary;
			}
		}

		#region IUser Members

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public int Id { get { return user.Id; } }

		/// <summary>
		/// Gets the authentication struct.
		/// </summary>
		/// <value>The authentication struct.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public UserStruct AuthenticationStruct { get { return user.AuthenticationStruct; } }

		/// <summary>
		/// Gets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public string UserName { get { return user.UserName; } }

		/// <summary>
		/// Gets the password. DO NOT STORE THIS IN A string-VARIABLE TO ENSURE A SECURE STORAGE!!!
		/// </summary>
		/// <value>The password.</value>
		/// <remarks>Documented by Dev05, 2008-08-28</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public string Password { get { return user.Password; } }

		/// <summary>
		/// Gets a value indicating whether this instance can open a learning module.
		/// </summary>
		/// <value><c>true</c> if this instance can open; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2009-03-02</remarks>
		public bool CanOpen { get { return user.CanOpen; } }

		/// <summary>
		/// Opens the learning module selected in the connection string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public IDictionary Open()
		{
			CheckConnection(ConnectionString);

			if (user is XmlUser && ConnectionString.Typ != DatabaseType.Xml)
			{
				user.Logout();
				if (ConnectionString.Typ == DatabaseType.MsSqlCe)
					user = GetCeUser(ConnectionString);
				else
					user = GetDbUser(ConnectionString, ErrorMessageDelegate, false);
			}
			else if (user is DbUser && ConnectionString.Typ == DatabaseType.Xml)
			{
				user.Logout();
				user = new XmlUser(ConnectionString, ErrorMessageDelegate);
			}

			IDictionary dictionary = user.Open();
			return dictionary;
		}

		/// <summary>
		/// Lists all available learning modules.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public IDictionaries List()
		{
			if (user is XmlUser && (ConnectionString.Typ != DatabaseType.Xml && ConnectionString.Typ != DatabaseType.Unc))
				user = GetDbUser(ConnectionString, ErrorMessageDelegate, false);
			else if (user is DbUser && ConnectionString.Typ == DatabaseType.Xml)
				user = new XmlUser(ConnectionString, ErrorMessageDelegate);

			return user.List();
		}

		/// <summary>
		/// Logins this user.
		/// </summary>
		public void Login() { user.Login(); }

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public ConnectionStringStruct ConnectionString
		{
			get
			{
				return user.ConnectionString;
			}
			set
			{
				dbConnectionValid = false;
				value.SessionId = isStandAlone ? standAloneId : value.SessionId == new Guid() ? sessionId : value.SessionId;
				user.ConnectionString = value;
			}
		}

		/// <summary>
		/// Sets the connection string.
		/// </summary>
		/// <param name="css">The CSS.</param>
		/// <remarks>Documented by Dev05, 2010-02-03</remarks>
		public void SetConnectionString(ConnectionStringStruct css)
		{
			user.SetConnectionString(css);
		}

		private static bool dbConnectionValid = false;
		/// <summary>
		/// Checks the connection.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public void CheckConnection(ConnectionStringStruct connectionString)
		{
			if (!dbConnectionValid && connectionString.Typ != DatabaseType.Xml)
			{
				DummyUser user = new DummyUser(connectionString);
				DbConnection con = connectionString.Typ == DatabaseType.PostgreSQL ? PostgreSQLConn.CreateConnection(user) as DbConnection : MSSQLCEConn.GetConnection(user) as DbConnection;

				if (con.State == System.Data.ConnectionState.Open)
					dbConnectionValid = true;
				else
					throw new ConnectionInvalidException();

				if (connectionString.Typ == DatabaseType.PostgreSQL)
					con.Close();

				DbDatabase.GetInstance(new ParentClass(new DummyUser(connectionString), this)).CheckIfDatabaseVersionSupported();
			}
		}

		private Cache cache = new Cache(true);
		/// <summary>
		/// Gets the cache for this user.
		/// </summary>
		/// <value>The cache.</value>
		/// <remarks>Documented by Dev05, 2008-09-24</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public Cache Cache { get { return cache; } }

		/// <summary>
		/// Closes the session.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public void Logout()
		{
			user.Logout();
			if (!isStandAlone)
				sessionId = Guid.NewGuid();
		}

		/// <summary>
		/// Gets or sets the get login delegate.
		/// </summary>
		/// <value>The get login delegate.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		public GetLoginInformation GetLoginDelegate
		{
			get
			{
				return getLogin;
			}
			set
			{
				getLogin = value;
			}
		}

		private DataAccessErrorDelegate errMsgDel;
		/// <summary>
		/// Gets or sets the error message delegate.
		/// </summary>
		/// <value>The error message delegate.</value>
		/// <remarks>Documented by Dev05, 2008-11-17</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public DataAccessErrorDelegate ErrorMessageDelegate { get { return errMsgDel; } set { errMsgDel = value; } }

		/// <summary>
		/// Gets the underlying database for this user.
		/// </summary>
		/// <value>The database.</value>
		/// <remarks>Documented by Dev03, 2009-05-01</remarks>
		public IDatabase Database
		{
			get { return user.Database; }
		}

		/// <summary>
		/// Determines whether the specified object has the given permission.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="permissionName">Name of the permission.</param>
		/// <returns>
		/// 	<c>true</c> if the specified o has permission; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		public bool HasPermission(object o, string permissionName)
		{
			return user.HasPermission(o, permissionName);
		}

		/// <summary>
		/// Gets the permissions for an object.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <returns>The list of permissions.</returns>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		public List<PermissionInfo> GetPermissions(object o)
		{
			return user.GetPermissions(o);
		}
		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public ParentClass Parent
		{
			get { return new ParentClass(this, null); }
		}

		#endregion

		#region IUser Members




		#endregion
	}
}
