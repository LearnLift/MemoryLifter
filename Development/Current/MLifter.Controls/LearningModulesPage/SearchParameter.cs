using System;
using System.Collections.Generic;
using System.Text;
using MLifter.BusinessLayer;

namespace MLifter.Controls.LearningModulesPage
{
	/// <summary>
	/// Search parameters
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-03-05</remarks>
	public struct SearchParameter
	{
		public string FilterWords;
		public IConnectionString SelectedConnectionString;
		public string UncPath;
		public bool ShowLearningModulesOfSubFolder;
		public bool AreAllConnectionSelected;

		public SearchParameter(string filterWords)
		{
			this.FilterWords = filterWords;
			SelectedConnectionString = null;
			UncPath = string.Empty;
			ShowLearningModulesOfSubFolder = true;
			AreAllConnectionSelected = false;
		}
	}
}
