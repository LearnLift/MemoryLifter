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
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using MLifter.Controls;
using MLifter.DAL;
using MLifter.DAL.ImportExport;
using MLifter.Properties;

namespace MLifter.ImportExport
{
    public partial class ExportForm : Form
    {
        LoadStatusMessage statusDialog;

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportForm"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        public ExportForm()
        {
            InitializeComponent();
            MLifter.Classes.Help.SetHelpNameSpace(mainHelp);
        }

        /// <summary>
        /// Handles the Load event of the ExportForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void ExportForm_Load(object sender, EventArgs e)
        {
            exportFieldsFrm.PrepareChapter(Dictionary);
            exportChapterFrm.Prepare(Dictionary);

            cmbFileFormat.Items.Add(Resources.FILETYPE_COMMA);
            cmbFileFormat.Items.Add(Resources.FILETYPE_SEMICOLON);
            cmbFileFormat.Items.Add(Resources.FILETYPE_TAB);
            cmbFileFormat.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the Click event of the btnExport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportDictionary();
        }

        /// <summary>
        /// Exports the dictionary.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void ExportDictionary()
        {
            if (exportChapterFrm.Total == 0 || exportFieldsFrm.StrOrder.Length == 0)
            {
                MessageBox.Show(Resources.EXPORTER_NOTHING_SELECTED_TEXT, Resources.EXPORTER_NOTHING_SELECTED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog dlgExportFile = new SaveFileDialog();
            dlgExportFile.Title = Resources.EXPORT_SAVEDIALOG_TITLE;
            dlgExportFile.Filter = Resources.EXPORT_FILETYPE;
            dlgExportFile.AddExtension = true;
            dlgExportFile.OverwritePrompt = true;
            dlgExportFile.CreatePrompt = false;
            dlgExportFile.CheckPathExists = true;

            if (dlgExportFile.ShowDialog() == DialogResult.OK)
            {
                string separator;
                switch (cmbFileFormat.SelectedIndex)
                {
                    case 1:
                        separator = ";";
                        break;
                    case 2:
                        separator = "\t";
                        break;
                    case 0:
                    default:
                        separator = ",";
                        break;
                }

                string chapters = separator;
                foreach (int chapter in exportChapterFrm.SelChapters)
                    chapters += chapter + separator;

                # region header and fields
                string headerline = "";
                bool containsFiles = false;

                XmlDocument document = new XmlDocument();
                document.LoadXml("<fieldsToExport></fieldsToExport>");
                for (int i = 0; i < exportFieldsFrm.StrOrder.Length; i++)
                {
                    headerline += exportFieldsFrm.StrOrder[i];
                    headerline += separator;

                    string caption = exportFieldsFrm.StrOrder[i];
                    XmlNode node = document.CreateNode(XmlNodeType.Element, "fieldName", null);

					if (isSameField(caption,Resources.LISTBOXFIELDS_QUESTION_DISTRACTORS_TEXT))
					{
						node.InnerText += "questiondistractors";
					}
					else if (isSameField(caption,Resources.LISTBOXFIELDS_ANSWER_DISTRACTORS_TEXT))
					{
						node.InnerText += "answerdistractors";
					}
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_ANSWER_TEXT))
                        node.InnerText += "answer";
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_EXANSWER_TEXT))
                        node.InnerText += "answerexample";
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_QUESTION_TEXT))
                        node.InnerText += "question";
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_EXQUESTION_TEXT))
                        node.InnerText += "questionexample";
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_IMAGE_ANSWER_TEXT))
                    {
                        node.InnerText += "answerimage";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_IMAGE_QUESTION_TEXT))
                    {
                        node.InnerText += "questionimage";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_SOUND_ANSWER_TEXT))
                    {
                        node.InnerText += "answeraudio";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_SOUND_EXANSWER_TEXT))
                    {
                        node.InnerText += "answerexampleaudio";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_SOUND_EXQUESTION_TEXT))
                    {
                        node.InnerText += "questionexampleaudio";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_SOUND_QUESTION_TEXT))
                    {
                        node.InnerText += "questionaudio";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_VIDEO_ANSWER_TEXT))
                    {
                        node.InnerText += "answervideo";
                        containsFiles = true;
                    }
                    else if (isSameField(caption,Resources.LISTBOXFIELDS_VIDEO_QUESTION_TEXT))
                    {
                        node.InnerText += "questionvideo";
                        containsFiles = true;
                    }
                    else if (caption == Resources.LISTBOXFIELDS_CHAPTER)
                        node.InnerText += "chapter";

                    document.DocumentElement.AppendChild(node);
                }
                headerline = headerline.Remove(headerline.LastIndexOf(separator));
                # endregion

                bool copyFiles = false;
                if (containsFiles)
                {
                    copyFiles = (0 == MLifter.Controls.TaskDialog.ShowCommandBox(Resources.EXPORTER_COPY_MEDIA_TITLE, Resources.EXPORTER_COPY_MEDIA_TEXT,
                        Resources.EXPORTER_COPY_MEDIA_CONTENT, string.Empty, string.Empty, string.Empty, Properties.Resources.EXPORTER_COPY_MEDIA_COMMANDS, false,
                        MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Question));
                }

                statusDialog = new LoadStatusMessage(Properties.Resources.EXPORT_STATUS_CAPTION, 100, true);
                statusDialog.Show();

                bool exceptionhappened = false;
                try
                {
                    Exporter exporter = new Exporter();
                    exporter.ProgressChanged += new EventHandler<MLifter.DAL.Tools.StatusMessageEventArgs>(exporter_ProgressChanged);
                    exporter.ExportToCSV(dlgExportFile.FileName, Path.Combine(Application.StartupPath, @"Designs\System\Export\default.xsl"),
                        MainForm.LearnLogic.Dictionary.DictionaryDAL, separator, chapters, headerline, copyFiles, document);
                }
                catch (IOException exp)
                {
                    exceptionhappened = true;
                    MessageBox.Show(Properties.Resources.DIC_ERROR_SAVING_TEXT, Properties.Resources.DIC_ERROR_SAVING_CAPTION);
                    System.Diagnostics.Trace.WriteLine("Export file save exception: " + exp.ToString());
                }

                statusDialog.Hide();

                if (!exceptionhappened)
                    Close();
            }
        }

        /// <summary>
        /// Determines whether two strings are equal, where the second string gets a replacement with the 
        /// corresponding caption of the dictionary (question- and answercaption).
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="resourceString">The resource string.</param>
        /// <returns>
        /// 	<c>true</c> if they match; otherwise, <c>false</c>.
        /// </returns>
        /// Documented by MatBre,  07.09.2009
        private Boolean isSameField(string field, string resourceString)
        {
            string caption;

            if (resourceString.Equals(Resources.LISTBOXFIELDS_QUESTION_DISTRACTORS_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_QUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_EXQUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_IMAGE_QUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_EXQUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_QUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_VIDEO_QUESTION_TEXT))
                caption = MainForm.LearnLogic.Dictionary.QuestionCaption;
            else if (resourceString.Equals(Resources.LISTBOXFIELDS_ANSWER_DISTRACTORS_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_ANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_EXANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_IMAGE_ANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_ANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_EXANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_VIDEO_ANSWER_TEXT))
                caption = MainForm.LearnLogic.Dictionary.AnswerCaption;
            else
                caption = "";

            string finalResourceString = String.Format(resourceString, caption);

            return field.Equals(finalResourceString);
        }

        /// <summary>
        /// Handles the ProgressChanged event of the exporter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        void exporter_ProgressChanged(object sender, MLifter.DAL.Tools.StatusMessageEventArgs e)
        {
            statusDialog.SetProgress(e.ProgressPercentage);
        }

        /// <summary>
        /// Handles the ProgressChanged event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusDialog.SetProgress(e.ProgressPercentage);
        }
    }
}
