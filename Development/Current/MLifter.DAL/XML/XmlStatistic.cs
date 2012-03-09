using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.XML
{
    /// <summary>
    /// XmlStatistic
    /// </summary>
    public class XmlStatistic : IStatistic
    {
        private XmlDictionary m_oDictionary;
        private XmlDocument m_dictionary;
        private XmlElement m_statistic;
        List<int> m_boxSizes = new List<int>();

        private const string m_XPathStatsFilter = "/dictionary/stats[@id='{0}']";
        private const string m_XPathBoxesFilter = "boxes[@id='{0}']";
        private const string m_XPathStatsElement = "stats";
        private const string m_XPathIdAttribute = "id";
        private const string m_XPathStartElement = "start";
        private const string m_XPathEndElement = "end";
        private const string m_XPathRightElement = "right";
        private const string m_XPathWrongElement = "wrong";
        private const string m_XPathBoxesElement = "boxes";

        internal bool RunningSession = false;

        internal XmlStatistic(XmlDictionary dictionary, int id, bool createNew)
        {
            m_oDictionary = dictionary;
            m_dictionary = dictionary.Dictionary;

            if (createNew)
            {
                XmlElement xeStat = m_dictionary.CreateElement(m_XPathStatsElement);
                XmlHelper.CreateAndAppendAttribute(xeStat, m_XPathIdAttribute, Convert.ToString(0));
                XmlHelper.CreateAndAppendElement(xeStat, m_XPathStartElement, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.RoundtripKind));
                XmlHelper.CreateAndAppendElement(xeStat, m_XPathEndElement, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.RoundtripKind));
                XmlHelper.CreateAndAppendElement(xeStat, m_XPathRightElement, Convert.ToString(0));
                XmlHelper.CreateAndAppendElement(xeStat, m_XPathWrongElement, Convert.ToString(0));
                for (int i = 0; i < m_oDictionary.NumberOfBoxes; i++)
                {
                    XmlHelper.CreateElementWithAttribute(xeStat, m_XPathBoxesElement, XmlConvert.ToString(0), m_XPathIdAttribute, XmlConvert.ToString(i));
                }
                m_statistic = xeStat;
                Id = id;

                m_dictionary.DocumentElement.AppendChild(Statistic);
            }
            else
            {
                string baseXPath = String.Format(m_XPathStatsFilter, id);
                m_statistic = (XmlElement)m_dictionary.SelectSingleNode(baseXPath);
            }

            InitializeBoxList();
        }

        internal XmlStatistic(XmlDictionary dictionary, XmlElement statistic)
        {
            m_oDictionary = dictionary;
            m_dictionary = dictionary.Dictionary;
            m_statistic = statistic;
            InitializeBoxList();
        }

        internal XmlElement Statistic
        {
            get
            {
                return m_statistic;
            }
        }

        #region IStatistic Members

        /// <summary>
        /// Gets the id of the Session
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        public int Id
        {
            get
            {
                return int.Parse(m_statistic.GetAttribute(m_XPathIdAttribute));
            }
            set
            {
                m_statistic.SetAttribute(m_XPathIdAttribute, value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the start timestamp of the session.
        /// </summary>
        /// <value>The start timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        public DateTime StartTimestamp
        {
            get
            {
                return XmlConvert.ToDateTime(m_statistic[m_XPathStartElement].InnerText, XmlDateTimeSerializationMode.RoundtripKind);
            }
            set
            {
                m_statistic[m_XPathStartElement].InnerText = XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
            }
        }

        /// <summary>
        /// Gets or sets the end timestamp of the session.
        /// </summary>
        /// <value>The end timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        public DateTime EndTimestamp
        {
            get
            {
                return RunningSession ? DateTime.Now : XmlConvert.ToDateTime(m_statistic[m_XPathEndElement].InnerText, XmlDateTimeSerializationMode.RoundtripKind);
            }
            set
            {
                m_statistic[m_XPathEndElement].InnerText = XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
            }
        }

        /// <summary>
        /// Gets or sets the correct answers of the session.
        /// </summary>
        /// <value>The right.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        public int Right
        {
            get
            {
                return int.Parse(m_statistic[m_XPathRightElement].InnerText);
            }
            set
            {
                m_statistic[m_XPathRightElement].InnerText = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the wrong answers of the session.
        /// </summary>
        /// <value>The wrong.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        public int Wrong
        {
            get
            {
                return int.Parse(m_statistic[m_XPathWrongElement].InnerText);
            }
            set
            {
                m_statistic[m_XPathWrongElement].InnerText = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the boxes of the session.
        /// </summary>
        /// <value>The boxes.</value>
        /// <remarks>Documented by Dev03, 2007-09-06</remarks>
        public IList<int> Boxes
        {
            get
            {
                BindingList<int> bindingListBoxes = new BindingList<int>(m_boxSizes);
                bindingListBoxes.ListChanged += new ListChangedEventHandler(bindingListBoxes_ListChanged);
                return bindingListBoxes;
            }
        }

        void bindingListBoxes_ListChanged(object sender, ListChangedEventArgs e)
        {
            Save();
        }

        #endregion

        internal void Save()
        {
            FlushToDOM();
        }

        private void FlushToDOM()
        {
            for (int i = 0; i < m_boxSizes.Count; i++)
            {
                XmlElement boxsize = (XmlElement)m_statistic.SelectSingleNode(String.Format(m_XPathBoxesFilter, i));
                if (boxsize == null)
                    boxsize = XmlHelper.CreateElementWithAttribute(m_statistic, m_XPathBoxesElement, m_boxSizes[i].ToString(), m_XPathIdAttribute, i.ToString());
                else
                    boxsize.InnerText = m_boxSizes[i].ToString();
            }
        }

        private void InitializeBoxList()
        {
            m_boxSizes.Clear();
            foreach (XmlNode node in m_statistic.SelectNodes(m_XPathBoxesElement))
                m_boxSizes.Add(int.Parse(node.InnerText));
        }
    }
}
