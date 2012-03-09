//#define DEBUG_OUTPUT

using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Net.Sockets;
using System.Web;
using System.Diagnostics;
using System.Net;

namespace MLifter.DAL.DB.DbMediaServer
{
	/// <summary>
	/// HTTP Server Class, with abstract methods to inherte from
	/// </summary>
	/// <remarks>Documented by Dev10, 2008-08-07</remarks>
	public abstract class CsHTTPServer
	{
		/// <summary>
		/// Final server port and hostname used by the listener
		/// </summary>
		protected int usedPortNum;
		/// <summary>
		/// Final server port and hostname used by the listener
		/// </summary>
		protected IPAddress usedIpAddress = IPAddress.Loopback;
		/// <summary>
		/// Gets the hostname.
		/// </summary>
		protected string hostname
		{
			get
			{
				//return usedIpAddress.ToString();    //there seems to be a problem with some vista systems where localhost is blocked
				return "localhost";
			}
		}

		//portnumber to use 0 if don'tmind
		private readonly int portNumber;

		//Listener + Thread
		private TcpListener listener;
		private System.Threading.Thread thread;

		/// <summary>
		/// Gets or sets the thread.
		/// </summary>
		/// <value>The thread.</value>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public Thread Thread
		{
			get { return thread; }
			set { thread = value; }
		}

		//To show to the outside world if the Server is ready and available.
		private bool isReady;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is ready.
		/// </summary>
		/// <value><c>true</c> if this instance is ready; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public bool IsReady
		{
			get { return isReady; }
			set { isReady = value; }
		}


		/// <summary>
		/// The response status
		/// </summary>
		public Hashtable respStatus;
		/// <summary>
		/// The server name
		/// </summary>
		public string Name = "CsHTTPServer/1.0.*";

		/// <summary>
		/// Gets the server port.
		/// </summary>
		/// <value>The server port.</value>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public int ServerPort
		{
			get
			{
				return usedPortNum;
			}
		}


		/// <summary>
		/// Gets a value indicating whether the Thread instance is alive.
		/// </summary>
		/// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public bool IsAlive
		{
			get
			{
				if (this.Thread == null)
					return false;

				return this.Thread.IsAlive;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CsHTTPServer"/> class.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public CsHTTPServer()
			: this(0)
		{
			//0 means that listner takes something
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CsHTTPServer"/> class.
		/// </summary>
		/// <param name="portNumber">The port number.</param>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public CsHTTPServer(int portNumber)
		{
			try
			{
				this.usedIpAddress = Dns.GetHostAddresses(this.hostname)[0];
			}
			catch { }
			this.portNumber = portNumber;
			isReady = false;
			respStatusInit();
		}


		private void respStatusInit()
		{
			respStatus = new Hashtable();

			respStatus.Add(200, "200 OK");
			respStatus.Add(201, "201 Created");
			respStatus.Add(202, "202 Accepted");
			respStatus.Add(204, "204 No Content");

			respStatus.Add(301, "301 Moved Permanently");
			respStatus.Add(302, "302 Redirection");
			respStatus.Add(304, "304 Not Modified");

			respStatus.Add(400, "400 Bad Request");
			respStatus.Add(401, "401 Unauthorized");
			respStatus.Add(403, "403 Forbidden");
			respStatus.Add(404, "404 Not Found");

			respStatus.Add(500, "500 Internal Server Error");
			respStatus.Add(501, "501 Not Implemented");
			respStatus.Add(502, "502 Bad Gateway");
			respStatus.Add(503, "503 Service Unavailable");
		}

		/// <summary>
		/// Setting up TcpListener and stuff
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public void Listen()
		{
			try
			{
				bool done = false;

				listener = new TcpListener(usedIpAddress, portNumber);
				listener.Start();

				//Get the portnumber the tcp listener is using
				usedPortNum = ((IPEndPoint)listener.LocalEndpoint).Port;

#if DEBUG && DEBUG_OUTPUT
				Debug.WriteLine("Listening On: IP: " + usedIpAddress + " Port: " + usedPortNum.ToString());
#endif

				IsReady = true;

				while (!done)
				{
					listener.Start();   //ToDo: Check why a restart is needed!

					if (listener.Pending())
					{
						CsHTTPRequest newRequest = new CsHTTPRequest(listener.AcceptTcpClient(), this);
						Thread Thread = new Thread(new ThreadStart(newRequest.Process));
						Thread.IsBackground = true;
						Thread.Name = "HTTP Request";
						Thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
						Thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
						Thread.Start();
					}
					else
						System.Threading.Thread.Sleep(10);
				}
			}
			catch (ObjectDisposedException)
			{ }
			catch (Exception e)
			{
				if (!(e is ThreadAbortException))
				{
					Trace.WriteLine("Listener Thread Exception was thrown: " + e.ToString());
				}
			}

		}

		object threadLock = new object();

		/// <summary>
		/// Starts Listen instance.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public void Start()
		{
			lock (threadLock)
			{
				if (this.Thread == null || !this.Thread.IsAlive)
				{
					this.Thread = new Thread(new ThreadStart(this.Listen));
					this.Thread.Name = "TCP Listener";
					this.Thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
					this.Thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
					this.Thread.IsBackground = true;
					this.Thread.Start();
				}
			}
		}

		/// <summary>
		/// Stops Listen instance.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public void Stop()
		{
			try
			{
				if (listener != null)
					listener.Stop();
				lock (threadLock)
				{
					if (this.Thread != null && this.Thread.IsAlive)
						this.Thread.Abort();
				}
			}
			catch (Exception exp)
			{
				Trace.WriteLine("Server stop exception: " + exp.ToString());
			}
		}

		/// <summary>
		/// To be overwritten by the corresponding subclass.
		/// </summary>
		/// <param name="rq">The rq.</param>
		/// <param name="rp">The rp.</param>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public abstract void OnResponse(ref HTTPRequestStruct rq, ref HTTPResponseStruct rp);

	}
}
