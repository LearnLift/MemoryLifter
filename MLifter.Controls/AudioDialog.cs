using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MLifter.DAL;
using MLifter.Controls.Properties;
using MLifter.AudioTools;

namespace MLifter.Controls
{
	public partial class AudioDialog : Form
	{
		private static readonly bool EnableMP3Recording = false;

		private AudioPlayer player;
		private Recorder recorder;

		private string path = string.Empty;
		public string Path
		{
			get { return path; }
			set
			{
				path = value;
				buttonPlay.Enabled = (path != string.Empty && SoundDevicesAvailable.SoundOutDeviceAvailable());
			}
		}

		/// <summary>
		/// Gets or sets the display Text.
		/// </summary>
		/// <value>The display Text.</value>
		/// <remarks>Documented by Dev02, 2008-02-06</remarks>
		public string DisplayText
		{
			get { return labelText.Text; }
			set
			{
				string text = value;
				if (text.Contains(Environment.NewLine))
				{
					string[] texts = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					if (texts.Length > 0) text = texts[0];
				}
				if (string.IsNullOrEmpty(text)) text = Resources.MAINTAIN_NA;
				labelText.Text = text;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioDialog"/> class.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <param name="side">The side.</param>
		/// <param name="example">if set to <c>true</c> to use the example.</param>
		/// <remarks>Documented by Dev05, 2007-12-06</remarks>
		public AudioDialog()
		{
			InitializeComponent();

			# region MP3-Settings
			LameDOTnet.Lame.MP3_Settings mp3Settings = new LameDOTnet.Lame.MP3_Settings();
			mp3Settings.Bitrate = 128;
			mp3Settings.CopyrightBit = true;
			mp3Settings.CRC_Bit = true;
			mp3Settings.DisableBitReservoir = false;
			mp3Settings.OriginalBit = true;
			mp3Settings.PrivatBit = false;
			mp3Settings.QualityPreset = LameDOTnet.Lame.LAME_QUALITY_PRESET.LQP_NORMAL_QUALITY;
			mp3Settings.StrictISOencoding = false;
			mp3Settings.VBR_enabled = true;
			mp3Settings.VBR_maxBitrate = 128;
			mp3Settings.VBR_method = LameDOTnet.Lame.VBR_METHOD.VBR_METHOD_DEFAULT;
			mp3Settings.VBR_Quality = Settings.Default.audioVBRQuality;
			mp3Settings.VBR_WriteHeader = true;
			# endregion
			if (EnableMP3Recording)
				recorder = new Recorder(mp3Settings, Settings.Default.audioChannels, Settings.Default.audioSamplingrate);
			else
				recorder = new Recorder(Settings.Default.audioChannels, Settings.Default.audioSamplingrate);

			buttonRecord.Enabled = SoundDevicesAvailable.SoundInDeviceAvailable();
		}

		/// <summary>
		/// Handles the Click event of the buttonBrowse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-12-06</remarks>
		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = Resources.AUDIO_FILE_FILTER;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
				Path = openFileDialog.FileName;
		}

		/// <summary>
		/// Handles the Click event of the buttonPlay control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-12-06</remarks>
		private void buttonPlay_Click(object sender, EventArgs e)
		{
			try
			{
				if (player == null)
				{
					player = new AudioPlayer();
					player.Ending += new EventHandler(player_Ending);
				}

				if (!player.IsPlaying)
				{
					buttonBrowse.Enabled = false;
					buttonRecord.Enabled = false;
					buttonRemove.Enabled = false;

					buttonPlay.Image = Resources.mediaPlaybackStop;
					buttonPlay.Text = Resources.AUDIO_STOP;

					player.Play(Path);
				}
				else
				{
					player.Stop();
					player_Ending(player, EventArgs.Empty);
				}
			}
			catch (Exception exp)
			{
				System.Windows.Forms.MessageBox.Show(Properties.Resources.AUDIOPLAYER_CRASHED_TEXT, Properties.Resources.AUDIOPLAYER_CRASHED_CAPTION,
							System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

				System.Diagnostics.Trace.WriteLine("AudioDialog Player crashed: " + exp.ToString());
			}
		}

		/// <summary>
		/// Handles the Ending event of the player control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-12-06</remarks>
		void player_Ending(object sender, EventArgs e)
		{
			Action ending = (Action)delegate()
			{
				buttonBrowse.Enabled = true;
				buttonRecord.Enabled = SoundDevicesAvailable.SoundInDeviceAvailable();
				buttonRemove.Enabled = true;

				buttonPlay.Image = Resources.mediaPlaybackStart;
				buttonPlay.Text = Resources.AUDIO_PLAY;

				if (player != null)
					player.Stop();  // Stop the current audio played
			};
			if (buttonBrowse.InvokeRequired)
				buttonBrowse.Invoke(ending);
			else
				ending.Invoke();
		}

		/// <summary>
		/// Handles the Click event of the buttonRecord control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-12-06</remarks>
		private void buttonRecord_Click(object sender, EventArgs e)
		{
			if (!recorder.Recording)
			{
				Path = System.IO.Path.GetTempFileName();
				Path = System.IO.Path.ChangeExtension(Path, EnableMP3Recording ? ".mp3" : ".wav");

				buttonBrowse.Enabled = false;
				buttonPlay.Enabled = false;
				buttonRemove.Enabled = false;

				buttonRecord.Image = Resources.mediaPlaybackStop;
				buttonRecord.Text = Resources.AUDIO_STOP;
				recorder.StartRecording(Path, -1, Settings.Default.audioPregap, Settings.Default.audioEndgap);
			}
			else
			{
				recorder.StopRecording();

				buttonBrowse.Enabled = true;
				buttonPlay.Enabled = SoundDevicesAvailable.SoundOutDeviceAvailable();
				buttonRemove.Enabled = true;

				buttonRecord.Image = Resources.mediaRecord;
				buttonRecord.Text = Resources.AUDIO_RECORD;
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonRemove control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2007-12-06</remarks>
		private void buttonRemove_Click(object sender, EventArgs e)
		{
			Path = string.Empty;
		}

		/// <summary>
		/// Handles the FormClosing event of the AudioDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-02-29</remarks>
		private void AudioDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (player != null)
			{
				try
				{
					// [ML-1181] Can't play MIDI-Files 
					player.Stop();  // Stop the current audio played
					player.Dispose();   // and clean-up
				}
				catch { }
				player = null;
			}
			if (recorder.Recording)
			{
				recorder.StopRecording();
			}
		}
	}
}