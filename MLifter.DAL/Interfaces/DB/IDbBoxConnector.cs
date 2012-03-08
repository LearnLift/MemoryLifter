using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
	interface IDbBoxConnector
	{
		/// <summary>
		/// Gets the size of the current.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		int GetCurrentSize(int id);

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		int GetSize(int id);

		/// <summary>
		/// Gets the size of the maximal.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		int GetMaximalSize(int id);
		/// <summary>
		/// Sets the size of the maximal.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="MaximalSize">Size of the maximal.</param>
		void SetMaximalSize(int id, int MaximalSize);

		/// <summary>
		/// Gets the default size.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		int GetDefaultSize(int id);
	}

	/// <summary>
	/// Stores the sizes of all boxes.
	/// </summary>
	public struct BoxSizes
	{
		/// <summary>
		/// Gets the sizes.
		/// </summary>
		public List<int> Sizes;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoxSizes"/> struct.
		/// </summary>
		/// <param name="pool">The pool.</param>
		/// <param name="box1">The box1.</param>
		/// <param name="box2">The box2.</param>
		/// <param name="box3">The box3.</param>
		/// <param name="box4">The box4.</param>
		/// <param name="box5">The box5.</param>
		/// <param name="box6">The box6.</param>
		/// <param name="box7">The box7.</param>
		/// <param name="box8">The box8.</param>
		/// <param name="box9">The box9.</param>
		/// <param name="box10">The box10.</param>
		public BoxSizes(int pool, int box1, int box2, int box3, int box4, int box5, int box6, int box7, int box8, int box9, int box10)
		{
			Sizes = new List<int>();
			Sizes.Add(pool);
			Sizes.Add(box1);
			Sizes.Add(box2);
			Sizes.Add(box3);
			Sizes.Add(box4);
			Sizes.Add(box5);
			Sizes.Add(box6);
			Sizes.Add(box7);
			Sizes.Add(box8);
			Sizes.Add(box9);
			Sizes.Add(box10);
		}
	}
}
