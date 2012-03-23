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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace MLifter.AudioTools.Codecs
{
    /// <summary>
    /// Provides a list of codecs.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-04-15</remarks>
    public class Codecs : List<Codec>, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Codecs"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public Codecs()
        {
            this.XMLString = Codecs.DefaultXML;
        }

        /// <summary>
        /// Gets or sets the serialized class as XML string.
        /// </summary>
        /// <value>The XML string.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public string XMLString
        {
            get
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<Codec>));
                MemoryStream stream = new MemoryStream();
                ser.Serialize(stream, this);
                byte[] xmlbyte = stream.GetBuffer();
                return System.Text.Encoding.Default.GetString(xmlbyte);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(List<Codec>));
                    MemoryStream stream = new MemoryStream();
                    byte[] xmlbyte = System.Text.Encoding.Default.GetBytes(value);
                    stream.Write(xmlbyte, 0, xmlbyte.Length);
                    stream.Position = 0;
                    List<Codec> codecs = (List<Codec>)ser.Deserialize(stream);
                    this.Clear();
                    this.AddRange(codecs);
                }
            }
        }

        /// <summary>
        /// Gets the codecs which support encoding, sorted by their file extension.
        /// </summary>
        /// <value>The encode codecs.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public Dictionary<string, Codec> encodeCodecs
        {
            get
            {
                Dictionary<string, Codec> codecs = new Dictionary<string, Codec>();
                foreach (Codec codec in this)
                    if (codec.CanEncode)
                        codecs.Add(codec.extension.ToLowerInvariant(), codec);
                return codecs;
            }
        }

        /// <summary>
        /// Gets the codecs which support decoding, sorted by their file extension.
        /// </summary>
        /// <value>The decode codecs.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public Dictionary<string, Codec> decodeCodecs
        {
            get
            {
                Dictionary<string, Codec> codecs = new Dictionary<string, Codec>();
                foreach (Codec codec in this)
                    if (codec.CanDecode)
                        codecs.Add(codec.extension.ToLowerInvariant(), codec);
                return codecs;
            }
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public object Clone()
        {
            Codecs clone = new Codecs();
            clone.Clear();
            foreach (Codec codec in this)
                clone.Add((Codec)codec.Clone());
            return clone;
        }

        #endregion

        /// <summary>
        /// Gets the default XML.
        /// </summary>
        /// <value>The default XML.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public static string DefaultXML
        {
            get { return MLifterAudioTools.Properties.Resources.DEFAULT_CODECS_XML; }
        }
    }

    /// <summary>
    /// This class describes the encoding / decoding facilities required for a defined file format.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-04-10</remarks>
    [Serializable]
    public class Codec : ICloneable
    {
        /// <summary>
        /// The file extension, with a leading dot.
        /// </summary>
        public string extension;
        /// <summary>
        /// The name of this format.
        /// </summary>
        public string name;

        /// <summary>
        /// The encoding application.
        /// </summary>
        public string EncodeApp;
        /// <summary>
        /// The arguments for the encoding application.
        /// </summary>
        public string EncodeArgs;

        /// <summary>
        /// The decoding application.
        /// </summary>
        public string DecodeApp;
        /// <summary>
        /// The arguments for the decoding application.
        /// </summary>
        public string DecodeArgs;

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        public override string ToString()
        {
            return string.Format("{0} (*{1})", this.name, this.extension);
        }

        /// <summary>
        /// Gets a value indicating whether this instance can encode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can encode; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        public bool CanEncode
        {
            get { return EncodeError == string.Empty; }
        }

        /// <summary>
        /// Sets a value indicating whether this instance can decode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can decode; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        public bool CanDecode
        {
            get { return DecodeError == string.Empty; }
        }

        /// <summary>
        /// Validates the supplied values.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// An error message when invalid or an empty string when valid.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        private string Validate(string application, string arguments)
        {
            string error = string.Empty;

            bool appvalid = CheckExecutablePath(application);
            bool argvalid = arguments.Contains("{0}") && arguments.Contains("{1}");
            if (!appvalid)
                error += "Application path not found. ";
            if (!argvalid)
                error += "Arguments: Both placeholders are required. ";
            return error;
        }

        /// <summary>
        /// Validates the encoding values.
        /// </summary>
        /// <returns>An error message when invalid or an empty string when valid.</returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public string EncodeError
        {
            get { return Validate(EncodeApp, EncodeArgs); }
        }

        /// <summary>
        /// Validates the decoding values.
        /// </summary>
        /// <returns>An error message when invalid or an empty string when valid.</returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public string DecodeError
        {
            get { return Validate(DecodeApp, DecodeArgs); }
        }

        /// <summary>
        /// Decodes the compressed file to a wavefile.
        /// </summary>
        /// <param name="sourcefile">The sourcefile.</param>
        /// <param name="tempfolder">The tempfolder.</param>
        /// <param name="showwindow">if set to <c>true</c> [showwindow].</param>
        /// <param name="minimizewindow">if set to <c>true</c> [minimizewindow].</param>
        /// <returns>The generated file.</returns>
        /// <remarks>Documented by Dev02, 2008-03-17</remarks>
        public FileInfo Decode(FileInfo sourcefile, DirectoryInfo tempfolder, bool showwindow, bool minimizewindow)
        {
            FileInfo destfile = new FileInfo(Path.ChangeExtension(Path.Combine(tempfolder.FullName, sourcefile.Name), MLifterAudioTools.Properties.Resources.AUDIO_WAVE_EXTENSION));
            return Decode(sourcefile, destfile, showwindow, minimizewindow);
        }

        /// <summary>
        /// Decodes the compressed file to a wavefile.
        /// </summary>
        /// <param name="sourcefile">The sourcefile.</param>
        /// <param name="destfile">The destfile.</param>
        /// <param name="showwindow">if set to <c>true</c> [showwindow].</param>
        /// <param name="minimizewindow">if set to <c>true</c> [minimizewindow].</param>
        /// <returns>The generated file.</returns>
        /// <remarks>Documented by Dev02, 2008-03-17</remarks>
        public FileInfo Decode(FileInfo sourcefile, FileInfo destfile, bool showwindow, bool minimizewindow)
        {
            StartExternalExe(this.DecodeApp, String.Format(this.DecodeArgs, '"' + sourcefile.FullName + '"', '"' + destfile.FullName + '"'), showwindow, minimizewindow);
            destfile.Refresh();
            if (destfile.Exists)
                return destfile;
            else
                return null;
        }

        /// <summary>
        /// Encodes the wavefile to a compressed file.
        /// </summary>
        /// <param name="sourcefile">The sourcefile.</param>
        /// <param name="tempfolder">The tempfolder.</param>
        /// <returns>The generated file.</returns>
        /// <remarks>Documented by Dev02, 2008-03-17</remarks>
        public FileInfo Encode(FileInfo sourcefile, DirectoryInfo tempfolder, bool showwindow, bool minimizewindow)
        {
            FileInfo destfile = new FileInfo(Path.ChangeExtension(Path.Combine(tempfolder.FullName, sourcefile.Name), this.extension));
            return Encode(sourcefile, destfile, showwindow, minimizewindow);
        }

        /// <summary>
        /// Encodes the wavefile to a compressed file.
        /// </summary>
        /// <param name="sourcefile">The sourcefile.</param>
        /// <param name="destfile">The destfile.</param>
        /// <param name="showwindow">if set to <c>true</c> [showwindow].</param>
        /// <param name="minimizewindow">if set to <c>true</c> [minimizewindow].</param>
        /// <returns>The generated file.</returns>
        /// <remarks>Documented by Dev02, 2008-03-17</remarks>
        public FileInfo Encode(FileInfo sourcefile, FileInfo destfile, bool showwindow, bool minimizewindow)
        {
            StartExternalExe(this.EncodeApp, String.Format(this.EncodeArgs, '"' + sourcefile.FullName + '"', '"' + destfile.FullName + '"'), showwindow, minimizewindow);
            destfile.Refresh();
            if (destfile.Exists)
                return destfile;
            else
                return null;
        }

        /// <summary>
        /// Starts an external executable.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="showwindow">if set to <c>true</c> [showwindow].</param>
        /// <param name="minimizewindow">if set to <c>true</c> [minimizewindow].</param>
        /// <remarks>Documented by Dev02, 2008-03-17</remarks>
        private static void StartExternalExe(string executable, string parameters, bool showwindow, bool minimizewindow)
        {
            FileInfo exe = GetExecutablePath(executable);
            if (exe != null)
            {
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.FileName = exe.FullName;
                pi.WorkingDirectory = exe.Directory.FullName;
                pi.Arguments = parameters;
                pi.WindowStyle = showwindow ? (minimizewindow ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal) : ProcessWindowStyle.Hidden;
                Process externalprocess = Process.Start(pi);
                externalprocess.WaitForExit();
            }
        }

        /// <summary>
        /// Checks the executable path.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <returns>The FileInfo, if it is valid, else null.</returns>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private static FileInfo GetExecutablePath(string executable)
        {
            if (string.IsNullOrEmpty(executable))
                return null;

            FileInfo exefile;

            if (Path.IsPathRooted(executable))
                exefile = new FileInfo(executable);
            else
                exefile = new FileInfo(Path.Combine(Application.StartupPath, executable));

            if (!exefile.Exists)
                exefile = null;

            return exefile;
        }

        /// <summary>
        /// Checks the executable path.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        public static bool CheckExecutablePath(string executable)
        {
            return GetExecutablePath(executable) != null;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Codec))
                return false;

            return this.Equals((Codec)obj);
        }

        /// <summary>
        /// Equalses the specified codec.
        /// </summary>
        /// <param name="codec">The codec.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public bool Equals(Codec codec)
        {
            if (codec == null)
                return false;

            return this.GetHashCode() == codec.GetHashCode();
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        public object Clone()
        {
            Codec codec = new Codec();
            codec.name = this.name;
            codec.extension = this.extension;
            codec.EncodeApp = this.EncodeApp;
            codec.EncodeArgs = this.EncodeArgs;
            codec.DecodeApp = this.DecodeApp;
            codec.DecodeArgs = this.DecodeArgs;
            return codec;
        }

        #endregion
    }
}
