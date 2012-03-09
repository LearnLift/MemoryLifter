namespace MLifter.Controls.LearningWindow
{
    partial class AnswerPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnswerPanel));
            this.gradientPanelAnswer = new MLifter.Components.GradientPanel();
            this.dockingButtonDontAskAgain = new MLifter.Components.DockingButton();
            this.panelMultiPane = new System.Windows.Forms.Panel();
            this.multiPaneControlMain = new Kerido.Controls.MultiPaneControl();
            this.multiPanePageMainTextbox = new Kerido.Controls.MultiPanePage();
            this.panelTextBox = new System.Windows.Forms.Panel();
            this.mLifterTextBox = new MLifter.Components.MLifterTextBox();
            this.multiPanePageMainViewer = new Kerido.Controls.MultiPanePage();
            this.webBrowserAnswer = new MLifter.Components.MLifterWebBrowser();
            this.multiPanePageMainMultipleChoice = new Kerido.Controls.MultiPanePage();
            this.labelAnswer = new System.Windows.Forms.Label();
            this.multipleChoice = new MLifter.Controls.MultipleChoice();
            this.gradientPanelAnswer.SuspendLayout();
            this.panelMultiPane.SuspendLayout();
            this.multiPaneControlMain.SuspendLayout();
            this.multiPanePageMainTextbox.SuspendLayout();
            this.panelTextBox.SuspendLayout();
            this.multiPanePageMainViewer.SuspendLayout();
            this.multiPanePageMainMultipleChoice.SuspendLayout();
            this.SuspendLayout();
            // 
            // gradientPanelAnswer
            // 
            this.gradientPanelAnswer.Controls.Add(this.dockingButtonDontAskAgain);
            this.gradientPanelAnswer.Controls.Add(this.panelMultiPane);
            this.gradientPanelAnswer.Controls.Add(this.labelAnswer);
            resources.ApplyResources(this.gradientPanelAnswer, "gradientPanelAnswer");
            this.gradientPanelAnswer.Gradient = null;
            this.gradientPanelAnswer.LayoutSuspended = false;
            this.gradientPanelAnswer.Name = "gradientPanelAnswer";
            // 
            // dockingButtonDontAskAgain
            // 
            resources.ApplyResources(this.dockingButtonDontAskAgain, "dockingButtonDontAskAgain");
            this.dockingButtonDontAskAgain.DockingParent = this.panelMultiPane;
            this.dockingButtonDontAskAgain.ForeColor = System.Drawing.Color.Maroon;
            this.dockingButtonDontAskAgain.Name = "dockingButtonDontAskAgain";
            this.dockingButtonDontAskAgain.Click += new System.EventHandler(this.dockingButtonDontAskAgain_Click);
            // 
            // panelMultiPane
            // 
            resources.ApplyResources(this.panelMultiPane, "panelMultiPane");
            this.panelMultiPane.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMultiPane.Controls.Add(this.multiPaneControlMain);
            this.panelMultiPane.Name = "panelMultiPane";
            // 
            // multiPaneControlMain
            // 
            this.multiPaneControlMain.Controls.Add(this.multiPanePageMainTextbox);
            this.multiPaneControlMain.Controls.Add(this.multiPanePageMainViewer);
            this.multiPaneControlMain.Controls.Add(this.multiPanePageMainMultipleChoice);
            resources.ApplyResources(this.multiPaneControlMain, "multiPaneControlMain");
            this.multiPaneControlMain.Name = "multiPaneControlMain";
            this.multiPaneControlMain.SelectedPage = this.multiPanePageMainTextbox;
            this.multiPaneControlMain.SelectedPageChanging += new System.EventHandler(this.multiPaneControlMain_SelectedPageChanging);
            this.multiPaneControlMain.SelectedPageChanged += new System.EventHandler(this.multiPaneControlMain_SelectedPageChanged);
            // 
            // multiPanePageMainTextbox
            // 
            this.multiPanePageMainTextbox.Controls.Add(this.panelTextBox);
            this.multiPanePageMainTextbox.Name = "multiPanePageMainTextbox";
            resources.ApplyResources(this.multiPanePageMainTextbox, "multiPanePageMainTextbox");
            // 
            // panelTextBox
            // 
            this.panelTextBox.Controls.Add(this.mLifterTextBox);
            resources.ApplyResources(this.panelTextBox, "panelTextBox");
            this.panelTextBox.Name = "panelTextBox";
            // 
            // mLifterTextBox
            // 
            this.mLifterTextBox.AllowDrop = true;
            resources.ApplyResources(this.mLifterTextBox, "mLifterTextBox");
            this.mLifterTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.mLifterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mLifterTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.mLifterTextBox.MaximumSize = new System.Drawing.Size(493, 409);
            this.mLifterTextBox.MinimumSize = new System.Drawing.Size(493, 0);
            this.mLifterTextBox.Name = "mLifterTextBox";
            this.mLifterTextBox.Text = global::MLifter.Controls.Properties.Resources.TOOLTIP_FRMCHAPTER_LBCHAPTERS;
            this.mLifterTextBox.TippBackColor = System.Drawing.Color.Empty;
            this.mLifterTextBox.TippForeColor = System.Drawing.Color.Empty;
            // 
            // multiPanePageMainViewer
            // 
            this.multiPanePageMainViewer.Controls.Add(this.webBrowserAnswer);
            this.multiPanePageMainViewer.Name = "multiPanePageMainViewer";
            resources.ApplyResources(this.multiPanePageMainViewer, "multiPanePageMainViewer");
            // 
            // webBrowserAnswer
            // 
            resources.ApplyResources(this.webBrowserAnswer, "webBrowserAnswer");
            this.webBrowserAnswer.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserAnswer.MinimumSize = new System.Drawing.Size(27, 25);
            this.webBrowserAnswer.Name = "webBrowserAnswer";
            this.webBrowserAnswer.ScriptErrorsSuppressed = true;
            this.webBrowserAnswer.WebBrowserShortcutsEnabled = false;
            this.webBrowserAnswer.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserAnswer_Navigating);
            // 
            // multiPanePageMainMultipleChoice
            // 
            this.multiPanePageMainMultipleChoice.Controls.Add(this.multipleChoice);
            this.multiPanePageMainMultipleChoice.Name = "multiPanePageMainMultipleChoice";
            resources.ApplyResources(this.multiPanePageMainMultipleChoice, "multiPanePageMainMultipleChoice");
            // 
            // labelAnswer
            // 
            this.labelAnswer.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelAnswer, "labelAnswer");
            this.labelAnswer.ForeColor = System.Drawing.Color.White;
            this.labelAnswer.Name = "labelAnswer";
            // 
            // multipleChoice
            // 
            this.multipleChoice.BackColor = System.Drawing.SystemColors.Window;
            this.multipleChoice.Choices = null;
            resources.ApplyResources(this.multipleChoice, "multipleChoice");
            this.multipleChoice.Name = "multipleChoice";
            this.multipleChoice.Options = null;
            // 
            // AnswerPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.gradientPanelAnswer);
            this.Name = "AnswerPanel";
            resources.ApplyResources(this, "$this");
            this.gradientPanelAnswer.ResumeLayout(false);
            this.panelMultiPane.ResumeLayout(false);
            this.multiPaneControlMain.ResumeLayout(false);
            this.multiPanePageMainTextbox.ResumeLayout(false);
            this.panelTextBox.ResumeLayout(false);
            this.multiPanePageMainViewer.ResumeLayout(false);
            this.multiPanePageMainMultipleChoice.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MLifter.Components.DockingButton dockingButtonDontAskAgain;
        private MLifter.Components.GradientPanel gradientPanelAnswer;
        private Kerido.Controls.MultiPaneControl multiPaneControlMain;
        private Kerido.Controls.MultiPanePage multiPanePageMainViewer;
        private MLifter.Components.MLifterWebBrowser webBrowserAnswer;
        private Kerido.Controls.MultiPanePage multiPanePageMainTextbox;
        private MLifter.Components.MLifterTextBox mLifterTextBox;
        private Kerido.Controls.MultiPanePage multiPanePageMainMultipleChoice;
        private MultipleChoice multipleChoice;
        private System.Windows.Forms.Panel panelTextBox;
        private System.Windows.Forms.Panel panelMultiPane;
        private System.Windows.Forms.Label labelAnswer;
    }
}
