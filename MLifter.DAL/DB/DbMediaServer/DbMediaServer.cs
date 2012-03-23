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
//# define debug_output

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using System.Net;
using MLifter.Generics;

namespace MLifter.DAL.DB.DbMediaServer
{
    /// <summary>
    /// WebServer for delifering media files.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-02-09</remarks>
    public class DbMediaServer : CsHTTPServer
    {
        private static Dictionary<ConnectionStringStruct, DbMediaServer> instances = new Dictionary<ConnectionStringStruct, DbMediaServer>();
        /// <summary>
        /// Instance with the specified parent class.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-09</remarks>
        public static DbMediaServer Instance(ParentClass parentClass)
        {
            return Instance(0, parentClass);
        }
        /// <summary>
        /// Instance which the specified media URI belongs to.
        /// </summary>
        /// <param name="mediaUri">The media URI.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-09</remarks>
        public static DbMediaServer Instance(Uri mediaUri)
        {
            foreach (DbMediaServer srv in instances.Values)
                if (srv.ServerPort == mediaUri.Port)
                    return srv;

            return null;
        }
        /// <summary>
        /// Instance with the specified port number.
        /// </summary>
        /// <param name="PortNumber">The port number.</param>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-09</remarks>
        public static DbMediaServer Instance(int PortNumber, ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new DbMediaServer(PortNumber));
                instances[connection].parent = parentClass;

                return instances[connection];
            }
        }

        private static Dictionary<Side, Dictionary<int, string>> contentQueue = new Dictionary<Side, Dictionary<int, string>>();
        private static Dictionary<Side, string> content = new Dictionary<Side, string>();

        /// <summary>
        /// Enqueues the answer.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <param name="answer">The answer.</param>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        private static void EnqueueAnswer(int cardId, string answer)
        {
            lock (contentQueue)
            {
                if (!contentQueue.ContainsKey(Side.Answer))
                    contentQueue[Side.Answer] = new Dictionary<int, string>();
                contentQueue[Side.Answer][cardId] = answer;
            }
        }

        /// <summary>
        /// Enqueues the question.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <param name="question">The question.</param>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        private static void EnqueueQuestion(int cardId, string question)
        {
            lock (contentQueue)
            {
                if (!contentQueue.ContainsKey(Side.Question))
                    contentQueue[Side.Question] = new Dictionary<int, string>();
                contentQueue[Side.Question][cardId] = question;
            }
        }

        /// <summary>
        /// Dequeues the answer.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        private static string DequeueAnswer(int cardId)
        {
            string answer = string.Empty;
            lock (contentQueue)
            {
                if (contentQueue.ContainsKey(Side.Answer) && contentQueue[Side.Answer].ContainsKey(cardId))
                {
                    answer = contentQueue[Side.Answer][cardId];
                    contentQueue[Side.Answer].Remove(cardId);
                }
            }
            return answer;
        }

        /// <summary>
        /// Dequeues the question.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        private static string DequeueQuestion(int cardId)
        {
            string question = string.Empty;
            lock (contentQueue)
            {
                if (contentQueue.ContainsKey(Side.Question) && contentQueue[Side.Question].ContainsKey(cardId))
                {
                    question = contentQueue[Side.Question][cardId];
                    contentQueue[Side.Question].Remove(cardId);
                }
            }
            return question;
        }

        /// <summary>
        /// Prepares the answer and enqueues it.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="cardId">The card id.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        public static Uri PrepareAnswer(ParentClass parent, int cardId, string content)
        {
            EnqueueAnswer(cardId, content);
            return DbMediaServer.Instance(parent).GetAnswerUri(cardId);
        }

        /// <summary>
        /// Prepares the question and enqueues.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="cardId">The card id.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        public static Uri PrepareQuestion(ParentClass parent, int cardId, string content)
        {
            EnqueueQuestion(cardId, content);
            return DbMediaServer.Instance(parent).GetQuestionUri(cardId);
        }

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev05, 2009-02-09</remarks>
        public ParentClass Parent { get { return parent; } }

        private IDbMediaConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeMediaConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private IDbMediaConnector direct_connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        switch (parent.CurrentUser.ConnectionString.SyncType)
                        {
                            case SyncType.NotSynchronized:
                                return MsSqlCeMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithDbAccess:
                                return PgSqlMediaConnector.GetInstance(new ParentClass(parent.CurrentUser.ConnectionString.ServerUser, parent.ParentObject));
                            case SyncType.FullSynchronized:
                                return MsSqlCeMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithoutDbAccess:
                            default:
                                throw new NotAllowedInSyncedModeException();
                        }
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private IDbExtensionConnector extensionconnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlExtensionConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeExtensionConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private IDbExtensionConnector direct_extensionconnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlExtensionConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        switch (parent.CurrentUser.ConnectionString.SyncType)
                        {
                            case SyncType.NotSynchronized:
                                return MsSqlCeExtensionConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithDbAccess:
                                return PgSqlExtensionConnector.GetInstance(new ParentClass(parent.CurrentUser.ConnectionString.ServerUser, parent.ParentObject));
                            case SyncType.FullSynchronized:
                                return MsSqlCeExtensionConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithoutDbAccess:
                            default:
                                throw new NotAllowedInSyncedModeException();
                        }
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }


        private CookieAwareWebClient webClient = new CookieAwareWebClient();

        private DbMediaServer()
            : base()
        {
        }

        private DbMediaServer(int portNumber)
            : base(portNumber)
        {
        }

        /// <summary>
        /// To be overwritten by the corresponding subclass.
        /// </summary>
        /// <param name="rq">The rq.</param>
        /// <param name="rp">The rp.</param>
        /// <remarks>Documented by Dev10, 2008-08-07</remarks>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public override void OnResponse(ref HTTPRequestStruct rq, ref HTTPResponseStruct rp)
        {
            Stream responseStream;
            int cardId;
            string ContentTypeString;
            Side side;
            Guid extensionGuid;
            if (IsContent(rq.URL, out cardId, out side, out ContentTypeString))
            {
                string content = (side == Side.Question) ? DequeueQuestion(cardId) : DequeueAnswer(cardId);
                responseStream = new MemoryStream(Encoding.Unicode.GetBytes(content));

                rp.mediaStream = responseStream;
                rp.Headers["Content-Type"] = ContentTypeString + "; charset=UTF-16";
                rp.Headers["Content-Length"] = responseStream.Length;
            }
            else if (IsExtension(rq.URL, out extensionGuid))
            {
                if (extensionconnector.IsStreamAvailable(extensionGuid))
                    responseStream = extensionconnector.GetExtensionStream(extensionGuid);
                else if (parent.CurrentUser.ConnectionString.SyncType == SyncType.HalfSynchronizedWithDbAccess)
                    responseStream = direct_extensionconnector.GetExtensionStream(extensionGuid);
                else if (parent.CurrentUser.ConnectionString.SyncType == SyncType.HalfSynchronizedWithoutDbAccess || parent.CurrentUser.ConnectionString.SyncType == SyncType.FullSynchronized)
                {
                    lock (webClient)
                    {
                        try
                        {
                            responseStream = new CachingStream(webClient.DownloadData(new Uri(string.Format(parent.CurrentUser.ConnectionString.ExtensionURI, extensionGuid))), extensionGuid, extensionconnector);
                        }
                        catch (WebException exp)
                        {
                            if ((exp.Response as HttpWebResponse).StatusCode != HttpStatusCode.Forbidden)
                                throw exp;

                            string result = webClient.DownloadString(new Uri(string.Format(parent.CurrentUser.ConnectionString.ExtensionURI + "&user={1}&password={2}", -1,
                                parent.CurrentUser.ConnectionString.ServerUser.UserName, parent.CurrentUser.ConnectionString.ServerUser.Password)));

                            if (result != "TRUE")
                                throw new NoValidUserException();

                            responseStream = new CachingStream(webClient.DownloadData(new Uri(string.Format(parent.CurrentUser.ConnectionString.ExtensionURI, extensionGuid))), extensionGuid, extensionconnector);
                        }
                    }
                }
                else
                    throw new ArgumentException();

                rp.mediaStream = responseStream;
                rp.Headers["Content-Type"] = "application/bin";
                rp.Headers["Content-Length"] = responseStream.Length;
            }
            else
            {
                int mediaId = GetMediaID(rq.URL);

                if (connector.IsMediaAvailable(mediaId))
                    responseStream = connector.GetMediaStream(mediaId, null);
                else if (parent.CurrentUser.ConnectionString.SyncType == SyncType.HalfSynchronizedWithDbAccess)
                    responseStream = direct_connector.GetMediaStream(mediaId, connector);
                else if (parent.CurrentUser.ConnectionString.SyncType == SyncType.HalfSynchronizedWithoutDbAccess || parent.CurrentUser.ConnectionString.SyncType == SyncType.FullSynchronized)
                {
                    lock (webClient)
                    {
                        try
                        {
                            responseStream = new CachingStream(webClient.DownloadData(new Uri(string.Format(parent.CurrentUser.ConnectionString.LearningModuleFolder, mediaId))), mediaId, connector);
                        }
                        catch (WebException exp)
                        {
                            if ((exp.Response as HttpWebResponse).StatusCode != HttpStatusCode.Forbidden)
                                throw exp;

                            string result = webClient.DownloadString(new Uri(string.Format(parent.CurrentUser.ConnectionString.LearningModuleFolder + "&user={1}&password={2}", -1,
                                parent.CurrentUser.ConnectionString.ServerUser.UserName, parent.CurrentUser.ConnectionString.ServerUser.Password)));

                            if (result != "TRUE")
                                throw new NoValidUserException();

                            responseStream = new CachingStream(webClient.DownloadData(new Uri(string.Format(parent.CurrentUser.ConnectionString.LearningModuleFolder, mediaId))), mediaId, connector);
                        }
                    }
                }
                else
                    throw new ArgumentException();

                rp.mediaStream = responseStream;

                ContentTypeString = connector.GetPropertyValue(mediaId, MediaProperty.MimeType);
                if (ContentTypeString == null)
                {
                    Trace.WriteLine("MimeType delivered null, obviously the corresponding DB entry is missing...");
                    ContentTypeString = Helper.UnknownMimeType;
                }
                rp.Headers["Content-Type"] = ContentTypeString;
                rp.Headers["Content-Length"] = responseStream.Length;
            }
        }

        /// <summary>
        /// Gets the media ID.
        /// </summary>
        /// <param name="mediaURI">The media URI.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-25</remarks>
        public static int GetMediaID(string mediaURI)
        {
            string dummyExtension = string.Empty;
            return GetMediaID(mediaURI, ref dummyExtension);
        }

        /// <summary>
        /// Determines whether the specified media URI is from this server.
        /// </summary>
        /// <param name="mediaURI">The media URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified media URI is yours; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public bool IsYours(Uri mediaURI)
        {
            return mediaURI.Port == ServerPort;
        }

        private static Regex contentRegex = new Regex(@"\/content\/(?'side'question|answer)\/(?'id'-?\d+)(?'ext'\.[a-zA-Z0-9]+)?$", RegexOptions.Compiled);
        /// <summary>
        /// Determines whether the specified URI is content.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cardId">The card id.</param>
        /// <param name="side">The side.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URI is content; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        private bool IsContent(string uri, out int cardId, out Side side, out string contentType)
        {
            //Get the mediaId out of the Url
            Match contentMatch = contentRegex.Match(uri);

            cardId = 0;
            side = Side.Question;
            contentType = Helper.UnknownMimeType;

            if (!contentMatch.Success)
                return false;

            try { cardId = Convert.ToInt32(contentMatch.Groups["id"].Value); }
            catch { }
            if (contentMatch.Groups["side"].Success)
                try { side = (Side)Enum.Parse(typeof(Side), contentMatch.Groups["side"].Value, true); }
                catch { };
            contentType = "text/html";

            return true;
        }

        private static Regex extensionRegex = new Regex(@"\/extension\/(?'id'[a-zA-Z0-9\-]+)?$", RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified URI is extension.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="guid">The GUID.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URI is extension; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        private bool IsExtension(string uri, out Guid guid)
        {
            guid = Guid.Empty;

            Match extensionMatch = extensionRegex.Match(uri);

            if (!extensionMatch.Success)
                return false;

            try { guid = new Guid(extensionMatch.Groups["id"].Value); }
            catch { }

            return true;
        }

        private static Regex mediaRegex = new Regex(@"\/(?'id'\d+)(?'ext'\.[a-zA-Z0-9]+)?$", RegexOptions.Compiled);
        /// <summary>
        /// Gets the media ID.
        /// </summary>
        /// <param name="mediaURI">The media URI.</param>
        /// <param name="mediaExtension">The media extension.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-25</remarks>
        internal static int GetMediaID(string mediaURI, ref string mediaExtension)
        {
            //Get the mediaId out of the Url
            Match mediaIdMatch = mediaRegex.Match(mediaURI);

            if (!mediaIdMatch.Success)
                return 0;

            int mediaId = Convert.ToInt32(mediaIdMatch.Groups["id"].Value);
            if (mediaIdMatch.Groups["ext"].Success)
                mediaExtension = mediaIdMatch.Groups["ext"].Value;

            return mediaId;
        }

        /// <summary>
        /// Gets the media URI.
        /// </summary>
        /// <param name="mediaID">The media ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-09-25</remarks>
        public Uri GetMediaURI(int mediaID)
        {
            //Prevent getting Port 0
            if (!IsAlive) Start();
            while (!IsReady) System.Threading.Thread.Sleep(10);

            UriBuilder serverUri = new UriBuilder();
            serverUri.Host = hostname;
            serverUri.Port = usedPortNum;
            serverUri.Path = "/media/" + mediaID + connector.GetPropertyValue(mediaID, MediaProperty.Extension) ?? string.Empty;
#if DEBUG && debug_output
            Debug.WriteLine("Media URI: " + serverUri.ToString());
#endif
            return serverUri.Uri;
        }

        /// <summary>
        /// Gets the extension URI.
        /// </summary>
        /// <param name="extensionID">The extension ID.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public Uri GetExtensionURI(Guid extensionID)
        {
            //Prevent getting Port 0
            if (!IsAlive) Start();
            while (!IsReady) System.Threading.Thread.Sleep(10);

            UriBuilder serverUri = new UriBuilder();
            serverUri.Host = hostname;
            serverUri.Port = usedPortNum;
            serverUri.Path = "/extension/" + extensionID.ToString();
#if DEBUG && debug_output
            Debug.WriteLine("Extension URI: " + serverUri.ToString());
#endif
            return serverUri.Uri;
        }

        /// <summary>
        /// Gets the question URI.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        public Uri GetQuestionUri(int cardId)
        {
            return GetContentURI(cardId, Side.Question);
        }

        /// <summary>
        /// Gets the answer URI.
        /// </summary>
        /// <param name="cardId">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-04-27</remarks>
        public Uri GetAnswerUri(int cardId)
        {
            return GetContentURI(cardId, Side.Answer);
        }

		/// <summary>
		/// Gets the media URI.
		/// </summary>
		/// <param name="cardId">The card id.</param>
		/// <param name="side">The side.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by DAC, 2008-09-25.
		/// </remarks>
        public Uri GetContentURI(int cardId, Side side)
        {
            //Prevent getting Port 0
            if (!IsAlive) Start();
            while (!IsReady) System.Threading.Thread.Sleep(10);

            UriBuilder serverUri = new UriBuilder();
            serverUri.Host = hostname;
            serverUri.Port = usedPortNum;
            serverUri.Path = "/" + DateTime.Now.Ticks.ToString() + "/content/" + side.ToString().ToLower() + "/" + cardId + ".html";
#if DEBUG && debug_output
            Debug.WriteLine("Media URI: " + serverUri.ToString());
#endif
            return serverUri.Uri;
        }
    }
}
