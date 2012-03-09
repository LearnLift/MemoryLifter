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
    public class QueryDirectionsTemplate : IQueryDirections
    {
        public QueryDirectionsTemplate() { }

        #region IQueryDirections Members
        [ValueCopyAttribute]
        public bool? Question2Answer { get; set; }

        [ValueCopyAttribute]
        public bool? Answer2Question { get; set; }

        [ValueCopyAttribute]
        public bool? Mixed { get; set; }
        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, MLifter.DAL.Tools.CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryDirections), progressDelegate);
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
