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

using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using XPTable.Models;
using MLifter.Controls.Properties;

namespace MLifter.Controls.Wizards.Print
{
    public partial class IndividualSelectionPage : MLifter.WizardPage
    {
        private Dictionary dic;
        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        public Dictionary Dictionary
        {
            get { return dic; }
            set { dic = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndividualSelectionPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        public IndividualSelectionPage()
        {
            InitializeComponent();

            textColumnQuestion.Text = Resources.PRINT_INDIVIDUALSELECTION_QUESTION;
            textColumnAnswer.Text = Resources.PRINT_INDIVIDUALSELECTION_ANSWER;
            textColumnBox.Text = Resources.PRINT_INDIVIDUALSELECTION_BOX;
            textColumnChapter.Text = Resources.PRINT_INDIVIDUALSELECTION_CHAPTER;
        }

        /// <summary>
        /// Handles the Load event of the IndividualSelectionPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private void IndividualSelectionPage_Load(object sender, EventArgs e)
        {
            if (dic != null)
            {
                if (dic.QuestionCulture.TextInfo.IsRightToLeft)
                {
                    columnModelCards.Columns[1].Renderer = new RtlTextRenderer();
                    (columnModelCards.Columns[1].Renderer as RtlTextRenderer).FormatFlags |= TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis;
                }
                if (dic.AnswerCulture.TextInfo.IsRightToLeft)
                {
                    columnModelCards.Columns[2].Renderer = new RtlTextRenderer();
                    (columnModelCards.Columns[2].Renderer as RtlTextRenderer).FormatFlags |= TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis;
                }

                foreach (ICard card in dic.Cards.Cards)
                {
                    Row row = new Row();
                    row.Cells.Add(new Cell());
                    row.Cells.Add(new Cell(card.Question.ToString()));
                    row.Cells.Add(new Cell(card.Answer.ToString()));
                    row.Cells.Add(new Cell(card.Box > 0 ? card.Box.ToString() : card.Box == 0 ? Resources.POOL : Resources.INACTIVE));
                    row.Cells.Add(new Cell(dic.Chapters.GetChapterByID(card.Chapter).ToString()));
                    row.Tag = card;

                    tableCards.TableModel.Rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Handles the CellDoubleClick event of the tableCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellMouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private void tableCards_CellDoubleClick(object sender, XPTable.Events.CellMouseEventArgs e)
        {
            e.Cell.Row.Cells[0].Checked = !e.Cell.Row.Cells[0].Checked;
        }

        /// <summary>
        /// Handles the Click event of the buttonSelect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            foreach (Row row in tableCards.TableModel.Selections.SelectedItems)
                row.Cells[0].Checked = true;
        }

        /// <summary>
        /// Handles the Click event of the buttonRemove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach (Row row in tableCards.TableModel.Selections.SelectedItems)
                row.Cells[0].Checked = false;
        }

        public override bool GoNext()
        {
            (ParentWizard.Tag as PrintSettings).IDs.Clear();

            foreach (Row row in tableCards.TableModel.Rows)
                if (row.Cells[0].Checked)
                    (ParentWizard.Tag as PrintSettings).IDs.Add((row.Tag as ICard).Id);

            return base.GoNext();
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

