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
    public class QueryMultipleChoiceOptionsTemplate : IQueryMultipleChoiceOptions
    {
        public QueryMultipleChoiceOptionsTemplate() { }

        #region IQueryMultipleChoiceOptions Members
        [ValueCopyAttribute]
        public bool? AllowRandomDistractors { get; set; }

        [ValueCopyAttribute]
        public bool? AllowMultipleCorrectAnswers { get; set; }

        [ValueCopyAttribute]
        public int? NumberOfChoices { get; set; }

        [ValueCopyAttribute]
        public int? MaxNumberOfCorrectAnswers { get; set; }
        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryMultipleChoiceOptions), progressDelegate);
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
