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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MLifter.AudioTools
{
	public delegate void EmtyDelegate();
	public delegate void EventDelegate(EventArgs e);
	public delegate void ColorDelegate(Color color);

	/// <summary>
	/// The possible functions of the control / class.
	/// </summary>
	public enum Function
	{
		/// <summary>
		/// If the control has no function or isn't doing anything.
		/// </summary>
		Nothing,
		/// <summary>
		/// If the control is recording or the button is to start the recording.
		/// </summary>
		Record,
		/// <summary>
		/// If the control is playing or the button is to start playing.
		/// </summary>
		Play,
		/// <summary>
		/// If the button is to stop recording.
		/// </summary>
		StopRecording,
		/// <summary>
		/// If the button is to stop playing.
		/// </summary>
		StopPlaying,
		/// <summary>
		/// If the button is to go to the next card.
		/// </summary>
		Forward,
		/// <summary>
		/// If the button is to go to the previous card.
		/// </summary>
		Backward
	}

	/// <summary>
	/// The different available font sizes.
	/// </summary>
	public enum FontSizes
	{
		Small,
		Medium,
		Large,
		Automatic
	}

	/// <summary>
	/// The different types of media items.
	/// </summary>
	public enum MediaItemType
	{
		Question,
		QuestionExample,
		Answer,
		AnswerExample
	}

	/// <summary>
	/// The last excecuted command.
	/// </summary>
	public enum LastCommand
	{
		Next,
		Back
	}
	
	/// <summary>
	/// Class which provides some heling methods.
	/// </summary>
	/// <remarks>Documented by Dev05, 2007-08-08</remarks>
	public static class Tools
	{
		/// <summary>
		/// Gets the valid filepath.
		/// </summary>
		/// <param name="OldFilepath">The old filepath.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		public static string GetValidFilename(string OldFilename)
		{
			string newFilename = System.Text.RegularExpressions.Regex.Replace(Path.GetFileNameWithoutExtension(OldFilename), @"[^\w-\\ ]", "") + Path.GetExtension(OldFilename);
			
			return newFilename;
		}
	}

	/// <summary>
	/// General constants.
	/// </summary>
	/// <remarks>Documented by Dev05, 2007-08-03</remarks>
	public class CONSTANTS
	{
		public static readonly string SETTINGS_FILENAME = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
			MLifter.Recorder.Properties.Settings.Default.SettingsPath, "settings.xml");
		public const int START_RECORDING_DELAY = 250;
		public const int STOP_RECORDING_DELAY = 200;
		public const bool DELAYS_ACTIVE = true;
		public const bool ALLOW_MULTIPLE_ASSIGNMENT = false;
		public const bool PRESENTER_ACTIVATED = true;
	}

	/// <summary>
	/// The standard appearance of the programm.
	/// </summary>
	public class STANDARD_APPEARANCE
	{
		public const bool ADVANCEDVIEW = true;
		public const bool KEYBOARDLAYOUT = false;
		public const bool LOCK_FUNCTIONS_IN_SIMPLE_VIEW = true;

		public const int FONT_SIZE_SMALL = 24;
		public const int FONT_SIZE_MEDIUM = 36;
		public const int FONT_SIZE_LARGE = 56;
		public const int FONT_SIZE_AUTOMATIC_LOWER_LIMIT = 16;
		public const int FONT_SIZE_AUTOMATIC_UPPER_LIMIT = 72;

		public static readonly Color COLOR_RECORD = Color.Red;
		public static readonly Color COLOR_PLAY = Color.Green;
		public static readonly Color COLOR_STOP = Color.Blue;
		public static readonly Color COLOR_MOVE = Color.Yellow;
		public const int COLOR_MOVE_TIME = 50;

		public static readonly Color COLOR_CARD_COMPLETE = Color.LightGreen;
		public static readonly Color COLOR_CARD_NOTHING = Color.Gainsboro;
		public static readonly Color COLOR_CARD_PARTS = Color.BurlyWood;
		public static readonly Color COLOR_CARD_NOTSELECTED = Color.DimGray;
		public static readonly Color COLOR_CARD_SEPARATOR = Color.Gray;
	}

	/// <summary>
	/// The standard Key-Assignment.
	/// </summary>
	public class STANDART_KEYS
	{
		public const Keys SWITCH1 = Keys.NumLock;
		public const Keys SWITCH2 = Keys.Back;
		public const Keys SWITCH3 = Keys.Tab;

		public const Keys PLAY = Keys.Enter;
		public const Keys RECORD1 = Keys.NumPad0;
		public const Keys RECORD2 = Keys.Space;
		public const Keys NEXT = Keys.NumPad3;
		public const Keys BACK = Keys.Decimal;

		public const Keys PRESENTER_RECORD1 = Keys.F5;
		public const Keys PRESENTER_RECORD2 = Keys.Escape;
		public const Keys PRESENTER_NEXT = Keys.PageDown;
		public const Keys PRESENTER_BACK = Keys.PageUp;
	}

	/// <summary>
	/// The standard functions of the NumPad-Keys.
	/// </summary>
	public class STANDART_FUNCTIONS
	{
		public const bool AUTOPLAY = false;

		public const Function KEY_0 = Function.Record;
		public const Function KEY_1 = Function.Nothing;
		public const Function KEY_2 = Function.Nothing;
		public const Function KEY_3 = Function.Forward;
		public const Function KEY_4 = Function.Nothing;
		public const Function KEY_5 = Function.Nothing;
		public const Function KEY_6 = Function.Nothing;
		public const Function KEY_7 = Function.Nothing;
		public const Function KEY_8 = Function.Nothing;
		public const Function KEY_9 = Function.Nothing;
		public const Function KEY_COMMA = Function.Backward;
		public const Function KEY_ENTER = Function.Play;
		public const Function KEY_PLUS = Function.Nothing;
		public const Function KEY_MINUS = Function.Nothing;
		public const Function KEY_MULTIPLY = Function.Nothing;
		public const Function KEY_DIVIDE = Function.Nothing;

		public const Function KEY_SPACE = Function.Record;
		public const Function KEY_C = Function.Nothing;
		public const Function KEY_V = Function.Backward;
		public const Function KEY_B = Function.Play;
		public const Function KEY_N = Function.Forward;
		public const Function KEY_M = Function.Nothing;
	}

	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		/// <summary>
		/// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();
			if (wasEmpty)
			{
				return;
			}
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");

				//Shorter version, but only for simple types (int, string, enum,...)
				//TKey key = (TKey)GetValue(typeof(TKey), reader.ReadElementString("key"));
				//TValue value = (TValue)GetValue(typeof(TValue), reader.ReadElementString("value"));

				reader.ReadStartElement("key");
				TKey key = (TKey)keySerializer.Deserialize(reader);
				reader.ReadEndElement();

				reader.ReadStartElement("value");
				TValue value = (TValue)valueSerializer.Deserialize(reader);
				reader.ReadEndElement();

				this.Add(key, value);

				reader.ReadEndElement();
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Parses the specified string read form the XML and returns the value
		/// it represents as the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		private object GetValue(Type type, string value)
		{
			if (type == typeof(string))
			{
				return value;
			}
			else if (type == typeof(bool))
			{
				return Convert.ToBoolean(value);
			}
			else if (type.IsEnum)
			{
				return Enum.Parse(type, value, true);
			}
			else if (type == typeof(DateTime))
			{
				if (value.Length == 0)
				{
					return DateTime.MinValue;
				}
				else
				{
					return DateTime.Parse(value);
					//return Convert.ToDateTime(value);
				}
			}
			else if (type == typeof(TimeSpan))
			{
				return TimeSpan.Parse(value); //Convert.Tot(value);
			}
			else if (type == typeof(Int16))
			{
				return Convert.ToInt16(value);
			}
			else if (type == typeof(Int32))
			{
				return Convert.ToInt32(value);
			}
			else if (type == typeof(Int64))
			{
				return Convert.ToInt64(value);
			}
			else if (type == typeof(float))
			{
				return Convert.ToSingle(value);
			}
			else if (type == typeof(double))
			{
				return Convert.ToDouble(value);
			}
			else if (type == typeof(decimal))
			{
				return Convert.ToDecimal(value);
			}
			else if (type == typeof(char))
			{
				return Convert.ToChar(value);
			}
			else if (type == typeof(byte))
			{
				return Convert.ToByte(value);
			}
			else if (type == typeof(UInt16))
			{
				return Convert.ToUInt16(value);
			}
			else if (type == typeof(UInt32))
			{
				return Convert.ToUInt32(value);
			}
			else if (type == typeof(UInt64))
			{
				return Convert.ToUInt64(value);
			}
			else if (type == typeof(Guid))
			{
				return new Guid(value);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

			foreach (TKey key in this.Keys)
			{
				writer.WriteStartElement("item");

				//Shorter version, but only for simple types (int, string, enum,...)
				//writer.WriteElementString("key", key.ToString());
				//writer.WriteElementString("value", this[key].ToString());

				writer.WriteStartElement("key");
				keySerializer.Serialize(writer, key);
				writer.WriteEndElement();

				writer.WriteStartElement("value");
				TValue value = this[key];

				valueSerializer.Serialize(writer, value);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
		}
	}
}
