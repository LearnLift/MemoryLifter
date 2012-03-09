using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SecurityFramework
{
    public class Group :Role, INotifyPropertyChanged
    {

        public Group(string id, string name)
        {
            Id = id;
            _Name = name;
        }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                    throw new Exception("Groupname must not be empty");
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public override string ToString()
        {
            return this.Name;
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
}
