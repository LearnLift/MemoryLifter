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
using System.Xml;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// The ISnoozeOptions implementation for XML learning modules.
	/// </summary>
	public class XmlSnoozeOptions : ISnoozeOptions
	{
		XmlDocument m_dictionary;
		const string m_xpath = "/dictionary/user/";
		const string m_xpathQueryType = "querytype";
		const string m_xpathQueryOptions = "queryoptions";
		const string m_xpathEndCards = "end-cards";
		const string m_xpathEndRights = "end-rights";
		const string m_xpathEndTime = "end-time";
		const string m_xpathSnoozeLow = "snooze-low";
		const string m_xpathSnoozeHigh = "snooze-high";

		internal XmlSnoozeOptions(XmlDocument dictionary, ParentClass parent)
		{
			m_dictionary = dictionary;
			Parent = parent;
		}

		#region ISnoozeOptions Members

		/// <summary>
		/// Disables the snooze mode 'Cards'.
		/// </summary>
		public void DisableCards()
		{
			UnsetQueryType(ESnoozeMode.Cards);
		}

		/// <summary>
		/// Disables the snooze mode 'Rights'.
		/// </summary>
		public void DisableRights()
		{
			UnsetQueryType(ESnoozeMode.Rights);
		}

		/// <summary>
		/// Disables the snooze mode 'Time'.
		/// </summary>
		public void DisableTime()
		{
			UnsetQueryType(ESnoozeMode.Time);
		}

		/// <summary>
		/// Enables snooze mode 'Cards'.
		/// </summary>
		/// <param name="cards">The number of cards.</param>
		public void EnableCards(int cards)
		{
			XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathEndCards, cards);
			SetQueryType(ESnoozeMode.Cards);
		}

		/// <summary>
		/// Enables snooze mode 'Rights'.
		/// </summary>
		/// <param name="rights">The number of correctly answered cards.</param>
		public void EnableRights(int rights)
		{
			XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathEndRights, rights);
			SetQueryType(ESnoozeMode.Rights);
		}

		/// <summary>
		/// Enables snooze mode 'Time'.
		/// </summary>
		/// <param name="time">The time in minutes.</param>
		public void EnableTime(int time)
		{
			XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathEndTime, time);
			SetQueryType(ESnoozeMode.Time);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is cards enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is cards enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsCardsEnabled
		{
			get
			{
				return CheckQueryType(ESnoozeMode.Cards);
			}
			set
			{
				if (value.GetValueOrDefault())
					SetQueryType(ESnoozeMode.Cards);
				else
					UnsetQueryType(ESnoozeMode.Cards);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is rights enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is rights enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsRightsEnabled
		{
			get
			{
				return CheckQueryType(ESnoozeMode.Rights);
			}
			set
			{
				if (value.GetValueOrDefault())
					SetQueryType(ESnoozeMode.Rights);
				else
					UnsetQueryType(ESnoozeMode.Rights);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is time enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is time enabled; otherwise, <c>false</c>.
		/// </value>
		public bool? IsTimeEnabled
		{
			get
			{
				return CheckQueryType(ESnoozeMode.Time);
			}
			set
			{
				if (value.GetValueOrDefault())
					SetQueryType(ESnoozeMode.Time);
				else
					UnsetQueryType(ESnoozeMode.Time);
			}
		}

		/// <summary>
		/// Sets the timeframe within which ML will popup from snooze mode.
		/// </summary>
		/// <param name="lower_time">The lower_time.</param>
		/// <param name="upper_time">The upper_time.</param>
		public void SetSnoozeTimes(int lower_time, int upper_time)
		{
			XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathSnoozeLow, lower_time);
			XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathSnoozeHigh, upper_time);
		}

		/// <summary>
		/// Gets the number of answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze rights.
		/// </value>
		public int? SnoozeCards
		{
			get
			{
				return XmlConvert.ToInt32(m_dictionary.SelectSingleNode(m_xpath + m_xpathEndCards).InnerText);
			}
			set
			{
				XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathEndCards, value.GetValueOrDefault());
			}
		}

		/// <summary>
		/// Gets or sets the snooze mode (SendToTray or QuitProgram).
		/// </summary>
		/// <value>
		/// The snooze mode.
		/// </value>
		public ESnoozeMode? SnoozeMode
		{
			get
			{
				if (CheckQueryOptions(ESnoozeMode.QuitProgram))
					return ESnoozeMode.QuitProgram;
				else
					return ESnoozeMode.SendToTray;
			}
			set
			{
				if (value == ESnoozeMode.QuitProgram)
				{
					UnsetQueryOptions(ESnoozeMode.SendToTray);
					SetQueryOptions(ESnoozeMode.QuitProgram);
				}
				else
				{
					SetQueryOptions(ESnoozeMode.SendToTray);
					UnsetQueryOptions(ESnoozeMode.QuitProgram);
				}
			}
		}
		#endregion

		/// <summary>
		/// Gets the number of correctly answered cards after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze rights.
		/// </value>
		public int? SnoozeRights
		{
			get
			{
				return XmlConvert.ToInt32(m_dictionary.SelectSingleNode(m_xpath + m_xpathEndRights).InnerText);
			}
			set
			{
				XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathEndRights, value.GetValueOrDefault());
			}
		}

		/// <summary>
		/// Gets the higher value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>
		/// The snooze high.
		/// </value>
		public int? SnoozeHigh
		{
			get
			{
				return XmlConvert.ToInt32(m_dictionary.SelectSingleNode(m_xpath + m_xpathSnoozeHigh).InnerText);
			}
			set
			{
				XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathSnoozeHigh, value.GetValueOrDefault());
			}
		}

		/// <summary>
		/// Gets the lower value (in minutes) after with ML will popup from snooze mode.
		/// </summary>
		/// <value>
		/// The snooze low.
		/// </value>
		public int? SnoozeLow
		{
			get
			{
				return XmlConvert.ToInt32(m_dictionary.SelectSingleNode(m_xpath + m_xpathSnoozeLow).InnerText);
			}
			set
			{
				XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathSnoozeLow, value.GetValueOrDefault());
			}
		}

		/// <summary>
		/// Gets the time (in minutes) after which ML will go into snooze mode.
		/// </summary>
		/// <value>
		/// The snooze time.
		/// </value>
		public int? SnoozeTime
		{
			get
			{
				return XmlConvert.ToInt32(m_dictionary.SelectSingleNode(m_xpath + m_xpathEndTime).InnerText);
			}
			set
			{
				XmlHelper.SetXmlInt32(m_dictionary, m_xpath + m_xpathEndTime, value.GetValueOrDefault());
			}
		}


		/// <summary>
		/// Sets the type of the query.
		/// </summary>
		/// <param name="mode">The mode.</param>
		private void SetQueryType(ESnoozeMode mode)
		{
			XmlConfigHelper.Set(m_dictionary, m_xpath + m_xpathQueryType, (int)mode);
		}

		/// <summary>
		/// Unsets the type of the query.
		/// </summary>
		/// <param name="mode">The mode.</param>
		private void UnsetQueryType(ESnoozeMode mode)
		{
			XmlConfigHelper.Unset(m_dictionary, m_xpath + m_xpathQueryType, (int)mode);
		}

		/// <summary>
		/// Checks the type of the query.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns></returns>
		private bool CheckQueryType(ESnoozeMode mode)
		{
			return XmlConfigHelper.Check(m_dictionary, m_xpath + m_xpathQueryType, (int)mode);
		}

		/// <summary>
		/// Sets the query options.
		/// </summary>
		/// <param name="mode">The mode.</param>
		private void SetQueryOptions(ESnoozeMode mode)
		{
			XmlConfigHelper.Set(m_dictionary, m_xpath + m_xpathQueryOptions, (int)mode);
		}

		/// <summary>
		/// Unsets the query options.
		/// </summary>
		/// <param name="mode">The mode.</param>
		private void UnsetQueryOptions(ESnoozeMode mode)
		{
			XmlConfigHelper.Unset(m_dictionary, m_xpath + m_xpathQueryOptions, (int)mode);
		}

		/// <summary>
		/// Checks the query options.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns></returns>
		private bool CheckQueryOptions(ESnoozeMode mode)
		{
			return XmlConfigHelper.Check(m_dictionary, m_xpath + m_xpathQueryOptions, (int)mode);
		}

		/// <summary>
		/// Sets the query options.
		/// </summary>
		/// <param name="type">The type.</param>
		private void SetQueryOptions(EQueryType type)
		{
			XmlConfigHelper.Set(m_dictionary, m_xpath + m_xpathQueryOptions, (int)type);
		}

		/// <summary>
		/// Unsets the query options.
		/// </summary>
		/// <param name="type">The type.</param>
		private void UnsetQueryOptions(EQueryType type)
		{
			XmlConfigHelper.Unset(m_dictionary, m_xpath + m_xpathQueryOptions, (int)type);
		}

		/// <summary>
		/// Checks the query options.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private bool CheckQueryOptions(EQueryType type)
		{
			return XmlConfigHelper.Check(m_dictionary, m_xpath + m_xpathQueryOptions, (int)type);
		}

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(ISnoozeOptions), progressDelegate);
		}

		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get; private set; }

		#endregion
	}
}
