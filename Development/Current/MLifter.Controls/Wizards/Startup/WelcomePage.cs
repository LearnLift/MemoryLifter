using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using MLifter.Controls.Properties;

namespace MLifter.Controls.Wizards.Startup
{
    public partial class WelcomePage : MLifter.WizardPage
    {
        private RegisterPage registerPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomePage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public WelcomePage(string registrationService)
        {
            registerPage = new RegisterPage(registrationService);
            InitializeComponent();

            string version = MLifter.Components.AssemblyData.Version;
            labelWelcome.Text = string.Format(labelWelcome.Text, version);
        }

        /// <summary>
        /// Handles the Load event of the WelcomePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void WelcomePage_Load(object sender, EventArgs e)
        {
            CheckRegisterPage();

            Thread showNews = new Thread(new ThreadStart(ShowNews));
            showNews.Name = "Show News Thread";
            showNews.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            showNews.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            showNews.Start();
        }

        /// <summary>
        /// Shows the news.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        private void ShowNews()
        {
            webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = News.LoadingMessage; });

            if (Tools.IsUserOnline())
            {
                string newsContent;
                int newNewsCount;
                DateTime newsDate = Settings.Default.NewsDate;
                MLifter.BusinessLayer.News news = new MLifter.BusinessLayer.News(Resources.RssFeedTransformer);
                if (news.GetNewsFeed(out newsContent, ref newsDate, out newNewsCount, Settings.Default.NewsFeedRss))
                {
                    Settings.Default.NewsDate = newsDate;
                    webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = newsContent; });
                }
                else
                    webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = News.UnavailableMessage; });
            }
            else
                webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = News.UnavailableMessage; });
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxRegister control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void checkBoxRegister_CheckedChanged(object sender, EventArgs e)
        {
            CheckRegisterPage();
        }

        /// <summary>
        /// Checks the register page.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void CheckRegisterPage()
        {
            if (checkBoxRegister.Checked)
                ParentWizard.Pages.Add(registerPage);
            else
                ParentWizard.Pages.Remove(registerPage);

            ParentWizard.UpdateButtonStates();
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabelEnlargeNews control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        private void linkLabelEnlargeNews_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            News newsForm = new News();
            newsForm.HelpNamespace = this.ParentWizard.HelpFile;
            newsForm.Prepare(false);
        }
        /// <summary>
        /// Gets or sets a value indicating whether show help.
        /// </summary>
        /// <value><c>true</c> if [show help]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        public bool ShowHelpAtStartup { get { return checkBoxShowHelp.Checked; } set { checkBoxShowHelp.Checked = value; } }

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
    }
}

