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
    /// This class represents WAVEHDR structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WAVEHDR
    {
        /// <summary>
        /// Long pointer to the address of the waveform buffer.
        /// </summary>
        public IntPtr lpData;
        /// <summary>
        /// Specifies the length, in bytes, of the buffer.
        /// </summary>
        public uint dwBufferLength;
        /// <summary>
        /// When the header is used in input, this member specifies how much data is in the buffer. 
        /// When the header is used in output, this member specifies the number of bytes played from the buffer.
        /// </summary>
        public uint dwBytesRecorded;
        /// <summary>
        /// Specifies user data.
        /// </summary>
        public IntPtr dwUser;
        /// <summary>
        /// Specifies information about the buffer.
        /// </summary>
        public uint dwFlags;
        /// <summary>
        /// Specifies the number of times to play the loop.
        /// </summary>
        public uint dwLoops;
        /// <summary>
        /// Reserved. This member is used within the audio driver to maintain a first-in, first-out linked list of headers awaiting playback.
        /// </summary>
        public IntPtr lpNext;
        /// <summary>
        /// Reserved.
        /// </summary>
        public uint reserved;
    }
}
