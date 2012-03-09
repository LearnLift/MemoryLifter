using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MLifter.AudioTools
{
    /// <summary>
    /// Contains functions to determine if there are any audio devices available.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-05-26</remarks>
    public class SoundDevicesAvailable
    {
        [DllImport("winmm.dll")]
        private static extern int waveOutGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int waveInGetNumDevs();

        /// <summary>
        /// Returns a value indicating if there are any sound playing devices available.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-20</remarks>
        public static bool SoundOutDeviceAvailable()
        {
            int count = waveOutGetNumDevs();
            return count > 0;
        }

        /// <summary>
        /// Returns a value indicating if there are any microphone devices available.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-20</remarks>
        public static bool SoundInDeviceAvailable()
        {
            int count = waveInGetNumDevs();
            return count > 0;
        }

    }
}
