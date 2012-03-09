using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace MLifter.DAL.Transformer.V17
{

	/// <summary>
	/// StringList can take arrayList Comma-separated Text and split it into single strings.
	/// </summary>
	/// <returns>No return value</returns>
	///   
	/// <exceptions>Does not throw any exception.</exceptions>
	/// <remarks>
	/// Documented by BBI, 2007-07-19
	/// </remarks>
	public class StringList
	{
		string[] TextArray;

		/// <summary>
		/// Sets the value of the properties p, q, index
		/// </summary>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public string SpecialImportFormat
		{
			set
			{
				int p = 0, q = 0, index = 0;
				while (p < value.Length)
				{
					if (value[p] == '"')
					{
						q = value.IndexOf('"', ++p);
						if (q != -1)
						{
							AdjustArraySize(index);
							TextArray[index] = value.Substring(p, q - p);
							index++;
							p = q + 2;
						}
					}
					else
					{
						q = value.IndexOf(',', p);
						if (q != -1)
						{
							AdjustArraySize(index);
							TextArray[index] = value.Substring(p, q - p);
							index++;
							p = q + 1;
						}
						else
						{
							AdjustArraySize(index);
							TextArray[index] = value.Substring(p, value.Length - p);
							index++;
							p = value.Length;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets/Sets the comma delimited text
		/// </summary>
			/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public string CommaText
		{
			get
			{
				return String.Join(", ", TextArray);
			}
			set
			{
				if (value.Trim().Length > 0)
				{
					TextArray = value.Split(new char[] { ',' });
					for (int i = 0; i < TextArray.Length; i++)
					{
						TextArray[i] = TextArray[i].Trim(new char[] { '"', ' ' });
					}
				}
			}
		}


		/// <summary>
		/// Returns or sets content of Text Array at delivered index in case index is not 0 and is not bigger than the length of the array
		/// </summary>
		/// <param name="index">Index for Array</param>
		/// <returns>Returns content of Array</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public string this[int index]
		{

			get { return index >= TextArray.Length || index < 0 ? null : TextArray[index]; }

			set
			{
				AdjustArraySize(index);
				TextArray[index] = value;
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
			get { return TextArray.Length; }
		}

		/// <summary>
		/// Deletes arrayList sign from an array
		/// </summary>
		/// <param name="index">Index of sighn that should be deleted</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public void Delete(int index)
		{
			if (index >= TextArray.Length) return;
			for (int i = index; i < TextArray.Length - 1; i++)
				TextArray[i] = TextArray[i + 1];
			string[] temp = new string[TextArray.Length - 1];
			for (int i = 0; i < TextArray.Length - 1; i++)
				temp[i] = TextArray[i];
			TextArray = new string[TextArray.Length - 1];
			temp.CopyTo(TextArray, 0);
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
			for (int i = 0; i < TextArray.Length; i++)
				if (TextArray[i].Equals(element))
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
			AdjustArraySize(TextArray.Length);
			TextArray[TextArray.Length - 1] = element;
		}

		/* buggy...ignores position!! by AWE at 07/21/06
		public void Insert(int position, string element)
		{
			string[] temp = new string[TextArray.Length];
			TextArray.CopyTo(temp,0);
			TextArray = new string[TextArray.Length+1];
			temp.CopyTo(TextArray,1);
			TextArray[0] = element;
		}*/

		/// <summary>
		/// An element is appended at arrayList specific position
		/// </summary>
		/// <param name="element">Element that should be appended</param>
		/// <param name="position">Position at which element should be appended</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>
		public void Insert(int position, string element)
		{
			if (position < 0 || position > TextArray.Length)
				return;

			AdjustArraySize(TextArray.Length);
			for (int count = TextArray.Length - 1; count > position; count--)
				TextArray[count] = TextArray[count - 1];
			TextArray[position] = element;
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
			return TextArray;
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
			TextArray = new_array;
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
			string temp = TextArray[index1];
			TextArray[index1] = TextArray[index2];
			TextArray[index2] = temp;
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
			if (index >= TextArray.Length)
			{
				string[] temp = new string[TextArray.Length];
				TextArray.CopyTo(temp, 0);
				TextArray = new string[index + 1];
				temp.CopyTo(TextArray, 0);
			}
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
		/// Default constructor of class StringList. Initialises text array
		/// </summary>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public StringList()
		{
			TextArray = new string[0];
		}

		/// <summary>
		/// Constructor of class StringList. Initialises text array with init_array
		/// </summary>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public StringList(string[] init_array)
		{
			TextArray = init_array;
		}
	}

	/// <summary>
	/// Tools for V1.7 conversion.
	/// </summary>
	public class Tools
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Tools"/> class.
		/// </summary>
		public Tools() { }

		/// <summary>
		/// Search for the index of no in arr 
		/// </summary>
		/// <param name="arr">Integer array</param>
		/// <param name="no">Integer value</param>
		/// <returns>Returns index of "no" in arr or -1 if "no" cannot be found</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static int IndexOf(int[] arr, int no)
		{
			for (int i = 0; i < arr.Length; i++)
				if (Math.Abs(arr[i]) == Math.Abs(no))
					return i;
			return -1;
		}

		/// <summary>
		/// Generates arrayList comma seperated list without quotes from arrayList StringList
		/// </summary>
		/// <param name="strings">StringList</param>
		/// <returns>Returns comma seperated list</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static string Comma(StringList strings)
		{
			string commalist = "";
			for (int i = 0; i < strings.Count; i++)
			{
				if (i < strings.Count - 1)
					commalist += strings[i] + ", ";
				else
					commalist += strings[i];
			}
			return commalist;
		}

		/// <summary>
		/// Generates arrayList comma seperated list without quotes from arrayList String with quotes
		/// </summary>
		/// <param name="comma_list_with_quotes">String</param>
		/// <returns>Returns comma seperated list</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static string Comma(string comma_list_with_quotes)
		{
			StringList strings = new StringList();
			strings.CommaText = comma_list_with_quotes;
			return Comma(strings);
		}

		/// <summary>
		/// Returns first entry
		/// </summary>
		/// <param name="data_with_quotes">String</param>
		/// <returns>Returns first entry</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static string FirstCommaEntry(string data_with_quotes)
		{
			StringList temp = new StringList();
			temp.CommaText = data_with_quotes;
			return temp[0];
		}

		/// <summary>
		/// Generates arrayList text with linefeed from arrayList StringList
		/// </summary>
		/// <param name="string_array">String array</param>
		/// <returns>Returns text</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static string PrettyPrint(string[] string_array)
		{
			StringList strings = new StringList(string_array);
			return PrettyPrint(strings);
		}

		/// <summary>
		/// Generates arrayList text with linefeed from arrayList StringList
		/// </summary>
		/// <param name="commalist">String</param>
		/// <returns>Returns text</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static string PrettyPrint(string commalist)
		{
			StringList strings = new StringList();
			strings.CommaText = commalist;
			return PrettyPrint(strings);
		}

		/// <summary>
		/// Generates linefeeds in arrayList text
		/// </summary>
		/// <param name="strings">StringList</param>
		/// <returns>Returns text</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static string PrettyPrint(StringList strings)
		{
			string prettytext = "";
			for (int i = 0; i < strings.Count; i++)
			{
				if (strings[i] != "")
				{
					if (i < strings.Count - 1)
						prettytext += strings[i] + "\r\n";
					else
						prettytext += strings[i];
				}
			}
			return prettytext;
		}

		/// <summary>
		/// Copies arrayList directory with subdirectories recursivly (function missing in System.IO.Directory)
		/// </summary>
		/// <param name="Src">String source</param>
		/// <param name="Dst">String destination</param>
		/// <returns>No return value</returns>
		/// <exceptions>Does not throw any exception.</exceptions>
		/// <remarks>Documented by Dev00, 2007-07-19</remarks>

		public static void copyDirectory(string Src, string Dst)
		{
			String[] Files;

			if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
				Dst += Path.DirectorySeparatorChar;
			if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
			Files = Directory.GetFileSystemEntries(Src);
			foreach (string Element in Files)
			{
				// Sub directories
				if (Directory.Exists(Element))
					copyDirectory(Element, Dst + Path.GetFileName(Element));
				// Files in directory
				else
					File.Copy(Element, Dst + Path.GetFileName(Element), true);
			}
		}

		/// <summary>
		/// Gets the short path.
		/// </summary>
		/// <param name="longPath">The long path.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-27</remarks>
		public static string GetShortPath(string longPath, int length)
		{
			if (longPath.Length <= length)
				return longPath;

			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(longPath.Split('\\'));

			bool bore = true;
			string output = longPath[0].ToString() + "...";
			string begin = string.Empty;
			string end = string.Empty;

			while (arrayList.Count != 0)
			{
				if (bore)
				{
					begin += arrayList[0].ToString() + "\\";
					arrayList.RemoveAt(0);
				}
				else
				{
					end = "\\" + arrayList[arrayList.Count - 1].ToString() + end;
					arrayList.RemoveAt(arrayList.Count - 1);
				}

				if (output.Length >= length)
					return output;
				else
					output = begin + "..." + end;

				bore = !bore;
			}

			return output;
		}
	}
}
