using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbWord : IWord
    {
        private IDbWordConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlWordConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeWordConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }
        private int id;
        private bool loading = false;
        private WordType type;
        private bool isDefault;
        private string word;

        public DbWord(int id, ParentClass parent)
        {
            loading = true;
            this.id = id;
            Word = connector.GetWord(id);
            Type = connector.GetType(id);
            Default = connector.GetDefault(id);
            this.parent = parent;
            loading = false;
        }
        public DbWord(int id, string word, WordType type, bool isDefault, ParentClass parent)
        {
            loading = true;
            this.id = id;
            Word = word;
            Type = type;
            Default = isDefault;
            this.parent = parent;
            loading = false;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Id, Word);
        }

        #region IWord Members

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2008-01-08</remarks>
        /// <remarks>Documented by Dev11, 2008-07-25</remarks>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Gets or sets the word type.
        /// </summary>
        /// <value>The word type.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        /// <remarks>Documented by Dev11, 2008-07-25</remarks>
        public WordType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                if (!loading)
                    connector.SetType(id, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWord"/> is default.
        /// </summary>
        /// <value><c>true</c> if default; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        /// <remarks>Documented by Dev11, 2008-07-25</remarks>
        public bool Default
        {
            get
            {
                return isDefault;
            }
            set
            {
                isDefault = value;
                if (!loading)
                    connector.SetDefault(id, value);
            }
        }

        /// <summary>
        /// Gets or sets the word.
        /// </summary>
        /// <value>The word.</value>
        /// <remarks>Documented by Dev03, 2007-10-02</remarks>
        /// <remarks>Documented by Dev11, 2008-07-25</remarks>
        public string Word
        {
            get
            {
                if (word != null)
                    return word;
                else
                    return string.Empty;
            }
            set
            {
                if (value != null)
                    word = value;
                else
                    word = string.Empty;
                if (!loading)
                    connector.SetWord(id, word);
            }
        }

        #endregion

        #region IParent Members

        private ParentClass parent;

        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion
    }
}
