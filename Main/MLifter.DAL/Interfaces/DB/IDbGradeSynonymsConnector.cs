using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbGradeSynonymsConnector
    {
        void CheckId(int id);

        bool? GetAllKnown(int id);
        void SetAllKnown(int id, bool? AllKnown);

        bool? GetHalfKnown(int id);
        void SetHalfKnown(int id, bool? HalfKnown);

        bool? GetOneKnown(int id);
        void SetOneKnown(int id, bool? OneKnown);

        bool? GetFirstKnown(int id);
        void SetFirstKnown(int id, bool? FirstKnown);

        bool? GetPrompt(int id);
        void SetPrompt(int id, bool? Prompt);
    }
}
