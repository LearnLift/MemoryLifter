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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using MLifter.DAL.Interfaces;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// This is to store an load XML Presets
	/// </summary>
	public class XmlPresets : IPresets
	{
		private Encoding FileEncoding = Encoding.UTF8;

		/// <summary>
		/// A list of XmlPresets
		/// </summary>
		public class PresetSettings : List<XmlPreset>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PresetSettings"/> class.
			/// </summary>
			public PresetSettings()
			{

			}
		}

		private XmlSerializer xmlSerializer = new XmlSerializer(typeof(PresetSettings), new XmlRootAttribute("PresetSettings"));
		private List<IPreset> m_presets = new List<IPreset>();

		internal XmlPresets(string path)
		{
			FileInfo presetsFile = new FileInfo(path);
			Load(presetsFile);
		}

		#region IPresets Members

		/// <summary>
		/// Gets the presets.
		/// </summary>
		/// <value>
		/// The presets.
		/// </value>
		public List<IPreset> Presets
		{
			get { return m_presets; }
		}

		#endregion

		/// <summary>
		/// Loads the whole list from a xml file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev02, 2008-01-17</remarks>
		private void Load(FileInfo file)
		{
			if (file.Exists)
			{
				try
				{
					using (StreamReader sr = new StreamReader(file.FullName, FileEncoding))
					{
						using (XmlReader reader = XmlReader.Create(sr.BaseStream))
						{
							LoadByStream(reader);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(string.Format("Unable to load PresetSettings ({0}).", ex.Message));
				}
			}
		}

		/// <summary>
		/// Loads the whole list as xml from a XmlReader.
		/// </summary>
		/// <param name="reader">An XmlReader object.</param>
		/// <remarks>Documented by Dev02, 2008-01-17</remarks>
		private void LoadByStream(XmlReader reader)
		{
			if (!xmlSerializer.CanDeserialize(reader))
			{
				Debug.WriteLine("Unable to deserialize PresetSettings.");
				return;
			}
			PresetSettings presets = (PresetSettings)xmlSerializer.Deserialize(reader);
			m_presets.Clear();
			foreach (XmlPreset preset in presets)
				m_presets.Add((IPreset)preset);
		}

		/// <summary>
		/// Saves the whole list as xml into a Stream.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <remarks>
		/// Documented by DAC, 2008-01-17.
		/// </remarks>
		public void Save(string filename)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter(filename, false, FileEncoding))
				{
					XmlWriterSettings xws = new XmlWriterSettings();
					xws.Indent = true;
					using (XmlWriter writer = XmlWriter.Create(sw.BaseStream, xws))
					{
						PresetSettings presets = new PresetSettings();
						foreach (IPreset preset in m_presets)
							presets.Add((XmlPreset)preset);
						xmlSerializer.Serialize(writer, presets);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("Unable to write PresetSettings ({0}).", ex.Message));
			}
		}

	}
}
