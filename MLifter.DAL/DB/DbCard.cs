using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Security;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// Database implementation of ICard.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-25</remarks>
    class DbCard : MLifter.DAL.Interfaces.ICard
    {
        private Interfaces.DB.IDbCardConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return MLifter.DAL.DB.PostgreSQL.PgSqlCardConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeCardConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private Interfaces.DB.IDbCardMediaConnector cardmediaconnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlCardMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        switch (parent.CurrentUser.ConnectionString.SyncType)
                        {
                            case SyncType.NotSynchronized:
                                return MsSqlCeCardMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithDbAccess:
                                return PgSqlCardMediaConnector.GetInstance(new ParentClass(parent.CurrentUser.ConnectionString.ServerUser, this));
                            case SyncType.FullSynchronized:
                                return MsSqlCeCardMediaConnector.GetInstance(parent);
                            case SyncType.HalfSynchronizedWithoutDbAccess:
                            default:
                                throw new NotAllowedInSyncedModeException();
                        }
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        protected Interfaces.DB.IDbCardMediaConnector cardmediaPropertiesConnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlCardMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeCardMediaConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private Interfaces.DB.IDbCardStyleConnector cardstyleconnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PostgreSQL.PgSqlCardStyleConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MsSqlCe.MsSqlCeCardStyleConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCard"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbCard(int id, ParentClass parentClass) : this(id, true, parentClass) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DbCard"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="checkId">if set to <c>true</c> [check id].</param>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DbCard(int id, bool checkId, ParentClass parentClass)
        {
            parent = parentClass;

            if (checkId)
                connector.CheckCardId(id);
            this.id = id;

            question = new DbWords(id, Side.Question, WordType.Word, Parent.GetChildParentClass(this));
            questionExample = new DbWords(id, Side.Question, WordType.Sentence, Parent.GetChildParentClass(this));
            questionDistractors = new DbWords(id, Side.Question, WordType.Distractor, Parent.GetChildParentClass(this));
            answer = new DbWords(id, Side.Answer, WordType.Word, Parent.GetChildParentClass(this));
            answerExample = new DbWords(id, Side.Answer, WordType.Sentence, Parent.GetChildParentClass(this));
            answerDistractors = new DbWords(id, Side.Answer, WordType.Distractor, Parent.GetChildParentClass(this));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public override string ToString()
        {
            return String.Format("{0} - {1} - {2}", Id, Question, Answer);
        }

        public override bool Equals(object card)
        {
            if (card as ICard == null)
                return false;

            return this.id == (card as ICard).Id;
        }

        public override int GetHashCode()
        {
            return this.id;
        }

        #region ICard Members

        /// <summary>
        /// Occurs when [create media progress changed].
        /// </summary>
        /// <remarks>Documented by Dev03, 2008-08-21</remarks>
        public event StatusMessageEventHandler CreateMediaProgressChanged;

        /// <summary>
        /// Gets the card.
        /// </summary>
        /// <value>The card.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public XmlElement Card
        {
            get
            {
                return Helper.GenerateXmlCard(this);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ICard"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public bool Active
        {
            get
            {
                return connector.GetActive(id);
            }
            set
            {
                connector.SetActive(id, value);
            }
        }

        private int id;
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Gets or sets the box.
        /// </summary>
        /// <value>The box.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Box
        {
            get
            {
                return connector.GetBox(id);
            }
            set
            {
                if (value < -1 || value > 10)
                    throw new InvalidBoxException(value);

                connector.SetBox(id, value == -1 ? 0 : value);
            }
        }

        /// <summary>
        /// Gets or sets the chapter.
        /// </summary>
        /// <value>The chapter.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int Chapter
        {
            get
            {
                return connector.GetChapter(id);
            }
            set
            {
                DbChapter chapter = new DbChapter(value, Parent);
                if (!chapter.HasPermission(PermissionTypes.CanModify))
                    throw new PermissionException();
                connector.SetChapter(id, value);
            }
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DateTime Timestamp
        {
            get
            {
                return connector.GetTimestamp(id);
            }
            set
            {
                connector.SetTimestamp(id, value);
            }
        }

        private IWords question;
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IWords Question
        {
            get
            {
                return question;
            }
            set
            {
                question = value;
            }
        }

        private IWords questionExample;
        /// <summary>
        /// Gets or sets the question example.
        /// </summary>
        /// <value>The question example.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IWords QuestionExample
        {
            get
            {
                return questionExample;
            }
            set
            {
                questionExample = value;
            }
        }

        /// <summary>
        /// Gets or sets the question stylesheet.
        /// </summary>
        /// <value>The question stylesheet.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string QuestionStylesheet
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return string.Empty;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets or sets the question media.
        /// </summary>
        /// <value>The question media.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IList<IMedia> QuestionMedia
        {
            get
            {
                IList<int> ids = cardmediaPropertiesConnector.GetCardMedia(Id, Side.Question);
                IList<IMedia> media = new List<IMedia>();

                foreach (int cardid in ids)
                    media.Add(cardmediaPropertiesConnector.GetMedia(cardid, Id));

                return media;
            }
        }

        private IWords questionDistractors;
        /// <summary>
        /// Gets or sets the question distractors.
        /// </summary>
        /// <value>The question distractors.</value>
        /// <remarks>Documented by Dev03, 2008-01-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IWords QuestionDistractors
        {
            get
            {
                return questionDistractors;
            }
            set
            {
                questionDistractors = value;
            }
        }

        private IWords answer;
        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>The answer.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IWords Answer
        {
            get
            {
                return answer;
            }
            set
            {
                answer = value;
            }
        }

        private IWords answerExample;
        /// <summary>
        /// Gets or sets the answer example.
        /// </summary>
        /// <value>The answer example.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IWords AnswerExample
        {
            get
            {
                return answerExample;
            }
            set
            {
                answerExample = value;
            }
        }

        /// <summary>
        /// Gets or sets the answer stylesheet.
        /// </summary>
        /// <value>The answer stylesheet.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public string AnswerStylesheet
        {
            get
            {
                Debug.WriteLine("The method or operation is not implemented.");
                return string.Empty;
            }
            set
            {
                Debug.WriteLine("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets or sets the answer media.
        /// </summary>
        /// <value>The answer media.</value>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IList<IMedia> AnswerMedia
        {
            get
            {
                IList<int> ids = cardmediaPropertiesConnector.GetCardMedia(Id, Side.Answer);
                IList<IMedia> media = new List<IMedia>();

                foreach (int cardid in ids)
                    media.Add(cardmediaPropertiesConnector.GetMedia(cardid, Id));

                return media;
            }
        }

        private IWords answerDistractors;
        /// <summary>
        /// Gets or sets the answer distractors.
        /// </summary>
        /// <value>The answer distractors.</value>
        /// <remarks>Documented by Dev03, 2008-01-07</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IWords AnswerDistractors
        {
            get
            {
                return answerDistractors;
            }
            set
            {
                answerDistractors = value;
            }
        }

        /// <summary>
        /// Adds the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="side">The side.</param>
        /// <returns>The media object.</returns>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2008-08-25</remarks>
        public IMedia AddMedia(IMedia media, Side side)
        {
            if (media == null)
                throw new NullReferenceException("Media object must not be null!");

            //[ML-1508] Can not add Media (images) to a LM: 
            if (media.Parent == null || media.Parent.CurrentUser.ConnectionString.ConnectionString != parent.CurrentUser.ConnectionString.ConnectionString)
                media = CreateMedia(media.MediaType, media.Filename, media.Active.Value, media.Default.Value, media.Example.Value);
            try
            {
                if (media.MediaType == EMedia.Audio)
                {
                    cardmediaconnector.SetCardMedia(media.Id, Id, side,
                        media.Example.GetValueOrDefault() ? WordType.Sentence : WordType.Word, media.Default.GetValueOrDefault(), media.MediaType);
                }
                else
                {
                    cardmediaconnector.SetCardMedia(media.Id, Id, side, WordType.Word, false, media.MediaType);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "DbCard.AddMedia() throws an exception.");
                return null;
            }
            return media;
        }

        /// <summary>
        /// Removes the media.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void RemoveMedia(IMedia media)
        {
            cardmediaconnector.ClearCardMedia(Id, media.Id);
        }

        /// <summary>
        /// Creates the media.
        /// </summary>
        /// <param name="type">The type of the media file.</param>
        /// <param name="path">The path to the media file.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="isExample">if set to <c>true</c> [is example].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public IMedia CreateMedia(EMedia type, string path, bool isActive, bool isDefault, bool isExample)
        {
            StatusMessageReportProgress rpu = new StatusMessageReportProgress(ReportProgressUpdate);
            return (parent.GetParentDictionary() as DbDictionary).CreateNewMediaObject(this, rpu, type, path, isActive, isDefault, isExample);
        }

		/// <summary>
		/// Sends the status message update.
		/// </summary>
		/// <param name="args">The <see cref="MLifter.DAL.Tools.StatusMessageEventArgs"/> instance containing the event data.</param>
		/// <param name="caller">The caller.</param>
		/// <returns>
		/// [true] if the process should be canceled.
		/// </returns>
		/// <remarks>
		/// Documented by AAB, 20.8.2008.
		/// </remarks>
        private bool ReportProgressUpdate(StatusMessageEventArgs args, object caller)
        {
            switch (args.MessageType)
            {
                case StatusMessageType.CreateMediaProgress:
                    if ((caller != null) && (caller is DbCard) && ((caller as DbCard).CreateMediaProgressChanged != null))
                        (caller as DbCard).CreateMediaProgressChanged(null, args);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Clears all media.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void ClearAllMedia()
        {
            ClearAllMedia(false);
        }

        /// <summary>
        /// Clears all media.
        /// </summary>
        /// <param name="removeFiles">if set to <c>true</c> [remove files].</param>
        /// <remarks>Documented by Dev03, 2007-09-03</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void ClearAllMedia(bool removeFiles)
        {
            cardmediaconnector.ClearCardMedia(Id);
        }

        /// <summary>
        /// Creates and returns a card style.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ICardStyle CreateCardStyle()
        {
            if (Settings == null)
                Settings = (parent.GetParentDictionary() as DbDictionary).CreateSettingsObject();
            return new DbCardStyle((Settings.Style as DbCardStyle).Id, parent);
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <remarks>Documented by Dev05, 2008-08-19</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ISettings Settings
        {
            get
            {
                return connector.GetSettings(id);
            }
            set
            {
                connector.SetSettings(id, value);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void Dispose()
        {
            Debug.WriteLine("The method or operation is not implemented.");
        }

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
            CopyBase.Copy(this, target, typeof(ICard), progressDelegate);

            //copy media objects
            ICard targetCard = target as ICard;
            if (targetCard != null)
            {
                foreach (IMedia media in QuestionMedia)
                    targetCard.AddMedia(media, Side.Question);
                foreach (IMedia media in AnswerMedia)
                    targetCard.AddMedia(media, Side.Answer);
                try
                {
                    if (targetCard is MLifter.DAL.XML.XmlCard)
                        (targetCard as MLifter.DAL.XML.XmlCard).Id = Id;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Tried to set the card id for XML but failed: " + ex.ToString());
                }
            }
        }

        #endregion

        #region IParent Members

        private ParentClass parent;
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ParentClass Parent { get { return parent; } }

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