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
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Data.OleDb;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.CSV;

namespace MLifter.DAL.ImportExport
{
	/// <summary>
	/// This class can import CSV-Files into a dictionary.
	/// </summary>
	public static class Importer
	{
		/// <summary>
		/// Reads the CSV.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="fileSchema">The file schema.</param>
		/// <returns>The two dimensional string list parsed from the csv file.</returns>
		/// <remarks>Documented by Dev05, 2007-08-31</remarks>
		public static List<List<string>> ReadCSV(string fileName, FileSchema fileSchema)
		{
			return ReadCSV(fileName, fileSchema, int.MaxValue);
		}

		/// <summary>
		/// Reads the CSV.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="fileSchema">The file schema.</param>
		/// <param name="count">The maximal count of readed rows.</param>
		/// <returns>The two dimensional string list parsed from the csv file.</returns>
		/// <remarks>Documented by Dev05, 2007-08-31</remarks>
		public static List<List<string>> ReadCSV(string fileName, FileSchema fileSchema, int count)
		{
			if (fileSchema == null)
				throw new ArgumentNullException();

			List<List<string>> data = new List<List<string>>();

			using (StreamReader stream = new StreamReader(fileName, fileSchema.Encoding))
			{
				//swallow the first line (headers) - CsvReader has a problem with multiple headers with the same name
				if (fileSchema.hasHeaders && !stream.EndOfStream)
					stream.ReadLine();

				//initialize the csvreader class and read out the values
				CsvReader csv = new CsvReader(stream, false, fileSchema.Delimiter, fileSchema.Quote, fileSchema.Escape, fileSchema.Comment, fileSchema.trimSpaces);
				csv.DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine;
				csv.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;
				csv.SupportsMultiline = false;

				int fieldCount = csv.FieldCount;
				try
				{
					while (csv.ReadNextRecord() && csv.CurrentRecordIndex < count)
					{

						List<string> row = new List<string>();
						for (int i = 0; i < fieldCount; i++)
							//[ML-968] Remove control characters and add field to row
							row.Add(System.Text.RegularExpressions.Regex.Replace(csv[i], @"[\p{C}]", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase));

						data.Add(row);

					}
				}
				catch { throw; }
				finally
				{
					if (stream != null)
						stream.Close();
				}
			}

			return data;
		}

		/// <summary>
		/// Imports to dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="data">The data.</param>
		/// <param name="chapter">The chapter to import to.</param>
		/// <param name="fields">The fields on which id the field is in the data.</param>
		/// <param name="worker">The worker to report the status; could be null.</param>
		/// <param name="cardTimeStamp">The card time stamp.</param>
		/// <param name="splitSynonyms">if set to <c>true</c> [split synonyms].</param>
		/// <remarks>Documented by Dev05, 2007-08-31</remarks>
		/// <remarks>Documented by Dev08, 2008-09-25</remarks>
		public static void ImportToDictionary(IDictionary dictionary, List<List<string>> data, IChapter chapter, Dictionary<Field, int> fields, BackgroundWorker worker, DateTime cardTimeStamp, bool splitSynonyms)
		{
			int value = 0;
			int pos = 0;
			string workingdirectory = Environment.CurrentDirectory;
			string[] synonymsSplitChar = splitSynonyms ? new string[] { " ,", " ;", ", ", "; ", ",", ";" } : new string[] { "\n" }; // following comment ist antiqued: "empty splitchar array would cause to split at space char -> split at newline char, which should not exist anymore in csv imported data"

			foreach (List<string> item in data)
			{
				ICard card = dictionary.Cards.AddNew();
				card.Timestamp = cardTimeStamp;
				card.Chapter = chapter.Id;

				IMedia questionImage = null, questionSound = null, questionVideo = null, questionExampleSound = null;
				IMedia answerImage = null, answerSound = null, answerVideo = null, answerExampleSound = null;

				if (fields.ContainsKey(Field.Question) && fields[Field.Question] < item.Count)
				{
					if (!string.IsNullOrEmpty(item[fields[Field.Question]]))
					{
						foreach (string word in item[fields[Field.Question]].Split(synonymsSplitChar, StringSplitOptions.RemoveEmptyEntries))
						{
							card.Question.AddWord(card.Question.CreateWord(word.Trim(), WordType.Word, true));
						}
					}
				}
				if (fields.ContainsKey(Field.QuestionDistractors) && fields[Field.QuestionDistractors] < item.Count)
				{
					if (!string.IsNullOrEmpty(item[fields[Field.QuestionDistractors]]))
					{
						foreach (string word in item[fields[Field.QuestionDistractors]].Split(synonymsSplitChar, StringSplitOptions.RemoveEmptyEntries))
						{
							card.QuestionDistractors.AddWord(card.QuestionDistractors.CreateWord(word.Trim(), WordType.Distractor, false));
						}
					}
				}
				if (fields.ContainsKey(Field.QuestionImage) && fields[Field.QuestionImage] < item.Count)
				{
					try
					{
						questionImage = card.CreateMedia(EMedia.Image, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.QuestionImage]]), workingdirectory), true, true, false);
					}
					catch (FileNotFoundException)
					{
						Debug.WriteLine("A media file was not found.");
					}
					if (questionImage != null)
						card.AddMedia(questionImage, Side.Question);
				}
				if (fields.ContainsKey(Field.QuestionSound) && fields[Field.QuestionSound] < item.Count)
				{
					try
					{
						questionSound = card.CreateMedia(EMedia.Audio, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.QuestionSound]]), workingdirectory), true, true, false);
					}
					catch (FileNotFoundException)
					{
						Debug.WriteLine("A media file was not found.");
					}
					if (questionSound != null)
						card.AddMedia(questionSound, Side.Question);
				}
				if (fields.ContainsKey(Field.QuestionVideo) && fields[Field.QuestionVideo] < item.Count)
				{
					try
					{
						questionVideo = card.CreateMedia(EMedia.Video, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.QuestionVideo]]), workingdirectory), true, true, false);
					}
					catch (FileNotFoundException)
					{
						Debug.WriteLine("A media file was not found.");
					}
					if (questionVideo != null)
						card.AddMedia(questionVideo, Side.Question);
				}
				if (fields.ContainsKey(Field.QuestionExample) && fields[Field.QuestionExample] < item.Count)
					card.QuestionExample.AddWord(card.QuestionExample.CreateWord(item[fields[Field.QuestionExample]].Trim(), WordType.Sentence, false));
				if (fields.ContainsKey(Field.QuestionExampleSound) && fields[Field.QuestionExampleSound] < item.Count)
				{
					try
					{
						questionExampleSound = card.CreateMedia(EMedia.Audio, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.QuestionExampleSound]]), workingdirectory), true, false, true);
					}
					catch (FileNotFoundException)
					{
						Debug.WriteLine("A media file was not found.");
					}
					if (questionExampleSound != null)
						card.AddMedia(questionExampleSound, Side.Question);
				}

				if (fields.ContainsKey(Field.Answer) && fields[Field.Answer] < item.Count)
				{
					if (!string.IsNullOrEmpty(item[fields[Field.Answer]]))
					{
						foreach (string word in item[fields[Field.Answer]].Split(synonymsSplitChar, StringSplitOptions.RemoveEmptyEntries))
						{
							card.Answer.AddWord(card.Answer.CreateWord(word.Trim(), WordType.Word, true));
						}
					}
				}
				if (fields.ContainsKey(Field.AnswerDistractors) && fields[Field.AnswerDistractors] < item.Count)
				{
					if (!string.IsNullOrEmpty(item[fields[Field.AnswerDistractors]]))
					{
						foreach (string word in item[fields[Field.AnswerDistractors]].Split(synonymsSplitChar, StringSplitOptions.RemoveEmptyEntries))
						{
							card.AnswerDistractors.AddWord(card.AnswerDistractors.CreateWord(word.Trim(), WordType.Distractor, false));
						}
					}
				}
				if (fields.ContainsKey(Field.AnswerImage) && fields[Field.AnswerImage] < item.Count)
				{
					if (questionImage != null && item[fields[Field.QuestionImage]] == item[fields[Field.AnswerImage]])
						answerImage = questionImage;
					else
					{
						try
						{
							answerImage = card.CreateMedia(EMedia.Image, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.AnswerImage]]), workingdirectory), true, true, false);
						}
						catch (FileNotFoundException)
						{
							Debug.WriteLine("A media file was not found.");
						}
					}
					if (answerImage != null)
						card.AddMedia(answerImage, Side.Answer);
				}
				if (fields.ContainsKey(Field.AnswerSound) && fields[Field.AnswerSound] < item.Count)
				{
					if (questionSound != null && item[fields[Field.QuestionSound]] == item[fields[Field.AnswerSound]])
						answerSound = questionSound;
					else
					{
						try
						{
							answerSound = card.CreateMedia(EMedia.Audio, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.AnswerSound]]), workingdirectory), true, true, false);
						}
						catch (FileNotFoundException)
						{
							Debug.WriteLine("A media file was not found.");
						}
					}
					if (answerSound != null)
						card.AddMedia(answerSound, Side.Answer);
				}
				if (fields.ContainsKey(Field.AnswerVideo) && fields[Field.AnswerVideo] < item.Count)
				{
					if (questionVideo != null && item[fields[Field.QuestionVideo]] == item[fields[Field.AnswerVideo]])
						answerVideo = questionVideo;
					else
					{
						try
						{
							answerVideo = card.CreateMedia(EMedia.Video, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.AnswerVideo]]), workingdirectory), true, true, false);
						}
						catch (FileNotFoundException)
						{
							Debug.WriteLine("A media file was not found.");
						}
					}
					if (answerVideo != null)
						card.AddMedia(answerVideo, Side.Answer);
				}
				if (fields.ContainsKey(Field.AnswerExample) && fields[Field.AnswerExample] < item.Count)
					card.AnswerExample.AddWord(card.AnswerExample.CreateWord(item[fields[Field.AnswerExample]].Trim(), WordType.Sentence, true));
				if (fields.ContainsKey(Field.AnswerExampleSound) && fields[Field.AnswerExampleSound] < item.Count)
				{
					if (questionExampleSound != null && item[fields[Field.QuestionExampleSound]] == item[fields[Field.AnswerExampleSound]])
						answerExampleSound = questionExampleSound;
					else
					{
						try
						{
							answerExampleSound = card.CreateMedia(EMedia.Audio, ExpandMediaPath(StripInvalidFileNameChars(item[fields[Field.AnswerExampleSound]]), workingdirectory), true, false, true);
						}
						catch (FileNotFoundException)
						{
							Debug.WriteLine("A media file was not found.");
						}
					}
					if (answerExampleSound != null)
						card.AddMedia(answerExampleSound, Side.Answer);
				}
				if (fields.ContainsKey(Field.Chapter) && fields[Field.Chapter] < item.Count && !string.IsNullOrEmpty(item[fields[Field.Chapter]]))
				{
					string cardchapterstring = item[fields[Field.Chapter]].Trim();
					if (cardchapterstring != string.Empty)
					{
						IChapter cardchapter = dictionary.Chapters.Find(cardchapterstring);
						if (cardchapter == null)
						{
							IChapter newchapter = dictionary.Chapters.AddNew();
							newchapter.Title = cardchapterstring;
							card.Chapter = newchapter.Id;
						}
						else
							card.Chapter = cardchapter.Id;
					}
				}

				pos++;
				if (worker != null)
				{
					if (value < (int)((double)pos / (double)data.Count * 100))
					{
						value = (int)((double)pos / (double)data.Count * 100);
						worker.ReportProgress(value);
					}
				}
			}

			dictionary.Save();
		}

		/// <summary>
		/// Strips the invalid file path characters.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-02-18</remarks>
		private static string StripInvalidFileNameChars(string path)
		{
			foreach (char invalidchar in System.IO.Path.GetInvalidPathChars())
				path = path.Replace(invalidchar.ToString(), string.Empty);
			path = path.Trim();
			return path;
		}

		/// <summary>
		/// Expands the media path to the absolute file path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="workingdirectory">The workingdirectory.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by DAC, 2008-02-22.
		/// </remarks>
		private static string ExpandMediaPath(string path, string workingdirectory)
		{
			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(workingdirectory, path);
			}
			return path;
		}

	}
}
