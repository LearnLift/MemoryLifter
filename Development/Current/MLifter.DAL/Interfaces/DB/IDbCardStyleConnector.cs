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
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using Npgsql;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbCardStyleConnector
    {
        /// <summary>
        /// Checks the id.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        void CheckId(int Id);

        /// <summary>
        /// Gets the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <returns>The CSS for the card style.</returns>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        string GetCardStyle(int Id);

        /// <summary>
        /// Sets the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <param name="CardStyle">The card style.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        void SetCardStyle(int Id, string CardStyle);

        /// <summary>
        /// Creates the new card style.
        /// </summary>
        /// <returns>The style id.</returns>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        int CreateNewCardStyle();

        /// <summary>
        /// Adds the media for the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <param name="mediaId">The media id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        int AddMediaForCardStyle(int Id, int mediaId);

        /// <summary>
        /// Gets the media for a card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev09, 2009-03-09</remarks>
        List<int> GetMediaForCardStyle(int Id);

        /// <summary>
        /// Deletes the media id for the card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <param name="mediaId">The media id.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        void DeleteMediaForCardStyle(int Id, int mediaId);

        /// <summary>
        /// Clears the media for card style.
        /// </summary>
        /// <param name="Id">The style id.</param>
        /// <remarks>Documented by Dev03, 2009-03-05</remarks>
        void ClearMediaForCardStyle(int Id);
    }
}
