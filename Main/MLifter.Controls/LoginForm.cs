using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Controls.Properties;

namespace MLifter.Controls
{
    public partial class LoginForm : Form
    {
        /// <summary>
        /// Gets the selected user.
        /// </summary>
        /// <value>The selected user.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public UserStruct SelectedUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();

            //fix for [ML-1490] Enter on Login Form doesn't log in always
            this.AcceptButton = AuthentificationControl.LoginButtonControl;
            this.CancelButton = AuthentificationControl.CancelButtonControl;
        }

        public static UserStruct? OpenLoginForm(UserStruct userStruct, ConnectionStringStruct connection)
        {
            LoginForm loginForm = new LoginForm();

            //select a server icon depending on connection type
            switch (connection.Typ)
            {
                case DatabaseType.Xml:
                case DatabaseType.Unc:
                case DatabaseType.MsSqlCe:
                    loginForm.ServerIcon = Resources.learning_48;
                    break;
                case DatabaseType.PostgreSQL:
                case DatabaseType.Web:
                    loginForm.ServerIcon = Resources.world_48;
                    break;
                default:
                    loginForm.ServerIcon = Resources.login_48;
                    break;
            }

            UserStruct? newUser = loginForm.AuthentificationControl.Initialize(connection, userStruct);

            if (newUser.HasValue)
                return newUser;

            if (loginForm.ShowDialog() == DialogResult.OK)
                return loginForm.SelectedUser;
            else
                return null;
        }

        /// <summary>
        /// Sets the server icon.
        /// </summary>
        /// <value>The server icon.</value>
        /// <remarks>Documented by Dev02, 2009-03-12</remarks>
        public Image ServerIcon
        {
            set
            {
                AuthentificationControl.LoginPicture = value;
            }
        }

        private void AuthentificationControl_Login_Click(object sender, UserLoginEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SelectedUser = e.SelectedUser;
            this.Close();
        }

        private void AuthentificationControl_Cancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdateHeight()
        {
            Height = AuthentificationControl.Height + Height - ClientSize.Height;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            UpdateHeight();
        }

        private void AuthentificationControl_SizeChanged(object sender, EventArgs e)
        {
            UpdateHeight();
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            AuthentificationControl.LoginError = AuthentificationControl.LoginError;
            TopMost = false;
            TopMost = true;
        }
    }
}