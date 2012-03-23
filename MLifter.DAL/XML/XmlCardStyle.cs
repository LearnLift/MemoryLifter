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
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// XML implementation of ICardStyle.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-13</remarks>
	[XmlRoot("cardStyle")]
	public class XmlCardStyle : ICardStyle, IXmlSerializable
	{
		private static Regex m_ResourceFinder = new Regex(@"url\((?<url>[^\)]+)\)");
		private static Regex m_extractSingleMediaId = new Regex(@"^MEDIA\[(\d+)\]$");

		private static XmlSerializer questionSerializer = new XmlSerializer(typeof(XmlTextStyle), new XmlRootAttribute("question"));
		private static XmlSerializer questionExampleSerializer = new XmlSerializer(typeof(XmlTextStyle), new XmlRootAttribute("questionExample"));
		private static XmlSerializer answerSerializer = new XmlSerializer(typeof(XmlTextStyle), new XmlRootAttribute("answer"));
		private static XmlSerializer answerExampleSerializer = new XmlSerializer(typeof(XmlTextStyle), new XmlRootAttribute("answerExample"));
		private static XmlSerializer answerCorrectSerializer = new XmlSerializer(typeof(XmlTextStyle), new XmlRootAttribute("answerCorrect"));
		private static XmlSerializer answerWrongSerializer = new XmlSerializer(typeof(XmlTextStyle), new XmlRootAttribute("answerWrong"));

		private ITextStyle answerStyle = new XmlTextStyle(".answer");
		private ITextStyle answerExampleStyle = new XmlTextStyle(".answerExample");
		private ITextStyle questionStyle = new XmlTextStyle(".question");
		private ITextStyle questionExampleStyle = new XmlTextStyle(".questionExample");
		private ITextStyle answerCorrect = new XmlTextStyle(".answerCorrect");
		private ITextStyle answerWrong = new XmlTextStyle(".answerWrong");

		internal XmlCardStyle() { parent = null; }
		internal XmlCardStyle(ParentClass parentClass) { parent = parentClass; }

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see> as a CSS-Style.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"> as a CSS-Style</see>.
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		public override string ToString()
		{
			string cssString = questionStyle.ToString();
			cssString += questionExampleStyle.ToString();
			cssString += answerStyle.ToString();
			cssString += answerExampleStyle.ToString();
			cssString += answerCorrect.ToString();
			cssString += answerWrong.ToString();

			return cssString;
		}

		/// <summary>
		/// Returns the CSS for this style using the given base path for media URIs.
		/// </summary>
		/// <param name="basePath">The base path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-22</remarks>
		public string ToString(string basePath)
		{
			string cssString = questionStyle.ToString(basePath);
			cssString += questionExampleStyle.ToString(basePath);
			cssString += answerStyle.ToString(basePath);
			cssString += answerExampleStyle.ToString(basePath);
			cssString += answerCorrect.ToString(basePath);
			cssString += answerWrong.ToString(basePath);

			return cssString;
		}

		#region ICardStyle Members

		/// <summary>
		/// Gets the XML.
		/// </summary>
		/// <value>The XML.</value>
		/// <remarks>Documented by Dev11, 2008-08-27</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public string Xml
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = Encoding.Unicode;
				settings.Indent = true;
				XmlWriter writer = XmlWriter.Create(sb, settings);
				writer.WriteStartDocument();
				writer.WriteStartElement("cardStyle");
				WriteXml(writer);
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the question style.
		/// </summary>
		/// <value>The question.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ITextStyle Question
		{
			get
			{
				return questionStyle;
			}
			set
			{
				questionStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets the question example style.
		/// </summary>
		/// <value>The question example.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ITextStyle QuestionExample
		{
			get
			{
				return questionExampleStyle;
			}
			set
			{
				questionExampleStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets the answer style.
		/// </summary>
		/// <value>The answer.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ITextStyle Answer
		{
			get
			{
				return answerStyle;
			}
			set
			{
				answerStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets the answer example style.
		/// </summary>
		/// <value>The answer example.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ITextStyle AnswerExample
		{
			get
			{
				return answerExampleStyle;
			}
			set
			{
				answerExampleStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets the answer correct style.
		/// </summary>
		/// <value>The answer correct.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ITextStyle AnswerCorrect
		{
			get
			{
				return answerCorrect;
			}
			set
			{
				answerCorrect = value;
			}
		}
		/// <summary>
		/// Gets or sets the answer wrong style.
		/// </summary>
		/// <value>The answer wrong.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ITextStyle AnswerWrong
		{
			get
			{
				return answerWrong;
			}
			set
			{
				answerWrong = value;
			}
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-11-07</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public ICardStyle Clone()
		{
			XmlSerializer ser = new XmlSerializer(typeof(XmlCardStyle));
			MemoryStream stream = new MemoryStream();
			ser.Serialize(stream, this);
			stream.Seek(0, SeekOrigin.Begin);
			return ser.Deserialize(stream) as ICardStyle;
		}
		# endregion
		# region IXmlSerializable Members
		/// <summary>
		/// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			bool wasEmpty = reader.IsEmptyElement;

			if (wasEmpty)
				return;

			reader.Read();

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				switch (reader.Name)
				{
					case "question":
						questionStyle = (XmlTextStyle)questionSerializer.Deserialize(reader);
						break;
					case "questionExample":
						questionExampleStyle = (XmlTextStyle)questionExampleSerializer.Deserialize(reader);
						break;
					case "answer":
						answerStyle = (XmlTextStyle)answerSerializer.Deserialize(reader);
						break;
					case "answerExample":
						answerExampleStyle = (XmlTextStyle)answerExampleSerializer.Deserialize(reader);
						break;
					case "answerCorrect":
						answerCorrect = (XmlTextStyle)answerCorrectSerializer.Deserialize(reader);
						break;
					case "answerWrong":
						answerWrong = (XmlTextStyle)answerWrongSerializer.Deserialize(reader);
						break;
				}
			}

			reader.Read();
		}
		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			questionSerializer.Serialize(writer, questionStyle);
			questionExampleSerializer.Serialize(writer, questionExampleStyle);
			answerSerializer.Serialize(writer, answerStyle);
			answerExampleSerializer.Serialize(writer, answerExampleStyle);
			answerCorrectSerializer.Serialize(writer, answerCorrect);
			answerWrongSerializer.Serialize(writer, answerWrong);
		}
		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public MLifter.DAL.Tools.ParentClass Parent { get { return parent; } internal set { parent = value; } }

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(ICardStyle), progressDelegate);
			if (target is MLifter.DAL.DB.DbCardStyle)
			{
				string basePath = Path.GetDirectoryName(this.Parent.GetParentDictionary().Connection);
				ITextStyle[] styles = { (target as MLifter.DAL.DB.DbCardStyle).Question, (target as MLifter.DAL.DB.DbCardStyle).QuestionExample, (target as MLifter.DAL.DB.DbCardStyle).Answer, (target as MLifter.DAL.DB.DbCardStyle).AnswerExample, (target as MLifter.DAL.DB.DbCardStyle).AnswerCorrect, (target as MLifter.DAL.DB.DbCardStyle).AnswerWrong };
				foreach (ITextStyle style in styles)
				{
					string[] keys = new String[style.OtherElements.Keys.Count];
					style.OtherElements.Keys.CopyTo(keys, 0);
					foreach (string key in keys)
					{
						String value = style.OtherElements[key];
						Match urlValue = m_ResourceFinder.Match(value);
						Match m = m_ResourceFinder.Match(value);
						if (m.Success)
						{
							string url = m.Groups["url"].Value.Trim(new char[] { '"', '\'' });
							Uri uri;
							Match extractId = m_extractSingleMediaId.Match(url);
							if (!extractId.Success)
							{
								if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
								{
									uri = new Uri(url);
								}
								else
								{
									uri = new Uri(new Uri(basePath + "/"), url);
								}
								style.OtherElements[key] = style.OtherElements[key].Replace(url, uri.AbsoluteUri);
							}
						}
					}
				}
				(target as MLifter.DAL.DB.DbCardStyle).FlushToDB();
			}
		}

		#endregion

		#region ISecurity Members

		/// <summary>
		/// Determines whether the object has the specified permission.
		/// </summary>
		/// <param name="permissionName">Name of the permission.</param>
		/// <returns>
		/// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public bool HasPermission(string permissionName)
		{
			return true;
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			return new List<SecurityFramework.PermissionInfo>();
		}

		#endregion
	}

	/// <summary>
	/// XML implementaion of ITextStyle.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-13</remarks>
	public class XmlTextStyle : ITextStyle, IXmlSerializable
	{
		private string name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get { return name; } set { name = value; } }

		private Color forecolor = Color.Empty;
		private Color backcolor = Color.Empty;
		private FontFamily fontFamily = null;
		private CSSFontStyle fontStyle = CSSFontStyle.None;
		private int fontSize = 0;
		private FontSizeUnit fontSizeUnit = FontSizeUnit.Pixel;
		private VerticalAlignment valign = VerticalAlignment.None;
		private HorizontalAlignment halign = HorizontalAlignment.None;
		private SerializableDictionary<string, string> otherElements = new SerializableDictionary<string, string>();

		private static Regex m_ResourceFinder = new Regex(@"url\((?<url>[^\)]+)\)");
		private static XmlSerializer otherElementsSerializer = new XmlSerializer(typeof(SerializableDictionary<string, string>), new XmlRootAttribute("OtherElements"));

		private XmlTextStyle() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlTextStyle"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public XmlTextStyle(string name) { Name = name; }

		/// <summary>
		/// Returns the CSS for this style using the given base path for media URIs.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		/// <remarks>
		/// Documented by AAB, 22.2.2008.
		/// </remarks>
		public override string ToString()
		{
			return ToString(String.Empty);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see> as a CSS-Style.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"> as a CSS-Style</see>.
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		public string ToString(string basePath)
		{
			string cssString = "\n" + Name + "\n" +
							   "{\n";
			if (forecolor.Name != "Empty" && forecolor.Name != "0")
				cssString += "\tcolor:\t\t\t\t" + string.Format("#{0:x2}{1:x2}{2:x2};", forecolor.R, forecolor.G, forecolor.B) + "\n";
			if (backcolor.Name != "Empty" && backcolor.Name != "0")
				cssString += "\tbackground-color:\t" + string.Format("#{0:x2}{1:x2}{2:x2};", backcolor.R, backcolor.G, backcolor.B) + "\n";
			if (fontFamily != null)
				cssString += "\tfont-family:\t\t" + fontFamily.Name + ";\n";
			if ((fontStyle & CSSFontStyle.Bold) == CSSFontStyle.Bold)
				cssString += "\tfont-weight:\t\tbold;\n";
			if ((fontStyle & CSSFontStyle.Italic) == CSSFontStyle.Italic)
				cssString += "\tfont-style:\t\t\titalic;\n";
			if ((fontStyle & CSSFontStyle.Strikeout) == CSSFontStyle.Strikeout)
				cssString += "\ttext-decoration:\tline-through" + ((fontStyle & CSSFontStyle.Underline) == CSSFontStyle.Underline ? ", underline" : "") + ";\n";
			else if ((fontStyle & CSSFontStyle.Underline) == CSSFontStyle.Underline)
				cssString += "\ttext-decoration:\tunderline;\n";
			else if ((fontStyle & CSSFontStyle.Regular) == CSSFontStyle.Regular && (fontStyle & CSSFontStyle.None) != CSSFontStyle.None)
				cssString += "\ttext-decoration:none;\n";
			if (fontSize > 0)
				cssString += "\tfont-size:\t\t\t" + fontSize.ToString() + (fontSizeUnit == FontSizeUnit.Pixel ? "px" : "%") + ";\n";
			if (valign != VerticalAlignment.None)
				cssString += "\tvertical-align:\t\t" + valign.ToString().ToLower() + ";\n";
			if (halign != HorizontalAlignment.None)
				cssString += "\ttext-align:\t\t\t" + halign.ToString().ToLower() + ";\n";

			foreach (KeyValuePair<string, string> pair in otherElements)
			{
				string item = pair.Key + ":";
				//[ML-1210] Card Style rendering is slow - this was never visible and only for cosmetic purposes - removed
				//float width = Graphics.FromImage(new Bitmap(10, 10)).MeasureString(item, new Font("Courier New", 12)).Width;
				//if (width < 36)
				//    item += "\t\t\t\t\t";
				//else if (width < 80)
				//    item += "\t\t\t\t";
				//else if (width < 115)
				//    item += "\t\t\t";
				//else if (width < 160)
				//    item += "\t\t";
				//else
				//    item += "\t";
				item += "\t";
				if (string.IsNullOrEmpty(basePath))
				{
					cssString += "\t" + item + "\t" + pair.Value + ";\n";
				}
				else
				{
					Match m = m_ResourceFinder.Match(pair.Value);
					if (m.Success)
					{
						string url = m.Groups["url"].Value.Trim(new char[] { '"', '\'' });
						Uri uri;
						if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
						{
							uri = new Uri(url);
						}
						else
						{
							uri = new Uri(new Uri(basePath + "/"), url);
						}
						cssString += "\t" + item + pair.Value.Replace(url, uri.AbsoluteUri) + ";\n";
					}
					else
					{
						cssString += "\t" + item + pair.Value + ";\n";
					}
				}
			}

			cssString += "}\n";

			return cssString;
		}

		#region ITextStyle Members
		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <value>The color of the fore.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public Color ForeColor
		{
			get
			{
				return forecolor;
			}
			set
			{
				forecolor = value;
			}
		}
		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public Color BackgroundColor
		{
			get
			{
				return backcolor;
			}
			set
			{
				backcolor = value;
			}
		}
		/// <summary>
		/// Gets or sets the font-family.
		/// </summary>
		/// <value>The font-family.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public FontFamily FontFamily
		{
			get
			{
				return fontFamily;
			}
			set
			{
				fontFamily = value;
			}
		}
		/// <summary>
		/// Gets or sets the font style.
		/// </summary>
		/// <value>The font style.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public CSSFontStyle FontStyle
		{
			get
			{
				return fontStyle;
			}
			set
			{
				fontStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets the size of the font.
		/// </summary>
		/// <value>The size of the font.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public int FontSize
		{
			get
			{
				return fontSize;
			}
			set
			{
				fontSize = value;
			}
		}
		/// <summary>
		/// Gets or sets the font size unit.
		/// </summary>
		/// <value>The font size unit.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public FontSizeUnit FontSizeUnit
		{
			get
			{
				return fontSizeUnit;
			}
			set
			{
				fontSizeUnit = value;
			}
		}
		/// <summary>
		/// Gets or sets the horizontal alignment.
		/// </summary>
		/// <value>The horizontal align.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public HorizontalAlignment HorizontalAlign
		{
			get
			{
				return halign;
			}
			set
			{
				halign = value;
			}
		}
		/// <summary>
		/// Gets or sets the vertical alignment.
		/// </summary>
		/// <value>The vertical align.</value>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public VerticalAlignment VerticalAlign
		{
			get
			{
				return valign;
			}
			set
			{
				valign = value;
			}
		}
		/// <summary>
		/// Gets or sets the other elements.
		/// </summary>
		/// <value>The other elements.</value>
		/// <remarks>Documented by Dev05, 2007-10-30</remarks>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public SerializableDictionary<string, string> OtherElements
		{
			get { return otherElements; }
			set { otherElements = value; }
		}
		# endregion
		# region IXmlSerializable Members
		/// <summary>
		/// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			bool wasEmpty = reader.IsEmptyElement;

			if (wasEmpty)
				return;

			Name = reader.GetAttribute("name");
			halign = (HorizontalAlignment)Helper.GetValue(typeof(HorizontalAlignment), reader.GetAttribute("hAlign"));
			valign = (VerticalAlignment)Helper.GetValue(typeof(VerticalAlignment), reader.GetAttribute("vAlign"));

			reader.Read();

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				switch (reader.Name)
				{
					case "color":
						try { forecolor = (Color)Helper.GetValue(typeof(Color), reader.GetAttribute("fore")); }
						catch { }
						try { backcolor = (Color)Helper.GetValue(typeof(Color), reader.GetAttribute("back")); }
						catch { }
						reader.Read();
						break;
					case "font":
						try { fontSize = Convert.ToInt32(reader.GetAttribute("size")); }
						catch { }
						try { fontSizeUnit = (FontSizeUnit)Enum.Parse(typeof(FontSizeUnit), reader.GetAttribute("unit")); }
						catch { }

						reader.Read();

						while (reader.NodeType != XmlNodeType.EndElement)
						{
							switch (reader.Name)
							{
								case "font-family":
									string fontFamilyString = reader.ReadElementContentAsString();
									try { fontFamily = new FontFamily(fontFamilyString); } //[ML-1252] Missing FontFamily causes an ArgumentException
									catch { }
									break;
								case "font-style":
									string fontStyleString = reader.ReadElementContentAsString();
									try { fontStyle = (CSSFontStyle)Enum.Parse(typeof(CSSFontStyle), fontStyleString); }
									catch { }
									break;
							}
						}
						reader.Read();
						break;
					case "OtherElements":
						otherElements = (SerializableDictionary<string, string>)otherElementsSerializer.Deserialize(reader);
						break;
					default:
						reader.Read();
						break;
				}
			}

			reader.Read();
		}
		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
		/// <remarks>Documented by Dev05, 2007-10-29</remarks>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("hAlign", halign.ToString());
			writer.WriteAttributeString("vAlign", valign.ToString());

			writer.WriteStartElement("color");
			writer.WriteAttributeString("fore", Helper.GetString(typeof(Color), forecolor));
			writer.WriteAttributeString("back", Helper.GetString(typeof(Color), backcolor));
			writer.WriteEndElement();

			writer.WriteStartElement("font");
			writer.WriteAttributeString("size", fontSize.ToString());
			writer.WriteAttributeString("unit", fontSizeUnit.ToString());
			if (fontFamily != null)
				writer.WriteElementString("font-family", fontFamily.Name);
			writer.WriteElementString("font-style", fontStyle.ToString());
			writer.WriteEndElement();

			if (otherElements.Count > 0)
			{
				XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, string>), new XmlRootAttribute("OtherElements"));
				serializer.Serialize(writer, otherElements);
			}
		}
		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2009-01-13</remarks>
		public void CopyTo(ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(ITextStyle), progressDelegate);
		}

		#endregion
	}
}
