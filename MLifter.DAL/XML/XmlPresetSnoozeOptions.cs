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
using System.Xml.Serialization;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents the implementation of ISnoozeOptions used for presets.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPresetSnoozeOptions : ISnoozeOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlPresetSnoozeOptions"/> class.
		/// </summary>
		public XmlPresetSnoozeOptions()
		{

		}

		#region ISnoozeOptions Members

		#region Unused functions
		/// <summary>
		/// Disables the snooze mode 'Cards'.
		/// </summary>
		public void DisableCards()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Disables the snooze mode 'Rights'.
		/// </summary>
		public void DisableRights()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Disables the snooze mode 'Time'.
		/// </summary>
		public void DisableTime()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Enables snooze mode 'Cards'.
		/// </summary>
		/// <param name="cards">The number of cards.</param>
		public void EnableCards(int cards)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Enables snooze mode 'Rights'.
		/// </summary>
		/// <param name="rights">The number of correctly answered cards.</param>
		public void EnableRights(int rights)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Enables snooze mode 'Time'.
		/// </summary>
		/// <param name="time">The time in minutes.</param>
		public void EnableTime(int time)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Sets the timeframe within which ML will popup from snooze mode.
		/// </summary>
		/// <param name="lower_time">The lower_time.</param>
		/// <param name="upper_time">The upper_time.</param>
		public void SetSnoozeTimes(int lower_time, int upper_time)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion

		private bool m_IsCardsEnabled;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is cards enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is cards enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsCardsEnabled
		{
			get { return m_IsCardsEnabled; }
			set { m_IsCardsEnabled = value.GetValueOrDefault(); }
		}

		private bool m_IsRightsEnabled;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is rights enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is rights enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsRightsEnabled
		{
			get { return m_IsRightsEnabled; }
			set { m_IsRightsEnabled = value.GetValueOrDefault(); }
		}

		private bool m_IsTimeEnabled;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is time enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is time enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsTimeEnabled
		{
			get { return m_IsTimeEnabled; }
			set { m_IsTimeEnabled = value.GetValueOrDefault(); }
		}

		private ESnoozeMode m_SnoozeMode = ESnoozeMode.SendToTray;
		/// <summary>
		/// Gets or sets the snooze mode (SendToTray or QuitProgram).
		/// </summary>
		/// <value>
		/// The snooze mode.
		/// </value>
		public ESnoozeMode? SnoozeMode
		{
			get { return m_SnoozeMode; }
			set
			{
				if (value.HasValue)
					m_SnoozeMode = value.Value;
				else
					m_SnoozeMode = ESnoozeMode.SendToTray;

			}
		}

		private int m_SnoozeCards;
		/// <summary>
		/// Gets the number of answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze rights.
		/// </value>
		public int? SnoozeCards
		{
			get { return m_SnoozeCards; }
			set { m_SnoozeCards = value.GetValueOrDefault(); }
		}

		private int m_SnoozeRights;
		/// <summary>
		/// Gets the number of correctly answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze rights.
		/// </value>
		public int? SnoozeRights
		{
			get { return m_SnoozeRights; }
			set { m_SnoozeRights = value.GetValueOrDefault(); }
		}

		private int m_SnoozeTime;
		/// <summary>
		/// Gets the time (in minutes) after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze time.
		/// </value>
		public int? SnoozeTime
		{
			get { return m_SnoozeTime; }
			set { m_SnoozeTime = value.GetValueOrDefault(); }
		}

		private int m_SnoozeHigh;
		/// <summary>
		/// Gets the higher value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>
		/// The snooze high.
		/// </value>
		public int? SnoozeHigh
		{
			get { return m_SnoozeHigh; }
			set { m_SnoozeHigh = value.GetValueOrDefault(); }
		}

		private int m_SnoozeLow;
		/// <summary>
		/// Gets the lower value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>
		/// The snooze low.
		/// </value>
		public int? SnoozeLow
		{
			get { return m_SnoozeLow; }
			set { m_SnoozeLow = value.GetValueOrDefault(); }
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		[XmlIgnore]
		public MLifter.DAL.Tools.ParentClass Parent
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion
	}
}
