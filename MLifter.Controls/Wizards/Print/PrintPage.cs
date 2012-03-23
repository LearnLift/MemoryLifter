/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.BusinessLayer;
using MLifter.Components;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MLifter.Controls.Wizards.Print
{
    public partial class PrintPage : MLifter.WizardPage
    {
        PrintSettings settings;
        StyleHandler styleHandler;
        Dictionary dictionary;
        LoadStatusMessage statusDialog;

        public readonly string[] QueryOrders = new string[5]
        {
            Properties.Resources.PRINT_QUERYORDER_Question,
            Properties.Resources.PRINT_QUERYORDER_Answer,
            Properties.Resources.PRINT_QUERYORDER_Box,
            Properties.Resources.PRINT_QUERYORDER_Chapter,
            Properties.Resources.PRINT_QUERYORDER_None
        };
        public readonly DAL.Interfaces.QueryOrder[] QueryOrdersEnum = new DAL.Interfaces.QueryOrder[5]
        {
            QueryOrder.Question,
            QueryOrder.Answer,
            QueryOrder.Box,
            QueryOrder.Chapter,
            QueryOrder.None
        };

        /// <summary>
        /// Gets or sets the style handler.
        /// </summary>
        /// <value>The style handler.</value>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        public StyleHandler StyleHandler
        {
            get { return styleHandler; }
            set { styleHandler = value; }
        }

        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        public Dictionary Dictionary
        {
            get { return dictionary; }
            set { dictionary = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        public PrintPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the PrintPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void PrintPage_Load(object sender, EventArgs e)
        {
            settings = ParentWizard.Tag as PrintSettings;

            //add queryorders
            comboBoxSorting.Items.Clear();
            comboBoxSorting.Items.AddRange(QueryOrders);

            //add printstyles
            comboBoxStyle.Items.Clear();
            foreach (StyleInfo styleinfo in styleHandler.CurrentStyle.PrintStyles)
                comboBoxStyle.Items.Add(styleinfo);

            //select default values
            comboBoxSorting.SelectedIndex = 0;
            comboBoxStyle.SelectedIndex = 0;
            Browser.DocumentText = string.Empty; //print settings don't display if the documenttext is not set
        }

        /// <summary>
        /// Handles the Click event of the buttonPreview control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void buttonPreview_Click(object sender, EventArgs e)
        {
            FillBrowser(sender);
        }

        /// <summary>
        /// Fills the browser with the current print material.
        /// </summary>
        /// <param name="sender">The sender button.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void FillBrowser(object sender)
        {
            this.Enabled = false;
            if (this.ParentWizard is Wizard)
                this.ParentWizard.Enabled = false;

            statusDialog = new LoadStatusMessage(Properties.Resources.PRINT_STATUS_FETCHINGCARDS, 100, false);
            statusDialog.Show();
            statusDialog.SetProgress(0);

            dictionary.PreloadCardCache();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

            BackgroundWorker prepareworker = new BackgroundWorker();
            prepareworker.WorkerReportsProgress = true;
            prepareworker.ProgressChanged += new ProgressChangedEventHandler(prepareworker_ProgressChanged);

            QueryOrder cardorder;
            QueryOrderDir cardorderdir;

            GetSelection(out cardorder, out cardorderdir);

            List<QueryStruct> querystructs = new List<QueryStruct>();
            List<ICard> cards = new List<ICard>();

            switch (settings.Type)
            {
                case PrintType.All:
                    querystructs.Add(new QueryStruct(QueryCardState.All));
                    break;
                case PrintType.Wrong:
                    querystructs.Add(new QueryStruct(-1, 1));
                    break;
                case PrintType.Chapter:
                    foreach (int id in settings.IDs)
                        querystructs.Add(new QueryStruct(id, -1));
                    break;
                case PrintType.Boxes:
                    foreach (int id in settings.IDs)
                        querystructs.Add(new QueryStruct(-1, id));
                    break;
                case PrintType.Individual:
                    cards = dictionary.GetPrintOutCards(settings.IDs, cardorder, cardorderdir);
                    break;
                default:
                    break;
            }

            if (settings.Type != PrintType.Individual)
                cards = dictionary.GetPrintOutCards(querystructs, cardorder, cardorderdir);

            if (cards == null || cards.Count < 1)
            {
                statusDialog.Close();
                MessageBox.Show(MLifter.Controls.Properties.Resources.PRINT_NOCARDS_TEXT, MLifter.Controls.Properties.Resources.PRINT_NOCARDS_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Enabled = true;
                if (this.ParentWizard is Wizard)
                    this.ParentWizard.Enabled = true;
                return;
            }

            string stylesheet = ((StyleInfo)comboBoxStyle.SelectedItem).Path;

            string htmlContent = dictionary.GeneratePrintOut(
                cards,
                stylesheet,
                worker,
                prepareworker);

#if DEBUG
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.Path.Combine(Application.StartupPath, "Print.htm"));
                writer.Write(htmlContent);
                writer.Close();
            }
            catch { }
#endif

            statusDialog.InfoMessage = Properties.Resources.PRINT_STATUS_RENDERING;
            statusDialog.EnableProgressbar = false;

            Browser.Tag = sender;
            Browser.DocumentText = htmlContent;
            //now we must wait for the webbrowser to complete loading [ML-63]
        }

        /// <summary>
        /// Handles the Click event of the buttonSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            Browser.ShowPageSetupDialog();
        }

        /// <summary>
        /// Handles the Click event of the buttonPrint control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            FillBrowser(sender);
        }

        /// <summary>
        /// Gets the query structs.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <param name="cardstate">The cardstate.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private static List<MLifter.DAL.Interfaces.QueryStruct> GetQueryStructs(List<int> chapters, List<int> boxes, QueryCardState cardstate)
        {
            if (chapters.Count < 1 || boxes.Count < 1)
                return null;

            //get cards with desired selection and ordering
            List<QueryStruct> querystructs = new List<QueryStruct>();
            foreach (int chapter in chapters)
                foreach (int box in boxes)
                    querystructs.Add(new QueryStruct(chapter, box, cardstate));

            return querystructs;
        }

        /// <summary>
        /// Gets the selection of the controls on the wizard page
        /// </summary>
        /// <param name="cardorder">The cardorder.</param>
        /// <param name="cardorderdir">The cardorderdir.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void GetSelection(out QueryOrder cardorder, out QueryOrderDir cardorderdir)
        {
            cardorder = QueryOrdersEnum[comboBoxSorting.SelectedIndex < 0 ? 0 : comboBoxSorting.SelectedIndex];
            cardorderdir = checkBoxReverseOrder.Checked ? QueryOrderDir.Descending : QueryOrderDir.Ascending;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-23</remarks>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (statusDialog != null)
            {
                if (statusDialog.InfoMessage != Properties.Resources.PRINT_STATUS_PAGES)
                {
                    statusDialog.InfoMessage = Properties.Resources.PRINT_STATUS_PAGES;
                    statusDialog.EnableProgressbar = true;
                    statusDialog.SetProgress(0);
                }
                statusDialog.SetProgress(e.ProgressPercentage);
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the prepareworker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        void prepareworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (statusDialog != null)
            {
                if (statusDialog.InfoMessage != Properties.Resources.PRINT_STATUS_MEDIA)
                {
                    statusDialog.InfoMessage = Properties.Resources.PRINT_STATUS_MEDIA;
                    statusDialog.EnableProgressbar = true;
                    statusDialog.SetProgress(0);
                }
                statusDialog.SetProgress(e.ProgressPercentage);
            }
        }

        /// <summary>
        /// Handles the DocumentCompleted event of the Browser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.WebBrowserDocumentCompletedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (statusDialog != null && statusDialog.Visible)
                statusDialog.Close();

            //now the webpage has finished loading and can be printed [ML-63]
            if (sender is WebBrowser && ((WebBrowser)sender).Tag is Button)
            {
                Button button = ((Button)((WebBrowser)sender).Tag);
                if (button == buttonPreview)
                {
                    //little hack to resize the PreviewDialog Window - without that, it would be shown in the size of this dialog
                    //if there is any other way, let me know ;-) AWE
                    ParentWizard.Hide();
                    Size size = ParentWizard.Size;
                    ParentWizard.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                    Browser.ShowPrintPreviewDialog();
                    Application.DoEvents();
                    ParentWizard.Size = size;
                    ParentWizard.Show();
                }
                else if (button == buttonPrint)
                    Browser.ShowPrintDialog();
            }

            this.Enabled = true;
            if (this.ParentWizard is Wizard)
                this.ParentWizard.Enabled = true;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxSorting control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        private void comboBoxSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryOrder cardorder;
            QueryOrderDir cardorderdir;
            GetSelection(out cardorder, out cardorderdir);
            checkBoxReverseOrder.Enabled = cardorder != QueryOrder.None;
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo4f78.htm");
        }
    }
}

