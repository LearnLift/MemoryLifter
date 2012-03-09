using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Controls
{
    /// <summary>
    /// LinkLabel with Image
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-03-26</remarks>
    public partial class ImageLinkLabel : UserControl
    {
        public event EventHandler LinkLabelClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLinkLabel"/> class.
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-03-26</remarks>
        public ImageLinkLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the link label image.
        /// </summary>
        /// <value>The link label image.</value>
        /// <remarks>Documented by Dev08, 2009-03-26</remarks>
        [Category("Appearance"), Browsable(true), DefaultValue(typeof(Image), "null"), Description("Gets or sets the Image before the LinkLabel")]
        public Image LinkLabelImage
        {
            get
            {
                return pictureBoxLinkLabel.Image;
            }
            set
            {
                pictureBoxLinkLabel.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the Text of the LinkLabel
        /// </summary>
        /// <value>Text of the LinkLabel</value>
        /// <returns>
        /// The text associated with this control.
        /// </returns>
        /// <remarks>Documented by Dev08, 2009-03-26</remarks>
        [Category("Appearance"), Browsable(true), Description("Gets or sets the text of the LinkLabel"), Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return linkLabel.Text;
            }
            set
            {
                linkLabel.Text = value;
            }
        }

        private void pictureBoxLinkLabel_Click(object sender, EventArgs e)
        {
            OnLinkLabelClicked(sender, e);
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnLinkLabelClicked(sender, e);
        }

        private void OnLinkLabelClicked(object sender, EventArgs e)
        {
            if (LinkLabelClicked != null)
                LinkLabelClicked(sender, e);
        }
    }
}
