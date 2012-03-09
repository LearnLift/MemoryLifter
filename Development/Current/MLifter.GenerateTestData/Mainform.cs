using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MLifter.BusinessLayer;
using MLifter.Controls;
using MLifter.Components;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.GenerateTestData.BL;

namespace MLifter.GenerateTestData
{
    public partial class Mainform : Form
    {
        private LearnLogic m_learnLogic;
        private IDictionaries dictionaries;
        LoadStatusMessage loadStatusMessageImport = new LoadStatusMessage(String.Empty, 100, true);
        private string connectionstringfile = Path.Combine(Application.StartupPath, "connectionstring.txt");

        private const int defSessionNum = 1;
        private const int maxSessionNum = 100;
        private const int defCardNum = 10;
        private const int maxCardNum = 10000;

        private Random rand = new Random((int)DateTime.Now.Ticks);
        private bool connected = false;

        /// <summary>
        /// Gets the checked learning modules.
        /// </summary>
        /// <value>The checked learning modules.</value>
        /// <remarks>Documented by Dev03, 2008-11-21</remarks>
        private List<int> CheckedLearningModules
        {
            get
            {
                List<int> checkedLMs = new List<int>();
                foreach (ListViewItem item in listViewLMs.CheckedItems)
                    checkedLMs.Add(Convert.ToInt32(item.Tag));
                return checkedLMs;
            }
        }

        public Mainform(string[] args)
        {
            InitializeComponent();
            m_learnLogic = new LearnLogic(GetUser, DataAccessError);
        }

        private Dictionary<ConnectionStringStruct, bool> defaultUserSubmitted = new Dictionary<ConnectionStringStruct, bool>();
        private static Dictionary<string, UserStruct> authenticationUsers = new Dictionary<string, UserStruct>();
        internal UserStruct? GetUser(UserStruct user, ConnectionStringStruct connection)
        {
            if (authenticationUsers.ContainsKey(connection.ConnectionString) && (!defaultUserSubmitted.ContainsKey(connection) || !defaultUserSubmitted[connection]))
            {
                defaultUserSubmitted[connection] = true;
                return authenticationUsers[connection.ConnectionString];
            }
            else
            {
                UserStruct? newUser = LoginForm.OpenLoginForm(user, connection);
                if (newUser.HasValue)
                    authenticationUsers[connection.ConnectionString] = newUser.Value;
                return newUser;
            }
        }

        /// <summary>
        /// Takes care of data access errors.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exp">The exp.</param>
        /// <remarks>Documented by Dev03, 2009-03-02</remarks>
        private void DataAccessError(object sender, Exception exp)
        {
            throw exp;
        }

        /// <summary>
        /// Handles the Click event of the buttonSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-11-21</remarks>
        private void buttonSource_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = textBoxSource.Text;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                textBoxSource.Text = openFileDialog.FileName;
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            listViewLMs.CheckBoxes = true;
            groupBoxCopyLM.Enabled = checkBoxCopyLM.Checked;

            textBoxSessionNum.Text = defSessionNum.ToString();
            textBoxCardNum.Text = defCardNum.ToString();

            if (File.Exists(connectionstringfile))
                textBoxConnectionString.Text = File.ReadAllText(connectionstringfile);

            Connect();
            UpdateList();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            m_learnLogic.CloseLearningModule();
            base.OnClosing(e);
        }

        /// <summary>
        /// Handles the Click event of the buttonGenerate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-11-21</remarks>
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            if (!Connect())
                return;
            if (checkBoxCopyLM.Checked)
                CopyLM();
            else
                GenerateTestData();
        }

        /// <summary>
        /// Generates the test data.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-11-18</remarks>
        private void GenerateTestData()
        {
            if (CheckedLearningModules.Count < 1)
            {
                MessageBox.Show(Properties.Resources.ERROR_NO_LM_SELECTED_TEXT,
                    Properties.Resources.ERROR_NO_LM_SELECTED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowStatusMessage(false);
            try
            {
                ConnectionStringStruct connectionTarget =
                    new ConnectionStringStruct(DatabaseType.PostgreSQL, textBoxConnectionString.Text, -1);
                m_learnLogic.CurrentLearningModule = new LearningModulesIndexEntry(connectionTarget);
                TestDataGenerator gen = new TestDataGenerator(m_learnLogic);
                gen.TestStatusProgressChanged += new TestStatusEventHandler(gen_TestStatusProgressChanged);
                gen.Generate(CheckedLearningModules, ParseSessionNum(), ParseCardNum());
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Properties.Resources.ERROR_GENERATE_TEXT, e.Message),
                    Properties.Resources.ERROR_GENERATE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideStatusMessage();
            }
            connected = false;
        }

        void gen_TestStatusProgressChanged(object sender, TestStatusEventArgs args)
        {
            this.Invoke((MethodInvoker)delegate
            {
                loadStatusMessageImport.InfoMessage = (sender as TestDataGenerator).CurrentDictionary.DictionaryDisplayTitle;
                loadStatusMessageImport.SetProgress(args.ProgressPercentage);
            });
        }

        private int importedId = -1;
        /// <summary>
        /// Copies a XML Learning Module to the database.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-11-18</remarks>
        private void CopyLM()
        {
            if (!File.Exists(textBoxSource.Text))
                return;
            ConnectionStringStruct connectionTarget;
            ConnectionStringStruct connectionSource = new ConnectionStringStruct(DatabaseType.Xml, textBoxSource.Text, false);
            int newLmId = dictionaries.AddNew(MLifter.DAL.Category.DefaultCategory, string.Empty).Id;
            connectionTarget = dictionaries.Parent.CurrentUser.ConnectionString;
            connectionTarget.LmId = importedId = newLmId;

            ShowStatusMessage(true);
            LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
            try
            {
                LearnLogic.CopyLearningModule(connectionSource, connectionTarget,
                    (GetLoginInformation)LoginForm.OpenLoginForm, (CopyToProgress)UpdateStatusMessage,
                    (DataAccessErrorDelegate)delegate(object o, Exception ex) { return; }, m_learnLogic.User);
            }
            catch
            {
                HideStatusMessage();

                //delete partially created dictionary
                dictionaries.Delete(connectionTarget);

                MessageBox.Show(Properties.Resources.DIC_ERROR_LOADING_TEXT,
                    Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Connects to the LM database.
        /// </summary>
        /// <returns>True if success</returns>
        /// <remarks>Documented by Dev03, 2008-11-18</remarks>
        private bool Connect()
        {
            if (connected)
                return true;

            try
            {
                if (m_learnLogic != null) m_learnLogic.CloseLearningModule();
                ConnectionStringStruct connectionTarget = new ConnectionStringStruct(DatabaseType.PostgreSQL, textBoxConnectionString.Text, -1);
                m_learnLogic.User.Authenticate(GetUser, connectionTarget, DataAccessError);
                dictionaries = m_learnLogic.User.GetLearningModuleList();
                File.WriteAllText(connectionstringfile, textBoxConnectionString.Text);
                textBoxCurrentUser.Text = m_learnLogic.User.ToString();
                connected = true;
            }
            catch (Exception exp)
            {
                System.Diagnostics.Trace.Write(exp.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Refills the list.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-07-31</remarks>
        private void UpdateList()
        {
            List<int> ids = new List<int>();
            foreach (ListViewItem item in listViewLMs.Items)
                if (item.Checked)
                    ids.Add(Convert.ToInt32(item.Tag));

            listViewLMs.Items.Clear();
            if (dictionaries == null || dictionaries.Dictionaries == null) return;
            foreach (IDictionary dictionary in dictionaries.Dictionaries)
            {
                ListViewItem item = new ListViewItem();
                item.Text = dictionary.Title;
                item.SubItems.Add(dictionary.Author);
                item.SubItems.Add(dictionary.Cards.Count.ToString());
                item.Tag = dictionary.Id;
                if (ids.Contains(dictionary.Id))
                    item.Checked = true;
                listViewLMs.Items.Add(item);
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
            UpdateList();
            if (importedId >= 0)
            {
                foreach (ListViewItem item in listViewLMs.Items)
                    if (Convert.ToInt32(item.Tag) == importedId)
                        item.Checked = true;
                importedId = -1;
            }

            checkBoxCopyLM.Checked = false;
            buttonGenerate_Click(null, EventArgs.Empty);
        }

        /// <summary>
        /// Hides the status message and activates the form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void HideStatusMessage()
        {
            LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
            this.Enabled = true;
            loadStatusMessageImport.Hide();
        }

        /// <summary>
        /// Shows the status message and deactivates the form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void ShowStatusMessage(bool importing)
        {
            this.Enabled = false;
            loadStatusMessageImport.InfoMessage = importing ? Properties.Resources.IMPORTING : Properties.Resources.GENERATING;
            loadStatusMessageImport.EnableProgressbar = true;
            loadStatusMessageImport.Width = 300;
            loadStatusMessageImport.Show();

        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxCopyLM control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-11-19</remarks>
        private void checkBoxCopyLM_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxCopyLM.Enabled = checkBoxCopyLM.Checked;
        }

        /// <summary>
        /// Handles the Click event of the buttonChangeUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-11-19</remarks>
        private void buttonChangeUser_Click(object sender, EventArgs e)
        {
            connected = false;
            Connect();
            UpdateList();
        }

        /// <summary>
        /// Handles the TextChanged event of the textBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-11-19</remarks>
        private void textBoxSessionNum_TextChanged(object sender, EventArgs e)
        {
            ParseSessionNum();
        }

        /// <summary>
        /// Parses the session number.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-11-19</remarks>
        private int ParseSessionNum()
        {
            int val;
            if (!int.TryParse(textBoxSessionNum.Text, out val))
            {
                MessageBox.Show(String.Format(Properties.Resources.ERROR_SESSION_NUM_TEXT, 0, maxSessionNum), Properties.Resources.ERROR_SESSION_NUM_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                val = defSessionNum;
            }
            else if ((val < 0) || (val > maxSessionNum))
            {
                MessageBox.Show(String.Format(Properties.Resources.ERROR_SESSION_NUM_TEXT, 0, maxSessionNum), Properties.Resources.ERROR_SESSION_NUM_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                val = defSessionNum;
            }
            textBoxSessionNum.Text = val.ToString();
            return val;
        }

        /// <summary>
        /// Handles the TextChanged event of the textBox2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-11-19</remarks>
        private void textBoxCardNum_TextChanged(object sender, EventArgs e)
        {
            ParseCardNum();
        }

        /// <summary>
        /// Parses the card num.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-11-19</remarks>
        private int ParseCardNum()
        {
            int val;
            if (!int.TryParse(textBoxCardNum.Text, out val))
            {
                MessageBox.Show(String.Format(Properties.Resources.ERROR_CARD_NUM_TEXT, 0, maxCardNum), Properties.Resources.ERROR_CARD_NUM_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                val = defCardNum;
            }
            else if ((val < 0) || (val > maxCardNum))
            {
                MessageBox.Show(String.Format(Properties.Resources.ERROR_CARD_NUM_TEXT, 0, maxCardNum), Properties.Resources.ERROR_CARD_NUM_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                val = defCardNum;
            }
            textBoxCardNum.Text = val.ToString();
            return val;
        }

        private void textBoxConnectionString_TextChanged(object sender, EventArgs e)
        {
            connected = false;
        }

        private bool GetRandomBool()
        {
            return (rand.Next(2) == 0);
        }
    }
}