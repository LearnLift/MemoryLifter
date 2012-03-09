using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.Components.Properties;

namespace MLifter.Components
{
    public partial class CheckButton : CheckBox
    {
        private Image backGroundCheckBox;
        [DefaultValue(typeof(Image), null), Category("Appearance-CheckedImage"), Description("The Background for the checked control")]
        public Image BackGroundCheckBox
        {
            get { return backGroundCheckBox; }
            set
            {
                backGroundCheckBox = value;
            }
        }

        private Image imageChecked;
        [DefaultValue(typeof(Image), null), Category("Appearance-CheckedImage"), Description("The Checkmark for the checked control")]
        public Image ImageChecked
        {
            get { return imageChecked; }
            set { imageChecked = value; }
        }
        /// <summary>
        /// Gets or sets the color of the cross.
        /// </summary>
        /// <value>The color of the cross.</value>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        [Category("Appearance"), Description("The Color of the cross.")]
        public Color CrossColor
        {
            get { return crossColor; }
            set
            {
                crossColor = value;
                crossBrush = new SolidBrush(value);
            }
        }

        /// <summary>
        /// Gets or sets the cross margin.
        /// </summary>
        /// <value>The cross margin.</value>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        [Category("Appearance"), Description("Margin of the cross inside the button"), DefaultValue(5)]
        public int CrossMargin { get; set; }

        private Color crossColor = Color.Red;
        private Brush crossBrush = Brushes.Red;

        public CheckButton()
        {
           Appearance = Appearance.Button;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //clear background
            Graphics g = pevent.Graphics;
            g.Clear(this.BackColor);
            int size = this.Width > this.Height ? this.Height : this.Width;
            int pointX = (int)((this.Width - size) / 2);
            int pointY = (int)((this.Height - size) / 2);
            //if no background image is set, draw button
            if (backGroundCheckBox == null)
                base.OnPaint(pevent);
            else
            {           
                //Draw parent background
                base.InvokePaintBackground(this,
                     new PaintEventArgs(g, base.ClientRectangle));

                Rectangle dest = new Rectangle(new Point(pointX, pointY), new Size(size, size));
                g.DrawImage(backGroundCheckBox, dest);
                //Draw font
                DrawForegroundFromButton(pevent);
            }
            if (Checked)
            {
                Rectangle dest = new Rectangle(new Point(pointX, pointY), new Size(size, size));
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillRectangle(Brushes.Transparent, dest);
                if (ImageChecked != null)
                    g.DrawImage(ImageChecked, dest);
                else
                    g.DrawImage(Resources.CheckButtonCross3, dest);
            }

        }
        class TransparentControl : Control
        {
            protected override void OnPaintBackground(PaintEventArgs pevent) { }
            protected override void OnPaint(PaintEventArgs e) { }
        }
        private Button imageButton;
        private void DrawForegroundFromButton(PaintEventArgs pevent)
        {
            if (imageButton == null)
            {
                imageButton = new Button();
                imageButton.Parent = new TransparentControl();
                imageButton.BackColor = Color.Transparent;
                imageButton.FlatAppearance.BorderSize = 0;
                imageButton.FlatStyle = FlatStyle.Flat;
            }
            if (backGroundCheckBox != null)
            {
                if (Checked)
                {
                    imageButton.Text = string.Empty;
                }
                else
                {
                    imageButton.Text = Text.Replace(".", string.Empty);
                    imageButton.Text = imageButton.Text.Replace("&", string.Empty);
                }
            }
            else
            {
                imageButton.Text = Text;
            }
            imageButton.ForeColor = ForeColor;
            imageButton.Font = Font;
            imageButton.RightToLeft = RightToLeft;
            imageButton.Image = Image;
            imageButton.ImageAlign = ImageAlign;
            imageButton.ImageIndex = ImageIndex;
            imageButton.ImageKey = ImageKey;
            imageButton.ImageList = ImageList;
            imageButton.Padding = Padding;
            imageButton.Size = Size;
            imageButton.TextAlign = TextAlign;
            imageButton.TextImageRelation = TextImageRelation;
            imageButton.UseCompatibleTextRendering = UseCompatibleTextRendering;
            imageButton.UseMnemonic = UseMnemonic;
            InvokePaint(imageButton, pevent);
        }
    }
}
