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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Composite.Wpf.Commands;
using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Security;
using SecurityAdminSuite.Properties;
using SecurityFramework;
using User = SecurityFramework.User;
using System.Windows.Data;
using System.Diagnostics;
using MLifter.Controls;

namespace SecurityAdminSuite
{

    public class Facade : INotifyPropertyChanged
    {
        #region Attached Property für die Facade
        //Damit diese Attached Property auch PropertyInheritance über InheritanceBoundaries wie z.B. Frames durchführt,
        //müssen die FrameworkPropertyMetadata mit der Option
        //FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior
        //versehen werden!!!!!!
        public static object GetItem(DependencyObject obj)
        {
            return (object)obj.GetValue(ItemProperty);
        }

        public static void SetItem(DependencyObject obj, object value)
        {
            obj.SetValue(ItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.RegisterAttached("Item", typeof(object), typeof(Facade), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));

        #endregion

        public Window MainWindow { get; set; }

        public Facade()
        {
            AppSettings settings = new AppSettings();
            string globalConfig = Path.Combine(Application.Current.StartupUri.AbsolutePath, settings.ConfigPath);
            string userConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.ConfigurationFolder);

            /**** Omicron ***********************************************************************/
            LearningModules = new LearningModulesIndex(globalConfig, userConfig, GetUser, DataAccessError, String.Empty);
            List<IConnectionString> connectionStrings = LearningModulesIndex.ConnectionsHandler.ConnectionStrings.FindAll(c => c.ConnectionType == MLifter.DAL.DatabaseType.PostgreSQL);
            try
            {
                if (connectionStrings.Count > 0)
                {
                    try
                    {
                        ConnectionString = connectionStrings[0];
                        FolderIndexEntry entry = LearningModules.GetFolderOfConnection(ConnectionString);
                        CurrentUser = LearningModulesIndex.ConnectionUsers[ConnectionString];
                        if (!CurrentUser.HasPermission(CurrentUser.List(), PermissionTypes.IsAdmin))
                            throw new PermissionException();

                        SecurityFramework = MLifter.DAL.Security.SecurityFramework.GetDataAdapter(ConnectionString.ConnectionString);
                    }
                    catch (PermissionException)
                    {
                        CurrentUser.Logout();
                        MessageBox.Show(Resources.CONNECTION_ERROR_PERMISSION_TEXT, Resources.CONNECTION_ERROR_PERMISSION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                        throw;
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(Resources.CONNECTION_ERROR_FAILURE_TEXT, Resources.CONNECTION_ERROR_FAILURE_CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                        TaskDialog.MessageBox(Resources.CONNECTION_ERROR_FAILURE_CAPTION, Resources.CONNECTION_ERROR_FAILURE_TEXT, string.Empty, e.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
                        throw;
                    }
                }
                else
                {
                    TaskDialog.MessageBox(Resources.CONNECTION_ERROR_NO_CONNECTION_AVAILABLE_CAPTION, Resources.CONNECTION_ERROR_NO_CONNECTION_AVAILABLE_TEXT,
                        String.Format(Resources.CONNECTION_ERROR_NO_CONNECTION_AVAILABLE_DETAILS, Settings.Default.ConfigurationFolder), TaskDialogButtons.OK, TaskDialogIcons.Error);
                    throw new Exception();
                }
            }
            catch
            {
                Environment.Exit(-1);
            }
            /**** Omicron ***********************************************************************/


            AddNewGroupCommand = new DelegateCommand<object>(ExecuteAddNewGroupCommand, CanAddNewGroupCommand);
            RemoveGroupCommand = new DelegateCommand<Group>(ExecuteRemoveGroupCommand, CanRemoveGroupCommand);
            AddNewUserCommand = new DelegateCommand<object>(ExecuteAddNewUserCommand, CanAddNewUserCommand);
            RemoveUserCommand = new DelegateCommand<UserInfo>(ExecuteRemoveUserCommand, CanRemoveUserCommand);
            ResetTypePermissionCommand = new DelegateCommand<TypeInformation>(ExecuteResetTypePermissionCommand, CanResetTypePermissionCommand);
            ResetObjectPermissionCommand = new DelegateCommand<ObjectInformation>(ExecuteResetObjectPermissionCommand, CanResetObjectPermissionCommand);
            RenameGroupCommand = new DelegateCommand<Group>(ExecuteRenameGroupCommand, CanRenameGroupCommand);
            RenameUserCommand = new DelegateCommand<UserInfo>(ExecuteRenameUserCommand, CanRenameUserCommand);
            ChangeUserPasswordCommand = new DelegateCommand<UserInfo>(ExecuteChangeUserPasswordCommand, CanChangeUserPasswordCommand);
            ImportNewUserCommand = new DelegateCommand<object>(ExecuteImportNewUserCommand, CanImportNewUserCommand);
            RemoveGroupFromUserCommand = new DelegateCommand<GroupMembershipInfo>(ExecuteRemoveGroupFromUserCommand, CanRemoveGroupFromUserCommand);
            RemoveUserFromActualGroupCommand = new DelegateCommand<UserInfo>(ExecuteRemoveUserFromActualGroupCommand, CanRemoveUserFromActualGroupCommand);
            AddUserToActualGroupCommand = new DelegateCommand<object>(ExecuteAddUserToActualGroupCommand, CanAddUserToActualGroupCommand);
        }

        public void Save()
        {
            if (CurrentUser != null)
                CurrentUser.Logout();
        }

        public Framework SecurityFramework { get; set; }
        public IConnectionString ConnectionString { get; set; }
        private IUser CurrentUser { get; set; }
        private LearningModulesIndex LearningModules { get; set; }

        private static Dictionary<string, UserStruct> authenticationUsers = new Dictionary<string, UserStruct>();
        public static UserStruct? GetUser(UserStruct user, ConnectionStringStruct connection)
        {
            if (authenticationUsers.ContainsKey(connection.ConnectionString) && user.LastLoginError == LoginError.NoError)
            {
                return authenticationUsers[connection.ConnectionString];
            }
            else
            {
                UserStruct? newUser = MLifter.Controls.LoginForm.OpenLoginForm(user, connection);
                if (newUser.HasValue)
                {
                    authenticationUsers[connection.ConnectionString] = newUser.Value;
                }
                return newUser;
            }
        }

        ObservableCollection<Group> _Groups;
        public ObservableCollection<Group> Groups
        {
            get
            {
                if (_Groups == null)
                {
                    List<Group> gs = this.SecurityFramework.Groups;
                    UpdateGroupFilter();

                    _Groups = new ObservableCollection<Group>(gs);
                    foreach (Group group in _Groups)
                    {
                        group.PropertyChanged += group_PropertyChanged;
                    }
                }

                return _Groups;
            }
        }

        public ObservableCollection<GroupInfo> GroupInfos
        {
            get
            {
                ObservableCollection<GroupInfo> list = new ObservableCollection<GroupInfo>();
                foreach (Group group in Groups)
                {
                    list.Add(new GroupInfo(group, this.SecurityFramework));
                }

                return list;
            }
        }

        void group_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SecurityFramework.UpdateGroup(sender as Group);
            NotifyPropertyChanged("GroupInfos");
        }
        private void AddGroup(string name)
        {
            try
            {
                Group g = SecurityFramework.AddGroup(name);

                _Groups.Add(g);
                _Users.ToList().ForEach(u => u.AddGroupToList(g));
                UpdateGroupFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemoveGroup(Group g)
        {
            try
            {
                SecurityFramework.RemoveGroup(g);

                if (_Groups.Contains(g))
                    _Groups.Remove(g);
                _Users.ToList().ForEach(u => u.RemoveGroupFromList(g));
                UpdateGroupFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                UpdateUserFilter();
            }
        }

        private string _GroupnameFilter;
        public string GroupnameFilter
        {
            get
            {
                return _GroupnameFilter;
            }
            set
            {
                if (_GroupnameFilter == value) return;
                _GroupnameFilter = value;
                UpdateGroupFilter();
            }
        }

        public void UpdateGroupFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_Groups);
            if (view != null)
                view.Filter = string.IsNullOrEmpty(_GroupnameFilter) ? null : new Predicate<object>(o => (o as Group).Name.ToLower().Contains(_GroupnameFilter.ToLower()));
        }
        public void UpdateUserFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_Users);
            if (view != null)
                view.Filter = string.IsNullOrEmpty(_UsernameFilter) ? null : new Predicate<object>(o => (o as UserInfo).User.Account.ToLower().Contains(_UsernameFilter.ToLower()));
        }

        ObservableCollection<UserInfo> _Users;
        public ObservableCollection<UserInfo> Users
        {
            get
            {
                if (_Users == null)
                {
                    List<User> us = this.SecurityFramework.Users;
                    UpdateUserFilter();

                    _Users = new ObservableCollection<UserInfo>(us.ConvertAll<UserInfo>(new Converter<User, UserInfo>(delegate(User u) { return new UserInfo(u, SecurityFramework.Groups); })));
                    foreach (UserInfo user in _Users)
                    {
                        user.User.PropertyChanged += User_PropertyChanged;
                        user.PropertyChanged += ui_PropertyChanged;
                    }
                }

                return _Users;
            }
        }

        void ui_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SecurityFramework.UpdateUser((sender as UserInfo).User);
        }

        public void AddUser(string name, string password, UserAuthType type)
        {
            try
            {
                name = name.ToLower(); //only lower case usernames are allowed

                User user = SecurityFramework.AddNewUser(name, password, type);
                if (_Users != null)
                {
                    if (string.IsNullOrEmpty(UsernameFilter))
                    {
                        UserInfo u = new UserInfo(user, SecurityFramework.Groups);
                        if (_Users.Count(ui => ui.User.Account == u.User.Account) > 0)
                        {
                            MessageBox.Show(String.Format("The given username '{0}' already exists.", u.User.Account),
                                "Execution was aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            _Users.Add(new UserInfo(user, SecurityFramework.Groups));
                        }
                    }
                    else if (user.Account.Contains(UsernameFilter))
                    {
                        _Users.Add(new UserInfo(user, SecurityFramework.Groups));
                        (MainWindow as MainWindow).ListBoxUsers.GetBindingExpression(System.Windows.Controls.ListBox.ItemsSourceProperty).UpdateTarget();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemoveUser(User user)
        {
            try
            {
                SecurityFramework.RemoveUser(user);
                if (_Users.ToList().Exists(ui => ui.User == user))
                {
                    _Users.Remove(_Users.ToList().Find(ui => ui.User == user));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private Group _ActualGroup;
        public Group ActualGroup
        {
            get
            {
                return _ActualGroup;
            }
            set
            {
                _ActualGroup = value;
                if (_ActualGroup != null)
                    ActualUser = null;
                NotifyPropertyChanged("ActualGroup");
                NotifyPropertyChanged("Types");
                NotifyPropertyChanged("ActualObjectPermissionInformation");
                NotifyPropertyChanged("ActualTypePermissionInformation");
            }
        }

        private UserInfo _ActualUser;
        public UserInfo ActualUser
        {
            get
            {
                return _ActualUser;
            }
            set
            {

                _ActualUser = value;
                if (_ActualUser != null)
                    ActualGroup = null;

                NotifyPropertyChanged("ActualUser");
                NotifyPropertyChanged("Types");
                NotifyPropertyChanged("ActualObjectPermissionInformation");
                NotifyPropertyChanged("ActualTypePermissionInformation");
            }
        }

        void User_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SecurityFramework.UpdateUser(sender as User);
            NotifyPropertyChanged("Users");
        }

        private object _ActualObject;
        public object ActualObject
        {
            get
            {
                return _ActualObject;
            }
            set
            {
                _ActualObject = value;
                NotifyPropertyChanged("ActualObjectPermissionInformation");
            }
        }

        private TypeInfo _ActualType;
        public TypeInfo ActualType
        {
            get
            {
                return _ActualType;
            }
            set
            {
                _ActualType = value;
                NotifyPropertyChanged("ActualTypePermissionInformation");
            }
        }

        public ObjectInformation ActualObjectPermissionInformation
        {
            get
            {
                if (ActualObject == null) return null;
                if (ActualRole == null) return null;
                return new ObjectInformation(SecurityFramework, ActualRole, ActualObject);
            }
        }

        public TypeInformation ActualTypePermissionInformation
        {
            get
            {
                if (ActualRole == null) return null;
                if (ActualType == null) return null;
                return new TypeInformation(SecurityFramework, ActualRole, ActualType);

            }
        }

        public Role ActualRole
        {
            get
            {
                if (ActualGroup != null) return ActualGroup;
                if (ActualUser != null) return ActualUser.User;
                return null;

            }
        }


        public ObservableCollection<TypeInformation> Types
        {
            get
            {
                ObservableCollection<TypeInformation> types = new ObservableCollection<TypeInformation>();
                if (ActualRole == null) return types;

                foreach (TypeInfo ti in SecurityFramework.Types)
                {
                    types.Add(new TypeInformation(SecurityFramework, ActualRole, ti));
                }
                return types;
            }
        }

        public List<DicInfo> Dictionaries
        {
            get
            {
                List<DicInfo> dicList = new List<DicInfo>();

                foreach (IDictionary dictionary in CurrentUser.List().Dictionaries)
                {
                    DicInfo di = new DicInfo();
                    di.Tag = dictionary;
                    di.Name = dictionary.Title;
                    di.Locator = SecurityFramework.GetLocator(dictionary);
                    di.Chapters = new List<ChapterInfo>();

                    foreach (IChapter chapter in dictionary.Chapters.Chapters)
                    {
                        ChapterInfo ci = new ChapterInfo();
                        ci.Tag = chapter;
                        ci.Name = chapter.Title;
                        ci.Locator = SecurityFramework.GetLocator(chapter);
                        ci.Cards = new List<CardInfo>();
                        di.Chapters.Add(ci);
                        //foreach (ICard card in dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(chapter.Id, -1) }, QueryOrder.Id, QueryOrderDir.Ascending, 0))
                        //{
                        //    CardInfo cai = new CardInfo();
                        //    cai.Tag = card;
                        //    cai.Name = card.ToString();
                        //    cai.Locator = SecurityFramework.GetLocator(card);
                        //    ci.Cards.Add(cai);
                        //}
                    }
                    dicList.Add(di);
                }
                return dicList;
            }
        }

        /// <summary>
        /// Prompts the exception thrown by the Import function.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exp">The exp.</param>
        /// <remarks>Documented by Dev10, 2009-18-02</remarks>
        private void DataAccessError(object sender, Exception exp)
        {
            return;
        }

        private void RaiseCanExecuteChanged()
        {
            AddNewGroupCommand.RaiseCanExecuteChanged();
            RemoveGroupCommand.RaiseCanExecuteChanged();
            AddNewUserCommand.RaiseCanExecuteChanged();
            RemoveUserCommand.RaiseCanExecuteChanged();
            RenameGroupCommand.RaiseCanExecuteChanged();
            ResetTypePermissionCommand.RaiseCanExecuteChanged();
            RemoveGroupFromUserCommand.RaiseCanExecuteChanged();
        }


        public DelegateCommand<GroupMembershipInfo> RemoveGroupFromUserCommand { get; set; }
        private void ExecuteRemoveGroupFromUserCommand(GroupMembershipInfo obj)
        {
            try
            {
                if (obj == null) return;
                if (ActualUser == null) return;
                if (MessageBox.Show("Do you really want to remove the group membership?", "Remove Group Membership", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    obj.IsMember = false;
                    SecurityFramework.UpdateUser(ActualUser.User);

                    NotifyPropertyChanged("ActualTypePermissionInformation");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanRemoveGroupFromUserCommand(GroupMembershipInfo obj)
        {
            return true;
        }


        public DelegateCommand<Group> RenameGroupCommand { get; set; }
        private void ExecuteRenameGroupCommand(Group obj)
        {
            try
            {
                EditStringWindow dlg = new EditStringWindow();
                dlg.Title = "Rename Group";
                dlg.Header = "Enter new group name for: " + obj.Name;
                dlg.TextItem = obj.Name;
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (dlg.ShowDialog() == true)
                {
                    obj.Name = dlg.TextItem;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanRenameGroupCommand(Group obj)
        {
            return true;
        }

        public DelegateCommand<object> AddNewGroupCommand { get; set; }
        private void ExecuteAddNewGroupCommand(object obj)
        {
            try
            {
                EditStringWindow dlg = new EditStringWindow();
                dlg.Title = "New Group";
                dlg.Header = "Enter name of new group";
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.Owner = Application.Current.MainWindow;
                if (dlg.ShowDialog() == true)
                {
                    AddGroup(dlg.TextItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanAddNewGroupCommand(object obj)
        {
            return true;
        }


        public DelegateCommand<Group> RemoveGroupCommand { get; set; }
        private void ExecuteRemoveGroupCommand(Group obj)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete this group?", "Delete Group", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    RemoveGroup(obj);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanRemoveGroupCommand(Group obj)
        {
            return true;
        }

        public DelegateCommand<UserInfo> RenameUserCommand { get; set; }
        private void ExecuteRenameUserCommand(UserInfo obj)
        {
            try
            {
                EditStringWindow dlg = new EditStringWindow();
                dlg.Title = "Rename User";
                dlg.Header = "Enter new user name";
                dlg.TextItem = obj.User.Account;
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (dlg.ShowDialog() == true)
                {
                    obj.User.Name = dlg.TextItem;
                    obj.User.Account = dlg.TextItem;

                    (MainWindow as MainWindow).lbUsers.Items.SortDescriptions.Clear();
                    (MainWindow as MainWindow).lbUsers.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("User.Account", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanRenameUserCommand(UserInfo obj)
        {
            return true;
        }

        public DelegateCommand<UserInfo> ChangeUserPasswordCommand { get; set; }
        private void ExecuteChangeUserPasswordCommand(UserInfo obj)
        {
            try
            {
                UserAuth dlg = new UserAuth(obj.User);
                dlg.Title = "Change User Authentification";
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (dlg.ShowDialog() == true)
                {
                    if (obj.User.Type == UserAuthType.FormsAuthentication)
                        obj.User.Password = dlg.tbString.Text;
                    else
                        obj.User.Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanChangeUserPasswordCommand(UserInfo obj)
        {
            return true;
        }

        public DelegateCommand<object> AddNewUserCommand { get; set; }
        private void ExecuteAddNewUserCommand(object obj)
        {
            try
            {
                EditUserWindow dlg = new EditUserWindow();
                dlg.Header = "Enter name of new user:";
                dlg.Title = "New User";
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.Owner = Application.Current.MainWindow;
                if (dlg.ShowDialog() == true)
                {
                    EditUserWindow dlgPW = new EditUserWindow();
                    dlgPW.Header = "Enter the password of new user (press 'Cancel' to create a List-User):";
                    dlgPW.Title = "Password";
                    dlgPW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    dlgPW.Owner = Application.Current.MainWindow;
                    dlgPW.ShowDialog();

                    AddUser(dlg.TextItem, dlgPW.TextItem, string.IsNullOrEmpty(dlgPW.TextItem) ? UserAuthType.ListAuthentication : UserAuthType.FormsAuthentication);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanAddNewUserCommand(object obj)
        {
            return true;
        }


        public DelegateCommand<object> ImportNewUserCommand { get; set; }
        private void ExecuteImportNewUserCommand(object obj)
        {
            try
            {
                ImportUserWindow importWindow = new ImportUserWindow(this);
                importWindow.Owner = MainWindow;
                importWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanImportNewUserCommand(object obj)
        {
            return true;
        }

        public DelegateCommand<UserInfo> RemoveUserCommand { get; set; }
        private void ExecuteRemoveUserCommand(UserInfo obj)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete the user '" + obj + "'?", "Delete User", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    RemoveUser(obj.User);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanRemoveUserCommand(UserInfo obj)
        {
            return true;
        }


        public DelegateCommand<TypeInformation> ResetTypePermissionCommand { get; set; }
        private void ExecuteResetTypePermissionCommand(TypeInformation obj)
        {
            try
            {
                if (ActualRole == null) return;
                if (ActualRole is User)
                {
                    SecurityFramework.ResetUserTypePermission(obj.TypeInfo.Name, ActualRole as User);
                }
                else
                {
                    SecurityFramework.ResetGroupTypePermission(obj.TypeInfo.Name, ActualRole as Group);
                }
                NotifyPropertyChanged("ActualTypePermissionInformation");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanResetTypePermissionCommand(TypeInformation obj)
        {
            return true;
        }

        public DelegateCommand<ObjectInformation> ResetObjectPermissionCommand { get; set; }
        private void ExecuteResetObjectPermissionCommand(ObjectInformation obj)
        {
            try
            {
                if (ActualRole == null) return;
                if (ActualRole is User)
                {
                    SecurityFramework.ResetUserObjectPermission(obj.Obj, ActualRole as User);
                }
                else
                {
                    SecurityFramework.ResetGroupObjectPermission(obj.Obj, ActualRole as Group);
                }
                NotifyPropertyChanged("ActualObjectPermissionInformation");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanResetObjectPermissionCommand(ObjectInformation obj)
        {
            return true;
        }

        public DelegateCommand<UserInfo> RemoveUserFromActualGroupCommand { get; set; }
        private void ExecuteRemoveUserFromActualGroupCommand(UserInfo obj)
        {
            try
            {
                if (ActualGroup == null) return;
                if (obj == null) return;

                var x = from gi in obj.Groups
                        where gi.Group.Name == ActualGroup.Name && gi.IsMember == true
                        select gi;

                GroupMembershipInfo groupInfo = x.FirstOrDefault<GroupMembershipInfo>();
                if (groupInfo == null) return;

                if (MessageBox.Show("Do you really want to remove the user from this group?", "Delete Group Member", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    groupInfo.IsMember = false;

                    SecurityFramework.UpdateUser(groupInfo.User);
                }

                Group g = ActualGroup;
                ActualGroup = null;
                _Users = null;
                NotifyPropertyChanged("Users");
                NotifyPropertyChanged("GroupInfos");
                ActualGroup = g;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanRemoveUserFromActualGroupCommand(UserInfo obj)
        {
            return true;
        }


        public DelegateCommand<object> AddUserToActualGroupCommand { get; set; }
        private void ExecuteAddUserToActualGroupCommand(object obj)
        {
            try
            {
                if (ActualGroup == null) return;
                AddUserToGroupDialog dlg = new AddUserToGroupDialog();
                dlg.Owner = Application.Current.MainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.Facade = this;
                if (dlg.ShowDialog() == true)
                {
                    foreach (UserInfo ui in dlg.SelectedUserInfos)
                    {
                        var x = from gi in ui.Groups
                                where gi.Group.Name == ActualGroup.Name
                                select gi;


                        GroupMembershipInfo groupInfo = x.FirstOrDefault<GroupMembershipInfo>();
                        if (groupInfo == null) return;



                        groupInfo.IsMember = true;

                        SecurityFramework.UpdateUser(groupInfo.User);

                        Group g = ActualGroup;
                        ActualGroup = null;
                        _Users = null;
                        //_Groups = null;
                        //ActualUser = null;
                        //NotifyPropertyChanged("Groups");
                        NotifyPropertyChanged("Users");
                        NotifyPropertyChanged("GroupInfos");
                        ActualGroup = g;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RaiseCanExecuteChanged();
        }
        private bool CanAddUserToActualGroupCommand(object obj)
        {
            return true;
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


    public class GroupInfo : INotifyPropertyChanged
    {
        public Group Group { get; set; }
        private Framework _Framework;
        public GroupInfo(Group g, Framework fw)
        {
            Group = g;
            _Framework = fw;
        }

        #region INotifyPropertyChanged Members

        public List<User> Users
        {
            get
            {
                var x = from u in _Framework.Users
                        where u.Groups.Contains(this.Group)
                        select u;
                return x.ToList<User>();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
    public class UserInfo : INotifyPropertyChanged
    {
        public UserInfo(User user, List<Group> groups)
        {
            this.User = user;
            Groups = new ObservableCollection<GroupMembershipInfo>();
            foreach (Group group in groups)
            {
                GroupMembershipInfo gmi = new GroupMembershipInfo(group, this.User);
                gmi.PropertyChanged += gmi_PropertyChanged;
                Groups.Add(gmi);
            }
        }
        public void AddGroupToList(Group group)
        {
            GroupMembershipInfo gmi = new GroupMembershipInfo(group, this.User);
            gmi.PropertyChanged += gmi_PropertyChanged;
            Groups.Add(gmi);
        }
        public void RemoveGroupFromList(Group group)
        {
            try
            {
                GroupMembershipInfo gmi = Groups.First(gi => gi.Group.Name == group.Name);
                if (gmi == null)
                    return;
                gmi.PropertyChanged -= gmi_PropertyChanged;
                Groups.Remove(gmi);
            }
            catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
        }
        public bool GroupMembershipChanged
        {
            get
            {
                return true;
            }
        }
        void gmi_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            NotifyPropertyChanged("GroupMembershipChanged");
            NotifyPropertyChanged("GroupsNotAssigned");
        }

        ~UserInfo()
        {
            foreach (GroupMembershipInfo gmi in Groups)
            {

                gmi.PropertyChanged -= gmi_PropertyChanged;

            }
        }

        public User User { get; set; }
        public ObservableCollection<GroupMembershipInfo> Groups { get; set; }
        public List<GroupMembershipInfo> GroupsNotAssigned
        {
            get
            {
                var x = from gmi in Groups
                        where gmi.IsMember == false
                        select gmi;

                return x.ToList<GroupMembershipInfo>();

            }
        }
        public override string ToString()
        {
            return User.Account;
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
    public class GroupMembershipInfo : INotifyPropertyChanged
    {
        public GroupMembershipInfo(Group group, User user)
        {
            Group = group;
            User = user;
        }
        public User User { get; set; }
        public Group Group { get; set; }
        public bool IsMember
        {
            get
            {
                return User.Groups.Find(g => g.Id == this.Group.Id) != null;
            }
            set
            {
                if (value == true)
                {
                    User.AddToGroup(this.Group);
                }
                if (value == false)
                {
                    User.RemoveFromGroup(this.Group);
                }
                NotifyPropertyChanged("IsMember");
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

    public class TypeInformation
    {
        public TypeInformation(Framework fw, Role actualRole, TypeInfo ti)
        {
            SecurityFramework = fw;
            ActualRole = actualRole;
            TypeInfo = ti;
        }
        public Framework SecurityFramework { get; set; }
        public Role ActualRole { get; set; }
        public TypeInfo TypeInfo { get; set; }

        public List<TypePermissionInformation> Permissions
        {
            get
            {
                List<TypePermissionInformation> list = new List<TypePermissionInformation>();
                if (this.ActualRole is User)
                {
                    List<PermissionInfo> pis = SecurityFramework.GetUserTypePermissions(TypeInfo.Name, ActualRole as User);
                    foreach (PermissionInfo pi in pis)
                    {
                        bool isInherited = SecurityFramework.IsUserTypePermissionInherited(TypeInfo.Name, pi.PermissionName, ActualRole as User);
                        list.Add(new TypePermissionInformation(SecurityFramework, TypeInfo, pi, this.ActualRole, isInherited));
                    }
                }
                else
                {
                    List<PermissionInfo> pis = SecurityFramework.GetGroupTypePermissions(TypeInfo.Name, ActualRole as Group);
                    foreach (PermissionInfo pi in pis)
                    {
                        bool isInherited = SecurityFramework.IsGroupTypePermissionInherited(TypeInfo.Name, pi.PermissionName, ActualRole as Group);
                        list.Add(new TypePermissionInformation(SecurityFramework, TypeInfo, pi, this.ActualRole, isInherited));
                    }

                }
                return list;
            }
        }

    }
    public class TypePermissionInformation : INotifyPropertyChanged
    {
        public TypePermissionInformation(Framework fw, TypeInfo ti, PermissionInfo pi, Role ar, bool isInherited)
        {
            SecurityFramework = fw;
            TypeInfo = ti;
            PermissionInfo = pi;
            ActualRole = ar;
            IsInherited = isInherited;
        }
        public Framework SecurityFramework { get; set; }
        public TypeInfo TypeInfo { get; set; }
        public PermissionInfo PermissionInfo { get; set; }
        public Role ActualRole { get; set; }
        public bool Access
        {
            get
            {
                return PermissionInfo.Access;
            }
            set
            {

                if (ActualRole is User)
                {
                    SecurityFramework.SetUserTypePermission(TypeInfo.Name, PermissionInfo.PermissionName, value, ActualRole as User);
                    this.IsInherited = SecurityFramework.IsUserTypePermissionInherited(TypeInfo.Name, PermissionInfo.PermissionName, ActualRole as User);
                }
                else
                {
                    SecurityFramework.SetGroupTypePermission(TypeInfo.Name, PermissionInfo.PermissionName, value, ActualRole as Group);
                    this.IsInherited = SecurityFramework.IsGroupTypePermissionInherited(TypeInfo.Name, PermissionInfo.PermissionName, ActualRole as Group);
                }



            }
        }

        private bool _IsInherited;
        public bool IsInherited
        {
            get
            {
                return _IsInherited;
            }
            set
            {
                _IsInherited = value;
                NotifyPropertyChanged("IsInherited");
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

    public class ObjectInformation
    {
        public ObjectInformation(Framework fw, Role actualRole, object obj)
        {
            SecurityFramework = fw;
            ActualRole = actualRole;
            Obj = obj;
        }
        public Framework SecurityFramework { get; set; }
        public Role ActualRole { get; set; }
        public object Obj { get; set; }


        public List<ObjectPermissionInformation> Permissions
        {
            get
            {
                List<ObjectPermissionInformation> list = new List<ObjectPermissionInformation>();
                if (this.ActualRole is User)
                {
                    List<PermissionInfo> pis = SecurityFramework.GetUserObjectPermissions(Obj, ActualRole as User);
                    foreach (PermissionInfo pi in pis)
                    {
                        bool isInherited = SecurityFramework.IsUserObjectPermissionInherited(Obj, pi.PermissionName, ActualRole as User);

                        list.Add(new ObjectPermissionInformation(SecurityFramework, Obj, pi, this.ActualRole, isInherited));
                    }
                }
                else
                {
                    List<PermissionInfo> pis = SecurityFramework.GetGroupObjectPermissions(Obj, ActualRole as Group);
                    foreach (PermissionInfo pi in pis)
                    {
                        bool isInherited = SecurityFramework.IsGroupObjectPermissionInherited(Obj, pi.PermissionName, ActualRole as Group);
                        list.Add(new ObjectPermissionInformation(SecurityFramework, Obj, pi, this.ActualRole, isInherited));
                    }

                }
                return list;
            }
        }

    }
    public class ObjectPermissionInformation : INotifyPropertyChanged
    {
        public ObjectPermissionInformation(Framework fw, object obj, PermissionInfo pi, Role ar, bool isInherited)
        {
            SecurityFramework = fw;
            Obj = obj;
            PermissionInfo = pi;
            ActualRole = ar;
            IsInherited = isInherited;
        }
        public Framework SecurityFramework { get; set; }


        public object Obj;

        public PermissionInfo PermissionInfo { get; set; }
        public Role ActualRole { get; set; }
        public bool Access
        {
            get
            {
                return PermissionInfo.Access;
            }
            set
            {

                if (ActualRole is User)
                {
                    SecurityFramework.SetUserObjectTypePermission(Obj, PermissionInfo.PermissionName, value, ActualRole as User);
                    this.IsInherited = SecurityFramework.IsUserObjectPermissionInherited(Obj, PermissionInfo.PermissionName, ActualRole as User);
                }
                else
                {
                    SecurityFramework.SetGroupObjectTypePermission(Obj, PermissionInfo.PermissionName, value, ActualRole as Group);
                    this.IsInherited = SecurityFramework.IsGroupObjectPermissionInherited(Obj, PermissionInfo.PermissionName, ActualRole as Group);
                }


            }
        }
        private bool _IsInherited;
        public bool IsInherited
        {
            get
            {
                return _IsInherited;
            }
            set
            {
                _IsInherited = value;
                NotifyPropertyChanged("IsInherited");
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



    public class EntityInfo
    {
        public string Name { get; set; }
        public string Locator { get; set; }
        public object Tag { get; set; }
    }
    public class DicInfo : EntityInfo
    {
        public List<ChapterInfo> Chapters { get; set; }
    }

    public class ChapterInfo : EntityInfo
    {
        public List<CardInfo> Cards { get; set; }
    }

    public class CardInfo : EntityInfo
    {
    }
}
