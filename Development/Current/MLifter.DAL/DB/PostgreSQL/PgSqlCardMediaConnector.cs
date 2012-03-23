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
using Npgsql;
using NpgsqlTypes;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    internal class PgSqlCardMediaConnector : IDbCardMediaConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlCardMediaConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlCardMediaConnector>();
        public static PgSqlCardMediaConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlCardMediaConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlCardMediaConnector(ParentClass parentClass)
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

        public IMedia GetMedia(int id, int cardid)
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

        public void SetCardMedia(int id, int cardid, Side side, WordType type, bool isDefault, EMedia mediatype)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction tran = con.BeginTransaction();
                ClearCardMedia(cardid, side, type, mediatype);

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Cards_MediaContent\" (media_id, cards_id, side, type, is_default) VALUES (:mediaid, :cardid, :side, :type, :isdefault);";
                    cmd.Parameters.Add("mediaid", id);
                    cmd.Parameters.Add("cardid", cardid);
                    cmd.Parameters.Add("side", side.ToString());
                    cmd.Parameters.Add("type", type.ToString());
                    cmd.Parameters.Add("isdefault", isDefault);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                tran.Commit();

                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
            }
        }

        public IList<int> GetCardMedia(int cardid, Side side)
        {
            return GetCardMedia(cardid, side, null);
        }

        public IList<int> GetCardMedia(int cardid, Side side, WordType type)
        {
            return GetCardMedia(cardid, side, type);
        }

        public void ClearCardMedia(int cardid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"Cards_MediaContent\" WHERE cards_id=:cardid;";
                    cmd.Parameters.Add("cardid", cardid);
                    cmd.ExecuteNonQuery();
                }
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public void ClearCardMedia(int cardid, int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"Cards_MediaContent\" WHERE media_id=:mediaid AND cards_id=:cardid;";
                    cmd.Parameters.Add("cardid", cardid);
                    cmd.Parameters.Add("mediaid", id);
                    cmd.ExecuteNonQuery();
                }
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public void ClearCardMedia(int cardid, Side side, WordType type, EMedia mediatype)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"Cards_MediaContent\" WHERE cards_id=:cardid AND side=:side AND type=:type " +
                        "AND media_id IN (SELECT id FROM \"MediaContent\" WHERE media_type=:mediatype);";
                    cmd.Parameters.Add("cardid", cardid);
                    cmd.Parameters.Add("side", side.ToString());
                    cmd.Parameters.Add("type", type.ToString());
                    cmd.Parameters.Add("mediatype", mediatype.ToString());
                    cmd.ExecuteNonQuery();
                }
            }

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid));
        }

        public void CheckCardMediaId(int id, int cardid)
        {
            using (NpgsqlConnection conn = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"Cards_MediaContent\" WHERE media_id=:id AND cards_id=:cardid;";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("cardid", cardid);
                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(id);
                }
            }
        }

        public IList<int> GetMediaResources(int lmid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT cc.media_id FROM \"Cards_MediaContent\" AS cc JOIN \"LearningModules_Cards\" AS lc ON cc.cards_id = lc.cards_id WHERE lc.lm_id=:lmid;";
                    cmd.Parameters.Add("lmid", lmid);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    IList<int> ids = new List<int>();
                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["media_id"]));

                    return ids;
                }
            }
        }

        #endregion

        private IList<int> GetCardMedia(int cardid, Side side, WordType? type)
        {
            IList<int> ids = new List<int>();
            IList<IMedia> mediaList = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardMedia, cardid)] as IList<IMedia>;
            if (mediaList != null)
            {
                foreach(DbMedia cms in mediaList)
                    if(cms.Side == side && (!type.HasValue || cms.Type == type))
                        ids.Add(cms.Id);

                return ids;
            }

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, media_type,side,type,is_default FROM \"Cards_MediaContent\" JOIN \"MediaContent\" ON media_id=id WHERE cards_id=:cardid;";
                    cmd.Parameters.Add("cardid", cardid);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

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

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardMedia, cardid)] = mediaList;

                    return ids;
                }
            }
        }
    }
}
