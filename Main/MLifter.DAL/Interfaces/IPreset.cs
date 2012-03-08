using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Contains one set of Preset.
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-09-24</remarks>
    public interface IPreset
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev08, 2008-09-24</remarks>
        string Title { get; set; }

        /// <summary>
        /// Gets the resource id.
        /// </summary>
        /// <value>The resource id.</value>
        /// <remarks>Documented by Dev08, 2008-09-24</remarks>
        string ResourceId { get; set; }

        /// <summary>
        /// Gets the presets.
        /// </summary>
        /// <value>The presets.</value>
        /// <remarks>Documented by Dev08, 2008-09-24</remarks>
        ISettings Preset { get; set; }
    }
}
