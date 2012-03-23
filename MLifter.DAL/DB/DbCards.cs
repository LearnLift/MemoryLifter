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
using System.Collections.Generic;
using System.Diagnostics;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using MLifter.DAL.Security;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Database implementation if ICards.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-28</remarks>
    class DbCards : ICards
    {
        private IDbCardsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlCardsConnector.GetInstance(parent.GetChildParentClass(this));
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeCardsConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }
        private int LearningModuleId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCards"/> class.
        /// </summary>
        /// <param name="lmId">The lm id.</param>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbCards(int lmId, ParentClass parentClass)
        {
            parent = parentClass;

            LearningModuleId = lmId;
        }

        #region ICards Members

        /// <summary>
        /// Gets the cards.
        /// </summary>
        /// <value>The cards.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IList<ICard> Cards
        {
            get
            {
                // return connector.GetCards(LearningModuleId);
                List<ICard> cards = connector.GetCards(LearningModuleId);
                List<ICard> visibleCards = new List<ICard>();
                foreach (ICard card in cards)
                    if (this.HasPermission(PermissionTypes.Visible))
                        visibleCards.Add(card);
                return visibleCards;
            }
        }

        /// <summary>
        /// Gets the number of cards.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Count
        {
            get
            {
                //return connector.GetCardsCount(LearningModuleId);
                return this.Cards.Count;
            }
        }

        /// <summary>
        /// Creates a new card object
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ICard Create()
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            ICard card = connector.GetNewCard(LearningModuleId);
            return card;
        }

        /// <summary>
        /// Adds the specified card to the dictionary. An exception is thrown if an ID conflict occurs.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <remarks>Documented by Dev03, 2007-09-21</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void Add(ICard card)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            connector.SetCardLearningModule(LearningModuleId, card.Id);
            Log.CardAdded(parent);
        }

        /// <summary>
        /// Creates a new card and appends it to the dictionary.
        /// </summary>
        /// <returns>The new card.</returns>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ICard AddNew()
        {
            ICard newCard = Create();
            Add(newCard);
            return newCard;
        }

        /// <summary>
        /// Copies the given card from one dictionary to this dictionary and returns the new ID.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-21</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int CopyCard(ICard card)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            ICard newCard = Create();
            card.CopyTo(newCard, null);
            Add(newCard);
            return newCard.Id;
        }

        /// <summary>
        /// Loads a card from Xml and returns it. The card is not assigned.
        /// </summary>
        /// <param name="xmlCard">The XML card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-21</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ICard LoadCardFromXml(string xmlCard)
        {
            Debug.WriteLine("The method or operation is not implemented.");
            return null;
        }

        /// <summary>
        /// Deletes the specified card.
        /// </summary>
        /// <param name="card_id">The card id.</param>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void Delete(int card_id)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();

            Log.CardFromBoxDeleted(Parent, Get(card_id).Box);
            connector.DeleteCard(card_id, LearningModuleId);
        }

        /// <summary>
        /// Gets the specified card.
        /// </summary>
        /// <param name="card_id">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ICard Get(int card_id)
        {
            ICard card = new DbCard(card_id, parent.GetChildParentClass(this));
            if (!this.HasPermission(PermissionTypes.Visible))
                throw new PermissionException();
            return card;
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
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public List<ICard> GetCards(QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number)
        {
            List<ICard> cards = connector.GetCardsByQuery(LearningModuleId, query, orderBy, orderDir, number);
            List<ICard> visibleCards = new List<ICard>();
            foreach (ICard card in cards)
                if (this.HasPermission(PermissionTypes.Visible))
                    visibleCards.Add(card);
            return visibleCards;
        }

        /// <summary>
        /// Resets the boxes of all cards.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-08</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void ClearAllBoxes()
        {
            connector.ClearAllBoxes(LearningModuleId);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ParentClass Parent { get { return parent; } }

        #endregion

        #region ICopy Members

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void CopyTo(ICopy target, CopyToProgress progressDelegate)
        {
            CardsHelper.Copy(this, target as ICards, progressDelegate);
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
            return Parent.CurrentUser.HasPermission(this, permissionName);
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            return Parent.CurrentUser.GetPermissions(this);
        }

        #endregion
    }
}
