using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Kerido.Controls;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Components;

namespace MLifter.Controls.LearningWindow
{
    [Docking(DockingBehavior.AutoDock)]
    public partial class AnswerPanel : UserControl, ILearnUserControl
    {

        #region override
        private Image BackupBackgroundImage = null;
        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        /// <remarks>Documented by Dev07, 2009-05-05</remarks>
        protected override void OnPaintBackground(PaintEventArgs e)
        {

            if (ComponentsHelper.IsResizing)
            {
                if (BackgroundImage != null)
                {
                    BackupBackgroundImage = BackgroundImage.Clone() as Image;
                    BackgroundImage = null;
                }
                base.OnPaintBackground(e);
            }
            else if (!ComponentsHelper.IsResizing)
            {

                if (BackupBackgroundImage != null)
                {
                    BackgroundImage = BackupBackgroundImage;
                    BackupBackgroundImage = null;
                }


                base.OnPaintBackground(e);

                using (Graphics g = e.Graphics)
                {

                    //paint Image
                    if (imageRightCorner != null)
                    {

                        Size size = new Size(imageRightCorner.Width, imageRightCorner.Height);
                        Point start = new Point(this.Width - size.Width - 5, this.Height - size.Height - 10);
                        if (this.Width < imageRightCorner.Width)
                        {
                            do
                            {
                                double factor = 0.5;
                                size = new Size((int)(imageRightCorner.Width * factor), (int)(imageRightCorner.Height * factor));
                                start = new Point(this.Width - size.Width - 5, this.Height - size.Height - 10);
                            } while (this.Width <= size.Width);
                        }

                        Rectangle dest = new Rectangle(start, size);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawImage(imageRightCorner, dest);
                    }
                }
            }
        }
        #endregion override

        #region properties
        private Image imageRightCorner;
        [DefaultValue(typeof(Image), null), Category("Appearance-BackgroundImage"), Description("The Image for the lower right corner")]
        public Image ImageRightCorner
        {
            get { return imageRightCorner; }
            set { imageRightCorner = value; }
        }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        [DefaultValue("Answer"), Category("Appearance"), Localizable(true)]
        public string Title
        {
            get { return labelAnswer.Text; }
            set { labelAnswer.Text = value; }
        }

        /// <summary>
        /// Gets or sets the color of the title.
        /// </summary>
        /// <value>The color of the title.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        [DefaultValue(typeof(Color), "White"), Category("Appearance")]
        public Color TitleColor
        {
            get { return labelAnswer.ForeColor; }
            set { labelAnswer.ForeColor = value; }
        }
        Thread generateAnswerThread = null;
        LearnLogic learnlogic = null;
        DateTime learnLogicWorkingStart;
        private EventArgs cardEventArgs = null;
        #endregion properties

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerPanel"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public AnswerPanel()
        {
            this.SuspendLayout();
            InitializeComponent();
            this.ResumeLayout();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            webBrowserAnswer.PreviewKeyDown += new PreviewKeyDownEventHandler(webBrowserAnswer_PreviewKeyDown);

            mLifterTextBox.Correct += new EventHandler(mLifterTextBox_Correct);
            mLifterTextBox.Wrong += new EventHandler(mLifterTextBox_Wrong);
            multipleChoice.ButtonKeyUp += new KeyEventHandler(multipleChoice_ButtonKeyUp);
            mLifterTextBox.WelcomeTipp = Resources.MLIFTER_TEXTBOX_WELCOME_TIPP;
            mLifterTextBox.Resize += new EventHandler(mLifterTextBox_Resize);
            panelTextBox.Resize += new EventHandler(panelTextBox_Resize);

            //reset pages
            multiPaneControlMain.SelectedPage = null;

            //set tooltips
            dockingButtonDontAskAgain.SetToolTip(Resources.TOOLTIP_SELF_ASSESSEMENT_DONTASK);
        }
        #endregion constructor




        #region controls

        public void UpdateCulture()
        {
            dockingButtonDontAskAgain.SetToolTip(Resources.TOOLTIP_SELF_ASSESSEMENT_DONTASK);
            dockingButtonDontAskAgain.Text = new ComponentResourceManager(this.GetType()).GetString(dockingButtonDontAskAgain.Name + ".Text");
            mLifterTextBox.WelcomeTipp = Resources.MLIFTER_TEXTBOX_WELCOME_TIPP;
        }
        /// <summary>
        /// Gets the info bar control.
        /// </summary>
        /// <value>The info bar control.</value>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        public Control InfoBarControl { get { return panelMultiPane; } }
        /// <summary>
        /// Gets the additional info bar suspend control.
        /// </summary>
        /// <value>The additional info bar suspend control.</value>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        public Control AdditionalInfoBarSuspendControl { get { return gradientPanelAnswer; } }

        #endregion controls

        #region selfAssesment
        public event EventHandler SelfAssesmentKeyUp;
        protected virtual void OnSelfAssesmentKeyUp(EventArgs e)
        {
            if (SelfAssesmentKeyUp != null)
                SelfAssesmentKeyUp(this, e);
        }
        public event EventHandler SelfAssesmentKeyDown;
        protected virtual void OnSelfAssesmentKeyDown(EventArgs e)
        {
            if (SelfAssesmentKeyDown != null)
                SelfAssesmentKeyDown(this, e);
        }
        /// <summary>
        /// Submits the self assessment reponse.
        /// </summary>
        /// <param name="doknow">if set to <c>true</c> [doknow].</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        public void SubmitSelfAssessmentReponse(bool doknow, bool dontaskagain)
        {
            if (learnlogic != null && cardEventArgs is CardStateChangedShowResultEventArgs)
            {
                //whether the question was answered right or wrong by the user
                AnswerResult result = ((CardStateChangedShowResultEventArgs)cardEventArgs).result;
                string answer = ((CardStateChangedShowResultEventArgs)cardEventArgs).answer;

                UserInputSubmitSelfAssessmentResponseEventArgs args = new UserInputSubmitSelfAssessmentResponseEventArgs(doknow, dontaskagain, result, answer);
                UserInputSubmit(this, args);
            }
        }
        /// <summary>
        /// Submits the deactivate card response.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-09-09</remarks>
        public void SubmitDeactivateCard()
        {
            if (learnlogic != null)
            {
                UserInputSubmitDeactivateCardEventArgs args = new UserInputSubmitDeactivateCardEventArgs();
                UserInputSubmit(this, args);
            }
        }
        /// <summary>
        /// Handles the KeyUp event of the buttonSelfAssessment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        private void buttonSelfAssessment_KeyUp(object sender, KeyEventArgs e)
        {
            //In self assessment mode, up & down arrow keys can be used to pro/demote
            if (e.KeyCode == Keys.Up)
                OnSelfAssesmentKeyUp(null);
            else if (e.KeyCode == Keys.Down)
                OnSelfAssesmentKeyDown(null);
        }

        #endregion selfAssesment

        #region buttonFocus
        public event MLifter.BusinessLayer.LearnLogic.CardStateChangedEventHandler SetButtonFocus;
        protected virtual void OnSetButtonFocus(CardStateChangedEventArgs e)
        {
            if (SetButtonFocus != null)
                SetButtonFocus(this, e);
        }
        #endregion buttonFocus


        #region learnlogic
        void learnlogic_LearnLogicIdle(object sender, EventArgs e)
        {

            TimeSpan ts = DateTime.Now - learnLogicWorkingStart;
            Debug.WriteLine(ts.TotalMilliseconds.ToString() + " ms", "Working time of LearnLogic");
        }

        void learnlogic_LearnLogicIsWorking(object sender, EventArgs e)
        {
            learnLogicWorkingStart = DateTime.Now;
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
            this.learnlogic.LearnLogicIsWorking += new EventHandler(learnlogic_LearnLogicIsWorking);
            this.learnlogic.LearnLogicIdle += new EventHandler(learnlogic_LearnLogicIdle);
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
            AnswerBrowserContent = "<html></html>";
        }

        /// <summary>
        /// Handles the CardStateChanged event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        void learnlogic_CardStateChanged(object sender, CardStateChangedEventArgs e)
        {
            Card card = e.dictionary.Cards.GetCardByID(e.cardid);

            //prepare title labels for either a new card, or the result in slideshow mode
            if (e is CardStateChangedNewCardEventArgs ||
                (e is CardStateChangedShowResultEventArgs && ((CardStateChangedShowResultEventArgs)e).slideshow))
            {
                string title = card != null ? card.CurrentAnswerCaption : string.Empty;
                if ((this.learnlogic == null) || (this.learnlogic.SlideShow == false))
                {
                    switch (e.dictionary.LearnMode)
                    {
                        case BusinessLayer.LearnModes.MultipleChoice:
                            title = String.Format(Resources.MAINFORM_LBLANSWER_MULTI_TEXT, title);
                            break;
                        case BusinessLayer.LearnModes.Sentence:
                            title = String.Format(Resources.MAINFORM_LBLANSWER_SENTENCES_TEXT, title);
                            break;
                        default:
                            if (card.CurrentAnswer.Words.Count > 1) //in case of synonyms
                                title = String.Format(Resources.MAINFORM_LBLANSWER_DEFAULT_TEXT2, title, card.CurrentAnswer.Words.Count);
                            else
                                title = String.Format(Resources.MAINFORM_LBLANSWER_DEFAULT_TEXT1, title);
                            break;
                    }
                }
                this.Title = title;
            }

            if (e is CardStateChangedNewCardEventArgs)
            {
                ComponentsHelper.IsResizing = true;

                //reset controls
                mLifterTextBox.Text = string.Empty;
                webBrowserAnswer.Stop();
                MLifter.Components.InfoBar.CleanUpInfobars(InfoBarControl);

                if (generateAnswerThread != null && generateAnswerThread.IsAlive)
                    generateAnswerThread.Abort();

                //fix for [ML-1335] Problems with bidirectional Text (RTL languages)
                RightToLeft controlTextDirection = (e.dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question ?
                    card.BaseCard.Question.Culture : card.BaseCard.Answer.Culture).TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
                mLifterTextBox.RightToLeft = multipleChoice.RightToLeft = controlTextDirection;

                //switch main multipane according to current learn mode
                if (e.dictionary.LearnMode == BusinessLayer.LearnModes.MultipleChoice)
                {
                    multiPaneControlMain.SelectedPage = multiPanePageMainMultipleChoice;
                    BusinessLayer.MultipleChoice multipleChoiceQuery = e.dictionary.GetChoices(card.BaseCard);
                    if (multipleChoiceQuery.Count > 0)
                    {
                        multipleChoice.Options = e.dictionary.CurrentMultipleChoiceOptions;
                        multipleChoice.Show(multipleChoiceQuery);
                        multipleChoice.Focus();
                    }
                }
                else
                {
                    multiPaneControlMain.SelectedPage = multiPanePageMainTextbox;
                    mLifterTextBox.IgnoreChars = e.dictionary.Settings.StripChars;
                    mLifterTextBox.CaseSensitive = e.dictionary.Settings.CaseSensitive.Value;
                    mLifterTextBox.CorrectOnTheFly = e.dictionary.Settings.CorrectOnTheFly.Value;

                    mLifterTextBox.Synonyms = (e.dictionary.LearnMode == BusinessLayer.LearnModes.Sentence ? card.CurrentAnswerExample : card.CurrentAnswer).ToStringList();

                    //show synonym notification infobar
                    if (mLifterTextBox.Synonyms.Count > 1 && learnlogic.SynonymInfoMessage)
                    {
                        string notificationText;
                        if (e.dictionary.Settings.CorrectOnTheFly.Value)
                            notificationText = Resources.SYNONYM_PROMPT_FLY_TEXT;
                        else
                            notificationText = Resources.SYNONYM_PROMPT_TEXT;
                        MLifter.Components.InfoBar infobar = new MLifter.Components.InfoBar(notificationText, this.InfoBarControl, DockStyle.Top, true, true, gradientPanelAnswer);
                        infobar.DontShowAgainChanged += new EventHandler(infobar_DontShowAgainChanged);
                    }
                    PositionTextBox();

                    mLifterTextBox.Focus();
                }

                ComponentsHelper.IsResizing = false;
                Invalidate();
            }
            else if (e is CardStateChangedShowResultEventArgs)
            {

                CardStateChangedShowResultEventArgs args = (CardStateChangedShowResultEventArgs)e;

                if (generateAnswerThread != null && generateAnswerThread.IsAlive)
                    generateAnswerThread.Abort();

                generateAnswerThread = new Thread(delegate()
                {
                    this.Invoke(new MethodInvoker(delegate()
                  {
                      this.AnswerBrowserUrl = new Uri("about:blank"); //[ML-1621]  Flicker in the answer result
                  }));

                    // WORKAROUND for Windows Media Player 6.4 [ML-2122]
                    if (MLifter.Generics.Methods.IsWMP7OrGreater())
                    {
                        Uri answer = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareAnswer(args.dictionary.DictionaryDAL.Parent, args.cardid,
                            args.dictionary.GenerateAnswer(args.cardid, args.answer, args.promoted));
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            this.AnswerBrowserUrl = answer;
                            multiPaneControlMain.SelectedPage = multiPanePageMainViewer;

                            if (!args.slideshow && e.dictionary.Settings.SelfAssessment.Value)
                                cardEventArgs = args;//this tag is needed for the submit of the answer

                            OnSetButtonFocus(e);
                        }));
                    }
                    else
                    {
                        string content = args.dictionary.GenerateAnswer(args.cardid, args.answer, args.promoted);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            this.AnswerBrowserContent = content;
                            multiPaneControlMain.SelectedPage = multiPanePageMainViewer;

                            if (!args.slideshow && e.dictionary.Settings.SelfAssessment.Value)
                                cardEventArgs = args;//this tag is needed for the submit of the answer

                            OnSetButtonFocus(e);
                        }));
                    }
                });

                generateAnswerThread.IsBackground = true;
                generateAnswerThread.Name = "Generate Answer Thread";
                generateAnswerThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                generateAnswerThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                generateAnswerThread.Start();


            }
            else if (e is CardStateChangedCountdownTimerEventArgs)
            {
                //submit answers in case the timer is finished
                CardStateChangedCountdownTimerEventArgs args = (CardStateChangedCountdownTimerEventArgs)e;
                if (args.TimerFinished)
                {
                    if (multiPaneControlMain.SelectedPage == multiPanePageMainTextbox)
                    {
                        mLifterTextBox.AllowAnswerSubmit = false;
                        mLifterTextBox.ManualOnKeyPress(new KeyPressEventArgs((char)Keys.Enter));
                        mLifterTextBox.AllowAnswerSubmit = true;

                        SubmitUserInput();
                    }
                    else if (multiPaneControlMain.SelectedPage == multiPanePageMainMultipleChoice)
                        SubmitMultipleChoice();
                    else if (learnlogic.SlideShow)
                        SubmitSlideShow(false);
                }
            }

        }
        #endregion learnlogic

        #region panelTextBox

        #region infobar
        /// <summary>
        /// Handles the DontShowAgainChanged event of the infobar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-09</remarks>
        void infobar_DontShowAgainChanged(object sender, EventArgs e)
        {
            if (learnlogic != null && sender is MLifter.Components.InfoBar)
                learnlogic.SynonymInfoMessage = !((MLifter.Components.InfoBar)sender).DontShowAgain;
        }
        #endregion infobar

        #region textBox
        /// <summary>
        /// Handles the FileDropped event of the mLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void mLifterTextBox_FileDropped(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                OnFileDropped(new FileDroppedEventArgs(file));
            }
        }
        /// <summary>
        /// Handles the Wrong event of the mLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        void mLifterTextBox_Wrong(object sender, EventArgs e)
        {
            SubmitUserInput();
        }

        /// <summary>
        /// Handles the Correct event of the mLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        void mLifterTextBox_Correct(object sender, EventArgs e)
        {
            SubmitUserInput();
        }
        #endregion textBox

        /// <summary>
        /// Handles the Resize event of the panelTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        void panelTextBox_Resize(object sender, EventArgs e)
        {
            if (multiPaneControlMain.SelectedPage == multiPanePageMainTextbox)
                PositionTextBox();
        }

        /// <summary>
        /// Handles the Resize event of the mLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        void mLifterTextBox_Resize(object sender, EventArgs e)
        {
            if (multiPaneControlMain.SelectedPage == multiPanePageMainTextbox)
                PositionTextBox();
        }

        /// <summary>
        /// Positions the text box.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        private void PositionTextBox()
        {
            mLifterTextBox.Top = Convert.ToInt32((panelTextBox.Height - mLifterTextBox.Height) / 2);
        }
        #endregion panelTextBox

        #region others

        /// <summary>
        /// Handles the Click event of the dockingButtonDontAskAgain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        private void dockingButtonDontAskAgain_Click(object sender, EventArgs e)
        {
            SubmitDeactivateCard();
            //SubmitSelfAssessmentReponse(false, true);
        }

        /// <summary>
        /// Focus the single control, when it is the only one on the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        private void FocusSingleControl(MultiPanePage page)
        {
            if (page != null && page.Controls.Count == 1)
                page.Controls[0].Focus();
        }
        /// <summary>
        /// Submits the slide show.
        /// </summary>
        /// <param name="stopslideshow">if set to <c>true</c> [stopslideshow].</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        public void SubmitSlideShow(bool stopslideshow)
        {
            if (learnlogic != null)
            {
                UserInputSubmitSlideshowEventArgs args = new UserInputSubmitSlideshowEventArgs(stopslideshow);
                UserInputSubmit(this, args);
            }
        }
        /// <summary>
        /// Gets a value indicating whether [result page visible].
        /// </summary>
        /// <value><c>true</c> if [result page visible]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2009-05-07</remarks>
        public bool ResultPageVisible
        {
            get { return multiPaneControlMain.SelectedPage == multiPanePageMainViewer; }
        }
        #endregion others

        #region multiPane
        /// <summary>
        /// Handles the SelectedPageChanged event of the multiPaneControlButtons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        private void multiPaneControlButtons_SelectedPageChanged(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Handles the SelectedPageChanged event of the multiPaneControlMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        private void multiPaneControlMain_SelectedPageChanged(object sender, EventArgs e)
        {
            if (multiPaneControlMain.SelectedPage != multiPanePageMainViewer)
                AnswerBrowserUrl = new Uri("about:blank");

            FocusSingleControl(multiPaneControlMain.SelectedPage);
        }


        /// <summary>
        /// Handles the SelectedPageChanging event of the multiPaneControlMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        private void multiPaneControlMain_SelectedPageChanging(object sender, EventArgs e)
        {
            MLifter.Components.InfoBar.CleanUpInfobars(multiPaneControlMain.SelectedPage);
        }
        #endregion multiPane

        #region multipleChoice
        /// <summary>
        /// Handles the ButtonKeyUp event of the multipleChoice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        void multipleChoice_ButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SubmitMultipleChoice();
        }
        /// <summary>
        /// Submits the multiple choice.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        private void SubmitMultipleChoice()
        {
            //[ML-2123] Null reference exception thrown when submitting MC answer 
            if (learnlogic != null && multipleChoice != null && multipleChoice.Choices != null)
            {
                string answer = multipleChoice.Choices.GetAnswers();
                UserInputSubmitMultipleChoiceEventArgs args = new UserInputSubmitMultipleChoiceEventArgs(answer, multipleChoice.Choices.GetResult());
                UserInputSubmit(this, args);
            }
        }
        #endregion multipleChoice

        #region answerBrowser
        /// <summary>
        /// Gets or sets the HTML content.
        /// </summary>
        /// <value>The content of the answer browser.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        private string AnswerBrowserContent
        {
            get
            {
                try { return webBrowserAnswer.DocumentText; }
                catch (Exception exp) { Trace.WriteLine(exp.ToString()); return string.Empty; }
            }
            set
            {
                try { webBrowserAnswer.Stop(); webBrowserAnswer.DocumentText = value; }
                catch (Exception exp) { Trace.WriteLine("Error setting DocumentText: " + exp.ToString()); }
            }
        }

        /// <summary>
        /// Gets or sets the Uri.
        /// </summary>
        /// <value>The Uri.</value>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        private Uri AnswerBrowserUrl
        {
            get { return webBrowserAnswer.Url; }
            set { webBrowserAnswer.Stop(); webBrowserAnswer.Url = value; }
        }

        #endregion answerBrowser

        #region logic
        /// <summary>
        /// Checks the answer.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void CheckAnswer()
        {
            if (learnlogic.Dictionary.LearnMode == MLifter.BusinessLayer.LearnModes.MultipleChoice)
                SubmitMultipleChoice();
            else
            {
                this.mLifterTextBox.AllowAnswerSubmit = false;
                this.mLifterTextBox.ManualOnKeyPress(new KeyPressEventArgs((char)Keys.Enter));      //this simults a "pressing ENTER"
                this.mLifterTextBox.AllowAnswerSubmit = true;
                SubmitUserInput();
            }

        }
        /// <summary>
        /// Shows the next card, depending on slideshow mode or word mode
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void ShowNextCard()
        {
            if (learnlogic == null)
                return;

            if (learnlogic.SlideShow == true)
            {
                SubmitSlideShow(false);
                return;
            }
            UserInputSubmit(this, new UserInputSubmitEventArgs());
        }
        /// <summary>
        /// Submits the user input.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-23</remarks>
        void SubmitUserInput()
        {
            if (learnlogic != null)
            {
                UserInputSubmitTextEventArgs args = new UserInputSubmitTextEventArgs(
                    mLifterTextBox.Errors,
                    mLifterTextBox.CorrectSynonyms,
                    mLifterTextBox.Synonyms.Count,
                    mLifterTextBox.CorrectFirstSynonym,
                    mLifterTextBox.Text);

                UserInputSubmit(this, args);
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
        #endregion logic

        #region browserControl
        /// <summary>
        /// Handles the Navigating event of the webBrowserAnswer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void webBrowserAnswer_Navigating(object sender, WebBrowserNavigatingEventArgs e)
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
        /// Handles the PreviewKeyDown event of the webBrowserAnswer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        private void webBrowserAnswer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (BrowserKeyDown != null)
                BrowserKeyDown(sender, e);
        }
        #endregion browserControl


    }
}
