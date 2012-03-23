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
using MLifter.Components;

namespace MLifter.Controls.LearningWindow
{
    /// <summary>
    /// The main learning window.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-04-22</remarks>
    public partial class LearningWindow : UserControl, ILearnUserControl
    {
        private LearnLogic learnlogic = null;
        private int stackHeight = -1;
        private int windowWidth = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningWindow"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public LearningWindow()
        {
            InitializeComponent();

            splitContainerHorizontal.Panel1MinSize = Settings.Default.QuestionAnswerPanelMinHeight;
            dockingButtonStack.Text = StackVisible ? Resources.DOCKING_BUTTON_STACK_HIDE : Resources.DOCKING_BUTTON_STACK_SHOW;
            if (StackVisible)
                stackHeight = splitContainerHorizontal.Height - splitContainerHorizontal.SplitterDistance;
            windowWidth = splitContainerVertical.Width - splitContainerVertical.SplitterDistance;

            this.VisibleChanged += new EventHandler(LearningWindow_VisibleChanged);

            answerPanel.FileDropped += new FileDroppedEventHandler(Panel_FileDropped);
            questionPanel.FileDropped += new FileDroppedEventHandler(Panel_FileDropped);

            answerPanel.BrowserKeyDown += new PreviewKeyDownEventHandler(Panel_BrowserKeyDown);
            questionPanel.BrowserKeyDown += new PreviewKeyDownEventHandler(Panel_BrowserKeyDown);

            buttonsPanelMainControl.ButtonNextClicked += new EventHandler(buttonsPanelMainControl_ButtonNextClicked);
            buttonsPanelMainControl.ButtonPreviousCardClicked += new EventHandler(buttonsPanelMainControl_ButtonPreviousCardClicked);
            buttonsPanelMainControl.ButtonQuestionClicked += new EventHandler(buttonsPanelMainControl_ButtonQuestionClicked);
            buttonsPanelMainControl.ButtonSelfAssesmentDontKnowClicked += new EventHandler(buttonsPanelMainControl_ButtonSelfAssesmentDontKnowClicked);
            buttonsPanelMainControl.ButtonSelfAssesmentDoKnowClicked += new EventHandler(buttonsPanelMainControl_ButtonSelfAssesmentDoKnowClicked);

            answerPanel.SelfAssesmentKeyUp += new EventHandler(answerPanel_SelfAssesmentButtonDoKnowClick);
            answerPanel.SelfAssesmentKeyDown += new EventHandler(answerPanel_SelfAssesmentButtonDontKnowClick);
            answerPanel.SetButtonFocus += new LearnLogic.CardStateChangedEventHandler(answerPanel_SetButtonFocus);
        }

        public void UpdateCulture()
        {
            answerPanel.UpdateCulture();
            buttonsPanelMainControl.UpdateCulture();
            dockingButtonStack.Text = new ComponentResourceManager(this.GetType()).GetString(dockingButtonStack.Name + ".Text");
        }

        void answerPanel_SetButtonFocus(object sender, CardStateChangedEventArgs e)
        {
            buttonsPanelMainControl.SetButtonFocus(e);
        }

        private void answerPanel_SelfAssesmentButtonDontKnowClick(object sender, EventArgs e)
        {
            buttonsPanelMainControl.SelfAssesmentClickDontKnow();
        }

        private void answerPanel_SelfAssesmentButtonDoKnowClick(object sender, EventArgs e)
        {
            buttonsPanelMainControl.SelfAssesmentClickDoKnow();
        }

        /// <summary>
        /// Handles the ButtonSelfAssesmentDoKnowClicked event of the buttonsPanelMainControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void buttonsPanelMainControl_ButtonSelfAssesmentDoKnowClicked(object sender, EventArgs e)
        {
            answerPanel.SubmitSelfAssessmentReponse(true, false);
        }

        private void buttonsPanelMainControl_ButtonSelfAssesmentDontKnowClicked(object sender, EventArgs e)
        {
            answerPanel.SubmitSelfAssessmentReponse(false, false);
        }

        /// <summary>
        /// Handles the ButtonQuestionClicked event of the buttonsPanelMainControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void buttonsPanelMainControl_ButtonQuestionClicked(object sender, EventArgs e)
        {
            answerPanel.CheckAnswer();
        }

        private void buttonsPanelMainControl_ButtonPreviousCardClicked(object sender, EventArgs e)
        {
            questionPanel.showPreviousCard();
        }


        /// <summary>
        /// Handles the ButtonNextClicked event of the buttonsPanelMainControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-04-16</remarks>
        private void buttonsPanelMainControl_ButtonNextClicked(object sender, EventArgs e)
        {
            answerPanel.ShowNextCard();
        }

        /// <summary>
        /// Handles the VisibleChanged event of the LearningWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        private void LearningWindow_VisibleChanged(object sender, EventArgs e)
        {
            //ensure that the learning window always is at the top of the z order
            if (this.Visible)
                this.BringToFront();
            else
                MLifter.Components.InfoBar.CleanUpInfobars(splitContainerHorizontal.Panel1);
        }

        #region LearnLogic Registration

        /// <summary>
        /// Registers the learn logic.
        /// </summary>
        /// <param name="learnlogic">The learnlogic.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public void RegisterLearnLogic(LearnLogic learnlogic)
        {
            if (this.learnlogic == null || this.learnlogic != learnlogic)
            {
                this.learnlogic = learnlogic;
                RegisterLearnLogicAllControls(this.Controls);
                RegisterLearnLogicAllComponents(this.components.Components);
                this.Visible = learnlogic.DictionaryReadyForLearning;
                this.learnlogic.LearningModuleClosed += new EventHandler(learnlogic_LearningModuleClosed);
                this.learnlogic.LearningModuleOptionsChanged += new EventHandler(learnlogic_LearningModuleOptionsChanged);
                this.learnlogic.UserDialog += new LearnLogic.UserDialogEventHandler(learnlogic_UserDialog);
                this.learnlogic.OnLearningModuleOptionsChanged();
            }
        }

        /// <summary>
        /// Handles the LearningModuleOptionsChanged event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        private void learnlogic_LearningModuleOptionsChanged(object sender, EventArgs e)
        {
            this.Visible = learnlogic.DictionaryReadyForLearning;
            MLifter.Components.InfoBar.CleanUpInfobars(this);
            MLifter.Components.InfoBar.CleanUpInfobars(answerPanel.InfoBarControl);
        }

        /// <summary>
        /// Handles the LearningModuleClosed event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        private void learnlogic_LearningModuleClosed(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        /// <summary>
        /// Registers the learn logic all controls.
        /// </summary>
        /// <param name="controls">The controls.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        private void RegisterLearnLogicAllControls(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                RegisterLearnLogicObject(control);

                if (control.Controls.Count > 0)
                    RegisterLearnLogicAllControls(control.Controls);
            }
        }

        /// <summary>
        /// Registers the learn logic all components.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        private void RegisterLearnLogicAllComponents(ComponentCollection components)
        {
            foreach (Component component in components)
                RegisterLearnLogicObject(component);
        }

        /// <summary>
        /// Registers the learn logic object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="type">The type.</param>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        private void RegisterLearnLogicObject(Object obj)
        {
            //check for each object if it has a "RegisterLearnLogic"-Method, and invoke it, if yes
            //System.Reflection.MethodInfo registermethod = obj.GetType().GetMethod("RegisterLearnLogic");
            //if (registermethod != null && registermethod.GetParameters().Length == 1)
            //    if (registermethod.GetParameters()[0].Name == "learnlogic" && registermethod.GetParameters()[0].ParameterType == typeof(LearnLogic))
            //        registermethod.Invoke(obj, new object[] { learnlogic });

            //check for each object if it implements the ILearnUserControl interface
            if (obj is ILearnUserControl)
                ((ILearnUserControl)obj).RegisterLearnLogic(learnlogic);
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether [stack visible].
        /// </summary>
        /// <value><c>true</c> if [stack visible]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-04-24</remarks>
        [DefaultValue(typeof(bool), "True")]
        public bool StackVisible
        {
            get
            {
                return !splitContainerHorizontal.Panel2Collapsed;
            }
            set
            {
                splitContainerHorizontal.Panel2Collapsed = !value;
                dockingButtonStack.Text = StackVisible ? Resources.DOCKING_BUTTON_STACK_HIDE : Resources.DOCKING_BUTTON_STACK_SHOW;
            }
        }

        /// <summary>
        /// Gets or sets the "layout values" (Splitter Positions).
        /// If the stack if hidden, it returns the old splitter value multiplied with -1.
        /// </summary>
        /// <value>The layout values.</value>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        [Browsable(false), ReadOnly(true)]
        public Size LayoutValues
        {
            get
            {
                if (!this.Visible)
                    return new Size(windowWidth, stackHeight);

                //bool wasVisible = this.Visible;
                //this.Visible = true;

                this.SuspendLayout();
                //control must be visible for the layout logic to work (performlayout does not seem to suffice)

                int width, height;
                if (StackVisible)
                    stackHeight = height = splitContainerHorizontal.Height - splitContainerHorizontal.SplitterDistance;
                else
                    height = stackHeight;

                windowWidth = width = splitContainerVertical.Width - splitContainerVertical.SplitterDistance;

                //if (!StackVisible)
                //    height = height * (-1);

                this.ResumeLayout();
                //this.Visible = wasVisible;
                return new Size(width, height);
            }
            set
            {
                //control must be visible for the layout logic to work (performlayout does not seem to suffice)

                bool wasVisible = this.Visible;
                this.Visible = true;

                this.SuspendLayout();
                int horizontalSplitterDistance = splitContainerHorizontal.Height - Math.Abs(value.Height);
                int verticalSplitterDistance = splitContainerVertical.Width - value.Width;

                if (horizontalSplitterDistance > 0 && horizontalSplitterDistance < splitContainerHorizontal.Height)
                    splitContainerHorizontal.SplitterDistance = horizontalSplitterDistance;

                if (verticalSplitterDistance > 0 && verticalSplitterDistance < splitContainerVertical.Width)
                    splitContainerVertical.SplitterDistance = verticalSplitterDistance;

                if (value.Height < 0)
                    StackVisible = false;

                stackHeight = value.Height;

                this.ResumeLayout();
                this.Visible = wasVisible;
            }
        }

        /// <summary>
        /// Occurs when [file dropped].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        [Description("Occurs when a file was dropped onto this control.")]
        public event FileDroppedEventHandler FileDropped;

        /// <summary>
        /// Raises the <see cref="E:FileDropped"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MLifter.Controls.LearningWindow.FileDroppedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void OnFileDropped(FileDroppedEventArgs e)
        {
            if (FileDropped != null)
                FileDropped(this, e);
        }

        /// <summary>
        /// Handles the FileDropped event of the Panel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.Controls.LearningWindow.FileDroppedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void Panel_FileDropped(object sender, FileDroppedEventArgs e)
        {
            OnFileDropped(e);
        }

        /// <summary>
        /// Occurs when [browser key down].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        public event PreviewKeyDownEventHandler BrowserKeyDown;

        /// <summary>
        /// Handles the BrowserKeyDown event of the Panel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        void Panel_BrowserKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //[ML-1568]  Hitting 'Enter' should always go to the next card
            if (e.KeyCode == Keys.Enter && answerPanel.ResultPageVisible)
                answerPanel.ShowNextCard();

            if (BrowserKeyDown != null)
                BrowserKeyDown(sender, e);
        }

        /// <summary>
        /// Sets the stack card back colors.
        /// </summary>
        /// <param name="styleHandler">The style handler.</param>
        /// <remarks>Documented by Dev02, 2008-05-09</remarks>
        public void SetStackCardBackColors(MLifter.Components.StyleHandler styleHandler)
        {
            stackFlow.SetStackCardBackColors(styleHandler);
        }

        /// <summary>
        /// Handles the UserDialog event of the learnlogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.BusinessLayer.UserDialogEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-21</remarks>
        void learnlogic_UserDialog(object sender, UserDialogEventArgs e)
        {
            if (e is UserNotifyDialogEventArgs)
            {
                UserNotifyDialogEventArgs args = (UserNotifyDialogEventArgs)e;
                string message = string.Empty;
                bool dontShowAgainMessage = false;
                switch (args.dialogkind)
                {
                    case UserNotifyDialogEventArgs.NotifyDialogKind.PoolEmpty:
                        message = Properties.Resources.POOL_EMPTY_TEXT;
                        break;
                    case UserNotifyDialogEventArgs.NotifyDialogKind.NotEnoughMultipleChoices:
                        message = string.Format(Properties.Resources.MULTIPLE_CHOICE_TEXT, e.dictionary.Settings.MultipleChoiceOptions.NumberOfChoices.Value);
                        break;
                    case UserNotifyDialogEventArgs.NotifyDialogKind.SelectedLearnModeNotAllowed:
                        if (Settings.Default.DontShowSelectedLearnModeNotAllowedMessage)
                            break;

                        dontShowAgainMessage = true;
                        //[ML-2115] LearnModes for Infobar not localized 
                        message = string.Format(Properties.Resources.SELECTED_LEARNMODE_NOT_ALLOWED,
                            (e.OptionalParamter is MLifter.BusinessLayer.LearnModes) ? Resources.ResourceManager.GetString("LEARNING_MODE_" + e.OptionalParamter.ToString().ToUpper()) : e.OptionalParamter);
                        break;
                    case UserNotifyDialogEventArgs.NotifyDialogKind.NoWords:
                        //gets displayed in UserDialogComponent
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    InfoBar infobar;
                    infobar = new InfoBar(message, answerPanel.InfoBarControl, DockStyle.Top, true, dontShowAgainMessage, answerPanel.AdditionalInfoBarSuspendControl);
                    infobar.DontShowAgainChanged += new EventHandler(infobar_DontShowAgainChanged);
                }
            }
        }

        /// <summary>
        /// Handles the DontShowAgainChanged event of the infobar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-04-10</remarks>
        private void infobar_DontShowAgainChanged(object sender, EventArgs e)
        {
            if (sender is InfoBar)
            {
                Settings.Default.DontShowSelectedLearnModeNotAllowedMessage = ((InfoBar)sender).DontShowAgain;
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Handles the Click event of the dockingButtonStack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void dockingButtonStack_Click(object sender, EventArgs e)
        {
            StackVisible = !StackVisible;
            if (StackVisible)
                stackHeight = Math.Abs(stackHeight);
            else
                stackHeight = Math.Abs(stackHeight) * -1;
        }

        private void stackFlow_SizeChanged(object sender, EventArgs e)
        {
            if (StackVisible)
                stackHeight = stackFlow.Height;
        }

        private void questionPanel_SizeChanged(object sender, EventArgs e)
        {
            windowWidth = splitContainerVertical.Width - splitContainerVertical.SplitterDistance;
        }
    }
}
