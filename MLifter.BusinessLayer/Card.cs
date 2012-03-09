using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using System.Collections;
using System.Text.RegularExpressions;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// This class represents a card froma dictionary.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-10-03</remarks>
    public partial class Card
    {
        private Dictionary dictionary;
        private ICard card;
        public ICard BaseCard
        {
            get { return card; }
            set
            {
                DetachEvents();
                card = value;
                AttachEvents();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public Card(ICard card, Dictionary dictionary)
        {
            this.card = card;
            this.dictionary = dictionary;

			ignoreChars = Regex.Escape(dictionary.Settings.StripChars).Replace("-", "\\-");
			validChar = new Regex("[" + ignoreChars + "\\b" + "]");

            AttachEvents();
        }

        /// <summary>
        /// Occurs when [create Media progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-22</remarks>
        public event StatusMessageEventHandler CreateMediaProgressChanged;

        private void AttachEvents()
        {
            if (card != null)
            {
                card.CreateMediaProgressChanged += new StatusMessageEventHandler(card_CreateMediaProgressChanged);
            }
        }

        void card_CreateMediaProgressChanged(object sender, StatusMessageEventArgs args)
        {
            if (CreateMediaProgressChanged != null) CreateMediaProgressChanged(this, args);
        }

        private void DetachEvents()
        {
            if (card != null)
            {
                card.CreateMediaProgressChanged -= new StatusMessageEventHandler(card_CreateMediaProgressChanged);
            }
        }

        /// <summary>
        /// Gets the question caption.
        /// </summary>
        /// <value>The question caption.</value>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public string CurrentQuestionCaption
        {
            get
            {
                return (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? dictionary.QuestionCaption : dictionary.AnswerCaption);
            }
        }
        /// <summary>
        /// Gets the answer caption.
        /// </summary>
        /// <value>The answer caption.</value>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public string CurrentAnswerCaption
        {
            get
            {
                return (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? dictionary.AnswerCaption : dictionary.QuestionCaption);
            }
        }

        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>The question.</value>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public IWords CurrentQuestion
        {
            get
            {
                return (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? card.Question : card.Answer);
            }
        }
        /// <summary>
        /// Gets the answer.
        /// </summary>
        /// <value>The answer.</value>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public IWords CurrentAnswer
        {
            get
            {
                return (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? card.Answer : card.Question);
            }
        }
        /// <summary>
        /// Gets the queston example.
        /// </summary>
        /// <value>The queston example.</value>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public IWords CurrentQuestionExample
        {
            get
            {
                return (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? card.QuestionExample : card.AnswerExample);
            }
        }
        /// <summary>
        /// Gets the answer example.
        /// </summary>
        /// <value>The answer example.</value>
        /// <remarks>Documented by Dev05, 2007-10-03</remarks>
        public IWords CurrentAnswerExample
        {
            get
            {
                return (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer ? card.AnswerExample : card.QuestionExample);
            }
        }

        # region XSLT-Methodes
        public bool ContainsImage(string side)
        {
            return dictionary.Cards.ImageAvailable(card, side.ToLower() == "question" ? Side.Question : Side.Answer);
        }
        public bool ContainsAudio(string side)
        {
            return dictionary.Cards.AudioAvailable(card, side.ToLower() == "question" ? Side.Question : Side.Answer);
        }
        public bool ContainsExampleAudio(string side)
        {
            return dictionary.Cards.AudioAvailable(card, side.ToLower() == "question" ? Side.Question : Side.Answer, false, true);
        }
        public bool ContainsVideo(string side)
        {
            return dictionary.Cards.GetVideo(card, side.ToLower() == "question" ? Side.Question : Side.Answer) != string.Empty;
        }

        public string GetText(string side)
        {
            switch (side)
            {
                case "question":
                    return ConvertNewlines(CurrentQuestion.ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
                case "answer":
                    return ConvertNewlines(CurrentAnswer.ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
            }

            return string.Empty;
        }

        public string GetExample(string side)
        {
            switch (side)
            {
                case "question":
                    return ConvertNewlines(CurrentQuestionExample.ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
                case "answer":
                    return ConvertNewlines(CurrentAnswerExample.ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
            }

            return string.Empty;
        }

        public string GetTextDirection(string side)
        {
            if (side == "question" ^ dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question)
                return BaseCard.Question.Culture.TextInfo.IsRightToLeft ? "RTL" : "LTR";
            else
                return BaseCard.Answer.Culture.TextInfo.IsRightToLeft ? "RTL" : "LTR";
        }

		private string ignoreChars;
		private Regex validChar;

		/// <summary>
		/// Parses the input answers.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		private List<String> ParseInputAnswers(string input, bool stripIgnoreChars)
		{
			List<string> inputAnswers = new List<string>();

			input = ConvertNewlines(input);		// converts multiple new line characters to <br>
			inputAnswers.AddRange(input.Split(new String[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries));
			for (int i = 0; i < inputAnswers.Count; i++)
				inputAnswers[i] = inputAnswers[i].Trim();

			// strip out ignore characters
			if (stripIgnoreChars)
				inputAnswers.ForEach(a => inputAnswers[inputAnswers.IndexOf(a)] = validChar.Replace(a, String.Empty));

			return inputAnswers;
		}

		/// <summary>
		/// Parses the correct answers.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev09, 2009-04-28</remarks>
		private List<String> ParseCorrectAnswers(bool stripIgnoreChars)
		{
			List<string> correctAnswers = new List<string>();

			// pull each word (answer) from the full answer
			// remove ignored characters
			IList<IWord> answers = CurrentAnswer.Words;
			foreach (IWord word in answers)
			{
				if (stripIgnoreChars)
					correctAnswers.Add(validChar.Replace(word.Word, String.Empty).Trim().Replace("&lt;", "<").Replace("&gt;", ">"));
				else
					correctAnswers.Add(word.Word.Trim().Replace("&lt;", "<").Replace("&gt;", ">"));
			}

			return correctAnswers;
		}

        /// <summary>
        /// Checks the synonyms.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev09, 2009-04-21</remarks>
        public string CheckSynonyms(string input)
        {
			List<string> correctAnswers = ParseCorrectAnswers(true);
			List<string> inputAnswers = ParseInputAnswers(input, true);
			if (input.Trim() == "" || (inputAnswers.Count == 1 && correctAnswers.Count == 1)) return "true";
			if (inputAnswers.Count > correctAnswers.Count) return "false";

            foreach (string correctAnswer in correctAnswers)
            {
                if (!inputAnswers.Contains(correctAnswer)) return "false";
            }
            return "true";
        }

        /// <summary>
        /// Colors the synonyms.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev09, 2009-04-21</remarks>
        public string ColorSynonyms(string input, string side)
        {
			if (dictionary.LearnMode == LearnModes.Sentence)
			{
				if (side == "answer")
				{
					if (dictionary.CurrentQueryDirection == EQueryDirection.Question2Answer)
						return card.AnswerExample.ToString();
					else
						return card.QuestionExample.ToString();
				}
				else
					return input;
			}

			List<string> correctAnswers = ParseCorrectAnswers(true);
			List<string> originalAnswers = ParseCorrectAnswers(false);
			List<string> inputAnswers = ParseInputAnswers(input, true);
			List<string> originalInputAnswers = ParseInputAnswers(input, false);
			if (correctAnswers.Count == 1 && inputAnswers.Count == 1)
			{
				if (side == "answer")
					return originalAnswers[0];
				else
					return input;
			}

			List<string> lcInputAnswers = new List<string>();
			List<string> lcCorrectAnswers = new List<string>();
			bool caseSensitive = (bool)dictionary.Settings.CaseSensitive;
			if (!caseSensitive)
			{
				inputAnswers.ForEach(a => lcInputAnswers.Add(a.ToLower()));
				correctAnswers.ForEach(a => lcCorrectAnswers.Add(a.ToLower()));
			}

            string htmlString = null;
            if (side == "question")
            {
                // compare the answers and assign true / false
                for (int i = 0; i < inputAnswers.Count; i++)
                {
                    if (correctAnswers.Contains(inputAnswers[i]) || (!caseSensitive && lcCorrectAnswers.Contains(lcInputAnswers[i])))
						htmlString += "<span class=\"correctInput\">" + originalInputAnswers[i] + "</span>\n";
                    else
						htmlString += "<span class=\"wrongInput\">" + originalInputAnswers[i] + "</span>\n";
                }
            }
            else if (side == "answer")
            {
                // compare the answers and assign true / false
                for (int i = 0; i < correctAnswers.Count; i++)
                {
                    if (inputAnswers.Contains(correctAnswers[i]) || (!caseSensitive && lcInputAnswers.Contains(lcCorrectAnswers[i])))
						htmlString += "<span class=\"correctAnswer\">" + originalAnswers[i] + "</span>\n";
                    else
						htmlString += "<span class=\"wrongAnswer\">" + originalAnswers[i] + "</span>\n";
                }
            }
            return ConvertNewlines((htmlString == null) ? String.Empty : htmlString.Trim());
        }

        public static string ConvertNewlines(string text)
        {
            return text.Replace("\r\n", "<br />").Replace("\n\r", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
        }

        public string GetImage(string side)
        {
            return dictionary.Cards.GetImage(card, side == "question" ? Side.Question : Side.Answer);
        }
        public int GetImageHeight(string side)
        {
            return dictionary.Cards.GetImageSize(card, side == "question" ? Side.Question : Side.Answer).Height;
        }
        public int GetImageWidth(string side)
        {
            return dictionary.Cards.GetImageSize(card, side == "question" ? Side.Question : Side.Answer).Width;
        }
        public string GetAudio(string side)
        {
            return dictionary.Cards.GetAudioFile(card, side == "question" ? Side.Question : Side.Answer, true, false);
        }
        public string GetExampleAudio(string side)
        {
            return dictionary.Cards.GetAudioFile(card, side == "question" ? Side.Question : Side.Answer, false, true);
        }
        public string GetVideo(string side)
        {
            return dictionary.Cards.GetVideo(card, side == "question" ? Side.Question : Side.Answer);
        }

        public string GetStyle()
        {
            if (!dictionary.UseDictionaryStyleSheets)
                return string.Empty;

            Uri dicUri;

            if (dictionary.IsDB)      //DB
            {
                //Just a default value; this has no effect to the program
                dicUri = new Uri(Environment.CurrentDirectory);
            }
            else    //XML
            {
                string dicPath = System.IO.Path.GetDirectoryName(dictionary.DictionaryPath);
                dicUri = new Uri(dicPath.Replace(@"\", @"/"));
            }

            string css = "";
            if (dictionary.CardStyle != null)
                css += dictionary.CardStyle.ToString(dicUri.AbsoluteUri);
            if ((dictionary.Chapters.GetChapterByID(card.Chapter).Settings != null) &&
                (dictionary.Chapters.GetChapterByID(card.Chapter).Settings.Style != null))
                css += dictionary.Chapters.GetChapterByID(card.Chapter).Settings.Style.ToString(dicUri.AbsoluteUri);
            if ((card.Settings != null) && (card.Settings.Style != null))
                css += card.Settings.Style.ToString(dicUri.AbsoluteUri);

            return css;
        }
        # endregion
    }
}