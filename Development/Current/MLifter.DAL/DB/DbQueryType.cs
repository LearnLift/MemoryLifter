using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.DB
{
    class DbQueryType:IQueryType
    {
        #region IQueryType Members

        public bool? ImageRecognition
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return false;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        public bool? ListeningComprehension
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return false;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        public bool? MultipleChoice
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return false;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        public bool? Sentence
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return false;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        public bool? Word
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return true;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
