using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Security;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Business layer dictionary / learning module.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-19</remarks>
    public partial class Dictionary
    {
        internal IUser User
        {
            get { return dictionary.Parent.CurrentUser; }
        }

        /// <summary>
        /// Gets a value indicating whether the [dictionary is content protected].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dictionary content protected]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-09-30</remarks>
        public bool DictionaryContentProtected { get { return dictionary.ContentProtected; } }

        /// <summary>
        /// Gets a value indicating whether this instance can print.
        /// </summary>
        /// <value><c>true</c> if this instance can print; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanPrint
        {
            get
            {
                return dictionary.HasPermission(PermissionTypes.CanPrint);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can export.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can export; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanExport
        {
            get
            {
                return dictionary.HasPermission(PermissionTypes.CanExport) && !dictionary.ContentProtected;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can mofify.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can mofify; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanModify
        {
            get
            {
                return dictionary.HasPermission(PermissionTypes.CanModify);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can save a copy of a LM.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can save copy; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev09, 2009-03-06</remarks>
        public bool CanSaveCopy
        {
            get
            {
                return (dictionary.Parent.CurrentUser.ConnectionString.Typ == MLifter.DAL.DatabaseType.MsSqlCe
                    && dictionary.Parent.CurrentUser.ConnectionString.SyncType == SyncType.NotSynchronized)
                    || dictionary.Parent.CurrentUser.ConnectionString.Typ == MLifter.DAL.DatabaseType.Xml;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can edit cards.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit cards; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanModifyCards
        {
            get
            {
                return dictionary.Cards.HasPermission(PermissionTypes.CanModify);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can edit chapters.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit chapters; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanModifyChapters
        {
            get
            {
                return dictionary.Chapters.HasPermission(PermissionTypes.CanModify);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can edit styles.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit styles; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanModifyStyles
        {
            get
            {
                return dictionary.HasPermission(PermissionTypes.CanModifyStyles);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can edit properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit properties; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanModifyProperties
        {
            get
            {
                return dictionary.HasPermission(PermissionTypes.CanModifyProperties);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance can edit settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can edit settings; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public bool CanModifySettings
        {
            get
            {
                return dictionary.HasPermission(PermissionTypes.CanModifySettings);
            }
        }
    }
}
