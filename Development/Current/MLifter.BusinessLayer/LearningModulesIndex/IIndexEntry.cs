using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using System.Drawing;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Interface to a learning module (path).
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-06</remarks>
    public interface IIndexEntry
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is verified (=not from cache).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        bool IsVerified { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is from cache.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is from cache; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        bool IsFromCache { get; set; }
        /// <summary>
        /// Gets or sets the card or learning modules count.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        int Count { get; set; }
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        Image Logo { get; set; }
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        IConnectionString Connection { get; set; }
    }
}
