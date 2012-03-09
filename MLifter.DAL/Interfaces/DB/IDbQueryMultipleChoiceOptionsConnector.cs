using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbQueryMultipleChoiceOptionsConnector
    {
        void CheckId(int id);

        bool? GetAllowMultiple(int id);
        void SetAllowMultiple(int id, bool? AllowMultiple);

        bool? GetAllowRandom(int id);
        void SetAllowRandom(int id, bool? AllowRandom);

        int? GetMaxCorrect(int id);
        void SetMaxCorrect(int id, int? MaxCorrect);

        int? GetChoices(int id);
        void SetChoices(int id, int? Choices);
    }
}
