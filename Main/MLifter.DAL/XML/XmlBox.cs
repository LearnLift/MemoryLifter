using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Properties;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// The IBox implementation for XML learning modules.
	/// </summary>
	public class XmlBox : IBox
	{
		private XmlDictionary m_Dictionary;
		private int m_Id;

		private int m_Size = -1;
		private int m_QuerySize = -1;
		private int m_MaximalSize = -1;

		private const string m_XPathBoxsize = "/dictionary/user/boxsize[@id={0}]/text()";

		private readonly string m_dictionaryTemplate = Resources.BlankDictionaryV2_0;

		internal XmlBox(XmlDictionary dictionary, int boxId, ParentClass parent)
		{
			m_Dictionary = dictionary;
			m_Id = boxId;
			Parent = parent;
		}

		#region IBox Members

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public int Id
		{
			get { return m_Id; }
		}

		/// <summary>
		/// Gets the current box size.
		/// </summary>
		/// <value>
		/// The box size.
		/// </value>
		public int CurrentSize
		{
			get
			{
				UpdateSize();
				return m_Size;
			}
		}

		/// <summary>
		/// Gets the box size based on query chapters.
		/// </summary>
		/// <value>
		/// The box size.
		/// </value>
		public int Size
		{
			get
			{
				if (m_QuerySize < 0)
					UpdateQuerySize();
				return m_QuerySize;
			}
			internal set
			{
				if (m_QuerySize < 0)
					UpdateQuerySize();
				else
					m_QuerySize = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximal box size.
		/// </summary>
		/// <value>
		/// The maximal box size.
		/// </value>
		public int MaximalSize
		{
			get
			{
				if ((m_Id == 0) || (m_Id == m_Dictionary.NumberOfBoxes))
					return m_Dictionary.Cards.Count;
				else
					return m_MaximalSize;
			}
			set
			{
				m_MaximalSize = value;
			}
		}

		/// <summary>
		/// Gets the default box size.
		/// </summary>
		/// <value>
		/// The default box size.
		/// </value>
		public int DefaultSize
		{
			get
			{
				if ((m_Id == 0) || (m_Id == m_Dictionary.NumberOfBoxes))
				{
					return m_Dictionary.Cards.Count;
				}
				else
				{
					try
					{
						XmlDocument dictionary = new XmlDocument();
						dictionary.LoadXml(m_dictionaryTemplate);
						return XmlConvert.ToInt32(dictionary.SelectSingleNode(String.Format(m_XPathBoxsize, m_Id)).InnerText);
					}
					catch
					{
						return m_MaximalSize;
					}
				}
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
			if (m_Id == 0)
				return Properties.Resources.BOX_NAME_POOL;
			else
				return m_Id.ToString();
		}

		/// <summary>
		/// Updates the size.
		/// </summary>
		internal void UpdateSize()
		{
			m_Size = m_Dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(-1, m_Id, QueryCardState.Active) }, QueryOrder.None, QueryOrderDir.Ascending, 0).Count;
		}

		/// <summary>
		/// Updates the size of the query.
		/// </summary>
		internal void UpdateQuerySize()
		{
			int counter = 0;
			foreach (ICard card in m_Dictionary.Cards.GetCards(new QueryStruct[] { new QueryStruct(-1, m_Id, QueryCardState.Active) }, QueryOrder.None, QueryOrderDir.Ascending, 0))
				if (m_Dictionary.QueryChapter.Contains(card.Chapter)) counter++;
			m_QuerySize = counter;
		}

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get; private set; }

		#endregion
	}
}
