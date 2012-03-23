/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
