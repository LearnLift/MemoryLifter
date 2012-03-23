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
using MLifter.DAL.Tools;

namespace MLifter.DAL.Preview
{
    /// <summary>
    /// This is a container object which is used to preview cards.
    /// It does not implement any data persistence!
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-25</remarks>
    public class PreviewCards : ICards
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewCards"/> class.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public PreviewCards(ParentClass parentClass)
        {
            parent = parentClass;
        }

        IList<ICard> cards = new List<ICard>();
        #region ICards Members

        /// <summary>
        /// Gets the cards.
        /// </summary>
        /// <value>The cards.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public IList<ICard> Cards
        {
            get { return cards; }
        }

        /// <summary>
        /// Gets the number of cards.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public int Count
        {
            get { return cards.Count; }
        }

        /// <summary>
        /// Creates a new card object
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public ICard Create()
        {
            return new PreviewCard(parent.GetChildParentClass(this));
        }

        /// <summary>
        /// Adds the specified card to the dictionary. An exception is thrown if an ID conflict occurs.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <remarks>Documented by Dev03, 2007-09-21</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public void Add(ICard card)
        {
            cards.Add(card); ;
        }

        /// <summary>
        /// Creates a new card and appends it to the dictionary.
        /// </summary>
        /// <returns>The new card.</returns>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public ICard AddNew()
        {
            ICard card = Create();
            cards.Add(card); ;
            return card;
        }

        /// <summary>
        /// Copies the given card from one dictionary to this dictionary and returns the new ID.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-21</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public int CopyCard(ICard card)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a card from Xml and returns it. The card is not assigned.
        /// </summary>
        /// <param name="xmlCard">The XML card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-21</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public ICard LoadCardFromXml(string xmlCard)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified card.
        /// </summary>
        /// <param name="card_id">The card id.</param>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public void Delete(int card_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the specified card.
        /// </summary>
        /// <param name="card_id">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public ICard Get(int card_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of cards specified by a query.
        /// If the query array is null or the lenght is 0 then all cards are returned.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderDir">The order dir.</param>
        /// <param name="number">The number of cards to return (0 for all).</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public List<ICard> GetCards(QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the boxes of all cards.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-08</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public void ClearAllBoxes()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public ParentClass Parent { get { return parent; } }

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

        #region ISecurity Members

        /// <summary>
        /// Determines whether the object has the specified permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns>
        /// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public bool HasPermission(string permissionName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-03-23</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
