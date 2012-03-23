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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MLifter.DAL;
using MLifter.DAL.XML;
using MLifter.DAL.Interfaces;
using MLifter.BusinessLayer;

namespace MLifter.Controls
{
    public partial class CardStyleEdit : UserControl
    {
        private object styleParent;
        private ICard card;
        private ICardStyle style;
        private ICardStyle backupStyle;
        private Dictionary dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardStyleEdit"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        public CardStyleEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Resize event of the CardStyleEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void CardStyleEdit_Resize(object sender, EventArgs e)
        {
            tabControlMain.ItemSize = new Size((tabControlMain.Width - 2) / tabControlMain.TabCount, tabControlMain.ItemSize.Height);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the tabControlMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPreview();
        }

        /// <summary>
        /// Initializes the specified actual card.
        /// </summary>
        /// <param name="actualCard">The actual card.</param>
        /// <param name="actualDictionary">The actual dictionary.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        public void Initialize(ICard actualCard, ICardStyle actualStyle, Dictionary actualDictionary, object elementToStyle)
        {
            styleParent = elementToStyle;
            dictionary = actualDictionary;
            style = actualStyle;
            backupStyle = style.Clone();
            card = actualCard;

            textStyleEditAnswer.Style = style.Answer;
            textStyleEditAnswerExample.Style = style.AnswerExample;
            textStyleEditCorrect.Style = style.AnswerCorrect;
            textStyleEditQuestion.Style = style.Question;
            textStyleEditQuestionExample.Style = style.QuestionExample;
            textStyleEditWrong.Style = style.AnswerWrong;

            ShowPreview();
        }

        /// <summary>
        /// Handles the Changed event of the CardStyle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void CardStyle_Changed(object sender, EventArgs e)
        {
            SaveChanges(false);
            ShowPreview();
        }

        /// <summary>
        /// Shows the preview.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void ShowPreview()
        {
            bool useStylesheets = dictionary.UseDictionaryStyleSheets;
            dictionary.UseDictionaryStyleSheets = true;

            if (card != null)
            {
                if (tabControlMain.SelectedIndex < 2)
                {
                    stylePreview.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareQuestion(dictionary.DictionaryDAL.Parent, card.Id,
                        dictionary.GenerateCard(new Card(card, dictionary), Side.Question, String.Empty, true));
                }
                else
                {
                    stylePreview.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareAnswer(dictionary.DictionaryDAL.Parent, card.Id,
                        dictionary.GenerateCard(new Card(card, dictionary), Side.Answer, Properties.Resources.XSLEDIT_USERANSWER, tabControlMain.SelectedIndex < 5));
                }
            }

            dictionary.UseDictionaryStyleSheets = useStylesheets;
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="saveToDictionary">if set to <c>true</c> [save to dictionary].</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        public void SaveChanges(bool saveToDictionary)
        {
            if (styleParent is ICard)
                card.Settings.Style = style;
            else if (styleParent is IChapter)
                (styleParent as IChapter).Settings.Style = style;
            else if (styleParent is IDictionary)
                (styleParent as IDictionary).DefaultSettings.Style = style;

            if (saveToDictionary)
                dictionary.Save();
        }

        /// <summary>
        /// Rollbacks the changes.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        public void RollbackChanges()
        {
            if (styleParent is ICard)
                card.Settings.Style = backupStyle;
            else if (styleParent is IChapter)
                (styleParent as IChapter).Settings.Style = backupStyle;
            else if (styleParent is IDictionary)
                (styleParent as IDictionary).DefaultSettings.Style = backupStyle;

            //dictionary.Save();
        }
    }
}
