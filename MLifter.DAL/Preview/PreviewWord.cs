using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Preview
{
    internal class PreviewWord : IWord
    {
        public PreviewWord(string word, WordType type, bool isDefault, ParentClass parent)
        {
            this.word = word;
            this.type = type;
            this._default = isDefault;
            this.parent = parent;
        }

        #region IWord Members

        public int Id
        {
            get { return -1; }
        }

        private WordType type;
        public WordType Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool _default;
        public bool Default
        {
            get { return _default; }
            set { _default = value; }
        }

        private string word;
        public string Word
        {
            get { return word; }
            set { word = value; }
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
