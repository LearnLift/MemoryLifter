namespace MLifter.Controls.Wizards.Print
{
    partial class IndividualSelectionPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndividualSelectionPage));
			this.buttonSelect = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tableCards = new XPTable.Models.Table();
			this.columnModelCards = new XPTable.Models.ColumnModel();
			this.checkBoxColumnSelect = new XPTable.Models.CheckBoxColumn();
			this.textColumnQuestion = new XPTable.Models.TextColumn();
			this.textColumnAnswer = new XPTable.Models.TextColumn();
			this.textColumnBox = new XPTable.Models.TextColumn();
			this.textColumnChapter = new XPTable.Models.TextColumn();
			this.tableModelCards = new XPTable.Models.TableModel();
			((System.ComponentModel.ISupportInitialize)(this.tableCards)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonSelect
			// 
			this.buttonSelect.Image = global::MLifter.Controls.Properties.Resources.listAdd;
			resources.ApplyResources(this.buttonSelect, "buttonSelect");
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.UseVisualStyleBackColor = true;
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Image = global::MLifter.Controls.Properties.Resources.listRemove;
			resources.ApplyResources(this.buttonRemove, "buttonRemove");
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// tableCards
			// 
			this.tableCards.AlternatingRowColor = System.Drawing.Color.Cornsilk;
			this.tableCards.ColumnModel = this.columnModelCards;
			this.tableCards.EnableHeaderContextMenu = false;
			this.tableCards.FullRowSelect = true;
			this.tableCards.GridLines = XPTable.Models.GridLines.Columns;
			this.tableCards.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			resources.ApplyResources(this.tableCards, "tableCards");
			this.tableCards.MultiSelect = true;
			this.tableCards.Name = "tableCards";
			this.tableCards.SortedColumnBackColor = System.Drawing.Color.Transparent;
			this.tableCards.TableModel = this.tableModelCards;
			this.tableCards.CellDoubleClick += new XPTable.Events.CellMouseEventHandler(this.tableCards_CellDoubleClick);
			// 
			// columnModelCards
			// 
			this.columnModelCards.Columns.AddRange(new XPTable.Models.Column[] {
            this.checkBoxColumnSelect,
            this.textColumnQuestion,
            this.textColumnAnswer,
            this.textColumnBox,
            this.textColumnChapter});
			// 
			// checkBoxColumnSelect
			// 
			this.checkBoxColumnSelect.Width = 16;
			// 
			// textColumnQuestion
			// 
			this.textColumnQuestion.Editable = false;
			this.textColumnQuestion.Text = "Question";
			this.textColumnQuestion.Width = 150;
			// 
			// textColumnAnswer
			// 
			this.textColumnAnswer.Editable = false;
			this.textColumnAnswer.Text = "Answer";
			this.textColumnAnswer.Width = 150;
			// 
			// textColumnBox
			// 
			this.textColumnBox.Alignment = XPTable.Models.ColumnAlignment.Center;
			this.textColumnBox.Editable = false;
			this.textColumnBox.Text = "Box";
			this.textColumnBox.Width = 45;
			// 
			// textColumnChapter
			// 
			this.textColumnChapter.Editable = false;
			this.textColumnChapter.Text = "Chapter";
			this.textColumnChapter.Width = 103;
			// 
			// IndividualSelectionPage
			// 
			this.Controls.Add(this.tableCards);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonSelect);
			resources.ApplyResources(this, "$this");
			this.HelpAvailable = true;
			this.Name = "IndividualSelectionPage";
			this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
			this.Load += new System.EventHandler(this.IndividualSelectionPage_Load);
			this.Controls.SetChildIndex(this.buttonSelect, 0);
			this.Controls.SetChildIndex(this.buttonRemove, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.tableCards, 0);
			((System.ComponentModel.ISupportInitialize)(this.tableCards)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Label label1;
        private XPTable.Models.Table tableCards;
        private XPTable.Models.ColumnModel columnModelCards;
        private XPTable.Models.TableModel tableModelCards;
        private XPTable.Models.CheckBoxColumn checkBoxColumnSelect;
        private XPTable.Models.TextColumn textColumnQuestion;
        private XPTable.Models.TextColumn textColumnAnswer;
        private XPTable.Models.TextColumn textColumnBox;
        private XPTable.Models.TextColumn textColumnChapter;
    }
}
