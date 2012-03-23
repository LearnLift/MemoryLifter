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
namespace MLifter.Controls.Wizards.Print
{
    partial class PrintPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintPage));
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxStyle = new System.Windows.Forms.ComboBox();
			this.buttonSettings = new System.Windows.Forms.Button();
			this.comboBoxSorting = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonPreview = new System.Windows.Forms.Button();
			this.buttonPrint = new System.Windows.Forms.Button();
			this.checkBoxReverseOrder = new System.Windows.Forms.CheckBox();
			this.Browser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// comboBoxStyle
			// 
			this.comboBoxStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStyle.FormattingEnabled = true;
			resources.ApplyResources(this.comboBoxStyle, "comboBoxStyle");
			this.comboBoxStyle.Name = "comboBoxStyle";
			// 
			// buttonSettings
			// 
			this.buttonSettings.Image = global::MLifter.Controls.Properties.Resources.documentProperties;
			resources.ApplyResources(this.buttonSettings, "buttonSettings");
			this.buttonSettings.Name = "buttonSettings";
			this.buttonSettings.UseVisualStyleBackColor = true;
			this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
			// 
			// comboBoxSorting
			// 
			this.comboBoxSorting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSorting.FormattingEnabled = true;
			this.comboBoxSorting.Items.AddRange(new object[] {
            resources.GetString("comboBoxSorting.Items"),
            resources.GetString("comboBoxSorting.Items1"),
            resources.GetString("comboBoxSorting.Items2")});
			resources.ApplyResources(this.comboBoxSorting, "comboBoxSorting");
			this.comboBoxSorting.Name = "comboBoxSorting";
			this.comboBoxSorting.SelectedIndexChanged += new System.EventHandler(this.comboBoxSorting_SelectedIndexChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// buttonPreview
			// 
			this.buttonPreview.Image = global::MLifter.Controls.Properties.Resources.documentPrintPreview;
			resources.ApplyResources(this.buttonPreview, "buttonPreview");
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.UseVisualStyleBackColor = true;
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			// 
			// buttonPrint
			// 
			this.buttonPrint.Image = global::MLifter.Controls.Properties.Resources.documentPrint;
			resources.ApplyResources(this.buttonPrint, "buttonPrint");
			this.buttonPrint.Name = "buttonPrint";
			this.buttonPrint.UseVisualStyleBackColor = true;
			this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
			// 
			// checkBoxReverseOrder
			// 
			resources.ApplyResources(this.checkBoxReverseOrder, "checkBoxReverseOrder");
			this.checkBoxReverseOrder.Name = "checkBoxReverseOrder";
			this.checkBoxReverseOrder.UseVisualStyleBackColor = true;
			// 
			// Browser
			// 
			this.Browser.AllowWebBrowserDrop = false;
			this.Browser.IsWebBrowserContextMenuEnabled = false;
			resources.ApplyResources(this.Browser, "Browser");
			this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
			this.Browser.Name = "Browser";
			this.Browser.WebBrowserShortcutsEnabled = false;
			this.Browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Browser_DocumentCompleted);
			// 
			// PrintPage
			// 
			this.Controls.Add(this.checkBoxReverseOrder);
			this.Controls.Add(this.buttonPrint);
			this.Controls.Add(this.buttonPreview);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxSorting);
			this.Controls.Add(this.buttonSettings);
			this.Controls.Add(this.comboBoxStyle);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Browser);
			resources.ApplyResources(this, "$this");
			this.HelpAvailable = true;
			this.Name = "PrintPage";
			this.NextAllowed = false;
			this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
			this.Load += new System.EventHandler(this.PrintPage_Load);
			this.Controls.SetChildIndex(this.Browser, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.comboBoxStyle, 0);
			this.Controls.SetChildIndex(this.buttonSettings, 0);
			this.Controls.SetChildIndex(this.comboBoxSorting, 0);
			this.Controls.SetChildIndex(this.label2, 0);
			this.Controls.SetChildIndex(this.buttonPreview, 0);
			this.Controls.SetChildIndex(this.buttonPrint, 0);
			this.Controls.SetChildIndex(this.checkBoxReverseOrder, 0);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxStyle;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.ComboBox comboBoxSorting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.CheckBox checkBoxReverseOrder;
        private System.Windows.Forms.WebBrowser Browser;
    }
}
