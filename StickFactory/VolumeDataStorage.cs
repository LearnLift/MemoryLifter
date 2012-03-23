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
using System.Xml.Serialization;
using System.IO;

namespace StickFactory
{
	/// <summary>
	/// A class to store metadata for a disk volume, including the necessary serializer.
	/// </summary>
	/// <remarks>Documented by Dev02, 2008-10-09</remarks>
	[XmlRoot("VolumeData")]
	public class VolumeDataStorage
	{
		/// <summary>
		/// The volume label.
		/// </summary>
		[XmlElement("VolumeLabel")]
		public string VolumeLabel = string.Empty;

		/// <summary>
		/// The volume serial.
		/// </summary>
		[XmlElement("VolumeSerial")]
		public string VolumeSerial = string.Empty;

		/// <summary>
		/// The File FileAttributes.
		/// </summary>
		[XmlElement("FileAttributes")]
		public SerializableDictionary<string, FileAttributes> FileAttributeList = new SerializableDictionary<string, FileAttributes>();

		/// <summary>
		/// The attributes of the folders.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		[XmlElement("FolderAttributes")]
		public SerializableDictionary<string, FileAttributes> FolderAttributeList = new SerializableDictionary<string, FileAttributes>();

		#region Serialization helpers
		private static XmlSerializer xmlSerializer = null;

		/// <summary>
		/// Loads the content.
		/// </summary>
		/// <param name="serializedData">The serialized data.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		public static VolumeDataStorage DeserializeData(string serializedData)
		{
			if (xmlSerializer == null)
				xmlSerializer = new XmlSerializer(typeof(VolumeDataStorage));

			return (VolumeDataStorage)xmlSerializer.Deserialize(new StringReader(serializedData));
		}

		/// <summary>
		/// Saves the content.
		/// </summary>
		/// <param name="storage">The storage.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		public static string SerializeData(VolumeDataStorage storage)
		{
			if (xmlSerializer == null)
				xmlSerializer = new XmlSerializer(typeof(VolumeDataStorage));

			StringWriter writer = new StringWriter();
			xmlSerializer.Serialize(writer, storage);

			return writer.ToString();
		}
		#endregion
	}
}
