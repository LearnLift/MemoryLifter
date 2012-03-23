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
    internal class XmlAllowedQueryTypes : IQueryType
    {
        XmlElement m_AllowedQueryTypes;
        const string m_XPathGeneral = "/dictionary/general";
        const string m_XPathAllowedQueryTypes = "allowedquerytypes";
        const string m_XPathBasePath = m_XPathGeneral + "/" + m_XPathAllowedQueryTypes;
        const string m_XPathQueryType = "querytype";
        const string m_XPathAttributeName = "name";
        const string m_XPathAttributeValue = "value";

        const string m_XPathFilter = "querytype[@name='{0}']/@value";

        internal XmlAllowedQueryTypes(XmlDictionary dictionary, ParentClass parent)
        {
            m_AllowedQueryTypes = (XmlElement)dictionary.Dictionary.SelectSingleNode(m_XPathBasePath);
            if (m_AllowedQueryTypes == null)
            {
                m_AllowedQueryTypes = XmlHelper.CreateAndAppendElement((XmlElement)dictionary.Dictionary.SelectSingleNode(m_XPathGeneral), m_XPathAllowedQueryTypes);
                AppendMode(EQueryType.Word, true);
                AppendMode(EQueryType.Sentences, true);
                AppendMode(EQueryType.MultipleChoice, true);
                AppendMode(EQueryType.ListeningComprehension, true);
                AppendMode(EQueryType.ImageRecognition, true);
            }
            this.parent = parent;
        }

        #region IQueryType Members

        public bool? ImageRecognition
        {
            get
            {
                return IsAllowed(EQueryType.ImageRecognition);
            }
            set
            {
                SetAllowed(EQueryType.ImageRecognition, value.GetValueOrDefault());
            }
        }

        public bool? ListeningComprehension
        {
            get
            {
                return IsAllowed(EQueryType.ListeningComprehension);
            }
            set
            {
                SetAllowed(EQueryType.ListeningComprehension, value.GetValueOrDefault());
            }
        }

        public bool? MultipleChoice
        {
            get
            {
                return IsAllowed(EQueryType.MultipleChoice);
            }
            set
            {
                SetAllowed(EQueryType.MultipleChoice, value.GetValueOrDefault());
            }
        }

        public bool? Sentence
        {
            get
            {
                return IsAllowed(EQueryType.Sentences);
            }
            set
            {
                SetAllowed(EQueryType.Sentences, value.GetValueOrDefault());
            }
        }

        public bool? Word
        {
            get
            {
                return IsAllowed(EQueryType.Word);
            }
            set
            {
                SetAllowed(EQueryType.Word, value.GetValueOrDefault());
            }
        }

        #endregion

        private bool IsAllowed(EQueryType queryType)
        {
            bool allowed = false;
            XmlNode node = m_AllowedQueryTypes.SelectSingleNode(String.Format(m_XPathFilter, queryType.ToString()));
            if (node == null)
                allowed = true;
            else
                allowed = XmlConvert.ToBoolean(node.Value);
            return allowed;
        }

        private void SetAllowed(EQueryType queryType, bool allowed)
        {
            XmlNode node = m_AllowedQueryTypes.SelectSingleNode(String.Format(m_XPathFilter, queryType.ToString()));
            if (node == null)
            {
                AppendMode(queryType, allowed);
            }
            else
            {
                node.Value = XmlConvert.ToString(allowed);
            }
        }

        private XmlElement AppendMode(EQueryType querytype, bool allowed)
        {
            XmlElement eWord = XmlHelper.CreateAndAppendElement(m_AllowedQueryTypes, m_XPathQueryType);
            eWord.SetAttribute(m_XPathAttributeName, querytype.ToString());
            eWord.SetAttribute(m_XPathAttributeValue, XmlConvert.ToString(allowed));
            return eWord;
        }

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryType), progressDelegate);
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
