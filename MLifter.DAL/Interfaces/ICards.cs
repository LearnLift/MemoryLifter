using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// Interface which defines cards.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-15</remarks>
	public interface ICards : IParent, ICopy, ISecurity
	{
		/// <summary>
		/// Gets the cards.
		/// </summary>
		/// <value>The cards.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		IList<ICard> Cards { get; }
		/// <summary>
		/// Gets the number of cards.
		/// </summary>
		/// <value>The count.</value>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		int Count { get; }

		/// <summary>
		/// Creates a new card object
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-01-04</remarks>
		ICard Create();
		/// <summary>
		/// Adds the specified card to the dictionary. An exception is thrown if an ID conflict occurs.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <remarks>Documented by Dev03, 2007-09-21</remarks>
		void Add(ICard card);
		/// <summary>
		/// Creates a new card and appends it to the dictionary.
		/// </summary>
		/// <returns>The new card.</returns>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		ICard AddNew();
		/// <summary>
		/// Copies the given card from one dictionary to this dictionary and returns the new ID.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <remarks>Documented by Dev03, 2007-09-21</remarks>
		int CopyCard(ICard card);
		/// <summary>
		/// Loads a card from Xml and returns it. The card is not assigned.
		/// </summary>
		/// <param name="xmlCard">The XML card.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-21</remarks>
		ICard LoadCardFromXml(string xmlCard);
		/// <summary>
		/// Deletes the specified card.
		/// </summary>
		/// <param name="card_id">The card id.</param>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		void Delete(int card_id);
		/// <summary>
		/// Gets the specified card.
		/// </summary>
		/// <param name="card_id">The card id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-04</remarks>
		ICard Get(int card_id);
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
		List<ICard> GetCards(QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number);
		/// <summary>
		/// Resets the boxes of all cards.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-08</remarks>
		void ClearAllBoxes();
	}

	/// <summary>
	/// Helper methods for ICards.
	/// </summary>
	public static class CardsHelper
	{
		/// <summary>
		/// Gets or sets the card ids not to copy.
		/// </summary>
		/// <value>
		/// The card ids not to copy.
		/// </value>
		public static List<int> CardIdsNotToCopy { get; set; }

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public static void Copy(ICards source, ICards target, CopyToProgress progressDelegate)
		{
			source.Parent.GetParentDictionary().Chapters.CopyTo(target.Parent.GetParentDictionary().Chapters, progressDelegate);

			CopyBase.Copy(source, target, typeof(ICards), progressDelegate);
			int counter = 0;
			int count = source.Cards.Count - (CardIdsNotToCopy != null ? CardIdsNotToCopy.Count : 0);
			Dictionary<int, int> chapterMappings = target.Parent.GetParentDictionary().Parent.Properties[ParentProperty.ChapterMappings] as Dictionary<int, int>;
			foreach (ICard card in source.Cards)
			{
				if (CardIdsNotToCopy != null && CardIdsNotToCopy.Contains(card.Id))
					continue;

				++counter;
				if (progressDelegate != null && counter % 5 == 0)
					progressDelegate.Invoke(String.Format(Properties.Resources.CARDS_COPYTO_STATUS, counter, count), counter * 1.0 / count * 100);

				ICard newCard = target.AddNew();
				card.CopyTo(newCard, progressDelegate);
				newCard.Chapter = chapterMappings[card.Chapter];
			}
		}
	}

	/// <summary>
	/// Structure defines a card query - negative values for fields will tell the query interface to ignore the value.
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-29</remarks>
	public struct QueryStruct
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStruct"/> struct.
		/// </summary>
		/// <param name="cardState">State of the card for which the query should search.</param>
		/// <remarks>Documented by Dev03, 2007-09-05</remarks>
		public QueryStruct(QueryCardState cardState)
		{
			ChapterId = -1;
			BoxId = -1;
			CardState = cardState;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStruct"/> struct.
		/// </summary>
		/// <param name="chapterId">The chapter id (values &lt;=0 are ignored).</param>
		/// <param name="boxId">The box id (values &lt;=0 are ignored).</param>
		/// <remarks>Documented by Dev03, 2007-09-05</remarks>
		public QueryStruct(int chapterId, int boxId)
		{
			ChapterId = chapterId;
			BoxId = boxId;
			CardState = QueryCardState.All;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryStruct"/> struct.
		/// </summary>
		/// <param name="chapterId">The chapter id for which the query should search (values &lt;=0 are ignored).</param>
		/// <param name="boxId">The box id for which the query should search (values &lt;=0 are ignored).</param>
		/// <param name="cardState">State of the card for which the query should search.</param>
		/// <remarks>Documented by Dev03, 2007-09-05</remarks>
		public QueryStruct(int chapterId, int boxId, QueryCardState cardState)
		{
			ChapterId = chapterId;
			BoxId = boxId;
			CardState = cardState;
		}

		/// <summary>
		/// The chapter id for which the query should search (values &lt;=0 are ignored).
		/// </summary>
		public int ChapterId;
		/// <summary>
		/// The box id for which the query should search (values &lt;=0 are ignored).
		/// </summary>
		public int BoxId;
		/// <summary>
		/// State of the card for which the query should search.
		/// </summary>
		public QueryCardState CardState;
	}

	/// <summary>
	/// Enumerator defines the available sort order types
	/// </summary>
	public enum QueryOrder
	{
		/// <summary>
		/// Order by timestamp
		/// </summary>
		Timestamp,
		/// <summary>
		/// Order by chapter
		/// </summary>
		Chapter,
		/// <summary>
		/// Order by box
		/// </summary>
		Box,
		/// <summary>
		/// Order by question
		/// </summary>
		Question,
		/// <summary>
		/// Order by answer
		/// </summary>
		Answer,
		/// <summary>
		/// Order by Id
		/// </summary>
		Id,
		/// <summary>
		/// Order by Random
		/// </summary>
		Random,
		/// <summary>
		/// Do not order (takes the order from the data source)
		/// </summary>
		None
	}

	/// <summary>
	/// Enumerator which defines the sort order direction.
	/// </summary>
	public enum QueryOrderDir
	{
		/// <summary>
		/// Ascending sort order
		/// </summary>
		Ascending,
		/// <summary>
		/// Descending sort order
		/// </summary>
		Descending
	}

	/// <summary>
	/// Enumerator defines the card states available for a query.
	/// </summary>
	public enum QueryCardState
	{
		/// <summary>
		/// Returns only active cards.
		/// </summary>
		Active,
		/// <summary>
		/// Returns only inactive cards.
		/// </summary>
		Inactive,
		/// <summary>
		/// Returns all (active and inactive) cards.
		/// </summary>
		All
	}
}