using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Interface which defines dictionaries.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    public interface IDictionaries : IParent, ISecurity
    {
        /// <summary>
        /// Gets all learning modules.
        /// </summary>
        /// <value>The dictionaries.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        IList<IDictionary> Dictionaries { get; }

        /// <summary>
        /// Gets the specified learning module.
        /// </summary>
        /// <param name="id">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        IDictionary Get(int id);

        /// <summary>
        /// Deletes a specific LM.
        /// </summary>
        /// <param name="css">The connection string struct.</param>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        void Delete(ConnectionStringStruct css);

        /// <summary>
        /// Adds a new learning module.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="title">The title.</param>
        /// <returns>The new learning module.</returns>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        IDictionary AddNew(int categoryId, string title);

		/// <summary>
		/// Adds a new learning module.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="title">The title.</param>
		/// <param name="licenceKey">The licence key.</param>
		/// <param name="contentProtected">if set to <c>true</c> the content is protected.</param>
		/// <param name="calCount">The cal count.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2009-02-12
		/// </remarks>
        IDictionary AddNew(int categoryId, string title, string licenceKey, bool contentProtected, int calCount);

        /// <summary>
        /// Gets the count of learning modules.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        int Count { get; }

        /// <summary>
        /// Gets all extensions (independent of the LearningModule id).
        /// </summary>
        /// <value>The extensions.</value>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        IList<IExtension> Extensions { get; }

        /// <summary>
        /// Creates new extensions.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        IExtension ExtensionFactory();

        /// <summary>
        /// Deletes the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <remarks>Documented by Dev02, 2009-07-10</remarks>
        void DeleteExtension(IExtension extension);
    }
}
