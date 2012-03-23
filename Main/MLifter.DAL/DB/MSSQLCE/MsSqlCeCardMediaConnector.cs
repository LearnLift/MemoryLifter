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
    class MsSqlCeCardMediaConnector : IDbCardMediaConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeCardMediaConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeCardMediaConnector>();
        public static MsSqlCeCardMediaConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new MsSqlCeCardMediaConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private MsSqlCeCardMediaConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbCardMediaConnector Members

        public MLifter.DAL.Interfaces.IMedia GetMedia(int id, int cardid)
        {
            IList<IMedia> mediaList = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid)] as IList<IMedia>;
            if (mediaList != null)
            {
                GetCardMedia(cardid, Side.Answer, (WordType?)null);
                foreach (DbMedia media in Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid)] as IList<IMedia>)
                    if (media.Id == id)
                        return media;
            }

            return null;
        }

        public void SetCardMedia(int id, int cardid, MLifter.DAL.Interfaces.Side side, MLifter.DAL.Interfaces.WordType type, bool isDefault, MLifter.DAL.Interfaces.EMedia mediatype)
        {

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                SqlCeTransaction tran = cmd.Connection.BeginTransaction();

                ClearCardMedia(cardid, side, type, mediatype);

                cmd.CommandText = "INSERT INTO \"Cards_MediaContent\" (media_id, cards_id, side, type, is_default) " +
                    "VALUES (@mediaid, @cardid, @side, @type, @isdefault);";
                cmd.Parameters.Add("@mediaid", id);
                cmd.Parameters.Add("@cardid", cardid);
                cmd.Parameters.Add("@side", side.ToString());
                cmd.Parameters.Add("@type", type.ToString());
                cmd.Parameters.Add("@isdefault", isDefault);
                MSSQLCEConn.ExecuteNonQuery(cmd);

                tran.Commit();
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public IList<int> GetCardMedia(int cardid, MLifter.DAL.Interfaces.Side side)
        {
            return GetCardMedia(cardid, side, null);
        }

        public IList<int> GetCardMedia(int cardid, MLifter.DAL.Interfaces.Side side, MLifter.DAL.Interfaces.WordType type)
        {
            return GetCardMedia(cardid, side, type);
        }

        public void ClearCardMedia(int cardid)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM \"Cards_MediaContent\" WHERE cards_id=@cardid;";
                cmd.Parameters.Add("@cardid", cardid);
                cmd.ExecuteNonQuery();
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public void ClearCardMedia(int cardid, int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM \"Cards_MediaContent\" WHERE media_id=@mediaid AND cards_id=@cardid;";
                cmd.Parameters.Add("@cardid", cardid);
                cmd.Parameters.Add("@mediaid", id);
                cmd.ExecuteNonQuery();
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public void ClearCardMedia(int cardid, Side side, WordType type, EMedia mediatype)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM \"Cards_MediaContent\" WHERE cards_id=@cardid AND side=@side AND type=@type " +
                    "AND media_id IN (SELECT id FROM \"MediaContent\" WHERE media_type=@mediatype);";
                cmd.Parameters.Add("@cardid", cardid);
                cmd.Parameters.Add("@side", side.ToString());
                cmd.Parameters.Add("@type", type.ToString());
                cmd.Parameters.Add("@mediatype", mediatype.ToString());
                cmd.ExecuteNonQuery();
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public void CheckCardMediaId(int id, int cardid)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"Cards_MediaContent\" WHERE media_id=@id AND cards_id=@cardid;";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@cardid", cardid);
                if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) < 1)
                    throw new IdAccessException(id);
            }
        }

        public IList<int> GetMediaResources(int lmid)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT DISTINCT cc.media_id FROM \"Cards_MediaContent\" AS cc " +
                    "JOIN \"LearningModules_Cards\" AS lc ON cc.cards_id = lc.cards_id WHERE lc.lm_id=@lmid;";
                cmd.Parameters.Add("@lmid", lmid);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

                IList<int> ids = new List<int>();
                while (reader.Read())
                    ids.Add(Convert.ToInt32(reader["media_id"]));
                reader.Close();

                return ids;
            }
        }

        #endregion

        private IList<int> GetCardMedia(int cardid, Side side, WordType? type)
        {
            IList<int> ids = new List<int>();
            IList<IMedia> mediaList = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid)] as IList<IMedia>;
            if (mediaList != null)
            {
                foreach (DbMedia cms in mediaList)
                    if (cms.Side == side && (!type.HasValue || cms.Type == type))
                        ids.Add(cms.Id);

                return ids;
            }

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id, media_type, side, type, is_default FROM \"Cards_MediaContent\" JOIN \"MediaContent\" ON media_id=id WHERE cards_id=@cardid;";
                cmd.Parameters.Add("@cardid", cardid);
                SqlCeDataReader reader = MsSqlCe.MSSQLCEConn.ExecuteReader(cmd);

                mediaList = new List<IMedia>();
                while (reader.Read())
                {
                    IMedia newMedia = null;
                    int id = Convert.ToInt32(reader["id"]);
                    EMedia mtype = (EMedia)Enum.Parse(typeof(EMedia), Convert.ToString(reader["media_type"]));
                    Side cside = (Side)Enum.Parse(typeof(Side), Convert.ToString(reader["side"]));
                    WordType wordtype = (WordType)Enum.Parse(typeof(WordType), Convert.ToString(reader["type"]));
                    bool isDefault = Convert.ToBoolean(reader["is_default"]);
                    switch (mtype)
                    {
                        case EMedia.Audio:
                            newMedia = new DbAudio(id, cardid, false, cside, wordtype, isDefault, (wordtype == WordType.Sentence), Parent);
                            break;
                        case EMedia.Image:
                            newMedia = new DbImage(id, cardid, false, cside, wordtype, isDefault, (wordtype == WordType.Sentence), Parent);
                            break;
                        case EMedia.Video:
                            newMedia = new DbVideo(id, cardid, false, cside, wordtype, isDefault, (wordtype == WordType.Sentence), Parent);
                            break;
                    }

                    mediaList.Add(newMedia);

                    if ((newMedia as DbMedia).Side == side && (!type.HasValue || (newMedia as DbMedia).Type == type))
                        ids.Add((newMedia as DbMedia).Id);
                }
                reader.Close();

                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardMedia, cardid)] = mediaList;

                return ids;
            }
        }
    }
}
