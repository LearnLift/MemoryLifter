using System;
using System.Collections.Generic;
using System.Text;
using SecurityFramework;
using Npgsql;
using System.Diagnostics;

namespace SecurityPgSqlAdapter
{



	internal class TypeDefinition
	{
		public int Id { get; set; }
		public string ClrName { get; set; }
		public int ParentId { get; set; }
	}
	internal class TypeDefinitionsAdapter : Dictionary<int, TypeDefinition>
	{
		private string _ConnectionString;
		public TypeDefinitionsAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
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
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT id, clr_name, parent_id FROM \"TypeDefinitions\";";
					NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd);

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
		private string _ConnectionString;
		public PermissionAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
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
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT id,name, types_id,\"Permissions\".default FROM \"Permissions\";";
					NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd);

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
	}
	internal class UserProfile
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public bool Enabled { get; set; }
		public UserAuthType Type { get; set; }
	}
	internal class UserProfilesAdapter : Dictionary<int, UserProfile>
	{
		private string _ConnectionString;
		public UserProfilesAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}
		public void Refresh()
		{
			this.Clear();

			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT id, username, enabled, user_type FROM \"UserProfiles\";";
					NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd);

					while (reader.Read())
					{
						UserProfile userProfile = new UserProfile();
						userProfile.Id = Convert.ToInt32(reader["id"]);
						userProfile.UserName = reader["username"].ToString();
						userProfile.Enabled = Convert.ToBoolean(reader["enabled"]);
						userProfile.Type = (UserAuthType)Enum.Parse(typeof(UserAuthType), reader["user_type"].ToString());
						this.Add(userProfile.Id, userProfile);
					}
				}
			}
		}

		public int AddUser(string account, string password, UserAuthType type)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					//CFI: Do not use CommandType.StoredProcedure as this will convert \ -> \\
					cmd.CommandText = "SELECT * FROM \"InsertUserProfile\"(:p_username, :p_password, :p_local_directory_id, :p_user_type)";
					cmd.Parameters.Add("p_username", account);
					cmd.Parameters.Add("p_password", password);
					cmd.Parameters.Add("p_local_directory_id", string.Empty);
					cmd.Parameters.Add("p_user_type", type.ToString());
					object result = cmd.ExecuteScalar();

					if (result == null || result is System.DBNull)
					{
						return -1;
					}
					Refresh();

					return Convert.ToInt32(result);
				}
			}
		}
		public void RemoveUser(int id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "DELETE From \"UserProfiles\" WHERE id =" + id;
					cmd.ExecuteNonQuery();
				}
			}
			Refresh();
		}
		public void UpdateUser(int userid, string name, string password, UserAuthType typ)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					string sCommand = "UPDATE \"UserProfiles\" ";
					sCommand = sCommand + " SET username='" + name + "',";
					if (typ == UserAuthType.ListAuthentication)
						sCommand = sCommand + " password='',";
					else
						sCommand = sCommand + " password=md5('" + password + "'),";
					sCommand = sCommand + " user_type='" + typ.ToString() + "'";
					sCommand = sCommand + " WHERE id=" + userid;
					cmd.CommandText = sCommand;

					cmd.ExecuteNonQuery();
				}
			}
			Refresh();
		}
	}
	internal class UserGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	internal class UserGroupsAdapter : Dictionary<int, UserGroup>
	{
		private string _ConnectionString;
		public UserGroupsAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}
		public void Refresh()
		{
			this.Clear();
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT id, name FROM \"UserGroups\";";
					NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd);

					while (reader.Read())
					{
						UserGroup userGroup = new UserGroup();
						userGroup.Id = Convert.ToInt32(reader["id"]);
						userGroup.Name = reader["name"].ToString();

						this.Add(userGroup.Id, userGroup);
					}
				}
			}
		}

		public int AddGroup(string name)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.CommandText = "\"InsertUserGroups\"";
					cmd.Parameters.Add("group_name", name);
					object result = cmd.ExecuteScalar();

					if (result == null || result is System.DBNull)
					{
						return -1;
					}
					Refresh();

					return Convert.ToInt32(result);
				}
			}
		}
		public void RemoveGroup(int id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "DELETE From \"UserGroups\" WHERE id =" + id;
					cmd.ExecuteNonQuery();
				}
			}
			Refresh();
		}
		public void UpdateGroup(int groupid, string name)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					string sCommand = "UPDATE \"UserGroups\" ";
					sCommand = sCommand + " SET name='" + name + "'";
					sCommand = sCommand + " WHERE id=" + groupid;
					cmd.CommandText = sCommand;

					cmd.ExecuteNonQuery();
				}
			}
			Refresh();
		}
	}
	internal class UserProfile_UserGroups
	{
		public int UserId { get; set; }
		public int GroupId { get; set; }
	}
	internal class UserProfiles_UserGroupsAdapter : List<UserProfile_UserGroups>
	{
		private string _ConnectionString;
		public UserProfiles_UserGroupsAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}
		public void Refresh()
		{
			this.Clear();

			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT users_id,groups_id FROM \"UserProfiles_UserGroups\";";
					NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd);

					while (reader.Read())
					{
						UserProfile_UserGroups entry = new UserProfile_UserGroups();
						entry.UserId = Convert.ToInt32(reader["users_id"]);
						entry.GroupId = Convert.ToInt32(reader["groups_id"]);
						this.Add(entry);

					}
				}
			}
		}
		public void RemoveEntriesByGroupId(int groupid)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "DELETE From \"UserProfiles_UserGroups\" WHERE groups_id =" + groupid;
					cmd.ExecuteNonQuery();
				}
			}

			Refresh();
		}
		public void RemoveEntriesByUserId(int userid)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					string sCommand = "DELETE FROM \"UserProfiles_UserGroups\" ";
					sCommand = sCommand + " WHERE users_id='" + userid.ToString() + "'";
					cmd.CommandText = sCommand;
					cmd.ExecuteNonQuery();
				}
			}

			Refresh();
		}
		public void AddEntry(int userid, int groupid)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.CommandText = "\"AddGroupToUser\"";
					cmd.Parameters.Add("uid", userid);
					cmd.Parameters.Add("gid", groupid);

					PostgreSQLConn.ExecuteNonQuery(cmd);
				}
			}
			Refresh();
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
		private string _ConnectionString;
		public AccessControlListAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}
		public int AddEntry(int object_id, int permission_id, bool access)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT \"InsertAclEntry\"(:object_id, :permission_id, :access);";
					cmd.Parameters.Add("object_id", object_id);
					cmd.Parameters.Add("permission_id", permission_id);
					cmd.Parameters.Add("access", access);
					object result = cmd.ExecuteScalar();
					return (result == null || result is System.DBNull) ? -1 : (int)result;
				}
			}
		}
		public void RemoveEntryById(int id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				//Lösche nun in der AccessControlList
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = string.Format("DELETE FROM \"AccessControlList\" WHERE id={0}", id);
					cmd.ExecuteNonQuery();
				}
			}
		}


	}

	internal class UserProfiles_AccessControlListEntry
	{
		public int UserId { get; set; }
		public int AclId { get; set; }
	}
	internal class UserProfiles_AccessControlListAdapter
	{
		private string _ConnectionString;
		public UserProfiles_AccessControlListAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}

		public void AddEntry(int user_id, int acl_id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					string sCommand = string.Format("INSERT INTO \"UserProfiles_AccessControlList\" (users_id,acl_id) VALUES({0},{1})",
						user_id, acl_id);

					cmd.CommandText = sCommand;
					cmd.ExecuteNonQuery();

				}
			}
		}
		public void RemoveEntry(int user_id, int acl_id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{


				//Lösche nun noch in der UserGroups_AccessControlList
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = string.Format("DELETE FROM \"UserProfiles_AccessControlList\" WHERE users_id={0} AND acl_id={1} ", user_id, acl_id);
					cmd.ExecuteNonQuery();
				}

			}
		}


	}
	internal class UserGroups_AccessControlListEntry
	{
		public int UserId { get; set; }
		public int AclId { get; set; }
	}
	internal class UserGroups_AccessControlListAdapter
	{
		private string _ConnectionString;
		public UserGroups_AccessControlListAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}
		public void AddEntry(int group_id, int acl_id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					string sCommand = string.Format("INSERT INTO \"UserGroups_AccessControlList\" (groups_id,acl_id) VALUES({0},{1})",
						group_id, acl_id);

					cmd.CommandText = sCommand;
					cmd.ExecuteNonQuery();

				}
			}
		}
		public void RemoveEntry(int group_id, int acl_id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{


				//Lösche nun noch in der UserGroups_AccessControlList
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = string.Format("DELETE FROM \"UserGroups_AccessControlList\" WHERE groups_id={0} AND acl_id={1} ", group_id, acl_id);
					cmd.ExecuteNonQuery();
				}

			}
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
		private string _ConnectionString;
		public ObjectListAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}
		public int AddEntry(string locator)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{

				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					string sCommand = string.Format("INSERT INTO \"ObjectList\" (locator,parent_id) VALUES('{0}','{1}') RETURNING id",
						locator, 0);

					cmd.CommandText = sCommand;
					object result = cmd.ExecuteScalar();
					if (result == null || result is System.DBNull)
					{
						return -1;
					}
					return (int)result;


				}
			}
		}
		public void RemoveEntry(int id)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{


				//Lösche nun noch in der UserGroups_AccessControlList
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = string.Format("DELETE FROM \"ObjectList\" WHERE id={0}", id);
					cmd.ExecuteNonQuery();
				}

			}
		}
		public int GetIdByLocator(string locator)
		{
			using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
			{


				//Lösche nun noch in der UserGroups_AccessControlList
				using (NpgsqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = string.Format("SELECT id FROM \"ObjectList\" WHERE locator='{0}'", locator);
					NpgsqlDataReader dr = cmd.ExecuteReader();

					object result = null;
					if (dr.Read())
						result = dr["id"];

					return (result == null || result is System.DBNull) ? -1 : (int)result;

				}

			}
		}
	}

	internal class DatabaseInformationAdapter : IDatabaseInformations
	{
		private string _ConnectionString;
		public DatabaseInformationAdapter(string connectionString)
		{
			_ConnectionString = connectionString;
		}

		#region IDatabaseInformations Members

		public string Version
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='Version';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='Version';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string SupportedDataLayerVersions
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='SupportedDataLayerVersions';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='SupportedDataLayerVersions';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public bool ListAuthentication
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='ListAuthentication';";
						cmd.CommandText = sCommand;
						return Convert.ToBoolean(cmd.ExecuteScalar());
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='ListAuthentication';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public bool FormsAuthentication
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='FormsAuthentication';";
						cmd.CommandText = sCommand;
						return Convert.ToBoolean(cmd.ExecuteScalar());
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='FormsAuthentication';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public bool LocalDirectoryAuthentication
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LocalDirectoryAuthentication';";
						cmd.CommandText = sCommand;
						return Convert.ToBoolean(cmd.ExecuteScalar());
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LocalDirectoryAuthentication';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string LocalDirectoryType
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LocalDirectoryType';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LocalDirectoryType';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string LdapServer
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LdapServer';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LdapServer';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string LdapPort
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LdapPort';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LdapPort';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string LdapContext
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LdapContext';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LdapContext';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string LdapUser
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LdapUser';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LdapUser';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public string LdapPassword
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LdapPassword';";
						cmd.CommandText = sCommand;
						return cmd.ExecuteScalar().ToString();
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LdapPassword';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public bool LdapUseSsl
		{
			get
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "SELECT value FROM \"DatabaseInformation\" WHERE property='LdapUseSsl';";
						cmd.CommandText = sCommand;
						return Convert.ToBoolean(cmd.ExecuteScalar());
					}
				}
			}
			set
			{
				using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(_ConnectionString))
				{
					using (NpgsqlCommand cmd = conn.CreateCommand())
					{
						string sCommand = "UPDATE \"DatabaseInformation\" SET value=:val WHERE property='LdapUseSsl';";
						cmd.CommandText = sCommand;
						cmd.Parameters.Add("val", value);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		#endregion
	}


	internal class RawAdapters
	{
		private string _ConnectionString;
		public RawAdapters(string connectionString)
		{
			_ConnectionString = connectionString;
			this.TypeDefinitionsAdapter = new TypeDefinitionsAdapter(_ConnectionString);
			this.PermissionAdatper = new PermissionAdapter(_ConnectionString);
			this.UserProfilesAdapter = new UserProfilesAdapter(_ConnectionString);
			this.UserGroupsAdapter = new UserGroupsAdapter(_ConnectionString);
			this.UserProfiles_UserGroupsAdapter = new UserProfiles_UserGroupsAdapter(_ConnectionString);
			this.ObjectListAdapter = new ObjectListAdapter(_ConnectionString);
			this.UserGroups_AccessControlListAdapter = new UserGroups_AccessControlListAdapter(_ConnectionString);
			this.UserProfiles_AccessControlListAdapter = new UserProfiles_AccessControlListAdapter(_ConnectionString);
			this.AccessControlListAdapter = new AccessControlListAdapter(_ConnectionString);
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
