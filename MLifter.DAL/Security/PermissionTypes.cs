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
