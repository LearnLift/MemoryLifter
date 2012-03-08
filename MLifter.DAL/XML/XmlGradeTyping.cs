using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    class XmlGradeTyping : IGradeTyping
    {
        public XmlGradeTyping(ParentClass parent)
        {
            this.parent = parent;
        }

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        private bool? allCorrect, halfCorrect, noneCorrect, prompt;

        public override bool Equals(Object obj)
        {
            return GradeTypingHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGradeTyping Members

        public bool? AllCorrect
        {
            get { return allCorrect; }
            set
            {
                if (value == allCorrect)
                    return;

                allCorrect = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? HalfCorrect
        {
            get { return halfCorrect; }
            set
            {
                if (value == halfCorrect)
                    return;

                halfCorrect = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        public bool? NoneCorrect
        {
            get { return noneCorrect; }
            set
            {
                if (value == noneCorrect)
                    return;

                noneCorrect = value;
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
            CopyBase.Copy(this, target, typeof(IGradeTyping), progressDelegate);
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
