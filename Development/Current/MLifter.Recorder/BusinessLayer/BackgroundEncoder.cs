using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MLifter.DAL;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using System.ComponentModel;
using System.Windows.Forms;
using MLifter.AudioTools.Codecs;

namespace MLifter.AudioTools
{
    /// <summary>
    /// This class provides transparent audio encoding in the background.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-04-14</remarks>
    public class BackgroundEncoder
    {
        private Thread convertThread = null;
        private Queue<EncodeJob> jobQueue = new Queue<EncodeJob>();
        private bool stopThread = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundEncoder"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        public BackgroundEncoder()
        {
            convertThread = new Thread(new ThreadStart(EncodeThread));
            convertThread.IsBackground = true;
            convertThread.Priority = ThreadPriority.Lowest;
            convertThread.Start();
        }

        /// <summary>
        /// Stops the background encoder.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        public void StopBackgroundEncoder()
        {
            stopThread = true;
            lock (jobQueue)
                Monitor.Pulse(jobQueue);
        }

        /// <summary>
        /// Adds a encodeJob to the queue.
        /// </summary>
        /// <param name="encodeJob">The encodeJob.</param>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        public void AddJob(EncodeJob job)
        {
            lock (jobQueue)
            {
                jobQueue.Enqueue(job);
                Monitor.Pulse(jobQueue);
            }
        }

        /// <summary>
        /// The encoding thread.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        private void EncodeThread()
        {
            try
            {
                while (true)
                {
                    EncodeJob job = null;

                    lock (jobQueue)
                    {
                        while (jobQueue.Count == 0 && !stopThread)
                            Monitor.Wait(jobQueue);
                        if (!stopThread)
                            job = jobQueue.Dequeue();

                    }

                    if (stopThread)
                        break;

                    if (job != null)
                    {
                        //begin encoding process
                        EncodeMedia(job);
                        job.OnEncodingFinished(new EventArgs());
                    }
                }
            }
            catch (Exception exp)
            {
                if (!(exp is ThreadAbortException))
                    Trace.WriteLine("Encode Thread Exception: " + exp.ToString());
            }
        }

        /// <summary>
        /// Encodes the media.
        /// </summary>
        /// <param name="encodeJob">The encodeJob.</param>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        private void EncodeMedia(EncodeJob job)
        {
            FileInfo tempFile = null;
            FileInfo sourceFile = null;

            try
            {
                //check for audio object
                IAudio oldAudio = job.oldMedia as IAudio;
                if (oldAudio == null)
                    return;

                if (!job.codec.CanEncode)
                {
                    Trace.WriteLine("Selected codec is not able to encode: " + job.codec.EncodeError);
                    return;
                }

                //prepare encoding
                sourceFile = new FileInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + job.oldMedia.Extension));

                //write to temp file
                using (Stream mediaStream = job.oldMedia.Stream)
                {
                    using (Stream output = new FileStream(sourceFile.FullName, FileMode.Create))
                    {
                        byte[] buffer = new byte[32 * 1024];
                        int read;

                        while ((read = mediaStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, read);
                        }
                    }
                }

                tempFile = new FileInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + job.codec.extension));
                if (tempFile.Exists)
                    tempFile.Delete();

                //start encoding
                DateTime startencode = DateTime.Now;
                Trace.WriteLine("Starting encoding of " + sourceFile.Name + " to " + tempFile.Name);
                job.codec.Encode(sourceFile, tempFile, job.ShowWindow, job.MinimizeWindow);
                Trace.WriteLine("Encoding finished, duration " + (DateTime.Now - startencode).ToString());

                //generate and attach new media object
                tempFile.Refresh();
                if (tempFile.Exists)
                {
                    job.newMedia = job.card.CreateMedia(oldAudio.MediaType, tempFile.FullName,
                        oldAudio.Active.HasValue ? oldAudio.Active.Value : false,
                        oldAudio.Default.HasValue ? oldAudio.Default.Value : false,
                        oldAudio.Example.HasValue ? oldAudio.Example.Value : false);
                    job.card.RemoveMedia(job.oldMedia);
                    job.card.AddMedia(job.newMedia, job.side);
                }
                else
                {
                    Trace.WriteLine("Encoded audio file missing: " + tempFile.FullName);
                }
            }
            catch (Exception exp)
            {
                job.exception = exp;
            }
            finally
            {
                if (sourceFile != null && sourceFile.Exists)
                    sourceFile.Delete();
                if (tempFile != null && tempFile.Exists)
                    tempFile.Delete();
            }
        }
    }

    /// <summary>
    /// Represents an encode encodeJob.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-04-14</remarks>
    public class EncodeJob
    {
        public IMedia oldMedia = null;
        public IMedia newMedia = null;

        public Side side;
        public ICard card = null;

        /// <summary>
        /// The exception, if one happened, else null.
        /// </summary>
        public Exception exception = null;

        /// <summary>
        /// The codec to use.
        /// </summary>
        public Codec codec = null;

        public bool ShowWindow = true;
        public bool MinimizeWindow = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodeJob"/> class.
        /// </summary>
        /// <param name="oldMedia">The old media.</param>
        /// <param name="card">The card.</param>
        /// <param name="side">The side.</param>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        public EncodeJob(IMedia oldMedia, ICard card, Side side, Codec codec)
        {
            this.oldMedia = oldMedia;
            this.card = card;
            this.side = side;
            this.codec = codec;
        }

        /// <summary>
        /// Occurs when [encoding finished].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        public event EventHandler EncodingFinished;

        /// <summary>
        /// Raises the <see cref="E:EncodingFinished"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-04-14</remarks>
        public void OnEncodingFinished(EventArgs e)
        {
            EventHandler Handler = EncodingFinished;
            if (Handler != null)
            {
                foreach (EventHandler Caster in Handler.GetInvocationList())
                {
                    ISynchronizeInvoke SyncInvoke = Caster.Target as ISynchronizeInvoke;
                    try
                    {
                        if (SyncInvoke != null && SyncInvoke.InvokeRequired)
                            SyncInvoke.Invoke(Handler, new object[] { this, e });
                        else
                            Caster(this, e);
                    }
                    catch
                    { }
                }
            }
        }
    }
}
