using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MLifterAudioTools.Codecs
{
    public partial class ForumLink : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForumLink"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-18</remarks>
        public ForumLink()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-18</remarks>
        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(Properties.Resources.ML_FORUM_LINK);
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Could not start link: " + exp.ToString());
            }
        }
    }
}
