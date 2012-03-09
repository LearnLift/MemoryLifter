using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using System.IO;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    internal class XmlVideo : XmlMedia, IVideo
    {
        internal XmlVideo(string filename, ParentClass parent)
            : base(EMedia.Video, filename, parent)
        {
        }

        internal XmlVideo(string filename, bool active, ParentClass parent)
            : base(EMedia.Video, filename, parent)
        {
            m_active = active;
        }

        internal XmlVideo(XmlDictionary dictionary, string filename, ParentClass parent)
            : base(dictionary, EMedia.Video, filename, parent)
        {
        }

        internal XmlVideo(XmlDictionary dictionary, string filename, bool active, ParentClass parent)
            : base(dictionary, EMedia.Video, filename, parent)
        {
            m_active = active;
        }
    }
}
