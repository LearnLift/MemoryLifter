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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SecurityFramework;
using System.ComponentModel;
using MLifter.Controls;

namespace SecurityAdminSuite
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Facade _Facade;

		internal ListBox ListBoxUsers { get { return lbUsers; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			try
			{
				TaskDialog.ForceEmulationMode = true;
				InitializeComponent();
			}
			catch (Exception e)
			{
				TaskDialog.MessageBox("Error during startup", "An error occured in InitializeComponent().", string.Empty, e.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
			}

			try
			{
				this.DataContext = _Facade = new Facade();
				_Facade.MainWindow = this;
				authSettingsControl.Facade = _Facade;
				sessionsControl.Facade = _Facade;
				this.InputBindings.Add(new KeyBinding(_Facade.AddNewGroupCommand, new KeyGesture(Key.N, ModifierKeys.Control)));
			}
			catch (Exception e)
			{
				TaskDialog.MessageBox("Error during startup", "An error occured while starting the AdminSuite", string.Empty, e.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
				Environment.Exit(-1);
			}
		}

		private void UserControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				TextBox tb = e.OriginalSource as TextBox;
				if (tb == null) return;
				BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
				be.UpdateSource();
			}

		}

		protected override void OnClosed(EventArgs e)
		{
			_Facade.Save();
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			EntityInfo ei = (sender as TreeView).SelectedItem as EntityInfo;
			_Facade.ActualObject = ei.Tag;
		}

		private void Window_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void AddGroupMenu_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = e.OriginalSource as MenuItem;
			if (item == null) return;

			GroupMembershipInfo gmsi = item.Header as GroupMembershipInfo;
			if (gmsi == null) return;
			gmsi.IsMember = true;

			permissionsPresenter.GetBindingExpression(System.Windows.Controls.ContentPresenter.ContentProperty).UpdateTarget();
		}

		private void expGroupObjectPermission_Expanded(object sender, RoutedEventArgs e)
		{
			if (expGroupTypePermission != null)
				// this.expGroupTypePermission.IsExpanded = false;
				if (_Facade != null)
				{
					object actualObject = this._Facade.ActualObject;
					this._Facade.ActualObject = null;
					this._Facade.ActualObject = actualObject;
				}
		}

		private void expGroupTypePermission_Expanded(object sender, RoutedEventArgs e)
		{
			if (expGroupObjectPermission != null)
				//  this.expGroupObjectPermission.IsExpanded = false;

				if (_Facade != null)
				{
					TypeInfo actualType = this._Facade.ActualType;
					this._Facade.ActualType = null;
					this._Facade.ActualType = actualType;
				}
		}

		private void expUserTypePermission_Expanded(object sender, RoutedEventArgs e)
		{
			if (expUserObjectPermission != null)
				//   this.expUserObjectPermission.IsExpanded = false;
				if (_Facade != null)
				{
					TypeInfo actualType = this._Facade.ActualType;
					this._Facade.ActualType = null;
					this._Facade.ActualType = actualType;
				}
		}

		private void expUserObjectPermission_Expanded(object sender, RoutedEventArgs e)
		{
			if (expUserTypePermission != null)
				//   this.expUserTypePermission.IsExpanded = false;

				if (_Facade != null)
				{
					object actualObject = this._Facade.ActualObject;
					this._Facade.ActualObject = null;
					this._Facade.ActualObject = actualObject;
				}

		}

		private void menuItemExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void menuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			AboutBox about = new AboutBox(this);
			about.ShowDialog();
		}

		private void lbTypes_Loaded(object sender, RoutedEventArgs e)
		{
			if (lbTypes.SelectedIndex < 0)
				lbTypes.SelectedIndex = 0;
		}

		private void lbUserTypePermissions_Loaded(object sender, RoutedEventArgs e)
		{
			if (lbUserTypePermissions.SelectedIndex < 0)
				lbUserTypePermissions.SelectedIndex = 0;
		}

		private void lbUsers_Loaded(object sender, RoutedEventArgs e)
		{
			// add permanent sort to users (username)
			lbUsers.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("User.Account", System.ComponentModel.ListSortDirection.Ascending));
		}

		private void Window_Closing(object sender, CancelEventArgs e) { }

		/// <summary>
		/// Handles the LearningModuleAdded event of the authControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-06-17</remarks>
		private void authControl_LearningModuleAdded(object sender, EventArgs e)
		{
			tvUsersObjectPermissions.Dispatcher.Invoke((Action)delegate()
			{
				tvUsersObjectPermissions.GetBindingExpression(TreeView.ItemsSourceProperty).UpdateTarget();
				tvGroupsObjectPermissions.GetBindingExpression(TreeView.ItemsSourceProperty).UpdateTarget();
			});
		}

		/// <summary>
		/// Handles the SelectionChanged event of the TabControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-06-19</remarks>
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_Facade == null) //fix for [AS-2338]
				return;

			if (tabControlMain.SelectedIndex < 1)
				_Facade.ActualObject = tvGroupsObjectPermissions.SelectedItem != null ? (tvGroupsObjectPermissions.SelectedItem as DicInfo).Tag : null;
			else
				_Facade.ActualObject = tvUsersObjectPermissions.SelectedItem != null ? (tvUsersObjectPermissions.SelectedItem as DicInfo).Tag : null;
		}

	}
}
