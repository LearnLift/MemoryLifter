using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace MLifter.AudioTools.Codecs
{
    public partial class CodecSettings : Form
    {
        Codecs codecs = null;
        bool omitsave = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodecSettings"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        public CodecSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show encoder].
        /// </summary>
        /// <value><c>true</c> if [show encoder]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public bool ShowEncoder
        {
            get { return checkBoxShowEncoder.Checked; }
            set { checkBoxShowEncoder.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show decoder].
        /// </summary>
        /// <value><c>true</c> if [show decoder]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public bool ShowDecoder
        {
            get { return checkBoxShowDecoder.Checked; }
            set { checkBoxShowDecoder.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [minimize windows].
        /// </summary>
        /// <value><c>true</c> if [minimize windows]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public bool MinimizeWindows
        {
            get { return checkBoxMinimizeWindows.Checked; }
            set { checkBoxMinimizeWindows.Checked = value; }
        }

        /// <summary>
        /// Sets a value indicating whether to [enable encode settings].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable encode settings]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public bool EnableEncodeSettings
        {
            set { textBoxEncodingApp.Enabled = textBoxEncodingArgs.Enabled = buttonBrowseEncodingApp.Enabled = checkBoxShowEncoder.Enabled = value; }
        }

        /// <summary>
        /// Sets a value indicating whether to [enable decode settings].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable decode settings]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public bool EnableDecodeSettings
        {
            set { textBoxDecodingApp.Enabled = textBoxDecodingArgs.Enabled = buttonBrowseDecodingApp.Enabled = checkBoxShowDecoder.Enabled = value; }
        }

        /// <summary>
        /// Gets or sets the codecs;
        /// </summary>
        /// <value>The codecs.</value>
        /// <remarks>Documented by Dev02, 2008-04-15</remarks>
        public Codecs Codecs
        {
            get { return codecs; }
            set { codecs = (Codecs)value.Clone(); }
        }

        /// <summary>
        /// Handles the Load event of the CodecSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private void CodecSettings_Load(object sender, EventArgs e)
        {
            //fill codec selection combobox
            comboBoxFormat.Items.AddRange(codecs.ToArray());

            if (comboBoxFormat.Items.Count > 0)
                comboBoxFormat.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxFormat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private void comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            //load new values
            if (comboBoxFormat.SelectedItem != null && comboBoxFormat.SelectedItem is Codec)
            {
                omitsave = true; //omit saving and checking
                Codec selected = (Codec)comboBoxFormat.SelectedItem;
                textBoxEncodingApp.Text = selected.EncodeApp;
                textBoxEncodingArgs.Text = selected.EncodeArgs;
                textBoxDecodingApp.Text = selected.DecodeApp;
                textBoxDecodingArgs.Text = selected.DecodeArgs;
                omitsave = false;

                //now save and check text
                textBox_TextChanged(this, new EventArgs());

                //update groupbox descriptors
                groupBoxEncoding.Text = string.Format("Encoding ({0} => {1})", MLifterAudioTools.Properties.Resources.AUDIO_WAVE_EXTENSION, selected.extension);
                groupBoxDecoding.Text = string.Format("Decoding ({0} => {1})", selected.extension, MLifterAudioTools.Properties.Resources.AUDIO_WAVE_EXTENSION);
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonBrowseEncodingApp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private void buttonBrowseEncodingApp_Click(object sender, EventArgs e)
        {
            BrowseApp(textBoxEncodingApp);
        }

        /// <summary>
        /// Handles the Click event of the buttonBrowseDecodingApp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private void buttonBrowseDecodingApp_Click(object sender, EventArgs e)
        {
            BrowseApp(textBoxDecodingApp);
        }

        /// <summary>
        /// Opens the browser to search an application.
        /// </summary>
        /// <param name="textbox">The textbox (for the desired path).</param>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        private void BrowseApp(TextBox textbox)
        {
            openFileDialogBrowse.InitialDirectory = Application.StartupPath;
            DialogResult result = openFileDialogBrowse.ShowDialog();
            if (result == DialogResult.OK)
            {
                string app = openFileDialogBrowse.FileName;
                //make path relative, when it is within the application path
                if (app.StartsWith(Application.StartupPath, true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    app = app.Remove(0, Application.StartupPath.Length);
                    if (app.StartsWith(@"\"))
                        app = app.Remove(0, 1);
                }
                textbox.Text = app;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the textBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-11</remarks>
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            //save current values
            if (comboBoxFormat.SelectedItem != null && comboBoxFormat.SelectedItem is Codec && !omitsave)
            {
                Codec selected = (Codec)comboBoxFormat.SelectedItem;
                selected.EncodeApp = textBoxEncodingApp.Text;
                selected.EncodeArgs = textBoxEncodingArgs.Text;
                selected.DecodeApp = textBoxDecodingApp.Text;
                selected.DecodeArgs = textBoxDecodingArgs.Text;

                //check values
                errorProvider.Clear();

                if (!selected.CanEncode)
                {
                    string errorstring = "Encoding values are currently not valid: " + selected.EncodeError;
                    errorProvider.SetError(groupBoxEncoding, errorstring);
                }
                if (!selected.CanDecode)
                {
                    string errorstring = "Decoding values are currently not valid: " + selected.DecodeError;
                    errorProvider.SetError(groupBoxDecoding, errorstring);
                }
            }
        }
    }
}