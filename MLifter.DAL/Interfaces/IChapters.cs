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
using MLifter.DAL.Security;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Interface which defines a chapter.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public interface IChapters : ICopy, IParent, ISecurity
    {
        /// <summary>
        /// Gets the chapters.
        /// </summary>
        /// <value>The chapters.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        IList<IChapter> Chapters { get; }
        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        IChapter AddNew();
        /// <summary>
        /// Deletes the specified chapter_id.
        /// </summary>
        /// <param name="chapter_id">The chapter_id.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        void Delete(int chapter_id);
        /// <summary>
        /// Finds the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        IChapter Find(string title);

        /// <summary>
        /// Gets the specified chapter_id.
        /// </summary>
        /// <param name="chapter_id">The chapter_id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        IChapter Get(int chapter_id);

        //void SwapId(int first_id, int second_id);

        /// <summary>
        /// Moves the specified first_id.
        /// </summary>
        /// <param name="first_id">The first_id.</param>
        /// <param name="second_id">The second_id.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        void Move(int first_id, int second_id);
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        int Count { get; }
    }

    /// <summary>
    /// Helper class to copy objects of the type IChapters.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public static class ChaptersHelper
    {
		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
        public static void Copy(IChapters source, IChapters target, CopyToProgress progressDelegate)
        {
            if (target.Parent.GetParentDictionary().Parent.Properties.ContainsKey(ParentProperty.ChapterMappings))
                return;

            Dictionary<int, int> chaperMappings = new Dictionary<int, int>();
            foreach (IChapter chapter in source.Chapters)
            {
                IChapter newChapter = target.AddNew();
                chapter.CopyTo(newChapter, progressDelegate);
                chaperMappings.Add(chapter.Id, newChapter.Id);

                if (source.Parent.GetParentDictionary().DefaultSettings.SelectedLearnChapters.Contains(chapter.Id))
                    target.Parent.GetParentDictionary().DefaultSettings.SelectedLearnChapters.Add(newChapter.Id);
                if (source.Parent.GetParentDictionary().UserSettings.SelectedLearnChapters.Contains(chapter.Id))
                    target.Parent.GetParentDictionary().UserSettings.SelectedLearnChapters.Add(newChapter.Id);
            }

            target.Parent.GetParentDictionary().Parent.Properties[ParentProperty.ChapterMappings] = chaperMappings;
        }
    }
}
