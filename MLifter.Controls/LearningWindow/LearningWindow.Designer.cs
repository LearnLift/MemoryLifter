namespace MLifter.Controls.LearningWindow
{
    partial class LearningWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearningWindow));
            this.splitContainerHorizontal = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelMainContent = new System.Windows.Forms.TableLayoutPanel();
            this.statisticsPanel = new MLifter.Controls.LearningWindow.StatisticsPanel();
            this.statusBar = new MLifter.Controls.LearningWindow.StatusBar();
            this.panelLearningWindowMiddle = new System.Windows.Forms.Panel();
            this.dockingButtonStack = new MLifter.Components.DockingButton();
            this.gradientPanelMain = new MLifter.Components.GradientPanel();
            this.tableLayoutPanelLearningWindowMiddle = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.questionPanel = new MLifter.Controls.LearningWindow.QuestionPanel();
            this.answerPanel = new MLifter.Controls.LearningWindow.AnswerPanel();
            this.buttonsPanelMainControl = new MLifter.Controls.LearningWindow.ButtonsPanel();
            this.stackFlow = new MLifter.Controls.LearningWindow.StackFlow();
            this.audioPlayerComponent = new MLifter.Controls.LearningWindow.AudioPlayerComponent(this.components);
            this.userDialogComponent = new MLifter.Controls.LearningWindow.UserDialogComponent(this.components);
            this.splitContainerHorizontal.Panel1.SuspendLayout();
            this.splitContainerHorizontal.Panel2.SuspendLayout();
            this.splitContainerHorizontal.SuspendLayout();
            this.tableLayoutPanelMainContent.SuspendLayout();
            this.panelLearningWindowMiddle.SuspendLayout();
            this.gradientPanelMain.SuspendLayout();
            this.tableLayoutPanelLearningWindowMiddle.SuspendLayout();
            this.splitContainerVertical.Panel1.SuspendLayout();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHorizontal
            // 
            resources.ApplyResources(this.splitContainerHorizontal, "splitContainerHorizontal");
            this.splitContainerHorizontal.Name = "splitContainerHorizontal";
            // 
            // splitContainerHorizontal.Panel1
            // 
            this.splitContainerHorizontal.Panel1.Controls.Add(this.tableLayoutPanelMainContent);
            // 
            // splitContainerHorizontal.Panel2
            // 
            this.splitContainerHorizontal.Panel2.Controls.Add(this.stackFlow);
            // 
            // tableLayoutPanelMainContent
            // 
            resources.ApplyResources(this.tableLayoutPanelMainContent, "tableLayoutPanelMainContent");
            this.tableLayoutPanelMainContent.Controls.Add(this.statisticsPanel, 0, 0);
            this.tableLayoutPanelMainContent.Controls.Add(this.statusBar, 0, 3);
            this.tableLayoutPanelMainContent.Controls.Add(this.panelLearningWindowMiddle, 0, 1);
            this.tableLayoutPanelMainContent.Name = "tableLayoutPanelMainContent";
            // 
            // statisticsPanel
            // 
            this.statisticsPanel.BoxCards = 0;
            this.statisticsPanel.BoxNo = 0;
            this.statisticsPanel.BoxSize = 0;
            resources.ApplyResources(this.statisticsPanel, "statisticsPanel");
            this.statisticsPanel.ForeColorTitle = System.Drawing.SystemColors.ControlText;
            this.statisticsPanel.Name = "statisticsPanel";
            this.statisticsPanel.RightCount = 0;
            this.statisticsPanel.Score = 0;
            this.statisticsPanel.Timer = 10;
            this.statisticsPanel.TimerVisible = true;
            this.statisticsPanel.WrongCount = 0;
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.ForeColor = System.Drawing.Color.Red;
            this.statusBar.Name = "statusBar";
            // 
            // panelLearningWindowMiddle
            // 
            this.panelLearningWindowMiddle.Controls.Add(this.dockingButtonStack);
            this.panelLearningWindowMiddle.Controls.Add(this.gradientPanelMain);
            resources.ApplyResources(this.panelLearningWindowMiddle, "panelLearningWindowMiddle");
            this.panelLearningWindowMiddle.Name = "panelLearningWindowMiddle";
            // 
            // dockingButtonStack
            // 
            this.dockingButtonStack.BackColor = System.Drawing.Color.Navy;
            this.dockingButtonStack.DockingParent = this.gradientPanelMain;
            this.dockingButtonStack.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.dockingButtonStack, "dockingButtonStack");
            this.dockingButtonStack.Name = "dockingButtonStack";
            this.dockingButtonStack.PanelBorder = MLifter.Components.PanelBorder.RightSideRounded;
            this.dockingButtonStack.Click += new System.EventHandler(this.dockingButtonStack_Click);
            // 
            // gradientPanelMain
            // 
            this.gradientPanelMain.BackColor = System.Drawing.Color.CornflowerBlue;
            this.gradientPanelMain.Controls.Add(this.tableLayoutPanelLearningWindowMiddle);
            resources.ApplyResources(this.gradientPanelMain, "gradientPanelMain");
            this.gradientPanelMain.Gradient = null;
            this.gradientPanelMain.LayoutSuspended = false;
            this.gradientPanelMain.Name = "gradientPanelMain";
            // 
            // tableLayoutPanelLearningWindowMiddle
            // 
            resources.ApplyResources(this.tableLayoutPanelLearningWindowMiddle, "tableLayoutPanelLearningWindowMiddle");
            this.tableLayoutPanelLearningWindowMiddle.Controls.Add(this.splitContainerVertical, 0, 0);
            this.tableLayoutPanelLearningWindowMiddle.Controls.Add(this.buttonsPanelMainControl, 0, 1);
            this.tableLayoutPanelLearningWindowMiddle.Name = "tableLayoutPanelLearningWindowMiddle";
            // 
            // splitContainerVertical
            // 
            this.splitContainerVertical.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.splitContainerVertical, "splitContainerVertical");
            this.splitContainerVertical.Name = "splitContainerVertical";
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainerVertical.Panel1.Controls.Add(this.questionPanel);
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.Controls.Add(this.answerPanel);
            // 
            // questionPanel
            // 
            this.questionPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.questionPanel, "questionPanel");
            this.questionPanel.Name = "questionPanel";
            this.questionPanel.SizeChanged += new System.EventHandler(this.questionPanel_SizeChanged);
            // 
            // answerPanel
            // 
            this.answerPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.answerPanel, "answerPanel");
            this.answerPanel.Name = "answerPanel";
            // 
            // buttonsPanelMainControl
            // 
            this.buttonsPanelMainControl.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonsPanelMainControl, "buttonsPanelMainControl");
            this.buttonsPanelMainControl.Name = "buttonsPanelMainControl";
            // 
            // stackFlow
            // 
            resources.ApplyResources(this.stackFlow, "stackFlow");
            this.stackFlow.Name = "stackFlow";
            this.stackFlow.SizeChanged += new System.EventHandler(this.stackFlow_SizeChanged);
            // 
            // LearningWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainerHorizontal);
            this.Name = "LearningWindow";
            resources.ApplyResources(this, "$this");
            this.splitContainerHorizontal.Panel1.ResumeLayout(false);
            this.splitContainerHorizontal.Panel2.ResumeLayout(false);
            this.splitContainerHorizontal.ResumeLayout(false);
            this.tableLayoutPanelMainContent.ResumeLayout(false);
            this.panelLearningWindowMiddle.ResumeLayout(false);
            this.gradientPanelMain.ResumeLayout(false);
            this.tableLayoutPanelLearningWindowMiddle.ResumeLayout(false);
            this.splitContainerVertical.Panel1.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            this.splitContainerVertical.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private StatisticsPanel statisticsPanel;
        private System.Windows.Forms.SplitContainer splitContainerHorizontal;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
        private StackFlow stackFlow;
        private QuestionPanel questionPanel;
        private AnswerPanel answerPanel;
        private AudioPlayerComponent audioPlayerComponent;
        private UserDialogComponent userDialogComponent;
        private StatusBar statusBar;
        private ButtonsPanel buttonsPanelMainControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMainContent;
        private MLifter.Components.GradientPanel gradientPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLearningWindowMiddle;
        private System.Windows.Forms.Panel panelLearningWindowMiddle;
        private MLifter.Components.DockingButton dockingButtonStack;

    }
}
