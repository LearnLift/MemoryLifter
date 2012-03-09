using System;
using System.Collections.Generic;
using System.Text;
using MLifter.BusinessLayer;

namespace MLifter.Controls.LearningWindow
{
    /// <summary>
    /// Defines a user control which is able to communicate with the LearnLogic Business Layer
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-04-25</remarks>
    internal interface ILearnUserControl
    {
        /// <summary>
        /// Registers the learn logic to this control.
        /// </summary>
        /// <param name="learnlogic">The learnlogic.</param>
        /// <remarks>Documented by Dev02, 2008-04-22</remarks>
        void RegisterLearnLogic(LearnLogic learnlogic);
    }
}
