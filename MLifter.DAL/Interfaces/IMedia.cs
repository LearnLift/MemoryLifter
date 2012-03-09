using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Media file interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IMedia : IParent, ISecurity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2008-08-05</remarks>
        int Id { get; }
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        string Filename { get; set; }
        /// <summary>
        /// Gets the stream containing the media file.
        /// </summary>
        /// <value>The stream.</value>
        /// <remarks>Documented by Dev05, 2008-08-06</remarks>
        Stream Stream { get; }
        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <value>The type of the media.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        EMedia MediaType { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IMedia"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        bool? Active { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IAudio"/> is an example audio file.
        /// </summary>
        /// <value><c>true</c> if example; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        bool? Example { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IAudio"/> is the default audio file.
        /// </summary>
        /// <value><c>true</c> if default; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        bool? Default { get; }
        /// <summary>
        /// Gets the mime type for the file.
        /// </summary>
        /// <value>The mime type.</value>
        /// <remarks>Documented by Dev03, 2008-08-07</remarks>
        string MimeType { get; }
        /// <summary>
        /// Gets the extension for the file.
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks>Documented by Dev03, 2008-08-13</remarks>
        string Extension { get; }

        /// <summary>
        /// Gets the size of the media.
        /// </summary>
        /// <value>The size of the media.</value>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        int MediaSize { get; }
    }

    /// <summary>
    /// Video file interface. Derived from IMedia.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IVideo : IMedia
    {
    }

    /// <summary>
    /// Audio file interface. Derived from IMedia.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IAudio : IMedia
    {
    }

    /// <summary>
    /// Image file interface. Derived from IMedia.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IImage : IMedia
    {
        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>The width.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Width { get; set; }
        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        int Height { get; set; }
    }

    /// <summary>
    /// Enumerator which defines the media file types.
    /// </summary>
    public enum EMedia
    {
        /// <summary>
        /// Audio file type.
        /// </summary>
        Audio = 0,
        /// <summary>
        /// Video file type.
        /// </summary>
        Video = 1,
        /// <summary>
        /// Image file type.
        /// </summary>
        Image = 2,
        /// <summary>
        /// Unknown file type.
        /// </summary>
        Unknown = 3
    }

    /// <summary>
    /// Enumerator which defines a property value that can be attached to a media.
    /// </summary>
    public enum MediaProperty
    {
        /// <summary>
        /// The extension of the media file (e.g. .jpg).
        /// </summary>
        Extension,
        /// <summary>
        /// The mimetype of the media file (e.g. image/jpeg).
        /// </summary>
        MimeType,
        /// <summary>
        /// The width of the media object.
        /// </summary>
        Width,
        /// <summary>
        /// The height of the media object.
        /// </summary>
        Height,
        /// <summary>
        /// The size of the media object.
        /// </summary>
        MediaSize
    }
}
