using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbBoxes : IBoxes
    {
        public const int m_numberOfBoxes = 10;

        internal DbBoxes(ParentClass parent)
        {
            this.parent = parent;
        }

        #region IBoxes Members

        public List<IBox> Box
        {
            get
            {
                List<IBox> boxes = new List<IBox>();
                for (int i = 0; i <= m_numberOfBoxes; i++) //including pool and box 10
                    boxes.Add(new DbBox(i, Parent.GetChildParentClass(this)));
                return boxes;
            }
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
