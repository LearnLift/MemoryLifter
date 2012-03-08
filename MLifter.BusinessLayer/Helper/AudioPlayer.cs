//#define debug_output

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Microsoft.DirectX.AudioVideoPlayback;

namespace MLifter.BusinessLayer.Helper
{
	public class AudioPlayer
	{
		private Thread playThread = null;
		private Queue<string> playQueue = new Queue<string>();
		private bool stopCurrentPlay = false;
		private bool stopThread = false;

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
				return playThread.ThreadState == System.Threading.ThreadState.Running;
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
			Audio audio = null;

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
						audio.Stop();
						audio.Dispose();
						audio = null;
					}

					if (filename != null)
					{
#if debug_output
						Trace.WriteLine("Fetching audio object for " + filename);
#endif
						audio = new Audio(filename, false);

#if debug_output
						Trace.WriteLine(string.Format("Starting playing {0}. Duration: {1}.", filename, audio.Duration.ToString()));
#endif
						audio.Play();

						//wait for completion
						while (audio.CurrentPosition < audio.Duration)
						{
							System.Threading.Thread.Sleep(100);
							if (stopCurrentPlay && (audio.Duration - audio.CurrentPosition) > 3) //don't stop short sounds
								break;
						}
						audio.Stop();
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
						System.Windows.Forms.MessageBox.Show(Properties.Resources.AUDIOPLAYER_CRASHED_TEXT, Properties.Resources.AUDIOPLAYER_CRASHED_CAPTION,
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
		/// Plays the current audio file and stops the thread afterwards.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public void StopThread()
		{
			StopThread(false);
		}

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
	}
}
