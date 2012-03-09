using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Interface which defines a flashcard.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public interface ICard : IDisposable, ICopy, IParent, ISecurity
    {
        /// <summary>
        /// Gets the card.
        /// </summary>
        /// <value>The card.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        XmlElement Card { get; }

        /// <summary>
        /// Occurs when [create media progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        event StatusMessageEventHandler CreateMediaProgressChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ICard"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        bool Active { get; set; }
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        int Id { get; }
        /// <summary>
        /// Gets or sets the box.
        /// </summary>
        /// <value>The box.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        int Box { get; set; }
        /// <summary>
        /// Gets or sets the chapter.
        /// </summary>
        /// <value>The chapter.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        int Chapter { get; set; }
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IWords Question { get; set; }
        /// <summary>
        /// Gets or sets the question example.
        /// </summary>
        /// <value>The question example.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IWords QuestionExample { get; set; }
        /// <summary>
        /// Gets or sets the question media.
        /// </summary>
        /// <value>The question media.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IList<IMedia> QuestionMedia { get; }
        /// <summary>
        /// Gets or sets the question distractors.
        /// </summary>
        /// <value>The question distractors.</value>
        /// <remarks>Documented by Dev03, 2008-01-07</remarks>
        IWords QuestionDistractors { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>The answer.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IWords Answer { get; set; }
        /// <summary>
        /// Gets or sets the answer example.
        /// </summary>
        /// <value>The answer example.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IWords AnswerExample { get; set; }
        /// <summary>
        /// Gets or sets the answer media.
        /// </summary>
        /// <value>The answer media.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IList<IMedia> AnswerMedia { get; }
        /// <summary>
        /// Gets or sets the answer distractors.
        /// </summary>
        /// <value>The answer distractors.</value>
        /// <remarks>Documented by Dev03, 2008-01-07</remarks>
        IWords AnswerDistractors { get; set; }

        /// <summary>
        /// Adds the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="side">The side.</param>
        /// <returns>The media object.</returns>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IMedia AddMedia(IMedia media, Side side);
        /// <summary>
        /// Removes the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void RemoveMedia(IMedia media);
        /// <summary>
        /// Creates the media.
        /// </summary>
        /// <param name="type">The type of the media file.</param>
        /// <param name="path">The path to the media file.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="isExample">if set to <c>true</c> [is example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample);
        /// <summary>
        /// Clears all media.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void ClearAllMedia();
        /// <summary>
        /// Clears all media.
        /// </summary>
        /// <param name="removeFiles">if set to <c>true</c> [remove files].</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void ClearAllMedia(bool removeFiles);

        /// <summary>
        /// Creates and returns a card style.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        ICardStyle CreateCardStyle();

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        ISettings Settings { get; set; }
    }

    /// <summary>
    /// Saves the current state of a card.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-09-12</remarks>
    public struct CardState
    {
        /// <summary>
        /// The box number (0-10).
        /// </summary>
        public int Box;
        /// <summary>
        /// Is the card active
        /// </summary>
        public bool Active;
        /// <summary>
        /// The timestamp when the card was touched the last time.
        /// </summary>
        public DateTime TimeStamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardState"/> struct.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public CardState(int box, bool active, DateTime timestamp)
        {
            Box = box;
            Active = active;
            TimeStamp = timestamp;
        }
    }

    /// <summary>
    /// Defines the possible values for the side of a card.
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// Question side.
        /// </summary>
        Question = 0,
        /// <summary>
        /// Answer side.
        /// </summary>
        Answer = 1
    }
}
