using System;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// This interface defines the possible query types of a card.
	/// </summary>
	public interface IQueryType : ICopy, IParent
	{
		/// <summary>
		/// Gets or sets if the image recognition query mode is possible.
		/// </summary>
		/// <value>
		/// The image recognition query mode is possible.
		/// </value>
		[ValueCopy]
		bool? ImageRecognition { get; set; }
		/// <summary>
		/// Gets or sets if the listening comprehension query mode is possible.
		/// </summary>
		/// <value>
		/// The listening comprehension query mode is possible.
		/// </value>
		[ValueCopy]
		bool? ListeningComprehension { get; set; }
		/// <summary>
		/// Gets or sets if the multiple choice query mode is possible.
		/// </summary>
		/// <value>
		/// The multiple choice query mode is possible.
		/// </value>
		[ValueCopy]
		bool? MultipleChoice { get; set; }
		/// <summary>
		/// Gets or sets if the sentence query mode is possible.
		/// </summary>
		/// <value>
		/// The sentence query mode is possible.
		/// </value>
		[ValueCopy]
		bool? Sentence { get; set; }
		/// <summary>
		/// Gets or sets if the word query mode is possible.
		/// </summary>
		/// <value>
		/// The word query mode is possible.
		/// </value>
		[ValueCopy]
		bool? Word { get; set; }
	}

	/// <summary>
	/// Helper methods for the IQueryType interface.
	/// </summary>
	public static class QueryTypeHelper
	{
		/// <summary>
		/// Compares the specified objects.
		/// </summary>
		/// <param name="a">The first object</param>
		/// <param name="b">The second object</param>
		/// <returns></returns>
		public static bool Compare(object a, object b)
		{
			if (!typeof(IQueryType).IsAssignableFrom(a.GetType()) || !typeof(IQueryType).IsAssignableFrom(b.GetType()))
				return false;

			bool isMatch = true;
			isMatch &= ((a as IQueryType).ImageRecognition == (b as IQueryType).ImageRecognition);
			isMatch &= ((a as IQueryType).ListeningComprehension == (b as IQueryType).ListeningComprehension);
			isMatch &= ((a as IQueryType).MultipleChoice == (b as IQueryType).MultipleChoice);
			isMatch &= ((a as IQueryType).Sentence == (b as IQueryType).Sentence);
			isMatch &= ((a as IQueryType).Word == (b as IQueryType).Word);

			return isMatch;
		}
	}

	/// <summary>
	/// Enumeration which defines the values for 'Learning modes' settings.
	/// Word - word mode (standard mode), 
	/// MultipleChoice - multiple choice, 
	/// Sentences - sentences, 
	/// ListeningComprehension - Listening comprehension,
	/// ImageRecognition - Image recognition,
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-08-02</remarks>
	public enum EQueryType
	{
		/// <summary>
		/// 
		/// </summary>
		Word = 1,
		/// <summary>
		/// 
		/// </summary>
		MultipleChoice = 2,
		/// <summary>
		/// 
		/// </summary>
		Sentences = 4,
		/// <summary>
		/// 
		/// </summary>
		ListeningComprehension = 8,
		/// <summary>
		/// 
		/// </summary>
		ImageRecognition = 16
	}
}
