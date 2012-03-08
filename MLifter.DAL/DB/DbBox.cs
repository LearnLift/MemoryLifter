using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using Npgsql;

namespace MLifter.DAL.DB
{
    class DbBox : IBox
    {
        private Interfaces.DB.IDbBoxConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return MLifter.DAL.DB.PostgreSQL.PgSqlBoxConnector.GetInstance(Parent.GetChildParentClass(this));
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeBoxConnector.GetInstance(Parent.GetChildParentClass(this));
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        public DbBox(int id, ParentClass parent)
        {
            this.id = id;
            this.parent = parent;
        }

        #region IBox Members

        private int id;
        public int Id { get { return id; } }

        public int CurrentSize
        {
            get
            {
                return connector.GetCurrentSize(id);
            }
        }

        public int Size
        {
            get
            {
                return connector.GetSize(id);
            }
        }

        public int MaximalSize
        {
            get
            {
                return connector.GetMaximalSize(id);
            }
            set
            {
                connector.SetMaximalSize(id, value);
            }
        }

        public int DefaultSize
        {
            get
            {
                return connector.GetDefaultSize(id);
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
