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
using MLifter.Controls.Properties;

namespace MLifter.Controls
{
    public partial class CardMultiEdit : MLifter.Controls.CardEdit
    {
        private int[] cards;
        public int[] SelectedCards { get { return cards; } }

        /// <summary>
        /// Occurs when a card is edited.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public event EventHandler MultiEdit;
        /// <summary>
        /// Raises the <see cref="E:MultiEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        protected virtual void OnMultiEdit(EventArgs e)
        {
            if (MultiEdit != null)
                MultiEdit(this, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardMultiEdit"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        /// <remarks>Documented by Dev05, 2007-10-12</remarks>
        public CardMultiEdit()
        {
            InitializeComponent();
            Multiselect = true;

            buttonAddEdit.Click += new EventHandler(buttonAddEdit_Click);
            comboBoxCardBox.TextUpdate += new EventHandler(comboBoxCardBox_TextUpdate);
        }

        /// <summary>
        /// Handles the TextUpdate event of the comboBoxCardBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-19</remarks>
        void comboBoxCardBox_TextUpdate(object sender, EventArgs e)
        {
            //when in multiple selection mode, don't allow any other Text
            if (Multiselect && comboBoxCardBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                //deactivated because of [ML-838]: the SelectedIndex property gets resetted to -1 until ModifyMultipleCards
                //if (comboBoxCardBox.Items.Contains(comboBoxCardBox.Text))
                //{
                //    comboBoxCardBox.SelectedIndex = comboBoxCardBox.Items.IndexOf(comboBoxCardBox.Text);
                //}
                //else
                //{
                //    comboBoxCardBox.Text = Resources.MAINTAIN_UNCHANGED;
                //}

                comboBoxCardBox.Text = Resources.MAINTAIN_UNCHANGED;
            }
        }

        /// <summary>
        /// Loads the multiple cards.
        /// </summary>
        /// <param name="cardIDs">The card I ds.</param>
        /// <remarks>Documented by Dev05, 2007-10-12</remarks>
        public void LoadMultipleCards(int[] cardIDs)
        {
            CheckModified();

            Multiselect = true;
            cards = cardIDs;

            buttonAddEdit.Text = Resources.MAINTAIN_CHANGEALL;

            if (!comboBoxChapter.Items.Contains(Resources.MAINTAIN_UNCHANGED))
            {
                comboBoxChapter.Items.Insert(0, Resources.MAINTAIN_UNCHANGED);
                comboBoxChapter.SelectedIndex = 0;
            }

            checkBoxActive.CheckState = CheckState.Indeterminate;
            checkBoxSamePicture.CheckState = CheckState.Indeterminate;

            comboBoxCardBox.DropDownStyle = ComboBoxStyle.DropDown;
            comboBoxCardBox.Text = Resources.MAINTAIN_UNCHANGED;

            pictureBoxAnswer.Image = null;
            pictureBoxAnswer.Tag = new PreviousValueDummy();
            pictureBoxQuestion.Image = null;
            pictureBoxQuestion.Tag = new PreviousValueDummy();

            buttonQuestionAudio.Image = Properties.Resources.Audio;
            buttonQuestionAudio.Tag = new PreviousValueDummy();
            buttonQuestionExampleAudio.Image = Properties.Resources.Audio;
            buttonQuestionExampleAudio.Tag = new PreviousValueDummy();
            buttonQuestionVideo.Image = Properties.Resources.Video;
            buttonQuestionVideo.Tag = new PreviousValueDummy();
            buttonAnswerAudio.Image = Properties.Resources.Audio;
            buttonAnswerAudio.Tag = new PreviousValueDummy();
            buttonAnswerExampleAudio.Image = Properties.Resources.Audio;
            buttonAnswerExampleAudio.Tag = new PreviousValueDummy();
            buttonAnswerVideo.Image = Properties.Resources.Video;
            buttonAnswerVideo.Tag = new PreviousValueDummy();

            SetWord(Side.Question, Resources.MAINTAIN_UNCHANGED);
            SetWord(Side.Answer, Resources.MAINTAIN_UNCHANGED);

            textBoxQuestionExample.Text = Resources.MAINTAIN_UNCHANGED;
            textBoxAnswerExample.Text = Resources.MAINTAIN_UNCHANGED;

            Modified = false;
        }

        /// <summary>
        /// Handles the Click event of the buttonAddEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        /// <remarks>Documented by Dev05, 2007-10-12</remarks>
        private void buttonAddEdit_Click(object sender, EventArgs e)
        {
            ModifyMultipleCards();
        }

        /// <summary>
        /// Modifies the multiple cards.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-12</remarks>
        private void ModifyMultipleCards()
        {
            if (!Multiselect)
                return;

            Cursor = Cursors.WaitCursor;
            LoadStatusMessage statusMessage = new LoadStatusMessage(string.Format(Properties.Resources.CARDEDIT_STATUS_SAVING, cards.Length), cards.Length, true);
            statusMessage.Show();

            string[] question = GetWord(Side.Question).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] answer = GetWord(Side.Answer).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string questionExample = textBoxQuestionExample.Text;
            string answerExample = textBoxAnswerExample.Text;

            foreach (int id in cards)
            {
                Dictionary.Cards.ChangeCardOnDifference(id, question, answer, questionExample, answerExample,
                    (comboBoxChapter.SelectedItem is IChapter ? ((IChapter)comboBoxChapter.SelectedItem).Id : -1), Resources.MAINTAIN_UNCHANGED);

                ICard card = Dictionary.Cards.GetCardByID(id).BaseCard;
                if (checkBoxActive.CheckState != CheckState.Indeterminate)
                    if (card.Active != checkBoxActive.Checked)
                        card.Active = checkBoxActive.Checked;

                if (comboBoxCardBox.SelectedIndex != -1 && card.Active)
                {
                    card.Box = comboBoxCardBox.SelectedIndex;
                }

                //Save old Media objects
                Dictionary<string, IMedia> oldMedia = new Dictionary<string, IMedia>();
                IMedia media = null;
                if ((media = Dictionary.Cards.GetImageObject(card, Side.Answer, true)) != null)
                    oldMedia.Add("answerImage", media);
                if ((media = Dictionary.Cards.GetImageObject(card, Side.Question, true)) != null)
                    oldMedia.Add("questionImage", media);
                if ((media = Dictionary.Cards.GetAudioObject(card, Side.Answer, true, false, true)) != null)
                    oldMedia.Add("answerAudio", media);
                if ((media = Dictionary.Cards.GetAudioObject(card, Side.Question, true, false, true)) != null)
                    oldMedia.Add("questionAudio", media);
                if ((media = Dictionary.Cards.GetAudioObject(card, Side.Answer, false, true, true)) != null)
                    oldMedia.Add("answerExample", media);
                if ((media = Dictionary.Cards.GetAudioObject(card, Side.Question, false, true, true)) != null)
                    oldMedia.Add("questionExample", media);
                if ((media = Dictionary.Cards.GetVideoObject(card, Side.Answer, true)) != null)
                    oldMedia.Add("answerVideo", media);
                if ((media = Dictionary.Cards.GetVideoObject(card, Side.Question, true)) != null)
                    oldMedia.Add("questionVideo", media);

                card.ClearAllMedia(false);

                //Image
                if (pictureBoxAnswer.Tag as IImage != null && checkBoxSamePicture.CheckState != CheckState.Unchecked)
                    card.AddMedia(pictureBoxAnswer.Tag as IMedia, Side.Answer);
                else if (pictureBoxAnswer.Tag is PreviousValueDummy && oldMedia.ContainsKey("answerImage") && checkBoxSamePicture.CheckState != CheckState.Unchecked)
                    card.AddMedia(oldMedia["answerImage"], Side.Answer);
                if (pictureBoxQuestion.Tag as IImage != null)
                {
                    IMedia image = pictureBoxQuestion.Tag as IMedia;
                    card.AddMedia(image, Side.Question);
                    if (checkBoxSamePicture.CheckState == CheckState.Checked)
                        card.AddMedia(image, Side.Answer);
                }
                else if (pictureBoxQuestion.Tag is PreviousValueDummy && oldMedia.ContainsKey("questionImage"))
                {
                    IMedia image = oldMedia["questionImage"];
                    card.AddMedia(image, Side.Question);
                    if (checkBoxSamePicture.CheckState == CheckState.Checked)
                        card.AddMedia(image, Side.Answer);
                }

                //Audio
                if (buttonAnswerAudio.Tag as IAudio != null)
                    card.AddMedia(buttonAnswerAudio.Tag as IMedia, Side.Answer);
                else if (buttonAnswerAudio.Tag is PreviousValueDummy && oldMedia.ContainsKey("answerAudio"))
                    card.AddMedia(oldMedia["answerAudio"], Side.Answer);
                if (buttonAnswerExampleAudio.Tag as IAudio != null)
                    card.AddMedia(buttonAnswerExampleAudio.Tag as IMedia, Side.Answer);
                else if (buttonAnswerExampleAudio.Tag is PreviousValueDummy && oldMedia.ContainsKey("answerExample"))
                    card.AddMedia(oldMedia["answerExample"], Side.Answer);
                if (buttonQuestionAudio.Tag as IAudio != null)
                    card.AddMedia(buttonQuestionAudio.Tag as IMedia, Side.Question);
                else if (buttonQuestionAudio.Tag is PreviousValueDummy && oldMedia.ContainsKey("questionAudio"))
                    card.AddMedia(oldMedia["questionAudio"], Side.Question);
                if (buttonQuestionExampleAudio.Tag as IAudio != null)
                    card.AddMedia(buttonQuestionExampleAudio.Tag as IMedia, Side.Question);
                else if (buttonQuestionExampleAudio.Tag is PreviousValueDummy && oldMedia.ContainsKey("questionExample"))
                    card.AddMedia(oldMedia["questionExample"], Side.Question);

                //Video
                if (buttonAnswerVideo.Tag as IVideo != null)
                    card.AddMedia(buttonAnswerVideo.Tag as IMedia, Side.Answer);
                else if (buttonAnswerVideo.Tag is PreviousValueDummy && oldMedia.ContainsKey("answerVideo"))
                    card.AddMedia(oldMedia["answerVideo"], Side.Answer);
                if (buttonQuestionVideo.Tag as IVideo != null)
                    card.AddMedia(buttonQuestionVideo.Tag as IMedia, Side.Question);
                else if (buttonQuestionVideo.Tag is PreviousValueDummy && oldMedia.ContainsKey("questionVideo"))
                    card.AddMedia(oldMedia["questionVideo"], Side.Question);

                statusMessage.ProgressStep();
            }

            statusMessage.Close();

            Modified = false;
            OnMultiEdit(EventArgs.Empty);
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Edits the card style.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        protected override void EditCardStyle()
        {
            if (Multiselect)
            {
                ICard card = Dictionary.Cards.GetCardByID(cards[0]).BaseCard;
                if (card.Settings.Style == null)
                    card.Settings.Style = card.CreateCardStyle();
                CardStyleEditor editor = new CardStyleEditor();
                editor.HelpNamespace = this.HelpNamespace;
                editor.LoadStyle(card, card.Settings.Style, Dictionary, card);
                switch (editor.ShowDialog())
                {
                    case DialogResult.Abort:
                        foreach (int id in cards)
                            Dictionary.Cards.GetCardByID(id).BaseCard.Settings.Style = null;
                        break;
                    case DialogResult.OK:
                    default:
                        foreach (int id in cards)
                            Dictionary.Cards.GetCardByID(id).BaseCard.Settings.Style = card.Settings.Style;
                        break;
                }
            }
            else
                base.EditCardStyle();
        }

        /// <summary>
        /// Checks the modified.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-12</remarks>
        public override void CheckModified()
        {
            if (!Multiselect)
                base.CheckModified();
            else
                if (Modified)
                {
                    Modified = false;
                    if (MessageBox.Show(Properties.Resources.CARDEDIT_SAVE_TEXT, Properties.Resources.CARDEDIT_SAVE_CAPION, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                        ModifyMultipleCards();
                }
        }
    }

    /// <summary>
    /// A plcaholder class to symbolize that the previous value should be inserted again.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-08-11</remarks>
    public class PreviousValueDummy
    { }
}

