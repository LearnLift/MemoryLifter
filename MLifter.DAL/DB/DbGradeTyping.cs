using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbGradeTyping : IGradeTyping
    {
        private IDbGradeTypingConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlGradeTypingConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeGradeTypingConnector.GetInstance(parent);
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

        public DbGradeTyping(int id, ParentClass parent)
            : this(id, true, parent) { }
        public DbGradeTyping(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
        }

        public override bool Equals(Object obj)
        {
            return GradeTypingHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGradeTyping Members

        public bool? AllCorrect
        {
            get
            {
                return connector.GetAllCorrect(id);
            }
            set
            {
                connector.SetAllCorrect(id, value);
            }
        }

        public bool? HalfCorrect
        {
            get
            {
                return connector.GetHalfCorrect(id);
            }
            set
            {
                connector.SetHalfCorrect(id, value);
            }
        }

        public bool? NoneCorrect
        {
            get
            {
                return connector.GetNoneCorrect(id);
            }
            set
            {
                connector.SetNoneCorrect(id, value);
            }
        }

        public bool? Prompt
        {
            get
            {
                return connector.GetPrompt(id);
            }
            set
            {
                connector.SetPrompt(id, value);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(IGradeTyping), progressDelegate);
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
