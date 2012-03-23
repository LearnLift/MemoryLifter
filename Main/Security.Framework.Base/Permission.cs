/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
