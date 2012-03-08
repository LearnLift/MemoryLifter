using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// The database connector for the IWord interface.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-31</remarks>
    interface IDbWordConnector
    {
        bool GetDefault(int id);
        void SetDefault(int id, bool Default);

        WordType GetType(int id);
        void SetType(int id, WordType Type);

        string GetWord(int id);
        void SetWord(int id, string Word);
    }
}
