using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using MLifter.Properties;

namespace MLifter
{
    /// <summary>
    /// Summary Description for LernOptions.
    /// </summary>
    public class QueryOptionsForm : System.Windows.Forms.Form
    {
        #region Fields

        private System.Windows.Forms.TabControl PCOptions;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabPage TSTeachers;
        private System.Windows.Forms.TabPage TSLearning;
        private System.Windows.Forms.TabPage TSSynonyms;
        private System.Windows.Forms.TabPage TSTyping;
        private System.Windows.Forms.TabPage TSPlanning;
        private System.Windows.Forms.TabPage TSGeneral;
        private System.Windows.Forms.GroupBox GBTeacher;
        private System.Windows.Forms.Label LTeachers;
        private System.Windows.Forms.CheckBox CBCorrect;
        private System.Windows.Forms.CheckBox CBCase;
        private System.Windows.Forms.CheckBox CBSelfAssessment;
        private System.Windows.Forms.Label LIgnore;
        private System.Windows.Forms.TextBox EStrip;
        private System.Windows.Forms.GroupBox GBEndSession;
        private System.Windows.Forms.CheckBox CBTime;
        private System.Windows.Forms.CheckBox CBCards;
        private System.Windows.Forms.CheckBox CBRights;
        private System.Windows.Forms.CheckBox CBSnooze;
        private System.Windows.Forms.CheckBox CBQuit;
        private System.Windows.Forms.NumericUpDown SETime;
        private System.Windows.Forms.NumericUpDown SECards;
        private System.Windows.Forms.NumericUpDown SERights;
        private System.Windows.Forms.NumericUpDown SEMin1;
        private System.Windows.Forms.NumericUpDown SEMin2;
        private System.Windows.Forms.Label LblTo;
        private System.Windows.Forms.Label LblMin;
        private System.Windows.Forms.GroupBox GBTyping;
        private System.Windows.Forms.RadioButton RGTyping0;
        private System.Windows.Forms.RadioButton RGTyping1;
        private System.Windows.Forms.RadioButton RGTyping2;
        private System.Windows.Forms.RadioButton RGTyping3;
        private System.Windows.Forms.GroupBox GBSynonyms;
        private System.Windows.Forms.RadioButton RGSynonym4;
        private System.Windows.Forms.RadioButton RGSynonym2;
        private System.Windows.Forms.RadioButton RGSynonym1;
        private System.Windows.Forms.RadioButton RGSynonym0;
        private System.Windows.Forms.RadioButton RGSynonym3;
        private System.Windows.Forms.GroupBox GBGeneral;
        private System.Windows.Forms.Button[] btnTeacher;
        private System.Windows.Forms.CheckBox CBGeneral7;
        private System.Windows.Forms.CheckBox CBGeneral6;
        private System.Windows.Forms.CheckBox CBGeneral5;
        private System.Windows.Forms.CheckBox CBGeneral4;
        private System.Windows.Forms.CheckBox CBGeneral3;
        private System.Windows.Forms.CheckBox CBGeneral2;
        private System.Windows.Forms.CheckBox CBGeneral1;
        private System.Windows.Forms.CheckBox CBGeneral0;

        private const int MaxTeacher = 4;
        private CheckBox checkBoxOneInstance;
        private MLifter.Controls.LearnModes learnModes;
        private Label labelTeacherLoadingError;
        private PictureBox pictureBoxTextInfo;
        private Label labelTextInfo;
        private HelpProvider MainHelp;
        private CheckBox checkBoxUseDictionaryStylesheets;
        private CheckBox checkBoxStartExitSound;
        private PictureBox pictureBoxTeacherHelp;
        private CheckBox checkBoxCheckForBetaUpdates;
        Teacher teacher;

        #endregion

        #region Main functions
        /// <summary>
        /// Creates teachers and form
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        public QueryOptionsForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);

            learnModes.QuestionCaption = Dictionary.QuestionCaption;
            learnModes.AnswerCaption = Dictionary.AnswerCaption;
            learnModes.SetAllowedQueryTypes(Dictionary.AllowedQueryTypes);
            learnModes.SetAllowedQueryDirections(Dictionary.AllowedQueryDirections);
            checkBoxOneInstance.Checked = Settings.Default.AllowOnlyOneInstance;
            checkBoxStartExitSound.Checked = Settings.Default.Play;
            checkBoxCheckForBetaUpdates.Checked = Settings.Default.CheckForBetaUpdates;
            checkBoxUseDictionaryStylesheets.Checked = Dictionary.UseDictionaryStyleSheets;

            //force multiple choice options to be visible
            learnModes.MultipleChoiceOptionsVisible = true;

            //load the current options
            LoadOptions(Dictionary.Settings);

            //load teachers
            try
            {
                if (Path.IsPathRooted(Properties.Settings.Default.Teacher))
                    teacher = new Teacher(Properties.Settings.Default.Teacher);
                else
                    teacher = new Teacher(Path.Combine(System.Windows.Forms.Application.StartupPath, Properties.Settings.Default.Teacher));
                if (teacher.Presets.Presets.Count < 1)
                    throw new Exception(Resources.LEARN_OPTIONS_NOTEACHERSFOUND);
            }
            catch (Exception e)
            {
                labelTeacherLoadingError.Text = string.Format(Resources.LEARN_OPTIONS_TEACHERSNOTLOADED, e.Message, e.InnerException != null ? e.InnerException.Message : string.Empty);
                labelTeacherLoadingError.Tag = e;
                labelTeacherLoadingError.Visible = true;
            }
            int i = 0;
            btnTeacher = new Button[teacher.Presets.Presets.Count];
            foreach (IPreset preset in teacher.Presets.Presets)
            {
                btnTeacher[i] = new Button();
                GBTeacher.Controls.Add(btnTeacher[i]);
                btnTeacher[i].Text = Resources.ResourceManager.GetString(preset.ResourceId);
                btnTeacher[i].Font = new Font(btnTeacher[i].Font.FontFamily, (int)(btnTeacher[i].Font.Size * 1.4));
                btnTeacher[i].Left = 16;
                btnTeacher[i].Width = GBTeacher.Width - 2 * btnTeacher[i].Left;
                btnTeacher[i].Height = (GBTeacher.Height - 40) / teacher.Presets.Presets.Count - 4;
                btnTeacher[i].Top = i * (btnTeacher[i].Height + 4) + 20;
                btnTeacher[i].Tag = preset;
                btnTeacher[i].Click += new System.EventHandler(this.LoadTeacher);
                i++;
            }

        }

        /// <summary>
        /// Applys the supplied options to the form controls.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <remarks>Documented by Dev02, 2008-01-17</remarks>
        private void LoadOptions(ISettings options)
        {
            if (options.QueryDirections.Answer2Question.Value)
                learnModes.QueryDirection = EQueryDirection.Answer2Question;
            else if (options.QueryDirections.Question2Answer.Value)
                learnModes.QueryDirection = EQueryDirection.Question2Answer;
            else
                learnModes.QueryDirection = EQueryDirection.Mixed;

            int index;
            if (options.GradeSynonyms.AllKnown.Value)
                index = 0;
            else if (options.GradeSynonyms.HalfKnown.Value)
                index = 1;
            else if (options.GradeSynonyms.OneKnown.Value)
                index = 2;
            else if (options.GradeSynonyms.FirstKnown.Value)
                index = 3;
            else
                index = 4;
            RGSynonym_CheckItem(index);

            if (options.GradeTyping.AllCorrect.GetValueOrDefault())
                index = 0;
            else if (options.GradeTyping.HalfCorrect.GetValueOrDefault())
                index = 1;
            else if (options.GradeTyping.NoneCorrect.GetValueOrDefault())
                index = 2;
            else
                index = 3;
            RGTyping_CheckItem(index);
            EStrip.Text = options.StripChars;

            learnModes.SetQueryTypes(options.QueryTypes);

            learnModes.SetQueryMultipleChoiceOptions(options.MultipleChoiceOptions);

            CBCase.Checked = options.CaseSensitive.Value;

            CBGeneral0.Checked = options.EnableTimer.Value;
            CBGeneral1.Checked = options.ShowStatistics.Value;
            CBGeneral2.Checked = options.ShowImages.Value;
            CBGeneral3.Checked = options.AutoplayAudio.Value;
            CBGeneral4.Checked = options.EnableCommentary.Value;
            CBGeneral5.Checked = options.SkipCorrectAnswers.Value;
            CBGeneral6.Checked = options.RandomPool.Value;
            CBGeneral7.Checked = options.ConfirmDemote.Value;

            CBCorrect.Checked = options.CorrectOnTheFly.Value;
            CBCorrect_CheckedChanged(null, null);
            CBSnooze.Checked = options.SnoozeOptions.SnoozeMode == ESnoozeMode.SendToTray;
            CBQuit.Checked = options.SnoozeOptions.SnoozeMode == ESnoozeMode.QuitProgram;
            CBSelfAssessment.Checked = options.SelfAssessment.Value;

            CBTime.Checked = options.SnoozeOptions.IsTimeEnabled.GetValueOrDefault();
            CBCards.Checked = options.SnoozeOptions.IsCardsEnabled.GetValueOrDefault();
            CBRights.Checked = options.SnoozeOptions.IsRightsEnabled.GetValueOrDefault();

            int temp = 0;
            if (options.SnoozeOptions.SnoozeLow <= options.SnoozeOptions.SnoozeHigh)
            {
                temp = options.SnoozeOptions.SnoozeLow.GetValueOrDefault();
                SEMin1.Maximum = SEMin2.Maximum;
                SEMin2.Minimum = SEMin1.Minimum;
                SEMin1.Value = (temp < SEMin1.Minimum || temp > SEMin1.Maximum) ? 55 : temp;

                temp = options.SnoozeOptions.SnoozeHigh.GetValueOrDefault();
                SEMin2.Value = (temp < SEMin2.Minimum || temp > SEMin2.Maximum) ? 110 : temp;
            }

            temp = options.SnoozeOptions.SnoozeTime.GetValueOrDefault();
            SETime.Value = (temp < SETime.Minimum || temp > SETime.Maximum) ? 10 : temp;

            temp = options.SnoozeOptions.SnoozeCards.GetValueOrDefault();
            SECards.Value = (temp < SECards.Minimum || temp > SECards.Maximum) ? 10 : temp;

            temp = options.SnoozeOptions.SnoozeRights.GetValueOrDefault();
            SERights.Value = (temp < SERights.Minimum || temp > SERights.Maximum) ? 5 : temp;
            DoNotEnd();
        }
        /// <summary>
        /// Shows the learning modes tab.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-24</remarks>
        public void showLearningModesTab()
        {
            PCOptions.SelectedIndex = 1;
        }
        /// <summary>
        /// Loads selected teacher
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void LoadTeacher(object sender, System.EventArgs e)
        {
            if (sender is Button && (sender as Button).Tag is IPreset)
            {
                IPreset selectedPreset = (IPreset)((Button)sender).Tag;
                LoadOptions(selectedPreset.Preset);
            }
        }

        /// <summary>
        /// Saving of querytypes, queryoptions, times
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void btnOkay_Click(object sender, System.EventArgs e)
        {
            if (!learnModes.ValidateInput())
                return;

            Dictionary.Settings.GradeSynonyms.AllKnown = false;
            Dictionary.Settings.GradeSynonyms.Prompt = false;
            Dictionary.Settings.GradeSynonyms.FirstKnown = false;
            Dictionary.Settings.GradeSynonyms.OneKnown = false;
            Dictionary.Settings.GradeSynonyms.HalfKnown = false;
            switch (RGSynonym_GetValue())
            {
                case EGradeSynonyms.AllKnown:
                    Dictionary.Settings.GradeSynonyms.AllKnown = true;
                    break;
                case EGradeSynonyms.HalfKnown:
                    Dictionary.Settings.GradeSynonyms.HalfKnown = true;
                    break;
                case EGradeSynonyms.OneKnown:
                    Dictionary.Settings.GradeSynonyms.OneKnown = true;
                    break;
                case EGradeSynonyms.FirstKnown:
                    Dictionary.Settings.GradeSynonyms.FirstKnown = true;
                    break;
                case EGradeSynonyms.Prompt:
                    Dictionary.Settings.GradeSynonyms.Prompt = true;
                    break;
            }


            Dictionary.Settings.GradeTyping.AllCorrect = false;
            Dictionary.Settings.GradeTyping.HalfCorrect = false;
            Dictionary.Settings.GradeTyping.NoneCorrect = false;
            Dictionary.Settings.GradeTyping.Prompt = false;
            switch (RGTyping_GetValue())
            {
                case EGradeTyping.AllCorrect:
                    Dictionary.Settings.GradeTyping.AllCorrect = true;
                    break;
                case EGradeTyping.HalfCorrect:
                    Dictionary.Settings.GradeTyping.HalfCorrect = true;
                    break;
                case EGradeTyping.NoneCorrect:
                    Dictionary.Settings.GradeTyping.NoneCorrect = true;
                    break;
                case EGradeTyping.Prompt:
                    Dictionary.Settings.GradeTyping.Prompt = true;
                    break;
            }

            Dictionary.Settings.StripChars = EStrip.Text;

            Settings.Default.AllowOnlyOneInstance = checkBoxOneInstance.Checked;
            Settings.Default.Play = checkBoxStartExitSound.Checked;
            Settings.Default.CheckForBetaUpdates = checkBoxCheckForBetaUpdates.Checked;

            Settings.Default.Save();

            if (Settings.Default.AllowOnlyOneInstance && !MLifter.Program.IPCrunning)
                //TODO: also check the firstInstance Mutex here
                try
                {
                    MLifter.Program.StartIPC();
                }
                catch (Exception exp)
                {
                    System.Diagnostics.Trace.WriteLine("Failed to start IPC server after QueryOptionsForm: " + exp.ToString());
                }

            // saving querytype -----------------------------------------------------------------------
            Dictionary.Settings.QueryDirections.Question2Answer = false;
            Dictionary.Settings.QueryDirections.Answer2Question = false;
            Dictionary.Settings.QueryDirections.Mixed = false;
            switch (learnModes.QueryDirection)
            {
                case EQueryDirection.Question2Answer:
                    Dictionary.Settings.QueryDirections.Question2Answer = true;
                    break;
                case EQueryDirection.Answer2Question:
                    Dictionary.Settings.QueryDirections.Answer2Question = true;
                    break;
                case EQueryDirection.Mixed:
                    Dictionary.Settings.QueryDirections.Mixed = true;
                    break;
            }

            IQueryType queryType = Dictionary.Settings.QueryTypes;
            learnModes.GetQueryTypes(ref queryType);

            IQueryMultipleChoiceOptions queryMultipleChoiceOptions = Dictionary.MultipleChoiceOptions;
            learnModes.GetQueryMultipleChoiceOptions(ref queryMultipleChoiceOptions);

            if (CBTime.Checked) Dictionary.Settings.SnoozeOptions.EnableTime((int)SETime.Value);
            else Dictionary.Settings.SnoozeOptions.DisableTime();
            if (CBCards.Checked) Dictionary.Settings.SnoozeOptions.EnableCards((int)SECards.Value);
            else Dictionary.Settings.SnoozeOptions.DisableCards();
            if (CBRights.Checked) Dictionary.Settings.SnoozeOptions.EnableRights((int)SERights.Value);
            else Dictionary.Settings.SnoozeOptions.DisableRights();

            // saving queryoptions --------------------------------------------------------------------
            Dictionary.Settings.EnableTimer = CBGeneral0.Checked;
            Dictionary.Settings.ShowStatistics = CBGeneral1.Checked;
            Dictionary.Settings.ShowImages = CBGeneral2.Checked;
            Dictionary.Settings.AutoplayAudio = CBGeneral3.Checked;
            Dictionary.Settings.EnableCommentary = CBGeneral4.Checked;
            Dictionary.Settings.SkipCorrectAnswers = CBGeneral5.Checked;
            Dictionary.Settings.RandomPool = CBGeneral6.Checked;
            Dictionary.Settings.ConfirmDemote = CBGeneral7.Checked;
            Dictionary.UseDictionaryStyleSheets = checkBoxUseDictionaryStylesheets.Checked;

            Dictionary.Settings.CorrectOnTheFly = CBCorrect.Checked;
            if (CBSnooze.Checked) Dictionary.Settings.SnoozeOptions.SnoozeMode = ESnoozeMode.SendToTray;
            if (CBQuit.Checked) Dictionary.Settings.SnoozeOptions.SnoozeMode = ESnoozeMode.QuitProgram;
            Dictionary.Settings.SelfAssessment = CBSelfAssessment.Checked;
            Dictionary.Settings.CaseSensitive = CBCase.Checked;

            // saving times ----------------------------------------------------------------------------
            if (SEMin1.Value > SEMin2.Value)
                SEMin1.Value = SEMin2.Value;
            Dictionary.Settings.SnoozeOptions.SetSnoozeTimes((int)SEMin1.Value, (int)SEMin2.Value);

            Dictionary.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryOptionsForm));
			this.PCOptions = new System.Windows.Forms.TabControl();
			this.TSTeachers = new System.Windows.Forms.TabPage();
			this.pictureBoxTeacherHelp = new System.Windows.Forms.PictureBox();
			this.LTeachers = new System.Windows.Forms.Label();
			this.GBTeacher = new System.Windows.Forms.GroupBox();
			this.labelTeacherLoadingError = new System.Windows.Forms.Label();
			this.TSLearning = new System.Windows.Forms.TabPage();
			this.learnModes = new MLifter.Controls.LearnModes();
			this.TSSynonyms = new System.Windows.Forms.TabPage();
			this.GBSynonyms = new System.Windows.Forms.GroupBox();
			this.pictureBoxTextInfo = new System.Windows.Forms.PictureBox();
			this.labelTextInfo = new System.Windows.Forms.Label();
			this.RGSynonym3 = new System.Windows.Forms.RadioButton();
			this.RGSynonym4 = new System.Windows.Forms.RadioButton();
			this.RGSynonym2 = new System.Windows.Forms.RadioButton();
			this.RGSynonym1 = new System.Windows.Forms.RadioButton();
			this.RGSynonym0 = new System.Windows.Forms.RadioButton();
			this.TSTyping = new System.Windows.Forms.TabPage();
			this.EStrip = new System.Windows.Forms.TextBox();
			this.LIgnore = new System.Windows.Forms.Label();
			this.CBSelfAssessment = new System.Windows.Forms.CheckBox();
			this.CBCase = new System.Windows.Forms.CheckBox();
			this.CBCorrect = new System.Windows.Forms.CheckBox();
			this.GBTyping = new System.Windows.Forms.GroupBox();
			this.RGTyping3 = new System.Windows.Forms.RadioButton();
			this.RGTyping2 = new System.Windows.Forms.RadioButton();
			this.RGTyping1 = new System.Windows.Forms.RadioButton();
			this.RGTyping0 = new System.Windows.Forms.RadioButton();
			this.TSPlanning = new System.Windows.Forms.TabPage();
			this.GBEndSession = new System.Windows.Forms.GroupBox();
			this.LblMin = new System.Windows.Forms.Label();
			this.LblTo = new System.Windows.Forms.Label();
			this.SEMin2 = new System.Windows.Forms.NumericUpDown();
			this.SEMin1 = new System.Windows.Forms.NumericUpDown();
			this.SERights = new System.Windows.Forms.NumericUpDown();
			this.SECards = new System.Windows.Forms.NumericUpDown();
			this.SETime = new System.Windows.Forms.NumericUpDown();
			this.CBQuit = new System.Windows.Forms.CheckBox();
			this.CBSnooze = new System.Windows.Forms.CheckBox();
			this.CBRights = new System.Windows.Forms.CheckBox();
			this.CBCards = new System.Windows.Forms.CheckBox();
			this.CBTime = new System.Windows.Forms.CheckBox();
			this.TSGeneral = new System.Windows.Forms.TabPage();
			this.GBGeneral = new System.Windows.Forms.GroupBox();
			this.checkBoxCheckForBetaUpdates = new System.Windows.Forms.CheckBox();
			this.checkBoxUseDictionaryStylesheets = new System.Windows.Forms.CheckBox();
			this.checkBoxStartExitSound = new System.Windows.Forms.CheckBox();
			this.checkBoxOneInstance = new System.Windows.Forms.CheckBox();
			this.CBGeneral3 = new System.Windows.Forms.CheckBox();
			this.CBGeneral2 = new System.Windows.Forms.CheckBox();
			this.CBGeneral1 = new System.Windows.Forms.CheckBox();
			this.CBGeneral0 = new System.Windows.Forms.CheckBox();
			this.CBGeneral7 = new System.Windows.Forms.CheckBox();
			this.CBGeneral6 = new System.Windows.Forms.CheckBox();
			this.CBGeneral5 = new System.Windows.Forms.CheckBox();
			this.CBGeneral4 = new System.Windows.Forms.CheckBox();
			this.btnOkay = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.MainHelp = new System.Windows.Forms.HelpProvider();
			this.PCOptions.SuspendLayout();
			this.TSTeachers.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTeacherHelp)).BeginInit();
			this.GBTeacher.SuspendLayout();
			this.TSLearning.SuspendLayout();
			this.TSSynonyms.SuspendLayout();
			this.GBSynonyms.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTextInfo)).BeginInit();
			this.TSTyping.SuspendLayout();
			this.GBTyping.SuspendLayout();
			this.TSPlanning.SuspendLayout();
			this.GBEndSession.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SEMin2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SEMin1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SERights)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SECards)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SETime)).BeginInit();
			this.TSGeneral.SuspendLayout();
			this.GBGeneral.SuspendLayout();
			this.SuspendLayout();
			// 
			// PCOptions
			// 
			this.PCOptions.Controls.Add(this.TSTeachers);
			this.PCOptions.Controls.Add(this.TSLearning);
			this.PCOptions.Controls.Add(this.TSSynonyms);
			this.PCOptions.Controls.Add(this.TSTyping);
			this.PCOptions.Controls.Add(this.TSPlanning);
			this.PCOptions.Controls.Add(this.TSGeneral);
			this.MainHelp.SetHelpNavigator(this.PCOptions, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("PCOptions.HelpNavigator"))));
			resources.ApplyResources(this.PCOptions, "PCOptions");
			this.PCOptions.Name = "PCOptions";
			this.PCOptions.SelectedIndex = 0;
			this.MainHelp.SetShowHelp(this.PCOptions, ((bool)(resources.GetObject("PCOptions.ShowHelp"))));
			// 
			// TSTeachers
			// 
			this.TSTeachers.Controls.Add(this.pictureBoxTeacherHelp);
			this.TSTeachers.Controls.Add(this.LTeachers);
			this.TSTeachers.Controls.Add(this.GBTeacher);
			this.MainHelp.SetHelpKeyword(this.TSTeachers, resources.GetString("TSTeachers.HelpKeyword"));
			this.MainHelp.SetHelpNavigator(this.TSTeachers, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("TSTeachers.HelpNavigator"))));
			resources.ApplyResources(this.TSTeachers, "TSTeachers");
			this.TSTeachers.Name = "TSTeachers";
			this.MainHelp.SetShowHelp(this.TSTeachers, ((bool)(resources.GetObject("TSTeachers.ShowHelp"))));
			this.TSTeachers.UseVisualStyleBackColor = true;
			// 
			// pictureBoxTeacherHelp
			// 
			this.pictureBoxTeacherHelp.Cursor = System.Windows.Forms.Cursors.Hand;
			resources.ApplyResources(this.pictureBoxTeacherHelp, "pictureBoxTeacherHelp");
			this.pictureBoxTeacherHelp.Name = "pictureBoxTeacherHelp";
			this.MainHelp.SetShowHelp(this.pictureBoxTeacherHelp, ((bool)(resources.GetObject("pictureBoxTeacherHelp.ShowHelp"))));
			this.pictureBoxTeacherHelp.TabStop = false;
			this.pictureBoxTeacherHelp.Click += new System.EventHandler(this.pictureBoxTeacherHelp_Click);
			// 
			// LTeachers
			// 
			resources.ApplyResources(this.LTeachers, "LTeachers");
			this.LTeachers.Name = "LTeachers";
			// 
			// GBTeacher
			// 
			this.GBTeacher.Controls.Add(this.labelTeacherLoadingError);
			resources.ApplyResources(this.GBTeacher, "GBTeacher");
			this.GBTeacher.Name = "GBTeacher";
			this.GBTeacher.TabStop = false;
			// 
			// labelTeacherLoadingError
			// 
			this.labelTeacherLoadingError.ForeColor = System.Drawing.Color.Red;
			resources.ApplyResources(this.labelTeacherLoadingError, "labelTeacherLoadingError");
			this.labelTeacherLoadingError.Name = "labelTeacherLoadingError";
			this.labelTeacherLoadingError.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.labelTeacherLoadingError_MouseDoubleClick);
			// 
			// TSLearning
			// 
			this.TSLearning.Controls.Add(this.learnModes);
			resources.ApplyResources(this.TSLearning, "TSLearning");
			this.TSLearning.Name = "TSLearning";
			this.TSLearning.UseVisualStyleBackColor = true;
			// 
			// learnModes
			// 
			this.learnModes.AnswerCaption = "Answer";
			this.learnModes.EditableControlsEnabled = true;
			resources.ApplyResources(this.learnModes, "learnModes");
			this.learnModes.MultipleChoiceOptionsVisible = false;
			this.learnModes.MultipleDirections = false;
			this.learnModes.Name = "learnModes";
			this.learnModes.QueryDirection = MLifter.DAL.Interfaces.EQueryDirection.Question2Answer;
			this.learnModes.QuestionCaption = "Question";
			// 
			// TSSynonyms
			// 
			this.TSSynonyms.Controls.Add(this.GBSynonyms);
			resources.ApplyResources(this.TSSynonyms, "TSSynonyms");
			this.TSSynonyms.Name = "TSSynonyms";
			this.TSSynonyms.UseVisualStyleBackColor = true;
			// 
			// GBSynonyms
			// 
			this.GBSynonyms.Controls.Add(this.pictureBoxTextInfo);
			this.GBSynonyms.Controls.Add(this.labelTextInfo);
			this.GBSynonyms.Controls.Add(this.RGSynonym3);
			this.GBSynonyms.Controls.Add(this.RGSynonym4);
			this.GBSynonyms.Controls.Add(this.RGSynonym2);
			this.GBSynonyms.Controls.Add(this.RGSynonym1);
			this.GBSynonyms.Controls.Add(this.RGSynonym0);
			resources.ApplyResources(this.GBSynonyms, "GBSynonyms");
			this.GBSynonyms.Name = "GBSynonyms";
			this.GBSynonyms.TabStop = false;
			// 
			// pictureBoxTextInfo
			// 
			resources.ApplyResources(this.pictureBoxTextInfo, "pictureBoxTextInfo");
			this.pictureBoxTextInfo.Name = "pictureBoxTextInfo";
			this.pictureBoxTextInfo.TabStop = false;
			// 
			// labelTextInfo
			// 
			resources.ApplyResources(this.labelTextInfo, "labelTextInfo");
			this.labelTextInfo.Name = "labelTextInfo";
			// 
			// RGSynonym3
			// 
			resources.ApplyResources(this.RGSynonym3, "RGSynonym3");
			this.RGSynonym3.Name = "RGSynonym3";
			// 
			// RGSynonym4
			// 
			resources.ApplyResources(this.RGSynonym4, "RGSynonym4");
			this.RGSynonym4.Name = "RGSynonym4";
			// 
			// RGSynonym2
			// 
			resources.ApplyResources(this.RGSynonym2, "RGSynonym2");
			this.RGSynonym2.Name = "RGSynonym2";
			// 
			// RGSynonym1
			// 
			resources.ApplyResources(this.RGSynonym1, "RGSynonym1");
			this.RGSynonym1.Name = "RGSynonym1";
			// 
			// RGSynonym0
			// 
			resources.ApplyResources(this.RGSynonym0, "RGSynonym0");
			this.RGSynonym0.Name = "RGSynonym0";
			// 
			// TSTyping
			// 
			this.TSTyping.Controls.Add(this.EStrip);
			this.TSTyping.Controls.Add(this.LIgnore);
			this.TSTyping.Controls.Add(this.CBSelfAssessment);
			this.TSTyping.Controls.Add(this.CBCase);
			this.TSTyping.Controls.Add(this.CBCorrect);
			this.TSTyping.Controls.Add(this.GBTyping);
			resources.ApplyResources(this.TSTyping, "TSTyping");
			this.TSTyping.Name = "TSTyping";
			this.TSTyping.UseVisualStyleBackColor = true;
			// 
			// EStrip
			// 
			resources.ApplyResources(this.EStrip, "EStrip");
			this.EStrip.Name = "EStrip";
			// 
			// LIgnore
			// 
			resources.ApplyResources(this.LIgnore, "LIgnore");
			this.LIgnore.Name = "LIgnore";
			// 
			// CBSelfAssessment
			// 
			resources.ApplyResources(this.CBSelfAssessment, "CBSelfAssessment");
			this.CBSelfAssessment.Name = "CBSelfAssessment";
			this.CBSelfAssessment.CheckedChanged += new System.EventHandler(this.CBSelfAssessment_CheckedChanged);
			// 
			// CBCase
			// 
			resources.ApplyResources(this.CBCase, "CBCase");
			this.CBCase.Name = "CBCase";
			// 
			// CBCorrect
			// 
			resources.ApplyResources(this.CBCorrect, "CBCorrect");
			this.CBCorrect.Name = "CBCorrect";
			this.CBCorrect.CheckedChanged += new System.EventHandler(this.CBCorrect_CheckedChanged);
			// 
			// GBTyping
			// 
			this.GBTyping.Controls.Add(this.RGTyping3);
			this.GBTyping.Controls.Add(this.RGTyping2);
			this.GBTyping.Controls.Add(this.RGTyping1);
			this.GBTyping.Controls.Add(this.RGTyping0);
			resources.ApplyResources(this.GBTyping, "GBTyping");
			this.GBTyping.Name = "GBTyping";
			this.GBTyping.TabStop = false;
			// 
			// RGTyping3
			// 
			resources.ApplyResources(this.RGTyping3, "RGTyping3");
			this.RGTyping3.Name = "RGTyping3";
			// 
			// RGTyping2
			// 
			resources.ApplyResources(this.RGTyping2, "RGTyping2");
			this.RGTyping2.Name = "RGTyping2";
			// 
			// RGTyping1
			// 
			resources.ApplyResources(this.RGTyping1, "RGTyping1");
			this.RGTyping1.Name = "RGTyping1";
			// 
			// RGTyping0
			// 
			resources.ApplyResources(this.RGTyping0, "RGTyping0");
			this.RGTyping0.Name = "RGTyping0";
			// 
			// TSPlanning
			// 
			this.TSPlanning.Controls.Add(this.GBEndSession);
			resources.ApplyResources(this.TSPlanning, "TSPlanning");
			this.TSPlanning.Name = "TSPlanning";
			this.TSPlanning.UseVisualStyleBackColor = true;
			// 
			// GBEndSession
			// 
			this.GBEndSession.Controls.Add(this.LblMin);
			this.GBEndSession.Controls.Add(this.LblTo);
			this.GBEndSession.Controls.Add(this.SEMin2);
			this.GBEndSession.Controls.Add(this.SEMin1);
			this.GBEndSession.Controls.Add(this.SERights);
			this.GBEndSession.Controls.Add(this.SECards);
			this.GBEndSession.Controls.Add(this.SETime);
			this.GBEndSession.Controls.Add(this.CBQuit);
			this.GBEndSession.Controls.Add(this.CBSnooze);
			this.GBEndSession.Controls.Add(this.CBRights);
			this.GBEndSession.Controls.Add(this.CBCards);
			this.GBEndSession.Controls.Add(this.CBTime);
			resources.ApplyResources(this.GBEndSession, "GBEndSession");
			this.GBEndSession.Name = "GBEndSession";
			this.GBEndSession.TabStop = false;
			// 
			// LblMin
			// 
			resources.ApplyResources(this.LblMin, "LblMin");
			this.LblMin.Name = "LblMin";
			// 
			// LblTo
			// 
			resources.ApplyResources(this.LblTo, "LblTo");
			this.LblTo.Name = "LblTo";
			// 
			// SEMin2
			// 
			resources.ApplyResources(this.SEMin2, "SEMin2");
			this.SEMin2.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
			this.SEMin2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SEMin2.Name = "SEMin2";
			this.SEMin2.Value = new decimal(new int[] {
            110,
            0,
            0,
            0});
			this.SEMin2.ValueChanged += new System.EventHandler(this.SEMin2_ValueChanged);
			// 
			// SEMin1
			// 
			resources.ApplyResources(this.SEMin1, "SEMin1");
			this.SEMin1.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
			this.SEMin1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SEMin1.Name = "SEMin1";
			this.SEMin1.Value = new decimal(new int[] {
            110,
            0,
            0,
            0});
			this.SEMin1.ValueChanged += new System.EventHandler(this.SEMin1_ValueChanged);
			// 
			// SERights
			// 
			resources.ApplyResources(this.SERights, "SERights");
			this.SERights.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
			this.SERights.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SERights.Name = "SERights";
			this.SERights.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// SECards
			// 
			resources.ApplyResources(this.SECards, "SECards");
			this.SECards.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
			this.SECards.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SECards.Name = "SECards";
			this.SECards.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			// 
			// SETime
			// 
			resources.ApplyResources(this.SETime, "SETime");
			this.SETime.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
			this.SETime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SETime.Name = "SETime";
			this.SETime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// CBQuit
			// 
			resources.ApplyResources(this.CBQuit, "CBQuit");
			this.CBQuit.Name = "CBQuit";
			this.CBQuit.CheckedChanged += new System.EventHandler(this.CBQuit_CheckedChanged);
			// 
			// CBSnooze
			// 
			resources.ApplyResources(this.CBSnooze, "CBSnooze");
			this.CBSnooze.Name = "CBSnooze";
			this.CBSnooze.CheckedChanged += new System.EventHandler(this.CBSnooze_CheckedChanged);
			// 
			// CBRights
			// 
			resources.ApplyResources(this.CBRights, "CBRights");
			this.CBRights.Name = "CBRights";
			this.CBRights.CheckedChanged += new System.EventHandler(this.CBRights_CheckedChanged);
			// 
			// CBCards
			// 
			resources.ApplyResources(this.CBCards, "CBCards");
			this.CBCards.Name = "CBCards";
			this.CBCards.CheckedChanged += new System.EventHandler(this.CBCards_CheckedChanged);
			// 
			// CBTime
			// 
			resources.ApplyResources(this.CBTime, "CBTime");
			this.CBTime.Name = "CBTime";
			this.CBTime.CheckedChanged += new System.EventHandler(this.CBTime_CheckedChanged);
			// 
			// TSGeneral
			// 
			this.TSGeneral.Controls.Add(this.GBGeneral);
			resources.ApplyResources(this.TSGeneral, "TSGeneral");
			this.TSGeneral.Name = "TSGeneral";
			this.TSGeneral.UseVisualStyleBackColor = true;
			// 
			// GBGeneral
			// 
			this.GBGeneral.Controls.Add(this.checkBoxCheckForBetaUpdates);
			this.GBGeneral.Controls.Add(this.checkBoxUseDictionaryStylesheets);
			this.GBGeneral.Controls.Add(this.checkBoxStartExitSound);
			this.GBGeneral.Controls.Add(this.checkBoxOneInstance);
			this.GBGeneral.Controls.Add(this.CBGeneral3);
			this.GBGeneral.Controls.Add(this.CBGeneral2);
			this.GBGeneral.Controls.Add(this.CBGeneral1);
			this.GBGeneral.Controls.Add(this.CBGeneral0);
			this.GBGeneral.Controls.Add(this.CBGeneral7);
			this.GBGeneral.Controls.Add(this.CBGeneral6);
			this.GBGeneral.Controls.Add(this.CBGeneral5);
			this.GBGeneral.Controls.Add(this.CBGeneral4);
			resources.ApplyResources(this.GBGeneral, "GBGeneral");
			this.GBGeneral.Name = "GBGeneral";
			this.GBGeneral.TabStop = false;
			// 
			// checkBoxCheckForBetaUpdates
			// 
			resources.ApplyResources(this.checkBoxCheckForBetaUpdates, "checkBoxCheckForBetaUpdates");
			this.checkBoxCheckForBetaUpdates.Name = "checkBoxCheckForBetaUpdates";
			this.checkBoxCheckForBetaUpdates.UseVisualStyleBackColor = true;
			// 
			// checkBoxUseDictionaryStylesheets
			// 
			resources.ApplyResources(this.checkBoxUseDictionaryStylesheets, "checkBoxUseDictionaryStylesheets");
			this.checkBoxUseDictionaryStylesheets.Name = "checkBoxUseDictionaryStylesheets";
			this.MainHelp.SetShowHelp(this.checkBoxUseDictionaryStylesheets, ((bool)(resources.GetObject("checkBoxUseDictionaryStylesheets.ShowHelp"))));
			// 
			// checkBoxStartExitSound
			// 
			resources.ApplyResources(this.checkBoxStartExitSound, "checkBoxStartExitSound");
			this.checkBoxStartExitSound.Name = "checkBoxStartExitSound";
			this.MainHelp.SetShowHelp(this.checkBoxStartExitSound, ((bool)(resources.GetObject("checkBoxStartExitSound.ShowHelp"))));
			// 
			// checkBoxOneInstance
			// 
			resources.ApplyResources(this.checkBoxOneInstance, "checkBoxOneInstance");
			this.checkBoxOneInstance.Name = "checkBoxOneInstance";
			// 
			// CBGeneral3
			// 
			resources.ApplyResources(this.CBGeneral3, "CBGeneral3");
			this.CBGeneral3.Name = "CBGeneral3";
			// 
			// CBGeneral2
			// 
			resources.ApplyResources(this.CBGeneral2, "CBGeneral2");
			this.CBGeneral2.Name = "CBGeneral2";
			// 
			// CBGeneral1
			// 
			resources.ApplyResources(this.CBGeneral1, "CBGeneral1");
			this.CBGeneral1.Name = "CBGeneral1";
			// 
			// CBGeneral0
			// 
			resources.ApplyResources(this.CBGeneral0, "CBGeneral0");
			this.CBGeneral0.Name = "CBGeneral0";
			// 
			// CBGeneral7
			// 
			resources.ApplyResources(this.CBGeneral7, "CBGeneral7");
			this.CBGeneral7.Name = "CBGeneral7";
			// 
			// CBGeneral6
			// 
			resources.ApplyResources(this.CBGeneral6, "CBGeneral6");
			this.CBGeneral6.Name = "CBGeneral6";
			// 
			// CBGeneral5
			// 
			resources.ApplyResources(this.CBGeneral5, "CBGeneral5");
			this.CBGeneral5.Name = "CBGeneral5";
			// 
			// CBGeneral4
			// 
			resources.ApplyResources(this.CBGeneral4, "CBGeneral4");
			this.CBGeneral4.Name = "CBGeneral4";
			// 
			// btnOkay
			// 
			resources.ApplyResources(this.btnOkay, "btnOkay");
			this.btnOkay.Name = "btnOkay";
			this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			// 
			// QueryOptionsForm
			// 
			this.AcceptButton = this.btnOkay;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOkay);
			this.Controls.Add(this.PCOptions);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
			this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "QueryOptionsForm";
			this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
			this.ShowInTaskbar = false;
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.QueryOptionsForm_HelpRequested);
			this.PCOptions.ResumeLayout(false);
			this.TSTeachers.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTeacherHelp)).EndInit();
			this.GBTeacher.ResumeLayout(false);
			this.TSLearning.ResumeLayout(false);
			this.TSSynonyms.ResumeLayout(false);
			this.GBSynonyms.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTextInfo)).EndInit();
			this.TSTyping.ResumeLayout(false);
			this.TSTyping.PerformLayout();
			this.GBTyping.ResumeLayout(false);
			this.TSPlanning.ResumeLayout(false);
			this.GBEndSession.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SEMin2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SEMin1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SERights)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SECards)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SETime)).EndInit();
			this.TSGeneral.ResumeLayout(false);
			this.GBGeneral.ResumeLayout(false);
			this.GBGeneral.PerformLayout();
			this.ResumeLayout(false);

        }
        #endregion

        #region Helpers

        /// <summary>
        /// Checks if learning session should be ended ie if one of the checkboxes is ticked off
        /// </summary>
        /// <returns>Returns true or false</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private bool NoneChecked()
        {
            if (CBTime.Checked) return false;
            if (CBCards.Checked) return false;
            if (CBRights.Checked) return false;
            return true;
        }

        /// <summary>
        /// Enables checkboxes for snooze and quit
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void DoEnd()
        {
            CBSnooze.Enabled = true;
            CBQuit.Enabled = true;
            if (!CBQuit.Checked)
                CBSnooze.Checked = true;
        }

        /// <summary>
        /// If NoneChecked is true checkboxes for sooze and quit are disabled
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void DoNotEnd()
        {
            if (NoneChecked())
            {
                CBSnooze.Checked = false;
                CBSnooze.Enabled = false;
                CBQuit.Checked = false;
                CBQuit.Enabled = false;
            }
        }

        /// <summary>
        /// Checkbox with the submitted index is ticked off
        /// </summary>
        /// <param name="sender">Index of checkbox</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void RGSynonym_CheckItem(int index)
        {
            switch (index)
            {
                case (int)EGradeSynonyms.AllKnown: RGSynonym0.Checked = true; break;
                case (int)EGradeSynonyms.HalfKnown: RGSynonym1.Checked = true; break;
                case (int)EGradeSynonyms.OneKnown: RGSynonym2.Checked = true; break;
                case (int)EGradeSynonyms.FirstKnown: RGSynonym3.Checked = true; break;
                case (int)EGradeSynonyms.Prompt: RGSynonym4.Checked = true; break;
            }
        }

        /// <summary>
        /// Checkbox with the submitted index is ticked off
        /// </summary>
        /// <param name="sender">Index of checkbox</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void RGTyping_CheckItem(int index)
        {
            switch (index)
            {
                case (int)EGradeTyping.AllCorrect: RGTyping0.Checked = true; break;
                case (int)EGradeTyping.HalfCorrect: RGTyping1.Checked = true; break;
                case (int)EGradeTyping.NoneCorrect: RGTyping2.Checked = true; break;
                case (int)EGradeTyping.Prompt: RGTyping3.Checked = true; break;
            }
        }

        /// <summary>
        /// Returns index of checkbox that is ticked off
        /// </summary>
        /// <returns>Index of checkbox</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private EGradeSynonyms RGSynonym_GetValue()
        {
            if (RGSynonym0.Checked) return EGradeSynonyms.AllKnown;
            if (RGSynonym1.Checked) return EGradeSynonyms.HalfKnown;
            if (RGSynonym2.Checked) return EGradeSynonyms.OneKnown;
            if (RGSynonym3.Checked) return EGradeSynonyms.FirstKnown;
            if (RGSynonym4.Checked) return EGradeSynonyms.Prompt;
            return EGradeSynonyms.Prompt;
        }

        /// <summary>
        /// Returns index of checkbox that is ticked off
        /// </summary>
        /// <returns>Index of checkbox</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private EGradeTyping RGTyping_GetValue()
        {
            if (RGTyping0.Checked) return EGradeTyping.AllCorrect;
            if (RGTyping1.Checked) return EGradeTyping.HalfCorrect;
            if (RGTyping2.Checked) return EGradeTyping.NoneCorrect;
            if (RGTyping3.Checked) return EGradeTyping.Prompt;
            return EGradeTyping.Prompt;
        }
        #endregion

        #region Other Eventhandlers

        /// <summary>
        /// Groupbox GBTyping (Typing mistakes) is enabled or disabled if value of Checkbox "Correct on the fly" is changed
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void CBCorrect_CheckedChanged(object sender, System.EventArgs e)
        {
            GBTyping.Enabled = CBCorrect.Checked;
        }

        /// <summary>
        /// General checkboxes are enabled or disabled if value of Checkbox "Self Assessment" is changed
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void CBSelfAssessment_CheckedChanged(object sender, System.EventArgs e)
        {
            if (CBSelfAssessment.Checked)
            {
                CBGeneral7.Checked = false;
                CBGeneral7.Enabled = false; // Confirm Demote
                CBGeneral5.Enabled = false; // Skip showing correct answers
            }
            else
            {
                CBGeneral7.Enabled = true; // Confirm Demote
                CBGeneral5.Enabled = true; // Skip showing correct answers
            }
        }

        /// <summary>
        /// If minutes1 for snoozing and popping up are changed, this is the new minimum for minutes2
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void SEMin1_ValueChanged(object sender, System.EventArgs e)
        {
            SEMin2.Minimum = SEMin1.Value;
        }

        /// <summary>
        /// If minutes2 for snoozing and popping up are changed, this is the new maximum for minutes1
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void SEMin2_ValueChanged(object sender, System.EventArgs e)
        {
            SEMin1.Maximum = SEMin2.Value;
        }

        /// <summary>
        /// Is CBTime is checked  checkboxes for snooze and quit are enabled, else disabled
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void CBTime_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!CBTime.Checked)
                DoNotEnd();
            else
                DoEnd();
        }

        /// <summary>
        /// If CBCards is checked  checkboxes for snooze and quit are enabled, else disabled
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void CBCards_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!CBCards.Checked)
                DoNotEnd();
            else
                DoEnd();
        }


        /// <summary>
        /// If CBRights is checked  checkboxes for snooze and quit are enabled, else disabled
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void CBRights_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!CBRights.Checked)
                DoNotEnd();
            else
                DoEnd();
        }

        /// <summary>
        /// If CBSnooze is checked it is ensured that CBQuit is not checked.
        /// If everything excluded CBSnooze is checked it is ensured that CBQuit is checked.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

        private void CBSnooze_CheckedChanged(object sender, System.EventArgs e)
        {
            if (CBSnooze.Checked)
                CBQuit.Checked = false;
            else
                if (!NoneChecked()) CBQuit.Checked = true;
        }

        /// <summary>
        /// If CBQuit is checked it is ensured that CBSnooze is not checked.
        /// If everything excluded CBQuit is checked it is ensured that CBSnooze is checked.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void CBQuit_CheckedChanged(object sender, System.EventArgs e)
        {
            if (CBQuit.Checked)
                CBSnooze.Checked = false;
            else
                if (!NoneChecked()) CBSnooze.Checked = true;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the labelTeacherLoadingError control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-17</remarks>
        private void labelTeacherLoadingError_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (labelTeacherLoadingError.Tag != null && labelTeacherLoadingError.Tag is Exception)
            {
                MLifter.Classes.ErrorReportGenerator.ReportError(labelTeacherLoadingError.Tag as Exception, false);
            }
        }
        #endregion

        private void pictureBoxTeacherHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this.ParentForm, MainHelp.HelpNamespace, HelpNavigator.Topic, "/html/Teachers.htm");
        }

        private void QueryOptionsForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (PCOptions.SelectedIndex == 0)	// to force show of teacher help
            {
                Help.ShowHelp(this.ParentForm, MainHelp.HelpNamespace, HelpNavigator.Topic, "/html/Teachers.htm");
                hlpevent.Handled = true;
            }
        }
    }
}
