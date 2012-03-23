/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
