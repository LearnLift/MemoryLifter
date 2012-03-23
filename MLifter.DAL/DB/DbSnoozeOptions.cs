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
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbSnoozeOptions : ISnoozeOptions
    {
        private IDbSnoozeOptionsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlSnoozeOptionsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeSnoozeOptionsConnector.GetInstance(parent);
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

        public DbSnoozeOptions(int id, ParentClass parent)
            : this(id, true, parent) { }

        public DbSnoozeOptions(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
        }

        #region ISnoozeOptions Members

        public void DisableCards()
        {
            connector.SetCardsEnabled(id, false);
        }

        public void DisableRights()
        {
            connector.SetRightsEnabled(id, false);
        }

        public void DisableTime()
        {
            connector.SetTimeEnabled(id, false);
        }

        public void EnableCards(int cards)
        {
            connector.SetCardsEnabled(id, true);
            connector.SetSnoozeCards(id, cards);
        }

        public void EnableRights(int rights)
        {
            connector.SetRightsEnabled(id, true);
            connector.SetSnoozeRights(id, rights);
        }

        public void EnableTime(int time)
        {
            connector.SetTimeEnabled(id, true);
            connector.SetSnoozeTime(id, time);
        }

        public bool? IsCardsEnabled
        {
            get
            {
                return connector.GetCardsEnabled(id);
            }
            set
            {
                connector.SetCardsEnabled(id, value);
            }
        }

        public bool? IsRightsEnabled
        {
            get
            {
                return connector.GetRightsEnabled(id);
            }
            set
            {
                connector.SetRightsEnabled(id, value);
            }
        }

        public bool? IsTimeEnabled
        {
            get
            {
                return connector.GetTimeEnabled(id);
            }
            set
            {
                connector.SetTimeEnabled(id, value);
            }
        }

        public void SetSnoozeTimes(int lower_time, int upper_time)
        {
            connector.SetSnoozeHigh(id, upper_time);
            connector.SetSnoozeLow(id, lower_time);
        }

        public ESnoozeMode? SnoozeMode
        {
            get
            {
                return connector.GetSnoozeMode(id);
            }
            set
            {
                connector.SetSnoozeMode(id, value);
            }
        }

        public int? SnoozeCards
        {
            get
            {
                return connector.GetSnoozeCards(id);
            }
            set
            {
                connector.SetSnoozeCards(id, value);
            }
        }

        public int? SnoozeRights
        {
            get
            {
                return connector.GetSnoozeRights(id);
            }
            set
            {
                connector.SetSnoozeRights(id, value);
            }
        }

        public int? SnoozeTime
        {
            get
            {
                return connector.GetSnoozeTime(id);
            }
            set
            {
                connector.SetSnoozeTime(id, value);
            }
        }

        public int? SnoozeHigh
        {
            get
            {
                return connector.GetSnoozeHigh(id);
            }
            set
            {
                connector.SetSnoozeHigh(id, value);
            }
        }

        public int? SnoozeLow
        {
            get
            {
                return connector.GetSnoozeLow(id);
            }
            set
            {
                connector.SetSnoozeLow(id, value);
            }
        }

        #endregion

        #region ICopy Members

        public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
        {
            CopyBase.Copy(this, target, typeof(ISnoozeOptions), progressDelegate);
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
