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
using MLifter.DAL.Interfaces;
using System.ComponentModel;
using System.Diagnostics;

namespace MLifterSettingsManager
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public ISettings Settings
        {
            get
            {
                return (ISettings)GetValue(SettingsProperty);
            }
            set
            {
                SetValue(SettingsProperty, value);
            }
        }
        /// <summary>
        /// Settings DP.
        /// </summary>
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings", typeof(ISettings), typeof(SettingsControl));

        /// <summary>
        /// Gets or sets a value indicating whether preview mode.
        /// </summary>
        /// <value><c>true</c> if [preview mode]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public bool PreviewMode
        {
            get { return (bool)GetValue(PreviewModeProperty); }
            set { SetValue(PreviewModeProperty, value); }
        }
        /// <summary>
        /// PreviewMode DP.
        /// </summary>
        public static readonly DependencyProperty PreviewModeProperty = DependencyProperty.Register("PreviewMode", typeof(bool), typeof(SettingsControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsControl"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public SettingsControl()
        {
            PreviewMode = false;

            InitializeComponent();

            numericUpDownControlNumberOfChoices.MinValue = 1;
            numericUpDownControlNumberOfChoices.MaxValue = 999;
            numericUpDownControlNumberOfChoices.Value = 4;

            numericUpDownControlNumberOfCorrectAnswers.MinValue = 1;
            numericUpDownControlNumberOfCorrectAnswers.MaxValue = numericUpDownControlNumberOfChoices.Value;
            numericUpDownControlNumberOfCorrectAnswers.Value = 3;

            DependencyPropertyDescriptor settingsDesc = DependencyPropertyDescriptor.FromProperty(SettingsControl.SettingsProperty, typeof(SettingsControl));
            settingsDesc.AddValueChanged(this, delegate { LoadSettings(); });
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownControlNumberOfChoices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void numericUpDownControlNumberOfChoices_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            checkBoxNumberOfChoices.IsChecked = true;
            numericUpDownControlNumberOfCorrectAnswers.MaxValue = numericUpDownControlNumberOfChoices.Value;
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownControlNumberOfCorrectAnswers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void numericUpDownControlNumberOfCorrectAnswers_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            checkBoxNumberOfCorrectAnswers.IsChecked = true;
        }

        /// <summary>
        /// Sets the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public void SetSettings(ISettings settings)
        {
            Settings.QueryTypes.Word = settings.QueryTypes.Word;
            Settings.QueryTypes.Sentence = settings.QueryTypes.Sentence;
            Settings.QueryTypes.MultipleChoice = settings.QueryTypes.MultipleChoice;
            Settings.QueryTypes.ListeningComprehension = settings.QueryTypes.ListeningComprehension;
            Settings.QueryTypes.ImageRecognition = settings.QueryTypes.ImageRecognition;

            Settings.QueryDirections.Question2Answer = settings.QueryDirections.Question2Answer;
            Settings.QueryDirections.Answer2Question = settings.QueryDirections.Answer2Question;
            Settings.QueryDirections.Mixed = settings.QueryDirections.Mixed;

            Settings.MultipleChoiceOptions.AllowMultipleCorrectAnswers = settings.MultipleChoiceOptions.AllowMultipleCorrectAnswers;
            Settings.MultipleChoiceOptions.AllowRandomDistractors = settings.MultipleChoiceOptions.AllowRandomDistractors;
            Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers = settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers;
            Settings.MultipleChoiceOptions.NumberOfChoices = settings.MultipleChoiceOptions.NumberOfChoices;

            LoadSettings();
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        private void LoadSettings()
        {
            if (Settings == null)       //Happens, when tooltip disappears (because the user moved the mouse), and the new binding does not contain any Settings, or something else
                return;

            checkBoxLearningDirectionAnswer2Question.IsChecked = Settings.QueryDirections.Answer2Question;
            checkBoxLearningDirectionMixed.IsChecked = Settings.QueryDirections.Mixed;
            checkBoxLearningDirectionQuestion2Answer.IsChecked = Settings.QueryDirections.Question2Answer;

            checkBoxLearnModeImageRecognition.IsChecked = Settings.QueryTypes.ImageRecognition;
            checkBoxLearnModeListeningComprehension.IsChecked = Settings.QueryTypes.ListeningComprehension;
            checkBoxLearnModeMultipleChoice.IsChecked = Settings.QueryTypes.MultipleChoice;
            checkBoxLearnModeSentence.IsChecked = Settings.QueryTypes.Sentence;
            checkBoxLearnModeStandard.IsChecked = Settings.QueryTypes.Word;

            checkBoxMultipleChoiceOptionsAllowMultipleCorrectAnswers.IsChecked = Settings.MultipleChoiceOptions.AllowMultipleCorrectAnswers;
            checkBoxMultipleChoiceOptionsAllowRandomDistractors.IsChecked = Settings.MultipleChoiceOptions.AllowRandomDistractors;

            numericUpDownControlNumberOfChoices.Value = Settings.MultipleChoiceOptions.NumberOfChoices.HasValue ? Settings.MultipleChoiceOptions.NumberOfChoices.Value : 4;
            numericUpDownControlNumberOfCorrectAnswers.Value = Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue ? Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.Value : 1;
            numericUpDownControlNumberOfCorrectAnswers.MaxValue = numericUpDownControlNumberOfChoices.Value;

            checkBoxNumberOfChoices.IsChecked = Settings.MultipleChoiceOptions.NumberOfChoices.HasValue;
            checkBoxNumberOfCorrectAnswers.IsChecked = Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue;
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public void UpdateSettings()
        {
            if (Settings == null)
                return;

            Settings.QueryDirections.Answer2Question = checkBoxLearningDirectionAnswer2Question.IsChecked;
            Settings.QueryDirections.Mixed = checkBoxLearningDirectionMixed.IsChecked;
            Settings.QueryDirections.Question2Answer = checkBoxLearningDirectionQuestion2Answer.IsChecked;

            Settings.QueryTypes.ImageRecognition = checkBoxLearnModeImageRecognition.IsChecked;
            Settings.QueryTypes.ListeningComprehension = checkBoxLearnModeListeningComprehension.IsChecked;
            Settings.QueryTypes.MultipleChoice = checkBoxLearnModeMultipleChoice.IsChecked;
            Settings.QueryTypes.Sentence = checkBoxLearnModeSentence.IsChecked;
            Settings.QueryTypes.Word = checkBoxLearnModeStandard.IsChecked;

            Settings.MultipleChoiceOptions.AllowMultipleCorrectAnswers = checkBoxMultipleChoiceOptionsAllowMultipleCorrectAnswers.IsChecked;
            Settings.MultipleChoiceOptions.AllowRandomDistractors = checkBoxMultipleChoiceOptionsAllowRandomDistractors.IsChecked;
            Settings.MultipleChoiceOptions.NumberOfChoices = checkBoxNumberOfChoices.IsChecked.Value ? Convert.ToInt32(numericUpDownControlNumberOfChoices.Value) : (int?)null;
            Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers = checkBoxNumberOfCorrectAnswers.IsChecked.Value ? Convert.ToInt32(numericUpDownControlNumberOfCorrectAnswers.Value) : (int?)null;
        }

        /// <summary>
        /// Clears the settings.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public void ClearSettings()
        {
            if (Settings == null)
                return;

            //Settings.QueryTypes.ImageRecognition = checkBoxLearnModeImageRecognition.IsChecked = null;
            //Settings.QueryTypes.ListeningComprehension = checkBoxLearnModeListeningComprehension.IsChecked = null;
            //Settings.QueryTypes.MultipleChoice = checkBoxLearnModeMultipleChoice.IsChecked = null;
            //Settings.QueryTypes.Sentence = checkBoxLearnModeSentence.IsChecked = null;
            //Settings.QueryTypes.Word = checkBoxLearnModeStandard.IsChecked = null;

            //Settings.QueryDirections.Answer2Question = checkBoxLearningDirectionAnswer2Question.IsChecked = null;
            //Settings.QueryDirections.Question2Answer = checkBoxLearningDirectionMixed.IsChecked = null;
            //Settings.QueryDirections.Mixed = checkBoxLearningDirectionMixed.IsChecked = null;

            //Settings.MultipleChoiceOptions.AllowMultipleCorrectAnswers = checkBoxMultipleChoiceOptionsAllowMultipleCorrectAnswers.IsChecked = null;
            //Settings.MultipleChoiceOptions.AllowRandomDistractors = checkBoxMultipleChoiceOptionsAllowRandomDistractors.IsChecked = null;
            //Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers = null;
            //Settings.MultipleChoiceOptions.NumberOfChoices = null;

            //checkBoxNumberOfChoices.IsChecked = false;
            //checkBoxNumberOfCorrectAnswers.IsChecked = false;
            //numericUpDownControlNumberOfChoices.Value = 4;
            //numericUpDownControlNumberOfCorrectAnswers.Value = 1;
            //numericUpDownControlNumberOfCorrectAnswers.Maximum = 4;
            checkBoxLearnModeImageRecognition.IsChecked = null;
            checkBoxLearnModeListeningComprehension.IsChecked = null;
            checkBoxLearnModeMultipleChoice.IsChecked = null;
            checkBoxLearnModeSentence.IsChecked = null;
            checkBoxLearnModeStandard.IsChecked = null;

            checkBoxLearningDirectionAnswer2Question.IsChecked = null;
            checkBoxLearningDirectionMixed.IsChecked = null;
            checkBoxLearningDirectionMixed.IsChecked = null;

            checkBoxMultipleChoiceOptionsAllowMultipleCorrectAnswers.IsChecked = null;
            checkBoxMultipleChoiceOptionsAllowRandomDistractors.IsChecked = null;

            checkBoxNumberOfChoices.IsChecked = false;
            checkBoxNumberOfCorrectAnswers.IsChecked = false;
            numericUpDownControlNumberOfChoices.Value = 4;
            numericUpDownControlNumberOfCorrectAnswers.Value = 1;
            numericUpDownControlNumberOfCorrectAnswers.MaxValue = 4;
        }

        /// <summary>
        /// Handles the MouseRightButtonUp event of the checkBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        private void checkBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as CheckBox).IsChecked = null;
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    /// <summary>
    /// Converts bool to visibility.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-04-10</remarks>
    public class BoolToVisibleOrCollapsedConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("Target must be Visibility!");

            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
