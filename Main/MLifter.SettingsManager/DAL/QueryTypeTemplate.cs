/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
