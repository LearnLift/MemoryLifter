using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MLifter;
using MLifter.AudioTools;
using MLifter.BusinessLayer.Properties;
using MLifter.DAL;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using MLifter.Generics;
using Npgsql;


namespace MLifter.BusinessLayer
{
	/// <summary>
	/// This class contains the main learning logic.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-22</remarks>
	public class LearnLogic
	{
		#region Common Fields and Properties
		//Dictionary dictionary = null;
		private User user = null;
		private Timer countdownTimer = null;
		private CardStack cardStack;
		private AudioPlayer audioPlayer = new AudioPlayer();
		private bool hasLearnModeChangedAndNotifyUser = false;
		private LearnModes originalLearnMode;
		private bool hasMultipleChoiceLearnModeChanged;

		/// <summary>
		/// This is the ID of the currently asked card.
		/// </summary>
		private int currentCardID = -1;
		/// <summary>
		/// This is a timestamp, when the card was asked.
		/// </summary>
		private DateTime currentCardAsked = DateTime.Now.ToUniversalTime();
		/// <summary>
		/// This is a timestamp, when the card was answered.
		/// </summary>
		private DateTime currentCardAnswered = DateTime.Now.ToUniversalTime();

		public event EventHandler LearnLogicIsWorking;
		public event EventHandler LearnLogicIdle;
		public event EventHandler LearningModuleSyncRequest;

		public LearningModulesIndexEntry CurrentLearningModule { get; set; }
		public GetLoginInformation GetLoginDelegate { get; set; }
		public DataAccessErrorDelegate DataAccessErrorDelegate { get; set; }
		public string SyncedLearningModulesPath { get; set; }

		public struct UserConnectionInfo
		{
			public static string GetDisplayName(User user)
			{
				if (user.ToString().Contains("\\"))
				{
					string[] helper = user.ToString().Split(new char[] { '\\' });
					return helper[1];
				}
				else
					return user.ToString();
			}

			public static string GetServerName(string ConnectionString)
			{
				if (ConnectionString == null)
					return String.Empty;
				if (ConnectionString.Contains(";"))
				{
					string[] helper = ConnectionString.Split(new char[] { ';' });
					if (helper.Length > 1)
					{
						string[] helper2 = helper[0].Split(new char[] { '=' });
						return helper2[1].ToString();
					}
					else
						return Properties.Resources.LOCAL_MODULE;
				}
				else
					return ConnectionString;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LearnLogic"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-22</remarks>
		public LearnLogic(GetLoginInformation getLoginDelegate, DataAccessErrorDelegate dataAccessErrorDelegate)
		{
			GetLoginDelegate = getLoginDelegate;
			DataAccessErrorDelegate = dataAccessErrorDelegate;

			cardStack = new CardStack(this);
			this.LearningModuleOpened += new EventHandler(LearnLogic_LearningModuleOpened);
			this.user = new User(this);
			this.CardStack.StackChanged += new EventHandler(CardStack_StackChanged);
		}

		public void OnLearnLogicIsWorking()
		{
			if (LearnLogicIsWorking != null)
				LearnLogicIsWorking(this, EventArgs.Empty);
		}

		public void OnLearnLogicIsIdle()
		{
			if (LearnLogicIdle != null)
				LearnLogicIdle(this, EventArgs.Empty);
		}

		/// <summary>
		/// Handles the StackChanged event of the CardStack control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-09-11</remarks>
		void CardStack_StackChanged(object sender, EventArgs e)
		{
			//save log entry
			if (this.CardStack.Count > 0)
			{
				StackCard card = this.CardStack.Peek();
				LearnLogStruct lls = new LearnLogStruct();

				lls.CardsID = card.Card.Id;
				lls.SessionID = Log.LastSessionID;
				lls.Answer = card.UserAnswer;
				lls.CaseSensitive = card.ParentDictionary.Settings.CaseSensitive;
				lls.CorrectOnTheFly = card.ParentDictionary.Settings.CorrectOnTheFly;
				lls.Direction = card.QueryDirection;
				lls.Duration = ((TimeSpan)(DateTime.Now.Subtract(card.CardAsked))).Seconds;
				lls.NewBox = card.NewBox;
				lls.OldBox = card.OldBox;
				lls.TimeStamp = DateTime.Now;

				//PercentageRequired, PercentageKnown
				if (!card.ParentDictionary.Settings.SelfAssessment.Value)
				{
					int synonyms = 0;
					int required = 0;

					if (card.QueryDirection == EQueryDirection.Question2Answer)
						synonyms = card.Card.Answer.Words.Count;
					else
						synonyms = card.Card.Question.Words.Count;

					if (synonyms != 0)
					{
						if (card.ParentDictionary.Settings.GradeSynonyms.AllKnown.Value)        //All synonyms have to be known
							required = 100;
						else if (card.ParentDictionary.Settings.GradeSynonyms.FirstKnown.Value || card.ParentDictionary.Settings.GradeSynonyms.OneKnown.Value)      //the first or only one synonym have to be known
							required = (int)(100.0 / synonyms);
						else if (card.ParentDictionary.Settings.GradeSynonyms.Prompt.Value)
							required = 100;
						else if (card.ParentDictionary.Settings.GradeSynonyms.HalfKnown.Value)
							required = 50;

						//MultipleChoice 
						if (card.ParentDictionary.LearnMode == LearnModes.MultipleChoice)
						{
							lls.PercentageRequired = 100;
							lls.PercentageKnown = (card.Result == AnswerResult.Correct) ? 100 : 0;
						}
						else
						{
							lls.PercentageRequired = required;
							lls.PercentageKnown = (int)(100.0 * card.CorrectSynonyms / synonyms);
						}
					}
				}

				//LearnMode
				if (card.ParentDictionary.LearnMode == LearnModes.ImageRecognition)
					lls.LearnMode = EQueryType.ImageRecognition;
				else if (card.ParentDictionary.LearnMode == LearnModes.ListeningComprehension)
					lls.LearnMode = EQueryType.ListeningComprehension;
				else if (card.ParentDictionary.LearnMode == LearnModes.MultipleChoice)
					lls.LearnMode = EQueryType.MultipleChoice;
				else if (card.ParentDictionary.LearnMode == LearnModes.Sentence)
					lls.LearnMode = EQueryType.Sentences;
				else if (card.ParentDictionary.LearnMode == LearnModes.Word)
					lls.LearnMode = EQueryType.Word;

				//MoveType
				if (card.ParentDictionary.Settings.SelfAssessment.Value)
				{
					if (card.Promoted)
						lls.MoveType = MoveType.ManualPromote;
					else
						lls.MoveType = MoveType.ManualDemote;
				}
				else
				{
					if (card.Promoted)
					{
						if (card.CanceledDemote)
							lls.MoveType = MoveType.CanceledDemote;
						else
							lls.MoveType = MoveType.AutoPromote;
					}
					else
						lls.MoveType = MoveType.AutoDemote;
				}

				//Save Logging Entry to DB
				Log.CreateLearnLogEntry(lls, Dictionary.DictionaryDAL.Parent);
			}
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>The dictionary.</value>
		/// <remarks>Documented by Dev02, 2008-04-22</remarks>
		public Dictionary Dictionary
		{
			get { return user.Dictionary; }
		}

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		/// <value>The user.</value>
		/// <remarks>Documented by Dev03, 2008-08-28</remarks>
		public User User
		{
			get { return user; }
		}

		/// <summary>
		/// Gets a value indicating whether [dictionary contains no chapters].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [dictionary no chapters]; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool DictionaryNoChapters
		{
			get { return user.Dictionary.Chapters.Chapters.Count == 0; }
		}

		/// <summary>
		/// Gets a value indicating whether [dictionary contains no query chapters].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [dictionary no query chapters]; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool DictionaryNoQueryChapters
		{
			get { return user.Dictionary.QueryChapters.Count == 0; }
		}

		/// <summary>
		/// Gets a value indicating whether [dictionary contains no cards].
		/// </summary>
		/// <value><c>true</c> if [dictionary no cards]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool DictionaryNoCards
		{
			get
			{
				if (user.Dictionary.LearningBox == 0)
					return user.Dictionary.Cards.ActiveCardsCount == 0;
				else if (user.Dictionary.LearningBox > 0 && user.Dictionary.LearningBox < user.Dictionary.Boxes.Count)
					return user.Dictionary.Boxes[user.Dictionary.LearningBox].Size == 0;
				else
					return true;    // this should never occur!
			}
		}

		/// <summary>
		/// Gets a value indicating whether [dictionary is ready for learning].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [dictionary ready for learning]; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool DictionaryReadyForLearning
		{
			get
			{
				return (this.LearningModuleLoaded && !this.DictionaryNoChapters && !this.DictionaryNoQueryChapters && !this.DictionaryNoCards);
			}
		}

		/// <summary>
		/// Gets the card stack.
		/// </summary>
		/// <value>The card stack.</value>
		/// <remarks>Documented by Dev02, 2008-04-22</remarks>
		public CardStack CardStack
		{
			get { return cardStack; }
		}

		/// <summary>
		/// Gets the current card ID.
		/// </summary>
		/// <value>The current card ID.</value>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		public int CurrentCardID
		{
			get { return currentCardID; }
		}

		/// <summary>
		/// Gets a value indicating whether the user session is alive.
		/// </summary>
		/// <value><c>true</c> if user session is alive; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		public bool UserSessionAlive
		{
			get
			{
				try { return Dictionary != null ? Dictionary.DictionaryDAL.CheckUserSession() : true; }
				catch (NpgsqlException exp)
				{
					if (exp.TargetSite.Name == "Open")
					{
						StopCountdownTimer();
						user.Dictionary.Close();
						user.Dictionary = null;
						OnLearningModuleClosed(EventArgs.Empty);
						audioPlayer.StopThread(false);

						throw new ConnectionLostException();
					}
					else
						throw;
				}
				catch (UserSessionInvalidException) { OnUserSessionClosed(this, EventArgs.Empty); return false; }
			}
		}
		#endregion

		#region Learning Module Opening/Closing/Restart

		/// <summary>
		/// Occurs when [learning module closing].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		public event LearningModuleClosingEventHandler LearningModuleClosing;

		public delegate void LearningModuleClosingEventHandler(object sender, LearningModuleClosingEventArgs e);

		/// <summary>
		/// Raises the <see cref="E:LearningModuleClosing"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		private void OnLearningModuleClosing(LearningModuleClosingEventArgs e)
		{
			if (LearningModuleClosing != null)
				LearningModuleClosing(this, e);
		}

		/// <summary>
		/// Occurs when [learning module closed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		public event EventHandler LearningModuleClosed;

		/// <summary>
		/// Raises the <see cref="E:LearningModuleClosed"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		private void OnLearningModuleClosed(EventArgs e)
		{
			if (CurrentLearningModule.ConnectionString.SyncType != SyncType.NotSynchronized)
				if (LearningModuleSyncRequest != null)
					LearningModuleSyncRequest(CurrentLearningModule, EventArgs.Empty);

			if (LearningModuleClosed != null)
				LearningModuleClosed(this, e);

			if (CurrentLearningModule.User != null)
				CurrentLearningModule.User.Logout();
			else if (CurrentLearningModule.ConnectionString.ServerUser != null)
				CurrentLearningModule.User.Logout();

			CurrentLearningModule = null;
		}

		/// <summary>
		/// Occurs when [learning module opened].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		public event EventHandler LearningModuleOpened;

		/// <summary>
		/// Raises the <see cref="E:LearningModuleOpened"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-20</remarks>
		private void OnLearningModuleOpened(EventArgs e)
		{
			if (LearningModuleOpened != null)
				LearningModuleOpened(this, e);
		}

		/// <summary>
		/// Handles the LearningModuleOpened event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-20</remarks>
		void LearnLogic_LearningModuleOpened(object sender, EventArgs e)
		{
			user.Dictionary.PoolEmpty += new EventHandler(dictionary_PoolEmpty);
			user.Dictionary.DictionaryDAL.BackupCompleted += new BackupCompletedEventHandler(DictionaryDAL_BackupCompleted);
		}

		/// <summary>
		/// Handles the BackupCompleted event of the DictionaryDAL control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="MLifter.DAL.BackupCompletedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		void DictionaryDAL_BackupCompleted(object sender, BackupCompletedEventArgs args)
		{
			BackupCompletedNotifyDialogEventArgs e = new BackupCompletedNotifyDialogEventArgs(Dictionary, -1, args.BackupFilename);
			OnUserDialog(e);
		}

		/// <summary>
		/// Gets a value indicating whether a [learning module is loaded].
		/// </summary>
		/// <value><c>true</c> if [learning module loaded]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		public bool LearningModuleLoaded { get { return user.Dictionary != null; } }

		private bool learningModuleClosing = false;
		public bool LearningModuleIsClosing { get { return learningModuleClosing; } }

		/// <summary>
		/// Occurs when learning module connection is lost (e.g. Server offline).
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-15</remarks>
		public event EventHandler LearningModuleConnectionLost;
		/// <summary>
		/// Raises the <see cref="E:LearningModuleConnectionLost"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-05-15</remarks>
		protected virtual void OnLearningModuleConnectionLost(EventArgs e)
		{
			if (LearningModuleConnectionLost != null)
				LearningModuleConnectionLost(this, e);
		}

		/// <summary>
		/// Closes the learning module.
		/// </summary>
		/// <returns>True, if successful, else false.</returns>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		/// <remarks>Documented by Dev08, 2008-08-08</remarks>
		public bool CloseLearningModule()
		{
			try
			{
				if (this.LearningModuleLoaded)
				{
					LearningModuleClosingEventArgs e = new LearningModuleClosingEventArgs();
					OnLearningModuleClosing(e);
					if (e.Cancel)
						return false;

					Log.CloseUserSession(Dictionary.DictionaryDAL.Parent);

					while (!user.Dictionary.Save())
					{
						UserModuleNotSavedDialogEventArgs args = new UserModuleNotSavedDialogEventArgs(user.Dictionary, -1);
						OnUserDialog(args);
						if (args.cancelClosing)
							return false;
						if (!args.tryagain)
							break;
					}

					StopCountdownTimer();
					user.Dictionary.Close();
					user.Dictionary = null;
					OnLearningModuleClosed(EventArgs.Empty);
				}
			}
			catch (NpgsqlException exp)
			{
				if (exp.TargetSite.Name == "Open")
					OnLearningModuleConnectionLost(EventArgs.Empty);
				else
					throw;

				StopCountdownTimer();
				user.Dictionary.Close();
				user.Dictionary = null;
				OnLearningModuleClosed(EventArgs.Empty);
				audioPlayer.StopThread(false);
			}

			return !this.LearningModuleLoaded; //success when the module is not open anymore
		}

		/// <summary>
		/// Closes the learning module without saving.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public void CloseLearningModuleWithoutSaving()
		{
			if (this.LearningModuleLoaded)
			{
				learningModuleClosing = true;

				Log.CloseUserSession(Dictionary.DictionaryDAL.Parent);

				StopCountdownTimer();
				user.Dictionary.Close();
				user.Dictionary = null;
				OnLearningModuleClosed(EventArgs.Empty);

				learningModuleClosing = false;
			}
		}

		/// <summary>
		/// Opens the learning module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <remarks>Documented by Dev05, 2009-03-02</remarks>
		public void OpenLearningModule(LearningModulesIndexEntry module)
		{
			try
			{
				if (module == null) throw new ArgumentNullException("module");
				ConnectionStringStruct css;
				string fileName = module.ConnectionString.ConnectionString;

				if (!CloseLearningModule())
					throw new CouldNotCloseLearningModuleException();

				CurrentLearningModule = module;
				css = module.ConnectionString;
				if (module.User != null)
					css.SessionId = module.User.ConnectionString.SessionId;

				switch (module.ConnectionString.Typ)
				{
					#region not synced connections
					case DatabaseType.Xml:
						if (!File.Exists(fileName))
							throw new FileNotFoundException(fileName);

						if (Path.GetExtension(fileName).Equals(DAL.Helper.DipExtension))
							throw new DipNotSupportedException();
						else if (Path.GetExtension(fileName).Equals(DAL.Helper.ZipExtension) || Path.GetExtension(fileName).Equals(DAL.Helper.DzpExtension))
							throw new NeedToUnPackException(module);
						else if (DAL.Helper.IsOdfFormat(fileName))
							throw new IsOdfFormatException(module);
						else if (DAL.Helper.IsOdxFormat(fileName))
							throw new IsOdxFormatException(module);

						throw new ArgumentException("XML-File are not valid anymore!");
					case DatabaseType.MsSqlCe:
						css = module.ConnectionString;
						break;
					#endregion
					#region synced connections
					case DatabaseType.PostgreSQL:
						switch (module.ConnectionString.SyncType)
						{
							case SyncType.NotSynchronized:
								break;
							case SyncType.HalfSynchronizedWithDbAccess:
								css.Typ = DatabaseType.MsSqlCe;
								if (module.User != null)
								{
									ConnectionStringStruct mcss = module.ConnectionString;
									mcss.SessionId = module.User.ConnectionString.SessionId;
									module.User.ConnectionString = mcss;
									css.ServerUser = module.User;
								}
								else
									css.ServerUser = new DummyUser(module.UserId, module.UserName);
								css.ConnectionString = Tools.GetFullSyncPath(module.SyncedPath, SyncedLearningModulesPath, module.ConnectionName, module.UserName);
								break;
							case SyncType.HalfSynchronizedWithoutDbAccess:
								css.Typ = DatabaseType.MsSqlCe;
								if (module.User != null)
								{
									ConnectionStringStruct mcssPHS = module.ConnectionString;
									mcssPHS.SessionId = module.User.ConnectionString.SessionId;
									module.User.ConnectionString = mcssPHS;
									css.ServerUser = module.User;
								}
								else
									css.ServerUser = new DummyUser(module.UserId, module.UserName);
								css.ConnectionString = Tools.GetFullSyncPath(module.SyncedPath, SyncedLearningModulesPath, module.ConnectionName, module.UserName);
								break;
							case SyncType.FullSynchronized:
								css.Typ = DatabaseType.MsSqlCe;
								if (module.User != null)
								{
									ConnectionStringStruct mcssPFS = module.ConnectionString;
									mcssPFS.SessionId = module.User.ConnectionString.SessionId;
									module.User.ConnectionString = mcssPFS;
									css.ServerUser = module.User;
								}
								else
									css.ServerUser = new DummyUser(module.UserId, module.UserName);
								css.ConnectionString = Tools.GetFullSyncPath(module.SyncedPath, SyncedLearningModulesPath, module.ConnectionName, module.UserName);
								break;
							default:
								throw new ArgumentException("Not a valid SyncType!");
						}
						break;
					case DatabaseType.Web:
						switch (module.ConnectionString.SyncType)
						{
							case SyncType.HalfSynchronizedWithoutDbAccess:
								css.Typ = DatabaseType.MsSqlCe;
								if (module.User != null)
								{
									ConnectionStringStruct mcssWHS = module.ConnectionString;
									mcssWHS.SessionId = module.User.ConnectionString.SessionId;
									module.User.ConnectionString = mcssWHS;
									css.ServerUser = module.User;
								}
								else
									css.ServerUser = new DummyUser(module.UserId, module.UserName);
								css.ConnectionString = Tools.GetFullSyncPath(module.SyncedPath, SyncedLearningModulesPath, module.ConnectionName, module.UserName);
								break;
							case SyncType.FullSynchronized:
								css.Typ = DatabaseType.MsSqlCe;
								if (module.User != null)
								{
									ConnectionStringStruct mcssWFS = module.ConnectionString;
									mcssWFS.SessionId = module.User.ConnectionString.SessionId;
									module.User.ConnectionString = mcssWFS;
									css.ServerUser = module.User;
								}
								else
									css.ServerUser = new DummyUser(module.UserId, module.UserName);
								css.ConnectionString = Tools.GetFullSyncPath(module.SyncedPath, SyncedLearningModulesPath, module.ConnectionName, module.UserName);
								break;
							case SyncType.NotSynchronized:
							case SyncType.HalfSynchronizedWithDbAccess:
							default:
								throw new ArgumentException("Not a valid SyncType!");
						}
						break;
					#endregion
					#region other stuff
					case DatabaseType.Unc:
					default:
						throw new ArgumentException("Not a valid DatabaseType!");
					#endregion
				}

				if (module.User != null && module.User.CanOpen)
				{
					if (module.ConnectionString.SyncType != SyncType.HalfSynchronizedWithDbAccess)
					{
						module.User.ConnectionString = css;
						User.SetBaseUser(module.User);
					}
					else
					{
						User.Authenticate((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return module.User.AuthenticationStruct; }, css, DataAccessErrorDelegate);
						User.BaseUser.ConnectionString = css;
					}
				}
				else
				{
					/////////////////////////////////////////////
					//catch if the learning module is protected//
					/////////////////////////////////////////////
					#region Authenticate User
					try
					{
						User.Authenticate(GetLoginDelegate, css, DataAccessErrorDelegate);
					}
					catch (MLifter.Generics.ProtectedLearningModuleException)
					{
						throw;
					}
					#endregion

					User.BaseUser.ConnectionString = css;
				}

				if (!this.user.OpenLearningModule())
					throw new CouldNotOpenLearningModuleException();

				//process LearningModule Extensions
				this.user.Dictionary.Extensions.Process();

				cardStack.Clear();
				this.user.StartLearningSession();
				this.user.Dictionary.AnswerStyleSheet = AnswerStyleSheet;
				this.user.Dictionary.QuestionStyleSheet = QuestionStyleSheet;
				OnLearningModuleOpened(EventArgs.Empty);
				OnLearningModuleOptionsChanged();
			}
			catch (NpgsqlException exp)
			{
				if (exp.TargetSite.Name == "Open")
					throw new ServerOfflineException();
				else
					throw;
			}
		}

		/// <summary>
		/// Imports the learning module.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="source">The source.</param>
		/// <param name="getLogin">The get login.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <param name="errorMessageDelegate">The error message delegate.</param>
		/// <param name="user">The user.</param>
		/// <param name="calCount">The cal count.</param>
		/// <remarks>Documented by Dev05, 2009-02-12</remarks>
		public static void ImportLearningModule(IConnectionString target, ConnectionStringStruct source,
			GetLoginInformation getLogin, CopyToProgress progressDelegate, DataAccessErrorDelegate errorMessageDelegate, User user, string licenseKey, bool contentProtected, int calCount)
		{
			IDictionary dic = LearningModulesIndex.ConnectionUsers[target].List().AddNew(1, string.Empty, licenseKey, contentProtected, calCount);
			ConnectionStringStruct targetConnection = new ConnectionStringStruct(target.ConnectionType, dic.Connection, dic.Id);
			CopyLearningModule(source, targetConnection, getLogin, progressDelegate, errorMessageDelegate, user, true, contentProtected);
		}

		/// <summary>
		/// Copies the contents of a learning module to another one.
		/// </summary>
		/// <param name="connectionSource">The connection source.</param>
		/// <param name="connectionTarget">The connection target.</param>
		/// <param name="getLogin">The get login delegate.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <param name="errorMessageDelegate">The error message delegate.</param>
		/// <param name="user">The currently logged in user.</param>
		/// <remarks>Documented by Dev02, 2008-09-24</remarks>
		/// <exception cref="DictionaryContentProtectedException"></exception>
		public static void CopyLearningModule(ConnectionStringStruct connectionSource, ConnectionStringStruct connectionTarget,
			GetLoginInformation getLogin, CopyToProgress progressDelegate, DataAccessErrorDelegate errorMessageDelegate, User user)
		{
			CopyLearningModule(connectionSource, connectionTarget, getLogin, progressDelegate, errorMessageDelegate, user, false);
		}

		/// <summary>
		/// Copies the contents of a learning module to another one.
		/// </summary>
		/// <param name="connectionSource">The connection source.</param>
		/// <param name="connectionTarget">The connection target.</param>
		/// <param name="getLogin">The get login delegate.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <param name="errorMessageDelegate">The error message delegate.</param>
		/// <param name="user">The currently logged in user.</param>
		/// <param name="resetAfterCopy">if set to <c>true</c> to reset after copy.</param>
		/// <remarks>Documented by Dev02, 2008-09-24</remarks>
		/// <exception cref="DictionaryContentProtectedException"></exception>
		public static void CopyLearningModule(ConnectionStringStruct connectionSource, ConnectionStringStruct connectionTarget,
			GetLoginInformation getLogin, CopyToProgress progressDelegate, DataAccessErrorDelegate errorMessageDelegate, User user, bool resetAfterCopy)
		{
			CopyLearningModule(connectionSource, connectionTarget, getLogin, progressDelegate, errorMessageDelegate, user, resetAfterCopy, false);
		}

		/// <summary>
		/// Copies the contents of a learning module to another one.
		/// </summary>
		/// <param name="connectionSource">The connection source.</param>
		/// <param name="connectionTarget">The connection target.</param>
		/// <param name="getLogin">The get login delegate.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <param name="errorMessageDelegate">The error message delegate.</param>
		/// <param name="user">The currently logged in user.</param>
		/// <param name="resetAfterCopy">if set to <c>true</c> to reset after copy.</param>
		/// <remarks>Documented by Dev02, 2008-09-24</remarks>
		/// <exception cref="DictionaryContentProtectedException"></exception>
		private static void CopyLearningModule(ConnectionStringStruct connectionSource, ConnectionStringStruct connectionTarget,
			GetLoginInformation getLogin, CopyToProgress progressDelegate, DataAccessErrorDelegate errorMessageDelegate, User user, bool resetAfterCopy, bool ignoreProtected)
		{
			bool CurrentUserTryed;
			UserStruct? CurrentUser = null;
			try
			{
				if (user != null)
				{
					UserStruct cUser = user.BaseUser.AuthenticationStruct;
					cUser.CloseOpenSessions = true;
					cUser.UserName = cUser.UserName ?? string.Empty;
					cUser.AuthenticationType = cUser.AuthenticationType ?? UserAuthenticationTyp.ListAuthentication;
					CurrentUser = cUser;
				}
			}
			catch { }

			User userSource = new User(null);
			User userTarget = new User(null);

			try
			{
				CurrentUserTryed = false;
				if (connectionSource.Typ == DatabaseType.Xml)   //XML needs to be in read-only mode otherwise the file would be locked
					connectionSource.ReadOnly = true;
				userSource.SetBaseUser(UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c)
					{
						if (!CurrentUserTryed && CurrentUser.HasValue)
						{
							CurrentUserTryed = true;
							return CurrentUser;
						}

						return getLogin.Invoke(u, c);
					}, connectionSource, errorMessageDelegate, userSource, true));

				if (!userSource.OpenLearningModule())
					return;

				//don't allow import of protected LMs
				if (!ignoreProtected && userSource.Dictionary.DictionaryContentProtected)
					throw new DictionaryContentProtectedException();

				CurrentUserTryed = false;
				userTarget.SetBaseUser(UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c)
					{
						if (!CurrentUserTryed && CurrentUser.HasValue)
						{
							CurrentUserTryed = true;
							return CurrentUser;
						}

						return getLogin.Invoke(u, c);
					}, connectionTarget, errorMessageDelegate, userTarget, true));

				if (!userTarget.OpenLearningModule())
					return;

				userSource.Dictionary.PreloadCardCache();

				//do copying
				userSource.Dictionary.CopyToFinished += new Dictionary.CopyToFinishedEventHandler(Dictionary_CopyToFinished);
				userSource.Dictionary.BeginCopyTo(userTarget.Dictionary, progressDelegate, userSource, userTarget, resetAfterCopy);
			}
			catch (Exception exp)
			{
				//clean up
				if (userSource != null && userSource.Dictionary != null)
					userSource.Dictionary.Dispose();

				if (userTarget != null && userTarget.Dictionary != null)
					userTarget.Dictionary.Dispose();

				Trace.WriteLine(exp.ToString());

				throw exp;
			}
		}

		/// <summary>
		/// Handles the CopyToFinished event of the Dictionary control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.CopyToEventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2008-09-26</remarks>
		static void Dictionary_CopyToFinished(object sender, CopyToEventArgs e)
		{
			//e.source.Save();
			e.source.Dispose();
			e.sourceUser.Logout();

			if (e.success)
			{
				e.target.Save();
				e.target.Dispose();
				e.targetUser.Logout();
			}
			else
			{
				e.target.Dispose();
				try
				{
					e.targetUser.BaseUser.List().Delete(e.targetUser.BaseUser.ConnectionString);
				}
				catch { }
				e.targetUser.Logout();
			}

			//send EventArg to Interface, so that this can react on this.
			OnCopyToFinished(e);
		}

		/// <summary>
		/// This Eventhandler is necessary for the U-interface to register on it. It will de-lock (e.g.) the Wizard after the Import is finished.
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-09-26</remarks>
		public static event EventHandler CopyToFinished;
		/// <summary>
		/// Raises the <see cref="E:CopyToFinished"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-09-26</remarks>
		public static void OnCopyToFinished(EventArgs e)
		{
			//if (CopyToFinished != null)
			//    CopyToFinished(null, e);
			EventHandler handler = CopyToFinished;
			if (handler != null)
			{
				foreach (EventHandler caster in handler.GetInvocationList())
				{
					ISynchronizeInvoke SyncInvoke = caster.Target as ISynchronizeInvoke;
					try
					{
						if (SyncInvoke != null && SyncInvoke.InvokeRequired)
							SyncInvoke.Invoke(handler, new object[] { null, e });
						else
							caster(null, e);
					}
					catch { }
				}
			}
		}

		/// <summary>
		/// Saves the learning module.
		/// </summary>
		/// <returns>True, if successful, else false.</returns>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool SaveLearningModule()
		{
			if (this.LearningModuleLoaded)
				return user.Dictionary.Save();

			return true;
		}

		/// <summary>
		/// Resets the learning progress.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		public void ResetLearningProgress()
		{
			Log.CloseUserSession(user.Dictionary.DictionaryDAL.Parent);

			CardStack.Clear();
			user.Dictionary.ResetLearningProgress();

			OnLearningModuleOptionsChanged();
		}

		#endregion

		#region Learn Settings Fields and Properties
		bool slideshow = false;
		int countdownTimerMinimum = 20;
		string questionStyleSheet = string.Empty;
		string answerStyleSheet = string.Empty;
		bool ignoreOldLearningModuleVersion = false;
		bool snoozeModeActive = false;
		bool synonymInfoMessage = true;

		/// <summary>
		/// Gets or sets a value indicating whether [slide show] mode is enabled.
		/// </summary>
		/// <value><c>true</c> if [slide show]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public bool SlideShow
		{
			get { return slideshow; }
			set { if (slideshow != value) { slideshow = value; OnLearningModuleOptionsChanged(); } }
		}

		/// <summary>
		/// Gets or sets the minimum value for the count down timer.
		/// </summary>
		/// <value>The count down timer minimum.</value>
		/// <remarks>Documented by Dev02, 2008-05-05</remarks>
		public int CountDownTimerMinimum
		{
			get { return countdownTimerMinimum; }
			set { countdownTimerMinimum = value; }
		}

		/// <summary>
		/// Gets or sets the question style sheet.
		/// </summary>
		/// <value>The question style sheet.</value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public string QuestionStyleSheet
		{
			get { return questionStyleSheet; }
			set { questionStyleSheet = value; if (LearningModuleLoaded) user.Dictionary.QuestionStyleSheet = value; }
		}

		/// <summary>
		/// Gets or sets the answer style sheet.
		/// </summary>
		/// <value>The answer style sheet.</value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public string AnswerStyleSheet
		{
			get { return answerStyleSheet; }
			set { answerStyleSheet = value; if (LearningModuleLoaded) user.Dictionary.AnswerStyleSheet = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to [ignore old learning module version].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [ignore old learning module version]; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool IgnoreOldLearningModuleVersion
		{
			get { return ignoreOldLearningModuleVersion; }
			set { ignoreOldLearningModuleVersion = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [snooze mode is activated].
		/// </summary>
		/// <value><c>true</c> if [snooze mode]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public bool SnoozeModeActive
		{
			get { return snoozeModeActive; }
			set
			{
				if (snoozeModeActive != value)
				{
					snoozeModeActive = value;
					OnLearningModuleOptionsChanged();

					if (snoozeModeActive)
						Log.CloseUserSession(Dictionary.DictionaryDAL.Parent);
					else
						Log.OpenUserSession(Dictionary.DictionaryDAL.Id, Dictionary.DictionaryDAL.Parent);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [synonym info message] is to show.
		/// </summary>
		/// <value><c>true</c> if [synonym info message]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-05-07</remarks>
		public bool SynonymInfoMessage
		{
			get { return synonymInfoMessage; }
			set { synonymInfoMessage = value; }
		}
		#endregion

		#region User interface communication
		/// <summary>
		/// Called when [the user submits input].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.UserInputSubmitEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public void OnUserInputSubmit(object sender, UserInputSubmitEventArgs e)
		{
			OnLearnLogicIsWorking();

			try
			{
				if (!UserSessionAlive)
					return;

				//deactivate countdown timer
				if (countdownTimer != null && countdownTimer.Enabled)
				{
					countdownTimer.Stop();
					countdownTimer.Dispose();
				}

				//store answered timestamp
				if (!(e is UserInputSubmitSelfAssessmentResponseEventArgs))
					currentCardAnswered = DateTime.Now.ToUniversalTime();

				Debug.WriteLine("User input submitted: " + currentCardID);

				//evaluate answer
				if (e is UserInputSubmitMultipleChoiceEventArgs)
				{
					//the user has submitted a multiple choice choice
					UserInputSubmitMultipleChoiceEventArgs args = (UserInputSubmitMultipleChoiceEventArgs)e;
					bool promote = (args.result == MultipleChoiceResult.Correct);
					AnswerResult result = promote ? AnswerResult.Correct : AnswerResult.Wrong;

					if (!user.Dictionary.Settings.SelfAssessment.Value)
						PromoteDemoteCard(ref promote, result, args.answer, 0, false, false);

					//show the result
					if (!user.Dictionary.Settings.SelfAssessment.Value && user.Dictionary.Settings.SkipCorrectAnswers.Value && promote)
						AskNextCard();
					else
					{
						CardStateChangedShowResultEventArgs cardstateargs = new CardStateChangedShowResultEventArgs(
							user.Dictionary, currentCardID, promote, result, args.answer, false, false);
						OnCardStateChanged(cardstateargs);
					}
				}
				else if (e is UserInputSubmitSelfAssessmentResponseEventArgs)
				{
					//the user has clicked on a button in self assessment mode
					if (user.Dictionary.Settings.SelfAssessment.Value)
					{
						UserInputSubmitSelfAssessmentResponseEventArgs args = (UserInputSubmitSelfAssessmentResponseEventArgs)e;
						if (args.dontaskagain)
							user.Dictionary.EnableCard(currentCardID, false);
						else
						{
							bool promote = args.doknow;
							PromoteDemoteCard(ref promote, args.result, args.answer, 0, false, false);
						}
					}
					AskNextCard();
				}
				else if (e is UserInputSubmitDeactivateCardEventArgs)
				{
					//the user has clicked the deactivate card button
					user.Dictionary.EnableCard(currentCardID, false);
					if (slideshow)
						Dictionary.LoadSlideShowContent();
					AskNextCard();
				}
				else if (e is UserInputSubmitSlideshowEventArgs)
				{
					//the slide show mode card has been seen, time for the next card
					UserInputSubmitSlideshowEventArgs args = (UserInputSubmitSlideshowEventArgs)e;

					//set the card used timestamp
					user.Dictionary.CardUsed(currentCardID);

					if (args.stopslideshow)
						this.SlideShow = false;
					else
						AskNextCard();
				}
				else if (e is UserInputSubmitTextEventArgs)
				{
					//the user has submitted a simple Text answer
					UserInputSubmitTextEventArgs args = (UserInputSubmitTextEventArgs)e;

					//in case any grading dialog is active => show result before
					if (user.Dictionary.Settings.ConfirmDemote.Value || user.Dictionary.Settings.GradeSynonyms.Prompt.Value ||
						(user.Dictionary.Settings.CorrectOnTheFly.Value && user.Dictionary.Settings.GradeTyping.Prompt.Value))
					{
						int typingErrors = 0;
						bool promoted = (args.correctsynonyms > 0);
						AnswerResult result_tmp = (args.correctsynonyms > 0) ? AnswerResult.Correct : AnswerResult.Wrong;

						if (user.Dictionary.Settings.CorrectOnTheFly.Value) //Override if Typing Mode is activated (Correct on the fly)
						{
							typingErrors = args.typingerrors;
							promoted = (typingErrors == 0);
							result_tmp = (typingErrors == 0) ? AnswerResult.Correct : AnswerResult.Wrong;
						}

						OnCardStateChanged(new CardStateChangedShowResultEventArgs(user.Dictionary, currentCardID, promoted, result_tmp, args.answer, false, true));
					}

					AnswerResult result;
					bool overrideSynonyms, overrideTyping;
					bool promote = GradeTextAnswer(args, out result, out overrideSynonyms, out overrideTyping);

					if (!user.Dictionary.Settings.SelfAssessment.Value)
						PromoteDemoteCard(ref promote, result, args.answer, args.correctsynonyms, overrideSynonyms, overrideTyping);

					//show the result
					if (user.Dictionary.Settings.SkipCorrectAnswers.Value && promote)
						AskNextCard();
					else
					{
						CardStateChangedShowResultEventArgs cardstateargs = new CardStateChangedShowResultEventArgs(
							user.Dictionary, currentCardID, promote, result, args.answer, false, false);
						OnCardStateChanged(cardstateargs);
					}
				}
				else if (e is UserInputSubmitPreviousCard)
				{
					//display previous card
					AskPreviousCard();
				}
				else
				{
					//display next card
					AskNextCard();
				}
			}
			catch (Npgsql.NpgsqlException ex)
			{
				throw new ServerOfflineException(ex);
			}
			finally
			{
				OnLearnLogicIsIdle();
			}
		}

		public delegate void CardStateChangedEventHandler(object sender, CardStateChangedEventArgs e);
		/// <summary>
		/// Occurs when [card state changed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public event CardStateChangedEventHandler CardStateChanged;

		/// <summary>
		/// Raises the <see cref="E:CardStateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.CardEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		private void OnCardStateChanged(CardStateChangedEventArgs e)
		{
			if (CardStateChanged != null)
				CardStateChanged(this, e);
			else
				Trace.WriteLine("Warning: No subscribers for CardStateChanged event!");
		}

		/// <summary>
		/// Occurs when [learning module options changed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		public event EventHandler LearningModuleOptionsChanged;

		/// <summary>
		/// Called when [options changed], and the card display needs to be refreshed.
		/// </summary>
		/// <value>The options changed.</value>
		/// <remarks>Documented by Dev02, 2008-04-25</remarks>
		public void OnLearningModuleOptionsChanged()
		{
			if (LearningModuleOptionsChanged != null)
				LearningModuleOptionsChanged(this, EventArgs.Empty);

			//ML-1844
			if (Dictionary == null)
				return;

			if (slideshow)
				Dictionary.LoadSlideShowContent();

			if (DictionaryReadyForLearning && !SnoozeModeActive)
				AskNextCard();
		}

		/// <summary>
		/// Called when [new LM loaded].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		public void OnNewLearningModuleLoaded()
		{
			if (LearningModuleOptionsChanged != null)
				LearningModuleOptionsChanged(this, EventArgs.Empty);

			this.CardStack.Clear();
			this.SlideShow = false;
		}

		public delegate void UserDialogEventHandler(object sender, UserDialogEventArgs e);

		/// <summary>
		/// Occurs when [a user dialog is needed].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		public event UserDialogEventHandler UserDialog;

		/// <summary>
		/// Raises the <see cref="E:UserDialog"/> event.
		/// </summary>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.UserGradingDialogEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		public void OnUserDialog(UserDialogEventArgs e)
		{
			if (UserDialog != null)
				UserDialog(this, e);
			else
				Trace.WriteLine("Warning: No subscribers for UserDialog event!");
		}

		/// <summary>
		/// Adds the specified audio file to the audio player playback queue.
		/// </summary>
		/// <param name="filepath">The filepath.</param>
		/// <param name="clearQueue">if set to <c>true</c> [clear queue].</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		public void PlayAudioFile(string filepath, bool clearQueue)
		{
			try
			{
				if (audioPlayer != null && !string.IsNullOrEmpty(filepath))
					audioPlayer.Play(filepath, clearQueue);
			}
			catch (Exception exp)
			{
				Trace.WriteLine("PlayAudioFile Exception: " + exp.ToString());
			}
		}

		/// <summary>
		/// Terminates the audioplayer thread.
		/// </summary>
		/// <param name="stopCurrentPlay">if set to <c>true</c> [stop current play].</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		public void TerminateAudioplayerThread(bool stopCurrentPlay)
		{
			if (audioPlayer != null)
				audioPlayer.StopThread(stopCurrentPlay);
		}

		/// <summary>
		/// Occurs when [snooze mode should quit the program].
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-14</remarks>
		public event EventHandler SnoozeModeQuitProgram;

		/// <summary>
		/// Occurs when the user session is closed.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		public event EventHandler UserSessionClosed;
		/// <summary>
		/// Called when the user session was closed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-11-18</remarks>
		public void OnUserSessionClosed(object sender, EventArgs e)
		{
			if (UserSessionClosed != null)
				UserSessionClosed(sender, e);
		}

		#endregion

		#region Learning/Grading

		/// <summary>
		/// Asks the previous card.
		/// </summary>
		/// <remarks>Documented by Dev07, 2009-04-10</remarks>
		private void AskPreviousCard()
		{
			if (!SlideShow)
				return;

			if (!UserSessionAlive)
				return;

			if (user.Dictionary == null)
				return;

			//check wether it's time to enable the snooze mode
			if (!SnoozeModeActive && cardStack.CheckSnooze(user.Dictionary.Settings.SnoozeOptions))
			{
				if (Dictionary.Settings.SnoozeOptions.SnoozeMode == MLifter.DAL.Interfaces.ESnoozeMode.SendToTray)
				{
					this.SnoozeModeActive = true;
					return;
				}
				if (Dictionary.Settings.SnoozeOptions.SnoozeMode == MLifter.DAL.Interfaces.ESnoozeMode.QuitProgram)
				{
					if (SnoozeModeQuitProgram != null)
						SnoozeModeQuitProgram(this, EventArgs.Empty);
				}
			}

			//get previous card
			ICard pCard = Dictionary.GetPreviousSlide();
			currentCardID = pCard == null ? -1 : pCard.Id;

			if (currentCardID == -1)
			{
				OnUserDialog(new UserNotifyDialogEventArgs(user.Dictionary, -1, UserNotifyDialogEventArgs.NotifyDialogKind.NoWords));
				return;
			}

			Debug.WriteLine("Previous card asked: " + currentCardID);

			//save timestamp
			currentCardAsked = DateTime.Now.ToUniversalTime();

			//set the current directory for the browsers
			System.IO.Directory.SetCurrentDirectory(user.Dictionary.DirectoryName);

			OnCardStateChanged(new CardStateChangedShowResultEventArgs(user.Dictionary, currentCardID, true, AnswerResult.Correct, string.Empty, SlideShow, false));
		}

		/// <summary>
		/// Asks a new card.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		private void AskNextCard()
		{
			if (!UserSessionAlive)
				return;

			if (Dictionary == null || Dictionary.Settings == null)
				return;

			//check wether it's time to enable the snooze mode
			if (!SnoozeModeActive && Dictionary.Settings.SnoozeOptions != null && Dictionary.Settings.SnoozeOptions.SnoozeMode.HasValue)
			{
				if (cardStack.CheckSnooze(Dictionary.Settings.SnoozeOptions))
				{
					if (Dictionary.Settings.SnoozeOptions.SnoozeMode == MLifter.DAL.Interfaces.ESnoozeMode.SendToTray)
					{
						this.SnoozeModeActive = true;
						return;
					}
					if (Dictionary.Settings.SnoozeOptions.SnoozeMode == MLifter.DAL.Interfaces.ESnoozeMode.QuitProgram)
					{
						if (SnoozeModeQuitProgram != null)
						{
							SnoozeModeQuitProgram(this, EventArgs.Empty);
							return;
						}
					}
				}
			}

			//get next card
			ICard cCard = SlideShow ? Dictionary.GetNextSlide() : Dictionary.GetNextCard();

			if (cCard == null)
			{
				currentCardID = -1;
				OnUserDialog(new UserNotifyDialogEventArgs(Dictionary, -1, UserNotifyDialogEventArgs.NotifyDialogKind.NoWords));
				OnLearningModuleOptionsChanged();
				return;
			}

			currentCardID = cCard.Id;

			//Use card / chapter settings
			if (!SlideShow)
			{
				hasLearnModeChangedAndNotifyUser = false;
				originalLearnMode = Dictionary.LearnMode;
				int currentChapterId = cCard.Chapter;
				CheckAndAdjustSettings(Dictionary.Chapters.GetChapterByID(currentChapterId).Settings);
				CheckAndAdjustSettings(cCard.Settings);

				//check if there are enough choices available for mutliple choice
				hasMultipleChoiceLearnModeChanged = false;
				if (Dictionary.LearnMode == LearnModes.MultipleChoice &&
					!Dictionary.ChoicesAvailable(cCard, Dictionary.CurrentMultipleChoiceOptions.NumberOfChoices.Value))
				{
					hasMultipleChoiceLearnModeChanged = true;
					Dictionary.LearnMode = LearnModes.Word; //switch to fail-safe word mode
				}

				//check if image / audio is available, switch back to fail-safe word mode otherwise
				if ((Dictionary.LearnMode == LearnModes.ImageRecognition && !Dictionary.Cards.ImageAvailable(cCard, Side.Question))
				 || (Dictionary.LearnMode == LearnModes.ListeningComprehension && !Dictionary.Cards.AudioAvailable(cCard, Side.Question))
				 || (Dictionary.LearnMode == LearnModes.Sentence && !Dictionary.Cards.SentenceAvailable(cCard, Side.Answer)))
				{
					originalLearnMode = Dictionary.LearnMode;
					hasLearnModeChangedAndNotifyUser = true;
					Dictionary.LearnMode = LearnModes.Word;
				}
			}
			else
			{
				//disable timer if enabled
				if (Dictionary.Settings.EnableTimer.HasValue && Dictionary.Settings.EnableTimer.Value)
					StopCountdownTimer();
			}

			Debug.WriteLine("New card asked: " + currentCardID);

			//save timestamp
			currentCardAsked = DateTime.Now.ToUniversalTime();

			//set the current directory for the browsers
			if (!String.IsNullOrEmpty(Dictionary.DirectoryName))
				System.IO.Directory.SetCurrentDirectory(Dictionary.DirectoryName);

			//fire the event to load the new card
			if (SlideShow)
				OnCardStateChanged(new CardStateChangedShowResultEventArgs(Dictionary, currentCardID, true, AnswerResult.Correct, string.Empty, SlideShow, false));
			else
				OnCardStateChanged(new CardStateChangedNewCardEventArgs(Dictionary, currentCardID));

			//activate countdown timer
			if (Dictionary.Settings.EnableTimer.HasValue && Dictionary.Settings.EnableTimer.Value && !SlideShow)
			{
				//determine the countdown timer out of the card
				Card currentCard = new Card(cCard, Dictionary);
				int countdownseconds = 0;

				if (currentCard != null && currentCard.CurrentAnswer != null)
					countdownseconds = currentCard.CurrentAnswer.ToQuotedString().Length + 10 * currentCard.CurrentAnswer.Words.Count;

				if (countdownseconds < CountDownTimerMinimum)
					countdownseconds = CountDownTimerMinimum;

				CardStateChangedCountdownTimerEventArgs args = new CardStateChangedCountdownTimerEventArgs(Dictionary, currentCardID, countdownseconds);
				OnCardStateChanged(args);

				//deactivate countdown timer
				StopCountdownTimer();

				//set up new timer
				countdownTimer = new Timer();
				countdownTimer.Tag = args;
				countdownTimer.Interval = 1000;
				countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
				countdownTimer.Start();
			}

			#region Show all Infobars at least

			if (hasLearnModeChangedAndNotifyUser && !SlideShow)
				OnUserDialog(new UserNotifyDialogEventArgs(Dictionary, currentCardID, UserNotifyDialogEventArgs.NotifyDialogKind.SelectedLearnModeNotAllowed, originalLearnMode));
			if (hasMultipleChoiceLearnModeChanged && !SlideShow)
				OnUserDialog(new UserNotifyDialogEventArgs(Dictionary, currentCardID, UserNotifyDialogEventArgs.NotifyDialogKind.NotEnoughMultipleChoices));

			#endregion
		}

		/// <summary>
		/// Compares the card setting with the LM-LearnMode settings and adjust the user.Dictionary.LearnMode
		/// </summary>
		/// <param name="card">The card.</param>
		/// <param name="learnMode">LearnMode of the current card.</param>
		/// <remarks>Documented by Dev08, 2009-04-10</remarks>
		private void VerifyLearningModes(ISettings setting)
		{
			List<LearnModes> allowedLearnModes = ConvertToLearnModesList(setting.QueryTypes);
			List<LearnModes> selectedLearnModes = user.Dictionary.GetAllLearnModes();
			if (allowedLearnModes.Count > 0)         // the cards has a settings?
			{
				#region check if there is a intersection (schnittmenge) between the allowed settings and the selected settings.

				List<LearnModes> intersectionLearnModes = new List<LearnModes>();
				foreach (LearnModes lm in selectedLearnModes)
				{
					if (allowedLearnModes.Contains(lm))
						intersectionLearnModes.Add(lm);
				}

				//Select a random Learnmode from the intersection learnmodes.
				if (intersectionLearnModes.Count > 0)
				{
					user.Dictionary.LearnMode = intersectionLearnModes[new Random().Next(0, intersectionLearnModes.Count)];
					return;
				}

				#endregion

				if (allowedLearnModes.Count == 1)
				{
					user.Dictionary.LearnMode = allowedLearnModes[0];
					hasLearnModeChangedAndNotifyUser = true;
				}
				else
				{
					//Random rand = new Random((int)DateTime.Now.Ticks);
					//int index = (int)Math.Round(rand.Next(0, (allowedLearnModes.Count - 1) * 1000) / 1000.0);
					//user.Dictionary.LearnMode = allowedLearnModes[index];
					user.Dictionary.LearnMode = allowedLearnModes[new Random().Next(0, allowedLearnModes.Count)];
					hasLearnModeChangedAndNotifyUser = true;
				}
			}
		}

		/// <summary>
		/// Checks (and if necessary set) the card settings.
		/// Checks the LearnMode, QueryDirection
		/// </summary>
		/// <param name="card">The card.</param>
		/// <param name="learningModuleLearnMode">The learning module learn mode.</param>
		/// <remarks>Documented by Dev08, 2009-04-10</remarks>
		private void CheckAndAdjustSettings(ISettings setting)
		{
			if (setting == null)
				return;

			//1. Check/Adjust LearnMode
			VerifyLearningModes(setting);

			//2. Check/Adjust MC Options
			if (user.Dictionary.LearnMode == LearnModes.MultipleChoice)
			{
				bool? allowRandomCorrectAnswers = setting.MultipleChoiceOptions.AllowMultipleCorrectAnswers;
				bool? allowRandomDistractors = setting.MultipleChoiceOptions.AllowRandomDistractors;
				int? maxNumberOfCorrectAnswers = setting.MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
				int? numberOfChoices = setting.MultipleChoiceOptions.NumberOfChoices;

				if (allowRandomCorrectAnswers.HasValue)
					user.Dictionary.CurrentMultipleChoiceOptions.AllowMultipleCorrectAnswers = allowRandomCorrectAnswers.Value;
				if (allowRandomDistractors.HasValue)
					user.Dictionary.CurrentMultipleChoiceOptions.AllowRandomDistractors = allowRandomDistractors.Value;
				if (maxNumberOfCorrectAnswers.HasValue)
					user.Dictionary.CurrentMultipleChoiceOptions.MaxNumberOfCorrectAnswers = maxNumberOfCorrectAnswers.Value;
				if (numberOfChoices.HasValue)
					user.Dictionary.CurrentMultipleChoiceOptions.NumberOfChoices = numberOfChoices.Value;
			}

			//3. Check/Adjust LearningDirection
			bool? question2answer = setting.QueryDirections.Question2Answer;
			bool? answer2question = setting.QueryDirections.Answer2Question;
			bool? mixed = setting.QueryDirections.Mixed;
			switch (user.Dictionary.CurrentQueryDirection)
			{
				case EQueryDirection.Question2Answer:
					if (mixed.HasValue && mixed.Value)
					{
						Random rand = new Random((int)DateTime.Now.Ticks);
						int rnd = (int)Math.Round(rand.Next(0, 1000) / 1000.0);
						if (rnd == 0)
							user.Dictionary.CurrentQueryDirection = EQueryDirection.Answer2Question;
						else
							user.Dictionary.CurrentQueryDirection = EQueryDirection.Question2Answer;
					}
					else if (answer2question.HasValue && answer2question.Value)
					{
						user.Dictionary.CurrentQueryDirection = EQueryDirection.Answer2Question;
					}
					break;

				case EQueryDirection.Answer2Question:
					if (mixed.HasValue && mixed.Value)
					{
						Random rand = new Random((int)DateTime.Now.Ticks);
						int rnd = (int)Math.Round(rand.Next(0, 1000) / 1000.0);
						if (rnd == 0)
							user.Dictionary.CurrentQueryDirection = EQueryDirection.Answer2Question;
						else
							user.Dictionary.CurrentQueryDirection = EQueryDirection.Question2Answer;
					}
					else if (question2answer.HasValue && question2answer.Value)
					{
						user.Dictionary.CurrentQueryDirection = EQueryDirection.Question2Answer;
					}
					break;

				default:
					Trace.Write("Unkown or invalid QueryDirection...");
					break;
			}
		}

		/// <summary>
		/// A helper method.
		/// Converts a IQueryType object to a LearnModes list.
		/// </summary>
		/// <param name="queryType">IQueryType object</param>
		/// <returns>List of LearnModes</returns>
		/// <remarks>Documented by Dev08, 2009-04-09</remarks>
		private List<LearnModes> ConvertToLearnModesList(IQueryType queryType)
		{
			List<LearnModes> allowedLearnModes = new List<LearnModes>();

			if (queryType.Word.HasValue && queryType.Word.Value)
				allowedLearnModes.Add(LearnModes.Word);
			if (queryType.Sentence.HasValue && queryType.Sentence.Value)
				allowedLearnModes.Add(LearnModes.Sentence);
			if (queryType.MultipleChoice.HasValue && queryType.MultipleChoice.Value)
				allowedLearnModes.Add(LearnModes.MultipleChoice);
			if (queryType.ImageRecognition.HasValue && queryType.ImageRecognition.Value)
				allowedLearnModes.Add(LearnModes.ImageRecognition);
			if (queryType.ListeningComprehension.HasValue && queryType.ListeningComprehension.Value)
				allowedLearnModes.Add(LearnModes.ListeningComprehension);

			return allowedLearnModes;
		}

		/// <summary>
		/// Handles the PoolEmpty event of the dictionary control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-20</remarks>
		void dictionary_PoolEmpty(object sender, EventArgs e)
		{
			if (this.LearningModuleLoaded && !user.Dictionary.PoolEmptyMessageShown)
			{
				user.Dictionary.PoolEmptyMessageShown = true;
				OnUserDialog(new UserNotifyDialogEventArgs(user.Dictionary, -1, UserNotifyDialogEventArgs.NotifyDialogKind.PoolEmpty));
			}
		}

		/// <summary>
		/// Handles the Tick event of the countdownTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		void countdownTimer_Tick(object sender, EventArgs e)
		{
			if (sender == countdownTimer && countdownTimer.Tag is CardStateChangedCountdownTimerEventArgs)
			{
				if (LearningModuleLoaded && this.DictionaryReadyForLearning)
				{
					CardStateChangedCountdownTimerEventArgs args = (CardStateChangedCountdownTimerEventArgs)countdownTimer.Tag;
					args.Remaining--;
					OnCardStateChanged(args);
					if (args.TimerFinished)
						StopCountdownTimer();
				}
				else
					StopCountdownTimer();
			}
		}

		/// <summary>
		/// Stops the countdown timer.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		private void StopCountdownTimer()
		{
			if (countdownTimer != null)
			{
				countdownTimer.Stop();
				countdownTimer.Dispose();
			}
		}

		/// <summary>
		/// Updates score, stack and promotes or demotes the card.
		/// </summary>
		/// <param name="promote">if set to <c>true</c> [promote].</param>
		/// <param name="result">The result.</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		private void PromoteDemoteCard(ref bool promote, AnswerResult result, string answer, int correctSynonyms, bool overrideSynonyms, bool overrideTyping)
		{
			if (currentCardID >= 0)
			{
				int old_box, new_box;       //Important for the DB-LogEntry
				bool canceledDemote = overrideSynonyms;
				new_box = old_box = user.Dictionary.Cards.GetCardByID(currentCardID).BaseCard.Box;

				if (user.Dictionary.LearningBox == 0) //grade card
				{
					//show confirm demote dialog
					if (!promote && user.Dictionary.Settings.ConfirmDemote.GetValueOrDefault())
					{
						UserGradingDialogEventArgs args = new UserGradingDialogEventArgs(user.Dictionary, currentCardID, UserGradingDialogEventArgs.GradingDialogKind.ConfirmDemote);
						OnUserDialog(args);
						promote = args.promote;

						if (promote)
							canceledDemote = true;
					}

					if (overrideTyping)
						canceledDemote = true;

					//promote/demote card
					if (promote)
					{
						user.Dictionary.PromoteCard(currentCardID);
						new_box = user.Dictionary.Cards.GetCardByID(currentCardID).BaseCard.Box;
					}
					else
					{
						user.Dictionary.DemoteCard(currentCardID);
						new_box = user.Dictionary.Cards.GetCardByID(currentCardID).BaseCard.Box;
					}

					if (user.Dictionary.Score > user.Dictionary.Highscore)
						user.Dictionary.Highscore = user.Dictionary.Score;
				}
				else
					user.Dictionary.CardUsed(currentCardID);    //only set the card used timestamp

				//add card to stack (important to be the last step since it triggers the cardstack_changed event)
				cardStack.Push(new StackCard(
					user.Dictionary.Cards.GetCardByID(currentCardID).BaseCard,
					result,
					promote,
					user.Dictionary.CurrentQueryDirection,
					user.Dictionary.LearnMode,
					currentCardAsked,
					currentCardAnswered,
					user.Dictionary.Cards.GetCardByID(currentCardID).BaseCard.Question.Culture,
					user.Dictionary.Cards.GetCardByID(currentCardID).BaseCard.Answer.Culture,
					answer, old_box, new_box, canceledDemote, user.Dictionary, correctSynonyms));
			}
		}

		/// <summary>
		/// Grades the Text answer.
		/// </summary>
		/// <param name="args">The <see cref="MLifter.BusinessLayer.UserInputSubmitTextEventArgs"/> instance containing the event data.</param>
		/// <param name="result">The result (optional).</param>
		/// <returns>Promote or Demote.</returns>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		private bool GradeTextAnswer(UserInputSubmitTextEventArgs args, out AnswerResult result, out bool overrideSynonyms, out bool overrideTyping)
		{
			bool correct = args.correctsynonyms > 0;
			result = correct ? AnswerResult.Correct : AnswerResult.Wrong;

			overrideSynonyms = overrideTyping = false;

			//[ML-1331] Textbox counts synonyms somethimes wrong: following lines were wrong:
			//  else if (user.Dictionary.Settings.GradeSynonyms.FirstKnown.Value)  promotesynonyms = args.correctsynonyms >= 1;
			//  else if (user.Dictionary.Settings.GradeSynonyms.OneKnown.Value)   promotesynonyms = args.correctfirstsynonym;
			// ... were wrong

			// Grade Synonyms
			bool promotesynonyms = true;
			if (args.synonyms > 1 && args.correctsynonyms < args.synonyms)
			{
				if (user.Dictionary.Settings.GradeSynonyms.AllKnown.Value)
					promotesynonyms = args.synonyms == args.correctsynonyms;
				else if (user.Dictionary.Settings.GradeSynonyms.HalfKnown.Value)
					promotesynonyms = args.synonyms > 0 ? 2 * args.correctsynonyms >= args.synonyms : false;
				else if (user.Dictionary.Settings.GradeSynonyms.FirstKnown.Value)
					promotesynonyms = args.correctfirstsynonym;
				else if (user.Dictionary.Settings.GradeSynonyms.OneKnown.Value)
					promotesynonyms = args.correctsynonyms >= 1;
				else if (user.Dictionary.Settings.GradeSynonyms.Prompt.Value)
				{
					UserGradingDialogEventArgs e = new UserGradingDialogEventArgs(user.Dictionary, currentCardID, UserGradingDialogEventArgs.GradingDialogKind.GradeSynonym);
					OnUserDialog(e);
					overrideSynonyms = promotesynonyms = e.promote;
				}
			}

			// Grade Typing
			bool promotetyping = true;
			if (args.typingerrors > 0 && !overrideSynonyms)
			{
				if (user.Dictionary.Settings.GradeTyping.AllCorrect.Value)
					promotetyping = false;
				else if (user.Dictionary.Settings.GradeTyping.HalfCorrect.Value)
					promotetyping = args.answer.Length > 0 ? 2 * args.typingerrors < args.answer.Length : false;
				else if (user.Dictionary.Settings.GradeTyping.NoneCorrect.Value)
					promotetyping = true;
				else if (user.Dictionary.Settings.GradeTyping.Prompt.Value)
				{
					UserGradingDialogEventArgs e = new UserGradingDialogEventArgs(user.Dictionary, currentCardID, UserGradingDialogEventArgs.GradingDialogKind.GradeTypo);
					OnUserDialog(e);
					overrideTyping = promotetyping = e.promote;
				}
			}

			//final decision about promoting
			bool promote = correct && promotesynonyms && promotetyping;
			if (overrideSynonyms)
				promote = promotesynonyms;
			if (overrideTyping)
				promote = promotetyping;

			if (correct && !promote)
				result = AnswerResult.Almost;

			return promote;
		}
		#endregion
	}

	#region CardStateChanged EventArgs
	/// <summary>
	/// EventArgs for the CardStateChanged event.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class CardStateChangedEventArgs : EventArgs
	{
		/// <summary>
		/// The dictionary.
		/// </summary>
		public Dictionary dictionary;
		/// <summary>
		/// The current card's id.
		/// </summary>
		public int cardid;

		/// <summary>
		/// Initializes a new instance of the <see cref="CardStateChangedEventArgs"/> class.
		/// With cardstate "NewCard".
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public CardStateChangedEventArgs(Dictionary dictionary, int cardid)
		{
			this.dictionary = dictionary;
			this.cardid = cardid;
		}
	}

	/// <summary>
	/// EventArgs for the CardStateChanged event: New Card.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class CardStateChangedNewCardEventArgs : CardStateChangedEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CardStateChangedNewCardEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public CardStateChangedNewCardEventArgs(Dictionary dictionary, int cardid)
			: base(dictionary, cardid)
		{ }
	}

	/// <summary>
	/// EventArgs for the CardStateChanged event: Show Result.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class CardStateChangedShowResultEventArgs : CardStateChangedEventArgs
	{
		/// <summary>
		/// Whether the card was promoted or not.
		/// </summary>
		public bool promoted;
		/// <summary>
		/// Whether the user answer was correct or not.
		/// </summary>
		public AnswerResult result;

		/// <summary>
		/// The given answer.
		/// </summary>
		public string answer;
		/// <summary>
		/// Whether the slideshow mode was enabled or not.
		/// </summary>
		public bool slideshow;

		/// <summary>
		/// Whether the result is a preview before grading.
		/// </summary>
		public bool preview;

		/// <summary>
		/// Initializes a new instance of the <see cref="CardStateChangedShowResultEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <param name="promoted">if set to <c>true</c> [promoted].</param>
		/// <param name="result">The result.</param>
		/// <param name="answer">The answer.</param>
		/// <param name="slideshow">if set to <c>true</c> [slideshow].</param>
		/// <param name="preview">if set to <c>true</c> [preview].</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public CardStateChangedShowResultEventArgs(Dictionary dictionary, int cardid, bool promoted, AnswerResult result, string answer, bool slideshow, bool preview)
			: base(dictionary, cardid)
		{
			this.promoted = promoted;
			this.result = result;
			this.answer = answer;
			this.slideshow = slideshow;
			this.preview = preview;
		}
	}

	/// <summary>
	/// EventArgs for the CardStateChanged event: Tick of the Countdown Timer.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-30</remarks>
	public class CardStateChangedCountdownTimerEventArgs : CardStateChangedEventArgs
	{
		/// <summary>
		/// The remaining seconds.
		/// </summary>
		public int Remaining;

		/// <summary>
		/// Gets a value indicating whether [timer finished].
		/// </summary>
		/// <value><c>true</c> if [timer finished]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		public bool TimerFinished
		{
			get { return Remaining < 1; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CardStateChangedCountdownTimerEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <param name="remaining">The remaining.</param>
		/// <remarks>Documented by Dev02, 2008-04-30</remarks>
		public CardStateChangedCountdownTimerEventArgs(Dictionary dictionary, int cardid, int remaining)
			: base(dictionary, cardid)
		{
			this.Remaining = remaining;
		}
	}

	#endregion
	#region UserInputSubmit EventArgs

	/// <summary>
	/// EventArgs for the UserInputSubmit event.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class UserInputSubmitEventArgs : EventArgs
	{ }
	public class UserInputSubmitPreviousCard : UserInputSubmitEventArgs
	{ }
	/// <summary>
	/// EventArgs for the UserInputSubmit event: Text submitted.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class UserInputSubmitTextEventArgs : UserInputSubmitEventArgs
	{
		/// <summary>
		/// The amount of typing mistakes.
		/// </summary>
		public int typingerrors;
		/// <summary>
		/// The amount of correctly entered synonyms.
		/// </summary>
		public int correctsynonyms;
		/// <summary>
		/// The amount of synonyms to enter.
		/// </summary>
		public int synonyms;
		/// <summary>
		/// Wheter the first synonym was correct or not.
		/// </summary>
		public bool correctfirstsynonym;
		/// <summary>
		/// The entered answer.
		/// </summary>
		public string answer;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserInputSubmitTextEventArgs"/> class.
		/// </summary>
		/// <param name="typingerrors">The typingerrors.</param>
		/// <param name="correctsynonyms">The correctsynonyms.</param>
		/// <param name="synonyms">The synonyms.</param>
		/// <param name="correctfirstsynonym">if set to <c>true</c> [correctfirstsynonym].</param>
		/// <param name="answer">The answer.</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public UserInputSubmitTextEventArgs(int typingerrors, int correctsynonyms, int synonyms, bool correctfirstsynonym, string answer)
			: base()
		{
			this.typingerrors = typingerrors;
			this.correctsynonyms = correctsynonyms;
			this.synonyms = synonyms;
			this.correctfirstsynonym = correctfirstsynonym;
			this.answer = answer;
		}
	}

	/// <summary>
	///  EventArgs for the UserInputSubmit event: Self Assessment Response Submitted.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class UserInputSubmitSelfAssessmentResponseEventArgs : UserInputSubmitEventArgs
	{
		/// <summary>
		/// Whether the answer is known or not.
		/// </summary>
		public bool doknow;

		/// <summary>
		/// Whether the card should not be asked again.
		/// </summary>
		public bool dontaskagain;

		/// <summary>
		/// The answer
		/// </summary>
		public string answer;

		/// <summary>
		/// The original answer result (not affected by the user's decision).
		/// </summary>
		public AnswerResult result;

		public UserInputSubmitSelfAssessmentResponseEventArgs(bool doknow, bool dontaskagain, AnswerResult result, string answer)
			: base()
		{
			this.doknow = doknow;
			this.dontaskagain = dontaskagain;
			this.result = result;
			this.answer = answer;
		}
	}

	/// <summary>
	///  EventArgs for the UserInputSubmit event: Self Assessment Response Submitted.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-23</remarks>
	public class UserInputSubmitMultipleChoiceEventArgs : UserInputSubmitEventArgs
	{
		/// <summary>
		/// The user's answer choice.
		/// </summary>
		public string answer;

		/// <summary>
		/// The multiple choice result.
		/// </summary>
		public MultipleChoiceResult result;

		public UserInputSubmitMultipleChoiceEventArgs(string answer, MultipleChoiceResult result)
			: base()
		{
			this.answer = answer;
			this.result = result;
		}
	}

	/// <summary>
	/// EventArgs for the UserInputSubmit event: Slideshow Event Submitted.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-29</remarks>
	public class UserInputSubmitSlideshowEventArgs : UserInputSubmitEventArgs
	{
		/// <summary>
		/// Whether the slideshow should be stopped.
		/// </summary>
		public bool stopslideshow = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserInputSubmitSlideshowEventArgs"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public UserInputSubmitSlideshowEventArgs()
			: base()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="UserInputSubmitSlideshowEventArgs"/> class.
		/// </summary>
		/// <param name="stopslideshow">if set to <c>true</c> [stopslideshow].</param>
		/// <remarks>Documented by Dev02, 2008-04-23</remarks>
		public UserInputSubmitSlideshowEventArgs(bool stopslideshow)
			: base()
		{
			this.stopslideshow = stopslideshow;
		}
	}

	/// <summary>
	/// EventArgs for the UserInputSubmit event: Deactivate current card.
	/// </summary>
	/// <remarks>Documented by Dev02, 2009-09-09</remarks>
	public class UserInputSubmitDeactivateCardEventArgs : UserInputSubmitEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserInputSubmitDeactivateCardEventArgs"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-09-09</remarks>
		public UserInputSubmitDeactivateCardEventArgs()
			: base()
		{ }
	}
	#endregion
	#region UserDialog EventArgs
	/// <summary>
	/// EventArgs for the UserDialog event.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-29</remarks>
	public class UserDialogEventArgs : EventArgs
	{
		/// <summary>
		/// The dictionary.
		/// </summary>
		public Dictionary dictionary;

		/// <summary>
		/// The card ID.
		/// </summary>
		public int cardid;

		/// <summary>
		/// asdf
		/// </summary>
		public object OptionalParamter;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserDialogEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		public UserDialogEventArgs(Dictionary dictionary, int cardid)
		{
			this.dictionary = dictionary;
			this.cardid = cardid;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserDialogEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <param name="optionalParamter">A optional paramter.</param>
		/// <remarks>Documented by Dev08, 2009-04-20</remarks>
		public UserDialogEventArgs(Dictionary dictionary, int cardid, object optionalParamter)
			: this(dictionary, cardid)
		{
			OptionalParamter = optionalParamter;
		}
	}

	/// <summary>
	/// EventArgs for the UserDialog event: Notify Dialog.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-29</remarks>
	public class UserNotifyDialogEventArgs : UserDialogEventArgs
	{
		/// <summary>
		/// Which dialog is to display.
		/// </summary>
		public NotifyDialogKind dialogkind;

		/// <summary>
		/// The available dialogs.
		/// </summary>
		public enum NotifyDialogKind
		{
			PoolEmpty,
			NoWords,
			NotEnoughMultipleChoices,
			SelectedLearnModeNotAllowed
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserNotifyDialogEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		public UserNotifyDialogEventArgs(Dictionary dictionary, int cardid, NotifyDialogKind dialogkind)
			: base(dictionary, cardid)
		{
			this.dialogkind = dialogkind;
		}

		public UserNotifyDialogEventArgs(Dictionary dictionary, int cardid, NotifyDialogKind dialogkind, object optionalParameter)
			: base(dictionary, cardid, optionalParameter)
		{
			this.dialogkind = dialogkind;
		}
	}

	/// <summary>
	/// EventArgs for the UserDialog event: Grading Dialog.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-04-29</remarks>
	public class UserGradingDialogEventArgs : UserDialogEventArgs
	{
		/// <summary>
		/// Whether the card should be promoted.
		/// </summary>
		public bool promote;

		/// <summary>
		/// Which dialog is to display.
		/// </summary>
		public GradingDialogKind dialogkind;

		/// <summary>
		/// The available dialogs.
		/// </summary>
		public enum GradingDialogKind
		{
			ConfirmDemote,
			GradeSynonym,
			GradeTypo
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserGradingDialogEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <param name="dialog">The dialog.</param>
		/// <remarks>Documented by Dev02, 2008-04-29</remarks>
		public UserGradingDialogEventArgs(Dictionary dictionary, int cardid, GradingDialogKind dialogkind)
			: base(dictionary, cardid)
		{
			this.dialogkind = dialogkind;
		}
	}

	/// <summary>
	/// EventArgs for the UserDialog event: Module not saved.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-05-13</remarks>
	public class UserModuleNotSavedDialogEventArgs : UserDialogEventArgs
	{
		/// <summary>
		/// Whether to try again to save or not.
		/// </summary>
		public bool tryagain = false; //default value false to avoid endless loops, if no eventhandler is registered to the event

		/// <summary>
		/// Whether to cancel the closing/saving process.
		/// </summary>
		public bool cancelClosing = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserModuleNotSavedDialogEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <remarks>Documented by Dev02, 2008-05-13</remarks>
		public UserModuleNotSavedDialogEventArgs(Dictionary dictionary, int cardid)
			: base(dictionary, cardid)
		{ }
	}

	/// <summary>
	/// EventArgs for the UserDialog event: Backup completed.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-09-08</remarks>
	public class BackupCompletedNotifyDialogEventArgs : UserDialogEventArgs
	{
		/// <summary>
		/// The used filename for the backup.
		/// </summary>
		public string backupFilename = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="BackupCompletedNotifyDialogEventArgs"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="cardid">The cardid.</param>
		/// <param name="backupFilename">The backup filename.</param>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		public BackupCompletedNotifyDialogEventArgs(Dictionary dictionary, int cardid, string backupFilename)
			: base(dictionary, cardid)
		{
			this.backupFilename = backupFilename;
		}
	}
	#endregion
	#region LearningModule Opening/Closing EventArgs
	/// <summary>
	/// EventArgs for the LearningModuleClosing event.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-05-06</remarks>
	public class LearningModuleClosingEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModuleClosingEventArgs"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public LearningModuleClosingEventArgs()
		{ }

		/// <summary>
		/// Whether to cancel the current closing process.
		/// </summary>
		public bool Cancel = false;
	}

	/// <summary>
	/// EventArgs for the LearningModuleOpened event.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-05-06</remarks>
	public class LearningModuleOpenedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModuleOpenedEventArgs"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-05-06</remarks>
		public LearningModuleOpenedEventArgs()
		{ }

		/// <summary>
		/// Whether the LM does not contain any active cards.
		/// </summary>
		public bool NoCards = false;

		/// <summary>
		/// Whether the LM does not contain any chapters.
		/// </summary>
		public bool NoChapters = false;
	}
	#endregion
}
