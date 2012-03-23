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
using System.Globalization;
using System.Linq;
using System.Text;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Security;
using SecurityFramework;
using System.IO;
using MLifter.DAL.Tools;

namespace MLifterSettingsManager.DAL
{
    [Serializable()]
    public class SettingsTemplate : ISettings
    {
        public SettingsTemplate()
        {

        }

        public string Filename { get; set; }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(Filename);
        }

        #region ISettings Members

        private IQueryDirections queryDirections = new QueryDirectionsTemplate();
        public IQueryDirections QueryDirections
        {
            get
            {
                return queryDirections;
            }
            set
            {
                queryDirections = value;
            }
        }

        private IQueryType queryTypes = new QueryTypeTemplate();
        public IQueryType QueryTypes
        {
            get
            {
                return queryTypes;
            }
            set
            {
                queryTypes = value;
            }
        }

        private IQueryMultipleChoiceOptions multipleChoiceOptions = new QueryMultipleChoiceOptionsTemplate();
        public IQueryMultipleChoiceOptions MultipleChoiceOptions
        {
            get
            {
                return multipleChoiceOptions;
            }
            set
            {
                multipleChoiceOptions = value;
            }
        }

        public IGradeTyping GradeTyping { get; set; }

        public IGradeSynonyms GradeSynonyms { get; set; }

        public ICardStyle Style { get; set; }

        public CompiledTransform? QuestionStylesheet { get; set; }

        public CompiledTransform? AnswerStylesheet { get; set; }

        public bool? AutoplayAudio { get; set; }

        public bool? CaseSensitive { get; set; }

        public bool? ConfirmDemote { get; set; }

        public bool? EnableCommentary { get; set; }

        public bool? CorrectOnTheFly { get; set; }

        public bool? EnableTimer { get; set; }

        public bool? RandomPool { get; set; }

        public bool? SelfAssessment { get; set; }

        public bool? ShowImages { get; set; }

        public bool? PoolEmptyMessageShown { get; set; }

        public bool? UseLMStylesheets { get; set; }

        public bool? AutoBoxSize { get; set; }

        public string StripChars { get; set; }

        public CultureInfo QuestionCulture { get; set; }

        public CultureInfo AnswerCulture { get; set; }

        public string QuestionCaption { get; set; }

        public string AnswerCaption { get; set; }

        public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds { get; set; }

        public IMedia Logo { get; set; }

        public bool? ShowStatistics { get; set; }

        public bool? SkipCorrectAnswers { get; set; }

        public ISnoozeOptions SnoozeOptions { get; set; }

        public IList<int> SelectedLearnChapters { get; set; }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this as ISettings, target, typeof(ISettings), progressDelegate);
        }

        #endregion

        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ISecurity Members

        public bool HasPermission(string permissionName)
        {
            throw new NotImplementedException();
        }

        public List<PermissionInfo> GetPermissions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
