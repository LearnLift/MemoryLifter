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
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    class DbGradeSynonyms : IGradeSynonyms
    {
        private IDbGradeSynonymsConnector connector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlGradeSynonymsConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeGradeSynonymsConnector.GetInstance(parent);
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

        public DbGradeSynonyms(int id, ParentClass parent)
            : this(id, true, parent) { }
        public DbGradeSynonyms(int id, bool checkId, ParentClass parent)
        {
            this.parent = parent;

            if (checkId)
                connector.CheckId(id);

            this.id = id;
        }

        public override bool Equals(object obj)
        {
            return GradeSynonymsHelper.Compare(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IGradeSynonyms Members

        public bool? AllKnown
        {
            get
            {
                return connector.GetAllKnown(id);
            }
            set
            {
                connector.SetAllKnown(id, value);
            }
        }

        public bool? HalfKnown
        {
            get
            {
                return connector.GetHalfKnown(id);
            }
            set
            {
                connector.SetHalfKnown(id, value);
            }
        }

        public bool? OneKnown
        {
            get
            {
                return connector.GetOneKnown(id);
            }
            set
            {
                connector.SetOneKnown(id, value);
            }
        }

        public bool? FirstKnown
        {
            get
            {
                return connector.GetFirstKnown(id);
            }
            set
            {
                connector.SetFirstKnown(id, value);
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
            CopyBase.Copy(this, target, typeof(IGradeSynonyms), progressDelegate);
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
