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
    // Why this must be class ? otherwise in win XP won't work.
    
    /// <summary>
    /// This class represents WAVEFORMATEX structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class WAVEFORMATEX
    {
        /// <summary>
        /// Waveform-audio format type. Format tags are registered with Microsoft Corporation for many 
        /// compression algorithms. A complete list of format tags can be found in the Mmreg.h header file. 
        /// For one- or two-channel PCM data, this value should be WAVE_FORMAT_PCM. When this structure is 
        /// included in a WAVEFORMATEXTENSIBLE structure, this value must be WAVE_FORMAT_EXTENSIBLE.</summary>
        public ushort wFormatTag;
        /// <summary>
        /// Number of channels in the waveform-audio data. Monaural data uses one channel and stereo data 
        /// uses two channels.
        /// </summary>
        public ushort nChannels;
        /// <summary>
        /// Sample rate, in samples per second (hertz). If wFormatTag is WAVE_FORMAT_PCM, then common 
        /// values for nSamplesPerSec are 8.0 kHz, 11.025 kHz, 22.05 kHz, and 44.1 kHz.
        /// </summary>
        public uint nSamplesPerSec;
        /// <summary>
        /// Required average data-transfer rate, in bytes per second, for the format tag. If wFormatTag 
        /// is WAVE_FORMAT_PCM, nAvgBytesPerSec should be equal to the product of nSamplesPerSec and nBlockAlign.
        /// </summary>
        public uint nAvgBytesPerSec;
        /// <summary>
        /// Block alignment, in bytes. The block alignment is the minimum atomic unit of data for the wFormatTag 
        /// format type. If wFormatTag is WAVE_FORMAT_PCM or WAVE_FORMAT_EXTENSIBLE, nBlockAlign must be equal 
        /// to the product of nChannels and wBitsPerSample divided by 8 (bits per byte).
        /// </summary>
        public ushort nBlockAlign;
        /// <summary>
        /// Bits per sample for the wFormatTag format type. If wFormatTag is WAVE_FORMAT_PCM, then 
        /// wBitsPerSample should be equal to 8 or 16.
        /// </summary>
        public ushort wBitsPerSample;
        /// <summary>
        /// Size, in bytes, of extra format information appended to the end of the WAVEFORMATEX structure.
        /// </summary>
        public ushort cbSize;

        #region method ToString

        /// <summary>
        /// Returns this object field values as string list.
        /// </summary>
        /// <returns>Returns this object field values as string list.</returns>
        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append("wFormatTag: " + wFormatTag + "\r\n");
            retVal.Append("nChannels: " + nChannels + "\r\n");
            retVal.Append("nSamplesPerSec: " + nSamplesPerSec + "\r\n");
            retVal.Append("nAvgBytesPerSec: " + nAvgBytesPerSec + "\r\n");
            retVal.Append("nBlockAlign: " + nBlockAlign + "\r\n");
            retVal.Append("wBitsPerSample: " + wBitsPerSample + "\r\n");
            retVal.Append("cbSize: " + cbSize + "\r\n");

            return retVal.ToString();
        }

        #endregion

    }
}
