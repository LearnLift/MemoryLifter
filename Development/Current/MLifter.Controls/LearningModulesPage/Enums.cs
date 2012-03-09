using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.Controls.LearningModulesPage
{
	/// <summary>
	/// How to display the left bar of the learning modules page.
	/// </summary>
	/// <remarks>Documented by Dev05, 2012-01-17</remarks>
	public enum LeftBarView
	{
		TreeView,
		XPExplorerBar
	}

	internal enum ServerIconState : int
	{
		Loaded = 4,
		LoadingOrOffline = 5,
		LicenseExceeded = 6,
		AuthenticationFailed = 7
	}

	internal enum LocalIconState : int
	{
		Loaded = 0,
		LoadingOrOffline = 1,
		LicenseExceeded = 2,
		AuthenticationFailed = 3
	}
}
