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
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// IWords interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-10-02</remarks>
    public interface IWords : ICopy, IParent
    {
        /// <summary>
        /// Gets the culture for the words.
        /// </summary>
        /// <value>The culture.</value>
        /// <remarks>Documented by Dev03, 2007-11-30</remarks>
        CultureInfo Culture { get;}
        /// <summary>
        /// Gets the words list.
        /// </summary>
        /// <value>The words.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        IList<IWord> Words { get; }
        /// <summary>
        /// Creates a word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="type">The type.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        IWord CreateWord(string word, WordType type, bool isDefault);
        /// <summary>
        /// Adds a word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        void AddWord(IWord word);
        /// <summary>
        /// Adds multiple words. The first word will be marked as default.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        void AddWords(string[] words);
        /// <summary>
        /// Adds multiple words.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        void AddWords(List<IWord> words);
        /// <summary>
        /// Aeletes all words.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        void ClearWords();
        /// <summary>
        /// Returns all words as quoted, comma-delimited string.
        /// </summary>
        /// <returns>The words.</returns>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        string ToQuotedString();
        /// <summary>
        /// Returns all words as newline-delimited string.
        /// </summary>
        /// <returns>The words.</returns>
        /// <remarks>Documented by Dev02, 2008-01-29</remarks>
        string ToNewlineString();
        /// <summary>
        /// Returns all words as a list of strings.
        /// </summary>
        /// <returns>The words.</returns>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        IList<string> ToStringList();
    }

    internal static class WordsHelper
    {
        public static void CopyWords(IWords source, IWords target)
        {
            if (!typeof(IWords).IsAssignableFrom(target.GetType()))
                throw new ArgumentException("Target must implement IWords!");

            foreach (IWord word in source.Words)
            {
                IWord newWord = target.CreateWord(word.Word, word.Type, word.Default);
                target.AddWord(newWord);
            }
        }
    }
}
