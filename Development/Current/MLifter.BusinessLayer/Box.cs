using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL;

namespace MLifter.BusinessLayer
{
    public class Box
    {
        private int size = 0;
        private int maximalSize = int.MaxValue;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        /// <summary>
        /// Gets or sets the maximal size.
        /// </summary>
        /// <value>The size of the maximal.</value>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public int MaximalSize
        {
            get { return maximalSize; }
            set { maximalSize = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public Box() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="MaximalSize">Size of the maximal.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public Box(int MaximalSize)
        {
            maximalSize = MaximalSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="Size">The size.</param>
        /// <param name="MaximalSize">Size of the maximal.</param>
        /// <remarks>Documented by Dev05, 2007-09-03</remarks>
        public Box(int Size, int MaximalSize)
        {
            maximalSize = MaximalSize;
            this.Size = Size;
        }
    }
}
