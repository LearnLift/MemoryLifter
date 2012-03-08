using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Exception gets thrown when the dictionary content is protected from being copied.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-09-30</remarks>
    public class DictionaryContentProtectedException : Exception
    { }

    /// <summary>
    /// Exception thrown if OpenDictionary/Learning Module Methods realize that they have to unpack.
    /// </summary>
    /// <remarks>Documented by Dev10, 2009-27-02</remarks>
    public class NeedToUnPackException : Exception
    {
        public NeedToUnPackException(LearningModulesIndexEntry module)
        {
            this.module = module;
        }

        public LearningModulesIndexEntry module { get; set; }
    }

    /// <summary>
    /// Exception is thrown when we discover a odf file.
    /// </summary>
    /// <remarks>Documented by Dev10, 2009-27-02</remarks>
    public class IsOdfFormatException : Exception
    {
        public IsOdfFormatException(LearningModulesIndexEntry module)
        {
            this.module = module;
        }

        public LearningModulesIndexEntry module { get; set; }
    }

    /// <summary>
    /// Exception is thrown when we discover a odx file.
    /// </summary>
    /// <remarks>Documented by Dev10, 2009-27-02</remarks>
    public class IsOdxFormatException : Exception
    {
        public IsOdxFormatException(LearningModulesIndexEntry module)
        {
            this.module = module;
        }

        public LearningModulesIndexEntry module { get; set; }
    }
}
