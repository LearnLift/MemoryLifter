using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    /// <summary>
    /// XML implementation of IChapters.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    public class XmlChapters : IChapters
    {
        private XmlDictionary m_oDictionary;
        private XmlDocument m_dictionary;
        private List<IChapter> m_chapters = new List<IChapter>();
        private const string m_BaseXPath = "/dictionary/chapter";
        private const string m_AttributeId = "id";
        private const string m_XPathId = "@id";
        private const string m_XPathIdFilter = "[@id={0}]";
        private const string m_XPathCardsForChapter = "/dictionary/card[chapter={0}]";
        //required for GetNextId()
        private XPathNavigator m_navigator; //always use idNavigator.Clone()...!
        private XPathExpression m_expression;

        internal XmlChapters(XmlDictionary dictionary, ParentClass parent)
        {
            this.parent = parent;
            m_oDictionary = dictionary;
            m_dictionary = dictionary.Dictionary;
            Initialize();
            PrepareIdNavigator();
        }

		/// <summary>
		/// Gets the chapters.
		/// </summary>
		/// <value>
		/// The chapters.
		/// </value>
        public IList<IChapter> Chapters
        {
            get
            {
                BindingList<IChapter> bindingListChapters = new BindingList<IChapter>(m_chapters);
                bindingListChapters.ListChanged += new ListChangedEventHandler(bindingListChapters_ListChanged);
                return bindingListChapters;
            }
        }

        void bindingListChapters_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Append(m_chapters[e.NewIndex]);
                    break;
                case ListChangedType.ItemChanged:
                    throw new NotImplementedException();
                case ListChangedType.ItemDeleted:
                    throw new NotImplementedException(); //we do not know which id was deleted
                case ListChangedType.ItemMoved:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        internal void Initialize()
        {
            m_chapters.Clear();
            foreach (XmlNode chapter in m_dictionary.SelectNodes(m_BaseXPath))
                m_chapters.Add(new XmlChapter(m_oDictionary, (XmlElement)chapter, Parent.GetChildParentClass(this)));
        }

		/// <summary>
		/// Appends the specified new chapter.
		/// </summary>
		/// <param name="newChapter">The new chapter.</param>
        public void Append(IChapter newChapter)
        {
            ((XmlChapter)newChapter).Id = GetNextId();
            m_dictionary.DocumentElement.AppendChild(((XmlChapter)newChapter).Chapter);
            Initialize();
        }

        #region IChapters Members

		/// <summary>
		/// Adds a new chapter.
		/// </summary>
		/// <returns></returns>
        public IChapter AddNew()
        {
            IChapter newChapter = new XmlChapter(m_oDictionary, Parent.GetChildParentClass(this));
            Append(newChapter);
            Initialize();
            return newChapter;
        }

		/// <summary>
		/// Deletes the specified chapter.
		/// </summary>
		/// <param name="chapter_id">The chapter_id.</param>
        public void Delete(int chapter_id)
        {
            XmlNode chapterToDelete = m_dictionary.SelectSingleNode(m_BaseXPath + String.Format(m_XPathIdFilter, chapter_id));
            if (chapterToDelete == null)
            {
                throw new IdAccessException(chapter_id);
            }
            chapterToDelete.ParentNode.RemoveChild(chapterToDelete);
            XmlNodeList cardsToDelete = m_dictionary.SelectNodes(String.Format(m_XPathCardsForChapter, chapter_id));
            foreach (XmlNode card in cardsToDelete)
            {
                card.ParentNode.RemoveChild(card);
            }
            ((XmlCards)m_oDictionary.Cards).Initialize();
            Initialize();
        }

		/// <summary>
		/// Gets the specified chapter.
		/// </summary>
		/// <param name="chapter_id">The chapter_id.</param>
		/// <returns></returns>
        public IChapter Get(int chapter_id)
        {
            IChapter chapter = m_chapters.Find(delegate(IChapter chap)
                    {
                        return (chap.Id == chapter_id);
                    });
            if (chapter == null)
                throw new IdAccessException(chapter_id);
            return chapter;
        }

		/// <summary>
		/// Finds the specified title.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <returns></returns>
        public IChapter Find(string title)
        {
            return m_chapters.Find(delegate(IChapter chapter)
                    {
                        return (chapter.Title.ToLower().Trim() == title.ToLower().Trim());
                    });
        }

		/// <summary>
		/// Swaps the id.
		/// </summary>
		/// <param name="first_id">The first_id.</param>
		/// <param name="second_id">The second_id.</param>
        public void SwapId(int first_id, int second_id)
        {
            XmlNode firstNode = m_dictionary.SelectSingleNode(m_BaseXPath + String.Format(m_XPathIdFilter, first_id));
            XmlNode secondNode = m_dictionary.SelectSingleNode(m_BaseXPath + String.Format(m_XPathIdFilter, second_id));
            firstNode.Attributes[m_AttributeId].Value = second_id.ToString();
            secondNode.Attributes[m_AttributeId].Value = first_id.ToString();
            Initialize();
        }

		/// <summary>
		/// Moves the specified chapter.
		/// </summary>
		/// <param name="first_id">The first_id.</param>
		/// <param name="second_id">The second_id.</param>
        public void Move(int first_id, int second_id)
        {
            XmlNode firstNode = m_dictionary.SelectSingleNode(m_BaseXPath + String.Format(m_XPathIdFilter, first_id));
            if (firstNode == null)
            {
                throw new IdAccessException(first_id);
            }
            XmlNode secondNode = m_dictionary.SelectSingleNode(m_BaseXPath + String.Format(m_XPathIdFilter, second_id));
            if (secondNode == null)
            {
                throw new IdAccessException(second_id);
            }
            foreach (IChapter chapter in Chapters)
            {
                if (chapter.Id == first_id)
                {
                    secondNode.ParentNode.InsertAfter(firstNode, secondNode);
                    break;
                }
                if (chapter.Id == second_id)
                {
                    secondNode.ParentNode.InsertBefore(firstNode, secondNode);
                    break;
                }
            }
            Initialize();
        }

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
        public int Count
        {
            get
            {
                return m_navigator.Clone().Select(m_expression).Count;
            }
        }

        #endregion

        /// <summary>
        /// Prepares the Xpath navigator to improve performance for GetNextId().
        /// </summary>
        private void PrepareIdNavigator()
        {
            m_navigator = m_dictionary.CreateNavigator();
            m_expression = m_navigator.Compile(m_BaseXPath);
            m_expression.AddSort(m_XPathId, XmlSortOrder.Descending, XmlCaseOrder.None, string.Empty, XmlDataType.Number);
        }

        /// <summary>
        /// Gets the next id.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-08-06</remarks>
        private int GetNextId()
        {
            int lastId = 0;
            XPathNodeIterator chapters = m_navigator.Clone().Select(m_expression);
            if (chapters.MoveNext())
            {
                if (chapters.Current.MoveToAttribute(m_AttributeId, string.Empty))
                {
                    if (Int32.TryParse(chapters.Current.Value, out lastId))
                    {
                        return ++lastId;
                    }
                }
                throw new IdOverflowException();
            }
            else
            {
                return lastId;
            }
        }

        #region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
        public void CopyTo(ICopy target, CopyToProgress progressDelegate)
        {
            IChapters targetChapters = target as IChapters;
            if (targetChapters != null)
                ChaptersHelper.Copy(this, targetChapters, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion

        #region ISecurity Members

        /// <summary>
        /// Determines whether the object has the specified permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns>
        /// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public bool HasPermission(string permissionName)
        {
            return true;
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            return new List<SecurityFramework.PermissionInfo>();
        }

        #endregion
    }
}
