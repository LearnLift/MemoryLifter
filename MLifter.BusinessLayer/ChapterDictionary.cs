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

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;

namespace MLifter.BusinessLayer
{
    public class ChapterDictionary
    {
        private Dictionary dictionary;
        private IChapters chapters;

        public IList<IChapter> Chapters 
        {
            get { return chapters.Chapters; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChapterDictionary"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public ChapterDictionary(Dictionary dict, IChapters Chapters)
        {
            dictionary = dict;
            this.chapters = Chapters;
        }

        /// <summary>
        /// Gets the chapter with the given ID.
        /// </summary>
        /// <param name="chapterID">The chapter ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public IChapter GetChapterByID(int chapterID)
        {
            return chapters.Get(chapterID);
        }
        /// <summary>
        /// Deletes the chapter with the given ID.
        /// </summary>
        /// <param name="chapterID">The chapter ID.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void DeleteChapterByID(int chapterID)
        {
            chapters.Delete(chapterID);
        }
        /// <summary>
        /// Gets the size of the chapter.
        /// </summary>
        /// <param name="chapterID">The chapter ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public int GetChapterSize(int chapterID)
        {
            return chapters.Get(chapterID).Size;
        }

        /// <summary>
        /// Gets the chapter names of all chapters.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string[] GetChapterNames()
        {
            List<string> chapterNames = new List<string>();
            foreach (IChapter chapter in chapters.Chapters)
                chapterNames.Add(chapter.Title);

            return chapterNames.ToArray();
        }
        /// <summary>
        /// Moves the chapter.
        /// </summary>
        /// <param name="chapterID">The chapter ID.</param>
        /// <param name="IDofNewPosition">The Id of the chapter which the position where the chapter should be moved to.</param>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public void MoveChapter(int chapterID, int IDofNewPosition)
        {
            chapters.Move(chapterID, IDofNewPosition);
        }

        /// <summary>
        /// Adds a chapter to the dictionary.
        /// </summary>
        /// <param name="title">Chapter title</param>
        /// <param name="Description">Chapter Description</param>
        /// <returns>ID of the newly created chapter or -1 to indicate an error</returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public int AddChapter(string title, string description)
        {
            IChapter chapter = chapters.AddNew();
            chapter.Title = title;
            chapter.Description = description;

            return chapter.Id;
        }
    }
}
