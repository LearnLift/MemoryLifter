using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityFramework
{
    public class SecurityToken
    {
        #region privates
        private Framework _Framework;
        private User _User;
        private bool _IsCaching = false;
        private Dictionary<object, List<PermissionInfo>> PermissionCache = new Dictionary<object, List<PermissionInfo>>();

        private bool HasPermissionFromCache(object o, string permissionName)
        {
            lock (this)
            {

                if (!PermissionCache.ContainsKey(o))
                {
                    List<PermissionInfo> list = GetPermissions(o);
                    PermissionCache.Add(o, list);
                }

                List<PermissionInfo> permissions = PermissionCache[o];

                PermissionInfo pi = null;
                foreach (PermissionInfo pinfo in permissions)
                {
                    if (pinfo.PermissionName == permissionName)
                    {
                        pi = pinfo;
                        break;
                    }
                }

                if (pi == null)
                {
                    return RawHasPermission(o, permissionName);
                    //throw new TypeOrPermissionNotFoundException();
                }
                return pi.Access;
            }
        }
        private bool RawHasPermission(object o, string permissionName)
        {
            lock (this)
            {


                return _Framework.GetUserObjectPermission(o, permissionName, _User);
            }
        }

        #endregion

        #region ctor
        internal SecurityToken(User user, Framework fw)
        {
            _User = user;
            _Framework = fw;
        }
        #endregion

        #region publics
        public User User
        {
            get
            {
                return _User;
            }
        }
        public bool IsCaching
        {
            get
            {
                return _IsCaching;
            }
            set
            {
                if (value == false)
                {
                    PermissionCache.Clear();
                }
                _IsCaching = value;
            }
        }
        public bool HasPermission(object o, string permissionName)
        {
            if (_IsCaching)
                return HasPermissionFromCache(o, permissionName);
            else
                return RawHasPermission(o, permissionName);
        }
        public List<PermissionInfo> GetPermissions(object o)
        {
            TypeInfo ti = _Framework.GetTypeInfoByTypeName(o.GetType().FullName);
            List<PermissionInfo> list = new List<PermissionInfo>();
            if (ti != null)
                foreach (PermissionInfo info in ti.Permissions)
                {
                    bool access = RawHasPermission(o, info.PermissionName);
                    PermissionInfo pi = new PermissionInfo(info.PermissionName, access);
                    list.Add(pi);
                }
            return list;
        }
        #endregion
    }
}
