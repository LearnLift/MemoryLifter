using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbQueryDirectionsConnector
    {
        void CheckId(int id);

        bool? GetQuestion2Answer(int id);
        void SetQuestion2Answer(int id, bool? Question2Answer);

        bool? GetAnswer2Question(int id);
        void SetAnswer2Question(int id, bool? Answer2Question);

        bool? GetMixed(int id);
        void SetMixed(int id, bool? Mixed);
    }
}
