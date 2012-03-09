using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Components
{
    public partial class DockingButton : DockingPanel
    {
        private Control dockingParent;
        private ToolTip tooltip = new ToolTip();

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingButton"/> class.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        public DockingButton()
        {
            InitializeComponent();
            FontChanged += new EventHandler(DockingButton_FontChanged);
            PanelBorderChanged += new EventHandler(DockingButton_PanelBorderChanged);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the text of the control
        /// </summary>
        /// <value></value>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        [Browsable(true), ReadOnly(false), DefaultValue("dockingButton1")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]      //see http://www.mycsharp.de/wbb2/thread.php?postid=3535709
        public override string Text
        {
            get
            {
                return linkLabelText.Text;
            }
            set
            {
                linkLabelText.Text = value;
                UpdateSize();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        /// <value></value>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        [Browsable(true), ReadOnly(false), DefaultValue(typeof(Color), "Blue")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]      //see http://www.mycsharp.de/wbb2/thread.php?postid=3535709
        public override Color ForeColor
        {
            get
            {
                return linkLabelText.LinkColor;
            }
            set
            {
                linkLabelText.LinkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the docking parent on which the button should dock.
        /// </summary>
        /// <value>The docking parent.</value>
        /// <remarks>Documented by Dev05, 2007-11-19</remarks>
        public Control DockingParent
        {
            get { return dockingParent; }
            set
            {
                if (dockingParent != null)
                {
                    dockingParent.Move -= new EventHandler(dockingParent_Move);
                    dockingParent.Resize -= new EventHandler(dockingParent_Resize);
                }

                dockingParent = value;
                UpdateDocking();

                if (dockingParent != null)
                {
                    Parent = dockingParent.Parent;
                    dockingParent.Move += new EventHandler(dockingParent_Move);
                    dockingParent.Resize += new EventHandler(dockingParent_Resize);
                }
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Handles the Resize event of the dockingParent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        private void dockingParent_Resize(object sender, EventArgs e)
        {
            UpdateDocking();
        }

        /// <summary>
        /// Handles the Move event of the dockingParent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        private void dockingParent_Move(object sender, EventArgs e)
        {
            UpdateDocking();
        }

        /// <summary>
        /// Handles the FontChanged event of the DockingButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        private void DockingButton_FontChanged(object sender, EventArgs e)
        {
            UpdateTextPosition();
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabelText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        private void linkLabelText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Resize event of the DockingButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        private void DockingButton_Resize(object sender, EventArgs e)
        {
            UpdateDocking();
            UpdateTextPosition();
        }

        /// <summary>
        /// Handles the PanelBorderChanged event of the DockingButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void DockingButton_PanelBorderChanged(object sender, EventArgs e)
        {
            UpdateTextPosition();
            UpdateSize();
        }

        #endregion

        /// <summary>
        /// Applies the current culture.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-03-27</remarks>
        public void ChangeCulture()
        {
            Text = new ComponentResourceManager(this.GetType()).GetString(linkLabelText.Name + ".Text");      
        }

        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void UpdateSize()
        {
            Size = new Size(linkLabelText.Width + (base.PanelBorder == PanelBorder.BothSideRounded ? 4 : 2) * Size.Height, Size.Height);
        }

        /// <summary>
        /// Updates the text position.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void UpdateTextPosition()
        {
            switch (base.PanelBorder)
            {
                case PanelBorder.BothSideRounded:
                    linkLabelText.Left = Width / 2 - linkLabelText.Width / 2;
                    break;
                case PanelBorder.LeftSideRounded:
                    linkLabelText.Left = (Width - 2 * Height) / 2 + 2 * Height - linkLabelText.Width / 2;
                    break;
                case PanelBorder.RightSideRounded:
                    linkLabelText.Left = 1;
                    break;
                default:
                    throw new ArgumentException("Unknown Enum in Panelborder: " + base.PanelBorder.ToString());
            }
        }

        /// <summary>
        /// Updates the docking.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void UpdateDocking()
        {
            if (dockingParent == null)
                return;

            Top = dockingParent.Top + dockingParent.Height - Height;

            switch (base.PanelBorder)
            {
                case PanelBorder.BothSideRounded:
                    Left = dockingParent.Left + dockingParent.Width / 2 - Width / 2;
                    break;
                case PanelBorder.LeftSideRounded:
                    Left = dockingParent.Width - Width;
                    break;
                case PanelBorder.RightSideRounded:
                    Left = 0;
                    break;
                default:
                    throw new ArgumentException("Unknown Enum in Panelborder: " + base.PanelBorder.ToString());
            }

            BringToFront();
        }

        /// <summary>
        /// Sets the tool tip.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        public void SetToolTip(string text)
        {
            tooltip.SetToolTip(linkLabelText, text);
        }
    }
}
