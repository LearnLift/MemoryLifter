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
	interface IDbCardMediaConnector
	{
		/// <summary>
		/// Gets the media.
		/// </summary>
		/// <param name="id">The media id.</param>
		/// <param name="cardid">The cardid.</param>
		/// <returns>
		/// A media object.
		/// </returns>
		/// <remarks>
		/// Documented by AAB, 5.8.2008.
		/// </remarks>
		IMedia GetMedia(int id, int cardid);
		/// <summary>
		/// Sets the card media.
		/// </summary>
		/// <param name="id">The media id.</param>
		/// <param name="cardid">The cardid.</param>
		/// <param name="side">The side.</param>
		/// <param name="type">The type.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="mediatype">The mediatype.</param>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		void SetCardMedia(int id, int cardid, Side side, WordType type, bool isDefault, EMedia mediatype);
		/// <summary>
		/// Gets the card media.
		/// </summary>
		/// <param name="cardid">The card id.</param>
		/// <param name="side">The side.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		IList<int> GetCardMedia(int cardid, Side side);
		/// <summary>
		/// Gets the card media.
		/// </summary>
		/// <param name="cardid">The card id.</param>
		/// <param name="side">The side.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		IList<int> GetCardMedia(int cardid, Side side, WordType type);
		/// <summary>
		/// Clears all card media.
		/// </summary>
		/// <param name="cardid">The cardid.</param>
		/// <remarks>Documented by Dev02, 2008-08-08</remarks>
		void ClearCardMedia(int cardid);
		/// <summary>
		/// Clears the card media.
		/// </summary>
		/// <param name="id">The media id.</param>
		/// <param name="cardid">The card id.</param>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		void ClearCardMedia(int cardid, int id);
		/// <summary>
		/// Clears the card media.
		/// </summary>
		/// <param name="cardid">The cardid.</param>
		/// <param name="side">The side.</param>
		/// <param name="type">The type.</param>
		/// <param name="mediatype">The mediatype.</param>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		void ClearCardMedia(int cardid, Side side, WordType type, EMedia mediatype);
		/// <summary>
		/// Checks the card media id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="cardid">The card id.</param>
		/// <remarks>Documented by Dev02, 2008-08-06</remarks>
		void CheckCardMediaId(int id, int cardid);
		/// <summary>
		/// Gets the list of media resources.
		/// </summary>
		/// <param name="lmid">The learning module id.</param>
		/// <returns>List of media resource ids.</returns>
		/// <remarks>Documented by Dev03, 2008-08-08</remarks>
		IList<int> GetMediaResources(int lmid);
	}
}
