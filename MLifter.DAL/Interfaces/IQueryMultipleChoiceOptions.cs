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
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// This descripbes the multiple choice query options.
	/// </summary>
	public interface IQueryMultipleChoiceOptions : ICopy, IParent
	{
		/// <summary>
		/// Gets or sets if allow random distractors.
		/// </summary>
		/// <value>
		/// Allow random distractors.
		/// </value>
		[ValueCopy]
		bool? AllowRandomDistractors { get; set; }

		/// <summary>
		/// Gets or sets if allow multiple correct answers.
		/// </summary>
		/// <value>
		/// Allow multiple correct answers.
		/// </value>
		[ValueCopy]
		bool? AllowMultipleCorrectAnswers { get; set; }

		/// <summary>
		/// Gets or sets the number of choices.
		/// </summary>
		/// <value>
		/// The number of choices.
		/// </value>
		[ValueCopy]
		int? NumberOfChoices { get; set; }

		/// <summary>
		/// Gets or sets the max number of correct answers.
		/// </summary>
		/// <value>
		/// The max number of correct answers.
		/// </value>
		[ValueCopy]
		int? MaxNumberOfCorrectAnswers { get; set; }
	}

	/// <summary>
	/// Helper methods for the !QueryMultipleChoiceOptions.
	/// </summary>
	public static class QueryMultipleChoiceOptionsHelper
	{
		/// <summary>
		/// Compares the specified objects.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns></returns>
		public static bool Compare(object a, object b)
		{
			if (!typeof(IQueryMultipleChoiceOptions).IsAssignableFrom(a.GetType()) || !typeof(IQueryMultipleChoiceOptions).IsAssignableFrom(b.GetType()))
				return false;

			bool isMatch = true;
			isMatch &= ((a as IQueryMultipleChoiceOptions).AllowMultipleCorrectAnswers == (b as IQueryMultipleChoiceOptions).AllowMultipleCorrectAnswers);
			isMatch &= ((a as IQueryMultipleChoiceOptions).AllowRandomDistractors == (b as IQueryMultipleChoiceOptions).AllowRandomDistractors);
			isMatch &= ((a as IQueryMultipleChoiceOptions).MaxNumberOfCorrectAnswers == (b as IQueryMultipleChoiceOptions).MaxNumberOfCorrectAnswers);
			isMatch &= ((a as IQueryMultipleChoiceOptions).NumberOfChoices == (b as IQueryMultipleChoiceOptions).NumberOfChoices);

			return isMatch;
		}
	}
}
