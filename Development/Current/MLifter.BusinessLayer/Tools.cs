using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.IO;

using MLifter.DAL;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using MLifter.Generics;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;

namespace MLifter
{
    public class Win32Window : IWin32Window
    {
        private IntPtr hwnd;

        public Win32Window(IntPtr handle)
        {
            hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return hwnd; }
        }
    }

    public class Tools
    {
        private const string m_short = "...";
        /// <summary>
        /// Gets the shortened path, and lets you set a maximum length.
        /// </summary>
        /// <param name="longPath">The long path.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-27</remarks>
        [Obsolete("If you do not need a maximum length, use GetShortPath(string path) instead (faster).")]
        public static string GetShortPath(string longPath, int length)
        {
            if (longPath.Length <= length)
                return longPath;

            List<string> pathList = new List<string>();
            pathList.AddRange(longPath.Split(Path.DirectorySeparatorChar));

            bool bore = true;
            string output = longPath[0].ToString() + m_short;
            string begin = string.Empty;
            string end = string.Empty;

            while (pathList.Count != 0)
            {
                if (bore)
                {
                    begin += pathList[0].ToString() + Path.DirectorySeparatorChar;
                    pathList.RemoveAt(0);
                }
                else
                {
                    end = Path.DirectorySeparatorChar + pathList[pathList.Count - 1].ToString() + end;
                    pathList.RemoveAt(pathList.Count - 1);
                }

                if (output.Length >= length)
                    return output;
                else
                    output = begin + m_short + end;

                bore = !bore;
            }

            return output;
        }

        /// <summary>
        /// Gets the shortened path in this form:
        /// C:\Documents and Settings\...\UnitTests\UnitTests.vb
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-06-27</remarks>
        public static string GetShortPath(string path)
        {
            const string pattern = @"^(\w+:|\\)(\\[^\\]+\\).*(\\[^\\]+\\[^\\]+)$";
            const string replacement = "$1$2...$3";
            if (Regex.IsMatch(path, pattern))
            {
                return Regex.Replace(path, pattern, replacement);
            }
            else
            {
                return path;
            }
        }

        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        /// <summary>
        /// Checks whether user is online or not.
        /// </summary>
        /// <returns>true / false</returns>
        /// <remarks>Documented by Dev04, 2007-07-23</remarks>
        public static bool IsUserOnline()
        {
            int connect_status =
                2 + //user uses arrayList lan
                1 + //user uses arrayList modem
                4;  //user uses arrayList proxy
            return InternetGetConnectedState(out connect_status, 0);
        }

        /// <summary>
        /// Creates the round rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public static GraphicsPath CreateRoundRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int l = rectangle.Left;
            int t = rectangle.Top;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int d = radius << 1;

            path.AddArc(l, t, d, d, 180, 90); // topleft
            path.AddLine(l + radius, t, l + w - radius, t); // top
            path.AddArc(l + w - d, t, d, d, 270, 90); // topright
            path.AddLine(l + w, t + radius, l + w, t + h - radius); // right
            path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright
            path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom
            path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft
            path.AddLine(l, t + h - radius, l, t + radius); // left

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Converts a numeric value into a string that represents the number
        /// expressed as a size value in bytes, kilobytes, megabytes, gigabytes,
        /// or terabytes depending on the size. Output is identical to
        /// StrFormatByteSize() in shlwapi.dll. This is a format similar to
        /// the Windows Explorer file Properties page. For example:
        ///      532 ->  532 bytes
        ///     1240 -> 1.21 KB
        ///   235606 ->  230 KB
        ///  5400016 -> 5.14 MB
        /// </summary>
        /// <remarks>
        /// It was surprisingly difficult to emulate the StrFormatByteSize() function
        /// due to a few quirks. First, the function only displays three digits:
        ///  - displays 2 decimal places for values under 10            (e.g. 2.12 KB)
        ///  - displays 1 decimal place for values under 100            (e.g. 88.2 KB)
        ///  - displays 0 decimal places for values under 1000         (e.g. 532 KB)
        ///  - jumps to the next unit of measure for values over 1000    (e.g. 0.97 MB)
        /// The second quirk: insiginificant digits are truncated rather than
        /// rounded. The original function likely uses integer math.
        /// This implementation was tested to 100 TB.
        /// </remarks>
        public static string FileSizeToString(long fileSize)
        {
            if (fileSize < 1024)
            {
                return string.Format("{0} bytes", fileSize);
            }
            else
            {
                double value = fileSize;
                value = value / 1024;
                string unit = "KB";
                if (value >= 1000)
                {
                    value = Math.Floor(value);
                    value = value / 1024;
                    unit = "MB";
                }
                if (value >= 1000)
                {
                    value = Math.Floor(value);
                    value = value / 1024;
                    unit = "GB";
                }
                if (value >= 1000)
                {
                    value = Math.Floor(value);
                    value = value / 1024;
                    unit = "TB";
                }

                if (value < 10)
                {
                    value = Math.Floor(value * 100) / 100;
                    return string.Format("{0:n2} {1}", value, unit);
                }
                else if (value < 100)
                {
                    value = Math.Floor(value * 10) / 10;
                    return string.Format("{0:n1} {1}", value, unit);
                }
                else
                {
                    value = Math.Floor(value * 1) / 1;
                    return string.Format("{0:n0} {1}", value, unit);
                }
            }
        }

        /// <summary>
        /// Gets the synced path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-04-22</remarks>
        public static string GetSyncedPath(string fullPath, string syncedLearningModulePath, string connectionName, string userName)
        {
            return fullPath.Substring(GetSyncedPathPrefix(syncedLearningModulePath, connectionName, userName).Length);
        }
        /// <summary>
        /// Gets the full sync path.
        /// </summary>
        /// <param name="syncedPath">The synced path.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-04-22</remarks>
        public static string GetFullSyncPath(string syncedPath, string syncedLearningModulePath, string connectionName, string userName)
        {
            return Path.Combine(GetSyncedPathPrefix(syncedLearningModulePath, connectionName, userName), Methods.GetValidPathName(syncedPath));
        }
        /// <summary>
        /// Gets the synced path prefix.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-04-22</remarks>
        public static string GetSyncedPathPrefix(string syncedLearningModulePath, string connectionName, string userName)
        {
            string folder = Path.Combine(Path.Combine(syncedLearningModulePath, Methods.GetValidPathName(connectionName)), Methods.GetValidPathName(userName));
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }
    }
}