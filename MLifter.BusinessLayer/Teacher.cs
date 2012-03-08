using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Teacher loads and contains presets.
    /// </summary>
    /// <remarks>Documented by Dev03, 2008-09-24</remarks>
    public class Teacher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Teacher"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev03, 2008-09-24</remarks>
        public Teacher(string filename)
        {
            LoadPresets(filename);
        }

        private IPresets m_Presets;

        /// <summary>
        /// Gets the presets.
        /// </summary>
        /// <value>The presets.</value>
        /// <remarks>Documented by Dev03, 2008-09-24</remarks>
        public IPresets Presets
        {
            get { return m_Presets; }
        }

        /// <summary>
        /// Loads the presets.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>Documented by Dev03, 2008-09-24</remarks>
        public void LoadPresets(string filename)
        {
            m_Presets = PresetFactory.CreatePresets(filename);
        }
    }
}
