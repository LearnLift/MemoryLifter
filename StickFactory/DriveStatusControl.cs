using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace StickFactory
{
    public partial class DriveStatusControl : UserControl
    {
        private ToolTip tooltip = new ToolTip();

        public char Drive { get { return labelDrive.Text[0]; } set { labelDrive.Text = value + ":"; } }
        public string StatusMessage { get { return labelStatus.Text; } set { labelStatus.Text = value; tooltip.SetToolTip(labelStatus, value); } }
        public bool FormattingFinish { get { return checkBoxFormat.Checked; } set { checkBoxFormat.Checked = value; } }
        public bool ContentWritten { get { return checkBoxContent.Checked; } set { checkBoxContent.Checked = value; } }
        public bool IdSet { get { return checkBoxStickID.Checked; } set { checkBoxStickID.Checked = value; } }
        public Color SignalColor { get { return labelDrive.BackColor; } set { labelDrive.BackColor = value; } }

        private double progress;
        public double Progress
        {
            get
            {
                return progressBarStatus.Value;
            }
            set
            {
                if (value < 100)
                    progressBarStatus.Value = Convert.ToInt32(value);
                else
                    progressBarStatus.Value = 100;

                progress = value;
                ProgressChanged();
            }
        }
        private int current;
        public int Current
        {
            get { return current; }
            set { current = value; }
        }
        private int count;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public DriveStatusControl()
        {
            InitializeComponent();
        }

        public void Set(char drive)
        {
            labelDrive.Text = drive + ":";
            OverridedBackColor = Color.White;
        }

        public void Reset()
        {
            StatusMessage = "Waiting for Stick...";
            FormattingFinish = false;
            ContentWritten = false;
            IdSet = false;
            Progress = 0;
            OverridedBackColor = SystemColors.Control;

            oldTime = new DateTime(1900, 1, 1);
            times.Clear();
        }

        public Color OverridedBackColor
        {
            get { return labelStatus.BackColor; }
            set
            {
                this.BackColor = value;
                labelDrive.BackColor = value;
                labelStatus.BackColor = value;
                checkBoxContent.BackColor = value;
                checkBoxFormat.BackColor = value;
                checkBoxStickID.BackColor = value;
                progressBarStatus.BackColor = value;
            }
        }

        private double oldValue = 101;
        private DateTime oldTime;
        private DateTime lastUpdate = new DateTime(1900, 1, 1);
        private List<double> times = new List<double>();
        private void ProgressChanged()
        {
            double actual = progressBarStatus.Value;
            double totalCnt = 100;

            if (oldValue < actual)
            {
                double elapsed = ((TimeSpan)(DateTime.Now - oldTime)).TotalSeconds;
                double time = elapsed;

                if (time < int.MaxValue)
                    times.Add(time);
                if (times.Count > 0)
                {
                    double total = 0;
                    foreach (double t in times)
                        total += t;
                    double mean = total / times.Count;
                    time = mean * (totalCnt - actual);

                    int min = Convert.ToInt32((time - time % 60) / 60);
                    int sec = Convert.ToInt32(time - min * 60);

                    labelTime.Invoke((MethodInvoker)delegate
                    {
                        labelTime.Text = string.Format("~{0:00}:{1:00} left", min, sec);
                        labelTime.Visible = true;
                    });
                }
                oldTime = DateTime.Now;
            }
            else if (oldValue != actual)
                labelTime.Invoke((MethodInvoker)delegate { labelTime.Visible = false; });

            oldValue = actual;
        }
    }
}
