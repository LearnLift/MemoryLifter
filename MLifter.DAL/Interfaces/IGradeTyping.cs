using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// This represents the grade typing options.
	/// </summary>
	public interface IGradeTyping : ICopy, IParent
	{
		/// <summary>
		/// Gets or sets all correct mode.
		/// </summary>
		/// <value>
		/// All correct.
		/// </value>
		[ValueCopy]
		bool? AllCorrect { get; set; }

		/// <summary>
		/// Gets or sets the half correct mode.
		/// </summary>
		/// <value>
		/// The half correct.
		/// </value>
		[ValueCopy]
		bool? HalfCorrect { get; set; }

		/// <summary>
		/// Gets or sets the none correct mode.
		/// </summary>
		/// <value>
		/// The none correct.
		/// </value>
		[ValueCopy]
		bool? NoneCorrect { get; set; }

		/// <summary>
		/// Gets or sets the prompt mode.
		/// </summary>
		/// <value>
		/// The prompt.
		/// </value>
		[ValueCopy]
		bool? Prompt { get; set; }
	}

	/// <summary>
	/// Helper methods for the IGradeTyping interface.
	/// </summary>
	public static class GradeTypingHelper
	{
		/// <summary>
		/// Compares the specified objects.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns></returns>
		public static bool Compare(object a, object b)
		{
			if (!typeof(IGradeTyping).IsAssignableFrom(a.GetType()) || !typeof(IGradeTyping).IsAssignableFrom(b.GetType()))
				return false;

			bool isMatch = true;
			isMatch &= ((a as IGradeTyping).AllCorrect == (b as IGradeTyping).AllCorrect);
			isMatch &= ((a as IGradeTyping).HalfCorrect == (b as IGradeTyping).HalfCorrect);
			isMatch &= ((a as IGradeTyping).NoneCorrect == (b as IGradeTyping).NoneCorrect);
			isMatch &= ((a as IGradeTyping).Prompt == (b as IGradeTyping).Prompt);

			return isMatch;
		}
	}
}
