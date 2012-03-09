using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;

namespace MLifter.BusinessLayer
{
    public enum MultipleChoiceResult
    {
        Correct,
        Wrong
    }

    /// <summary>
    /// A MultipleChoce query.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-01-10</remarks>
    public class MultipleChoice : List<Choice>
    {
        private Random m_Random;

        private ICard m_CorrectCard;

        /// <summary>
        /// Gets or sets the correct card.
        /// </summary>
        /// <value>The correct card.</value>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public ICard CorrectCard
        {
            get { return m_CorrectCard; }
            set { m_CorrectCard = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleChoice"/> class.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public MultipleChoice()
        {
            m_Random = new Random((int)DateTime.Now.Ticks);
        }

        /// <summary>
        /// Gets the result of the query.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public MultipleChoiceResult GetResult()
        {
            MultipleChoiceResult result = MultipleChoiceResult.Wrong;
            bool first = true;
            foreach (Choice choice in this)
            {
                if (choice.IsCorrect)
                {
                    result = (choice.Checked && (first || (result == MultipleChoiceResult.Correct))) ? MultipleChoiceResult.Correct : MultipleChoiceResult.Wrong;
                    first = false;
                }
                else if (choice.Checked)
                {
                    result = MultipleChoiceResult.Wrong;
                    break;
                }
            }
            return result;
        }

        public string GetAnswers()
        {
            string result = String.Empty;
            List<Choice> answers = this.FindAll(c => c.Checked);
            answers.ForEach(a => result += a.Word + "\n");
            return result;
        }

        /// <summary>
        /// Randomizes the list of choices.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public void Randomize()
        {
            this.ForEach(c => c.SortKey = m_Random.Next());
            this.Sort((l, r) => l.SortKey.CompareTo(r.SortKey));
        }

    }

    /// <summary>
    /// A single choice for a MultipleChoice.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-01-10</remarks>
    public class Choice
    {
        private int m_SortKey = 0;
        private bool m_Checked = false;
        private string m_Word;
        private bool m_Correct;

        /// <summary>
        /// Gets or sets a value indicating whether this choice is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public bool Checked
        {
            get { return m_Checked; }
            set { m_Checked = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this choice is marked as correct answer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is correct; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public bool IsCorrect
        {
            get { return m_Correct; }
            set { m_Correct = value; }
        }

        /// <summary>
        /// Gets or sets the word.
        /// </summary>
        /// <value>The word.</value>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public string Word
        {
            get { return m_Word; }
            set { m_Word = value; }
        }

        /// <summary>
        /// Gets or sets the sort key.
        /// </summary>
        /// <value>The sort key.</value>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        internal int SortKey
        {
            get { return m_SortKey; }
            set { m_SortKey = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Choice"/> class.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="isCorrect">if set to <c>true</c> [is correct].</param>
        /// <remarks>Documented by Dev03, 2008-01-10</remarks>
        public Choice(string word, bool isCorrect)
        {
            m_Correct = isCorrect;
            m_Word = word;
        }
    }
}
