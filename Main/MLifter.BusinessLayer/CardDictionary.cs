using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;
using System.Drawing;
using System.Diagnostics;

namespace MLifter.BusinessLayer
{
    public class CardDictionary
    {
        private Dictionary dictionary;
        private ICards cards;

        public IList<ICard> Cards
        {
            get { return cards.Cards; }
        }
        /// <summary>
        /// Gets the active cards.
        /// </summary>
        /// <value>The active cards.</value>
        /// <remarks>Documented by Dev05, 2007-09-06</remarks>
        public int ActiveCardsCount
        {
            get
            {
                int i = 0;
                foreach (int chapterId in dictionary.QueryChapters)
                {
                    try
                    {
                        IChapter chapter = dictionary.Chapters.GetChapterByID(chapterId);
                        i += chapter.ActiveSize;
                    }
                    catch (MLifter.DAL.Security.PermissionException)
                    {
                        Trace.WriteLine("PermissionException in CardDictionary.ActiveCardsCount");
                    }
                }

                return i;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardDictionary"/> class.
        /// </summary>
        /// <param name="cards">The cards.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public CardDictionary(Dictionary dict, ICards Cards)
        {
            dictionary = dict;
            this.cards = Cards;
        }

        /// <summary>
        /// Adds the Media to the spezified card.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="type">The type.</param>
        /// <param name="side">The side.</param>
        /// <param name="active">if set to <c>true</c> the file is active.</param>
        /// <param name="standard">if set to <c>true</c> the file is the default file.</param>
        /// <param name="example">if set to <c>true</c> the file is the example.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void AddMedia(int cardID, string filename, EMedia type, Side side, bool active, bool standard, bool example)
        {
            ICard card = cards.Get(cardID);
            card.AddMedia(card.CreateMedia(type, filename, active, standard, example), side);
        }

        /// <summary>
        /// Gets the card by ID.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public Card GetCardByID(int cardID)
        {
            return new Card(cards.Get(cardID), dictionary);
        }
        /// <summary>
        /// Adds a card to the currently loaded dictionary.
        /// </summary>
        /// <param name="questionText">CurrentQuestion Text</param>
        /// <param name="answerText">CurrentAnswer Text</param>
        /// <param name="questionExample">CurrentQuestion example</param>
        /// <param name="answerExample">CurrentAnswer example</param>
        /// <param name="questionStyle">CurrentQuestion stylesheet</param>
        /// <param name="answerStyle">CurrentAnswer stylesheet</param>
        /// <param name="chapter">Chapter ID</param>
        /// <returns>Card ID if successfull, -1 if failure</returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public int AddCard(string questionText, string answerText, string questionExample,
            string answerExample, string questionStyle, string answerStyle, int chapter)
        {
            try
            {
                dictionary.Chapters.GetChapterByID(chapter);
            }
            catch (IdAccessException)
            {
                return -1;
            }

            ICard card = cards.AddNew();
            foreach (string word in answerText.Split(new char[] { ',', ';' }))
                card.Answer.AddWord(card.Answer.CreateWord(word, WordType.Word, true));
            foreach (string word in questionText.Split(new char[] { ',', ';' }))
                card.Question.AddWord(card.Question.CreateWord(word, WordType.Word, true));
            card.QuestionExample.AddWord(card.QuestionExample.CreateWord(questionExample.Trim('"'), WordType.Sentence, true));
            card.AnswerExample.AddWord(card.AnswerExample.CreateWord(answerExample.Trim('"'), WordType.Sentence, true));
            card.Settings.QuestionStylesheet = new CompiledTransform(questionStyle, null);
            card.Settings.AnswerStylesheet = new CompiledTransform(answerStyle, null);
            card.Chapter = chapter;

            return card.Id;
        }
        /// <summary>
        /// Removes the card with the given ID.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public void DeleteCardByID(int cardID)
        {
            cards.Delete(cardID);
        }
        /// <summary>
        /// Removes a list of cards from the dictionary.
        /// </summary>
        /// <param name="cardIds">The card ids.</param>
        /// <remarks>Documented by Dev03, 2008-01-04</remarks>
        public void DeleteCards(List<int> cardIds)
        {
            foreach (int cardID in cardIds)
                cards.Delete(cardID);
        }
        /// <summary>
        /// Gets the name of the chapter of te card with the given ID.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string GetChapterName(int cardID)
        {
            int chapterID = cards.Get(cardID).Chapter;
            return dictionary.Chapters.GetChapterByID(chapterID).Title;
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public ICard AddNew()
        {
            return dictionary.Cards.cards.AddNew();
        }

        /// <summary>
        /// Creates a new card.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-01-04</remarks>
        public ICard CreateNew()
        {
            return cards.Create();
        }

        /// <summary>
        /// Changes the card on difference.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="question">The question.</param>
        /// <param name="answer">The answer.</param>
        /// <param name="questionExample">The question example.</param>
        /// <param name="answerExample">The answer example.</param>
        /// <param name="chapter">The chapter.</param>
        /// <param name="ignoreText">The ingore Text.</param>
        /// <remarks>Documented by Dev05, 2007-09-17</remarks>
        public void ChangeCardOnDifference(int cardID, string[] question, string[] answer, string questionExample, string answerExample, int chapter, string ignoreText)
        {
            ICard card = cards.Get(cardID);
            if (card != null)
            {
                if (question != null && question.Length > 0 && question[0] != ignoreText)
                {
                    card.Question.ClearWords();
                    card.Question.AddWords(question);
                }
                if (answer != null && answer.Length > 0 && answer[0] != ignoreText)
                {
                    card.Answer.ClearWords();
                    card.Answer.AddWords(answer);
                }
                if (questionExample != ignoreText)
                {
                    card.QuestionExample.ClearWords();
                    card.QuestionExample.AddWord(card.QuestionExample.CreateWord(questionExample, WordType.Sentence, true));
                }
                if (answerExample != ignoreText)
                {
                    card.AnswerExample.ClearWords();
                    card.AnswerExample.AddWord(card.AnswerExample.CreateWord(answerExample, WordType.Sentence, true));
                }
                if (chapter != -1)
                    card.Chapter = chapter;
            }
        }

        /// <summary>
        /// Checks if Sentences is available.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-05-05</remarks>
        public bool SentenceAvailable(int cardID, Side side)
        {
            return SentenceAvailable(cards.Get(cardID), side);
        }
        /// <summary>
        /// Checks if Sentences is available.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-05-05</remarks>
        public bool SentenceAvailable(ICard card, Side side)
        {
            if (dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question)
            {
                if (side == Side.Answer)
                    side = Side.Question;
                else
                    side = Side.Answer;
            }

            if (side == Side.Question)
                return card.QuestionExample.Words.Count > 0 && (card.QuestionExample.Words as List<IWord>).Find(w => w.Word != string.Empty) != null;
            else
                return card.AnswerExample.Words.Count > 0 && (card.AnswerExample.Words as List<IWord>).Find(w => w.Word != string.Empty) != null;
        }

        /// <summary>
        /// Checks if an Audio is available.
        /// </summary>
        /// <param name="cardID">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public bool AudioAvailable(ICard card, Side side)
        {
            return AudioAvailable(card, side, true, false);
        }
        /// <summary>
        /// Checks if an Audio is available.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public bool AudioAvailable(int cardID, Side side)
        {
            return AudioAvailable(cardID, side, true, false);
        }
        /// <summary>
        /// Audioes the available.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-25</remarks>
        public bool AudioAvailable(int cardID, Side side, bool standard, bool example)
        {
            ICard card = cards.Get(cardID);
            return AudioAvailable(card, side, standard, example);
        }
        /// <summary>
        /// Audioes the available.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="standard">if set to <c>true</c> [standard].</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public bool AudioAvailable(ICard card, Side side, bool standard, bool example)
        {
            if (dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question)
            {
                if (side == Side.Answer)
                    side = Side.Question;
                else
                    side = Side.Answer;
            }

            if (side == Side.Question)
            {
                foreach (IMedia media in card.QuestionMedia)
                {
                    if (media.MediaType == EMedia.Audio)
                        if (((IAudio)media).Example == example && (standard ? ((IAudio)media).Default == standard : true))
                            return true;
                }
            }
            else
            {
                foreach (IMedia media in card.AnswerMedia)
                {
                    if (media.MediaType == EMedia.Audio)
                        if (((IAudio)media).Example == example && (standard ? ((IAudio)media).Default == standard : true))
                            return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if an Image is available.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public bool ImageAvailable(int cardID, Side side)
        {
            ICard card = cards.Get(cardID);
            return ImageAvailable(card, side);
        }
        /// <summary>
        /// Images the available.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public bool ImageAvailable(ICard card, Side side)
        {
            if (dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question)
            {
                if (side == Side.Answer)
                    side = Side.Question;
                else
                    side = Side.Answer;
            }

            if (side == Side.Question)
            {
                foreach (IMedia media in card.QuestionMedia)
                {
                    if (media.MediaType == EMedia.Image)
                        return true;
                }
            }
            else
            {
                foreach (IMedia media in card.AnswerMedia)
                {
                    if (media.MediaType == EMedia.Image)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the audio file.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public string GetAudioFile(int cardID, Side side, bool example) { return GetAudioFile(cardID, side, false, example); }
        /// <summary>
        /// Gets the audio file.
        /// </summary>
        /// <param name="cardID">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="standard">if set to <c>true</c> the returned file must be a default file.</param>
        /// <param name="example">if set to <c>true</c> the returned file is an example file.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public string GetAudioFile(ICard card, Side side, bool standard, bool example) { return GetAudioFile(card, side, standard, example, false); }
        /// <summary>
        /// Gets the audio file.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <param name="standard">if set to <c>true</c> the returned file must be a default file.</param>
        /// <param name="example">if set to <c>true</c> the returned file is an example file.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string GetAudioFile(int cardID, Side side, bool standard, bool example) { return GetAudioFile(cardID, side, standard, example, false); }
        /// <summary>
        /// Gets the audio file.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <param name="standard">if set to <c>true</c> [standard].</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public string GetAudioFile(int cardID, Side side, bool standard, bool example, bool donotChange)
        {
            ICard card = cards.Get(cardID);
            return GetAudioFile(card, side, standard, example, donotChange);
        }

        /// <summary>
        /// Gets the audio file.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="standard">if set to <c>true</c> [standard].</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public string GetAudioFile(ICard card, Side side, bool standard, bool example, bool donotChange)
        {
            IMedia media = GetAudioObject(card, side, standard, example, donotChange);
            if (media == null)
                return String.Empty;
            else
                return media.Filename;
        }
        /// <summary>
        /// Gets the audio file.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="standard">if set to <c>true</c> [standard].</param>
        /// <param name="example">if set to <c>true</c> [example].</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public IMedia GetAudioObject(ICard card, Side side, bool standard, bool example, bool donotChange)
        {
            if (dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question && !donotChange)
            {
                if (side == Side.Question)
                    side = Side.Answer;
                else
                    side = Side.Question;
            }

            switch (side)
            {
                case Side.Question:
                    foreach (IMedia media in card.QuestionMedia)
                    {
                        if (media.MediaType == EMedia.Audio)
                        {
                            if (((IAudio)media).Example == example && (standard ? ((IAudio)media).Default == standard : true))
                            {
                                return media;
                            }
                        }
                    }
                    break;
                case Side.Answer:
                    foreach (IMedia media in card.AnswerMedia)
                    {
                        if (media.MediaType == EMedia.Audio)
                        {
                            if (((IAudio)media).Example == example && (standard ? ((IAudio)media).Default == standard : true))
                            {
                                return media;
                            }
                        }
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public string GetImage(int cardID, Side side) { return GetImage(cardID, side, false); }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="cardID">The card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public string GetImage(ICard card, Side side) { return GetImage(card, side, false); }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public string GetImage(int cardID, Side side, bool donotChange)
        {
            ICard card = cards.Get(cardID);
            return GetImage(card, side, donotChange);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public string GetImage(ICard card, Side side, bool donotChange)
        {
            IMedia media = GetMedia(card, side, EMedia.Image, donotChange);
            if (media == null)
                return string.Empty;

            return media.Filename;
        }
        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-08</remarks>
        public Size GetImageSize(ICard card, Side side)
        {
            return GetImageSize(card, side, false);
        }
        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-08</remarks>
        public Size GetImageSize(ICard card, Side side, bool donotChange)
        {
            IImage media = GetMedia(card, side, EMedia.Image, donotChange) as IImage;
            if (media != null)
                return new Size(media.Width, media.Height);

            return Size.Empty;
        }

        /// <summary>
        /// Gets the image stream.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public Stream GetImageStream(int cardID, Side side) { return GetImageStream(cardID, side, false); }
        /// <summary>
        /// Gets the image stream.
        /// </summary>
        /// <param name="cardID">The card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public Stream GetImageStream(ICard card, Side side) { return GetImageStream(card, side, false); }
        /// <summary>
        /// Gets the image stream.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public Stream GetImageStream(int cardID, Side side, bool donotChange)
        {
            ICard card = cards.Get(cardID);
            return GetImageStream(card, side, donotChange);
        }

        /// <summary>
        /// Gets the image stream.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public Stream GetImageStream(ICard card, Side side, bool donotChange)
        {
            IMedia media = GetMedia(card, side, EMedia.Image, donotChange);
            if (media == null)
                return null;

            return media.Stream;
        }

        /// <summary>
        /// Gets the image object.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-11</remarks>
        public IMedia GetImageObject(int cardId, Side side, bool donotChange)
        {
            ICard card = cards.Get(cardId);
            return GetImageObject(card, side, donotChange);
        }

        /// <summary>
        /// Gets the image object.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-11</remarks>
        public IMedia GetImageObject(ICard card, Side side, bool donotChange)
        {
            return GetMedia(card, side, EMedia.Image, donotChange);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public string GetVideo(int cardID, Side side) { return GetVideo(cardID, side, false); }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="cardID">The card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-10-11</remarks>
        public string GetVideo(ICard card, Side side) { return GetVideo(card, side, false); }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-05</remarks>
        public string GetVideo(int cardID, Side side, bool donotChange)
        {
            ICard card = cards.Get(cardID);
            return GetVideo(card, side, donotChange);
        }

        /// <summary>
        /// Gets the video.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public string GetVideo(ICard card, Side side, bool donotChange)
        {
            IMedia media = GetVideoObject(card, side, donotChange);
            if (media == null)
                return String.Empty;
            else
                return media.Filename;
        }

        /// <summary>
        /// Gets the video.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="donotChange">if set to <c>true</c> [donot change].</param>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public IMedia GetVideoObject(ICard card, Side side, bool donotChange)
        {
            return GetMedia(card, side, EMedia.Video, donotChange);
        }

        /// <summary>
        /// Gets the Media.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <param name="donotChange">if set to <c>true</c> do not change the direction.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-08-06</remarks>
        private IMedia GetMedia(ICard card, Side side, EMedia type, bool donotChange)
        {
            if (dictionary.CurrentQueryDirection == EQueryDirection.Answer2Question && !donotChange)
            {
                if (side == Side.Question)
                    side = Side.Answer;
                else
                    side = Side.Question;
            }

            switch (side)
            {
                case Side.Question:
                    foreach (IMedia media in card.QuestionMedia)
                    {
                        if (media.MediaType == type)
                            return media;
                    }
                    break;
                case Side.Answer:
                    foreach (IMedia media in card.AnswerMedia)
                    {
                        if (media.MediaType == type)
                            return media;
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Gets the Media files from the speciefied card.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string[] GetMediaFiles(int cardID, Side side) { return GetMediaFiles(cardID, side, false); }
        /// <summary>
        /// Gets the Media files from the speciefied card.
        /// </summary>
        /// <param name="cardID">The card ID.</param>
        /// <param name="side">The side.</param>
        /// <param name="unused">if set to <c>true</c> only the unused files are returned.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public string[] GetMediaFiles(int cardID, Side side, bool unused)
        {
            ICard card = cards.Get(cardID);
            return GetMediaFiles(card, side, unused);
        }

        /// <summary>
        /// Gets the Media files.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <param name="unused">if set to <c>true</c> [unused].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-16</remarks>
        public string[] GetMediaFiles(ICard card, Side side, bool unused)
        {
            List<string> files = new List<string>();

            switch (side)
            {
                case Side.Question:
                    foreach (IMedia media in card.QuestionMedia)
                        if ((unused ? !media.Active.GetValueOrDefault() : true))
                            files.Add(media.Filename);
                    break;
                case Side.Answer:
                    foreach (IMedia media in card.AnswerMedia)
                        if ((unused ? !media.Active.GetValueOrDefault() : true))
                            files.Add(media.Filename);
                    break;
            }

            return files.ToArray();
        }
    }
}
