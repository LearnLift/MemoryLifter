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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MLifter.Generics
{
    /// <summary>
    /// A List which reports its changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>Documented by Dev05, 2009-03-12</remarks>
    public class ObservableList<T> : IList<T>
    {
        private List<T> internalList = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList&lt;T&gt;"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-09-25</remarks>
        public ObservableList() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <remarks>Documented by Dev05, 2009-09-25</remarks>
        public ObservableList(IEnumerable<T> collection)
        {
            AddRange(collection);
        }

        /// <summary>
        /// Occurs when the list was changed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public event EventHandler<ObservableListChangedEventArgs<T>> ListChanged;
        /// <summary>
        /// Raises the <see cref="E:ListChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MLifter.Components.ObservableListChangedEventArgs&lt;T&gt;"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        protected virtual void OnListChanged(ObservableListChangedEventArgs<T> e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ObservableList_Item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        private void ObservableList_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnListChanged(new ObservableListChangedEventArgs<T>((T)sender, ListChangedType.ItemChanged));
        }

        /// <summary>
        /// Moves the specified item from the old index to the new index.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;
            if (oldIndex >= internalList.Count || newIndex >= internalList.Count || oldIndex < 0 || newIndex < 0)
                throw new ArgumentOutOfRangeException();

            T item = internalList[oldIndex];
            internalList.RemoveAt(oldIndex);
            internalList.Insert(newIndex, item);

            OnListChanged(new ObservableListChangedEventArgs<T>(item, ListChangedType.ItemMoved, oldIndex, newIndex));
        }

        #region List<T> Paththrough
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void AddRange(IEnumerable<T> collection)
        {
            int cnt = internalList.Count;
            internalList.AddRange(collection);

            if (internalList.Count != cnt)
                OnListChanged(new ObservableListChangedEventArgs<T>(collection.GetEnumerator().Current, ListChangedType.Reset));
        }

        /// <summary>
        /// Returns a read-only <see cref="T:System.Collections.Generic.IList`1"/> wrapper for the current collection.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return internalList.AsReadOnly();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IList`1"/> contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public bool Exists(Predicate<T> match)
        {
            return internalList.Exists(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public T Find(Predicate<T> match)
        {
            return internalList.Find(match);
        }
        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public List<T> FindAll(Predicate<T> match)
        {
            return internalList.FindAll(match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index of the first occurrence within the <see cref="T:System.Collections.Generic.IList`1"/> or a portion of it.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public int FindIndex(Predicate<T> match)
        {
            return internalList.FindIndex(match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public T FindLast(Predicate<T> match)
        {
            return internalList.FindLast(match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index of the last occurrence within the <see cref="T:System.Collections.Generic.IList`1"/> or a portion of it.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public int FindLastIndex(Predicate<T> match)
        {
            return internalList.FindLastIndex(match);
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void ForEach(Action<T> action)
        {
            internalList.ForEach(action);

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <remarks>Documented by Dev05, 2009-04-01</remarks>
        public void RemoveAll(Predicate<T> match)
        {
            var items = internalList.FindAll(match);
            foreach (T item in items)
            {
                internalList.Remove(item);
                OnListChanged(new ObservableListChangedEventArgs<T>(item, ListChangedType.ItemDeleted));
            }
        }

        /// <summary>
        /// Reverses the order of the elements in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Reverse()
        {
            internalList.Reverse();

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }
        /// <summary>
        /// Reverses the order of a portion of the elements in the <see cref="T:System.Collections.Generic.IList`1"/> or  of it.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Reverse(int index, int count)
        {
            internalList.Reverse(index, count);

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.IList`1"/> using the default comparer.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Sort()
        {
            internalList.Sort();

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }
        /// <summary>
        /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.IList`1"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Sort(IComparer<T> comparer)
        {
            internalList.Sort(comparer);

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }
        /// <summary>
        /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.IList`1"/> using the specified comparer.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Sort(Comparer<T> comparison)
        {
            internalList.Sort(comparison);

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }
        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="T:System.Collections.Generic.IList`1"/> using the specified comparer.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="comparer">The comparer.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            internalList.Sort(index, count, comparer);

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }

        /// <summary>
        /// Determines whether every element in the <see cref="T:System.Collections.Generic.IList`1"/> matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public bool TrueForAll(Predicate<T> match)
        {
            return internalList.TrueForAll(match);
        }
        #endregion

        #region IList<T> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public int IndexOf(T item)
        {
            return internalList.IndexOf(item);
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
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Insert(int index, T item)
        {
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)) && !Contains(item))
                (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ObservableList_Item_PropertyChanged);

            internalList.Insert(index, item);

            OnListChanged(new ObservableListChangedEventArgs<T>(item, ListChangedType.ItemAdded, index));
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
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void RemoveAt(int index)
        {
            if (internalList.Count <= index)
                return;

            T item = internalList[index];
            internalList.RemoveAt(index);

            OnListChanged(new ObservableListChangedEventArgs<T>(item, ListChangedType.ItemDeleted, index));
        }

        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public T this[int index]
        {
            get
            {
                return internalList[index];
            }
            set
            {
                T oldItem = internalList[index];

                if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)) && !Contains(value))
                    (value as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ObservableList_Item_PropertyChanged);

                internalList[index] = value;

                OnListChanged(new ObservableListChangedEventArgs<T>(oldItem, ListChangedType.ItemDeleted, index));
                OnListChanged(new ObservableListChangedEventArgs<T>(value, ListChangedType.ItemAdded, index));
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Add(T item)
        {
            internalList.Add(item);

            OnListChanged(new ObservableListChangedEventArgs<T>(item, ListChangedType.ItemAdded, IndexOf(item)));
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void Clear()
        {
            internalList.Clear();

            OnListChanged(new ObservableListChangedEventArgs<T>());
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public bool Contains(T item)
        {
            return internalList.Contains(item);
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
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public int Count { get { return internalList.Count; } }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
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
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public bool Remove(T item)
        {
            if (!Contains(item))
                return false;

            int oldIndex = IndexOf(item);
            bool result = internalList.Remove(item);

            if (result)
                OnListChanged(new ObservableListChangedEventArgs<T>(item, ListChangedType.ItemDeleted, oldIndex));

            return result;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Change event of a ObservableList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>Documented by Dev05, 2009-03-12</remarks>
    public class ObservableListChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the type of the list change.
        /// </summary>
        /// <value>The type of the list change.</value>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public ListChangedType ListChangedType { get; private set; }
        /// <summary>
        /// Gets the item which produced the change.
        /// </summary>
        /// <value>The item.</value>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public T Item { get; private set; }
        /// <summary>
        /// Gets the new index. -1 if item was deleted.
        /// </summary>
        /// <value>The new index.</value>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public int NewIndex { get; private set; }
        /// <summary>
        /// Gets the old index. -1 if new item.
        /// </summary>
        /// <value>The old index.</value>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public int OldIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public ObservableListChangedEventArgs()
        {
            ListChangedType = ListChangedType.Reset;
            NewIndex = -1;
            OldIndex = -1;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        private ObservableListChangedEventArgs(T item)
            : this()
        {
            Item = item;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public ObservableListChangedEventArgs(T item, ListChangedType changeType)
            : this(item)
        {
            ListChangedType = changeType;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="index">The index of the item.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public ObservableListChangedEventArgs(T item, ListChangedType changeType, int index)
            : this(item, changeType)
        {
            if (changeType == ListChangedType.ItemDeleted)
                OldIndex = index;
            else
                NewIndex = index;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public ObservableListChangedEventArgs(T item, ListChangedType changeType, int oldIndex, int newIndex)
            : this(item, changeType)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }
}
