namespace MLifter.Controls.LearningModulesPage
{
    partial class LearningModulesTreeViewControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearningModulesTreeViewControl));
            this.checkBoxShowLearningModulesOfSubFolder = new System.Windows.Forms.CheckBox();
            this.treeViewLearnModules = new MLifter.Components.FolderTreeView();
            this.linkLabelLogout = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // checkBoxShowLearningModulesOfSubFolder
            // 
            this.checkBoxShowLearningModulesOfSubFolder.AllowDrop = true;
            resources.ApplyResources(this.checkBoxShowLearningModulesOfSubFolder, "checkBoxShowLearningModulesOfSubFolder");
            this.checkBoxShowLearningModulesOfSubFolder.Name = "checkBoxShowLearningModulesOfSubFolder";
            this.checkBoxShowLearningModulesOfSubFolder.UseVisualStyleBackColor = true;
            this.checkBoxShowLearningModulesOfSubFolder.CheckedChanged += new System.EventHandler(this.checkBoxShowLearningModulesOfSubFolder_CheckedChanged);
            // 
            // treeViewLearnModules
            // 
            this.treeViewLearnModules.AllowDrop = true;
            resources.ApplyResources(this.treeViewLearnModules, "treeViewLearnModules");
            this.treeViewLearnModules.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewLearnModules.HideSelection = false;
            this.treeViewLearnModules.Name = "treeViewLearnModules";
            this.treeViewLearnModules.SelectedFolder = null;
            this.treeViewLearnModules.SelectedNode = null;
            this.treeViewLearnModules.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewLearnModules_DragDrop);
            this.treeViewLearnModules.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewLearnModules_AfterSelect);
            this.treeViewLearnModules.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewLearnModules_DragEnter);
            this.treeViewLearnModules.ContentLoadException += new MLifter.BusinessLayer.FolderIndexEntry.ContentLoadExceptionEventHandler(this.treeViewLearnModules_ContentLoadException);
            // 
            // linkLabelLogout
            // 
            resources.ApplyResources(this.linkLabelLogout, "linkLabelLogout");
            this.linkLabelLogout.Name = "linkLabelLogout";
            this.linkLabelLogout.TabStop = true;
            this.linkLabelLogout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLogout_LinkClicked);
            // 
            // LearningModulesTreeViewControl
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.linkLabelLogout);
            this.Controls.Add(this.treeViewLearnModules);
            this.Controls.Add(this.checkBoxShowLearningModulesOfSubFolder);
            resources.ApplyResources(this, "$this");
            this.Name = "LearningModulesTreeViewControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowLearningModulesOfSubFolder;
        private MLifter.Components.FolderTreeView treeViewLearnModules;
        private System.Windows.Forms.LinkLabel linkLabelLogout;
    }
}
