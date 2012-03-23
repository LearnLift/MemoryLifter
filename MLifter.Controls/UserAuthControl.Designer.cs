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
    partial class UserAuthControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserAuthControl));
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelUsername = new System.Windows.Forms.Label();
			this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
			this.comboBoxUserSelection = new System.Windows.Forms.ComboBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelHeader = new System.Windows.Forms.Label();
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.panelBackColor = new System.Windows.Forms.Panel();
			this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
			this.checkBoxSaveUsername = new System.Windows.Forms.CheckBox();
			this.checkBoxSavePassword = new System.Windows.Forms.CheckBox();
			this.checkBoxAutoLogin = new System.Windows.Forms.CheckBox();
			this.buttonShowHideOptions = new System.Windows.Forms.Button();
			this.buttonCancel = new MLifter.Controls.LinklabelButton();
			this.bLogin = new MLifter.Controls.LinklabelButton();
			this.tableLayoutPanelMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.panelBackColor.SuspendLayout();
			this.tableLayoutPanelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelPassword
			// 
			resources.ApplyResources(this.labelPassword, "labelPassword");
			this.labelPassword.Name = "labelPassword";
			// 
			// labelUsername
			// 
			resources.ApplyResources(this.labelUsername, "labelUsername");
			this.labelUsername.Name = "labelUsername";
			// 
			// tableLayoutPanelMain
			// 
			resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
			this.tableLayoutPanelMain.Controls.Add(this.comboBoxUserSelection, 2, 1);
			this.tableLayoutPanelMain.Controls.Add(this.textBoxPassword, 2, 2);
			this.tableLayoutPanelMain.Controls.Add(this.labelUsername, 1, 1);
			this.tableLayoutPanelMain.Controls.Add(this.labelPassword, 1, 2);
			this.tableLayoutPanelMain.Controls.Add(this.labelHeader, 1, 0);
			this.tableLayoutPanelMain.Controls.Add(this.pictureBoxIcon, 0, 0);
			this.tableLayoutPanelMain.Controls.Add(this.panelBackColor, 0, 3);
			this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
			// 
			// comboBoxUserSelection
			// 
			resources.ApplyResources(this.comboBoxUserSelection, "comboBoxUserSelection");
			this.comboBoxUserSelection.FormattingEnabled = true;
			this.comboBoxUserSelection.Name = "comboBoxUserSelection";
			this.comboBoxUserSelection.SelectedIndexChanged += new System.EventHandler(this.lbUserSelection_SelectedIndexChanged);
			this.comboBoxUserSelection.Leave += new System.EventHandler(this.comboBoxUserSelection_Leave);
			this.comboBoxUserSelection.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPassword_KeyDown);
			this.comboBoxUserSelection.TextChanged += new System.EventHandler(this.comboBoxUserSelection_TextChanged);
			// 
			// textBoxPassword
			// 
			resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.UseSystemPasswordChar = true;
			this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
			this.textBoxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPassword_KeyDown);
			// 
			// labelHeader
			// 
			resources.ApplyResources(this.labelHeader, "labelHeader");
			this.labelHeader.AutoEllipsis = true;
			this.tableLayoutPanelMain.SetColumnSpan(this.labelHeader, 2);
			this.labelHeader.Name = "labelHeader";
			// 
			// pictureBoxIcon
			// 
			resources.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.tableLayoutPanelMain.SetRowSpan(this.pictureBoxIcon, 3);
			this.pictureBoxIcon.TabStop = false;
			// 
			// panelBackColor
			// 
			this.tableLayoutPanelMain.SetColumnSpan(this.panelBackColor, 3);
			this.panelBackColor.Controls.Add(this.tableLayoutPanelBottom);
			resources.ApplyResources(this.panelBackColor, "panelBackColor");
			this.panelBackColor.Name = "panelBackColor";
			// 
			// tableLayoutPanelBottom
			// 
			resources.ApplyResources(this.tableLayoutPanelBottom, "tableLayoutPanelBottom");
			this.tableLayoutPanelBottom.Controls.Add(this.buttonCancel, 2, 0);
			this.tableLayoutPanelBottom.Controls.Add(this.bLogin, 1, 0);
			this.tableLayoutPanelBottom.Controls.Add(this.checkBoxSaveUsername, 0, 1);
			this.tableLayoutPanelBottom.Controls.Add(this.checkBoxSavePassword, 0, 2);
			this.tableLayoutPanelBottom.Controls.Add(this.checkBoxAutoLogin, 0, 3);
			this.tableLayoutPanelBottom.Controls.Add(this.buttonShowHideOptions, 0, 0);
			this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
			// 
			// checkBoxSaveUsername
			// 
			resources.ApplyResources(this.checkBoxSaveUsername, "checkBoxSaveUsername");
			this.tableLayoutPanelBottom.SetColumnSpan(this.checkBoxSaveUsername, 3);
			this.checkBoxSaveUsername.Name = "checkBoxSaveUsername";
			this.checkBoxSaveUsername.UseVisualStyleBackColor = true;
			this.checkBoxSaveUsername.CheckedChanged += new System.EventHandler(this.checkBoxSaveUsername_CheckedChanged);
			// 
			// checkBoxSavePassword
			// 
			resources.ApplyResources(this.checkBoxSavePassword, "checkBoxSavePassword");
			this.tableLayoutPanelBottom.SetColumnSpan(this.checkBoxSavePassword, 3);
			this.checkBoxSavePassword.Name = "checkBoxSavePassword";
			this.checkBoxSavePassword.UseVisualStyleBackColor = true;
			this.checkBoxSavePassword.CheckedChanged += new System.EventHandler(this.checkBoxSavePassword_CheckedChanged);
			// 
			// checkBoxAutoLogin
			// 
			resources.ApplyResources(this.checkBoxAutoLogin, "checkBoxAutoLogin");
			this.tableLayoutPanelBottom.SetColumnSpan(this.checkBoxAutoLogin, 3);
			this.checkBoxAutoLogin.Name = "checkBoxAutoLogin";
			this.checkBoxAutoLogin.UseVisualStyleBackColor = true;
			// 
			// buttonShowHideOptions
			// 
			resources.ApplyResources(this.buttonShowHideOptions, "buttonShowHideOptions");
			this.buttonShowHideOptions.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.buttonShowHideOptions.FlatAppearance.BorderSize = 0;
			this.buttonShowHideOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonShowHideOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonShowHideOptions.Image = global::MLifter.Controls.Properties.Resources.arrow_down_bw;
			this.buttonShowHideOptions.Name = "buttonShowHideOptions";
			this.buttonShowHideOptions.UseVisualStyleBackColor = true;
			this.buttonShowHideOptions.MouseLeave += new System.EventHandler(this.buttonShowHideOptions_MouseLeave);
			this.buttonShowHideOptions.Click += new System.EventHandler(this.buttonShowHideOptions_Click);
			this.buttonShowHideOptions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonShowHideOptions_MouseDown);
			this.buttonShowHideOptions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonShowHideOptions_MouseUp);
			this.buttonShowHideOptions.MouseEnter += new System.EventHandler(this.buttonShowHideOptions_MouseEnter);
			// 
			// buttonCancel
			// 
			this.buttonCancel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.None;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// bLogin
			// 
			this.bLogin.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.bLogin.DialogResult = System.Windows.Forms.DialogResult.None;
			resources.ApplyResources(this.bLogin, "bLogin");
			this.bLogin.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.bLogin.Name = "bLogin";
			this.bLogin.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
			this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
			// 
			// UserAuthControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanelMain);
			this.MinimumSize = new System.Drawing.Size(280, 100);
			this.Name = "UserAuthControl";
			this.Load += new System.EventHandler(this.UserAuthControl_Load);
			this.tableLayoutPanelMain.ResumeLayout(false);
			this.tableLayoutPanelMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.panelBackColor.ResumeLayout(false);
			this.tableLayoutPanelBottom.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
		private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.CheckBox checkBoxSaveUsername;
        private System.Windows.Forms.CheckBox checkBoxSavePassword;
        private System.Windows.Forms.CheckBox checkBoxAutoLogin;
        private LinklabelButton bLogin;
        private LinklabelButton buttonCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Panel panelBackColor;
		private System.Windows.Forms.Button buttonShowHideOptions;
		private System.Windows.Forms.ComboBox comboBoxUserSelection;
		private System.Windows.Forms.TextBox textBoxPassword;
    }
}
