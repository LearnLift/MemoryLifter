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
    class DbStatistics : IStatistics
    {
        #region Init...
        private IList<IStatistic> stats = new List<IStatistic>();
        private int learningModuleId;

        private IDbStatisticsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlStatisticsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeStatisticsConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        public DbStatistics(int lmId, ParentClass parentClass)
        {
            this.parent = parentClass;
            this.learningModuleId = lmId;

            //Update the whole Statistics list
            stats.Clear();
            List<int> learnSessions = connector.GetLearnSessions(learningModuleId);
            foreach (int id in learnSessions)
                stats.Add(new DbStatistic(id, parent));

        }
        #endregion

        #region IList<IStatistic> Members
        public int IndexOf(IStatistic item)
        {
            return stats.IndexOf(item);
        }

        public void Insert(int index, IStatistic item)
        {
            throw new NotAllowedInDbModeException();
        }

        public void RemoveAt(int index)
        {
            throw new NotAllowedInDbModeException();
        }

        public IStatistic this[int index]
        {
            get
            {
                //Update the whole Statistics list
                stats.Clear();
                List<int> learnSessions = connector.GetLearnSessions(learningModuleId);
                foreach (int id in learnSessions)
                    stats.Add(new DbStatistic(id, parent));

                return stats[index];
            }
            set
            {
                throw new NotAllowedInDbModeException();
            }
        }

        #endregion

        #region ICollection<IStatistic> Members

        public void Add(IStatistic item)
        {
            connector.CopyStatistics(learningModuleId, item);
        }

        public void Clear()
        {
            throw new NotAllowedInDbModeException();
        }

        public bool Contains(IStatistic item)
        {
            return stats.Contains(item);
        }

        public void CopyTo(IStatistic[] array, int arrayIndex)
        {
            stats.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return stats.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(IStatistic item)
        {
            throw new NotAllowedInDbModeException();
        }

        #endregion

        #region IEnumerable<IStatistic> Members

        public IEnumerator<IStatistic> GetEnumerator()
        {
            //Update the whole Statistics list
            stats.Clear();
            List<int> learnSessions = connector.GetLearnSessions(learningModuleId);
            foreach (int id in learnSessions)
                stats.Add(new DbStatistic(id, parent));

            return (stats as IEnumerable<IStatistic>).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return stats.GetEnumerator();
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            IStatistics targetStatistics = target as IStatistics;
            if (targetStatistics != null)
                StatisticsHelper.Copy(this as IStatistics, targetStatistics, progressDelegate);
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
