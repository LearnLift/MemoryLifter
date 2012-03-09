using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbQueryDirections : IQueryDirections
    {
        private IDbQueryDirectionsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlQueryDirectionsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeQueryDirectionsConnector.GetInstance(parent);
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

        public DbQueryDirections(int id, ParentClass parent)
            : this(id, true, parent) { }

        public DbQueryDirections(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
        }

        public override bool Equals(Object obj)
        {
            return QueryDirectionsHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IQueryDirections Members

        public bool? Question2Answer
        {
            get
            {
                return connector.GetQuestion2Answer(id);
            }
            set
            {
                connector.SetQuestion2Answer(id, value);
            }
        }

        public bool? Answer2Question
        {
            get
            {
                return connector.GetAnswer2Question(id);
            }
            set
            {
                connector.SetAnswer2Question(id, value);
            }
        }

        public bool? Mixed
        {
            get
            {
                return connector.GetMixed(id);
            }
            set
            {
                connector.SetMixed(id, value);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IQueryDirections), progressDelegate);
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
