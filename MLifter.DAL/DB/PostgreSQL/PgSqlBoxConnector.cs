using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlBoxConnector : IDbBoxConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlBoxConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlBoxConnector>();
        public static PgSqlBoxConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlBoxConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlBoxConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbBoxConnector Members

        public int GetCurrentSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CurrentBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    sizes = new BoxSizes(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    cmd.CommandText = @"SELECT box, count(*) AS count FROM ""UserCardState"" WHERE active=true and user_id=:user_id and cards_id IN (SELECT cards_id FROM ""LearningModules_Cards"" WHERE lm_id=:lm_id) GROUP BY box";
                    cmd.Parameters.Add("user_id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser, false);
                    while (reader.Read())
                        sizes.Value.Sizes[Convert.ToInt32(reader["box"])] = Convert.ToInt32(reader["count"]);

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CurrentBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                    return sizes.Value.Sizes[id];
                }
            }
        }

        public int GetSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.BoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    sizes = new BoxSizes(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    cmd.CommandText = @"SELECT ""UserCardState"".box AS box, count(*) AS count FROM ""UserCardState"", ""Chapters_Cards""
	                                WHERE ""UserCardState"".active=true and 
		                            ""Chapters_Cards"".cards_id=""UserCardState"".cards_id and ""UserCardState"".user_id=:user_id and 
		                            ""UserCardState"".cards_id IN (SELECT cards_id FROM ""LearningModules_Cards"" WHERE lm_id=:lm_id) and 
		                            ""Chapters_Cards"".chapters_id IN (
			                            SELECT chapters_id FROM ""SelectedLearnChapters"" INNER JOIN ""UserProfilesLearningModulesSettings""
					                    ON ""SelectedLearnChapters"".settings_id=""UserProfilesLearningModulesSettings"".settings_id
				                        WHERE ""UserProfilesLearningModulesSettings"".user_id=:user_id and ""UserProfilesLearningModulesSettings"".lm_id=:lm_id
		                            ) GROUP BY ""UserCardState"".box";
                    cmd.Parameters.Add("user_id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser, false);
                    while (reader.Read())
                        sizes.Value.Sizes[Convert.ToInt32(reader["box"])] = Convert.ToInt32(reader["count"]);

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.BoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                    return sizes.Value.Sizes[id];
                }
            }
        }

        public int GetMaximalSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MaximalBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT * FROM \"GetBoxSizes\"(:uid, :lm_id)", id);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();
                    sizes = new BoxSizes(Convert.ToInt32(reader["box0"]),
                        Convert.ToInt32(reader["box1"]),
                        Convert.ToInt32(reader["box2"]),
                        Convert.ToInt32(reader["box3"]),
                        Convert.ToInt32(reader["box4"]),
                        Convert.ToInt32(reader["box5"]),
                        Convert.ToInt32(reader["box6"]),
                        Convert.ToInt32(reader["box7"]),
                        Convert.ToInt32(reader["box8"]),
                        Convert.ToInt32(reader["box9"]),
                        Convert.ToInt32(reader["box10"]));

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.MaximalBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                    return sizes.Value.Sizes[id];
                }
            }
        }

        public void SetMaximalSize(int id, int MaximalSize)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE \"Boxes\" SET box{0}_size=:value " +
                        "WHERE id=(SELECT boxes FROM \"Settings\" WHERE id=(SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:lm_id and user_id=:uid))", id);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                    cmd.Parameters.Add("value", MaximalSize);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MaximalBoxSizes, Parent.CurrentUser.ConnectionString.LmId));
                }
            }
        }

        public int GetDefaultSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DefaultBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT * FROM \"Boxes\"" +
                        "WHERE id=(SELECT boxes FROM \"Settings\" WHERE id=(SELECT default_settings_id FROM \"LearningModules\" WHERE id=:lm_id))", id);
                    cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();
                    sizes = new BoxSizes(int.MaxValue,
                        Convert.ToInt32(reader["box1_size"]),
                        Convert.ToInt32(reader["box2_size"]),
                        Convert.ToInt32(reader["box3_size"]),
                        Convert.ToInt32(reader["box4_size"]),
                        Convert.ToInt32(reader["box5_size"]),
                        Convert.ToInt32(reader["box6_size"]),
                        Convert.ToInt32(reader["box7_size"]),
                        Convert.ToInt32(reader["box8_size"]),
                        Convert.ToInt32(reader["box9_size"]),
                        int.MaxValue);

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DefaultBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                    return sizes.Value.Sizes[id];
                }
            }
        }

        #endregion
    }
}
