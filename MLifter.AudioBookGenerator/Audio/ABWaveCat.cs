using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using MLifter.AudioBookGenerator.Properties;

namespace MLifterAudioBookGenerator.Audio
{
    /// <summary>
    /// This class is able to concatenate multiple wave files.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-03-30</remarks>
    class ABWaveCat
    {
        /// <summary>
        /// Concatenates the specified files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="outfile">The outfile.</param>
        /// <param name="stereo">if set to <c>true</c> [stereo].</param>
        /// <param name="worker">The worker.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public void Concatenate(List<MediaFieldFile> files, FileInfo outfile, bool stereo)
        {
            WaveCat wa_IN = new WaveCat();
            WaveCat wa_out = new WaveCat();

            wa_out.DataLength = 0;
            wa_out.length = 0;
            wa_out.channels = (short)(stereo ? 2 : 1);
            wa_out.BitsPerSample = 0;
            wa_out.samplerate = 0;

            //gather header data for each audio file
            int count = files.Count;
            int index = 0;
            foreach (MediaFieldFile file in files)
            {
                if (file.mediafield.Type == MediaField.TypeEnum.AudioField && file.ContainsFile && file.Extension.ToLowerInvariant() == Resources.AUDIO_WAVE_EXTENSION.ToLowerInvariant()) //fix for [MLA-1271]: only able to concatenate wave files
                {
                    wa_IN.WaveHeaderIN(file.file.FullName);

                    //take over sampling rate from first input file
                    if (wa_out.samplerate < 1)
                        wa_out.samplerate = wa_IN.samplerate;

                    //take over bits per sample from first input file
                    if (wa_out.BitsPerSample < 1)
                        wa_out.BitsPerSample = wa_IN.BitsPerSample;

                    //check if input wave properties are conform to output wave properties
                    if (wa_IN.BitsPerSample != wa_out.BitsPerSample)
                        BusinessLayer.AddLog(string.Format("Warning: {0} Bits Per Sample instead of {1} in {2}", wa_IN.BitsPerSample, wa_out.BitsPerSample, file.file.Name));
                    if (wa_IN.samplerate != wa_out.samplerate)
                        BusinessLayer.AddLog(string.Format("Warning: {0} Samplingrate instead of {1} in {2}", wa_IN.samplerate, wa_out.samplerate, file.file.Name));

                    if (wa_IN.channels == wa_out.channels)
                    {
                        wa_out.DataLength += wa_IN.DataLength;
                        wa_out.length += wa_IN.DataLength;
                    }
                    else
                    {
                        wa_out.DataLength += Convert.ToInt32(1.0 * wa_IN.length / wa_IN.channels * wa_out.channels);
                        wa_out.length += Convert.ToInt32(1.0 * wa_IN.length / wa_IN.channels * wa_out.channels);
                    }
                }
                BusinessLayer.ReportProgress(index++, count);
            }

            //generate the new header out of last input file
            //wa_out.BitsPerSample = wa_IN.BitsPerSample;
            //wa_out.channels = wa_IN.channels;
            //wa_out.samplerate = wa_IN.samplerate;

            //check for samplingrate, if it is right
            if (wa_out.samplerate < 1 || wa_out.BitsPerSample < 1)
            {
                BusinessLayer.AddLog("Error: SamplingRate or BitsPerSample could not be detected properly.");
                return;
            }

            //modify header data length for each silence
            foreach (MediaFieldFile file in files)
            {
                if (file.mediafield.Type == MediaField.TypeEnum.Silence)
                {
                    //just add the silence data length to the header
                    int length = CalculateSilenceLength(file.mediafield.SilenceDuration, wa_out);
                    wa_out.length += length;
                    wa_out.DataLength += length;
                }
            }

            //save the header
            wa_out.WaveHeaderOUT(outfile.FullName);

            //generate the wave data
            index = 0;
            foreach (MediaFieldFile file in files)
            {
                if (file.mediafield.Type == MediaField.TypeEnum.AudioField && file.ContainsFile)
                {
                    wa_IN.WaveHeaderIN(file.file.FullName);

                    FileStream fs = new FileStream(file.file.FullName, FileMode.Open, FileAccess.Read);
                    byte[] arrfile = new byte[fs.Length - 44];
                    fs.Position = 44;
                    fs.Read(arrfile, 0, arrfile.Length);
                    fs.Close();

                    if (wa_IN.channels != wa_out.channels)
                    {
                        //transform from mono to stereo
                        if (wa_IN.channels == 1 && wa_out.channels == 2)
                            arrfile = TransformMonoToStereo(arrfile);
                        //transform from stereo to mono
                        if (wa_IN.channels == 2 && wa_out.channels == 1)
                            arrfile = TransformStereoToMono(arrfile);
                    }

                    FileStream fo = new FileStream(outfile.FullName, FileMode.Append, FileAccess.Write);
                    BinaryWriter bw = new BinaryWriter(fo);
                    bw.Write(arrfile);
                    bw.Close();
                    fo.Close();
                }
                else if (file.mediafield.Type == MediaField.TypeEnum.Silence)
                {
                    FileStream fo = new FileStream(outfile.FullName, FileMode.Append, FileAccess.Write);
                    BinaryWriter bw = new BinaryWriter(fo);
                    bw.Write(GenerateSilence(file.mediafield.SilenceDuration, wa_out));
                    bw.Close();
                    fo.Close();
                }
                BusinessLayer.ReportProgress(index++, count);
            }
        }

        /// <summary>
        /// Transforms the stereo signal to mono.
        /// </summary>
        /// <param name="arrfile">The arrfile.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-31</remarks>
        private static byte[] TransformStereoToMono(byte[] arrfile)
        {
            byte[] newarrfile = new byte[arrfile.Length / 2];
            for (int i = 0; i < newarrfile.Length; i += 2)
            {
                newarrfile[i] = arrfile[2 * i + 0];
                newarrfile[i + 1] = arrfile[2 * i + 1];
            }
            arrfile = newarrfile;
            return arrfile;
        }

        /// <summary>
        /// Transforms the mono signal to stereo.
        /// </summary>
        /// <param name="arrfile">The arrfile.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-31</remarks>
        private static byte[] TransformMonoToStereo(byte[] arrfile)
        {
            byte[] newarrfile = new byte[arrfile.Length * 2];
            for (int i = 0; i < arrfile.Length; i += 2)
            {
                newarrfile[2 * i + 0] = arrfile[i];
                newarrfile[2 * i + 1] = arrfile[i + 1];
                newarrfile[2 * i + 2] = arrfile[i];
                newarrfile[2 * i + 3] = arrfile[i + 1];

            }
            arrfile = newarrfile;
            return arrfile;
        }

        /// <summary>
        /// Generates a wave buffer, consisting of silence.
        /// </summary>
        /// <param name="length">The length of silence, in seconds.</param>
        /// <param name="wave">The wave object, containing bitrate, channels, etc.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private byte[] GenerateSilence(double length, WaveCat wave)
        {
            int buffersize = CalculateSilenceLength(length, wave);

            //generate empty byte buffer with the required length
            byte[] data = new byte[buffersize];
            return data;
        }

        /// <summary>
        /// Calculates the length of the silence.
        /// </summary>
        /// <param name="length">The length of silence, in seconds..</param>
        /// <param name="wave">The wave object, containing bitrate, channels, etc..</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private int CalculateSilenceLength(double length, WaveCat wave)
        {
            int samplecount = Convert.ToInt32(length * wave.samplerate);
            int buffersize = samplecount * wave.channels * wave.BitsPerSample / 8;

            return buffersize;
        }
    }
}
