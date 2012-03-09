using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using MLifterErrorHandler.Properties;
using System.Diagnostics;
using System.Collections.Generic;

namespace MLifterErrorHandler.BusinessLayer
{
    /// <summary>
    /// Class to manipulate an error report zip archive.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-07-17</remarks>
    public class ErrorReportHandler
    {
        private FileInfo file = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportHandler"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <remarks>Documented by Dev03, 2009-07-16</remarks>
        public ErrorReportHandler(FileInfo file)
        {
            this.file = file;
        }

        /// <summary>
        /// Gets the zip stream.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="zipFile">The zip file.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-07-17</remarks>
        private static Stream GetZipStream(ZipEntry entry, ZipFile zipFile)
        {
            MemoryStream output = new MemoryStream();
            using (InflaterInputStream s = (InflaterInputStream)zipFile.GetInputStream(entry))
            {
                byte[] buffer = new byte[4096];
                int size = 0;
                do
                {
                    size = s.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, size);
                }
                while (size > 0);
                output.Position = 0;
            }
            return output;
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="name">The Zip Entry.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-07-16</remarks>
        public Stream GetEntry(ZipEntry entry)
        {
            ZipFile zipFile = null;
            Stream output = null;
            try
            {
                zipFile = new ZipFile(file.FullName);
                output = GetZipStream(entry, zipFile);
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }
            return output;
        }

        /// <summary>
        /// Removes the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev03, 2009-07-16</remarks>
        public void RemoveFile(string filename)
        {
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(file.FullName);
                zipFile.BeginUpdate();
                zipFile.Delete(zipFile.GetEntry(filename));
                zipFile.CommitUpdate();
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="path">The path z.B. "Exception/Message".</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev07, 2009-07-16</remarks>
        public string GetValue(string path)
        {
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(file.FullName);

                ZipEntry errorReport = zipFile.GetEntry(Resources.ERRORFILE_NAME);

                using (Stream s = GetZipStream(errorReport, zipFile))
                {
                    XmlDocument doc = new XmlDocument();
                    using (StreamReader reader = new StreamReader(s, Encoding.Unicode))
                    {
                        doc.LoadXml(reader.ReadToEnd());
                    }
                    if (!path.StartsWith("/"))
                        path = "//" + path;
                    return doc.SelectSingleNode(path).InnerText;
                }
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }
        }

        private List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Documented by Dev03, 2009-07-16</remarks>
        public void SetValue(string nodeName, string value)
        {
            SetValue("/ErrorReport", nodeName, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Documented by Dev03, 2009-07-16</remarks>
        public void SetValue(string parentPath, string nodeName, string value)
        {
            Dictionary<string, string> newValue = new Dictionary<string, string>();
            newValue["parentPath"] = parentPath;
            newValue["nodeName"] = nodeName;
            newValue["value"] = value;
            values.Add(newValue);
        }

        /// <summary>
        /// Commits the updates.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-07-20</remarks>
        public void CommitUpdates()
        {
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(file.FullName);
                zipFile.UseZip64 = UseZip64.Off;  // AAB-20090720: Zip64 caused some problem when modifing the archive (ErrorReportHandler.cs) - Zip64 is required to bypass the 4.2G limitation of the original Zip format (http://en.wikipedia.org/wiki/ZIP_(file_format))

                ZipEntry errorReport = zipFile.GetEntry(Resources.ERRORFILE_NAME);

                MemoryStream stream = new MemoryStream();
                using (Stream s = GetZipStream(errorReport, zipFile))
                {
                    XmlDocument doc = new XmlDocument();
                    using (StreamReader reader = new StreamReader(s, Encoding.Unicode))
                    {
                        doc.LoadXml(reader.ReadToEnd());
                    }
                    foreach (Dictionary<string, string> value in values)
                    {
                        XmlElement xE = doc.CreateElement(value["nodeName"]);
                        xE.InnerText = value["value"].Trim();
                        XmlNode parentNode = doc.SelectSingleNode(value["parentPath"]);
                        if (parentNode == null)
                            return;
                        parentNode.AppendChild(xE);
                    }
                    doc.Save(stream);
                }
                ZipData data = new ZipData(stream);
                zipFile.BeginUpdate();
                zipFile.Delete(errorReport);    //delete first!
                zipFile.CommitUpdate();
                zipFile.BeginUpdate();
                zipFile.Add(data, errorReport.Name);
                zipFile.CommitUpdate();
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Close();
            }
        }
    }

    /// <summary>
    /// Custom implementation of IStaticDataSource.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-07-17</remarks>
    public class ZipData : IStaticDataSource
    {
        private Stream stream = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="ZipData"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev03, 2009-07-17</remarks>
        public ZipData(Stream stream)
        {
            this.stream = stream;
        }
        #region IStaticDataSource Members

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-07-17</remarks>
        public Stream GetSource()
        {
            stream.Position = 0;
            return stream;
        }

        #endregion
    }
}
