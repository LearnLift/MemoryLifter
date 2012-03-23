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
using System.Text;
using XPTable.Renderers;
using System.Windows.Forms;
using XPTable.Events;
using System.Drawing;

namespace MLifter.Controls
{

    /// <summary>
    /// A Textrenderer for the XPTable for supporting RTL-Text.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-17</remarks>
    public class RtlTextRenderer : CellRenderer
    {
        private TextFormatFlags formatFlags;
        /// <summary>
        /// Gets or sets the format flags.
        /// </summary>
        /// <value>The format flags.</value>
        /// <remarks>Documented by Dev05, 2008-07-17</remarks>
        public TextFormatFlags FormatFlags
        {
            get { return formatFlags; }
            set { formatFlags = value; }
        }

        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);

            // don't bother going any further if the Cell is null
            if (e.Cell == null)
                return;

            // make sure we have some Text to draw
            if (e.Cell.Text != null && e.Cell.Text.Length != 0)
            {
                // check whether the cell is enabled
                if (e.Enabled)
                {
                    TextRenderer.DrawText(e.Graphics, e.Cell.Text, base.Font, base.ClientRectangle, base.ForeColor, formatFlags);
                }
                else
                {
                    Color GrayColor = Color.Gray;
                    try
                    {
                        GrayColor = (base.GrayTextBrush as SolidBrush).Color;
                    }
                    catch { }

                    TextRenderer.DrawText(e.Graphics, e.Cell.Text, base.Font, base.ClientRectangle, GrayColor, formatFlags);
                }
            }

            // draw a focus rect around the cell if it is
            // enabled and has focus
            if (e.Focused && e.Enabled)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics,
                                              base.ClientRectangle);
            }
        }
    }
}
