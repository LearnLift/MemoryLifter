using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MLifter.AudioTools.Codecs;
using MLifter.Controls;
using MLifter.Recorder.Properties;

namespace MLifter.AudioTools
{
	public partial class MainForm : Form
	{
		# region Variables
		private bool updating = false;
		private bool selecting = false;
		private bool collapsed = false;
		private bool multiselecting = false;

		private bool loading;
		private bool Loading
		{
			get { return loading; }
			set { loading = value; }
		}

		private string actualFilename = string.Empty;
		private LastCommand lastCommand;

		private Thread updatingThread;

		private TreeNode actualNode;
		private string actualText = string.Empty;

		private Settings settings;
		private Recorder recorder;
		private AudioPlayer player = new AudioPlayer();
		private DictionaryManagement dictionaryManager = new DictionaryManagement();
		# endregion
		# region Constructor and basic methods
		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-02</remarks>
		public MainForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the Load event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void MainForm_Load(object sender, EventArgs e)
		{
			//the following line is required for the login form to work
			MLifter.BusinessLayer.LearningModulesIndex learningModules = new MLifter.BusinessLayer.LearningModulesIndex(
				Path.Combine(Application.StartupPath, MLifter.Recorder.Properties.Settings.Default.ConfigPath),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MLifter.Recorder.Properties.Settings.Default.ConfigurationFolder),
				(MLifter.DAL.GetLoginInformation)MLifter.Controls.LoginForm.OpenLoginForm,
				(MLifter.DAL.DataAccessErrorDelegate)delegate { return; }, String.Empty);

			groupBoxSelect.Visible = numPadControl.AdvancedView;

			# region Load settings
			settings = new Settings();
			settings.KeyFunctions = numPadControl.KeyFunctions;
			settings.KeyboardFunctions = numPadControl.KeyboardFunctions;
			settings.PresenterKeyFunctions = numPadControl.PresenterKeyFunctions;

			if (File.Exists(CONSTANTS.SETTINGS_FILENAME))
			{
				LoadSettings();

				if (File.Exists(settings.Dictionary) && TaskDialog.ShowCommandBox(MLifter.Recorder.Properties.Resources.RELOAD_LAST_DIC_CAPTION,
					MLifter.Recorder.Properties.Resources.RELOAD_LAST_DIC_TEXT, settings.Dictionary, 
					MLifter.Recorder.Properties.Resources.RELOAD_LAST_DIC_COMMANDS, false) == 0)
				{
					try
					{
						dictionaryManager.LoadDictionary(settings.Dictionary);
					}
					catch
					{
						if (!OpenDictionary())
							Environment.Exit(-1);
					}
				}
				else
				{
					if (!OpenDictionary())
						Environment.Exit(-1);
				}
			}
			else
			{
				if (!OpenDictionary())
					Environment.Exit(-1);
			}
			Stop();

			updatingThread = new Thread(new ThreadStart(UpdateListView));
			updatingThread.Start();
			SelectAllCards();
			# endregion
			Stop();
			UpdateSettings();

			treeViewCards.Focus();
			numPadControl.InstallHook();

			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(buttonAllCards, "Select all cards");
			toolTip.SetToolTip(buttonNoCards, "Deselect all cards");
			toolTip.SetToolTip(buttonSelectIncomplete, "Select all cards, where at least one part is missing");
			toolTip.SetToolTip(buttonSelectInverse, "Inverse the selection");
		}

		/// <summary>
		/// Opens a dictionary.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private bool OpenDictionary()
		{
			try
			{
				Stop();

				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Dictionaries (*.mlm)|*.mlm";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					CardColors = new Dictionary<int, Color>();
					dictionaryManager.LoadDictionary(openFileDialog.FileName);

					LanguageForm languageForm = new LanguageForm(dictionaryManager);
					languageForm.ShowDialog();

					settings.RecordAnswer = true;
					settings.RecordAnswerExample = true;
					settings.RecordQuestion = true;
					settings.RecordQuestionExample = true;

					settings.Dictionary = openFileDialog.FileName;
					settings.ActualCard = 0;
					settings.ActualStep = 0;
					settings.Side = (settings.RecordQuestion || settings.RecordQuestionExample) ? MLifter.DAL.Interfaces.Side.Question : MLifter.DAL.Interfaces.Side.Answer;
					settings.Save(CONSTANTS.SETTINGS_FILENAME);

					return true;
				}
				else
					return false;
			}
			catch (Exception exp)
			{
				Trace.WriteLine("Exception in OpenDictionary: " + exp.ToString());
				return false;
			}
		}

		/// <summary>
		/// Updates the list view.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void UpdateListView()
		{
			if (!treeViewCards.InvokeRequired)
			{
				Loading = true;
				groupBoxFontSize.Enabled = false;
				groupBoxPosition.Enabled = false;
				groupBoxSelect.Enabled = false;
				numPadControl.FunctionsEnabled = false;

				loadingControl.Visible = true;
				loadingControl.BringToFront();
				treeViewCards.Nodes.Clear();

				loadingControl.Message = "Loading...";
				this.Refresh();

				using (Graphics graphics = treeViewCards.CreateGraphics())
				{
					int cardCount = dictionaryManager.CardCount;
					TreeNode[] nodes = new TreeNode[cardCount];
					for (int i = 0; i < cardCount; i++)
					{
						if ((i + 1) % 10 == 0)
							loadingControl.Message = "Loading card " + (i + 1) + " / " + dictionaryManager.CardCount;

						nodes[i] = new TreeNode((dictionaryManager.GetWord(i, MLifter.DAL.Interfaces.Side.Question).Length > 0 && settings.RecordQuestion ?
							dictionaryManager.GetWord(i, MLifter.DAL.Interfaces.Side.Question) : (dictionaryManager.GetWord(i, MLifter.DAL.Interfaces.Side.Answer).Length > 0 ?
							dictionaryManager.GetWord(i, MLifter.DAL.Interfaces.Side.Answer) : "Card " + i)));
						nodes[i].BackColor = GetCardColor(i);
						nodes[i].Checked = true;
						nodes[i].Tag = "Card";

						if (graphics.MeasureString(nodes[i].Text, treeViewCards.Font).Width + 50 > treeViewCards.Width)
						{
							string text = nodes[i].Text;

							while (graphics.MeasureString(text, treeViewCards.Font).Width + 50 > treeViewCards.Width)
							{
								text = text.Remove(text.Length - 4, 4);
								text = text.Insert(text.Length, "...");
							}

							nodes[i].Text = text;
						}
					}
					treeViewCards.Nodes.AddRange(nodes);
				}

				UpdateSettings();
				loadingControl.Visible = false;

				groupBoxFontSize.Enabled = true;
				groupBoxPosition.Enabled = true;
				groupBoxSelect.Enabled = true;
				numPadControl.FunctionsEnabled = true;
				Loading = false;
			}
			else
				treeViewCards.Invoke((EmtyDelegate)UpdateListView);
		}

		/// <summary>
		/// Gets the current encoder.
		/// </summary>
		/// <value>The current encoder.</value>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-04-15</remarks>
		private Codec CurrentEncoder
		{
			get
			{
				Codecs.Codecs codecs = new MLifter.AudioTools.Codecs.Codecs();
				codecs.XMLString = settings.CodecSettings;

				foreach (Codec codec in codecs.encodeCodecs.Values)
				{
					if (codec.ToString() == settings.SelectedEncoder)
						return codec;
				}

				return null;
			}
		}

		/// <summary>
		/// Refreshes the list view.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-27</remarks>
		private void RefreshListView()
		{
			foreach (TreeNode node in treeViewCards.Nodes)
				node.BackColor = GetCardColor(node.Index);
		}

		Dictionary<int, Color> CardColors = new Dictionary<int, Color>();
		/// <summary>
		/// Gets the color of the card.
		/// </summary>
		/// <param name="cardIndex">Index of the card.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private Color GetCardColor(int cardIndex)
		{
			if (CardColors.ContainsKey(cardIndex))
				return CardColors[cardIndex];

			int i = 0;

			if (settings.RecordAnswer && dictionaryManager.HasMediaFile(cardIndex, false, MLifter.DAL.Interfaces.Side.Answer))
				i++;
			if (settings.RecordAnswerExample && dictionaryManager.HasMediaFile(cardIndex, true, MLifter.DAL.Interfaces.Side.Answer))
				i++;
			if (settings.RecordQuestion && dictionaryManager.HasMediaFile(cardIndex, false, MLifter.DAL.Interfaces.Side.Question))
				i++;
			if (settings.RecordQuestionExample && dictionaryManager.HasMediaFile(cardIndex, true, MLifter.DAL.Interfaces.Side.Question))
				i++;

			int j = 0;
			if (settings.RecordAnswer && dictionaryManager.GetWord(cardIndex, MLifter.DAL.Interfaces.Side.Answer) != "")
				j++;
			if (settings.RecordAnswerExample && dictionaryManager.GetSentence(cardIndex, MLifter.DAL.Interfaces.Side.Answer) != "")
				j++;
			if (settings.RecordQuestion && dictionaryManager.GetWord(cardIndex, MLifter.DAL.Interfaces.Side.Question) != "")
				j++;
			if (settings.RecordQuestionExample && dictionaryManager.GetSentence(cardIndex, MLifter.DAL.Interfaces.Side.Question) != "")
				j++;

			if (i == 0)
			{
				CardColors[cardIndex] = STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
				return STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
			}
			else if (i < j)
			{
				CardColors[cardIndex] = STANDARD_APPEARANCE.COLOR_CARD_PARTS;
				return STANDARD_APPEARANCE.COLOR_CARD_PARTS;
			}
			else
			{
				CardColors[cardIndex] = STANDARD_APPEARANCE.COLOR_CARD_COMPLETE;
				return STANDARD_APPEARANCE.COLOR_CARD_COMPLETE;
			}
		}

		/// <summary>
		/// Loads the settings.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-08</remarks>
		private void LoadSettings()
		{
			if (File.Exists(CONSTANTS.SETTINGS_FILENAME))
			{
				settings.Load(CONSTANTS.SETTINGS_FILENAME);

				switch (settings.FontSize)
				{
					case FontSizes.Small:
						radioButtonFontSizeSmall.Checked = true;
						break;
					case FontSizes.Medium:
						radioButtonFontSizeMedium.Checked = true;
						break;
					case FontSizes.Large:
						radioButtonFontSizeLarge.Checked = true;
						break;
					case FontSizes.Automatic:
						radioButtonFontSizeAutomatic.Checked = true;
						break;
					default:
						radioButtonFontSizeMedium.Checked = true;
						break;
				}
			}

			numPadControl.KeyboardLayout = settings.KeyboardLayout;
			numPadControl.AdvancedView = settings.AdvancedView;
			numPadControl.PresenterActivated = settings.PresenterActivated;
			numPadControl.NumLockSwitchSleepTime = settings.NumLockSwitchSleepTime;
		}

		/// <summary>
		/// Handles the Click event of the openDictionaryToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-06</remarks>
		private void openDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.DoEvents(); //necessary to enable immediate repainting
			if (Loading)
				updatingThread.Abort();

			if (OpenDictionary()) //[MLR-1295] Update only in case a new LM was opened
			{
				UpdateSettings();

				updatingThread = new Thread(new ThreadStart(UpdateListView));
				updatingThread.Start();
			}
		}

		/// <summary>
		/// Handles the Click event of the settingsToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-06</remarks>
		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.DoEvents(); //necessary to enable immediate repainting
			SettingsForm settingsForm = new SettingsForm(settings, numPadControl, dictionaryManager);

			bool mediaFileAvailableOld = numPadControl.MediaFileAvailable;
			numPadControl.MediaFileAvailable = true;
			settingsForm.ShowDialog();
			numPadControl.MediaFileAvailable = mediaFileAvailableOld;

			settings.Save(CONSTANTS.SETTINGS_FILENAME);
			numPadControl.PresenterActivated = settings.PresenterActivated;

			lastCommand = (lastCommand == LastCommand.Next) ? LastCommand.Back : LastCommand.Next;

			RefreshListView();
			UpdateSettings();
		}

		/// <summary>
		/// Handles the Click event of the aboutToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog();
			about.ShowDialog();
		}

		/// <summary>
		/// Handles the Click event of the exitToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-06</remarks>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Handles the FormClosing event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (MessageBox.Show(Resources.CLOSE_TEXT, Resources.CLOSE_CAPTION, 
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Visible = false;

				if (numPadControl.CurrentState == Function.Play || numPadControl.CurrentState == Function.Record)
				{
					if (recorder != null)
						recorder.StopRecording();
					if (player != null)
						player.StopThread();
				}
			}
			else
				e.Cancel = true;
		}

		/// <summary>
		/// Handles the FormClosed event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Environment.Exit(-1);
		}

		/// <summary>
		/// Handles the CheckedChanged event of the radioButtonFontSize control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void radioButtonFontSize_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonFontSizeSmall.Checked)
			{
				labelWordSentence.Font = new Font(labelWordSentence.Font.FontFamily, STANDARD_APPEARANCE.FONT_SIZE_SMALL, labelWordSentence.Font.Style);
				settings.FontSize = FontSizes.Small;
			}
			else if (radioButtonFontSizeMedium.Checked)
			{
				labelWordSentence.Font = new Font(labelWordSentence.Font.FontFamily, STANDARD_APPEARANCE.FONT_SIZE_MEDIUM, labelWordSentence.Font.Style);
				settings.FontSize = FontSizes.Medium;
			}
			else if (radioButtonFontSizeLarge.Checked)
			{
				labelWordSentence.Font = new Font(labelWordSentence.Font.FontFamily, STANDARD_APPEARANCE.FONT_SIZE_LARGE, labelWordSentence.Font.Style);
				settings.FontSize = FontSizes.Large;
			}
			else
			{
				settings.FontSize = FontSizes.Automatic;
				labelWordSentence.Refresh();
			}

			settings.Save(CONSTANTS.SETTINGS_FILENAME);
		}

		/// <summary>
		/// Handles the Activated event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-09-13</remarks>
		private void MainForm_Activated(object sender, EventArgs e)
		{
			if (!loading)
				numPadControl.FunctionsEnabled = true;
		}

		/// <summary>
		/// Handles the Deactivate event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-09-13</remarks>
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (!loading)
				numPadControl.FunctionsEnabled = false;
		}

		/// <summary>
		/// Handles the ViewChanged event of the numPadControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void numPadControl_ViewChanged(object sender, EventArgs e)
		{
			groupBoxSelect.Visible = numPadControl.AdvancedView;
			statusStrip.Visible = !numPadControl.AdvancedView;
			buttonAdvancedMode.Visible = !numPadControl.AdvancedView;
			groupBoxFontSize.Visible = numPadControl.AdvancedView;

			if (numPadControl.CurrentState == Function.Nothing)
				toolStripStatusLabelAction.Text = string.Format(Resources.STATUS_STRIP_RECORD_MESSAGE, STANDART_KEYS.RECORD1.ToString(), STANDART_KEYS.RECORD2.ToString());
			else
				toolStripStatusLabelAction.Text = string.Format(Resources.STATUS_STRIP_STOP_MESSAGE, STANDART_KEYS.RECORD1.ToString(), STANDART_KEYS.RECORD2.ToString());

			settings.AdvancedView = numPadControl.AdvancedView;
			settings.Save(CONSTANTS.SETTINGS_FILENAME);
		}

		/// <summary>
		/// Handles the Resize event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-24</remarks>
		private void MainForm_Resize(object sender, EventArgs e)
		{
			numPadControl.SuspendLayout();

			numPadControl.Width = (int)((double)Width / 2.25);
			numPadControl.Height = (int)((double)numPadControl.Width / 0.8);

			if ((double)numPadControl.Height * 1.48 > ClientRectangle.Height)
			{
				numPadControl.Height = (int)((double)ClientRectangle.Height / 1.477777);
				numPadControl.Width = (int)((double)numPadControl.Height * 0.8);
			}

			numPadControl.Left = (int)(Width / 2.0 - numPadControl.Width / 2.0) - 4;

			numPadControl.ResumeLayout();
		}

		/// <summary>
		/// Handles the ResizeEnd event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-05-28</remarks>
		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			//redraw numPadControl
			numPadControl.ImagesValid = false;
			numPadControl.Refresh();
		}

		# endregion
		# region Commandhandling
		/// <summary>
		/// Handles the Record event of the numPad control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void numPadControl_Record(object sender, EventArgs e)
		{
			Record();
		}

		/// <summary>
		/// Start a recording.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void Record()
		{
			if (!SoundDevicesAvailable.SoundInDeviceAvailable())
			{
				MessageBox.Show(Resources.NO_WAVEIN_DEVICE_TEXT, Resources.NO_WAVEIN_DEVICE_CAPTION);
				Stop();
				return;
			}

			collapsed = false;
			groupBoxSelect.Enabled = false;
			menuStripMain.Enabled = false;
			UpdateSettings();

			bool enableMP3 = false;

			//Initialize default settings for the Lame MP3 encoding.
			LameDOTnet.Lame.MP3_Settings MP3Settings = new LameDOTnet.Lame.MP3_Settings();
			MP3Settings.Bitrate = 192;
			MP3Settings.CopyrightBit = true;
			MP3Settings.CRC_Bit = true;
			MP3Settings.DisableBitReservoir = false;
			MP3Settings.OriginalBit = true;
			MP3Settings.PrivatBit = false;
			MP3Settings.QualityPreset = LameDOTnet.Lame.LAME_QUALITY_PRESET.LQP_NORMAL_QUALITY;
			MP3Settings.StrictISOencoding = false;
			MP3Settings.VBR_enabled = true;
			MP3Settings.VBR_maxBitrate = 320;
			MP3Settings.VBR_method = LameDOTnet.Lame.VBR_METHOD.VBR_METHOD_DEFAULT;
			MP3Settings.VBR_Quality = 0;
			MP3Settings.VBR_WriteHeader = true;

			if (enableMP3)
				recorder = new Recorder(MP3Settings, settings.Channels, settings.SamplingRate);
			else
				recorder = new Recorder(settings.Channels, settings.SamplingRate);

			//fetch temporary filename
			do
			{
				actualFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName().Split('.')[0] + (enableMP3 ? Resources.MP3_EXTENSION : Resources.WAV_EXTENSION));
			}
			while (File.Exists(actualFilename));

			recorder.StartRecording(actualFilename, 0, (settings.DelaysActive ? settings.StartDelay : 0), (settings.DelaysActive ? settings.StopDelay : 0));

			if (numPadControl.CurrentState == Function.Nothing)
				toolStripStatusLabelAction.Text = string.Format(Resources.STATUS_STRIP_RECORD_MESSAGE, STANDART_KEYS.RECORD1.ToString(), STANDART_KEYS.RECORD2.ToString());
			else
				toolStripStatusLabelAction.Text = string.Format(Resources.STATUS_STRIP_STOP_MESSAGE, STANDART_KEYS.RECORD1.ToString(), STANDART_KEYS.RECORD2.ToString());
		}

		/// <summary>
		/// Handles the Play event of the numPad control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void numPadControl_Play(object sender, EventArgs e)
		{
			Play();
		}

		/// <summary>
		/// Plays a recording.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void Play()
		{
			if (!SoundDevicesAvailable.SoundOutDeviceAvailable())
			{
				MessageBox.Show(Resources.NO_WAVEOUT_DEVICE_TEXT, Resources.NO_WAVEOUT_DEVICE_CAPTION);
				Stop();
				return;
			}

			collapsed = false;
			groupBoxSelect.Enabled = false;
			menuStripMain.Enabled = false;
			UpdateSettings();

			if (numPadControl.CurrentState != Function.Record)
			{
				string actualFilePath = dictionaryManager.GetMediaFilePath(settings.ActualCard, settings.Sentence, settings.Side);
				player.Play(actualFilePath, true);
				Thread waitThread = new Thread(new ThreadStart(WaitUntilEndOfPlayback));
				waitThread.Start();
			}
			numPadControl.Invalidate();
		}

		/// <summary>
		/// Waits until the end of the playback.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void WaitUntilEndOfPlayback()
		{
			while (player.IsPlaying)
				Thread.Sleep(100);

			Stop();
		}

		/// <summary>
		/// Handles the Stop event of the numPad control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void numPadControl_Stop(object sender, EventArgs e)
		{
			Stop();
		}

		/// <summary>
		/// Stops the playback or recording.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void Stop()
		{
			if (numPadControl.CurrentState == Function.Record)
			{
				try
				{
					recorder.StopRecording();
					if (recorder != null) recorder.Dispose();
					dictionaryManager.AddMediaFile(settings.ActualCard, actualFilename, settings.Sentence, settings.Side, CurrentEncoder, settings.ShowEncoderWindow, settings.MinimizeEncoderWindow);
					CardColors.Remove(settings.ActualCard);
					RefreshListView();
					Next();
				}
				catch
				{
					if (recorder != null) recorder.Dispose();
				}
				try
				{
					File.Delete(actualFilename);
				}
				catch { }
			}
			else if (numPadControl.CurrentState == Function.Play)
			{
				player.StopThread();
			}

			numPadControl.CurrentState = Function.Nothing;
			Invoke((EmtyDelegate)delegate() { menuStripMain.Enabled = true; });
			Invoke((EmtyDelegate)delegate() { groupBoxSelect.Enabled = true; });

			Invoke((EmtyDelegate)numPadControl.Refresh);

			if (numPadControl.CurrentState == Function.Nothing)
				toolStripStatusLabelAction.Text = string.Format(Resources.STATUS_STRIP_RECORD_MESSAGE, STANDART_KEYS.RECORD1.ToString(), STANDART_KEYS.RECORD2.ToString());
			else
				toolStripStatusLabelAction.Text = string.Format(Resources.STATUS_STRIP_STOP_MESSAGE, STANDART_KEYS.RECORD1.ToString(), STANDART_KEYS.RECORD2.ToString());
		}

		/// <summary>
		/// Handles the Next event of the numPad control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void numPadControl_Next(object sender, EventArgs e)
		{
			Next();
		}

		/// <summary>
		/// Go to the next card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void Next()
		{
			numPadControl.UninstallHook();

			if (settings.ActualCard == dictionaryManager.CardCount - 1 && settings.ActualStep == settings.RecordingOrder.Length - 1)
			{
				if (!multiselecting && MessageBox.Show(Resources.JUMP_TO_OTHER_END_TEXT, Resources.JUMP_TO_OTHER_END_CAPTION,
					MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					settings.ActualStep = -1;
					settings.ActualCard = 0;
				}
				else
				{
					if (!treeViewCards.Nodes[treeViewCards.Nodes.Count - 1].Checked)
						Back();
					return;
				}
			}

			lastCommand = LastCommand.Next;

			if (settings.ActualStep + 1 < settings.RecordingOrder.Length)
				settings.ActualStep++;
			else
			{
				if (settings.ActualCard + 1 < dictionaryManager.CardCount)
				{
					settings.ActualCard++;
					settings.ActualStep = 0;

					while (settings.ActualCard + 1 < dictionaryManager.CardCount && !treeViewCards.Nodes[settings.ActualCard].Checked)
						settings.ActualCard++;
				}
			}

			settings.Save(CONSTANTS.SETTINGS_FILENAME);
			UpdateSettings();

			numPadControl.InstallHook();
		}

		/// <summary>
		/// Handles the Previous event of the numPad control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void numPadControl_Previous(object sender, EventArgs e)
		{
			Back();
		}

		/// <summary>
		/// Back to the previous card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void Back()
		{
			numPadControl.UninstallHook();

			if (settings.ActualStep == 0 && settings.ActualCard == 0)
			{
				if (!multiselecting && MessageBox.Show(Resources.JUMP_TO_END_TEXT, Resources.JUMP_TO_OTHER_END_CAPTION,
					MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					settings.ActualStep = 4;
					settings.ActualCard = dictionaryManager.CardCount - 1;
				}
				else
				{
					if (!treeViewCards.Nodes[0].Checked)
						Next();
					return;
				}
			}

			lastCommand = LastCommand.Back;

			if (settings.ActualStep > 0)
				settings.ActualStep--;
			else
			{
				if (settings.ActualCard > 0)
				{
					settings.ActualCard--;
					settings.ActualStep = settings.RecordingOrder.Length - 1;

					while (settings.ActualCard > 0 && !treeViewCards.Nodes[settings.ActualCard].Checked)
						settings.ActualCard--;
				}
			}

			settings.Save(CONSTANTS.SETTINGS_FILENAME);
			UpdateSettings();

			numPadControl.InstallHook();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-03</remarks>
		private void UpdateSettings()
		{
			if (!InvokeRequired)
			{
				bool somethingSelected = false;

				foreach (TreeNode node in treeViewCards.Nodes)
				{
					if (node.Checked)
					{
						somethingSelected = true;
						break;
					}
				}

				if (!somethingSelected || selecting)
					return;

				updating = true;

				this.Text = dictionaryManager.DictionaryName + (!string.IsNullOrEmpty(dictionaryManager.DictionaryName) ? " - " : "") + AssemblyTitle;
				labelCard.Text = (settings.ActualCard + 1) + " / " + dictionaryManager.CardCount;
				progressBarStatus.Maximum = dictionaryManager.CardCount;
				progressBarStatus.Value = settings.ActualCard + 1;

				# region Validate actual step
				if (!treeViewCards.Nodes[settings.ActualCard].Checked)
				{
					if (lastCommand == LastCommand.Next)
						Next();
					else
						Back();

					updating = false;
					return;
				}

				switch (settings.RecordingOrder[settings.ActualStep])
				{
					case MediaItemType.Answer:
						if (!settings.RecordAnswer)
						{
							if (lastCommand == LastCommand.Next)
								Next();
							else
								Back();
							updating = false;
							return;
						}
						settings.Side = MLifter.DAL.Interfaces.Side.Answer;
						settings.Sentence = false;
						break;
					case MediaItemType.AnswerExample:
						if (!settings.RecordAnswerExample)
						{
							if (lastCommand == LastCommand.Next)
								Next();
							else
								Back();
							updating = false;
							return;
						}
						settings.Side = MLifter.DAL.Interfaces.Side.Answer;
						settings.Sentence = true;
						break;
					case MediaItemType.Question:
						if (!settings.RecordQuestion)
						{
							if (lastCommand == LastCommand.Next)
								Next();
							else
								Back();
							updating = false;
							return;
						}
						settings.Side = MLifter.DAL.Interfaces.Side.Question;
						settings.Sentence = false;
						break;
					case MediaItemType.QuestionExample:
						if (!settings.RecordQuestionExample)
						{
							if (lastCommand == LastCommand.Next)
								Next();
							else
								Back();
							updating = false;
							return;
						}
						settings.Side = MLifter.DAL.Interfaces.Side.Question;
						settings.Sentence = true;
						break;
				}
				# endregion

				labelWordSentence.Text = settings.Sentence ? dictionaryManager.GetSentence(settings.ActualCard, settings.Side) : dictionaryManager.GetWord(settings.ActualCard, settings.Side);
				actualText = labelWordSentence.Text;

				TreeNode actualSubNode = null;
				if (actualNode != null)
					actualNode.Nodes.Clear();
				actualNode = treeViewCards.Nodes[settings.ActualCard];

				if (collapsed)
					actualSubNode = actualNode;
				else
				{
					for (int i = 0; i < settings.RecordingOrder.Length; i++)
					{
						if (IsMediaTypeSelected(settings.RecordingOrder[i]))
						{
							TreeNode item = GetTreeNode(settings.RecordingOrder[i]);
							actualSubNode = (i == settings.ActualStep) ? item : actualSubNode;
							actualNode.Nodes.Add(item);
						}
					}
				}

				if (actualSubNode.Tag is string && (actualSubNode.Tag as string) == "")
				{
					if (lastCommand == LastCommand.Next)
						Next();
					else
						Back();
					updating = false;
					return;
				}

				treeViewCards.SelectedNode = actualSubNode;

				// New behavior
				//treeViewCards.SuspendLayout();
				//if (treeViewCards.Nodes.Count > 0)
				//{
				//    if (settings.ActualCard == treeViewCards.Nodes.Count - 1 && treeViewCards.Nodes[treeViewCards.Nodes.Count - 1].Nodes.Count > 0)
				//        treeViewCards.Nodes[treeViewCards.Nodes.Count - 1].Nodes[treeViewCards.Nodes[treeViewCards.Nodes.Count - 1].Nodes.Count - 1].EnsureVisible();
				//    else
				//        treeViewCards.Nodes[treeViewCards.Nodes.Count - 1].EnsureVisible();

				//    if (actualNode.Index > 0)
				//        treeViewCards.Nodes[actualNode.Index - 1].EnsureVisible();
				//    else
				//        actualNode.EnsureVisible();
				//}
				//treeViewCards.ResumeLayout();

				// Old behavior
				if (actualNode.Nodes.Count > 0)
					actualNode.Nodes[actualNode.Nodes.Count - 1].EnsureVisible();
				else
					actualNode.EnsureVisible();

				numPadControl.MediaFileAvailable = dictionaryManager.HasMediaFile(settings.ActualCard,
					((settings.RecordingOrder[settings.ActualStep] == MediaItemType.AnswerExample) || (settings.RecordingOrder[settings.ActualStep] == MediaItemType.QuestionExample)),
					(((settings.RecordingOrder[settings.ActualStep] == MediaItemType.Answer) || (settings.RecordingOrder[settings.ActualStep] == MediaItemType.AnswerExample)) ?
					MLifter.DAL.Interfaces.Side.Answer : MLifter.DAL.Interfaces.Side.Question));
				numPadControl.ImagesValid = false;
				numPadControl.Refresh();

				updating = false;
				treeViewCards.Refresh();
			}
			else
				Invoke((EmtyDelegate)UpdateSettings);
		}

		/// <summary>
		/// Gets the assembly title.
		/// </summary>
		/// <value>The assembly title.</value>
		/// <remarks>Documented by Dev02, 2008-04-15</remarks>
		private string AssemblyTitle
		{
			get
			{
				// Get all Title attributes on this assembly
				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				// If there is at least one Title attribute
				if (attributes.Length > 0)
				{
					// Select the first one
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					// If it is not an empty string, return it
					if (titleAttribute.Title != "")
						return titleAttribute.Title;
				}
				// If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
			}
		}

		/// <summary>
		/// Determines whether the media type is selected.
		/// </summary>
		/// <param name="mediaItemType">Type of the media item.</param>
		/// <returns>
		/// 	<c>true</c> if the media type is selected; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-08-22</remarks>
		private bool IsMediaTypeSelected(MediaItemType mediaItemType)
		{
			switch (mediaItemType)
			{
				case MediaItemType.Answer:
					return settings.RecordAnswer;
				case MediaItemType.AnswerExample:
					return settings.RecordAnswerExample;
				case MediaItemType.Question:
					return settings.RecordQuestion;
				case MediaItemType.QuestionExample:
					return settings.RecordQuestionExample;
			}

			return false;
		}

		/// <summary>
		/// Gets the tree node.
		/// </summary>
		/// <param name="mediaItemType">Type of the media item.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private TreeNode GetTreeNode(MediaItemType mediaItemType)
		{
			TreeNode node = null;

			switch (mediaItemType)
			{
				case MediaItemType.Answer:
					node = new TreeNode(Resources.LISTBOXFIELDS_ANSWER_TEXT.Remove(Resources.LISTBOXFIELDS_ANSWER_TEXT.LastIndexOf(' ')));
					node.Checked = settings.RecordAnswer;
					node.Tag = dictionaryManager.GetWord(settings.ActualCard, MLifter.DAL.Interfaces.Side.Answer);
					break;
				case MediaItemType.AnswerExample:
					node = new TreeNode(Resources.LISTBOXFIELDS_EXANSWER_TEXT.Remove(Resources.LISTBOXFIELDS_EXANSWER_TEXT.LastIndexOf(' ')));
					node.Checked = settings.RecordAnswerExample;
					node.Tag = dictionaryManager.GetSentence(settings.ActualCard, MLifter.DAL.Interfaces.Side.Answer);
					break;
				case MediaItemType.Question:
					node = new TreeNode(Resources.LISTBOXFIELDS_QUESTION_TEXT.Remove(Resources.LISTBOXFIELDS_QUESTION_TEXT.LastIndexOf(' ')));
					node.Checked = settings.RecordQuestion;
					node.Tag = dictionaryManager.GetWord(settings.ActualCard, MLifter.DAL.Interfaces.Side.Question);
					break;
				case MediaItemType.QuestionExample:
					node = new TreeNode(Resources.LISTBOXFIELDS_EXQUESTION_TEXT.Remove(Resources.LISTBOXFIELDS_EXQUESTION_TEXT.LastIndexOf(' ')));
					node.Checked = settings.RecordQuestionExample;
					node.Tag = dictionaryManager.GetSentence(settings.ActualCard, MLifter.DAL.Interfaces.Side.Question);
					break;
			}

			if (node.Tag is string && (node.Tag as string) == "")
			{
				node.Text += Resources.NO_TEXT_MESSAGE;
				node.Checked = false;
			}

			node.BackColor = GetSubNodeColor(mediaItemType);
			return node;
		}

		/// <summary>
		/// Gets the color of the sub node.
		/// </summary>
		/// <param name="mediaItemTye">The media item tye.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private Color GetSubNodeColor(MediaItemType mediaItemTye)
		{
			switch (mediaItemTye)
			{
				case MediaItemType.Answer:
					if (dictionaryManager.GetWord(settings.ActualCard, MLifter.DAL.Interfaces.Side.Answer) == "")
						return STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
					return dictionaryManager.HasMediaFile(settings.ActualCard, false, MLifter.DAL.Interfaces.Side.Answer) ? STANDARD_APPEARANCE.COLOR_CARD_COMPLETE : STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
				case MediaItemType.AnswerExample:
					if (dictionaryManager.GetSentence(settings.ActualCard, MLifter.DAL.Interfaces.Side.Answer) == "")
						return STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
					return dictionaryManager.HasMediaFile(settings.ActualCard, true, MLifter.DAL.Interfaces.Side.Answer) ? STANDARD_APPEARANCE.COLOR_CARD_COMPLETE : STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
				case MediaItemType.Question:
					if (dictionaryManager.GetWord(settings.ActualCard, MLifter.DAL.Interfaces.Side.Question) == "")
						return STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
					return dictionaryManager.HasMediaFile(settings.ActualCard, false, MLifter.DAL.Interfaces.Side.Question) ? STANDARD_APPEARANCE.COLOR_CARD_COMPLETE : STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
				case MediaItemType.QuestionExample:
					if (dictionaryManager.GetSentence(settings.ActualCard, MLifter.DAL.Interfaces.Side.Question) == "")
						return STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
					return dictionaryManager.HasMediaFile(settings.ActualCard, true, MLifter.DAL.Interfaces.Side.Question) ? STANDARD_APPEARANCE.COLOR_CARD_COMPLETE : STANDARD_APPEARANCE.COLOR_CARD_NOTHING;
			}

			return Color.White;
		}
		# endregion
		# region Other

		/// <summary>
		/// Handles the Paint event of the labelWordSentence control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void labelWordSentence_Paint(object sender, PaintEventArgs e)
		{
			if (radioButtonFontSizeAutomatic.Checked)
			{
				labelWordSentence.ForeColor = Color.Black;
				int fontSize = STANDARD_APPEARANCE.FONT_SIZE_AUTOMATIC_LOWER_LIMIT;

				while (fontSize < STANDARD_APPEARANCE.FONT_SIZE_AUTOMATIC_UPPER_LIMIT &&
					TextRenderer.MeasureText(actualText,
					 new Font(labelWordSentence.Font.FontFamily, fontSize + 1, labelWordSentence.Font.Style),
					 labelWordSentence.Size, TextFormatFlags.WordBreak).Height < labelWordSentence.Height)
					fontSize++;

				labelWordSentence.Font = new Font(labelWordSentence.Font.FontFamily, fontSize, labelWordSentence.Font.Style);
			}

			if (TextRenderer.MeasureText(actualText, labelWordSentence.Font, labelWordSentence.Size, TextFormatFlags.WordBreak).Height > labelWordSentence.Height + 10)
			{
				labelWordSentence.ForeColor = Color.Red;
				string text = actualText;

				do
				{
					text = text.Remove(text.Length - 4, 4) + "...";
				}
				while (TextRenderer.MeasureText(text, labelWordSentence.Font, labelWordSentence.Size, TextFormatFlags.WordBreak).Height > labelWordSentence.Height + 10
					&& text.Length >= 4);

				labelWordSentence.Text = text;
			}
			else
			{
				labelWordSentence.Text = actualText;
				labelWordSentence.ForeColor = Color.Black;
			}

			base.OnPaint(e);
		}

		/// <summary>
		/// Handles the Click event of the buttonAllCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void buttonAllCards_Click(object sender, EventArgs e)
		{
			SelectAllCards();
		}

		/// <summary>
		/// Handles the Click event of the buttonNoCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void buttonNoCards_Click(object sender, EventArgs e)
		{
			SelectNoCard();
		}

		/// <summary>
		/// Selects all cards.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void SelectAllCards()
		{
			selecting = true;
			treeViewCards.SuspendLayout();

			foreach (TreeNode item in treeViewCards.Nodes)
				item.Checked = true;

			treeViewCards.ResumeLayout();
			selecting = false;
		}

		/// <summary>
		/// Selects the no card.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-08-07</remarks>
		private void SelectNoCard()
		{
			selecting = true;
			treeViewCards.SuspendLayout();
			SuspendLayout();

			foreach (TreeNode item in treeViewCards.Nodes)
				item.Checked = false;

			treeViewCards.ResumeLayout();
			ResumeLayout();
			selecting = false;
		}

		/// <summary>
		/// Handles the Click event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_Click(object sender, EventArgs e)
		{
			// For future use...
		}

		/// <summary>
		/// Handles the DoubleClick event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_DoubleClick(object sender, EventArgs e)
		{
			collapsed = !collapsed;
			UpdateSettings();
		}

		/// <summary>
		/// Handles the Click event of the buttonSelectInverse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void buttonSelectInverse_Click(object sender, EventArgs e)
		{
			multiselecting = true;
			selecting = true;
			treeViewCards.SuspendLayout();

			foreach (TreeNode item in treeViewCards.Nodes)
				item.Checked = !item.Checked;

			treeViewCards.ResumeLayout();
			selecting = false;

			UpdateSettings();
			multiselecting = false;
		}

		/// <summary>
		/// Handles the Click event of the buttonSelectIncomplete control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void buttonSelectIncomplete_Click(object sender, EventArgs e)
		{
			multiselecting = true;
			selecting = true;
			treeViewCards.SuspendLayout();

			foreach (TreeNode item in treeViewCards.Nodes)
				item.Checked = (item.BackColor != STANDARD_APPEARANCE.COLOR_CARD_COMPLETE);

			treeViewCards.ResumeLayout();
			selecting = false;

			UpdateSettings();
			multiselecting = false;
		}

		/// <summary>
		/// Handles the BeforeCheck event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_BeforeCheck(object sender, TreeViewCancelEventArgs e)
		{
			if (treeViewCards.SelectedNode != null && treeViewCards.SelectedNode.Tag is string && (string)e.Node.Tag != "Card")
				e.Cancel = true;
		}

		/// <summary>
		/// Handles the BeforeCollapse event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			// For future use...
		}

		/// <summary>
		/// Handles the BeforeExpand event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Index != settings.ActualCard)
				e.Cancel = true;
		}

		/// <summary>
		/// Handles the BeforeSelect event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			if (treeViewCards.SelectedNode != null)
				treeViewCards.SelectedNode.ForeColor = Color.Black;

			if (e.Action == TreeViewAction.ByKeyboard || !e.Node.Checked)
				e.Cancel = true;
		}

		/// <summary>
		/// Handles the AfterCheck event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-22</remarks>
		private void treeViewCards_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Index == settings.ActualCard)
				UpdateSettings();
		}

		/// <summary>
		/// Handles the AfterSelect event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeViewCards.SelectedNode != null && !updating)
			{
				if (treeViewCards.SelectedNode.Tag is string && (string)treeViewCards.SelectedNode.Tag == "Card")
				{
					lastCommand = LastCommand.Next;
					settings.ActualStep = 0;
					settings.ActualCard = treeViewCards.SelectedNode.Index;
				}
				else
					settings.ActualStep = treeViewCards.SelectedNode.Index;
			}

			if (treeViewCards.SelectedNode != null)
				treeViewCards.SelectedNode.ForeColor = treeViewCards.SelectedNode.BackColor;

			if (!updating)
				UpdateSettings();
		}

		/// <summary>
		/// Handles the DrawNode event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DrawTreeNodeEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-09</remarks>
		private void treeViewCards_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (selecting || !e.Node.IsVisible)
				return;

			if (e.Graphics.MeasureString(e.Node.Text, e.Node.TreeView.Font).Width + (e.Node.IsSelected ? 60 : 50) > e.Node.TreeView.Width)
			{
				string text = e.Node.Text;

				while (e.Graphics.MeasureString(text, e.Node.TreeView.Font).Width + (e.Node.IsSelected ? 60 : 50) > e.Node.TreeView.Width)
				{
					text = text.Remove(text.Length - 4, 4);
					text = text.Insert(text.Length, "...");
				}

				e.Node.Text = text;
			}

			HatchBrush hatchBrush = new HatchBrush(System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal,
				STANDARD_APPEARANCE.COLOR_CARD_COMPLETE, STANDARD_APPEARANCE.COLOR_CARD_NOTHING);
			SolidBrush backgroundBrush = new SolidBrush(((e.Node.IsSelected || e.Node == actualNode) ? Color.White : e.Node.BackColor));

			if (e.Node.Tag is string && (string)e.Node.Tag == "Card")
			{
				if (!e.Node.Checked)
					e.Node.ForeColor = STANDARD_APPEARANCE.COLOR_CARD_NOTSELECTED;
				else
					e.Node.ForeColor = Color.Black;

				SolidBrush textBrush = new SolidBrush(e.Node.ForeColor);

				if (e.Node.BackColor == STANDARD_APPEARANCE.COLOR_CARD_NOTHING || e.Node.BackColor == STANDARD_APPEARANCE.COLOR_CARD_COMPLETE || e.Node.Index == settings.ActualCard)
					e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
				else
					e.Graphics.FillRectangle(hatchBrush, e.Bounds);

				Font treeviewcardsFont = new Font(treeViewCards.Font.FontFamily, treeViewCards.Font.Size, (e.Node.IsSelected ? FontStyle.Bold : FontStyle.Regular));
				e.Graphics.DrawString(e.Node.Text, treeviewcardsFont, textBrush, Math.Abs(e.Bounds.X) + 18, e.Bounds.Y + 2);

				int height = 11;
				int leftMargin = 5;
				int topMargin = e.Bounds.Top + (e.Bounds.Height / 2) - (height / 2);
				Pen blackPen = new Pen(Color.Black, 2);
				e.Graphics.DrawRectangle(blackPen, leftMargin, topMargin, height, height);

				if (e.Node.Checked)
				{
					Point[] a_points = new Point[] { new Point(leftMargin + 1, topMargin + 4),
						 new Point(leftMargin + 4, topMargin + 7), new Point(leftMargin + height - 2, topMargin + 2) };
					e.Graphics.DrawLines(blackPen, a_points);
				}

				if (e.Node.Index == settings.ActualCard)
				{
					e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
						e.Bounds.X, e.Bounds.Y,
						e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

					e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
						e.Bounds.X + 1, e.Bounds.Y,
						e.Bounds.X + 1, e.Bounds.Y + e.Bounds.Height);

					e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
						e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y,
						e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height);

					if (e.Node.Nodes.Count == 0)
						e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
							e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1,
							e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height - 1);
				}

				textBrush.Dispose();
			}
			else
			{
				SolidBrush textBrush = new SolidBrush(e.Node.Text.Contains(Resources.NO_TEXT_MESSAGE) ? STANDARD_APPEARANCE.COLOR_CARD_NOTSELECTED : Color.Black);
				e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
				e.Graphics.DrawString(e.Node.Text, new Font(treeViewCards.Font.FontFamily, treeViewCards.Font.Size, (e.Node.IsSelected ? FontStyle.Bold : FontStyle.Regular)),
					textBrush, Math.Abs(e.Bounds.X) + 25, e.Bounds.Y + 2);

				e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
					e.Bounds.X + 1, e.Bounds.Y,
					e.Bounds.X + 1, e.Bounds.Y + e.Bounds.Height);

				e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
					e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y,
					e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height);

				if (e.Node.Index == e.Node.Parent.Nodes.Count - 1)
					e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 3),
						e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1,
						e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height - 1);

				textBrush.Dispose();
			}
			hatchBrush.Dispose();
			backgroundBrush.Dispose();

			e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 1),
				e.Bounds.X, e.Bounds.Y,
				e.Bounds.X + e.Bounds.Width, e.Bounds.Y);
			e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 1),
				e.Bounds.X, e.Bounds.Y + e.Bounds.Height,
				e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height);

			e.Graphics.DrawLine(new Pen(STANDARD_APPEARANCE.COLOR_CARD_SEPARATOR, 1),
				e.Bounds.X + e.Bounds.Width - 15, e.Bounds.Y,
				e.Bounds.X + e.Bounds.Width - 15, e.Bounds.Y + e.Bounds.Height);

			if (e.Node.BackColor == STANDARD_APPEARANCE.COLOR_CARD_COMPLETE && e.Node.Tag is string && (e.Node.Tag as string) != "")
			{
				using (SolidBrush solidBrush = new SolidBrush(Color.Black))
				{
					e.Graphics.DrawString("X", new Font("Webdings", (int)(treeViewCards.Font.Size * 1.5)), solidBrush, treeViewCards.Width - 34, e.Bounds.Y - 2);
				}
			}
			else if (e.Node.BackColor == STANDARD_APPEARANCE.COLOR_CARD_PARTS)
			{
				using (SolidBrush solidBrush = new SolidBrush(Color.Gray))
				{
					e.Graphics.DrawString("X", new Font("Webdings", (int)(treeViewCards.Font.Size * 1.5)), solidBrush, treeViewCards.Width - 34, e.Bounds.Y - 2);
				}
			}
		}

		/// <summary>
		/// Handles the Enter event of the treeViewCards control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-10</remarks>
		private void treeViewCards_Enter(object sender, EventArgs e)
		{
			// To enable the mouse wheel
			//numPadControl.Focus();
		}

		/// <summary>
		/// Handles the FocusEnter event of the buttons control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-22</remarks>
		private void buttons_FocusEnter(object sender, EventArgs e)
		{
			numPadControl.Focus();
		}

		/// <summary>
		/// Handles the Enter event of the numPadControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-08-29</remarks>
		private void numPadControl_Enter(object sender, EventArgs e)
		{
			treeViewCards.Focus();
		}

		/// <summary>
		/// Handles the Click event of the buttonAdvancedMode control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-09-11</remarks>
		private void buttonAdvancedMode_Click(object sender, EventArgs e)
		{
			numPadControl.AdvancedView = true;
		}

		/// <summary>
		/// Handles the Enter event of the radioButtonFontSize controls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-09-25</remarks>
		private void radioButtonFontSize_Enter(object sender, EventArgs e)
		{
			numPadControl.Focus();
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemHowTo control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-10-08</remarks>
		private void toolStripMenuItemHowTo_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Resources.HOWTO_URL);
			}
			catch (Exception exp)
			{
				Trace.WriteLine("Could not open HowTo: " + exp.ToString());
			}
		}

		/// <summary>
		/// Handles the Click event of the codecInformationToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-03-01</remarks>
		private void codecInformationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SettingsForm.ShowCodecSettings(settings);
		}
		
		/// <summary>
		/// Handles the Shown event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-04-16</remarks>
		private void MainForm_Shown(object sender, EventArgs e)
		{
			if (settings.AskLayoutAtStartup)
			{
				//show layout selector and save the selected values
				LayoutSelectorForm selectForm = new LayoutSelectorForm();
				selectForm.KeyboardLayout = settings.KeyboardLayout;
				selectForm.AskAgain = settings.AskLayoutAtStartup;

				selectForm.ShowDialog();

				if (settings.KeyboardLayout != selectForm.KeyboardLayout || settings.AskLayoutAtStartup != selectForm.AskAgain)
				{
					settings.KeyboardLayout = selectForm.KeyboardLayout;
					settings.AskLayoutAtStartup = selectForm.AskAgain;
					settings.Save(CONSTANTS.SETTINGS_FILENAME);
					if (settings.AdvancedView)
					{
						numPadControl.KeyboardLayout = settings.KeyboardLayout;
						numPadControl.Refresh();
					}
				}
			}
		}
		# endregion
	}
}