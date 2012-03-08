using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Security;

namespace MLifter.BusinessLayer
{
    public partial class Card
    {
        public bool CanModify { get { return card.HasPermission(PermissionTypes.CanModify); } }
        public bool CanModifyMedia { get { return card.HasPermission(PermissionTypes.CanModifyMedia); } }
        public bool CanModifyStyles { get { return card.HasPermission(PermissionTypes.CanModifyStyles); } }
    }
}
