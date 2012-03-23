/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA / LumiSoft															*
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

namespace LumiSoft.Media.Wave.Native
{
    /// <summary>
    /// This class represents WAVEOUTCAPS structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WAVEOUTCAPS
    {
        /// <summary>
        /// Manufacturer identifier for the device driver for the device.
        /// </summary>
        public ushort wMid;
        /// <summary>
        /// Product identifier for the device.
        /// </summary>
        public ushort wPid;
        /// <summary>
        /// Version number of the device driver for the device.
        /// </summary>
        public uint vDriverVersion;
        /// <summary>
        /// Product name in a null-terminated string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr,SizeConst = 32)]
        public string szPname;
        /// <summary>
        /// Standard formats that are supported.
        /// </summary>
        public uint dwFormats;
        /// <summary>
        /// Number specifying whether the device supports mono (1) or stereo (2) output.
        /// </summary>
        public ushort wChannels;
        /// <summary>
        /// Packing.
        /// </summary>
        public ushort wReserved1;
        /// <summary>
        /// Optional functionality supported by the device.
        /// </summary>
        public uint dwSupport;
    }
}
