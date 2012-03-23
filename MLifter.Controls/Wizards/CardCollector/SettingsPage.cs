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

namespace MLifter.Controls.Wizards.CardCollector
{
    public partial class SettingsPage : MLifter.WizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-14</remarks>
        public SettingsPage()
        {
            InitializeComponent();

            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_QUESTION);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_QUESTIONIMAGE);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_QUESTIONAUDIO);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_QUESTIONVIDEO);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_QUESTIONEXAMPLE);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_QUESTIONEXAMPLEAUDIO);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_ANSWER);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_ANSWERIMAGE);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_ANSWERAUDIO);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_ANSWERVIDEO);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_ANSWEREXAMPLE);
            listViewElements.Items.Add(MLifter.Controls.Properties.Resources.CARDCOLLECTOR_COLUMN_ANSWEREXAMPLEAUDIO);

            listViewElements.Items[0].Tag = DAL.Helper.CardFields.Question;
            listViewElements.Items[1].Tag = DAL.Helper.CardFields.QuestionImage;
            listViewElements.Items[2].Tag = DAL.Helper.CardFields.QuestionAudio;
            listViewElements.Items[3].Tag = DAL.Helper.CardFields.QuestionVideo;
            listViewElements.Items[4].Tag = DAL.Helper.CardFields.QuestionExample;
            listViewElements.Items[5].Tag = DAL.Helper.CardFields.QuestionExampleAudio;
            listViewElements.Items[6].Tag = DAL.Helper.CardFields.Answer;
            listViewElements.Items[7].Tag = DAL.Helper.CardFields.AnswerImage;
            listViewElements.Items[8].Tag = DAL.Helper.CardFields.AnswerAudio;
            listViewElements.Items[9].Tag = DAL.Helper.CardFields.AnswerVideo;
            listViewElements.Items[10].Tag = DAL.Helper.CardFields.AnswerExample;
            listViewElements.Items[11].Tag = DAL.Helper.CardFields.AnswerExampleAudio;

            listViewElements.Items[0].Checked = true;
            listViewElements.Items[6].Checked = true;
            listViewElements.Items[0].Selected = true;
        }

        /// <summary>
        /// Handles the Load event of the SettingsPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-14</remarks>
        private void SettingsPage_Load(object sender, EventArgs e)
        {
            //Used to avoid miscalculation because of wrong count of checked items.
            listViewElements.Items[0].Checked = !listViewElements.Items[0].Checked;
            listViewElements.Items[0].Checked = !listViewElements.Items[0].Checked;
        }

        /// <summary>
        /// Handles the ItemChecked event of the listViewElements control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ItemCheckedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-17</remarks>
        private void listViewElements_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (listViewElements.CheckedItems.Count == 0)
                NextAllowed = false;
            else
                NextAllowed = true;
        }

        /// <summary>
        /// Handles the Click event of the buttonUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void buttonUp_Click(object sender, EventArgs e)
        {
            MoveListViewItem(true);
        }

        /// <summary>
        /// Handles the Click event of the buttonDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            MoveListViewItem(false);
        }

        /// <summary>
        /// Moves the selected list view item.
        /// </summary>
        /// <param name="up">if set to <c>true</c> [up], else [down].</param>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void MoveListViewItem(bool up)
        {
            foreach (ListViewItem lvi in listViewElements.SelectedItems)
            {
                int oldindex = lvi.Index;
                int newindex = up ? oldindex - 1 : oldindex + 1;
                if (newindex > -1 && newindex < listViewElements.Items.Count)
                {
                    listViewElements.Items.RemoveAt(oldindex);
                    listViewElements.Items.Insert(newindex, lvi);
                }
            }
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/Collect.htm");
        }
    }
}

