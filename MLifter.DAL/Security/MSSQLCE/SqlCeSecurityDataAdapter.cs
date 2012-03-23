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
using SecurityFramework;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using System.Data.SqlServerCe;
using MLifter.DAL.DB.MsSqlCe;

namespace MLifter.Security.MsSqlCe
{
    /// <summary>
    /// Security adapter for SQL CE.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-02-27</remarks>
    internal class SqlCeSecurityDataAdapter : ISecurityDataAdapter, IDisposable
    {
        private IUser m_User;
        private RawAdapters Adapters;

        internal List<TypeInfo> __Types { get; private set; }
        internal List<User> __Users { get; set; }
        internal List<Group> __Groups { get; set; }


        private void Reset()
        {
            __Types = null;
            __Users = null;
            __Groups = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCeSecurityDataAdapter"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public SqlCeSecurityDataAdapter(IUser user)
        {
            this.m_User = user;
            Adapters = new RawAdapters(this.m_User);
        }

        #region ISecurityDataAdapter Members

        public IDatabaseInformations DatabaseInformations { get; private set; }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>The _ types.</value>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public List<TypeInfo> _Types
        {
            get
            {
                if (__Types == null)
                {
                    __Types = new List<TypeInfo>();
                    Adapters.TypeDefinitionsAdapter.Refresh();
                    Adapters.PermissionAdatper.Refresh();



                    Dictionary<int, TypeInfo> tempDic = new Dictionary<int, TypeInfo>();
                    foreach (TypeDefinition td in Adapters.TypeDefinitionsAdapter.Values)
                    {
                        TypeInfo info = new TypeInfo();
                        info.Name = td.ClrName;
                        tempDic.Add(td.Id, info);
                        __Types.Add(info);

                    }
                    foreach (TypeDefinition td in Adapters.TypeDefinitionsAdapter.Values)
                    {
                        TypeInfo ti = tempDic[td.Id];
                        TypeInfo pati = null;
                        if (tempDic.ContainsKey(td.ParentId))
                            pati = tempDic[td.ParentId];
                        ti.HierachicalParent = pati;
                    }

                    foreach (TypeDefinition td in Adapters.TypeDefinitionsAdapter.Values)
                    {
                        foreach (Permission pii in Adapters.PermissionAdatper.Values)
                        {
                            if (pii.TypeId == td.Id)
                            {
                                TypeInfo typeInfo = tempDic[pii.TypeId];
                                PermissionInfo pi = new PermissionInfo(pii.PermissionName, pii.DefaultAccess);
                                typeInfo.Permissions.Add(pi);
                            }
                        }
                    }

                }
                return __Types;
            }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The _ users.</value>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public List<User> _Users
        {
            get
            {
                if (__Users == null)
                {
                    __Users = new List<User>();

                    Adapters.UserProfilesAdapter.Refresh();
                    foreach (UserProfile up in Adapters.UserProfilesAdapter.Values)
                    {
                        User user = new User(up.UserName);
                        user.IsActivated = up.Enabled;
                        user.Id = up.Id.ToString();
                        __Users.Add(user);



                    }
                    foreach (User user in __Users)
                    {
                        Adapters.UserProfiles_UserGroupsAdapter.Refresh();
                        foreach (UserProfile_UserGroups upug in Adapters.UserProfiles_UserGroupsAdapter)
                        {
                            foreach (Group group in _Groups)
                            {
                                if (upug.UserId.ToString() == user.Id && group.Id == upug.GroupId.ToString())
                                    user.Groups.Add(group);
                            }

                        }
                    }


                }
                return __Users;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets groups.
        /// </summary>
        /// <value>The _ groups.</value>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public List<Group> _Groups
        {
            get
            {

                if (__Groups == null)
                {
                    __Groups = new List<Group>();
                    Adapters.UserGroupsAdapter.Refresh();
                    foreach (UserGroup userGroup in Adapters.UserGroupsAdapter.Values)
                    {
                        Group group = new Group(userGroup.Id.ToString(), userGroup.Name);
                        __Groups.Add(group);
                    }
                }
                return __Groups;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Adds group.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public Group _AddGroup(string name)
        {

            int groupid = Adapters.UserGroupsAdapter.AddGroup(name);
            Adapters.UserGroupsAdapter.Refresh();
            Reset();

            foreach (Group group in _Groups)
            {
                if (group.Id == groupid.ToString()) return group;
            }
            return null;

        }
        /// <summary>
        /// Removes group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _RemoveGroup(Group group)
        {
            //1. Löscht Gruppe aus UserGroups
            //2. Entfernt GruppeRelation zu User
            //3. Entfernt aus der TypACL alle Gruppeneinträge
            //4. Entfernt aus der ObjectACL alle Gruppeneinträge

            Adapters.UserGroupsAdapter.RemoveGroup(int.Parse(group.Id));
            Adapters.UserProfiles_UserGroupsAdapter.RemoveEntriesByGroupId(int.Parse(group.Id));
            Reset();
        }
        /// <summary>
        /// Updates group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _UpdateGroup(Group group)
        {
            Adapters.UserGroupsAdapter.UpdateGroup(int.Parse(group.Id), group.Name);
            Reset();
        }
		/// <summary>
		/// Adds a user.
		/// </summary>
		/// <param name="account">The account.</param>
		/// <param name="password">The password.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by AAB, 27.02.2009.
		/// </remarks>
        public User _AddUser(string account, string password, UserAuthType type)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }

        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _RemoveUser(User user)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _UpdateUser(User user)
        {
            Adapters.UserProfiles_UserGroupsAdapter.RemoveEntriesByUserId(int.Parse(user.Id));
            foreach (Group group in user.Groups)
            {
                Adapters.UserProfiles_UserGroupsAdapter.AddEntry(int.Parse(user.Id), int.Parse(group.Id));
            }

            Reset();
        }

        /// <summary>
        /// Wird nicht implementiert, diese Infos werden direkt in die Datenbank geschrieben
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public TypeInfo _AddTypeInfo(string typeName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// wird nicht implementiert
        /// </summary>
        /// <param name="ti"></param>
        public void _RemoveTypeInfo(TypeInfo ti)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// wird nicht implementiert, da direkt in de Datenbank geschrieben
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public PermissionInfo _AddPermissionToTypeInfo(TypeInfo ti, string permissionName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// wird nicht implementiert, da direkt in de Datenbank geschrieben
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public void _RemovePermissionToTypeInfo(TypeInfo ti, string permissionName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the default permission for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public bool _GetDefaultPermissionOfType(string type, string permissionName)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT pe.\"default\" FROM Permissions AS pe INNER JOIN TypeDefinitions AS td ON pe.types_id = td.id WHERE td.clr_name = @clr_name AND pe.name = @permission";
                cmd.Parameters.Add("@clr_name", type);
                cmd.Parameters.Add("@permission", permissionName);
                object result = cmd.ExecuteScalar();
                return (result == null || result is System.DBNull) ? false : (bool)result;
            }
        }

        /// <summary>
        /// Gets the group permission for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public bool? _GetPermissionOfGroupTypePermissionList(string type, string permissionName, string groupId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT al.access FROM \"AccessControlList\" AS al INNER JOIN \"UserGroups_AccessControlList\" AS ua ON ua.acl_id = al.id " +
                   "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                   string.Format("WHERE ua.groups_id = {0} AND pe.name = '{1}' AND ol.locator = 'DUMMYOBJECT' AND td.clr_name='{2}'", groupId, permissionName, type);

                object result = cmd.ExecuteScalar();
                return (result == null || result is System.DBNull) ? (bool?)null : (bool?)result;
            }
        }

        /// <summary>
        /// Gets the users permission for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public bool? _GetPermissionOfUserTypePermissionList(string type, string permissionName, string userId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT al.access FROM \"AccessControlList\" AS al INNER JOIN \"UserProfiles_AccessControlList\" AS ua ON ua.acl_id = al.id " +
                    "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                    "WHERE ua.users_id = @user_id AND pe.name = @permission AND ol.locator = 'DUMMYOBJECT' AND td.clr_name = @clr_name";
                cmd.Parameters.Add("@user_id", userId);
                cmd.Parameters.Add("@permission", permissionName);
                cmd.Parameters.Add("@clr_name", type);
                object result = cmd.ExecuteScalar();
                return (result == null || result is System.DBNull) ? (bool?)null : (bool?)result;
            }
        }

        /// <summary>
        /// Gets the groups permission for an object.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public bool? _GetPermissionOfGroupObjectPermissionList(string locator, string type, string permissionName, string groupId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT al.access FROM \"AccessControlList\" AS al INNER JOIN \"UserGroups_AccessControlList\" AS ua ON ua.acl_id = al.id " +
                   "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id  INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                   string.Format("WHERE ua.groups_id = {0} AND pe.name = '{1}' AND ol.locator = '{2}' AND td.clr_name='{3}'", groupId, permissionName, locator, type);

                object result = cmd.ExecuteScalar();
                return (result == null || result is System.DBNull) ? (bool?)null : (bool?)result;
            }
        }

        /// <summary>
        /// Gets the users permission for an object.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public bool? _GetPermissionOfUserObjectPermissionList(string locator, string type, string permissionName, string userId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT al.access FROM \"AccessControlList\" AS al INNER JOIN \"UserProfiles_AccessControlList\" AS ua ON ua.acl_id = al.id " +
                   "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id  INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                   string.Format("WHERE ua.users_id = {0} AND pe.name = '{1}' AND ol.locator = '{2}' AND td.clr_name='{3}'", userId, permissionName, locator, type);

                object result = cmd.ExecuteScalar();
                return (result == null || result is System.DBNull) ? (bool?)null : (bool?)result;
            }
        }

        /// <summary>
        /// Wird nicht implementiert, da direkt in der Datenbank gesetzt
        /// </summary>
        /// <param name="type"></param>
        /// <param name="permissionName"></param>
        /// <param name="access"></param>
        public void _SetDefaultTypePermission(string type, string permissionName, bool access)
        {
            throw new NotImplementedException();
        }


        private int FindAC_ID_Object(string type, string object_locator, string permissionName, string roleId, bool bIsGroup)
        {
            string XXX_AccessControlList;
            string XXX_RoleId;
            int ac_id;
            if (bIsGroup)
            {
                XXX_AccessControlList = "UserGroups_AccessControlList";
                XXX_RoleId = "groups_id";
            }
            else
            {
                XXX_AccessControlList = "UserProfiles_AccessControlList";
                XXX_RoleId = "users_id";
            }
            //Suche entsprechende ac_id
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText =
                  string.Format("SELECT al.id FROM \"AccessControlList\" AS al INNER JOIN \"{0}\" AS ua ON ua.acl_id = al.id ", XXX_AccessControlList) +
                  "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                  string.Format("WHERE ua.{0} = {1} AND pe.name = '{2}' AND ol.locator = '{3}' AND td.clr_name='{4}'",
                  XXX_RoleId, roleId, permissionName, object_locator, type);

                object result = cmd.ExecuteScalar();
                ac_id = (result == null || result is System.DBNull) ? -1 : (int)result;

            }
            return ac_id;
        }
        private int FindObject_ID(string type, string object_locator, string permissionName, string roleId, bool bIsGroup)
        {
            string XXX_AccessControlList;
            string XXX_RoleId;
            int ac_id;
            if (bIsGroup)
            {
                XXX_AccessControlList = "UserGroups_AccessControlList";
                XXX_RoleId = "groups_id";
            }
            else
            {
                XXX_AccessControlList = "UserProfiles_AccessControlList";
                XXX_RoleId = "users_id";
            }
            //Suche entsprechende ac_id
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText =
                  string.Format("SELECT al.object_id FROM \"AccessControlList\" AS al INNER JOIN \"{0}\" AS ua ON ua.acl_id = al.id ", XXX_AccessControlList) +
                  "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                  string.Format("WHERE ua.{0} = {1} AND pe.name = '{2}' AND ol.locator = '{3}' AND td.clr_name='{4}'",
                  XXX_RoleId, roleId, permissionName, object_locator, type);

                object result = cmd.ExecuteScalar();
                ac_id = (result == null || result is System.DBNull) ? -1 : (int)result;

            }
            return ac_id;
        }


        private int FindAC_ID_Type(string type, string permissionName, string roleId, bool bIsGroup)
        {
            string XXX_AccessControlList;
            string XXX_RoleId;
            int ac_id;
            if (bIsGroup)
            {
                XXX_AccessControlList = "UserGroups_AccessControlList";
                XXX_RoleId = "groups_id";
            }
            else
            {
                XXX_AccessControlList = "UserProfiles_AccessControlList";
                XXX_RoleId = "users_id";
            }
            //Suche entsprechende ac_id
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText =
                  string.Format("SELECT al.id FROM \"AccessControlList\" AS al INNER JOIN \"{0}\" AS ua ON ua.acl_id = al.id ", XXX_AccessControlList) +
                  "INNER JOIN \"Permissions\" AS pe ON al.permissions_id = pe.id INNER JOIN \"TypeDefinitions\" AS td ON pe.types_id = td.id INNER JOIN \"ObjectList\" AS ol ON ol.id=al.object_id " +
                  string.Format("WHERE ua.{0} = {1} AND pe.name = '{2}' AND td.clr_name='{3}' AND ol.locator='DUMMYOBJECT'",
                  XXX_RoleId, roleId, permissionName, type);


                object result = cmd.ExecuteScalar();
                ac_id = (result == null || result is System.DBNull) ? -1 : (int)result;
                //ac_id = (int)cmd.ExecuteScalar();
            }
            return ac_id;
        }
        /// <summary>
        /// Add permission for a group.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="access">if set to <c>true</c> [access].</param>
        /// <param name="groupId">The group id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _AddGroupTypePermission(string type, string permissionName, bool access, string groupId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);
            int object_id = 0;
            int permission_id = Adapters.PermissionAdatper.GetIdByTypeIdAndPermissionName(type_id, permissionName);
            int group_id = int.Parse(groupId);
            //1. Eintrag in die AccessControlList => zurück kommt die Id


            int ac_id = Adapters.AccessControlListAdapter.AddEntry(object_id, permission_id, access);

            //2. Eintrag in die UserGroups_AccessControlList mit Angabe dieser Id
            Adapters.UserGroups_AccessControlListAdapter.AddEntry(group_id, ac_id);

        }
        /// <summary>
        /// Remove a permission for a group.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="groupId">The group id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _RemoveGroupTypePermission(string type, string permissionName, string groupId)
        {
            int ac_id = -1;
            int group_id = int.Parse(groupId);

            ac_id = FindAC_ID_Type(type, permissionName, groupId, true);

            Adapters.AccessControlListAdapter.RemoveEntryById(ac_id);
            Adapters.UserGroups_AccessControlListAdapter.RemoveEntry(group_id, ac_id);
        }

        /// <summary>
        /// Add permission for a user.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="access">if set to <c>true</c> [access].</param>
        /// <param name="userId">The user id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _AddUserTypePermission(string type, string permissionName, bool access, string userId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);
            int object_id = 0;
            int permission_id = Adapters.PermissionAdatper.GetIdByTypeIdAndPermissionName(type_id, permissionName);
            int user_id = int.Parse(userId);
            //1. Eintrag in die AccessControlList => zurück kommt die Id


            int ac_id = Adapters.AccessControlListAdapter.AddEntry(object_id, permission_id, access);

            //2. Eintrag in die UserGroups_AccessControlList mit Angabe dieser Id
            Adapters.UserProfiles_AccessControlListAdapter.AddEntry(user_id, ac_id);
        }

        /// <summary>
        /// Remove permission for a user.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="userId">The user id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _RemoveUserTypePermission(string type, string permissionName, string userId)
        {
            int ac_id = -1;
            int user_id = int.Parse(userId);

            ac_id = FindAC_ID_Type(type, permissionName, userId, false);
            //Lösche nun in der AccessControlList
            Adapters.AccessControlListAdapter.RemoveEntryById(ac_id);
            Adapters.UserProfiles_AccessControlListAdapter.RemoveEntry(user_id, ac_id);

        }

        /// <summary>
        /// Add permission for an object to a group.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="access">if set to <c>true</c> [access].</param>
        /// <param name="groupId">The group id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _AddGroupObjectPermission(string locator, string type, string permissionName, bool access, string groupId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);

            int permission_id = Adapters.PermissionAdatper.GetIdByTypeIdAndPermissionName(type_id, permissionName);
            int group_id = int.Parse(groupId);

            //1. Eintrag in die ObjectList
            int object_id = Adapters.ObjectListAdapter.AddEntry(locator);

            //1. Eintrag in die AccessControlList => zurück kommt die Id


            int ac_id = Adapters.AccessControlListAdapter.AddEntry(object_id, permission_id, access);

            //2. Eintrag in die UserGroups_AccessControlList mit Angabe dieser Id
            Adapters.UserGroups_AccessControlListAdapter.AddEntry(group_id, ac_id);
        }

        /// <summary>
        /// Removes an object permission for a group.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="groupId">The group id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _RemoveGroupObjectPermission(string locator, string type, string permissionName, string groupId)
        {
            int ac_id = -1;
            int group_id = int.Parse(groupId);


            ac_id = FindAC_ID_Object(type, locator, permissionName, groupId, true);
            int object_id = FindObject_ID(type, locator, permissionName, groupId, true);

            //Lösche nun in der AccessControlList
            Adapters.ObjectListAdapter.RemoveEntry(object_id);
            Adapters.AccessControlListAdapter.RemoveEntryById(ac_id);
            Adapters.UserGroups_AccessControlListAdapter.RemoveEntry(group_id, ac_id);
        }

        /// <summary>
        /// Adds an object permission for a user.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="access">if set to <c>true</c> [access].</param>
        /// <param name="userId">The user id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _AddUserObjectPermission(string locator, string type, string permissionName, bool access, string userId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);
            int permission_id = Adapters.PermissionAdatper.GetIdByTypeIdAndPermissionName(type_id, permissionName);
            int user_id = int.Parse(userId);
            //1. Eintrag in die ObjectList

            int object_id = Adapters.ObjectListAdapter.AddEntry(locator);


            //1. Eintrag in die AccessControlList => zurück kommt die Id


            int ac_id = Adapters.AccessControlListAdapter.AddEntry(object_id, permission_id, access);

            //2. Eintrag in die UserGroups_AccessControlList mit Angabe dieser Id
            Adapters.UserProfiles_AccessControlListAdapter.AddEntry(user_id, ac_id);
        }

        /// <summary>
        /// Removes a users object permissions.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <param name="userId">The user id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _RemoveUserObjectPermission(string locator, string type, string permissionName, string userId)
        {
            int ac_id = -1;
            int user_id = int.Parse(userId);

            ac_id = FindAC_ID_Object(type, locator, permissionName, userId, false);
            int object_id = FindObject_ID(type, locator, permissionName, userId, false);

            //Lösche nun in der AccessControlList
            Adapters.ObjectListAdapter.RemoveEntry(object_id);
            Adapters.AccessControlListAdapter.RemoveEntryById(ac_id);
            Adapters.UserProfiles_AccessControlListAdapter.RemoveEntry(user_id, ac_id);
        }

        /// <summary>
        /// Resets a groups permission for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="groupId">The group id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _ResetGroupTypePermission(string type, string groupId)
        {

            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);

            //suche alle acl_id Einträge in UserGroups_AccessControlList mit groups_id
            //lösche alle Einträge in der AccessControlList mit id == acl_id eines Typs
            int group_id = int.Parse(groupId);

            List<int> acl_ids = new List<int>();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT ugacl.acl_id FROM \"UserGroups_AccessControlList\" as ugacl " +
                                  "INNER JOIN \"AccessControlList\" AS acl ON acl.id=ugacl.acl_id " +
                                  "INNER JOIN \"Permissions\"  AS pe ON pe.id=acl.permissions_id " +
                                  "INNER JOIN \"ObjectList\" AS ol ON ol.id=acl.object_id " +
                                  "WHERE ugacl.groups_id=@groups_id AND pe.types_id=@type_id AND ol.locator='DUMMYOBJECT'";
                cmd.Parameters.Add("@groups_id", group_id);
                cmd.Parameters.Add("@type_id", type_id);
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        acl_ids.Add((int)dr["acl_id"]);
                    }
                }
            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.UserGroups_AccessControlListAdapter.RemoveEntry(group_id, acl_id);
            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.AccessControlListAdapter.RemoveEntryById(acl_id);
            }
        }

        /// <summary>
        /// Resets a users permission for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="userId">The user id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _ResetUserTypePermission(string type, string userId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);

            //suche alle acl_id Einträge in UserGroups_AccessControlList mit groups_id
            //lösche alle Einträge in der AccessControlList mit id == acl_id eines Typs
            int user_id = int.Parse(userId);

            List<int> acl_ids = new List<int>();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT upacl.acl_id FROM \"UserProfiles_AccessControlList\" as upacl " +
                                 "INNER JOIN \"AccessControlList\" AS acl ON acl.id=upacl.acl_id " +
                                 "INNER JOIN \"Permissions\"  AS pe ON pe.id=acl.permissions_id " +
                                 "INNER JOIN \"ObjectList\" AS ol ON ol.id=acl.object_id " +
                                 "WHERE upacl.users_id=@users_id AND pe.types_id=@type_id AND ol.locator='DUMMYOBJECT'";
                cmd.Parameters.Add("@users_id", user_id);
                cmd.Parameters.Add("@type_id", type_id);
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        acl_ids.Add((int)dr["acl_id"]);
                    }
                }
            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.UserProfiles_AccessControlListAdapter.RemoveEntry(user_id, acl_id);

            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.AccessControlListAdapter.RemoveEntryById(acl_id);
            }
        }

        /// <summary>
        /// Resets a groups permission for an object.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="groupId">The group id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _ResetGroupObjectPermission(string locator, string type, string groupId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);

            //suche alle acl_id Einträge in UserGroups_AccessControlList mit groups_id
            //lösche alle Einträge in der AccessControlList mit id == acl_id eines Typs
            int group_id = int.Parse(groupId);

            List<int> acl_ids = new List<int>();
            List<int> object_ids = new List<int>();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT ugacl.acl_id,acl.object_id FROM \"UserGroups_AccessControlList\" as ugacl " +
                                  "INNER JOIN \"AccessControlList\" AS acl ON acl.id=ugacl.acl_id " +
                                  "INNER JOIN \"Permissions\"  AS pe ON pe.id=acl.permissions_id " +
                                  "INNER JOIN \"ObjectList\" AS ol ON ol.id=acl.object_id " +
                                  "WHERE ugacl.groups_id=@groups_id AND pe.types_id=@type_id AND ol.locator=@locator";
                cmd.Parameters.Add("@groups_id", group_id);
                cmd.Parameters.Add("@type_id", type_id);
                cmd.Parameters.Add("@locator", locator);
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        acl_ids.Add((int)dr["acl_id"]);
                        object_ids.Add((int)dr["object_id"]);
                    }
                }
            }
            foreach (int object_id in object_ids)
            {
                Adapters.ObjectListAdapter.RemoveEntry(object_id);
            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.UserGroups_AccessControlListAdapter.RemoveEntry(group_id, acl_id);

            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.AccessControlListAdapter.RemoveEntryById(acl_id);

            }
        }

        /// <summary>
        /// Resets the users permission for an object.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="type">The type.</param>
        /// <param name="userId">The user id.</param>
        /// <remarks>Documented by Dev03, 2009-02-27</remarks>
        public void _ResetUserObjectPermission(string locator, string type, string userId)
        {
            int type_id = Adapters.TypeDefinitionsAdapter.GetIdByTypeName(type);

            //suche alle acl_id Einträge in UserGroups_AccessControlList mit groups_id
            //lösche alle Einträge in der AccessControlList mit id == acl_id eines Typs
            int user_id = int.Parse(userId);

            List<int> acl_ids = new List<int>();
            List<int> object_ids = new List<int>();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT upacl.acl_id,acl.object_id FROM \"UserProfiles_AccessControlList\" as upacl " +
                                  "INNER JOIN \"AccessControlList\" AS acl ON acl.id=upacl.acl_id " +
                                  "INNER JOIN \"Permissions\"  AS pe ON pe.id=acl.permissions_id " +
                                  "INNER JOIN \"ObjectList\" AS ol ON ol.id=acl.object_id " +
                                  "WHERE upacl.users_id=@users_id AND pe.types_id=@type_id AND ol.locator=@locator";
                cmd.Parameters.Add("@users_id", user_id);
                cmd.Parameters.Add("@type_id", type_id);
                cmd.Parameters.Add("@locator", locator);
                using (SqlCeDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        acl_ids.Add((int)dr["acl_id"]);
                        object_ids.Add((int)dr["object_id"]);
                    }
                }
            }
            foreach (int object_id in object_ids)
            {
                Adapters.ObjectListAdapter.RemoveEntry(object_id);
            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.UserProfiles_AccessControlListAdapter.RemoveEntry(user_id, acl_id);

            }
            foreach (int acl_id in acl_ids)
            {
                Adapters.AccessControlListAdapter.RemoveEntryById(acl_id);


            }
        }

        public List<ISession> Sessions
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void FlushCalForLM(int lmId)
        {
            throw new NotImplementedException();
        }

        public void FlushCal()
        {
            throw new NotImplementedException();
        }

        public void CloseSession(int userId)
        {
            throw new NotImplementedException();
        }

        public void CloseAllSessions()
        {
            throw new NotImplementedException();
        }

        public List<ISession> GetOpenSessions()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Debug.WriteLine("Disposing...");
        }

        #endregion
    }
}
