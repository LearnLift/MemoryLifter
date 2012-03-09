using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using MLifter.Controls.Properties;
using MLifter.BusinessLayer;

namespace MLifter.Controls
{
    /// <summary>
    /// Loads news from the web and displays it using arrayList pop-up window, if the news page has changed.
    /// </summary>
    /// <remarks>Documented by Dev01, 2007-07-20</remarks>
    public class News : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button btnClose;
        private MLifter.Components.MLifterWebBrowser Browser;
        private Label lblConnectionError;
        MLifter.BusinessLayer.News news;
        private HelpProvider MainHelp;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Constructor of the News Object
        /// </summary>
        /// <exception>no exception</exception>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public News()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Browser.IsWebBrowserContextMenuEnabled = false;
        }

        /// <summary>
        /// Gets or sets the help namespace.
        /// </summary>
        /// <value>The help namespace.</value>
        /// <remarks>Documented by Dev02, 2008-03-07</remarks>
        public string HelpNamespace
        {
            get { return MainHelp.HelpNamespace; }
            set { MainHelp.HelpNamespace = value; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(News));
            this.btnClose = new System.Windows.Forms.Button();
            this.Browser = new MLifter.Components.MLifterWebBrowser();
            this.lblConnectionError = new System.Windows.Forms.Label();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            // 
            // Browser
            // 
            this.Browser.AllowWebBrowserDrop = false;
            this.Browser.IsWebBrowserContextMenuEnabled = false;
            resources.ApplyResources(this.Browser, "Browser");
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.WebBrowserShortcutsEnabled = false;
            this.Browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Browser_DocumentCompleted);
            // 
            // lblConnectionError
            // 
            resources.ApplyResources(this.lblConnectionError, "lblConnectionError");
            this.lblConnectionError.Name = "lblConnectionError";
            // 
            // News
            // 
            this.AcceptButton = this.btnClose;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.lblConnectionError);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "News";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// This method checks the settings and starts the news fetching thread when necessary.
        /// </summary>
        /// <param name="startup">if set to <c>true</c> called during startup.</param>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        /// <remarks>Documented by Dev02, 2007-11-28</remarks>
        public void Prepare(bool startup)
        {
            //ensures that the window handle gets created under the stat thread
            this.CreateHandle();

            Thread thread = new Thread(new ParameterizedThreadStart(PrepareAsync));
            thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            thread.Name = "News Thread";
            thread.Start(startup);
        }

        /// <summary>
        /// Checks for news updates and displays them, if necessary. Is the Async part of prepare.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <remarks>Documented by Dev02, 2007-11-28</remarks>
        private void PrepareAsync(object start)
        {
            if (news == null) //prepare the news transformer in async news thread
                news = new MLifter.BusinessLayer.News(Resources.RssFeedTransformer);

            bool startup = (bool)start;

            if (!startup) //show dialog immediately
                Browser.BeginInvoke((MethodInvoker)delegate { Browser.DocumentText = News.LoadingMessage; });

            if (!Tools.IsUserOnline())
            {
                if (startup)
                    return;
                else
                {
                    lblConnectionError.Visible = true;
                    Browser.Visible = false;
                }
            }
            else
            {
                string newsContent;
                int newNewsCount;
                DateTime newsDate = Settings.Default.NewsDate;
                if (news.GetNewsFeed(out newsContent, ref newsDate, out newNewsCount, Settings.Default.NewsFeedRss))
                {
                    if (newNewsCount > 0 || !startup)
                    {
                        Browser.BeginInvoke((MethodInvoker)delegate { Browser.DocumentText = newsContent; });
                        Settings.Default.NewsDate = newsDate;
                        Settings.Default.Save();
                    }
                }
                else if (!startup)
                {
                    lblConnectionError.Visible = true;
                    Browser.Visible = false;
                }
            }
        }

        /// <summary>
        /// This method finally shows News form and updates the NewsDate RegistryKey
        /// </summary>
        /// <param name="sender">sender not used</param>
        /// <param name="e">no exceptions in use</param>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ShowDialog();
        }


        /// <summary>
        /// Gets the loading message.
        /// </summary>
        /// <value>The loading message.</value>
        /// <remarks>Documented by Dev03, 2009-05-08</remarks>
        internal static string LoadingMessage { get { return String.Format(Resources.NEWS_MESSAGE_ENVELOPE, Resources.NEWS_LOADING); } }
        /// <summary>
        /// Gets the unavailable message.
        /// </summary>
        /// <value>The unavailable message.</value>
        /// <remarks>Documented by Dev03, 2009-05-08</remarks>
        internal static string UnavailableMessage { get { return String.Format(Resources.NEWS_MESSAGE_ENVELOPE, Resources.NEWS_UNAVAILABLE); } }

    }
}
