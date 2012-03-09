using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MLifter.DAL.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace MLifterSettingsManager
{
    public class CardTreeViewItem : GeneralTreeViewItem
    {
        #region Private Fields

        private ICard card;
        private bool isChecked;
        private string answer = string.Empty;
        private string question = string.Empty;
        private string answerExample = string.Empty;
        private string questionExample = string.Empty;
        private int id;
        private bool? hasAudio = null;
        private bool? hasImage = null;
        private bool? hasVideo = null;
        private bool? hasExampleSentence = null;
        private bool hasCustomSettings = false;

        #endregion

        public CardTreeViewItem(ICard card, ChapterTreeViewItem parent)
            : base(parent)
        {
            this.card = card;

            //Cache
            question = BusinessLogic.HtmlHelper.HtmlStripTags(card.Question.ToString(), false, true);
            answer = BusinessLogic.HtmlHelper.HtmlStripTags(card.Answer.ToString(), false, true);
            questionExample = BusinessLogic.HtmlHelper.HtmlStripTags(card.QuestionExample.ToString(), false, true);
            answerExample = BusinessLogic.HtmlHelper.HtmlStripTags(card.AnswerExample.ToString(), false, true);
            id = card.Id;
            hasCustomSettings = card.Settings != null && !SettingsManagerBusinessLogic.IsEmptySetting(card.Settings) ? true : false;
        }

        /// <summary>
        /// Caches the index of the media.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public void CacheMediaInfo()
        {
            hasAudio = hasVideo = hasImage = false;
            if (card.AnswerMedia.Count > 0)
            {
                foreach (IMedia media in card.AnswerMedia)
                {
                    if (media.MediaType == EMedia.Audio)
                        hasAudio = true;
                    else if (media.MediaType == EMedia.Image)
                        hasImage = true;
                    else if (media.MediaType == EMedia.Video)
                        hasVideo = true;
                }
            }

            if (card.QuestionMedia.Count > 0)
            {
                foreach (IMedia media in card.QuestionMedia)
                {
                    if (media.MediaType == EMedia.Audio)
                        hasAudio = true;
                    else if (media.MediaType == EMedia.Image)
                        hasImage = true;
                    else if (media.MediaType == EMedia.Video)
                        hasVideo = true;
                }
            }
        }

        /// <summary>
        /// Caches the example sentence info.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void CacheExampleSentenceInfo()
        {
            hasExampleSentence = false;

            if (card.AnswerExample.Words.Count > 0)
            {
                if (card.AnswerExample.Words.Count == 1)
                    hasExampleSentence = card.AnswerExample.Words[0].Word != string.Empty;
                else
                    hasExampleSentence = true;
            }
            else if (!hasExampleSentence.Value && card.QuestionExample.Words.Count > 0)
            {
                if (card.QuestionExample.Words.Count == 1)
                    hasExampleSentence = card.QuestionExample.Words[0].Word != string.Empty;
                else
                    hasExampleSentence = true;
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets a value, if the item is checked (by checkbox)
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked == value)
                    return;

                isChecked = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }

        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <value>The question.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string Question
        {
            get
            {
                return question;
            }
        }

        /// <summary>
        /// Gets the answer.
        /// </summary>
        /// <value>The answer.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string Answer
        {
            get
            {
                return answer;
            }
        }

        /// <summary>
        /// Gets the question example.
        /// </summary>
        /// <value>The question example.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string QuestionExample
        {
            get
            {
                return questionExample;
            }
        }

        /// <summary>
        /// Gets the answer example.
        /// </summary>
        /// <value>The answer example.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string AnswerExample
        {
            get
            {
                return answerExample;
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ISettings Settings
        {
            get
            {
                return card.Settings;
            }
        }

        /// <summary>
        /// Gets the card.
        /// </summary>
        /// <value>The card.</value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public ICard Card
        {
            get
            {
                return card;
            }
        }

        /// <summary>
        /// Gets the learning module.
        /// </summary>
        /// <value>The learning module.</value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public IDictionary LearningModule
        {
            get
            {
                ChapterTreeViewItem item = Parent as ChapterTreeViewItem;
                LearningModuleTreeViewItem learningModuleTreeViewItem = item.Parent as LearningModuleTreeViewItem;

                return learningModuleTreeViewItem.LearningModule;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has audio.
        /// </summary>
        /// <value><c>true</c> if this instance has audio; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public bool HasAudio
        {
            get
            {
                if (!hasAudio.HasValue)
                    CacheMediaInfo();

                return hasAudio.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has image.
        /// </summary>
        /// <value><c>true</c> if this instance has image; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public bool HasImage
        {
            get
            {
                if (!hasImage.HasValue)
                    CacheMediaInfo();

                return hasImage.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has video.
        /// </summary>
        /// <value><c>true</c> if this instance has video; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public bool HasVideo
        {
            get
            {
                if (!hasVideo.HasValue)
                    CacheMediaInfo();

                return hasVideo.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has example sentence.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has example sentence; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public bool HasExampleSentence
        {
            get
            {
                if (!hasExampleSentence.HasValue)
                    CacheExampleSentenceInfo();

                return hasExampleSentence.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has custom settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom settings; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public bool HasCustomSettings
        {
            get
            {
                return hasCustomSettings;
            }
            set
            {
                hasCustomSettings = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs("HasCustomSettings"));
            }
        }

        /// <summary>
        /// Gets the question image.
        /// </summary>
        /// <value>The question image.</value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public string QuestionImage
        {
            get
            {
                if (!HasImage)
                    return "Resources\\empty.png";

                if (card.QuestionMedia.Count == 0)
                    return "Resources\\empty.png";

                foreach (IMedia media in card.QuestionMedia)
                {
                    if (media.MediaType == EMedia.Image)
                        return media.Filename;
                }

                return "Resources\\empty.png";
            }
        }

        /// <summary>
        /// Gets the answer image.
        /// </summary>
        /// <value>The answer image.</value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public string AnswerImage
        {
            get
            {
                if (!HasImage)
                    return "Resources\\empty.png";

                if (card.AnswerMedia.Count == 0)
                    return "Resources\\empty.png";

                foreach (IMedia media in card.AnswerMedia)
                {
                    if (media.MediaType == EMedia.Image)
                        return media.Filename;
                }

                return "Resources\\empty.png";
            }
        }

        #endregion
    }
}
