using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using System.IO;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    internal class XmlAudio : XmlMedia, IAudio
    {
        internal XmlAudio(string filename, ParentClass parent)
            : base(EMedia.Audio, filename, parent)
        {
        }

        internal XmlAudio(string filename, bool defaultAudio, bool exampleAudio, ParentClass parent)
            : base(EMedia.Audio, filename, parent)
        {
            m_default = defaultAudio;
            m_example = exampleAudio;
        }

        internal XmlAudio(string filename, bool active, bool defaultAudio, bool exampleAudio, ParentClass parent)
            : base(EMedia.Audio, filename, parent)
        {
            m_active = active;
            m_default = defaultAudio;
            m_example = exampleAudio;
        }

        internal XmlAudio(XmlDictionary dictionary, string filename, ParentClass parent)
            : base(dictionary, EMedia.Audio, filename, parent)
        {
        }

        internal XmlAudio(XmlDictionary dictionary, string filename, bool defaultAudio, bool exampleAudio, ParentClass parent)
            : base(dictionary, EMedia.Audio, filename, parent)
        {
            m_default = defaultAudio;
            m_example = exampleAudio;
        }

        internal XmlAudio(XmlDictionary dictionary, string filename, bool active, bool defaultAudio, bool exampleAudio, ParentClass parent)
            : base(dictionary, EMedia.Audio, filename, parent)
        {
            m_active = active;
            m_default = defaultAudio;
            m_example = exampleAudio;
        }
    }
}
