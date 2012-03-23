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

namespace MLifter.DAL.Interfaces.DB
{
	/// <summary>
	/// The database connector for LearnLogging
	/// </summary>
	/// <remarks>Documented by Dev08, 2008-09-05</remarks>
	interface IDbLearnLoggingConnector
	{
		/// <summary>
		/// Creates the learn log entry.
		/// </summary>
		/// <param name="learnLog">The learn log.</param>
		void CreateLearnLogEntry(LearnLogStruct learnLog);
	}

	/// <summary>
	/// The move type of the card.
	/// </summary>
	public enum MoveType
	{
		/// <summary>
		/// The card was automatically promoted
		/// </summary>
		AutoPromote = 1,
		/// <summary>
		/// The card was automatically demoted
		/// </summary>
		AutoDemote = 2,
		/// <summary>
		/// The card was promoted manually
		/// </summary>
		ManualPromote = 4,
		/// <summary>
		/// The card was demoted manually
		/// </summary>
		ManualDemote = 8,
		/// <summary>
		/// The card was moved manually
		/// </summary>
		Manual = 16,
		/// <summary>
		/// The demotion was canceld.
		/// </summary>
		CanceledDemote = 32
	}

	/// <summary>
	/// The log struct for an answer.
	/// </summary>
	public struct LearnLogStruct
	{
		/// <summary>
		/// The learning session id
		/// </summary>
		public int? SessionID;
		/// <summary>
		/// The id of the card
		/// </summary>
		public int? CardsID;
		/// <summary>
		/// The old box of the card
		/// </summary>
		public int? OldBox;
		/// <summary>
		/// The new box of the card
		/// </summary>
		public int? NewBox;
		/// <summary>
		/// The time the card was shown
		/// </summary>
		public DateTime? TimeStamp;
		/// <summary>
		/// The time the card was answerd
		/// </summary>
		public long? Duration;
		/// <summary>
		/// The mode the card was asked
		/// </summary>
		public EQueryType? LearnMode;
		/// <summary>
		/// The move type which was executed
		/// </summary>
		public MoveType? MoveType;
		/// <summary>
		/// The answer which where given
		/// </summary>
		public string Answer;
		/// <summary>
		/// The direction the card was asked
		/// </summary>
		public EQueryDirection? Direction;
		/// <summary>
		/// Was case sensitivity enabled?
		/// </summary>
		public bool? CaseSensitive;
		/// <summary>
		/// Was correct on the fly enabled?
		/// </summary>
		public bool? CorrectOnTheFly;
		/// <summary>
		/// The precantage of the required anwer which was known
		/// </summary>
		public int? PercentageKnown;
		/// <summary>
		/// The percantage of the possible anwer which was required
		/// </summary>
		public int? PercentageRequired;
	}
}
