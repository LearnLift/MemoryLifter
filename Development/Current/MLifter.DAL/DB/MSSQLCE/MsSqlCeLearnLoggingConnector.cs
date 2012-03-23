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
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE connector for the IDbLearnLoggingConnector interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    class MsSqlCeLearnLoggingConnector : IDbLearnLoggingConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeLearnLoggingConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeLearnLoggingConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public static MsSqlCeLearnLoggingConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeLearnLoggingConnector(parentClass));
                else if (instances[connection].Parent.CurrentUser.Id != parentClass.CurrentUser.Id)
                    instances[connection].Parent = parentClass;

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeLearnLoggingConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbLearnLoggingConnector Members
        /// <summary>
        /// Creates the learn log entry.
        /// </summary>
        /// <param name="learnLog">The learn log.</param>
        /// <remarks>Documented by Dev10, 2009-01-11</remarks>
        public void CreateLearnLogEntry(LearnLogStruct learnLog)
        {
            bool answeredCardCorrect = false;
            if (learnLog.MoveType != MoveType.Manual)
            {
                if (learnLog.NewBox > 1)
                    answeredCardCorrect = true;
            }

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO LearnLog (session_id, user_id, lm_id, cards_id, old_box, new_box, timestamp, duration, learn_mode, move_type, answer, direction, case_sensitive, correct_on_the_fly, percentage_known, percentage_required) ";
                cmd.CommandText += "VALUES(@sid, @uid, @lmid, @cid, @obox, @nbox, @ts, @dur, @lmode, @mtype, @answ, @dir, @csen, @cotf, @pknown, @preq); ";
                if (learnLog.MoveType != MoveType.Manual && answeredCardCorrect)
                    cmd.CommandText += "UPDATE LearningSessions SET sum_right=sum_right + 1 WHERE id=@sid AND user_id=@uid AND lm_id=@lmid; ";
                else if (learnLog.MoveType != MoveType.Manual && !answeredCardCorrect)
                    cmd.CommandText += "UPDATE LearningSessions SET sum_wrong=sum_wrong + 1 WHERE id=@sid AND user_id=@uid AND lm_id=@lmid; ";

                string newBoxContent = string.Empty;
                string oldBoxContent = string.Empty;
                if (learnLog.NewBox.Value > 0)
                    newBoxContent = "box" + learnLog.NewBox.Value.ToString() + "_content";
                else
                    newBoxContent = "pool_content";

                if (learnLog.OldBox.Value > 0)
                    oldBoxContent = "box" + learnLog.OldBox.Value.ToString() + "_content";
                else
                    oldBoxContent = "pool_content";

                if (learnLog.NewBox.Value != learnLog.OldBox.Value)
                    cmd.CommandText += "UPDATE LearningSessions SET " + newBoxContent + "=" + newBoxContent + " + 1, " + oldBoxContent + "=" + oldBoxContent + " - 1 WHERE id=@sid AND user_id=@uid AND lm_id=@lmid; ";

                cmd.Parameters.Add("@sid", learnLog.SessionID.Value);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lmid", Parent.CurrentUser.ConnectionString.LmId);
                cmd.Parameters.Add("@cid", learnLog.CardsID.Value);
                cmd.Parameters.Add("@obox", learnLog.OldBox.Value);
                cmd.Parameters.Add("@nbox", learnLog.NewBox.Value);
                cmd.Parameters.Add("@ts", learnLog.TimeStamp.Value);
                cmd.Parameters.Add("@dur", learnLog.Duration.Value);
                cmd.Parameters.Add("@lmode", learnLog.LearnMode.Value.ToString());
                cmd.Parameters.Add("@mtype", learnLog.MoveType.Value.ToString());
                cmd.Parameters.Add("@answ", learnLog.Answer);
                cmd.Parameters.Add("@dir", learnLog.Direction.Value.ToString());
                cmd.Parameters.Add("@csen", learnLog.CaseSensitive);
                cmd.Parameters.Add("@cotf", learnLog.CorrectOnTheFly);
                cmd.Parameters.Add("@pknown", learnLog.PercentageKnown);
                cmd.Parameters.Add("@preq", learnLog.PercentageRequired);

                MSSQLCEConn.ExecuteNonQuery(cmd);
            }

            //delete caches
            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticWrongCards, learnLog.SessionID.Value));
            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticCorrectCards, learnLog.SessionID.Value));
            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticContentOfBoxes, learnLog.SessionID.Value));
        }

        #endregion
    }
}
