/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
