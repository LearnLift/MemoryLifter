using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;

using LameDOTnet;
using LumiSoft.Media.Wave;

namespace MLifter.AudioTools
{
    /// <summary>
    /// This class is able to record direct to MP3.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-08-03</remarks>
    public class Recorder : IDisposable
    {
        bool isDisposed = false;
        private bool recording;
        public bool Recording
        {
            get { return recording; }
            set { recording = value; }
        }

        # region Variables
        private int channels;
        private int samplingRate;
        private int queueLength;

        private Stream mp3Stream;
        private Queue<byte[]> buffer;

        private Lame mp3Encoder;
        private Lame.MP3_Settings mp3Settings;
        private WaveIn recorder;
        private bool useMP3encoding;

        private string filename = string.Empty;
        # endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Recorder"/> class.
        /// Uses MP3 encoding.
        /// </summary>
        /// <param name="settings">The settings for the encoding.</param>
        /// <remarks>Documented by Dev05, 2007-08-03</remarks>
        public Recorder(Lame.MP3_Settings settings, int channels, int samplingRate)
        {
            this.channels = channels;
            this.samplingRate = samplingRate;

            mp3Settings = settings;
            mp3Encoder = new Lame(mp3Settings, channels, samplingRate);
            useMP3encoding = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Recorder"/> class.
        /// Uses no encoding (raw pcm).
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <param name="samplingRate">The sampling rate.</param>
        /// <remarks>Documented by Dev02, 2008-04-04</remarks>
        public Recorder(int channels, int samplingRate)
        {
            this.channels = channels;
            this.samplingRate = samplingRate;
            useMP3encoding = false;
        }

        ~Recorder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (recorder != null)
                    recorder.Dispose();
                if (mp3Stream != null)
                    mp3Stream.Close();
                if (mp3Encoder != null)
                    mp3Encoder.Close();
                Debug.WriteLine("Recorder.Dispose() - disposed - " + GC.GetTotalMemory(false));
            }
            isDisposed = true;
        }

        /// <summary>
        /// Starts the recording.
        /// </summary>
        /// <param name="filename">The filename to record to.</param>
        /// <param name="deviceNumber">The device number (-1 for standard device).</param>
        /// <param name="startDelayInMS">The start delay in MS.</param>
        /// <param name="stopDelayInMS">The stop delay in MS - 100ms steps.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        public void StartRecording(string filename, int deviceNumber, int startDelayInMS, int stopDelayInMS)
        {
            if (recording) return;
            recording = true;

            Debug.WriteLine("StartRecording() - " + GC.GetTotalMemory(false));
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            mp3Stream = File.Create(filename);
            this.filename = filename;
            queueLength = (int)(stopDelayInMS / 100);
            buffer = new Queue<byte[]>(queueLength);
            Thread recordingThread = new Thread(new ParameterizedThreadStart(startRecording));
            recordingThread.Name = "Recording Thread";
            recordingThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            recordingThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            recordingThread.Start(new object[] { deviceNumber, startDelayInMS });
        }

        /// <summary>
        /// Starts the recording.
        /// </summary>
        /// <param name="deviceNumber">The device number.</param>
        /// <param name="format">The wave-format.</param>
        /// <param name="delay">The delay to start.</param>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        private void startRecording(object parameters)
        {
            try
            {
                object[] Parameters = (object[])parameters;
                int deviceNumber = (int)Parameters[0];
                int delay = (int)Parameters[1];
                int bytesPerSecond = (samplingRate * 16 * channels) / 8;

                Thread.Sleep(delay);
                recorder = new WaveIn(WaveIn.Devices[deviceNumber > 0 ? deviceNumber : 0], samplingRate, 16, channels, bytesPerSecond / 10);
                recorder.BufferFull += new BufferFullHandler(WaveDataArrived);
                recorder.Start();
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Recording start exception: " + exp.ToString());
            }
        }

        /// <summary>
        /// Called when wave data arrived.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <remarks>Documented by Dev05, 2007-08-08</remarks>
        private void WaveDataArrived(byte[] recBuffer)
        {
            if (recording)
            {
                if ((mp3Stream == null) || !mp3Stream.CanWrite)
                {
                    //something went wrong...
                    this.StopRecording();
                    this.Dispose();
                    return;
                }

                buffer.Enqueue(recBuffer);

                if (buffer.Count > queueLength)
                {
                    try
                    {
                        if (useMP3encoding)
                        {
                            byte[] mp3Buffer = new byte[buffer.Peek().Length * 2];
                            uint encodedBytes = mp3Encoder.EncodeChunk(buffer.Dequeue(), mp3Buffer);

                            //if ((mp3Stream != null) && mp3Stream.CanWrite)
                            mp3Stream.Write(mp3Buffer, 0, (int)encodedBytes);
                        }
                        else
                        {
                            byte[] wavBuffer = buffer.Dequeue();
                            mp3Stream.Write(wavBuffer, 0, wavBuffer.Length);
                        }

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Stops the recording.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-06</remarks>
        public void StopRecording()
        {
            if (recorder == null || !recorder.IsRecording) return;
            recording = false;

            Debug.WriteLine("StopRecording() - " + GC.GetTotalMemory(false));

            if (recorder != null)
            {
                recorder.Stop();
                recorder.Dispose(); //fix for [ML-1311] Audio recording problem on Windows 2000 - important to dispose audio object
            }

            //fix for [ML-541]  ML crash when adding audio to a card - mp3 stream was not closed
            if (mp3Stream != null)
            {
                mp3Stream.Flush();
                mp3Stream.Close();
            }

            Debug.WriteLine("StopRecording() - Adding wave header - " + GC.GetTotalMemory(false));
            //add wave header
            if (!useMP3encoding && !string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                FileStream wavestream = null;
                try
                {
                    //read in wave data
                    FileInfo wavefile = new FileInfo(filename);
                    byte[] rawwave = File.ReadAllBytes(wavefile.FullName);
                    wavefile.Delete();

                    //write out header
                    WaveHeader waveout = new WaveHeader();
                    waveout.BitsPerSample = 16;
                    waveout.channels = Convert.ToInt16(channels);
                    waveout.samplerate = this.samplingRate;
                    waveout.DataLength = rawwave.Length;
                    waveout.length = waveout.DataLength + 44;
                    waveout.WaveHeaderOUT(wavefile.FullName);

                    //calculate audio length
                    int samplecount = waveout.DataLength / waveout.channels / (waveout.BitsPerSample / 8);
                    double length = 1.0 * samplecount / waveout.samplerate;
                    Trace.WriteLine("Length of audio file " + wavefile.FullName + ": " + length.ToString() + " sec");

                    //write out wave data
                    wavestream = File.OpenWrite(wavefile.FullName);
                    wavestream.Position = 44;
                    wavestream.Write(rawwave, 0, rawwave.Length);
                }
                finally
                {
                    if (wavestream != null)
                        wavestream.Close();
                }
            }

            Debug.WriteLine("StopRecording() - End - " + GC.GetTotalMemory(false));
        }
    }
}
