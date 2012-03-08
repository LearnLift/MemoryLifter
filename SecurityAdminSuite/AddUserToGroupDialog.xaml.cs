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
