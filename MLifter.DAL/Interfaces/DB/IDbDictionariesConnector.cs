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
