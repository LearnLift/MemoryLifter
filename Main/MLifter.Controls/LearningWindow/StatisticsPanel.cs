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
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;

namespace MLifter.Controls.LearningWindow
{
    public partial class StatisticsPanel : UserControl, ILearnUserControl
    {
        private int boxcards = 0, boxsize = 0;
        private int boxno = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsPanel"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public StatisticsPanel()
        {
            InitializeComponent();

            this.ForeColorTitle = Color.Blue;
            this.ForeColorValue = SystemColors.ControlText;
            this.FontTitle = this.Font;
            this.FontValue = this.Font;

            ToolTip ttip = new ToolTip();
            ttip.SetToolTip(labelChapter, Resources.LISTBOXFIELDS_CHAPTER);
            ttip.SetToolTip(pictureBoxChapters, Resources.LISTBOXFIELDS_CHAPTER);
        }

        LearnLogic learnlogic = null;

        /// <summary>
        /// Registers the learn logic.
        /// </summary>
        /// <param name="learnlogic">The learnlogic.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public void RegisterLearnLogic(LearnLogic learnlogic)
        {
            this.learnlogic = learnlogic;
            this.learnlogic.CardStateChanged += new LearnLogic.CardStateChangedEventHandler(learnlogic_CardStateChanged);
            this.learnlogic.CardStack.StackChanged += new EventHandler(CardStack_StackChanged);
        }

        private ToolTip tTip = new ToolTip();
        /// <summary>
        /// Handles the CardStateChanged event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-30</remarks>
        private void learnlogic_CardStateChanged(object sender, CardStateChangedEventArgs e)
        {
            if (e is CardStateChangedNewCardEventArgs ||
                (e is CardStateChangedShowResultEventArgs && ((CardStateChangedShowResultEventArgs)e).slideshow))
            {
                Card card = e.dictionary.Cards.GetCardByID(e.cardid);
                Box box = new Box(e.dictionary.Boxes[card.BaseCard.Box].Size, e.dictionary.Boxes[card.BaseCard.Box].MaximalSize);

                this.BoxNo = card.BaseCard.Box;
                this.BoxCards = box.Size;
                this.BoxSize = box.MaximalSize;

                this.Score = e.dictionary.Score;

                this.labelUser.Text = LearnLogic.UserConnectionInfo.GetDisplayName(learnlogic.User);
                tTip.RemoveAll();
                tTip.SetToolTip(this.labelUser, LearnLogic.UserConnectionInfo.GetServerName(learnlogic.CurrentLearningModule.ConnectionName));

                // [ML-2111] Not possible to display ampersand in chapter label 
                this.labelChapter.Text = e.dictionary.Chapters.GetChapterByID(card.BaseCard.Chapter).Title.Replace("&", "&&");

                this.TimerVisible = e.dictionary.Settings.EnableTimer.Value;
                this.pictureBoxTimer.Visible = this.TimerVisible;
            }
            else if (e is CardStateChangedCountdownTimerEventArgs)
            {
                CardStateChangedCountdownTimerEventArgs args = (CardStateChangedCountdownTimerEventArgs)e;
                this.Timer = args.Remaining;
            }
        }

        /// <summary>
        /// Handles the StackChanged event of the CardStack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-30</remarks>
        void CardStack_StackChanged(object sender, EventArgs e)
        {
            if (sender is CardStack)
            {
                this.RightCount = ((CardStack)sender).RightCount;
                this.WrongCount = ((CardStack)sender).WrongCount;
            }
        }

        /// <summary>
        /// Gets or sets the box number.
        /// </summary>
        /// <value>The box no.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public int BoxNo
        {
            get { return boxno; }
            set { boxno = value; UpdateBoxSize(); }
        }

        /// <summary>
        /// Updates the box number.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        private string GetBoxNo()
        {
            return boxno == 0 ? Properties.Resources.POOL : string.Format(Resources.BOX_NAME, Convert.ToString(boxno));
        }

        /// <summary>
        /// Gets or sets the amount of cards in the current box.
        /// </summary>
        /// <value>The amount of cards in the current box.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public int BoxCards
        {
            get { return boxcards; }
            set { boxcards = value; UpdateBoxSize(); }
        }

        /// <summary>
        /// Gets or sets the size of the current box.
        /// </summary>
        /// <value>The size of the current box.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public int BoxSize
        {
            get { return boxsize; }
            set { boxsize = value; UpdateBoxSize(); }
        }

        /// <summary>
        /// Updates the box size label.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        private void UpdateBoxSize()
        {
            labelBoxNumber.Text = string.Format(Resources.BOX_TEXT, GetBoxNo(), BoxCards, BoxSize);
        }

        /// <summary>
        /// Gets or sets the right count.
        /// </summary>
        /// <value>The right count.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public int RightCount
        {
            get { return Convert.ToInt32(labelRight.Text); }
            set { labelRight.Text = Convert.ToString(value); }
        }

        /// <summary>
        /// Gets or sets the wrong count.
        /// </summary>
        /// <value>The wrong count.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public int WrongCount
        {
            get { return Convert.ToInt32(labelWrong.Text); }
            set { labelWrong.Text = Convert.ToString(value); }
        }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public double Score
        {
            get { return scoreControlKnown.Score; }
            set { scoreControlKnown.Score = value; }
        }

        /// <summary>
        /// Gets or sets the timer.
        /// </summary>
        /// <value>The timer.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public int Timer
        {
            get { return Convert.ToInt32(labelTimer.Text); }
            set
            {
                labelTimer.Text = Convert.ToString(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the [timer Description is visible].
        /// </summary>
        /// <value><c>true</c> if [timer visible]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        public bool TimerVisible
        {
            get { return labelTimer.Visible; }
            set { labelTimer.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the title font.
        /// </summary>
        /// <value>The title font.</value>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        [Category("Appearance")]
        public Font FontTitle
        {
            get { return labelBoxNumber.Font; }
            set
            {
                labelBoxNumber.Font = value;
            }
        }

        /// <summary>
        /// Shoulds the serialize font title.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        public bool ShouldSerializeFontTitle()
        {
            return labelBoxNumber.Font != this.Font;
        }

        /// <summary>
        /// Resets the font title.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        public void ResetFontTitle()
        {
            labelBoxNumber.Font = this.Font;
        }

        /// <summary>
        /// Gets or sets the title fore color.
        /// </summary>
        /// <value>The fore color title.</value>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        [Category("Appearance"), DefaultValue(typeof(Color), "Blue")]
        public Color ForeColorTitle
        {
            get { return labelBoxNumber.ForeColor; }
            set
            {
                labelBoxNumber.ForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the value font.
        /// </summary>
        /// <value>The value font.</value>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        [Category("Appearance")]
        public Font FontValue
        {
            get { return labelUser.Font; }
            set
            {
                labelUser.Font =
                    labelChapter.Font =
                    labelRight.Font =
                    labelWrong.Font =
                    scoreControlKnown.Font =
                    labelTimer.Font = value;
            }
        }

        /// <summary>
        /// Shoulds the serialize font value.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        public bool ShouldSerializeFontValue()
        {
            return labelUser.Font != this.Font;
        }

        /// <summary>
        /// Resets the font value.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        public void ResetFontValue()
        {
            labelUser.Font = this.Font;
        }

        /// <summary>
        /// Gets or sets the value fore color.
        /// </summary>
        /// <value>The fore color value.</value>
        /// <remarks>Documented by Dev02, 2008-04-21</remarks>
        [Category("Appearance"), DefaultValue(typeof(Color), "ControlText")]
        public Color ForeColorValue
        {
            get { return labelUser.ForeColor; }
            set
            {
                labelUser.ForeColor =
                    labelChapter.ForeColor =
                    labelRight.ForeColor =
                    labelWrong.ForeColor =
                    scoreControlKnown.ForeColor =
                    labelTimer.ForeColor = value;
            }
        }

        private void scoreControlKnown_Load(object sender, EventArgs e)
        {

        }
    }
}
