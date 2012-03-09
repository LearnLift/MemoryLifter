using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MLifter.Controls.Properties;

namespace MLifter
{
    public partial class Wizard : Form
    {
        private const int FOOTER_AREA_HEIGHT = 75;

        internal int Position = 0;

        private string m_HelpFile;

        private object m_UserState;

        /// <summary>
        /// Gets or sets the help file.
        /// </summary>
        /// <value>The help file.</value>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public string HelpFile
        {
            get { return m_HelpFile; }
            set { m_HelpFile = value; }
        }

        public object UserState
        {
            get { return m_UserState; }
        }

        public event EventHandler PageActualized;

        private List<WizardPage> pages = new List<WizardPage>();
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        public List<WizardPage> Pages
        {
            get { return pages; }
            set { pages = value; }
        }

        private WizardPage actualPage;
        /// <summary>
        /// Gets the actual page.
        /// </summary>
        /// <value>The actual page.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public WizardPage ActualPage
        {
            get
            {
                if (actualPage != null)
                    return actualPage;
                else
                    return pages[0];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wizard"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        public Wizard()
        {
            InitializeWizard(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wizard"/> class.
        /// </summary>
        /// <param name="helpfile">The helpfile.</param>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        public Wizard(string helpfile)
        {
            InitializeWizard(helpfile);
        }

        /// <summary>
        /// Initializes the wizard.
        /// </summary>
        /// <param name="helpfile">The helpfile.</param>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        private void InitializeWizard(string helpfile)
        {
            this.HelpFile = helpfile;
            Application.EnableVisualStyles();
            InitializeComponent();
            this.DialogResult = DialogResult.Abort;
        }

        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle bottomRect = this.ClientRectangle;
            bottomRect.Y = this.Height - FOOTER_AREA_HEIGHT;
            bottomRect.Height = FOOTER_AREA_HEIGHT;
            ControlPaint.DrawBorder3D(e.Graphics, bottomRect, Border3DStyle.Etched, Border3DSide.Top);
        }

        /// <summary>
        /// Handles the Shown event of the Wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void Wizard_Shown(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            foreach (WizardPage page in pages)
                page.ParentWizard = this;

            if (pages.Count > 0)
                ActualizePage();
            else
                Close();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the actualPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        void actualPage_PropertyChanged(object sender, WizardEventArgs e)
        {
            m_UserState = e.UserState;
            UpdateButtonStates();
        }

        /// <summary>
        /// Updates the button states.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public void UpdateButtonStates()
        {
            buttonBack.Enabled = ActualPage.BackAllowed && pages.IndexOf(actualPage) > 0;
            buttonCancel.Enabled = ActualPage.CancelAllowed;
            ShowIcon = !ActualPage.CancelAllowed;
            ControlBox = ActualPage.CancelAllowed;
            buttonHelp.Visible = ActualPage.HelpAvailable && !string.IsNullOrEmpty(HelpFile);
            buttonNext.Enabled = ActualPage.NextAllowed;

            buttonNext.Text = (ActualPage.LastStep || pages.IndexOf(ActualPage) == pages.Count - 1 ? Resources.WIZARD_FINISH : Resources.WIZARD_NEXT);
            buttonNext.Image = (ActualPage.LastStep || pages.IndexOf(ActualPage) == pages.Count - 1 ? null : Resources.goNext);
            buttonNext.TextAlign = (ActualPage.LastStep || pages.IndexOf(ActualPage) == pages.Count - 1 ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight);
        }

        public void GoToNextPage()
        {
            buttonNext_Click(this, EventArgs.Empty);
        }

        public void GoToPreviousPage()
        {
            buttonBack_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the buttonNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (!actualPage.GoNext())
                return;

            actualPage.SubmitPage -= new EventHandler(actualPage_SubmitPage);

            Position++;
            DetachEvents();
            if (actualPage.LastStep || Position >= pages.Count)
                Finish();
            else
                ActualizePage();
        }

        /// <summary>
        /// Handles the Click event of the buttonBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (!actualPage.GoBack())
                return;
            Position--;
            DetachEvents();
            ActualizePage();
        }

        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (actualPage.Cancel())
            {
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonHelp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-20</remarks>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        /// <summary>
        /// Shows the help of the current page.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        private void ShowHelp()
        {
            if (!string.IsNullOrEmpty(HelpFile))
                actualPage.ShowHelp();
        }

        /// <summary>
        /// Actualizes the page.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void ActualizePage()
        {
            tableLayoutPanelPages.Controls.Clear();
            tableLayoutPanelPages.Controls.Add(pages[Position], 0, 0);
            actualPage = pages[Position];
            AttachEvents();
            UpdateButtonStates();

            OnPageActualized(this, EventArgs.Empty);
        }

        private void OnPageActualized(object sender, EventArgs e)
        {
            if (PageActualized != null)
                PageActualized(sender, e);
        }

        /// <summary>
        /// Attach event handler to actual page.
        /// </summary>
        private void AttachEvents()
        {
            actualPage.PropertyChanged += new EventHandler<WizardEventArgs>(actualPage_PropertyChanged);
            actualPage.SubmitPage += new EventHandler(actualPage_SubmitPage);
        }

        /// <summary>
        /// Detach event handler from actual page.
        /// </summary>
        private void DetachEvents()
        {
            actualPage.PropertyChanged -= new EventHandler<WizardEventArgs>(actualPage_PropertyChanged);
            actualPage.SubmitPage -= new EventHandler(actualPage_SubmitPage);
        }

        void actualPage_SubmitPage(object sender, EventArgs e)
        {
            buttonNext_Click(sender, e);
        }

        /// <summary>
        /// Finishes this wizard.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void Finish()
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the HelpButtonClicked event of the Wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        private void Wizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            ShowHelp();
        }

        /// <summary>
        /// Handles the HelpRequested event of the Wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="hlpevent">The <see cref="System.Windows.Forms.HelpEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        private void Wizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
        }

        /// <summary>
        /// Handles the FormClosing event of the Wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-05-28</remarks>
        private void Wizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            //[ML-1925] don't allow closing in case the user pressed alt+f4
            if (this.DialogResult != DialogResult.OK && !actualPage.CancelAllowed && e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

        }
    }
    /// <summary>
    /// Wizard state event args.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-21</remarks>
    public class WizardEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardEventArgs"/> class.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        public WizardEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardEventArgs"/> class.
        /// </summary>
        /// <param name="UserState">State of the user.</param>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        public WizardEventArgs(object UserState)
        {
            m_UserState = UserState;
        }

        private object m_UserState = null;

        /// <summary>
        /// Gets or sets a unique user state.
        /// </summary>
        /// <value>The user state.</value>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        public object UserState
        {
            get { return m_UserState; }
            set { m_UserState = value; }
        }

        public static new readonly WizardEventArgs Empty = new WizardEventArgs();
    }
}