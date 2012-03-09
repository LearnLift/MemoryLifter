using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// Business layer representation of a chapter.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-19</remarks>
    public partial class Chapter
    {
        private IChapter chapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chapter"/> class.
        /// </summary>
        /// <param name="chapter">The chapter.</param>
        /// <remarks>Documented by Dev03, 2009-01-19</remarks>
        public Chapter(IChapter chapter)
        {
            this.chapter = chapter;
        }
    }
}
