using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;
using MLifter.Components;

namespace MLifter.Controls.LearningWindow
{
    public partial class ButtonsPanel : UserControl, ILearnUserControl
    {
        LearnLogic learnlogic = null;
        public ButtonsPanel()
        {
            InitializeComponent();
            DisableButtons();
        }

        /// <summary>
        /// Handles the CardStateChanged event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-14</remarks>
        void learnlogic_CardStateChanged(object sender, CardStateChangedEventArgs e)
        {
            if (e is CardStateChangedCountdownTimerEventArgs)
            {
            }
            else
            {

                DisableButtons();
                //-- Hide / show selfAssesment buttons
                if (e.dictionary.Settings.SelfAssessment.Value)
                    ButtonSelfAssesmentVisible(true);
                else
                    ButtonSelfAssesmentVisible(false);

                //-- Enable Buttons depending on card status
                if (e is CardStateChangedNewCardEventArgs)
                {
                    if (learnlogic.SlideShow)
                    {
                        ButtonSlideShow(true);
                    }
                    else
                    {
                        ButtonQuestion(true);
                    }
                }

                if (e is CardStateChangedShowResultEventArgs)
                {
                    //this should correspond properly to SetButtonFocus(CardStateChangedEventArgs e)

                    CardStateChangedShowResultEventArgs args = (CardStateChangedShowResultEventArgs)e;

                    if (args.slideshow)
                        ButtonSlideShow(true);
                    else
                    {
                        if (e.dictionary.Settings.SelfAssessment.Value)
                            ButtonSelfAssesment(true);
                        else
                            ButtonNext(true);
                    }
                }
            }
        }

        #region ILearnUserControl Members

        void ILearnUserControl.RegisterLearnLogic(LearnLogic learnlogic)
        {
            this.learnlogic = learnlogic;
            this.learnlogic.CardStateChanged += new LearnLogic.CardStateChangedEventHandler(learnlogic_CardStateChanged);

        }

        #endregion

        #region ShowButtons
        /// <summary>
        /// Disables the buttons.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-14</remarks>
        private void DisableButtons()
        {
            //[ML-1620] The button looses focus, but still works with keyboard
            if (this.ContainsFocus)
                buttonDummyFocus.Focus();

            ButtonSlideShow(false);
            ButtonQuestion(false);
            ButtonSelfAssesment(false);
        }
        /// <summary>
        /// Disables / Enables the button for the slideshow
        /// </summary>
        /// <param name="showCard">if set to <c>true</c> [show card].</param>
        /// <remarks>Documented by Dev07, 2009-04-14</remarks>
        private void ButtonSlideShow(bool showButton)
        {
            glassButtonPreviousCard.Enabled = showButton;
            ButtonNext(showButton);
        }
        /// <summary>
        /// Dosables / Enables the button for the self assesment
        /// </summary>
        /// <param name="showCard">if set to <c>true</c> [show card].</param>
        /// <remarks>Documented by Dev07, 2009-04-14</remarks>
        private void ButtonSelfAssesment(bool showButton)
        {
            glassButtonSelfAssesmentDontKnow.Enabled = showButton;
            glassButtonSelfAssesmentDoKnow.Enabled = showButton;
        }
        /// <summary>
        /// Buttons the self assesment visible.
        /// </summary>
        /// <param name="showButton">if set to <c>true</c> [show button].</param>
        /// <remarks>Documented by Dev07, 2009-04-22</remarks>
        private void ButtonSelfAssesmentVisible(bool showButton)
        {
            glassButtonSelfAssesmentDontKnow.Visible = showButton;
            glassButtonSelfAssesmentDoKnow.Visible = showButton;
        }
        /// <summary>
        /// Disables / Enables the button to check the answer
        /// </summary>
        /// <param name="showCard">if set to <c>true</c> [show card].</param>
        /// <remarks>Documented by Dev07, 2009-04-14</remarks>
        private void ButtonQuestion(bool showButton)
        {
            glassButtonQuestion.Enabled = showButton;
            //if (showButton)
            //     glassButtonQuestion.Focus();
        }
        /// <summary>
        /// Buttons the next.
        /// </summary>
        /// <param name="showButton">if set to <c>true</c> [show button].</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void ButtonNext(bool showButton)
        {
            glassButtonNextCard.Enabled = showButton;
        }
        #endregion Showbuttons

        #region ClickButtons
        /// <summary>
        /// Occurs when [button next clicked].
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public event EventHandler ButtonNextClicked;
        protected virtual void OnButtonNextClicked(EventArgs e)
        {
            if (ButtonNextClicked != null)
                ButtonNextClicked(this, e);
        }
        /// <summary>
        /// Handles the Click event of the glassButtonNextCard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void glassButtonNextCard_Click(object sender, EventArgs e)
        {
            previousFocusedSlideshowButton = sender as Button;
            OnButtonNextClicked(e);
        }
        /// <summary>
        /// Occurs when [button question clicked].
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public event EventHandler ButtonQuestionClicked;
        protected virtual void OnButtonQuestionClicked(EventArgs e)
        {
            if (ButtonQuestionClicked != null)
                ButtonQuestionClicked(this, e);
        }
        /// <summary>
        /// Handles the Click event of the glassButtonQuestion control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void glassButtonQuestion_Click(object sender, EventArgs e)
        {
            OnButtonQuestionClicked(e);
        }
        /// <summary>
        /// Occurs when [button self assesment dont know clicked].
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public event EventHandler ButtonSelfAssesmentDontKnowClicked;
        protected virtual void OnButtonSelfAssesmentDontKnowClicked(EventArgs e)
        {
            if (ButtonSelfAssesmentDontKnowClicked != null)
                ButtonSelfAssesmentDontKnowClicked(this, e);
        }
        /// <summary>
        /// Handles the Click event of the glassButtonSelfAssesmentDontKnow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void glassButtonSelfAssesmentDontKnow_Click(object sender, EventArgs e)
        {
            OnButtonSelfAssesmentDontKnowClicked(e);
        }
        /// <summary>
        /// Occurs when [button previous card clicked].
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public event EventHandler ButtonPreviousCardClicked;
        protected virtual void OnButtonPreviousCardClicked(EventArgs e)
        {
            if (ButtonPreviousCardClicked != null)
                ButtonPreviousCardClicked(this, e);
        }
        /// <summary>
        /// Handles the Click event of the glassButtonPreviousCard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void glassButtonPreviousCard_Click(object sender, EventArgs e)
        {
            previousFocusedSlideshowButton = sender as Button;
            OnButtonPreviousCardClicked(e);
        }
        /// <summary>
        /// Occurs when [button self assesment do know clicked].
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public event EventHandler ButtonSelfAssesmentDoKnowClicked;
        protected virtual void OnButtonSelfAssesmentDoKnowClicked(EventArgs e)
        {
            if (ButtonSelfAssesmentDoKnowClicked != null)
                ButtonSelfAssesmentDoKnowClicked(this, e);
        }
        /// <summary>
        /// Handles the Click event of the glassButtonSelfAssesmentDoKnow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void glassButtonSelfAssesmentDoKnow_Click(object sender, EventArgs e)
        {
            OnButtonSelfAssesmentDoKnowClicked(e);
        }
        #endregion ClickButtons

        #region Button focus

        /// <summary>
        /// Sets the button focus.
        /// </summary>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.CardStateChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-05-07</remarks>
        public void SetButtonFocus(CardStateChangedEventArgs e)
        {
            if (e is CardStateChangedShowResultEventArgs)
            {
                CardStateChangedShowResultEventArgs args = (CardStateChangedShowResultEventArgs)e;

                if (args.slideshow)
                {
                    FocusSlideshowButton();
                }
                else
                {
                    if (e.dictionary.Settings.SelfAssessment.Value)
                    {
                        if (args.result == AnswerResult.Correct)
                            SelfAssesmentFocusDoKnow();
                        else
                            SelfAssesmentFocusDontKnow();
                    }
                    else
                    {
                        FocusSlideshowButton();
                    }
                }
            }
        }

        /// <summary>
        /// Selfs the assesment focus do know.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void SelfAssesmentFocusDoKnow()
        {
            glassButtonSelfAssesmentDoKnow.Focus();
        }
        /// <summary>
        /// Selfs the assesment focus dont know.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void SelfAssesmentFocusDontKnow()
        {
            glassButtonSelfAssesmentDontKnow.Focus();
        }

        Button previousFocusedSlideshowButton = null;
        /// <summary>
        /// Focuses the slideshow button.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-05-07</remarks>
        public void FocusSlideshowButton()
        {
            if (previousFocusedSlideshowButton == null)
                previousFocusedSlideshowButton = glassButtonNextCard;

            previousFocusedSlideshowButton.Focus();
        }
        /// <summary>
        /// Selfs the assesment click do know.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void SelfAssesmentClickDoKnow()
        {
            glassButtonSelfAssesmentDoKnow.PerformClick();
        }
        /// <summary>
        /// Selfs the assesment click dont know.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        public void SelfAssesmentClickDontKnow()
        {
            glassButtonSelfAssesmentDontKnow.PerformClick();
        }
        /// <summary>
        /// Handles the KeyDown event of the glassButtonSelfAssesmentDontKnow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-20</remarks>
        private void glassButtonSelfAssesmentDontKnow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && glassButtonSelfAssesmentDontKnow.Enabled == true)
            {
                SelfAssesmentClickDontKnow();
            }
            else if (e.KeyCode == Keys.Up && glassButtonSelfAssesmentDoKnow.Enabled == true)
            {
                SelfAssesmentClickDoKnow();
            }
        }
        /// <summary>
        /// Handles the KeyDown event of the glassButtonSelfAssesmentDoKnow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-20</remarks>
        private void glassButtonSelfAssesmentDoKnow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && glassButtonSelfAssesmentDoKnow.Enabled == true)
            {
                SelfAssesmentClickDoKnow();
            }
            else if (e.KeyCode == Keys.Down && glassButtonSelfAssesmentDontKnow.Enabled == true)
            {
                SelfAssesmentClickDontKnow();
            }
        }
        #endregion SelfAssesment


        public void UpdateCulture()
        {
            glassButtonSelfAssesmentDoKnow.ToolTipText = new ComponentResourceManager(this.GetType()).GetString(glassButtonSelfAssesmentDoKnow.Name + ".ToolTipText");
            glassButtonSelfAssesmentDontKnow.ToolTipText = new ComponentResourceManager(this.GetType()).GetString(glassButtonSelfAssesmentDontKnow.Name + ".ToolTipText");
            glassButtonQuestion.ToolTipText = new ComponentResourceManager(this.GetType()).GetString(glassButtonQuestion.Name + ".ToolTipText");
            glassButtonPreviousCard.ToolTipText = new ComponentResourceManager(this.GetType()).GetString(glassButtonPreviousCard.Name + ".ToolTipText");
            glassButtonNextCard.ToolTipText = new ComponentResourceManager(this.GetType()).GetString(glassButtonNextCard.Name + ".ToolTipText");
        }
        private GlassButton lastButton = null;
        /// <summary>
        /// Handles the Enter event of the buttonDummyFocus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-05-18</remarks>
        private void buttonDummyFocus_Enter(object sender, EventArgs e)
        {
            if (lastButton != null && lastButton.Enabled)
            {
                if (lastButton == glassButtonNextCard && glassButtonPreviousCard.Enabled)
                    glassButtonPreviousCard.Focus();
                else if (glassButtonNextCard.Enabled)
                    glassButtonNextCard.Focus();
                else
                    lastButton.Focus();
            }
        }
        /// <summary>
        /// Handles the Leave event of the glassButtonPreviousCard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-05-18</remarks>
        private void glassButtonPreviousCard_Leave(object sender, EventArgs e)
        {
            lastButton = glassButtonPreviousCard;
        }
        /// <summary>
        /// Handles the Leave event of the glassButtonNextCard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-05-18</remarks>
        private void glassButtonNextCard_Leave(object sender, EventArgs e)
        {
            lastButton = glassButtonNextCard;
        }
    }
}
