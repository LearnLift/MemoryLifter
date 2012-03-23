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
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;

namespace MLifter.Controls.LearningWindow
{
    [Docking(DockingBehavior.AutoDock)]
    public partial class QuestionPanel : UserControl, ILearnUserControl
    {
        LearnLogic learnlogic = null;
        Thread generateQuestionThread = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionPanel"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public QuestionPanel()
        {
            InitializeComponent();

            webBrowserQuestion.PreviewKeyDown += new PreviewKeyDownEventHandler(webBrowserQuestion_PreviewKeyDown);
        }

        /// <summary>
        /// Registers the learn logic.
        /// </summary>
        /// <param name="learnlogic">The learnlogic.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public void RegisterLearnLogic(LearnLogic learnlogic)
        {
            this.learnlogic = learnlogic;
            this.learnlogic.CardStateChanged += new LearnLogic.CardStateChangedEventHandler(learnlogic_CardStateChanged);
            this.learnlogic.LearningModuleClosed += new EventHandler(learnlogic_LearningModuleClosed);
        }

        /// <summary>
        /// Handles the LearningModuleClosed event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2008-11-27</remarks>
        void learnlogic_LearningModuleClosed(object sender, EventArgs e)
        {
            //set the browser to an empty page
            this.QuestionBrowserUrl = new Uri("about:blank");
            Application.DoEvents();
        }

        /// <summary>
        /// Handles the CardStateChanged event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        void learnlogic_CardStateChanged(object sender, CardStateChangedEventArgs e)
        {
            if (e is CardStateChangedNewCardEventArgs ||
                (e is CardStateChangedShowResultEventArgs && ((CardStateChangedShowResultEventArgs)e).slideshow))
            {
                //abort old thread in case it is still running
                if (generateQuestionThread != null && generateQuestionThread.IsAlive)
                    generateQuestionThread.Abort();

                generateQuestionThread = new Thread(delegate()
                {
					// WORKAROUND for Windows Media Player 6.4 [ML-2122]
					if (MLifter.Generics.Methods.IsWMP7OrGreater())
					{
						Uri question = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareQuestion(e.dictionary.DictionaryDAL.Parent, e.cardid, e.dictionary.GenerateQuestion(e.cardid));
						string title = e.dictionary.Cards.GetCardByID(e.cardid).CurrentQuestionCaption;
						if (this.IsHandleCreated && !this.IsDisposed)
							this.Invoke(new MethodInvoker(delegate()
							{
								this.QuestionBrowserUrl = question;
								this.Title = title;
							}));
					}
					else
					{
						string content = e.dictionary.GenerateQuestion(e.cardid);
						string title = e.dictionary.Cards.GetCardByID(e.cardid).CurrentQuestionCaption;
						if (this.IsHandleCreated && !this.IsDisposed)
							this.Invoke(new MethodInvoker(delegate()
							{
								this.QuestionBrowserContent = content;
								this.Title = title;
							}));
					}

                });
                generateQuestionThread.IsBackground = true;
                generateQuestionThread.Name = "Generate Question Thread";
                generateQuestionThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                generateQuestionThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                generateQuestionThread.Start();

            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        [DefaultValue("Question"), Category("Appearance"), Localizable(true)]
        public string Title
        {
            get { return labelQuestion.Text; }
            set { labelQuestion.Text = value; }
        }

        /// <summary>
        /// Gets or sets the color of the title.
        /// </summary>
        /// <value>The color of the title.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        [DefaultValue(typeof(Color), "White"), Category("Appearance")]
        public Color TitleColor
        {
            get { return labelQuestion.ForeColor; }
            set { labelQuestion.ForeColor = value; }
        }

        /// <summary>
        /// Gets or sets the HTML content.
        /// </summary>
        /// <value>The content of the question browser.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        private string QuestionBrowserContent
        {
            get { return webBrowserQuestion.DocumentText; }
            set { webBrowserQuestion.Stop(); webBrowserQuestion.DocumentText = value; }
        }

        /// <summary>
        /// Gets or sets the Uri.
        /// </summary>
        /// <value>The Uri.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        private Uri QuestionBrowserUrl
        {
            get { return webBrowserQuestion.Url; }
            set { webBrowserQuestion.Stop(); webBrowserQuestion.Url = value; }
        }

        /// <summary>
        /// Handles the Navigating event of the webBrowserQuestion control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void webBrowserQuestion_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() != "about:blank" && e.Url.Scheme != "http")
            {
                e.Cancel = true;
                OnFileDropped(new FileDroppedEventArgs(e.Url.OriginalString));
            }
        }

        /// <summary>
        /// Occurs when [file dropped].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        [Description("Occurs when a file was dropped onto this control.")]
        public event FileDroppedEventHandler FileDropped;

        /// <summary>
        /// Raises the <see cref="E:FileDropped"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MLifter.Controls.LearningWindow.FileDroppedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void OnFileDropped(FileDroppedEventArgs e)
        {
            if (FileDropped != null)
                FileDropped(this, e);
        }

        /// <summary>
        /// Occurs when [browser key down].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        public event PreviewKeyDownEventHandler BrowserKeyDown;

        /// <summary>
        /// Handles the PreviewKeyDown event of the webBrowserQuestion control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        private void webBrowserQuestion_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (BrowserKeyDown != null)
                BrowserKeyDown(sender, e);
        }

        /// <summary>
        /// Handles the Click event of the buttonSlidePrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void buttonSlidePrevious_Click(object sender, EventArgs e)
        {
            showPreviousCard();
        }
        /// <summary>
        /// Shows the previous card.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void showPreviousCard()
        {
            if (learnlogic != null && learnlogic.SlideShow == true)
            {
                UserInputSubmit(this, new UserInputSubmitPreviousCard());
            }
        }

        /// <summary>
        /// Users the input submit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="MLifter.BusinessLayer.UserInputSubmitTextEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2009-04-28</remarks>
        void UserInputSubmit(object sender, UserInputSubmitEventArgs args)
        {
            try
            {
                learnlogic.OnUserInputSubmit(this, args);
            }
            catch (Generics.ServerOfflineException)
            {
                TaskDialog.MessageBox(Resources.LEARNING_WINDOW_SERVER_OFFLINE_DIALOG_TITLE, Resources.LEARNING_WINDOW_SERVER_OFFLINE_DIALOG_TITLE,
                        Resources.LEARNING_WINDOW_SERVER_OFFLINE_DIALOG_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Error);
                learnlogic.CloseLearningModuleWithoutSaving();
            }
        }
    }


    public delegate void FileDroppedEventHandler(object sender, FileDroppedEventArgs e);

    /// <summary>
    /// EventArgs for the FileDropped event.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-05-08</remarks>
    public class FileDroppedEventArgs : EventArgs
    {
        /// <summary>
        /// The filename.
        /// </summary>
        public string filename = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDroppedEventArgs"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public FileDroppedEventArgs(string filename)
        {
            this.filename = filename;
        }
    }
}
