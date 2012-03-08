using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Preview;

namespace MLifter.BusinessLayer
{
    public partial class LearningModulesIndex
    {
        /// <summary>
        /// Gets the statistics for an index entry.
        /// </summary>
        /// <param name="entry">The learning module index entry.</param>
        /// <remarks>Documented by Dev03, 2008-12-04</remarks>
        private static void GetStatistics(LearningModulesIndexEntry entry)
        {
            LearningModuleStatistics lms = new LearningModuleStatistics();
            entry.Statistics = lms;
            try
            {
                if (entry.Dictionary.Statistics.Count > 0)
                {
                    for (int i = (entry.Dictionary.Statistics.Count - 1); i >= 0; i--)
                    {
                        IStatistic stat = entry.Dictionary.Statistics[i];
                        if ((stat.Right + stat.Wrong) > 0)
                        {
                            lms.CardsAsked = stat.Right + stat.Wrong;
                            lms.LastSessionTime = stat.EndTimestamp - stat.StartTimestamp;
                            lms.LastEndTime = stat.EndTimestamp;
                            lms.LastStartTime = stat.StartTimestamp;
                            lms.Right = stat.Right;
                            lms.Wrong = stat.Wrong;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LearningModulesIndex.GetStatistics() - " + ex.Message);
            }
        }
    }
}
