namespace MLifter.Controls
{
    partial class TextStyleEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextStyleEdit));
            this.checkBoxForeColor = new System.Windows.Forms.CheckBox();
            this.checkBoxBackColor = new System.Windows.Forms.CheckBox();
            this.checkBoxFontStyleNone = new System.Windows.Forms.CheckBox();
            this.groupBoxFontStyle = new System.Windows.Forms.GroupBox();
            this.checkBoxFontStyleRegular = new System.Windows.Forms.CheckBox();
            this.checkBoxFontStyleStrikeout = new System.Windows.Forms.CheckBox();
            this.checkBoxFontStyleItalic = new System.Windows.Forms.CheckBox();
            this.checkBoxFontStyleUnderline = new System.Windows.Forms.CheckBox();
            this.checkBoxFontStyleBold = new System.Windows.Forms.CheckBox();
            this.checkBoxFontFamily = new System.Windows.Forms.CheckBox();
            this.comboBoxFontFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxFontSizeUnit = new System.Windows.Forms.ComboBox();
            this.checkBoxFontSize = new System.Windows.Forms.CheckBox();
            this.numericUpDownFontSize = new System.Windows.Forms.NumericUpDown();
            this.comboBoxVAlign = new System.Windows.Forms.ComboBox();
            this.checkBoxVAlign = new System.Windows.Forms.CheckBox();
            this.comboBoxHAlign = new System.Windows.Forms.ComboBox();
            this.checkBoxHAlign = new System.Windows.Forms.CheckBox();
            this.comboBoxColorPickerBack = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.comboBoxColorPickerFore = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.groupBoxOther = new System.Windows.Forms.GroupBox();
            this.tableOtherElements = new XPTable.Models.Table();
            this.columnModelOtherElements = new XPTable.Models.ColumnModel();
            this.buttonColumnDelete = new XPTable.Models.ButtonColumn();
            this.textColumnName = new XPTable.Models.TextColumn();
            this.textColumnValue = new XPTable.Models.TextColumn();
            this.tableModelOtherElemets = new XPTable.Models.TableModel();
            this.groupBoxFontStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFontSize)).BeginInit();
            this.groupBoxOther.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableOtherElements)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxForeColor
            // 
            resources.ApplyResources(this.checkBoxForeColor, "checkBoxForeColor");
            this.checkBoxForeColor.Name = "checkBoxForeColor";
            this.checkBoxForeColor.UseVisualStyleBackColor = true;
            this.checkBoxForeColor.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxBackColor
            // 
            resources.ApplyResources(this.checkBoxBackColor, "checkBoxBackColor");
            this.checkBoxBackColor.Name = "checkBoxBackColor";
            this.checkBoxBackColor.UseVisualStyleBackColor = true;
            this.checkBoxBackColor.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontStyleNone
            // 
            resources.ApplyResources(this.checkBoxFontStyleNone, "checkBoxFontStyleNone");
            this.checkBoxFontStyleNone.Name = "checkBoxFontStyleNone";
            this.checkBoxFontStyleNone.UseVisualStyleBackColor = true;
            this.checkBoxFontStyleNone.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // groupBoxFontStyle
            // 
            this.groupBoxFontStyle.Controls.Add(this.checkBoxFontStyleRegular);
            this.groupBoxFontStyle.Controls.Add(this.checkBoxFontStyleStrikeout);
            this.groupBoxFontStyle.Controls.Add(this.checkBoxFontStyleItalic);
            this.groupBoxFontStyle.Controls.Add(this.checkBoxFontStyleUnderline);
            this.groupBoxFontStyle.Controls.Add(this.checkBoxFontStyleBold);
            this.groupBoxFontStyle.Controls.Add(this.checkBoxFontStyleNone);
            resources.ApplyResources(this.groupBoxFontStyle, "groupBoxFontStyle");
            this.groupBoxFontStyle.Name = "groupBoxFontStyle";
            this.groupBoxFontStyle.TabStop = false;
            // 
            // checkBoxFontStyleRegular
            // 
            resources.ApplyResources(this.checkBoxFontStyleRegular, "checkBoxFontStyleRegular");
            this.checkBoxFontStyleRegular.Name = "checkBoxFontStyleRegular";
            this.checkBoxFontStyleRegular.UseVisualStyleBackColor = true;
            this.checkBoxFontStyleRegular.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontStyleStrikeout
            // 
            resources.ApplyResources(this.checkBoxFontStyleStrikeout, "checkBoxFontStyleStrikeout");
            this.checkBoxFontStyleStrikeout.Name = "checkBoxFontStyleStrikeout";
            this.checkBoxFontStyleStrikeout.UseVisualStyleBackColor = true;
            this.checkBoxFontStyleStrikeout.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontStyleItalic
            // 
            resources.ApplyResources(this.checkBoxFontStyleItalic, "checkBoxFontStyleItalic");
            this.checkBoxFontStyleItalic.Name = "checkBoxFontStyleItalic";
            this.checkBoxFontStyleItalic.UseVisualStyleBackColor = true;
            this.checkBoxFontStyleItalic.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontStyleUnderline
            // 
            resources.ApplyResources(this.checkBoxFontStyleUnderline, "checkBoxFontStyleUnderline");
            this.checkBoxFontStyleUnderline.Name = "checkBoxFontStyleUnderline";
            this.checkBoxFontStyleUnderline.UseVisualStyleBackColor = true;
            this.checkBoxFontStyleUnderline.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontStyleBold
            // 
            resources.ApplyResources(this.checkBoxFontStyleBold, "checkBoxFontStyleBold");
            this.checkBoxFontStyleBold.Name = "checkBoxFontStyleBold";
            this.checkBoxFontStyleBold.UseVisualStyleBackColor = true;
            this.checkBoxFontStyleBold.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontFamily
            // 
            resources.ApplyResources(this.checkBoxFontFamily, "checkBoxFontFamily");
            this.checkBoxFontFamily.Name = "checkBoxFontFamily";
            this.checkBoxFontFamily.UseVisualStyleBackColor = true;
            this.checkBoxFontFamily.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // comboBoxFontFamily
            // 
            this.comboBoxFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFontFamily.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxFontFamily, "comboBoxFontFamily");
            this.comboBoxFontFamily.Name = "comboBoxFontFamily";
            this.comboBoxFontFamily.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // comboBoxFontSizeUnit
            // 
            this.comboBoxFontSizeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFontSizeUnit.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxFontSizeUnit, "comboBoxFontSizeUnit");
            this.comboBoxFontSizeUnit.Name = "comboBoxFontSizeUnit";
            this.comboBoxFontSizeUnit.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxFontSize
            // 
            resources.ApplyResources(this.checkBoxFontSize, "checkBoxFontSize");
            this.checkBoxFontSize.Name = "checkBoxFontSize";
            this.checkBoxFontSize.UseVisualStyleBackColor = true;
            this.checkBoxFontSize.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // numericUpDownFontSize
            // 
            resources.ApplyResources(this.numericUpDownFontSize, "numericUpDownFontSize");
            this.numericUpDownFontSize.Name = "numericUpDownFontSize";
            this.numericUpDownFontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownFontSize.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // comboBoxVAlign
            // 
            this.comboBoxVAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVAlign.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxVAlign, "comboBoxVAlign");
            this.comboBoxVAlign.Name = "comboBoxVAlign";
            this.comboBoxVAlign.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxVAlign
            // 
            resources.ApplyResources(this.checkBoxVAlign, "checkBoxVAlign");
            this.checkBoxVAlign.Name = "checkBoxVAlign";
            this.checkBoxVAlign.UseVisualStyleBackColor = true;
            this.checkBoxVAlign.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // comboBoxHAlign
            // 
            this.comboBoxHAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHAlign.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxHAlign, "comboBoxHAlign");
            this.comboBoxHAlign.Name = "comboBoxHAlign";
            this.comboBoxHAlign.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // checkBoxHAlign
            // 
            resources.ApplyResources(this.checkBoxHAlign, "checkBoxHAlign");
            this.checkBoxHAlign.Name = "checkBoxHAlign";
            this.checkBoxHAlign.UseVisualStyleBackColor = true;
            this.checkBoxHAlign.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // comboBoxColorPickerBack
            // 
            this.comboBoxColorPickerBack.Color = System.Drawing.Color.Black;
            this.comboBoxColorPickerBack.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxColorPickerBack.DropDownHeight = 1;
            this.comboBoxColorPickerBack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColorPickerBack.DropDownWidth = 1;
            this.comboBoxColorPickerBack.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxColorPickerBack, "comboBoxColorPickerBack");
            this.comboBoxColorPickerBack.Items.AddRange(new object[] {
            resources.GetString("comboBoxColorPickerBack.Items"),
            resources.GetString("comboBoxColorPickerBack.Items1"),
            resources.GetString("comboBoxColorPickerBack.Items2"),
            resources.GetString("comboBoxColorPickerBack.Items3"),
            resources.GetString("comboBoxColorPickerBack.Items4"),
            resources.GetString("comboBoxColorPickerBack.Items5"),
            resources.GetString("comboBoxColorPickerBack.Items6"),
            resources.GetString("comboBoxColorPickerBack.Items7"),
            resources.GetString("comboBoxColorPickerBack.Items8"),
            resources.GetString("comboBoxColorPickerBack.Items9"),
            resources.GetString("comboBoxColorPickerBack.Items10"),
            resources.GetString("comboBoxColorPickerBack.Items11"),
            resources.GetString("comboBoxColorPickerBack.Items12"),
            resources.GetString("comboBoxColorPickerBack.Items13"),
            resources.GetString("comboBoxColorPickerBack.Items14"),
            resources.GetString("comboBoxColorPickerBack.Items15"),
            resources.GetString("comboBoxColorPickerBack.Items16"),
            resources.GetString("comboBoxColorPickerBack.Items17"),
            resources.GetString("comboBoxColorPickerBack.Items18"),
            resources.GetString("comboBoxColorPickerBack.Items19"),
            resources.GetString("comboBoxColorPickerBack.Items20"),
            resources.GetString("comboBoxColorPickerBack.Items21")});
            this.comboBoxColorPickerBack.Name = "comboBoxColorPickerBack";
            this.comboBoxColorPickerBack.SelectedColorChanged += new System.EventHandler(this.ValueChanged);
            // 
            // comboBoxColorPickerFore
            // 
            this.comboBoxColorPickerFore.Color = System.Drawing.Color.Black;
            this.comboBoxColorPickerFore.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxColorPickerFore.DropDownHeight = 1;
            this.comboBoxColorPickerFore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColorPickerFore.DropDownWidth = 1;
            this.comboBoxColorPickerFore.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxColorPickerFore, "comboBoxColorPickerFore");
            this.comboBoxColorPickerFore.Items.AddRange(new object[] {
            resources.GetString("comboBoxColorPickerFore.Items"),
            resources.GetString("comboBoxColorPickerFore.Items1"),
            resources.GetString("comboBoxColorPickerFore.Items2"),
            resources.GetString("comboBoxColorPickerFore.Items3"),
            resources.GetString("comboBoxColorPickerFore.Items4"),
            resources.GetString("comboBoxColorPickerFore.Items5"),
            resources.GetString("comboBoxColorPickerFore.Items6"),
            resources.GetString("comboBoxColorPickerFore.Items7"),
            resources.GetString("comboBoxColorPickerFore.Items8"),
            resources.GetString("comboBoxColorPickerFore.Items9"),
            resources.GetString("comboBoxColorPickerFore.Items10"),
            resources.GetString("comboBoxColorPickerFore.Items11"),
            resources.GetString("comboBoxColorPickerFore.Items12"),
            resources.GetString("comboBoxColorPickerFore.Items13"),
            resources.GetString("comboBoxColorPickerFore.Items14"),
            resources.GetString("comboBoxColorPickerFore.Items15"),
            resources.GetString("comboBoxColorPickerFore.Items16"),
            resources.GetString("comboBoxColorPickerFore.Items17"),
            resources.GetString("comboBoxColorPickerFore.Items18"),
            resources.GetString("comboBoxColorPickerFore.Items19"),
            resources.GetString("comboBoxColorPickerFore.Items20")});
            this.comboBoxColorPickerFore.Name = "comboBoxColorPickerFore";
            this.comboBoxColorPickerFore.SelectedColorChanged += new System.EventHandler(this.ValueChanged);
            // 
            // groupBoxOther
            // 
            this.groupBoxOther.Controls.Add(this.tableOtherElements);
            resources.ApplyResources(this.groupBoxOther, "groupBoxOther");
            this.groupBoxOther.Name = "groupBoxOther";
            this.groupBoxOther.TabStop = false;
            // 
            // tableOtherElements
            // 
            resources.ApplyResources(this.tableOtherElements, "tableOtherElements");
            this.tableOtherElements.ColumnModel = this.columnModelOtherElements;
            this.tableOtherElements.EditStartAction = XPTable.Editors.EditStartAction.SingleClick;
            this.tableOtherElements.GridLines = XPTable.Models.GridLines.Both;
            this.tableOtherElements.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableOtherElements.Name = "tableOtherElements";
            this.tableOtherElements.NoItemsText = "There are no items in this view.";
            this.tableOtherElements.TableModel = this.tableModelOtherElemets;
            this.tableOtherElements.CellPropertyChanged += new XPTable.Events.CellEventHandler(this.tableOtherElements_CellPropertyChanged);
            this.tableOtherElements.CellButtonClicked += new XPTable.Events.CellButtonEventHandler(this.tableOtherElements_CellButtonClicked);
            // 
            // columnModelOtherElements
            // 
            this.columnModelOtherElements.Columns.AddRange(new XPTable.Models.Column[] {
            this.buttonColumnDelete,
            this.textColumnName,
            this.textColumnValue});
            // 
            // buttonColumnDelete
            // 
            this.buttonColumnDelete.ToolTipText = "Delete the Element";
            this.buttonColumnDelete.Width = 16;
            // 
            // textColumnName
            // 
            this.textColumnName.Text = "Style Name";
            this.textColumnName.Width = 185;
            // 
            // textColumnValue
            // 
            this.textColumnValue.Text = "Style Value";
            this.textColumnValue.Width = 185;
            // 
            // TextStyleEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupBoxOther);
            this.Controls.Add(this.comboBoxHAlign);
            this.Controls.Add(this.checkBoxHAlign);
            this.Controls.Add(this.comboBoxVAlign);
            this.Controls.Add(this.checkBoxVAlign);
            this.Controls.Add(this.numericUpDownFontSize);
            this.Controls.Add(this.comboBoxFontSizeUnit);
            this.Controls.Add(this.checkBoxFontSize);
            this.Controls.Add(this.comboBoxFontFamily);
            this.Controls.Add(this.checkBoxFontFamily);
            this.Controls.Add(this.groupBoxFontStyle);
            this.Controls.Add(this.checkBoxBackColor);
            this.Controls.Add(this.comboBoxColorPickerBack);
            this.Controls.Add(this.checkBoxForeColor);
            this.Controls.Add(this.comboBoxColorPickerFore);
            resources.ApplyResources(this, "$this");
            this.Name = "TextStyleEdit";
            this.groupBoxFontStyle.ResumeLayout(false);
            this.groupBoxFontStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFontSize)).EndInit();
            this.groupBoxOther.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tableOtherElements)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OfficePickers.ColorPicker.ComboBoxColorPicker comboBoxColorPickerFore;
        private System.Windows.Forms.CheckBox checkBoxForeColor;
        private System.Windows.Forms.CheckBox checkBoxBackColor;
        private OfficePickers.ColorPicker.ComboBoxColorPicker comboBoxColorPickerBack;
        private System.Windows.Forms.CheckBox checkBoxFontStyleNone;
        private System.Windows.Forms.GroupBox groupBoxFontStyle;
        private System.Windows.Forms.CheckBox checkBoxFontStyleStrikeout;
        private System.Windows.Forms.CheckBox checkBoxFontStyleItalic;
        private System.Windows.Forms.CheckBox checkBoxFontStyleUnderline;
        private System.Windows.Forms.CheckBox checkBoxFontStyleBold;
        private System.Windows.Forms.CheckBox checkBoxFontFamily;
        private System.Windows.Forms.ComboBox comboBoxFontFamily;
        private System.Windows.Forms.ComboBox comboBoxFontSizeUnit;
        private System.Windows.Forms.CheckBox checkBoxFontSize;
        private System.Windows.Forms.NumericUpDown numericUpDownFontSize;
        private System.Windows.Forms.ComboBox comboBoxVAlign;
        private System.Windows.Forms.CheckBox checkBoxVAlign;
        private System.Windows.Forms.ComboBox comboBoxHAlign;
        private System.Windows.Forms.CheckBox checkBoxHAlign;
        private System.Windows.Forms.CheckBox checkBoxFontStyleRegular;
        private System.Windows.Forms.GroupBox groupBoxOther;
        private XPTable.Models.Table tableOtherElements;
        private XPTable.Models.ColumnModel columnModelOtherElements;
        private XPTable.Models.ButtonColumn buttonColumnDelete;
        private XPTable.Models.TextColumn textColumnName;
        private XPTable.Models.TextColumn textColumnValue;
        private XPTable.Models.TableModel tableModelOtherElemets;


    }
}
