using System;
using System.Collections.Generic;
using System.Text;
using SecurityFramework;
using System.Data.SqlServerCe;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.DB.MsSqlCe;

namespace MLifter.Security.MsSqlCe
{
    internal class TypeDefinition
    {
        public int Id { get; set; }
        public string ClrName { get; set; }
        public int ParentId { get; set; }
    }

    internal class TypeDefinitionsAdapter : Dictionary<int, TypeDefinition>
    {
        private IUser m_User;
        public TypeDefinitionsAdapter(IUser user)
        {
            m_User = user;
        }
        public int GetIdByTypeName(string typeName)
        {
            foreach (TypeDefinition td in this.Values)
            {
                if (td.ClrName == typeName) return td.Id;
            }
            return -1;
        }
        public void Refresh()
        {
            this.Clear();
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT id, clr_name, parent_id FROM TypeDefinitions";
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    TypeDefinition info = new TypeDefinition();
                    info.Id = Convert.ToInt32(reader["id"]);
                    info.ClrName = reader["clr_name"].ToString();

                    object result = reader["parent_id"];
                    info.ParentId = (result is DBNull || result == null) ? -1 : Convert.ToInt32(result);


                    this.Add(info.Id, info);
                }
            }
        }
    }
    internal class Permission
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string PermissionName { get; set; }
        public bool DefaultAccess { get; set; }
    }
    internal class PermissionAdapter : Dictionary<int, Permission>
    {
        private IUser m_User;
        public PermissionAdapter(IUser user)
        {
            m_User = user;
        }
        public int GetIdByTypeIdAndPermissionName(int type_id, string permissionName)
        {
            foreach (Permission p in this.Values)
            {
                if (p.TypeId == type_id && p.PermissionName == permissionName)
                    return p.Id;
            }
            return -1;
        }

        public void Refresh()
        {
            this.Clear();
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT id,name, types_id,\"default\" FROM Permissions";
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Permission info = new Permission();
                    info.Id = Convert.ToInt32(reader["id"]);
                    info.PermissionName = reader["name"].ToString();
                    info.TypeId = Convert.ToInt32(reader["types_id"]);
                    info.DefaultAccess = Convert.ToBoolean(reader["default"]);
                    this.Add(info.Id, info);
                }
            }
        }
    }
    internal class UserProfile
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool Enabled { get; set; }
    }
    internal class UserProfilesAdapter : Dictionary<int, UserProfile>
    {
        private IUser m_User;
        public UserProfilesAdapter(IUser user)
        {
            m_User = user;
        }
        public void Refresh()
        {
            this.Clear();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT id, username, enabled FROM UserProfiles";
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    UserProfile userProfile = new UserProfile();
                    userProfile.Id = Convert.ToInt32(reader["id"]);
                    userProfile.UserName = reader["username"].ToString();
                    userProfile.Enabled = Convert.ToBoolean(reader["enabled"]);

                    this.Add(userProfile.Id, userProfile);
                }
            }
        }
    }
    internal class UserGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    internal class UserGroupsAdapter : Dictionary<int, UserGroup>
    {
        private IUser m_User;
        public UserGroupsAdapter(IUser user)
        {
            m_User = user;
        }
        public void Refresh()
        {
            this.Clear();
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT id, name FROM UserGroups";
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    UserGroup userGroup = new UserGroup();
                    userGroup.Id = Convert.ToInt32(reader["id"]);
                    userGroup.Name = reader["name"].ToString();

                    this.Add(userGroup.Id, userGroup);
                }
            }
        }

        public int AddGroup(string name)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void RemoveGroup(int id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void UpdateGroup(int groupid, string name)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
    }
    internal class UserProfile_UserGroups
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
    internal class UserProfiles_UserGroupsAdapter : List<UserProfile_UserGroups>
    {
        private IUser m_User;
        public UserProfiles_UserGroupsAdapter(IUser user)
        {
            m_User = user;
        }
        public void Refresh()
        {
            this.Clear();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = "SELECT users_id,groups_id FROM UserProfiles_UserGroups";
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    UserProfile_UserGroups entry = new UserProfile_UserGroups();
                    entry.UserId = Convert.ToInt32(reader["users_id"]);
                    entry.GroupId = Convert.ToInt32(reader["groups_id"]);
                    this.Add(entry);
                }
            }
        }
        public void RemoveEntriesByGroupId(int groupid)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void RemoveEntriesByUserId(int userid)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void AddEntry(int userid, int groupid)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
    }
    internal class AccessControlListEntry
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int PermissionId { get; set; }
        public bool Access { get; set; }
    }
    internal class AccessControlListAdapter
    {
        private IUser m_User;
        public AccessControlListAdapter(IUser user)
        {
            m_User = user;
        }
        public int AddEntry(int object_id, int permission_id, bool access)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void RemoveEntryById(int id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }


    }

    internal class UserProfiles_AccessControlListEntry
    {
        public int UserId { get; set; }
        public int AclId { get; set; }
    }
    internal class UserProfiles_AccessControlListAdapter
    {
        private IUser m_User;
        public UserProfiles_AccessControlListAdapter(IUser user)
        {
            m_User = user;
        }

        public void AddEntry(int user_id, int acl_id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void RemoveEntry(int user_id, int acl_id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }


    }
    internal class UserGroups_AccessControlListEntry
    {
        public int UserId { get; set; }
        public int AclId { get; set; }
    }
    internal class UserGroups_AccessControlListAdapter
    {
        private IUser m_User;
        public UserGroups_AccessControlListAdapter(IUser user)
        {
            m_User = user;
        }
        public void AddEntry(int group_id, int acl_id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void RemoveEntry(int group_id, int acl_id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
    }

    internal class ObjectListEntry
    {
        public int Id { get; set; }
        public string Locator { get; set; }
        public int ParentId { get; set; }
    }
    internal class ObjectListAdapter
    {
        private IUser m_User;
        public ObjectListAdapter(IUser user)
        {
            m_User = user;
        }
        public int AddEntry(string locator)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public void RemoveEntry(int id)
        {
            throw new NotSupportedException("Not supported for SQL CE!");
        }
        public int GetIdByLocator(string locator)
        {
            //Lösche nun noch in der UserGroups_AccessControlList
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(m_User))
            {
                cmd.CommandText = string.Format("SELECT id FROM ObjectList WHERE locator='{0}'", locator);
                object result = null;
                using (SqlCeDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        result = reader["id"];
                }
                return (result == null || result is System.DBNull) ? -1 : (int)result;
            }
        }
    }


    internal class RawAdapters
    {
        private IUser m_User;
        public RawAdapters(IUser user)
        {
            m_User = user;
            this.TypeDefinitionsAdapter = new TypeDefinitionsAdapter(m_User);
            this.PermissionAdatper = new PermissionAdapter(m_User);
            this.UserProfilesAdapter = new UserProfilesAdapter(m_User);
            this.UserGroupsAdapter = new UserGroupsAdapter(m_User);
            this.UserProfiles_UserGroupsAdapter = new UserProfiles_UserGroupsAdapter(m_User);
            this.ObjectListAdapter = new ObjectListAdapter(m_User);
            this.UserGroups_AccessControlListAdapter = new UserGroups_AccessControlListAdapter(m_User);
            this.UserProfiles_AccessControlListAdapter = new UserProfiles_AccessControlListAdapter(m_User);
            this.AccessControlListAdapter = new AccessControlListAdapter(m_User);
        }

        public TypeDefinitionsAdapter TypeDefinitionsAdapter { get; set; }
        public PermissionAdapter PermissionAdatper { get; set; }
        public UserProfilesAdapter UserProfilesAdapter { get; set; }
        public UserGroupsAdapter UserGroupsAdapter { get; set; }
        public UserProfiles_UserGroupsAdapter UserProfiles_UserGroupsAdapter { get; set; }
        public AccessControlListAdapter AccessControlListAdapter { get; set; }
        public ObjectListAdapter ObjectListAdapter { get; set; }
        public UserProfiles_AccessControlListAdapter UserProfiles_AccessControlListAdapter { get; set; }
        public UserGroups_AccessControlListAdapter UserGroups_AccessControlListAdapter { get; set; }
    }
}
