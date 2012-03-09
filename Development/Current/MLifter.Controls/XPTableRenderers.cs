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
