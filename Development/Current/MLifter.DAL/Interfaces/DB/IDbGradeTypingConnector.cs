using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbGradeTypingConnector
    {
        void CheckId(int id);

        bool? GetAllCorrect(int id);
        void SetAllCorrect(int id, bool? AllCorrect);

        bool? GetHalfCorrect(int id);
        void SetHalfCorrect(int id, bool? HalfCorrect);

        bool? GetNoneCorrect(int id);
        void SetNoneCorrect(int id, bool? NoneCorrect);

        bool? GetPrompt(int id);
        void SetPrompt(int id, bool? Prompt);
    }
}
