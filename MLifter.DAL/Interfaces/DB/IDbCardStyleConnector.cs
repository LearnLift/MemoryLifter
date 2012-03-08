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
