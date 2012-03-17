//#define debug_output

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MLifterAudioTools.Properties;
using WMPLib;

namespace MLifter.AudioTools
{
	public class AudioPlayer : IDisposable
	{
		private Thread playThread = null;
		private Queue<string> playQueue = new Queue<string>();
		private bool stopCurrentPlay = false;
		private bool stopThread = false;

		/// <summary>
		/// Occurs when the current played audio is ending.
		/// </summary>
		/// <remarks>CFI, 2012-03-17</remarks>
		public event EventHandler Ending;
		/// <summary>
		/// Raises the <see cref="E:Ending"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-03-17</remarks>
		protected virtual void OnEnding(EventArgs e)
		{
			if (Ending != null)
				Ending(this, e);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is playing an audio file.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is playing; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>CFI, 2012-02-10</remarks>
		public bool IsPlaying
		{
			get
			{
				if (playThread == null)
					return false;
				return playThread.ThreadState == System.Threading.ThreadState.Running || playThread.ThreadState == System.Threading.ThreadState.Background;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioPlayer"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public AudioPlayer()
		{ }

		/// <summary>
		/// Plays the specified filename.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public void Play(string filename)
		{
			Play(filename, false);
		}

		/// <summary>
		/// Adds the specified filename to the playback queue.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="clearQueue">if set to <c>true</c> [clear queue].</param>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public void Play(string filename, bool clearQueue)
		{
			if (!AudioTools.SoundDevicesAvailable.SoundOutDeviceAvailable())
			{
				Trace.WriteLine("AudioPlayer Thread invocation skipped - there are no audio output devices available.");
				return;
			}

			if (playThread == null || !playThread.IsAlive)
			{
				Trace.WriteLine("AudioPlayer Thread started.");
				playThread = new Thread(new ThreadStart(PlayThread));
				playThread.Name = "ML Audioplayer Thread";
				playThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
				playThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
				playThread.Priority = ThreadPriority.Lowest;
				playThread.IsBackground = true;
				playThread.Start();
			}
#if DEBUG && debug_output
			Debug.WriteLine("Adding file to audio queue: " + filename);
#endif
			lock (playQueue)
			{
				if (clearQueue)
				{
					playQueue.Clear();
					stopCurrentPlay = true;
				}
				playQueue.Enqueue(filename);
				Monitor.Pulse(playQueue);
			}
#if DEBUG && debug_output
			Debug.WriteLine("Finished adding file to audio queue.");
#endif
		}

		/// <summary>
		/// Plays the thread.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		private void PlayThread()
		{
			stopThread = false;
			WindowsMediaPlayer audio = null;

			while (true)
			{
				try
				{
					string filename = null;

					lock (playQueue)
					{
						while (playQueue.Count == 0 && !stopThread)
							Monitor.Wait(playQueue);
						if (playQueue.Count > 0 && !stopCurrentPlay)
							filename = playQueue.Dequeue();
					}

					if (stopThread && filename == null)
						break;

					//play sound file
					stopCurrentPlay = false;
					if (audio != null)
					{
						audio.controls.stop();
						audio = null;
					}

					if (filename != null)
					{
#if debug_output
						Trace.WriteLine("Fetching audio object for " + filename);
#endif
						audio = new WindowsMediaPlayer();
						audio.URL = filename;

#if debug_output
						Trace.WriteLine(string.Format("Starting playing {0}. Duration: {1}.", filename, audio.currentMedia.duration.ToString()));
#endif
						audio.controls.play();
						try
						{
							while (audio.playState == WMPPlayState.wmppsTransitioning || audio.playState == WMPPlayState.wmppsBuffering)
								Thread.Sleep(25);
						}
						catch { Thread.Sleep(250); }

						//wait for completion
						while (audio.controls.currentPosition < audio.currentMedia.duration &&
							(audio.playState == WMPPlayState.wmppsPlaying || audio.playState == WMPPlayState.wmppsBuffering))
						{
							System.Threading.Thread.Sleep(100);
							if (stopCurrentPlay && (audio.currentMedia.duration - audio.controls.currentPosition) > 3) //don't stop short sounds
								break;
						}
						audio.controls.stop();
						OnEnding(EventArgs.Empty);
#if debug_output
						Trace.WriteLine("Finished/Stopped playing " + filename);
#endif
					}
				}
				catch (Exception exp)
				{
					if (exp is FileNotFoundException)
					{
						//Display a message and terminate player thread
						//[ML-724] On-Stick-Mode: Audioplayer thread crashes when the stick is pulled off and plugged back in (DAC, 2008-03-05)
						System.Windows.Forms.MessageBox.Show(Resources.AUDIOPLAYER_CRASHED_TEXT, Resources.AUDIOPLAYER_CRASHED_CAPTION,
							System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
						break;
					}
					if (exp is ThreadAbortException)
					{
						Trace.WriteLine("AudioPlayer Thread terminated.");
					}
					else
					{
						try
						{
							Trace.WriteLine("AudioPlayer crashed: " + exp.ToString());
						}
						catch { Trace.WriteLine("AudioPlayer crashed with unreadable exception!"); }
						break;
					}
				}
			}
		}

		/// <summary>
		/// Stops the playback.
		/// </summary>
		/// <remarks>CFI, 2012-03-17</remarks>
		public void Stop() { StopThread(true); }
		/// <summary>
		/// Plays the current audio file and stops the thread afterwards.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public void StopThread() { StopThread(false); }
		/// <summary>
		/// Stops the thread.
		/// </summary>
		/// <param name="stopCurrentPlay">if set to <c>true</c> [stop current play].</param>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public void StopThread(bool stopCurrentPlay)
		{
			Trace.WriteLine("AudioPlayer Thread terminate requested. StopCurrentPlay: " + stopCurrentPlay.ToString());
			this.stopCurrentPlay = stopCurrentPlay;
			stopThread = true;
			lock (playQueue)
				Monitor.Pulse(playQueue);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>CFI, 2012-03-17</remarks>
		public bool IsDisposed { get; set; }
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>CFI, 2012-03-17</remarks>
		public void Dispose() { IsDisposed = true; }
	}
}
