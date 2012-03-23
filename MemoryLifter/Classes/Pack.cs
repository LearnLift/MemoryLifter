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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using MLifter.Properties;
using MLifter.Controls;

namespace MLifter
{
    internal class Pack
    {
        internal LoadStatusMessage m_LoadMsg = new LoadStatusMessage(String.Empty, 100, true);

        private SaveFileDialog SaveDialog = new SaveFileDialog();
        private FolderBrowserDialog DirDialog = new FolderBrowserDialog();

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Constructor of the Pack Object
        /// No Unit Test necessary
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        internal Pack()
        {
            this.SaveDialog.DefaultExt = DAL.Pack.Packer.Extension.Substring(1);
            this.SaveDialog.Filter = String.Format(Resources.PACK_SAVEDIALOG_FILTER, Settings.Default.FILE_SupportedPackFormats);
            this.SaveDialog.Title = Resources.PACK_SAVEDIALOG_TITLE;
            this.DirDialog.ShowNewFolderButton = true;
            this.DirDialog.Description = Resources.PACK_DIRDIALOG_DESCRIPTION;
        }

        /// <summary>
        /// This method packs arrayList dictionary in arrayList dzp file (ZIP).
        /// It just collects all Media-files from the root-directory (= directory where the odx-file is located)
        /// and all Media-files from the sub-folders.
        /// Unit Test necessary
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        internal void PackDic()
        {
            SaveDialog.Title = Resources.PACKDIC_SAVEDIALOG_TITLE;
            SaveDialog.FileName = Path.GetFileNameWithoutExtension(Dictionary.DictionaryPath) + DAL.Pack.Packer.Extension;
            if (SaveDialog.ShowDialog() != DialogResult.OK)
                return;

            //m_LoadMsg = new LoadStatusMessage(Resources.PACK_TLOADMSG, 100, true);
            m_LoadMsg.InfoMessage = Resources.PACK_TLOADMSG;
            m_LoadMsg.SetProgress(0);
            Cursor.Current = Cursors.WaitCursor;
            m_LoadMsg.Show();

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(Zip_ProgressChanged);
            try
            {
                MLifter.DAL.Pack.Packer packer = new MLifter.DAL.Pack.Packer(backgroundWorker);
                string packedDict = packer.Pack(Dictionary.DictionaryDAL, SaveDialog.FileName);
            }
            catch (IOException)
            {
                MessageBox.Show(Properties.Resources.PACK_IOEXCEPTION_TEXT, Properties.Resources.PACK_IOEXCEPTION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                m_LoadMsg.Hide();
            }
        }

        /// <summary>
        /// This method unpacks arrayList dictionary in arrayList dzp file (ZIP).
        /// Unit Test necessary
        /// </summary>
        /// <param name="filename">name of file to extract</param>
        /// <returns>The dictionary path.</returns>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        internal string UnpackDic(string filename)
        {
            DialogResult dialogResult;

            bool ignoreInvalidHeader = false;
            string dictionaryPath = String.Empty;
            DAL.Interfaces.IDictionary dictionary = null;
            string fileNameWithoutExtension = String.Empty;

            if (!DAL.Pack.Packer.IsValidArchive(filename))
            {
                TaskDialog.ShowTaskDialogBox(Resources.UNPACK_NOT_ORIGINAL_CAPTION, Resources.UNPACK_NOT_ORIGINAL_TEXT, Resources.UNPACK_NOT_ORIGINAL_CONTENT,
                    string.Empty, string.Empty, string.Empty, string.Empty, Resources.UNPACK_NOT_ORIGINAL_OPTION_YES + "|" + Resources.UNPACK_NOT_ORIGINAL_OPTION_NO, TaskDialogButtons.None,
                    TaskDialogIcons.Question, TaskDialogIcons.Information);
                switch (TaskDialog.CommandButtonResult)
                {
                    case 1:
                    default:
                        return String.Empty;
                    case 0:
                        ignoreInvalidHeader = true;
                        break;
                }
            }

            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            dictionaryPath = GenerateDictionaryPath(filename);

            dialogResult = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.UNPACK_DEFAULT_MBX_CAPTION,
                 Resources.UNPACK_DEFAULT_MBX_TEXT, String.Format(Resources.UNPACK_DEFAULT_MBX_CONTENT, dictionaryPath),
                 String.Empty, String.Empty, String.Empty, String.Empty,
                 String.Format("{0}|{1}|{2}", Resources.UNPACK_DEFAULT_OPTION_YES, Resources.UNPACK_DEFAULT_MBX_OPTION_NO, Resources.UNPACK_DEFAULT_MBX_OPTION_CANCEL),
                 MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);
            switch (MLifter.Controls.TaskDialog.CommandButtonResult)
            {
                case 0:
                    break;
                case 1:
                    if (Directory.Exists(dictionaryPath))
                        DirDialog.SelectedPath = dictionaryPath;
                    else
                        DirDialog.SelectedPath = Settings.Default.DicDir;
                    if (DirDialog.ShowDialog(m_LoadMsg) == DialogResult.OK)
                        dictionaryPath = DirDialog.SelectedPath;
                    else
                        return String.Empty;
                    break;
                case 2:
                default:
                    return String.Empty;
            }

            try
            {
                if (!Directory.Exists(dictionaryPath))
                {
                    Directory.CreateDirectory(dictionaryPath);
                }
            }
            catch
            {
                dictionaryPath = Settings.Default.DicDir;
            }

            bool ignore = false;
            bool pathExists = false;
            do
            {
                if (File.Exists(Path.Combine(dictionaryPath, fileNameWithoutExtension + DAL.Helper.OdxExtension)))
                {
                    pathExists = true;

                    dialogResult = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.UNPACK_DICTIONARY_EXISTS_MBX_CAPTION,
                         Resources.UNPACK_REPLACE_FILES_MBX_TEXT, Resources.UNPACK_DICTIONARY_EXISTS_MBX_CONTENT,
                         String.Empty, String.Empty, String.Empty, String.Empty,
                         String.Format("{0}|{1}|{2}", Resources.UNPACK_DICTIONARY_EXISTS_MBX_OPTION_YES, Resources.UNPACK_DICTIONARY_EXISTS_MBX_OPTION_NO, Resources.UNPACK_DICTIONARY_EXISTS_MBX_OPTION_CANCEL),
                         MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);
                    switch (MLifter.Controls.TaskDialog.CommandButtonResult)
                    {
                        case 0:
                            ignore = true;
                            break;
                        case 2:
                            DirDialog.SelectedPath = dictionaryPath;
                            if (DirDialog.ShowDialog(m_LoadMsg) == DialogResult.OK)
                                dictionaryPath = DirDialog.SelectedPath;
                            else
                                return String.Empty;
                            continue;
                        case 1:
                        default:
                            return String.Empty;
                    }
                }
                else if (Directory.GetFiles(dictionaryPath, "*" + DAL.Helper.OdxExtension).Length > 0)
                {
                    pathExists = true;

                    dialogResult = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.UNPACK_DICTIONARY_FOLDER_EXISTS_MBX_CAPTION,
                        Resources.UNPACK_DICTIONARY_FOLDER_EXISTS_MBX_TEXT, Resources.UNPACK_DICTIONARY_FOLDER_EXISTS_MBX_CONTENT,
                        String.Empty, String.Empty, String.Empty, String.Empty,
                        String.Format("{0}|{1}|{2}", Resources.UNPACK_DICTIONARY_FOLDER_EXISTS_MBX_OPTION_YES, Resources.UNPACK_DICTIONARY_FOLDER_EXISTS_MBX_OPTION_NO, Resources.UNPACK_DICTIONARY_FOLDER_EXISTS_MBX_OPTION_CANCEL),
                        MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Warning, MLifter.Controls.TaskDialogIcons.Warning);
                    switch (MLifter.Controls.TaskDialog.CommandButtonResult)
                    {
                        case 0:
                            ignore = true;
                            break;
                        case 2:
                            DirDialog.SelectedPath = dictionaryPath;
                            if (DirDialog.ShowDialog(m_LoadMsg) == DialogResult.OK)
                                dictionaryPath = DirDialog.SelectedPath;
                            else
                                return String.Empty;
                            break;
                        case 1:
                        default:
                            return String.Empty;
                    }
                }
                else
                {
                    pathExists = false;
                }
            }
            while (pathExists && !ignore);

            m_LoadMsg.InfoMessage = Resources.UNPACK_TLOADMSG;
            m_LoadMsg.SetProgress(0);
            Cursor.Current = Cursors.WaitCursor;
            m_LoadMsg.Show();
            try
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(Unzip_ProgressChanged);
                MLifter.DAL.Pack.Packer packer = new MLifter.DAL.Pack.Packer(backgroundWorker);
                packer.TemporaryFolder = dictionaryPath;
                packer.LoginCallback = (MLifter.DAL.GetLoginInformation)LoginForm.OpenLoginForm;
                dictionary = packer.Unpack(filename, ignoreInvalidHeader);
                //dictionaryPath = Path.Combine(dictionaryPath, fileNameWithoutExtension + DAL.DictionaryFactory.OdxExtension);
                dictionaryPath = dictionary.Connection;
            }
            catch (DAL.NoValidArchiveHeaderException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename), Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return String.Empty;
            }
            catch (DAL.NoValidArchiveException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename), Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return String.Empty;
            }
            catch (System.Xml.XmlException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename), Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return String.Empty;
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename), Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return String.Empty;
            }
            catch (ICSharpCode.SharpZipLib.Zip.ZipException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename), Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return String.Empty;
            }
            catch (MLifter.DAL.DictionaryFormatNotSupported ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(Properties.Resources.DIC_ERROR_NEWERVERSION_TEXT, Properties.Resources.DIC_ERROR_NEWERVERSION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return String.Empty;
            }
            catch (MLifter.DAL.DictionaryNotDecryptedException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(Properties.Resources.DIC_ERROR_NOT_DECRYPTED_TEXT, Properties.Resources.DIC_ERROR_NOT_DECRYPTED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return String.Empty;
            }
            finally
            {
                if (dictionary != null) dictionary.Dispose();
                Cursor.Current = Cursors.Default;
                m_LoadMsg.Hide();
            }

            return dictionaryPath;
        }

        /// <summary>
        /// Generates the dictionary path.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The dictionary path.</returns>
        /// <remarks>Documented by Dev02, 2007-12-10</remarks>
        private static string GenerateDictionaryPath(string filename)
        {
            string dictionaryPath = Path.Combine(Settings.Default.DicDir, Path.GetFileNameWithoutExtension(filename));
            return dictionaryPath;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the Zip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="progressArgs">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-10</remarks>
        private void Zip_ProgressChanged(object sender, ProgressChangedEventArgs progressArgs)
        {
            m_LoadMsg.SetProgress(progressArgs.ProgressPercentage);
        }

        /// <summary>
        /// Handles the ProgressChanged event of the Unzip control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="progressArgs">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-10</remarks>
        private void Unzip_ProgressChanged(object sender, ProgressChangedEventArgs progressArgs)
        {
            m_LoadMsg.SetProgress(progressArgs.ProgressPercentage);
        }

        /// <summary>
        /// Handles the ProgressChanged event of the Move control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="progressArgs">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-10</remarks>
        private void Move_ProgressChanged(object sender, ProgressChangedEventArgs progressArgs)
        {
            m_LoadMsg.SetProgress(progressArgs.ProgressPercentage);
        }
    }
}

