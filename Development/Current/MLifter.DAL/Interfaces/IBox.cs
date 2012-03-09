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
