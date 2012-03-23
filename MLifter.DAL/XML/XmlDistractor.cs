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
using System.ComponentModel;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	internal class XmlDistractor : IWord, INotifyPropertyChanged
	{
		private WordType m_type = WordType.Distractor;
		private bool m_default = false;

		XmlElement m_Distractor;
		protected string m_XPathId = "id";
		protected string m_XPathDistractor = "distractor";

		internal XmlDistractor(XmlElement card, string distractor,ParentClass parent)
		{
			m_Distractor = card.OwnerDocument.CreateElement(m_XPathDistractor);
			XmlHelper.CreateAndAppendAttribute(m_Distractor, m_XPathId, Convert.ToString(-1));
			m_Distractor.InnerText = distractor;
			this.parent = parent;
		}

		internal XmlDistractor(XmlElement distractor,ParentClass parent)
		{
			m_Distractor = distractor;
			this.parent = parent;
		}

		internal XmlElement Distractor
		{
			get
			{
				return m_Distractor;
			}
		}

		#region IDistractor Members

		public int Id
		{
			get
			{
				return XmlConvert.ToInt32(m_Distractor.GetAttribute(m_XPathId));
			}
			set
			{
				m_Distractor.SetAttribute(m_XPathId, XmlConvert.ToString(value));
				OnPropertyChanged("Id");
			}
		}

		public string Word
		{
			get
			{
				return m_Distractor.InnerText;
			}
			set
			{
				if (m_Distractor.InnerText != value)
				{
					m_Distractor.InnerText = value;
					OnPropertyChanged("Word");
				}
			}
		}

		public WordType Type
		{
			get
			{
				return m_type;
			}
			set
			{
				//not implemented;
			}
		}

		public bool Default
		{
			get
			{
				return m_default;
			}
			set
			{
				//not implemented;
			}
		}

		#endregion

		public override string ToString()
		{
			return String.Format("{0} - {1}", Id, Word);
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the <see cref="E:PropertyChanged"/> event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <remarks>
		/// Documented by DAC, 2008-06-05.
		/// </remarks>
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region IParent Members

		private ParentClass parent;

		public ParentClass Parent
		{
			get { return parent; }
		}

		#endregion
	}
}
