using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MLifter.Properties;
using MLifter.Generics;

namespace MLifter
{
    /// <summary>
    /// This form allows you to chose the chapters you want to learn.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-19</remarks>
    public class TChapterForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox GBChapters;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private MLifter.Controls.ChapterFrame Chapters;
        private System.Windows.Forms.Label LInfo;
        private System.Windows.Forms.HelpProvider MainHelp;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public TChapterForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
        }

        private static MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TChapterForm));
            this.GBChapters = new System.Windows.Forms.GroupBox();
            this.LInfo = new System.Windows.Forms.Label();
            this.Chapters = new MLifter.Controls.ChapterFrame();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.GBChapters.SuspendLayout();
            this.SuspendLayout();
            // 
            // GBChapters
            // 
            this.GBChapters.Controls.Add(this.LInfo);
            this.GBChapters.Controls.Add(this.Chapters);
            resources.ApplyResources(this.GBChapters, "GBChapters");
            this.GBChapters.Name = "GBChapters";
            this.GBChapters.TabStop = false;
            // 
            // LInfo
            // 
            resources.ApplyResources(this.LInfo, "LInfo");
            this.LInfo.Name = "LInfo";
            // 
            // Chapters
            // 
            resources.ApplyResources(this.Chapters, "Chapters");
            this.Chapters.Name = "Chapters";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            // 
            // TChapterForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.GBChapters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TChapterForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.TChapterForm_Load);
            this.GBChapters.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Stores the selected chapters to the dictionary and closes the form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, System.EventArgs e)
        {
            // Only start the query when at least one chapter has been selected 
            if (Chapters.GetItemCount() == 0)
            {
                MessageBox.Show(Resources.CHAPTER_NO_CARDS_SELECTED_ERROR_DESC, Resources.CHAPTER_NO_CARDS_SELECTED_ERROR_CAPTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Copy Query chapters
                Dictionary.QueryChapters.Clear();
                (Dictionary.QueryChapters as ObservableList<int>).AddRange(Chapters.SelChapters);

                if (Dictionary.Cards.ActiveCardsCount == 0)
                {
                    MessageBox.Show(Resources.CHAPTER_NO_CARDS_SELECTED_ERROR_DESC, Resources.CHAPTER_NO_CARDS_SELECTED_ERROR_CAPTION,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary.Save();
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Loads the chapters into the DragAndDropLists when the form is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TChapterForm_Load(object sender, System.EventArgs e)
        {
            if (Dictionary.Chapters.Chapters.Count == 1)
            {
                if (!Dictionary.QueryChapters.Contains(Dictionary.Chapters.Chapters[0].Id))
                    Dictionary.QueryChapters.Add(Dictionary.Chapters.Chapters[0].Id);

                Close();
                MessageBox.Show(Resources.CHAPTERS_ONE_TEXT, Resources.CHAPTERS_ONE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Chapters.Prepare(Dictionary, true);
        }
    }
}

