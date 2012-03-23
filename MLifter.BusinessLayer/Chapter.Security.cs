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
using MLifter.DAL.Security;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Business layer representation of a chapter.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-19</remarks>
    public partial class Chapter
    {
        public bool CanModify { get { return chapter.HasPermission(PermissionTypes.CanModify); } }
        public bool CanModifyMedia { get { return chapter.HasPermission(PermissionTypes.CanModifyMedia); } }
        public bool CanModifyStyles { get { return chapter.HasPermission(PermissionTypes.CanModifyStyles); } }
    }
}
