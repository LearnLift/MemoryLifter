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
using MLifter.DAL.Interfaces;
using MLifter.DAL;
using MLifter.Controls;
using System.Windows;
using System.Threading;
using System.IO;
using MLifter.DAL.XML;
using System.Windows.Documents;
using MLifter.SettingsManager.Properties;

namespace MLifterSettingsManager
{
    public class SettingsManagerBusinessLogic
    {
        #region Properties

        public IDictionary LearningModule { get; set; }

        public event EventHandler LearningModuleOpened;

        public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);
        public event ExceptionEventHandler LearningModuleException;

        public delegate void LoadingEventHandler(object sender, LoadingEventArgs e);
        public event LoadingEventHandler LoadingIPhone;

        public event EventHandler FinishLoadingIPhone;

        #endregion

        #region Private Fields

        private IUser learningModuleUser = null;

        /// <summary>
        /// You don't no what a SynchronizationContext is? See: http://www.codeproject.com/KB/cpp/SyncContextTutorial.aspx
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private SynchronizationContext context;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManagerBusinessLogic"/> class.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public SettingsManagerBusinessLogic()
        {
            context = SynchronizationContext.Current;
            if (context == null)
                context = new SynchronizationContext();
        }

        /// <summary>
        /// Opens the learning module.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void OpenLearningModule(string filename)
        {
            string globalConfig = System.IO.Path.Combine(Application.Current.StartupUri.AbsolutePath, Settings.Default.ConfigPath);
            string userConfig = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.ConfigurationFolder);
            string syncedModulesPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
				System.IO.Path.Combine(Settings.Default.AppDataFolder, Settings.Default.SyncedLMFolder));

            ThreadStart threadStart = new ThreadStart(delegate()
            {
                try
                {
                    CloseLearningModule();

                    MLifter.BusinessLayer.LearningModulesIndex index = new MLifter.BusinessLayer.LearningModulesIndex(globalConfig, userConfig, LoginForm.OpenLoginForm, delegate { return; }, string.Empty);
                    learningModuleUser = null;

                    learningModuleUser = UserFactory.Create(LoginForm.OpenLoginForm, new ConnectionStringStruct(DatabaseType.MsSqlCe, filename, -1), delegate { return; }, this);
                    ConnectionStringStruct css = learningModuleUser.ConnectionString;
                    css.LmId = MLifter.DAL.User.GetIdOfLearningModule(filename, learningModuleUser);
                    learningModuleUser.ConnectionString = css;
                    LearningModule = learningModuleUser.Open();
                }
                catch (Exception e)
                {
                    context.Post(new SendOrPostCallback(delegate(object state)
                    {
                        OnLearningModuleException(this, new ExceptionEventArgs(e, filename));
                    }), null);

                    return;
                }

                context.Post(new SendOrPostCallback(delegate(object state)
                {
                    OnLearningModuleOpened(this, EventArgs.Empty);
                }), null);
            });
            new Thread(threadStart) { Name = "Open LearningModule Thread" }.Start();
        }

        /// <summary>
        /// Updates the learning module.
        /// </summary>
        /// <param name="databaseVersion">The database version.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public bool UpdateLearningModule(Version databaseVersion)
        {
            if (learningModuleUser == null)
                return false;

            bool result = learningModuleUser.Database.UpgradeDatabase(databaseVersion);
            LearningModule = learningModuleUser.Open();

            return result;
        }

        /// <summary>
        /// Closes the learning module.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void CloseLearningModule()
        {
            if (LearningModule != null)
            {
                LearningModule.Parent.CurrentUser.Logout();
                LearningModule.Dispose();
                LearningModule = null;
            }
        }

        /// <summary>
        /// Clears the unused learning module.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void ClearUnusedMedia()
        {
            if (LearningModule == null)
                return;

            LearningModule.ClearUnusedMedia();
        }

        /// <summary>
        /// Converts to XML format.
        /// </summary>
        /// <param name="destinationFileName">Name of the destination file.</param>
        /// <param name="cardIdsNotToExport">A list of card ids that should not be exported.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public void ConvertToXmlFormat(string destinationFileName, List<int> cardIdsNotToExport)
        {
            ThreadStart threadStart = delegate()
            {
                //create new directory
                string fileName = System.IO.Path.GetFileNameWithoutExtension(destinationFileName);
                string filePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(destinationFileName), fileName);//learningModule.Title);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                //export for odx format
                XmlDictionaries dic = new XmlDictionaries(filePath);
                IDictionary export = dic.AddNew(LearningModule.Category.Id, fileName);

                CardsHelper.CardIdsNotToCopy = cardIdsNotToExport;

                LearningModule.CopyTo(export, ConvertingToXml);

                CardsHelper.CardIdsNotToCopy = null;

                export.Save();

                //delete empty chapters
                List<int> chapterIds = new List<int>();
                foreach (IChapter ch in export.Chapters.Chapters)
                    if (ch.Size == 0)
                        chapterIds.Add(ch.Id);
                foreach (int chapterId in chapterIds)
                    if (export.Chapters.Get(chapterId) != null)
                        export.Chapters.Delete(chapterId);
                export.Save();

                OnFinishedConvertingToXml(this, EventArgs.Empty);
            };

            new Thread(threadStart) { Name = "Convert To $export Thread", IsBackground = true }.Start();
        }

        /// <summary>
        /// Converting to XML.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        /// <param name="currentPercentage">The current percentage.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void ConvertingToXml(string statusMessage, double currentPercentage)
        {
            OnConvertingToXml(this, new LoadingEventArgs(statusMessage, Convert.ToInt32(currentPercentage), 100));
        }

        /// <summary>
        /// Determines whether [is empty setting] [the specified setting].
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>
        /// 	<c>true</c> if [is empty setting] [the specified setting]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        public static bool IsEmptySetting(ISettings setting)
        {
            return !setting.QueryTypes.ImageRecognition.HasValue
                && !setting.QueryTypes.ListeningComprehension.HasValue
                && !setting.QueryTypes.MultipleChoice.HasValue
                && !setting.QueryTypes.Sentence.HasValue
                && !setting.QueryTypes.Word.HasValue
                && !setting.MultipleChoiceOptions.AllowMultipleCorrectAnswers.HasValue
                && !setting.MultipleChoiceOptions.AllowRandomDistractors.HasValue
                && !setting.QueryDirections.Question2Answer.HasValue
                && !setting.QueryDirections.Answer2Question.HasValue
                && !setting.QueryDirections.Mixed.HasValue;
        }

        /// <summary>
        /// Called when [learning module opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnLearningModuleOpened(object sender, EventArgs e)
        {

            EventHandler handler = LearningModuleOpened;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [learning module exception].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ExceptionEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnLearningModuleException(object sender, ExceptionEventArgs e)
        {
            ExceptionEventHandler handler = LearningModuleException;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [converting to XML].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.LoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnConvertingToXml(object sender, LoadingEventArgs e)
        {
            LoadingEventHandler handler = LoadingIPhone;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Called when [finished converting to XML].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void OnFinishedConvertingToXml(object sender, EventArgs e)
        {
            EventHandler handler = FinishLoadingIPhone;
            if (handler != null)
                handler(sender, e);
        }
    }

    /// <summary>
    /// ExceptionEventArgs
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-07-18</remarks>
    public class ExceptionEventArgs : EventArgs
    {
        public Exception ThrownException { get; private set; }

        public string LearningModuleFileName { get; private set; }

        public ExceptionEventArgs(Exception exception, string learningModuleFilename)
            : base()
        {
            ThrownException = exception;
            LearningModuleFileName = learningModuleFilename;
        }
    }

    /// <summary>
    /// LoadingEventArgs
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-07-18</remarks>
    public class LoadingEventArgs : EventArgs
    {
        public int Value { get; private set; }

        public int Maximum { get; private set; }

        public string StatusMessage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingEventArgs"/> class.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        public LoadingEventArgs(string statusMessage, int currentValue, int maxValue)
        {
            StatusMessage = statusMessage;
            Value = currentValue;
            Maximum = maxValue;
        }
    }
}
