using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// IWord interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-10-02</remarks>
    public interface IWord : IParent
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        int Id { get; }
        /// <summary>
        /// Gets or sets the word type.
        /// </summary>
        /// <value>The word type.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        WordType Type { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWord"/> is default.
        /// </summary>
        /// <value><c>true</c> if default; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        bool Default { get; set; }
        /// <summary>
        /// Gets or sets the word.
        /// </summary>
        /// <value>The word.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        string Word { get; set; }
    }

    /// <summary>
    /// Defines the different word types [Word|Sentence].
    /// </summary>
    public enum WordType
    {
        /// <summary>
        /// Word.
        /// </summary>
        Word,
        /// <summary>
        /// Sentence.
        /// </summary>
        Sentence,
        /// <summary>
        /// Distractor
        /// </summary>
        Distractor
    }
}
