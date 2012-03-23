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
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;


namespace MLifter.DAL.Tools
{
	/// <summary>
	/// Helper Methods for ZIP-Files
	/// </summary>
	public static class ZipHelper
	{
		/// <summary>
		/// Extracts the zip stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="targetpath">The targetpath.</param>
		/// <remarks>
		/// Documented by DanAch, 2009-07-03.
		/// </remarks>
		public static void ExtractZipStream(Stream stream, string targetpath)
		{
			if (!Directory.Exists(targetpath))
				Directory.CreateDirectory(targetpath);

			using (ZipInputStream zipStream = new ZipInputStream(stream))
			{
				ZipEntry zipEntry;

				int nBytes = 2048;
				byte[] data = new byte[nBytes];

				while ((zipEntry = zipStream.GetNextEntry()) != null)
				{
					if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(targetpath, zipEntry.Name))))
						Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(targetpath, zipEntry.Name)));

					try
					{
						// write file
						string filePath = Path.Combine(targetpath, zipEntry.Name);
						FileMode fileMode = (File.Exists(filePath)) ? FileMode.Truncate : FileMode.OpenOrCreate;
						using (FileStream writer = new FileStream(filePath, fileMode, FileAccess.ReadWrite, FileShare.Read))
						{
							while ((nBytes = zipStream.Read(data, 0, data.Length)) > 0)
								writer.Write(data, 0, nBytes);
						}
					}
					catch (Exception exp)
					{
						Trace.WriteLine("ExtractZip error: " + exp.ToString());
					}
				}
				zipStream.Close();
			}
		}
		/// <summary>
		/// Gets the content of the zip.
		/// </summary>
		/// <param name="zipFile">The zip file.</param>
		/// <param name="showDirectory">if set to <c>true</c> [show directory].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev07, 2009-07-06</remarks>
		public static List<string> GetZipContent(Stream zipFile, bool showDirectory)
		{
			List<string> content = new List<string>();
			ZipEntry zipEntry;
			try
			{
				using (ZipInputStream zipStream = new ZipInputStream(zipFile))
				{
					while ((zipEntry = zipStream.GetNextEntry()) != null)
					{
						if (!showDirectory && zipEntry.IsDirectory)
							continue;

						content.Add(zipEntry.Name);
					}
				}
			}
			catch (Exception ze)
			{ Trace.WriteLine("Zip Exception: " + ze.ToString()); }
			return content;
		}
	}
}
