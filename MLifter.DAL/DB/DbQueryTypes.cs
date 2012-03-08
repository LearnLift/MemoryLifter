using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using System.Globalization;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbQueryTypes : IQueryType
    {
        private IDbQueryTypesConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlQueryTypesConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeQueryTypesConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        private int id;
        public int Id
        {
            get { return id; }
        }

        public DbQueryTypes(int id, ParentClass parent)
            : this(id, true,parent) { }

        public DbQueryTypes(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
        }

        public override bool Equals(Object obj)
        {
            return QueryTypeHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryType Members

        public bool? ImageRecognition
        {
            get
            {
                return connector.GetImageRecognition(id);
            }
            set
            {
                connector.SetImageRecognition(id, value);
            }
        }

        public bool? ListeningComprehension
        {
            get
            {
                return connector.GetListeningComprehension(id);
            }
            set
            {
                connector.SetListeningComprehension(id, value);
            }
        }

        public bool? MultipleChoice
        {
            get
            {
                return connector.GetMultipleChoice(id);
            }
            set
            {
                connector.SetMultipleChoice(id, value);
            }
        }

        public bool? Sentence
        {
            get
            {
                return connector.GetSentence(id);
            }
            set
            {
                connector.SetSentence(id, value);
            }
        }

        public bool? Word
        {
            get
            {
                return connector.GetWord(id);
            }
            set
            {
                connector.SetWord(id, value);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryType), progressDelegate);
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
