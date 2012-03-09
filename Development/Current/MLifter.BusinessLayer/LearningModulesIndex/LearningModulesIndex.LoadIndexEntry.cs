using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Preview;

namespace MLifter.BusinessLayer
{
    public partial class LearningModulesIndex
    {
        /// <summary>
        /// Loads the detailed information for an index entry.
        /// </summary>
        /// <param name="entry">The learning module entry.</param>
        /// <remarks>Documented by Dev03, 2008-12-03</remarks>
        private static void LoadIndexEntry(LearningModulesIndexEntry entry)
        {
            try
            {
                if (entry.Dictionary is PreviewDictionary)
                    entry.Dictionary = DAL.User.UpdatePreviewDictionary(entry.Dictionary as PreviewDictionary);
                entry.DisplayName = entry.Dictionary.Title;
                entry.Description = entry.Dictionary.Description;
                entry.Author = entry.Dictionary.Author;
                entry.Category = entry.Dictionary.Category;
                entry.Count = entry.Dictionary.Cards.Count;
                entry.Size = entry.Dictionary.DictionarySize;
                Settings settings = new Settings(entry.Dictionary);
                IMedia logo = settings.Logo as IImage;
                if (logo != null)
                    entry.Logo = (Image)Bitmap.FromStream(logo.Stream).Clone();
                if (entry.Dictionary.Statistics.Count > 0)
                    entry.LastTimeLearned = entry.Dictionary.Statistics[entry.Dictionary.Statistics.Count - 1].StartTimestamp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadIndexEntry - " + ex.Message);
            }
        }
    }
}
