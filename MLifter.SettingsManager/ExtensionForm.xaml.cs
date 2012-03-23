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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MLifter.BusinessLayer;
using MLifter.Controls;
using MLifter.DAL;
using MLifter.DAL.DB;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.Generics;

namespace MLifterSettingsManager
{
	/// <summary>
	/// Interaction logic for ExtensionForm.xaml
	/// </summary>
	public partial class ExtensionForm : Window
	{
		public ObservableCollection<string> ExtensionDataFiles = new ObservableCollection<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionForm"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		public ExtensionForm()
		{
			InitializeComponent();

			this.DataContext = this;

			foreach (object value in Enum.GetValues(typeof(ExtensionType)))
				ComboBoxExtensionTyp.Items.Add(value);

			if (ComboBoxExtensionTyp.Items.Count > 0)
				ComboBoxExtensionTyp.SelectedIndex = 0;

			ComboBoxStartFile.ItemsSource = ExtensionDataFiles;

			ExtensionDataFiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ExtensionDataFiles_CollectionChanged);

			DependencyPropertyDescriptor descriptorExtensionFile = DependencyPropertyDescriptor.FromProperty(ExtensionFileProperty, typeof(ExtensionForm));
			if (descriptorExtensionFile != null)
			{
				descriptorExtensionFile.AddValueChanged(this, delegate
				{
					ExtensionFileLoaded = ExtensionFile != null;
					RefreshExtensionDataFiles();
					listBoxActions.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
				});
			}

			Clear();
		}

		/// <summary>
		/// Handles the CollectionChanged event of the ExtensionDataFiles control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		private void ExtensionDataFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			StartFilesAvailable = ExtensionDataFiles.Count > 0;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [start files available].
		/// </summary>
		/// <value><c>true</c> if [start files available]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		public bool StartFilesAvailable
		{
			get { return (bool)GetValue(StartFilesAvailableProperty); }
			set { SetValue(StartFilesAvailableProperty, value); }
		}

		// Using a DependencyProperty as the backing store for StartFilesAvailable.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StartFilesAvailableProperty =
			DependencyProperty.Register("StartFilesAvailable", typeof(bool), typeof(ExtensionForm));

		/// <summary>
		/// Gets or sets the extension file.
		/// </summary>
		/// <value>The extension file.</value>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		public ExtensionFile ExtensionFile
		{
			get { return (ExtensionFile)GetValue(ExtensionFileProperty); }
			set { SetValue(ExtensionFileProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ExtensionFile.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ExtensionFileProperty =
			DependencyProperty.Register("ExtensionFile", typeof(ExtensionFile), typeof(ExtensionForm));


		/// <summary>
		/// Gets or sets a value indicating whether [extension file loaded].
		/// </summary>
		/// <value><c>true</c> if [extension file loaded]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		public bool ExtensionFileLoaded
		{
			get { return (bool)GetValue(ExtensionFileLoadedProperty); }
			set { SetValue(ExtensionFileLoadedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ExtensionFileLoaded.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ExtensionFileLoadedProperty =
			DependencyProperty.Register("ExtensionFileLoaded", typeof(bool), typeof(ExtensionForm));


		/// <summary>
		/// Handles the Click event of the ButtonCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Handles the Click event of the ButtonLoadExtension control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		private void ButtonLoadExtension_Click(object sender, RoutedEventArgs e)
		{
			string file = getFilePath(MLifter.SettingsManager.Properties.Resources.EXTENSION_FILEFILTER);
			if (!String.IsNullOrEmpty(file))
				OpenExtensionFile(file);
		}

		/// <summary>
		/// Handles the Click event of the ButtonBrowseDataFile control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		private void ButtonBrowseDataFile_Click(object sender, RoutedEventArgs e)
		{
			string file = getFilePath(MLifter.SettingsManager.Properties.Resources.EXTENSION_DATAFILEFILTER);
			if (!String.IsNullOrEmpty(file))
			{
				using (FileStream ExtensionDataFileStream = new FileStream(file, FileMode.Open))
				{
					ExtensionFile.Extension.Data = ExtensionDataFileStream;
					RefreshExtensionDataFiles();
				}
			}
		}

		/// <summary>
		/// Refreshes the extension data files.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		private void RefreshExtensionDataFiles()
		{
			object selectedStartfile = ComboBoxStartFile.SelectedItem;
			ExtensionDataFiles.Clear();
			ExtensionDataFiles.Add(String.Empty);

			if (ExtensionFile != null && ExtensionFile.Extension != null)
			{
				using (Stream stream = ExtensionFile.Extension.Data)
				{
					if (stream != null)
						MLifter.DAL.Tools.ZipHelper.GetZipContent(stream, false).ForEach(i => this.ExtensionDataFiles.Add(i));
				}

				TextBlockDataFile.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
			}

			ComboBoxStartFile.SelectedItem = selectedStartfile;
		}

		/// <summary>
		/// Opens the extension file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		private void OpenExtensionFile(string filename)
		{
			ExtensionFile extFile = new ExtensionFile(filename);
			extFile.Open(LoginForm.OpenLoginForm);
			if (extFile.Extension.Version.Major < 1)
				extFile.Extension.Version = new Version(1, 0, 0);
			ExtensionFile = extFile;
		}

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev07, 2009-07-06</remarks>
		private string getFilePath(string filter)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = filter;
			if (ofd.ShowDialog().Value)
				return ofd.FileName;

			return string.Empty;
		}

		/// <summary>
		/// Handles the Click event of the ButtonNewExtension control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		private void ButtonNewExtension_Click(object sender, RoutedEventArgs e)
		{
			Clear();
			System.Windows.Forms.SaveFileDialog DialogSave = new System.Windows.Forms.SaveFileDialog();
			DialogSave.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			DialogSave.Filter = MLifter.SettingsManager.Properties.Resources.EXTENSION_FILEFILTER;
			DialogSave.FilterIndex = 1;
			DialogSave.RestoreDirectory = true;
			if (DialogSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string file = DialogSave.FileName;
				ExtensionFile extensionFile = new ExtensionFile(file);
				extensionFile.Create();
				OpenExtensionFile(file);
			}
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		private void Clear()
		{
			if (ExtensionFile != null)
			{
				ExtensionFile.Dispose();
				ExtensionFile = null;
			}

			TextBoxName.Text = string.Empty;
			TextBoxVersion.Text = "1.0.0";

			RefreshExtensionDataFiles();
		}

		/// <summary>
		/// Gets the extension action executions.
		/// </summary>
		/// <value>The extension action execution.</value>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		public List<ExtensionActionExecution> ExtensionActionExecutions
		{
			get
			{
				return new List<ExtensionActionExecution>(Enum.GetValues(typeof(ExtensionActionExecution)).OfType<ExtensionActionExecution>());
			}
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>The actions.</value>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		public IList<ExtensionActionDisplay> Actions
		{
			get
			{
				List<ExtensionActionDisplay> actions = new List<ExtensionActionDisplay>();

				if (ExtensionFile != null && ExtensionFile.Extension != null)
					foreach (ExtensionActionKind kind in Enum.GetValues(typeof(ExtensionActionKind)).OfType<ExtensionActionKind>())
						actions.Add(new ExtensionActionDisplay(ExtensionFile.Extension, kind));

				return actions;
			}
		}

		/// <summary>
		/// Handles the SelectionChanged event of the ComboBoxExtensionTyp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void ComboBoxExtensionTyp_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ExtensionFile != null)
				ExtensionFile.Extension.Type = (ExtensionType)ComboBoxExtensionTyp.SelectedItem;
		}

		/// <summary>
		/// Handles the SelectionChanged event of the ComboBoxStartFile control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void ComboBoxStartFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ExtensionFile != null)
				ExtensionFile.Extension.StartFile = (string)ComboBoxStartFile.SelectedItem;
		}

		/// <summary>
		/// Handles the Click event of the HyperlinkHelp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void HyperlinkHelp_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(HelpUrl.LearningModulesExtensions);
		}
	}

	/// <summary>
	/// A display mash-up for the DAL class ExtensionAction.
	/// </summary>
	/// <remarks>Documented by Dev02, 2009-07-10</remarks>
	public class ExtensionActionDisplay
	{
		IExtension extension;
		ExtensionActionKind kind;

		public ExtensionActionDisplay(IExtension extension, ExtensionActionKind kind)
		{
			this.extension = extension;
			this.kind = kind;
		}

		public ExtensionActionKind Kind
		{
			get { return kind; }
		}

		public ExtensionActionExecution Execution
		{
			get
			{
				foreach (ExtensionAction action in extension.Actions)
					if (action.Kind == kind)
						return action.Execution;

				return ExtensionActionExecution.Never;
			}
			set
			{
				foreach (ExtensionAction action in extension.Actions)
					if (action.Kind == kind)
						extension.Actions.Remove(action);

				ExtensionAction newaction = new ExtensionAction() { Kind = kind, Execution = value };
				extension.Actions.Add(newaction);
			}
		}

	}
}
