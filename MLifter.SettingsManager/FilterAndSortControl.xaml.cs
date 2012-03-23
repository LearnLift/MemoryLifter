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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MLifter.DAL.Interfaces;
using System.Diagnostics;
using System.Collections;

namespace MLifterSettingsManager
{
    /// <summary>
    /// Interaction logic for FilterAndSortControl.xaml
    /// </summary>
    public partial class FilterAndSortControl : UserControl, INotifyPropertyChanged
    {
        #region Private fields

        private string cardFilter = string.Empty;
        private List<ICollectionView> CardFilterViewList;
        private string progressBarText = string.Empty;
        private int progressBarValue = 0;
        private int progressBarMaximum = 100;
        private bool mainWindowEnabled = true;

        #endregion

        public FilterAndSortControl()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the tree view items.
        /// </summary>
        /// <value>The tree view items.</value>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public ObservableCollection<LearningModuleTreeViewItem> TreeViewItems { get; set; }

        /// <summary>
        /// Gets or sets the card filter.
        /// </summary>
        /// <value>The card filter.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public string CardFilter
        {
            get
            {
                return cardFilter;
            }
            set
            {
                if (cardFilter == value)
                    return;

                cardFilter = value;
                OnUserControlPropertyChanged(this, new PropertyChangedEventArgs("CardFilter"));

                UpdateFilter();
            }
        }

        /// <summary>
        /// Gets the progress bar text.
        /// </summary>
        /// <value>The progress bar text.</value>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public string ProgressBarText
        {
            get
            {
                return progressBarText;
            }
            set
            {
                progressBarText = value;
                OnUserControlPropertyChanged(this, new PropertyChangedEventArgs("ProgressBarText"));
            }
        }

        /// <summary>
        /// Gets or sets the progress bar value.
        /// </summary>
        /// <value>The progress bar value.</value>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public int ProgressBarValue
        {
            get
            {
                return progressBarValue;
            }
            set
            {
                if (progressBarValue == value)
                    return;

                progressBarValue = value;
                OnUserControlPropertyChanged(this, new PropertyChangedEventArgs("ProgressBarValue"));
            }
        }

        /// <summary>
        /// Gets or sets the progress bar maximum.
        /// </summary>
        /// <value>The progress bar maximum.</value>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public int ProgressBarMaximum
        {
            get
            {
                return progressBarMaximum;
            }
            set
            {
                if (progressBarMaximum == value)
                    return;

                progressBarMaximum = value;
                OnUserControlPropertyChanged(this, new PropertyChangedEventArgs("ProgressBarMaximum"));
            }
        }

        /// <summary>
        /// Gets or sets the main window enabled.
        /// </summary>
        /// <value>The main window enabled.</value>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public bool MainWindowEnabled
        {
            get
            {
                return mainWindowEnabled;
            }
            set
            {
                mainWindowEnabled = value;
                OnUserControlPropertyChanged(this, new PropertyChangedEventArgs("MainWindowEnabled"));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnUserControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, e);
        }

        #endregion

        #region public Methods

        public void ReloadFilter()
        {
            CardFilterViewList = new List<ICollectionView>();
            foreach (ChapterTreeViewItem item in TreeViewItems[0].Chapters)
                CardFilterViewList.Add(CollectionViewSource.GetDefaultView(item.Cards));
            InitCardSearchFilter();
        }

        #endregion

        #region private Methods

        /// <summary>
        /// Updates the filter.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void UpdateFilter()
        {
            if (CardFilterViewList != null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (ICollectionView view in CardFilterViewList)
                    view.Refresh();

                sw.Stop();
                Trace.WriteLine(" +++ Update Filter +++ " + sw.ElapsedMilliseconds + " ms");
            }
        }

        /// <summary>
        /// Inits the text search filter.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void InitCardSearchFilter()
        {
            foreach (ICollectionView cardView in CardFilterViewList)
            {
                cardView.Filter = delegate(object obj)
                {
                    CardTreeViewItem cardTreeViewItem = obj as CardTreeViewItem;

                    if (cardTreeViewItem == null)
                        return false;

                    if (!CardSearchFilterText(cardFilter, new List<string>() { cardTreeViewItem.Question, cardTreeViewItem.Answer, cardTreeViewItem.QuestionExample, cardTreeViewItem.AnswerExample }))
                        return false;

                    if (!CardSearchFilterLearningMode(cardTreeViewItem.Settings.QueryTypes))
                        return false;

                    if (!CardSearchFilterDirection(cardTreeViewItem.Settings.QueryDirections))
                        return false;

                    if (!CardSearchFilterMCOptions(cardTreeViewItem.Settings.MultipleChoiceOptions))
                        return false;

                    if (!CardSearchFilterMedia(cardTreeViewItem))
                        return false;

                    if (!CardSearchExampleSentence(cardTreeViewItem))
                        return false;

                    return true;
                };
            }
        }

        /// <summary>
        /// Filter by Card text content
        /// </summary>
        /// <param name="searchStr">The search STR.</param>
        /// <param name="actualStr">The actual STR.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool CardSearchFilterText(string searchStr, List<string> actualStrings)
        {
            if (string.IsNullOrEmpty(searchStr))        //nothing typed --> show all
                return true;

            return actualStrings.Exists(s => s.ToLower().Contains(searchStr.ToLower()));
        }

        /// <summary>
        /// Filter by LearningMode
        /// </summary>
        /// <param name="actualQueryType">Actual type of the query.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool CardSearchFilterLearningMode(IQueryType actualQueryType)
        {
            //If all checkboxes are unchecked, return true;
            if (!checkBoxFilterStandard.IsChecked.Value && !checkBoxFilterSentence.IsChecked.Value && !checkBoxFilterMC.IsChecked.Value &&
                !checkBoxFilterListening.IsChecked.Value && !checkBoxFilterImage.IsChecked.Value)
                return true;

            //At least one checkbox is checked, so don't show cards with "null" values
            if (!actualQueryType.Word.HasValue || !actualQueryType.Sentence.HasValue || !actualQueryType.MultipleChoice.HasValue ||
                !actualQueryType.ListeningComprehension.HasValue || !actualQueryType.ImageRecognition.HasValue)
                return false;

            if (actualQueryType.Word.Value != checkBoxFilterStandard.IsChecked.Value)
                return false;

            if (actualQueryType.Sentence.Value != checkBoxFilterSentence.IsChecked.Value)
                return false;

            if (actualQueryType.MultipleChoice.Value != checkBoxFilterMC.IsChecked.Value)
                return false;

            if (actualQueryType.ListeningComprehension.Value != checkBoxFilterListening.IsChecked.Value)
                return false;

            if (actualQueryType.ImageRecognition.Value != checkBoxFilterImage.IsChecked.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Filter by direction
        /// </summary>
        /// <param name="actualQueryDirection">The actual query direction.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool CardSearchFilterDirection(IQueryDirections actualQueryDirection)
        {
            //If all checkboxes are unchecked, return true;
            if (!checkBoxFilterQA.IsChecked.Value && !checkBoxFilterAQ.IsChecked.Value && !checkBoxFilterMix.IsChecked.Value)
                return true;

            //At least one checkbox is checked, so don't show cards with "null" values
            if (!actualQueryDirection.Question2Answer.HasValue || !actualQueryDirection.Answer2Question.HasValue || !actualQueryDirection.Mixed.HasValue)
                return false;

            if (actualQueryDirection.Question2Answer.Value != checkBoxFilterQA.IsChecked.Value)
                return false;

            if (actualQueryDirection.Answer2Question.Value != checkBoxFilterAQ.IsChecked.Value)
                return false;

            if (actualQueryDirection.Mixed.Value != checkBoxFilterMix.IsChecked.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Cards the search filter MC options.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool CardSearchFilterMCOptions(IQueryMultipleChoiceOptions mcOptions)
        {
            //If all checkboxes are unchecked, return true;
            if (!toggleButtonAllowMultipleAnswers.IsChecked.Value && !toggleButtonAllowRandDistractors.IsChecked.Value && !toggleButtonMaxNumber.IsChecked.Value &&
                !toggleButtonNumberChoices.IsChecked.Value)
                return true;

            if (toggleButtonAllowMultipleAnswers.IsChecked.Value)
            {
                if (!mcOptions.AllowMultipleCorrectAnswers.HasValue)
                    return false;

                if (checkBoxFilterAllowMultipleCorrectAnswers.IsChecked.Value != mcOptions.AllowMultipleCorrectAnswers.Value)
                    return false;
            }

            if (toggleButtonAllowRandDistractors.IsChecked.Value)
            {
                if (!mcOptions.AllowRandomDistractors.HasValue)
                    return false;

                if (checkboxFilterAllowRandomDistractors.IsChecked.Value != mcOptions.AllowRandomDistractors.Value)
                    return false;
            }

            if (toggleButtonMaxNumber.IsChecked.Value)
            {
                if (!mcOptions.MaxNumberOfCorrectAnswers.HasValue)
                    return false;

                if (numericUpDownMaxNumberCorrectAnswers.Value != mcOptions.MaxNumberOfCorrectAnswers.Value)
                    return false;
            }

            if (toggleButtonNumberChoices.IsChecked.Value)
            {
                if (!mcOptions.NumberOfChoices.HasValue)
                    return false;

                if (numericUpDownNumberOfChoices.Value != mcOptions.NumberOfChoices.Value)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Filter by Media
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool CardSearchFilterMedia(CardTreeViewItem card)
        {
            //If all checkboxes are unchecked, return true;
            if (!checkBoxFilterPicture.IsChecked.Value && !checkBoxFilterAudio.IsChecked.Value && !checkBoxFilterVideo.IsChecked.Value)
                return true;

            if (card.HasAudio != checkBoxFilterAudio.IsChecked.Value)
                return false;

            if (card.HasImage != checkBoxFilterPicture.IsChecked.Value)
                return false;

            if (card.HasVideo != checkBoxFilterVideo.IsChecked.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if there is a media from the given type in the list
        /// </summary>
        /// <param name="mediaList">The media list.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool ExistsMedia(IList<IMedia> mediaList, EMedia mediaType)
        {
            if (mediaList.Count == 0)
                return false;

            bool mediaFound = false;
            foreach (IMedia media in mediaList)
            {
                if (media.MediaType == mediaType)
                {
                    mediaFound = true;
                    break;
                }
            }

            return mediaFound;
        }

        /// <summary>
        /// Filters by Example Sentence
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private bool CardSearchExampleSentence(CardTreeViewItem card)
        {
            if (!checkBoxFilterExampleSentence.IsChecked.Value)
                return true;

            return card.HasExampleSentence;
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the buttonClearText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void buttonClearText_Click(object sender, RoutedEventArgs e)
        {
            CardFilter = string.Empty;
        }

        /// <summary>
        /// Handles the Click event of the buttonLearningModeDirectionClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void buttonLearningModeDirectionClear_Click(object sender, RoutedEventArgs e)
        {
            checkBoxFilterStandard.IsChecked = checkBoxFilterSentence.IsChecked = checkBoxFilterMC.IsChecked
                                             = checkBoxFilterListening.IsChecked = checkBoxFilterImage.IsChecked
                                             = checkBoxFilterQA.IsChecked = checkBoxFilterAQ.IsChecked = checkBoxFilterMix.IsChecked = false;
            UpdateFilter();
        }

        /// <summary>
        /// Handles the Click event of the buttonMCOptionsClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void buttonMCOptionsClear_Click(object sender, RoutedEventArgs e)
        {
            toggleButtonAllowMultipleAnswers.IsChecked = toggleButtonAllowRandDistractors.IsChecked = toggleButtonMaxNumber.IsChecked = toggleButtonNumberChoices.IsChecked = false;
            UpdateFilter();
        }

        /// <summary>
        /// Handles the Click event of the buttonOthersClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void buttonOthersClear_Click(object sender, RoutedEventArgs e)
        {
            checkBoxFilterPicture.IsChecked = checkBoxFilterAudio.IsChecked = checkBoxFilterVideo.IsChecked = checkBoxFilterExampleSentence.IsChecked = false;
            UpdateFilter();
        }

        /// <summary>
        /// Handles the Click event of the checkBoxFilterLearningMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void checkBoxFilter_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckBox)
            {
                if ((e.Source as CheckBox) == checkBoxFilterPicture || (e.Source as CheckBox) == checkBoxFilterAudio || (e.Source as CheckBox) == checkBoxFilterVideo)
                {
                    LearningModuleTreeViewItem item = TreeViewItems[0];
                    if (!item.IsMediaInfoLoaded)
                    {
                        item.MediaContentLoaded += new EventHandler(MediaContentLoaded);
                        item.MediaContentLoading += new LearningModuleTreeViewItem.MediaContentLoadingEventHandler(MediaContentLoading);
                        item.LoadLearningModuleMediaInfo();

                        return;
                    }
                }
                else if ((e.Source as CheckBox) == checkBoxFilterExampleSentence)
                {
                    LearningModuleTreeViewItem item = TreeViewItems[0];
                    if (!item.IsExampleInfoLoaded)
                    {
                        item.ExampleContentLoaded += new EventHandler(ExampleContentLoaded);
                        item.ExampleContentLoading += new LearningModuleTreeViewItem.ExampleContentLoadingEventHandler(ExampleContentLoading);
                        item.LoadLearningModuleExampleSentenceInfo();

                        return;
                    }
                }
            }

            UpdateFilter();
        }

        private void ExampleContentLoading(object sender, ContentLoadingEventArgs e)
        {
            MainWindowEnabled = false;
            ProgressBarText = e.ContentType == ContentType.Card ? "Loading Card Example Sentences Info..." : string.Empty;
            ProgressBarMaximum = e.Maximum;
            ProgressBarValue = e.Value;
        }

        private void ExampleContentLoaded(object sender, EventArgs e)
        {
            ProgressBarText = string.Empty;
            ProgressBarValue = 0;
            MainWindowEnabled = true;

            UpdateFilter();
        }

        /// <summary>
        /// Medias the content loading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ContentLoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void MediaContentLoading(object sender, ContentLoadingEventArgs e)
        {
            MainWindowEnabled = false;
            ProgressBarText = e.ContentType == ContentType.Card ? "Loading Card Media Info..." : string.Empty;
            ProgressBarMaximum = e.Maximum;
            ProgressBarValue = e.Value;
        }

        /// <summary>
        /// Medias the content loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void MediaContentLoaded(object sender, EventArgs e)
        {
            ProgressBarText = string.Empty;
            ProgressBarValue = 0;
            MainWindowEnabled = true;

            UpdateFilter();
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownMaxNumberCorrectAnswers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void numericUpDownMaxNumberCorrectAnswers_ValueChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        /// <summary>
        /// Handles the Click event of the toggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilter();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboBoxSort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardFilterViewList == null)
                return;

            string sortTitle = string.Empty;
            if ((e.AddedItems[0] as System.Windows.Controls.ComboBoxItem) == comboBoxItemSortId)
                sortTitle = "Id";
            else if ((e.AddedItems[0] as System.Windows.Controls.ComboBoxItem) == comboBoxItemSortQuestion)
                sortTitle = "Question";
            else if ((e.AddedItems[0] as System.Windows.Controls.ComboBoxItem) == comboBoxItemSortAnswer)
                sortTitle = "Answer";

            foreach (ChapterTreeViewItem item in TreeViewItems[0].Chapters)
            {
                ListCollectionView listCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(item.Cards);
                listCollectionView.CustomSort = new CardSorter((CardSortColumn)Enum.Parse(typeof(CardSortColumn), sortTitle));
            }
        }
    }

    /// <summary>
    /// Very very very very much faster than using the ICollectionView
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-07-16</remarks>
    public class CardSorter : IComparer
    {
        private CardSortColumn column;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardSorter"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public CardSorter(CardSortColumn column)
        {
            this.column = column;
        }

        #region IComparer Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.
        /// -or-
        /// <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
        /// </exception>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public int Compare(object x, object y)
        {
            if (!(x is CardTreeViewItem) || !(y is CardTreeViewItem))
                return -1;

            CardTreeViewItem card1 = x as CardTreeViewItem;
            CardTreeViewItem card2 = y as CardTreeViewItem;

            if (column == CardSortColumn.Id)
            {
                if (card1.Id == card2.Id)
                    return 0;
                else if (card1.Id < card2.Id)
                    return -1;
                else if (card1.Id > card2.Id)
                    return 1;
            }

            if (column == CardSortColumn.Question)
                return string.Compare(card1.Question, card2.Question);

            if (column == CardSortColumn.Answer)
                return string.Compare(card1.Answer, card2.Answer);

            return -1;
        }

        #endregion
    }

    public enum CardSortColumn
    {
        Id,
        Question,
        Answer
    }
}
