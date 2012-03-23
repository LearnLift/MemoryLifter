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

namespace LumiSoft.Media.Wave.Native
{
    /// <summary>
    /// This class holds MMSYSERR errors.
    /// </summary>
    internal class MMSYSERR
    {
        /// <summary>
        /// Success.
        /// </summary>
        public const int NOERROR = 0;
        /// <summary>
        /// Unspecified error.
        /// </summary>
        public const int ERROR = 1;
        /// <summary>
        /// Device ID out of range.
        /// </summary>
        public const int BADDEVICEID = 2;
        /// <summary>
        /// Driver failed enable.
        /// </summary>
        public const int NOTENABLED = 3;
        /// <summary>
        /// Device already allocated.
        /// </summary>
        public const int ALLOCATED = 4;
        /// <summary>
        /// Device handle is invalid.
        /// </summary>
        public const int INVALHANDLE = 5;
        /// <summary>
        /// No device driver present.
        /// </summary>
        public const int NODRIVER = 6;
        /// <summary>
        /// Memory allocation error.
        /// </summary>
        public const int NOMEM = 7;
        /// <summary>
        /// Function isn't supported.
        /// </summary>
        public const int NOTSUPPORTED = 8;
        /// <summary>
        /// Error value out of range.
        /// </summary>
        public const int BADERRNUM = 9;
        /// <summary>
        /// Invalid flag passed.
        /// </summary>
        public const int INVALFLAG = 1;
        /// <summary>
        /// Invalid parameter passed.
        /// </summary>
        public const int INVALPARAM = 11;
        /// <summary>
        /// Handle being used simultaneously on another thread (eg callback).
        /// </summary>
        public const int HANDLEBUSY = 12;
        /// <summary>
        /// Specified alias not found.
        /// </summary>
        public const int INVALIDALIAS = 13;
        /// <summary>
        /// Bad registry database.
        /// </summary>
        public const int BADDB = 14;
        /// <summary>
        /// Registry key not found.
        /// </summary>
        public const int KEYNOTFOUND = 15;
        /// <summary>
        /// Registry read error.
        /// </summary>
        public const int READERROR = 16;
        /// <summary>
        /// Registry write error.
        /// </summary>
        public const int WRITEERROR = 17;
        /// <summary>
        /// Eegistry delete error.
        /// </summary>
        public const int DELETEERROR = 18;
        /// <summary>
        /// Registry value not found. 
        /// </summary>
        public const int VALNOTFOUND = 19;
        /// <summary>
        /// Driver does not call DriverCallback.
        /// </summary>
        public const int NODRIVERCB = 20;
        /// <summary>
        /// Last error in range.
        /// </summary>
        public const int LASTERROR = 20;
    }
}
