using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MLifter.Controls.Properties;

namespace MLifter.Controls.Wizards.DictionaryCreator
{
    public partial class InfoPage : MLifter.WizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public InfoPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoPage"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public InfoPage(MLifter.BusinessLayer.Dictionary dictionary)
        {
            SetInfo(dictionary);
        }

        /// <summary>
        /// Sets the information on this page from a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public void SetInfo(MLifter.BusinessLayer.Dictionary dictionary)
        {
            if (dictionary.DictionaryDAL.Parent.CurrentUser.ConnectionString.Typ == MLifter.DAL.DatabaseType.PostgreSQL)
                textBoxDictionaryLocation.Text = GenerateDatabaseInformation(dictionary.DictionaryPath);    //dictionary.DictionaryPath;
            else
                textBoxDictionaryLocation.Text = dictionary.DictionaryPath;

            textBoxDictionaryLocation.SelectionStart = textBoxDictionaryLocation.Text.Length - 1;
            labelNumberOfCards.Text = Convert.ToString(dictionary.Cards.Cards.Count);

			// so that "Nan %" can't be displayed ML-1764
			double score = (double.IsNaN(dictionary.Score)) ? 0.0 : dictionary.Score;
			double highScore = (double.IsNaN(dictionary.Highscore)) ? 0.0 : dictionary.Highscore;

            labelScore.Text = Convert.ToString(Math.Round(score, 2)) + " %";
            labelHighscore.Text = Convert.ToString(Math.Round(highScore, 2)) + " %";
            dictionary.UsedDiskSpaceComplete += new MLifter.BusinessLayer.Dictionary.UsedDiskSpaceCompleteHandler(dictionary_UsedDiskSpaceComplete);
            dictionary.UsedDiskSpaceAsync();
        }

        private string GenerateDatabaseInformation(string connectionString)
        {
            string subStringServer = connectionString.Substring(connectionString.ToLowerInvariant().IndexOf("server="));
            subStringServer = subStringServer.Substring(0, subStringServer.IndexOf(";"));

            string subStringDatabase = connectionString.Substring(connectionString.ToLowerInvariant().IndexOf("database="));
            subStringDatabase = subStringDatabase.Substring(0, subStringDatabase.IndexOf(";"));

            return subStringServer + ", " + subStringDatabase;
        }

        /// <summary>
        /// Handles the UsedDiskSpaceComplete event of the dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.Dictionary.UsedDiskSpaceCompleteEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        void dictionary_UsedDiskSpaceComplete(object sender, MLifter.BusinessLayer.Dictionary.UsedDiskSpaceCompleteEventArgs e)
        {
            if (!labelUsedDiskSpace.InvokeRequired)
            {
                labelUsedDiskSpace.Text = MLifter.Tools.FileSizeToString(e.space);
                labelUsedDiskFiles.Text = Convert.ToString(e.count);
            }
            else
                labelUsedDiskSpace.Invoke((MethodInvoker)delegate { dictionary_UsedDiskSpaceComplete(sender, e); });
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo9sqf.htm");
        }
    }
}

