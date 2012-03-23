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
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MLifter.Controls.Properties;
using System.Diagnostics;

namespace MLifter.Controls
{
    /// <summary>
    /// Creates arrayList character table to select special characters e.g. umlauts for user with english keyboard layout.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-19</remarks>
    public class CharacterForm : System.Windows.Forms.Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private System.Windows.Forms.GroupBox GBSymbols;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// The control to receive the user input.
        /// </summary>
        private Control currentControl = null;

        /// <summary>
        /// The last position of the character map form.
        /// </summary>
        private static Point formPosition = Point.Empty;

        /// <summary>
        /// Gets or sets the current control.
        /// </summary>
        /// <value>The current control.</value>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public Control CurrentControl
        {
            get
            {
                return currentControl;
            }
            set
            {
                if (currentControl != value)
                {
                    currentControl = value;
                }
            }
        }


        private HelpProvider MainHelp;
        private System.Windows.Forms.Button[] SomeButtons;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterForm"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public CharacterForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            SomeButtons = new Button[200];

            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    //byte[] bytes2 = new byte[] { (byte)(count + 128), 0, 0, 0 };
                    //string symbol = System.Text.Encoding.UTF32.GetString(bytes2);
                    // Dirty�
                    string symbol = Resources.CHARACTERMAP_CHARS;
                    SomeButtons[count] = new Button();
                    GBSymbols.Controls.Add(SomeButtons[count]);
                    SomeButtons[count].MouseUp += new MouseEventHandler(SomeButtons_MouseUp);
                    SomeButtons[count].Size = new System.Drawing.Size(20, 23);
                    SomeButtons[count].Tag = symbol[count].ToString();
                    SomeButtons[count].Font = new Font(FontFamily.GenericSansSerif, 8);
                    SomeButtons[count].Text = symbol[count].ToString();
                    SomeButtons[count].BackColor = Color.White;
                    SomeButtons[count].Location = new Point(SomeButtons[count].Width * j + 5, SomeButtons[count].Height * i + 16);
                    SomeButtons[count].FlatStyle = FlatStyle.Flat;
                    SomeButtons[count].FlatAppearance.BorderSize = 0;
                    count++;
                }
            }
        }

        /// <summary>
        /// Gets or sets the help namespace.
        /// </summary>
        /// <value>The help namespace.</value>
        /// <remarks>Documented by Dev02, 2008-03-07</remarks>
        public string HelpNamespace
        {
            get { return MainHelp.HelpNamespace; }
            set { MainHelp.HelpNamespace = value; }
        }

        /// <summary>
        /// Handles the MouseUp event of the SomeButtons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-29</remarks>
        void SomeButtons_MouseUp(object sender, MouseEventArgs e)
        {
            this.SomeButtons_Click(sender, new EventArgs());
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterForm));
            this.GBSymbols = new System.Windows.Forms.GroupBox();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // GBSymbols
            // 
            resources.ApplyResources(this.GBSymbols, "GBSymbols");
            this.GBSymbols.Name = "GBSymbols";
            this.GBSymbols.TabStop = false;
            // 
            // CharacterForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.GBSymbols);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.Name = "CharacterForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.CharacterForm_Shown);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Sends the selected character to the answer control (PEanswer) and sets the focus back on CharacterForm.
        /// </summary>
        /// <param name="sender">Button object that raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        private void SomeButtons_Click(object sender, System.EventArgs e)
        {
            // Add symbol to currently focused control 
            string Symbol = (string)(sender as Button).Tag;

            if (CurrentControl != null)
            {
                if (CurrentControl is MLifter.Components.MLifterTextBox)
                    (CurrentControl as MLifter.Components.MLifterTextBox).SendChar(Symbol[0]);
                else if (CurrentControl is TextBox)
                {
                    TextBox textbox = CurrentControl as TextBox;
                    int selectionstart = textbox.SelectionStart;
                    string newtext = string.Empty;
                    newtext += textbox.Text.Substring(0, textbox.SelectionStart);
                    newtext += Symbol;
                    newtext += textbox.Text.Substring(textbox.SelectionStart + textbox.SelectionLength, textbox.Text.Length - textbox.SelectionStart - textbox.SelectionLength);
                    textbox.Text = newtext;
                    textbox.SelectionStart = selectionstart + Symbol.Length;
                }
                else if (CurrentControl is ListView && CurrentControl.Parent.Parent is CardEdit)
                {
                    (CurrentControl.Parent.Parent as CardEdit).SendListViewChar(Symbol, (ListView)CurrentControl);
                }

                this.TopMost = true;

                // Set focus back
                if (CurrentControl.Visible)
                    CurrentControl.Focus();
            }
        }

        /// <summary>
        /// Determines whether this instance can control the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can control the specified control; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public static bool CanControl(Control control)
        {
            return (control is MLifter.Components.MLifterTextBox
                    || control is TextBox
                    || (control is ListView && ((Control)control).Parent != null && ((Control)control).Parent.Parent != null && ((Control)control).Parent.Parent is CardEdit));
        }

        /// <summary>
        /// Handles the LocationChanged event of the CharacterForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void CharacterForm_LocationChanged(object sender, EventArgs e)
         {
            formPosition = this.Location;
        }

        /// <summary>
        /// Handles the Shown event of the CharacterForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        private void CharacterForm_Shown(object sender, EventArgs e)
        {
            if (!formPosition.IsEmpty)
                this.Location = formPosition;

            //register eventhandler to save location
            this.LocationChanged+=new EventHandler(CharacterForm_LocationChanged);
        }
    }
}
