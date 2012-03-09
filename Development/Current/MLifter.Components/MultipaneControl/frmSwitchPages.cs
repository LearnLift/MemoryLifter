/*
* Copyright (c) 2007, KO Software (official@koapproach.com)
*
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
*     * All original and modified versions of this source code must include the
*       above copyright notice, this list of conditions and the following
*       disclaimer.
*
*     * This code may not be used with or within any modules or code that is 
*       licensed in any way that that compels or requires users or modifiers
*       to release their source code or changes as a requirement for
*       the use, modification or distribution of binary, object or source code
*       based on the licensed source code. (ex: Cannot be used with GPL code.)
*
*     * The name of KO Software may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY KO SOFTWARE ``AS IS'' AND ANY EXPRESS OR
* IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
* EVENT WILL KO SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
* OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
* WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
* OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
* ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Security;
using System.Security.Permissions;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Resources;

using Kerido.Controls;

namespace Kerido.Controls.Design
{
	public class frmSwitchPages : Form
	{
		private System.Windows.Forms.Label myCtlLblSwitchPage;
		private System.Windows.Forms.ComboBox myCtlCmbItems;
		private System.Windows.Forms.CheckBox myCtlChkSetSelectedPage;
		private System.Windows.Forms.Button myCtlBtnOK;
		private System.Windows.Forms.Button myCtlBtnCancel;


		MultiPaneControlDesigner myDesigner;
		MultiPanePage myFutureSelectedItem;
		bool mySetSelectedPage;

		class MultiPanePageItem
		{
			MultiPanePage myPage;

			public MultiPanePageItem(MultiPanePage thePg)
			{
				myPage = thePg;
			}

			public MultiPanePage Page { get { return myPage; } }

			public override string ToString()
			{
				return myPage.Name;
			}
		}

		public frmSwitchPages(MultiPaneControlDesigner theDesigner)
		{
			myDesigner = theDesigner;
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			foreach (MultiPanePage aIt in myDesigner.DesignedControl.Controls)
			{
				MultiPanePageItem aItem = new MultiPanePageItem(aIt);
				myCtlCmbItems.Items.Add(aItem);

				if (myDesigner.DesignerSelectedPage == aIt)
					myCtlCmbItems.SelectedItem = aItem;
			}

		}

		public MultiPanePage FutureSelection
		{
			get { return myFutureSelectedItem; }
		}

		public bool SetSelectedPage
		{
			get { return mySetSelectedPage; }
		}

		private void Handler_OK(object sender, EventArgs e)
		{
			myFutureSelectedItem = ((MultiPanePageItem)myCtlCmbItems.SelectedItem).Page;
			mySetSelectedPage = myCtlChkSetSelectedPage.Checked;
		}

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
			this.myCtlLblSwitchPage = new System.Windows.Forms.Label();
			this.myCtlCmbItems = new System.Windows.Forms.ComboBox();
			this.myCtlChkSetSelectedPage = new System.Windows.Forms.CheckBox();
			this.myCtlBtnOK = new System.Windows.Forms.Button();
			this.myCtlBtnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// myCtlLblSwitchPage
			// 
			this.myCtlLblSwitchPage.AutoSize = true;
			this.myCtlLblSwitchPage.Location = new System.Drawing.Point(9, 9);
			this.myCtlLblSwitchPage.Name = "myCtlLblSwitchPage";
			this.myCtlLblSwitchPage.TabIndex = 0;
			this.myCtlLblSwitchPage.Text = "Switch the page to:";
			// 
			// myCtlCmbItems
			// 
			this.myCtlCmbItems.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.myCtlCmbItems.Location = new System.Drawing.Point(12, 25);
			this.myCtlCmbItems.Name = "myCtlCmbItems";
			this.myCtlCmbItems.Size = new System.Drawing.Size(227, 21);
			this.myCtlCmbItems.TabIndex = 1;
			// 
			// myCtlChkSetSelectedPage
			// 
			this.myCtlChkSetSelectedPage.Location = new System.Drawing.Point(12, 61);
			this.myCtlChkSetSelectedPage.Name = "myCtlChkSetSelectedPage";
			this.myCtlChkSetSelectedPage.Size = new System.Drawing.Size(220, 17);
			this.myCtlChkSetSelectedPage.TabIndex = 2;
			this.myCtlChkSetSelectedPage.Text = "Also set the SelectedPage property";
			// 
			// myCtlBtnOK
			// 
			this.myCtlBtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.myCtlBtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.myCtlBtnOK.Location = new System.Drawing.Point(77, 97);
			this.myCtlBtnOK.Name = "myCtlBtnOK";
			this.myCtlBtnOK.TabIndex = 3;
			this.myCtlBtnOK.Text = "OK";
			this.myCtlBtnOK.Click += new System.EventHandler(this.Handler_OK);
			// 
			// myCtlBtnCancel
			// 
			this.myCtlBtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.myCtlBtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.myCtlBtnCancel.Location = new System.Drawing.Point(164, 97);
			this.myCtlBtnCancel.Name = "myCtlBtnCancel";
			this.myCtlBtnCancel.TabIndex = 4;
			this.myCtlBtnCancel.Text = "Cancel";
			// 
			// frmSwitchPages
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(251, 132);
			this.Controls.Add(this.myCtlLblSwitchPage);
			this.Controls.Add(this.myCtlCmbItems);
			this.Controls.Add(this.myCtlChkSetSelectedPage);
			this.Controls.Add(this.myCtlBtnCancel);
			this.Controls.Add(this.myCtlBtnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSwitchPages";
			this.ShowInTaskbar = false;
			this.Text = "Switch Pages";
			this.ResumeLayout(false);

		}

		#endregion
	}
}