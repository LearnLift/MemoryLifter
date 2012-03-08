using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace StickFactory
{
    class UsbFile
    {
        private string filename;

        public UsbFile() { }

        /// <summary>
        /// Gets or sets the full filename.
        /// </summary>
        /// <value>The filename.</value>
        /// <remarks>Documented by Dev08, 2008-10-07</remarks>
        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if(File.Exists(value))
                    filename = value;
            }
        }

        /// <summary>
        /// Gets the name of the file (without path).
        /// </summary>
        /// <value>The short name of the file.</value>
        /// <remarks>Documented by Dev08, 2008-10-07</remarks>
        public string ShortFileName
        {
            get
            {
                return Path.GetFileName(filename);
            }
        }
    }
}
