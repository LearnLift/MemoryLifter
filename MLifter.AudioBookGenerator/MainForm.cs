using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MLifterAudioBookGenerator.Forms;
using System.Xml.Serialization;

using MLifter.AudioTools.Codecs;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;

namespace MLifterAudioBookGenerator
{
	public partial class MainForm : Form
	{
		LogTraceListener tracelistener;
		Codecs codecs = new Codecs();

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-03-10</remarks>
		public MainForm()
		{
			InitializeComponent();

			//upgrade settings
			if (!Properties.Settings.Default.Upgraded)
			{
				try
				{
					Properties.Settings.Default.Upgrade();
				}
				catch { }
				Properties.Settings.Default.Upgraded = true;
				Properties.Settings.Default.Save();
			}

			//load recent window size from settings
			Size windowsize = Properties.Settings.Default.WindowSize;
			if (!windowsize.IsEmpty)
				this.Size = windowsize;

			//add tracelistener
			tracelistener = new LogTraceListener();
			tracelistener.LogMessageAdded += new EventHandler(tracelistener_LogMessageAdded);
			System.Diagnostics.Trace.Listeners.Add(tracelistener);

			BusinessLayer.WorkingThreadFinished += new EventHandler(BusinessLayer_WorkingThreadFinished);
			BusinessLayer.ProgressChanged += new BusinessLayer.ProgressChangedEventHandler(BusinessLayer_ProgressChanged);
		}

		/// <summary>
		/// Handles the LogMessageAdded event of the tracelistener control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void tracelistener_LogMessageAdded(object sender, EventArgs e)
		{
			if (textBoxLog.InvokeRequired)
			{
				textBoxLog.BeginInvoke(new EventHandler(tracelistener_LogMessageAdded), sender, e);
			}
			else
			{
				while (tracelistener.LogMessages.Count > 0 && textBoxLog != null && !textBoxLog.IsDisposed)
				{
					 textBoxLog.AppendText(tracelistener.LogMessages.Dequeue());
				}
			}
		}


		/// <summary>
		/// Handles the Load event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-10</remarks>
		private void MainForm_Load(object sender, EventArgs e)
		{
			//load codecs
			codecs.XMLString = Properties.Settings.Default.CodecSettings;

			//load recent LearningModule from settings
			if (!string.IsNullOrEmpty(Properties.Settings.Default.RecentLearningModule) && File.Exists(Properties.Settings.Default.RecentLearningModule))
				textBoxLearningModule.Text = Properties.Settings.Default.RecentLearningModule;

			//load recent AudioBook Path from settings
			if (!string.IsNullOrEmpty(Properties.Settings.Default.RecentAudioBook))
			{
				FileInfo recentaudiobook = new FileInfo(Properties.Settings.Default.RecentAudioBook);

				textBoxAudiobook.Text = recentaudiobook.FullName;
			}

			GenerateAvailableFields();

			//use the events to reset to "default" state
			BusinessLayer_WorkingThreadFinished(null, EventArgs.Empty);
			ProgressChangedEventArgs args = new ProgressChangedEventArgs();
			args.enableProgressReporting = false;
			BusinessLayer_ProgressChanged(null, args);

			//the following line is required for the login form to work
			MLifter.BusinessLayer.LearningModulesIndex learningModules = new MLifter.BusinessLayer.LearningModulesIndex(
				Path.Combine(Application.StartupPath, Properties.Settings.Default.ConfigPath),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.ConfigurationFolder),
				(MLifter.DAL.GetLoginInformation)MLifter.Controls.LoginForm.OpenLoginForm,
				(MLifter.DAL.DataAccessErrorDelegate)delegate { return; }, String.Empty);
		}

		/// <summary>
		/// Generates the available fields.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-03-29</remarks>
		private void GenerateAvailableFields()
		{
			List<MediaField> mediafields = new List<MediaField>();
			//add standard fields
			mediafields.Add(new MediaField(MediaField.SideEnum.Question, false));
			mediafields.Add(new MediaField(MediaField.SideEnum.Answer, false));
			mediafields.Add(new MediaField(MediaField.SideEnum.Question, true));
			mediafields.Add(new MediaField(MediaField.SideEnum.Answer, true));
			//add silence fields
			mediafields.Add(new MediaField(0.5));
			mediafields.Add(new MediaField(1));
			mediafields.Add(new MediaField(2));
			mediafields.Add(new MediaField(5));
			mediafields.Add(new MediaField(0)); //custom silence field

			listViewAvailableFields.Items.Clear();
			foreach (MediaField mediafield in mediafields)
			{
				ListViewItem lvi = new ListViewItem();
				lvi.Text = mediafield.ToString();
				lvi.Tag = mediafield;
				listViewAvailableFields.Items.Add(lvi);
			}

			//add default playback sequence fields
			listViewPlaybackSequence.Items.Clear();
			listViewPlaybackSequence.Items.Add((ListViewItem)listViewAvailableFields.Items[0].Clone());
			listViewPlaybackSequence.Items.Add((ListViewItem)listViewAvailableFields.Items[1].Clone());
			listViewPlaybackSequence.Items.Add((ListViewItem)listViewAvailableFields.Items[5].Clone());
		}

		/// <summary>
		/// Handles the Click event of the buttonBrowseLearningModule control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-17</remarks>
		private void buttonBrowseLearningModule_Click(object sender, EventArgs e)
		{
			OpenFileDialog filedialog = new OpenFileDialog();
			if (File.Exists(textBoxLearningModule.Text))
				filedialog.FileName = textBoxLearningModule.Text;
			filedialog.Filter = Properties.Resources.SOURCE_FILEFILTER;
			if (filedialog.ShowDialog() == DialogResult.OK)
			{
				textBoxLearningModule.Text = filedialog.FileName;
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonBrowseAudiobook control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-17</remarks>
		private void buttonBrowseAudiobook_Click(object sender, EventArgs e)
		{
			SaveFileDialog filedialog = new SaveFileDialog();

			if (!string.IsNullOrEmpty(textBoxLearningModule.Text)) //fix for [MLA-1270] unhandled exception when clicking onto file open button            
			{
				try
				{
					filedialog.InitialDirectory = Path.GetDirectoryName(textBoxLearningModule.Text);
					filedialog.FileName = Path.GetFileNameWithoutExtension(textBoxLearningModule.Text);
				}
				catch
				{ }
			}

			//prepare filter list
			List<string> extensions = new List<string>();
			string filter = string.Format("{0} (*{1})|*{1}", Properties.Resources.AUDIO_WAVE_NAME, Properties.Resources.AUDIO_WAVE_EXTENSION);
			extensions.Add(Properties.Resources.AUDIO_WAVE_EXTENSION);

			foreach (Codec codec in codecs)
			{
				if (codec.CanEncode)
				{
					filter += string.Format("|{0} (*{1})|*{1}", codec.name, codec.extension);
					extensions.Add(codec.extension.ToLowerInvariant());
				}
			}
			filedialog.Filter = filter;

			string recentextension = Path.GetExtension(textBoxAudiobook.Text);
			if (recentextension != string.Empty && extensions.Contains(recentextension.ToLowerInvariant()))
				filedialog.FilterIndex = extensions.IndexOf(recentextension.ToLowerInvariant()) + 1;

			//show the dialog
			if (filedialog.ShowDialog() == DialogResult.OK)
			{
				textBoxAudiobook.Text = filedialog.FileName;
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonStart control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-17</remarks>
		private void buttonStart_Click(object sender, EventArgs e)
		{
			if (BusinessLayer.WorkingThreadActive)
			{
				BusinessLayer.AbortWorkingThread();
				return;
			}

			textBoxLog.Clear();
			tracelistener.LogMessages.Clear();

			//check if input file exists
			if (!File.Exists(textBoxLearningModule.Text))
			{
				BusinessLayer.AddLog("The specified learning module file cannot be found");
				return;
			}

			//check if any audio fields are selected
			if (listViewPlaybackSequence.Items.Count == 0)
			{
				BusinessLayer.AddLog("Please select at least one field for the playback sequence");
				return;
			}

			IDictionary dictionary = null;
			try
			{
				AudioBookOptions options = new AudioBookOptions();

				//add mono/stereo selection to options
				options.Stereo = radioButtonStereo.Checked;

				//add selected fields to the options
				foreach (ListViewItem lvi in listViewPlaybackSequence.Items)
				{
					MediaField mediafield = lvi.Tag as MediaField;
					if (mediafield != null)
						options.MediaFields.Add(mediafield);
				}

				//check for valid file paths
				FileInfo dictionaryfile = null, audiobook = null;
				try
				{
					dictionaryfile = new FileInfo(textBoxLearningModule.Text);
					audiobook = new FileInfo(textBoxAudiobook.Text);
					if (string.IsNullOrEmpty(audiobook.Extension))
					{
						BusinessLayer.AddLog("Please enter a valid extension for the target audiobook file");
						return;
					}
				}
				catch
				{
					BusinessLayer.AddLog("The specified file paths are not valid. Please check your input");
					return;
				}

				//save file paths to settings as recent files
				Properties.Settings.Default.RecentLearningModule = dictionaryfile.FullName;
				Properties.Settings.Default.RecentAudioBook = audiobook.FullName;
				Properties.Settings.Default.Save();

				//open learning module and start audio book generation
				try
				{
					ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.MsSqlCe, dictionaryfile.FullName);
					MLifter.DAL.Interfaces.IUser user = UserFactory.Create((GetLoginInformation)MLifter.Controls.LoginForm.OpenLoginForm, css, (DataAccessErrorDelegate)delegate { return; }, this);
					css.LmId = User.GetIdOfLearningModule(dictionaryfile.FullName, user);
					user.ConnectionString = css;
					dictionary = user.Open();
				}
				catch (System.IO.IOException)
				{
					BusinessLayer.AddLog("The Learning Module file could not be accessed. Please make sure that it is not open within another program (e.g. MemoryLifter)");
					return;
				}
				catch (System.Xml.XmlException)
				{
					BusinessLayer.AddLog("The Learning Module file does not seem to be in a valid format");
					return;
				}
				catch (ProtectedLearningModuleException)
				{
					BusinessLayer.AddLog("The Learning Module could not be opened: It is content protected");
					return;
				}
				catch (Exception exp)
				{
					BusinessLayer.AddLog("The Learning Module file could not be opened:" + Environment.NewLine + exp.Message);
					return;
				}
				buttonStart.Text = "Cancel";
				BusinessLayer.GenerateAudioBook(dictionary, audiobook, options, codecs);
			}
			catch (Exception exp)
			{
				BusinessLayer.AddLog("Audio book generation exception happened: " + Environment.NewLine + exp.ToString());
				return;
			}
			finally
			{
				if (dictionary != null)
					dictionary.Dispose();
			}
		}

		/// <summary>
		/// Handles the WorkingThreadFinished event of the BusinessLayer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-20</remarks>
		void BusinessLayer_WorkingThreadFinished(object sender, EventArgs e)
		{
			buttonStart.Text = "Create audiobook";
		}

		/// <summary>
		/// Handles the ProgressChanged event of the BusinessLayer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifterAudioBookGenerator.ProgressChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-20</remarks>
		void BusinessLayer_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Visible = e.enableProgressReporting;
			progressBar.Value = e.percentProgress > 0 ? Convert.ToInt32(e.percentProgress) : 0;
		}

		#region ListView Events
		/// <summary>
		/// Handles the ItemDrag event of the listViewPlaybackSequence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.ItemDragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-29</remarks>
		private void listViewPlaybackSequence_ItemDrag(object sender, ItemDragEventArgs e)
		{
			listViewPlaybackSequence.DoDragDrop(listViewPlaybackSequence.SelectedItems, DragDropEffects.Move);
		}

		/// <summary>
		/// Handles the DragEnter event of the listViewPlaybackSequence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-29</remarks>
		private void listViewPlaybackSequence_DragEnter(object sender, DragEventArgs e)
		{
			int len = e.Data.GetFormats().Length - 1;
			int i;
			for (i = 0; i <= len; i++)
			{
				if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
				{
					if (e.AllowedEffect == DragDropEffects.Move)
					{
						//The data from the drag source is moved to the target.	
						e.Effect = DragDropEffects.Move;
					}
				}
			}

		}

		/// <summary>
		/// Handles the DragDrop event of the listViewPlaybackSequence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-29</remarks>
		private void listViewPlaybackSequence_DragDrop(object sender, DragEventArgs e)
		{
			//Return if the items are not selected in the ListView control.
			if (listViewPlaybackSequence.SelectedItems.Count == 0)
			{
				return;
			}
			//Returns the location of the mouse pointer in the ListView control.
			Point cp = listViewPlaybackSequence.PointToClient(new Point(e.X, e.Y));
			//Obtain the item that is located at the specified location of the mouse pointer.
			ListViewItem dragToItem = listViewPlaybackSequence.GetItemAt(cp.X, cp.Y);
			if (dragToItem == null)
			{
				return;
			}
			//Obtain the index of the item at the mouse pointer.
			int dragIndex = dragToItem.Index;
			ListViewItem[] sel = new ListViewItem[listViewPlaybackSequence.SelectedItems.Count];
			for (int i = 0; i <= listViewPlaybackSequence.SelectedItems.Count - 1; i++)
			{
				sel[i] = listViewPlaybackSequence.SelectedItems[i];
			}
			for (int i = 0; i < sel.GetLength(0); i++)
			{
				//Obtain the ListViewItem to be dragged to the target location.
				ListViewItem dragItem = sel[i];
				int itemIndex = dragIndex;
				if (itemIndex == dragItem.Index)
				{
					return;
				}
				if (dragItem.Index < itemIndex)
					itemIndex++;
				else
					itemIndex = dragIndex + i;
				//Insert the item at the mouse pointer.
				ListViewItem insertItem = (ListViewItem)dragItem.Clone();
				listViewPlaybackSequence.Items.Insert(itemIndex, insertItem);
				//Removes the item from the initial location while 
				//the item is moved to the new location.
				listViewPlaybackSequence.Items.Remove(dragItem);
			}
		}

		/// <summary>
		/// Handles the MouseDoubleClick event of the listViewAvailableFields control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-29</remarks>
		private void listViewAvailableFields_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewItem item = listViewAvailableFields.GetItemAt(e.X, e.Y);
			if (item == null || !item.Selected)
				return;

			AddMediaField(item);
		}

		/// <summary>
		/// Inputs the length of the silence.
		/// </summary>
		/// <param name="mediafield">The mediafield.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private MediaField InputSilenceLength(MediaField mediafield)
		{
			if (mediafield != null && mediafield.Type == MediaField.TypeEnum.Silence && mediafield.SilenceDuration == 0)
			{
				SilenceLength lengthform = new SilenceLength();
				lengthform.ShowDialog();
				if (lengthform.DialogResult == DialogResult.OK)
					mediafield = new MediaField(lengthform.Length);
				lengthform.Close();
			}
			return mediafield;
		}

		/// <summary>
		/// Handles the MouseDoubleClick event of the listViewPlaybackSequence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-29</remarks>
		private void listViewPlaybackSequence_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewItem item = listViewPlaybackSequence.GetItemAt(e.X, e.Y);
			if (item == null || !item.Selected)
				return;

			listViewPlaybackSequence.Items.Remove(item);
		}

		/// <summary>
		/// Handles the ItemSelectionChanged event of the listViewPlaybackSequence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.ListViewItemSelectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void listViewPlaybackSequence_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			buttonDelete.Enabled = listViewPlaybackSequence.SelectedItems.Count > 0;
		}

		/// <summary>
		/// Handles the ItemSelectionChanged event of the listViewAvailableFields control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.ListViewItemSelectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void listViewAvailableFields_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			buttonAdd.Enabled = listViewAvailableFields.SelectedItems.Count > 0;
		}

		#endregion

		/// <summary>
		/// Handles the Click event of the buttonDelete control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (listViewPlaybackSequence.SelectedItems.Count > 0)
			{
				List<ListViewItem> deleteitems = new List<ListViewItem>();
				foreach (ListViewItem lvi in listViewPlaybackSequence.SelectedItems)
					deleteitems.Add(lvi);

				foreach (ListViewItem lvi in deleteitems)
					listViewPlaybackSequence.Items.Remove(lvi);
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonAdd control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void buttonAdd_Click(object sender, EventArgs e)
		{
			if (listViewAvailableFields.SelectedItems.Count > 0)
			{
				foreach (ListViewItem lvi in listViewAvailableFields.SelectedItems)
					AddMediaField(lvi);
			}
		}

		/// <summary>
		/// Adds a media field to the playback sequence listview.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void AddMediaField(ListViewItem item)
		{
			item = (ListViewItem)item.Clone();

			MediaField mediafield = item.Tag as MediaField;
			if (mediafield != null && mediafield.Type == MediaField.TypeEnum.Silence && mediafield.SilenceDuration == 0)
			{
				//ask the user to enter the desired duration of the silence
				mediafield = InputSilenceLength(mediafield);
				if (mediafield.SilenceDuration == 0)
					return;
				else
				{
					item.Tag = mediafield;
					item.Text = mediafield.ToString();
				}
			}

			listViewPlaybackSequence.Items.Add(item);
		}

		/// <summary>
		/// Handles the FormClosed event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			//save the current window size
			Properties.Settings.Default.WindowSize = this.Size;

			//save the codec list
			Properties.Settings.Default.CodecSettings = codecs.XMLString;
			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Handles the Resize event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-03-30</remarks>
		private void MainForm_Resize(object sender, EventArgs e)
		{
			//align the add button to the middle of the window
			Point buttonlocation = buttonAdd.Location;
			buttonlocation.X = Convert.ToInt32(1.0 * groupBoxOptions.Width / 2 - buttonAdd.Width / 2);
			buttonAdd.Location = buttonlocation;

			//resize the two groupboxes, so that they use half of the width each
			groupBoxAvailable.Width = Convert.ToInt32(1.0 * groupBoxOptions.Width / 2 - buttonAdd.Width / 2 - 20);
			groupBoxPlaybackSequence.Width = Convert.ToInt32(1.0 * groupBoxOptions.Width / 2 - buttonAdd.Width / 2 - 20);
			Point groupboxlocation = groupBoxPlaybackSequence.Location;
			groupboxlocation.X = groupBoxOptions.Width - groupBoxPlaybackSequence.Width - 5;
			groupBoxPlaybackSequence.Location = groupboxlocation;
		}

		/// <summary>
		/// Handles the Click event of the settingsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-10</remarks>
		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowCodecSettings();
		}

		/// <summary>
		/// Shows the codec settings form.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-04-15</remarks>
		private void ShowCodecSettings()
		{
			CodecSettings settingsForm = new CodecSettings();
			settingsForm.Codecs = codecs;
			settingsForm.ShowEncoder = Properties.Settings.Default.ShowEncodingWindow;
			settingsForm.ShowDecoder = Properties.Settings.Default.ShowDecodingWindow;
			settingsForm.MinimizeWindows = Properties.Settings.Default.MimimizeWindows;

			if (settingsForm.ShowDialog() == DialogResult.OK)
			{
				codecs = settingsForm.Codecs;
				Properties.Settings.Default.ShowEncodingWindow = settingsForm.ShowEncoder;
				Properties.Settings.Default.ShowDecodingWindow = settingsForm.ShowDecoder;
				Properties.Settings.Default.MimimizeWindows = settingsForm.MinimizeWindows;
				Properties.Settings.Default.Save();
			}
		}

		/// <summary>
		/// Handles the Click event of the exitToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-10</remarks>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Handles the FormClosing event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-10</remarks>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (BusinessLayer.WorkingThreadActive)
			{
				BusinessLayer.AbortWorkingThread();
			}
		}

		/// <summary>
		/// Handles the Click event of the howToToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-10</remarks>
		private void howToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(MLifterAudioBookGenerator.Properties.Resources.HOWTO_URL);
			}
			catch (Exception exp)
			{
				Trace.WriteLine("How to could not be executed: " + exp.ToString());
			}
		}

		/// <summary>
		/// Handles the Click event of the aboutToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-10</remarks>
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog();
			about.ShowDialog();
		}

		/// <summary>
		/// Handles the Shown event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-11</remarks>
		private void MainForm_Shown(object sender, EventArgs e) { }
	}

	/// <summary>
	/// A logging Trace Listener.
	/// </summary>
	public class LogTraceListener : TraceListener
	{
		/// <summary>
		/// The current log message queue.
		/// </summary>
		public Queue<string> LogMessages = new Queue<string>();

		/// <summary>
		/// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public override void Write(string message)
		{
			AddMessage(message);
		}

		/// <summary>
		/// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public override void WriteLine(string message)
		{
			AddMessage(message + Environment.NewLine);
		}

		/// <summary>
		/// Adds the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void AddMessage(string message)
		{
			LogMessages.Enqueue(message);
			if (LogMessageAdded != null)
				LogMessageAdded(this, new EventArgs());
		}

		/// <summary>
		/// Occurs when [log changed].
		/// </summary>
		public event EventHandler LogMessageAdded;

	}

}