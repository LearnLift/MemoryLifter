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
            get { return (ISettings)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
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
        /// Occurs when apply was clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public event EventHandler ApplyClick;
        /// <summary>
        /// Raises the <see cref="E:ApplyClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        protected virtual void OnApplyClick(EventArgs e)
        {
            UpdateSettings();

            if (ApplyClick != null)
                ApplyClick(this, e);
        }

        /// <summary>
        /// Occurs when a value changed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public event EventHandler ValueChanged;
        /// <summary>
        /// Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        protected virtual void OnValueChanged(EventArgs e)
        {
            //UpdateSettings();

            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsControl"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public SettingsControl()
        {
            PreviewMode = false;

            InitializeComponent();

            numericUpDownControlNumberOfChoices.Minimum = 1;
            numericUpDownControlNumberOfChoices.Maximum = 999;
            numericUpDownControlNumberOfChoices.Value = 4;

            numericUpDownControlNumberOfCorrectAnswers.Minimum = 1;
            numericUpDownControlNumberOfCorrectAnswers.Maximum = numericUpDownControlNumberOfChoices.Value;
            numericUpDownControlNumberOfCorrectAnswers.Value = 3;

            DependencyPropertyDescriptor settingsDesc = DependencyPropertyDescriptor.FromProperty(SettingsControl.SettingsProperty, typeof(SettingsControl));
            settingsDesc.AddValueChanged(this, delegate { LoadSettings(); });
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownControlNumberOfChoices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void numericUpDownControlNumberOfChoices_ValueChanged(object sender, EventArgs e)
        {
            checkBoxNumberOfChoices.IsChecked = true;

            numericUpDownControlNumberOfCorrectAnswers.Maximum = numericUpDownControlNumberOfChoices.Value;

            OnValueChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownControlNumberOfCorrectAnswers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void numericUpDownControlNumberOfCorrectAnswers_ValueChanged(object sender, EventArgs e)
        {
            checkBoxNumberOfCorrectAnswers.IsChecked = true;

            OnValueChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Sets the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        public void SetSettings(ISettings settings)
        {
            Settings = settings;

            LoadSettings();
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <remarks>Documented by Dev05, 2009-04-10</remarks>
        private void LoadSettings()
        {
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

            checkBoxNumberOfChoices.IsChecked = Settings.MultipleChoiceOptions.NumberOfChoices.HasValue;
            labelNumberOfChoices.Content = Settings.MultipleChoiceOptions.NumberOfChoices.HasValue ? Settings.MultipleChoiceOptions.NumberOfChoices.Value : 4;
            numericUpDownControlNumberOfChoices.Value = Settings.MultipleChoiceOptions.NumberOfChoices.HasValue ? Settings.MultipleChoiceOptions.NumberOfChoices.Value : 4;

            checkBoxNumberOfCorrectAnswers.IsChecked = Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue;
            labelNumberOfCorrectAnswers.Content = Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue ? Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.Value : 4;
            if (Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.HasValue)
            {
                if (Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.Value > numericUpDownControlNumberOfCorrectAnswers.Maximum)
                    Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers = Convert.ToInt32(numericUpDownControlNumberOfCorrectAnswers.Maximum);

                numericUpDownControlNumberOfCorrectAnswers.Value = Settings.MultipleChoiceOptions.MaxNumberOfCorrectAnswers.Value;
            }
            else
                numericUpDownControlNumberOfCorrectAnswers.Value = 4;
        }

        /// <summary>
        /// Handles the Click event of the Apply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            OnApplyClick(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the checkBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            OnValueChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void UpdateSettings()
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

        private void checkBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as CheckBox).IsChecked = null;
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
