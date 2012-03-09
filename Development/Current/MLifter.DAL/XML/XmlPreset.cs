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
