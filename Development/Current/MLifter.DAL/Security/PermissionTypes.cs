using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Security
{
    /// <summary>
    /// Defines the available permission types.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-19</remarks>
    public static class PermissionTypes
    {
        /// <summary>
        /// Is the user Administrator.
        /// </summary>
        public static readonly string IsAdmin = "IsAdmin";
        /// <summary>
        /// Is the object visible.
        /// </summary>
        public static readonly string Visible = "Visible";
        /// <summary>
        /// Can a user print the object.
        /// </summary>
        public static readonly string CanPrint = "CanPrint";
        /// <summary>
        /// Can a user print the object.
        /// </summary>
        public static readonly string CanExport = "CanExport";
        /// <summary>
        /// Can a user modify the object.
        /// </summary>
        public static readonly string CanModify = "CanModify";
        /// <summary>
        /// Can a user modify media objects.
        /// </summary>
        public static readonly string CanModifyMedia = "CanModifyMedia";
        /// <summary>
        /// Can a user modify the settings.
        /// </summary>
        public static readonly string CanModifySettings = "CanModifySettings";
        /// <summary>
        /// Can a user modify the styles.
        /// </summary>
        public static readonly string CanModifyStyles = "CanModifyStyles";
        /// <summary>
        /// Can a user modify the properties.
        /// </summary>
        public static readonly string CanModifyProperties = "CanModifyProperties";
    }
}
