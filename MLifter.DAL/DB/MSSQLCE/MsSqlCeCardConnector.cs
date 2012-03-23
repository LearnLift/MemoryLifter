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
	/// MsSqlCeCardConnector
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-01-09</remarks>
	class MsSqlCeCardConnector : IDbCardConnector
	{
		private static Dictionary<ConnectionStringStruct, MsSqlCeCardConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeCardConnector>();
		public static MsSqlCeCardConnector GetInstance(ParentClass parentClass)
		{
			lock (instances)
			{
				ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

				if (!instances.ContainsKey(connection))
					instances.Add(connection, new MsSqlCeCardConnector(parentClass));

				return instances[connection];
			}
		}

		private ParentClass parent;
		private MsSqlCeCardConnector(ParentClass parentClass)
		{
			parent = parentClass;
			parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
		}

		void parent_DictionaryClosed(object sender, EventArgs e)
		{
			IParent parent = sender as IParent;
			instances.Remove(parent.Parent.CurrentUser.ConnectionString);
		}


		#region IDbCardConnector Members

		/// <summary>
		/// Checks if the card exists and throws an IdAccessException if not.
		/// </summary>
		/// <param name="id">The card id.</param>
		/// <remarks>Documented by Dev03, 2008-08-06</remarks>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public void CheckCardId(int id)
		{
			List<int> cardIdsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardIdsList, 0)] as List<int>;
			if (cardIdsCache != null && cardIdsCache.Contains(id))
				return;

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "(SELECT lm_id FROM \"LearningModules_Cards\" WHERE cards_id=@id)";
				cmd.Parameters.Add("@id", id);
				int lmId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
				cmd.Parameters.Clear();

				cmd.CommandText = "SELECT id FROM \"Cards\" WHERE id IN " +
					"(SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=@lm_id)";
				cmd.Parameters.Add("@lm_id", lmId);
				SqlCeDataReader reader;
				try { reader = MSSQLCEConn.ExecuteReader(cmd); }
				catch { throw new IdAccessException(id); }

				List<int> cardIds = new List<int>();
				while (reader.Read())
					cardIds.Add(Convert.ToInt32(reader["id"]));
				reader.Close();

				parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardIdsList, 0)] = cardIds;
			}
		}

		/// <summary>
		/// Gets the chapter for a card.
		/// </summary>
		/// <param name="id">The card id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-08-06</remarks>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public int GetChapter(int id)
		{
			Dictionary<int, int> cardChaptersCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardChapterList, 0)] as Dictionary<int, int>;
			if (cardChaptersCache != null && cardChaptersCache.ContainsKey(id))
				return cardChaptersCache[id];

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "(SELECT lm_id FROM \"LearningModules_Cards\" WHERE cards_id=@id)";
				cmd.Parameters.Add("@id", id);
				int lmid = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));

				cmd.Parameters.Clear();

				cmd.CommandText = "SELECT chapters_id, cards_id FROM Chapters_Cards WHERE chapters_id IN " +
					"(SELECT chapters_id FROM Chapters WHERE lm_id=@lmid)";
				cmd.Parameters.Add("@lmid", lmid);
				SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

				Dictionary<int, int> cardChapters = new Dictionary<int, int>();

				while (reader.Read())
				{
					object[] chapters = new object[2];
					reader.GetValues(chapters);
					int chid = Convert.ToInt32(chapters[0]);
					int cid = Convert.ToInt32(chapters[1]);
					if (!cardChapters.ContainsKey(cid))
						cardChapters[cid] = chid;
				}
				reader.Close();

				// this should fix the bug where for some reason a card didn't have chapter assigned [ML-1708] (and similar)
				int chapterId = 0;
				if (!cardChapters.TryGetValue(id, out chapterId))
				{
					lock (cardChapters)
					{
						foreach (int c in cardChapters.Values)
						{
							chapterId = c;
							break;
						}
						SetChapter(id, chapterId);
						cardChapters.Add(id, chapterId);
					}
				}

				parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardChapterList, 0, new TimeSpan(23, 59, 59))] = cardChapters;

				return chapterId;
			}
		}

		/// <summary>
		/// Sets the chapter for a card.
		/// </summary>
		/// <param name="id">The card id.</param>
		/// <param name="chapter">The chapter id.</param>
		/// <remarks>Documented by Dev03, 2008-08-06</remarks>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public void SetChapter(int id, int chapter)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				SqlCeTransaction transaction = cmd.Connection.BeginTransaction();
				cmd.CommandText = "SELECT count(*) FROM \"Chapters\" WHERE id=@chapterid";
				cmd.Parameters.Add("chapterid", chapter);
				if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
					throw new IdAccessException(chapter);
				Dictionary<int, int> cardChapterCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardChapterList, 0)] as Dictionary<int, int>;
				if (cardChapterCache != null)
					cardChapterCache[id] = chapter;

				using (SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
				{
					cmd2.CommandText = "DELETE FROM \"Chapters_Cards\" WHERE cards_id=@id; ";
					cmd2.CommandText += "INSERT INTO \"Chapters_Cards\" (chapters_id, cards_id) VALUES (@chapterid, @id);";
					cmd2.CommandText += "UPDATE Cards SET chapters_id=@chapterid WHERE id=@id;";
					cmd2.Parameters.Add("@chapterid", chapter);
					cmd2.Parameters.Add("@id", id);
					MSSQLCEConn.ExecuteNonQuery(cmd2);
				}
				transaction.Commit();
			}
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public ISettings GetSettings(int id)
		{
			int? cardSettingsId = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardSetting, id)] as int?;
			if (cardSettingsId.HasValue)
				return new DbSettings(cardSettingsId.Value, false, parent);

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT settings_id FROM \"Cards\" WHERE id=@id";
				cmd.Parameters.Add("@id", id);

				int? sid = MSSQLCEConn.ExecuteScalar<int>(cmd);

				if (sid.HasValue)
				{
					parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardSetting, id)] = sid;
					return new DbSettings(sid.Value, false, parent);
				}
				else
					return null;
			}
		}

		/// <summary>
		/// Sets the settings.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="Settings">The settings.</param>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public void SetSettings(int id, MLifter.DAL.Interfaces.ISettings Settings)
		{
			throw new NotAllowedInDbModeException();

			//using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			//{
			//    cmd.CommandText = "UPDATE \"Cards\" SET settings_id=@value WHERE id=@id";
			//    cmd.Parameters.Add("@id", id);
			//    cmd.Parameters.Add("@value", (Settings as DbSettings).Id);

			//    MSSQLCEConn.ExecuteNonQuery(cmd);
			//}
		}

		/// <summary>
		/// Gets the box.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public int GetBox(int id)
		{
			CardState? state = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?;
			if (state.HasValue)
				return state.Value.Box;

			ReadCardState(id);

			return (parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?).Value.Box;
		}

		/// <summary>
		/// Sets the box.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="box">The box.</param>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public void SetBox(int id, int box)
		{
			SetCardState(parent.CurrentUser.Id, id, box, GetActive(id), GetTimestamp(id));
			parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id));
			parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Score, parent.GetParentDictionary().Id));
			parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.BoxSizes, parent.CurrentUser.ConnectionString.LmId));
			parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CurrentBoxSizes, parent.CurrentUser.ConnectionString.LmId));
		}

		/// <summary>
		/// Gets the active.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public bool GetActive(int id)
		{
			CardState? state = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?;
			if (state.HasValue)
				return state.Value.Active;

			ReadCardState(id);

			return (parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?).Value.Active;
		}

		/// <summary>
		/// Sets the active.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="active">if set to <c>true</c> [active].</param>
		/// <remarks>
		/// Documented by FabThe, 9.1.2009
		/// </remarks>
		public void SetActive(int id, bool active)
		{
			SetCardState(parent.CurrentUser.Id, id, GetBox(id), active, GetTimestamp(id));
			parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id));
		}

		/// <summary>
		/// Gets the timestamp.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		public DateTime GetTimestamp(int id)
		{
			CardState? state = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?;
			if (state.HasValue)
				return state.Value.TimeStamp;

			ReadCardState(id);

			return (parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?).Value.TimeStamp;
		}

		/// <summary>
		/// Sets the timestamp.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <remarks>
		/// Documented by FabThe, 9.1.2009
		/// </remarks>
		public void SetTimestamp(int id, DateTime timestamp)
		{
			SetCardState(parent.CurrentUser.Id, id, GetBox(id), GetActive(id), timestamp);
			parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id));
		}

		#endregion

		/// <summary>
		/// Reads the state of the card.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		private void ReadCardState(int id)
		{
			int? cnt;
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT count(*) FROM \"UserCardState\" WHERE user_id = @param_user_id and cards_id = @param_cards_id;";
				cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
				cmd.Parameters.Add("@param_cards_id", id);

				cnt = MSSQLCEConn.ExecuteScalar<int>(cmd);
			}

			if (!cnt.HasValue || cnt.Value < 1)
			{
				using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
				{
					cmd.CommandText = "INSERT INTO \"UserCardState\" (user_id, cards_id, box, active) VALUES (@param_user_id, @param_cards_id, 0, 1);";
					cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
					cmd.Parameters.Add("@param_cards_id", id);

					MSSQLCEConn.ExecuteNonQuery(cmd);
				}
			}

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT timestamp, box, active FROM \"UserCardState\" WHERE user_id = @param_user_id and cards_id = @param_cards_id;";
				cmd.Parameters.Add("@param_user_id", parent.CurrentUser.Id);
				cmd.Parameters.Add("@param_cards_id", id);

				SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

				reader.Read();
				object ts = reader["timestamp"];
				DateTime timestamp = ts is DBNull ? new DateTime(1901, 1, 1) : Convert.ToDateTime(ts);
				parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardState, id)] = new CardState(Convert.ToInt32(reader["box"]),
																													 Convert.ToBoolean(reader["active"]), timestamp);
				reader.Close();
			}
		}

		/// <summary>
		/// C# Function which replaces the SetCardState(..) procedure of pgsql
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="card">The card.</param>
		/// <param name="box">The box.</param>
		/// <param name="active">if set to <c>true</c> [active].</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <remarks>Documented by Dev08, 2009-01-09</remarks>
		private void SetCardState(int id, int card, int box, bool active, DateTime timestamp)
		{
			int? cnt;
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT count(*) AS cnt FROM \"UserCardState\" WHERE user_id=@param_user_id and cards_id=@param_cards_id;";
				cmd.Parameters.Add("@param_user_id", id);
				cmd.Parameters.Add("@param_cards_id", card);

				cnt = MSSQLCEConn.ExecuteScalar<int>(cmd);
			}

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				if (!cnt.HasValue || cnt.Value < 1)
				{
					cmd.CommandText = "INSERT INTO \"UserCardState\" (user_id, cards_id, box, active, timestamp) VALUES (@param_user_id, @param_cards_id, @param_box, @param_active, @param_timestamp);";
					cmd.Parameters.Add("@param_user_id", id);
					cmd.Parameters.Add("@param_cards_id", card);
					cmd.Parameters.Add("@param_box", box);
					cmd.Parameters.Add("@param_active", active);
					cmd.Parameters.Add("@param_timestamp", timestamp);

					MSSQLCEConn.ExecuteNonQuery(cmd);
				}
				else
				{
					cmd.CommandText = "UPDATE \"UserCardState\" SET box=@param_box, active=@param_active, timestamp=@param_timestamp WHERE user_id=@param_user_id and cards_id=@param_cards_id;";
					cmd.Parameters.Add("@param_user_id", id);
					cmd.Parameters.Add("@param_cards_id", card);
					cmd.Parameters.Add("@param_box", box);
					cmd.Parameters.Add("@param_active", active);
					cmd.Parameters.Add("@param_timestamp", timestamp);

					MSSQLCEConn.ExecuteNonQuery(cmd);
				}
			}
		}
	}
}
