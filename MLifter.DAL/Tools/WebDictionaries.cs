using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Preview;

namespace MLifter.DAL
{
    /// <summary>
    /// Get a list of learning modules from a web service.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-06</remarks>
    internal class WebDictionaries : IDictionaries
    {
        SerializableDictionary<int, string> dictionaryList;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDictionaries"/> class.
        /// </summary>
        /// <param name="dics">The dics.</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public WebDictionaries(SerializableDictionary<int, string> dics, ParentClass parent)
        {
            dictionaryList = dics;
            Parent = parent;
        }

        #region IDictionaries Members

        /// <summary>
        /// Gets all learning modules.
        /// </summary>
        /// <value>The dictionaries.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public IList<IDictionary> Dictionaries
        {
            get
            {
                List<IDictionary> dics = new List<IDictionary>();
                foreach (KeyValuePair<int, string> pair in dictionaryList)
                {
                    IDictionary dic = new PreviewDictionary(Parent.CurrentUser);
                    dic.Title = pair.Value;
                    (dic as PreviewDictionary).Id = pair.Key;
                    (dic as PreviewDictionary).Connection = Parent.CurrentUser.ConnectionString.ConnectionString;
                    dics.Add(dic);
                }
                return dics;
            }
        }

        /// <summary>
        /// Gets the specified learning module.
        /// </summary>
        /// <param name="id">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public IDictionary Get(int id) { throw new NotImplementedException(); }

        /// <summary>
        /// Deletes a specific LM.
        /// </summary>
        /// <param name="css">The connection string struct.</param>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev08, 2008-12-09</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public void Delete(ConnectionStringStruct css) { throw new NotImplementedException(); }

        /// <summary>
        /// Adds a new learning module.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="title">The title.</param>
        /// <returns>The new learning module.</returns>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public IDictionary AddNew(int categoryId, string title) { throw new NotImplementedException(); }
        /// <summary>
        /// Adds a new learning module.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="title">The title.</param>
        /// <param name="licenceKey"></param>
        /// <param name="contentProtected"></param>
        /// <param name="calCount">The cal count.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-12</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public IDictionary AddNew(int categoryId, string title, string licenceKey, bool contentProtected, int calCount) { throw new NotImplementedException(); }

        /// <summary>
        /// Gets the count of learning modules.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public int Count { get { return dictionaryList.Count; } }

        /// <summary>
        /// Gets all extensions (independent of the LearningModule id).
        /// </summary>
        /// <value>The extensions.</value>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        /// <remarks>Documented by Dev08, 2009-07-02</remarks>
        public IList<IExtension> Extensions
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public IExtension ExtensionFactory()
        {
            ExtensionFactory(-1);
            throw new NotImplementedException();
        }

        public IExtension ExtensionFactory(int lmId)
        {
            throw new NotImplementedException();
        }

        public void DeleteExtension(IExtension extension)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IParent Members

        public ParentClass Parent { get; private set; }

        #endregion

        #region ISecurity Members

        public bool HasPermission(string permissionName) { throw new NotImplementedException(); }
        public List<SecurityFramework.PermissionInfo> GetPermissions() { throw new NotImplementedException(); }

        #endregion
    }
}
