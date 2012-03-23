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
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// This reperesents a learning box.
	/// </summary>
	public interface IBox : IParent
	{
		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev02, 2008-01-16</remarks>
		int Id { get; }
		/// <summary>
		/// Gets the current box size.
		/// </summary>
		/// <value>The box size.</value>
		/// <remarks>Documented by Dev05, 2007-09-03</remarks>
		int CurrentSize { get; }
		/// <summary>
		/// Gets the box size based on query chapters.
		/// </summary>
		/// <value>The box size.</value>
		/// <remarks>Documented by Dev05, 2007-09-03</remarks>
		int Size { get; }
		/// <summary>
		/// Gets or sets the maximal box size.
		/// </summary>
		/// <value>The maximal box size.</value>
		/// <remarks>Documented by Dev05, 2007-09-03</remarks>
		int MaximalSize { get; set; }
		/// <summary>
		/// Gets the default box size.
		/// </summary>
		/// <value>The default box size.</value>
		/// <remarks>Documented by Dev05, 2007-09-03</remarks>
		int DefaultSize { get; }
	}
}
