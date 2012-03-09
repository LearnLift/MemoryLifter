using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using MLifter.DAL;
using MLifter.DAL.Tools;

namespace MLifter.DAL.ImportExport
{
	/// <summary>
	/// Provides functionality to export learning modules to text.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-08-21</remarks>
	public class Exporter
	{
		/// <summary>
		/// Occurs when the progress changed.
		/// </summary>
		public event EventHandler<StatusMessageEventArgs> ProgressChanged;
		BackgroundWorker m_backgroundworker = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Exporter"/> class.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		public Exporter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Exporter"/> class.
		/// </summary>
		/// <param name="backgroundworker">The backgroundworker.</param>
		/// <remarks>Documented by Dev03, 2008-08-21</remarks>
		public Exporter(BackgroundWorker backgroundworker)
		{
			m_backgroundworker = backgroundworker;
		}

		/// <summary>
		/// Gets the background worker.
		/// </summary>
		internal BackgroundWorker BackgroundWorker
		{
			get
			{
				return m_backgroundworker;
			}
		}

		/// <summary>
		/// Exports to CSV.
		/// </summary>
		/// <param name="csvFilename">The CSV filename.</param>
		/// <param name="xslStyleSheet">The XSL style sheet.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="chapters">The chapters - format "SeparatorChapternumberSeparator", e.g. ",1,2,4,".</param>
		/// <param name="header">The header.</param>
		/// <param name="fieldsToExport">The fields to export.</param>
		/// <param name="backgroundworker">The backgroundworker.</param>
		/// <param name="CardCount">The card count.</param>
		/// <remarks>Documented by Dev05, 2007-09-03</remarks>
		public void ExportToCSV(string csvFilename, string xslStyleSheet, string dictionary, string separator, string chapters, string header, XmlDocument fieldsToExport,
			BackgroundWorker backgroundworker, int CardCount)
		{
			StreamWriter file = new StreamWriter(csvFilename, false, System.Text.Encoding.Unicode);
			XslCompiledTransform transformer = new XslCompiledTransform();
			XsltArgumentList arguments = new XsltArgumentList();
			XsltSettings settings = new XsltSettings(false, false);
			transformer.Load(xslStyleSheet, settings, new XmlUrlResolver());

			arguments.AddParam("separator", string.Empty, separator);
			arguments.AddParam("chapters", string.Empty, chapters);
			arguments.AddParam("header", string.Empty, header);
			arguments.AddParam("fieldsElement", string.Empty, fieldsToExport);
			//arguments.AddExtensionObject("urn:status", new ExportStatusReport(backgroundworker, CardCount));
			transformer.Transform(dictionary, arguments, file);

			file.Close();
		}
		/// <summary>
		/// Exports to CSV.
		/// </summary>
		/// <param name="csvFilename">The CSV filename.</param>
		/// <param name="xslStyleSheet">The XSL style sheet.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="chapters">The chapters - format "SeparatorChapternumberSeparator", e.g. ",1,2,4,".</param>
		/// <param name="header">The header.</param>
		/// <param name="copyFiles">if set to <c>true</c> [copy files].</param>
		/// <param name="fieldsToExport">The fields to export.</param>
		/// <remarks>
		/// Documented by CFI, 2007-09-03.
		/// </remarks>
		public void ExportToCSV(string csvFilename, string xslStyleSheet, Interfaces.IDictionary dictionary, string separator, string chapters, string header, bool copyFiles, XmlDocument fieldsToExport)
		{
			StreamWriter file = null;
			try
			{
				file = new StreamWriter(csvFilename, false, System.Text.Encoding.Unicode);
				XslCompiledTransform transformer = new XslCompiledTransform();
				XsltArgumentList arguments = new XsltArgumentList();
				XsltSettings settings = new XsltSettings(false, false);
				transformer.Load(xslStyleSheet, settings, new XmlUrlResolver());

				arguments.AddParam("separator", string.Empty, separator);
				arguments.AddParam("chapters", string.Empty, chapters);
				arguments.AddParam("header", string.Empty, header);
				arguments.AddParam("fieldsElement", string.Empty, fieldsToExport);
				arguments.AddParam("copyFiles", string.Empty, copyFiles.ToString().ToLower());
				arguments.AddExtensionObject("urn:status", new ExportStatusReport(this, dictionary.Cards.Count));
				arguments.AddExtensionObject("urn:export", new ExportHelper(dictionary, Path.GetFullPath(csvFilename)));
				if (m_backgroundworker != null)
					dictionary.BackgroundWorker = m_backgroundworker;
				else
					dictionary.XmlProgressChanged += new StatusMessageEventHandler(dictionary_XmlProgressChanged);
				XmlDocument xDic = new XmlDocument();
				xDic.LoadXml(dictionary.Xml);
				if (m_backgroundworker != null)
					dictionary.BackgroundWorker = null;
				else
					dictionary.XmlProgressChanged -= new StatusMessageEventHandler(dictionary_XmlProgressChanged);
				transformer.Transform(xDic, arguments, file);
			}
			finally
			{
				if (file != null)
					file.Close();
			}
		}

		void dictionary_XmlProgressChanged(object sender, StatusMessageEventArgs e)
		{
			this.ProgressChanged(sender, e);
		}

		/// <summary>
		/// Provides some helper methods for export.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-08-11</remarks>
		public class ExportHelper
		{
			private MLifter.DAL.Interfaces.IDictionary m_dictionary;
			private string m_exportPath;
			private string m_exportFolder;

			/// <summary>
			/// Initializes a new instance of the <see cref="ExportHelper"/> class.
			/// </summary>
			/// <param name="dictionary">The dictionary.</param>
			/// <param name="exportPath">The full export path including the export file name.</param>
			/// <remarks>Documented by Dev03, 2008-08-20</remarks>
			public ExportHelper(MLifter.DAL.Interfaces.IDictionary dictionary, string exportPath)
			{
				m_dictionary = dictionary;
				m_exportPath = exportPath;
				m_exportFolder = Path.GetDirectoryName(exportPath);
			}

			/// <summary>
			/// Gets the local copy of the media file and returns the new path.
			/// </summary>
			/// <param name="fileRef">The file reference.</param>
			/// <param name="mediaType">Type of the media object.</param>
			/// <returns>The path where the media file is stored.</returns>
			/// <remarks>Documented by Dev03, 2008-08-11</remarks>
			public string GetLocalFile(string fileRef, string mediaType)
			{
				string filePath = String.Empty;
				string fileName = String.Empty;
				MLifter.DAL.Interfaces.EMedia type = (MLifter.DAL.Interfaces.EMedia)Enum.Parse(typeof(MLifter.DAL.Interfaces.EMedia), mediaType);

				if (m_dictionary is MLifter.DAL.XML.XmlDictionary)
				{
					if (!Path.IsPathRooted(fileRef))
					{
						fileRef = Path.Combine(Path.GetDirectoryName(m_dictionary.Connection), fileRef);
					}
					if (!File.Exists(fileRef)) return String.Empty;
				}
				else if ((m_dictionary is MLifter.DAL.DB.DbDictionary) && (fileRef.Trim().Length == 0))
				{
					return String.Empty;
				}

				MLifter.DAL.Interfaces.IMedia media;
				try
				{
					media = m_dictionary.CreateMedia(type, fileRef, true, false, false);

					byte[] buffer = new byte[media.Stream.Length];
					media.Stream.Read(buffer, 0, (int)media.Stream.Length);

					if (media is MLifter.DAL.DB.DbMedia)
					{
						fileName = "mf_" + media.Id.ToString() + media.Extension;
					}
					else
					{
						fileName = Path.GetFileName(fileRef);
					}
					filePath = Path.Combine(m_exportFolder, fileName);
					using (FileStream fs = new FileStream(filePath, FileMode.Create))
					{
						fs.Write(buffer, 0, buffer.Length);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception in ExportHelper.GetLocalFile()! (" + ex.Message + ")");
				}
				return fileName;
			}
		}

		/// <summary>
		/// The Export status report class.
		/// </summary>
		public class ExportStatusReport
		{
			private int CardCount;
			private Exporter exporter;
			private StatusMessageEventArgs status;

			internal ExportStatusReport(Exporter exporter, int cardCount)
			{
				this.exporter = exporter;
				CardCount = cardCount;
				status = new StatusMessageEventArgs(StatusMessageType.ExporterProgress, CardCount);
			}

			/// <summary>
			/// Sends the status.
			/// </summary>
			/// <returns></returns>
			public bool SendStatus()
			{
				bool cancelProcess = false;
				if (exporter.BackgroundWorker != null)
				{
					if (exporter.BackgroundWorker.CancellationPending)
					{
						cancelProcess = true;
					}
					else
					{
						if (status.Progress % 10 == 0)
							exporter.BackgroundWorker.ReportProgress(status.ProgressPercentage);
					}
				}
				else
				{
					if (status.Progress % 10 == 0)
						exporter.ProgressChanged(null, status);
				}
				status.Progress++;
				return !cancelProcess;
			}
		}
	}
}
