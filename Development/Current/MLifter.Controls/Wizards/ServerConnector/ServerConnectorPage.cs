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
using System.IO;
using System.Text;
using System.Windows.Forms;

using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Diagnostics;
using MLifter.Controls.Properties;
using MLifter.Generics;

namespace MLifter.Controls.Wizards.ServerConnector
{
    /// <summary>
    /// A wizard page for configuring the server connection.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-07-23</remarks>
    public partial class ServerConnectorPage : MLifter.WizardPage
    {
        private LearnLogic m_learnLogic = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnectorPage"/> class.
        /// </summary>
        /// <param name="learnLogic">The learn logic.</param>
        /// <remarks>Documented by Dev02, 2008-07-23</remarks>
        public ServerConnectorPage(LearnLogic learnLogic)
        {
            InitializeComponent();
#if DEBUG
            if (File.Exists(connectionstringfile))
                textBoxConnectionString.Text = File.ReadAllText(connectionstringfile);
#endif
            m_learnLogic = learnLogic;
        }

        private string connectionstringfile = Path.Combine(Application.StartupPath, "connectionstring.txt");

        /// <summary>
        /// Called if the next-button is clicked.
        /// </summary>
        /// <returns>
        /// 	<i>false</i> to abort, otherwise<i>true</i>
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        public override bool GoNext()
        {

#if DEBUG
            File.WriteAllText(connectionstringfile, textBoxConnectionString.Text);
#endif

            try
            {
                m_learnLogic.CloseLearningModule();
                m_learnLogic.User.Authenticate((GetLoginInformation)LoginForm.OpenLoginForm, new ConnectionStringStruct(DatabaseType.PostgreSQL, textBoxConnectionString.Text, -1),
                    (DataAccessErrorDelegate)DataAccessError);
                return true;
            }
            catch (DAL.DatabaseVersionNotSupported)
            {
                MessageBox.Show(Properties.Resources.LOGIN_WIZARD_DATABASE_VERSION_NOT_SUPPORTED_TEXT, Properties.Resources.LOGIN_WIZARD_DATABASE_VERSION_NOT_SUPPORTED_CAPTION);
                return false;
            }
            catch (InvalidUsernameException)
            {
                MessageBox.Show(Properties.Resources.LOGIN_WIZARD_WRONG_USERNAME_OR_PASSWORD_TEXT, Properties.Resources.LOGIN_WIZARD_WRONG_USERNAME_OR_PASSWORD_CAPTION);
                return false;
            }
            catch (InvalidPasswordException)
            {
                MessageBox.Show(Properties.Resources.LOGIN_WIZARD_WRONG_USERNAME_OR_PASSWORD_TEXT, Properties.Resources.LOGIN_WIZARD_WRONG_USERNAME_OR_PASSWORD_CAPTION);
                return false;
            }
            catch (DAL.NoValidUserException)
            {
                Debug.WriteLine("Authenication failed or canceled by user."); ;
                return false;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), "Exception during attempt to connect.");
                return false;
            }
        }

        private void DataAccessError(object sender, Exception exp)
        {
            throw exp;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        public string ConnectionString
        {
            get
            {
                return textBoxConnectionString.Text;
            }
        }
    }
}
