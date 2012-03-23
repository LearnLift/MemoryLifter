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
