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
using System.Collections;

namespace MLifter.Controls.SyntaxHighlightingTextBox
{
	/// <summary>
	/// Summary Description for SeperaratorCollection.
	/// </summary>
	public class SeperaratorCollection
	{
		private ArrayList mInnerList = new ArrayList();
		internal SeperaratorCollection()
		{
		}

		public void AddRange(ICollection c)
		{
			mInnerList.AddRange(c);
		}

		internal char[] GetAsCharArray()
		{
			return (char[])mInnerList.ToArray(typeof(char));
		}
		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return mInnerList.IsReadOnly;
			}
		}

		public char this[int index]
		{
			get
			{
				return (char)mInnerList[index];
			}
			set
			{
				mInnerList[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			mInnerList.RemoveAt(index);
		}

		public void Insert(int index, char value)
		{
			mInnerList.Insert(index, value);
		}

		public void Remove(char value)
		{
			mInnerList.Remove(value);
		}

		public bool Contains(char value)
		{
			return mInnerList.Contains(value);
		}

		public void Clear()
		{
			mInnerList.Clear();
		}

		public int IndexOf(char value)
		{
			return mInnerList.IndexOf(value);
		}

		public int Add(char value)
		{
			return mInnerList.Add(value);
		}

		public bool IsFixedSize
		{
			get
			{
				return mInnerList.IsFixedSize;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return mInnerList.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return mInnerList.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			mInnerList.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return mInnerList.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return mInnerList.GetEnumerator();
		}

		#endregion
	}
}
