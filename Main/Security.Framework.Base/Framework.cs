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
using System.Runtime.Serialization;
using System.IO;

namespace SecurityFramework
{
    public abstract class Framework
    {
        private ISecurityDataAdapter _DataAdapter;
        public ISecurityDataAdapter DataAdapter { get { return _DataAdapter; } }

        #region ctor
        public Framework(ISecurityDataAdapter dataAdapter)
        {
            _DataAdapter = dataAdapter;
        }
        #endregion



        #region Gruppen und User Verwaltung
        public List<User> Users
        {
            get
            {
                return _DataAdapter._Users;
            }
        }
        public List<Group> Groups
        {
            get
            {
                return _DataAdapter._Groups;
            }
        }
        public List<Role> Roles
        {
            get
            {
                List<Role> roles = new List<Role>();
                foreach (User user in Users) roles.Add(user);
                foreach (Group group in Groups) roles.Add(group);
                return roles;

            }
        }



        public User AddNewUser(string account, string password, UserAuthType type)
        {
            return _DataAdapter._AddUser(account, password, type);
        }
        public void RemoveUser(User user)
        {
            _DataAdapter._RemoveUser(user);
        }

        public void UpdateUser(User user)
        {
            _DataAdapter._UpdateUser(user);
        }

        public Group AddGroup(string name)
        {
            return _DataAdapter._AddGroup(name);

        }
        public void RemoveGroup(Group g)
        {
            _DataAdapter._RemoveGroup(g);
        }
        public void UpdateGroup(Group g)
        {
            _DataAdapter._UpdateGroup(g);
        }

        #endregion

        public List<TypeInfo> Types
        {
            get
            {
                return _DataAdapter._Types;
            }
        }
        public TypeInfo GetTypeInfoByTypeName(string typeName)
        {
            foreach (TypeInfo ti in Types)
                if (ti.Name == typeName)
                    return ti;
            return null;

        }

        public TypeInfo AddTypeInfo(string typeName)
        {
            return _DataAdapter._AddTypeInfo(typeName);
        }
        public void RemoveTypeInfo(TypeInfo ti)
        {
            _DataAdapter._RemoveTypeInfo(ti);
        }

        private Dictionary<string, bool> defaultPermissionTypeCache = new Dictionary<string, bool>();
        /// <summary>
        /// Gibt die konfigurierte Default Permission eine Typs zurück
        /// </summary>
        /// <param name="type"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public bool GetDefaultPermissionOfType(string type, string permissionName)
        {
            string key = type + "-" + permissionName;
            lock (defaultPermissionTypeCache)
            {
                if (!defaultPermissionTypeCache.ContainsKey(key))
                {
                    defaultPermissionTypeCache.Add(key, _DataAdapter._GetDefaultPermissionOfType(type, permissionName));
                }
                return defaultPermissionTypeCache[key];
            }
            //return _DataAdapter._GetDefaultPermissionOfType(type, permissionName);
        }



        public bool IsGroupTypePermissionInherited(string type, string permissionName, Group g)
        {

            bool ival = GetGroupTypePermissionInherited(type, permissionName, g);
            bool val = GetGroupTypePermission(type, permissionName, g);

            //forciere Cleaning
            SetGroupTypePermission(type, permissionName, val, g);

            return ival == val;
        }
        public bool IsUserTypePermissionInherited(string type, string permissionName, User user)
        {
            bool ival = GetUserTypePermissionInherited(type, permissionName, user);
            bool val = GetUserTypePermission(type, permissionName, user);

            //forciere Cleaning
            SetUserTypePermission(type, permissionName, val, user);

            return ival == val;
        }
        public bool IsGroupObjectPermissionInherited(object o, string permissionName, Group group)
        {

            bool ival = GetGroupObjectPermissionInherited(o, permissionName, group);
            bool val = GetGroupObjectPermission(o, permissionName, group);
            //forciere Cleaning
            SetGroupObjectTypePermission(o, permissionName, val, group);
            return ival == val;


        }
        public bool IsUserObjectPermissionInherited(object o, string permissionName, User user)
        {
            bool ival = GetUserObjectPermissionInherited(o, permissionName, user);
            bool val = GetUserObjectPermission(o, permissionName, user);
            //forciere Cleaning
            SetUserObjectTypePermission(o, permissionName, val, user);
            return ival == val;


        }




        public bool GetUserObjectPermission(object o, string permissionName, User user)
        {
            string type = o.GetType().FullName;
            object parent = GetParent(o);
            string locator = this.GetLocator(o);
            string parentLocator = null;
            if (locator != null && parent != null)
                parentLocator = this.GetLocator(parent);
            string parentType = null;
            if (parent != null)
                parentType = parent.GetType().FullName;

            //es existiertn ein direkter Eintrag in der Liste, also diesen Wert verwenden
            bool? access = _DataAdapter._GetPermissionOfUserObjectPermissionList(locator, type, permissionName, user.Id);
            if (access != null) return access.Value;


            return GetUserObjectPermissionInherited(o, permissionName, user);

        }
        private bool GetUserObjectPermissionInherited(object o, string permissionName, User user)
        {
            string type = o.GetType().FullName;
            object parent = GetParent(o);
            string locator = this.GetLocator(o);
            string parentLocator = null;
            if (locator != null && parent != null)
                parentLocator = this.GetLocator(parent);
            string parentType = null;
            if (parent != null)
                parentType = parent.GetType().FullName;

            bool? access = null;


            //Gibt es Einträge in den Gruppen, zu der der User gehört? Alle Werte der Gruppen werden OR - verknüpft,
            //d.h. ein Eintrag true, dann wird dieser zurückgegeben
            foreach (Group g in user.Groups)
            {
                bool? gp = _DataAdapter._GetPermissionOfGroupObjectPermissionList(locator, type, permissionName, g.Id);
                if (gp == null) continue;
                if (access == null)
                    access = gp;

                access = access | gp;
            }
            if (access != null) return access.Value;

            //Hat man über den ObjektParent einen entsprechende Berechtigung?
            if (parentLocator != null && parentType != null)
            {
                access = _DataAdapter._GetPermissionOfUserObjectPermissionList(parentLocator, parentType, permissionName, user.Id);
                if (access != null) return access.Value;
            }


            //Eventuell hat ein Gruppe über ObjektParent einen Eintrag?
            if (parentLocator != null && parentType != null)
            {
                foreach (Group g in user.Groups)
                {
                    bool? gp = _DataAdapter._GetPermissionOfGroupObjectPermissionList(parentLocator, parentType, permissionName, g.Id);
                    if (gp == null) continue;
                    if (access == null)
                        access = gp;

                    access = access | gp;
                }
                if (access != null) return access.Value;

            }


            //exisitiert ein Typeeintrag?
            access = _DataAdapter._GetPermissionOfUserTypePermissionList(type, permissionName, user.Id);
            if (access != null) return access.Value;


            //existiert ein Typeeintrag über die Gruppenzugehörigkeit?
            foreach (Group g in user.Groups)
            {
                bool? gp = _DataAdapter._GetPermissionOfGroupTypePermissionList(type, permissionName, g.Id);
                if (gp == null) continue;
                if (access == null)
                    access = gp;
                access = access | gp;
            }
            if (access != null) return access.Value;

            //Hat man über HierarchicalParent einen Eintrag?
            TypeInfo ti = GetTypeInfoByTypeName(type);

            string hierachicalParentType = (ti.HierachicalParent == null) ? null : ti.HierachicalParent.Name;

            //Hat man über den ObjektParent einen Eintrag?
            if (hierachicalParentType != null)
            {
                access = _DataAdapter._GetPermissionOfUserTypePermissionList(hierachicalParentType, permissionName, user.Id);
                if (access != null) return access.Value;
            }



            //Hat eventuell eine Gruppe über den ObjektParent einen Eintrag?
            if (hierachicalParentType != null)
            {
                foreach (Group g in user.Groups)
                {
                    bool? gp = _DataAdapter._GetPermissionOfGroupTypePermissionList(hierachicalParentType, permissionName, g.Id);
                    if (gp == null) continue;
                    if (access == null)
                        access = gp;
                    access = access | gp;
                }
                if (access != null) return access.Value;
            }



            //nun die Defaultpermission zurückgeben
            return _DataAdapter._GetDefaultPermissionOfType(type, permissionName);

        }

        private bool GetGroupObjectPermission(object o, string permission, Group group)
        {
            string locator = GetLocator(o);
            string type = o.GetType().FullName;
            object parent = GetParent(o);
            string parentLocator = null;
            if (parent != null)
                parentLocator = GetLocator(parent);
            string parentType = null;
            if (parent != null)
                parentType = parent.GetType().FullName;






            bool? access = _DataAdapter._GetPermissionOfGroupObjectPermissionList(locator, type, permission, group.Id);
            if (access != null) return access.Value;


            return GetGroupObjectPermissionInherited(o, permission, group);
        }
        private bool GetGroupObjectPermissionInherited(object o, string permission, Group group)
        {
            string locator = GetLocator(o);
            string type = o.GetType().FullName;
            object parent = GetParent(o);
            string parentLocator = null;
            if (parent != null)
                parentLocator = GetLocator(parent);
            string parentType = null;
            if (parent != null)
                parentType = parent.GetType().FullName;



            bool? access = null;

            //Wenn ein Object ein Parent, dann diese noch berücksichtigen = > aber nur eine Stufe!
            if (parent != null && parentLocator != null)
            {
                access = _DataAdapter._GetPermissionOfGroupObjectPermissionList(parentLocator, type, permission, group.Id);
                if (access != null) return access.Value;
            }

            //Group Type?
            access = _DataAdapter._GetPermissionOfGroupTypePermissionList(type, permission, group.Id);
            if (access != null) return access.Value;

            if (parentType != null)
            {
                access = _DataAdapter._GetPermissionOfGroupTypePermissionList(parentType, permission, group.Id);
                if (access != null) return access.Value;
            }

            return GetDefaultPermissionOfType(type, permission);
        }


        private bool GetUserTypePermission(string type, string permissionName, User user)
        {
            bool? access = null;

            access = _DataAdapter._GetPermissionOfUserTypePermissionList(type, permissionName, user.Id);
            if (access != null) return access.Value;

            return GetUserTypePermissionInherited(type, permissionName, user);


        }
        private bool GetUserTypePermissionInherited(string type, string permissionName, User user)
        {
            bool? access = null;





            //Gibt es Einträge in den Gruppen, zu der der User gehört?
            foreach (Group g in user.Groups)
            {
                bool? gp = _DataAdapter._GetPermissionOfGroupTypePermissionList(type, permissionName, g.Id);
                if (gp == null) continue;
                if (access == null)
                    access = gp;

                access = access | gp;
            }
            if (access != null) return access.Value;


            //Parent?
            TypeInfo ti = GetTypeInfoByTypeName(type);
            if (ti != null && ti.HierachicalParent != null)
            {
                string hierachicalParentType = ti.HierachicalParent.Name;
                access = _DataAdapter._GetPermissionOfUserTypePermissionList(hierachicalParentType, permissionName, user.Id);
            }
            if (access != null) return access.Value;

            //Parent Group
            if (ti != null && ti.HierachicalParent != null)
            {
                string hierachicalParentType = ti.HierachicalParent.Name;
                foreach (Group g in user.Groups)
                {
                    bool? gp = _DataAdapter._GetPermissionOfGroupTypePermissionList(hierachicalParentType, permissionName, g.Id);
                    if (gp == null) continue;
                    if (access == null)
                        access = gp;

                    access = access | gp;
                }

                if (access != null) return access.Value;
            }

            return GetDefaultPermissionOfType(type, permissionName);
        }

        private bool GetGroupTypePermission(string type, string permissionName, Group g)
        {

            bool? access = _DataAdapter._GetPermissionOfGroupTypePermissionList(type, permissionName, g.Id);
            if (access != null) return access.Value;


            return GetGroupTypePermissionInherited(type, permissionName, g);
        }
        private bool GetGroupTypePermissionInherited(string type, string permissionName, Group g)
        {

            bool? access = null;


            TypeInfo ti = GetTypeInfoByTypeName(type);
            if (ti != null && ti.HierachicalParent != null)
            {
                string hierachicalParentType = ti.HierachicalParent.Name;
                access = _DataAdapter._GetPermissionOfGroupTypePermissionList(hierachicalParentType, permissionName, g.Id);
            }
            if (access != null) return access.Value;


            return GetDefaultPermissionOfType(type, permissionName);
        }


        public List<PermissionInfo> GetUserObjectPermissions(object o, User user)
        {
            List<PermissionInfo> pis = new List<PermissionInfo>();
            string type = o.GetType().FullName;
            TypeInfo ti = GetTypeInfoByTypeName(type);
            foreach (PermissionInfo pi in ti.Permissions)
            {
                bool access = GetUserObjectPermission(o, pi.PermissionName, user);
                pis.Add(new PermissionInfo(pi.PermissionName, access));
            }
            return pis;
        }
        public List<PermissionInfo> GetGroupObjectPermissions(object o, Group group)
        {
            List<PermissionInfo> pis = new List<PermissionInfo>();
            string type = o.GetType().FullName;
            TypeInfo ti = GetTypeInfoByTypeName(type);
            foreach (PermissionInfo pi in ti.Permissions)
            {
                bool access = GetGroupObjectPermission(o, pi.PermissionName, group);
                pis.Add(new PermissionInfo(pi.PermissionName, access));
            }
            return pis;
        }
        public List<PermissionInfo> GetUserTypePermissions(string type, User user)
        {
            List<PermissionInfo> pis = new List<PermissionInfo>();
            TypeInfo ti = GetTypeInfoByTypeName(type);
            foreach (PermissionInfo pi in ti.Permissions)
            {
                bool access = GetUserTypePermission(type, pi.PermissionName, user);
                pis.Add(new PermissionInfo(pi.PermissionName, access));
            }
            return pis;
        }
        public List<PermissionInfo> GetGroupTypePermissions(string type, Group g)
        {
            List<PermissionInfo> pis = new List<PermissionInfo>();
            TypeInfo ti = GetTypeInfoByTypeName(type);
            foreach (PermissionInfo pi in ti.Permissions)
            {
                bool access = GetGroupTypePermission(type, pi.PermissionName, g);
                pis.Add(new PermissionInfo(pi.PermissionName, access));
            }
            return pis;
        }







        public void SetDefaultTypePermission(string type, string permissionName, bool access)
        {
            _DataAdapter._SetDefaultTypePermission(type, permissionName, access);
        }
        public void SetGroupTypePermission(string type, string permissionName, bool access, Group g)
        {
            //Wenn Gruppe mit Inherited übereinstimmt, dann Wert aus der Liste streichen
            if (GetGroupTypePermissionInherited(type, permissionName, g) == access)
            {
                _DataAdapter._RemoveGroupTypePermission(type, permissionName, g.Id);
            }
            else
            {
                if (GetGroupTypePermission(type, permissionName, g) != access)
                    //ansonsten hinzufügen
                    _DataAdapter._AddGroupTypePermission(type, permissionName, access, g.Id);
            }
        }
        public void SetUserTypePermission(string type, string permissionName, bool access, User user)
        {
            //Wenn durch Mitgliedgruppen vererbt, dann Wert aus der Liste streichen
            if (GetUserTypePermissionInherited(type, permissionName, user) == access)
            {
                _DataAdapter._RemoveUserTypePermission(type, permissionName, user.Id);
            }

            else
            {
                if (GetUserTypePermission(type, permissionName, user) != access)
                    //ansonsten hinzufügen
                    _DataAdapter._AddUserTypePermission(type, permissionName, access, user.Id);
            }

        }
        public void SetGroupObjectTypePermission(object o, string permissionName, bool access, Group g)
        {
            string locator = GetLocator(o);
            string type = o.GetType().FullName;
            //Wenn als Gruppe schon durch den Typ vererbt, dann braucht es den Eintrag nicht
            if (GetGroupObjectPermissionInherited(o, permissionName, g) == access)
            {
                _DataAdapter._RemoveGroupObjectPermission(locator, type, permissionName, g.Id);
            }
            else
            {
                if (GetGroupObjectPermission(o, permissionName, g) != access)
                    _DataAdapter._AddGroupObjectPermission(locator, type, permissionName, access, g.Id);
            }

        }
        public void SetUserObjectTypePermission(object o, string permissionName, bool access, User user)
        {


            string type = o.GetType().FullName;
            string locator = GetLocator(o);
            //Wenn schon durch einen MemberGruppe vererbt, dann braucht es den Eintrag nicht!
            if (GetUserObjectPermissionInherited(o, permissionName, user) == access)
            {
                _DataAdapter._RemoveUserObjectPermission(locator, type, permissionName, user.Id);
            }
            else
            {
                if (GetUserObjectPermission(o, permissionName, user) != access)
                    //ansonsten hinzufügen
                    _DataAdapter._AddUserObjectPermission(locator, type, permissionName, access, user.Id);
            }
        }



        public void ResetGroupTypePermission(string type, Group group)
        {
            _DataAdapter._ResetGroupTypePermission(type, group.Id);
        }
        public void ResetUserTypePermission(string type, User user)
        {
            _DataAdapter._ResetUserTypePermission(type, user.Id);
        }
        public void ResetGroupObjectPermission(object o, Group group)
        {
            string locator = GetLocator(o);
            string type = o.GetType().FullName;
            _DataAdapter._ResetGroupObjectPermission(locator, type, group.Id);
        }
        public void ResetUserObjectPermission(object o, User user)
        {
            string locator = GetLocator(o);
            string type = o.GetType().FullName;
            _DataAdapter._ResetUserObjectPermission(locator, type, user.Id);
        }

        public abstract string GetLocator(object o);
        public abstract object GetParent(object o);


        public SecurityToken CreateSecurityToken(string account)
        {
            User user = null;
            foreach (User u in Users)
            {
                if (u.Account.ToLower() == account.ToLower())
                {
                    user = u;
                    break;
                }
            }

            if (user == null)
                throw new AccountWrongException();

            return new SecurityToken(user, this);

        }




    }




}

