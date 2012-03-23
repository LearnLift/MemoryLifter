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
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    class XmlQueryDirections : IQueryDirections
    {
        public XmlQueryDirections(ParentClass parent)
        {
            this.parent = parent;
        }

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        bool? q2a, a2q, mixed;

        public override bool Equals(Object obj)
        {
            return QueryDirectionsHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryDirections Members

        public bool? Question2Answer
        {
            get
            {
                return q2a;
            }
            set
            {
                if (value == q2a)
                    return;

                q2a = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? Answer2Question
        {
            get
            {
                return a2q;
            }
            set
            {
                if (value == a2q)
                    return;

                a2q = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? Mixed
        {
            get
            {
                return mixed;
            }
            set
            {
                if (value == mixed)
                    return;

                mixed = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryDirections), progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion
    }
}
