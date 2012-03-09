using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Controls
{
    public partial class RadioCheckbox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioCheckbox"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-11</remarks>
        public RadioCheckbox()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(this.Text))
                this.Text = this.GetType().Name;

            this.Appearance = RadioCheckBoxAppearance.Checkbox;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadioCheckbox"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-06-11</remarks>
        [Category("Appearance"), DefaultValue(false)]
        public bool Checked
        {
            get
            {
                return Appearance == RadioCheckBoxAppearance.Checkbox ? checkBox.Checked : radioButton.Checked;
            }
            set
            {
                checkBox.Checked = radioButton.Checked = value;
            }
        }

        private RadioCheckBoxAppearance appearance = RadioCheckBoxAppearance.Checkbox;

        /// <summary>
        /// Gets or sets the appearance.
        /// </summary>
        /// <value>The appearance.</value>
        /// <remarks>Documented by Dev02, 2008-06-11</remarks>
        [Category("Appearance"), DefaultValue(typeof(MLifter.Controls.RadioCheckbox.RadioCheckBoxAppearance), "Checkbox")]
        public RadioCheckBoxAppearance Appearance
        {
            get
            {
                return appearance;
            }
            set
            {
                if (value == RadioCheckBoxAppearance.Checkbox)
                {
                    checkBox.BringToFront();
                    checkBox.Visible = true;
                    radioButton.Visible = false;
                    checkBox.Checked = radioButton.Checked;
                }
                else
                {
                    radioButton.BringToFront();
                    checkBox.Visible = false;
                    radioButton.Visible = true;
                    radioButton.Checked = checkBox.Checked;
                }
                appearance = value;
            }
        }

        public enum RadioCheckBoxAppearance
        {
            Checkbox,
            Radiobutton
        }

        /// <summary>
        /// The Text associated with this control.
        /// </summary>
        /// <value>The Text associated with this control.</value>
        /// <returns>The Text associated with this control.</returns>
        /// <remarks>Documented by Dev02, 2008-06-11</remarks>
        [Category("Appearance"), ReadOnly(false), Browsable(true), Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return Appearance == RadioCheckBoxAppearance.Checkbox ? checkBox.Text : radioButton.Text;
            }
            set
            {
                checkBox.Text = radioButton.Text = value;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the radioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-11</remarks>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                //go through all other parent-childs radiobuttons and uncheck them
                if (this.Parent != null)
                {
                    foreach (Control control in this.Parent.Controls)
                    {
                        if (control != this)
                        {
                            if (control is RadioButton)
                                ((RadioButton)control).Checked = false;
                            else if (control is RadioCheckbox && ((RadioCheckbox)control).Appearance == RadioCheckBoxAppearance.Radiobutton)
                                ((RadioCheckbox)control).Checked = false;
                        }
                    }
                }
            }
        }
    }
}
