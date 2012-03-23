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
using System.Reflection;

namespace MLifter.DAL.Tools
{
	/// <summary>
	/// Represents a object that can be copied using CopyBase.
	/// </summary>
	public interface ICopy
	{
		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev05, 2012-01-11</remarks>
		void CopyTo(ICopy target, CopyToProgress progressDelegate);
	}

	/// <summary>
	/// Callback for the copy process.
	/// </summary>
	/// <param name="statusMessage">The status message.</param>
	/// <param name="currentProgress">The current progress.</param>
	public delegate void CopyToProgress(string statusMessage, double currentProgress);

	/// <summary>
	/// The base clase which uses the ICopy interface implementations.
	/// </summary>
	public static class CopyBase
	{
		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		/// <remarks>Documented by Dev05, 2012-01-11</remarks>
		public static void Copy(ICopy source, ICopy target, CopyToProgress progressDelegate)
		{
			if (source.GetType() != target.GetType())
				throw new ArgumentException("Source and Target must be the same type!");

			Copy(source, target, source.GetType(), progressDelegate);
		}

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="type">The type the source and target should be interpreded as.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public static void Copy(ICopy source, ICopy target, Type type, CopyToProgress progressDelegate)
		{
			if (!(type.IsAssignableFrom(source.GetType()) && type.IsAssignableFrom(target.GetType())))
				throw new ArgumentException("Source and Target must implement " + type.ToString());

			foreach (PropertyInfo info in type.GetProperties())
			{
				if (type.GetProperty(info.Name).IsDefined(typeof(IgnoreCopyAttribute), true) ||
					source.GetType().GetProperty(info.Name).IsDefined(typeof(IgnoreCopyAttribute), true) ||
					target.GetType().GetProperty(info.Name).IsDefined(typeof(IgnoreCopyAttribute), true))
					continue;

				if (typeof(ICopy).IsAssignableFrom(info.PropertyType))
				{
					ICopy copyObject = (info.GetValue(source, null) as ICopy);
					if (copyObject != null)
						copyObject.CopyTo(info.GetValue(target, null) as ICopy, progressDelegate);
				}

				if (info.IsDefined(typeof(ValueCopyAttribute), true))
				{
					object value = info.GetValue(source, null);
					if (value != null)
						info.SetValue(target, value, null);
				}
			}
		}
	}

	/// <summary>
	/// Marks a property to be copied by its value.
	/// </summary>
	public class ValueCopyAttribute : Attribute { }

	/// <summary>
	/// Marks a property to be ignroed from the copy process.
	/// </summary>
	public class IgnoreCopyAttribute : Attribute { }
}
