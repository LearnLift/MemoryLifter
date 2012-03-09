using System.Collections;
using System.Collections.Generic;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Security;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Database implementation of IChapters.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    class DbChapters : IChapters
    {
        private Interfaces.DB.IDbChaptersConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return MLifter.DAL.DB.PostgreSQL.PgSqlChaptersConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeChaptersConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbChapters"/> class.
        /// </summary>
        /// <param name="lm_id">The lm_id.</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbChapters(int lm_id, ParentClass parent)
        {
            this.parent = parent;
            connector.CheckLMId(lm_id);
            lmid = lm_id;
        }

        private int lmid;
        /// <summary>
        /// Gets the lm id.
        /// </summary>
        /// <value>The lm id.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int LmId
        {
            get
            {
                return lmid;
            }
        }

        #region IChapters Members

        /// <summary>
        /// Gets the chapters.
        /// </summary>
        /// <value>The chapters.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IList<IChapter> Chapters
        {
            get
            {
                IList<IChapter> list = new List<IChapter>();
                IList<int> chapterIds = connector.GetChapterIds(LmId);

                foreach (int id in chapterIds)
                {
                    DbChapter chapter = new DbChapter(id, false, Parent.GetChildParentClass(this));
                    if (chapter.HasPermission(PermissionTypes.Visible))
                        list.Add(chapter);
                }

                return list;
            }
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IChapter AddNew()
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            return new DbChapter(connector.AddNewChapter(LmId), false, Parent.GetChildParentClass(this));
        }

        /// <summary>
        /// Deletes the specified chapter_id.
        /// </summary>
        /// <param name="chapter_id">The chapter_id.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void Delete(int chapter_id)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            connector.DeleteChapter(LmId, chapter_id);
            Log.RecalculateBoxSizes(Parent);
            parent.CurrentUser.Cache.Clear();   // need to remove all cards from cache as well
        }

        /// <summary>
        /// Finds the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IChapter Find(string title)
        {
            int id = connector.FindChapter(LmId, title);

            if (id < 0)
                return null;

            DbChapter chapter = new DbChapter(id, false, Parent.GetChildParentClass(this));
            if (!chapter.HasPermission(PermissionTypes.Visible))
                return null;

            return chapter;
        }

        /// <summary>
        /// Gets the specified chapter_id.
        /// </summary>
        /// <param name="chapter_id">The chapter_id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IChapter Get(int chapter_id)
        {
            DbChapter chapter = new DbChapter(chapter_id, false, Parent.GetChildParentClass(this));
            if (!chapter.HasPermission(PermissionTypes.Visible))
                throw new PermissionException();
            return chapter;
        }

        /// <summary>
        /// Moves the specified first_id.
        /// </summary>
        /// <param name="first_id">The first_id.</param>
        /// <param name="second_id">The second_id.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void Move(int first_id, int second_id)
        {
            if (!this.HasPermission(PermissionTypes.CanModify))
                throw new PermissionException();
            connector.MoveChapter(LmId, first_id, second_id);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Count
        {
            get
            {
                return connector.GetChapterCount(LmId);
            }
        }

        #endregion

        #region ICopy Members

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
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
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
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
            return Parent.CurrentUser.HasPermission(this, permissionName);
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            return Parent.CurrentUser.GetPermissions(this);
        }

        #endregion
    }
}
