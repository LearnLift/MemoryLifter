using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MLifter.AudioTools
{
    class LoadingControl : UserControl
    {
        private Label labelMessage;
        public string Message
        {
            set { labelMessage.Text = value; labelMessage.Refresh(); }
            get { return labelMessage.Text; }
        }

        public LoadingControl()
        {
            InitializeComponent();
            Message = "Loading...";

            int radius = 20;
            GraphicsPath path = new GraphicsPath();
            int l = this.ClientRectangle.Left;
            int t = this.ClientRectangle.Top;
            int w = this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;
            int d = radius << 1;
            path.AddArc(l, t, d, d, 180, 90); // topleft 
            path.AddLine(l + radius, t, l + w - radius, t); // top 
            path.AddArc(l + w - d, t, d, d, 270, 90); // topright 
            path.AddLine(l + w, t + radius, l + w, t + h - radius); // right 
            path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright 
            path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom 
            path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft 
            path.AddLine(l, t + h - radius, l, t + radius); // left 
            
            this.Region = new Region(path);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingControl));
            this.labelMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.Color.Transparent;
            this.labelMessage.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.White;
            this.labelMessage.Location = new System.Drawing.Point(35, -4);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(249, 27);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Loading...";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelMessage.Paint += new System.Windows.Forms.PaintEventHandler(this.labelMessage_Paint);
            // 
            // LoadingControl
            // 
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Controls.Add(this.labelMessage);
            this.DoubleBuffered = true;
            this.Name = "LoadingControl";
            this.Size = new System.Drawing.Size(600, 140);
            this.ResumeLayout(false);

        }

        private void labelMessage_Paint(object sender, PaintEventArgs e)
        {
            
        }
    }
}
