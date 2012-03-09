using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LameDOTnet
{
    /// <summary>
    /// This class contains the lame_enc.dll wrapper.
    /// </summary>
    public class Lame
    {
        # region " lame structs, enums and variables"
        /// <summary>
        /// Error codes of the lame-encoder
        /// </summary>
        protected enum LAME_ERR : uint
        {
            SUCCESSFUL                 = 0,
            INVALID_FORMAT             = 1,
            INVALID_FORMAT_PARAMETERS  = 2,
            NO_MORE_HANDLES            = 3,
            INVALID_HANDLE             = 4
        }

        private const uint LAME_ERR_SUCCESSFUL = 0;
        private const uint LAME_ERR_INVALID_FORMAT = 1;
        private const uint LAME_ERR_INVALID_FORMAT_PARAMETERS = 2;
        private const uint LAME_ERR_NO_MORE_HANDLES = 3;
        private const uint LAME_ERR_INVALID_HANDLE = 4;
        
        /// <summary>
        /// The quality presets for lame.
        /// </summary>
		public enum LAME_QUALITY_PRESET : int
	  	{
			LQP_NOPRESET			=-1,
			LQP_NORMAL_QUALITY		= 0,
			LQP_LOW_QUALITY			= 1,
			LQP_HIGH_QUALITY		= 2,
			LQP_VOICE_QUALITY		= 3,
			LQP_R3MIX				= 4,
			LQP_VERYHIGH_QUALITY	= 5,
			LQP_STANDARD			= 6,
			LQP_FAST_STANDARD		= 7,
			LQP_EXTREME				= 8,
			LQP_FAST_EXTREME		= 9,
			LQP_INSANE				= 10,
			LQP_ABR					= 11,
			LQP_CBR					= 12,
			LQP_MEDIUM				= 13,
			LQP_FAST_MEDIUM			= 14,

			LQP_PHONE	            = 1000,
			LQP_SW		            = 2000,
			LQP_AM		            = 3000,
			LQP_FM		            = 4000,
			LQP_VOICE	            = 5000,
			LQP_RADIO	            = 6000,
			LQP_TAPE	            = 7000,
			LQP_HIFI	            = 8000,
			LQP_CD		            = 9000,
			LQP_STUDIO	            = 10000
        }

        /// <summary>
        /// The channel mode of / for an mpeg-stream.
        /// </summary>
		protected enum MPEG_MODE : uint 
		{
			STEREO = 0,
			JOINT_STEREO,
            /// <summary>
            /// Not supported!
            /// </summary>
			DUAL_CHANNEL,
			MONO,
			NOT_SET,
            /// <summary>
            /// Do not use!
            /// </summary>
			MAX_INDICATOR
		}
        
        /// <summary>
        /// VBR method for encoding.
        /// </summary>
		public enum VBR_METHOD : int
	  	{
			VBR_METHOD_NONE			= -1,
			VBR_METHOD_DEFAULT	    =  0,
			VBR_METHOD_OLD			=  1,
			VBR_METHOD_NEW			=  2,
			VBR_METHOD_MTRH			=  3,
			VBR_METHOD_ABR			=  4
		}

        /// <summary>
        /// Sound format.
        /// </summary>
        protected enum SOUND_FORMAT
        {
            # region " standard formats "
            /// <summary>
            /// Microsoft WAV format (little endian).
            /// </summary>
            WAV         = 0x010000,
            /// <summary>
            /// Apple/SGI AIFF format (big endian).
            /// </summary>
            AIFF        = 0x020000,
            /// <summary>
            /// Sun/NeXT AU format (big endian).
            /// </summary>
            AU          = 0x030000,
            /// <summary>
            /// RAW PCM data.
            /// </summary>
            RAW         = 0x040000,
            /// <summary>
            /// Ensoniq PARIS file format.
            /// </summary>
            PAF         = 0x050000,
            /// <summary>
            /// Amiga IFF / SVX8 / SV16 format.
            /// </summary>
            SVX         = 0x060000,
            /// <summary>
            /// Sphere NIST format.
            /// </summary>
            NIST        = 0x070000,
            /// <summary>
            /// VOC files.
            /// </summary>
            VOC         = 0x080000,
            /// <summary>
            /// Berkeley/IRCAM/CARL.
            /// </summary>
            IRCAM       = 0x0A0000,
            /// <summary>
            /// Sonic Foundry's 64 bit RIFF/WAV.
            /// </summary>
            WAV64       = 0x0B0000,
            /// <summary>
            /// Matlab (tm) V4.2.
            /// </summary>
            MATLAB4     = 0x0C0000,
            /// <summary>
            /// Matlab (tm) V5.0.
            /// </summary>
            MATLAB5     = 0x0D0000,
            /// <summary>
            /// Portable Voice Format.
            /// </summary>
            PVF         = 0x0E0000,
            /// <summary>
            /// Fast tracker 2 Extended Instrument.
            /// </summary>
            FT2EI       = 0x0F0000,
            /// <summary>
            /// HMM Tool Kit format.
            /// </summary>
            HTK         = 0x100000,
            /// <summary>
            /// Midi Sample Dump Standard.
            /// </summary>
            MSDS        = 0x110000,
            # endregion
            # region " PCM sub formats "
            /// <summary>
            /// Signed 8 bit PCM data.
            /// </summary>
            PCM_S8 = 0x0001,
            /// <summary>
            /// Signed 16 bit PCM data.
            /// </summary>
            PCM_S16 = 0x0002,
            /// <summary>
            /// Signed 24 bit PCM data.
            /// </summary>
            PCM_S24 = 0x0003,
            /// <summary>
            /// Signed 32 bit PCM data.
            /// </summary>
            PCM_S32 = 0x0004,
            /// <summary>
            /// Unsigned 8 bit WAV or RAW data
            /// </summary>
            PCM_U8 = 0x0005,
            # endregion
            # region " ADPCM formats"
            /// <summary>
            /// Dialogix ADPCM
            /// </summary>
            DX_ADPCM = 0x0021,
            /// <summary>
            /// 32kbs G721 ADPCM.
            /// </summary>
            G721_32 = 0x0030,
            /// <summary>
            /// 24kbs G723 ADPCM.
            /// </summary>
            G723_24 = 0x0031,
            /// <summary>
            /// 40kbs G723 ADPCM.
            /// </summary>
            G723_40 = 0x0032,
            /// <summary>
            /// IMA ADPCM.
            /// </summary>
            ADPCM_IMA = 0x0012,
            /// <summary>
            /// Microsoft ADPCM.
            /// </summary>
            ADPCM_MS = 0x0013,
            # endregion
            # region " Delta Width Variable Word formats "
            /// <summary>
            /// 12bit Delta Width Variable Word.
            /// </summary>
            DWVW_12 = 0x0040,
            /// <summary>
            /// 16bit Delta Width Variable Word.
            /// </summary>
            DWVW_16 = 0x0041,
            /// <summary>
            /// 24bit Delta Width Variable Word.
            /// </summary>
            DWVW_24 = 0x0042,
            /// <summary>
            /// n-bit Delta Width Variable Word.
            /// </summary>
            DWVM_N = 0x0043,
            # endregion
            # region " DPCM formats "
            /// <summary>
            /// 8 bit differential PCM.
            /// </summary>
            DPCM_8 = 0x0050,
            /// <summary>
            /// 16 bit differential PCM.
            /// </summary>
            DPCM_16 = 0x0051,
            # endregion
            # region " float data "
            /// <summary>
            /// 32bit float data.
            /// </summary>
            FLOAT = 0x0006,
            /// <summary>
            /// 64bit float data.
            /// </summary>
            DOUBLE = 0x0007,
            # endregion
            # region " other formats "
            /// <summary>
            /// U-Law.
            /// </summary>
            ULAW = 0x0010,
            /// <summary>
            /// A-Law.
            /// </summary>
            ALAW = 0x0011,
            /// <summary>
            /// GSM 6.10.
            /// </summary>
            GSM610 = 0x0020,
            # endregion
            # region " endian options "
            /// <summary>
            /// File endian.
            /// </summary>
            ENDIAN_FILE = 0x00000000,
            /// <summary>
            /// Little endian.
            /// </summary>
            ENDIAN_LITTLE = 0x10000000,
            /// <summary>
            /// Big endian.
            /// </summary>
            ENDIAN_BIG = 0x20000000,
            /// <summary>
            /// CPU endian.
            /// </summary>
            ENDIAN_CPU = 0x30000000,
            # endregion
            # region " other "
            FORMAT_SUBMASK = 0x0000FFFF,
            FORMAT_TYPEMASK = 0x0FFF0000,
            FORMAT_ENDMASK = 0x30000000
            # endregion
        }

        /// <summary>
        /// Holds all version informations of a lame_enc.dll
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class LAME_VERSION
        {
            public const uint BE_MAX_HOMEPAGE = 256;
            public byte byDLLMajorVersion;
            public byte byDLLMinorVersion;
            public byte byMajorVersion;
            public byte byMinorVersion;

            // Release date of the linked lame_enc.dll:
            public byte byDay;
            public byte byMonth;
            public ushort wYear;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
            public string zHomepage;

            public byte byAlphaLevel;
            public byte byBetaLevel;
            public byte byMMXEnabled;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 125)]
            public byte[] btReserved;

            public LAME_VERSION() { btReserved = new byte[125]; }
        }

        /// <summary>
        /// Holds the config of a lame-encoder.
        /// </summary>
        [StructLayout(LayoutKind.Sequential), Serializable]
        protected class LAME_CONFIG
        {
            public const uint BE_CONFIG_MP3 = 0;
            public const uint BE_CONFIG_LAME = 256;
            
            public uint dwConfig;
            public config union;

            public LAME_CONFIG(SOUND_INFO soundInfo, MP3_Settings settings)
            {
                dwConfig = BE_CONFIG_LAME;
                union = new config(soundInfo, settings);
            }
        }

        /// <summary>
        /// Hold de detailed configuration.
        /// </summary>
        [StructLayout(LayoutKind.Explicit), Serializable]
        protected class config
        {
            [FieldOffset(0)]
            public MP3 mp3;
            [FieldOffset(0)]
            public LameHeaderVersion1 lhv1;
            [FieldOffset(0)]
            public ACC acc;

            public config(SOUND_INFO soundInfo, MP3_Settings settings)
            {
                lhv1 = new LameHeaderVersion1(soundInfo, settings);
            }
        }

        /// <summary>
        /// The acc settings.
        /// </summary>
        [StructLayout(LayoutKind.Sequential), Serializable]
        protected struct ACC
        {
            public uint dwSampleRate;
            public byte byMode;
            public ushort wBitrate;
            public byte byEncodingMethod;
        }

        /// <summary>
        /// The lame header configuration.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 327), Serializable]
        protected struct LameHeaderVersion1
        {
            # region " mpeg-version "
            public const uint MPEG1 = 1;
            public const uint MPEG2 = 0;
            # endregion
            # region " informations "
            public uint dwStructVersion;
            public uint dwStructSize;
            # endregion
            # region " encoder settings "
            /// <summary>
            /// Sample rate of the input file.
            /// </summary>
            public uint dwSampleRate;
            /// <summary>
            /// Sample rate for resampling; 0 = lame will decide.
            /// </summary>
            public uint dwReSampleRate;
            /// <summary>
            /// Stereo or mono.
            /// </summary>
            public MPEG_MODE nMode;
            /// <summary>
            /// CBR bitrate or VBR min. bitrate.
            /// </summary>
            public uint dwBitrate;
            /// <summary>
            /// Max. bitrate for VBR; ignored at CBR.
            /// </summary>
            public uint dwMaxBitrate;
            /// <summary>
            /// The quality preset.
            /// </summary>
            public LAME_QUALITY_PRESET nPreset;
            /// <summary>
            /// mpeg-version (MPEG-1 or MPEG-2).
            /// </summary>
            public uint dwMpegVersion;
            /// <summary>
            /// For future use, set to 0.
            /// </summary>
            public uint dwPsyModel;
            /// <summary>
            /// For future use, set to 0.
            /// </summary>
            public uint dwEmphasis;
            # endregion
            # region " bit settings "
            /// <summary>
            /// Set the private bit; 0 = false, 1 = true.
            /// </summary>
            public int bPrivate;
            /// <summary>
            /// Set the CRC bit; 0 = false, 1 = true.
            /// </summary>
            public int bCRC;
            /// <summary>
            /// Set the copyright bit; 0 = false, 1 = true.
            /// </summary>
            public int bCopyright;
            /// <summary>
            /// Set the original bit; 0 = false, 1 = true.
            /// </summary>
            public int bOriginal;
            # endregion
            # region " VBR settings "
            /// <summary>
            /// Write the XING VBR header; 0 = false, 1 = true.
            /// </summary>
            public int bWriteVBRHeader;
            /// <summary>
            /// Use VBR encoding; 0 = false, 1 = true.
            /// </summary>
            public int bEnableVBR;
            /// <summary>
            /// VBR quality mode; 0...9.
            /// </summary>
            public int nVBRQuality;
            /// <summary>
            /// Use ABR in stead of VBR.
            /// </summary>
            public uint dwVbrAbr_bps;
            /// <summary>
            /// VBR method
            /// </summary>
            public VBR_METHOD nVbrMethod;
            /// <summary>
            /// Disable bit reservoir; 0 = false, 1 = true.
            /// </summary>
            public int bNoRes;
            # endregion
            # region " other settings "
            /// <summary>
            /// Use strict ISO encoding rules; 0 = false, 1 = true.
            /// </summary>
            public int bStrictIso;
            /// <summary>
            /// Quality settings, high byte should not be low byte, otherwise quality = 5.
            /// </summary>
            public ushort nQuality;
            # endregion
            # region " constructor "
            public LameHeaderVersion1(SOUND_INFO soundInfo, MP3_Settings settings)
            {
                #region " Check the input format "
                if (soundInfo.format != ((int)SOUND_FORMAT.WAV | (int)SOUND_FORMAT.PCM_S16))
                {
                    throw new InvalidDataException("Wrong format!", new Exception("Only 16 bit uncompressed WAV supported. You gave " + soundInfo.format));
                }
                # endregion

                dwStructVersion = 1;
                dwStructSize = (uint)Marshal.SizeOf(typeof(LAME_CONFIG));

                # region " Check input sample rate "
                switch (soundInfo.samplerate)
                {
                    case 16000:
                    case 22050:
                    case 24000:
                        dwMpegVersion = MPEG2;
                        break;
                    case 32000:
                    case 44100:
                    case 48000:
                        dwMpegVersion = MPEG1;
                        break;
                    default:
                        throw new InvalidDataException("Wrong format!", new Exception("Sample rate " + soundInfo.samplerate + " not supported."));
                }
                # endregion

                dwSampleRate = (uint)soundInfo.samplerate;
                dwReSampleRate = 0;

                # region " Set encoding channels "
                switch (soundInfo.channels)
                {
                    case 1:
                        nMode = MPEG_MODE.MONO;
                        break;
                    case 2:
                        nMode = MPEG_MODE.STEREO;
                        break;
                    default:
                        throw new InvalidDataException("Wrong format!", new Exception("Invalid number of channels:" + soundInfo.channels));
                }
                # endregion
                # region " Check encoding bit rate "
                switch (settings.Bitrate)
                {
                    case 32:
                    case 40:
                    case 48:
                    case 56:
                    case 64:
                    case 80:
                    case 96:
                    case 112:
                    case 128:
                    case 160:
                        break;
                    // Allowed only in MPEG1:
                    case 192:
                    case 224:
                    case 256:
                    case 320:
                        if (dwMpegVersion != MPEG1)
                        {
                            throw new InvalidDataException("Wrong mp3 bit rate!", new Exception("Incompatible bit rate:" + settings.Bitrate));
                        }
                        break;
                    // Allowed only in MPEG2:
                    case 8:
                    case 16:
                    case 24:
                    case 144:
                        if (dwMpegVersion != MPEG2)
                        {
                            throw new InvalidDataException("Wrong mp3 bit rate!", new Exception("Incompatible bit rate:" + settings.Bitrate));
                        }
                        break;
                    default:
                        throw new InvalidDataException("Wrong mp3 bit rate!", new Exception("Can't support bit rate"));
                }
                # endregion

                dwBitrate = settings.Bitrate;
                nPreset = settings.QualityPreset;

                bEnableVBR = settings.VBR_enabled ? 1 : 0;
                dwMaxBitrate = settings.VBR_maxBitrate;
                dwVbrAbr_bps = 0;
                nQuality = 0;
                nVbrMethod = settings.VBR_method;
                nVBRQuality = settings.VBR_Quality;
                bWriteVBRHeader = settings.VBR_WriteHeader ? 1 : 0;

                bOriginal = settings.OriginalBit ? 1 : 0;
                bCopyright = settings.CopyrightBit ? 1 : 0;
                bCRC = settings.CRC_Bit ? 1 : 0;
                bPrivate = settings.PrivatBit ? 1 : 0;

                bNoRes = settings.DisableBitReservoir ? 1 : 0;
                bStrictIso = settings.StrictISOencoding ? 1 : 0;

                dwPsyModel = 0;
                dwEmphasis = 0;
            }
            # endregion
        }

        /// <summary>
        /// MP3 configuration
        /// </summary>
        [StructLayout(LayoutKind.Sequential), Serializable]
        protected struct MP3
        {
            /// <summary>
            /// Sample rate; 48000, 44100 and 32000 allowed.
            /// </summary>
            public uint dwSampleRate;
            /// <summary>
            /// Encoding mode (STEREO, DUALCHANNEL or MONO).
            /// </summary>
            public byte byMode;
            /// <summary>
            /// Encoding bitrate: 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256 and 320 allowed.
            /// </summary>
            public ushort wBitrate;

            /// <summary>
            /// Private bit.
            /// </summary>
            public int bPrivate;
            /// <summary>
            /// CRC bit.
            /// </summary>
            public int bCRC;
            /// <summary>
            /// Copyright bit.
            /// </summary>
            public int bCopyright;
            /// <summary>
            /// Original bit.
            /// </summary>
            public int bOriginal;
        }

        /// <summary>
        /// Holds all information about an audio file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct SOUND_INFO
        {
            public System.Int64 frames;
            public int samplerate;
            public int channels;
            public int format;
            public int sections;
            public int seekable;
        };
        # endregion
        # region " lame_enc.dll and libsndfile.dll inter ops "
        /// <summary>
        /// This function is the first to call before starting an encoding stream.
        /// </summary>
        /// <param name="Config">Encoder settings</param>
        /// <param name="Samples">Receives the number of samples (not bytes, each sample is a SHORT) to send to each beEncodeChunk() on return.</param>
        /// <param name="OutputBufferSize">Receives the minimum number of bytes that must have the output(result) buffer</param>
        /// <param name="StreamHandle">Receives the stream handle on return</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        [DllImport("lame_enc.dll")]
        protected static extern uint beInitStream(LAME_CONFIG Config, ref uint Samples, ref uint OutputBufferSize, ref uint StreamHandle);

        /// <summary>
        /// Encodes a chunk of samples. Please note that if you have set the output to 
        /// generate mono MP3 files you must feed beEncodeChunk() with mono samples
        /// </summary>
        /// <param name="StreamHandle">Handle of the stream.</param>
        /// <param name="Samples">Number of samples to be encoded for this call. 
        /// This should be identical to what is returned by beInitStream(), 
        /// unless you are encoding the last chunk, which might be smaller.</param>
        /// <param name="InSamples">Array of 16-bit signed samples to be encoded. 
        /// These should be in stereo when encoding a stereo MP3 
        /// and mono when encoding a mono MP3</param>
        /// <param name="Output">Buffer where to write the encoded data. 
        /// This buffer should be at least of the minimum size returned by beInitStream().</param>
        /// <param name="Written">Returns the number of bytes of encoded data written. 
        /// The amount of data written might vary from chunk to chunk</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        [DllImport("lame_enc.dll")]
        private static extern uint beEncodeChunk(uint StreamHandle, uint Samples, short[] InSamples, [In, Out] byte[] Output, ref uint Written);

        /// <summary>
        /// Encodes a chunk of samples. Please note that if you have set the output to 
        /// generate mono MP3 files you must feed beEncodeChunk() with mono samples
        /// </summary>
        /// <param name="StreamHandle">Handle of the stream.</param>
        /// <param name="SampleCount">Number of samples to be encoded for this call. 
        /// This should be identical to what is returned by beInitStream(), 
        /// unless you are encoding the last chunk, which might be smaller.</param>
        /// <param name="Samples">Pointer at the 16-bit signed samples to be encoded. 
        /// InPtr is used to pass any type of array without need of make memory copy, 
        /// then gaining in performance. Note that nSamples is not the number of bytes,
        /// but samples (is sample is a SHORT)</param>
        /// <param name="Output">Buffer where to write the encoded data. 
        /// This buffer should be at least of the minimum size returned by beInitStream().</param>
        /// <param name="Written">Returns the number of bytes of encoded data written. 
        /// The amount of data written might vary from chunk to chunk</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        [DllImport("lame_enc.dll")]
        private static extern uint beEncodeChunk(uint StreamHandle, uint SampleCount, IntPtr Samples, [In, Out] byte[] Output, ref uint Written);

        /// <summary>
        /// Encodes a chunk of samples. Samples are contained in a byte array
        /// </summary>
        /// <param name="StreamHandle">Handle of the stream.</param>
        /// <param name="buffer">Bytes to encode</param>
        /// <param name="index">Position of the first byte to encode</param>
        /// <param name="bytes">Number of bytes to encode (not samples, samples are two byte length)</param>
        /// <param name="Output">Buffer where to write the encoded data.
        /// This buffer should be at least of the minimum size returned by beInitStream().</param>
        /// <param name="Written">Returns the number of bytes of encoded data written. 
        /// The amount of data written might vary from chunk to chunk</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        protected static uint EncodeChunk(uint StreamHandle, byte[] buffer, int index, uint bytes, byte[] Output, ref uint Written)
        {
            uint res;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                IntPtr ptr = (IntPtr)(handle.AddrOfPinnedObject().ToInt32() + index);
                res = beEncodeChunk(StreamHandle, bytes / 2, ptr, Output, ref Written);
            }
            finally
            {
                handle.Free();
            }

            return res;
        }

        /// <summary>
        /// Encodes a chunk of samples. Samples are contained in a byte array
        /// </summary>
        /// <param name="StreamHandle">Handle of the stream.</param>
        /// <param name="buffer">Bytes to encode</param>
        /// <param name="Output">Buffer where to write the encoded data.
        /// This buffer should be at least of the minimum size returned by beInitStream().</param>
        /// <param name="Written">Returns the number of bytes of encoded data written. 
        /// The amount of data written might vary from chunk to chunk</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        protected static uint EncodeChunk(uint StreamHandle, byte[] buffer, byte[] Output, ref uint Written)
        {
            return EncodeChunk(StreamHandle, buffer, 0, (uint)buffer.Length, Output, ref Written);
        }

        /// <summary>
        /// This function should be called after encoding the last chunk in order to flush 
        /// the encoder. It writes any encoded data that still might be left inside the 
        /// encoder to the output buffer. This function should NOT be called unless 
        /// you have encoded all of the chunks in your stream.
        /// </summary>
        /// <param name="StreamHandle">Handle of the stream.</param>
        /// <param name="Output">Where to write the encoded data. This buffer should be 
        /// at least of the minimum size returned by beInitStream().</param>
        /// <param name="Written">Returns number of bytes of encoded data written.</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        [DllImport("lame_enc.dll")]
        protected static extern uint beDeinitStream(uint StreamHandle, [In, Out] byte[] Output, ref uint Written);

        /// <summary>
        /// Last function to be called when finished encoding a stream. 
        /// Should unlike beDeinitStream() also be called if the encoding is canceled.
        /// </summary>
        /// <param name="StreamHandle">Handle of the stream.</param>
        /// <returns>On success: LAME_ERR_SUCCESSFUL</returns>
        [DllImport("lame_enc.dll")]
        protected static extern uint beCloseStream(uint StreamHandle);
        
        /// <summary>
        /// Returns information like version numbers (both of the DLL and encoding engine), 
        /// release date and URL for lame_enc's homepage. 
        /// All this information should be made available to the user of your product 
        /// through a dialog box or something similar.
        /// </summary>
        /// <param name="version">Where version number, release date and URL for homepage is returned.</param>
        [DllImport("lame_enc.dll")]
        protected static extern void beVersion([Out] LAME_VERSION version);
        
        [DllImport("lame_enc.dll", CharSet = CharSet.Ansi)]
        protected static extern void beWriteVBRHeader(string MP3FileName);

        [DllImport("lame_enc.dll")]
        protected static extern uint beEncodeChunkFloatS16NI(uint StreamHandle, uint Samples, [In]float[] buffer_l, [In]float[] buffer_r, [In, Out]byte[] Output, ref uint Written);

        [DllImport("lame_enc.dll")]
        protected static extern uint beFlushNoGap(uint StreamHandle, [In, Out]byte[] Output, ref uint Written);

        [DllImport("lame_enc.dll", CharSet = CharSet.Ansi)]
        protected static extern uint beWriteInfoTag(uint StreamHandle, string FileName);

        /// <summary>
        /// Get some infos about an audio file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="soundInfo">The info.</param>
        /// <returns></returns>
        protected int GetSoundFileInfo(string filename, ref SOUND_INFO soundInfo)
        {
            IntPtr errorRef = sf_open(filename, 0x10, ref soundInfo);

            int status = sf_error(errorRef);
            int c = sf_close(errorRef);

            return status;
        }

        [DllImport("libsndfile.dll")]
        private static extern IntPtr sf_open([MarshalAs(UnmanagedType.LPStr)] string path, int mode, ref SOUND_INFO soundInfo);
        [DllImport("libsndfile.dll")]
        private static extern int sf_error(IntPtr sndfile);
        [DllImport("libsndfile.dll")]
        private static extern int sf_close(IntPtr sndfile);
        # endregion
        # region " variables "
        private bool closed = true;

        private uint Samples;
        private uint outputBufferSize;
        public uint OutputBufferSize { get { return outputBufferSize; } }
        private uint StreamHandle;

        private int optimalBufferSize;
        public int OptimalBufferSize { get { return optimalBufferSize; } }

        private string inputWaveFile;

        private Stream InputStream;
        private Stream OutputStream;

        private byte[] InputBuffer;
        private byte[] OutputBuffer;
        # endregion
        # region " mp3 settings "
        public class MP3_Settings
        {
            # region " basic settings "
            /// <summary>
            /// Encoding bitrate: 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256 and 320 allowed.
            /// Min. bitrate in VBR mode.
            /// </summary>
            public uint Bitrate = 128;
            /// <summary>
            /// The overall quality of the encoding.
            /// </summary>
            public LAME_QUALITY_PRESET QualityPreset = LAME_QUALITY_PRESET.LQP_NORMAL_QUALITY;
            # endregion
            # region " VBR settings "
            /// <summary>
            /// If <i>true</i>, the encoder uses VBR.
            /// </summary>
            public bool VBR_enabled = false;
            /// <summary>
            /// Max. bitrate in VBR mode.
            /// </summary>
            public uint VBR_maxBitrate = 192;
            /// <summary>
            /// The used method for VBR encoding.
            /// </summary>
            public VBR_METHOD VBR_method = VBR_METHOD.VBR_METHOD_DEFAULT;
            /// <summary>
            /// The quality for the VBR encoding, must be value between 0...8.
            /// </summary>
            public int VBR_Quality = 0;
            /// <summary>
            /// If <i>true</i>, an extra XING VBR header will be written.
            /// </summary>
            public bool VBR_WriteHeader = false;
            # endregion
            # region " bit settings "
            /// <summary>
            /// If <i>true</i>, the original bit will be set.
            /// </summary>
            public bool OriginalBit = true;
            /// <summary>
            /// If <i>true</i>, the copyright bit will be set.
            /// </summary>
            public bool CopyrightBit = false;
            /// <summary>
            /// If <i>true</i>, the CRC bit will be set.
            /// </summary>
            public bool CRC_Bit = false;
            /// <summary>
            /// If <i>true</i>, the private bit will be set.
            /// </summary>
            public bool PrivatBit = false;
            # endregion
            # region " other settings 
            /// <summary>
            /// If <i>true</i>, the bit reservoir will be disabled.
            /// </summary>
            public bool DisableBitReservoir = false;
            /// <summary>
            ///  If <i>true</i>, the encoder will use a strict ISO encoding.
            /// </summary>
            public bool StrictISOencoding = false;
            # endregion
        };
        # endregion
        # region " constructor "
        /// <summary>
        /// Initializes a new instance of the <see cref="Lame"/> class.
        /// </summary>
        /// <param name="settings">The encoding settings.</param>
        public Lame(MP3_Settings settings, int Channels, int SampleRate)
        {
            SOUND_INFO info = new SOUND_INFO();
            info.format = 0x010002;
            info.samplerate = SampleRate;
            info.channels = Channels;

            LAME_CONFIG mp3config = new LAME_CONFIG(info, settings);

            if (beInitStream(mp3config, ref Samples, ref outputBufferSize, ref StreamHandle) != LAME_ERR_SUCCESSFUL)
                throw new Exception("Failed to initialize lame!");

            optimalBufferSize = 2 * (int)Samples;
            InputBuffer = new byte[optimalBufferSize];
            OutputBuffer = new byte[OutputBufferSize];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lame"/> class.
        /// </summary>
        /// <param name="InputWaveFile">The input wave file.</param>
        /// <param name="settings">The encoding settings.</param>
        public Lame(string InputWaveFile, MP3_Settings settings)
        {
            inputWaveFile = InputWaveFile;

            SOUND_INFO info = new SOUND_INFO();
            GetSoundFileInfo(InputWaveFile, ref info);

            LAME_CONFIG mp3config = new LAME_CONFIG(info, settings);

            if (beInitStream(mp3config, ref Samples, ref outputBufferSize, ref StreamHandle) != LAME_ERR_SUCCESSFUL)
                throw new Exception("Failed to initialize lame!");

            optimalBufferSize = 2 * (int)Samples;
            InputBuffer = new byte[optimalBufferSize];
            OutputBuffer = new byte[OutputBufferSize];
        }
        # endregion
        # region " encoding "
        /// <summary>
        /// Encodes to the specified output Mp3 file.
        /// </summary>
        /// <param name="OutputMP3File">The output Mp3 file.</param>
        /// <param name="status">The status.</param>
        public void EncodeWAV(string OutputMP3File, Delegate status)
        {
            closed = false;

            InputStream = File.OpenRead(inputWaveFile);
            OutputStream = File.Open(OutputMP3File, FileMode.Create);

            long total = InputStream.Length;
            int readed = 0;
            int read = 0;

            while ((read = InputStream.Read(InputBuffer, 0, InputBuffer.Length)) > 0)
            {
                uint encoded = 0;

                if (EncodeChunk(StreamHandle, InputBuffer, OutputBuffer, ref encoded) == LAME_ERR_SUCCESSFUL)
                {
                    if (encoded > 0)
                        OutputStream.Write(OutputBuffer, 0, (int)encoded);
                }
                else
                    throw new Exception("Chunk failed to encode");

                readed += read;
                double pos = ((double)readed * (double)100) / (double)total;
                status.DynamicInvoke(new object[] { pos });
            }
        }

        /// <summary>
        /// Encodes the specified chunk to Mp3.
        /// </summary>
        /// <param name="InputBuffer">The input buffer.</param>
        /// <param name="OutputBuffer">The output buffer.</param>
        public uint EncodeChunk(byte[] InputBuffer, byte[] OutputBuffer)
        {
            closed = false;

            uint encoded = 0;
            if (EncodeChunk(StreamHandle, InputBuffer, OutputBuffer, ref encoded) != LAME_ERR_SUCCESSFUL)
                throw new Exception("Chunk failed to encode");

            return encoded;
        }
        # endregion
        # region " other methods "
        /// <summary>
        /// Closes this stream.
        /// </summary>
        public void Close()
        {
            try
            {
                if (!closed)
                {
                    uint encoded = 0;

                    if (beDeinitStream(StreamHandle, OutputBuffer, ref encoded) == LAME_ERR_SUCCESSFUL)
                    {
                        if ((OutputStream != null) && (encoded > 0))
                            OutputStream.Write(OutputBuffer, 0, (int)encoded);
                    }

                    beCloseStream(StreamHandle);
                }
            }
            finally
            {
                if (InputStream != null)
                    InputStream.Close();
                if (OutputStream != null)
                    OutputStream.Close();
            }

            closed = true;
        }

        /// <summary>
        /// Closes this stream.
        /// </summary>
        /// <param name="lastData">The last data to write into the Mp3 file.</param>
        /// <returns>The count of the last bytes, if 0 no more data available.</returns>
        public uint Close(byte[] lastData)
        {
            uint encoded = 0;

            if (!closed)
            {
                if (beDeinitStream(StreamHandle, lastData, ref encoded) != LAME_ERR_SUCCESSFUL)
                    encoded = 0;
                
                beCloseStream(StreamHandle);
            }

            closed = true;

            return encoded;
        }

        /// <summary>
        /// Gets the version of lame.
        /// </summary>
        /// <returns></returns>
        public static LAME_VERSION GetVersion()
        {
            LAME_VERSION version = new LAME_VERSION();
            beVersion(version);

            return version;
        }
        # endregion
    }
}
