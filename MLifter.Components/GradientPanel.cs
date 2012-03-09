using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MLifter.Components
{
    public class GradientPanel : Panel
    {
        private bool layoutSuspended = false;
        public bool LayoutSuspended
        {
            get { return layoutSuspended; }
            set
            {
                if (value == layoutSuspended)
                    return;

                layoutSuspended = value;
                if (!layoutSuspended)
                    Invalidate();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientPanel"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        public GradientPanel()
            : base()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            Resize += new EventHandler(GradientPanel_Resize);
        }

        /// <summary>
        /// Handles the Resize event of the GradientPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-15</remarks>
        private void GradientPanel_Resize(object sender, EventArgs e)
        {
            UpdateValues();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Gradient == null || !Gradient.IsValidGradient || layoutSuspended || ComponentsHelper.IsResizing)
                return;

            e.Graphics.FillRectangle(brush, e.ClipRectangle);
        }

        private PanelGradient gradient = null;
        /// <summary>
        /// Gets or sets the gradient.
        /// </summary>
        /// <value>The gradient.</value>
        /// <remarks>Documented by Dev02, 2009-04-09</remarks>
        public PanelGradient Gradient
        {
            get { return gradient; }
            set
            {
                gradient = value;
                UpdateValues();
                Invalidate();
            }
        }

        LinearGradientBrush brush;
        private void UpdateValues()
        {
            if (Gradient == null)
                return;

            if (Gradient.Colors.Count > 2)
            {
                ColorBlend blend = new ColorBlend();
                blend.Colors = Gradient.Colors.ToArray();
                blend.Positions = Gradient.Positions.ToArray();

                brush = new LinearGradientBrush(this.DisplayRectangle, this.BackColor, this.BackColor, Gradient.Direction);
                brush.InterpolationColors = blend;
            }
            else
                brush = new LinearGradientBrush(this.DisplayRectangle, Gradient.Colors[0], Gradient.Colors[1], Gradient.Direction);
        }
    }
}
