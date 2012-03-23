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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SecurityFramework;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SecurityAdminSuite
{
    /// <summary>
    /// Interaction logic for AddUserToGroupDialog.xaml
    /// </summary>
    public partial class AddUserToGroupDialog : Window, INotifyPropertyChanged
    {

        public Facade Facade { get; set; }
        public List<UserInfo> SelectedUserInfos { get; set; }
        public AddUserToGroupDialog()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(AddUserToGroupDialog_Loaded);
        }

        private string _UsernameFilter;
        public string UsernameFilter
        {
            get
            {
                return _UsernameFilter;
            }
            set
            {
                if (_UsernameFilter == value) return;
                _UsernameFilter = value;

                NotifyPropertyChanged("UsernameFilter");
                NotifyPropertyChanged("Users");
            }
        }
        public ObservableCollection<UserInfo> Users
        {
            get
            {
                ObservableCollection<UserInfo> users = new ObservableCollection<UserInfo>();

                ObservableCollection<UserInfo> us;
                if (string.IsNullOrEmpty(UsernameFilter))
                {
                    var x = from ui in Facade.Users
                            from gr in ui.Groups
                            where gr.Group.Name == Facade.ActualGroup.Name && gr.IsMember == false

                            select ui;

                    us = new ObservableCollection<UserInfo>(x);
                }
                else
                {
                    var x = from ui in Facade.Users
                            from gr in ui.Groups
                            where gr.Group.Name == Facade.ActualGroup.Name && gr.IsMember == false
                            where ui.User.Account.ToLower().Contains(this.UsernameFilter.ToLower())
                            select ui;

                    us = new ObservableCollection<UserInfo>(x);
                }

                return us;
            }
        }

        void AddUserToGroupDialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedUserInfos = new List<UserInfo>();

            foreach (object o in this.lbUsers.SelectedItems)
            {
                if (o is UserInfo)
                    SelectedUserInfos.Add(o as UserInfo);
            }
            DialogResult = true;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
