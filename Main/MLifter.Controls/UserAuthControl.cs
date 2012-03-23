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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Security;
using System.Text;
using System.Windows.Forms;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Diagnostics;
using MLifter.Controls.Properties;
using MLifter.BusinessLayer;
using MLifter.Generics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;

namespace MLifter.Controls
{

    public partial class UserAuthControl : UserControl
    {
        private enum PasswordType
        {
            Hashed,
            ClearText
        }
        [Serializable()]
        private class SavedUserData
        {
            public bool SaveUsername;
            public bool SavePassword;
            public bool AutoLogin;
            public string Username;
            public string Password;

            /// <summary>
            /// Initializes a new instance of the <see cref="SavedUserData"/> class.
            /// </summary>
            /// <remarks>Documented by Dev05, 2009-03-04</remarks>
            public SavedUserData() { }

            /// <summary>
            /// Initializes a new instance of the <see cref="SavedUserData"/> class.
            /// </summary>
            /// <param name="saveUsername">if set to <c>true</c> [save username].</param>
            /// <param name="savePassword">if set to <c>true</c> [save password].</param>
            /// <param name="autoLogin">if set to <c>true</c> [auto login].</param>
            /// <param name="username">The username.</param>
            /// <param name="password">The password.</param>
            /// <remarks>Documented by Dev05, 2009-03-04</remarks>
            public SavedUserData(bool saveUsername, bool savePassword, bool autoLogin, string username, string password)
            {
                SaveUsername = saveUsername;
                SavePassword = savePassword;
                AutoLogin = autoLogin;
                Username = username;
                Password = password;
            }
        }
        private Dictionary<string, SavedUserData> persitancyData;
        private IConnectionString ConnectionString;

        /// <summary>
        /// Gets or sets a value indicating whether auto login is allowed or not.
        /// </summary>
        /// <value><c>true</c> if auto login is allowed; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        public bool AllowAutoLogin { get; set; }
        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>The name of the connection.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public string ConnectionName { get; private set; }
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public ConnectionStringStruct Connection { get; private set; }

        private bool showHeaderText = true;
        /// <summary>
        /// Gets or sets a value indicating whether to show header Text.
        /// </summary>
        /// <value><c>true</c> if the header Text is shown; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        [Description("Show the 'Sign in' header Text"), Browsable(true), DefaultValue(true)]
        public bool ShowHeaderText
        {
            get { return showHeaderText; }
            set
            {
                if (value == showHeaderText)
                    return;

                showHeaderText = value;
                if (value)
                {
                    tableLayoutPanelMain.RowStyles[0].Height = 20;
                    Height += Convert.ToInt32(tableLayoutPanelMain.RowStyles[0].Height);
                }
                else
                {
                    Height -= Convert.ToInt32(tableLayoutPanelMain.RowStyles[0].Height);
                    tableLayoutPanelMain.RowStyles[0].Height = 0;
                }
            }
        }

        private bool showOptions = false;
        /// <summary>
        /// Gets or sets a value indicating whether to show details or not.
        /// </summary>
        /// <value><c>true</c> if details are shown; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        [Description("Show the options"), Browsable(true), DefaultValue(false)]
        public bool ShowOptions
        {
            get { return showOptions; }
            set
            {
                if (value == showOptions)
                    return;

                showOptions = value;
                tableLayoutPanelMain.RowStyles[3].Height = 25;
                if (value)
                {
                    tableLayoutPanelMain.RowStyles[3].Height += 3 * tableLayoutPanelBottom.RowStyles[1].Height;
                    checkBoxSaveUsername.Visible = checkBoxSavePassword.Visible = checkBoxAutoLogin.Visible = true;
                    Height += System.Convert.ToInt32(3 * tableLayoutPanelBottom.RowStyles[1].Height);
                }
                else
                {
                    Height -= System.Convert.ToInt32(3 * tableLayoutPanelBottom.RowStyles[1].Height);
                    checkBoxSaveUsername.Visible = checkBoxSavePassword.Visible = checkBoxAutoLogin.Visible = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the login picture.
        /// </summary>
        /// <value>The login picture.</value>
        /// <remarks>Documented by Dev02, 2009-03-19</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), DefaultValue(typeof(Image), null), Description("Sets the login icon image.")]
        public Image LoginPicture
        {
            get
            {
                return pictureBoxIcon.Image;
            }
            set
            {
                if (pictureBoxIcon.Image == null && value != null)
                {
                    this.Width = this.Width + value.Width;
                    tableLayoutPanelMain.ColumnStyles[0].Width = tableLayoutPanelBottom.ColumnStyles[0].Width = value.Width;
                }
                else if (pictureBoxIcon.Image != null && value == null)
                {
                    this.Width = this.Width - Convert.ToInt32(tableLayoutPanelMain.ColumnStyles[0].Width);
                    tableLayoutPanelMain.ColumnStyles[0].Width = tableLayoutPanelBottom.ColumnStyles[0].Width = 0;
                }

                pictureBoxIcon.Image = value;
            }
        }

        private LoginError loginError = LoginError.NoError;
        private Color oldUsernameColor = Color.Empty;
        private Color oldPasswordColor = Color.Empty;
        /// <summary>
        /// Gets or sets the login error.
        /// </summary>
        /// <value>The login error.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        [Description("Set an Error, which occured during authentication"), Browsable(true), DefaultValue(LoginError.NoError)]
        public LoginError LoginError
        {
            get { return loginError; }
            set
            {
                if (oldUsernameColor == Color.Empty)
                    oldUsernameColor = comboBoxUserSelection.BackColor;
                if (oldPasswordColor == Color.Empty)
                    oldPasswordColor = textBoxPassword.BackColor;

                loginError = value;
                switch (value)
                {
                    case LoginError.NoError:
                    case LoginError.AlreadyLoggedIn:
                        comboBoxUserSelection.BackColor = oldUsernameColor;
                        textBoxPassword.BackColor = oldPasswordColor;
                        break;
                    case LoginError.InvalidUsername:
                        textBoxPassword.BackColor = oldPasswordColor;
                        comboBoxUserSelection.BackColor = Color.LightCoral;
                        comboBoxUserSelection.Focus();
                        comboBoxUserSelection.SelectAll();
                        break;
                    case LoginError.InvalidPassword:
                        comboBoxUserSelection.BackColor = oldUsernameColor;
                        textBoxPassword.BackColor = Color.LightCoral;
                        textBoxPassword.Focus();
                        textBoxPassword.SelectAll();
                        break;
                    case LoginError.WrongAuthentication:
                    case LoginError.ForbiddenAuthentication:
                        comboBoxUserSelection.Text = string.Empty;
                        comboBoxUserSelection.BackColor = oldUsernameColor;
                        textBoxPassword.Text = string.Empty;
                        textBoxPassword.BackColor = oldPasswordColor;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthControl"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public UserAuthControl()
        {
            InitializeComponent();
            AllowAutoLogin = true;
            textBoxPassword.Tag = PasswordType.ClearText;
        }

        private void UserAuthControl_Load(object sender, EventArgs e)
        {
            ShowOptions = showOptions;
            LoginError = loginError;
            buttonShowHideOptions.Text = " " + (ShowOptions ? Resources.LOGIN_HIDE_OPTIONS : Resources.LOGIN_SHOW_OPTIONS);
            buttonShowHideOptions.Image = ShowOptions ? Properties.Resources.arrow_up_bw : Properties.Resources.arrow_down_bw;
        }

        /// <summary>
        /// Initializes the control with the specified connection (name).
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="connection">The connection.</param>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public UserStruct? Initialize(ConnectionStringStruct connection, UserStruct lastUser)
        {
            ConnectionString = LearningModulesIndex.ConnectionsHandler.GetConnectionFromConnectionStringStruct(connection);
            if (ConnectionString == null)
            {
                LearningModulesIndexEntry entry = RecentLearningModules.GetRecentModules().Find(m => m.ConnectionString.ConnectionString == connection.ConnectionString);
                if (entry != null)
                    ConnectionString = entry.Connection;
            }

            ConnectionName = connection.Typ != DatabaseType.MsSqlCe ? (ConnectionString != null ? ConnectionString.Name : Resources.NO_CONNECTION_NAME) : Path.GetFileName(connection.ConnectionString);
            Connection = connection;

            if (connection.Typ == DatabaseType.MsSqlCe)
                comboBoxUserSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            else
                comboBoxUserSelection.DropDownStyle = ComboBoxStyle.DropDown;

            #region error handling
            switch (lastUser.LastLoginError)
            {
                case LoginError.AlreadyLoggedIn:
                    EmulatedTaskDialog taskDialog = new EmulatedTaskDialog();
                    taskDialog.Title = Resources.TASKDIALOG_KICK_TITLE;
                    taskDialog.MainInstruction = string.Format(Resources.TASKDIALOG_KICK_MAIN, lastUser.UserName);
                    taskDialog.Content = string.Format(Resources.TASKDIALOG_KICK_CONTENT, ConnectionName);
                    taskDialog.CommandButtons = Resources.TASKDIALOG_KICK_BUTTONS;

                    taskDialog.VerificationText = string.Empty;
                    taskDialog.VerificationCheckBoxChecked = false;
                    taskDialog.Buttons = TaskDialogButtons.None;
                    taskDialog.MainIcon = TaskDialogIcons.Warning;
                    taskDialog.FooterIcon = TaskDialogIcons.Warning;
                    taskDialog.MainImages = new Image[] { Resources.system_log_out, Resources.processStopBig };
                    taskDialog.HoverImages = new Image[] { Resources.system_log_out, Resources.processStopBig };
                    taskDialog.CenterImages = true;
                    taskDialog.BuildForm();
                    DialogResult dialogResult = taskDialog.ShowDialog();

                    if (dialogResult != DialogResult.Cancel && taskDialog.CommandButtonClickedIndex == 0)
                    {
                        lastUser.CloseOpenSessions = true;
                        return lastUser;
                    }
                    else
                        AllowAutoLogin = false;
                    break;
                case LoginError.InvalidUsername:
                    TaskDialog.MessageBox(Resources.TASKDIALOG_WRONG_USER_TITLE, Resources.TASKDIALOG_WRONG_USER_MAIN, Resources.TASKDIALOG_WRONG_USER_CONTENT,
                        TaskDialogButtons.OK, TaskDialogIcons.Error);
                    break;
                case LoginError.InvalidPassword:
                    TaskDialog.MessageBox(Resources.TASKDIALOG_WRONG_PASSWORD_TITLE, Resources.TASKDIALOG_WRONG_PASSWORD_MAIN, Resources.TASKDIALOG_WRONG_PASSWORD_CONTENT,
                        TaskDialogButtons.OK, TaskDialogIcons.Error);
                    break;
                case LoginError.WrongAuthentication:
                case LoginError.ForbiddenAuthentication:
                    TaskDialog.MessageBox(Resources.TASKDIALOG_WRONG_AUTHENTIFICATION_TITLE, Resources.TASKDIALOG_WRONG_AUTHENTIFICATION_MAIN, Resources.TASKDIALOG_WRONG_AUTHENTIFICATION_CONTENT,
                        TaskDialogButtons.OK, TaskDialogIcons.Error);
                    break;
                case LoginError.NoError:
                default:
                    break;
            }
            #endregion

            labelHeader.Text = string.Format(Resources.LOGIN_HEADER_TEXT, ConnectionName);

            buttonCancel.Enabled = connection.Typ != DatabaseType.MsSqlCe;

            try
            {
                SetListItems();
                UpdateSelection(false);

                #region persitancy
                Stream outputStream = null;
                try
                {
                    BinaryFormatter binary = new BinaryFormatter();
                    IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForAssembly();
                    outputStream = new IsolatedStorageFileStream(Settings.Default.AuthDataFile, FileMode.Open, storageFile);
                    outputStream = new CryptoStream(outputStream, Rijndael.Create().CreateDecryptor(Encoding.Unicode.GetBytes("mlifter"), Encoding.Unicode.GetBytes("omicron")), CryptoStreamMode.Read);
                    persitancyData = binary.Deserialize(outputStream) as Dictionary<string, SavedUserData>;
                    outputStream.Close();
                }
                catch { persitancyData = new Dictionary<string, SavedUserData>(); }
                finally
                {
                    if (outputStream != null)
                        outputStream.Close();
                }

                if (ConnectionString != null && persitancyData.ContainsKey(ConnectionString.ConnectionString))
                {
                    SavedUserData data = persitancyData[ConnectionString.ConnectionString];
                    if (data.SaveUsername)
                    {
                        checkBoxSaveUsername.Checked = true;
                        ShowOptions = true;
                        comboBoxUserSelection.Text = data.Username;
                        if (data.SavePassword)
                        {
                            checkBoxSavePassword.Checked = true;
                            textBoxPassword.Text = data.Password;
                            textBoxPassword.Tag = PasswordType.Hashed;
                            if (data.AutoLogin)
                            {
                                checkBoxAutoLogin.Checked = true;
                                if (AllowAutoLogin && !MLifter.BusinessLayer.User.PreventAutoLogin)
                                {
                                    Login();
                                    return GetActualUser();
                                }
                            }
                            else
                                checkBoxAutoLogin.Checked = false;
                        }
                        else
                            checkBoxSavePassword.Checked = false;
                    }
                    else
                    {
                        checkBoxSaveUsername.Checked = false;
                        ShowOptions = false;
                    }
                }
                #endregion

                comboBoxUserSelection.Focus();

                if (lastUser.LastLoginError != LoginError.NoError)
                {
                    comboBoxUserSelection.Text = lastUser.UserName;
                    textBoxPassword.Text = string.Empty;
                    textBoxPassword.Tag = PasswordType.ClearText;
                }

                LoginError = lastUser.LastLoginError;
            }
            catch (ConnectionNotSetException ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return null;
        }

        private void UpdatePercistancyData(UserStruct user)
        {
            if (ConnectionString == null)
                return;

            persitancyData[ConnectionString.ConnectionString] = new SavedUserData(checkBoxSaveUsername.Checked, checkBoxSavePassword.Checked, checkBoxAutoLogin.Checked,
                checkBoxSaveUsername.Checked ? comboBoxUserSelection.Text : string.Empty,
                checkBoxSavePassword.Checked ? user.AuthenticationType == UserAuthenticationTyp.ListAuthentication || textBoxPassword.Text.Length <= 0 ? string.Empty :
                    (PasswordType)textBoxPassword.Tag == PasswordType.Hashed ? textBoxPassword.Text : Methods.GetHashedPassword(textBoxPassword.Text) : string.Empty);

            Stream outputStream = null;
            try
            {
                BinaryFormatter binary = new BinaryFormatter();
                IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForAssembly();
                outputStream = new IsolatedStorageFileStream(Settings.Default.AuthDataFile, FileMode.Create, storageFile);
                outputStream = new CryptoStream(outputStream, Rijndael.Create().CreateEncryptor(Encoding.Unicode.GetBytes("mlifter"), Encoding.Unicode.GetBytes("omicron")), CryptoStreamMode.Write);
                binary.Serialize(outputStream, persitancyData);
            }
            catch (Exception e) { Trace.WriteLine(e.ToString()); }
            finally
            {
                if (outputStream != null)
                    outputStream.Close();
            }
        }

        [Description("Event on the Login"), Browsable(true)]
        public event EventHandler<UserLoginEventArgs> Login_Click;
        private void bLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void Login()
        {
            if (comboBoxUserSelection.Text.Length == 0)
                return;

            if (Login_Click != null)
                Login_Click(this, new UserLoginEventArgs(GetActualUser()));
        }

        private UserStruct GetActualUser()
        {
            UpdateSelection(true);
            UserStruct user = comboBoxUserSelection.SelectedItem is UserStruct ? (UserStruct)comboBoxUserSelection.SelectedItem :
                new UserStruct(comboBoxUserSelection.Text, UserAuthenticationTyp.FormsAuthentication);
            user.Password = user.AuthenticationType == UserAuthenticationTyp.ListAuthentication || textBoxPassword.Text.Length <= 0 ? string.Empty :
                (PasswordType)textBoxPassword.Tag == PasswordType.Hashed ? textBoxPassword.Text : Methods.GetHashedPassword(textBoxPassword.Text);
            UpdatePercistancyData(user);
            return user;
        }

        [Description("Event on Cancel"), Browsable(true)]
        public event EventHandler Cancel_Clicked;
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (Cancel_Clicked != null)
                Cancel_Clicked(this, e);
        }

        private void lbUserSelection_SelectedIndexChanged(object sender, EventArgs e) { UpdateSelection(false); }

        private void UpdateSelection(bool searchItem)
        {
            if (searchItem && comboBoxUserSelection.SelectedItem == null)
            {
                int sel = comboBoxUserSelection.SelectionStart;
                int index = comboBoxUserSelection.FindStringExact(comboBoxUserSelection.Text);
                comboBoxUserSelection.SelectedIndex = index;
                comboBoxUserSelection.SelectionStart = sel;
            }

            UserStruct userStruct = comboBoxUserSelection.SelectedItem is UserStruct ? (UserStruct)comboBoxUserSelection.SelectedItem :
                new UserStruct(comboBoxUserSelection.Text, UserAuthenticationTyp.FormsAuthentication);

            if (userStruct.AuthenticationType.HasValue)
                ChangeUserAuthType(userStruct.AuthenticationType.Value);
        }

        private void ChangeUserAuthType(UserAuthenticationTyp type)
        {
            if (type == UserAuthenticationTyp.FormsAuthentication)
            {
                labelPassword.Enabled = true;
                textBoxPassword.Enabled = true;
            }
            else if (type == UserAuthenticationTyp.ListAuthentication)
            {
                labelPassword.Enabled = false;
                textBoxPassword.Enabled = false;
                textBoxPassword.Text = String.Empty;
            }
        }

        private void SetListItems()
        {
            comboBoxUserSelection.Items.Clear();

            foreach (UserStruct user in UserFactory.GetUserList(Connection))
                if (user.AuthenticationType.HasValue && (user.AuthenticationType.Value != UserAuthenticationTyp.LocalDirectoryAuthentication))
                    comboBoxUserSelection.Items.Add((object)user);

            int position = comboBoxUserSelection.FindStringExact(Properties.Settings.Default.LastUser);
            if (position >= 0)
                comboBoxUserSelection.SelectedIndex = position;
            else
                comboBoxUserSelection.SelectedIndex = 0;

            if (comboBoxUserSelection.SelectedItem is UserStruct)
                ChangeUserAuthType(((UserStruct)comboBoxUserSelection.SelectedItem).AuthenticationType.Value);
        }

        private void comboBoxUserSelection_TextChanged(object sender, EventArgs e) { UpdateSelection(true); }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bLogin_Click(sender, e);
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxPassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            textBoxPassword.Tag = PasswordType.ClearText;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxSaveUsername control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        private void checkBoxSaveUsername_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSaveUsername.Checked)
                checkBoxSavePassword.Enabled = true;
            else
            {
                checkBoxSavePassword.Checked = false;
                checkBoxSavePassword.Enabled = false;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxSavePassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-04</remarks>
        private void checkBoxSavePassword_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSavePassword.Checked)
                checkBoxAutoLogin.Enabled = true;
            else
            {
                checkBoxAutoLogin.Checked = false;
                checkBoxAutoLogin.Enabled = false;
            }
        }

        /// <summary>
        /// Gets or sets the button appearance.
        /// </summary>
        /// <value>The button appearance.</value>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), DefaultValue(typeof(LinklabelButton.LinklabelButtonAppearance), "Button"), Description("Switches through the different appearance modes of the control.")]
        public LinklabelButton.LinklabelButtonAppearance ButtonAppearance
        {
            get
            {
                return bLogin.Appearance;
            }
            set
            {
                bLogin.Appearance = buttonCancel.Appearance = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the bottom.
        /// </summary>
        /// <value>The color of the bottom.</value>
        /// <remarks>Documented by Dev02, 2009-03-19</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), DefaultValue(typeof(Color), null), Description("Sets the color of the bottom table pane.")]
        public Color BottomColor
        {
            get { return tableLayoutPanelBottom.BackColor; }
            set { tableLayoutPanelBottom.BackColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of the bottom.
        /// </summary>
        /// <value>The color of the bottom.</value>
        /// <remarks>Documented by Dev02, 2009-03-19</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), DefaultValue(typeof(Color), null), Description("Sets the color of the bottom table pane border.")]
        public Color BottomBorderColor
        {
            get { return panelBackColor.BackColor; }
            set { panelBackColor.BackColor = value; }
        }

        /// <summary>
        /// Gets the login button control. (To support default button actions.)
        /// </summary>
        /// <value>The login button control.</value>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        public IButtonControl LoginButtonControl
        {
            get { return bLogin; }
        }

        /// <summary>
        /// Gets the cancel button control. (To support default button actions.)
        /// </summary>
        /// <value>The cancel button control.</value>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        public IButtonControl CancelButtonControl
        {
            get { return buttonCancel; }
        }

        /// <summary>
        /// Handles the Leave event of the comboBoxUserSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-05-07</remarks>
        private void comboBoxUserSelection_Leave(object sender, EventArgs e) { }

        private void buttonShowHideOptions_Click(object sender, EventArgs e)
        {
            ShowOptions = !ShowOptions;
            buttonShowHideOptions.Text = " " + (ShowOptions ? Resources.LOGIN_HIDE_OPTIONS : Resources.LOGIN_SHOW_OPTIONS);
        }

        private void buttonShowHideOptions_MouseDown(object sender, MouseEventArgs e)
        {
            buttonShowHideOptions.Image = ShowOptions ? Properties.Resources.arrow_up_color_pressed : Properties.Resources.arrow_down_color_pressed;
        }

        private void buttonShowHideOptions_MouseEnter(object sender, EventArgs e)
        {
            buttonShowHideOptions.Image = ShowOptions ? Properties.Resources.arrow_up_color : Properties.Resources.arrow_down_color;
        }

        private void buttonShowHideOptions_MouseLeave(object sender, EventArgs e)
        {
            buttonShowHideOptions.Image = ShowOptions ? Properties.Resources.arrow_up_bw : Properties.Resources.arrow_down_bw;
        }

        private void buttonShowHideOptions_MouseUp(object sender, MouseEventArgs e)
        {
            buttonShowHideOptions.Image = ShowOptions ? Properties.Resources.arrow_up_color : Properties.Resources.arrow_down_color;
        }
    }

    /// <summary>
    /// EvntArgs to deliver a UserStruct.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-03</remarks>
    public class UserLoginEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected user.
        /// </summary>
        /// <value>The selected user.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public UserStruct SelectedUser { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginEventArgs"/> class.
        /// </summary>
        /// <param name="selectedUser">The selected user.</param>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public UserLoginEventArgs(UserStruct selectedUser)
        {
            SelectedUser = selectedUser;
        }
    }
}
