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
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.Preview
{
	/// <summary>
	/// This is a container object which is used for preview statistics.
	/// It does not implement any data persistence!
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-03-23</remarks>
	public class PreviewStatistics : IStatistics
	{
		private List<IStatistic> stats = new List<IStatistic>();

		#region IList<IStatistic> Members

		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <returns>
		/// The index of <paramref name="item"/> if found in the list; otherwise, -1.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int IndexOf(IStatistic item)
		{
			return stats.IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		/// </exception>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void Insert(int index, IStatistic item)
		{
			stats.Insert(index, item);
		}

		/// <summary>
		/// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		/// </exception>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void RemoveAt(int index)
		{
			stats.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the <see cref="MLifter.DAL.Interfaces.IStatistic"/> at the specified index.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IStatistic this[int index]
		{
			get
			{
				return stats[index];
			}
			set
			{
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
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void Add(IStatistic item)
		{
			stats.Add(item);
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </exception>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void Clear()
		{
			stats.Clear();
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool Contains(IStatistic item)
		{
			return stats.Contains(item);
		}

		/// <summary>
		/// Copies to the array.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="arrayIndex">Index of the array.</param>
		/// <remarks>
		/// Documented by AAB, 23.03.2009.
		/// </remarks>
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
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Count
		{
			get { return stats.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool IsReadOnly
		{
			get { return false; }
		}

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
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public bool Remove(IStatistic item)
		{
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
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public IEnumerator<IStatistic> GetEnumerator()
		{
			return (stats as IEnumerable<IStatistic>).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return stats.GetEnumerator();
		}

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
