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

namespace StickFactory
{
    class UsbFolder
    {
        private string foldername;
        private List<UsbFolder> subfolders = new List<UsbFolder>();
        private List<UsbFile> subfiles = new List<UsbFile>();

        public UsbFolder() { }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        /// <value>The name of the folder.</value>
        /// <remarks>Documented by Dev08, 2008-10-07</remarks>
        public string FolderName
        {
            get
            {
                return foldername;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    foldername = value;

                    //Load all Folders and Files
                    DirectoryInfo dinfo = new DirectoryInfo(value);
                    foreach (DirectoryInfo tmp in dinfo.GetDirectories())
                    {
                        UsbFolder xf = new UsbFolder();
                        xf.FolderName = tmp.FullName;
                        SubFolders.Add(xf);
                    }

                    foreach (FileInfo tmp in dinfo.GetFiles())
                    {
                        UsbFile xf = new UsbFile();
                        xf.Filename = tmp.FullName;
                        this.SubFiles.Add(xf);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the sub folders of this folder.
        /// </summary>
        /// <value>The sub folders.</value>
        /// <remarks>Documented by Dev08, 2008-10-07</remarks>
        public List<UsbFolder> SubFolders
        {
            get
            {
                return subfolders;
            }
        }

        /// <summary>
        /// Gets the sub files of this folder.
        /// </summary>
        /// <value>The sub files.</value>
        /// <remarks>Documented by Dev08, 2008-10-07</remarks>
        public List<UsbFile> SubFiles
        {
            get
            {
                return subfiles;
            }
        }
    }
}
