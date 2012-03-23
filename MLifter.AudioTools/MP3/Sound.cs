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
using System.Runtime.InteropServices;
using QuartzTypeLib;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Diagnostics;

// Code extracted from MLifter-Solution and shortened to the required methods
namespace MLifter.AudioTools
{
    /// <summary>
    /// Sound class for playing mp3
    /// </summary>
    [Obsolete("Use BusinessLayer/AudioPlayer class instead!")]
    public class SoundIO
    {
        private static QuartzTypeLib.FilgraphManager graphManager;
        private static QuartzTypeLib.IMediaControl mp3control;
        private static QuartzTypeLib.IMediaPosition StreamInfo;

        /// <summary>
        /// Constructor of the SoundIO Object
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public SoundIO() { }

        /// <summary>
        /// This method waits until the playback of the sound has finished.
        /// No Unit Test necessary
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public void Wait()
        {
            if (StreamInfo != null)
            {
                while (StreamInfo.CurrentPosition < StreamInfo.StopTime)
                    Thread.Sleep(100);
                mp3control.Stop();
            }
        }

        /// <summary>
        /// Checks if sound-replay finished.
        /// Returns true or false.
        /// Unit Test necessary
        /// </summary>
        /// <returns>true or false</returns>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public bool PlayingFinished()
        {
            try
            {
                if (StreamInfo != null)
                {
                    return StreamInfo.CurrentPosition >= StreamInfo.StopTime;
                }
            }
            catch (InvalidComObjectException) { }

            return true;
        }

        /// <summary>
        /// This method plays the audio-file which is specified in the parameter "filename".
        /// Unit Test necessary
        /// </summary>
        /// <param name="filename">filename of audio-file</param>
        /// <param name="wait_until_finished">boolean variable</param>
        /// <returns>true or false</returns>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public bool Play(string filename, bool wait_until_finished)
        {
            filename = filename.Replace("/", "\\");
            try
            {
                if (wait_until_finished)
                    Wait();
                else if (StreamInfo != null)
                    mp3control.Stop();

                graphManager = new QuartzTypeLib.FilgraphManager();
                mp3control = (QuartzTypeLib.IMediaControl)graphManager;
                mp3control.RenderFile(filename);
                mp3control.Run();
                System.Diagnostics.Trace.WriteLine("Started playing " + filename);
                StreamInfo = (QuartzTypeLib.IMediaPosition)graphManager;
                System.Diagnostics.Trace.WriteLine("Streaminfo currentposition: " + StreamInfo.CurrentPosition.ToString() + " stoptime: " + StreamInfo.StopTime.ToString());

                return true;
            }
            catch (Exception exp)
            {
                Trace.WriteLine(exp.ToString());
                return false;
            }
        }

        /// <summary>
        /// This method stops the current playback of a sound.
        /// No Unit Test necessary
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-19</remarks>
        public void StopPlaying()
        {
            if (StreamInfo != null)
            {
                try
                {
                    mp3control.Stop();
                    Marshal.ReleaseComObject(mp3control);
                    mp3control = null;
                    Marshal.ReleaseComObject(StreamInfo);
                    StreamInfo = null;
                    Marshal.ReleaseComObject(graphManager);
                    graphManager = null;
                }
                catch { }
            }
        }
    }
}
