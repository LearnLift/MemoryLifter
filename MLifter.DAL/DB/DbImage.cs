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
