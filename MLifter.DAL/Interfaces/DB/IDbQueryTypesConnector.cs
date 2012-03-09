using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbQueryTypesConnector
    {
        void CheckId(int id);

        bool? GetImageRecognition(int id);
        void SetImageRecognition(int id, bool? ImageRecognition);

        bool? GetListeningComprehension(int id);
        void SetListeningComprehension(int id, bool? ListeningComprehension);

        bool? GetMultipleChoice(int id);
        void SetMultipleChoice(int id, bool? MultipleChoice);

        bool? GetSentence(int id);
        void SetSentence(int id, bool? Sentence);

        bool? GetWord(int id);
        void SetWord(int id, bool? Word);
    }
}
