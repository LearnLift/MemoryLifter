using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    class XmlGradeSynonyms : IGradeSynonyms
    {
        public XmlGradeSynonyms(ParentClass parent)
        {
            this.parent = parent;
        }

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        private bool? allKnown, halfKnown, oneKnown, firstKnown, prompt;

        public override bool Equals(Object obj)
        {
            return GradeSynonymsHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGradeSynonyms Members

        public bool? AllKnown
        {
            get { return allKnown; }
            set
            {
                if (value == allKnown)
                    return;

                allKnown = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? HalfKnown
        {
            get { return halfKnown; }
            set
            {
                if (value == halfKnown)
                    return;

                halfKnown = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? OneKnown
        {
            get { return oneKnown; }
            set
            {
                if (value == oneKnown)
                    return;

                oneKnown = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? FirstKnown
        {
            get { return firstKnown; }
            set
            {
                if (value == firstKnown)
                    return;

                firstKnown = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? Prompt
        {
            get { return prompt; }
            set
            {
                if (value == prompt)
                    return;

                prompt = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IGradeSynonyms), progressDelegate);
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
