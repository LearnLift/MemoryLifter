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
using System.Diagnostics;

namespace MLifter.DAL.Tools
{
    /// <summary>
    /// A class where files can be added to be deleted later (temporary files).
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-09-29</remarks>
    public class FileCleanupQueue : Queue<string>
    {
        /// <summary>
        /// Deletes all files that are added for cleanup.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        public void DoCleanup()
        {
            if (this.Count > 0)
            {
                Debug.WriteLine(string.Format("Beginning cleanup of {0} temporary files.", this.Count));
                while (this.Count > 0)
                {
                    string filename = this.Dequeue();
                    try
                    {
                        if (filename != null && filename.Length > 0 && File.Exists(filename))
                            File.Delete(filename);
                    }
                    catch (Exception exp)
                    {
                        Debug.WriteLine(string.Format("File \"{0}\" could not be deleted: {1}.", filename, exp.Message));
                    }
                }
                Debug.WriteLine("Temporary files cleanup finished.");
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FileCleanupQueue"/> is reclaimed by garbage collection.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        ~FileCleanupQueue()
        {
            DoCleanup();
        }

        /// <summary>
        /// Gets a temporary file path (without extension).
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        public string GetTempFilePath()
        {
            return GetTempFilePath(string.Empty);
        }

        /// <summary>
        /// Gets a temporary file path with a specified extension.
        /// </summary>
        /// <param name="extension">The extension (with dot, e.g. .txt).</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        public string GetTempFilePath(string extension)
        {
            string path;
            do
            {
                path = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(), 
                    System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), extension)
                    );
            }
            while (File.Exists(path));
            this.Enqueue(path);
            return path;
        }
    }
}
