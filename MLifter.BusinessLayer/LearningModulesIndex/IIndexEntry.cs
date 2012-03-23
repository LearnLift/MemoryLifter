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
using MLifter.DAL.Interfaces;
using System.Drawing;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Interface to a learning module (path).
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-06</remarks>
    public interface IIndexEntry
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is verified (=not from cache).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        bool IsVerified { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is from cache.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is from cache; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        bool IsFromCache { get; set; }
        /// <summary>
        /// Gets or sets the card or learning modules count.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        int Count { get; set; }
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        Image Logo { get; set; }
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        IConnectionString Connection { get; set; }
    }
}
