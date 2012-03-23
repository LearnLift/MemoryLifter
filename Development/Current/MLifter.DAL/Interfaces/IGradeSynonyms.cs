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
    /// Inteface which define synonym gradion
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public interface IGradeSynonyms : ICopy, IParent
    {
        /// <summary>
        /// Gets or sets all known.
        /// </summary>
        /// <value>All known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? AllKnown { get; set; }

        /// <summary>
        /// Gets or sets the half known.
        /// </summary>
        /// <value>The half known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? HalfKnown { get; set; }

        /// <summary>
        /// Gets or sets the one known.
        /// </summary>
        /// <value>The one known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? OneKnown { get; set; }

        /// <summary>
        /// Gets or sets the first known.
        /// </summary>
        /// <value>The first known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? FirstKnown { get; set; }

        /// <summary>
        /// Gets or sets the prompt.
        /// </summary>
        /// <value>The prompt.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? Prompt { get; set; }
    }

    /// <summary>
    /// Helper class to match objects of the type IGradeSynonyms.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public static class GradeSynonymsHelper
    {
        /// <summary>
        /// Compares the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public static bool Compare(object a, object b)
        {
            if (!typeof(IGradeSynonyms).IsAssignableFrom(a.GetType()) || !typeof(IGradeSynonyms).IsAssignableFrom(b.GetType()))
                return false;

            bool isMatch = true;
            isMatch &= ((a as IGradeSynonyms).AllKnown == (b as IGradeSynonyms).AllKnown);
            isMatch &= ((a as IGradeSynonyms).FirstKnown == (b as IGradeSynonyms).FirstKnown);
            isMatch &= ((a as IGradeSynonyms).HalfKnown == (b as IGradeSynonyms).HalfKnown);
            isMatch &= ((a as IGradeSynonyms).OneKnown == (b as IGradeSynonyms).OneKnown);
            isMatch &= ((a as IGradeSynonyms).Prompt == (b as IGradeSynonyms).Prompt);

            return isMatch;
        }
    }
}
