using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MLifter.Controls.Properties;

namespace MLifter.Controls
{
    public partial class SearchTextbox : UserControl
    {
        Color searchBoxHintColor = SystemColors.ControlDark;

        /// <summary>
        /// Gets or sets the color of the search hint Text.
        /// </summary>
        /// <value>The color of the search box default.</value>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        [Browsable(true), Category("Appearance"), DefaultValue(typeof(Color), "ControlDark"),
        Description("The foreground color for displaying the search hint Text when the control is not focused.")]
        public Color SearchBoxHintColor
        {
            get { return searchBoxHintColor; }
            set
            {
                if (textBoxSearchBox.ForeColor == searchBoxHintColor)
                    textBoxSearchBox.ForeColor = value;

                searchBoxHintColor = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTextbox"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        public SearchTextbox()
        {
            InitializeComponent();
            textBoxSearchBox_Leave(textBoxSearchBox, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxSearchBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        private void textBoxSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSearchBox.ForeColor != searchBoxHintColor)
                OnTextChanged(e);
        }

        /// <summary>
        /// Handles the Enter event of the textBoxSearchBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        private void textBoxSearchBox_Enter(object sender, EventArgs e)
        {
            TextBox searchBox = textBoxSearchBox;
            if (searchBox.ForeColor == searchBoxHintColor)
            {
                searchBox.Text = string.Empty;
                searchBox.ForeColor = SystemColors.WindowText;
            }
        }

        /// <summary>
        /// Handles the Leave event of the textBoxSearchBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        private void textBoxSearchBox_Leave(object sender, EventArgs e)
        {
            TextBox searchBox = textBoxSearchBox;
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.ForeColor = searchBoxHintColor;
                searchBox.Text = Resources.SEARCHHELP;
            }
        }

        /// <summary>
        /// Handles the Click event of the pictureBoxSearchBoxClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        private void pictureBoxSearchBoxClear_Click(object sender, EventArgs e)
        {
            ResetSearch(sender as Control);
        }

        /// <summary>
        /// Resets the search box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        private void ResetSearch(Control sender)
        {
            textBoxSearchBox.Text = string.Empty;
            //focus the search box clear button so that the searchbox looses its focus
            textBoxSearchBox.Focus();
            sender.Focus();
        }

        /// <summary>
        /// Resets this instance and clears the search Text.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        public void Reset()
        {
            ResetSearch(pictureBoxSearchBoxClear);
        }

        /// <summary>
        /// Sets or gets the search Text.
        /// </summary>
        /// <value>The Text in the Searchbox.</value>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        public new string Text
        {
            get
            {
                if (textBoxSearchBox.ForeColor == searchBoxHintColor)
                    return string.Empty;

                return textBoxSearchBox.Text;
            }
            set
            {
                textBoxSearchBox.Focus();
                textBoxSearchBox.Text = value;
            }
        }

        /// <summary>
        /// Occurs when the search Text has changed.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        [Browsable(true)]
        public new event EventHandler TextChanged;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.TextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <remarks>Documented by Dev02, 2008-12-18</remarks>
        public new virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                //change picture
                if (Text == String.Empty)
                    pictureBoxSearchBoxClear.Image = Resources.search;
                else
                    pictureBoxSearchBoxClear.Image = Resources.editundo;

                TextChanged(this, e);
            }       

        }

    }
}
