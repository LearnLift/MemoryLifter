using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using MLifter.Components;

namespace MLifter.Controls
{
    /// <summary>
    /// Multiple choice user control.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-02-26</remarks>
    public partial class MultipleChoice : UserControl
    {
        private bool ignoreKeyUp = true;

        public event KeyEventHandler ButtonKeyUp;

        #region properties
        private IQueryMultipleChoiceOptions m_Options;
        private MLifter.BusinessLayer.MultipleChoice m_Choices;
        private Image imageChecked;
        [DefaultValue(typeof(Image), null), Category("Appearance-CheckedImage"), Description("The Image for the checked control")]
        public Image ImageChecked
        {
            get { return imageChecked; }
            set { imageChecked = value; }
        }
        private Image backGroundCheckBox;
        [DefaultValue(typeof(Image), null), Category("Appearance-CheckedImage"), Description("The Image for the checked control")]
        public Image BackGroundCheckBox
        {
            get { return backGroundCheckBox; }
            set { backGroundCheckBox = value; }
        }
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        public IQueryMultipleChoiceOptions Options
        {
            get { return m_Options; }
            set { m_Options = value; }
        }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        /// <value>The choices.</value>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        public MLifter.BusinessLayer.MultipleChoice Choices
        {
            get { return m_Choices; }
            set { m_Choices = value; }
        }
        #endregion properties

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleChoice"/> class.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        public MultipleChoice()
        {
            InitializeComponent();
            flowLayoutPanelMultipleChoice.Font = this.Font;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// Shows the specified choices.
        /// </summary>
        /// <param name="choices">The choices.</param>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        public void Show(MLifter.BusinessLayer.MultipleChoice choices)
        {
            flowLayoutPanelMultipleChoice.Font = this.Font;            
            m_Choices = choices;
            flowLayoutPanelMultipleChoice.SuspendLayout();
            PrepareList();
            ResizeButtons();
            flowLayoutPanelMultipleChoice.ResumeLayout();
        }

        /// <summary>
        /// Handles the Load event of the MultipleChoice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        private void MultipleChoice_Load(object sender, EventArgs e)
        {
            flowLayoutPanelMultipleChoice.SuspendLayout();
            PrepareList();
            ResizeButtons();
            flowLayoutPanelMultipleChoice.ResumeLayout();
        }

        /// <summary>
        /// Handles the Resize event of the flowLayoutPanelMultipleChoice control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        private void flowLayoutPanelMultipleChoice_Resize(object sender, EventArgs e)
        {            
            flowLayoutPanelMultipleChoice.SuspendLayout();
            flowLayoutPanelMultipleChoice.AutoScroll = false;
            ResizeButtons();
            flowLayoutPanelMultipleChoice.AutoScroll = true;
            flowLayoutPanelMultipleChoice.ResumeLayout();
        }

        /// <summary>
        /// Prepares the option list.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        private void PrepareList()
        {
                ignoreKeyUp = true;

                if ((m_Choices == null) || (m_Options == null)) return;
                while (flowLayoutPanelMultipleChoice.Controls.Count > 0)
                    flowLayoutPanelMultipleChoice.Controls.RemoveAt(0);

                for (int i = 0; i < m_Choices.Count; i++)
                {
                    MLifterCheckBox checkBox = new MLifterCheckBox();
                    checkBox.Number = flowLayoutPanelMultipleChoice.Controls.Count + 1;
                    checkBox.Text = m_Choices[i].Word;
                    checkBox.Font = flowLayoutPanelMultipleChoice.Font;
                    checkBox.Tag = m_Choices[i];
                    checkBox.Name = "rbtn" + flowLayoutPanelMultipleChoice.Controls.Count + 1;
                    checkBox.MinimumSize = new Size(300, 25);
                    checkBox.Cursor = Cursors.Hand;
                    checkBox.ImageChecked = this.ImageChecked;
                    checkBox.BackGroundCheckBox = this.backGroundCheckBox;
                    checkBox.BackColor = this.BackColor;
                    checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                    checkBox.KeyUp += new KeyEventHandler(checkBox_KeyUp);
                    checkBox.KeyDown += new KeyEventHandler(checkBox_KeyDown);
                    flowLayoutPanelMultipleChoice.Controls.Add(checkBox);
                }

        }

        /// <summary>
        /// Resizes the buttons.
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        private void ResizeButtons()
        {
                if (flowLayoutPanelMultipleChoice.Controls.Count > 0)
                {

                    int height = Convert.ToInt32((this.Height - (this.Padding.Top + this.Padding.Bottom)) / flowLayoutPanelMultipleChoice.Controls.Count);

                    foreach (Control button in flowLayoutPanelMultipleChoice.Controls)
                    {
                        //resize the button height according to the Text length (to enable word wrap)
                        Size buttonsize = button.GetPreferredSize(new Size(this.Width, 0));
                        int lines = Convert.ToInt32(Math.Ceiling(1.0 * buttonsize.Width / (this.Width - 30)));
                        buttonsize.Width = this.Width - 30;
                        buttonsize.Height = buttonsize.Height * lines;
                        button.Size = buttonsize;
                        
                        int padding = Convert.ToInt32((height - button.Height) / 2);
                        button.Margin = new Padding(0, padding, 0, padding);
                    }

                }
        }

        #region CheckBox
        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            MLifterCheckBox checkBox = (MLifterCheckBox)sender;
            ((Choice)checkBox.Tag).Checked = checkBox.Checked;

            if (checkBox.Checked && (!m_Options.AllowMultipleCorrectAnswers.HasValue || !m_Options.AllowMultipleCorrectAnswers.Value))
                foreach (Control button in flowLayoutPanelMultipleChoice.Controls)
                {
                    if (button != checkBox)
                        (button as MLifterCheckBox).Checked = false;
                }
        }
        void checkBox_KeyDown(object sender, KeyEventArgs e)
        {
            ignoreKeyUp = false;
        }

        void checkBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ignoreKeyUp && ButtonKeyUp != null)
                ButtonKeyUp(sender, e); // the control go the KeyDown event as well so it should forward the KeyUp
        }
        #endregion CheckBox

        #region override
        /// <summary>
        /// Gets a value indicating whether the control has input focus.
        /// </summary>
        /// <value></value>
        /// <returns>true if the control has focus; otherwise, false.</returns>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        public override bool Focused
        {
            get
            {
                bool focused = false;
                foreach (Control control in flowLayoutPanelMultipleChoice.Controls)
                    focused = focused || control.Focused;
                focused = focused || flowLayoutPanelMultipleChoice.Focused || base.Focused;
                return focused;
            }
        }
        #endregion override

        #region flowLayoutPanelMultipleChoice
        private void flowLayoutPanelMultipleChoice_Scroll(object sender, ScrollEventArgs e)
        {
            flowLayoutPanelMultipleChoice.Invalidate();
        }
        #endregion flowLayoutPanelMultipleChoice

    }
}
