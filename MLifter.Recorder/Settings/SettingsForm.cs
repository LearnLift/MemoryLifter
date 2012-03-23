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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Globalization;
using MLifter.AudioTools.Codecs;
using MLifter.Recorder.Properties;

namespace MLifter.AudioTools
{
    public partial class SettingsForm : Form
    {
        private Settings settings;
        private SerializableDictionary<Keys, Function> backupFunctions;
        private SerializableDictionary<Keys, Function> backupKeyboardFunctions;
        private bool backupKeyboardMode;
        private NumPadControl numPadControl;
        private DictionaryManagement dictionaryManager;
        private bool loading;

        private ListViewItem questionItem;
        private ListViewItem questionExampleItem;
        private ListViewItem answerItem;
        private ListViewItem answerExampleItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsForm"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="numPadControl">The num pad control.</param>
        /// <param name="dictionaryManager">The dictionary manager.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        public SettingsForm(Settings settings, NumPadControl numPadControl, DictionaryManagement dictionaryManager)
        {
            InitializeComponent();

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(groupBoxDelays, Resources.DELAY_DESCRIPTION);

            this.settings = settings;

            backupFunctions = new SerializableDictionary<Keys, Function>();
            foreach (KeyValuePair<Keys, Function> pair in settings.KeyFunctions)
                backupFunctions.Add(pair.Key, pair.Value);
            backupKeyboardFunctions = new SerializableDictionary<Keys, Function>();
            foreach (KeyValuePair<Keys, Function> pair in settings.KeyboardFunctions)
                backupKeyboardFunctions.Add(pair.Key, pair.Value);
            backupKeyboardMode = settings.KeyboardLayout;

            this.numPadControl = numPadControl;
            this.dictionaryManager = dictionaryManager;

            LoadValues();
        }

        /// <summary>
        /// Loads the values out of the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="numPadControl">The num pad control.</param>
        /// <remarks>Documented by Dev05, 2007-08-07</remarks>
        private void LoadValues()
        {
            loading = true;

            checkBoxMultipleAssignment.Checked = settings.AllowMultipleAssignment;
            checkBoxPresenter.Checked = settings.PresenterActivated;

            comboBoxDecimal.Tag = Keys.Decimal;
            comboBoxDivide.Tag = Keys.Divide;
            comboBoxEnter.Tag = Keys.Enter;
            comboBoxMinus.Tag = Keys.Subtract;
            comboBoxMultiply.Tag = Keys.Multiply;
            comboBoxNum0.Tag = Keys.NumPad0;
            comboBoxNum1.Tag = Keys.NumPad1;
            comboBoxNum2.Tag = Keys.NumPad2;
            comboBoxNum3.Tag = Keys.NumPad3;
            comboBoxNum4.Tag = Keys.NumPad4;
            comboBoxNum5.Tag = Keys.NumPad5;
            comboBoxNum6.Tag = Keys.NumPad6;
            comboBoxNum7.Tag = Keys.NumPad7;
            comboBoxNum8.Tag = Keys.NumPad8;
            comboBoxNum9.Tag = Keys.NumPad9;
            comboBoxPlus.Tag = Keys.Add;

            comboBoxSpace.Tag = Keys.Space;
            comboBoxC.Tag = Keys.C;
            comboBoxV.Tag = Keys.V;
            comboBoxB.Tag = Keys.B;
            comboBoxN.Tag = Keys.N;
            comboBoxM.Tag = Keys.M;

            foreach (Control control in tabPageNumPad.Controls)
            {
                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox comboBox = (ComboBox)control;

                    if (comboBox.Tag is Keys)
                    {
                        comboBox.Items.Clear();
                        comboBox.Items.Add("");
                        comboBox.Items.Add(Function.Backward);
                        comboBox.Items.Add(Function.Forward);
                        comboBox.Items.Add(Function.Play);
                        comboBox.Items.Add(Function.Record);

                        if (settings.KeyFunctions.ContainsKey((Keys)comboBox.Tag))
                            comboBox.SelectedItem = settings.KeyFunctions[(Keys)comboBox.Tag];
                    }
                }
            }

            foreach (Control control in tabPageKeyboard.Controls)
            {
                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox comboBox = (ComboBox)control;

                    if (comboBox.Tag is Keys)
                    {
                        comboBox.Items.Clear();
                        comboBox.Items.Add("");
                        comboBox.Items.Add(Function.Backward);
                        comboBox.Items.Add(Function.Forward);
                        comboBox.Items.Add(Function.Play);
                        comboBox.Items.Add(Function.Record);

                        if (settings.KeyboardFunctions.ContainsKey((Keys)comboBox.Tag))
                            comboBox.SelectedItem = settings.KeyboardFunctions[(Keys)comboBox.Tag];
                    }
                }
            }

            if (!settings.AllowMultipleAssignment)
                RemoveDoubleAssignemts();

            numericUpDownStartDelay.Value = settings.StartDelay;
            numericUpDownStopDelay.Value = settings.StopDelay;

            listViewElementsToRecord.Items.Clear();

            questionItem = new ListViewItem(string.Format(Resources.LISTBOXFIELDS_QUESTION_TEXT, dictionaryManager.QuestionCaption));
            questionItem.Checked = settings.RecordQuestion;
            questionItem.Tag = MediaItemType.Question;

            questionExampleItem = new ListViewItem(string.Format(Resources.LISTBOXFIELDS_EXQUESTION_TEXT, dictionaryManager.QuestionCaption));
            questionExampleItem.Checked = settings.RecordQuestionExample;
            questionExampleItem.Tag = MediaItemType.QuestionExample;

            answerItem = new ListViewItem(string.Format(Resources.LISTBOXFIELDS_ANSWER_TEXT, dictionaryManager.AnswerCaption));
            answerItem.Checked = settings.RecordAnswer;
            answerItem.Tag = MediaItemType.Answer;

            answerExampleItem = new ListViewItem(string.Format(Resources.LISTBOXFIELDS_EXANSWER_TEXT, dictionaryManager.AnswerCaption));
            answerExampleItem.Checked = settings.RecordAnswerExample;
            answerExampleItem.Tag = MediaItemType.AnswerExample;

            checkBoxDelayActive.Checked = settings.DelaysActive;
            checkBoxDelayActive.Focus();

            comboBoxKeyboardMode.SelectedIndex = (settings.KeyboardLayout ? 1 : 0);

            for (int i = 0; i < settings.RecordingOrder.Length; i++)
            {
                switch (settings.RecordingOrder[i])
                {
                    case MediaItemType.Question:
                        listViewElementsToRecord.Items.Add(questionItem);
                        break;
                    case MediaItemType.QuestionExample:
                        listViewElementsToRecord.Items.Add(questionExampleItem);
                        break;
                    case MediaItemType.Answer:
                        listViewElementsToRecord.Items.Add(answerItem);
                        break;
                    case MediaItemType.AnswerExample:
                        listViewElementsToRecord.Items.Add(answerExampleItem);
                        break;
                }
            }

            ////load cbr/vbr settings
            //checkBoxVBR.Checked = settings.MP3Settings.VBR_enabled;

            ////fill bitrates combobox and select current setting
            //comboBoxCBRBitrate.Items.Clear();
            //foreach (string bitrateString in Properties.Resources.ENCODING_AVAILABLE_BITRATES.Split('|'))
            //{
            //    Bitrate bitrate = new Bitrate(bitrateString);
            //    comboBoxCBRBitrate.Items.Add(bitrate);

            //    if (bitrate.Value == settings.MP3Settings.Bitrate)
            //        comboBoxCBRBitrate.SelectedItem = bitrate;
            //}

            ////fill vbr quality combobox
            //colorSliderVBRQuality.Value = 10 - settings.MP3Settings.VBR_Quality;
            //colorSliderVBRQuality_Scroll(colorSliderVBRQuality, new ScrollEventArgs(ScrollEventType.ThumbPosition, colorSliderVBRQuality.Value));

            //fill samplingrates combobox and select current setting
            comboBoxSamplingRate.Items.Clear();
            foreach (string samplingRateString in Resources.ENCODING_AVAILABLE_SAMPLING_RATES.Split('|'))
            {
                SamplingRate samplingRate = new SamplingRate(samplingRateString);
                comboBoxSamplingRate.Items.Add(samplingRate);

                if (samplingRate.Value == settings.SamplingRate)
                    comboBoxSamplingRate.SelectedItem = samplingRate;
            }

            //fill channels combobox and select current setting
            comboBoxChannels.Items.Clear();
            foreach (string channelString in Resources.ENCODING_AVAILABLE_CHANNELS.Split('|'))
                comboBoxChannels.Items.Add(int.Parse(channelString));
            comboBoxChannels.SelectedItem = settings.Channels;

            RefillEncoderCombobox();
            LoadCultures();
            loading = false;
        }

        /// <summary>
        /// Refills the encoder combobox.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        private void RefillEncoderCombobox()
        {
            //fill combobox
            comboBoxEncoder.Items.Clear();
            comboBoxEncoder.Items.Add(Resources.ENCODING_NONE);
            comboBoxEncoder.SelectedIndex = 0;
            Codecs.Codecs codecs = new MLifter.AudioTools.Codecs.Codecs();
            codecs.XMLString = settings.CodecSettings;

            foreach (Codec codec in codecs.encodeCodecs.Values)
                comboBoxEncoder.Items.Add(codec);

            //select item
            foreach (object item in comboBoxEncoder.Items)
            {
                if (item.ToString() == settings.SelectedEncoder)
                {
                    comboBoxEncoder.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Loads the cultures.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-11</remarks>
        private void LoadCultures()
        {
            comboBoxAnswerCulture.Items.Clear();
            comboBoxQuestionCulture.Items.Clear();

            comboBoxAnswerCulture.Items.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
            comboBoxAnswerCulture.SelectedItem = dictionaryManager.AnswerCulture;
            comboBoxQuestionCulture.Items.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
            comboBoxQuestionCulture.SelectedItem = dictionaryManager.QuestionCulture;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                ComboBox comboBox = (ComboBox)sender;
                if (tabPageNumPad.Controls.Contains(comboBox))
                    settings.KeyFunctions[(Keys)comboBox.Tag] = (Function)(comboBox.SelectedItem is string ? Function.Nothing : comboBox.SelectedItem);
                else if (tabPageKeyboard.Controls.Contains(comboBox))
                    settings.KeyboardFunctions[(Keys)comboBox.Tag] = (Function)(comboBox.SelectedItem is string ? Function.Nothing : comboBox.SelectedItem);

                numPadControl.ImagesValid = false;
                numPadControl.Refresh();
                numPadControl.Redraw();

                if (!checkBoxMultipleAssignment.Checked)
                    RemoveDoubleAssignemts();
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownStartDelay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        private void numericUpDownStartDelay_ValueChanged(object sender, EventArgs e) { }

        /// <summary>
        /// Handles the ValueChanged event of the numericUpDownStopDelay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        private void numericUpDownStopDelay_ValueChanged(object sender, EventArgs e) { }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (listViewElementsToRecord.CheckedIndices.Count < 1)
            {
                MessageBox.Show(Resources.NO_ITEM_TO_RECORD_TEXT, Resources.NO_ITEM_TO_RECORD_CAPTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!loading)
                {
                    settings.DelaysActive = checkBoxDelayActive.Checked;
                    settings.StopDelay = (int)numericUpDownStopDelay.Value;
                    settings.StartDelay = (int)numericUpDownStartDelay.Value;
                    settings.RecordAnswer = answerItem.Checked;
                    settings.RecordAnswerExample = answerExampleItem.Checked;
                    settings.RecordQuestion = questionItem.Checked;
                    settings.RecordQuestionExample = questionExampleItem.Checked;

                    dictionaryManager.AnswerCulture = comboBoxAnswerCulture.SelectedItem as CultureInfo;
                    dictionaryManager.QuestionCulture = comboBoxQuestionCulture.SelectedItem as CultureInfo;

                    settings.AllowMultipleAssignment = checkBoxMultipleAssignment.Checked;
                    settings.PresenterActivated = checkBoxPresenter.Checked;

                    for (int i = 0; i < listViewElementsToRecord.Items.Count; i++)
                        settings.RecordingOrder[i] = (MediaItemType)listViewElementsToRecord.Items[i].Tag;

                    //settings.MP3Settings.VBR_enabled = checkBoxVBR.Checked;
                    //if (settings.MP3Settings.VBR_enabled)
                    //{
                    //    settings.MP3Settings.Bitrate = (comboBoxCBRBitrate.Items[0] as Bitrate).Value;
                    //    settings.MP3Settings.VBR_maxBitrate = (comboBoxCBRBitrate.Items[comboBoxCBRBitrate.Items.Count - 1] as Bitrate).Value;
                    //    settings.MP3Settings.VBR_Quality = 10 - colorSliderVBRQuality.Value;
                    //}
                    //else
                    //    settings.MP3Settings.Bitrate = (comboBoxCBRBitrate.SelectedItem as Bitrate).Value;

                    if (comboBoxEncoder.SelectedItem != null)
                        settings.SelectedEncoder = comboBoxEncoder.SelectedItem.ToString();

                    settings.SamplingRate = (comboBoxSamplingRate.SelectedItem as SamplingRate).Value;
                    settings.Channels = (int)comboBoxChannels.SelectedItem;
                }

                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonDefault control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-07</remarks>
        private void buttonDefault_Click(object sender, EventArgs e)
        {
            //settings = new Settings(); //[MLR-1300] New instance causes settings not to be saved

            Settings defaultSettings = new Settings();
            settings.StartDelay = defaultSettings.StartDelay;
            settings.StopDelay = defaultSettings.StopDelay;
            settings.DelaysActive = defaultSettings.DelaysActive;

            settings.SelectedEncoder = defaultSettings.SelectedEncoder;
            settings.ShowEncoderWindow = defaultSettings.ShowEncoderWindow;
            settings.SamplingRate = defaultSettings.SamplingRate;
            settings.Channels = defaultSettings.Channels;

            settings.AllowMultipleAssignment = defaultSettings.AllowMultipleAssignment;

            settings.KeyFunctions.Clear();
            settings.KeyFunctions.Add(Keys.NumPad0, STANDART_FUNCTIONS.KEY_0);
            settings.KeyFunctions.Add(Keys.NumPad1, STANDART_FUNCTIONS.KEY_1);
            settings.KeyFunctions.Add(Keys.NumPad2, STANDART_FUNCTIONS.KEY_2);
            settings.KeyFunctions.Add(Keys.NumPad3, STANDART_FUNCTIONS.KEY_3);
            settings.KeyFunctions.Add(Keys.NumPad4, STANDART_FUNCTIONS.KEY_4);
            settings.KeyFunctions.Add(Keys.NumPad5, STANDART_FUNCTIONS.KEY_5);
            settings.KeyFunctions.Add(Keys.NumPad6, STANDART_FUNCTIONS.KEY_6);
            settings.KeyFunctions.Add(Keys.NumPad7, STANDART_FUNCTIONS.KEY_7);
            settings.KeyFunctions.Add(Keys.NumPad8, STANDART_FUNCTIONS.KEY_8);
            settings.KeyFunctions.Add(Keys.NumPad9, STANDART_FUNCTIONS.KEY_9);
            settings.KeyFunctions.Add(Keys.Decimal, STANDART_FUNCTIONS.KEY_COMMA);
            settings.KeyFunctions.Add(Keys.Enter, STANDART_FUNCTIONS.KEY_ENTER);
            settings.KeyFunctions.Add(Keys.Add, STANDART_FUNCTIONS.KEY_PLUS);
            settings.KeyFunctions.Add(Keys.Subtract, STANDART_FUNCTIONS.KEY_MINUS);
            settings.KeyFunctions.Add(Keys.Multiply, STANDART_FUNCTIONS.KEY_MULTIPLY);
            settings.KeyFunctions.Add(Keys.Divide, STANDART_FUNCTIONS.KEY_DIVIDE);

            settings.KeyboardFunctions.Clear();
            settings.KeyboardFunctions.Add(Keys.Space,STANDART_FUNCTIONS.KEY_SPACE);
            settings.KeyboardFunctions.Add(Keys.C, STANDART_FUNCTIONS.KEY_C);
            settings.KeyboardFunctions.Add(Keys.V, STANDART_FUNCTIONS.KEY_V);
            settings.KeyboardFunctions.Add(Keys.B, STANDART_FUNCTIONS.KEY_B);
            settings.KeyboardFunctions.Add(Keys.N, STANDART_FUNCTIONS.KEY_N);
            settings.KeyboardFunctions.Add(Keys.M, STANDART_FUNCTIONS.KEY_M);

            LoadValues();
            LoadCultures();
            numPadControl.ImagesValid = false;
            numPadControl.Refresh();
            numPadControl.Redraw();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxDelayActive control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-21</remarks>
        private void checkBoxDelayActive_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownStartDelay.Enabled = checkBoxDelayActive.Checked;
            labelStartDelay.Enabled = checkBoxDelayActive.Checked;
            labelStartDelayMS.Enabled = checkBoxDelayActive.Checked;
            numericUpDownStopDelay.Enabled = checkBoxDelayActive.Checked;
            labelStopDelay.Enabled = checkBoxDelayActive.Checked;
            labelStopDelayMS.Enabled = checkBoxDelayActive.Checked;
        }

        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-21</remarks>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxMultipleAssignment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-21</remarks>
        private void checkBoxMultipleAssignment_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxMultipleAssignment.Checked)
            {
                Keys key = RemoveDoubleAssignemts();
                loading = false;

                if (key != Keys.FinalMode)
                {
                    List<Keys> assignetKeys = new List<Keys>();

                    if (settings.KeyFunctions.ContainsKey(key))
                        foreach (KeyValuePair<Keys, Function> pair in settings.KeyFunctions)
                        {
                            if (pair.Value == settings.KeyFunctions[key])
                                assignetKeys.Add(pair.Key);
                        }
                    else
                        foreach (KeyValuePair<Keys, Function> pair in settings.KeyboardFunctions)
                        {
                            if (pair.Value == settings.KeyboardFunctions[key])
                                assignetKeys.Add(pair.Key);
                        }

                    string text = (settings.KeyFunctions.ContainsKey(key) ? settings.KeyFunctions[key] : settings.KeyboardFunctions[key])
                        + Resources.MULTIPLE_KEY_ASSIGNMENT_ERROR_TEXT + "\n\r";

                    foreach (Keys assignetKey in assignetKeys)
                        text += "  * " + assignetKey + "\n\r";

                    MessageBox.Show(text, Resources.MULTIPLE_KEY_ASSIGNMENT_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    checkBoxMultipleAssignment.Checked = true;
                }
            }
            else
            {
                foreach (Control control in tabPageNumPad.Controls)
                {
                    if (control.GetType() == typeof(ComboBox))
                    {
                        ComboBox comboBox = (ComboBox)control;

                        comboBox.Items.Clear();
                        comboBox.Items.Add("");
                        comboBox.Items.Add(Function.Backward);
                        comboBox.Items.Add(Function.Forward);
                        comboBox.Items.Add(Function.Play);
                        comboBox.Items.Add(Function.Record);

                        try
                        {
                            comboBox.SelectedItem = settings.KeyFunctions[(Keys)comboBox.Tag];
                        }
                        catch (NullReferenceException)
                        {
                            comboBox.SelectedIndex = 0;
                        }
                    }
                }
                foreach (Control control in tabPageKeyboard.Controls)
                {
                    if (control.GetType() == typeof(ComboBox))
                    {
                        ComboBox comboBox = (ComboBox)control;

                        comboBox.Items.Clear();
                        comboBox.Items.Add("");
                        comboBox.Items.Add(Function.Backward);
                        comboBox.Items.Add(Function.Forward);
                        comboBox.Items.Add(Function.Play);
                        comboBox.Items.Add(Function.Record);

                        try
                        {
                            comboBox.SelectedItem = settings.KeyboardFunctions[(Keys)comboBox.Tag];
                        }
                        catch (KeyNotFoundException)
                        {
                            comboBox.SelectedIndex = 0;
                        }
                        catch (NullReferenceException)
                        {
                            comboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the double assignemts.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-21</remarks>
        private Keys RemoveDoubleAssignemts()
        {
            loading = true;

            List<Function> availableFunctions = new List<Function>();
            availableFunctions.Add(Function.Backward);
            availableFunctions.Add(Function.Forward);
            availableFunctions.Add(Function.Play);
            availableFunctions.Add(Function.Record);

            foreach (Control control in tabPageNumPad.Controls)
            {
                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox comboBox = (ComboBox)control;

                    if (comboBox.SelectedItem is Function && availableFunctions.Contains((Function)comboBox.SelectedItem))
                        availableFunctions.Remove((Function)comboBox.SelectedItem);
                    else if (comboBox.SelectedItem is Function)
                        return (Keys)comboBox.Tag;
                }
            }

            foreach (Control control in tabPageNumPad.Controls)
            {
                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox comboBox = (ComboBox)control;

                    Function actualFunction = Function.Nothing;
                    if (comboBox.SelectedItem is Function)
                        actualFunction = (Function)comboBox.SelectedItem;

                    comboBox.Items.Clear();

                    if (actualFunction != Function.Nothing)
                        comboBox.Items.Add(actualFunction);

                    comboBox.Items.Add("");

                    foreach (Function function in availableFunctions)
                        comboBox.Items.Add(function);

                    comboBox.SelectedIndex = 0;
                }
            }

            availableFunctions = new List<Function>();
            availableFunctions.Add(Function.Backward);
            availableFunctions.Add(Function.Forward);
            availableFunctions.Add(Function.Play);
            availableFunctions.Add(Function.Record);

            foreach (Control control in tabPageKeyboard.Controls)
            {
                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox comboBox = (ComboBox)control;

                    if (comboBox.SelectedItem is Function && availableFunctions.Contains((Function)comboBox.SelectedItem))
                        availableFunctions.Remove((Function)comboBox.SelectedItem);
                    else if (comboBox.SelectedItem is Function)
                        return (Keys)comboBox.Tag;
                }
            }

            foreach (Control control in tabPageKeyboard.Controls)
            {
                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox comboBox = (ComboBox)control;

                    Function actualFunction = Function.Nothing;
                    if (comboBox.SelectedItem is Function)
                        actualFunction = (Function)comboBox.SelectedItem;

                    comboBox.Items.Clear();

                    if (actualFunction != Function.Nothing)
                        comboBox.Items.Add(actualFunction);

                    comboBox.Items.Add("");

                    foreach (Function function in availableFunctions)
                        comboBox.Items.Add(function);

                    comboBox.SelectedIndex = 0;
                }
            }

            loading = false;

            return Keys.FinalMode;
        }

        /// <summary>
        /// Handles the FormClosing event of the SettingsForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-11</remarks>
        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
                return;

            settings.KeyFunctions.Clear();
            settings.KeyboardFunctions.Clear();

            foreach (KeyValuePair<Keys, Function> pair in backupFunctions)
                settings.KeyFunctions.Add(pair.Key, pair.Value);

            foreach (KeyValuePair<Keys, Function> pair in backupKeyboardFunctions)
                settings.KeyboardFunctions.Add(pair.Key, pair.Value);

            settings.KeyboardLayout = backupKeyboardMode;
            if (settings.AdvancedView)
                numPadControl.KeyboardLayout = backupKeyboardMode;

            numPadControl.Refresh();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxKeyboardMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-12</remarks>
        private void comboBoxKeyboardMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.KeyboardLayout = (comboBoxKeyboardMode.SelectedIndex == 1);
            if (settings.AdvancedView)
                numPadControl.KeyboardLayout = settings.KeyboardLayout;
            numPadControl.Refresh();
        }

        /// <summary>
        /// Handles the Click event of the buttonCodecSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        private void buttonCodecSettings_Click(object sender, EventArgs e)
        {
            if (ShowCodecSettings(settings) == DialogResult.OK)
                RefillEncoderCombobox();
        }

        /// <summary>
        /// Shows the codec settings.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public static DialogResult ShowCodecSettings(Settings settings)
        {
            Codecs.Codecs codecs = new MLifter.AudioTools.Codecs.Codecs();
            codecs.XMLString = settings.CodecSettings;

            CodecSettings codecSettings = new CodecSettings();
            codecSettings.Codecs = codecs;
            codecSettings.EnableDecodeSettings = false;
            codecSettings.ShowEncoder = settings.ShowEncoderWindow;
            codecSettings.MinimizeWindows = settings.MinimizeEncoderWindow;

            DialogResult result = codecSettings.ShowDialog();

            if (result == DialogResult.OK)
            {
                settings.CodecSettings = codecSettings.Codecs.XMLString;
                settings.ShowEncoderWindow = codecSettings.ShowEncoder;
                settings.MinimizeEncoderWindow = codecSettings.MinimizeWindows;
            }

            return result;
        }

        ///// <summary>
        ///// The estimated average bitrate of the LAME VBR Quality settings
        ///// taken from http://www.hydrogenaudio.org/forums/index.php?showtopic=28124&st=0&p=1595&#entry1595
        ///// </summary>
        //private int[] LAMEQuality ={ 245, 225, 190, 175, 165, 130, 115, 100, 85, 65 };

        ///// <summary>
        ///// Updates the estimated average filesize.
        ///// </summary>
        ///// <remarks>Documented by Dev02, 2008-01-02</remarks>
        //private void UpdateEstimated()
        //{
        //    int CBRBitrate = 0;
        //    if (comboBoxCBRBitrate.SelectedIndex >= 0)
        //        CBRBitrate = Convert.ToInt32((comboBoxCBRBitrate.SelectedItem as Bitrate).Value);
        //    if (checkBoxVBR.Checked)
        //        CBRBitrate = LAMEQuality[10 - colorSliderVBRQuality.Value];

        //    double mbPerMin = 1.0 * CBRBitrate / 8 * 60 / 1000;
        //    labelExpectedValue.Text = string.Format(Properties.Resources.ENCODING_EXPECTED_STRINGFORMAT, mbPerMin);
        //}
    }

    ///// <summary>
    ///// This class defines a mp3 encoding bitrate.
    ///// </summary>
    ///// <remarks>Documented by Dev02, 2008-01-02</remarks>
    //public class Bitrate
    //{
    //    private uint bitrate;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="Bitrate"/> class.
    //    /// </summary>
    //    /// <remarks>Documented by Dev02, 2008-01-02</remarks>
    //    public Bitrate()
    //    { }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="Bitrate"/> class.
    //    /// </summary>
    //    /// <param name="bitrate">The bitrate.</param>
    //    /// <remarks>Documented by Dev02, 2008-01-02</remarks>
    //    public Bitrate(uint bitrate)
    //    {
    //        this.bitrate = bitrate;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="Bitrate"/> class.
    //    /// </summary>
    //    /// <param name="bitrate">The bitrate.</param>
    //    /// <remarks>Documented by Dev02, 2008-01-02</remarks>
    //    public Bitrate(string bitrate)
    //    {
    //        this.bitrate = uint.Parse(bitrate);
    //    }

    //    /// <summary>
    //    /// Gets or sets the bitrate.
    //    /// </summary>
    //    /// <value>The bitrate.</value>
    //    /// <remarks>Documented by Dev02, 2008-01-02</remarks>
    //    public uint Value
    //    {
    //        get
    //        {
    //            return bitrate;
    //        }
    //        set
    //        {
    //            if (value > 0) bitrate = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    //    /// </summary>
    //    /// <returns>
    //    /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    //    /// </returns>
    //    /// <remarks>Documented by Dev02, 2008-01-02</remarks>
    //    public override string ToString()
    //    {
    //        string bitrate = string.Format(Properties.Resources.ENCODING_BITRATE_TOSTRINGFORMAT, this.bitrate);
    //        return bitrate;
    //    }
    //}

    /// <summary>
    /// This class defines a mp3 encoding sampling rate.
    /// </summary>
    /// <remarks>Documented by Dev02, 2007-12-21</remarks>
    public class SamplingRate
    {
        private int frequency;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplingRate"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-21</remarks>
        public SamplingRate()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplingRate"/> class.
        /// </summary>
        /// <param name="frequency">The frequency in Hz.</param>
        /// <remarks>Documented by Dev02, 2007-12-21</remarks>
        public SamplingRate(int frequency)
        {
            this.frequency = frequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplingRate"/> class.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <remarks>Documented by Dev02, 2007-12-21</remarks>
        public SamplingRate(string frequency)
        {
            this.frequency = int.Parse(frequency);
        }

        /// <summary>
        /// Gets or sets the sampling rate.
        /// </summary>
        /// <value>The sampling rate.</value>
        /// <remarks>Documented by Dev02, 2007-12-21</remarks>
        public int Value
        {
            get
            {
                return frequency;
            }
            set
            {
                if (value > 0) frequency = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2007-12-21</remarks>
        public override string ToString()
        {
            string samplingrate = string.Format(Resources.ENCODING_SAMPLINGRATE_TOSTRINGFORMAT, 1.0 * frequency / 1000);
            return samplingrate;
        }
    }
}
