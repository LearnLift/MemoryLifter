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
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MLifter.DAL.Interfaces;
using System.ComponentModel;

namespace MLifterSettingsManager
{
    public class ChapterTreeViewItem : GeneralTreeViewItem
    {
        #region Private Fields

        private IChapter chapter;
        private bool hasCustomSettings = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChapterTreeViewItem"/> class.
        /// </summary>
        /// <param name="chapter">The chapter.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ChapterTreeViewItem(IChapter chapter, LearningModuleTreeViewItem parent)
            : base(parent)
        {
            this.chapter = chapter;
            hasCustomSettings = chapter.Settings != null && !SettingsManagerBusinessLogic.IsEmptySetting(chapter.Settings) ? true : false;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the cards.
        /// </summary>
        /// <value>The cards.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ObservableCollection<CardTreeViewItem> Cards { get; set; }

        /// <summary>
        /// Gets the chapter.
        /// </summary>
        /// <value>The chapter.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public IChapter Chapter
        {
            get
            {
                return chapter;
            }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string Title
        {
            get
            {
                return chapter.Title;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string Description
        {
            get
            {
                return chapter.Description;
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public int Id
        {
            get
            {
                return chapter.Id;
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ISettings Settings
        {
            get
            {
                return chapter.Settings;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom settings; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public bool HasCustomSettings
        {
            get
            {
                return hasCustomSettings;
            }
            set
            {
                hasCustomSettings = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs("HasCustomSettings"));
            }
        }

        #endregion
    }
}
