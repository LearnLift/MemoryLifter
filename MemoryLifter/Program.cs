using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting;

using MLifter.Properties;
using MLifter.Classes;
using MLifter.Controls;
using System.Data.SqlServerCe;
using System.Security.Policy;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace MLifter
{
	public abstract class Program
	{
		private static Mutex instanceMutex = null;
		private static string UniqueChannelName = string.Empty;
		private static string UniqueChannelPortName = string.Empty;
		private static string ClientURL = string.Empty;
		private static string ServiceURL = string.Empty;
		private static IChannel IPCchannel = null;
		private static bool suspendIPC = false;

		public static MainForm MainForm = null;

		/// <summary>
		/// The main entry point for the application.
		/// Also works as the last place to catch exceptions before they result in arrayList runtime error.
		/// </summary>
		/// <remarks>Documented by Dev01, 2007-07-20</remarks>
		[STAThread]
		static void Main(string[] args)
		{
			TaskDialog.ForceEmulationMode = true;
			Application.SetCompatibleTextRenderingDefault(false);
			Application.EnableVisualStyles();

			//register errorhandler as default unhandled exception handler
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			List<string> arguments = new List<string>(args);

			//check settings file before any other setting access occurs
			CheckSettings();

			#region Check for updates
			if (arguments.Contains("--no-autoupdate"))
			{
				arguments.Remove("--no-autoupdate");
				UpdateChecked = true;
			}
			else if (!Properties.Settings.Default.CheckForUpdates)
			{
				UpdateChecked = true;
			}
			else
			{
				try
				{
					Thread updateChecker = new Thread(new ThreadStart(CheckForUpdate));
					updateChecker.CurrentCulture = Thread.CurrentThread.CurrentCulture;
					updateChecker.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
					updateChecker.IsBackground = true;
					updateChecker.Name = "Check for Updates";
					updateChecker.Priority = ThreadPriority.BelowNormal;
					updateChecker.Start();
				}
				catch (Exception e)
				{
					Trace.WriteLine(e.ToString());
				}
			}
			#endregion

			Mutex splashMutex = new Mutex(false, "Local\\MLifterSplashMutex");

			#region Running from stick
			//execute the program in a new appdomain with shadow copying enabled
			if (Setup.RunningFromStick() && !AppDomain.CurrentDomain.ShadowCopyFiles)
			{
				SplashManager splashManager = new SplashManager(typeof(Splash));
				splashManager.InitialMessage = Resources.SPLASHSCREEN_PREPARING;
				splashManager.ShowSplash();

				AppDomainSetup setup = new AppDomainSetup();
				setup.ShadowCopyFiles = "true";
				setup.ShadowCopyDirectories = null; //all files

				Evidence evidence = System.Reflection.Assembly.GetEntryAssembly().Evidence;

				AppDomain appdomain = AppDomain.CreateDomain("MLifter", evidence, setup);
				appdomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				AssemblyName entryassembly = System.Reflection.Assembly.GetEntryAssembly().GetName();

				appdomain.Load(entryassembly);

				//test the mutex every 100 ms and hide the splash screen if it cannot be taken anymore
				Thread mutexWatcherThread = new Thread(new ThreadStart(delegate
					{
						TimeSpan wait = new TimeSpan(0, 0, 0, 0, 20);
						try
						{
							while (true)
							{
								Thread.Sleep(100);
								if (splashMutex.WaitOne(wait, false))
									splashMutex.ReleaseMutex();
								else
									break;
							}
						}
						catch (AbandonedMutexException)
						{ }
						finally
						{
							splashManager.HideSplash();
						}
					}));
				mutexWatcherThread.Name = "Splash Mutex Watcher Thread";
				mutexWatcherThread.IsBackground = true;
				mutexWatcherThread.Start();

				appdomain.ExecuteAssemblyByName(entryassembly, args); //evidence removed as second parameter to avoid compiler warning
				//let main function end
			}
			#endregion
			else
			{
				SplashManager splashManager = new SplashManager(typeof(Splash));
				splashManager.EnableFadingMainForm = true;

				#region Only one program instance
				//register IPC channel so that only one instance can be active
				GetIPCData(out UniqueChannelName, out UniqueChannelPortName, out ClientURL, out ServiceURL);

				//check if this is the first instance
				bool firstInstance = true;
				if (Settings.Default.AllowOnlyOneInstance)
				{
					try
					{
						instanceMutex = new Mutex(false, "Local\\MLifterMutex", out firstInstance);
					}
					catch (Exception exp)
					{
						System.Diagnostics.Trace.WriteLine("Failed to create Mutex: " + exp.ToString());
					}
				}

#if !DEBUG
				//show the splashscreen
				if (firstInstance || !Settings.Default.AllowOnlyOneInstance)
					splashManager.ShowSplash();
#endif

				//begin preparing styles
				MLifter.Components.StyleHandler.BeginStylePreparation(System.IO.Path.Combine(Application.StartupPath, Properties.Settings.Default.AppDataFolderDesigns));

				try
				{
					StartIPC();
					SuspendIPC = true; //suspend IPC until ML is fully loaded
				}
				catch (Exception exp)
				{
					System.Diagnostics.Trace.WriteLine("Failed to start IPC server: " + exp.ToString());
				}

				if (!firstInstance && Settings.Default.AllowOnlyOneInstance)
				{
					try
					{
						if (arguments.Count > 0)
							SendPathToIPC(arguments[0]);
						else
							BringMLifterToFront();
						Environment.Exit(-1);
					}
					catch (Exception exp)
					{
						System.Diagnostics.Trace.WriteLine("Failed to SendPathToIPC/BringMLifterToFront with another instance active: "
							+ exp.ToString());
					}
				}
				#endregion

				// Run Application
				Program.MainForm = new MainForm(arguments.ToArray());
#if !DEBUG
				splashManager.HideMainForm(Program.MainForm); //register MainForm to be automatically hidden and showed
#endif

				#region Running from stick (Mutex check)
				//take the mutex signal for 500 ms to trigger the hiding of the previous splash screen
				Thread mutexReleaseThread = new Thread(new ThreadStart(delegate
				{
					TimeSpan wait = new TimeSpan(0, 0, 0, 0, 100);
					try
					{
						if (splashMutex.WaitOne(wait, false))
						{
							Thread.Sleep(500);
							splashMutex.ReleaseMutex();
						}
					}
					catch (AbandonedMutexException)
					{ }
				}));
				mutexReleaseThread.Name = "Splash Mutex Release Thread";
				mutexReleaseThread.IsBackground = true;
				mutexReleaseThread.Start();
				#endregion

				Application.Run(Program.MainForm);
			}
		}

		/// <summary>
		/// true if no update found.
		/// </summary>
		public static bool UpdateChecked = false;
		/// <summary>
		/// Checks for update.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-07-15</remarks>
		public static void CheckForUpdate()
		{
			try
			{
				WebClient client = new WebClient();
				string updateVersion = client.DownloadString(String.Format("{0}?base={1}&beta={2}&onstick={3}",
					Settings.Default.UpdateVersionUrl, MLifter.DAL.Tools.AssemblyData.Version.ToString(2),
					Settings.Default.CheckForBetaUpdates.ToString(), Setup.RunningFromStick()));
				if (updateVersion.Length == 0)
				{
					Trace.WriteLine("CheckForUpdate got an empty version string.");
					return;
				}
				Version newVersion = new Version(updateVersion);

				if (newVersion > MLifter.DAL.Tools.AssemblyData.Version)
				{
					string updaterPath = Path.Combine(Application.StartupPath, "MLifterUpdateHandler.exe");
					Process.Start(updaterPath, "-v " + updateVersion);
					Environment.Exit(-1);
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.ToString());
			}
			finally
			{
				UpdateChecked = true;
			}
		}

		/// <summary>
		/// Gets the IPC data.
		/// </summary>
		/// <param name="UniqueChannelName">Name of the unique channel.</param>
		/// <param name="UniqueChannelPortName">Name of the unique channel port.</param>
		/// <param name="ClientURL">The client URL.</param>
		/// <param name="ServiceURL">The service URL.</param>
		/// <remarks>Documented by Dev02, 2009-06-26</remarks>
		public static void GetIPCData(out string UniqueChannelName, out string UniqueChannelPortName, out string ClientURL, out string ServiceURL)
		{
			UniqueChannelName = GetAssemblyGUID() + "/" + GetMd5Sum(GetAssemblyPath(false));
			UniqueChannelPortName = "MLifterIPCChannel";
			ClientURL = "ipc://" + UniqueChannelPortName + "/" + UniqueChannelName;
			ServiceURL = UniqueChannelName;
		}

		/// <summary>
		/// Handles the UnhandledException event of the CurrentDomain control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-06-25</remarks>
		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			HandleUnhandledException(e);
		}
		/// <summary>
		/// Handles the ThreadException event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-01-21</remarks>
		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			HandleUnhandledException(new UnhandledExceptionEventArgs(e.Exception, true));
		}
		/// <summary>
		/// Handles the unhandled exception.
		/// </summary>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-06-25</remarks>
		private static void HandleUnhandledException(UnhandledExceptionEventArgs e)
		{
			try
			{
				Exception ex = e.ExceptionObject as Exception;

				Trace.WriteLine("Unhandled Exception: " + ex.ToString());

				try
				{
					string errorMsg = "An application error occurred. Please contact the administrator with the following information:"
						+ Environment.NewLine + Environment.NewLine;

					// Since we can't prevent the app from terminating, log this to the event log.
					if (!EventLog.SourceExists("MemoryLifter"))
						EventLog.CreateEventSource("MemoryLifter", "Application");

					// Create an EventLog instance and assign its source.
					EventLog myLog = new EventLog();
					myLog.Source = "MemoryLifter";
					myLog.WriteEntry(errorMsg + ex.Message + Environment.NewLine + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ex.StackTrace);
				}
				catch (System.Security.SecurityException)
				{
					Trace.WriteLine("Could not write exception to the Windows EventLog for security reasons.");
				}

				ErrorReportGenerator.ReportError(e.ExceptionObject as Exception, true);
			}
			catch (Exception exp)
			{
				try
				{
					MessageBox.Show("Could not create an error report. Reason: " + Environment.NewLine + exp.ToString(),
						"Error Report Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
				finally
				{
					Environment.Exit(-1);
				}
			}
		}

		/// <summary>
		/// Checks for a malformed settings file and rewrites it.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-18</remarks>
		public static void CheckSettings()
		{
			//check for a malformed settings file (fix for [ML-546] Configuration system failed to initialize)
			try
			{
				Settings.Default.AllowOnlyOneInstance = Settings.Default.AllowOnlyOneInstance;
			}
			catch (System.Configuration.ConfigurationErrorsException exp)
			{
				bool errorfixed = false;
				if (exp.InnerException != null && exp.InnerException is System.Configuration.ConfigurationErrorsException)
				{
					System.Configuration.ConfigurationErrorsException innerexception = exp.InnerException as System.Configuration.ConfigurationErrorsException;
					if (System.IO.File.Exists(innerexception.Filename))
					{
						try
						{
							System.IO.File.Delete(innerexception.Filename);
							errorfixed = true;
						}
						catch { }
					}
				}
				if (System.IO.File.Exists(exp.Filename))
				{
					try
					{
						System.IO.File.Delete(exp.Filename);
						errorfixed = true;
					}
					catch { }
				}
				if (errorfixed)
				{
					MainForm.BringToFront();
					MainForm.TopMost = true;
					Application.DoEvents();

					MessageBox.Show(Properties.Resources.INITIALIZE_SETTINGS_ERROR, Properties.Resources.INITIALIZE_SETTINGS_CAPTION);
					Environment.Exit(-1);
				}
				else
					throw;
			}
		}

		/// <summary>
		/// Starts the IPC-Server.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		public static void StartIPC()
		{
			if (IPCrunning)
				return;

			RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.On;
			ChannelServices.RegisterChannel(IPCchannel = new IpcChannel(UniqueChannelPortName), false);
			RemotingConfiguration.ApplicationName = UniqueChannelName;
			RemotingConfiguration.RegisterWellKnownServiceType(
			   typeof(GlobalDictionaryLoader),
			   ServiceURL,
			   WellKnownObjectMode.SingleCall
			);
		}

		/// <summary>
		/// Ends the IPC.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-07</remarks>
		public static void EndIPC()
		{
			if (!IPCrunning)
				return;

			if (instanceMutex != null)
				instanceMutex.Close();

			ChannelServices.UnregisterChannel(IPCchannel);

			IPCchannel = null;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [suspend IPC].
		/// </summary>
		/// <value><c>true</c> if [suspend IPC]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-02-07</remarks>
		public static bool SuspendIPC
		{
			get { return suspendIPC; }
			set { suspendIPC = value; }
		}

		/// <summary>
		/// Gets a value indicating whether [IPC server is running].
		/// </summary>
		/// <value><c>true</c> if [IP crunning]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-02-07</remarks>
		public static bool IPCrunning
		{
			get { return IPCchannel != null; }
		}

		/// <summary>
		/// Sends the path to the IPC-Server.
		/// </summary>
		/// <param name="Path">The path.</param>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		public static void SendPathToIPC(string Path)
		{
			RemotingConfiguration.RegisterWellKnownClientType(typeof(GlobalDictionaryLoader), ClientURL);
			GlobalDictionaryLoader loader = new GlobalDictionaryLoader();
			loader.LoadDictionary(Path);
		}

		/// <summary>
		/// Brings the MLifter to front.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		public static void BringMLifterToFront()
		{
			RemotingConfiguration.RegisterWellKnownClientType(typeof(GlobalDictionaryLoader), ClientURL);
			GlobalDictionaryLoader loader = new GlobalDictionaryLoader();
			loader.BringToFront();
		}

		/// <summary>
		/// Gets the assembly GUID.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-11-28</remarks>
		public static string GetAssemblyGUID()
		{
			Assembly assy;
			object Attributes;

			assy = Assembly.GetExecutingAssembly();
			Attributes = assy.GetCustomAttributes(typeof(GuidAttribute), false);

			if (((object[])Attributes).Length != 0)
				return ((GuidAttribute)((object[])Attributes)[0]).Value;

			return "MLifterGUIDnotFound";
		}

		/// <summary>
		/// Gets the assembly path.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-06-26</remarks>
		public static string GetAssemblyPath(bool withFilename)
		{
			try
			{
				Assembly assy = Assembly.GetExecutingAssembly();
				return withFilename ? assy.Location : System.IO.Path.GetDirectoryName(assy.Location);
			}
			catch
			{
				return "MLifterAssemblyPathnotFound";
			}
		}

		/// <summary>
		/// Gets the MD5 sum.
		/// </summary>
		/// <param name="str">The STR.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2008-02-26</remarks>
		static public string GetMd5Sum(string str)
		{
			// First we need to convert the string into bytes, which
			// means using a Text encoder.
			Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

			// Create a buffer large enough to hold the string
			byte[] unicodeText = new byte[str.Length * 2];
			enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

			// Now that we have a byte array we can ask the CSP to hash it
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(unicodeText);

			// Build the final string by converting each byte
			// into hex and appending it to a StringBuilder
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < result.Length; i++)
			{
				sb.Append(result[i].ToString("X2"));
			}

			// And return it
			return sb.ToString();
		}
	}
}
