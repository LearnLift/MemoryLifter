using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Management;
using System.Threading;
using System.Diagnostics;

namespace MLifter.Components
{
	/// <summary>
	/// 03-01-2008 by SDE
	/// </summary>
	public sealed class Registration
	{
		// Reference System.Management  necessary
		private string m_URI;
		private string m_Name;
		private string m_Email;
		private string m_Comment;
		private bool m_Contact;
		private string m_AssemblyVersion;
		private string m_AssemblyName;

		public Registration(string URI, string Name, string Email, string Comment, bool Contact, string AssemblyVersion, string AssemblyName)
		{
			this.m_URI = URI;
			this.m_Name = Name;
			this.m_Email = Email;
			this.m_Comment = Comment;
			this.m_Contact = Contact;
			this.m_AssemblyVersion = AssemblyVersion;
			this.m_AssemblyName = AssemblyName;
		}

		public void Submit()
		{
			Thread t1 = new Thread(new ThreadStart(SubmitRoutine));
			t1.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			t1.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			t1.Start();
		}

		private void SubmitRoutine()
		{
			// parameters: name1=value1&name2=value2
			string Parameters = "name=" + Uri.EscapeUriString(m_Name)
				+ "&email=" + Uri.EscapeUriString(m_Email)
				+ "&comment=" + Uri.EscapeUriString(m_Comment)
				+ "&contact=" + ((m_Contact == true) ? "1" : "0")
				+ "&assemblyversion=" + Uri.EscapeUriString(m_AssemblyVersion)
				+ "&assemblyname=" + Uri.EscapeUriString(m_AssemblyName)
				+ "&os=" + Uri.EscapeUriString(System.Environment.OSVersion.ToString())
				+ "&mac=" + GetMACAddress();

			WebRequest webRequest = WebRequest.Create(m_URI + "?" + DateTime.Now.Ticks.ToString());
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";

			byte[] bytes = Encoding.ASCII.GetBytes(Parameters);

			Stream os = null;

			try
			{
				// send the Post
				webRequest.ContentLength = bytes.Length;   //Count bytes to send
				os = webRequest.GetRequestStream();
				os.Write(bytes, 0, bytes.Length);         //Send it
				webRequest.GetResponse();
			}
			catch (WebException ex)
			{
				System.Diagnostics.Trace.TraceError(ex.Message.ToString());
			}
			finally
			{
				if (os != null)
				{
					os.Close();
				}
			}
		}

		private string GetMACAddress()
		{
			string MACAddress = string.Empty;
			try
			{
				ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
				ManagementObjectCollection moc = mc.GetInstances();
				foreach (ManagementObject mo in moc)
				{
					if (MACAddress == String.Empty) // only return MAC Address from first card
					{
						if ((bool)mo["IPEnabled"] == true && !mo["Description"].ToString().ToLower().Contains("vmware"))
							MACAddress = mo["MacAddress"].ToString();
					}

					mo.Dispose();
				}

				MACAddress = MACAddress.Replace(":", "");
			}
			catch (Exception exp)
			{
				Trace.WriteLine(exp.ToString());
				MACAddress = "ErrorReadingMAC";
			}

			return MACAddress;
		}

	}
}
