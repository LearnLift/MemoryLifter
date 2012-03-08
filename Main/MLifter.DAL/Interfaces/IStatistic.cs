using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Statistic interface. EVERY Statistic has only ONE SESSION!!!
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IStatistic
    {
        /// <summary>
        /// Gets the id of the Session
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Id { get; }
        /// <summary>
        /// Gets or sets the start timestamp of the session.
        /// </summary>
        /// <value>The start timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        DateTime StartTimestamp { get; set; }
        /// <summary>
        /// Gets or sets the end timestamp of the session.
        /// </summary>
        /// <value>The end timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        DateTime EndTimestamp { get; set; }
        /// <summary>
        /// Gets or sets the correct answers of the session.
        /// </summary>
        /// <value>The right.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Right { get; set; }
        /// <summary>
        /// Gets or sets the wrong answers of the session.
        /// </summary>
        /// <value>The wrong.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Wrong { get; set; }
        /// <summary>
        /// Gets or sets the boxes of the session.
        /// </summary>
        /// <value>The boxes.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        IList<int> Boxes { get; }
    }
}
