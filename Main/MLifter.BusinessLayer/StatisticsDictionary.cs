using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;

namespace MLifter.BusinessLayer
{
    public class StatisticsDictionary
    {
        private Dictionary dictionary;
        private IStatistics statistics;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsDictionary"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        /// <remarks>Documented by Dev05, 2007-09-04</remarks>
        public StatisticsDictionary(Dictionary dict, IStatistics Statistics)
        {
            dictionary = dict;
            this.statistics = Statistics;
        }

        /// <summary>
        /// Gets the known cards at the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-10-22</remarks>
        public int GetKnown(DateTime time)
        {
            IStatistic stat = GetLatestStatisticToTime(time);

            if (stat == null)
                return 0;

            int known = 0;
            for (int i = 1; i < stat.Boxes.Count; i++)
                known += stat.Boxes[i];

            return known;
        }

        /// <summary>
        /// Gets the content of the box at the specified time.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-10-22</remarks>
        public int GetBoxContent(int box, DateTime time)
        {
            if (box < 1 || box > 10)
                return 0; //throw new ArgumentException("Not a valid Box number");

            IStatistic stat = GetLatestStatisticToTime(time);
            return stat != null ? stat.Boxes[box - 1] : 0;
        }

        /// <summary>
        /// Gets the latest statistic to the given time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-10-23</remarks>
        public IStatistic GetLatestStatisticToTime(DateTime time)
        {
            IStatistic statistic = null;
            DateTime? oldEndTime = null;
            foreach (IStatistic stat in statistics)
            {
                DateTime newEndTime = stat.EndTimestamp;
                if (newEndTime <= time && (statistic == null || newEndTime > oldEndTime.Value))
                {
                    statistic = stat;
                    oldEndTime = newEndTime;
                }
            }
            return statistic;
        }

        /// <summary>
        /// Gets the current stats.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-10-23</remarks>
        public LearnStats GetCurrentStats()
        {
            LearnStats stats = new LearnStats();
            foreach (IStatistic stat in statistics)
            {
                stats.LearnTime += (stat.EndTimestamp - stat.StartTimestamp);
                stats.NumberOfRights += stat.Right;
                stats.NumberOfWrongs += stat.Wrong;
            }

            return stats;
        }

        /// <summary>
        /// Gets the oldest statistic.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-10-23</remarks>
        public IStatistic GetOldestStatistic()
        {
            IStatistic statistic = null;
            DateTime? oldEndTime = null;
            foreach (IStatistic stat in statistics)
            {
                DateTime newEndTime = stat.EndTimestamp;
                if (statistic == null || newEndTime < oldEndTime.Value)
                {
                    statistic = stat;
                    oldEndTime = newEndTime;
                }
            }
            return statistic;
        }

        /// <summary>
        /// Gets the newest statistic.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-10-23</remarks>
        public IStatistic GetNewestStatistic()
        {
            IStatistic statistic = null;
            DateTime? oldEndTime = null;
            foreach (IStatistic stat in statistics)
            {
                DateTime newEndTime = stat.EndTimestamp;
                if (statistic == null || newEndTime > oldEndTime.Value)
                {
                    statistic = stat;
                    oldEndTime = newEndTime;
                }
            }
            return statistic;
        }

        /// <summary>
        /// Deletes all statistics.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public void DeleteAllStatistics()
        {
            statistics.Clear();
        }
    }

    public class LearnStats
    {
        public TimeSpan LearnTime = new TimeSpan(0);
        public int NumberOfRights = 0;
        public int NumberOfWrongs = 0;
    }
}
