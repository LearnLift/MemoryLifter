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
    /// 
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-08-05</remarks>
    interface IDbChaptersConnector
    {
        /// <summary>
        /// Gets the chapter ids.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        IList<int> GetChapterIds(int lmid);

        /// <summary>
        /// Adds the new chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        int AddNewChapter(int lmid);

        /// <summary>
        /// Deletes the chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void DeleteChapter(int lmid, int id);

        /// <summary>
        /// Finds the chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        int FindChapter(int lmid, string title);

        /// <summary>
        /// Moves the chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <param name="first_id">The id of the first chapter.</param>
        /// <param name="second_id">The id of the second chapter..</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void MoveChapter(int lmid, int first_id, int second_id);

        /// <summary>
        /// Gets the chapter count in the specified learning module.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        int GetChapterCount(int lmid);

        /// <summary>
        /// Checks the id of the learning module for correctness.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void CheckLMId(int lmid);
    }
}
