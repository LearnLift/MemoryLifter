using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MLifter.DAL.Interfaces;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;

namespace MLifterSettingsManager
{
    public class LearningModuleTreeViewItem : GeneralTreeViewItem
    {
        #region Private Fields

        private IDictionary learningModule;
        private bool isMediaInfoLoaded = false;
        private bool isExampleInfoLoaded = false;
        private bool hasCustomSettings = false;

        /// <summary>
        /// You don't no what a SynchronizationContext is? See: http://www.codeproject.com/KB/cpp/SyncContextTutorial.aspx
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private SynchronizationContext context;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModuleTreeViewItem"/> class.
        /// </summary>
        /// <param name="learningModule">The learning module.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public LearningModuleTreeViewItem(IDictionary learningModule)
            : base()
        {
            context = SynchronizationContext.Current;
            if (context == null)
                context = new SynchronizationContext();

            this.learningModule = learningModule;
            hasCustomSettings = learningModule.AllowedSettings != null && !SettingsManagerBusinessLogic.IsEmptySetting(learningModule.AllowedSettings) ? true : false;
        }

        #region Properties

        public delegate void ContentLoadingEventHandler(object sender, ContentLoadingEventArgs e);
        public event ContentLoadingEventHandler ContentLoading;

        public event EventHandler ContentLoaded;

        public delegate void MediaContentLoadingEventHandler(object sender, ContentLoadingEventArgs e);
        public event MediaContentLoadingEventHandler MediaContentLoading;

        public event EventHandler MediaContentLoaded;

        public delegate void ExampleContentLoadingEventHandler(object sender, ContentLoadingEventArgs e);
        public event ExampleContentLoadingEventHandler ExampleContentLoading;

        public event EventHandler ExampleContentLoaded;

        /// <summary>
        /// Gets a value indicating whether this instance is media info loaded. (HasImage/Audio/Video)
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is media info loaded; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public bool IsMediaInfoLoaded
        {
            get
            {
                return isMediaInfoLoaded;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is example info loaded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is example info loaded; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public bool IsExampleInfoLoaded
        {
            get
            {
                return isExampleInfoLoaded;
            }
        }

        /// <summary>
        /// Gets or sets the chapters.
        /// </summary>
        /// <value>The chapters.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ObservableCollection<ChapterTreeViewItem> Chapters { get; set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string Title
        {
            get
            {
                return learningModule.Title;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [content protected].
        /// </summary>
        /// <value><c>true</c> if [content protected]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public bool ContentProtected
        {
            get
            {
                return learningModule.ContentProtected;
            }
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string FilePath
        {
            get
            {
                return learningModule.Connection;
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
                return learningModule.AllowedSettings;
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
                return learningModule;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has custom settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has custom settings; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
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

        #endregion

        /// <summary>
        /// Loads the content of the learning module.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void LoadLearningModuleContent()
        {
            new Thread(LoadContent) { Name = "Load LearningModuleContent Thread", IsBackground = true }.Start();
        }

        /// <summary>
        /// Loads the learning module media info.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void LoadLearningModuleMediaInfo()
        {
            new Thread(LoadMediaInfo) { Name = "Load LearningModule MediaInfo Thread", IsBackground = true }.Start();
        }

        /// <summary>
        /// Loads the learning module example sentence info.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void LoadLearningModuleExampleSentenceInfo()
        {
            new Thread(LoadExampleSentenceInfo) { Name = "Load LearningModule Example Sentences Info Thread", IsBackground = true }.Start();
        }

        /// <summary>
        /// Loads/caches the media info.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void LoadMediaInfo()
        {
            int counter = 0;
            int max = learningModule.Cards.Count;
            foreach (ChapterTreeViewItem chapterTreeViewItem in Chapters)
            {
                foreach (CardTreeViewItem cardTreeViewItem in chapterTreeViewItem.Cards)
                {
                    context.Post(new SendOrPostCallback(delegate(object state)
                    {
                        OnMediaContentLoading(this, new ContentLoadingEventArgs(counter, max, ContentType.Card));
                    }), null);

                    cardTreeViewItem.CacheMediaInfo();
                    ++counter;
                }
            }

            isMediaInfoLoaded = true;

            context.Post(new SendOrPostCallback(delegate(object state)
            {
                OnMediaContentLoaded(this, EventArgs.Empty);
            }), null);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void LoadContent()
        {
            Chapters = new ObservableCollection<ChapterTreeViewItem>();

            int counter = 0;
            int max = learningModule.Chapters.Count;
            foreach (IChapter chapter in learningModule.Chapters.Chapters)
            {
                ChapterTreeViewItem item = new ChapterTreeViewItem(chapter, this);
                item.Cards = new ObservableCollection<CardTreeViewItem>();
                Chapters.Add(item);
                ++counter;
            }

            counter = 0;
            max = learningModule.Cards.Count;
            foreach (ICard card in learningModule.Cards.Cards)
            {
                context.Post(new SendOrPostCallback(delegate(object state)
                {
                    OnContentLoading(this, new ContentLoadingEventArgs(counter, max, ContentType.Card));
                }), null);

                foreach (ChapterTreeViewItem item in Chapters)
                {
                    if (item.Id == card.Chapter)
                    {
                        item.Cards.Add(new CardTreeViewItem(card, item));
                        break;
                    }
                }
                ++counter;
            }

            context.Post(new SendOrPostCallback(delegate(object state)
            {
                OnContentLoaded(this, EventArgs.Empty);
            }), null);
        }

        /// <summary>
        /// Loads the example sentence info.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void LoadExampleSentenceInfo()
        {
            int counter = 0;
            int max = learningModule.Cards.Count;
            foreach (ChapterTreeViewItem chapterTreeViewItem in Chapters)
            {
                foreach (CardTreeViewItem cardTreeViewItem in chapterTreeViewItem.Cards)
                {
                    context.Post(new SendOrPostCallback(delegate(object state)
                    {
                        OnExampleContentLoading(this, new ContentLoadingEventArgs(counter, max, ContentType.Card));
                    }), null);

                    cardTreeViewItem.CacheExampleSentenceInfo();
                    ++counter;
                }
            }

            isExampleInfoLoaded = true;

            context.Post(new SendOrPostCallback(delegate(object state)
            {
                OnExampleContentLoaded(this, EventArgs.Empty);
            }), null);
        }

        /// <summary>
        /// Called when [loading content].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ContentLoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnContentLoading(object sender, ContentLoadingEventArgs e)
        {
            ContentLoadingEventHandler handler = ContentLoading;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [content loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnContentLoaded(object sender, EventArgs e)
        {
            EventHandler handler = ContentLoaded;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [media content loading].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ContentLoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnMediaContentLoading(object sender, ContentLoadingEventArgs e)
        {
            MediaContentLoadingEventHandler handler = MediaContentLoading;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [media content loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnMediaContentLoaded(object sender, EventArgs e)
        {
            EventHandler handler = MediaContentLoaded;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [example content loading].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ContentLoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnExampleContentLoading(object sender, ContentLoadingEventArgs e)
        {
            ExampleContentLoadingEventHandler handler = ExampleContentLoading;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [example content loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnExampleContentLoaded(object sender, EventArgs e)
        {
            EventHandler handler = ExampleContentLoaded;
            if (handler != null)
                handler(sender, e);
        }
    }

    /// <summary>
    /// LoadingContentEventArgs
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-07-18</remarks>
    public class ContentLoadingEventArgs : EventArgs
    {
        public int Value { get; private set; }

        public int Maximum { get; private set; }

        public ContentType ContentType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingContentEventArgs"/> class.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public ContentLoadingEventArgs(int currentValue, int maxValue, ContentType contentType)
        {
            Value = currentValue;
            Maximum = maxValue;
            ContentType = contentType;
        }
    }

    public enum ContentType
    {
        Chapter,
        Card
    }
}
