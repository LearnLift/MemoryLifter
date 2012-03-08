using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

using MLifter;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using System.Diagnostics;
using MLifter.AudioTools.Codecs;
using MLifter.Generics;
using MLifter.Controls;

namespace MLifter.AudioTools
{
    /// <summary>
    /// This class handles the communication between the MLifter-DAL and the MLifer-Recorder.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-08-07</remarks>
    public class DictionaryManagement
    {
        # region Variables and properties
        private IDictionary dictionary;
        private IList<ICard> cards = null;
        private string filename;
        private BackgroundEncoder backgroundEncoder = new BackgroundEncoder();

        /// <summary>
        /// Gets the name of the dictionary.
        /// </summary>
        /// <value>The name of the dictionary.</value>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string DictionaryName
        {
            get { return Path.GetFileNameWithoutExtension(filename); }
        }
        /// <summary>
        /// Gets the dictionary media folder.
        /// </summary>
        /// <value>The dictionary media folder.</value>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string DictionaryMediaFolder
        {
            get { return Path.Combine(Path.GetDirectoryName(filename), dictionary.MediaDirectory) + @"\"; }
        }
        /// <summary>
        /// Gets the card count.
        /// </summary>
        /// <value>The card count.</value>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public int CardCount
        {
            get { return cards.Count; }
        }

        /// <summary>
        /// Gets the question caption.
        /// </summary>
        /// <value>The question caption.</value>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string QuestionCaption
        {
            get { return dictionary.DefaultSettings.QuestionCaption; }
        }
        /// <summary>
        /// Gets the answer caption.
        /// </summary>
        /// <value>The answer caption.</value>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string AnswerCaption
        {
            get { return dictionary.DefaultSettings.AnswerCaption; }
        }

        /// <summary>
        /// Gets the question culture.
        /// </summary>
        /// <value>The question culture.</value>
        /// <remarks>Documented by Dev02, 2007-12-20</remarks>
        public CultureInfo QuestionCulture
        {
            get { return dictionary.DefaultSettings.QuestionCulture; }
            set { if (value != null) dictionary.DefaultSettings.QuestionCulture = value; SaveDictionary(); }
        }

        /// <summary>
        /// Gets the answer culture.
        /// </summary>
        /// <value>The answer culture.</value>
        /// <remarks>Documented by Dev02, 2007-12-20</remarks>
        public CultureInfo AnswerCulture
        {
            get { return dictionary.DefaultSettings.AnswerCulture; }
            set { if (value != null) dictionary.DefaultSettings.AnswerCulture = value; SaveDictionary(); }
        }

        # endregion
        # region Constructor and basic methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryManagement"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public DictionaryManagement()
        { }

        /// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public void LoadDictionary(string filename)
        {
            this.filename = filename;
            if (dictionary != null)
                dictionary.Dispose();
            try
            {
                ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.MsSqlCe, filename);
                MLifter.DAL.Interfaces.IUser user = UserFactory.Create((GetLoginInformation)MLifter.Controls.LoginForm.OpenLoginForm,
                    css, (DataAccessErrorDelegate)delegate { return; }, this);
                css.LmId = User.GetIdOfLearningModule(filename, user);
                user.ConnectionString = css;
                dictionary = user.Open();

                //fetch all cards
                dictionary.PreloadCardCache();
                cards = dictionary.Cards.Cards;
            }
            catch (IOException)
            {
                TaskDialog.MessageBox(Properties.Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, Properties.Resources.DIC_ERROR_LOADING_LOCKED_CAPTION,
                    String.Format(Properties.Resources.DIC_ERROR_LOADING_LOCKED_TEXT, filename), TaskDialogButtons.OK, TaskDialogIcons.Error);
                throw;
            }
            catch (ProtectedLearningModuleException)
            {
                TaskDialog.MessageBox(Properties.Resources.DIC_ERROR_LOADING_PROTECTED_CAPTION, Properties.Resources.DIC_ERROR_LOADING_PROTECTED_CAPTION,
                    Properties.Resources.DIC_ERROR_LOADING_PROTECTED_TEXT, TaskDialogButtons.OK, TaskDialogIcons.Error);
                throw;
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Exception in LoadDictionary: " + exp.ToString());
                TaskDialog.MessageBox(Properties.Resources.DIC_ERROR_LOADING_CAPTION, Properties.Resources.DIC_ERROR_LOADING_CAPTION,
                    String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename), TaskDialogButtons.OK, TaskDialogIcons.Error);
                throw; //rethrow exception to notify the caller
            }
        }
        /// <summary>
        /// Saves the dictionary.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public void SaveDictionary()
        {
            dictionary.Save();
        }
        # endregion
        # region Communication
        /// <summary>
        /// Gets the word from the acutal card.
        /// </summary>
        /// <param name="cardIndex">Index of the card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string GetWord(int cardIndex, Side side)
        {
            switch (side)
            {
                case Side.Question:
                    return cards[cardIndex].Question.ToString();
                case Side.Answer:
                    return cards[cardIndex].Answer.ToString();
            }

            return string.Empty;
        }
        /// <summary>
        /// Gets the sentence from the acutal card.
        /// </summary>
        /// <param name="cardIndex">Index of the card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string GetSentence(int cardIndex, Side side)
        {
            switch (side)
            {
                case Side.Question:
                    return cards[cardIndex].QuestionExample.ToString();
                case Side.Answer:
                    return cards[cardIndex].AnswerExample.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets if the acutal card has the specifier media.
        /// </summary>
        /// <param name="cardIndex">Index of the card.</param>
        /// <param name="sentence">if set to <c>true</c>, the path of the file to the sentence is returned.</param>
        /// <param name="side">The side.</param>
        /// <returns>string.emty if no file available</returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public bool HasMediaFile(int cardIndex, bool sentence, Side side)
        {
            bool result = false;
            switch (side)
            {
                case Side.Question:
                    result = (cards[cardIndex].QuestionMedia as List<IMedia>).Exists(m => m.MediaType == EMedia.Audio && m.Example == sentence);
                    break;
                case Side.Answer:
                    result = (cards[cardIndex].AnswerMedia as List<IMedia>).Exists(m => m.MediaType == EMedia.Audio && m.Example == sentence);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the media file path from the acutal card.
        /// </summary>
        /// <param name="cardIndex">Index of the card.</param>
        /// <param name="sentence">if set to <c>true</c>, the path of the file to the sentence is returned.</param>
        /// <param name="side">The side.</param>
        /// <returns>string.emty if no file available</returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public string GetMediaFilePath(int cardIndex, bool sentence, Side side)
        {
            IMedia media = null;
            switch (side)
            {
                case Side.Question:
                    media = (cards[cardIndex].QuestionMedia as List<IMedia>).Find(m => m.MediaType == EMedia.Audio && m.Example == sentence);
                    break;
                case Side.Answer:
                    media = (cards[cardIndex].AnswerMedia as List<IMedia>).Find(m => m.MediaType == EMedia.Audio && m.Example == sentence);
                    break;
            }

            if (media != null)
                return media.Filename;

            return string.Empty;
        }

        /// <summary>
        /// Adds the media file to the actual card.
        /// </summary>
        /// <param name="cardIndex">Index of the card.</param>
        /// <param name="path">The path.</param>
        /// <param name="sentence">if set to <c>true</c> the file is the sentence of the actual card.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public bool AddMediaFile(int cardIndex, string path, bool sentence, Side side)
        {
            return AddMediaFile(cardIndex, path, sentence, side, null, false, true);
        }

        /// <summary>
        /// Adds the media file to the actual card.
        /// </summary>
        /// <param name="cardIndex">Index of the card.</param>
        /// <param name="path">The path.</param>
        /// <param name="sentence">if set to <c>true</c> the file is the sentence of the actual card.</param>
        /// <param name="side">The side.</param>
        /// <param name="codec">The codec to encode. Null to disable.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        public bool AddMediaFile(int cardIndex, string path, bool sentence, Side side, Codec codec, bool showWindow, bool minimizeWindow)
        {
            //add media object
            ICard card = cards[cardIndex];
            IMedia media = card.CreateMedia(EMedia.Audio, path, true, !sentence, sentence);
            card.AddMedia(media, side);

            if (codec != null && codec.CanEncode)
            {
                //create and add background encoding encodeJob
                EncodeJob encodeJob = new EncodeJob(media, card, side, codec);
                encodeJob.EncodingFinished += new EventHandler(encodeJob_EncodingFinished);
                encodeJob.ShowWindow = showWindow;
                encodeJob.MinimizeWindow = minimizeWindow;
                backgroundEncoder.AddJob(encodeJob);
            }
            else
                SaveDictionary();

            return true;
        }

        /// <summary>
        /// Handles the EncodingFinished event of the encodeJob control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        void encodeJob_EncodingFinished(object sender, EventArgs e)
        {
            if ((sender is EncodeJob) && ((EncodeJob)sender).exception != null)
                Trace.WriteLine("Encode Job failed: " + ((EncodeJob)sender).exception.ToString());

            SaveDictionary();
        }

        # endregion
    }
}
