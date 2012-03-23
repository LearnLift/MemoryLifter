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
	class UsbFileDirectory
	{
		private static int count = 0;

		/// <summary>
		/// Internal function to calculate the number of files in a defined path. 
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2008-10-07</remarks>
		private static int GetFileCountInternal(string path)
		{
			DirectoryInfo dinfo = new DirectoryInfo(path);          //Current path
			DirectoryInfo[] subdinfos = dinfo.GetDirectories();     //SubFolder of the current path
			FileInfo[] files = dinfo.GetFiles();                    //Files of the current path (without subfolders)

			count += files.Length;

			foreach (DirectoryInfo d in subdinfos)
			{
				GetFileCountInternal(d.FullName);
			}

			return count;
		}

		/// <summary>
		/// Internal function to calculate the number of folders in a defined path
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2008-10-07</remarks>
		private static int GetFolderCountInternal(string path)
		{
			DirectoryInfo dinfo = new DirectoryInfo(path);          //Current path
			DirectoryInfo[] subdinfos = dinfo.GetDirectories();     //SubFolder of the current path

			count += dinfo.GetDirectories().Length;

			foreach (DirectoryInfo d in subdinfos)
			{
				GetFolderCountInternal(d.FullName);
			}

			return count;
		}

		/// <summary>
		/// Gets the number of files in a defined path
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2008-10-07</remarks>
		public static int GetFileCount(string path)
		{
			count = 0;
			int temp = GetFileCountInternal(path);
			count = 0;

			return temp;
		}

		/// <summary>
		/// Gets the folder count.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>CFI, 2012-02-24</remarks>
		public static int GetFolderCount(string path)
		{
			count = 0;
			int temp = GetFolderCountInternal(path);
			count = 0;

			return temp;
		}
	}
}
