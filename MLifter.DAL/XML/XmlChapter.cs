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
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// Represents an Xml chapter object derived from IChapter
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-07</remarks>
	public class XmlChapter : IChapter
	{
		private XmlDictionary m_oDictionary;
		private XmlDocument m_dictionary;
		private XmlElement m_chapter;

		private ICardStyle m_Style = null;

		private const string m_BaseXPath = "/dictionary/chapter";
		private const string m_XPathIdFilter = "[@id={0}]";
		private const string m_XPathId = "id";
		private const string m_XPathChapter = "chapter";
		private const string m_XPathTitle = "title";
		private const string m_XPathDescription = "description";
		private const string m_XPath_Style = "cardStyle";

		private readonly string m_ChapterFormatString = DAL.Properties.Resources.CHAPTER_FORMAT_STRING;
		private ISettings settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlChapter"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>
		/// Documented by AAB, 7.8.2007.
		/// </remarks>
		internal XmlChapter(XmlDictionary dictionary, ParentClass parent)
		{
			this.parent = parent;
			settings = new XmlChapterSettings(this, Parent.GetChildParentClass(this));

			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;
			XmlElement xeChapter = m_dictionary.CreateElement(m_XPathChapter);
			XmlHelper.CreateAndAppendAttribute(xeChapter, m_XPathId, Convert.ToString(0));
			XmlHelper.CreateAndAppendElement(xeChapter, m_XPathTitle);
			XmlHelper.CreateAndAppendElement(xeChapter, m_XPathDescription);
			m_chapter = xeChapter;

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlChapter"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="chapter">The chapter.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>
		/// Documented by AAB, 7.8.2007.
		/// </remarks>
		internal XmlChapter(XmlDictionary dictionary, XmlElement chapter,ParentClass parent)
		{
			this.parent = parent;
			settings = new XmlChapterSettings(this, Parent.GetChildParentClass(this));

			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;
			m_chapter = chapter;

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlChapter"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="chapter_id">The chapter_id.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>
		/// Documented by AAB, 7.8.2007.
		/// </remarks>
		internal XmlChapter(XmlDictionary dictionary, int chapter_id,ParentClass parent)
		{
			this.parent = parent;
			settings = new XmlChapterSettings(this, Parent.GetChildParentClass(this));

			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;
			m_chapter = (XmlElement)m_dictionary.SelectSingleNode(m_BaseXPath + String.Format(m_XPathIdFilter, chapter_id.ToString()));
			if (m_chapter == null)
				throw new IdAccessException(chapter_id);

			Initialize();
		}

		private XmlSerializer StyleSerializer
		{
			get
			{
				return m_oDictionary.StyleSerializer;
			}
		}

		#region IChapter Members

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public int Id
		{
			get
			{
				return XmlConvert.ToInt32(m_chapter.GetAttribute(m_XPathId));
			}
			set
			{
				m_chapter.SetAttribute(m_XPathId, XmlConvert.ToString(value));
			}
		}

		/// <summary>
		/// Gets the Xml chapter.
		/// </summary>
		/// <value>The chapter.</value>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		public XmlElement Chapter
		{
			get
			{
				return m_chapter;
			}
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public string Title
		{
			get
			{
				return m_chapter[m_XPathTitle].InnerText;
			}
			set
			{
				m_chapter[m_XPathTitle].InnerText = value;
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		/// <remarks>Documented by Dev03, 2007-08-07</remarks>
		public string Description
		{
			get
			{
				return m_chapter[m_XPathDescription].InnerText;
			}
			set
			{
				m_chapter[m_XPathDescription].InnerText = value;
			}
		}

		/// <summary>
		/// Gets the total number of cards.
		/// </summary>
		/// <value>
		/// The total number of cards.
		/// </value>
		public int Size
		{
			get
			{
				return CalculateChapterSize();
			}
		}

		/// <summary>
		/// Gets the number of active cards.
		/// </summary>
		/// <value>
		/// The number of active cards.
		/// </value>
		public int ActiveSize
		{
			get
			{
				return CalculateChapterActiveSize();
			}
		}

		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>
		/// The style.
		/// </value>
		public ICardStyle Style
		{
			get
			{
				if (m_Style == null)
					ReadStyleFromDOM();
				return m_Style;
			}
			set
			{
				if (SaveStyleToDOM(value))
					m_Style = value;
				else
					m_Style = null;
			}
		}

		/// <summary>
		/// Creates and returns a card style.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		public ICardStyle CreateCardStyle()
		{
			return new XmlCardStyle(parent);
		}

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public ISettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			//return String.Format(m_ChapterFormatString, Id + 1, Title);
			return Title;
		}

		private int CalculateChapterSize()
		{
			return m_oDictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(Id, -1, QueryCardState.All) }, QueryOrder.None, QueryOrderDir.Ascending, 0).Count;
		}

		private int CalculateChapterActiveSize()
		{
			return m_oDictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(Id, -1, QueryCardState.Active) }, QueryOrder.None, QueryOrderDir.Ascending, 0).Count;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private void Initialize()
		{
			//does nothing...
		}

		/// <summary>
		/// Reads the style from DOM.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-11-07</remarks>
		private void ReadStyleFromDOM()
		{
			long lTicks = DateTime.Now.Ticks;
			//System.Diagnostics.Trace.TraceInformation(@"Read chapter style (id={0}) - {1}", this.Id, DateTime.Now.Ticks - lTicks);
			//System.Diagnostics.Trace.Indent();
			XmlElement xeStyle = m_chapter[m_XPath_Style];
			if (xeStyle != null)
			{
				if (xeStyle.HasChildNodes)
				{
					XmlReader xmlReader = new XmlNodeReader(xeStyle);
					if (StyleSerializer.CanDeserialize(xmlReader))
					{
						m_Style = (XmlCardStyle)StyleSerializer.Deserialize(xmlReader);
						(m_Style as XmlCardStyle).Parent = Parent;
						//System.Diagnostics.Trace.TraceInformation(@"did it - {0}", DateTime.Now.Ticks - lTicks);
					}
				}
			}
			//System.Diagnostics.Trace.TraceInformation(@"finished - {0}", DateTime.Now.Ticks - lTicks);
			//System.Diagnostics.Trace.Unindent();
		}

		/// <summary>
		/// Saves the style to the DOM.
		/// </summary>
		/// <param name="style">The style.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-10-30</remarks>
		private bool SaveStyleToDOM(ICardStyle style)
		{
			bool success = false;
			StringBuilder stringBuilder = new StringBuilder();
			TextWriter stringWriter = new StringWriter(stringBuilder);
			try
			{
				StyleSerializer.Serialize(stringWriter, style);
				XmlElement xeStyle = m_chapter[m_XPath_Style];
				if (xeStyle != null)
				{
					m_chapter.RemoveChild(xeStyle);
				}
				XmlDocument xdStyle = new XmlDocument();
				xdStyle.LoadXml(stringBuilder.ToString());
				m_chapter.AppendChild(m_dictionary.ImportNode(xdStyle.DocumentElement, true));
				success = true;
			}
			catch { }
			return success;
		}

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
		{
			IChapter targetChapter = target as IChapter;
			if (targetChapter != null && targetChapter.Settings == null && this.Settings != null)
				targetChapter.Settings = targetChapter.Parent.GetParentDictionary().CreateSettings();

			CopyBase.Copy(this, target, typeof(IChapter), progressDelegate);
		}

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent
		{
			get { return parent; }
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
}
