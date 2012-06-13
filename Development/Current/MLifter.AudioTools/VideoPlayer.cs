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
//#define debug_output

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AxWMPLib;
using MLifterAudioTools.Properties;
using WMPLib;

namespace MLifter.AudioTools
{
	public class VideoPlayer : IDisposable
	{
		private Thread playThread = null;
		private Queue<string> playQueue = new Queue<string>();
		private bool stopCurrentPlay = false;
		private bool stopThread = false;

		protected AxWindowsMediaPlayer video = null;

		private Control owner;
		/// <summary>
		/// Gets the owner, where the video will be played on.
		/// </summary>
		/// <remarks>CFI, 2012-03-17</remarks>
		public Control Owner
		{
			get { return owner; }
			set
			{
				owner = value;
				if (video != null)
					video.Parent = owner;
			}
		}

		/// <summary>
		/// Occurs when the current played video is ending.
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
		/// Occurs when the current played video is stopping.
		/// </summary>
		/// <remarks>CFI, 2012-03-17</remarks>
		public event EventHandler Stopping;
		/// <summary>
		/// Raises the <see cref="E:Stopping"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-03-17</remarks>
		protected virtual void OnStopping(EventArgs e)
		{
			if (Stopping != null)
				Stopping(this, e);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is playing a video file.
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
				return playThread.isAlive || playThread.isBackground;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioPlayer"/> class.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		public VideoPlayer(Control owner) { Owner = owner; }

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
			if (playThread == null || !playThread.IsAlive)
			{
				Trace.WriteLine("VideoPlayer Thread started.");
				playThread = new Thread(new ThreadStart(PlayThread));
				playThread.Name = "ML VideoPlayer Thread";
				playThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
				playThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
				playThread.Priority = ThreadPriority.Lowest;
				playThread.IsBackground = true;
				playThread.Start();
			}
#if DEBUG && debug_output
			Debug.WriteLine("Adding file to video queue: " + filename);
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
			Debug.WriteLine("Finished adding file to video queue.");
#endif
		}

		/// <summary>
		/// Plays the thread.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-02-22</remarks>
		private void PlayThread()
		{
			stopThread = false;

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

					//play video file
					stopCurrentPlay = false;
					if (video != null)
					{
						video.Ctlcontrols.stop();
						video = null;
					}

					if (filename != null)
					{
#if debug_output
						Trace.WriteLine("Fetching video object for " + filename);
#endif
						Owner.Invoke((Action)delegate()
						{
							video = new AxWindowsMediaPlayer();
							video.BeginInit();
							video.Location = new System.Drawing.Point(0, 0);
							video.Size = Owner.Size;
							Owner.Controls.Add(video);
							video.PlayStateChange += (s, e) =>
								{
									if (video.playState == WMPPlayState.wmppsStopped)
										OnStopping(EventArgs.Empty);
								};
							video.EndInit();
						});

						video.URL = filename;
#if debug_output
						Trace.WriteLine(string.Format("Starting playing {0}. Duration: {1}.", filename, video.currentMedia.duration.ToString()));
#endif
						video.Ctlcontrols.play();
						while (video.playState == WMPPlayState.wmppsTransitioning || video.playState == WMPPlayState.wmppsBuffering)
							Thread.Sleep(25);

						//wait for completion
						while (video.Ctlcontrols.currentPosition < video.currentMedia.duration && 
							(video.playState == WMPPlayState.wmppsPlaying || video.playState == WMPPlayState.wmppsBuffering))
						{
							System.Threading.Thread.Sleep(100);
							if (stopCurrentPlay && (video.currentMedia.duration - video.Ctlcontrols.currentPosition) > 1) //don't stop short sounds
								break;
						}
						if (video.playState == WMPPlayState.wmppsPlaying)
							video.Ctlcontrols.stop();
						else
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
		/// Plays the current video file and stops the thread afterwards.
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
			Trace.WriteLine("VideoPlayer Thread terminate requested. StopCurrentPlay: " + stopCurrentPlay.ToString());
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
