using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.ComponentModel;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	internal class XmlWord : IWord, INotifyPropertyChanged
	{
		private int m_id = -1;
		private string m_word;
		private WordType m_type;
		private bool m_default;

		internal XmlWord(string word, WordType type, bool isDefault, ParentClass parent)
		{
			m_word = (word == null) ? String.Empty : word;
			m_type = type;
			m_default = isDefault;
			this.parent = parent;
		}

		#region IWord Members

		public int Id
		{
			get
			{
				return m_id;
			}
			set
			{
				if (m_id != value)
				{
					m_id = value;
					OnPropertyChanged("Id");
				}
			}
		}

		public string Word
		{
			get
			{
				return m_word;
			}
			set
			{
				if (m_word != value)
				{
					m_word = value;
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
				if (m_type != value)
				{
					m_type = value;
					OnPropertyChanged("Type");
				}
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
				if (m_default != value)
				{
					m_default = value;
					OnPropertyChanged("Default");
				}
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
