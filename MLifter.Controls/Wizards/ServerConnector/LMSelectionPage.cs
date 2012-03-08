using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Components;
using System.IO;
using MLifter.DAL.Tools;
using MLifter.Controls.Properties;

namespace MLifter.Controls.Wizards.ServerConnector
{
    public partial class LMSelectionPage : WizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LMSelectionPage"/> class.
        /// </summary>
        /// <param name="learnLogic">The learn logic.</param>
        /// <param name="localLMDirectory">The local file open dialog start directory.</param>
        /// <param name="localLMOpenFilter">The local file open dialog filter mask.</param>
        /// <param name="localLMSaveFilter">The local LM save filter.</param>
        /// <remarks>Documented by Dev02, 2008-09-22</remarks>
        public LMSelectionPage(LearnLogic learnLogic, string localLMDirectory, string localLMOpenFilter, string localLMSaveFilter)
        {
            InitializeComponent();
            listViewLMs_SelectedIndexChanged(listViewLMs, EventArgs.Empty);
            m_learnLogic = learnLogic;

            this.localLMDirectory = localLMDirectory;
            this.localLMOpenFilter = localLMOpenFilter;
            this.localLMSaveFilter = localLMSaveFilter;
        }

        LearnLogic m_learnLogic = null;
        IDictionaries dictionaries = null;

        string localLMDirectory = string.Empty;
        string localLMOpenFilter = string.Empty;
        string localLMSaveFilter = string.Empty;

        LoadStatusMessage loadStatusMessageImport = new LoadStatusMessage(Properties.Resources.COPYTO_STATUS_MESSAGE, 100, false);

        protected override void OnParentChanged(EventArgs e)
        {
            if (m_learnLogic.User != null)
                SetIDictionaries(m_learnLogic.User.GetLearningModuleList());
            base.OnParentChanged(e);
        }

        /// <summary>
        /// Refreshes the specified dictionaries.
        /// </summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        public void SetIDictionaries(IDictionaries dictionaries)
        {
            this.dictionaries = dictionaries;
            UpdateList();
            if (listViewLMs.Items.Count > 0)
            {
                listViewLMs.SelectedIndices.Clear();
                listViewLMs.SelectedIndices.Add(0);
            }
        }

        /// <summary>
        /// Refills the list.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        private void UpdateList()
        {
            listViewLMs.Items.Clear();
            foreach (IDictionary dictionary in dictionaries.Dictionaries)
            {
                ListViewItem item = new ListViewItem();
                item.Text = dictionary.Title;
                item.SubItems.Add(dictionary.Author);
                item.Tag = dictionary.Id;
                listViewLMs.Items.Add(item);
            }
            UpdateButtonsEnabled();
        }

        /// <summary>
        /// Gets the selected LM id.
        /// </summary>
        /// <value>The selected LM id.</value>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        public int SelectedLMId
        {
            get
            {
                if (listViewLMs.SelectedItems.Count < 1)
                    return -1;

                return (int)listViewLMs.SelectedItems[0].Tag;
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonCreateNew control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        private void buttonCreateNew_Click(object sender, EventArgs e)
        {
            if (dictionaries != null)
            {
                CreateNewLM(dictionaries);
            }
        }

        /// <summary>
        /// Creates a new LM.
        /// </summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        private void CreateNewLM(IDictionaries dictionaries)
        {
            if (dictionaries != null)
            {
                Wizard dicWizard = new Wizard();
                dicWizard.Text = MLifter.Controls.Properties.Resources.NEWDIC_CAPTION;
                dicWizard.Pages.Add(new Controls.Wizards.DictionaryCreator.WelcomePage());
                dicWizard.Pages.Add(new Controls.Wizards.DictionaryCreator.SideSettingsPage());

                if (dicWizard.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string dicName = (dicWizard.Pages[0] as Controls.Wizards.DictionaryCreator.WelcomePage).DictionaryName;

                        IDictionary newDic = dictionaries.AddNew(
                            (dicWizard.Pages[0] as Controls.Wizards.DictionaryCreator.WelcomePage).DictionaryCategory.Id,
                            (dicWizard.Pages[0] as Controls.Wizards.DictionaryCreator.WelcomePage).DictionaryName);

                        newDic.Author = (dicWizard.Pages[0] as Controls.Wizards.DictionaryCreator.WelcomePage).DictionaryAuthor;
                        newDic.Description = (dicWizard.Pages[0] as Controls.Wizards.DictionaryCreator.WelcomePage).DictionaryDescription;

                        newDic.DefaultSettings.AnswerCaption = (dicWizard.Pages[1] as Controls.Wizards.DictionaryCreator.SideSettingsPage).AnswerTitle;
                        newDic.DefaultSettings.QuestionCaption = (dicWizard.Pages[1] as Controls.Wizards.DictionaryCreator.SideSettingsPage).QuestionTitle;
                        newDic.DefaultSettings.AnswerCulture = (dicWizard.Pages[1] as Controls.Wizards.DictionaryCreator.SideSettingsPage).AnswerCulture;
                        newDic.DefaultSettings.QuestionCulture = (dicWizard.Pages[1] as Controls.Wizards.DictionaryCreator.SideSettingsPage).QuestionCulture;

                        UpdateList();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.ToString(), "Exception during attempt create LM.");
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewLMs.SelectedItems.Count > 0 && dictionaries != null)
            {
                try
                {
                    string messageBoxText = listViewLMs.SelectedItems.Count > 1 ?
                        string.Format("Are you sure to delete {0} Learning Modules?", listViewLMs.SelectedItems.Count) :
                        string.Format("Are you sure to delete the Learning Module \"{0}\"?", dictionaries.Get(SelectedLMId).Title);

                    if (MessageBox.Show(messageBoxText, "Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        foreach (ListViewItem selectedItem in listViewLMs.SelectedItems)
                            if (selectedItem.Tag is int)
                                dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, (int)selectedItem.Tag));

                        UpdateList();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString(), "Exception during attempt to delete.");
                }
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the listViewLMs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        private void listViewLMs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonsEnabled();
        }

        /// <summary>
        /// Updates the buttons disabled/enabled state.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void UpdateButtonsEnabled()
        {
            NextAllowed = buttonExport.Enabled = buttonDelete.Enabled = listViewLMs.SelectedItems.Count > 0;
        }

        /// <summary>
        /// Handles the Click event of the buttonImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-09-22</remarks>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (localLMOpenFilter != string.Empty)
                openDialog.Filter = localLMOpenFilter;
            if (localLMDirectory != string.Empty)
                openDialog.InitialDirectory = localLMDirectory;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = openDialog.FileName;
                ConnectionStringStruct connectionSource = new ConnectionStringStruct(DatabaseType.Xml, filename, true);

                int newLmId = dictionaries.AddNew(MLifter.DAL.Category.DefaultCategory, string.Empty).Id;
                ConnectionStringStruct connectionTarget = dictionaries.Parent.CurrentUser.ConnectionString;
                connectionTarget.LmId = newLmId;

                ShowStatusMessage(true);
                LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
                try
                {
                    LearnLogic.CopyLearningModule(connectionSource, connectionTarget,
                        (GetLoginInformation)LoginForm.OpenLoginForm, (CopyToProgress)UpdateStatusMessage,
                        (DataAccessErrorDelegate)DataAccessError, m_learnLogic.User);
                }
                catch (DictionaryNotDecryptedException)
                {
                    HideStatusMessage();

                    //delete partially created dictionary
                    dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, newLmId));
                    UpdateList();

                    MessageBox.Show(Properties.Resources.DIC_ERROR_NOT_DECRYPTED_TEXT,
                        Properties.Resources.DIC_ERROR_NOT_DECRYPTED_CAPTION,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (DictionaryContentProtectedException)
                {
                    HideStatusMessage();

                    //delete partially created dictionary
                    dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, newLmId));
                    UpdateList();

                    MessageBox.Show(string.Format(Properties.Resources.DIC_ERROR_CONTENTPROTECTED_TEXT, filename),
                        Properties.Resources.DIC_ERROR_CONTENTPROTECTED_CAPTION,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (MLifter.DAL.InvalidDictionaryException)
                {
                    HideStatusMessage();

                    //delete partially created dictionary
                    dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, newLmId));
                    UpdateList();

                    MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename),
                        Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.Xml.XmlException)
                {
                    HideStatusMessage();

                    //delete partially created dictionary
                    dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, newLmId));
                    UpdateList();

                    MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename),
                        Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.IO.IOException)
                {
                    HideStatusMessage();

                    //delete partially created dictionary
                    dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, newLmId));
                    UpdateList();


                    MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_LOCKED_TEXT, filename),
                        Properties.Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    HideStatusMessage();

                    //delete partially created dictionary
                    dictionaries.Delete(new ConnectionStringStruct(DatabaseType.PostgreSQL, string.Empty, newLmId));
                    UpdateList();

                    MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename),
                        Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataAccessError(object sender, Exception exp)
        {
            throw exp;
        }

        /// <summary>
        /// Handles the Click event of the buttonExport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-09-22</remarks>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (SelectedLMId >= 0)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                if (localLMSaveFilter != string.Empty)
                    saveDialog.Filter = localLMSaveFilter;
                if (localLMDirectory != string.Empty)
                    saveDialog.InitialDirectory = localLMDirectory;

                saveDialog.FileName = listViewLMs.SelectedItems[0].Text;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ConnectionStringStruct connectionSource = dictionaries.Parent.CurrentUser.ConnectionString;
                    connectionSource.LmId = SelectedLMId;

                    string filename = saveDialog.FileName;
                    ConnectionStringStruct connectionTarget = new ConnectionStringStruct(DatabaseType.Xml, filename, true);

                    //delete target LM if it already exists (=overwrite it)
                    try
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    }
                    catch (System.IO.IOException)
                    {
                        MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_LOCKED_TEXT, filename),
                            Properties.Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ShowStatusMessage(false);
                    LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
                    try
                    {
                        LearnLogic.CopyLearningModule(connectionSource, connectionTarget,
                            (GetLoginInformation)LoginForm.OpenLoginForm, (CopyToProgress)UpdateStatusMessage,
                            (DataAccessErrorDelegate)DataAccessError, m_learnLogic.User);
                    }
                    catch
                    {
                        HideStatusMessage();

                        //delete partially created dictionary
                        if (File.Exists(filename))
                            File.Delete(filename);

                        MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, filename),
                            Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        /// <summary>
        /// Updates the status message dialog with the current progress/infomessage.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        /// <param name="currentPercentage">The current percentage.</param>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void UpdateStatusMessage(string statusMessage, double currentPercentage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                loadStatusMessageImport.InfoMessage = statusMessage;
                loadStatusMessageImport.SetProgress(Convert.ToInt32(currentPercentage));
            });
        }

        /// <summary>
        /// Handles the CopyToFinished event of the LearnLogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        void LearnLogic_CopyToFinished(object sender, EventArgs e)
        {
            HideStatusMessage();
        }

        /// <summary>
        /// Hides the status message and activates the form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void HideStatusMessage()
        {
            LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
            ParentForm.Enabled = true;
            loadStatusMessageImport.Hide();
            UpdateList();
        }

        /// <summary>
        /// Shows the status message and deactivates the form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void ShowStatusMessage(bool importing)
        {
            ParentForm.Enabled = false;
            loadStatusMessageImport.InfoMessage = importing ? Properties.Resources.IMPORTING : Properties.Resources.EXPORTING;
            loadStatusMessageImport.EnableProgressbar = true;
            loadStatusMessageImport.Show();

        }

    }
}
