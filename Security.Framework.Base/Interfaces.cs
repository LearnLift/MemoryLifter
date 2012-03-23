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
using System.Collections.ObjectModel;

namespace SecurityFramework
{
    public interface ISession
    {
        int UserId { get; set; }
        string UserName { get; set; }
        DateTime LoginTime { get; set; }
        DateTime LogoutTime { get; set; }

    }
    public interface ISecurityDataAdapter
    {
        IDatabaseInformations DatabaseInformations { get; }

        List<TypeInfo> _Types { get; }
        List<User> _Users { get; set; }
        List<Group> _Groups { get; set; }
        List<ISession> Sessions{ get; set; }

        TypeInfo _AddTypeInfo(string typeName);
        void _RemoveTypeInfo(TypeInfo ti);
        PermissionInfo _AddPermissionToTypeInfo(TypeInfo ti, string permissionName);
        void _RemovePermissionToTypeInfo(TypeInfo ti, string permissionName);

        Group _AddGroup(string name);
        void _RemoveGroup(Group group);
        void _UpdateGroup(Group group);

        User _AddUser(string account, string password, UserAuthType type);
        void _RemoveUser(User user);
        void _UpdateUser(User user);

        bool _GetDefaultPermissionOfType(string type, string permissionName);
        bool? _GetPermissionOfGroupTypePermissionList(string type, string permissionName, string groupId);
        bool? _GetPermissionOfUserTypePermissionList(string type, string permissionName, string userId);
        bool? _GetPermissionOfGroupObjectPermissionList(string locator, string type, string permissionName, string groupId);
        bool? _GetPermissionOfUserObjectPermissionList(string locator, string type, string permissionName, string userId);

        void _SetDefaultTypePermission(string type, string permissionName, bool access);
        void _AddGroupTypePermission(string type, string permissionName, bool access, string groupId);
        void _RemoveGroupTypePermission(string type, string permissionName, string groupId);
        void _ResetGroupTypePermission(string type, string groupId);
        void _AddUserTypePermission(string type, string permissionName, bool access, string userId);
        void _RemoveUserTypePermission(string type, string permissionName, string userId);
        void _ResetUserTypePermission(string type, string userId);
        void _AddGroupObjectPermission(string locator, string type, string permissionName, bool access, string groupId);
        void _RemoveGroupObjectPermission(string locator, string type, string permissionName, string groupId);
        void _ResetGroupObjectPermission(string locator, string type, string groupId);
        void _AddUserObjectPermission(string locator, string type, string permissionName, bool access, string userId);
        void _RemoveUserObjectPermission(string locator, string type, string permissionName, string userId);
        void _ResetUserObjectPermission(string locator, string type, string userId);
        void CloseSession(int userId);
        void CloseAllSessions();
        List<ISession> GetOpenSessions();
    }

    public interface IDatabaseInformations
    {
        string Version { get; set; }
        string SupportedDataLayerVersions { get; set; }
        bool ListAuthentication { get; set; }
        bool FormsAuthentication { get; set; }
        bool LocalDirectoryAuthentication { get; set; }
        string LocalDirectoryType { get; set; }
        string LdapServer { get; set; }
        string LdapPort { get; set; }
        string LdapContext { get; set; }
        string LdapUser { get; set; }
        string LdapPassword { get; set; }
        bool LdapUseSsl { get; set; }
    }
}
