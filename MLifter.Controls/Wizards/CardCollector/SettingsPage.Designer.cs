namespace MLifter.Controls.Wizards.CardCollector
{
    partial class SettingsPage
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPage));
            this.labelDescription = new System.Windows.Forms.Label();
            this.listViewElements = new DragNDrop.DragAndDropListView();
            this.column = new System.Windows.Forms.ColumnHeader();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.labelHowTo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            // 
            // listViewElements
            // 
            this.listViewElements.AllowDrop = true;
            this.listViewElements.AllowReorder = true;
            this.listViewElements.CheckBoxes = true;
            this.listViewElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column});
            resources.ApplyResources(this.listViewElements, "listViewElements");
            this.listViewElements.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewElements.HideSelection = false;
            this.listViewElements.LineColor = System.Drawing.Color.Red;
            this.listViewElements.MultiSelect = false;
            this.listViewElements.Name = "listViewElements";
            this.listViewElements.UseCompatibleStateImageBehavior = false;
            this.listViewElements.View = System.Windows.Forms.View.Details;
            this.listViewElements.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewElements_ItemChecked);
            // 
            // column
            // 
            resources.ApplyResources(this.column, "column");
            // 
            // buttonUp
            // 
            this.buttonUp.Image = global::MLifter.Controls.Properties.Resources.goUp;
            resources.ApplyResources(this.buttonUp, "buttonUp");
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Image = global::MLifter.Controls.Properties.Resources.goDown;
            resources.ApplyResources(this.buttonDown, "buttonDown");
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // labelHowTo
            // 
            resources.ApplyResources(this.labelHowTo, "labelHowTo");
            this.labelHowTo.Name = "labelHowTo";
            // 
            // SettingsPage
            // 
            this.Controls.Add(this.labelHowTo);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.listViewElements);
            this.Controls.Add(this.labelDescription);
            resources.ApplyResources(this, "$this");
            this.HelpAvailable = true;
            this.LastStep = true;
            this.Name = "SettingsPage";
            this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
            this.Load += new System.EventHandler(this.SettingsPage_Load);
            this.Controls.SetChildIndex(this.labelDescription, 0);
            this.Controls.SetChildIndex(this.listViewElements, 0);
            this.Controls.SetChildIndex(this.buttonUp, 0);
            this.Controls.SetChildIndex(this.buttonDown, 0);
            this.Controls.SetChildIndex(this.labelHowTo, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Label labelHowTo;
        public DragNDrop.DragAndDropListView listViewElements;
        private System.Windows.Forms.ColumnHeader column;

    }
}
