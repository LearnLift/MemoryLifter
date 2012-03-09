using System.Collections.Generic;

using SecurityFramework;

namespace MLifter.DAL.Security
{
    /// <summary>
    /// Interface that extends 
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    public interface ISecurity
    {
        /// <summary>
        /// Determines whether the object has the specified permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns>
        /// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        bool HasPermission(string permissionName);
        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        List<PermissionInfo> GetPermissions();
    }
}
