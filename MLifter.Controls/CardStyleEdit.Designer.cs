namespace MLifter.Controls
{
    partial class CardStyleEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardStyleEdit));
            this.stylePreview = new MLifter.Components.MLifterWebBrowser();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageQuestion = new System.Windows.Forms.TabPage();
            this.textStyleEditQuestion = new MLifter.Controls.TextStyleEdit();
            this.tabPageQuestionExample = new System.Windows.Forms.TabPage();
            this.textStyleEditQuestionExample = new MLifter.Controls.TextStyleEdit();
            this.tabPageAnswer = new System.Windows.Forms.TabPage();
            this.textStyleEditAnswer = new MLifter.Controls.TextStyleEdit();
            this.tabPageAnswerExample = new System.Windows.Forms.TabPage();
            this.textStyleEditAnswerExample = new MLifter.Controls.TextStyleEdit();
            this.tabPageCorrect = new System.Windows.Forms.TabPage();
            this.textStyleEditCorrect = new MLifter.Controls.TextStyleEdit();
            this.tabPageWrong = new System.Windows.Forms.TabPage();
            this.textStyleEditWrong = new MLifter.Controls.TextStyleEdit();
            this.tabControlMain.SuspendLayout();
            this.tabPageQuestion.SuspendLayout();
            this.tabPageQuestionExample.SuspendLayout();
            this.tabPageAnswer.SuspendLayout();
            this.tabPageAnswerExample.SuspendLayout();
            this.tabPageCorrect.SuspendLayout();
            this.tabPageWrong.SuspendLayout();
            this.SuspendLayout();
            // 
            // stylePreview
            // 
            resources.ApplyResources(this.stylePreview, "stylePreview");
            this.stylePreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.stylePreview.Name = "stylePreview";
            this.stylePreview.Tag = "";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageQuestion);
            this.tabControlMain.Controls.Add(this.tabPageQuestionExample);
            this.tabControlMain.Controls.Add(this.tabPageAnswer);
            this.tabControlMain.Controls.Add(this.tabPageAnswerExample);
            this.tabControlMain.Controls.Add(this.tabPageCorrect);
            this.tabControlMain.Controls.Add(this.tabPageWrong);
            resources.ApplyResources(this.tabControlMain, "tabControlMain");
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
            // 
            // tabPageQuestion
            // 
            this.tabPageQuestion.Controls.Add(this.textStyleEditQuestion);
            resources.ApplyResources(this.tabPageQuestion, "tabPageQuestion");
            this.tabPageQuestion.Name = "tabPageQuestion";
            this.tabPageQuestion.UseVisualStyleBackColor = true;
            // 
            // textStyleEditQuestion
            // 
            resources.ApplyResources(this.textStyleEditQuestion, "textStyleEditQuestion");
            this.textStyleEditQuestion.Name = "textStyleEditQuestion";
            this.textStyleEditQuestion.Style = null;
            this.textStyleEditQuestion.Changed += new System.EventHandler(this.CardStyle_Changed);
            // 
            // tabPageQuestionExample
            // 
            this.tabPageQuestionExample.Controls.Add(this.textStyleEditQuestionExample);
            resources.ApplyResources(this.tabPageQuestionExample, "tabPageQuestionExample");
            this.tabPageQuestionExample.Name = "tabPageQuestionExample";
            this.tabPageQuestionExample.UseVisualStyleBackColor = true;
            // 
            // textStyleEditQuestionExample
            // 
            resources.ApplyResources(this.textStyleEditQuestionExample, "textStyleEditQuestionExample");
            this.textStyleEditQuestionExample.Name = "textStyleEditQuestionExample";
            this.textStyleEditQuestionExample.Style = null;
            this.textStyleEditQuestionExample.Changed += new System.EventHandler(this.CardStyle_Changed);
            // 
            // tabPageAnswer
            // 
            this.tabPageAnswer.Controls.Add(this.textStyleEditAnswer);
            resources.ApplyResources(this.tabPageAnswer, "tabPageAnswer");
            this.tabPageAnswer.Name = "tabPageAnswer";
            this.tabPageAnswer.UseVisualStyleBackColor = true;
            // 
            // textStyleEditAnswer
            // 
            resources.ApplyResources(this.textStyleEditAnswer, "textStyleEditAnswer");
            this.textStyleEditAnswer.Name = "textStyleEditAnswer";
            this.textStyleEditAnswer.Style = null;
            this.textStyleEditAnswer.Changed += new System.EventHandler(this.CardStyle_Changed);
            // 
            // tabPageAnswerExample
            // 
            this.tabPageAnswerExample.Controls.Add(this.textStyleEditAnswerExample);
            resources.ApplyResources(this.tabPageAnswerExample, "tabPageAnswerExample");
            this.tabPageAnswerExample.Name = "tabPageAnswerExample";
            this.tabPageAnswerExample.UseVisualStyleBackColor = true;
            // 
            // textStyleEditAnswerExample
            // 
            resources.ApplyResources(this.textStyleEditAnswerExample, "textStyleEditAnswerExample");
            this.textStyleEditAnswerExample.Name = "textStyleEditAnswerExample";
            this.textStyleEditAnswerExample.Style = null;
            this.textStyleEditAnswerExample.Changed += new System.EventHandler(this.CardStyle_Changed);
            // 
            // tabPageCorrect
            // 
            this.tabPageCorrect.Controls.Add(this.textStyleEditCorrect);
            resources.ApplyResources(this.tabPageCorrect, "tabPageCorrect");
            this.tabPageCorrect.Name = "tabPageCorrect";
            this.tabPageCorrect.UseVisualStyleBackColor = true;
            // 
            // textStyleEditCorrect
            // 
            resources.ApplyResources(this.textStyleEditCorrect, "textStyleEditCorrect");
            this.textStyleEditCorrect.Name = "textStyleEditCorrect";
            this.textStyleEditCorrect.Style = null;
            this.textStyleEditCorrect.Changed += new System.EventHandler(this.CardStyle_Changed);
            // 
            // tabPageWrong
            // 
            this.tabPageWrong.Controls.Add(this.textStyleEditWrong);
            resources.ApplyResources(this.tabPageWrong, "tabPageWrong");
            this.tabPageWrong.Name = "tabPageWrong";
            this.tabPageWrong.UseVisualStyleBackColor = true;
            // 
            // textStyleEditWrong
            // 
            resources.ApplyResources(this.textStyleEditWrong, "textStyleEditWrong");
            this.textStyleEditWrong.Name = "textStyleEditWrong";
            this.textStyleEditWrong.Style = null;
            this.textStyleEditWrong.Changed += new System.EventHandler(this.CardStyle_Changed);
            // 
            // CardStyleEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.stylePreview);
            resources.ApplyResources(this, "$this");
            this.Name = "CardStyleEdit";
            this.Resize += new System.EventHandler(this.CardStyleEdit_Resize);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageQuestion.ResumeLayout(false);
            this.tabPageQuestionExample.ResumeLayout(false);
            this.tabPageAnswer.ResumeLayout(false);
            this.tabPageAnswerExample.ResumeLayout(false);
            this.tabPageCorrect.ResumeLayout(false);
            this.tabPageWrong.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MLifter.Components.MLifterWebBrowser stylePreview;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageQuestion;
        private System.Windows.Forms.TabPage tabPageQuestionExample;
        private System.Windows.Forms.TabPage tabPageAnswer;
        private System.Windows.Forms.TabPage tabPageAnswerExample;
        private System.Windows.Forms.TabPage tabPageCorrect;
        private System.Windows.Forms.TabPage tabPageWrong;
        private TextStyleEdit textStyleEditQuestion;
        private TextStyleEdit textStyleEditQuestionExample;
        private TextStyleEdit textStyleEditAnswer;
        private TextStyleEdit textStyleEditAnswerExample;
        private TextStyleEdit textStyleEditCorrect;
        private TextStyleEdit textStyleEditWrong;
    }
}
