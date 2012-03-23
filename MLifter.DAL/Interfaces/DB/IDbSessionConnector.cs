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
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// The database connector for PgSqlSessionConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-09-05</remarks>
    interface IDbSessionConnector
    {
        int OpenUserSession(int lm_id);
        void RecalculateBoxSizes(int sessionId);
        void CardFromBoxDeleted(int sessionId, int boxId);
        void CardAdded(int sessionId);
        void CloseUserSession(int last_entry);
        IDictionary RestartLearningSuccess(int lm_id);
    }
}
