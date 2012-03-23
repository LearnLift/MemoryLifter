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
