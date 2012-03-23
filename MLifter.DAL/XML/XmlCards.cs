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
using System.Xml.XPath;
using System.ComponentModel;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// XML implementation of ICards.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-15</remarks>
	public class XmlCards : ICards
	{
		int m_currentId = 0;

		private XmlDictionary m_oDictionary;
		private XmlDocument m_dictionary;
		private List<ICard> m_cards = new List<ICard>();
		private const string m_basePath = "/dictionary/card";
		private const string m_AttributeId = "id";
		private const string m_XPathId = "@id";
		private const string m_XPathCard = "card";
		private const string m_XPathLoadCard = "loadcard";
		private const string m_XPathIdFilter = "[@id={0}]";
		//required for GetNextId()
		private XPathNavigator m_navigator;
		private XPathExpression m_expression;

		internal XmlCards(XmlDictionary dictionary, ParentClass parentClass)
		{
			parent = parentClass;

			m_oDictionary = dictionary;
			m_dictionary = dictionary.Dictionary;
			PrepareIdNavigator();
			Initialize();
		}

		/// <summary>
		/// Initializes this instance - reads all cards from the data file.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-08-24</remarks>
		internal void Initialize()
		{
			m_cards.Clear();
			foreach (XmlNode card in m_dictionary.SelectNodes(m_basePath))
			{
				XmlCard xmlCard = new XmlCard(m_oDictionary, (XmlElement)card, parent.GetChildParentClass(this));
				m_cards.Add(xmlCard);
			}
			m_currentId = GetCurrentId();
		}

		#region ICards Members
		/// <summary>
		/// Gets the cards.
		/// </summary>
		/// <value>
		/// The cards.
		/// </value>
		public IList<ICard> Cards
		{
			get
			{
				BindingList<ICard> bindingListCards = new BindingList<ICard>(m_cards);
				bindingListCards.ListChanged += new ListChangedEventHandler(bindingListCards_ListChanged);
				return bindingListCards;
			}
		}

		/// <summary>
		/// Handles the ListChanged event of the bindingListCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-06-05</remarks>
		void bindingListCards_ListChanged(object sender, ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					ICard card = m_cards[e.NewIndex];
					m_cards.RemoveAt(e.NewIndex);   //this is kind of dirty - we need to remove the new item because Add() adds the same card once again -> twice
					Add(card);
					break;
				case ListChangedType.ItemChanged:
					throw new NotImplementedException();
				case ListChangedType.ItemDeleted:
					throw new NotImplementedException(); //we do not know which id was deleted
				case ListChangedType.ItemMoved:
					throw new NotImplementedException();
				default:
					break;
			}
		}

		/// <summary>
		/// Creates a new card object
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-01-04</remarks>
		public ICard Create()
		{
			return new XmlCard(m_oDictionary, parent.GetChildParentClass(this));
		}

		/// <summary>
		/// Adds the specified card to the dictionary. An exception is thrown if an ID conflict occurs.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <remarks>Documented by Dev03, 2007-09-21</remarks>
		public void Add(ICard card)
		{
			if (card.Id < 0)
			{
				((XmlCard)card).Id = GetNextId();
			}
			if (m_oDictionary.Cards.Get(card.Id) != null)
			{
				throw new IdExistsException(card.Id);
			}
			m_dictionary.DocumentElement.AppendChild(m_dictionary.ImportNode((card as XmlCard).Xml, true));
			m_cards.Add(card);
		}

		/// <summary>
		/// Adds the new card to the dictionary.
		/// </summary>
		/// <returns>The card.</returns>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		public ICard AddNew()
		{
			XmlCard newCard = new XmlCard(m_oDictionary, parent.GetChildParentClass(this));
			UpdateCurrentId();
			newCard.Id = GetNextId();
			m_dictionary.DocumentElement.AppendChild(newCard.Xml);
			m_cards.Add(newCard);
			newCard.Active = true;
			PrepareIdNavigator();
			return newCard;
		}

		/// <summary>
		/// Copies the given card from one dictionary to this dictionary and returns the new ID.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-21</remarks>
		/// <remarks>Documented by Dev03, 2008-03-28</remarks>
		public int CopyCard(ICard card)
		{
			((XmlCard)card).Dictionary = m_oDictionary;
			((XmlCard)card).Id = GetNextId();
			m_dictionary.DocumentElement.AppendChild(m_dictionary.ImportNode((card as XmlCard).Xml, true));
			Initialize();
			return card.Id;
		}

		/// <summary>
		/// Loads a card from Xml and returns it. The card is not assigned.
		/// </summary>
		/// <param name="xmlCard">The XML card.</param>
		/// <returns></returns>
		public ICard LoadCardFromXml(string xmlCard)
		{
			XmlElement xeCard = m_dictionary.CreateElement(m_XPathLoadCard);
			xeCard.InnerXml = xmlCard;
			return new XmlCard(m_oDictionary, (XmlElement)xeCard.SelectSingleNode(m_XPathCard), parent.GetChildParentClass(this));
		}

		/// <summary>
		/// Deletes the specified card.
		/// </summary>
		/// <param name="card_id">The card id.</param>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		public void Delete(int card_id)
		{
			//XmlNode nodeToDelete = m_dictionary.SelectSingleNode(m_basePath + String.Format(m_XPathIdFilter, card_id));
			//nodeToDelete.ParentNode.RemoveChild(nodeToDelete);
			System.Diagnostics.Debug.WriteLine("XmlCards.Delete() - start:\t" + DateTime.Now.Ticks);
			XmlCard cardToDelete = (XmlCard)Get(card_id);
			cardToDelete.Active = false;
			//remove from list
			System.Diagnostics.Debug.WriteLine("XmlCards.Delete() - list:\t" + DateTime.Now.Ticks);
			m_cards.Remove(cardToDelete as ICard);
			//remove from DOM
			System.Diagnostics.Debug.WriteLine("XmlCards.Delete() - dom:\t" + DateTime.Now.Ticks);
			(cardToDelete as XmlCard).Xml.ParentNode.RemoveChild((cardToDelete as XmlCard).Xml);
			System.Diagnostics.Debug.WriteLine("XmlCards.Delete() - end:\t" + DateTime.Now.Ticks);
		}

		/// <summary>
		/// Gets the card for the specified id.
		/// </summary>
		/// <param name="card_id">The card id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		public ICard Get(int card_id)
		{
			return m_cards.Find(delegate(ICard card)
					{
						return (card.Id == card_id);
					});
		}

		/// <summary>
		/// Gets the cards.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="orderBy">The order type.</param>
		/// <param name="orderDir">The order direction. The orderDir has no effect if <see cref="T:MLifter.DAL.QueryOrder">orderBy</see> is Random or None.</param>
		/// <param name="number">The number cards to return (0 for all).</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		public List<ICard> GetCards(QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number)
		{
			//filter the cards
			List<ICard> cards = new List<ICard>();
			if ((query != null) && (query.Length != 0))
			{
				foreach (QueryStruct qry in query)
				{
					List<ICard> foundCards = m_cards.FindAll(
						delegate(ICard card)
						{
							bool isMatch = true;
							if (qry.BoxId >= 0) isMatch = isMatch && (qry.BoxId == card.Box);
							if (qry.ChapterId >= 0) isMatch = isMatch && (qry.ChapterId == card.Chapter);
							switch (qry.CardState)
							{
								case QueryCardState.Active:
									isMatch = isMatch && (card.Box >= 0);
									break;
								case QueryCardState.Inactive:
									isMatch = isMatch && (card.Box < 0);
									break;
								case QueryCardState.All:
								default:
									break;
							}
							return isMatch;
						});
					cards.AddRange(foundCards);
				}
			}
			else
			{
				cards.AddRange(m_cards);    //add all cards
			}

			//sort the cards
			if (orderBy != QueryOrder.None)
			{
				if (orderBy == QueryOrder.Random)
				{
					foreach(ICard card in cards)
						((XmlCard)card).Random = m_oDictionary.GetRandomNumber();
				}
				cards.Sort(
					delegate(ICard left, ICard right)
					{
						int result;
						switch (orderBy)
						{
							case QueryOrder.Id:
								result = Comparer<int>.Default.Compare(left.Id, right.Id);
								break;
							case QueryOrder.Chapter:
								result = Comparer<int>.Default.Compare(left.Chapter, right.Chapter);
								break;
							case QueryOrder.Box:
								result = Comparer<int>.Default.Compare(left.Box, right.Box);
								break;
							case QueryOrder.Answer:
								result = Comparer<string>.Default.Compare(left.Answer.ToString(), right.Answer.ToString());
								break;
							case QueryOrder.Question:
								result = Comparer<string>.Default.Compare(left.Question.ToString(), right.Question.ToString());
								break;
							case QueryOrder.Timestamp:
								result = Comparer<DateTime>.Default.Compare(left.Timestamp, right.Timestamp);
								break;
							case QueryOrder.Random:
								result = Comparer<int>.Default.Compare(((XmlCard)left).Random, ((XmlCard)right).Random);
								break;
							default:
								result = 0;
								break;
						}
						return result;
					});
			}

			if ((orderBy != QueryOrder.Random) && (orderBy != QueryOrder.None))
			{
				if (orderDir == QueryOrderDir.Descending)
				{
					cards.Reverse();
				}
			}

			if ((number > 0) && (number < cards.Count))
			{
				return cards.GetRange(0, number); ;
			}
			else
			{
				return cards;
			}
		}

		/// <summary>
		/// Swaps swaps two card ids.
		/// </summary>
		/// <param name="first_id">The first id.</param>
		/// <param name="second_id">The second id.</param>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		public void SwapId(int first_id, int second_id)
		{
			XmlNode firstNode = m_dictionary.SelectSingleNode(m_basePath + String.Format(m_XPathIdFilter, first_id));
			XmlNode secondNode = m_dictionary.SelectSingleNode(m_basePath + String.Format(m_XPathIdFilter, second_id));
			firstNode.Attributes[m_AttributeId].Value = second_id.ToString();
			secondNode.Attributes[m_AttributeId].Value = first_id.ToString();
			Initialize();
		}

		/// <summary>
		/// Gets the number of cards.
		/// </summary>
		/// <value>The number of cards.</value>
		/// <remarks>Documented by Dev03, 2007-08-30</remarks>
		public int Count
		{
			get
			{
				return m_cards.Count;
			}
		}

		/// <summary>
		/// Resets the boxes of all cards.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		public void ClearAllBoxes()
		{
			foreach (ICard card in Cards)
				card.Box = 0;
		}

		#endregion

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
		/// <returns>The next free id value.</returns>
		/// <remarks>Documented by Dev03, 2007-08-06</remarks>
		private int GetNextId()
		{
			if (Count == 0)
				return m_currentId;
			else
				return ++m_currentId;
		}

		/// <summary>
		/// Gets the current id.
		/// </summary>
		/// <returns>The current id value.</returns>
		/// <remarks>Documented by Dev03, 2009-08-13</remarks>
		private int GetCurrentId()
		{
			int currentId = 0;
			XPathNodeIterator cards = m_navigator.Clone().Select(m_expression);
			if (cards.MoveNext())
			{
				if (cards.Current.MoveToAttribute(m_AttributeId, String.Empty))
				{
					if (Int32.TryParse(cards.Current.Value, out currentId))
					{
						return currentId;
					}
				}
				throw new IdOverflowException();
			}
			else
			{
				return currentId;
			}
		}

		/// <summary>
		/// Updates the current id.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-08-13</remarks>
		private void UpdateCurrentId()
		{
			m_currentId = GetCurrentId();
		}

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get { return parent; } }

		#endregion

		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
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
			return true;
		}

		/// <summary>
		/// Gets the permissions for the object.
		/// </summary>
		/// <returns>A list of permissions for the object.</returns>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		/// <remarks>Documented by Dev03, 2009-01-15</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions()
		{
			return new List<SecurityFramework.PermissionInfo>();
		}

		#endregion
	}
}
