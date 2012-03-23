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

namespace LumiSoft.Media.Wave
{
    /// <summary>
    /// This class represents wav input device.
    /// </summary>
    public class WavInDevice
    {
        private int    m_Index    = 0;
        private string m_Name     = "";
        private int    m_Channels = 1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="index">Device index in devices.</param>
        /// <param name="name">Device name.</param>
        /// <param name="channels">Number of audio channels.</param>
        internal WavInDevice(int index,string name,int channels)
        {
            m_Index    = index;
            m_Name     = name;
            m_Channels = channels;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets device name.
        /// </summary>
        public string Name
        {
            get{ return m_Name; }
        }

        /// <summary>
        /// Gets number of input channels(mono,stereo,...) supported.
        /// </summary>
        public int Channels
        {
            get{ return m_Channels; }
        }


        /// <summary>
        /// Gets device index in devices.
        /// </summary>
        internal int Index
        {
            get{ return m_Index; }
        }

        #endregion
    }
}
