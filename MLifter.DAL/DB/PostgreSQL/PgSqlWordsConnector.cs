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
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    /// <summary>
    /// The PostgreSQL connector for the IWords interface.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-31</remarks>
    class PgSqlWordsConnector : IDbWordsConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlWordsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlWordsConnector>();
        public static PgSqlWordsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlWordsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlWordsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbWordsConnector Members

        public IWord CreateNewWord(int id, string word, Side side, WordType type, bool isDefault)
        {
            if (word != null)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO \"TextContent\" (cards_id, text, side, type, position, is_default) VALUES (:id, :text, :side, :type, " +
                            "(COALESCE((SELECT position FROM \"TextContent\" WHERE cards_id=:id AND side=:side AND type=:type ORDER BY position DESC LIMIT 1), 0) + 10), " +
                            ":isdefault) RETURNING id";
                        cmd.Parameters.Add("id", id);
                        cmd.Parameters.Add("text", word);
                        cmd.Parameters.Add("side", side.ToString());
                        cmd.Parameters.Add("type", type.ToString());
                        cmd.Parameters.Add("isdefault", isDefault);

                        Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));

                        return new DbWord(Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)), word, type, isDefault, Parent);
                    }
                }
            }
            else
                return null;
        }

        public IList<IWord> GetTextContent(int id, Side side, WordType type)
        {
            return GetWords(id, side, type);
        }
        public void ClearAllWords(int id, Side side, WordType type)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"TextContent\" WHERE cards_id=:id AND side=:side AND type=:type";
                    cmd.Parameters.Add("id", id);
                    cmd.Parameters.Add("side", side.ToString());
                    cmd.Parameters.Add("type", type.ToString());
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));
                }
            }
        }
        public void AddWord(int id, Side side, WordType type, IWord word)
        {
            if (word != null && word.Word.Length > 0)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT \"InsertWordIfNotExists\"(:id,:cardid,:isdefault,:text,:side,:type);";
                        cmd.Parameters.Add("id", word.Id);
                        cmd.Parameters.Add("isdefault", word.Default);
                        cmd.Parameters.Add("cardid", id);
                        cmd.Parameters.Add("text", word.Word);
                        cmd.Parameters.Add("side", side.ToString());
                        cmd.Parameters.Add("type", type.ToString());
                        PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                        Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));
                    }
                }
            }
        }
        public void AddWords(int id, Side side, WordType type, List<IWord> words)
        {
            if (words.Count > 0)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = string.Empty;
                        int paramnum = 0;
                        string textparamname, idparametername, isdefaultparametername;
                        foreach (IWord word in words)
                        {
                            if (word != null && word.Word.Length > 0)
                            {
                                paramnum++;
                                textparamname = string.Format("text{0}", paramnum);
                                idparametername = string.Format("id{0}", paramnum);
                                isdefaultparametername = string.Format("isdefault{0}", paramnum);

                                cmd.CommandText += string.Format("SELECT \"InsertWordIfNotExists\"(:{0},:cardid,:{1},:{2},:side,:type);",
                                    idparametername, isdefaultparametername, textparamname);

                                cmd.Parameters.Add(textparamname, word.Word);
                                cmd.Parameters.Add(idparametername, word.Id);
                                cmd.Parameters.Add(isdefaultparametername, word.Default);
                            }
                        }

                        cmd.Parameters.Add("cardid", id);
                        cmd.Parameters.Add("side", side.ToString());
                        cmd.Parameters.Add("type", type.ToString());
                        PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                        Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));
                    }
                }
            }
        }

        #endregion

        private DbWord GetWord(int textid, int cardid)
        {
            DbWord wordCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Word, textid)] as DbWord;
            if (wordCache != null)
                return wordCache;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, text, type, is_default FROM \"TextContent\" WHERE cards_id=:id;";
                    cmd.Parameters.Add("id", cardid);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    IList<IWord> list = new List<IWord>();
                    while (reader.Read())
                    {
                        int tid = Convert.ToInt32(reader["id"]);

                        DbWord word = new DbWord(tid, reader["text"].ToString(),
                            (WordType)Enum.Parse(typeof(WordType), reader["type"].ToString(), true), Convert.ToBoolean(reader["is_default"]), Parent);

                        if (tid == textid)
                            wordCache = word;

                        if (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Word, tid)] == null)
                            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.Word, tid)] = word;
                    }
                }
            }

            return wordCache;
        }

        private IList<IWord> GetWords(int cardid, Side side, WordType type)
        {
            CacheObject obj = ObjectLifetimeIdentifier.GetCacheObject(side, type);

            IList<IWord> wordsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(obj, cardid)] as IList<IWord>;
            if (wordsCache != null)
                return wordsCache;

            IList<IWord> question = new List<IWord>();
            IList<IWord> questionExample = new List<IWord>();
            IList<IWord> questionDistractor = new List<IWord>();
            IList<IWord> answer = new List<IWord>();
            IList<IWord> answerExample = new List<IWord>();
            IList<IWord> answerDistractor = new List<IWord>();

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, side, type, text, is_default FROM \"TextContent\" WHERE cards_id=:id ORDER BY position ASC;";
                    cmd.Parameters.Add("id", cardid);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        Side wside = (Side)Enum.Parse(typeof(Side), reader["side"].ToString(), true);
                        WordType wtype = (WordType)Enum.Parse(typeof(WordType), reader["type"].ToString(), false);
                        string text = reader["text"].ToString();
                        bool isDefault = Convert.ToBoolean(reader["is_default"]);

                        switch (wside)
                        {
                            case Side.Question:
                                switch (wtype)
                                {
                                    case WordType.Word:
                                        question.Add(new DbWord(id, text, wtype, isDefault, Parent));
                                        break;
                                    case WordType.Sentence:
                                        questionExample.Add(new DbWord(id, text, wtype, isDefault, Parent));
                                        break;
                                    case WordType.Distractor:
                                        questionDistractor.Add(new DbWord(id, text, wtype, isDefault, Parent));
                                        break;
                                }
                                break;
                            case Side.Answer:
                                switch (wtype)
                                {
                                    case WordType.Word:
                                        answer.Add(new DbWord(id, text, wtype, isDefault, Parent));
                                        break;
                                    case WordType.Sentence:
                                        answerExample.Add(new DbWord(id, text, wtype, isDefault, Parent));
                                        break;
                                    case WordType.Distractor:
                                        answerDistractor.Add(new DbWord(id, text, wtype, isDefault, Parent));
                                        break;
                                }
                                break;
                        }
                    }
                }
            }

            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.QuestionWords, cardid)] = question;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.QuestionExampleWords, cardid)] = questionExample;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.QuestionDistractorWords, cardid)] = questionDistractor;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AnswerWords, cardid)] = answer;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AnswerExampleWords, cardid)] = answerExample;
            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.AnswerDistractorWords, cardid)] = answerDistractor;

            switch (side)
            {
                case Side.Question:
                    switch (type)
                    {
                        case WordType.Word:
                            return question;
                        case WordType.Sentence:
                            return questionExample;
                        case WordType.Distractor:
                            return questionDistractor;
                    }
                    break;
                case Side.Answer:
                    switch (type)
                    {
                        case WordType.Word:
                            return answer;
                        case WordType.Sentence:
                            return answerExample;
                        case WordType.Distractor:
                            return answerDistractor;
                    }
                    break;
            }

            return new List<IWord>();
        }
    }
}
