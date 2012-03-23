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
using System.Data;
using System.Data.SqlServerCe;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE connector for the IDbBoxConnector interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-09</remarks>
    class MsSqlCeBoxConnector : IDbBoxConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeBoxConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeBoxConnector>();
        private static TimeSpan cacheLifeSpan = new TimeSpan(0, 0, 1);
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public static MsSqlCeBoxConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new MsSqlCeBoxConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private MsSqlCeBoxConnector(ParentClass parentClass)
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

        /// <summary>
        /// Gets the size of the current.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        /// <remarks>Documented by Dev08, 2009-01-09</remarks>
        public int GetCurrentSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CurrentBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                sizes = new BoxSizes(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);          //filled with temp/default values

                cmd.CommandText = "SELECT CS.box, count(*) AS count FROM UserCardState CS INNER JOIN Cards C ON CS.cards_id = C.id WHERE CS.active=1 and CS.user_id=@user_id and C.lm_id=@lm_id GROUP BY CS.box";
                cmd.Parameters.Add("@user_id", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", Parent.CurrentUser.ConnectionString.LmId);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                while (reader.Read())
                    sizes.Value.Sizes[Convert.ToInt32(reader["box"])] = Convert.ToInt32(reader["count"]);
                reader.Close();

                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CurrentBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                return sizes.Value.Sizes[id];
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public int GetSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.BoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                sizes = new BoxSizes(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                cmd.CommandText = @"SELECT UserCardState.box AS box, count(*) AS count FROM UserCardState, Chapters_Cards
	                                WHERE UserCardState.active=1 and 
		                            Chapters_Cards.cards_id=UserCardState.cards_id and UserCardState.user_id=@user_id and 
		                            Chapters_Cards.chapters_id IN (
			                            SELECT chapters_id FROM SelectedLearnChapters INNER JOIN UserProfilesLearningModulesSettings
					                    ON SelectedLearnChapters.settings_id=UserProfilesLearningModulesSettings.settings_id
				                        WHERE UserProfilesLearningModulesSettings.user_id=@user_id and UserProfilesLearningModulesSettings.lm_id=@lm_id
		                            ) GROUP BY UserCardState.box";
                cmd.Parameters.Add("@user_id", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", Parent.CurrentUser.ConnectionString.LmId);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                while (reader.Read())
                    sizes.Value.Sizes[Convert.ToInt32(reader["box"])] = Convert.ToInt32(reader["count"]);
                reader.Close();

                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.BoxSizes, Parent.CurrentUser.ConnectionString.LmId, cacheLifeSpan)] = sizes.Value;

                return sizes.Value.Sizes[id];
            }
        }

        /// <summary>
        /// Gets the size of the maximal.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public int GetMaximalSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MaximalBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM UserProfilesLearningModulesSettings WHERE user_id=@uid and lm_id=@lm_id";
                cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                int settingsCount = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();

                cmd.CommandText = "SELECT count(*) FROM Cards WHERE id IN (SELECT cards_id FROM LearningModules_Cards WHERE lm_id=@lm_id)";
                cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                int cardsCount = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();

                // get the default sizes
                cmd.CommandText = @"SELECT box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size
	FROM Boxes INNER JOIN Settings ON Boxes.id = Settings.boxes
		INNER JOIN LearningModules ON Settings.id=LearningModules.default_settings_id
	WHERE LearningModules.id=@lm_id";
                cmd.Parameters.Add("@lm_id", Parent.CurrentUser.ConnectionString.LmId);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                reader.Read();
                BoxSizes defaultSizes = new BoxSizes(cardsCount,
                    Convert.ToInt32(reader["box1_size"]),
                    Convert.ToInt32(reader["box2_size"]),
                    Convert.ToInt32(reader["box3_size"]),
                    Convert.ToInt32(reader["box4_size"]),
                    Convert.ToInt32(reader["box5_size"]),
                    Convert.ToInt32(reader["box6_size"]),
                    Convert.ToInt32(reader["box7_size"]),
                    Convert.ToInt32(reader["box8_size"]),
                    Convert.ToInt32(reader["box9_size"]),
                    cardsCount);
                reader.Close();

                if (settingsCount > 0)
                {
                    // get the user sizes
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT boxes FROM Settings INNER JOIN UserProfilesLearningModulesSettings ON Settings.id=UserProfilesLearningModulesSettings.settings_id WHERE user_id=@uid and lm_id=@lm_id";
                    cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("@lm_id", Parent.CurrentUser.ConnectionString.LmId);
                    int boxesId = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"SELECT box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size FROM Boxes WHERE id=@bid";
                    cmd.Parameters.Add("@bid", boxesId);
                    reader = MSSQLCEConn.ExecuteReader(cmd);
                    if (reader.Read())
                    {
                        sizes = new BoxSizes(cardsCount,
                            GetUserOrDefaultSize(reader["box1_size"], defaultSizes.Sizes[1]),
                            GetUserOrDefaultSize(reader["box2_size"], defaultSizes.Sizes[2]),
                            GetUserOrDefaultSize(reader["box3_size"], defaultSizes.Sizes[3]),
                            GetUserOrDefaultSize(reader["box4_size"], defaultSizes.Sizes[4]),
                            GetUserOrDefaultSize(reader["box5_size"], defaultSizes.Sizes[5]),
                            GetUserOrDefaultSize(reader["box6_size"], defaultSizes.Sizes[6]),
                            GetUserOrDefaultSize(reader["box7_size"], defaultSizes.Sizes[7]),
                            GetUserOrDefaultSize(reader["box8_size"], defaultSizes.Sizes[8]),
                            GetUserOrDefaultSize(reader["box9_size"], defaultSizes.Sizes[9]),
                            cardsCount);
                    }
                    else
                    {
                        sizes = defaultSizes;
                    }
                    reader.Close();
                }
                else
                {
                    sizes = defaultSizes;
                }

                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.MaximalBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                return sizes.Value.Sizes[id];
            }
        }

        private int GetUserOrDefaultSize(object userValue, int defaultSize)
        {
            return (userValue == null || userValue is DBNull) ? defaultSize : Convert.ToInt32(userValue);
        }

        /// <summary>
        /// Sets the size of the maximal.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="MaximalSize">Size of the maximal.</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public void SetMaximalSize(int id, int MaximalSize)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = @"SELECT boxes FROM Settings INNER JOIN UserProfilesLearningModulesSettings
				ON Settings.id=UserProfilesLearningModulesSettings.settings_id
			WHERE UserProfilesLearningModulesSettings.lm_id=@lm_id and UserProfilesLearningModulesSettings.user_id=@uid";
                cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);
                cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                int boxesId = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();

                cmd.CommandText = string.Format("UPDATE Boxes SET box{0}_size=@value WHERE id=@box_id", id);
                cmd.Parameters.Add("@value", MaximalSize);
                cmd.Parameters.Add("@box_id", boxesId);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.MaximalBoxSizes, Parent.CurrentUser.ConnectionString.LmId));
            }
        }

        /// <summary>
        /// Gets the size of the default.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public int GetDefaultSize(int id)
        {
            BoxSizes? sizes = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.DefaultBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] as BoxSizes?;
            if (sizes.HasValue)
                return sizes.Value.Sizes[id];

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT Boxes.* FROM Boxes INNER JOIN Settings ON Boxes.id = Settings.boxes INNER JOIN LearningModules ON Settings.id = LearningModules.default_settings_id WHERE LearningModules.id = @lm_id";
                cmd.Parameters.Add("@lm_id", Parent.CurrentUser.ConnectionString.LmId);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
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
                reader.Close();

                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.DefaultBoxSizes, Parent.CurrentUser.ConnectionString.LmId)] = sizes.Value;

                return sizes.Value.Sizes[id];
            }
        }

        #endregion
    }
}
