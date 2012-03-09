using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Diagnostics;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
    internal class XmlImage : XmlMedia, IImage
    {
        private int m_width = 0;
        private int m_height = 0;

        internal XmlImage(string filename, ParentClass parent)
            : base(EMedia.Image, filename, parent)
        {
        }

        internal XmlImage(string filename, bool active, ParentClass parent)
            : base(EMedia.Image, filename, parent)
        {
            m_active = active;
        }

        internal XmlImage(string filename, int width, int height, bool active, ParentClass parent)
            : base(EMedia.Image, filename, parent)
        {
            m_active = active;
            m_width = width;
            m_height = height;
        }

        internal XmlImage(XmlDictionary dictionary, string filename, ParentClass parent)
            : base(dictionary, EMedia.Image, filename, parent)
        {
        }

        internal XmlImage(XmlDictionary dictionary, string filename, bool active, ParentClass parent)
            : base(dictionary, EMedia.Image, filename, parent)
        {
            m_active = active;
        }

        internal XmlImage(XmlDictionary dictionary, string filename, int width, int height, bool active, ParentClass parent)
            : base(dictionary, EMedia.Image, filename, parent)
        {
            m_active = active;
            m_width = width;
            m_height = height;
        }

        private void ReadSizeValues()
        {
            if (m_height == 0 && m_width == 0)
            {
                try
                {
                    if (System.IO.File.Exists(Filename))
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(Filename))
                        {
                            m_height = image.Height;
                            m_width = image.Width;
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Error during XML reading of Image size: " + exp.ToString());
                }
            }
        }

        #region IMedia Members
        public int Width
        {
            get
            {
                ReadSizeValues();
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }

        public int Height
        {
            get
            {
                ReadSizeValues();
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }
        #endregion
    }
}
