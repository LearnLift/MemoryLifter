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
	internal class PreviewWords : Interfaces.IWords
	{
		public PreviewWords(ParentClass parent)
		{
			this.parent = parent;
		}

		#region IWords Members

		public System.Globalization.CultureInfo Culture
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture; }
		}

		List<IWord> words = new List<IWord>();
		public IList<IWord> Words
		{
			get { return words; }
		}

		public IWord CreateWord(string word, WordType type, bool isDefault)
		{
			return new PreviewWord(word, type, isDefault, null);
		}

		public void AddWord(IWord word)
		{
			if (!(word is PreviewWord))
				throw new Exception("Only Preview word objects can be added.");

			words.Add(word);
		}

		public void AddWords(string[] words)
		{
			bool isDefault = true;
			foreach (string wordstring in words)
			{
				AddWord(CreateWord(wordstring, WordType.Sentence, isDefault));
				isDefault = false;
			}
		}

		public void AddWords(List<IWord> words)
		{
			words.AddRange(words);
		}

		public void ClearWords()
		{
			words.Clear();
		}

		public string ToQuotedString()
		{
			string quotedString = null;
			quotedString = string.Join("\", \"", BuildStringArray());
			quotedString = "\"" + quotedString + "\"";
			return quotedString;
		}

		public string ToNewlineString()
		{
			return string.Join(Environment.NewLine, BuildStringArray());
		}

		/// <summary>
		/// Returns all words as a list of strings.
		/// </summary>
		/// <returns>The words.</returns>
		/// <remarks>Documented by Dev03, 2009-04-14</remarks>
		public IList<string> ToStringList()
		{
			IList<string> words = new List<string>();
			foreach (IWord word in Words)
				words.Add(word.Word);
			return words;
		}

		#endregion

		/// <summary>
		/// Builds the string array from the IList&lt;Word&gt; words.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev11, 2008-07-28</remarks>
		public string[] BuildStringArray()
		{
			IList<IWord> words = Words;
			string[] wordArray = new string[words.Count];
			int i = 0;
			foreach (IWord word in words)
			{
				wordArray[i] = word.Word;
				i++;
			}
			return wordArray;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return String.Join(", ", BuildStringArray());
		}

		#region ICopy Members

		public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
		{
			WordsHelper.CopyWords(this, target as IWords);
		}

		#endregion

		#region IParent Members

		private ParentClass parent;

		public MLifter.DAL.Tools.ParentClass Parent
		{
			get { return parent; }
		}

		#endregion
	}
}
