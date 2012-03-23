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
using System.Globalization;
using System.Xml.Xsl;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbSettingsConnector
    {
        void CheckSettingsId(int id);

        IQueryDirections GetQueryDirections(int id);
        void SetQueryDirections(int id, IQueryDirections QueryDirections);

        IQueryType GetQueryType(int id);
        void SetQueryType(int id, IQueryType QueryType);

        IQueryMultipleChoiceOptions GetMultipleChoiceOptions(int id);
        void SetMultipleChoiceOptions(int id, IQueryMultipleChoiceOptions MultipleChoiceOptions);

        IGradeTyping GetGradeTyping(int id);
        void SetGradeTyping(int id, IGradeTyping GradeTyping);

        IGradeSynonyms GetGradeSynonyms(int id);
        void SetGradeSynonyms(int id, IGradeSynonyms GradeSynonyms);

        bool? GetAutoplayAudio(int id);
        void SetAutoplayAudio(int id, bool? AutoplayAudio);

        string GetCaption(int id, Side side);
        void SetCaption(int id, Side side, string Caption);

        CultureInfo GetCulture(int id, Side side);
        void SetCulture(int id, Side side, CultureInfo Culture);

        bool? GetCaseSensitive(int id);
        void SetCaseSensitive(int id, bool? CaseSensetive);

        Dictionary<CommentarySoundIdentifier, IMedia> GetCommentarySounds(int id);
        void SetCommentarySounds(int id, Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds);

        bool? GetConfirmDemote(int id);
        void SetConfirmDemote(int id, bool? ConfirmDemote);

        bool? GetCorrectOnTheFly(int id);
        void SetCorrectOnTheFly(int id, bool? CorrectOnTheFly);

        bool? GetEnableCommentary(int id);
        void SetEnableCommentary(int id, bool? EnableCommentary);

        bool? GetEnableTimer(int id);
        void SetEnableTimer(int id, bool? EnableTime);

        IMedia GetLogo(int id);
        void SetLogo(int id, IMedia Logo);

        bool? GetRandomPool(int id);
        void SetRandomPool(int id, bool? RandomPool);

        bool? GetSelfAssessment(int id);
        void SetSelfAssessment(int id, bool? SelfAssessment);

        bool? GetShowImages(int id);
        void SetShowImages(int id, bool? ShowImages);

        bool? GetShowStatistics(int id);
        void SetShowStatistics(int id, bool? ShowStatistics);

        bool? GetSkipCorrectAnswers(int id);
        void SetSkipCorrectAnswers(int id, bool? SkipCorrectAnswers);

        bool? GetPoolEmptyMessage(int id);
        void SetPoolEmptyMessage(int id, bool? PoolEmtyMessage);

        bool? GetUseLmStylesheets(int id);
        void SetUseLmStylesheets(int id, bool? UseLmStylesheets);

        IList<int> GetSelectedLearningChapters(int id);
        void SetSelectedLearningChapters(int id, IList<int> SelectedLearningChapters);

        ISnoozeOptions GetSnoozeOptions(int id);
        void SetSnoozeOptions(int id, ISnoozeOptions SnoozeOptions);

        string GetStripChars(int id);
        void SetStripChars(int id, string StripChars);

        bool? GetAutoBoxSize(int id);
        void SetAutoBoxSize(int id, bool? AutoBoxSize);

        ICardStyle GetCardStyle(int id);
        void SetCardStyle(int id, ICardStyle Style);

        string GetQuestionStylesheet(int id);
        void SetQuestionStylesheet(int id, string QuestionStylesheet);

        string GetAnswerStylesheet(int id);
        void SetAnswerStylesheet(int id, string AnswerStylesheet);
    }
}
