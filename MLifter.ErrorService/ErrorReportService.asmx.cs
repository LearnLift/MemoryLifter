using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Text;

namespace MLifterWebService
{
	/// <summary>
	/// Summary description for Service1
	/// </summary>
	[WebService(Namespace = "http://www.memorylifter.com/ErrorReportService/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class ErrorReportService : System.Web.Services.WebService
	{
		private static string LogFile;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorReportService"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		public ErrorReportService()
		{
			if (String.IsNullOrEmpty(LogFile))
				LogFile = Server.MapPath(ConfigurationManager.AppSettings["LogPath"]);
		}
		/// <summary>
		/// Logs the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		public static void Log(string message)
		{
			string line = Environment.NewLine + DateTime.Now.ToLongTimeString() + ": ";
			File.AppendAllText(LogFile, line + message);
		}

		/// <summary>
		/// Starts a new Error Report Transfer.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="filesize">The filesize.</param>
		/// <param name="chunksize">The chunksize.</param>
		/// <returns>True, if successful, else false.</returns>
		/// <remarks>Documented by Dev02, 2007-11-19</remarks>
		[WebMethod(Description = "Starts a new Error Report Transfer", EnableSession = true)]
		public bool TransferStart(string filename, int filesize, int chunksize, string sender, string senderMessage, string stackTrace)
		{
			int chunkLimit = Convert.ToInt32(ConfigurationManager.AppSettings["ChunkSizeLimit"]) * 1024;
			int sizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSizeLimit"]) * 1024;
			if (filesize < sizeLimit && chunksize < chunkLimit && !string.IsNullOrEmpty(filename) && filesize > 0 && chunksize > 0)
			{
				ErrorReport report = new ErrorReport();
				report.FileName = Path.GetFileName(filename);
				report.FileSize = filesize;
				report.ChunkSize = chunksize;
				report.PartCount = Convert.ToInt32(Math.Ceiling(filesize * 1.0 / chunksize));
				report.Sender = sender;

				report.Message = senderMessage;
				report.StackTrace = stackTrace;

				Session[filename] = report;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Transfers a Data Chunk to the server.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="content">The content.</param>
		/// <param name="partcount">The partcount.</param>
		/// <returns>True, if successful, else false.</returns>
		/// <remarks>Documented by Dev02, 2007-11-19</remarks>
		[WebMethod(Description = "Transfers a Data Chunk to the server", EnableSession = true)]
		public bool TransferChunk(string filename, byte[] content, int partcount)
		{
			ErrorReport report;
			if (Session[filename] == null || (report = Session[filename] as ErrorReport) == null)
				return false;
			//check session
			if (filename == new FileInfo(report.FileName).Name && partcount == report.PartCount && content.Length <= report.ChunkSize && report.DataChunks.Count < report.PartCount)
			{
				report.DataChunks.Add(content);
				return true;
			}
			else
				return false;

		}

		/// <summary>
		/// Finishes an Error Report Transfer.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="partcount">The partcount.</param>
		/// <returns>True, if successful, else false.</returns>
		/// <remarks>Documented by Dev02, 2007-11-19</remarks>
		[WebMethod(Description = "Finishes an Error Report Transfer", EnableSession = true)]
		public bool TransferFinish(string filename, int partcount)
		{
			ErrorReport report;
			if (Session[filename] == null || (report = Session[filename] as ErrorReport) == null)
				return false;
			//check session
			if (filename == new FileInfo(report.FileName).Name && partcount == report.PartCount && partcount == report.DataChunks.Count)
			{
				try
				{
					SendEmail(report);
					return true;
				}
				catch
				{
					return false;
				}
				finally
				{
					Session.Clear();
				}
			}
			else
				return false;
		}

		/// <summary>
		/// Internally used error container object.
		/// </summary>
		class ErrorReport
		{
			/// <summary>
			/// The file name
			/// </summary>
			public string FileName { get; set; }
			/// <summary>
			/// The file size
			/// </summary>
			public int FileSize { get; set; }
			/// <summary>
			/// The chunk size
			/// </summary>
			public int ChunkSize { get; set; }
			/// <summary>
			/// The number of parts/chunks
			/// </summary>
			public int PartCount { get; set; }
			/// <summary>
			/// The data chunks
			/// </summary>
			public List<byte[]> DataChunks { get; set; }
			/// <summary>
			/// The sender email address
			/// </summary>
			public string Sender { get; set; }
			/// <summary>
			/// The senders message
			/// </summary>
			public string Message { get; set; }
			/// <summary>
			/// The stack trace
			/// </summary>
			public string StackTrace { get; set; }

			/// <summary>
			/// Constructor
			/// </summary>
			public ErrorReport()
			{
				DataChunks = new List<byte[]>();
			}
		}


		/// <summary>
		/// Class for grabbing the error report information from the filename of the zip file.
		/// </summary>
		/// <remarks>Documented by Dev09, 2009-07-16</remarks>
		class ReportInformation
		{
			public string Date = DateTime.Today.ToShortDateString();
			public string Time = DateTime.Now.ToShortTimeString();
			public string MLVersion = "-";

			public ReportInformation(string filename)
			{
				Match m = Regex.Match(filename, @"^MemoryLifter_ErrorReport_(.+)_(.+)_(.+)\.zip$");

				if (m.Success)
				{
					string tdate = m.Groups[1].ToString();
					string ttime = m.Groups[2].ToString();
					string tversion = m.Groups[3].ToString();
					Match d = Regex.Match(tdate, @"(\d{4})-(\d{2})-(\d{2})");

					if (d.Success)
					{
						string year = d.Groups[1].ToString();
						string month = d.Groups[2].ToString();
						string day = d.Groups[3].ToString();

						Date = day + "." + month + "." + year;
					}
					Time = ttime.Replace('-', ':');
					MLVersion = tversion;
				}
			}
		}

		/// <summary>
		/// Sends the error report email using the data stored in Session.
		/// </summary>
		/// <param name="errorReport">The error report.</param>
		/// <remarks>Documented by Dev09, 2009-07-16</remarks>
		private void SendEmail(ErrorReport errorReport)
		{
			try
			{
				MailMessage mail = new MailMessage(ConfigurationManager.AppSettings["ErrorReportDefaultSender"].ToString(),
					ConfigurationManager.AppSettings["ErrorReportReceiver"].ToString());

				ReportInformation report = new ReportInformation(errorReport.FileName);
				mail.Subject = string.Format("MemoryLifter Version {0} Error Report", report.MLVersion);

				// message body containing the user's message and stack trace
				string separator = ConfigurationManager.AppSettings["EmailSeparator"].ToString();
				string reportDate = String.Format("\t\t<p>Report from {0} at {1}</p>\r\n\r\n", report.Date, report.Time);
				string usermail = String.Format("\t\t<p>User E-Mail:<br />\r\n{0}</p>\r\n\r\n", errorReport.Sender);
				string message = String.Format("\t\t<p>{0}<br />\r\nUser Message:<br />\r\n{1}<br />\r\n{2}</p>\r\n\r\n", separator, separator, errorReport.Message);
				string trace = String.Format("\t\t<p>{0}<br />\r\nStack Trace:<br />\r\n{1}<br />\r\n{2}</p>\r\n", separator, separator, errorReport.StackTrace.Replace(Environment.NewLine, "<br />\r\n"));
				string body = reportDate + usermail + message + trace;

				mail.Body = "<HTML>\r\n\t<HEAD>\r\n\t\t<META HTTP-EQUIV='Content-Type' CONTENT='text/html; charset=utf-8'>\r\n\t</HEAD>\r\n\t<BODY>\r\n" + body + "\t</BODY>\r\n</HTML>";
				mail.IsBodyHtml = true;
				mail.BodyEncoding = System.Text.Encoding.UTF8;

				//include the users mail address as reply-to
				if (!String.IsNullOrEmpty(errorReport.Sender))
				{
					//OMICRON spam filter kills the mail if the user address is the From-address
					mail.Headers["Reply-To"] = errorReport.Sender;
				}

				// write the attachment to a MemoryStream then attach to email
				using (MemoryStream ms = new MemoryStream())
				{
					foreach (byte[] dataChunk in errorReport.DataChunks)
						ms.Write(dataChunk, 0, dataChunk.Length);

					ms.Position = 0; // CRITICAL
					mail.Attachments.Add(new Attachment(ms, errorReport.FileName, "application/zip"));

					// send the email through the omicron smtp server
					SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["MailServer"].ToString());
					smtp.Send(mail);
				}
			}
			catch (Exception e)
			{
				Log("SendEmail exception: " + e.ToString());

				SmtpFailedRecipientsException smtpexp = e as SmtpFailedRecipientsException;
				if (smtpexp != null)
				{
					foreach (SmtpFailedRecipientException recipient in smtpexp.InnerExceptions)
						Log("FailedRecipient: " + recipient.FailedRecipient + " StatusCode: " + recipient.StatusCode.ToString());
				}

				throw;
			}
		}
	}
}
