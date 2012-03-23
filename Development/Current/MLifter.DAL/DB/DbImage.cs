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
using MLifter.DAL.DB;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    internal class DbImage : DbMedia, Interfaces.IImage
    {
        internal DbImage(int id, int cardid, bool checkId, Side side, WordType type, bool isDefault, bool isExample, ParentClass parent)
            : base(id, cardid, checkId, EMedia.Image, side, type, isDefault, isExample, parent) { }

        protected Interfaces.DB.IDbMediaConnector mediaPropertiesConnector
        {
            get
            {
                switch (parent.CurrentUser.ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlMediaConnector.GetInstance(parent);
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeMediaConnector.GetInstance(parent);
                    default:
                        throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
                }
            }
        }

        #region IImage Members

        public int Width
        {
            get
            {
                int width = -1;
                string sWidth = mediaPropertiesConnector.GetPropertyValue(Id, MediaProperty.Width);

                if (sWidth == null || !int.TryParse(sWidth, out width))
                    width = -1;

                return width;
            }
            set
            {
                if (parent.CurrentUser.ConnectionString.SyncType == SyncType.HalfSynchronizedWithoutDbAccess)
                    throw new NotAllowedInSyncedModeException();

                mediaconnector.SetPropertyValue(Id, MediaProperty.Width, Convert.ToString(value));
            }
        }

        public int Height
        {
            get
            {
                int height = -1;
                string sHeight = mediaPropertiesConnector.GetPropertyValue(Id, MediaProperty.Height);

                if (sHeight == null || !int.TryParse(sHeight, out height))
                    height = -1;

                return height;
            }
            set
            {
                if (parent.CurrentUser.ConnectionString.SyncType == SyncType.HalfSynchronizedWithoutDbAccess)
                    throw new NotAllowedInSyncedModeException();

                mediaconnector.SetPropertyValue(Id, MediaProperty.Height, Convert.ToString(value));
            }
        }

        #endregion
    }
}
