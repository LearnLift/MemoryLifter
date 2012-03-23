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
