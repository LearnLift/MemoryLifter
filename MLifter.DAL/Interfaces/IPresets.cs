using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Contains a list of Presets.
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-09-24</remarks>
    public interface IPresets
    {
        /// <summary>
        /// Gets the presets.
        /// </summary>
        /// <value>The presets.</value>
        /// <remarks>Documented by Dev08, 2008-09-24</remarks>
        List<IPreset> Presets { get; }
    }
}
