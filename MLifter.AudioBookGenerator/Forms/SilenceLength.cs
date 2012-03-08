using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MLifterAudioBookGenerator.Forms
{
    public partial class SilenceLength : Form
    {
        /// <summary>
        /// Gets or sets the length of the silence.
        /// </summary>
        /// <value>The length of the silence.</value>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public double Length
        {
            get { return Double.Parse(textBoxLength.Text); }
            set { textBoxLength.Text = Convert.ToString(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SilenceLength"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public SilenceLength()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the FormClosing event of the SilenceLength control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private void SilenceLength_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
                this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool inputvalid = true;
            try
            {
                if (double.Parse(textBoxLength.Text) <= 0)
                    inputvalid = false;
            }
            catch
            {
                inputvalid = false;
            }

            if (inputvalid)
            {
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            else
            {
                MessageBox.Show("Your input is not in a valid format, please correct it.", "Valid value required.");
                return;
            }
        }
    }
}