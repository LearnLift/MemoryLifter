using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Tools;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// This represents a list of all boxes.
	/// </summary>
	public interface IBoxes : IParent
	{
		/// <summary>
		/// Gets the box.
		/// </summary>
		List<IBox> Box { get; }
	}
}
