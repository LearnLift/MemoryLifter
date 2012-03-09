//# define debug_output

using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace MLifter.DAL.DB.DbMediaServer
{
	enum RState
	{
		METHOD, URL, URLPARM, URLVALUE, VERSION,
		HEADERKEY, HEADERVALUE, BODY, OK
	};

	enum RespState
	{
		OK = 200,
		BAD_REQUEST = 400,
		NOT_FOUND = 404
	}


	/// <summary>
	/// Content struct for a HTTP request
	/// </summary>
	public struct HTTPRequestStruct
	{
		/// <summary>
		/// The Method
		/// </summary>
		public string Method;
		/// <summary>
		/// The URL
		/// </summary>
		public string URL;
		/// <summary>
		/// The version
		/// </summary>
		public string Version;
		/// <summary>
		/// The arguments
		/// </summary>
		public Hashtable Args;
		/// <summary>
		/// ToDo: ?
		/// </summary>
		public bool Execute;
		/// <summary>
		/// The headers
		/// </summary>
		public Hashtable Headers;
		/// <summary>
		/// The body size
		/// </summary>
		public int BodySize;
		/// <summary>
		/// The body content
		/// </summary>
		public byte[] BodyData;
	}

	/// <summary>
	/// Content struct for a HTTP response
	/// </summary>
	public struct HTTPResponseStruct
	{
		/// <summary>
		/// The status
		/// </summary>
		public int status;
		/// <summary>
		/// The version
		/// </summary>
		public string version;
		/// <summary>
		/// The headers
		/// </summary>
		public Hashtable Headers;
		/// <summary>
		/// The body size
		/// </summary>
		public int BodySize;
		/// <summary>
		/// The body content
		/// </summary>
		public byte[] BodyData;
		/// <summary>
		/// The response stream
		/// </summary>
		public Stream mediaStream;
	}


	/// <summary>
	/// Takes care of the incoming http request, parses the request
	/// </summary>
	/// <remarks>Documented by Dev10, 2008-08-07</remarks>
	public class CsHTTPRequest
	{
		private TcpClient client;
		private RState ParserState;
		private HTTPRequestStruct HTTPRequest;
		private HTTPResponseStruct HTTPResponse;

		byte[] myReadBuffer;

		CsHTTPServer Parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="CsHTTPRequest"/> class.
		/// </summary>
		/// <param name="client">The TcpClient.</param>
		/// <param name="Parent">The Server instance (TcpListener).</param>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public CsHTTPRequest(TcpClient client, CsHTTPServer Parent)
		{
			this.client = client;
			this.Parent = Parent;

			this.HTTPResponse.BodySize = 0;

		}

		/// <summary>
		/// Processes actually the HTTP Request.
		/// </summary>
		/// <remarks>Documented by Dev10, 2008-08-07</remarks>
		public void Process()
		{
			myReadBuffer = new byte[client.ReceiveBufferSize];
			String myCompleteMessage = "";
			int numberOfBytesRead = 0;

#if DEBUG && debug_output
			Debug.WriteLine("Connection accepted. Buffer: " + client.ReceiveBufferSize.ToString());
#endif
			NetworkStream ns = client.GetStream();

			string hValue = "";
			string hKey = "";

			try
			{
				// binary data buffer index
				int bfndx = 0;

				// Incoming message may be larger than the buffer size.
				do
				{
					numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);
					myCompleteMessage =
						String.Concat(myCompleteMessage, Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

					// read buffer index
					int ndx = 0;
					do
					{
						switch (ParserState)
						{
							case RState.METHOD:
								if (myReadBuffer[ndx] != ' ')
									HTTPRequest.Method += (char)myReadBuffer[ndx++];
								else
								{
									ndx++;
									ParserState = RState.URL;
								}
								break;
							case RState.URL:
								if (myReadBuffer[ndx] == '?')
								{
									ndx++;
									hKey = "";
									HTTPRequest.Execute = true;
									HTTPRequest.Args = new Hashtable();
									ParserState = RState.URLPARM;
								}
								else if (myReadBuffer[ndx] != ' ')
									HTTPRequest.URL += (char)myReadBuffer[ndx++];
								else
								{
									ndx++;
									HTTPRequest.URL = Uri.UnescapeDataString(HTTPRequest.URL);
									ParserState = RState.VERSION;
								}
								break;
							case RState.URLPARM:
								if (myReadBuffer[ndx] == '=')
								{
									ndx++;
									hValue = "";
									ParserState = RState.URLVALUE;
								}
								else if (myReadBuffer[ndx] == ' ')
								{
									ndx++;

									HTTPRequest.URL = Uri.UnescapeDataString(HTTPRequest.URL);
									ParserState = RState.VERSION;
								}
								else
								{
									hKey += (char)myReadBuffer[ndx++];
								}
								break;
							case RState.URLVALUE:
								if (myReadBuffer[ndx] == '&')
								{
									ndx++;
									hKey = Uri.UnescapeDataString(hKey);
									hValue = Uri.UnescapeDataString(hValue);
									HTTPRequest.Args[hKey] = HTTPRequest.Args[hKey] != null ? HTTPRequest.Args[hKey] + ", " + hValue : hValue;
									hKey = "";
									ParserState = RState.URLPARM;
								}
								else if (myReadBuffer[ndx] == ' ')
								{
									ndx++;
									hKey = Uri.UnescapeDataString(hKey);
									hValue = Uri.UnescapeDataString(hValue);
									HTTPRequest.Args[hKey] = HTTPRequest.Args[hKey] != null ? HTTPRequest.Args[hKey] + ", " + hValue : hValue;

									HTTPRequest.URL = Uri.UnescapeDataString(HTTPRequest.URL);
									ParserState = RState.VERSION;
								}
								else
								{
									hValue += (char)myReadBuffer[ndx++];
								}
								break;
							case RState.VERSION:
								if (myReadBuffer[ndx] == '\r')
									ndx++;
								else if (myReadBuffer[ndx] != '\n')
									HTTPRequest.Version += (char)myReadBuffer[ndx++];
								else
								{
									ndx++;
									hKey = "";
									HTTPRequest.Headers = new Hashtable();
									ParserState = RState.HEADERKEY;
								}
								break;
							case RState.HEADERKEY:
								if (myReadBuffer[ndx] == '\r')
									ndx++;
								else if (myReadBuffer[ndx] == '\n')
								{
									ndx++;
									if (HTTPRequest.Headers["Content-Length"] != null)
									{
										HTTPRequest.BodySize = Convert.ToInt32(HTTPRequest.Headers["Content-Length"]);
										this.HTTPRequest.BodyData = new byte[this.HTTPRequest.BodySize];
										ParserState = RState.BODY;
									}
									else
										ParserState = RState.OK;

								}
								else if (myReadBuffer[ndx] == ':')
									ndx++;
								else if (myReadBuffer[ndx] != ' ')
									hKey += (char)myReadBuffer[ndx++];
								else
								{
									ndx++;
									hValue = "";
									ParserState = RState.HEADERVALUE;
								}
								break;
							case RState.HEADERVALUE:
								if (myReadBuffer[ndx] == '\r')
									ndx++;
								else if (myReadBuffer[ndx] != '\n')
									hValue += (char)myReadBuffer[ndx++];
								else
								{
									ndx++;
									if (!HTTPRequest.Headers.ContainsKey(hKey))
										HTTPRequest.Headers.Add(hKey, hValue);
									hKey = "";
									ParserState = RState.HEADERKEY;
								}
								break;
							case RState.BODY:
								// Append to request BodyData
								Array.Copy(myReadBuffer, ndx, this.HTTPRequest.BodyData, bfndx, numberOfBytesRead - ndx);
								bfndx += numberOfBytesRead - ndx;
								ndx = numberOfBytesRead;
								if (this.HTTPRequest.BodySize <= bfndx)
								{
									ParserState = RState.OK;
								}
								break;
							default:
								ndx++;
								break;
						}
					}
					while (ndx < numberOfBytesRead);

				}
				while (ns.DataAvailable);

#if DEBUG && debug_output
				// Print out the received message to the console.
				Debug.WriteLine("Media server received request: " + Environment.NewLine +
					myCompleteMessage);
#endif

				//Build up the HTTPResponse
				HTTPResponse.version = "HTTP/1.1";

				if (ParserState != RState.OK)
					HTTPResponse.status = (int)RespState.BAD_REQUEST;
				else
					HTTPResponse.status = (int)RespState.OK;

				this.HTTPResponse.Headers = new Hashtable();
				this.HTTPResponse.Headers.Add("Server", Parent.Name);
				this.HTTPResponse.Headers.Add("Date", DateTime.Now.ToString("r"));
				this.HTTPResponse.Headers.Add("Cache-Control", "no-cache");
				this.HTTPResponse.Headers.Add("Pragma", "no-cache");
				this.HTTPResponse.Headers.Add("Expires", "-1");

#if DEBUG && debug_output
				//Call the overriden SubClass Method to complete the HTTPResponse Content
				Debug.WriteLine("Preparing reponse.");
#endif
				this.Parent.OnResponse(ref this.HTTPRequest, ref this.HTTPResponse);

				//Create the Header String
				string HeadersString = this.HTTPResponse.version + " " + this.Parent.respStatus[this.HTTPResponse.status] + "\r\n";
				foreach (DictionaryEntry Header in this.HTTPResponse.Headers)
				{
					HeadersString += Header.Key + ": " + Header.Value + "\r\n";
				}

				HeadersString += "\r\n";
				byte[] bHeadersString = Encoding.ASCII.GetBytes(HeadersString);

#if DEBUG && debug_output
				// Send headers	
				Debug.WriteLine("Response headers: " + Environment.NewLine + HeadersString);
#endif
				ns.Write(bHeadersString, 0, bHeadersString.Length);
				ns.Flush();
				// Send body (File)
				if (this.HTTPResponse.mediaStream != null)
					using (this.HTTPResponse.mediaStream)
					{
						byte[] b = new byte[client.SendBufferSize];
						int bytesRead;
						int totalBytes = 0;
						int totalLength = Convert.ToInt32(this.HTTPResponse.mediaStream.Length);

						while ((bytesRead = this.HTTPResponse.mediaStream.Read(b, 0, b.Length)) > 0)
						{
							ns.Write(b, 0, bytesRead);
							totalBytes += bytesRead;
#if DEBUG && debug_output
							Debug.WriteLine(string.Format("Sent {0:0,0} / {1:0,0} KBytes ({2:0.0%}).",
								1.0 * totalBytes / 1024, 1.0 * totalLength / 1024, 1.0 * totalBytes / totalLength));
#endif
						}

						ns.Flush();
						this.HTTPResponse.mediaStream.Close();
					}
			}
			catch (Exception e)
			{
				if (e is WebException)
					Trace.WriteLine("A Webexception in CsHTTPRequest was thrown. URI: " + ((WebException)e).Response.ResponseUri.ToString() + Environment.NewLine + e.ToString());
				else
					Trace.WriteLine("An Exception in CsHTTPRequest was thrown: " + e.ToString());

				if (e.InnerException != null && e.InnerException is SocketException)
				{
					SocketException se = ((SocketException)e.InnerException);
					Trace.WriteLine("Socket exception: " + se.ToString());
					Trace.WriteLine("Error code: " + se.ErrorCode + " SocketErrorCode: " + se.SocketErrorCode);
				}


			}
			finally
			{
				ns.Close();
				client.Close();
				if (this.HTTPResponse.mediaStream != null)
					this.HTTPResponse.mediaStream.Close();


			}
		}

	}
}
