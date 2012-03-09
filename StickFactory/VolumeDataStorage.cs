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
