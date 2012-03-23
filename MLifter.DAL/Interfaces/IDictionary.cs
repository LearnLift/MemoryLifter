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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Security;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Interface which defines a dictionary.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    public interface IDictionary : IDisposable, ICopy, IParent, ISecurity
    {
        /// <summary>
        /// Gets a value indicating whether this instance is DB.
        /// </summary>
        /// <value><c>true</c> if this instance is DB; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        bool IsDB { get; }

        /// <summary>
        /// Gets or sets the background worker.
        /// </summary>
        /// <value>The background worker.</value>
        /// <remarks>Documented by Dev03, 2007-09-11</remarks>
        BackgroundWorker BackgroundWorker { get; set; }

        /// <summary>
        /// Occurs when [XML progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        event StatusMessageEventHandler XmlProgressChanged;
        /// <summary>
        /// Occurs when [move progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        event StatusMessageEventHandler MoveProgressChanged;
        /// <summary>
        /// Occurs when [save as progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        event StatusMessageEventHandler SaveAsProgressChanged;
        /// <summary>
        /// Occurs when [create media progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        event StatusMessageEventHandler CreateMediaProgressChanged;

        /// <summary>
        /// Gets or sets the default settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2008-08-11</remarks>
        ISettings DefaultSettings { get; set; }
        /// <summary>
        /// Gets or sets the allowed settings.
        /// </summary>
        /// <value>The allowed settings.</value>
        /// <remarks>Documented by Dev05, 2008-09-22</remarks>
        ISettings AllowedSettings { get; set; }
        /// <summary>
        /// Gets or sets the user settings.
        /// </summary>
        /// <value>The user settings.</value>
        /// <remarks>Documented by Dev05, 2008-10-01</remarks>
        ISettings UserSettings { get; set; }

		/// <summary>
		/// Creates a new settings object.
		/// </summary>
		/// <returns></returns>
        ISettings CreateSettings();

        /// <summary>
        /// Defines whether the content of the LM is protected from being copied/extracted.
        /// </summary>
        bool ContentProtected { get; }
        /// <summary>
        /// Gets the connection string (could conatain a path or a db connection string).
        /// </summary>
        /// <value>The connection string.</value>
        /// <remarks>Documented by Dev03, 2007-10-17</remarks>
        string Connection { get; }
        /// <summary>
        /// Gets the dictionary as Xml.
        /// </summary>
        /// <value>The Xml.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        string Xml { get; }
        /// <summary>
        /// Gets the number of boxes.
        /// </summary>
        /// <value>The number of boxes.</value>
        /// <remarks>Documented by Dev03, 2007-10-17</remarks>
        int NumberOfBoxes { get; }
        // General settings 
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        int Version { get; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        [ValueCopy]
        string Title { get; set; }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        string Author { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        string Description { get; set; }
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        int Id { get; }
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        [ValueCopy]
        string Guid { get; set; }
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        Category Category { get; set; }
        /// <summary>
        /// Gets or sets the media directory.
        /// </summary>
        /// <value>The media directory.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        string MediaDirectory { get; set; }

        /// <summary>
        /// Gets the size of the dictionary.
        /// </summary>
        /// <value>The size of the dictionary.</value>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        long DictionarySize { get; }

        /// <summary>
        /// Gets the number of all dictionary media objects/files.
        /// </summary>
        /// <value>The dictionary media objects count.</value>
        /// <remarks>Documented by Dev08, 2008-10-02</remarks>
        int DictionaryMediaObjectsCount { get; }

        /// <summary>
        /// Gets actual the score.
        /// </summary>
        /// <value>The score.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        double Score { get; }
        /// <summary>
        /// Gets or sets the high score.
        /// </summary>
        /// <value>The high score.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        [ValueCopy]
        double HighScore { get; set; }
        /// <summary>
        /// Gets the boxes.
        /// </summary>
        /// <value>The boxes.</value>
        /// <remarks>Documented by Dev03, 2007-11-22</remarks>
        IBoxes Boxes { get; }

        // Content
        /// <summary>
        /// Gets or sets the cards.
        /// </summary>
        /// <value>The cards.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        ICards Cards { get; }
        /// <summary>
        /// Gets or sets the chapters.
        /// </summary>
        /// <value>The chapters.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IChapters Chapters { get; }
        /// <summary>
        /// Gets or sets the statistics.
        /// </summary>
        /// <value>The statistics.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        IStatistics Statistics { get; }

        // Methods
        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void Load();
        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void Save();
        /// <summary>
        /// Saves the dictionary to the new path.
        /// </summary>
        /// <param name="newPath">The new path.</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void SaveAs(string newPath);
        /// <summary>
        /// Saves the dictionary to the new path.
        /// </summary>
        /// <param name="newPath">The new path.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void SaveAs(string newPath, bool overwrite);
        /// <summary>
        /// Moves the specified new path.
        /// </summary>
        /// <param name="newPath">The new path.</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void Move(string newPath);
        /// <summary>
        /// Moves the specified new path.
        /// </summary>
        /// <param name="newPath">The new path.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite] existing files.</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void Move(string newPath, bool overwrite);
        /// <summary>
        /// Changes the media path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="move">if set to <c>true</c> [move].</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        void ChangeMediaPath(string path, bool move);
        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        List<string> GetResources();
        /// <summary>
        /// Gets the empty resources.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-31</remarks>
        List<int> GetEmptyResources();
        /// <summary>
        /// Creates a new instance of a card style object.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-10-30</remarks>
        ICardStyle CreateCardStyle();
        /// <summary>
        /// Creates a new media object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="path">The path.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="isExample">if set to <c>true</c> [is example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-11</remarks>
        IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample);
        /// <summary>
        /// Resets the learning progress.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-08</remarks>
        IDictionary ResetLearningProgress();
        /// <summary>
        /// Checks the user session.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-11-18</remarks>
        bool CheckUserSession();
        /// <summary>
        /// Occurs when [backup completed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-08</remarks>
        event BackupCompletedEventHandler BackupCompleted;
        /// <summary>
        /// Preloads the card cache.
        /// </summary>
        /// <remarks>Documented by Dev09, 2009-04-28</remarks>
        void PreloadCardCache();
        /// <summary>
        /// Clears the unused media.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-05-27</remarks>
        void ClearUnusedMedia();
        /// <summary>
        /// Gets the LearningModule extensions.
        /// </summary>
        /// <value>The extensions.</value>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        IList<IExtension> Extensions { get; }
        /// <summary>
        /// Creates a new extension.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        IExtension ExtensionFactory();
        /// <summary>
        /// Creates new extensions.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        IExtension ExtensionFactory(Guid guid);
    }

    /// <summary>
    /// Enumerator for the possible commentary (teacher) sounds.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-05</remarks>
    public enum ETeacherSounds
    {
        /// <summary>
        /// Stand-alone sound file for the answer side if the answer was correct (ex.: "This was correct, good Job!").
        /// </summary>
        AnswerRightStandAlone = 0,
        /// <summary>
        /// Stand-alone sound file for the answer side if the answer was wrong (ex.: "I'm sorry, that's wrong!").
        /// </summary>
        AnswerWrongStandAlone,
        /// <summary>
        /// Stand-alone sound file for the answer side if the answer was almost correct (ex.: "You almost had it!").
        /// </summary>
        AnswerAlmostStandAlone,
        /// <summary>
        /// Partial sound file for the answer side if the answer was correct (ex.: "[...] is correct!").
        /// </summary>
        AnswerRight,
        /// <summary>
        /// Partial sound file for the answer side if the answer was wrong (ex.: "I'm sorry, the correct answer is [...].").
        /// </summary>
        AnswerWrong,
        /// <summary>
        /// Partial sound file for the answer side if the answer was almost correct (ex.: "You almost had it! The correct answer is [...].").
        /// </summary>
        AnswerAlmost,
        /// <summary>
        /// Stand-alone sound file for the question side if the answer was correct (ex.: "This was correct, good Job!").
        /// </summary>
        QuestionRightStandAlone,
        /// <summary>
        /// Stand-alone sound file for the question side if the answer was wrong (ex.: "I'm sorry, that's wrong!").
        /// </summary>
        QuestionWrongStandAlone,
        /// <summary>
        /// Stand-alone sound file for the question side if the answer was almost correct (ex.: "You almost had it!").
        /// </summary>
        QuestionAlmostStandAlone,
        /// <summary>
        /// Partial sound file for the question side if the answer was correct (ex.: "[...] is correct!").
        /// </summary>
        QuestionRight,
        /// <summary>
        /// Partial sound file for the question side if the answer was wrong (ex.: "I'm sorry, the correct answer is [...].").
        /// </summary>
        QuestionWrong,
        /// <summary>
        /// Partial sound file for the question side if the answer was almost correct (ex.: "You almost had it! The correct answer is [...].").
        /// </summary>
        QuestionAlmost,
    }
}
