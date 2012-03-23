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
using System.Web;
using System.Web.SessionState;
using System.IO;
using Npgsql;
using NpgsqlTypes;

namespace MLifterSyncService
{
    /// <summary>
    /// Handles extension file requests.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-07-06</remarks>
    public class ExtensionFileHandler : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionFileHandler"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public ExtensionFileHandler()
        { }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public void ProcessRequest(HttpContext context)
        {
            HttpSessionState session = context.Session;
            Guid extensionId = Guid.Empty;
            try
            {
                extensionId = new Guid(context.Request.Params["guid"]);
            }
            catch (Exception)
            { }

            if (extensionId == Guid.Empty)
            {
                int uid = FileHandlerHelpers.Login(context.Request.Params["user"], context.Request.Params["password"]);
                context.Response.StatusCode = 200;
                context.Response.Write(uid < 0 ? uid.ToString() : "TRUE");
                session["uid"] = uid;
            }
            else
            {
                object uid = session["uid"];

                if (uid == null || (int)uid < 0)
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                MemoryStream extensionData = new MemoryStream();
                WriteExtensionData(extensionId, extensionData);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/bin";
                extensionData.WriteTo(context.Response.OutputStream);
            }
        }

        /// <summary>
        /// Writes the extension data.
        /// </summary>
        /// <param name="extensionId">The extension id.</param>
        /// <param name="output">The output.</param>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        private void WriteExtensionData(Guid extensionId, Stream output)
        {
            using (NpgsqlConnection conn = FileHandlerHelpers.GetPgConnection())
            {
                int noid = 0;
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM \"Extensions\" WHERE guid=:guid;";
                    cmd.Parameters.Add("guid", extensionId.ToString());
                    noid = Convert.ToInt32(cmd.ExecuteScalar());
                }

                NpgsqlTransaction tran = conn.BeginTransaction();
                LargeObjectManager lbm = new LargeObjectManager(conn);
                LargeObject largeObject = lbm.Open(noid, LargeObjectManager.READWRITE);
                largeObject.Seek(0);
                int size = largeObject.Size();
                byte[] buffer = new byte[size];
                int read = 0;
                int offset = 0;
                while (offset < size)
                {
                    read = largeObject.Read(buffer, offset, Math.Min(102400, size - offset));
                    output.Write(buffer, offset, read);
                    offset += 102400;
                }
                largeObject.Close();
                tran.Commit();
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
