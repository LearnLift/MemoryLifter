using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB
{
    internal class DbAudio : DbMedia, Interfaces.IAudio
    {
        internal DbAudio(int id, int cardid, bool checkId, Side side, WordType type, bool isDefault, bool isExample, ParentClass parentClass)
            : base(id, cardid, checkId, EMedia.Audio, side, type, isDefault, isExample, parentClass) { }

    }
}
