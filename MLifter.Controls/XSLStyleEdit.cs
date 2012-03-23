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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

using XPTable;
using XPTable.Models;
using XPTable.Editors;

using MLifter.BusinessLayer;
using MLifter.Controls.SyntaxHighlightingTextBox;
using MLifter.DAL.Interfaces;
using System.Diagnostics;

namespace MLifter.Controls
{
    public enum DisplayTyp
    {
        Question,
        AnswerCorrect,
        AnswerWrong
    }

    public partial class XSLStyleEdit : UserControl
    {
        private readonly List<EnumLocalizer> horizontalElements = new List<EnumLocalizer>();
        private readonly List<EnumLocalizer> verticalElements = new List<EnumLocalizer>();

        private bool loading = false;
        private bool textchanged = false;

        private int cardID;
        private Dictionary dictionary;
        private string filename;

        private string header;
        private string footer;
        private List<XSLStyleElement> xslElements;

        private DisplayTyp display = DisplayTyp.Question;
        [Browsable(false), ReadOnly(true)]
        public DisplayTyp Display
        {
            get { return display; }
            set { display = value; ShowPreview(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether table view or text view.
        /// </summary>
        /// <value><c>true</c> if table view; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-05-12</remarks>
        public bool TableView { get { return tabControlMode.SelectedIndex == 0; } set { tabControlMode.SelectedIndex = value ? 0 : 1; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="XSLStyleEdit"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        public XSLStyleEdit()
        {
            InitializeComponent();

            tableXSL.HeaderRenderer = new XPTable.Renderers.GradientHeaderRenderer();

            //manually localized resource strings
            tableXSL.NoItemsText = Properties.Resources.STYLEEDIT_NOITEMSTEXT;
            textColumnElementName.Text = Properties.Resources.STYLEEDIT_COLUMN_ELEMENT;
            colorColumnColor.Text = Properties.Resources.STYLEEDIT_COLUMN_COLOR;
            colorColumnBackColor.Text = Properties.Resources.STYLEEDIT_COLUMN_BACKCOLOR;
            buttonColumnFont.Text = Properties.Resources.STYLEEDIT_COLUMN_FONT;
            comboBoxColumnHAlign.Text = Properties.Resources.STYLEEDIT_COLUMN_HALIGN;
            comboBoxColumnVAlign.Text = Properties.Resources.STYLEEDIT_COLUMN_VALIGN;

            horizontalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.None));
            horizontalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.Left));
            horizontalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.Center));
            horizontalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.Right));
            ComboBoxCellEditor horizontalEditor = new ComboBoxCellEditor();
            horizontalEditor.DropDownStyle = DropDownStyle.DropDownList;
            horizontalEditor.Items.AddRange(horizontalElements.ToArray());
            comboBoxColumnHAlign.Editor = horizontalEditor;

            verticalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.VerticalAlignment.None));
            verticalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.VerticalAlignment.Top));
            verticalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.VerticalAlignment.Middle));
            verticalElements.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.VerticalAlignment.Bottom));
            ComboBoxCellEditor verticalEditor = new ComboBoxCellEditor();
            verticalEditor.DropDownStyle = DropDownStyle.DropDownList;
            verticalEditor.Items.AddRange(verticalElements.ToArray());
            comboBoxColumnVAlign.Editor = verticalEditor;

            textBoxFile.ScrollBars = RichTextBoxScrollBars.Both;
            textBoxFile.WordWrap = false;

            textBoxFile.Seperators.Add(' ');
            textBoxFile.Seperators.Add('\t');
            textBoxFile.Seperators.Add('\r');
            textBoxFile.Seperators.Add('\n');
            textBoxFile.Seperators.Add(';');
            textBoxFile.Seperators.Add('=');

            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor("body", Color.Red, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor("button", Color.Red, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor("a", Color.Red, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor("a:", Color.Red, null, DescriptorType.Word, DescriptorRecognition.StartsWith, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor("div.", Color.Red, null, DescriptorType.Word, DescriptorRecognition.StartsWith, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor(".", Color.Red, null, DescriptorType.Word, DescriptorRecognition.StartsWith, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor("#", Color.Cyan, null, DescriptorType.Word, DescriptorRecognition.StartsWith, false));
            textBoxFile.HighlightDescriptors.Add(new HighlightDescriptor(":", Color.DarkBlue, null, DescriptorType.Word, DescriptorRecognition.Contains, false));
        }

        /// <summary>
        /// Handles the Resize event of the XSLStyleEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void XSLStyleEdit_Resize(object sender, EventArgs e)
        {
            stylePreview.Width = Width / 3;
            tabControlMode.Width = Width * 2 / 3;
            tabControlMode.ItemSize = new Size((tabControlMode.Width - 3) / 2, tabControlMode.ItemSize.Height);
            textBoxFile.Height = tabPageText.Height - 40;

            foreach (XPTable.Models.Column column in tableXSL.ColumnModel.Columns)
                column.Width = (int)((tableXSL.Width - 24) / tableXSL.ColumnModel.Columns.Count);
        }

        /// <summary>
        /// Initializes the file.
        /// </summary>
        /// <param name="xslFilePath">The XSL file path.</param>
        /// <param name="actualCardID">The actual card ID.</param>
        /// <param name="actualDictinary">The actual dictinary.</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        public void InitializeFile(string xslFilePath, int actualCardID, Dictionary actualDictinary)
        {
            cardID = actualCardID;
            dictionary = actualDictinary;
            filename = xslFilePath;

            ShowPreview();
            tabControlMode_SelectedIndexChanged(this, null);
        }

        /// <summary>
        /// Shows the preview.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void ShowPreview()
        {
            dictionary.CurrentlyLoadedStyleSheet = new Dictionary<MLifter.DAL.Interfaces.Side, string>();

            if (dictionary != null)
            {
                Card card = null;
                try
                {
                    if (cardID >= 0)
                        card = dictionary.Cards.GetCardByID(cardID);
                }
                catch (Exception exp) { Trace.WriteLine(exp.ToString()); }

                if (card == null)
                    DisplayPreviewCard();
                else
                {
                    try { DisplayPreview(card); }
                    catch (Exception exp) { Trace.WriteLine(exp.ToString()); DisplayPreviewCard(); }
                }
            }
        }
        /// <summary>
        /// Diplays the preview card.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-10-13</remarks>
        private void DisplayPreviewCard()
        {
            DisplayPreview(GeneratePreviewCard());
        }
        /// <summary>
        /// Generates the preview card.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-10-13</remarks>
        private Card GeneratePreviewCard()
        {
            MLifter.DAL.Preview.PreviewCard pcard = new MLifter.DAL.Preview.PreviewCard(null);
            pcard.Answer.AddWord(pcard.Answer.CreateWord(Properties.Resources.EXAMPLE_CARD_ANSWER, WordType.Word, true));
            pcard.AnswerExample.AddWord(pcard.AnswerExample.CreateWord(Properties.Resources.EXAMPLE_CARD_ANSWER_EXAMPLE, WordType.Sentence, false));
            pcard.Question.AddWord(pcard.Question.CreateWord(Properties.Resources.EXAMPLE_CARD_QUESTION, WordType.Word, true));
            pcard.QuestionExample.AddWord(pcard.QuestionExample.CreateWord(Properties.Resources.EXAMPLE_CARD_QUESTION_EXAMPLE, WordType.Sentence, false));
            return new Card(pcard, dictionary);
        }
        /// <summary>
        /// Displays the priview.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <remarks>Documented by Dev05, 2009-10-13</remarks>
        private void DisplayPreview(Card card)
        {
            switch (Display)
            {
                case DisplayTyp.Question:
                    stylePreview.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareQuestion(dictionary.DictionaryDAL.Parent, card.BaseCard.Id,
                        dictionary.GenerateCard(card, MLifter.DAL.Interfaces.Side.Question, String.Empty, false));
                    break;
                case DisplayTyp.AnswerCorrect:
                    stylePreview.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareAnswer(dictionary.DictionaryDAL.Parent, card.BaseCard.Id,
                        dictionary.GenerateCard(card, MLifter.DAL.Interfaces.Side.Answer, Properties.Resources.XSLEDIT_USERANSWER, true));
                    break;
                case DisplayTyp.AnswerWrong:
                    stylePreview.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareAnswer(dictionary.DictionaryDAL.Parent, card.BaseCard.Id,
                        dictionary.GenerateCard(card, MLifter.DAL.Interfaces.Side.Answer, Properties.Resources.XSLEDIT_USERANSWER, false));
                    break;
            }
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="parse">if set to <c>true</c> [parse].</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void LoadFile(bool parse)
        {
            loading = true;
            try
            {
                string[] content = File.ReadAllText(filename).Split(new string[] { "<![CDATA[", "]]>" }, StringSplitOptions.None);
                header = content[0] + "<![CDATA[\n";
                footer = "]]>" + content[2];

                if (parse) ParseCSS(content[1]);
            }
            catch (Exception exp)
            {
                tabControlMode.SelectedIndex = 1;
                TaskDialog.MessageBox(Properties.Resources.XSLEDIT_FILERROR_CAPTION, Properties.Resources.XSLEDIT_FILEERROR_TEXT, string.Empty, exp.ToString(), string.Empty, string.Empty,
                     TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
            }
            loading = false;
        }

        /// <summary>
        /// Parses the CSS.
        /// </summary>
        /// <param name="css">The CSS.</param>
        /// <remarks>Documented by Dev03, 2008-03-05</remarks>
        private void ParseCSS(string css)
        {
            // ignore comments when parsing css
            Regex stripComments = new Regex(@"/\*(.|\n)*?\*/", RegexOptions.Multiline);
            css = stripComments.Replace(css, String.Empty);

            string[] elements = css.Split(new char[] { '{', '}' });
            xslElements = new List<XSLStyleElement>();
            for (int i = 0; i < elements.Length - 1; i++)
            {
                XSLStyleElement element = new XSLStyleElement();
                element.Name = elements[i].Trim();
                i++;

                string subElements = elements[i].Trim();

                Regex whitespace = new Regex("[\n\r\t]");
                subElements = whitespace.Replace(subElements, String.Empty);

                string[] pairs = subElements.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in pairs)
                {
                    string[] parts = pair.Split(new char[] { ':' }, 2);

                    element.Items[parts[0].ToLower().Trim()] = parts[1].Trim();
                }
                // end parsing

                xslElements.Add(element);
            }

            tableXSL.TableModel.Rows.Clear();
            foreach (XSLStyleElement element in xslElements)
            {
                tableXSL.TableModel.Rows.Add(new XPTable.Models.Row(new string[] { element.Name.StartsWith(".") ? element.Name.Substring(1) : element.Name }));

                Cell colorCell;
                if (element.Items.ContainsKey("color"))
                {
                    int colorValue;
                    if (Int32.TryParse(element.Items["color"].Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier, null, out colorValue))
                    {
                        colorCell = new XPTable.Models.Cell(Color.FromArgb(255, Color.FromArgb(colorValue)));
                    }
                    else
                    {
                        colorCell = new XPTable.Models.Cell(Color.Empty);
                    }
                }
                else
                {
                    colorCell = new XPTable.Models.Cell(Color.Empty);
                }
                colorCell.PropertyChanged += new XPTable.Events.CellEventHandler(XSLStyleEdit_PropertyChanged);
                colorCell.Tag = element;
                colorCell.ToolTipText = "color";
                tableXSL.TableModel.Rows[tableXSL.TableModel.Rows.Count - 1].Cells.Add(colorCell);

                Cell backgroundColorCell;
                if (element.Items.ContainsKey("background-color"))
                {
                    int colorValue;
                    if (Int32.TryParse(element.Items["background-color"].Substring(1, 6), System.Globalization.NumberStyles.AllowHexSpecifier, null, out colorValue))
                    {
                        backgroundColorCell = new XPTable.Models.Cell(Color.FromArgb(255, Color.FromArgb(colorValue)));
                    }
                    else
                    {
                        backgroundColorCell = new XPTable.Models.Cell(Color.Empty);
                    }
                }
                else
                {
                    backgroundColorCell = new XPTable.Models.Cell(Color.Empty);
                }
                backgroundColorCell.PropertyChanged += new XPTable.Events.CellEventHandler(XSLStyleEdit_PropertyChanged);
                backgroundColorCell.Tag = element;
                backgroundColorCell.ToolTipText = "background-color";
                tableXSL.TableModel.Rows[tableXSL.TableModel.Rows.Count - 1].Cells.Add(backgroundColorCell);

                Cell cellToAdd = new XPTable.Models.Cell(Properties.Resources.STYLEEDIT_SELECTFONTBUTTON);
                cellToAdd.PropertyChanged += new XPTable.Events.CellEventHandler(XSLStyleEdit_PropertyChanged);
                cellToAdd.Tag = element;
                cellToAdd.ToolTipText = "font";
                tableXSL.TableModel.Rows[tableXSL.TableModel.Rows.Count - 1].Cells.Add(cellToAdd);

                cellToAdd = new XPTable.Models.Cell();
                cellToAdd.PropertyChanged += new XPTable.Events.CellEventHandler(XSLStyleEdit_PropertyChanged);
                cellToAdd.Tag = element;
                cellToAdd.ToolTipText = "HAlign";
                tableXSL.TableModel.Rows[tableXSL.TableModel.Rows.Count - 1].Cells.Add(cellToAdd);

                EnumLocalizer hNone = new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.None);
                if (element.Items.ContainsKey("text-align"))
                {
                    EnumLocalizer textAlign = horizontalElements.Find(delegate(EnumLocalizer loc)
                    {
                        return loc.value.ToString().Equals(element.Items["text-align"], StringComparison.InvariantCultureIgnoreCase);
                    });
                    cellToAdd.Text = (textAlign != null) ? textAlign.ToString() : hNone.ToString();
                }
                else if (element.Items.ContainsKey("float"))
                {
                    cellToAdd.Text = element.Items["float"];
                    cellToAdd.ToolTipText = "float";
                }
                else
                    cellToAdd.Text = hNone.ToString();

                cellToAdd = new XPTable.Models.Cell();
                cellToAdd.PropertyChanged += new XPTable.Events.CellEventHandler(XSLStyleEdit_PropertyChanged);
                cellToAdd.Tag = element;
                cellToAdd.ToolTipText = "VAlign";
                tableXSL.TableModel.Rows[tableXSL.TableModel.Rows.Count - 1].Cells.Add(cellToAdd);

                EnumLocalizer vNone = new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.VerticalAlignment.None);
                if (element.Items.ContainsKey("vertical-align"))
                {
                    EnumLocalizer verticalAlign = verticalElements.Find(delegate(EnumLocalizer loc)
                    {
                        return loc.value.ToString().Equals(element.Items["vertical-align"], StringComparison.InvariantCultureIgnoreCase);
                    });
                    cellToAdd.Text = (verticalAlign != null) ? verticalAlign.ToString() : vNone.ToString();
                }
                else
                    cellToAdd.Text = vNone.ToString();
            }

            SaveFile();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the XSLStyleEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        void XSLStyleEdit_PropertyChanged(object sender, XPTable.Events.CellEventArgs e)
        {
            if (!loading)
            {
                XSLStyleElement element = (e.Cell.Tag as XSLStyleElement);
                if (e.Cell.ToolTipText == "color" || e.Cell.ToolTipText == "background-color")
                {
                    Color color = (Color)e.Cell.Data;
                    if (element.Items.ContainsKey(e.Cell.ToolTipText))
                        element.Items[e.Cell.ToolTipText] = string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
                    else
                        element.Items.Add(e.Cell.ToolTipText, string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B));
                }
                else if (e.Cell.ToolTipText == "HAlign")
                {
                    if (e.Cell.Text == horizontalElements[0].ToString())
                    {
                        element.Items.Remove("text-align");
                    }
                    else
                    {
                        EnumLocalizer textAlign = horizontalElements.Find(delegate(EnumLocalizer loc)
                        {
                            return loc.ToString().Equals(e.Cell.Text, StringComparison.CurrentCultureIgnoreCase);
                        });
                        if (textAlign == null)
                        {
                            element.Items.Remove("text-align");
                        }
                        else
                        {
                            if (element.Items.ContainsKey("text-align"))
                                element.Items["text-align"] = textAlign.value.ToString().ToLower();
                            else
                                element.Items.Add("text-align", textAlign.value.ToString().ToLower());
                        }
                    }

                }
                else if (e.Cell.ToolTipText == "float")
                {
                    if (element.Items.ContainsKey("float"))
                        element.Items["float"] = e.Cell.Text;
                    else
                        element.Items.Add("float", e.Cell.Text);

                    if (e.Cell.Text == horizontalElements[0].ToString())
                        element.Items.Remove("float");
                }
                else if (e.Cell.ToolTipText == "VAlign")
                {
                    if (e.Cell.Text == verticalElements[0].ToString())
                    {
                        element.Items.Remove("vertical-align");
                    }
                    else
                    {
                        EnumLocalizer verticalAlign = verticalElements.Find(delegate(EnumLocalizer loc)
                        {
                            return loc.ToString().Equals(e.Cell.Text, StringComparison.CurrentCultureIgnoreCase);
                        });
                        if (verticalAlign == null)
                        {
                            element.Items.Remove("vertical-align");
                        }
                        else
                        {
                            if (element.Items.ContainsKey("vertical-align"))
                                element.Items["vertical-align"] = verticalAlign.value.ToString().ToLower();
                            else
                                element.Items.Add("vertical-align", verticalAlign.value.ToString().ToLower());
                        }
                    }
                }
                SaveFile();
                ShowPreview();
            }
        }

        /// <summary>
        /// Handles the CellButtonClicked event of the tableXSL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellButtonEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void tableXSL_CellButtonClicked(object sender, XPTable.Events.CellButtonEventArgs e)
        {
            XSLStyleElement element = e.Cell.Tag as XSLStyleElement;

            FontDialog fontdialog = new FontDialog();
            fontdialog.ShowColor = false;
            fontdialog.ShowApply = false;
            fontdialog.ShowEffects = false;
            fontdialog.ShowHelp = false;
            fontdialog.ScriptsOnly = true;
            fontdialog.MinSize = 6;
            fontdialog.MaxSize = 72;
            fontdialog.FontMustExist = true;
            FontStyle style = FontStyle.Regular;
            if (element.Items.ContainsKey("font-weight") && element.Items["font-weight"].ToLower() == "bold")
                style |= FontStyle.Bold;
            if (element.Items.ContainsKey("font-style") && element.Items["font-style"].ToLower() == "italic")
                style |= FontStyle.Italic;

            string fontFace = element.Items.ContainsKey("font-family") ? element.Items["font-family"] : "Arial";
            int defaultFontSize = 12;
            int fontSize = defaultFontSize;
            if (element.Items.ContainsKey("font-size") && !element.Items["font-size"].Contains("%"))
            {
                if (element.Items["font-size"].EndsWith("pt"))
                {
                    if (!Int32.TryParse(element.Items["font-size"].Replace("pt", String.Empty), out fontSize))
                        fontSize = defaultFontSize;
                }
                else
                {
                    if (!Int32.TryParse(element.Items["font-size"], out fontSize))
                        fontSize = defaultFontSize;
                }
            }

            try		// to handle a rare exception that can be thrown when the user selects an invalid font
            {
                fontdialog.Font = new Font(fontFace, fontSize, style);
            }
            catch
            {
                fontdialog.Font = new Font("Microsoft Sans Serif", fontSize, style);
            }

            if (fontdialog.ShowDialog() == DialogResult.OK)
            {
                if (element.Items.ContainsKey("font-size"))
                    element.Items["font-size"] = Math.Round(fontdialog.Font.Size, 0).ToString() + "pt";
                else
                    element.Items.Add("font-size", Math.Round(fontdialog.Font.Size, 0).ToString() + "pt");

                if (element.Items.ContainsKey("font-style"))
                    element.Items["font-style"] = fontdialog.Font.Italic ? "italic" : "normal";
                else
                    element.Items.Add("font-style", fontdialog.Font.Italic ? "italic" : "normal");

                if (element.Items.ContainsKey("font-weight"))
                    element.Items["font-weight"] = fontdialog.Font.Bold ? "bold" : "normal";
                else
                    element.Items.Add("font-weight", fontdialog.Font.Bold ? "bold" : "normal");

                if (element.Items.ContainsKey("font-family"))
                    element.Items["font-family"] = fontdialog.Font.FontFamily.Name;
                else
                    element.Items.Add("font-family", fontdialog.Font.FontFamily.Name);

                SaveFile();
                ShowPreview();
            }
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void SaveFile()
        {
            StreamWriter writer = new StreamWriter(filename, false, System.Text.Encoding.Unicode);
            writer.Write(header);
            foreach (XSLStyleElement element in xslElements)
            {
                writer.Write("\n\t" + element.Name);
                writer.Write("\n\t{");
                foreach (KeyValuePair<string, string> pair in element.Items)
                    writer.Write("\n\t\t" + pair.Key + ":" +
                        (Graphics.FromHwnd(this.Handle).MeasureString(pair.Key, new Font("Arial", 12)).Width < 80 ? "\t\t" : "\t") +
                        pair.Value + ";");
                writer.Write("\n\t}\n");
            }
            writer.Write(footer);
            writer.Close();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the tabControlMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void tabControlMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            loading = true;
            switch (tabControlMode.SelectedIndex)
            {
                case 0:
                    if (textchanged)
                    {
                        timerTextChanged.Stop();
                        WriteTextFile(false);
                    }
                    LoadFile(true);
                    break;
                case 1:
                    LoadFile(false);
                    //textBoxFile.LoadFile(filename, RichTextBoxStreamType.UnicodePlainText);
                    string[] content = File.ReadAllText(filename).Split(new string[] { "<![CDATA[", "]]>" }, StringSplitOptions.None);
                    if (content.Length == 3)
                    {
                        textBoxFile.Text = content[1];
                        if (textBoxFile.Text.Length > 0)
                            textBoxFile.Text = textBoxFile.Text.Substring(1);
                    }
                    break;
            }
            loading = false;
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        private void textBoxFile_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                textchanged = true;
                timerTextChanged.Stop();
                timerTextChanged.Start();
            }
        }

        /// <summary>
        /// Writes the Text file.
        /// </summary>
        /// <param name="supresserrors">if set to <c>true</c> [supresserrors].</param>
        /// <remarks>Documented by Dev02, 2008-02-20</remarks>
        private void WriteTextFile(bool supresserrors)
        {
            try
            {
                File.WriteAllText(filename, header + textBoxFile.Text + footer, System.Text.Encoding.Unicode);
                textchanged = false;
                if (!IsClosing)
                    ShowPreview();
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Could not write style file: " + exp.ToString());
                if (!supresserrors)
                    throw;
            }
        }

        /// <summary>
        /// Handles the Tick event of the timerTextChanged control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-20</remarks>
        private void timerTextChanged_Tick(object sender, EventArgs e)
        {
            if (this.Enabled)
                WriteTextFile(true);
            timerTextChanged.Stop();
        }

        /// <summary>
        /// Handles the Leave event of the textBoxFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-07</remarks>
        private void textBoxFile_Leave(object sender, EventArgs e)
        {
            if (textchanged && this.Enabled)
            {
                timerTextChanged.Stop();
                WriteTextFile(true);
            }
        }

        public bool IsClosing = false;

        /// <summary>
        /// Saves this instance and stops the timer.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-03-05</remarks>
        public void Save()
        {
            if (timerTextChanged != null) timerTextChanged.Stop();
            this.stylePreview.Stop();

            switch (tabControlMode.SelectedIndex)
            {
                case 0:
                    SaveFile();
                    break;
                case 1:
                    WriteTextFile(false);
                    break;
            }
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the stylePreview control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PreviewKeyDownEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-09</remarks>
        private void stylePreview_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //[ML-1193] Forward F1 to the usercontrol in case it was pressed in the browser control
            if (e.KeyCode == Keys.F1)
            {
                this.Focus();
                SendKeys.Send("{F1}");
            }
        }
    }

    class XSLStyleElement
    {
        public string Name;
        public Dictionary<string, string> Items = new Dictionary<string, string>();
    }
}
