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
