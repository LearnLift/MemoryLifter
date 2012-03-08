using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace MLifterErrorHandler
{
    public enum PreviewType
    {
        HTML,
        Text,
        Image
    }

    public partial class PreviewForm : Form
    {
        private PreviewType Type { get; set; }
        private Stream Data { get; set; }

        private string tempFile = string.Empty;

        public PreviewForm(PreviewType type, Stream data)
        {
            InitializeComponent();

            Type = type;
            Data = data;

            switch (type)
            {
                case PreviewType.HTML:
                    webBrowserPreview.Visible = true;
                    pictureBoxPreview.Visible = false;
                    richTextBoxPreview.Visible = false;

                    tempFile = Path.Combine(Path.GetTempPath(), "ml_error_report_temp.xml");
                    byte[] buffer = new byte[data.Length];
                    data.Seek(0, SeekOrigin.Begin);
                    data.Read(buffer, 0, buffer.Length);
                    File.WriteAllBytes(tempFile, buffer);

                    webBrowserPreview.Url = new Uri(tempFile);
                    break;
                case PreviewType.Image:
                    webBrowserPreview.Visible = false;
                    pictureBoxPreview.Visible = true;
                    richTextBoxPreview.Visible = false;

                    pictureBoxPreview.Image = Image.FromStream(data);
                    break;
                case PreviewType.Text:
                    webBrowserPreview.Visible = false;
                    pictureBoxPreview.Visible = false;
                    richTextBoxPreview.Visible = true;

                    data.Seek(0, SeekOrigin.Begin);
                    richTextBoxPreview.Text = (new StreamReader(data)).ReadToEnd();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void PreviewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tempFile))
                File.Delete(tempFile);
        }
    }
}
