using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.CSV;

namespace MLifter.DAL.ImportExport
{
    /// <summary>
    /// Defines a import file schema for a CSV file.
    /// </summary>
    /// <remarks>Documented by Dev02, 2007-12-14</remarks>
    public class FileSchema
    {
        /// <summary>
        /// The text encoding to use.
        /// </summary>
        public Encoding Encoding = Encoding.Default;
        /// <summary>
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.
        /// </summary>
        public bool hasHeaders = false;
        /// <summary>
        /// The delimiter character separating each field (default is ',').
        /// </summary>
        public char Delimiter = CsvReader.DefaultDelimiter;
        /// <summary>
        /// The quotation character wrapping every field (default is '"').
        /// </summary>
        public char Quote = CsvReader.DefaultQuote;
        /// <summary>
        /// The escape character letting insert quotation characters inside a quoted field (default is '\0').
        /// If no escape character, set to '\0' to gain some performance.
        /// </summary>
        public char Escape = CsvReader.DefaultEscape;
        /// <summary>
        /// The comment character indicating that a line is commented out (default is '#').
        /// </summary>
        public char Comment = CsvReader.DefaultComment;
        /// <summary>
        /// <see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>.
        /// </summary>
        public bool trimSpaces = false;
    }

    /// <summary>
    /// Defines a field to import.
    /// </summary>
    public enum Field
    {
		/// <summary>
		/// 
		/// </summary>
        Question,
		/// <summary>
		/// 
		/// </summary>
        QuestionDistractors,
		/// <summary>
		/// 
		/// </summary>
        QuestionImage,
		/// <summary>
		/// 
		/// </summary>
        QuestionSound,
		/// <summary>
		/// 
		/// </summary>
        QuestionVideo,
		/// <summary>
		/// 
		/// </summary>
        QuestionExample,
		/// <summary>
		/// 
		/// </summary>
        QuestionExampleSound,
		/// <summary>
		/// 
		/// </summary>
        Answer,
		/// <summary>
		/// 
		/// </summary>
        AnswerDistractors,
		/// <summary>
		/// 
		/// </summary>
        AnswerImage,
		/// <summary>
		/// 
		/// </summary>
        AnswerSound,
		/// <summary>
		/// 
		/// </summary>
        AnswerVideo,
		/// <summary>
		/// 
		/// </summary>
        AnswerExample,
		/// <summary>
		/// 
		/// </summary>
        AnswerExampleSound,
		/// <summary>
		/// 
		/// </summary>
        Chapter
    }
}
