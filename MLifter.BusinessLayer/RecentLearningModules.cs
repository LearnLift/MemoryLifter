using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MLifter.DAL.Interfaces;
using System.Diagnostics;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Maintains a list of the recently opened learning modules.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-02-26</remarks>
    /// <remarks>Documented by Dev08, 2009-03-04</remarks>
    public static class RecentLearningModules
    {
        private static SortedList<DateTime, LearningModulesIndexEntry> recentModules = new SortedList<DateTime, LearningModulesIndexEntry>();

        /// <summary>
        /// Gets or sets the recent files list maximum.
        /// </summary>
        /// <value>The recent files list maximum.</value>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        public static int RecentFilesListMaximum
        {
            get
            {
                return Properties.Settings.Default.RecentFilesListMaximum;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentLearningModules"/> class.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-03-03</remarks>
        //public RecentLearningModules()
        //{
        //    //this.RecentFilesListMaximum = Properties.Settings.Default.RecentFilesListMaximum;
        //}

        #region EventHandler
        /// <summary>
        /// Occurs when [list changed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public static event EventHandler ListChanged;

        /// <summary>
        /// Raises the <see cref="E:ListChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        private static void OnListChanged(EventArgs e)
        {
            if (ListChanged != null)
                ListChanged(null, e);
        }
        #endregion

        #region List functions
        /// <summary>
        /// Adds the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public static void Add(LearningModulesIndexEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");
            List<DateTime> deleteKeys = new List<DateTime>();

            //delete old duplicates
            foreach (KeyValuePair<DateTime, LearningModulesIndexEntry> pair in recentModules)
            {
                if (pair.Value.ConnectionString.ConnectionString == entry.ConnectionString.ConnectionString &&
                    pair.Value.ConnectionString.LmId == entry.ConnectionString.LmId)
                {
                    deleteKeys.Add(pair.Key);
                }
            }

            foreach (DateTime key in deleteKeys)
                recentModules.Remove(key);

            while (recentModules.ContainsKey(DateTime.Now))
                System.Threading.Thread.Sleep(5);

            recentModules.Add(DateTime.Now, entry);
            OnListChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.Generic.List`1"></see>.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public static void Clear()
        {
            recentModules.Clear();
            OnListChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Cleans up the list.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        private static void CleanupList()
        {
            //truncate too old entries
            while (recentModules.Count > RecentFilesListMaximum)
                recentModules.RemoveAt(0);

            //...clear non-existant entries here
        }
        #endregion

        #region Output functions
        /// <summary>
        /// Gets the most recent learning module.
        /// </summary>
        /// <value>The most recent learning module.</value>
        /// <remarks>Documented by Dev02, 2008-04-29</remarks>
        public static LearningModulesIndexEntry MostRecentLearningModule
        {
            get
            {
                if (recentModules.Keys.Count <= 0)
                    return null;

                return recentModules.Values[recentModules.Count - 1];
            }
        }

        /// <summary>
        /// Gets the recent modules list, sorted with newest on top.
        /// </summary>
        /// <value>The recent modules list.</value>
        /// <remarks>Documented by Dev02, 2009-02-26</remarks>
        public static List<LearningModulesIndexEntry> GetRecentModules()
        {
            List<LearningModulesIndexEntry> list = new List<LearningModulesIndexEntry>();
            CleanupList();

            //go through all items in reverse order (newest items at top).
            for (int i = 1; i <= recentModules.Count; i++)
            {
                LearningModulesIndexEntry entry = recentModules.Values[recentModules.Count - i];
                list.Add(entry);
            }
            return list;
        }
        #endregion

        #region Serialize & File functions
        /// <summary>
        /// Serializes to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private static void Serialize(Stream stream)
        {
            //cache for passwords - they must not be saved on disk
            Dictionary<DateTime, string> passwords = new Dictionary<DateTime, string>();

            foreach (KeyValuePair<DateTime, LearningModulesIndexEntry> pair in recentModules)
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
            formatter.Serialize(stream, recentModules);

            foreach (KeyValuePair<DateTime, string> pair in passwords)
            {
                LearningModulesIndexEntry entry = recentModules[pair.Key];

                ConnectionStringStruct css = entry.ConnectionString;
                css.Password = pair.Value;
                entry.ConnectionString = css;
            }
        }

        /// <summary>
        /// Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private static void Deserialize(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            recentModules = (SortedList<DateTime, LearningModulesIndexEntry>)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Dumps to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public static void Dump(string filename)
        {
            if (recentModules.Count > 0)
            {
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(filename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(filename));

                    using (Stream stream = new FileStream(filename, FileMode.Create))
                        Serialize(stream);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("RecentLearningModules.Dump() - " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Restores from a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public static void Restore(string filename)
        {
            recentModules.Clear();

            try
            {
                if (!File.Exists(filename))
                    return;

                using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    Deserialize(stream);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("RecentLearningModules.Restore() - " + ex.Message);
                try { File.Delete(filename); }
                catch { }
            }
        }
        #endregion
    }
}
