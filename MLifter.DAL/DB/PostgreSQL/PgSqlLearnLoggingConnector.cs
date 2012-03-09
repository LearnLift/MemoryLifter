using System;
using System.Collections.Generic;
using System.Text;

using Npgsql;
using NpgsqlTypes;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlLearnLoggingConnector : IDbLearnLoggingConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlLearnLoggingConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlLearnLoggingConnector>();
        public static PgSqlLearnLoggingConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlLearnLoggingConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlLearnLoggingConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        /// <summary>
        /// Creates a learn log entry.
        /// </summary>
        /// <param name="learnLog">A learn log struct.</param>
        /// <remarks>Documented by Dev08, 2008-09-05</remarks>
        public void CreateLearnLogEntry(LearnLogStruct learnLog)
        {
            bool answeredCardCorrect = false;
            if (learnLog.MoveType != MoveType.Manual)
            {
                if (learnLog.NewBox > 1)
                    answeredCardCorrect = true;
            }

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"LearnLog\" (session_id, user_id, lm_id, cards_id, old_box, new_box, timestamp, duration, learn_mode, move_type, answer, direction, case_sensitive, correct_on_the_fly, percentage_known, percentage_required) ";
                    cmd.CommandText += " VALUES(:sid, :uid, :lmid, :cid, :obox, :nbox, :ts, :dur, :lmode, :mtype, :answ, :dir, :csen, :cotf, :pknown, :preq); ";

                    if (learnLog.MoveType != MoveType.Manual && answeredCardCorrect)
                        cmd.CommandText += "UPDATE \"LearningSessions\" SET sum_right=sum_right + 1 WHERE id=:sid AND user_id=:uid AND lm_id=:lmid; ";
                    else if (learnLog.MoveType != MoveType.Manual && !answeredCardCorrect)
                        cmd.CommandText += "UPDATE \"LearningSessions\" SET sum_wrong=sum_wrong + 1 WHERE id=:sid AND user_id=:uid AND lm_id=:lmid; ";

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
                        cmd.CommandText += "UPDATE \"LearningSessions\" SET " + newBoxContent + "=" + newBoxContent + " + 1, " + oldBoxContent + "=" + oldBoxContent + " - 1 WHERE id=:sid AND user_id=:uid AND lm_id=:lmid; ";

                    cmd.Parameters.Add("sid", learnLog.SessionID.Value);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", Parent.CurrentUser.ConnectionString.LmId);
                    cmd.Parameters.Add("cid", learnLog.CardsID.Value);
                    cmd.Parameters.Add("obox", learnLog.OldBox.Value);
                    cmd.Parameters.Add("nbox", learnLog.NewBox.Value);
                    cmd.Parameters.Add("ts", (NpgsqlTypes.NpgsqlTimeStamp)learnLog.TimeStamp.Value);
                    cmd.Parameters.Add("dur", learnLog.Duration.Value);
                    cmd.Parameters.Add("lmode", learnLog.LearnMode.Value.ToString());
                    cmd.Parameters.Add("mtype", learnLog.MoveType.Value.ToString());
                    cmd.Parameters.Add("answ", learnLog.Answer);
                    cmd.Parameters.Add("dir", learnLog.Direction.Value.ToString());
                    cmd.Parameters.Add("csen", learnLog.CaseSensitive);
                    cmd.Parameters.Add("cotf", learnLog.CorrectOnTheFly);
                    cmd.Parameters.Add("pknown", learnLog.PercentageKnown);
                    cmd.Parameters.Add("preq", learnLog.PercentageRequired);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }
    }
}