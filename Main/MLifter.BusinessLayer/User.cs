using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;
using System.Diagnostics;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Implements a BL user.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-28</remarks>
    public class User
    {
        /// <summary>
        /// If true, do not auto login.
        /// </summary>
        public static bool PreventAutoLogin = false;

        /// <summary>
        /// Gets or sets the current learn logic.
        /// </summary>
        /// <value>The current learn logic.</value>
        /// <remarks>Documented by Dev09, 2009-04-10</remarks>
        public LearnLogic CurrentLearnLogic { get; private set; }
        Dictionary dictionary = null;

        IUser user = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="learnLogic">The learn logic.</param>
        /// <remarks>Documented by Dev09, 2009-04-10</remarks>
        public User(LearnLogic learnLogic)
        {
            CurrentLearnLogic = learnLogic;
        }

        public Guid SessionId { get { return (user as MLifter.DAL.User).SessionId; } }

        /// <summary>
        /// Gets the current DL user.
        /// </summary>
        /// <value>The DL user.</value>
        /// <remarks>Documented by Dev03, 2008-08-28</remarks>
        internal IUser BaseUser
        {
            get { return user; }
        }

        /// <summary>
        /// Gets and set the current dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev03, 2008-08-28</remarks>
        public Dictionary Dictionary
        {
            get { return dictionary; }
            internal set { dictionary = value; }
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="loginCallback">The login callback.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-28</remarks>
        public bool Authenticate(GetLoginInformation loginCallback, ConnectionStringStruct connection, DataAccessErrorDelegate errorMessageDelegate)
        {
            user = UserFactory.Create(loginCallback, connection, errorMessageDelegate, this);
            return true;
        }

        /// <summary>
        /// Sets the base user to an already existing user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks>Documented by Dev02, 2008-09-24</remarks>
        public void SetBaseUser(IUser user)
        {
            this.user = user;
            user.Logout();  // force logout for the new base user
            user.Login();
        }
        /// <summary>
        /// Sets the base user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="getLoginDelegate">The login delegate.</param>
        /// <param name="errorMessageDelegate">The error message delegate.</param>
        /// <remarks>Documented by Dev05, 2008-12-12</remarks>
        public void SetBaseUser(IUser user, GetLoginInformation getLoginDelegate, DataAccessErrorDelegate errorMessageDelegate)
        {
            SetBaseUser(user);
            user.GetLoginDelegate = getLoginDelegate;
            user.ErrorMessageDelegate = errorMessageDelegate;
        }
        /// <summary>
        /// Gets the base user.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-04-29</remarks>
        public IUser GetBaseUser() { return user; }

        /// <summary>
        /// Gets the learning module list.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-28</remarks>
        public IDictionaries GetLearningModuleList()
        {
            if (user == null) return null;
            return user.List();
        }

        /// <summary>
        /// Opens the learning module.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-28</remarks>
        internal bool OpenLearningModule()
        {
            if (user == null) return false;
            try
            {
                dictionary = new Dictionary(user.Open(), CurrentLearnLogic);
            }
            catch (DatabaseVersionNotSupported) { throw; }
            catch (DictionaryFormatNotSupported) { throw; }
            catch (DictionaryFormatOldVersion) { throw; }
            catch (DictionaryNotDecryptedException) { throw; }
            catch (System.Xml.XmlException) { throw new InvalidDictionaryException(); }
            catch (System.IO.IOException) { throw; }
            catch (UserSessionInvalidException) { throw; }
            catch (NotEnoughtDiskSpaceException) { throw; }
            catch (Exception exp)
            {
                Trace.WriteLine(exp.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Upgrades the database to the current version (only for MS SQL CE).
        /// </summary>
        /// <returns>[true] if success.</returns>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        public bool UpgradeDatabase(Version currentVersion)
        {
            return user.Database.UpgradeDatabase(currentVersion);
        }

        /// <summary>
        /// Starts the learning session.
        /// Against: [ML-1378] - Import of ODX causes two LearningSessions
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        internal void StartLearningSession()
        {
            dictionary.StartUserSession();
        }

        /// <summary>
        /// Creates the learning module.
        /// </summary>
        /// <param name="catergoryId">The catergory id.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-12-22</remarks>
        public IDictionary CreateLearningModule(int catergoryId, string title)
        {
            IDictionaries dictionaries = GetLearningModuleList();
            if (dictionaries == null)
                return null;
            else
                return dictionaries.AddNew(catergoryId, title);
        }

        /// <summary>
        /// Closes the actual user-session.
        /// </summary>
        /// <remarks>Documented by Dev05, 2008-11-14</remarks>
        public void Logout()
        {
            if (BaseUser != null)
                BaseUser.Logout();
        }

        public override string ToString()
        {
            if (this.BaseUser != null)
                return this.BaseUser.UserName;
            else
                return String.Empty;
        }
    }
}
