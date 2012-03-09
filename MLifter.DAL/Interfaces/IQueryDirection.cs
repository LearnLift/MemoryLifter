using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// Interface that defines the available query directions for a dictionary.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-01-08</remarks>
	public interface IQueryDirections : ICopy, IParent
	{
		/// <summary>
		/// Gets or sets a value indicating whether [question to answer] is allowed.
		/// </summary>
		/// <value><c>true</c> if [question to answer]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		[ValueCopy]
		bool? Question2Answer { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [answer to question] is allowed.
		/// </summary>
		/// <value><c>true</c> if [answer to question]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		[ValueCopy]
		bool? Answer2Question { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this is in mixed mode.
		/// </summary>
		/// <value><c>true</c> if mixed; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev03, 2008-01-08</remarks>
		[ValueCopy]
		bool? Mixed { get; set; }
	}

	/// <summary>
	/// Helper for the IQueryDirections interface
	/// </summary>
	public static class QueryDirectionsHelper
	{
		/// <summary>
		/// Compares the specified a.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">The b.</param>
		/// <returns></returns>
		public static bool Compare(object a, object b)
		{
			if (!typeof(IQueryDirections).IsAssignableFrom(a.GetType()) || !typeof(IQueryDirections).IsAssignableFrom(b.GetType()))
				return false;

			bool isMatch = true;
			isMatch &= ((a as IQueryDirections).Answer2Question == (b as IQueryDirections).Answer2Question);
			isMatch &= ((a as IQueryDirections).Mixed == (b as IQueryDirections).Mixed);
			isMatch &= ((a as IQueryDirections).Question2Answer == (b as IQueryDirections).Question2Answer);

			return isMatch;
		}
	}

	/// <summary>
	/// Enumerate the possible query directions. Available values are:
	/// Question2Answer - from question to answer,
	/// Answer2Question - from answer to question,
	/// Mixed - both directions
	/// </summary>
	/// <remarks>Documented by Dev03, 2007-07-26</remarks>
	public enum EQueryDirection
	{
		/// <summary>
		/// From question to answer.
		/// </summary>
		Question2Answer = 0,
		/// <summary>
		/// From answer to question.
		/// </summary>
		Answer2Question = 1,
		/// <summary>
		/// Both directions.
		/// </summary>
		Mixed = 2
	}
}
