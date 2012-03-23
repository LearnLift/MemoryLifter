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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using MLifter.DAL;
using System.ComponentModel;
using System.Drawing;
using System.Net;

namespace MLifter.Components
{
    /// <summary>
    /// This is the Textbox to use for entering the answer including the OnTheFly-Correction.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-09-19</remarks>
    public class MLifterTextBox : RichTextBox
    {
        private int selectionStart = 0;
        private int selectionLength = 0;
        private bool lastCharIsWrong = false;
        private Regex validChar;
        private List<int> wrongChars = new List<int>();
        private string oldText = string.Empty;
        private List<char> allowedControlChars = new List<char>();
        private bool selectionEventEnabled = true;
        private bool showTipp = false;

        public int Errors = 0;
        public int CorrectSynonyms;
        public bool CorrectFirstSynonym;

        #region properties and events
        private List<string> stripChars = new List<string>();
        [Browsable(false), ReadOnly(true)]
        public List<string> StripChars
        {
            get { return stripChars; }
            set { stripChars = value; }
        }

        private string ignoreChars;
        [Browsable(false), ReadOnly(true)]
        public string IgnoreChars
        {
            get { return ignoreChars; }
            set
            {
                //[ML-558] Adding a '-' to the ingnore chars throws an unhandled exception - Escape each character
                ignoreChars = Regex.Escape(value).Replace("-", "\\-");
                validChar = new Regex("[" + ignoreChars + "\\b" + "]");
            }
        }

        private List<char> newLineChars = new List<char>();
        [Browsable(false), ReadOnly(true)]
        public List<char> NewLineChars
        {
            get { return newLineChars; }
            set { newLineChars = value; }
        }

        private bool caseSensitive;
        [Browsable(false), ReadOnly(true)]
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }

        private bool correctOnTheFly;
        [Browsable(false), ReadOnly(true)]
        public bool CorrectOnTheFly
        {
            get { return correctOnTheFly; }
            set { correctOnTheFly = value; }
        }

        private Color tippForeColor = Color.Empty, orgForeColor = Color.Empty;
        public Color TippForeColor
        {
            get { return tippForeColor; }
            set { tippForeColor = value; orgForeColor = ForeColor; }
        }

        private Color tippBackColor = Color.Empty, orgBackColor = Color.Empty;
        public Color TippBackColor
        {
            get { return tippBackColor; }
            set { tippBackColor = value; orgBackColor = BackColor; }
        }

        private List<string> synonyms = new List<string>();
        [Browsable(false), ReadOnly(true)]
        public IList<string> Synonyms
        {
            get
            {
                return synonyms;
            }
            set
            {
                Text = String.Empty;
                Errors = 0;
                wrongChars.Clear();
                synonyms.Clear();
                foreach (string word in value)
                    synonyms.Add(WebUtility.HtmlDecode(word));     //[ML-1474] Convert Html entities to their character representations for the answer input window 
                CalulateSize();
                ShowTipp();
            }
        }

        private string welcomeTipp = String.Empty;
        [Browsable(false), ReadOnly(true)]
        public string WelcomeTipp
        {
            get { return welcomeTipp; }
            set 
            { 
                welcomeTipp = value;
                if (showTipp)
                    ShowTipp(); //refresh tip [ML-1955]
            }
        }

        [Browsable(false), DefaultValue(true)]
        public bool AllowAnswerSubmit { get; set; }

        public event EventHandler Correct;
        protected void OnCorrect(EventArgs e)
        {
            if (!AllowAnswerSubmit)
                return;

            if (Correct != null)
                Correct(this, e);
        }

        public event EventHandler Wrong;
        protected void OnWrong(EventArgs e)
        {
            if (!AllowAnswerSubmit)
                return;

            if (Wrong != null)
                Wrong(this, e);
        }
        # endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MLifterTextBox"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-19</remarks>
        public MLifterTextBox()
        {
            AllowAnswerSubmit = true;
            BorderStyle = BorderStyle.None;
            Font = new System.Drawing.Font("MS Sans Serif", 14, System.Drawing.FontStyle.Bold);
            SelectionAlignment = HorizontalAlignment.Center;

            //ScrollBars = RichTextBoxScrollBars.ForcedVertical;

            AllowDrop = true;
            DragEnter += new DragEventHandler(MLifterTextBox_DragEnter);
            DragDrop += new DragEventHandler(MLifterTextBox_DragDrop);
            MouseDown += new MouseEventHandler(MLifterTextBox_MouseDown);
            Leave += new EventHandler(MLifterTextBox_Leave);
            ContentsResized += new ContentsResizedEventHandler(MLifterTextBox_ContentsResized);

            caseSensitive = false;
            correctOnTheFly = false;

            StripChars.Add("\n\r");
            StripChars.Add("\r\n");
            StripChars.Add("\n");
            //StripChars.Add(",");

            allowedControlChars.Clear();
            allowedControlChars.Add((char)Keys.Enter);
            allowedControlChars.Add((char)Keys.Back);
            allowedControlChars.Add((char)Keys.Space);
            allowedControlChars.Add((char)Keys.LineFeed);

            IgnoreChars = ".!?;,";
        }

        /// <summary>
        /// Handles the Leave event of the MLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        void MLifterTextBox_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.Text))
                ShowTipp();
        }

        /// <summary>
        /// Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Windows.Forms.RightToLeft"/> values. The default is <see cref="F:System.Windows.Forms.RightToLeft.Inherit"/>.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the <see cref="T:System.Windows.Forms.RightToLeft"/> values. </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        /// <remarks>Documented by Dev02, 2008-07-16</remarks>
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;

                //restore the horizontal text alignment (gets destroyed through RightToLeft)
                this.SelectAll();
                this.SelectionAlignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the MLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-02</remarks>
        void MLifterTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            ClearTipp();

            if (SelectionLength > 0 && !CorrectOnTheFly)
            {
                selectionEventEnabled = false;
                selectionStart = SelectionStart;
                selectionLength = SelectionLength;
                selectionEventEnabled = true;
                DoDragDrop(SelectedText, DragDropEffects.Move);

                if (selectionStart <= Text.Length)
                    SelectionStart = selectionStart; //[ML-877] Restore selection as it was before drag and drop operation
                if (SelectionStart + selectionLength <= Text.Length)
                    SelectionLength = selectionLength;
            }
        }

        /// <summary>
        /// Handles the DragEnter event of the MLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-02</remarks>
        void MLifterTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) && !CorrectOnTheFly)
                e.Effect = DragDropEffects.Move;
            else if (e.Data.GetDataPresent(DataFormats.FileDrop) && FileDropped != null)
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Handles the DragDrop event of the MLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-02</remarks>
        void MLifterTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) &&
                (SelectionStart <= selectionStart || SelectionStart >= selectionStart + selectionLength)) //[ML-877] Do not allow text to be inserted inside itself
            {
                ClearTipp();

                selectionEventEnabled = false;
                int selStart = SelectionStart;
                string text = e.Data.GetData(DataFormats.Text) as string;
                Text = Text.Remove(selectionStart, selectionLength).Insert(SelectionStart - (SelectionStart > selectionStart ? selectionLength : 0), text);
                SelectionStart = selStart + text.Length;
                selectionStart = 0;
                selectionLength = 0;
                selectionEventEnabled = true;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (FileDropped != null)
                    FileDropped(this, e);
            }
        }

        public delegate void FileDroppedEventHandler(object sender, DragEventArgs e);
        public event FileDroppedEventHandler FileDropped;

        /// <summary>
        /// Sends the char.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <remarks>Documented by Dev05, 2007-09-19</remarks>
        public void SendChar(char character)
        {
            OnKeyPress(new KeyPressEventArgs(character));
        }

        public void ManualOnKeyPress(KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyPress"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyPressEventArgs"></see> that contains the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-19</remarks>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            ClearTipp();

            if (!allowedControlChars.Contains(e.KeyChar) && char.IsControl(e.KeyChar))
                return;

            if (e.KeyChar == (char)Keys.Back && CorrectOnTheFly)
            {
                for (int i = 0; i < wrongChars.Count; i++)
                    if (wrongChars[i] > Text.Length - 1)
                    {
                        wrongChars.Remove(wrongChars[i]);
                        i++;
                    }
                lastCharIsWrong = false;
                OnSelectionChanged(new EventArgs());
                return;
            }

            //fix for [ML-799] MLifterTextbox: Cannot go back and insert additional characters - delete the newline at the current cursor position
            if (!CorrectOnTheFly && e.KeyChar == (char)Keys.Enter && SelectionLength == 0)
            {
                if (SelectionStart > 0 && Text[SelectionStart - 1] == Convert.ToChar("\n"))
                {
                    int cursorposition = SelectionStart; //preserve the cursor position
                    Text = Text.Remove(cursorposition - 1, 1);
                    this.SelectionStart = cursorposition - 1;
                }
            }

            # region generate lists with all and entered synonyms
            List<string> openSynonyms = new List<string>();
            List<string> gottenSynonyms = new List<string>();
            foreach (string str in synonyms)
            {
                string strg;
                if (!CaseSensitive)
                    strg = str.ToLower();
                else
                    strg = str;
                strg = validChar.Replace(strg, string.Empty);
                strg = RemoveUnicodeFormatChars(strg); //fix for [ML-588]  MLifterTextbox: Unicode control characters in answer cause comparison to fail

                openSynonyms.Add(RemoveIgnoreChars(strg));
            }

            if (CorrectOnTheFly && SelectionLength > 0)
                Text = Text.Substring(0, Text.Length - 1);

            if (CorrectOnTheFly)
                Text += e.KeyChar.ToString();

            //corrected because of [ML-486] - deletion with backspace still causes the answer to be true
            //if (e.KeyChar == (char)Keys.Enter)
            //    Undo();

            List<string> currentSynonyms = new List<string>();
            currentSynonyms.AddRange(Text.Split(StripChars.ToArray(), StringSplitOptions.RemoveEmptyEntries));
            # endregion
            # region remove entered synonyms from open-list
            CorrectFirstSynonym = false;
            CorrectSynonyms = 0;
            foreach (string str in currentSynonyms.ToArray())
            {
                string strg = CaseSensitive ? str.Trim() : str.ToLower().Trim();
                strg = RemoveIgnoreChars(strg);

                if (openSynonyms.Contains(strg))
                {
                    CorrectSynonyms++;
                    if (RemoveIgnoreChars(synonyms[0].ToLower()) == strg.ToLower())
                        CorrectFirstSynonym = true;

                    openSynonyms.Remove(strg);
                    //currentSynonyms.Remove(strg); //[ML-522] currentsynonyms contains the not-lowered version (str) instead of the lowered version (strg)
                    currentSynonyms.Remove(str);
                    gottenSynonyms.Add(str);
                }
                else
                    continue;
            }
            # endregion
            # region check input
            if (e.KeyChar == (char)Keys.Back)
                return;
            else if (CorrectOnTheFly)
            {
                Text = string.Join("\n", gottenSynonyms.ToArray()) + (CorrectSynonyms > 0 ? "\n" : "");
                e.Handled = true;

                string currentSynonym = (currentSynonyms.Count > 0 ? currentSynonyms[0] : "");
                Text += currentSynonym;

                selectionEventEnabled = false;
                SelectionLength = 0;
                SelectionStart = Text.Length;
                selectionEventEnabled = true;

                MarkWrongChars();

                if (e.KeyChar == (int)Keys.Enter || openSynonyms.Count == 0)
                {
                    if (e.KeyChar == (int)Keys.Enter && lastCharIsWrong) //[ML-525] Correct-on-the-fly does not show last (wrong) char
                    {
                        Text = oldText;
                        oldText = string.Empty; //[ML-540] when only pressing enter, the last entered text gets shown
                    }

                    if (openSynonyms.Count == 0)
                        OnCorrect(EventArgs.Empty);
                    else
                        OnWrong(EventArgs.Empty);
                }
                else if (currentSynonyms.Count != 0)
                {
                    foreach (string str in openSynonyms.ToArray())
                        if (!str.StartsWith(CaseSensitive ? currentSynonym : currentSynonym.ToLower())) //[ML-522] currentsynonym must be converted to lower for the comparison to work
                            openSynonyms.Remove(str);

                    if (openSynonyms.Count == 0)
                    {
                        if (!lastCharIsWrong)
                            Errors++;
                        wrongChars.Add(Text.Length - 1);
                        lastCharIsWrong = true;

                        SelectionStart = Text.Length - 1;
                        SelectionLength = 1;
                    }
                    else
                        lastCharIsWrong = false;
                    oldText = Text;
                }
            }
            else
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    if (Lines.Length < synonyms.Count && (Lines.Length > 0 && Lines[Lines.Length - 1].Length > 0))
                    {
                        int selectionStart = SelectionStart;
                        Text = Text.Insert(SelectionStart, e.KeyChar.ToString());
                        SelectionStart = selectionStart + 1;
                        return;
                    }

                    //corrected because of [ML-530] - MLifterTextbox: Chinese Input (IME) does not appear correctly
                    //Undo();

                    if (openSynonyms.Count == 0)
                        OnCorrect(EventArgs.Empty);
                    else
                        OnWrong(EventArgs.Empty);

                    return;
                }
                else if (StripChars.Contains(e.KeyChar.ToString()))
                {
                    int selectionStart = SelectionStart;
                    Text = Text.Insert(SelectionStart, "\n");
                    SelectionStart = selectionStart + 1;
                    e.Handled = true;
                }
                else
                {
                    int selectionStart = SelectionStart;
                    if (SelectionLength > 0)
                        Text = Text.Substring(0, selectionStart) + e.KeyChar.ToString() + Text.Substring(selectionStart + SelectionLength, Text.Length - selectionStart - SelectionLength);
                    else
                        Text = Text.Insert(selectionStart, e.KeyChar.ToString());
                    SelectionStart = selectionStart + 1;
                    e.Handled = true;
                }
            }
            # endregion
        }

        /// <summary>
        /// Removes the ignor chars.
        /// </summary>
        /// <param name="strg">The STRG.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-11-19</remarks>
        private string RemoveIgnoreChars(string strg)
        {
            return validChar.Replace(strg, "");
        }

        /// <summary>
        /// Marks the wrong chars.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-19</remarks>
        private void MarkWrongChars()
        {
            selectionEventEnabled = false;

            int oldLength = SelectionLength;
            int oldPos = SelectionStart;

            //Bugfix for [ML-533] MLifterTextbox: Whole word gets marked as red in correct-on-the-fly
            SelectionStart = 0;
            SelectionLength = Text.Length;
            SelectionColor = System.Drawing.Color.Black;

            foreach (int index in wrongChars)
            {
                SelectionStart = index;
                SelectionLength = 1;
                SelectionColor = System.Drawing.Color.Red;
            }
            SelectionStart = oldPos;
            SelectionLength = oldLength;

            selectionEventEnabled = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"></see> that contains the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-19</remarks>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
                e.Handled = true;
            else if (e.Control && (e.KeyCode == Keys.V || e.KeyCode == Keys.X) && CorrectOnTheFly) //prevent paste and cut (from/to clipboard)
                e.Handled = true;
            else if (e.KeyCode == Keys.Delete && CorrectOnTheFly) //prevent delete key
                e.Handled = true;
            else
                base.OnKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="mevent">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-19</remarks>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (CorrectOnTheFly && Text.Length > 0 && selectionEventEnabled)
            {
                selectionEventEnabled = false;
                if (lastCharIsWrong)
                {
                    SelectionStart = Text.Length - 1;
                    SelectionLength = 1;
                }
                else
                {
                    SelectionLength = 0;
                    SelectionStart = Text.Length;
                }
                selectionEventEnabled = true;
            }

            base.OnMouseUp(mevent);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.RichTextBox.SelectionChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-25</remarks>
        protected override void OnSelectionChanged(EventArgs e)
        {
            if (selectionEventEnabled)
            {
                selectionEventEnabled = false;
                if (CorrectOnTheFly && Text.Length > 0 &&
                    (lastCharIsWrong && (SelectionStart != Text.Length - 1 || SelectionLength != 1) || !lastCharIsWrong && SelectionStart != Text.Length))
                {
                    if (lastCharIsWrong)
                    {
                        SelectionStart = Text.Length - 1;
                        SelectionLength = 1; // [ML-538]  MLifterTextbox: Right arrow confuses the 'On-The-Fly' Mode - the selectionlength must be set after the selectionstart
                    }
                    else
                    {
                        SelectionLength = 0;
                        SelectionStart = Text.Length;
                    }
                }
                selectionEventEnabled = true;
            }

            base.OnSelectionChanged(e);
        }

        /// <summary>
        /// Replaces the control chars.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-02-20</remarks>
        private string RemoveUnicodeFormatChars(string text)
        {
            List<char> formatChars = new List<char>();
            foreach (char character in text.ToCharArray())
            {
                if (Char.IsControl(character) || Char.GetUnicodeCategory(character) == System.Globalization.UnicodeCategory.Format)
                    formatChars.Add(character);
            }

            foreach (char formatChar in formatChars)
            {
                text = text.Replace(formatChar.ToString(), string.Empty);
            }

            return text;
        }

        /// <summary>
        /// Handles the ContentsResized event of the MLifterTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ContentsResizedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        void MLifterTextBox_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            if (!Visible) return;

            if (Height < e.NewRectangle.Height)
            {
                Height = e.NewRectangle.Height + 10;
            }
            if (Height > minHeigth)
            {
                Height = e.NewRectangle.Height + 10;
            }
        }

        /// <summary>
        /// Gets or sets the size that is the lower limit that <see cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)"/> can specify.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        public override Size MinimumSize
        {
            get
            {
                if (this.DesignMode)
                    return base.MinimumSize;
                int width = Parent.Width - 10;
                return new Size(width, minHeigth);
            }
            set
            {
                base.MinimumSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size that is the upper limit that <see cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)"/> can specify.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        public override Size MaximumSize
        {
            get
            {
                if (this.DesignMode)
                    return base.MaximumSize;
                int width = Parent.Width - 10;
                int heigth = Parent.Height - 10;
                foreach (Control c in Parent.Controls)
                {
                    if (c.Visible && !(c is MLifterTextBox) && !(c is Panel))
                        heigth -= c.Height;
                }

                return new Size(width, heigth);
            }
            set
            {
                base.MaximumSize = value;
            }
        }

        private int linecount = 0;
        private int minHeigth = 0;
        /// <summary>
        /// Calulates the size.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        private void CalulateSize()
        {
            using (Graphics g = this.CreateGraphics())
            {
                string lines = String.Empty;
                linecount = (((synonyms.Count >= 3) ? synonyms.Count : 3) > 5) ? 5 : (synonyms.Count >= 3) ? synonyms.Count : 3;
                for (int i = 0; i < linecount; i++)
                    lines += "line" + Environment.NewLine;
                this.Height = minHeigth = Convert.ToInt32(g.MeasureString(lines.Trim(), Font).Height);
            }
        }

        /// <summary>
        /// Shows the tipp.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        private void ShowTipp()
        {
            showTipp = true;
            if (tippBackColor != Color.Empty)
                BackColor = tippBackColor;
            if (tippForeColor != Color.Empty)
                ForeColor = tippForeColor;
            Text = String.Empty;
            for (int i = 0; i < Convert.ToInt32(Math.Ceiling(linecount / 2.0)) - 1; i++)
                Text += Environment.NewLine;
            Text += welcomeTipp;
            SelectionStart = Text.Length;
        }

        /// <summary>
        /// Clears the tipp.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-04-14</remarks>
        private void ClearTipp()
        {
            if (showTipp)
            {
                showTipp = false;
                Text = String.Empty;
                BackColor = orgBackColor;
                ForeColor = orgForeColor;
            }
        }

    }
}
