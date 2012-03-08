using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using MLifter.Generics;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// DB implementation of ISettings.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-15</remarks>
    class DbSettings : ISettings
    {
        private IDbSettingsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlSettingsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeSettingsConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSettings"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public DbSettings(int id, ParentClass parent)
            : this(id, true, parent) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DbSettings"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="checkId">if set to <c>true</c> [check id].</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public DbSettings(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckSettingsId(id);

            this.id = id;
        }

        private int id;
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2009-01-15</remarks>
        public int Id
        {
            get { return id; }
        }

        #region ISettings Members

        public IQueryDirections QueryDirections
        {
            get
            {
                return connector.GetQueryDirections(id);
            }
            set
            {
                connector.SetQueryDirections(id, value);
            }
        }

        public IQueryType QueryTypes
        {
            get
            {
                return connector.GetQueryType(id);
            }
            set
            {
                connector.SetQueryType(id, value);
            }
        }

        public IQueryMultipleChoiceOptions MultipleChoiceOptions
        {
            get
            {
                return connector.GetMultipleChoiceOptions(id);
            }
            set
            {
                connector.SetMultipleChoiceOptions(id, value);
            }
        }

        public IGradeTyping GradeTyping
        {
            get
            {
                return connector.GetGradeTyping(id);
            }
            set
            {
                connector.SetGradeTyping(id, value);
            }
        }

        public IGradeSynonyms GradeSynonyms
        {
            get
            {
                return connector.GetGradeSynonyms(id);
            }
            set
            {
                connector.SetGradeSynonyms(id, value);
            }
        }

        public bool? AutoplayAudio
        {
            get
            {
                return connector.GetAutoplayAudio(id);
            }
            set
            {
                connector.SetAutoplayAudio(id, value);
            }
        }

        public bool? CaseSensitive
        {
            get
            {
                return connector.GetCaseSensitive(id);
            }
            set
            {
                connector.SetCaseSensitive(id, value);
            }
        }

        public bool? ConfirmDemote
        {
            get
            {
                return connector.GetConfirmDemote(id);
            }
            set
            {
                connector.SetConfirmDemote(id, value);
            }
        }

        public bool? EnableCommentary
        {
            get
            {
                return connector.GetEnableCommentary(id);
            }
            set
            {
                connector.SetEnableCommentary(id, value);
            }
        }

        public bool? CorrectOnTheFly
        {
            get
            {
                return connector.GetCorrectOnTheFly(id);
            }
            set
            {
                connector.SetCorrectOnTheFly(id, value);
            }
        }

        public bool? EnableTimer
        {
            get
            {
                return connector.GetEnableTimer(id);
            }
            set
            {
                connector.SetEnableTimer(id, value);
            }
        }

        public bool? RandomPool
        {
            get
            {
                return connector.GetRandomPool(id);
            }
            set
            {
                connector.SetRandomPool(id, value);
            }
        }

        public bool? SelfAssessment
        {
            get
            {
                return connector.GetSelfAssessment(id);
            }
            set
            {
                connector.SetSelfAssessment(id, value);
            }
        }

        public bool? ShowImages
        {
            get
            {
                return connector.GetShowImages(id);
            }
            set
            {
                connector.SetShowImages(id, value);
            }
        }

        public string StripChars
        {
            get
            {
                return connector.GetStripChars(id);
            }
            set
            {
                connector.SetStripChars(id, value);
            }
        }

        public CultureInfo QuestionCulture
        {
            get
            {
                return connector.GetCulture(id, Side.Question);
            }
            set
            {
                connector.SetCulture(id, Side.Question, value);
            }
        }

        public CultureInfo AnswerCulture
        {
            get
            {
                return connector.GetCulture(id, Side.Answer);
            }
            set
            {
                connector.SetCulture(id, Side.Answer, value);
            }
        }

        public string QuestionCaption
        {
            get
            {
                return connector.GetCaption(id, Side.Question);
            }
            set
            {
                connector.SetCaption(id, Side.Question, value);
            }
        }

        public string AnswerCaption
        {
            get
            {
                return connector.GetCaption(id, Side.Answer);
            }
            set
            {
                connector.SetCaption(id, Side.Answer, value);
            }
        }

        public Dictionary<CommentarySoundIdentifier, IMedia> CommentarySounds
        {
            get
            {
                return connector.GetCommentarySounds(id);
            }
            set
            {
                connector.SetCommentarySounds(id, value);
            }
        }

        void cSounds_ListChanged(object sender, ListChangedEventArgs e)
        {
            CommentarySounds = sender as Dictionary<CommentarySoundIdentifier, IMedia>;
        }

        public IMedia Logo
        {
            get
            {
                return connector.GetLogo(id);
            }
            set
            {
                connector.SetLogo(id, value);
            }
        }

        public bool? ShowStatistics
        {
            get
            {
                return connector.GetShowStatistics(id);
            }
            set
            {
                connector.SetShowStatistics(id, value);
            }
        }

        public bool? SkipCorrectAnswers
        {
            get
            {
                return connector.GetSkipCorrectAnswers(id);
            }
            set
            {
                connector.SetSkipCorrectAnswers(id, value);
            }
        }

        public ISnoozeOptions SnoozeOptions
        {
            get
            {
                return connector.GetSnoozeOptions(id);
            }
            set
            {
                connector.SetSnoozeOptions(id, value);
            }
        }

        public bool? PoolEmptyMessageShown
        {
            get
            {
                return connector.GetPoolEmptyMessage(id);
            }
            set
            {
                connector.SetPoolEmptyMessage(id, value);
            }
        }

        public bool? UseLMStylesheets
        {
            get
            {
                return connector.GetUseLmStylesheets(id);
            }
            set
            {
                connector.SetUseLmStylesheets(id, value);
            }
        }

        public IList<int> SelectedLearnChapters
        {
            get
            {
                ObservableList<int> list = new ObservableList<int>(connector.GetSelectedLearningChapters(id));
                list.ListChanged += new System.EventHandler<ObservableListChangedEventArgs<int>>(list_ListChanged);

                return list;
            }
            set
            {
                connector.SetSelectedLearningChapters(id, value);
            }
        }
        void list_ListChanged(object sender, ObservableListChangedEventArgs<int> e)
        {
            SelectedLearnChapters = sender as IList<int>;
        }

        public ICardStyle Style
        {
            get
            {
                return connector.GetCardStyle(id);
            }
            set
            {
                connector.SetCardStyle(id, value);
                (Style as DbCardStyle).FlushToDB();
            }
        }

        public CompiledTransform? QuestionStylesheet
        {
            get
            {
                string xsl = connector.GetQuestionStylesheet(id);
                if (xsl != null)
                    return new CompiledTransform(null, xsl);
                else
                    return null;
            }
            set
            {
                connector.SetQuestionStylesheet(id, value.Value.XslContent);
            }
        }

        public CompiledTransform? AnswerStylesheet
        {
            get
            {
                string xsl = connector.GetAnswerStylesheet(id);
                if (xsl != null)
                    return new CompiledTransform(null, xsl);
                else
                    return null;
            }
            set
            {
                connector.SetAnswerStylesheet(id, value.Value.XslContent);
            }
        }

        public bool? AutoBoxSize
        {
            get
            {
                return connector.GetAutoBoxSize(id);
            }
            set
            {
                connector.SetAutoBoxSize(id, value);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            SettingsHelper.Copy(this, target as ISettings, progressDelegate);
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
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
