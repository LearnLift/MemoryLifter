using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using MLifter.Controls.Properties;

namespace MLifter.Controls.LearningWindow
{
    [Docking(DockingBehavior.AutoDock)]
    public partial class StackFlow : UserControl, ILearnUserControl
    {
        #region Fields and Properties

        LearnLogic learnlogic = null;
        private Dictionary<AnswerResult, Color> stackCardBackColors = new Dictionary<AnswerResult, Color>();
        int oldCardStackVisibleValue = 0;
        private List<RichTextBox> richTextBoxList = new List<RichTextBox>();

        /// <summary>
        /// Gets or sets the displayed amount of stack cards.
        /// </summary>
        /// <value>The max stack count.</value>
        /// <remarks>Documented by Dev02, 2008-04-16</remarks>
        [Description("Gets or sets the displayed amount of stack cards."), DefaultValue(4), Category("Appearance")]
        public int MaxStackCount
        {
            get { return tableLayoutPanel.ColumnCount; }
            set { tableLayoutPanel.ColumnCount = value; RebuildStack(true); }
        }

        /// <summary>
        /// Gets or sets the back color of the stack card.
        /// </summary>
        /// <value>The color of the stack card back.</value>
        /// <remarks>Documented by Dev02, 2008-04-16</remarks>
        [Browsable(false), ReadOnly(true)]
        public Dictionary<AnswerResult, Color> StackCardBackColors
        {
            get { return stackCardBackColors; }
            set { stackCardBackColors = value; RebuildStack(true); }
        }

        [Browsable(false)]
        public int NumberOfVisibleStackCards
        {
            get
            {
                int counter = 0;
                foreach (Control c in tableLayoutPanel.Controls)
                {
                    if (c.Visible)
                        ++counter;
                }
                return counter;
            }
        }

        /// <summary>
        /// Sets the stack card back colors.
        /// </summary>
        /// <param name="styleHandler">The style handler.</param>
        /// <remarks>Documented by Dev02, 2008-05-09</remarks>
        public void SetStackCardBackColors(MLifter.Components.StyleHandler styleHandler)
        {
            Dictionary<AnswerResult, Color> stackColors = new Dictionary<AnswerResult, Color>();
            foreach (AnswerResult result in Enum.GetValues(typeof(AnswerResult)))
            {
                string stylename = string.Format("{0}_{1}", this.Name, result.ToString());
                if (styleHandler.CurrentStyle.StyledControls.ContainsKey(stylename) && styleHandler.CurrentStyle.StyledControls[stylename].BackColor != Color.Empty)
                {
                    Color color = styleHandler.CurrentStyle.StyledControls[stylename].BackColor;
                    stackColors.Add(result, color);
                }
            }
            this.StackCardBackColors = stackColors;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StackFlow"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-16</remarks>
        public StackFlow()
        {
            InitializeComponent();
            rtbStack1.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            rtbStack2.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            rtbStack3.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            rtbStack4.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;

            //Copy references to the rtbs into a list for a better handling
            richTextBoxList.Add(rtbStack1);
            richTextBoxList.Add(rtbStack2);
            richTextBoxList.Add(rtbStack3);
            richTextBoxList.Add(rtbStack4);
        }

        /// <summary>
        /// Registers the learn logic.
        /// </summary>
        /// <param name="learnlogic">The learnlogic.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        public void RegisterLearnLogic(LearnLogic learnlogic)
        {
            this.learnlogic = learnlogic;
            this.oldCardStackVisibleValue = learnlogic.CardStack.VisibleStack.Count;
            this.learnlogic.CardStack.StackChanged += new EventHandler(CardStack_StackChanged);
        }

        Image backupBackgroundImage = null;

        /// <summary>
        /// Handles the StackChanged event of the CardStack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        private void CardStack_StackChanged(object sender, EventArgs e)
        {
            RebuildStack(false);

            //[ML-1606] Performance Optimizing: Repainting of Stack-Bg-Picture
            //[ML-1658] Changing the skin: stack remains
            if (this.BackgroundImage != null && NumberOfVisibleStackCards >= tableLayoutPanel.ColumnCount)
            {
                backupBackgroundImage = BackgroundImage;
                BackgroundImage = null;
            }
            else if (backupBackgroundImage != null && this.BackgroundImage == null && NumberOfVisibleStackCards < tableLayoutPanel.ColumnCount)
            {
                BackgroundImage = backupBackgroundImage;
                backupBackgroundImage = null;
            }
        }

        /// <summary>
        /// Unicode control char: Right-to-left Embedding
        /// </summary>
        private const short RLE = 0x202B;

        /// <summary>
        /// Unicode control char: Pop Directional Format (Restore the previous bidirectional state)
        /// </summary>
        private const short PDF = 0x202C;

        /// <summary>
        /// Builds the stack.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-01-29</remarks>
        /// <remarks>Documented by Dev08, 2009-05-12</remarks>
        private void RebuildStack(bool forceRepaint)
        {
            if (learnlogic == null || learnlogic.CardStack == null)
                return;

            int visibleStackCount = learnlogic.CardStack.VisibleStack.Count;
            if (forceRepaint || oldCardStackVisibleValue != visibleStackCount)      //something in the stack has changed
            {
                if (visibleStackCount == 0)
                {
                    foreach (RichTextBox rtb in richTextBoxList)
                        rtb.Visible = false;
                }
                else
                {
                    for (int i = 0; i < visibleStackCount; i++)
                        if (richTextBoxList.Count > i)
                            AddTextToRTB(richTextBoxList[i], learnlogic.CardStack.VisibleStack[i]);
                }
            }

            oldCardStackVisibleValue = learnlogic.CardStack.VisibleStack.Count;     //save new cardStackVisible value
            return;
        }

        /// <summary>
        /// Adds the text to RTB.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <param name="stackCard">The stack card.</param>
        /// <remarks>Documented by Dev08, 2009-05-12</remarks>
        private void AddTextToRTB(RichTextBox richTextBox, StackCard stackCard)
        {
            richTextBox.SuspendLayout();

            AnswerResult result = stackCard.Promoted ? AnswerResult.Correct : (stackCard.Result == AnswerResult.Almost ? AnswerResult.Almost : AnswerResult.Wrong);
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<[^>]*>");  //to replace html code if any
            bool questionRTL = stackCard.QuestionCulture.TextInfo.IsRightToLeft;
            bool answerRTL = stackCard.AnswerCulture.TextInfo.IsRightToLeft;

            richTextBox.BackColor = StackCardBackColors.ContainsKey(result) ? StackCardBackColors[result] : SystemColors.Window;
            richTextBox.ForeColor = this.ForeColor;

            string text = Environment.NewLine;

            string questionRLE = (questionRTL ? Convert.ToString(Convert.ToChar(RLE)) : string.Empty);

            text += questionRLE + regex.Replace(stackCard.Question, String.Empty) + Environment.NewLine;
            text += questionRLE + regex.Replace(stackCard.QuestionExample, String.Empty) + Environment.NewLine;

            text += Environment.NewLine;

            string answerRLE = (answerRTL ? Convert.ToString(Convert.ToChar(RLE)) : string.Empty);

            if (stackCard.LearnMode == MLifter.BusinessLayer.LearnModes.Sentence && stackCard.AnswerExample.Length > 0)
            {
                text += answerRLE + regex.Replace(stackCard.AnswerExample, String.Empty) + Environment.NewLine;
                text += answerRLE + regex.Replace(stackCard.Answer, String.Empty) + Environment.NewLine;
            }
            else
            {
                text += answerRLE + regex.Replace(stackCard.Answer, String.Empty) + Environment.NewLine;
                text += answerRLE + regex.Replace(stackCard.AnswerExample, String.Empty) + Environment.NewLine;
            }
            richTextBox.Visible = true;
            richTextBox.Text = text;
            richTextBox.Font = this.Font;

            richTextBox.SelectAll();
            richTextBox.SelectionFont = this.Font;
            richTextBox.Select(0, 0);

            richTextBox.ResumeLayout();
        }

        /// <summary>
        /// Handles the FontChanged event of the StackFlow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        private void StackFlow_FontChanged(object sender, EventArgs e)
        {
            RebuildStack(true);
        }

        /// <summary>
        /// Handles the ForeColorChanged event of the StackFlow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-17</remarks>
        private void StackFlow_ForeColorChanged(object sender, EventArgs e)
        {
            RebuildStack(true);
        }
    }
}
