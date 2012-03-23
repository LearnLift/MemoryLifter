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
