using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MLifter.DAL.Interfaces;

namespace MLifter.Controls
{
    public partial class LearnModes : UserControl
    {
        public LearnModes()
        {
            InitializeComponent();

            UpdateCaptions();
            numericUpDownNumberOfChoices_ValueChanged(numericUpDownNumberOfChoices, new EventArgs());
        }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple directions] should be possible.
        /// </summary>
        /// <value><c>true</c> if [multiple directions]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public bool MultipleDirections
        {
            get
            {
                return RBDirectionQuestionAnswer.Appearance == RadioCheckbox.RadioCheckBoxAppearance.Checkbox;
            }
            set
            {
                RBDirectionAnswerQuestion.Appearance = RBDirectionMixed.Appearance = RBDirectionQuestionAnswer.Appearance =
                    (value ? RadioCheckbox.RadioCheckBoxAppearance.Checkbox : RadioCheckbox.RadioCheckBoxAppearance.Radiobutton);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [editable controls enabled].
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        [Browsable(false)]
        public bool EditableControlsEnabled
        {
            get
            {
                return RBDirectionQuestionAnswer.Enabled;
            }
            set
            {
                RBDirectionQuestionAnswer.Enabled = value;
                RBDirectionAnswerQuestion.Enabled = value;
                RBDirectionMixed.Enabled = value;

                LModeStandard.Enabled = value;
                LModeSentences.Enabled = value;
                LModeMultipleChoice.Enabled = value;
                LModeListeningComprehension.Enabled = value;
                LModeImageRecognition.Enabled = value;
            }
        }

        #region Question/Answer captions
        private string questionCaption = Properties.Resources.NEWDIC_QUESTION;
        private string answerCaption = Properties.Resources.NEWDIC_ANSWER;

        /// <summary>
        /// Gets or sets the question caption.
        /// </summary>
        /// <value>The question caption.</value>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public string QuestionCaption
        {
            get { return questionCaption; }
            set { questionCaption = value; UpdateCaptions(); }
        }

        /// <summary>
        /// Gets or sets the answer caption.
        /// </summary>
        /// <value>The answer caption.</value>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public string AnswerCaption
        {
            get { return answerCaption; }
            set { answerCaption = value; UpdateCaptions(); }
        }

        /// <summary>
        /// Updates the captions.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        private void UpdateCaptions()
        {
            if (string.IsNullOrEmpty(answerCaption) || string.IsNullOrEmpty(questionCaption))
                return;

            RBDirectionQuestionAnswer.Text = string.Format(Properties.Resources.LEARN_OPTIONS_DIRECTION_FORMAT, questionCaption, answerCaption);
            RBDirectionAnswerQuestion.Text = string.Format(Properties.Resources.LEARN_OPTIONS_DIRECTION_FORMAT, answerCaption, questionCaption);
        }
        #endregion

        #region QueryTypes
        /// <summary>
        /// Sets the type of the query.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public void SetQueryTypes(IQueryType queryType)
        {
            if (LModeStandard.Enabled)
                LModeStandard.Checked = queryType.Word.GetValueOrDefault();
            if (LModeMultipleChoice.Enabled)
                LModeMultipleChoice.Checked = queryType.MultipleChoice.GetValueOrDefault();
            if (LModeSentences.Enabled)
                LModeSentences.Checked = queryType.Sentence.GetValueOrDefault();
            if (LModeListeningComprehension.Enabled)
                LModeListeningComprehension.Checked = queryType.ListeningComprehension.GetValueOrDefault();
            if (LModeImageRecognition.Enabled)
                LModeImageRecognition.Checked = queryType.ImageRecognition.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the type of the query.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public void GetQueryTypes(ref IQueryType queryType)
        {
            queryType.Word = LModeStandard.Checked;
            queryType.MultipleChoice = LModeMultipleChoice.Checked;
            queryType.Sentence = LModeSentences.Checked;
            queryType.ListeningComprehension = LModeListeningComprehension.Checked;
            queryType.ImageRecognition = LModeImageRecognition.Checked;
        }

        /// <summary>
        /// Sets the allowed query types.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public void SetAllowedQueryTypes(IQueryType queryType)
        {
            if (!(LModeStandard.Enabled = queryType.Word.Value))
                LModeStandard.Checked = false;
            if (!(LModeMultipleChoice.Enabled = queryType.MultipleChoice.Value))
                LModeMultipleChoice.Checked = false;
            if (!(LModeSentences.Enabled = queryType.Sentence.Value))
                LModeSentences.Checked = false;
            if (!(LModeListeningComprehension.Enabled = queryType.ListeningComprehension.Value))
                LModeListeningComprehension.Checked = false;
            if (!(LModeImageRecognition.Enabled = queryType.ImageRecognition.Value))
                LModeImageRecognition.Checked = false;
        }
        #endregion

        #region QueryDirections
        /// <summary>
        /// Sets the query directions.
        /// </summary>
        /// <param name="queryDirections">The query directions.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public void SetQueryDirections(IQueryDirections queryDirections)
        {
            if (RBDirectionQuestionAnswer.Enabled)
                RBDirectionQuestionAnswer.Checked = queryDirections.Question2Answer.GetValueOrDefault();
            if (RBDirectionAnswerQuestion.Enabled)
                RBDirectionAnswerQuestion.Checked = queryDirections.Answer2Question.GetValueOrDefault();
            if (RBDirectionMixed.Enabled)
                RBDirectionMixed.Checked = queryDirections.Mixed.GetValueOrDefault();
        }

        /// <summary>
        /// Sets the allowed query directions.
        /// </summary>
        /// <param name="queryDirections">The query directions.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public void SetAllowedQueryDirections(IQueryDirections queryDirections)
        {
            if (!(RBDirectionQuestionAnswer.Enabled = queryDirections.Question2Answer.GetValueOrDefault()))
                RBDirectionQuestionAnswer.Checked = false;
            if (!(RBDirectionAnswerQuestion.Enabled = queryDirections.Answer2Question.GetValueOrDefault()))
                RBDirectionAnswerQuestion.Checked = false;
            if (!(RBDirectionMixed.Enabled = queryDirections.Mixed.GetValueOrDefault()))
                RBDirectionMixed.Checked = false;
        }

        /// <summary>
        /// Gets the query directions.
        /// </summary>
        /// <param name="queryDirections">The query directions.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public void GetQueryDirections(ref IQueryDirections queryDirections)
        {
            queryDirections.Question2Answer = RBDirectionQuestionAnswer.Checked;
            queryDirections.Answer2Question = RBDirectionAnswerQuestion.Checked;
            queryDirections.Mixed = RBDirectionMixed.Checked;
        }

        ///// <summary>
        ///// Handles the CheckedChanged event of the RBDirection control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///// <remarks>Documented by Dev02, 2008-01-09</remarks>
        //private void RBDirection_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!multipleDirections && sender is CheckBox)
        //    {
        //        //set multipleDirections so that the event handler does not call itself again
        //        multipleDirections = true;
        //        RBDirectionQuestionAnswer.Checked = false;
        //        RBDirectionAnswerQuestion.Checked = false;
        //        RBDirectionMixed.Checked = false;

        //        if ((sender as CheckBox).Enabled)
        //            (sender as CheckBox).Checked = true;
        //        multipleDirections = false;
        //    }
        //}

        /// <summary>
        /// Gets or sets the query direction.
        /// </summary>
        /// <value>The query direction.</value>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public EQueryDirection QueryDirection
        {
            get
            {
                if (RBDirectionQuestionAnswer.Checked)
                    return EQueryDirection.Question2Answer;
                if (RBDirectionAnswerQuestion.Checked)
                    return EQueryDirection.Answer2Question;
                if (RBDirectionMixed.Checked)
                    return EQueryDirection.Mixed;
                return EQueryDirection.Question2Answer;
            }
            set
            {
                switch (value)
                {
                    case EQueryDirection.Question2Answer:
                        RBDirectionQuestionAnswer.Checked = true;
                        break;
                    case EQueryDirection.Answer2Question:
                        RBDirectionAnswerQuestion.Checked = true;
                        break;
                    case EQueryDirection.Mixed:
                        RBDirectionMixed.Checked = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region QueryMultipleChoice
        /// <summary>
        /// Sets the query multiple choice options.
        /// </summary>
        /// <param name="multipleChoiceOptions">The multiple choice options.</param>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        public void SetQueryMultipleChoiceOptions(IQueryMultipleChoiceOptions multipleChoiceOptions)
        {
            if (checkBoxAllowRandomDistractors.Enabled)
                checkBoxAllowRandomDistractors.Checked = multipleChoiceOptions.AllowRandomDistractors.GetValueOrDefault();
            if (checkBoxAllowMultipleCorrectAnswers.Enabled)
                checkBoxAllowMultipleCorrectAnswers.Checked = multipleChoiceOptions.AllowMultipleCorrectAnswers.GetValueOrDefault();
            if (numericUpDownNumberOfChoices.Enabled && multipleChoiceOptions.NumberOfChoices > 0)
                numericUpDownNumberOfChoices.Value = multipleChoiceOptions.NumberOfChoices.GetValueOrDefault();
            if (numericUpDownMaxNumberOfCorrectAnswers.Enabled && multipleChoiceOptions.MaxNumberOfCorrectAnswers > 0)
                numericUpDownMaxNumberOfCorrectAnswers.Value = multipleChoiceOptions.MaxNumberOfCorrectAnswers.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the query multiple choice options.
        /// </summary>
        /// <param name="multipleChoiceOptions">The multiple choice options.</param>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        public void GetQueryMultipleChoiceOptions(ref IQueryMultipleChoiceOptions multipleChoiceOptions)
        {
            multipleChoiceOptions.AllowRandomDistractors = checkBoxAllowRandomDistractors.Checked;
            multipleChoiceOptions.AllowMultipleCorrectAnswers = checkBoxAllowMultipleCorrectAnswers.Checked;
            multipleChoiceOptions.NumberOfChoices = Convert.ToInt32(numericUpDownNumberOfChoices.Value);
            multipleChoiceOptions.MaxNumberOfCorrectAnswers = Convert.ToInt32(numericUpDownMaxNumberOfCorrectAnswers.Value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple choice options visible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [multiple choice options visible]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        public bool MultipleChoiceOptionsVisible
        {
            get { return groupBoxMultipleChoice.Visible; }
            set { groupBoxMultipleChoice.Visible = value; }
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownNumberOfChoices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        private void numericUpDownNumberOfChoices_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownMaxNumberOfCorrectAnswers.Value = Math.Min(numericUpDownMaxNumberOfCorrectAnswers.Value, numericUpDownNumberOfChoices.Value);
            numericUpDownMaxNumberOfCorrectAnswers.Maximum = numericUpDownNumberOfChoices.Value;
        }

        #endregion

        /// <summary>
        /// Validates the input and displays a messagebox if invalid.
        /// </summary>
        /// <returns>True, when input is valid, else false.</returns>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public bool ValidateInput()
        {
            if ((!LModeStandard.Checked && !LModeMultipleChoice.Checked && !LModeSentences.Checked && !LModeListeningComprehension.Checked && !LModeImageRecognition.Checked)
                || (!RBDirectionQuestionAnswer.Checked && !RBDirectionAnswerQuestion.Checked && !RBDirectionMixed.Checked))
            {
                MessageBox.Show(MLifter.Controls.Properties.Resources.LEARN_OPTIONS_NO_MODE_TEXT, MLifter.Controls.Properties.Resources.LEARN_OPTIONS_NO_MODE_CAPTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
