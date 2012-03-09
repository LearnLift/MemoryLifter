using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using System.Threading;
using System.ComponentModel;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Add words to an existing List
    /// </summary>
    /// <remarks>Documented by Dev11, 2008-07-25</remarks>
    class DbWords : MLifter.DAL.Interfaces.IWords
    {
        private IDbWordsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlWordsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeWordsConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }
        private int id;
        private Side side;
        private WordType type;

        public DbWords(int CardId, Side ListSide, WordType ListType, ParentClass parentClass)
        {
            id = CardId;
            side = ListSide;
            type = ListType;
            parent = parentClass;
        }
        public DbWords(int CardId, Side ListSide, WordType ListType, IList<string> existingWords, ParentClass parentClass)
            : this(CardId, ListSide, ListType, parentClass)
        {
            AddWords(existingWords);
        }

        public void AddWords(IList<string> existingWords)
        {
            List<IWord> words = new List<IWord>();
            foreach (string word in existingWords)
                words.Add(CreateWord(word, type, false));
            AddWords(words);
        }

        /// <summary>
        /// Builds the string array from the IList&lt;Word&gt; words.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-07-28</remarks>
        public string[] BuildStringArray()
        {
            IList<IWord> words = Words;
            string[] wordArray = new string[words.Count];
            int i = 0;
            foreach (IWord word in words)
            {
                wordArray[i] = word.Word;
                i++;
            }
            return wordArray;
        }

        public override string ToString()
        {
            return String.Join(", ", BuildStringArray());
        }

        #region IWords Members

        public System.Globalization.CultureInfo Culture
        {
            get
            {
                return (side == Side.Answer) ? Parent.GetParentDictionary().DefaultSettings.AnswerCulture : Parent.GetParentDictionary().DefaultSettings.QuestionCulture;
            }
        }

        public IList<IWord> Words
        {
            get { return connector.GetTextContent(id, side, type); }
        }

        public IWord CreateWord(string word, WordType type, bool isDefault)
        {
            return connector.CreateNewWord(id, word == null ? string.Empty : word, side, type, isDefault);
        }

        public void AddWord(IWord word)
        {
            if (word == null) throw new NullReferenceException("The word must not be null");
            connector.AddWord(id, side, type, word);
        }

        public void AddWords(string[] words)
        {
            List<IWord> wordList = new List<IWord>();
            foreach (string aWord in words)
                wordList.Add(CreateWord(aWord, type, false));
            AddWords(wordList);
        }

        public void AddWords(List<IWord> words)
        {
            connector.AddWords(id, side, type, words);
        }

        public void ClearWords()
        {
            connector.ClearAllWords(id, side, type);
        }

        public string ToQuotedString()
        {
            string quotedString = null;
            quotedString = string.Join("\", \"", BuildStringArray());
            if (quotedString.Length > 0)
                quotedString = "\"" + quotedString + "\"";
            return quotedString;
        }

        public string ToNewlineString()
        {
            return string.Join(Environment.NewLine, BuildStringArray());
        }

        /// <summary>
        /// Returns all words as a list of strings.
        /// </summary>
        /// <returns>The words.</returns>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        public IList<string> ToStringList()
        {
            IList<string> words = new List<string>();
            foreach (IWord word in Words)
                words.Add(word.Word);
            return words;
        }
        #endregion

        #region ICopy Members

        public void CopyTo(ICopy target, CopyToProgress progressDelegate)
        {
            WordsHelper.CopyWords(this, target as IWords);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion
    }
}
