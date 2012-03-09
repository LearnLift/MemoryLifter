using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System.Web.Caching;

namespace MLifterSyncService
{
    /// <summary>
    /// Handles media file requests.
    /// </summary>
    public class MediaFileHandler : IHttpHandler, IRequiresSessionState
    {
        private const int defaultCacheTime = 60;
        private TimeSpan cacheTimeSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFileHandler"/> class.
        /// </summary>
        public MediaFileHandler()
        {
            int seconds;
            if (!int.TryParse(ConfigurationManager.AppSettings["MediaCacheTimeSpan"], out seconds))
                seconds = defaultCacheTime;
            cacheTimeSpan = new TimeSpan(0, 0, seconds);
        }

        /// <summary>
        /// Process the Request.
        /// </summary>
        /// <param name="context">The HTTP context for the request.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-05-24</remarks>
        public void ProcessRequest(HttpContext context)
        {
            HttpSessionState session = context.Session;
            int mediaId = Convert.ToInt32(context.Request.Params["id"]);

            if (mediaId < 0)
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

                CachedMedia cachedMedia;
                if ((cachedMedia = HttpContext.Current.Cache[mediaId.ToString()] as CachedMedia) == null)
                {
                    cachedMedia = new CachedMedia(mediaId, GetMediaMimeType(mediaId));
                    WriteMedia(mediaId, cachedMedia.Data);
                    HttpContext.Current.Cache.Insert(cachedMedia.Id.ToString(), cachedMedia, null, Cache.NoAbsoluteExpiration, cacheTimeSpan);
                }
                context.Response.StatusCode = 200;
                context.Response.ContentType = cachedMedia.MimeType;
                cachedMedia.Data.WriteTo(context.Response.OutputStream);
            }
        }

        /// <summary>
        /// Writes the media object to a stream.
        /// </summary>
        /// <param name="mediaId">The media id.</param>
        /// <param name="output">The output.</param>
        private void WriteMedia(int mediaId, Stream output)
        {
            using (NpgsqlConnection conn = FileHandlerHelpers.GetPgConnection())
            {
                int noid = 0;
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT data FROM \"MediaContent\" WHERE id=:id;";
                    cmd.Parameters.Add("id", mediaId);
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
        /// Gets the media MIME type.
        /// </summary>
        /// <param name="mediaId">The media id.</param>
        /// <returns></returns>
        private string GetMediaMimeType(int mediaId)
        {
            using (NpgsqlConnection con = FileHandlerHelpers.GetPgConnection())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT value FROM \"MediaProperties\" WHERE media_id=:id and property=:prop";
                    cmd.Parameters.Add("id", mediaId);
                    cmd.Parameters.Add("prop", MediaProperty.MimeType.ToString());
                    object result = cmd.ExecuteScalar();
                    return (result == null || result is DBNull) ? string.Empty : result.ToString();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Contains a cached media object.
        /// </summary>
        protected class CachedMedia
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id.</value>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the MIME type.
            /// </summary>
            /// <value>The type of the MIME.</value>
            public string MimeType { get; set; }
            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            /// <value>The data.</value>
            public MemoryStream Data { get; set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="CachedMedia"/> class.
            /// </summary>
            /// <param name="Id">The id.</param>
            /// <param name="MimeType">Type of the MIME.</param>
            public CachedMedia(int Id, string MimeType)
            {
                this.Id = Id;
                this.MimeType = MimeType;
                this.Data = new MemoryStream();
            }
        }
    }
}
