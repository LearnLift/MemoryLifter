using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using MLifter.Components;
using ICSharpCode.SharpZipLib.Zip;
using MLifter.BusinessLayer;
using System.Reflection;
using MLifter.Properties;
using System.Diagnostics;
using MLifter.DAL.Interfaces;

namespace MLifter.Classes
{
	/// <summary>
	/// Generates error reports and submits them to the error handler.
	/// </summary>
	/// <remarks>Documented by Dev02, 2009-07-14</remarks>
	public static class ErrorReportGenerator
	{
		/// <summary>
		/// Posts a new error to create a report.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="fatal">Set to true in case the error is fatal (ML cannot resume operation).</param>
		/// <remarks>Documented by Dev02, 2009-07-14</remarks>
		public static void ReportError(Exception exception, bool fatal)
		{
			if (exception != null)
			{
				using (Stream stream = GenerateReport(exception))
				{
					string folder = Setup.ErrorReportsFilePath;

					if (!Directory.Exists(folder))
						Directory.CreateDirectory(folder);

					string file = Path.Combine(folder, string.Format("{0}_ErrorReport_{1}_{2}.zip", AssemblyData.Title, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), AssemblyData.AssemblyVersion));

					using (FileStream outstream = File.Create(file))
					{
						stream.Position = 0;
						byte[] buffer = ((MemoryStream)stream).GetBuffer();
						outstream.Write(buffer, 0, buffer.Length);
						outstream.Close();
					}
					StartErrorHandlerExecutable(fatal, Setup.RunningFromStick());
				}
			}

			if (fatal)
				Environment.Exit(-1);
		}

		/// <summary>
		/// Processes the pending reports.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-14</remarks>
		public static void ProcessPendingReports()
		{
			StartErrorHandlerExecutable(false, Setup.RunningFromStick());
		}

		/// <summary>
		/// Starts the error handler executable.
		/// </summary>
		/// <param name="fatal">if set to <c>true</c> [fatal].</param>
		/// <param name="stickMode">if set to <c>true</c> [stick mode].</param>
		/// <remarks>Documented by Dev02, 2009-07-16</remarks>
		private static void StartErrorHandlerExecutable(bool fatal, bool stickMode)
		{
			try
			{
				ProcessStartInfo psi = new ProcessStartInfo();
				psi.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.MLIFTER_ERROR_HANDLER);
				if (!File.Exists(psi.FileName))
					return;
				psi.Arguments = string.Format("{0} {1}", fatal.ToString(), stickMode.ToString());
				System.Diagnostics.Process.Start(psi);
			}
			catch (Exception exp)
			{
				Trace.WriteLine("Exception during starting of error handler:");
				Trace.WriteLine(exp.ToString());
			}
		}

		/// <summary>
		/// Generates a new error report.
		/// </summary>
		/// <param name="exp">The exp.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-07-14</remarks>
		private static Stream GenerateReport(Exception exception)
		{
			if (exception == null)
				return null;

			//generate report
			MemoryStream report = new MemoryStream();
			ZipOutputStream zipstream = new ZipOutputStream(report);
			zipstream.UseZip64 = UseZip64.Off;  // AAB-20090720: Zip64 caused some problem when modifing the archive (ErrorReportHandler.cs) - Zip64 is required to bypass the 4.2G limitation of the original Zip format (http://en.wikipedia.org/wiki/ZIP_(file_format))
			zipstream.SetLevel(9);

			AddErrorReport(zipstream, exception);
			AddScreenshot(zipstream);
			AddUserProfileFootPrint(zipstream);
			AddConnections(zipstream);

			zipstream.Finish();
			zipstream.Flush();

			return report;
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>The dictionary.</value>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		private static Dictionary Dictionary
		{
			get { return MainForm.LearnLogic.Dictionary; }
		}

		#region Error report generation helpers
		/// <summary>
		/// Adds the error report.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		private static void AddErrorReport(ZipOutputStream stream, Exception exception)
		{
			#region collect data
			//fetch some information about ML
			Dictionary<string, string> mlInfos = new Dictionary<string, string>();
			mlInfos["MLVersion"] = AssemblyData.AssemblyVersion;
			mlInfos["MLPath"] = Application.StartupPath;
			mlInfos["MLOnStick"] = Setup.RunningFromStick().ToString();
			try
			{
				if (Dictionary != null)
				{
					mlInfos["DatabaseType"] = Dictionary.DictionaryDAL.Parent.CurrentUser.ConnectionString.Typ.ToString();
					if (Dictionary.IsDB)
					{
						mlInfos["DatabaseVersion"] = Dictionary.DictionaryDAL.Parent.CurrentUser.Database.DatabaseVersion.ToString();
						List<string> versions = new List<string>();
						foreach (DataLayerVersionInfo info in Dictionary.DictionaryDAL.Parent.CurrentUser.Database.SupportedDataLayerVersions)
						{
							string prefix = "";
							switch (info.Type)
							{
								case VersionType.LowerBound:
									prefix = ">";
									break;
								case VersionType.UpperBound:
									prefix = "<";
									break;
								case VersionType.Equal:
								default:
									break;
							}
							versions.Add(prefix + info.Version);
						}
						mlInfos["SupportedDataLayerVersions"] = String.Join(", ", versions.ToArray());
						mlInfos["DatabaseUser"] = Dictionary.DictionaryDAL.Parent.CurrentUser.UserName;
					}
					mlInfos["DatabaseInfo"] = Dictionary.DictionaryDAL.Parent.CurrentUser.ConnectionString.ToString();
					mlInfos["DictionaryDescription"] = Dictionary.Description;
					mlInfos["DictionaryAuthor"] = Dictionary.Author;
					mlInfos["DictionaryCategory"] = Dictionary.Category.ToString();
					mlInfos["DictionaryPath"] = Dictionary.DictionaryPath;
					mlInfos["DictionaryCardID"] = Dictionary.CurrentCard.ToString();
					mlInfos["DictionaryCardCount"] = Dictionary.Cards.Cards.Count.ToString();
					mlInfos["DictionaryChapterCount"] = Dictionary.Chapters.Chapters.Count.ToString();
					mlInfos["DictionaryStyleSheetsUsed"] = Dictionary.UseDictionaryStyleSheets.ToString();
					mlInfos["DictionaryQuestionStyleSheet"] = Dictionary.QuestionStyleSheet;
					mlInfos["DictionaryAnswerStyleSheet"] = Dictionary.AnswerStyleSheet;
				}
				else
				{
					mlInfos["Dictionary"] = "No dictionary open.";
				}
			}
			catch
			{
				mlInfos["Dictionary"] = "Readout failed.";
			}
			try
			{
				if (Dictionary != null && Dictionary.IsFileDB)
				{
					mlInfos["DictionarySize"] = new FileInfo(Dictionary.DictionaryPath).Length.ToString() + " Bytes";
					mlInfos["DictionaryFileAttributes"] = new FileInfo(Dictionary.DictionaryPath).Attributes.ToString();
					mlInfos["DictionaryACL"] = new FileInfo(Dictionary.DictionaryPath).GetAccessControl().GetSecurityDescriptorSddlForm(System.Security.AccessControl.AccessControlSections.All);
				}
				mlInfos["LoggedinUser"] = string.Format("{0} - {1}", Environment.UserName, new System.Security.Principal.NTAccount(Environment.UserName).Translate(typeof(System.Security.Principal.SecurityIdentifier)).Value);
			}
			catch
			{
				mlInfos["DictionaryFile"] = "Readout failed.";
			}

			//get the extensions
			Dictionary<string, string> extInfos = new Dictionary<string, string>();
			try
			{
				if (Dictionary != null && Dictionary.DictionaryDAL.Extensions.Count > 0)
					foreach (IExtension extension in Dictionary.DictionaryDAL.Extensions)
						extInfos["Extension"] = extension.ToString() + " - (" + extension.Id + ")";
				else
					extInfos["Extensions"] = "No extensions.";
			}
			catch
			{
				extInfos["Extensions"] = "Readout failed.";
			}

			//fetch some basic system information
			Dictionary<string, string> sysInfos = new Dictionary<string, string>();
			try
			{
				sysInfos["OSVersion"] = System.Environment.OSVersion.ToString();
				sysInfos["IEVersion"] = MLifter.Generics.Methods.GetInternetExplorerVersion().ToString();
				sysInfos["CLRVersion"] = System.Environment.Version.ToString();
				sysInfos["SystemDirectory"] = System.Environment.SystemDirectory;
				sysInfos["WMPVersion"] = MLifter.Generics.Methods.GetWindowsMediaPlayerVersion().ToString();
			}
			catch
			{
				sysInfos["SystemInformation"] = "Readout failed.";
			}
			#endregion

			#region write data
			//add the XSL to the output
			FileInfo errorReportStylesheet = new FileInfo(Path.Combine(Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Properties.Settings.Default.AppDataFolderDesigns)), @"System\ErrorHandler\default.xsl"));
			if (errorReportStylesheet.Exists)
			{
				ZipEntry styleEntry = new ZipEntry("ErrorReport.xsl");
				stream.PutNextEntry(styleEntry);
				using (FileStream styleStream = errorReportStylesheet.OpenRead())
				{
					try
					{
						int i;
						do
						{
							i = styleStream.ReadByte();
							if (i != -1) stream.WriteByte((byte)i);
						} while (i != -1);
					}
					catch (IOException) { }
				}
			}

			//write the error report
			ZipEntry entry = new ZipEntry("ErrorReport.xml");
			stream.PutNextEntry(entry);

			XmlWriterSettings settings = new XmlWriterSettings() { CloseOutput = true, Encoding = Encoding.Unicode, Indent = true };
			XmlWriter errorReportXML = XmlWriter.Create(stream, settings);
			errorReportXML.WriteStartDocument();
			if (errorReportStylesheet.Exists)
			{
				errorReportXML.WriteProcessingInstruction("xml-stylesheet", string.Format("type='Text/xsl' href='{0}'", errorReportStylesheet.FullName));
				errorReportXML.WriteProcessingInstruction("xml-stylesheet", string.Format("type='Text/xsl' href='{0}'", "ErrorReport.xsl"));
			}

			errorReportXML.WriteComment(String.Format("{0} {1} Error Report, generated {2}", AssemblyData.Title, AssemblyData.AssemblyVersion, System.DateTime.Now.ToString()));
			errorReportXML.WriteStartElement("ErrorReport");

			errorReportXML.WriteStartElement("MLInformation");
			foreach (KeyValuePair<string, string> pair in mlInfos)
				errorReportXML.WriteElementString(pair.Key, pair.Value);
			errorReportXML.WriteEndElement();

			errorReportXML.WriteStartElement("ExtensionsInformation");
			foreach (KeyValuePair<string, string> pair in extInfos)
				errorReportXML.WriteElementString(pair.Key, pair.Value);
			errorReportXML.WriteEndElement();

			errorReportXML.WriteStartElement("SystemInformation");
			foreach (KeyValuePair<string, string> pair in sysInfos)
				errorReportXML.WriteElementString(pair.Key, pair.Value);
			errorReportXML.WriteEndElement();

			XMLWriteException(errorReportXML, exception);

			errorReportXML.WriteEndElement();
			errorReportXML.Flush();
			#endregion
		}

		/// <summary>
		/// Adds the screenshot.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		private static void AddScreenshot(ZipOutputStream stream)
		{
			try
			{
				int screenno = 1;
				foreach (Screen screen in Screen.AllScreens)
				{
					ZipEntry entry = new ZipEntry(string.Format("Screenshot{0}.jpg", screenno));
					stream.PutNextEntry(entry);

					Bitmap screenShotBitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
					Graphics screenShotGraphics = Graphics.FromImage(screenShotBitmap);
					screenShotGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);
					EncoderParameters ep = new EncoderParameters(1);
					ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 15L);
					ConvertToGrayscale(screenShotBitmap).Save(stream, GetEncoderInfo("image/jpeg"), ep);
					screenno++;
				}
			}
			catch { }
		}

		/// <summary>
		/// Adds the user profile foot print.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <remarks>Documented by Dev03, 2009-07-15</remarks>
		private static void AddUserProfileFootPrint(ZipOutputStream stream)
		{
			try
			{
				System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd",
					String.Format("/c dir /s \"{0}\"", Setup.UserProfileFootPrintPath)) { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo = procStartInfo;
				proc.Start();
				string result = proc.StandardOutput.ReadToEnd();
				ZipEntry entry = new ZipEntry("ProfileFootPrint.txt");
				stream.PutNextEntry(entry);
				byte[] bytes = Encoding.Default.GetBytes(result); ;
				stream.Write(bytes, 0, bytes.Length);
			}
			catch { }
		}

		private static void AddConnections(ZipOutputStream stream)
		{
			try
			{
				if (MLifter.BusinessLayer.LearningModulesIndex.ConnectionsHandler != null)
				{
					ZipEntry entry = new ZipEntry("Connections" + MLifter.DAL.Helper.ConfigFileExtension);
					stream.PutNextEntry(entry);
					XmlWriterSettings settings = new XmlWriterSettings() { CloseOutput = true, Encoding = Encoding.Unicode, Indent = true };
					XmlWriter writer = XmlWriter.Create(stream, settings);
					writer.WriteStartElement("Configuration");
					writer.WriteStartElement("Connections");
					foreach (IConnectionString conString in MLifter.BusinessLayer.LearningModulesIndex.ConnectionsHandler.ConnectionStrings)
					{
						//mask the password
						if (conString.ConnectionType == MLifter.DAL.DatabaseType.PostgreSQL)
							(conString as PostgreSqlConnectionStringBuilder).Password = "********";

						writer.WriteStartElement("Connection");
						conString.WriteXml(writer);
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
					writer.WriteEndElement();
					writer.Flush();
				}
			}
			catch { }
		}

		/// <summary>
		/// Writes an Exception to an XML writer, recursive writing of Inner Exceptions.
		/// </summary>
		/// <param name="xmlw">The XML writer.</param>
		/// <param name="exception">The exception.</param>
		/// <remarks>Documented by Dev02, 2007-11-12</remarks>
		private static void XMLWriteException(XmlWriter xmlWriter, Exception exception)
		{
			if (exception != null && xmlWriter != null)
			{
				xmlWriter.WriteStartElement("Exception");
				try
				{
					xmlWriter.WriteElementString("Type", exception.GetType().ToString());
					xmlWriter.WriteElementString("Message", exception.Message);
					xmlWriter.WriteElementString("Stacktrace", exception.StackTrace);
					if (exception.InnerException != null)
						XMLWriteException(xmlWriter, exception.InnerException);
				}
				catch { }
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
		}

		/// <summary>
		/// Gets the image encoder info.
		/// </summary>
		/// <param name="mimeType">MIME type of the bitmap encoder.</param>
		/// <returns>The image encoder info. (System.Drawing.Imaging.ImageCodecInfo)</returns>
		/// <remarks>Documented by Dev02, 2007-11-20</remarks>
		private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

		/// <summary>
		/// Converts a bitmap to grayscale using a color matrix.
		/// </summary>
		/// <param name="original">The original bitmap.</param>
		/// <returns>The grayscale bitmap.</returns>
		/// <remarks>Documented by Dev02, 2007-11-20</remarks>
		private static Bitmap ConvertToGrayscale(Bitmap original)
		{
			//create a blank bitmap the same size as original
			Bitmap newBitmap =
			   new Bitmap(original.Width, original.Height);

			//get a graphics object from the new image
			Graphics g = Graphics.FromImage(newBitmap);

			//create the grayscale ColorMatrix
			ColorMatrix colorMatrix = new ColorMatrix(
			   new float[][]
				  {
					 new float[] {.3f, .3f, .3f, 0, 0},
					 new float[] {.59f, .59f, .59f, 0, 0},
					 new float[] {.11f, .11f, .11f, 0, 0},
					 new float[] {0, 0, 0, 1, 0},
					 new float[] {0, 0, 0, 0, 1}
				  });

			//create some image attributes
			ImageAttributes attributes = new ImageAttributes();

			//set the color matrix attribute
			attributes.SetColorMatrix(colorMatrix);

			//draw the original image on the new image
			//using the grayscale color matrix
			g.DrawImage(original,
			   new Rectangle(0, 0, original.Width, original.Height),
			   0, 0, original.Width, original.Height,
			   GraphicsUnit.Pixel, attributes);

			//dispose the Graphics object
			g.Dispose();
			return newBitmap;
		}
		#endregion
	}
}
