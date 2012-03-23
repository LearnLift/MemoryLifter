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
	interface IDbDictionariesConnector
	{
		/// <summary>
		/// Gets the db version.
		/// </summary>
		/// <returns></returns>
		string GetDbVersion();

		/// <summary>
		/// Gets the LM ids.
		/// </summary>
		/// <returns></returns>
		IList<int> GetLMIds();

		/// <summary>
		/// Deletes the LM.
		/// </summary>
		/// <param name="id">The id.</param>
		void DeleteLM(int id);

		/// <summary>
		/// Adds a new LM.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="categoryId">The category id.</param>
		/// <param name="title">The title.</param>
		/// <param name="licenceKey">The licence key.</param>
		/// <param name="contentProtected">if set to <c>true</c> [content protected].</param>
		/// <param name="calCount">The cal count.</param>
		/// <returns></returns>
		int AddNewLM(string guid, int categoryId, string title, string licenceKey, bool contentProtected, int calCount);

		/// <summary>
		/// Gets the LM count.
		/// </summary>
		/// <returns></returns>
		int GetLMCount();

		/// <summary>
		/// Gets all extension GUIDs.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Documented by FabThe, 2.7.2009
		/// </remarks>
		IList<Guid> GetExtensions();
	}
}
