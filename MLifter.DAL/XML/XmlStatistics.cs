using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// XmlStatistics
	/// </summary>
	public class XmlStatistics : IStatistics
	{
		private XmlDictionary m_oDictionary;
		private XmlDocument m_dictionary;

		private const string m_basePath = "/dictionary/stats";
		private const string m_XPathAttributeId = "id";
		private const string m_XPathId = "@id";
		//required for GetNextId()
		private XPathNavigator m_navigator;
		private XPathExpression m_expression;

		private List<IStatistic> stats = new List<IStatistic>();

		internal XmlStatistics(XmlDictionary dictionary)
		{
			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;
			PrepareIdNavigator();

			XmlNodeList xmlStats = m_dictionary.SelectNodes(m_basePath);
			foreach (XmlNode node in xmlStats)
				stats.Add(new XmlStatistic(dictionary, node as XmlElement));
		}

		/// <summary>
		/// Prepares the Xpath navigator to improve performance for GetNextId().
		/// </summary>
		private void PrepareIdNavigator()
		{
			m_navigator = m_dictionary.CreateNavigator();
			m_expression = m_navigator.Compile(m_basePath);
			m_expression.AddSort(m_XPathId, XmlSortOrder.Descending, XmlCaseOrder.None, String.Empty, XmlDataType.Number);
		}

		/// <summary>
		/// Gets the next id.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		internal int GetNextId()
		{
			int lastId = 0;
			XPathNodeIterator stats = m_navigator.Clone().Select(m_expression);
			if (stats.MoveNext())
			{
				if (stats.Current.MoveToAttribute(m_XPathAttributeId, String.Empty))
				{
					if (Int32.TryParse(stats.Current.Value, out lastId))
					{
						return ++lastId;
					}
				}
				throw new IdOverflowException();
			}
			else
			{
				return lastId;
			}
		}

		#region IList<IStatistic> Members

		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <returns>
		/// The index of <paramref name="item"/> if found in the list; otherwise, -1.
		/// </returns>
		public int IndexOf(IStatistic item)
		{
			return stats.IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		///   </exception>
		public void Insert(int index, IStatistic item)
		{
			if (item is XmlStatistic)
			{
				if ((item as XmlStatistic).Statistic.OwnerDocument != m_dictionary)
					m_dictionary.DocumentElement.AppendChild((item as XmlStatistic).Statistic);
				stats.Insert(index, item);
			}
			else
				throw new ArgumentException("Can only add XmlStatistics");
		}

		/// <summary>
		/// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		///   </exception>
		public void RemoveAt(int index)
		{
			m_dictionary.RemoveChild((stats[index] as XmlStatistic).Statistic);
			stats.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>
		/// The element at the specified index.
		///   </returns>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		///   </exception>
		public IStatistic this[int index]
		{
			get
			{
				return stats[index];
			}
			set
			{
				if (!(value is XmlStatistic))
					throw new ArgumentException("Can only add XmlStatistics");

				m_dictionary.RemoveChild((stats[index] as XmlStatistic).Statistic);
				m_dictionary.DocumentElement.AppendChild((value as XmlStatistic).Statistic);
				stats[index] = value;
			}
		}

		#endregion

		#region ICollection<IStatistic> Members

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		public void Add(IStatistic item)
		{
			if (item is XmlStatistic)
			{
				if ((item as XmlStatistic).Statistic.OwnerDocument != m_dictionary)
					m_dictionary.DocumentElement.AppendChild((item as XmlStatistic).Statistic);
				stats.Add(item);
			}
			else
				throw new ArgumentException("Can only add XmlStatistics");
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		public void Clear()
		{
			XmlNodeList stats = m_dictionary.SelectNodes(m_basePath);
			foreach (XmlNode node in stats)
				node.ParentNode.RemoveChild(node);
			this.stats.Clear();
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		public bool Contains(IStatistic item)
		{
			return stats.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="array"/> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="arrayIndex"/> is less than 0.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="array"/> is multidimensional.
		/// -or-
		/// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
		/// -or-
		/// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
		/// </exception>
		public void CopyTo(IStatistic[] array, int arrayIndex)
		{
			stats.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		public int Count { get { return stats.Count; } }

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		/// </returns>
		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		public bool Remove(IStatistic item)
		{
			m_dictionary.RemoveChild((item as XmlStatistic).Statistic);
			return stats.Remove(item);
		}

		#endregion

		#region IEnumerable<IStatistic> Members

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<IStatistic> GetEnumerator()
		{
			return (stats as IEnumerable<IStatistic>).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return stats.GetEnumerator();
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// CopyTo method
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev08, 2009-02-06</remarks>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
		{
			IStatistics targetStatistics = target as IStatistics;
			if (targetStatistics != null)
				StatisticsHelper.Copy(this as IStatistics, targetStatistics, progressDelegate);
		}

		#endregion
	}
}
