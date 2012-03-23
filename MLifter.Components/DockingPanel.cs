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
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Components
{
    /// <summary>
    /// DockingPanel control
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-04-16</remarks>
    public partial class DockingPanel : UserControl
    {
        private PanelBorder panelBorder = PanelBorder.BothSideRounded;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingPanel"/> class.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        public DockingPanel()
        {
            InitializeComponent();
            UpdateRegion();
        }

        public event EventHandler PanelBorderChanged;

        /// <summary>
        /// Gets or sets the panel border.
        /// </summary>
        /// <value>The panel border.</value>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        [Browsable(true), Category("Appearance"), DefaultValue(PanelBorder.BothSideRounded)]
        public PanelBorder PanelBorder
        {
            get
            {
                return panelBorder;
            }
            set
            {
                panelBorder = value;
                UpdateRegion();
            }
        }

        /// <summary>
        /// Handles the Resize event of the DockingPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        private void DockingPanel_Resize(object sender, EventArgs e)
        {
            UpdateRegion();
        }

        /// <summary>
        /// Updates the region.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-16</remarks>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void UpdateRegion()
        {
            GraphicsPath path = new GraphicsPath();

            bool drawLeft = panelBorder == PanelBorder.BothSideRounded || panelBorder == PanelBorder.LeftSideRounded;
            bool drawRight = panelBorder == PanelBorder.BothSideRounded || panelBorder == PanelBorder.RightSideRounded;

            path.AddLine(0, Height, Width, Height);         //draws the bottom line
            if (drawRight)
            {
                path.AddArc(Width - 2 * Height, -3 * Height, 4 * Height, 4 * Height, 90, 45);       //draws the lower part of the right side
                path.AddArc(Width - 4.65F * Height, 0, 4 * Height, 4 * Height, 315, -45);           //draws the upper part of the right side
            }
            else
                path.AddLine(Width, Height, Width, 0);          //draws a line up, from the bottom to the top (right side)

            path.AddLine(drawRight ? Width - 2 * Height : 0, 0, drawLeft ? 2 * Height : 0, 0);      //draws the top line (depending on the left/right borders)
            if (drawLeft)
            {
                path.AddArc(Height * 0.65F, 0, 4 * Height, 4 * Height, 270, -45);
                path.AddArc(-2 * Height, -3 * Height, 4 * Height, 4 * Height, 45, 45);
            }
            else
                path.AddLine(0, 0, 0, Height);                  //draws a line up, from the bottom to the top (left side)

            path.CloseFigure();

            Region = new Region(path);
            OnPanelBorderChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [panel border changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev08, 2009-04-16</remarks>
        private void OnPanelBorderChanged(object sender, EventArgs e)
        {
            if (PanelBorderChanged != null)
                PanelBorderChanged(sender, e);
        }
    }

    /// <summary>
    /// Enum for the Panel Border View.
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-04-16</remarks>
    public enum PanelBorder
    {
        BothSideRounded,
        LeftSideRounded,
        RightSideRounded
    }
}
