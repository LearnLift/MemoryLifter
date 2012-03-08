using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SecurityFramework
{

    public class User : Role, INotifyPropertyChanged
    {

        public User(string account)
        {

            _Account = account;
            Groups = new List<Group>();
        }


        private string _Account;
        public string Account
        {
            get
            {
                return _Account;
            }
            set
            {
                try { Name = value; }
                catch { NotifyPropertyChanged("Account"); }
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Account = value;
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Username must not be empty");
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                NotifyPropertyChanged("Password");
            }
        }

        private bool _IsActivated = true;
        public bool IsActivated
        {
            get
            {
                return _IsActivated;
            }
            set
            {
                _IsActivated = value;
                NotifyPropertyChanged("IsActivated");
            }
        }

        public List<Group> Groups { get; set; }
        public void AddToGroup(Group group)
        {
            if (Groups.Contains(group)) return;
            Groups.Add(group);
        }
        public void RemoveFromGroup(Group group)
        {
            Groups.Remove(group);
        }

        private UserAuthType _Type;
        public UserAuthType Type
        {
            get { return _Type; }
            set {
                _Type = value;
                NotifyPropertyChanged("Type");
            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
    /// <summary>
    /// The authentication typ of the user.
    /// </summary>
    [Flags]
    public enum UserAuthType
    {
        /// <summary>
        /// The user is only authenticated by it's username.
        /// </summary>
        ListAuthentication = 1,
        /// <summary>
        /// The user is authenticated by a username / password combination.
        /// </summary>
        FormsAuthentication = 2,
        /// <summary>
        /// The user is authenticated by his local directory user profile.
        /// </summary>
        LocalDirectoryAuthentication = 4
    }
}
