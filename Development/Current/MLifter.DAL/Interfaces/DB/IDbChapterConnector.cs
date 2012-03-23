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
    interface IDbChapterConnector
    {
        /// <summary>
        /// Gets the title of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        string GetTitle(int id);

        /// <summary>
        /// Sets the title of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void SetTitle(int id, string title);

        /// <summary>
        /// Gets the description of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        string GetDescription(int id);

        /// <summary>
        /// Sets the description of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="description">The description.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void SetDescription(int id, string description);

        /// <summary>
        /// Gets the size of the chapter (amount of cards).
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        int GetSize(int id);

        /// <summary>
        /// Gets the active size of the chapter (amount of activated cards).
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-11</remarks>
        int GetActiveSize(int id);

        /// <summary>
        /// Gets the associated lm id of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        int GetLmId(int id);

        /// <summary>
        /// Checks the chapter id of the chapter.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void CheckChapterId(int id);

        ISettings GetSettings(int id);

        ICardStyle CreateId();
        void SetSettings(int id, ISettings Settings);
    }
}
