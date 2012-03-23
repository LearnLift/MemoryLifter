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
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    internal abstract class XmlDistractors : IWords
    {
        private XmlElement m_card;
        protected CultureInfo m_Culture = null;
        protected XmlDictionary m_oDictionary;
        private List<IWord> m_Distractors = new List<IWord>();

        protected string m_XPathBasePath = null;
        protected string m_XPathDistractor = "distractor";

        protected XmlDistractors(ParentClass parent) { this.parent = parent; }
        public XmlDistractors(ParentClass parent, XmlDictionary dic)
            : this(parent)
        {
            m_oDictionary = dic;
        }

        #region IDistractors Members

        public System.Globalization.CultureInfo Culture
        {
            get
            {
                return m_Culture;
            }
        }

        public IList<IWord> Words
        {
            get
            {
                BindingList<IWord> distractorsBindingList = new BindingList<IWord>(m_Distractors);
                distractorsBindingList.ListChanged += new ListChangedEventHandler(distractorsBindingList_ListChanged);
                return distractorsBindingList;
            }
        }

        void distractorsBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
                AssignWordPropertyChanged(Words[e.NewIndex]);
            FlushToDOM();
        }

        public IWord CreateWord(string distractor, WordType type, bool isDefault)
        {
            return new XmlWord(distractor, WordType.Distractor, false, this.Parent);
        }

        public void AddWord(IWord distractor)
        {
            Words.Add(distractor);
        }

        public void AddWords(string[] distractors)
        {
            List<IWord> dist = new List<IWord>();
            foreach (string dis in distractors)
                dist.Add(CreateWord(dis, WordType.Distractor, false));
            AddWords(dist);
        }

        public void AddWords(List<IWord> distractors)
        {
            foreach (IWord distractor in distractors)
                AddWord(distractor);
        }

        public void ClearWords()
        {
            Words.Clear();
        }

        private void AssignWordPropertyChanged(IWord word)
        {
            //add property changed events
            if (word is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)word).PropertyChanged -= new PropertyChangedEventHandler(word_PropertyChanged);
                ((INotifyPropertyChanged)word).PropertyChanged += new PropertyChangedEventHandler(word_PropertyChanged);
            }
        }

        void word_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is IWord && this.Words.Contains((IWord)sender))
                FlushToDOM();
        }

        #endregion

        internal void FlushToDOM()
        {
            if (m_card[m_XPathBasePath] == null)
                XmlHelper.CreateAndAppendElement(m_card, m_XPathBasePath);
            m_card[m_XPathBasePath].RemoveAll();
            foreach (IWord distractor in m_Distractors)
            {
                XmlDistractor xmlDistractor = new XmlDistractor(m_card, distractor.Word, this.Parent.GetChildParentClass(this));
                xmlDistractor.Id = distractor.Id;
                xmlDistractor.Word = xmlDistractor.Word;
                m_card[m_XPathBasePath].AppendChild(xmlDistractor.Distractor as XmlNode);
            }
        }

        protected void Initialize(XmlCard card)
        {
            m_card = card.Xml;
            m_Distractors.Clear();
            if (m_card[m_XPathBasePath] != null)
            {
                foreach (XmlNode distractor in m_card[m_XPathBasePath].SelectNodes(m_XPathDistractor))
                {
                    XmlDistractor xmlDistractor = new XmlDistractor(distractor as XmlElement, this.Parent.GetChildParentClass(this));
                    IWord word = new XmlWord(xmlDistractor.Word, xmlDistractor.Type, false, Parent.GetChildParentClass(this));
                    AssignWordPropertyChanged(word);
                    m_Distractors.Add(word);
                }
            }
        }

        public string ToQuotedString()
        {
            if (m_Distractors.Count == 0)
                return String.Empty;

            List<string> quotedList = new List<string>();
            foreach (IWord distractor in m_Distractors)
            {
                quotedList.Add("\"" + distractor.Word + "\"");
            }
            return String.Join(", ", quotedList.ToArray());
        }

        public string ToNewlineString()
        {
            if (m_Distractors.Count == 0)
                return String.Empty;

            List<string> commaList = new List<string>();
            foreach (IWord distractor in m_Distractors)
            {
                commaList.Add(distractor.Word);
            }

            return String.Join(Environment.NewLine, commaList.ToArray());
        }

        /// <summary>
        /// Returns all words as a list of strings.
        /// </summary>
        /// <returns>The words.</returns>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        public IList<string> ToStringList()
        {
            IList<string> distractors = new List<string>();
            foreach (IWord distractor in m_Distractors)
                distractors.Add(distractor.Word);
            return distractors;
        }

        public override string ToString()
        {
            return ToQuotedString();
        }

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            WordsHelper.CopyWords(this, target as IWords);
        }

        #endregion

        #region IParent Members

        private MLifter.DAL.Tools.ParentClass parent;
        public MLifter.DAL.Tools.ParentClass Parent { get { return parent; } }

        #endregion
    }

    internal class XmlQuestionDistractors : XmlDistractors
    {
        private new const string m_XPathBasePath = "questiondistractors";

        internal XmlQuestionDistractors(XmlDictionary dic, XmlCard card, ParentClass parent)
            : base(parent)
        {
            base.m_oDictionary = dic;
            base.m_XPathBasePath = m_XPathBasePath;
            this.m_Culture = card.Dictionary.QuestionCulture;
            Initialize(card);
        }
    }

    internal class XmlAnswerDistractors : XmlDistractors
    {
        private new const string m_XPathBasePath = "answerdistractors";

        internal XmlAnswerDistractors(XmlDictionary dic, XmlCard card, ParentClass parent)
            : base(parent)
        {
            base.m_oDictionary = dic;
            base.m_XPathBasePath = m_XPathBasePath;
            this.m_Culture = card.Dictionary.AnswerCulture;
            Initialize(card);
        }
    }
}
