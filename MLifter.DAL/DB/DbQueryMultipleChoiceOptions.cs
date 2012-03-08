using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbQueryMultipleChoiceOptions : IQueryMultipleChoiceOptions
    {
        private IDbQueryMultipleChoiceOptionsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlQueryMultipleChoiceOptionsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeQueryMultipleChoiceOptionsConnector.GetInstance(parent);
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

        public DbQueryMultipleChoiceOptions(int id, ParentClass parent)
            : this(id, true, parent) { }
        public DbQueryMultipleChoiceOptions(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
        }

        public override bool Equals(object obj)
        {
            return QueryMultipleChoiceOptionsHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryMultipleChoiceOptions Members

        public bool? AllowRandomDistractors
        {
            get
            {
                return connector.GetAllowRandom(id);
            }
            set
            {
                connector.SetAllowRandom(id, value);
            }
        }

        public bool? AllowMultipleCorrectAnswers
        {
            get
            {
                return connector.GetAllowMultiple(id);
            }
            set
            {
                connector.SetAllowMultiple(id, value);
            }
        }

        public int? NumberOfChoices
        {
            get
            {
                return connector.GetChoices(id);
            }
            set
            {
                connector.SetChoices(id, value);
            }
        }

        public int? MaxNumberOfCorrectAnswers
        {
            get
            {
                return connector.GetMaxCorrect(id);
            }
            set
            {
                connector.SetMaxCorrect(id, value);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryMultipleChoiceOptions), progressDelegate);
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
