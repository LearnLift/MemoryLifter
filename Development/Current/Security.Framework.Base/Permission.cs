using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SecurityFramework
{
  
    public class TypePermissionEntry
    {
        public string RoleId { get; set; }
        public string TypeName { get; set; }
        public PermissionInfo Permission { get; set; }
        
    }

    public class ObjectPermissionEntry
    {
        public string RoleId { get; set; }
        public string ObjectLocator { get; set; }
        public string TypeName { get; set; }
        public PermissionInfo Permission { get; set; }
    }

    public class PermissionInfo:INotifyPropertyChanged
    {
        public string PermissionName { get; set; }

        private bool _Access;
        public bool Access
        {
            get
            {
                return _Access;
            }
            set
            {
                _Access = value;
                NotifyPropertyChanged("Right");
            }
        }
        public PermissionInfo(string name, bool access)
        {
            PermissionName = name;
            Access = access;
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}",this.PermissionName,this.Access.ToString());
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
