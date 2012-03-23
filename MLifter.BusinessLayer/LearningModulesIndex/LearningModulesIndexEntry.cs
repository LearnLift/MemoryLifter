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
using System.Drawing;
using System.Windows.Forms;
using MLifter.DAL;
using MLifter.DAL.Interfaces;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// This class hold all needed information about a learning module.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-18</remarks>
    [Serializable()]
    public class LearningModulesIndexEntry : IIndexEntry
    {
        [NonSerialized()]
        private IUser user;
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public IUser User
        {
            get { return user; }
            set
            {
                user = value;
                if (user != null)
                {
                    UserId = user.Id;
                    UserName = user.UserName;
                }
                else
                {
                    UserId = -1;
                    UserName = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        /// <remarks>Documented by Dev05, 2009-04-29</remarks>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks>Documented by Dev05, 2009-04-28</remarks>
        public string UserName { get; set; }

        [NonSerialized()]
        private IDictionary dictionary;
        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public IDictionary Dictionary
        {
            get { return dictionary; }
            set { dictionary = value; }
        }

        private LearningModuleType type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public LearningModuleType Type
        {
            get { return type; }
            set { type = value; }
        }

        [NonSerialized()]
        private ListViewGroup group;
        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public ListViewGroup Group
        {
            get { return group; }
            set { group = value; }
        }

        private ConnectionStringStruct connectionString;
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public ConnectionStringStruct ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        private string description = string.Empty;
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The Description.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private Category category;
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public Category Category
        {
            get { return category; }
            set { category = value; }
        }

        private string author = string.Empty;
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        /// <remarks>Documented by Dev05, 2009-03-26</remarks>
        public long Size { get; set; }

        private DateTime lastTimeLearned;
        /// <summary>
        /// Gets or sets the last time learned.
        /// </summary>
        /// <value>The last time learned.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public DateTime LastTimeLearned
        {
            get { return lastTimeLearned; }
            set { lastTimeLearned = value; }
        }

        private LearningModuleStatistics statistics = null;
        /// <summary>
        /// Gets or sets the statistics.
        /// </summary>
        /// <value>The statistics.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public LearningModuleStatistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }

        private LearningModulePreview preview = null;
        /// <summary>
        /// Gets or sets the preview.
        /// </summary>
        /// <value>The preview.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public LearningModulePreview Preview
        {
            get { return preview; }
            set { preview = value; }
        }

        /// <summary>
        /// Gets or sets the synced path.
        /// </summary>
        /// <value>The synced path.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public string SyncedPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the connection.
        /// </summary>
        /// <value>The name of the connection.</value>
        /// <remarks>Documented by Dev05, 2009-03-02</remarks>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this learning module is accessible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this learning module is accessible; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public bool IsAccessible { get; set; }
        /// <summary>
        /// Gets or sets the not accessible reason.
        /// </summary>
        /// <value>The not accessible reason.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public LearningModuleNotAccessibleReason NotAccessibleReason { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModulesIndexEntry"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public LearningModulesIndexEntry()
        {
            DisplayName = string.Empty;
            IsAccessible = true;
            IsVerified = false;
            IsFromCache = false;
            Description = string.Empty;
            Count = -1;
            Author = string.Empty;
            Statistics = null;
            Preview = null;
            IsAccessible = true;
            NotAccessibleReason = LearningModuleNotAccessibleReason.IsAccessible;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModulesIndexEntry"/> class.
        /// </summary>
        /// <param name="connectionStringStruct">The connection string struct.</param>
        /// <remarks>Documented by Dev05, 2009-03-02</remarks>
        public LearningModulesIndexEntry(ConnectionStringStruct connectionStringStruct)
            : this()
        {
            ConnectionString = connectionStringStruct;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public override string ToString()
        {
            return DisplayName + " (" + Author + ") - Cards: " + Count.ToString() + " - Protected: " + ConnectionString.ProtectedLm.ToString();
        }

        /// <summary>
        /// Determines whether this entry contains the specified filter words.
        /// </summary>
        /// <param name="filterWords">The filter words.</param>
        /// <returns>
        /// 	<c>true</c> if contains the specified filter words; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2008-12-10</remarks>
        public bool Contains(string[] filterWords)
        {
            foreach (string word in filterWords)
                if (!Contains(word))
                    return false;
            return true;
        }
        /// <summary>
        /// Determines whether this entry contains the specified filter word.
        /// </summary>
        /// <param name="filterWord">The filter word.</param>
        /// <returns>
        /// 	<c>true</c> if contains the specified filter word; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2008-12-10</remarks>
        public bool Contains(string filterWord)
        {
            return DisplayName.ToLower().Contains(filterWord) || IsVerified && IsAccessible && (Author.ToLower().Contains(filterWord) ||
                                Category.ToString().ToLower().Contains(filterWord) || Description.ToLower().Contains(filterWord) ||
                                Group.Header.ToLower().Contains(filterWord) || LastTimeLearned.ToLongTimeString().ToLower().Contains(filterWord));
        }

        /// <summary>
        /// Occurs when IsVerified changed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-07</remarks>
        [field: NonSerialized]
        public event EventHandler IsVerifiedChanged;
        /// <summary>
        /// Raises the <see cref="E:IsVerifiedChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-07</remarks>
        protected virtual void OnIsVerifiedChanged(EventArgs e)
        {
            if (IsVerifiedChanged != null)
                IsVerifiedChanged(this, e);
        }

        #region IIndexEntry Members

        private string displayName = string.Empty;
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        private bool isVerified = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is verified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public bool IsVerified
        {
            get { return isVerified; }
            set { isVerified = value; OnIsVerifiedChanged(EventArgs.Empty); }
        }

        private bool isFromCache = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is from cache.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is from cache; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public bool IsFromCache
        {
            get { return isFromCache; }
            set { isFromCache = value; }
        }

        private int count = -1;
        /// <summary>
        /// Gets or sets the card count.
        /// </summary>
        /// <value>The card count.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        private Image logo;
        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public Image Logo
        {
            get { return logo; }
            set { logo = value; }
        }

        private IConnectionString connection;
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public IConnectionString Connection { get { return connection; } set { connection = value; ConnectionName = value.Name; } }

        #endregion
    }

    /// <summary>
    /// The last statistics of a learning module.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-18</remarks>
    [Serializable()]
    public class LearningModuleStatistics
    {
        /// <summary>
        /// Gets or sets the last session time.
        /// </summary>
        /// <value>The last session time.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public TimeSpan LastSessionTime { get; set; }
        /// <summary>
        /// Gets or sets the last start time.
        /// </summary>
        /// <value>The last start time.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public DateTime LastStartTime { get; set; }
        /// <summary>
        /// Gets or sets the last end time.
        /// </summary>
        /// <value>The last end time.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public DateTime LastEndTime { get; set; }
        /// <summary>
        /// Gets or sets the cards asked count.
        /// </summary>
        /// <value>The cards asked.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public int CardsAsked { get; set; }
        /// <summary>
        /// Gets or sets the right card count.
        /// </summary>
        /// <value>The right.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public int Right { get; set; }
        /// <summary>
        /// Gets or sets the wrong card count.
        /// </summary>
        /// <value>The wrong.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public int Wrong { get; set; }

        /// <summary>
        /// Gets the ratio.
        /// </summary>
        /// <value>The ratio.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public double Ratio { get { return Right * 100.0 / CardsAsked; } }
        /// <summary>
        /// Gets the time per card.
        /// </summary>
        /// <value>The time per card.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public TimeSpan TimePerCard { get { try { return new TimeSpan(LastSessionTime.Ticks / CardsAsked); } catch (DivideByZeroException) { return new TimeSpan(); } } }
        /// <summary>
        /// Gets the cards per minute.
        /// </summary>
        /// <value>The cards per minute.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public double CardsPerMinute { get { try { return 60 / TimePerCard.TotalSeconds; } catch (DivideByZeroException) { return 0; } } }
    }

    /// <summary>
    /// The preview-data of a learning module.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-18</remarks>
    [Serializable()]
    public class LearningModulePreview
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The Description.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the preview image.
        /// </summary>
        /// <value>The preview image.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public Image PreviewImage { get; set; }
    }
}
