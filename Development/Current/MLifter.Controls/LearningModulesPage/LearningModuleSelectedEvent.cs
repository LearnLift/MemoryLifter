using System;
using System.Collections.Generic;
using System.Text;
using MLifter.BusinessLayer;

namespace MLifter.Controls.LearningModulesPage
{
	public delegate void LearningModuleSelectedEventHandler(object sender, LearningModuleSelectedEventArgs e);
	public class LearningModuleSelectedEventArgs : EventArgs
	{
		private LearningModulesIndexEntry learnModule;
		/// <summary>
		/// Gets or sets the entry which was updated.
		/// </summary>
		/// <value>The entry.</value>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public LearningModulesIndexEntry LearnModule
		{
			get { return learnModule; }
			set { learnModule = value; }
		}

		public bool IsUsedDragAndDrop { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModuleSelectedEventArgs"/> class.
		/// </summary>
		/// <param name="entry">The entry which was updated.</param>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public LearningModuleSelectedEventArgs(LearningModulesIndexEntry learnModuleConnection)
		{
			this.learnModule = learnModuleConnection;
			IsUsedDragAndDrop = false;
		}
	}
}
