using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Properties;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;

namespace MLifter.DAL.Pack
{
	/// <summary>
	/// This class packs an Xml learning module into a ZIP file.
	/// </summary>
	public sealed class Packer
	{
		private BackgroundWorker m_BackgroundWorker = null;
		private string m_temporaryFolder = String.Empty;
		private GetLoginInformation m_loginCallback = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Packer"/> class.
		/// </summary>
		/// <remarks>Documented by Dev03, 2007-09-10</remarks>
		public Packer()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Packer"/> class.
		/// </summary>
		/// <param name="backgroundWorker">The background worker.</param>
		/// <remarks>Documented by Dev03, 2007-09-10</remarks>
		public Packer(BackgroundWorker backgroundWorker)
		{
			m_BackgroundWorker = backgroundWorker;
		}

		/// <summary>
		/// Gets the ZIP extension.
		/// </summary>
		/// <value>The ZIP extension.</value>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		public static string Extension
		{
			get { return Resources.DICTIONARY_ARCHIVE_EXTENSION; }
		}

		/// <summary>
		/// Gets or sets the temorary folder where the dictionary should be unpacked to.
		/// </summary>
		/// <value>The temorary folder.</value>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		public string TemporaryFolder
		{
			get { return m_temporaryFolder; }
			set { m_temporaryFolder = value; }
		}

		/// <summary>
		/// Gets or sets the login callback.
		/// </summary>
		/// <value>The login callback.</value>
		/// <remarks>Documented by Dev03, 2008-09-08</remarks>
		public GetLoginInformation LoginCallback
		{
			get { return m_loginCallback; }
			set { m_loginCallback = value; }
		}

		/// <summary>
		/// Packs the specified dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="archivePath">The archive path.</param>
		/// <returns>The full path (including the filename) to the archive or String.Empty of the process was interrupted.</returns>
		/// <remarks>Documented by Dev03, 2007-09-10</remarks>
		public string Pack(IDictionary dictionary, string archivePath)
		{
			string dictionaryPath = ((XML.XmlDictionary)dictionary).Path;
			string fullArchivePath = String.Empty;
			if ((Path.GetFileName(archivePath).Length == 0) || (Path.GetExtension(archivePath).Length == 0))
			{
				fullArchivePath = Path.Combine(archivePath, Path.GetRandomFileName() + Resources.DICTIONARY_ARCHIVE_EXTENSION);
			}
			else
			{
				fullArchivePath = archivePath;
			}

			FileStream target = File.Create(fullArchivePath);
			//ZipConstants.DefaultCodePage = Encoding.Unicode.CodePage;   //this may possibly mean that we are not Zip compatible anymore
			ZipOutputStream zip_stream = new ZipOutputStream(target);

			// set compression level 0(low) to 9 (high)
			zip_stream.SetLevel(5);
			// write the old category id to the header for backward compatibility
			zip_stream.SetComment(String.Format(Resources.PACKER_ARCHIVE_HEADER_COMMENT_PACK, dictionary.Category.OldId));

			string dirPath = Path.GetDirectoryName(dictionaryPath);
			Directory.SetCurrentDirectory(dirPath);
			List<string> resourceList = dictionary.GetResources();

			resourceList.Add(dictionaryPath);   //add the dictionary

			if (!ReportProgressUpdate(0)) return String.Empty;
			for (int i = 0; i < resourceList.Count; i++)
			{
				string fullPath, relPath;
				if (Path.IsPathRooted(resourceList[i]))
				{
					if (resourceList[i].StartsWith(dirPath))
					{
						relPath = resourceList[i].Replace(dirPath, String.Empty).Trim(new char[] { Path.DirectorySeparatorChar, ' ' });
						fullPath = resourceList[i];
					}
					else
					{
						continue;
					}
				}
				else
				{
					try
					{
						relPath = resourceList[i];
						fullPath = Path.GetFullPath(resourceList[i]);
					}
					catch
					{
						continue;
					}
				}
				if (!File.Exists(fullPath))
					continue;

				FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				// allocate file buffer
				byte[] buffer = new byte[fs.Length];
				fs.Read(buffer, 0, buffer.Length);

				// writing zip entry and data
				ZipEntry entry = new ZipEntry(relPath);
				zip_stream.PutNextEntry(entry);
				zip_stream.Write(buffer, 0, buffer.Length);
				zip_stream.CloseEntry();
				fs.Close();

				int progress = (int)Math.Floor(100.0 * (i + 1) / resourceList.Count);
				if (!ReportProgressUpdate(progress)) return String.Empty;
			}
			if (!ReportProgressUpdate(100)) return String.Empty;

			zip_stream.Finish();
			zip_stream.Close();
			target.Close();
			return fullArchivePath;
		}

		/// <summary>
		/// Unpacks the specified source path.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		public IDictionary Unpack(string sourcePath)
		{
			return Unpack(sourcePath, false);
		}

		/// <summary>
		/// Unpacks the specified source path.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <param name="ignoreInvalidHeaders">if set to <c>true</c> [ignore invalid headers].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		public IDictionary Unpack(string sourcePath, bool ignoreInvalidHeaders)
		{
			string dictionaryPath = String.Empty;
			//ZipConstants.DefaultCodePage = Encoding.Unicode.CodePage;   //this may possibly mean that we are not Zip compatible anymore

			using (FileStream sourceStream = File.OpenRead(sourcePath))
			{
				string temporaryPath;

				if (!ignoreInvalidHeaders)
				{
					// Checking Zip File Header
					if (!IsValidArchive(sourcePath))
					{
						throw new NoValidArchiveException();
					}
				}

				long zipFileSize = 0;

				ZipFile zipFile = null;
				try
				{
					zipFile = new ZipFile(sourcePath);
					zipFileSize = zipFile.Count;
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					if (zipFile != null)
					{
						zipFile.Close();
					}
				}

				if (m_temporaryFolder.Length == 0)
				{
					temporaryPath = Path.GetTempPath();
					temporaryPath = Path.Combine(temporaryPath, Path.GetRandomFileName());
				}
				else
				{
					temporaryPath = m_temporaryFolder;
				}

				if (!Directory.Exists(temporaryPath))
					Directory.CreateDirectory(temporaryPath);

				System.IO.Directory.SetCurrentDirectory(temporaryPath);

				// Extract files
				ZipInputStream zipStream = new ZipInputStream(sourceStream);
				ZipEntry zipEntry;

				int nBytes = 2048;
				byte[] data = new byte[nBytes];

				int counter = 1;
				if (!ReportProgressUpdate(0)) return null;
				while ((zipEntry = zipStream.GetNextEntry()) != null)
				{
					if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(temporaryPath, zipEntry.Name))))
						Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(temporaryPath, zipEntry.Name)));

					if (Path.GetExtension(zipEntry.Name).ToLower() == Resources.DICTIONARY_ODX_EXTENSION)
						dictionaryPath = Path.Combine(temporaryPath, zipEntry.Name);

					// write file
					try
					{
						string filePath = Path.Combine(temporaryPath, zipEntry.Name);
						FileMode fileMode = (File.Exists(filePath)) ? FileMode.Truncate : FileMode.OpenOrCreate;
						using (FileStream writer = new FileStream(filePath, fileMode, FileAccess.ReadWrite, FileShare.Read))
						{
							while ((nBytes = zipStream.Read(data, 0, data.Length)) > 0)
							{
								writer.Write(data, 0, nBytes);
							}
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("DAL.Pack.Unpack() - " + zipEntry.Name + " - " + ex.Message);
						if (zipEntry.Name.Contains(DAL.Helper.OdxExtension))
							throw ex;
					}

					int progress = (int)Math.Floor(100.0 * (counter++) / zipFileSize);
					if (!ReportProgressUpdate(progress)) return null;
				}
				if (!ReportProgressUpdate(100)) return null;
				zipStream.Close();
				sourceStream.Close();
			}

			if (dictionaryPath.Length == 0)
			{
				throw new NoValidArchiveException();
			}

			return UserFactory.Create(m_loginCallback, new ConnectionStringStruct(DatabaseType.Xml, dictionaryPath, true), 
				(DataAccessErrorDelegate)delegate { return; }, this).Open();
		}

		/// <summary>
		/// Determines whether [is valid archive] [the specified path].
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid archive] [the specified path]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2007-09-11</remarks>
		public static bool IsValidArchive(string path)
		{
			bool isValid = false;
			ZipFile zipFile = null;
			try
			{
				zipFile = new ZipFile(path);
				isValid = zipFile.ZipFileComment.StartsWith(Resources.PACKER_ARCHIVE_HEADER_COMMENT_UNPACK);
				zipFile.Close();
			}
			catch { }
			finally
			{
				if (zipFile != null)
				{
					zipFile.Close();
				}
			}
			return isValid;
		}

		/// <summary>
		/// Reads out the category from the archive comment.
		/// </summary>
		/// <param name="path">The archive path.</param>
		/// <returns>The category number, negative when readout failed.</returns>
		/// <remarks>Documented by Dev02, 2007-12-10</remarks>
		public static int ArchiveCategory(string path)
		{
			if (!IsValidArchive(path))
				return -1;

			int archiveCategory = -1;
			ZipFile zipFile = null;
			try
			{
				zipFile = new ZipFile(path);
				Regex categoryRegex = new Regex(string.Format(Resources.PACKER_ARCHIVE_HEADER_COMMENT_PACK, @"(?<Category>\d+)"));
				Match categoryMatch = categoryRegex.Match(zipFile.ZipFileComment);
				zipFile.Close();

				if (categoryMatch.Success)
				{
					int categoryMatchNumber = Int32.Parse(categoryMatch.Groups["Category"].Value);
					if (categoryMatchNumber >= 0)
						archiveCategory = categoryMatchNumber;
				}
			}
			catch { }
			finally
			{
				if (zipFile != null)
				{
					zipFile.Close();
				}
			}

			return archiveCategory;
		}

		/// <summary>
		/// Reports the progress update.
		/// </summary>
		/// <param name="percent">The process advancement percent.</param>
		/// <returns>True if the process has been canceled.</returns>
		/// <remarks>Documented by Dev03, 2007-09-03</remarks>
		private bool ReportProgressUpdate(int percent)
		{
			bool cancelProcess = false;
			if (m_BackgroundWorker != null)
			{
				if (m_BackgroundWorker.CancellationPending)
				{
					cancelProcess = true;
				}
				else
				{
					m_BackgroundWorker.ReportProgress(percent);
				}
			}
			return !cancelProcess;
		}
	}
}
