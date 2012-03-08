using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MLifter.RegistrationService
{
	/// <summary>
	/// Summary description for RegistrationHandler
	/// </summary>
	public class RegistrationHandler : IHttpHandler
	{
		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		/// <remarks>CFI, 2012-02-09</remarks>
		public void ProcessRequest(HttpContext context)
		{
			string logPath = HttpContext.Current.Server.MapPath("~/App_Data/registrations.txt");
			string logText = String.Empty;

			for (int i = 0; i < HttpContext.Current.Request.Form.Count; ++i)
				logText += HttpContext.Current.Request.Form.GetKey(i) + "=" + HttpContext.Current.Request.Form[i] + ";";

			File.AppendAllText(logPath, logText + Environment.NewLine);
			context.Response.StatusCode = 200;
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		/// <remarks>CFI, 2012-02-09</remarks>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}