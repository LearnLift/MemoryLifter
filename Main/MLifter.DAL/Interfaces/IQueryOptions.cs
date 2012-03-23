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
	/// Represents the available query options.
	/// </summary>
	public interface IQueryOptions : IParent
	{
		/// <summary>
		/// Gets or sets the strip chars.
		/// </summary>
		/// <value>The strip chars.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		string StripChars { get; set; }
		/// <summary>
		/// Gets or sets if to grade synonyms.
		/// </summary>
		/// <value>
		/// The grade synonyms value.
		/// </value>
		EGradeSynonyms GradeSynonyms { get; set; }
		/// <summary>
		/// Gets or sets if to grade typing.
		/// </summary>
		/// <value>
		/// The grade typing value.
		/// </value>
		EGradeTyping GradeTyping { get; set; }
		/// <summary>
		/// Gets or sets the snooze options.
		/// </summary>
		/// <value>
		/// The snooze options.
		/// </value>
		ISnoozeOptions SnoozeOptions { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether user confirmation is required to confirm demote.
		/// </summary>
		/// <value><c>true</c> if [confirm demote]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool ConfirmDemote { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether cards should be drawn randomly from pool.
		/// </summary>
		/// <value><c>true</c> if [random pool]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool RandomPool { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether self assessment mode is active.
		/// </summary>
		/// <value><c>true</c> if [self assessment]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool SelfAssessment { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [skip correct answers].
		/// </summary>
		/// <value><c>true</c> if [skip correct answers]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool SkipCorrectAnswers { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [correct on the fly].
		/// </summary>
		/// <value><c>true</c> if [correct on the fly]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool CorrectOnTheFly { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether commentary sounds are enabled.
		/// </summary>
		/// <value><c>true</c> if [enable commentary]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool EnableCommentary { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether standard audio files should be played automatically.
		/// </summary>
		/// <value><c>true</c> if [autoplay audio]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool AutoplayAudio { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [show images].
		/// </summary>
		/// <value><c>true</c> if [show images]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool ShowImages { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [show statistics].
		/// </summary>
		/// <value><c>true</c> if [show statistics]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool ShowStatistics { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the time should be enabled.
		/// </summary>
		/// <value><c>true</c> if [enable timer]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool EnableTimer { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [case sensitive].
		/// </summary>
		/// <value><c>true</c> if [case sensitive]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		bool CaseSensitive { get; set; }
		/// <summary>
		/// Gets or sets the query types.
		/// </summary>
		/// <value>
		/// The query types.
		/// </value>
		/// <remarks>Documented by Dev05, 2012-01-11</remarks>
		IQueryType QueryTypes { get; set; }
		/// <summary>
		/// Gets or sets the query direction.
		/// </summary>
		/// <value>
		/// The query direction.
		/// </value>
		/// <remarks>Documented by Dev05, 2012-01-11</remarks>
		EQueryDirection QueryDirection { get; set; }
		// User settings 
		/// <summary>
		/// Gets or sets the multiple choice options.
		/// </summary>
		/// <value>The multiple choice options.</value>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		IQueryMultipleChoiceOptions MultipleChoiceOptions { get; set; }
	}

	/// <summary>
	/// Enumeration which defines the values for the 'Synonyms' options:
	/// AllKnown - all synonyms need to be known,
	/// HalfKnown - half need to be known,
	/// OneKnown - one synonym needs to be known,
	/// FirstKnown - the card is promoted when the first synonym is known,
	/// Promp - prompt when not all were correct
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-02</remarks>
	public enum EGradeSynonyms
	{
		/// <summary>
		/// All synonyms need to be known.
		/// </summary>
		AllKnown = 0,
		/// <summary>
		/// Half need to be known.
		/// </summary>
		HalfKnown = 1,
		/// <summary>
		/// One synonym needs to be known.
		/// </summary>
		OneKnown = 2,
		/// <summary>
		/// The card is promoted when the first synonym is known.
		/// </summary>
		FirstKnown = 3,
		/// <summary>
		/// Prompt when not all were correct.
		/// </summary>
		Prompt = 4
	}
	/// <summary>
	/// Enumeration which defines the values for the 'Typing mistakes' options:
	/// AllCorrect - only promote when all correct,
	/// HalfCorrect - promote when more than 50% were typed correctly,
	/// NoneCorrect - always promote,
	/// Prompt - prompt
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-02</remarks>
	public enum EGradeTyping
	{
		/// <summary>
		/// Only promote when all correct.
		/// </summary>
		AllCorrect = 0,
		/// <summary>
		/// Promote when more than 50% were typed correctly.
		/// </summary>
		HalfCorrect = 1,
		/// <summary>
		/// Always promote.
		/// </summary>
		NoneCorrect = 2,
		/// <summary>
		/// Prompt user.
		/// </summary>
		Prompt = 3
	}

	/// <summary>
	/// Enumerate the possible query directions (required for backward compatibility). Available values are:
	/// CaseSensitive - typed answers are case sensitive,
	/// CountDown - activates countdown timer,
	/// Stats - display statistics,
	/// Images - display images,
	/// Sounds - Auto-play sound,
	/// Commentary - play commentary sounds (e.g. very good...!),
	/// Correct - correct typing on the fly,
	/// Self - self assessment mode,
	/// RandomPool - choses a random card from the pool,
	/// ConfirmDemote - confirmation is required for a card to be demoted
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-07-20</remarks>
	public enum EQueryOption
	{
		/// <summary>
		/// Typed answers are case sensitive.
		/// </summary>
		CaseSensitive = 1,
		/// <summary>
		/// Activates countdown timer.
		/// </summary>
		CountDown = 2,
		/// <summary>
		/// Display statistics.
		/// </summary>
		Stats = 4,
		/// <summary>
		/// Display images.
		/// </summary>
		Images = 32,
		/// <summary>
		/// Auto-play sound.
		/// </summary>
		Sounds = 64,
		/// <summary>
		/// Play commentary sounds (e.g. very good...!).
		/// </summary>
		Commentary = 128,
		/// <summary>
		/// Correct typing on the fly.
		/// </summary>
		Correct = 256,
		/// <summary>
		/// 
		/// </summary>
		Skip = 512,
		/// <summary>
		/// Self assessment mode.
		/// </summary>
		Self = 4096,
		/// <summary>
		/// Choses a random card from the pool.
		/// </summary>
		RandomPool = 8192,
		/// <summary>
		/// Confirmation is required for a card to be demoted.
		/// </summary>
		ConfirmDemote = 16384
	}
}
