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
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE connector for the IWords interface.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-31</remarks>
    class MsSqlCeWordsConnector : IDbWordsConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeWordsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeWordsConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public static MsSqlCeWordsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeWordsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeWordsConnector(ParentClass parentClass)
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

        /// <summary>
        /// Creates the new word.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="word">The word.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public IWord CreateNewWord(int id, string word, Side side, WordType type, bool isDefault)
        {
            if (word != null)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT position FROM TextContent WHERE cards_id=@id AND side=@side AND type=@type ORDER BY position DESC";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@side", side.ToString());
                cmd.Parameters.Add("@type", type.ToString());

                int currentPos = 0;
                object retval = MSSQLCEConn.ExecuteScalar(cmd);
                if (retval != DBNull.Value)
                    currentPos = Convert.ToInt32(retval);
                cmd.Parameters.Clear();

                cmd.CommandText = @"INSERT INTO TextContent (cards_id, text, side, type, position, is_default) VALUES (@id, @text, @side, @type, @position, @isdefault); SELECT @@IDENTITY;";
                cmd.Parameters.Add("@id", id);
                cmd.Parameters.Add("@text", word);
                cmd.Parameters.Add("@side", side.ToString());
                cmd.Parameters.Add("@type", type.ToString());
                cmd.Parameters.Add("@position", currentPos + 10);
                cmd.Parameters.Add("@isdefault", isDefault);

                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));

                return new DbWord(Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)), word, type, isDefault, Parent);
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the content of the text.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public IList<IWord> GetTextContent(int id, Side side, WordType type)
        {
            return GetWords(id, side, type);
        }
        /// <summary>
        /// Clears all words.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public void ClearAllWords(int id, Side side, WordType type)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "DELETE FROM TextContent WHERE cards_id=@id AND side=@side AND type=@type";
            cmd.Parameters.Add("@id", id);
            cmd.Parameters.Add("@side", side.ToString());
            cmd.Parameters.Add("@type", type.ToString());
            MSSQLCEConn.ExecuteNonQuery(cmd);

            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));
        }
        /// <summary>
        /// Adds the word.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <param name="word">The word.</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        public void AddWord(int id, Side side, WordType type, IWord word)
        {
            if (word != null && word.Word.Length > 0)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT count(*) FROM TextContent WHERE id = @wordid AND text = @word AND type = @type AND is_default = @isdefault;";
                cmd.Parameters.Add("@wordid", word.Id);
                cmd.Parameters.Add("@word", word.Word);
                cmd.Parameters.Add("@type", type.ToString());
                cmd.Parameters.Add("@isdefault", word.Default);
                bool wordExists = (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) > 0);
                if (!wordExists)
                {
                    cmd.CommandText = "SELECT position FROM TextContent WHERE cards_id=@id AND side=@side AND type=@type ORDER BY position DESC";
                    cmd.Parameters.Add("@id", id);
                    cmd.Parameters.Add("@side", side.ToString());
                    cmd.Parameters.Add("@type", type.ToString());

                    int currentPos = 0;
                    object retval = MSSQLCEConn.ExecuteScalar(cmd);
                    if (retval != DBNull.Value)
                        currentPos = Convert.ToInt32(retval);
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"INSERT INTO TextContent (cards_id, text, side, type, position, is_default) VALUES (@id, @text, @side, @type, @position, @isdefault); SELECT @@IDENTITY;";
                    cmd.Parameters.Add("@id", id);
                    cmd.Parameters.Add("@text", word.Word);
                    cmd.Parameters.Add("@side", side.ToString());
                    cmd.Parameters.Add("@type", type.ToString());
                    cmd.Parameters.Add("@position", currentPos + 10);
                    cmd.Parameters.Add("@isdefault", word.Default);
                    MSSQLCEConn.ExecuteNonQuery(cmd);
                }

                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));
            }
        }

        /// <summary>
        /// Adds the words.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <param name="words">The words.</param>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        /// <remarks>Documented by Dev08, 2009-01-19</remarks>
        public void AddWords(int id, Side side, WordType type, List<IWord> words)
        {
            if (words.Count > 0)
            {
                //SqlCeCommand cmd1 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                //SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                //SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);

                //SqlCeParameter paramWordId = new SqlCeParameter("@id", SqlDbType.Int);
                //SqlCeParameter paramCardId = new SqlCeParameter("@cardid", SqlDbType.Int);
                //SqlCeParameter paramType = new SqlCeParameter("@type", SqlDbType.NVarChar);
                //SqlCeParameter paramIsDefault = new SqlCeParameter("@isdefault", SqlDbType.Bit);
                //cmd1.CommandText = "SELECT count(*) FROM TextContent WHERE id = @wordid AND text = @word AND type = @type AND is_default = @isdefault;";
                //cmd1.Parameters.Add(paramWordId);
                //cmd1.Parameters.Add(paramCardId);
                //cmd1.Parameters.Add(paramType);
                //cmd1.Parameters.Add(paramIsDefault);

                //SqlCeParameter paramSide = new SqlCeParameter("@side", SqlDbType.NVarChar);
                //cmd2.CommandText = "SELECT position FROM TextContent WHERE cards_id=@id AND side=@side AND type=@type ORDER BY position DESC";
                //cmd2.Parameters.Add(paramWordId);
                //cmd2.Parameters.Add(paramSide);
                //cmd2.Parameters.Add(paramType);

                //SqlCeParameter paramText = new SqlCeParameter("@text", SqlDbType.NText);
                //SqlCeParameter paramPosition = new SqlCeParameter("@position", SqlDbType.Int);
                //cmd3.CommandText = @"INSERT INTO TextContent (cards_id, text, side, type, position, is_default) VALUES (@id, @text, @:side, @type, @position, @isdefault); SELECT @@IDENTITY;";
                //cmd3.Parameters.Add(paramCardId);
                //cmd3.Parameters.Add(paramText);
                //cmd3.Parameters.Add(paramSide);
                //cmd3.Parameters.Add(paramType);
                //cmd3.Parameters.Add(paramPosition);
                //cmd3.Parameters.Add(paramIsDefault);

                foreach (IWord word in words)
                {
                    if (word != null && word.Word.Length > 0)
                    {
                        //paramCardId.Value = id;
                        //paramWordId.Value = word.Id;
                        //paramText.Value = word.Word;
                        //paramSide.Value = side.ToString();
                        //paramType.Value = type.ToString();
                        //paramIsDefault.Value = word.Default;

                        SqlCeCommand cmd1 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                        cmd1.CommandText = "SELECT count(*) FROM TextContent WHERE id = @wordid AND text = @word AND type = @type AND is_default = @isdefault;";
                        cmd1.Parameters.Add("@wordid", word.Id);
                        cmd1.Parameters.Add("@word", word.Word);
                        cmd1.Parameters.Add("@type", type.ToString());
                        cmd1.Parameters.Add("@isdefault", word.Default);

                        bool wordExists = (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd1)) > 0);
                        if (!wordExists)
                        {
                            int currentPos = 0;
                            SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                            cmd2.CommandText = "SELECT position FROM TextContent WHERE cards_id=@id AND side=@side AND type=@type ORDER BY position DESC";
                            cmd2.Parameters.Add("@id", id);
                            cmd2.Parameters.Add("@side", side.ToString());
                            cmd2.Parameters.Add("@type", type.ToString());

                            object retval = MSSQLCEConn.ExecuteScalar(cmd2);
                            if (retval != DBNull.Value)
                                currentPos = Convert.ToInt32(retval);

                            //paramPosition.Value = currentPos + 10;
                            SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                            cmd3.CommandText = "INSERT INTO TextContent (cards_id, text, side, type, position, is_default) VALUES (@id, @word, @side, @type, @position, @isdefault); SELECT @@IDENTITY;";
                            cmd3.Parameters.Add("@id", id);
                            cmd3.Parameters.Add("@word", word.Word);
                            cmd3.Parameters.Add("@side", side.ToString());
                            cmd3.Parameters.Add("@type", type.ToString());
                            cmd3.Parameters.Add("@position", currentPos + 10);
                            cmd3.Parameters.Add("@isdefault", word.Default);
                            MSSQLCEConn.ExecuteNonQuery(cmd3);
                        }
                    }
                }

                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(ObjectLifetimeIdentifier.GetCacheObject(side, type), id));
            }
        }

        #endregion

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <param name="textid">The textid.</param>
        /// <param name="cardid">The cardid.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
        private DbWord GetWord(int textid, int cardid)
        {
            DbWord wordCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Word, textid)] as DbWord;
            if (wordCache != null)
                return wordCache;

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT id, text, type, is_default FROM TextContent WHERE cards_id=@id;";
            cmd.Parameters.Add("@id", cardid);
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

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
            reader.Close();

            return wordCache;
        }

        /// <summary>
        /// Gets the words.
        /// </summary>
        /// <param name="cardid">The cardid.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-09</remarks>
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

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT id, side, type, text, is_default FROM TextContent WHERE cards_id=@id ORDER BY position ASC;";
            cmd.Parameters.Add("@id", cardid);
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

            while (reader.Read())
            {
                object[] word = new object[5];
                reader.GetValues(word);
                int id = Convert.ToInt32(word[0]);
                Side wside = (Side)Enum.Parse(typeof(Side), word[1].ToString(), true);
                WordType wtype = (WordType)Enum.Parse(typeof(WordType), word[2].ToString(), false);
                string text = word[3].ToString();
                bool isDefault = Convert.ToBoolean(word[4]);

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
            reader.Close();

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
