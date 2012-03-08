using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.Preview
{
	/// <summary>
	/// This is a container object which is used for a preview statistic.
	/// It does not implement any data persistence!
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-03-23</remarks>
	public class PreviewStatistic : IStatistic
	{
		#region IStatistic Members

		/// <summary>
		/// Gets the id of the Session
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev03, 2007-09-06</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the start timestamp of the session.
		/// </summary>
		/// <value>The start timestamp.</value>
		/// <remarks>Documented by Dev03, 2007-09-06</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public DateTime StartTimestamp { get; set; }

		/// <summary>
		/// Gets or sets the end timestamp of the session.
		/// </summary>
		/// <value>The end timestamp.</value>
		/// <remarks>Documented by Dev03, 2007-09-06</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public DateTime EndTimestamp { get; set; }

		/// <summary>
		/// Gets or sets the correct answers of the session.
		/// </summary>
		/// <value>The right.</value>
		/// <remarks>Documented by Dev03, 2007-09-06</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Right { get; set; }

		/// <summary>
		/// Gets or sets the wrong answers of the session.
		/// </summary>
		/// <value>The wrong.</value>
		/// <remarks>Documented by Dev03, 2007-09-06</remarks>
		/// <remarks>Documented by Dev03, 2009-03-23</remarks>
		public int Wrong { get; set; }

		/// <summary>
		/// Gets or sets the boxes of the session.
		/// </summary>
		/// <value>
		/// The boxes.
		/// </value>
		public IList<int> Boxes
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
