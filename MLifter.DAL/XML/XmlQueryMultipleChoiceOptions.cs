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
    internal class XmlQueryMultipleChoiceOptions : IQueryMultipleChoiceOptions
    {
        XmlElement m_MultipleChoiceOptions;

        const string m_XPathGeneral = "/dictionary/general";
        const string m_XPathMultipleChoiceOptions = "multiplechoice";
        const string m_XPathBasePath = m_XPathGeneral + "/" + m_XPathMultipleChoiceOptions;
        const string m_XPathAllowRandomDistractorsName = "allowrandomdistractors";
        const string m_XPathNumberOfChoicesName = "numberofchoices";
        const string m_XPathMultipleCorrectAnswersName = "multiplecorrectanswers";
        const string m_XPathMaximumCorrectAnswersName = "maximumcorrectanswers";

        const bool m_DefValAllowRandomDistractors = true;
        const bool m_DefValMultipleCorrectAnswers = false;
        const int m_DefValNumberOfChoices = 3;
        const int m_DefValMaximumCorrectAnswers = 1;

        internal XmlQueryMultipleChoiceOptions(XmlDictionary dictionary, ParentClass parent)
        {
            m_MultipleChoiceOptions = (XmlElement)dictionary.Dictionary.SelectSingleNode(m_XPathBasePath);
            if (m_MultipleChoiceOptions == null)
            {
                m_MultipleChoiceOptions = XmlHelper.CreateAndAppendElement((XmlElement)dictionary.Dictionary.SelectSingleNode(m_XPathGeneral), m_XPathMultipleChoiceOptions);
                XmlHelper.CreateAndAppendAttribute(m_MultipleChoiceOptions, m_XPathAllowRandomDistractorsName, XmlConvert.ToString(m_DefValAllowRandomDistractors));
                XmlHelper.CreateAndAppendAttribute(m_MultipleChoiceOptions, m_XPathNumberOfChoicesName, XmlConvert.ToString(m_DefValNumberOfChoices));
                XmlHelper.CreateAndAppendAttribute(m_MultipleChoiceOptions, m_XPathMultipleCorrectAnswersName, XmlConvert.ToString(m_DefValMultipleCorrectAnswers));
                XmlHelper.CreateAndAppendAttribute(m_MultipleChoiceOptions, m_XPathMaximumCorrectAnswersName, XmlConvert.ToString(m_DefValMaximumCorrectAnswers));
            }
            this.parent = parent;
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

        public bool? AllowRandomDistractors
        {
            get
            {
                try
                {
                    return XmlConvert.ToBoolean(m_MultipleChoiceOptions.GetAttribute(m_XPathAllowRandomDistractorsName));
                }
                catch
                {
                    return m_DefValAllowRandomDistractors;
                }
            }
            set
            {
                m_MultipleChoiceOptions.SetAttribute(m_XPathAllowRandomDistractorsName, XmlConvert.ToString(value.GetValueOrDefault()));
            }
        }

        public bool? AllowMultipleCorrectAnswers
        {
            get
            {
                try
                {
                    return XmlConvert.ToBoolean(m_MultipleChoiceOptions.GetAttribute(m_XPathMultipleCorrectAnswersName));
                }
                catch
                {
                    return m_DefValMultipleCorrectAnswers;
                }
            }
            set
            {
                m_MultipleChoiceOptions.SetAttribute(m_XPathMultipleCorrectAnswersName, XmlConvert.ToString(value.GetValueOrDefault()));
            }
        }

        public int? NumberOfChoices
        {
            get
            {
                try
                {
                    return XmlConvert.ToInt32(m_MultipleChoiceOptions.GetAttribute(m_XPathNumberOfChoicesName));
                }
                catch
                {
                    return m_DefValNumberOfChoices;
                }
            }
            set
            {
                if ((value > 0) && (value < 100))
                {
                    m_MultipleChoiceOptions.SetAttribute(m_XPathNumberOfChoicesName, XmlConvert.ToString(value.Value));
                }
                else
                    throw new ArgumentOutOfRangeException("NumberOfChoices", "The parameter value must be between 1 and 100!");
            }
        }

        public int? MaxNumberOfCorrectAnswers
        {
            get
            {
                try
                {
                    return XmlConvert.ToInt32(m_MultipleChoiceOptions.GetAttribute(m_XPathMaximumCorrectAnswersName));
                }
                catch
                {
                    return m_DefValMaximumCorrectAnswers;
                }
            }
            set
            {
                if ((value > 0) && (value < 100))
                {
                    m_MultipleChoiceOptions.SetAttribute(m_XPathMaximumCorrectAnswersName, XmlConvert.ToString(value.Value));
                }
                else
                    throw new ArgumentOutOfRangeException("MaxNumberOfCorrectAnswers", "The parameter value must be between 1 and 100!");
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryMultipleChoiceOptions), progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion
    }
}
