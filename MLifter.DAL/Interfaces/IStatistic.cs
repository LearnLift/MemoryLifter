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

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Statistic interface. EVERY Statistic has only ONE SESSION!!!
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IStatistic
    {
        /// <summary>
        /// Gets the id of the Session
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Id { get; }
        /// <summary>
        /// Gets or sets the start timestamp of the session.
        /// </summary>
        /// <value>The start timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        DateTime StartTimestamp { get; set; }
        /// <summary>
        /// Gets or sets the end timestamp of the session.
        /// </summary>
        /// <value>The end timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        DateTime EndTimestamp { get; set; }
        /// <summary>
        /// Gets or sets the correct answers of the session.
        /// </summary>
        /// <value>The right.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Right { get; set; }
        /// <summary>
        /// Gets or sets the wrong answers of the session.
        /// </summary>
        /// <value>The wrong.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Wrong { get; set; }
        /// <summary>
        /// Gets or sets the boxes of the session.
        /// </summary>
        /// <value>The boxes.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        IList<int> Boxes { get; }
    }
}
