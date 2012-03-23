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
using System.Globalization;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using System.Reflection;
using MLifter.Generics;


namespace MLifter.BusinessLayer
{
    public enum AnswerResult
    {
        Correct = 0,
        Wrong = 1,
        Almost = 2
    }

    public class CardStack : Stack<StackCard>
    {
        private LearnLogic learnLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardStack"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public CardStack(LearnLogic currentLearnLogic)
        {
            learnLogic = currentLearnLogic;
        }

        /// <summary>
        /// Pushes the specified card.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        public new void Push(StackCard card)
        {
            //refresh cache variables
            if (card.Promoted)
                rightCount += card.CardPoints;
            else
                wrongCount += card.CardPoints;

            base.Push(card);
            OnStackChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Removes all objects from the <see cref="T:System.Collections.Generic.Stack`1"></see>.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public new void Clear()
        {
            rightCount = wrongCount = 0;
            base.Clear();
            OnStackChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when [stack changed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public event EventHandler StackChanged;

        /// <summary>
        /// Raises the <see cref="E:StackChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        protected virtual void OnStackChanged(EventArgs e)
        {
            if (StackChanged != null && !learnLogic.LearningModuleIsClosing)
                StackChanged(this, e);
        }

        /// <summary>
        /// A cache variable for the count of promoted cards.
        /// </summary>
        private int rightCount = 0;

        /// <summary>
        /// A cache variable for the count of demoted cards.
        /// </summary>
        private int wrongCount = 0;

        /// <summary>
        /// Gets the count of correct answers.
        /// </summary>
        /// <value>The correct count.</value>
        /// <remarks>Documented by Dev02, 2008-04-30</remarks>
        public int RightCount
        {
            get { return rightCount; }
        }

        /// <summary>
        /// Gets the count of wrong answers.
        /// </summary>
        /// <value>The wrong count.</value>
        /// <remarks>Documented by Dev02, 2008-04-30</remarks>
        public int WrongCount
        {
            get { return wrongCount; }
        }

        /// <summary>
        /// Gets the session duration.
        /// </summary>
        /// <value>The session duration.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public TimeSpan SessionDuration
        {
            get
            {
                TimeSpan duration = new TimeSpan();
                foreach (StackCard card in this)
                    duration += card.Duration;
                return duration;
            }
        }

        /// <summary>
        /// Gets the visible stack cards.
        /// </summary>
        /// <value>The visible stack.</value>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        public List<StackCard> VisibleStack
        {
            get
            {
                List<StackCard> list = new List<StackCard>();
                foreach (StackCard card in this)
                    if (!card.Promoted)
                        list.Add(card);
                return list;
            }
        }

        /// <summary>
        /// The card at which the snooze mode was activated most recently.
        /// </summary>
        private StackCard lastSnoozeCard = null;

        /// <summary>
        /// Checks if it is time to activate the snooze mode.
        /// </summary>
        /// <param name="options">The snooze options.</param>
        /// <returns>True, if the snooze mode should be activated, else false.</returns>
        /// <remarks>Documented by Dev02, 2008-05-06</remarks>
        internal bool CheckSnooze(ISnoozeOptions options)
        {
            if (CheckOptionsChanged(options))
                ResetSnoozeValues();

            if (options.SnoozeMode == ESnoozeMode.SendToTray || options.SnoozeMode == ESnoozeMode.QuitProgram)
            {
                //calculate differential values since last snooze
                TimeSpan time = new TimeSpan();
                int cards = 0;
                int rights = 0;

                foreach (StackCard card in this)
                {
                    if (lastSnoozeCard != null && card == lastSnoozeCard)
                        break;

                    time += card.Duration;
                    cards++;
                    if (card.Promoted)
                        rights += card.CardPoints;
                }

                //compare with settings
                if ((options.IsTimeEnabled.GetValueOrDefault() && time.Minutes >= options.SnoozeTime.GetValueOrDefault()) ||
                    (options.IsCardsEnabled.GetValueOrDefault() && cards >= options.SnoozeCards.GetValueOrDefault()) ||
                    (options.IsRightsEnabled.GetValueOrDefault() && rights >= options.SnoozeRights.GetValueOrDefault()))
                {
                    lastSnoozeCard = this.Peek(); //set snooze card
                    return true;
                }
            }
            return false;
        }

        private ISnoozeOptions previousOptions = new MLifter.DAL.Preview.PreviewSnoozeOptions();

        /// <summary>
        /// Checks the options changed.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        private bool CheckOptionsChanged(ISnoozeOptions options)
        {
            bool changed = false;

            if (previousOptions != null)
            {
                if (!Methods.IsEqual<bool?>(options.IsCardsEnabled, previousOptions.IsCardsEnabled) ||
                    !Methods.IsEqual<bool?>(options.IsRightsEnabled, previousOptions.IsRightsEnabled) ||
                    !Methods.IsEqual<bool?>(options.IsTimeEnabled, previousOptions.IsTimeEnabled))
                    changed = true;

                if (!Methods.IsEqual<int?>(options.SnoozeCards, previousOptions.SnoozeCards) ||
                    !Methods.IsEqual<int?>(options.SnoozeHigh, previousOptions.SnoozeHigh) ||
                    !Methods.IsEqual<int?>(options.SnoozeLow, previousOptions.SnoozeLow) ||
                    !Methods.IsEqual<ESnoozeMode?>(options.SnoozeMode, previousOptions.SnoozeMode) ||
                    !Methods.IsEqual<int?>(options.SnoozeRights, previousOptions.SnoozeRights) ||
                    !Methods.IsEqual<int?>(options.SnoozeTime, previousOptions.SnoozeTime))
                    changed = true;
            }

            foreach (PropertyInfo property in typeof(ISnoozeOptions).GetProperties())
            {
                if (property.CanRead && property.CanWrite)
                {
                    object obj = property.GetValue(options, null);
                    if (obj != null)
                        property.SetValue(previousOptions, obj, null);
                }
            }

            return changed;
        }

        /// <summary>
        /// Resets the snooze values.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        public void ResetSnoozeValues()
        {
            if (this.Count > 0)
                lastSnoozeCard = this.Peek();
            else
                lastSnoozeCard = null;
        }
    }

    /// <summary>
    /// StackCard... 
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-09-09</remarks>
    public class StackCard
    {
        private ICard m_Card;
        private LearnModes m_LearnMode;
        private EQueryDirection m_QueryDirection;
        private AnswerResult m_Result;
        private bool m_Promoted;

        private DateTime m_CardAsked;
        private DateTime m_CardAnswered;

        private CultureInfo m_QuestionCulture;
        private CultureInfo m_AnswerCulture;

        private string m_answer;
        private int m_newBox;
        private int m_oldBox;
        private bool m_canceledDemote;
        private Dictionary m_lm;
        private int m_correctSynonyms;

        public LearnModes LearnMode
        {
            get { return m_LearnMode; }
            set { m_LearnMode = value; }
        }

        public EQueryDirection QueryDirection
        {
            get { return m_QueryDirection; }
            set { m_QueryDirection = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackCard"/> class.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="result">The result.</param>
        /// <param name="promoted">if set to <c>true</c> [promoted].</param>
        /// <param name="queryDirection">The query direction.</param>
        /// <param name="learnMode">The learn mode.</param>
        /// <param name="asked">The asked.</param>
        /// <param name="answered">The answered.</param>
        /// <param name="questionCulture">The question culture.</param>
        /// <param name="answerculture">The answerculture.</param>
        /// <remarks>Documented by Dev02, 2008-07-17</remarks>
        public StackCard(ICard card, AnswerResult result, bool promoted, EQueryDirection queryDirection, LearnModes learnMode, DateTime asked, DateTime answered, CultureInfo questionCulture, CultureInfo answerculture, string answer, int obox, int nbox, bool canDem, Dictionary lm, int cs)
        {
            m_Card = card;
            m_Result = result;
            m_Promoted = promoted;
            m_QueryDirection = queryDirection;
            m_LearnMode = learnMode;
            m_CardAsked = asked;
            m_CardAnswered = answered;

            m_QuestionCulture = questionCulture;
            m_AnswerCulture = answerculture;

            m_answer = answer;
            m_newBox = nbox;
            m_oldBox = obox;
            m_canceledDemote = canDem;
            m_lm = lm;
            m_correctSynonyms = cs;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public AnswerResult Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }

        /// <summary>
        /// Gets or sets the card.
        /// </summary>
        /// <value>The card.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public ICard Card
        {
            get { return m_Card; }
            set { m_Card = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StackCard"/> was promoted.
        /// </summary>
        /// <value><c>true</c> if promoted; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public bool Promoted
        {
            get { return m_Promoted; }
            set { m_Promoted = value; }
        }

        public string Question
        {
            get { return (m_QueryDirection == EQueryDirection.Question2Answer ? m_Card.Question.ToString() : m_Card.Answer.ToString()); }
        }

        public string Answer
        {
            get { return (m_QueryDirection == EQueryDirection.Question2Answer ? m_Card.Answer.ToString() : m_Card.Question.ToString()); }
        }

        public string QuestionExample
        {
            get { return (m_QueryDirection == EQueryDirection.Question2Answer ? m_Card.QuestionExample.ToString() : m_Card.AnswerExample.ToString()); }
        }

        public string AnswerExample
        {
            get { return (m_QueryDirection == EQueryDirection.Question2Answer ? m_Card.AnswerExample.ToString() : m_Card.QuestionExample.ToString()); }
        }

        /// <summary>
        /// Gets the timestamp, when the card was asked.
        /// </summary>
        /// <value>The card asked.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public DateTime CardAsked
        {
            get { return m_CardAsked; }
        }

        /// <summary>
        /// Gets the timestamp, when the card was answered.
        /// </summary>
        /// <value>The card answered.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public DateTime CardAnswered
        {
            get { return m_CardAnswered; }
        }

        /// <summary>
        /// Gets the duration, how long the card was visible.
        /// </summary>
        /// <value>The duration.</value>
        /// <remarks>Documented by Dev02, 2008-05-05</remarks>
        public TimeSpan Duration
        {
            get { return this.CardAnswered - this.CardAsked; }
        }

        /// <summary>
        /// Gets the points for the card, if answered correct or wrong.
        /// </summary>
        /// <value>The card points.</value>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public int CardPoints
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the question culture.
        /// </summary>
        /// <value>The question culture.</value>
        /// <remarks>Documented by Dev02, 2008-07-17</remarks>
        public CultureInfo QuestionCulture
        {
            get { return (m_QueryDirection == EQueryDirection.Question2Answer ? m_QuestionCulture : m_AnswerCulture); }
        }

        /// <summary>
        /// Gets the answer culture.
        /// </summary>
        /// <value>The answer culture.</value>
        /// <remarks>Documented by Dev02, 2008-07-17</remarks>
        public CultureInfo AnswerCulture
        {
            get { return (m_QueryDirection == EQueryDirection.Question2Answer ? m_AnswerCulture : m_QuestionCulture); }
        }


        /// <summary>
        /// Gets the user answer (NOT the correct answer).
        /// </summary>
        /// <value>The user answer.</value>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        public string UserAnswer
        {
            get
            {
                return m_answer;
            }
        }

        /// <summary>
        /// Gets the old box.
        /// </summary>
        /// <value>The old box.</value>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        public int OldBox
        {
            get
            {
                return m_oldBox;
            }
        }

        /// <summary>
        /// Gets the new box.
        /// </summary>
        /// <value>The new box.</value>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        public int NewBox
        {
            get
            {
                return m_newBox;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [canceled demote].
        /// </summary>
        /// <value><c>true</c> if [canceled demote]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        public bool CanceledDemote
        {
            get
            {
                return m_canceledDemote;
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev08, 2008-09-09</remarks>
        public Dictionary ParentDictionary
        {
            get
            {
                return m_lm;
            }
        }

        /// <summary>
        /// Gets the correct synonyms.
        /// </summary>
        /// <value>The correct synonyms.</value>
        /// <remarks>Documented by Dev08, 2008-09-10</remarks>
        public int CorrectSynonyms
        {
            get
            {
                return m_correctSynonyms;
            }
        }
    }
}
