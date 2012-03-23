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

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// Database interface for DbCard.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-25</remarks>
    interface IDbCardConnector
    {
        /// <summary>
        /// Checks if the card exists and throws an IdAccessException if not.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        void CheckCardId(int id);
        /// <summary>
        /// Gets the chapter for a card.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        int GetChapter(int id);
        /// <summary>
        /// Sets the chapter for a card.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <param name="chapter">The chapter id.</param>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        void SetChapter(int id, int chapter);

        ISettings GetSettings(int id);
        void SetSettings(int id, ISettings Settings);

        int GetBox(int id);
        void SetBox(int id, int Box);

        bool GetActive(int id);
        void SetActive(int id, bool Active);

        DateTime GetTimestamp(int id);
        void SetTimestamp(int id, DateTime Timestamp);
    }
}
