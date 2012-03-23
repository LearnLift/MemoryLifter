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
using MLifter.DAL.Interfaces;
using MLifter.DAL.DB;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Business layer implementation of ISettings.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    class Settings : ISettings
    {
        private IDictionary dictionary;

        private ISettings defaultSettings
        {
            get
            {
                return dictionary.DefaultSettings;
            }
        }

        private ISettings userSettings
        {
            get
            {
                return dictionary.UserSettings;
            }
        }


        internal Settings(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        #region ISettings Members

        public IQueryDirections QueryDirections
        {
            get
            {
                return new QueryDirections(defaultSettings.QueryDirections, userSettings.QueryDirections);
            }
            set
            {
                userSettings.QueryDirections = value;
            }
        }

        public IQueryType QueryTypes
        {
            get
            {
                return new QueryType(defaultSettings.QueryTypes, userSettings.QueryTypes);
            }
            set
            {
                userSettings.QueryTypes = value;
            }
        }

        public IQueryMultipleChoiceOptions MultipleChoiceOptions
        {
            get
            {
                return new QueryMultipleChoiceOptions(defaultSettings.MultipleChoiceOptions, userSettings.MultipleChoiceOptions);
            }
            set
            {
                userSettings.MultipleChoiceOptions = value;
            }
        }

        public IGradeTyping GradeTyping
        {
            get
            {
                return new GradeTyping(defaultSettings.GradeTyping, userSettings.GradeTyping);
            }
            set
            {
                userSettings.GradeTyping = value;
            }
        }

        public IGradeSynonyms GradeSynonyms
        {
            get
            {
                return new GradeSynonyms(defaultSettings.GradeSynonyms, userSettings.GradeSynonyms);
            }
            set
            {
                userSettings.GradeSynonyms = value;
            }
        }

        public ICardStyle Style
        {
            get
            {
                return defaultSettings.Style;
            }
            set
            {
                defaultSettings.Style = value;
            }
        }

        public CompiledTransform? QuestionStylesheet
        {
            get
            {
                return userSettings.QuestionStylesheet ?? defaultSettings.QuestionStylesheet;
            }
            set
            {
                userSettings.QuestionStylesheet = value;
            }
        }

        public CompiledTransform? AnswerStylesheet
        {
            get
            {
                return userSettings.AnswerStylesheet ?? defaultSettings.AnswerStylesheet;
            }
            set
            {
                userSettings.AnswerStylesheet = value;
            }
        }

        public bool? AutoplayAudio
        {
            get
            {
                return userSettings.AutoplayAudio ?? defaultSettings.AutoplayAudio;
            }
            set
            {
                userSettings.AutoplayAudio = value;
            }
        }

        public bool? CaseSensitive
        {
            get
            {
                return userSettings.CaseSensitive ?? defaultSettings.CaseSensitive;
            }
            set
            {
                userSettings.CaseSensitive = value;
            }
        }

        public bool? ConfirmDemote
        {
            get
            {
                return userSettings.ConfirmDemote ?? defaultSettings.ConfirmDemote;
            }
            set
            {
                userSettings.ConfirmDemote = value;
            }
        }

        public bool? EnableCommentary
        {
            get
            {
                return userSettings.EnableCommentary ?? defaultSettings.EnableCommentary;
            }
            set
            {
                userSettings.EnableCommentary = value;
            }
        }

        public bool? CorrectOnTheFly
        {
            get
            {
                return userSettings.CorrectOnTheFly ?? defaultSettings.CorrectOnTheFly;
            }
            set
            {
                userSettings.CorrectOnTheFly = value;
            }
        }

        public bool? EnableTimer
        {
            get
            {
                return userSettings.EnableTimer ?? defaultSettings.EnableTimer;
            }
            set
            {
                userSettings.EnableTimer = value;
            }
        }

        public bool? RandomPool
        {
            get
            {
                return userSettings.RandomPool ?? defaultSettings.RandomPool;
            }
            set
            {
                userSettings.RandomPool = value;
            }
        }

        public bool? SelfAssessment
        {
            get
            {
                return userSettings.SelfAssessment ?? defaultSettings.SelfAssessment;
            }
            set
            {
                userSettings.SelfAssessment = value;
            }
        }

        public bool? ShowImages
        {
            get
            {
                return userSettings.ShowImages ?? defaultSettings.ShowImages;
            }
            set
            {
                userSettings.ShowImages = value;
            }
        }

        public bool? PoolEmptyMessageShown
        {
            get
            {
                return userSettings.PoolEmptyMessageShown ?? defaultSettings.PoolEmptyMessageShown;
            }
            set
            {
                userSettings.PoolEmptyMessageShown = value;
            }
        }

        public bool? UseLMStylesheets
        {
            get
            {
                return userSettings.UseLMStylesheets ?? defaultSettings.UseLMStylesheets;
            }
            set
            {
                userSettings.UseLMStylesheets = value;
            }
        }

        public bool? AutoBoxSize
        {
            get
            {
                return userSettings.AutoBoxSize ?? defaultSettings.AutoBoxSize;
            }
            set
            {
                userSettings.AutoBoxSize = value;
            }
        }

        public string StripChars
        {
            get
            {
                return userSettings.StripChars != string.Empty ? userSettings.StripChars : defaultSettings.StripChars;
            }
            set
            {
                userSettings.StripChars = value;
            }
        }

        public System.Globalization.CultureInfo QuestionCulture
        {
            get
            {
                return userSettings.QuestionCulture ?? defaultSettings.QuestionCulture;
            }
            set
            {
                userSettings.QuestionCulture = value;
            }
        }

        public System.Globalization.CultureInfo AnswerCulture
        {
            get
            {
                return userSettings.AnswerCulture ?? defaultSettings.AnswerCulture;
            }
            set
            {
                userSettings.AnswerCulture = value;
            }
        }

        public string QuestionCaption
        {
            get
            {
                return userSettings.QuestionCaption != string.Empty ? userSettings.QuestionCaption : defaultSettings.QuestionCaption;
            }
            set
            {
                userSettings.QuestionCaption = value;
            }
        }

        public string AnswerCaption
        {
            get
            {
                return userSettings.AnswerCaption != string.Empty ? userSettings.AnswerCaption : defaultSettings.AnswerCaption;
            }
            set
            {
                userSettings.AnswerCaption = value;
            }
        }

        public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds
        {
            get
            {
                return defaultSettings.CommentarySounds;
            }
            set
            {
                defaultSettings.CommentarySounds = value;
            }
        }

        public IMedia Logo
        {
            get
            {
                return userSettings.Logo ?? defaultSettings.Logo;
            }
            set
            {
                userSettings.Logo = value;
            }
        }

        public bool? ShowStatistics
        {
            get
            {
                return userSettings.ShowStatistics ?? defaultSettings.ShowStatistics;
            }
            set
            {
                userSettings.ShowStatistics = value;
            }
        }

        public bool? SkipCorrectAnswers
        {
            get
            {
                return userSettings.SkipCorrectAnswers ?? defaultSettings.SkipCorrectAnswers;
            }
            set
            {
                userSettings.SkipCorrectAnswers = value;
            }
        }

        public ISnoozeOptions SnoozeOptions
        {
            get
            {
                return new SnoozeOptions(defaultSettings.SnoozeOptions, userSettings.SnoozeOptions);
            }
            set
            {
                userSettings.SnoozeOptions = value;
            }
        }

        public IList<int> SelectedLearnChapters
        {
            get
            {
                return userSettings.SelectedLearnChapters ?? defaultSettings.SelectedLearnChapters;
            }
            set
            {
                userSettings.SelectedLearnChapters = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region ISecurity Members

        public bool HasPermission(string permissionName)
        {
            throw new NotImplementedException();
        }

        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class QueryDirections : IQueryDirections
    {
        private IQueryDirections defaultSettings;
        private IQueryDirections userSettings;

        public QueryDirections(IQueryDirections defaultSettings, IQueryDirections userSettings)
        {
            this.defaultSettings = defaultSettings;
            this.userSettings = userSettings;
        }

        public override bool Equals(object obj)
        {
            return QueryDirectionsHelper.Compare(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryDirections Members

        public bool? Question2Answer
        {
            get
            {
                return userSettings.Question2Answer ?? defaultSettings.Question2Answer;
            }
            set
            {
                userSettings.Question2Answer = value;
            }
        }

        public bool? Answer2Question
        {
            get
            {
                return userSettings.Answer2Question ?? defaultSettings.Answer2Question;
            }
            set
            {
                userSettings.Answer2Question = value;
            }
        }

        public bool? Mixed
        {
            get
            {
                return userSettings.Mixed ?? defaultSettings.Mixed;
            }
            set
            {
                userSettings.Mixed = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    class QueryType : IQueryType
    {
        private IQueryType defaultSettings;
        private IQueryType userSettings;

        public QueryType(IQueryType defaultSettings, IQueryType userSettings)
        {
            this.defaultSettings = defaultSettings;
            this.userSettings = userSettings;
        }

        public override bool Equals(object obj)
        {
            return QueryTypeHelper.Compare(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryType Members

        public bool? ImageRecognition
        {
            get
            {
                return userSettings.ImageRecognition ?? defaultSettings.ImageRecognition;
            }
            set
            {
                userSettings.ImageRecognition = value;
            }
        }

        public bool? ListeningComprehension
        {
            get
            {
                return userSettings.ListeningComprehension ?? defaultSettings.ListeningComprehension;
            }
            set
            {
                userSettings.ListeningComprehension = value;
            }
        }

        public bool? MultipleChoice
        {
            get
            {
                return userSettings.MultipleChoice ?? defaultSettings.MultipleChoice;
            }
            set
            {
                userSettings.MultipleChoice = value;
            }
        }

        public bool? Sentence
        {
            get
            {
                return userSettings.Sentence ?? defaultSettings.Sentence;
            }
            set
            {
                userSettings.Sentence = value;
            }
        }

        public bool? Word
        {
            get
            {
                return userSettings.Word ?? defaultSettings.Word;
            }
            set
            {
                userSettings.Word = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    /// <summary>
    /// The BL IQueryMultipleChoiceOptions implementation
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-04-10</remarks>
    class QueryMultipleChoiceOptions : IQueryMultipleChoiceOptions
    {
        private bool workAsContainer = false;
        private IQueryMultipleChoiceOptions defaultSettings;
        private IQueryMultipleChoiceOptions userSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMultipleChoiceOptions"/> class.
        /// </summary>
        /// <param name="defaultSettings">The default settings.</param>
        /// <param name="userSettings">The user settings.</param>
        /// <remarks>Documented by Dev08, 2009-04-10</remarks>
        public QueryMultipleChoiceOptions(IQueryMultipleChoiceOptions defaultSettings, IQueryMultipleChoiceOptions userSettings)
        {
            this.defaultSettings = defaultSettings;
            this.userSettings = userSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMultipleChoiceOptions"/> class.
        /// With this constructor, this can be used like a container class.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-10</remarks>
        public QueryMultipleChoiceOptions()
        {
            workAsContainer = true;
        }

        public override bool Equals(object obj)
        {
            return QueryMultipleChoiceOptionsHelper.Compare(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryMultipleChoiceOptions Members

        private bool? allowRandomDistractors;
        public bool? AllowRandomDistractors
        {
            get
            {
                if (workAsContainer)
                    return allowRandomDistractors;
                else
                    return userSettings.AllowRandomDistractors ?? defaultSettings.AllowRandomDistractors;
            }
            set
            {
                if (workAsContainer)
                    allowRandomDistractors = value;
                else
                    userSettings.AllowRandomDistractors = value;
            }
        }

        private bool? allowMultipleCorrectAnswers;
        public bool? AllowMultipleCorrectAnswers
        {
            get
            {
                if (workAsContainer)
                    return allowMultipleCorrectAnswers;
                else
                    return userSettings.AllowMultipleCorrectAnswers ?? defaultSettings.AllowMultipleCorrectAnswers;
            }
            set
            {
                if (workAsContainer)
                    allowMultipleCorrectAnswers = value;
                else
                    userSettings.AllowMultipleCorrectAnswers = value;
            }
        }

        private int? numberOfChoices;
        public int? NumberOfChoices
        {
            get
            {
                if (workAsContainer)
                    return numberOfChoices;
                else
                    return userSettings.NumberOfChoices ?? defaultSettings.NumberOfChoices;
            }
            set
            {
                if (workAsContainer)
                    numberOfChoices = value;
                else
                    userSettings.NumberOfChoices = value;
            }
        }

        private int? maxNumberOfCorrectAnswers;
        public int? MaxNumberOfCorrectAnswers
        {
            get
            {
                if (workAsContainer)
                    return maxNumberOfCorrectAnswers;
                else
                    return userSettings.MaxNumberOfCorrectAnswers ?? defaultSettings.MaxNumberOfCorrectAnswers;
            }
            set
            {
                if (workAsContainer)
                    maxNumberOfCorrectAnswers = value;
                else
                    userSettings.MaxNumberOfCorrectAnswers = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    class GradeTyping : IGradeTyping
    {
        private IGradeTyping defaultSettings;
        private IGradeTyping userSettings;

        public GradeTyping(IGradeTyping defaultSettings, IGradeTyping userSettings)
        {
            this.defaultSettings = defaultSettings;
            this.userSettings = userSettings;
        }

        public override bool Equals(object obj)
        {
            return GradeTypingHelper.Compare(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGradeTyping Members

        public bool? AllCorrect
        {
            get
            {
                return userSettings.AllCorrect ?? defaultSettings.AllCorrect;
            }
            set
            {
                userSettings.AllCorrect = value;
            }
        }

        public bool? HalfCorrect
        {
            get
            {
                return userSettings.HalfCorrect ?? defaultSettings.HalfCorrect;
            }
            set
            {
                userSettings.HalfCorrect = value;
            }
        }

        public bool? NoneCorrect
        {
            get
            {
                return userSettings.NoneCorrect ?? defaultSettings.NoneCorrect;
            }
            set
            {
                userSettings.NoneCorrect = value;
            }
        }

        public bool? Prompt
        {
            get
            {
                return userSettings.Prompt ?? defaultSettings.Prompt;
            }
            set
            {
                userSettings.Prompt = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    class GradeSynonyms : IGradeSynonyms
    {
        private IGradeSynonyms defaultSettings;
        private IGradeSynonyms userSettings;

        public GradeSynonyms(IGradeSynonyms defaultSettings, IGradeSynonyms userSettings)
        {
            this.defaultSettings = defaultSettings;
            this.userSettings = userSettings;
        }

        public override bool Equals(object obj)
        {
            return GradeSynonymsHelper.Compare(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGradeSynonyms Members

        public bool? AllKnown
        {
            get
            {
                return userSettings.AllKnown ?? defaultSettings.AllKnown;
            }
            set
            {
                userSettings.AllKnown = value;
            }
        }

        public bool? HalfKnown
        {
            get
            {
                return userSettings.HalfKnown ?? defaultSettings.HalfKnown;
            }
            set
            {
                userSettings.HalfKnown = value;
            }
        }

        public bool? OneKnown
        {
            get
            {
                return userSettings.OneKnown ?? defaultSettings.OneKnown;
            }
            set
            {
                userSettings.OneKnown = value;
            }
        }

        public bool? FirstKnown
        {
            get
            {
                return userSettings.FirstKnown ?? defaultSettings.FirstKnown;
            }
            set
            {
                userSettings.FirstKnown = value;
            }
        }

        public bool? Prompt
        {
            get
            {
                return userSettings.Prompt ?? defaultSettings.Prompt;
            }
            set
            {
                userSettings.Prompt = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    class SnoozeOptions : ISnoozeOptions
    {
        private ISnoozeOptions defaultSettings;
        private ISnoozeOptions userSettings;

        public SnoozeOptions(ISnoozeOptions defaultSettings, ISnoozeOptions userSettings)
        {
            this.defaultSettings = defaultSettings;
            this.userSettings = userSettings;
        }

        public override bool Equals(object obj)
        {
            return SnoozeOptionsHelper.Compare(this, obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region ISnoozeOptions Members

        public void DisableCards()
        {
            userSettings.DisableCards();
        }

        public void DisableRights()
        {
            userSettings.DisableRights();
        }

        public void DisableTime()
        {
            userSettings.DisableTime();
        }

        public void EnableCards(int cards)
        {
            userSettings.EnableCards(cards);
        }

        public void EnableRights(int rights)
        {
            userSettings.EnableRights(rights);
        }

        public void EnableTime(int time)
        {
            userSettings.EnableTime(time);
        }

        public bool? IsCardsEnabled
        {
            get
            {
                return userSettings.IsCardsEnabled ?? defaultSettings.IsCardsEnabled;
            }
            set
            {
                userSettings.IsCardsEnabled = value;
            }
        }

        public bool? IsRightsEnabled
        {
            get
            {
                return userSettings.IsRightsEnabled ?? defaultSettings.IsRightsEnabled;
            }
            set
            {
                userSettings.IsRightsEnabled = value;
            }
        }

        public bool? IsTimeEnabled
        {
            get
            {
                return userSettings.IsTimeEnabled ?? defaultSettings.IsTimeEnabled;
            }
            set
            {
                userSettings.IsTimeEnabled = value;
            }
        }

        public void SetSnoozeTimes(int lower_time, int upper_time)
        {
            userSettings.SetSnoozeTimes(lower_time, upper_time);
        }

        public ESnoozeMode? SnoozeMode
        {
            get
            {
                return userSettings.SnoozeMode ?? defaultSettings.SnoozeMode;
            }
            set
            {
                userSettings.SnoozeMode = value;
            }
        }

        public int? SnoozeCards
        {
            get
            {
                return userSettings.SnoozeCards ?? defaultSettings.SnoozeCards;
            }
            set
            {
                userSettings.SnoozeCards = value;
            }
        }

        public int? SnoozeRights
        {
            get
            {
                return userSettings.SnoozeRights ?? defaultSettings.SnoozeRights;
            }
            set
            {
                userSettings.SnoozeRights = value;
            }
        }

        public int? SnoozeTime
        {
            get
            {
                return userSettings.SnoozeTime ?? defaultSettings.SnoozeTime;
            }
            set
            {
                userSettings.SnoozeTime = value;
            }
        }

        public int? SnoozeHigh
        {
            get
            {
                return userSettings.SnoozeHigh ?? defaultSettings.SnoozeHigh;
            }
            set
            {
                userSettings.SnoozeHigh = value;
            }
        }

        public int? SnoozeLow
        {
            get
            {
                return userSettings.SnoozeLow ?? defaultSettings.SnoozeLow;
            }
            set
            {
                userSettings.SnoozeLow = value;
            }
        }

        #endregion
        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
