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
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// This interface describes the possible snooze options
	/// </summary>
	public interface ISnoozeOptions : ICopy, IParent
	{
		/// <summary>
		/// Disables the snooze mode 'Cards'.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void DisableCards();
		/// <summary>
		/// Disables the snooze mode 'Rights'.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void DisableRights();
		/// <summary>
		/// Disables the snooze mode 'Time'.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void DisableTime();
		/// <summary>
		/// Enables snooze mode 'Cards'.
		/// </summary>
		/// <param name="cards">The number of cards.</param>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void EnableCards(int cards);
		/// <summary>
		/// Enables snooze mode 'Rights'.
		/// </summary>
		/// <param name="rights">The number of correctly answered cards.</param>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void EnableRights(int rights);
		/// <summary>
		/// Enables snooze mode 'Time'.
		/// </summary>
		/// <param name="time">The time in minutes.</param>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void EnableTime(int time);
		/// <summary>
		/// Gets or sets a value indicating whether this instance is cards enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is cards enabled; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev03, 2008-01-17</remarks>
		[ValueCopy]
		bool? IsCardsEnabled { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is rights enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is rights enabled; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev03, 2008-01-17</remarks>
		[ValueCopy]
		bool? IsRightsEnabled { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is time enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is time enabled; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev03, 2008-01-17</remarks>
		[ValueCopy]
		bool? IsTimeEnabled { get; set; }
		/// <summary>
		/// Sets the timeframe within which ML will popup from snooze mode.
		/// </summary>
		/// <param name="lower_time">The lower_time.</param>
		/// <param name="upper_time">The upper_time.</param>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void SetSnoozeTimes(int lower_time, int upper_time);
		/// <summary>
		/// Gets or sets the snooze mode (SendToTray or QuitProgram).
		/// </summary>
		/// <value>The snooze mode.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		[ValueCopy]
		ESnoozeMode? SnoozeMode { get; set; }
		/// <summary>
		/// Gets the number of answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>The snooze rights.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		[ValueCopy]
		int? SnoozeCards { get; set; }
		/// <summary>
		/// Gets the number of correctly answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>The snooze rights.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		[ValueCopy]
		int? SnoozeRights { get; set; }
		/// <summary>
		/// Gets the time (in minutes) after which ML will go into snooze mode.
		/// </summary>
		/// <value>The snooze time.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		[ValueCopy]
		int? SnoozeTime { get; set; }
		/// <summary>
		/// Gets the higher value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>The snooze high.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		[ValueCopy]
		int? SnoozeHigh { get; set; }
		/// <summary>
		/// Gets the lower value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>The snooze low.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		[ValueCopy]
		int? SnoozeLow { get; set; }
	}

	/// <summary>
	/// Helper method for the ISnoozeOptions interface
	/// </summary>
	public static class SnoozeOptionsHelper
	{
		/// <summary>
		/// Compares the specified object.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns></returns>
		public static bool Compare(object a, object b)
		{
			return false;
		}
	}

	/// <summary>
	/// Enumerator which defines the possible options when a learning session is finished. Available values are:
	/// - SendToTray: minimize MemoryLifter to tray icon
	/// - QuitProgram: quit the program
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-02</remarks>
	public enum ESnoozeMode
	{
		/// <summary>
		/// After the specified time
		/// </summary>
		Time = 32,
		/// <summary>
		/// After the specified asked cards
		/// </summary>
		Cards = 64,
		/// <summary>
		/// After the specified amount of right answers
		/// </summary>
		Rights = 128,
		/// <summary>
		/// Minimize MemoryLifter to tray icon.
		/// </summary>
		SendToTray = 1024,
		/// <summary>
		/// Quit the program.
		/// </summary>
		QuitProgram = 2048
	}
}
