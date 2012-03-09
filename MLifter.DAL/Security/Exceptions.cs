using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Security
{
    /// <summary>
    /// Exception is thrown if a user has insufficient permission to perform an action.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-02-12</remarks>
    public class PermissionException : Exception { }
}
