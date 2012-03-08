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
