using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MLifter.BusinessLayer
{
    public partial class LearningModulesIndex
    {
        /// <summary>
        /// Serializes the index.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private void SerializeIndex(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream,
                this.LearningModules.FindAll(delegate(LearningModulesIndexEntry entry)
                    { return entry.IsVerified; }));
        }

        /// <summary>
        /// Deserializes the index.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        private void DeserializeIndex(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            List<LearningModulesIndexEntry> list = (List<LearningModulesIndexEntry>)formatter.Deserialize(stream);
            this.learningModules.Clear();
            this.learningModules.AddRange(list);
        }

        /// <summary>
        /// Dumps the index to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public void DumpIndex(string filename)
        {
            Stream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.Create);
                SerializeIndex(stream);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Restores the index from a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev02, 2008-12-04</remarks>
        public void RestoreIndex(string filename)
        {
            Stream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                DeserializeIndex(stream);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
    }
}

