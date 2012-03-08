using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Tools;
using System.Diagnostics;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// DbStatistic class
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-11-12</remarks>
    class DbStatistic : IStatistic
    {
        #region Init...

        private IDbStatisticConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlStatisticConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeStatisticConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbStatistic"/> class.
        /// </summary>
        /// <param name="id">The sessionId.</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev08, 2008-11-13</remarks>
        public DbStatistic(int id, ParentClass parent)
        {
            this.parent = parent;
            this.id = id;
        }
        #endregion

        private int id;
        public int Id
        {
            get
            {
                return id;
            }
        }

        public int Right
        {
            get
            {
                int? c = connector.GetCorrectCards(id);
                return c.HasValue ? c.Value : 0;
            }
            set 
            { 
                Debug.WriteLine("Setter of the 'Right' property in DbStatistic is not allowed");
                throw new NotAllowedInDbModeException();
            }
        }

        public int Wrong
        {
            get
            {
                int? c = connector.GetWrongCards(id);
                return c.HasValue ? c.Value : 0;
            }
            set
            {
                Debug.WriteLine("Setter of the 'Wrong' property in DbStatistic is not allowed");
                throw new NotAllowedInDbModeException();
            }
        }

        public IList<int> Boxes
        {
            get
            {
                return connector.GetContentOfBoxes(id);
            }
        }

        public DateTime StartTimestamp
        {
            get
            {
                return connector.GetStartTimeStamp(id).Value;
            }
            set
            {
                Debug.WriteLine("Setter of the 'StartTimestamp' property in DbStatistic is not allowed");
                throw new NotAllowedInDbModeException();
            }
        }

        public DateTime EndTimestamp
        {
            get
            {
                return connector.GetEndTimeStamp(id).Value;
            }
            set
            {
                Debug.WriteLine("Setter of the 'EndTimestamp' property in DbStatistic is not allowed");
                throw new NotAllowedInDbModeException();
            }
        }

        #region IParent Members

        private ParentClass parent;
        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion
    }
}
