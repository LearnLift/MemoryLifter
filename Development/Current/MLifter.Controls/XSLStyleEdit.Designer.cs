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
namespace MLifter.Controls
{
    partial class XSLStyleEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XSLStyleEdit));
            this.stylePreview = new MLifter.Components.MLifterWebBrowser();
            this.tableXSL = new XPTable.Models.Table();
            this.columnModelXSL = new XPTable.Models.ColumnModel();
            this.textColumnElementName = new XPTable.Models.TextColumn();
            this.colorColumnColor = new XPTable.Models.ColorColumn();
            this.colorColumnBackColor = new XPTable.Models.ColorColumn();
            this.buttonColumnFont = new XPTable.Models.ButtonColumn();
            this.comboBoxColumnHAlign = new XPTable.Models.ComboBoxColumn();
            this.comboBoxColumnVAlign = new XPTable.Models.ComboBoxColumn();
            this.tableModelXSL = new XPTable.Models.TableModel();
            this.tabControlMode = new System.Windows.Forms.TabControl();
            this.tabPageTable = new System.Windows.Forms.TabPage();
            this.tabPageText = new System.Windows.Forms.TabPage();
            this.textBoxFile = new MLifter.Controls.SyntaxHighlightingTextBox.SyntaxHighlightingTextBox();
            this.timerTextChanged = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tableXSL)).BeginInit();
            this.tabControlMode.SuspendLayout();
            this.tabPageTable.SuspendLayout();
            this.tabPageText.SuspendLayout();
            this.SuspendLayout();
            // 
            // stylePreview
            // 
            resources.ApplyResources(this.stylePreview, "stylePreview");
            this.stylePreview.IsWebBrowserContextMenuEnabled = false;
            this.stylePreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.stylePreview.Name = "stylePreview";
            this.stylePreview.WebBrowserShortcutsEnabled = false;
            this.stylePreview.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.stylePreview_PreviewKeyDown);
            // 
            // tableXSL
            // 
            this.tableXSL.AlternatingRowColor = System.Drawing.Color.LightYellow;
            this.tableXSL.ColumnModel = this.columnModelXSL;
            resources.ApplyResources(this.tableXSL, "tableXSL");
            this.tableXSL.GridLines = XPTable.Models.GridLines.Both;
            this.tableXSL.Name = "tableXSL";
            this.tableXSL.NoItemsText = "There are no items in this view.";
            this.tableXSL.TableModel = this.tableModelXSL;
            this.tableXSL.CellButtonClicked += new XPTable.Events.CellButtonEventHandler(this.tableXSL_CellButtonClicked);
            // 
            // columnModelXSL
            // 
            this.columnModelXSL.Columns.AddRange(new XPTable.Models.Column[] {
            this.textColumnElementName,
            this.colorColumnColor,
            this.colorColumnBackColor,
            this.buttonColumnFont,
            this.comboBoxColumnHAlign,
            this.comboBoxColumnVAlign});
            // 
            // textColumnElementName
            // 
            this.textColumnElementName.Alignment = XPTable.Models.ColumnAlignment.Center;
            this.textColumnElementName.Editable = false;
            this.textColumnElementName.Text = "Element";
            // 
            // colorColumnColor
            // 
            this.colorColumnColor.Text = "Color";
            // 
            // colorColumnBackColor
            // 
            this.colorColumnBackColor.Text = "Back-Color";
            // 
            // buttonColumnFont
            // 
            this.buttonColumnFont.Text = "Font";
            // 
            // comboBoxColumnHAlign
            // 
            this.comboBoxColumnHAlign.Text = "H-Align";
            // 
            // comboBoxColumnVAlign
            // 
            this.comboBoxColumnVAlign.Text = "V-Align";
            // 
            // tableModelXSL
            // 
            this.tableModelXSL.RowHeight = 18;
            // 
            // tabControlMode
            // 
            resources.ApplyResources(this.tabControlMode, "tabControlMode");
            this.tabControlMode.Controls.Add(this.tabPageTable);
            this.tabControlMode.Controls.Add(this.tabPageText);
            this.tabControlMode.Name = "tabControlMode";
            this.tabControlMode.SelectedIndex = 0;
            this.tabControlMode.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlMode.SelectedIndexChanged += new System.EventHandler(this.tabControlMode_SelectedIndexChanged);
            // 
            // tabPageTable
            // 
            this.tabPageTable.Controls.Add(this.tableXSL);
            resources.ApplyResources(this.tabPageTable, "tabPageTable");
            this.tabPageTable.Name = "tabPageTable";
            this.tabPageTable.UseVisualStyleBackColor = true;
            // 
            // tabPageText
            // 
            this.tabPageText.Controls.Add(this.textBoxFile);
            resources.ApplyResources(this.tabPageText, "tabPageText");
            this.tabPageText.Name = "tabPageText";
            this.tabPageText.UseVisualStyleBackColor = true;
            // 
            // textBoxFile
            // 
            this.textBoxFile.AcceptsTab = true;
            this.textBoxFile.CaseSensitive = false;
            resources.ApplyResources(this.textBoxFile, "textBoxFile");
            this.textBoxFile.FilterAutoComplete = false;
            this.textBoxFile.MaxUndoRedoSteps = 50;
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Text = global::MLifter.Controls.Properties.Resources.TOOLTIP_FRMFIELDS_LISTBOXSELFIELDS;
            this.textBoxFile.Leave += new System.EventHandler(this.textBoxFile_Leave);
            this.textBoxFile.TextChanged += new System.EventHandler(this.textBoxFile_TextChanged);
            // 
            // timerTextChanged
            // 
            this.timerTextChanged.Interval = 3000;
            this.timerTextChanged.Tick += new System.EventHandler(this.timerTextChanged_Tick);
            // 
            // XSLStyleEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.stylePreview);
            this.Controls.Add(this.tabControlMode);
            this.Name = "XSLStyleEdit";
            resources.ApplyResources(this, "$this");
            this.Resize += new System.EventHandler(this.XSLStyleEdit_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.tableXSL)).EndInit();
            this.tabControlMode.ResumeLayout(false);
            this.tabPageTable.ResumeLayout(false);
            this.tabPageText.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MLifter.Components.MLifterWebBrowser stylePreview;
        private XPTable.Models.Table tableXSL;
        private XPTable.Models.TableModel tableModelXSL;
        private XPTable.Models.ColumnModel columnModelXSL;
        private XPTable.Models.TextColumn textColumnElementName;
        private XPTable.Models.ColorColumn colorColumnColor;
        private System.Windows.Forms.TabControl tabControlMode;
        private System.Windows.Forms.TabPage tabPageText;
        private System.Windows.Forms.TabPage tabPageTable;
        private MLifter.Controls.SyntaxHighlightingTextBox.SyntaxHighlightingTextBox textBoxFile;
        private XPTable.Models.ColorColumn colorColumnBackColor;
        private XPTable.Models.ButtonColumn buttonColumnFont;
        private XPTable.Models.ComboBoxColumn comboBoxColumnHAlign;
        private XPTable.Models.ComboBoxColumn comboBoxColumnVAlign;
        private System.Windows.Forms.Timer timerTextChanged;
    }
}
