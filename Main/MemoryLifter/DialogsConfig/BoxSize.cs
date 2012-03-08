using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MLifter.Properties;
using MLifter.Components;

namespace MLifter
{
    /// <summary>
    /// Used to set the number of cards in each box.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-19</remarks>
    public class SetupBoxesForm : System.Windows.Forms.Form
    {
        private bool Loading = false;
        private System.Windows.Forms.GroupBox GBBoxes;
        private System.Windows.Forms.Label LInfo;
        private System.Windows.Forms.Button btnOkay;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private System.Windows.Forms.Label[] BoxLabels = new Label[MainForm.LearnLogic.Dictionary.Boxes.Count];
        private System.Windows.Forms.Label[] BoxSize = new Label[MainForm.LearnLogic.Dictionary.Boxes.Count];
        private ColorProgressBar[] BoxIndicator = new ColorProgressBar[MainForm.LearnLogic.Dictionary.Boxes.Count];
        private System.Windows.Forms.NumericUpDown[] BoxMaxSize = new NumericUpDown[MainForm.LearnLogic.Dictionary.Boxes.Count - 2];
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Label BoxLabelSizePool = new Label();
        private System.Windows.Forms.HelpProvider MainHelp;
        private System.Windows.Forms.Label BoxLabelSizePark = new Label();
        private CheckBox checkBoxAutoBoxSize;

        private Color[,] barColors = new Color[,]
            {
                { Color.Chocolate, Color.Orange },
                { Color.Red, Color.OrangeRed },
                { Color.Orange, Color.Coral },
                { Color.Gold, Color.Khaki },
                { Color.Yellow, Color.LightGoldenrodYellow },
                { Color.Chartreuse, Color.Aquamarine },
                { Color.LimeGreen, Color.LightGreen },
                { Color.DarkTurquoise, Color.Turquoise },
                { Color.PaleTurquoise, Color.SkyBlue },
                { Color.DodgerBlue, Color.DeepSkyBlue },
                { Color.MediumBlue, Color.RoyalBlue }
            };

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SetupBoxesForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);

            Loading = true;
            checkBoxAutoBoxSize.Checked = Dictionary.AutoBoxSize;

            // Create labels and meters }
            for (int i = 0; i < Dictionary.Boxes.Count; i++)
            {
                BoxLabels[i] = new Label();
                BoxLabels[i].Parent = GBBoxes;
                BoxLabels[i].Left = 7;
                BoxLabels[i].Top = 19 * i + LInfo.Top + LInfo.Height + 5;
                BoxLabels[i].Height = 14;
                BoxLabels[i].Width = 95;
                BoxLabels[i].AutoSize = true;

                BoxSize[i] = new Label();
                BoxSize[i].Parent = GBBoxes;
                BoxSize[i].TextAlign = ContentAlignment.MiddleRight;
                BoxSize[i].Anchor = AnchorStyles.Right;

                BoxSize[i].Left = GBBoxes.Width - 120;
                BoxSize[i].Top = BoxLabels[i].Top;
                BoxSize[i].Width = 50;
                BoxSize[i].Height = 14;

                if (i > 0 && i < Dictionary.Boxes.Count - 1)
                {
                    BoxMaxSize[i - 1] = new NumericUpDown();
                    BoxMaxSize[i - 1].Parent = GBBoxes;
                    BoxMaxSize[i - 1].Maximum = 64000;
                    BoxMaxSize[i - 1].Minimum = 2;
                    BoxMaxSize[i - 1].Size = new System.Drawing.Size(58, 14);
                    BoxMaxSize[i - 1].Value = BoxMaxSize[i - 1].Minimum;
                    BoxMaxSize[i - 1].ValueChanged += new EventHandler(SEMax_ValueChanged);
                    BoxMaxSize[i - 1].LostFocus += new EventHandler(SEMax_TextChanged);
                    BoxMaxSize[i - 1].Tag = i;
                    BoxMaxSize[i - 1].Top = BoxLabels[i].Top - 3;
                    BoxMaxSize[i - 1].Left = GBBoxes.Width - 65;
                }

                BoxIndicator[i] = new ColorProgressBar();
                BoxIndicator[i].Parent = GBBoxes;
                BoxIndicator[i].Left = BoxLabels[i].Left + BoxLabels[i].Width + 5;
                BoxIndicator[i].Top = BoxLabels[i].Top;
                BoxIndicator[i].Step = 1;
                BoxIndicator[i].Width = GBBoxes.Width - BoxIndicator[i].Left - 120;
                BoxIndicator[i].Height = 14;
                BoxIndicator[i].BarColor = Color.SteelBlue;
                BoxIndicator[i].FillStyle = ColorProgressBar.FillStyles.Solid;
            }
            BoxLabelSizePool.Parent = GBBoxes;
            BoxLabelSizePool.Top = BoxLabels[0].Top;
            BoxLabelSizePool.Left = GBBoxes.Width - 65;
            BoxLabelSizePool.TextAlign = ContentAlignment.MiddleLeft;
            BoxLabelSizePool.Size = new System.Drawing.Size(58, 14);

            BoxLabelSizePark.Parent = GBBoxes;
            BoxLabelSizePark.Top = BoxLabels[10].Top;
            BoxLabelSizePark.Left = GBBoxes.Width - 65;
            BoxLabelSizePark.TextAlign = ContentAlignment.MiddleLeft;
            BoxLabelSizePark.Size = new System.Drawing.Size(58, 14);

            int lastBox = Dictionary.Boxes.Count - 1;
            for (int i = lastBox - 1; i >= 0; i--)
            {
                BoxIndicator[i].DataBindings.Add(new Binding("Left", BoxIndicator[lastBox], "Left"));
                BoxIndicator[i].DataBindings.Add(new Binding("Width", BoxIndicator[lastBox], "Width"));
            }

            BoxLabels[0].Text = Resources.SETUPBOXES_BOXLABEL0_TEXT;
            for (int i = 1; i < Dictionary.Boxes.Count; i++)
                BoxLabels[i].Text = Resources.SETUPBOXES_BOXLABELI_TEXT + " " + i.ToString();

            BoxIndicator[lastBox].Left = BoxLabels[lastBox].Left + BoxLabels[lastBox].Width + 5;
            BoxIndicator[lastBox].Width = GBBoxes.Width - BoxIndicator[lastBox].Left - 120;

            Loading = false;
            UpdateMeters();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupBoxesForm));
            this.GBBoxes = new System.Windows.Forms.GroupBox();
            this.LInfo = new System.Windows.Forms.Label();
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.checkBoxAutoBoxSize = new System.Windows.Forms.CheckBox();
            this.GBBoxes.SuspendLayout();
            this.SuspendLayout();
            // 
            // GBBoxes
            // 
            resources.ApplyResources(this.GBBoxes, "GBBoxes");
            this.GBBoxes.Controls.Add(this.LInfo);
            this.GBBoxes.Name = "GBBoxes";
            this.MainHelp.SetShowHelp(this.GBBoxes, ((bool)(resources.GetObject("GBBoxes.ShowHelp"))));
            this.GBBoxes.TabStop = false;
            // 
            // LInfo
            // 
            resources.ApplyResources(this.LInfo, "LInfo");
            this.LInfo.Name = "LInfo";
            this.MainHelp.SetShowHelp(this.LInfo, ((bool)(resources.GetObject("LInfo.ShowHelp"))));
            // 
            // btnOkay
            // 
            resources.ApplyResources(this.btnOkay, "btnOkay");
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOkay.Name = "btnOkay";
            this.MainHelp.SetShowHelp(this.btnOkay, ((bool)(resources.GetObject("btnOkay.ShowHelp"))));
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnDefault
            // 
            resources.ApplyResources(this.btnDefault, "btnDefault");
            this.btnDefault.Name = "btnDefault";
            this.MainHelp.SetShowHelp(this.btnDefault, ((bool)(resources.GetObject("btnDefault.ShowHelp"))));
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // checkBoxAutoBoxSize
            // 
            resources.ApplyResources(this.checkBoxAutoBoxSize, "checkBoxAutoBoxSize");
            this.checkBoxAutoBoxSize.Name = "checkBoxAutoBoxSize";
            this.MainHelp.SetShowHelp(this.checkBoxAutoBoxSize, ((bool)(resources.GetObject("checkBoxAutoBoxSize.ShowHelp"))));
            this.checkBoxAutoBoxSize.UseVisualStyleBackColor = true;
            this.checkBoxAutoBoxSize.CheckedChanged += new System.EventHandler(this.checkBoxAutoBoxSize_CheckedChanged);
            // 
            // SetupBoxesForm
            // 
            this.AcceptButton = this.btnOkay;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnOkay;
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.GBBoxes);
            this.Controls.Add(this.checkBoxAutoBoxSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupBoxesForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.SetupBoxesForm_Closing);
            this.GBBoxes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void SEMax_ValueChanged(object sender, System.EventArgs e)
        {
            int box_id = (int)(sender as NumericUpDown).Tag;

            Dictionary.Boxes[box_id].MaximalSize = (int)BoxMaxSize[box_id - 1].Value;
            UpdateMeters();
        }

        private void SEMax_TextChanged(object sender, System.EventArgs e)
        {
            int box_id = (int)(sender as NumericUpDown).Tag;

            if (BoxMaxSize[box_id - 1].Value > BoxMaxSize[box_id - 1].Maximum)
                BoxMaxSize[box_id - 1].Value = BoxMaxSize[box_id - 1].Maximum;
            if (BoxMaxSize[box_id - 1].Value < BoxMaxSize[box_id - 1].Minimum)
                BoxMaxSize[box_id - 1].Value = BoxMaxSize[box_id - 1].Minimum;

            SEMax_ValueChanged(sender, e);
        }


        private void btnOkay_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Used to update and visualize the number of cards in each box.
        /// Updates the ColorProgressBar controls.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-19</remarks>
        private void UpdateMeters()
        {
            if (!Loading)
            {
                int size = 0, total = 0;

                // Find out numbers in boxes, pool = total number of cards - cards in boxes }
                for (int i = 1; i < Dictionary.Boxes.Count - 1; i++)
                {
                    BoxIndicator[i].BarColor = barColors[i, (i == Dictionary.CurrentBox ? 1 : 0)];
                    size = Dictionary.Boxes[i].Size;
                    BoxSize[i].Text = size.ToString() + " / ";
                    BoxIndicator[i].Minimum = 0;
                    BoxMaxSize[i - 1].Enabled = !Dictionary.AutoBoxSize;

                    if (i < Dictionary.Boxes.Count)
                    {
                        BoxIndicator[i].Maximum = Dictionary.Boxes[i].MaximalSize;
                        try
                        {
                            BoxMaxSize[i - 1].Value = BoxIndicator[i].Maximum;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            BoxMaxSize[i - 1].Value = BoxMaxSize[i - 1].Minimum;
                        }
                        if (BoxIndicator[i].Maximum < size)
                            BoxIndicator[i].Maximum = size;

                        int min = BoxIndicator[i].Minimum;
                        int max = BoxIndicator[i].Maximum;
                        BoxIndicator[i].Value = size > max ? max : (size < min ? min : size);
                    }
                }
                size = Dictionary.Boxes[Dictionary.Boxes.Count - 1].Size;
                total = Dictionary.Cards.ActiveCardsCount;
                BoxLabelSizePark.Text = total.ToString();
                BoxSize[Dictionary.Boxes.Count - 1].Text = size.ToString() + " / ";
                BoxIndicator[Dictionary.Boxes.Count - 1].Maximum = total;
                BoxIndicator[Dictionary.Boxes.Count - 1].Minimum = 0;
                int minL = BoxIndicator[Dictionary.Boxes.Count - 1].Minimum;
                int maxL = BoxIndicator[Dictionary.Boxes.Count - 1].Maximum;
                BoxIndicator[Dictionary.Boxes.Count - 1].Value = size > maxL ? maxL : (size < minL ? minL : size);

                size = Dictionary.Boxes[0].Size;
                BoxLabelSizePool.Text = total.ToString();
                BoxSize[0].Text = size.ToString() + " / ";
                BoxIndicator[0].Maximum = total;
                BoxIndicator[0].Minimum = 0;
                BoxIndicator[0].Value = size;

                btnDefault.Enabled = !Dictionary.AutoBoxSize;
            }
        }

        /// <summary>
        /// Handles the Closing event of the SetupBoxesForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-04</remarks>
        private void SetupBoxesForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Loading)
                Dictionary.Save();
        }

        /// <summary>
        /// Handles the Click event of the btnDefault control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-04</remarks>
        private void btnDefault_Click(object sender, System.EventArgs e)
        {
            if (!Loading && !Dictionary.AutoBoxSize)
            {
                for (int i = 1; i < Dictionary.Boxes.Count - 1; i++)
                    BoxMaxSize[i - 1].Value = Dictionary.Boxes[i].DefaultSize;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxAutoBoxSize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-04</remarks>
        private void checkBoxAutoBoxSize_CheckedChanged(object sender, EventArgs e)
        {
            if (!Loading)
            {
                Dictionary.AutoBoxSize = checkBoxAutoBoxSize.Checked;
                UpdateMeters();
            }
        }
    }
}

