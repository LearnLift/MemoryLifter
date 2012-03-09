using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Controls.LearningModulesPage
{
    public partial class LearningModulePreviewControl : UserControl
    {
        /// <summary>
        /// Gets or sets the preview image.
        /// </summary>
        /// <value>The preview image.</value>
        /// <remarks>Documented by Dev05, 2008-12-05</remarks>
        public Image PreviewImage { get { return pictureBoxScreenShot.Image; } set { pictureBoxScreenShot.Image = value; } }
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The Description.</value>
        /// <remarks>Documented by Dev05, 2008-12-05</remarks>
        public string Description { get { return richTextBoxDescription.Text; } set { richTextBoxDescription.Text = value; } }

        public LearningModulePreviewControl()
        {
            InitializeComponent();
        }
    }
}
