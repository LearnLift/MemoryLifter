using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SecurityFramework
{

    public class TypeInfo : INotifyPropertyChanged
    {
        public TypeInfo()
        {
            Permissions = new List<PermissionInfo>();
        }
        public TypeInfo(string name)
            : this()
        {
            Name = name;
        }


        public string Name { get; set; }

        public List<PermissionInfo> Permissions { get; set; }

     
        public TypeInfo HierachicalParent { get; set; }

       

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
