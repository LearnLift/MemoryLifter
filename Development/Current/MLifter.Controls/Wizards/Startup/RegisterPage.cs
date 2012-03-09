using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using MLifter.Components;

namespace MLifter.Controls.Wizards.Startup
{
    public partial class RegisterPage : MLifter.WizardPage
    {
        bool registrationSent = false;

        string m_RegistrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        public RegisterPage(string registrationService)
        {
            m_RegistrationService = registrationService;
            InitializeComponent();

            //check if Text for email is too long and adjust textbox
            if (labelMail.Bounds.Right > textBoxMail.Bounds.Left)
            {
                int adjust = labelMail.Bounds.Right - textBoxMail.Bounds.Left + 10;
                Rectangle textboxmail = textBoxMail.Bounds;
                textboxmail.X += adjust;
                textboxmail.Width -= adjust;
                textBoxMail.Bounds = textboxmail;
            }
        }

        /// <summary>
        /// Handles the Load event of the RegisterPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-12</remarks>
        private void RegisterPage_Load(object sender, EventArgs e)
        {
            textBoxName.Focus();
        }

        /// <summary>
        /// Called if the next-button is clicked.
        /// </summary>
        /// <returns>
        /// 	<i>false</i> to abort, otherwise<i>true</i>
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-12</remarks>
        public override bool GoNext()
        {
            if (EmailValid())
            {
                SendRegistration();
                return base.GoNext();
            }
            else
            {
                MessageBox.Show(Properties.Resources.REGISTER_NOVALIDEMAIL_TEXT, Properties.Resources.REGISTER_NOVALIDEMAIL_CAPTION);
                return false;
            }
        }

        /// <summary>
        /// Sends the registration.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-12-12</remarks>
        private void SendRegistration()
        {
            MLifter.Components.Registration registration = new MLifter.Components.Registration(m_RegistrationService,
                textBoxName.Text, textBoxMail.Text, textBoxComment.Text, checkBoxNewsletter.Checked, AssemblyData.AssemblyVersion, AssemblyData.Title);
            registration.Submit();
            registrationSent = true;
        }

        /// <summary>
        /// Gets a value indicating whether [registration was sent].
        /// </summary>
        /// <value><c>true</c> if [registration sent]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-02-20</remarks>
        public bool RegistrationSent
        {
            get { return registrationSent; }
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            //TODO link the correct help topic id
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/Starting_Memory_Lifter.htm");
        }

        /// <summary>
        /// Validates the entered email-address.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-10</remarks>
        private bool EmailValid()
        {
            return System.Text.RegularExpressions.Regex.IsMatch(textBoxMail.Text, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$");
        }
    }
}

