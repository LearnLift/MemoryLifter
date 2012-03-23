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

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents the implementation of preset settings.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-09-24</remarks>
	public class XmlPreset : IPreset
	{
		private string m_Title;
		private string m_ResourceId;
		private XmlPresetSettings m_Presets;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlPreset"/> class.
		/// </summary>
		public XmlPreset()
		{

		}

		#region IPreset Members

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>The title.</value>
		/// <remarks>Documented by Dev08, 2008-09-24</remarks>
		/// <remarks>Documented by Dev03, 2008-09-24</remarks>
		public string Title
		{
			get { return m_Title; }
			set { m_Title = value; }
		}

		/// <summary>
		/// Gets the resource id.
		/// </summary>
		/// <value>The resource id.</value>
		/// <remarks>Documented by Dev08, 2008-09-24</remarks>
		/// <remarks>Documented by Dev03, 2008-09-24</remarks>
		public string ResourceId
		{
			get { return m_ResourceId; }
			set { m_ResourceId = value; }
		}

		/// <summary>
		/// Gets the presets.
		/// </summary>
		/// <value>The presets.</value>
		/// <remarks>Documented by Dev08, 2008-09-24</remarks>
		/// <remarks>Documented by Dev03, 2008-09-24</remarks>
		[XmlIgnore]
		public ISettings Preset
		{
			get
			{
				if (m_Presets == null)
					m_Presets = new XmlPresetSettings();
				return m_Presets;
			}
			set { m_Presets = (XmlPresetSettings)value; }
		}

		/// <summary>
		/// Gets or sets the serializable presets.
		/// </summary>
		/// <value>
		/// The serializable presets.
		/// </value>
		[XmlElement("Settings")]
		public XmlPresetSettings SerializablePresets
		{
			get
			{
				if (m_Presets == null)
					m_Presets = new XmlPresetSettings();
				return m_Presets;
			}
			set { m_Presets = value; }
		}

		#endregion
	}
}
