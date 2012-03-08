using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MLifter.Properties;

namespace MLifter
{
    /// <summary>
    /// Summary Description for LearnStats.
    /// </summary>
    public class QueryStatsForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label LblTime;
        private System.Windows.Forms.Label LblWords;
        private System.Windows.Forms.Label LblCorrect;
        private System.Windows.Forms.Label LblWrong;
        private System.Windows.Forms.Label LblRatio;
        private System.Windows.Forms.Label LblAverage;
        private System.Windows.Forms.Label LblWPM;
        private System.Windows.Forms.Button BtnOkay;
        private System.Windows.Forms.Button BtnCancel;
        private LinkLabel linkLabelHomepage;
        private Timer timerCountdown;
        private IContainer components;


        /// <summary>
        /// Constructor - required for Windows Form Designer support
        /// </summary
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

        public QueryStatsForm()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Shows summary (Session time, cards processed, correct answers, wrong answers, ratio, average time/card, cards/minute)
        /// </summary>
        /// <param name="time">Time interval</param>
        /// <param name="right">Number of right answers</param>
        /// <param name="wrong">Number of wrong answers</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

        public void ShowStats(System.TimeSpan time, int right, int wrong)
        {
            int all = right + wrong;
            double temp = 0.0;
            LblTime.Text = String.Format(Resources.LEARNSTAT_TIME_FORMAT, time.Hours, time.Minutes, time.Seconds);
            LblWords.Text = all.ToString();
            LblCorrect.Text = right.ToString();
            LblWrong.Text = wrong.ToString();

            if (right > 0)
            {
                temp = (right * 100.0) / (all * 1.0);
                LblRatio.Text = temp.ToString(Resources.COMMON_NUMBER_FORMAT) + " " + Resources.ABBREVIATION_PERCENT;
            }
            else
                LblRatio.Text = "0 %";

            if (all > 0)
            {
                temp = time.TotalSeconds / all;
                LblAverage.Text = temp.ToString(Resources.COMMON_NUMBER_FORMAT) + " " + Resources.ABBREVIATION_SECONDS;
            }
            else
                LblAverage.Text = Resources.COMMON_NUMBER_FORMAT;

            if (time.TotalSeconds > 0)
            {
                temp = all * 60 / time.TotalSeconds;
                LblWPM.Text = temp.ToString("0");
            }

            if (Properties.Settings.Default.StatisticsShowTime > 0)
            {
                timerCountdown.Tag = new TimeSpan(0, 0, Properties.Settings.Default.StatisticsShowTime);
                timerCountdown.Enabled = true;
                timerCountdown.Start();
            }
            this.ShowDialog();
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabelHomepage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-03</remarks>
        private void linkLabelHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start((sender as LinkLabel).Text);
            }
            catch
            {
                MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryStatsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LblWPM = new System.Windows.Forms.Label();
            this.LblAverage = new System.Windows.Forms.Label();
            this.LblRatio = new System.Windows.Forms.Label();
            this.LblWrong = new System.Windows.Forms.Label();
            this.LblCorrect = new System.Windows.Forms.Label();
            this.LblWords = new System.Windows.Forms.Label();
            this.LblTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.linkLabelHomepage = new System.Windows.Forms.LinkLabel();
            this.timerCountdown = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LblWPM);
            this.groupBox1.Controls.Add(this.LblAverage);
            this.groupBox1.Controls.Add(this.LblRatio);
            this.groupBox1.Controls.Add(this.LblWrong);
            this.groupBox1.Controls.Add(this.LblCorrect);
            this.groupBox1.Controls.Add(this.LblWords);
            this.groupBox1.Controls.Add(this.LblTime);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // LblWPM
            // 
            resources.ApplyResources(this.LblWPM, "LblWPM");
            this.LblWPM.Name = "LblWPM";
            // 
            // LblAverage
            // 
            resources.ApplyResources(this.LblAverage, "LblAverage");
            this.LblAverage.Name = "LblAverage";
            // 
            // LblRatio
            // 
            resources.ApplyResources(this.LblRatio, "LblRatio");
            this.LblRatio.Name = "LblRatio";
            // 
            // LblWrong
            // 
            resources.ApplyResources(this.LblWrong, "LblWrong");
            this.LblWrong.Name = "LblWrong";
            // 
            // LblCorrect
            // 
            resources.ApplyResources(this.LblCorrect, "LblCorrect");
            this.LblCorrect.Name = "LblCorrect";
            // 
            // LblWords
            // 
            resources.ApplyResources(this.LblWords, "LblWords");
            this.LblWords.Name = "LblWords";
            // 
            // LblTime
            // 
            resources.ApplyResources(this.LblTime, "LblTime");
            this.LblTime.Name = "LblTime";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // BtnOkay
            // 
            this.BtnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.BtnOkay, "BtnOkay");
            this.BtnOkay.Name = "BtnOkay";
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.Name = "BtnCancel";
            // 
            // linkLabelHomepage
            // 
            resources.ApplyResources(this.linkLabelHomepage, "linkLabelHomepage");
            this.linkLabelHomepage.LinkColor = System.Drawing.Color.Blue;
            this.linkLabelHomepage.Name = "linkLabelHomepage";
            this.linkLabelHomepage.TabStop = true;
            this.linkLabelHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelHomepage_LinkClicked);
            // 
            // timerCountdown
            // 
            this.timerCountdown.Tick += new System.EventHandler(this.timerCountdown_Tick);
            // 
            // QueryStatsForm
            // 
            this.AcceptButton = this.BtnOkay;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.BtnCancel;
            this.Controls.Add(this.linkLabelHomepage);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryStatsForm";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Handles the Tick event of the timerCountdown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-10</remarks>
        private void timerCountdown_Tick(object sender, EventArgs e)
        {
            timerCountdown.Tag = ((TimeSpan)timerCountdown.Tag).Add(new TimeSpan(0, 0, 0, 0, -timerCountdown.Interval));
            this.Text = string.Format(Properties.Resources.STATISTICS_CLOSING_COUNTDOWN, ((TimeSpan)timerCountdown.Tag).TotalSeconds);
            if (((TimeSpan)timerCountdown.Tag).TotalMilliseconds <= 0)
            {
                timerCountdown.Stop();
                timerCountdown.Enabled = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
