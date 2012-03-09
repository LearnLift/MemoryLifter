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
