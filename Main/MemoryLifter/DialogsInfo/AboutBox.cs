using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MLifter.Properties;

namespace MLifter
{
	/// <summary>
	/// Shows the about box with details of the current ML version.
	/// </summary>
	public class AboutBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.PictureBox ProgramIcon;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.LinkLabel RecommendLink;
		private System.Windows.Forms.Label Version;
		private System.Windows.Forms.Label LblProductName;
		private Label lblWebsite;
		private Label lblEmail;
		private LinkLabel lblEmailLinl;
		private WebBrowser webBrowserCredits;
		private LinkLabel lblWebsiteLink;

		/// <summary>
		/// Initializes a new instance of the <see cref="AboutBox"/> class.
		/// </summary>
		/// <remarks>Dev01, 2012-02-21</remarks>
		public AboutBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//[ML-925] Adapt the controls for localization - select all Text for the link
			RecommendLink.LinkArea = new LinkArea(0, RecommendLink.Text.Length);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			this.OKButton = new System.Windows.Forms.Button();
			this.Panel1 = new System.Windows.Forms.Panel();
			this.webBrowserCredits = new System.Windows.Forms.WebBrowser();
			this.lblEmailLinl = new System.Windows.Forms.LinkLabel();
			this.lblWebsiteLink = new System.Windows.Forms.LinkLabel();
			this.lblEmail = new System.Windows.Forms.Label();
			this.lblWebsite = new System.Windows.Forms.Label();
			this.RecommendLink = new System.Windows.Forms.LinkLabel();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.Version = new System.Windows.Forms.Label();
			this.LblProductName = new System.Windows.Forms.Label();
			this.ProgramIcon = new System.Windows.Forms.PictureBox();
			this.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ProgramIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.OKButton, "OKButton");
			this.OKButton.Name = "OKButton";
			// 
			// Panel1
			// 
			this.Panel1.Controls.Add(this.webBrowserCredits);
			this.Panel1.Controls.Add(this.lblEmailLinl);
			this.Panel1.Controls.Add(this.lblWebsiteLink);
			this.Panel1.Controls.Add(this.lblEmail);
			this.Panel1.Controls.Add(this.lblWebsite);
			this.Panel1.Controls.Add(this.RecommendLink);
			this.Panel1.Controls.Add(this.lblCopyright);
			this.Panel1.Controls.Add(this.Version);
			this.Panel1.Controls.Add(this.LblProductName);
			this.Panel1.Controls.Add(this.ProgramIcon);
			resources.ApplyResources(this.Panel1, "Panel1");
			this.Panel1.Name = "Panel1";
			// 
			// webBrowserCredits
			// 
			this.webBrowserCredits.AllowNavigation = false;
			this.webBrowserCredits.AllowWebBrowserDrop = false;
			this.webBrowserCredits.IsWebBrowserContextMenuEnabled = false;
			resources.ApplyResources(this.webBrowserCredits, "webBrowserCredits");
			this.webBrowserCredits.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserCredits.Name = "webBrowserCredits";
			this.webBrowserCredits.ScriptErrorsSuppressed = true;
			this.webBrowserCredits.ScrollBarsEnabled = false;
			this.webBrowserCredits.WebBrowserShortcutsEnabled = false;
			// 
			// lblEmailLinl
			// 
			resources.ApplyResources(this.lblEmailLinl, "lblEmailLinl");
			this.lblEmailLinl.Name = "lblEmailLinl";
			this.lblEmailLinl.TabStop = true;
			this.lblEmailLinl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EmailLink_Click);
			// 
			// lblWebsiteLink
			// 
			resources.ApplyResources(this.lblWebsiteLink, "lblWebsiteLink");
			this.lblWebsiteLink.Name = "lblWebsiteLink";
			this.lblWebsiteLink.TabStop = true;
			this.lblWebsiteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebsiteLink_Click);
			// 
			// lblEmail
			// 
			resources.ApplyResources(this.lblEmail, "lblEmail");
			this.lblEmail.Name = "lblEmail";
			// 
			// lblWebsite
			// 
			resources.ApplyResources(this.lblWebsite, "lblWebsite");
			this.lblWebsite.Name = "lblWebsite";
			// 
			// RecommendLink
			// 
			resources.ApplyResources(this.RecommendLink, "RecommendLink");
			this.RecommendLink.Name = "RecommendLink";
			this.RecommendLink.TabStop = true;
			this.RecommendLink.UseCompatibleTextRendering = true;
			this.RecommendLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Recommend_Click);
			// 
			// lblCopyright
			// 
			resources.ApplyResources(this.lblCopyright, "lblCopyright");
			this.lblCopyright.Name = "lblCopyright";
			// 
			// Version
			// 
			resources.ApplyResources(this.Version, "Version");
			this.Version.Name = "Version";
			// 
			// LblProductName
			// 
			resources.ApplyResources(this.LblProductName, "LblProductName");
			this.LblProductName.Name = "LblProductName";
			// 
			// ProgramIcon
			// 
			this.ProgramIcon.Image = global::MLifter.Properties.Resources.MLIcon48;
			resources.ApplyResources(this.ProgramIcon, "ProgramIcon");
			this.ProgramIcon.Name = "ProgramIcon";
			this.ProgramIcon.TabStop = false;
			// 
			// AboutBox
			// 
			this.AcceptButton = this.OKButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.OKButton;
			this.Controls.Add(this.Panel1);
			this.Controls.Add(this.OKButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.AboutBox_Load);
			this.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ProgramIcon)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		
		/// <summary>
		/// Opens arrayList browser window with the ML website.
		/// </summary>
		/// <param name="sender">Event Sender, unused.</param>
		/// <param name="e">Event Arguments, unused.</param>
		/// <remarks>Documented by Dev01, 2007-07-18</remarks>
		private void WebsiteLink_Click(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.URL_MLIFTER_HOMEPAGE);
			}
			catch
			{
				MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Opens arrayList mail window for sending an email to ML support.
		/// </summary>
		/// <param name="sender">Event Sender, unused.</param>
		/// <param name="e">Event Arguments, unused.</param>
		/// <remarks>Documented by Dev01, 2007-07-18</remarks>
		private void EmailLink_Click(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.EMAIL_MLIFTER_SUPPORT_MAIL);
			}
			catch
			{
				MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Opens arrayList browser window with the appropriate page to recommend ML to friends.
		/// </summary>
		/// <param name="sender">Event Sender, unused.</param>
		/// <param name="e">Event Arguments, unused.</param>
		/// <remarks>Documented by Dev01, 2007-07-18</remarks>
		private void Recommend_Click(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.URL_MLIFTER_REFERPAGE);
			}
			catch
			{
				MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Loads the About Box. A browser window is filled with fixed HTML content describing version, copyrights etc.
		/// </summary>
		/// <param name="sender">Event Sender, unused.</param>
		/// <param name="e">Event Arguments, unused.</param>
		/// <remarks>Documented by Dev01, 2007-07-18</remarks>
		private void AboutBox_Load(object sender, System.EventArgs e)
		{
			Version.Text = String.Format(Resources.ABOUT_VERSION, Application.ProductVersion);
			lblCopyright.Text = Resources.ABOUT_COPYRIGHT;

			string sBackGroundColor = ColorTranslator.ToHtml(Color.FromKnownColor(KnownColor.Control));
			string sForeGroundColor = ColorTranslator.ToHtml(Color.FromKnownColor(KnownColor.ControlText));
			webBrowserCredits.DocumentText = String.Format(Resources.DisclaimerText, sForeGroundColor, sBackGroundColor, Resources.ABOUT_HTML_DISCLAIMER_CAPTION, Resources.ABOUT_HTML_DISCLAIMER_TEXT); ;
			webBrowserCredits.Visible = true;
		}
	}
}
