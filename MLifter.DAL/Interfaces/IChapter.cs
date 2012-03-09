using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Inteface definition for a chapter.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-04</remarks>
    public interface IChapter : ICopy, IParent, ISecurity
    {
        /// <summary>
        /// Gets or sets the id for a chapter.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        int Id { get; }
        /// <summary>
        /// Gets an Xml representation of the chapter.
        /// </summary>
        /// <value>The chapter.</value>
        /// <remarks>Documented by Dev03, 2008-08-14</remarks>
        XmlElement Chapter { get;}
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        string Title { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks>Documented by Dev03, 2007-09-04</remarks>
        [ValueCopy]
        string Description { get; set; }
        /// <summary>
        /// Gets the total number of cards.
        /// </summary>
        /// <value>The total number of cards.</value>
        /// <remarks>Documented by Dev03, 2007-11-22</remarks>
        int Size { get; }
        /// <summary>
        /// Gets the number of active cards.
        /// </summary>
        /// <value>The number of active cards.</value>
        /// <remarks>Documented by Dev03, 2007-11-22</remarks>
        int ActiveSize { get; }
        /// <summary>
        /// Creates and returns a card style.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        ICardStyle CreateCardStyle();

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        ISettings Settings { get; set; }
    }
}
