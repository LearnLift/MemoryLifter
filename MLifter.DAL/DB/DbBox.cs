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
