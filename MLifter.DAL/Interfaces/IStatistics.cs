using System;
using System.Collections.Generic;
using System.Diagnostics;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Statistics interface;
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-09-06</remarks>
    public interface IStatistics : IList<IStatistic>, ICopy
    {
    }


    /// <summary>
    /// StatisticsHelper
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-02-06</remarks>
    public static class StatisticsHelper
    {
        /// <summary>
        /// Copies the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <remarks>Documented by Dev08, 2009-02-06</remarks>
        public static void Copy(IStatistics source, IStatistics target, CopyToProgress progressDelegate)
        {
            try
            {
                foreach (IStatistic statistic in source)
                    target.Add(statistic);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString(), "StatisticsHelper.Copy()");
            }
        }
    }
}
