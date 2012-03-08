using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// Interface for DB media connections
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-08-05</remarks>
    interface IDbMediaConnector
    {
        /// <summary>
        /// Gets the media resources of the learning module.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        List<int> GetMediaResources(int Id);
        /// <summary>
        /// Gets the empty media resources of the learning module.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-31</remarks>
        List<int> GetEmptyMediaResources(int Id);

        /// <summary>
        /// Creates a new media object.
        /// </summary>
        /// <param name="media">The memory stream containing the media.</param>
        /// <param name="type">The media type.</param>
        /// <param name="rpu">A delegate of type <see cref="StatusMessageReportProgress"/> used to send messages back to the calling object.</param>
        /// <param name="caller">The calling object.</param>
        /// <returns>The id for the new media object.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        int CreateMedia(Stream media, EMedia type, StatusMessageReportProgress rpu, object caller);
        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-06-25</remarks>
        EMedia GetMediaType(int id);
        /// <summary>
        /// Updates the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="media">The media.</param>
        /// <remarks>Documented by Dev02, 2008-08-06</remarks>
        void UpdateMedia(int id, Stream media);
        /// <summary>
        /// Deletes the media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        void DeleteMedia(int id);
        /// <summary>
        /// Determines whether this media is available.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// 	<c>true</c> if media is available; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        bool IsMediaAvailable(int id);
		/// <summary>
		/// Gets the media.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by AAB, 5.8.2008.
		/// </remarks>
        Stream GetMediaStream(int id);
        /// <summary>
        /// Gets the media.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cacheConnector">The cache connector.</param>
        /// <returns>A memory stream for the media object.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        Stream GetMediaStream(int id, IDbMediaConnector cacheConnector);
        /// <summary>
        /// Gets the properties for a media object.
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <returns>A list of properties.</returns>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        Dictionary<MediaProperty, string> GetProperties(int id);
        /// <summary>
        /// Sets the properties for a media object.
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <param name="properties">The properties for the media object.</param>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        void SetProperties(int id, Dictionary<MediaProperty, string> properties);
        /// <summary>
        /// Gets a single property value for a media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-07</remarks>
        string GetPropertyValue(int id, MediaProperty property);
        /// <summary>
        /// Sets a single property value for a media object.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Documented by Dev02, 2008-08-07</remarks>
        void SetPropertyValue(int id, MediaProperty property, string value);
        /// <summary>
        /// Checks the media id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        void CheckMediaId(int id);
    }
}
