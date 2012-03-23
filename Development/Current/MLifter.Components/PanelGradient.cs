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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace MLifter.Components
{
    [Serializable]
    public class PanelGradient
    {
        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        public string Serialize()
        {
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            XmlWriter xmlt = XmlWriter.Create(stream, settings);
            XmlSerializer ser = new XmlSerializer(typeof(PanelGradient));
            ser.Serialize(xmlt, this);
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            stream.Dispose();
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Gets or sets the colors.
        /// </summary>
        /// <value>The colors.</value>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        [XmlIgnore]
        public List<Color> Colors
        { get; set; }

        /// <summary>
        /// Gets or sets the serializeable colors.
        /// This is only for the XML serialization. Use Colors instead.
        /// </summary>
        /// <value>The serializeable colors.</value>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        [XmlArray("Colors"), XmlArrayItem("Color"), Browsable(false)]
        public string[] SerializeableColors
        {
            get
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
                List<string> colors = new List<string>();
                if (this.Colors != null && this.Colors.Count > 0)
                    this.Colors.ForEach(c => colors.Add(converter.ConvertToInvariantString(c)));
                return colors.ToArray();
            }
            set
            {
                this.Colors = new List<Color>();
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
                List<string> colors = new List<string>(value);
                colors.ForEach(c => this.Colors.Add((Color)converter.ConvertFromInvariantString(c)));
            }
        }

        /// <summary>
        /// Gets or sets the positions.
        /// </summary>
        /// <value>The positions.</value>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        [XmlArray("Positions"), XmlArrayItem("Position")]
        public List<float> Positions
        { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        public LinearGradientMode Direction
        { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid gradient.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is valid gradient; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        public bool IsValidGradient
        {
            get
            {
                return (Colors.Count > 0 && Colors.Count == Positions.Count);
            }
        }
    }
}
