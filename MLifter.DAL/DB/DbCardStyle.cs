using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using MLifter.DAL.XML;
using System.Text.RegularExpressions;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Database implementation of ICardStyle.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    class DbCardStyle : ICardStyle
    {
        private IDbCardStyleConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlCardStyleConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeCardStyleConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private IDbMediaConnector mediaConnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeMediaConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private static XmlSerializer styleSerializer = new XmlSerializer(typeof(XmlCardStyle));
        private XmlCardStyle xmlStyle;

        private static Regex cssUrlEntry = new Regex(@"url\((.+)\)");
        private static Regex replaceMediaIds = new Regex(@"MEDIA\[(\d+)\]", RegexOptions.Multiline);
        private static Regex extractSingleMediaId = new Regex(@"^MEDIA\[(\d+)\]$");
        private static string mediaPlaceHolder = "url(MEDIA[{0}])";

        private int id;
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCardStyle"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbCardStyle(int id, ParentClass parentClass) : this(id, true, parentClass) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DbCardStyle"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="checkId">if set to <c>true</c> [check id].</param>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbCardStyle(int id, bool checkId, ParentClass parentClass)
        {
            parent = parentClass;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
            String XmlValue = connector.GetCardStyle(id);

            if ((XmlValue != null) && (XmlValue.Length > 0))
            {
                try
                {
                    xmlStyle = (XmlCardStyle)styleSerializer.Deserialize(new StringReader(XmlValue));
                }
                catch
                {
                    Debug.WriteLine("Failed to deserialize card style!");
                    xmlStyle = new XmlCardStyle(parentClass);
                }
            }
            else
            {
                xmlStyle = new XmlCardStyle(parentClass);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see> as a CSS-Style.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"> as a CSS-Style</see>.
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        public override string ToString()
        {
            string cssString = xmlStyle.Question.ToString();
            cssString += xmlStyle.QuestionExample.ToString();
            cssString += xmlStyle.Answer.ToString();
            cssString += xmlStyle.AnswerExample.ToString();
            cssString += xmlStyle.AnswerCorrect.ToString();
            cssString += xmlStyle.AnswerWrong.ToString();

            return ReplaceMediaIds(cssString);
        }

        /// <summary>
        /// Returns the CSS for this style using the given base path for media URIs.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public string ToString(string basePath)
        {
            basePath = null;
            string cssString = xmlStyle.Question.ToString(basePath);
            cssString += xmlStyle.QuestionExample.ToString(basePath);
            cssString += xmlStyle.Answer.ToString(basePath);
            cssString += xmlStyle.AnswerExample.ToString(basePath);
            cssString += xmlStyle.AnswerCorrect.ToString(basePath);
            cssString += xmlStyle.AnswerWrong.ToString(basePath);

            return ReplaceMediaIds(cssString);
        }

        private string ReplaceMediaIds(string css)
        {
            string correctCss = replaceMediaIds.Replace(css, delegate(Match match)
            {
                return DbMediaServer.DbMediaServer.Instance(parent).GetMediaURI(Convert.ToInt32(match.Groups[1].Value)).AbsoluteUri;
            });
            return correctCss;
        }

        /// <summary>
        /// Flushes to DB.
        /// </summary>
        /// <remarks>Documented by Dev05, 2008-09-29</remarks>
        internal void FlushToDB()
        {
            SaveMediaToDb();
            StringWriter writer = new StringWriter();
            styleSerializer.Serialize(writer, xmlStyle);
            connector.SetCardStyle(id, writer.ToString());
        }

        /// <summary>
        /// Scans the css and remaps and saves any media to the database.
        /// </summary>
        /// <remarks>Documented by Dev09, 2009-03-10</remarks>
        private void SaveMediaToDb()
        {
            List<int> mediaIds = connector.GetMediaForCardStyle(Id);
            connector.ClearMediaForCardStyle(Id);

            // find url's in styles
            // regex finds "url(...)" and extracts (or replaces) ...
            ITextStyle[] styles = { xmlStyle.Question, xmlStyle.QuestionExample, xmlStyle.Answer, xmlStyle.AnswerExample, xmlStyle.AnswerCorrect, xmlStyle.AnswerWrong };
            foreach (ITextStyle style in styles)
            {
                string[] keys = new String[style.OtherElements.Keys.Count];
                style.OtherElements.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    String value = style.OtherElements[key];
                    Match urlValue = cssUrlEntry.Match(value);
                    if (urlValue.Success)
                    {
                        int id = -1;
                        String filename = urlValue.Groups[1].ToString();
                        Match extractId = extractSingleMediaId.Match(filename);
                        if (extractId.Success)
                        {
                            id = Convert.ToInt32(extractId.Groups[1].ToString());

                            if (mediaIds.Contains(id))
                            {
                                // if style holds id, check that it exists in database, and reconnect with style
                                connector.AddMediaForCardStyle(this.Id, id);
                                mediaIds.Remove(id);
                            }
                        }
                        else
                        {
                            Uri uri = new Uri(filename, UriKind.RelativeOrAbsolute);
                            if (!uri.IsAbsoluteUri)
                                uri = new Uri(new Uri(Environment.CurrentDirectory), filename);
                            if (File.Exists(Uri.UnescapeDataString(uri.LocalPath)))
                            {
                                // if style holds filename, add media to database and update style
                                IMedia media = Parent.GetParentDictionary().CreateMedia(EMedia.Image, Uri.UnescapeDataString(uri.LocalPath), false, false, false);
                                style.OtherElements[key] = cssUrlEntry.Replace(style.OtherElements[key], String.Format(mediaPlaceHolder, media.Id));

                                // connect media with style
                                connector.AddMediaForCardStyle(this.Id, media.Id);
                            }
                        }
                    }
                }
            }

            // remove orphaned media objects in mediaIds
            foreach (int orphanId in mediaIds)
            {
                mediaConnector.DeleteMedia(orphanId);
            }
        }

        #region ICardStyle Members

        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <value>The XML.</value>
        /// <remarks>Documented by Dev11, 2008-08-27</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string Xml
        {
            get
            {
                return xmlStyle.Xml;
            }
        }

        /// <summary>
        /// Gets or sets the question style.
        /// </summary>
        /// <value>The question.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ITextStyle Question
        {
            get
            {
                return xmlStyle.Question;
            }
            set
            {
                xmlStyle.Question = value;
                connector.SetCardStyle(id, Xml);
            }
        }
        /// <summary>
        /// Gets or sets the question example style.
        /// </summary>
        /// <value>The question example.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ITextStyle QuestionExample
        {
            get
            {
                return xmlStyle.QuestionExample;
            }
            set
            {
                xmlStyle.QuestionExample = value;
                connector.SetCardStyle(id, Xml);
            }
        }
        /// <summary>
        /// Gets or sets the answer style.
        /// </summary>
        /// <value>The answer.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ITextStyle Answer
        {
            get
            {
                return xmlStyle.Answer;
            }
            set
            {
                xmlStyle.Answer = value;
                connector.SetCardStyle(id, Xml);
            }
        }
        /// <summary>
        /// Gets or sets the answer example style.
        /// </summary>
        /// <value>The answer example.</value>
        /// <remarks>Documented by Dev05, 2007-10-29</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ITextStyle AnswerExample
        {
            get
            {
                return xmlStyle.AnswerExample;
            }
            set
            {
                xmlStyle.AnswerExample = value;
                connector.SetCardStyle(id, Xml);
            }
        }
        /// <summary>
        /// Gets or sets the answer correct style.
        /// </summary>
        /// <value>The answer correct.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ITextStyle AnswerCorrect
        {
            get
            {
                return xmlStyle.AnswerCorrect;
            }
            set
            {
                xmlStyle.AnswerCorrect = value;
                connector.SetCardStyle(id, Xml);
            }
        }
        /// <summary>
        /// Gets or sets the answer wrong style.
        /// </summary>
        /// <value>The answer wrong.</value>
        /// <remarks>Documented by Dev05, 2007-10-30</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ITextStyle AnswerWrong
        {
            get
            {
                return xmlStyle.AnswerWrong;
            }
            set
            {
                xmlStyle.AnswerWrong = value;
                connector.SetCardStyle(id, Xml);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2007-11-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ICardStyle Clone()
        {
            XmlSerializer ser = new XmlSerializer(typeof(XmlCardStyle));
            MemoryStream stream = new MemoryStream();
            ser.Serialize(stream, xmlStyle);
            stream.Seek(0, SeekOrigin.Begin);
            return ser.Deserialize(stream) as ICardStyle;
        }
        #endregion

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public MLifter.DAL.Tools.ParentClass Parent { get { return parent; } }

        #endregion

        #region ICopy Members

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void CopyTo(ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(ICardStyle), progressDelegate);
            if (target is DbCardStyle) CopyMediaTo((DbCardStyle)target);
            if (target is DbCardStyle) ((DbCardStyle)target).FlushToDB();
        }

        #endregion

        #region Media Object Copying
        /// <summary>
        /// Copies the media to.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <remarks>Documented by Dev02, 2009-06-25</remarks>
        private void CopyMediaTo(DbCardStyle target)
        {
            //mapping dictionary current id, new id
            Dictionary<int, int> mediaMapping = new Dictionary<int, int>();

            //copy media objects
            foreach (int mediaId in connector.GetMediaForCardStyle(Id))
            {
                int newMediaId;

                //fetch current media data
                using (Stream mediaStream = mediaConnector.GetMediaStream(mediaId))
                {
                    EMedia mediaType = mediaConnector.GetMediaType(mediaId);
                    Dictionary<MediaProperty, string> mediaProperties = mediaConnector.GetProperties(mediaId);

                    //copy to new
                    newMediaId = target.mediaConnector.CreateMedia(mediaStream, mediaType, delegate { return false; }, null);
                    target.mediaConnector.SetProperties(newMediaId, mediaProperties);
                }

                //add
                target.connector.AddMediaForCardStyle(target.Id, newMediaId);
                mediaMapping[mediaId] = newMediaId;
            }

            //translate ids
            ITextStyle[] styles = { target.xmlStyle.Question, target.xmlStyle.QuestionExample, 
                                      target.xmlStyle.Answer, target.xmlStyle.AnswerExample, target.xmlStyle.AnswerCorrect, target.xmlStyle.AnswerWrong };
            foreach (ITextStyle style in styles)
            {
                string[] keys = new String[style.OtherElements.Keys.Count];
                style.OtherElements.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    style.OtherElements[key] = cssUrlEntry.Replace(style.OtherElements[key], delegate(Match urlMatch)
                    {
                        return replaceMediaIds.Replace(urlMatch.Groups[1].Value, delegate(Match mediaIdMatch)
                        {
                            int id = Convert.ToInt32(mediaIdMatch.Groups[1].Value);
                            if (mediaMapping.ContainsKey(id))
                                return string.Format(mediaPlaceHolder, mediaMapping[id]);
                            return urlMatch.Groups[0].Value;
                        });
                    });
                }
            }
        }
        #endregion

        #region ISecurity Members

        /// <summary>
        /// Determines whether the object has the specified permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns>
        /// 	<c>true</c> if the object name has the specified permission; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public bool HasPermission(string permissionName)
        {
            return Parent.CurrentUser.HasPermission(this, permissionName);
        }

        /// <summary>
        /// Gets the permissions for the object.
        /// </summary>
        /// <returns>A list of permissions for the object.</returns>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public List<SecurityFramework.PermissionInfo> GetPermissions()
        {
            return Parent.CurrentUser.GetPermissions(this);
        }

        #endregion
    }
}
