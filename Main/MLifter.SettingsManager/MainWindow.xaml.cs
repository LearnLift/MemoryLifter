using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
using MLifter.Controls;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.Generics;
using MLifterSettingsManager.DAL;

namespace MLifterSettingsManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields/Designer Variables

        private const string CUSTOM = "<Custom>";
        private OptimizeAudioProgress optimizeAudioProgress = null;
        private OptimizeAudio optimizeAudio = null;

        #endregion Fields/Designer Variables

        #region Properties

        /// <summary>
        /// Gets or sets the tree view items (LearningModule/Chapter/Card)
        /// </summary>
        /// <value>The tree view items.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ObservableCollection<LearningModuleTreeViewItem> TreeViewItems { get; set; }

        /// <summary>
        /// Gets or sets the extension list.
        /// </summary>
        /// <value>The extension list.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public ObservableCollection<IExtension> ExtensionList { get; set; }

        /// <summary>
        /// Gets or sets the settings manager logic.
        /// </summary>
        /// <value>The settings manager logic.</value>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public SettingsManagerBusinessLogic SettingsManagerLogic { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        public MainWindow()
        {
            InitializeComponent();

            SettingsManagerLogic = new SettingsManagerBusinessLogic();
            SettingsManagerLogic.LearningModuleOpened += new EventHandler(SettingsManagerLogic_LearningModuleOpened);
            SettingsManagerLogic.LearningModuleException += new SettingsManagerBusinessLogic.ExceptionEventHandler(SettingsManagerLogic_LearningModuleException);
            SettingsManagerLogic.LoadingIPhone += new SettingsManagerBusinessLogic.LoadingEventHandler(SettingsManagerLogic_LoadingIPhone);
            SettingsManagerLogic.FinishLoadingIPhone += new EventHandler(SettingsManagerLogic_FinishLoadingIPhone);

            settingsControlMain.Settings = new SettingsTemplate();
            ExtensionList = new ObservableCollection<IExtension>();
            TreeViewItems = new ObservableCollection<LearningModuleTreeViewItem>();
            filterSortControl.TreeViewItems = TreeViewItems;

            EnableControls(false);
        }

        #region GUI Events

        #region MainMenu Events

        /// <summary>
        /// Handles the Click event of the menuItemOpen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void menuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Learning Modules (*.mlm)|*.mlm";

            if (ofd.ShowDialog().Value)
                OpenLearningModule(ofd.FileName);
        }

        /// <summary>
        /// Handles the Click event of the menuItemPublish control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-05-20</remarks>
        private void menuItemPublish_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsManagerLogic.LearningModule == null)
                return;

            string learningModulePath = SettingsManagerLogic.LearningModule.Connection;

            SettingsManagerLogic.ClearUnusedMedia();
            CloseModule();

            PublisherForm publisherForm = new PublisherForm(learningModulePath);
            publisherForm.Owner = this;
            publisherForm.ShowDialog();

            OpenLearningModule(learningModulePath);
        }

        /// <summary>
        /// Handles the Click event of the menuItemOptimizeAudio control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void menuItemOptimizeAudio_Click(object sender, RoutedEventArgs e)
        {
            optimizeAudio = new OptimizeAudio(SettingsManagerLogic.LearningModule);
            optimizeAudio.StartOptimization(AudioOptimizeCallback);
            optimizeAudio.OptimizationFinished += new EventHandler(optimize_OptimizationFinished);

            optimizeAudioProgress = new OptimizeAudioProgress();
            optimizeAudioProgress.Owner = this;
            optimizeAudioProgress.Closing += new System.ComponentModel.CancelEventHandler(optimizeAudioProgress_Closing);
            optimizeAudioProgress.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the menuItemExportToXml control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-07-13</remarks>
        private void menuItemExportToXml_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = MLifter.SettingsManager.Properties.Resources.ODX_FILEFILTER;
            saveFileDialog.FileName = SettingsManagerLogic.LearningModule.Title.Replace(':', '_').Replace('\\', '_').Replace('/', '_').Replace('?', '_').Replace('*', '_').Replace('|', '_').Replace('<', '_').Replace('>', '_');
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                EnableAllControls(false);
                List<int> uncheckedCardIds = GetUncheckCards();
                if (uncheckedCardIds.Count == SettingsManagerLogic.LearningModule.Cards.Count)
                    uncheckedCardIds.Clear();
                else if (uncheckedCardIds.Count > 0)
                {
                    System.Windows.Forms.DialogResult dr = TaskDialog.MessageBox(
                        "Export only selected cards?",
                        String.Format("{0} cards are not checked in the tree menu.", uncheckedCardIds.Count),
                        "Do you wish to export only the selected cards?",
                        TaskDialogButtons.YesNoCancel, TaskDialogIcons.Question);
                    switch (dr)
                    {
                        case System.Windows.Forms.DialogResult.Cancel:
                            EnableAllControls(true);
                            return;
                        case System.Windows.Forms.DialogResult.No:
                            uncheckedCardIds.Clear();
                            break;
                        case System.Windows.Forms.DialogResult.Yes:
                        default:
                            break;
                    }
                }
                SettingsManagerLogic.ConvertToXmlFormat(saveFileDialog.FileName, uncheckedCardIds);
            }
        }

        /// <summary>
        /// Handles the Click event of the menuItemExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-09</remarks>
        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the menuItemAutoConvertPublish control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void menuItemAutoConvertPublish_Click(object sender, RoutedEventArgs e)
        {
            PublisherConverterForm autoConverterPublisher = new PublisherConverterForm();
            autoConverterPublisher.Owner = this;
            autoConverterPublisher.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the menuItemCreateExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void menuItemCreateExtension_Click(object sender, RoutedEventArgs e)
        {
            ExtensionForm createExtension = new ExtensionForm();
            createExtension.Owner = this;
            createExtension.ShowDialog();
        }

        #endregion

        #region Context Menu Events

        /// <summary>
        /// Handles the Click event of the menuItemSelectAllCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void menuItemSelectAllCards_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem))
                return;

            ChapterTreeViewItem chapterTreeViewItem = (sender as MenuItem).Tag as ChapterTreeViewItem;
            if (chapterTreeViewItem == null)
                return;

            SelectCardTreeViewItems(chapterTreeViewItem);
        }

        /// <summary>
        /// Handles the Click event of the menuItemDeselectAllCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void menuItemDeselectAllCards_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem))
                return;

            ChapterTreeViewItem chapterTreeViewItem = (sender as MenuItem).Tag as ChapterTreeViewItem;
            if (chapterTreeViewItem == null)
                return;

            DeselectCardTreeViewItems(chapterTreeViewItem);
        }

        /// <summary>
        /// Handles the Click event of the menuItemInvertSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void menuItemInvertSelection_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem))
                return;

            ChapterTreeViewItem chapterTreeViewItem = (sender as MenuItem).Tag as ChapterTreeViewItem;
            if (chapterTreeViewItem == null)
                return;

            InvertSelectionCardTreeViewItems(chapterTreeViewItem);
        }

        /// <summary>
        /// Handles the Click event of the menuItemLMDeselectAllCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void menuItemLMDeselectAllCards_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem))
                return;

            LearningModuleTreeViewItem learningModuleTreeViewItem = (sender as MenuItem).Tag as LearningModuleTreeViewItem;
            if (learningModuleTreeViewItem == null)
                return;

            DeselectCardTreeViewItems(learningModuleTreeViewItem);
        }

        /// <summary>
        /// Handles the Opened event of the contextMenuChapter control.
        /// Usability: Selects the treeViewItem, which was used to open the context menu
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void contextMenuChapter_Opened(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContextMenu))
                return;

            ChapterTreeViewItem chapterTreeViewItem = (sender as ContextMenu).Tag as ChapterTreeViewItem;
            if (chapterTreeViewItem == null)
                return;

            chapterTreeViewItem.IsSelected = true;
        }

        /// <summary>
        /// Handles the Opened event of the contextMenuLearningModule control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void contextMenuLearningModule_Opened(object sender, RoutedEventArgs e)
        {
            if (!(sender is ContextMenu))
                return;

            LearningModuleTreeViewItem learningModuleTreeViewItem = (sender as ContextMenu).Tag as LearningModuleTreeViewItem;
            if (learningModuleTreeViewItem == null)
                return;

            learningModuleTreeViewItem.IsSelected = true;
        }

        #endregion

        /// <summary>
        /// Handles the SelectedItemChanged event of the treeViewLearningModule control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Object&gt;"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        private void treeViewLearningModule_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(sender is TreeView))
                return;

            if ((sender as TreeView).SelectedItem is LearningModuleTreeViewItem)
            {
                LearningModuleTreeViewItem learningModuleTreeViewItem = (sender as TreeView).SelectedItem as LearningModuleTreeViewItem;
                settingsControlMain.SetSettings(learningModuleTreeViewItem.Settings);
            }
            else if ((sender as TreeView).SelectedItem is ChapterTreeViewItem)
            {
                ChapterTreeViewItem chapterTreeViewItem = (sender as TreeView).SelectedItem as ChapterTreeViewItem;
                settingsControlMain.SetSettings(chapterTreeViewItem.Settings);
            }
            else if ((sender as TreeView).SelectedItem is CardTreeViewItem)
            {
                CardTreeViewItem cardTreeViewItem = (sender as TreeView).SelectedItem as CardTreeViewItem;
                settingsControlMain.SetSettings(cardTreeViewItem.Settings);
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonClearSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        private void buttonClearSettings_Click(object sender, EventArgs e)
        {
            if (treeViewLearningModule.SelectedItem is LearningModuleTreeViewItem)
            {
                settingsControlMain.ClearSettings();
                ClearSetting((treeViewLearningModule.SelectedItem as LearningModuleTreeViewItem).Settings);

                (treeViewLearningModule.SelectedItem as LearningModuleTreeViewItem).HasCustomSettings = false;
            }
            else if (treeViewLearningModule.SelectedItem is ChapterTreeViewItem)
            {
                settingsControlMain.ClearSettings();
                ClearSetting((treeViewLearningModule.SelectedItem as ChapterTreeViewItem).Settings);

                (treeViewLearningModule.SelectedItem as ChapterTreeViewItem).HasCustomSettings = false;
            }
            else if (treeViewLearningModule.SelectedItem is CardTreeViewItem)
            {
                bool settingsControlsCleared = false;
                foreach (CardTreeViewItem cardTreeViewItem in ((treeViewLearningModule.SelectedItem as CardTreeViewItem).Parent as ChapterTreeViewItem).Cards)
                {
                    if (cardTreeViewItem.IsChecked)
                    {
                        if (!settingsControlsCleared)
                        {
                            settingsControlMain.ClearSettings();
                            settingsControlsCleared = true;
                        }

                        ClearSetting(cardTreeViewItem.Settings);
                        cardTreeViewItem.HasCustomSettings = false;
                    }
                }
            }

            //Deselect all checkboxes
            foreach (ChapterTreeViewItem item in TreeViewItems[0].Chapters)
                DeselectCardTreeViewItems(item);
        }

        /// <summary>
        /// Handles the Click event of the buttonApplySettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        private void buttonApplySettings_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewLearningModule.SelectedItem is LearningModuleTreeViewItem)
            {
                settingsControlMain.UpdateSettings();
                settingsControlMain.Settings.CopyTo(SettingsManagerLogic.LearningModule.AllowedSettings, null);
                (treeViewLearningModule.SelectedItem as LearningModuleTreeViewItem).HasCustomSettings = true;
            }
            else if (treeViewLearningModule.SelectedItem is ChapterTreeViewItem)
            {
                settingsControlMain.UpdateSettings();
                IChapter chapter = (treeViewLearningModule.SelectedItem as ChapterTreeViewItem).Chapter;
                settingsControlMain.Settings.CopyTo(chapter.Settings, null);

                (treeViewLearningModule.SelectedItem as ChapterTreeViewItem).HasCustomSettings = true;
            }
            else if (treeViewLearningModule.SelectedItem is CardTreeViewItem)
            {
                foreach (CardTreeViewItem cardTreeViewItem in ((treeViewLearningModule.SelectedItem as CardTreeViewItem).Parent as ChapterTreeViewItem).Cards)
                {
                    if (cardTreeViewItem.IsChecked)
                    {
                        settingsControlMain.UpdateSettings();
                        settingsControlMain.Settings.CopyTo(cardTreeViewItem.Settings, null);
                        cardTreeViewItem.HasCustomSettings = true;
                    }
                }
            }

            //Deselect all checkboxes
            foreach (ChapterTreeViewItem item in TreeViewItems[0].Chapters)
                DeselectCardTreeViewItems(item);
        }

        /// <summary>
        /// Handles the Closing event of the optimizeAudioProgress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void optimizeAudioProgress_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (optimizeAudio != null)
            {
                if (MessageBox.Show("Are you sure to abort the optimization process?", "Abort", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    optimizeAudio.StopOptimization();
                else
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the OptimizationFinished event of the optimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void optimize_OptimizationFinished(object sender, EventArgs e)
        {
            if (optimizeAudioProgress != null && optimizeAudioProgress.IsVisible)
            {
                optimizeAudioProgress.Closing -= new System.ComponentModel.CancelEventHandler(optimizeAudioProgress_Closing);
                optimizeAudioProgress.Close();
            }
        }

        /// <summary>
        /// Audioes the optimize callback.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="progress">The progress.</param>
        private void AudioOptimizeCallback(string status, double progress)
        {
            if (optimizeAudioProgress != null)
                optimizeAudioProgress.ProgressCallback(status, progress);
        }

        /// <summary>
        /// Handles the Click event of the buttonRemoveExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-10</remarks>
        private void buttonRemoveExtension_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < SettingsManagerLogic.LearningModule.Extensions.Count; i++)
            {
                if (SettingsManagerLogic.LearningModule.Extensions[i].Id == (listViewExtensions.SelectedItem as IExtension).Id)
                {
                    SettingsManagerLogic.LearningModule.Extensions.RemoveAt(i);
                    break;
                }
            }

            ExtensionList.Remove(listViewExtensions.SelectedItem as IExtension);                    //remove from listview
            listViewExtensions.ItemsSource = ExtensionList;
        }


        /// <summary>
        /// Handles the Click event of the buttonAddExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-10</remarks>
        private void buttonAddExtension_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = MLifter.SettingsManager.Properties.Resources.EXTENSION_FILEFILTER;
            if (ofd.ShowDialog().Value)
            {
                ExtensionFile extFile = new ExtensionFile(ofd.FileName);
                extFile.Open(LoginForm.OpenLoginForm);
                Guid extensionId = extFile.Extension.Id;

				if (SettingsManagerLogic.LearningModule.Extensions.Any(ext => ext.Id == extensionId) && 
					MessageBox.Show(String.Format(MLifter.SettingsManager.Properties.Resources.EXTENSION_REPLACE_TEXT, extFile.Extension.Name), 
					MLifter.SettingsManager.Properties.Resources.EXTENSION_REPLACE_CAPTION, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                IExtension newExt = SettingsManagerLogic.LearningModule.ExtensionFactory(extFile.Extension.Id);
                extFile.Extension.CopyTo(newExt, null);

                LoadLMExtensions();
            }
        }

        #endregion

        #region BusinessLogic Events

        /// <summary>
        /// Handles the LearningModuleOpened event of the SettingsManagerLogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void SettingsManagerLogic_LearningModuleOpened(object sender, EventArgs e)
        {
            progressBarLoad.IsIndeterminate = false;
            textBlockProgressBar.Text = string.Empty;
            LoadContent();
        }

        /// <summary>
        /// Handles the LearningModuleException event of the SettingsManagerLogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ExceptionEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void SettingsManagerLogic_LearningModuleException(object sender, ExceptionEventArgs e)
        {
            progressBarLoad.IsIndeterminate = false;

            try
            {
                throw e.ThrownException;
            }
            #region Error handling
            catch (DatabaseVersionNotSupported exp)
            {
                if (!exp.SilentUpgrade)
                {
                    TaskDialog.MessageBox(MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_CAPTION,
                        MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_MAIN,
                        MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Warning);

                    // Update??? yes or no???
                }
                else
                {
                    System.Windows.Forms.DialogResult dr = TaskDialog.MessageBox(MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_CAPTION,
                                                               MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_MAIN,
                                                               MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_UPGRADE,
                                                               TaskDialogButtons.YesNo, TaskDialogIcons.Warning);

                    if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            if (!SettingsManagerLogic.UpdateLearningModule(exp.DatabaseVersion))
                            {
                                TaskDialog.MessageBox(MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_UPGRADE_FAILED_CAPTION,
                                    MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_UPGRADE_FAILED_MAIN,
                                    MLifter.SettingsManager.Properties.Resources.DBVERSION_NOT_SUPPORTED_UPGRADE_FAILED_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Error);

                                return;
                            }
                        }
                        catch (Exception exp2)
                        {
                            TaskDialog.MessageBox(MLifter.SettingsManager.Properties.Resources.DIC_ERROR_LOADING_CAPTION,
                                MLifter.SettingsManager.Properties.Resources.DIC_ERROR_LOADING_CAPTION,
                                string.Format(MLifter.SettingsManager.Properties.Resources.DIC_ERROR_LOADING_TEXT, e.LearningModuleFileName),
                                exp2.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);

                            return;
                        }
                    }
                    else
                        return;
                }
            }
            catch (ProtectedLearningModuleException exp)
            {
                TaskDialog.MessageBox(MLifter.SettingsManager.Properties.Resources.LM_IS_PROTECTED_CAPTION,
                    MLifter.SettingsManager.Properties.Resources.LM_IS_PROTECTED_CONTENT, exp.Message.ToString(),
                    TaskDialogButtons.OK, TaskDialogIcons.Error);

                return;
            }
            catch (Exception exp)
            {
                TaskDialog.MessageBox("Unknown Error", "An unknown error occured. Please immediately call ML Development",
                    exp.Message.ToString(), TaskDialogButtons.OK, TaskDialogIcons.Error);

                return;
            }
            #endregion

            LoadContent();
        }

        /// <summary>
        /// Handles the ContentLoaded event of the item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void ContentLoaded(object sender, EventArgs e)
        {
            treeViewLearningModule.ItemsSource = TreeViewItems;
            filterSortControl.ReloadFilter();
            LoadLMExtensions();
            EnableAllControls(true);

            TreeViewItems[0].IsSelected = true;
            textBlockProgressBar.Text = string.Empty;
            progressBarLoad.Value = 0;
        }

        /// <summary>
        /// Handles the LoadingIPhone event of the SettingsManagerLogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.LoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void SettingsManagerLogic_LoadingIPhone(object sender, LoadingEventArgs e)
        {
            //Here I can't use the SyncronizedContext, because I have to use the CopyProgress from MLifter.DAL
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                progressBarLoad.Maximum = e.Maximum;
                progressBarLoad.Value = e.Value;
                textBlockProgressBar.Text = e.StatusMessage;
            }));
        }

        /// <summary>
        /// Handles the FinishLoadingIPhone event of the SettingsManagerLogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void SettingsManagerLogic_FinishLoadingIPhone(object sender, EventArgs e)
        {
            //Here I can't use the SyncronizedContext, because I have to use the CopyProgress from MLifter.DAL
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                progressBarLoad.Value = 0;
                textBlockProgressBar.Text = string.Empty;
                TaskDialog.MessageBox("IPhone export successful", "The IPhone export was successful", string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Information);
                EnableAllControls(true);
            }));
        }

        /// <summary>
        /// Contents the loading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MLifterSettingsManager.ContentLoadingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void ContentLoading(object sender, ContentLoadingEventArgs e)
        {
            textBlockProgressBar.Text = e.ContentType == ContentType.Card ? "Loading Cards..." : "Loading Chapters";
            progressBarLoad.Maximum = e.Maximum;
            progressBarLoad.Value = e.Value;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Opens the learning module.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void OpenLearningModule(string filename)
        {
            progressBarLoad.IsIndeterminate = true;
            CloseModule();
            SettingsManagerLogic.OpenLearningModule(filename);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-18</remarks>
        private void LoadContent()
        {
            textBlockProgressBar.Text = string.Empty;
            progressBarLoad.IsIndeterminate = false;

            TreeViewItems.Add(new LearningModuleTreeViewItem(SettingsManagerLogic.LearningModule));

            LearningModuleTreeViewItem item = TreeViewItems[0];
            item.ContentLoaded += new EventHandler(ContentLoaded);
            item.ContentLoading += new LearningModuleTreeViewItem.ContentLoadingEventHandler(ContentLoading);
            item.LoadLearningModuleContent();
        }

        /// <summary>
        /// Clears the setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        private void ClearSetting(ISettings setting)
        {
            setting.QueryTypes.Word = null;
            setting.QueryTypes.Sentence = null;
            setting.QueryTypes.MultipleChoice = null;
            setting.QueryTypes.ListeningComprehension = null;
            setting.QueryTypes.ImageRecognition = null;

            setting.QueryDirections.Answer2Question = null;
            setting.QueryDirections.Question2Answer = null;
            setting.QueryDirections.Mixed = null;

            setting.MultipleChoiceOptions.AllowMultipleCorrectAnswers = null;
            setting.MultipleChoiceOptions.AllowRandomDistractors = null;
            setting.MultipleChoiceOptions.MaxNumberOfCorrectAnswers = null;
            setting.MultipleChoiceOptions.NumberOfChoices = null;
        }

        /// <summary>
        /// Closes the module.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void CloseModule()
        {
            SettingsManagerLogic.CloseLearningModule();

            TreeViewItems.Clear();
            treeViewLearningModule.ItemsSource = null;
            ExtensionList.Clear();
            EnableControls(false);
        }

        /// <summary>
        /// Loads the LM extensions.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void LoadLMExtensions()
        {
            ExtensionList = new ObservableCollection<IExtension>(SettingsManagerLogic.LearningModule.Extensions);
            listViewExtensions.ItemsSource = ExtensionList;
        }

        /// <summary>
        /// Enables the controls.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void EnableControls(bool value)
        {
            tabControlSettingsManager.IsEnabled = value;
            menuItemPublish.IsEnabled = value;
            menuItemExportToXml.IsEnabled = value;
            menuItemOptimizeAudio.IsEnabled = value;
        }


        /// <summary>
        /// Enables all controls. One additional plus the usual above.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// Documented by MatBre00, 14.8.2009
        private void EnableAllControls(bool value)
        {
            menuMain.IsEnabled = value;
            EnableControls(value);
        }

        /// <summary>
        /// Selects the card tree view items.
        /// </summary>
        /// <param name="chapterTreeViewItem">The chapter tree view item.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void SelectCardTreeViewItems(ChapterTreeViewItem chapterTreeViewItem)
        {
            foreach (CardTreeViewItem cardTreeViewItem in chapterTreeViewItem.Cards)
                cardTreeViewItem.IsChecked = true;
        }

        /// <summary>
        /// Selects the card tree view items.
        /// </summary>
        /// <param name="chapterTreeViewItem">The chapter tree view item.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void DeselectCardTreeViewItems(ChapterTreeViewItem chapterTreeViewItem)
        {
            foreach (CardTreeViewItem cardTreeViewItem in chapterTreeViewItem.Cards)
                cardTreeViewItem.IsChecked = false;
        }

        /// <summary>
        /// Deselects the card tree view items.
        /// </summary>
        /// <param name="learningModuleTreeViewItem">The learning module tree view item.</param>
        /// <remarks>Documented by Dev08, 2009-07-16</remarks>
        private void DeselectCardTreeViewItems(LearningModuleTreeViewItem learningModuleTreeViewItem)
        {
            foreach (ChapterTreeViewItem chapterTreeViewItem in learningModuleTreeViewItem.Chapters)
                DeselectCardTreeViewItems(chapterTreeViewItem);
        }

        /// <summary>
        /// Inverts the selection card tree view items.
        /// </summary>
        /// <param name="chapterTreeViewItem">The chapter tree view item.</param>
        /// <remarks>Documented by Dev08, 2009-07-15</remarks>
        private void InvertSelectionCardTreeViewItems(ChapterTreeViewItem chapterTreeViewItem)
        {
            foreach (CardTreeViewItem cardTreeViewItem in chapterTreeViewItem.Cards)
                cardTreeViewItem.IsChecked = !cardTreeViewItem.IsChecked;
        }

        /// <summary>
        /// Gets a list of card ids which are not checked.
        /// </summary>
        /// <returns>List of card ids.</returns>
        /// <remarks>Documented by Dev03, 2009-08-13</remarks>
        private List<int> GetUncheckCards()
        {
            List<int> cardIdsNotToExport = new List<int>();
            foreach (LearningModuleTreeViewItem lm in treeViewLearningModule.Items)
            {
                foreach (ChapterTreeViewItem ch in lm.Chapters)
                {
                    foreach (CardTreeViewItem ca in ch.Cards)
                    {
                        if (!ca.IsChecked)
                            cardIdsNotToExport.Add(ca.Id);
                    }
                }
            }
            return cardIdsNotToExport;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnWindowPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, e);
        }

        #endregion
    }
}