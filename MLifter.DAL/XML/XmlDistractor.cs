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
