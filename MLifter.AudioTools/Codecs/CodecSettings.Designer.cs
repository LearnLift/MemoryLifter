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
namespace MLifter.AudioTools.Codecs
{
    partial class CodecSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodecSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.forumLink1 = new MLifterAudioTools.Codecs.ForumLink();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxEncoding = new System.Windows.Forms.GroupBox();
            this.buttonBrowseEncodingApp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxEncodingArgs = new System.Windows.Forms.TextBox();
            this.textBoxEncodingApp = new System.Windows.Forms.TextBox();
            this.groupBoxDecoding = new System.Windows.Forms.GroupBox();
            this.buttonBrowseDecodingApp = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDecodingArgs = new System.Windows.Forms.TextBox();
            this.textBoxDecodingApp = new System.Windows.Forms.TextBox();
            this.comboBoxFormat = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.checkBoxShowEncoder = new System.Windows.Forms.CheckBox();
            this.checkBoxShowDecoder = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.checkBoxMinimizeWindows = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBoxEncoding.SuspendLayout();
            this.groupBoxDecoding.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.forumLink1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.groupBoxEncoding);
            this.groupBox1.Controls.Add(this.groupBoxDecoding);
            this.groupBox1.Controls.Add(this.comboBoxFormat);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(493, 441);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Codec settings";
            // 
            // forumLink1
            // 
            this.forumLink1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.forumLink1.BackColor = System.Drawing.Color.White;
            this.forumLink1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.forumLink1.Location = new System.Drawing.Point(6, 404);
            this.forumLink1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.forumLink1.Name = "forumLink1";
            this.forumLink1.Size = new System.Drawing.Size(481, 30);
            this.forumLink1.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(3, 327);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(481, 73);
            this.label6.TabIndex = 4;
            this.label6.Text = "For the command line arguments, you have to include the following placeholders:\r\n" +
                "\r\n{0} - input file\r\n{1} - output file";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "Configure file format:";
            // 
            // groupBoxEncoding
            // 
            this.groupBoxEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEncoding.Controls.Add(this.buttonBrowseEncodingApp);
            this.groupBoxEncoding.Controls.Add(this.label2);
            this.groupBoxEncoding.Controls.Add(this.label1);
            this.groupBoxEncoding.Controls.Add(this.textBoxEncodingArgs);
            this.groupBoxEncoding.Controls.Add(this.textBoxEncodingApp);
            this.groupBoxEncoding.Location = new System.Drawing.Point(6, 87);
            this.groupBoxEncoding.Name = "groupBoxEncoding";
            this.groupBoxEncoding.Size = new System.Drawing.Size(457, 98);
            this.groupBoxEncoding.TabIndex = 2;
            this.groupBoxEncoding.TabStop = false;
            this.groupBoxEncoding.Text = "Encoding";
            // 
            // buttonBrowseEncodingApp
            // 
            this.buttonBrowseEncodingApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseEncodingApp.Image = global::MLifterAudioTools.Properties.Resources.document_open;
            this.buttonBrowseEncodingApp.Location = new System.Drawing.Point(400, 19);
            this.buttonBrowseEncodingApp.Name = "buttonBrowseEncodingApp";
            this.buttonBrowseEncodingApp.Size = new System.Drawing.Size(51, 28);
            this.buttonBrowseEncodingApp.TabIndex = 1;
            this.buttonBrowseEncodingApp.UseVisualStyleBackColor = true;
            this.buttonBrowseEncodingApp.Click += new System.EventHandler(this.buttonBrowseEncodingApp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Arguments";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Application";
            // 
            // textBoxEncodingArgs
            // 
            this.textBoxEncodingArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEncodingArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxEncodingArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxEncodingArgs.Location = new System.Drawing.Point(78, 64);
            this.textBoxEncodingArgs.Name = "textBoxEncodingArgs";
            this.textBoxEncodingArgs.Size = new System.Drawing.Size(373, 23);
            this.textBoxEncodingArgs.TabIndex = 2;
            this.textBoxEncodingArgs.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxEncodingApp
            // 
            this.textBoxEncodingApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEncodingApp.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxEncodingApp.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxEncodingApp.Location = new System.Drawing.Point(78, 22);
            this.textBoxEncodingApp.Name = "textBoxEncodingApp";
            this.textBoxEncodingApp.Size = new System.Drawing.Size(316, 23);
            this.textBoxEncodingApp.TabIndex = 0;
            this.textBoxEncodingApp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // groupBoxDecoding
            // 
            this.groupBoxDecoding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDecoding.Controls.Add(this.buttonBrowseDecodingApp);
            this.groupBoxDecoding.Controls.Add(this.label4);
            this.groupBoxDecoding.Controls.Add(this.label3);
            this.groupBoxDecoding.Controls.Add(this.textBoxDecodingArgs);
            this.groupBoxDecoding.Controls.Add(this.textBoxDecodingApp);
            this.groupBoxDecoding.Location = new System.Drawing.Point(6, 218);
            this.groupBoxDecoding.Name = "groupBoxDecoding";
            this.groupBoxDecoding.Size = new System.Drawing.Size(457, 100);
            this.groupBoxDecoding.TabIndex = 1;
            this.groupBoxDecoding.TabStop = false;
            this.groupBoxDecoding.Text = "Decoding";
            // 
            // buttonBrowseDecodingApp
            // 
            this.buttonBrowseDecodingApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseDecodingApp.Image = global::MLifterAudioTools.Properties.Resources.document_open;
            this.buttonBrowseDecodingApp.Location = new System.Drawing.Point(400, 19);
            this.buttonBrowseDecodingApp.Name = "buttonBrowseDecodingApp";
            this.buttonBrowseDecodingApp.Size = new System.Drawing.Size(51, 28);
            this.buttonBrowseDecodingApp.TabIndex = 1;
            this.buttonBrowseDecodingApp.UseVisualStyleBackColor = true;
            this.buttonBrowseDecodingApp.Click += new System.EventHandler(this.buttonBrowseDecodingApp_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Arguments";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Application";
            // 
            // textBoxDecodingArgs
            // 
            this.textBoxDecodingArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDecodingArgs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxDecodingArgs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxDecodingArgs.Location = new System.Drawing.Point(78, 67);
            this.textBoxDecodingArgs.Name = "textBoxDecodingArgs";
            this.textBoxDecodingArgs.Size = new System.Drawing.Size(373, 23);
            this.textBoxDecodingArgs.TabIndex = 2;
            this.textBoxDecodingArgs.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxDecodingApp
            // 
            this.textBoxDecodingApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDecodingApp.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxDecodingApp.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxDecodingApp.Location = new System.Drawing.Point(78, 22);
            this.textBoxDecodingApp.Name = "textBoxDecodingApp";
            this.textBoxDecodingApp.Size = new System.Drawing.Size(316, 23);
            this.textBoxDecodingApp.TabIndex = 0;
            this.textBoxDecodingApp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // comboBoxFormat
            // 
            this.comboBoxFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFormat.FormattingEnabled = true;
            this.comboBoxFormat.Location = new System.Drawing.Point(6, 47);
            this.comboBoxFormat.Name = "comboBoxFormat";
            this.comboBoxFormat.Size = new System.Drawing.Size(457, 24);
            this.comboBoxFormat.TabIndex = 0;
            this.comboBoxFormat.SelectedIndexChanged += new System.EventHandler(this.comboBoxFormat_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(339, 492);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 28);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(425, 492);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 28);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // openFileDialogBrowse
            // 
            this.openFileDialogBrowse.FileName = "Application";
            this.openFileDialogBrowse.Filter = "Application (*.exe)|*.exe";
            this.openFileDialogBrowse.RestoreDirectory = true;
            // 
            // checkBoxShowEncoder
            // 
            this.checkBoxShowEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxShowEncoder.AutoSize = true;
            this.checkBoxShowEncoder.Location = new System.Drawing.Point(12, 452);
            this.checkBoxShowEncoder.Name = "checkBoxShowEncoder";
            this.checkBoxShowEncoder.Size = new System.Drawing.Size(210, 20);
            this.checkBoxShowEncoder.TabIndex = 0;
            this.checkBoxShowEncoder.Text = "Show encoder program window";
            this.checkBoxShowEncoder.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowDecoder
            // 
            this.checkBoxShowDecoder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxShowDecoder.AutoSize = true;
            this.checkBoxShowDecoder.Location = new System.Drawing.Point(12, 478);
            this.checkBoxShowDecoder.Name = "checkBoxShowDecoder";
            this.checkBoxShowDecoder.Size = new System.Drawing.Size(210, 20);
            this.checkBoxShowDecoder.TabIndex = 1;
            this.checkBoxShowDecoder.Text = "Show decoder program window";
            this.checkBoxShowDecoder.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // checkBoxMinimizeWindows
            // 
            this.checkBoxMinimizeWindows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxMinimizeWindows.AutoSize = true;
            this.checkBoxMinimizeWindows.Location = new System.Drawing.Point(12, 504);
            this.checkBoxMinimizeWindows.Name = "checkBoxMinimizeWindows";
            this.checkBoxMinimizeWindows.Size = new System.Drawing.Size(184, 20);
            this.checkBoxMinimizeWindows.TabIndex = 4;
            this.checkBoxMinimizeWindows.Text = "Minimize program windows";
            this.checkBoxMinimizeWindows.UseVisualStyleBackColor = true;
            // 
            // CodecSettings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(513, 532);
            this.Controls.Add(this.checkBoxMinimizeWindows);
            this.Controls.Add(this.checkBoxShowDecoder);
            this.Controls.Add(this.checkBoxShowEncoder);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(521, 501);
            this.Name = "CodecSettings";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Codec Settings";
            this.Load += new System.EventHandler(this.CodecSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxEncoding.ResumeLayout(false);
            this.groupBoxEncoding.PerformLayout();
            this.groupBoxDecoding.ResumeLayout(false);
            this.groupBoxDecoding.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxFormat;
        private System.Windows.Forms.GroupBox groupBoxEncoding;
        private System.Windows.Forms.TextBox textBoxEncodingApp;
        private System.Windows.Forms.GroupBox groupBoxDecoding;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxEncodingArgs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDecodingArgs;
        private System.Windows.Forms.TextBox textBoxDecodingApp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBrowseEncodingApp;
        private System.Windows.Forms.Button buttonBrowseDecodingApp;
        private System.Windows.Forms.OpenFileDialog openFileDialogBrowse;
        private System.Windows.Forms.CheckBox checkBoxShowEncoder;
        private System.Windows.Forms.CheckBox checkBoxShowDecoder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox checkBoxMinimizeWindows;
        private MLifterAudioTools.Codecs.ForumLink forumLink1;
    }
}
