using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    internal class DbVideo : DbMedia, IVideo
    {
        internal DbVideo(int id, int cardid, bool checkId, Side side, WordType type, bool isDefault, bool isExample, ParentClass parent)
            : base(id, cardid, checkId, EMedia.Video, side, type, isDefault, isExample, parent) { }

    }
}
