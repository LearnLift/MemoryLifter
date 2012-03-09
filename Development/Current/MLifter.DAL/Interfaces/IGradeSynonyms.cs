using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
    /// <summary>
    /// Inteface which define synonym gradion
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public interface IGradeSynonyms : ICopy, IParent
    {
        /// <summary>
        /// Gets or sets all known.
        /// </summary>
        /// <value>All known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? AllKnown { get; set; }

        /// <summary>
        /// Gets or sets the half known.
        /// </summary>
        /// <value>The half known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? HalfKnown { get; set; }

        /// <summary>
        /// Gets or sets the one known.
        /// </summary>
        /// <value>The one known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? OneKnown { get; set; }

        /// <summary>
        /// Gets or sets the first known.
        /// </summary>
        /// <value>The first known.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? FirstKnown { get; set; }

        /// <summary>
        /// Gets or sets the prompt.
        /// </summary>
        /// <value>The prompt.</value>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        [ValueCopy]
        bool? Prompt { get; set; }
    }

    /// <summary>
    /// Helper class to match objects of the type IGradeSynonyms.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    public static class GradeSynonymsHelper
    {
        /// <summary>
        /// Compares the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public static bool Compare(object a, object b)
        {
            if (!typeof(IGradeSynonyms).IsAssignableFrom(a.GetType()) || !typeof(IGradeSynonyms).IsAssignableFrom(b.GetType()))
                return false;

            bool isMatch = true;
            isMatch &= ((a as IGradeSynonyms).AllKnown == (b as IGradeSynonyms).AllKnown);
            isMatch &= ((a as IGradeSynonyms).FirstKnown == (b as IGradeSynonyms).FirstKnown);
            isMatch &= ((a as IGradeSynonyms).HalfKnown == (b as IGradeSynonyms).HalfKnown);
            isMatch &= ((a as IGradeSynonyms).OneKnown == (b as IGradeSynonyms).OneKnown);
            isMatch &= ((a as IGradeSynonyms).Prompt == (b as IGradeSynonyms).Prompt);

            return isMatch;
        }
    }
}
