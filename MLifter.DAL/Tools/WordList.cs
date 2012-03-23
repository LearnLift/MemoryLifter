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

namespace MLifter.DAL
{

	/// <summary>
	/// WordList can take a Comma-separated Text and split it into single strings.
	/// </summary>
	/// <returns>No return value</returns>
	/// <exceptions>Does not throw any exception.</exceptions>
	/// <remarks>Documented by Dev00, 2007-07-19</remarks>
	[Obsolete("Do not use this anymore. Use List<IWord> or string[]!")]
	internal class WordList
	{
		private string[] m_words;

		/// <summary>
		/// Gets/Sets the comma delimited text
		/// </summary>
		/// <returns>Returns text</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public string CommaText
		{
			get
			{
				return String.Join(", ", m_words);
			}
			set
			{
				m_words = SplitWordList(value);
			}
		}

		/// <summary>
		/// Gets/Sets the quoted, comma delimited text
		/// </summary>
		/// <returns>Returns text</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public string QuotedCommaText
		{
			get
			{
				string[] words = (string[])m_words.Clone();
				//encode some protected chars ('"' and ',') 
				for (int i = 0; i < words.Length; i++)
				{
					words[i] = words[i].Replace("\"", "\"\"").Replace(",", "\\,");
				}
				return "\"" + String.Join("\", \"", words) + "\"";
			}
			set
			{
				m_words = SplitWordList(value);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.String"/> at the specified index.
		/// </summary>
		/// <value></value>
		/// <remarks>Documented by Dev03, 2007-08-27</remarks>
		public string this[int index]
		{
			get { return index >= m_words.Length || index < 0 ? null : m_words[index]; }
			set
			{
				AdjustArraySize(index);
				m_words[index] = value;
			}
		}


		/// <summary>
		/// Counts length of Arry
		/// </summary>
		/// <returns>Returns length of array</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public int Count
		{
			get { return m_words.Length; }
		}

		/// <summary>
		/// Deletes a sign from an array
		/// </summary>
		/// <param name="index">Index of sighn that should be deleted</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public void Delete(int index)
		{
			if (index >= m_words.Length) return;
			for (int i = index; i < m_words.Length - 1; i++)
				m_words[i] = m_words[i + 1];
			string[] temp = new string[m_words.Length - 1];
			for (int i = 0; i < m_words.Length - 1; i++)
				temp[i] = m_words[i];
			m_words = new string[m_words.Length - 1];
			temp.CopyTo(m_words, 0);
		}

		/// <summary>
		/// Returns index of element
		/// </summary>
		/// <param name="element">Element that is looked for</param>
		/// <returns>Index of element</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public int IndexOf(string element)
		{
			for (int i = 0; i < m_words.Length; i++)
				if (m_words[i].Equals(element))
					return i;
			return (-1);
		}

		/// <summary>
		/// An element is appended at the end of the array
		/// </summary>
		/// <param name="element">Element that should be appended</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public void Append(string element)
		{
			AdjustArraySize(m_words.Length);
			m_words[m_words.Length - 1] = element;
		}

		/// <summary>
		/// An element is appended at a specific position
		/// </summary>
		/// <param name="element">Element that should be appended</param>
		/// <param name="position">Position at which element should be appended</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public void Insert(int position, string element)
		{
			if (position < 0 || position > m_words.Length)
				return;

			AdjustArraySize(m_words.Length);
			for (int count = m_words.Length - 1; count > position; count--)
				m_words[count] = m_words[count - 1];
			m_words[position] = element;
		}

		/// <summary>
		/// Returns array
		/// </summary>
		/// <returns>
		/// Returns array
		/// </returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>
		/// Documented by BBI, 2007-07-19
		/// </remarks>
		public string[] GetArray()
		{
			return m_words;
		}

		/// <summary>
		/// Creates new array with data from parameter
		/// </summary>
		/// <param name="new_array">Array</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public void SetArray(string[] new_array)
		{
			m_words = new_array;
		}

		/// <summary>
		/// Swaps entries in an array
		/// </summary>
		/// <param name="index1">Index of entry 1</param>
		/// <param name="index2">Index of entry 2</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public void Swap(int index1, int index2)
		{
			string temp = m_words[index1];
			m_words[index1] = m_words[index2];
			m_words[index2] = temp;
		}

		/// <summary>
		/// Adjusts size of array (only if new size is bigger than old size)
		/// </summary>
		/// <param name="index">New length of array</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		private void AdjustArraySize(int index)
		{
			if (index >= m_words.Length)
			{
				string[] temp = new string[m_words.Length];
				m_words.CopyTo(temp, 0);
				m_words = new string[index + 1];
				temp.CopyTo(m_words, 0);
			}
		}

		/// <summary>
		/// Splits the word list.
		/// </summary>
		/// <param name="wordList">The word list.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2007-09-28</remarks>
		private string[] SplitWordList(string wordList)
		{
			string[] words;
			if (wordList != null)
			{
				//wordList = wordList.Trim(new char[] { ',', ' ' });
				if (wordList.Length > 0)
				{
					words = wordList.Split(new string[] { "\",\"", "\", \"" }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < words.Length; i++)
					{
						words[i] = words[i].Trim();
					}
					//remove trailing and leading '"' which is used to enclose synonym text
					if (words.Length > 0)
					{
						if (words[0].StartsWith("\"")) words[0] = words[0].Substring(1);
						if (words[words.Length - 1].EndsWith("\"")) words[words.Length - 1] = words[words.Length - 1].Substring(0, words[words.Length - 1].Length - 1);
					}
					//decode some protected chars ('"' and ',') 
					for (int i = 0; i < words.Length; i++)
					{
						words[i] = words[i].Replace("\"\"", "\"").Replace("\\,", ",");
					}
				}
				else
				{
					words = new string[] { };
				}
			}
			else
			{
				words = new string[] { };
			}
			return words;
		}

		/// <summary>
		/// Helper Function: String to Int
		/// </summary>
		/// <param name="str">String that should be converted</param>
		/// <returns>Integer value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public static int StrToInt(string str)
		{
			int val = 0;
			for (int i = 0; i < str.Length; i++)
			{
				val += (int)Char.GetNumericValue(str[i]) * (int)Math.Pow(10, str.Length - i - 1);
			}
			return val;
		}

		/// <summary>
		/// Default constructor of class WordList. Initialises text array
		/// </summary>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public WordList()
		{
			m_words = new string[0];
		}

		/// <summary>
		/// Constructor of class WordList. Initialises text array with init_array
		/// </summary>
		/// <param name="init_array">The init_array.</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public WordList(string[] init_array)
		{
			m_words = init_array;
		}

		/// <summary>
		/// Constructor of class WordList. Initialiszs text array with a word list string.
		/// </summary>
		/// <param name="wordList">The word list.</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public WordList(string wordList)
		{
			this.CommaText = wordList;
		}

		/// <summary>
		/// Creates a new WordList.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-06</remarks>
		public static WordList Create(string text)
		{
			WordList wl = new WordList();
			wl.CommaText = text;
			return wl;
		}

		/// <summary>
		/// Creates a new WordList.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="splitChars">The split chars.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-06</remarks>
		public static WordList Create(string text, string[] splitChars)
		{
			string[] words = text.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
			WordList wl = new WordList(words);
			return wl;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// Returns the quoted, comma-delimited list of words.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// A quoted, comma-delimited list of words.
		/// </returns>
		/// <remarks>Documented by Dev03, 2007-09-13</remarks>
		public override string ToString()
		{
			return this.QuotedCommaText;
		}
	}

}
