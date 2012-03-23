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
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.AuthentificationControl = new MLifter.Controls.UserAuthControl();
            this.SuspendLayout();
            // 
            // AuthentificationControl
            // 
            this.AuthentificationControl.AllowAutoLogin = true;
            this.AuthentificationControl.BottomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.AuthentificationControl.BottomColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            resources.ApplyResources(this.AuthentificationControl, "AuthentificationControl");
            this.AuthentificationControl.LoginPicture = global::MLifter.Controls.Properties.Resources.login_48;
            this.AuthentificationControl.MinimumSize = new System.Drawing.Size(260, 105);
            this.AuthentificationControl.Name = "AuthentificationControl";
            this.AuthentificationControl.Login_Click += new System.EventHandler<MLifter.Controls.UserLoginEventArgs>(this.AuthentificationControl_Login_Click);
            this.AuthentificationControl.SizeChanged += new System.EventHandler(this.AuthentificationControl_SizeChanged);
            this.AuthentificationControl.Cancel_Clicked += new System.EventHandler(this.AuthentificationControl_Cancel_Clicked);
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.AuthentificationControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private UserAuthControl AuthentificationControl;
    }
}
