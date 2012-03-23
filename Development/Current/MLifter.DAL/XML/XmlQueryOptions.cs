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
using System.Xml;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    class XmlQueryOptions : IQueryOptions
    {
        XmlDocument m_dictionary;
        XmlElement m_userSettings;
        private IQueryMultipleChoiceOptions m_MultipleChoiceOptions;

        private const string m_basePath = "/dictionary/user";
        private const string m_xpathGenSet = "/dictionary/general";
        private const string m_xpathQueryOptions = "/queryoptions";
        private const string m_xpathStripChars = "stripchars";
        private const string m_xpathGradeSynonyms = "gradesynonyms";
        private const string m_xpathGradeTyping = "gradetyping";
        private const string m_xpathQueryDir = "querydir";

        internal XmlQueryOptions(XmlDictionary dictionary, ParentClass parentClass)
        {
            parent = parentClass;

            m_dictionary = dictionary.Dictionary;
            m_userSettings = (XmlElement)m_dictionary.SelectSingleNode(m_basePath);
            m_MultipleChoiceOptions = new XmlQueryMultipleChoiceOptions(dictionary, Parent.GetChildParentClass(this));
        }

        #region IQueryOptions Members

        public string StripChars
        {
            get
            {
                return m_userSettings[m_xpathStripChars].InnerText;
            }
            set
            {
                m_userSettings[m_xpathStripChars].InnerText = value;
            }
        }

        public EGradeSynonyms GradeSynonyms
        {
            get
            {
                return (EGradeSynonyms)XmlConvert.ToInt32(m_userSettings[m_xpathGradeSynonyms].InnerText);
            }
            set
            {
                m_userSettings[m_xpathGradeSynonyms].InnerText = XmlConvert.ToString(((int)value));
            }
        }

        public EGradeTyping GradeTyping
        {
            get
            {
                return (EGradeTyping)XmlConvert.ToInt32(m_userSettings[m_xpathGradeTyping].InnerText);
            }
            set
            {
                m_userSettings[m_xpathGradeTyping].InnerText = XmlConvert.ToString(((int)value));
            }
        }

        public ISnoozeOptions SnoozeOptions
        {
            get
            {
                return new XmlSnoozeOptions(m_dictionary, Parent);
            }
            set
            {
                //not implemented because modifications to the XmlSnoozeOptions object directly affect the underlying XmlDocument dictionary - AAB 20070802
            }
        }

        public bool ConfirmDemote
        {
            get
            {
                return Check(EQueryOption.ConfirmDemote);
            }
            set
            {
                if (value)
                    Set(EQueryOption.ConfirmDemote);
                else
                    Unset(EQueryOption.ConfirmDemote);
            }
        }

        public bool RandomPool
        {
            get
            {
                return Check(EQueryOption.RandomPool);
            }
            set
            {
                if (value)
                    Set(EQueryOption.RandomPool);
                else
                    Unset(EQueryOption.RandomPool);
            }
        }

        public bool SelfAssessment
        {
            get
            {
                return Check(EQueryOption.Self);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Self);
                else
                    Unset(EQueryOption.Self);
            }
        }

        public bool SkipCorrectAnswers
        {
            get
            {
                return Check(EQueryOption.Skip);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Skip);
                else
                    Unset(EQueryOption.Skip);
            }
        }

        public bool CorrectOnTheFly
        {
            get
            {
                return Check(EQueryOption.Correct);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Correct);
                else
                    Unset(EQueryOption.Correct);
            }
        }

        public bool EnableCommentary
        {
            get
            {
                return Check(EQueryOption.Commentary);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Commentary);
                else
                    Unset(EQueryOption.Commentary);
            }
        }

        public bool AutoplayAudio
        {
            get
            {
                return Check(EQueryOption.Sounds);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Sounds);
                else
                    Unset(EQueryOption.Sounds);
            }
        }

        public bool ShowImages
        {
            get
            {
                return Check(EQueryOption.Images);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Images);
                else
                    Unset(EQueryOption.Images);
            }
        }

        public bool ShowStatistics
        {
            get
            {
                return Check(EQueryOption.Stats);
            }
            set
            {
                if (value)
                    Set(EQueryOption.Stats);
                else
                    Unset(EQueryOption.Stats);
            }
        }

        public bool EnableTimer
        {
            get
            {
                return Check(EQueryOption.CountDown);
            }
            set
            {
                if (value)
                    Set(EQueryOption.CountDown);
                else
                    Unset(EQueryOption.CountDown);
            }
        }

        public bool CaseSensitive
        {
            get
            {
                return Check(EQueryOption.CaseSensitive);
            }
            set
            {
                if (value)
                    Set(EQueryOption.CaseSensitive);
                else
                    Unset(EQueryOption.CaseSensitive);
            }
        }

        public IQueryType QueryTypes
        {
            get
            {
                return new XmlQueryType(m_dictionary, Parent.GetChildParentClass(this));
            }
            set
            {
                //not implemented because modifications to the XmlQueryType object directly affect the underlying XmlDocument dictionary - AAB 20070802
            }
        }

        public EQueryDirection QueryDirection
        {
            get
            {
                return (EQueryDirection)XmlConvert.ToInt32(m_userSettings[m_xpathQueryDir].InnerText);
            }
            set
            {
                m_userSettings[m_xpathQueryDir].InnerText = XmlConvert.ToString(((int)value));
            }
        }


        public IQueryMultipleChoiceOptions MultipleChoiceOptions
        {
            get
            {
                return m_MultipleChoiceOptions;
            }
            set
            {
                m_MultipleChoiceOptions.AllowMultipleCorrectAnswers = value.AllowMultipleCorrectAnswers;
                m_MultipleChoiceOptions.AllowRandomDistractors = value.AllowRandomDistractors;
                m_MultipleChoiceOptions.MaxNumberOfCorrectAnswers = value.MaxNumberOfCorrectAnswers;
                m_MultipleChoiceOptions.NumberOfChoices = value.NumberOfChoices;
            }
        }
        #endregion

        /// <summary>
        /// Sets the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        private void Set(EQueryOption option)
        {
            XmlConfigHelper.Set(m_dictionary, m_basePath + m_xpathQueryOptions, (int)option);
        }

        /// <summary>
        /// Unsets the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        private void Unset(EQueryOption option)
        {
            XmlConfigHelper.Unset(m_dictionary, m_basePath + m_xpathQueryOptions, (int)option);
        }

        /// <summary>
        /// Checks the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-08-02</remarks>
        private bool Check(EQueryOption option)
        {
            return XmlConfigHelper.Check(m_dictionary, m_basePath + m_xpathQueryOptions, (int)option);
        }

        #region IParent Members

        private ParentClass parent;
        public ParentClass Parent { get { return parent; } }

        #endregion
    }
}
