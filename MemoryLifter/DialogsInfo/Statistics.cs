using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ZedGraph;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Properties;
using MLifter.BusinessLayer;
using System.Collections.Generic;

namespace MLifter
{
    /// <summary>
    /// Shows the Statistics saved in the XML File / the Statistics.Statistics Array in Dictionary
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-20</remarks>
    public class TStatsForm : System.Windows.Forms.Form
    {
        #region Initialization and Constructors

        private System.Windows.Forms.Button btnOkay;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TabControl PCStats;
        private System.Windows.Forms.TabPage TSRightWrong;
        private System.Windows.Forms.TabPage TSDistribution;
        private System.Windows.Forms.TabPage TSOther;
        private System.Windows.Forms.Label LKnowledge;
        private System.Windows.Forms.Label LMemory;
        private System.Windows.Forms.Button btnPrintMemory;
        private System.Windows.Forms.Label LblTimeFrame;
        private System.Windows.Forms.ComboBox CBShow;
        private System.Windows.Forms.Button btnPrintKnowledge;
        private System.Windows.Forms.Label LblWSum;
        private System.Windows.Forms.Label LblRSum;
        private System.Windows.Forms.Label LblQryCards;
        private System.Windows.Forms.Label LblTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LNote;
        private System.Windows.Forms.Label LblPercent;
        private System.Windows.Forms.Label LPercKnown;
        private System.Windows.Forms.Button btnPrintOther;
        private System.Windows.Forms.Label LKnowledgeDesc;
        private System.Windows.Forms.Label LOtherDesc;
        private ZedGraph.ZedGraphControl zedGraphControlKnowledge;
        private ZedGraphControl zedGraphControlMemoryDistribution;
        private ZedGraphControl zedGraphControlCurrentDistribution;
        private HelpProvider MainHelp;
        private CheckBox checkBoxShowTarget;

        private bool loading = false;

        private Color[] GradientColors = new Color[11]
			{Color.Red, Color.FromArgb(0xFF,0x55,0x00), Color.FromArgb(0xFF,0xAA,0x00),
			Color.FromArgb(0xFF,0xFF,0x00), Color.FromArgb(0xB0,0xFF,0x00), Color.FromArgb(0x00,0xFF,0x00),
			Color.FromArgb(0x00,0xFF,0xB0), Color.FromArgb(0x00,0xFF,0xFF), Color.FromArgb(0x00,0x80,0xFF),
			Color.FromArgb(0x00,0x00,0xFF), Color.Maroon};

        /// <summary>
        /// Enumerator that defines the available time frames
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        private enum TimeFrame : int
        {
            All = 0,
            Today = 1,
            Week = 2,
            Month = 3,
            Year = 4
        }

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Constructor - Reads the prefered time frame from the registry and calls CBShow_SelectedIndexChange to load the data.
        /// </summary>
        public TStatsForm()
        {
            loading = true;
            InitializeComponent();

            checkBoxShowTarget.Checked = Settings.Default.STATISTICS_ShowTargetLine;

            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
            int reg_entry = Settings.Default.Statistics;
            if (reg_entry >= 0 && reg_entry <= 4)
                CBShow.SelectedIndex = reg_entry;
            else
                CBShow.SelectedIndex = 1;


            CBShow_SelectedIndexChange(null, null);
            PCStats.SelectedTab = TSRightWrong;

            loading = false;
        }

        private void TStatsForm_Load(object sender, EventArgs e)
        {
            DrawStatistics();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TStatsForm));
            this.PCStats = new System.Windows.Forms.TabControl();
            this.TSRightWrong = new System.Windows.Forms.TabPage();
            this.checkBoxShowTarget = new System.Windows.Forms.CheckBox();
            this.zedGraphControlKnowledge = new ZedGraph.ZedGraphControl();
            this.LKnowledgeDesc = new System.Windows.Forms.Label();
            this.btnPrintKnowledge = new System.Windows.Forms.Button();
            this.LKnowledge = new System.Windows.Forms.Label();
            this.TSDistribution = new System.Windows.Forms.TabPage();
            this.zedGraphControlMemoryDistribution = new ZedGraph.ZedGraphControl();
            this.btnPrintMemory = new System.Windows.Forms.Button();
            this.LMemory = new System.Windows.Forms.Label();
            this.TSOther = new System.Windows.Forms.TabPage();
            this.btnPrintOther = new System.Windows.Forms.Button();
            this.LblPercent = new System.Windows.Forms.Label();
            this.LPercKnown = new System.Windows.Forms.Label();
            this.LblWSum = new System.Windows.Forms.Label();
            this.LblRSum = new System.Windows.Forms.Label();
            this.LblQryCards = new System.Windows.Forms.Label();
            this.LblTime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LNote = new System.Windows.Forms.Label();
            this.zedGraphControlCurrentDistribution = new ZedGraph.ZedGraphControl();
            this.LOtherDesc = new System.Windows.Forms.Label();
            this.btnOkay = new System.Windows.Forms.Button();
            this.LblTimeFrame = new System.Windows.Forms.Label();
            this.CBShow = new System.Windows.Forms.ComboBox();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.PCStats.SuspendLayout();
            this.TSRightWrong.SuspendLayout();
            this.TSDistribution.SuspendLayout();
            this.TSOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // PCStats
            // 
            this.PCStats.Controls.Add(this.TSRightWrong);
            this.PCStats.Controls.Add(this.TSDistribution);
            this.PCStats.Controls.Add(this.TSOther);
            resources.ApplyResources(this.PCStats, "PCStats");
            this.PCStats.Name = "PCStats";
            this.PCStats.SelectedIndex = 0;
            this.PCStats.SelectedIndexChanged += new System.EventHandler(this.PCStats_SelectedIndexChanged);
            // 
            // TSRightWrong
            // 
            this.TSRightWrong.Controls.Add(this.checkBoxShowTarget);
            this.TSRightWrong.Controls.Add(this.zedGraphControlKnowledge);
            this.TSRightWrong.Controls.Add(this.LKnowledgeDesc);
            this.TSRightWrong.Controls.Add(this.btnPrintKnowledge);
            this.TSRightWrong.Controls.Add(this.LKnowledge);
            resources.ApplyResources(this.TSRightWrong, "TSRightWrong");
            this.TSRightWrong.Name = "TSRightWrong";
            // 
            // checkBoxShowTarget
            // 
            resources.ApplyResources(this.checkBoxShowTarget, "checkBoxShowTarget");
            this.checkBoxShowTarget.Name = "checkBoxShowTarget";
            this.checkBoxShowTarget.UseVisualStyleBackColor = true;
            this.checkBoxShowTarget.CheckedChanged += new System.EventHandler(this.checkBoxShowTarget_CheckedChanged);
            // 
            // zedGraphControlKnowledge
            // 
            resources.ApplyResources(this.zedGraphControlKnowledge, "zedGraphControlKnowledge");
            this.zedGraphControlKnowledge.Name = "zedGraphControlKnowledge";
            this.zedGraphControlKnowledge.ScrollGrace = 0;
            this.zedGraphControlKnowledge.ScrollMaxX = 0;
            this.zedGraphControlKnowledge.ScrollMaxY = 0;
            this.zedGraphControlKnowledge.ScrollMaxY2 = 0;
            this.zedGraphControlKnowledge.ScrollMinX = 0;
            this.zedGraphControlKnowledge.ScrollMinY = 0;
            this.zedGraphControlKnowledge.ScrollMinY2 = 0;
            // 
            // LKnowledgeDesc
            // 
            resources.ApplyResources(this.LKnowledgeDesc, "LKnowledgeDesc");
            this.LKnowledgeDesc.Name = "LKnowledgeDesc";
            // 
            // btnPrintKnowledge
            // 
            resources.ApplyResources(this.btnPrintKnowledge, "btnPrintKnowledge");
            this.btnPrintKnowledge.Name = "btnPrintKnowledge";
            this.btnPrintKnowledge.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // LKnowledge
            // 
            resources.ApplyResources(this.LKnowledge, "LKnowledge");
            this.LKnowledge.Name = "LKnowledge";
            // 
            // TSDistribution
            // 
            this.TSDistribution.Controls.Add(this.zedGraphControlMemoryDistribution);
            this.TSDistribution.Controls.Add(this.btnPrintMemory);
            this.TSDistribution.Controls.Add(this.LMemory);
            resources.ApplyResources(this.TSDistribution, "TSDistribution");
            this.TSDistribution.Name = "TSDistribution";
            // 
            // zedGraphControlMemoryDistribution
            // 
            resources.ApplyResources(this.zedGraphControlMemoryDistribution, "zedGraphControlMemoryDistribution");
            this.zedGraphControlMemoryDistribution.Name = "zedGraphControlMemoryDistribution";
            this.zedGraphControlMemoryDistribution.ScrollGrace = 0;
            this.zedGraphControlMemoryDistribution.ScrollMaxX = 0;
            this.zedGraphControlMemoryDistribution.ScrollMaxY = 0;
            this.zedGraphControlMemoryDistribution.ScrollMaxY2 = 0;
            this.zedGraphControlMemoryDistribution.ScrollMinX = 0;
            this.zedGraphControlMemoryDistribution.ScrollMinY = 0;
            this.zedGraphControlMemoryDistribution.ScrollMinY2 = 0;
            // 
            // btnPrintMemory
            // 
            resources.ApplyResources(this.btnPrintMemory, "btnPrintMemory");
            this.btnPrintMemory.Name = "btnPrintMemory";
            this.btnPrintMemory.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // LMemory
            // 
            resources.ApplyResources(this.LMemory, "LMemory");
            this.LMemory.Name = "LMemory";
            // 
            // TSOther
            // 
            this.TSOther.Controls.Add(this.btnPrintOther);
            this.TSOther.Controls.Add(this.LblPercent);
            this.TSOther.Controls.Add(this.LPercKnown);
            this.TSOther.Controls.Add(this.LblWSum);
            this.TSOther.Controls.Add(this.LblRSum);
            this.TSOther.Controls.Add(this.LblQryCards);
            this.TSOther.Controls.Add(this.LblTime);
            this.TSOther.Controls.Add(this.label5);
            this.TSOther.Controls.Add(this.label4);
            this.TSOther.Controls.Add(this.label3);
            this.TSOther.Controls.Add(this.label2);
            this.TSOther.Controls.Add(this.LNote);
            this.TSOther.Controls.Add(this.zedGraphControlCurrentDistribution);
            this.TSOther.Controls.Add(this.LOtherDesc);
            resources.ApplyResources(this.TSOther, "TSOther");
            this.TSOther.Name = "TSOther";
            // 
            // btnPrintOther
            // 
            resources.ApplyResources(this.btnPrintOther, "btnPrintOther");
            this.btnPrintOther.Name = "btnPrintOther";
            // 
            // LblPercent
            // 
            resources.ApplyResources(this.LblPercent, "LblPercent");
            this.LblPercent.Name = "LblPercent";
            // 
            // LPercKnown
            // 
            resources.ApplyResources(this.LPercKnown, "LPercKnown");
            this.LPercKnown.Name = "LPercKnown";
            // 
            // LblWSum
            // 
            resources.ApplyResources(this.LblWSum, "LblWSum");
            this.LblWSum.Name = "LblWSum";
            // 
            // LblRSum
            // 
            resources.ApplyResources(this.LblRSum, "LblRSum");
            this.LblRSum.Name = "LblRSum";
            // 
            // LblQryCards
            // 
            resources.ApplyResources(this.LblQryCards, "LblQryCards");
            this.LblQryCards.Name = "LblQryCards";
            // 
            // LblTime
            // 
            resources.ApplyResources(this.LblTime, "LblTime");
            this.LblTime.Name = "LblTime";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // LNote
            // 
            resources.ApplyResources(this.LNote, "LNote");
            this.LNote.Name = "LNote";
            // 
            // zedGraphControlCurrentDistribution
            // 
            resources.ApplyResources(this.zedGraphControlCurrentDistribution, "zedGraphControlCurrentDistribution");
            this.zedGraphControlCurrentDistribution.Name = "zedGraphControlCurrentDistribution";
            this.zedGraphControlCurrentDistribution.ScrollGrace = 0;
            this.zedGraphControlCurrentDistribution.ScrollMaxX = 0;
            this.zedGraphControlCurrentDistribution.ScrollMaxY = 0;
            this.zedGraphControlCurrentDistribution.ScrollMaxY2 = 0;
            this.zedGraphControlCurrentDistribution.ScrollMinX = 0;
            this.zedGraphControlCurrentDistribution.ScrollMinY = 0;
            this.zedGraphControlCurrentDistribution.ScrollMinY2 = 0;
            // 
            // LOtherDesc
            // 
            resources.ApplyResources(this.LOtherDesc, "LOtherDesc");
            this.LOtherDesc.Name = "LOtherDesc";
            // 
            // btnOkay
            // 
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOkay, "btnOkay");
            this.btnOkay.Name = "btnOkay";
            // 
            // LblTimeFrame
            // 
            resources.ApplyResources(this.LblTimeFrame, "LblTimeFrame");
            this.LblTimeFrame.Name = "LblTimeFrame";
            // 
            // CBShow
            // 
            this.CBShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBShow.Items.AddRange(new object[] {
            resources.GetString("CBShow.Items"),
            resources.GetString("CBShow.Items1"),
            resources.GetString("CBShow.Items2"),
            resources.GetString("CBShow.Items3"),
            resources.GetString("CBShow.Items4")});
            resources.ApplyResources(this.CBShow, "CBShow");
            this.CBShow.Name = "CBShow";
            this.CBShow.SelectedIndexChanged += new System.EventHandler(this.CBShow_SelectedIndexChange);
            // 
            // TStatsForm
            // 
            this.AcceptButton = this.btnOkay;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnOkay;
            this.Controls.Add(this.CBShow);
            this.Controls.Add(this.LblTimeFrame);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.PCStats);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TStatsForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.TStatsForm_Load);
            this.PCStats.ResumeLayout(false);
            this.TSRightWrong.ResumeLayout(false);
            this.TSDistribution.ResumeLayout(false);
            this.TSOther.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Initialisation & Helpers

        public static void ShowStats()
        {
            //No longer needed:
            //if (MainForm.LearnLogic.Dictionary.Statistics.Statistics.Count == 0)
            //{
            //    MessageBox.Show(Resources.STATISTICS_NO_STATS_MSGBOX_DESC, Resources.STATISTICS_NO_STATS_MSGBOX_CAPT, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            TStatsForm StatsForm = new TStatsForm();
            //TODO print functionality not yet implemented, therefore invisible
            StatsForm.btnPrintKnowledge.Visible = false;
            StatsForm.btnPrintMemory.Visible = false;
            StatsForm.btnPrintOther.Visible = false;
            StatsForm.ShowDialog();
        }

        #endregion

        #region Component Events

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            //TODO case PCStats.ActivePageIndex of
            //0 : TCKnowledge.PrintLandscape;
            //1 : TCDistribution.PrintLandscape;
            //2 : TCCurrent.Print;
        }

        /// <summary>
        /// Draws the statistics.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-28</remarks>
        public void DrawStatistics()
        {
            if (loading)
                return;

            # region set values and start time/date
            int values;
            XDate date = new XDate(DateTime.Now);
            TimeSpan offset = new TimeSpan(1, 0, 0, 0);
            AxisType axisType = AxisType.Text;

            switch ((TimeFrame)CBShow.SelectedIndex)
            {
                case TimeFrame.Today:
                    values = date.DateTime.Hour;
                    //date.DateTime = date.DateTime.AddHours(-date.DateTime.Hour);
                    //date.DateTime = date.DateTime.AddMinutes(-date.DateTime.Minute);
                    //date.DateTime = date.DateTime.AddSeconds(-date.DateTime.Second);
                    date.DateTime = date.DateTime.Date;
                    offset = new TimeSpan(1, 0, 0);
                    axisType = AxisType.Date;
                    break;
                case TimeFrame.Week:
                    values = 7;
                    date.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-7);
                    break;
                case TimeFrame.Month:
                    values = 30;
                    date.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-30);
                    break;
                case TimeFrame.Year:
                    values = 12;
                    date.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(-365);
                    offset = new TimeSpan(31, 0, 0, 0);
                    break;
                case TimeFrame.All:
                default:
                    values = (Dictionary.Statistics.GetNewestStatistic().EndTimestamp -
                        Dictionary.Statistics.GetOldestStatistic().EndTimestamp).Days + 1;
                    date.DateTime = Dictionary.Statistics.GetOldestStatistic().EndTimestamp;
                    break;
            }
            values++;
            # endregion
            # region initialize graphs

            //for performance
            int activeCards = Dictionary.Cards.ActiveCardsCount;

            GraphPane knowledgePane = zedGraphControlKnowledge.GraphPane;
            GraphPane distrubutionPane = zedGraphControlMemoryDistribution.GraphPane;
            GraphPane currentPane = zedGraphControlCurrentDistribution.GraphPane;

            knowledgePane.CurveList.Clear();
            distrubutionPane.CurveList.Clear();
            currentPane.CurveList.Clear();

            knowledgePane.Title.Text = Properties.Resources.STATISTICS_KNOWLEDGE_CAPTION;
            knowledgePane.XAxis.Title.Text = Properties.Resources.STATISTICS_XAXIS;
            knowledgePane.YAxis.Title.Text = Properties.Resources.STATISTICS_YAXIS;
            distrubutionPane.Title.Text = Properties.Resources.STATISTICS_DISTRIBUTION_CAPTION;
            distrubutionPane.XAxis.Title.Text = Properties.Resources.STATISTICS_XAXIS;
            distrubutionPane.YAxis.Title.Text = Properties.Resources.STATISTICS_YAXIS;
            currentPane.Title.Text = Properties.Resources.STATISTICS_OTHER;
            currentPane.Title.FontSpec.Size = 24;
            currentPane.Title.FontSpec.IsBold = true;

            knowledgePane.YAxis.Scale.Max = activeCards + 1;
            knowledgePane.YAxis.Scale.Min = 0;
            distrubutionPane.YAxis.Scale.Max = 10;
            //distrubutionPane.YAxis.Scale.MinorStep = 1;
            distrubutionPane.YAxis.Scale.Min = 0;

            if (((TimeFrame)CBShow.SelectedIndex) == TimeFrame.Today)
            {
                knowledgePane.XAxis.Scale.Min = (new XDate(date.DateTime + offset)).XLDate;
                knowledgePane.XAxis.Scale.Max = (new XDate(DateTime.Now + offset)).XLDate;
                distrubutionPane.XAxis.Scale.Min = (new XDate(date.DateTime + offset)).XLDate;
                distrubutionPane.XAxis.Scale.Max = (new XDate(DateTime.Now + offset)).XLDate;
            }
            else
            {
                knowledgePane.XAxis.Scale.Min = 2;
                knowledgePane.XAxis.Scale.Max = values + 2;
                distrubutionPane.XAxis.Scale.Min = 1;
                distrubutionPane.XAxis.Scale.Max = values + 1;
            }

            knowledgePane.XAxis.Type = axisType;

            distrubutionPane.XAxis.Type = axisType;
            distrubutionPane.BarSettings.Type = BarType.Stack;
            distrubutionPane.Legend.Position = LegendPos.Right;
            distrubutionPane.Legend.Gap = 0;

            currentPane.Legend.IsVisible = false;
            currentPane.Legend.Position = LegendPos.Right;
            currentPane.Legend.Gap = 0;
            currentPane.Legend.FontSpec.Size = 18;

            knowledgePane.Fill = new Fill(Color.WhiteSmoke, Color.Lavender, 0F);
            knowledgePane.Chart.Fill = new Fill(Color.FromArgb(255, 255, 245),
                Color.FromArgb(255, 255, 190), 90F);
            distrubutionPane.Fill = new Fill(Color.WhiteSmoke, Color.Lavender, 0F);
            distrubutionPane.Chart.Fill = new Fill(Color.FromArgb(255, 255, 245),
                Color.FromArgb(255, 255, 190), 90F);

            List<string> xAxisListKnowledge = new List<string>();
            List<string> xAxisListDistribution = new List<string>();

            //Add "filler labels" for the x axis to correspond properly to the values
            date.AddDays(-1);
            AddCurrentPointLabel(ref xAxisListKnowledge, date);
            if (((TimeFrame)CBShow.SelectedIndex) == TimeFrame.All)
            {
                AddCurrentPointLabel(ref xAxisListKnowledge, date);
                AddCurrentPointLabel(ref xAxisListDistribution, date);
            }
            date.AddDays(1);

            PointPairList targetList = new PointPairList();
            targetList.Add(new PointPair(0, activeCards));
            PointPairList knownList = new PointPairList();

            PointPairList[] boxLists = new PointPairList[10];
            for (int k = 0; k < boxLists.Length; k++)
                boxLists[k] = new PointPairList();
            # endregion
            # region get values
            knownList.Add(new PointPair(0, Dictionary.Statistics.GetKnown(date.DateTime)));


            while (date.DateTime <= DateTime.Now + offset)
            {
                targetList.Add(new PointPair(date.XLDate, activeCards));
                int known = Dictionary.Statistics.GetKnown(date.DateTime);
                knownList.Add(new PointPair(date.XLDate, known));

                int box1 = Dictionary.Statistics.GetBoxContent(1, date.DateTime);
                boxLists[0].Add(new PointPair(date.XLDate, box1));
                for (int i = 2; i <= 10; i++)
                    boxLists[i - 1].Add(new PointPair(date.XLDate, Dictionary.Statistics.GetBoxContent(i, date.DateTime)));

                if (known + box1 + 1 > distrubutionPane.YAxis.Scale.Max)
                    distrubutionPane.YAxis.Scale.Max = known + box1 + 1;

                AddCurrentPointLabel(ref xAxisListKnowledge, date);
                AddCurrentPointLabel(ref xAxisListDistribution, date);

                date.DateTime += offset;
            }

            CheckDoubleDistribution(ref boxLists);

            knowledgePane.XAxis.Scale.TextLabels = xAxisListKnowledge.ToArray();
            distrubutionPane.XAxis.Scale.TextLabels = xAxisListDistribution.ToArray();

            targetList.Add(new PointPair(date.XLDate + 1, activeCards));
            knownList.Add(new PointPair(date.XLDate + 1, knownList[knownList.Count - 1].Y));

            # endregion
            # region draw lines
            if (checkBoxShowTarget.Checked)
            {
                LineItem targetLine = knowledgePane.AddCurve(Resources.STATISTICS_KNOWLEDGE_TAB_TARGET_CARDS_STRING, targetList, Color.Red, SymbolType.None);
                targetLine.Line.Width = 2;
                targetLine.Line.IsSmooth = true;
                targetLine.Line.SmoothTension = 1;
            }
            else
            {
                double max = -1;
                foreach (PointPair pp in knownList)
                    if (pp.Y > max)
                        max = pp.Y;
                knowledgePane.YAxis.Scale.Max = max + 1;
            }

            LineItem knownLine = knowledgePane.AddCurve(Resources.STATISTICS_KNOWLEDGE_TAB_KNOWN_CARDS_STRING, knownList, Color.Blue, SymbolType.None);
            knownLine.Line.Width = 2;
            knownLine.Line.Fill = new Fill(Color.WhiteSmoke, Color.LightBlue, Color.DodgerBlue);
            knownLine.Line.IsAntiAlias = true;

            for (int k = 0; k < boxLists.Length; k++)
            {
                CurveItem boxLine = distrubutionPane.AddBar(String.Format(Resources.STATISTICS_KNOWLEDGE_TAB_BOX_STRING, (k + 1).ToString()), boxLists[k], GradientColors[k]);
            }

            for (int k = 1; k < Dictionary.Boxes.Count; k++)
            {
                PieItem item = currentPane.AddPieSlice(Dictionary.Boxes[k].Size, GradientColors[k], 0,
                    String.Format(Resources.STATISTICS_OTHER_TAB_BOX_SIZE_STRING, k, Dictionary.Boxes[k].Size));
                item.LabelDetail.FontSpec.Size = 16;
                item.Fill = new Fill(Color.WhiteSmoke, GradientColors[k], 45F);

                if (Dictionary.Boxes[k].Size <= 0)
                    item.LabelDetail.IsVisible = false;
            }

            PieItem poolItem = currentPane.AddPieSlice(Dictionary.Boxes[0].Size, GradientColors[0], 0,
                String.Format(Resources.STATISTICS_OTHER_TAB_POOL_SIZE_STRING, Dictionary.Boxes[0].Size));
            poolItem.LabelDetail.FontSpec.Size = 16;
            poolItem.Fill = new Fill(Color.WhiteSmoke, GradientColors[0], 45F);

            if (Dictionary.Boxes[0].Size <= 0)
                poolItem.LabelDetail.IsVisible = false;

            # endregion
            zedGraphControlKnowledge.AxisChange();
            zedGraphControlKnowledge.Refresh();
            zedGraphControlMemoryDistribution.AxisChange();
            zedGraphControlMemoryDistribution.Refresh();
            zedGraphControlCurrentDistribution.AxisChange();
            zedGraphControlCurrentDistribution.Refresh();
        }

        private void CheckDoubleDistribution(ref PointPairList[] list)
        {
            List<int> eraseIDs = new List<int>();
            for (int i = 1; i < list[0].Count; i++)
            {
                bool erase = true;
                for (int j = 0; j < list.Length; j++)
                {
                    if (list[j][i].Y != list[j][i - 1].Y)
                    {
                        erase = false;
                        break;
                    }
                }
                if (erase)
                    eraseIDs.Add(i);
            }

            foreach (int id in eraseIDs)
                foreach (PointPairList li in list)
                    li[id].Y = 0;
        }

        private void AddCurrentPointLabel(ref List<string> xAxisList, XDate date)
        {
            switch (((TimeFrame)CBShow.SelectedIndex)) //TODO: format date/time according to regional settings
            {
                case TimeFrame.Week:
                    xAxisList.Add(date.DateTime.ToString("ddd"));
                    break;
                case TimeFrame.Month:
                    xAxisList.Add(date.DateTime.ToString("MMM-dd"));
                    break;
                case TimeFrame.Year:
                    xAxisList.Add(date.DateTime.ToString("yyyy-MMM"));
                    break;
                case TimeFrame.Today:
                case TimeFrame.All:
                default:
                    xAxisList.Add(date.DateTime.ToShortDateString());
                    break;
            }
        }

        /// <summary>
        /// Reads the statistics from the currently loaded dictionary and loads the data 
        /// into the appropriate control tab depending on the selected time frame.
        /// Finally the selected time frame is stored to the registry as this should be the prefered.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-20</remarks>
        private void CBShow_SelectedIndexChange(object sender, System.EventArgs e)
        {
            DrawStatistics();

            int Know = 0, RSum = 0, WSum = 0;
            TimeSpan Total = new TimeSpan();

            LearnStats curStats = Dictionary.Statistics.GetCurrentStats();

            Total = curStats.LearnTime;
            RSum = curStats.NumberOfRights;
            WSum = curStats.NumberOfWrongs;

            // Some general statistics 
            LblTime.Text = String.Format(Resources.STATISTICS_TIMESPAN_FORMAT_STRING, Total.Days, Total.Hours, Total.Minutes, Total.Seconds);
            LblQryCards.Text = (WSum + RSum).ToString();
            LblRSum.Text = RSum.ToString();
            LblWSum.Text = WSum.ToString();

            for (int i = 2; i < Dictionary.Boxes.Count; i++)
                Know += Dictionary.Boxes[i].Size;

            int all_cards = Dictionary.Cards.ActiveCardsCount - Dictionary.Boxes[0].Size;
            if (all_cards > 0)
                LblPercent.Text = ((RSum) * 100.0 / ((RSum + WSum) * 1.0)).ToString(Resources.COMMON_NUMBER_FORMAT) + " " + Resources.ABBREVIATION_PERCENT;
            else
                LblPercent.Text = Resources.STATISTICS_PERCENT;

            Settings.Default.Statistics = CBShow.SelectedIndex;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the PCStats control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-26</remarks>
        private void PCStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            //deactivate timeframe selection combobox on tab with "current values"
            CBShow.Enabled = PCStats.SelectedTab != TSOther;
        }
        #endregion

        private void checkBoxShowTarget_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.STATISTICS_ShowTargetLine = checkBoxShowTarget.Checked;
            Settings.Default.Save();

            DrawStatistics();
        }
    }
}

