using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Imaging;
using System.Drawing;

namespace MLifter.BusinessLayer
{
    public partial class LearningModulesIndex
    {
        #region Cache fields
        /// <summary>
        /// The cache of index entries.
        /// </summary>
        private Dictionary<string, LearningModulesIndexEntry> indexCache =
            new Dictionary<string, LearningModulesIndexEntry>();

        /// <summary>
        /// The timestamps of the cache index entries.
        /// </summary>
        private Dictionary<string, DateTime> indexCacheTimestamps =
            new Dictionary<string, DateTime>();
        #endregion

        #region Methods for getting and refreshing index cache entries
        /// <summary>
        /// Refreshes the cache item.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev02, 2008-12-11</remarks>
        public void RefreshCacheItem(LearningModulesIndexEntry entry)
        {
            if (entry.IsVerified)
            {
                string key = GetEntryKey(entry);
                lock (indexCache)
                {
                    lock (indexCacheTimestamps)
                    {
                        if (!indexCache.ContainsKey(key) && entry.IsAccessible)
                            indexCache.Add(key, entry);
                        else
                            indexCache[key] = entry;
                        if (!indexCacheTimestamps.ContainsKey(key))
                            indexCacheTimestamps.Add(key, DateTime.Now);
                        else
                            indexCacheTimestamps[key] = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cache item.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>True, if success, false, when the entry was not found in cache.</returns>
        /// <remarks>Documented by Dev02, 2008-12-11</remarks>
        public bool GetCacheItem(LearningModulesIndexEntry entry)
        {
            string key = GetEntryKey(entry);
            if (!indexCache.ContainsKey(key))
                return false;

            LearningModulesIndexEntry cacheEntry = indexCache[key];

            if (cacheEntry.Preview != null && cacheEntry.Preview.PreviewImage != null)
                entry.Preview = cacheEntry.Preview;
            if (cacheEntry.Statistics != null)
                entry.Statistics = cacheEntry.Statistics;
            entry.Author = cacheEntry.Author;
            entry.Count = cacheEntry.Count;
            entry.Category = cacheEntry.Category;
            entry.Description = cacheEntry.Description;
            entry.DisplayName = cacheEntry.DisplayName;
            entry.LastTimeLearned = cacheEntry.LastTimeLearned;

            ConnectionStringStruct css = entry.ConnectionString;
            css.ProtectedLm = cacheEntry.ConnectionString.ProtectedLm;
            entry.ConnectionString = css;

            if (cacheEntry.Logo != null)
                entry.Logo = cacheEntry.Logo;

            entry.IsFromCache = entry.IsVerified = true;
            return true;
        }

        /// <summary>
        /// Gets the cache item timestamp.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>The timestamp, null, in case not found.</returns>
        /// <remarks>Documented by Dev02, 2008-12-11</remarks>
        public DateTime? GetCacheItemTimestamp(LearningModulesIndexEntry entry)
        {
            string key = GetEntryKey(entry);
            if (!indexCacheTimestamps.ContainsKey(key))
                return null;

            return indexCacheTimestamps[key];
        }

        private static string GetEntryKey(LearningModulesIndexEntry entry)
        {
            if (!entry.IsAccessible && entry.NotAccessibleReason == LearningModuleNotAccessibleReason.Protected)
                return entry.ConnectionString.ConnectionString + "_PROTECTED";
            else
                return entry.ConnectionString.ConnectionString + entry.ConnectionString.LmId.ToString() + entry.User.Id.ToString();
        }

        /// <summary>
        /// Cleans up old cache entries.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-12-11</remarks>
        private void CleanupOldCacheEntries()
        {
            List<string> cleanUpConnectionStrings = new List<string>();

            foreach (KeyValuePair<string, DateTime> value in indexCacheTimestamps)
            {
                if (value.Value.AddDays(Properties.Settings.Default.LMIndexCacheCleanupDays) < DateTime.Now)
                    cleanUpConnectionStrings.Add(value.Key);
            }

            foreach (string key in cleanUpConnectionStrings)
            {
                if (indexCacheTimestamps.ContainsKey(key))
                    indexCacheTimestamps.Remove(key);

                if (indexCache.ContainsKey(key))
                    indexCache.Remove(key);
            }
        }
        #endregion

        #region Serialize & File functions
        /// <summary>
        /// Serializes the index cache.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private void SerializeIndexCache(Stream stream)
        {
            //cache for passwords - they must not be saved in the disk cache
            Dictionary<string, string> passwords = new Dictionary<string, string>();

            foreach (KeyValuePair<string, LearningModulesIndexEntry> pair in indexCache)
            {
                if (pair.Value.ConnectionString.Password != string.Empty)
                {
                    passwords.Add(pair.Key, pair.Value.ConnectionString.Password);

                    ConnectionStringStruct css = pair.Value.ConnectionString;
                    css.Password = string.Empty;
                    pair.Value.ConnectionString = css;
                }
            }

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, indexCacheTimestamps);
            formatter.Serialize(stream, indexCache);

            foreach (KeyValuePair<string, string> pair in passwords)
            {
                LearningModulesIndexEntry entry = indexCache[pair.Key];

                ConnectionStringStruct css = entry.ConnectionString;
                css.Password = pair.Value;
                entry.ConnectionString = css;
            }
        }

        /// <summary>
        /// Deserializes the index cache.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private void DeserializeIndexCache(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            indexCacheTimestamps = (Dictionary<string, DateTime>)formatter.Deserialize(stream);
            indexCache = (Dictionary<string, LearningModulesIndexEntry>)formatter.Deserialize(stream);
            CleanupOldCacheEntries();
        }

        /// <summary>
        /// Dumps the index cache to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public void DumpIndexCache(string filename)
        {
            if (indexCache.Count > 0)
            {
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(filename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(filename));

                    using (Stream stream = new FileStream(filename, FileMode.Create))
                        SerializeIndexCache(stream);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("LearningModulesIndex.DumpIndexCache() - " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Restores the index cache from a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        internal void RestoreIndexCache(string filename)
        {
            try
            {
                using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    DeserializeIndexCache(stream);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("LearningModulesIndex.DumpIndexCache() - " + ex.Message);
            }
        }
        #endregion
    }
}

