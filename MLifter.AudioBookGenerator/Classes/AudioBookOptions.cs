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
using System.IO;

namespace MLifterAudioBookGenerator
{
    /// <summary>
    /// Represents the necessary options to create an audio book.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-03-12</remarks>
    public class AudioBookOptions
    {
        private List<MediaField> mediaFields = new List<MediaField>();

        /// <summary>
        /// The media playback sequence for each card.
        /// </summary>
        /// <value>The media fields.</value>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public List<MediaField> MediaFields
        {
            get { return mediaFields; }
        }

        private bool stereo = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AudioBookOptions"/> is stereo.
        /// </summary>
        /// <value><c>true</c> if stereo; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-03-31</remarks>
        public bool Stereo
        {
            get { return stereo; }
            set { stereo = value; }
        }
    }

    /// <summary>
    /// Represents a media field (e.g. question, answer, silence...)
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-03-29</remarks>
    public class MediaField
    {
        public enum SideEnum { Question, Answer };
        public enum TypeEnum { AudioField, Silence };

        private SideEnum side;
        private bool example;
        private TypeEnum type;
        private double silenceduration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaField"/> class.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public MediaField(SideEnum side, bool example)
        {
            this.type = TypeEnum.AudioField;
            this.side = side;
            this.example = example;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaField"/> class.
        /// </summary>
        /// <param name="silenceduration">The silenceduration.</param>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public MediaField(double duration)
        {
            this.type = TypeEnum.Silence;
            this.silenceduration = duration;
        }

        /// <summary>
        /// Gets the side.
        /// </summary>
        /// <value>The side.</value>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public SideEnum Side
        {
            get { return side; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MediaField"/> is example.
        /// </summary>
        /// <value><c>true</c> if example; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public bool Example
        {
            get { return example; }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public TypeEnum Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the silenceduration.
        /// </summary>
        /// <value>The silenceduration.</value>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public double SilenceDuration
        {
            get { return silenceduration; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-03-29</remarks>
        public override string ToString()
        {
            switch (type)
            {
                case TypeEnum.AudioField:
                    return (example ? "Ex. " : string.Empty) + side.ToString();
                case TypeEnum.Silence:
                    return string.Format("Silence ({0})", silenceduration > 0 ? string.Format("{0:0.0} s", silenceduration) : "custom");
                default:
                    return "Undefined";
            }
        }
    }

    /// <summary>
    /// Represents a media object.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-03-30</remarks>
    public class MediaFieldFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFieldFile"/> class.
        /// </summary>
        /// <param name="mediafield">The mediafield.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public MediaFieldFile(MediaField mediafield)
        {
            this.mediafield = mediafield;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFieldFile"/> class.
        /// </summary>
        /// <param name="mediafield">The mediafield.</param>
        /// <param name="file">The file.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public MediaFieldFile(MediaField mediafield, FileInfo file)
        {
            this.mediafield = mediafield;
            this.file = file;
        }

        /// <summary>
        /// The media file.
        /// </summary>
        public FileInfo file = null;

        /// <summary>
        /// The corresponding media field.
        /// </summary>
        public MediaField mediafield;

        /// <summary>
        /// Gets a value indicating whether [contains a media file].
        /// </summary>
        /// <value><c>true</c> if [contains file]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public bool ContainsFile
        {
            get { return file != null; }
        }

        /// <summary>
        /// Gets the media file extension.
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public string Extension
        {
            get
            {
                if (this.file == null)
                    return string.Empty;

                return this.file.Extension.ToLowerInvariant();
            }
        }

    }
}

