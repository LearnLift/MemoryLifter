using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.Tools
{
    /// <summary>
    /// PresentFactory Class, handling of Presets.
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-09-24</remarks>
    public class PresetFactory
    {
        /// <summary>
        /// Creates the Presets Class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-09-24</remarks>
        public static IPresets CreatePresets(string path)
        {
            return new XML.XmlPresets(path);
        }
    }
}
