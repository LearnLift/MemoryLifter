using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Security;
using SecurityFramework;
using MLifter.DAL.Tools;

namespace MLifterSettingsManager.DAL
{
    [Serializable()]
    public class QueryTypeTemplate : IQueryType
    {
        public QueryTypeTemplate() { }

        #region IQueryType Members
        [ValueCopyAttribute]
        public bool? ImageRecognition { get; set; }

        [ValueCopyAttribute]
        public bool? ListeningComprehension { get; set; }

        [ValueCopyAttribute]
        public bool? MultipleChoice { get; set; }

        [ValueCopyAttribute]
        public bool? Sentence { get; set; }

        [ValueCopyAttribute]
        public bool? Word { get; set; }
        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryType), progressDelegate);
        }

        #endregion

        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
