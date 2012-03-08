using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MLifter.Components;

namespace MLifter.Controls
{
    /// <summary>
    /// Shows arrayList load message with arrayList progress bar.
    /// </summary>
    public class LoadStatusMessage : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label lblInfo;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelProgressRow;
        private Label lblWait;
        private ProgressBar colorProgressBar;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Gets or sets the info message.
        /// </summary>
        /// <value>The info message.</value>
        /// <remarks>Documented by Dev03, 2007-09-27</remarks>
        public string InfoMessage
        {
            get
            {
                return lblInfo.Text;
            }
            set
            {
                lblInfo.Text = value; Application.DoEvents();
            }
        }

        /// <summary>
        /// This is the constructor of LoadStatusMessage Object
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        /// <param name="maxValue">The max value of the progressbar.</param>
        /// <param name="EnableProgressbar">if set to <c>true</c> [enable progressbar].</param>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public LoadStatusMessage(string infoMessage, int maxValue, bool EnableProgressbar)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.DataBindings.Add(new Binding("Width", lblInfo, "Width"));

            this.InfoMessage = infoMessage;
            if (EnableProgressbar)
            {
                colorProgressBar.Minimum = 0;
                colorProgressBar.Maximum = maxValue;
                colorProgressBar.Value = 0;
                colorProgressBar.Visible = true;
                lblWait.Visible = false;
                SetProgress(0);
            }
            else
            {
                colorProgressBar.Visible = false;
                lblWait.Visible = true;
            }
            Application.DoEvents();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadStatusMessage));
            this.lblInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelProgressRow = new System.Windows.Forms.Panel();
            this.colorProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblWait = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelProgressRow.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            resources.ApplyResources(this.lblInfo, "lblInfo");
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Resize += new System.EventHandler(this.lblInfo_Resize);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lblInfo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelProgressRow, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panelProgressRow
            // 
            this.panelProgressRow.Controls.Add(this.colorProgressBar);
            this.panelProgressRow.Controls.Add(this.lblWait);
            resources.ApplyResources(this.panelProgressRow, "panelProgressRow");
            this.panelProgressRow.Name = "panelProgressRow";
            // 
            // colorProgressBar
            // 
            resources.ApplyResources(this.colorProgressBar, "colorProgressBar");
            this.colorProgressBar.Name = "colorProgressBar";
            this.colorProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.colorProgressBar.Value = 50;
            // 
            // lblWait
            // 
            resources.ApplyResources(this.lblWait, "lblWait");
            this.lblWait.ForeColor = System.Drawing.Color.Red;
            this.lblWait.Name = "lblWait";
            // 
            // LoadStatusMessage
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.White;
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadStatusMessage";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.LoadStatusMessage_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelProgressRow.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether to [enable progressbar].
        /// </summary>
        /// <value><c>true</c> if [enable progressbar]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        public bool EnableProgressbar
        {
            get { return colorProgressBar.Visible; }
            set
            {
                if (colorProgressBar.Visible == value)
                    return;

                colorProgressBar.Visible = value;
                lblWait.Visible = !value;
                if (value)
                    SetProgress(0);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the progressbar maxvalue.
        /// </summary>
        /// <value>The progressbar maxvalue.</value>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        public int ProgressbarMaxvalue
        {
            get { return colorProgressBar.Maximum; }
            set { if (value > colorProgressBar.Minimum) colorProgressBar.Maximum = value; }
        }

        /// <summary>
        /// This method updates the ProgressBar until 100% (e.g. if you pack arrayList dictionary)
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public void ProgressStep()
        {
            if (colorProgressBar.Value < colorProgressBar.Maximum)
                colorProgressBar.Value++;
            colorProgressBar.Update();
            Application.DoEvents();
        }
        /// <summary>
        /// Sets the progress to a given value.
        /// </summary>
        /// <param name="progess">The progess.</param>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public void SetProgress(int progess)
        {
            if (progess < colorProgressBar.Maximum)
                colorProgressBar.Value = progess;
            colorProgressBar.Update();
            Application.DoEvents();
        }

        /// <summary>
        /// Handles the Shown event of the LoadStatusMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        private void LoadStatusMessage_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
        }

        private void lblInfo_Resize(object sender, EventArgs e)
        {
            //[ML-925] Adapt the controls for localization
            //if (lblInfo.Width < this.Width)
            //{
            //    lblInfo.Padding = new Padding(0, 0, 0, 0);
            //    lblInfo.Left = (this.Width - lblInfo.Width) / 2;
            //}
            //else
            //{
            //    lblInfo.Padding = new Padding(20, 0, 20, 0);
            //    lblInfo.Left = 0;
            //}
        }
    }
}
