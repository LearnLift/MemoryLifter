using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MLifter.Components;
using MLifter.Properties;
using System.Threading;

namespace MLifter
{
    /// <summary>
    /// Summary Description for Splash.
    /// </summary>
    public class Splash : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label lblStatus;
        private Label lblVersion;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Constructor of the Splash Object
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public Splash()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            lblVersion.Text = String.Format(Resources.SPLASH_VERSION_TEXT, AssemblyData.Version);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblStatus.Name = "lblStatus";
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersion.Name = "lblVersion";
            // 
            // Splash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Magenta;
            this.BackgroundImage = global::MLifter.Properties.Resources.MLSplashScreen;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Splash";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.Load += new System.EventHandler(this.Splash_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Splash_FormClosing);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Changes Text (e.g. "Loading settings...") on Splash-Screen
        /// </summary>
        /// <param name="Text">Current status Text.</param>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public void SetStatusMessage(string text)
        {
            lblStatus.Text = text;
            if (lblStatus.Visible)
                lblStatus.Refresh();
        }

        //experimental code: tells windows that this form must not get focus
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        const int WS_EX_NOACTIVATE = 0x08000000;
        //        CreateParams myParams = base.CreateParams;
        //        myParams.ExStyle = myParams.ExStyle | WS_EX_NOACTIVATE;
        //        return myParams;
        //    }
        //}

        /// <summary>
        /// Handles the Load event of the Splash control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        private void Splash_Load(object sender, EventArgs e)
        {
            //[ML-892] Win2000 does not like it when the region gets set before the form is shown
            //Region was necessary for Win 2000 (TransparancyKey does not work in combination with DoubleBuffered=true), but not necessary anymore with DoubleBuffered=false
            //this.Region = new Region(Tools.CreateRoundRectangle(this.ClientRectangle, 18)); 

            progressTimer = new System.Windows.Forms.Timer();
            progressTimer.Interval = 500;
            progressTimer.Tick += new EventHandler(progressTimer_Tick);
            progressTimer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the progressTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-27</remarks>
        void progressTimer_Tick(object sender, EventArgs e)
        {
            lblStatus.Text += ".";
        }

        System.Windows.Forms.Timer progressTimer = null;

        /// <summary>
        /// Handles the FormClosing event of the Splash control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-27</remarks>
        private void Splash_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (progressTimer != null)
            {
                progressTimer.Stop();
                progressTimer.Dispose();
                progressTimer = null;
            }
        }
    }
}
