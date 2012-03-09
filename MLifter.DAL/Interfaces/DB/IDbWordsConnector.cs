using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// The database connector for the IWords interface.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-28</remarks>
    interface IDbWordsConnector
    {
        IWord CreateNewWord(int id, string word, Side side, WordType type, bool isDefault);
        IList<IWord> GetTextContent(int id, Side side, WordType type);

        void ClearAllWords(int id, Side side, WordType type);
        void AddWord(int id, Side side, WordType type, IWord word);
        void AddWords(int id, Side side, WordType type, List<IWord> words);
    }
}
