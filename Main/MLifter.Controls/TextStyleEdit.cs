using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MLifter.DAL;
using MLifter.DAL.Interfaces;

namespace MLifter.Controls
{
    public partial class TextStyleEdit : UserControl
    {
        private bool actualizing = false;

        private ITextStyle style;
        public ITextStyle Style
        {
            get { return style; }
            set { style = value; LoadStyle(); }
        }

        public event EventHandler Changed;
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private Dictionary<FontFamily, FontFamilyContainer> families = new Dictionary<FontFamily, FontFamilyContainer>();

        public TextStyleEdit()
        {
            InitializeComponent();

            tableOtherElements.HeaderRenderer = new XPTable.Renderers.GradientHeaderRenderer();

            //manually localized resource strings
            tableOtherElements.NoItemsText = Properties.Resources.STYLEEDIT_NOITEMSTEXT;
            buttonColumnDelete.ToolTipText = Properties.Resources.STYLEEDIT_TOOLTIP_DELETE;
            textColumnName.Text = Properties.Resources.STYLEEDIT_COLUMN_STYLENAME;
            textColumnValue.Text = Properties.Resources.STYLEEDIT_COLUMN_STYLEVALUE;

            foreach (FontFamily family in FontFamily.Families)
            {
                families.Add(family, new FontFamilyContainer(family));
                comboBoxFontFamily.Items.Add(families[family]);
            }

            comboBoxFontSizeUnit.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, FontSizeUnit.Pixel));
            comboBoxFontSizeUnit.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, FontSizeUnit.Percent));

            comboBoxHAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.None));
            comboBoxHAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.Left));
            comboBoxHAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.Center));
            comboBoxHAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, MLifter.DAL.Interfaces.HorizontalAlignment.Right));

            comboBoxVAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, VerticalAlignment.None));
            comboBoxVAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, VerticalAlignment.Top));
            comboBoxVAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, VerticalAlignment.Middle));
            comboBoxVAlign.Items.Add(new EnumLocalizer(Properties.Resources.ResourceManager, VerticalAlignment.Bottom));
        }

        /// <summary>
        /// Loads the style.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void LoadStyle()
        {
            if (style == null)
                return;

            actualizing = true;

            comboBoxColorPickerBack.Color = style.BackgroundColor != Color.Empty ? style.BackgroundColor : Color.White;
            comboBoxColorPickerFore.Color = style.ForeColor != Color.Empty ? style.ForeColor : Color.White;

            if (style.FontFamily != null)
                comboBoxFontFamily.SelectedItem = families[style.FontFamily];
            else
                comboBoxFontFamily.SelectedIndex = 0;

            checkBoxFontStyleNone.Checked = style.FontStyle == CSSFontStyle.None;
            checkBoxFontStyleRegular.Checked = (style.FontStyle & CSSFontStyle.Regular) == CSSFontStyle.Regular;
            checkBoxFontStyleBold.Checked = (style.FontStyle & CSSFontStyle.Bold) == CSSFontStyle.Bold;
            checkBoxFontStyleItalic.Checked = (style.FontStyle & CSSFontStyle.Italic) == CSSFontStyle.Italic;
            checkBoxFontStyleStrikeout.Checked = (style.FontStyle & CSSFontStyle.Strikeout) == CSSFontStyle.Strikeout;
            checkBoxFontStyleUnderline.Checked = (style.FontStyle & CSSFontStyle.Underline) == CSSFontStyle.Underline;

            EnumLocalizer.SelectItem(comboBoxFontSizeUnit, style.FontSizeUnit);
            numericUpDownFontSize.Value = (style.FontSize == 0) ? 12 : style.FontSize;
            EnumLocalizer.SelectItem(comboBoxHAlign, style.HorizontalAlign);
            EnumLocalizer.SelectItem(comboBoxVAlign, style.VerticalAlign);

            checkBoxBackColor.Checked = style.BackgroundColor.Name != "Empty" && style.BackgroundColor.Name != "0";
            checkBoxFontFamily.Checked = style.FontFamily != null;
            checkBoxFontSize.Checked = style.FontSize > 0;
            checkBoxForeColor.Checked = style.ForeColor.Name != "Empty" && style.ForeColor.Name != "0";
            checkBoxHAlign.Checked = style.HorizontalAlign != MLifter.DAL.Interfaces.HorizontalAlignment.None;
            checkBoxVAlign.Checked = style.VerticalAlign != VerticalAlignment.None;

            foreach (KeyValuePair<string, string> pair in style.OtherElements)
            {
                XPTable.Models.Row row = new XPTable.Models.Row();
                tableOtherElements.TableModel.Rows.Add(row);
                row.Cells.Add(new XPTable.Models.Cell("x"));
                row.Cells.Add(new XPTable.Models.Cell(pair.Key));
                row.Cells.Add(new XPTable.Models.Cell(pair.Value));
            }

            AddEmtyRow();

            EnableControls();
            actualizing = false;
        }

        /// <summary>
        /// Adds the emty row.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-02</remarks>
        private void AddEmtyRow()
        {
            List<XPTable.Models.Row> rowsToDelete = new List<XPTable.Models.Row>();

            foreach (XPTable.Models.Row rowToCheck in tableOtherElements.TableModel.Rows)
            {
                if (rowToCheck.Cells[1].Text.Length == 0 && rowToCheck.Cells[2].Text.Length == 0)
                    rowsToDelete.Add(rowToCheck);
            }

            foreach (XPTable.Models.Row rowToDelete in rowsToDelete)
                tableOtherElements.TableModel.Rows.Remove(rowToDelete);

            XPTable.Models.Row row = new XPTable.Models.Row();
            tableOtherElements.TableModel.Rows.Add(row);
            row.Cells.Add(new XPTable.Models.Cell("x"));
            row.Cells.Add(new XPTable.Models.Cell(""));
            row.Cells.Add(new XPTable.Models.Cell(""));
        }

        /// <summary>
        /// Values the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void ValueChanged(object sender, EventArgs e)
        {
            if (!actualizing)
                Actualize(sender);
        }

        /// <summary>
        /// Actualizes this instance.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void Actualize(object sender)
        {
            actualizing = true;

            EnableControls();
            AddEmtyRow();

            style.OtherElements.Clear();
            foreach (XPTable.Models.Row rowToCheck in tableOtherElements.TableModel.Rows)
            {
                rowToCheck.Cells[1].Text = rowToCheck.Cells[1].Text.Trim();
                if (rowToCheck.Cells[1].Text.Length != 0 && rowToCheck.Cells[2].Text.Length != 0)
                {
                    if (style.OtherElements.ContainsKey(rowToCheck.Cells[1].Text))
                        style.OtherElements[rowToCheck.Cells[1].Text] = rowToCheck.Cells[2].Text;
                    else
                        style.OtherElements.Add(rowToCheck.Cells[1].Text, rowToCheck.Cells[2].Text);
                }
            }

            //if (!checkBoxFontStyleNone.Checked && !checkBoxFontStyleStrikeout.Checked && !checkBoxFontStyleUnderline.Checked)
            //    checkBoxFontStyleRegular.Checked = true;
            //else
            //    checkBoxFontStyleRegular.Checked = false;

            if (checkBoxForeColor.Checked)
                style.ForeColor = comboBoxColorPickerFore.Color;
            else
                style.ForeColor = Color.Empty;

            if (checkBoxBackColor.Checked)
                style.BackgroundColor = comboBoxColorPickerBack.Color;
            else
                style.BackgroundColor = Color.Empty;

            if (checkBoxFontFamily.Checked)
                style.FontFamily = (comboBoxFontFamily.SelectedItem as FontFamilyContainer).FontFamily;
            else
                style.FontFamily = null;

            if (checkBoxFontStyleNone.Checked)
                style.FontStyle = CSSFontStyle.None;
            else
            {
                if (sender.Equals(checkBoxFontStyleRegular) && ((CheckBox)sender).Checked)
                {
                    checkBoxFontStyleUnderline.Checked = false;
                    checkBoxFontStyleStrikeout.Checked = false;
                }
                if ((sender.Equals(checkBoxFontStyleUnderline) || sender.Equals(checkBoxFontStyleStrikeout)) && ((CheckBox)sender).Checked)
                {
                    if (sender.Equals(checkBoxFontStyleStrikeout) && ((CheckBox)sender).Checked)
                        checkBoxFontStyleUnderline.Checked = false;
                    if (sender.Equals(checkBoxFontStyleUnderline) && ((CheckBox)sender).Checked)
                        checkBoxFontStyleStrikeout.Checked = false;
                    checkBoxFontStyleRegular.Checked = false;
                }

                if (checkBoxFontStyleRegular.Checked)
                {
                    style.FontStyle = CSSFontStyle.Regular;
                }
                else
                {
                    if (checkBoxFontStyleUnderline.Checked)
                        style.FontStyle = CSSFontStyle.Underline;
                    else if (checkBoxFontStyleStrikeout.Checked)
                        style.FontStyle = CSSFontStyle.Strikeout;
                }

                if (checkBoxFontStyleBold.Checked)
                    style.FontStyle |= CSSFontStyle.Bold;
                if (checkBoxFontStyleItalic.Checked)
                    style.FontStyle |= CSSFontStyle.Italic;
            }

            if (checkBoxFontSize.Checked)
                style.FontSize = (int)numericUpDownFontSize.Value;
            else
                style.FontSize = 0;
            style.FontSizeUnit = (FontSizeUnit)((EnumLocalizer)comboBoxFontSizeUnit.SelectedItem).value;

            if (checkBoxHAlign.Checked)
                style.HorizontalAlign = (MLifter.DAL.Interfaces.HorizontalAlignment)((EnumLocalizer)comboBoxHAlign.SelectedItem).value;
            else
                style.HorizontalAlign = MLifter.DAL.Interfaces.HorizontalAlignment.None;

            if (checkBoxVAlign.Checked)
                style.VerticalAlign = (VerticalAlignment)((EnumLocalizer)comboBoxVAlign.SelectedItem).value;
            else
                style.VerticalAlign = VerticalAlignment.None;

            OnChanged(EventArgs.Empty);

            actualizing = false;
        }

        /// <summary>
        /// Enables the controls.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void EnableControls()
        {
            comboBoxColorPickerBack.Enabled = checkBoxBackColor.Checked;
            comboBoxColorPickerFore.Enabled = checkBoxForeColor.Checked;
            comboBoxFontFamily.Enabled = checkBoxFontFamily.Checked;
            comboBoxFontSizeUnit.Enabled = checkBoxFontSize.Checked;
            comboBoxHAlign.Enabled = checkBoxHAlign.Checked;
            comboBoxVAlign.Enabled = checkBoxVAlign.Checked;

            numericUpDownFontSize.Enabled = checkBoxFontSize.Checked;

            checkBoxFontStyleRegular.Enabled = !checkBoxFontStyleNone.Checked;
            checkBoxFontStyleBold.Enabled = !checkBoxFontStyleNone.Checked;
            checkBoxFontStyleItalic.Enabled = !checkBoxFontStyleNone.Checked;
            checkBoxFontStyleStrikeout.Enabled = !checkBoxFontStyleNone.Checked;
            checkBoxFontStyleUnderline.Enabled = !checkBoxFontStyleNone.Checked;
        }

        /// <summary>
        /// Handles the CellButtonClicked event of the tableOtherElements control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellButtonEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-02</remarks>
        private void tableOtherElements_CellButtonClicked(object sender, XPTable.Events.CellButtonEventArgs e)
        {
            tableOtherElements.TableModel.Rows.RemoveAt(e.Cell.Row.Index);
            if (!actualizing)
                Actualize(sender);
        }

        /// <summary>
        /// Handles the CellPropertyChanged event of the tableOtherElements control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-02</remarks>
        private void tableOtherElements_CellPropertyChanged(object sender, XPTable.Events.CellEventArgs e)
        {
            if (!actualizing)
                Actualize(sender);
        }
    }

    public class FontFamilyContainer
    {
        public FontFamily FontFamily;

        public FontFamilyContainer(FontFamily family)
        {
            FontFamily = family;
        }

        public override string ToString()
        {
            return FontFamily.Name;
        }
    }
}
