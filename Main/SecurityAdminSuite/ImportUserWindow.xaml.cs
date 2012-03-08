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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using MLifter.DAL;
using System.Net;
using System.DirectoryServices.Protocols;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace SecurityAdminSuite
{
	/// <summary>
	/// Interaction logic for ImportUserWindow.xaml
	/// </summary>
	public partial class ImportUserWindow : Window
	{
		Facade Facade { get; set; }

		public ImportUserWindow(Facade facade)
		{
			Facade = facade;

			InitializeComponent();
		}
		/// <summary>
		/// Gets the LDAP users from the LDAP server.
		/// </summary>
		/// <param name="ldapServer">The LDAP server, string format: "LDAP://172.22.100.10:389/OU=AT,O=ON"</param>
		/// <param name="directoryType">Type of the directory.</param>
		/// <param name="user">The user.</param>
		/// <param name="password">The password.</param>
		/// <param name="domain">The domain (AD only).</param>
		/// <returns>String list of LDAP users.</returns>
		/// <remarks>Documented by Dev09, 2009-06-08</remarks>
		public List<string> GetLdapUsers(string ldapServer, LocalDirectoryType directoryType, string user, string password, string domain)
		{
			List<string> LdapUsers = new List<string>();

			switch (directoryType)
			{
				case LocalDirectoryType.ActiveDirectory:
					if (String.IsNullOrWhiteSpace(domain))
					{
						string username = WindowsIdentity.GetCurrent().Name;
						domain = username.Substring(0, username.IndexOf(@"\"));
					}

					PrincipalContext context;
					if (!String.IsNullOrWhiteSpace(user) && !String.IsNullOrWhiteSpace(password) && !String.IsNullOrWhiteSpace(domain))
						context = new PrincipalContext(ContextType.Domain, domain, user, password);
					if (!String.IsNullOrWhiteSpace(domain))
						context = new PrincipalContext(ContextType.Domain, domain);
					else
						context = new PrincipalContext(ContextType.Domain);
					UserPrincipal userP = new UserPrincipal(context);
					userP.Enabled = true;
					PrincipalSearcher pS = new PrincipalSearcher();
					pS.QueryFilter = userP;

					PrincipalSearchResult<Principal> result = pS.FindAll();
					foreach (Principal p in result)
						LdapUsers.Add(domain + "\\" + p.SamAccountName);
					break;
				case LocalDirectoryType.eDirectory:
					string serverName = Regex.Match(ldapServer, @"^.+//(.+?):").Groups[1].ToString();
					string distinguishedName = ldapServer.Substring(ldapServer.LastIndexOf("/") + 1);

					LdapConnection connection = new LdapConnection(new LdapDirectoryIdentifier(serverName));
					connection.AuthType = AuthType.Basic;

					// attempt to connect
					try { connection.Bind(new NetworkCredential(user, password)); }
					catch (Exception exception)
					{
						Trace.WriteLine(exception.ToString());
					}

					// run search for users
					SearchResponse response = connection.SendRequest(new SearchRequest(distinguishedName, "(|(objectClass=person)(objectClass=user))", System.DirectoryServices.Protocols.SearchScope.Subtree, null)) as SearchResponse;

					foreach (SearchResultEntry entry in response.Entries)
					{
						if (entry.Attributes.Contains("cn") && entry.Attributes["cn"][0].ToString() != String.Empty)
						{
							LdapUsers.Add("cn=" + entry.Attributes["cn"][0].ToString());
						}

					}
					break;
			}

			return LdapUsers;
		}

		private void buttonImport_Click(object sender, RoutedEventArgs e)
		{
			foreach (string user in listBoxImportUsers.Items)
			{
				Facade.AddUser(user, string.Empty, SecurityFramework.UserAuthType.LocalDirectoryAuthentication);
			}

			Close();
		}

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void listBoxFoundUsers_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (listBoxFoundUsers.SelectedItems.Count <= 0)
				return;

			List<string> selectedUsers = new List<string>();
			foreach (object item in listBoxFoundUsers.SelectedItems)
				selectedUsers.Add(item as string);

			DragDrop.DoDragDrop(listBoxFoundUsers, selectedUsers, DragDropEffects.Move);
		}

		private void listBoxImportUsers_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(List<string>)) != null)
				e.Effects = DragDropEffects.Move;
			else
				e.Effects = DragDropEffects.None;
		}

		private void listBoxImportUsers_Drop(object sender, DragEventArgs e)
		{
			List<string> selectedUsers = e.Data.GetData(typeof(List<string>)) as List<string>;
			foreach (string user in selectedUsers)
				if (!listBoxImportUsers.Items.Contains(user))
					listBoxImportUsers.Items.Add(user);
		}

		private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			object obj = GetObjectDataFromPoint(sender as ListBox, e.GetPosition(sender as ListBox));

			if (!listBoxImportUsers.Items.Contains(obj))
				listBoxImportUsers.Items.Add(obj);
		}

		private ObservableCollection<string> UserFromDirectory;
		private void buttonSearch_Click(object sender, RoutedEventArgs e)
		{
			UserFromDirectory = new ObservableCollection<string>(GetLdapUsers("LDAP://" + textBoxServer.Text + ":" + textBoxPort.Text + "/" + textBoxON.Text,
				radioButtonActiveDirectory.IsChecked.Value ? LocalDirectoryType.ActiveDirectory : LocalDirectoryType.eDirectory, textBoxUser.Text,
				textBoxPassword.Password, textBoxDomain.Text));

			ICollectionView view = CollectionViewSource.GetDefaultView(UserFromDirectory);
			if (view != null)
				view.SortDescriptions.Add(new SortDescription(".", ListSortDirection.Ascending));

			listBoxFoundUsers.ItemsSource = UserFromDirectory;
		}

		private static object GetObjectDataFromPoint(ListBox source, Point point)
		{
			UIElement element = source.InputHitTest(point) as UIElement;
			if (element != null)
			{
				//get the object from the element
				object data = DependencyProperty.UnsetValue;
				while (data == DependencyProperty.UnsetValue)
				{
					// try to get the object value for the corresponding element
					data = source.ItemContainerGenerator.ItemFromContainer(element);

					//get the parent and we will iterate again
					if (data == DependencyProperty.UnsetValue)
						element = VisualTreeHelper.GetParent(element) as UIElement;

					//if we reach the actual listbox then we must break to avoid an infinite loop
					if (element == source)
						return null;
				}

				//return the data that we fetched only if it is not Unset value, 
				//which would mean that we did not find the data
				if (data != DependencyProperty.UnsetValue)
					return data;
			}

			return null;
		}

		private void radioButtonEDirectory_Click(object sender, RoutedEventArgs e)
		{
			if (radioButtonEDirectory.IsChecked.Value)
			{
				enableMicrosoftActiveDirectory(false);
			}
		}

		private void enableNoveleDirectory(bool value)
		{
			textBoxPort.IsEnabled = value;
			textBoxPassword.IsEnabled = value;
			textBoxON.IsEnabled = value;
			textBoxUser.IsEnabled = value;
			textBoxServer.IsEnabled = value;
		}

		private void enableMicrosoftActiveDirectory(bool value)
		{
			textBoxDomain.IsEnabled = value;
		}

		private void radioButtonActiveDirectory_Click(object sender, RoutedEventArgs e)
		{
			if (radioButtonActiveDirectory.IsChecked.Value)
			{
				enableMicrosoftActiveDirectory(true);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ICollectionView view = CollectionViewSource.GetDefaultView(UserFromDirectory);
			if (view != null)
			{
				view.Filter = new Predicate<object>(u => (u as string).ToLower().Contains(textBoxFilterUserList.Text.ToLower()));
			}
		}

		private void listBoxImportUsers_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			object obj = GetObjectDataFromPoint(sender as ListBox, e.GetPosition(sender as ListBox));
			listBoxImportUsers.Items.Remove(obj);
		}
	}
}
