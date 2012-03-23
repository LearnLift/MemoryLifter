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
