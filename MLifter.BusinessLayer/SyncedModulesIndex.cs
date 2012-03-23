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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Maintains a list of the fully synced modules for offline availability.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-03-26</remarks>
    public static class SyncedModulesIndex
    {
        private static Dictionary<string, List<LearningModulesIndexEntry>> syncedModules = new Dictionary<string, List<LearningModulesIndexEntry>>();

        /// <summary>
        /// Adds the specified synced module index entry, together with the online connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev02, 2009-03-26</remarks>
        public static void Add(IConnectionString connectionString, LearningModulesIndexEntry entry)
        {
            if (!syncedModules.ContainsKey(connectionString.ConnectionString))
                syncedModules[connectionString.ConnectionString] = new List<LearningModulesIndexEntry>();

            syncedModules[connectionString.ConnectionString].RemoveAll(e => e.SyncedPath == entry.SyncedPath && e.ConnectionName == entry.ConnectionName && e.UserName == entry.UserName);
            syncedModules[connectionString.ConnectionString].Add(entry);
        }

        /// <summary>
        /// Removes all synced modules of a specified connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <remarks>Documented by Dev02, 2009-03-26</remarks>
        public static void Remove(IConnectionString connectionString)
        {
            if (syncedModules.ContainsKey(connectionString.ConnectionString))
                syncedModules.Remove(connectionString.ConnectionString);
        }

        /// <summary>
        /// Removes a specific offline module index entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev02, 2009-03-26</remarks>
        public static void Remove(IConnectionString connectionString, LearningModulesIndexEntry entry)
        {
            if (syncedModules.ContainsKey(connectionString.ConnectionString))
            {
                if (syncedModules[connectionString.ConnectionString].Contains(entry))
                    syncedModules[connectionString.ConnectionString].Remove(entry);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.Generic.List`1"></see>.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-03-26</remarks>
        public static void Clear()
        {
            syncedModules.Clear();
        }

        /// <summary>
        /// Gets the offline module index entries for a connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-03-26</remarks>
        public static List<LearningModulesIndexEntry> Get(IConnectionString connectionString)
        {
            if (!syncedModules.ContainsKey(connectionString.ConnectionString))
                return new List<LearningModulesIndexEntry>();

            return syncedModules[connectionString.ConnectionString];
        }

        #region Serialize & File functions
        /// <summary>
        /// Serializes to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private static void Serialize(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, syncedModules);
        }

        /// <summary>
        /// Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private static void Deserialize(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            syncedModules = (Dictionary<string, List<LearningModulesIndexEntry>>)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Dumps the index contents to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public static void Dump(string filename)
        {
            if (syncedModules.Count > 0)
            {
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(filename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(filename));

                    using (Stream stream = new FileStream(filename, FileMode.Create))
                    {
                        using (Stream cStream = new CryptoStream(stream, Rijndael.Create().CreateEncryptor(Encoding.Unicode.GetBytes("mlifter"), Encoding.Unicode.GetBytes("omicron00")), CryptoStreamMode.Write))
                            Serialize(cStream);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("SyncedLearningModules.Dump() - " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Restores the index contents from a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public static void Restore(string filename)
        {
            syncedModules.Clear();

            try
            {
                if (!File.Exists(filename))
                    return;

                using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (Stream cStream = new CryptoStream(stream, Rijndael.Create().CreateDecryptor(Encoding.Unicode.GetBytes("mlifter"), Encoding.Unicode.GetBytes("omicron00")), CryptoStreamMode.Read))
                        Deserialize(cStream);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SyncedLearningModules.Restore() - " + ex.Message);
                try { File.Delete(filename); }
                catch { }
            }
        }
        #endregion
    }
}
