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
using System.Data.SqlServerCe;
using System.IO;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Properties;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    class MsSqlCeDictionariesConnector : IDbDictionariesConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeDictionariesConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeDictionariesConnector>();
        public static MsSqlCeDictionariesConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new MsSqlCeDictionariesConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private MsSqlCeDictionariesConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbDictionariesConnector Members

        public string GetDbVersion()
        {
            string version = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
            if (version != null && version.Length > 0)
                return version;

            MsSqlCeDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DataBaseVersion, 0)] as string;
        }

        public IList<int> GetLMIds()
        {
            List<int> ids = new List<int>();
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT id FROM LearningModules";
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
            while (reader.Read())
                ids.Add(Convert.ToInt32(reader["id"]));
            reader.Close();
            return ids;
        }

        public void DeleteLM(int id)
        {
            throw new NotImplementedException("Not possible here! This is done in DbDictionaries.Delete()!");
        }

        public int AddNewLM(string guid, int categoryId, string title, string licenceKey, bool contentProtected, int calCount)
        {
            if (!Parent.CurrentUser.ConnectionString.ConnectionString.EndsWith(Helper.EmbeddedDbExtension))
            {
                ConnectionStringStruct css = Parent.CurrentUser.ConnectionString;
                css.ConnectionString += @"\" + title.Replace(@"\", "_") + Helper.EmbeddedDbExtension;
                css.Typ = DatabaseType.MsSqlCe;
                Parent.CurrentUser.ConnectionString = css;
            }
            {
                //replace invalid filename characters
                ConnectionStringStruct css = Parent.CurrentUser.ConnectionString;
                css.ConnectionString = Helper.FilterInvalidFilenameCharacters(css.ConnectionString);
                Parent.CurrentUser.ConnectionString = css;
            }
            if (File.Exists(Parent.CurrentUser.ConnectionString.ConnectionString))
            {
                int i = 1;
                while (File.Exists(Parent.CurrentUser.ConnectionString.ConnectionString.Replace(Helper.EmbeddedDbExtension, "_" + i + Helper.EmbeddedDbExtension))) i++;

                ConnectionStringStruct css = Parent.CurrentUser.ConnectionString;
                css.ConnectionString = css.ConnectionString.Replace(Helper.EmbeddedDbExtension, "_" + i + Helper.EmbeddedDbExtension);
                Parent.CurrentUser.ConnectionString = css;
            }
            if (!Directory.Exists(Path.GetDirectoryName(Parent.CurrentUser.ConnectionString.ConnectionString)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Parent.CurrentUser.ConnectionString.ConnectionString));
            }
            using (SqlCeEngine clientEngine = new SqlCeEngine("Data Source=" + Parent.CurrentUser.ConnectionString.ConnectionString))
            {
                clientEngine.CreateDatabase();
                clientEngine.Dispose();
            }
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = Resources.MsSqlCeDbCreateScript;
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }
            MSSQLCEConn.ApplyIndicesToDatabase(MSSQLCEConn.GetConnection(Parent.CurrentUser));
            int cat_id;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id FROM Categories WHERE global_id=@cat_id;";
                cmd.Parameters.Add("@cat_id", categoryId);
                cat_id = MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
            }
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO LearningModules (guid, title, categories_id, default_settings_id, allowed_settings_id, licence_key, content_protected, cal_count) " +
                    "VALUES (@guid, @title, @cat_id, @dset, @aset, @lk, @cp, @cals); SELECT @@IDENTITY;";
                cmd.Parameters.Add("@guid", guid);
                cmd.Parameters.Add("@title", title);
                cmd.Parameters.Add("@cat_id", cat_id);
                cmd.Parameters.Add("@lk", licenceKey);
                cmd.Parameters.Add("@cp", contentProtected);
                cmd.Parameters.Add("@cals", calCount);
                cmd.Parameters.Add("@dset", MsSqlCeSettingsConnector.CreateNewDefaultSettings(Parent));
                cmd.Parameters.Add("@aset", MsSqlCeSettingsConnector.CreateNewAllowedSettings(Parent));

                return MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
            }
        }

        public int GetLMCount()
        {
            string path = Path.GetDirectoryName(Parent.GetParentDictionary().Connection);
            string[] files = Directory.GetFiles(path, "*" + Helper.EmbeddedDbExtension);
            return files.Length;
        }

        public IList<Guid> GetExtensions()
        {
            IList<Guid> guids = new List<Guid>();
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT guid FROM \"Extensions\"";
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

                while (reader.Read())
                    guids.Add(new Guid(reader["guid"] as string));
            }

            return guids;
        }

        #endregion
    }
}
